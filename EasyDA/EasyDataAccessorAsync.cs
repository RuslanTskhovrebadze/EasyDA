using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDA
{
	public class EasyDataAccessorAsync<T> : EasyDataAccessor<T> where T : DbConnection
	{
		public Task ExecuteCommandAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
		{
			return new Task(() => { });
		}

		public async Task<IEnumerable<TResult>> GetListResultAsync<TResult>(string commandText, object parameters = null, CommandType? commandType = null) where TResult : new()
		{
			return await GetListResultAsync<TResult>(false, commandText, parameters, commandType);
		}


		public async Task<TResult> GetSingleResultAsync<TResult>(string commandText, object parameters = null, CommandType? commandType = null) where TResult : new()
		{
			return (await GetListResultAsync<TResult>(true, commandText, parameters, commandType)).FirstOrDefault();
		}

		private async Task<IEnumerable<TResult>> GetListResultAsync<TResult>(bool isSingleResult, string commandText, object parameters = null, CommandType? commandType = null) where TResult : new()
		{
			List<TResult> result = new List<TResult>();
			using (var command = GetPreparedCommand(commandText, parameters, commandType.HasValue ? commandType : Settings.ProviderCommandType))
			{
				try
				{
					OpenConnectionIfNeeded(command.Connection);
					using (var reader = await command.ExecuteReaderAsync())
					{
						var commonProperties = GetCommonProperties(reader, typeof(T));

						while (await reader.ReadAsync())
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
							if (isSingleResult) break;
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

		protected new DbConnection CreateConnection()
		{
			return (DbConnection)base.CreateConnection();
		}

		protected new DbCommand GetPreparedCommand(string commandText, object parameters, CommandType? commandType)
		{
			return (DbCommand)base.GetPreparedCommand(commandText, parameters, commandType);
		}
	}
}
