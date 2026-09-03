using LIMS.DAL;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;

namespace LIMS
{
    public partial class ConductorResistance : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //upper section
                roomno.Text = "-";
                cableType.Text = "Cu/XLPE/LS/DSTA/PE";
                refSpec.Text = "IEC 60502-2,IEC 60228";
                cableSize.Text = "3 x 25 mm²";
                testMethodNo.Text = "IEC 60502-2";
                lblvoltage.Text = "3,6/6(7,2)kV";
                jobOrderNo.Text = "WO/2024120905";
                worksheetno.Text = "-";
                equipmentno.Text = "QE/15/El/41";

                CheckAndPrefillForm();
            }

        }

        //Buttons 
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, tempFactor = 0, rt = 0, r20 = 0, Specified = 0;

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
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
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

            // Create a new row with a unique ID
            DataRow dr = dt.NewRow();
            dr["Core"] = ddlCorePhase.SelectedValue;
            dr["Length"] = length;
            dr["TempFactor"] = tempFactor;
            dr["Rt"] = rt;
            dr["R20"] = r20;
            dr["Specified"] = Specified;
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

            // Validate Main Section (required regardless of grid data)
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
                // Save data logic here (e.g., database insertion)
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

            // Validate Main Section (required regardless of grid data)
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
                try
                {
                    if (Request.QueryString["s"] != null)
                    {
                        int sampleId = Convert.ToInt32(Request.QueryString["s"]);

                        // Create DAL object
                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;


                        // First delete existing records for this sample
                        DeleteExistingRecords(sampleId);

                        // Then save the updated data (reusing your Submitform logic)
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
        private void DeleteExistingRecords(int sampleId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
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

                // Extract data from GridView directly (instead of using testTypeData)
                // This assumes you're binding the GridView to a DataTable or similar source

                // Create comma-separated values for each column
                string coreValues = string.Join(",", GetColumnValuesFromGridView("Core"));
                string lengthValues = string.Join(",", GetColumnValuesFromGridView("Length"));
                string tempFactorValues = string.Join(",", GetColumnValuesFromGridView("Temp. Factor Correction at 20ºC"));
                string rtValues = string.Join(",", GetColumnValuesFromGridView("Rt (Ω/L)"));
                string r20cValues = string.Join(",", GetColumnValuesFromGridView("R20ºC (Ω/km)"));

                // Add the measurements to the parameter lists using correct ReportParamMapIDs
                reportParamMapList.Add("2138"); resultList.Add(coreValues);           // Core
                reportParamMapList.Add("2139"); resultList.Add(lengthValues);         // Length
                reportParamMapList.Add("2140"); resultList.Add(tempFactorValues);     // TempFactorCorrection
                reportParamMapList.Add("2141"); resultList.Add(rtValues);             // Rt
                reportParamMapList.Add("2142"); resultList.Add(r20cValues);           // R20C

                //Upper section
                reportParamMapList.Add("4177"); resultList.Add(cableType.Text);
                reportParamMapList.Add("4183"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("4178"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("4184"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("4179"); resultList.Add(lblvoltage.Text);
                reportParamMapList.Add("4185"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("4186"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("4181"); resultList.Add(roomno.Text);
                reportParamMapList.Add("4187"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("4182"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("4180"); resultList.Add(Date.Text);
                reportParamMapList.Add("4188"); resultList.Add(equipmentno.Text);

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
                    Response.Redirect($"GenerateTestRecord_ConductorResistance.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
                // Log error and show message
                // Log.Error(ex);
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('Error: {ex.Message}');", true);
            }
        }
        private List<string> GetColumnValuesFromGridView(string columnName)
        {
            List<string> values = new List<string>();

            // Assuming your GridView is named gvResistanceTest
            GridView gv = gvResistanceTest;

            // Loop through each row in the GridView
            foreach (GridViewRow row in gv.Rows)
            {
                // Find control based on column name
                switch (columnName)
                {
                    case "Core":
                        // Assuming first cell (index 0) contains Core data
                        values.Add(row.Cells[0].Text);
                        break;
                    case "Length":
                        // Assuming second cell (index 1) contains Length data
                        values.Add(row.Cells[1].Text);
                        break;
                    case "Temp. Factor Correction at 20ºC":
                        // Assuming third cell (index 2) contains Temperature Factor data
                        values.Add(row.Cells[2].Text);
                        break;
                    case "Rt (Ω/L)":
                        // Assuming fourth cell (index 3) contains Rt data
                        values.Add(row.Cells[3].Text);
                        break;
                    case "R20ºC (Ω/km)":
                        // Assuming fifth cell (index 4) contains R20C data
                        values.Add(row.Cells[4].Text);
                        break;
                }
            }

            return values;
        }

        //Grids
        protected void gvResistanceTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                // Get the ID of the row to delete
                Guid rowId = new Guid(e.CommandArgument.ToString());

                // Get the DataTable from ViewState
                System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
                if (dt != null)
                {
                    // Find the row with the matching ID
                    DataRow[] rowsToDelete = dt.Select("ID = '" + rowId.ToString() + "'");
                    if (rowsToDelete.Length > 0)
                    {
                        // Remove the row
                        dt.Rows.Remove(rowsToDelete[0]);

                        // Update ViewState and rebind the grid
                        ViewState["ResistanceTestResults"] = dt;
                        gvResistanceTest.DataSource = dt;
                        gvResistanceTest.DataBind();
                    }
                }
            }
        }
        protected void gvResistanceTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the DataTable from ViewState
                System.Data.DataTable dt = ViewState["ResistanceTestResults"] as System.Data.DataTable;
                if (dt != null)
                {
                    // Only apply special formatting to the first row
                    if (e.Row.RowIndex == 0)
                    {
                        // Get the index of the Specified column
                        int specifiedColumnIndex = 5; // Adjust this based on your actual column index

                        // Set rowspan for the first row's Specified cell
                        TableCell cell = e.Row.Cells[specifiedColumnIndex];
                        cell.RowSpan = dt.Rows.Count;
                        cell.Text = "Max.: 0.727";

                        // Style the cell
                        cell.HorizontalAlign = HorizontalAlign.Right;
                        cell.VerticalAlign = VerticalAlign.Bottom;
                    }
                    else
                    {
                        // Hide the Specified cell for all other rows
                        e.Row.Cells[5].Visible = false;
                    }
                }
            }
        }
        private void ClearInputFields()
        {
            txtLength.Text = "";
            txtTempFactor.Text = "";
            txtRt.Text = "";
            txtR20.Text = "";
        }
        protected void ResetControlBorders()
        {
            // Reset TextBox borders
            Control[] controls = { txtLength, txtTempFactor, txtRt, txtR20 };
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

            // Test Details
            ddlCorePhase.SelectedIndex = 0;
            txtLength.Text = "";
            txtTempFactor.Text = "";
            txtRt.Text = "";
            txtR20.Text = "";

            // Clear GridView
            gvResistanceTest.DataSource = null;
            gvResistanceTest.DataBind();

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
            // Create a dictionary to store parameter results
            Dictionary<string, string> parameterResults = new Dictionary<string, string>();

            try
            {
                // Fetch all results for this sample
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

                // Create a DataTable to return the results
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
                // Log error
                ScriptManager.RegisterStartupScript(this, GetType(), "DataFetchError",
                    $"alert('Error fetching existing data: {ex.Message}');", true);
                return null;
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "4177");
            SetLabelText(refSpec, "4183");
            SetLabelText(cableSize, "4178");
            SetLabelText(testMethodNo, "4184");
            SetLabelText(lblvoltage, "4179");
            SetLabelText(jobOrderNo, "4185");
            SetLabelText(worksheetno, "4186");
            SetLabelText(equipmentno, "4188");

            // Textbox fields
            SetTextBoxText(roomno, "4181");
            SetTextBoxText(roomhumidity, "4187");
            SetTextBoxText(roomtemp, "4182");
            SetTextBoxText(Date, "4180");

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
                string coreValues = GetParameterValue(existingData, "2138");
                string lengthValues = GetParameterValue(existingData, "2139");
                string tempFactorValues = GetParameterValue(existingData, "2140");
                string rtValues = GetParameterValue(existingData, "2141");
                string r20Values = GetParameterValue(existingData, "2142");

                // If we have data, parse it and rebuild the grid
                if (!string.IsNullOrEmpty(coreValues))
                {
                    // Split the comma-separated values
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
                            dr["Specified"] = "";
                            dr["ID"] = Guid.NewGuid(); // Generate new IDs for each row
                            dt.Rows.Add(dr);
                        }
                    }

                    // Store in ViewState and bind to the grid
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

        // Helper method to extract parameter values from the data table
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

    }
}