using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Khooversoft.Sql
{
    /// <summary>
    /// SQL Execute, primary abstraction for ADO.NET with strong contracts
    /// </summary>
    public class SqlExec
    {
        private static readonly Tag _tag = new Tag(nameof(SqlExec));

        public SqlExec(SqlConfiguration configuration)
        {
            Verify.IsNotNull(nameof(configuration), configuration);

            Configuration = configuration;
        }

        /// <summary>
        /// SQL configuration
        /// </summary>
        public SqlConfiguration Configuration { get; }

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

            if (value == null)
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
        public void ExecuteNonQuery(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            context = context.WithTag(_tag);

            using (var conn = new SqlConnection(Configuration.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;
                cmd.CommandType = CommandType;
                cmd.Parameters.AddRange(Parameters.Select(x => x.ToSqlParameter()).ToArray());

                conn.Open();

                try
                {
                    using (var scope = new ActivityScope(context, Command))
                    {
                        cmd.ExecuteNonQuery();
                        return;
                    }
                }
                catch (SqlException sqlEx)
                {
                    Exception ex = ToException(context, sqlEx);
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw new WorkException(ex.Message, context);
                }
            }
        }

        /// <summary>
        /// Execute None SQL query (no response)
        /// </summary>
        /// <param name="context">execution context</param>
        /// <returns>task</returns>
        public async Task ExecuteNonQueryAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            context = context.WithTag(_tag);

            using (var conn = new SqlConnection(Configuration.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;
                cmd.CommandType = CommandType;
                cmd.Parameters.AddRange(Parameters.Select(x => x.ToSqlParameter()).ToArray());

                conn.Open();

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
                    Exception newEx = ToException(context, sqlEx);
                    ToolboxEventSource.Log.Error(context, newEx.Message, newEx);
                    throw newEx;
                }
                catch (Exception ex)
                {
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw new WorkException(ex.Message, context);
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
        public IList<T> Exceute<T>(IWorkContext context, Func<SqlDataReader, T> factory)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(factory), factory);

            context = context.WithTag(_tag);
            IList<T> list = new List<T>();

            using (var conn = new SqlConnection(Configuration.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;
                cmd.CommandType = CommandType;
                cmd.Parameters.AddRange(Parameters.Select(x => x.ToSqlParameter()).ToArray());

                conn.Open();

                SqlDataReader reader;
                try
                {
                    using (var scope = new ActivityScope(context, Command))
                    {
                        reader = cmd.ExecuteReader();
                    }
                }
                catch (SqlException sqlEx)
                {
                    Exception ex = ToException(context, sqlEx);
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw new WorkException(ex.Message, context);
                }

                while (reader.Read())
                {
                    list.Add(factory(reader));
                }
            }

            return list;
        }

        /// <summary>
        /// Execute SQL and return data set
        /// </summary>
        /// <typeparam name="T">type to return</typeparam>
        /// <param name="context">execution context</param>
        /// <param name="factory">type factor</param>
        /// <returns>list of types</returns>
        public async Task<IList<T>> ExceuteAsync<T>(IWorkContext context, Func<IWorkContext, SqlDataReader, T> factory)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(factory), factory);

            context = context.WithTag(_tag);
            IList<T> list = new List<T>();

            using (var conn = new SqlConnection(Configuration.ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Command;
                cmd.CommandType = CommandType;
                cmd.Parameters.AddRange(Parameters.Select(x => x.ToSqlParameter()).ToArray());

                conn.Open();

                SqlDataReader reader;
                try
                {
                    using (var scope = new ActivityScope(context, Command))
                    {
                        reader = await cmd.ExecuteReaderAsync();
                    }
                }
                catch (SqlException sqlEx)
                {
                    Exception ex = ToException(context, sqlEx);
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    ToolboxEventSource.Log.Error(context, ex.Message, ex);
                    throw new WorkException(ex.Message, context);
                }

                while (reader.Read())
                {
                    list.Add(factory(context, reader));
                }
            }

            return list;
        }

        /// <summary>
        /// Return the first row of the dataset or none if there is no response
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="context">execution context</param>
        /// <param name="factory">type factor</param>
        /// <returns>type or null</returns>
        public T ExecuteFirstOrDefault<T>(IWorkContext context, Func<SqlDataReader, T> factory)
        {
            context.WithTag(_tag);

            IList<T> result = Exceute(context, factory);
            return result.Count == 0 ? default(T) : result[0];
        }

        /// <summary>
        /// Return the first row of the dataset or none if there is no response
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="context">execution context</param>
        /// <param name="factory">type factor</param>
        /// <returns>type or null</returns>
        public async Task<T> ExecuteFirstOrDefaultAsync<T>(IWorkContext context, Func<IWorkContext, SqlDataReader, T> factory)
        {
            context = context.WithTag(_tag);

            IList<T> result = await ExceuteAsync(context, factory);
            return result.Count == 0 ? default(T) : result[0];
        }

        /// <summary>
        /// Convert SQL Exception to standard exceptions, if possible
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="sqlEx">sql exception</param>
        /// <returns>exception</returns>
        private Exception ToException(IWorkContext context, SqlException sqlEx)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(sqlEx), sqlEx);

            List<SqlError> list = new List<SqlError>(Enumerable.Range(0, sqlEx.Errors.Count).Select(x => sqlEx.Errors[x]));

            if (list.Any(x => x.Class == 17 && x.State == 2))
            {
                return new ETagException(sqlEx.Message, context);
            }

            return new WorkException(sqlEx.Message, context);
        }
    }
}
