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
    public partial class StainlessSteelFlatWear : System.Web.UI.Page
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

                CreateGridTable();
                CreateGridTable_cookingequ();
                DataTable existingData = _cm.GetExistingTestResults_Flatware_RP(lblSampleID.Text, lblReportID.Text);

                if (existingData != null && existingData.Rows.Count > 0)
                {
                    PrefillTestResultsGrid(existingData);
                    PrefillAllData(existingData);
                    PrefillTestResultsGrid_cookingequ(existingData);
                    PrefillAllData_cookingequ(existingData);

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
            DataTable snm = _cm.GetSampleName_EA(cloneCRFSampleID);
            if (snm != null && snm.Rows.Count > 0)
            {
                sampleName = snm.Rows[0]["SampleName"].ToString();
                ViewState["sampleName"] = sampleName;
                txtmodel.Text = sampleName;
            }

            DataTable dts = _cm.GetCRFSampleDetails_LED(sampleId, reportId, cloneCRFSampleID);
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
                txtusageofgroups.Text = saved.Rows[0]["Usagegroups"]?.ToString() ?? "";
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

            DataTable DisResget = _cm.GetTestResultLED_DISTINCT(lblSampleID.Text, lblReportID.Text);
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
            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_Flatware(sampleId, reportID, standardID);

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

                        // Col 4: Result
                        sb.Append("<td class='result'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
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
                        sb.Append("<td>");
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

                        // Col 2: Requirement (Test Parameters)
                        sb.Append("<td class='requirement'>");
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
                    {
                        sb.Append("<div class='checkbox-row'>");
                    }

                    sb.Append($"<label class='checkbox-item'><input type='checkbox' name='chk_{clauseID}_{optValue}' value='{optValue}' {isChecked}/> {optDisplayBH}</label>");

                    if ((i + 1) % 3 == 0 || i == options.Length - 1)
                    {
                        sb.Append("</div>");
                    }
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

                        int result0 = _cm.TestResultsAdd_Flatware(
                            clauseID,
                            txtstartingdate.Text,
                            txtstartingdate.Text,
                            result,
                            specification,
                            specification_BH,
                            sampleId,
                            sampleIdSeries,
                            isHeader ? "header" : inputType,   // ✅ FIXED
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
                            txtusageofgroups.Text,
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
                                    /* -------- DATA PREP -------- */

                                    DataTable dtRepoParam = ViewState["TestData"] as DataTable;

                                    if (dtRepoParam == null || dtRepoParam.Rows.Count == 0)
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No test data to save!');", true);
                                        return;
                                    }

                                    List<string> reportParamMapList = new List<string>();
                                    List<string> resultList = new List<string>();

                                    foreach (DataRow row in dtRepoParam.Rows)
                                    {
                                        reportParamMapList.Add("17954"); resultList.Add(row["Group"].ToString());
                                        reportParamMapList.Add("17957"); resultList.Add(row["TypeOfProducts"].ToString());
                                        reportParamMapList.Add("17955"); resultList.Add(row["MeasurementResults"].ToString());
                                        reportParamMapList.Add("17956"); resultList.Add(row["MinThicknessRequirement"].ToString());
                                        reportParamMapList.Add("17958"); resultList.Add(row["Decision"].ToString());
                                    }

                                    reportParamMapList.Add("17960");
                                    resultList.Add(txtAdditionalinfo.Text.Trim());

                                    string[] reportParamMap = reportParamMapList.ToArray();
                                    string[] resultNew = resultList.ToArray();

                                    int modified = _cm.TestResultsAdd_Flateware_RP(
                                        0,
                                        txtstartingdate.Text,
                                        txtcompletiondate.Text,
                                        "",
                                        sampleId,
                                        sampleIdSeries,
                                        "",
                                        language,
                                        ReportID,
                                        equipmentIdsCSV,
                                        txtworkorder.Text,
                                        txtTypeOfProducts_header.Text,
                                        txtBrand.Text,
                                        txtmodel.Text,
                                        lblstandardID.Text,
                                        txtstandard.Text,
                                        txttestmethod.Text,
                                        txttemp.Text,
                                        txthumidity.Text,
                                        reportParamMap,
                                        resultNew
                                    );

                                    /* -------- FILE GENERATION -------- */

                                    string SampleUID = lblSampleID.Text + "_" + lblReportID.Text;
                                    string pathout = Server.MapPath("~/TestReport/");
                                    string templatePath = Server.MapPath("~/InputFiles/StainlessSteelFlatWearTestRecord_1.docx");

                                    if (!Directory.Exists(pathout))
                                        Directory.CreateDirectory(pathout);

                                    string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

                                    Application wordApp = new Application();
                                    Document doc = wordApp.Documents.Open(templatePath);

                                    ReplaceTemplateVariables(doc);

                                    CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", GetReportData());
                                    CreateReportTableT2(wordApp, doc, "{Tableware}", GetReportDataForT2());
                                    CreateReportTableT3(wordApp, doc, "{Tablecookingequipment}", GetReportDataForT3());

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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }
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
          AND ReportParamMapID IN (17830, 17831, 17832, 17833, 17834, 17835)
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
            string useageofgroup = txtusageofgroups.Text;
            string TypeOfProduct_header = txtTypeOfProducts_header.Text;
            string testingdate = "";
            string serialno = txtSerialNo.Text;
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 
            TestMethod, Temperature, Humidity, TestDate, 
            TestCompletationDate, EquipmentIds, WorkOrderNo,serialNO,
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
                        useageofgroup = reader["Usagegroups"]?.ToString() ?? "";
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
            ReplaceInDoc(doc, "{usagegroup}", useageofgroup);      // Suhu (ºC)
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

        // ── HELPER: Find and replace in Word doc ─────────────────────────────────
        private void ReplaceInDoc(Document doc, string findText, string replaceText)
        {
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

            // Columns matching 5-column header exactly
            dt.Columns.Add("ClauseID", typeof(int));
            dt.Columns.Add("ClauseNumber", typeof(string));  // Klausul / Clause
            dt.Columns.Add("ClauseTitle", typeof(string));  // Parameter Uji / Test Parameters
            dt.Columns.Add("Requirement", typeof(string));  // Parameter Uji / Test Parameters
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Condition", typeof(string));
            dt.Columns.Add("Specification", typeof(string));  // Persyaratan Mutu / Quality Requirements
            dt.Columns.Add("Unit", typeof(string));  // Satuan / Unit
            dt.Columns.Add("Result", typeof(string));  // Hasil Tes / Test Results
            dt.Columns.Add("isHeader", typeof(string));
            dt.Columns.Add("isDescription", typeof(string));

            string standardID = lblstandardID.Text;

            DataTable clauseData = GetClauseData();
            DataTable savedResults = GetSavedResults();

            // Check if Unit column exists in saved results (safety check)
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

                        // Get saved data for this clause
                        var savedRow = savedResults?.AsEnumerable()
                            .FirstOrDefault(r => Convert.ToInt32(r["ClauseID"]) == clauseID);

                        if (savedRow != null)
                        {
                            // isDescription rows → Specification comes from Remark
                            // normal rows       → Specification comes from Specification
                            if (clauseRow["isDescription"].ToString() == "1")
                                newRow["Specification"] = savedRow["Remark"]?.ToString() ?? "";
                            else
                                newRow["Specification"] = !string.IsNullOrEmpty(savedRow["Specification"]?.ToString())
                                    ? savedRow["Specification"].ToString()
                                    : clauseRow["Specification"]?.ToString() ?? "";

                            // Unit — safe check in case column doesn't exist yet in DB
                            newRow["Unit"] = hasUnitColumn ? (savedRow["UnitName"]?.ToString() ?? "") : "";
                            newRow["Result"] = savedRow["Result"]?.ToString() ?? "";
                        }
                        else
                        {
                            // No saved data — use defaults from clause table
                            newRow["Specification"] = clauseRow["Specification"]?.ToString() ?? "";
                            newRow["Unit"] = "";
                            newRow["Result"] = "";
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
        private DataTable GetReportDataForT2()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (17954, 17955, 17956, 17957, 17958, 17960)";
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
        private DataTable GetReportDataForT3()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (18961, 18962, 18963, 18964, 18965, 18966)";
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

            // Column order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
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

            // STEP 1: Count rows
            int totalRows = 1; // header row
            bool needDescHeader = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";

                if (isHeader)
                {
                    totalRows++;
                    needDescHeader = false;
                }
                else if (isDescription)
                {
                    if (!needDescHeader)
                    {
                        totalRows += 1; // sub-column header row
                        needDescHeader = true;
                    }
                    totalRows++; // actual data row
                }
                else
                {
                    totalRows++;
                    needDescHeader = false;
                }
            }

            // STEP 2: Build table with 5 main columns
            Table table = doc.Tables.Add(range, totalRows, 5);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);

            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            // Set column widths
            for (int c = 1; c <= 5; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            // MAIN HEADER ROW
            // Col order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
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

                // ── HEADER ROW ───────────────────────────────────────────────────────
                if (isHeader)
                {
                    descHeaderShown = false;

                    // Col 1: Clause Number
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Clause Title (Test Parameters)
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Col 3: Unit - empty for header
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Quality Requirements - empty for header
                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
                // ── DESCRIPTION ROW ──────────────────────────────────────────────────
                else if (isDescription)
                {
                    // Sub-header row (shown once per description section)
                    if (!descHeaderShown)
                    {
                        Cell d1 = table.Cell(r, 1);
                        SetCellText(d1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 2: Sub-table header for Description | Condition
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

                        // Col 3: Unit header - empty
                        Cell d3 = table.Cell(r, 3);
                        SetCellText(d3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 4: Result header - empty
                        Cell d4 = table.Cell(r, 4);
                        SetCellText(d4, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 5: Quality Requirements header - empty
                        Cell d5 = table.Cell(r, 5);
                        SetCellText(d5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        r++;
                        descHeaderShown = true;
                    }

                    // Description data row
                    Cell a1 = table.Cell(r, 1);
                    SetCellText(a1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Sub-table data for Description | Condition
                    Cell a2 = table.Cell(r, 2);
                    Table subTableData = a2.Range.Tables.Add(a2.Range, 1, 2);
                    subTableData.Borders.Enable = 0;
                    subTableData.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
                    subTableData.Range.Font.Size = DEFAULT_FONT_SIZE;
                    subTableData.Range.Font.Name = DEFAULT_FONT_NAME;
                    subTableData.Columns[1].SetWidth(wordApp.InchesToPoints(SUB_COLUMN1_WIDTH), WdRulerStyle.wdAdjustNone);
                    subTableData.Columns[2].SetWidth(wordApp.InchesToPoints(SUB_COLUMN2_WIDTH), WdRulerStyle.wdAdjustNone);

                    Cell subData1 = subTableData.Cell(1, 1);
                    SetCellText(subData1, row["Description"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    subData1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell subData2 = subTableData.Cell(1, 2);
                    SetCellText(subData2, row["Condition"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    subData2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    ApplySubTableVerticalBorders(subTableData);
                    ApplyBorders(a2);

                    // Col 3: Unit (Satuan)
                    Cell a3 = table.Cell(r, 3);
                    SetCellText(a3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Specification (Quality Requirements)
                    Cell a5 = table.Cell(r, 5);
                    SetCellText(a5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
                // ── NORMAL ROW ───────────────────────────────────────────────────────
                else
                {
                    descHeaderShown = false;

                    // Col 1: Clause Number
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Requirement (Parameter Uji)
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Col 3: Unit (Satuan)
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Specification (Quality Requirements)
                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
            }
        }
        /* T2 Start */
        private void CreateReportTableT2(Application wordApp, Document doc, string placeholder, DataTable dt)
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
        { 17954, 0 },  // Group
        { 17957, 1 },  // Type Of Products
        { 17955, 2 },  // Measurement results (mm)
        { 17956, 3 },  // Min thickness requirement (mm)
        { 17958, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++)
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
            int numCols = 5;

            // +3 = 2 title rows + 1 column header row
            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            float[] colWidths = { 0.80f, 1.20f, 1.20f, 1.27f, 0.80f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── ROW 1: "Table: Product Thickness Measurement" ─────────────────────

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

            // ── ROW 2: "Tableware" ────────────────────────────────────────────────
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

            // ── ROW 3: Column Headers ─────────────────────────────────────────────
            string[] headers = {
        "Group",
        "Type Of Products",
        "Measurement results\r\n(mm)",
        "Min thickness requirement\r\n(mm)",
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

            int additionalInfoRow = numRows + 4;
            table.Rows[additionalInfoRow].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[additionalInfoRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            // Merge all columns
            table.Cell(additionalInfoRow, 1).Merge(table.Cell(additionalInfoRow, numCols));
            Cell additionalInfoCell = table.Cell(additionalInfoRow, 1);

            // Get value from dt where ReportParamMapID = 17960
            string additionalInfoText = "Additional information: ";
            DataRow additionalRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 17960);
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

        private void CreateGridTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Group");
            dt.Columns.Add("TypeOfProducts");
            dt.Columns.Add("MeasurementResults");
            dt.Columns.Add("MinThicknessRequirement");
            dt.Columns.Add("Decision");
            // dt.Columns.Add("ConformityMark");
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["Group"] = txtGroup.Text.Trim();
            dr["TypeOfProducts"] = txtTypeOfProducts.Text.Trim();
            dr["MeasurementResults"] = txtMeasurementResults.Text.Trim();
            dr["MinThicknessRequirement"] = txtMinThicknessRequirement.Text.Trim();
            dr["Decision"] = txtDecision.Text.Trim();
            //dr["ConformityMark"] = txtConformityMark.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();

            // Clear TextBoxes after adding
            txtGroup.Text = "";
            txtTypeOfProducts.Text = "";
            txtMeasurementResults.Text = "";
            txtMinThicknessRequirement.Text = "";
            txtDecision.Text = "";
            //txtConformityMark.Text = "";
        }

        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
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
            gridDt.Columns.Add("Group");
            gridDt.Columns.Add("TypeOfProducts");
            gridDt.Columns.Add("MeasurementResults");
            gridDt.Columns.Add("MinThicknessRequirement");
            gridDt.Columns.Add("Decision");

            //
            string groupCsv = GetResultValue(dt, "17954");
            string productTypeCsv = GetResultValue(dt, "17957");
            string measurementsCsv = GetResultValue(dt, "17955");
            string minThicknessCsv = GetResultValue(dt, "17956");
            string decisionCsv = GetResultValue(dt, "17958");
            // 17835 = Standard/SNI — add a column if needed

            if (!string.IsNullOrEmpty(groupCsv))
            {
                string[] groups = groupCsv.Split(',');
                string[] productTypes = productTypeCsv?.Split(',') ?? new string[0];
                string[] measurements = measurementsCsv?.Split(',') ?? new string[0];
                string[] minThicknesses = minThicknessCsv?.Split(',') ?? new string[0];
                string[] decisions = decisionCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < groups.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["Group"] = groups[i].Trim();
                    dr["TypeOfProducts"] = (i < productTypes.Length) ? productTypes[i].Trim() : "";
                    dr["MeasurementResults"] = (i < measurements.Length) ? measurements[i].Trim() : "";
                    dr["MinThicknessRequirement"] = (i < minThicknesses.Length) ? minThicknesses[i].Trim() : "";
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
            PrefillSingleValues(dt);
            // PrefillTwoValues(dt);
            // PrefillSixValues(dt);
        }
        private void PrefillSingleValues(DataTable dt)
        {
            Dictionary<string, TextBox> singleValueMap = new Dictionary<string, TextBox>
            {
                { "17954", txtGroup },
                { "17955", txtMeasurementResults },
                { "17956", txtMinThicknessRequirement},
                { "17958", txtDecision },
                { "17957", txtTypeOfProducts},
                { "17960", txtAdditionalinfo}

            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (singleValueMap.ContainsKey(paramId))
                {
                    singleValueMap[paramId].Text = rawValue;
                }
            }
        }


        /********************************Table 2*************************************/
        private void CreateGridTable_cookingequ()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Group_cookingequ");
            dt.Columns.Add("TypeOfProducts_cookingequ");
            dt.Columns.Add("MeasurementResults_cookingequ");
            dt.Columns.Add("MinThicknessRequirement_cookingequ");
            dt.Columns.Add("Decision_cookingequ");
            // dt.Columns.Add("ConformityMark");
            ViewState["TestData_cookingequ"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        protected void btnAddcookingequ_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData_cookingequ"] as DataTable;
            DataRow dr = dt.NewRow();
            dr["Group_cookingequ"] = txtGroupcookingequi.Text.Trim();
            dr["TypeOfProducts_cookingequ"] = txtTypeOfProductscookingequi.Text.Trim();
            dr["MeasurementResults_cookingequ"] = txtMeasurementResultscookingequi.Text.Trim();
            dr["MinThicknessRequirement_cookingequ"] = txtMinThicknessRequirementcookingequi.Text.Trim();
            dr["Decision_cookingequ"] = txtDecisioncookingequi.Text.Trim();
            //dr["ConformityMark"] = txtConformityMark.Text.Trim();
            dt.Rows.Add(dr);
            ViewState["TestData_cookingequ"] = dt;
            gvcooking.DataSource = dt;
            gvcooking.DataBind();

            // Clear TextBoxes after adding
            txtGroupcookingequi.Text = "";
            txtTypeOfProductscookingequi.Text = "";
            txtMeasurementResultscookingequi.Text = "";
            txtMinThicknessRequirementcookingequi.Text = "";
            txtDecisioncookingequi.Text = "";
            //txtConformityMark.Text = "";
        }
        protected void gvcooking_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData_cookingequ"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData_cookingequ"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }
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
            gvcooking.DataSource = gridDt;
            gvcooking.DataBind();
        }
        private void PrefillAllData_cookingequ(DataTable dt)
        {
            PrefillSingleValues_cookingequ(dt);
            // PrefillTwoValues(dt);
            // PrefillSixValues(dt);
        }
        private void PrefillSingleValues_cookingequ(DataTable dt)
        {
            Dictionary<string, TextBox> singleValueMap = new Dictionary<string, TextBox>
            {
                { "18964", txtGroupcookingequi },
                { "18965", txtMeasurementResultscookingequi },
                { "18966", txtMinThicknessRequirementcookingequi},
                { "18962", txtDecisioncookingequi },
                { "18961", txtTypeOfProductscookingequi},
                { "18963", txtAdditionalinfocookingequi}

            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (singleValueMap.ContainsKey(paramId))
                {
                    singleValueMap[paramId].Text = rawValue;
                }
            }
        }
        private string GetResultValue(DataTable dt, string parameterId)
        {
            DataRow[] rows = dt.Select($"ParameterID = '{parameterId}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
        }
        private void CreateReportTableT3(Application wordApp, Document doc, string placeholder, DataTable dt)
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
        { 18964, 0 },  // Group
        { 18961, 1 },  // Type Of Products
        { 18965, 2 },  // Measurement results (mm)
        { 18966, 3 },  // Min thickness requirement (mm)
        { 18962, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++)
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
            int numCols = 5;

            // +3 = 2 title rows + 1 column header row
            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            float[] colWidths = { 0.80f, 1.20f, 1.20f, 1.27f, 0.80f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── ROW 1: "Table: Product Thickness Measurement" ─────────────────────

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

            // ── ROW 2: "Tableware" ────────────────────────────────────────────────
            Cell mergedRow2 = table.Cell(2, 1);
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            mergedRow2.Range.Text = "8.2.2\tCooking equipment";
            mergedRow2.Range.Bold = 1;
            mergedRow2.Range.Font.Size = 10;
            mergedRow2.Range.Font.Name = "Calibri";
            mergedRow2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow2.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow2.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 3: Column Headers ─────────────────────────────────────────────
            string[] headers = {
        "Group",
        "Type Of Products",
        "Measurement results\r\n(mm)",
        "Min thickness requirement\r\n(mm)",
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

            int additionalInfoRow = numRows + 4;
            table.Rows[additionalInfoRow].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[additionalInfoRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            // Merge all columns
            table.Cell(additionalInfoRow, 1).Merge(table.Cell(additionalInfoRow, numCols));
            Cell additionalInfoCell = table.Cell(additionalInfoRow, 1);

            // Get value from dt where ReportParamMapID = 17960
            string additionalInfoText = "Additional information: ";
            DataRow additionalRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 17960);
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
    }
}