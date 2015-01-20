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
           
            
            List<int> parametersList = new List<int>();
            
            EasyDA.EasyDataAccessor<SqlConnection> dacc = new EasyDA.EasyDataAccessor<SqlConnection>();
            dacc.Settings.ConnectionString = "connection";

            dacc.ExecuteCommand("DELETE * FROM dbo.table", CommandType.Text);
            dacc.ExecuteCommand("DELETE * FROM dbo.table WHERE column1 =@param1", CommandType.Text, "10");
            dacc.ExecuteCommand("DELETE * FROM dbo.table WHERE column1=@param1 AND column2=@param2", CommandType.Text, parametersList);

            dacc.ExecuteCommand("dbo.DeletetionProcedure", CommandType.StoredProcedure);
            dacc.ExecuteCommand("dbo.DeletetionProcedure", CommandType.StoredProcedure, parametersList); //parameterized stored procedure call

		}
	}
}
