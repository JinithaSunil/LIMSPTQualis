using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ElectricalWaterAbsorptionTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();
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
                    roomno.Text = "SHIELDING";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/15/EL/34";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();
            }
        }

        //Buttons
        protected void btnAddToGridAC_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBordersAC();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, immerse = 0, temperature = 0, voltage = 0, time = 0;
            string result = txtresultsAC.Text.Trim();

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdateAC.Text))
            {
                invalidControls.Add(testdateAC);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCoreAC.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCoreAC.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlCoreAC.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLengthAC.Text) || !decimal.TryParse(txtLengthAC.Text, out length))
            {
                invalidControls.Add(txtLengthAC);
                hasError = true;
            }

            // Validate Immerse
            if (string.IsNullOrWhiteSpace(txtImmerseAC.Text) || !decimal.TryParse(txtImmerseAC.Text, out immerse))
            {
                invalidControls.Add(txtImmerseAC);
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTempAC.Text) || !decimal.TryParse(txtTempAC.Text, out temperature))
            {
                invalidControls.Add(txtTempAC);
                hasError = true;
            }

            // Validate Voltage
            if (string.IsNullOrWhiteSpace(txtVoltageAC.Text) || !decimal.TryParse(txtVoltageAC.Text, out voltage))
            {
                invalidControls.Add(txtVoltageAC);
                hasError = true;
            }

            // Validate Time
            if (string.IsNullOrWhiteSpace(txtTimeAC.Text) || !decimal.TryParse(txtTimeAC.Text, out time))
            {
                invalidControls.Add(txtTimeAC);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150); // Darker red
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["ACTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Immerse", typeof(decimal));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("Voltage", typeof(decimal));
                dt.Columns.Add("Time", typeof(decimal));
                dt.Columns.Add("Result", typeof(string));
                dt.Columns.Add("Specified", typeof(string));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdateAC.Text;
            dr["Core"] = ddlCoreAC.SelectedItem.Text;
            dr["Length"] = length;
            dr["Immerse"] = immerse;
            dr["Temperature"] = temperature;
            dr["Voltage"] = voltage;
            dr["Time"] = time;
            dr["Result"] = result;
            dr["Specified"] = txtspecifiedAC.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["ACTestResults"] = dt;

            // Rebind the GridView
            gvVoltageTestAC.DataSource = dt;
            gvVoltageTestAC.DataBind();

            // Clear the input fields for next entry
            ClearInputFieldsAC();

        }
        protected void btnAddToGridDC_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBordersDC();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, immerse = 0, temperature = 0, voltage = 0, time = 0;
            string result = txtresultsDC.Text.Trim();
            // Validate Date
            if (string.IsNullOrWhiteSpace(testdateDC.Text))
            {
                invalidControls.Add(testdateDC);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCoreDC.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCoreDC.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlCoreDC.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLengthDC.Text) || !decimal.TryParse(txtLengthDC.Text, out length))
            {
                invalidControls.Add(txtLengthDC);
                hasError = true;
            }

            // Validate Immerse
            if (string.IsNullOrWhiteSpace(txtImmerseDC.Text) || !decimal.TryParse(txtImmerseDC.Text, out immerse))
            {
                invalidControls.Add(txtImmerseDC);
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTempDC.Text) || !decimal.TryParse(txtTempDC.Text, out temperature))
            {
                invalidControls.Add(txtTempDC);
                hasError = true;
            }

            // Validate Voltage
            if (string.IsNullOrWhiteSpace(txtVoltageDC.Text) || !decimal.TryParse(txtVoltageDC.Text, out voltage))
            {
                invalidControls.Add(txtVoltageDC);
                hasError = true;
            }

            // Validate Time
            if (string.IsNullOrWhiteSpace(txtTimeDC.Text) || !decimal.TryParse(txtTimeDC.Text, out time))
            {
                invalidControls.Add(txtTimeDC);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150); // Darker red
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["DCTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Immerse", typeof(decimal));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("Voltage", typeof(decimal));
                dt.Columns.Add("Time", typeof(decimal));
                dt.Columns.Add("Result", typeof(string));
                dt.Columns.Add("Specified", typeof(string));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdateDC.Text;
            dr["Core"] = ddlCoreDC.SelectedItem.Text;
            dr["Length"] = length;
            dr["Immerse"] = immerse;
            dr["Temperature"] = temperature;
            dr["Voltage"] = voltage;
            dr["Time"] = time;
            dr["Result"] = result;
            dr["Specified"] = txtspecifiedDC.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["DCTestResults"] = dt;

            // Rebind the GridView
            gvVoltageTestDC.DataSource = dt;
            gvVoltageTestDC.DataBind();

            // Clear the input fields for next entry
            ClearInputFieldsDC();

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check GridView counts
            bool hasACData = gvVoltageTestAC.Rows.Count > 0;
            bool hasDCData = gvVoltageTestDC.Rows.Count > 0;

            // Validate Main Info
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
                errors.Add("Room Temperature must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // If AC GridView is empty, validate manual AC entry
            if (!hasACData)
            {
                if (string.IsNullOrEmpty(testdateAC.Text))
                {
                    errors.Add("AC Test Date is required.");
                    invalidControls.Add(testdateAC);
                }
                if (ddlCoreAC.SelectedIndex == 0)
                {
                    errors.Add("Select a Core (AC).");
                    invalidControls.Add(ddlCoreAC);
                }
                if (string.IsNullOrEmpty(txtLengthAC.Text) || !double.TryParse(txtLengthAC.Text, out _))
                {
                    errors.Add("Length (AC) must be a valid number.");
                    invalidControls.Add(txtLengthAC);
                }
                if (string.IsNullOrEmpty(txtImmerseAC.Text) || !double.TryParse(txtImmerseAC.Text, out _))
                {
                    errors.Add("Immerse (AC) must be a valid number.");
                    invalidControls.Add(txtImmerseAC);
                }
                if (string.IsNullOrEmpty(txtTempAC.Text) || !double.TryParse(txtTempAC.Text, out _))
                {
                    errors.Add("Temperature (AC) must be a valid number.");
                    invalidControls.Add(txtTempAC);
                }
                if (string.IsNullOrEmpty(txtVoltageAC.Text) || !double.TryParse(txtVoltageAC.Text, out _))
                {
                    errors.Add("Voltage (AC) must be a valid number.");
                    invalidControls.Add(txtVoltageAC);
                }
                if (string.IsNullOrEmpty(txtTimeAC.Text) || !double.TryParse(txtTimeAC.Text, out _))
                {
                    errors.Add("Time (AC) must be a valid number.");
                    invalidControls.Add(txtTimeAC);
                }
                if (string.IsNullOrEmpty(txtresultsAC.Text))
                {
                    errors.Add("Result (AC) is required.");
                    invalidControls.Add(txtresultsAC);
                }
            }

            // If DC GridView is empty, validate manual DC entry
            if (!hasDCData)
            {
                if (string.IsNullOrEmpty(testdateDC.Text))
                {
                    errors.Add("DC Test Date is required.");
                    invalidControls.Add(testdateDC);
                }
                if (ddlCoreDC.SelectedIndex == 0)
                {
                    errors.Add("Select a Core (DC).");
                    invalidControls.Add(ddlCoreDC);
                }
                if (string.IsNullOrEmpty(txtLengthDC.Text) || !double.TryParse(txtLengthDC.Text, out _))
                {
                    errors.Add("Length (DC) must be a valid number.");
                    invalidControls.Add(txtLengthDC);
                }
                if (string.IsNullOrEmpty(txtImmerseDC.Text) || !double.TryParse(txtImmerseDC.Text, out _))
                {
                    errors.Add("Immerse (DC) must be a valid number.");
                    invalidControls.Add(txtImmerseDC);
                }
                if (string.IsNullOrEmpty(txtTempDC.Text) || !double.TryParse(txtTempDC.Text, out _))
                {
                    errors.Add("Temperature (DC) must be a valid number.");
                    invalidControls.Add(txtTempDC);
                }
                if (string.IsNullOrEmpty(txtVoltageDC.Text) || !double.TryParse(txtVoltageDC.Text, out _))
                {
                    errors.Add("Voltage (DC) must be a valid number.");
                    invalidControls.Add(txtVoltageDC);
                }
                if (string.IsNullOrEmpty(txtTimeDC.Text) || !double.TryParse(txtTimeDC.Text, out _))
                {
                    errors.Add("Time (DC) must be a valid number.");
                    invalidControls.Add(txtTimeDC);
                }
                if (string.IsNullOrEmpty(txtresultsDC.Text))
                {
                    errors.Add("Result (DC) is required.");
                    invalidControls.Add(txtresultsDC);
                }
            }

            // Ensure at least one test set is filled
            if (!hasACData && !hasDCData)
            {
                errors.Add("Add at least one AC or DC test to the grid.");
            }

            // Reset all previous error styles
            ResetControlBorders();

            if (errors.Count > 0)
            {
                foreach (var control in invalidControls)
                {
                    if (control is TextBox txt)
                    {
                        txt.BorderColor = System.Drawing.Color.Red;
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
                    }
                    else if (control is DropDownList ddl)
                    {
                        ddl.BorderColor = System.Drawing.Color.Red;
                        ddl.BorderStyle = BorderStyle.Solid;
                        ddl.BorderWidth = 1;
                    }
                }

                string msg = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{msg}');", true);
            }
            else
            {
                Submitform();
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                    "alert('Test results and report saved successfully!');", true);
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

            // Check GridView counts
            bool hasACData = gvVoltageTestAC.Rows.Count > 0;
            bool hasDCData = gvVoltageTestDC.Rows.Count > 0;

            // Validate Main Info
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
                errors.Add("Room Temperature must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // If AC GridView is empty, validate manual AC entry
            if (!hasACData)
            {
                if (string.IsNullOrEmpty(testdateAC.Text))
                {
                    errors.Add("AC Test Date is required.");
                    invalidControls.Add(testdateAC);
                }
                if (ddlCoreAC.SelectedIndex == 0)
                {
                    errors.Add("Select a Core (AC).");
                    invalidControls.Add(ddlCoreAC);
                }
                if (string.IsNullOrEmpty(txtLengthAC.Text) || !double.TryParse(txtLengthAC.Text, out _))
                {
                    errors.Add("Length (AC) must be a valid number.");
                    invalidControls.Add(txtLengthAC);
                }
                if (string.IsNullOrEmpty(txtImmerseAC.Text) || !double.TryParse(txtImmerseAC.Text, out _))
                {
                    errors.Add("Immerse (AC) must be a valid number.");
                    invalidControls.Add(txtImmerseAC);
                }
                if (string.IsNullOrEmpty(txtTempAC.Text) || !double.TryParse(txtTempAC.Text, out _))
                {
                    errors.Add("Temperature (AC) must be a valid number.");
                    invalidControls.Add(txtTempAC);
                }
                if (string.IsNullOrEmpty(txtVoltageAC.Text) || !double.TryParse(txtVoltageAC.Text, out _))
                {
                    errors.Add("Voltage (AC) must be a valid number.");
                    invalidControls.Add(txtVoltageAC);
                }
                if (string.IsNullOrEmpty(txtTimeAC.Text) || !double.TryParse(txtTimeAC.Text, out _))
                {
                    errors.Add("Time (AC) must be a valid number.");
                    invalidControls.Add(txtTimeAC);
                }
                if (string.IsNullOrEmpty(txtresultsAC.Text))
                {
                    errors.Add("Result (AC) is required.");
                    invalidControls.Add(txtresultsAC);
                }
            }

            // If DC GridView is empty, validate manual DC entry
            if (!hasDCData)
            {
                if (string.IsNullOrEmpty(testdateDC.Text))
                {
                    errors.Add("DC Test Date is required.");
                    invalidControls.Add(testdateDC);
                }
                if (ddlCoreDC.SelectedIndex == 0)
                {
                    errors.Add("Select a Core (DC).");
                    invalidControls.Add(ddlCoreDC);
                }
                if (string.IsNullOrEmpty(txtLengthDC.Text) || !double.TryParse(txtLengthDC.Text, out _))
                {
                    errors.Add("Length (DC) must be a valid number.");
                    invalidControls.Add(txtLengthDC);
                }
                if (string.IsNullOrEmpty(txtImmerseDC.Text) || !double.TryParse(txtImmerseDC.Text, out _))
                {
                    errors.Add("Immerse (DC) must be a valid number.");
                    invalidControls.Add(txtImmerseDC);
                }
                if (string.IsNullOrEmpty(txtTempDC.Text) || !double.TryParse(txtTempDC.Text, out _))
                {
                    errors.Add("Temperature (DC) must be a valid number.");
                    invalidControls.Add(txtTempDC);
                }
                if (string.IsNullOrEmpty(txtVoltageDC.Text) || !double.TryParse(txtVoltageDC.Text, out _))
                {
                    errors.Add("Voltage (DC) must be a valid number.");
                    invalidControls.Add(txtVoltageDC);
                }
                if (string.IsNullOrEmpty(txtTimeDC.Text) || !double.TryParse(txtTimeDC.Text, out _))
                {
                    errors.Add("Time (DC) must be a valid number.");
                    invalidControls.Add(txtTimeDC);
                }
                if (string.IsNullOrEmpty(txtresultsDC.Text))
                {
                    errors.Add("Result (DC) is required.");
                    invalidControls.Add(txtresultsDC);
                }
            }

            // Ensure at least one test set is filled
            if (!hasACData && !hasDCData)
            {
                errors.Add("Add at least one AC or DC test to the grid.");
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

                        int reportid = Convert.ToInt32(Request.QueryString["r"]);
                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;

                        DeleteExistingRecords(sampleId, reportid);

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

        //Grids
        protected void gvVoltageTestAC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                if (e.Row.RowIndex > 0)
                {
                    // Merge SPECIFIED column 
                    string currentSpecified = rowView["Specified"]?.ToString() ?? "";
                    string previousSpecified = dt.Rows[e.Row.RowIndex - 1]["Specified"]?.ToString() ?? "";

                    if (currentSpecified == previousSpecified)
                    {
                        int specifiedStartRowIndex = e.Row.RowIndex - 1;
                        while (specifiedStartRowIndex > 0 &&
                              (dt.Rows[specifiedStartRowIndex - 1]["Specified"]?.ToString() ?? "") == currentSpecified)
                        {
                            specifiedStartRowIndex--;
                        }

                        // Find the cell index for "Specified" column
                        int specifiedCellIndex = -1;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            if (((BoundField)gridView.Columns[i]).DataField == "Specified")
                            {
                                specifiedCellIndex = i;
                                break;
                            }
                        }
                        if (specifiedCellIndex >= 0)
                        {
                            if (gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan == 0)
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan = 2;
                            else
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan++;

                            e.Row.Cells[specifiedCellIndex].Visible = false;
                        }
                    }
                }
            // Format numeric values to show appropriate decimal places
            if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Immerse"] != null && decimal.TryParse(rowView["Immerse"].ToString(), out decimal immerse))
                {
                    e.Row.Cells[3].Text = immerse.ToString("0.##");
                }

                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[4].Text = temperature.ToString("0.##");
                }

                if (rowView["Voltage"] != null && decimal.TryParse(rowView["Voltage"].ToString(), out decimal voltage))
                {
                    e.Row.Cells[5].Text = voltage.ToString("0.##");
                }

                if (rowView["Time"] != null && decimal.TryParse(rowView["Time"].ToString(), out decimal time))
                {
                    e.Row.Cells[6].Text = time.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this AC voltage test record?');");
                }
            }

        }
        protected void gvVoltageTestAC_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["ACTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["ACTestResults"] = dt;

                        // Rebind the GridView
                        gvVoltageTestAC.DataSource = dt;
                        gvVoltageTestAC.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gvVoltageTestDC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                if (e.Row.RowIndex > 0)
                {
                    // Merge SPECIFIED column 
                    string currentSpecified = rowView["Specified"]?.ToString() ?? "";
                    string previousSpecified = dt.Rows[e.Row.RowIndex - 1]["Specified"]?.ToString() ?? "";

                    if (currentSpecified == previousSpecified)
                    {
                        int specifiedStartRowIndex = e.Row.RowIndex - 1;
                        while (specifiedStartRowIndex > 0 &&
                              (dt.Rows[specifiedStartRowIndex - 1]["Specified"]?.ToString() ?? "") == currentSpecified)
                        {
                            specifiedStartRowIndex--;
                        }

                        // Find the cell index for "Specified" column
                        int specifiedCellIndex = -1;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            if (((BoundField)gridView.Columns[i]).DataField == "Specified")
                            {
                                specifiedCellIndex = i;
                                break;
                            }
                        }
                        if (specifiedCellIndex >= 0)
                        {
                            if (gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan == 0)
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan = 2;
                            else
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan++;

                            e.Row.Cells[specifiedCellIndex].Visible = false;
                        }
                    }
                }

                if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Immerse"] != null && decimal.TryParse(rowView["Immerse"].ToString(), out decimal immerse))
                {
                    e.Row.Cells[3].Text = immerse.ToString("0.##");
                }

                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[4].Text = temperature.ToString("0.##");
                }

                if (rowView["Voltage"] != null && decimal.TryParse(rowView["Voltage"].ToString(), out decimal voltage))
                {
                    e.Row.Cells[5].Text = voltage.ToString("0.##");
                }

                if (rowView["Time"] != null && decimal.TryParse(rowView["Time"].ToString(), out decimal time))
                {
                    e.Row.Cells[6].Text = time.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this DC voltage test record?');");
                }
            }

        }
        protected void gvVoltageTestDC_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["DCTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["DCTestResults"] = dt;

                        // Rebind the GridView
                        gvVoltageTestDC.DataSource = dt;
                        gvVoltageTestDC.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }

        //Saving Data
        private void Submitform()
        {
            try
            {
                // Get GridView data from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                if ((dt == null || dt.Rows.Count == 0) && gvVoltageTestAC.Rows.Count == 0 && gvVoltageTestDC.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('No test data to save. Please add results first.');", true);
                    return;
                }

                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Upper Section
                reportParamMapList.Add("14224"); resultList.Add(cableType.Text);        // Type
                reportParamMapList.Add("14225"); resultList.Add(cableSize.Text);        // Size
                reportParamMapList.Add("14226"); resultList.Add(lblvoltage.Text);       // Voltage
                reportParamMapList.Add("14227"); resultList.Add(Date.Text);             // Date
                reportParamMapList.Add("14228"); resultList.Add(roomno.Text);           // Room No
                reportParamMapList.Add("14229"); resultList.Add(roomtemp.Text);         // Room Temp
                reportParamMapList.Add("14230"); resultList.Add(refSpec.Text);          // Ref Spec
                reportParamMapList.Add("14231"); resultList.Add(testMethodNo.Text);     // Test Method No
                reportParamMapList.Add("14232"); resultList.Add(jobOrderNo.Text);       // Job Order No
                reportParamMapList.Add("14233"); resultList.Add(worksheetno.Text);      // Worksheet No
                reportParamMapList.Add("14234"); resultList.Add(roomhumidity.Text);     // Room Humidity
                reportParamMapList.Add("14235"); resultList.Add(equipmentno.Text);      // Equipment No

                // AC TEST 
                if (gvVoltageTestAC.Rows.Count > 0)
                {
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> immerseList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> voltageList = new List<string>();
                    List<string> timeList = new List<string>();
                    List<string> resultACList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    string dateAC = ""; // assuming same date for all rows

                    foreach (GridViewRow row in gvVoltageTestAC.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if (string.IsNullOrEmpty(dateAC))
                                dateAC = row.Cells[0].Text; // Date column

                            coreList.Add(row.Cells[1].Text);        // Core
                            lengthList.Add(row.Cells[2].Text);      // Length
                            immerseList.Add(row.Cells[3].Text);     // Immerse
                            tempList.Add(row.Cells[4].Text);        // Temperature
                            voltageList.Add(row.Cells[5].Text);     // Voltage
                            timeList.Add(row.Cells[6].Text);        // Time
                            resultACList.Add(row.Cells[7].Text);    // Result
                            specifiedList.Add(row.Cells[8].Text);   // Specified
                        }
                    }

                    reportParamMapList.Add("14236"); resultList.Add(dateAC);
                    reportParamMapList.Add("14237"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("14238"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("14239"); resultList.Add(string.Join(",", immerseList));
                    reportParamMapList.Add("14240"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("14241"); resultList.Add(string.Join(",", voltageList));
                    reportParamMapList.Add("14242"); resultList.Add(string.Join(",", timeList));
                    reportParamMapList.Add("14243"); resultList.Add(string.Join(",", resultACList));
                    reportParamMapList.Add("14244"); resultList.Add(string.Join(",", specifiedList));
                }

                // DC TEST 
                if (gvVoltageTestDC.Rows.Count > 0)
                {
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> immerseList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> voltageList = new List<string>();
                    List<string> timeList = new List<string>();
                    List<string> resultDCList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    string dateDC = ""; // assuming same date for all rows

                    foreach (GridViewRow row in gvVoltageTestDC.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if (string.IsNullOrEmpty(dateDC))
                                dateDC = row.Cells[0].Text; // Date column

                            coreList.Add(row.Cells[1].Text);        // Core
                            lengthList.Add(row.Cells[2].Text);      // Length
                            immerseList.Add(row.Cells[3].Text);     // Immerse
                            tempList.Add(row.Cells[4].Text);        // Temperature
                            voltageList.Add(row.Cells[5].Text);     // Voltage
                            timeList.Add(row.Cells[6].Text);        // Time
                            resultDCList.Add(row.Cells[7].Text);    // Result
                            specifiedList.Add(row.Cells[8].Text);   // Specified
                        }
                    }

                    reportParamMapList.Add("14245"); resultList.Add(dateDC);
                    reportParamMapList.Add("14246"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("14247"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("14248"); resultList.Add(string.Join(",", immerseList));
                    reportParamMapList.Add("14249"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("14250"); resultList.Add(string.Join(",", voltageList));
                    reportParamMapList.Add("14251"); resultList.Add(string.Join(",", timeList));
                    reportParamMapList.Add("14252"); resultList.Add(string.Join(",", resultDCList));
                    reportParamMapList.Add("14253"); resultList.Add(string.Join(",", specifiedList));
                }

                // Convert lists to arrays and submit to DB
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Test results saved successfully!');", true);
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    Response.Redirect($"GenerateTestRecord_ElectricalWaterAbsorptionTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save test results.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
            }
        }

        //Other
        private void ClearInputFieldsAC()
        {
            // Clear only AC Section input fields
            ddlCoreAC.SelectedIndex = 0;
            txtLengthAC.Text = "";
            txtImmerseAC.Text = "";
            txtTempAC.Text = "";
            txtVoltageAC.Text = "";
            txtresultsAC.Text = "";
            txtTimeAC.Text = "";
        }
        private void ClearInputFieldsDC()
        {
            // Clear only DC Section input fields
            txtresultsDC.Text = "";
            ddlCoreDC.SelectedIndex = 0;
            txtLengthDC.Text = "";
            txtImmerseDC.Text = "";
            txtTempDC.Text = "";
            txtVoltageDC.Text = "";
            txtTimeDC.Text = "";
        }
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        private void ClearForm()
        {
            // AC Test Details
            testdateAC.Text = "";
            ddlCoreAC.SelectedIndex = 0;
            txtLengthAC.Text = "";
            txtImmerseAC.Text = "";
            txtTempAC.Text = "";
            txtVoltageAC.Text = "";
            txtTimeAC.Text = "";

            // DC Test Details
            testdateDC.Text = "";
            ddlCoreDC.SelectedIndex = 0;
            txtLengthDC.Text = "";
            txtImmerseDC.Text = "";
            txtTempDC.Text = "";
            txtVoltageDC.Text = "";
            txtTimeDC.Text = "";

            // Clear GridViews
            gvVoltageTestAC.DataSource = null;
            gvVoltageTestAC.DataBind();

            gvVoltageTestDC.DataSource = null;
            gvVoltageTestDC.DataBind();

        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add AC Section TextBox controls
            allTextBoxes.Add(testdateAC);
            allTextBoxes.Add(txtLengthAC);
            allTextBoxes.Add(txtImmerseAC);
            allTextBoxes.Add(txtTempAC);
            allTextBoxes.Add(txtVoltageAC);
            allTextBoxes.Add(txtTimeAC);

            // Add DC Section TextBox controls
            allTextBoxes.Add(testdateDC);
            allTextBoxes.Add(txtLengthDC);
            allTextBoxes.Add(txtImmerseDC);
            allTextBoxes.Add(txtTempDC);
            allTextBoxes.Add(txtVoltageDC);
            allTextBoxes.Add(txtTimeDC);


            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
            List<DropDownList> allDropDownLists = new List<DropDownList>();
            allDropDownLists.Add(ddlCoreAC);
            allDropDownLists.Add(ddlCoreDC);
            foreach (var ddl in allDropDownLists)
            {
                ddl.BorderColor = System.Drawing.Color.Empty;
                ddl.BorderStyle = BorderStyle.NotSet;
                ddl.BorderWidth = 0;
            }
        }
        private void ResetControlBordersAC()
        {
            // Reset only AC Section controls
            List<TextBox> acTextBoxes = new List<TextBox>
            {
                testdateAC,
                txtLengthAC,
                txtImmerseAC,
                txtTempAC,
                txtVoltageAC,
                txtTimeAC
            };

            foreach (var textBox in acTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty;
                textBox.BorderStyle = BorderStyle.NotSet;
                textBox.BorderWidth = 0;
            }

            ddlCoreAC.BorderColor = System.Drawing.Color.Empty;
            ddlCoreAC.BorderStyle = BorderStyle.NotSet;
            ddlCoreAC.BorderWidth = 0;
        }
        private void ResetControlBordersDC()
        {
            // Reset only DC Section controls
            List<TextBox> dcTextBoxes = new List<TextBox>
            {
                testdateDC,
                txtLengthDC,
                txtImmerseDC,
                txtTempDC,
                txtVoltageDC,
                txtTimeDC
            };

            foreach (var textBox in dcTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty;
                textBox.BorderStyle = BorderStyle.NotSet;
                textBox.BorderWidth = 0;
            }

            ddlCoreDC.BorderColor = System.Drawing.Color.Empty;
            ddlCoreDC.BorderStyle = BorderStyle.NotSet;
            ddlCoreDC.BorderWidth = 0;
        }
        private void PopulateCoreDropdown()
        {
            try
            {
                // Get Core data from database
                DataTable coreData = _cm.Get_Core();

                // Clear existing items
                ddlCoreAC.Items.Clear();
                ddlCoreDC.Items.Clear();

                // Check if we got data
                if (coreData != null && coreData.Rows.Count > 0)
                {
                    // Bind data to dropdown
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddlCoreAC.Items.Add(new ListItem(coreName, coreId));
                        ddlCoreDC.Items.Add(new ListItem(coreName, coreId));
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

        //PRE-FILLING DATA IF EXISTS FOR SAMPLEID
        protected void CheckAndPrefillForm()
        {
            try
            {
                if (Request.QueryString["s"] != null)
                {
                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int reportId = Convert.ToInt32(Request.QueryString["r"]);

                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;
                    _cm.ReportID = reportId.ToString();

                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        PrefillUpperSection(existingData);
                        PrefillVoltageTestAC(existingData);
                        PrefillVoltageTestDC(existingData);
                    }
                }
            }
            catch (Exception ex)
            {
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
            // Use ParameterID instead of ReportParamMapID
            SetLabelText(cableType, "14224");       // Type
            SetLabelText(refSpec, "14230");         // RefSpec
            SetLabelText(cableSize, "14225");       // Size
            SetLabelText(testMethodNo, "14231");    // Test Method No
            SetLabelText(lblvoltage, "14226");      // Voltage
            SetLabelText(jobOrderNo, "14232");      // Job Order No
            SetLabelText(worksheetno, "14233");     // Worksheet No
            SetLabelText(equipmentno, "14235");     // Equipment No

            // Textbox fields
            SetTextBoxText(roomno, "14227");        // Room No
            SetTextBoxText(roomhumidity, "14234");  // Room Humidity
            SetTextBoxText(roomtemp, "14229");      // Room Temperature
            SetTextBoxText(Date, "14228");          // Date

            // Helper to set Label controls
            void SetLabelText(Label label, string parameterId)
            {
                var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

            // Helper to set TextBox controls
            void SetTextBoxText(TextBox textBox, string parameterId)
            {
                var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }
        private void PrefillVoltageTestAC(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "14236");
                string[] core = GetParameterValue(data, "14237").Split(',');
                string[] length = GetParameterValue(data, "14238").Split(',');
                string[] immerse = GetParameterValue(data, "14239").Split(',');
                string[] temp = GetParameterValue(data, "14240").Split(',');
                string[] voltage = GetParameterValue(data, "14241").Split(',');
                string[] time = GetParameterValue(data, "14242").Split(',');
                string[] result = GetParameterValue(data, "14243").Split(',');
                string[] specified = GetParameterValue(data, "14244").Split(',');

                DataTable dt = CreateVoltageTestDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Immerse"] = i < immerse.Length ? immerse[i] : "";
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["Voltage"] = i < voltage.Length ? voltage[i] : "";
                    row["Time"] = i < time.Length ? time[i] : "";
                    row["Result"] = i < result.Length ? result[i] : "";
                    row["Specified"] = i < specified.Length ? specified[i] : "";

                    dt.Rows.Add(row);
                }

                ViewState["TestResultsAC"] = dt;
                gvVoltageTestAC.DataSource = dt;
                gvVoltageTestAC.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillACError",
                    $"alert('Error loading AC data: {ex.Message}');", true);
            }
        }
        private void PrefillVoltageTestDC(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "14245");
                string[] core = GetParameterValue(data, "14246").Split(',');
                string[] length = GetParameterValue(data, "14247").Split(',');
                string[] immerse = GetParameterValue(data, "14248").Split(',');
                string[] temp = GetParameterValue(data, "14249").Split(',');
                string[] voltage = GetParameterValue(data, "14250").Split(',');
                string[] time = GetParameterValue(data, "14251").Split(',');
                string[] result = GetParameterValue(data, "14252").Split(',');
                string[] specified = GetParameterValue(data, "14253").Split(',');

                DataTable dt = CreateVoltageTestDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Immerse"] = i < immerse.Length ? immerse[i] : "";
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["Voltage"] = i < voltage.Length ? voltage[i] : "";
                    row["Time"] = i < time.Length ? time[i] : "";
                    row["Result"] = i < result.Length ? result[i] : "";
                    row["Specified"] = i < specified.Length ? specified[i] : "";

                    dt.Rows.Add(row);
                }

                ViewState["TestResultsDC"] = dt;
                gvVoltageTestDC.DataSource = dt;
                gvVoltageTestDC.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillDCError",
                    $"alert('Error loading DC data: {ex.Message}');", true);
            }
        }
        private string GetParameterValue(DataTable data, string paramId)
        {
            var row = data.Select($"ParameterID = '{paramId}'").FirstOrDefault();
            return row != null ? row["Result"].ToString() : "";
        }
        private DataTable CreateVoltageTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Core", typeof(string));
            dt.Columns.Add("Length", typeof(string));
            dt.Columns.Add("Immerse", typeof(string));
            dt.Columns.Add("Temperature", typeof(string));
            dt.Columns.Add("Voltage", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Result", typeof(string));
            dt.Columns.Add("Specified", typeof(string));
            return dt;
        }

    }
}