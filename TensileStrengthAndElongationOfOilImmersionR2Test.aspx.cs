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
    public partial class TensileStrengthAndElongationOfOilImmersionR2Test : System.Web.UI.Page
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
            SetLabelText(cableType, "17414");
            SetLabelText(cableSize, "17415");
            SetLabelText(voltage, "17416");
            SetTextBoxText(Date, "17417");
            SetTextBoxText(roomno, "17418");
            SetTextBoxText(roomtemp, "17419");
            SetLabelText(refSpec, "17420");
            SetLabelText(testMethodNo, "17421");
            SetLabelText(jobOrderNo, "17422");
            SetLabelText(worksheetno, "17423");
            SetTextBoxText(roomhumidity, "17424");
            SetLabelText(equipmentno, "17425");

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
            // FIXED: inputMap0 now expects only a single TextBox in the tuple
            var inputMap0 = new Dictionary<string, TextBox>()
            {
                { "17428", txtCore },
                { "17429", txtSpecimenType },
                { "17458", txtBDate },
                { "17459", txtADate },
            };

            var inputMap1 = new Dictionary<string, (TextBox txtEquipment, TextBox txtInput1, TextBox txtInput2, TextBox txtInput3, TextBox txtInput4, TextBox txtInput5, TextBox txtSpecified)>()
            {
                { "17431", (txtEquipNo2, txtMeasuredValue1_1, txtMeasuredValue1_2, txtMeasuredValue1_3, txtMeasuredValue1_4, txtMeasuredValue1_5, txtSpecified2) },
                { "17432", (txtEquipNo3, txtMeasuredValue2_1, txtMeasuredValue2_2, txtMeasuredValue2_3, txtMeasuredValue2_4, txtMeasuredValue2_5, txtSpecified3) },
                { "17433", (txtEquipNo4, txtMeasuredValue3_1, txtMeasuredValue3_2, txtMeasuredValue3_3, txtMeasuredValue3_4, txtMeasuredValue3_5, txtSpecified4) },
                { "17434", (txtEquipNo5, txtMeasuredValue4_1, txtMeasuredValue4_2, txtMeasuredValue4_3, txtMeasuredValue4_4, txtMeasuredValue4_5, txtSpecified5) },
                { "17435", (txtEquipNo5, txtMeasuredValue5_1, txtMeasuredValue5_2, txtMeasuredValue5_3, txtMeasuredValue5_4, txtMeasuredValue5_5, txtSpecified6) },
                { "17436", (txtEquipNo5, txtMeasuredValue6_1, txtMeasuredValue6_2, txtMeasuredValue6_3, txtMeasuredValue6_4, txtMeasuredValue6_5, txtSpecified7) },
                { "17437", (txtEquipNo5, txtMeasuredValue7_1, txtMeasuredValue7_2, txtMeasuredValue7_3, txtMeasuredValue7_4, txtMeasuredValue7_5, txtSpecified8) },
                { "17438", (txtEquipNo5, txtMeasuredValue8_1, txtMeasuredValue8_2, txtMeasuredValue8_3, txtMeasuredValue8_4, txtMeasuredValue8_5, txtSpecified9) },

                { "17448", (txtEquipNo10, txtMeasuredValue9_1, txtMeasuredValue9_2, txtMeasuredValue9_3, txtMeasuredValue9_4, txtMeasuredValue9_5, txtSpecified18) },
                { "17449", (txtEquipNo11, txtMeasuredValue10_1, txtMeasuredValue10_2, txtMeasuredValue10_3, txtMeasuredValue10_4, txtMeasuredValue10_5, txtSpecified19) },
                { "17450", (txtEquipNo12, txtMeasuredValue11_1, txtMeasuredValue11_2, txtMeasuredValue11_3, txtMeasuredValue11_4, txtMeasuredValue11_5, txtSpecified20) },
                { "17451", (txtEquipNo13, txtMeasuredValue12_1, txtMeasuredValue12_2, txtMeasuredValue12_3, txtMeasuredValue12_4, txtMeasuredValue12_5, txtSpecified21) },
                { "17452", (txtEquipNo13, txtMeasuredValue13_1, txtMeasuredValue13_2, txtMeasuredValue13_3, txtMeasuredValue13_4, txtMeasuredValue13_5, txtSpecified22) },
                { "17453", (txtEquipNo13, txtMeasuredValue14_1, txtMeasuredValue14_2, txtMeasuredValue14_3, txtMeasuredValue14_4, txtMeasuredValue14_5, txtSpecified23) },
                { "17454", (txtEquipNo13, txtMeasuredValue15_1, txtMeasuredValue15_2, txtMeasuredValue15_3, txtMeasuredValue15_4, txtMeasuredValue15_5, txtSpecified24) },
                { "17455", (txtEquipNo13, txtMeasuredValue16_1, txtMeasuredValue16_2, txtMeasuredValue16_3, txtMeasuredValue16_4, txtMeasuredValue16_5, txtSpecified25) },
                { "17456", (txtEquipNo14, txtMeasuredValue17_1, txtMeasuredValue17_2, txtMeasuredValue17_3, txtMeasuredValue17_4, txtMeasuredValue17_5, txtSpecified26) },
                { "17457", (txtEquipNo15, txtMeasuredValue18_1, txtMeasuredValue18_2, txtMeasuredValue18_3, txtMeasuredValue18_4, txtMeasuredValue18_5, txtSpecified27) }
            };

            var singleInputMap = new Dictionary<string, (TextBox txtEquipment, TextBox txtInput, TextBox txtSpecified)>()
            {
                { "17430", (txtEquipNo1, txtRoomTemp1, txtSpecified1) },
                { "17439", (txtEquipNo6, txtTemp1, txtSpecified10) },
                { "17443", (txtEquipNo8, txtConditioningRoomTemp, txtSpecified14) },
            };

            var singleInputMap1 = new Dictionary<string, (TextBox txtEquipment, TextBox txtInput1, TextBox txtInput2, TextBox txtSpecified)>()
            {
                { "17440", (txtEquipNo7, txtH1, txtH2, txtSpecified11) },
                { "17441", (txtEquipNo7, txtH3, txtH4, txtSpecified13) },
                { "17442", (txtEquipNo7, txtStartDate1, txtEndDate1, txtSpecified12) },

                { "17444", (txtEquipNo9, txtConditioningTime1, txtConditioningTime2, txtSpecified15) },
                { "17445", (txtEquipNo9, txtConditioningTime3, txtConditioningTime4, txtSpecified17) },
                { "17446", (txtEquipNo9, txtConditioningDate1, txtConditioningDate2, txtSpecified16) },
            };

            var avgMap = new Dictionary<string, Label>()
            {
                { "17462", lblMedian1 },
                { "17463", lblMedian2 },
                { "17464", lblMedian3 },
                { "17465", lblMedian4 },
                { "17466", lblMedian5 },
                { "17467", lblMedian6 },
                { "17468", lblMedian7 },
                { "17469", lblMedian8 },
                { "17470", lblMedian9 },
                { "17471", lblMedian10 },
                { "17472", lblMedian11 },
                { "17473", lblMedian12 },
                { "17474", lblMedian13 },
                { "17475", lblMedian14 },
                { "17476", lblMedian15 },
                { "17477", lblMedian16 },
                { "17478", lblMedian17 },
                { "17479", lblMedian18 }
            };

            // Debug logging to see what data is being retrieved
            System.Diagnostics.Debug.WriteLine("=== PrefillTestResultsGrid Data ===");
            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"]?.ToString();
                string rawValue = row["Result"]?.ToString();
                System.Diagnostics.Debug.WriteLine($"ParamID: {paramId}, Value: {rawValue}");
            }
            System.Diagnostics.Debug.WriteLine("=== End of Data ===");

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"]?.ToString();
                string rawValue = row["Result"]?.ToString();

                if (string.IsNullOrEmpty(paramId) || string.IsNullOrEmpty(rawValue))
                    continue;

                // Prefill Input Controls with single value (inputMap0)
                if (inputMap0.ContainsKey(paramId))
                {
                    var control = inputMap0[paramId];
                    control.Text = rawValue;
                    System.Diagnostics.Debug.WriteLine($"Set inputMap0[{paramId}] = {rawValue}");
                }
                // Prefill Input Controls with multiple values
                else if (inputMap1.ContainsKey(paramId))
                {
                    var controls = inputMap1[paramId];
                    string[] parts = rawValue.Split('|');

                    if (parts.Length >= 3)
                    {
                        controls.txtEquipment.Text = parts[0];
                        controls.txtSpecified.Text = parts[1];

                        string[] inputs = parts[2].Split(',');
                        TextBox[] textInputs = { controls.txtInput1, controls.txtInput2, controls.txtInput3, controls.txtInput4, controls.txtInput5 };

                        for (int i = 0; i < inputs.Length && i < textInputs.Length; i++)
                        {
                            textInputs[i].Text = inputs[i];
                        }
                        System.Diagnostics.Debug.WriteLine($"Set inputMap1[{paramId}] = Equipment: {parts[0]}, Specified: {parts[1]}, Values: {parts[2]}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Warning: inputMap1[{paramId}] has invalid format: {rawValue}");
                    }
                }
                // Prefill single input controls with equipment
                else if (singleInputMap.ContainsKey(paramId))
                {
                    var controls = singleInputMap[paramId];
                    string[] parts = rawValue.Split('|');

                    if (parts.Length >= 3)
                    {
                        controls.txtEquipment.Text = parts[0];
                        controls.txtSpecified.Text = parts[1];
                        controls.txtInput.Text = parts[2];
                        System.Diagnostics.Debug.WriteLine($"Set singleInputMap[{paramId}] = Equipment: {parts[0]}, Specified: {parts[1]}, Value: {parts[2]}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Warning: singleInputMap[{paramId}] has invalid format: {rawValue}");
                    }
                }
                // Prefill dual input controls with equipment - CORRECTED
                else if (singleInputMap1.ContainsKey(paramId))
                {
                    var controls = singleInputMap1[paramId];
                    string[] parts = rawValue.Split('|');

                    if (parts.Length >= 3) // Changed from >= 4 to >= 3
                    {
                        controls.txtEquipment.Text = parts[0];
                        controls.txtSpecified.Text = parts[1];

                        // Split the third part by comma for the two values
                        string[] values = parts[2].Split(',');
                        if (values.Length >= 2)
                        {
                            controls.txtInput1.Text = values[0];
                            controls.txtInput2.Text = values[1];
                        }
                        else if (values.Length == 1)
                        {
                            controls.txtInput1.Text = values[0];
                        }
                        System.Diagnostics.Debug.WriteLine($"Set singleInputMap1[{paramId}] = Equipment: {parts[0]}, Specified: {parts[1]}, Values: {parts[2]}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Warning: singleInputMap1[{paramId}] has invalid format: {rawValue}");
                    }
                }
                // Prefill Average Labels
                else if (avgMap.ContainsKey(paramId))
                {
                    avgMap[paramId].Text = rawValue;
                    System.Diagnostics.Debug.WriteLine($"Set avgMap[{paramId}] = {rawValue}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: ParameterID {paramId} not found in any mapping");
                }
            }
        }

        protected void TextValue_TextChanged(object sender, EventArgs e)
        {
            CalculateMedians();
        }

        private void CalculateMedians()
        {
            CalculateRowMedian(txtMeasuredValue1_1, txtMeasuredValue1_2, txtMeasuredValue1_3, txtMeasuredValue1_4, txtMeasuredValue1_5, lblMedian1);
            CalculateRowMedian(txtMeasuredValue2_1, txtMeasuredValue2_2, txtMeasuredValue2_3, txtMeasuredValue2_4, txtMeasuredValue2_5, lblMedian2);
            CalculateRowMedian(txtMeasuredValue3_1, txtMeasuredValue3_2, txtMeasuredValue3_3, txtMeasuredValue3_4, txtMeasuredValue3_5, lblMedian3);
            CalculateRowMedian(txtMeasuredValue4_1, txtMeasuredValue4_2, txtMeasuredValue4_3, txtMeasuredValue4_4, txtMeasuredValue4_5, lblMedian4);
            CalculateRowMedian(txtMeasuredValue5_1, txtMeasuredValue5_2, txtMeasuredValue5_3, txtMeasuredValue5_4, txtMeasuredValue5_5, lblMedian5);
            CalculateRowMedian(txtMeasuredValue6_1, txtMeasuredValue6_2, txtMeasuredValue6_3, txtMeasuredValue6_4, txtMeasuredValue6_5, lblMedian6);
            CalculateRowMedian(txtMeasuredValue7_1, txtMeasuredValue7_2, txtMeasuredValue7_3, txtMeasuredValue7_4, txtMeasuredValue7_5, lblMedian7);
            CalculateRowMedian(txtMeasuredValue8_1, txtMeasuredValue8_2, txtMeasuredValue8_3, txtMeasuredValue8_4, txtMeasuredValue8_5, lblMedian8);
            CalculateRowMedian(txtMeasuredValue9_1, txtMeasuredValue9_2, txtMeasuredValue9_3, txtMeasuredValue9_4, txtMeasuredValue9_5, lblMedian9);
            CalculateRowMedian(txtMeasuredValue10_1, txtMeasuredValue10_2, txtMeasuredValue10_3, txtMeasuredValue10_4, txtMeasuredValue10_5, lblMedian10);
            CalculateRowMedian(txtMeasuredValue11_1, txtMeasuredValue11_2, txtMeasuredValue11_3, txtMeasuredValue11_4, txtMeasuredValue11_5, lblMedian11);
            CalculateRowMedian(txtMeasuredValue12_1, txtMeasuredValue12_2, txtMeasuredValue12_3, txtMeasuredValue12_4, txtMeasuredValue12_5, lblMedian12);
            CalculateRowMedian(txtMeasuredValue13_1, txtMeasuredValue13_2, txtMeasuredValue13_3, txtMeasuredValue13_4, txtMeasuredValue13_5, lblMedian13);
            CalculateRowMedian(txtMeasuredValue14_1, txtMeasuredValue14_2, txtMeasuredValue14_3, txtMeasuredValue14_4, txtMeasuredValue14_5, lblMedian14);
            CalculateRowMedian(txtMeasuredValue15_1, txtMeasuredValue15_2, txtMeasuredValue15_3, txtMeasuredValue15_4, txtMeasuredValue15_5, lblMedian15);
            CalculateRowMedian(txtMeasuredValue16_1, txtMeasuredValue16_2, txtMeasuredValue16_3, txtMeasuredValue16_4, txtMeasuredValue16_5, lblMedian16);
            CalculateRowMedian(txtMeasuredValue17_1, txtMeasuredValue17_2, txtMeasuredValue17_3, txtMeasuredValue17_4, txtMeasuredValue17_5, lblMedian17);
            CalculateRowMedian(txtMeasuredValue18_1, txtMeasuredValue18_2, txtMeasuredValue18_3, txtMeasuredValue18_4, txtMeasuredValue18_5, lblMedian18);
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


                reportParamMapList.Add("17414"); resultList.Add(cableType.Text); 
                reportParamMapList.Add("17415"); resultList.Add(cableSize.Text); 
                reportParamMapList.Add("17416"); resultList.Add(voltage.Text); 
                reportParamMapList.Add("17417"); resultList.Add(Date.Text); 
                reportParamMapList.Add("17418"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17419"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17420"); resultList.Add(refSpec.Text); 
                reportParamMapList.Add("17421"); resultList.Add(testMethodNo.Text); 
                reportParamMapList.Add("17422"); resultList.Add(jobOrderNo.Text); 
                reportParamMapList.Add("17423"); resultList.Add(worksheetno.Text); 
                reportParamMapList.Add("17424"); resultList.Add(roomhumidity.Text); 
                reportParamMapList.Add("17425"); resultList.Add(equipmentno.Text);

                // -----------------------
                // Basic information (matches inputMap0)
                // -----------------------
                reportParamMapList.Add("17428"); resultList.Add(txtCore.Text);
                reportParamMapList.Add("17429"); resultList.Add(txtSpecimenType.Text);
                reportParamMapList.Add("17458"); resultList.Add(txtBDate.Text);
                reportParamMapList.Add("17459"); resultList.Add(txtADate.Text);

                // -----------------------
                // Multi-input groups (matches inputMap1)
                // -----------------------
                reportParamMapList.Add("17431");
                resultList.Add($"{txtEquipNo2.Text}|{txtSpecified2.Text}|{txtMeasuredValue1_1.Text},{txtMeasuredValue1_2.Text},{txtMeasuredValue1_3.Text},{txtMeasuredValue1_4.Text},{txtMeasuredValue1_5.Text}");

                reportParamMapList.Add("17432");
                resultList.Add($"{txtEquipNo3.Text}|{txtSpecified3.Text}|{txtMeasuredValue2_1.Text},{txtMeasuredValue2_2.Text},{txtMeasuredValue2_3.Text},{txtMeasuredValue2_4.Text},{txtMeasuredValue2_5.Text}");

                reportParamMapList.Add("17433");
                resultList.Add($"{txtEquipNo4.Text}|{txtSpecified4.Text}|{txtMeasuredValue3_1.Text},{txtMeasuredValue3_2.Text},{txtMeasuredValue3_3.Text},{txtMeasuredValue3_4.Text},{txtMeasuredValue3_5.Text}");

                reportParamMapList.Add("17434");
                resultList.Add($"{txtEquipNo5.Text}|{txtSpecified5.Text}|{txtMeasuredValue4_1.Text},{txtMeasuredValue4_2.Text},{txtMeasuredValue4_3.Text},{txtMeasuredValue4_4.Text},{txtMeasuredValue4_5.Text}");

                reportParamMapList.Add("17435");
                resultList.Add($"{txtEquipNo5.Text}|{txtSpecified6.Text}|{txtMeasuredValue5_1.Text},{txtMeasuredValue5_2.Text},{txtMeasuredValue5_3.Text},{txtMeasuredValue5_4.Text},{txtMeasuredValue5_5.Text}");

                reportParamMapList.Add("17436");
                resultList.Add($"{txtEquipNo5.Text}|{txtSpecified7.Text}|{txtMeasuredValue6_1.Text},{txtMeasuredValue6_2.Text},{txtMeasuredValue6_3.Text},{txtMeasuredValue6_4.Text},{txtMeasuredValue6_5.Text}");

                reportParamMapList.Add("17437");
                resultList.Add($"{txtEquipNo5.Text}|{txtSpecified8.Text}|{txtMeasuredValue7_1.Text},{txtMeasuredValue7_2.Text},{txtMeasuredValue7_3.Text},{txtMeasuredValue7_4.Text},{txtMeasuredValue7_5.Text}");

                reportParamMapList.Add("17438");
                resultList.Add($"{txtEquipNo5.Text}|{txtSpecified9.Text}|{txtMeasuredValue8_1.Text},{txtMeasuredValue8_2.Text},{txtMeasuredValue8_3.Text},{txtMeasuredValue8_4.Text},{txtMeasuredValue8_5.Text}");

                // -----------------------
                // Single input (matches singleInputMap)
                // -----------------------
                reportParamMapList.Add("17430");
                resultList.Add($"{txtEquipNo1.Text}|{txtSpecified1.Text}|{txtRoomTemp1.Text}");

                reportParamMapList.Add("17439");
                resultList.Add($"{txtEquipNo6.Text}|{txtSpecified10.Text}|{txtTemp1.Text}");

                reportParamMapList.Add("17443");
                resultList.Add($"{txtEquipNo8.Text}|{txtSpecified14.Text}|{txtConditioningRoomTemp.Text}");

                // -----------------------
                // Dual input (matches singleInputMap1)
                // -----------------------
                reportParamMapList.Add("17440");
                resultList.Add($"{txtEquipNo7.Text}|{txtSpecified11.Text}|{txtH1.Text},{txtH2.Text}");

                reportParamMapList.Add("17441");
                resultList.Add($"{txtEquipNo7.Text}|{txtSpecified13.Text}|{txtH3.Text},{txtH4.Text}");

                reportParamMapList.Add("17442");
                resultList.Add($"{txtEquipNo7.Text}|{txtSpecified12.Text}|{txtStartDate1.Text},{txtEndDate1.Text}");

                reportParamMapList.Add("17444");
                resultList.Add($"{txtEquipNo9.Text}|{txtSpecified15.Text}|{txtConditioningTime1.Text},{txtConditioningTime2.Text}");

                reportParamMapList.Add("17445");
                resultList.Add($"{txtEquipNo9.Text}|{txtSpecified17.Text}|{txtConditioningTime3.Text},{txtConditioningTime4.Text}");

                reportParamMapList.Add("17446");
                resultList.Add($"{txtEquipNo9.Text}|{txtSpecified16.Text}|{txtConditioningDate1.Text},{txtConditioningDate2.Text}");

                // -----------------------
                // TS & EL After Section (matches inputMap1 continuation)
                // -----------------------
                reportParamMapList.Add("17448");
                resultList.Add($"{txtEquipNo10.Text}|{txtSpecified18.Text}|{txtMeasuredValue9_1.Text},{txtMeasuredValue9_2.Text},{txtMeasuredValue9_3.Text},{txtMeasuredValue9_4.Text},{txtMeasuredValue9_5.Text}");

                reportParamMapList.Add("17449");
                resultList.Add($"{txtEquipNo11.Text}|{txtSpecified19.Text}|{txtMeasuredValue10_1.Text},{txtMeasuredValue10_2.Text},{txtMeasuredValue10_3.Text},{txtMeasuredValue10_4.Text},{txtMeasuredValue10_5.Text}");

                reportParamMapList.Add("17450");
                resultList.Add($"{txtEquipNo12.Text}|{txtSpecified20.Text}|{txtMeasuredValue11_1.Text},{txtMeasuredValue11_2.Text},{txtMeasuredValue11_3.Text},{txtMeasuredValue11_4.Text},{txtMeasuredValue11_5.Text}");

                reportParamMapList.Add("17451");
                resultList.Add($"{txtEquipNo13.Text}|{txtSpecified21.Text}|{txtMeasuredValue12_1.Text},{txtMeasuredValue12_2.Text},{txtMeasuredValue12_3.Text},{txtMeasuredValue12_4.Text},{txtMeasuredValue12_5.Text}");

                reportParamMapList.Add("17452");
                resultList.Add($"{txtEquipNo13.Text}|{txtSpecified22.Text}|{txtMeasuredValue13_1.Text},{txtMeasuredValue13_2.Text},{txtMeasuredValue13_3.Text},{txtMeasuredValue13_4.Text},{txtMeasuredValue13_5.Text}");

                reportParamMapList.Add("17453");
                resultList.Add($"{txtEquipNo13.Text}|{txtSpecified23.Text}|{txtMeasuredValue14_1.Text},{txtMeasuredValue14_2.Text},{txtMeasuredValue14_3.Text},{txtMeasuredValue14_4.Text},{txtMeasuredValue14_5.Text}");

                reportParamMapList.Add("17454");
                resultList.Add($"{txtEquipNo13.Text}|{txtSpecified24.Text}|{txtMeasuredValue15_1.Text},{txtMeasuredValue15_2.Text},{txtMeasuredValue15_3.Text},{txtMeasuredValue15_4.Text},{txtMeasuredValue15_5.Text}");

                reportParamMapList.Add("17455");
                resultList.Add($"{txtEquipNo13.Text}|{txtSpecified25.Text}|{txtMeasuredValue16_1.Text},{txtMeasuredValue16_2.Text},{txtMeasuredValue16_3.Text},{txtMeasuredValue16_4.Text},{txtMeasuredValue16_5.Text}");

                reportParamMapList.Add("17456");
                resultList.Add($"{txtEquipNo14.Text}|{txtSpecified26.Text}|{txtMeasuredValue17_1.Text},{txtMeasuredValue17_2.Text},{txtMeasuredValue17_3.Text},{txtMeasuredValue17_4.Text},{txtMeasuredValue17_5.Text}");

                reportParamMapList.Add("17457");
                resultList.Add($"{txtEquipNo15.Text}|{txtSpecified27.Text}|{txtMeasuredValue18_1.Text},{txtMeasuredValue18_2.Text},{txtMeasuredValue18_3.Text},{txtMeasuredValue18_4.Text},{txtMeasuredValue18_5.Text}");

                // -----------------------
                // Median values (matches avgMap)
                // -----------------------
                reportParamMapList.Add("17462"); resultList.Add(lblMedian1.Text);
                reportParamMapList.Add("17463"); resultList.Add(lblMedian2.Text);
                reportParamMapList.Add("17464"); resultList.Add(lblMedian3.Text);
                reportParamMapList.Add("17465"); resultList.Add(lblMedian4.Text);
                reportParamMapList.Add("17466"); resultList.Add(lblMedian5.Text);
                reportParamMapList.Add("17467"); resultList.Add(lblMedian6.Text);
                reportParamMapList.Add("17468"); resultList.Add(lblMedian7.Text);
                reportParamMapList.Add("17469"); resultList.Add(lblMedian8.Text);
                reportParamMapList.Add("17470"); resultList.Add(lblMedian9.Text);
                reportParamMapList.Add("17471"); resultList.Add(lblMedian10.Text);
                reportParamMapList.Add("17472"); resultList.Add(lblMedian11.Text);
                reportParamMapList.Add("17473"); resultList.Add(lblMedian12.Text);
                reportParamMapList.Add("17474"); resultList.Add(lblMedian13.Text);
                reportParamMapList.Add("17475"); resultList.Add(lblMedian14.Text);
                reportParamMapList.Add("17476"); resultList.Add(lblMedian15.Text);
                reportParamMapList.Add("17477"); resultList.Add(lblMedian16.Text);
                reportParamMapList.Add("17478"); resultList.Add(lblMedian17.Text);
                reportParamMapList.Add("17479"); resultList.Add(lblMedian18.Text);

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
                    Response.Redirect($"GenerateTestRecord_TensileStrengthAndElongationOfOilImmersionR2Test.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            try
            {
                //// Clear basic information
                //Date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                //roomno.Text = "";
                //roomhumidity.Text = "";
                //roomtemp.Text = "";

                // Clear all equipment number fields
                txtEquipNo1.Text = "";
                txtEquipNo2.Text = "";
                txtEquipNo3.Text = "";
                txtEquipNo4.Text = "";
                txtEquipNo5.Text = "";
                txtEquipNo6.Text = "";
                txtEquipNo7.Text = "";
                txtEquipNo8.Text = "";
                txtEquipNo9.Text = "";
                txtEquipNo10.Text = "";
                txtEquipNo11.Text = "";
                txtEquipNo12.Text = "";
                txtEquipNo13.Text = "";
                txtEquipNo14.Text = "";
                txtEquipNo15.Text = "";

                // Clear all specified value fields
                txtSpecified1.Text = "";
                txtSpecified2.Text = "";
                txtSpecified3.Text = "";
                txtSpecified4.Text = "";
                txtSpecified5.Text = "";
                txtSpecified6.Text = "";
                txtSpecified7.Text = "";
                txtSpecified8.Text = "";
                txtSpecified9.Text = "";
                txtSpecified10.Text = "";
                txtSpecified11.Text = "";
                txtSpecified12.Text = "";
                txtSpecified13.Text = "";
                txtSpecified14.Text = "";
                txtSpecified15.Text = "";
                txtSpecified16.Text = "";
                txtSpecified17.Text = "";
                txtSpecified18.Text = "";
                txtSpecified19.Text = "";
                txtSpecified20.Text = "";
                txtSpecified21.Text = "";
                txtSpecified22.Text = "";
                txtSpecified23.Text = "";
                txtSpecified24.Text = "";
                txtSpecified25.Text = "";
                txtSpecified26.Text = "";
                txtSpecified27.Text = "";

                // Clear all measurement value fields - Core Specimen Section
                txtRoomTemp1.Text = "";

                // Thickness
                txtMeasuredValue1_1.Text = "";
                txtMeasuredValue1_2.Text = "";
                txtMeasuredValue1_3.Text = "";
                txtMeasuredValue1_4.Text = "";
                txtMeasuredValue1_5.Text = "";

                // Width/Dia
                txtMeasuredValue2_1.Text = "";
                txtMeasuredValue2_2.Text = "";
                txtMeasuredValue2_3.Text = "";
                txtMeasuredValue2_4.Text = "";
                txtMeasuredValue2_5.Text = "";

                // A
                txtMeasuredValue3_1.Text = "";
                txtMeasuredValue3_2.Text = "";
                txtMeasuredValue3_3.Text = "";
                txtMeasuredValue3_4.Text = "";
                txtMeasuredValue3_5.Text = "";

                // BF
                txtMeasuredValue4_1.Text = "";
                txtMeasuredValue4_2.Text = "";
                txtMeasuredValue4_3.Text = "";
                txtMeasuredValue4_4.Text = "";
                txtMeasuredValue4_5.Text = "";

                // TS
                txtMeasuredValue5_1.Text = "";
                txtMeasuredValue5_2.Text = "";
                txtMeasuredValue5_3.Text = "";
                txtMeasuredValue5_4.Text = "";
                txtMeasuredValue5_5.Text = "";

                // LO
                txtMeasuredValue6_1.Text = "";
                txtMeasuredValue6_2.Text = "";
                txtMeasuredValue6_3.Text = "";
                txtMeasuredValue6_4.Text = "";
                txtMeasuredValue6_5.Text = "";

                // L1
                txtMeasuredValue7_1.Text = "";
                txtMeasuredValue7_2.Text = "";
                txtMeasuredValue7_3.Text = "";
                txtMeasuredValue7_4.Text = "";
                txtMeasuredValue7_5.Text = "";

                // EL
                txtMeasuredValue8_1.Text = "";
                txtMeasuredValue8_2.Text = "";
                txtMeasuredValue8_3.Text = "";
                txtMeasuredValue8_4.Text = "";
                txtMeasuredValue8_5.Text = "";

                // Clear Immersed In Oil Section
                txtTemp1.Text = "";
                txtH1.Text = "";
                txtH2.Text = "";
                txtStartDate1.Text = "";
                txtEndDate1.Text = "";

                // Clear Conditioning Section
                txtConditioningRoomTemp.Text = "";
                txtConditioningTime1.Text = "";
                txtConditioningTime2.Text = "";
                txtConditioningDate1.Text = "";
                txtConditioningDate2.Text = "";
                txtConditioningTime3.Text = "";
                txtConditioningTime4.Text = "";

                // Clear TS & EL After Section
                // Thickness
                txtMeasuredValue9_1.Text = "";
                txtMeasuredValue9_2.Text = "";
                txtMeasuredValue9_3.Text = "";
                txtMeasuredValue9_4.Text = "";
                txtMeasuredValue9_5.Text = "";

                // Width/Dia
                txtMeasuredValue10_1.Text = "";
                txtMeasuredValue10_2.Text = "";
                txtMeasuredValue10_3.Text = "";
                txtMeasuredValue10_4.Text = "";
                txtMeasuredValue10_5.Text = "";

                // A
                txtMeasuredValue11_1.Text = "";
                txtMeasuredValue11_2.Text = "";
                txtMeasuredValue11_3.Text = "";
                txtMeasuredValue11_4.Text = "";
                txtMeasuredValue11_5.Text = "";

                // BF
                txtMeasuredValue12_1.Text = "";
                txtMeasuredValue12_2.Text = "";
                txtMeasuredValue12_3.Text = "";
                txtMeasuredValue12_4.Text = "";
                txtMeasuredValue12_5.Text = "";

                // TS
                txtMeasuredValue13_1.Text = "";
                txtMeasuredValue13_2.Text = "";
                txtMeasuredValue13_3.Text = "";
                txtMeasuredValue13_4.Text = "";
                txtMeasuredValue13_5.Text = "";

                // LO
                txtMeasuredValue14_1.Text = "";
                txtMeasuredValue14_2.Text = "";
                txtMeasuredValue14_3.Text = "";
                txtMeasuredValue14_4.Text = "";
                txtMeasuredValue14_5.Text = "";

                // L1
                txtMeasuredValue15_1.Text = "";
                txtMeasuredValue15_2.Text = "";
                txtMeasuredValue15_3.Text = "";
                txtMeasuredValue15_4.Text = "";
                txtMeasuredValue15_5.Text = "";

                // EL
                txtMeasuredValue16_1.Text = "";
                txtMeasuredValue16_2.Text = "";
                txtMeasuredValue16_3.Text = "";
                txtMeasuredValue16_4.Text = "";
                txtMeasuredValue16_5.Text = "";

                // Clear Variation Section
                // TS
                txtMeasuredValue17_1.Text = "";
                txtMeasuredValue17_2.Text = "";
                txtMeasuredValue17_3.Text = "";
                txtMeasuredValue17_4.Text = "";
                txtMeasuredValue17_5.Text = "";

                // EL
                txtMeasuredValue18_1.Text = "";
                txtMeasuredValue18_2.Text = "";
                txtMeasuredValue18_3.Text = "";
                txtMeasuredValue18_4.Text = "";
                txtMeasuredValue18_5.Text = "";

                // Clear all median labels
                lblMedian1.Text = "";
                lblMedian2.Text = "";
                lblMedian3.Text = "";
                lblMedian4.Text = "";
                lblMedian5.Text = "";
                lblMedian6.Text = "";
                lblMedian7.Text = "";
                lblMedian8.Text = "";
                lblMedian9.Text = "";
                lblMedian10.Text = "";
                lblMedian11.Text = "";
                lblMedian12.Text = "";
                lblMedian13.Text = "";
                lblMedian14.Text = "";
                lblMedian15.Text = "";
                lblMedian16.Text = "";
                lblMedian17.Text = "";
                lblMedian18.Text = "";

                // Clear validation errors
                foreach (IValidator validator in Page.Validators)
                {
                    validator.IsValid = true;
                }

                // Show save button, hide update button
                btnSaveData.Visible = true;
                btnUpdateData.Visible = false;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "clearError",
                    $"alert('Error clearing form: {ex.Message}');", true);
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