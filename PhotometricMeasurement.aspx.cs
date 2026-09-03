using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using ListItem = System.Web.UI.WebControls.ListItem;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace LIMS
{
    public partial class PhotometricMeasurement : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DAL.CM _cm = new DAL.CM();

        // Constants for A4 page layout
        private const int A4_PAGE_WIDTH_TWIPS = 12240;
        private const int LEFT_MARGIN_TWIPS = 1440;
        private const int RIGHT_MARGIN_TWIPS = 1440;
        private const int USABLE_WIDTH_TWIPS = A4_PAGE_WIDTH_TWIPS - LEFT_MARGIN_TWIPS - RIGHT_MARGIN_TWIPS;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Register Syncfusion License Key
                string licenseKey = "Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTc0RnWX5cd3FcTmBdV091XA==";
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

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

                    txtbrand.Enabled = false;
                    txtstandard.Enabled = false;
                    txtproduct.Enabled = false;
                    txttestMethodNo.Enabled = false;
                    txtworkorder.Enabled = false;
                    txtmodel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        private void LoadClauseTable()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);

            DataTable dtcl = _cm.GetCRFCloneList_LED(sampleId);
            int cloneCRFSampleID = Convert.ToInt32(dtcl.Rows[0]["CloneCRFSampleID"].ToString());
            ViewState["CloneCRFSampleID"] = dtcl.Rows[0]["CloneCRFSampleID"].ToString();

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

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestDate"].ToString()))
                {
                    DateTime testDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestDate"].ToString(), out testDate))
                    {
                        txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");
                    }
                }

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestCompletationDate"].ToString()))
                {
                    DateTime completionDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestCompletationDate"].ToString(), out completionDate))
                    {
                        txtcompletiondate.Text = completionDate.ToString("yyyy-MM-dd");
                    }
                }
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

            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_LED(sampleId);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                int departmentID = Convert.ToInt32(dt1.Rows[0]["DepartmentID"]);
                int productCategoryID = Convert.ToInt32(dt1.Rows[0]["ProductCategoryID"]);
                string StandardID = dt1.Rows[0]["StandardID"].ToString();
                int reportID = Convert.ToInt32(Request.QueryString["r"]);

                string TestStandardGet = dts.Rows[0]["TestStandard"].ToString();
                txtproduct.Text = dt1.Rows[0]["ProductName"].ToString();
                txtbrand.Text = dt1.Rows[0]["BrandProduct"].ToString();
                txtmodel.Text = dt1.Rows[0]["ProductSerialNo"].ToString();
                txtworkorder.Text = "";
                txttestMethodNo.Text = dts.Rows[0]["TestMethod"].ToString();
                ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"].ToString();

                string[] ids = StandardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> standardIdList = ids.Select(id => id.Trim()).ToList();

                DataTable filteredClauseTable = null;
                if (standardIdList.Count > 0)
                {
                    var matchingRows = dt.AsEnumerable().Where(r => standardIdList.Contains(r["Standard_ID"].ToString()));

                    if (matchingRows.Any())
                        filteredClauseTable = matchingRows.CopyToDataTable();
                }

                if (filteredClauseTable == null || filteredClauseTable.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No matching clauses found!');</script>");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                bool tableStarted = false;

                foreach (DataRow row in filteredClauseTable.Rows)
                {
                    bool isHeader = row["isHeader"].ToString() == "1";

                    if (isHeader)
                    {
                        if (tableStarted) sb.Append("</table>");

                        sb.Append("<table class='clause-table'>");

                        sb.Append("<tr class='sub-header-row'>");
                        sb.Append("<th class='clause-number'>Klausul<br/>Clause</th>");
                        sb.Append("<th class='requirement'>Persyaratan<br/>Requirements</th>");
                        sb.Append("<th class='result'>Keputusan<br/>Decision</th>");
                        sb.Append("<th class='remark'>*Keterangan<br/>Remark</th>");
                        sb.Append("</tr>");

                        sb.Append("<tr class='header-row'>");
                        sb.Append("<td>");
                        sb.Append($"<b>{row["ClauseNumber"]}</b> <br/>");
                        sb.Append("</td>");
                        sb.Append("<td colspan='2'>");
                        sb.Append($"<u>{row["ClauseTitle_BH"]}</u><br/>");
                        sb.Append($"<i>{row["ClauseTitle"]}</i>");
                        sb.Append("</td>");

                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string savedRemark = "";
                        var savedRowForRemark = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForRemark != null)
                            savedRemark = savedRowForRemark["Remark"]?.ToString() ?? "";

                        sb.Append($"<td class='remark'><textarea name='remark_{clauseID}' rows='2' style='width:96%; resize:vertical;'>{HttpUtility.HtmlEncode(savedRemark)}</textarea></td>");

                        sb.Append("</tr>");

                        tableStarted = true;
                    }
                    else
                    {
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        sb.Append("<tr>");
                        sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                        sb.Append("<td class='requirement'>");
                        sb.Append($"<u>{row["Requirement_BH"]}</u><br/>");
                        sb.Append($"<i>{row["Requirement"]}</i>");
                        sb.Append("</td>");

                        sb.Append("<td class='result'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        GenerateResultInput(sb, row, saved, clauseID);
                        sb.Append("</td>");

                        string savedRemark = "";
                        var savedRowForRemark = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForRemark != null)
                            savedRemark = savedRowForRemark["Remark"]?.ToString() ?? "";

                        sb.Append($"<td class='remark'><textarea name='remark_{clauseID}' rows='2' style='width:96%; resize:vertical;'>{HttpUtility.HtmlEncode(savedRemark)}</textarea></td>");
                        sb.Append("</tr>");
                    }
                }

                if (tableStarted) sb.Append("</table>");
                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
            }
        }

        private void BindEquipmentCheckboxes()
        {
            string query = "SELECT * FROM Calibration_Equipment WHERE DepartmentId = 4009";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    chkEquipmentList.DataSource = reader;
                    chkEquipmentList.DataTextField = "Equipment";
                    chkEquipmentList.DataValueField = "EpID";
                    chkEquipmentList.DataBind();

                    reader.Close();
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
                sb.Append($"<input type='text' name='textresult_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='width:94%;' />");
            }
        }

        private DataTable GetClauseData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string[] standardIds = lblstandardID.Text.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToArray();

                if (standardIds.Length > 0)
                {
                    string inClause = string.Join(",", standardIds.Select(id => $"'{id}'"));
                    using (SqlCommand cmd = new SqlCommand($"SELECT * FROM Clauses WHERE Standard_ID IN ({inClause}) ORDER BY ClauseID", conn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
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
                bool b1 = false; bool b2 = false;
                string cloneCFRsampleID = ViewState["CloneCRFSampleID"].ToString();
                string equipmentIdsCSV = "";

                List<string> selectedEquipmentIds = new List<string>();

                foreach (ListItem item in chkEquipmentList.Items)
                {
                    if (item.Selected)
                    {
                        selectedEquipmentIds.Add(item.Value);
                    }
                }

                if (selectedEquipmentIds.Count > 0)
                {
                    equipmentIdsCSV = string.Join(",", selectedEquipmentIds);
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
                        string remark = Request.Form[$"remark_{clauseID}"] ?? "";

                        if (isHeader)
                        {
                            int result0 = _cm.TestResultsAdd_LED(clauseID, txtstartingdate.Text, txtcompletiondate.Text, "", remark, sampleId, sampleIdSeries,
                                 "header", language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                 txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text);
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
                                result = Request.Form[$"textresult_{clauseID}"] ?? "";
                            }

                            int result0 = _cm.TestResultsAdd_LED(clauseID, txtstartingdate.Text, txtcompletiondate.Text, result, remark, sampleId, sampleIdSeries,
                                inputType, language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text);

                            if (result0 > 0)
                            {
                                b2 = true;
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select at least one equipment.');", true);
                    return;
                }

                if (b1 == true)
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
                                b2 = true;
                            }
                        }
                    }
                }

                if (b2 == true)
                {
                    btnExport_Click(this, new EventArgs());
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            //new change//
            string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
            string pathout = Server.MapPath("~/Reports/");
            string templatePath = Server.MapPath("~/InputFiles/LEDTestRecord_2.docx");

            if (!Directory.Exists(pathout))
                Directory.CreateDirectory(pathout);

            string outputDocxPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".docx");
            string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

            try
            {
                // Load template using OpenXML
                byte[] templateBytes = File.ReadAllBytes(templatePath);

                // Process document using OpenXML
                byte[] processedBytes = ProcessDocument(templateBytes);

                // Save DOCX
                File.WriteAllBytes(outputDocxPath, processedBytes);

                // Convert to PDF using Syncfusion
                byte[] pdfBytes = ConvertDocxToPdfSyncfusion(processedBytes);
                File.WriteAllBytes(outputPdfPath, pdfBytes);

                // Stream PDF to client
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                Response.WriteFile(outputPdfPath);
                Response.End();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script> alert('Error: {ex.Message.Replace("'", "\\'")}'); </script>");
            }
        }

        /// <summary>
        /// Process document using OpenXML
        /// </summary>
        private byte[] ProcessDocument(byte[] docBytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(docBytes, 0, docBytes.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    // Replace template variables
                    ReplaceTemplateVariablesOpenXML(wordDoc);

                    // Create report table
                    DataTable dtReport = GetReportData();
                    CreateReportTableOpenXML(wordDoc, dtReport);

                    // Clean up any unresolved placeholders
                    CleanupUnresolvedPlaceholders(wordDoc);

                    wordDoc.MainDocumentPart.Document.Save();
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Replace template variables using OpenXML
        /// </summary>
        private void ReplaceTemplateVariablesOpenXML(WordprocessingDocument doc)
        {
            DataTable DisResget = _cm.GetTestResultLED_DISTINCT(lblSampleID.Text, lblReportID.Text);

            if (DisResget != null && DisResget.Rows.Count > 0)
            {
                string equipmentIds = DisResget.Rows[0]["EquipmentIds"]?.ToString() ?? "";
                List<string> equipmentNames = GetEquipmentNamesAsList(equipmentIds);
                DataRow row = DisResget.Rows[0];

                var replacements = new Dictionary<string, string>
                {
                    {"{WorkorderNumber}", GetSafeString(row, "WorkOrderNo") },
                    {"{Product}", GetSafeString(row, "ProductName")},
                    {"{Brand}", GetSafeString(row, "ProductBrand")},
                    {"{TestStartDate}", FormatDate(GetSafeString(row, "TestDate"))},
                    {"{TestCompletiondate}", FormatDate(GetSafeString(row, "TestCompletationDate"))},
                    {"{Temperature}", GetSafeString(row, "Temperature")},
                    {"{Humidity}", GetSafeString(row, "Humidity") },
                    {"{Method}", GetSafeString(row, "TestMethod")},
                    {"{Standard}", GetSafeString(row, "StandardName")},
                    {"{Model}", GetSafeString(row, "ProductModel")},
                };

                // Replace using OpenXML
                foreach (var replacement in replacements)
                {
                    if (!string.IsNullOrEmpty(replacement.Key) && !string.IsNullOrEmpty(replacement.Value))
                    {
                        FindAndReplaceOpenXML(doc, replacement.Key, replacement.Value);
                    }
                }

                // Replace equipment placeholder
                if (equipmentNames != null && equipmentNames.Count > 0)
                {
                    StringBuilder equipmentText = new StringBuilder();
                    for (int i = 0; i < equipmentNames.Count; i++)
                    {
                        equipmentText.AppendLine($"{i + 1}. {equipmentNames[i]}");
                    }
                    FindAndReplaceOpenXML(doc, "{Equipment}", equipmentText.ToString());
                }
            }
        }

        /// <summary>
        /// Find and replace text using OpenXML
        /// </summary>
        private void FindAndReplaceOpenXML(WordprocessingDocument doc, string findText, string replaceText)
        {
            if (string.IsNullOrEmpty(findText)) return;
            if (replaceText == null) replaceText = "";

            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            // Search in body
            ReplaceAllInElementOpenXML(mainPart.Document.Body, findText, replaceText);

            // Search in headers
            foreach (var headerPart in mainPart.HeaderParts)
            {
                if (headerPart.Header != null)
                    ReplaceAllInElementOpenXML(headerPart.Header, findText, replaceText);
            }

            // Search in footers
            foreach (var footerPart in mainPart.FooterParts)
            {
                if (footerPart.Footer != null)
                    ReplaceAllInElementOpenXML(footerPart.Footer, findText, replaceText);
            }
        }

        /// <summary>
        /// Replace all occurrences in an OpenXML element
        /// </summary>
        private void ReplaceAllInElementOpenXML(OpenXmlElement element, string findText, string replaceText)
        {
            foreach (var paragraph in element.Descendants<Paragraph>().ToList())
            {
                ReplaceInParagraphOpenXML(paragraph, findText, replaceText);
            }
        }

        /// <summary>
        /// Replace text in a paragraph using OpenXML
        /// </summary>
        private bool ReplaceInParagraphOpenXML(Paragraph paragraph, string findText, string replaceText)
        {
            var runs = paragraph.Descendants<Run>().ToList();
            if (runs.Count == 0) return false;

            foreach (var run in runs)
            {
                var textElement = run.Descendants<WordText>().FirstOrDefault();
                if (textElement != null && textElement.Text.Contains(findText))
                {
                    textElement.Text = textElement.Text.Replace(findText, replaceText);
                    textElement.Space = SpaceProcessingModeValues.Preserve;
                    return true;
                }
            }

            // Handle case where text is split across multiple runs
            string fullText = string.Join("", runs.Select(r => r.Descendants<WordText>().FirstOrDefault()?.Text ?? ""));
            if (!fullText.Contains(findText)) return false;

            string newFullText = fullText.Replace(findText, replaceText);

            WordText firstTextEl = null;
            foreach (var run in runs)
            {
                var textEl = run.Descendants<WordText>().FirstOrDefault();
                if (textEl == null) continue;
                if (firstTextEl == null)
                {
                    firstTextEl = textEl;
                    textEl.Text = newFullText;
                    textEl.Space = SpaceProcessingModeValues.Preserve;
                }
                else
                {
                    textEl.Text = "";
                }
            }

            return firstTextEl != null;
        }

        /// <summary>
        /// Get paragraph full text
        /// </summary>
        private string GetParagraphFullText(Paragraph paragraph)
        {
            return string.Join("", paragraph.Descendants<WordText>().Select(t => t.Text));
        }

        /// <summary>
        /// Find paragraph containing placeholder
        /// </summary>
        private Paragraph FindParagraphContaining(OpenXmlElement root, string placeholder)
        {
            foreach (var para in root.Descendants<Paragraph>())
            {
                if (GetParagraphFullText(para).Contains(placeholder))
                    return para;
            }
            return null;
        }

        /// <summary>
        /// Clean up unresolved placeholders
        /// </summary>
        private void CleanupUnresolvedPlaceholders(WordprocessingDocument doc)
        {
            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            var regex = new Regex(@"\{[A-Za-z][A-Za-z0-9_.\*:'/ ]{0,60}\}");

            void CleanElement(OpenXmlElement element)
            {
                foreach (var paragraph in element.Descendants<Paragraph>().ToList())
                {
                    string fullText = GetParagraphFullText(paragraph);
                    if (string.IsNullOrEmpty(fullText)) continue;

                    var matches = regex.Matches(fullText);
                    foreach (Match m in matches)
                    {
                        if (GetParagraphFullText(paragraph).Contains(m.Value))
                        {
                            ReplaceInParagraphOpenXML(paragraph, m.Value, "");
                        }
                    }
                }
            }

            CleanElement(mainPart.Document.Body);
            foreach (var headerPart in mainPart.HeaderParts)
                if (headerPart.Header != null) CleanElement(headerPart.Header);
            foreach (var footerPart in mainPart.FooterParts)
                if (footerPart.Footer != null) CleanElement(footerPart.Footer);
        }

        /// <summary>
        /// Create report table using OpenXML
        /// </summary>
        private void CreateReportTableOpenXML(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            // Find the placeholder paragraph
            Paragraph targetParagraph = FindParagraphContaining(body, "{TestingAndMeasurement}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{TestingAndMeasurement}' not found in the document template.");

            // Remove placeholder
            ReplaceInParagraphOpenXML(targetParagraph, "{TestingAndMeasurement}", "");

            // Count rows
            int dataRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() != "1");
            int headerRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() == "1");
            int totalRows = 1 + dataRows + headerRows; // 1 for main header

            // Create table
            Table table = new Table();

            // Table properties
            TableProperties tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            table.AppendChild(tableProps);

            // Table grid - column widths in twips
            TableGrid grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = "700" });   // Clause (70 twips)
            grid.AppendChild(new GridColumn() { Width = "3300" });  // Requirements (330 twips)
            grid.AppendChild(new GridColumn() { Width = "2000" });  // Result (200 twips)
            grid.AppendChild(new GridColumn() { Width = "1000" });  // Remark (100 twips)
            table.AppendChild(grid);

            // Create main header row
            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            string[] headers = { "Klausul\nClause", "Persyaratan\nRequirements", "Hasil\nResult", "*Keterangan\nRemark" };
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                TableCellProperties cellProps = new TableCellProperties();
                cellProps.AppendChild(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
                cellProps.AppendChild(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" });
                cell.AppendChild(cellProps);

                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );

                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            int currentRow = 1;
            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                tableRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 300, HeightType = HeightRuleValues.AtLeast }
                ));

                if (row["isHeader"].ToString() == "1")
                {
                    // Section header row
                    // Clause cell
                    TableCell clauseCell = new TableCell();
                    clauseCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph clausePara = new Paragraph();
                    clausePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run clauseRun = new Run();
                    clauseRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    clauseRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clausePara.AppendChild(clauseRun);
                    clauseCell.AppendChild(clausePara);
                    tableRow.AppendChild(clauseCell);

                    // Title cell (merged columns 2-3)
                    TableCell titleCell = new TableCell();
                    titleCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                        new GridSpan() { Val = 2 }
                    ));
                    string clauseTitleBH = row["ClauseTitle_BH"]?.ToString() ?? "";
                    string clauseTitle = row["ClauseTitle"]?.ToString() ?? "";
                    Paragraph titlePara = new Paragraph();
                    titlePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run titleRun = new Run();
                    titleRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    titleRun.AppendChild(new WordText() { Text = $"{clauseTitleBH}\n{clauseTitle}", Space = SpaceProcessingModeValues.Preserve });
                    titlePara.AppendChild(titleRun);
                    titleCell.AppendChild(titlePara);
                    tableRow.AppendChild(titleCell);

                    // Remark cell
                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }
                else
                {
                    // Data row
                    // Clause number
                    TableCell clauseNumCell = new TableCell();
                    clauseNumCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph clauseNumPara = new Paragraph();
                    clauseNumPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run clauseNumRun = new Run();
                    clauseNumRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    clauseNumRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clauseNumPara.AppendChild(clauseNumRun);
                    clauseNumCell.AppendChild(clauseNumPara);
                    tableRow.AppendChild(clauseNumCell);

                    // Requirements
                    TableCell reqCell = new TableCell();
                    reqCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    string reqBH = row["Requirement_BH"]?.ToString() ?? "";
                    string reqEN = row["Requirement"]?.ToString() ?? "";
                    Paragraph reqPara = new Paragraph();
                    reqPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run reqRun = new Run();
                    reqRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    reqRun.AppendChild(new WordText() { Text = $"{reqBH}\n{reqEN}", Space = SpaceProcessingModeValues.Preserve });
                    reqPara.AppendChild(reqRun);
                    reqCell.AppendChild(reqPara);
                    tableRow.AppendChild(reqCell);

                    // Result
                    TableCell resultCell = new TableCell();
                    resultCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string inputType = GetInputTypeFromClause(row);
                    string resultText = FormatResultsForExport(
                        inputType,
                        row["ResultOptions"]?.ToString(),
                        row["ResultOptions_BH"]?.ToString(),
                        row["Results"]?.ToString()
                    );
                    Paragraph resultPara = new Paragraph();
                    resultPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run resultRun = new Run();
                    int fontSize = inputType == "checkbox" ? 20 : 18;
                    resultRun.AppendChild(new RunProperties(new FontSize() { Val = fontSize.ToString() }));
                    resultRun.AppendChild(new WordText() { Text = resultText, Space = SpaceProcessingModeValues.Preserve });
                    resultPara.AppendChild(resultRun);
                    resultCell.AppendChild(resultPara);
                    tableRow.AppendChild(resultCell);

                    // Remark
                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }

                table.AppendChild(tableRow);
                currentRow++;
            }

            // Insert table after the target paragraph
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private string GetInputTypeFromClause(DataRow row)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
                return "checkbox";
            else
                return "text";
        }

        private string GetRemarkText(DataRow row)
        {
            if (row.Table.Columns.Contains("Remark"))
                return row["Remark"]?.ToString() ?? "";
            else if (row.Table.Columns.Contains("Remarks"))
                return row["Remarks"]?.ToString() ?? "";
            else if (row.Table.Columns.Contains("Keterangan"))
                return row["Keterangan"]?.ToString() ?? "";
            return "";
        }

        private string FormatResultsForExport(string inputType, string resultOptions, string resultOptionsBH, string selectedResults)
        {
            if (inputType == "checkbox" && !string.IsNullOrEmpty(resultOptions))
            {
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : options;
                string[] selected = !string.IsNullOrEmpty(selectedResults) ? selectedResults.Split(',') : new string[0];
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < options.Length; i += 2)
                {
                    string option1 = options[i].Trim();
                    string optionDisplay1 = i < optionsBH.Length ? optionsBH[i].Trim() : option1;
                    bool isSelected1 = selected.Any(s => s.Trim().Equals(option1, StringComparison.OrdinalIgnoreCase));
                    string checkbox1 = isSelected1 ? "☒" : "☐";
                    sb.Append($"{checkbox1} {optionDisplay1}");

                    if (i + 1 < options.Length)
                    {
                        string option2 = options[i + 1].Trim();
                        string optionDisplay2 = (i + 1) < optionsBH.Length ? optionsBH[i + 1].Trim() : option2;
                        bool isSelected2 = selected.Any(s => s.Trim().Equals(option2, StringComparison.OrdinalIgnoreCase));
                        string checkbox2 = isSelected2 ? "☒" : "☐";
                        sb.Append($"\t\t{checkbox2} {optionDisplay2}");
                    }

                    if (i + 2 < options.Length)
                    {
                        sb.Append("\r\n");
                    }
                }
                return sb.ToString();
            }
            return selectedResults ?? "";
        }

        private string GetSafeString(DataRow row, string columnName)
        {
            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                {
                    return row[columnName].ToString();
                }
                return "";
            }
            catch
            {
                return "";
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
                return dateString;
            }
            catch
            {
                return dateString;
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

        private DataTable GetReportData()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT 
                                c.ClauseID,
                                c.ClauseNumber,
                                c.ClauseTitle,
                                c.ClauseTitle_BH,
                                c.Requirement,
                                c.Requirement_BH,
                                c.ResultOptions,
                                c.ResultOptions_BH,
                                c.isHeader, c.standard_ID,
                                ISNULL(t.Result, '') as Results,
                                ISNULL(t.Remark, '') as Remark,
                                ISNULL(t.InputType, '') as SavedInputType
                            FROM Clauses c
                            inner JOIN TestResultsLED t ON c.ClauseID = t.ClauseID 
                                AND t.SampleID = @SampleID
                                AND t.ReportID = @ReportID
                            ORDER BY c.ClauseID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Convert DOCX to PDF using Syncfusion
        /// </summary>
        private byte[] ConvertDocxToPdfSyncfusion(byte[] docxBytes)
        {
            try
            {
                using (MemoryStream docxStream = new MemoryStream(docxBytes))
                using (WordDocument wordDocument = new WordDocument(docxStream, FormatType.Automatic))
                using (DocIORenderer renderer = new DocIORenderer())
                using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                using (MemoryStream pdfStream = new MemoryStream())
                {
                    pdfDocument.Save(pdfStream);
                    return pdfStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Syncfusion PDF conversion error: {ex.Message}");
            }
        }
    }
}