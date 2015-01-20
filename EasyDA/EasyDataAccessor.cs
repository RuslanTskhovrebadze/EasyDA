using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EasyDA
{
	public class EasyDataAccessor<T> : IDisposable where T : IDbConnection
	{

		#region Constructors
		public EasyDataAccessor()
		{
			Settings = new EasyDataAccessorSettings();
		}

		public EasyDataAccessor(string connectionString, CommandType? commandType, int? connectionTimeout)
		{
			Settings = new EasyDataAccessorSettings();
			Settings.ConnectionString = connectionString;
			Settings.ConnectionTimeout = connectionTimeout.Value;
			Settings.ProviderCommandType = commandType ?? CommandType.Text;
			Settings.DbConnectionType = typeof(T);
		}

		internal EasyDataAccessor(string connectionString, CommandType? commandType, int? connectionTimeout, Type dbConnectionType)
		{
			Settings = new EasyDataAccessorSettings();
			Settings.ConnectionString = connectionString;
			Settings.ConnectionTimeout = connectionTimeout.Value;
			Settings.ProviderCommandType = commandType ?? CommandType.Text;
			Settings.DbConnectionType = dbConnectionType;
		}
		#endregion

		public EasyDataAccessorSettings Settings { get; set; }

		protected virtual IDbConnection CreateConnection()
		{
			return (IDbConnection)Activator.CreateInstance(Settings.DbConnectionType);
		}

		protected virtual IDbCommand GetPreparedCommand(string commandText, object parameters, CommandType? commandType)
		{
			if (string.IsNullOrWhiteSpace(commandText))
			{
				throw new ArgumentException("Command text is null or empty", "commandText");
			}

			IDbConnection conn;
			if (IsTransactionStarted)
			{
				conn = Transaction.Connection;
			}
			else
			{
				conn = CreateConnection();
				conn.ConnectionString = Settings.ConnectionString;
			}

			IDbCommand command = conn.CreateCommand();
			command.Connection = conn;
			command.CommandText = commandText;
			command.CommandTimeout = Settings.ConnectionTimeout;
			command.CommandType = commandType.HasValue ? commandType.Value : Settings.ProviderCommandType;
			command.Transaction = Transaction;
			FillCommandParameters(command, parameters);
			
			return command;
		}

		protected void FillCommandParameters(IDbCommand command, object o)
		{
			//var p = command.CreateParameter();
		}

        private bool IsPrimitiveType(Type R)
        {
            return R.IsPrimitive
                    || R == typeof(string)
                    || R == typeof(decimal)
                    || R == typeof(DateTime);
        }
		

		protected void OpenConnectionIfNeeded(IDbConnection connection)
		{
			//connection.State == ConnectionState.
		}

		protected void CloseConnectionIfNeeded(IDbConnection connection)
		{
			//закрывать только если нет транзакции
			
		}

        #region Execute methods

		public void ExecuteCommand(string commandText, System.Data.CommandType commandType, object parameters = null)
		{
			using (IDbCommand command = GetPreparedCommand(commandText, parameters, commandType))
            {
                try
                {
					OpenConnectionIfNeeded(command.Connection);
					command.ExecuteNonQuery();
                }
                catch
                {                   
                    throw;
                }
				finally
				{
					CloseConnectionIfNeeded(command.Connection);
				}
            }
		}

		public void ExecuteCommand(string commandText, object parameters = null)
		{
			ExecuteCommand(commandText, Settings.ProviderCommandType, parameters);
		}

		public TResult GetScalarResult<TResult>(string commandText, object parameters = null) where TResult : new()
		{
			return new TResult();
		}

		public IEnumerable<TResult> GetListResult<TResult>(string commandText, object parameters = null) where TResult : new()
		{
			return null;
		}

		public TResult GetSingleResult<TResult>(string commandText, object parameters = null) where TResult : new()
		{
			return new TResult();
		}

		#endregion

		#region Transaction

		protected IDbTransaction Transaction { get; set; }

		public bool IsTransactionStarted
		{
			get
			{
				return (Transaction != null) && (Transaction.Connection != null) && (Transaction.Connection.State == ConnectionState.Open);
			}
		}

		public void BeginTransaction()
		{

		}

		public void CommitTransaction()
		{

		}
		public void RollbackTransaction()
		{

		}
		#endregion

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
