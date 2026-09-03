using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class ResistanceAgainstAcidAndAlkalineSoluiton : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
        private void PrefillUpperSection(DataTable existingData)
        {
            // Second set of header mappings
            SetTextBoxText(cableType, "17368");   // Type
            SetTextBoxText(cableSize, "17369");   // Size
            SetTextBoxText(voltage, "17370");   // Voltage
            SetTextBoxText(Date, "17371");   // Date
            SetTextBoxText(roomno, "17372");   // RoomNo
            SetTextBoxText(roomtemp, "17373");   // RoomTemp
            SetTextBoxText(refSpec, "17374");   // RefSpec
            SetTextBoxText(testMethodNo, "17375");   // TestMethodNo
            SetTextBoxText(jobOrderNo, "17376");   // JobOrderNo
            SetTextBoxText(worksheetno, "17377");   // WorkSheetNo
            SetTextBoxText(roomhumidity, "17378");   // RoomHumidity
            SetTextBoxText(equipmentno, "17379");   // EquipmentNo





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
        private DataTable CreateTestResultsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("no", typeof(string));
            dt.Columns.Add("date", typeof(DateTime));
            dt.Columns.Add("itemtest", typeof(string));
            dt.Columns.Add("ItemSubtype", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(int));
            dt.Columns.Add("InputthicknessInputs", typeof(string));
            dt.Columns.Add("InputthicknessInputAvg", typeof(string));
            dt.Columns.Add("InputdiameterInputs", typeof(string));
            dt.Columns.Add("InputdiameterInputAvg", typeof(string));
            return dt;
        }

        private void PrefillGridData(DataTable existingData)
        {
            try
            {
                // Get the saved grid data from database
                var numberData = GetParameterValue(existingData, "17360");    // Number
                var dateData = GetParameterValue(existingData, "17361");      // Date
                var itemTestData = GetParameterValue(existingData, "17362");  // ItemTest
                var sampleData = GetParameterValue(existingData, "17363");    // Sample
                var specimenData = GetParameterValue(existingData, "17587");  // SpecimenNo
                var thicknessData = GetParameterValue(existingData, "17364"); // Thickness
                var thicknessAvgData = GetParameterValue(existingData, "17365"); // ThicknessAvg
                var widthData = GetParameterValue(existingData, "17366");     // Width
                var widthAvgData = GetParameterValue(existingData, "17367");  // WidthAvg

                // If any grid data exists, reconstruct the DataTable
                if (!string.IsNullOrEmpty(dateData))
                {
                    DataTable dt = CreateTestResultsDataTable(); // CREATE EMPTY TABLE

                    // Split by pipe to get different groups
                    var numberGroups = numberData?.Split('|') ?? new string[0];
                    var dateGroups = dateData.Split('|');
                    var itemTestGroups = itemTestData?.Split('|') ?? new string[0];
                    var sampleGroups = sampleData?.Split('|') ?? new string[0];
                    var specimenGroups = specimenData?.Split('|') ?? new string[0];
                    var thicknessGroups = thicknessData?.Split('|') ?? new string[0];
                    var thicknessAvgGroups = thicknessAvgData?.Split('|') ?? new string[0];
                    var widthGroups = widthData?.Split('|') ?? new string[0];
                    var widthAvgGroups = widthAvgData?.Split('|') ?? new string[0];

                    for (int groupIndex = 0; groupIndex < dateGroups.Length; groupIndex++)
                    {
                        var currentDate = dateGroups[groupIndex];
                        var currentItemTest = groupIndex < itemTestGroups.Length ? itemTestGroups[groupIndex] : "";

                        // Split by semicolon to get subgroups
                        var numberSubGroups = groupIndex < numberGroups.Length ? numberGroups[groupIndex].Split(';') : new string[0];
                        var sampleSubGroups = groupIndex < sampleGroups.Length ? sampleGroups[groupIndex].Split(';') : new string[0];
                        var specimenSubGroups = groupIndex < specimenGroups.Length ? specimenGroups[groupIndex].Split(';') : new string[0];
                        var thicknessSubGroups = groupIndex < thicknessGroups.Length ? thicknessGroups[groupIndex].Split(';') : new string[0];
                        var thicknessAvgSubGroups = groupIndex < thicknessAvgGroups.Length ? thicknessAvgGroups[groupIndex].Split(';') : new string[0];
                        var widthSubGroups = groupIndex < widthGroups.Length ? widthGroups[groupIndex].Split(';') : new string[0];
                        var widthAvgSubGroups = groupIndex < widthAvgGroups.Length ? widthAvgGroups[groupIndex].Split(';') : new string[0];

                        // Process each subgroup
                        for (int subGroupIndex = 0; subGroupIndex < sampleSubGroups.Length; subGroupIndex++)
                        {
                            var numberValues = subGroupIndex < numberSubGroups.Length ? numberSubGroups[subGroupIndex].Split(',') : new string[0];
                            var sampleValues = subGroupIndex < sampleSubGroups.Length ? sampleSubGroups[subGroupIndex].Split(',') : new string[0];
                            var specimenValues = subGroupIndex < specimenSubGroups.Length ? specimenSubGroups[subGroupIndex].Split(',') : new string[0];

                            // Extract content inside parentheses
                            var thicknessValues = subGroupIndex < thicknessSubGroups.Length ? ExtractParenthesesContent(thicknessSubGroups[subGroupIndex]) : new string[0];
                            var thicknessAvgValues = subGroupIndex < thicknessAvgSubGroups.Length ? ExtractParenthesesContent(thicknessAvgSubGroups[subGroupIndex]) : new string[0];
                            var widthValues = subGroupIndex < widthSubGroups.Length ? ExtractParenthesesContent(widthSubGroups[subGroupIndex]) : new string[0];
                            var widthAvgValues = subGroupIndex < widthAvgSubGroups.Length ? ExtractParenthesesContent(widthAvgSubGroups[subGroupIndex]) : new string[0];

                            // Determine the number of rows in this subgroup
                            int rowCount = Math.Max(sampleValues.Length, specimenValues.Length);

                            for (int valueIndex = 0; valueIndex < rowCount; valueIndex++)
                            {
                                DataRow row = dt.NewRow();

                                // Only populate No for the FIRST row of each number group
                                if (valueIndex == 0 && subGroupIndex == 0) // First row of first subgroup
                                    row["no"] = numberValues.Length > 0 ? numberValues[0] : "";
                                else
                                    row["no"] = ""; // Empty for merged rows

                                DateTime parsedDate;
                                if (DateTime.TryParse(currentDate, out parsedDate))
                                    row["date"] = parsedDate;
                                row["itemtest"] = currentItemTest;
                                row["ItemSubtype"] = valueIndex < sampleValues.Length ? sampleValues[valueIndex] : "";
                                row["SpecimenNo"] = valueIndex < specimenValues.Length ? specimenValues[valueIndex] : "";
                                row["InputthicknessInputs"] = valueIndex < thicknessValues.Length ? thicknessValues[valueIndex] : "";
                                row["InputthicknessInputAvg"] = valueIndex < thicknessAvgValues.Length ? thicknessAvgValues[valueIndex] : "";
                                row["InputdiameterInputs"] = valueIndex < widthValues.Length ? widthValues[valueIndex] : "";
                                row["InputdiameterInputAvg"] = valueIndex < widthAvgValues.Length ? widthAvgValues[valueIndex] : "";

                                dt.Rows.Add(row); // ADD THE POPULATED ROW TO DATATABLE
                            }
                        }
                    }

                    // Bind to grid and store in ViewState
                    gvPressureTest.DataSource = dt;
                    gvPressureTest.DataBind();
                    ViewState["TestResultsData"] = dt;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error prefilling grid data: {ex.Message}');", true);
            }
        }
        private string[] ExtractParenthesesContent(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];

            List<string> result = new List<string>();
            int pos = 0;

            while (pos < input.Length)
            {
                int start = input.IndexOf('(', pos);
                if (start == -1) break;

                int end = input.IndexOf(')', start);
                if (end == -1) break;

                string content = input.Substring(start + 1, end - start - 1);
                result.Add(content.Trim());
                pos = end + 1;
            }

            return result.ToArray();
        }
        private string GetParameterValue(DataTable existingData, string parameterId)
        {
            var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
            return row != null ? row["Result"].ToString() : "";
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
            CalculateAndDisplayStats(txtthicknessInputs.Text, txtthicknessInputAvg);
        }
        protected void DiameterInputs_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplayStats(txtdiameterInputs.Text, txtdiameterInputAvg);
        }
        private decimal TryParseDecimal(string input)
        {
            return decimal.TryParse(input, out decimal result) ? result : -1;
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            int specimenNo = 0;

            // Validate 'no'
            if (string.IsNullOrWhiteSpace(txtno.Text) || !int.TryParse(txtno.Text.Trim(), out _))
            {
                invalidControls.Add(txtno);
                hasError = true;
            }

            // Validate 'date'
            if (string.IsNullOrWhiteSpace(txtdate.Text) || !DateTime.TryParse(txtdate.Text.Trim(), out _))
            {
                invalidControls.Add(txtdate);
                hasError = true;
            }

            // Validate 'itemtest'
            if (string.IsNullOrWhiteSpace(txtitemtest.Text))
            {
                invalidControls.Add(txtitemtest);
                hasError = true;
            }

            // Validate 'SpecimenNo'
            if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out specimenNo))
            {
                invalidControls.Add(txtSpecimenNo);
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
            TextBox[] individualInputs = { txtthicknessInputs, txtdiameterInputs };

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

            // Check duplicates (unchanged)
            DataTable dt = ViewState["TestResultsData"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("date", typeof(DateTime));
                dt.Columns.Add("no", typeof(int));
                dt.Columns.Add("itemtest", typeof(string));
                dt.Columns.Add("ItemSubtype", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("InputthicknessInputs", typeof(string));
                dt.Columns.Add("InputthicknessInputAvg", typeof(string));
                dt.Columns.Add("InputdiameterInputs", typeof(string));
                dt.Columns.Add("InputdiameterInputAvg", typeof(string));
            }

            DateTime inputDate = DateTime.Parse(txtdate.Text.Trim());
            int inputNo = int.Parse(txtno.Text.Trim());
            string inputItemTest = txtitemtest.Text.Trim();
            string inputItemSubtype = txtitemsubtype.Text.Trim();

            bool isDuplicate = dt.AsEnumerable().Any(row =>
                row.Field<DateTime>("date") == inputDate &&
                row.Field<string>("itemtest") == inputItemTest &&
                row.Field<string>("ItemSubtype") == inputItemSubtype &&
                row.Field<int>("SpecimenNo") == specimenNo);

            if (isDuplicate)
            {
                ShowMessage("Duplicate specimen data is not allowed for same date, item test, and subtype.");
                return;
            }

            // Add new row
            DataRow dr = dt.NewRow();
            dr["no"] = inputNo;
            dr["date"] = inputDate;
            dr["itemtest"] = inputItemTest;
            dr["ItemSubtype"] = inputItemSubtype;
            dr["SpecimenNo"] = specimenNo;
            dr["InputthicknessInputs"] = txtthicknessInputs.Text.Trim();
            dr["InputthicknessInputAvg"] = txtthicknessInputAvg.Text.Trim();
            dr["InputdiameterInputs"] = txtdiameterInputs.Text.Trim();
            dr["InputdiameterInputAvg"] = txtdiameterInputAvg.Text.Trim();

            dt.Rows.Add(dr);
            ViewState["TestResultsData"] = dt;

            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();

            txtSpecimenNo.Text = (specimenNo + 1).ToString();
            ClearInputFields();
        }


        protected void gvPressureTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            GridView grid = (GridView)sender;

            string GetCellText(GridViewRow row, int cellIndex)
            {
                if (row.Cells[cellIndex].Controls.Count > 0)
                {
                    foreach (Control ctl in row.Cells[cellIndex].Controls)
                    {
                        if (ctl is Label lbl)
                            return lbl.Text.Trim();
                        else if (ctl is Literal lit)
                            return lit.Text.Trim();
                    }
                }
                return HttpUtility.HtmlDecode(row.Cells[cellIndex].Text).Replace("&nbsp;", "").Trim();
            }

            if (e.Row.RowIndex > 0)
            {
                // Get current row values
                string currNo = GetCellText(e.Row, 0);
                string currDate = GetCellText(e.Row, 1);
                string currItemTest = GetCellText(e.Row, 2);
                string currItemSubtype = GetCellText(e.Row, 3);

                // 1. Merge No column - if current row has empty No, merge with previous visible No
                GridViewRow noMasterRow = null;
                for (int i = e.Row.RowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[0].Visible)
                    {
                        noMasterRow = grid.Rows[i];
                        break;
                    }
                }

                // Merge if current No is empty (indicating it should merge) OR if values match
                if (noMasterRow != null &&
                    (string.IsNullOrEmpty(currNo) ||
                     (currNo.ToLower() == GetCellText(noMasterRow, 0).ToLower() && !string.IsNullOrEmpty(currNo))))
                {
                    if (noMasterRow.Cells[0].RowSpan <= 1)
                        noMasterRow.Cells[0].RowSpan = 2;
                    else
                        noMasterRow.Cells[0].RowSpan += 1;
                    e.Row.Cells[0].Visible = false;
                }

                // 2. Merge Date column - if current date is empty or matches
                GridViewRow dateMasterRow = null;
                for (int i = e.Row.RowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[1].Visible)
                    {
                        dateMasterRow = grid.Rows[i];
                        break;
                    }
                }

                if (dateMasterRow != null &&
                    (string.IsNullOrEmpty(currDate) || currDate == "01/01/0001" || // Handle empty DateTime
                     (currDate.ToLower() == GetCellText(dateMasterRow, 1).ToLower() && !string.IsNullOrEmpty(currDate))))
                {
                    if (dateMasterRow.Cells[1].RowSpan <= 1)
                        dateMasterRow.Cells[1].RowSpan = 2;
                    else
                        dateMasterRow.Cells[1].RowSpan += 1;
                    e.Row.Cells[1].Visible = false;
                }

                // 3. Merge ItemTest - if current ItemTest is empty or matches
                GridViewRow testMasterRow = null;
                for (int i = e.Row.RowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[2].Visible)
                    {
                        testMasterRow = grid.Rows[i];
                        break;
                    }
                }

                if (testMasterRow != null &&
                    (string.IsNullOrEmpty(currItemTest) ||
                     (currItemTest.ToLower() == GetCellText(testMasterRow, 2).ToLower() && !string.IsNullOrEmpty(currItemTest))))
                {
                    if (testMasterRow.Cells[2].RowSpan <= 1)
                        testMasterRow.Cells[2].RowSpan = 2;
                    else
                        testMasterRow.Cells[2].RowSpan += 1;
                    e.Row.Cells[2].Visible = false;
                }

                // 4. Merge ItemSubtype - check if all parent fields should merge and current subtype matches
                GridViewRow subtypeMasterRow = null;
                for (int i = e.Row.RowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[3].Visible)
                    {
                        subtypeMasterRow = grid.Rows[i];
                        break;
                    }
                }

                if (subtypeMasterRow != null)
                {
                    string masterNo = GetCellText(subtypeMasterRow, 0);
                    string masterDate = GetCellText(subtypeMasterRow, 1);
                    string masterItemTest = GetCellText(subtypeMasterRow, 2);
                    string masterItemSubtype = GetCellText(subtypeMasterRow, 3);

                    // Check if this row should merge with the master row
                    bool shouldMerge =
                        (string.IsNullOrEmpty(currNo) || currNo.ToLower() == masterNo.ToLower()) &&
                        (string.IsNullOrEmpty(currDate) || currDate == "01/01/0001" || currDate.ToLower() == masterDate.ToLower()) &&
                        (string.IsNullOrEmpty(currItemTest) || currItemTest.ToLower() == masterItemTest.ToLower()) &&
                        (!string.IsNullOrEmpty(currItemSubtype) && currItemSubtype.ToLower() == masterItemSubtype.ToLower());

                    if (shouldMerge)
                    {
                        if (subtypeMasterRow.Cells[3].RowSpan <= 1)
                            subtypeMasterRow.Cells[3].RowSpan = 2;
                        else
                            subtypeMasterRow.Cells[3].RowSpan += 1;
                        e.Row.Cells[3].Visible = false;
                    }
                }
            }
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
                    ViewState["TestResultsData"] = dt;
                    gvPressureTest.DataSource = dt;
                    gvPressureTest.DataBind();


                    // Show confirmation message
                    ShowMessage("Record deleted successfully.");
                }
            }
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


            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
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
        private string GetArrayValue(string[] array, int index)
        {
            return index < array.Length ? array[index] : array[0];
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
                if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"]);
                if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                // Group by Date + ItemTest combination
                var dateItemTestGroups = dt.AsEnumerable()
                    .GroupBy(r => new {
                        Date = r["date"] != DBNull.Value ? Convert.ToDateTime(r["date"]).ToString("yyyy-MM-dd") : "",
                        ItemTest = r["itemtest"]?.ToString() ?? ""
                    })
                    .OrderBy(g => g.Key.Date)
                    .ThenBy(g => g.Key.ItemTest)
                    .ToList();

                // Lists to store the grouped data
                List<string> numberGroups = new List<string>();             // <-- "Number" field (grouped one level up)
                List<string> dateGroups = new List<string>();
                List<string> itemTestGroups = new List<string>();
                List<string> itemSubtypeGroups = new List<string>();
                List<string> thicknessInputGroups = new List<string>();
                List<string> thicknessAvgGroups = new List<string>();
                List<string> diameterInputGroups = new List<string>();
                List<string> diameterAvgGroups = new List<string>();
                List<string> specimenNoGroups = new List<string>();

                foreach (var group in dateItemTestGroups)
                {
                    // Add Date and ItemTest
                    dateGroups.Add(group.Key.Date);
                    itemTestGroups.Add(group.Key.ItemTest);

                    var groupRows = group.ToList();

                    var firstNumberValue = groupRows.FirstOrDefault()?["No"]?.ToString() ?? "";
                    numberGroups.Add(firstNumberValue);


                    //// "No" field — collected at Date+ItemTest level (one level up from ItemSubtype)
                    //var numberValues = groupRows.Select(r => r["No"]?.ToString() ?? "").ToList();
                    //numberGroups.Add(string.Join(",", numberValues));

                    // Group all fields by ItemSubtype within this Date+ItemTest group
                    var subtypeSubGroups = groupRows.GroupBy(r => r["ItemSubtype"]?.ToString() ?? "").ToList();

                    // ItemSubtype field
                    var itemSubtypeGroupStrings = new List<string>();
                    foreach (var subtypeSubGroup in subtypeSubGroups)
                    {
                        var subtypeValue = subtypeSubGroup.Key;
                        var count = subtypeSubGroup.Count();
                        itemSubtypeGroupStrings.Add(string.Join(",", Enumerable.Repeat(subtypeValue, count)));
                    }
                    itemSubtypeGroups.Add(string.Join(";", itemSubtypeGroupStrings));

                    // Other fields grouped by subtype
                    var thicknessInputGroupStrings = new List<string>();
                    var thicknessAvgGroupStrings = new List<string>();
                    var diameterInputGroupStrings = new List<string>();
                    var diameterAvgGroupStrings = new List<string>();
                    var specimenNoGroupStrings = new List<string>();

                    foreach (var subtypeSubGroup in subtypeSubGroups)
                    {
                        var subtypeRows = subtypeSubGroup.ToList();

                        thicknessInputGroupStrings.Add(string.Join(",", subtypeRows.Select(r => $"({r["InputthicknessInputs"]?.ToString() ?? ""})")));
                        thicknessAvgGroupStrings.Add(string.Join(",", subtypeRows.Select(r => $"({r["InputthicknessInputAvg"]?.ToString() ?? ""})")));
                        diameterInputGroupStrings.Add(string.Join(",", subtypeRows.Select(r => $"({r["InputdiameterInputs"]?.ToString() ?? ""})")));
                        diameterAvgGroupStrings.Add(string.Join(",", subtypeRows.Select(r => $"({r["InputdiameterInputAvg"]?.ToString() ?? ""})")));
                        specimenNoGroupStrings.Add(string.Join(",", subtypeRows.Select(r => r["SpecimenNo"]?.ToString() ?? "")));
                    }

                    // Join all subtype groups with semicolon
                    thicknessInputGroups.Add(string.Join(";", thicknessInputGroupStrings));
                    thicknessAvgGroups.Add(string.Join(";", thicknessAvgGroupStrings));
                    diameterInputGroups.Add(string.Join(";", diameterInputGroupStrings));
                    diameterAvgGroups.Add(string.Join(";", diameterAvgGroupStrings));
                    specimenNoGroups.Add(string.Join(";", specimenNoGroupStrings));
                }

                // Prepare parameters for saving
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                reportParamMapList.Add("17360"); resultList.Add(string.Join("|", numberGroups));            // "Number" — now added at higher level
                reportParamMapList.Add("17361"); resultList.Add(string.Join("|", dateGroups));              // Date
                reportParamMapList.Add("17362"); resultList.Add(string.Join("|", itemTestGroups));          // ItemTest
                reportParamMapList.Add("17363"); resultList.Add(string.Join("|", itemSubtypeGroups));       // ItemSubtype
                reportParamMapList.Add("17364"); resultList.Add(string.Join("|", thicknessInputGroups));    // Thickness Input
                reportParamMapList.Add("17365"); resultList.Add(string.Join("|", thicknessAvgGroups));      // Thickness Avg
                reportParamMapList.Add("17366"); resultList.Add(string.Join("|", diameterInputGroups));     // Diameter Input
                reportParamMapList.Add("17367"); resultList.Add(string.Join("|", diameterAvgGroups));       // Diameter Avg
                reportParamMapList.Add("17587"); resultList.Add(string.Join("|", specimenNoGroups));        // Specimen No

                // Saving data - use same IDs as loading
                reportParamMapList.Add("17368"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17369"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17370"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17371"); resultList.Add(Date.Text);
                reportParamMapList.Add("17372"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17373"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17374"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17375"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17376"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17377"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17378"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17379"); resultList.Add(equipmentno.Text);
                // Save to DB
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

                    Response.Redirect($"GenerateTestRecord_ResistanceAgainstAcidAndAlkalineSolution.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

                // Validate no as int
                if (string.IsNullOrWhiteSpace(txtno.Text) || !int.TryParse(txtno.Text.Trim(), out _))
                {
                    invalidControls.Add(txtno);

                }

                // Validate date as DateTime
                if (string.IsNullOrWhiteSpace(txtdate.Text) || !DateTime.TryParse(txtdate.Text.Trim(), out _))
                {
                    invalidControls.Add(txtdate);

                }

                // Validate itemtest as non-empty string (no int parse)
                if (string.IsNullOrWhiteSpace(txtitemtest.Text))
                {
                    invalidControls.Add(txtitemtest);

                }

                // Validate SpecimenNo as int
                if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out
                    _))
                {
                    invalidControls.Add(txtSpecimenNo);

                }


                // Validate your new input textboxes here (thickness and diameter)
                TextBox[] newTestInputs = {
            txtthicknessInputs,
            txtdiameterInputs,
            txtthicknessInputAvg, txtdiameterInputAvg,

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


            ResetControlBorders();

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

                GridViewRow headerRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);

                headerRow.Cells.Add(CreateHeaderCell("No", 1, 1));
                headerRow.Cells.Add(CreateHeaderCell("Date", 1, 1));
                headerRow.Cells.Add(CreateHeaderCell("Item Test", 1, 1));
                headerRow.Cells.Add(CreateHeaderCell("ItemSubtype", 1, 1));
                headerRow.Cells.Add(CreateHeaderCell("Specimen No.", 1, 1));

                headerRow.Cells.Add(CreateHeaderCell("Thickness (inputs)", 1, 1));
                headerRow.Cells.Add(CreateHeaderCell("Min Max Avg", 1, 1));

                headerRow.Cells.Add(CreateHeaderCell("Diameter/Width (inputs)", 1, 1));
                headerRow.Cells.Add(CreateHeaderCell("Min Max Avg", 1, 1));

                headerRow.Cells.Add(CreateHeaderCell("Actions", 1, 1));

                gv.Controls[0].Controls.AddAt(0, headerRow);
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


            txtno.Text = "";
            txtitemtest.Text = ""; ;
            txtdate.Text = "";
            txtthicknessInputs.Text = "";
            txtthicknessInputAvg.Text = "";

            txtdiameterInputs.Text = "";
            txtdiameterInputAvg.Text = "";

            // Clear GridView
            gvPressureTest.DataSource = null;
            gvPressureTest.DataBind();
                


            ViewState["TestResultsData"] = null;



        }
        private void ClearInputFields()

        {

            //txtno.Text = "";
            // txtitemtest.Text = ""; ;
            //  txtdate.Text = "";
            // txtitemsubtype.Text = "";
            txtthicknessInputs.Text = "";
            txtthicknessInputAvg.Text = "";

            txtdiameterInputs.Text = "";
            txtdiameterInputAvg.Text = "";

            // txtMedian.Text = "";
        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();


            //  allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);

            allTextBoxes.Add(txtthicknessInputAvg);
            allTextBoxes.Add(txtdiameterInputAvg);


            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{safeMessage}');", true);
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}