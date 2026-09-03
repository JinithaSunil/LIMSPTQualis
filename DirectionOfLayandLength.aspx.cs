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
	public partial class DirectionOfLay_Length : System.Web.UI.Page
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



            SetTextBoxText(cableType, "12791");        // Cable Type  
            SetTextBoxText(cableSize, "12792");        // Cable Size  
            SetTextBoxText(voltage, "12793");          // Voltage  
            SetTextBoxText(Date, "12794");             // Date  
            SetTextBoxText(roomno, "12795");           // Room No  
            SetTextBoxText(roomtemp, "12796");         // Room Temperature  
            SetTextBoxText(refSpec, "12797");          // Reference Specification  
            SetTextBoxText(testMethodNo, "12798");     // Test Method No  
            SetTextBoxText(jobOrderNo, "12799");       // Job Order No  
            SetTextBoxText(worksheetno, "12800");      // Worksheet No  
            SetTextBoxText(roomhumidity, "12801");     // Room Humidity  
            SetTextBoxText(equipmentno, "12802");      // Equipment No  

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
            
            var parameterControls = new Dictionary<string, Action<string>>
    {
        // ===== Table 1 =====
        // Conductor Diameter - Test Results (6 values)
        { "12775", value => {
            string[] values = value.Split(',');
            if (values.Length >= 6) {
                txtCondDia1_1.Text = values[0];
                txtCondDia2_1.Text = values[1];
                txtCondDia3_1.Text = values[2];
                txtCondDia4_1.Text = values[3];
                txtCondDia5_1.Text = values[4];
                txtCondDia6_1.Text = values[5];
            }
        }},
        // Conductor Diameter - Nominal
        { "12776", value => txtConDiaNom_1.Text = value },
        
        // Lay Ratios - Test Results (min;max)
        { "12779", value => {
            string[] values = value.Split(';');
            if (values.Length >= 2) {
                txtLayRatioMin1_1.Text = values[0];
                txtLayRatioMax1_1.Text = values[1];
            }
        }},
        // Lay Ratios - Nominal
        { "12780", value => txtLayRatioNom_1.Text = value },
        
        // Direction of Lay - Test Result
        { "12781", value => txtDirectionOfLay1_1.Text = value },
        
        // Standard Requirement (min;max)
        { "12777", value => {
            string[] values = value.Split(';');
            if (values.Length >= 2) {
                txtStdMin_1.Text = values[0];
                txtStdMax_1.Text = values[1];
            }
        }},
        // Standard Nominal
        { "12778", value => txtStdNom_1.Text = value },
        
        // Standard Lay Ratios (min;max)
        { "12783", value => {
            string[] values = value.Split(';');
            if (values.Length >= 2) {
                txtStdLayMin_1.Text = values[0];
                txtStdLayMax_1.Text = values[1];
            }
        }},
        // Standard Lay Ratios - Nominal
        { "12784", value => txtStdLayNom_1.Text = value },
        
        // Standard Direction of Lay
        { "12782", value => txtStdDirectionOfLay_1.Text = value },

        // ===== Table 2 =====
        // Conductor Diameter - Test Results (6 values)
        { "12785", value => {
            string[] values = value.Split(',');
            if (values.Length >= 6) {
                txtCondDia1_2.Text = values[0];
                txtCondDia2_2.Text = values[1];
                txtCondDia3_2.Text = values[2];
                txtCondDia4_2.Text = values[3];
                txtCondDia5_2.Text = values[4];
                txtCondDia6_2.Text = values[5];
            }
        }},
        // Conductor Diameter - Nominal
        { "12786", value => txtConDiaNom_2.Text = value },
        
        // Standard Requirement (min;max)
        { "12787", value => {
            string[] values = value.Split(';');
            if (values.Length >= 2) {
                txtStdMin_2.Text = values[0];
                txtStdMax_2.Text = values[1];
            }
        }},
        // Standard Nominal
        { "12788", value => txtStdNom_2.Text = value },
        
        // Lay Ratios - Test Results (min;max)
        { "12789", value => {
            string[] values = value.Split(';');
            if (values.Length >= 2) {
                txtLayRatioMin1_2.Text = values[0];
                txtLayRatioMax1_2.Text = values[1];
            }
        }},
        // Lay Ratios - Nominal
        { "12790", value => txtLayRatioNom_2.Text = value },
        
        // Direction of Lay - Test Result
        { "12819", value => txtDirectionOfLay1_2.Text = value },
        
        // Direction of Lay - Standard Requirement
        { "12820", value => txtStdDirectionOfLay_2.Text = value },
        
        // Standard Lay Ratios (min;max)
        { "12821", value => {
            string[] values = value.Split(';');
            if (values.Length >= 2) {
                txtStdLayMin_2.Text = values[0];
                txtStdLayMax_2.Text = values[1];
            }
        }},
        // Standard Lay Ratios - Nominal
        { "12822", value => txtStdLayNom_2.Text = value },

        // ===== Other Fields =====
        { "12791", value => cableType.Text = value },
        { "12792", value => cableSize.Text = value },
        { "12793", value => voltage.Text = value },
        { "12794", value => Date.Text = value },
        { "12795", value => roomno.Text = value },
        { "12796", value => roomtemp.Text = value },
        { "12797", value => refSpec.Text = value },
        { "12798", value => testMethodNo.Text = value },
        { "12799", value => jobOrderNo.Text = value },
        { "12800", value => worksheetno.Text = value },
        { "12801", value => roomhumidity.Text = value },
        { "12802", value => equipmentno.Text = value }
    };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"]?.ToString();

                if (parameterControls.TryGetValue(paramId, out var setControlValue))
                {
                    setControlValue(rawValue);
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

                // ===== Table 1 =====
                reportParamMapList.Add("12775");
                resultList.Add($"{txtCondDia1_1.Text.Trim()},{txtCondDia2_1.Text.Trim()},{txtCondDia3_1.Text.Trim()},{txtCondDia4_1.Text.Trim()},{txtCondDia5_1.Text.Trim()},{txtCondDia6_1.Text.Trim()}");

                reportParamMapList.Add("12776");
                resultList.Add($"{txtConDiaNom_1.Text.Trim()}");

                reportParamMapList.Add("12779");
                resultList.Add($"{txtLayRatioMin1_1.Text.Trim()};{txtLayRatioMax1_1.Text.Trim()}");

                reportParamMapList.Add("12780");
                resultList.Add($"{txtLayRatioNom_1.Text.Trim()}");

                reportParamMapList.Add("12781");
                resultList.Add($"{txtDirectionOfLay1_1.Text.Trim()}");

                reportParamMapList.Add("12777");
                resultList.Add($"{txtStdMin_1.Text.Trim()};{txtStdMax_1.Text.Trim()}");

                reportParamMapList.Add("12778");
                resultList.Add($"{txtStdNom_1.Text.Trim()}");

             
                reportParamMapList.Add("12783");
                resultList.Add($"{txtStdLayMin_1.Text.Trim()};{txtStdLayMax_1.Text.Trim()}");

                reportParamMapList.Add("12784");
                resultList.Add($"{txtStdLayNom_1.Text.Trim()}");

                reportParamMapList.Add("12782");
                resultList.Add($"{txtStdDirectionOfLay_1.Text.Trim()}");


             
                // ===== Table 2: Conductor Diameter - Test Results =====
                reportParamMapList.Add("12785"); // Test results (comma-separated)
                resultList.Add($"{txtCondDia1_2.Text.Trim()},{txtCondDia2_2.Text.Trim()},{txtCondDia3_2.Text.Trim()},{txtCondDia4_2.Text.Trim()},{txtCondDia5_2.Text.Trim()},{txtCondDia6_2.Text.Trim()}");

                // ===== Table 2: Conductor Diameter - Test Results Nominal =====
                reportParamMapList.Add("12786"); // Nominal
                resultList.Add($"{txtConDiaNom_2.Text.Trim()}");

                // ===== Table 2: Conductor Diameter - Standard Requirement =====
                reportParamMapList.Add("12787"); // Min/Max (standard requirement)
                resultList.Add($"{txtStdMin_2.Text.Trim()};{txtStdMax_2.Text.Trim()}");

                // ===== Table 2: Conductor Diameter - Standard Requirement Nominal =====
                reportParamMapList.Add("12788"); // Nominal
                resultList.Add($"{txtStdNom_2.Text.Trim()}");

                // ===== Table 2: Lay Ratios - Test Results =====
                reportParamMapList.Add("12789"); // Min/Max
                resultList.Add($"{txtLayRatioMin1_2.Text.Trim()};{txtLayRatioMax1_2.Text.Trim()}");

                // ===== Table 2: Lay Ratios - Test Results Nominal =====
                reportParamMapList.Add("12790"); // Nominal
                resultList.Add($"{txtLayRatioNom_2.Text.Trim()}");

                // ===== Table 2: Direction of Lay - Test Result =====
                reportParamMapList.Add("12819"); // Test Result
                resultList.Add($"{txtDirectionOfLay1_2.Text.Trim()}");

                // ===== Table 2: Direction of Lay - Standard Requirement =====
                reportParamMapList.Add("12820"); // Standard Requirement
                resultList.Add($"{txtStdDirectionOfLay_2.Text.Trim()}");

                // ===== Table 2: Lay Ratios - Standard Requirement =====
                reportParamMapList.Add("12821"); // Standard Min/Max
                resultList.Add($"{txtStdLayMin_2.Text.Trim()};{txtStdLayMax_2.Text.Trim()}");

                // ===== Table 2: Lay Ratios - Standard Requirement Nominal =====
                reportParamMapList.Add("12822"); // Nominal
                resultList.Add($"{txtStdLayNom_2.Text.Trim()}");


                reportParamMapList.Add("12791"); resultList.Add(cableType.Text); // Cable Type
                reportParamMapList.Add("12792"); resultList.Add(cableSize.Text); // Cable Size
                reportParamMapList.Add("12793"); resultList.Add(voltage.Text); // Voltage
                reportParamMapList.Add("12794"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("12795"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("12796"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("12797"); resultList.Add(refSpec.Text); // Reference Specification              
                reportParamMapList.Add("12798"); resultList.Add(testMethodNo.Text); // Test Method No               
                reportParamMapList.Add("12799"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("12800"); resultList.Add(worksheetno.Text); // Worksheet No            
                reportParamMapList.Add("12801"); resultList.Add(roomhumidity.Text); // Room Humidity             
                reportParamMapList.Add("12802"); resultList.Add(equipmentno.Text); // Equipment No



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

                    Response.Redirect($"GenerateTestRecord_DirectionOfLayandLength.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // ==== Table 1 - Test Results ====
            txtCondDia1_1.Text = "";
            txtCondDia2_1.Text = "";
            txtCondDia3_1.Text = "";
            txtCondDia4_1.Text = "";
            txtCondDia5_1.Text = "";
            txtCondDia6_1.Text = "";
            txtConDiaNom_1.Text = "";
            txtLayRatioMin1_1.Text = "";
            txtLayRatioMax1_1.Text = "";
            txtLayRatioNom_1.Text = "";
            txtDirectionOfLay1_1.Text = "";

            // ==== Table 1 - Standard Requirements ====
            txtStdMin_1.Text = "";
            txtStdMax_1.Text = "";
            txtStdNom_1.Text = "";
            txtStdLayMin_1.Text = "";
            txtStdLayMax_1.Text = "";
            txtStdLayNom_1.Text = "";
            txtStdDirectionOfLay_1.Text = "";

            // ==== Table 2 - Test Results ====
            txtCondDia1_2.Text = "";
            txtCondDia2_2.Text = "";
            txtCondDia3_2.Text = "";
            txtCondDia4_2.Text = "";
            txtCondDia5_2.Text = "";
            txtCondDia6_2.Text = "";
            txtConDiaNom_2.Text = "";
            txtLayRatioMin1_2.Text = "";
            txtLayRatioMax1_2.Text = "";
            txtLayRatioNom_2.Text = "";
            txtDirectionOfLay1_2.Text = "";

            // ==== Table 2 - Standard Requirements ====
            txtStdMin_2.Text = "";
            txtStdMax_2.Text = "";
            txtStdNom_2.Text = "";
            txtStdLayMin_2.Text = "";
            txtStdLayMax_2.Text = "";
            txtStdLayNom_2.Text = "";
            txtStdDirectionOfLay_2.Text = "";

         
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