using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace LIMS
{
    public partial class MaterialTransactionInUnit : System.Web.UI.Page
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
                        if (Request.QueryString["PageID"] != null && Request.QueryString["PageData"] != null)
                        {
                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            byte[] decodedBytes1 = Convert.FromBase64String(Request.QueryString["PageData"]);
                            string decodedEventListId1 = Encoding.UTF8.GetString(decodedBytes1);

                            lblPageData.Text = decodedEventListId1;
                            lblPageData.Visible = false;

                            int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                            int employeeId = Convert.ToInt32(Session["UserID"]);

                            DataTable dt = dal.MaterialRequestDataBind(lblPageData.Text);
                            RepDataShow.DataSource = dt;
                            RepDataShow.DataBind();

                            List<MaterialRequestData> response = dal.MaterialRequestGetDataforApprovalBothShow();
                            List<MaterialRequestData> res = response.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text)).ToList();
                            if (res != null && res.Count > 0)
                            {
                                MaterialRequestData selectedItem = res[0];
                                lblMRNo.Text = selectedItem.MRM_NUMBER.ToString();                                
                                lblMRGeneratedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();
                                lblPreparedBy.Text = selectedItem.FirstName.ToString() + " " + selectedItem.LastName.ToString();
                                lblPreparedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();
                                lblApprovedBy.Text = selectedItem.ApprovalFirstName.ToString();
                                lblApprovedDate.Text = selectedItem.MRM_ApprovedDateTime.ToString();

                                List<MasterTransactionData> response4 = dal.MasterTransactionDataGet();
                                List<MasterTransactionData> filteredData4 = response4.Where(item => item.MT_MRMID == Convert.ToInt32(lblPageID.Text)).ToList();
                                if (filteredData4 != null && filteredData4.Count > 0)
                                {
                                    MasterTransactionData selectedItem4 = filteredData4[0];
                                    int EmpID2 = Convert.ToInt32(selectedItem4.MT_EmployeeId);
                                    //List<AllEmployeeDetailsData> resData2 = dal.ActiveCompanyEmployeeDetailsDataGet();
                                    //List<AllEmployeeDetailsData> filteredData2 = resData1.Where(item => item.EmployeeId == EmpID2).ToList();
                                    //if (filteredData2 != null && filteredData2.Count > 0)
                                    //{
                                    //    AllEmployeeDetailsData selectedItems2 = filteredData2[0];
                                    //    lblReleasedBy.Text = selectedItems2.FirstName.ToString() + " " + selectedItems2.LastName.ToString();
                                    //}
                                    //lblReleasedBy.Text = selectedItems2.FirstName.ToString() + " " + selectedItems2.LastName.ToString();
                                    lblReleasedDate.Text = selectedItem4.MT_CreatedDateTime.ToString();
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                                }
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
            }
        }

        protected void RepDataShow_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView dataItem = e.Item.DataItem as DataRowView;

                if (dataItem != null)
                {
                    Repeater tagDataRepeater = e.Item.FindControl("TagDataRepeater") as Repeater;

                    if (tagDataRepeater != null)
                    {
                        string filterValue = dataItem["MR_ItemCode"].ToString();
                        List<MaterialRequestData> filteredData = GetFilteredDataForTagRepeater(filterValue);
                        tagDataRepeater.DataSource = filteredData;
                        tagDataRepeater.DataBind();
                    }
                }
            }
        }

        private List<MaterialRequestData> GetFilteredDataForTagRepeater(string filterValue)
        {
            List<MaterialRequestData> filteredData = dal.MaterialTransationTAGDataget().Where(item => item.MR_ItemCode == filterValue && item.MRM_NUMBER == lblPageData.Text).ToList();
            return filteredData;
        }


    }
}