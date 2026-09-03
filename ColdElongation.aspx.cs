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
    public partial class ColdElongation : System.Web.UI.Page
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

                /*
                 * DataTable dt0 = _cm.Get_Core();
                ddlGetCore.DataSource = dt0;
                ddlGetCore.DataTextField = "CoreName";
                ddlGetCore.DataValueField = "CoreId";
                ddlGetCore.DataBind();
                ddlGetCore.Items.Insert(0, new ListItem("-- Select Core --", "0"));*/

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
            SetLabelText(cableType, "14058");
            SetLabelText(cableSize, "14059");
            SetLabelText(voltage, "14060");
            SetTextBoxText(Date, "14061");
            SetTextBoxText(roomno, "14062");
            SetTextBoxText(roomtemp, "14063");
            SetLabelText(refSpec, "14064");
            SetLabelText(testMethodNo, "14065");
            SetLabelText(jobOrderNo, "14066");
            SetLabelText(worksheetno, "14067");
            SetTextBoxText(roomhumidity, "14068");
            SetLabelText(equipmentno, "14069");

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
            if (dt == null || dt.Rows.Count == 0) return;

            // Simple sample data mapping (corrected types)
            var sampleData = new Dictionary<string, Control>
            {
                { "14072", txtInsulation },
                { "14073", ddlGetCore }
            };

            Dictionary<string, (DropDownList unit, TextBox spec, TextBox test)> fieldMap =
                new Dictionary<string, (DropDownList, TextBox, TextBox)>
                {
                    { "14074", (ddlUnit1, txtSpecifiedValue1, txtMeasured1) },
                    { "14075", (ddlUnit2, txtSpecifiedValue2, txtMeasured2) },
                    { "14076", (ddlUnit3, txtSpecifiedValue3, txtMeasured3) }
                };

            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox[] inputBoxes)>
            {
                { "14077", (ddlUnit1, txtSpecified1_1, new[] { txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3, txtValueTest1_1_4 }) },
                { "14078", (ddlUnit2, txtSpecified1_2, new[] { txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3, txtValueTest1_2_4 }) },
                { "14079", (ddlUnit3, txtSpecified1_3, new[] { txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3, txtValueTest1_3_4 }) },
                { "14080", (ddlUnit4, txtSpecified1_4, new[] { txtValueTest1_4_1, txtValueTest1_4_2, txtValueTest1_4_3, txtValueTest1_4_4 }) }
            };

            var avgMap = new Dictionary<string, Label>
            {
                { "14081", lblAverage1_1 },
                { "14082", lblAverage1_2 },
                { "14083", lblAverage1_3 },
                { "14084", lblAverage1_4 }
            };

            foreach (DataRow row in dt.Rows)
            {
                if (row["ParameterID"] == null || row["Result"] == null) continue;

                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (sampleData.TryGetValue(paramId, out var sampleControl))
                {
                    if (sampleControl is TextBox textBox)
                    {
                        textBox.Text = rawValue;
                    }
                    else if (sampleControl is DropDownList dropDown)
                    {
                        if (dropDown.Items.FindByValue(rawValue) != null)
                        {
                            dropDown.SelectedValue = rawValue;
                        }
                    }
                    continue;
                }

                if (fieldMap.TryGetValue(paramId, out var simpleControls))
                {
                    if (simpleControls.unit == null || simpleControls.spec == null || simpleControls.test == null) continue;

                    string[] parts = rawValue.Split('|');
                    if (parts.Length == 3)
                    {
                        if (simpleControls.unit.Items.FindByValue(parts[0]) != null)
                        {
                            simpleControls.unit.SelectedValue = parts[0];
                        }
                        simpleControls.spec.Text = parts[1];
                        simpleControls.test.Text = parts[2];
                    }
                    continue;
                }

                if (inputMap.TryGetValue(paramId, out var inputControls))
                {
                    if (inputControls.ddlUnit == null || inputControls.txtSpecified == null || inputControls.inputBoxes == null) continue;

                    string[] parts = rawValue.Split('|');
                    if (parts.Length >= 3)
                    {
                        if (inputControls.ddlUnit.Items.FindByValue(parts[0]) != null)
                        {
                            inputControls.ddlUnit.SelectedValue = parts[0];
                        }
                        inputControls.txtSpecified.Text = parts[1];

                        if (parts.Length > 2)
                        {
                            string[] inputs = parts[2].Split(',');
                            for (int i = 0; i < Math.Min(inputs.Length, inputControls.inputBoxes.Length); i++)
                            {
                                if (inputControls.inputBoxes[i] != null)
                                {
                                    inputControls.inputBoxes[i].Text = inputs[i];
                                }
                            }
                        }
                    }
                    continue;
                }

                if (avgMap.TryGetValue(paramId, out var avgControl) && avgControl != null)
                {
                    avgControl.Text = rawValue;
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
                reportParamMapList.Add("14058"); resultList.Add(cableType.Text);
                reportParamMapList.Add("14059"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("14060"); resultList.Add(voltage.Text);
                reportParamMapList.Add("14061"); resultList.Add(Date.Text);
                reportParamMapList.Add("14062"); resultList.Add(roomno.Text);
                reportParamMapList.Add("14063"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("14064"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("14065"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("14066"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("14067"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("14068"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("14069"); resultList.Add(equipmentno.Text);


                reportParamMapList.Add("14072");
                resultList.Add($"{txtInsulation.Text.Trim()}");
                reportParamMapList.Add("14073");
                resultList.Add($"{ddlGetCore.SelectedValue}");

                reportParamMapList.Add("14074");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecifiedValue1.Text.Trim()}|{txtMeasured1.Text.Trim()}");
                reportParamMapList.Add("14075");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecifiedValue2.Text.Trim()}|{txtMeasured2.Text.Trim()}");
                reportParamMapList.Add("14076");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecifiedValue3.Text.Trim()}|{txtMeasured3.Text.Trim()}");

                reportParamMapList.Add("14077");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1_1.Text.Trim()}|{txtValueTest1_1_1.Text.Trim()},{txtValueTest1_1_2.Text.Trim()},{txtValueTest1_1_3.Text.Trim()},{txtValueTest1_1_4.Text.Trim()}");
                reportParamMapList.Add("14078");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified1_2.Text.Trim()}|{txtValueTest1_2_1.Text.Trim()},{txtValueTest1_2_2.Text.Trim()},{txtValueTest1_2_3.Text.Trim()},{txtValueTest1_2_4.Text.Trim()}");
                reportParamMapList.Add("14079");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecified1_3.Text.Trim()}|{txtValueTest1_3_1.Text.Trim()},{txtValueTest1_3_2.Text.Trim()},{txtValueTest1_3_3.Text.Trim()},{txtValueTest1_3_4.Text.Trim()}");
                reportParamMapList.Add("14080");
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecified1_4.Text.Trim()}|{txtValueTest1_4_1.Text.Trim()},{txtValueTest1_4_2.Text.Trim()},{txtValueTest1_4_3.Text.Trim()},{txtValueTest1_4_4.Text.Trim()}");

                reportParamMapList.Add("14081");
                resultList.Add($"{lblAverage1_1.Text.Trim()}");
                reportParamMapList.Add("14082");
                resultList.Add($"{lblAverage1_2.Text.Trim()}");
                reportParamMapList.Add("14083");
                resultList.Add($"{lblAverage1_3.Text.Trim()}");
                reportParamMapList.Add("14084");
                resultList.Add($"{lblAverage1_4.Text.Trim()}");

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

                    Response.Redirect($"ColdElongation_GR.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // Clear Test values
            txtMeasured1.Text = "";
            txtMeasured2.Text = "";
            txtMeasured3.Text = "";
            // Reset DropDowns (set to default or first item)
            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;
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
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID and ReportID = @ReportID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {// Log error and rethrow
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

        protected void TextValue_TextChanged(object sender, EventArgs e)
        {
            CalculateAverages();
        }

        private void CalculateAverages()
        {
            CalculateSingleAverage(txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3, txtValueTest1_1_4, lblAverage1_1);

            CalculateSingleAverage(txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3, txtValueTest1_2_4, lblAverage1_2);

            CalculateSingleAverage(txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3, txtValueTest1_3_4, lblAverage1_3);

            CalculateSingleAverage(txtValueTest1_4_1, txtValueTest1_4_2, txtValueTest1_4_3, txtValueTest1_4_4, lblAverage1_4);
        }

        private void CalculateSingleAverage(TextBox txt1, TextBox txt2, TextBox txt3, TextBox txt4, Label lblMedian)
        {
            double value1, value2, value3, value4;
            double.TryParse(txt1.Text, out value1);
            double.TryParse(txt2.Text, out value2);
            double.TryParse(txt3.Text, out value3);
            double.TryParse(txt4.Text, out value4);

            List<double> values = new List<double> { value1, value2, value3, value4 };

            values.Sort();
            double median;
            int count = values.Count;

            if (count % 2 == 0) // Even number of elements
            {
                median = (values[count / 2 - 1] + values[count / 2]) / 2.0;
            }
            else // Odd number of elements (though not applicable here since we have 4 inputs)
            {
                median = values[count / 2];
            }
            lblMedian.Text = median.ToString("0.00");
        }

        // for Average Calculation //
        /*private void CalculateSingleAverage(TextBox txt1, TextBox txt2, TextBox txt3, TextBox txt4, Label lblAverage)
        {
            double value1, value2, value3, value4;
            double.TryParse(txt1.Text, out value1);
            double.TryParse(txt2.Text, out value2);
            double.TryParse(txt3.Text, out value3);
            double.TryParse(txt4.Text, out value4);
            double average = (value1 + value2 + value3 + value4) / 4;
            lblAverage.Text = average.ToString("0.00");
        }*/


    }
}