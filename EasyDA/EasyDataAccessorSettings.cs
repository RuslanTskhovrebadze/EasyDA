using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EasyDA
{
	public class EasyDataAccessorSettings
	{
		public string ConnectionString { get; set; }

		private int connectionTimeout = 30; //defaultValue
		public int ConnectionTimeout
		{
			get { return connectionTimeout; }
			set { connectionTimeout = value; }
		}
		

		private CommandType providerCommandType = CommandType.Text; //default value
		public CommandType ProviderCommandType
		{
			get
			{
				return providerCommandType;
			}

			set
			{
				providerCommandType = value;
			}
		}

		private Type dbConnectionType;
		internal Type DbConnectionType
		{
			get
			{
				return dbConnectionType;
			}
			set
			{
				if (!(value is IDbConnection))
				{
					throw new ArgumentException("DbConnectionType must be type of System.Data.IDbConnection", "DbConnectionType");
				}
				dbConnectionType = value;
			}
		}
	}
}
