using LIMSStaging;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;
using Table = Microsoft.Office.Interop.Word.Table;


namespace LIMS
{
    public partial class MetalCookware : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["s"] != null)
                {
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                    lblSampleID.Text = Request.QueryString["s"].ToLower();
                }
                if (Request.QueryString["r"] != null)
                {
                    _cm.ReportID = Request.QueryString["r"];
                    lblReportID.Text = _cm.ReportID;
                }

                BindEquipmentCheckboxes();
                LoadClauseTable();
                //ReportName();
                CreateGridTable_StrengthTable();
                CreateGridTable_FormTest();
                CreateGridTable_BaseMaterial();
                CreateGridTable_FatigueResistance();
                CreateGridTable_NonStickCoating();
                CreateGridTable_ThermalShock();
                CreateGridTable_CoatingThickness();

                // CreateGridTable_cookingequ();
                DataTable existingData = _cm.GetExistingTestResults_Flatware_RP(lblSampleID.Text, lblReportID.Text);

                if (existingData != null && existingData.Rows.Count > 0)
                {
                    PrefillTestResultsGrid(existingData);
                    PrefillAllData(existingData);
                    // PrefillTestResultsGrid_cookingequ(existingData);
                    //  PrefillAllData_cookingequ(existingData);
                    PrefillFormTest(existingData);
                    PrefillBaseMaterial(existingData);
                    PrefillFatigueResistance(existingData);
                    PrefillNonStickCoating(existingData);
                    PrefillThermalShock(existingData);
                    PrefillCoatingThickness(existingData);

                }

                if (existingData != null)
                {
                    PrefillTestResultsGrid(existingData);
                    PrefillAllData(existingData);
                    //PrefillTestResultsGrid_cookingequ(existingData);
                    // PrefillAllData_cookingequ(existingData);
                    PrefillFormTest(existingData);
                    PrefillBaseMaterial(existingData);
                    PrefillFatigueResistance(existingData);
                    PrefillNonStickCoating(existingData);
                    PrefillThermalShock(existingData);
                    PrefillCoatingThickness(existingData);
                }

                // txtRequestNo.Enabled = false;
                txtstandard.Enabled = false;
                //txtType.Enabled = false;
                txtmodel.Enabled = false;
                txtworkorder.Enabled = false;
            }
        }

        private void LoadClauseTable()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);
            DataTable dtcl = _cm.GetCRFCloneList_LED(sampleId);
            int cloneCRFSampleID = Convert.ToInt32(dtcl.Rows[0]["CloneCRFSampleID"].ToString());
            ViewState["CloneCRFSampleID"] = dtcl.Rows[0]["CloneCRFSampleID"].ToString();

            string sampleName;
            DataTable snm = _cm.GetSampleName_MetalCookware(cloneCRFSampleID);
            if (snm != null && snm.Rows.Count > 0)
            {
                sampleName = snm.Rows[0]["SampleName"].ToString();
                ViewState["sampleName"] = sampleName;
                txtmodel.Text = sampleName;
            }

            DataTable dts = _cm.GetCRFSampleDetails_MetalCookware(sampleId, reportId, cloneCRFSampleID);
            if (dts != null && dts.Rows.Count > 0)
            {
                var standardIds = dts.AsEnumerable()
                    .Where(row => row["TestStandardID"] != null && !string.IsNullOrEmpty(row["TestStandardID"].ToString()))
                    .Select(row => row["TestStandardID"].ToString().Trim()).Distinct().ToList();
                var standardNames = dts.AsEnumerable()
                    .Where(row => row["TestStandard"] != null && !string.IsNullOrEmpty(row["TestStandard"].ToString()))
                    .Select(row => row["TestStandard"].ToString().Trim()).Distinct().ToList();
                lblstandardID.Text = string.Join(", ", standardIds);
                txtstandard.Text = string.Join(" ; ", standardNames);
            }

            DataTable dt = GetClauseData();
            DataTable saved = GetSavedResults();

            if (saved != null && saved.Rows.Count > 0)
            {
                txtsampleCode.Text = saved.Rows[0]["SampleCode"]?.ToString() ?? "";
                txtTestEquipmentNo.Text = saved.Rows[0]["TestEquipmentNumber"]?.ToString() ?? "";
                txttestmethod.Text = saved.Rows[0]["TestMethod"]?.ToString() ?? "";
                txttemp.Text = saved.Rows[0]["Temperature"]?.ToString() ?? "";
                txthumidity.Text = saved.Rows[0]["Humidity"]?.ToString() ?? "";
                txtSerialNo.Text = saved.Rows[0]["SampleIDSeries"]?.ToString() ?? "";
                txtBrand.Text = saved.Rows[0]["ProductBrand"]?.ToString() ?? "";
                txttypeofmaterial.Text = saved.Rows[0]["typeofmaterial"]?.ToString() ?? "";
                txtcoatingmaterial.Text = saved.Rows[0]["coatingmaterial"]?.ToString() ?? "";
                txtForm.Text = saved.Rows[0]["Form"]?.ToString() ?? "";
                txtTypeOfProducts_header.Text = saved.Rows[0]["Typesofproducts"]?.ToString() ?? "";
                txtSerialNo.Text = saved.Rows[0]["SerialNo"]?.ToString() ?? "";

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestDate"].ToString()))
                {
                    DateTime testDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestDate"].ToString(), out testDate))
                        txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrEmpty(saved.Rows[0]["TestCompletationDate"].ToString()))
                {
                    DateTime testDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestCompletationDate"].ToString(), out testDate))
                        txtcompletiondate.Text = testDate.ToString("yyyy-MM-dd");
                }
            }

            DataTable dtCategoryTypeStdTest = _cm.GetCategoryTypeStdTest(lblSampleID.Text);
            if (dtCategoryTypeStdTest != null && dtCategoryTypeStdTest.Rows.Count > 0)
                txtTestCategory.Text = dtCategoryTypeStdTest.Rows[0]["TestCategory"].ToString();

            DataTable DisResget = _cm.GetTestResultMetalCookware_DISTINCT(lblSampleID.Text, lblReportID.Text);
            if (DisResget != null && DisResget.Rows.Count > 0)
            {
                string equipementIDGet = DisResget.Rows[0]["EquipmentIds"].ToString();
                if (!string.IsNullOrEmpty(equipementIDGet))
                {
                    string[] selectedEquipmentIds = equipementIDGet.Split(',').Select(id => id.Trim()).ToArray();
                    foreach (ListItem item in chkEquipmentList.Items)
                    {
                        if (selectedEquipmentIds.Contains(item.Value))
                            item.Selected = true;
                    }
                }
            }

            int reportID = Convert.ToInt32(lblReportID.Text);
            string standardID = lblstandardID.Text;
            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_MetalCookware(sampleId, reportID, standardID);

            if (dt1 != null && dt1.Rows.Count > 0)
            {
                txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"].ToString();
                lblReportName.Text = dt1.Rows[0]["ReportName"].ToString();

                string sis = dts.Rows[0]["sampleIDSeries"].ToString();
                string requestNo = "";
                if (!string.IsNullOrEmpty(sis) && sis.Contains("/"))
                {
                    string[] parts = sis.Split('/');
                    if (parts.Length > 1)
                        requestNo = "R/" + parts[1];
                }
                ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"].ToString();

                string StandardID = dt1.Rows[0]["StandardID"].ToString();
                string[] ids = StandardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> standardIdList = ids.Select(id => id.Trim()).ToList();

                DataTable filteredClauseTable = null;
                if (standardIdList.Count > 0)
                {
                    var matchingRows = dt.AsEnumerable().Where(r =>
                    {
                        string clauseStandardIds = r["Standard_ID"].ToString();
                        string[] clauseIds = clauseStandardIds
                            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim()).ToArray();
                        return clauseIds.Any(cid => standardIdList.Contains(cid));
                    });
                    if (matchingRows.Any())
                        filteredClauseTable = matchingRows.CopyToDataTable();
                }

                if (filteredClauseTable == null || filteredClauseTable.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                bool descriptionHeaderShown = false;

                // ── TABLE + MAIN HEADER (5 columns) ──────────────────────────────────
                // Column order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
                sb.Append("<table class='clause-table'>");
                sb.Append("<tr class='sub-header-row'>");
                sb.Append("<th class='clause-number'>Klausul<br/>Clause</th>");
                sb.Append("<th class='requirement'>Parameter Uji<br/>Test Parameters</th>");
                sb.Append("<th class='unit'>Satuan<br/>Unit</th>");
                sb.Append("<th class='result'>Hasil Tes<br/>Test Results</th>");
                sb.Append("<th class='specification'>Persyaratan Mutu<br/>Quality Requirements</th>");
                sb.Append("</tr>");

                foreach (DataRow row in filteredClauseTable.Rows)
                {
                    bool isHeader = row["isHeader"].ToString() == "1";
                    bool isDescription = row["isDescription"].ToString() == "1";
                    bool isRefereReq = row.Table.Columns.Contains("isRefereRequirement") &&
                                       (row["isRefereRequirement"].ToString().Trim() == "1" ||
                                        row["isRefereRequirement"].ToString().Trim().ToLower() == "true");

                    // ── HEADER ROW ───────────────────────────────────────────────────
                    if (isHeader)
                    {
                        descriptionHeaderShown = false;
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        string savedResult = "";
                        string savedUnit = "";
                        var savedRow = saved?.Select($"ReportParamMapID={clauseID}").FirstOrDefault();
                        if (savedRow != null)
                        {
                            savedResult = savedRow["Result"]?.ToString() ?? "";
                            savedUnit = savedRow.Table.Columns.Contains("UnitName") && savedRow["UnitName"] != DBNull.Value
                                ? savedRow["UnitName"].ToString() : "";
                        }

                        sb.Append("<tr class='header-row'>");

                        // Col 1: Clause Number
                        sb.Append($"<td class='clause-number'><b>{row["ClauseNumber"]}</b></td>");

                        // Col 2: Title (spans Test Parameters only now)
                        sb.Append("<td>");
                        sb.Append($"<b>{row["ClauseTitle_BH"]}</b><br/>{row["ClauseTitle"]}");
                        sb.Append("</td>");

                        // Col 3: Unit
                        sb.Append($"<td class='unit'><input type='text' class='form-control unit-input' id='Unit_{clauseID}' name='Unit_{clauseID}' value='{savedUnit}' /></td>");

                        //// Col 4: Result
                        //sb.Append("<td class='result'>");
                        //sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        //sb.Append($"<input type='text' class='form-control' id='result_{clauseID}' name='result_{clauseID}' value='{savedResult}' />");
                        //sb.Append("</td>");
                        // Col 4: Result
                        sb.Append("<td class='result'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        if (row.Table.Columns.Contains("isRefre") && (row["isRefre"].ToString().Trim() == "1" || row["isRefre"].ToString().Trim().ToLower() == "true"))
                            sb.Append("<span style='font-style:italic;color:#888;'>&#8594; Refer to Table</span>");
                        else
                            sb.Append($"<input type='text' class='form-control' id='result_{clauseID}' name='result_{clauseID}' value='{savedResult}' />");
                        sb.Append("</td>");

                        // Col 5: Quality Requirements - empty for header
                        sb.Append("<td class='specification'></td>");

                        sb.Append("</tr>");
                    }
                    // ── DESCRIPTION ROW ──────────────────────────────────────────────
                    else if (isDescription)
                    {
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        string savedSpecification = "";
                        string savedSpecification_BH = "";
                        string savedUnit = "";
                        var savedRow = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRow != null)
                        {
                            savedSpecification = savedRow["Remark"]?.ToString() ?? "";
                            savedSpecification_BH = savedRow["Remark_BH"]?.ToString() ?? "";
                            savedUnit = savedRow.Table.Columns.Contains("UnitName") && savedRow["UnitName"] != DBNull.Value
                                ? savedRow["UnitName"].ToString() : "";
                        }

                        string specification = row["Specification"]?.ToString() ?? "";
                        string specification_BH = row["Specification_BH"]?.ToString() ?? "";
                        string displaySpec = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
                        string displaySpec_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;

                        // Sub-header shown once per description section
                        if (!descriptionHeaderShown)
                        {
                            sb.Append("<tr class='desc-sub-header-row'>");
                            sb.Append("<td></td>");
                            sb.Append("<th>Description</th>");
                            sb.Append("<th>Unit</th>");
                            sb.Append("<th>Result</th>");
                            sb.Append("<th>Condition (mm)</th>");
                            sb.Append("</tr>");
                            descriptionHeaderShown = true;
                        }

                        sb.Append("<tr>");

                        // Col 1: Clause Number
                        sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                        // Col 2: Description
                        //sb.Append("<td>");
                        //sb.Append($"<b>{row["Description_BH"]}</b><br/>{row["Description"]}");
                        //sb.Append("</td>");
                        // Col 2: Description
                        sb.Append("<td>");
                        if (row.Table.Columns.Contains("IsRefereRequirement") && (row["IsRefereRequirement"].ToString() == "1" || row["IsRefereRequirement"].ToString().ToLower() == "true"))
                            sb.Append("<span style='font-style:italic;color:#888;'>&#8594; Refer to Requirement</span>");
                        else
                            sb.Append($"<b>{row["Description_BH"]}</b><br/>{row["Description"]}");
                        sb.Append("</td>");
                        // Col 3: Unit
                        sb.Append($"<td class='unit'><input type='text' class='form-control unit-input' id='Unit_{clauseID}' name='Unit_{clauseID}' value='{savedUnit}' /></td>");

                        // Col 4: Result
                        sb.Append("<td class='result'>");
                        GenerateResultInput(sb, row, saved, clauseID);
                        sb.Append("</td>");

                        // Col 5: Condition / Specification (Quality Requirements)
                        sb.Append("<td class='specification'>");
                        if (!string.IsNullOrEmpty(displaySpec_BH))
                            sb.Append($"<b>{displaySpec_BH}</b><br/>");
                        sb.Append($"<input type='text' class='form-control spec-input' id='specification_{clauseID}' name='specification_{clauseID}' value='{displaySpec.Replace("'", "&#39;")}' />");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        sb.Append("</td>");

                        sb.Append("</tr>");
                    }
                    // ── NORMAL ROW ───────────────────────────────────────────────────
                    else
                    {
                        descriptionHeaderShown = false;
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        string savedSpecification = "";
                        string savedUnit = "";
                        var savedRow = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRow != null)
                        {
                            savedSpecification = savedRow["Specification"]?.ToString() ?? "";
                            savedUnit = savedRow.Table.Columns.Contains("UnitName") && savedRow["UnitName"] != DBNull.Value
                                ? savedRow["UnitName"].ToString() : "";
                        }

                        string specification = row["Specification"]?.ToString() ?? "";
                        string specification_BH = row["Specification_BH"]?.ToString() ?? "";
                        string displaySpec = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
                        string safeUnit = (savedUnit ?? "").Replace("'", "&#39;");

                        sb.Append("<tr>");

                        // Col 1: Clause Number
                        sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                        //// Col 2: Requirement (Test Parameters)
                        //sb.Append("<td class='requirement'>");
                        //sb.Append($"<b>{row["Requirement_BH"]}</b><br/>{row["Requirement"]}");
                        //sb.Append("</td>");
                        // Col 2: Requirement (Test Parameters)
                        sb.Append("<td class='requirement'>");
                        if (row.Table.Columns.Contains("IsRefereRequirement") && (row["IsRefereRequirement"].ToString() == "1" || row["IsRefereRequirement"].ToString().ToLower() == "true"))
                            sb.Append("<span style='font-style:italic;color:#888;'>&#8594; Refer to Requirement</span>");
                        else
                            sb.Append($"<b>{row["Requirement_BH"]}</b><br/>{row["Requirement"]}");
                        sb.Append("</td>");
                        // Col 3: Unit
                        sb.Append($"<td class='unit'><input type='text' class='form-control unit-input' id='Unit_{clauseID}' name='Unit_{clauseID}' value='{safeUnit}' /></td>");

                        // Col 4: Result
                        sb.Append("<td class='result'>");
                        GenerateResultInput(sb, row, saved, clauseID);
                        sb.Append("</td>");

                        // Col 5: Specification (Quality Requirements)
                        sb.Append("<td class='specification'>");
                        sb.Append($"<input type='text' class='form-control spec-input' id='specification_{clauseID}' name='specification_{clauseID}' value='{displaySpec.Replace("'", "&#39;")}' />");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        sb.Append("</td>");

                        sb.Append("</tr>");
                    }
                }

                sb.Append("</table>");
                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "");
            }
        }

        private void BindEquipmentCheckboxes()
        {

            int DepartmentId = Convert.ToInt32(Session["DeptID"].ToString());

            string query = "SELECT * FROM Calibration_Equipment WHERE DepartmentId = @DepartmentId";


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    chkEquipmentList.DataSource = reader;
                    chkEquipmentList.DataTextField = "Equipment";
                    chkEquipmentList.DataValueField = "EpID";
                    chkEquipmentList.DataBind();
                    con.Close();
                }
            }
        }

        private string GetInputType(DataRow row)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
                return "checkbox";
            else
                return "text";
        }

        private void GenerateResultInput(StringBuilder sb, DataRow row, DataTable saved, int clauseID)
        {
            // ── ADDED ────────────────────────────────────────────────────────────
            if (row.Table.Columns.Contains("isRefre") && row["isRefre"].ToString() == "1")
            {
                sb.Append("<span style='font-style:italic;color:#888;'>&#8594; Refer to Table</span>");
                return;
            }

            if (row.Table.Columns.Contains("isRefereRequirement") && (row["isRefereRequirement"].ToString().Trim() == "1" || row["isRefereRequirement"].ToString().Trim().ToLower() == "true"))
            {
                sb.Append("<span style='font-style:italic;color:#888;'>&#8594; Refer to Requirement</span>");
                return;
            }
            // ─────────────────────────────────────────────────────────────────────

            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
            {
                string resultOptionsBH = row["ResultOptions"]?.ToString()?.Trim() ?? "";
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : new string[0];

                var savedRows = saved.Select($"ClauseID={clauseID}");
                List<string> savedValues = new List<string>();
                foreach (var savedRow in savedRows)
                {
                    string savedResult = savedRow["Result"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(savedResult))
                        savedValues.AddRange(savedResult.Split(',').Select(s => s.Trim()));
                }

                sb.Append("<div class='checkbox-group'>");

                for (int i = 0; i < options.Length; i++)
                {
                    string optValue = options[i].Trim();
                    string optDisplayBH = i < optionsBH.Length ? optionsBH[i].Trim() : optValue;
                    string isChecked = savedValues.Contains(optValue) ? "checked='checked'" : "";

                    if (i % 3 == 0)
                        sb.Append("<div class='checkbox-row'>");

                    sb.Append($"<label class='checkbox-item'><input type='checkbox' name='chk_{clauseID}_{optValue}' value='{optValue}' {isChecked}/> {optDisplayBH}</label>");

                    if ((i + 1) % 3 == 0 || i == options.Length - 1)
                        sb.Append("</div>");
                }
                sb.Append("</div>");
            }
            else
            {
                var savedRow = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                string savedResult = savedRow != null ? savedRow["Result"]?.ToString() ?? "" : "";
                sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='width:94%;' />");
            }
        }

        private DataTable GetClauseData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string reportId = lblReportID.Text.Trim();
                string[] standardIds = lblstandardID.Text.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToArray();

                if (!string.IsNullOrEmpty(reportId) && standardIds.Length > 0)
                {
                    string whereClause = string.Join(" OR ", standardIds.Select(id => $"(',' + Standard_ID + ',') LIKE '%,{id},%'"));

                    string query = $@"SELECT * FROM Clauses WHERE Report_Id = @ReportId AND ({whereClause}) ORDER BY ClauseID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.Parameters.AddWithValue("@ReportId", reportId);
                        conn.Open();
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        private DataTable GetSavedResults()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM TestResultsLED WHERE SampleID = @SampleID and ReportID = @ReportID", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                    conn.Close();
                }
            }
            return dt;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool b1 = false;

                string cloneCFRsampleID = ViewState["CloneCRFSampleID"].ToString();
                string equipmentIdsCSV = "";
                string equipmentNamesCSV = "";

                List<string> selectedEquipmentIds = new List<string>();
                List<string> selectedEquipmentNames = new List<string>();

                foreach (ListItem item in chkEquipmentList.Items)
                {
                    if (item.Selected)
                    {
                        selectedEquipmentIds.Add(item.Value);
                        selectedEquipmentNames.Add(item.Text);
                    }
                }

                if (selectedEquipmentIds.Count > 0)
                {
                    equipmentIdsCSV = string.Join(",", selectedEquipmentIds);
                    equipmentNamesCSV = string.Join(",", selectedEquipmentNames);

                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int ReportID = Convert.ToInt32(Request.QueryString["r"]);
                    string sampleIdSeries = ViewState["SampleIDSeriesVS"].ToString();
                    string language = "";
                    int reportParamMapID = 0;

                    ClearExistingResults();
                    DataTable clauses = GetClauseData();

                    // ── EXISTING CLAUSE SAVE LOOP ────────────────────────────────────
                    foreach (DataRow row in clauses.Rows)
                    {
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        bool isHeader = row["isHeader"].ToString() == "1";

                        string inputType = Request.Form[$"inputtype_{clauseID}"] ?? "";
                        string specification = Request.Form[$"specification_{clauseID}"] ?? "";
                        string specification_BH = Request.Form[$"specification_BH{clauseID}"] ?? "";
                        string unit = Request.Form[$"Unit_{clauseID}"] ?? "";
                        string result = "";

                        if (inputType == "checkbox")
                        {
                            string[] options = row["ResultOptions"].ToString().Split(',');
                            List<string> selectedOptions = new List<string>();
                            foreach (var option in options)
                            {
                                if (Request.Form[$"chk_{clauseID}_{option.Trim()}"] != null)
                                    selectedOptions.Add(option.Trim());
                            }
                            result = string.Join(",", selectedOptions);
                        }
                        else
                        {
                            result = Request.Form[$"result_{clauseID}"] ?? "";
                        }

                        int result0 = _cm.TestResultsAdd_Cookware(
                            clauseID,
                            txtstartingdate.Text,
                            txtstartingdate.Text,
                            result,
                            specification,
                            specification_BH,
                            sampleId,
                            sampleIdSeries,
                            isHeader ? "header" : inputType,
                            language,
                            ReportID,
                            reportParamMapID,
                            equipmentIdsCSV,
                            txtworkorder.Text,
                            lblReportName.Text,
                            txtBrand.Text,
                            txtmodel.Text,
                            lblstandardID.Text,
                            txtstandard.Text,
                            txttestmethod.Text,
                            txtTestCategory.Text,
                            txtsampleCode.Text,
                            txtTestEquipmentNo.Text,
                            txttemp.Text,
                            txthumidity.Text,
                            unit,
                            txtcoatingmaterial.Text, txtForm.Text, txttypeofmaterial.Text,
                            txtTypeOfProducts_header.Text,
                            txtSerialNo.Text
                        );

                        if (result0 > 0)
                            b1 = true;
                    }

                    /* ================= REPORT GENERATION ================= */

                    if (b1 == true)
                    {
                        int sampleStatusID = 27;

                        int res1 = _cm.UpdateCRFSampleDetailsStatus_Flatware(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
                        if (res1 > 0)
                        {
                            int res2 = _cm.UpdateCRFCloneStatus_Flatware(sampleStatusID, cloneCFRsampleID);
                            if (res2 > 0)
                            {
                                int res3 = _cm.updateSampleDetailStatus_Flatware(sampleStatusID, lblSampleID.Text);
                                if (res3 > 0)
                                {
                                    // ── STRENGTH TEST TABLE (18967–18971) ────────────────────
                                    DataTable dtStrength = ViewState["TestData"] as DataTable;
                                    if (dtStrength == null || dtStrength.Rows.Count == 0)
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No strength test data to save!');", true);
                                        return;
                                    }

                                    List<string> strengthRpmList = new List<string>();
                                    List<string> strengthResultList = new List<string>();

                                    foreach (DataRow row in dtStrength.Rows)
                                    {
                                        strengthRpmList.Add("18967"); strengthResultList.Add(row["Diameter"].ToString());
                                        strengthRpmList.Add("18968"); strengthResultList.Add(row["TestDuration"].ToString());
                                        strengthRpmList.Add("18969"); strengthResultList.Add(row["TestResults"].ToString());
                                        strengthRpmList.Add("18970"); strengthResultList.Add(row["Decision"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(txtAdditionalinfo.Text.Trim()))
                                    {
                                        strengthRpmList.Add("20014");
                                        strengthResultList.Add(txtAdditionalinfo.Text.Trim());
                                    }

                                    _cm.TestResultsAdd_Flateware_RP(
                                        0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                        sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                        txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                        lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                        strengthRpmList.ToArray(), strengthResultList.ToArray());

                                    // ── FORM TEST TABLE (19967–19972) ─────────────────────────
                                    DataTable dtFormTest = ViewState["TestData_FormTest"] as DataTable;
                                    if (dtFormTest != null && dtFormTest.Rows.Count > 0)
                                    {
                                        List<string> rpmList = new List<string>();
                                        List<string> resultList = new List<string>();
                                        foreach (DataRow row in dtFormTest.Rows)
                                        {
                                            rpmList.Add("19967"); resultList.Add(row["D1"].ToString());
                                            rpmList.Add("19968"); resultList.Add(row["D2"].ToString());
                                            rpmList.Add("19969"); resultList.Add(row["RoundnessValue"].ToString());
                                            rpmList.Add("19970"); resultList.Add(row["RoundnessValueTolerance"].ToString());
                                            rpmList.Add("19971"); resultList.Add(row["Decision"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(txtFormTestAdditionalinfo.Text.Trim()))
                                        {
                                            rpmList.Add("19972");
                                            resultList.Add(txtFormTestAdditionalinfo.Text.Trim());
                                        }
                                        _cm.TestResultsAdd_Flateware_RP(
                                            0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                            sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                            txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                            lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                            rpmList.ToArray(), resultList.ToArray());
                                    }

                                    // ── BASE MATERIAL THICKNESS TEST TABLE (19973–19979) ──────
                                    DataTable dtBase = ViewState["TestData_BaseMaterial"] as DataTable;
                                    if (dtBase != null && dtBase.Rows.Count > 0)
                                    {
                                        List<string> rpmList = new List<string>();
                                        List<string> resultList = new List<string>();
                                        foreach (DataRow row in dtBase.Rows)
                                        {
                                            rpmList.Add("19973"); resultList.Add(row["Angle0"].ToString());
                                            rpmList.Add("19974"); resultList.Add(row["Angle90"].ToString());
                                            rpmList.Add("19975"); resultList.Add(row["Angle180"].ToString());
                                            rpmList.Add("19976"); resultList.Add(row["Angle270"].ToString());
                                            rpmList.Add("19977"); resultList.Add(row["Average"].ToString());
                                            rpmList.Add("19978"); resultList.Add(row["ThicknessMin"].ToString());
                                            rpmList.Add("19979"); resultList.Add(row["Decision"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(txtBaseMaterialAdditionalinfo.Text.Trim()))
                                        {
                                            rpmList.Add("20013");
                                            resultList.Add(txtFatigueAdditionalinfo.Text.Trim());
                                        }
                                        _cm.TestResultsAdd_Flateware_RP(
                                    0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                    sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                    txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                    lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                    rpmList.ToArray(), resultList.ToArray());
                                    }

                                    // ── FATIGUE RESISTANCE TEST TABLE (19980–19985) ───────────
                                    DataTable dtFatigue = ViewState["TestData_FatigueResistance"] as DataTable;
                                    if (dtFatigue != null && dtFatigue.Rows.Count > 0)
                                    {
                                        List<string> rpmList = new List<string>();
                                        List<string> resultList = new List<string>();
                                        foreach (DataRow row in dtFatigue.Rows)
                                        {
                                            rpmList.Add("19980"); resultList.Add(row["WaterVolume"].ToString());
                                            rpmList.Add("19981"); resultList.Add(row["WaterVolume5x15"].ToString());
                                            rpmList.Add("19982"); resultList.Add(row["NumberOfCycles"].ToString());
                                            rpmList.Add("19983"); resultList.Add(row["TestResults"].ToString());
                                            rpmList.Add("19985"); resultList.Add(row["Decision"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(txtFatigueAdditionalinfo.Text.Trim()))
                                        {
                                            rpmList.Add("19984");
                                            resultList.Add(txtFatigueAdditionalinfo.Text.Trim());
                                        }
                                        _cm.TestResultsAdd_Flateware_RP(
                                            0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                            sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                            txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                            lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                            rpmList.ToArray(), resultList.ToArray());
                                    }

                                    // ── NON-STICK COATING ADHESION TEST TABLE (19986–19991) ───
                                    DataTable dtNonStick = ViewState["TestData_NonStickCoating"] as DataTable;
                                    if (dtNonStick != null && dtNonStick.Rows.Count > 0)
                                    {
                                        List<string> rpmList = new List<string>();
                                        List<string> resultList = new List<string>();
                                        foreach (DataRow row in dtNonStick.Rows)
                                        {
                                            rpmList.Add("19986"); resultList.Add(row["SoakingDuration"].ToString());
                                            rpmList.Add("19987"); resultList.Add(row["NumberOfScratches"].ToString());
                                            rpmList.Add("19988"); resultList.Add(row["StrokeDistance"].ToString());
                                            rpmList.Add("19989"); resultList.Add(row["TestResults"].ToString());
                                            rpmList.Add("19990"); resultList.Add(row["Decision"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(txtNonStickAdditionalinfo.Text.Trim()))
                                        {
                                            rpmList.Add("19991");
                                            resultList.Add(txtNonStickAdditionalinfo.Text.Trim());
                                        }
                                        _cm.TestResultsAdd_Flateware_RP(
                                            0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                            sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                            txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                            lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                            rpmList.ToArray(), resultList.ToArray());
                                    }

                                    // ── THERMAL SHOCK TEST TABLE (19992–19996) ────────────────
                                    DataTable dtThermal = ViewState["TestData_ThermalShock"] as DataTable;
                                    if (dtThermal != null && dtThermal.Rows.Count > 0)
                                    {
                                        List<string> rpmList = new List<string>();
                                        List<string> resultList = new List<string>();
                                        foreach (DataRow row in dtThermal.Rows)
                                        {
                                            rpmList.Add("19992"); resultList.Add(row["TestTemp1"].ToString());
                                            rpmList.Add("19993"); resultList.Add(row["TestTemp2"].ToString());
                                            rpmList.Add("19994"); resultList.Add(row["TestResults"].ToString());
                                            rpmList.Add("19995"); resultList.Add(row["Decision"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(txtThermalAdditionalinfo.Text.Trim()))
                                        {
                                            rpmList.Add("19996");
                                            resultList.Add(txtThermalAdditionalinfo.Text.Trim());
                                        }
                                        _cm.TestResultsAdd_Flateware_RP(
                                            0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                            sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                            txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                            lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                            rpmList.ToArray(), resultList.ToArray());
                                    }

                                    // ── COATING THICKNESS TEST TABLE (19997–20007) ────────────
                                    DataTable dtCoating = ViewState["TestData_CoatingThickness"] as DataTable;
                                    if (dtCoating != null && dtCoating.Rows.Count > 0)
                                    {
                                        List<string> rpmList = new List<string>();
                                        List<string> resultList = new List<string>();
                                        foreach (DataRow row in dtCoating.Rows)
                                        {
                                            rpmList.Add("19997"); resultList.Add(row["TypeOfMaterial"].ToString());
                                            rpmList.Add("19998"); resultList.Add(row["CoatingMaterial"].ToString());
                                            rpmList.Add("19999"); resultList.Add(row["Measurement1"].ToString());
                                            rpmList.Add("20002"); resultList.Add(row["Measurement2"].ToString());
                                            rpmList.Add("20003"); resultList.Add(row["Measurement3"].ToString());
                                            rpmList.Add("20004"); resultList.Add(row["Average"].ToString());
                                            rpmList.Add("20005"); resultList.Add(row["MinLayerThickness"].ToString());
                                            rpmList.Add("20006"); resultList.Add(row["Decision"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(txtCoatingAdditionalinfo.Text.Trim()))
                                        {
                                            rpmList.Add("20007");
                                            resultList.Add(txtCoatingAdditionalinfo.Text.Trim());
                                        }
                                        _cm.TestResultsAdd_Flateware_RP(
                                            0, txtstartingdate.Text, txtcompletiondate.Text, "",
                                            sampleId, sampleIdSeries, "", language, ReportID, equipmentIdsCSV,
                                            txtworkorder.Text, txtTypeOfProducts_header.Text, txtBrand.Text, txtmodel.Text,
                                            lblstandardID.Text, txtstandard.Text, txttestmethod.Text, txttemp.Text, txthumidity.Text,
                                            rpmList.ToArray(), resultList.ToArray());
                                    }

                                    /* -------- FILE GENERATION -------- */

                                    string SampleUID = lblSampleID.Text + "_" + lblReportID.Text;
                                    string pathout = Server.MapPath("~/TestReport/");
                                    string templatePath = Server.MapPath("~/InputFiles/MetalCookwareTestRecord_1.docx");

                                    if (!Directory.Exists(pathout))
                                        Directory.CreateDirectory(pathout);

                                    string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

                                    Application wordApp = new Application();
                                    Document doc = wordApp.Documents.Open(templatePath);

                                    ReplaceTemplateVariables(doc);

                                    CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", GetReportData());
                                    CreateReportTableStrength(wordApp, doc, "{Strengthtest}", GetReportDataForStrength());
                                    CreateReportTableFormTest(wordApp, doc, "{FormTest}", GetReportDataForFormTest());
                                    CreateReportTableBaseMaterial(wordApp, doc, "{BaseMaterial}", GetReportDataForBaseMaterial());
                                    CreateReportTableFatigueResistance(wordApp, doc, "{FatigueResistance}", GetReportDataForFatigueResistance());
                                    CreateReportTableNonStickCoating(wordApp, doc, "{NonStickCoating}", GetReportDataForNonStickCoating());
                                    CreateReportTableThermalShock(wordApp, doc, "{ThermalShock}", GetReportDataForThermalShock());
                                    CreateReportTableCoatingThickness(wordApp, doc, "{CoatingThickness}", GetReportDataForCoatingThickness());

                                    doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);
                                    doc.Close(false);
                                    wordApp.Quit();

                                    Response.Clear();
                                    Response.ContentType = "application/pdf";
                                    Response.AddHeader("Content-Disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                                    Response.TransmitFile(outputPdfPath);
                                    Response.End();
                                }
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select at least one equipment.');", true);
                }
            }
            catch (Exception ex)
            {
                throw;
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }
        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        bool b1 = false;

        //        string cloneCFRsampleID = ViewState["CloneCRFSampleID"].ToString();
        //        string equipmentIdsCSV = "";
        //        string equipmentNamesCSV = "";

        //        List<string> selectedEquipmentIds = new List<string>();
        //        List<string> selectedEquipmentNames = new List<string>();

        //        foreach (ListItem item in chkEquipmentList.Items)
        //        {
        //            if (item.Selected)
        //            {
        //                selectedEquipmentIds.Add(item.Value);
        //                selectedEquipmentNames.Add(item.Text);
        //            }
        //        }

        //        if (selectedEquipmentIds.Count > 0)
        //        {
        //            equipmentIdsCSV = string.Join(",", selectedEquipmentIds);
        //            equipmentNamesCSV = string.Join(",", selectedEquipmentNames);

        //            int sampleId = Convert.ToInt32(Request.QueryString["s"]);
        //            int ReportID = Convert.ToInt32(Request.QueryString["r"]);
        //            string sampleIdSeries = ViewState["SampleIDSeriesVS"].ToString();
        //            string language = "";
        //            int reportParamMapID = 0;

        //            ClearExistingResults();
        //            DataTable clauses = GetClauseData();

        //            foreach (DataRow row in clauses.Rows)
        //            {
        //                int clauseID = Convert.ToInt32(row["ClauseID"]);
        //                bool isHeader = row["isHeader"].ToString() == "1";

        //                string inputType = Request.Form[$"inputtype_{clauseID}"] ?? "";
        //                string specification = Request.Form[$"specification_{clauseID}"] ?? "";
        //                string specification_BH = Request.Form[$"specification_BH{clauseID}"] ?? "";
        //                string unit = Request.Form[$"Unit_{clauseID}"] ?? "";

        //                string result = "";

        //                if (inputType == "checkbox")
        //                {
        //                    string[] options = row["ResultOptions"].ToString().Split(',');
        //                    List<string> selectedOptions = new List<string>();

        //                    foreach (var option in options)
        //                    {
        //                        if (Request.Form[$"chk_{clauseID}_{option.Trim()}"] != null)
        //                            selectedOptions.Add(option.Trim());
        //                    }

        //                    result = string.Join(",", selectedOptions);
        //                }
        //                else
        //                {
        //                    result = Request.Form[$"result_{clauseID}"] ?? "";
        //                }

        //                int result0 = _cm.TestResultsAdd_Cookware(
        //                    clauseID,
        //                    txtstartingdate.Text,
        //                    txtstartingdate.Text,
        //                    result,
        //                    specification,
        //                    specification_BH,
        //                    sampleId,
        //                    sampleIdSeries,
        //                    isHeader ? "header" : inputType,   //
        //                    language,
        //                    ReportID,
        //                    reportParamMapID,
        //                    equipmentIdsCSV,
        //                    txtworkorder.Text,
        //                    lblReportName.Text,
        //                    txtBrand.Text,
        //                    txtmodel.Text,
        //                    lblstandardID.Text,
        //                    txtstandard.Text,
        //                    txttestmethod.Text,
        //                    txtTestCategory.Text,
        //                    txtsampleCode.Text,
        //                    txtTestEquipmentNo.Text,
        //                    txttemp.Text,
        //                    txthumidity.Text,
        //                    unit,
        //                  txtcoatingmaterial.Text, txtForm.Text, txttypeofmaterial.Text,
        //                    txtTypeOfProducts_header.Text,
        //                    txtSerialNo.Text
        //                );

        //                if (result0 > 0)
        //                    b1 = true;
        //            }

        //            /* ================= REPORT GENERATION ================= */


        //            if (b1 == true)
        //            {
        //                //int sampleStatusID = 27;

        //                //int res1 = _cm.UpdateCRFSampleDetailsStatus_Flatware(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
        //                //if (res1 > 0)
        //                //{
        //                //    int res2 = _cm.UpdateCRFCloneStatus_Flatware(sampleStatusID, cloneCFRsampleID);
        //                //    if (res2 > 0)
        //                //    {
        //                //        int res3 = _cm.updateSampleDetailStatus_Flatware(sampleStatusID, lblSampleID.Text);
        //                //        if (res3 > 0)
        //                {
        //                    ///* -------- DATA PREP -------- */

        //                    DataTable dtRepoParam = ViewState["TestData"] as DataTable;

        //                    if (dtRepoParam == null || dtRepoParam.Rows.Count == 0)
        //                    {
        //                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No test data to save!');", true);
        //                        return;
        //                    }

        //                    List<string> reportParamMapList = new List<string>();
        //                    List<string> resultList = new List<string>();

        //                    foreach (DataRow row in dtRepoParam.Rows)
        //                    {

        //                        reportParamMapList.Add("18967"); resultList.Add(row["Diameter"].ToString());
        //                        reportParamMapList.Add("18968"); resultList.Add(row["TestDuration"].ToString());
        //                        reportParamMapList.Add("18969"); resultList.Add(row["TestResults"].ToString());
        //                        reportParamMapList.Add("18970"); resultList.Add(row["Decision"].ToString());
        //                        // reportParamMapList.Add("17958"); resultList.Add(row["Decision"].ToString());
        //                    }

        //                    //  reportParamMapList.Add("17960");
        //                    //resultList.Add(txtAdditionalinfo.Text.Trim());

        //                    string[] reportParamMap = reportParamMapList.ToArray();
        //                    string[] resultNew = resultList.ToArray();

        //                    int modified = _cm.TestResultsAdd_Flateware_RP(
        //                        0,
        //                        txtstartingdate.Text,
        //                        txtcompletiondate.Text,
        //                        "",
        //                        sampleId,
        //                        sampleIdSeries,
        //                        "",
        //                        language,
        //                        ReportID,
        //                        equipmentIdsCSV,
        //                        txtworkorder.Text,
        //                        txtTypeOfProducts_header.Text,
        //                        txtBrand.Text,
        //                        txtmodel.Text,
        //                        lblstandardID.Text,
        //                        txtstandard.Text,
        //                        txttestmethod.Text,
        //                        txttemp.Text,
        //                        txthumidity.Text,
        //                        reportParamMap,
        //                        resultNew
        //                    );

        //                    /* -------- FILE GENERATION -------- */

        //                    string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
        //                    string pathout = Server.MapPath("~/Reports/");
        //                    string templatePath = Server.MapPath("~/InputFiles/MetalCookwareTestRecord_1.docx");

        //                    if (!Directory.Exists(pathout))
        //                        Directory.CreateDirectory(pathout);

        //                    string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

        //                    Application wordApp = new Application();
        //                    Document doc = wordApp.Documents.Open(templatePath);

        //                    ReplaceTemplateVariables(doc);

        //                    CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", GetReportData());
        //                    CreateReportTableT2(wordApp, doc, "{Strengthtest}", GetReportDataForT2());
        //                    // CreateReportTableT3(wordApp, doc, "{Tablecookingequipment}", GetReportDataForT3());

        //                    doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);
        //                    doc.Close(false);
        //                    wordApp.Quit();

        //                    Response.Clear();
        //                    Response.ContentType = "application/pdf";
        //                    Response.AddHeader("Content-Disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
        //                    Response.TransmitFile(outputPdfPath);
        //                    Response.End();
        //                }
        //                //    }
        //                //}
        //            }
        //        }
        //        else
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select at least one equipment.');", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
        //    }
        //}
        private void ClearExistingResults()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM TestResultsLED WHERE SampleID=@SampleID AND ReportID = @ReportID", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        private void CreateBasicTemplate(Document doc)
        {
            // Add title
            Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = "Test Results Report\n\n";
            title.Range.Font.Size = 16;
            title.Range.Font.Bold = 1;
            title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            // Add sample info
            Paragraph info = doc.Paragraphs.Add();
            info.Range.Text = "Sample ID: SMP/1001-0125\nTest Date: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n\n";

            // Add placeholder for data table
            Paragraph placeholder = doc.Paragraphs.Add();
            placeholder.Range.Text = "{TestingAndMeasurement}";
        }
        private void ReplaceTemplateVariables(Document doc)
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);

            // ── FETCH ReportParam values ─────────────────────────────────────────
            Dictionary<int, string> paramValues = new Dictionary<int, string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT ReportParamMapID, Result 
        FROM TestResultsLED 
        WHERE SampleID = @SampleID 
          AND ReportID = @ReportID
          AND ReportParamMapID IN (18967, 18968, 18969, 18970, 17834)
        ORDER BY ReportParamMapID", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleId);
                cmd.Parameters.AddWithValue("@ReportID", reportId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int mapId = Convert.ToInt32(reader["ReportParamMapID"]);
                        string result = reader["Result"]?.ToString() ?? "";
                        // Result stored as "value|value" - take first part
                        string value = result.Contains("|") ? result.Split('|')[0] : result;
                        paramValues[mapId] = value;
                    }
                }
            }

            // ── FETCH basic fields from TestResultsLED ───────────────────────────
            string testMethod = "";
            string temperature = "";
            string humidity = "";
            string testStartDate = "";
            string completionDate = "";
            string equipmentNames = "";
            string workorderNumber = txtworkorder.Text;
            string product = lblReportName.Text;
            string brand = "";
            string model = "";
            string standard = txtstandard.Text;
            string typeofmaterial = txttypeofmaterial.Text;
            string coatingmaterial = txtcoatingmaterial.Text;
            string TypeOfProduct_header = txtTypeOfProducts_header.Text;
            string testingdate = "";
            string serialno = txtSerialNo.Text;
            string Form = txtForm.Text;
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 
            TestMethod, Temperature, Humidity, TestDate, 
            TestCompletationDate, EquipmentIds, WorkOrderNo,serialNO,coatingmaterial,Form,typeofmaterial,
            ProductName, ProductBrand, ProductModel,Usagegroups,Typesofproducts, CASE 
        WHEN TestDate IS NOT NULL 
             AND TestCompletationDate IS NOT NULL
            THEN FORMAT(TestDate, 'dd-MM-yyyy') 
                 + ' - ' + 
                 FORMAT(TestCompletationDate, 'dd-MM-yyyy')

        WHEN TestDate IS NOT NULL 
            THEN FORMAT(TestDate, 'yyyy-MM-dd')

        WHEN TestCompletationDate IS NOT NULL 
            THEN FORMAT(TestCompletationDate, 'yyyy-MM-dd')

        ELSE NULL
    END AS testingdate

        FROM TestResultsLED 
        WHERE SampleID = @SampleID AND ReportID = @ReportID
        ORDER BY ResultID ASC", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleId);
                cmd.Parameters.AddWithValue("@ReportID", reportId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        testMethod = reader["TestMethod"]?.ToString() ?? "";
                        temperature = reader["Temperature"]?.ToString() ?? "";
                        humidity = reader["Humidity"]?.ToString() ?? "";
                        testStartDate = reader["TestDate"]?.ToString() ?? "";
                        completionDate = reader["TestCompletationDate"]?.ToString() ?? "";
                        brand = reader["ProductBrand"]?.ToString() ?? "";
                        model = reader["ProductModel"]?.ToString() ?? "";
                        product = reader["ProductName"]?.ToString() ?? "";
                        workorderNumber = reader["WorkOrderNo"]?.ToString() ?? "";
                        Form = reader["Form"]?.ToString() ?? "";
                        coatingmaterial = reader["coatingmaterial"]?.ToString() ?? "";
                        typeofmaterial = reader["typeofmaterial"]?.ToString() ?? "";
                        TypeOfProduct_header = reader["Typesofproducts"]?.ToString() ?? "";
                        serialno = reader["serialNo"]?.ToString() ?? "";

                        testingdate = reader["testingdate"]?.ToString() ?? "";

                        string equipmentIds = reader["EquipmentIds"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(equipmentIds))
                            equipmentNames = GetEquipmentNamesByIds(equipmentIds);
                    }
                }
            }
            string reportName = "";
            string docNo = "";

            DataTable dtReport = _cm.GetReportname(reportId);
            if (dtReport.Rows.Count > 0)
            {
                reportName = dtReport.Rows[0]["ReportName"]?.ToString() ?? "";
                docNo = dtReport.Rows[0]["DocNo"]?.ToString() ?? "";
            }
            // ── REPLACE ALL PLACEHOLDERS matching image ───────────────────────────
            ReplaceInDoc(doc, "{WorkorderNumber}", workorderNumber);  // Nomor Order (WO)
            ReplaceInDoc(doc, "{Product}", product);          // Produk / Product
            ReplaceInDoc(doc, "{Brand}", brand);            // Merk / Brand
            ReplaceInDoc(doc, "{Model}", model);            // Tipe/Model
            ReplaceInDoc(doc, "{Standard}", standard);         // Nomor Standard
            ReplaceInDoc(doc, "{Method}", testMethod);       // Nomor Metode Uji
            ReplaceInDoc(doc, "{Temperature}", temperature);      // Suhu (ºC)
            ReplaceInDoc(doc, "{Humidity}", humidity);         // Kelembaban (%)
            ReplaceInDoc(doc, "{TestStartDate}", testStartDate);    // Tanggal Mulai Uji
            ReplaceInDoc(doc, "{TestCompletiondate}", completionDate);   // Tanggal Selesai Uji
            ReplaceInDoc(doc, "{Equipment}", equipmentNames);   // Peralatan Uji
            ReplaceInDoc(doc, "{typeofmaterial}", typeofmaterial);      // Suhu (ºC)
            ReplaceInDoc(doc, "{coatingmateial}", coatingmaterial);      // Suhu (ºC)
            ReplaceInDoc(doc, "{Form}", Form);      // Suhu (ºC)
            ReplaceInDoc(doc, "{typeofproducts}", TypeOfProduct_header);
            ReplaceInDoc(doc, "{testingdate}", testingdate);
            ReplaceInDoc(doc, "{serialNo}", serialno);

            ReplaceInHeaders(doc, "{ReportName}", reportName);
            ReplaceInHeaders(doc, "{DocNo}", docNo);

        }
        private void ReplaceInHeaders(Microsoft.Office.Interop.Word.Document doc, string placeholder, string replacementText)
        {
            foreach (Microsoft.Office.Interop.Word.Section section in doc.Sections)
            {
                foreach (Microsoft.Office.Interop.Word.HeaderFooter header in section.Headers)
                {
                    Microsoft.Office.Interop.Word.Range range = header.Range;
                    range.Find.ClearFormatting();

                    bool found = range.Find.Execute(FindText: placeholder);
                    if (found)
                    {
                        range.Find.Execute(FindText: placeholder, ReplaceWith: replacementText,
                            Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
                    }
                }
            }
        }
        // ── HELPER:─────────────────────────────────
        private void ReplaceInDoc(Document doc, string findText, string replaceText)
        {
            if (doc == null)
            {
                throw new Exception(
                    "ReplaceInDoc: doc is NULL. " +
                    "findText = " + (findText ?? "NULL") +
                    ", replaceText = " + (replaceText ?? "NULL"));
            }

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Replacement.ClearFormatting();
            find.Text = findText;
            find.Replacement.Text = replaceText ?? "";
            find.Execute(
                FindText: findText,
                MatchCase: false,
                MatchWholeWord: false,
                MatchWildcards: false,
                MatchSoundsLike: false,
                MatchAllWordForms: false,
                Forward: true,
                Wrap: WdFindWrap.wdFindContinue,
                Format: false,
                ReplaceWith: replaceText ?? "",
                Replace: WdReplace.wdReplaceAll
            );
        }
        private void ReplaceInRange(Range range, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                if (!string.IsNullOrEmpty(replacement.Key))
                {
                    range.Find.ClearFormatting();
                    range.Find.Replacement.ClearFormatting();

                    object findText = replacement.Key;
                    object replaceText = replacement.Value ?? string.Empty;
                    object replaceAll = WdReplace.wdReplaceAll;

                    range.Find.Execute(
                        ref findText,
                        MatchCase: false,
                        MatchWholeWord: false,
                        MatchWildcards: false,
                        MatchSoundsLike: false,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: WdFindWrap.wdFindContinue,
                        Format: false,
                        ref replaceText,
                        ref replaceAll
                    );
                }
            }
        }
        private void ReplaceInAllHeaders(Document doc, Dictionary<string, string> replacements)
        {
            foreach (Section section in doc.Sections)
            {
                // Primary header
                if (section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Exists)
                {
                    ReplaceInRange(section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range, replacements);
                }

                // First page header
                if (section.Headers[WdHeaderFooterIndex.wdHeaderFooterFirstPage].Exists)
                {
                    ReplaceInRange(section.Headers[WdHeaderFooterIndex.wdHeaderFooterFirstPage].Range, replacements);
                }

                // Even pages header
                if (section.Headers[WdHeaderFooterIndex.wdHeaderFooterEvenPages].Exists)
                {
                    ReplaceInRange(section.Headers[WdHeaderFooterIndex.wdHeaderFooterEvenPages].Range, replacements);
                }
            }
        }
        private string FormatDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return string.Empty;

            try
            {
                DateTime date;
                if (DateTime.TryParse(dateString, out date))
                {
                    return date.ToString("dd/MM/yyyy");
                }
                return dateString; // Return original if parsing fails
            }
            catch
            {
                return dateString; // Return original if any error
            }
        }
        private void ReplaceEquipmentPlaceholder(Document doc, List<string> equipmentNames)
        {
            if (equipmentNames == null || equipmentNames.Count == 0)
                return;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = "{Equipment}";

            if (find.Execute())
            {
                Range equipmentRange = find.Parent as Range;
                if (equipmentRange != null)
                {
                    equipmentRange.Text = "";
                    for (int i = 0; i < equipmentNames.Count; i++)
                    {
                        equipmentRange.InsertAfter($"{i + 1}. {equipmentNames[i]}\r\n");
                        equipmentRange = doc.Range(equipmentRange.End, equipmentRange.End);
                    }
                }
            }
        }
        private List<string> GetEquipmentNamesAsList(string equipmentIds)
        {
            var equipmentList = new List<string>();

            if (string.IsNullOrEmpty(equipmentIds))
                return equipmentList;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string[] ids = equipmentIds.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToArray();

                    if (ids.Length == 0)
                        return equipmentList;

                    string parameterPlaceholders = string.Join(",", ids.Select((_, index) => $"@id{index}"));

                    string query = $@"SELECT * FROM Calibration_Equipment WHERE EpID IN ({parameterPlaceholders}) ORDER BY EpID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
                        }

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                equipmentList.Add(reader["Equipment"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting equipment names: {ex.Message}");
            }

            return equipmentList;
        }
        private string GetSafeString(DataRow row, string columnName)
        {
            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                {
                    return row[columnName].ToString();
                }
                return "[Not Found]"; // or return string.Empty;
            }
            catch
            {
                return "[Error]";
            }
        }
        private DataTable GetReportData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ClauseID", typeof(int));
            dt.Columns.Add("ClauseNumber", typeof(string));
            dt.Columns.Add("ClauseTitle", typeof(string));
            dt.Columns.Add("Requirement", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Condition", typeof(string));
            dt.Columns.Add("Specification", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Result", typeof(string));
            dt.Columns.Add("isHeader", typeof(string));
            dt.Columns.Add("isDescription", typeof(string));
            dt.Columns.Add("isRefre", typeof(string));
            dt.Columns.Add("isRefereRequirement", typeof(string));  // ── ADDED

            string standardID = lblstandardID.Text;
            DataTable clauseData = GetClauseData();
            DataTable savedResults = GetSavedResults();
            bool hasUnitColumn = savedResults != null && savedResults.Columns.Contains("UnitName");

            string[] ids = standardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> standardIdList = ids.Select(id => id.Trim()).ToList();

            if (standardIdList.Count > 0)
            {
                var matchingRows = clauseData.AsEnumerable().Where(r =>
                {
                    string clauseStandardIds = r["Standard_ID"].ToString();
                    string[] clauseIds = clauseStandardIds
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim()).ToArray();
                    return clauseIds.Any(cid => standardIdList.Contains(cid));
                });

                if (matchingRows.Any())
                {
                    foreach (DataRow clauseRow in matchingRows)
                    {
                        DataRow newRow = dt.NewRow();
                        int clauseID = Convert.ToInt32(clauseRow["ClauseID"]);

                        newRow["ClauseID"] = clauseID;
                        newRow["ClauseNumber"] = clauseRow["ClauseNumber"];
                        newRow["ClauseTitle"] = clauseRow["ClauseTitle"];
                        newRow["Requirement"] = clauseRow["Requirement"];
                        newRow["Description"] = clauseRow["Description"];
                        newRow["Condition"] = clauseRow["Condition"];
                        newRow["isHeader"] = clauseRow["isHeader"];
                        newRow["isDescription"] = clauseRow["isDescription"];

                        newRow["isRefre"] = clauseRow.Table.Columns.Contains("IsRefere")
                            && (clauseRow["IsRefere"].ToString() == "1"
                                || clauseRow["IsRefere"].ToString().ToLower() == "true") ? "1" : "0";

                        newRow["isRefereRequirement"] = clauseRow.Table.Columns.Contains("IsRefereRequirement")  // ── ADDED
                            && (clauseRow["IsRefereRequirement"].ToString() == "1"
                                || clauseRow["IsRefereRequirement"].ToString().ToLower() == "true") ? "1" : "0";

                        var savedRow = savedResults?.AsEnumerable()
                            .FirstOrDefault(r => Convert.ToInt32(r["ClauseID"]) == clauseID);

                        if (savedRow != null)
                        {
                            if (clauseRow["isDescription"].ToString() == "1")
                                newRow["Specification"] = savedRow["Remark"]?.ToString() ?? "";
                            else
                                newRow["Specification"] = !string.IsNullOrEmpty(savedRow["Specification"]?.ToString())
                                    ? savedRow["Specification"].ToString()
                                    : clauseRow["Specification"]?.ToString() ?? "";

                            newRow["Unit"] = hasUnitColumn ? (savedRow["UnitName"]?.ToString() ?? "") : "";
                            newRow["Result"] = newRow["isRefre"].ToString() == "1"
                                ? "Refer to Table"
                                : savedRow["Result"]?.ToString() ?? "";
                        }
                        else
                        {
                            newRow["Specification"] = clauseRow["Specification"]?.ToString() ?? "";
                            newRow["Unit"] = "";
                            newRow["Result"] = newRow["isRefre"].ToString() == "1" ? "Refer to Table" : "";
                        }

                        dt.Rows.Add(newRow);
                    }
                }
            }

            return dt;
        }
        //private DataTable GetReportDataForT2()
        //{
        //    DataTable dt = new DataTable();

        //    string query = @"SELECT * FROM TestResultsLED trl
        //                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
        //                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
        //                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID AND ClauseID = @ClauseID";

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlCommand cmd = new SqlCommand(query, conn))
        //    {
        //        cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
        //        cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
        //        cmd.Parameters.AddWithValue("@ClauseID", 0);

        //        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //        {
        //            conn.Open();
        //            da.Fill(dt);
        //        }
        //    }
        //    return dt;
        //}
        private DataTable GetReportDataForStrength()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (18967, 18968, 18969, 18970, 20014)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void ApplyBorders(Cell cell)
        {
            cell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
        }

        private void ApplySubTableVerticalBorders(Table subTable)
        {
            // Remove all borders first
            subTable.Borders.Enable = 0;

            // Apply only vertical border between columns (right border of first cell)
            foreach (Row subRow in subTable.Rows)
            {
                if (subRow.Cells.Count >= 2)
                {
                    // Apply right border only to the first cell to create vertical line between columns
                    Cell firstCell = subRow.Cells[1];
                    firstCell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                }
            }
        }
        private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

            float[] COLUMN_WIDTHS = { 0.6f, 2.50f, 0.70f, 1.17f, 1.30f };
            float SUB_COLUMN1_WIDTH = 1.5f;
            float SUB_COLUMN2_WIDTH = 1.0f;
            float TABLE_TOTAL_WIDTH = 6.27f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            int totalRows = 1;
            bool needDescHeader = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";

                if (isHeader) { totalRows++; needDescHeader = false; }
                else if (isDescription)
                {
                    if (!needDescHeader) { totalRows += 1; needDescHeader = true; }
                    totalRows++;
                }
                else { totalRows++; needDescHeader = false; }
            }

            Table table = doc.Tables.Add(range, totalRows, 5);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);
            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            for (int c = 1; c <= 5; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            string[] headers = {
        "Klausul\nClause",
        "Parameter Uji\nTest Parameters",
        "Satuan\nUnit",
        "Hasil Tes\nTest Results",
        "Persyaratan Mutu\nQuality Requirements"
    };

            for (int col = 1; col <= 5; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = HEADER_FONT_SIZE;
                cell.Range.Font.Name = DEFAULT_FONT_NAME;
                cell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                ApplyBorders(cell);
            }

            int r = 2;
            bool descHeaderShown = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isRefre = row.Table.Columns.Contains("isRefre") && row["isRefre"].ToString() == "1";
                bool isRefereReq = row.Table.Columns.Contains("isRefereRequirement") && row["isRefereRequirement"].ToString() == "1";  // ── ADDED

                if (isHeader)
                {
                    descHeaderShown = false;

                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, isRefre ? "Refer to Table" : row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
                else if (isDescription)
                {
                    if (!descHeaderShown)
                    {
                        SetCellText(table.Cell(r, 1), "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        Cell d2 = table.Cell(r, 2);
                        Table subTableHeader = d2.Range.Tables.Add(d2.Range, 1, 2);
                        subTableHeader.Borders.Enable = 0;
                        subTableHeader.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
                        subTableHeader.Range.Font.Size = DEFAULT_FONT_SIZE;
                        subTableHeader.Range.Font.Name = DEFAULT_FONT_NAME;
                        subTableHeader.Columns[1].SetWidth(wordApp.InchesToPoints(SUB_COLUMN1_WIDTH), WdRulerStyle.wdAdjustNone);
                        subTableHeader.Columns[2].SetWidth(wordApp.InchesToPoints(SUB_COLUMN2_WIDTH), WdRulerStyle.wdAdjustNone);

                        Cell subHeader1 = subTableHeader.Cell(1, 1);
                        SetCellText(subHeader1, "Description", true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                        subHeader1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        Cell subHeader2 = subTableHeader.Cell(1, 2);
                        SetCellText(subHeader2, "Condition (mm)", true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                        subHeader2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        ApplySubTableVerticalBorders(subTableHeader);
                        ApplyBorders(d2);

                        SetCellText(table.Cell(r, 3), "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                        SetCellText(table.Cell(r, 4), "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                        SetCellText(table.Cell(r, 5), "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        r++;
                        descHeaderShown = true;
                    }

                    SetCellText(table.Cell(r, 1), "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell a2 = table.Cell(r, 2);
                    Table subTableData = a2.Range.Tables.Add(a2.Range, 1, 2);
                    subTableData.Borders.Enable = 0;
                    subTableData.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
                    subTableData.Range.Font.Size = DEFAULT_FONT_SIZE;
                    subTableData.Range.Font.Name = DEFAULT_FONT_NAME;
                    subTableData.Columns[1].SetWidth(wordApp.InchesToPoints(SUB_COLUMN1_WIDTH), WdRulerStyle.wdAdjustNone);
                    subTableData.Columns[2].SetWidth(wordApp.InchesToPoints(SUB_COLUMN2_WIDTH), WdRulerStyle.wdAdjustNone);

                    // Col 2 sub: Description text — ── CHANGED
                    Cell subData1 = subTableData.Cell(1, 1);
                    SetCellText(subData1, isRefereReq ? "Refer to Requirement" : row["Description"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    subData1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell subData2 = subTableData.Cell(1, 2);
                    SetCellText(subData2, row["Condition"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    subData2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    ApplySubTableVerticalBorders(subTableData);
                    ApplyBorders(a2);

                    Cell a3 = table.Cell(r, 3);
                    SetCellText(a3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, isRefre ? "Refer to Table" : row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell a5 = table.Cell(r, 5);
                    SetCellText(a5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
                else
                {
                    descHeaderShown = false;

                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Requirement — ── CHANGED
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, isRefereReq ? "Refer to Requirement" : row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, isRefre ? "Refer to Table" : row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
            }
        }
        /* T2 Start */
        private void CreateReportTableStrength(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
{
    { 18967, 0 },  // Diameter
    { 18968, 1 },  // Test duration (minutes)
    { 18969, 2 },  // Test results
    { 18970, 3 }   // Decision
};


            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 4; i++)  // ── 
                parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(reportParamMapID))
                {
                    int columnIndex = columnMapping[reportParamMapID];
                    string[] values = row["Result"].ToString().Split(',');
                    parsedColumns[columnIndex] = values;
                }
            }

            int numRows = parsedColumns.Max(col => col.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 4;

            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            float[] colWidths = { 1.00f, 1.80f, 1.80f, 1.67f };  // ── FIXED: 
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── ROW 1: merged title ───────────────────────────────────────────────
            table.Rows[1].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell mergedRow1 = table.Cell(1, 1);
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            mergedRow1.Range.Text = "8.2\tTable: Product Thickness Measurement";
            mergedRow1.Range.Bold = 1;
            mergedRow1.Range.Font.Size = 10;
            mergedRow1.Range.Font.Name = "Calibri";
            mergedRow1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow1.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow1.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 2: sub-title ─────────────────────────────────────────────────
            Cell mergedRow2 = table.Cell(2, 1);
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            mergedRow2.Range.Text = "8.2.1\tTableware";
            mergedRow2.Range.Bold = 1;
            mergedRow2.Range.Font.Size = 10;
            mergedRow2.Range.Font.Name = "Calibri";
            mergedRow2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow2.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow2.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 3: column headers ─────────────────────────────────────────────
            string[] headers = {
    "Diameter",
    "Test duration (minutes)",
    "Test results",
    "Decision"
};

            table.Rows[3].Height = wordApp.InchesToPoints(0.55f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 12;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── DATA ROWS ─────────────────────────────────────────────────────────
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length)
                        ? parsedColumns[colIndex][rowIndex].Trim()
                        : "";

                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10;
                    cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0;
                    cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                    if (colIndex == 0 || colIndex == 1)
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    else
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // ── ADDITIONAL INFO ROW ───────────────────────────────────────────────
            int additionalInfoRow = numRows + 4;
            table.Rows[additionalInfoRow].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[additionalInfoRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            table.Cell(additionalInfoRow, 1).Merge(table.Cell(additionalInfoRow, numCols));
            Cell additionalInfoCell = table.Cell(additionalInfoRow, 1);

            string additionalInfoText = "Additional information: ";
            DataRow additionalRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 18971);  // ──
            if (additionalRow != null)
                additionalInfoText += additionalRow["Result"]?.ToString() ?? "";

            additionalInfoCell.Range.Text = additionalInfoText;
            additionalInfoCell.Range.Bold = 0;
            additionalInfoCell.Range.Font.Size = 10;
            additionalInfoCell.Range.Font.Name = "Calibri";
            additionalInfoCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            additionalInfoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            additionalInfoCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            additionalInfoCell.Range.ParagraphFormat.SpaceBefore = 0;
            additionalInfoCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        private void SetCellText(Cell cell, string text, bool bold, float fontSize, string fontName)
        {
            cell.Range.Text = text;
            cell.Range.Bold = bold ? 1 : 0;
            cell.Range.Font.Size = fontSize;
            cell.Range.Font.Name = fontName;
            ApplyBorders(cell);
        }

        private void CreateGridTable_StrengthTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Diameter");
            dt.Columns.Add("TestDuration");
            dt.Columns.Add("TestResults");
            dt.Columns.Add("Decision");
            ViewState["TestData"] = dt;
            // dt.Columns.Add("ConformityMark");
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        protected void btnAddStrengthTable_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["Diameter"] = txtDiameter.Text.Trim();
            dr["TestDuration"] = txtTestDuration.Text.Trim();
            dr["TestResults"] = txtTestResults.Text.Trim();
            dr["Decision"] = txtDecision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();

            txtDiameter.Text = "";
            txtTestDuration.Text = "";
            txtTestResults.Text = "";
            txtDecision.Text = "";
        }

        protected void gvTestsStrengthTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        private void CreateEquipmentTable(Application wordApp, Document doc, string placeholder)
        {
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            float TABLE_TOTAL_WIDTH = 6.27f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // ── FETCH DATA ───────────────────────────────────────────────────────
            string testMethodNo = "";
            string serialNo = "";
            string roomTempHumidity = "";
            string placeDateTesting = "";
            string equipmentNames = "";

            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 
            TestMethod, TestEquipmentNumber, Temperature, Humidity, EquipmentIds
        FROM TestResultsLED 
        WHERE SampleID = @SampleID AND ReportID = @ReportID
        ORDER BY ResultID ASC", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleId);
                cmd.Parameters.AddWithValue("@ReportID", reportId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        testMethodNo = reader["TestMethod"]?.ToString() ?? "";
                        serialNo = reader["TestEquipmentNumber"]?.ToString() ?? "";
                        roomTempHumidity = reader["Temperature"]?.ToString() ?? "";
                        placeDateTesting = reader["Humidity"]?.ToString() ?? "";
                        string equipmentIds = reader["EquipmentIds"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(equipmentIds))
                            equipmentNames = GetEquipmentNamesByIds(equipmentIds);
                    }
                }
            }

            // ── BUILD TABLE ──────────────────────────────────────────────────────
            range.Delete(); // ← DELETE placeholder first before adding table

            Table table = doc.Tables.Add(range, 2, 3);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);
            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            table.Columns[1].SetWidth(wordApp.InchesToPoints(2.80f), WdRulerStyle.wdAdjustNone);
            table.Columns[2].SetWidth(wordApp.InchesToPoints(1.60f), WdRulerStyle.wdAdjustNone);
            table.Columns[3].SetWidth(wordApp.InchesToPoints(1.87f), WdRulerStyle.wdAdjustNone);

            // ── ROW 1 COL 1: Equipment List ──────────────────────────────────────
            Cell equipCell = table.Cell(1, 1);
            equipCell.Range.Delete();
            equipCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;

            // Insert header bold first
            Range equipRange = equipCell.Range;
            equipRange.Collapse(WdCollapseDirection.wdCollapseStart);
            equipRange.Text = "Test equipment / Equipment no:";
            equipRange.Bold = 1;
            equipRange.Font.Size = DEFAULT_FONT_SIZE;
            equipRange.Font.Name = DEFAULT_FONT_NAME;

            // Insert each equipment item
            if (!string.IsNullOrEmpty(equipmentNames))
            {
                string[] equipList = equipmentNames.Split(',');
                foreach (string equip in equipList)
                {
                    if (!string.IsNullOrEmpty(equip.Trim()))
                    {
                        // Move to end of cell and insert new paragraph
                        Range endRange = equipCell.Range;
                        endRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                        endRange.InsertParagraphBefore();
                        endRange.Collapse(WdCollapseDirection.wdCollapseStart);
                        endRange.Text = "□ " + equip.Trim();
                        endRange.Bold = 0;
                        endRange.Font.Size = DEFAULT_FONT_SIZE;
                        endRange.Font.Name = DEFAULT_FONT_NAME;
                    }
                }
            }
            // ── ROW 1 COL 2: Labels ──────────────────────────────────────────────
            Cell labelCell = table.Cell(1, 2);
            labelCell.Range.Delete();  // ← clear cell first
            labelCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;

            string[] labels = {
        "Test method No.",
        "Serial No.",
        "Room Temp. Humidity",
        "Place / Date testing"
    };

            Range labelRange = labelCell.Range;
            labelRange.Collapse(WdCollapseDirection.wdCollapseStart);
            labelRange.InsertAfter(string.Join("\n", labels));
            labelRange.Font.Size = DEFAULT_FONT_SIZE;
            labelRange.Font.Name = DEFAULT_FONT_NAME;

            // ── ROW 1 COL 3: Values ──────────────────────────────────────────────
            Cell valueCell = table.Cell(1, 3);
            valueCell.Range.Delete();  // ← clear cell first
            valueCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;

            string[] values = {
        ": " + testMethodNo,
        ": " + serialNo,
        ": " + roomTempHumidity,
        ": Qualis 1, " + placeDateTesting
    };

            Range valueRange = valueCell.Range;
            valueRange.Collapse(WdCollapseDirection.wdCollapseStart);
            valueRange.InsertAfter(string.Join("\n", values));
            valueRange.Font.Size = DEFAULT_FONT_SIZE;
            valueRange.Font.Name = DEFAULT_FONT_NAME;

            // ── ROW 2: Examiner | Checked | Approval | Decision ──────────────────
            table.Rows[2].Height = wordApp.InchesToPoints(1.0f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightAtLeast;

            // Col 1: Examiner
            Cell row2c1 = table.Cell(2, 1);
            row2c1.Range.Delete();
            row2c1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom;
            Range r2c1Range = row2c1.Range;
            r2c1Range.Collapse(WdCollapseDirection.wdCollapseStart);
            r2c1Range.InsertAfter("Examiner:_______________");
            r2c1Range.Font.Size = DEFAULT_FONT_SIZE;
            r2c1Range.Font.Name = DEFAULT_FONT_NAME;

            // Col 2: Checked
            Cell row2c2 = table.Cell(2, 2);
            row2c2.Range.Delete();
            row2c2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom;
            Range r2c2Range = row2c2.Range;
            r2c2Range.Collapse(WdCollapseDirection.wdCollapseStart);
            r2c2Range.InsertAfter("Checked :\n_______________");
            r2c2Range.Font.Size = DEFAULT_FONT_SIZE;
            r2c2Range.Font.Name = DEFAULT_FONT_NAME;

            // Col 3: Approval + Decision via nested table
            Cell row2c3 = table.Cell(2, 3);
            row2c3.Range.Delete();

            Table nestedTable = row2c3.Range.Tables.Add(row2c3.Range, 1, 2);
            nestedTable.Borders.Enable = 1;
            nestedTable.Columns[1].SetWidth(wordApp.InchesToPoints(0.93f), WdRulerStyle.wdAdjustNone);
            nestedTable.Columns[2].SetWidth(wordApp.InchesToPoints(0.93f), WdRulerStyle.wdAdjustNone);

            Cell approvalCell = nestedTable.Cell(1, 1);
            approvalCell.Range.Delete();
            approvalCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom;
            SetCellText(approvalCell, "Approval: _______________", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

            Cell decisionCell = nestedTable.Cell(1, 2);
            decisionCell.Range.Delete();
            decisionCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
            SetCellText(decisionCell, "Decision:", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

            // Remove spacing
            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        // ── HELPER: Get equipment names from comma-separated IDs ─────────────────
        private string GetEquipmentNamesByIds(string equipmentIds)
        {
            List<string> names = new List<string>();
            string[] ids = equipmentIds.Split(',');

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string inClause = string.Join(",", ids.Select(id => id.Trim()));

                string query = $"SELECT Equipment FROM Calibration_Equipment WHERE EpID IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            names.Add(reader["Equipment"].ToString());
                    }
                }
            }
            return string.Join(", ", names);
        }
        private void PrefillTestResultsGrid(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("Diameter");
            gridDt.Columns.Add("TestDuration");
            gridDt.Columns.Add("TestResults");
            gridDt.Columns.Add("Decision");

            string diameterCsv = GetResultValue(dt, "18967");
            string durationCsv = GetResultValue(dt, "18968");
            string testResultsCsv = GetResultValue(dt, "18969");
            string decisionCsv = GetResultValue(dt, "18970");

            if (!string.IsNullOrEmpty(diameterCsv))
            {
                string[] diameters = diameterCsv.Split(',');
                string[] durations = durationCsv?.Split(',') ?? new string[0];
                string[] testResults = testResultsCsv?.Split(',') ?? new string[0];
                string[] decisions = decisionCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < diameters.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["Diameter"] = diameters[i].Trim();
                    dr["TestDuration"] = (i < durations.Length) ? durations[i].Trim() : "";
                    dr["TestResults"] = (i < testResults.Length) ? testResults[i].Trim() : "";
                    dr["Decision"] = (i < decisions.Length) ? decisions[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData"] = gridDt;
            gvTests.DataSource = gridDt;
            gvTests.DataBind();
        }
        private void PrefillAllData(DataTable dt)
        {
            //PrefillSingleValues(dt);
            // PrefillTwoValues(dt);
            // PrefillSixValues(dt);
        }
        //private void PrefillSingleValues(DataTable dt)
        //{
        //    Dictionary<string, TextBox> singleValueMap = new Dictionary<string, TextBox>
        //    {
        //        { "17954", txtGroup },
        //        { "17955", txtMeasurementResults },
        //        { "17956", txtMinThicknessRequirement},
        //        { "17958", txtDecision },
        //        { "17957", txtTypeOfProducts},
        //        { "17960", txtAdditionalinfo}

        //    };

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string paramId = row["ParameterID"].ToString();
        //        string rawValue = row["Result"].ToString();

        //        if (singleValueMap.ContainsKey(paramId))
        //        {
        //            singleValueMap[paramId].Text = rawValue;
        //        }
        //    }
        //}


        /********************************Table 2*************************************/
        //private void CreateGridTable_cookingequ()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Group_cookingequ");
        //    dt.Columns.Add("TypeOfProducts_cookingequ");
        //    dt.Columns.Add("MeasurementResults_cookingequ");
        //    dt.Columns.Add("MinThicknessRequirement_cookingequ");
        //    dt.Columns.Add("Decision_cookingequ");
        //    // dt.Columns.Add("ConformityMark");
        //    ViewState["TestData_cookingequ"] = dt;
        //    gvTests.DataSource = dt;
        //    gvTests.DataBind();
        //}

        //protected void btnAddcookingequ_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = ViewState["TestData_cookingequ"] as DataTable;
        //    DataRow dr = dt.NewRow();
        //    dr["Group_cookingequ"] = txtGroupcookingequi.Text.Trim();
        //    dr["TypeOfProducts_cookingequ"] = txtTypeOfProductscookingequi.Text.Trim();
        //    dr["MeasurementResults_cookingequ"] = txtMeasurementResultscookingequi.Text.Trim();
        //    dr["MinThicknessRequirement_cookingequ"] = txtMinThicknessRequirementcookingequi.Text.Trim();
        //    dr["Decision_cookingequ"] = txtDecisioncookingequi.Text.Trim();
        //    //dr["ConformityMark"] = txtConformityMark.Text.Trim();
        //    dt.Rows.Add(dr);
        //    ViewState["TestData_cookingequ"] = dt;
        //    gvcooking.DataSource = dt;
        //    gvcooking.DataBind();

        //    // Clear TextBoxes after adding
        //    txtGroupcookingequi.Text = "";
        //    txtTypeOfProductscookingequi.Text = "";
        //    txtMeasurementResultscookingequi.Text = "";
        //    txtMinThicknessRequirementcookingequi.Text = "";
        //    txtDecisioncookingequi.Text = "";
        //    //txtConformityMark.Text = "";
        //}
        //protected void gvcooking_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    DataTable dt = ViewState["TestData_cookingequ"] as DataTable;
        //    dt.Rows.RemoveAt(e.RowIndex);
        //    ViewState["TestData_cookingequ"] = dt;
        //    gvTests.DataSource = dt;
        //    gvTests.DataBind();
        //}
        private void PrefillTestResultsGrid_cookingequ(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("Group_cookingequ");
            gridDt.Columns.Add("TypeOfProducts_cookingequ");
            gridDt.Columns.Add("MeasurementResults_cookingequ");
            gridDt.Columns.Add("MinThicknessRequirement_cookingequ");
            gridDt.Columns.Add("Decision_cookingequ");

            //
            string groupCsv_cookingequ = GetResultValue(dt, "18964");
            string productTypeCsv_cookingequ = GetResultValue(dt, "18961");
            string measurementsCsv_cookingequ = GetResultValue(dt, "18965");
            string minThicknessCsv_cookingequ = GetResultValue(dt, "18966");
            string decisionCsv_cookingequ = GetResultValue(dt, "18962");
            // 17835 = Standard/SNI — add a column if needed

            if (!string.IsNullOrEmpty(groupCsv_cookingequ))
            {
                string[] groups = groupCsv_cookingequ.Split(',');
                string[] productTypes = productTypeCsv_cookingequ?.Split(',') ?? new string[0];
                string[] measurements = measurementsCsv_cookingequ?.Split(',') ?? new string[0];
                string[] minThicknesses = minThicknessCsv_cookingequ?.Split(',') ?? new string[0];
                string[] decisions = decisionCsv_cookingequ?.Split(',') ?? new string[0];

                for (int i = 0; i < groups.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["Group_cookingequ"] = groups[i].Trim();
                    dr["TypeOfProducts_cookingequ"] = (i < productTypes.Length) ? productTypes[i].Trim() : "";
                    dr["MeasurementResults_cookingequ"] = (i < measurements.Length) ? measurements[i].Trim() : "";
                    dr["MinThicknessRequirement_cookingequ"] = (i < minThicknesses.Length) ? minThicknesses[i].Trim() : "";
                    dr["Decision_cookingequ"] = (i < decisions.Length) ? decisions[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_cookingequ"] = gridDt;
            //gvcooking.DataSource = gridDt;
            // gvcooking.DataBind();
        }
        private void PrefillAllData_cookingequ(DataTable dt)
        {
            // PrefillSingleValues_cookingequ(dt);
            // PrefillTwoValues(dt);
            // PrefillSixValues(dt);
        }
        //private void PrefillSingleValues_cookingequ(DataTable dt)
        //{
        //    Dictionary<string, TextBox> singleValueMap = new Dictionary<string, TextBox>
        //    {
        //        { "18964", txtGroupcookingequi },
        //        { "18965", txtMeasurementResultscookingequi },
        //        { "18966", txtMinThicknessRequirementcookingequi},
        //        { "18962", txtDecisioncookingequi },
        //        { "18961", txtTypeOfProductscookingequi},
        //        { "18963", txtAdditionalinfocookingequi}

        //    };

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string paramId = row["ParameterID"].ToString();
        //        string rawValue = row["Result"].ToString();

        //        if (singleValueMap.ContainsKey(paramId))
        //        {
        //            singleValueMap[paramId].Text = rawValue;
        //        }
        //    }
        //}
        private string GetResultValue(DataTable dt, string parameterId)
        {
            DataRow[] rows = dt.Select($"ParameterID = '{parameterId}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
        }


        /******************************************************Form Test************************************************************************************/
        private void CreateGridTable_FormTest()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("D1");
            dt.Columns.Add("D2");
            dt.Columns.Add("RoundnessValue");
            dt.Columns.Add("RoundnessValueTolerance");
            dt.Columns.Add("Decision");
            ViewState["TestData_FormTest"] = dt;
            gvFormTest.DataSource = dt;
            gvFormTest.DataBind();
        }

        protected void btnAdd_FormTest_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_FormTest"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["D1"] = txtFormTest_D1.Text.Trim();
            dr["D2"] = txtFormTest_D2.Text.Trim();
            dr["RoundnessValue"] = txtFormTest_RoundnessValue.Text.Trim();
            dr["RoundnessValueTolerance"] = txtFormTest_RoundnessTolerance.Text.Trim();
            dr["Decision"] = txtFormTest_Decision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_FormTest"] = dt;
            gvFormTest.DataSource = dt;
            gvFormTest.DataBind();

            txtFormTest_D1.Text = "";
            txtFormTest_D2.Text = "";
            txtFormTest_RoundnessValue.Text = "";
            txtFormTest_RoundnessTolerance.Text = "";
            txtFormTest_Decision.Text = "";
        }

        protected void gvFormTest_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_FormTest"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_FormTest"] = dt;
            gvFormTest.DataSource = dt;
            gvFormTest.DataBind();
        }

        // PrefillFormTest
        private void PrefillFormTest(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("D1");
            gridDt.Columns.Add("D2");
            gridDt.Columns.Add("RoundnessValue");
            gridDt.Columns.Add("RoundnessValueTolerance");
            gridDt.Columns.Add("Decision");

            string d1Csv = GetResultValue(dt, "19967");
            string d2Csv = GetResultValue(dt, "19968");
            string rvCsv = GetResultValue(dt, "19969");
            string rvtCsv = GetResultValue(dt, "19970");
            string decCsv = GetResultValue(dt, "19971");
            txtFormTestAdditionalinfo.Text = GetResultValue(dt, "19972");

            if (!string.IsNullOrEmpty(d1Csv))
            {
                string[] d1s = d1Csv.Split(',');
                string[] d2s = d2Csv?.Split(',') ?? new string[0];
                string[] rvs = rvCsv?.Split(',') ?? new string[0];
                string[] rvts = rvtCsv?.Split(',') ?? new string[0];
                string[] decs = decCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < d1s.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["D1"] = d1s[i].Trim();
                    dr["D2"] = (i < d2s.Length) ? d2s[i].Trim() : "";
                    dr["RoundnessValue"] = (i < rvs.Length) ? rvs[i].Trim() : "";
                    dr["RoundnessValueTolerance"] = (i < rvts.Length) ? rvts[i].Trim() : "";
                    dr["Decision"] = (i < decs.Length) ? decs[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_FormTest"] = gridDt;
            gvFormTest.DataSource = gridDt;
            gvFormTest.DataBind();
        }

        private DataTable GetReportDataForFormTest()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19967, 19968, 19969, 19970, 19971, 19972)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void CreateReportTableFormTest(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19967, 0 },  // D1
        { 19968, 1 },  // D2
        { 19969, 2 },  // Roundness value
        { 19970, 3 },  // Roundness value tolerance Max 2%
        { 19971, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.00f, 1.00f, 1.27f, 1.80f, 1.20f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.2\tForm test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Sub-title
            table.Rows[2].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            Cell r2 = table.Cell(2, 1);
            r2.Range.Text = "6.2.1\tMeasuring round cookware";
            r2.Range.Bold = 1; r2.Range.Font.Size = 10; r2.Range.Font.Name = "Calibri";
            r2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r2.Range.ParagraphFormat.SpaceBefore = 0; r2.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 3: Column headers
            string[] headers = { "D1", "D2", "Roundness value", "Roundness value tolerance\r\nMax 2%", "Decision" };
            table.Rows[3].Height = wordApp.InchesToPoints(0.45f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 4;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19972);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /*************************************************** Base Material************************************************************************/
        private void CreateGridTable_BaseMaterial()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Angle0");
            dt.Columns.Add("Angle90");
            dt.Columns.Add("Angle180");
            dt.Columns.Add("Angle270");
            dt.Columns.Add("Average");
            dt.Columns.Add("ThicknessMin");
            dt.Columns.Add("Decision");
            ViewState["TestData_BaseMaterial"] = dt;
            gvBaseMaterial.DataSource = dt;
            gvBaseMaterial.DataBind();
        }

        protected void btnAdd_BaseMaterial_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_BaseMaterial"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["Angle0"] = txtBaseMaterial_Angle0.Text.Trim();
            dr["Angle90"] = txtBaseMaterial_Angle90.Text.Trim();
            dr["Angle180"] = txtBaseMaterial_Angle180.Text.Trim();
            dr["Angle270"] = txtBaseMaterial_Angle270.Text.Trim();
            dr["Average"] = txtBaseMaterial_Average.Text.Trim();
            dr["ThicknessMin"] = txtBaseMaterial_Thickness.Text.Trim();
            dr["Decision"] = txtBaseMaterial_Decision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_BaseMaterial"] = dt;
            gvBaseMaterial.DataSource = dt;
            gvBaseMaterial.DataBind();

            txtBaseMaterial_Angle0.Text = "";
            txtBaseMaterial_Angle90.Text = "";
            txtBaseMaterial_Angle180.Text = "";
            txtBaseMaterial_Angle270.Text = "";
            txtBaseMaterial_Average.Text = "";
            txtBaseMaterial_Thickness.Text = "";
            txtBaseMaterial_Decision.Text = "";
        }

        protected void gvBaseMaterial_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_BaseMaterial"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_BaseMaterial"] = dt;
            gvBaseMaterial.DataSource = dt;
            gvBaseMaterial.DataBind();
        }

        private void PrefillBaseMaterial(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("Angle0");
            gridDt.Columns.Add("Angle90");
            gridDt.Columns.Add("Angle180");
            gridDt.Columns.Add("Angle270");
            gridDt.Columns.Add("Average");
            gridDt.Columns.Add("ThicknessMin");
            gridDt.Columns.Add("Decision");

            string a0Csv = GetResultValue(dt, "19973");
            string a90Csv = GetResultValue(dt, "19974");
            string a180Csv = GetResultValue(dt, "19975");
            string a270Csv = GetResultValue(dt, "19976");
            string avgCsv = GetResultValue(dt, "19977");
            string thkCsv = GetResultValue(dt, "19978");
            string decCsv = GetResultValue(dt, "19979");

            if (!string.IsNullOrEmpty(a0Csv))
            {
                string[] a0s = a0Csv.Split(',');
                string[] a90s = a90Csv?.Split(',') ?? new string[0];
                string[] a180s = a180Csv?.Split(',') ?? new string[0];
                string[] a270s = a270Csv?.Split(',') ?? new string[0];
                string[] avgs = avgCsv?.Split(',') ?? new string[0];
                string[] thks = thkCsv?.Split(',') ?? new string[0];
                string[] decs = decCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < a0s.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["Angle0"] = a0s[i].Trim();
                    dr["Angle90"] = (i < a90s.Length) ? a90s[i].Trim() : "";
                    dr["Angle180"] = (i < a180s.Length) ? a180s[i].Trim() : "";
                    dr["Angle270"] = (i < a270s.Length) ? a270s[i].Trim() : "";
                    dr["Average"] = (i < avgs.Length) ? avgs[i].Trim() : "";
                    dr["ThicknessMin"] = (i < thks.Length) ? thks[i].Trim() : "";
                    dr["Decision"] = (i < decs.Length) ? decs[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_BaseMaterial"] = gridDt;
            gvBaseMaterial.DataSource = gridDt;
            gvBaseMaterial.DataBind();
        }

        private DataTable GetReportDataForBaseMaterial()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19973, 19974, 19975, 19976, 19977, 19978, 19979,20013)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void CreateReportTableBaseMaterial(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19973, 0 },  // 0° angle
        { 19974, 1 },  // 90° angle
        { 19975, 2 },  // 180° angle
        { 19976, 3 },  // 270° angle
        { 19977, 4 },  // Average
        { 19978, 5 },  // Thickness(t) Min(mm)
        { 19979, 6 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 7; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 7;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.87f, 0.40f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.3\tBase material thickness test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = {
        "0° angle\r\npoint", "90° angle\r\npoint", "180° angle\r\npoint", "270° angle\r\npoint",
        "Average", "Thickness (t)\r\nMin (mm)", "Decision"
    };
            table.Rows[2].Height = wordApp.InchesToPoints(0.50f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            addCell.Range.Text = "Additional information: ";
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }


        /***********************************************************Fatigue Resistance***********************************************************************************/
        private void CreateGridTable_FatigueResistance()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("WaterVolume");
            dt.Columns.Add("WaterVolume5x15");
            dt.Columns.Add("NumberOfCycles");
            dt.Columns.Add("TestResults");
            dt.Columns.Add("Decision");
            ViewState["TestData_FatigueResistance"] = dt;
            gvFatigueResistance.DataSource = dt;
            gvFatigueResistance.DataBind();
        }

        protected void btnAdd_FatigueResistance_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_FatigueResistance"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["WaterVolume"] = txtFatigue_WaterVolume.Text.Trim();
            dr["WaterVolume5x15"] = txtFatigue_WaterVolume5x15.Text.Trim();
            dr["NumberOfCycles"] = txtFatigue_NumberOfCycles.Text.Trim();
            dr["TestResults"] = txtFatigue_TestResults.Text.Trim();
            dr["Decision"] = txtFatigue_Decision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_FatigueResistance"] = dt;
            gvFatigueResistance.DataSource = dt;
            gvFatigueResistance.DataBind();

            txtFatigue_WaterVolume.Text = "";
            txtFatigue_WaterVolume5x15.Text = "";
            txtFatigue_NumberOfCycles.Text = "";
            txtFatigue_TestResults.Text = "";
            txtFatigue_Decision.Text = "";
        }

        protected void gvFatigueResistance_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_FatigueResistance"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_FatigueResistance"] = dt;
            gvFatigueResistance.DataSource = dt;
            gvFatigueResistance.DataBind();
        }

        private void PrefillFatigueResistance(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("WaterVolume");
            gridDt.Columns.Add("WaterVolume5x15");
            gridDt.Columns.Add("NumberOfCycles");
            gridDt.Columns.Add("TestResults");
            gridDt.Columns.Add("Decision");

            string wvCsv = GetResultValue(dt, "19980");
            string wv5Csv = GetResultValue(dt, "19981");
            string ncCsv = GetResultValue(dt, "19982");
            string trCsv = GetResultValue(dt, "19983");
            string decCsv = GetResultValue(dt, "19985");
            txtFatigueAdditionalinfo.Text = GetResultValue(dt, "19984");

            if (!string.IsNullOrEmpty(wvCsv))
            {
                string[] wvs = wvCsv.Split(',');
                string[] wv5s = wv5Csv?.Split(',') ?? new string[0];
                string[] ncs = ncCsv?.Split(',') ?? new string[0];
                string[] trs = trCsv?.Split(',') ?? new string[0];
                string[] decs = decCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < wvs.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["WaterVolume"] = wvs[i].Trim();
                    dr["WaterVolume5x15"] = (i < wv5s.Length) ? wv5s[i].Trim() : "";
                    dr["NumberOfCycles"] = (i < ncs.Length) ? ncs[i].Trim() : "";
                    dr["TestResults"] = (i < trs.Length) ? trs[i].Trim() : "";
                    dr["Decision"] = (i < decs.Length) ? decs[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_FatigueResistance"] = gridDt;
            gvFatigueResistance.DataSource = gridDt;
            gvFatigueResistance.DataBind();
        }
        private DataTable GetReportDataForFatigueResistance()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19980, 19981, 19982, 19983, 19984, 19985)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void CreateReportTableFatigueResistance(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19980, 0 },  // Water volume
        { 19981, 1 },  // 5 x 1.5 Water volume (V)
        { 19982, 2 },  // Number of cycles 15,000 times
        { 19983, 3 },  // Test results
        { 19985, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.10f, 1.30f, 1.40f, 1.47f, 1.00f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.4.2\tFatigue resistance test on handle strength";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = {
        "Water volume", "5 x 1.5 Water\r\nvolume (V)",
        "Number of cycles\r\n15,000 times", "Test results", "Decision"
    };
            table.Rows[2].Height = wordApp.InchesToPoints(0.50f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19984);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }


        /********************************************************NonStick Coating***********************************************************************/
        private void CreateGridTable_NonStickCoating()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SoakingDuration");
            dt.Columns.Add("NumberOfScratches");
            dt.Columns.Add("StrokeDistance");
            dt.Columns.Add("TestResults");
            dt.Columns.Add("Decision");
            ViewState["TestData_NonStickCoating"] = dt;
            gvNonStickCoating.DataSource = dt;
            gvNonStickCoating.DataBind();
        }

        protected void btnAdd_NonStickCoating_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_NonStickCoating"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["SoakingDuration"] = txtNonStick_SoakingDuration.Text.Trim();
            dr["NumberOfScratches"] = txtNonStick_NumberOfScratches.Text.Trim();
            dr["StrokeDistance"] = txtNonStick_StrokeDistance.Text.Trim();
            dr["TestResults"] = txtNonStick_TestResults.Text.Trim();
            dr["Decision"] = txtNonStick_Decision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_NonStickCoating"] = dt;
            gvNonStickCoating.DataSource = dt;
            gvNonStickCoating.DataBind();

            txtNonStick_SoakingDuration.Text = "";
            txtNonStick_NumberOfScratches.Text = "";
            txtNonStick_StrokeDistance.Text = "";
            txtNonStick_TestResults.Text = "";
            txtNonStick_Decision.Text = "";
        }

        protected void gvNonStickCoating_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_NonStickCoating"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_NonStickCoating"] = dt;
            gvNonStickCoating.DataSource = dt;
            gvNonStickCoating.DataBind();
        }

        private void PrefillNonStickCoating(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("SoakingDuration");
            gridDt.Columns.Add("NumberOfScratches");
            gridDt.Columns.Add("StrokeDistance");
            gridDt.Columns.Add("TestResults");
            gridDt.Columns.Add("Decision");

            string sdCsv = GetResultValue(dt, "19986");
            string nsCsv = GetResultValue(dt, "19987");
            string stCsv = GetResultValue(dt, "19988");
            string trCsv = GetResultValue(dt, "19989");
            string decCsv = GetResultValue(dt, "19990");
            txtNonStickAdditionalinfo.Text = GetResultValue(dt, "19991");

            if (!string.IsNullOrEmpty(sdCsv))
            {
                string[] sds = sdCsv.Split(',');
                string[] nss = nsCsv?.Split(',') ?? new string[0];
                string[] sts = stCsv?.Split(',') ?? new string[0];
                string[] trs = trCsv?.Split(',') ?? new string[0];
                string[] decs = decCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < sds.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["SoakingDuration"] = sds[i].Trim();
                    dr["NumberOfScratches"] = (i < nss.Length) ? nss[i].Trim() : "";
                    dr["StrokeDistance"] = (i < sts.Length) ? sts[i].Trim() : "";
                    dr["TestResults"] = (i < trs.Length) ? trs[i].Trim() : "";
                    dr["Decision"] = (i < decs.Length) ? decs[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_NonStickCoating"] = gridDt;
            gvNonStickCoating.DataSource = gridDt;
            gvNonStickCoating.DataBind();
        }

        private DataTable GetReportDataForNonStickCoating()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19986, 19987, 19988, 19989, 19990, 19991)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void CreateReportTableNonStickCoating(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19986, 0 },  // Soaking duration (minutes)
        { 19987, 1 },  // Number of scratches
        { 19988, 2 },  // Stroke distance
        { 19989, 3 },  // Test results
        { 19990, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.40f, 1.20f, 1.20f, 1.47f, 1.00f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.6\tNon-stick coating adhesion test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = {
        "Soaking duration\r\n(minutes)", "Number of\r\nscratches",
        "Stroke\r\ndistance", "Test results", "Decision"
    };
            table.Rows[2].Height = wordApp.InchesToPoints(0.50f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19991);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /****************************************Thermal Shock********************************************************/
        private void CreateGridTable_ThermalShock()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TestTemp1");
            dt.Columns.Add("TestTemp2");
            dt.Columns.Add("TestResults");
            dt.Columns.Add("Decision");
            ViewState["TestData_ThermalShock"] = dt;
            gvThermalShock.DataSource = dt;
            gvThermalShock.DataBind();
        }

        protected void btnAdd_ThermalShock_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_ThermalShock"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["TestTemp1"] = txtThermal_Temp1.Text.Trim();
            dr["TestTemp2"] = txtThermal_Temp2.Text.Trim();
            dr["TestResults"] = txtThermal_TestResults.Text.Trim();
            dr["Decision"] = txtThermal_Decision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_ThermalShock"] = dt;
            gvThermalShock.DataSource = dt;
            gvThermalShock.DataBind();

            txtThermal_Temp1.Text = "";
            txtThermal_Temp2.Text = "";
            txtThermal_TestResults.Text = "";
            txtThermal_Decision.Text = "";
        }

        protected void gvThermalShock_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_ThermalShock"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_ThermalShock"] = dt;
            gvThermalShock.DataSource = dt;
            gvThermalShock.DataBind();
        }

        private void PrefillThermalShock(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("TestTemp1");
            gridDt.Columns.Add("TestTemp2");
            gridDt.Columns.Add("TestResults");
            gridDt.Columns.Add("Decision");

            string t1Csv = GetResultValue(dt, "19992");
            string t2Csv = GetResultValue(dt, "19993");
            string trCsv = GetResultValue(dt, "19994");
            string decCsv = GetResultValue(dt, "19995");
            txtThermalAdditionalinfo.Text = GetResultValue(dt, "19996");

            if (!string.IsNullOrEmpty(t1Csv))
            {
                string[] t1s = t1Csv.Split(',');
                string[] t2s = t2Csv?.Split(',') ?? new string[0];
                string[] trs = trCsv?.Split(',') ?? new string[0];
                string[] decs = decCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < t1s.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["TestTemp1"] = t1s[i].Trim();
                    dr["TestTemp2"] = (i < t2s.Length) ? t2s[i].Trim() : "";
                    dr["TestResults"] = (i < trs.Length) ? trs[i].Trim() : "";
                    dr["Decision"] = (i < decs.Length) ? decs[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_ThermalShock"] = gridDt;
            gvThermalShock.DataSource = gridDt;
            gvThermalShock.DataBind();
        }
        private DataTable GetReportDataForThermalShock()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19992, 19993, 19994, 19995, 19996)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void CreateReportTableThermalShock(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19992, 0 },  // Test Temperature 1
        { 19993, 1 },  // Test Temperature 2
        { 19994, 2 },  // Test results
        { 19995, 3 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 4; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 4;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.50f, 1.50f, 1.77f, 1.50f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.7\tThermal shock test for enameled cookware";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = { "Test Temperature 1", "Test Temperature 2", "Test results", "Decision" };
            table.Rows[2].Height = wordApp.InchesToPoints(0.40f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19996);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }


        /**************************************************Coating Thickness****************************************************************************/
        private void CreateGridTable_CoatingThickness()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TypeOfMaterial");
            dt.Columns.Add("CoatingMaterial");
            dt.Columns.Add("Measurement1");
            dt.Columns.Add("Measurement2");
            dt.Columns.Add("Measurement3");
            dt.Columns.Add("Average");
            dt.Columns.Add("MinLayerThickness");
            dt.Columns.Add("Decision");
            ViewState["TestData_CoatingThickness"] = dt;
            gvCoatingThickness.DataSource = dt;
            gvCoatingThickness.DataBind();
        }

        protected void btnAdd_CoatingThickness_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_CoatingThickness"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["TypeOfMaterial"] = txtCoating_TypeOfMaterial.Text.Trim();
            dr["CoatingMaterial"] = txtCoating_CoatingMaterial.Text.Trim();
            dr["Measurement1"] = txtCoating_Measurement1.Text.Trim();
            dr["Measurement2"] = txtCoating_Measurement2.Text.Trim();
            dr["Measurement3"] = txtCoating_Measurement3.Text.Trim();
            dr["Average"] = txtCoating_Average.Text.Trim();
            dr["MinLayerThickness"] = txtCoating_MinLayer.Text.Trim();
            dr["Decision"] = txtCoating_Decision.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_CoatingThickness"] = dt;
            gvCoatingThickness.DataSource = dt;
            gvCoatingThickness.DataBind();

            txtCoating_TypeOfMaterial.Text = "";
            txtCoating_CoatingMaterial.Text = "";
            txtCoating_Measurement1.Text = "";
            txtCoating_Measurement2.Text = "";
            txtCoating_Measurement3.Text = "";
            txtCoating_Average.Text = "";
            txtCoating_MinLayer.Text = "";
            txtCoating_Decision.Text = "";
        }

        protected void gvCoatingThickness_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_CoatingThickness"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_CoatingThickness"] = dt;
            gvCoatingThickness.DataSource = dt;
            gvCoatingThickness.DataBind();
        }

        private void PrefillCoatingThickness(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("TypeOfMaterial");
            gridDt.Columns.Add("CoatingMaterial");
            gridDt.Columns.Add("Measurement1");
            gridDt.Columns.Add("Measurement2");
            gridDt.Columns.Add("Measurement3");
            gridDt.Columns.Add("Average");
            gridDt.Columns.Add("MinLayerThickness");
            gridDt.Columns.Add("Decision");

            string tmCsv = GetResultValue(dt, "19997");
            string cmCsv = GetResultValue(dt, "19998");
            string m1Csv = GetResultValue(dt, "19999");
            string m2Csv = GetResultValue(dt, "20002");
            string m3Csv = GetResultValue(dt, "20003");
            string avgCsv = GetResultValue(dt, "20004");
            string mlCsv = GetResultValue(dt, "20005");
            string decCsv = GetResultValue(dt, "20006");
            txtCoatingAdditionalinfo.Text = GetResultValue(dt, "20007");

            if (!string.IsNullOrEmpty(tmCsv))
            {
                string[] tms = tmCsv.Split(',');
                string[] cms = cmCsv?.Split(',') ?? new string[0];
                string[] m1s = m1Csv?.Split(',') ?? new string[0];
                string[] m2s = m2Csv?.Split(',') ?? new string[0];
                string[] m3s = m3Csv?.Split(',') ?? new string[0];
                string[] avgs = avgCsv?.Split(',') ?? new string[0];
                string[] mls = mlCsv?.Split(',') ?? new string[0];
                string[] decs = decCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < tms.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["TypeOfMaterial"] = tms[i].Trim();
                    dr["CoatingMaterial"] = (i < cms.Length) ? cms[i].Trim() : "";
                    dr["Measurement1"] = (i < m1s.Length) ? m1s[i].Trim() : "";
                    dr["Measurement2"] = (i < m2s.Length) ? m2s[i].Trim() : "";
                    dr["Measurement3"] = (i < m3s.Length) ? m3s[i].Trim() : "";
                    dr["Average"] = (i < avgs.Length) ? avgs[i].Trim() : "";
                    dr["MinLayerThickness"] = (i < mls.Length) ? mls[i].Trim() : "";
                    dr["Decision"] = (i < decs.Length) ? decs[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData_CoatingThickness"] = gridDt;
            gvCoatingThickness.DataSource = gridDt;
            gvCoatingThickness.DataBind();
        }

        private DataTable GetReportDataForCoatingThickness()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19997, 19998, 19999, 20002, 20003, 20004, 20005, 20006, 20007)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }
        private void CreateReportTableCoatingThickness(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19997, 0 },  // Type of material
        { 19998, 1 },  // Coating Material
        { 19999, 2 },  // Measurement 1
        { 20002, 3 },  // Measurement 2
        { 20003, 4 },  // Measurement 3
        { 20004, 5 },  // Average
        { 20005, 6 },  // Min layer thickness
        { 20006, 7 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 8; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 8;

            // rows: 1 section header + 2 header rows + numRows data + 1 additional info
            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 0.70f, 0.70f, 0.65f, 0.65f, 0.65f, 0.65f, 0.77f, 0.70f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── STEP 1: Merge additional info row FIRST ───────────────────────
            int addRow = numRows + 4;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));

            // ── STEP 2: Merge section header row ─────────────────────────────
            table.Cell(1, 1).Merge(table.Cell(1, numCols));

            // ── ROW 1: Section header content ────────────────────────────────
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.8\tCoating Thickness Test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 2: Top header row (no merges) ────────────────────────────
            table.Rows[2].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            string[] headers2 = {
        "Type of\r\nmaterial",
        "Coating\r\nMaterial",
        "Results of measuring layer thickness (\u00b5)",
        "",
        "",
        "",
        "Min layer\r\nthickness (\u00b5)",
        "Decision"
    };
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers2[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── ROW 3: Sub-header row (no merges) ────────────────────────────
            table.Rows[3].Height = wordApp.InchesToPoints(0.40f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            string[] headers3 = {
        "",
        "",
        "Measurement\r\n1",
        "Measurement\r\n2",
        "Measurement\r\n3",
        "Average",
        "",
        ""
    };
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers3[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── DATA ROWS (start at row 4) ────────────────────────────────────
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length)
                        ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = (colIndex < 2)
                        ? WdParagraphAlignment.wdAlignParagraphLeft
                        : WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // ── ADDITIONAL INFO ROW content  ───────
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 20007);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        /*********************************************************************************************************************************/
    }
}