using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class FiberTest2 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the grid
                InitializeGrid();

                // Initialize specimen number
                txtSpecimenNo.Text = "1";

                //upper section
                roomno.Text = "Mechanical";
                cableType.Text = "CIVA PELSIDSTAPE";
                refSpec.Text = "IEC 60502-2";
                cableSize.Text = "3 x 25 mm²";
                testMethodNo.Text = "IEC 60502-2";
                voltage.Text = "3,6/6(7,2)kV";
                jobOrderNo.Text = "WO/2024120905";
                worksheetno.Text = "-";
                equipmentno.Text = "QE/12/PH/16,QE/16/PH/03,QE/15/PH/43";
            }
        }

        //Binds or initialize
        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("ItemTest", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(string));
            dt.Columns.Add("WidthStrip", typeof(string));
            dt.Columns.Add("PullingSpeed", typeof(string));
            dt.Columns.Add("Measured", typeof(string));
            dt.Columns.Add("MinStrippability", typeof(string));
            dt.Columns.Add("MaxStrippability", typeof(string));

            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;

            // Sort the DataView by the columns you want to merge
            DataView dv = dt.DefaultView;
            dv.Sort = "Date, ItemTest, CorePhase, SpecimenNo";

            // Bind the empty DataTable to the GridView
            gvStrippabilityTest.DataSource = dt;
            gvStrippabilityTest.DataBind();
        }

        //Grids 
        protected void gvStrippabilityTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the DataTable
                DataTable dt = (DataTable)ViewState["TestResults"];

                // Current row index
                int rowIndex = e.Row.RowIndex;

                // Handle Date column - only merge if ItemTest is the same
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["Date"].ToString() == dt.Rows[rowIndex - 1]["Date"].ToString() &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 0);
                }

                // Handle ItemTest column - only merge if it's the same as previous row
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 1);
                }

                // Handle Core Phase column - merge only within same ItemTest group
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString() &&
                    dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 2);
                }

                // Handle Strippability Min. column
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["MinStrippability"].ToString() == dt.Rows[rowIndex - 1]["MinStrippability"].ToString() &&
                    dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString() &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 7); // Assuming Strippability Min is at index 7
                }

                // Handle Strippability Max. column
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["MaxStrippability"].ToString() == dt.Rows[rowIndex - 1]["MaxStrippability"].ToString() &&
                    dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString() &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 8); // Assuming Strippability Max is at index 8
                }
            }
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            // Get current cell value
            string currentValue = dt.Rows[currentRowIndex][columnIndex].ToString();

            // Get previous cell value
            string previousValue = dt.Rows[currentRowIndex - 1][columnIndex].ToString();

            // If values are the same
            if (currentValue == previousValue)
            {
                // Hide current cell
                currentRow.Cells[columnIndex].Visible = false;

                // Find the first visible cell above with the same value
                int spanRowIndex = currentRowIndex - 1;
                while (spanRowIndex > 0 && !gvStrippabilityTest.Rows[spanRowIndex].Cells[columnIndex].Visible)
                {
                    spanRowIndex--;
                }

                // Increase rowspan of that cell
                if (gvStrippabilityTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan == 0)
                {
                    gvStrippabilityTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                }
                else
                {
                    gvStrippabilityTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan += 1;
                }
            }
        }
        protected void gvStrippabilityTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["TestResults"];
                dt.Rows.RemoveAt(rowIndex);
                ViewState["TestResults"] = dt;
                gvStrippabilityTest.DataSource = dt;
                gvStrippabilityTest.DataBind();

                // Update the Specimen No. in the upper section
                UpdateSpecimenNumber();
            }
        }

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

            // Find your actual control - you need to use the correct ID and search method
            // If it's in a form:
            TextBox txtSpecimenNo = FindControl("txtSpecimenNo") as TextBox;

            // If the control isn't directly accessible via FindControl, try:
            // TextBox txtSpecimenNo = this.txtSpecimenNo; // If it's declared as a class member

            // Or if it's somewhere else in the page hierarchy:
            // TextBox txtSpecimenNo = Page.FindControl("txtSpecimenNo") as TextBox;

            // Set the value if found
            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
            }
         
        }

        //ddl's
        protected void ddlTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSpecimenNo.Text = "1";
        }
        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSpecimenNo.Text = "1";
        }

        //Buttons
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0;
            decimal widthStrip = 0, pullingSpeed = 0, measured = 0;

            if (string.IsNullOrWhiteSpace(testdate.Text))
            {
                invalidControls.Add(testdate);
                hasError = true;
            }

            if (ddlTestType.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlTestType.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlTestType.CssClass += " input-error";
                hasError = true;
            }

            if (ddlCorePhase.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlCorePhase.CssClass += " input-error";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text, out specimenNo))
            {
                invalidControls.Add(txtSpecimenNo);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtWidthStrip.Text) || !decimal.TryParse(txtWidthStrip.Text, out widthStrip))
            {
                invalidControls.Add(txtWidthStrip);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtPullingSpeed.Text) || !decimal.TryParse(txtPullingSpeed.Text, out pullingSpeed))
            {
                invalidControls.Add(txtPullingSpeed);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtMeasured.Text) || !decimal.TryParse(txtMeasured.Text, out measured))
            {
                invalidControls.Add(txtMeasured);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150); // Darker red
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
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
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("ItemTest", typeof(string));
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("WidthStrip", typeof(decimal));
                dt.Columns.Add("PullingSpeed", typeof(decimal));
                dt.Columns.Add("Measured", typeof(decimal));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate.Text;
            dr["ItemTest"] = ddlTestType.SelectedValue;
            dr["CorePhase"] = ddlCorePhase.SelectedValue;
            dr["SpecimenNo"] = specimenNo;
            dr["WidthStrip"] = widthStrip;
            dr["PullingSpeed"] = pullingSpeed;
            dr["Measured"] = measured;

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["TestResults"] = dt;

            // Calculate and update Min/Max values for each core phase and test type
            UpdateMinMaxValues(dt);

            // Rebind the GridView
            gvStrippabilityTest.DataSource = dt;
            gvStrippabilityTest.DataBind();

            // Increment the specimen number
            txtSpecimenNo.Text = (specimenNo + 1).ToString();

            // Clear the input fields for next entry
            ClearInputFields();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvStrippabilityTest.Rows.Count > 0);

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
                if (string.IsNullOrEmpty(testdate.Text))
                {
                    errors.Add("Test Date is required.");
                    invalidControls.Add(testdate);
                }
                if (string.IsNullOrEmpty(txtSpecimenNo.Text))
                {
                    errors.Add("Specimen No. is required.");
                    invalidControls.Add(txtSpecimenNo);
                }
                if (string.IsNullOrEmpty(txtWidthStrip.Text) || !double.TryParse(txtWidthStrip.Text, out _))
                {
                    errors.Add("Width Strip must be a valid number.");
                    invalidControls.Add(txtWidthStrip);
                }
                if (string.IsNullOrEmpty(txtPullingSpeed.Text) || !double.TryParse(txtPullingSpeed.Text, out _))
                {
                    errors.Add("Pulling Speed must be a valid number.");
                    invalidControls.Add(txtPullingSpeed);
                }
                if (string.IsNullOrEmpty(txtMeasured.Text) || !double.TryParse(txtMeasured.Text, out _))
                {
                    errors.Add("Measured value must be a valid number.");
                    invalidControls.Add(txtMeasured);
                }
            }

            // Validate Standard Values section
            if (string.IsNullOrEmpty(txtStandardWidth.Text) || !double.TryParse(txtStandardWidth.Text, out _))
            {
                errors.Add("Standard Width must be a valid number.");
                invalidControls.Add(txtStandardWidth);
            }
            if (string.IsNullOrEmpty(txtStandardSpeed.Text) || !double.TryParse(txtStandardSpeed.Text, out _))
            {
                errors.Add("Standard Pulling Speed must be a valid number.");
                invalidControls.Add(txtStandardSpeed);
            }
            if (string.IsNullOrEmpty(txtStandardmeasured.Text) || !double.TryParse(txtStandardmeasured.Text, out _))
            {
                errors.Add("Standard Measured value must be a valid number.");
                invalidControls.Add(txtStandardmeasured);
            }
            if (string.IsNullOrEmpty(txtStandardMin.Text) || !double.TryParse(txtStandardMin.Text, out _))
            {
                errors.Add("Minimum Value must be a valid number.");
                invalidControls.Add(txtStandardMin);
            }
            if (string.IsNullOrEmpty(txtStandardMax.Text) || !double.TryParse(txtStandardMax.Text, out _))
            {
                errors.Add("Maximum Value must be a valid number.");
                invalidControls.Add(txtStandardMax);
            }

            // Validate Ageing section
            if (string.IsNullOrEmpty(txtSpecimenAirflow.Text))
            {
                errors.Add("Specimen Airflow must be a valid number.");
                invalidControls.Add(txtSpecimenAirflow);
            }
            if (string.IsNullOrEmpty(txtCableAirflow.Text))
            {
                errors.Add("Cable Airflow must be a valid number.");
                invalidControls.Add(txtCableAirflow);
            }

            // Validate Ageing specimen dates and times
            if (string.IsNullOrEmpty(txtSpecimenDateStart.Text))
            {
                errors.Add("Specimen Start Date is required.");
                invalidControls.Add(txtSpecimenDateStart);
            }
            if (string.IsNullOrEmpty(txtSpecimenDateFinish.Text))
            {
                errors.Add("Specimen Finish Date is required.");
                invalidControls.Add(txtSpecimenDateFinish);
            }
            if (string.IsNullOrEmpty(txtSpecimenTimeStart.Text))
            {
                errors.Add("Specimen Start Time is required.");
                invalidControls.Add(txtSpecimenTimeStart);
            }
            if (string.IsNullOrEmpty(txtSpecimenTimeFinish.Text))
            {
                errors.Add("Specimen Finish Time is required.");
                invalidControls.Add(txtSpecimenTimeFinish);
            }

            // Validate Ageing complete cable dates and times
            if (string.IsNullOrEmpty(txtCableDateStart.Text))
            {
                errors.Add("Cable Start Date is required.");
                invalidControls.Add(txtCableDateStart);
            }
            if (string.IsNullOrEmpty(txtCableDateFinish.Text))
            {
                errors.Add("Cable Finish Date is required.");
                invalidControls.Add(txtCableDateFinish);
            }
            if (string.IsNullOrEmpty(txtCableTimeStart.Text))
            {
                errors.Add("Cable Start Time is required.");
                invalidControls.Add(txtCableTimeStart);
            }
            if (string.IsNullOrEmpty(txtCableTimeFinish.Text))
            {
                errors.Add("Cable Finish Time is required.");
                invalidControls.Add(txtCableTimeFinish);
            }

            // Validate Conditioning in desiccators dates and times
            if (string.IsNullOrEmpty(txtDesiccatorsDateStart.Text))
            {
                errors.Add("Desiccators Start Date is required.");
                invalidControls.Add(txtDesiccatorsDateStart);
            }
            if (string.IsNullOrEmpty(txtDesiccatorsDateFinish.Text))
            {
                errors.Add("Desiccators Finish Date is required.");
                invalidControls.Add(txtDesiccatorsDateFinish);
            }
            if (string.IsNullOrEmpty(txtDesiccatorsTimeStart.Text))
            {
                errors.Add("Desiccators Start Time is required.");
                invalidControls.Add(txtDesiccatorsTimeStart);
            }
            if (string.IsNullOrEmpty(txtDesiccatorsTimeFinish.Text))
            {
                errors.Add("Desiccators Finish Time is required.");
                invalidControls.Add(txtDesiccatorsTimeFinish);
            }

            // Validate Conditioning dates and times
            if (string.IsNullOrEmpty(txtConditioningDateStart.Text))
            {
                errors.Add("Conditioning Start Date is required.");
                invalidControls.Add(txtConditioningDateStart);
            }
            if (string.IsNullOrEmpty(txtConditioningDateFinish.Text))
            {
                errors.Add("Conditioning Finish Date is required.");
                invalidControls.Add(txtConditioningDateFinish);
            }
            if (string.IsNullOrEmpty(txtConditioningTimeStart.Text))
            {
                errors.Add("Conditioning Start Time is required.");
                invalidControls.Add(txtConditioningTimeStart);
            }
            if (string.IsNullOrEmpty(txtConditioningTimeFinish.Text))
            {
                errors.Add("Conditioning Finish Time is required.");
                invalidControls.Add(txtConditioningTimeFinish);
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
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }


        //Data Saving
        private void Submitform() 
        {
            try
            {
                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    // Show error message if no data to save
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return;
                }
               
                // Create DAL object
                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) { _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower()); }
                if (Request.QueryString["r"] != null) { _cm.ReportID = Request.QueryString["r"]; }
                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                // Organize data by test types (Before aging and After aging)
                Dictionary<string, List<DataRow>> testTypeData = new Dictionary<string, List<DataRow>>();

                foreach (DataRow row in dt.Rows)
                {
                    string testType = row["ItemTest"].ToString();

                    if (!testTypeData.ContainsKey(testType))
                    {
                        testTypeData[testType] = new List<DataRow>();
                    }

                    testTypeData[testType].Add(row);
                }

                // Prepare arrays for the AddTestResultGlobal method
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Process each test type and collect min/max values
                List<string> beforeAgeingMinValues = new List<string>();
                List<string> beforeAgeingMaxValues = new List<string>();
                List<string> afterAgeingMinValues = new List<string>();
                List<string> afterAgeingMaxValues = new List<string>();

                foreach (var testType in testTypeData.Keys)
                {
                    var rows = testTypeData[testType];
                    string testDate = rows[0]["Date"].ToString();

                    // Get all core phases in order of specimen numbers
                    string corePhases = string.Join(",", rows.Select(r => r["CorePhase"].ToString()));

                    // Get all width strips in order
                    string widthStrips = string.Join(",", rows.Select(r => r["WidthStrip"].ToString()));

                    // Get all pulling speeds in order
                    string pullingSpeeds = string.Join(",", rows.Select(r => r["PullingSpeed"].ToString()));

                    // Get all measured values in order
                    string measuredValues = string.Join(",", rows.Select(r => r["Measured"].ToString()));

                    // Group rows by CorePhase to get unique core phases
                    var corePhaseGroups = rows.GroupBy(r => r["CorePhase"].ToString()).ToList();
                    // Collect min and max values
                    if (testType.Contains("Before"))
                    {
                        foreach (var group in corePhaseGroups)
                        {
                            beforeAgeingMinValues.Add(group.First()["MinStrippability"].ToString());
                            beforeAgeingMaxValues.Add(group.First()["MaxStrippability"].ToString());
                        }

                        // Add parameters for Before Ageing
                        reportParamMapList.Add("2125"); // BeforeAgeingDate param ID
                        resultList.Add(testDate);

                        // Add core phases for Before Ageing
                        reportParamMapList.Add("2113"); // ID for Before Ageing core phases
                        resultList.Add(corePhases);

                        // Add other measurements
                        reportParamMapList.Add("2114"); resultList.Add(widthStrips); // BeforeAgeingWidthStrip
                        reportParamMapList.Add("2115"); resultList.Add(pullingSpeeds); // BeforeAgeingPullingSpeed
                        reportParamMapList.Add("2116"); resultList.Add(measuredValues); // BeforeAgeingMeasured
                    }
                    else if (testType.Contains("After"))
                    {
                        foreach (var group in corePhaseGroups)
                        {
                            afterAgeingMinValues.Add(group.First()["MinStrippability"].ToString());
                            afterAgeingMaxValues.Add(group.First()["MaxStrippability"].ToString());
                        }

                        // Add parameters for After Ageing
                        reportParamMapList.Add("2126"); // AfterAgeingDate param ID
                        resultList.Add(testDate);

                        // Add core phases for After Ageing
                        reportParamMapList.Add("2119"); // ID for After Ageing core phases
                        resultList.Add(corePhases);

                        // Add other measurements
                        reportParamMapList.Add("2120"); resultList.Add(widthStrips); // AfterAgeingWidthStrip
                        reportParamMapList.Add("2121"); resultList.Add(pullingSpeeds); // AfterAgeingPullingSpeed
                        reportParamMapList.Add("2122"); resultList.Add(measuredValues); // AfterAgeingMeasured
                    }
                }

                // Join min/max values with commas
                string beforeAgeingMinStr = string.Join(",", beforeAgeingMinValues);
                string beforeAgeingMaxStr = string.Join(",", beforeAgeingMaxValues);
                string afterAgeingMinStr = string.Join(",", afterAgeingMinValues);
                string afterAgeingMaxStr = string.Join(",", afterAgeingMaxValues);

                // Add Standard values
                reportParamMapList.Add("3113"); resultList.Add(txtStandardWidth.Text); // StandardWidthNom
                reportParamMapList.Add("3114"); resultList.Add(txtStandardSpeed.Text); // StandardPullingSpeedNom
                reportParamMapList.Add("3115"); resultList.Add(txtStandardmeasured.Text); // StandardMeasured 
                reportParamMapList.Add("3116"); resultList.Add(txtStandardMin.Text); // StandardStrippabilityMin
                reportParamMapList.Add("3117"); resultList.Add(txtStandardMax.Text); // StandardStrippabilityMax

                // Ageing-Airflow parameters
                reportParamMapList.Add("3133"); resultList.Add(txtSpecimenAirflow.Text); // AgeingSpecimenAirflow
                reportParamMapList.Add("3134"); resultList.Add(txtCableAirflow.Text); // AgeingCompleteCableAirflow

                //Upper section
                reportParamMapList.Add("3118"); resultList.Add(cableType.Text);
                reportParamMapList.Add("3124"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("3119"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("3125"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("3120"); resultList.Add(voltage.Text);
                reportParamMapList.Add("3126"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("3127"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("3122"); resultList.Add(roomno.Text);
                reportParamMapList.Add("3128"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("3123"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("3121"); resultList.Add(Date.Text);
                reportParamMapList.Add("3129"); resultList.Add(equipmentno.Text);

                // Helper function to concatenate date and time values safely
                string ConcatenateValues(string dateTimeValue)
                {
                    // Return empty string if the input is null or empty
                    if (string.IsNullOrEmpty(dateTimeValue?.Trim()))
                        return string.Empty;
                    return dateTimeValue;
                }

                // Specimen dates and times
                reportParamMapList.Add("3131");
                resultList.Add(ConcatenateValues(txtSpecimenDateStart.Text + " " + txtSpecimenTimeStart.Text)); // AgeingSpecimenStartDate

                reportParamMapList.Add("3132");
                resultList.Add(ConcatenateValues(txtSpecimenDateFinish.Text + " " + txtSpecimenTimeFinish.Text)); // AgeingSpecimenFinishDate

                reportParamMapList.Add("2127");
                resultList.Add(ConcatenateValues(txtCableDateStart.Text + " " + txtCableTimeStart.Text)); // AgeingCompleteCableStartDate

                reportParamMapList.Add("2128");
                resultList.Add(ConcatenateValues(txtCableDateFinish.Text + " " + txtCableTimeFinish.Text)); // AgeingCompleteCableFinishDate

                // Conditioning times
                reportParamMapList.Add("2130");
                resultList.Add(ConcatenateValues(txtDesiccatorsDateStart.Text + " " + txtDesiccatorsTimeStart.Text)); // AgeingConditioningInDesciccatorsStartDate

                reportParamMapList.Add("3130");
                resultList.Add(ConcatenateValues(txtDesiccatorsDateFinish.Text + " " + txtDesiccatorsTimeFinish.Text)); // AgeingConditioningInDesciccatorsFinishDate

                reportParamMapList.Add("2131");
                resultList.Add(ConcatenateValues(txtConditioningDateStart.Text + " " + txtConditioningTimeStart.Text)); // AfterAgeingConditioningStartDate

                reportParamMapList.Add("2132");
                resultList.Add(ConcatenateValues(txtConditioningDateFinish.Text + " " + txtConditioningTimeFinish.Text)); // AfterAgeingConditioningFinishDate

                // Add Strippability Min and Max for Before and After Ageing
                reportParamMapList.Add("3135"); resultList.Add(beforeAgeingMinStr); // BeforeAgeingStrippabilityMin
                reportParamMapList.Add("3136"); resultList.Add(beforeAgeingMaxStr); // BeforeAgeingStrippabilityMax
                reportParamMapList.Add("2123"); resultList.Add(afterAgeingMinStr); // AfterAgeingStrippabilityMin
                reportParamMapList.Add("2124"); resultList.Add(afterAgeingMaxStr); // AfterAgeingStrippabilityMax

                // Convert lists to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                // Call the database save method
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);


                if (modified > 0)
                {
                    // Show success message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results and Report Generated successfully!');", true);
                    _cm.sampleStatusID = 27;//Test Result Submitted
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // GenerateTestReport(_cm.sampleId);
                    Response.Redirect($"GenerateTestRecord.aspx?s={_cm.sampleId}");
                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results.');", true);
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                // Log.Error(ex);
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('Error: {ex.Message}');", true);
            }
        }
      
        //Other
        private void UpdateMinMaxValues(DataTable dt)
        {
            // Group the data by test type and core phase
            var testTypes = dt.AsEnumerable().Select(row => row.Field<string>("ItemTest")).Distinct();
            var corePhases = dt.AsEnumerable().Select(row => row.Field<string>("CorePhase")).Distinct();

            foreach (var testType in testTypes)
            {
                foreach (var corePhase in corePhases)
                {
                    // Get all rows for this test type and core phase
                    var rows = dt.AsEnumerable().Where(row =>
                        row.Field<string>("ItemTest") == testType &&
                        row.Field<string>("CorePhase") == corePhase);

                    if (rows.Any())
                    {
                        // Find min and max measured values
                        decimal min = decimal.MaxValue;
                        decimal max = decimal.MinValue;

                        foreach (var row in rows)
                        {
                            if (!string.IsNullOrEmpty(row.Field<string>("Measured")))
                            {
                                decimal measured;
                                if (decimal.TryParse(row.Field<string>("Measured"), out measured))
                                {
                                    min = Math.Min(min, measured);
                                    max = Math.Max(max, measured);
                                }
                            }
                        }

                        // Update min and max values for each row in this group
                        foreach (var row in rows)
                        {
                            if (min != decimal.MaxValue)
                                row["MinStrippability"] = min.ToString("0.0");
                            if (max != decimal.MinValue)
                                row["MaxStrippability"] = max.ToString("0.0");
                        }
                    }
                }
            }
        }
        private void ClearInputFields()
        {
            txtWidthStrip.Text = "";
            txtPullingSpeed.Text = "";
            txtMeasured.Text = "";
        }
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
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
            testdate.Text = "";
            ddlTestType.SelectedIndex = 0;
            ddlCorePhase.SelectedIndex = 0;
            txtSpecimenNo.Text = "";
            txtWidthStrip.Text = "";
            txtPullingSpeed.Text = "";
            txtMeasured.Text = "";

            // Clear GridView
            gvStrippabilityTest.DataSource = null;
            gvStrippabilityTest.DataBind();

            // Standard Values
            txtStandardWidth.Text = "";
            txtStandardSpeed.Text = "";
            txtStandardmeasured.Text = "";
            txtStandardMin.Text = "";
            txtStandardMax.Text = "";

            // Ageing Specimen & Cable
            txtSpecimenAirflow.Text = "";
            txtCableAirflow.Text = "";
            txtSpecimenDateStart.Text = "";
            txtSpecimenDateFinish.Text = "";
            txtSpecimenTimeStart.Text = "";
            txtSpecimenTimeFinish.Text = "";
            txtCableDateStart.Text = "";
            txtCableDateFinish.Text = "";
            txtCableTimeStart.Text = "";
            txtCableTimeFinish.Text = "";

            // Conditioning
            txtDesiccatorsDateStart.Text = "";
            txtDesiccatorsDateFinish.Text = "";
            txtDesiccatorsTimeStart.Text = "";
            txtDesiccatorsTimeFinish.Text = "";
            txtConditioningDateStart.Text = "";
            txtConditioningDateFinish.Text = "";
            txtConditioningTimeStart.Text = "";
            txtConditioningTimeFinish.Text = "";
        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);
            allTextBoxes.Add(testdate);
            allTextBoxes.Add(txtSpecimenNo);
            allTextBoxes.Add(txtWidthStrip);
            allTextBoxes.Add(txtPullingSpeed);
            allTextBoxes.Add(txtMeasured);
            allTextBoxes.Add(txtStandardWidth);
            allTextBoxes.Add(txtStandardSpeed);
            allTextBoxes.Add(txtStandardmeasured);
            allTextBoxes.Add(txtStandardMin);
            allTextBoxes.Add(txtStandardMax);
            allTextBoxes.Add(txtSpecimenAirflow);
            allTextBoxes.Add(txtCableAirflow);
            allTextBoxes.Add(txtSpecimenDateStart);
            allTextBoxes.Add(txtSpecimenDateFinish);
            allTextBoxes.Add(txtSpecimenTimeStart);
            allTextBoxes.Add(txtSpecimenTimeFinish);
            allTextBoxes.Add(txtCableDateStart);
            allTextBoxes.Add(txtCableDateFinish);
            allTextBoxes.Add(txtCableTimeStart);
            allTextBoxes.Add(txtCableTimeFinish);
            allTextBoxes.Add(txtDesiccatorsDateStart);
            allTextBoxes.Add(txtDesiccatorsDateFinish);
            allTextBoxes.Add(txtDesiccatorsTimeStart);
            allTextBoxes.Add(txtDesiccatorsTimeFinish);
            allTextBoxes.Add(txtConditioningDateStart);
            allTextBoxes.Add(txtConditioningDateFinish);
            allTextBoxes.Add(txtConditioningTimeStart);
            allTextBoxes.Add(txtConditioningTimeFinish);

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }


    }
}

//protected void btnSubmit_Click1(object sender, EventArgs e)
//{
//    try
//    {
//        // Get the DataTable from ViewState
//        DataTable dt = ViewState["TestResults"] as DataTable;

//        if (dt == null || dt.Rows.Count == 0)
//        {
//            // Show error message if no data to save
//            ScriptManager.RegisterStartupScript(this, this.GetType(),
//                "showError", "alert('No data to save. Please add test results first.');", true);
//            return;
//        }

//        // Create DAL object
//        if (_cm == null) _cm = new DAL.CM();
//        if (Request.QueryString["s"] != null) { _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower()); }
//        _cm.sampleIDSeries = "PTQ/08/03/2025";

//        // Organize data by test types (Before aging and After aging)
//        Dictionary<string, List<DataRow>> testTypeData = new Dictionary<string, List<DataRow>>();

//        foreach (DataRow row in dt.Rows)
//        {
//            string testType = row["ItemTest"].ToString();

//            if (!testTypeData.ContainsKey(testType))
//            {
//                testTypeData[testType] = new List<DataRow>();
//            }

//            testTypeData[testType].Add(row);
//        }

//        // Prepare arrays for the AddTestResultGlobal method
//        List<string> reportParamMapList = new List<string>();
//        List<string> resultList = new List<string>();

//        // Process each test type
//        foreach (var testType in testTypeData.Keys)
//        {
//            var rows = testTypeData[testType];
//            string testDate = rows[0]["Date"].ToString();

//            // Get all core phases in order of specimen numbers
//            string corePhases = string.Join(",", rows.Select(r => r["CorePhase"].ToString()));

//            // Get all width strips in order
//            string widthStrips = string.Join(",", rows.Select(r => r["WidthStrip"].ToString()));

//            // Get all pulling speeds in order
//            string pullingSpeeds = string.Join(",", rows.Select(r => r["PullingSpeed"].ToString()));

//            // Get all measured values in order
//            string measuredValues = string.Join(",", rows.Select(r => r["Measured"].ToString()));

//            // Add test date to parameters
//            if (testType.Contains("Before"))
//            {
//                reportParamMapList.Add("2125"); // BeforeAgeingDate param ID
//                resultList.Add(testDate);

//                // Add core phases for Before Ageing
//                reportParamMapList.Add("2113"); // Assuming this is the ID for Before Ageing core phases
//                resultList.Add(corePhases);

//                // Add other measurements
//                reportParamMapList.Add("2114"); resultList.Add(widthStrips); // BeforeAgeingWidthStrip
//                reportParamMapList.Add("2115"); resultList.Add(pullingSpeeds); // BeforeAgeingPullingSpeed
//                reportParamMapList.Add("2116"); resultList.Add(measuredValues); // BeforeAgeingMeasured
//            }
//            else if (testType.Contains("After"))
//            {
//                reportParamMapList.Add("2126"); // AfterAgeingDate param ID
//                resultList.Add(testDate);

//                // Add core phases for After Ageing
//                reportParamMapList.Add("2119"); // Assuming this is the ID for After Ageing core phases
//                resultList.Add(corePhases);

//                // Add other measurements
//                reportParamMapList.Add("2120"); resultList.Add(widthStrips); // AfterAgeingWidthStrip
//                reportParamMapList.Add("2121"); resultList.Add(pullingSpeeds); // AfterAgeingPullingSpeed
//                reportParamMapList.Add("2122"); resultList.Add(measuredValues); // AfterAgeingMeasured
//            }
//        }

//        // Convert lists to arrays
//        string[] reportParamMap = reportParamMapList.ToArray();
//        string[] result = resultList.ToArray();

//        // Call the database save method
//        int modified = _cm.AddTestResultGlobal(_cm, reportParamMap, result);

//        if (modified > 0)
//        {
//            // Show success message
//            ScriptManager.RegisterStartupScript(this, this.GetType(),
//                "showSuccess", "alert('Test results saved successfully!');", true);
//        }
//        else
//        {
//            // Show error message
//            ScriptManager.RegisterStartupScript(this, this.GetType(),
//                "showError", "alert('Failed to save test results.');", true);
//        }
//    }
//    catch (Exception ex)
//    {
//        // Log error and show message
//        // Log.Error(ex);
//        ScriptManager.RegisterStartupScript(this, this.GetType(),
//            "showError", $"alert('Error: {ex.Message}');", true);
//    }
//}


//protected void btnSubmit_Click(object sender, EventArgs e)
//{

//    try
//    {
//        // Get the DataTable from ViewState
//        DataTable dt = ViewState["TestResults"] as DataTable;

//        if (dt == null || dt.Rows.Count == 0)
//        {
//            // Show error message if no data to save
//            ScriptManager.RegisterStartupScript(this, this.GetType(),
//                "showError", "alert('No data to save. Please add test results first.');", true);
//            return;
//        }

//        // Create DAL object
//        if (_cm == null) _cm = new DAL.CM();
//        if (Request.QueryString["s"] != null) { _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower()); }
//        _cm.sampleIDSeries = "PTQ/08/03/2025";

//        // Organize data by test types (Before aging and After aging)
//        Dictionary<string, List<Dictionary<string, string>>> testTypeData = new Dictionary<string, List<Dictionary<string, string>>>();

//        foreach (DataRow row in dt.Rows)
//        {
//            string testType = row["ItemTest"].ToString();
//            string corePhase = row["CorePhase"].ToString();

//            if (!testTypeData.ContainsKey(testType))
//            {
//                testTypeData[testType] = new List<Dictionary<string, string>>();
//            }

//            Dictionary<string, string> rowData = new Dictionary<string, string>();
//            rowData["Date"] = row["Date"].ToString();
//            rowData["CorePhase"] = corePhase;
//            rowData["WidthStrip"] = row["WidthStrip"].ToString();
//            rowData["PullingSpeed"] = row["PullingSpeed"].ToString();
//            rowData["Measured"] = row["Measured"].ToString();
//            rowData["MinStrippability"] = row["MinStrippability"].ToString();
//            rowData["MaxStrippability"] = row["MaxStrippability"].ToString();

//            testTypeData[testType].Add(rowData);
//        }

//        // Prepare arrays for the AddTestResultGlobal method
//        List<string> reportParamMapList = new List<string>();
//        List<string> resultList = new List<string>();

//        // Add sample info parameters if needed
//        // reportParamMapList.Add("SomeParamID"); 
//        // resultList.Add("SomeValue");

//        // For each test type (e.g., "Before ageing", "After ageing complete cable 100°C/168 h")
//        foreach (var testType in testTypeData.Keys)
//        {
//            string testDate = testTypeData[testType][0]["Date"];

//            // Add test date to parameters
//            if (testType.Contains("Before"))
//            {
//                reportParamMapList.Add("2125"); // BeforeAgeingDate param ID
//                resultList.Add(testDate);
//            }
//            else if (testType.Contains("After"))
//            {
//                reportParamMapList.Add("2126"); // AfterAgeingDate param ID
//                resultList.Add(testDate);
//            }

//            // Group data by core phases
//            Dictionary<string, List<Dictionary<string, string>>> corePhaseData =
//                testTypeData[testType].GroupBy(r => r["CorePhase"])
//                                     .ToDictionary(g => g.Key, g => g.ToList());

//            // Extract measurements for Red, Blue, Yellow phases
//            string widthStrips = "";
//            string pullingSpeeds = "";
//            string measuredValues = "";

//            // Process each core phase
//            foreach (string corePhase in new[] { "Red", "Blue", "Yellow" })
//            {
//                if (corePhaseData.ContainsKey(corePhase))
//                {
//                    // Get average or representative values for the core phase
//                    var phaseData = corePhaseData[corePhase];
//                    string width = phaseData[0]["WidthStrip"]; // Use first value or calculate average
//                    string pulling = phaseData[0]["PullingSpeed"]; // Use first value or calculate average

//                    // Use Min/Max as the measured values if available
//                    string measured = phaseData[0]["MinStrippability"] + "-" + phaseData[0]["MaxStrippability"];

//                    // Append values with commas
//                    widthStrips += (widthStrips.Length > 0 ? "," : "") + width;
//                    pullingSpeeds += (pullingSpeeds.Length > 0 ? "," : "") + pulling;
//                    measuredValues += (measuredValues.Length > 0 ? "," : "") + measured;
//                }
//                else
//                {
//                    // If core phase doesn't exist, use empty values
//                    widthStrips += (widthStrips.Length > 0 ? "," : "") + "0";
//                    pullingSpeeds += (pullingSpeeds.Length > 0 ? "," : "") + "0";
//                    measuredValues += (measuredValues.Length > 0 ? "," : "") + "0-0";
//                }
//            }

//            // Add parameters based on test type
//            if (testType.Contains("Before"))
//            {
//                reportParamMapList.Add("2114"); resultList.Add(widthStrips); // BeforeAgeingWidthStrip
//                reportParamMapList.Add("2115"); resultList.Add(pullingSpeeds); // BeforeAgeingPullingSpeed
//                reportParamMapList.Add("2116"); resultList.Add(measuredValues); // BeforeAgeingMeasured
//            }
//            else if (testType.Contains("After"))
//            {
//                reportParamMapList.Add("2120"); resultList.Add(widthStrips); // AfterAgeingWidthStrip
//                reportParamMapList.Add("2121"); resultList.Add(pullingSpeeds); // AfterAgeingPullingSpeed
//                reportParamMapList.Add("2122"); resultList.Add(measuredValues); // AfterAgeingMeasured
//            }
//        }

//        // Convert lists to arrays
//        string[] reportParamMap = reportParamMapList.ToArray();
//        string[] result = resultList.ToArray();

//        // Call the database save method
//        int modified = _cm.AddTestResultGlobal(_cm, reportParamMap, result);

//        if (modified > 0)
//        {
//            // Show success message
//            ScriptManager.RegisterStartupScript(this, this.GetType(),
//                "showSuccess", "alert('Test results saved successfully!');", true);
//        }
//        else
//        {
//            // Show error message
//            ScriptManager.RegisterStartupScript(this, this.GetType(),
//                "showError", "alert('Failed to save test results.');", true);
//        }
//    }
//    catch (Exception ex)
//    {
//        // Log error and show message
//        // Log.Error(ex);
//        ScriptManager.RegisterStartupScript(this, this.GetType(),
//            "showError", $"alert('Error: {ex.Message}');", true);
//    }
//}