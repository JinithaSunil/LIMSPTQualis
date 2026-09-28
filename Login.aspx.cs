using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using static LIMS.Global;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace LIMS
{
    public partial class Login : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();           
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if ((txtUserName.Text != "") && (txtPassword.Text != ""))
            {
                string username = txtUserName.Text;
                string password = txtPassword.Text.ToString();
                string sQuery = string.Empty;
                _cm.username = username;
                _cm.password = password;

                DataTable dt = _cm.getLoginDetails(_cm);

                if (dt.Rows.Count == 0)
                {
                    txtPassword.Text = string.Empty;
                    txtUserName.Text = string.Empty;
                    txtUserName.Focus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Invalid username or password.')", true);
                }
                else
                {
                    int RoleID = 0;
                    int resetRequired = Convert.ToInt32(dt.Rows[0]["PasswordResetRequired"].ToString());
                    
                    
                        Session["OrganizationID"] = dt.Rows[0]["OrganizationID"].ToString();
                        Session["UserID"] = dt.Rows[0]["UserID"].ToString();
                        Session["UserName"] = dt.Rows[0]["UserName"].ToString();
                        Session["RoleID"] = dt.Rows[0]["RoleID"].ToString();
                        Session["ActiveRole"] = dt.Rows[0]["RoleName"].ToString();
                        RoleID = Convert.ToInt16(dt.Rows[0]["RoleID"].ToString());
                        Session["EmployeeID"] = dt.Rows[0]["EmployeeID"].ToString();
                        Session["LoggedUser"] = dt.Rows[0]["designationName"].ToString();
                        Session["Email"] = dt.Rows[0]["Email"].ToString();
                        Session["EmployeeName"] = dt.Rows[0]["EmployeeName"].ToString();
                        string user = Session["UserName"].ToString();
                        string empName = Session["EmployeeName"].ToString();
                        if (resetRequired == 1)
                        {
                            Session["Password"] = "Reset";
                            Response.Redirect("EmployeeChangePassword.aspx");
                        }
                        lock (Global.LoggedUsers)
                        {
                            if (!Global.LoggedUsers.Any(u => u.UserName == user))
                            {
                                Global.LoggedUsers.Add(new LoggedUser
                                {
                                    EmployeeName = empName,
                                    UserName = user,
                                    LoginTime = DateTime.Now
                                });
                            }
                        }
                        Session["DeptID"] = dt.Rows[0]["DeptID"].ToString();
                        Session["Usertype"] = dt.Rows[0]["designationName"].ToString();
                        //newly added //
                        Session["DeptName"] = dt.Rows[0]["EmployeeDepartmentName"].ToString();
                        Session["CompanyUnitId"] = "1";
                        Session["RoleId"] = dt.Rows[0]["RoleId"].ToString();
                        Session["EmailId"] = dt.Rows[0]["Email"].ToString();
                        Session["MasterCompanyId"] = "1";
                        Session["CreatedEmployeeId"] = dt.Rows[0]["userid"].ToString();
                        Session["userType"] = dt.Rows[0]["designationName"].ToString();
                        Session["HODEmployeeId"] = dt.Rows[0]["PID"].ToString();
                        Session["EmployeeIDget"] = dt.Rows[0]["EmployeeIdGet"].ToString();
                        Session["EmpID"] = dt.Rows[0]["EmployeeIdGet"].ToString();

                        //save auditlog
                        if (Session["UserID"] != null && Session["UserName"] != null && Session["EmployeeName"] != null)
                        {
                            _cm.AuditLog(
                               Convert.ToInt32(Session["UserID"].ToString()),
                               Session["UserName"].ToString() + " : " + Session["EmployeeName"].ToString(),
                               "LOGIN",
                               "User Login"
                           );
                        }

                        Session["DepartmentID"] = dt.Rows[0]["DepartmentID1"].ToString();
                        string str1 = @"select * from Roles r inner join ModuleMaster m ON m.ModuleId = r.ModuleID Where r.RoleId ='" + RoleID + "' ";

                        SqlDataAdapter sda = new SqlDataAdapter(str1, con);
                        con.Open();
                        DataTable dt1 = new DataTable();
                        sda.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            int ModuleId = Convert.ToInt32(dt1.Rows[0]["ModuleId"].ToString());
                            Session["ModuleID"] = dt1.Rows[0]["ModuleId"].ToString();

                            switch (ModuleId)
                            {
                                case 1://LIMS
                                    Response.Redirect("Default.aspx");
                                    break;
                                case 2://Sales & Marketing  
                                    Response.Redirect("DashboardSalesAndMarketing.aspx");
                                    break;
                                case 3://Purchasing  
                                    Response.Redirect("DashboardPurchasing.aspx");
                                    break;
                                case 4://Accounting
                                    Response.Redirect("DashboardAccounting1.aspx");
                                    break;
                                case 5://Store
                                    Response.Redirect("GRNGenerateShow.aspx");
                                    //Response.Redirect("StoreHomePage.aspx");
                                    break;
                                case 6://Customer Service
                                case 7:
                                    Response.Redirect("CustomerPOList.aspx");
                                    break;
                                default:
                                    Response.Redirect("Login.aspx");
                                    break;
                            }
                        }
                        con.Close();
                }
            }
            else
            {
                if (txtUserName.Text == "") { txtUserName.BorderColor = Color.Red;flag = 1; } else { txtUserName.BorderColor = Color.Empty; }
                if (txtPassword.Text == "") { txtPassword.BorderColor = Color.Red; flag = 1; } else { txtPassword.BorderColor = Color.Empty; }

            }
        }
    }
}