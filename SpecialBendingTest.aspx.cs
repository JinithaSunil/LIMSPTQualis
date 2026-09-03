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
    public partial class SpecialBendingTest : System.Web.UI.Page
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
                    roomno.Text = "Mechanical";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/12/PH/16,QE/16/PH/03,QE/15/PH/43";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
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
                    else
                    {
                        //txtSpecified1.Text = "Clean, No oxidation, No Crack, No Corrosion, not wave and Not Curved";
                        //txtSpecified2.Text = "Min. : 99.96";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError", $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }

        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "12968");
            SetLabelText(cableSize, "12969");
            SetLabelText(voltage, "12970");
            SetTextBoxText(Date, "12971");
            SetTextBoxText(roomno, "12972");
            SetTextBoxText(roomtemp, "12973");
            SetLabelText(refSpec, "12974");
            SetLabelText(testMethodNo, "12975");
            SetLabelText(jobOrderNo, "12976");
            SetLabelText(worksheetno, "12977");
            SetTextBoxText(roomhumidity, "12978");
            SetLabelText(equipmentno, "12979");

            void SetLabelText(Label label, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

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
            Dictionary<string, (DropDownList unit, TextBox spec, TextBox test)> fieldMap = new Dictionary<string, (DropDownList, TextBox, TextBox)>
            {
                { "12982", (ddlUnit1, txtSpecifiedValue1, txtMeasured1) },
                { "12983", (ddlUnit2, txtSpecifiedValue2, txtMeasured2) },
                { "12984", (ddlUnit3, txtSpecifiedValue3, txtMeasured3) },
                { "12985", (ddlUnit4, txtSpecifiedValue4, txtMeasured4) },
                { "12986", (ddlUnit5, txtSpecifiedValue5, txtMeasured5) },
                { "12987", (ddlUnit6, txtSpecifiedValue6, txtMeasured6) },
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (fieldMap.ContainsKey(paramId))
                {
                    var controls = fieldMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length == 3)
                    {
                        if (controls.unit.Items.FindByValue(parts[0]) != null)
                        {
                            controls.unit.SelectedValue = parts[0];
                        }
                        controls.spec.Text = parts[1];
                        controls.test.Text = parts[2];
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool isSaved = Submitform();
            if (isSaved)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess", "alert('Test results and Report Generated successfully!');", true);
                ClearForm();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveFailed", "alert('Test results were not saved due to an error.');", true);
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

                //Upper section
                reportParamMapList.Add("12968"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12969"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12970"); resultList.Add(voltage.Text);
                reportParamMapList.Add("12971"); resultList.Add(Date.Text);
                reportParamMapList.Add("12972"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12973"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12974"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12975"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12976"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12977"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12978"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12979"); resultList.Add(equipmentno.Text);


                reportParamMapList.Add("12982");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecifiedValue1.Text.Trim()}|{txtMeasured1.Text.Trim()}");

                reportParamMapList.Add("12983");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecifiedValue2.Text.Trim()}|{txtMeasured2.Text.Trim()}");

                reportParamMapList.Add("12984");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecifiedValue3.Text.Trim()}|{txtMeasured3.Text.Trim()}");

                reportParamMapList.Add("12985");
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecifiedValue4.Text.Trim()}|{txtMeasured4.Text.Trim()}");

                reportParamMapList.Add("12986");
                resultList.Add($"{ddlUnit5.SelectedValue}|{txtSpecifiedValue5.Text.Trim()}|{txtMeasured5.Text.Trim()}");

                reportParamMapList.Add("12987");
                resultList.Add($"{ddlUnit6.SelectedValue}|{txtSpecifiedValue6.Text.Trim()}|{txtMeasured6.Text.Trim()}");


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

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Data saved successfully!');", true);

                    Response.Redirect($"SpecialBendingTest_GR.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save data.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }

        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void ClearForm()
        {
            // Clear Specified values
            txtSpecifiedValue1.Text = "";
            txtSpecifiedValue2.Text = "";
            txtSpecifiedValue3.Text = "";
            txtSpecifiedValue4.Text = "";
            txtSpecifiedValue5.Text = "";

            // Clear Test values
            txtMeasured1.Text = "";
            txtMeasured2.Text = "";
            txtMeasured3.Text = "";
            txtMeasured4.Text = "";
            txtMeasured5.Text = "";

            // Reset DropDowns (set to default or first item)
            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;
            ddlUnit4.SelectedIndex = 0;
            ddlUnit5.SelectedIndex = 0;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void DeleteExistingRecords(int sampleId, int reportId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID and ReportID = @ReportID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);
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
                    int reportID = Convert.ToInt32(Request.QueryString["r"]);
                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;

                    DeleteExistingRecords(sampleId, reportID);

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