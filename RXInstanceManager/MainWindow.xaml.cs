using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;

namespace RXInstanceManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static Instance _instance;

        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(Constants.LogPath))
                Directory.CreateDirectory(Constants.LogPath);

            DBInitializer.Initialize();
            Instances.Create();
            Configs.Create();

            ActionButtonVisibleChanging();
            LoadInstances();
            StartAsyncHandlers();
        }

        private void GridInstances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var instance = GridInstances.SelectedItem as Instance;
            if (instance != null)
            {
                _instance = instance;
                ActionButtonVisibleChanging(instance.Status);
            }
        }

        #region ActionHandlers

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                var serviceName = _instance.ServiceName;
                using (var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, false))
                {
                    if (regKey != null && (int)regKey.GetValue("Start") != 3)
                        AppHandlers.ExecuteCmdCommands(true, false,
                            @"REG ADD HKLM\SYSTEM\CurrentControlSet\Services\" + serviceName + " /v Start /t REG_DWORD /d 3 /f",
                            @"sc start " + serviceName);
                    else
                        AppHandlers.ExecuteCmdCommands(true, false, @"sc start " + serviceName);
                }
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                var serviceName = _instance.ServiceName;
                using (var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, false))
                {
                    if (regKey != null && (int)regKey.GetValue("Start") != 3)
                        AppHandlers.ExecuteCmdCommands(true, false,
                            @"REG ADD HKLM\SYSTEM\CurrentControlSet\Services\" + serviceName + " /v Start /t REG_DWORD /d 3 /f",
                            @"sc stop " + serviceName);
                    else
                        AppHandlers.ExecuteCmdCommands(true, false, @"sc stop " + serviceName);
                }
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            var instancePath = Dialogs.ShowEnterValueDialog("Укажите путь до инстанса");
            if (instancePath == null)
                return;

            var isValid = ValidateBeforeAddInstance(instancePath);
            if (!isValid)
                return;

            Instance instance;
            try
            {
                var config = AppHandlers.GetInstanceConfig(instancePath);
                var yamlValues = YamlSimple.Parser.Parse(config.Body);

                var instanceCode = yamlValues.GetConfigStringValue("variables.instance_name");
                if (string.IsNullOrEmpty(instanceCode))
                {
                    instanceCode = Dialogs.ShowEnterValueDialog("Укажите код системы");
                    if (string.IsNullOrEmpty(instanceCode))
                        return;

                    if (!AppHelper.ValidateInputCode(instanceCode))
                    {
                        MessageBox.Show("Код должен быть более от 4 до 10 символов английского алфавита в нижнем регистре и цифр");
                        return;
                    }
                }

                instance = Instances.Get().FirstOrDefault(x => x.Code == instanceCode);
                if (instance != null)
                {
                    MessageBox.Show($"Экземпляр DirectumRX с кодом \"{instanceCode}\" уже добавлен");
                    LoadInstances(instance);
                    return;
                }

                AppHandlers.SetConfigStringValue(config, "variables.instance_name", instanceCode);

                instance = new Instance();
                instance.Code = instanceCode;
                instance.InstancePath = instancePath;
                instance.ServiceName = $"{Constants.Service}_{instanceCode}";
                instance.Status = Constants.InstanceStatus.NeedInstall;
                instance.Config = config;
                AppHandlers.UpdateInstanceData(instance);

                if (config.Instance == null)
                {
                    config.Instance = instance;
                    config.Save();
                }

                _instance = instance;
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(null, ex);
            }

            LoadInstances(_instance);
        }

        private void ButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            if (_instance == null || _instance.Status != Constants.InstanceStatus.NeedInstall)
                return;

            var config = AppHandlers.GetInstanceConfig(_instance.InstancePath);
            var yamlValues = YamlSimple.Parser.Parse(config.Body);

            var isValid = ValidateBeforeInstallInstance(yamlValues);
            if (!isValid)
                return;

            try
            {
                var serviceStatus = AppHandlers.GetServiceStatus(_instance);
                if (serviceStatus == Constants.InstanceStatus.NeedInstall)
                    AppHandlers.LaunchProcess(AppHelper.GetDirectumLauncherPath(_instance.InstancePath));
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {

            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                var acceptResult = MessageBox.Show($"Подтвердите удаление экземпляра \"{_instance.Code}\"",
                                                   "Подтверждение удаления", MessageBoxButton.YesNo);

                if (acceptResult != MessageBoxResult.Yes)
                    return;

                if (Instances.Get().Count() == 1)
                {
                    acceptResult = MessageBox.Show("Вы удаляете последний экземпляр системы. Продолжить?",
                                                   "Подтверждение удаления", MessageBoxButton.YesNo);

                    if (acceptResult != MessageBoxResult.Yes)
                        return;
                }

                var serviceStatus = AppHandlers.GetServiceStatus(_instance);
                if (serviceStatus != Constants.InstanceStatus.NeedInstall)
                    AppHandlers.ExecuteDoCommands(_instance.InstancePath, "do all down");

                var removeFolderCommands = new List<string>();
                if (!string.IsNullOrEmpty(_instance.StoragePath) && Directory.Exists(_instance.StoragePath))
                    removeFolderCommands.Add("rmdir /s /q \"" + _instance.StoragePath + "\"");
                if (!string.IsNullOrEmpty(_instance.SourcesPath) && Directory.Exists(_instance.SourcesPath))
                    removeFolderCommands.Add("rmdir /s /q \"" + _instance.SourcesPath + "\"");
                if (!string.IsNullOrEmpty(_instance.InstancePath) && Directory.Exists(_instance.InstancePath))
                    removeFolderCommands.Add("rmdir /s /q \"" + _instance.InstancePath + "\"");
                if (Directory.Exists(@"C:\inetpub\DirectumRX Web Site_" + _instance.Code))
                    removeFolderCommands.Add("rmdir /s /q \"C:\\inetpub\\DirectumRX Web Site_" + _instance.Code + "\"");

                if (removeFolderCommands.Any())
                    AppHandlers.ExecuteCmdCommands(true, false, removeFolderCommands.ToArray());

                if (_instance.Config != null)
                    Configs.Delete(_instance.Config);

                Instances.Delete(_instance);

                LoadInstances();
                ActionButtonVisibleChanging();
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonDDSStart_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                if (string.IsNullOrEmpty(_instance.StoragePath))
                {
                    MessageBox.Show("Не указана папка исходников");
                    return;
                }

                AppHandlers.LaunchProcess(AppHelper.GetDDSPath(_instance.InstancePath), true);
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonRXStart_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                if (!string.IsNullOrEmpty(_instance.URL))
                    AppHandlers.LaunchProcess(_instance.URL);
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void ButtonInstruction_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                if (File.Exists("readme.txt"))
                    Dialogs.ShowFileContentDialog("readme.txt");
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        #endregion

        #region ContextHandlers

        private void ConfigContext_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                var configYamlPath = AppHelper.GetConfigYamlPath(_instance.InstancePath);
                if (File.Exists(configYamlPath))
                    AppHandlers.LaunchProcess(AppHelper.GetConfigYamlPath(_instance.InstancePath));
                else
                    MessageBox.Show("Конфигурационный файл не найден");
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void RestartContext_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            try
            {
                AppHandlers.ExecuteDoCommands(_instance.InstancePath, "do all config_up", "do dds config_up", "do all up",
                    @"REG ADD HKLM\SYSTEM\CurrentControlSet\Services\" + _instance.ServiceName + " /v Start /t REG_DWORD /d 3 /f");
                AppHandlers.UpdateInstanceData(_instance);
                LoadInstances(_instance);
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void CmdContext_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            if (_instance == null)
                return;
            try
            {
                AppHandlers.ExecuteCmdCommand($"cd {_instance.InstancePath}", false);
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void CmdAdminContext_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            if (_instance == null)
                return;

            try
            {
                AppHandlers.ExecuteCmdCommand($"cd {_instance.InstancePath}", true);
            }
            catch (Exception ex)
            {
                AppHandlers.ErrorHandler(_instance, ex);
            }
        }

        private void InfoContext_Click(object sender, RoutedEventArgs e)
        {
            AppHandlers.InfoHandler(_instance, MethodBase.GetCurrentMethod().Name);

            if (_instance == null)
                return;

            Dialogs.ShowInformationDialog(_instance.ToString());
        }

        #endregion

        private void StartAsyncHandlers()
        {
#pragma warning disable CS4014
            // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            UpdateInstanceGridAsync();
            UpdateInstanceDataAsync();
#pragma warning restore CS4014
        }

        private void HiddenButton_Click(object sender, RoutedEventArgs e)
        {
            //var process = new Process();
            //process.StartInfo.FileName = Path.Combine(_instance.InstancePath, "DirectumLauncher.exe");
            //process.Start();

            //var process = new Process();
            //process.StartInfo.FileName = "cmd";
            //process.StartInfo.Arguments = "/user:Administrator \"cmd /K cd " + _instance.InstancePath + "\"";
            //process.Start();

            //var process = new Process();
            //var startInfo = new ProcessStartInfo();
            //startInfo.FileName = "cmd.exe";
            //startInfo.Arguments = "\"cmd /K cd " + _instance.InstancePath + "\"";
            //startInfo.Verb = "runas";
            //process.StartInfo = startInfo;
            //process.Start();

            //var content = File.ReadAllText(@"C:\Temp\config.yml");
            //var yaml = new YamlParser(content);

            //MessageBox.Show(AppHandlers.GetInstanceSolutionVersion(_instance.InstancePath));

            AppHandlers.SetConfigStringValue(_instance.Config, "variables.instance_name", "test");
        }


    }
}
