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
    public partial class GRNitemShow : System.Web.UI.Page
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

                        if (Request.QueryString["PageID"] != null && Request.QueryString["PageData"] != null && Request.QueryString["PageData1"] != null)
                        {
                            string pageID = Request.QueryString["PageID"];
                            string encodedString = pageID;
                            byte[] decodedBytes = Convert.FromBase64String(encodedString);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            string PageData = Request.QueryString["PageData"];
                            string encodedString1 = PageData;
                            byte[] decodedBytes1 = Convert.FromBase64String(encodedString1);
                            string decodedEventListId1 = Encoding.UTF8.GetString(decodedBytes1);

                            byte[] decodedBytes2 = Convert.FromBase64String(Request.QueryString["PageData1"]);
                            string decodedEventListId2 = Encoding.UTF8.GetString(decodedBytes2);

                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            lblPageData.Text = decodedEventListId1;
                            lblPageData.Visible = false;

                            lblPageData1.Text = decodedEventListId2;
                            lblPageData1.Visible = false;

                            lblGRNShow.Text = decodedEventListId1;

                            int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                            string getText = lblPageData1.Text;
                            int gdPoId = 0;

                            if (roleId == "1" || roleId == "7")
                            {
                                List<GRNMasterData> response1 = dal.GRNDetailsShowNew();
                                List<GRNMasterData> filteredData2 = response1.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();
                                if (filteredData2.Count > 0)
                                {
                                    GRNMasterData selectedItem = filteredData2[0];

                                    lblVendorName.Text = selectedItem.VendorName?.ToString();
                                    lblVendorCompany.Text = selectedItem.VendorCompanyName?.ToString();
                                    lblPoNo.Text = selectedItem.GRN_PoNumber?.ToString();
                                    lblGRNDate.Text = selectedItem.GRN_CreatedDateTime?.ToString();
                                    lblPODate.Text = selectedItem.PODate.ToString();
                                    gdPoId = selectedItem.GDPoId;

                                    GridView1.DataSource = filteredData2;
                                    GridView1.DataBind();
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                                }

                            }
                            else
                            {
                                List<GRNMasterData> response1 = dal.GRNDetailsShowNew();
                                List<GRNMasterData> filteredData1 = response1.Where(item => item.GRN_CompanyUnitId == companyUnitId &&
                                                                                    item.GRN_NUMBER == lblPageData.Text).ToList();

                                if (filteredData1.Count > 0)
                                {
                                    GRNMasterData selectedItem = filteredData1[0];

                                    lblVendorName.Text = selectedItem.VendorName?.ToString();
                                    lblVendorCompany.Text = selectedItem.VendorCompanyName?.ToString();
                                    lblPoNo.Text = selectedItem.GRN_PoNumber?.ToString();
                                    lblGRNDate.Text = selectedItem.GRN_CreatedDateTime?.ToString();
                                    lblPODate.Text = selectedItem.PODate.ToString();
                                    gdPoId = selectedItem.GDPoId;

                                    GridView1.DataSource = filteredData1;
                                    GridView1.DataBind();
                                }
                            }

                            /* code is use for Bill Image Show */
                            List<GRNMasterData> res = dal.GRNBillImageShow();
                            List<GRNMasterData> resdata = res.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                            if (resdata != null && resdata.Count > 0)
                            {
                                foreach (var item in resdata)
                                {
                                    string fileName = item.GRNFileName.ToString();
                                    string weburl = Session["WebsiteURL"].ToString();

                                    foreach (string individualFileName in fileName.Split(','))
                                    {
                                        HyperLink fileLink = new HyperLink();
                                        fileLink.Text = individualFileName;
                                        fileLink.NavigateUrl = weburl + "/UploadedFiles/" + individualFileName.Trim();
                                        fileLink.Target = "_blank"; /* Set the target to open in a new page */
                                        Panel filePanel = new Panel();
                                        filePanel.Controls.Add(fileLink);
                                        filePanel.Controls.Add(new LiteralControl("<br/>"));
                                        fileLinksContainer.Controls.Add(filePanel);
                                    }
                                }
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                            }

                            /* code is use for PreparedBy and ApprovedBy*/
                            List<GRNMasterData> resdataget = dal.GRNMasterDataShow();
                            List<GRNMasterData> fdres = resdataget.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                            if (fdres != null && fdres.Count > 0)
                            {
                                GRNMasterData selectedItem4 = fdres[0];

                                string firstName = selectedItem4.FirstName.ToString();
                                //string lastName = selectedItem4.LastName.ToString();

                                lblPreparedBy.Text = firstName;
                                lblPreparedDate.Text = selectedItem4.GRN_CreatedDateTime.ToString();

                                int GRNApprovedEmployeeID = selectedItem4.GRN_ApprovedEmployeeID;
                                if (GRNApprovedEmployeeID > 0)
                                {
                                    //List<GRNMasterData> data1 = dal.GRNMasterDataShowForApprovedHod();
                                    //List<GRNMasterData> data2 = data1.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();
                                    List<GRNMasterData> data2 = dal.GRNMasterDataShowForApprovedHod(lblPageData.Text);
                                    if (data2 != null && data2.Count > 0)
                                    {
                                        GRNMasterData selectData = data2[0];
                                        string AfirstName = selectData.FirstName.ToString();
                                        //string AlastName = selectData.LastName.ToString();
                                        lblApprovedBy.Text = AfirstName;
                                        lblApprovedDate.Text = selectedItem4.GRN_ApprovedDateTime.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something wrong, PageID Not Getting!!'); setTimeout(function() { window.location = 'GRNGenerateShow.aspx'; }, 100); </script>");
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


    }
}