using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Sql
{
    /// <summary>
    /// SQL Execute, primary abstraction for ADO.NET with strong contracts
    /// 
    /// If a deadlock is detected, the sql command will be retried n times with a random back off delay between 10 and 1000 ms.
    /// </summary>
    public class SqlExec
    {
        private static readonly Tag _tag = new Tag(nameof(SqlExec));
        private static readonly Random _random = new Random();
        private const int _retryCount = 5;
        private const int _deadLockNumber = 1205;
        private const string _deadLockMessage = "Deadlock retry failed";

        public SqlExec(ISqlConfiguration configuration)
        {
            Verify.IsNotNull(nameof(configuration), configuration);

            Configuration = configuration;
        }

        /// <summary>
        /// SQL configuration
        /// </summary>
        public ISqlConfiguration Configuration { get; }

        /// <summary>
        /// SQL command
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// SQL command type
        /// </summary>
        public CommandType CommandType { get; private set; } = CommandType.StoredProcedure;

        /// <summary>
        /// Parameters
        /// </summary>
        public IList<ISqlParameter> Parameters { get; } = new List<ISqlParameter>();

        /// <summary>
        /// Set SQL command
        /// </summary>
        /// <param name="command">SQL command to use</param>
        /// <returns>this</returns>
        public SqlExec SetCommand(string command, CommandType commandType = CommandType.StoredProcedure)
        {
            Verify.IsNotEmpty(nameof(command), command);

            Command = command;
            CommandType = commandType;

            return this;
        }

        /// <summary>
        /// Set parameter
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">value</param>
        /// <param name="addValueIfNull">Add value if null</param>
        /// <returns>this</returns>
        public SqlExec AddParameter<T>(string name, T value, bool addValueIfNull = false)
        {
            Verify.IsNotEmpty(nameof(name), name);

            if (!addValueIfNull && value == null)
            {
                return this;
            }

            if (typeof(T).IsEnum)
            {
                Parameters.Add(new SqlSimpleParameter(name, value.ToString()));
                return this;
            }

            // Handle custom types
            var objectValue = value as IObjectValue<object>;
            if (objectValue != null)
            {
                object objValue = objectValue.GetObjectValue();
                if (objValue != null)
                {
                    Parameters.Add(new SqlSimpleParameter(name, objValue));
                }

                return this;
            }

            Parameters.Add(new SqlSimpleParameter(name, value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public SqlExec AddParameter(ISqlParameter parameter)
        {
            Verify.IsNotNull(nameof(parameter), parameter);

            Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Execute None SQL query (no response)
        /// </summary>
        /// <param name="context">execution context</param>
        /// <returns>task</returns>
        public async Task ExecuteNonQuery(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            context = context.WithTag(_tag);
            SqlException saveEx = null;

            using (var conn = new SqlConnection(Configuration.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;
                cmd.CommandType = CommandType;
                cmd.Parameters.AddRange(Parameters.Select(x => x.ToSqlParameter()).ToArray());

                conn.Open();

                for (int retry = 0; retry < _retryCount; retry++)
                {
                    try
                    {
                        using (var scope = new ActivityScope(context, Command))
                        {
                            await cmd.ExecuteNonQueryAsync();
                            return;
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == _deadLockNumber)
                        {
                            saveEx = sqlEx;
                            Thread.Sleep(TimeSpan.FromMilliseconds(_random.Next(10, 1000)));
                            continue;
                        }

                        ToolboxEventSource.Log.Error(context, sqlEx.Message, sqlEx);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ToolboxEventSource.Log.Error(context, ex.Message, ex);
                        throw new WorkException(ex.Message, context);
                    }
                }
            }
        }

        /// <summary>
        /// Execute SQL and return data set
        /// </summary>
        /// <typeparam name="T">type to return</typeparam>
        /// <param name="context">execution context</param>
        /// <param name="factory">type factor</param>
        /// <returns>list of types</returns>
        public async Task<IList<T>> Execute<T>(IWorkContext context, Func<IWorkContext, SqlDataReader, T> factory)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(factory), factory);
            context = context.WithTag(_tag);

            return await ExecuteBatch(context, (c, r) => r.GetCollection(c, factory));
        }

        /// <summary>
        /// Execute SQL and return results in DataTable
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>data table</returns>
        public async Task<DataTable> ExecuteDataTable(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            return await ExecuteBatch<DataTable>(context, (c, r) =>
            {
                var dt = new DataTable();
                dt.Load(r);
                return dt;
            });
        }

        /// <summary>
        /// Execute SQL and batch object
        /// </summary>
        /// <typeparam name="T">type to return</typeparam>
        /// <param name="context">execution context</param>
        /// <param name="factory">batch type factor</param>
        /// <returns>list of types</returns>
        public async Task<T> ExecuteBatch<T>(IWorkContext context, Func<IWorkContext, SqlDataReader, T> factory)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(factory), factory);
            context = context.WithTag(_tag);

            SqlException saveEx = null;

            using (var conn = new SqlConnection(Configuration.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;
                cmd.CommandType = CommandType;
                cmd.Parameters.AddRange(Parameters.Select(x => x.ToSqlParameter()).ToArray());

                conn.Open();

                for (int retry = 0; retry < _retryCount; retry++)
                {
                    try
                    {
                        using (var scope = new ActivityScope(context, Command, ToolboxEventSource.Log))
                        {
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                return factory(context, reader);
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == _deadLockNumber)
                        {
                            saveEx = sqlEx;
                            await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(10, 1000)));
                            continue;
                        }

                        ToolboxEventSource.Log.Error(context, sqlEx.Message, sqlEx);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ToolboxEventSource.Log.Error(context, ex.Message, ex);
                        throw;
                    }
                }
            }

            throw new WorkException(_deadLockMessage, context, saveEx);
        }
    }
}
