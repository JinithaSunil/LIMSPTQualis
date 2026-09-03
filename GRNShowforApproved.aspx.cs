using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GRNShowforApproved : System.Web.UI.Page
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
                        //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                        int employeeId = Convert.ToInt32(Session["UserID"]);
                        int masterCompanyId = Convert.ToInt32(Session["MasterCompanyId"]);


                        if (roleId == "1" || roleId == "7")
                        {
                            List<GRNMasterData> response = dal.GRNMasterDataShow();
                            List<GRNMasterData> res = response.Where(item => item.GRN_Status == 12 || item.GRN_Status == 34 || item.GRN_Status == 37).ToList();
                            if (res != null && res.Count > 0)
                            {
                                RepDataShow.DataSource = res;
                                RepDataShow.DataBind();
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Not Found.'); </script>");
                            }
                        }
                        else
                        {
                            List<GRNMasterData> response = dal.GRNMasterDataShow();

                            if (response != null && response.Count > 0)
                            {
                                List<GRNMasterData> res = response.Where(item => item.GRN_Status == 12 || item.GRN_Status == 15 || item.GRN_Status == 34 || item.GRN_Status == 37).ToList();

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
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
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

        protected System.Drawing.Color GetStatusColor(object statusTitle)
        {
            string status = statusTitle.ToString();

            switch (status)
            {
                case "GRN Generated":
                    return System.Drawing.Color.Blue;
                case "GRN Approved":
                    return System.Drawing.Color.Green;
                case "Item Stored":
                    return System.Drawing.Color.DarkGreen;
                case "GRN Rejected":
                    return System.Drawing.Color.Red;
                default:
                    return System.Drawing.Color.OrangeRed;
            }
        }

        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemShow")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string GRN_ID = arguments[0];
                string GRN_NUMBER = arguments[1];
                string GRN_PoNumber = arguments[2];

                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(GRN_ID));
                string encodedEventID1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(GRN_NUMBER));
                string encodedEventID2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(GRN_PoNumber));

                string url = $"GRNitemShowforApproval.aspx?PageID={encodedEventID}&PageData={encodedEventID1}&PageData1={encodedEventID2}";
                Response.Redirect(url);
            }
        }


    }
}