using System;
using System.Text;
using SQLQueryGen;

namespace RXInstanceManager
{
    [Table("instance")]
    public class Instance
    {
        [Field("id", Key = true)]
        public int Id { get; set; }

        [Field("code", Size = 10)]
        public string Code { get; set; }

        [Field("platform", Size = 20)]
        public string PlatformVersion { get; set; }

        [Field("sungero", Size = 20)]
        public string SolutionVersion { get; set; }

        [Field("name", Size = 100)]
        public string Name { get; set; }

        [Field("port")]
        public int Port { get; set; }

        [Field("url", Size = 100)]
        public string URL { get; set; }

        [Field("dbname", Size = 100)]
        public string DBName { get; set; }

        [Field("service", Size = 50)]
        public string ServiceName { get; set; }

        [Field("instance", Size = 100)]
        public string InstancePath { get; set; }

        [Field("storage", Size = 150)]
        public string StoragePath { get; set; }

        [Field("sources", Size = 150)]
        public string SourcesPath { get; set; }

        [Field("project", Size = 150)]
        public string ProjectPath { get; set; }

        [Field("logs", Size = 150)]
        public string LogsPath { get; set; }

        [Field("status", Size = 12)]
        public string Status { get; set; }

        [Field("config")]
        [Navigate(TableName = "config", FieldName = "id", Required = false)]
        public Config Config { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Код:                " + Code ?? string.Empty);
            builder.AppendLine("Назначениие:        " + Name ?? string.Empty);
            builder.AppendLine("URL:                " + URL ?? string.Empty);
            builder.AppendLine("Версия платформы:   " + PlatformVersion ?? string.Empty);
            builder.AppendLine("Версия решения:     " + SolutionVersion ?? string.Empty);
            builder.AppendLine("Путь до инстанса:   " + InstancePath ?? string.Empty);
            builder.AppendLine("Путь до хранилища:  " + StoragePath ?? string.Empty);
            builder.AppendLine("Путь до исходников: " + SourcesPath ?? string.Empty);
            builder.AppendLine("Путь до логов:      " + LogsPath ?? string.Empty);
            builder.AppendLine("Путь до проекта:    " + ProjectPath ?? string.Empty);
            builder.AppendLine("Имя службы:         " + ServiceName ?? string.Empty);
            builder.AppendLine("Имя БД:             " + DBName ?? string.Empty);
            return builder.ToString();
        }
    }
}
