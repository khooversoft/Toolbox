// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Khooversoft.Sql
{
    /// <summary>
    /// SQL configuration (immutable builder pattern)
    /// </summary>
    [DebuggerDisplay("ConnectionString={ConnectionString}")]
    public class SqlConfiguration : ISqlConfiguration
    {
        private static readonly Tag _tag = new Tag(nameof(SqlConfiguration));

        public SqlConfiguration(string connectionString)
        {
            Verify.IsNotEmpty(nameof(connectionString), connectionString);

            ConnectionString = connectionString;
        }

        public SqlConfiguration(string server, string databaseName)
        {
            Verify.IsNotEmpty(nameof(server), server);
            Verify.IsNotEmpty(nameof(databaseName), databaseName);

            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = server,
                InitialCatalog = databaseName,
                IntegratedSecurity = true
            };

            ConnectionString = builder.ToString();
        }

        public SqlConfiguration(string server, string databaseName, string userId, string password)
        {
            Verify.IsNotEmpty(nameof(server), server);
            Verify.IsNotEmpty(nameof(databaseName), databaseName);
            Verify.IsNotEmpty(nameof(userId), userId);

            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = server,
                InitialCatalog = databaseName,
                UserID = userId,
                Password = password,
            };

            ConnectionString = builder.ToString();
        }

        public SqlConfiguration(SqlConnectionStringBuilder builder)
        {
            Verify.IsNotNull(nameof(builder), builder);

            ConnectionString = builder.ToString();
        }

        /// <summary>
        /// SQL connection string
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Test the DB connection by executing "select 1"
        /// </summary>
        /// <returns>true if pass, false if not</returns>
        public bool TestConnection(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select 1";
                    cmd.CommandType = CommandType.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    SqlEventSource.Log.Verbose(context, $"Database connect test passed - connection string: {ConnectionString}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SqlEventSource.Log.Error(context, $"Cannot open database - connection string: {ConnectionString}", ex);
                return false;
            }
        }
    }
}
