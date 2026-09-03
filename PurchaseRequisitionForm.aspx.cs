using DocumentFormat.OpenXml.Bibliography;
using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class PurchaseRequisitionForm : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int employeeID = Convert.ToInt32(Session["EmployeeID"]);

                    BindApprovalHierarchy(employeeID);

                    InitializeDocumentTable();
                    BindUserDetails(Session["UserID"].ToString());

                    // Bind dropdown data FIRST
                    BindPRTypes();
                    BindStoreType();
                    BindProject();

                    // THEN restore form data (after dropdowns are populated)
                    if (Session["SavedPRData"] != null)
                    {
                        RestoreFormData();
                    }
                    else
                    {
                        InitializeGridView();
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        //Bind
        protected void BindUserDetails(String UserID)
        {
            DataTable dt = _cm.GetEmpDetails(UserID);
            if (dt.Rows.Count > 0)
            {
                // Session["Unit"] = dt.Rows[0]["JobLocation"].ToString();
                Session["EmployeeID"] = dt.Rows[0]["EmployeeID"].ToString();
                Session["DepartmentID"] = dt.Rows[0]["DepartmentID"].ToString();
            }
        }
        private void InitializeGridView()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("RowNumber", typeof(int)),
                new DataColumn("ItemCode", typeof(string)),
                new DataColumn("ItemDescription", typeof(string)),
                new DataColumn("ProductCategory", typeof(string)),
                new DataColumn("UOM", typeof(string)),
                new DataColumn("Qty", typeof(string)),
                new DataColumn("RequiredOn", typeof(string)),
                new DataColumn("StockBalance", typeof(string)),
                new DataColumn("Purpose", typeof(string)),
                new DataColumn("Remarks", typeof(string)),
               // new DataColumn("Status", typeof(string))
            });

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            Session["CurrentTable"] = dt;
            gvPR.DataSource = dt;
            gvPR.DataBind();
        }
        private void BindStoreType()
        {
            DataTable dt = _cm.GetItemType();
            if (dt.Rows.Count > 0)
            {
                rblStoreType.DataSource = dt;
                rblStoreType.DataTextField = "ItemType";
                rblStoreType.DataValueField = "ItemTypeID";
                rblStoreType.DataBind();
            }
        }
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetItem(string prefixText, int count, string contextKey)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    //com.CommandText = "Select distinct TestParameter from TestParameter where TestParameter like " + " @Search + '%' and isRequirement=0";
                    com.CommandText = "Select distinct ItemDescription from Items where isActive=1 and ItemDescription like " + " @Search + '%' and ItemTypeID=" + " @contextKey";
                    com.Parameters.AddWithValue("@Search", prefixText);
                    com.Parameters.AddWithValue("@contextKey", contextKey);
                    com.Connection = con;
                    con.Open();
                    List<string> TestParameter = new List<string>();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            TestParameter.Add(sdr["ItemDescription"].ToString());
                        }
                    }
                    con.Close();
                    return TestParameter;
                }

            }

        }
        private void BindProject()
        {
            DataTable dt = _cm.GetProjects();
            if (dt.Rows.Count > 0)
            {
                ddlProject.DataSource = dt;
                ddlProject.DataTextField = "ProjectName";
                ddlProject.DataValueField = "ProjectID";
                ddlProject.DataBind();
                ddlProject.Items.Insert(0, new ListItem("Select Project"));
            }
        }
        private void BindPRTypes()
        {
            DataTable dt = _cm.GetPRTypes();
            if (dt.Rows.Count > 0)
            {
                ddlPRType.DataSource = dt;
                ddlPRType.DataTextField = "PRType";
                ddlPRType.DataValueField = "PRTypesID";
                ddlPRType.DataBind();
                ddlPRType.Items.Insert(0, new ListItem("Select PR Type"));
            }
        }

        //More functions
        protected void ddlPRType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPRType.SelectedIndex > 0)
            {
                ddlPRType.BorderColor = Color.Empty;
                Session["PRType"] = ddlPRType.SelectedValue.ToString();
                //divProject.Visible = ddlPRType.SelectedValue.ToString() == "2";
            }
            SaveFormDataToSession();
        }
        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Add your project selection logic here
        }

        private void AddNewRowWithExistingData()
        {
            try
            {
                DataTable dt = Session["CurrentTable"] as DataTable;
                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[]
                    {
                new DataColumn("RowNumber", typeof(int)),
                new DataColumn("ItemCode", typeof(string)),
                new DataColumn("ItemDescription", typeof(string)),
                new DataColumn("ProductCategory", typeof(string)),
                new DataColumn("UOM", typeof(string)),
                new DataColumn("Qty", typeof(string)),
                new DataColumn("RequiredOn", typeof(string)),
                new DataColumn("StockBalance", typeof(string)),
                new DataColumn("Purpose", typeof(string)),
                new DataColumn("Remarks", typeof(string)),
                //new DataColumn("Status", typeof(string)) 
                    });
                }

                int currentRowIndex = dt.Rows.Count - 1;

                // Get the current row's data
                if (gvPR.Rows.Count > 0)
                {
                    GridViewRow currentRow = gvPR.Rows[currentRowIndex];
                    string itemCode = (currentRow.FindControl("txtItemCode") as TextBox)?.Text;

                    if (!string.IsNullOrWhiteSpace(itemCode))
                    {
                        // Update the current row instead of adding a new one
                        dt.Rows[currentRowIndex]["ItemCode"] = itemCode;
                        dt.Rows[currentRowIndex]["ItemDescription"] = (currentRow.FindControl("txtDescription") as TextBox)?.Text;
                        dt.Rows[currentRowIndex]["ProductCategory"] = (currentRow.FindControl("txtCategory") as TextBox)?.Text;
                        dt.Rows[currentRowIndex]["UOM"] = (currentRow.FindControl("txtUOM") as TextBox)?.Text;
                        dt.Rows[currentRowIndex]["Qty"] = (currentRow.FindControl("txtQty") as TextBox)?.Text;
                        dt.Rows[currentRowIndex]["RequiredOn"] = (currentRow.FindControl("txtRequiredOn") as HtmlInputControl)?.Value;
                        dt.Rows[currentRowIndex]["StockBalance"] = (currentRow.FindControl("txtStock") as TextBox)?.Text;
                        dt.Rows[currentRowIndex]["Purpose"] = (currentRow.FindControl("txtPurpose") as TextBox)?.Text;
                        dt.Rows[currentRowIndex]["Remarks"] = (currentRow.FindControl("txtRemarks") as TextBox)?.Text;

                        // Get selected value from ddlStatus
                        //var ddlStatus = (currentRow.FindControl("ddlStatus") as DropDownList);
                        //if (ddlStatus != null)
                        //{
                        //    dt.Rows[currentRowIndex]["Status"] = ddlStatus.SelectedValue; // Store the selected value
                        //}

                        // Add new empty row for next entry
                        dt.Rows.Add(dt.NewRow());
                    }
                }

                Session["CurrentTable"] = dt;
                gvPR.DataSource = dt;
                gvPR.DataBind();

                // Disable filled rows and enable the last row
                for (int i = 0; i < gvPR.Rows.Count; i++)
                {
                    GridViewRow row = gvPR.Rows[i];
                    bool isLastRow = (i == gvPR.Rows.Count - 1);
                    bool hasData = !string.IsNullOrWhiteSpace((row.FindControl("txtItemCode") as TextBox)?.Text);

                    DisableEnableRowControls(row, !hasData && isLastRow);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error",
                    $"alert('Error adding new row: {ex.Message}');", true);
            }
        }

        private void DisableEnableRowControls(GridViewRow row, bool enable)
        {
            var controlIds = new[]
            {
                "txtItemCode", "txtDescription", "txtCategory",
                "txtUOM", "txtQty", "txtRequiredOn",
                "txtStock", "txtPurpose","txtRemarks","ddlStatus"
            };

            foreach (string controlId in controlIds)
            {
                var control = row.FindControl(controlId);
                if (control is TextBox textBox)
                {
                    textBox.Enabled = enable;
                }
                else if (control is HtmlInputControl inputControl)
                {
                    inputControl.Disabled = !enable;
                }
            }
        }
        private void UpdateDataTableWithRowData(GridViewRow row)
        {
            if (Session["CurrentTable"] is DataTable dt)
            {
                int rowIndex = row.RowIndex;
                if (rowIndex < dt.Rows.Count)
                {
                    dt.Rows[rowIndex]["ItemCode"] = (row.FindControl("txtItemCode") as TextBox)?.Text;
                    dt.Rows[rowIndex]["ItemDescription"] = (row.FindControl("txtDescription") as TextBox)?.Text;
                    dt.Rows[rowIndex]["ProductCategory"] = (row.FindControl("txtCategory") as TextBox)?.Text;
                    dt.Rows[rowIndex]["UOM"] = (row.FindControl("txtUOM") as TextBox)?.Text;
                    dt.Rows[rowIndex]["StockBalance"] = (row.FindControl("txtStock") as TextBox)?.Text;

                    Session["CurrentTable"] = dt;
                }
            }
        }
        protected void gvPR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Set minimum date for RequiredOn field
                var txtRequiredOn = e.Row.FindControl("txtRequiredOn") as HtmlInputControl;
                if (txtRequiredOn != null)
                {
                    txtRequiredOn.Attributes["min"] = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                }

                DropDownList ddlStatus = e.Row.FindControl("ddlStatus") as DropDownList;
                if (ddlStatus != null)
                {
                    // Get the status options from the database
                    DataTable dtStatus = _cm.GetPurchaseRequisitionStatus(); // Assuming this method fetches status options

                    // Bind the DropDownList with the status options
                    ddlStatus.DataSource = dtStatus;
                    ddlStatus.DataTextField = "Status";  // Field for display text
                    ddlStatus.DataValueField = "StatusID"; // Field for value
                    ddlStatus.DataBind();

                    // Insert a default "Select Status" item at the top
                    ddlStatus.Items.Insert(0, new ListItem("Select Status", ""));

                    // Set the selected value for ddlStatus
                    string selectedStatus = (e.Row.DataItem as DataRowView)?["Status"]?.ToString();
                    if (!string.IsNullOrEmpty(selectedStatus))
                    {
                        ListItem item = ddlStatus.Items.FindByValue(selectedStatus);
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }

        //Get Items for grid
        public class ItemDetails
        {
            public string ItemCode { get; set; }
            public string Description { get; set; }
            public string UOMName { get; set; }
            public string CategoryName { get; set; }
            public string Stock { get; set; }
            public string ItemType { get; set; }
        }
        protected void txtCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (rblStoreType.SelectedIndex != -1)
                {
                    TextBox txtCode = (TextBox)sender;
                    GridViewRow row = (GridViewRow)txtCode.NamingContainer;

                    TextBox txtDescription = (TextBox)row.FindControl("txtDescription");
                    TextBox txtUOM = (TextBox)row.FindControl("txtUOM");
                    TextBox txtCategory = (TextBox)row.FindControl("txtCategory");
                    TextBox txtStock = (TextBox)row.FindControl("txtStock");

                    ItemDetails details = _cm.GetItemDetails(txtCode.Text.Trim(), rblStoreType.SelectedValue);

                    txtDescription.Text = details.Description;
                    txtStock.Text = details.Stock;
                    txtUOM.Text = details.UOMName;
                    txtCategory.Text = details.CategoryName;

                    UpdateDataTableWithRowData(row);
                    SaveFormDataToSession();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Item Type not selected')", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error",
                    $"alert('Error retrieving item details: {ex.Message}');", true);
            }
        }
        public ItemDetails GetItemDetails(string itemCode, string ItemTypeID)
        {
            ItemDetails details = new ItemDetails();
            string query = @"SELECT i.ItemDescription, i.StockQuantity, i.ItemTypeID,
                           u.UOMName, 
                           ps.PurchaseSubGroupName
                    FROM Items i 
                    LEFT JOIN UOM u ON i.UOMID = u.UOMID 
                    LEFT JOIN PurchaseSubGroup ps ON i.PurchaseSubGroupID = ps.PurchaseSubGroupID
                    WHERE i.ItemCode = @ItemCode and i.ItemTypeID=@ItemTypeID and i.isActive=1";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                    cmd.Parameters.AddWithValue("@ItemTypeID", ItemTypeID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                details.ItemCode = reader["ItemCode"]?.ToString() ?? string.Empty;
                                details.Description = reader["ItemDescription"]?.ToString() ?? string.Empty;
                                details.ItemType = reader["ItemTypeID"]?.ToString() ?? string.Empty;
                                details.UOMName = reader["UOMName"]?.ToString() ?? string.Empty;
                                details.CategoryName = reader["PurchaseSubGroupName"]?.ToString() ?? string.Empty;
                                details.Stock = reader["StockQuantity"]?.ToString() ?? string.Empty;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return details;
        }
        public ItemDetails GetItemDetailsByItemDescription(string ItemDescription, string contextKey)
        {
            ItemDetails details = new ItemDetails();
            string query = @"SELECT i.ItemCode, i.ItemDescription, i.StockQuantity, i.ItemTypeID,
                           u.UOMName, 
                           ps.PurchaseSubGroupName
                    FROM Items i 
                    LEFT JOIN UOM u ON i.UOMID = u.UOMID 
                    LEFT JOIN PurchaseSubGroup ps ON i.PurchaseSubGroupID = ps.PurchaseSubGroupID
                    WHERE TRIM(REPLACE(i.ItemDescription, CHAR(160), ''))  = TRIM(REPLACE(@ItemDescription, CHAR(160), ''))  and i.ItemTypeID=@ItemTypeID and i.isActive=1";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemDescription", ItemDescription);
                    cmd.Parameters.AddWithValue("@ItemTypeID", contextKey);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                details.ItemCode = reader["ItemCode"]?.ToString() ?? string.Empty;
                                details.Description = reader["ItemDescription"]?.ToString() ?? string.Empty;
                                details.ItemType = reader["ItemTypeID"]?.ToString() ?? string.Empty;
                                details.UOMName = reader["UOMName"]?.ToString() ?? string.Empty;
                                details.CategoryName = reader["PurchaseSubGroupName"]?.ToString() ?? string.Empty;
                                details.Stock = reader["StockQuantity"]?.ToString() ?? string.Empty;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return details;
        }
        //Buttons
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            // Validate the "Qty" field
            GridViewRow currentRow = gvPR.Rows[gvPR.Rows.Count - 1]; // Get the last row (current row to add)
            var txtQty = currentRow.FindControl("txtQty") as TextBox;

            if (txtQty != null)
            {
                // Check if the Qty is empty or not a valid number
                if (string.IsNullOrWhiteSpace(txtQty.Text) || !decimal.TryParse(txtQty.Text, out decimal qty) || qty <= 0)
                {
                    // Show a simple alert if Qty is invalid
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Please enter a valid quantity greater than zero.');", true);
                    return;
                }
            }

            // Proceed with adding a new row if validation is passed
            AddNewRowWithExistingData();
            SaveFormDataToSession();
        }
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            // Get the row index from CommandArgument
            Button btnRemove = sender as Button;
            if (btnRemove != null)
            {
                int rowIndex = Convert.ToInt32(btnRemove.CommandArgument);

                // Retrieve the DataTable from the session
                DataTable dt = Session["CurrentTable"] as DataTable;

                // Ensure that there's at least one row in the DataTable
                if (dt != null && dt.Rows.Count > 1 && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                {
                    // Remove the row from the DataTable
                    dt.Rows.RemoveAt(rowIndex);
                    Session["CurrentTable"] = dt; // Update the session with the modified DataTable

                    // Rebind the GridView to reflect the changes
                    gvPR.DataSource = dt;
                    gvPR.DataBind();
                }
                else if (dt != null && dt.Rows.Count == 1)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('There should be atleast one Row in the table.');", true);
                    GridViewRow lastRow = gvPR.Rows[0]; // Since there's only one row, we access the first one (index 0)
                    var txtItemCode = lastRow.FindControl("txtItemCode") as TextBox;
                    var txtDescription = lastRow.FindControl("txtDescription") as TextBox;
                    var txtCategory = lastRow.FindControl("txtCategory") as TextBox;
                    var txtUOM = lastRow.FindControl("txtUOM") as TextBox;
                    var txtQty = lastRow.FindControl("txtQty") as TextBox;
                    var txtRequiredOn = lastRow.FindControl("txtRequiredOn") as HtmlInputControl;
                    var txtStock = lastRow.FindControl("txtStock") as TextBox;
                    var txtPurpose = lastRow.FindControl("txtPurpose") as TextBox;
                    var txtRemarks = lastRow.FindControl("txtRemarks") as TextBox;
                    var ddlStatus = lastRow.FindControl("ddlStatus") as DropDownList;

                    if (txtItemCode != null) txtItemCode.Text = string.Empty;
                    if (txtDescription != null) txtDescription.Text = string.Empty;
                    if (txtCategory != null) txtCategory.Text = string.Empty;
                    if (txtUOM != null) txtUOM.Text = string.Empty;
                    if (txtQty != null) txtQty.Text = string.Empty;
                    if (txtRequiredOn != null) txtRequiredOn.Value = string.Empty;
                    if (txtStock != null) txtStock.Text = string.Empty;
                    if (txtPurpose != null) txtPurpose.Text = string.Empty;
                    if (txtRemarks != null) txtRemarks.Text = string.Empty;
                    if (ddlStatus != null) ddlStatus.SelectedValue = string.Empty;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string LocalOrImport = ""; int PRNumber = 0; string PRYear = ""; 
            int highestApprovalOrgTreeID = 0; 
            if (Session["Saving"] != null)
                return;
            try
            {
                Session["Saving"] = true;

                LocalOrImport = radLocalOrImport.SelectedItem.ToString();
                // Get the text value of the selected radio button, not just the value
                string storeType = rblStoreType.SelectedItem != null ? rblStoreType.SelectedItem.Text : null;
                if (string.IsNullOrEmpty(storeType))
                {
                    throw new Exception("Please select an Item Type");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                int prid; // Declare PRID outside the using block so we can use it later

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // [Previous code remains the same until PRID is obtained]
                    string prTypeid = "1";// ddlPRType.SelectedItem.Value;
                    string prType = "General";// ddlPRType.SelectedItem.Text;
                    if (string.IsNullOrEmpty(prType))
                    {
                        throw new Exception("Please select a PR Type");
                    }

                    // Project Dropdown
                    string project = null;
                    //if (divProject.Visible)
                    //{
                        if (ddlProject.SelectedItem != null)
                        {
                            project = ddlProject.SelectedItem.Text;
                        }
                        else
                        {
                            throw new Exception("Please select a Project");
                        }
                    //  }
                    if (rblHighestApproval.SelectedIndex == -1)
                    {
                        lblMessage.Text = "Please select the highest approval level.";
                        return;
                    }
                    else
                    {
                        highestApprovalOrgTreeID = Convert.ToInt32(rblHighestApproval.SelectedValue);
                    }
                    // Get the EmployeeID and DepartmentID from Session
                    string requestBy = Session["EmployeeID"]?.ToString();
                    string departmentID = Session["DepartmentID"]?.ToString();

                    if (string.IsNullOrEmpty(requestBy) || string.IsNullOrEmpty(departmentID))
                    {
                        throw new Exception("Employee details are missing. Please login again.");
                    }

                    // Insert data into Purchaserequisition table

                    // Step 1: Retrieve the last ReportFileName value
                    string getLastReportFileNameQuery = "SELECT TOP 1 ReportFileName FROM PurchaseRequisition ORDER BY PRID DESC";
                    string lastReportFileName = null;

                    using (SqlCommand cmd = new SqlCommand(getLastReportFileNameQuery, conn))
                    {
                        lastReportFileName = (string)cmd.ExecuteScalar();
                    }

                    // Step 2: Generate the new ReportFileName
                    string newReportFileName = "PR-1"; // default if no records exist

                    if (!string.IsNullOrEmpty(lastReportFileName))
                    {
                        // Extract the numeric part after "PR-"
                        int lastNumber = int.Parse(lastReportFileName.Replace("PR-", ""));
                        int newNumber = lastNumber + 1;
                        newReportFileName = "PR-" + newNumber.ToString();
                    }
                    DataTable dtPRNo = _cm.GetLastPR(DateTime.Now.ToString("yyyy"));
                    if (dtPRNo.Rows.Count > 0)
                    {
                        PRNumber = Convert.ToInt32(dtPRNo.Rows[0]["PRNumber"].ToString()) + 1;
                    }
                    else
                    {
                        PRNumber = 1;
                    }
                    PRYear = DateTime.Now.ToString("yyyy");
                    // Step 3: Insert the new record
                    string insertPRQuery = @"
                    INSERT INTO PurchaseRequisition 
                    (RequestBy, DepartmentID, RequestDate, StatusID, ProjectName, PRTypeID, StoreType, ReportFileName, PRNumber, PRYear,LocalOrImport)
                    VALUES 
                    (@RequestBy, @DepartmentID, @RequestDate, @StatusID, @ProjectName, @PRTypeID, @StoreType, @ReportFileName, @PRNumber, @PRYear, @LocalOrImport);
                    SELECT SCOPE_IDENTITY();";

                    using (SqlCommand prCmd = new SqlCommand(insertPRQuery, conn))
                    {
                        prCmd.Parameters.AddWithValue("@RequestBy", requestBy);
                        prCmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        prCmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);
                        prCmd.Parameters.AddWithValue("@StatusID", 1);
                        prCmd.Parameters.AddWithValue("@ProjectName", string.IsNullOrEmpty(project) ? (object)DBNull.Value : project);
                        prCmd.Parameters.AddWithValue("@PRTypeID", prTypeid);
                        prCmd.Parameters.AddWithValue("@StoreType", storeType);
                        prCmd.Parameters.AddWithValue("@ReportFileName", newReportFileName);
                        prCmd.Parameters.AddWithValue("@PRNumber", PRNumber);
                        prCmd.Parameters.AddWithValue("@PRYear", PRYear);
                        prCmd.Parameters.AddWithValue("@LocalOrImport", LocalOrImport);


                        prid = Convert.ToInt32(prCmd.ExecuteScalar());
                        _cm.PRID = prid;
                        DataTable dtDocs = Session["PRDocuments"] as DataTable;

                        if (dtDocs != null && dtDocs.Rows.Count > 0)
                        {
                            string insertDocQuery = @"
                                        INSERT INTO PurchaseRequisitionDocuments
                                        (
                                            PRID,
                                            DocumentType,
                                            Description,
                                            FileName,
                                            FilePath,
                                            FileSize,
                                            UploadedBy,
                                            UploadedDate
                                        )
                                        VALUES
                                        (
                                            @PRID,
                                            @DocumentType,
                                            @Description,
                                            @FileName,
                                            @FilePath,
                                            @FileSize,
                                            @UploadedBy,
                                            GETDATE()
                                        )";

                            string prFolder = Server.MapPath("~/PurchaseRequisition/" + PRYear + "/PR" + PRNumber.ToString("000000"));

                            if (!Directory.Exists(prFolder))
                                Directory.CreateDirectory(prFolder);

                            foreach (DataRow dr in dtDocs.Rows)
                            {
                                string tempPath = dr["FilePath"].ToString();
                                string fileName = Path.GetFileName(tempPath);

                                string newPath = Path.Combine(prFolder, fileName);

                                if (File.Exists(tempPath))
                                {
                                    File.Move(tempPath, newPath);
                                }

                                // Save newPath into database

                                using (SqlCommand cmdDoc = new SqlCommand(insertDocQuery, conn))
                                {
                                    cmdDoc.Parameters.AddWithValue("@PRID", prid);
                                    cmdDoc.Parameters.AddWithValue("@DocumentType", dr["DocumentType"].ToString());
                                    cmdDoc.Parameters.AddWithValue("@Description", dr["Description"].ToString());
                                    cmdDoc.Parameters.AddWithValue("@FileName", dr["FileName"].ToString());
                                    cmdDoc.Parameters.AddWithValue("@FilePath", newPath);
                                    cmdDoc.Parameters.AddWithValue("@FileSize", Convert.ToInt64(dr["Size"]));
                                    cmdDoc.Parameters.AddWithValue("@UploadedBy", Session["EmployeeID"].ToString());

                                    cmdDoc.ExecuteNonQuery();
                                }
                            }
                        }
                        string EmployeeID = "";
                        DataTable dtReporting = new DataTable();
                        if (Session["EmployeeID"] != null)
                        {
                            EmployeeID = Session["EmployeeID"].ToString();
                            dtReporting = _cm.GetReportingTo(EmployeeID);
                            if (dtReporting.Rows.Count > 0)
                            {
                                if (dtReporting.Rows[0]["PID"].ToString() == "1")
                                {
                                    _cm.CurrentTier = 4;
                                    _cm.CurrentApproverRole = "Director";
                                    _cm.PurchaserequisitionSetApprover(_cm);
                                }
                                else
                                {
                                    _cm.CurrentTier = 1;
                                    _cm.CurrentApproverRole = "Manager";
                                    _cm.PurchaserequisitionSetApprover(_cm);
                                }
                            }
                        }


                    }

                    foreach (GridViewRow row in gvPR.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            // Get values from GridView controls
                            string itemCode = (row.FindControl("txtItemCode") as TextBox).Text;
                            if (itemCode != null)
                            {
                                string uom = (row.FindControl("txtUOM") as TextBox).Text;
                                decimal quantity = Convert.ToDecimal((row.FindControl("txtQty") as TextBox).Text);
                                decimal stock = 0;// Convert.ToDecimal((row.FindControl("txtStock") as TextBox).Text);
                                string purpose = (row.FindControl("txtPurpose") as TextBox).Text;

                                DropDownList ddlStatus = (row.FindControl("ddlStatus") as DropDownList);
                                int statusId = 1;
                                string remarks = (row.FindControl("txtRemarks") as TextBox).Text;
                                HtmlInputGenericControl requiredOn = (row.FindControl("txtRequiredOn") as HtmlInputGenericControl);

                                if (string.IsNullOrEmpty(requiredOn.Value))
                                {
                                    throw new Exception("Required On date cannot be empty");
                                }

                                DateTime requiredOnDate;
                                if (!DateTime.TryParse(requiredOn.Value, out requiredOnDate))
                                {
                                    throw new Exception($"Invalid date format. The value '{requiredOn.Value}' could not be converted to a date.");
                                }

                                // Get UOMID from UOM code
                                string getUOMIDQuery = "SELECT UOMId FROM UOM WHERE UOMName = @UOM";
                                int uomId;
                                using (SqlCommand uomCmd = new SqlCommand(getUOMIDQuery, conn))
                                {
                                    uomCmd.Parameters.AddWithValue("@UOM", uom);
                                    object result = uomCmd.ExecuteScalar();
                                    if (result == null)
                                    {
                                        throw new Exception($"UOM '{uom}' not found in database");
                                    }
                                    uomId = Convert.ToInt32(result);
                                }

                                // Insert into PurchaseRequisitionDetails table
                                string insertQuery = @"
                            INSERT INTO PurchaseRequisitionDetails 
                            (PRID, ItemCode, UOMID, Quantity, Stock, Purpose, StatusID, Remarks, RequiredOn, PRType, ProjectName, StoreType)
                            VALUES 
                            (@PRID, @ItemCode, @UOMID, @Quantity, @Stock, @Purpose, @StatusID, @Remarks, @RequiredOn, @PRType, @ProjectName, @StoreType);
                            SELECT SCOPE_IDENTITY();";

                                int prdetailsid;
                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@PRID", prid);
                                    cmd.Parameters.AddWithValue("@ItemCode", itemCode.Trim());
                                    cmd.Parameters.AddWithValue("@UOMID", uomId);
                                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                                    cmd.Parameters.AddWithValue("@Stock", stock);
                                    cmd.Parameters.AddWithValue("@Purpose", purpose);
                                    cmd.Parameters.AddWithValue("@StatusID", statusId);
                                    cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? DBNull.Value : (object)remarks);
                                    cmd.Parameters.AddWithValue("@RequiredOn", requiredOnDate);
                                    cmd.Parameters.AddWithValue("@PRType", prType);
                                    cmd.Parameters.AddWithValue("@ProjectName", string.IsNullOrEmpty(project) ? (object)DBNull.Value : project);
                                    cmd.Parameters.AddWithValue("@StoreType", storeType);

                                    prdetailsid = Convert.ToInt32(cmd.ExecuteScalar());

                                    var User = new DAL.User();
                                    User.EmployeeID = Session["EmployeeID"]?.ToString();
                                    User.DepartmentID = Session["DepartmentID"]?.ToString();
                                    User.UserID = Session["UserID"]?.ToString();
                                    User.RoleID = Session["RoleID"]?.ToString();
                                    User.Remarks = "New PR";
                                    User.PRStatus = statusId.ToString();
                                    User.PurchaseRequisitionDetailsID = prdetailsid.ToString();
                                    _cm.AddPRHistory(User);
                                }
                            }

                        }
                    }
                }
                try
                {
                    _cm.highestApprovalOrgTreeID = highestApprovalOrgTreeID;
                    _cm.UpdateHighestApproval(_cm);
                }
                catch(Exception ex)
                {

                }
                

                ClearFields();

                // After successful save, redirect to the report page with the PRID
                string reportUrl = $"GeneratePurchaseRequisitionReport.aspx?ID={prid}";

                // Use JavaScript to show success message and then redirect
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccessAndRedirect",
                    $"alert('Purchase Requisition details saved successfully!'); window.location.href = '{reportUrl}';", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveError",
                    $"alert('Error : {ex.Message}');", true);
            }
            finally
            {
                Session["Saving"] = null;
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        //restore
        private void SaveFormDataToSession()
        {
            try
            {
                var formData = new Dictionary<string, object>();

                // Save dropdown selections
                formData["StoreType"] = rblStoreType.SelectedValue;
                formData["PRType"] = "General";// ddlPRType.SelectedValue;
                formData["Project"] = "1";// ddlProject.SelectedValue;

                // Save grid data
                DataTable currentTable = Session["CurrentTable"] as DataTable;
                if (currentTable != null)
                {
                    // Update current table with latest grid values
                    UpdateCurrentTableFromGrid();
                    formData["GridData"] = currentTable;
                }

                // Save timestamp for expiration
                formData["SavedTime"] = DateTime.Now;

                Session["SavedPRData"] = formData;
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine("Error saving form data: " + ex.Message);
            }
        }
        private void RestoreFormData()
        {
            try
            {
                var formData = Session["SavedPRData"] as Dictionary<string, object>;
                if (formData != null)
                {
                    // Check if data is not expired (e.g., 30 minutes)
                    DateTime savedTime = (DateTime)formData["SavedTime"];
                    if (DateTime.Now.Subtract(savedTime).TotalMinutes > 30)
                    {
                        // Data expired, clear it
                        Session.Remove("SavedPRData");
                        InitializeGridView();
                        return;
                    }

                    // Restore dropdown selections
                    if (formData.ContainsKey("StoreType") && !string.IsNullOrEmpty(formData["StoreType"].ToString()))
                    {
                        rblStoreType.SelectedValue = formData["StoreType"].ToString();
                    }

                    if (formData.ContainsKey("PRType") && !string.IsNullOrEmpty(formData["PRType"].ToString()))
                    {
                        ddlPRType.SelectedValue = formData["PRType"].ToString();
                    }

                    if (formData.ContainsKey("Project") && !string.IsNullOrEmpty(formData["Project"].ToString()))
                    {
                        ddlProject.SelectedValue = formData["Project"].ToString();
                    }

                    // Restore grid data
                    if (formData.ContainsKey("GridData"))
                    {
                        DataTable savedTable = formData["GridData"] as DataTable;
                        if (savedTable != null && savedTable.Rows.Count > 0)
                        {
                            Session["CurrentTable"] = savedTable;
                            gvPR.DataSource = savedTable;
                            gvPR.DataBind();
                        }
                        else
                        {
                            InitializeGridView();
                        }
                    }
                    else
                    {
                        InitializeGridView();
                    }
                }
                else
                {
                    InitializeGridView();
                }
            }
            catch (Exception ex)
            {
                // If restore fails, initialize normally
                System.Diagnostics.Debug.WriteLine("Error restoring form data: " + ex.Message);
                InitializeGridView();
            }
        }
        private void UpdateCurrentTableFromGrid()
        {
            DataTable dt = Session["CurrentTable"] as DataTable;
            if (dt == null) return;

            foreach (GridViewRow row in gvPR.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int rowIndex = row.RowIndex;

                    if (rowIndex < dt.Rows.Count)
                    {
                        // Update each column value from the grid controls
                        TextBox txtItemCode = row.FindControl("txtItemCode") as TextBox;
                        TextBox txtDescription = row.FindControl("txtDescription") as TextBox;
                        TextBox txtCategory = row.FindControl("txtCategory") as TextBox;
                        TextBox txtUOM = row.FindControl("txtUOM") as TextBox;
                        TextBox txtQty = row.FindControl("txtQty") as TextBox;
                        HtmlInputGenericControl txtRequiredOn = row.FindControl("txtRequiredOn") as HtmlInputGenericControl;
                        TextBox txtStock = row.FindControl("txtStock") as TextBox;
                        TextBox txtPurpose = row.FindControl("txtPurpose") as TextBox;
                        TextBox txtRemarks = row.FindControl("txtRemarks") as TextBox;

                        if (txtItemCode != null) dt.Rows[rowIndex]["ItemCode"] = txtItemCode.Text;
                        if (txtDescription != null) dt.Rows[rowIndex]["ItemDescription"] = txtDescription.Text;
                        if (txtCategory != null) dt.Rows[rowIndex]["ProductCategory"] = txtCategory.Text;
                        if (txtUOM != null) dt.Rows[rowIndex]["UOM"] = txtUOM.Text;
                        if (txtQty != null) dt.Rows[rowIndex]["Qty"] = txtQty.Text;
                        if (txtRequiredOn != null) dt.Rows[rowIndex]["RequiredOn"] = txtRequiredOn.Value;
                        if (txtStock != null) dt.Rows[rowIndex]["StockBalance"] = txtStock.Text;
                        if (txtPurpose != null) dt.Rows[rowIndex]["Purpose"] = txtPurpose.Text;
                        if (txtRemarks != null) dt.Rows[rowIndex]["Remarks"] = txtRemarks.Text;
                    }
                }
            }
        }

        //clear form
        private void ClearFields()
        {
            rblStoreType.ClearSelection();
            ddlPRType.SelectedIndex = -1;
            ddlProject.SelectedIndex = -1;
            //divProject.Visible = false;

            lblMessage.Text = string.Empty;

            // Clear session data so RestoreFormData doesn't repopulate
            Session.Remove("CurrentTable");
            Session.Remove("SavedPRData");

            // Optionally: re-initialize empty DataTable for GridView
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCode");
            dt.Columns.Add("ItemDescription");
            dt.Columns.Add("ProductCategory");
            dt.Columns.Add("UOM");
            dt.Columns.Add("Qty");
            dt.Columns.Add("RequiredOn");
            dt.Columns.Add("StockBalance");
            dt.Columns.Add("Purpose");
            dt.Columns.Add("Remarks");
            dt.Rows.Add(dt.NewRow());

            Session["CurrentTable"] = dt; // if needed for consistent session logic

            gvPR.DataSource = dt;
            gvPR.DataBind();
        }

        protected void txtDescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (rblStoreType.SelectedIndex != -1)
                {
                    TextBox txtDescription = (TextBox)sender;
                    GridViewRow row = (GridViewRow)txtDescription.NamingContainer;

                    TextBox txtItemCode = (TextBox)row.FindControl("txtItemCode");
                    TextBox txtUOM = (TextBox)row.FindControl("txtUOM");
                    TextBox txtCategory = (TextBox)row.FindControl("txtCategory");
                    TextBox txtStock = (TextBox)row.FindControl("txtStock");

                    string ItemType = rblStoreType.SelectedValue.ToString();

                    ItemDetails details = GetItemDetailsByItemDescription(txtDescription.Text.Trim(), rblStoreType.SelectedValue);

                    if (details.ItemType != rblStoreType.SelectedValue.ToString())
                    {
                        txtDescription.Text = "";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Item does not belong to Item Type selected')", true);
                    }
                    else
                    {
                        txtItemCode.Text = details.ItemCode;
                        txtStock.Text = details.Stock;
                        txtUOM.Text = details.UOMName;
                        txtCategory.Text = details.CategoryName;
                        UpdateDataTableWithRowData(row);
                        SaveFormDataToSession();
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Item Type not selected')", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error",
                    $"alert('Error retrieving item details: {ex.Message}');", true);
            }

        }
        private void InitializeDocumentTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("DocumentType");
            dt.Columns.Add("Description");
            dt.Columns.Add("FileName");
            dt.Columns.Add("FilePath");
            dt.Columns.Add("Size");

            Session["PRDocuments"] = dt;

            gvDocuments.DataSource = dt;
            gvDocuments.DataBind();
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuDocument.HasFile)
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "a", "alert('Select a file');", true);
                return;
            }

            string folder = Server.MapPath("~/Documents/Temp/");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() +
                              Path.GetExtension(fuDocument.FileName);

            string path = Path.Combine(folder, fileName);

            fuDocument.SaveAs(path);

            DataTable dt = Session["PRDocuments"] as DataTable;

            DataRow dr = dt.NewRow();

            dr["DocumentType"] = ddlDocumentType.SelectedItem.Text;
            dr["Description"] = txtDocDescription.Text;
            dr["FileName"] = fuDocument.FileName;
            dr["FilePath"] = path;
            dr["Size"] = (fuDocument.PostedFile.ContentLength / 1024);

            dt.Rows.Add(dr);

            Session["PRDocuments"] = dt;

            gvDocuments.DataSource = dt;
            gvDocuments.DataBind();

            ddlDocumentType.SelectedIndex = 0;
            txtDocDescription.Text = "";
        }
        protected void btnDeleteDoc_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index = Convert.ToInt32(btn.CommandArgument);

            DataTable dt = Session["PRDocuments"] as DataTable;

            if (dt != null && index >= 0 && index < dt.Rows.Count)
            {
                // Delete physical file if it exists
                string filePath = dt.Rows[index]["FilePath"].ToString();

                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Remove row
                dt.Rows.RemoveAt(index);

                Session["PRDocuments"] = dt;

                gvDocuments.DataSource = dt;
                gvDocuments.DataBind();
            }
        }

        private void BindApprovalHierarchy(int employeeID)
        {
            DataTable dt = _cm.BindApprovalHierarchy(employeeID);

            if (dt.Rows.Count > 0)
            {
                // ==========================================
                // Bind hierarchy display
                // ==========================================

                rptApprovalHierarchy.DataSource = dt;
                rptApprovalHierarchy.DataBind();


                // ==========================================
                // Bind Highest Approval RadioButtonList
                // Exclude Level 0 - employee's own organization
                // ==========================================

                rblHighestApproval.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    int approvalLevel = Convert.ToInt32(row["ApprovalLevel"]);

                    // Level 0 is the employee's own organization.
                    // It should NOT be an approval option.
                    if (approvalLevel == 0)
                        continue;

                    string orgTreeID = row["ID"].ToString();
                    string orgName = row["Name"].ToString();

                    string text = "Level " + approvalLevel +
                                  " - " + orgName;

                    rblHighestApproval.Items.Add(
                        new System.Web.UI.WebControls.ListItem(
                            text,
                            orgTreeID
                        )
                    );
                }

                // ==========================================
                // SELECT HIGHEST APPROVAL LEVEL BY DEFAULT
                // ==========================================

                if (rblHighestApproval.Items.Count > 0)
                {
                    rblHighestApproval.SelectedIndex =
                        rblHighestApproval.Items.Count - 1;
                }
            }
            else
            {
                rptApprovalHierarchy.DataSource = null;
                rptApprovalHierarchy.DataBind();

                rblHighestApproval.DataSource = null;
                rblHighestApproval.DataBind();
            }
        }
    }
}
