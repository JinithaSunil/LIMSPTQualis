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
	public partial class LowVoltagecCbleLCR : System.Web.UI.Page
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
                PopulateCoreDropdown(ddlCorePhase);
                PopulateCoreDropdown(ddlCore1);


            }
        }
        private void PopulateCoreDropdown(DropDownList ddl)
        {
            try
            {
                DataTable coreData = _cm.Get_Core();
                ddl.Items.Clear();
                if (coreData != null && coreData.Rows.Count > 0)
                {
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddl.Items.Add(new ListItem(coreName, coreId));
                    }
                }
            }
            catch (Exception ex)
            {
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
        private void InitializeGrid()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("DrumNo", typeof(string));
            dt.Columns.Add("Length", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("Core1", typeof(string));      // Second dropdown
            dt.Columns.Add("Input1", typeof(decimal));
            dt.Columns.Add("Input2", typeof(decimal));
            dt.Columns.Add("Input3", typeof(decimal));
            dt.Columns.Add("Input4", typeof(decimal));
            dt.Columns.Add("Input5", typeof(decimal));

            // New columns to capture Units and Specified values
            dt.Columns.Add("UnitInd", typeof(string));
            dt.Columns.Add("UnitMaxInd", typeof(string));
            dt.Columns.Add("UnitRes", typeof(string));
            dt.Columns.Add("UnitLR", typeof(string));
            dt.Columns.Add("UnitMutual", typeof(string));

            dt.Columns.Add("SpecifiedInductance", typeof(string));
            dt.Columns.Add("SpecifiedMaxInductance", typeof(string));
            dt.Columns.Add("SpecifiedResistance", typeof(string));
            dt.Columns.Add("SpecifiedLR", typeof(string));
            dt.Columns.Add("SpecifiedMutual", typeof(string));

            ViewState["TestResults"] = dt;

            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();
        }

        private void PrefillUpperSection(DataTable existingData)
        {
            SetTextBoxText(cableType, "16074");         // Type
            SetTextBoxText(cableSize, "16075");         // Size
            SetTextBoxText(voltage, "16076");           // Voltage
            SetTextBoxText(Date, "16077");              // Date
            SetTextBoxText(roomno, "16078");            // RoomNo
            SetTextBoxText(roomtemp, "16079");          // RoomTemp
            SetTextBoxText(refSpec, "16080");           // RefSpec
            SetTextBoxText(testMethodNo, "16081");      // TestMethodNo
            SetTextBoxText(jobOrderNo, "16082");        // JobOrderNo
            SetTextBoxText(worksheetno, "16083");       // WorkSheetNo
            SetTextBoxText(roomhumidity, "16084");      // RoomHumidity
            SetTextBoxText(equipmentno, "16085");       // EquipmentNo



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
                InitializeGrid();
                if (_cm == null) _cm = new DAL.CM();

                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null) return;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
SELECT ReportParamMapID, Result 
FROM TestResultsPTQualis 
WHERE SampleID = @SampleID 
AND ReportParamMapID IN ('16068','16090','16069','16070','16071','16072','16073')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Clear();

                            while (reader.Read())
                            {
                                string paramId = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();

                                switch (paramId)
                                {
                                    case "16068": // Units
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] units = result.Split(',');
                                            if (dt.Rows.Count == 0)
                                                dt.Rows.Add(dt.NewRow());
                                            DataRow unitRow = dt.Rows[0];

                                            if (units.Length > 0) unitRow["UnitInd"] = units.ElementAtOrDefault(0) ?? "";
                                            if (units.Length > 1) unitRow["UnitMaxInd"] = units.ElementAtOrDefault(1) ?? "";
                                            if (units.Length > 2) unitRow["UnitRes"] = units.ElementAtOrDefault(2) ?? "";
                                            if (units.Length > 3) unitRow["UnitLR"] = units.ElementAtOrDefault(3) ?? "";
                                            if (units.Length > 4) unitRow["UnitMutual"] = units.ElementAtOrDefault(4) ?? "";

                                            ddlunitInd.SelectedValue = units.ElementAtOrDefault(0) ?? "";
                                            ddlunitMax.SelectedValue = units.ElementAtOrDefault(1) ?? "";
                                            ddlunitResi.SelectedValue = units.ElementAtOrDefault(2) ?? "";
                                            ddlunitLR.SelectedValue = units.ElementAtOrDefault(3) ?? "";
                                            ddlunitMutual.SelectedValue = units.ElementAtOrDefault(4) ?? "";
                                        }
                                        break;

                                    case "16090": // Specifications
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] specs = result.Split(',');
                                            if (dt.Rows.Count == 0)
                                                dt.Rows.Add(dt.NewRow());
                                            DataRow specRow = dt.Rows[0];

                                            if (specs.Length > 0) specRow["SpecifiedInductance"] = specs.ElementAtOrDefault(0) ?? "";
                                            if (specs.Length > 1) specRow["SpecifiedMaxInductance"] = specs.ElementAtOrDefault(1) ?? "";
                                            if (specs.Length > 2) specRow["SpecifiedResistance"] = specs.ElementAtOrDefault(2) ?? "";
                                            if (specs.Length > 3) specRow["SpecifiedLR"] = specs.ElementAtOrDefault(3) ?? "";
                                            if (specs.Length > 4) specRow["SpecifiedMutual"] = specs.ElementAtOrDefault(4) ?? "";

                                           
                                            txtspecifiedinductance.Text = specs.ElementAtOrDefault(0) ?? "";
                                            txtspecifiedMaximumind.Text = specs.ElementAtOrDefault(1) ?? "";
                                            txtspecifiedResistance.Text = specs.ElementAtOrDefault(2) ?? "";
                                            txtspecifiedLR.Text = specs.ElementAtOrDefault(3) ?? "";
                                            txtspecifiedMutual.Text = specs.ElementAtOrDefault(4) ?? "";
                                        }
                                        break;

                                    case "16069": // Date of test
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] dates = result.Split('|');

                                            for (int i = 0; i < dates.Length; i++)
                                            {
                                                DataRow row;
                                                if (i < dt.Rows.Count)
                                                    row = dt.Rows[i];
                                                else
                                                {
                                                    row = dt.NewRow();
                                                    dt.Rows.Add(row);
                                                }

                                                if (DateTime.TryParse(dates[i], out DateTime parsedDate))
                                                {
                                                    row["Date"] = parsedDate.ToString("dd-MM-yyyy");
                                                }
                                                else
                                                {
                                                    // If parsing fails, store original or blank
                                                    row["Date"] = dates[i];
                                                }
                                            }
                                        }
                                        break;


                                    case "16070": // DrumNo
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] drums = result.Split('|');

                                            for (int i = 0; i < drums.Length; i++)
                                            {
                                                DataRow row;
                                                if (i < dt.Rows.Count)
                                                    row = dt.Rows[i];
                                                else
                                                {
                                                    row = dt.NewRow();
                                                    dt.Rows.Add(row);
                                                }
                                                row["DrumNo"] = drums[i];
                                            }
                                        }
                                        break;

                                    case "16072": // Length
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] lengths = result.Split('|');

                                            for (int i = 0; i < lengths.Length; i++)
                                            {
                                                DataRow row;
                                                if (i < dt.Rows.Count)
                                                    row = dt.Rows[i];
                                                else
                                                {
                                                    row = dt.NewRow();
                                                    dt.Rows.Add(row);
                                                }
                                                row["Length"] = lengths[i];
                                            }
                                        }
                                        break;

                                    case "16071": // Core
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] coreGroups = result.Split('|');

                                            for (int i = 0; i < coreGroups.Length; i++)
                                            {
                                                DataRow row;
                                                if (i < dt.Rows.Count)
                                                    row = dt.Rows[i];
                                                else
                                                {
                                                    row = dt.NewRow();
                                                    dt.Rows.Add(row);
                                                }

                                                string coreValue = coreGroups[i].Trim();

                                                // Split by comma into CorePhase and Core1
                                                string[] cores = coreValue.Split(new[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);

                                                // Assign safely
                                                row["CorePhase"] = cores.Length > 0 ? cores[0].Trim() : "";
                                                row["Core1"] = cores.Length > 1 ? cores[1].Trim() : "";
                                            }
                                        }
                                        break;

                                    case "16073": // Results
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] groups = result.Split('|');

                                            for (int i = 0; i < groups.Length; i++)
                                            {
                                                DataRow row;
                                                if (i < dt.Rows.Count)
                                                    row = dt.Rows[i];
                                                else
                                                {
                                                    row = dt.NewRow();
                                                    dt.Rows.Add(row);
                                                }

                                                string[] values = groups[i].Split(',');

                                                for (int j = 0; j < Math.Min(values.Length, 5); j++)
                                                {
                                                    if (decimal.TryParse(values[j], out decimal val))
                                                        row[$"Input{j + 1}"] = val;
                                                    else
                                                        row[$"Input{j + 1}"] = DBNull.Value;
                                                }
                                            }
                                        }
                                        break;
                                   
                                }
                            }
                        }
                    }
                }

                gvPressureTest.DataSource = dt;
                gvPressureTest.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError", $"alert('Error: {ex.Message}');", true);
            }
        }
        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            if (string.IsNullOrEmpty(ddlCorePhase.SelectedValue))
            {
                
                ShowMessage("Please select a valid Core Phase.");
                return;
            }         
           
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
                ResetControlBorders();

                bool hasError = false;
                List<Control> invalidControls = new List<Control>();

                // Validate controls
                if (string.IsNullOrWhiteSpace(txtDate.Text))
                {
                    txtDate.BorderColor = System.Drawing.Color.Red;
                    hasError = true;
                }

                if (string.IsNullOrWhiteSpace(txtdrumnNo.Text))
                {
                    txtdrumnNo.BorderColor = System.Drawing.Color.Red;
                    hasError = true;
                }

                if (string.IsNullOrWhiteSpace(txtlength.Text))
                {
                    txtlength.BorderColor = System.Drawing.Color.Red;
                    hasError = true;
                }

                // Validate CorePhase
                if (ddlCorePhase.SelectedIndex < 0 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
                {
                    ddlCorePhase.CssClass += " input-error";
                    invalidControls.Add(ddlCorePhase);
                    hasError = true;
                }
                if (ddlCore1.SelectedIndex < 0 || string.IsNullOrWhiteSpace(ddlCore1.SelectedValue))
                {
                    ddlCore1.CssClass += " input-error";
                    invalidControls.Add(ddlCore1);
                    hasError = true;
                }


                // Validate numeric inputs
                TextBox[] inputs = {
            txtInput1, txtInput2, txtInput3, txtInput4, txtInput5
        };

                foreach (TextBox txt in inputs)
                {
                    if (string.IsNullOrWhiteSpace(txt.Text) || !decimal.TryParse(txt.Text.Trim(), out _))
                    {
                        txt.BorderColor = System.Drawing.Color.Red;
                        hasError = true;
                    }
                }

                // Validate Specified Fields (optional but recommended)
                if (string.IsNullOrWhiteSpace(txtspecifiedinductance.Text) ||
                    string.IsNullOrWhiteSpace(txtspecifiedMaximumind.Text) ||
                    string.IsNullOrWhiteSpace(txtspecifiedResistance.Text) ||
                    string.IsNullOrWhiteSpace(txtspecifiedLR.Text) ||
                    string.IsNullOrWhiteSpace(txtspecifiedMutual.Text))
                {
                    ShowMessage("Please fill in all specified fields.");
                    hasError = true;
                }

                if (hasError)
                {
                    ShowMessage("Please fill in all required fields correctly.");
                    return;
                }

                // Get or create DataTable
                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Date", typeof(string));
                    dt.Columns.Add("DrumNo", typeof(string));
                    dt.Columns.Add("Length", typeof(string));
                    dt.Columns.Add("CorePhase", typeof(string));
                    dt.Columns.Add("Core1", typeof(string));
                    dt.Columns.Add("Input1", typeof(decimal));
                    dt.Columns.Add("Input2", typeof(decimal));
                    dt.Columns.Add("Input3", typeof(decimal));
                    dt.Columns.Add("Input4", typeof(decimal));
                    dt.Columns.Add("Input5", typeof(decimal));

                    // Add columns for unit and specified values
                    dt.Columns.Add("UnitInd", typeof(string));
                    dt.Columns.Add("UnitMaxInd", typeof(string));
                    dt.Columns.Add("UnitRes", typeof(string));
                    dt.Columns.Add("UnitLR", typeof(string));
                    dt.Columns.Add("UnitMutual", typeof(string));

                    dt.Columns.Add("SpecifiedInductance", typeof(string));
                    dt.Columns.Add("SpecifiedMaxInductance", typeof(string));
                    dt.Columns.Add("SpecifiedResistance", typeof(string));
                    dt.Columns.Add("SpecifiedLR", typeof(string));
                    dt.Columns.Add("SpecifiedMutual", typeof(string));
                }

                // Create new row
                DataRow dr = dt.NewRow();
                DateTime parsedDate;
                if (DateTime.TryParse(txtDate.Text.Trim(), out parsedDate))
                {
                    dr["Date"] = parsedDate.ToString("dd-MM-yyyy");
                }
                else
                {
                    dr["Date"] = txtDate.Text.Trim(); 
                }

                dr["DrumNo"] = txtdrumnNo.Text.Trim();
                dr["Length"] = txtlength.Text.Trim();
                dr["CorePhase"] = ddlCorePhase.SelectedItem.Text;
                dr["Core1"] = ddlCore1.SelectedItem.Text;
                dr["Input1"] = Convert.ToDecimal(txtInput1.Text.Trim());
                dr["Input2"] = Convert.ToDecimal(txtInput2.Text.Trim());
                dr["Input3"] = Convert.ToDecimal(txtInput3.Text.Trim());
                dr["Input4"] = Convert.ToDecimal(txtInput4.Text.Trim());
                dr["Input5"] = Convert.ToDecimal(txtInput5.Text.Trim());

                // Add unit and specified values
                dr["UnitInd"] = ddlunitInd.SelectedValue;
                dr["UnitMaxInd"] = ddlunitMax.SelectedValue;
                dr["UnitRes"] = ddlunitResi.SelectedValue;
                dr["UnitLR"] = ddlunitLR.SelectedValue;
                dr["UnitMutual"] = ddlunitMutual.SelectedValue;

                dr["SpecifiedInductance"] = txtspecifiedinductance.Text.Trim();
                dr["SpecifiedMaxInductance"] = txtspecifiedMaximumind.Text.Trim();
                dr["SpecifiedResistance"] = txtspecifiedResistance.Text.Trim();
                dr["SpecifiedLR"] = txtspecifiedLR.Text.Trim();
                dr["SpecifiedMutual"] = txtspecifiedMutual.Text.Trim();

                // Add to table
                dt.Rows.Add(dr);
                ViewState["TestResults"] = dt;

                // Rebind to GridView
                gvPressureTest.DataSource = dt;
                gvPressureTest.DataBind();

                // Lock unit and specified fields after first entry
                if (dt.Rows.Count > 0)
                {
                    ddlunitInd.Enabled = false;
                    ddlunitMax.Enabled = false;
                    ddlunitResi.Enabled = false;
                    ddlunitLR.Enabled = false;
                    ddlunitMutual.Enabled = false;

                    txtspecifiedinductance.ReadOnly = true;
                    txtspecifiedMaximumind.ReadOnly = true;
                    txtspecifiedResistance.ReadOnly = true;
                    txtspecifiedLR.ReadOnly = true;
                    txtspecifiedMutual.ReadOnly = true;
                }

                // Optionally clear entry fields
                ClearInputFields();
            }
        protected void btnClearData_Click(object sender, EventArgs e)
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

            if (txtspecifiedinductance.Text.Trim() != "-" && (string.IsNullOrEmpty(txtspecifiedinductance.Text) || !double.TryParse(txtspecifiedinductance.Text, out _)))
            {
                errors.Add("Specific inductance  must be a valid number or '-'.");
                invalidControls.Add(txtspecifiedinductance);
            }

            if (txtspecifiedMaximumind.Text.Trim() != "-" && (string.IsNullOrEmpty(txtspecifiedMaximumind.Text) || !double.TryParse(txtspecifiedMaximumind.Text, out _)))
            {
                errors.Add("Specific Maximum inductance must be a valid number or '-'.");
                invalidControls.Add(txtspecifiedMaximumind);
            }


            if (txtspecifiedResistance.Text.Trim() != "-" && (string.IsNullOrEmpty(txtspecifiedResistance.Text) || !double.TryParse(txtspecifiedResistance.Text, out _)))
            {
                errors.Add("Specific Resistance  must be a valid number or '-'.");
                invalidControls.Add(txtspecifiedResistance);
            }




            if (txtspecifiedLR.Text.Trim() != "-" && (string.IsNullOrEmpty(txtspecifiedLR.Text) || !double.TryParse(txtspecifiedLR.Text, out _)))
            {
                errors.Add("Specific  L/R Ratio must be a valid number or '-'.");
                invalidControls.Add(txtspecifiedLR);
            }


            if (txtspecifiedMutual.Text.Trim() != "-" && (string.IsNullOrEmpty(txtspecifiedMutual.Text) || !double.TryParse(txtspecifiedMutual.Text, out _)))
            {
                errors.Add("Specific Mutual capacitance  must be a valid number or '-'.");
                invalidControls.Add(txtspecifiedMutual);
            }
            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                // Validate Test Details Input 
                if (string.IsNullOrEmpty(txtlength.Text) ||
                    (txtlength.Text.Trim() != "-" && !double.TryParse(txtlength.Text.Trim(), out _)))
                {
                    errors.Add("Length must be a valid number or '-'.");
                    invalidControls.Add(txtlength);
                }

                if (string.IsNullOrEmpty(txtdrumnNo.Text) ||
                    (txtdrumnNo.Text.Trim() != "-" && !double.TryParse(txtdrumnNo.Text.Trim(), out _)))
                {
                    errors.Add("Drumn No must be a valid number or '-'.");
                    invalidControls.Add(txtdrumnNo);
                }

                if (string.IsNullOrEmpty(txtDate.Text) ||
                    (txtDate.Text.Trim() != "-" && !double.TryParse(txtDate.Text.Trim(), out _)))
                {
                    errors.Add("Data must be in Datee formate or '-'.");
                    invalidControls.Add(txtDate);
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
        protected void gvPressureTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
            DateTime dtTest;
            TableCell dateCell = e.Row.Cells[0];

            if (DateTime.TryParse(dateCell.Text, out dtTest))
            {
                dateCell.Text = dtTest.ToString("dd-MM-yyyy");
            }
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null) return;


          
        }
        protected void gvPressureTest_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    gvPressureTest.DataSource = dt;
                    gvPressureTest.DataBind();


                    // Show confirmation message
                    ShowMessage("Record deleted successfully.");
                }
            }
        }
        // Helper method
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
            txtdrumnNo.Text = "";
           txtDate.Text = "";
            txtlength.Text = "";
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";

           
            // Clear GridView
            gvPressureTest.DataSource = null;
            gvPressureTest.DataBind();


            // Standard Values
            txtspecifiedMutual.Text = "";
            txtspecifiedinductance.Text = "";
            txtspecifiedMaximumind.Text = "";
            txtspecifiedLR.Text = " ";
            txtspecifiedResistance.Text = "";
           
          
        }
        private void ClearInputFields()
        {
        
            txtdrumnNo.Text = "";
            txtDate.Text = "";
            txtlength.Text = "";

            ddlCorePhase.SelectedIndex = 0;
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
          
        }
        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{safeMessage}');", true);
        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();


            //  allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);

         
            allTextBoxes.Add(txtlength);
            allTextBoxes.Add(txtdrumnNo);
            allTextBoxes.Add(txtDate);
         

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
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
                if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"]);
                if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                string units = $"{ddlunitInd.SelectedValue},{ddlunitMax.SelectedValue},{ddlunitResi.SelectedValue},{ddlunitLR.SelectedValue},{ddlunitMutual.SelectedValue}";
                reportParamMapList.Add("16068");
                resultList.Add(units);

                string specifiedValues = $"{txtspecifiedinductance.Text},{txtspecifiedMaximumind.Text},{txtspecifiedResistance.Text},{txtspecifiedLR.Text},{txtspecifiedMutual.Text}";
                reportParamMapList.Add("16090");
                resultList.Add(specifiedValues);

                // 16069 - Date of test (date|date|...)
                List<string> dateList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(row["Date"]?.ToString()))
                        dateList.Add(row["Date"].ToString());
                }
                if (dateList.Any())
                {
                    reportParamMapList.Add("16069");
                    resultList.Add(string.Join("|", dateList));
                }

                // 16070 - DrumNo (drumno|drumno|...)
                List<string> drumNoList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(row["DrumNo"]?.ToString()))
                        drumNoList.Add(row["DrumNo"].ToString());
                }
                if (drumNoList.Any())
                {
                    reportParamMapList.Add("16070");
                    resultList.Add(string.Join("|", drumNoList));
                }

                // New code: get the combined CorePhase,Core1 string directly from the datatable
                List<string> coreList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string corePhase = row["CorePhase"]?.ToString().Trim();
                    string core1 = row["Core1"]?.ToString().Trim();

                    string combinedCore;
                    if (string.IsNullOrEmpty(corePhase) && string.IsNullOrEmpty(core1))
                    {
                        combinedCore = "";
                    }
                    else if (string.IsNullOrEmpty(corePhase))
                    {
                        combinedCore = core1;
                    }
                    else if (string.IsNullOrEmpty(core1))
                    {
                        combinedCore = corePhase;
                    }
                    else
                    {
                        combinedCore = corePhase + "," + core1;
                    }

                    coreList.Add(combinedCore);

                }

                if (coreList.Any())
                {
                    reportParamMapList.Add("16071");
                    resultList.Add(string.Join("|", coreList));
                }


                // 16072 - Length (length|length|...)
                List<string> lengthList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(row["Length"]?.ToString()))
                        lengthList.Add(row["Length"].ToString());
                }
                if (lengthList.Any())
                {
                    reportParamMapList.Add("16072");
                    resultList.Add(string.Join("|", lengthList));
                }

                // 16073 - Results (txtinput1,txtinput2,txtinput3,txtinput4,txtinput5|txtinput1,...)
                List<string> resultsList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string inputs = $"{row["Input1"]?.ToString()},{row["Input2"]?.ToString()}," +
                                    $"{row["Input3"]?.ToString()},{row["Input4"]?.ToString()}," +
                                    $"{row["Input5"]?.ToString()}";
                    resultsList.Add(inputs);
                }
                if (resultsList.Any())
                {
                    reportParamMapList.Add("16073");
                    resultList.Add(string.Join("|", resultsList));
                }

                // Additional Form Fields
                reportParamMapList.Add("16074"); resultList.Add(cableType.Text);        // Type
                reportParamMapList.Add("16075"); resultList.Add(cableSize.Text);        // Size
                reportParamMapList.Add("16076"); resultList.Add(voltage.Text);          // Voltage
                reportParamMapList.Add("16077"); resultList.Add(Date.Text);             // Date
                reportParamMapList.Add("16078"); resultList.Add(roomno.Text);           // RoomNo
                reportParamMapList.Add("16079"); resultList.Add(roomtemp.Text);         // RoomTemp
                reportParamMapList.Add("16080"); resultList.Add(refSpec.Text);          // RefSpec
                reportParamMapList.Add("16081"); resultList.Add(testMethodNo.Text);     // TestMethodNo
                reportParamMapList.Add("16082"); resultList.Add(jobOrderNo.Text);       // JobOrderNo
                reportParamMapList.Add("16083"); resultList.Add(worksheetno.Text);      // WorkSheetNo
                reportParamMapList.Add("16084"); resultList.Add(roomhumidity.Text);     // RoomHumidity
                reportParamMapList.Add("16085"); resultList.Add(equipmentno.Text);      // EquipmentNo
                // SUBMIT
                int modified = _cm.AddTestResultPTQualis(_cm,
                                    reportParamMapList.ToArray(),
                                    resultList.ToArray());

                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results and Report Generated successfully!');", true);
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);
                    Response.Redirect($"GenerateTestReport_LowVoltageCableLCR.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

                

                // Validate your new input textboxes here (thickness and diameter)
                TextBox[] newTestInputs = {
             txtInput1, txtInput2, txtInput3, txtInput4, txtInput5,
         
            txtlength, txtDate,
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

                // 1st Header Row - Categories
                GridViewRow row1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row1.Cells.Add(CreateHeaderCell("ITEM TEST", 0, 4));
                row1.Cells.Add(CreateHeaderCell("Inductance", 0, 1));
                row1.Cells.Add(CreateHeaderCell("Maximum Inductance", 0, 1));
                row1.Cells.Add(CreateHeaderCell("Resistance", 0, 1));
                row1.Cells.Add(CreateHeaderCell("L/R Ratio", 0, 1));
                row1.Cells.Add(CreateHeaderCell("Mutual Capacitance", 0, 1));
                row1.Cells.Add(CreateHeaderCell("Actions", 4,0));
                gv.Controls[0].Controls.AddAt(0, row1);

                // --- Get selected units from dropdowns ---
                string unitInd = ddlunitInd?.SelectedValue ?? "";
                string unitMax = ddlunitMax?.SelectedValue ?? "";
                string unitResi = ddlunitResi?.SelectedValue ?? "";
                string unitLR = ddlunitLR?.SelectedValue ?? "";
                string unitMutual = ddlunitMutual?.SelectedValue ?? "";

                // 2nd Header Row - Units
                GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row2.Cells.Add(CreateHeaderCell("Unit", 0, 4));
                row2.Cells.Add(CreateHeaderCell(unitInd, 0, 1));
                row2.Cells.Add(CreateHeaderCell(unitMax, 0, 1));
                row2.Cells.Add(CreateHeaderCell(unitResi, 0, 1));
                row2.Cells.Add(CreateHeaderCell(unitLR, 0, 1));
                row2.Cells.Add(CreateHeaderCell(unitMutual, 0, 1));
                //row2.Cells.Add(CreateHeaderCell("", 0, 1));
                gv.Controls[0].Controls.AddAt(1, row2);

                // --- Get specified values from textboxes ---
                string specInd = txtspecifiedinductance?.Text ?? "";
                string specMax = txtspecifiedMaximumind?.Text ?? "";
                string specResi = txtspecifiedResistance?.Text ?? "";
                string specLR = txtspecifiedLR?.Text ?? "";
                string specMutual = txtspecifiedMutual?.Text ?? "";

                // 3rd Header Row - Specified Values
                GridViewRow row3 = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row3.Cells.Add(CreateHeaderCell("Specified", 0, 4));
                row3.Cells.Add(CreateHeaderCell(specInd, 0, 1));
                row3.Cells.Add(CreateHeaderCell(specMax, 0, 1));
                row3.Cells.Add(CreateHeaderCell(specResi, 0, 1));
                row3.Cells.Add(CreateHeaderCell(specLR, 0, 1));
                row3.Cells.Add(CreateHeaderCell(specMutual, 0, 1));
                //row3.Cells.Add(CreateHeaderCell("", 0, 1));
                gv.Controls[0].Controls.AddAt(2, row3);

                // 4th Header Row - Column Titles
                GridViewRow row4 = new GridViewRow(3, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row4.Cells.Add(CreateHeaderCell("Date", 2, 1));
                row4.Cells.Add(CreateHeaderCell("Drum No.", 2, 1));
                row4.Cells.Add(CreateHeaderCell("Length", 2, 1));
                row4.Cells.Add(CreateHeaderCell("Core No/Pair", 2, 1));
                row4.Cells.Add(CreateHeaderCell("Test Result", 0, 5));
           //     row4.Cells.Add(CreateHeaderCell("Actions", 2, 1));
                gv.Controls[0].Controls.AddAt(3, row4);
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