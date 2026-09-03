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
using LIMS.DAL;


namespace LIMS
{
    public partial class Dimensionofzinccoatingtest : System.Web.UI.Page
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
                    //// Set dynamic values from database
                    cableType.Text = dt1.Rows[0]["SampleType"].ToString();
                    voltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();

                    //cableType.Text = "cy/pfc";
                    //voltage.Text = "25";
                    //cableSize.Text = "45";
                    //testMethodNo.Text = "12345";
                    //refSpec.Text = "45678";

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

                // ADD THIS LINE - Initialize the grid
                InitializeGrid();

                CheckAndPrefillForm();
            }
        }

        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();

            // Column order should match your GridView display order
            dt.Columns.Add("ItemType", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(string));  // Keep this to match GridView
            dt.Columns.Add("Input1", typeof(string));
            dt.Columns.Add("Input2", typeof(string));
            dt.Columns.Add("Input3", typeof(string));
            dt.Columns.Add("Average", typeof(string));
            dt.Columns.Add("Specification", typeof(string));

            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
            gvDimension.DataSource = dt;
            gvDimension.DataBind();
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

                        // Prefill standard values
                        //PrefillStandardValues(existingData);

                        // Prefill ageing and conditioning details
                        //PrefillAgeingAndConditioningDetails(existingData);

                        // Prefill test results grid
                        PrefillDimensionGrid(sampleId);
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
        private void PrefillDimensionGrid(int sampleId)
        {
            try
            {
                // New parameter map
                var paramMap = new Dictionary<int, string>
        {
            { 12593, "Diameter of steel wire" },
            { 12594, "Weight/Thickness before immersed" },
            { 12595, "Weight/Thickness after immersed" },
            { 12596, "Weight/Thickness zinc coating immersed" }
        };

                // Load results from DB
                Dictionary<int, string> allResults = new Dictionary<int, string>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ReportParamMapID, Result FROM TestResultsPTQualis WHERE SampleID = @SampleID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int paramId = Convert.ToInt32(reader["ReportParamMapID"]);
                                string result = reader["Result"].ToString();
                                allResults[paramId] = result;
                            }
                        }
                    }
                }

                // Prepare DataTable matching GridView columns
                DataTable dt = new DataTable();
                dt.Columns.Add("ItemType");
                dt.Columns.Add("Unit");
                dt.Columns.Add("Input1");
                dt.Columns.Add("Input2");
                dt.Columns.Add("Input3");
                dt.Columns.Add("Average");
                dt.Columns.Add("Specification");

                // Process results
                foreach (var entry in allResults)
                {
                    int paramId = entry.Key;
                    string rawResult = entry.Value;

                    if (!paramMap.ContainsKey(paramId) || string.IsNullOrWhiteSpace(rawResult))
                        continue;

                    string[] sections = rawResult.Split('|');
                    if (sections.Length < 3)
                        continue;

                    string unit = sections[0].Trim();
                    string[] values = sections[1].Split(',');
                    string avg = values.Length > 3 ? values[3].Trim() : "-";
                    string specified = sections[2].Trim();

                    DataRow row = dt.NewRow();
                    row["ItemType"] = paramMap[paramId];
                    row["Unit"] = unit;
                    row["Input1"] = values.Length > 0 ? values[0].Trim() : "-";
                    row["Input2"] = values.Length > 1 ? values[1].Trim() : "-";
                    row["Input3"] = values.Length > 2 ? values[2].Trim() : "-";
                    row["Average"] = avg;
                    row["Specification"] = specified;

                    dt.Rows.Add(row);
                }

                // Bind to GridView
                gvDimension.DataSource = dt;
                gvDimension.DataBind();
                ViewState["TestResults"] = dt;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error loading dimension grid: {ex.Message}');", true);
            }
        }

        private void PrefillUpperSection(DataTable existingData)
        { 

            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "12598");
            SetLabelText(refSpec, "12603");
            SetLabelText(cableSize, "12597");
            SetLabelText(testMethodNo, "12604");
            SetLabelText(voltage, "12599");
            SetLabelText(jobOrderNo, "12605");
            SetLabelText(worksheetno, "12606");
            SetTextBoxText(roomno, "12601");
            SetTextBoxText(roomhumidity, "12607");
            SetTextBoxText(roomtemp, "12602");
            SetTextBoxText(Date, "12600");
            SetLabelText(equipmentno, "12608");

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
        protected void txtInput_TextChanged(object sender, EventArgs e)
        {
            List<double> values = new List<double>();

            // Collect values from 3 textboxes
            string[] inputs = {
        txtInput1.Text.Trim(),
        txtInput2.Text.Trim(),
        txtInput3.Text.Trim()
    };

            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue; // Skip empty fields
                }
                else if (input == "-")
                {
                    values.Add(0); // Treat "-" as zero
                }
                else if (double.TryParse(input, out double number))
                {
                    values.Add(number); // Add valid numbers
                }
            }

            if (values.Count > 0)
            {
                double avg = values.Average(); // Calculate the average
                txtAvg.Text = avg.ToString("0.##"); // Display with up to 2 decimal places
            }
            else
            {
                txtAvg.Text = ""; // Clear if nothing to average
            }
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
            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            decimal input1 = 0, input2 = 0, input3 = 0, avg = 0;

            // Reset previous error styles
            ddlItemType.BorderColor = System.Drawing.Color.Empty;
            ddlUnit.BorderColor = System.Drawing.Color.Empty;
            txtInput1.BorderColor = System.Drawing.Color.Empty;
            txtInput2.BorderColor = System.Drawing.Color.Empty;
            txtInput3.BorderColor = System.Drawing.Color.Empty;

            // Validate Item Type
            if (ddlItemType.SelectedIndex <= 0 || ddlItemType.SelectedValue == "Select")
            {
                ddlItemType.BorderColor = System.Drawing.Color.Red;
                hasError = true;
            }

            // Validate Unit
            if (ddlUnit.SelectedIndex <= 0 || ddlUnit.SelectedValue == "Select")
            {
                ddlUnit.BorderColor = System.Drawing.Color.Red;
                hasError = true;
            }

            // Validate Inputs
            if (!decimal.TryParse(txtInput1.Text.Trim(), out input1))
            {
                txtInput1.BorderColor = System.Drawing.Color.Red;
                hasError = true;
            }
            if (!decimal.TryParse(txtInput2.Text.Trim(), out input2))
            {
                txtInput2.BorderColor = System.Drawing.Color.Red;
                hasError = true;
            }
            if (!decimal.TryParse(txtInput3.Text.Trim(), out input3))
            {
                txtInput3.BorderColor = System.Drawing.Color.Red;
                hasError = true;
            }

            // Early return if any validation failed
            if (hasError)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill in all fields correctly.');", true);
                return;
            }

            // Calculate average
            avg = Math.Round((input1 + input2 + input3) / 3, 2);
            txtAvg.Text = avg.ToString("0.##");

            // Get DataTable from ViewState
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("ItemType", typeof(string));
                dt.Columns.Add("Unit", typeof(string));
                dt.Columns.Add("Input1", typeof(string));
                dt.Columns.Add("Input2", typeof(string));
                dt.Columns.Add("Input3", typeof(string));
                dt.Columns.Add("Average", typeof(string));
                dt.Columns.Add("Specification", typeof(string));
            }

            //  Check for duplicate ItemType
            string selectedItemType = ddlItemType.SelectedItem.Text.Trim();
            bool alreadyExists = dt.AsEnumerable()
                                   .Any(row => row.Field<string>("ItemType").Equals(selectedItemType, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "duplicateAlert",
                    $"alert('The parameter \"{selectedItemType}\" has already been added.');", true);
                return;
            }

            // Add new row
            DataRow dr = dt.NewRow();
            dr["ItemType"] = selectedItemType;
            dr["Unit"] = ddlUnit.SelectedItem.Text;
            dr["Input1"] = txtInput1.Text.Trim();
            dr["Input2"] = txtInput2.Text.Trim();
            dr["Input3"] = txtInput3.Text.Trim();
            dr["Average"] = avg.ToString("0.##");
            dr["Specification"] = txtSpecifications.Text.Trim();

            dt.Rows.Add(dr);
            ViewState["TestResults"] = dt;

            // Bind updated data to grid
            gvDimension.DataSource = dt;
            gvDimension.DataBind();

            // Clear form fields
            ddlItemType.SelectedIndex = 0;
            ddlUnit.SelectedIndex = 0;
            txtInput1.Text = txtInput2.Text = txtInput3.Text = txtAvg.Text = txtSpecifications.Text = "";
        }


        protected void gvDimension_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Don't append unit to individual values - just display the raw values
                // The unit is already shown in the "Unit" column

                // Column indices after removing SpecimenNo:
                // ItemType=0, Unit=1, Input1=2, Input2=3, Input3=4, Average=5, Specification=6

                // Display Input1, Input2, Input3 as raw values
                for (int i = 1; i <= 3; i++)
                {
                    string inputVal = DataBinder.Eval(e.Row.DataItem, $"Input{i}")?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(inputVal))
                    {
                        e.Row.Cells[i + 1].Text = inputVal;  // Just show the number, no unit
                    }
                }

                // Display Average as raw value
                string avgVal = DataBinder.Eval(e.Row.DataItem, "Average")?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(avgVal))
                {
                    e.Row.Cells[5].Text = avgVal;  // Just show the number, no unit
                }
            }
        }
        protected void gvDimension_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                // Check if the "TestResults" ViewState exists
                DataTable dt = (DataTable)ViewState["TestResults"];
                //if (dt == null)
                //{
                //    // If "TestResults" doesn't exist, try "DimensionResults"
                //    dt = (DataTable)ViewState["DimensionResults"];
                //}

                // Ensure the DataTable exists and the row index is valid before attempting to remove the row
                if (dt != null && dt.Rows.Count > 0 && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(rowIndex);  // Remove the row at the specified index
                    ViewState["TestResults"] = dt;  // Update ViewState with the modified DataTable for "TestResults"
                                                    // ViewState["DimensionResults"] = dt;  // Update ViewState with the modified DataTable for "DimensionResults"

                    gvDimension.DataSource = dt;  // Rebind the updated DataTable to the GridView
                    gvDimension.DataBind();  // Refresh the grid to reflect the deletion

                    // Update Specimen Number (if necessary)

                }
            }
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            try
            {
                // Add boundary checks
                if (dt == null || currentRowIndex <= 0 || currentRowIndex >= dt.Rows.Count ||
                    columnIndex < 0 || columnIndex >= dt.Columns.Count)
                {
                    return;
                }

                // Get column name from the column index
                string columnName = dt.Columns[columnIndex].ColumnName;

                // Get current cell value
                string currentValue = dt.Rows[currentRowIndex][columnName]?.ToString() ?? "";

                // Get previous cell value
                string previousValue = dt.Rows[currentRowIndex - 1][columnName]?.ToString() ?? "";

                // If values are the same and not empty
                if (!string.IsNullOrEmpty(currentValue) && currentValue == previousValue)
                {
                    // Hide current cell
                    currentRow.Cells[columnIndex].Visible = false;

                    // Find the first visible cell above with the same value
                    int spanRowIndex = currentRowIndex - 1;
                    while (spanRowIndex >= 0 && !gvDimension.Rows[spanRowIndex].Cells[columnIndex].Visible)
                    {
                        spanRowIndex--;
                    }

                    // Increase rowspan of that cell
                    if (spanRowIndex >= 0)
                    {
                        if (gvDimension.Rows[spanRowIndex].Cells[columnIndex].RowSpan == 0)
                        {
                            gvDimension.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                        }
                        else
                        {
                            gvDimension.Rows[spanRowIndex].Cells[columnIndex].RowSpan += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Add error logging or handling as needed
                System.Diagnostics.Debug.WriteLine("Error in ProcessColumn: " + ex.Message);
            }
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

            // Test Details
            // testdate.Text = "";
            ddlItemType.SelectedIndex = 0;       
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtAvg.Text = "";
          

            // Clear GridView
            gvDimension.DataSource = null;
            gvDimension.DataBind();

            //// Standard Values
            //txtStandardWidth.Text = "";
            //txtStandardSpeed.Text = "";
            //txtStandardmeasured.Text = "";
            //txtStandardMin.Text = "";
            //txtStandardMax.Text = "";


        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvDimension.Rows.Count > 0);

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

                if (string.IsNullOrEmpty(txtInput1.Text) || (txtInput1.Text != "-" && !double.TryParse(txtInput1.Text, out _)))
                {
                    errors.Add("Input 1 must be valid number or '-'.");
                    invalidControls.Add(txtInput1);
                }
                if (string.IsNullOrEmpty(txtInput2.Text) || (txtInput2.Text != "-" && !double.TryParse(txtInput2.Text, out _)))
                {
                    errors.Add("Input 2 must be a valid number or '-'.");
                    invalidControls.Add(txtInput2);
                }
                if (string.IsNullOrEmpty(txtInput3.Text) || (txtInput3.Text != "-" && !double.TryParse(txtInput3.Text, out _)))
                {
                    errors.Add("Input 3 value must be a valid number or '-'.");
                    invalidControls.Add(txtInput3);
                }
                if (string.IsNullOrEmpty(txtSpecifications.Text) || (txtSpecifications.Text != "-" && !double.TryParse(txtSpecifications.Text, out _)))
                {
                    errors.Add("Specifications value must be a valid number or '-'.");
                    invalidControls.Add(txtSpecifications);
                }
                if (ddlUnit.SelectedIndex <= 0 || ddlUnit.SelectedValue == "Select" || string.IsNullOrWhiteSpace(ddlUnit.SelectedValue))
                {
                    errors.Add("Unit must be selected.");
                    ddlUnit.CssClass += " input-error";
                    invalidControls.Add(ddlUnit);
                }
                if (ddlItemType.SelectedIndex <= 0 || ddlItemType.SelectedValue == "Select" || string.IsNullOrWhiteSpace(ddlItemType.SelectedValue))
                {
                    errors.Add("Item type must be selected.");
                    ddlItemType.CssClass += " input-error";
                    invalidControls.Add(ddlItemType);
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
                foreach (var control in invalidControls)
                {
                    if (control is TextBox txt)
                    {
                        txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
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


                var groupedData = dt.AsEnumerable().GroupBy(r => r["ItemType"].ToString());

                foreach (var group in groupedData)
                {
                    string itemType = group.Key;
                    List<string> dataForThisType = new List<string>();

                    foreach (DataRow row in group)
                    {
                        string unit = row["Unit"].ToString();
                        string input1 = row["Input1"].ToString();
                        string input2 = row["Input2"].ToString();
                        string input3 = row["Input3"].ToString();
                        string average = row["Average"].ToString();
                        string specification = row["Specification"].ToString();

                        // Format: unitvalue|1,2,3,avg|specified value
                        string formattedData = $"{unit}|{input1},{input2},{input3},{average}|{specification}";
                        dataForThisType.Add(formattedData);
                    }

                    // Determine parameter ID based on ItemType using ReportParamMapID
                    string paramId = "";
                    if (itemType.ToLower().Contains("diameter") && itemType.ToLower().Contains("steel") && itemType.ToLower().Contains("wire"))
                        paramId = "12593"; // Diameter of steel wire
                    else if (itemType.ToLower().Contains("weight") || itemType.ToLower().Contains("thickness"))
                    {
                        if (itemType.ToLower().Contains("before") && itemType.ToLower().Contains("immersed"))
                            paramId = "12594"; // Weight/Thickness before immersed
                        else if (itemType.ToLower().Contains("after") && itemType.ToLower().Contains("immersed"))
                            paramId = "12595"; // Weight/Thickness after immersed
                        else if ((itemType.ToLower().Contains("zinc") && itemType.ToLower().Contains("coating") && itemType.ToLower().Contains("immersed"))
                              || itemType.ToLower().Contains("zinccoating"))
                            paramId = "12596"; // Weight/Thickness zinc coating immersed
                    }


                    if (!string.IsNullOrEmpty(paramId) && dataForThisType.Any())
                    {
                        reportParamMapList.Add(paramId);
                        resultList.Add(string.Join("|", dataForThisType));
                    }
                }



               
                // Upper section form fields
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

                    Response.Redirect("GenerateDimensionOfZincCoatingTest.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID);
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvDimension.Rows.Count > 0);
            //bool gridWeightLoadHasData = (gvWeightloadData.Rows.Count > 0);

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
                bool hasError = false;
                int specimenNo = 0;

                // Validate Test Details Input

                if (ddlItemType.SelectedIndex <= 0 || ddlItemType.SelectedValue == "Select")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a valid ItemType.');", true);
                    ddlItemType.CssClass += " input-error";
                    invalidControls.Add(ddlItemType);
                    hasError = true;
                }

                if (ddlItemType.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(ddlItemType.SelectedValue))
                {
                    ddlItemType.CssClass += " input-error";
                    invalidControls.Add(ddlItemType);
                    hasError = true;
                }                    

                // Validate Input1 to Input6
                TextBox[] inputTextBoxes = { txtInput1, txtInput2, txtInput3};

                foreach (TextBox txt in inputTextBoxes)
                {
                    string inputVal = txt.Text.Trim();

                    // Allow "-" as valid input by converting to "0"
                    if (inputVal == "-")
                    {
                        txt.Text = "0";
                        inputVal = "0";
                    }

                    if (string.IsNullOrEmpty(inputVal) || !decimal.TryParse(inputVal, out decimal result))
                    {
                        invalidControls.Add(txt);
                        hasError = true;
                        txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
                    }
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
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
    }
}