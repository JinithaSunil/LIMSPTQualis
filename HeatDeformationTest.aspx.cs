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
    public partial class HeatDeformationTest : System.Web.UI.Page
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
                    else
                    {
                        DataTable dtGetSpecified = _cm.Get_Specified(reportId);

                        if (dtGetSpecified != null && dtGetSpecified.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtGetSpecified.Rows)
                            {
                                int ReportParamMapID = Convert.ToInt32(row["ReportParamMapID"].ToString());

                                if (ReportParamMapID == 16200)
                                {
                                    txtSpecified1_1.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 16201)
                                {
                                    txtSpecified1_2.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 16202)
                                {
                                    txtSpecified1_3.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 16203)
                                {
                                    txtSpecified1_4.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 16204)
                                {
                                    txtSpecified1_5.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 16205)
                                {
                                    txtSpecified1_6.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 16206)
                                {
                                    txtSpecified1_7.Text = row["Specification"].ToString();
                                }
                                
                            }
                        }
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
            SetLabelText(cableType, "16186");
            SetLabelText(cableSize, "16187");
            SetLabelText(voltage, "16188");
            SetTextBoxText(Date, "16189");
            SetTextBoxText(roomno, "16190");
            SetTextBoxText(roomtemp, "16191");
            SetLabelText(refSpec, "16192");
            SetLabelText(testMethodNo, "16193");
            SetLabelText(jobOrderNo, "16194");
            SetLabelText(worksheetno, "16195");
            SetTextBoxText(roomhumidity, "16196");
            SetLabelText(equipmentno, "16197");

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
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox txtInput1, TextBox txtInput2, TextBox txtInput3)>
            {
                { "16200", (ddlUnit1, txtSpecified1_1, txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3) },
                { "16201", (ddlUnit2, txtSpecified1_2, txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3) },
                { "16202", (ddlUnit3, txtSpecified1_3, txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3) },
                { "16203", (ddlUnit4, txtSpecified1_4, txtValueTest1_4_1, txtValueTest1_4_2, txtValueTest1_4_3) },
                { "16204", (ddlUnit5, txtSpecified1_5, txtValueTest1_5_1, txtValueTest1_5_2, txtValueTest1_5_3) },
                { "16205", (ddlUnit6, txtSpecified1_6, txtValueTest1_6_1, txtValueTest1_6_2, txtValueTest1_6_3) },
                { "16206", (ddlUnit7, txtSpecified1_7, txtValueTest1_7_1, txtValueTest1_7_2, txtValueTest1_7_3) }
            };

            var avgMap = new Dictionary<string, Label>
            {
                { "16207", lblAverage1_1 },
                { "16208", lblAverage1_2 },
                { "16209", lblAverage1_3 },
                { "16210", lblAverage1_4 },
                { "16211", lblAverage1_5 },
                { "16212", lblAverage1_6 },
                { "16213", lblAverage1_7 }
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

                        if (inputs.Length == 3)
                        {
                            controls.txtInput1.Text = inputs[0];
                            controls.txtInput2.Text = inputs[1];
                            controls.txtInput3.Text = inputs[2];
                        }
                    }
                }
                else if (avgMap.ContainsKey(paramId))
                {
                    avgMap[paramId].Text = rawValue;
                }
            }
        }

        protected void TextValue_TextChanged(object sender, EventArgs e)
        {
            CalculateAverages();
        }

        private void CalculateAverages()
        {
            CalculateSingleAverage(txtValueTest1_1_1, txtValueTest1_1_2, txtValueTest1_1_3,lblAverage1_1);
            CalculateSingleAverage(txtValueTest1_2_1, txtValueTest1_2_2, txtValueTest1_2_3,lblAverage1_2);
            CalculateSingleAverage(txtValueTest1_3_1, txtValueTest1_3_2, txtValueTest1_3_3,lblAverage1_3);
            CalculateSingleAverage(txtValueTest1_4_1, txtValueTest1_4_2, txtValueTest1_4_3,lblAverage1_4);
            CalculateSingleAverage(txtValueTest1_5_1, txtValueTest1_5_2, txtValueTest1_5_3, lblAverage1_5);
            CalculateSingleAverage(txtValueTest1_6_1, txtValueTest1_6_2, txtValueTest1_6_3, lblAverage1_6);
            CalculateSingleAverage(txtValueTest1_7_1, txtValueTest1_7_2, txtValueTest1_7_3, lblAverage1_7);
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
                reportParamMapList.Add("16186"); resultList.Add(cableType.Text);
                reportParamMapList.Add("16187"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("16188"); resultList.Add(voltage.Text);
                reportParamMapList.Add("16189"); resultList.Add(Date.Text);
                reportParamMapList.Add("16190"); resultList.Add(roomno.Text);
                reportParamMapList.Add("16191"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("16192"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("16193"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("16194"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("16195"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("16196"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("16197"); resultList.Add(equipmentno.Text);
                // Original 4 parameters
                reportParamMapList.Add("16200");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1_1.Text.Trim()}|{txtValueTest1_1_1.Text.Trim()},{txtValueTest1_1_2.Text.Trim()},{txtValueTest1_1_3.Text.Trim()}");
                reportParamMapList.Add("16201");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified1_2.Text.Trim()}|{txtValueTest1_2_1.Text.Trim()},{txtValueTest1_2_2.Text.Trim()},{txtValueTest1_2_3.Text.Trim()}");
                reportParamMapList.Add("16202");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecified1_3.Text.Trim()}|{txtValueTest1_3_1.Text.Trim()},{txtValueTest1_3_2.Text.Trim()},{txtValueTest1_3_3.Text.Trim()}");
                reportParamMapList.Add("16203");
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecified1_4.Text.Trim()}|{txtValueTest1_4_1.Text.Trim()},{txtValueTest1_4_2.Text.Trim()},{txtValueTest1_4_3.Text.Trim()}");
                reportParamMapList.Add("16204");
                resultList.Add($"{ddlUnit5.SelectedValue}|{txtSpecified1_5.Text.Trim()}|{txtValueTest1_5_1.Text.Trim()},{txtValueTest1_5_2.Text.Trim()},{txtValueTest1_5_3.Text.Trim()}");
                reportParamMapList.Add("16205");
                resultList.Add($"{ddlUnit6.SelectedValue}|{txtSpecified1_6.Text.Trim()}|{txtValueTest1_6_1.Text.Trim()},{txtValueTest1_6_2.Text.Trim()},{txtValueTest1_6_3.Text.Trim()}");
                reportParamMapList.Add("16206");
                resultList.Add($"{ddlUnit7.SelectedValue}|{txtSpecified1_7.Text.Trim()}|{txtValueTest1_7_1.Text.Trim()},{txtValueTest1_7_2.Text.Trim()},{txtValueTest1_7_3.Text.Trim()}");
                // Averages for all parameters
                reportParamMapList.Add("16207");
                resultList.Add($"{lblAverage1_1.Text.Trim()}");
                reportParamMapList.Add("16208");
                resultList.Add($"{lblAverage1_2.Text.Trim()}");
                reportParamMapList.Add("16209");
                resultList.Add($"{lblAverage1_3.Text.Trim()}");
                reportParamMapList.Add("16210");
                resultList.Add($"{lblAverage1_4.Text.Trim()}");
                reportParamMapList.Add("16211");
                resultList.Add($"{lblAverage1_5.Text.Trim()}");
                reportParamMapList.Add("16212");
                resultList.Add($"{lblAverage1_6.Text.Trim()}");
                reportParamMapList.Add("16213");
                resultList.Add($"{lblAverage1_7.Text.Trim()}");

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
                    Response.Redirect($"GenerateTestRecord_HeatDeformationTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

            // Clear Cross Section Area values
            txtValueTest1_4_1.Text = "";
            txtValueTest1_4_2.Text = "";
            txtValueTest1_4_3.Text = "";
            lblAverage1_4.Text = "";

            // Reset DropDowns
            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;
            //ddlTestResult.SelectedIndex = 0;
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