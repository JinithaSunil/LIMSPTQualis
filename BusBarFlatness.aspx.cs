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
    public partial class BusBarFlatness : System.Web.UI.Page
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
                    roomno.Text = "PHYSICAL";
                    jobOrderNo.Text = "WO/2022081385";
                    worksheetno.Text = "-";
                    equipmentno.Text = "Extension meter";
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
            SetLabelText(cableType, "12683");
            SetLabelText(cableSize, "12477");
            SetLabelText(voltage, "12478");
            SetTextBoxText(Date, "12479");
            SetTextBoxText(roomno, "12480");
            SetTextBoxText(roomtemp, "12481");
            SetLabelText(refSpec, "12482");
            SetLabelText(testMethodNo, "12483");
            SetLabelText(jobOrderNo, "12484");
            SetLabelText(worksheetno, "12485");
            SetTextBoxText(roomhumidity, "12486");
            SetLabelText(equipmentno, "12487");

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
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox txtInput1, TextBox txtInput2, TextBox txtInput3, TextBox txtInput4, TextBox txtInput5, TextBox txtInput6, TextBox txtInput7, TextBox txtInput8)>
            {
                { "12490", (ddlUnit1, txtSpecified1_1, txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3, txtValueTest1_1_4, txtValueTest1_1_5, txtValueTest1_1_6, txtValueTest1_1_7, txtValueTest1_1_8) },
                { "12491", (ddlUnit2, txtSpecified1_2, txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3, txtValueTest1_2_4, txtValueTest1_2_5, txtValueTest1_2_6, txtValueTest1_2_7, txtValueTest1_2_8) },
                { "12492", (ddlUnit3, txtSpecified1_3, txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3, txtValueTest1_3_4, txtValueTest1_3_5, txtValueTest1_3_6, txtValueTest1_3_7, txtValueTest1_3_8) }
            };

            var avgMap = new Dictionary<string, Label>
            {
                { "12871", lblAverage1_1 },
                { "12872", lblAverage1_2 },
                { "12873", lblAverage1_3 }
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"]?.ToString();
                string rawValue = row["Result"]?.ToString();

                if (string.IsNullOrEmpty(paramId) || string.IsNullOrEmpty(rawValue))
                    continue;

                // Prefill Input Controls
                if (inputMap.ContainsKey(paramId))
                {
                    var controls = inputMap[paramId];
                    string[] parts = rawValue.Split('|');

                    if (parts.Length >= 3)
                    {
                        try
                        {
                            controls.ddlUnit.SelectedValue = parts[0];
                        }
                        catch
                        {
                            // ignore if value not found in dropdown
                        }

                        controls.txtSpecified.Text = parts[1];

                        string[] inputs = parts[2].Split(',');
                        TextBox[] textInputs = { controls.txtInput1, controls.txtInput2, controls.txtInput3, controls.txtInput4, controls.txtInput5, controls.txtInput6, controls.txtInput7, controls.txtInput8 };

                        for (int i = 0; i < inputs.Length && i < textInputs.Length; i++)
                        {
                            textInputs[i].Text = inputs[i];
                        }
                    }
                }
                // Prefill Average Labels
                else if (avgMap.ContainsKey(paramId))
                {
                    avgMap[paramId].Text = rawValue;
                }
            }
        }

        protected void TextValue_TextChanged(object sender, EventArgs e)
        {
            /* CalculateAverages(); */
            CalculateMedians();
        }

        private void CalculateMedians()
        {
            // Row 1.1
            CalculateRowMedian(
                txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3, txtValueTest1_1_4,
                txtValueTest1_1_5, txtValueTest1_1_6, txtValueTest1_1_7, txtValueTest1_1_8,
                lblAverage1_1); // Keep the ID as lblAverage for backward compatibility

            // Row 1.2
            CalculateRowMedian(
                txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3, txtValueTest1_2_4,
                txtValueTest1_2_5, txtValueTest1_2_6, txtValueTest1_2_7, txtValueTest1_2_8,
                lblAverage1_2);

            // Row 1.3
            CalculateRowMedian(
                txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3, txtValueTest1_3_4,
                txtValueTest1_3_5, txtValueTest1_3_6, txtValueTest1_3_7, txtValueTest1_3_8,
                lblAverage1_3);
        }

        private void CalculateRowMedian(params ITextControl[] controls)
        {
            List<double> validValues = new List<double>();

            // Step 1: Collect and validate inputs (skip the last control which is the label)
            for (int i = 0; i < controls.Length - 1; i++)
            {
                string input = controls[i].Text.Trim();

                // Skip if blank or "-"
                if (!string.IsNullOrEmpty(input) && input != "-")
                {
                    if (double.TryParse(input, out double value))
                    {
                        validValues.Add(value);
                    }
                }
            }

            // Step 2: Calculate median only if valid values exist
            if (validValues.Count > 0)
            {
                validValues.Sort(); // Sort for median calculation
                double median;

                if (validValues.Count % 2 == 0)
                {
                    // Even: Average of two middle values
                    int midIndex = validValues.Count / 2;
                    median = (validValues[midIndex - 1] + validValues[midIndex]) / 2.0;
                }
                else
                {
                    // Odd: Middle value
                    median = validValues[validValues.Count / 2];
                }

                controls.Last().Text = median.ToString("F2"); // Display median (e.g., "5.00")
            }
            else
            {
                controls.Last().Text = ""; // Clear if no valid inputs
            }
        }

        private void CalculateAverages()
        {
            // Row 1.1
            CalculateRowAverage(
                txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3, txtValueTest1_1_4,
                txtValueTest1_1_5, txtValueTest1_1_6, txtValueTest1_1_7, txtValueTest1_1_8,
                lblAverage1_1);

            // Row 1.2
            CalculateRowAverage(
                txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3, txtValueTest1_2_4,
                txtValueTest1_2_5, txtValueTest1_2_6, txtValueTest1_2_7, txtValueTest1_2_8,
                lblAverage1_2);

            // Row 1.3
            CalculateRowAverage(
                txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3, txtValueTest1_3_4,
                txtValueTest1_3_5, txtValueTest1_3_6, txtValueTest1_3_7, txtValueTest1_3_8,
                lblAverage1_3);
        }

        private void CalculateRowAverage(params ITextControl[] controls)
        {
            double sum = 0;
            int count = 0;

            // Last control is the average label
            for (int i = 0; i < controls.Length - 1; i++)
            {
                if (double.TryParse(controls[i].Text, out double value))
                {
                    sum += value;
                    count++;
                }
            }

            if (count > 0)
            {
                controls[controls.Length - 1].Text = (sum / count).ToString("F2");
            }
            else
            {
                controls[controls.Length - 1].Text = "";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            /*CalculateAverages();*/
            CalculateMedians();

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
                reportParamMapList.Add("12683"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12477"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12478"); resultList.Add(voltage.Text);
                reportParamMapList.Add("12479"); resultList.Add(Date.Text);
                reportParamMapList.Add("12480"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12481"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12482"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12483"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12484"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12485"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12486"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12487"); resultList.Add(equipmentno.Text);

                reportParamMapList.Add("12490");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1_1.Text.Trim()}|" +
                    $"{txtValueTest1_1_1.Text.Trim()},{txtValueTest1_1_2.Text.Trim()},{txtValueTest1_1_3.Text.Trim()}," +
                    $"{txtValueTest1_1_4.Text.Trim()},{txtValueTest1_1_5.Text.Trim()},{txtValueTest1_1_6.Text.Trim()}," +
                    $"{txtValueTest1_1_7.Text.Trim()},{txtValueTest1_1_8.Text.Trim()}");

                reportParamMapList.Add("12491");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified1_2.Text.Trim()}|" +
                    $"{txtValueTest1_2_1.Text.Trim()},{txtValueTest1_2_2.Text.Trim()},{txtValueTest1_2_3.Text.Trim()}," +
                    $"{txtValueTest1_2_4.Text.Trim()},{txtValueTest1_2_5.Text.Trim()},{txtValueTest1_2_6.Text.Trim()}," +
                    $"{txtValueTest1_2_7.Text.Trim()},{txtValueTest1_2_8.Text.Trim()}");

                reportParamMapList.Add("12492");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecified1_3.Text.Trim()}|" +
                    $"{txtValueTest1_3_1.Text.Trim()},{txtValueTest1_3_2.Text.Trim()},{txtValueTest1_3_3.Text.Trim()}," +
                    $"{txtValueTest1_3_4.Text.Trim()},{txtValueTest1_3_5.Text.Trim()},{txtValueTest1_3_6.Text.Trim()}," +
                    $"{txtValueTest1_3_7.Text.Trim()},{txtValueTest1_3_8.Text.Trim()}");

                reportParamMapList.Add("12871");
                resultList.Add($"{lblAverage1_1.Text.Trim()}");

                reportParamMapList.Add("12872");
                resultList.Add($"{lblAverage1_2.Text.Trim()}");

                reportParamMapList.Add("12873");
                resultList.Add($"{lblAverage1_3.Text.Trim()}");


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
                    Response.Redirect($"GenerateTestRecord_BusBarFlatness.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // Clear  values (Row 1.1)
            txtValueTest1_1_1.Text = "";
            txtValueTest1_1_2.Text = "";
            txtValueTest1_1_3.Text = "";
            lblAverage1_1.Text = "";

            // Clear  values (Row 1.2)
            txtValueTest1_2_1.Text = "";
            txtValueTest1_2_2.Text = "";
            txtValueTest1_2_3.Text = "";
            lblAverage1_2.Text = "";

            // Clear values (Row 1.3)
            txtValueTest1_3_1.Text = "";
            txtValueTest1_3_2.Text = "";
            txtValueTest1_3_3.Text = "";
            lblAverage1_3.Text = "";

            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;

            txtSpecified1_1.Text = "";
            txtSpecified1_2.Text = "";
            txtSpecified1_3.Text = "";
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