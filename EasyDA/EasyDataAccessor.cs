using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

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
			if (o != null)
			{
				var properties = o.GetType().GetProperties();
				foreach (var p in properties)
				{
					var parameter = command.CreateParameter();
					parameter.ParameterName = p.Name;
					parameter.Value = p.GetValue(o) ?? DBNull.Value;
					command.Parameters.Add(parameter);
				}
			}
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

		/// <summary>
		/// Returns dictionary with Type property names matchingg DataReader's field names
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="T"></param>
		/// <returns></returns>
		protected static Dictionary<string, PropertyInfo> GetCommonProperties(IDataRecord reader, Type T)
		{
			var dict = new Dictionary<string, PropertyInfo>();
			var typeProperties = T.GetProperties().ToDictionary(k=>k.Name.ToUpper());
			
			for (int i = 0; i < reader.FieldCount; i++)
			{
				var columnName = reader.GetName(i).ToUpper();
				PropertyInfo prop;
				if (typeProperties.TryGetValue(columnName, out prop))
				{
					dict.Add(columnName, prop);
				}
			}
			return dict;
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

		public IEnumerable<TResult> GetListResult<TResult>(string commandText, object parameters = null, CommandType? commandType=null) where TResult : new()
		{
			return GetListResult<TResult>(false, commandText, parameters, commandType);
		}

		public TResult GetSingleResult<TResult>(string commandText, object parameters = null, CommandType? commandType=null) where TResult : new()
		{
			return GetListResult<TResult>(true, commandText, parameters, commandType).FirstOrDefault();
		}

		private IEnumerable<TResult> GetListResult<TResult>(bool isFirstResultOnly, string commandText, object parameters = null, CommandType? commandType = null) where TResult : new()
		{
			List<TResult> result = new List<TResult>();
			using (IDbCommand command = GetPreparedCommand(commandText, parameters, commandType.HasValue ? commandType : Settings.ProviderCommandType))
			{
				try
				{
					OpenConnectionIfNeeded(command.Connection);
					using (var reader = command.ExecuteReader())
					{
						var commonProperties = GetCommonProperties(reader, typeof(T));

						while (reader.Read())
						{
							var t = new TResult();
							foreach (var prop in commonProperties)
							{
								var columnValue = reader[prop.Key];
								if (columnValue != null && !(columnValue is System.DBNull))
									prop.Value.SetValue(t, columnValue);
							}
							result.Add(t);

							//stop reading if only first result needed
							if (isFirstResultOnly) break;
						}
					}
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

			return result;
		}

		#endregion

		#region Transaction

		public IDbTransaction Transaction { get; set; }

		public bool IsTransactionStarted
		{
			get
			{
				return (Transaction != null) && (Transaction.Connection != null) && (Transaction.Connection.State == ConnectionState.Open);
			}
		}

		//Number of BeginTransaction() calls were made.
		//Idea is to have maximum one active transaction.
		//If new transactions requested while previous exists, just increase the number, but do not start new nested transaction.
		//During Commit, decrease number and perform actual commit only if number is zero
		protected int TransactionRequestCount { get; set; }

		public void BeginTransaction()
		{
			TransactionRequestCount++;

			if (!IsTransactionStarted)
			{
				var connection = CreateConnection();
				OpenConnectionIfNeeded(connection);
				Transaction = connection.BeginTransaction();
			}
		}

		public void BeginTransaction(IsolationLevel il)
		{
			TransactionRequestCount++;

			if (!IsTransactionStarted)
			{
				var connection = CreateConnection();
				OpenConnectionIfNeeded(connection);
				Transaction = connection.BeginTransaction(il);
			}
		}


		public void CommitTransaction()
		{
			if (IsTransactionStarted)
			{
				TransactionRequestCount--;

				//perform actual commit only if it is last commit request
				if (TransactionRequestCount <= 0)
				{
					var connection = Transaction.Connection;
					Transaction.Commit();
					Transaction.Dispose();
					Transaction = null;
					CloseConnectionIfNeeded(connection);
				}
			}
		}
		public void RollbackTransaction()
		{
			if (IsTransactionStarted)
			{
				TransactionRequestCount = 0;
				var connection = Transaction.Connection;
				Transaction.Rollback();
				Transaction.Dispose();
				Transaction = null;
				CloseConnectionIfNeeded(connection);
			}
		}
		#endregion

		public void Dispose()
		{
			RollbackTransaction();
		}
	}
}
