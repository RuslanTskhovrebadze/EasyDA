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
