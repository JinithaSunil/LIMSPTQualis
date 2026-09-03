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
    public partial class GalvanicProtectionBarrierLayer : System.Web.UI.Page
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
            SetTextBoxText(cableType, "12803");       // Cable Type
            SetTextBoxText(refSpec, "12809");         // Reference Specification
            SetTextBoxText(cableSize, "12804");       // Cable Size
            SetTextBoxText(testMethodNo, "12810");    // Test Method No
            SetTextBoxText(voltage, "12805");         // Voltage
            SetTextBoxText(jobOrderNo, "12811");      // Job Order No
            SetTextBoxText(worksheetno, "12812");     // Worksheet No
            SetTextBoxText(roomno, "12807");          // Room No
            SetTextBoxText(roomhumidity, "12813");    // Room Humidity
            SetTextBoxText(roomtemp, "12808");        // Room Temperature
            SetTextBoxText(Date, "12806");            // Date
            SetTextBoxText(equipmentno, "12814");     // Equipment No


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
    
                { "12815", (ddlUnit1, txtSpecified1, txtValueTest1, txtValueTest2, txtValueTest3, txtValueTest4, txtValueTest5, txtValueTest6) }
            };

            var avgMap = new Dictionary<string, TextBox>
            {
                { "12816", txtValueavg },
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
                else if (avgMap.ContainsKey(paramId))
                {
                    avgMap[paramId].Text = rawValue;
                }
            }
        }

        protected void Input_TextChanged(object sender, EventArgs e)
        {
            // Calculate average for the 6 value test inputs
            CalculateAverage(txtValueTest1, txtValueTest2, txtValueTest3, txtValueTest4, txtValueTest5, txtValueTest6, txtValueavg);
        }

        private void CalculateAverage(TextBox input1, TextBox input2, TextBox input3, TextBox input4, TextBox input5, TextBox input6, TextBox avgTextBox)
        {
            try
            {
                double value1 = 0, value2 = 0, value3 = 0, value4 = 0, value5 = 0, value6 = 0;
                int count = 0;
                double sum = 0;

                // Check and parse first input (treat "-" as 0)
                if (!string.IsNullOrEmpty(input1.Text))
                {
                    if (input1.Text.Trim() == "-")
                    {
                        value1 = 0;
                        sum += value1;
                        count++;
                    }
                    else if (double.TryParse(input1.Text, out value1))
                    {
                        sum += value1;
                        count++;
                    }
                }

                // Check and parse second input
                if (!string.IsNullOrEmpty(input2.Text))
                {
                    if (input2.Text.Trim() == "-")
                    {
                        value2 = 0;
                        sum += value2;
                        count++;
                    }
                    else if (double.TryParse(input2.Text, out value2))
                    {
                        sum += value2;
                        count++;
                    }
                }

                // Check and parse third input
                if (!string.IsNullOrEmpty(input3.Text))
                {
                    if (input3.Text.Trim() == "-")
                    {
                        value3 = 0;
                        sum += value3;
                        count++;
                    }
                    else if (double.TryParse(input3.Text, out value3))
                    {
                        sum += value3;
                        count++;
                    }
                }

                // Check and parse fourth input
                if (!string.IsNullOrEmpty(input4.Text))
                {
                    if (input4.Text.Trim() == "-")
                    {
                        value4 = 0;
                        sum += value4;
                        count++;
                    }
                    else if (double.TryParse(input4.Text, out value4))
                    {
                        sum += value4;
                        count++;
                    }
                }

                // Check and parse fifth input
                if (!string.IsNullOrEmpty(input5.Text))
                {
                    if (input5.Text.Trim() == "-")
                    {
                        value5 = 0;
                        sum += value5;
                        count++;
                    }
                    else if (double.TryParse(input5.Text, out value5))
                    {
                        sum += value5;
                        count++;
                    }
                }

                // Check and parse sixth input
                if (!string.IsNullOrEmpty(input6.Text))
                {
                    if (input6.Text.Trim() == "-")
                    {
                        value6 = 0;
                        sum += value6;
                        count++;
                    }
                    else if (double.TryParse(input6.Text, out value6))
                    {
                        sum += value6;
                        count++;
                    }
                }

                // Calculate average if we have valid inputs
                if (count > 0)
                {
                    double average = sum / count;
                    avgTextBox.Text = average.ToString("F2"); // Format to 2 decimal places
                }
                else
                {
                    avgTextBox.Text = "";
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                avgTextBox.Text = "Error";
                System.Diagnostics.Debug.WriteLine($"Error calculating average: {ex.Message}");
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
                reportParamMapList.Add("12815"); // Thickness of coated
                resultList.Add($"{ddlUnit1.SelectedValue.Trim()}|{txtSpecified1.Text.Trim()}|{txtValueTest1.Text.Trim()},{txtValueTest2.Text.Trim()},{txtValueTest3.Text.Trim()},{txtValueTest4.Text.Trim()},{txtValueTest5.Text.Trim()},{txtValueTest6.Text.Trim()}");
                reportParamMapList.Add("12816"); // Thickness of coated Avg.
                resultList.Add($"{txtValueavg.Text.Trim()}");


                //Upper section
                reportParamMapList.Add("12803"); resultList.Add(cableType.Text); // Cable Type (Type)
                reportParamMapList.Add("12809"); resultList.Add(refSpec.Text); // Reference Specification (RefSpec)
                reportParamMapList.Add("12804"); resultList.Add(cableSize.Text); // Cable Size (Size)
                reportParamMapList.Add("12810"); resultList.Add(testMethodNo.Text); // Test Method No (TestMethodNo)
                reportParamMapList.Add("12805"); resultList.Add(voltage.Text); // Voltage (Voltage)
                reportParamMapList.Add("12811"); resultList.Add(jobOrderNo.Text); // Job Order No (JobOrderNo)
                reportParamMapList.Add("12812"); resultList.Add(worksheetno.Text); // Worksheet No (WorkSheetNo)
                reportParamMapList.Add("12807"); resultList.Add(roomno.Text); // Room No (RoomNo)
                reportParamMapList.Add("12813"); resultList.Add(roomhumidity.Text); // Room Humidity (RoomHumidity)
                reportParamMapList.Add("12808"); resultList.Add(roomtemp.Text); // Room Temperature (RoomTemp)
                reportParamMapList.Add("12806"); resultList.Add(Date.Text); // Date (Date)
                reportParamMapList.Add("12814"); resultList.Add(equipmentno.Text); // Equipment No (EquipmentNo)


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

                    Response.Redirect($"GenerateTestRecord_GalvanicProtectionBarrierLayer.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            txtValueavg.Text = "";

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