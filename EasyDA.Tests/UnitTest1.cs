using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace EasyDA.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			// System.Data.SqlClient.SqlCommand c;
            
			EasyDA.EasyDataAccessor<SqlConnection> dacc = new EasyDA.EasyDataAccessor<SqlConnection>("connection string", CommandType.StoredProcedure, null);
			
            dacc.ExecuteCommand("DELETE * FROM dbo.table", CommandType.Text);
            dacc.ExecuteCommand("DELETE * FROM dbo.table WHERE column1 =@param1", CommandType.Text, "10");
			dacc.ExecuteCommand("DELETE * FROM dbo.table WHERE column1=@param1 AND column2=@param2", CommandType.Text, new {param1=4, param2="sdfsdf" });

            dacc.ExecuteCommand("dbo.DeletetionProcedure", CommandType.StoredProcedure);
			dacc.ExecuteCommand("dbo.DeletetionProcedure", CommandType.StoredProcedure, new { param1 = 4, param2 = "sdfsdf" }); //parameterized stored procedure call

		}
	}
}
