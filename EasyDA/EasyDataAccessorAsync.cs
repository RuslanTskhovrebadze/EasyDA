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
        protected async Task<int> OpenConnectionIfNeededAsync(DbConnection connection)
        {
            if (connection.State == ConnectionState.Open || connection.State == ConnectionState.Connecting)
                return 0;
            else
            {
                await connection.OpenAsync();
                return 1;
            }
        }
        
        public async Task<int> ExecuteCommandAsync(string commandText, System.Data.CommandType commandType, object parameters = null)
		{
            using (var command = (DbCommand) GetPreparedCommand(commandText, parameters, commandType))
            {
                try
                {
                    await OpenConnectionIfNeededAsync(command.Connection);
                    return await command.ExecuteNonQueryAsync();
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

        public async Task<TResult> GetScalarResult<TResult>(string commandText, object parameters = null, CommandType? commandType = null) where TResult : new()
        {
            using (var command = (DbCommand) GetPreparedCommand(commandText, parameters, commandType))
            {
                try
                {
                    await OpenConnectionIfNeededAsync(command.Connection);
                    return (TResult) await command.ExecuteScalarAsync();
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


		Task<IEnumerable<TResult>> GetListResultAsync<TResult>(string commandText, object parameters = null, CommandType commandType = CommandType.Text) where TResult : new()
		{
			return new Task<IEnumerable<TResult>>(() => { return new List<TResult>(); });
		}


		Task<TResult> GetSingleResultAsync<TResult>(string commandText, object parameters = null, CommandType commandType = CommandType.Text) where TResult : new()
		{
			return new Task<TResult>(() => { return new TResult(); });
		}
	}
}
