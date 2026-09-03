using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS;
using System.Diagnostics;

namespace LIMS
{
	public partial class pHAndConductivity : System.Web.UI.Page
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
                      else
                        {

                        DataTable dtGetSpecified = _cm.Get_Specified(reportId);
                        if (dtGetSpecified != null && dtGetSpecified.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtGetSpecified.Rows)
                            {
                                if (!row.Table.Columns.Contains("ReportParameter") || !row.Table.Columns.Contains("ReportParaMapId"))
                                {
                                    Debug.WriteLine("Required columns not found in data table");
                                    break;
                                }

                                int reportParaMapId = Convert.ToInt32(row["ReportParaMapId"]);

                                if (reportParaMapId == 16142)
                                {
                                    txtweight_sampleSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16143)
                                {
                                    txtAirSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16144)
                                {
                                    txtBurnTemperatureSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16145)
                                {
                                    txtBurnTimeSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16146)
                                {
                                    pHSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16147)
                                {
                                    txtConductivitySpecified.Text = row["Specification"].ToString();
                                }
                            }

                        
                            Debug.WriteLine("Available Parameters:");
                            foreach (DataRow row in dtGetSpecified.Rows)
                            {
                                if (row.Table.Columns.Contains("ReportParameter"))
                                {
                                    Debug.WriteLine(row["ReportParameter"].ToString());
                                }
                            }
                        }

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
            SetTextBoxText(cableType, "16148");        // Type  
            SetTextBoxText(refSpec, "16154");          // Reference Specification  
            SetTextBoxText(cableSize, "16149");        // Size  
            SetTextBoxText(testMethodNo, "16155");     // Test Method No  
            SetTextBoxText(voltage, "16150");          // Voltage  
            SetTextBoxText(jobOrderNo, "16156");       // Job Order No  
            SetTextBoxText(worksheetno, "16157");      // Worksheet No  
            SetTextBoxText(roomno, "16152");           // Room No  
            SetTextBoxText(roomhumidity, "16158");     // Room Humidity  
            SetTextBoxText(roomtemp, "16153");         // Room Temperature  
            SetTextBoxText(Date, "16151");              // Date  
            SetTextBoxText(equipmentno, "16159");      // Equipment No  

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

        private void PrefillTestResultsGrid(DataTable dt)
        {
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox txtInput1, TextBox txtInput2)>
{
    { "16142", (ddlweight_sample1, txtweight_sampleSpecified, txtweight_sampleinput1, txtweight_sampleinput2) }, // Weightofsample
    { "16143", (ddlair2, txtAirSpecified, txtAirInput1, txtAirInput2) }, // AirDestilasi
    { "16144", (ddlBurnTemperatureUnit, txtBurnTemperatureSpecified, txtBurnTemperatureInput1, txtBurnTemperatureInput2) }, // BurnTemperature
    { "16145", (ddlBurnTimeUnit, txtBurnTimeSpecified, txtBurnTimeInput1, txtBurnTimeInput2) }, // Burning time
    { "16146", (ddlpH1, pHSpecified, pHinput1, pHinput2) }, // pH
    { "16147", (ddlConductivityUnit, txtConductivitySpecified, txtConductivityInput1, txtConductivityInput2) } // Conductivity
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
                        if (inputs.Length == 2)
                        {
                            controls.txtInput1.Text = inputs[0];
                            controls.txtInput2.Text = inputs[1];
                        }
                    }
                }
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
                reportParamMapList.Add("16142"); // Weightofsample
                resultList.Add($"{ddlweight_sample1.SelectedValue.Trim()}|{txtweight_sampleSpecified.Text.Trim()}|{txtweight_sampleinput1.Text.Trim()},{txtweight_sampleinput2.Text.Trim()}");

                reportParamMapList.Add("16143"); // AirDestilasi
                resultList.Add($"{ddlair2.SelectedValue.Trim()}|{txtAirSpecified.Text.Trim()}|{txtAirInput1.Text.Trim()},{txtAirInput2.Text.Trim()}");

                reportParamMapList.Add("16144"); // BurnTemperature
                resultList.Add($"{ddlBurnTemperatureUnit.SelectedValue.Trim()}|{txtBurnTemperatureSpecified.Text.Trim()}|{txtBurnTemperatureInput1.Text.Trim()},{txtBurnTemperatureInput2.Text.Trim()}");

                reportParamMapList.Add("16145"); // Burning time
                resultList.Add($"{ddlBurnTimeUnit.SelectedValue.Trim()}|{txtBurnTimeSpecified.Text.Trim()}|{txtBurnTimeInput1.Text.Trim()},{txtBurnTimeInput2.Text.Trim()}");

                reportParamMapList.Add("16146"); // pH
                resultList.Add($"{ddlpH1.SelectedValue.Trim()}|{pHSpecified.Text.Trim()}|{pHinput1.Text.Trim()},{pHinput2.Text.Trim()}");

                reportParamMapList.Add("16147"); // Conductivity
                resultList.Add($"{ddlConductivityUnit.SelectedValue.Trim()}|{txtConductivitySpecified.Text.Trim()}|{txtConductivityInput1.Text.Trim()},{txtConductivityInput2.Text.Trim()}");

                reportParamMapList.Add("16148"); resultList.Add(cableType.Text); // Type
                reportParamMapList.Add("16149"); resultList.Add(cableSize.Text); // Size
                reportParamMapList.Add("16150"); resultList.Add(voltage.Text); // Voltage
                reportParamMapList.Add("16151"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("16152"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("16153"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("16154"); resultList.Add(refSpec.Text); // Reference Specification
                reportParamMapList.Add("16155"); resultList.Add(testMethodNo.Text); // Test Method No
                reportParamMapList.Add("16156"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("16157"); resultList.Add(worksheetno.Text); // Worksheet No
                reportParamMapList.Add("16158"); resultList.Add(roomhumidity.Text); // Room Humidity
                reportParamMapList.Add("16159"); resultList.Add(equipmentno.Text); // Equipment No


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

                    Response.Redirect($"GenerateTestReport_pHAndConducitivity.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void ClearForm()
        {
            // Sample Weight
            ddlweight_sample1.SelectedIndex = 0;
            txtweight_sampleSpecified.Text = "";
            txtweight_sampleinput1.Text = "";
            txtweight_sampleinput2.Text = "";

            // Air destilasi
            ddlair2.SelectedIndex = 0;
            txtAirSpecified.Text = "";
            txtAirInput1.Text = "";
            txtAirInput2.Text = "";

            // Burn Temperature
            ddlBurnTemperatureUnit.SelectedIndex = 0;
            txtBurnTemperatureSpecified.Text = "";
            txtBurnTemperatureInput1.Text = "";
            txtBurnTemperatureInput2.Text = "";

            // Burn Time
            ddlBurnTimeUnit.SelectedIndex = 0;
            txtBurnTimeSpecified.Text = "";
            txtBurnTimeInput1.Text = "";
            txtBurnTimeInput2.Text = "";

            // pH
            ddlpH1.SelectedIndex = 0;
            pHSpecified.Text = "";
            pHinput1.Text = "";
            pHinput2.Text = "";

            // Conductivity
            ddlConductivityUnit.SelectedIndex = 0;
            txtConductivitySpecified.Text = "";
            txtConductivityInput1.Text = "";
            txtConductivityInput2.Text = "";

          
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