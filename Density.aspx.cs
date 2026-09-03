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
	public partial class Density : System.Web.UI.Page
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


            SetTextBoxText(cableType, "12726");        // Cable Type  
            SetTextBoxText(refSpec, "12732");          // Reference Specification  
            SetTextBoxText(cableSize, "12727");        // Cable Size  
            SetTextBoxText(testMethodNo, "12733");     // Test Method No  
            SetTextBoxText(voltage, "12728");          // Voltage  
            SetTextBoxText(jobOrderNo, "12734");       // Job Order No  
            SetTextBoxText(worksheetno, "12735");      // Worksheet No  
            SetTextBoxText(roomno, "12730");           // Room No  
            SetTextBoxText(roomhumidity, "12736");     // Room Humidity  
            SetTextBoxText(roomtemp, "12731");         // Room Temperature  
            SetTextBoxText(Date, "12729");             // Date  
            SetTextBoxText(equipmentno, "12737");      // Equipment No  

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
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox txtInput1, TextBox txtInput2, TextBox txtInput3, TextBox txtInput4, TextBox txtInput5)>
    {
        { "12552", (ddlweight_sample1, weight_sampleSpecified, weight_sampleinput1, weight_sampleinput2, weight_sampleinput3, weight_sampleinput4, weight_sampleinput5) },
        { "12554", (ddlsolution2, solutionSpecified, solutioninput1, solutioninput2, solutioninput3, solutioninput4, solutioninput5) },
        { "12556", (ddlweight_sample_solution3, weight_sample_solutionSpecified, weight_sample_solutioninput1, weight_sample_solutioninput2, weight_sample_solutioninput3, weight_sample_solutioninput4, weight_sample_solutioninput5) },
        { "12558", (ddldensity4, densitySpecified, densityinput1, densityinput2, densityinput3, densityinput4, densityinput5) }
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
                        if (inputs.Length == 5)
                        {
                            controls.txtInput1.Text = inputs[0];
                            controls.txtInput2.Text = inputs[1];
                            controls.txtInput3.Text = inputs[2];
                            controls.txtInput4.Text = inputs[3];
                            controls.txtInput5.Text = inputs[4];
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

                reportParamMapList.Add("12552");
                resultList.Add($"{ddlweight_sample1.SelectedValue.Trim()}|{weight_sampleSpecified.Text.Trim()}|{weight_sampleinput1.Text.Trim()},{weight_sampleinput2.Text.Trim()},{weight_sampleinput3.Text.Trim()},{weight_sampleinput4.Text.Trim()},{weight_sampleinput5.Text.Trim()}");

              

             
                reportParamMapList.Add("12554");
                resultList.Add($"{ddlsolution2.SelectedValue.Trim()}|{solutionSpecified.Text.Trim()}|{solutioninput1.Text.Trim()},{solutioninput2.Text.Trim()},{solutioninput3.Text.Trim()},{solutioninput4.Text.Trim()},{solutioninput5.Text.Trim()}");

           

                reportParamMapList.Add("12556"); 
                resultList.Add($"{ddlweight_sample_solution3.SelectedValue.Trim()}|{weight_sample_solutionSpecified.Text.Trim()}|{weight_sample_solutioninput1.Text.Trim()},{weight_sample_solutioninput2.Text.Trim()},{weight_sample_solutioninput3.Text.Trim()},{weight_sample_solutioninput4.Text.Trim()},{weight_sample_solutioninput5.Text.Trim()}");

             
                reportParamMapList.Add("12558"); 
                resultList.Add($"{ddldensity4.SelectedValue.Trim()}|{densitySpecified.Text.Trim()}|{densityinput1.Text.Trim()},{densityinput2.Text.Trim()},{densityinput3.Text.Trim()},{densityinput4.Text.Trim()},{densityinput5.Text.Trim()}");


                reportParamMapList.Add("12726"); resultList.Add(cableType.Text); // Cable Type
                reportParamMapList.Add("12727"); resultList.Add(cableSize.Text); // Cable Size
                reportParamMapList.Add("12728"); resultList.Add(voltage.Text); // Voltage
                reportParamMapList.Add("12729"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("12730"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("12731"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("12732"); resultList.Add(refSpec.Text); // Reference Specification              
                reportParamMapList.Add("12733"); resultList.Add(testMethodNo.Text); // Test Method No               
                reportParamMapList.Add("12734"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("12735"); resultList.Add(worksheetno.Text); // Worksheet No            
                reportParamMapList.Add("12736"); resultList.Add(roomhumidity.Text); // Room Humidity             
                reportParamMapList.Add("12737"); resultList.Add(equipmentno.Text); // Equipment No


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

                    Response.Redirect($"GenerateTestRecord_Density.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // Weight of sample
            ddlweight_sample1.SelectedIndex = 0;
            weight_sampleSpecified.Text = "";
            weight_sampleinput1.Text = "";
            weight_sampleinput2.Text = "";
            weight_sampleinput3.Text = "";
            weight_sampleinput4.Text = "";
            weight_sampleinput5.Text = "";

            // Solution
            ddlsolution2.SelectedIndex = 0;
            solutionSpecified.Text = "";
            solutioninput1.Text = "";
            solutioninput2.Text = "";
            solutioninput3.Text = "";
            solutioninput4.Text = "";
            solutioninput5.Text = "";

            // Weight of sample in solution
            ddlweight_sample_solution3.SelectedIndex = 0;
            weight_sample_solutionSpecified.Text = "";
            weight_sample_solutioninput1.Text = "";
            weight_sample_solutioninput2.Text = "";
            weight_sample_solutioninput3.Text = "";
            weight_sample_solutioninput4.Text = "";
            weight_sample_solutioninput5.Text = "";

            // Density
            ddldensity4.SelectedIndex = 0;
            densitySpecified.Text = "";
            densityinput1.Text = "";
            densityinput2.Text = "";
            densityinput3.Text = "";
            densityinput4.Text = "";
            densityinput5.Text = "";
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