using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class CompositionTest : System.Web.UI.Page
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
                    voltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();
                    // hardcoded fields
                    roomno.Text = "Mechanical & Oven";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/17/PH/175,QE/12/TH/09";
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
                    DataTable existingData = _cm.GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        // Show update button since data exists
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        // Prefill upper section details
                        PrefillUpperSection(existingData);


                        // Prefill test results grid
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
        private void PrefillUpperSection(DataTable existingData)
        {

            // Method to prefill upper section details based on parameter IDs
            SetTextBoxText(cableType, "12662");
            SetTextBoxText(cableSize, "12663");
            SetTextBoxText(voltage, "12664");
            SetTextBoxText(refSpec, "12668");
            SetTextBoxText(testMethodNo, "12669");
            SetTextBoxText(jobOrderNo, "12670");
            SetTextBoxText(worksheetno, "12671");
            SetTextBoxText(roomno, "12666");
            SetTextBoxText(roomhumidity, "12672");
            SetTextBoxText(roomtemp, "12667");
            SetTextBoxText(Date, "12665");
            SetTextBoxText(equipmentno, "12673");


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
        private void PrefillTestResultsGrid(DataTable dt)
        {
            // Map reportParamId to  controls
            Dictionary<string, (TextBox TestResult ,TextBox Standard, TextBox Remark)> fieldMap = new Dictionary<string, (TextBox, TextBox, TextBox)>
    {
       { "12531", (txtAlTestResult, txtAlStandard, txtAlRemark) },
    { "12532", (txtSiTestResult, txtSiStandard, txtSiRemark) },
    { "12533", (txtFeTestResult, txtFeStandard, txtFeRemark) },
    { "12534", (txtCuTestResult, txtCuStandard, txtCuRemark) },
    { "12535", (txtMnTestResult, txtMnStandard, txtMnRemark) },
    { "12536", (txtTiTestResult, txtTiStandard, txtTiRemark) },
    { "12537", (txtVTestResult, txtVStandard, txtVRemark) },
    { "12538", (txtCrTestResult, txtCrStandard, txtCrRemark) },
    { "12539", (txtBTestResult, txtBStandard, txtBRemark) },
    { "12540", (txtZnTestResult, txtZnStandard, txtZnRemark) },
    { "12541", (txtGaTestResult, txtGaStandard, txtGaRemark) },
    { "12543", (txtCTestResult, txtCStandard, txtCRemark) },
    { "12544", (txtSTestResult, txtSStandard, txtSRemark) },
    { "12545", (txtPTestResult, txtPStandard, txtPRemark) },
    { "12546", (txtMgTestResult, txtMgStandard, txtMgRemark) },
    { "12547", (txtNaTestResult, txtNaStandard, txtNaRemark) },
    { "12548", (txtZrTestResult, txtZrStandard, txtZrRemark) },
    {"12549", (txtTestResultSpecMax,txtStandardSpecMax,txtRemarkSpecMax)}  
                    };

            foreach (DataRow row in dt.Rows)
            {
                // Use correct DataTable column names
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (fieldMap.ContainsKey(paramId))
                {
                    var controls = fieldMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length == 3)
                    {

                        controls.TestResult.Text = parts[0];
                        controls.Standard.Text = parts[1];
                        controls.Remark.Text = parts[2];
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


                reportParamMapList.Add("12531"); // Al
                resultList.Add($"{txtAlTestResult.Text.Trim()}|{txtAlStandard.Text.Trim()}|{txtAlRemark.Text.Trim()}");

                reportParamMapList.Add("12532"); // Si
                resultList.Add($"{txtSiTestResult.Text.Trim()}|{txtSiStandard.Text.Trim()}|{txtSiRemark.Text.Trim()}");

                reportParamMapList.Add("12533"); // Fe
                resultList.Add($"{txtFeTestResult.Text.Trim()}|{txtFeStandard.Text.Trim()}|{txtFeRemark.Text.Trim()}");

                reportParamMapList.Add("12534"); // Cu
                resultList.Add($"{txtCuTestResult.Text.Trim()}|{txtCuStandard.Text.Trim()}|{txtCuRemark.Text.Trim()}");

                reportParamMapList.Add("12535"); // Mn
                resultList.Add($"{txtMnTestResult.Text.Trim()}|{txtMnStandard.Text.Trim()}|{txtMnRemark.Text.Trim()}");

                reportParamMapList.Add("12536"); // Ti
                resultList.Add($"{txtTiTestResult.Text.Trim()}|{txtTiStandard.Text.Trim()}|{txtTiRemark.Text.Trim()}");

                reportParamMapList.Add("12537"); // V
                resultList.Add($"{txtVTestResult.Text.Trim()}|{txtVStandard.Text.Trim()}|{txtVRemark.Text.Trim()}");

                reportParamMapList.Add("12538"); // Cr
                resultList.Add($"{txtCrTestResult.Text.Trim()}|{txtCrStandard.Text.Trim()}|{txtCrRemark.Text.Trim()}");

                reportParamMapList.Add("12539"); // B
                resultList.Add($"{txtBTestResult.Text.Trim()}|{txtBStandard.Text.Trim()}|{txtBRemark.Text.Trim()}");

                reportParamMapList.Add("12540"); // Zn
                resultList.Add($"{txtZnTestResult.Text.Trim()}|{txtZnStandard.Text.Trim()}|{txtZnRemark.Text.Trim()}");

                reportParamMapList.Add("12541"); // Ga
                resultList.Add($"{txtGaTestResult.Text.Trim()}|{txtGaStandard.Text.Trim()}|{txtGaRemark.Text.Trim()}");

                reportParamMapList.Add("12543"); // C
                resultList.Add($"{txtCTestResult.Text.Trim()}|{txtCStandard.Text.Trim()}|{txtCRemark.Text.Trim()}");

                reportParamMapList.Add("12544"); // S
                resultList.Add($"{txtSTestResult.Text.Trim()}|{txtSStandard.Text.Trim()}|{txtSRemark.Text.Trim()}");

                reportParamMapList.Add("12545"); // P
                resultList.Add($"{txtPTestResult.Text.Trim()}|{txtPStandard.Text.Trim()}|{txtPRemark.Text.Trim()}");

                reportParamMapList.Add("12546"); // Mg
                resultList.Add($"{txtMgTestResult.Text.Trim()}|{txtMgStandard.Text.Trim()}|{txtMgRemark.Text.Trim()}");

                reportParamMapList.Add("12547"); // Na
                resultList.Add($"{txtNaTestResult.Text.Trim()}|{txtNaStandard.Text.Trim()}|{txtNaRemark.Text.Trim()}");

                reportParamMapList.Add("12548"); // Zr
                resultList.Add($"{txtZrTestResult.Text.Trim()}|{txtZrStandard.Text.Trim()}|{txtZrRemark.Text.Trim()}");

                reportParamMapList.Add("12549"); // Spec.Max
                resultList.Add($"{txtTestResultSpecMax.Text.Trim()}|{txtStandardSpecMax.Text.Trim()}|{txtRemarkSpecMax.Text.Trim()}");




                //Upper section
                reportParamMapList.Add("12662"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12663"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12664"); resultList.Add(voltage.Text);
                reportParamMapList.Add("12668"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12669"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12670"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12671"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12666"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12672"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12667"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12665"); resultList.Add(Date.Text);
                reportParamMapList.Add("12673"); resultList.Add(equipmentno.Text);

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

                    Response.Redirect($"GenerateTestRecord_CompositionTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // Al
            txtAlTestResult.Text = "";
            txtAlStandard.Text = "";
            txtAlRemark.Text = "";

            // Si
            txtSiTestResult.Text = "";
            txtSiStandard.Text = "";
            txtSiRemark.Text = "";

            // Fe
            txtFeTestResult.Text = "";
            txtFeStandard.Text = "";
            txtFeRemark.Text = "";

            // Cu
            txtCuTestResult.Text = "";
            txtCuStandard.Text = "";
            txtCuRemark.Text = "";

            // Mn
            txtMnTestResult.Text = "";
            txtMnStandard.Text = "";
            txtMnRemark.Text = "";

            // Ti
            txtTiTestResult.Text = "";
            txtTiStandard.Text = "";
            txtTiRemark.Text = "";

            // V
            txtVTestResult.Text = "";
            txtVStandard.Text = "";
            txtVRemark.Text = "";

            // Cr
            txtCrTestResult.Text = "";
            txtCrStandard.Text = "";
            txtCrRemark.Text = "";

            // B
            txtBTestResult.Text = "";
            txtBStandard.Text = "";
            txtBRemark.Text = "";

            // Zn
            txtZnTestResult.Text = "";
            txtZnStandard.Text = "";
            txtZnRemark.Text = "";

            // Ga
            txtGaTestResult.Text = "";
            txtGaStandard.Text = "";
            txtGaRemark.Text = "";

            // C
            txtCTestResult.Text = "";
            txtCStandard.Text = "";
            txtCRemark.Text = "";

         
            txtSTestResult.Text = "";
            txtSStandard.Text = "";
            txtSRemark.Text = "";

           
            txtPTestResult.Text = "";
            txtPStandard.Text = "";
            txtPRemark.Text = "";

            txtMgTestResult.Text = "";
            txtMgStandard.Text = "";
            txtMgRemark.Text = "";

           
            txtNaTestResult.Text = "";
            txtNaStandard.Text = "";
            txtNaRemark.Text = "";

           
            txtZrTestResult.Text = "";
            txtZrStandard.Text = "";
            txtZrRemark.Text = "";

            txtTestResultSpecMax.Text = "";
                txtStandardSpecMax.Text = "";
            txtRemarkSpecMax.Text = "";
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