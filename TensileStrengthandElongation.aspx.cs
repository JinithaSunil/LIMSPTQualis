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
using DataTable = System.Data.DataTable;

namespace LIMS
{
    public partial class TensileStrengthandElongation : System.Web.UI.Page
    {

        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

                    //upper section
                    test.Text = "Test 1";
                    drumno.Text = "D-2";
                    length.Text = "10";
                    roomno.Text = "SHIELDING";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/15/EL/34";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }
                InitializeGrid();
                PopulateCoreDropdown();

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();
            }
        }
        private void PopulateCoreDropdown()
        {
            try
            {
                DataTable coreData = _cm.Get_Core();
                ddlCorePhase.Items.Clear();
                ddlCorePhase.Items.Insert(0, new ListItem("Select Phase", "0"));

                if (coreData != null && coreData.Rows.Count > 0)
                {
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
                ScriptManager.RegisterStartupScript(this, GetType(), "DropdownError",
                    $"alert('Error loading core data: {ex.Message}');", true);
            }
        }
        private void InitializeGrid()
        {
            DataTable dt = CreateTestResultsDataTable();
            gvTestResults.DataSource = dt;
            gvTestResults.DataBind();
        }
        private DataTable CreateTestResultsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TestDate", typeof(DateTime));
            dt.Columns.Add("ItemTest", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SampleType", typeof(string));
            dt.Columns.Add("SpecimenNumber", typeof(string));
            dt.Columns.Add("ThicknessWeight", typeof(string));
            dt.Columns.Add("ThicknessAvgMin", typeof(string));
            dt.Columns.Add("WidthDiameter", typeof(string));
            dt.Columns.Add("WidthAvgMin", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("BF", typeof(string));
            dt.Columns.Add("TS", typeof(string));
            dt.Columns.Add("LO", typeof(string));
            dt.Columns.Add("L1", typeof(string));
            dt.Columns.Add("EL", typeof(string));
            dt.Columns.Add("MedianTS", typeof(string));
            dt.Columns.Add("MedianEL", typeof(string));
            dt.Columns.Add("VariationTS", typeof(string));
            dt.Columns.Add("VariationEL", typeof(string));

            return dt;
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ResetControlBorders(this);

                List<string> errors = new List<string>();
                List<Control> invalidControls = new List<Control>();

                if (string.IsNullOrWhiteSpace(txtDate.Text))
                {
                    errors.Add("Date is required.");
                    invalidControls.Add(txtDate);
                }

                if (string.IsNullOrWhiteSpace(txtItemTest.Text))
                {
                    errors.Add("Item Test is required.");
                    invalidControls.Add(txtItemTest);
                }

                if (ddlCorePhase.SelectedIndex == 0 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
                {
                    errors.Add("Core Phase is required.");
                    invalidControls.Add(ddlCorePhase);
                }

                if (string.IsNullOrWhiteSpace(txtsampletest.Text))
                {
                    errors.Add("Type of Sample Test is required.");
                    invalidControls.Add(txtsampletest);
                }

                if (string.IsNullOrWhiteSpace(txtSpecimenNumber.Text))
                {
                    errors.Add("Specimen Number is required.");
                    invalidControls.Add(txtSpecimenNumber);
                }

                if (string.IsNullOrWhiteSpace(txtThicknessWeight.Text))
                {
                    errors.Add("Thickness/Weight is required.");
                    invalidControls.Add(txtThicknessWeight);
                }
                else if (!IsCommaSeparatedNumbers(txtThicknessWeight.Text))
                {
                    errors.Add("Thickness/Weight must be comma-separated numeric values.");
                    invalidControls.Add(txtThicknessWeight);
                }

                if (string.IsNullOrWhiteSpace(txtWidthDiameter.Text))
                {
                    errors.Add("Width/Diameter is required.");
                    invalidControls.Add(txtWidthDiameter);
                }
                else if (!IsCommaSeparatedNumbers(txtWidthDiameter.Text))
                {
                    errors.Add("Width/Diameter must be comma-separated numeric values.");
                    invalidControls.Add(txtWidthDiameter);
                }

                ValidateNumeric(txtArea, "Area", errors, invalidControls);
                ValidateNumeric(txtBF, "BF", errors, invalidControls);
                ValidateNumeric(txtTS, "TS", errors, invalidControls);
                ValidateNumeric(txtLO, "Elongation LO", errors, invalidControls);
                ValidateNumeric(txtL1, "Elongation L1", errors, invalidControls);
                ValidateNumeric(txtEL, "Elongation EL", errors, invalidControls);
                ValidateNumeric(txtMedianTS, "Median TS", errors, invalidControls);
                ValidateNumeric(txtMedianEL, "Median EL", errors, invalidControls);
                ValidateNumeric(txtVariationTS, "Variation TS", errors, invalidControls);
                ValidateNumeric(txtVariationEL, "Variation EL", errors, invalidControls);

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
                            ddl.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                            ddl.BorderStyle = BorderStyle.Solid;
                            ddl.BorderWidth = 1;
                        }
                    }

                    string errorMessage = "Validation Errors:\\n• " +
                        string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                    ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                        $"alert('{errorMessage}');", true);

                    return; 
                }

                DataTable dt = GetCurrentGridData();
                DataRow newRow = dt.NewRow();

                newRow["TestDate"] = DateTime.Parse(txtDate.Text);
                newRow["ItemTest"] = txtItemTest.Text;
                newRow["CorePhase"] = ddlCorePhase.SelectedItem.Text;
                newRow["SampleType"] = txtsampletest.Text;
                newRow["SpecimenNumber"] = txtSpecimenNumber.Text;

                newRow["ThicknessWeight"] = txtThicknessWeight.Text;
                string thicknessAvgMin = CalculateAvgMin(txtThicknessWeight.Text);
                newRow["ThicknessAvgMin"] = thicknessAvgMin;
                txtThicknessAvgMin.Text = thicknessAvgMin;

                newRow["WidthDiameter"] = txtWidthDiameter.Text;
                string widthAvgMin = CalculateAvgMin(txtWidthDiameter.Text);
                newRow["WidthAvgMin"] = widthAvgMin;
                txtWidthAvgMin.Text = widthAvgMin;

                newRow["Area"] = txtArea.Text;
                newRow["BF"] = txtBF.Text;
                newRow["TS"] = txtTS.Text;
                newRow["LO"] = txtLO.Text;
                newRow["L1"] = txtL1.Text;
                newRow["EL"] = txtEL.Text;
                newRow["MedianTS"] = txtMedianTS.Text;
                newRow["MedianEL"] = txtMedianEL.Text;
                newRow["VariationTS"] = txtVariationTS.Text;
                newRow["VariationEL"] = txtVariationEL.Text;

                dt.Rows.Add(newRow);
                gvTestResults.DataSource = dt;
                gvTestResults.DataBind();
                ViewState["TestResultsData"] = dt;

                ClearFormFields();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Error: " + ex.Message + "');", true);
            }
        }
        private void ValidateNumeric(TextBox txt, string fieldName, List<string> errors, List<Control> invalidControls)
        {
            if (!string.IsNullOrWhiteSpace(txt.Text) && !double.TryParse(txt.Text, out _))
            {
                errors.Add($"{fieldName} must be numeric.");
                invalidControls.Add(txt);
            }
        }
        private bool IsCommaSeparatedNumbers(string input)
        {
        return Regex.IsMatch(input.Trim(), @"^(\d+(\.\d+)?)(\s*,\s*\d+(\.\d+)?)*$");
        }
  
        private string CalculateAvgMin(string commaSeparatedValues)
        {
            if (string.IsNullOrWhiteSpace(commaSeparatedValues))
                return "";

            try
            {
                string[] values = commaSeparatedValues.Split(',');
                List<decimal> numbers = new List<decimal>();

                foreach (string value in values)
                {
                    if (decimal.TryParse(value.Trim(), out decimal number))
                    {
                        numbers.Add(number);
                    }
                }

                if (numbers.Count > 0)
                {
                    decimal average = numbers.Average();
                    decimal minimum = numbers.Min();
                    return $"Avg: {average:F2}, Min: {minimum:F2}";
                }
            }
            catch
            {
                
            }

            return "";
        }
        private DataTable GetCurrentGridData()
        {
            DataTable dt;

            if (ViewState["TestResultsData"] != null)
            {
                dt = (DataTable)ViewState["TestResultsData"];
            }
            else
            {
                dt = CreateTestResultsDataTable();
            }

            return dt;
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
        private void ClearFormFields()
        {
            txtThicknessWeight.Text = "";
            txtThicknessAvgMin.Text = "";
            txtWidthDiameter.Text = "";
            txtWidthAvgMin.Text = "";
            txtArea.Text = "";
            txtBF.Text = "";
            txtTS.Text = "";
            txtLO.Text = "";
            txtL1.Text = "";
            txtEL.Text = "";
            txtMedianTS.Text = "";
            txtMedianEL.Text = "";
            txtVariationTS.Text = "";
            txtVariationEL.Text = "";
        }
        private void ClearAllFields()
        {
            Date.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";

            txtDate.Text = "";
            txtItemTest.Text = "";
            ddlCorePhase.SelectedIndex = 0;
            txtsampletest.Text = "";
            txtSpecimenNumber.Text = "";
            txtThicknessWeight.Text = "";
            txtThicknessAvgMin.Text = "";
            txtWidthDiameter.Text = "";
            txtWidthAvgMin.Text = "";
            txtArea.Text = "";
            txtBF.Text = "";
            txtTS.Text = "";
            txtLO.Text = "";
            txtL1.Text = "";
            txtEL.Text = "";
            txtMedianTS.Text = "";
            txtMedianEL.Text = "";
            txtVariationTS.Text = "";
            txtVariationEL.Text = "";
            gvTestResults.DataSource = null;
            gvTestResults.DataBind();
            ViewState["TestResults"] = null;
        }
        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSpecimenNumber.Text = "1";
        }

        protected void gvTestResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    DataTable dt = GetCurrentGridData();

                    if (rowIndex < dt.Rows.Count)
                    {
                        dt.Rows[rowIndex].Delete();
                        dt.AcceptChanges();
                    }

                    gvTestResults.DataSource = dt;
                    gvTestResults.DataBind();

                    ViewState["TestResultsData"] = dt;
                    UpdateSpecimenNumber();
                }
                catch (Exception ex)
                {
                    // Handle error
                     ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Error deleting row: " + ex.Message + "');", true);
                }
            }
        }
        protected void gvTestResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            GridView grid = (GridView)sender;
            int rowIndex = e.Row.RowIndex;

            if (rowIndex == 0)
                return;

            GridViewRow currentRow = e.Row;

            string currentDate = currentRow.Cells[0].Text.Trim().ToLower();
            string currentItemTest = currentRow.Cells[1].Text.Trim().ToLower();
            string currentCorePhase = currentRow.Cells[2].Text.Trim().ToLower();
            string currentSampleType = currentRow.Cells[3].Text.Trim().ToLower();

            // Merge Date (column 0) - only if both Date AND ItemTest match
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[0].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentDate == targetRow.Cells[0].Text.Trim().ToLower() &&
                    currentItemTest == targetRow.Cells[1].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentDate) &&
                    currentDate != "&nbsp;")
                {
                    if (targetRow.Cells[0].RowSpan == 0)
                        targetRow.Cells[0].RowSpan = 1;

                    targetRow.Cells[0].RowSpan += 1;
                    currentRow.Cells[0].Visible = false;
                }
            }

            // Merge ItemTest (column 1) only if Date also matches
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[1].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentItemTest == targetRow.Cells[1].Text.Trim().ToLower() &&
                    currentDate == targetRow.Cells[0].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentItemTest) &&
                    currentItemTest != "&nbsp;")
                {
                    if (targetRow.Cells[1].RowSpan == 0)
                        targetRow.Cells[1].RowSpan = 1;

                    targetRow.Cells[1].RowSpan += 1;
                    currentRow.Cells[1].Visible = false;
                }
            }

            // Merge CorePhase (column 2) if Date and ItemTest match
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[2].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentCorePhase == targetRow.Cells[2].Text.Trim().ToLower() &&
                    currentItemTest == targetRow.Cells[1].Text.Trim().ToLower() &&
                    currentDate == targetRow.Cells[0].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentCorePhase) &&
                    currentCorePhase != "&nbsp;")
                {
                    if (targetRow.Cells[2].RowSpan == 0)
                        targetRow.Cells[2].RowSpan = 1;

                    targetRow.Cells[2].RowSpan += 1;
                    currentRow.Cells[2].Visible = false;
                }
            }

            // Merge Type of Sample Test (column 3) if Date, ItemTest, and CorePhase match
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[3].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentSampleType == targetRow.Cells[3].Text.Trim().ToLower() &&
                    currentCorePhase == targetRow.Cells[2].Text.Trim().ToLower() &&
                    currentItemTest == targetRow.Cells[1].Text.Trim().ToLower() &&
                    currentDate == targetRow.Cells[0].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentSampleType) &&
                    currentSampleType != "&nbsp;")
                {
                    if (targetRow.Cells[3].RowSpan == 0)
                        targetRow.Cells[3].RowSpan = 1;

                    targetRow.Cells[3].RowSpan += 1;
                    currentRow.Cells[3].Visible = false;
                }
            }
        }
        private void UpdateSpecimenNumber()
        {
            DataTable dt = (DataTable)ViewState["TestResults"];
            int nextSpecimenNo = 1; 
            if (dt != null && dt.Rows.Count > 0)
            {
                nextSpecimenNo = dt.Rows.Count + 1;
            }
            TextBox txtSpecimenNo = FindControl("txtSpecimenNumber") as TextBox;

            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
            }
        }
        protected void btnSaveData_Click(object sender, EventArgs e)
        {
            ResetControlBorders(this);

            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            bool gridHasData = (gvTestResults.Rows.Count > 0);

            if (string.IsNullOrWhiteSpace(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }

            if (string.IsNullOrWhiteSpace(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }

            if (string.IsNullOrWhiteSpace(roomhumidity.Text) ||
                (!double.TryParse(roomhumidity.Text, out _) && roomhumidity.Text != "-"))
            {
                errors.Add("Room Humidity must be a valid number or '-'.");
                invalidControls.Add(roomhumidity);
            }

            if (string.IsNullOrWhiteSpace(roomtemp.Text) ||
                (!double.TryParse(roomtemp.Text, out _) && roomtemp.Text != "-"))
            {
                errors.Add("Room Temp. must be a valid number or '-'.");
                invalidControls.Add(roomtemp);
            }

            if (!gridHasData)
            {
                errors.Add("At least one Test Result is required in the grid.");
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

                string errorMessage = "Validation Errors:\\n• " +
                    string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);
            }
            else
            {
                // Save data
                bool isSaved = Submitform();
                if (isSaved)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                        "alert('Test results and Report Generated successfully!');", true);

                    ClearAllFields();
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

                // Group by Date + ItemTest combination
                var dateItemTestGroups = dt.AsEnumerable()
                    .GroupBy(r => new {
                        Date = r["TestDate"] != DBNull.Value ? Convert.ToDateTime(r["TestDate"]).ToString("yyyy-MM-dd") : "",
                        ItemTest = r["ItemTest"]?.ToString() ?? ""
                    })
                    .OrderBy(g => g.Key.Date)
                    .ThenBy(g => g.Key.ItemTest)
                    .ToList();

                // Lists to store the grouped data
                List<string> dateGroups = new List<string>();
                List<string> itemTestGroups = new List<string>();
                List<string> coreGroups = new List<string>();
                List<string> sampleGroups = new List<string>();
                List<string> specimenNoGroups = new List<string>();
                List<string> thicknessGroups = new List<string>();
                List<string> thicknessAvgGroups = new List<string>();
                List<string> widthGroups = new List<string>();
                List<string> widthAvgGroups = new List<string>();
                List<string> areaGroups = new List<string>();
                List<string> bfGroups = new List<string>();
                List<string> tsGroups = new List<string>();
                List<string> elongationL0Groups = new List<string>();
                List<string> elongationL1Groups = new List<string>();
                List<string> elongationELGroups = new List<string>();
                List<string> medianTSGroups = new List<string>();
                List<string> medianELGroups = new List<string>();
                List<string> variationTSGroups = new List<string>();
                List<string> variationELGroups = new List<string>();

                foreach (var group in dateItemTestGroups)
                {
                    // Add Date and ItemTest (these will be same for all rows in this group)
                    dateGroups.Add(group.Key.Date);
                    itemTestGroups.Add(group.Key.ItemTest);

                    var groupRows = group.ToList();

                    // Group all fields by CorePhase within this Date+ItemTest group
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
                    var bfGroupStrings = new List<string>();
                    var tsGroupStrings = new List<string>();
                    var elongationL0GroupStrings = new List<string>();
                    var elongationL1GroupStrings = new List<string>();
                    var elongationELGroupStrings = new List<string>();
                    var medianTSGroupStrings = new List<string>();
                    var medianELGroupStrings = new List<string>();
                    var variationTSGroupStrings = new List<string>();
                    var variationELGroupStrings = new List<string>();

                    foreach (var coreSubGroup in coreSubGroups)
                    {
                        var coreRows = coreSubGroup.ToList();

                        // Sample field (no parentheses, just values)
                        sampleGroupStrings.Add(string.Join(",", coreRows.Select(r => r["SampleType"]?.ToString() ?? "")));

                        // Specimen Number (no parentheses, just values)
                        specimenGroupStrings.Add(string.Join(",", coreRows.Select(r => r["SpecimenNumber"]?.ToString() ?? "")));

                        // Multi-value fields (with parentheses)
                        thicknessGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["ThicknessWeight"]?.ToString() ?? ""})")));
                        thicknessAvgGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["ThicknessAvgMin"]?.ToString() ?? ""})")));
                        widthGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["WidthDiameter"]?.ToString() ?? ""})")));
                        widthAvgGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["WidthAvgMin"]?.ToString() ?? ""})")));
                        areaGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["Area"]?.ToString() ?? ""})")));
                        bfGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["BF"]?.ToString() ?? ""})")));
                        tsGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["TS"]?.ToString() ?? ""})")));
                        elongationL0GroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["LO"]?.ToString() ?? ""})")));
                        elongationL1GroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["L1"]?.ToString() ?? ""})")));
                        elongationELGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["EL"]?.ToString() ?? ""})")));
                        medianTSGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["MedianTS"]?.ToString() ?? ""})")));
                        medianELGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["MedianEL"]?.ToString() ?? ""})")));
                        variationTSGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["VariationTS"]?.ToString() ?? ""})")));
                        variationELGroupStrings.Add(string.Join(",", coreRows.Select(r => $"({r["VariationEL"]?.ToString() ?? ""})")));
                    }

                    // Join all core groups with semicolon
                    sampleGroups.Add(string.Join(";", sampleGroupStrings));
                    specimenNoGroups.Add(string.Join(";", specimenGroupStrings));
                    thicknessGroups.Add(string.Join(";", thicknessGroupStrings));
                    thicknessAvgGroups.Add(string.Join(";", thicknessAvgGroupStrings));
                    widthGroups.Add(string.Join(";", widthGroupStrings));
                    widthAvgGroups.Add(string.Join(";", widthAvgGroupStrings));
                    areaGroups.Add(string.Join(";", areaGroupStrings));
                    bfGroups.Add(string.Join(";", bfGroupStrings));
                    tsGroups.Add(string.Join(";", tsGroupStrings));
                    elongationL0Groups.Add(string.Join(";", elongationL0GroupStrings));
                    elongationL1Groups.Add(string.Join(";", elongationL1GroupStrings));
                    elongationELGroups.Add(string.Join(";", elongationELGroupStrings));
                    medianTSGroups.Add(string.Join(";", medianTSGroupStrings));
                    medianELGroups.Add(string.Join(";", medianELGroupStrings));
                    variationTSGroups.Add(string.Join(";", variationTSGroupStrings));
                    variationELGroups.Add(string.Join(";", variationELGroupStrings));
                }

                // Prepare parameters for saving
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Add upper section values (these remain the same)
                reportParamMapList.Add("17380"); resultList.Add(cableType.Text);       // Type
                reportParamMapList.Add("17381"); resultList.Add(cableSize.Text);       // Size
                reportParamMapList.Add("17382"); resultList.Add(voltage.Text);         // Voltage
                reportParamMapList.Add("17383"); resultList.Add(Date.Text);            // Date
                reportParamMapList.Add("17384"); resultList.Add(roomno.Text);          // RoomNo
                reportParamMapList.Add("17385"); resultList.Add(roomtemp.Text);        // RoomTemp
                reportParamMapList.Add("17386"); resultList.Add(refSpec.Text);         // RefSpec
                reportParamMapList.Add("17387"); resultList.Add(testMethodNo.Text);    // TestMethodNo
                reportParamMapList.Add("17388"); resultList.Add(jobOrderNo.Text);      // JobOrderNo
                reportParamMapList.Add("17389"); resultList.Add(worksheetno.Text);     // WorkSheetNo
                reportParamMapList.Add("17390"); resultList.Add(roomhumidity.Text);    // RoomHumidity
                reportParamMapList.Add("17391"); resultList.Add(equipmentno.Text);     // EquipmentNo
                reportParamMapList.Add("17392"); resultList.Add(test.Text);            // Test Item
                reportParamMapList.Add("17393"); resultList.Add(length.Text);          // Length
                reportParamMapList.Add("17394"); resultList.Add(drumno.Text);          // DrumNo

                // Add grid data using pipe separator for different Date+ItemTest groups
                reportParamMapList.Add("17395"); resultList.Add(string.Join("|", dateGroups));              // Date
                reportParamMapList.Add("17396"); resultList.Add(string.Join("|", coreGroups));              // Core (semicolon within groups)
                reportParamMapList.Add("17397"); resultList.Add(string.Join("|", itemTestGroups));          // ItemTest
                reportParamMapList.Add("17398"); resultList.Add(string.Join("|", sampleGroups));            // Sample (semicolon within groups)
                reportParamMapList.Add("17399"); resultList.Add(string.Join("|", specimenNoGroups));        // SpecimenNo (semicolon within groups)
                reportParamMapList.Add("17400"); resultList.Add(string.Join("|", thicknessGroups));         // Thickness (semicolon within groups)
                reportParamMapList.Add("17401"); resultList.Add(string.Join("|", thicknessAvgGroups));      // ThicknessAvg (semicolon within groups)
                reportParamMapList.Add("17402"); resultList.Add(string.Join("|", widthGroups));             // Width (semicolon within groups)
                reportParamMapList.Add("17403"); resultList.Add(string.Join("|", widthAvgGroups));          // WidthAvg (semicolon within groups)
                reportParamMapList.Add("17404"); resultList.Add(string.Join("|", areaGroups));              // Area (semicolon within groups)
                reportParamMapList.Add("17405"); resultList.Add(string.Join("|", bfGroups));                // BF (semicolon within groups)
                reportParamMapList.Add("17406"); resultList.Add(string.Join("|", tsGroups));                // TS (semicolon within groups)
                reportParamMapList.Add("17407"); resultList.Add(string.Join("|", elongationL0Groups));      // ElongationL0 (semicolon within groups)
                reportParamMapList.Add("17408"); resultList.Add(string.Join("|", elongationL1Groups));      // ElongationL1 (semicolon within groups)
                reportParamMapList.Add("17409"); resultList.Add(string.Join("|", elongationELGroups));      // ElongationEL (semicolon within groups)
                reportParamMapList.Add("17410"); resultList.Add(string.Join("|", medianTSGroups));          // MedianTS (semicolon within groups)
                reportParamMapList.Add("17411"); resultList.Add(string.Join("|", medianELGroups));          // MedianEL (semicolon within groups)
                reportParamMapList.Add("17412"); resultList.Add(string.Join("|", variationTSGroups));       // VariationTS (semicolon within groups)
                reportParamMapList.Add("17413"); resultList.Add(string.Join("|", variationELGroups));       // VariationEL (semicolon within groups)

                //Specified Values
                reportParamMapList.Add("17491"); resultList.Add(txtSpecThickness.Text);
                reportParamMapList.Add("17492"); resultList.Add(txtSpecWidth.Text);
                reportParamMapList.Add("17493"); resultList.Add(txtSpecArea.Text);
                reportParamMapList.Add("17494"); resultList.Add(txtSpecBF.Text);
                reportParamMapList.Add("17495"); resultList.Add(txtSpecTS.Text);
                reportParamMapList.Add("17496"); resultList.Add(txtSpecLO.Text);
                reportParamMapList.Add("17497"); resultList.Add(txtSpecL1.Text);
                reportParamMapList.Add("17498"); resultList.Add(txtSpecEL.Text);
                reportParamMapList.Add("17499"); resultList.Add(txtSpecMedianTS.Text);
                reportParamMapList.Add("17500"); resultList.Add(txtSpecMedianEL.Text);
                reportParamMapList.Add("17501"); resultList.Add(txtSpecVarTS.Text);
                reportParamMapList.Add("17502"); resultList.Add(txtSpecVarEL.Text);


                // Ageing-Airflow parameters
                reportParamMapList.Add("17483"); resultList.Add(txtSpecimenAirflow.Text);
                reportParamMapList.Add("17484"); resultList.Add(txtCableAirflow.Text);


                string ConcatenateValues(string dateTimeValue)
                {
                    // Return empty string if the input is null or empty
                    if (string.IsNullOrEmpty(dateTimeValue?.Trim()))
                        return string.Empty;
                    return dateTimeValue;
                }
                // Specimen dates and times
                reportParamMapList.Add("17481");
                resultList.Add(ConcatenateValues(txtSpecimenDateStart.Text + " " + txtSpecimenTimeStart.Text)); // AgeingSpecimenStartDate

                reportParamMapList.Add("17482");
                resultList.Add(ConcatenateValues(txtSpecimenDateFinish.Text + " " + txtSpecimenTimeFinish.Text)); // AgeingSpecimenFinishDate

                reportParamMapList.Add("17486");
                resultList.Add(ConcatenateValues(txtCableDateStart.Text + " " + txtCableTimeStart.Text)); // AgeingCompleteCableStartDate

                reportParamMapList.Add("17487");
                resultList.Add(ConcatenateValues(txtCableDateFinish.Text + " " + txtCableTimeFinish.Text)); // AgeingCompleteCableFinishDate

                // Conditioning times
                reportParamMapList.Add("17488");
                resultList.Add(ConcatenateValues(txtDesiccatorsDateStart.Text + " " + txtDesiccatorsTimeStart.Text)); // AgeingConditioningInDesciccatorsStartDate

                reportParamMapList.Add("17480");
                resultList.Add(ConcatenateValues(txtDesiccatorsDateFinish.Text + " " + txtDesiccatorsTimeFinish.Text)); // AgeingConditioningInDesciccatorsFinishDate

                reportParamMapList.Add("17489");
                resultList.Add(ConcatenateValues(txtConditioningDateStart.Text + " " + txtConditioningTimeStart.Text)); // AfterAgeingConditioningStartDate

                reportParamMapList.Add("17490");
                resultList.Add(ConcatenateValues(txtConditioningDateFinish.Text + " " + txtConditioningTimeFinish.Text)); // AfterAgeingConditioningFinishDate


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

                    Response.Redirect($"GenerateTestRecord_TensileStrengthandElongation.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ResetControlBorders(this);

            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            bool gridHasData = (gvTestResults.Rows.Count > 0);

            if (string.IsNullOrWhiteSpace(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }

            if (string.IsNullOrWhiteSpace(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }

            if (string.IsNullOrWhiteSpace(roomhumidity.Text) ||
                (!double.TryParse(roomhumidity.Text, out _) && roomhumidity.Text != "-"))
            {
                errors.Add("Room Humidity must be a valid number or '-'.");
                invalidControls.Add(roomhumidity);
            }

            if (string.IsNullOrWhiteSpace(roomtemp.Text) ||
                (!double.TryParse(roomtemp.Text, out _) && roomtemp.Text != "-"))
            {
                errors.Add("Room Temp. must be a valid number or '-'.");
                invalidControls.Add(roomtemp);
            }

            if (!gridHasData)
            {
                errors.Add("At least one Test Result is required in the grid.");
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

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'")));

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

        //PRE-FILLING DATA IF EXISTS
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

                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        PrefillUpperSection(existingData);
                        PrefillGridData(existingData);
                        PrefillAgeingAndConditioningDetails(existingData);
          
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
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
        private void PrefillAgeingAndConditioningDetails(DataTable existingData)
        {
            // Prefill dates and times
            SetDateTimeText(txtSpecimenDateStart, txtSpecimenTimeStart, "17481");
            SetDateTimeText(txtSpecimenDateFinish, txtSpecimenTimeFinish, "17482");
            SetDateTimeText(txtCableDateStart, txtCableTimeStart, "17486");
            SetDateTimeText(txtCableDateFinish, txtCableTimeFinish, "17487");
            SetDateTimeText(txtDesiccatorsDateStart, txtDesiccatorsTimeStart, "17488");
            SetDateTimeText(txtDesiccatorsDateFinish, txtDesiccatorsTimeFinish, "17480");
            SetDateTimeText(txtConditioningDateStart, txtConditioningTimeStart, "17489");
            SetDateTimeText(txtConditioningDateFinish, txtConditioningTimeFinish, "17490");

            // Helper method to split and set date and time
            void SetDateTimeText(TextBox dateBox, TextBox timeBox, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    string dateTimeValue = row["Result"].ToString();
                    if (!string.IsNullOrEmpty(dateTimeValue))
                    {
                        // Attempt to split date and time
                        string[] parts = dateTimeValue.Split(' ');
                        if (parts.Length == 2)
                        {
                            dateBox.Text = parts[0];
                            timeBox.Text = parts[1];
                        }
                        else if (parts.Length == 1)
                        {
                            // If only one part, assume it's a date
                            dateBox.Text = parts[0];
                        }
                    }
                }
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Use ParameterID instead of ReportParamMapID
            SetTextBoxText(cableType, "17380");       // Type
            SetTextBoxText(cableSize, "17381");       // Size
            SetTextBoxText(voltage, "17382");      // Voltage
            SetTextBoxText(Date, "17383");          // Date
            SetTextBoxText(roomno, "17384");        // Room No
            SetTextBoxText(roomtemp, "17385");      // Room Temperature
            SetTextBoxText(refSpec, "17386");         // RefSpec
            SetTextBoxText(testMethodNo, "17387");    // Test Method No
            SetTextBoxText(jobOrderNo, "17388");      // Job Order No
            SetTextBoxText(worksheetno, "17389");     // Worksheet No
            SetTextBoxText(roomhumidity, "17390");  // Room Humidity
            SetTextBoxText(voltage, "17391");     // Equipment No
            SetTextBoxText(test, "17392");            // Test Item
            SetTextBoxText(length, "17393");          // Length
            SetTextBoxText(drumno, "17394");          // Drum No
      

            // Helper to set TextBox controls
            void SetTextBoxText(TextBox textBox, string parameterId)
            {
                var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }
        private void PrefillGridData(DataTable existingData)
        {
            try
            {
                // Get the saved grid data from database
                var dateData = GetParameterValue(existingData, "17395");      // Date
                var coreData = GetParameterValue(existingData, "17396");      // Core
                var itemTestData = GetParameterValue(existingData, "17397");  // ItemTest
                var sampleData = GetParameterValue(existingData, "17398");    // Sample
                var specimenData = GetParameterValue(existingData, "17399");  // SpecimenNo
                var thicknessData = GetParameterValue(existingData, "17400"); // Thickness
                var thicknessAvgData = GetParameterValue(existingData, "17401"); // ThicknessAvg
                var widthData = GetParameterValue(existingData, "17402");     // Width
                var widthAvgData = GetParameterValue(existingData, "17403");  // WidthAvg
                var areaData = GetParameterValue(existingData, "17404");      // Area
                var bfData = GetParameterValue(existingData, "17405");        // BF
                var tsData = GetParameterValue(existingData, "17406");        // TS
                var elongationL0Data = GetParameterValue(existingData, "17407"); // ElongationL0
                var elongationL1Data = GetParameterValue(existingData, "17408"); // ElongationL1
                var elongationELData = GetParameterValue(existingData, "17409"); // ElongationEL
                var medianTSData = GetParameterValue(existingData, "17410");  // MedianTS
                var medianELData = GetParameterValue(existingData, "17411");  // MedianEL
                var variationTSData = GetParameterValue(existingData, "17412"); // VariationTS
                var variationELData = GetParameterValue(existingData, "17413"); // VariationEL

                // Also prefill the specified values
                SetTextBoxText(txtSpecThickness, "17491");
                SetTextBoxText(txtSpecWidth, "17492");
                SetTextBoxText(txtSpecArea, "17493");
                SetTextBoxText(txtSpecBF, "17494");
                SetTextBoxText(txtSpecTS, "17495");
                SetTextBoxText(txtSpecLO, "17496");
                SetTextBoxText(txtSpecL1, "17497");
                SetTextBoxText(txtSpecEL, "17498");
                SetTextBoxText(txtSpecMedianTS, "17499");
                SetTextBoxText(txtSpecMedianEL, "17500");
                SetTextBoxText(txtSpecVarTS, "17501");
                SetTextBoxText(txtSpecVarEL, "17502");

                // Also prefill the airflow values
                SetTextBoxText(txtSpecimenAirflow, "17483");
                SetTextBoxText(txtCableAirflow, "17484");

                // If any grid data exists, reconstruct the DataTable
                if (!string.IsNullOrEmpty(dateData))
                {
                    DataTable dt = CreateTestResultsDataTable();

                    // Split by pipe to get different Date+ItemTest groups
                    var dateGroups = dateData.Split('|');
                    var coreGroups = coreData?.Split('|') ?? new string[0];
                    var itemTestGroups = itemTestData?.Split('|') ?? new string[0];
                    var sampleGroups = sampleData?.Split('|') ?? new string[0];
                    var specimenGroups = specimenData?.Split('|') ?? new string[0];
                    var thicknessGroups = thicknessData?.Split('|') ?? new string[0];
                    var thicknessAvgGroups = thicknessAvgData?.Split('|') ?? new string[0];
                    var widthGroups = widthData?.Split('|') ?? new string[0];
                    var widthAvgGroups = widthAvgData?.Split('|') ?? new string[0];
                    var areaGroups = areaData?.Split('|') ?? new string[0];
                    var bfGroups = bfData?.Split('|') ?? new string[0];
                    var tsGroups = tsData?.Split('|') ?? new string[0];
                    var elongationL0Groups = elongationL0Data?.Split('|') ?? new string[0];
                    var elongationL1Groups = elongationL1Data?.Split('|') ?? new string[0];
                    var elongationELGroups = elongationELData?.Split('|') ?? new string[0];
                    var medianTSGroups = medianTSData?.Split('|') ?? new string[0];
                    var medianELGroups = medianELData?.Split('|') ?? new string[0];
                    var variationTSGroups = variationTSData?.Split('|') ?? new string[0];
                    var variationELGroups = variationELData?.Split('|') ?? new string[0];

                    for (int groupIndex = 0; groupIndex < dateGroups.Length; groupIndex++)
                    {
                        var currentDate = dateGroups[groupIndex];
                        var currentItemTest = groupIndex < itemTestGroups.Length ? itemTestGroups[groupIndex] : "";

                        // Split by semicolon to get core subgroups within this Date+ItemTest group
                        var coreSubGroups = groupIndex < coreGroups.Length ? coreGroups[groupIndex].Split(';') : new string[0];
                        var sampleSubGroups = groupIndex < sampleGroups.Length ? sampleGroups[groupIndex].Split(';') : new string[0];
                        var specimenSubGroups = groupIndex < specimenGroups.Length ? specimenGroups[groupIndex].Split(';') : new string[0];
                        var thicknessSubGroups = groupIndex < thicknessGroups.Length ? thicknessGroups[groupIndex].Split(';') : new string[0];
                        var thicknessAvgSubGroups = groupIndex < thicknessAvgGroups.Length ? thicknessAvgGroups[groupIndex].Split(';') : new string[0];
                        var widthSubGroups = groupIndex < widthGroups.Length ? widthGroups[groupIndex].Split(';') : new string[0];
                        var widthAvgSubGroups = groupIndex < widthAvgGroups.Length ? widthAvgGroups[groupIndex].Split(';') : new string[0];
                        var areaSubGroups = groupIndex < areaGroups.Length ? areaGroups[groupIndex].Split(';') : new string[0];
                        var bfSubGroups = groupIndex < bfGroups.Length ? bfGroups[groupIndex].Split(';') : new string[0];
                        var tsSubGroups = groupIndex < tsGroups.Length ? tsGroups[groupIndex].Split(';') : new string[0];
                        var elongationL0SubGroups = groupIndex < elongationL0Groups.Length ? elongationL0Groups[groupIndex].Split(';') : new string[0];
                        var elongationL1SubGroups = groupIndex < elongationL1Groups.Length ? elongationL1Groups[groupIndex].Split(';') : new string[0];
                        var elongationELSubGroups = groupIndex < elongationELGroups.Length ? elongationELGroups[groupIndex].Split(';') : new string[0];
                        var medianTSSubGroups = groupIndex < medianTSGroups.Length ? medianTSGroups[groupIndex].Split(';') : new string[0];
                        var medianELSubGroups = groupIndex < medianELGroups.Length ? medianELGroups[groupIndex].Split(';') : new string[0];
                        var variationTSSubGroups = groupIndex < variationTSGroups.Length ? variationTSGroups[groupIndex].Split(';') : new string[0];
                        var variationELSubGroups = groupIndex < variationELGroups.Length ? variationELGroups[groupIndex].Split(';') : new string[0];

                        // Process each core subgroup
                        for (int coreIndex = 0; coreIndex < coreSubGroups.Length; coreIndex++)
                        {
                            var coreValues = coreSubGroups[coreIndex].Split(',');
                            var sampleValues = coreIndex < sampleSubGroups.Length ? sampleSubGroups[coreIndex].Split(',') : new string[0];
                            var specimenValues = coreIndex < specimenSubGroups.Length ? specimenSubGroups[coreIndex].Split(',') : new string[0];

                            // For other fields - split by parentheses groups
                            var thicknessValues = coreIndex < thicknessSubGroups.Length ? SplitByParentheses(thicknessSubGroups[coreIndex]) : new string[0];
                            var thicknessAvgValues = coreIndex < thicknessAvgSubGroups.Length ? SplitByParentheses(thicknessAvgSubGroups[coreIndex]) : new string[0];
                            var widthValues = coreIndex < widthSubGroups.Length ? SplitByParentheses(widthSubGroups[coreIndex]) : new string[0];
                            var widthAvgValues = coreIndex < widthAvgSubGroups.Length ? SplitByParentheses(widthAvgSubGroups[coreIndex]) : new string[0];
                            var areaValues = coreIndex < areaSubGroups.Length ? SplitByParentheses(areaSubGroups[coreIndex]) : new string[0];
                            var bfValues = coreIndex < bfSubGroups.Length ? SplitByParentheses(bfSubGroups[coreIndex]) : new string[0];
                            var tsValues = coreIndex < tsSubGroups.Length ? SplitByParentheses(tsSubGroups[coreIndex]) : new string[0];
                            var elongationL0Values = coreIndex < elongationL0SubGroups.Length ? SplitByParentheses(elongationL0SubGroups[coreIndex]) : new string[0];
                            var elongationL1Values = coreIndex < elongationL1SubGroups.Length ? SplitByParentheses(elongationL1SubGroups[coreIndex]) : new string[0];
                            var elongationELValues = coreIndex < elongationELSubGroups.Length ? SplitByParentheses(elongationELSubGroups[coreIndex]) : new string[0];
                            var medianTSValues = coreIndex < medianTSSubGroups.Length ? SplitByParentheses(medianTSSubGroups[coreIndex]) : new string[0];
                            var medianELValues = coreIndex < medianELSubGroups.Length ? SplitByParentheses(medianELSubGroups[coreIndex]) : new string[0];
                            var variationTSValues = coreIndex < variationTSSubGroups.Length ? SplitByParentheses(variationTSSubGroups[coreIndex]) : new string[0];
                            var variationELValues = coreIndex < variationELSubGroups.Length ? SplitByParentheses(variationELSubGroups[coreIndex]) : new string[0];

                            // Determine the number of rows in this core subgroup
                            int rowCount = Math.Max(coreValues.Length, Math.Max(sampleValues.Length, specimenValues.Length));

                            // Add rows for each value in this core subgroup
                            for (int valueIndex = 0; valueIndex < rowCount; valueIndex++)
                            {
                                DataRow row = dt.NewRow();

                                // Parse date
                                DateTime parsedDate;
                                if (DateTime.TryParse(currentDate, out parsedDate))
                                    row["TestDate"] = parsedDate;

                                row["ItemTest"] = currentItemTest;
                                row["CorePhase"] = valueIndex < coreValues.Length ? coreValues[valueIndex] : "";
                                row["SampleType"] = valueIndex < sampleValues.Length ? sampleValues[valueIndex] : "";
                                row["SpecimenNumber"] = valueIndex < specimenValues.Length ? specimenValues[valueIndex] : "";

                                // Remove parentheses and parse values
                                row["ThicknessWeight"] = valueIndex < thicknessValues.Length ? CleanValue(thicknessValues[valueIndex]) : "";
                                row["ThicknessAvgMin"] = valueIndex < thicknessAvgValues.Length ? CleanValue(thicknessAvgValues[valueIndex]) : "";
                                row["WidthDiameter"] = valueIndex < widthValues.Length ? CleanValue(widthValues[valueIndex]) : "";
                                row["WidthAvgMin"] = valueIndex < widthAvgValues.Length ? CleanValue(widthAvgValues[valueIndex]) : "";
                                row["Area"] = valueIndex < areaValues.Length ? CleanValue(areaValues[valueIndex]) : "";
                                row["BF"] = valueIndex < bfValues.Length ? CleanValue(bfValues[valueIndex]) : "";
                                row["TS"] = valueIndex < tsValues.Length ? CleanValue(tsValues[valueIndex]) : "";
                                row["LO"] = valueIndex < elongationL0Values.Length ? CleanValue(elongationL0Values[valueIndex]) : "";
                                row["L1"] = valueIndex < elongationL1Values.Length ? CleanValue(elongationL1Values[valueIndex]) : "";
                                row["EL"] = valueIndex < elongationELValues.Length ? CleanValue(elongationELValues[valueIndex]) : "";
                                row["MedianTS"] = valueIndex < medianTSValues.Length ? CleanValue(medianTSValues[valueIndex]) : "";
                                row["MedianEL"] = valueIndex < medianELValues.Length ? CleanValue(medianELValues[valueIndex]) : "";
                                row["VariationTS"] = valueIndex < variationTSValues.Length ? CleanValue(variationTSValues[valueIndex]) : "";
                                row["VariationEL"] = valueIndex < variationELValues.Length ? CleanValue(variationELValues[valueIndex]) : "";

                                dt.Rows.Add(row);
                            }
                        }
                    }

                    // Bind to grid and store in ViewState
                    gvTestResults.DataSource = dt;
                    gvTestResults.DataBind();
                    ViewState["TestResultsData"] = dt;
                }

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


    }
}