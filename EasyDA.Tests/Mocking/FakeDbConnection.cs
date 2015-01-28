using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDA.Tests.Mocking
{
	public class FakeDbConnection: IDbConnection
	{
		private bool isTransactionStarted = false;

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			if (isTransactionStarted)
			{
				throw new Exception("Transaction Already Started");
			}
			var tran = new FakeDbTransaction();
			tran.IsolationLevel = il;
			return tran;
		}

		public IDbTransaction BeginTransaction()
		{
			if (isTransactionStarted)
			{
				throw new Exception("Transaction Already Started");
			}
			return new FakeDbTransaction();
		}

		public void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			if (State == ConnectionState.Closed)
			{
				throw new Exception("Already closed");
			}
			State = ConnectionState.Closed;
		}

		public string ConnectionString
		{
			get;
			set;
		}

		public int ConnectionTimeout
		{
			get { throw new NotImplementedException(); }
		}

		public IDbCommand CreateCommand()
		{
			var c = new FakeDbCommand();
			c.Connection = this;
			return c;
		}

		public string Database
		{
			get { throw new NotImplementedException(); }
		}

		public void Open()
		{
			if (State == ConnectionState.Open)
			{
				throw new Exception("Already opened");
			}
			State = ConnectionState.Open;
		}

		public ConnectionState State
		{
			get;
			private set;
		}

		public void Dispose()
		{
			
		}
	}
}
