// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class DatabaseConfigurationBuilder
    {
        private const string _mongoUrlKey = "MongoUrl";
        private const string _databaseKey = "Database";

        public DatabaseConfigurationBuilder()
        {
        }

        public DatabaseConfigurationBuilder(string connectionString, string databaseName)
        {
            Verify.IsNotEmpty(nameof(connectionString), connectionString);
            Verify.IsNotEmpty(nameof(databaseName), databaseName);

            Url = new MongoUrl(connectionString);
            DatabaseName = databaseName;
        }

        public DatabaseConfigurationBuilder(string connectionString)
        {
            Verify.IsNotEmpty(nameof(connectionString), connectionString);

            var dict = connectionString.ParsePropertyString();
            Verify.Assert(dict.ContainsKey(_mongoUrlKey), $"{_mongoUrlKey} not found in connection string");
            Verify.Assert(dict.ContainsKey(_databaseKey), $"{_databaseKey} not found in connection string");

            Url = new MongoUrl(dict[_mongoUrlKey]);
            DatabaseName = dict[_databaseKey];
        }

        public MongoUrl Url { get; set; }

        public string DatabaseName { get; set; }

        public DatabaseConfigurationBuilder SetUrl(MongoUrl url)
        {
            Verify.IsNotNull(nameof(url), url);

            Url = url;
            return this;
        }

        public DatabaseConfigurationBuilder SetDatabaseName(string databaseName)
        {
            Verify.IsNotEmpty(nameof(databaseName), databaseName);

            DatabaseName = databaseName;
            return this;
        }

        public override string ToString()
        {
            Verify.IsNotNull(nameof(Url), Url);
            Verify.IsNotEmpty(nameof(DatabaseName), DatabaseName);

            var dict = new Dictionary<string, string>
            {
                [_mongoUrlKey] = Url.ToString(),
                [_databaseKey] = DatabaseName,
            };

            return dict.ToPropertyString();
        }
    }
}
