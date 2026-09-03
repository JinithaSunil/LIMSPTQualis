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
    public partial class BendingTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //upper section
                roomno.Text = "Mechanical";
                cableType.Text = "CIVA PELSIDSTAPE";
                refSpec.Text = "IEC 60502-2";
                cableSize.Text = "3 x 25 mm²";
                testMethodNo.Text = "IEC 60811.1.1";
                lblvoltage.Text = "3,6/6(7,2)kV";
                jobOrderNo.Text = "WO/2024120905";
                worksheetno.Text = "-";
                equipmentno.Text = "QE/12/PH/16,QE/16/PH/03,QE/15/PH/43";

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

            // Validate Last section
            if (string.IsNullOrEmpty(txtOuterDiameter.Text) || !double.TryParse(txtOuterDiameter.Text, out _))
            {
                errors.Add("Outer Diameter must be a valid number.");
                invalidControls.Add(txtOuterDiameter);
            }
            if (string.IsNullOrEmpty(txtConductorDiameter.Text) || !double.TryParse(txtConductorDiameter.Text, out _))
            {
                errors.Add("Conductor Diameter must be a valid number.");
                invalidControls.Add(txtConductorDiameter);
            }
            if (string.IsNullOrEmpty(txtBendingDiameter.Text) || !double.TryParse(txtBendingDiameter.Text, out _))
            {
                errors.Add("Bending Diameter must be a valid number.");
                invalidControls.Add(txtBendingDiameter);
            }
            if (string.IsNullOrEmpty(txtTestCylinder.Text) || !double.TryParse(txtTestCylinder.Text, out _))
            {
                errors.Add("Test Cylinder must be a valid number.");
                invalidControls.Add(txtTestCylinder);
            }


            // Validate Characteristics section
            if (string.IsNullOrEmpty(txtsinglecore1.Text))
            {
                errors.Add("Single-core Cable must be a valid entry.");
                invalidControls.Add(txtsinglecore1);
            }
            if (string.IsNullOrEmpty(txtsinglecore2.Text))
            {
                errors.Add("Single-core Cable must be a valid entry.");
                invalidControls.Add(txtsinglecore2);
            }
            if (string.IsNullOrEmpty(txtthreecore1.Text))
            {
                errors.Add("Three-core Cable must be a valid entry.");
                invalidControls.Add(txtthreecore1);
            }
            if (string.IsNullOrEmpty(txtthreecore2.Text))
            {
                errors.Add("Three-core Cable must be a valid entry.");
                invalidControls.Add(txtthreecore2);
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

            // Validate Last section
            if (string.IsNullOrEmpty(txtOuterDiameter.Text) || !double.TryParse(txtOuterDiameter.Text, out _))
            {
                errors.Add("Outer Diameter must be a valid number.");
                invalidControls.Add(txtOuterDiameter);
            }
            if (string.IsNullOrEmpty(txtConductorDiameter.Text) || !double.TryParse(txtConductorDiameter.Text, out _))
            {
                errors.Add("Conductor Diameter must be a valid number.");
                invalidControls.Add(txtConductorDiameter);
            }
            if (string.IsNullOrEmpty(txtBendingDiameter.Text) || !double.TryParse(txtBendingDiameter.Text, out _))
            {
                errors.Add("Bending Diameter must be a valid number.");
                invalidControls.Add(txtBendingDiameter);
            }
            if (string.IsNullOrEmpty(txtTestCylinder.Text) || !double.TryParse(txtTestCylinder.Text, out _))
            {
                errors.Add("Test Cylinder must be a valid number.");
                invalidControls.Add(txtTestCylinder);
            }


            // Validate Characteristics section
            if (string.IsNullOrEmpty(txtsinglecore1.Text))
            {
                errors.Add("Single-core Cable must be a valid entry.");
                invalidControls.Add(txtsinglecore1);
            }
            if (string.IsNullOrEmpty(txtsinglecore2.Text))
            {
                errors.Add("Single-core Cable must be a valid entry.");
                invalidControls.Add(txtsinglecore2);
            }
            if (string.IsNullOrEmpty(txtthreecore1.Text))
            {
                errors.Add("Three-core Cable must be a valid entry.");
                invalidControls.Add(txtthreecore1);
            }
            if (string.IsNullOrEmpty(txtthreecore2.Text))
            {
                errors.Add("Three-core Cable must be a valid entry.");
                invalidControls.Add(txtthreecore2);
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

                // Get single-core and three-core values directly from controls
                string ForSingleCoreCablesstr = string.Join(",", new[]
                {
                    txtsinglecore1.Text?.Trim() ?? string.Empty,
                    txtsinglecore2.Text?.Trim() ?? string.Empty
                });

                string ForThreeCoreCablesstr = string.Join(",", new[]
                {
                    txtthreecore1.Text?.Trim() ?? string.Empty,
                    txtthreecore2.Text?.Trim() ?? string.Empty
                });

                // Get diameter and result values
                string outerDiameterValue = txtOuterDiameter.Text?.Trim() ?? string.Empty;
                string conductorDiameterValue = txtConductorDiameter.Text?.Trim() ?? string.Empty;
                string bendingDiameterValue = txtBendingDiameter.Text?.Trim() ?? string.Empty;
                string testCylinderValue = txtTestCylinder.Text?.Trim() ?? string.Empty;
                string resultValue = ddlresult.SelectedItem.Text?.Trim() ?? string.Empty;

                // Add Standard values
                reportParamMapList.Add("4148"); resultList.Add(outerDiameterValue); // Outer Diameter
                reportParamMapList.Add("4149"); resultList.Add(conductorDiameterValue); // Conductor Diameter
                reportParamMapList.Add("4152"); resultList.Add(bendingDiameterValue); // Bending Diameter 
                reportParamMapList.Add("4151"); resultList.Add(testCylinderValue); // Test Cylinder
                reportParamMapList.Add("4150"); resultList.Add(resultValue); // Result

                // Add Upper section values (with null checks)
                reportParamMapList.Add("4153"); resultList.Add(cableTypeValue);
                reportParamMapList.Add("4159"); resultList.Add(refSpecValue);
                reportParamMapList.Add("4154"); resultList.Add(cableSizeValue);
                reportParamMapList.Add("4160"); resultList.Add(testMethodNoValue);
                reportParamMapList.Add("4155"); resultList.Add(voltageValue);
                reportParamMapList.Add("4161"); resultList.Add(jobOrderNoValue);
                reportParamMapList.Add("4162"); resultList.Add(worksheetNoValue);
                reportParamMapList.Add("4157"); resultList.Add(roomNoValue);  // Use the stored value
                reportParamMapList.Add("4163"); resultList.Add(roomHumidityValue);  // Use the stored value
                reportParamMapList.Add("4158"); resultList.Add(roomTempValue);  // Use the stored value
                reportParamMapList.Add("4156"); resultList.Add(dateValue);
                reportParamMapList.Add("4164"); resultList.Add(equipmentNoValue);

                // Add Strippability Min and Max values
                reportParamMapList.Add("4146"); resultList.Add(ForSingleCoreCablesstr); // ForSingleCoreCables
                reportParamMapList.Add("4147"); resultList.Add(ForThreeCoreCablesstr); // ForThreeCoreCables 

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
                    Response.Redirect($"GenerateTestRecord_BendingTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

            // Test Details
            txtOuterDiameter.Text = "";
            txtConductorDiameter.Text = "";
            txtBendingDiameter.Text = "";
            txtTestCylinder.Text = "";
            ddlresult.SelectedIndex = 0;


            // Characterictics Values
            txtsinglecore1.Text = "";
            txtsinglecore2.Text = "";
            txtthreecore1.Text = "";
            txtthreecore2.Text = "";


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
            allTextBoxes.Add(txtOuterDiameter);
            allTextBoxes.Add(txtConductorDiameter);
            allTextBoxes.Add(txtBendingDiameter);
            allTextBoxes.Add(txtTestCylinder);
            allTextBoxes.Add(txtsinglecore1);
            allTextBoxes.Add(txtsinglecore2);
            allTextBoxes.Add(txtthreecore1);
            allTextBoxes.Add(txtthreecore2);


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
            SetLabelText(cableType, "4153");
            SetLabelText(refSpec, "4159");
            SetLabelText(cableSize, "4154");
            SetLabelText(testMethodNo, "4160");
            SetLabelText(lblvoltage, "4155");
            SetLabelText(jobOrderNo, "4161");
            SetLabelText(worksheetno, "4162");
            SetLabelText(equipmentno, "4164");

            // Textbox fields
            SetTextBoxText(roomno, "4157");
            SetTextBoxText(roomhumidity, "4163");
            SetTextBoxText(roomtemp, "4158");
            SetTextBoxText(Date, "4156");

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
        private void PrefillValues(DataTable existingData)
        {
            // Prefill standard values
            SetTextBoxText(txtOuterDiameter, "4148");
            SetTextBoxText(txtConductorDiameter, "4149");
            SetTextBoxText(txtBendingDiameter, "4152");
            SetTextBoxText(txtTestCylinder, "4151");
            //SetTextBoxText(ddlresult.SelectedItem.Text, "3117");

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
            // Prefill dates and times
            SetTextBoxText(txtsinglecore1, txtsinglecore2, "4146");
            SetTextBoxText(txtthreecore1, txtthreecore2, "4147");

            // Helper method to set text in two textboxes
            void SetTextBoxText(TextBox textBox1, TextBox textBox2, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    string value = row["Result"].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        // Split the value by comma
                        string[] parts = value.Split(',');

                        if (parts.Length >= 2)
                        {
                            textBox1.Text = parts[0].Trim();
                            textBox2.Text = parts[1].Trim();
                        }
                        else if (parts.Length == 1)
                        {
                            textBox1.Text = parts[0].Trim();
                            textBox2.Text = string.Empty; // If only one value, leave the second textbox empty
                        }
                    }
                }
            }
        }

    }
}