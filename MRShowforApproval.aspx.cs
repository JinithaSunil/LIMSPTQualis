using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class MRShowforApproval : Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        DataAccessLayer dal = new DataAccessLayer();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            try
            {
                if (Session["EmailId"] == null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                }
                else
                {
                    if (!IsPostBack)
                    {
                        string roleId = Session["RoleId"].ToString();
                        string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                        string pageName = System.IO.Path.GetFileName(currentPageUrl);
                        string urlToCheck = pageName;
                        int employeeID = Convert.ToInt32(Session["EmployeeIDget"]);

                        List<MaterialRequestData> response = dal.MaterialRequestShowHOD();
                        if (response != null && response.Count > 0)
                        {
                            List<MaterialRequestData> res = response.Where(item => item.MRM_HODEmployeeID == employeeID && (item.MRM_StatusId == 20 || item.MRM_StatusId == 35)).ToList();

                            if (res != null && res.Count > 0)
                            {
                                RepDataShow.DataSource = res;
                                RepDataShow.DataBind();
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Found but Filtered Data is Empty.'); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = Path.GetFileName(Request.Url.AbsolutePath);
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
                bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                if (success)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Save: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
            }
        }

        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemEdit")
            {
                string GetID = e.CommandArgument.ToString();
                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetID));
                string url = $"MRFormShowforApproval.aspx?PageID={encodedEventID}";
                Response.Redirect(url);
            }
        }

        protected System.Drawing.Color GetStatusColor(object statusTitle)
        {
            string status = statusTitle.ToString();

            switch (status)
            {
                case "MR Generated":
                    return System.Drawing.Color.Blue;
                case "MR Approved":
                    return System.Drawing.Color.Green;
                case "Item Send":
                    return System.Drawing.Color.DarkGreen;
                default:
                    return System.Drawing.Color.Red;
            }
        }


    }
}