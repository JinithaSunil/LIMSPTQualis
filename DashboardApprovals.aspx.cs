using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class DashboardApprovals : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM();
        private string currentDepartment;
        private string currentUserID;
        private string currentEmployeeID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Assuming UserID is stored in session after login
                currentUserID = Session["UserID"]?.ToString();

                if (!IsPostBack && !string.IsNullOrEmpty(currentUserID))
                {
                    DataTable userDetails = _cm.GetEmpDetails(currentUserID);
                    if (userDetails != null && userDetails.Rows.Count > 0)
                    {
                        // Store department in ViewState
                        ViewState["CurrentDepartment"] = userDetails.Rows[0]["DepartmentID"]?.ToString() ?? string.Empty;
                        ViewState["currentEmployeeID"] = userDetails.Rows[0]["EmployeeID"]?.ToString() ?? string.Empty;
                        
                        currentDepartment = ViewState["CurrentDepartment"].ToString();
                        currentEmployeeID = ViewState["currentEmployeeID"].ToString();

                        if (Session["RoleID"] != null)
                        {
                            BindPR();
                            BindOnlineUsers(); 
                            //BindProcess();
                        }

                    }
                    else
                    {
                        // Handle case where user details cannot be retrieved
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Detail Not Found');", true);
                    }
                }
                else
                {
                    // Retrieve department from ViewState on postback
                    currentDepartment = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;
                }
            }
        }

        protected void BindPR()
        {
            DataTable dtAll = new DataTable();
            DataTable dataTable = new DataTable();
            string RoleID = Session["RoleID"].ToString();
            string Role = "";
            string Period = radPeriod.SelectedItem.ToString();

            switch (RoleID)
            {
                case "2":
                    Role = "Manager";
                    dataTable = _cm.getPRCount(currentDepartment,Period);
                    break;
                case "22":
                    Role = "IT Manager";
                    dataTable = _cm.getPRCount(Role, 2, Period);
                    break;
                case "23":
                    Role = "QMS Manager";
                    dataTable = _cm.getPRCount(Role, 2, Period);
                    break;
                case "24":
                    Role = "HCGA Manager";
                    dataTable = _cm.getPRCount(Role, 2, Period);
                    break;
                case "25":
                    Role = "Maintenance Manager";
                    dataTable = _cm.getPRCount(Role, 2, Period);
                    break;
                case "26":
                    Role = "Director[Sales Marketing]";
                    dataTable = _cm.getPRCount(Role, 3, Period);
                    break;
                case "27":
                    Role = "Director[Lab]";
                    dataTable = _cm.getPRCount(Role, 3, Period);
                    break;
                case "28":
                    Role = "Director";
                    dataTable = _cm.getPRCount(Role, 4, Period);
                    break;
                default:
                    break;

            }
            currentEmployeeID = ViewState["currentEmployeeID"].ToString();
            dtAll = _cm.getAllPRCount(currentEmployeeID, Period);
            lblAllPR.Text = dtAll.Rows.Count.ToString();
            lblPendingPR.Text = dataTable.Rows.Count.ToString();

        }
        protected void radPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPR();
        }
        protected void BindProcess()
        {
            List<string> statusNames = new List<string>();
            List<int> statusCounts = new List<int>();
            DataTable dt= new DataTable();
            dt = _cm.getStatus("Quotation");
            if(dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Panel step = new Panel();
                    step.CssClass = "step";

                    step.Controls.Add(new Literal
                    {
                        Text = row["StatusName"].ToString()
                    });

                    phProcess.Controls.Add(step);
                }
            }
        }       

        protected void BindOnlineUsers()
        {
            var list = Global.LoggedUsers.Select(u =>
            {
                var duration = DateTime.Now - u.LoginTime;
                var idle = DateTime.Now - u.LastActivity;

                return new
                {
                    u.EmployeeName,
                    u.UserName,
                    u.LoginTime,
                    Duration = duration.ToString(@"hh\:mm\:ss"),
                    IdleMinutes = idle.TotalMinutes,
                    Status = idle.TotalMinutes > 5 ? "Idle" : "Online",
                    StatusClass = idle.TotalMinutes > 5 ? "idle" : "online",
                    StatusText = idle.TotalMinutes > 5 ? "Idle" : "Online"
                };
            }).ToList();

            gvUsers.DataSource = list;
            gvUsers.DataBind();
            lblCount.Text = "Active Users: " + list.Count;
        }

    }

}