using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class PressureTestAtHighTemp_Revision : System.Web.UI.Page
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
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
         
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(int));
            dt.Columns.Add("Weightload", typeof(string));
            dt.Columns.Add("ThicknessatTemperature", typeof(string));
            dt.Columns.Add("DeformationTemperature", typeof(decimal));
            dt.Columns.Add("Median", typeof(decimal));

            // Add the 6 additional input columns
            dt.Columns.Add("InputthicknessInput1", typeof(decimal));
            dt.Columns.Add("InputthicknessInput2", typeof(decimal));
            dt.Columns.Add("InputthicknessInput3", typeof(decimal));
            dt.Columns.Add("InputthicknessInput4", typeof(decimal));
            dt.Columns.Add("InputthicknessInput5", typeof(decimal));
            dt.Columns.Add("InputthicknessInput6", typeof(decimal));
            dt.Columns.Add("InputthicknessInputAvg", typeof(decimal));
            dt.Columns.Add("InputdiameterInput1" ,typeof(decimal));
            dt.Columns.Add("InputdiameterInput2" ,typeof(decimal));
            dt.Columns.Add("InputdiameterInput3" ,typeof(decimal));
            dt.Columns.Add("InputdiameterInput4" ,typeof(decimal));
            dt.Columns.Add("InputdiameterInput5" ,typeof(decimal));
            dt.Columns.Add("InputdiameterInput6" ,typeof(decimal));
            dt.Columns.Add("InputdiameterInputAvg" ,typeof(decimal));

            dt.Columns.Add("aftertemp",typeof(string));
            dt.Columns.Add("underloadatTemp", typeof(string));
            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();
   
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetTextBoxText(cableType, "13067");
            SetTextBoxText(cableSize, "13068");
            SetTextBoxText(voltage, "13069");
            SetTextBoxText(Date, "13070");
            SetTextBoxText(roomno, "13072");
            SetTextBoxText(roomtemp, "13073");
            SetTextBoxText(refSpec, "13074");
            SetTextBoxText(testMethodNo, "13075");
            SetTextBoxText(jobOrderNo, "13076");
            SetTextBoxText(worksheetno, "13077");
            SetTextBoxText(roomhumidity, "13078");
            SetTextBoxText(equipmentno, "13079");


            SetTextBoxText(txtSpecificThickness,"13062");
            SetTextBoxText(txtSpecificDiameter,"13063");
            SetTextBoxText(txtSpecificMedian,"14090");
            SetTextBoxText(txtSpecificWeightload,"13064");
            SetTextBoxText(txtSpecific6hourThickness,"13065");
            SetTextBoxText(txtSpecificDeformation, "13066");

            SetTextBoxText(txtaftertemp, "13054");
            SetTextBoxText(txtunderloadatTemp, "13055");
         

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
            txtWeightload.Text = "";
            txtThicknessatTemperature.Text = "";
            txtDeformationTemperature.Text = "";

            txtWeightload.Enabled = true;
            txtThicknessatTemperature.Enabled = true;
            txtDeformationTemperature.Enabled = true;
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
AND ReportParamMapID IN ('13057','13058','13059','13060','13061','14087','14088','14089')";

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
                                    //case "13054": // AfterTemp
                                    //    txtaftertemp.Text = result;
                                    //    break;

                                    //case "13055": // UnderLoadAtTemp  
                                    //    txtunderloadatTemp.Text = result;
                                    //    break;

                                    case "13057": // Thickness data
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] coreData = result.Split('|');
                                            foreach (string coreEntry in coreData)
                                            {
                                                string[] parts = coreEntry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string[] values = specimenParts[1].Split(',');

                                                // Find or create row
                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                // Fill thickness inputs
                                                for (int i = 0; i < Math.Min(values.Length, 6); i++)
                                                {
                                                    row[$"InputthicknessInput{i + 1}"] = values[i];
                                                }
                                            }
                                        }
                                        break;

                                    case "13058": // ThicknessAvg
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] avgValues = result.Split('|');
                                            var thicknessRows = dt.Rows.Cast<DataRow>().Where(r => !string.IsNullOrEmpty(r["InputthicknessInput1"].ToString())).ToArray();
                                            for (int i = 0; i < Math.Min(avgValues.Length, thicknessRows.Length); i++)
                                            {
                                                if (decimal.TryParse(avgValues[i], out decimal avg))
                                                    thicknessRows[i]["InputthicknessInputAvg"] = avg;
                                            }
                                        }
                                        break;

                                    case "13059": // Diameter data
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] coreData = result.Split('|');
                                            foreach (string coreEntry in coreData)
                                            {
                                                string[] parts = coreEntry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string[] values = specimenParts[1].Split(',');

                                                // Find or create row
                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                // Fill diameter inputs
                                                for (int i = 0; i < Math.Min(values.Length, 6); i++)
                                                {
                                                    row[$"InputdiameterInput{i + 1}"] = values[i];
                                                }
                                            }
                                        }
                                        break;

                                    case "13060": // DiameterAvg
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] avgValues = result.Split('|');
                                            var diameterRows = dt.Rows.Cast<DataRow>().Where(r => !string.IsNullOrEmpty(r["InputdiameterInput1"].ToString())).ToArray();
                                            for (int i = 0; i < Math.Min(avgValues.Length, diameterRows.Length); i++)
                                            {
                                                if (decimal.TryParse(avgValues[i], out decimal avg))
                                                    diameterRows[i]["InputdiameterInputAvg"] = avg;
                                            }
                                        }
                                        break;

                                    case "13061": // WeightLoad
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row != null) row["Weightload"] = value;
                                            }
                                        }
                                        break;

                                    case "14087": // ThicknessTemperature
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row != null) row["ThicknessatTemperature"] = value;
                                            }
                                        }
                                        break;

                                    case "14088": // Deformation
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row != null && decimal.TryParse(value, out decimal decValue))
                                                    row["DeformationTemperature"] = decValue;
                                            }
                                        }
                                        break;

                                    case "14089": // Median
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row != null && decimal.TryParse(value, out decimal decValue))
                                                    row["Median"] = decValue;
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
            // Store current values in ViewState before the dropdown change processing
            ViewState["AfterTemp"] = txtaftertemp.Text;
            ViewState["UnderloadTemp"] = txtunderloadatTemp.Text;

            if (string.IsNullOrEmpty(ddlCorePhase.SelectedValue))
            {
                txtSpecimenNo.Text = "";
                ShowMessage("Please select a valid Core Phase.");
                return;
            }

            txtSpecimenNo.Text = "1";

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
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0;

            // Validate CorePhase
            if (ddlCorePhase.SelectedIndex < 0 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                ddlCorePhase.CssClass += " input-error";
                invalidControls.Add(ddlCorePhase);
                hasError = true;
            }

            // Validate SpecimenNo
            if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out specimenNo))
            {
                invalidControls.Add(txtSpecimenNo);
                hasError = true;
            }

            // Validate and calculate averages (assuming "-" allowed only for diameter inputs)
            TextBox[] allInputs = {
        txtthicknessInput1, txtthicknessInput2, txtthicknessInput3, txtthicknessInput4, txtthicknessInput5, txtthicknessInput6,
        txtdiameterInput1, txtdiameterInput2, txtdiameterInput3, txtdiameterInput4, txtdiameterInput5, txtdiameterInput6,
        txtthicknessInputAvg, txtdiameterInputAvg,
        txtWeightload, txtThicknessatTemperature, txtDeformationTemperature
    };

            foreach (TextBox txt in allInputs)
            {
                string val = txt.Text.Trim();
                // "-" allowed only for diameter inputs
                bool allowDash = txt.ID.StartsWith("txtdiameterInput");

                if (string.IsNullOrWhiteSpace(val) || (!allowDash && !decimal.TryParse(val, out _)) || (allowDash && val != "-" && !decimal.TryParse(val, out _)))
                {
                    invalidControls.Add(txt);
                    hasError = true;
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

            // Get or initialize DataTable
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("Weightload", typeof(string));
                dt.Columns.Add("ThicknessatTemperature", typeof(string));
                dt.Columns.Add("DeformationTemperature", typeof(decimal));
                dt.Columns.Add("Median", typeof(decimal));

                dt.Columns.Add("InputthicknessInput1", typeof(string));
                dt.Columns.Add("InputthicknessInput2", typeof(string));
                dt.Columns.Add("InputthicknessInput3", typeof(string));
                dt.Columns.Add("InputthicknessInput4", typeof(string));
                dt.Columns.Add("InputthicknessInput5", typeof(string));
                dt.Columns.Add("InputthicknessInput6", typeof(string));
                dt.Columns.Add("InputthicknessInputAvg", typeof(decimal));

                dt.Columns.Add("InputdiameterInput1", typeof(decimal));
                dt.Columns.Add("InputdiameterInput2", typeof(decimal));
                dt.Columns.Add("InputdiameterInput3", typeof(decimal));
                dt.Columns.Add("InputdiameterInput4", typeof(decimal));
                dt.Columns.Add("InputdiameterInput5", typeof(decimal));
                dt.Columns.Add("InputdiameterInput6", typeof(decimal));
                dt.Columns.Add("InputdiameterInputAvg", typeof(decimal));

                dt.Columns.Add("aftertemp", typeof(string));
                dt.Columns.Add("underloadatTemp", typeof(string));
            }

            // Check for duplicate SpecimenNo + CorePhase
            bool duplicateExists = dt.AsEnumerable().Any(row =>
                row["CorePhase"].ToString() == ddlCorePhase.SelectedItem.Text &&
                Convert.ToInt32(row["SpecimenNo"]) == specimenNo
            );

            if (duplicateExists)
            {
                ShowMessage($"Data for Core Phase '{ddlCorePhase.SelectedItem.Text}' and Specimen No. '{specimenNo}' already exists. Please delete it before adding again.");
                txtSpecimenNo.Text = (specimenNo + 1).ToString();
                return;
            }

            // Create new row
            DataRow dr = dt.NewRow();

            dr["CorePhase"] = ddlCorePhase.SelectedItem.Text;
            dr["SpecimenNo"] = specimenNo;
            dr["Weightload"] = txtWeightload.Text.Trim();
            dr["ThicknessatTemperature"] = txtThicknessatTemperature.Text.Trim();
            dr["DeformationTemperature"] = decimal.Parse(txtDeformationTemperature.Text.Trim());

            dr["aftertemp"] = txtaftertemp.Text.Trim();
            dr["underloadatTemp"] = txtunderloadatTemp.Text.Trim();

            // Thickness inputs as strings
            dr["InputthicknessInput1"] = txtthicknessInput1.Text.Trim();
            dr["InputthicknessInput2"] = txtthicknessInput2.Text.Trim();
            dr["InputthicknessInput3"] = txtthicknessInput3.Text.Trim();
            dr["InputthicknessInput4"] = txtthicknessInput4.Text.Trim();
            dr["InputthicknessInput5"] = txtthicknessInput5.Text.Trim();
            dr["InputthicknessInput6"] = txtthicknessInput6.Text.Trim();

            // Thickness Avg as decimal
            dr["InputthicknessInputAvg"] = decimal.Parse(txtthicknessInputAvg.Text.Trim());

            // Diameter inputs as decimals, convert "-" to 0
            dr["InputdiameterInput1"] = Convert.ToDecimal(txtdiameterInput1.Text.Trim() == "-" ? "0" : txtdiameterInput1.Text.Trim());
            dr["InputdiameterInput2"] = Convert.ToDecimal(txtdiameterInput2.Text.Trim() == "-" ? "0" : txtdiameterInput2.Text.Trim());
            dr["InputdiameterInput3"] = Convert.ToDecimal(txtdiameterInput3.Text.Trim() == "-" ? "0" : txtdiameterInput3.Text.Trim());
            dr["InputdiameterInput4"] = Convert.ToDecimal(txtdiameterInput4.Text.Trim() == "-" ? "0" : txtdiameterInput4.Text.Trim());
            dr["InputdiameterInput5"] = Convert.ToDecimal(txtdiameterInput5.Text.Trim() == "-" ? "0" : txtdiameterInput5.Text.Trim());
            dr["InputdiameterInput6"] = Convert.ToDecimal(txtdiameterInput6.Text.Trim() == "-" ? "0" : txtdiameterInput6.Text.Trim());
            dr["InputdiameterInputAvg"] = decimal.Parse(txtdiameterInputAvg.Text.Trim());

            // Add the new row to DataTable
            dt.Rows.Add(dr);

            // Calculate and update Median per CorePhase group in DataTable
            foreach (var group in dt.AsEnumerable().GroupBy(r => r.Field<string>("CorePhase")))
            {
                var deformationTemps = group
                    .Where(r => !r.IsNull("DeformationTemperature"))
                    .Select(r => r.Field<decimal>("DeformationTemperature"))
                    .Where(d => d > 0)
                    .OrderBy(d => d)
                    .ToList();

                string medianValue = CalculateMedian(deformationTemps);

                decimal medianDecimal = decimal.TryParse(medianValue, out var medVal) ? medVal : 0m;

                foreach (var row in group)
                {
                    row["Median"] = medianDecimal;
                }
            }

            // Save updated DataTable to ViewState
            ViewState["TestResults"] = dt;

          
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();
          decimal maxMedian = dt.AsEnumerable()
                .Select(row => row.Field<decimal>("Median"))
                .DefaultIfEmpty(0)
                .Max();
            txtSpecificMedian.Text = maxMedian.ToString("F2");

            txtSpecimenNo.Text = (specimenNo + 1).ToString();

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


            if (txtSpecific6hourThickness.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecific6hourThickness.Text) || !double.TryParse(txtSpecific6hourThickness.Text, out _)))
            {
                errors.Add("Specific Thickness  must be a valid number or '-'.");
                invalidControls.Add(txtSpecific6hourThickness);
            }

            if (txtSpecificDeformation.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificDeformation.Text) || !double.TryParse(txtSpecificDeformation.Text, out _)))
            {
                errors.Add("Specific Deformation  must be a valid number or '-'.");
                invalidControls.Add(txtSpecific6hourThickness);
            }


            if (txtSpecificDeformation.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificDeformation.Text) || !double.TryParse(txtSpecificDeformation.Text, out _)))
            {
                errors.Add("Specific Deformation  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificDeformation);
            }




            if (txtSpecificDiameter.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificDiameter.Text) || !double.TryParse(txtSpecificDiameter.Text, out _)))
            {
                errors.Add("Specific Diameter  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificDiameter);
            }


            if (txtSpecificMedian.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificMedian.Text) || !double.TryParse(txtSpecificMedian.Text, out _)))
            {
                errors.Add("Specific Median  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificMedian);
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

                if (string.IsNullOrEmpty(txtThicknessatTemperature.Text) ||
                    (txtThicknessatTemperature.Text.Trim() != "-" && !double.TryParse(txtThicknessatTemperature.Text.Trim(), out _)))
                {
                    errors.Add("Thickness must be a valid number or '-'.");
                    invalidControls.Add(txtThicknessatTemperature);
                }

                if (string.IsNullOrEmpty(txtDeformationTemperature.Text) ||
                    (txtDeformationTemperature.Text.Trim() != "-" && !double.TryParse(txtDeformationTemperature.Text.Trim(), out _)))
                {
                    errors.Add("Deformation Temperature must be a valid number or '-'.");
                    invalidControls.Add(txtDeformationTemperature);
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

            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null) return;

            DataRowView drv = (DataRowView)e.Row.DataItem;
            string currentCore = drv["CorePhase"].ToString();
            int specimenNo = Convert.ToInt32(drv["SpecimenNo"]);

          
            var rowsForCore = dt.AsEnumerable()
                .Where(r => r.Field<string>("CorePhase") == currentCore)
                .OrderBy(r => r.Field<int>("SpecimenNo"))
                .ToList();

            int currentIndex = rowsForCore.FindIndex(r => r.Field<int>("SpecimenNo") == specimenNo);

            int corePhaseColIndex = GetColumnIndexByName("CorePhase");
            int medianColIndex = GetColumnIndexByName("Median");

            if (corePhaseColIndex == -1 || medianColIndex == -1)
                return; // Column not found

            TableCell coreCell = e.Row.Cells[corePhaseColIndex];
            TableCell medianCell = e.Row.Cells[medianColIndex];

            if (currentIndex == 0)
            {
                int rowSpanCount = rowsForCore.Count;
                coreCell.RowSpan = rowSpanCount;
                medianCell.RowSpan = rowSpanCount;

                decimal median = drv.Row.Field<decimal>("Median");
                medianCell.Text = median.ToString("F2");
            }
            else
            {
                coreCell.Visible = false;
                medianCell.Visible = false;
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
        protected void ThicknessInputs_TextChanged(object sender, EventArgs e)
        {
            string[] inputs = new string[] {
        txtthicknessInput1.Text.Trim(),
        txtthicknessInput2.Text.Trim(),
        txtthicknessInput3.Text.Trim(),
        txtthicknessInput4.Text.Trim(),
        txtthicknessInput5.Text.Trim(),
        txtthicknessInput6.Text.Trim()
    };

            decimal sum = 0;
            int count = 0;

            foreach (string input in inputs)
            {
                if (input == "-")
                {
                    count++; // "-" is treated as 0
                }
                else if (decimal.TryParse(input, out decimal value))
                {
                    sum += value;
                    count++;
                }
            }

            txtthicknessInputAvg.Text = (count > 0) ? Math.Round(sum / count, 2).ToString() : "0";
        }
        protected void DiameterInputs_TextChanged(object sender, EventArgs e)
        {
            string[] inputs = new string[] {
        txtdiameterInput1.Text.Trim(),
        txtdiameterInput2.Text.Trim(),
        txtdiameterInput3.Text.Trim(),
        txtdiameterInput4.Text.Trim(),
        txtdiameterInput5.Text.Trim(),
        txtdiameterInput6.Text.Trim()
    };

            decimal sum = 0;
            int count = 0;

            foreach (string input in inputs)
            {
                if (input == "-")
                {
                    count++; // "-" is treated as 0
                }
                else if (decimal.TryParse(input, out decimal value))
                {
                    sum += value;
                    count++;
                }
            }

            txtdiameterInputAvg.Text = (count > 0) ? Math.Round(sum / count, 2).ToString() : "0";
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
            txtthicknessInput1.Text = "";
            txtthicknessInput2.Text = "";
            txtthicknessInput3.Text = "";
            txtthicknessInput4.Text = "";
            txtthicknessInput5.Text = "";
            txtthicknessInput6.Text = "";
             txtthicknessInputAvg.Text = "";

            txtdiameterInput1.Text = "";
            txtdiameterInput2.Text = "";
            txtdiameterInput3.Text = "";
            txtdiameterInput4.Text = "";
            txtdiameterInput5.Text = "";
            txtdiameterInput6.Text = "";
            txtdiameterInputAvg.Text = "";
           
            // Clear GridView
            gvPressureTest.DataSource = null;
            gvPressureTest.DataBind();
           

            // Standard Values
            txtSpecificThickness.Text = "";
            txtSpecificDiameter.Text = "";
            txtSpecificWeightload.Text = "";
            txtSpecific6hourThickness.Text = " ";
            txtSpecificDeformation.Text = "";
            txtSpecificMedian.Text = "";
            txtWeightload.Text = "";
            txtThicknessatTemperature.Text = "";
            txtDeformationTemperature.Text = "";

        }
        private void ClearInputFields()
        {
            string afterTemp = txtaftertemp.Text;
            string underloadTemp = txtunderloadatTemp.Text;
            txtWeightload.Text = "";
            txtThicknessatTemperature.Text = "";
            txtDeformationTemperature.Text = "";
            txtthicknessInput1.Text = "";
            txtthicknessInput2.Text = "";
            txtthicknessInput3.Text = "";
            txtthicknessInput4.Text = "";
            txtthicknessInput5.Text = "";
            txtthicknessInput6.Text = "";
            txtthicknessInputAvg.Text = "";

            txtdiameterInput1.Text = "";
            txtdiameterInput2.Text = "";
            txtdiameterInput3.Text = "";
            txtdiameterInput4.Text = "";
            txtdiameterInput5.Text = "";
            txtdiameterInput6.Text = "";
            txtdiameterInputAvg.Text = "";
          
            // txtMedian.Text = "";
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

            allTextBoxes.Add(txtthicknessInputAvg);
            allTextBoxes.Add(txtdiameterInputAvg);
            allTextBoxes.Add(txtWeightload);
            allTextBoxes.Add(txtThicknessatTemperature);
            allTextBoxes.Add(txtDeformationTemperature);

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

                // THICKNESS SPECIMENS
                List<string> thicknessSpecimens = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string core = row["CorePhase"].ToString();
                    int specimen = Convert.ToInt32(row["SpecimenNo"]);
                    string inputs = $"{row["InputthicknessInput1"]},{row["InputthicknessInput2"]}," +
                                    $"{row["InputthicknessInput3"]},{row["InputthicknessInput4"]}," +
                                    $"{row["InputthicknessInput5"]},{row["InputthicknessInput6"]}";
                    thicknessSpecimens.Add($"{core};{specimen}:{inputs}");
                }
                if (thicknessSpecimens.Any())
                {
                    reportParamMapList.Add("13057"); // Thickness
                    resultList.Add(string.Join("|", thicknessSpecimens));
                }

                // DIAMETER SPECIMENS
                List<string> diameterSpecimens = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string core = row["CorePhase"].ToString();
                    int specimen = Convert.ToInt32(row["SpecimenNo"]);
                    string inputs = $"{row["InputdiameterInput1"]},{row["InputdiameterInput2"]}," +
                                    $"{row["InputdiameterInput3"]},{row["InputdiameterInput4"]}," +
                                    $"{row["InputdiameterInput5"]},{row["InputdiameterInput6"]}";
                    diameterSpecimens.Add($"{core};{specimen}:{inputs}");
                }
                if (diameterSpecimens.Any())
                {
                    reportParamMapList.Add("13059"); // Diameter
                    resultList.Add(string.Join("|", diameterSpecimens));
                }

                // THICKNESS AVERAGES
                var thicknessAvgValues = dt.AsEnumerable()
                                          .Select(r => r["InputthicknessInputAvg"]?.ToString())
                                          .Where(v => !string.IsNullOrWhiteSpace(v))
                                          .ToList();
                if (thicknessAvgValues.Any())
                {
                    reportParamMapList.Add("13058"); // ThicknessAvg.
                    resultList.Add(string.Join("|", thicknessAvgValues));
                }

                // DIAMETER AVERAGES
                var diameterAvgValues = dt.AsEnumerable()
                                         .Select(r => r["InputdiameterInputAvg"]?.ToString())
                                         .Where(v => !string.IsNullOrWhiteSpace(v))
                                         .ToList();
                if (diameterAvgValues.Any())
                {
                    reportParamMapList.Add("13060"); // DiameterAvg.
                    resultList.Add(string.Join("|", diameterAvgValues));
                }

                // AFTER TEMP & LOAD
                List<string> thicknessAfter6H = new List<string>();
                List<string> deformationAfter6H = new List<string>();
                List<string> Median = new List<string>();
                List<string> weigthload = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string core = row["CorePhase"].ToString();
                    int specimen = Convert.ToInt32(row["SpecimenNo"]);

                    if (!string.IsNullOrWhiteSpace(row["ThicknessatTemperature"]?.ToString()))
                        thicknessAfter6H.Add($"{core};{specimen}:{row["ThicknessatTemperature"]}");
                    if (!string.IsNullOrWhiteSpace(row["DeformationTemperature"]?.ToString()))
                        deformationAfter6H.Add($"{core};{specimen}:{row["DeformationTemperature"]}");
                    if (!string.IsNullOrWhiteSpace(row["Median"]?.ToString()))
                        Median.Add($"{core};{specimen}:{row["Median"]}");
                    if (!string.IsNullOrWhiteSpace(row["Weightload"]?.ToString()))
                        weigthload.Add($"{core};{specimen}:{row["Weightload"]}");

                }
                if (thicknessAfter6H.Any())
                {
                    reportParamMapList.Add("14087"); // 
                    resultList.Add(string.Join("|", thicknessAfter6H));
                }
                if (deformationAfter6H.Any())
                {
                    reportParamMapList.Add("14088"); // AfterTempLoad
                    resultList.Add(string.Join("|", deformationAfter6H));
                }
                if (Median.Any())
                {
                    reportParamMapList.Add("14089"); 
                    resultList.Add(string.Join("|", Median));
                }
                if (weigthload.Any())
                {
                    reportParamMapList.Add("13061");
                    resultList.Add(string.Join("|", weigthload));
                }

                reportParamMapList.Add("13062"); resultList.Add(txtSpecificThickness.Text);  // SpecifiedThickness
                reportParamMapList.Add("13063"); resultList.Add(txtSpecificDiameter.Text);   // SpecifiedDiameter
                reportParamMapList.Add("13064"); resultList.Add(txtSpecificWeightload.Text); // SpecifiedWeightload
                reportParamMapList.Add("13065"); resultList.Add(txtSpecific6hourThickness.Text); // Specified6hourThickness
                reportParamMapList.Add("13066"); resultList.Add(txtSpecificDeformation.Text);   // Specified6hourDeformation
                reportParamMapList.Add("14090"); resultList.Add(txtSpecificMedian.Text);   // Specified6hourDeformation
                reportParamMapList.Add("13067"); resultList.Add(cableType.Text);               // Type
                reportParamMapList.Add("13068"); resultList.Add(cableSize.Text);               // Size
                reportParamMapList.Add("13069"); resultList.Add(voltage.Text);                 // Voltage
                reportParamMapList.Add("13070"); resultList.Add(Date.Text);                    // Date
                reportParamMapList.Add("13072"); resultList.Add(roomno.Text);                  // RoomNo
                reportParamMapList.Add("13073"); resultList.Add(roomtemp.Text);                // RoomTemp
                reportParamMapList.Add("13074"); resultList.Add(refSpec.Text);                 // RefSpec
                reportParamMapList.Add("13075"); resultList.Add(testMethodNo.Text);            // TestMethodNo
                reportParamMapList.Add("13076"); resultList.Add(jobOrderNo.Text);              // JobOrderNo
                reportParamMapList.Add("13077"); resultList.Add(worksheetno.Text);             // WorkSheetNo
                reportParamMapList.Add("13078"); resultList.Add(roomhumidity.Text);            // RoomHumidity
                reportParamMapList.Add("13079"); resultList.Add(equipmentno.Text);             // EquipmentNo
                reportParamMapList.Add("13054"); resultList.Add(txtaftertemp.Text);
                reportParamMapList.Add("13055"); resultList.Add(txtunderloadatTemp.Text);
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
                    Response.Redirect($"GenerateTestReport_PressureTestAtHighTemperature_Revision.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

                if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out _))
                {
                    errors.Add("Specimen No. must be a valid integer.");
                    invalidControls.Add(txtSpecimenNo);
                }

                // Validate your new input textboxes here (thickness and diameter)
                TextBox[] newTestInputs = {
            txtthicknessInput1, txtthicknessInput2, txtthicknessInput3, txtthicknessInput4, txtthicknessInput5, txtthicknessInput6,
            txtdiameterInput1, txtdiameterInput2, txtdiameterInput3, txtdiameterInput4, txtdiameterInput5, txtdiameterInput6,
            txtthicknessInputAvg, txtdiameterInputAvg,
            txtWeightload, txtThicknessatTemperature, txtDeformationTemperature
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

                string afterTempText = string.IsNullOrWhiteSpace(txtaftertemp.Text) ? "___" : txtaftertemp.Text;
                string underLoadTempText = string.IsNullOrWhiteSpace(txtunderloadatTemp.Text) ? "___" : txtunderloadatTemp.Text;

            
                GridViewRow row1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row1.Cells.Add(CreateHeaderCell("Core Phase", 3, 1));
                row1.Cells.Add(CreateHeaderCell("Specimen number", 3, 1));
                row1.Cells.Add(CreateHeaderCell("Thickness", 1, 7));
                row1.Cells.Add(CreateHeaderCell("Diameter/Width", 1, 7));
                row1.Cells.Add(CreateHeaderCell("Weight Load", 2, 1));
                row1.Cells.Add(CreateHeaderCell($"After: {afterTempText} under load at temperature: {underLoadTempText}°C", 1, 3));
                row1.Cells.Add(CreateHeaderCell("Action", 3, 1));

                gv.Controls[0].Controls.AddAt(0, row1);

             
                GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
                for (int i = 1; i <= 6; i++) row2.Cells.Add(CreateHeaderCell(i.ToString(), 1, 1));
                row2.Cells.Add(CreateHeaderCell("Avg.", 1, 1));
                for (int i = 1; i <= 6; i++) row2.Cells.Add(CreateHeaderCell(i.ToString(), 1, 1));
                row2.Cells.Add(CreateHeaderCell("Avg.", 1, 1));
                row2.Cells.Add(CreateHeaderCell("Thickness", 1, 1));
                row2.Cells.Add(CreateHeaderCell("Deformation", 1, 1));
                row2.Cells.Add(CreateHeaderCell("Median", 1, 1));
               
                gv.Controls[0].Controls.AddAt(1, row2);

                GridViewRow row3 = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row3.Cells.Add(CreateHeaderCell("mm", 1, 7));
                row3.Cells.Add(CreateHeaderCell("mm", 1, 7));
                row3.Cells.Add(CreateHeaderCell("N", 1, 1));
                row3.Cells.Add(CreateHeaderCell("mm", 1, 1));
                row3.Cells.Add(CreateHeaderCell("%", 1, 1));
                row3.Cells.Add(CreateHeaderCell("%", 1, 1));
               
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