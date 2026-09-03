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
    public partial class BusBarDimensionTest : System.Web.UI.Page
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
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError", $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }

        private void PrefillUpperSection(DataTable existingData)
        {
            SetLabelText(cableType, "12425");
            SetLabelText(cableSize, "12426");
            SetLabelText(voltage, "12427");
            SetTextBoxText(Date, "12428");
            SetTextBoxText(roomno, "12429");
            SetTextBoxText(roomtemp, "12430");
            SetLabelText(refSpec, "12431");
            SetLabelText(testMethodNo, "12432");
            SetLabelText(jobOrderNo, "12433");
            SetLabelText(worksheetno, "12434");
            SetTextBoxText(roomhumidity, "12435");
            SetLabelText(equipmentno, "12436");

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
            // Input rows with: Unit dropdown, Specified textbox, and 3 test value textboxes
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox txtInput1, TextBox txtInput2, TextBox txtInput3)>
            {
                { "12439", (ddlUnit1, txtSpecified1_1, txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3) }, // Width
                { "12441", (ddlUnit2, txtSpecified1_3, txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3) }, // Thickness                
            };

            // Label rows with average values
            var avgMap = new Dictionary<string, Label>
            {
                { "12440", lblAverage1_1 }, // Width average
                { "12442", lblAverage1_3 }, // Thickness average
            };

            // Rows with only a dropdown (e.g., test result row at the bottom)
            var dropDownOnlyMap = new Dictionary<string, DropDownList>
            {
                { "12443", ddlTestResult } // Result of test dropdown
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                // Process full input rows
                if (inputMap.ContainsKey(paramId))
                {
                    var controls = inputMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length >= 3)
                    {
                        controls.ddlUnit.SelectedValue = parts[0];
                        controls.txtSpecified.Text = parts[1];

                        string[] inputs = parts[2].Split(',');
                        if (inputs.Length == 3)
                        {
                            controls.txtInput1.Text = inputs[0];
                            controls.txtInput2.Text = inputs[1];
                            controls.txtInput3.Text = inputs[2];
                        }
                    }
                }
                // Process average labels
                else if (avgMap.ContainsKey(paramId))
                {
                    avgMap[paramId].Text = rawValue;
                }
                // Process only-dropdown rows (e.g., test result)
                else if (dropDownOnlyMap.ContainsKey(paramId))
                {
                    dropDownOnlyMap[paramId].SelectedValue = rawValue;
                }
            }
        }

        protected void TextValue_TextChanged(object sender, EventArgs e)
        {
            CalculateAverages();
        }

        private void CalculateAverages()
        {
            // Width average
            CalculateSingleAverage(
                txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3,
                lblAverage1_1);

            // Thickness average
            CalculateSingleAverage(
                txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3,
                lblAverage1_3);

        }

        private void CalculateSingleAverage(TextBox txt1, TextBox txt2, TextBox txt3, Label lblAverage)
        {
            double value1, value2, value3;

            double.TryParse(txt1.Text, out value1);
            double.TryParse(txt2.Text, out value2);
            double.TryParse(txt3.Text, out value3);

            double average = (value1 + value2 + value3) / 3;
            lblAverage.Text = average.ToString("0.00");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CalculateAverages();

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
                reportParamMapList.Add("12425"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12426"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12427"); resultList.Add(voltage.Text);
                reportParamMapList.Add("12428"); resultList.Add(Date.Text);
                reportParamMapList.Add("12429"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12430"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12431"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12432"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12433"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12434"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12435"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12436"); resultList.Add(equipmentno.Text);

                reportParamMapList.Add("12439");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1_1.Text.Trim()}|{txtValueTest1_1_1.Text.Trim()},{txtValueTest1_1_2.Text.Trim()},{txtValueTest1_1_3.Text.Trim()}");

                reportParamMapList.Add("12440");
                resultList.Add($"{lblAverage1_1.Text.Trim()}");

                reportParamMapList.Add("12441");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified1_3.Text.Trim()}|{txtValueTest1_3_1.Text.Trim()},{txtValueTest1_3_2.Text.Trim()},{txtValueTest1_3_3.Text.Trim()}");

                reportParamMapList.Add("12442");
                resultList.Add($"{lblAverage1_3.Text.Trim()}");
                
                reportParamMapList.Add("12443");
                resultList.Add($"{ddlTestResult.SelectedValue}");

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

                    Response.Redirect($"GenerateTestRecord_BusBarDimensionTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void ClearForm()
        {
            // Clear Width values
            txtValueTest1_1_1.Text = "";
            txtValueTest1_1_2.Text = "";
            txtValueTest1_1_3.Text = "";
            lblAverage1_1.Text = "";

            // Clear Thickness values
            txtValueTest1_3_1.Text = "";
            txtValueTest1_3_2.Text = "";
            txtValueTest1_3_3.Text = "";
            lblAverage1_3.Text = "";


            // Reset DropDowns
            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlTestResult.SelectedIndex = 0;
        }

        private void DeleteExistingRecords(int sampleId, int reportId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

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