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
    public partial class TemperatureRiseSwitchSupervisionTest : System.Web.UI.Page
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

            // First row - shows the main requirement and creates the table header
            DataRow firstRow = rows[0];
            int firstClauseID = Convert.ToInt32(firstRow["ClauseID"]);
            string inputType = GetInputType(firstRow);
            string requirement = firstRow["Requirement"]?.ToString() ?? "";
            string requirement_BH = firstRow["Requirement_BH"]?.ToString() ?? "";

            // Determine column configuration based on requirement text
            int columnCount = 3;
            string[] columnHeaders = { "A", "B", "C" };
            string[] thresholdValues = { "", "", "" };

            // Check for 5-column configuration - "Number of screwless terminals"
            if (requirement.Contains("Number of screwless terminals") ||
                requirement_BH.Contains("Number of screwless terminals"))
            {
                columnCount = 5;
                columnHeaders = new string[] { "1", "2", "3", "4", "5" };
                thresholdValues = new string[] { "-", "-", "-", "-", "-" };
            }
            // Check for 3-column with numeric headers - "Terminal temperature rise"
            else if (requirement.Contains("Terminal temperature rise") ||
                     requirement_BH.Contains("Terminal temperature rise") ||
                     requirement.Contains("Terminal temperature rise") ||
                     requirement_BH.Contains("Terminal temperature rise"))
            {
                columnCount = 3;
                columnHeaders = new string[] { "A", "B", "C" };
                thresholdValues = new string[] { " ", " ", " " };
            }
            // Default: 3 columns A, B, C
            else
            {
                columnCount = 3;
                columnHeaders = new string[] { "A", "B", "C" };
                thresholdValues = new string[] { " ", " ", " " };
            }

            int columnWidth = 100 / columnCount;

            // Main requirement row
            sb.Append("<tr>");
            sb.Append($"<td class='clause-number'>{firstRow["ClauseNumber"]}</td>");

            // Requirements column - show the main requirement text
            sb.Append("<td class='requirement'>");
            sb.Append($"<u>{firstRow["Requirement_BH"]}</u><br/>");
            sb.Append($"<i>{firstRow["Requirement"]}</i>");
            sb.Append("</td>");

            // Specification column - create the table header and threshold row
            sb.Append("<td class='specification' style='padding:0;'>");
            sb.Append("<table style='width:100%; border-collapse: collapse; font-size:10px; border:1px solid #ccc;'>");

            // Header row - dynamic columns
            sb.Append("<tr style='text-align:center; font-weight:bold; background-color:#f0f0f0;'>");
            for (int i = 0; i < columnCount; i++)
            {
                sb.Append($"<td style='border:1px solid #ccc; width:{columnWidth}%; text-align: center;'>{columnHeaders[i]}</td>");
            }
            sb.Append("</tr>");

            // Threshold row - dynamic columns
            sb.Append("<tr style='text-align:center;'>");
            for (int i = 0; i < columnCount; i++)
            {
                sb.Append($"<td style='border:1px solid #ccc;'><input type='text' name='spec_threshold_{columnHeaders[i]}' value='{thresholdValues[i]}' style='width:90%; text-align:center; border:none; background:transparent;' readonly /></td>");
            }
            sb.Append("</tr>");

            sb.Append("</table>");
            sb.Append("</td>");

            // Result column
            sb.Append("<td class='result'>");
            sb.Append($"<input type='hidden' name='inputtype_{firstClauseID}' value='{inputType}' />");
            sb.Append($"<input type='hidden' name='columncount_{firstClauseID}' value='{columnCount}' />");
            sb.Append($"<input type='hidden' name='columnheaders_{firstClauseID}' value='{string.Join(",", columnHeaders)}' />");
            GenerateResultInput(sb, firstRow, saved, firstClauseID);
            sb.Append("</td>");
            sb.Append("</tr>");

            // Subsequent rows - each measurement gets its own row
            for (int i = 1; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                int clauseID = Convert.ToInt32(row["ClauseID"]);

                sb.Append("<tr>");
                sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                // Requirements column - show the measurement name
                sb.Append("<td class='requirement'>");
                sb.Append($"<u>{row["Requirement_BH"]}</u><br/>");
                sb.Append($"<i>{row["Requirement"]}</i>");
                sb.Append("</td>");

                // Specification column - input fields for this measurement
                sb.Append("<td class='specification' style='padding:0;'>");
                sb.Append("<table style='width:100%; border-collapse: collapse; font-size:10px; border:1px solid #ccc;'>");
                sb.Append("<tr style='text-align:center;'>");

                // Get saved values for this specific measurement - parse from Remark column
                string[] savedValues = new string[columnCount];
                for (int j = 0; j < columnCount; j++) savedValues[j] = "";

                var savedRow = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                if (savedRow != null)
                {
                    string remark = savedRow["Remark"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(remark))
                    {
                        string[] values = remark.Split('|');
                        for (int j = 0; j < columnCount && j < values.Length; j++)
                        {
                            savedValues[j] = values[j];
                        }
                    }
                }

                // Create input fields dynamically
                for (int j = 0; j < columnCount; j++)
                {
                    sb.Append($"<td style='border:1px solid #ccc; width:{columnWidth}%;'><input type='text' name='spec_value_{columnHeaders[j]}_{clauseID}' value='{HttpUtility.HtmlEncode(savedValues[j])}' style='width:90%; text-align:center;' /></td>");
                }

                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");

                // Result column
                sb.Append("<td class='result'>");
                sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                sb.Append($"<input type='hidden' name='columncount_{clauseID}' value='{columnCount}' />");
                sb.Append($"<input type='hidden' name='columnheaders_{clauseID}' value='{string.Join(",", columnHeaders)}' />");
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
                            // Get column configuration from hidden fields
                            string columnCountStr = Request.Form[$"columncount_{clauseID}"] ?? "3";
                            string columnHeadersStr = Request.Form[$"columnheaders_{clauseID}"] ?? "A,B,C";

                            int columnCount = int.Parse(columnCountStr);
                            string[] columnHeaders = columnHeadersStr.Split(',');

                            // Collect values from form fields
                            List<string> values = new List<string>();
                            for (int j = 0; j < columnCount; j++)
                            {
                                string fieldName = $"spec_value_{columnHeaders[j]}_{clauseID}";
                                string value = Request.Form[fieldName] ?? "";
                                values.Add(value);
                            }

                            // Combine values with pipe separator - this goes to Remark column
                            specification = string.Join("|", values);
                            specification_BH = specification;
                        }
                        else
                        {
                            // For regular rows, use normal specification fields
                            specification = Request.Form[$"specification_{clauseID}"] ?? "";
                            specification_BH = Request.Form[$"specification_BH{clauseID}"] ?? "";
                        }

                        // Get result value
                        string result = "";

                        if (inputType == "checkbox")
                        {
                            string resultOptions = row["ResultOptions"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(resultOptions))
                            {
                                string[] options = resultOptions.Split(',');
                                List<string> selectedOptions = new List<string>();
                                foreach (var option in options)
                                {
                                    if (Request.Form[$"chk_{clauseID}_{option.Trim()}"] != null)
                                        selectedOptions.Add(option.Trim());
                                }
                                result = string.Join(",", selectedOptions);
                            }
                        }
                        else
                        {
                            result = Request.Form[$"result_{clauseID}"] ?? "";
                        }

                        // Determine record type
                        string recordType = isHeader ? "header" : inputType;

                        // Save to database
                        int result0 = _cm.TestResultsAdd_EA(
                            clauseID,
                            txtstartingdate.Text,
                            txtstartingdate.Text,
                            result,
                            specification,  // Contains pipe-separated values for checkup results
                            specification_BH,
                            sampleId,
                            sampleIdSeries,
                            recordType,
                            language,
                            ReportID,
                            reportParamMapID,
                            equipmentIdsCSV,
                            txtworkorder.Text,
                            lblReportName.Text,
                            txtsampleCode.Text,
                            txtTestEquipmentNo.Text,
                            lblstandardID.Text,
                            txtstandard.Text,
                            txtstandard.Text,
                            txttemp.Text,
                            txthumidity.Text,
                            txtTestCategory.Text,
                            txtRanking.Text,
                            txtsampleCode.Text,
                            txtRequestNo.Text,
                            txtTestEquipmentNo.Text);

                        if (result0 > 0)
                        {
                            b1 = true;
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
                                    //process.Kill();
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

        /* With Border of Sub Table */
        private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

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

            // Calculate total rows more accurately
            int totalRows = 1; // Start with header row
            bool needDescHeader = false;
            List<DataRow> currentCheckUpGroup = new List<DataRow>();
            string currentGroupType = "";

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                string requirement = row["Requirement"]?.ToString() ?? "";

                if (isHeader)
                {
                    // Process any pending group before handling header
                    if (currentCheckUpGroup.Count > 0)
                    {
                        totalRows += currentCheckUpGroup.Count;
                        currentCheckUpGroup.Clear();
                        currentGroupType = "";
                    }

                    totalRows++;
                    needDescHeader = false;
                }
                else if (isDescription)
                {
                    // Process any pending group before handling description
                    if (currentCheckUpGroup.Count > 0)
                    {
                        totalRows += currentCheckUpGroup.Count;
                        currentCheckUpGroup.Clear();
                        currentGroupType = "";
                    }

                    if (!needDescHeader)
                    {
                        totalRows++; // Description header row
                        needDescHeader = true;
                    }
                    totalRows++; // Description data row
                }
                else if (isCheckUpResult)
                {
                    if (currentCheckUpGroup.Count == 0)
                    {
                        // First row in checkup group - determine group type
                        if (requirement.Contains("Number of screwless terminals"))
                        {
                            currentGroupType = "SCREWLESS_TERMINALS";
                        }
                        else if (requirement.Contains("Terminal temperature rise"))
                        {
                            currentGroupType = "COMPLETE_SPECIMENS";
                        }
                        else if (requirement.Contains("temperature rise should not exceed"))
                        {
                            currentGroupType = "TEMPERATURE";
                        }
                        else
                        {
                            currentGroupType = "OTHER";
                        }
                    }
                    currentCheckUpGroup.Add(row);
                }
                else
                {// Process any pending group before handling regular row
                    if (currentCheckUpGroup.Count > 0)
                    {
                        totalRows += currentCheckUpGroup.Count;
                        currentCheckUpGroup.Clear();
                        currentGroupType = "";
                    }
                    totalRows++;
                    needDescHeader = false;
                }
            }

            // Add any remaining group at the end (but don't double count)
            if (currentCheckUpGroup.Count > 0)
            {
                totalRows += currentCheckUpGroup.Count;
            }
            // Create table with exact row count
            Table table = doc.Tables.Add(range, totalRows, 4);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);
            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            for (int c = 1; c <= 4; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            // Set up header row
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

            // Populate table data
            int r = 2;
            bool descHeaderShown = false;
            List<DataRow> currentGroup = new List<DataRow>();
            string groupType = "";

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                string requirement = row["Requirement"]?.ToString() ?? "";

                // Process existing group if we encounter a non-checkup row
                if ((isHeader || isDescription || !isCheckUpResult) && currentGroup.Count > 0)
                {
                    CreateCheckUpSection(table, r, currentGroup, groupType, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    r += currentGroup.Count;
                    currentGroup.Clear();
                    groupType = "";
                }

                if (isHeader)
                {
                    descHeaderShown = false;
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    string headerResult = GetActualResult(row);
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, headerResult, true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    // Merge cells for header row
                    if (r <= table.Rows.Count)
                    {
                        table.Cell(r, 2).Merge(table.Cell(r, 3));
                    }
                    r++;
                }
                else if (isCheckUpResult)
                {
                    if (currentGroup.Count == 0)
                    {// First row in group - determine group type
                        if (requirement.Contains("Number of screwless terminals"))
                        {
                            groupType = "SCREWLESS_TERMINALS";
                        }
                        else if (requirement.Contains("Terminal temperature rise"))
                        {
                            groupType = "COMPLETE_SPECIMENS";
                        }
                        else if (requirement.Contains("temperature rise should not exceed"))
                        {
                            groupType = "TEMPERATURE";
                        }
                        else
                        {
                            groupType = "OTHER";
                        }
                    }
                    currentGroup.Add(row);
                }
                else if (isDescription)
                {
                    if (!descHeaderShown)
                    {
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

                        r++;
                        descHeaderShown = true;
                    }
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
                    string descriptionSpecification = GetDescriptionSpecification(row);
                    Cell a3 = table.Cell(r, 3);
                    SetCellText(a3, descriptionSpecification, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    string descriptionResult = GetActualResult(row);
                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, descriptionResult, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
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
                    string actualSpecification = GetActualSpecification(row);
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, actualSpecification, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    string actualResult = GetActualResult(row);
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, actualResult, false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
            }

            // Process any remaining checkup group at the end
            if (currentGroup.Count > 0)
            {
                CreateCheckUpSection(table, r, currentGroup, groupType, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                // Don't increment r here as we're at the end
            }
        }

        private void CreateCheckUpSection(Table table, int startRow, List<DataRow> checkUpRows, string groupType, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            if (checkUpRows.Count == 0) return;

            if (groupType == "SCREWLESS_TERMINALS")
            {
                CreateScrewlessTerminalsSection(table, startRow, checkUpRows, wordApp, columnWidths, fontSize, fontName);
            }
            else if (groupType == "COMPLETE_SPECIMENS")
            {
                CreateCompleteSpecimensSection(table, startRow, checkUpRows, wordApp, columnWidths, fontSize, fontName);
            }
            else if (groupType == "TEMPERATURE")
            {
                CreateTemperatureSection(table, startRow, checkUpRows, wordApp, columnWidths, fontSize, fontName);
            }
            else
            {
                CreateOtherCheckUpSection(table, startRow, checkUpRows, wordApp, columnWidths, fontSize, fontName);
            }
        }

        private void CreateScrewlessTerminalsSection(Table table, int startRow, List<DataRow> checkUpRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            int columnCount = 5;
            string[] columnHeaders = { "1", "2", "3", "4", "5" };
            float subColWidth = (columnWidths[2] - 0.1f) / columnCount;

            for (int i = 0; i < checkUpRows.Count; i++)
            {
                DataRow row = checkUpRows[i];
                int currentRow = startRow + i;

                Cell c1 = table.Cell(currentRow, 1);
                SetCellText(c1, row["ClauseNumber"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c2 = table.Cell(currentRow, 2);
                SetCellText(c2, row["Requirement"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c3 = table.Cell(currentRow, 3);
                Table dataSubTable = c3.Range.Tables.Add(c3.Range, 1, columnCount);
                dataSubTable.Borders.Enable = 1;
                dataSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2] - 0.1f);

                for (int j = 1; j <= columnCount; j++)
                {
                    dataSubTable.Columns[j].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
                }

                // Parse pipe-separated values from Specification and Specification_BH
                string[] specValues = ParsePipeSeparatedValues(row["Specification"]?.ToString() ?? "", columnCount);

                //string[] specBHValues = ParsePipeSeparatedValues(row["Specification_BH"]?.ToString() ?? "", columnCount);

                // For first row, show headers
                if (i == 0)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        Cell headerCell = dataSubTable.Cell(1, j + 1);
                        headerCell.Range.Text = columnHeaders[j];
                        headerCell.Range.Font.Size = fontSize;
                        headerCell.Range.Font.Name = fontName;
                        headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        headerCell.Range.Bold = 1;
                        headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        ApplyAllBorders(headerCell);
                    }
                }
                else
                {
                    // Combine values for display
                    string[] displayValues = new string[columnCount];
                    for (int j = 0; j < columnCount; j++)
                    {
                        string spec = specValues[j];

                        if (!string.IsNullOrEmpty(spec))
                            displayValues[j] = $"{spec}";
                        else if (!string.IsNullOrEmpty(spec))
                            displayValues[j] = spec;

                        else
                            displayValues[j] = "-";
                    }

                    for (int j = 0; j < columnCount; j++)
                    {
                        Cell dataCell = dataSubTable.Cell(1, j + 1);
                        dataCell.Range.Text = displayValues[j];
                        dataCell.Range.Font.Size = fontSize;
                        dataCell.Range.Font.Name = fontName;
                        dataCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        dataCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        ApplyAllBorders(dataCell);
                    }
                }
                ApplyAllBorders(c3);

                Cell c4 = table.Cell(currentRow, 4);
                SetCellText(c4, row["Result"]?.ToString() ?? "", false, fontSize, fontName);
            }
        }

        private void CreateCompleteSpecimensSection(Table table, int startRow, List<DataRow> checkUpRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            int columnCount = 3;
            string[] columnHeaders = { "A", "B", "C" };
            float subColWidth = (columnWidths[2] - 0.1f) / columnCount;

            for (int i = 0; i < checkUpRows.Count; i++)
            {
                DataRow row = checkUpRows[i];
                int currentRow = startRow + i;

                Cell c1 = table.Cell(currentRow, 1);
                SetCellText(c1, row["ClauseNumber"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c2 = table.Cell(currentRow, 2);
                SetCellText(c2, row["Requirement"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c3 = table.Cell(currentRow, 3);
                Table dataSubTable = c3.Range.Tables.Add(c3.Range, 1, columnCount);
                dataSubTable.Borders.Enable = 1;
                dataSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2] - 0.1f);

                for (int j = 1; j <= columnCount; j++)
                {
                    dataSubTable.Columns[j].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
                }

                // Parse pipe-separated values from Specification and Specification_BH
                string[] specValues = ParsePipeSeparatedValues(row["Specification"]?.ToString() ?? "", columnCount);
                //string[] specBHValues = ParsePipeSeparatedValues(row["Specification_BH"]?.ToString() ?? "", columnCount);

                // For first row, show headers
                if (i == 0)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        Cell headerCell = dataSubTable.Cell(1, j + 1);
                        headerCell.Range.Text = columnHeaders[j];
                        headerCell.Range.Font.Size = fontSize;
                        headerCell.Range.Font.Name = fontName;
                        headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        headerCell.Range.Bold = 1;
                        headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        ApplyAllBorders(headerCell);
                    }
                }
                else
                {
                    // Combine values for display
                    string[] displayValues = new string[columnCount];
                    for (int j = 0; j < columnCount; j++)
                    {
                        string spec = specValues[j];

                        if (!string.IsNullOrEmpty(spec))
                            displayValues[j] = $"{spec}";
                        else if (!string.IsNullOrEmpty(spec))
                            displayValues[j] = spec;
                        else
                            displayValues[j] = "-";
                    }

                    for (int j = 0; j < columnCount; j++)
                    {
                        Cell dataCell = dataSubTable.Cell(1, j + 1);
                        dataCell.Range.Text = displayValues[j];
                        dataCell.Range.Font.Size = fontSize;
                        dataCell.Range.Font.Name = fontName;
                        dataCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        dataCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        ApplyAllBorders(dataCell);
                    }
                }
                ApplyAllBorders(c3);

                Cell c4 = table.Cell(currentRow, 4);
                SetCellText(c4, row["Result"]?.ToString() ?? "", false, fontSize, fontName);
            }
        }

        private void CreateTemperatureSection(Table table, int startRow, List<DataRow> checkUpRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            // Find threshold row and temperature rows
            DataRow thresholdRow = null;
            List<DataRow> temperatureRows = new List<DataRow>();

            foreach (DataRow row in checkUpRows)
            {
                string requirement = row["Requirement"]?.ToString() ?? "";
                if (requirement.Contains("temperature rise should not exceed"))
                {
                    thresholdRow = row;
                }
                else
                {
                    temperatureRows.Add(row);
                }
            }

            if (temperatureRows.Count == 0 && thresholdRow == null) return;

            int columnCount = 3;
            string[] columnHeaders = { "A", "B", "C" };
            string[] thresholdValues = { " ", " ", " " };

            if (temperatureRows.Count > 0)
            {
                DataRow firstTempRow = temperatureRows[0];
                string requirement = firstTempRow["Requirement"]?.ToString() ?? "";

                if (requirement.Contains("Number of screwless terminals") ||
                    requirement.Contains("Voltage drop measured after") ||
                    requirement.Contains("Voltage drop is measured after"))
                {
                    columnCount = 5;
                    columnHeaders = new string[] { "1", "2", "3", "4", "5" };
                    thresholdValues = new string[] { "-", "-", "-", "-", "-" };
                }
            }

            float subColWidth = (columnWidths[2] - 0.1f) / columnCount;

            // Create threshold row
            if (thresholdRow != null)
            {
                Cell t1 = table.Cell(startRow, 1);
                SetCellText(t1, thresholdRow["ClauseNumber"]?.ToString() ?? "", false, fontSize, fontName);
                Cell t2 = table.Cell(startRow, 2);
                string thresholdText = thresholdRow["Requirement"]?.ToString() ?? "Number of screwless terminals";
                SetCellText(t2, thresholdText, false, fontSize, fontName);
                Cell t3 = table.Cell(startRow, 3);
                Table headerSubTable = t3.Range.Tables.Add(t3.Range, 1, columnCount);
                headerSubTable.Borders.Enable = 1;
                headerSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2] - 0.1f);

                for (int i = 1; i <= columnCount; i++)
                {
                    headerSubTable.Columns[i].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
                }

                for (int i = 0; i < columnCount; i++)
                {
                    Cell headerCell = headerSubTable.Cell(1, i + 1);
                    headerCell.Range.Text = columnHeaders[i];
                    headerCell.Range.Font.Size = fontSize;
                    headerCell.Range.Font.Name = fontName;
                    headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headerCell.Range.Bold = 1;
                    headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    ApplyAllBorders(headerCell);
                }

                ApplyAllBorders(t3);
                Cell t4 = table.Cell(startRow, 4);
                SetCellText(t4, thresholdRow["Result"]?.ToString() ?? "", false, fontSize, fontName);
                startRow++;
            }

            // Create temperature data rows
            for (int i = 0; i < temperatureRows.Count; i++)
            {
                DataRow row = temperatureRows[i];
                int currentRow = startRow + i;
                Cell c1 = table.Cell(currentRow, 1);
                SetCellText(c1, row["ClauseNumber"]?.ToString() ?? "", false, fontSize, fontName);
                Cell c2 = table.Cell(currentRow, 2);
                SetCellText(c2, row["Requirement"]?.ToString() ?? "", false, fontSize, fontName);
                Cell c3 = table.Cell(currentRow, 3);
                Table dataSubTable = c3.Range.Tables.Add(c3.Range, 1, columnCount);
                dataSubTable.Borders.Enable = 1;
                dataSubTable.PreferredWidth = wordApp.InchesToPoints(columnWidths[2] - 0.1f);

                for (int j = 1; j <= columnCount; j++)
                {
                    dataSubTable.Columns[j].SetWidth(wordApp.InchesToPoints(subColWidth), WdRulerStyle.wdAdjustNone);
                }

                string[] tempValues = new string[columnCount];
                for (int j = 0; j < columnCount; j++)
                    tempValues[j] = "-";

                if (!string.IsNullOrEmpty(row["Specification"]?.ToString()))
                {
                    string spec = row["Specification"].ToString();
                    if (spec.Contains("|"))
                    {
                        string[] values = spec.Split('|');
                        for (int j = 0; j < Math.Min(columnCount, values.Length); j++)
                        {
                            tempValues[j] = values[j].Trim();
                        }
                    }
                    else
                    {
                        tempValues[0] = spec;
                    }
                }

                for (int j = 0; j < columnCount; j++)
                {
                    Cell dataCell = dataSubTable.Cell(1, j + 1);
                    dataCell.Range.Text = tempValues[j];
                    dataCell.Range.Font.Size = fontSize;
                    dataCell.Range.Font.Name = fontName;
                    dataCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    dataCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    ApplyAllBorders(dataCell);
                }
                ApplyAllBorders(c3);
                Cell c4 = table.Cell(currentRow, 4);
                SetCellText(c4, row["Result"]?.ToString() ?? "", false, fontSize, fontName);
            }
        }

        private void CreateOtherCheckUpSection(Table table, int startRow, List<DataRow> checkUpRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            // Handle other types of checkup results
            for (int i = 0; i < checkUpRows.Count; i++)
            {
                DataRow row = checkUpRows[i];
                int currentRow = startRow + i;

                Cell c1 = table.Cell(currentRow, 1);
                SetCellText(c1, row["ClauseNumber"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c2 = table.Cell(currentRow, 2);
                SetCellText(c2, row["Requirement"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c3 = table.Cell(currentRow, 3);
                SetCellText(c3, row["Specification"]?.ToString() ?? "", false, fontSize, fontName);

                Cell c4 = table.Cell(currentRow, 4);
                SetCellText(c4, row["Result"]?.ToString() ?? "", false, fontSize, fontName);
            }
        }

        private string[] ParsePipeSeparatedValues(string input, int expectedCount)
        {
            string[] values = new string[expectedCount];

            if (string.IsNullOrEmpty(input))
            {
                for (int i = 0; i < expectedCount; i++)
                    values[i] = "";
                return values;
            }

            string[] parts = input.Split('|');

            for (int i = 0; i < expectedCount; i++)
            {
                if (i < parts.Length)
                    values[i] = parts[i].Trim();
                else
                    values[i] = "";
            }

            return values;
        }

        private string GetActualResult(DataRow row)
        {
            if (row.Table.Columns.Contains("Result") && !string.IsNullOrEmpty(row["Result"]?.ToString()))
            {
                return row["Result"].ToString();
            }
            return "";
        }

        private string GetActualSpecification(DataRow row)
        {
            if (row.Table.Columns.Contains("Specification") && !string.IsNullOrEmpty(row["Specification"]?.ToString()))
            {
                return row["Specification"].ToString();
            }
            return "";
        }

        private string GetDescriptionSpecification(DataRow row)
        {
            if (row.Table.Columns.Contains("Specification") && !string.IsNullOrEmpty(row["Specification"]?.ToString()))
            {
                return row["Specification"].ToString();
            }
            return "";
        }

        private void ApplyBorders(Cell cell)
        {
            cell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
        }

        private void ApplyAllBorders(Cell cell)
        {
            cell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

            cell.Borders[WdBorderType.wdBorderLeft].LineWidth = WdLineWidth.wdLineWidth050pt;
            cell.Borders[WdBorderType.wdBorderRight].LineWidth = WdLineWidth.wdLineWidth050pt;
            cell.Borders[WdBorderType.wdBorderTop].LineWidth = WdLineWidth.wdLineWidth050pt;
            cell.Borders[WdBorderType.wdBorderBottom].LineWidth = WdLineWidth.wdLineWidth050pt;
        }

        private void ApplySubTableVerticalBorders(Table subTable)
        {
            subTable.Borders.Enable = 1;
            foreach (Row subRow in subTable.Rows)
            {
                foreach (Cell cell in subRow.Cells)
                {
                    ApplyAllBorders(cell);
                }
            }
        }

        private void SetCellText(Cell cell, string text, bool bold, float fontSize, string fontName)
        {
            cell.Range.Text = text;
            cell.Range.Bold = bold ? 1 : 0;
            cell.Range.Font.Size = fontSize;
            cell.Range.Font.Name = fontName;
            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyAllBorders(cell);
        }


    }
}