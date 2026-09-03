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
    public partial class TemperatureRise : System.Web.UI.Page
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

                //txtTestCategory.Text = saved.Rows[0]["TestCategory"].ToString();
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
                            </style>");

                sb.Append("<table class='clause-table'>");
                sb.Append("<tr class='sub-header-row'>");
                sb.Append("<th class='clause-number'>Klausul<br/>Clause</th>");
                sb.Append("<th class='requirement'>Persyaratan<br/>Requirements</th>");
                sb.Append("<th class='specification'>Hasil uji<br/>Test Result</th>");
                sb.Append("<th class='result'>Keputusan<br/>Decision</th>");
                sb.Append("</tr>");

                // We'll track if we're in a check-up result group
                bool inCheckUpResultGroup = false;
                List<DataRow> checkUpResultRows = new List<DataRow>();

                foreach (DataRow row in filteredClauseTable.Rows)
                {
                    bool isHeader = row["isHeader"].ToString() == "1";
                    bool isDescription = row["isDescription"].ToString() == "1";
                    bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";

                    // If we encounter a header or non-checkup row while in a group, process the group first
                    if ((isHeader || !isCheckUpResult) && inCheckUpResultGroup && checkUpResultRows.Count > 0)
                    {
                        ProcessCheckUpResultGroup(sb, checkUpResultRows, saved);
                        checkUpResultRows.Clear();
                        inCheckUpResultGroup = false;
                    }

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

                        sb.Append("<td class='result'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        string savedResult = "";
                        var savedRowForResult = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForResult != null)
                            savedResult = savedRowForResult["Result"]?.ToString() ?? "";
                        sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' />");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    else if (isCheckUpResult)
                    {
                        checkUpResultRows.Add(row);
                        inCheckUpResultGroup = true;
                    }
                    else if (isDescription)
                    {
                        ProcessDescriptionRow(sb, row, saved, ref descriptionHeaderShown);
                    }
                    else
                    {
                        // Process regular row normally
                        ProcessRegularRow(sb, row, saved, ref descriptionHeaderShown);
                    }
                }

                // Process any remaining check-up result rows at the end
                if (inCheckUpResultGroup && checkUpResultRows.Count > 0)
                {
                    ProcessCheckUpResultGroup(sb, checkUpResultRows, saved);
                }

                sb.Append("</table>");

                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
            }
        }

        private void ProcessCheckUpResultGroup(StringBuilder sb, List<DataRow> rows, DataTable saved)
        {
            if (rows.Count == 0) return;

            // First row - shows the main requirement and creates the A/B/C table header
            DataRow firstRow = rows[0];
            int firstClauseID = Convert.ToInt32(firstRow["ClauseID"]);
            string inputType = GetInputType(firstRow);

            // Main requirement row
            sb.Append("<tr>");
            sb.Append($"<td class='clause-number'>{firstRow["ClauseNumber"]}</td>");

            // Requirements column - show the main requirement text
            sb.Append("<td class='requirement'>");
            sb.Append($"<u>{firstRow["Requirement_BH"]}</u><br/>");
            sb.Append($"<i>{firstRow["Requirement"]}</i>");
            sb.Append("</td>");

            // Specification column - create the A/B/C table header and threshold row
            sb.Append("<td class='specification' style='padding:0;'>");
            sb.Append("<table style='width:100%; border-collapse: collapse; font-size:10px; border:1px solid #ccc;'>");

            // Header row
            sb.Append("<tr style='text-align:center; font-weight:bold; background-color:#f0f0f0;'>");
            sb.Append("<td style='border:1px solid #ccc; width:33%; text-align: center;'>A</td>");
            sb.Append("<td style='border:1px solid #ccc; width:33%; text-align: center;'>B</td>");
            sb.Append("<td style='border:1px solid #ccc; width:33%; text-align: center;'>C</td>");
            sb.Append("</tr>");

            // Threshold row
            sb.Append("<tr style='text-align:center;'>");
            sb.Append("<td style='border:1px solid #ccc;'><input type='text' name='spec_threshold_A' value='&lt; 45' style='width:90%; text-align:center; border:none; background:transparent;' readonly /></td>");
            sb.Append("<td style='border:1px solid #ccc;'><input type='text' name='spec_threshold_B' value='&lt; 45' style='width:90%; text-align:center; border:none; background:transparent;' readonly /></td>");
            sb.Append("<td style='border:1px solid #ccc;'><input type='text' name='spec_threshold_C' value='&lt; 45' style='width:90%; text-align:center; border:none; background:transparent;' readonly /></td>");
            sb.Append("</tr>");

            sb.Append("</table>");
            sb.Append("</td>");

            // Result column
            sb.Append("<td class='result'>");
            sb.Append($"<input type='hidden' name='inputtype_{firstClauseID}' value='{inputType}' />");
            GenerateResultInput(sb, firstRow, saved, firstClauseID);
            sb.Append("</td>");
            sb.Append("</tr>");

            // Subsequent rows - each measurement gets its own row with measurement name in Requirements column
            for (int i = 1; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                int clauseID = Convert.ToInt32(row["ClauseID"]);

                sb.Append("<tr>");
                sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                // Requirements column - show the measurement name (Phase contact, Neutral contact, etc.)
                sb.Append("<td class='requirement'>");
                sb.Append($"<u>{row["Requirement_BH"]}</u><br/>");
                sb.Append($"<i>{row["Requirement"]}</i>");
                sb.Append("</td>");

                // Specification column - input fields for this measurement
                sb.Append("<td class='specification' style='padding:0;'>");
                sb.Append("<table style='width:100%; border-collapse: collapse; font-size:10px; border:1px solid #ccc;'>");
                sb.Append("<tr style='text-align:center;'>");

                // Get saved values for this specific measurement - parse from Remark column
                string savedValueA = "", savedValueB = "", savedValueC = "";
                var savedRow = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                if (savedRow != null)
                {
                    string remark = savedRow["Remark"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(remark))
                    {
                        // Split the pipe-separated values: "10 mm|12 mm|22 mm"
                        string[] values = remark.Split('|');
                        if (values.Length >= 1) savedValueA = values[0];
                        if (values.Length >= 2) savedValueB = values[1];
                        if (values.Length >= 3) savedValueC = values[2];
                    }
                }

                sb.Append($"<td style='border:1px solid #ccc; width:33%;'><input type='text' name='spec_value_A_{clauseID}' value='{HttpUtility.HtmlEncode(savedValueA)}' style='width:90%; text-align:center;' /></td>");
                sb.Append($"<td style='border:1px solid #ccc; width:33%;'><input type='text' name='spec_value_B_{clauseID}' value='{HttpUtility.HtmlEncode(savedValueB)}' style='width:90%; text-align:center;' /></td>");
                sb.Append($"<td style='border:1px solid #ccc; width:33%;'><input type='text' name='spec_value_C_{clauseID}' value='{HttpUtility.HtmlEncode(savedValueC)}' style='width:90%; text-align:center;' /></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");

                // Result column - empty or with appropriate input
                sb.Append("<td class='result'>");
                sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                GenerateResultInput(sb, row, saved, clauseID);
                sb.Append("</td>");
                sb.Append("</tr>");
            }
        }

        private void ProcessDescriptionRow(StringBuilder sb, DataRow row, DataTable saved, ref bool descriptionHeaderShown)
        {
            int clauseID = Convert.ToInt32(row["ClauseID"]);
            string inputType = GetInputType(row);

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

            sb.Append("<tr>");
            sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");
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

            sb.Append("<td class='specification'>");
            string specification = row["Specification"]?.ToString() ?? "";
            string specification_BH = row["Specification_BH"]?.ToString() ?? "";
            string savedSpecification = ""; string savedSpecification_BH = "";
            var savedRowForSpec = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
            if (savedRowForSpec != null)
            {
                savedSpecification = savedRowForSpec["Remark"]?.ToString() ?? "";
                savedSpecification_BH = savedRowForSpec["Remark_BH"]?.ToString() ?? "";
            }
            string displaySpecification = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
            string displaySpecification_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;

            if (!string.IsNullOrEmpty(specification_BH) || !string.IsNullOrEmpty(displaySpecification))
            {
                if (!string.IsNullOrEmpty(specification_BH))
                    sb.Append($"<input type='text' name='specification_BH{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification_BH)}' />");
                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification)}' />");
            }
            sb.Append("</td>");

            sb.Append("<td class='result'>");
            sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
            GenerateResultInput(sb, row, saved, clauseID);
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        private void ProcessRegularRow(StringBuilder sb, DataRow row, DataTable saved, ref bool descriptionHeaderShown)
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

            sb.Append("<td class='specification'>");
            string specification = row["Specification"]?.ToString() ?? "";
            string specification_BH = row["Specification_BH"]?.ToString() ?? "";
            string savedSpecification = ""; string savedSpecification_BH = "";
            var savedRowForSpec = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
            if (savedRowForSpec != null)
                savedSpecification = savedRowForSpec["Specification"]?.ToString() ?? "";
            string displaySpecification = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
            string displaySpecification_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;

            if (!string.IsNullOrEmpty(specification_BH) || !string.IsNullOrEmpty(displaySpecification))
            {
                if (!string.IsNullOrEmpty(specification_BH))
                    sb.Append($"<input type='text' name='specification_BH_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification_BH)}' />");
                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification)}' />");
            }
            sb.Append("</td>");

            sb.Append("<td class='result'>");
            sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
            GenerateResultInput(sb, row, saved, clauseID);
            sb.Append("</td>");
            sb.Append("</tr>");
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
                        bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                        string inputType = Request.Form[$"inputtype_{clauseID}"] ?? "";

                        string specification = "";
                        string specification_BH = "";

                        // Handle different types of specification data
                        if (isCheckUpResult)
                        {
                            // For check-up result rows, get A, B, C values and combine with pipe separator
                            string valueA = Request.Form[$"spec_value_A_{clauseID}"] ?? "";
                            string valueB = Request.Form[$"spec_value_B_{clauseID}"] ?? "";
                            string valueC = Request.Form[$"spec_value_C_{clauseID}"] ?? "";

                            // Combine values with pipe separator
                            specification = $"{valueA}|{valueB}|{valueC}";
                            specification_BH = specification; // Use same format for BH if needed
                        }
                        else
                        {
                            // For regular rows, use normal specification fields
                            specification = Request.Form[$"specification_{clauseID}"] ?? "";
                            specification_BH = Request.Form[$"specification_BH{clauseID}"] ?? "";
                        }

                        if (isHeader)
                        {
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

                            int result0 = _cm.TestResultsAdd_EA(clauseID, txtstartingdate.Text, txtstartingdate.Text, result, specification, specification_BH, sampleId, sampleIdSeries,
                                inputType, language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, lblReportName.Text, txtsampleCode.Text,
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
                        /* Report Generate Code Start */
                        string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
                        string pathout = Server.MapPath("~/Reports/");
                        string templatePath = Server.MapPath("~/InputFiles/EATestRecord_2.docx");

                                    if (!Directory.Exists(pathout))
                                        Directory.CreateDirectory(pathout);

                                    string outputDocxPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".docx");
                                    string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

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

            if (DisResget.Rows.Count > 0)
            {
                string equipmentIds = DisResget.Rows[0]["EquipmentIds"].ToString();
                List<string> equipmentNames = GetEquipmentNamesAsList(equipmentIds);
                DataRow row = DisResget.Rows[0];
                DataTable docNumberGet = _cm.GetDocumentsNumber(lblReportID.Text);
                string DocNo = docNumberGet.Rows[0]["DocNo"].ToString();

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

                    {"{DocNo}", DocNo },
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
            dt.Columns.Add("isCheckUpResult", typeof(string)); // Add this column

            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);
            string standardID = lblstandardID.Text;

            // Get clause data
            DataTable clauseData = GetClauseData();
            // Get saved results
            DataTable savedResults = GetSavedResults();

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

                        // IMPORTANT: Populate the isCheckUpResult column
                        if (clauseRow.Table.Columns.Contains("isCheckUpResult"))
                        {
                            newRow["isCheckUpResult"] = clauseRow["isCheckUpResult"]?.ToString() ?? "0";
                        }
                        else
                        {
                            // If the column doesn't exist in source, set default value
                            newRow["isCheckUpResult"] = "0";
                        }

                        // Get saved data
                        var savedRow = savedResults.AsEnumerable()
                            .FirstOrDefault(r => Convert.ToInt32(r["ClauseID"]) == clauseID);

                        if (savedRow != null)
                        {
                            if (clauseRow["isDescription"].ToString() == "1")
                            {
                                newRow["Specification"] = savedRow["Remark"]?.ToString() ?? "";
                            }
                            else
                            {
                                newRow["Specification"] = savedRow["Specification"]?.ToString() ?? clauseRow["Specification"]?.ToString() ?? "";
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


        /* final without border work */
        
        private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            // Font configuration constants for easy maintenance
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

            // Column width configuration (in inches) for easy maintenance
            float[] COLUMN_WIDTHS = { 0.60f, 2.80f, 2.30f, 0.80f };
            float SUB_COLUMN1_WIDTH = 2.1f;
            float SUB_COLUMN2_WIDTH = 0.7f;
            float TABLE_TOTAL_WIDTH = 6.5f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // STEP 1: Count rows - adjust for different row types
            int totalRows = 1; // header row
            bool needDescHeader = false;
            bool inTemperatureGroup = false;
            int temperatureRowCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";

                if (isHeader)
                {
                    totalRows++;
                    needDescHeader = false;
                    if (inTemperatureGroup) temperatureRowCount = 0;
                    inTemperatureGroup = false;
                }
                else if (isDescription)
                {
                    if (!needDescHeader)
                    {
                        totalRows += 1;
                        needDescHeader = true;
                    }
                    totalRows++;
                    if (inTemperatureGroup) temperatureRowCount = 0;
                    inTemperatureGroup = false;
                }
                else if (isCheckUpResult)
                {
                    if (!inTemperatureGroup)
                    {
                        totalRows += 2; // threshold header + threshold values
                        inTemperatureGroup = true;
                        temperatureRowCount = 1;
                    }
                    else
                    {
                        totalRows++;
                        temperatureRowCount++;
                    }
                }
                else
                {
                    totalRows++;
                    needDescHeader = false;
                    if (inTemperatureGroup) temperatureRowCount = 0;
                    inTemperatureGroup = false;
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

            // MAIN HEADER - Always 4 columns
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
            List<DataRow> temperatureRows = new List<DataRow>();
            DataRow thresholdRow = null;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                string requirement = row["Requirement"]?.ToString() ?? "";

                // Process temperature rows if we encounter non-temperature row or end of data
                if ((isHeader || isDescription || (!isCheckUpResult && temperatureRows.Count > 0)) &&
                    (temperatureRows.Count > 0 || thresholdRow != null))
                {
                    CreateTemperatureSection(table, r, thresholdRow, temperatureRows, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    r += temperatureRows.Count + 2; // +2 for threshold header and values rows
                    temperatureRows.Clear();
                    thresholdRow = null;
                }

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
                    SetCellText(c4, row["Result"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
                else if (isCheckUpResult)
                {
                    // Check if this is the threshold row
                    if (requirement.Contains("temperature rise should not exceed"))
                    {
                        thresholdRow = row;
                    }
                    else
                    {
                        temperatureRows.Add(row);
                    }
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

                    Cell a3 = table.Cell(r, 3);
                    SetCellText(a3, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
                else
                {
                    descHeaderShown = false;

                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
            }

            // Process any remaining temperature rows at the end
            if (temperatureRows.Count > 0 || thresholdRow != null)
            {
                CreateTemperatureSection(table, r, thresholdRow, temperatureRows, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
            }
        }

        private void CreateTemperatureSection(Table table, int startRow, DataRow thresholdRow, List<DataRow> temperatureRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            if (temperatureRows.Count == 0 && thresholdRow == null) return;

            // Create threshold header row
            Cell t1 = table.Cell(startRow, 1);
            SetCellText(t1, "", false, fontSize, fontName);

            Cell t2 = table.Cell(startRow, 2);
            SetCellText(t2, "The temperature rise should not exceed 45 K (K)", false, fontSize, fontName);

            // Column 3: Create sub-table for A, B, C headers
            Cell t3 = table.Cell(startRow, 3);
            Table headerSubTable = t3.Range.Tables.Add(t3.Range, 1, 3);
            headerSubTable.Borders.Enable = 0; // NO BORDERS
            headerSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2]);

            // Set equal widths for three columns
            float subColWidth = columnWidths[2] / 3;
            for (int i = 1; i <= 3; i++)
            {
                headerSubTable.Columns[i].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
            }

            // Set A, B, C headers
            Cell headerA = headerSubTable.Cell(1, 1);
            headerA.Range.Text = "A";
            headerA.Range.Font.Size = fontSize;
            headerA.Range.Font.Name = fontName;
            headerA.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            Cell headerB = headerSubTable.Cell(1, 2);
            headerB.Range.Text = "B";
            headerB.Range.Font.Size = fontSize;
            headerB.Range.Font.Name = fontName;
            headerB.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            Cell headerC = headerSubTable.Cell(1, 3);
            headerC.Range.Text = "C";
            headerC.Range.Font.Size = fontSize;
            headerC.Range.Font.Name = fontName;
            headerC.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            ApplyBorders(t3);

            Cell t4 = table.Cell(startRow, 4);
            SetCellText(t4, thresholdRow?["Result"]?.ToString() ?? "", false, fontSize, fontName);

            // Create threshold values row
            Cell v1 = table.Cell(startRow + 1, 1);
            SetCellText(v1, "", false, fontSize, fontName);

            Cell v2 = table.Cell(startRow + 1, 2);
            SetCellText(v2, "", false, fontSize, fontName);

            // Column 3: Create sub-table for threshold values
            Cell v3 = table.Cell(startRow + 1, 3);
            Table valueSubTable = v3.Range.Tables.Add(v3.Range, 1, 3);
            valueSubTable.Borders.Enable = 0; // NO BORDERS
            valueSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2]);

            // Set equal widths
            for (int i = 1; i <= 3; i++)
            {
                valueSubTable.Columns[i].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
            }

            // Set threshold values
            Cell valueA = valueSubTable.Cell(1, 1);
            valueA.Range.Text = "< 45";
            valueA.Range.Font.Size = fontSize;
            valueA.Range.Font.Name = fontName;
            valueA.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            Cell valueB = valueSubTable.Cell(1, 2);
            valueB.Range.Text = "< 45";
            valueB.Range.Font.Size = fontSize;
            valueB.Range.Font.Name = fontName;
            valueB.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            Cell valueC = valueSubTable.Cell(1, 3);
            valueC.Range.Text = "< 45";
            valueC.Range.Font.Size = fontSize;
            valueC.Range.Font.Name = fontName;
            valueC.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            ApplyBorders(v3);

            Cell v4 = table.Cell(startRow + 1, 4);
            SetCellText(v4, "", false, fontSize, fontName);

            // Create temperature data rows
            for (int i = 0; i < temperatureRows.Count; i++)
            {
                DataRow row = temperatureRows[i];
                int currentRow = startRow + 2 + i;

                Cell c1 = table.Cell(currentRow, 1);
                SetCellText(c1, row["ClauseNumber"].ToString(), false, fontSize, fontName);

                Cell c2 = table.Cell(currentRow, 2);
                SetCellText(c2, row["Requirement"].ToString(), false, fontSize, fontName);

                // Column 3: Create sub-table for temperature values
                Cell c3 = table.Cell(currentRow, 3);
                Table dataSubTable = c3.Range.Tables.Add(c3.Range, 1, 3);
                dataSubTable.Borders.Enable = 0; // NO BORDERS
                dataSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2]);

                // Set equal widths
                for (int j = 1; j <= 3; j++)
                {
                    dataSubTable.Columns[j].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
                }

                // Parse pipe-separated temperature values
                string[] tempValues = new string[3];
                if (!string.IsNullOrEmpty(row["Specification"]?.ToString()))
                {
                    string spec = row["Specification"].ToString();
                    if (spec.Contains("|"))
                    {
                        string[] values = spec.Split('|');
                        for (int j = 0; j < Math.Min(3, values.Length); j++)
                        {
                            tempValues[j] = values[j].Trim();
                        }
                    }
                    else
                    {
                        tempValues[0] = spec;
                    }
                }

                // Fill all three columns
                Cell dataA = dataSubTable.Cell(1, 1);
                dataA.Range.Text = tempValues[0];
                dataA.Range.Font.Size = fontSize;
                dataA.Range.Font.Name = fontName;
                dataA.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                Cell dataB = dataSubTable.Cell(1, 2);
                dataB.Range.Text = tempValues[1];
                dataB.Range.Font.Size = fontSize;
                dataB.Range.Font.Name = fontName;
                dataB.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                Cell dataC = dataSubTable.Cell(1, 3);
                dataC.Range.Text = tempValues[2];
                dataC.Range.Font.Size = fontSize;
                dataC.Range.Font.Name = fontName;
                dataC.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                ApplyBorders(c3);

                Cell c4 = table.Cell(currentRow, 4);
                SetCellText(c4, row["Result"].ToString(), false, fontSize, fontName);
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

        private void ApplyBorders(Cell cell)
        {
            cell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
        }

        private void ApplySubTableVerticalBorders(Table subTable)
        {// Remove all borders first
            subTable.Borders.Enable = 0;
            // Apply only vertical border between columns (right border of first and second cells)
            foreach (Row subRow in subTable.Rows)
            {
                if (subRow.Cells.Count >= 2)
                {// Apply right border to create vertical lines between columns
                    for (int i = 1; i < subRow.Cells.Count; i++)
                    {
                        Cell cell = subRow.Cells[i];
                        cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    }
                }
            }
        }

        

        /* new code*/

        //private void CreateTemperatureSection(Table table, int startRow, DataRow thresholdRow, List<DataRow> temperatureRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        //{
        //    if (temperatureRows.Count == 0 && thresholdRow == null) return;

        //    // ROW 1: Threshold header with A, B, C columns
        //    Cell t1 = table.Cell(startRow, 1);
        //    SetCellText(t1, "", false, fontSize, fontName);

        //    Cell t2 = table.Cell(startRow, 2);
        //    SetCellText(t2, "The temperature rise should not exceed 45 K (K)", false, fontSize, fontName);

        //    // Column 3: Create sub-table for A, B, C headers
        //    Cell t3 = table.Cell(startRow, 3);
        //    Table headerSubTable = t3.Range.Tables.Add(t3.Range, 1, 3);
        //    headerSubTable.Borders.Enable = 0; // No external borders
        //    headerSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2]);

        //    // Set equal widths for three columns
        //    float subColWidth = columnWidths[2] / 3;
        //    for (int i = 1; i <= 3; i++)
        //    {
        //        headerSubTable.Columns[i].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
        //    }

        //    // Set A, B, C headers
        //    string[] headers = { "A", "B", "C" };
        //    for (int i = 1; i <= 3; i++)
        //    {
        //        Cell headerCell = headerSubTable.Cell(1, i);
        //        headerCell.Range.Text = headers[i - 1];
        //        headerCell.Range.Font.Size = fontSize;
        //        headerCell.Range.Font.Name = fontName;
        //        headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
        //        headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

        //        // Apply borders to all sides for proper appearance
        //        headerCell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        headerCell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        headerCell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        headerCell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
        //    }

        //    ApplyBorders(t3);

        //    Cell t4 = table.Cell(startRow, 4);
        //    SetCellText(t4, thresholdRow?["Result"]?.ToString() ?? "ABC", false, fontSize, fontName);

        //    // ROW 2: Threshold values row
        //    Cell v1 = table.Cell(startRow + 1, 1);
        //    SetCellText(v1, "", false, fontSize, fontName);

        //    Cell v2 = table.Cell(startRow + 1, 2);
        //    SetCellText(v2, "", false, fontSize, fontName);

        //    // Column 3: Create sub-table for threshold values
        //    Cell v3 = table.Cell(startRow + 1, 3);
        //    Table valueSubTable = v3.Range.Tables.Add(v3.Range, 1, 3);
        //    valueSubTable.Borders.Enable = 0; // No external borders
        //    valueSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2]);

        //    // Set equal widths
        //    for (int i = 1; i <= 3; i++)
        //    {
        //        valueSubTable.Columns[i].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
        //    }

        //    // Set threshold values
        //    string[] thresholdValues = { "< 45", "< 45", "< 45" };
        //    for (int i = 1; i <= 3; i++)
        //    {
        //        Cell valueCell = valueSubTable.Cell(1, i);
        //        valueCell.Range.Text = thresholdValues[i - 1];
        //        valueCell.Range.Font.Size = fontSize;
        //        valueCell.Range.Font.Name = fontName;
        //        valueCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
        //        valueCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

        //        // Apply borders to all sides for proper appearance
        //        valueCell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        valueCell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        valueCell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        valueCell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
        //    }

        //    ApplyBorders(v3);

        //    Cell v4 = table.Cell(startRow + 1, 4);
        //    SetCellText(v4, "", false, fontSize, fontName);

        //    // Create temperature data rows
        //    for (int i = 0; i < temperatureRows.Count; i++)
        //    {
        //        DataRow row = temperatureRows[i];
        //        int currentRow = startRow + 2 + i; // +2 because we used two rows for threshold

        //        Cell c1 = table.Cell(currentRow, 1);
        //        // Use the actual clause number from the data row instead of empty string
        //        string clauseNumber = row["ClauseNumber"]?.ToString();
        //        SetCellText(c1, string.IsNullOrEmpty(clauseNumber) ? "" : clauseNumber, false, fontSize, fontName);

        //        Cell c2 = table.Cell(currentRow, 2);
        //        SetCellText(c2, row["Requirement"].ToString(), false, fontSize, fontName);

        //        // Column 3: Create sub-table for temperature values with proper borders
        //        Cell c3 = table.Cell(currentRow, 3);
        //        Table dataSubTable = c3.Range.Tables.Add(c3.Range, 1, 3);
        //        dataSubTable.Borders.Enable = 0; // No external borders
        //        dataSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2]);

        //        // Set equal widths
        //        for (int j = 1; j <= 3; j++)
        //        {
        //            dataSubTable.Columns[j].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
        //        }

        //        // Parse pipe-separated temperature values
        //        string[] tempValues = new string[3];
        //        if (!string.IsNullOrEmpty(row["Specification"]?.ToString()))
        //        {
        //            string spec = row["Specification"].ToString();
        //            if (spec.Contains("|"))
        //            {
        //                string[] values = spec.Split('|');
        //                for (int j = 0; j < Math.Min(3, values.Length); j++)
        //                {
        //                    tempValues[j] = values[j].Trim();
        //                }
        //            }
        //            else
        //            {
        //                tempValues[0] = spec;
        //            }
        //        }

        //        // Fill all three columns with proper borders
        //        for (int j = 1; j <= 3; j++)
        //        {
        //            Cell dataCell = dataSubTable.Cell(1, j);
        //            dataCell.Range.Text = tempValues[j - 1];
        //            dataCell.Range.Font.Size = fontSize;
        //            dataCell.Range.Font.Name = fontName;
        //            dataCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
        //            dataCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

        //            // Apply borders to all sides for proper appearance
        //            dataCell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
        //            dataCell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
        //            dataCell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
        //            dataCell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
        //        }

        //        // Apply main table borders to the container cell
        //        ApplyBorders(c3);

        //        Cell c4 = table.Cell(currentRow, 4);
        //        SetCellText(c4, row["Result"].ToString(), false, fontSize, fontName);
        //    }
        //}

        //private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        //{
        //    // Font configuration constants for easy maintenance
        //    const float DEFAULT_FONT_SIZE = 10;
        //    const string DEFAULT_FONT_NAME = "Calibri";
        //    const float HEADER_FONT_SIZE = 10;

        //    // Column width configuration (in inches) for easy maintenance
        //    float[] COLUMN_WIDTHS = { 0.60f, 2.80f, 2.30f, 0.80f };
        //    float SUB_COLUMN1_WIDTH = 2.1f;
        //    float SUB_COLUMN2_WIDTH = 0.7f;
        //    float TABLE_TOTAL_WIDTH = 6.5f;

        //    Range range = doc.Content;
        //    Find find = range.Find;
        //    find.ClearFormatting();
        //    find.Text = placeholder;

        //    if (!find.Execute())
        //        throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

        //    // STEP 1: Count rows - adjust for different row types
        //    int totalRows = 1; // header row
        //    bool needDescHeader = false;
        //    bool inTemperatureGroup = false;
        //    int temperatureRowCount = 0;

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        bool isHeader = row["isHeader"].ToString() == "1";
        //        bool isDescription = row["isDescription"].ToString() == "1";
        //        bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";

        //        if (isHeader)
        //        {
        //            totalRows++;
        //            needDescHeader = false;
        //            if (inTemperatureGroup) temperatureRowCount = 0;
        //            inTemperatureGroup = false;
        //        }
        //        else if (isDescription)
        //        {
        //            if (!needDescHeader)
        //            {
        //                totalRows += 1;
        //                needDescHeader = true;
        //            }
        //            totalRows++;
        //            if (inTemperatureGroup) temperatureRowCount = 0;
        //            inTemperatureGroup = false;
        //        }
        //        else if (isCheckUpResult)
        //        {
        //            if (!inTemperatureGroup)
        //            {
        //                totalRows += 2; // CHANGED BACK TO 2 ROWS for threshold header + values
        //                inTemperatureGroup = true;
        //                temperatureRowCount = 1;
        //            }
        //            else
        //            {
        //                totalRows++;
        //                temperatureRowCount++;
        //            }
        //        }
        //        else
        //        {
        //            totalRows++;
        //            needDescHeader = false;
        //            if (inTemperatureGroup) temperatureRowCount = 0;
        //            inTemperatureGroup = false;
        //        }
        //    }

        //    // STEP 2: Build table with 4 main columns
        //    Table table = doc.Tables.Add(range, totalRows, 4);
        //    table.Borders.Enable = 1;
        //    table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);

        //    // Apply default font to entire table
        //    table.Range.Font.Size = DEFAULT_FONT_SIZE;
        //    table.Range.Font.Name = DEFAULT_FONT_NAME;

        //    // Adjust column widths
        //    for (int c = 1; c <= 4; c++)
        //        table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

        //    // MAIN HEADER - Always 4 columns
        //    string[] headers = { "Clause", "Test requirements", "Check up result", "Decision" };
        //    for (int col = 1; col <= 4; col++)
        //    {
        //        Cell cell = table.Cell(1, col);
        //        cell.Range.Text = headers[col - 1];
        //        cell.Range.Bold = 1;
        //        cell.Range.Font.Size = HEADER_FONT_SIZE;
        //        cell.Range.Font.Name = DEFAULT_FONT_NAME;
        //        cell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
        //        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
        //        cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
        //        ApplyBorders(cell);
        //    }

        //    int r = 2;
        //    bool descHeaderShown = false;
        //    List<DataRow> temperatureRows = new List<DataRow>();
        //    DataRow thresholdRow = null;

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        bool isHeader = row["isHeader"].ToString() == "1";
        //        bool isDescription = row["isDescription"].ToString() == "1";
        //        bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
        //        string requirement = row["Requirement"]?.ToString() ?? "";

        //        // Process temperature rows if we encounter non-temperature row or end of data
        //        if ((isHeader || isDescription || (!isCheckUpResult && temperatureRows.Count > 0)) &&
        //            (temperatureRows.Count > 0 || thresholdRow != null))
        //        {
        //            CreateTemperatureSection(table, r, thresholdRow, temperatureRows, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //            r += temperatureRows.Count + 2; // CHANGED BACK TO +2 for threshold header + values rows
        //            temperatureRows.Clear();
        //            thresholdRow = null;
        //        }

        //        if (isHeader)
        //        {
        //            descHeaderShown = false;

        //            Cell c1 = table.Cell(r, 1);
        //            SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            Cell c2 = table.Cell(r, 2);
        //            SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //            c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

        //            Cell c3 = table.Cell(r, 3);
        //            SetCellText(c3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            Cell c4 = table.Cell(r, 4);
        //            SetCellText(c4, row["Result"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            r++;
        //        }
        //        else if (isCheckUpResult)
        //        {
        //            // Check if this is the threshold row
        //            if (requirement.Contains("temperature rise should not exceed"))
        //            {
        //                thresholdRow = row;
        //            }
        //            else
        //            {
        //                temperatureRows.Add(row);
        //            }
        //        }
        //        else if (isDescription)
        //        {
        //            if (!descHeaderShown)
        //            {
        //                Cell d1 = table.Cell(r, 1);
        //                SetCellText(d1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //                Cell d2 = table.Cell(r, 2);
        //                Table subTableHeader = d2.Range.Tables.Add(d2.Range, 1, 2);
        //                subTableHeader.Borders.Enable = 0;
        //                subTableHeader.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
        //                subTableHeader.Range.Font.Size = DEFAULT_FONT_SIZE;
        //                subTableHeader.Range.Font.Name = DEFAULT_FONT_NAME;

        //                subTableHeader.Columns[1].SetWidth(wordApp.InchesToPoints(SUB_COLUMN1_WIDTH), WdRulerStyle.wdAdjustNone);
        //                subTableHeader.Columns[2].SetWidth(wordApp.InchesToPoints(SUB_COLUMN2_WIDTH), WdRulerStyle.wdAdjustNone);

        //                Cell subHeader1 = subTableHeader.Cell(1, 1);
        //                SetCellText(subHeader1, "Description", true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //                subHeader1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

        //                Cell subHeader2 = subTableHeader.Cell(1, 2);
        //                SetCellText(subHeader2, "Condition (mm)", true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //                subHeader2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

        //                ApplySubTableVerticalBorders(subTableHeader);
        //                ApplyBorders(d2);

        //                Cell d3 = table.Cell(r, 3);
        //                SetCellText(d3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //                Cell d4 = table.Cell(r, 4);
        //                SetCellText(d4, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //                r++;
        //                descHeaderShown = true;
        //            }

        //            // Data row with Description and Condition in sub-columns
        //            Cell a1 = table.Cell(r, 1);
        //            SetCellText(a1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            Cell a2 = table.Cell(r, 2);
        //            Table subTableData = a2.Range.Tables.Add(a2.Range, 1, 2);
        //            subTableData.Borders.Enable = 0;
        //            subTableData.PreferredWidth = wordApp.InchesToPoints(COLUMN_WIDTHS[1]);
        //            subTableData.Range.Font.Size = DEFAULT_FONT_SIZE;
        //            subTableData.Range.Font.Name = DEFAULT_FONT_NAME;

        //            subTableData.Columns[1].SetWidth(wordApp.InchesToPoints(SUB_COLUMN1_WIDTH), WdRulerStyle.wdAdjustNone);
        //            subTableData.Columns[2].SetWidth(wordApp.InchesToPoints(SUB_COLUMN2_WIDTH), WdRulerStyle.wdAdjustNone);

        //            Cell subData1 = subTableData.Cell(1, 1);
        //            SetCellText(subData1, row["Description"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //            subData1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

        //            Cell subData2 = subTableData.Cell(1, 2);
        //            SetCellText(subData2, row["Condition"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //            subData2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

        //            ApplySubTableVerticalBorders(subTableData);
        //            ApplyBorders(a2);

        //            Cell a3 = table.Cell(r, 3);
        //            SetCellText(a3, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //            a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

        //            Cell a4 = table.Cell(r, 4);
        //            SetCellText(a4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //            a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

        //            r++;
        //        }
        //        else
        //        {
        //            descHeaderShown = false;

        //            Cell c1 = table.Cell(r, 1);
        //            SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            Cell c2 = table.Cell(r, 2);
        //            SetCellText(c2, row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            Cell c3 = table.Cell(r, 3);
        //            SetCellText(c3, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            Cell c4 = table.Cell(r, 4);
        //            SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

        //            r++;
        //        }
        //    }

        //    // Process any remaining temperature rows at the end
        //    if (temperatureRows.Count > 0 || thresholdRow != null)
        //    {
        //        CreateTemperatureSection(table, r, thresholdRow, temperatureRows, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
        //    }
        //}
                
        //private void SetCellText(Cell cell, string text, bool bold, float fontSize, string fontName)
        //{
        //    cell.Range.Text = text;
        //    cell.Range.Bold = bold ? 1 : 0;
        //    cell.Range.Font.Size = fontSize;
        //    cell.Range.Font.Name = fontName;
        //    ApplyBorders(cell);
        //}

        //private void ApplyBorders(Cell cell)
        //{
        //    cell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
        //    cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
        //    cell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
        //    cell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

        //    // Set border weight and color for consistency
        //    cell.Borders[WdBorderType.wdBorderLeft].LineWidth = WdLineWidth.wdLineWidth050pt;
        //    cell.Borders[WdBorderType.wdBorderRight].LineWidth = WdLineWidth.wdLineWidth050pt;
        //    cell.Borders[WdBorderType.wdBorderTop].LineWidth = WdLineWidth.wdLineWidth050pt;
        //    cell.Borders[WdBorderType.wdBorderBottom].LineWidth = WdLineWidth.wdLineWidth050pt;
        //}

        //private void ApplySubTableVerticalBorders(Table subTable)
        //{
        //    // Remove all borders first
        //    subTable.Borders.Enable = 0;

        //    // Apply only vertical border between columns (right border of first and second cells)
        //    foreach (Row subRow in subTable.Rows)
        //    {
        //        if (subRow.Cells.Count >= 2)
        //        {
        //            // Apply right border to create vertical lines between columns
        //            for (int i = 1; i < subRow.Cells.Count; i++)
        //            {
        //                Cell cell = subRow.Cells[i];
        //                cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
        //                cell.Borders[WdBorderType.wdBorderRight].LineWidth = WdLineWidth.wdLineWidth050pt;
        //            }
        //        }
        //    }
        //}


    }
}