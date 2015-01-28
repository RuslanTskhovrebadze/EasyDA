using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDA.Tests.Mocking
{
	public class FakeDbTransaction: IDbTransaction
	{
		private bool isStarted = true;

		public void Commit()
		{
			this.Connection = null;
			isStarted = false;
		}

		public FakeDbConnection Connection
		{
			get;
			private set;
		}

		IDbConnection IDbTransaction.Connection { get { return this.Connection; } }

		public IsolationLevel IsolationLevel
		{
			get;
			internal set;
		}

		public void Rollback()
		{
			this.Connection = null;
			isStarted = false;
		}

		public void Dispose()
		{
			Rollback();
		}
	}

}
