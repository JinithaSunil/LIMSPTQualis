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
    public partial class InsulationResistanceAndElectricStrength : System.Web.UI.Page
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

            DataTable snm = _cm.GetSampleName_EA(cloneCRFSampleID);
            if (snm != null && snm.Rows.Count > 0)
            {
                string sampleName = snm.Rows[0]["SampleName"].ToString();
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
                txthumidity.Text = saved.Rows[0]["Humidity"]?.ToString() ?? "";
                txttemp.Text = saved.Rows[0]["Temperature"]?.ToString() ?? "";
                txtRanking.Text = saved.Rows[0]["Ranking"]?.ToString() ?? "";
                txtsampleCode.Text = saved.Rows[0]["SampleCode"]?.ToString() ?? "";
                txtTestEquipmentNo.Text = saved.Rows[0]["TestEquipmentNumber"]?.ToString() ?? "";

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestDate"]?.ToString()))
                {
                    if (DateTime.TryParse(saved.Rows[0]["TestDate"].ToString(), out DateTime testDate))
                        txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");
                }
            }

            DataTable dtCategoryTypeStdTest = _cm.GetCategoryTypeStdTest(lblSampleID.Text);
            if (dtCategoryTypeStdTest != null && dtCategoryTypeStdTest.Rows.Count > 0)
                txtTestCategory.Text = dtCategoryTypeStdTest.Rows[0]["TestCategory"]?.ToString() ?? "";

            DataTable DisResget = _cm.GetTestResultLED_DISTINCT(lblSampleID.Text, lblReportID.Text);
            if (DisResget != null && DisResget.Rows.Count > 0)
            {
                string equipementIDGet = DisResget.Rows[0]["EquipmentIds"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(equipementIDGet))
                {
                    string[] selectedEquipmentIds = equipementIDGet.Split(',').Select(x => x.Trim()).ToArray();
                    foreach (ListItem item in chkEquipmentList.Items)
                        if (selectedEquipmentIds.Contains(item.Value)) item.Selected = true;
                }
            }

            int reportID = Convert.ToInt32(lblReportID.Text);
            string standardID = lblstandardID.Text;

            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_EA(sampleId, reportID, standardID);
            if (dt1 == null || dt1.Rows.Count == 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Record Not Found!');</script>");
                return;
            }

            txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"]?.ToString() ?? "";
            lblReportName.Text = dt1.Rows[0]["ReportName"]?.ToString() ?? "";
            string sis = dts.Rows[0]["sampleIDSeries"]?.ToString() ?? "";
            string requestNo = "";
            if (!string.IsNullOrEmpty(sis) && sis.Contains("/"))
            {
                string[] parts = sis.Split('/');
                if (parts.Length > 1) requestNo = "R/" + parts[1];
            }
            txtRequestNo.Text = requestNo;
            ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"]?.ToString() ?? "";

            string[] ids = dt1.Rows[0]["StandardID"]?.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            List<string> standardIdList = ids.Select(x => x.Trim()).ToList();

            DataTable filteredClauseTable = null;
            if (standardIdList.Count > 0)
            {
                var matchingRows = dt.AsEnumerable().Where(r =>
                {
                    string clauseStandardIds = r["Standard_ID"]?.ToString() ?? "";
                    string[] clauseIds = clauseStandardIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    return clauseIds.Any(cid => standardIdList.Contains(cid));
                });
                if (matchingRows.Any()) filteredClauseTable = matchingRows.CopyToDataTable();
            }

            if (filteredClauseTable == null || filteredClauseTable.Rows.Count == 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('No matching clauses found!');</script>");
                return;
            }

            var sb = new StringBuilder();
            bool descriptionHeaderShown = false;

            // Outer table (980px total, using colgroup to force widths)
            sb.Append("<table style='width:1200px; table-layout:fixed; border-collapse:collapse; font-family:Arial, sans-serif; font-size:12px; margin:0;'>");
            sb.Append("<colgroup>");
            sb.Append("<col style='width:80px;' />");   // Clause
            sb.Append("<col style='width:500px;' />");  // Requirement
            sb.Append("<col style='width:500px;' />");  // Check-up
            sb.Append("<col style='width:120px;' />");  // Decision
            sb.Append("</colgroup>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='border:1px solid #000; padding:6px; text-align:center; box-sizing:border-box;'>Clause</th>");
            sb.Append("<th style='border:1px solid #000; padding:6px; text-align:center; box-sizing:border-box;'>Test Requirements</th>");
            sb.Append("<th style='border:1px solid #000; padding:6px; text-align:center; box-sizing:border-box;'>Check up result</th>");
            sb.Append("<th style='border:1px solid #000; padding:6px; text-align:center; box-sizing:border-box;'>Decision</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");

            bool inCheckUpResultGroup = false;
            List<DataRow> checkUpResultRows = new List<DataRow>();
            bool inDiagramGroup = false;
            List<DataRow> diagramRows = new List<DataRow>();
            string currentDiagramFileName = "";
            int diagramStartIndex = 0;

            foreach (DataRow row in filteredClauseTable.Rows)
            {
                bool isHeader = row["isHeader"]?.ToString() == "1";
                bool isDescription = row["isDescription"]?.ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"]?.ToString() == "1";
                bool isDiagram = row["isDiagram"]?.ToString() == "1";

                if ((isHeader || !isCheckUpResult) && inCheckUpResultGroup && checkUpResultRows.Count > 0)
                {
                    ProcessCheckUpResultGroup(sb, checkUpResultRows, saved);
                    checkUpResultRows.Clear();
                    inCheckUpResultGroup = false;
                }

                if ((isHeader || !isDiagram) && inDiagramGroup && diagramRows.Count > 0)
                {
                    ProcessDiagramGroup(sb, diagramRows, saved, diagramStartIndex);
                    diagramRows.Clear();
                    inDiagramGroup = false;
                    currentDiagramFileName = "";
                }

                if (isHeader)
                {
                    descriptionHeaderShown = false;
                    int clauseID = Convert.ToInt32(row["ClauseID"]);
                    string inputType = GetInputType(row);

                    sb.Append("<tr>");
                    sb.Append($"<td style='border:1px solid #000; padding:6px; vertical-align:middle; text-align:center; box-sizing:border-box; overflow:hidden; white-space:nowrap; text-overflow:ellipsis;'><b>{HttpUtility.HtmlEncode(row["ClauseNumber"]?.ToString() ?? "")}</b></td>");
                    sb.Append("<td colspan='2' style='border:1px solid #000; padding:6px; vertical-align:middle; box-sizing:border-box; overflow:hidden;'>");
                    sb.Append($"<u>{HttpUtility.HtmlEncode(row["ClauseTitle_BH"]?.ToString() ?? "")}</u><br/><i>{HttpUtility.HtmlEncode(row["ClauseTitle"]?.ToString() ?? "")}</i>");
                    sb.Append("</td>");
                    sb.Append("<td style='border:1px solid #000; padding:6px; vertical-align:middle; box-sizing:border-box;'>");
                    sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{HttpUtility.HtmlEncode(inputType)}' />");
                    string savedResult = "";
                    var savedRowForResult = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
                    if (savedRowForResult != null) savedResult = savedRowForResult["Result"]?.ToString() ?? "";
                    sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; font-size:11px;' />");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
                else if (isDiagram)
                {
                    string fileName = row["FileName"]?.ToString() ?? "";
                    if (!inDiagramGroup || fileName != currentDiagramFileName)
                    {
                        if (inDiagramGroup && diagramRows.Count > 0)
                        {
                            ProcessDiagramGroup(sb, diagramRows, saved, diagramStartIndex);
                            diagramRows.Clear();
                        }
                        inDiagramGroup = true;
                        currentDiagramFileName = fileName;
                        diagramStartIndex = diagramRows.Count;
                    }
                    diagramRows.Add(row);
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
                    ProcessRegularRow(sb, row, saved, ref descriptionHeaderShown);
                }
            }

            if (inCheckUpResultGroup && checkUpResultRows.Count > 0) ProcessCheckUpResultGroup(sb, checkUpResultRows, saved);
            if (inDiagramGroup && diagramRows.Count > 0) ProcessDiagramGroup(sb, diagramRows, saved, diagramStartIndex);

            sb.Append("</tbody>");
            sb.Append("</table>");

            clauseContainer.InnerHtml = sb.ToString();
        }

        private string CreateDiagramTable(List<DataRow> rows, string fileName)
        {
            var sb = new StringBuilder();

            sb.Append("<div style='width:100%; padding:0; margin:0; box-sizing:border-box;'>");
            sb.Append("<table style='width:100%; table-layout:fixed; border-collapse:collapse; margin:0; font-size:11px; box-sizing:border-box;'>");

            sb.Append("<colgroup>");
            sb.Append("<col style='width:15%;' />");   // Diagram
            sb.Append("<col style='width:10%;' />");   // Position
            sb.Append("<col style='width:15%;' />");   // Terminal
            sb.Append("<col style='width:14%;' />");   // Body (B)
            sb.Append("<col style='width:36%;' />");
            sb.Append("</colgroup>");

            sb.Append("<thead><tr>");
            sb.Append("<th style='border:1px solid #000; padding:4px; text-align:center; white-space:normal;'>Diagram<br/>Connection</th>");
            sb.Append("<th style='border:1px solid #000; padding:4px; text-align:center; white-space:normal;'>Position</th>");
            sb.Append("<th style='border:1px solid #000; padding:4px; text-align:center; white-space:normal;'>Voltage Application<br/>Between terminal</th>");
            sb.Append("<th style='border:1px solid #000; padding:4px; text-align:center; white-space:normal;'>Voltage Application<br/>Between body (B)</th>");
            sb.Append("<th style='display:none;'></th>");
            sb.Append("</tr></thead>");

            sb.Append("<tbody>");

            if (rows == null || rows.Count == 0)
            {
                sb.Append("</tbody></table></div>");
                return sb.ToString();
            }

            sb.Append("<tr>");
            sb.Append($"<td rowspan='{rows.Count}' style='border:1px solid #000; padding:4px; text-align:center; vertical-align:middle; box-sizing:border-box;'>");

            if (!string.IsNullOrEmpty(fileName))
            {
                sb.Append($"<img src='SampleDiagram/{fileName}' style='max-width:125px; max-height:101px;' />");
                sb.Append("<div style='font-size:9px;'>Diagram</div>");
            }
            sb.Append("</td>");

            DataRow first = rows[0];

            sb.Append(Cell(first["Position"]));
            sb.Append(Cell(first["TerminalNumber"]));
            sb.Append(Cell(first["TheBodiesBTogetherWithTerminalNumber"]));
            sb.Append("</tr>");

            for (int i = 1; i < rows.Count; i++)
            {
                DataRow r = rows[i];
                sb.Append("<tr>");
                sb.Append(Cell(r["Position"]));
                sb.Append(Cell(r["TerminalNumber"]));
                sb.Append(Cell(r["TheBodiesBTogetherWithTerminalNumber"]));
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table></div>");

            return sb.ToString();
        }

        private string Cell(object data)
        {
            return $"<td style='border:1px solid #000; padding:4px; text-align:center; vertical-align:middle; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;'>{HttpUtility.HtmlEncode(data?.ToString() ?? "")}</td>";
        }

        private void ProcessDiagramGroup(StringBuilder sb, List<DataRow> rows, DataTable saved, int startIndex)
        {
            if (rows == null || rows.Count == 0) return;
            DataRow firstRow = rows[0];
            string fileName = firstRow["FileName"]?.ToString() ?? "";

            sb.Append("<tr>");
            sb.Append($"<td style='width:70px; border:1px solid #000; padding:6px; vertical-align:middle; text-align:center; box-sizing:border-box; overflow:hidden; white-space:nowrap; text-overflow:ellipsis;'>{HttpUtility.HtmlEncode(firstRow["ClauseNumber"]?.ToString() ?? "")}</td>");

            // Important: padding:0 here so nested table width:100% exactly fits this cell
            sb.Append("<td style='width:560px; border:1px solid #000; padding:0; vertical-align:top; box-sizing:border-box; overflow:hidden;'>");
            sb.Append(CreateDiagramTable(rows, fileName));
            sb.Append("</td>");

            sb.Append("<td style='width:230px; border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box; overflow:hidden;'>");
            foreach (DataRow row in rows)
            {
                int clauseID = Convert.ToInt32(row["ClauseID"]);
                string spec = row["Specification"]?.ToString() ?? "";
                string savedSpec = "";
                var savedRowForSpec = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
                if (savedRowForSpec != null) savedSpec = savedRowForSpec["Remark"]?.ToString() ?? "";
                string display = !string.IsNullOrEmpty(savedSpec) ? savedSpec : spec;
                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(display)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; margin-bottom:6px; font-size:11px; overflow:hidden; white-space:nowrap; text-overflow:ellipsis;' />");
            }
            sb.Append("</td>");

            sb.Append("<td style='width:120px; border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box; overflow:hidden;'>");
            foreach (DataRow row in rows)
            {
                int clauseID = Convert.ToInt32(row["ClauseID"]);
                sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{HttpUtility.HtmlEncode(GetInputType(row))}' />");
                string savedResult = "";
                var savedRowForResult = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
                if (savedRowForResult != null) savedResult = savedRowForResult["Result"]?.ToString() ?? "";
                sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; margin-bottom:6px; font-size:11px; overflow:hidden; white-space:nowrap; text-overflow:ellipsis;' />");
            }
            sb.Append("</td>");

            sb.Append("</tr>");
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
            // Check for 3-column with numeric headers - "Terminal temperature rise:"
            else if (requirement.Contains("Terminal temperature rise:") || requirement_BH.Contains("Kenaikan suhu terminal:"))
            {
                columnCount = 3;
                columnHeaders = new string[] { "A", "B", "C" };
                thresholdValues = new string[] { "", "", "" };
            }
            // Default: 3 columns A, B, C
            else
            {
                columnCount = 3;
                columnHeaders = new string[] { "A", "B", "C" };
                thresholdValues = new string[] { "", "", "" };
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
            sb.Append("<table style='width:100%; min-width: 500px !important; border-collapse: collapse; font-size:10px; border:1px solid #ccc;'>");

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
                sb.Append("<table style='width:100%; min-width: 500px !important; border-collapse: collapse; font-size:10px; border:1px solid #ccc;'>");
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
                    sb.Append($"<td style='border:1px solid #ccc; width:{columnWidth}%;'><input type='text' name='spec_value_{columnHeaders[j]}_{clauseID}' value='{HttpUtility.HtmlEncode(savedValues[j])}' style='width:98%; text-align:center;' /></td>");
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
                sb.Append("<td style='border:1px solid #000; padding:6px; box-sizing:border-box;'></td>");
                sb.Append("<td style='border:1px solid #000; padding:0; box-sizing:border-box;'>");
                sb.Append("<table style='width:100%; border-collapse:collapse; margin:0;'><tr style='background:#f0f0f0; font-weight:bold;'><td style='padding:6px; border-right:1px solid #000;'>Description</td><td style='padding:6px; text-align:center;'>Condition<br/>(mm)</td></tr></table>");
                sb.Append("</td>");
                sb.Append("<td style='border:1px solid #000; padding:6px; box-sizing:border-box;'></td>");
                sb.Append("<td style='border:1px solid #000; padding:6px; box-sizing:border-box;'></td>");
                sb.Append("</tr>");
                descriptionHeaderShown = true;
            }

            sb.Append("<tr>");
            sb.Append($"<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>{HttpUtility.HtmlEncode(row["ClauseNumber"]?.ToString() ?? "")}</td>");
            sb.Append("<td style='border:1px solid #000; padding:0; box-sizing:border-box; vertical-align:top;'>");
            sb.Append("<table style='width:100%; border-collapse:collapse; margin:0;'><tr>");
            sb.Append("<td style='padding:8px; border-right:1px solid #000; vertical-align:top;'>");
            sb.Append($"<u>{HttpUtility.HtmlEncode(row["Description_BH"]?.ToString() ?? "")}</u><br/><i>{HttpUtility.HtmlEncode(row["Description"]?.ToString() ?? "")}</i>");
            sb.Append("</td>");
            sb.Append("<td style='padding:8px; text-align:center; vertical-align:middle;'>");
            sb.Append($"{HttpUtility.HtmlEncode(row["Condition"]?.ToString() ?? "")}");
            sb.Append("</td>");
            sb.Append("</tr></table>");
            sb.Append("</td>");

            sb.Append("<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>");
            string specification = row["Specification"]?.ToString() ?? "";
            string specification_BH = row["Specification_BH"]?.ToString() ?? "";
            string savedSpecification = "", savedSpecification_BH = "";
            var savedRowForSpec = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
            if (savedRowForSpec != null)
            {
                savedSpecification = savedRowForSpec["Remark"]?.ToString() ?? "";
                savedSpecification_BH = savedRowForSpec["Remark_BH"]?.ToString() ?? "";
            }
            string displaySpecification = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
            string displaySpecification_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;
            if (!string.IsNullOrEmpty(displaySpecification_BH))
                sb.Append($"<input type='text' name='specification_BH_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification_BH)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; margin-bottom:6px; font-size:11px;' />");
            if (!string.IsNullOrEmpty(displaySpecification))
                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; font-size:11px;' />");
            sb.Append("</td>");

            sb.Append("<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>");
            sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{HttpUtility.HtmlEncode(inputType)}' />");
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
            sb.Append($"<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>{HttpUtility.HtmlEncode(row["ClauseNumber"]?.ToString() ?? "")}</td>");
            sb.Append("<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>");
            sb.Append($"<u>{HttpUtility.HtmlEncode(row["Requirement_BH"]?.ToString() ?? "")}</u><br/><i>{HttpUtility.HtmlEncode(row["Requirement"]?.ToString() ?? "")}</i>");
            sb.Append("</td>");

            sb.Append("<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>");
            string specification = row["Specification"]?.ToString() ?? "";
            string specification_BH = row["Specification_BH"]?.ToString() ?? "";
            string savedSpecification = "", savedSpecification_BH = "";
            var savedRowForSpec = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
            if (savedRowForSpec != null) savedSpecification = savedRowForSpec["Specification"]?.ToString() ?? "";
            string displaySpecification = !string.IsNullOrEmpty(savedSpecification) ? savedSpecification : specification;
            string displaySpecification_BH = !string.IsNullOrEmpty(savedSpecification_BH) ? savedSpecification_BH : specification_BH;
            if (!string.IsNullOrEmpty(displaySpecification_BH))
                sb.Append($"<input type='text' name='specification_BH_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification_BH)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; margin-bottom:6px; font-size:11px;' />");
            if (!string.IsNullOrEmpty(displaySpecification))
                sb.Append($"<input type='text' name='specification_{clauseID}' value='{HttpUtility.HtmlEncode(displaySpecification)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; font-size:11px;' />");
            sb.Append("</td>");

            sb.Append("<td style='border:1px solid #000; padding:6px; vertical-align:top; box-sizing:border-box;'>");
            sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{HttpUtility.HtmlEncode(inputType)}' />");
            GenerateResultInput(sb, row, saved, clauseID);
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        private string GetInputType(DataRow row)
        {
            if (row["isHeader"] != null && row["isHeader"].ToString() == "1") return "header";
            if (row["isDescription"] != null && row["isDescription"].ToString() == "1") return "description";
            if (row["isCheckUpResult"] != null && row["isCheckUpResult"].ToString() == "1") return "checkup";
            if (row["isDiagram"] != null && row["isDiagram"].ToString() == "1") return "diagram";
            return "regular";
        }

        private void GenerateResultInput(StringBuilder sb, DataRow row, DataTable saved, int clauseID)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
            {
                string[] options = resultOptions.Split(',');
                var savedRows = saved?.Select($"ClauseID={clauseID}");
                List<string> savedValues = new List<string>();
                if (savedRows != null)
                {
                    foreach (var s in savedRows)
                    {
                        string savedResult = s["Result"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(savedResult)) savedValues.AddRange(savedResult.Split(',').Select(x => x.Trim()));
                    }
                }

                sb.Append("<div style='width:100%;'>");
                for (int i = 0; i < options.Length; i++)
                {
                    string optValue = options[i].Trim();
                    string isChecked = savedValues.Contains(optValue) ? "checked='checked'" : "";
                    if (i % 3 == 0) sb.Append("<div style='display:flex; margin-bottom:4px;'>");
                    sb.Append($"<label style='flex:1; font-size:10px; white-space:nowrap;'><input type='checkbox' name='chk_{clauseID}_{HttpUtility.HtmlEncode(optValue)}' value='{HttpUtility.HtmlEncode(optValue)}' {isChecked} /> {HttpUtility.HtmlEncode(optValue)}</label>");
                    if ((i + 1) % 3 == 0 || i == options.Length - 1) sb.Append("</div>");
                }
                sb.Append("</div>");
            }
            else
            {
                var savedRow = saved?.Select($"ClauseID={clauseID}").FirstOrDefault();
                string savedResult = savedRow != null ? savedRow["Result"]?.ToString() ?? "" : "";
                sb.Append($"<input type='text' name='result_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='display:block; width:calc(100% - 8px); box-sizing:border-box; height:22px; font-size:11px;' />");
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
                        int result0 = _cm.TestResultsAdd_EA(clauseID, txtstartingdate.Text, txtstartingdate.Text, result, specification, specification_BH,
                            sampleId, sampleIdSeries, recordType, language, ReportID, reportParamMapID, equipmentIdsCSV,
                            txtworkorder.Text, lblReportName.Text, txtsampleCode.Text, txtTestEquipmentNo.Text, lblstandardID.Text,
                            txtstandard.Text, txtstandard.Text, txttemp.Text, txthumidity.Text, txtTestCategory.Text, txtRanking.Text,
                            txtsampleCode.Text, txtRequestNo.Text, txtTestEquipmentNo.Text);

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

            // Add required columns - UPDATED WITH DIAGRAM FIELDS
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
            dt.Columns.Add("isCheckUpResult", typeof(string));

            // ADD THESE FOR DIAGRAM SUPPORT
            dt.Columns.Add("isDiagram", typeof(string));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("TerminalNumber", typeof(string));
            dt.Columns.Add("TheBodiesBTogetherWithTerminalNumber", typeof(string));

            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);
            string standardID = lblstandardID.Text;

            // Get clause data
            DataTable clauseData = GetClauseData();
            // Get saved results
            DataTable savedResults = GetSavedResults();

            string[] ids = standardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> standardIdList = ids.Select(id => id.Trim()).ToList();

            if (standardIdList.Count > 0 && clauseData != null && clauseData.Rows.Count > 0)
            {
                var matchingRows = clauseData.AsEnumerable().Where(r =>
                {
                    if (r["Standard_ID"] == DBNull.Value) return false;

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

                        // Basic clause information
                        newRow["ClauseID"] = clauseID;
                        newRow["ClauseNumber"] = clauseRow["ClauseNumber"]?.ToString() ?? "";
                        newRow["ClauseTitle"] = clauseRow["ClauseTitle"]?.ToString() ?? "";
                        newRow["Requirement"] = clauseRow["Requirement"]?.ToString() ?? "";
                        newRow["Description"] = clauseRow["Description"]?.ToString() ?? "";
                        newRow["Condition"] = clauseRow["Condition"]?.ToString() ?? "";
                        newRow["isHeader"] = clauseRow["isHeader"]?.ToString() ?? "0";
                        newRow["isDescription"] = clauseRow["isDescription"]?.ToString() ?? "0";

                        // IMPORTANT: Populate the isCheckUpResult column
                        if (clauseRow.Table.Columns.Contains("isCheckUpResult"))
                        {
                            newRow["isCheckUpResult"] = clauseRow["isCheckUpResult"]?.ToString() ?? "0";
                        }
                        else
                        {
                            newRow["isCheckUpResult"] = "0";
                        }

                        // CRITICAL: Populate diagram-related columns
                        if (clauseRow.Table.Columns.Contains("isDiagram"))
                        {
                            newRow["isDiagram"] = clauseRow["isDiagram"]?.ToString() ?? "0";
                        }
                        else
                        {
                            newRow["isDiagram"] = "0";
                        }

                        if (clauseRow.Table.Columns.Contains("FileName"))
                        {
                            newRow["FileName"] = clauseRow["FileName"]?.ToString() ?? "";
                        }
                        else
                        {
                            newRow["FileName"] = "";
                        }

                        if (clauseRow.Table.Columns.Contains("Position"))
                        {
                            newRow["Position"] = clauseRow["Position"]?.ToString() ?? "";
                        }
                        else
                        {
                            newRow["Position"] = "";
                        }

                        if (clauseRow.Table.Columns.Contains("TerminalNumber"))
                        {
                            newRow["TerminalNumber"] = clauseRow["TerminalNumber"]?.ToString() ?? "";
                        }
                        else
                        {
                            newRow["TerminalNumber"] = "";
                        }

                        if (clauseRow.Table.Columns.Contains("TheBodiesBTogetherWithTerminalNumber"))
                        {
                            newRow["TheBodiesBTogetherWithTerminalNumber"] = clauseRow["TheBodiesBTogetherWithTerminalNumber"]?.ToString() ?? "";
                        }
                        else
                        {
                            newRow["TheBodiesBTogetherWithTerminalNumber"] = "";
                        }

                        // Get saved data
                        var savedRow = savedResults.AsEnumerable()
                            .FirstOrDefault(r => Convert.ToInt32(r["ClauseID"]) == clauseID);

                        if (savedRow != null)
                        {
                            // Handle different types of specifications
                            if (clauseRow["isDescription"]?.ToString() == "1")
                            {
                                newRow["Specification"] = savedRow["Remark"]?.ToString() ?? "";
                            }
                            else if (clauseRow["isCheckUpResult"]?.ToString() == "1")
                            {
                                // For checkup results, Specification might contain pipe-separated values
                                newRow["Specification"] = savedRow["Remark"]?.ToString() ?? savedRow["Specification"]?.ToString() ?? "";
                            }
                            else if (clauseRow["isDiagram"]?.ToString() == "1")
                            {
                                // For diagram rows, use Remark for specification
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
                            // No saved data, use clause data
                            if (clauseRow.Table.Columns.Contains("Specification"))
                            {
                                newRow["Specification"] = clauseRow["Specification"]?.ToString() ?? "";
                            }
                            else
                            {
                                newRow["Specification"] = "";
                            }
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
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

            float[] COLUMN_WIDTHS = { 0.60f, 3.20f, 1.90f, 0.80f };
            float SUB_COLUMN1_WIDTH = 2.1f;
            float SUB_COLUMN2_WIDTH = 0.7f;
            float TABLE_TOTAL_WIDTH = 6.5f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // Calculate total rows
            int totalRows = 1; // header
            bool needDescHeader = false;
            List<DataRow> currentCheckUpGroup = new List<DataRow>();
            string currentGroupType = "";
            bool needDiagramHeader = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                bool isDiagram = row.Table.Columns.Contains("isDiagram") && row["isDiagram"].ToString() == "1";

                if (isHeader)
                {
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
                    if (currentCheckUpGroup.Count > 0)
                    {
                        totalRows += currentCheckUpGroup.Count;
                        currentCheckUpGroup.Clear();
                        currentGroupType = "";
                    }
                    if (!needDescHeader)
                    {
                        totalRows++; // description header row
                        needDescHeader = true;
                    }
                    totalRows++; // description data row
                }
                else if (isCheckUpResult)
                {
                    if (currentCheckUpGroup.Count == 0)
                    {
                        string requirement = row["Requirement"]?.ToString() ?? "";
                        if (requirement.Contains("Number of screwless terminals"))
                            currentGroupType = "SCREWLESS_TERMINALS";
                        else if (requirement.Contains("Terminal temperature rise:"))
                            currentGroupType = "COMPLETE_SPECIMENS";
                        else if (requirement.Contains("temperature rise should not exceed"))
                            currentGroupType = "TEMPERATURE";
                        else
                            currentGroupType = "OTHER";
                    }
                    currentCheckUpGroup.Add(row);
                }
                else if (isDiagram)
                {
                    if (!needDiagramHeader)
                    {
                        totalRows++; // Add one row for diagram header
                        needDiagramHeader = true;
                    }
                    totalRows++; // Add row for diagram data
                }
                else
                {
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

            if (currentCheckUpGroup.Count > 0)
                totalRows += currentCheckUpGroup.Count;

            // Create table
            Table table = doc.Tables.Add(range, totalRows, 4);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);

            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            for (int c = 1; c <= 4; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            // Header row
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

            // Populate rows
            int r = 2;
            bool descHeaderShown = false;
            List<DataRow> currentGroup = new List<DataRow>();
            string groupType = "";
            bool diagramHeaderShown = false;

            for (int idx = 0; idx < dt.Rows.Count; idx++)
            {
                DataRow row = dt.Rows[idx];
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                bool isDiagram = row.Table.Columns.Contains("isDiagram") && row["isDiagram"].ToString() == "1";
                string requirement = row["Requirement"]?.ToString() ?? "";

                // ========== FIX: Reset diagram header flag when encountering non-diagram rows ==========
                if (!isDiagram)
                {
                    diagramHeaderShown = false; // Reset so header shows again next time isDiagram = 1
                }
                // =======================================================================================

                // flush accumulated checkup group when needed
                if ((isHeader || isDescription || (!isCheckUpResult && !isDiagram)) && currentGroup.Count > 0)
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

                    if (r <= table.Rows.Count)
                        table.Cell(r, 2).Merge(table.Cell(r, 3));

                    r++;
                }
                else if (isCheckUpResult)
                {
                    if (currentGroup.Count == 0)
                    {
                        if (requirement.Contains("Number of screwless terminals"))
                            groupType = "SCREWLESS_TERMINALS";
                        else if (requirement.Contains("Terminal temperature rise:"))
                            groupType = "COMPLETE_SPECIMENS";
                        else if (requirement.Contains("temperature rise should not exceed"))
                            groupType = "TEMPERATURE";
                        else
                            groupType = "OTHER";
                    }
                    currentGroup.Add(row);
                }
                else if (isDiagram)
                {
                    // Show diagram header if not already shown
                    if (!diagramHeaderShown)
                    {
                        CreateDiagramStaticHeader(table, r, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_NAME);
                        r++;
                        diagramHeaderShown = true;
                    }

                    // Process diagram rows
                    List<DataRow> diagramGroup = new List<DataRow>();
                    string initialFile = row.Table.Columns.Contains("FileName") ? (row["FileName"]?.ToString() ?? "") : "";
                    diagramGroup.Add(row);

                    int look = idx + 1;
                    while (look < dt.Rows.Count)
                    {
                        DataRow nextRow = dt.Rows[look];
                        bool nextIsDiagram = nextRow.Table.Columns.Contains("isDiagram") && nextRow["isDiagram"].ToString() == "1";
                        if (!nextIsDiagram) break;

                        string nextFile = nextRow.Table.Columns.Contains("FileName") ? (nextRow["FileName"]?.ToString() ?? "") : "";
                        if (string.Equals(initialFile, nextFile, StringComparison.OrdinalIgnoreCase))
                        {
                            diagramGroup.Add(nextRow);
                            look++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    CreateDiagramGroupSection(table, r, diagramGroup, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    r += diagramGroup.Count;
                    idx += (diagramGroup.Count - 1);
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
        }

        private void CreateDiagramStaticHeader(Table table, int headerRow, Application wordApp, float[] colWidths, string fontName)
        {
            float fontSize = 7;
            // Column 1: Clause Number
            Cell h1 = table.Cell(headerRow, 1);
            h1.Range.Text = "";
            h1.Range.Bold = 1;
            h1.Range.Font.Size = fontSize;
            h1.Range.Font.Name = fontName;
            h1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            h1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(h1);

            // Column 2: Create nested table for the 4 diagram columns
            Cell h2 = table.Cell(headerRow, 2);
            h2.Range.Text = "";

            // Create nested header table with 2 rows and 4 columns
            Table nestedHeader = h2.Range.Tables.Add(h2.Range, 2, 4);
            nestedHeader.Borders.Enable = 1;
            nestedHeader.Range.Font.Size = fontSize;
            nestedHeader.Range.Font.Name = fontName;

            // Set column widths
            float totalWidth = colWidths[1];
            float col1Width = 0.9f;
            float remainingWidth = totalWidth - col1Width;

            nestedHeader.Columns[1].SetWidth(wordApp.InchesToPoints(col1Width), WdRulerStyle.wdAdjustNone);
            nestedHeader.Columns[2].SetWidth(wordApp.InchesToPoints(remainingWidth * 0.25f), WdRulerStyle.wdAdjustNone);
            nestedHeader.Columns[3].SetWidth(wordApp.InchesToPoints(remainingWidth * 0.35f), WdRulerStyle.wdAdjustNone);
            nestedHeader.Columns[4].SetWidth(wordApp.InchesToPoints(remainingWidth * 0.40f), WdRulerStyle.wdAdjustNone);

            // ============ ROW 1: MAIN HEADER ============
            Cell mainHeader1 = nestedHeader.Cell(1, 1);
            mainHeader1.Range.Text = "";
            mainHeader1.Range.Bold = 1;
            mainHeader1.Range.Font.Size = fontSize;
            mainHeader1.Range.Font.Name = fontName;
            mainHeader1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            mainHeader1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(mainHeader1);

            // "Voltage Application" spanning columns 2-4
            Cell mainHeader2 = nestedHeader.Cell(1, 2);
            mainHeader2.Range.Text = "";
            mainHeader2.Range.Bold = 1;
            mainHeader2.Range.Font.Size = fontSize;
            mainHeader2.Range.Font.Name = fontName;
            mainHeader2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            mainHeader2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mainHeader2.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader2.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader2.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader2.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

            Cell mainHeader3 = nestedHeader.Cell(1, 3);
            mainHeader3.Range.Text = "";
            mainHeader3.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader3.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader3.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
            mainHeader3.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

            Cell mainHeader4 = nestedHeader.Cell(1, 4);
            mainHeader4.Range.Text = "";
            mainHeader4.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader4.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
            mainHeader4.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
            mainHeader4.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;

            // ============ ROW 2: SUB-HEADERS ============
            Cell subHeader1 = nestedHeader.Cell(2, 1);
            subHeader1.Range.Text = "Diagram\nConnection";
            ApplyBorders(subHeader1);

            Cell subHeader2 = nestedHeader.Cell(2, 2);
            subHeader2.Range.Text = "Position";
            subHeader2.Range.Bold = 1;
            subHeader2.Range.Font.Size = fontSize;
            subHeader2.Range.Font.Name = fontName;
            subHeader2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            subHeader2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(subHeader2);

            Cell subHeader3 = nestedHeader.Cell(2, 3);
            subHeader3.Range.Text = "Voltage\nApplication\nBetween\nterminal,\nnumber";
            subHeader3.Range.Bold = 1;
            subHeader3.Range.Font.Size = fontSize;
            subHeader3.Range.Font.Name = fontName;
            subHeader3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            subHeader3.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(subHeader3);

            Cell subHeader4 = nestedHeader.Cell(2, 4);
            subHeader4.Range.Text = "Voltage\nApplication\nBetween\nbody (B)\ntogether\nwith\nterminal,\nnumber";
            subHeader4.Range.Bold = 1;
            subHeader4.Range.Font.Size = fontSize;
            subHeader4.Range.Font.Name = fontName;
            subHeader4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            subHeader4.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(subHeader4);

            ApplyBorders(h2);

            // Column 3: Specification
            Cell h3 = table.Cell(headerRow, 3);
            h3.Range.Text = "";
            h3.Range.Bold = 1;
            h3.Range.Font.Size = fontSize;
            h3.Range.Font.Name = fontName;
            h3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            h3.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(h3);

            // Column 4: Result/Decision
            Cell h4 = table.Cell(headerRow, 4);
            h4.Range.Text = "";
            h4.Range.Bold = 1;
            h4.Range.Font.Size = fontSize;
            h4.Range.Font.Name = fontName;
            h4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            h4.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ApplyBorders(h4);
        }

        private void CreateDiagramGroupSection(Table table, int startRow, List<DataRow> rows, Application wordApp, float[] colWidths, float fontSize, string fontName)
        {
            int n = rows.Count;
            string fileName = rows[0]["FileName"]?.ToString() ?? "";
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleDiagram", fileName);
            bool hasValidImage = !string.IsNullOrEmpty(fileName) && File.Exists(imagePath);

            for (int i = 0; i < n; i++)
            {
                DataRow dr = rows[i];
                int currentRow = startRow + i;
                // Set Clause Number in column 1
                Cell c1 = table.Cell(currentRow, 1);
                SetCellText(c1, dr["ClauseNumber"]?.ToString() ?? "", false, fontSize, fontName);
                c1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                ApplyBorders(c1);
                // Create nested table in column 2 - NOW ALWAYS 1 ROW (data only)
                Cell c2 = table.Cell(currentRow, 2);
                c2.Range.Text = "";
                Table nested = c2.Range.Tables.Add(c2.Range, 1, 4); // Always 1 row
                nested.Borders.Enable = 1;
                nested.Range.Font.Size = fontSize;
                nested.Range.Font.Name = fontName;
                // Set column widths
                float totalWidth = colWidths[1];
                float col1Width = 0.9f;
                float remainingWidth = totalWidth - col1Width;
                nested.Columns[1].SetWidth(wordApp.InchesToPoints(col1Width), WdRulerStyle.wdAdjustNone);
                nested.Columns[2].SetWidth(wordApp.InchesToPoints(remainingWidth * 0.25f), WdRulerStyle.wdAdjustNone);
                nested.Columns[3].SetWidth(wordApp.InchesToPoints(remainingWidth * 0.35f), WdRulerStyle.wdAdjustNone);
                nested.Columns[4].SetWidth(wordApp.InchesToPoints(remainingWidth * 0.40f), WdRulerStyle.wdAdjustNone);
                // ============ DATA ROW (Always row 1) ============
                Cell dataC1 = nested.Cell(1, 1);
                dataC1.Range.Text = "";
                // Image only in first row
                if (i == 0 && hasValidImage)
                {
                    InlineShape img = dataC1.Range.InlineShapes.AddPicture(imagePath);
                    img.Width = wordApp.InchesToPoints(0.65f);
                    img.Height = wordApp.InchesToPoints(0.65f);
                    dataC1.Range.ParagraphFormat.SpaceBefore = 3;
                    dataC1.Range.ParagraphFormat.SpaceAfter = 0;
                    dataC1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    dataC1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }
                // Apply borders for image cell
                if (i > 0)
                {
                    dataC1.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                    dataC1.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    dataC1.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                }
                else
                {
                    ApplyBorders(dataC1);
                }
                if (i < n - 1)
                {
                    dataC1.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
                }
                // Column 2: Position
                Cell dataC2 = nested.Cell(1, 2);
                SetCellText(dataC2, dr["Position"]?.ToString() ?? "", false, fontSize, fontName);
                dataC2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                dataC2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                ApplyBorders(dataC2);
                // Column 3: Between terminal
                Cell dataC3 = nested.Cell(1, 3);
                SetCellText(dataC3, dr["TerminalNumber"]?.ToString() ?? "", false, fontSize, fontName);
                dataC3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                dataC3.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                ApplyBorders(dataC3);

                // Column 4: Between body (B) together with terminal
                Cell dataC4 = nested.Cell(1, 4);
                SetCellText(dataC4, dr["TheBodiesBTogetherWithTerminalNumber"]?.ToString() ?? "", false, fontSize, fontName);
                dataC4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                dataC4.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                ApplyBorders(dataC4);

                ApplyBorders(c2);

                // Column 3: Specification
                Cell c3 = table.Cell(currentRow, 3);
                SetCellText(c3, dr["Specification"]?.ToString() ?? "", false, fontSize, fontName);
                c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                ApplyBorders(c3);

                // Column 4: Result/Decision
                Cell c4 = table.Cell(currentRow, 4);
                SetCellText(c4, dr["Result"]?.ToString() ?? "", false, fontSize, fontName);
                c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                //ApplyBorders(c4);
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

                string[] specValues = ParsePipeSeparatedValues(row["Specification"]?.ToString() ?? "", columnCount);

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
            string[] columnHeaders = { "1", "2", "3" };
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