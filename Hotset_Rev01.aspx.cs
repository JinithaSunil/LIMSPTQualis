using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;

namespace LIMS
{
    public partial class Hotset_Rev01 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Restore the header temperature values from ViewState
                if (ViewState["AfterTemp"] != null)
                    txtaftertemp.Text = ViewState["AfterTemp"].ToString();
                if (ViewState["UnderloadTemp"] != null)
                    txtunderloadatTemp.Text = ViewState["UnderloadTemp"].ToString();

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
                    roomno.Text = "Mechanical & Oven";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/17/PH/175,QE/12/TH/09";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                CheckAndPrefillForm();
                PopulateCoreDropdown();

            }
        }
        private void PopulateCoreDropdown()
        {
            try
            {
                DataTable coreData = _cm.Get_Core();
                ddlCorePhase.Items.Clear();
                if (coreData != null && coreData.Rows.Count > 0)
                {
                    // Bind data to dropdown
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddlCorePhase.Items.Add(new ListItem(coreName, coreId));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                ScriptManager.RegisterStartupScript(this, GetType(), "DropdownError",
                    $"alert('Error loading core data: {ex.Message}');", true);
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
                        PrefillGridData(existingData);

                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }
        private void InitializeGrid()
        {
            DataTable dt = CreateTestResultsDataTable();
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();
        }
        private DataTable CreateTestResultsDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Material", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SampleType", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(string));
            dt.Columns.Add("Thickness", typeof(string));
            dt.Columns.Add("ThicknessAvgMin", typeof(string));
            dt.Columns.Add("WidthDiameter", typeof(string));
            dt.Columns.Add("WidthAvgMin", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Weightload", typeof(string));
            dt.Columns.Add("Initiallength", typeof(string));
            dt.Columns.Add("Length", typeof(string));
            dt.Columns.Add("EL", typeof(string));
            dt.Columns.Add("Median", typeof(string));
            dt.Columns.Add("coolingLength", typeof(string));
            dt.Columns.Add("coolingEL", typeof(string));
            dt.Columns.Add("coolingMedian", typeof(string));

            return dt;
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetTextBoxText(cableType, "17640");
            SetTextBoxText(cableSize, "17641");
            SetTextBoxText(voltage, "17642");
            SetTextBoxText(Date, "17643");
            SetTextBoxText(roomno, "17644");
            SetTextBoxText(roomtemp, "17645");
            SetTextBoxText(refSpec, "17646");
            SetTextBoxText(testMethodNo, "17647");
            SetTextBoxText(jobOrderNo, "17648");
            SetTextBoxText(worksheetno, "17649");
            SetTextBoxText(roomhumidity, "17650");
            SetTextBoxText(equipmentno, "17651");


            SetTextBoxText(txtSpecificThickness, "17664");
            SetTextBoxText(txtSpecificDiameter, "17665");
            SetTextBoxText(txtSpecificArea, "17666");
            SetTextBoxText(txtSpecificWeightload, "17667");
            SetTextBoxText(txtSpecificInitiallength, "17668");
            SetTextBoxText(txtSpecificLength, "17669");
            SetTextBoxText(txtSpecificEL, "17670");
            SetTextBoxText(txtSpecificMedian, "17671");
            SetTextBoxText(txtSpecificoolingLength, "17672");
            SetTextBoxText(txtSpecificoolingEL, "17673");
            SetTextBoxText(txtSpecificoolingMedian, "17674");

            SetTextBoxText(txtaftertemp, "17675");
            SetTextBoxText(txtunderloadatTemp, "17676");


            // Helper method to set text for textboxes
            void SetTextBoxText(TextBox textBox, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }
        protected void ResetInputs()
        {
            txtArea.Text = "";
            txtWeightload.Text = "";
            txtInitiallength.Text = "";
            txtlengthafter.Text = "";
            txtELafter.Text = "";
            txtaftercoolingMedian.Text = "";
            txtaftercoolinglength.Text = "";
            txtaftercoolingEL.Text = "";
            txtaftercoolingMedian.Text = "";
            txtWeightload.Enabled = true;
            //txtThicknessatTemperature.Enabled = true;
            //txtDeformationTemperature.Enabled = true;
        }
        private void PrefillGridData(DataTable existingData)
        {
            try
            {
                // Get the saved grid data from database
                // Date
                var Material = GetParameterValue(existingData, "17663");
                var coreData = GetParameterValue(existingData, "17662");      // Core
                var sampleData = GetParameterValue(existingData, "17661");    // Sample
                var specimenData = GetParameterValue(existingData, "17677");  // SpecimenNo
                var thicknessData = GetParameterValue(existingData, "17678"); // Thickness
                var thicknessAvgData = GetParameterValue(existingData, "17679"); // ThicknessAvg
                var widthData = GetParameterValue(existingData, "17680");     // Width
                var widthAvgData = GetParameterValue(existingData, "17681");  // WidthAvg
                var weightload = GetParameterValue(existingData, "17653");        // BF
                var areaData = GetParameterValue(existingData, "17652");      // Area
                var Initiallength = GetParameterValue(existingData, "17654");        // BF
                var After15Length = GetParameterValue(existingData, "17655");        // TS
                var After15EL = GetParameterValue(existingData, "17656"); // ElongationL0
                var After15Median = GetParameterValue(existingData, "17657"); // ElongationL1
                var AftercoolingLength = GetParameterValue(existingData, "17658"); // ElongationEL
                var AftercoolingEL = GetParameterValue(existingData, "17659");  // MedianTS
                var AftercoolingMedian = GetParameterValue(existingData, "17660");  // MedianEL


                if (!string.IsNullOrEmpty(Material))
                {
                    DataTable dt = CreateTestResultsDataTable();

                    var materialGroups = Material.Split('|');
                    var coreGroups = coreData?.Split('|') ?? new string[0];
                    var sampleGroups = sampleData?.Split('|') ?? new string[0];
                    var specimenGroups = specimenData?.Split('|') ?? new string[0];
                    var thicknessGroups = thicknessData?.Split('|') ?? new string[0];
                    var thicknessAvgGroups = thicknessAvgData?.Split('|') ?? new string[0];
                    var widthGroups = widthData?.Split('|') ?? new string[0];
                    var widthAvgGroups = widthAvgData?.Split('|') ?? new string[0];
                    var areaGroups = areaData?.Split('|') ?? new string[0];
                    var weightLoadGroups = weightload?.Split('|') ?? new string[0];
                    var initialLengthGroups = Initiallength?.Split('|') ?? new string[0];
                    var after15LengthGroups = After15Length?.Split('|') ?? new string[0];
                    var after15ELGroups = After15EL?.Split('|') ?? new string[0];
                    var after15MedianGroups = After15Median?.Split('|') ?? new string[0];
                    var afterCoolingLengthGroups = AftercoolingLength?.Split('|') ?? new string[0];
                    var afterCoolingELGroups = AftercoolingEL?.Split('|') ?? new string[0];
                    var afterCoolingMedianGroups = AftercoolingMedian?.Split('|') ?? new string[0];

                    for (int groupIndex = 0; groupIndex < materialGroups.Length; groupIndex++)
                    {
                        var currentMaterial = materialGroups[groupIndex];

                        var coreSubGroups = groupIndex < coreGroups.Length ? coreGroups[groupIndex].Split(';') : new string[0];
                        var sampleSubGroups = groupIndex < sampleGroups.Length ? sampleGroups[groupIndex].Split(';') : new string[0];
                        var specimenSubGroups = groupIndex < specimenGroups.Length ? specimenGroups[groupIndex].Split(';') : new string[0];
                        var thicknessSubGroups = groupIndex < thicknessGroups.Length ? thicknessGroups[groupIndex].Split(';') : new string[0];
                        var thicknessAvgSubGroups = groupIndex < thicknessAvgGroups.Length ? thicknessAvgGroups[groupIndex].Split(';') : new string[0];
                        var widthSubGroups = groupIndex < widthGroups.Length ? widthGroups[groupIndex].Split(';') : new string[0];
                        var widthAvgSubGroups = groupIndex < widthAvgGroups.Length ? widthAvgGroups[groupIndex].Split(';') : new string[0];
                        var areaSubGroups = groupIndex < areaGroups.Length ? areaGroups[groupIndex].Split(';') : new string[0];
                        var weightLoadSubGroups = groupIndex < weightLoadGroups.Length ? weightLoadGroups[groupIndex].Split(';') : new string[0];
                        var initialLengthSubGroups = groupIndex < initialLengthGroups.Length ? initialLengthGroups[groupIndex].Split(';') : new string[0];
                        var after15LengthSubGroups = groupIndex < after15LengthGroups.Length ? after15LengthGroups[groupIndex].Split(';') : new string[0];
                        var after15ELSubGroups = groupIndex < after15ELGroups.Length ? after15ELGroups[groupIndex].Split(';') : new string[0];
                        var after15MedianSubGroups = groupIndex < after15MedianGroups.Length ? after15MedianGroups[groupIndex].Split(';') : new string[0];
                        var afterCoolingLengthSubGroups = groupIndex < afterCoolingLengthGroups.Length ? afterCoolingLengthGroups[groupIndex].Split(';') : new string[0];
                        var afterCoolingELSubGroups = groupIndex < afterCoolingELGroups.Length ? afterCoolingELGroups[groupIndex].Split(';') : new string[0];
                        var afterCoolingMedianSubGroups = groupIndex < afterCoolingMedianGroups.Length ? afterCoolingMedianGroups[groupIndex].Split(';') : new string[0];
                        for (int coreIndex = 0; coreIndex < coreSubGroups.Length; coreIndex++)
                        {
                            var coreValues = coreSubGroups[coreIndex].Split(',');
                            var sampleValues = coreIndex < sampleSubGroups.Length ? sampleSubGroups[coreIndex].Split(',') : new string[0];
                            var specimenValues = coreIndex < specimenSubGroups.Length ? specimenSubGroups[coreIndex].Split(',') : new string[0];

                            var thicknessValues = coreIndex < thicknessSubGroups.Length ? SplitByParentheses(thicknessSubGroups[coreIndex]) : new string[0];
                            var thicknessAvgValues = coreIndex < thicknessAvgSubGroups.Length ? SplitByParentheses(thicknessAvgSubGroups[coreIndex]) : new string[0];
                            var widthValues = coreIndex < widthSubGroups.Length ? SplitByParentheses(widthSubGroups[coreIndex]) : new string[0];
                            var widthAvgValues = coreIndex < widthAvgSubGroups.Length ? SplitByParentheses(widthAvgSubGroups[coreIndex]) : new string[0];
                            var areaValues = coreIndex < areaSubGroups.Length ? SplitByParentheses(areaSubGroups[coreIndex]) : new string[0];
                            var weightLoadValues = coreIndex < weightLoadSubGroups.Length ? SplitByParentheses(weightLoadSubGroups[coreIndex]) : new string[0];
                            var initialLengthValues = coreIndex < initialLengthSubGroups.Length ? SplitByParentheses(initialLengthSubGroups[coreIndex]) : new string[0];
                            var after15LengthValues = coreIndex < after15LengthSubGroups.Length ? SplitByParentheses(after15LengthSubGroups[coreIndex]) : new string[0];
                            var after15ELValues = coreIndex < after15ELSubGroups.Length ? SplitByParentheses(after15ELSubGroups[coreIndex]) : new string[0];
                            var after15MedianValues = coreIndex < after15MedianSubGroups.Length ? SplitByParentheses(after15MedianSubGroups[coreIndex]) : new string[0];
                            var afterCoolingLengthValues = coreIndex < afterCoolingLengthSubGroups.Length ? SplitByParentheses(afterCoolingLengthSubGroups[coreIndex]) : new string[0];
                            var afterCoolingELValues = coreIndex < afterCoolingELSubGroups.Length ? SplitByParentheses(afterCoolingELSubGroups[coreIndex]) : new string[0];
                            var afterCoolingMedianValues = coreIndex < afterCoolingMedianSubGroups.Length ? SplitByParentheses(afterCoolingMedianSubGroups[coreIndex]) : new string[0];

                            int rowCount = Math.Max(coreValues.Length, Math.Max(sampleValues.Length, specimenValues.Length));

                            for (int i = 0; i < rowCount; i++)
                            {
                                DataRow row = dt.NewRow();

                                row["Material"] = currentMaterial;
                                row["CorePhase"] = i < coreValues.Length ? coreValues[i] : "";
                                row["SampleType"] = i < sampleValues.Length ? sampleValues[i] : "";
                                row["SpecimenNo"] = i < specimenValues.Length ? specimenValues[i] : "";
                                row["Thickness"] = i < thicknessValues.Length ? CleanValue(thicknessValues[i]) : "";
                                row["ThicknessAvgMin"] = i < thicknessAvgValues.Length ? CleanValue(thicknessAvgValues[i]) : "";
                                row["WidthDiameter"] = i < widthValues.Length ? CleanValue(widthValues[i]) : "";
                                row["WidthAvgMin"] = i < widthAvgValues.Length ? CleanValue(widthAvgValues[i]) : "";
                                row["Area"] = i < areaValues.Length ? CleanValue(areaValues[i]) : "";
                                row["Weightload"] = i < weightLoadValues.Length ? CleanValue(weightLoadValues[i]) : "";
                                row["Initiallength"] = i < initialLengthValues.Length ? CleanValue(initialLengthValues[i]) : "";
                                row["Length"] = i < after15LengthValues.Length ? CleanValue(after15LengthValues[i]) : "";
                                row["EL"] = i < after15ELValues.Length ? CleanValue(after15ELValues[i]) : "";
                                row["Median"] = i < after15MedianValues.Length ? CleanValue(after15MedianValues[i]) : "";
                                row["coolingLength"] = i < afterCoolingLengthValues.Length ? CleanValue(afterCoolingLengthValues[i]) : "";
                                row["coolingEL"] = i < afterCoolingELValues.Length ? CleanValue(afterCoolingELValues[i]) : "";
                                row["coolingMedian"] = i < afterCoolingMedianValues.Length ? CleanValue(afterCoolingMedianValues[i]) : "";

                                dt.Rows.Add(row);
                            }
                        }
                    }

                    // Bind to grid and store in ViewState

                    ViewState["TestResultsData"] = dt;
                    gvPressureTest.DataSource = dt;
                    gvPressureTest.DataBind();
                    // Helper method to set TextBox values
                    void SetTextBoxText(TextBox textBox, string parameterId)
                    {
                        var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
                        if (row != null)
                        {
                            textBox.Text = row["Result"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error prefilling grid data: {ex.Message}');", true);
            }
        }
        private string GetParameterValue(DataTable existingData, string parameterId)
        {
            var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
            return row != null ? row["Result"].ToString() : "";
        }
        private string[] SplitByParentheses(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];

            List<string> result = new List<string>();
            int start = -1;
            int parenthesesCount = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    if (parenthesesCount == 0)
                        start = i;
                    parenthesesCount++;
                }
                else if (input[i] == ')')
                {
                    parenthesesCount--;
                    if (parenthesesCount == 0 && start != -1)
                    {
                        result.Add(input.Substring(start, i - start + 1));
                        start = -1;
                    }
                }
            }

            return result.ToArray();
        }
        private string CleanValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

  
            value = value.Trim();
            if (value.StartsWith("(") && value.EndsWith(")"))
                value = value.Substring(1, value.Length - 2);

            return value;
        }
        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Store current values in ViewState before the dropdown change processing
            ViewState["AfterTemp"] = txtaftertemp.Text;
            ViewState["UnderloadTemp"] = txtunderloadatTemp.Text;

            if (string.IsNullOrEmpty(ddlCorePhase.SelectedValue))
            {
                txtSpecimenNumber.Text = "";
                ShowMessage("Please select a valid Core Phase.");
                return;
            }

            txtSpecimenNumber.Text = "1";

            // Restore the values immediately
            if (ViewState["AfterTemp"] != null)
                txtaftertemp.Text = ViewState["AfterTemp"].ToString();
            if (ViewState["UnderloadTemp"] != null)
                txtunderloadatTemp.Text = ViewState["UnderloadTemp"].ToString();
        }
        protected void UpdateSpecimenNumber()
        {
            // Get the DataTable from ViewState
            DataTable dt = (DataTable)ViewState["TestResults"];


            int nextSpecimenNo = 1; // Default to 1 if no rows
            if (dt != null && dt.Rows.Count > 0)
            {
                nextSpecimenNo = dt.Rows.Count + 1;
            }

            TextBox txtSpecimenNo = FindControl("txtSpecimenNo") as TextBox;


            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
            }

        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders(this);

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            int specimenNo = 0;

            // Validate 'itemtest'
            if (string.IsNullOrWhiteSpace(txtmaterial.Text))
            {
                invalidControls.Add(txtmaterial);
                hasError = true;
            }
            // Validate 'SpecimenNo'
            if (string.IsNullOrWhiteSpace(txtSpecimenNumber.Text) || !int.TryParse(txtSpecimenNumber.Text.Trim(), out specimenNo))
            {
                invalidControls.Add(txtSpecimenNumber);
                hasError = true;
            }
            // Validate Avg fields with regex (unchanged)
            string avgPattern = @"^Min:\s*(\d+(\.\d+)?)\s*Max:\s*(\d+(\.\d+)?)\s*Avg:\s*(\d+(\.\d+)?)$";
            if (!Regex.IsMatch(txtthicknessInputAvg.Text.Trim(), avgPattern))
            {
                invalidControls.Add(txtthicknessInputAvg);
                hasError = true;
            }
            if (!Regex.IsMatch(txtdiameterInputAvg.Text.Trim(), avgPattern))
            {
                invalidControls.Add(txtdiameterInputAvg);
                hasError = true;
            }

            // Validate input comma-separated values: treat "-" as 0 and catch empty entries
            TextBox[] individualInputs = { txtThickness, txtdiameterInputs };

            foreach (TextBox txt in individualInputs)
            {
                var parts = txt.Text.Split(new[] { ',' }, StringSplitOptions.None);

                for (int i = 0; i < parts.Length; i++)
                {
                    string val = parts[i].Trim();

                    if (val == "")
                    {
                        hasError = true;
                        invalidControls.Add(txt);
                        break;
                    }

                    if (val == "-")
                        continue;

                    if (!decimal.TryParse(val, out _))
                    {
                        hasError = true;
                        invalidControls.Add(txt);
                        break;
                    }
                }
            }

            // Highlight invalid controls
            foreach (var control in invalidControls)
            {
                if (control is TextBox txt)
                {
                    txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    txt.BorderStyle = BorderStyle.Solid;
                    txt.BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Initialize DataTable if null
            DataTable dt = ViewState["TestResultsData"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("Material", typeof(string));
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("SampleType", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(string)); // Match GridView column name
                dt.Columns.Add("Thickness", typeof(string));
                dt.Columns.Add("ThicknessAvgMin", typeof(string));
                dt.Columns.Add("WidthDiameter", typeof(string));
                dt.Columns.Add("WidthAvgMin", typeof(string)); // ADD THIS MISSING COLUMN
                dt.Columns.Add("Area", typeof(string));
                dt.Columns.Add("Weightload", typeof(string)); // match casing in GridView
                dt.Columns.Add("Initiallength", typeof(string));
                dt.Columns.Add("Length", typeof(string));
                dt.Columns.Add("EL", typeof(string));
                dt.Columns.Add("Median", typeof(string));
                dt.Columns.Add("coolingLength", typeof(string));
                dt.Columns.Add("coolingEL", typeof(string));
                dt.Columns.Add("coolingMedian", typeof(string));
            }

            // Create new row
            DataRow dr = dt.NewRow();
            dr["Material"] = txtmaterial.Text.Trim();
            dr["CorePhase"] = ddlCorePhase.SelectedItem.Text;
            dr["SampleType"] = txtsampletest.Text.Trim();
            dr["SpecimenNo"] = txtSpecimenNumber.Text.Trim();
            dr["Thickness"] = txtThickness.Text.Trim();
            dr["ThicknessAvgMin"] = txtthicknessInputAvg.Text.Trim();
            dr["WidthDiameter"] = txtdiameterInputs.Text.Trim();
            dr["WidthAvgMin"] = txtdiameterInputAvg.Text.Trim();
            dr["Area"] = txtArea.Text.Trim();
            dr["Weightload"] = txtWeightload.Text.Trim();
            dr["Initiallength"] = txtInitiallength.Text.Trim();
            dr["Length"] = txtlengthafter.Text.Trim();
            dr["EL"] = txtELafter.Text.Trim();
            dr["Median"] = ""; 
            dr["coolingLength"] = txtaftercoolinglength.Text.Trim(); 
            dr["coolingEL"] = txtaftercoolingEL.Text.Trim();
            dr["coolingMedian"] = ""; 

            dt.Rows.Add(dr);

            // CALCULATE MEDIANS FOR ALL GROUPS AFTER ADDING ROW
            // Group all rows by Material + CorePhase + SampleType
            var allGroups = dt.AsEnumerable()
                .GroupBy(r => new
                {
                    Material = r.Field<string>("Material"),
                    CorePhase = r.Field<string>("CorePhase"),
                    SampleType = r.Field<string>("SampleType")
                })
                .ToList();

            // Calculate medians for each group
            foreach (var group in allGroups)
            {
                var rowsInGroup = group.ToList();

                // Calculate Regular Median (Length + EL from ALL specimens in group)
                List<decimal> lengthValues = new List<decimal>();
                List<decimal> elValues = new List<decimal>();

                foreach (var row in rowsInGroup)
                {
                    if (decimal.TryParse(row["Length"].ToString(), out decimal length) && length > 0)
                        lengthValues.Add(length);

                    if (decimal.TryParse(row["EL"].ToString(), out decimal el) && el > 0)
                        elValues.Add(el);
                }

                var combinedValues = lengthValues.Concat(elValues).ToList();
                string medianStr = CalculateMedian(combinedValues);

                // Calculate Cooling Median (coolingLength + coolingEL from ALL specimens in group)
                List<decimal> coolingLengthValues = new List<decimal>();
                List<decimal> coolingELValues = new List<decimal>();

                foreach (var row in rowsInGroup)
                {
                    if (decimal.TryParse(row["coolingLength"].ToString(), out decimal coolingLength) && coolingLength > 0)
                        coolingLengthValues.Add(coolingLength);

                    if (decimal.TryParse(row["coolingEL"].ToString(), out decimal coolingEL) && coolingEL > 0)
                        coolingELValues.Add(coolingEL);
                }

                var coolingCombinedValues = coolingLengthValues.Concat(coolingELValues).ToList();
                string coolingMedianStr = CalculateMedian(coolingCombinedValues);

                // Update median for ALL rows in this group
                foreach (var row in rowsInGroup)
                {
                    row["Median"] = medianStr;
                    row["coolingMedian"] = coolingMedianStr;
                }
            }

            ViewState["TestResultsData"] = dt;

            // Bind to GridView
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();

            // Update specimen number for next entry
            txtSpecimenNumber.Text = (specimenNo + 1).ToString();
            ClearInputFields();
        }
        protected void gvPressureTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            DataTable dt = ViewState["TestResultsData"] as DataTable;
            if (dt == null) return;

            DataRowView drv = (DataRowView)e.Row.DataItem;
            int currentRowIndex = e.Row.RowIndex;

            string currentMaterial = drv["Material"].ToString();
            string currentCore = drv["CorePhase"].ToString();
            string currentSample = drv["SampleType"].ToString();

            // Find all rows with same Material+Core+SampleType
            var matchingRows = dt.AsEnumerable()
                .Select((row, index) => new { Row = row, Index = index })
                .Where(x => x.Row.Field<string>("Material") == currentMaterial &&
                           x.Row.Field<string>("CorePhase") == currentCore &&
                           x.Row.Field<string>("SampleType") == currentSample)
                .ToList();

            // Check if this is the first row in the group
            bool isFirstInGroup = matchingRows.First().Index == currentRowIndex;
            int groupCount = matchingRows.Count;

            // Merge Material, Core, Sample, and Median columns for first row in group
            if (isFirstInGroup && groupCount > 1)
            {
                // Material column
                int materialColIndex = GetColumnIndexByName("Material");
                if (materialColIndex != -1)
                {
                    e.Row.Cells[materialColIndex].RowSpan = groupCount;
                    e.Row.Cells[materialColIndex].VerticalAlign = VerticalAlign.Middle;
                }

                // CorePhase column  
                int coreColIndex = GetColumnIndexByName("CorePhase");
                if (coreColIndex != -1)
                {
                    e.Row.Cells[coreColIndex].RowSpan = groupCount;
                    e.Row.Cells[coreColIndex].VerticalAlign = VerticalAlign.Middle;
                }

                // SampleType column
                int sampleColIndex = GetColumnIndexByName("SampleType");
                if (sampleColIndex != -1)
                {
                    e.Row.Cells[sampleColIndex].RowSpan = groupCount;
                    e.Row.Cells[sampleColIndex].VerticalAlign = VerticalAlign.Middle;
                }

                // Regular Median column
                int medianColIndex = GetColumnIndexByName("Median");
                if (medianColIndex != -1)
                {
                    e.Row.Cells[medianColIndex].Text = drv["Median"].ToString();
                    e.Row.Cells[medianColIndex].RowSpan = groupCount;
                    e.Row.Cells[medianColIndex].VerticalAlign = VerticalAlign.Middle;
                }

                // Cooling Median column
                int coolingMedianColIndex = GetColumnIndexByName("coolingMedian");
                if (coolingMedianColIndex != -1)
                {
                    e.Row.Cells[coolingMedianColIndex].Text = drv["coolingMedian"].ToString();
                    e.Row.Cells[coolingMedianColIndex].RowSpan = groupCount;
                    e.Row.Cells[coolingMedianColIndex].VerticalAlign = VerticalAlign.Middle;
                }
            }
            else if (!isFirstInGroup)
            {
                // Hide these columns for subsequent rows in the group
                int materialColIndex = GetColumnIndexByName("Material");
                if (materialColIndex != -1)
                    e.Row.Cells[materialColIndex].Visible = false;

                int coreColIndex = GetColumnIndexByName("CorePhase");
                if (coreColIndex != -1)
                    e.Row.Cells[coreColIndex].Visible = false;

                int sampleColIndex = GetColumnIndexByName("SampleType");
                if (sampleColIndex != -1)
                    e.Row.Cells[sampleColIndex].Visible = false;

                int medianColIndex = GetColumnIndexByName("Median");
                if (medianColIndex != -1)
                    e.Row.Cells[medianColIndex].Visible = false;

                int coolingMedianColIndex = GetColumnIndexByName("coolingMedian");
                if (coolingMedianColIndex != -1)
                    e.Row.Cells[coolingMedianColIndex].Visible = false;
            }
            else
            {
                // Single row in group - just display values normally
                int medianColIndex = GetColumnIndexByName("Median");
                if (medianColIndex != -1)
                {
                    e.Row.Cells[medianColIndex].Text = drv["Median"].ToString();
                }

                int coolingMedianColIndex = GetColumnIndexByName("coolingMedian");
                if (coolingMedianColIndex != -1)
                {
                    e.Row.Cells[coolingMedianColIndex].Text = drv["coolingMedian"].ToString();
                }
            }
        }
        private int GetColumnIndexByName(string columnName)
        {
            for (int i = 0; i < gvPressureTest.Columns.Count; i++)
            {
                if (gvPressureTest.Columns[i] is BoundField boundField)
                {
                    if (boundField.DataField == columnName)
                        return i;
                }
                else if (gvPressureTest.Columns[i] is TemplateField templateField)
                {
                    if (templateField.HeaderText == columnName)
                        return i;
                }
            }
            return -1; // Column not found
        }
        private string CalculateMedian(List<decimal> values)
        {
            if (values == null || values.Count == 0)
                return "0";

            values = values.OrderBy(x => x).ToList();
            int count = values.Count;

            if (count % 2 == 0)
            {
                // Even number of values - average of middle two
                decimal median = (values[count / 2 - 1] + values[count / 2]) / 2;
                return median.ToString("F2");
            }
            else
            {
                // Odd number of values - middle value
                decimal median = values[count / 2];
                return median.ToString("F2");
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvPressureTest.Rows.Count > 0);


            if (string.IsNullOrEmpty(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }
            if (string.IsNullOrEmpty(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }
            if (roomhumidity.Text.Trim() != "-" && (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _)))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (roomtemp.Text.Trim() != "-" && (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _)))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            if (txtSpecificThickness.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificThickness.Text) || !double.TryParse(txtSpecificThickness.Text, out _)))
            {
                errors.Add("Specific Thickness must be a valid number or '-'.");
                invalidControls.Add(txtSpecificThickness);
            }

            if (txtSpecificDiameter.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificDiameter.Text) || !double.TryParse(txtSpecificDiameter.Text, out _)))
            {
                errors.Add("Specific Diameter must be a valid number or '-'.");
                invalidControls.Add(txtSpecificDiameter);
            }

            if (txtSpecificArea.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificArea.Text) || !double.TryParse(txtSpecificArea.Text, out _)))
            {
                errors.Add("Specific Area must be a valid number or '-'.");
                invalidControls.Add(txtSpecificArea);
            }

            if (txtSpecificWeightload.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificWeightload.Text) || !double.TryParse(txtSpecificWeightload.Text, out _)))
            {
                errors.Add("Specific Weight Load must be a valid number or '-'.");
                invalidControls.Add(txtSpecificWeightload);
            }

            if (txtSpecificInitiallength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificInitiallength.Text) || !double.TryParse(txtSpecificInitiallength.Text, out _)))
            {
                errors.Add("Specific Initial Length must be a valid number or '-'.");
                invalidControls.Add(txtSpecificInitiallength);
            }

            if (txtSpecificLength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificLength.Text) || !double.TryParse(txtSpecificLength.Text, out _)))
            {
                errors.Add("Specific Length must be a valid number or '-'.");
                invalidControls.Add(txtSpecificLength);
            }

            if (txtSpecificEL.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificEL.Text) || !double.TryParse(txtSpecificEL.Text, out _)))
            {
                errors.Add("Specific Elongation must be a valid number or '-'.");
                invalidControls.Add(txtSpecificEL);
            }

            if (txtSpecificMedian.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificMedian.Text) || !double.TryParse(txtSpecificMedian.Text, out _)))
            {
                errors.Add("Specific Median must be a valid number or '-'.");
                invalidControls.Add(txtSpecificMedian);
            }

            if (txtSpecificoolingLength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificoolingLength.Text) || !double.TryParse(txtSpecificoolingLength.Text, out _)))
            {
                errors.Add("Specific After Cooling Length must be a valid number or '-'.");
                invalidControls.Add(txtSpecificoolingLength);
            }

            if (txtSpecificoolingEL.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificoolingEL.Text) || !double.TryParse(txtSpecificoolingEL.Text, out _)))
            {
                errors.Add("Specific After Cooling Elongation must be a valid number or '-'.");
                invalidControls.Add(txtSpecificoolingEL);
            }



            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                // Validate Test Details Input 
                if (string.IsNullOrEmpty(txtWeightload.Text) ||
                    (txtWeightload.Text.Trim() != "-" && !double.TryParse(txtWeightload.Text.Trim(), out _)))
                {
                    errors.Add("Weightload must be a valid number or '-'.");
                    invalidControls.Add(txtWeightload);
                }

                // Validate Test Details Input 
                if (string.IsNullOrEmpty(txtmaterial.Text) ||
                    (txtmaterial.Text.Trim() != "-" && !double.TryParse(txtmaterial.Text.Trim(), out _)))
                {
                    errors.Add("material must be a valid number or '-'.");
                    invalidControls.Add(txtmaterial);
                }
                // Validate Test Details Input 
                if (string.IsNullOrEmpty(txtArea.Text) ||
                    (txtArea.Text.Trim() != "-" && !double.TryParse(txtArea.Text.Trim(), out _)))
                {
                    errors.Add("Area must be a valid number or '-'.");
                    invalidControls.Add(txtArea);
                }
                if (string.IsNullOrEmpty(txtWeightload.Text) ||
                    (txtWeightload.Text.Trim() != "-" && !double.TryParse(txtWeightload.Text.Trim(), out _)))
                {
                    errors.Add("Weightload must be a valid number or '-'.");
                    invalidControls.Add(txtWeightload);
                }
                if (string.IsNullOrEmpty(txtInitiallength.Text) ||
                    (txtInitiallength.Text.Trim() != "-" && !double.TryParse(txtInitiallength.Text.Trim(), out _)))
                {
                    errors.Add("Initial length must be a valid number or '-'.");
                    invalidControls.Add(txtInitiallength);
                }
                if (string.IsNullOrEmpty(txtlengthafter.Text) ||
                    (txtlengthafter.Text.Trim() != "-" && !double.TryParse(txtlengthafter.Text.Trim(), out _)))
                {
                    errors.Add("length must be a valid number or '-'.");
                    invalidControls.Add(txtlengthafter);
                }
                if (string.IsNullOrEmpty(txtELafter.Text) ||
                    (txtELafter.Text.Trim() != "-" && !double.TryParse(txtELafter.Text.Trim(), out _)))
                {
                    errors.Add("EL must be a valid number or '-'.");
                    invalidControls.Add(txtELafter);
                }
                if (string.IsNullOrEmpty(txtaftercoolinglength.Text) ||
                    (txtaftercoolinglength.Text.Trim() != "-" && !double.TryParse(txtaftercoolinglength.Text.Trim(), out _)))
                {
                    errors.Add("length must be a valid number or '-'.");
                    invalidControls.Add(txtaftercoolinglength);
                }
                if (string.IsNullOrEmpty(txtaftercoolingEL.Text) ||
                 (txtaftercoolingEL.Text.Trim() != "-" && !double.TryParse(txtaftercoolingEL.Text.Trim(), out _)))
                {
                    errors.Add("EL must be a valid number or '-'.");
                    invalidControls.Add(txtaftercoolingEL);
                }

            }

            // Reset all control borders first
            ResetControlBorders(this);

            // Display errors or proceed
            if (errors.Count > 0)
            {
                // Highlight invalid controls with light red border
                foreach (var control in invalidControls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        ((TextBox)control).BorderStyle = BorderStyle.Solid;
                        ((TextBox)control).BorderWidth = 1;
                    }
                }

                string errorMessage = "Validation Errors:\\n" + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);

            }
            else
            {
                bool isSaved = Submitform();
                if (isSaved)
                    if (isSaved)
                    {
                        // Show success message
                        ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                        "alert('Test results and Report Generated successfully!');", true);
                        // Clear form
                        ClearForm();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "SaveFailed",
                            "alert('Test results were not saved due to an error.');", true);
                    }
            }
        }
        private string[] SplitAndTrim(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? new[] { string.Empty }
                : input.Split(',').Select(s => s.Trim()).ToArray();
        }
        private string GetArrayValue(string[] array, int index)
        {
            return index < array.Length ? array[index] : array[0];
        }  
     
        private void CalculateAndDisplayStats(string rawInput, TextBox outputBox)
        {
            List<decimal> values = new List<decimal>();

            string[] inputs = rawInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); // Remove empty entries here

            foreach (string input in inputs)
            {
                string trimmed = input.Trim();

                if (trimmed == "-")
                {
                    values.Add(0); // Treat "-" as 0
                    continue;
                }

                if (decimal.TryParse(trimmed, out decimal val))
                {
                    values.Add(val);
                }
                else
                {
                    outputBox.Text = "Invalid input format";  // Set message immediately and return
                    return;
                }
            }

            if (values.Count > 0)
            {
                decimal min = values.Min();
                decimal max = values.Max();
                decimal avg = Math.Round(values.Average(), 2);

                outputBox.Text = $"Min: {min:F2}   Max: {max:F2}   Avg: {avg:F2}";
            }
            else
            {
                outputBox.Text = "No valid inputs";
            }
        }
        protected void ThicknessInputs_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplayStats(txtThickness.Text, txtthicknessInputAvg);
        }
        protected void DiameterInputs_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplayStats(txtdiameterInputs.Text, txtdiameterInputAvg);
        }
        protected void gvPressureTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResultsData"] as DataTable;

                // Delete the row from the DataTable
                if (dt != null && index >= 0 && index < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(index);

                    // Update ViewState and rebind the grid
                    ViewState["TestResults"] = dt;
                    gvPressureTest.DataSource = dt;
                    gvPressureTest.DataBind();


                    // Show confirmation message
                    ShowMessage("Record deleted successfully.");
                }
            }
        }
        private decimal TryParseDecimal(string input)
        {
            return decimal.TryParse(input, out decimal result) ? result : -1;
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            try
            {
                // Get column name from the column index
                string columnName = dt.Columns[columnIndex].ColumnName;

                // Assuming Input1–Input6 are at indexes 3–8 exactly
                if (columnIndex >= 3 && columnIndex <= 8)
                {
                    return;
                }

                // Only get values if we're going to use them
                // Get current and previous cell values
                string currentValue = dt.Rows[currentRowIndex][columnName]?.ToString();
                string previousValue = dt.Rows[currentRowIndex - 1][columnName]?.ToString();

                // If values are the same, merge cells
                if (currentValue == previousValue)
                {
                    // Hide current cell
                    currentRow.Cells[columnIndex].Visible = false;
                    // Find the first visible cell above with the same value
                    int spanRowIndex = currentRowIndex - 1;
                    while (spanRowIndex > 0 && !gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].Visible)
                    {
                        spanRowIndex--;
                    }
                    // Increase rowspan of that cell
                    if (gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan == 0)
                    {
                        gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                    }
                    else
                    {
                        gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Add error logging or handling as needed
                System.Diagnostics.Debug.WriteLine("Error in ProcessColumn: " + ex.Message);
            }
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
        private void ClearForm()
        {
            // Main Section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";


            ddlCorePhase.SelectedIndex = 0;
            txtThickness.Text = "";

            txtthicknessInputAvg.Text = "";

            txtdiameterInputs.Text = "";

            txtdiameterInputAvg.Text = "";
            ViewState["TestResultsData"] = null;
            // Clear GridView
            gvPressureTest.DataSource = null;
            gvPressureTest.DataBind();


            // Standard Values
            txtSpecificThickness.Text = "";
            txtSpecificDiameter.Text = "";
            txtSpecificWeightload.Text = "";
            txtSpecificMedian.Text = "";
            txtWeightload.Text = "";
            txtArea.Text = "";
            txtInitiallength.Text = "";
            txtlengthafter.Text = "";
            txtELafter.Text = "";
            //txtmedianafter.Text = "";
            txtaftercoolinglength.Text = "";
            txtaftercoolingEL.Text = "";
            txtaftercoolingMedian.Text = "";





            //txtThicknessatTemperature.Text = "";
            //txtDeformationTemperature.Text = "";

        }
        private void ClearInputFields()
        {
            string afterTemp = txtaftertemp.Text;
            string underloadTemp = txtunderloadatTemp.Text;
            txtWeightload.Text = "";
            //txtThicknessatTemperature.Text = "";
            //txtDeformationTemperature.Text = "";
            txtThickness.Text = "";
            txtArea.Text = "";
            txtWeightload.Text = "";
            txtInitiallength.Text = "";
            txtlengthafter.Text = "";
            txtlengthafter.Text = "";
            txtELafter.Text = "";
            txtaftercoolinglength.Text = "";
            txtaftercoolingEL.Text = "";
            txtaftercoolingMedian.Text = "";

            txtthicknessInputAvg.Text = "";

            txtdiameterInputs.Text = "";

            txtdiameterInputAvg.Text = "";

            //ViewState["TestResultsData"] = null;

            // txtMedian.Text = "";
        }
        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{safeMessage}');", true);
        }
        private void ResetControlBorders(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox txt)
                {
                    txt.BorderColor = System.Drawing.Color.Empty;
                    txt.BorderStyle = BorderStyle.NotSet;
                    txt.BorderWidth = Unit.Empty;
                }
                else if (control is DropDownList ddl)
                {
                    ddl.BorderColor = System.Drawing.Color.Empty;
                    ddl.BorderStyle = BorderStyle.NotSet;
                    ddl.BorderWidth = Unit.Empty;
                }

                if (control.HasControls())
                {
                    ResetControlBorders(control);
                }
            }
        }
        private bool Submitform()
        {
            try
            {
                DataTable dt = ViewState["TestResultsData"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return false;
                }

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

                // New grouping: by Material only
                var materialGroups = dt.AsEnumerable()
                    .GroupBy(r => r["Material"]?.ToString() ?? "")
                    .OrderBy(g => g.Key)
                    .ToList();

                // Lists to store the grouped data
                List<string> materialGroupList = new List<string>();
                List<string> coreGroups = new List<string>();
                List<string> sampleGroups = new List<string>();
                List<string> specimenNoGroups = new List<string>();
                List<string> thicknessGroups = new List<string>();
                List<string> thicknessAvgGroups = new List<string>();
                List<string> widthGroups = new List<string>();
                List<string> widthAvgGroups = new List<string>();
                List<string> areaGroups = new List<string>();
                List<string> weightGroups = new List<string>();
                List<string> initialLengthGroups = new List<string>();
                List<string> lengthGroups = new List<string>();
                List<string> elGroups = new List<string>();
                List<string> medianGroups = new List<string>();
                List<string> afterCoolingLengthGroups = new List<string>();
                List<string> afterCoolingELGroups = new List<string>();
                List<string> afterCoolingMedianGroups = new List<string>();

                foreach (var group in materialGroups)
                {
                    // Add Material name
                    materialGroupList.Add(group.Key);

                    var groupRows = group.ToList();

                    // Group all fields by CorePhase within this Material group
                    var coreSubGroups = groupRows.GroupBy(r => r["CorePhase"]?.ToString() ?? "").ToList();

                    // For Core field
                    var coreGroupStrings = new List<string>();
                    foreach (var coreSubGroup in coreSubGroups)
                    {
                        var coreValue = coreSubGroup.Key;
                        var count = coreSubGroup.Count();
                        coreGroupStrings.Add(string.Join(",", Enumerable.Repeat(coreValue, count)));
                    }
                    coreGroups.Add(string.Join(";", coreGroupStrings));

                    // For all other fields, group by core and apply the same logic
                    var sampleGroupStrings = new List<string>();
                    var specimenGroupStrings = new List<string>();
                    var thicknessGroupStrings = new List<string>();
                    var thicknessAvgGroupStrings = new List<string>();
                    var widthGroupStrings = new List<string>();
                    var widthAvgGroupStrings = new List<string>();
                    var areaGroupStrings = new List<string>();
                    var weightGroupStrings = new List<string>();
                    var initiallengthGroupStrings = new List<string>();
                    var lengthGroupStrings = new List<string>();
                    var ELGroupStrings = new List<string>();
                    var MedianGroupStrings = new List<string>();
                    var AftercoolingLengthGroupStrings = new List<string>();
                    var AftercoolingELGroupStrings = new List<string>();
                    var AftercoolingMedianGroupStrings = new List<string>();

                    foreach (var coreSubGroup in coreSubGroups)
                    {
                        var coreRows = coreSubGroup.ToList();

                    
                        sampleGroupStrings.Add(string.Join(",", coreRows.Select(r => r["SampleType"]?.ToString() ?? "")));
                        specimenGroupStrings.Add(string.Join(",", coreRows.Select(r => r["SpecimenNo"]?.ToString() ?? "")));  // FIXED: SpecimenNo not SpecimenNumber

                    
                        thicknessGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Thickness"]?.ToString() ?? ""})")));  // FIXED
                        thicknessAvgGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["ThicknessAvgMin"]?.ToString() ?? ""})")));
                        widthGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["WidthDiameter"]?.ToString() ?? ""})")));
                        widthAvgGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["WidthAvgMin"]?.ToString() ?? ""})")));
                        areaGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Area"]?.ToString() ?? ""})")));

                        weightGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Weightload"]?.ToString() ?? ""})")));  // FIXED: Weightload not WeightLoad
                        initiallengthGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Initiallength"]?.ToString() ?? ""})")));  // FIXED: Initiallength not InitialLength
                        lengthGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Length"]?.ToString() ?? ""})")));
                        ELGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["EL"]?.ToString() ?? ""})")));
                        MedianGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Median"]?.ToString() ?? ""})")));
                        AftercoolingLengthGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["coolingLength"]?.ToString() ?? ""})")));  // FIXED: coolingLength not CoolingLength
                        AftercoolingELGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["coolingEL"]?.ToString() ?? ""})")));  // FIXED: coolingEL not CoolingEL
                        AftercoolingMedianGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["coolingMedian"]?.ToString() ?? ""})")));  // FIXED: coolingMedian not CoolingMedian
                    }

                    // Join all core groups with semicolon
                    sampleGroups.Add(string.Join(";", sampleGroupStrings));
                    specimenNoGroups.Add(string.Join(";", specimenGroupStrings));
                    thicknessGroups.Add(string.Join(";", thicknessGroupStrings));
                    thicknessAvgGroups.Add(string.Join(";", thicknessAvgGroupStrings));
                    widthGroups.Add(string.Join(";", widthGroupStrings));
                    widthAvgGroups.Add(string.Join(";", widthAvgGroupStrings));
                    areaGroups.Add(string.Join(";", areaGroupStrings));

                
                    weightGroups.Add(string.Join(";", weightGroupStrings));
                    initialLengthGroups.Add(string.Join(";", initiallengthGroupStrings));
                    lengthGroups.Add(string.Join(";", lengthGroupStrings));
                    elGroups.Add(string.Join(";", ELGroupStrings));
                    medianGroups.Add(string.Join(";", MedianGroupStrings));
                    afterCoolingLengthGroups.Add(string.Join(";", AftercoolingLengthGroupStrings));
                    afterCoolingELGroups.Add(string.Join(";", AftercoolingELGroupStrings));
                    afterCoolingMedianGroups.Add(string.Join(";", AftercoolingMedianGroupStrings));
                }

                // Prepare parameters for saving
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Add upper section values
                reportParamMapList.Add("17640"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17641"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17642"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17643"); resultList.Add(Date.Text);
                reportParamMapList.Add("17644"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17645"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17646"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17647"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17648"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17649"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17650"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17651"); resultList.Add(equipmentno.Text);

                // Add grid data
                reportParamMapList.Add("17663"); resultList.Add(string.Join("|", materialGroupList));
                reportParamMapList.Add("17662"); resultList.Add(string.Join("|", coreGroups));              // FIXED: Use 17397 for core
                reportParamMapList.Add("17661"); resultList.Add(string.Join("|", sampleGroups));
                reportParamMapList.Add("17677"); resultList.Add(string.Join("|", specimenNoGroups));
                reportParamMapList.Add("17678"); resultList.Add(string.Join("|", thicknessGroups));
                reportParamMapList.Add("17679"); resultList.Add(string.Join("|", thicknessAvgGroups));
                reportParamMapList.Add("17680"); resultList.Add(string.Join("|", widthGroups));
                reportParamMapList.Add("17681"); resultList.Add(string.Join("|", widthAvgGroups));
                reportParamMapList.Add("17652"); resultList.Add(string.Join("|", areaGroups));

                reportParamMapList.Add("17653"); resultList.Add(string.Join("|", weightGroups));
                reportParamMapList.Add("17654"); resultList.Add(string.Join("|", initialLengthGroups));
                reportParamMapList.Add("17655"); resultList.Add(string.Join("|", lengthGroups));
                reportParamMapList.Add("17656"); resultList.Add(string.Join("|", elGroups));
                reportParamMapList.Add("17657"); resultList.Add(string.Join("|", medianGroups));
                reportParamMapList.Add("17658"); resultList.Add(string.Join("|", afterCoolingLengthGroups));
                reportParamMapList.Add("17659"); resultList.Add(string.Join("|", afterCoolingELGroups));
                reportParamMapList.Add("17660"); resultList.Add(string.Join("|", afterCoolingMedianGroups));

                // Specified Values
                reportParamMapList.Add("17664"); resultList.Add(txtSpecificThickness.Text);
                reportParamMapList.Add("17665"); resultList.Add(txtSpecificDiameter.Text);
                reportParamMapList.Add("17666"); resultList.Add(txtSpecificArea.Text);
                reportParamMapList.Add("17667"); resultList.Add(txtSpecificWeightload.Text);
                reportParamMapList.Add("17668"); resultList.Add(txtSpecificInitiallength.Text);
                reportParamMapList.Add("17669"); resultList.Add(txtSpecificLength.Text);
                reportParamMapList.Add("17670"); resultList.Add(txtSpecificEL.Text);
                reportParamMapList.Add("17671"); resultList.Add(txtSpecificMedian.Text);
                reportParamMapList.Add("17672"); resultList.Add(txtSpecificoolingLength.Text);
                reportParamMapList.Add("17673"); resultList.Add(txtSpecificoolingEL.Text);
                reportParamMapList.Add("17674"); resultList.Add(txtSpecificoolingMedian.Text);

                reportParamMapList.Add("17675"); resultList.Add(txtaftertemp.Text);
                reportParamMapList.Add("17676"); resultList.Add(txtunderloadatTemp.Text);

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

                    Response.Redirect($"GenerateTestRecord_HotSetRev01.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            bool gridHasData = (gvPressureTest.Rows.Count > 0);


            // Helper function to validate inputs allowing "-" as zero
            bool IsValidNumberOrDash(string input)
            {
                if (input.Trim() == "-") return true;
                return double.TryParse(input, out _);
            }

            // General validations
            if (string.IsNullOrEmpty(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }

            if (string.IsNullOrEmpty(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }

            if (string.IsNullOrEmpty(roomhumidity.Text) || !IsValidNumberOrDash(roomhumidity.Text))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }

            if (string.IsNullOrEmpty(roomtemp.Text) || !IsValidNumberOrDash(roomtemp.Text))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }


            if (!gridHasData)
            {
                if (ddlCorePhase.SelectedIndex == -1)
                {
                    errors.Add("Core Phase is required.");
                    invalidControls.Add(ddlCorePhase);
                }

                if (string.IsNullOrWhiteSpace(txtSpecimenNumber.Text) || !int.TryParse(txtSpecimenNumber.Text.Trim(), out _))
                {
                    errors.Add("Specimen No. must be a valid integer.");
                    invalidControls.Add(txtSpecimenNumber);
                }

                // Validate your new input textboxes here (thickness and diameter)
                TextBox[] newTestInputs = {
            txtThickness,txtdiameterInputs,
            txtthicknessInputAvg, txtdiameterInputAvg,txtArea,txtWeightload,txtInitiallength,txtlengthafter,
            txtELafter,txtaftercoolinglength,txtaftercoolingEL,txtaftercoolingMedian,
            txtWeightload
        };

                foreach (var txt in newTestInputs)
                {
                    if (string.IsNullOrWhiteSpace(txt.Text) || !IsValidNumberOrDash(txt.Text))
                    {
                        errors.Add($"{txt.ID.Replace("txt", "")} must be a valid number or '-'.");
                        invalidControls.Add(txt);
                    }
                }
            }


            ResetControlBorders(this);

            if (errors.Count > 0)
            {
                foreach (var control in invalidControls)
                {
                    if (control is TextBox txt)
                    {
                        txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
                    }
                    else if (control is DropDownList ddl)
                    {
                        ddl.CssClass += " input-error";
                    }
                }

                string errorMessage = "Validation Errors:\\n" + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);

                return; // stop further processing
            }

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
                    ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                        "alert('Sample ID not found. Cannot update.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                    $"alert('Error updating test results: {ex.Message}');", true);
            }
        }
        protected void gvPressureTest_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView gv = (GridView)sender;
                e.Row.Controls.Clear();

                string afterTempText = string.IsNullOrWhiteSpace(txtaftertemp.Text) ? "___" : txtaftertemp.Text;
                string underLoadTempText = string.IsNullOrWhiteSpace(txtunderloadatTemp.Text) ? "___" : txtunderloadatTemp.Text;

                // ---------------- Row 1 (Main Headers) ----------------
                GridViewRow row1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row1.Cells.Add(CreateHeaderCell("Material", 3, 1));                    // rowspan=3, colspan=1
                row1.Cells.Add(CreateHeaderCell("Core Phase", 3, 1));                 // rowspan=3, colspan=1
                row1.Cells.Add(CreateHeaderCell("Type of Sample Test", 3, 1));        // rowspan=3, colspan=1
                row1.Cells.Add(CreateHeaderCell("Specimen Number", 3, 1));            // rowspan=3, colspan=1
                row1.Cells.Add(CreateHeaderCell("Thickness", 1, 2));                  // rowspan=1, colspan=2
                row1.Cells.Add(CreateHeaderCell("Diameter/Width", 1, 2));             // rowspan=1, colspan=2
                row1.Cells.Add(CreateHeaderCell("Area", 2, 1));                       // rowspan=2, colspan=1
                row1.Cells.Add(CreateHeaderCell("Weight Load", 2, 1));                // rowspan=2, colspan=1
                row1.Cells.Add(CreateHeaderCell("Initial Length", 2, 1));             // rowspan=2, colspan=1
                row1.Cells.Add(CreateHeaderCell($"After: {afterTempText} under load at temperature: {underLoadTempText}°C", 1, 3)); // rowspan=1, colspan=3
                row1.Cells.Add(CreateHeaderCell("After Cooling", 1, 3));              // rowspan=1, colspan=3
                row1.Cells.Add(CreateHeaderCell("Action", 3, 1));                   // rowspan=3, colspan=1
                gv.Controls[0].Controls.AddAt(0, row1);

                // ---------------- Row 2 (Sub Headers) ----------------
                GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
                // Note: Material, Core Phase, Type of Sample Test, Specimen Number are rowspan=3, so they don't appear in row2
                row2.Cells.Add(CreateHeaderCell("Inputs", 1, 1));                     // Thickness Inputs
                row2.Cells.Add(CreateHeaderCell("Min Max", 1, 1));                    // Thickness Min Max
                row2.Cells.Add(CreateHeaderCell("Inputs", 1, 1));                     // Diameter Inputs
                row2.Cells.Add(CreateHeaderCell("Min Max", 1, 1));                    // Diameter Min Max
                                                                                      // Note: Area, Weight Load, Initial Length are rowspan=2, so they don't appear in row2
                row2.Cells.Add(CreateHeaderCell("Length", 1, 1));                     // After temp - Length
                row2.Cells.Add(CreateHeaderCell("EL", 1, 1));                         // After temp - EL
                row2.Cells.Add(CreateHeaderCell("Median", 1, 1));                     // After temp - Median
                row2.Cells.Add(CreateHeaderCell("Length", 1, 1));                     // After Cooling - Length
                row2.Cells.Add(CreateHeaderCell("EL", 1, 1));                         // After Cooling - EL
                row2.Cells.Add(CreateHeaderCell("Median", 1, 1));                     // After Cooling - Median
                                                                                      // Note: Add Data is rowspan=3, so it doesn't appear in row2
                gv.Controls[0].Controls.AddAt(1, row2);

                // ---------------- Row 3 (Units) ----------------
                GridViewRow row3 = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Normal);
                // Note: Material, Core Phase, Type of Sample Test, Specimen Number, Add Data are rowspan=3, so they don't appear in row3
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // Thickness Inputs unit
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // Thickness Min Max unit
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // Diameter Inputs unit
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // Diameter Min Max unit
                row3.Cells.Add(CreateHeaderCell("mm²", 1, 1));                        // Area unit
                row3.Cells.Add(CreateHeaderCell("g", 1, 1));                          // Weight Load unit
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // Initial Length unit
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // After temp Length unit
                row3.Cells.Add(CreateHeaderCell("%", 1, 1));                          // After temp EL unit
                row3.Cells.Add(CreateHeaderCell("%", 1, 1));                          // After temp Median unit
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));                         // After Cooling Length unit
                row3.Cells.Add(CreateHeaderCell("%", 1, 1));                          // After Cooling EL unit
                row3.Cells.Add(CreateHeaderCell("%", 1, 1));                          // After Cooling Median unit
                gv.Controls[0].Controls.AddAt(2, row3);
            }
        }
        private TableHeaderCell CreateHeaderCell(string text, int rowSpan, int colSpan)
        {
            TableHeaderCell cell = new TableHeaderCell
            {
                Text = text,
                RowSpan = rowSpan,
                ColumnSpan = colSpan,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle
            };
            return cell;
        }

    }
}