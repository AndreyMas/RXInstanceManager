﻿using System;
using System.IO;
using System.Linq;
using System.Net;

namespace RXInstanceManager
{
    public static class AppHelper
    {
        public static bool PingURL(string url)
        {
            Uri uri = new Uri(url);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string Base64EncodeFromUTF8(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64EncodeFromASCII(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.ASCII.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Encode(byte[] plainText)
        {
            return System.Convert.ToBase64String(plainText);
        }

        public static byte[] Base64Decode(string base64EncodedData)
        {
            return System.Convert.FromBase64String(base64EncodedData);
        }

        public static string Base64DecodeToUTF8(string base64EncodedData)
        {
            return System.Text.Encoding.UTF8.GetString(Base64Decode(base64EncodedData));
        }

        public static string Base64DecodeToASCII(string base64EncodedData)
        {
            return System.Text.Encoding.ASCII.GetString(Base64Decode(base64EncodedData));
        }

        public static bool ValidateInputCode(string code)
        {
            return code.Length > 3 && code.Length <= 10 && code.All(x => (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z') || (x >= '0' && x <= '9'));
        }

        public static bool ValidateInputDBName(string name)
        {
            return name.Length >= 3 && name.Length <= 25 && name.All(x => (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z') || (x >= '0' && x <= '9') || (x == '_'));
        }

        public static bool ValidateInputPort(string port)
        {
            return port.Length <= 10 && port.All(x => (x >= '0' && x <= '9'));
        }

        public static string GetDirectumLauncherPath(string instancePath)
        {
            return Path.Combine(instancePath, "DirectumLauncher.exe");
        }

        public static string GetConfigYamlPath(string instancePath)
        {
            return Path.Combine(instancePath, "etc", "config.yml");
        }

        public static string GetConfigYamlExamplePath(string instancePath)
        {
            return Path.Combine(instancePath, "etc", "config.yml.example");
        }

        public static string GetBuildsPath(string instancePath)
        {
            return Path.Combine(instancePath, "etc", "_builds");
        }

        public static string GetBuildsBinPath(string instancePath)
        {
            return Path.Combine(instancePath, "etc", "_builds_bin");
        }

        public static string GetPlatformBuildsPath(string instancePath)
        {
            var path = Path.Combine(instancePath, "etc", "_builds", "PlatformBuilds");
            if (Directory.Exists(path))
                return path;

            return Path.Combine(instancePath, "etc", "_builds", "Platform");
        }

        public static string GetDirectumRXBuildsPath(string instancePath)
        {
            return Path.Combine(instancePath, "etc", "_builds", "DirectumRX");
        }

        public static string GetDDSPath(string instancePath)
        {
            return Path.Combine(instancePath, "etc", "_builds", "DevelopmentStudio", "bin", "DevelopmentStudio.exe");
        }

        public static string GetClientURL(string protocol, string host, int port)
        {
            return $"{protocol}://{host}:{port}/Client";
        }

        public static string GetLogsPath(string storagePath, string logsPath)
        {
            return logsPath
                .Replace("{{ home_path }}", storagePath)
                .Replace("{{home_path}}", storagePath);
        }

        public static string GetSourcesPath(string storagePath, string sourcesPath)
        {
            return sourcesPath
                .Replace("{{ home_path }}", storagePath)
                .Replace("{{home_path}}", storagePath);
        }

        public static string GetDBName(string dbName, string dbNameFromVar)
        {
            if (string.IsNullOrEmpty(dbNameFromVar))
                return dbName;

            return dbName
                .Replace("{{ database }}", dbNameFromVar)
                .Replace("{{database}}", dbNameFromVar);
        }

        public static string GetDBNameFromConnectionString(string engine, string connectionString, string dbNameFromVar)
        {
            if (engine == "mssql")
            {
                var databaseNameParam = connectionString.Split(';').FirstOrDefault(x => x.ToLower().Contains("initial catalog"));
                if (databaseNameParam != null)
                    return GetDBName(databaseNameParam.Split('=')[1], dbNameFromVar);
            }

            if (engine == "postgres")
            {
                var databaseNameParam = connectionString.Split(';').FirstOrDefault(x => x.ToLower().Contains("database"));
                if (databaseNameParam != null)
                    return GetDBName(databaseNameParam.Split('=')[1], dbNameFromVar);
            }

            return null;
        }

        public static bool CheckInstance(string url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static DateTime GetFileChangeTime(string path)
        {
            if (!File.Exists(path))
                return DateTime.MinValue;

            return File.GetLastWriteTime(path);
        }

        public static bool EqualsUpToSeconds(this DateTime dt1, DateTime dt2)
        {
            var date1 = dt1.Date.AddHours(dt1.Hour).AddMinutes(dt1.Minute).AddSeconds(dt1.Second);
            var date2 = dt2.Date.AddHours(dt2.Hour).AddMinutes(dt2.Minute).AddSeconds(dt2.Second);
            return date1.Equals(date2);
        }

        public static bool LessThanUpToSeconds(this DateTime dt1, DateTime dt2)
        {
            var date1 = dt1.Date.AddHours(dt1.Hour).AddMinutes(dt1.Minute).AddSeconds(dt1.Second);
            var date2 = dt2.Date.AddHours(dt2.Hour).AddMinutes(dt2.Minute).AddSeconds(dt2.Second);
            return date1 < date2;
        }

        public static bool MoreThanUpToSeconds(this DateTime dt1, DateTime dt2)
        {
            var date1 = dt1.Date.AddHours(dt1.Hour).AddMinutes(dt1.Minute).AddSeconds(dt1.Second);
            var date2 = dt2.Date.AddHours(dt2.Hour).AddMinutes(dt2.Minute).AddSeconds(dt2.Second);
            return date1 > date2;
        }
    }
}
