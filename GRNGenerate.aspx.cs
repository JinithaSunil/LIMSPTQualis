using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GRNGenerate : System.Web.UI.Page
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

                    btnUploadFile.Visible = false;
                    btnAddGRN.Enabled = false;

                    try
                    {

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

        protected void btnAddGRN_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string fileName = System.IO.Path.GetFileName(FileUpload1.FileName);
                var fileNames = FileUpload1.PostedFiles.Select(file => System.IO.Path.GetFileName(file.FileName));

                try
                {
                    List<string> errorMessages = new List<string>();

                    con.Open();
                    int prevIdentity = dal.GetReseedIdentity("GRNMaster", "GRN_ID");
                    dal.ReseedIdentity("GRNMaster", prevIdentity);
                    con.Close();

                    LicenseNumberGet();
                    string GRNNumber = ViewState["GRN_Number"].ToString();
                    string GRNCounter = ViewState["GRN_NumberCounter"].ToString();
                    int currentYear = Convert.ToInt32(ViewState["GRN_CurrentYear"].ToString());

                    string insertQuery = @"INSERT INTO GRNMaster
                                            (GRN_PoNumber, GRN_MasterCompanyId, GRN_EmployeeID, GRN_CompanyUnitId, GRN_Status, GRN_CounterYear, 
                                             GRN_CounterNumber, GRN_NUMBER, GRN_CreatedDateTime, GRN_ApprovedEmployeeID) 
                                            output INSERTED.GRN_ID values  
                                            (@GRN_PoNumber, @GRN_MasterCompanyId, @GRN_EmployeeID, @GRN_CompanyUnitId, @GRN_Status, @GRN_CounterYear, 
                                             @GRN_CounterNumber, @GRN_NUMBER, @GRN_CreatedDateTime, @GRN_ApprovedEmployeeID)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        //cmd.Parameters.AddWithValue("@GRN_PoNumber", ddlPoNo.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@GRN_PoNumber", txtPoNumber.Text);
                        cmd.Parameters.AddWithValue("@GRN_MasterCompanyId", Session["MasterCompanyId"].ToString());
                        cmd.Parameters.AddWithValue("@GRN_EmployeeID", Session["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@GRN_CompanyUnitId", Session["CompanyUnitId"].ToString());
                        cmd.Parameters.AddWithValue("@GRN_Status", 12);
                        cmd.Parameters.AddWithValue("@GRN_CounterYear", currentYear);
                        cmd.Parameters.AddWithValue("@GRN_CounterNumber", GRNCounter);
                        cmd.Parameters.AddWithValue("@GRN_NUMBER", GRNNumber);
                        cmd.Parameters.AddWithValue("@GRN_CreatedDateTime", GetDateTime);
                        cmd.Parameters.AddWithValue("@GRN_ApprovedEmployeeID", 0);

                        con.Open();
                        int GRN_ID = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (GRN_ID > 0)
                        {
                            var TableRowId = GRN_ID.ToString();
                            bool success = dal.AuditLogData("GRNMaster Data Add", "GRNMaster", TableRowId, "");
                            if (success)
                            {

                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed when ItemTransactionMaster update!.'); </script>");
                            }

                            using (SqlConnection connection = new SqlConnection(constr))
                            {
                                foreach (GridViewRow row in GridView1.Rows)
                                {
                                    con.Open();
                                    int prevIdentity1 = dal.GetReseedIdentity("GRNDetails", "GRNDetailsID");
                                    dal.ReseedIdentity("GRNDetails", prevIdentity1);
                                    con.Close();

                                    int ITId = Convert.ToInt32(GridView1.DataKeys[row.RowIndex]["IT_ID"]);
                                    ViewState["ITIDset"] = ITId;
                                    string itemCode = row.Cells[0].Text;
                                    string itemName = row.Cells[1].Text;
                                    decimal orderQty = Convert.ToDecimal(row.Cells[2].Text);
                                    decimal receivedQty = Convert.ToDecimal(row.Cells[3].Text);
                                    decimal balanceQty = Convert.ToDecimal(row.Cells[4].Text);

                                    List<ItemTransactionData> response = dal.ItemTransactionDataGet();
                                    List<ItemTransactionData> filteredData = response.Where(item => item.IT_Id == ITId).ToList();

                                    if (filteredData != null && filteredData.Count > 0)
                                    {
                                        ItemTransactionData selectedItem = filteredData[0];
                                        //string IGIR_No = selectedItem.IT_IgirNumber.ToString();
                                        //string projectCode = selectedItem.IT_ProjectID.ToString();

                                        int poId = selectedItem.IT_PoId;

                                        string str = @"INSERT INTO GRNDetails  
                                                        (GRN_ID, GRN, GDPoId, GDITId, GDPoNumber, GDPoItemCode, GDItemName, GDPoOrderQty, GDPoReceivedQty, GDPoBalanceQty, GDStatusId) 
                                                        output INSERTED.GRNDetailsID values
                                                        (@GRN_ID, @GRN, @GDPoId, @GDITId, @GDPoNumber, @GDPoItemCode, @GDItemName, @GDPoOrderQty, @GDPoReceivedQty, @GDPoBalanceQty, @GDStatusId)";

                                        SqlCommand cmd1 = new SqlCommand(str, connection);

                                        cmd1.Parameters.AddWithValue("@GRN_ID", GRN_ID);
                                        cmd1.Parameters.AddWithValue("@GRN", GRNNumber);
                                        cmd1.Parameters.AddWithValue("@GDPoId", poId);
                                        cmd1.Parameters.AddWithValue("@GDITId", ITId);
                                        cmd1.Parameters.AddWithValue("@GDPoNumber", txtPoNumber.Text);
                                        cmd1.Parameters.AddWithValue("@GDPoItemCode", itemCode);
                                        cmd1.Parameters.AddWithValue("@GDItemName", itemName);
                                        cmd1.Parameters.AddWithValue("@GDPoOrderQty", orderQty);
                                        cmd1.Parameters.AddWithValue("@GDPoReceivedQty", receivedQty);
                                        cmd1.Parameters.AddWithValue("@GDPoBalanceQty", balanceQty);
                                        cmd1.Parameters.AddWithValue("@GDStatusId", 12);

                                        connection.Open();
                                        int GRNDetailsID = (int)cmd1.ExecuteScalar();
                                        connection.Close();

                                        if (GRNDetailsID > 0)
                                        {
                                            var TableRowId1 = GRNDetailsID.ToString();
                                            /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                                            bool success1 = dal.AuditLogData("GRNDetails ADD", "GRNDetails", TableRowId1, "");
                                            if (success1)
                                            {
                                            }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                            }

                                            using (SqlConnection con2 = new SqlConnection(constr))
                                            {
                                                string str1 = @"UPDATE PurchaseOrderDetails set PO_StatusId = @PO_StatusId, PO_StatusForIT = @PO_StatusForIT
                                                                WHERE PODetailsID = @PODetailsID";

                                                SqlCommand cmd2 = new SqlCommand(str1, con2);
                                                cmd2.Parameters.AddWithValue("@PODetailsID", poId);
                                                cmd2.Parameters.AddWithValue("@PO_StatusId", 12);
                                                cmd2.Parameters.AddWithValue("@PO_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                                cmd2.Parameters.AddWithValue("@PO_CreatedEmployeeId", Session["EmployeeID"].ToString());
                                                cmd2.Parameters.AddWithValue("@PO_UpdatedDateTime", GetDateTime);
                                                cmd2.Parameters.AddWithValue("@PO_StatusForIT", 1);

                                                con2.Open();
                                                int j = cmd2.ExecuteNonQuery();
                                                con2.Close();

                                                if (j > 0)
                                                {
                                                    var TableRowId2 = poId.ToString();
                                                    bool success2 = dal.AuditLogData("Status Update in PurchaseOrder", "PurchaseOrder", TableRowId2, "");
                                                    if (success2)
                                                    {
                                                        /* PO MASTER STATUS SET */
                                                    }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed when ItemTransactionMaster update!.'); </script>");
                                                    }
                                                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                                }
                                                else
                                                {
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                                                }
                                            }

                                            using (SqlConnection con22 = new SqlConnection(constr))
                                            {
                                                string str11 = @"UPDATE ItemTransactionMaster set IT_Status = @IT_Status, 
                                                                    IT_CreatedEmployeeId = @IT_CreatedEmployeeId, IT_MasterCompanyId = @IT_MasterCompanyId,
                                                                    IT_UpdatedDateTime = @IT_UpdatedDateTime
                                                                    WHERE IT_Id = @IT_Id";

                                                SqlCommand cmd22 = new SqlCommand(str11, con22);
                                                cmd22.Parameters.AddWithValue("@IT_Id", ITId);
                                                cmd22.Parameters.AddWithValue("@IT_Status",12);
                                                cmd22.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                                cmd22.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
                                                cmd22.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);

                                                con22.Open();
                                                int k = cmd22.ExecuteNonQuery();
                                                con22.Close();

                                                if (k > 0)
                                                {
                                                    var TableRowId3 = ITId.ToString();
                                                    /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                                                    bool success3 = dal.AuditLogData("Status Update in ItemTransactionMaster", "ItemTransactionMaster", TableRowId3, "");
                                                    if (success3)
                                                    { }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed when ItemTransactionMaster update!.'); </script>");
                                                    }
                                                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                                }
                                                else
                                                {
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            errorMessages.Add("Failed to update Item Transaction for item: " + itemName);
                                        }
                                    }
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('GRN Generate But PO-Excel & Item Transaction Data not Updated!!'); setTimeout(function() { window.location = 'GRNGenerate.aspx'; }, 100); </script>");
                                    }
                                }
                            }

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
                                    command.Parameters.AddWithValue("@GRN_ID", GRN_ID);
                                    command.Parameters.AddWithValue("@GRNO", GRNNumber);
                                    command.Parameters.AddWithValue("@GRNFileName", uniqueFileName);

                                    connection2.Open();
                                    int GRNFileId = (int)command.ExecuteScalar();
                                    connection2.Close();

                                    if (GRNFileId <= 0)
                                    {
                                        errorMessages.Add("Failed to upload file: " + uniqueFileName);
                                    }
                                    else if (GRNFileId > 0)
                                    {
                                        var TableRowId4 = GRNFileId.ToString();
                                        /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                                        bool success4 = dal.AuditLogData("GRN File Upload", "GRNFileUpload", TableRowId4, "");
                                        if (success4)
                                        {                                            
                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('GRN Generate for GRN Approval'); setTimeout(function() { window.location = 'GRNGenerateShow.aspx'; }, 10);</script>");
                                        }
                                        else
                                        {
                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                        }
                                    }
                                }
                            }


                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
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
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please select a file/Image first.');", true);
            }

        }

        public void LicenseNumberGet()
        {
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now;
            DateTime startOfPeriod = new DateTime(currentYear, 4, 1);       /* Start on April 1st */
            DateTime endOfPeriod = new DateTime(currentYear + 1, 3, 31);    /* End on March 31st of next year */

            string GRN_Number;
            int counter = 1;

            if (currentDate >= startOfPeriod && currentDate <= endOfPeriod)
            {
                int imaxCounter = IGetMaxCounterForCurrentYearFromDatabase();
                if (imaxCounter > 0)
                {
                    counter = imaxCounter + 1;
                    GRN_Number = "GRN/" + currentYear + "-" + (currentYear + 1) + "/" + counter.ToString("D4");
                    ViewState["GRN_Number"] = GRN_Number;
                    ViewState["GRN_NumberCounter"] = counter.ToString("D4");
                    ViewState["GRN_CurrentYear"] = currentYear;
                }
                else
                {
                    counter = 1;
                    GRN_Number = "GRN/" + currentYear + "-" + (currentYear + 1) + "/" + counter.ToString("D4");
                    ViewState["GRN_Number"] = GRN_Number;
                    ViewState["GRN_NumberCounter"] = counter.ToString("D4");
                    ViewState["GRN_CurrentYear"] = currentYear;
                }
            }
            else
            {

                int imaxCounter = IGetMaxCounterForCurrentYearFromDatabase();
                if (imaxCounter > 0)
                {
                    counter = imaxCounter + 1;
                    GRN_Number = "GRN/" + (currentYear - 1) + "-" + (currentYear) + "/" + counter.ToString("D4");
                    ViewState["GRN_Number"] = GRN_Number;
                    ViewState["GRN_NumberCounter"] = counter.ToString("D4");
                    ViewState["GRN_CurrentYear"] = currentYear - 1;
                }
                else
                {
                    counter = 1;
                    GRN_Number = "GRN/" + (currentYear - 1) + "-" + (currentYear) + "/" + counter.ToString("D4");
                    ViewState["GRN_Number"] = GRN_Number;
                    ViewState["GRN_NumberCounter"] = counter.ToString("D4");
                    ViewState["GRN_CurrentYear"] = currentYear - 1;
                }
            }

            ViewState["GRN_Number"] = GRN_Number;
            ViewState["GRN_NumberCounter"] = counter.ToString("D4");
        }

        private int IGetMaxCounterForCurrentYearFromDatabase()
        {
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now;
            DateTime startOfPeriod = new DateTime(currentYear, 4, 1); // Start on April 1st
            DateTime endOfPeriod = new DateTime(currentYear + 1, 3, 31); // End on March 31st of next year

            string GRN_Number;
            int counter = 1;
            int imaxCounter = 0;

            if (currentDate >= startOfPeriod && currentDate <= endOfPeriod)
            {
                string query = "SELECT MAX(GRN_CounterNumber) FROM GRNMaster WHERE GRN_CounterYear = @GRN_CounterYear";
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@GRN_CounterYear", currentYear);
                con.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    imaxCounter = Convert.ToInt32(result);
                }
                con.Close();

                return imaxCounter;
            }
            else
            {
                string query = "SELECT MAX(GRN_CounterNumber) FROM GRNMaster WHERE GRN_CounterYear = @GRN_CounterYear";
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@GRN_CounterYear", currentYear - 1);
                con.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    imaxCounter = Convert.ToInt32(result);
                }
                con.Close();

                return imaxCounter;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string getText = txtPoNumber.Text;

            List<ItemTransactionData> response = dal.ItemTransactionDataGet();
            List<ItemTransactionData> filteredData = response.Where(item => item.IT_Status == 11 && item.IT_PoNumber == getText).ToList();

            if (filteredData != null && filteredData.Count > 0)
            {
                ItemTransactionData selectedItem = filteredData[0];
                GridView1.DataSource = filteredData;
                GridView1.DataBind();
                btnAddGRN.Enabled = true;
            }
            else
            {
                btnAddGRN.Enabled = false;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('GRN Generated or No Data Found !!'); setTimeout(function() { window.location = 'GRNGenerate.aspx'; }, 100); </script>");
            }
        }


        [WebMethod]
        public static string[] GetSuggestions(string prefixText, int count)
        {
            DataAccessLayer dal = new DataAccessLayer();
            List<string> suggestions = dal.PoNumberSearchNew(prefixText);
            return suggestions.ToArray();
        }

    }
}