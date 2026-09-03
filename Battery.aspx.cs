using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LIMS.DAL;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Licensing;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;

namespace LIMS
{
    public partial class Battery : System.Web.UI.Page
    {
        private const string SYNCFUSION_LICENSE_KEY = "Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTc0RnWX5cd3FcTmBdV091XA==";

        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        DAL.CM _cm = new DAL.CM();
        private int reportId = 0;
        private int sampleId = 0;
        private string sampleIdSeries = "smp/2001";
        private string uploadFolder = "~/FilesUpload/";
        private class ExcelBlock
        {
            public DataTable Data;
            public List<int[]> ColSpans;
            public List<byte[]> Images;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);

            if (!IsPostBack)
            {
                if (_cm == null) _cm = new DAL.CM();

                int reportId = 0;
                int sampleId = 0;

                // Decode ReportID from Base64
                if (Request.QueryString["r"] != null)
                {
                    string encodedReport = Request.QueryString["r"].ToString();
                    byte[] decodedReportBytes = Convert.FromBase64String(encodedReport);
                    string decodedReportId = Encoding.UTF8.GetString(decodedReportBytes);
                    reportId = Convert.ToInt32(decodedReportId);
                }

                // Decode SampleID from Base64
                if (Request.QueryString["s"] != null)
                {
                    string encodedSample = Request.QueryString["s"].ToString();
                    byte[] decodedSampleBytes = Convert.FromBase64String(encodedSample);
                    string decodedSampleId = Encoding.UTF8.GetString(decodedSampleBytes);
                    sampleId = Convert.ToInt32(decodedSampleId);
                }

                ViewState["ReportID"] = reportId;
                ViewState["SampleID"] = sampleId;

                string physicalPath = Server.MapPath(uploadFolder);
                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);

                _cm.sampleId = sampleId;
                _cm.ReportID = reportId.ToString();


                DataTable dt1 = _cm.Getdataforseriesandreportid_PV(_cm);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    //// Set dynamic values from database
                    //txtbrand.Text = dt1.Rows[0]["BrandProduct"].ToString();
                    txtR.Text = dt1.Rows[0]["R"].ToString();
                  
//txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"].ToString();
                    //txttestmethod.Text = dt1.Rows[0]["TestMethod"].ToString();
                    
                   // txtManufacturer.Text = "Haitai Power Technology";
                   
                  //  txtpic.Text = "AD AJ DD";

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check  sample ID.');", true);
                }

                // **NEW: Check if status is 1005 (retest)**
                // **Check if status is 1005 (retest)**
                if (_cm == null) _cm = new DAL.CM();
                DataTable dtStatus = _cm.GetSampleStatus(sampleId, reportId);

                if (dtStatus != null && dtStatus.Rows.Count > 0)
                {
                    foreach (DataRow row in dtStatus.Rows)
                    {
                        int status = Convert.ToInt32(row["samplestatus"]);

                        if (status == 19) // New test
                        {
                            btnDownloadExisting.Visible = false;
                            btnUpdateUpload.Visible = false;
                            btnDownlaodNew.Visible = true;
                            btnConfirmUpload.Visible = true;
                            break;
                        }
                        else if (status == 1005) // Retest
                        {
                            LoadExistingTestResultForRetest(reportId, sampleId);
                            btnDownloadExisting.Visible = true;
                            btnUpdateUpload.Visible = true;
                            btnDownlaodNew.Visible = false;
                            btnConfirmUpload.Visible = false;
                            break;
                        }
                    }
                }
                else
                {
                    // No status found, hide all buttons
                    btnDownloadExisting.Visible = false;
                    btnUpdateUpload.Visible = false;
                    btnDownlaodNew.Visible = false;
                    btnConfirmUpload.Visible = false;

                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample status.');", true);
                }


            }
            else
            {
                int reportId = Convert.ToInt32(ViewState["ReportID"]);
                int sampleId = Convert.ToInt32(ViewState["SampleID"]);
            }
        }
        protected void LoadExistingTestResultForRetest(int reportId, int sampleId)
        {
            try
            {
                if (_cm == null) _cm = new DAL.CM();
                DataTable dtExisting = _cm.GetExistingTestResult(reportId, sampleId);

                if (dtExisting != null && dtExisting.Rows.Count > 0)
                {
                    DataRow row = dtExisting.Rows[0];
                    string fileName = row["FileName"].ToString();
                    string excelPath = row["Result"]?.ToString();

                    if (string.IsNullOrEmpty(excelPath))
                    {
                        ShowMessage("Excel path not found in database. Please upload a new file.");
                        return;
                    }

                    string fullExcelPath = Server.MapPath(excelPath);
                    if (!File.Exists(fullExcelPath))
                    {
                        ShowMessage($"Excel file not found at path: {excelPath}. Please upload a new file.");
                        InitializeFormFields();
                        return;
                    }

                    List<ExcelBlock> blocks = ReadExcelBlocks(fullExcelPath);

                    if (row["AnalysisStartingDate"] != DBNull.Value)
                        txtAnalysisStart.Text = Convert.ToDateTime(row["AnalysisStartingDate"]).ToString("yyyy-MM-dd");
                    if (row["AnalysisCompletionDate"] != DBNull.Value)
                        txtCompleteDate.Text = Convert.ToDateTime(row["AnalysisCompletionDate"]).ToString("yyyy-MM-dd");

                    Session["ExcelData"] = blocks;
                    Session["IsRetest"] = true;
                    Session["ExistingFileName"] = fileName;
                    Session["PreviewFilePath"] = fullExcelPath;

                    ViewState["ExistingFilePath"] = excelPath;
                    ViewState["ExistingFileName"] = fileName;

                    btnDownloadExisting.Visible = true;
                    btnDownloadExisting.Text = $"Download Existing File: {fileName}";
                    btnUpdateUpload.Visible = true;
                    btnConfirmUpload.Visible = false;
                    btnDownlaodNew.Visible = false;

                    ShowMessage("Existing test result loaded. You may update and re-upload.");
                }
                else
                {
                    ShowMessage("No existing test result found for this sample. Please upload a new file.");
                    InitializeFormFields();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading existing test result: {ex.Message}");
                InitializeFormFields();
            }
        }

        protected void btnDownloadExisting_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownloadExisting.Visible = true;
                btnUpdateUpload.Visible = true;
                btnDownlaodNew.Visible = false;
                btnConfirmUpload.Visible = false;

                string filePath = ViewState["ExistingFilePath"]?.ToString();
                string fileName = ViewState["ExistingFileName"]?.ToString();

                if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(fileName))
                {
                    string fullPath = Server.MapPath(filePath);
                    if (File.Exists(fullPath))
                    {
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                        Response.TransmitFile(fullPath);
                        Response.Flush();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    else ShowMessage($"File not found at: {fullPath}");
                }
                else ShowMessage("File information not available for download.");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error downloading file: {ex.Message}");
            }
        }

        protected void InitializeFormFields()
        {
            txtAnalysisStart.Text = "";
            txtCompleteDate.Text = "";
            txtR.Text = "";
            txtRoomTempBegin.Text = "";
            txtsample.Text = "";
            txtHumidity.Text = "";
            txtPIC.Text = "";
            txtwo.Text = "";
        }

        private DataTable GetReportDetailsFromDatabase(int reportId)
        {
            return _cm.GetDocNoByReportID(reportId);
        }

        private string GetExcelFileName(int reportId)
        {
            string excelFileName = "ReportTemplate";
            DataTable dt = GetReportDetailsFromDatabase(reportId);
            if (dt != null && dt.Rows.Count > 0)
                excelFileName = dt.Rows[0]["ExcelFileName"]?.ToString() ?? "ReportTemplate";
            if (!excelFileName.ToLower().EndsWith(".xlsx") && !excelFileName.ToLower().EndsWith(".xls"))
                excelFileName += ".xlsx";
            return excelFileName;
        }

        // =====================================================================
        // DOWNLOAD BLANK TEMPLATE — built with Syncfusion XlsIO
        // =====================================================================
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            btnDownlaodNew.Visible = true;
            btnUpdateUpload.Visible = false;
            btnDownloadExisting.Visible = false;
            int flag = 0;

            if (string.IsNullOrWhiteSpace(txtAnalysisStart.Text)) { txtAnalysisStart.BorderColor = System.Drawing.Color.Red; flag = 1; }
            else txtAnalysisStart.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtCompleteDate.Text)) { txtCompleteDate.BorderColor = System.Drawing.Color.Red; flag = 1; }
            else txtCompleteDate.BorderColor = System.Drawing.Color.Empty;

            if (flag != 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill all required fields.');", true);
                return;
            }

            try
            {
                btnDownlaodNew.Visible = true;
                btnConfirmUpload.Visible = true;
                btnUpdateUpload.Visible = false;
                btnDownloadExisting.Visible = false;

                int sampleId = 0;
                if (ViewState["ReportID"] != null) reportId = Convert.ToInt32(ViewState["ReportID"]);
                if (ViewState["SampleID"] != null) sampleId = Convert.ToInt32(ViewState["SampleID"]);

                DataTable dtReport = GetReportDetailsFromDatabase(reportId);
                if (dtReport == null || dtReport.Rows.Count == 0) { ShowMessage("Report details not found."); return; }

                string excelFileName = dtReport.Rows[0]["ExcelFileName"]?.ToString() ?? "ReportTemplate";
                string reportName = dtReport.Rows[0]["ReportName"]?.ToString() ?? "";
                string documentNumber = dtReport.Rows[0]["docno"]?.ToString() ?? "";

                if (!excelFileName.ToLower().EndsWith(".xlsx") && !excelFileName.ToLower().EndsWith(".xls"))
                    excelFileName += ".xlsx";

                string templatePath = Server.MapPath($"~/InputFiles/{excelFileName}");
                if (!File.Exists(templatePath)) { ShowMessage($"Excel file '{excelFileName}' not found."); return; }

                bool isOriginalDownload = !CheckIfFileExistsInDatabase(reportId.ToString(), sampleId);
                string downloadFileName;
                if (isOriginalDownload)
                    downloadFileName = GenerateFileName(reportId.ToString(), sampleId, reportName, documentNumber, ".xlsx", false, 0);
                else
                {
                    DataTable dtExisting = _cm.GetExistingTestResult(reportId, sampleId);
                    downloadFileName = (dtExisting != null && dtExisting.Rows.Count > 0)
                        ? dtExisting.Rows[0]["FileName"].ToString()
                        : GenerateFileName(reportId.ToString(), sampleId, reportName, documentNumber, ".xlsx", false, 0);
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Open(templatePath);
                    IWorksheet worksheet = workbook.Worksheets[0];


                    void SetLabelValuePair(int row, string label1, string value1, string label2, string value2)
                    {
                        worksheet.Range[row, 1].Text = label1;
                        worksheet.Range[row, 1].CellStyle.Font.Bold = true;
                        worksheet.Range[row, 2].Text = value1;

                        worksheet.Range[row, 3].Text = label2;
                        worksheet.Range[row, 3].CellStyle.Font.Bold = true;
                        worksheet.Range[row, 4].Text = value2;
                    }

                    SetLabelValuePair(1, "#R", txtR.Text, "PIC", txtPIC.Text);
                    SetLabelValuePair(2, "Sample", txtsample.Text, "Temperature", txtRoomTempBegin.Text + " \u00B0C");
                    SetLabelValuePair(3, "#WO", txtwo.Text, "Humidity", txtHumidity.Text + " %RH");
                    SetLabelValuePair(4, "Analysis Starting Date", txtAnalysisStart.Text, "Tools", "Battery tester");
                    SetLabelValuePair(5, "Analysis Completion Date", txtCompleteDate.Text, "Calibration Date", "");

                    IRange dataRange = worksheet.Range["A1:D5"];
                    dataRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    dataRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    dataRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    dataRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    dataRange.CellStyle.Borders[ExcelBordersIndex.InsideHorizontal].LineStyle = ExcelLineStyle.Thin;
                    dataRange.CellStyle.Borders[ExcelBordersIndex.InsideVertical].LineStyle = ExcelLineStyle.Thin;

                    worksheet.Columns[0].ColumnWidth = 24;
                    worksheet.Columns[1].ColumnWidth = 16;
                    worksheet.Columns[2].ColumnWidth = 20;
                    worksheet.Columns[3].ColumnWidth = 16;

                    Directory.CreateDirectory(Server.MapPath("~/Temp/"));
                    string tempPath = Server.MapPath($"~/Temp/temp_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                    workbook.SaveAs(tempPath);
                    workbook.Close();
                    byte[] fileBytes = File.ReadAllBytes(tempPath);

                    Response.Clear();
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={downloadFileName}");
                    Response.AddHeader("Content-Length", fileBytes.Length.ToString());
                    Response.BinaryWrite(fileBytes);
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    File.Delete(tempPath);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error downloading Excel: {ex.Message}");
            }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileUpload.HasFile) { ShowMessage("Please select a file to upload."); return; }

                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls") { ShowMessage("Please upload a valid Excel file (.xlsx or .xls)"); return; }

                int sampleId = 0, reportIdInt = 0;

                if (ViewState["SampleID"] != null) sampleId = Convert.ToInt32(ViewState["SampleID"]);
                else if (Request.QueryString["s"] != null) int.TryParse(Request.QueryString["s"], out sampleId);

                if (ViewState["ReportID"] != null) reportIdInt = Convert.ToInt32(ViewState["ReportID"]);
                else if (Request.QueryString["r"] != null) { if (!int.TryParse(Request.QueryString["r"], out reportIdInt)) { ShowMessage("Invalid Report ID."); return; } }

                DataTable dtReport = GetReportDetailsFromDatabase(reportIdInt);
                string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";

                string tempFilePath = Path.GetTempFileName();
                string tempExcelPath = Path.ChangeExtension(tempFilePath, fileExtension);
                fileUpload.SaveAs(tempExcelPath);

                // Each sub-table on the sheet becomes its own block, sized to that
                // block's own actual columns. No pass/fail row filtering is applied --
                // every row from the sheet is kept as-is. The placeholder
                // "ADD YOUR OWN TEST TABLES" row is always dropped, and the block that
                // duplicates the #R/Sample/#WO/PIC key-value header (already rendered
                // via {headerTable}) is skipped so it doesn't print twice.
                List<ExcelBlock> blocks = ReadExcelBlocks(tempExcelPath);

                ViewState["R"] = txtR.Text;
                ViewState["Sample"] = txtsample.Text;
                ViewState["RoomTempBegin"] = txtRoomTempBegin.Text;
                ViewState["Humidity"] = txtHumidity.Text;
                ViewState["PIC"] = txtPIC.Text;
                ViewState["WO"] = txtwo.Text;
                ViewState["AnalysisStart"] = txtAnalysisStart.Text;
                ViewState["CompleteDate"] = txtCompleteDate.Text;

                bool previewGenerated = GeneratePdfPreview(blocks, reportIdInt, sampleId, documentNumber);
                if (previewGenerated)
                    ShowMessage("PDF preview opened in new tab! Review and click 'Save Excel File' when ready.");

                if (File.Exists(tempExcelPath)) File.Delete(tempExcelPath);
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing file: {ex.Message}");
            }
        }

        private string GenerateFileName(string reportId, int sampleId, string reportName, string documentNumber, string extension, bool isPreview, int revNumber = 0)
        {
            string safereportName = reportName.Replace(" ", " ");
            string prefix = isPreview ? "preview_" : "";
            if (revNumber > 0)
                return $"{prefix}{reportId}_{sampleId}_{safereportName}_rev{revNumber:D2}{extension}";
            else
                return $"{prefix}{reportId}_{sampleId}_{safereportName}{extension}";
        }

       
        private bool GeneratePdfPreview(List<ExcelBlock> blocks, int reportId, int sampleId, string documentNumber)
        {
            try
            {
                string templatePath = Server.MapPath("~/InputFiles/Battery.docx");
                if (!File.Exists(templatePath)) { ShowMessage("Template not found."); return false; }

                string pdfFileName = $"Preview_{reportId}_{sampleId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string previewDir = Server.MapPath("~/PreviewFiles_pv/");
                Directory.CreateDirectory(previewDir);
                string pdfBrowserPath = Path.Combine(previewDir, pdfFileName);

                string tempDocxPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".docx");
                File.Copy(templatePath, tempDocxPath, true);

                ReplaceAllPlaceholdersOpenXml(tempDocxPath, blocks, reportId);

                ConvertDocxToPdfSyncfusion(tempDocxPath, pdfBrowserPath);

                if (File.Exists(tempDocxPath)) File.Delete(tempDocxPath);

                string url = ResolveUrl("~/PreviewFiles_pv/" + pdfFileName);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openPdfPreview",
                    $"window.open('{url}', '_blank');", true);

                Session["PdfPreviewPath"] = pdfBrowserPath;
                return true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating preview: {ex.Message}");
                return false;
            }
        }

        // =====================================================================
        // FINAL PDF — same flow as preview but saved to upload folder
        // =====================================================================
        private bool GenerateFinalPdf(List<ExcelBlock> blocks, int reportId, int sampleId, string outputPdfPath)
        {
            try
            {
                string templatePath = Server.MapPath("~/InputFiles/Calibration.docx");
                if (!File.Exists(templatePath)) { ShowMessage("Template not found."); return false; }

                string tempDocxPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".docx");
                File.Copy(templatePath, tempDocxPath, true);

                ReplaceAllPlaceholdersOpenXml(tempDocxPath, blocks, reportId);
                ConvertDocxToPdfSyncfusion(tempDocxPath, outputPdfPath);

                if (File.Exists(tempDocxPath)) File.Delete(tempDocxPath);
                return true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating PDF: {ex.Message}");
                return false;
            }
        }

        // =====================================================================
        // CORE: Replace placeholders in docx using OpenXML
        // =====================================================================
        private void ReplaceAllPlaceholdersOpenXml(string docxPath, List<ExcelBlock> blocks, int reportId)
        {
            DataTable dtReport = GetReportDetailsFromDatabase(reportId);
            string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";
            string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(docxPath, true))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;

                foreach (var headerPart in wordDoc.MainDocumentPart.HeaderParts)
                {
                    ReplaceTextInElement(headerPart.Header, "{docno}", documentNumber);
                    ReplaceTextInElement(headerPart.Header, "{reportname}", reportName);
                    headerPart.Header.Save();
                }

                ReplacePlaceholderWithTable(wordDoc, body, "{headerTable}");

                // Each block becomes its OWN table, separated by a spacer paragraph,
                // so Word does not visually merge unrelated sub-tables into one.
                ReplacePlaceholderWithExcelTables(wordDoc, body, "{MainResultTable}", blocks, reportId);

                wordDoc.MainDocumentPart.Document.Save();
            }
        }

        private void ReplaceTextInElement(OpenXmlElement element, string placeholder, string value)
        {
            foreach (var para in element.Descendants<Paragraph>())
            {
                string fullText = string.Concat(para.Descendants<Text>().Select(t => t.Text));
                if (fullText.Contains(placeholder))
                {
                    var runs = para.Descendants<Run>().ToList();
                    if (runs.Any())
                    {
                        runs[0].GetFirstChild<Text>().Text = fullText.Replace(placeholder, value);
                        runs[0].GetFirstChild<Text>().Space = SpaceProcessingModeValues.Preserve;
                        for (int i = 1; i < runs.Count; i++)
                        {
                            var t = runs[i].GetFirstChild<Text>();
                            if (t != null) t.Text = "";
                        }
                    }
                }
            }
        }
        private void ReplacePlaceholderWithTable(WordprocessingDocument wordDoc, Body body, string placeholder)
        {
            Paragraph targetPara = body.Descendants<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains(placeholder));

            if (targetPara == null) return;

            // Matches the real template layout exactly: 4 rows, 2 columns,
            // each cell = "Label : value" on one line. No Tools / Calibration Date.
            var rows = new (string Label1, string Value1, string Label2, string Value2)[]
            {
        ("#R",     ViewState["R"]?.ToString() ?? "",
         "Temperature", (ViewState["RoomTempBegin"]?.ToString() ?? "") + " \u00B0C"),

        ("Sample", ViewState["Sample"]?.ToString() ?? "",
         "Humidity", (ViewState["Humidity"]?.ToString() ?? "") + " %RH"),

        ("#WO",    ViewState["WO"]?.ToString() ?? "",
         "Start date", ViewState["AnalysisStart"]?.ToString() ?? ""),

        ("PIC",    ViewState["PIC"]?.ToString() ?? "",
         "Completion date", ViewState["CompleteDate"]?.ToString() ?? ""),
            };

            Table table = BuildBorderedTable();

            foreach (var (label1, value1, label2, value2) in rows)
            {
                TableRow tr = new TableRow();
                tr.Append(
                    MakeLabelValueCell(label1, value1),
                    MakeLabelValueCell(label2, value2)
                );
                table.AppendChild(tr);
            }

            body.InsertBefore(table, targetPara);
            body.RemoveChild(targetPara);
        }

        // One cell: bold "Label : " followed by the value, same line
        private TableCell MakeLabelValueCell(string label, string value)
        {
            return new TableCell(
                new TableCellProperties(new TableCellWidth { Width = "5000", Type = TableWidthUnitValues.Pct }),
                new Paragraph(
                    new Run(
                        new RunProperties(new Bold()),
                        new Text(label + " : ") { Space = SpaceProcessingModeValues.Preserve }
                    ),
                    new Run(
                        new Text(value) { Space = SpaceProcessingModeValues.Preserve }
                    )
                )
            );
        }

        private Table BuildBorderedTable()
        {
            Table table = new Table();
            TableProperties tblProps = new TableProperties(
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                )
            );
            table.AppendChild(tblProps);
            return table;
        }

        private void ReplacePlaceholderWithExcelTables(WordprocessingDocument wordDoc, Body body,
            string placeholder, List<ExcelBlock> blocks, int reportId)
        {
            Paragraph targetPara = body.Descendants<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains(placeholder));

            if (targetPara == null) return;

            if (blocks == null || blocks.Count == 0)
            {
                body.RemoveChild(targetPara);
                return;
            }

            OpenXmlElement anchor = targetPara;

            foreach (ExcelBlock eb in blocks)
            {
                DataTable dt = eb.Data;
                if (dt.Rows.Count == 0) continue;

                Table table = BuildBorderedTable();

                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    DataRow dr = dt.Rows[r];
                    int[] rowSpans = eb.ColSpans[r];
                    TableRow tr = new TableRow();

                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        int span = rowSpans[c];
                        if (span == 0) continue; // 

                        var cellProps = new TableCellProperties();
                        if (span > 1)
                            cellProps.Append(new GridSpan { Val = span });

                        TableCell cell = new TableCell(
                            cellProps,
                            new Paragraph(new Run(
                                new RunProperties(new FontSize { Val = "20" }),
                                new Text(dr[c]?.ToString() ?? "") { Space = SpaceProcessingModeValues.Preserve }
                            ))
                        );
                        tr.AppendChild(cell);
                    }
                    table.AppendChild(tr);
                }

                body.InsertAfter(table, anchor);
                anchor = table;

                // Any pictures from this block's rows in Excel get inserted here,
                // right after this block's table.
                if (eb.Images != null)
                {
                    foreach (byte[] imgBytes in eb.Images)
                    {
                        Paragraph imgPara = InsertImageParagraph(wordDoc, imgBytes);
                        body.InsertAfter(imgPara, anchor);
                        anchor = imgPara;
                    }
                }

                // Spacer paragraph -- required so this table and the next one
                // are not treated by Word as a single merged table.
                Paragraph spacer = new Paragraph();
                body.InsertAfter(spacer, anchor);
                anchor = spacer;
            }

            body.RemoveChild(targetPara);
        }

        // =====================================================================
        // Embeds a PNG image (byte[]) into the docx and returns a paragraph
        // containing it, sized to fit the page width (capped at 400px wide,
        // scaled proportionally so it never overflows the page).
        // =====================================================================
        private Paragraph InsertImageParagraph(WordprocessingDocument wordDoc, byte[] imageBytes)
        {
            // Normalize to PNG regardless of source format (jpg/png/etc.) so the
            // declared ImagePartType.Png below always matches the actual bytes.
            byte[] pngBytes;
            int origWidth, origHeight;
            using (var srcMs = new MemoryStream(imageBytes))
            using (var srcImg = System.Drawing.Image.FromStream(srcMs))
            using (var pngMs = new MemoryStream())
            {
                origWidth = srcImg.Width;
                origHeight = srcImg.Height;
                srcImg.Save(pngMs, System.Drawing.Imaging.ImageFormat.Png);
                pngBytes = pngMs.ToArray();
            }

            ImagePart imagePart = wordDoc.MainDocumentPart.AddImagePart(ImagePartType.Png);
            using (var ms = new MemoryStream(pngBytes))
            {
                imagePart.FeedData(ms);
            }
            string relationshipId = wordDoc.MainDocumentPart.GetIdOfPart(imagePart);

            double maxWidthPx = 400;
            double scale = origWidth > maxWidthPx ? maxWidthPx / origWidth : 1.0;
            double widthPx = origWidth * scale;
            double heightPx = origHeight * scale;
            // 1 pixel = 9525 EMUs
            long widthEmu = (long)(widthPx * 9525);
            long heightEmu = (long)(heightPx * 9525);

            var element = new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent { Cx = widthEmu, Cy = heightEmu },
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties { Id = 1, Name = "Picture" },
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                    new DocumentFormat.OpenXml.Drawing.GraphicFrameLocks { NoChangeAspect = true }),
                new DocumentFormat.OpenXml.Drawing.Graphic(
                    new DocumentFormat.OpenXml.Drawing.GraphicData(
                        new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                            new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties { Id = 0, Name = "img.png" },
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()),
                            new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                new DocumentFormat.OpenXml.Drawing.Blip { Embed = relationshipId },
                                new DocumentFormat.OpenXml.Drawing.Stretch(new DocumentFormat.OpenXml.Drawing.FillRectangle())),
                            new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                new DocumentFormat.OpenXml.Drawing.Transform2D(
                                    new DocumentFormat.OpenXml.Drawing.Offset { X = 0, Y = 0 },
                                    new DocumentFormat.OpenXml.Drawing.Extents { Cx = widthEmu, Cy = heightEmu }),
                                new DocumentFormat.OpenXml.Drawing.PresetGeometry(
                                    new DocumentFormat.OpenXml.Drawing.AdjustValueList())
                                { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle }))
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            )
            { DistanceFromTop = 0, DistanceFromBottom = 0, DistanceFromLeft = 0, DistanceFromRight = 0 };

            var drawing = new Drawing(element);
            return new Paragraph(new Run(drawing));
        }

        private void ConvertDocxToPdfSyncfusion(string docxPath, string pdfOutputPath)
        {
            using (FileStream docStream = new FileStream(docxPath, FileMode.Open, FileAccess.Read))
            {
                using (WordDocument wordDocument = new WordDocument(docStream, FormatType.Docx))
                {
                    DocIORenderer renderer = new DocIORenderer();
                    using (PdfDocument pdfDoc = renderer.ConvertToPDF(wordDocument))
                    {
                        using (FileStream pdfStream = new FileStream(pdfOutputPath, FileMode.Create, FileAccess.Write))
                        {
                            pdfDoc.Save(pdfStream);
                        }
                    }
                }
            }
        }

        // =====================================================================
        // Excel reader — Syncfusion XlsIO (no ClosedXML anywhere).
        //
        // Splits the sheet into BLOCKS: a block is the run of non-blank rows
        // between two blank rows, further split whenever a new bold header row
        // starts (unless the row above it was already part of the same bold
        // header run -- this keeps multi-row headers collapsed into a single
        // table instead of one table per row).
        //
        // Each block is sized to ONLY the columns it actually uses (not the
        // sheet's overall last column).
        //
        // Merged Excel cells are detected and carried through as a colspan
        // grid (ExcelBlock.ColSpans) so the generated Word table can render
        // them as a single merged cell via GridSpan instead of splitting
        // them into blank separate cells.
        //
        //   - the "<< ADD YOUR OWN TEST TABLES BELOW >>" placeholder row is
        //     always skipped, wherever it appears.
        //   - NO pass/fail row filtering is applied -- every row is kept
        //     exactly as typed in Excel. "Yes"/"No" is literal cell text,
        //     not a filter condition.
        //   - the block that duplicates the #R / Sample / #WO / PIC key-value
        //     header (already rendered once via {headerTable}) is skipped so
        //     it doesn't get printed a second time from the Excel data.
        // =====================================================================
        // =====================================================================
        // Reads embedded pictures directly from the .xlsx file using OpenXml --
        // the same DocumentFormat.OpenXml assembly already referenced for the
        // Word side of this page. No Syncfusion picture API, no dynamic, no
        // new package needed.
        //
        // Returns a map of 0-based Excel row -> list of PNG/JPEG bytes for
        // every picture anchored to that row.
        // =====================================================================
        private Dictionary<int, List<byte[]>> ReadPicturesByRow(string filePath)
        {
            var result = new Dictionary<int, List<byte[]>>();

            using (var doc = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(filePath, false))
            {
                var workbookPart = doc.WorkbookPart;

                var firstSheet = workbookPart.Workbook.Sheets
                    .Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>()
                    .FirstOrDefault();

                if (firstSheet == null || firstSheet.Id?.Value == null)
                    return result;

                var wsPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)
                    workbookPart.GetPartById(firstSheet.Id.Value);

                var drawingsPart = wsPart?.DrawingsPart;
                if (drawingsPart?.WorksheetDrawing == null)
                    return result;

                void AddPicture(int row, string embedId)
                {
                    if (string.IsNullOrEmpty(embedId)) return;
                    var imagePart = drawingsPart.GetPartById(embedId) as DocumentFormat.OpenXml.Packaging.ImagePart;
                    if (imagePart == null) return;

                    using (var stream = imagePart.GetStream())
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        byte[] bytes = ms.ToArray();
                        if (!result.TryGetValue(row, out var list))
                        {
                            list = new List<byte[]>();
                            result[row] = list;
                        }
                        list.Add(bytes);
                    }
                }

                foreach (var anchor in drawingsPart.WorksheetDrawing.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.TwoCellAnchor>())
                {
                    var fromMarker = anchor.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker>().FirstOrDefault();
                    var blip = anchor.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();
                    if (fromMarker?.RowId == null || blip == null) continue;
                    AddPicture(int.Parse(fromMarker.RowId.Text), blip.Embed?.Value);
                }

                foreach (var anchor in drawingsPart.WorksheetDrawing.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.OneCellAnchor>())
                {
                    var fromMarker = anchor.Elements<DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker>().FirstOrDefault();
                    var blip = anchor.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();
                    if (fromMarker?.RowId == null || blip == null) continue;
                    AddPicture(int.Parse(fromMarker.RowId.Text), blip.Embed?.Value);
                }
            }

            return result;
        }
        private List<ExcelBlock> ReadExcelBlocks(string filePath)
        {
            var blocks = new List<ExcelBlock>();
            if (!File.Exists(filePath)) { ShowMessage($"File not found: {filePath}"); return blocks; }

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Open(filePath);
                IWorksheet worksheet = workbook.Worksheets[0];

                int lastRow = worksheet.UsedRange.LastRow;
                int lastCol = worksheet.UsedRange.LastColumn;

                if (lastRow == 0 || lastCol == 0)
                {
                    ShowMessage("Excel sheet appears to be empty.");
                    workbook.Close();
                    return blocks;
                }

                var allVals = new List<string[]>();
                var allBold = new List<bool>();
                var allEmpty = new List<bool>();
                var allSpans = new List<int[]>();

                var picturesByRow = ReadPicturesByRow(filePath);

                if (picturesByRow.Count > 0)
                {
                    int maxPicRow = picturesByRow.Keys.Max() + 1;
                    if (maxPicRow > lastRow) lastRow = maxPicRow;
                }

                for (int row = 1; row <= lastRow; row++)
                {
                    var vals = new string[lastCol];
                    var spans = new int[lastCol];
                    bool isEmptyRow = true;

                    for (int col = 1; col <= lastCol; col++)
                    {
                        IRange cell = worksheet.Range[row, col];
                        string v = (cell.DisplayText ?? "").Trim();
                        vals[col - 1] = v;
                        if (!string.IsNullOrWhiteSpace(v)) isEmptyRow = false;

                        if (cell.IsMerged)
                        {
                            IRange mergeArea = cell.MergeArea;
                            bool isMergeStart = (mergeArea.Row == row && mergeArea.Column == col);
                            spans[col - 1] = isMergeStart ? (mergeArea.LastColumn - mergeArea.Column + 1) : 0;
                        }
                        else
                        {
                            spans[col - 1] = 1;
                        }
                    }

                    bool isBold = worksheet.Range[row, 1].CellStyle.Font.Bold;

                    allVals.Add(vals);
                    allSpans.Add(spans);
                    allBold.Add(isBold);
                    allEmpty.Add(isEmptyRow);
                }

                workbook.Close();

                var blockRowIndexes = new List<List<int>>();
                List<int> currentBlockRows = null;
                bool prevRowWasSectionHeader = false;

                for (int i = 0; i < allVals.Count; i++)
                {
                    var vals = allVals[i];

                    if (allEmpty[i])
                    {
                        currentBlockRows = null;
                        prevRowWasSectionHeader = false;
                        continue;
                    }

                    if (vals[0].IndexOf("ADD YOUR OWN TEST TABLES", StringComparison.OrdinalIgnoreCase) >= 0)
                        continue;

                    bool looksLikeSectionHeader = allBold[i];
                    bool startsNewBlock = currentBlockRows == null || (looksLikeSectionHeader && !prevRowWasSectionHeader);
                    prevRowWasSectionHeader = looksLikeSectionHeader;

                    if (startsNewBlock)
                    {
                        currentBlockRows = new List<int>();
                        blockRowIndexes.Add(currentBlockRows);
                    }

                    currentBlockRows.Add(i);
                }

                var extendedStarts = new List<int>();
                var extendedEnds = new List<int>();
                for (int b = 0; b < blockRowIndexes.Count; b++)
                {
                    int start = (b == 0) ? 0 : blockRowIndexes[b][0];
                    int end = (b + 1 < blockRowIndexes.Count)
                        ? blockRowIndexes[b + 1][0] - 1
                        : allVals.Count - 1;
                    extendedStarts.Add(start);
                    extendedEnds.Add(end);
                }

                var carriedOverImages = new List<byte[]>();

                for (int b = 0; b < blockRowIndexes.Count; b++)
                {
                    var rowIdxs = blockRowIndexes[b];
                    if (rowIdxs.Count == 0) continue;

                    string firstCellOfBlock = allVals[rowIdxs[0]][0]?.Trim() ?? "";

                    int blockFirstRow = extendedStarts[b];
                    int blockLastRow = extendedEnds[b];
                    var imagesInThisRange = new List<byte[]>();
                    foreach (var kvp in picturesByRow)
                    {
                        if (kvp.Key >= blockFirstRow && kvp.Key <= blockLastRow)
                            imagesInThisRange.AddRange(kvp.Value);
                    }

                    if (string.Equals(firstCellOfBlock, "#R", StringComparison.OrdinalIgnoreCase))
                    {
                        carriedOverImages.AddRange(imagesInThisRange);
                        continue;
                    }

                    int blockWidth = 1;
                    foreach (int ri in rowIdxs)
                    {
                        var vals = allVals[ri];
                        for (int c = vals.Length - 1; c >= 0; c--)
                        {
                            if (!string.IsNullOrWhiteSpace(vals[c])) { blockWidth = Math.Max(blockWidth, c + 1); break; }
                        }
                    }

                    DataTable dt = new DataTable();
                    for (int c = 1; c <= blockWidth; c++)
                        dt.Columns.Add("Col" + c);

                    var colSpansForBlock = new List<int[]>();

                    foreach (int ri in rowIdxs)
                    {
                        var vals = allVals[ri];
                        var spans = allSpans[ri];

                        DataRow dr = dt.NewRow();
                        var rowSpans = new int[blockWidth];
                        for (int c = 0; c < blockWidth; c++)
                        {
                            dr[c] = vals[c];
                            rowSpans[c] = spans[c];
                        }
                        dt.Rows.Add(dr);
                        colSpansForBlock.Add(rowSpans);
                    }

                    var imagesForBlock = new List<byte[]>();
                    imagesForBlock.AddRange(carriedOverImages);
                    carriedOverImages.Clear();
                    imagesForBlock.AddRange(imagesInThisRange);

                    if (dt.Rows.Count > 0)
                        blocks.Add(new ExcelBlock { Data = dt, ColSpans = colSpansForBlock, Images = imagesForBlock });
                }

                if (carriedOverImages.Count > 0 && blocks.Count > 0)
                {
                    blocks[blocks.Count - 1].Images.AddRange(carriedOverImages);
                }
            }

            return blocks;
        }
        protected void btnConfirmUpload_Click(object sender, EventArgs e)
        {
            try
            {
                btnUpdateUpload.Visible = false;
                btnDownloadExisting.Visible = false;
                btnConfirmUpload.Visible = true;
                btnDownlaodNew.Visible = true;

                if (!fileUpload.HasFile) { ShowMessage("Please select a file to upload."); return; }
                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls") { ShowMessage("Please upload a valid Excel file."); return; }

                int sampleId = 0, reportIdInt = 0; string reportIdStr = "";
                if (ViewState["SampleID"] != null) sampleId = Convert.ToInt32(ViewState["SampleID"]);
                else if (Request.QueryString["s"] != null) int.TryParse(Request.QueryString["s"], out sampleId);
                if (ViewState["ReportID"] != null) { reportIdInt = Convert.ToInt32(ViewState["ReportID"]); reportIdStr = reportIdInt.ToString(); }
                else if (Request.QueryString["r"] != null) { reportIdStr = Request.QueryString["r"]; if (!int.TryParse(reportIdStr, out reportIdInt)) { ShowMessage("Invalid Report ID."); return; } }

                DataTable dtReport = GetReportDetailsFromDatabase(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";
                string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";

                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;
                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);
                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                // Check the SAME table we save to (AddTestResultPV -> GetExistingTestResult)
                DataTable dtExistCheck = _cm.GetExistingTestResult(reportIdInt, sampleId);
                bool fileExistsInDB = (dtExistCheck != null && dtExistCheck.Rows.Count > 0
                                       && !string.IsNullOrEmpty(dtExistCheck.Rows[0]["Result"]?.ToString()));

                string finalFileName = !fileExistsInDB
                    ? GenerateFileName(reportIdStr, sampleId, reportName, documentNumber, fileExtension, false, 0)
                    : GenerateFileName(reportIdStr, sampleId, reportName, documentNumber, fileExtension, false, GetNextRevisionNumber(reportIdStr, sampleId, reportName, documentNumber));

                string uploadFolderPath = Server.MapPath("~/FilesUpload/");
                string finalFilePath = Path.Combine(uploadFolderPath, finalFileName);
                Directory.CreateDirectory(uploadFolderPath);
                if (File.Exists(finalFilePath)) { ShowMessage("File already exists with this name!"); return; }
                fileUpload.SaveAs(finalFilePath);

                List<ExcelBlock> blocks = ReadExcelBlocks(finalFilePath);
                if (blocks == null || blocks.Count == 0) { ShowMessage("No data found in Excel file."); if (File.Exists(finalFilePath)) File.Delete(finalFilePath); return; }

                if (_cm == null) _cm = new DAL.CM();
                _cm.sampleId = sampleId;
                _cm.ReportID = reportIdStr;

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }
                else
                {
                    _cm.sampleIDSeries = "smp/2001";
                }

                ViewState["R"] = txtR.Text;
                ViewState["Sample"] = txtsample.Text;
                ViewState["RoomTempBegin"] = txtRoomTempBegin.Text;
                ViewState["Humidity"] = txtHumidity.Text;
                ViewState["PIC"] = txtPIC.Text;
                ViewState["WO"] = txtwo.Text;
                ViewState["AnalysisStart"] = txtAnalysisStart.Text;
                ViewState["CompleteDate"] = txtCompleteDate.Text;

                _cm.RoomTemperatureBegining = txtRoomTempBegin.Text;
                _cm.HumidityBegining = txtHumidity.Text;

                string pdfFileName = Path.ChangeExtension(finalFileName, ".pdf");
                string pdfFullPath = Path.Combine(uploadFolderPath, pdfFileName);
                bool pdfGenerated = GenerateFinalPdf(blocks, reportIdInt, sampleId, pdfFullPath);

                if (!pdfGenerated) { ShowMessage("Excel saved but PDF generation failed."); if (File.Exists(finalFilePath)) File.Delete(finalFilePath); return; }

                string relativeExcelPath = "~/FilesUpload/" + finalFileName;
                string relativePdfPath = "~/FilesUpload/" + pdfFileName;

                int savedCount = _cm.AddTestResultPV(_cm, finalFileName, relativeExcelPath, relativePdfPath, AnalysisStartingDate, AnalysisCompletionDate);

                if (savedCount > 0)
                {
                    _cm.sampleStatusID = 27;
                    _cm.StatusID = 27;

                    btnConfirmUpload.Visible = false;
                    string message = fileExistsInDB ? "Revision saved successfully!" : "Test results saved successfully";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", $"alert('{message}');window.location='WorkList.aspx';", true);
                }
                else
                {
                    if (File.Exists(finalFilePath)) File.Delete(finalFilePath);
                    if (File.Exists(pdfFullPath)) File.Delete(pdfFullPath);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save test results.');", true);
                }
            }
            catch (Exception ex) { ShowMessage($"Error confirming upload: {ex.Message}"); }
        }
        protected void btnUpdateUpload_Click(object sender, EventArgs e)
        {
            btnUpdateUpload.Visible = true;
            btnDownloadExisting.Visible = true;
            btnConfirmUpload.Visible = false;
            btnDownlaodNew.Visible = false;

            try
            {
                if (!fileUpload.HasFile) { ShowMessage("Please select a file to upload."); return; }
                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls") { ShowMessage("Please upload a valid Excel file."); return; }

                int sampleId = 0, reportIdInt = 0; string reportIdStr = "";
                if (ViewState["SampleID"] != null) sampleId = Convert.ToInt32(ViewState["SampleID"]);
                else if (Request.QueryString["s"] != null) int.TryParse(Request.QueryString["s"], out sampleId);
                if (ViewState["ReportID"] != null) { reportIdInt = Convert.ToInt32(ViewState["ReportID"]); reportIdStr = reportIdInt.ToString(); }
                else if (Request.QueryString["r"] != null) { reportIdStr = Request.QueryString["r"]; if (!int.TryParse(reportIdStr, out reportIdInt)) { ShowMessage("Invalid Report ID."); return; } }

                DataTable dtReport = GetReportDetailsFromDatabase(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";
                string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";

                int revNumber = GetNextRevisionNumber(reportIdStr, sampleId, reportName, documentNumber);
                string finalFileName = GenerateFileName(reportIdStr, sampleId, reportName, documentNumber, fileExtension, false, revNumber);

                string uploadFolderPath = Server.MapPath("~/FilesUpload/");
                string finalFilePath = Path.Combine(uploadFolderPath, finalFileName);
                Directory.CreateDirectory(uploadFolderPath);
                if (File.Exists(finalFilePath)) { ShowMessage("File already exists. Try again."); return; }
                fileUpload.SaveAs(finalFilePath);

                List<ExcelBlock> blocks = ReadExcelBlocks(finalFilePath);
                if (blocks == null || blocks.Count == 0) { ShowMessage("No data found in Excel file."); if (File.Exists(finalFilePath)) File.Delete(finalFilePath); return; }

                if (_cm == null) _cm = new DAL.CM();
                _cm.sampleId = sampleId;
                _cm.ReportID = reportIdStr;

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                _cm.sampleIDSeries = dt1.Rows.Count > 0 ? dt1.Rows[0]["sampleIDSeries"].ToString() : "smp/2001";
                if (dt1.Rows.Count > 0) _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);

                ViewState["R"] = txtR.Text;
                ViewState["Sample"] = txtsample.Text;
                ViewState["RoomTempBegin"] = txtRoomTempBegin.Text;
                ViewState["Humidity"] = txtHumidity.Text;
                ViewState["PIC"] = txtPIC.Text;
                ViewState["WO"] = txtwo.Text;
                ViewState["AnalysisStart"] = txtAnalysisStart.Text;
                ViewState["CompleteDate"] = txtCompleteDate.Text;

                _cm.RoomTemperatureBegining = txtRoomTempBegin.Text;
                _cm.HumidityBegining = txtHumidity.Text;

                string pdfFileName = Path.ChangeExtension(finalFileName, ".pdf");
                string pdfFullPath = Path.Combine(uploadFolderPath, pdfFileName);
                bool pdfGenerated = GenerateFinalPdf(blocks, reportIdInt, sampleId, pdfFullPath);

                if (!pdfGenerated) { ShowMessage("Excel saved but PDF generation failed."); if (File.Exists(finalFilePath)) File.Delete(finalFilePath); return; }

                string relativeExcelPath = "~/FilesUpload/" + finalFileName;
                string relativePdfPath = "~/FilesUpload/" + pdfFileName;
                int updatedCount = _cm.UpdateAdditionalRecordDetails(_cm, relativeExcelPath, relativePdfPath);

                if (updatedCount > 0)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Revision updated successfully!');window.location='WorkList_PV.aspx';", true);
                else
                {
                    if (File.Exists(finalFilePath)) File.Delete(finalFilePath);
                    if (File.Exists(pdfFullPath)) File.Delete(pdfFullPath);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to update database.');", true);
                }
            }
            catch (Exception ex) { ShowMessage($"Error: {ex.Message}"); }
        }

        private int GetNextRevisionNumber(string reportId, int sampleId, string productName, string documentNumber)
        {
            int nextRev = 1;
            string folderPath = Server.MapPath("~/FilesUpload/");
            string safeProductName = productName.Replace(" ", "_");
            string basePattern = $"{reportId}_{sampleId}_{safeProductName}";

            var allFiles = Directory.GetFiles(folderPath, "*.xls*")
                                    .Where(f => Path.GetFileName(f).StartsWith(basePattern)).ToList();
            foreach (string file in allFiles)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                var match = Regex.Match(filename, @"_rev(\d{2})$");
                if (match.Success) { int rev = int.Parse(match.Groups[1].Value); if (rev >= nextRev) nextRev = rev + 1; }
                else if (filename == basePattern) nextRev = Math.Max(nextRev, 1);
            }
            return nextRev;
        }

        private bool CheckIfFileExistsInDatabase(string reportId, int sampleId)
        {
            try
            {
                if (_cm == null) _cm = new DAL.CM();
                DataTable dt = _cm.GetExistingTestResultCalibration(Convert.ToInt32(reportId), sampleId);
                return (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["Result"]?.ToString()));
            }
            catch { return false; }
        }

        private void ShowMessage(string message)
        {
            message = message.Replace("'", "\\'");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", $"alert('{message}');", true);
        }
    }
}