using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using LIMS.DAL;
using LIMS.Models;

namespace LIMS
{
    public partial class GRNitemShowforApproval : System.Web.UI.Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
        private int Year;
        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();
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
                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            byte[] decodedBytes1 = Convert.FromBase64String(Request.QueryString["PageData"]);
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

                            if (roleId == "1" || roleId == "7" || roleId == "15")
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
                                    lblPODate.Text = selectedItem.PO_CreatedDateTime.ToString();
                                    gdPoId = selectedItem.GDITId;


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
                                List<GRNMasterData> filteredData1 = response1.Where(item => item.GRN_CompanyUnitId == companyUnitId && item.GRN_NUMBER == lblPageData.Text).ToList();

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

                            List<GRNMasterData> res = dal.GRNBillImageShow();
                            List<GRNMasterData> resdata = res.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                            if (resdata != null && resdata.Count > 0)
                            {
                                foreach (var item in resdata)
                                {
                                    string DocumentName = item.GRNDocumentName.ToString(); 
                                    string fileName = item.GRNFileName.ToString();
                                    string weburl = Session["WebsiteURL"].ToString();

                                    foreach (string individualFileName in fileName.Split(','))
                                    {
                                        HyperLink fileLink = new HyperLink();
                                        fileLink.Text = DocumentName;
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

                            List<GRNMasterData> resdataget = dal.GRNMasterDataShow();
                            List<GRNMasterData> fdres = resdataget.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                            if (fdres != null && fdres.Count > 0)
                            {
                                GRNMasterData selectedItem4 = fdres[0];
                                string firstName = selectedItem4.FirstName.ToString();
                                //string lastName = selectedItem4.LastName.ToString();
                                ViewState["GRN_EmployeeIdForNotification"] = selectedItem4.GRN_EmployeeID.ToString();
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
                                        
                                        /*string AlastName = selectData.LastName.ToString();*/
                                        lblApprovedBy.Text = AfirstName;
                                        lblApprovedDate.Text = selectedItem4.GRN_ApprovedDateTime.ToString();
                                    }

                                    //List<AllEmployeeDetailsData> data1 = dal.AllCompanyEmployeeDetailsDataGet();
                                    //List<AllEmployeeDetailsData> data2 = data1.Where(item => item.EmployeeId == GRNApprovedEmployeeID).ToList();
                                    //if (data2 != null && data2.Count > 0)
                                    //{
                                    //    AllEmployeeDetailsData selectData = data2[0];
                                    //    string AfirstName = selectData.FirstName.ToString();
                                    //    //string AlastName = selectData.LastName.ToString();
                                    //    lblApprovedBy.Text = AfirstName;
                                    //    lblApprovedDate.Text = selectedItem4.GRN_ApprovedDateTime.ToString();
                                    //    btnApproved.Visible = false;
                                    //    btnRejected.Visible = false;
                                    //}
                                }
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something wrong, PageID Not Getting!!'); setTimeout(function() { window.location = 'GRNShowforApproved.aspx'; }, 100); </script>");
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

        protected void btnApproved_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = @"UPDATE TransactionMaster set GRNStatus = @GRN_Status, 
                                ApprovedBy = @GRN_ApprovedEmployeeID, ApprovedAt = @GRN_ApprovedDateTime
                                WHERE InwardID = @GRN_ID";

                SqlCommand cmd = new SqlCommand(str1, con);

                cmd.Parameters.AddWithValue("@GRN_ID", lblPageID.Text);
                cmd.Parameters.AddWithValue("@GRN_ApprovedEmployeeID", Session["EmployeeID"].ToString());
                cmd.Parameters.AddWithValue("@GRN_Status", 13);
                cmd.Parameters.AddWithValue("@GRN_ApprovedDateTime", GetDateTime);

                con.Open();
                int j = cmd.ExecuteNonQuery();
                con.Close();

                if (j > 0)
                {
                    var TableRowId = lblPageID.Text;
                    bool successes = dal.AuditLogData("Update GRN Master Status for Approved", "GRNMaster", TableRowId, "");
                    if (successes) { }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                    }

                    using (SqlConnection con4 = new SqlConnection(constr))
                    {
                        string str4 = @"UPDATE Transactiondetails set GRNStatus = @GRNStatus WHERE InwardID = @InwardID";

                        SqlCommand cmd4 = new SqlCommand(str4, con4);
                        cmd4.Parameters.AddWithValue("@GRNStatus", 13);
                        cmd4.Parameters.AddWithValue("@InwardID", lblPageID.Text);

                        con4.Open();
                        int e1 = cmd4.ExecuteNonQuery();
                        con4.Close();

                        if (e1 > 0)
                        {
                            var TableRowId1 = lblPageID.Text;
                            bool successes1 = dal.AuditLogData("Update GRN Details Status for Approved", "GRNDetails", TableRowId1, "");
                            if (successes1)
                            {
                                /* Add New Code start */
                                List<PurchaseOrder> filteredData111 = dal.GetPurchaseOrdersData();
                                List<PurchaseOrder> filteredData11 = filteredData111.Where(item => item.PONumber == lblPoNo.Text).ToList();
                                if (filteredData11 != null && filteredData11.Count > 0)
                                {
                                    PurchaseOrder selectedItem = filteredData11[0];
                                    int purchaseOrderID = Convert.ToInt32(selectedItem.PurchaseOrderID.ToString());
                                    using (SqlConnection con2 = new SqlConnection(constr))
                                    {
                                        string str2 = @"UPDATE PurchaseOrderDetails set POStatusID = @PO_StatusId, PO_StatusId = @PO_StatusId WHERE PurchaseOrderID = @PurchaseOrderID";
                                        SqlCommand cmd2 = new SqlCommand(str2, con2);
                                        cmd2.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderID);
                                        cmd2.Parameters.AddWithValue("@PO_StatusId", 13);
                                        con2.Open();
                                        int c = cmd2.ExecuteNonQuery();
                                        con2.Close();

                                        if (c > 0)
                                        {
                                            using (SqlConnection con12 = new SqlConnection(constr))
                                            {
                                                string str12 = @"UPDATE PurchaseOrder set PurchaseOrderStatus = @PurchaseOrderStatus WHERE PONumber = @PO_Number";
                                                SqlCommand cmd12 = new SqlCommand(str12, con12);
                                                cmd12.Parameters.AddWithValue("@PO_Number", lblPoNo.Text);
                                                cmd12.Parameters.AddWithValue("@PurchaseOrderStatus", 13);
                                                con12.Open();
                                                int c12 = cmd12.ExecuteNonQuery();
                                                con12.Close();

                                                if (c12 > 0)
                                                {

                                                }
                                                else
                                                {
                                                    throw new Exception("Failed to update PoUploadMaster.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Failed to update PoUploadMaster.");
                                        }
                                    }
                                }
                                /* Add New Code end */
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }

                            bool success = false;
                        //foreach (GridViewRow row in GridView1.Rows)
                        //{
                        //    int ITId = Convert.ToInt32(GridView1.DataKeys[row.RowIndex]["IT_Id"]);

                        //    using (SqlConnection con22 = new SqlConnection(constr))
                        //    {
                        //        string str11 = @"UPDATE ItemTransactionMaster set IT_Status = @IT_Status, 
                        //                            IT_CreatedEmployeeId = @IT_CreatedEmployeeId, IT_MasterCompanyId = @IT_MasterCompanyId,
                        //                            IT_UpdatedDateTime = @IT_UpdatedDateTime
                        //                            WHERE IT_Id = @IT_Id";

                        //        SqlCommand cmd22 = new SqlCommand(str11, con22);
                        //        cmd22.Parameters.AddWithValue("@IT_Id", ITId);
                        //        cmd22.Parameters.AddWithValue("@IT_Status", 13);
                        //        cmd22.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                        //        cmd22.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
                        //        cmd22.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);

                        //        con22.Open();
                        //        int k = cmd22.ExecuteNonQuery();
                        //        con22.Close();

                        //        if (k > 0)
                        //        {
                        //            var TableRowId2 = ITId.ToString();
                        //            bool successes2 = dal.AuditLogData("Update Item Transaction Status for Approved", "ItemTransactionMaster", TableRowId2, "");
                        //            if (successes2)
                        //            {
                        //            }
                        //            else
                        //            {
                        //                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                        //            }

                        //            success = true;
                        //            /*  Response.Redirect(Request.RawUrl);
                        //            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");   */
                        //        }
                        //        else
                        //        {
                        //            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                        //        }
                        //    }
                        //}

                        //if (success)
                        //{
                        //    //bool notificationSent = dal.NotificationSend("Department", "GRN Successfully Approved", "GRN No: " + lblGRNShow.Text + " -GRN Approval successfully, you can now proceed to item(s) store in Warehouse.", ViewState["GRN_EmployeeIdForNotification"].ToString());
                        //    //if (notificationSent)
                        //    //{
                        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Successfully Approve'); setTimeout(function() { window.location = 'GRNShowforApproved.aspx'; }, 100); </script>");
                        //    //}
                        //    //else
                        //    //{
                        //    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Approval was successful, but the notification was not sent!'); setTimeout(function() { window.location = 'GRNShowforApproved.aspx'; }, 100); </script>");
                        //    //}
                        //    /*Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Successfully Approve'); </script>");*/
                        //    //Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Successfully Approve'); setTimeout(function() { window.location = 'GRNShowforApproved.aspx'; }, 100); </script>");
                        //}
                        //else
                        //{
                        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "ErrorScript", "<script> alert('Update Failed for all rows'); </script>");
                        //}
                        }
                        else
                        {
                            throw new Exception("Failed to update GRNDetails.");
                        }
                    }

                    string encodedEventID = _cm.Encode(lblPageID.Text);

                    string url = $"GenerateGRN.aspx?ID={encodedEventID}";
                    //Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                    Response.Redirect(url);

                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
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

        protected void btnRejected_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = @"UPDATE TransactionMaster set GRNStatus = @GRN_Status, 
                                ApprovedBy = @GRN_ApprovedEmployeeID, ApprovedAt = @GRN_ApprovedDateTime
                                WHERE InwardID = @GRN_ID";

                SqlCommand cmd = new SqlCommand(str1, con);

                cmd.Parameters.AddWithValue("@GRN_ID", lblPageID.Text);
                cmd.Parameters.AddWithValue("@GRN_ApprovedEmployeeID", Session["EmployeeID"].ToString());
                cmd.Parameters.AddWithValue("@GRN_Status", 14);
                cmd.Parameters.AddWithValue("@GRN_ApprovedDateTime", GetDateTime);

                con.Open();
                int j = cmd.ExecuteNonQuery();
                con.Close();

                if (j > 0)
                {
                    var TableRowId = lblPageID.Text;
                    bool successes = dal.AuditLogData("Update GRN Master Status for Reject", "GRNMaster", TableRowId, "");
                    if (successes) { }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                    }

                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        int ITId = Convert.ToInt32(GridView1.DataKeys[row.RowIndex]["IT_Id"]);

                        using (SqlConnection con22 = new SqlConnection(constr))
                        {
                            string str11 = @"UPDATE ItemTransactionMaster set IT_Status = @IT_Status, 
                                             IT_CreatedEmployeeId = @IT_CreatedEmployeeId, IT_MasterCompanyId = @IT_MasterCompanyId,
                                             IT_UpdatedDateTime = @IT_UpdatedDateTime
                                             WHERE IT_Id = @IT_Id";

                            SqlCommand cmd22 = new SqlCommand(str11, con22);
                            cmd22.Parameters.AddWithValue("@IT_Id", ITId);
                            cmd22.Parameters.AddWithValue("@IT_Status", 14);
                            cmd22.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                            cmd22.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
                            cmd22.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);

                            con22.Open();
                            int k = cmd22.ExecuteNonQuery();
                            con22.Close();

                            if (k > 0)
                            {
                                var TableRowId2 = ITId.ToString();
                                bool successes2 = dal.AuditLogData("Update Item Transaction Status for Reject", "ItemTransactionMaster", TableRowId2, "");
                                if (successes2) { }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                }

                                //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                            }
                        }
                    }

                    bool notificationSent = dal.NotificationSend("Department", "GRN Rejected", "GRN No: " + lblGRNShow.Text + " -GRN Rejected, you can now proceed to Check documents...", ViewState["GRN_EmployeeIdForNotification"].ToString());
                    if (notificationSent)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Rejecte!'); setTimeout(function() { window.location = 'GRNShowforApproved.aspx'; }, 100); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Approval was successful, but the notification was not sent!'); setTimeout(function() { window.location = 'GRNShowforApproved.aspx'; }, 100); </script>");
                    }
                    //Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
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

        protected void btnCancle_Click(object sender, EventArgs e)
        {
            Response.Redirect("GRNShowforApproved.aspx");
        }


    }
}