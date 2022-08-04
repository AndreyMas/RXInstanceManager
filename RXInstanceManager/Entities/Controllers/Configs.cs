﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using Dapper;
using SQLQueryGen;

namespace RXInstanceManager
{
    public static class Configs
    {
        #region Save.

        public static void Save(this Config config)
        {
            config.Body = File.ReadAllText(config.Path);

            if (config.Id > 0)
                Update(config);
            else
                Insert(config);
        }

        private static void Insert(Config config)
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                config.Id = connection.QuerySingle<int>(QueryGenerator.GenerateInsertQuery<Config>(config));
            }
        }

        private static void Update(Config config)
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                connection.Execute(QueryGenerator.GenerateUpdateQuery<Config>(config));
            }
        }

        #endregion

        #region Get.

        public static List<Config> Get()
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                return connection.Query<Config>(QueryGenerator.GenerateSelectQuery<Config>()).ToList();
            }
        }

        public static List<Config> GetLessOrEqualVersion(string version)
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                var addWhere = new AddWhere<Config>("version", QueryConstants.Expression.LessOrEqual, version);
                return connection.Query<Config>(QueryGenerator.GenerateSelectQuery<Config>(addWhere)).ToList();
            }
        }

        public static List<Config> GetGreaterOrEqualVersion(string version)
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                var addWhere = new AddWhere<Config>("version", QueryConstants.Expression.GreaterOrEqual, version);
                return connection.Query<Config>(QueryGenerator.GenerateSelectQuery<Config>(addWhere)).ToList();
            }
        }

        #endregion

        #region Delete.

        public static void Delete(Config config)
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                connection.Execute(QueryGenerator.GenerateDeleteQuery<Config>(config));
                config = null;
            }
        }

        #endregion

        #region Create.

        public static void Create()
        {
            using (var connection = new SQLiteConnection(DBInitializer.ConnectionString))
            {
                connection.Execute(QueryGenerator.GenerateCreateQuery<Config>());
            }
        }

        #endregion
    }
}