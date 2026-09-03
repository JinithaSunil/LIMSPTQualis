using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class DbHelper
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            
        }

        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
           
        }
        public static int ExecuteInsertWithReturn(string sql, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    // ExecuteScalar returns the first column of the first row
                    object result = cmd.ExecuteScalar();

                    return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                }
            }
           
        }
        public static object ExecuteScalarStoredProc(
                        string procName,
                        SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    foreach (SqlError err in ex.Errors)
                    {
                        Console.WriteLine("Message: " + err.Message);
                        Console.WriteLine("Procedure: " + err.Procedure);
                        Console.WriteLine("Line: " + err.LineNumber);
                    }

                    throw;
                }
            }
        }
        public static int ExecuteStoredProc(string procName, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    SqlParameter returnValueParam = new SqlParameter();
                    returnValueParam.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValueParam);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        foreach (SqlError err in ex.Errors)
                        {
                            Console.WriteLine("Message: " + err.Message);
                            Console.WriteLine("Procedure: " + err.Procedure);
                            Console.WriteLine("Line: " + err.LineNumber);
                        }
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        // Handle other exceptions
                    }

                    return ((int)returnValueParam.Value);
                }
            }
            
        }

        public static DataTable ExecuteStoredProcDT(string procName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public static DataTable ExecuteDataTable(SqlCommand command)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                command.Connection = con;
                command.CommandType = CommandType.Text;

                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    con.Open();
                    da.Fill(dt);
                }
            }

            return dt;
        }
        public static DataSet ExecuteDataSet(
        CommandType commandType,
        string commandText,
        params SqlParameter[] commandParameters)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(commandText, con))
            {
                cmd.CommandType = commandType;

                if (commandParameters != null)
                    cmd.Parameters.AddRange(commandParameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }

            return ds;
        }
    }

}