using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
 namespace LIMS
{
    public static class ErrorLogger
    {
        public static void LogError(Guid errorId, Exception ex)
        {
            try
            {
                string cs = ConfigurationManager
                    .ConnectionStrings["CM"]
                    .ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = @"
                INSERT INTO ErrorLog
                (
                    ErrorGuid,
                    UserName,
                    PageUrl,
                    ErrorMessage,
                    StackTrace
                )
                VALUES
                (
                    @ErrorGuid,
                    @UserName,
                    @PageUrl,
                    @ErrorMessage,
                    @StackTrace
                )";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@ErrorGuid", errorId);

                        cmd.Parameters.AddWithValue("@UserName",
                            HttpContext.Current?.Session?["UserName"]?.ToString() ?? "");

                        cmd.Parameters.AddWithValue("@PageUrl",
                            HttpContext.Current?.Request?.RawUrl ?? "");

                        cmd.Parameters.AddWithValue("@ErrorMessage",
                            ex.Message);

                        cmd.Parameters.AddWithValue("@StackTrace",
                            ex.ToString());

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Never throw from logger
            }
        }

        public static void LogError(Exception ex)
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO ErrorLog
                    (
                        UserName,
                        PageUrl,
                        ErrorMessage,
                        StackTrace,
                        InnerException,
                        ServerName
                    )
                    VALUES
                    (
                        @UserName,
                        @PageUrl,
                        @ErrorMessage,
                        @StackTrace,
                        @InnerException,
                        @ServerName
                    )", con))
                    {
                        string userName = "";

                        if (HttpContext.Current?.Session != null)
                        {
                            userName = Convert.ToString(
                                HttpContext.Current.Session["UserName"]);
                        }

                        cmd.Parameters.AddWithValue("@UserName", userName ?? "");
                        cmd.Parameters.AddWithValue("@PageUrl",
                            HttpContext.Current?.Request?.RawUrl ?? "");

                        cmd.Parameters.AddWithValue("@ErrorMessage",
                            ex.Message ?? "");

                        cmd.Parameters.AddWithValue("@StackTrace",
                            ex.ToString());

                        cmd.Parameters.AddWithValue("@InnerException",
                            ex.InnerException?.ToString() ?? "");

                        cmd.Parameters.AddWithValue("@ServerName",
                            Environment.MachineName);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Never throw from logger
            }
        }
    }
}
