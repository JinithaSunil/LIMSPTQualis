using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMSStaging
{
	public partial class HOTSET : System.Web.UI.Page
	{
<<<<<<< .mine
		protected void Page_Load(object sender, EventArgs e)
		{
||||||| .r236
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();
=======
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
>>>>>>> .r267

<<<<<<< .mine
		}
	}
||||||| .r236
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
            }
        }
        protected void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCategory", typeof(string));
            dt.Columns.Add("ItemType", typeof(string));
            dt.Columns.Add("SampleType", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(int));
            dt.Columns.Add("Weightload", typeof(string));

            // Add the 6 additional input columns
            dt.Columns.Add("Input1", typeof(string));
            dt.Columns.Add("Input2", typeof(string));
            dt.Columns.Add("Input3", typeof(string));
            dt.Columns.Add("Input4", typeof(string));
            dt.Columns.Add("Input5", typeof(string));
            dt.Columns.Add("Input6", typeof(string));
            //  Add the average column here too
            dt.Columns.Add("InputMin", typeof(decimal));

            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;
            // Bind the empty DataTable to the GridView
            gvHotsetTest.DataSource = dt;
            gvHotsetTest.DataBind();

            // Initialize Weightload Data grid
            DataTable dtWeightloadData = ViewState["WeightloadData"] as DataTable;
            if (dtWeightloadData == null)
            {
                dtWeightloadData = new DataTable();
                // Add CorePhase and ItemCategory first to match the click handler expectations
                dtWeightloadData.Columns.Add("CorePhase", typeof(string));
                dtWeightloadData.Columns.Add("ItemCategory", typeof(string));
                dtWeightloadData.Columns.Add("SpecimenNo", typeof(int));
                dtWeightloadData.Columns.Add("Weightload", typeof(string));
                dtWeightloadData.Columns.Add("Initiallength", typeof(string));
                dtWeightloadData.Columns.Add("After15MinLength", typeof(double));  // Changed to double
                dtWeightloadData.Columns.Add("After15MinEL", typeof(double));      // Changed to double
                dtWeightloadData.Columns.Add("After15MinMedian", typeof(double));  // Changed to double
                dtWeightloadData.Columns.Add("AftercoolingLength", typeof(double)); // Changed to double
                dtWeightloadData.Columns.Add("AftercoolingEL", typeof(double));     // Changed to double
                dtWeightloadData.Columns.Add("AftercoolingMedian", typeof(double)); // Changed to double
                dtWeightloadData.Columns.Add("Area", typeof(string));

                ViewState["WeightloadData"] = dtWeightloadData;
                gvWeightloadData.DataSource = dtWeightloadData;
                gvWeightloadData.DataBind();
            }
        }
        protected void ddlTestCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset Specimen Number to 1 on Test Category change
            txtSpecimenNo.Text = "1";
        }
        protected void ddlItemtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSpecimenNo.Text = "1";
        }
        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCorePhase.SelectedValue))
            {
                txtSpecimenNo.Text = "";
                ShowMessage("Please select a valid Core Phase.");
                return;
            }

            txtSpecimenNo.Text = "1";
        }
        protected void UpdateSpecimenNumber()
        {
            // Get the DataTable from ViewState
            DataTable dt = (DataTable)ViewState["TestResults"];

            // Calculate next specimen number
            int nextSpecimenNo = 1; // Default to 1 if no rows
            if (dt != null && dt.Rows.Count > 0)
            {
                nextSpecimenNo = dt.Rows.Count + 1;
            }

            // Find your actual control - you need to use the correct ID and search method
            // If it's in a form:
            TextBox txtSpecimenNo = FindControl("txtSpecimenNo") as TextBox;

         

            // Set the value if found
            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
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
                        PrefillTestResultsGrid(sampleId);
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
        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "10279");
            SetLabelText(cableSize, "10268");
            SetLabelText(voltage, "10269 ");
            SetTextBoxText(Date, " 10270");
            SetTextBoxText(roomno, "10272 ");
            SetTextBoxText(roomtemp, "10272 ");
            SetLabelText(refSpec, " 10273");
            SetLabelText(testMethodNo, "10274 ");
            SetLabelText(jobOrderNo, " 10275");
            SetLabelText(worksheetno, "10276 ");
            SetTextBoxText(roomhumidity, "10277 ");
            SetLabelText(equipmentno, " 10278");
            SetTextBoxText(txtSpecificThickness, "10257");
            SetTextBoxText(txtSpecificWidth, "10258");
            SetTextBoxText(txtSpecificArea, "10259");
            SetTextBoxText(txtSpecificWeightload, "10260");
            SetTextBoxText(txtSpecificInitialLength, "10261");
            SetTextBoxText(txtSpecificAfter15MinLength, "10262");
            SetTextBoxText(txtSpecificAfter15MinEL, "10263");
            SetTextBoxText(txtSpecificAfter15MinMedian, "10264");
            SetTextBoxText(txtSpecificAftercoolingLength, "10265");
            SetTextBoxText(txtSpecificAftercoolingEL, "10266");
            SetTextBoxText(txtSpecificAftercoolingMedian, "10267");


            // Helper method to set text for labels
            void SetLabelText(Label label, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

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
        private void PrefillTestResultsGrid(int sampleId)
        
        {
            try
            {
                // Initialize grid if needed
                InitializeGrid();

                // Create DAL object if not exists
                if (_cm == null)
                    _cm = new DAL.CM();

                // Fetch existing test results for this sample
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
WITH ResultPivot AS (
    SELECT 
        SampleID,
        MAX(CASE WHEN ReportParamMapID = '10244' THEN Result END) AS XLPEDumbellThickness,
        MAX(CASE WHEN ReportParamMapID = '10245' THEN Result END) AS XLPEDumbellThicknessMin,
        MAX(CASE WHEN ReportParamMapID = '10246' THEN Result END) AS XLPEDumbellWidth,
        MAX(CASE WHEN ReportParamMapID = '10247' THEN Result END) AS XLPEDumbellWidthMin,
        MAX(CASE WHEN ReportParamMapID = '10248' THEN Result END) AS Area,
        MAX(CASE WHEN ReportParamMapID = '10249' THEN Result END) AS Weightload,
        MAX(CASE WHEN ReportParamMapID = '10250' THEN Result END) AS InitialLength,
        MAX(CASE WHEN ReportParamMapID = '10251' THEN Result END) AS After15MinLength,
        MAX(CASE WHEN ReportParamMapID = '10252' THEN Result END) AS After15MinEL,
        MAX(CASE WHEN ReportParamMapID = '10253' THEN Result END) AS After15MinMedian,
        MAX(CASE WHEN ReportParamMapID = '10254' THEN Result END) AS AftercoolingLength,
        MAX(CASE WHEN ReportParamMapID = '10255' THEN Result END) AS AftercoolingEL,
        MAX(CASE WHEN ReportParamMapID = '10256' THEN Result END) AS AftercoolingMedian
    FROM TestResultsPTQualis
    WHERE SampleID = @SampleID
    GROUP BY SampleID
)
SELECT 
    XLPEDumbellThickness,
    XLPEDumbellThicknessMin,
    XLPEDumbellWidth,
    XLPEDumbellWidthMin,
    Area,
    Weightload,
    InitialLength,
    After15MinLength,
    After15MinEL,
    After15MinMedian,
    AftercoolingLength,
    AftercoolingEL,
    AftercoolingMedian
FROM ResultPivot";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Clear existing data
                                DataTable dtPressureTest = ViewState["TestResults"] as DataTable;
                                if (dtPressureTest != null)
                                    dtPressureTest.Clear();

                                // Process test results for main grid - always use "XLPE" as ItemType
                                // Use ItemType = "XLPE", ItemCategory = "Thickness"
                                ProcessThicknessWidthData(
                                    reader["XLPEDumbellThickness"].ToString(),
                                    reader["XLPEDumbellThicknessMin"].ToString(),
                                    "Thickness",
                                    "XLPE", "Dumbell");

                                // Use ItemType = "XLPE", ItemCategory = "Width"
                                ProcessThicknessWidthData(
                                    reader["XLPEDumbellWidth"].ToString(),
                                    reader["XLPEDumbellWidthMin"].ToString(),
                                    "Width",
                                    "XLPE",
                                    "Dumbell");


                                // Process weightload data for second grid
                                ProcessWeightloadData(
                                    reader["Area"].ToString(),
                                    reader["Weightload"].ToString(),
                                    reader["InitialLength"].ToString(),
                                    reader["After15MinLength"].ToString(),
                                    reader["After15MinEL"].ToString(),
                                    reader["After15MinMedian"].ToString(),
                                    reader["AftercoolingLength"].ToString(),
                                    reader["AftercoolingEL"].ToString(),
                                    reader["AftercoolingMedian"].ToString()
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error prefilling test results grid: {ex.Message}');", true);
            }
        }

        // Process thickness and width data separately with fixed ItemType as "XLPE"
        private void ProcessThicknessWidthData(string testData, string minData, string itemCategory, string itemType , string sampleType)
        {
            if (string.IsNullOrEmpty(testData)) return;

            // Split by | for different cores (Red, Blue, Yellow)
            string[] coreData = testData.Split('|');
            string[] minValues = !string.IsNullOrEmpty(minData) ? minData.Split('|') : new string[0];
            DataTable dt = ViewState["TestResults"] as DataTable;

            if (dt == null) return;

            for (int coreIndex = 0; coreIndex < coreData.Length; coreIndex++)
            {
                string coreEntry = coreData[coreIndex];
                // Parse core data (format: "Red;1:1,2,3,4,5,6")
                string[] coreAndValues = coreEntry.Split(';');
                if (coreAndValues.Length < 2) continue;

                string corePhase = coreAndValues[0]; // Red, Blue, or Yellow

                // Parse specimen and values (format: "1:1,2,3,4,5,6")
                string[] specimenAndValues = coreAndValues[1].Split(':');
                if (specimenAndValues.Length < 2) continue;

                int specimenNo;
                if (!int.TryParse(specimenAndValues[0], out specimenNo))
                    continue;

                string[] inputValues = specimenAndValues[1].Split(',');

                bool duplicateExists = dt.AsEnumerable().Any(row =>
                    row["CorePhase"].ToString() == corePhase &&
                    Convert.ToInt32(row["SpecimenNo"]) == specimenNo &&
                    row["ItemCategory"].ToString() == itemCategory
                );

                if (duplicateExists)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DuplicateError",
                        $"alert('Data for Core Phase \'{corePhase}\', Specimen No. \'{specimenNo}\', and Category \'{itemCategory}\' already exists.');", true);
                    continue;
                }

                // Create a new row for this test data
                DataRow newRow = dt.NewRow();
                newRow["ItemCategory"] = itemCategory;
                newRow["ItemType"] = itemType;  // Always use the provided itemType (XLPE)
                newRow["SampleType"] = sampleType;
                newRow["CorePhase"] = corePhase;
                newRow["SpecimenNo"] = specimenNo;

                // Fill in all input values for this test
                for (int i = 0; i < Math.Min(inputValues.Length, 6); i++)
                {
                    newRow[$"Input{i + 1}"] = inputValues[i];
                }

                // Set the min value
                if (coreIndex < minValues.Length)
                {
                    decimal min;
                    if (decimal.TryParse(minValues[coreIndex], out min))
                    {
                        newRow["InputMin"] = min;
                    }
                }

                dt.Rows.Add(newRow);
            }

            // Sort the data
            DataView dv = dt.DefaultView;
            dv.Sort = "ItemCategory ASC, CorePhase ASC, SpecimenNo ASC";
            dt = dv.ToTable();
            ViewState["TestResults"] = dt;

            // Rebind grid
            gvHotsetTest.DataSource = dt;
            gvHotsetTest.DataBind();
        }

        // Process weightload data for second grid
        // Process weightload data for second grid
        private void ProcessWeightloadData(string areaData, string weightloadData, string initialLengthData,
                                   string after15MinLengthData, string after15MinELData,
                                   string after15MinMedianData, string coolingLengthData,
                                   string coolingELData, string coolingMedianData)
        {
            try
            {
                // Create a new DataTable for weightload data
                DataTable dtWeightload = new DataTable();
                dtWeightload.Columns.Add("CorePhase", typeof(string));
                dtWeightload.Columns.Add("ItemCategory", typeof(string));
                dtWeightload.Columns.Add("SpecimenNo", typeof(int));
                dtWeightload.Columns.Add("Area", typeof(string));
                dtWeightload.Columns.Add("Weightload", typeof(string));
                dtWeightload.Columns.Add("Initiallength", typeof(string));
                dtWeightload.Columns.Add("After15MinLength", typeof(string));
                dtWeightload.Columns.Add("After15MinEL", typeof(string));
                dtWeightload.Columns.Add("After15MinMedian", typeof(string));
                dtWeightload.Columns.Add("AftercoolingLength", typeof(string));
                dtWeightload.Columns.Add("AftercoolingEL", typeof(string));
                dtWeightload.Columns.Add("AftercoolingMedian", typeof(string));

                // Dictionary to store all data by core_specimen key
                Dictionary<string, Dictionary<string, string>> allData = new Dictionary<string, Dictionary<string, string>>();

                // Parse all data types
                string[] dataTypes = { areaData, weightloadData, initialLengthData, after15MinLengthData,
                              after15MinELData, after15MinMedianData, coolingLengthData,
                              coolingELData, coolingMedianData };

                string[] columnNames = { "Area", "Weightload", "Initiallength", "After15MinLength",
                                "After15MinEL", "After15MinMedian", "AftercoolingLength",
                                "AftercoolingEL", "AftercoolingMedian" };

                for (int i = 0; i < dataTypes.Length; i++)
                {
                    string data = dataTypes[i];
                    string columnName = columnNames[i];

                    if (string.IsNullOrEmpty(data)) continue;

                    // Split by | for different entries (Red;1:7|Red;2:17)
                    string[] entries = data.Split('|');

                    foreach (string entry in entries)
                    {
                        if (string.IsNullOrEmpty(entry)) continue;

                        // Parse entry (format: "Red;1:7")
                        string[] coreAndValues = entry.Split(';');
                        if (coreAndValues.Length < 2) continue;

                        string corePhase = coreAndValues[0]; // Red, Blue, Yellow

                        // Parse specimen and value (format: "1:7")
                        string[] specimenAndValue = coreAndValues[1].Split(':');
                        if (specimenAndValue.Length < 2) continue;

                        string specimenNo = specimenAndValue[0];
                        string value = specimenAndValue[1];

                        // Create unique key for core + specimen
                        string key = $"{corePhase}_{specimenNo}";

                        // Initialize dictionary if not exists
                        if (!allData.ContainsKey(key))
                        {
                            allData[key] = new Dictionary<string, string>();
                        }

                        // Store the value for this column
                        allData[key][columnName] = value;
                    }
                }

                // Create rows from the parsed data
                foreach (var coreData in allData)
                {
                    string[] coreAndSpecimen = coreData.Key.Split('_');
                    if (coreAndSpecimen.Length == 2)
                    {
                        string corePhase = coreAndSpecimen[0];
                        int specimenNo = Convert.ToInt32(coreAndSpecimen[1]);

                        DataRow newRow = dtWeightload.NewRow();
                        newRow["CorePhase"] = corePhase;
                        newRow["SpecimenNo"] = specimenNo;

                        // Fill in all the values for this core/specimen combination
                        foreach (var dataValue in coreData.Value)
                        {
                            newRow[dataValue.Key] = dataValue.Value;
                        }

                        dtWeightload.Rows.Add(newRow);
                    }
                }

                // Sort the data by CorePhase and SpecimenNo
                DataView dv = dtWeightload.DefaultView;
                dv.Sort = "CorePhase ASC, SpecimenNo ASC";
                dtWeightload = dv.ToTable();

                // Store in ViewState
                ViewState["WeightloadData"] = dtWeightload;

                // Bind to grid (assuming you have a grid for weightload data)
                if (gvWeightloadData != null)
                {
                    gvWeightloadData.DataSource = dtWeightload;
                    gvWeightloadData.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "WeightloadDataError",
                    $"alert('Error processing weightload data: {ex.Message}');", true);
            }
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0;
            decimal inputMin = 0;

            if (ddlSampleType.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(ddlSampleType.SelectedValue))
            {
                ddlSampleType.CssClass += " input-error";
                invalidControls.Add(ddlSampleType);
                hasError = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a valid Sample Type.');", true);
            }
            // Validate Core Phase
            if (ddlCorePhase.SelectedIndex <= -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                ddlCorePhase.CssClass += " input-error";
                invalidControls.Add(ddlCorePhase);
                hasError = true;
            }

            // Validate Specimen Number
            if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out specimenNo))
            {
                invalidControls.Add(txtSpecimenNo);
                hasError = true;
            }

            if (ddlitemtype.SelectedIndex == 0 || ddlitemtype.SelectedValue == "Select")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a valid ItemType.');", true);
                ddlitemtype.CssClass += " input-error";
                hasError = true;
            }
            // Validate Item Category
            if (ddlTestCategory.SelectedIndex == 0 || ddlTestCategory.SelectedValue == "Select")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a valid ItemCategory.');", true);
                ddlTestCategory.CssClass += " input-error";
                hasError = true;
            }

            // Validate Input1 to Input6
            TextBox[] inputTextBoxes = { txtInput1, txtInput2, txtInput3, txtInput4, txtInput5, txtInput6 };
            foreach (TextBox txt in inputTextBoxes)
            {
                if (string.IsNullOrWhiteSpace(txt.Text.Trim()))
                {
                    invalidControls.Add(txt);
                    hasError = true;
                }
            }

            // Validate Average
            if (string.IsNullOrWhiteSpace(txtMin.Text) || !decimal.TryParse(txtMin.Text.Trim(), out inputMin))
            {
                invalidControls.Add(txtMin);
                hasError = true;
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

            // Get or create DataTable from ViewState
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("CorePhase", typeof(string));    
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("ItemCategory", typeof(string));               
                dt.Columns.Add("ItemType", typeof(string));
                dt.Columns.Add("SampleType", typeof(string));
                dt.Columns.Add("Input1", typeof(string));
                dt.Columns.Add("Input2", typeof(string));
                dt.Columns.Add("Input3", typeof(string));
                dt.Columns.Add("Input4", typeof(string));
                dt.Columns.Add("Input5", typeof(string));
                dt.Columns.Add("Input6", typeof(string));
                dt.Columns.Add("InputMin", typeof(decimal));
            }

            // Check for duplicates
            bool duplicateExists = dt.AsEnumerable().Any(row =>
                row["CorePhase"].ToString() == ddlCorePhase.SelectedValue &&
                Convert.ToInt32(row["SpecimenNo"]) == specimenNo &&
                row["ItemCategory"].ToString() == ddlTestCategory.SelectedItem.Text &&
                 row["SampleType"].ToString() == ddlSampleType.SelectedItem.Text
            );

            if (duplicateExists)
            {
                ShowMessage($"Data for Core Phase '{ddlCorePhase.SelectedValue}', Specimen No. '{specimenNo}', and Category '{ddlTestCategory.SelectedItem.Text}' already exists. Please delete the existing entry before adding a new one.");
                txtSpecimenNo.Text = (specimenNo + 1).ToString();
                return;

            }

            // Add new row
            DataRow dr = dt.NewRow();
            dr["ItemType"] = ddlitemtype.SelectedItem.Text;
            dr["ItemCategory"] = ddlTestCategory.SelectedItem.Text;
            dr["SampleType"] = ddlSampleType.SelectedItem.Text;
            dr["CorePhase"] = ddlCorePhase.SelectedValue;
            dr["SpecimenNo"] = specimenNo; // Use original, not incremented value
            dr["Input1"] = txtInput1.Text.Trim();
            dr["Input2"] = txtInput2.Text.Trim();
            dr["Input3"] = txtInput3.Text.Trim();
            dr["Input4"] = txtInput4.Text.Trim();
            dr["Input5"] = txtInput5.Text.Trim();
            dr["Input6"] = txtInput6.Text.Trim();
            dr["InputMin"] = inputMin;

            dt.Rows.Add(dr);

            // Save DataTable to ViewState
            ViewState["TestResults"] = dt;

            // Rebind GridView
            gvHotsetTest.DataSource = dt;
            gvHotsetTest.DataBind();

            // Set specimen and phase for weightload
            ViewState["CurrentSpecimenForWeightload"] = specimenNo;
            ViewState["CurrentPhase"] = ddlCorePhase.SelectedValue;

            txtspeicimentnumebrForweghightload.Text = specimenNo.ToString();
            lblPhaseForWeightload.Text = ddlCorePhase.SelectedValue;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertWeightload", "alert('Please add data for Weightload.');", true);
            // Increment for next entry (AFTER adding to grid)
            txtSpecimenNo.Text = (specimenNo + 1).ToString();

            // Clear fields
            ClearInputFields();
        }
        protected void btnAddWeightloadData_Click(object sender, EventArgs e)
        {
            // Helper method inside
            bool TryParseOrDashAsZero(string input, out double result)
            {
                if (input == "-")
                {
                    result = 0;
                    return true;
                }
                return double.TryParse(input, out result);
            }

            // Get test data from ViewState
            DataTable dtTestResults = ViewState["TestResults"] as DataTable;
            if (dtTestResults == null || dtTestResults.Rows.Count == 0)
            {
                ShowMessage("Please add specimen data first before adding weightload data.");
                return;
            }

            // Get current selections
            string currentCategory = ddlTestCategory.SelectedItem.Text;
            string currentPhase = ddlCorePhase.SelectedValue;

            var matchingRows = dtTestResults.AsEnumerable()
                .Where(row => row["CorePhase"].ToString() == currentPhase &&
                              row["ItemCategory"].ToString() == currentCategory);

            if (!matchingRows.Any())
            {
                ShowMessage("Please add specimen data for this Core Phase and Category before adding weightload data.");
                return;
            }

            int currentSpecimen = 1;
            DataTable dtWeightloadData = ViewState["WeightloadData"] as DataTable;
            if (dtWeightloadData != null)
            {
                var existingSpecimens = dtWeightloadData.AsEnumerable()
                    .Where(row => row["CorePhase"].ToString() == currentPhase);

                if (existingSpecimens.Any())
                {
                    currentSpecimen = existingSpecimens.Max(row => Convert.ToInt32(row["SpecimenNo"])) + 1;
                    ShowMessage($"Data already exists for Core Phase '{currentPhase}', assigning next specimen number: {currentSpecimen}");
                }
            }

            // Read inputs as-is (preserve "-" exactly)
            string weightloadInput = txtWeightload.Text.Trim();
            string areaInput = txtarea.Text.Trim();
            string initialLengthInput = txtInitiallength.Text.Trim();
            string after15MinLengthInput = txtAfter15Minlength.Text.Trim();
            string after15MinELInput = txtAfter15MinEL.Text.Trim();
            string afterCoolingLengthInput = txtaftercoolinglength.Text.Trim();
            string afterCoolingELInput = txtaftercoolingEL.Text.Trim();

            // Validate numeric inputs (treat "-" as zero temporarily for validation)
            if (!TryParseOrDashAsZero(after15MinLengthInput, out double after15MinLengthVal) ||
                !TryParseOrDashAsZero(after15MinELInput, out double after15MinELVal) ||
                !TryParseOrDashAsZero(afterCoolingLengthInput, out double afterCoolingLengthVal) ||
                !TryParseOrDashAsZero(afterCoolingELInput, out double afterCoolingELVal))
            {
                ShowMessage("Please enter valid numeric values for lengths and ELs.");
                return;
            }

            // Calculate medians based on numeric values
            double after15MinMedian = Math.Round((after15MinLengthVal + after15MinELVal) / 2.0, 2);
            double afterCoolingMedian = Math.Round((afterCoolingLengthVal + afterCoolingELVal) / 2.0, 2);

            // Initialize WeightloadData table if null
            if (dtWeightloadData == null)
            {
                dtWeightloadData = new DataTable();
                dtWeightloadData.Columns.Add("CorePhase", typeof(string));
                dtWeightloadData.Columns.Add("ItemCategory", typeof(string));
                dtWeightloadData.Columns.Add("SpecimenNo", typeof(int));
                dtWeightloadData.Columns.Add("Weightload", typeof(string));
                dtWeightloadData.Columns.Add("Initiallength", typeof(string));
                dtWeightloadData.Columns.Add("After15MinLength", typeof(string));  // Store as string to preserve "-"
                dtWeightloadData.Columns.Add("After15MinEL", typeof(string));      // Store as string to preserve "-"
                dtWeightloadData.Columns.Add("After15MinMedian", typeof(double));
                dtWeightloadData.Columns.Add("AftercoolingLength", typeof(string)); // Store as string to preserve "-"
                dtWeightloadData.Columns.Add("AftercoolingEL", typeof(string));     // Store as string to preserve "-"
                dtWeightloadData.Columns.Add("AftercoolingMedian", typeof(double));
                dtWeightloadData.Columns.Add("Area", typeof(string));
            }

            DataRow newRow = dtWeightloadData.NewRow();
            newRow["CorePhase"] = currentPhase;
            newRow["ItemCategory"] = currentCategory;
            newRow["SpecimenNo"] = currentSpecimen;

            // Store exact input strings (preserve "-" exactly)
            newRow["Weightload"] = weightloadInput;
            newRow["Initiallength"] = initialLengthInput;
            newRow["After15MinLength"] = after15MinLengthInput;
            newRow["After15MinEL"] = after15MinELInput;
            newRow["After15MinMedian"] = after15MinMedian;
            newRow["AftercoolingLength"] = afterCoolingLengthInput;
            newRow["AftercoolingEL"] = afterCoolingELInput;
            newRow["AftercoolingMedian"] = afterCoolingMedian;
            newRow["Area"] = areaInput;

            dtWeightloadData.Rows.Add(newRow);

            ViewState["WeightloadData"] = dtWeightloadData;

            gvWeightloadData.DataSource = dtWeightloadData;
            gvWeightloadData.DataBind();

            txtspeicimentnumebrForweghightload.Text = currentSpecimen.ToString();
            txtspeicimentnumebrForweghightload.Enabled = false;

            // Clear inputs for next entry
            txtWeightload.Text = "";
            txtInitiallength.Text = "";
            txtAfter15Minlength.Text = "";
            txtAfter15MinEL.Text = "";
            txtaftercoolinglength.Text = "";
            txtaftercoolingEL.Text = "";
            txtarea.Text = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "DebugInfo", $"console.log('Added specimen {currentSpecimen} for {currentPhase}');", true);
        }


        string lastCorePhase = string.Empty;
           int lastSpecimenNo = -1;
       protected void gvWeightloadData_RowCreated(object sender, GridViewRowEventArgs e)
{
    if (e.Row.RowType == DataControlRowType.Header)
    {
        // Clear existing header cells
        e.Row.Cells.Clear();
        
        // Create first header row
        GridViewRow headerRow1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
        
        // Add cells with proper rowspan/colspan
        headerRow1.Cells.Add(CreateHeaderCell("Core Phase", 2, 1)); // RowSpan=2
        headerRow1.Cells.Add(CreateHeaderCell("Specimen No", 2, 1)); // RowSpan=2
        headerRow1.Cells.Add(CreateHeaderCell("Area (mm²)", 2, 1)); // RowSpan=2
        headerRow1.Cells.Add(CreateHeaderCell("Weight Load (gr)", 2, 1)); // RowSpan=2
        headerRow1.Cells.Add(CreateHeaderCell("Initial Length (mm)", 2, 1)); // RowSpan=2
        
        // After 15 min section (colspan=3)
        headerRow1.Cells.Add(CreateHeaderCell("After 15 min under Load at 200°C", 1, 3));
        
        // After Cooling section (colspan=3)
        headerRow1.Cells.Add(CreateHeaderCell("After Cooling", 1, 3));
        
        // Action (RowSpan=2)
        headerRow1.Cells.Add(CreateHeaderCell("Action", 2, 1));
        
        // Create second header row
        GridViewRow headerRow2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
        
        // Add subheaders for After 15 min section
        headerRow2.Cells.Add(CreateHeaderCell("Length (mm)", 1, 1));
        headerRow2.Cells.Add(CreateHeaderCell("EL (%)", 1, 1));
        headerRow2.Cells.Add(CreateHeaderCell("Median (%)", 1, 1));
        
        // Add subheaders for After Cooling section
        headerRow2.Cells.Add(CreateHeaderCell("Length (mm)", 1, 1));
        headerRow2.Cells.Add(CreateHeaderCell("EL (%)", 1, 1));
        headerRow2.Cells.Add(CreateHeaderCell("Median (%)", 1, 1));
        
        // Add the header rows to the GridView
        Table gridTable = (Table)gvWeightloadData.Controls[0];
        gridTable.Rows.AddAt(0, headerRow1);
        gridTable.Rows.AddAt(1, headerRow2);
        
        // Hide the default header row
        e.Row.Visible = false;
    }
}
 
        private TableHeaderCell CreateHeaderCell(string text, int rowSpan, int colSpan)
{
    TableHeaderCell cell = new TableHeaderCell();
    cell.Text = text;
    cell.RowSpan = rowSpan;
    cell.ColumnSpan = colSpan;
    cell.HorizontalAlign = HorizontalAlign.Center;
    cell.VerticalAlign = VerticalAlign.Middle;
    cell.CssClass = "text-center align-middle";
    return cell;
}
        protected void gvWeightloadData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string currentCorePhase = DataBinder.Eval(e.Row.DataItem, "CorePhase")?.ToString();
                int currentSpecimenNo = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "SpecimenNo"));

                // Get median values
                string after15MinMedian = DataBinder.Eval(e.Row.DataItem, "After15MinMedian")?.ToString();
                string afterCoolingMedian = DataBinder.Eval(e.Row.DataItem, "AftercoolingMedian")?.ToString();

                // Column indices (adjust if your column order is different)
                int after15MinMedianColIndex = 7;  // 8th column (0-based index 7)
                int afterCoolingMedianColIndex = 10; // 11th column (0-based index 10)

                if (currentCorePhase == lastCorePhase && currentSpecimenNo == lastSpecimenNo)
                {
                    // Hide median cells for repeated rows
                    e.Row.Cells[after15MinMedianColIndex].Text = "";
                    e.Row.Cells[afterCoolingMedianColIndex].Text = "";
                }
                else
                {
                    // Show median values
                    e.Row.Cells[after15MinMedianColIndex].Text = after15MinMedian;
                    e.Row.Cells[afterCoolingMedianColIndex].Text = afterCoolingMedian;

                    lastCorePhase = currentCorePhase;
                    lastSpecimenNo = currentSpecimenNo;
                }

                // Center-align all cells
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }
        protected void gvWeightloadData_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;

            // Get the current data in the weightload data grid from ViewState
            DataTable dtWeightloadData = ViewState["WeightloadData"] as DataTable;

            if (dtWeightloadData != null && dtWeightloadData.Rows.Count > 0)
            {
                // Remove the selected row
                dtWeightloadData.Rows.RemoveAt(index);

                // Update ViewState and rebind the grid
                ViewState["WeightloadData"] = dtWeightloadData;
                gvWeightloadData.DataSource = dtWeightloadData;
                gvWeightloadData.DataBind();
            }

            // Get the current data in the pressure test grid
            DataTable dtTestResults = ViewState["TestResults"] as DataTable;

            // Check if there are any rows left in both grids
            bool isPressureTestGridEmpty = dtTestResults == null || dtTestResults.Rows.Count == 0;
            bool isWeightloadDataGridEmpty = dtWeightloadData == null || dtWeightloadData.Rows.Count == 0;

            // Enable btnAddToGrid button no matter if the Pressure Test Grid has data
            // btnAddToGrid.Enabled = true;

            //// Enable btnAddWeightloadData button only if there's data in the Pressure Test grid
            //if (!isPressureTestGridEmpty)
            //{
            //    btnAddWeightloadData.Enabled = true;
            //}
            //else
            //{
            //    btnAddWeightloadData.Enabled = false;
            //}

            // Show message
            ShowMessage("Record deleted successfully.");
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvHotsetTest.Rows.Count > 0);

            // Validate Main Section (required regardless of grid data)
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
                errors.Add("Standard Thickness must be a valid number.");
                invalidControls.Add(txtSpecificThickness);
            }
            if (txtSpecificWidth.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificWidth.Text) || !double.TryParse(txtSpecificWidth.Text, out _)))
            {
                errors.Add("Standard Width must be a valid number.");
                invalidControls.Add(txtSpecificWidth);
            }
            if (txtSpecificArea.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificArea.Text) || !double.TryParse(txtSpecificArea.Text, out _)))
            {
                errors.Add("Standard Are value must be a valid number.");
                invalidControls.Add(txtSpecificArea);
            }
            if (txtSpecificInitialLength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificInitialLength.Text) || !double.TryParse(txtSpecificInitialLength.Text, out _)))
            {
                errors.Add("Standard Initial Length  value must be a valid number.");
                invalidControls.Add(txtSpecificInitialLength);
            }
            if (txtSpecificWeightload.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificWeightload.Text) || !double.TryParse(txtSpecificWeightload.Text, out _)))
            {
                errors.Add("Standard Weightload value must be a valid number.");
                invalidControls.Add(txtSpecificWeightload);
            }
            if (txtSpecificAfter15MinLength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificAfter15MinLength.Text) || !double.TryParse(txtSpecificAfter15MinLength.Text, out _)))
            {
                errors.Add("Standard length value must be a valid number.");
                invalidControls.Add(txtSpecificAfter15MinLength);
            }
            if (txtSpecificAfter15MinEL.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificAfter15MinEL.Text) || !double.TryParse(txtSpecificAfter15MinEL.Text, out _)))
            {
                errors.Add("Standard EL value must be a valid number.");
                invalidControls.Add(txtSpecificAfter15MinEL);
            }
            if (txtSpecificAfter15MinMedian.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificAfter15MinMedian.Text) || !double.TryParse(txtSpecificAfter15MinMedian.Text, out _)))
            {
                errors.Add("Standard Median value must be a valid number.");
                invalidControls.Add(txtSpecificAfter15MinMedian);
            }
            if (txtSpecificAftercoolingEL.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificAftercoolingEL.Text) || !double.TryParse(txtSpecificAftercoolingEL.Text, out _)))
            {
                errors.Add("Standard EL value must be a valid number.");
                invalidControls.Add(txtSpecificAftercoolingEL);
            }
            if (txtSpecificAftercoolingLength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificAftercoolingLength.Text) || !double.TryParse(txtSpecificAftercoolingLength.Text, out _)))
            {
                errors.Add("Standard Length value must be a valid number.");
                invalidControls.Add(txtSpecificAftercoolingLength);
            }
            if (txtSpecificAftercoolingMedian.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificAftercoolingMedian.Text) || !double.TryParse(txtSpecificAftercoolingMedian.Text, out _)))
            {
                errors.Add("Standard Median value must be a valid number.");
                invalidControls.Add(txtSpecificAftercoolingMedian);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                if (txtWeightload.Text.Trim() != "-" && (string.IsNullOrEmpty(txtWeightload.Text) || !double.TryParse(txtWeightload.Text, out _)))
                {
                    errors.Add("Weightload must be a valid number.");
                    invalidControls.Add(txtWeightload);
                }
                if (txtarea.Text.Trim() != "-" && (string.IsNullOrEmpty(txtarea.Text) || !double.TryParse(txtarea.Text, out _)))
                {
                    errors.Add("Area must be a valid number.");
                    invalidControls.Add(txtarea);
                }
                if (txtInitiallength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtInitiallength.Text) || !double.TryParse(txtInitiallength.Text, out _)))
                {
                    errors.Add("Initial length must be a valid number.");
                    invalidControls.Add(txtInitiallength);
                }
                if (txtAfter15Minlength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtAfter15Minlength.Text) || !double.TryParse(txtAfter15Minlength.Text, out _)))
                {
                    errors.Add("length must be a valid number.");
                    invalidControls.Add(txtAfter15Minlength);
                }
                if (txtAfter15MinEL.Text.Trim() != "-" && (string.IsNullOrEmpty(txtAfter15MinEL.Text) || !double.TryParse(txtAfter15MinEL.Text, out _)))
                {
                    errors.Add("EL must be a valid number.");
                    invalidControls.Add(txtAfter15MinEL);
                }
                if (txtaftercoolingEL.Text.Trim() != "-" && (string.IsNullOrEmpty(txtaftercoolingEL.Text) || !double.TryParse(txtaftercoolingEL.Text, out _)))
                {
                    errors.Add("EL must be a valid number.");
                    invalidControls.Add(txtaftercoolingEL);
                }
                if (txtaftercoolinglength.Text.Trim() != "-" && (string.IsNullOrEmpty(txtaftercoolinglength.Text) || !double.TryParse(txtaftercoolinglength.Text, out _)))
                {
                    errors.Add("Length must be a valid number.");
                    invalidControls.Add(txtaftercoolinglength);
                }
            }

            // Check if Grid has data (only required if grid is empty)
            if (!gridHasData)
            {
                //Validate Test Details Input
                if (ddlitemtype.SelectedIndex == 0 || ddlitemtype.SelectedValue == "Select")
                {
                    errors.Add("Item Type is required.");
                    invalidControls.Add(ddlitemtype);
                }
                if (ddlCorePhase.SelectedIndex == -1)
                {
                    errors.Add("Core Phase is required.");
                    invalidControls.Add(ddlCorePhase);
                }
                if (ddlSampleType.SelectedIndex == 0 || ddlSampleType.SelectedValue == "Select")
                {
                    errors.Add("Sample Type is required.");
                    invalidControls.Add(ddlSampleType);
                }
                if (ddlTestCategory.SelectedIndex == 0 || ddlTestCategory.SelectedValue == "Select")
                {
                    errors.Add("Test Category is required.");
                    invalidControls.Add(ddlTestCategory);
                }
                if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text))
                {
                    errors.Add("Specimen No. is required.");
                    invalidControls.Add(txtSpecimenNo);
                }
                if (string.IsNullOrEmpty(txtInput1.Text))
                {
                    errors.Add("Input 1  is required.");
                    invalidControls.Add(txtInput1);
                }
                if (string.IsNullOrEmpty(txtInput2.Text))
                {
                    errors.Add("Input 2  is required.");
                    invalidControls.Add(txtInput2);
                }
                if (string.IsNullOrEmpty(txtInput3.Text))
                {
                    errors.Add("Input 3  is required.");
                    invalidControls.Add(txtInput3);
                }
                if (string.IsNullOrEmpty(txtInput4.Text))
                {
                    errors.Add("Input 4  is required.");
                    invalidControls.Add(txtInput4);
                }
                if (string.IsNullOrEmpty(txtInput5.Text))
                {
                    errors.Add("Input 5  is required.");
                    invalidControls.Add(txtInput5);
                }
                if (string.IsNullOrEmpty(txtInput6.Text))
                {
                    errors.Add("Input 6 is required.");
                    invalidControls.Add(txtInput6);
                }
                if (string.IsNullOrEmpty(txtMin.Text))
                {
                    errors.Add("Min  is required.");
                    invalidControls.Add(txtMin);
                }
                if (string.IsNullOrEmpty(txtSpecimenNo.Text))
                {
                    errors.Add("Specimen No. is required.");
                    invalidControls.Add(txtSpecimenNo);
                }
            }

            // Reset all control borders first
            ResetControlBorders();

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

        private bool Submitform()
        {
            try
            {
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return false;
                }

                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Validate for duplicate ItemCategory, CorePhase, and SpecimenNo
                string selectedCategory = ddlTestCategory.SelectedItem.Text;
                string selectedCorePhase = ddlCorePhase.SelectedValue;
               // int specimenNo = Convert.ToInt32(txtSpecimenNo.Text.Trim());

                //  Thickness Measurements (Formatted)
                List<string> thicknessSpecimens = new List<string>();
                var thicknessRows = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString().Contains("Thickness") && !r["ItemCategory"].ToString().Contains("Min"))
                    .ToList();

                var thicknessGroups = thicknessRows.GroupBy(r => r["CorePhase"].ToString());
                foreach (var group in thicknessGroups)
                {
                    string core = group.Key;
                    int specimenNum = 1;
                    foreach (var row in group)
                    {
                        string inputs = string.Format("{0},{1},{2},{3},{4},{5}",
                            row["Input1"], row["Input2"], row["Input3"],
                            row["Input4"], row["Input5"], row["Input6"]);
                        thicknessSpecimens.Add($"{core};{specimenNum}:{inputs}");
                        specimenNum++;
                    }
                }

                if (thicknessSpecimens.Any())
                {
                    reportParamMapList.Add(" 10244"); // Thickness
                    resultList.Add(string.Join("|", thicknessSpecimens));
                }


                //  Diameter Measurements (Formatted)
                List<string> WidthSpecimens = new List<string>();
                var WidthRows = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString().Contains("Width") && !r["ItemCategory"].ToString().Contains("Min"))
                    .ToList();

                var WidthGroups = WidthRows.GroupBy(r => r["CorePhase"].ToString());
                foreach (var group in WidthGroups)
                {
                    string core = group.Key;
                    int specimenNum = 1;
                    foreach (var row in group)
                    {
                        string inputs = string.Format("{0},{1},{2},{3},{4},{5}",
                            row["Input1"], row["Input2"], row["Input3"],
                            row["Input4"], row["Input5"], row["Input6"]);
                        WidthSpecimens.Add($"{core};{specimenNum}:{inputs}");
                        specimenNum++;
                    }
                }

                if (WidthSpecimens.Any())
                {
                    reportParamMapList.Add("10246"); // 
                    resultList.Add(string.Join("|", WidthSpecimens));
                }

                //  Thickness Averages (FIXED to use InputAvg instead of Avg)
                List<string> thicknessMinValues = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString() == "Thickness")
                    .Select(r => r["InputMin"] != DBNull.Value ? Convert.ToDecimal(r["InputMin"]).ToString("0.##") : "")
                    .Where(val => !string.IsNullOrWhiteSpace(val)) // Filter out empty or null values
                    .ToList();

                if (thicknessMinValues.Any())
                {
                    reportParamMapList.Add("10245"); // ThicknessAvg
                    resultList.Add(string.Join("|", thicknessMinValues));
                }

                //  Diameter Averages (FIXED to use InputAvg instead of Avg)
                List<string> WidthMinValues = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString() == "Width")
                    .Select(r => r["InputMin"] != DBNull.Value ? Convert.ToDecimal(r["InputMin"]).ToString("0.##") : "")
                    .Where(val => !string.IsNullOrWhiteSpace(val)) // Filter out empty or null values
                    .ToList();

                if (WidthMinValues.Any())
                {
                    reportParamMapList.Add("10247"); // DiameterAvg
                    resultList.Add(string.Join("|", WidthMinValues));
                }
                //  Weight Load Data (Formatted)
                // Prepare lists to collect values for each parameter across specimens




                //  Weight Load Data (Formatted like Thickness)
                List<string> areaSpecimens = new List<string>();
                List<string> weightloadSpecimens = new List<string>();
                List<string> initialLengthSpecimens = new List<string>();
                List<string> after15MinLengthSpecimens = new List<string>();
                List<string> after15MinELSpecimens = new List<string>();
                List<string> after15MinMedianSpecimens = new List<string>();
                List<string> afterCoolingLengthSpecimens = new List<string>();
                List<string> afterCoolingELSpecimens = new List<string>();
                List<string> afterCoolingMedianSpecimens = new List<string>();

                // Get DataTable from ViewState
                DataTable dtWeightloadData = ViewState["WeightloadData"] as DataTable;
                if (dtWeightloadData != null && dtWeightloadData.Rows.Count > 0)
                {
                    // Get unique CorePhase values from main DataTable or use a default grouping
                    var corePhases = dt.AsEnumerable()
                        .Select(r => r["CorePhase"].ToString())
                        .Distinct()
                        .ToList();

                    // If no CorePhase available, create default grouping
                    if (!corePhases.Any())
                    {
                        corePhases.Add("Core1"); // Default core phase
                    }

                    foreach (string corePhase in corePhases)
                    {
                        int specimenNum = 1;
                        foreach (DataRow row in dtWeightloadData.Rows)
                        {
                            // Area
                            string areaValue = row["Area"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(areaValue))
                            {
                                areaSpecimens.Add($"{corePhase};{specimenNum}:{areaValue}");
                            }

                            // Weightload
                            string weightloadValue = row["Weightload"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(weightloadValue))
                            {
                                weightloadSpecimens.Add($"{corePhase};{specimenNum}:{weightloadValue}");
                            }

                            // Initial Length
                            string initialLengthValue = row["Initiallength"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(initialLengthValue))
                            {
                                initialLengthSpecimens.Add($"{corePhase};{specimenNum}:{initialLengthValue}");
                            }

                            // After 15 Min Length
                            string after15MinLengthValue = row["After15MinLength"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(after15MinLengthValue))
                            {
                                after15MinLengthSpecimens.Add($"{corePhase};{specimenNum}:{after15MinLengthValue}");
                            }

                            // After 15 Min EL
                            string after15MinELValue = row["After15MinEL"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(after15MinELValue))
                            {
                                after15MinELSpecimens.Add($"{corePhase};{specimenNum}:{after15MinELValue}");
                            }

                            // After 15 Min Median
                            string after15MinMedianValue = row["After15MinMedian"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(after15MinMedianValue))
                            {
                                after15MinMedianSpecimens.Add($"{corePhase};{specimenNum}:{after15MinMedianValue}");
                            }

                            // After Cooling Length
                            string afterCoolingLengthValue = row["AftercoolingLength"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(afterCoolingLengthValue))
                            {
                                afterCoolingLengthSpecimens.Add($"{corePhase};{specimenNum}:{afterCoolingLengthValue}");
                            }

                            // After Cooling EL
                            string afterCoolingELValue = row["AftercoolingEL"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(afterCoolingELValue))
                            {
                                afterCoolingELSpecimens.Add($"{corePhase};{specimenNum}:{afterCoolingELValue}");
                            }

                            // After Cooling Median
                            string afterCoolingMedianValue = row["AftercoolingMedian"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(afterCoolingMedianValue))
                            {
                                afterCoolingMedianSpecimens.Add($"{corePhase};{specimenNum}:{afterCoolingMedianValue}");
                            }

                            specimenNum++;
                        }
                    }

                    // Add all parameters to reportParamMapList and resultList
                    if (areaSpecimens.Any())
                    {
                        reportParamMapList.Add("10248"); // Area
                        resultList.Add(string.Join("|", areaSpecimens));
                    }

                    if (weightloadSpecimens.Any())
                    {
                        reportParamMapList.Add("10249"); // Weightload
                        resultList.Add(string.Join("|", weightloadSpecimens));
                    }

                    if (initialLengthSpecimens.Any())
                    {
                        reportParamMapList.Add("10250"); // InitialLength
                        resultList.Add(string.Join("|", initialLengthSpecimens));
                    }

                    if (after15MinLengthSpecimens.Any())
                    {
                        reportParamMapList.Add("10251"); // After15MinLength
                        resultList.Add(string.Join("|", after15MinLengthSpecimens));
                    }

                    if (after15MinELSpecimens.Any())
                    {
                        reportParamMapList.Add("10252"); // After15MinEL
                        resultList.Add(string.Join("|", after15MinELSpecimens));
                    }

                    if (after15MinMedianSpecimens.Any())
                    {
                        reportParamMapList.Add("10253"); // After15MinMedian
                        resultList.Add(string.Join("|", after15MinMedianSpecimens));
                    }

                    if (afterCoolingLengthSpecimens.Any())
                    {
                        reportParamMapList.Add("10254"); // AftercoolingLength
                        resultList.Add(string.Join("|", afterCoolingLengthSpecimens));
                    }

                    if (afterCoolingELSpecimens.Any())
                    {
                        reportParamMapList.Add("10255"); // AftercoolingEL
                        resultList.Add(string.Join("|", afterCoolingELSpecimens));
                    }

                    if (afterCoolingMedianSpecimens.Any())
                    {
                        reportParamMapList.Add("10256"); // AftercoolingMedian
                        resultList.Add(string.Join("|", afterCoolingMedianSpecimens));
                    }
                }
                // Add Standard values
                reportParamMapList.Add("10257"); resultList.Add(txtSpecificThickness.Text); // Standard temperature
                reportParamMapList.Add("10258"); resultList.Add(txtSpecificWidth.Text); // Standard duration
                reportParamMapList.Add("10259"); resultList.Add(txtSpecificArea.Text); // Standard before heating
                reportParamMapList.Add("10260"); resultList.Add(txtSpecificWeightload.Text); // Standard before heating

                reportParamMapList.Add("10261"); resultList.Add(txtSpecificInitialLength.Text); // Standard before heating
                reportParamMapList.Add("10262"); resultList.Add(txtSpecificAfter15MinLength.Text); // Standard after heating
                reportParamMapList.Add("10263"); resultList.Add(txtSpecificAfter15MinEL.Text); // Standard result

                reportParamMapList.Add("10264"); resultList.Add(txtSpecificAfter15MinMedian.Text); // Standard result
                reportParamMapList.Add("10265"); resultList.Add(txtSpecificAftercoolingLength.Text); // Standard result
                reportParamMapList.Add("10266"); resultList.Add(txtSpecificAftercoolingEL.Text); // Standard result
                reportParamMapList.Add("10267"); resultList.Add(txtSpecificAftercoolingMedian.Text); // Standard result




                // Upper section
                reportParamMapList.Add("10279"); resultList.Add(cableType.Text); // Cable Type               
                reportParamMapList.Add("10268"); resultList.Add(cableSize.Text); // Cable Size
                reportParamMapList.Add("10269"); resultList.Add(voltage.Text); // Voltage   
                reportParamMapList.Add("10270"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("10271"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("10272"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("10273"); resultList.Add(refSpec.Text); // Reference Specification
                reportParamMapList.Add("10274"); resultList.Add(testMethodNo.Text); // Test Method No            
                reportParamMapList.Add("10275"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("10276"); resultList.Add(worksheetno.Text); // Worksheet No          
                reportParamMapList.Add("10277"); resultList.Add(roomhumidity.Text); // Room Humidity                           
                reportParamMapList.Add("10278"); resultList.Add(equipmentno.Text); // Equipment No


                // Final save
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // Success message and redirect
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results and Report Generated successfully!'); window.location='GenerateTestReport_Hotset.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID + "';", true);

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
            // Helper function to validate inputs allowing "-" as zero
            bool IsValidNumberOrDash(string input)
            {
                if (input.Trim() == "-") return true;
                return double.TryParse(input, out _);
            }

            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            bool gridHasData = (gvHotsetTest.Rows.Count > 0);
            bool gridWeightLoadHasData = (gvWeightloadData.Rows.Count > 0);

            // Validate Main Section (required regardless of grid data)
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

            // roomhumidity allow "-" as valid
            if (string.IsNullOrEmpty(roomhumidity.Text) || !IsValidNumberOrDash(roomhumidity.Text))
            {
                errors.Add("Room Humidity must be a valid number or '-'.");
                invalidControls.Add(roomhumidity);
            }
            // roomtemp allow "-" as valid
            if (string.IsNullOrEmpty(roomtemp.Text) || !IsValidNumberOrDash(roomtemp.Text))
            {
                errors.Add("Room Temp. must be a valid number or '-'.");
                invalidControls.Add(roomtemp);
            }

            if (!gridWeightLoadHasData)
            {
                // CorePhase dropdown must be selected (no "-")
                if (ddlCorePhase.SelectedIndex == -1)
                {
                    errors.Add("Core Phase is required.");
                    invalidControls.Add(ddlCorePhase);
                }
                // SpecimenNo cannot be "-" or empty
                if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || txtSpecimenNo.Text.Trim() == "-")
                {
                    errors.Add("Specimen No. is required and cannot be '-'.");
                    invalidControls.Add(txtSpecimenNo);
                }
                // Weightload allow "-" or valid number
                if (string.IsNullOrEmpty(txtWeightload.Text) || !IsValidNumberOrDash(txtWeightload.Text))
                {
                    errors.Add("Weightload must be a valid number or '-'.");
                    invalidControls.Add(txtWeightload);
                }
                // Area allow "-" or valid number
                if (string.IsNullOrEmpty(txtarea.Text) || !IsValidNumberOrDash(txtarea.Text))
                {
                    errors.Add("Area must be a valid number or '-'.");
                    invalidControls.Add(txtarea);
                }
                // Initiallength allow "-" or valid number
                if (string.IsNullOrEmpty(txtInitiallength.Text) || !IsValidNumberOrDash(txtInitiallength.Text))
                {
                    errors.Add("Initial length must be a valid number or '-'.");
                    invalidControls.Add(txtInitiallength);
                }
                // After15Minlength allow "-" or valid number
                if (string.IsNullOrEmpty(txtAfter15Minlength.Text) || !IsValidNumberOrDash(txtAfter15Minlength.Text))
                {
                    errors.Add("Length must be a valid number or '-'.");
                    invalidControls.Add(txtAfter15Minlength);
                }
                // After15MinEL allow "-" or valid number
                if (string.IsNullOrEmpty(txtAfter15MinEL.Text) || !IsValidNumberOrDash(txtAfter15MinEL.Text))
                {
                    errors.Add("EL must be a valid number or '-'.");
                    invalidControls.Add(txtAfter15MinEL);
                }
                // aftercoolingEL allow "-" or valid number
                if (string.IsNullOrEmpty(txtaftercoolingEL.Text) || !IsValidNumberOrDash(txtaftercoolingEL.Text))
                {
                    errors.Add("EL must be a valid number or '-'.");
                    invalidControls.Add(txtaftercoolingEL);
                }
                // aftercoolinglength allow "-" or valid number
                if (string.IsNullOrEmpty(txtaftercoolinglength.Text) || !IsValidNumberOrDash(txtaftercoolinglength.Text))
                {
                    errors.Add("Length must be a valid number or '-'.");
                    invalidControls.Add(txtaftercoolinglength);
                }
            }

            if (!gridHasData)
            {
                // Item Type dropdown must be selected (no "-")
                if (ddlitemtype.SelectedIndex == 0 || ddlitemtype.SelectedValue == "Select")
                {
                    errors.Add("Item Type is required.");
                    invalidControls.Add(ddlitemtype);
                }
                // Core Phase dropdown must be selected (no "-")
                if (ddlCorePhase.SelectedIndex == -1)
                {
                    errors.Add("Core Phase is required.");
                    invalidControls.Add(ddlCorePhase);
                }
                // Sample Type dropdown must be selected (no "-")
                if (ddlSampleType.SelectedIndex == 0 || ddlSampleType.SelectedValue == "Select")
                {
                    errors.Add("Sample Type is required.");
                    invalidControls.Add(ddlSampleType);
                }
                // Test Category dropdown must be selected (no "-")
                if (ddlTestCategory.SelectedIndex == 0 || ddlTestCategory.SelectedValue == "Select")
                {
                    errors.Add("Test Category is required.");
                    invalidControls.Add(ddlTestCategory);
                }
                // SpecimenNo cannot be "-" or empty
                if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || txtSpecimenNo.Text.Trim() == "-")
                {
                    errors.Add("Specimen No. is required and cannot be '-'.");
                    invalidControls.Add(txtSpecimenNo);
                }
                // Validate Input fields as required (assuming they must not be empty or "-")
                if (string.IsNullOrEmpty(txtInput1.Text))
                {
                    errors.Add("Input 1 is required.");
                    invalidControls.Add(txtInput1);
                }
                if (string.IsNullOrEmpty(txtInput2.Text))
                {
                    errors.Add("Input 2 is required.");
                    invalidControls.Add(txtInput2);
                }
                if (string.IsNullOrEmpty(txtInput3.Text))
                {
                    errors.Add("Input 3 is required.");
                    invalidControls.Add(txtInput3);
                }
                if (string.IsNullOrEmpty(txtInput4.Text))
                {
                    errors.Add("Input 4 is required.");
                    invalidControls.Add(txtInput4);
                }
                if (string.IsNullOrEmpty(txtInput5.Text))
                {
                    errors.Add("Input 5 is required.");
                    invalidControls.Add(txtInput5);
                }
                if (string.IsNullOrEmpty(txtInput6.Text))
                {
                    errors.Add("Input 6 is required.");
                    invalidControls.Add(txtInput6);
                }
                if (string.IsNullOrEmpty(txtMin.Text))
                {
                    errors.Add("Min is required.");
                    invalidControls.Add(txtMin);
                }
            }

            if (!gridHasData && !gridWeightLoadHasData)
            {
                errors.Add("Add at least one test result to either grid.");
            }

            // Validate Standard Values section (allow "-" or valid number)
            if (string.IsNullOrEmpty(txtSpecificThickness.Text) || !IsValidNumberOrDash(txtSpecificThickness.Text))
            {
                errors.Add("Standard Thickness must be a valid number or '-'.");
                invalidControls.Add(txtSpecificThickness);
            }
            if (string.IsNullOrEmpty(txtSpecificWidth.Text) || !IsValidNumberOrDash(txtSpecificWidth.Text))
            {
                errors.Add("Standard Width must be a valid number or '-'.");
                invalidControls.Add(txtSpecificWidth);
            }
            if (string.IsNullOrEmpty(txtSpecificArea.Text) || !IsValidNumberOrDash(txtSpecificArea.Text))
            {
                errors.Add("Standard Area value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificArea);
            }
            if (string.IsNullOrEmpty(txtSpecificInitialLength.Text) || !IsValidNumberOrDash(txtSpecificInitialLength.Text))
            {
                errors.Add("Standard Initial Length value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificInitialLength);
            }
            if (string.IsNullOrEmpty(txtSpecificWeightload.Text) || !IsValidNumberOrDash(txtSpecificWeightload.Text))
            {
                errors.Add("Standard Weightload value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificWeightload);
            }
            if (string.IsNullOrEmpty(txtSpecificAfter15MinLength.Text) || !IsValidNumberOrDash(txtSpecificAfter15MinLength.Text))
            {
                errors.Add("Standard length value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificAfter15MinLength);
            }
            if (string.IsNullOrEmpty(txtSpecificAfter15MinEL.Text) || !IsValidNumberOrDash(txtSpecificAfter15MinEL.Text))
            {
                errors.Add("Standard EL value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificAfter15MinEL);
            }
            if (string.IsNullOrEmpty(txtSpecificAfter15MinMedian.Text) || !IsValidNumberOrDash(txtSpecificAfter15MinMedian.Text))
            {
                errors.Add("Standard Median value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificAfter15MinMedian);
            }
            if (string.IsNullOrEmpty(txtSpecificAftercoolingEL.Text) || !IsValidNumberOrDash(txtSpecificAftercoolingEL.Text))
            {
                errors.Add("Standard EL value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificAftercoolingEL);
            }
            if (string.IsNullOrEmpty(txtSpecificAftercoolingLength.Text) || !IsValidNumberOrDash(txtSpecificAftercoolingLength.Text))
            {
                errors.Add("Standard Length value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificAftercoolingLength);
            }
            if (string.IsNullOrEmpty(txtSpecificAftercoolingMedian.Text) || !IsValidNumberOrDash(txtSpecificAftercoolingMedian.Text))
            {
                errors.Add("Standard Median value must be a valid number or '-'.");
                invalidControls.Add(txtSpecificAftercoolingMedian);
            }

            // If no data in grid, add error
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            // Reset all control borders first
            ResetControlBorders();

            if (errors.Count > 0)
            {
                foreach (var control in invalidControls)
                {
                    if (control is TextBox tb)
                    {
                        tb.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        tb.BorderStyle = BorderStyle.Solid;
                        tb.BorderWidth = 1;
                    }
                    // You can add dropdown highlight logic here if desired
                }
                string errorMessage = "Validation Errors:\\n" + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);

            }
            else
            {
                try
                {
                    if (Request.QueryString["s"] != null)
                    {
                        int sampleId = Convert.ToInt32(Request.QueryString["s"]);

                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;

                        DeleteExistingRecords(sampleId);

                        bool result = Submitform();
                        if (!result)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                                "alert('Update failed. Please check the input data.');", true);
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
        }
        private void DeleteExistingRecords(int sampleId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
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
        private void ClearInputFields()
        {
            txtWeightload.Text = "";
            txtarea.Text = "";
            txtInitiallength.Text = "";
            txtAfter15Minlength.Text = "";
            txtAfter15MinEL.Text = "";            
            txtaftercoolinglength.Text = "";
            txtaftercoolingEL.Text = "";           
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
            txtInput6.Text = "";
            txtWeightload.Text = "";
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
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
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
            txtInput6.Text = "";
            txtMin.Text = "";
            txtWeightload.Text = " ";
            txtarea.Text = "";
            txtInitiallength.Text = "";
            txtAfter15Minlength.Text = "";
            txtAfter15MinEL.Text = "";
            txtaftercoolinglength.Text = "";
            txtaftercoolingEL.Text = "";           
            // Clear GridView
            gvHotsetTest.DataSource = null;
            gvHotsetTest.DataBind();
            // Standard Values
            txtSpecificThickness.Text = "";
            txtSpecificWeightload.Text = "";
            txtSpecificArea.Text = "";
            txtSpecificInitialLength.Text = "";
            txtSpecificWidth.Text = "";
            txtSpecificAfter15MinLength.Text = "";
            txtSpecificAfter15MinEL.Text = "";
            txtSpecificAfter15MinMedian.Text = "";
            txtSpecificAftercoolingLength.Text = "";
            txtSpecificAftercoolingEL.Text = "";
            txtSpecificAftercoolingMedian.Text = "";
        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();


            //  allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);

            allTextBoxes.Add(txtMin);
   
            allTextBoxes.Add(txtWeightload);
            allTextBoxes.Add(txtarea);
            allTextBoxes.Add(txtInitiallength);
            allTextBoxes.Add(txtAfter15Minlength);
            allTextBoxes.Add(txtAfter15MinEL);

            allTextBoxes.Add(txtaftercoolinglength);
            allTextBoxes.Add(txtaftercoolingEL);
        
      
            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        protected void Input_TextChanged(object sender, EventArgs e)
        {
            // Get values from all 6 input fields
            string[] inputs = new string[] {
        txtInput1.Text.Trim(),
        txtInput2.Text.Trim(),
        txtInput3.Text.Trim(),
        txtInput4.Text.Trim(),
        txtInput5.Text.Trim(),
        txtInput6.Text.Trim()
    };

            // Initialize a list to hold valid decimal values
            List<decimal> validValues = new List<decimal>();

            foreach (string input in inputs)
            {
                // If the value is "-" replace it with 0
                if (input == "-")
                {
                    validValues.Add(0);  // Add 0 instead of "-"
                    continue;
                }

                // Try to parse the decimal
                decimal value;
                if (decimal.TryParse(input, out value))
                {
                    validValues.Add(value);
                }
            }

            // If we have valid values, calculate the minimum
            if (validValues.Count > 0)
            {
                decimal minValue = validValues.Min();  // Get the minimum value
                txtMin.Text = minValue.ToString();     // Display in txtMin field
            }
            else
            {
                txtMin.Text = "";  // Clear if no valid values
            }
        }
        protected void gvHotsetTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                    return;

                // Get current row index
                int rowIndex = e.Row.DataItemIndex;

                if (rowIndex > 0)
                {
                    try
                    {
                        // --- ItemType (Column 0) ---
                        string currentItemType = dt.Rows[rowIndex]["ItemType"].ToString();
                        string prevItemType = dt.Rows[rowIndex - 1]["ItemType"].ToString();

                        if (currentItemType == prevItemType)
                        {
                            // Hide current cell
                            e.Row.Cells[0].Visible = false;

                            // Find the previous visible cell in this column to increase its rowspan
                            int prevVisibleRowIndex = rowIndex - 1;
                            GridViewRow prevVisibleRow = null;

                            while (prevVisibleRowIndex >= 0)
                            {
                                GridViewRow checkRow = gvHotsetTest.Rows[prevVisibleRowIndex];
                                if (checkRow.Cells[0].Visible)
                                {
                                    prevVisibleRow = checkRow;
                                    break;
                                }
                                prevVisibleRowIndex--;
                            }

                            if (prevVisibleRow != null)
                            {
                                prevVisibleRow.Cells[0].RowSpan =
                                    (prevVisibleRow.Cells[0].RowSpan == 0) ? 2 : prevVisibleRow.Cells[0].RowSpan + 1;
                            }
                        }

                        // --- ItemCategory (Column 1) ---
                        // Only merge if ItemType also matches
                        string currentItemCategory = dt.Rows[rowIndex]["ItemCategory"].ToString();
                        string prevItemCategory = dt.Rows[rowIndex - 1]["ItemCategory"].ToString();

                        if (currentItemType == prevItemType && currentItemCategory == prevItemCategory)
                        {
                            // Hide current cell
                            e.Row.Cells[1].Visible = false;

                            // Find the previous visible cell in this column to increase its rowspan
                            int prevVisibleRowIndex = rowIndex - 1;
                            GridViewRow prevVisibleRow = null;

                            while (prevVisibleRowIndex >= 0)
                            {
                                GridViewRow checkRow = gvHotsetTest.Rows[prevVisibleRowIndex];
                                if (checkRow.Cells[1].Visible)
                                {
                                    prevVisibleRow = checkRow;
                                    break;
                                }
                                prevVisibleRowIndex--;
                            }

                            if (prevVisibleRow != null)
                            {
                                prevVisibleRow.Cells[1].RowSpan =
                                    (prevVisibleRow.Cells[1].RowSpan == 0) ? 2 : prevVisibleRow.Cells[1].RowSpan + 1;
                            }
                        }

                        // --- CorePhase (Column 2) ---
                        // Only merge if ItemType and ItemCategory also match
                        string currentCorePhase = dt.Rows[rowIndex]["CorePhase"].ToString();
                        string prevCorePhase = dt.Rows[rowIndex - 1]["CorePhase"].ToString();

                        if (currentItemType == prevItemType && currentItemCategory == prevItemCategory &&
                            currentCorePhase == prevCorePhase)
                        {
                            // Hide current cell
                            e.Row.Cells[2].Visible = false;

                            // Find the previous visible cell in this column to increase its rowspan
                            int prevVisibleRowIndex = rowIndex - 1;
                            GridViewRow prevVisibleRow = null;

                            while (prevVisibleRowIndex >= 0)
                            {
                                GridViewRow checkRow = gvHotsetTest.Rows[prevVisibleRowIndex];
                                if (checkRow.Cells[2].Visible)
                                {
                                    prevVisibleRow = checkRow;
                                    break;
                                }
                                prevVisibleRowIndex--;
                            }

                            if (prevVisibleRow != null)
                            {
                                prevVisibleRow.Cells[2].RowSpan =
                                    (prevVisibleRow.Cells[2].RowSpan == 0) ? 2 : prevVisibleRow.Cells[2].RowSpan + 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error in RowDataBound: " + ex.Message);
                    }
                }
            }
        }
        // Helper function to merge columns
        private void ProcessColumn(GridViewRow currentRow, int currentRowIndex, int columnIndex)
        {
            try
            {
                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null || columnIndex >= dt.Columns.Count)
                    return;

                // Get current and previous values for comparison
                string currentValue = dt.Rows[currentRowIndex][columnIndex]?.ToString();
                string previousValue = dt.Rows[currentRowIndex - 1][columnIndex]?.ToString();

                // If values are the same, hide the current cell and merge by adjusting RowSpan of the previous cell
                if (currentValue == previousValue)
                {
                    currentRow.Cells[columnIndex].Visible = false;

                    // Find the previous row with a visible cell in the same column to adjust RowSpan
                    int spanRowIndex = currentRowIndex - 1;
                    while (spanRowIndex >= 0)
                    {
                        TableCell prevCell = gvHotsetTest.Rows[spanRowIndex].Cells[columnIndex];
                        if (prevCell.Visible)
                        {
                            if (prevCell.RowSpan == 0 || prevCell.RowSpan == 1)
                                prevCell.RowSpan = 2;
                            else
                                prevCell.RowSpan += 1;
                            break;
                        }
                        spanRowIndex--;
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in ProcessColumn: " + ex.Message);
            }
        }

        protected void gvHotsetTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                // Delete the row from the DataTable
                if (dt != null && index >= 0 && index < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(index);

                    // Update ViewState and rebind the grid
                    ViewState["TestResults"] = dt;
                    gvHotsetTest.DataSource = dt;
                    gvHotsetTest.DataBind();
                   
                    // Show confirmation message
                    ShowMessage("Record deleted successfully.");
                }
            }
        }      
        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{safeMessage}');", true);
        }
    }
=======
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
            SetTextBoxText(cableType, "10279");
            SetTextBoxText(cableSize, "10268");
            SetTextBoxText(voltage, "10269");
            SetTextBoxText(Date, "10270");
            SetTextBoxText(roomno, "10271");
            SetTextBoxText(roomtemp, "10272");
            SetTextBoxText(refSpec, "10273");
            SetTextBoxText(testMethodNo, "10274");
            SetTextBoxText(jobOrderNo, "10275");
            SetTextBoxText(worksheetno, "10276");
            SetTextBoxText(roomhumidity, "10277");
            SetTextBoxText(equipmentno, "10278");


            SetTextBoxText(txtSpecificThickness, "10257");
            SetTextBoxText(txtSpecificDiameter, "10258");
            SetTextBoxText(txtSpecificArea, "10259");
            SetTextBoxText(txtSpecificWeightload, "10260");
            SetTextBoxText(txtSpecificInitiallength, "10261");
            SetTextBoxText(txtSpecificLength, "10262");
            SetTextBoxText(txtSpecificEL, "10262");
            SetTextBoxText(txtSpecificMedian, "10264");
            SetTextBoxText(txtSpecificoolingLength, "10265");
            SetTextBoxText(txtSpecificoolingEL, "10266");
            SetTextBoxText(txtSpecificoolingMedian, "10267");

            SetTextBoxText(txtaftertemp, "17685");
            SetTextBoxText(txtunderloadatTemp, "17686");


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

                var Material = GetParameterValue(existingData, "17684");
                var coreData = GetParameterValue(existingData, "11245");      // Core
                var sampleData = GetParameterValue(existingData, "17683");    // Sample
                var specimenData = GetParameterValue(existingData, "17687");  // SpecimenNo
                var thicknessData = GetParameterValue(existingData, "17688"); // Thickness
                var thicknessAvgData = GetParameterValue(existingData, "17689"); // ThicknessAvg
                var widthData = GetParameterValue(existingData, "17690");     // 
                var widthAvgData = GetParameterValue(existingData, "17691");  // 
                var weightload = GetParameterValue(existingData, "10249");        // 
                var areaData = GetParameterValue(existingData, "10248");      // 
                var Initiallength = GetParameterValue(existingData, "10250");        // 
                var After15Length = GetParameterValue(existingData, "10251");        // 
                var After15EL = GetParameterValue(existingData, "10252"); // 
                var After15Median = GetParameterValue(existingData, "10253"); // 
                var AftercoolingLength = GetParameterValue(existingData, "10254"); // 
                var AftercoolingEL = GetParameterValue(existingData, "10255");  // MedinTS
                var AftercoolingMedian = GetParameterValue(existingData, "10256");  // 


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

            // Remove parentheses if present
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
            dr["WidthAvgMin"] = txtdiameterInputAvg.Text.Trim(); // ADD THIS LINE
            dr["Area"] = txtArea.Text.Trim();
            dr["Weightload"] = txtWeightload.Text.Trim();
            dr["Initiallength"] = txtInitiallength.Text.Trim();
            dr["Length"] = txtlengthafter.Text.Trim();
            dr["EL"] = txtELafter.Text.Trim();
            dr["Median"] = ""; // median is calculated below
            dr["coolingLength"] = txtaftercoolinglength.Text.Trim(); // MAKE SURE THIS TEXTBOX EXISTS
            dr["coolingEL"] = txtaftercoolingEL.Text.Trim();
            dr["coolingMedian"] = ""; // cooling median calculated below

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



            ViewState["TestResultsData"] = null;

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

            ViewState["TestResultsData"] = null;

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
                reportParamMapList.Add("10279"); resultList.Add(cableType.Text);
                reportParamMapList.Add("10268"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("10269"); resultList.Add(voltage.Text);
                reportParamMapList.Add("10270"); resultList.Add(Date.Text);
                reportParamMapList.Add("10271"); resultList.Add(roomno.Text);
                reportParamMapList.Add("10272"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("10273"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("10274"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("10275"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("10276"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("10277"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("10278"); resultList.Add(equipmentno.Text);

                // Add grid data
                reportParamMapList.Add("17684"); resultList.Add(string.Join("|", materialGroupList));
                reportParamMapList.Add("11245"); resultList.Add(string.Join("|", coreGroups));
                reportParamMapList.Add("17683"); resultList.Add(string.Join("|", sampleGroups));
                reportParamMapList.Add("17687"); resultList.Add(string.Join("|", specimenNoGroups));
                reportParamMapList.Add("17688"); resultList.Add(string.Join("|", thicknessGroups));
                reportParamMapList.Add("17689"); resultList.Add(string.Join("|", thicknessAvgGroups));
                reportParamMapList.Add("17690"); resultList.Add(string.Join("|", widthGroups));
                reportParamMapList.Add("17691"); resultList.Add(string.Join("|", widthAvgGroups));
                reportParamMapList.Add("10248"); resultList.Add(string.Join("|", areaGroups));

                reportParamMapList.Add("10249"); resultList.Add(string.Join("|", weightGroups));
                reportParamMapList.Add("10250"); resultList.Add(string.Join("|", initialLengthGroups));
                reportParamMapList.Add("10251"); resultList.Add(string.Join("|", lengthGroups));
                reportParamMapList.Add("10252"); resultList.Add(string.Join("|", elGroups));
                reportParamMapList.Add("10253"); resultList.Add(string.Join("|", medianGroups));
                reportParamMapList.Add("10254"); resultList.Add(string.Join("|", afterCoolingLengthGroups));
                reportParamMapList.Add("10255"); resultList.Add(string.Join("|", afterCoolingELGroups));
                reportParamMapList.Add("10256"); resultList.Add(string.Join("|", afterCoolingMedianGroups));

                // Specified Values
                reportParamMapList.Add("10257"); resultList.Add(txtSpecificThickness.Text);
                reportParamMapList.Add("10258"); resultList.Add(txtSpecificDiameter.Text);
                reportParamMapList.Add("10259"); resultList.Add(txtSpecificArea.Text);
                reportParamMapList.Add("10260"); resultList.Add(txtSpecificWeightload.Text);
                reportParamMapList.Add("10261"); resultList.Add(txtSpecificInitiallength.Text);
                reportParamMapList.Add("10262"); resultList.Add(txtSpecificLength.Text);
                reportParamMapList.Add("10263"); resultList.Add(txtSpecificEL.Text);
                reportParamMapList.Add("10264"); resultList.Add(txtSpecificMedian.Text);
                reportParamMapList.Add("10265"); resultList.Add(txtSpecificoolingLength.Text);
                reportParamMapList.Add("10266"); resultList.Add(txtSpecificoolingEL.Text);
                reportParamMapList.Add("10267"); resultList.Add(txtSpecificoolingMedian.Text);

                reportParamMapList.Add("17685"); resultList.Add(txtaftertemp.Text);
                reportParamMapList.Add("17686"); resultList.Add(txtunderloadatTemp.Text);

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
>>>>>>> .r267
}