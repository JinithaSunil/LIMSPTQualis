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
    public partial class DimensionCFCC : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (_cm == null) _cm = new DAL.CM();


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
                    cableType.Text = dt1.Rows[0]["SampleType"].ToString();
                    voltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();


                    // hardcoded fields
                    roomno.Text = "Mechanical";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/12/PH/16,QE/16/PH/03,QE/15/PH/43";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                CheckAndPrefillForm();
            }
        }

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

                    DataTable existingData = _cm.GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {

                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;


                        PrefillUpperSection(existingData);



                        PrefillTestResultsGrid(existingData);

                    }
                }
            }
            catch (Exception ex)
            {

                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            SetTextBoxText(cableType, "12859");       // Cable Type (Type)
            SetTextBoxText(refSpec, "12865");         // Reference Specification (RefSpec)
            SetTextBoxText(cableSize, "12860");       // Cable Size (Size)
            SetTextBoxText(testMethodNo, "12866");    // Test Method No (TestMethodNo)
            SetTextBoxText(voltage, "12861");         // Voltage (Voltage)
            SetTextBoxText(jobOrderNo, "12867");      // Job Order No (JobOrderNo)
            SetTextBoxText(worksheetno, "12868");     // Worksheet No (WorkSheetNo)
            SetTextBoxText(roomno, "12863");          // Room No (RoomNo)
            SetTextBoxText(roomhumidity, "12869");    // Room Humidity (RoomHumidity)
            SetTextBoxText(roomtemp, "12864");        // Room Temperature (RoomTemp)
            SetTextBoxText(Date, "12862");            // Date (Date)
            SetTextBoxText(equipmentno, "12870");     // Equipment No (EquipmentNo)

            void SetTextBoxText(TextBox textBox, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }

        private void PrefillTestResultsGrid(DataTable dt)
        {
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox txtInput1, TextBox txtInput2, TextBox txtInput3, TextBox txtInput4, TextBox txtInput5, TextBox txtInput6)>
            {
                { "12580", (ddlUnit1, txtSpecified1, txtValueTest1, txtValueTest2, txtValueTest3, txtValueTest4, txtValueTest5, txtValueTest6) } // Thickness of galvanic
            };

            var avgMapmin = new Dictionary<string, TextBox>
            {
                { "12581", txtValueavgmin }, // Thickness of galvanic min
            };

            var avgMapmax = new Dictionary<string, TextBox>
            {
                { "12582", txtValueavgmax }, // Thickness of galvanic max
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (inputMap.ContainsKey(paramId))
                {
                    var controls = inputMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length >= 3)
                    {
                        controls.ddlUnit.SelectedValue = parts[0];
                        controls.txtSpecified.Text = parts[1];

                        string[] inputs = parts[2].Split(',');
                        if (inputs.Length == 6)
                        {
                            controls.txtInput1.Text = inputs[0];
                            controls.txtInput2.Text = inputs[1];
                            controls.txtInput3.Text = inputs[2];
                            controls.txtInput4.Text = inputs[3];
                            controls.txtInput5.Text = inputs[4];
                            controls.txtInput6.Text = inputs[5];
                        }
                    }
                }
                else if (avgMapmin.ContainsKey(paramId))
                {
                    avgMapmin[paramId].Text = rawValue;
                }
                else if (avgMapmax.ContainsKey(paramId))
                {
                    avgMapmax[paramId].Text = rawValue;
                }
            }
        }

        protected void Input_TextChanged(object sender, EventArgs e)
        {
            // Calculate min, max, and average for the 6 value test inputs
            CalculateMinMaxAverage(txtValueTest1, txtValueTest2, txtValueTest3, txtValueTest4, txtValueTest5, txtValueTest6);
        }

        private void CalculateMinMaxAverage(TextBox input1, TextBox input2, TextBox input3, TextBox input4, TextBox input5, TextBox input6)
        {
            try
            {
                List<double> values = new List<double>();

                // Array of all input textboxes for easier processing
                TextBox[] inputs = { input1, input2, input3, input4, input5, input6 };

                // Process each input
                foreach (TextBox input in inputs)
                {
                    if (!string.IsNullOrEmpty(input.Text))
                    {
                        if (input.Text.Trim() == "-")
                        {
                            values.Add(0);
                        }
                        else if (double.TryParse(input.Text, out double value))
                        {
                            values.Add(value);
                        }
                    }
                }

                // Calculate and display results only if we have values
                if (values.Count > 0)
                {
                    double min = values.Min();
                    double max = values.Max();
                    double average = values.Average();

                    // Update the display textboxes
                    txtValueavgmin.Text = $"Min: {min:F2}";
                    txtValueavgmax.Text = $"Max: {max:F2}";

                    // If you have an average textbox, you can update it too
                    // txtValueAverage.Text = $"Avg: {average:F2}";
                }
                else
                {
                    // Clear the display if no valid values
                    txtValueavgmin.Text = "Min: ";
                    txtValueavgmax.Text = "Max: ";

                    // txtValueAverage.Text = "Avg: ";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating min/max/average: {ex.Message}");

                // Reset display on error
                txtValueavgmin.Text = "Min: Error";
                txtValueavgmax.Text = "Max: Error";
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool isSaved = Submitform();
            if (isSaved)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                    "alert('Test results and Report Generated successfully!');", true);
                ClearForm();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveFailed",
                    "alert('Test results were not saved due to an error.');", true);
            }
        }
        private bool Submitform()
        {
            try
            {
                if (_cm == null) _cm = new DAL.CM();

                if (Request.QueryString["s"] != null)
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                if (Request.QueryString["r"] != null)
                    _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                reportParamMapList.Add("12580"); // Thickness of galvanic
                resultList.Add($"{ddlUnit1.SelectedValue.Trim()}|{txtSpecified1.Text.Trim()}|{txtValueTest1.Text.Trim()},{txtValueTest2.Text.Trim()},{txtValueTest3.Text.Trim()},{txtValueTest4.Text.Trim()},{txtValueTest5.Text.Trim()},{txtValueTest6.Text.Trim()}");

                reportParamMapList.Add("12581"); // Thickness of galvanic min.
                resultList.Add($"{txtValueavgmin.Text.Trim()}");

                reportParamMapList.Add("12582"); // Thickness of galvanic max.
                resultList.Add($"{txtValueavgmax.Text.Trim()}");

                //Upper section
                reportParamMapList.Add("12859"); resultList.Add(cableType.Text); // Cable Type (Type)
                reportParamMapList.Add("12865"); resultList.Add(refSpec.Text); // Reference Specification (RefSpec)
                reportParamMapList.Add("12860"); resultList.Add(cableSize.Text); // Cable Size (Size)
                reportParamMapList.Add("12866"); resultList.Add(testMethodNo.Text); // Test Method No (TestMethodNo)
                reportParamMapList.Add("12861"); resultList.Add(voltage.Text); // Voltage (Voltage)
                reportParamMapList.Add("12867"); resultList.Add(jobOrderNo.Text); // Job Order No (JobOrderNo)
                reportParamMapList.Add("12868"); resultList.Add(worksheetno.Text); // Worksheet No (WorkSheetNo)
                reportParamMapList.Add("12863"); resultList.Add(roomno.Text); // Room No (RoomNo)
                reportParamMapList.Add("12869"); resultList.Add(roomhumidity.Text); // Room Humidity (RoomHumidity)
                reportParamMapList.Add("12864"); resultList.Add(roomtemp.Text); // Room Temperature (RoomTemp)
                reportParamMapList.Add("12862"); resultList.Add(Date.Text); // Date (Date)
                reportParamMapList.Add("12870"); resultList.Add(equipmentno.Text); // Equipment No (EquipmentNo)


                // Convert to arrays and submit
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess",
                        "alert('Data saved successfully!');", true);

                    Response.Redirect($"GenerateTestRecord_DimensionCFCC.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                        "alert('Failed to save data.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                    $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void ClearForm()
        {
            txtSpecified1.Text = "";
            txtValueTest1.Text = "";
            txtValueTest2.Text = "";
            txtValueTest3.Text = "";
            txtValueTest4.Text = "";
            txtValueTest5.Text = "";
            txtValueTest6.Text = "";
            txtValueavgmin.Text = "";
            txtValueavgmax.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";

        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
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
        protected void btnUpdate_Click(object sender, EventArgs e)
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
                    DeleteExistingRecords(sampleId, reportId);

                    bool success = Submitform();
                    if (!success)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError", "alert('Update failed.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError", "alert('Sample ID not found.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError", $"alert('Error: {ex.Message}');", true);
            }
        }
    }
}