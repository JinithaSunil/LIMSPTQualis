using LIMS.DAL;

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
    public partial class PartialDischarge : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //upper section
                roomno.Text = "SHIELDING";
                cableType.Text = "Cu/XLPE/LS/DSTA/PE";
                refSpec.Text = "IEC 60502-2";
                cableSize.Text = "3 x 25 mm²";
                testMethodNo.Text = "IEC 60502-2";
                lblvoltage.Text = "3,6/6(7,2)kV";
                jobOrderNo.Text = "WO/2024120905";
                worksheetno.Text = "-";
                equipmentno.Text = "QE/15/EL/34";

                //standard
                txtStandardmaxpd.Text = "MAX: 5";

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();
            }
        }

        //Buttons
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            decimal volatagetest1 = 0, volatagetest2 = 0, duration = 0, maxpd = 0;

            if (string.IsNullOrWhiteSpace(testdate.Text))
            {
                invalidControls.Add(testdate);
                hasError = true;
            }

            if (ddlitemtest.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlitemtest.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlitemtest.CssClass += " input-error";
                hasError = true;
            }

            if (ddlCorePhase.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlCorePhase.CssClass += " input-error";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtvoltagetestU0.Text) || !decimal.TryParse(txtvoltagetestU0.Text, out volatagetest1))
            {
                invalidControls.Add(txtvoltagetestU0);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtvoltagetestkV.Text) || !decimal.TryParse(txtvoltagetestkV.Text, out volatagetest2))
            {
                invalidControls.Add(txtvoltagetestkV);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtduration.Text) || !decimal.TryParse(txtduration.Text, out duration))
            {
                invalidControls.Add(txtduration);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtmaximumpd.Text) || !decimal.TryParse(txtmaximumpd.Text, out maxpd))
            {
                invalidControls.Add(txtmaximumpd);
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
                dt.Columns.Add("volatagetest1", typeof(decimal));
                dt.Columns.Add("volatagetest2", typeof(decimal));
                dt.Columns.Add("duration", typeof(decimal));
                dt.Columns.Add("maxpd", typeof(decimal));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate.Text;
            dr["ItemTest"] = ddlitemtest.SelectedValue;
            dr["CorePhase"] = ddlCorePhase.SelectedValue;
            dr["volatagetest1"] = volatagetest1;
            dr["volatagetest2"] = volatagetest2;
            dr["duration"] = duration;
            dr["maxpd"] = maxpd;

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["TestResults"] = dt;


            // Rebind the GridView
            gvPartialdischarge.DataSource = dt;
            gvPartialdischarge.DataBind();


            // Clear the input fields for next entry
            ClearInputFields();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvPartialdischarge.Rows.Count > 0);

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

            // Validate Characteristics Values section
            if (string.IsNullOrEmpty(txtbandwidth.Text) || !double.TryParse(txtbandwidth.Text, out _))
            {
                errors.Add("Band width must be a valid number.");
                invalidControls.Add(txtbandwidth);
            }
            if (string.IsNullOrEmpty(txtcalibration.Text) || !double.TryParse(txtcalibration.Text, out _))
            {
                errors.Add("Calibration must be a valid number.");
                invalidControls.Add(txtcalibration);
            }
            if (string.IsNullOrEmpty(txttestfrequency.Text) || !double.TryParse(txttestfrequency.Text, out _))
            {
                errors.Add("Test frequency must be a valid number.");
                invalidControls.Add(txttestfrequency);
            }
            if (string.IsNullOrEmpty(txtnoise.Text) || !double.TryParse(txtnoise.Text, out _))
            {
                errors.Add("Noise must be a valid number.");
                invalidControls.Add(txtnoise);
            }
            if (string.IsNullOrEmpty(txtcouplingcap.Text) || !double.TryParse(txtcouplingcap.Text, out _))
            {
                errors.Add("Coupling capactor must be a valid number.");
                invalidControls.Add(txtcouplingcap);
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

                if (string.IsNullOrEmpty(txtvoltagetestU0.Text) || !double.TryParse(txtvoltagetestU0.Text, out _))
                {
                    errors.Add("Voltage test (U0) must be a valid number.");
                    invalidControls.Add(txtvoltagetestU0);
                }
                if (string.IsNullOrEmpty(txtvoltagetestkV.Text) || !double.TryParse(txtvoltagetestkV.Text, out _))
                {
                    errors.Add("Voltage test (kV) must be a valid number.");
                    invalidControls.Add(txtvoltagetestkV);
                }
                if (string.IsNullOrEmpty(txtduration.Text) || !double.TryParse(txtduration.Text, out _))
                {
                    errors.Add("Duration must be in valid form.");
                    invalidControls.Add(txtduration);
                }
                if (string.IsNullOrEmpty(txtmaximumpd.Text) || !double.TryParse(txtmaximumpd.Text, out _))
                {
                    errors.Add("Max. Partial Discharge value must be a valid number.");
                    invalidControls.Add(txtmaximumpd);
                }
            }

            // Validate Standard Values section
            if (string.IsNullOrEmpty(txtStandardmaxpd.Text))
            {
                errors.Add("Standard Max. Partial Discharge must be valid .");
                invalidControls.Add(txtStandardmaxpd);
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
                // Save data 
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvPartialdischarge.Rows.Count > 0);

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

            // Validate Characteristics Values section
            if (string.IsNullOrEmpty(txtbandwidth.Text) || !double.TryParse(txtbandwidth.Text, out _))
            {
                errors.Add("Band width must be a valid number.");
                invalidControls.Add(txtbandwidth);
            }
            if (string.IsNullOrEmpty(txtcalibration.Text) || !double.TryParse(txtcalibration.Text, out _))
            {
                errors.Add("Calibration must be a valid number.");
                invalidControls.Add(txtcalibration);
            }
            if (string.IsNullOrEmpty(txttestfrequency.Text) || !double.TryParse(txttestfrequency.Text, out _))
            {
                errors.Add("Test frequency must be a valid number.");
                invalidControls.Add(txttestfrequency);
            }
            if (string.IsNullOrEmpty(txtnoise.Text) || !double.TryParse(txtnoise.Text, out _))
            {
                errors.Add("Noise must be a valid number.");
                invalidControls.Add(txtnoise);
            }
            if (string.IsNullOrEmpty(txtcouplingcap.Text) || !double.TryParse(txtcouplingcap.Text, out _))
            {
                errors.Add("Coupling capactor must be a valid number.");
                invalidControls.Add(txtcouplingcap);
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

                if (string.IsNullOrEmpty(txtvoltagetestU0.Text) || !double.TryParse(txtvoltagetestU0.Text, out _))
                {
                    errors.Add("Voltage test (U0) must be a valid number.");
                    invalidControls.Add(txtvoltagetestU0);
                }
                if (string.IsNullOrEmpty(txtvoltagetestkV.Text) || !double.TryParse(txtvoltagetestkV.Text, out _))
                {
                    errors.Add("Voltage test (kV) must be a valid number.");
                    invalidControls.Add(txtvoltagetestkV);
                }
                if (string.IsNullOrEmpty(txtduration.Text) || !double.TryParse(txtduration.Text, out _))
                {
                    errors.Add("Duration must be in valid form.");
                    invalidControls.Add(txtduration);
                }
                if (string.IsNullOrEmpty(txtmaximumpd.Text) || !double.TryParse(txtmaximumpd.Text, out _))
                {
                    errors.Add("Max. Partial Discharge value must be a valid number.");
                    invalidControls.Add(txtmaximumpd);
                }
            }

            // Validate Standard Values section
            if (string.IsNullOrEmpty(txtStandardmaxpd.Text))
            {
                errors.Add("Standard Max. Partial Discharge must be valid .");
                invalidControls.Add(txtStandardmaxpd);
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

        //Grids
        protected void gvPartialdischarge_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                // Get the row index from the CommandArgument
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                {
                    // Remove the row at the specified index
                    dt.Rows.RemoveAt(rowIndex);

                    // Accept the changes to the DataTable
                    dt.AcceptChanges();

                    // Update the ViewState
                    ViewState["TestResults"] = dt;

                    // Rebind the GridView
                    gvPartialdischarge.DataSource = dt;
                    gvPartialdischarge.DataBind();
                }

            }
        }
        protected void gvPartialdischarge_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous values
                    string currentItemTest = rowView["ItemTest"].ToString();
                    string currentDate = rowView["Date"].ToString();
                    string previousItemTest = dt.Rows[e.Row.RowIndex - 1]["ItemTest"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Item Tests are the same
                    if (currentItemTest == previousItemTest)
                    {
                        // Find the first row with this Item Test
                        int startRowIndex = e.Row.RowIndex - 1;
                        while (startRowIndex > 0 && dt.Rows[startRowIndex - 1]["ItemTest"].ToString() == currentItemTest)
                        {
                            startRowIndex--;
                        }

                        // Increase the row span for the Item Test cell
                        if (gridView.Rows[startRowIndex].Cells[1].RowSpan == 0)
                            gridView.Rows[startRowIndex].Cells[1].RowSpan = 2;
                        else
                            gridView.Rows[startRowIndex].Cells[1].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[1].Visible = false;

                        // If Dates are also the same, merge them too
                        if (currentDate == previousDate)
                        {
                            // Find the first row with this Date in current Item Test group
                            int dateStartRowIndex = e.Row.RowIndex - 1;
                            while (dateStartRowIndex > startRowIndex &&
                                  dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                            {
                                dateStartRowIndex--;
                            }

                            // Increase the row span for the Date cell
                            if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                                gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                            else
                                gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                            // Hide the cell in the current row
                            e.Row.Cells[0].Visible = false;
                        }
                    }

                    // Handle Core column merging - this should only happen within the same Item Test
                    string currentCore = rowView["CorePhase"].ToString();
                    string previousCore = dt.Rows[e.Row.RowIndex - 1]["CorePhase"].ToString();

                    if (currentItemTest == previousItemTest && currentCore == previousCore)
                    {
                        // Find the first row with this Core value in the current Item Test group
                        int coreStartRowIndex = e.Row.RowIndex - 1;
                        while (coreStartRowIndex > 0 &&
                              dt.Rows[coreStartRowIndex]["ItemTest"].ToString() == currentItemTest &&
                              dt.Rows[coreStartRowIndex - 1]["CorePhase"].ToString() == currentCore)
                        {
                            coreStartRowIndex--;
                        }

                        // Increase the row span for the Core cell
                        if (gridView.Rows[coreStartRowIndex].Cells[2].RowSpan == 0)
                            gridView.Rows[coreStartRowIndex].Cells[2].RowSpan = 2;
                        else
                            gridView.Rows[coreStartRowIndex].Cells[2].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[2].Visible = false;
                    }
                }
            }
        }

        //Saving Data
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


                foreach (var testType in testTypeData.Keys)
                {
                    var rows = testTypeData[testType];
                    string testDate = rows[0]["Date"].ToString();

                    // Get all core phases in order of specimen numbers
                    string corePhases = string.Join(",", rows.Select(r => r["CorePhase"].ToString()));

                    // Get all width strips in order
                    string volatagetest1 = string.Join(",", rows.Select(r => r["volatagetest1"].ToString()));

                    // Get all pulling speeds in order
                    string volatagetest2 = string.Join(",", rows.Select(r => r["volatagetest2"].ToString()));

                    // Get all measured values in order
                    string duration = string.Join(",", rows.Select(r => r["duration"].ToString()));
                    // Get all measured values in order
                    string maxpd = string.Join(",", rows.Select(r => r["maxpd"].ToString()));

                    // Group rows by CorePhase to get unique core phases
                    var corePhaseGroups = rows.GroupBy(r => r["CorePhase"].ToString()).ToList();
                    // Collect min and max values
                    if (testType.Contains("Partial discharge after bending test 3 cycle diameter cylinder 120cm"))
                    {

                        // Add parameters for After Bending test
                        reportParamMapList.Add("5148"); // Date param ID
                        resultList.Add(testDate);

                        // Add core phases After Bending test
                        reportParamMapList.Add("4136"); // ID for After Bending test core phases
                        resultList.Add(corePhases);

                        // Add other measurements
                        reportParamMapList.Add("4137"); resultList.Add(volatagetest1); // volatagetest1
                        reportParamMapList.Add("5146"); resultList.Add(volatagetest2); // volatagetest2
                        reportParamMapList.Add("4138"); resultList.Add(duration); // duration
                        reportParamMapList.Add("4139"); resultList.Add(maxpd); // maxpd
                    }
                    else if (testType.Contains("Partial discharge after heating cycle 20 times"))
                    {

                        // Add parameters for After Heating cycle
                        reportParamMapList.Add("5149"); // After Heating cycle param ID
                        resultList.Add(testDate);

                        // Add core phases for After Heating cycle
                        reportParamMapList.Add("4140"); // ID for After Heating cycle core phases
                        resultList.Add(corePhases);

                        // Add other measurements
                        reportParamMapList.Add("5156"); resultList.Add(volatagetest1); // volatagetest1
                        reportParamMapList.Add("5147"); resultList.Add(volatagetest2); // volatagetest2
                        reportParamMapList.Add("4141"); resultList.Add(duration); // duration
                        reportParamMapList.Add("4142"); resultList.Add(maxpd); // maxpd
                    }
                }


                // Add Characteristics values
                reportParamMapList.Add("5150"); resultList.Add(ddlcircuit.SelectedItem.Text); // circuit
                reportParamMapList.Add("5151"); resultList.Add(txtbandwidth.Text); // band width
                reportParamMapList.Add("5152"); resultList.Add(txtcalibration.Text); // calibration 
                reportParamMapList.Add("5154"); resultList.Add(txtnoise.Text); // noise
                reportParamMapList.Add("5153"); resultList.Add(txtcouplingcap.Text); // couplingcap
                reportParamMapList.Add("5155"); resultList.Add(txttestfrequency.Text); // t freq.

                // Standard value 
                reportParamMapList.Add("4143"); resultList.Add(txtStandardmaxpd.Text);

                //Upper section
                reportParamMapList.Add("4165"); resultList.Add(cableType.Text);
                reportParamMapList.Add("4171"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("4166"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("4172"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("4167"); resultList.Add(lblvoltage.Text);
                reportParamMapList.Add("4173"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("4174"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("4169"); resultList.Add(roomno.Text);
                reportParamMapList.Add("4175"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("4170"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("4168"); resultList.Add(Date.Text);
                reportParamMapList.Add("4176"); resultList.Add(equipmentno.Text);


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

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // GenerateTestReport(_cm.sampleId);
                    Response.Redirect($"GenerateTestRecord_PartialDischarge.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
        private void ClearInputFields()
        {
            txtvoltagetestU0.Text = "";
            txtvoltagetestkV.Text = "";
            txtduration.Text = "";
            txtmaximumpd.Text = "";
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

            //Characteristics details
            ddlcircuit.SelectedIndex = 0;
            txtbandwidth.Text = "";
            txtcalibration.Text = "";
            txttestfrequency.Text = "";
            txtnoise.Text = "";
            txtcouplingcap.Text = "";

            // Test Details
            testdate.Text = "";
            ddlitemtest.SelectedIndex = 0;
            ddlCorePhase.SelectedIndex = 0;
            txtvoltagetestU0.Text = "";
            txtvoltagetestkV.Text = "";
            txtduration.Text = "";
            txtmaximumpd.Text = "";

            // Clear GridView
            gvPartialdischarge.DataSource = null;
            gvPartialdischarge.DataBind();

            // Standard Values
            txtStandardmaxpd.Text = "";


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
            allTextBoxes.Add(txtvoltagetestU0);
            allTextBoxes.Add(txtvoltagetestkV);
            allTextBoxes.Add(txtduration);
            allTextBoxes.Add(txtmaximumpd);
            allTextBoxes.Add(txtStandardmaxpd);

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }

        //PRE-FILLING DATA IF EXISTS FOR SAMPLEID
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
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        PrefillUpperSection(existingData);
                        PrefillTestResultsGrid(existingData);
                        PrefillCharacteristics(existingData);
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

        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "4165");
            SetLabelText(refSpec, "4171");
            SetLabelText(cableSize, "4166");
            SetLabelText(testMethodNo, "4172");
            SetLabelText(lblvoltage, "4167");
            SetLabelText(jobOrderNo, "4173");
            SetLabelText(worksheetno, "4174");
            SetLabelText(equipmentno, "4176");

            // Textbox fields
            SetTextBoxText(roomno, "4169");
            SetTextBoxText(roomhumidity, "4175");
            SetTextBoxText(roomtemp, "4170");
            SetTextBoxText(Date, "4168");

            //standard
            SetTextBoxText(txtStandardmaxpd, "4143");

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
        protected void PrefillTestResultsGrid(DataTable existingData)
        {
            try
            {
                // Create a DataTable to hold the grid data
                DataTable dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("ItemTest", typeof(string));
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("volatagetest1", typeof(string));
                dt.Columns.Add("volatagetest2", typeof(string));
                dt.Columns.Add("duration", typeof(string));
                dt.Columns.Add("maxpd", typeof(string));
                dt.Columns.Add("ID", typeof(Guid));

                // Check for After Bending Test data
                string bendingCorePhases = GetParameterValue(existingData, "4136");
                string bendingVoltageTest1 = GetParameterValue(existingData, "4137");
                string bendingVoltageTest2 = GetParameterValue(existingData, "5146");
                string bendingDuration = GetParameterValue(existingData, "4138");
                string bendingMaxPD = GetParameterValue(existingData, "4139");
                string bendingDate = GetParameterValue(existingData, "5148");

                // Add After Bending Test rows if data exists
                if (!string.IsNullOrEmpty(bendingCorePhases))
                {
                    string[] cores = bendingCorePhases.Split(',');
                    string[] voltages1 = bendingVoltageTest1.Split(',');
                    string[] voltages2 = bendingVoltageTest2.Split(',');
                    string[] durations = bendingDuration.Split(',');
                    string[] maxPDs = bendingMaxPD.Split(',');

                    // Add rows for After Bending Test
                    for (int i = 0; i < cores.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(cores[i]))
                        {
                            DataRow row = dt.NewRow();
                            row["Date"] = bendingDate;
                            row["ItemTest"] = "Partial discharge after bending test 3 cycle diameter cylinder 120cm";
                            row["CorePhase"] = cores[i];
                            row["volatagetest1"] = i < voltages1.Length ? voltages1[i] : "";
                            row["volatagetest2"] = i < voltages2.Length ? voltages2[i] : "";
                            row["duration"] = i < durations.Length ? durations[i] : "";
                            row["maxpd"] = i < maxPDs.Length ? maxPDs[i] : "";
                            row["ID"] = Guid.NewGuid();
                            dt.Rows.Add(row);
                        }
                    }
                }

                // Check for After Heating Cycle data
                string heatingCorePhases = GetParameterValue(existingData, "4140");
                string heatingVoltageTest1 = GetParameterValue(existingData, "5156");
                string heatingVoltageTest2 = GetParameterValue(existingData, "5147");
                string heatingDuration = GetParameterValue(existingData, "4141");
                string heatingMaxPD = GetParameterValue(existingData, "4142");
                string heatingDate = GetParameterValue(existingData, "5149");

                // Add After Heating Cycle rows if data exists
                if (!string.IsNullOrEmpty(heatingCorePhases))
                {
                    string[] cores = heatingCorePhases.Split(',');
                    string[] voltages1 = heatingVoltageTest1.Split(',');
                    string[] voltages2 = heatingVoltageTest2.Split(',');
                    string[] durations = heatingDuration.Split(',');
                    string[] maxPDs = heatingMaxPD.Split(',');

                    // Add rows for After Heating Cycle
                    for (int i = 0; i < cores.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(cores[i]))
                        {
                            DataRow row = dt.NewRow();
                            row["Date"] = heatingDate;
                            row["ItemTest"] = "Partial discharge after heating cycle 20 times";
                            row["CorePhase"] = cores[i];
                            row["volatagetest1"] = i < voltages1.Length ? voltages1[i] : "";
                            row["volatagetest2"] = i < voltages2.Length ? voltages2[i] : "";
                            row["duration"] = i < durations.Length ? durations[i] : "";
                            row["maxpd"] = i < maxPDs.Length ? maxPDs[i] : "";
                            row["ID"] = Guid.NewGuid();
                            dt.Rows.Add(row);
                        }
                    }
                }

                // Bind the data to the grid if we have rows
                if (dt.Rows.Count > 0)
                {
                    // Store in ViewState for later use
                    ViewState["TestResults"] = dt;

                    // Bind to GridView
                    gvPartialdischarge.DataSource = dt;
                    gvPartialdischarge.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error loading test grid data: {ex.Message}');", true);
            }
        }

        private void PrefillCharacteristics(DataTable existingData)
        {
            // Prefill values from DataTable based on ParameterID
            SetTextFromDataTable(ddlcircuit, "5150");
            SetTextFromDataTable(txtbandwidth, "5151");
            SetTextFromDataTable(txtcalibration, "5152");
            SetTextFromDataTable(txtcouplingcap, "5153");
            SetTextFromDataTable(txtnoise, "5154");
            SetTextFromDataTable(txttestfrequency, "5155");

            // Helper method to set text for dropdowns or textboxes
            void SetTextFromDataTable(dynamic control, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    string value = row["Result"].ToString();
                    if (control is DropDownList)
                    {
                        control.SelectedValue = value;
                    }
                    else if (control is TextBox)
                    {
                        control.Text = value;
                    }
                }
            }
        }


        // Helper method to extract parameter values from the data table
        private string GetParameterValue(DataTable data, string reportParamMapId)
        {
            foreach (DataRow row in data.Rows)
            {
                // Using ParameterID instead of ReportParamMapID to match your actual column name
                if (row["ParameterID"].ToString() == reportParamMapId)
                {
                    return row["Result"].ToString();
                }
            }
            return string.Empty;
        }
    }
}