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
    public partial class AcidGasEmissionTest : System.Web.UI.Page
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
                {// Set dynamic values from database
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

                                if (ReportParamMapID == 17176)
                                {
                                    txtSpecified1.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17177)
                                {
                                    txtSpecified2.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17178)
                                {
                                    txtSpecified3.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17179)
                                {
                                    txtSpecified4.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17180)
                                {
                                    txtSpecified5.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17181)
                                {
                                    txtSpecified6.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17182)
                                {
                                    txtSpecified7.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17183)
                                {
                                    txtSpecified8.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17184)
                                {
                                    txtSpecified9.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17185)
                                {
                                    txtSpecified10.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17186)
                                {
                                    txtSpecified11.Text = row["Specification"].ToString();
                                }
                                else if (ReportParamMapID == 17187)
                                {
                                    txtSpecified12.Text = row["Specification"].ToString();
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
            SetLabelText(cableType, "17162");
            SetLabelText(cableSize, "17163");
            SetLabelText(voltage, "17164");
            SetTextBoxText(Date, "17165");
            SetTextBoxText(roomno, "17166");
            SetTextBoxText(roomtemp, "17167");
            SetLabelText(refSpec, "17168");
            SetLabelText(testMethodNo, "17169");
            SetLabelText(jobOrderNo, "17170");
            SetLabelText(worksheetno, "17171");
            SetTextBoxText(roomhumidity, "17172");
            SetLabelText(equipmentno, "17173");

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
            var inputMap = new Dictionary<string, (DropDownList ddlUnit, TextBox txtSpecified, TextBox[] txtInputs)>
            {
                // Parameter 1: Length
                { "17176", (ddlUnit1, txtSpecified1, new TextBox[] { txtValueTest1_1, txtValueTest1_2, txtValueTest1_3 }) },
    
                // Parameter 2: Thickness
                { "17177", (ddlUnit2, txtSpecified2, new TextBox[] { txtValueTest2_1, txtValueTest2_2, txtValueTest2_3 }) },
    
                // Parameter 3: Width
                { "17178", (ddlUnit3, txtSpecified3, new TextBox[] { txtValueTest3_1, txtValueTest3_2, txtValueTest3_3 }) },
    
                // Parameter 4: Gas mixture temperature
                { "17179", (ddlUnit4, txtSpecified4, new TextBox[] { txtValueTest4_1, txtValueTest4_2, txtValueTest4_3}) },
    
                // Parameter 5: Mixed gas flow
                { "17180", (ddlUnit5, txtSpecified5, new TextBox[] { txtValueTest5_1, txtValueTest5_2, txtValueTest5_3 }) },
    
                // Parameter 6: Oxygen Percent
                { "17181", (ddlUnit6, txtSpecified6, new TextBox[] { txtValueTest6_1, txtValueTest6_2, txtValueTest6_3 }) },

                { "17182", (ddlUnit7, txtSpecified7, new TextBox[] { txtValueTest7_1, txtValueTest7_2, txtValueTest7_3 }) },

                { "17183", (ddlUnit8, txtSpecified8, new TextBox[] { txtValueTest8_1, txtValueTest8_2, txtValueTest8_3 }) },

                { "17184", (ddlUnit9, txtSpecified9, new TextBox[] { txtValueTest9_1, txtValueTest9_2, txtValueTest9_3 }) },

                { "17185", (ddlUnit10, txtSpecified10, new TextBox[] { txtValueTest10_1, txtValueTest10_2, txtValueTest10_3 }) },

                { "17186", (ddlUnit11, txtSpecified11, new TextBox[] { txtValueTest11_1, txtValueTest11_2, txtValueTest11_3 }) },

                { "17187", (ddlUnit12, txtSpecified12, new TextBox[] { txtValueTest12_1, txtValueTest12_2, txtValueTest12_3 }) }
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"]?.ToString() ?? string.Empty;

                if (inputMap.ContainsKey(paramId))
                {
                    var (ddlUnit, txtSpecified, txtInputs) = inputMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length >= 2) // At least unit and specified value
                    {
                        // Set unit if part exists
                        if (!string.IsNullOrEmpty(parts[0]))
                        {
                            ddlUnit.SelectedValue = parts[0];
                        }

                        // Set specified value if part exists
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                        {
                            txtSpecified.Text = parts[1];
                        }

                        // Set input values if they exist
                        if (parts.Length > 2 && !string.IsNullOrEmpty(parts[2]))
                        {
                            string[] inputs = parts[2].Split(',');
                            for (int i = 0; i < Math.Min(inputs.Length, txtInputs.Length); i++)
                            {
                                txtInputs[i].Text = inputs[i];
                            }
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

                reportParamMapList.Add("17162"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17163"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17164"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17165"); resultList.Add(Date.Text);
                reportParamMapList.Add("17166"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17167"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17168"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17169"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17170"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17171"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17172"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17173"); resultList.Add(equipmentno.Text);
                // Parameters //
                reportParamMapList.Add("17176");
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1.Text.Trim()}|{txtValueTest1_1.Text.Trim()},{txtValueTest1_2.Text.Trim()},{txtValueTest1_3.Text.Trim()}");
                reportParamMapList.Add("17177");
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified2.Text.Trim()}|{txtValueTest2_1.Text.Trim()},{txtValueTest2_2.Text.Trim()},{txtValueTest2_3.Text.Trim()}");
                reportParamMapList.Add("17178");
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecified3.Text.Trim()}|{txtValueTest3_1.Text.Trim()},{txtValueTest3_2.Text.Trim()},{txtValueTest3_3.Text.Trim()}");
                reportParamMapList.Add("17179");
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecified4.Text.Trim()}|{txtValueTest4_1.Text.Trim()},{txtValueTest4_2.Text.Trim()},{txtValueTest4_3.Text.Trim()}");
                reportParamMapList.Add("17180");
                resultList.Add($"{ddlUnit5.SelectedValue}|{txtSpecified5.Text.Trim()}|{txtValueTest5_1.Text.Trim()},{txtValueTest5_2.Text.Trim()},{txtValueTest5_3.Text.Trim()}");
                reportParamMapList.Add("17181");
                resultList.Add($"{ddlUnit6.SelectedValue}|{txtSpecified6.Text.Trim()}|{txtValueTest6_1.Text.Trim()},{txtValueTest6_2.Text.Trim()},{txtValueTest6_3.Text.Trim()}");
                reportParamMapList.Add("17182");
                resultList.Add($"{ddlUnit7.SelectedValue}|{txtSpecified7.Text.Trim()}|{txtValueTest7_1.Text.Trim()},{txtValueTest7_2.Text.Trim()},{txtValueTest7_3.Text.Trim()}");
                reportParamMapList.Add("17183");
                resultList.Add($"{ddlUnit8.SelectedValue}|{txtSpecified8.Text.Trim()}|{txtValueTest8_1.Text.Trim()},{txtValueTest8_2.Text.Trim()},{txtValueTest8_3.Text.Trim()}");
                reportParamMapList.Add("17184");
                resultList.Add($"{ddlUnit9.SelectedValue}|{txtSpecified9.Text.Trim()}|{txtValueTest9_1.Text.Trim()},{txtValueTest9_2.Text.Trim()},{txtValueTest9_3.Text.Trim()}");
                reportParamMapList.Add("17185");
                resultList.Add($"{ddlUnit10.SelectedValue}|{txtSpecified10.Text.Trim()}|{txtValueTest10_1.Text.Trim()},{txtValueTest10_2.Text.Trim()},{txtValueTest10_3.Text.Trim()}");
                reportParamMapList.Add("17186");
                resultList.Add($"{ddlUnit11.SelectedValue}|{txtSpecified11.Text.Trim()}|{txtValueTest11_1.Text.Trim()},{txtValueTest11_2.Text.Trim()},{txtValueTest11_3.Text.Trim()}");
                reportParamMapList.Add("17187");
                resultList.Add($"{ddlUnit12.SelectedValue}|{txtSpecified12.Text.Trim()}|{txtValueTest12_1.Text.Trim()},{txtValueTest12_2.Text.Trim()},{txtValueTest12_3.Text.Trim()}");

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
                    Response.Redirect($"GenerateTestRecord_AcidGasEmissionTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // Clear all text values for Parameter 1 (Length)
            txtSpecified1.Text = "";
            txtValueTest1_1.Text = "";
            txtValueTest1_2.Text = "";
            txtValueTest1_3.Text = "";
            

            // Clear all text values for Parameter 2 (Thickness)
            txtSpecified2.Text = "";
            txtValueTest2_1.Text = "";
            txtValueTest2_2.Text = "";
            txtValueTest2_3.Text = "";

            // Clear all text values for Parameter 3 (Width)
            txtSpecified3.Text = "";
            txtValueTest3_1.Text = "";
            txtValueTest3_2.Text = "";
            txtValueTest3_3.Text = "";

            // Clear all text values for Parameter 4 (Gas mixture temperature)
            txtSpecified4.Text = "";
            txtValueTest4_1.Text = "";
            txtValueTest4_2.Text = "";
            txtValueTest4_3.Text = "";

            // Clear all text values for Parameter 5 (Mixed gas flow)
            txtSpecified5.Text = "";
            txtValueTest5_1.Text = "";
            txtValueTest5_2.Text = "";
            txtValueTest5_3.Text = "";

            // Clear all text values for Parameter 6 (Oxygen Percent)
            txtSpecified6.Text = "";
            txtValueTest6_1.Text = "";
            txtValueTest6_2.Text = "";
            txtValueTest6_3.Text = "";

            // Clear all text values for Parameter 7 (Burning Length)
            txtSpecified7.Text = "";
            txtValueTest7_1.Text = "";
            txtValueTest7_2.Text = "";
            txtValueTest7_3.Text = "";

            // Clear all text values for Parameter 8 (Burning time)
            txtSpecified8.Text = "";
            txtValueTest8_1.Text = "";
            txtValueTest8_2.Text = "";
            txtValueTest8_3.Text = "";

            // Clear all text values for Parameter 9 (Result)
            txtSpecified9.Text = "";
            txtValueTest9_1.Text = "";
            txtValueTest9_2.Text = "";
            txtValueTest9_3.Text = "";

            // Clear all text values for Parameter 10 (Lower oxygen percent for burn the specimen)
            txtSpecified10.Text = "";
            txtValueTest10_1.Text = "";
            txtValueTest10_2.Text = "";
            txtValueTest10_3.Text = "";

            // Reset all dropdowns to their first item
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

            // Clear any validation errors
            foreach (var validator in Page.Validators)
            {
                if (validator is BaseValidator baseValidator)
                {
                    baseValidator.IsValid = true;
                    baseValidator.ErrorMessage = string.Empty;
                }
            }
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