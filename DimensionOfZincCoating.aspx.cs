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
	public partial class Zinccoating1 : System.Web.UI.Page
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

          
            SetTextBoxText(cableType, "12598");
            SetTextBoxText(refSpec, "12603");
            SetTextBoxText(cableSize, "12597");
            SetTextBoxText(testMethodNo, "12604");
            SetTextBoxText(voltage, "12599");
            SetTextBoxText(jobOrderNo, "12605");
            SetTextBoxText(worksheetno, "12606");
            SetTextBoxText(roomno, "12601");
            SetTextBoxText(roomhumidity, "12607");
            SetTextBoxText(roomtemp, "12602");
            SetTextBoxText(Date, "12600");
            SetTextBoxText(equipmentno, "12608");

       
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
        { "12593", (ddlDiameter_steelwire1, diameterSpecified, diameterInput1, diameterInput2, diameterInput3) },
        { "12594", (ddlBeforeImmersed, beforeImmersedSpecified, beforeImmersedInput1, beforeImmersedInput2, beforeImmersedInput3) },
        { "12595", (ddlAfterImmersed, afterImmersedSpecified, afterImmersedInput1, afterImmersedInput2, afterImmersedInput3) },
        { "12596", (ddlZincCoating, zincCoatingSpecified, zincCoatingInput1, zincCoatingInput2, zincCoatingInput3) }
    };

            var avgMap = new Dictionary<string, TextBox>
    {
        { "12700", diameterInputAvg },
        { "12701", beforeImmersedInputAvg },
        { "12702", afterImmersedInputAvg },
        { "12703", zincCoatingInputAvg }
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

        protected void Input_TextChanged(object sender, EventArgs e)
        {
         
            CalculateAverage(diameterInput1, diameterInput2, diameterInput3, diameterInputAvg);

          
            CalculateAverage(beforeImmersedInput1, beforeImmersedInput2, beforeImmersedInput3, beforeImmersedInputAvg);

      
            CalculateAverage(afterImmersedInput1, afterImmersedInput2, afterImmersedInput3, afterImmersedInputAvg);

           
            CalculateAverage(zincCoatingInput1, zincCoatingInput2, zincCoatingInput3, zincCoatingInputAvg);
        }

        private void CalculateAverage(TextBox input1, TextBox input2, TextBox input3, TextBox avgTextBox)
        {
            try
            {
                double value1 = 0, value2 = 0, value3 = 0;
                int count = 0;
                double sum = 0;

                // Check and parse first input (treat "-" as 0)
                if (!string.IsNullOrEmpty(input1.Text))
                {
                    if (input1.Text.Trim() == "-")
                    {
                        value1 = 0;
                        sum += value1;
                        count++;
                    }
                    else if (double.TryParse(input1.Text, out value1))
                    {
                        sum += value1;
                        count++;
                    }
                }

       
                if (!string.IsNullOrEmpty(input2.Text))
                {
                    if (input2.Text.Trim() == "-")
                    {
                        value2 = 0;
                        sum += value2;
                        count++;
                    }
                    else if (double.TryParse(input2.Text, out value2))
                    {
                        sum += value2;
                        count++;
                    }
                }

               
                if (!string.IsNullOrEmpty(input3.Text))
                {
                    if (input3.Text.Trim() == "-")
                    {
                        value3 = 0;
                        sum += value3;
                        count++;
                    }
                    else if (double.TryParse(input3.Text, out value3))
                    {
                        sum += value3;
                        count++;
                    }
                }

            
                if (count > 0)
                {
                    double average = sum / count;
                    avgTextBox.Text = average.ToString("F2"); 
                }
                else
                {
                    avgTextBox.Text = ""; 
                }
            }
            catch (Exception ex)
            {
          
                avgTextBox.Text = "Error";
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

                reportParamMapList.Add("12593"); // Diameter of steel wire
                resultList.Add($"{ddlDiameter_steelwire1.SelectedValue.Trim()}|{diameterSpecified.Text.Trim()}|{diameterInput1.Text.Trim()},{diameterInput2.Text.Trim()},{diameterInput3.Text.Trim()}");

                reportParamMapList.Add("12700"); // Diameter of steel wire - Average
                resultList.Add($"{diameterInputAvg.Text.Trim()}");

                // Row 2: Weight / Thickness before immersed
                reportParamMapList.Add("12594"); // Weight / Thickness before immersed
                resultList.Add($"{ddlBeforeImmersed.SelectedValue.Trim()}|{beforeImmersedSpecified.Text.Trim()}|{beforeImmersedInput1.Text.Trim()},{beforeImmersedInput2.Text.Trim()},{beforeImmersedInput3.Text.Trim()}");

                reportParamMapList.Add("12701"); // Weight / Thickness before immersed - Average
                resultList.Add($"{beforeImmersedInputAvg.Text.Trim()}");

                // Row 3: Weight / Thickness after immersed
                reportParamMapList.Add("12595"); // Weight / Thickness after immersed
                resultList.Add($"{ddlAfterImmersed.SelectedValue.Trim()}|{afterImmersedSpecified.Text.Trim()}|{afterImmersedInput1.Text.Trim()},{afterImmersedInput2.Text.Trim()},{afterImmersedInput3.Text.Trim()}");

                reportParamMapList.Add("12702"); // Weight / Thickness after immersed - Average
                resultList.Add($"{afterImmersedInputAvg.Text.Trim()}");


                // Row 4: Weight / Thickness of zinc coating
                reportParamMapList.Add("12596"); // Weight / Thickness of zinc coating
                resultList.Add($"{ddlZincCoating.SelectedValue.Trim()}|{zincCoatingSpecified.Text.Trim()}|{zincCoatingInput1.Text.Trim()},{zincCoatingInput2.Text.Trim()},{zincCoatingInput3.Text.Trim()}");

                reportParamMapList.Add("12703"); // Weight / Thickness of zinc coating - Average
                resultList.Add($"{zincCoatingInputAvg.Text.Trim()}");
                //Upper section

                
                reportParamMapList.Add("12598"); resultList.Add(cableType.Text); // Cable Type
                reportParamMapList.Add("12603"); resultList.Add(refSpec.Text); // Reference Specification
                reportParamMapList.Add("12597"); resultList.Add(cableSize.Text); // Cable Size
                reportParamMapList.Add("12604"); resultList.Add(testMethodNo.Text); // Test Method No
                reportParamMapList.Add("12599"); resultList.Add(voltage.Text); // Voltage
                reportParamMapList.Add("12605"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("12606"); resultList.Add(worksheetno.Text); // Worksheet No
                reportParamMapList.Add("12601"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("12607"); resultList.Add(roomhumidity.Text); // Room Humidity
                reportParamMapList.Add("12602"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("12600"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("12608"); resultList.Add(equipmentno.Text); // Equipment No


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

                    Response.Redirect($"GenerateTestRecord_DimenisonOfzinccoatingTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
          
            ddlDiameter_steelwire1.SelectedIndex = 0;
            diameterSpecified.Text = "";
            diameterInput1.Text = "";
            diameterInput2.Text = "";
            diameterInput3.Text = "";
            diameterInputAvg.Text = "";

  ddlBeforeImmersed.SelectedIndex = 0;
            beforeImmersedSpecified.Text = "";
            beforeImmersedInput1.Text = "";
            beforeImmersedInput2.Text = "";
            beforeImmersedInput3.Text = "";
            beforeImmersedInputAvg.Text = "";

        
            ddlAfterImmersed.SelectedIndex = 0;
            afterImmersedSpecified.Text = "";
            afterImmersedInput1.Text = "";
            afterImmersedInput2.Text = "";
            afterImmersedInput3.Text = "";
            afterImmersedInputAvg.Text = "";

            ddlZincCoating.SelectedIndex = 0;
            zincCoatingSpecified.Text = "";
            zincCoatingInput1.Text = "";
            zincCoatingInput2.Text = "";
            zincCoatingInput3.Text = "";
            zincCoatingInputAvg.Text = "";
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