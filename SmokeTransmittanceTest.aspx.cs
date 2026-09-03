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
    public partial class SmokeTransmittanceTest : System.Web.UI.Page
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
                    roomno.Text = "9";
                    jobOrderNo.Text = "WO/2024020848";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/12/TH/19";
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

                                if (ReportParamMapID == 17131)
                                {
                                    txtSpecifiedValue1.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17132)
                                {
                                    txtSpecifiedValue2.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17133)
                                {
                                    txtSpecifiedValue3.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17134)
                                {
                                    txtSpecifiedValue4.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17135)
                                {
                                    txtSpecifiedValue5.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17136)
                                {
                                    txtSpecifiedValue6.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17137)
                                {
                                    txtSpecifiedValue7.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17138)
                                {
                                    txtSpecifiedValue8.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17139)
                                {
                                    txtSpecifiedValue9.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17140)
                                {
                                    txtSpecifiedValue10.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17141)
                                {
                                    txtSpecifiedValue11.Text = row["Specification"].ToString();
                                }
                                //else if (ReportParamMapID == 16185)
                                //{
                                //    txtSpecifiedValue12.Text = row["Specification"].ToString();
                                //}
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
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "17117");
            SetLabelText(cableSize, "17118");
            SetLabelText(voltage, "17119");
            SetTextBoxText(Date, "17120");
            SetTextBoxText(roomno, "17121");
            SetTextBoxText(roomtemp, "17122");
            SetLabelText(refSpec, "17123");
            SetLabelText(testMethodNo, "17124");
            SetLabelText(jobOrderNo, "17125");
            SetLabelText(worksheetno, "17126");
            SetTextBoxText(roomhumidity, "17127");
            SetLabelText(equipmentno, "17128");

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
                { "17131", (ddlUnit1, txtSpecifiedValue1, txtMeasured1) },
                { "17132", (ddlUnit2, txtSpecifiedValue2, txtMeasured2) },
                { "17133", (ddlUnit3, txtSpecifiedValue3, txtMeasured3) },
                { "17134", (ddlUnit4, txtSpecifiedValue4, txtMeasured4) },
                { "17135", (ddlUnit5, txtSpecifiedValue5, txtMeasured5) },
                { "17136", (ddlUnit6, txtSpecifiedValue6, txtMeasured6) },
                { "17137", (ddlUnit7, txtSpecifiedValue7, txtMeasured7) },
                { "17138", (ddlUnit8, txtSpecifiedValue8, txtMeasured8) },
                { "17139", (ddlUnit9, txtSpecifiedValue9, txtMeasured9) },
                { "17140", (ddlUnit10, txtSpecifiedValue10, txtMeasured10) },
                { "17141", (ddlUnit11, txtSpecifiedValue11, txtMeasured11) }
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

                reportParamMapList.Add("17117"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17118"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17119"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17120"); resultList.Add(Date.Text);
                reportParamMapList.Add("17121"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17122"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17123"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17124"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17125"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17126"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17127"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17128"); resultList.Add(equipmentno.Text);

                reportParamMapList.Add("17131");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecifiedValue1.Text.Trim()}|{txtMeasured1.Text.Trim()}");

                reportParamMapList.Add("17132");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecifiedValue2.Text.Trim()}|{txtMeasured2.Text.Trim()}");

                reportParamMapList.Add("17133");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecifiedValue3.Text.Trim()}|{txtMeasured3.Text.Trim()}");

                reportParamMapList.Add("17134");
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecifiedValue4.Text.Trim()}|{txtMeasured4.Text.Trim()}");

                reportParamMapList.Add("17135");
                resultList.Add($"{ddlUnit5.SelectedValue}|{txtSpecifiedValue5.Text.Trim()}|{txtMeasured5.Text.Trim()}");

                reportParamMapList.Add("17136");
                resultList.Add($"{ddlUnit6.SelectedValue}|{txtSpecifiedValue6.Text.Trim()}|{txtMeasured6.Text.Trim()}");

                reportParamMapList.Add("17137");
                resultList.Add($"{ddlUnit7.SelectedValue}|{txtSpecifiedValue7.Text.Trim()}|{txtMeasured7.Text.Trim()}");

                reportParamMapList.Add("17138");
                resultList.Add($"{ddlUnit8.SelectedValue}|{txtSpecifiedValue8.Text.Trim()}|{txtMeasured8.Text.Trim()}");

                reportParamMapList.Add("17139");
                resultList.Add($"{ddlUnit9.SelectedValue}|{txtSpecifiedValue9.Text.Trim()}|{txtMeasured9.Text.Trim()}");

                reportParamMapList.Add("17140");
                resultList.Add($"{ddlUnit10.SelectedValue}|{txtSpecifiedValue10.Text.Trim()}|{txtMeasured10.Text.Trim()}");

                reportParamMapList.Add("17141");
                resultList.Add($"{ddlUnit11.SelectedValue}|{txtSpecifiedValue11.Text.Trim()}|{txtMeasured11.Text.Trim()}");

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
                    Response.Redirect($"GenerateTestRecord_SmokeTransmittanceTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            txtSpecifiedValue6.Text = "";
            txtSpecifiedValue7.Text = "";
            txtSpecifiedValue8.Text = "";
            txtSpecifiedValue9.Text = "";
            txtSpecifiedValue10.Text = "";
            txtSpecifiedValue11.Text = "";

            // Clear Test values
            txtMeasured1.Text = "";
            txtMeasured2.Text = "";
            txtMeasured3.Text = "";
            txtMeasured4.Text = "";
            txtMeasured5.Text = "";
            txtMeasured6.Text = "";
            txtMeasured7.Text = "";
            txtMeasured8.Text = "";
            txtMeasured9.Text = "";
            txtMeasured10.Text = "";
            txtMeasured11.Text = "";

            // Reset DropDowns (set to default or first item)
            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;
            ddlUnit4.SelectedIndex = 0;
            ddlUnit5.SelectedIndex = 0;
            ddlUnit6.SelectedIndex = 0;
            ddlUnit7.SelectedIndex = 0;
            ddlUnit8.SelectedIndex = 0;
            ddlUnit9.SelectedIndex = 0;
            ddlUnit10.SelectedIndex = 0;
            ddlUnit11.SelectedIndex = 0;
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