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
    public partial class PackagingMethodLampFittingSupervisionTest : System.Web.UI.Page
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

                txtRequestNo.Enabled = false;
                txtstandard.Enabled = false;
                txtType.Enabled = false;
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
                txtType.Text = sampleName;
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
                txthumidity.Text = saved.Rows[0]["Humidity"].ToString();
                txttemp.Text = saved.Rows[0]["Temperature"].ToString();

                txtRanking.Text = saved.Rows[0]["Ranking"].ToString();
                txtsampleCode.Text = saved.Rows[0]["SampleCode"].ToString();
                txtTestEquipmentNo.Text = saved.Rows[0]["TestEquipmentNumber"].ToString();

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestDate"].ToString()))
                {
                    DateTime testDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestDate"].ToString(), out testDate))
                    {
                        txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");
                    }
                }
            }

            DataTable dtCategoryTypeStdTest = _cm.GetCategoryTypeStdTest(lblSampleID.Text);
            if (dtCategoryTypeStdTest != null && dtCategoryTypeStdTest.Rows.Count > 0)
            {
                txtTestCategory.Text = dtCategoryTypeStdTest.Rows[0]["TestCategory"].ToString();
            }

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
                        {
                            item.Selected = true;
                        }
                    }
                }
            }

            int reportID = Convert.ToInt32(lblReportID.Text);
            string standardID = lblstandardID.Text;

            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_EA(sampleId, reportID, standardID);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                int departmentID = Convert.ToInt32(dt1.Rows[0]["DepartmentID"]);
                int productCategoryID = Convert.ToInt32(dt1.Rows[0]["ProductCategoryID"]);
                string StandardID = dt1.Rows[0]["StandardID"].ToString();
                string TestStandardGet = dts.Rows[0]["TestStandard"].ToString();
                txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"].ToString();
                lblReportName.Text = dt1.Rows[0]["ReportName"].ToString();
                string sis = dts.Rows[0]["sampleIDSeries"].ToString();
                string requestNo = "";

                if (!string.IsNullOrEmpty(sis) && sis.Contains("/"))
                {
                    string[] parts = sis.Split('/');
                    if (parts.Length > 1)
                    {
                        requestNo = "R/" + parts[1];
                    }
                }

                txtRequestNo.Text = requestNo;
                ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"].ToString();

                string[] ids = StandardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> standardIdList = ids.Select(id => id.Trim()).ToList();
                DataTable filteredClauseTable = null;

                if (standardIdList.Count > 0)
                {
                    var matchingRows = dt.AsEnumerable().Where(r =>
                    {
                        string clauseStandardIds = r["Standard_ID"].ToString();
                        string[] clauseIds = clauseStandardIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                        return clauseIds.Any(cid => standardIdList.Contains(cid));
                    });

                    if (matchingRows.Any())
                        filteredClauseTable = matchingRows.CopyToDataTable();
                }

                if (filteredClauseTable == null || filteredClauseTable.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No matching clauses found!');</script>");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                bool descriptionHeaderShown = false;

                sb.Append(@"<style>
                    .clause-table {
                        width: 100%;
                        border-collapse: collapse;
                        border: 1px solid #000;
                        font-family: Arial, sans-serif;
                        font-size: 12px;
                    }
                    .clause-table th, .clause-table td {
                        border: 1px solid #000;
                        padding: 6px 4px;
                        vertical-align: top;
                    }
                    .clause-table th {
                        background-color: #f0f0f0;
                        font-weight: bold;
                        text-align: center;
                    }
                    .clause-number {
                        width: 8%;
                        text-align: center;
                        font-weight: bold;
                    }
                    .requirement {
                        width: 52%;
                    }
                    .specification {
                        width: 25%;
                    }
                    .result {
                        width: 15%;
                    }
                    .header-row {
                        background-color: #e8e8e8;
                        font-weight: bold;
                    }
                    .sub-header-row {
                        background-color: #d0d0d0;
                    }
                    input[type='text'] {
                        width: 98%;
                        padding: 4px;
                        border: 1px solid #ccc;
                        font-size: 11px;
                    }
                    select {
                        width: 98%;
                        padding: 4px;
                        border: 1px solid #ccc;
                        font-size: 11px;
                    }
                    .checkbox-group {
                        display: flex;
                        flex-direction: column;
                        gap: 5px;
                    }
                    .checkbox-row {
                        display: flex;
                        gap: 15px;
                    }
                    .checkbox-item {
                        display: flex;
                        align-items: center;
                        white-space: nowrap;
                        font-size: 11px;
                    }
                    .checkbox-item input[type='checkbox'] {
                        width: 16px;
                        height: 16px;
                        margin-right: 5px;
                    }
                    .specification-text {
                        font-size: 11px;
                        padding: 2px;
                    }
                    .specification-bh {
                        font-weight: bold;
                        text-decoration: underline;
                    }
                    .specification-en {
                        font-style: italic;
                    }
                    </style>");

                sb.Append("<table class='clause-table'>");
                sb.Append("<tr class='sub-header-row'>");
                sb.Append("<th class='clause-number'>Klausul<br/>Clause</th>");
                sb.Append("<th class='requirement'>Persyaratan<br/>Requirements</th>");
                sb.Append("<th class='specification'>Hasil uji<br/>Test Result</th>");
                sb.Append("<th class='result'>Keputusan<br/>Decision</th>");
                sb.Append("</tr>");

                foreach (DataRow row in filteredClauseTable.Rows)
                {
                    bool isHeader = row["isHeader"].ToString() == "1";
                    bool isDescription = row["isDescription"].ToString() == "1";

                    if (isHeader)
                    {
                        descriptionHeaderShown = false;

                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        sb.Append("<tr class='header-row'>");
                        sb.Append("<td>");
                        sb.Append($"<b>{row["ClauseNumber"]}</b><br/>");
                        sb.Append("</td>");
                        sb.Append("<td colspan='2'>");
                        sb.Append($"<u>{row["ClauseTitle_BH"]}</u><br/>");
                        sb.Append($"<i>{row["ClauseTitle"]}</i>");
                        sb.Append("</td>");

                        // Result column for header row with textbox
                        sb.Append("<td class='result'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");

                        // Get saved result value
                        string savedResult = "";
                        var savedRowForResult = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForResult != null)
                            savedResult = savedRowForResult["Result"]?.ToString() ?? "";

                        sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' />");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    else if (isDescription)
                    {
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        // Show Description/Condition header only once per section
                        if (!descriptionHeaderShown)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td class='clause-number'></td>");
                            sb.Append("<td style='padding:0;'>");
                            sb.Append("<table style='width:100%; border-collapse: collapse; margin:0; border: 1px solid #000;'>");
                            sb.Append("<tr style='background-color:#f0f0f0; font-weight:bold;'>");
                            sb.Append("<td style='width:50%; padding:5px; border-right:1px solid #000; border-bottom:1px solid #000;'>Description</td>");
                            sb.Append("<td style='width:20%; padding:5px; border-bottom:1px solid #000; text-align:center;'>Condition<br/>(mm)</td>");
                            sb.Append("</tr>");
                            sb.Append("</table>");
                            sb.Append("</td>");
                            sb.Append("<td></td>");
                            sb.Append("<td></td>");
                            sb.Append("</tr>");

                            descriptionHeaderShown = true;
                        }

                        // Data row with Description and Condition in Requirements column
                        sb.Append("<tr>");
                        sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                        // Requirements column with nested table for Description and Condition
                        sb.Append("<td class='requirement' style='padding:0;'>");
                        sb.Append("<table style='width:100%; border-collapse: collapse; margin:0; border: 1px solid #000;'>");
                        sb.Append("<tr>");
                        sb.Append("<td style='width:50%; padding:5px; border-right:1px solid #000; vertical-align:top;'>");
                        sb.Append($"<u>{row["Description_BH"]}</u><br/>");
                        sb.Append($"<i>{row["Description"]}</i>");
                        sb.Append("</td>");
                        sb.Append("<td style='width:20%; padding:5px; text-align:center; vertical-align:middle;'>");
                        sb.Append($"{row["Condition"]}");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("</td>");

                        // Specification column - Show checkboxes if ResultOptions exists
                        sb.Append("<td class='specification'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");

                        string resultOptions = row["ResultOptions"]?.ToString();
                        bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

                        if (hasResultOptions)
                        {
                            // Generate checkboxes in Specification column
                            GenerateCheckboxesInSpecification(sb, row, saved, clauseID);
                        }
                        else
                        {
                            // Show textbox for specification if no ResultOptions
                            string specification = row["Specification"]?.ToString() ?? "";
                            string specification_BH = row["Specification_BH"]?.ToString() ?? "";

                            // Get saved specification value
                            string savedSpecification = ""; string savedSpecification_BH = "";
                            var savedRowForSpec = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                            if (savedRowForSpec != null)
                            {
                                savedSpecification = savedRowForSpec["Remark"]?.ToString() ?? "";
                                savedSpecification_BH = savedRowForSpec["Remark_BH"]?.ToString() ?? "";
                            }

                            // Use saved value if available, otherwise use default from database
                            string displaySpecification = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
                            string displaySpecification_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;

                            if (!string.IsNullOrEmpty(specification_BH) || !string.IsNullOrEmpty(displaySpecification))
                            {
                                if (!string.IsNullOrEmpty(specification_BH))
                                    sb.Append($"<input type='text' name='specification_BH{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification_BH)}' /><br/>");
                                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification)}' />");
                            }
                        }
                        sb.Append("</td>");

                        // Result column - Always textbox
                        sb.Append("<td class='result'>");
                        // Get saved result value
                        string savedResult = "";
                        var savedRowForResult = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForResult != null)
                            savedResult = savedRowForResult["Result"]?.ToString() ?? "";

                        sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='width:94%;' />");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    else
                    {
                        descriptionHeaderShown = false;

                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        sb.Append("<tr>");
                        sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");
                        sb.Append("<td class='requirement'>");
                        sb.Append($"<u>{row["Requirement_BH"]}</u><br/>");
                        sb.Append($"<i>{row["Requirement"]}</i>");
                        sb.Append("</td>");

                        // Specification column - Show checkboxes if ResultOptions exists
                        sb.Append("<td class='specification'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");

                        string resultOptions = row["ResultOptions"]?.ToString();
                        bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

                        if (hasResultOptions)
                        {
                            // Generate checkboxes in Specification column
                            GenerateCheckboxesInSpecification(sb, row, saved, clauseID);
                        }
                        else
                        {
                            // Show textbox for specification if no ResultOptions
                            string specification = row["Specification"]?.ToString() ?? "";
                            string specification_BH = row["Specification_BH"]?.ToString() ?? "";

                            // Get saved specification value
                            string savedSpecification = ""; string savedSpecification_BH = "";
                            var savedRowForSpec = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                            if (savedRowForSpec != null)
                            {
                                savedSpecification = savedRowForSpec["Specification"]?.ToString() ?? "";
                                savedSpecification_BH = savedRowForSpec["Specification_BH"]?.ToString() ?? "";
                            }

                            // Use saved value if available, otherwise use default from database
                            string displaySpecification = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
                            string displaySpecification_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;

                            if (!string.IsNullOrEmpty(specification_BH) || !string.IsNullOrEmpty(displaySpecification))
                            {
                                if (!string.IsNullOrEmpty(specification_BH))
                                    sb.Append($"<input type='text' name='specification_BH_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification_BH)}' /><br/>");
                                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification)}' />");
                            }
                        }
                        sb.Append("</td>");

                        // Result column - Always textbox
                        sb.Append("<td class='result'>");
                        // Get saved result value
                        string savedResult = "";
                        var savedRowForResult = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForResult != null)
                            savedResult = savedRowForResult["Result"]?.ToString() ?? "";

                        sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='width:94%;' />");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</table>");

                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
            }
        }

        private void GenerateCheckboxesInSpecification(StringBuilder sb, DataRow row, DataTable saved, int clauseID)
        {
            string resultOptions = row["ResultOptions"]?.ToString()?.Trim() ?? "";
            string[] options = resultOptions.Split(',');

            var savedRows = saved.Select($"ClauseID={clauseID}");
            List<string> savedValues = new List<string>();
            foreach (var savedRow in savedRows)
            {
                string savedResult = savedRow["Specification"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(savedResult))
                    savedValues.AddRange(savedResult.Split(',').Select(s => s.Trim()));
            }

            sb.Append("<div class='checkbox-group'>");

            for (int i = 0; i < options.Length; i++)
            {
                string optValue = options[i].Trim();
                string optDisplay = optValue;
                string isChecked = savedValues.Contains(optValue) ? "checked='checked'" : "";

                if (i % 2 == 0)
                {
                    sb.Append("<div class='checkbox-row'>");
                }

                sb.Append($"<label class='checkbox-item'><input type='checkbox' name='spec_chk_{clauseID}_{optValue}' value='{optValue}' {isChecked}/> {optDisplay}</label>");

                if ((i + 1) % 2 == 0 || i == options.Length - 1)
                {
                    sb.Append("</div>");
                }
            }
            sb.Append("</div>");
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
                    int sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
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

                        // Handle specification based on whether ResultOptions exists
                        string specification = "";
                        string specification_BH = "";
                        string resultOptions = row["ResultOptions"]?.ToString();
                        bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

                        if (hasResultOptions)
                        {
                            // Handle checkboxes in specification column
                            string[] options = row["ResultOptions"].ToString().Split(',');
                            List<string> selectedOptions = new List<string>();
                            foreach (var option in options)
                            {
                                string optionTrimmed = option.Trim();
                                if (Request.Form[$"spec_chk_{clauseID}_{optionTrimmed}"] != null)
                                    selectedOptions.Add(optionTrimmed);
                            }
                            specification = string.Join(",", selectedOptions);
                            specification_BH = ""; // No BH version for checkbox values
                        }
                        else
                        {
                            // Handle textbox specification
                            specification = Request.Form[$"specification_{clauseID}"] ?? "";
                            specification_BH = Request.Form[$"specification_BH{clauseID}"] ?? "";
                            if (string.IsNullOrEmpty(specification_BH))
                                specification_BH = Request.Form[$"specification_BH_{clauseID}"] ?? "";
                        }

                        if (isHeader)
                        {
                            string result = "";

                            // For header rows, result is always textbox (no checkboxes)
                            result = Request.Form[$"result_{clauseID}"] ?? "";

                            int result0 = _cm.TestResultsAdd_EA(clauseID, txtstartingdate.Text, txtstartingdate.Text, result, specification, specification_BH, sampleId, sampleIdSeries,
                                 "header", language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, lblReportName.Text, txtsampleCode.Text,
                                 txtTestEquipmentNo.Text, lblstandardID.Text, txtstandard.Text, txtstandard.Text, txttemp.Text, txthumidity.Text,
                                 txtTestCategory.Text, txtRanking.Text, txtsampleCode.Text, txtRequestNo.Text, txtTestEquipmentNo.Text);
                            if (result0 > 0)
                            {
                                b1 = true;
                            }
                        }
                        else
                        {
                            string result = "";

                            // For non-header rows, result is always textbox (no checkboxes)
                            result = Request.Form[$"result_{clauseID}"] ?? "";

                            int result0 = _cm.TestResultsAdd_EA(clauseID, txtstartingdate.Text, txtstartingdate.Text, result, specification, specification_BH, sampleId, sampleIdSeries,
                                "text", language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, lblReportName.Text, txtsampleCode.Text,
                                txtTestEquipmentNo.Text, lblstandardID.Text, txtstandard.Text, txtstandard.Text, txttemp.Text, txthumidity.Text,
                                txtTestCategory.Text, txtRanking.Text, txtsampleCode.Text, txtRequestNo.Text, txtTestEquipmentNo.Text);

                            if (result0 > 0)
                            {
                                b1 = true;
                            }

                        }
                    }

                    /* Record/Report Generate */
                    if (b1 == true)
                    {
                        //int sampleStatusID = 27;
                        //int res1 = _cm.UpdateCRFSampleDetailsStatus_LED(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
                        //if (res1 > 0)
                        //{
                        //    int res2 = _cm.UpdateCRFCloneStatus_LED(sampleStatusID, cloneCFRsampleID);
                        //    if (res2 > 0)
                        //    {
                        //        int res3 = _cm.updateSampleDetailStatus_LED(sampleStatusID, lblSampleID.Text);
                        //        if (res3 > 0)
                        //        {
                        /* Report Generate Code */
                        string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
                        string pathout = Server.MapPath("~/Reports/");
                        string templatePath = Server.MapPath("~/InputFiles/EATestRecord_2.docx");

                        if (!Directory.Exists(pathout))
                            Directory.CreateDirectory(pathout);

                        string outputDocxPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".docx");
                        string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");
                        /* Report Generate Code */

                        Application wordApp = null;
                        Document doc = null;

                        try
                        {
                            wordApp = new Application();
                            wordApp.Visible = false;

                            if (File.Exists(templatePath))
                            {
                                doc = wordApp.Documents.Open(templatePath);
                                ReplaceTemplateVariables(doc);
                            }
                            else
                            {
                                doc = wordApp.Documents.Add();
                                CreateBasicTemplate(doc);
                            }
                            DataTable dtReport = GetReportData();
                            CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                            doc.SaveAs2(outputDocxPath);
                            doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);
                            if (File.Exists(outputDocxPath) && File.Exists(outputPdfPath))
                            {
                                int sampleStatusID = 27;
                                int res1 = _cm.UpdateCRFSampleDetailsStatus_LED(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
                                if (res1 > 0)
                                {
                                    int res2 = _cm.UpdateCRFCloneStatus_LED(sampleStatusID, cloneCFRsampleID);
                                    if (res2 > 0)
                                    {
                                        int res3 = _cm.updateSampleDetailStatus_LED(sampleStatusID, lblSampleID.Text);
                                        if (res3 > 0)
                                        {
                                        }
                                    }
                                }
                            }
                            Response.Clear();
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("Content-Disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                            Response.TransmitFile(outputPdfPath);
                            Response.Flush();
                            Response.End();
                        }
                        catch (Exception ex)
                        {
                            string msg = ex.Message;
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                        }
                        finally
                        {
                            try
                            {
                                if (doc != null)
                                {
                                    doc.Close(false);
                                    Marshal.ReleaseComObject(doc);
                                    doc = null;
                                }
                            }
                            catch { }

                            try
                            {
                                if (wordApp != null)
                                {
                                    wordApp.Quit();
                                    Marshal.ReleaseComObject(wordApp);
                                    wordApp = null;
                                }
                            }
                            catch { }
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            /*Force - kill any lingering WINWORD processes started by this user session for locally server */
                            foreach (var process in Process.GetProcessesByName("WINWORD"))
                            {
                                if (string.IsNullOrEmpty(process.MainWindowTitle))
                                {
                                    process.Kill();
                                }
                            }
                        }

                        //        }
                        //    }
                        //}

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
            DataTable DisResget = _cm.GetDistinctTestResult_EA(lblSampleID.Text, lblReportID.Text);

            DataTable docNumberGet = _cm.GetDocumentsNumber(lblReportID.Text);
            string DocNo = docNumberGet.Rows[0]["DocNo"].ToString();

            if (DisResget.Rows.Count > 0)
            {
                string equipmentIds = DisResget.Rows[0]["EquipmentIds"].ToString();
                List<string> equipmentNames = GetEquipmentNamesAsList(equipmentIds);
                DataRow row = DisResget.Rows[0];

                var replacements = new Dictionary<string, string>
                {
                    {"{WorkorderNumber}", GetSafeString(row, "WorkOrderNo") },
                    {"{Product}", ViewState["sampleName"].ToString()},
                    {"{TestStartDate}", FormatDate(GetSafeString(row, "TestDate"))},
                    {"{TestCompletiondate}", FormatDate(GetSafeString(row, "TestCompletationDate"))},
                    {"{Temperature}", GetSafeString(row, "Temperature")},
                    {"{Humidity}", GetSafeString(row, "Humidity") },
                    {"{Method}", GetSafeString(row, "TestMethod")},
                    {"{Standard}", GetSafeString(row, "StandardName")},
                    {"{TestCategory}", GetSafeString(row, "TestCategory")},
                    {"{RequestNo}", GetSafeString(row, "RequestNumber") },
                    {"{ReportNameDisplay}", GetSafeString(row, "ProductName") },
                    {"{Ranking}", GetSafeString(row, "Ranking") },
                    {"{SampleCode}", GetSafeString(row, "SampleCode") },
                    {"{EquipmentNo}", GetSafeString(row, "TestEquipmentNumber") },

                    {"{DocNo}",  DocNo },
                    {"{Revision}", "03" },
                    {"{EffectiveDate}", "06/01/2020"}
                };
                // Replace in main document body
                ReplaceInRange(doc.Content, replacements);
                // Replace in all headers
                ReplaceInAllHeaders(doc, replacements);

                // Replace equipment placeholder separately when it need
                // ReplaceEquipmentPlaceholder(doc, equipmentNames);
            }
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

            // Add required columns
            dt.Columns.Add("ClauseID", typeof(int));
            dt.Columns.Add("ClauseNumber", typeof(string));
            dt.Columns.Add("ClauseTitle", typeof(string));
            dt.Columns.Add("Requirement", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Condition", typeof(string));
            dt.Columns.Add("Specification", typeof(string));
            dt.Columns.Add("Result", typeof(string));
            dt.Columns.Add("isHeader", typeof(string));
            dt.Columns.Add("isDescription", typeof(string));
            dt.Columns.Add("ResultOptions", typeof(string)); // NEW: Add ResultOptions column
            dt.Columns.Add("hasCheckboxes", typeof(bool));    // NEW: Flag for checkbox rows

            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);
            string standardID = lblstandardID.Text;

            // Get clause data
            DataTable clauseData = GetClauseData();

            // Get saved results
            DataTable savedResults = GetSavedResults();

            // Filter clauses by standard
            string[] ids = standardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> standardIdList = ids.Select(id => id.Trim()).ToList();

            if (standardIdList.Count > 0)
            {
                var matchingRows = clauseData.AsEnumerable().Where(r =>
                {
                    string clauseStandardIds = r["Standard_ID"].ToString();
                    string[] clauseIds = clauseStandardIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
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
                        newRow["ResultOptions"] = clauseRow["ResultOptions"]?.ToString() ?? ""; // NEW: Store ResultOptions

                        // Determine if this row has checkboxes
                        string resultOptions = clauseRow["ResultOptions"]?.ToString();
                        bool hasCheckboxes = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";
                        newRow["hasCheckboxes"] = hasCheckboxes;

                        // Get saved data
                        var savedRow = savedResults.AsEnumerable()
                            .FirstOrDefault(r => Convert.ToInt32(r["ClauseID"]) == clauseID);

                        if (savedRow != null)
                        {
                            // For isDescription rows, Specification comes from Remark
                            if (clauseRow["isDescription"].ToString() == "1")
                            {
                                newRow["Specification"] = savedRow["Remark"]?.ToString() ?? "";
                            }
                            else
                            {
                                newRow["Specification"] = savedRow["Specification"]?.ToString() ??
                                                         clauseRow["Specification"]?.ToString() ?? "";
                            }

                            newRow["Result"] = savedRow["Result"]?.ToString() ?? "";
                        }
                        else
                        {
                            newRow["Specification"] = clauseRow["Specification"]?.ToString() ?? "";
                            newRow["Result"] = "";
                        }

                        dt.Rows.Add(newRow);
                    }
                }
            }

            return dt;
        }

        private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            // Font configuration constants for easy maintenance
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

            // Column width configuration (in inches) for easy maintenance
            float[] COLUMN_WIDTHS = { 0.6f, 3.73f, 1.14f, 0.80f };
            float SUB_COLUMN1_WIDTH = 2.1f; // 60% of 3.5f
            float SUB_COLUMN2_WIDTH = 1.4f; // 40% of 3.5f
            float TABLE_TOTAL_WIDTH = 6.27f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // STEP 1: Count rows - adjust for checkbox rows
            int totalRows = 1; // header row
            bool needDescHeader = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool hasCheckboxes = Convert.ToBoolean(row["hasCheckboxes"]);

                if (isHeader)
                {
                    totalRows++;
                    needDescHeader = false;
                }
                else if (isDescription)
                {
                    if (!needDescHeader)
                    {
                        totalRows += 1; // Only sub-column header row (removed the extra row)
                        needDescHeader = true;
                    }
                    totalRows++; // actual data row
                }
                else
                {
                    totalRows++; // Single row for all cases (checkboxes will be in same cell)
                    needDescHeader = false;
                }
            }

            // STEP 2: Build table with 4 main columns
            Table table = doc.Tables.Add(range, totalRows, 4);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);

            // Apply default font to entire table
            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            // Adjust column widths
            for (int c = 1; c <= 4; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            // MAIN HEADER
            string[] headers = { "Clause", "Test requirements", "Test Results", "Decision" };
            for (int col = 1; col <= 4; col++)
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
                bool hasCheckboxes = Convert.ToBoolean(row["hasCheckboxes"]);
                string specification = row["Specification"].ToString();
                string result = row["Result"].ToString();
                string resultOptions = row["ResultOptions"].ToString();

                if (isHeader)
                {
                    descHeaderShown = false;

                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, result, true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
                else if (isDescription)
                {
                    if (!descHeaderShown)
                    {
                        Cell d1 = table.Cell(r, 1);
                        SetCellText(d1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        Cell d2 = table.Cell(r, 2);
                        Table subTableHeader = d2.Range.Tables.Add(d2.Range, 1, 2);
                        subTableHeader.Borders.Enable = 0;
                        subTableHeader.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
                        subTableHeader.Range.Font.Size = DEFAULT_FONT_SIZE;
                        subTableHeader.Range.Font.Name = DEFAULT_FONT_NAME;

                        // Set sub-column widths
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

                        Cell d3 = table.Cell(r, 3);
                        SetCellText(d3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        Cell d4 = table.Cell(r, 4);
                        SetCellText(d4, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        r++;
                        descHeaderShown = true;
                    }

                    // Data row with Description and Condition in sub-columns
                    Cell a1 = table.Cell(r, 1);
                    SetCellText(a1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell a2 = table.Cell(r, 2);
                    // Create sub-table for Description and Condition data
                    Table subTableData = a2.Range.Tables.Add(a2.Range, 1, 2);
                    subTableData.Borders.Enable = 0;
                    subTableData.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
                    subTableData.Range.Font.Size = DEFAULT_FONT_SIZE;
                    subTableData.Range.Font.Name = DEFAULT_FONT_NAME;

                    // Set sub-column widths (same as headers)
                    subTableData.Columns[1].SetWidth(wordApp.InchesToPoints(SUB_COLUMN1_WIDTH), WdRulerStyle.wdAdjustNone);
                    subTableData.Columns[2].SetWidth(wordApp.InchesToPoints(SUB_COLUMN2_WIDTH), WdRulerStyle.wdAdjustNone);

                    Cell subData1 = subTableData.Cell(1, 1);
                    SetCellText(subData1, row["Description"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    subData1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell subData2 = subTableData.Cell(1, 2);
                    SetCellText(subData2, row["Condition"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    subData2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Apply only vertical border between sub-columns
                    ApplySubTableVerticalBorders(subTableData);
                    ApplyBorders(a2);

                    Cell a3 = table.Cell(r, 3);
                    // For description rows with checkboxes
                    if (hasCheckboxes && !string.IsNullOrEmpty(resultOptions))
                    {
                        string checkboxText = FormatCheckboxesInPairs(resultOptions, specification);
                        SetCellText(a3, checkboxText, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    }
                    else
                    {
                        SetCellText(a3, specification, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    }
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, result, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
                else
                {
                    descHeaderShown = false;

                    // Regular row
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c3 = table.Cell(r, 3);
                    // Handle checkbox formatting
                    if (hasCheckboxes && !string.IsNullOrEmpty(resultOptions))
                    {
                        string checkboxText = FormatCheckboxesInPairs(resultOptions, specification);
                        SetCellText(c3, checkboxText, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    }
                    else
                    {
                        SetCellText(c3, specification, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    }

                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, result, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
            }
        }

        private string FormatCheckboxesInPairs(string resultOptions, string specification)
        {
            if (string.IsNullOrEmpty(resultOptions))
                return string.Empty;

            // Get all options and selected values
            string[] allOptions = resultOptions.Split(',');
            string[] selectedValues = !string.IsNullOrEmpty(specification) ?
                specification.Split(',') : new string[0];
            List<string> selectedList = selectedValues.Select(v => v.Trim()).ToList();

            StringBuilder checkboxText = new StringBuilder();
            List<string> checkboxLines = new List<string>();

            // Process options in pairs (2 per row)
            for (int i = 0; i < allOptions.Length; i += 2)
            {
                string line = "";

                // First checkbox in the pair
                string option1 = allOptions[i].Trim();
                bool isChecked1 = selectedList.Contains(option1);
                string checkboxSymbol1 = isChecked1 ? "☒" : "☐";
                line += $"{checkboxSymbol1} {option1}";

                // Add space between pairs
                if (option1.Length < 8)
                    line += new string(' ', 8 - option1.Length);
                else
                    line += "    ";

                // Second checkbox in the pair (if exists)
                if (i + 1 < allOptions.Length)
                {
                    string option2 = allOptions[i + 1].Trim();
                    bool isChecked2 = selectedList.Contains(option2);
                    string checkboxSymbol2 = isChecked2 ? "☒" : "☐";
                    line += $"{checkboxSymbol2} {option2}";
                }

                checkboxLines.Add(line);
            }

            // Join all lines
            return string.Join(Environment.NewLine, checkboxLines);
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

        private void SetCellText(Cell cell, string text, bool bold, float fontSize, string fontName)
        {
            cell.Range.Text = text;
            cell.Range.Bold = bold ? 1 : 0;
            cell.Range.Font.Size = fontSize;
            cell.Range.Font.Name = fontName;
            ApplyBorders(cell);
        }


    }
}