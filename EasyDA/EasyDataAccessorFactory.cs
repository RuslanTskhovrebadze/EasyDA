using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EasyDA
{
	public static class EasyDataAccessorFactory
	{
		private static EasyDataAccessorSettings settings = new EasyDataAccessorSettings();
		public static EasyDataAccessorSettings Settings {get;set;}
		
		public static EasyDataAccessor<IDbConnection> CreateInstance()
		{
			return new EasyDataAccessor<IDbConnection>(Settings.ConnectionString, Settings.ProviderCommandType, Settings.ConnectionTimeout, Settings.DbConnectionType);
		}

		public static void Configure<T>(string connectionString, CommandType? commandType, int? connectionTimeout) where T : IDbConnection, new()
		{
			Settings.ConnectionString = connectionString;
			if (commandType.HasValue)
			{
				Settings.ProviderCommandType = commandType.Value;
			}			
			Settings.ConnectionTimeout = connectionTimeout.Value;
			Settings.DbConnectionType = typeof(T);
		}
	}
}
