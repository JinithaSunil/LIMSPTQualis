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
    public partial class GRNGeneratedEdit : System.Web.UI.Page
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

                            txtGRN.Text = decodedEventListId1;
                            txtGRN.Enabled = false;
                            txtPoNumber.Text = decodedEventListId2;
                            txtPoNumber.Enabled = false;

                            //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);

                            List<GRNMasterData> response = dal.GRNDetailsShow(lblPageData.Text);
                            List<GRNMasterData> filteredData1 = response.Where(item => item.GRN_NUMBER == lblPageData.Text && (item.GDStatusId == 3 || item.GDStatusId == 16)).ToList();
                            if (filteredData1 != null && filteredData1.Count > 0)
                            {
                                GridView1.Visible = true;
                                GridView1.DataSource = filteredData1;
                                GridView1.DataBind();
                                btnEditGRNImage.Enabled = true;
                                FileUpload1.Enabled = true;
                            }
                            else
                            {
                                btnEditGRNImage.Enabled = false;
                                lblMsg.Text = "GRN Approved so, you can't Edit!";
                                GridView1.Visible = false;
                                FileUpload1.Enabled = false;
                            }

                            List<GRNMasterData> res = dal.GRNBillImageShow();
                            List<GRNMasterData> resdata = res.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                            if (resdata != null && resdata.Count > 0)
                            {
                                rptGRNData.DataSource = resdata.Select(item => new
                                {
                                    FilePath = $"{Session["WebsiteURL"]}/UploadedFiles/{item.GRNFileName}",
                                    FileType = Path.GetExtension(item.GRNFileName).ToLower()
                                });
                                rptGRNData.DataBind();
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Record Not Found!');</script>");
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

        protected void btnRemoveImage_Click(object sender, EventArgs e)
        {
            try
            {
                //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                List<GRNMasterData> response = dal.GRNDetailsShow();
                List<GRNMasterData> filteredData1 = response.Where(item => item.GRN_NUMBER == lblPageData.Text && (item.GDStatusId == 3 || item.GDStatusId == 16)).ToList();
                if (filteredData1 != null && filteredData1.Count > 0)
                {
                    string imageUrl = (sender as Button)?.CommandArgument;
                    string fileName = Path.GetFileName(new Uri(imageUrl).AbsolutePath);

                    if (fileName != null)
                    {
                        List<GRNMasterData> res1 = dal.GRNBillImageShow();
                        List<GRNMasterData> resdata1 = res1.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                        if (resdata1 != null && resdata1.Count > 1)
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Record Found!');</script>");
                            List<GRNMasterData> res = dal.GRNBillImage();
                            List<GRNMasterData> resdata = res.Where(item => item.GRNFileName == fileName).ToList();

                            if (resdata != null && resdata.Count > 0)
                            {
                                GRNMasterData seletedItem = resdata[0];
                                int GRNFieldID = seletedItem.GRNFileId;

                                string filePath = Server.MapPath("~/UploadedFiles/" + fileName);
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                    string deleteQuery = "DELETE FROM GRNFileUpload WHERE GRNFileId = @GRNFileId";
                                    using (SqlCommand command = new SqlCommand(deleteQuery, con))
                                    {
                                        con.Open();
                                        command.Parameters.AddWithValue("@GRNFileId", GRNFieldID);
                                        int rowsAffected = command.ExecuteNonQuery();
                                        con.Close();

                                        if (rowsAffected > 0)
                                        {
                                            var TableRowId = GRNFieldID.ToString();
                                            bool success = dal.AuditLogData("GRN Bill Image Delete", "GRNFileUpload", TableRowId, "");
                                            if (success) { }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                            }
                                            Response.Redirect(Request.RawUrl);
                                            //ClientScript.RegisterStartupScript(this.GetType(), "RemoveSuccess", "alert('Image and Database Record Deleted successfully!'); ", true);
                                        }
                                        else
                                        {
                                            ClientScript.RegisterStartupScript(this.GetType(), "RemoveError", "alert('Image not found or already Deleted.');", true);
                                        }
                                    }
                                }
                                else
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "FileNotFoundError", "alert('Image file not found.');", true);
                                }
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Record Not Found!');</script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('one Image Required, so you can not Delete!');</script>");
                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('The Remove operation is not allowed because the GRN has already been approved!');</script>");
                }

            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);

                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
                /*  ClientScript.RegisterStartupScript(this.GetType(), "RemoveError", $"alert('Error: {ex.Message}');", true);  */
            }

        }

        protected void btnEditGRNImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileUpload1.HasFile)
                {
                    string fileName = System.IO.Path.GetFileName(FileUpload1.FileName);
                    var fileNames = FileUpload1.PostedFiles.Select(file => System.IO.Path.GetFileName(file.FileName));

                    using (SqlConnection connection2 = new SqlConnection(constr))
                    {
                        foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
                        {
                            con.Open();
                            int prevIdentity2 = dal.GetReseedIdentity("GRNFileUpload", "GRNFileId");
                            dal.ReseedIdentity("GRNFileUpload", prevIdentity2);
                            con.Close();

                            string uniqueFileName = dal.GetUniqueFileName(postedFile.FileName);

                            string filePath = Server.MapPath("~/UploadedFiles/" + uniqueFileName);
                            postedFile.SaveAs(filePath);

                            string insertFileQuery = @"INSERT INTO GRNFileUpload (GRN_ID, GRNO, GRNFileName) 
                                                       output INSERTED.GRNFileId values (@GRN_ID, @GRNO, @GRNFileName)";

                            SqlCommand command = new SqlCommand(insertFileQuery, connection2);
                            command.Parameters.AddWithValue("@GRN_ID", lblPageID.Text);
                            command.Parameters.AddWithValue("@GRNO", lblPageData.Text);
                            command.Parameters.AddWithValue("@GRNFileName", uniqueFileName);

                            connection2.Open();
                            int GRNFileId = (int)command.ExecuteScalar();
                            connection2.Close();

                            if (GRNFileId > 0)
                            {
                                var TableRowId4 = GRNFileId.ToString();
                                bool success4 = dal.AuditLogData("GRN File Upload", "GRNFileUpload", TableRowId4, "");
                                if (success4)
                                { }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                }
                            }
                        }


                        string str1 = @"UPDATE GRNMaster set GRN_Status = @GRN_Status, GRN_UpdatedDateTime = @GRN_UpdatedDateTime WHERE GRN_ID = @GRN_ID";

                        SqlCommand cmd = new SqlCommand(str1, con);

                        cmd.Parameters.AddWithValue("@GRN_ID", lblPageID.Text);
                        cmd.Parameters.AddWithValue("@GRN_ApprovedEmployeeID", Session["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@GRN_Status", 27);
                        cmd.Parameters.AddWithValue("@GRN_UpdatedDateTime", GetDateTime);
                        cmd.Parameters.AddWithValue("@GRN_ApprovedDateTime", GetDateTime);

                        con.Open();
                        int j = cmd.ExecuteNonQuery();
                        con.Close();

                        if (j > 0)
                        {
                            var TableRowId = lblPageID.Text;
                            bool successes = dal.AuditLogData("Update GRN Master Status for GRN Update", "GRNMaster", TableRowId, "");
                            if (successes) { }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                        }



                        List<GRNMasterData> response1 = dal.GRNDetailsShow();
                        List<GRNMasterData> filteredData2 = response1.Where(item => item.GRN_NUMBER == lblPageData.Text).ToList();

                        if (filteredData2.Count > 0)
                        {
                            foreach (GRNMasterData selectedItem in filteredData2)
                            {
                                string ITID = selectedItem.GDITId.ToString();

                                using (SqlConnection con22 = new SqlConnection(constr))
                                {
                                    string str11 = @"UPDATE ItemTransactionMaster set IT_Status = @IT_Status, IT_CreatedEmployeeId = @IT_CreatedEmployeeId, 
                                                     IT_MasterCompanyId = @IT_MasterCompanyId, IT_UpdatedDateTime = @IT_UpdatedDateTime WHERE IT_Id = @IT_Id";

                                    SqlCommand cmd22 = new SqlCommand(str11, con22);
                                    cmd22.Parameters.AddWithValue("@IT_Id", ITID);
                                    cmd22.Parameters.AddWithValue("@IT_Status", 27);
                                    cmd22.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                    cmd22.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
                                    cmd22.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);

                                    con22.Open();
                                    int k = cmd22.ExecuteNonQuery();
                                    con22.Close();

                                    if (k > 0)
                                    {
                                        var TableRowId2 = ITID.ToString();
                                        bool successes2 = dal.AuditLogData("Update Item Transaction Status for GRN Update", "ItemTransactionMaster", TableRowId2, "");
                                        if (successes2) { }
                                        else
                                        {
                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                        }
                                        Response.Redirect(Request.RawUrl);
                                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                    }
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                        }


                        //Response.Redirect(Request.RawUrl);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please select a file/Image first.');", true);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);
                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }


        }



    }
}