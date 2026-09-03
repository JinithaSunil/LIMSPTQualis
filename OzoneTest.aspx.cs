using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace LIMS
{
	public partial class OzoneTest : System.Web.UI.Page
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
                    else
                    {

                        DataTable dtGetSpecified = _cm.Get_Specified(reportId);
                        if (dtGetSpecified != null && dtGetSpecified.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtGetSpecified.Rows)
                            {
                                if (!row.Table.Columns.Contains("ReportParameter") || !row.Table.Columns.Contains("ReportParaMapId"))
                                {
                                    Debug.WriteLine("Required columns not found in data table");
                                    break;
                                }

                                int reportParaMapId = Convert.ToInt32(row["ReportParaMapId"]);

                                if (reportParaMapId == 16214)
                                {
                                    txtthicknessSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16215)
                                {
                                    txtwidthSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16216)
                                {
                                    txtdiameterSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16217)
                                {
                                    txtlengthsampleSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16218)
                                {
                                    txtgaugelengthSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16219)
                                {
                                    txttemperatureSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16210)
                                {
                                    txtvolumeovenSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16221)
                                {
                                    txtozoneconcentrationSpecified.Text = row["Specification"].ToString();
                                } else if (reportParaMapId == 16223)
                                {
                                    txtozoneconcentrationSpecified2.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16222)
                                {
                                    txtdurationSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16224)
                                {
                                    txtelongateSpecified.Text = row["Specification"].ToString();
                                }
                                else if (reportParaMapId == 16225)
                                {
                                    txtresultSpecified.Text = row["Specification"].ToString();
                                }
                            }

                            Debug.WriteLine("Available Parameters:");
                            foreach (DataRow row in dtGetSpecified.Rows)
                            {
                                if (row.Table.Columns.Contains("ReportParameter"))
                                {
                                    Debug.WriteLine(row["ReportParameter"].ToString());
                                }
                            }
                        }

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


            SetTextBoxText(cableType, "16226");        // Type  
            SetTextBoxText(cableSize, "16227");        // Size  
            SetTextBoxText(voltage, "16228");          // Voltage  
            SetTextBoxText(Date, "16229");             // Date  
            SetTextBoxText(roomno, "16230");           // Room No  
            SetTextBoxText(roomtemp, "16231");         // Room Temperature  
            SetTextBoxText(refSpec, "16232");          // Reference Specification  
            SetTextBoxText(testMethodNo, "16233");     // Test Method No  
            SetTextBoxText(jobOrderNo, "16234");       // Job Order No  
            SetTextBoxText(worksheetno, "16235");      // Worksheet No  
            SetTextBoxText(roomhumidity, "16236");     // Room Humidity  
            SetTextBoxText(equipmentno, "16237");      // Equipment No  
            SetTextBoxText(txtmaterial, "16262");      // Equipment No  


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
        { "16214", (ddlthickness1, txtthicknessSpecified, txtthicknessInput1, txtthicknessInput2, txtthicknessInput3) },      // Thickness
        { "16215", (ddlwidth1, txtwidthSpecified, txtwidthInput1, txtwidthInput2, txtwidthInput3) },                          // Width
        { "16216", (ddldiameter1, txtdiameterSpecified, txtdiameterInput1, txtdiameterInput2, txtdiameterInput3) },            // Diameter
        { "16217", (ddllengthsample1, txtlengthsampleSpecified, txtlengthsampleInput1, txtlengthsampleInput2, txtlengthsampleInput3) }, // LengthOfSample
        { "16218", (ddlgaugelength1, txtgaugelengthSpecified, txtgaugelengthInput1, txtgaugelengthInput2, txtgaugelengthInput3) },       // gaugelength
        { "16219", (ddltemperature1, txttemperatureSpecified, txttemperatureInput1, txttemperatureInput2, txttemperatureInput3) },       // Temperature
        { "16220", (ddlvolumeoven1, txtvolumeovenSpecified, txtvolumeovenInput1, txtvolumeovenInput2, txtvolumeovenInput3) },             // VolumeOven
        { "16221", (ddlozoneconcentration1, txtozoneconcentrationSpecified, txtozoneconcentrationInput1, txtozoneconcentrationInput2, txtozoneconcentrationInput3) }, // OzoneConcentration1
        // For OzoneConcentration2  separate control set:
        { "16223", (ddlozoneconcentration2, txtozoneconcentrationSpecified2, txtozoneconcentrationInput2_1, txtozoneconcentrationInput2_2, txtozoneconcentrationInput2_3) },
        { "16222", (ddlduration1, txtdurationSpecified, txtdurationInput1, txtdurationInput2, txtdurationInput3) },                // Duration
        { "16224", (ddlelongate1, txtelongateSpecified, txtelongateInput1, txtelongateInput2, txtelongateInput3) },                // Elongate
        { "16225", (ddlresult1, txtresultSpecified, txtresultInput1, txtresultInput2, txtresultInput3) }                            // Results
    };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString(); // Use ReportParamMapID here
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
                        if (inputs.Length >= 3)
                        {
                            controls.txtInput1.Text = inputs[0];
                            controls.txtInput2.Text = inputs[1];
                            controls.txtInput3.Text = inputs[2];
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
                // Thickness
                reportParamMapList.Add("16214");
                resultList.Add($"{ddlthickness1.SelectedValue.Trim()}|{txtthicknessSpecified.Text.Trim()}|{txtthicknessInput1.Text.Trim()},{txtthicknessInput2.Text.Trim()},{txtthicknessInput3.Text.Trim()}");

                // Width
                reportParamMapList.Add("16215");
                resultList.Add($"{ddlwidth1.SelectedValue.Trim()}|{txtwidthSpecified.Text.Trim()}|{txtwidthInput1.Text.Trim()},{txtwidthInput2.Text.Trim()},{txtwidthInput3.Text.Trim()}");

                // Diameter
                reportParamMapList.Add("16216");
                resultList.Add($"{ddldiameter1.SelectedValue.Trim()}|{txtdiameterSpecified.Text.Trim()}|{txtdiameterInput1.Text.Trim()},{txtdiameterInput2.Text.Trim()},{txtdiameterInput3.Text.Trim()}");

                // LengthOfSample
                reportParamMapList.Add("16217");
                resultList.Add($"{ddllengthsample1.SelectedValue.Trim()}|{txtlengthsampleSpecified.Text.Trim()}|{txtlengthsampleInput1.Text.Trim()},{txtlengthsampleInput2.Text.Trim()},{txtlengthsampleInput3.Text.Trim()}");

                // GaugeLength
                reportParamMapList.Add("16218");
                resultList.Add($"{ddlgaugelength1.SelectedValue.Trim()}|{txtgaugelengthSpecified.Text.Trim()}|{txtgaugelengthInput1.Text.Trim()},{txtgaugelengthInput2.Text.Trim()},{txtgaugelengthInput3.Text.Trim()}");

                // Temperature
                reportParamMapList.Add("16219");
                resultList.Add($"{ddltemperature1.SelectedValue.Trim()}|{txttemperatureSpecified.Text.Trim()}|{txttemperatureInput1.Text.Trim()},{txttemperatureInput2.Text.Trim()},{txttemperatureInput3.Text.Trim()}");

                // VolumeOven
                reportParamMapList.Add("16220");
                resultList.Add($"{ddlvolumeoven1.SelectedValue.Trim()}|{txtvolumeovenSpecified.Text.Trim()}|{txtvolumeovenInput1.Text.Trim()},{txtvolumeovenInput2.Text.Trim()},{txtvolumeovenInput3.Text.Trim()}");

                // OzoneConcentration1 (first)
                reportParamMapList.Add("16221");
                resultList.Add($"{ddlozoneconcentration1.SelectedValue.Trim()}|{txtozoneconcentrationSpecified.Text.Trim()}|{txtozoneconcentrationInput1.Text.Trim()},{txtozoneconcentrationInput2.Text.Trim()},{txtozoneconcentrationInput3.Text.Trim()}");

                // Duration
                reportParamMapList.Add("16222");
                resultList.Add($"{ddlduration1.SelectedValue.Trim()}|{txtdurationSpecified.Text.Trim()}|{txtdurationInput1.Text.Trim()},{txtdurationInput2.Text.Trim()},{txtdurationInput3.Text.Trim()}");

                // OzoneConcentration1 (second)
                reportParamMapList.Add("16223");
                resultList.Add($"{ddlozoneconcentration2.SelectedValue.Trim()}|{txtozoneconcentrationSpecified2.Text.Trim()}|{txtozoneconcentrationInput2_1.Text.Trim()},{txtozoneconcentrationInput2_2.Text.Trim()},{txtozoneconcentrationInput2_3.Text.Trim()}");

                // Elongate
                reportParamMapList.Add("16224");
                resultList.Add($"{ddlelongate1.SelectedValue.Trim()}|{txtelongateSpecified.Text.Trim()}|{txtelongateInput1.Text.Trim()},{txtelongateInput2.Text.Trim()},{txtelongateInput3.Text.Trim()}");

                // Results
                reportParamMapList.Add("16225");
                resultList.Add($"{ddlresult1.SelectedValue.Trim()}|{txtresultSpecified.Text.Trim()}|{txtresultInput1.Text.Trim()},{txtresultInput2.Text.Trim()},{txtresultInput3.Text.Trim()}");

                reportParamMapList.Add("16226"); resultList.Add(cableType.Text); // Type
                reportParamMapList.Add("16227"); resultList.Add(cableSize.Text); // Size
                reportParamMapList.Add("16228"); resultList.Add(voltage.Text); // Voltage
                reportParamMapList.Add("16229"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("16230"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("16231"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("16232"); resultList.Add(refSpec.Text); // Reference Specification              
                reportParamMapList.Add("16233"); resultList.Add(testMethodNo.Text); // Test Method No               
                reportParamMapList.Add("16234"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("16235"); resultList.Add(worksheetno.Text); // Worksheet No            
                reportParamMapList.Add("16236"); resultList.Add(roomhumidity.Text); // Room Humidity             
                reportParamMapList.Add("16237"); resultList.Add(equipmentno.Text); // Equipment No
                reportParamMapList.Add("16262"); resultList.Add(txtmaterial.Text); // Equipment No



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

                    Response.Redirect($"GenerateTestReport_OzoneTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            ddlthickness1.SelectedIndex = 0;
            txtthicknessSpecified.Text = "";
            txtthicknessInput1.Text = "";
            txtthicknessInput2.Text = "";
            txtthicknessInput3.Text = "";


            ddlwidth1.SelectedIndex = 0;
            txtwidthSpecified.Text = "";
            txtwidthInput1.Text = "";
            txtwidthInput2.Text = "";
            txtwidthInput3.Text = "";



            ddldiameter1.SelectedIndex = 0;
            txtdiameterSpecified.Text = "";
            txtdiameterInput1.Text = "";
            txtdiameterInput2.Text = "";
            txtdiameterInput3.Text = "";


            ddllengthsample1.SelectedIndex = 0;
            txtlengthsampleSpecified.Text = "";
            txtlengthsampleInput1.Text = "";
            txtlengthsampleInput2.Text = "";
            txtlengthsampleInput3.Text = "";



            ddlgaugelength1.SelectedIndex = 0;
            txtgaugelengthSpecified.Text = "";
            txtgaugelengthInput1.Text = "";
            txtgaugelengthInput2.Text = "";
            txtgaugelengthInput3.Text = "";

            ddltemperature1.SelectedIndex = 0;
            txttemperatureSpecified.Text = "";
            txttemperatureInput1.Text = "";
            txttemperatureInput2.Text = "";
            txttemperatureInput3.Text = "";



            ddlvolumeoven1.SelectedIndex = 0;
            txtvolumeovenSpecified.Text = "";
            txtvolumeovenInput1.Text = "";
            txtvolumeovenInput2.Text = "";
            txtvolumeovenInput3.Text = "";



            ddlozoneconcentration1.SelectedIndex = 0;
            txtozoneconcentrationSpecified.Text = "";
            txtozoneconcentrationInput1.Text = "";
            txtozoneconcentrationInput2.Text = "";
            txtozoneconcentrationInput3.Text = "";




            ddlduration1.SelectedIndex = 0;
            txtdiameterSpecified.Text = "";
            txtdurationInput1.Text = "";
            txtdurationInput2.Text = "";
            txtdurationInput3.Text = "";



            ddlelongate1.SelectedIndex = 0;
            txtelongateSpecified.Text = "";
            txtelongateInput1.Text = "";
            txtelongateInput2.Text = "";
            txtelongateInput3.Text = "";


            ddlresult1.SelectedIndex = 0;
            txtresultSpecified.Text = "";
            txtresultInput1.Text = "";
            txtresultInput2.Text = "";
            txtresultInput3.Text = "";
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