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
    public partial class MaterialRequestApprovalShow : System.Web.UI.Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
        private int Year;
        DataAccessLayer dal = new DataAccessLayer();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

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

                    try
                    {
                        if (Request.QueryString["PageID"] != null)
                        {
                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                            int employeeId = Convert.ToInt32(Session["UserID"]);

                            List<MaterialRequestData> response = dal.MaterialRequestGetDataforApprovalBothShow();
                            List<MaterialRequestData> res = response.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text)).ToList();
                            if (res != null && res.Count > 0)
                            {
                                MaterialRequestData selectedItem = res[0];

                                lblMRNo.Text = selectedItem.MRM_NUMBER.ToString();
                                lblMRGeneratedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();

                                GridView1.DataSource = res;
                                GridView1.DataBind();

                                lblPreparedBy.Text = selectedItem.FirstName.ToString() ;
                                lblPreparedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();

                                lblApprovedBy.Text = selectedItem.ApprovalFirstName.ToString();
                                lblApprovedDate.Text = selectedItem.MRM_ApprovedDateTime.ToString();


                                //int EmpID = Convert.ToInt32(selectedItem.MRM_EmployeeID.ToString());
                                //List<AllEmployeeDetailsData> resData = dal.ActiveCompanyEmployeeDetailsDataGet();
                                //List<AllEmployeeDetailsData> filteredData = resData.Where(item => item.EmployeeId == EmpID).ToList();
                                //if (filteredData != null && filteredData.Count > 0)
                                //{
                                //    AllEmployeeDetailsData selectedItems = filteredData[0];
                                //    lblPreparedBy.Text = selectedItems.FirstName.ToString() + " " + selectedItems.LastName.ToString();
                                //}
                                //lblPreparedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();

                                //int EmpID1 = Convert.ToInt32(selectedItem.MRM_ApprovedEmployeeID.ToString());
                                //List<AllEmployeeDetailsData> resData1 = dal.ActiveCompanyEmployeeDetailsDataGet();
                                //List<AllEmployeeDetailsData> filteredData1 = resData1.Where(item => item.EmployeeId == EmpID1).ToList();
                                //if (filteredData1 != null && filteredData1.Count > 0)
                                //{
                                //    AllEmployeeDetailsData selectedItems1 = filteredData1[0];
                                //    lblApprovedBy.Text = selectedItems1.FirstName.ToString() + " " + selectedItems1.LastName.ToString();
                                //}
                                //lblApprovedDate.Text = selectedItem.MRM_ApprovedDateTime.ToString();
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something wrong, PageID Not Getting!!'); setTimeout(function() { window.location = 'GRNGenerateShow.aspx'; }, 100); </script>");
                        }
                    }
                    catch (Exception ex)
                    {
                        int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                        var s = new StackTrace(ex);
                        var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                        /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
                        bool success = dal.ErrorLogData(methodName, ex.Message, "MaterialRequestApprovalShow.aspx.cs", linenum.ToString());
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
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                /* Assuming the index of the column MR_RequireDate is 8 */
                int columnIndex = 5;

                if (e.Row.Cells.Count > columnIndex)
                {
                    string originalDate = e.Row.Cells[columnIndex].Text;

                    if (!string.IsNullOrEmpty(originalDate))
                    {
                        DateTime parsedDate;
                        if (DateTime.TryParse(originalDate, out parsedDate))
                        {
                            e.Row.Cells[columnIndex].Text = parsedDate.ToString("dd-MMM-yyyy");
                        }
                    }
                }
            }
        }


    }
}