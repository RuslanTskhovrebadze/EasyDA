using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDA.Tests.Mocking
{
	public class FakeDbCommand: IDbCommand
	{
		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public string CommandText
		{
			get;
			set;
		}

		public int CommandTimeout
		{
			get;
			set;
		}

		public CommandType CommandType
		{
			get;
			set;
		}

		public IDbConnection Connection
		{
			get;
			set;
		}

		public IDbDataParameter CreateParameter()
		{
			return new FakeDbDataParameter();
		}

		public int ExecuteNonQuery()
		{
			throw new NotImplementedException();
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}

		public IDataReader ExecuteReader()
		{
			throw new NotImplementedException();
		}

		public object ExecuteScalar()
		{
			throw new NotImplementedException();
		}

		public IDataParameterCollection Parameters
		{
			get { throw new NotImplementedException(); }
		}

		public void Prepare()
		{
			throw new NotImplementedException();
		}

		public IDbTransaction Transaction
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
