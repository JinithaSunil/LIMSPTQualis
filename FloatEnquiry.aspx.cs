using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class FloatEnquiry : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDivisions();
                BindPurchaseGroup();
                BindDocsGridView();
                BindAllEnquiryDetails(0);
            }
        }

        //Binds
        private void BindDivisions()
        {
            DataTable dt = _cm.GetAllDivisions("LIMS");
            if (dt.Rows.Count > 0)
            {
                ddlDivision.DataSource = dt;
                ddlDivision.DataTextField = "DivisionName";
                ddlDivision.DataValueField = "DivisionID";
                ddlDivision.DataBind();
                ddlDivision.Items.Insert(0, new ListItem("Select Department", ""));
            }

        }
        protected void BindPurchaseSubGroup(string PurchaseGroupID)
        {
            DataTable dt = new DataTable();
            dt = _cm.GetAllPurchaseSubGroup(PurchaseGroupID);
            ddlPurchaseSubGroup.DataSource = dt;
            ddlPurchaseSubGroup.DataTextField = "PurchaseSubGroupName";
            ddlPurchaseSubGroup.DataValueField = "PurchaseSubGroupID";
            ddlPurchaseSubGroup.DataBind();
            ddlPurchaseSubGroup.Items.Insert(0, new ListItem("Select Sub Group", "NA"));
        }
        private void BindPurchaseGroup()
        {
            DataTable dt = _cm.FillParentTable();
            if (dt.Rows.Count > 0)
            {
                ddlPurchaseGroup.DataSource = dt;
                ddlPurchaseGroup.DataTextField = "PurchaseGroupName";
                ddlPurchaseGroup.DataValueField = "PurchaseGroupID";
                ddlPurchaseGroup.DataBind();
                ddlPurchaseGroup.Items.Insert(0, new ListItem("Select Purchase Group", "NA"));

            }
        }
        private void BindAllEnquiryDetails(int index)
        {
            int PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvEnquiry.PageSize = PageSize;

            int StartIndex = index * PageSize;

            try
            {
                DataTable dt = _cm.GetAllDetailsForFloatEnquiries(StartIndex, PageSize);
                int TotalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"].ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    gvEnquiry.VirtualItemCount = TotalRecords;
                    gvEnquiry.DataSource = dt;
                    gvEnquiry.DataBind();

                }
                else
                {
                    gvEnquiry.DataSource = null;
                    gvEnquiry.DataBind();
                }
                spanCount.InnerHtml = TotalRecords.ToString();
            }
            catch (Exception ex)
            {
                // Add proper error handling
                ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
                    "alert('Error loading data: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }
        private void BindEnquiryDetails(string PurchaseGroupID, string PurchaseSubGroupID)
        {
            try
            {
                DataTable dt = _cm.GetDetailsForFloatEnquiries(PurchaseGroupID, PurchaseSubGroupID);
                int TotalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"].ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    gvEnquiry.VirtualItemCount = TotalRecords;
                    gvEnquiry.DataSource = dt;
                    gvEnquiry.DataBind();
                    //divGrid.Attributes.Add("style", "width: 1120px; height: 250px; overflow: scroll");

                }
                else
                {
                    gvEnquiry.DataSource = null;
                    gvEnquiry.DataBind();
                }
                spanCount.InnerHtml = TotalRecords.ToString();
            }
            catch (Exception ex)
            {
                // Add proper error handling
                ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
                    "alert('Error loading data: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }
        private void BindDocsGridView()
        {
            if (ViewState["UploadedFiles"] != null)
            {
                gvAttach.DataSource = (DataTable)ViewState["UploadedFiles"];
                gvAttach.DataBind();
            }
        }

        //ddl's
        protected void ddlPurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string PurchaseGroupID = ddlPurchaseGroup.SelectedValue.ToString();
                Session["PurchaseGroupID"] = PurchaseGroupID;
                BindPurchaseSubGroup(PurchaseGroupID);
            }
            catch (Exception ex)
            {
                string s1 = ex.Message;
                Response.Write(s1);
                LogExceptionToFile(ex);

            }

        }
        private void LogExceptionToFile(Exception ex)
        {
            try
            {
                // Define the file path (use Server.MapPath to get a physical path)
                // Ensure the 'App_Data' folder exists in your project for storing logs
                string logFilePath = Server.MapPath("~/App_Data/ErrorLog.txt");

                // Use StreamWriter to append the exception details to the file
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine("--------------------------------------------------");
                    sw.WriteLine("Date: " + DateTime.Now.ToString());
                    sw.WriteLine("Message: " + ex.Message);
                    sw.WriteLine("Source: " + ex.Source);
                    // This is the full stack trace
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                    sw.WriteLine("--------------------------------------------------");
                    sw.WriteLine();
                }
            }
            catch (Exception logEx)
            {
                // Handle potential errors during the logging process itself
                // For example, if the file is locked or the path is invalid
                // You might consider using System.Diagnostics.Debug.WriteLine() here
                // if logging fails.
                System.Diagnostics.Debug.WriteLine("Error during logging: " + logEx.Message);
            }
        }
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelectAll.Checked;

            foreach (GridViewRow row in gvSuppliers.Rows)
            {
                CheckBox chkSelect = row.FindControl("chkSelectVendor") as CheckBox;
                if (chkSelect != null)
                {
                    chkSelect.Checked = isChecked;
                }
            }
        }
        protected void chkSelectAllEnquiries_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = CheckBox1.Checked;

            foreach (GridViewRow row in gvEnquiry.Rows)
            {
                CheckBox chkSelect = row.FindControl("chkFloat") as CheckBox;
                if (chkSelect != null)
                {
                    chkSelect.Checked = isChecked;
                }
            }
        }
        protected void ddlPurchaseSubGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPurchaseSubGroup.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Select Sub Group!');", true);
            }
            else
            {
                // Bind the enquiry details
                BindEnquiryDetails(ddlPurchaseGroup.SelectedValue, ddlPurchaseSubGroup.SelectedValue);

                // Bind the supplier list
                BindSupplierList(ddlPurchaseSubGroup.SelectedValue);
            }
        }
        private void BindSupplierList(string purchaseSubGroupID)
        {
            // Get the vendor list using the existing method
            DataTable dtVendors = _cm.GetVendorList(purchaseSubGroupID);

            if (dtVendors != null && dtVendors.Rows.Count > 0)
            {
                gvSuppliers.DataSource = dtVendors;
                gvSuppliers.DataBind();
                gvSuppliers.Visible = true;
                chkSelectAll.Visible = true;
                chkSelectAll.Checked = false; // Reset the select all checkbox
            }
            else
            {
                gvSuppliers.DataSource = null;
                gvSuppliers.DataBind();
                gvSuppliers.Visible = false;
                chkSelectAll.Visible = false;

                // Show message when no suppliers found
                ScriptManager.RegisterStartupScript(this, this.GetType(), "noSuppliers",
                    "alert('No suppliers found for the selected sub group.');", true);
            }
        }
        protected void gvEnquiry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add onclick event to each row
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gvEnquiry, "Select$" + e.Row.RowIndex);
                e.Row.Style["cursor"] = "pointer";
            }
        }
        protected void gvEnquiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvEnquiry.Rows)
            {
                if (row.RowIndex == gvEnquiry.SelectedIndex)
                {
                    row.BackColor = System.Drawing.Color.LightBlue;
                    CheckBox chkFloat = (CheckBox)row.FindControl("chkFloat");
                    if (chkFloat != null)
                    {
                        chkFloat.Checked = true;
                    }
                }
                else
                {
                    row.BackColor = System.Drawing.Color.White;
                    CheckBox chkFloat = (CheckBox)row.FindControl("chkFloat");
                    if (chkFloat != null)
                    {
                        chkFloat.Checked = false;
                    }
                }
            }
        }
        protected void gvAttach_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["UploadedFiles"];

                if (dt != null && dt.Rows.Count > index)
                {
                    dt.Rows.RemoveAt(index);
                    ViewState["UploadedFiles"] = dt;
                    gvAttach.DataSource = dt;
                    gvAttach.DataBind();
                }
            }
        }
        private void AddFileToGrid(string fileName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FileName", typeof(string));

            DataRow dr = dt.NewRow();
            dr["FileName"] = fileName;
            dt.Rows.Add(dr);

            ViewState["UploadedFiles"] = dt;
            gvAttach.DataSource = dt;
            gvAttach.DataBind();
        }

        //buttons 
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fpAttach.HasFile)
            {
                try
                {
                    // Validate if a file is already uploaded
                    if (ViewState["UploadedFiles"] != null)
                    {
                        lblMessage.Text = "Only one file can be uploaded per Enquiry!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string uploadFolder = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string fileName = Path.GetFileName(fpAttach.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);
                    fpAttach.SaveAs(filePath); // Save file to server

                    // Store file in Hidden Field
                    hdnFileUpload.Value = fileName;

                    // Save to ViewState to restrict multiple files
                    AddFileToGrid(fileName);

                    lblMessage.Text = "File uploaded successfully!";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblMessage.Text = "Please select a file first!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
        private (string itemCode, string itemID) GetItemFromGridRow(GridViewRow row, string prID)
        {
            try
            {
                // Get all items for this PRID
                var allItems = GetAllPurchaseRequisitionItems(prID);

                // Get the row index to determine which item this represents
                int rowIndex = row.RowIndex;

                if (rowIndex < allItems.Count)
                {
                    return (allItems[rowIndex].ItemCode, allItems[rowIndex].ItemID);
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // For now, return empty values
            }

            return ("", "");
        }
        private bool ValidateSelectedVendors(out string errorMessage)
        {
            errorMessage = "";
            List<string> vendorsWithoutEmail = new List<string>();

            foreach (GridViewRow row in gvSuppliers.Rows)
            {
                CheckBox chkSelectVendor = (CheckBox)row.FindControl("chkSelectVendor");

                if (chkSelectVendor != null && chkSelectVendor.Checked)
                {
                    // FIXED: Email is at index 4 (ContactEmail column), Vendor Name is at index 3
                    string email = row.Cells[4].Text.Trim();  // Changed from 3 to 4
                    string vendorName = row.Cells[3].Text.Trim();  // Changed from 2 to 3

                    // Decode HTML entities like &nbsp;
                    email = System.Web.HttpUtility.HtmlDecode(email);

                    // Check if email is empty or whitespace
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        vendorsWithoutEmail.Add(vendorName);
                    }
                }
            }

            if (vendorsWithoutEmail.Count > 0)
            {
                errorMessage = "The following vendor(s) do not have email addresses:\\n\\n" +
                               string.Join("\\n", vendorsWithoutEmail) +
                               "\\n\\nPlease add email addresses or uncheck these vendors.";
                return false;
            }

            return true;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<int> enquiryIDs = new List<int>();
            string remarks = txtRemarks.Text.Trim();

            // Get selected vendor IDs first
            List<string> selectedVendorIDs = GetSelectedVendorIDs();
            if (selectedVendorIDs.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "VendorAlert",
                    "alert('No vendors selected! Please select at least one vendor.');", true);
                return;
            }

            string validationError;
            if (!ValidateSelectedVendors(out validationError))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "VendorEmailAlert",
                    "alert(\"" + validationError.Replace("\"", "\\\"") + "\");", true);
                return;
            }

            bool anyRowSelected = false;

            // DEBUG: Add this to check if gvEnquiry has rows
            if (gvEnquiry.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoDataAlert",
                    "alert('No enquiry data found! Please select Purchase Group and Sub Group first.');", true);
                return;
            }

            // FIXED: Collect all selected items first, then create ONE enquiry
            List<EnquiryItem> selectedItems = new List<EnquiryItem>();
            foreach (GridViewRow row in gvEnquiry.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkFloat");

                if (chk == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "CheckboxAlert",
                        $"alert('Checkbox not found in row {row.RowIndex}');", true);
                    continue;
                }

                if (chk != null && chk.Checked)
                {
                    anyRowSelected = true;

                    // FIXED: Better way to get PRID
                    HyperLink hypPRFinal = (HyperLink)row.FindControl("hypPRFinal");
                    if (hypPRFinal == null || string.IsNullOrEmpty(hypPRFinal.NavigateUrl))
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "PRIDAlert",
                            $"alert('Cannot find PR ID for row {row.RowIndex}');", true);
                        continue;
                    }

                    string prID = "";
                    try
                    {
                        // Extract PRID from NavigateUrl more safely
                        if (hypPRFinal.NavigateUrl.Contains("ID="))
                        {
                            prID = hypPRFinal.NavigateUrl.Split('=')[1];
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "PRIDFormatAlert",
                                $"alert('Invalid PR ID format in row {row.RowIndex}');", true);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "PRIDExceptionAlert",
                            $"alert('Error extracting PR ID from row {row.RowIndex}: {ex.Message}');", true);
                        continue;
                    }

                    TextBox txtEnquiryQty = (TextBox)row.FindControl("txtEnquiryQty");
                    if (txtEnquiryQty == null)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "QtyTextboxAlert",
                            $"alert('Quantity textbox not found in row {row.RowIndex}');", true);
                        continue;
                    }

                    string enquiryQty = txtEnquiryQty.Text.Trim();

                    // Validate enquiry quantity
                    if (string.IsNullOrEmpty(enquiryQty) || !decimal.TryParse(enquiryQty, out decimal qty) || qty <= 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "QuantityAlert",
                            "alert('Please enter a valid quantity for each selected item.');", true);
                        return;
                    }

                    // Get the specific item for this row
                    HiddenField hdnItemCode = (HiddenField)row.FindControl("hdnItemCode");
                    HiddenField hdnItemID = (HiddenField)row.FindControl("hdnItemID");
                    string currentItemCode = hdnItemCode?.Value ?? "";
                    string currentItemID = hdnItemID?.Value ?? "";

                    // Method 2: Alternative - Get item based on row index
                    if (string.IsNullOrEmpty(currentItemCode) && string.IsNullOrEmpty(currentItemID))
                    {
                        var (itemCode, itemID) = GetItemFromGridRow(row, prID);
                        currentItemCode = itemCode;
                        currentItemID = itemID;
                    }

                    // Fetch the specific item data
                    var (PurchaseRequisitionID, reportFileName, projectName, storeType, fetchedItemCode, fetchedItemID) =
                        GetPurchaseRequisitionDataByItem(prID, currentItemCode, currentItemID);

                    string gridRemarks = row.Cells[9].Text.Trim();

                    // FIXED: Add to collection instead of creating enquiry immediately
                    selectedItems.Add(new EnquiryItem
                    {
                        PRID = prID,
                        ItemCode = fetchedItemCode,
                        ItemID = fetchedItemID, // Keep as string since InsertEnquiryDetails expects string
                        Quantity = enquiryQty,
                        Remarks = gridRemarks,
                        PurchaseRequisitionID = PurchaseRequisitionID, // Keep as string
                        ReportFileName = reportFileName,
                        ProjectName = projectName,
                        StoreType = storeType
                    });
                }
            }

            // Check if any row was selected
            if (!anyRowSelected)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoRowAlert",
                    $"alert('No rows selected! Please select at least one enquiry to create. Total rows: {gvEnquiry.Rows.Count}');", true);
                return;
            }

            // FIXED: Create ONE enquiry for all selected items
            if (selectedItems.Count > 0)
            {
                // Use data from first item for enquiry header info
                var firstItem = selectedItems.First();

                // Create single enquiry
                int enquiryID = InsertEnquiry(remarks, firstItem.PRID, firstItem.ReportFileName, firstItem.ProjectName, firstItem.StoreType);

                if (enquiryID > 0)
                {
                    enquiryIDs.Add(enquiryID);

                    // FIXED: Create ONE VendorQuotation per vendor (moved outside the items loop)
                    Dictionary<int, int> vendorQuotationIDs = new Dictionary<int, int>();
                    foreach (string vendorID in selectedVendorIDs)
                    {
                        int vendorQuotationID = InsertVendorQuotation(enquiryID, Convert.ToInt32(vendorID));
                        vendorQuotationIDs.Add(Convert.ToInt32(vendorID), vendorQuotationID);
                    }

                    // FIXED: Create multiple EnquiryDetails records for the same EnquiryID
                    foreach (var item in selectedItems)
                    {
                        int enquiryDetailsID = InsertEnquiryDetails(enquiryID, item.ItemID, item.ItemCode, item.Quantity, item.Remarks);

                        // Insert VendorQuotationDetails for each vendor for this item
                        foreach (string vendorID in selectedVendorIDs)
                        {
                            int vendorQuotationID = vendorQuotationIDs[Convert.ToInt32(vendorID)];
                            int vendorQuotationDetailsID = InsertVendorQuotationDetails(
                                vendorQuotationID, enquiryDetailsID, Convert.ToInt32(vendorID), item.Quantity, item.PurchaseRequisitionID);
                        }

                        // Update enquiry in purchase requisition details for each item
                        UpdateEnquiryInPurchaseRequisitionDetails(item.PurchaseRequisitionID, enquiryID);
                    }

                    // Update enquiry vendors once for the entire enquiry
                    UpdateEnquiryVendors(enquiryID, selectedVendorIDs);
                }
            }

            // Handle file upload if a file is selected
            if (enquiryIDs.Count > 0 && !string.IsNullOrEmpty(hdnFileUpload.Value))
            {
                foreach (int enquiryID in enquiryIDs)
                {
                    SaveEnquiryDocument(enquiryID, hdnFileUpload.Value);
                }
            }

            // Send emails to selected vendors if at least one enquiry was created
            if (enquiryIDs.Count > 0)
            {
                // Generate reports for ALL enquiries
                GenerateReportsForAllEnquiries(enquiryIDs);

                SendEmailsToSelectedVendors(selectedVendorIDs, enquiryIDs);

                // Show success message and redirect to enquiry list or first report
                ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert",
                    $"alert('Data uploaded successfully! {enquiryIDs.Count} enquiry report(s) generated with {selectedItems.Count} items and emails sent to {selectedVendorIDs.Count} vendor(s)!'); " +
                    $"window.location.href='EnquiryList.aspx';", true);
            }

            // Clear ViewState and GridView
            ViewState["UploadedFiles"] = null;
            gvAttach.DataSource = null;
            gvAttach.DataBind();
            hdnFileUpload.Value = "";
            txtRemarks.Text = "";
        }
        protected string GetVendorProfileUrl(object vendorID)
        {
            if (vendorID == null || string.IsNullOrEmpty(vendorID.ToString()))
                return "#";

            string vendorCode = GetVendorCodeByID(vendorID.ToString());

            if (!string.IsNullOrEmpty(vendorCode))
            {
                return "VendorProfile.aspx?ID=" + vendorCode;
            }

            return "#";
        }

        private string GetVendorCodeByID(string vendorID)
        {
            string vendorCode = "";

            string query = "SELECT VendorCode FROM VendorDetails WHERE VendorID = @VendorID";


            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", vendorID);
                        con.Open();

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            vendorCode = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                // For now, return empty string
            }

            return vendorCode;
        }
        public class EnquiryItem
        {
            public string PRID { get; set; }
            public string ItemCode { get; set; }
            public string ItemID { get; set; }
            public string Quantity { get; set; }
            public string Remarks { get; set; }
            public string PurchaseRequisitionID { get; set; }
            public string ReportFileName { get; set; }
            public string ProjectName { get; set; }
            public string StoreType { get; set; }
        }

        // NEW METHOD: Generate reports for all enquiries using the existing GenerateEnquiryReport page
        private void GenerateReportsForAllEnquiries(List<int> enquiryIDs)
        {
            try
            {
                // Create an instance of the GenerateEnquiryReport page to use its methods
                GenerateEnquiryReport reportPage = new GenerateEnquiryReport();

                foreach (int enquiryID in enquiryIDs)
                {
                    bool success = reportPage.GenerateIndividualReport(enquiryID);

                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine($"Report generated successfully for EnquiryID: {enquiryID}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to generate report for EnquiryID: {enquiryID}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating reports: {ex.Message}");
            }
        }
        // FIXED: Better GetSelectedVendorIDs method
        private List<string> GetSelectedVendorIDs()
        {
            List<string> selectedVendorIDs = new List<string>();

            try
            {
                // Check if supplier grid exists and has rows
                if (gvSuppliers == null)
                {
                    System.Diagnostics.Debug.WriteLine("gvSuppliers is null");
                    return selectedVendorIDs;
                }

                if (gvSuppliers.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No supplier rows found");
                    return selectedVendorIDs;
                }

                foreach (GridViewRow row in gvSuppliers.Rows)
                {
                    try
                    {
                        CheckBox chk = (CheckBox)row.FindControl("chkSelectVendor");
                        HiddenField hdnVendorID = (HiddenField)row.FindControl("hdnVendorID");

                        if (chk != null && chk.Checked && hdnVendorID != null)
                        {
                            string vendorID = hdnVendorID.Value;
                            if (!string.IsNullOrEmpty(vendorID))
                            {
                                selectedVendorIDs.Add(vendorID);
                                System.Diagnostics.Debug.WriteLine($"Added vendor ID: {vendorID}");
                            }
                        }
                    }
                    catch (Exception rowEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing row {row.RowIndex}: {rowEx.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Selected vendor count: {selectedVendorIDs.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetSelectedVendorIDs: {ex.Message}");
                // Return empty list on error
            }

            return selectedVendorIDs;
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Ensure ViewState is maintained for checkboxes
            if (gvEnquiry.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvEnquiry.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkFloat");
                    if (chk != null)
                    {
                        // Ensure checkbox maintains its state
                        chk.EnableViewState = true;
                    }
                }
            }

            if (gvSuppliers.Rows.Count > 0)
            {
                foreach (GridViewRow row in gvSuppliers.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelectVendor");
                    if (chk != null)
                    {
                        // Ensure checkbox maintains its state
                        chk.EnableViewState = true;
                    }
                }
            }
        }
        private List<(string PurchaseRequisitionID, string ReportFileName, string ProjectName, string StoreType, string ItemCode, string ItemID)> GetAllPurchaseRequisitionItems(string prID)
        {
            var items = new List<(string, string, string, string, string, string)>();

            string query = @"
                SELECT pd.PurchaseRequisitionID, pr.ReportFileName, pd.ProjectName, pr.StoreType, i.ItemCode, i.ItemID
                FROM PurchaseRequisition pr
                LEFT JOIN PurchaseRequisitionDetails pd ON pr.PRID = pd.PRID
                LEFT JOIN Items i ON i.ItemCode = pd.ItemCode
                WHERE pd.PRID = @PRID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PRID", prID);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // Use while instead of if to get all rows
                        {
                            items.Add((
                                reader["PurchaseRequisitionID"]?.ToString(),
                                reader["ReportFileName"]?.ToString(),
                                reader["ProjectName"]?.ToString(),
                                reader["StoreType"]?.ToString(),
                                reader["ItemCode"]?.ToString(),
                                reader["ItemID"]?.ToString()
                            ));
                        }
                    }
                }
            }
            return items;
        }

        //Saving data in DB
        private (string PurchaseRequisitionID, string ReportFileName, string ProjectName, string StoreType, string ItemCode, string ItemID) GetPurchaseRequisitionDataByItem(string prID, string specificItemCode = null, string specificItemID = null)
        {
            string PurchaseRequisitionID = "", reportFileName = "", projectName = "", storeType = "", itemCode = "", ItemID = "";

            string query = @"
                SELECT pd.PurchaseRequisitionID, pr.ReportFileName, pd.ProjectName, pr.StoreType, i.ItemCode, i.ItemID
                FROM PurchaseRequisition pr
                LEFT JOIN PurchaseRequisitionDetails pd ON pr.PRID = pd.PRID
                LEFT JOIN Items i ON i.ItemCode = pd.ItemCode
                WHERE pd.PRID = @PRID";

            // Add condition to filter by specific item if provided
            if (!string.IsNullOrEmpty(specificItemCode))
            {
                query += " AND i.ItemCode = @ItemCode";
            }
            else if (!string.IsNullOrEmpty(specificItemID))
            {
                query += " AND i.ItemID = @ItemID";
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PRID", prID);

                    if (!string.IsNullOrEmpty(specificItemCode))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", specificItemCode);
                    }
                    else if (!string.IsNullOrEmpty(specificItemID))
                    {
                        cmd.Parameters.AddWithValue("@ItemID", specificItemID);
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PurchaseRequisitionID = reader["PurchaseRequisitionID"]?.ToString();
                            reportFileName = reader["ReportFileName"]?.ToString();
                            projectName = reader["ProjectName"]?.ToString();
                            storeType = reader["StoreType"]?.ToString();
                            itemCode = reader["ItemCode"]?.ToString();
                            ItemID = reader["ItemID"]?.ToString();
                        }
                    }
                }
            }
            return (PurchaseRequisitionID, reportFileName, projectName, storeType, itemCode, ItemID);
        }
        private int InsertEnquiry(string remarks, string prID, string reportFileName, string projectName, string storeType)
        {
            int enquiryID = 0;

            string query = @"
    INSERT INTO Enquiry (PurchaseSubGroupID, created, Remarks, UserID, ReportFileName, ProjectName, StoreType, EnquiryStatus) 
    OUTPUT INSERTED.EnquiryID
    VALUES (@PurchaseSubGroupID, GETDATE(), @Remarks, @UserID, @ReportFileName, @ProjectName, @StoreType, @EnquiryStatus)";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PurchaseSubGroupID", ddlPurchaseSubGroup.SelectedValue);
                    cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? DBNull.Value : (object)remarks);
                    cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@EnquiryStatus", 1);
                    cmd.Parameters.AddWithValue("@ReportFileName", string.IsNullOrEmpty(reportFileName) ? DBNull.Value : (object)reportFileName);
                    cmd.Parameters.AddWithValue("@ProjectName", string.IsNullOrEmpty(projectName) ? DBNull.Value : (object)projectName);
                    cmd.Parameters.AddWithValue("@StoreType", string.IsNullOrEmpty(storeType) ? DBNull.Value : (object)storeType);

                    conn.Open();
                    enquiryID = (int)cmd.ExecuteScalar();
                }
            }
            return enquiryID;
        }
        private int InsertEnquiryDetails(int enquiryID, string ItemID, string itemCode, string quantity, string remarks)
        {
            int enquiryDetailsID = 0; // Store the inserted ID

            string query = @"
    INSERT INTO EnquiryDetails (EnquiryID, ItemCode, Quantity, Remarks, EnquiryDetailsStatus, ItemID) 
    OUTPUT INSERTED.EnquiryDetailsID 
    VALUES (@EnquiryID, @ItemCode, @Quantity, @Remarks, @EnquiryDetailsStatus, @ItemID)";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                    cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Remarks", remarks);
                    cmd.Parameters.AddWithValue("@EnquiryDetailsStatus", "1"); // Initial status
                    cmd.Parameters.AddWithValue("@ItemID", ItemID);

                    conn.Open();
                    enquiryDetailsID = Convert.ToInt32(cmd.ExecuteScalar()); // Retrieve the inserted ID
                }
            }

            return enquiryDetailsID; // Return the inserted ID
        }
        private void SaveEnquiryDocument(int enquiryID, string fileName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO EnquiryDocuments (EnquiryID, FileName, CreatedDate) VALUES (@EnquiryID, @FileName, @CreatedDate)", conn))
                    {
                        cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                        cmd.Parameters.AddWithValue("@FileName", fileName);
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Error saving file: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }
        private void UpdateEnquiryVendors(int enquiryID, List<string> vendorIDs)
        {
            // Join all vendorIDs with comma separation
            string combinedVendorIDs = string.Join(",", vendorIDs);

            // Update the Enquiry record with the combined vendor IDs
            string query = "UPDATE Enquiry SET VendorID = @VendorID WHERE EnquiryID = @EnquiryID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                    cmd.Parameters.AddWithValue("@VendorID", combinedVendorIDs);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private int InsertVendorQuotation(int enquiryID, int vendorID)
        {
            int vendorQuotationID = 0;
            string query = @"
        INSERT INTO VendorQuotation (EnquiryID, VendorID, StatusID, CreatedDate) 
        OUTPUT INSERTED.VendorQuotationID 
        VALUES (@EnquiryID, @VendorID, @StatusID, @CreatedDate)";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                    cmd.Parameters.AddWithValue("@VendorID", vendorID);
                    cmd.Parameters.AddWithValue("@StatusID", 1); // Assuming 1 is the initial status
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    con.Open();
                    vendorQuotationID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return vendorQuotationID;
        }
        private int InsertVendorQuotationDetails(int vendorQuotationID, int enquiryDetailsID, int vendorID, string orderedQty, string purchaseRequisitionID)
        {
            int vendorQuotationDetailsID = 0;
            string query = @"
        INSERT INTO VendorQuotationDetails (VendorQuotationID, EnquiryDetailsID, VendorID, OrderedQty, PurchaseRequisitionID, StatusID, PurchaseRequisitionNumber) 
        OUTPUT INSERTED.VendorQuotationDetailsID 
        VALUES (@VendorQuotationID, @EnquiryDetailsID, @VendorID, @OrderedQty, @PurchaseRequisitionID, @StatusID, @PurchaseRequisitionNumber)";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@VendorQuotationID", vendorQuotationID);
                    cmd.Parameters.AddWithValue("@EnquiryDetailsID", enquiryDetailsID);
                    cmd.Parameters.AddWithValue("@VendorID", vendorID);
                    cmd.Parameters.AddWithValue("@OrderedQty", orderedQty);
                    cmd.Parameters.AddWithValue("@PurchaseRequisitionID", purchaseRequisitionID);
                    cmd.Parameters.AddWithValue("@StatusID", 1); // Assuming 1 is the initial status
                    cmd.Parameters.AddWithValue("@PurchaseRequisitionNumber", "PR-" + purchaseRequisitionID); // Example format

                    con.Open();
                    vendorQuotationDetailsID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return vendorQuotationDetailsID;
        }
        private void UpdateEnquiryInPurchaseRequisitionDetails(string purchaseRequisitionID, int enquiryID)
        {
            string query = @"
        UPDATE PurchaseRequisitionDetails 
        SET EnquiryID = @EnquiryID , StatusID = 1008
        WHERE PurchaseRequisitionID = @PurchaseRequisitionID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                    cmd.Parameters.AddWithValue("@PurchaseRequisitionID", purchaseRequisitionID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Email
        private void SendMail(string VendorID, int EnquiryID)
        {
            try
            {
                _cm.EnquiryID = EnquiryID;
                DataTable dtVendor = _cm.GetVendorReference(VendorID);
                DataTable dtVendorEmail = _cm.GetVendorEmail(VendorID);

                if (dtVendor.Rows.Count > 0 && dtVendorEmail.Rows.Count > 0)
                {
                    _cm.VendorName = dtVendor.Rows[0]["VendorName"].ToString();
                    _cm.Reference = dtVendor.Rows[0]["Reference"].ToString();

                    // Build email addresses
                    string Email = "";
                    if (dtVendorEmail.Rows.Count > 1)
                    {
                        foreach (DataRow dr in dtVendorEmail.Rows)
                        {
                            Email += dr["ContactEmail"].ToString() + ",";
                        }
                        Email = Email.TrimEnd(',');
                    }
                    else
                    {
                        Email = dtVendorEmail.Rows[0]["ContactEmail"].ToString();
                    }
                    string ToAddress = Email;

                    // Get enquiry notes - Use the passed EnquiryID instead of session
                    DataTable dte = _cm.GetEnquiryNote(EnquiryID.ToString());
                    string Note = "";
                    if (dte.Rows.Count > 0)
                    {
                        Note = dte.Rows[0]["Remarks"].ToString();
                    }

                    // Get email configuration
                    DataTable dtEmailDetails = _cm.getEmaildetails("Float Enquiry");
                    string baseUrl = "http://localhost:41414/"; //  default
                    DataTable dtOrg = _cm.GetOrganizationDetails();
                    if (dtOrg.Rows.Count > 0 && dtOrg.Columns.Contains("url"))
                    {
                        baseUrl = dtOrg.Rows[0]["url"].ToString();
                        if (!baseUrl.EndsWith("/"))
                            baseUrl += "/";
                    }

                    if (dtEmailDetails.Rows.Count > 0)
                    {
                        string FromAddress = dtEmailDetails.Rows[0]["FromAddress"].ToString();
                        string CCAddress = dtEmailDetails.Rows[0]["CCAddress"].ToString();
                        string BCCAddress = dtEmailDetails.Rows[0]["BCCAddress"].ToString();
                        string Subject = "Enquiry#" + _cm.EnquiryID + dtEmailDetails.Rows[0]["EmailSubject"].ToString();

                        // Create the Link string
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(string.Format("<a href='" + baseUrl + "EnquiryForm.aspx?Reference={0}&Enquiry={1}' target='_blank'>", _cm.Reference, _cm.EnquiryID));
                        sb.AppendFormat("View Quotation");
                        sb.Append("</a>");

                        // Prepare body with template - use only the required placeholders
                        string body = objEmail.PrepareMailBodyWith("Enquiry.html", "{VendorName}", _cm.VendorName, "{Link}", sb.ToString(), "{Note}", Note);

                        // Save copy to disk
                        string filename = Server.MapPath("//Enquiry/Enquiry-" + _cm.EnquiryID + _cm.Reference + ".html");
                        System.IO.File.WriteAllText(@filename, body);

                        // Collect attachments - Use the passed EnquiryID instead of session
                        StringBuilder sbEnq = new StringBuilder();
                        DataTable dtEnq = _cm.GetEnquiryDocuments(EnquiryID.ToString());
                        if (dtEnq.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtEnq.Rows)
                            {
                                string attachmentPath = HttpContext.Current.Server.MapPath("//EnquiryDocuments/" + dr["FileName"].ToString());
                                if (File.Exists(attachmentPath))
                                {
                                    sbEnq.Append(attachmentPath + ";");
                                }
                            }
                        }

                        // Store email metadata
                        _cm.FromAddress = FromAddress;
                        _cm.ToAddress = ToAddress;
                        _cm.CCAddress = CCAddress;
                        _cm.BCCAddress = BCCAddress;
                        _cm.Subject = Subject;
                        _cm.Purpose = "Float Enquiry";
                        _cm.FileName = "Enquiry.html";
                        _cm.SentTo = _cm.VendorName;
                        _cm.Body = body;

                        // Send email
                        objEmail.SendMail(FromAddress, ToAddress, Subject, body, sbEnq.ToString().TrimEnd(';'));

                        // Handle success/failure
                        if (Session["Failed"].ToString() == "0")
                        {
                            _cm.SentMailStatus = true;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                        }
                        else
                        {
                            _cm.SentMailStatus = false;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        }
                        _cm.AddMailBoxDetails(_cm);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email configuration not found!');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Vendor Email Found! Kindly update Vendor Details!');", true);
                }
            }
            catch (Exception ex)
            {
                _cm.SentMailStatus = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed: " + ex.Message + "');", true);
                Session["ErrorMessage"] = ex.Message + "," + ex.Source + "," + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    Session["ErrorMessage"] += "," + ex.InnerException.Message;
                }
                Response.Redirect("Error.aspx");
            }
        }
        private void SendEmailsToSelectedVendors(List<string> vendorIDs, List<int> enquiryIDs)
        {
            try
            {
                // Validate input
                if (vendorIDs == null || vendorIDs.Count == 0)
                {
                    if (lblMessage != null)
                    {
                        lblMessage.Text = "No vendors selected for email sending!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                    return;
                }

                // Validate enquiry IDs
                if (enquiryIDs == null || enquiryIDs.Count == 0)
                {
                    if (lblMessage != null)
                    {
                        lblMessage.Text = "No enquiry IDs available for email sending!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                    return;
                }

                int emailsSent = 0;
                int emailsFailed = 0;

                // Send emails for each enquiry to each vendor
                foreach (int enquiryID in enquiryIDs)
                {
                    foreach (string vendorID in vendorIDs)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(vendorID))
                            {
                                // Send mail to this vendor for this specific enquiry ID
                                SendMail(vendorID, enquiryID);
                                emailsSent++;
                            }
                            else
                            {
                                emailsFailed++;
                                System.Diagnostics.Debug.WriteLine("Empty vendor ID encountered");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with other vendors
                            emailsFailed++;
                            System.Diagnostics.Debug.WriteLine($"Failed to send email to vendor {vendorID} for enquiry {enquiryID}: {ex.Message}");
                        }
                    }
                }

                // Update the message with email sending results
                if (lblMessage != null)
                {
                    if (emailsSent > 0)
                    {
                        lblMessage.Text = $"Data uploaded successfully! Emails sent to {emailsSent} vendor(s)";
                        if (emailsFailed > 0)
                        {
                            lblMessage.Text += $" ({emailsFailed} failed)";
                        }
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblMessage.Text = "No emails were sent successfully!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                if (lblMessage != null)
                {
                    lblMessage.Text = $"Error in email sending process: {ex.Message}";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
                System.Diagnostics.Debug.WriteLine($"SendEmailsToSelectedVendors Error: {ex.Message}");
            }
        }
        protected void BindFilteredData(int index)
        {
            int PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvEnquiry.PageSize = PageSize;

            int StartIndex = index * PageSize;   // important for OFFSET

            string division = ddlDivision.SelectedIndex > 0 ? ddlDivision.SelectedValue : null;

            DateTime? fromDate = string.IsNullOrWhiteSpace(txtFromDate.Text)
                ? (DateTime?)null
                : Convert.ToDateTime(txtFromDate.Text);

            DateTime? toDate = string.IsNullOrWhiteSpace(txtToDate.Text)
                ? (DateTime?)null
                : Convert.ToDateTime(txtToDate.Text);

            string prNo = string.IsNullOrWhiteSpace(txtPPNo.Text)
                ? null
                : txtPPNo.Text.Trim();

            DataTable dt = _cm.GetDetailsForFloatEnquiries(
                StartIndex,
                PageSize,
                ddlPurchaseGroup.SelectedValue,
                ddlPurchaseSubGroup.SelectedValue,
                division,
                fromDate,
                toDate,
                prNo);
            int TotalRecords = 0;
            if (dt.Rows.Count > 0)
            {
                TotalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"].ToString());
                gvEnquiry.VirtualItemCount = TotalRecords;
                gvEnquiry.DataSource = dt;
                gvEnquiry.DataBind();
            }
            else
            {
                gvEnquiry.DataSource = null;
                gvEnquiry.DataBind();
            }
            spanCount.InnerHtml = TotalRecords.ToString();
        }
        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFilteredData(0);
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            BindFilteredData(0);
        }

        protected void txtPPNo_TextChanged(object sender, EventArgs e)
        {
            BindFilteredData(0);
        }

        protected void txtToDate_TextChanged(object sender, EventArgs e)
        {
            BindFilteredData(0);
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEnquiry.PageIndex = 0;
            BindFilteredData(0);
        }

        protected void gvEnquiry_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEnquiry.PageIndex = e.NewPageIndex;
            BindFilteredData(e.NewPageIndex);
        }
    }
}