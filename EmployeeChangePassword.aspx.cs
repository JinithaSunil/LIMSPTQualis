using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using DocumentFormat.OpenXml.Bibliography;
using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class EmployeeChangePassword : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            try
            {
                if (Session["UserID"] == null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                }
                else
                {
                    if (!IsPostBack)
                    {
                        string roleId = Session["RoleID"].ToString();
                        int DepartmentId = Convert.ToInt32(Session["DeptID"]);
                        string EmployeeId = Session["EmployeeID"].ToString();
                        string UserId = Session["UserID"].ToString();
                        lblPageID.Text = UserId;
                        lblPageID.Visible = false;

                        txtcurrentPassword.Text = "";
                        txtnewPassword.Text = "";
                        txtrenewPassword.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Save: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {

            bool isPasswordCorrect = VerifyEmployeeCurrentPassword(txtcurrentPassword.Text);

            if (isPasswordCorrect)
            {
                string updatePasswordQuery = "UPDATE Users SET Password = @NewPassword, PasswordResetRequired=0 WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(updatePasswordQuery, con))
                {
                    
                    command.Parameters.AddWithValue("@NewPassword", txtnewPassword.Text);
                    command.Parameters.AddWithValue("@UserID", lblPageID.Text);

                    con.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    con.Close();

                    if (rowsAffected > 0)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Password Change successfully.');</script>");
                    }

                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Current Password not correct');</script>");
            }
        }

        public bool VerifyEmployeeCurrentPassword(string currentPassword)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string selectPasswordQuery = "SELECT Password FROM Users WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(selectPasswordQuery, con))
                {
                    command.Parameters.AddWithValue("@UserID", lblPageID.Text);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader["Password"].ToString();
                            return storedPassword == currentPassword;
                        }
                    }
                }
                con.Close();
            }
            return false;
        }


    }
}