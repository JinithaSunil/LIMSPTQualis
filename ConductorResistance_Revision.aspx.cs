using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;

namespace LIMS
{
    public partial class ConductorResistance_Revision : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        private string _specifiedValue = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();
                txtspecified.ReadOnly = false;
                PopulateCoreDropdown();

                // Get sample information from query string
                if (Request.QueryString["s"] != null)
                {
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                }

                if (Request.QueryString["r"] != null)
                {
                    _cm.ReportID = Request.QueryString["r"];
                }
                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    // Set dynamic values from database
                    cableType.Text = dt1.Rows[0]["SampleType"].ToString();
                    lblvoltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();

                    //upper section
                    roomno.Text = "-";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/15/El/41";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                CheckAndPrefillForm();
            }

        }

        //Buttons 
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, tempFactor = 0, rt = 0, r20 = 0;

            if (ddlCorePhase.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                ddlCorePhase.CssClass += " input-error";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtLength.Text) || !decimal.TryParse(txtLength.Text, out length))
            {
                invalidControls.Add(txtLength);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtTempFactor.Text) || !decimal.TryParse(txtTempFactor.Text, out tempFactor))
            {
                invalidControls.Add(txtTempFactor);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtRt.Text) || !decimal.TryParse(txtRt.Text, out rt))
            {
                invalidControls.Add(txtRt);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtR20.Text) || !decimal.TryParse(txtR20.Text, out r20))
            {
                invalidControls.Add(txtR20);
                hasError = true;
            }

            // Validate Specified field - only required for first entry
            System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
            bool isFirstEntry = (dt == null || dt.Rows.Count == 0);

            if (isFirstEntry && string.IsNullOrWhiteSpace(txtspecified.Text))
            {
                invalidControls.Add(txtspecified);
                hasError = true;
            }

            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = Color.FromArgb(255, 150, 150); // Darker red
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                string message = isFirstEntry ? "Please fill in all required fields correctly including the Specified value."
                                             : "Please fill in all required fields correctly.";
                ShowMessage(message);
                return;
            }

            if (dt == null)
            {
                dt = new System.Data.DataTable();
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("TempFactor", typeof(decimal));
                dt.Columns.Add("Rt", typeof(decimal));
                dt.Columns.Add("R20", typeof(decimal));
                dt.Columns.Add("Specified", typeof(string));
                dt.Columns.Add("ID", typeof(Guid)); // Add an ID for unique identification
            }

            // Store specified value from first entry
            if (isFirstEntry)
            {
                _specifiedValue = txtspecified.Text.Trim();
                ViewState["SpecifiedValue"] = _specifiedValue; // Store in ViewState
            }
            else
            {
                // Get stored specified value from ViewState
                _specifiedValue = ViewState["SpecifiedValue"]?.ToString() ?? string.Empty;
            }

            // Create a new row with a unique ID
            DataRow dr = dt.NewRow();
            dr["Core"] = ddlCorePhase.SelectedItem.Text;
            dr["Length"] = length;
            dr["TempFactor"] = tempFactor;
            dr["Rt"] = rt;
            dr["R20"] = r20;
            dr["Specified"] = _specifiedValue; // Use the stored specified value
            dr["ID"] = Guid.NewGuid(); // Assign a unique ID

            dt.Rows.Add(dr);

            ViewState["ResistanceTestResults"] = dt;

            gvResistanceTest.DataSource = dt;
            gvResistanceTest.DataBind();

            ClearInputFields();
        }
        protected void btnSaveData_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvResistanceTest.Rows.Count > 0);

            // Validate Main Section 
            if (string.IsNullOrEmpty(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }
            if (string.IsNullOrEmpty(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }
            if (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                if (string.IsNullOrEmpty(txtLength.Text) || !double.TryParse(txtLength.Text, out _))
                {
                    errors.Add("Length must be a valid number.");
                    invalidControls.Add(txtLength);
                }
                if (string.IsNullOrEmpty(txtTempFactor.Text) || !double.TryParse(txtTempFactor.Text, out _))
                {
                    errors.Add("Temp Factor Correction must be a valid number.");
                    invalidControls.Add(txtTempFactor);
                }
                if (string.IsNullOrEmpty(txtTempAtValue.Text) || !double.TryParse(txtTempAtValue.Text, out _))
                {
                    errors.Add("Temperature Value must be a valid number.");
                    invalidControls.Add(txtTempAtValue);
                }
                if (string.IsNullOrEmpty(txtRt.Text) || !double.TryParse(txtRt.Text, out _))
                {
                    errors.Add("RT value must be a valid number.");
                    invalidControls.Add(txtRt);
                }
                if (string.IsNullOrEmpty(txtR20.Text) || !double.TryParse(txtR20.Text, out _))
                {
                    errors.Add("RT20 C value must be a valid number.");
                    invalidControls.Add(txtR20);
                }

            }


            // Check if Grid has data (only required if grid is empty)
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            // Reset all control borders first
            ResetControlBorders();

            // Display errors or proceed
            if (errors.Count > 0)
            {
                // Highlight invalid controls with light red border
                foreach (var control in invalidControls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        ((TextBox)control).BorderStyle = BorderStyle.Solid;
                        ((TextBox)control).BorderWidth = 1;
                    }
                }

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);
            }
            else
            {
                Submitform();
                // Show success message
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                    "alert('Test results and Report Generated successfully!');", true);
                // Clear form
                ClearForm();
            }
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvResistanceTest.Rows.Count > 0);

            // Validate Main Section 
            if (string.IsNullOrEmpty(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }
            if (string.IsNullOrEmpty(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }
            if (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                if (string.IsNullOrEmpty(txtLength.Text) || !double.TryParse(txtLength.Text, out _))
                {
                    errors.Add("Length must be a valid number.");
                    invalidControls.Add(txtLength);
                }
                if (string.IsNullOrEmpty(txtTempFactor.Text) || !double.TryParse(txtTempFactor.Text, out _))
                {
                    errors.Add("Temp Factor Correction must be a valid number.");
                    invalidControls.Add(txtTempFactor);
                }
                if (string.IsNullOrEmpty(txtTempAtValue.Text) || !double.TryParse(txtTempAtValue.Text, out _))
                {
                    errors.Add("Temperature Value must be a valid number.");
                    invalidControls.Add(txtTempAtValue);
                }
                if (string.IsNullOrEmpty(txtRt.Text) || !double.TryParse(txtRt.Text, out _))
                {
                    errors.Add("RT value must be a valid number.");
                    invalidControls.Add(txtRt);
                }
                if (string.IsNullOrEmpty(txtR20.Text) || !double.TryParse(txtR20.Text, out _))
                {
                    errors.Add("RT20 C value must be a valid number.");
                    invalidControls.Add(txtR20);
                }
            }

            // Check if Grid has data
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            ResetControlBorders();

            if (errors.Count > 0)
            {
                foreach (var control in invalidControls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        ((TextBox)control).BorderStyle = BorderStyle.Solid;
                        ((TextBox)control).BorderWidth = 1;
                    }
                }

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);
            }
            else
            {
                try
                {
                    if (Request.QueryString["s"] != null)
                    {
                        int sampleId = Convert.ToInt32(Request.QueryString["s"]);

                        int reportid = Convert.ToInt32(Request.QueryString["r"]);
                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;

                        DeleteExistingRecords(sampleId, reportid);

                        Submitform();

                        // Show success message
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateSuccess",
                            "alert('Test results updated successfully!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                            "alert('Sample ID not found. Cannot update.');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                        $"alert('Error updating test results: {ex.Message}');", true);
                }
            }
        }
        private void DeleteExistingRecords(int sampleId, int reportid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();


                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID and ReportId = @ReportId";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportId", reportid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and rethrow
                throw new Exception("Error deleting existing records: " + ex.Message);
            }
        }

        //Data Saving
        private void Submitform()
        {
            try
            {
                // Get the DataTable from ViewState
                System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    // Show error message if no data to save
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return;
                }

                // Create DAL object
                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) { _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower()); }
                if (Request.QueryString["r"] != null) { _cm.ReportID = Request.QueryString["r"]; }
                System.Data.DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                // Prepare arrays for the AddTestResultGlobal method
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Create comma-separated values for each column
                string coreValues = string.Join(",", GetColumnValuesFromGridView("Core"));
                string lengthValues = string.Join(",", GetColumnValuesFromGridView("Length"));
                string tempFactorValues = string.Join(",", GetColumnValuesFromGridView("TempFactor"));
                string rtValues = string.Join(",", GetColumnValuesFromGridView("Rt"));
                string r20cValues = string.Join(",", GetColumnValuesFromGridView("R20"));
                string specifiedValue = ViewState["SpecifiedValue"]?.ToString() ?? string.Empty;

                // Form
                reportParamMapList.Add("14093"); resultList.Add(coreValues);           
                reportParamMapList.Add("14094"); resultList.Add(lengthValues);        
                reportParamMapList.Add("14095"); resultList.Add(tempFactorValues);     
                reportParamMapList.Add("14110"); resultList.Add(txtTempAtValue.Text);  
                reportParamMapList.Add("14096"); resultList.Add(rtValues);            
                reportParamMapList.Add("14097"); resultList.Add(r20cValues);
                reportParamMapList.Add("14159"); resultList.Add(specifiedValue);

                //Upper section 
                reportParamMapList.Add("14098"); resultList.Add(cableType.Text);       
                reportParamMapList.Add("14104"); resultList.Add(refSpec.Text);       
                reportParamMapList.Add("14099"); resultList.Add(cableSize.Text);       
                reportParamMapList.Add("14105"); resultList.Add(testMethodNo.Text);    
                reportParamMapList.Add("14100"); resultList.Add(lblvoltage.Text);      
                reportParamMapList.Add("14106"); resultList.Add(jobOrderNo.Text);  
                reportParamMapList.Add("14107"); resultList.Add(worksheetno.Text);    
                reportParamMapList.Add("14102"); resultList.Add(roomno.Text);      
                reportParamMapList.Add("14108"); resultList.Add(roomhumidity.Text);    
                reportParamMapList.Add("14103"); resultList.Add(roomtemp.Text);        
                reportParamMapList.Add("14101"); resultList.Add(Date.Text);            
                reportParamMapList.Add("14109"); resultList.Add(equipmentno.Text);     

                // Convert lists to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                // Call the database save method
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    // Show success message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results and Report Generated successfully!');", true);
                    _cm.sampleStatusID = 27;//Test Result Submitted
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // GenerateTestReport(_cm.sampleId);
                    Response.Redirect($"GenerateTestRecord_ConductorResistance_Revision.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('Error: {ex.Message}');", true);
            }
        }
        private List<string> GetColumnValuesFromGridView(string columnName)
        {
            List<string> values = new List<string>();

            foreach (GridViewRow row in gvResistanceTest.Rows)
            {
                switch (columnName)
                {
                    case "Core":
                        values.Add(row.Cells[0].Text.Trim());
                        break;
                    case "Length":
                        values.Add(row.Cells[1].Text.Trim());
                        break;
                    case "TempFactor":
                        var tempControl = row.Cells[2].Controls[0] as DataBoundLiteralControl;
                        if (tempControl != null)
                        {
                            values.Add(tempControl.Text.Trim());
                        }
                        else
                        {
                            values.Add(""); // fallback
                        }
                        break;
                    case "Rt":
                        values.Add(row.Cells[3].Text.Trim());
                        break;
                    case "R20":
                        values.Add(row.Cells[4].Text.Trim());
                        break;
                }
            }

            return values;
        }

        //Grids
        protected void gvResistanceTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    // Handle header row if needed
                    Label lblTempHeader = (Label)e.Row.FindControl("lblTempHeader");
                    if (lblTempHeader != null && txtTempAtValue != null)
                    {
                        lblTempHeader.Text = txtTempAtValue.Text;
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Get the DataTable from ViewState
                    System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Only apply special formatting to the first row
                        if (e.Row.RowIndex == 0)
                        {
                            // Get the index of the Specified column (should be column 5)
                            int specifiedColumnIndex = 5;

                            // Make sure we have enough columns
                            if (e.Row.Cells.Count > specifiedColumnIndex)
                            {
                                // Set rowspan for the first row's Specified cell
                                TableCell cell = e.Row.Cells[specifiedColumnIndex];
                                cell.RowSpan = dt.Rows.Count;

                                // Get the specified value from the first row's data or from stored value
                                string specifiedValue = "";
                                if (dt.Rows[0]["Specified"] != null && dt.Rows[0]["Specified"] != DBNull.Value)
                                {
                                    specifiedValue = dt.Rows[0]["Specified"].ToString();
                                }

                                if (!string.IsNullOrEmpty(specifiedValue))
                                {
                                    cell.Text = specifiedValue;
                                    _specifiedValue = specifiedValue; // Store for future use
                                }

                                // Style the cell
                                cell.HorizontalAlign = HorizontalAlign.Center;
                                cell.VerticalAlign = VerticalAlign.Middle;
                                cell.CssClass = "specified-cell"; // Add CSS class for styling if needed
                            }
                        }
                        else
                        {
                            // Hide the Specified cell for all other rows
                            if (e.Row.Cells.Count > 5)
                            {
                                e.Row.Cells[5].Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                System.Diagnostics.Debug.WriteLine($"RowDataBound Error: {ex.Message}");
                // Optionally show error in development
                // ShowMessage($"Error in row binding: {ex.Message}");
            }
        }
        protected void gvResistanceTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteRow")
                {
                    // Get the ID of the row to delete
                    string commandArg = e.CommandArgument.ToString();
                    if (string.IsNullOrEmpty(commandArg))
                    {
                        ShowMessage("Invalid row ID for deletion.");
                        return;
                    }

                    Guid rowId;
                    if (!Guid.TryParse(commandArg, out rowId))
                    {
                        ShowMessage("Invalid row ID format.");
                        return;
                    }

                    // Get the DataTable from ViewState
                    System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
                    if (dt != null)
                    {
                        // Find the row with the matching ID
                        DataRow[] rowsToDelete = dt.Select($"ID = '{rowId.ToString()}'");
                        if (rowsToDelete.Length > 0)
                        {
                            // Remove the row
                            dt.Rows.Remove(rowsToDelete[0]);

                            // Update ViewState
                            ViewState["ResistanceTestResults"] = dt;

                            // Check if this was the last row
                            if (dt.Rows.Count == 0)
                            {
                                // No more data - clear everything and make specified field editable
                                gvResistanceTest.DataSource = null;
                                gvResistanceTest.DataBind();

                                txtspecified.ReadOnly = false;
                                txtspecified.CssClass = txtspecified.CssClass.Replace(" readonly-field", "");
                                ViewState["SpecifiedValue"] = null;
                                _specifiedValue = string.Empty;
                                txtspecified.Text = string.Empty;

                                ShowMessage("All rows deleted. You can now enter a new specified value.");
                            }
                            else
                            {
                                // Still have data - rebind the grid
                                gvResistanceTest.DataSource = dt;
                                gvResistanceTest.DataBind();

                                ShowMessage("Row deleted successfully.");
                            }
                        }
                        else
                        {
                            ShowMessage("Row not found for deletion.");
                        }
                    }
                    else
                    {
                        ShowMessage("No data table found.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and show user-friendly message
                ShowMessage($"Error deleting row: {ex.Message}");

                // Optionally log the full exception for debugging
                // System.Diagnostics.Debug.WriteLine($"Delete Error: {ex.ToString()}");
            }
        }
        private void ClearInputFields()
        {
            ddlCorePhase.SelectedIndex = -1;
            txtLength.Text = string.Empty;
            txtTempFactor.Text = string.Empty;
            txtRt.Text = string.Empty;
            txtR20.Text = string.Empty;

            // Only clear specified field if it's the first entry
            System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                txtspecified.Text = string.Empty;
            }
            else
            {
                // Keep the specified value for subsequent entries
                txtspecified.Text = ViewState["SpecifiedValue"]?.ToString() ?? string.Empty;
                txtspecified.ReadOnly = true; // Make it read-only after first entry
                txtspecified.CssClass += " readonly-field"; // Add CSS class for styling
            }
        }
        protected void ResetControlBorders()
        {
            // Reset TextBox borders
            Control[] controls = { txtLength, txtTempFactor, txtRt, txtR20,txtspecified };
            foreach (Control control in controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = Color.Empty;
                    ((TextBox)control).BorderStyle = BorderStyle.NotSet;
                    ((TextBox)control).BorderWidth = 0;
                }
            }

            // Reset DropDownList styles
            ddlCorePhase.CssClass = "form-control";
        }

        //others
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        private void ClearForm()
        {
            // Main Section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";
            txtTempAtValue.Text= "";
            txtspecified.ReadOnly = false; // Make it editable again
            txtspecified.CssClass = txtspecified.CssClass.Replace(" readonly-field", "");

            // Test Details
            ddlCorePhase.SelectedIndex = 0;
            txtLength.Text = "";
            txtTempFactor.Text = "";
            txtRt.Text = "";
            txtR20.Text = "";

            // Clear GridView
            ViewState["ResistanceTestResults"] = null;
            ViewState["SpecifiedValue"] = null;
            _specifiedValue = string.Empty;
            gvResistanceTest.DataSource = null;
            gvResistanceTest.DataBind();

            ResetControlBorders();

        }

        //PRE-FILLING DATA IF EXISTS FOR SAMPLEID
        protected void CheckAndPrefillForm()
        {
            try
            {
                // Check if there's a sample ID in the query string
                if (Request.QueryString["s"] != null)
                {
                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int reportId = Convert.ToInt32(Request.QueryString["r"]);

                    // Create DAL object
                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;
                    _cm.ReportID = reportId.ToString();

                    // Pass both sampleId and reportId
                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        // Show update button since data exists
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        // Prefill upper section details
                        PrefillUpperSection(existingData);

                        // Prefill values
                        PrefillTestResultsGrid(existingData);

                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }
        private DataTable GetExistingTestResultsForSample(int sampleId, int reportId)
        {
            Dictionary<string, string> parameterResults = new Dictionary<string, string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT ReportParamMapID, Result 
                FROM TestResultsPTQualis 
                WHERE SampleID = @SampleID AND ReportID = @ReportID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string paramId = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();
                                parameterResults[paramId] = result;
                            }
                        }
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("ParameterID", typeof(string));
                dt.Columns.Add("Result", typeof(string));

                foreach (var param in parameterResults)
                {
                    dt.Rows.Add(param.Key, param.Value);
                }

                return dt;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DataFetchError",
                    $"alert('Error fetching existing data: {ex.Message}');", true);
                return null;
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "14098");
            SetLabelText(refSpec, "14104");
            SetLabelText(cableSize, "14099");
            SetLabelText(testMethodNo, "14105");
            SetLabelText(lblvoltage, "14100");
            SetLabelText(jobOrderNo, "14106");
            SetLabelText(worksheetno, "14107");
            SetLabelText(equipmentno, "14109");

            // Textbox fields
            SetTextBoxText(roomno, "14102");
            SetTextBoxText(roomhumidity, "14108");
            SetTextBoxText(roomtemp, "14103");
            SetTextBoxText(Date, "14101");

            SetTextBoxText(txtTempAtValue, "14110");

            // Helper method to set text for labels
            void SetLabelText(Label label, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

            // Helper method to set text for textboxes
            void SetTextBoxText(TextBox textBox, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }
        protected void PrefillTestResultsGrid(DataTable existingData)
        {
            try
            {
                string coreValues = GetParameterValue(existingData, "14093");
                string lengthValues = GetParameterValue(existingData, "14094");
                string tempFactorValues = GetParameterValue(existingData, "14095");
                string rtValues = GetParameterValue(existingData, "14096");
                string r20Values = GetParameterValue(existingData, "14097");
                string specifiedValue = GetParameterValue(existingData, "14159");

                if (!string.IsNullOrEmpty(coreValues))
                {
                    string[] cores = coreValues.Split(',');
                    string[] lengths = lengthValues.Split(',');
                    string[] tempFactors = tempFactorValues.Split(',');
                    string[] rts = rtValues.Split(',');
                    string[] r20s = r20Values.Split(',');

                    // Create a new DataTable
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("Core", typeof(string));
                    dt.Columns.Add("Length", typeof(decimal));
                    dt.Columns.Add("TempFactor", typeof(decimal));
                    dt.Columns.Add("Rt", typeof(decimal));
                    dt.Columns.Add("R20", typeof(decimal));
                    dt.Columns.Add("Specified", typeof(string));
                    dt.Columns.Add("ID", typeof(Guid)); // Add an ID for unique identification

                    // Add rows to the DataTable
                    for (int i = 0; i < cores.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(cores[i]))
                        {
                            DataRow dr = dt.NewRow();
                            dr["Core"] = cores[i];
                            dr["Length"] = Convert.ToDecimal(lengths[i]);
                            dr["TempFactor"] = Convert.ToDecimal(tempFactors[i]);
                            dr["Rt"] = Convert.ToDecimal(rts[i]);
                            dr["R20"] = Convert.ToDecimal(r20s[i]);
                            dr["Specified"] = specifiedValue ?? string.Empty;
                            dr["ID"] = Guid.NewGuid(); 
                            dt.Rows.Add(dr);
                        }
                    }
                    ViewState["SpecifiedValue"] = specifiedValue;

                    ViewState["ResistanceTestResults"] = dt;
                    gvResistanceTest.DataSource = dt;
                    gvResistanceTest.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error loading test grid data: {ex.Message}');", true);
            }
        }

        private string GetParameterValue(DataTable data, string reportParamMapId)
        {
            foreach (DataRow row in data.Rows)
            {
                // Using ParameterID instead of ReportParamMapID to match your actual column name
                if (row["ParameterID"].ToString() == reportParamMapId)
                {
                    return row["Result"].ToString();
                }
            }
            return string.Empty;
        }
        private void PopulateCoreDropdown()
        {
            try
            {
                // Get Core data from database
                DataTable coreData = _cm.Get_Core();

                // Clear existing items
                ddlCorePhase.Items.Clear();


                // Check if we got data
                if (coreData != null && coreData.Rows.Count > 0)
                {
                    // Bind data to dropdown
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddlCorePhase.Items.Add(new ListItem(coreName, coreId));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                ScriptManager.RegisterStartupScript(this, GetType(), "DropdownError",
                    $"alert('Error loading core data: {ex.Message}');", true);
            }
        }

    }
}