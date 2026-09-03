using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace LIMS
{
    public partial class Gravimetricwaterabsorption : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                // Initialize specimen number
                txtSpecimenNo.Text = "1";
                //upper section
                roomno.Text = "Mechanical & Oven ";
                cableType.Text = "Cu/XLPE/LS/DSTA/PE";
                refSpec.Text = "IEC 60502-2";
                cableSize.Text = "3 x 25 mm²";
                testMethodNo.Text = "IEC 60811-508";
                voltage.Text = "3,6/6(7,2)kV";
                jobOrderNo.Text = "WO/2024120905";
                worksheetno.Text = "-";
                equipmentno.Text = "QE/17/PH/175,QE/12/TH/09";

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();

            }
        }

        // Initialize grid
        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCategory", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(int));
            dt.Columns.Add("ItemCategory", typeof(string));
      

            // Add the 6 additional input columns
            //dt.Columns.Add("Input1", typeof(string));
            dt.Columns.Add("Input2", typeof(string));
            dt.Columns.Add("Input3", typeof(string));
            dt.Columns.Add("Input4", typeof(string));
            dt.Columns.Add("Input5", typeof(string));
            dt.Columns.Add("Input6", typeof(string));
            // ✅ Add the average column here too
            dt.Columns.Add("InputAvg", typeof(decimal));
            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();

        }
       
        // Method to prefill both grids from database
        private void PrefillUpperSection(DataTable existingData)
        { //Upper section

            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "8231");
            SetLabelText(refSpec, "8225");
            SetLabelText(cableSize, "8220");
            SetLabelText(testMethodNo, "8226");
            SetLabelText(voltage, "8221");
            SetLabelText(jobOrderNo, "8227");
            SetLabelText(worksheetno, "8228");
            SetLabelText(equipmentno, "8230");

            // Textbox fields
            SetTextBoxText(roomno, "8234");
            SetTextBoxText(roomhumidity, "8229");
            SetTextBoxText(roomtemp, "8224");
            SetTextBoxText(Date, "8222");

            SetTextBoxText(txtSpecificThickness, "8214");
            SetTextBoxText(txtSpecificWidth, "8215");
            SetTextBoxText(txtSpecificLength, "8216");
            SetTextBoxText(txtSpecifictotalsurfacearea, "8217");
            SetTextBoxText(txtSpecificmgcm2, "8218");
            SetTextBoxText(txtSpecificMean, "8219");


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
                // Initialize grids first
                InitializeGrid();

                // Create DAL object if not exists
                if (_cm == null)
                    _cm = new DAL.CM();

                // Fetch existing test results for this sample
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

                    // Query for thickness/diameter data (Pressure Test grid)
                    string pressureTestQuery = @"
                    WITH ResultPivot AS (
                        SELECT 
                            SampleID,
                            MAX(CASE WHEN ReportParamMapID = '6247' THEN Result END) AS Thickness,
                            MAX(CASE WHEN ReportParamMapID = '6248' THEN Result END) AS ThicknessAvg,
                            MAX(CASE WHEN ReportParamMapID = '6249' THEN Result END) AS Diameter,
                            MAX(CASE WHEN ReportParamMapID = '6250' THEN Result END) AS DiameterAvg
                        FROM TestResultsPTQualis
                        WHERE SampleID = @SampleID
                        GROUP BY SampleID
                    )
                    SELECT 
                        Thickness,
                        ThicknessAvg,
                        Diameter,
                        DiameterAvg
                    FROM ResultPivot";

                    // Query for weightload data (Weightload Data grid)
                    string weightloadQuery = @"
                        WITH ResultPivot AS (
                            SELECT 
                                SampleID,
                                MAX(CASE WHEN ReportParamMapID = '6251' THEN Result END) AS DiameterWeightload,
                                MAX(CASE WHEN ReportParamMapID = '6252' THEN Result END) AS ThicknessAfterLoad,
                                MAX(CASE WHEN ReportParamMapID = '6253' THEN Result END) AS DeformationAfterLoad,
                                MAX(CASE WHEN ReportParamMapID = '6254' THEN Result END) AS DeformationMedian
                            FROM TestResultsPTQualis
                            WHERE SampleID = @SampleID
                            GROUP BY SampleID
                        )
                        SELECT 
                            DiameterWeightload,
                            ThicknessAfterLoad,
                            DeformationAfterLoad,
                            DeformationMedian
                        FROM ResultPivot";

                    // Process pressure test data
                    using (SqlCommand cmd = new SqlCommand(pressureTestQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Get the TestResults table from ViewState
                            DataTable dtPressureTest = ViewState["TestResults"] as DataTable;
                            dtPressureTest.Clear(); // Clear existing rows

                            if (reader.Read())
                            {
                                // Get thickness and diameter data
                                string thicknessStr = reader["Thickness"] != DBNull.Value ? reader["Thickness"].ToString().Trim() : "";
                                string thicknessAvgStr = reader["ThicknessAvg"] != DBNull.Value ? reader["ThicknessAvg"].ToString().Trim() : "";
                                string diameterStr = reader["Diameter"] != DBNull.Value ? reader["Diameter"].ToString().Trim() : "";
                                string diameterAvgStr = reader["DiameterAvg"] != DBNull.Value ? reader["DiameterAvg"].ToString().Trim() : "";

                                // Split comma-separated values for thickness and diameter
                                string[] thicknessValues = SplitAndTrim(thicknessStr);
                                string[] diameterValues = SplitAndTrim(diameterStr);

                                // Get max count of values to determine number of specimens
                                int specimenCount = Math.Max(thicknessValues.Length, diameterValues.Length);
                                if (specimenCount == 0) specimenCount = 1; // Ensure at least one row

                                // Get current test category and core phase
                                string currentCategory = ddlTestCategory.SelectedItem.Text;
                                string currentCorePhase = ddlCorePhase.SelectedValue;

                                // Create a row for each specimen
                                for (int i = 0; i < specimenCount; i++)
                                {
                                    DataRow newRow = dtPressureTest.NewRow();
                                    newRow["ItemCategory"] = currentCategory;
                                    newRow["CorePhase"] = currentCorePhase;
                                    newRow["SpecimenNo"] = i + 1;

                                    // Use thickness values for Input1
                                    newRow["Input1"] = i < thicknessValues.Length ? thicknessValues[i] : "";

                                    // Use diameter values for Input2
                                    newRow["Input2"] = i < diameterValues.Length ? diameterValues[i] : "";

                                    // Empty values for other inputs
                                    newRow["Input3"] = "";
                                    newRow["Input4"] = "";
                                    newRow["Input5"] = "";
                                    newRow["Input6"] = "";

                                    // Use thickness average for InputAvg
                                    newRow["InputAvg"] = thicknessAvgStr;

                                    dtPressureTest.Rows.Add(newRow);
                                }

                                // Update ViewState and bind grid
                                ViewState["TestResults"] = dtPressureTest;
                                gvPressureTest.DataSource = dtPressureTest;
                                gvPressureTest.DataBind();
                            }
                        }
                    }

                    // Process weightload data
                    using (SqlCommand cmd = new SqlCommand(weightloadQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Get the WeightloadData table from ViewState
                            DataTable dtWeightloadData = ViewState["WeightloadData"] as DataTable;
                            dtWeightloadData.Clear(); // Clear existing rows

                            if (reader.Read())
                            {
                                // Get weightload data
                                string weightloadStr = reader["DiameterWeightload"] != DBNull.Value ? reader["DiameterWeightload"].ToString().Trim() : "";
                                string thicknessAfterLoadStr = reader["ThicknessAfterLoad"] != DBNull.Value ? reader["ThicknessAfterLoad"].ToString().Trim() : "";
                                string deformationStr = reader["DeformationAfterLoad"] != DBNull.Value ? reader["DeformationAfterLoad"].ToString().Trim() : "";
                                string medianStr = reader["DeformationMedian"] != DBNull.Value ? reader["DeformationMedian"].ToString().Trim() : "";

                                // Split comma-separated values
                                string[] weightloadValues = SplitAndTrim(weightloadStr);
                                string[] thicknessAfterLoadValues = SplitAndTrim(thicknessAfterLoadStr);
                                string[] deformationValues = SplitAndTrim(deformationStr);

                                // Get max count of values to determine number of specimens
                                int maxCount = Math.Max(
                                    Math.Max(weightloadValues.Length, thicknessAfterLoadValues.Length),
                                    deformationValues.Length
                                );
                                if (maxCount == 0) maxCount = 1; // Ensure at least one row

                                // Get current test category
                                string currentCategory = ddlTestCategory.SelectedItem.Text;

                                // Create a row for each specimen
                                for (int i = 0; i < maxCount; i++)
                                {
                                    DataRow newRow = dtWeightloadData.NewRow();
                                    newRow["ItemCategory"] = currentCategory;
                                    newRow["SpecimenNo"] = i + 1;
                                    newRow["Weightload"] = i < weightloadValues.Length ? weightloadValues[i] : "";
                                    newRow["ThicknessatTemperature"] = i < thicknessAfterLoadValues.Length ? thicknessAfterLoadValues[i] : "";
                                    newRow["Deformation"] = i < deformationValues.Length ? deformationValues[i] : "";
                                    newRow["Median"] = medianStr;

                                    dtWeightloadData.Rows.Add(newRow);
                                }

                                // Update ViewState and bind grid
                                ViewState["WeightloadData"] = dtWeightloadData;
                                gvMassVariation.DataSource = dtWeightloadData;
                                gvMassVariation.DataBind();
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
        private DataTable GetExistingTestResultsForSample(int sampleId, int reportId)
        {
            Dictionary<string, string> parameterResults = new Dictionary<string, string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT ReportParamMapID, Result 
                FROM TestResultsPTQualis 
                WHERE SampleID = @SampleID AND ReportID = @ReportID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string paramId = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();
                                parameterResults[paramId] = result;
                            }
                        }
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("ParameterID", typeof(string));
                dt.Columns.Add("Result", typeof(string));

                foreach (var param in parameterResults)
                {
                    dt.Rows.Add(param.Key, param.Value);
                }

                return dt;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DataFetchError",
                    $"alert('Error fetching existing data: {ex.Message}');", true);
                return null;
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
                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        // Show update button since data exists
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        // Prefill upper section details
                        PrefillUpperSection(existingData);

                        // Prefill standard values
                        //PrefillStandardValues(existingData);

                        // Prefill ageing and conditioning details
                        //PrefillAgeingAndConditioningDetails(existingData);

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
        protected void ResetInputs()
        {
            txttotalsurfacearea.Text = "";
            txtweightM1.Text = "";
            txtweightM2.Text = "";
            txtweightM3.Text = "";

            txttotalsurfacearea.Enabled = true;
            txtweightM1.Enabled = true;
            txtweightM2.Enabled = true;
            txtweightM3.Enabled = true;
        }
       
        protected void ddlTestCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset Specimen Number to 1 on Test Category change
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
        //private void UpdateSpecimenNumber()
        //{
        //    // Get the DataTable from ViewState
        //    DataTable dt = (DataTable)ViewState["TestResults"];

        //    // Calculate next specimen number
        //    int nextSpecimenNo = 1; // Default to 1 if no rows
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        nextSpecimenNo = dt.Rows.Count + 1;
        //    }

        //    // Find your actual control - you need to use the correct ID and search method
        //    // If it's in a form:
        //    TextBox txtSpecimenNo = FindControl("txtSpecimenNo") as TextBox;

        //    // If the control isn't directly accessible via FindControl, try:
        //    // TextBox txtSpecimenNo = this.txtSpecimenNo; // If it's declared as a class member

        //    // Or if it's somewhere else in the page hierarchy:
        //    // TextBox txtSpecimenNo = Page.FindControl("txtSpecimenNo") as TextBox;

        //    // Set the value if found
        //    if (txtSpecimenNo != null)
        //    {
        //        txtSpecimenNo.Text = nextSpecimenNo.ToString();
        //    }

        //}

        //protected void btnAddToGrid_Click(object sender, EventArgs e)
        //{
        //    // Reset previous error styles
        //    ResetControlBorders();

        //    bool hasError = false;
        //    List<Control> invalidControls = new List<Control>();

        //    int specimenNo = 0;

        //    decimal inputAvg = 0;

        //    UpdateSpecimenNumber();

        //    if (ddlTestCategory.SelectedIndex == 0 || ddlTestCategory.SelectedValue == "Select")
        //    {
        //        // Show JavaScript alert if 'Select' is selected
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a valid ItemCategory.');", true);
        //        ddlTestCategory.CssClass += " input-error"; // Highlight the dropdown
        //        hasError = true;
        //    }

        //    // Validate Core Phase
        //    if (ddlCorePhase.SelectedIndex <= -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
        //    {
        //        ddlCorePhase.CssClass += " input-error";
        //        invalidControls.Add(ddlCorePhase);
        //        hasError = true;
        //    }


        //    // Validate Specimen Number
        //    if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out specimenNo))
        //    {
        //        invalidControls.Add(txtSpecimenNo);
        //        hasError = true;
        //    }

        //    // Validate Input1 to Input6
        //    TextBox[] inputTextBoxes = { txtInput1, txtInput2, txtInput3, txtInput4, txtInput5, txtInput6 };
        //    foreach (TextBox txt in inputTextBoxes)
        //    {
        //        if (string.IsNullOrWhiteSpace(txt.Text.Trim()))
        //        {
        //            invalidControls.Add(txt);
        //            hasError = true;
        //        }
        //    }

        //    // Validate Average (optional: make sure it's a valid decimal)
        //    if (string.IsNullOrWhiteSpace(txtAvg.Text) || !decimal.TryParse(txtAvg.Text.Trim(), out inputAvg))
        //    {
        //        invalidControls.Add(txtAvg);
        //        hasError = true;
        //    }


        //    // Highlight invalid controls
        //    foreach (var control in invalidControls)
        //    {
        //        if (control is TextBox txt)
        //        {
        //            txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150); // Dark red
        //            txt.BorderStyle = BorderStyle.Solid;
        //            txt.BorderWidth = 1;
        //        }
        //    }


        //    if (hasError)
        //    {
        //        ShowMessage("Please fill in all required fields correctly.");
        //        return;
        //    }
        //    // Get the DataTable from ViewState
        //    DataTable dt = ViewState["TestResults"] as DataTable;
        //    if (dt == null)
        //    {
        //        // Initialize the DataTable if it doesn't exist
        //        dt = new DataTable();
        //        dt.Columns.Add("CorePhase", typeof(string));
        //        dt.Columns.Add("SpecimenNo", typeof(int));
        //        //dt.Columns.Add("Weightload", typeof(string));
        //        //dt.Columns.Add("ThicknessatTemperature", typeof(string));
        //        //dt.Columns.Add("DeformationTemperature", typeof(string));
        //        //dt.Columns.Add("Median", typeof(string));
        //        dt.Columns.Add("ItemCategory", typeof(string));
        //        // Add the 6 additional inputs
        //        dt.Columns.Add("Input1", typeof(string));
        //        dt.Columns.Add("Input2", typeof(string));
        //        dt.Columns.Add("Input3", typeof(string));
        //        dt.Columns.Add("Input4", typeof(string));
        //        dt.Columns.Add("Input5", typeof(string));
        //        dt.Columns.Add("Input6", typeof(string));
        //        dt.Columns.Add("InputAvg", typeof(decimal));

        //    }
        //    // Retrieve the dictionary of specimen numbers from ViewState
        //    Dictionary<string, int> specimenNumbers = ViewState["SpecimenNumbers"] as Dictionary<string, int>;
        //    if (specimenNumbers == null)
        //    {
        //        specimenNumbers = new Dictionary<string, int>();
        //        ViewState["SpecimenNumbers"] = specimenNumbers;
        //    }
        //    // Retrieve the current specimen number for the selected category (CorePhase or TestCategory)
        //    string category = ddlCorePhase.SelectedValue;  // Use ddlTestCategory if needed
        //    int currentSpecimenNo = specimenNumbers.ContainsKey(category) ? specimenNumbers[category] : 1;
        //    // Create a new row
        //    DataRow dr = dt.NewRow();
        //    dr["ItemCategory"] = ddlTestCategory.SelectedItem.Text;

        //    dr["CorePhase"] = ddlCorePhase.SelectedValue;
        //    dr["SpecimenNo"] = specimenNo;
        //    //dr["Weightload"] = Weightload;
        //    //dr["ThicknessatTemperature"] = ThicknessatTemperature;
        //    //dr["DeformationTemperature"] = DeformationTemperature;
        //    //dr["Median"] = Median;


        //    dr["Input1"] = txtInput1.Text.Trim();
        //    dr["Input2"] = txtInput2.Text.Trim();
        //    dr["Input3"] = txtInput3.Text.Trim();
        //    dr["Input4"] = txtInput4.Text.Trim();
        //    dr["Input5"] = txtInput5.Text.Trim();
        //    dr["Input6"] = txtInput6.Text.Trim();
        //    dr["InputAvg"] = txtAvg.Text.Trim();
        //    // Add the row to the DataTable
        //    dt.Rows.Add(dr);

        //    // Update the ViewState
        //    ViewState["TestResults"] = dt;

        //    // Rebind the GridView
        //    gvPressureTest.DataSource = dt;
        //    gvPressureTest.DataBind();

        //    // Store current specimen for weightload data entry
        //    ViewState["CurrentSpecimen"] = specimenNo;
        //    txtspeicimentnumebrForweghightload.Text = specimenNo.ToString();
        //    txtcorephase2.Text = ddlCorePhase.SelectedItem.Text;

        //    // Disable Add to Grid button until weightload data is added for this specimen
        //    btnAddToGrid.Enabled = false;

        //    // Enable Add Weightload Data button
        //    btnAddMassVariation.Enabled = true;

        //    // Clear the input fields for next entry
        //    ClearInputFields();
        //}
        private void UpdateSpecimenNumber()
        {
            // Get the DataTable from ViewState
            DataTable dt = (DataTable)ViewState["TestResults"];

            // Calculate next specimen number
            int nextSpecimenNo = 1; // Default to 1 if no rows
            if (dt != null && dt.Rows.Count > 0)
            {
                nextSpecimenNo = dt.Rows.Count + 1;
            }

            // Set the value if found
            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
            }
        }

        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0;
            decimal inputAvg = 0;

            UpdateSpecimenNumber();

   
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
            if (string.IsNullOrWhiteSpace(txtAvg.Text) || !decimal.TryParse(txtAvg.Text.Trim(), out inputAvg))
            {
                invalidControls.Add(txtAvg);
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

            // Get the DataTable from ViewState
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("ItemCategory", typeof(string));
                dt.Columns.Add("Input1", typeof(string));
                dt.Columns.Add("Input2", typeof(string));
                dt.Columns.Add("Input3", typeof(string));
                dt.Columns.Add("Input4", typeof(string));
                dt.Columns.Add("Input5", typeof(string));
                dt.Columns.Add("Input6", typeof(string));
                dt.Columns.Add("InputAvg", typeof(decimal));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["ItemCategory"] = ddlTestCategory.SelectedItem.Text;
            dr["CorePhase"] = ddlCorePhase.SelectedValue;
            dr["SpecimenNo"] = specimenNo;
            dr["Input1"] = txtInput1.Text.Trim();
            dr["Input2"] = txtInput2.Text.Trim();
            dr["Input3"] = txtInput3.Text.Trim();
            dr["Input4"] = txtInput4.Text.Trim();
            dr["Input5"] = txtInput5.Text.Trim();
            dr["Input6"] = txtInput6.Text.Trim();
            dr["InputAvg"] = txtAvg.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["TestResults"] = dt;

            // Rebind the GridView
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();

            // Store current specimen for weightload data entry
            ViewState["CurrentSpecimenForWeightload"] = specimenNo;
            txtspeicimentnumebrForweghightload.Text = specimenNo.ToString();
            txtcorephase2.Text = ddlCorePhase.SelectedItem.Text;


            // Disable Add to Grid button until weightload data is added for this specimen
            btnAddToGrid.Enabled = false;

            // Enable Add Mass Variation button
            btnAddMassVariation.Enabled = true;

            // Clear the input fields for next entry
            ClearInputFields();

            ShowMessage($"Specimen {specimenNo} added. Please add mass variation data for this specimen before adding another.");
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvPressureTest.Rows.Count > 0);

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
            if (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                // Validate Test Details Input 

                if (string.IsNullOrEmpty(txttotalsurfacearea.Text) || !double.TryParse(txttotalsurfacearea.Text, out _))
                {
                    errors.Add("Total Surface area must be valid.");
                    invalidControls.Add(txttotalsurfacearea);
                }
                if (string.IsNullOrEmpty(txtweightM1.Text) || !double.TryParse(txtweightM1.Text, out _))
                {
                    errors.Add("Weight must be a valid number.");
                    invalidControls.Add(txtweightM1);
                }
                if (string.IsNullOrEmpty(txtweightM2.Text) || !double.TryParse(txtweightM2.Text, out _))
                {
                    errors.Add("Weight must be a valid number.");
                    invalidControls.Add(txtweightM2);
                }
                if (string.IsNullOrEmpty(txtweightM3.Text) || !double.TryParse(txtweightM3.Text, out _))
                {
                    errors.Add("Weight must be a valid number.");
                    invalidControls.Add(txtweightM3);
                }
            }

            // Check if Grid has data (only required if grid is empty)
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
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

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);
            }
            else
            {
                // Save data logic here (e.g., database insertion)
                Submitform();
                // Show success message
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                    "alert('Test results and Report Generated successfully!');", true);
                // Clear form
                ClearForm();
            }
        }


        //protected void btnAddMassVariation_Click(object sender, EventArgs e)
        //{
        //    DataTable dtTestResults = ViewState["TestResults"] as DataTable;
        //    if (dtTestResults == null || dtTestResults.Rows.Count == 0)
        //    {
        //        ShowMessage("Please add specimen data first before adding mass variation data.");
        //        return;
        //    }

        //    string currentCategory = ddlTestCategory.SelectedValue;
        //    int currentSpecimen = Convert.ToInt32(ViewState["CurrentSpecimenForWeightload"]);

        //    // Get or create mass variation DataTable
        //    DataTable dtMassVariation = ViewState["MassVariationData"] as DataTable;
        //    if (dtMassVariation == null)
        //    {
        //        dtMassVariation = new DataTable();
        //        dtMassVariation.Columns.Add("CorePhase", typeof(string));
        //        dtMassVariation.Columns.Add("SpecimenNo", typeof(int));
        //        dtMassVariation.Columns.Add("TotalSurfaceArea", typeof(double));
        //        dtMassVariation.Columns.Add("M1", typeof(double));
        //        dtMassVariation.Columns.Add("M2", typeof(double));
        //        dtMassVariation.Columns.Add("M3", typeof(double));
        //        dtMassVariation.Columns.Add("mgpercm2", typeof(double));
        //        dtMassVariation.Columns.Add("Mean", typeof(double));
        //    }

        //    // Check if mass variation data already exists for this specimen
        //    DataRow[] existingRows = dtMassVariation.Select($"SpecimenNo = {currentSpecimen}");
        //    if (existingRows.Length > 0)
        //    {
        //        ShowMessage($"Mass variation data already exists for Specimen {currentSpecimen}. Please add a new specimen first.");
        //        return;
        //    }

        //    // Parse and validate input fields
        //    double totalSurfaceArea, weightM1, weightM2, weightM3;

        //    if (string.IsNullOrWhiteSpace(txttotalsurfacearea.Text) || !double.TryParse(txttotalsurfacearea.Text.Trim(), out totalSurfaceArea))
        //    {
        //        ShowMessage("Please enter a valid Total Surface Area.");
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(txtweightM1.Text) || !double.TryParse(txtweightM1.Text.Trim(), out weightM1))
        //    {
        //        ShowMessage("Please enter a valid M1 weight.");
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(txtweightM2.Text) || !double.TryParse(txtweightM2.Text.Trim(), out weightM2))
        //    {
        //        ShowMessage("Please enter a valid M2 weight.");
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(txtweightM3.Text) || !double.TryParse(txtweightM3.Text.Trim(), out weightM3))
        //    {
        //        ShowMessage("Please enter a valid M3 weight.");
        //        return;
        //    }


        //    // Add new mass variation data row
        //    DataRow newRow = dtMassVariation.NewRow();
        //    newRow["CorePhase"] = txtcorephase2.Text;
        //    newRow["SpecimenNo"] = currentSpecimen;
        //    newRow["TotalSurfaceArea"] = totalSurfaceArea;
        //    newRow["M1"] = weightM1;
        //    newRow["M2"] = weightM2;
        //    newRow["M3"] = weightM3;
        //    dtMassVariation.Rows.Add(newRow);

        //    // Save updated data
        //    ViewState["MassVariationData"] = dtMassVariation;

        //    // Bind to GridView
        //    gvMassVariation.DataSource = dtMassVariation;
        //    gvMassVariation.DataBind();

        //    // Calculate next specimen number
        //    int nextSpecimenNo = currentSpecimen + 1;
        //    txtSpecimenNo.Text = nextSpecimenNo.ToString();

        //    // Clear mass variation input fields
        //    txtcorephase2.Text = "";
        //    txttotalsurfacearea.Text = "";
        //    txtweightM1.Text = "";
        //    txtweightM2.Text = "";
        //    txtweightM3.Text = "";
        //    txtspeicimentnumebrForweghightload.Text = "";

        //    // Re-enable Add to Grid button for next specimen
        //    btnAddToGrid.Enabled = true;

        //    // Disable Add Mass Variation button until next specimen is added
        //    btnAddMassVariation.Enabled = false;

        //    ShowMessage($"Mass variation data added for Specimen {currentSpecimen}. You can now add the next specimen.");
        //}
       
        //protected void gvMassVariation_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        // Get values from data item
        //        double m1 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "M1"));
        //        double m2 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "M2"));
        //        double m3 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "M3"));
        //        double surfaceArea = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "TotalSurfaceArea"));
        //        string corePhase = DataBinder.Eval(e.Row.DataItem, "CorePhase").ToString();

        //        // Store the core phase value in the row's cells to access it later
        //        // We'll use the first cell's ToolTip property to store this value
        //        e.Row.Cells[0].ToolTip = corePhase;

        //        // Calculate mg/cm² value
        //        double mgPerCm2 = (m3 < m1) ? (m2 - m3) / surfaceArea : (m2 - m1) / surfaceArea;

        //        // Apply formatting or styling based on values if needed
        //        if (mgPerCm2 > 0)
        //        {
        //            // You can add special styling for positive values
        //            // e.Row.Cells[6].CssClass = "positive-value";
        //        }
        //        else if (mgPerCm2 < 0)
        //        {
        //            // You can add special styling for negative values
        //            // e.Row.Cells[6].CssClass = "negative-value";
        //        }

        //        // For merging Core Phase cells
        //        DataTable dt = (DataTable)ViewState["MassVariationData"];

        //        if (e.Row.RowIndex > 0)
        //        {
        //            // Previous row's Core Phase value - get it from the cells we stored it in
        //            GridViewRow prevRow = gvMassVariation.Rows[e.Row.RowIndex - 1];
        //            string prevCorePhase = prevRow.Cells[0].ToolTip;

        //            // If current Core Phase matches previous row's Core Phase
        //            if (corePhase == prevCorePhase)
        //            {
        //                // Hide current row's Core Phase cell
        //                e.Row.Cells[0].Visible = false;

        //                // Find the first row with this core phase
        //                int firstRowIndex = -1;
        //                for (int i = e.Row.RowIndex - 1; i >= 0; i--)
        //                {
        //                    GridViewRow row = gvMassVariation.Rows[i];
        //                    if (row.Cells[0].ToolTip != corePhase || !row.Cells[0].Visible)
        //                    {
        //                        firstRowIndex = i + 1;
        //                        break;
        //                    }

        //                    if (i == 0)
        //                    {
        //                        firstRowIndex = 0;
        //                        break;
        //                    }
        //                }

        //                if (firstRowIndex >= 0)
        //                {
        //                    GridViewRow firstRow = gvMassVariation.Rows[firstRowIndex];

        //                    // Increase rowspan for Core Phase column
        //                    if (firstRow.Cells[0].RowSpan == 0)
        //                    {
        //                        firstRow.Cells[0].RowSpan = 2; // Start with 2 rows
        //                    }
        //                    else
        //                    {
        //                        firstRow.Cells[0].RowSpan += 1; // Increment rowspan
        //                    }

        //                    // Do the same for Mean column (assuming it's the second-to-last column)
        //                    int meanColumnIndex = e.Row.Cells.Count - 2;
        //                    e.Row.Cells[meanColumnIndex].Visible = false;

        //                    if (firstRow.Cells[meanColumnIndex].RowSpan == 0)
        //                    {
        //                        firstRow.Cells[meanColumnIndex].RowSpan = 2;
        //                    }
        //                    else
        //                    {
        //                        firstRow.Cells[meanColumnIndex].RowSpan += 1;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //private int FindFirstRowInGroup(int currentRowIndex, string corePhase)
        //{
        //    for (int i = currentRowIndex - 1; i >= 0; i--)
        //    {
        //        GridViewRow row = gvMassVariation.Rows[i];
        //        string rowCorePhase = ((DataRowView)row.DataItem)["CorePhase"].ToString();

        //        if (rowCorePhase != corePhase)
        //        {
        //            // If we find a different Core Phase, return the next row (first in current group)
        //            return i + 1;
        //        }

        //        if (i == 0)
        //        {
        //            // If we reach the first row and it's still the same Core Phase, return it
        //            return 0;
        //        }
        //    }

        //    return -1; // Should not reach here
        //}
        protected void btnAddMassVariation_Click(object sender, EventArgs e)
        {
            DataTable dtTestResults = ViewState["TestResults"] as DataTable;
            if (dtTestResults == null || dtTestResults.Rows.Count == 0)
            {
                ShowMessage("Please add specimen data first before adding mass variation data.");
                return;
            }

            string currentCategory = ddlTestCategory.SelectedValue;
            int currentSpecimen = Convert.ToInt32(ViewState["CurrentSpecimenForWeightload"]);

            // Get or create mass variation DataTable
            DataTable dtMassVariation = ViewState["MassVariationData"] as DataTable;
            if (dtMassVariation == null)
            {
                dtMassVariation = new DataTable();
                dtMassVariation.Columns.Add("CorePhase", typeof(string));
                dtMassVariation.Columns.Add("SpecimenNo", typeof(int));
                dtMassVariation.Columns.Add("TotalSurfaceArea", typeof(double));
                dtMassVariation.Columns.Add("M1", typeof(double));
                dtMassVariation.Columns.Add("M2", typeof(double));
                dtMassVariation.Columns.Add("M3", typeof(double));
                dtMassVariation.Columns.Add("mgpercm2", typeof(double));
                dtMassVariation.Columns.Add("Mean", typeof(double));
            }

            // Check if mass variation data already exists for this specimen
            DataRow[] existingRows = dtMassVariation.Select($"SpecimenNo = {currentSpecimen}");
            if (existingRows.Length > 0)
            {
                ShowMessage($"Mass variation data already exists for Specimen {currentSpecimen}. Please add a new specimen first.");
                return;
            }

            // Parse and validate input fields
            double totalSurfaceArea, weightM1, weightM2, weightM3;

            if (string.IsNullOrWhiteSpace(txttotalsurfacearea.Text) || !double.TryParse(txttotalsurfacearea.Text.Trim(), out totalSurfaceArea))
            {
                ShowMessage("Please enter a valid Total Surface Area.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtweightM1.Text) || !double.TryParse(txtweightM1.Text.Trim(), out weightM1))
            {
                ShowMessage("Please enter a valid M1 weight.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtweightM2.Text) || !double.TryParse(txtweightM2.Text.Trim(), out weightM2))
            {
                ShowMessage("Please enter a valid M2 weight.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtweightM3.Text) || !double.TryParse(txtweightM3.Text.Trim(), out weightM3))
            {
                ShowMessage("Please enter a valid M3 weight.");
                return;
            }

            // Calculate mgpercm2 value
            double mgPerCm2;
            if (weightM3 < weightM1)
            {
                mgPerCm2 = (weightM2 - weightM3) / totalSurfaceArea;
            }
            else // M3 >= M1
            {
                mgPerCm2 = (weightM2 - weightM1) / totalSurfaceArea;
            }

            // Add new mass variation data row
            DataRow newRow = dtMassVariation.NewRow();
            newRow["CorePhase"] = txtcorephase2.Text;
            newRow["SpecimenNo"] = currentSpecimen;
            newRow["TotalSurfaceArea"] = totalSurfaceArea;
            newRow["M1"] = weightM1;
            newRow["M2"] = weightM2;
            newRow["M3"] = weightM3;
            newRow["mgpercm2"] = Math.Round(mgPerCm2, 2); // Save the calculated value
            dtMassVariation.Rows.Add(newRow);

            // Calculate and update mean value for all rows with the same CorePhase
            CalculateAndUpdateMeanValues(dtMassVariation, txtcorephase2.Text);

            // Save updated data
            ViewState["MassVariationData"] = dtMassVariation;

            // Bind to GridView
            gvMassVariation.DataSource = dtMassVariation;
            gvMassVariation.DataBind();

            // Calculate next specimen number
            int nextSpecimenNo = currentSpecimen + 1;
            txtSpecimenNo.Text = nextSpecimenNo.ToString();

            // Clear mass variation input fields
            txtcorephase2.Text = "";
            txttotalsurfacearea.Text = "";
            txtweightM1.Text = "";
            txtweightM2.Text = "";
            txtweightM3.Text = "";
            txtspeicimentnumebrForweghightload.Text = "";

            // Re-enable Add to Grid button for next specimen
            btnAddToGrid.Enabled = true;

            // Disable Add Mass Variation button until next specimen is added
            btnAddMassVariation.Enabled = false;

            ShowMessage($"Mass variation data added for Specimen {currentSpecimen}. You can now add the next specimen.");
        }
        private void CalculateAndUpdateMeanValues(DataTable dtMassVariation, string corePhase)
        {
            // Group rows by CorePhase
            var corePhases = new HashSet<string>();
            foreach (DataRow row in dtMassVariation.Rows)
            {
                corePhases.Add(row["CorePhase"].ToString());
            }

            // Calculate mean for each core phase
            foreach (string phase in corePhases)
            {
                double sum = 0;
                int count = 0;

                // Calculate sum of mgpercm2 values for this phase
                foreach (DataRow row in dtMassVariation.Select($"CorePhase = '{phase}'"))
                {
                    sum += Convert.ToDouble(row["mgpercm2"]);
                    count++;
                }

                // Calculate mean and update all rows for this phase
                if (count > 0)
                {
                    double mean = Math.Round(sum / count, 2);
                    foreach (DataRow row in dtMassVariation.Select($"CorePhase = '{phase}'"))
                    {
                        row["Mean"] = mean;
                    }
                }
            }
        }
        protected string CalculateMgPerCm2(object dataItem)
        {
            DataRowView rowView = dataItem as DataRowView;
            if (rowView == null) return "0";

            double surfaceArea = Convert.ToDouble(rowView["TotalSurfaceArea"]);
            if (surfaceArea <= 0) return "0";

            double m1 = Convert.ToDouble(rowView["M1"]);
            double m2 = Convert.ToDouble(rowView["M2"]);
            double m3 = Convert.ToDouble(rowView["M3"]);

            double mgPerCm2;

            // Apply the rules from the image
            if (m3 < m1)
            {
                mgPerCm2 = (m2 - m3) / surfaceArea;
            }
            else // M3 >= M1
            {
                mgPerCm2 = (m2 - m1) / surfaceArea;
            }

            return mgPerCm2.ToString("F2");
        }
        protected string CalculateMean(object dataItem)
        {
            // Get the DataTable from ViewState
            DataTable dtMassVariation = ViewState["MassVariationData"] as DataTable;
            if (dtMassVariation == null || dtMassVariation.Rows.Count == 0)
                return "0";

            double sum = 0;
            int count = 0;

            foreach (DataRow row in dtMassVariation.Rows)
            {
                // Create a DataRowView from the DataRow (if needed)
                // Alternatively, compute mg/cm² directly without DataRowView
                double m1 = Convert.ToDouble(row["M1"]);
                double m2 = Convert.ToDouble(row["M2"]);
                double m3 = Convert.ToDouble(row["M3"]);
                double area = Convert.ToDouble(row["TotalSurfaceArea"]);

                // Apply the same logic as CalculateMgPerCm2
                double mgPerCm2 = (m3 < m1) ? (m2 - m3) / area : (m2 - m1) / area;
                sum += mgPerCm2;
                count++;
            }

            if (count == 0)
                return "0";

            double mean = sum / count;
            return mean.ToString("F2"); // Format to 2 decimal places
        }
        protected void gvMassVariation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get values from data item
                double m1 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "M1"));
                double m2 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "M2"));
                double m3 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "M3"));
                double surfaceArea = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "TotalSurfaceArea"));
                string corePhase = DataBinder.Eval(e.Row.DataItem, "CorePhase").ToString();
                double mgPerCm2 = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "mgpercm2"));

                // Store the core phase value in the row's cells to access it later
                // We'll use the first cell's ToolTip property to store this value
                e.Row.Cells[0].ToolTip = corePhase;

                // Apply formatting or styling based on values if needed
                if (mgPerCm2 > 0)
                {
                    // You can add special styling for positive values
                    // e.Row.Cells[6].CssClass = "positive-value";
                }
                else if (mgPerCm2 < 0)
                {
                    // You can add special styling for negative values
                    // e.Row.Cells[6].CssClass = "negative-value";
                }

                // For merging Core Phase cells
                DataTable dt = (DataTable)ViewState["MassVariationData"];

                if (e.Row.RowIndex > 0)
                {
                    // Previous row's Core Phase value - get it from the cells we stored it in
                    GridViewRow prevRow = gvMassVariation.Rows[e.Row.RowIndex - 1];
                    string prevCorePhase = prevRow.Cells[0].ToolTip;

                    // If current Core Phase matches previous row's Core Phase
                    if (corePhase == prevCorePhase)
                    {
                        // Hide current row's Core Phase cell
                        e.Row.Cells[0].Visible = false;

                        // Find the first row with this core phase
                        int firstRowIndex = -1;
                        for (int i = e.Row.RowIndex - 1; i >= 0; i--)
                        {
                            GridViewRow row = gvMassVariation.Rows[i];
                            if (row.Cells[0].ToolTip != corePhase || !row.Cells[0].Visible)
                            {
                                firstRowIndex = i + 1;
                                break;
                            }

                            if (i == 0)
                            {
                                firstRowIndex = 0;
                                break;
                            }
                        }

                        if (firstRowIndex >= 0)
                        {
                            GridViewRow firstRow = gvMassVariation.Rows[firstRowIndex];

                            // Increase rowspan for Core Phase column
                            if (firstRow.Cells[0].RowSpan == 0)
                            {
                                firstRow.Cells[0].RowSpan = 2; // Start with 2 rows
                            }
                            else
                            {
                                firstRow.Cells[0].RowSpan += 1; // Increment rowspan
                            }

                            // Do the same for Mean column (assuming it's the second-to-last column)
                            int meanColumnIndex = e.Row.Cells.Count - 2;
                            e.Row.Cells[meanColumnIndex].Visible = false;

                            if (firstRow.Cells[meanColumnIndex].RowSpan == 0)
                            {
                                firstRow.Cells[meanColumnIndex].RowSpan = 2;
                            }
                            else
                            {
                                firstRow.Cells[meanColumnIndex].RowSpan += 1;
                            }
                        }
                    }
                }
            }
        }
        protected void gvMassVariation_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Get data keys from the row being deleted
            int specimenNo = Convert.ToInt32(gvMassVariation.DataKeys[e.RowIndex].Values["SpecimenNo"]);

            // Retrieve the current DataTable from ViewState
            DataTable dtMassVariation = ViewState["MassVariationData"] as DataTable;

            if (dtMassVariation != null)
            {
                // Find the row with the matching specimen number
                DataRow[] rows = dtMassVariation.Select("SpecimenNo = " + specimenNo);
                if (rows.Length > 0)
                {
                    // Remove the row from the DataTable
                    dtMassVariation.Rows.Remove(rows[0]);

                    // Update the ViewState to reflect the changes
                    ViewState["MassVariationData"] = dtMassVariation;

                    // Rebind the GridView to display the updated data
                    gvMassVariation.DataSource = dtMassVariation;
                    gvMassVariation.DataBind();

                    ShowMessage("Specimen " + specimenNo + " deleted successfully.");
                }
            }
        }

        private string[] SplitAndTrim(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? new[] { string.Empty }
                : input.Split(',').Select(s => s.Trim()).ToArray();
        }

      
        protected void gvPressureTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt = ViewState["TestResults"] as DataTable;
                int rowIndex = e.Row.RowIndex;
                if (dt == null || rowIndex >= dt.Rows.Count)
                    return;

                // Grouped merges (hierarchical logic)
                if (rowIndex > 0)
                {
                    string currentItemCategory = dt.Rows[rowIndex]["ItemCategory"].ToString();
                    string prevItemCategory = dt.Rows[rowIndex - 1]["ItemCategory"].ToString();
                    string currentCorePhase = dt.Rows[rowIndex]["CorePhase"].ToString();
                    string prevCorePhase = dt.Rows[rowIndex - 1]["CorePhase"].ToString();
                    string currentSpecimen = dt.Rows[rowIndex]["SpecimenNo"].ToString();
                    string prevSpecimen = dt.Rows[rowIndex - 1]["SpecimenNo"].ToString();

                    bool sameItemCategory = currentItemCategory == prevItemCategory;
                    bool sameCorePhase = currentCorePhase == prevCorePhase;
                    bool sameSpecimen = currentSpecimen == prevSpecimen;

                    // 1. Merge ItemCategory (Column 0)
                    if (sameItemCategory)
                        ProcessColumn(dt, rowIndex, e.Row, 0); // 0 = ItemCategory

                    // 2. Merge CorePhase (Column 1)
                    if (sameItemCategory && sameCorePhase)
                        ProcessColumn(dt, rowIndex, e.Row, 1); // 1 = CorePhase

                    // 3. Merge SpecimenNo (Column 2)
                    if (sameItemCategory && sameCorePhase && sameSpecimen)
                        ProcessColumn(dt, rowIndex, e.Row, 2); // 2 = SpecimenNo

                    // Check if the columns exist before accessing them
                    // The column indexes might need adjustment based on your actual GridView structure

                    // Note the capitalization difference: "Weightload" vs "WeightLoad"
                    if (dt.Columns.Contains("Weightload") &&
                        dt.Rows[rowIndex]["Weightload"].ToString() == dt.Rows[rowIndex - 1]["Weightload"].ToString())
                        ProcessColumn(dt, rowIndex, e.Row, 3); // 3 = Weightload (adjust index if needed)

                    // Note the capitalization difference: "ThicknessatTemperature" vs "ThicknessAtTemperature"
                    if (dt.Columns.Contains("ThicknessatTemperature") &&
                        dt.Rows[rowIndex]["ThicknessatTemperature"].ToString() == dt.Rows[rowIndex - 1]["ThicknessatTemperature"].ToString())
                        ProcessColumn(dt, rowIndex, e.Row, 4); // 4 = ThicknessatTemperature (adjust index if needed)

                    // Note the capitalization difference: "DeformationTemperature" vs "DeformationTemperature"
                    if (dt.Columns.Contains("DeformationTemperature") &&
                        dt.Rows[rowIndex]["DeformationTemperature"].ToString() == dt.Rows[rowIndex - 1]["DeformationTemperature"].ToString())
                        ProcessColumn(dt, rowIndex, e.Row, 5); // 5 = DeformationTemperature (adjust index if needed)

                    // Add similar checks for any other columns you want to merge
                }
            }
        }
        protected void gvPressureTest_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                string selectedCategory = ViewState["SelectedCategory"]?.ToString() ?? "Category";

                GridViewRow topHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);

                topHeader.Cells.Add(new TableHeaderCell { Text = "Core Phase", RowSpan = 2 });
                topHeader.Cells.Add(new TableHeaderCell { Text = "Specimen No.", RowSpan = 2 });

                TableHeaderCell categoryGroup = new TableHeaderCell
                {
                    Text = selectedCategory,
                    ColumnSpan = 7,
                    HorizontalAlign = HorizontalAlign.Center,
                    CssClass = "group-header"
                };
                topHeader.Cells.Add(categoryGroup);

                topHeader.Cells.Add(new TableHeaderCell { Text = "Weight Load", RowSpan = 2 });

                // ✅ Group Header: After 6 Hours under load...
                TableHeaderCell after6Group = new TableHeaderCell
                {
                    Text = "After 6 Hours under load<br/>at temperature 110°C",
                    ColumnSpan = 2,
                    HorizontalAlign = HorizontalAlign.Center
                };
                after6Group.Attributes["style"] = "text-align: center; white-space: normal;";
                topHeader.Cells.Add(after6Group);

                // ✅ Median is separate
                topHeader.Cells.Add(new TableHeaderCell { Text = "Median", RowSpan = 2 });

                // ✅ Actions column
                topHeader.Cells.Add(new TableHeaderCell { Text = "Actions", RowSpan = 2 });

                gvPressureTest.Controls[0].Controls.AddAt(0, topHeader);

                // Sub-header
                GridViewRow subHeader = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
                for (int i = 1; i <= 6; i++)
                {
                    subHeader.Cells.Add(new TableHeaderCell { Text = i.ToString(), HorizontalAlign = HorizontalAlign.Center });
                }
                subHeader.Cells.Add(new TableHeaderCell { Text = "Avg", HorizontalAlign = HorizontalAlign.Center });

                subHeader.Cells.Add(new TableHeaderCell { Text = "Thickness", HorizontalAlign = HorizontalAlign.Center });
                subHeader.Cells.Add(new TableHeaderCell { Text = "Deformation", HorizontalAlign = HorizontalAlign.Center });

                gvPressureTest.Controls[0].Controls.AddAt(1, subHeader);
            }
        }
        protected void txtInput6_TextChanged(object sender, EventArgs e)
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

            // Count valid values and calculate sum
            decimal sum = 0;
            int validCount = 0;

            foreach (string input in inputs)
            {
                // Skip "-" values
                if (input == "-")
                    continue;

                // Try to parse the decimal
                decimal value;
                if (decimal.TryParse(input, out value))
                {
                    sum += value;
                    validCount++;
                }
            }

            // Calculate average if we have at least one valid value
            if (validCount > 0)
            {
                decimal avg = Math.Round(sum / validCount, 2);
                txtAvg.Text = avg.ToString();
            }
            else
            {
                txtAvg.Text = "0";
            }
        }

        // Helper method
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

            // Count valid values and calculate sum
            decimal sum = 0;
            int validCount = 0;

            foreach (string input in inputs)
            {
                // Skip "-" values
                if (input == "-")
                    continue;

                // Try to parse the decimal
                decimal value;
                if (decimal.TryParse(input, out value))
                {
                    sum += value;
                    validCount++;
                }
            }

            // Calculate average if we have at least one valid value
            if (validCount > 0)
            {
                decimal avg = Math.Round(sum / validCount, 2);
                txtAvg.Text = avg.ToString();
            }
            else
            {
                txtAvg.Text = "0";
            }
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            try
            {
                // Get column name from the column index
                string columnName = dt.Columns[columnIndex].ColumnName;

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
        protected void gvPressureTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["TestResults"];
                dt.Rows.RemoveAt(rowIndex);
                ViewState["TestResults"] = dt;
                gvPressureTest.DataSource = dt;
                gvPressureTest.DataBind();

                // Update the Specimen No. in the upper section
                UpdateSpecimenNumber();
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

        private void ClearForm()
        {
            // Main Section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";

            // Test Details
            ddlCorePhase.SelectedIndex = 0;
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
            txtInput6.Text = "";
            txtAvg.Text = "";

            txttotalsurfacearea.Text = "";
            txtweightM1.Text = "";
            txtweightM2.Text = "";
            txtweightM3.Text = "";


            // Clear GridView
            gvPressureTest.DataSource = null;
            gvPressureTest.DataBind();

        }
        private void ClearInputFields()
        {
            txttotalsurfacearea.Text = "";
            txtweightM1.Text = "";
            txtweightM2.Text = "";
            txtweightM3.Text = "";
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
            txtInput6.Text = "";
      
        }
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            //  allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);
            //  allTextBoxes.Add(testdate);
            allTextBoxes.Add(txtAvg);
            allTextBoxes.Add(txttotalsurfacearea);
            allTextBoxes.Add(txtweightM1);
            allTextBoxes.Add(txtweightM2);
            allTextBoxes.Add(txtweightM3);

 
            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        private void Submitform()
        {
            try
            {
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return;
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

                // Thickness Measurements (Formatted)
                List<string> thicknessSpecimens = new List<string>();
                var thicknessRows = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString().Contains("Thickness") && !r["ItemCategory"].ToString().Contains("Avg"))
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
                    reportParamMapList.Add("8274"); // Thickness
                    resultList.Add(string.Join("|", thicknessSpecimens));
                }

                // Width (Diameter) Measurements (Formatted)
                List<string> diameterSpecimens = new List<string>();
                var diameterRows = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString().Contains("Diameter") && !r["ItemCategory"].ToString().Contains("Avg"))
                    .ToList();

                var diameterGroups = diameterRows.GroupBy(r => r["CorePhase"].ToString());
                foreach (var group in diameterGroups)
                {
                    string core = group.Key;
                    int specimenNum = 1;
                    foreach (var row in group)
                    {
                        string inputs = string.Format("{0},{1},{2},{3},{4},{5}",
                            row["Input1"], row["Input2"], row["Input3"],
                            row["Input4"], row["Input5"], row["Input6"]);
                        diameterSpecimens.Add($"{core};{specimenNum}:{inputs}");
                        specimenNum++;
                    }
                }

                if (diameterSpecimens.Any())
                {
                    reportParamMapList.Add("8275"); // Width
                    resultList.Add(string.Join("|", diameterSpecimens));
                }

                // Length Measurements (Formatted)
                List<string> lengthSpecimens = new List<string>();
                var lengthRows = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString().Contains("Length") && !r["ItemCategory"].ToString().Contains("Avg"))
                    .ToList();

                var lengthGroups = lengthRows.GroupBy(r => r["CorePhase"].ToString());
                foreach (var group in lengthGroups)
                {
                    string core = group.Key;
                    int specimenNum = 1;
                    foreach (var row in group)
                    {
                        string inputs = string.Format("{0},{1},{2},{3},{4},{5}",
                            row["Input1"], row["Input2"], row["Input3"],
                            row["Input4"], row["Input5"], row["Input6"]);
                        lengthSpecimens.Add($"{core};{specimenNum}:{inputs}");
                        specimenNum++;
                    }
                }

                if (lengthSpecimens.Any())
                {
                    reportParamMapList.Add("8276"); // Length 
                    resultList.Add(string.Join("|", lengthSpecimens));
                }

                // Thickness Averages
                List<string> thicknessAvgValues = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString() == "Thickness")
                    .Select(r => r["InputAvg"] != DBNull.Value ? Convert.ToDecimal(r["InputAvg"]).ToString("0.##") : "")
                    .Where(val => !string.IsNullOrWhiteSpace(val))
                    .ToList();

                if (thicknessAvgValues.Any())
                {
                    reportParamMapList.Add("8312"); // ThicknessAvg
                    resultList.Add(string.Join("|", thicknessAvgValues));
                }

                // Width (Diameter) Averages
                List<string> diameterAvgValues = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString() == "Diameter")
                    .Select(r => r["InputAvg"] != DBNull.Value ? Convert.ToDecimal(r["InputAvg"]).ToString("0.##") : "")
                    .Where(val => !string.IsNullOrWhiteSpace(val))
                    .ToList();

                if (diameterAvgValues.Any())
                {
                    reportParamMapList.Add("8313"); // DiameterAvg(width)
                    resultList.Add(string.Join("|", diameterAvgValues));
                }

                // Length Averages
                List<string> lengthAvgValues = dt.AsEnumerable()
                    .Where(r => r["ItemCategory"].ToString() == "Length")
                    .Select(r => r["InputAvg"] != DBNull.Value ? Convert.ToDecimal(r["InputAvg"]).ToString("0.##") : "")
                    .Where(val => !string.IsNullOrWhiteSpace(val))
                    .ToList();

                if (lengthAvgValues.Any())
                {
                    reportParamMapList.Add("8314"); // LengthAvg 
                    resultList.Add(string.Join("|", lengthAvgValues));
                }

                //  Weight Load Data (Formatted)
                DataTable dtWeightData = ViewState["MassVariationData"] as DataTable;
                if (dtWeightData != null && dtWeightData.Rows.Count > 0)
                {
                    List<string> totalsurfacearea = new List<string>();
                    List<string> WeightM1 = new List<string>();
                    List<string> WeightM2 = new List<string>();
                    List<string> WeightM3 = new List<string>();
                    List<string> mgpercm = new List<string>();
                    List<string> Mean = new List<string>();

                    foreach (DataRow row in dtWeightData.Rows)
                    {
                        totalsurfacearea.Add(row["TotalSurfaceArea"]?.ToString() ?? "");
                        WeightM1.Add(row["M1"]?.ToString() ?? "");
                        WeightM2.Add(row["M2"]?.ToString() ?? "");
                        WeightM3.Add(row["M3"]?.ToString() ?? "");
                        mgpercm.Add(row["mgpercm2"]?.ToString() ?? "");
                        Mean.Add(row["Mean"]?.ToString() ?? "");
                    }

                    reportParamMapList.Add("8277"); 
                    resultList.Add(string.Join("|", totalsurfacearea));
                    reportParamMapList.Add("8278");
                    resultList.Add(string.Join("|", WeightM1));
                    reportParamMapList.Add("8279");
                    resultList.Add(string.Join("|", WeightM2));
                    reportParamMapList.Add("8280");
                    resultList.Add(string.Join("|", WeightM3));
                    reportParamMapList.Add("8281");
                    resultList.Add(string.Join("|", mgpercm));
                    reportParamMapList.Add("8282");
                    resultList.Add(string.Join("|", Mean));

                }
                // Add Specific values
                reportParamMapList.Add("8296"); resultList.Add(txtSpecificThickness.Text); // Standard temperature
                reportParamMapList.Add("8283"); resultList.Add(txtSpecificWidth.Text); // Standard duration
                reportParamMapList.Add("8284"); resultList.Add(txtSpecificLength.Text); // Standard before heating
                reportParamMapList.Add("8285"); resultList.Add(txtSpecifictotalsurfacearea.Text); // Standard before heating
                reportParamMapList.Add("8286"); resultList.Add(txtSpecificmgcm2.Text); // Standard after heating
                reportParamMapList.Add("8287"); resultList.Add(txtSpecificMean.Text); // Standard result


                // Helper function to concatenate date and time values safely
                string ConcatenateValues(string dateTimeValue)
                {
                    // Return empty string if the input is null or empty
                    if (string.IsNullOrEmpty(dateTimeValue?.Trim()))
                        return string.Empty;
                    return dateTimeValue;
                }

                // Vacuum temperature at 70°C
                reportParamMapList.Add("8288"); // Example ID, assign actual parameter code
                resultList.Add(ConcatenateValues(txtVacuumDateStart.Text + " " + txtVacuumTimeStart.Text)); // VacuumTemp70Start

                reportParamMapList.Add("8289");
                resultList.Add(ConcatenateValues(txtVacuumDateFinish.Text + " " + txtVacuumTimeFinish.Text)); // VacuumTemp70Finish

                // Conditioning in desiccator after Vacuum
                reportParamMapList.Add("8290");
                resultList.Add(ConcatenateValues(txtCond1DateStart.Text + " " + txtCond1TimeStart.Text)); // CondAfterVacuumStart

                reportParamMapList.Add("8291");
                resultList.Add(ConcatenateValues(txtCond1DateFinish.Text + " " + txtCond1TimeFinish.Text)); // CondAfterVacuumFinish

                // Immersed at 85°C
                reportParamMapList.Add("8292");
                resultList.Add(ConcatenateValues(txtImmDateStart.Text + " " + txtImmTimeStart.Text)); // Immersed85Start

                reportParamMapList.Add("8293");
                resultList.Add(ConcatenateValues(txtImmDateFinish.Text + " " + txtImmTimeFinish.Text)); // Immersed85Finish

                // Second Vacuum temperature at 70°C
                reportParamMapList.Add("8297");
                resultList.Add(ConcatenateValues(txtVacuum2DateStart.Text + " " + txtVacuum2TimeStart.Text)); // Vacuum2Temp70Start

                reportParamMapList.Add("8298");
                resultList.Add(ConcatenateValues(txtVacuum2DateFinish.Text + " " + txtVacuum2TimeFinish.Text)); // Vacuum2Temp70Finish

                // Condition in Desiccator
                reportParamMapList.Add("8294");
                resultList.Add(ConcatenateValues(txtCond2DateStart.Text + " " + txtCond2TimeStart.Text)); // DesiccatorCondStart

                reportParamMapList.Add("8295");
                resultList.Add(ConcatenateValues(txtCond2DateFinish.Text + " " + txtCond2TimeFinish.Text)); // DesiccatorCondFinish


                // Upper section
                reportParamMapList.Add("8262"); resultList.Add(cableType.Text); // Cable Type
                reportParamMapList.Add("8263"); resultList.Add(refSpec.Text); // Reference Specification
                reportParamMapList.Add("8264"); resultList.Add(cableSize.Text); // Cable Size
                reportParamMapList.Add("8265"); resultList.Add(testMethodNo.Text); // Test Method No
                reportParamMapList.Add("8266"); resultList.Add(voltage.Text); // Voltage
                reportParamMapList.Add("8267"); resultList.Add(jobOrderNo.Text); // Job Order No
                reportParamMapList.Add("8268"); resultList.Add(worksheetno.Text); // Worksheet No
                reportParamMapList.Add("8269"); resultList.Add(roomno.Text); // Room No
                reportParamMapList.Add("8270"); resultList.Add(roomhumidity.Text); // Room Humidity
                reportParamMapList.Add("8271"); resultList.Add(roomtemp.Text); // Room Temperature
                reportParamMapList.Add("8272"); resultList.Add(Date.Text); // Date
                reportParamMapList.Add("8273"); resultList.Add(equipmentno.Text); // Equipment No

                // Final save
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results and Report Generated successfully!');", true);

                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    Response.Redirect("GenerateTestReport_PressureTestAtHighTemperature.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results.');", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvPressureTest.Rows.Count > 0);
            bool gridWeightLoadHasData = (gvMassVariation.Rows.Count > 0);

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
            if (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }
            if (!gridWeightLoadHasData)
            {
                if (string.IsNullOrEmpty(txtspeicimentnumebrForweghightload.Text))
                {
                    errors.Add("Specimen No. for Weight Load is required.");
                    invalidControls.Add(txtspeicimentnumebrForweghightload);
                }
                if (string.IsNullOrEmpty(txttotalsurfacearea.Text) || !double.TryParse(txttotalsurfacearea.Text, out _))
                {
                    errors.Add("Total Surface area must be valid.");
                    invalidControls.Add(txttotalsurfacearea);
                }
                if (string.IsNullOrEmpty(txtweightM1.Text) || !double.TryParse(txtweightM1.Text, out _))
                {
                    errors.Add("Weight must be a valid number.");
                    invalidControls.Add(txtweightM1);
                }
                if (string.IsNullOrEmpty(txtweightM2.Text) || !double.TryParse(txtweightM2.Text, out _))
                {
                    errors.Add("Weight must be a valid number.");
                    invalidControls.Add(txtweightM2);
                }
                if (string.IsNullOrEmpty(txtweightM3.Text) || !double.TryParse(txtweightM3.Text, out _))
                {
                    errors.Add("Weight must be a valid number.");
                    invalidControls.Add(txtweightM3);
                }
            }

            if (!gridHasData && !gridWeightLoadHasData)
            {
                errors.Add("Add at least one test result to either grid.");
            }

            // Validate Standard Values section
            if (string.IsNullOrEmpty(txtSpecificThickness.Text) || !double.TryParse(txtSpecificThickness.Text, out _))
            {
                errors.Add("SpecificThickness must be a valid number.");
                invalidControls.Add(txtSpecificThickness);
            }
            if (string.IsNullOrEmpty(txtSpecificWidth.Text) || !double.TryParse(txtSpecificWidth.Text, out _))
            {
                errors.Add("Specific Duration must be a valid number.");
                invalidControls.Add(txtSpecificWidth);
            }
            if (string.IsNullOrEmpty(txtSpecificLength.Text) || !double.TryParse(txtSpecificLength.Text, out _))
            {
                errors.Add("SpecificWeightload value must be a valid number.");
                invalidControls.Add(txtSpecificLength);
            }
            if (string.IsNullOrEmpty(txtSpecificmgcm2.Text) || !double.TryParse(txtSpecificmgcm2.Text, out _))
            {
                errors.Add("SpecificDeformation value must be a valid number.");
                invalidControls.Add(txtSpecificmgcm2);
            }
            if (string.IsNullOrEmpty(txtSpecificMean.Text) || !double.TryParse(txtSpecificMean.Text, out _))
            {
                errors.Add("SpecificMedian value must be a valid number.");
                invalidControls.Add(txtSpecificMean);
            }
            // Check if Grid has data (only required if grid is empty)
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
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

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
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

                        // Create DAL object
                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;

                        // First delete existing records for this sample
                        DeleteExistingRecords(sampleId);

                        // Then save the updated data (reusing your Submitform logic)
                        Submitform();

                        // Show success message
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateSuccess",
                            "alert('Test results updated successfully!');", true);
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
    }
}