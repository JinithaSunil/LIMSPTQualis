using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class TestRecordHeatStressAFCC : System.Web.UI.Page
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

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();
                BindUOM();
                BindSpec(_cm.ReportID);
            }

        }
        protected void BindUOM()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("UOMName"));
            DataRow dr = dt.NewRow(); dr["UOMName"] = "meter"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "mm"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "mm²"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "°C"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "Mpa (kN)"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "Hours"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "-"; dt.Rows.Add(dr);
            dr = dt.NewRow(); dr["UOMName"] = "Mpa (kN)"; dt.Rows.Add(dr);

            ddlUOM1.DataSource = dt; ddlUOM1.DataTextField = "UOMName"; ddlUOM1.DataValueField = "UOMName"; ddlUOM1.DataBind();
            ddlUOM2.DataSource = dt; ddlUOM2.DataTextField = "UOMName"; ddlUOM2.DataValueField = "UOMName"; ddlUOM2.DataBind();
            ddlUOM3.DataSource = dt; ddlUOM3.DataTextField = "UOMName"; ddlUOM3.DataValueField = "UOMName"; ddlUOM3.DataBind();
            ddlUOM4.DataSource = dt; ddlUOM4.DataTextField = "UOMName"; ddlUOM4.DataValueField = "UOMName"; ddlUOM4.DataBind();
            ddlUOM5.DataSource = dt; ddlUOM5.DataTextField = "UOMName"; ddlUOM5.DataValueField = "UOMName"; ddlUOM5.DataBind();
            ddlUOM6.DataSource = dt; ddlUOM6.DataTextField = "UOMName"; ddlUOM6.DataValueField = "UOMName"; ddlUOM6.DataBind();
            ddlUOM7.DataSource = dt; ddlUOM7.DataTextField = "UOMName"; ddlUOM7.DataValueField = "UOMName"; ddlUOM7.DataBind();
            ddlUOM8.DataSource = dt; ddlUOM8.DataTextField = "UOMName"; ddlUOM8.DataValueField = "UOMName"; ddlUOM8.DataBind();

        }
        //Buttons

        protected void BindSpec(string ReportID)
        {
            txtSpecified1.Text = _cm.GetSpec(ReportID, 12259);
            txtSpecified2.Text = _cm.GetSpec(ReportID, 12260);
            txtSpecified3.Text = _cm.GetSpec(ReportID, 12261);
            txtSpecified4.Text = _cm.GetSpec(ReportID, 12262);
            txtSpecified5.Text = _cm.GetSpec(ReportID, 12263);
            txtSpecified6.Text = _cm.GetSpec(ReportID, 12264);
            txtSpecified7.Text = _cm.GetSpec(ReportID, 12265);
            txtSpecified8.Text = _cm.GetSpec(ReportID, 12266);
        }
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
                    //   return;
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

                // Upper Section
                reportParamMapList.Add("12245"); resultList.Add(cableTypeValue);
                reportParamMapList.Add("12251"); resultList.Add(refSpecValue);
                reportParamMapList.Add("12246"); resultList.Add(cableSizeValue);
                reportParamMapList.Add("12252"); resultList.Add(testMethodNoValue);
                reportParamMapList.Add("12247"); resultList.Add(voltageValue);
                reportParamMapList.Add("12253"); resultList.Add(jobOrderNoValue);
                reportParamMapList.Add("12254"); resultList.Add(worksheetNoValue);
                reportParamMapList.Add("12256"); resultList.Add(equipmentNoValue);

                reportParamMapList.Add("12249"); resultList.Add(roomNoValue);
                reportParamMapList.Add("12255"); resultList.Add(roomHumidityValue);
                reportParamMapList.Add("12250"); resultList.Add(roomTempValue);
                reportParamMapList.Add("12248"); resultList.Add(dateValue);


                // Extract grid data for comma-separated storage
                string LengthOfSample = txtValue1.Text,
                    ActualDiameterOfCore = txtValue2.Text,
                    ActualCrosssectionalArea = txtValue3.Text,
                    TemperatureofTgTest = txtValue4.Text,
                    TensionOf25pcofMinUltimateTensileStrength = txtValue5.Text,
                    HeatStressdurationfor25pcofRTS = txtValue6.Text,
                    Visualafterexposure = txtValue7.Text, Tensilemin95OfRTS = txtValue8.Text;

                // Add grid data to parameter lists with the specified IDs
                reportParamMapList.Add("12259"); resultList.Add(ddlUOM1.Text + "|" + txtSpecified1.Text + "|" + LengthOfSample.ToString());
                reportParamMapList.Add("12260"); resultList.Add(ddlUOM2.Text + "|" + txtSpecified2.Text + "|" + ActualDiameterOfCore.ToString());
                reportParamMapList.Add("12261"); resultList.Add(ddlUOM3.Text + "|" + txtSpecified3.Text + "|" + ActualCrosssectionalArea.ToString());
                reportParamMapList.Add("12262"); resultList.Add(ddlUOM4.Text + "|" + txtSpecified4.Text + "|" + TemperatureofTgTest.ToString());
                reportParamMapList.Add("12263"); resultList.Add(ddlUOM5.Text + "|" + txtSpecified5.Text + "|" + TensionOf25pcofMinUltimateTensileStrength.ToString());
                reportParamMapList.Add("12264"); resultList.Add(ddlUOM6.Text + "|" + txtSpecified6.Text + "|" + HeatStressdurationfor25pcofRTS.ToString());
                reportParamMapList.Add("12265"); resultList.Add(ddlUOM7.Text + "|" + txtSpecified7.Text + "|" + Visualafterexposure.ToString());
                reportParamMapList.Add("12266"); resultList.Add(ddlUOM8.Text + "|" + txtSpecified8.Text + "|" + Tensilemin95OfRTS.ToString());

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
                    Response.Redirect($"GenerateTestRecord_HeatStressAFCC.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

        //Other
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);


            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        private void ClearInputFields()
        {

        }
        private void ShowMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{message}');", true);
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
                        PrefillGridView(existingData);

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
            SetLabelText(cableType, "5212");
            SetLabelText(refSpec, "5218");
            SetLabelText(cableSize, "5213");
            SetLabelText(testMethodNo, "5219");
            SetLabelText(lblvoltage, "5214");
            SetLabelText(jobOrderNo, "5220");
            SetLabelText(worksheetno, "5221");
            SetLabelText(equipmentno, "5223");

            // Textbox fields
            SetTextBoxText(roomno, "5216");
            SetTextBoxText(roomhumidity, "5222");
            SetTextBoxText(roomtemp, "5217");
            SetTextBoxText(Date, "5215");

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
        private void PrefillGridView(DataTable existingData)
        {
            try
            {
                // Look for the grid-related parameters in the existingData
                DataRow corePhaseRow = existingData.Select("ParameterID = '6243'").FirstOrDefault();
                DataRow durationRow = existingData.Select("ParameterID = '5195'").FirstOrDefault();
                DataRow currentRow = existingData.Select("ParameterID = '5196'").FirstOrDefault();
                DataRow measuredRow = existingData.Select("ParameterID = '5197'").FirstOrDefault();
                DataRow requirementRow = existingData.Select("ParameterID = '6244'").FirstOrDefault();

                // Create DataTable to hold grid data
                DataTable gridData = new DataTable();
                gridData.Columns.Add("ItemTest", typeof(string));
                gridData.Columns.Add("CorePhase", typeof(string));
                gridData.Columns.Add("Duration", typeof(string));
                gridData.Columns.Add("Current", typeof(string));
                gridData.Columns.Add("Measured", typeof(string));

                // Add test data rows
                if (corePhaseRow != null && durationRow != null && currentRow != null && measuredRow != null)
                {
                    string[] phases = corePhaseRow["Result"].ToString().Split(',');
                    string[] durations = durationRow["Result"].ToString().Split(',');
                    string[] currents = currentRow["Result"].ToString().Split(',');
                    string[] measureds = measuredRow["Result"].ToString().Split(',');

                    // Find the maximum length of any of these arrays
                    int maxLength = Math.Max(phases.Length,
                        Math.Max(durations.Length,
                        Math.Max(currents.Length, measureds.Length)));

                    // Add each row to the grid
                    for (int i = 0; i < maxLength; i++)
                    {
                        string phase = i < phases.Length ? phases[i].Trim() : "";
                        string duration = i < durations.Length ? durations[i].Trim() : "";
                        string current = i < currents.Length ? currents[i].Trim() : "";
                        string measured = i < measureds.Length ? measureds[i].Trim() : "";

                        // Skip empty rows
                        if (string.IsNullOrWhiteSpace(phase) && string.IsNullOrWhiteSpace(duration) &&
                            string.IsNullOrWhiteSpace(current) && string.IsNullOrWhiteSpace(measured))
                        {
                            continue;
                        }

                        gridData.Rows.Add("Voltage to be applied to core to core and Earth connected to metal screen", phase, duration, current, measured);
                    }
                }


            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridFillError",
                    $"alert('Error prefilling grid data: {ex.Message}');", true);
            }
        }

    }
}