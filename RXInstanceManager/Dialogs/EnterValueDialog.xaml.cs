using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RXInstanceManager
{
    /// <summary>
    /// Логика взаимодействия для EnterValueDialog.xaml
    /// </summary>
    public partial class EnterValueDialog : Window
    {
        public string Value { get; set; }
        public string EmptyValue { get; set; }

        public EnterValueDialog(string emptyValue, string value)
        {
            InitializeComponent();

            ButtonSelect.Visibility = Visibility.Hidden;

            if (value != null)
            {
                this.Value = value;
                Input.Text = this.Value;
            }
            else
            {
                this.EmptyValue = emptyValue;
                Input.Text = emptyValue;
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter))
            {
                if (!string.IsNullOrWhiteSpace(Input.Text))
                {
                    this.Value = Input.Text;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                    MessageBox.Show("Необходимо указать значение.");
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            EditStyleChanging(Input, this.EmptyValue);
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Input.Text == this.EmptyValue)
                Input.Text = string.Empty;
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Input.Text))
                Input.Text = this.EmptyValue;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Input.Text))
            {
                this.Value = Input.Text;
                this.DialogResult = true;
                this.Close();
            }
            else
                MessageBox.Show("Необходимо указать значение.");
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Для реализации логики выбора.
        }

        private void EditStyleChanging(TextBox textBox, string emptyValue)
        {
            textBox.Style = (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == emptyValue) ?
                this.FindResource("EditTextBoxEmpty") as Style :
                this.FindResource("EditTextBoxEdit") as Style;
        }
    }
}
