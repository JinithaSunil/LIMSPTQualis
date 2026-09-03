using LIMS.DAL;
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
    public partial class Voltagefourhours : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();

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

                //standard
                txtDuration.Text = "4";
                txtRequirement.Text = "No Breakdown of Insulation shall occur";
               

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();
            }
        }

        //Buttons
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

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

            // Duration fields (should be numbers)
            if (string.IsNullOrEmpty(txtDur30.Text) || !double.TryParse(txtDur30.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur30);
            }
            if (string.IsNullOrEmpty(txtDur60.Text) || !double.TryParse(txtDur60.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur60);
            }
            if (string.IsNullOrEmpty(txtDur90.Text) || !double.TryParse(txtDur90.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur90);
            }
            if (string.IsNullOrEmpty(txtDur120.Text) || !double.TryParse(txtDur120.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur120);
            }
            if (string.IsNullOrEmpty(txtDur150.Text) || !double.TryParse(txtDur150.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur150);
            }
            if (string.IsNullOrEmpty(txtDur180.Text) || !double.TryParse(txtDur180.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur180);
            }
            if (string.IsNullOrEmpty(txtDur210.Text) || !double.TryParse(txtDur210.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur210);
            }
            if (string.IsNullOrEmpty(txtDur240.Text) || !double.TryParse(txtDur240.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur240);
            }

            // Current fields (should be numbers)
            if (string.IsNullOrEmpty(txtCurr30.Text) || !double.TryParse(txtCurr30.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr30);
            }
            if (string.IsNullOrEmpty(txtCurr60.Text) || !double.TryParse(txtCurr60.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr60);
            }
            if (string.IsNullOrEmpty(txtCurr90.Text) || !double.TryParse(txtCurr90.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr90);
            }
            if (string.IsNullOrEmpty(txtCurr120.Text) || !double.TryParse(txtCurr120.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr120);
            }
            if (string.IsNullOrEmpty(txtCurr150.Text) || !double.TryParse(txtCurr150.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr150);
            }
            if (string.IsNullOrEmpty(txtCurr180.Text) || !double.TryParse(txtCurr180.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr180);
            }
            if (string.IsNullOrEmpty(txtCurr210.Text) || !double.TryParse(txtCurr210.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr210);
            }
            if (string.IsNullOrEmpty(txtCurr240.Text) || !double.TryParse(txtCurr240.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr240);
            }

            // Requirement fields (just not empty)
            if (string.IsNullOrWhiteSpace(txtRequirement.Text))
            {
                errors.Add("Requirement must not be empty.");
                invalidControls.Add(txtRequirement);
            }
            if (string.IsNullOrWhiteSpace(txtrequirementDurcurrent.Text))
            {
                errors.Add("Requirement (Current/Duration) must not be empty.");
                invalidControls.Add(txtrequirementDurcurrent);
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
            // Duration fields (should be numbers)
            if (string.IsNullOrEmpty(txtDur30.Text) || !double.TryParse(txtDur30.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur30);
            }
            if (string.IsNullOrEmpty(txtDur60.Text) || !double.TryParse(txtDur60.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur60);
            }
            if (string.IsNullOrEmpty(txtDur90.Text) || !double.TryParse(txtDur90.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur90);
            }
            if (string.IsNullOrEmpty(txtDur120.Text) || !double.TryParse(txtDur120.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur120);
            }
            if (string.IsNullOrEmpty(txtDur150.Text) || !double.TryParse(txtDur150.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur150);
            }
            if (string.IsNullOrEmpty(txtDur180.Text) || !double.TryParse(txtDur180.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur180);
            }
            if (string.IsNullOrEmpty(txtDur210.Text) || !double.TryParse(txtDur210.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur210);
            }
            if (string.IsNullOrEmpty(txtDur240.Text) || !double.TryParse(txtDur240.Text, out _))
            {
                errors.Add("Duration must be a valid number.");
                invalidControls.Add(txtDur240);
            }

            // Current fields (should be numbers)
            if (string.IsNullOrEmpty(txtCurr30.Text) || !double.TryParse(txtCurr30.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr30);
            }
            if (string.IsNullOrEmpty(txtCurr60.Text) || !double.TryParse(txtCurr60.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr60);
            }
            if (string.IsNullOrEmpty(txtCurr90.Text) || !double.TryParse(txtCurr90.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr90);
            }
            if (string.IsNullOrEmpty(txtCurr120.Text) || !double.TryParse(txtCurr120.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr120);
            }
            if (string.IsNullOrEmpty(txtCurr150.Text) || !double.TryParse(txtCurr150.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr150);
            }
            if (string.IsNullOrEmpty(txtCurr180.Text) || !double.TryParse(txtCurr180.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr180);
            }
            if (string.IsNullOrEmpty(txtCurr210.Text) || !double.TryParse(txtCurr210.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr210);
            }
            if (string.IsNullOrEmpty(txtCurr240.Text) || !double.TryParse(txtCurr240.Text, out _))
            {
                errors.Add("Current must be a valid number.");
                invalidControls.Add(txtCurr240);
            }

            // Requirement fields (just not empty)
            if (string.IsNullOrWhiteSpace(txtRequirement.Text))
            {
                errors.Add("Requirement must not be empty.");
                invalidControls.Add(txtRequirement);
            }
            if (string.IsNullOrWhiteSpace(txtrequirementDurcurrent.Text))
            {
                errors.Add("Requirement (Current/Duration) must not be empty.");
                invalidControls.Add(txtrequirementDurcurrent);
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
                //Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();

                // Get sample information from query string
                if (Request.QueryString["s"] != null)
                {
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                }

                if (Request.QueryString["r"] != null)
                {
                    _cm.ReportID = Request.QueryString["r"];
                }

                // Get data for series and report ID
                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                    return;
                }

                // Prepare arrays for the AddTestResultGlobal method
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Use values from the form or if available, from stored values
                string roomNoValue = roomno.Text?.Trim() ?? string.Empty;
                string roomHumidityValue = roomhumidity.Text?.Trim() ?? string.Empty;
                string roomTempValue = roomtemp.Text?.Trim() ?? string.Empty;
                string dateValue = Date.Text?.Trim() ?? string.Empty;

                string cableTypeValue = cableType.Text?.Trim() ?? string.Empty;
                string refSpecValue = refSpec.Text?.Trim() ?? string.Empty;
                string cableSizeValue = cableSize.Text?.Trim() ?? string.Empty;
                string testMethodNoValue = testMethodNo.Text?.Trim() ?? string.Empty;
                string voltageValue = lblvoltage.Text?.Trim() ?? string.Empty;
                string jobOrderNoValue = jobOrderNo.Text?.Trim() ?? string.Empty;
                string worksheetNoValue = worksheetno.Text?.Trim() ?? string.Empty;
                string equipmentNoValue = equipmentno.Text?.Trim() ?? string.Empty;

                //Characteristics
                string voltagetest = txtVoltagetest.Text?.Trim() ?? string.Empty;
                string durationhrs = txtDuration.Text?.Trim() ?? string.Empty;
                string frequency = txtFrequency.Text?.Trim() ?? string.Empty;

                // For requirement + duration field (comma-separated)
                string requirementCombined = $"{txtrequirementDurcurrent.Text?.Trim()},{txtRequirement.Text?.Trim()}";
          
                // Upper Section
                reportParamMapList.Add("5226"); resultList.Add(cableTypeValue);
                reportParamMapList.Add("5232"); resultList.Add(refSpecValue);
                reportParamMapList.Add("5227"); resultList.Add(cableSizeValue);
                reportParamMapList.Add("5233"); resultList.Add(testMethodNoValue);
                reportParamMapList.Add("5228"); resultList.Add(voltageValue);
                reportParamMapList.Add("5234"); resultList.Add(jobOrderNoValue);
                reportParamMapList.Add("5235"); resultList.Add(worksheetNoValue);
                reportParamMapList.Add("5237"); resultList.Add(equipmentNoValue);

                reportParamMapList.Add("5230"); resultList.Add(roomNoValue);
                reportParamMapList.Add("5236"); resultList.Add(roomHumidityValue);
                reportParamMapList.Add("5231"); resultList.Add(roomTempValue);
                reportParamMapList.Add("5229"); resultList.Add(dateValue);

                // Characteristics
                reportParamMapList.Add("6212"); resultList.Add(voltagetest);
                reportParamMapList.Add("6213"); resultList.Add(durationhrs);
                reportParamMapList.Add("5225"); resultList.Add(frequency);

                // Requirement fields (combined)
                reportParamMapList.Add("6214"); resultList.Add(requirementCombined);

                // Duration, Current, Measured lists (combine 8 values into CSV strings)
                reportParamMapList.Add("5189"); resultList.Add(string.Join(",", new[]
                {
                        txtDur30.Text, txtDur60.Text, txtDur90.Text, txtDur120.Text,
                        txtDur150.Text, txtDur180.Text, txtDur210.Text, txtDur240.Text
                    }));

                reportParamMapList.Add("5190"); resultList.Add(string.Join(",", new[]
                {
                        txtCurr30.Text, txtCurr60.Text, txtCurr90.Text, txtCurr120.Text,
                        txtCurr150.Text, txtCurr180.Text, txtCurr210.Text, txtCurr240.Text
                }));

                reportParamMapList.Add("5191"); resultList.Add(string.Join(",", new[]
                {
                        ddlMeasured30.SelectedItem.Text, ddlMeasured60.SelectedItem.Text, ddlMeasured90.SelectedItem.Text,
                        ddlMeasured120.SelectedItem.Text, ddlMeasured150.SelectedItem.Text, ddlMeasured180.SelectedItem.Text,
                        ddlMeasured210.SelectedItem.Text, ddlMeasured240.SelectedItem.Text
                }));

                // Convert lists to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                // Log data being sent to database (for debugging)
                System.Diagnostics.Debug.WriteLine($"Sending {reportParamMap.Length} values to database");
                for (int i = 0; i < reportParamMap.Length; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter: {reportParamMap[i]}, Value: {result[i]}");
                }

                // Call the database save method
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    // Update sample status
                    _cm.sampleStatusID = 27; // Test Result Submitted
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // Show success message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results saved and report generated successfully!');", true);

                    // Redirect to report page
                    Response.Redirect($"GenerateTestRecord_Voltagefourhours.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results. No records were saved to the database.');", true);
                }
            }
            catch (Exception ex)
            {
                // Log error details
                System.Diagnostics.Debug.WriteLine($"Error in Submitform: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                // Show user-friendly error message
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('An error occurred while saving data: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        //Others
        private void ClearForm()
        {
            // Main Section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";

            // Characterictics Values
            txtVoltagetest.Text = "";
            txtDuration.Text = "";
            txtFrequency.Text = "";

            // Duration TextBoxes
            txtDur30.Text = "";
            txtDur60.Text = "";
            txtDur90.Text = "";
            txtDur120.Text = "";
            txtDur150.Text = "";
            txtDur180.Text = "";
            txtDur210.Text = "";
            txtDur240.Text = "";

            // Current TextBoxes
            txtCurr30.Text = "";
            txtCurr60.Text = "";
            txtCurr90.Text = "";
            txtCurr120.Text = "";
            txtCurr150.Text = "";
            txtCurr180.Text = "";
            txtCurr210.Text = "";
            txtCurr240.Text = "";

            // Measured DropDownLists
            ddlMeasured30.SelectedIndex = 0;
            ddlMeasured60.SelectedIndex = 0;
            ddlMeasured90.SelectedIndex = 0;
            ddlMeasured120.SelectedIndex = 0;
            ddlMeasured150.SelectedIndex = 0;
            ddlMeasured180.SelectedIndex = 0;
            ddlMeasured210.SelectedIndex = 0;
            ddlMeasured240.SelectedIndex = 0;

            // Requirements TextBoxes
            txtrequirementDurcurrent.Text = "";
            txtRequirement.Text = "";

        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);
            allTextBoxes.Add(txtVoltagetest);
            allTextBoxes.Add(txtDuration);
            allTextBoxes.Add(txtFrequency);

            // Add Duration and Current textboxes
            allTextBoxes.Add(txtDur30);
            allTextBoxes.Add(txtDur60);
            allTextBoxes.Add(txtDur90);
            allTextBoxes.Add(txtDur120);
            allTextBoxes.Add(txtDur150);
            allTextBoxes.Add(txtDur180);
            allTextBoxes.Add(txtDur210);
            allTextBoxes.Add(txtDur240);

            allTextBoxes.Add(txtCurr30);
            allTextBoxes.Add(txtCurr60);
            allTextBoxes.Add(txtCurr90);
            allTextBoxes.Add(txtCurr120);
            allTextBoxes.Add(txtCurr150);
            allTextBoxes.Add(txtCurr180);
            allTextBoxes.Add(txtCurr210);
            allTextBoxes.Add(txtCurr240);

            // Requirements boxes
            allTextBoxes.Add(txtrequirementDurcurrent);
            allTextBoxes.Add(txtRequirement);


            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
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
                        PrefillValues(existingData);

                        //Prefill Characteristics Details
                        PrefillCharacteristicsDetails(existingData);

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
            SetLabelText(cableType, "5226");
            SetLabelText(refSpec, "5232");
            SetLabelText(cableSize, "5227");
            SetLabelText(testMethodNo, "5233");
            SetLabelText(lblvoltage, "5228");
            SetLabelText(jobOrderNo, "5234");
            SetLabelText(worksheetno, "5235");
            SetLabelText(equipmentno, "5237");

            // Textbox fields
            SetTextBoxText(roomno, "5230");
            SetTextBoxText(roomhumidity, "5236");
            SetTextBoxText(roomtemp, "5231");
            SetTextBoxText(Date, "5229");

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
        private void PrefillCharacteristicsDetails(DataTable existingData)
        {
            
            SetTextBoxText(txtVoltagetest, "6212");
            SetTextBoxText(txtDuration, "6213");
            SetTextBoxText(txtFrequency, "5225");

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
        private void PrefillValues(DataTable existingData)
        {
            PrefillMultipleValues(existingData, "5189",
                new Control[] { txtDur30, txtDur60, txtDur90, txtDur120, txtDur150, txtDur180, txtDur210, txtDur240 });

            PrefillMultipleValues(existingData, "5190",
                new Control[] { txtCurr30, txtCurr60, txtCurr90, txtCurr120, txtCurr150, txtCurr180, txtCurr210, txtCurr240 });

            PrefillMultipleValues(existingData, "5191",
                new Control[] { ddlMeasured30, ddlMeasured60, ddlMeasured90, ddlMeasured120, ddlMeasured150, ddlMeasured180, ddlMeasured210, ddlMeasured240 });


            //requirement
            SetTwoTextBoxes(existingData, "6214", txtrequirementDurcurrent, txtRequirement);

            void SetTwoTextBoxes(DataTable table, string paramId, TextBox box1, TextBox box2)
            {
                var row = table.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    var parts = row["Result"].ToString().Split(',');
                    if (parts.Length > 0) box1.Text = parts[0].Trim();
                    if (parts.Length > 1) box2.Text = parts[1].Trim();
                }
            }
        }
        private void PrefillMultipleValues(DataTable table, string parameterId, Control[] controls)
        {
            var row = table.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
            if (row == null) return;

            var values = row["Result"].ToString().Split(',');

            for (int i = 0; i < controls.Length && i < values.Length; i++)
            {
                if (controls[i] is TextBox textBox)
                {
                    textBox.Text = values[i];
                }
                else if (controls[i] is DropDownList ddl)
                {
                    var item = ddl.Items.FindByValue(values[i]);
                    if (item != null)
                    {
                        ddl.ClearSelection();
                        item.Selected = true;
                    }
                }
            }
        }



    }
}