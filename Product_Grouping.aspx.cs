using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Bibliography;
using LIMS.DAL;
using System.Linq.Expressions;
using System.Text;

namespace LIMS
{
    public partial class Product_Grouping : System.Web.UI.Page
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        DAL.CM _cm = new DAL.CM();
        private int reportId = 0;
        private int sampleId = 0;
        private string sampleIdSeries = "smp/2001";
        private string uploadFolder = "~/UploadedExcelFiles/";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                // Get ReportID and SampleID from query string
                if (Request.QueryString["r"] != null)
                {
                    string encodedString1 = Request.QueryString["r"].ToString();
                    byte[] decodedBytes1 = Convert.FromBase64String(encodedString1);
                    string decodedEventListId1 = Encoding.UTF8.GetString(decodedBytes1);
                    reportId = Convert.ToInt32(decodedEventListId1);
                    ViewState["ReportID"] = reportId;
                }

                if (Request.QueryString["s"] != null)
                {

                    string encodedString = Request.QueryString["s"].ToString();
                    byte[] decodedBytes = Convert.FromBase64String(encodedString);
                    string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);
                    sampleId = Convert.ToInt32(decodedEventListId);
                    ViewState["SampleID"] = sampleId;
                }

                // Ensure upload folder exists
                string physicalPath = Server.MapPath(uploadFolder);
                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }
                _cm.sampleId = sampleId;
                _cm.ReportID = reportId.ToString();
                DataTable dt1 = _cm.Getdataforseriesandreportid_PV(_cm);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    //// Set dynamic values from database
                    txtbrand.Text = dt1.Rows[0]["BrandProduct"].ToString();

                    txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"].ToString();
                    txttestmethod.Text = dt1.Rows[0]["TestMethod"].ToString();


                    txtR.Text = "R/2024110019";
                    txtManufacturer.Text = "Haitai Power Technology";

                    txtpic.Text = "AD AJ DD";

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
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
                reportId = Convert.ToInt32(ViewState["ReportID"]);
                sampleId = Convert.ToInt32(ViewState["SampleID"]);
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
                    string fileName = dtExisting.Rows[0]["FileName"].ToString();
                    string excelPath = dtExisting.Rows[0]["Result"]?.ToString();

                    System.Diagnostics.Debug.WriteLine($"Database FileName: {fileName}");
                    System.Diagnostics.Debug.WriteLine($"Database ExcelPath: {excelPath}");

                    if (string.IsNullOrEmpty(excelPath))
                    {
                        ShowMessage("Excel path not found in database. Please upload a new file.");
                        return;
                    }

                    // Build full physical path
                    string fullExcelPath = Server.MapPath(excelPath);

                    if (File.Exists(fullExcelPath))
                    {
                        // Read the existing Excel file
                        DataTable dt = ReadExcelToDataTable(fullExcelPath);

                        // Populate form fields from Excel
                        using (var workbook = new XLWorkbook(fullExcelPath))
                        {
                            var ws = workbook.Worksheet(1);

                            txtbrand.Text = ws.Cell("B1").Value.ToString();
                            txtR.Text = ws.Cell("E1").Value.ToString();
                            txtManufacturer.Text = ws.Cell("B2").Value.ToString();
                            txtworkorder.Text = ws.Cell("E2").Value.ToString();
                            txttestmethod.Text = ws.Cell("B3").Value.ToString();
                            txtdate.Text = ws.Cell("E3").Value.ToString();
                            txtequipmentno.Text = ws.Cell("B4").Value.ToString();
                            txtpic.Text = ws.Cell("E4").Value.ToString();
                        }

                        // Store in ViewState
                        ViewState["Brand"] = txtbrand.Text;
                        ViewState["Manufacturer"] = txtManufacturer.Text;
                        ViewState["TestMethod"] = txttestmethod.Text;
                        ViewState["WorkOrder"] = txtworkorder.Text;
                        ViewState["Date"] = txtdate.Text;
                        ViewState["PIC"] = txtpic.Text;
                        ViewState["R"] = txtR.Text;
                        ViewState["EquipmentNo"] = txtequipmentno.Text;

                        // Extract document number and product name from report
                        DataTable dtReport = _cm.GetExcelFileNameByReportID(reportId);
                        string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"].ToString() : "";
                        string documentNumber = ExtractDocumentNumber(reportName);
                        string productName = ExtractProductName(reportName);

                        // Store in Session for later use
                        Session["ExcelData"] = dt;
                        Session["IsRetest"] = true;
                        Session["ExistingFileName"] = fileName;
                        Session["PreviewFilePath"] = fullExcelPath;
                        Session["DocumentNumber"] = documentNumber;
                        Session["ProductName"] = productName;

                        // Store the file path for download
                        ViewState["ExistingFilePath"] = excelPath;
                        ViewState["ExistingFileName"] = fileName;

                        btnDownloadExisting.Visible = true;
                        btnDownloadExisting.Text = $"Download Existing File: {fileName}";
                        btnUpdateUpload.Visible = true;  // This should be true for existing files
                        btnConfirmUpload.Visible = false; // This should be false for existing files
                        btnDownlaodNew.Visible = false;

                        // Show message to user with info about what will happen
                        ShowMessage($"Download the existing file "
                                );
                    }
                    else
                    {
                        ShowMessage($"Excel file not found at path: {excelPath}. Please upload a new file.");
                        InitializeFormFields();
                    }
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
                        // Set response headers for file download
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                        Response.TransmitFile(fullPath);
                        Response.Flush();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        ShowMessage($"File not found at: {fullPath}");
                    }
                }
                else
                {
                    ShowMessage("File information not available for download.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error downloading file: {ex.Message}");
            }
        }
        protected void InitializeFormFields()
        {
            // Initialize empty or default values
            txtbrand.Text = "";
            txtR.Text = "";
            txtManufacturer.Text = "";
            txtworkorder.Text = "";
            txttestmethod.Text = "";
            txtdate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtequipmentno.Text = "";
            txtpic.Text = "";
        }
        private string GetExcelFileNameFromDatabase(int reportId)
        {
            string excelFileName = "ReportTemplate"; // Default without extension

            DataTable dt = _cm.GetExcelFileNameByReportID(reportId);

            if (dt != null && dt.Rows.Count > 0)
            {
                excelFileName = dt.Rows[0]["ExcelFileName"].ToString();
            }

            // ADD .xlsx EXTENSION to filename from database
            if (!excelFileName.ToLower().EndsWith(".xlsx") && !excelFileName.ToLower().EndsWith(".xls"))
            {
                excelFileName = excelFileName + ".xlsx";
            }

            return excelFileName;
        }
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            btnDownlaodNew.Visible = true;
            btnUpdateUpload.Visible = false;
            btnDownloadExisting.Visible = false;
            try
            {
                btnDownlaodNew.Visible = true;
                btnConfirmUpload.Visible = true;
                btnUpdateUpload.Visible = false;
                btnDownloadExisting.Visible = false;

                // Retrieve reportId and sampleId from ViewState
                if (ViewState["ReportID"] != null)
                {
                    reportId = Convert.ToInt32(ViewState["ReportID"]);
                }


                if (ViewState["SampleID"] != null)
                {
                    sampleId = Convert.ToInt32(ViewState["SampleID"]);
                }


                string excelFileName = GetExcelFileNameFromDatabase(reportId);
                string templatePath = Server.MapPath($"~/InputFiles/{excelFileName}");

                if (!File.Exists(templatePath))
                {
                    ShowMessage($"Excel file '{excelFileName}' not found.");
                    return;
                }

                // Get ReportName from DB
                DataTable dtReport = _cm.GetExcelFileNameByReportID(reportId);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"].ToString() : "";

                // Extract document number and product name
                string documentNumber = ExtractDocumentNumber(reportName);
                string productName = ExtractProductName(reportName);

                // Check if file already exists in database (means it's already been downloaded/uploaded once)
                bool isOriginalDownload = !CheckIfFileExistsInDatabase(reportId.ToString(), sampleId);

                // Generate filename based on whether it's original or revision
                string downloadFileName;
                if (isOriginalDownload)
                {
                    // First time download - use base format (no revision number)
                    downloadFileName = GenerateFileName(reportId.ToString(), sampleId, productName, documentNumber, ".xlsx", false, 0);
                }
                else
                {
                    // Already exists - get the saved filename from database
                    DataTable dtExisting = _cm.GetExistingTestResult(reportId, sampleId);
                    if (dtExisting != null && dtExisting.Rows.Count > 0)
                    {
                        // Use the actual saved filename from database
                        downloadFileName = dtExisting.Rows[0]["FileName"].ToString();
                    }
                    else
                    {
                        // Fallback - use base format
                        downloadFileName = GenerateFileName(reportId.ToString(), sampleId, productName, documentNumber, ".xlsx", false, 0);
                    }
                }

                // Add form fields to template
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1);

                    // Insert 5 rows at top for form fields
                   //worksheet.Range("1:5").InsertRowsAbove(5);

                    // Add form fields with VALUES from textboxes
                    worksheet.Cell("A1").Value = "Brand";
                    worksheet.Cell("B1").Value = txtbrand.Text;
                    worksheet.Cell("C1").Value = "#R";
                    worksheet.Cell("D1").Value = txtR.Text;

                    worksheet.Cell("A2").Value = "Manufacturer";
                    worksheet.Cell("B2").Value = txtManufacturer.Text;
                    worksheet.Cell("C2").Value = "#WO";
                    worksheet.Cell("D2").Value = txtworkorder.Text;

                    worksheet.Cell("A3").Value = "Test Method No.";
                    worksheet.Cell("B3").Value = txttestmethod.Text;
                    worksheet.Cell("C3").Value = "Date";
                    worksheet.Cell("D3").Value = txtdate.Text;

                    worksheet.Cell("A4").Value = "Equipment No.";
                    worksheet.Cell("B4").Value = txtequipmentno.Text;
                    worksheet.Cell("C4").Value = "PIC";
                    worksheet.Cell("D4").Value = txtpic.Text;

                    // Style Label columns
                    var labelCells = new[] { "A1", "D1", "A2", "D2", "A3", "D3", "A4", "D4" };
                    foreach (var cell in labelCells)
                    {
                        worksheet.Cell(cell).Style.Font.Bold = true;
                        worksheet.Cell(cell).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    // Style Value columns
                    var valueCells = new[] { "B1", "E1", "B2", "E2", "B3", "E3", "B4", "E4" };
                    foreach (var cell in valueCells)
                    {
                        worksheet.Cell(cell).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    //// Auto-fit column widths
                    //worksheet.Column("A").Width = 18;
                    //worksheet.Column("B").Width = 25;
                    //worksheet.Column("C").Width = 15;
                    //worksheet.Column("D").Width = 12;
                    //worksheet.Column("E").Width = 25;
                    //worksheet.Column("F").Width = 5;
                    worksheet.Columns().AdjustToContents();

//                    if (worksheet.Column("A").Width < 18) worksheet.Column("A").Width = 18;
 //                   if (worksheet.Column("B").Width < 25) worksheet.Column("B").Width = 25;


                    string tempPath = Server.MapPath($"~/Temp/temp_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                    Directory.CreateDirectory(Server.MapPath("~/Temp/"));


                    workbook.SaveAs(tempPath);

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={downloadFileName}");
                    Response.TransmitFile(tempPath);
                    Response.Flush();
                    Response.End();

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
                if (!fileUpload.HasFile)
                {
                    ShowMessage("Please select a file to upload.");
                    return;
                }

                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    ShowMessage("Please upload a valid Excel file (.xlsx or .xls)");
                    return;
                }

                // Get IDs from QueryString
                int sampleId = 0;
                int reportIdInt = 0;

                // Get IDs from ViewState (fallback to QueryString if not available)
                if (ViewState["SampleID"] != null)
                    sampleId = Convert.ToInt32(ViewState["SampleID"]);
                else if (Request.QueryString["s"] != null)
                    int.TryParse(Request.QueryString["s"], out sampleId);

                if (ViewState["ReportID"] != null)
                    reportIdInt = Convert.ToInt32(ViewState["ReportID"]);
                else if (Request.QueryString["r"] != null)
                {
                    if (!int.TryParse(Request.QueryString["r"], out reportIdInt))
                    {
                        ShowMessage("Invalid Report ID.");
                        return;
                    }
                }

                // Get ReportName from DB
                DataTable dtReport = _cm.GetExcelFileNameByReportID(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"].ToString() : "";

                // Extract document number and product name
                string documentNumber = ExtractDocumentNumber(reportName);
                string productName = ExtractProductName(reportName);

                // REMOVED: Preview file saving - use temp file instead
                string tempFilePath = Path.GetTempFileName();
                string tempExcelPath = Path.ChangeExtension(tempFilePath, fileExtension);

                // Save to temp location
                fileUpload.SaveAs(tempExcelPath);

                // Read form fields and table from Excel
                DataTable dt = ReadExcelToDataTable(tempExcelPath);

                // Read and populate form fields from Excel
                using (var workbook = new XLWorkbook(tempExcelPath))
                {
                    var ws = workbook.Worksheet(1);

                    txtbrand.Text = ws.Cell("B1").Value.ToString();
                    txtR.Text = ws.Cell("D1").Value.ToString();
                    txtManufacturer.Text = ws.Cell("B2").Value.ToString();
                    txtworkorder.Text = ws.Cell("D2").Value.ToString();
                    txttestmethod.Text = ws.Cell("B3").Value.ToString();
                    txtdate.Text = ws.Cell("D3").Value.ToString();
                    txtequipmentno.Text = ws.Cell("B4").Value.ToString();
                    txtpic.Text = ws.Cell("D4").Value.ToString();
                }

                // Store in ViewState
                ViewState["Brand"] = txtbrand.Text;
                ViewState["Manufacturer"] = txtManufacturer.Text;
                ViewState["TestMethod"] = txttestmethod.Text;
                ViewState["WorkOrder"] = txtworkorder.Text;
                ViewState["Date"] = txtdate.Text;
                ViewState["PIC"] = txtpic.Text;
                ViewState["R"] = txtR.Text;
                ViewState["EquipmentNo"] = txtequipmentno.Text;

                // Generate PDF preview (will save to temp folder)
                bool previewGenerated = GeneratePdfPreview(dt, reportIdInt, sampleId, documentNumber);

                if (previewGenerated)
                {
                    ShowMessage("PDF preview opened in new tab! Review and click 'Save Excel File' when ready.");
                }

                // Clean up temp Excel file
                if (File.Exists(tempExcelPath))
                    File.Delete(tempExcelPath);

                // Also delete the original temp file
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing file: {ex.Message}");
            }
        }
        // Helper methods for filename generation
        private string ExtractDocumentNumber(string reportName)
        {
            if (string.IsNullOrEmpty(reportName))
                return "TSQ-LAB-PMD-000";

            try
            {
                // Pattern: TSQ-LAB-PMD-002_G1_...
                // Extract everything before the first underscore
                int firstUnderscore = reportName.IndexOf('_');

                if (firstUnderscore > 0)
                {
                    return reportName.Substring(0, firstUnderscore).Trim();
                }

                // Fallback: try regex
                var regex = new System.Text.RegularExpressions.Regex(@"(TSQ-LAB-PMD-\d+)");
                var match = regex.Match(reportName);

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }

                return "TSQ-LAB-PMD-000";
            }
            catch
            {
                return "TSQ-LAB-PMD-000";
            }
        }
        private string ExtractProductName(string reportName)
        {
            if (string.IsNullOrEmpty(reportName))
                return "UnknownProduct";

            try
            {
                // Pattern: TSQ-LAB-PMD-002_G1_Product Grouping (Rev.01) - HPTD 640
                // Find _GX_ pattern
                var regex = new System.Text.RegularExpressions.Regex(@"_G[\dA-Z]+_(.+)");
                var match = regex.Match(reportName);

                if (match.Success && match.Groups.Count > 1)
                {
                    string productName = match.Groups[1].Value.Trim();

                    // Check and remove " (Rev" pattern if it exists
                    int revIndex = productName.IndexOf(" (Rev");
                    if (revIndex > 0)
                        productName = productName.Substring(0, revIndex).Trim();

                    // Check and remove " - HPTD" pattern if it exists
                    int hptdIndex = productName.IndexOf(" - HPTD");
                    if (hptdIndex > 0)
                        productName = productName.Substring(0, hptdIndex).Trim();

                    return productName;
                }

                return "UnknownProduct";
            }
            catch
            {
                return "UnknownProduct";
            }
        }
        private string ExtractProductNameFallback(string reportName)
        {
            // Remove file extension if present
            string name = Path.GetFileNameWithoutExtension(reportName);

            // Remove " (Rev.XX)" pattern and everything after
            int revIndex = name.IndexOf(" (Rev.");
            if (revIndex > 0)
                name = name.Substring(0, revIndex).Trim();

            // Remove " - HPTD" pattern and everything after
            int hptdIndex = name.IndexOf(" - HPTD");
            if (hptdIndex > 0)
                name = name.Substring(0, hptdIndex).Trim();

            // Split by underscore and get last meaningful part
            string[] parts = name.Split('_');

            if (parts.Length >= 3)
            {
                // Return the part after the group identifier
                // Format: XXX_GY_ProductName
                return parts[parts.Length - 1].Trim();
            }

            return "UnknownProduct";
        }
        private string GenerateFileName(string reportId, int sampleId, string productName, string documentNumber, string extension, bool isPreview, int revNumber = 0)
        {
            string safeProductName = productName.Replace(" ", " ");
            string prefix = isPreview ? "preview_" : "";

            // Format: reportid=X&sampleid=Y_productname OR reportid=X&sampleid=Y_productname_revXX
            if (revNumber > 0)
            {
                return $"{prefix}{reportId}_{sampleId}_{safeProductName}_rev{revNumber:D2}{extension}";
            }
            else
            {
                // Original file - no revision suffix
                return $"{prefix}{reportId}_{sampleId}_{safeProductName}{extension}";
            }
        }        // CHANGE 2: Update GeneratePdfPreview to open in NEW TAB
        private bool GeneratePdfPreview(DataTable dt, int reportId, int sampleId, string documentNumber)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document doc = null;
           // string line = "";
            try
            {
                string templatePath = Server.MapPath("~/InputFiles/PVReportTemplate.docx");
                if (!File.Exists(templatePath))
                {
                    ShowMessage("Template not found at InputFiles folder.");
                    return false;
                }

                string productName = Session["ProductName"]?.ToString() ?? "UnknownProduct";
                string safeProductName = productName.Replace(" ", "_");

                // CHANGED: Use TEMP folder instead of PreviewFiles_pv
                string pdfFileName = $"Preview_{reportId}_{sampleId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string tempPdfPath = Path.Combine(Path.GetTempPath(), pdfFileName);

                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(templatePath);
                wordApp.Visible = false;
                ReplaceAllPlaceholders(doc, dt, reportId, sampleId);
                doc.ExportAsFixedFormat(
                    tempPdfPath,
                    Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF
                );
                Session["PdfPreviewPath"] = tempPdfPath;

                // CHANGED: Copy to PreviewFiles_pv ONLY for browser access
                string previewDir = Server.MapPath("~/PreviewFiles_pv/");
                Directory.CreateDirectory(previewDir);
                string browserAccessPath = Path.Combine(previewDir, pdfFileName);
                File.Copy(tempPdfPath, browserAccessPath, true);
                // Open PDF in NEW TAB
                string relativePath = "~/PreviewFiles_pv/" + pdfFileName;
                string url = ResolveUrl(relativePath);
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "openPdfPreview",
                    $"window.open('{url}', '_blank');",
                    true);
                // ADDED: Schedule cleanup of preview PDF after 5 minutes
                SchedulePreviewCleanup(browserAccessPath, tempPdfPath);
                return true;
            }
            catch (Exception ex)
            {
               // ShowMessage($"Error generating PDF preview: {ex.Message + line}");
                return false;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                }
                if (wordApp != null)
                {
                    wordApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                }
            }
        }
        // ADDED: Method to cleanup preview files
        private void SchedulePreviewCleanup(string browserPath, string tempPath)
        {
            System.Threading.Tasks.Task.Delay(300000).ContinueWith(t =>
            {
                try
                {
                    if (File.Exists(browserPath))
                        File.Delete(browserPath);
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                catch { /* Ignore cleanup errors */ }
            });
        }
        private void ReplaceAllPlaceholders(Microsoft.Office.Interop.Word.Document doc, DataTable dt, int reportId, int sampleId)
        {
            // Get values from ViewState
            string brand = ViewState["Brand"]?.ToString() ?? "";
            string manufacturer = ViewState["Manufacturer"]?.ToString() ?? "";
            string testMethod = ViewState["TestMethod"]?.ToString() ?? "";
            string workOrder = ViewState["WorkOrder"]?.ToString() ?? "";
            string date = ViewState["Date"]?.ToString() ?? "";
            string pic = ViewState["PIC"]?.ToString() ?? "";
            string rValue = ViewState["R"]?.ToString() ?? "";
            string equipmentNo = ViewState["EquipmentNo"]?.ToString() ?? "";
           // string line = "";
            try
            {
                // Inside GeneratePdfPreview method
                DataTable dtReport = _cm.GetExcelFileNameByReportID(reportId);
               
                // Get full report name from DB
                string fullReportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"].ToString() : "";

                //  Extract product name from report name
                string productName = ExtractProductName(fullReportName);

                //  Extract document number from full report name
                string documentNumber = ExtractDocumentNumber(fullReportName);

                ReplaceInHeaders(doc, "{docno}", documentNumber);
                ReplaceInHeaders(doc, "{reportname}", productName);


                ReplacePlaceholder(doc, "{reportId}", reportId.ToString());
                ReplacePlaceholder(doc, "{sampleId}", sampleId.ToString());
               
                // CREATE FORM FIELDS TABLE for {table1}
                DataTable formTable = new DataTable();
                formTable.Columns.Add("Col1");
                formTable.Columns.Add("Col2");
                formTable.Columns.Add("Col3");
                formTable.Columns.Add("Col4");

                formTable.Rows.Add("Brand", brand, "#R", rValue);
                formTable.Rows.Add("Manufacturer", manufacturer, "#WO", workOrder);
                formTable.Rows.Add("Test Method No.", testMethod, "Date", date);
                formTable.Rows.Add("Equipment No.", equipmentNo, "PIC", pic);
                
                // Replace {table1} WITHOUT headers
                ReplaceTablePlaceholderNoHeader(doc, "{table1}", formTable);
                
                // Replace {table} WITH headers
                ReplaceTablePlaceholder(doc, "{table}", dt);
               
            }
            catch (Exception ex)
            {
                ShowMessage($"Error confirming upload: {ex.Message}");
            }

        }
        // NEW METHOD: Replace table WITHOUT header row
        private void ReplaceTablePlaceholderNoHeader(Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();

            if (range.Find.Execute(FindText: placeholder))
            {
                range.Text = "";

                // Create table WITHOUT header row - just data rows
                Microsoft.Office.Interop.Word.Table wordTable = doc.Tables.Add(
                    range,
                    dt.Rows.Count,  // NO +1 for header
                    dt.Columns.Count
                );

                wordTable.Borders.Enable = 1;
                wordTable.AllowAutoFit = true;

                // Add ONLY data rows (no header)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        wordTable.Cell(i + 1, j + 1).Range.Text = dt.Rows[i][j]?.ToString() ?? "";
                        wordTable.Cell(i + 1, j + 1).Range.Font.Size = 10;

                        // Make first and third columns bold (labels)
                        if (j == 0 || j == 2)
                        {
                            wordTable.Cell(i + 1, j + 1).Range.Font.Bold = 1;
                        }
                    }
                }

                wordTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);
                range.InsertParagraphAfter();
            }
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
                        range.Find.Execute(FindText: placeholder, ReplaceWith: replacementText, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
                    }
                }
            }
        }
        // EXISTING METHOD: Replace table WITH header row (for data table)
        private void ReplaceTablePlaceholder(Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            //string line = "";
            // Get the Excel file path that was just uploaded
            string excelPath = Path.Combine(Server.MapPath("~/TempFolder"), Guid.NewGuid().ToString() + ".xlsx");

            // Save the uploaded file temporarily
            fileUpload.SaveAs(excelPath);
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            Excel.Range excelRange = null;
            try
            {
                // Open Excel and copy the table with formatting
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                wb = excelApp.Workbooks.Open(excelPath);
                ws = wb.Sheets[1];
                excelRange = ws.Range["A8"].CurrentRegion; //  table starts at row 15
                excelRange.Copy();

                // Find placeholder in Word and paste
                Microsoft.Office.Interop.Word.Range range = doc.Content;
                range.Find.ClearFormatting();

                if (range.Find.Execute(FindText: placeholder))
                {
                    range.Text = "";
                    range.Paste();

                    Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];
                    wordTable.Borders.Enable = 1;

                    //  Center align all text and set table width**
                    wordTable.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    wordTable.Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Set table to full page width
                    wordTable.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPercent;
                    wordTable.PreferredWidth = 100;

                    // Auto-fit columns to distribute evenly
                    wordTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);
                
            }
            }
            catch(Exception ex)
            {
                ShowMessage($"Error confirming upload: {ex.Message}");
            }
            finally
            {
                // Cleanup
                if (wb != null) { wb.Close(false); Marshal.ReleaseComObject(wb); }
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (excelRange != null) Marshal.ReleaseComObject(excelRange);
                if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }

                if (File.Exists(excelPath))
                {
                    try { File.Delete(excelPath); } catch { }
                }

            }

        }
        private void ReplacePlaceholder(Microsoft.Office.Interop.Word.Document doc, string placeholder, string value)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();

            while (range.Find.Execute(FindText: placeholder))
            {
                range.Text = value;
                range = doc.Content; // Reset range for next search
            }
        }
        protected void btnConfirmUpload_Click(object sender, EventArgs e)
        {
            try
            {
                btnUpdateUpload.Visible = false;
                btnDownloadExisting.Visible = false;
                btnConfirmUpload.Visible = true;
                btnDownlaodNew.Visible = true;

                if (!fileUpload.HasFile)
                {
                    ShowMessage("Please select a file to upload.");
                    return;
                }

                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    ShowMessage("Please upload a valid Excel file (.xlsx or .xls)");
                    return;
                }

                int sampleId = 0;
                int reportIdInt = 0;
                string reportIdStr = "";

                // Get SampleID from ViewState first, fallback to QueryString
                if (ViewState["SampleID"] != null)
                {
                    sampleId = Convert.ToInt32(ViewState["SampleID"]);
                }
                else if (Request.QueryString["s"] != null)
                {
                    int.TryParse(Request.QueryString["s"], out sampleId);
                }

                // Get ReportID from ViewState first, fallback to QueryString
                if (ViewState["ReportID"] != null)
                {
                    reportIdInt = Convert.ToInt32(ViewState["ReportID"]);
                    reportIdStr = reportIdInt.ToString();
                }
                else if (Request.QueryString["r"] != null)
                {
                    reportIdStr = Request.QueryString["r"];
                    if (!int.TryParse(reportIdStr, out reportIdInt))
                    {
                        ShowMessage("Invalid Report ID.");
                        return;
                    }
                }


                DataTable dtReport = _cm.GetExcelFileNameByReportID(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"].ToString() : "";

                string documentNumber = ExtractDocumentNumber(reportName);
                string productName = ExtractProductName(reportName);

                bool fileExistsInDB = CheckIfFileExistsInDatabase(reportIdStr, sampleId);

                string finalFileName;
                if (!fileExistsInDB)
                {
                    finalFileName = GenerateFileName(reportIdStr, sampleId, productName, documentNumber, fileExtension, false, 0);
                }
                else
                {
                    int revNumber = GetNextRevisionNumber(reportIdStr, sampleId, productName, documentNumber);
                    finalFileName = GenerateFileName(reportIdStr, sampleId, productName, documentNumber, fileExtension, false, revNumber);
                }

                string uploadFolder = "~/FilesUpload/";
                string uploadFolderPath = Server.MapPath(uploadFolder);
                string finalFilePath = Path.Combine(uploadFolderPath, finalFileName);

                Directory.CreateDirectory(uploadFolderPath);

                if (File.Exists(finalFilePath))
                {
                    ShowMessage("File already exists with this name. This shouldn't happen!");
                    return;
                }

                fileUpload.SaveAs(finalFilePath);

                DataTable excelData = ReadExcelToDataTable(finalFilePath);

                if (excelData == null || excelData.Rows.Count == 0)
                {
                    ShowMessage("No data found in Excel file. Cannot generate PDF.");
                    // Delete the Excel file
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return;
                }

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

                string pdfFileName = Path.ChangeExtension(finalFileName, ".pdf");
                string pdfFullPath = Path.Combine(uploadFolderPath, pdfFileName);

                bool pdfGenerated = GenerateFinalPdf(excelData, reportIdInt, sampleId, pdfFullPath);

                if (!pdfGenerated)
                {
                    ShowMessage("Excel file saved but PDF generation failed. Please check the template and data.");
                    // ROLLBACK: Delete the Excel file since PDF failed
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return; // Stop here - don't save to DB
                }
                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;

                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);

                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                string relativeExcelPath = "~/FilesUpload/" + finalFileName;
                string relativePdfPath = "~/FilesUpload/" + pdfFileName;

                int savedCount = _cm.AddTestResultPV(_cm, finalFileName, relativeExcelPath, relativePdfPath, AnalysisStartingDate, AnalysisCompletionDate);

                if (savedCount > 0)
                {
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.UpdateCRFCloneList_Pv(_cm);
                    _cm.UpdateSampleDetails_Pv(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    btnConfirmUpload.Visible = false;

                    string message = fileExistsInDB
                        ? $"Revision saved successfully!\\n\\nFile saved as: {finalFileName}\\n\\nNote: "
                        : $"Test results saved successfully!\\n\\nFile saved as: {finalFileName}";

                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", $"alert('{message}');window.location='WorkList_PV.aspx';", true);
                }
                else
                {
                    // ROLLBACK: Delete both files if DB save fails
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    if (File.Exists(pdfFullPath))
                        File.Delete(pdfFullPath);

                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results.');", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error confirming upload: {ex.Message}");
            }
        }





        protected void btnUpdateUpload_Click(object sender, EventArgs e)
        {
            btnUpdateUpload.Visible = true;
            btnDownloadExisting.Visible = true;
            btnConfirmUpload.Visible = false;
            btnDownlaodNew.Visible = false;

            try
            {
                if (!fileUpload.HasFile)
                {
                    ShowMessage("Please select a file to upload.");
                    return;
                }

                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    ShowMessage("Please upload a valid Excel file (.xlsx or .xls)");
                    return;
                }

                int sampleId = 0;
                int reportIdInt = 0;
                string reportIdStr = "";

                // Get SampleID from ViewState first, fallback to QueryString
                if (ViewState["SampleID"] != null)
                {
                    sampleId = Convert.ToInt32(ViewState["SampleID"]);
                }
                else if (Request.QueryString["s"] != null)
                {
                    int.TryParse(Request.QueryString["s"], out sampleId);
                }

                // Get ReportID from ViewState first, fallback to QueryString
                if (ViewState["ReportID"] != null)
                {
                    reportIdInt = Convert.ToInt32(ViewState["ReportID"]);
                    reportIdStr = reportIdInt.ToString();
                }
                else if (Request.QueryString["r"] != null)
                {
                    reportIdStr = Request.QueryString["r"];
                    if (!int.TryParse(reportIdStr, out reportIdInt))
                    {
                        ShowMessage("Invalid Report ID.");
                        return;
                    }
                }


                DataTable dtReport = _cm.GetExcelFileNameByReportID(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"].ToString() : "";

                string documentNumber = ExtractDocumentNumber(reportName);
                string productName = ExtractProductName(reportName);

                int revNumber = GetNextRevisionNumber(reportIdStr, sampleId, productName, documentNumber);
                string finalFileName = GenerateFileName(reportIdStr, sampleId, productName, documentNumber, fileExtension, false, revNumber);

                string uploadFolder = "~/FilesUpload/";
                string uploadFolderPath = Server.MapPath(uploadFolder);
                string finalFilePath = Path.Combine(uploadFolderPath, finalFileName);

                Directory.CreateDirectory(uploadFolderPath);

                if (File.Exists(finalFilePath))
                {
                    ShowMessage("File already exists. Try again.");
                    return;
                }

                fileUpload.SaveAs(finalFilePath);

                DataTable excelData = ReadExcelToDataTable(finalFilePath);

                if (excelData == null || excelData.Rows.Count == 0)
                {
                    ShowMessage("No data found in Excel file.");
                    // Delete Excel file
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return;
                }

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

                string pdfFileName = Path.ChangeExtension(finalFileName, ".pdf");
                string pdfFullPath = Path.Combine(uploadFolderPath, pdfFileName);
                bool pdfGenerated = GenerateFinalPdf(excelData, reportIdInt, sampleId, pdfFullPath);

                if (!pdfGenerated)
                {
                    ShowMessage("Excel saved but PDF generation failed.");
                    // ROLLBACK: Delete Excel file since PDF failed
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return; // Stop here - don't update DB
                }

                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;

                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);

                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                string relativeExcelPath = "~/FilesUpload/" + finalFileName;
                string relativePdfPath = "~/FilesUpload/" + pdfFileName;

                int savedCount = _cm.UpdateTestResultPV(_cm, finalFileName, relativeExcelPath, relativePdfPath, AnalysisStartingDate,
    AnalysisCompletionDate);

                if (savedCount > 0)
                {

                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.UpdateCRFCloneList_Pv(_cm);
                    _cm.UpdateSampleDetails_Pv(_cm);
                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);


                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", $"alert('Revision {revNumber:D2} updated successfully!\\n\\nFile: {finalFileName}');window.location='WorkList_PV.aspx';", true);
                }
                else
                {
                    // ROLLBACK: Delete both files if DB update fails
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    if (File.Exists(pdfFullPath))
                        File.Delete(pdfFullPath);

                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to update database.');", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}");
            }
        }


        private int GetNextRevisionNumber(string reportId, int sampleId, string productName, string documentNumber)
        {
            int nextRev = 1;
            string folderPath = Server.MapPath("~/UploadedExcelFiles/");
            string safeProductName = productName.Replace(" ", "_");

            // Base pattern without revision
            string basePattern = $"{reportId}_{sampleId}_{safeProductName}";

            // Get all files (both xlsx and xls)
            var allFiles = Directory.GetFiles(folderPath, "*.xls*")
                                    .Where(f => Path.GetFileName(f).StartsWith(basePattern))
                                    .ToList();

            foreach (string file in allFiles)
            {
                string filename = Path.GetFileNameWithoutExtension(file);

                // Check if it has revision number
                var match = Regex.Match(filename, @"_rev(\d{2})$");

                if (match.Success)
                {
                    int rev = int.Parse(match.Groups[1].Value);
                    if (rev >= nextRev)
                        nextRev = rev + 1;
                }
                else
                {
                    // Original file exists (no revision), so next should be rev01
                    if (filename == basePattern)
                        nextRev = Math.Max(nextRev, 1);
                }
            }

            return nextRev;
        }
        private bool CheckIfFileExistsInDatabase(string reportId, int sampleId)
        {
            try
            {
                if (_cm == null) _cm = new DAL.CM();

                // Check if there's already a record for this reportId and sampleId
                DataTable dt = _cm.GetExistingTestResult(Convert.ToInt32(reportId), sampleId);

                return (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["Result"]?.ToString()));
            }
            catch
            {
                return false;
            }
        }
        //  existing PDF generation method 
        private bool GenerateFinalPdf(DataTable dt, int reportId, int sampleId, string outputPdfPath)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                string templatePath = Server.MapPath("~/InputFiles/PVReportTemplate.docx");
                if (!File.Exists(templatePath))
                {
                    ShowMessage("Template not found at InputFiles folder.");
                    return false;
                }

                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(templatePath);
                wordApp.Visible = false;

                ReplaceAllPlaceholders(doc, dt, reportId, sampleId);

                doc.ExportAsFixedFormat(
                    outputPdfPath,
                    Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF
                );

                return true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating PDF: {ex.Message}");
                return false;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                }
                if (wordApp != null)
                {
                    wordApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                }
            }
        }
        private DataTable ReadExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);

                // Find the table starting row (row 15 with "Group ID")
                int tableStartRow = 8;

                // Add columns from row  (table headers)
                for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                {
                    string columnName = worksheet.Cell(tableStartRow, col).Value.ToString();
                    if (string.IsNullOrWhiteSpace(columnName))
                        break; // Stop when we hit empty column
                    dt.Columns.Add(columnName);
                }

                if (dt.Columns.Count == 0)
                    return dt; // No columns found

                // Add data rows starting from row 16
                for (int row = tableStartRow + 1; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    DataRow dr = dt.NewRow();
                    bool isEmptyRow = true;

                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        var cellValue = worksheet.Cell(row, col + 1).Value.ToString();
                        dr[col] = cellValue;
                        if (!string.IsNullOrWhiteSpace(cellValue))
                            isEmptyRow = false;
                    }

                    // Only add non-empty rows
                    if (!isEmptyRow)
                        dt.Rows.Add(dr);
                }
            }
            return dt;
        } // Helper: Save DataTable to Database
        private void ShowMessage(string message)
        {
            message = message.Replace("'", "\\'"); // escape single quotes
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", $"alert('{message}');", true);
        }
        //private string GetSampleIDSeries(int sampleId)
        //{
        //    string series = "";
        //    try
        //    {
        //        string query = "SELECT SampleIDSeries FROM Samples WHERE SampleID = @SampleID";
        //        using (SqlCommand cmd = new SqlCommand(query, _sCon))
        //        {
        //            cmd.Parameters.AddWithValue("@SampleID", sampleId);
        //            _sCon.Open();
        //            object result = cmd.ExecuteScalar();
        //            if (result != null && result != DBNull.Value)
        //            {
        //                series = result.ToString();
        //            }
        //            _sCon.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_sCon.State == ConnectionState.Open)
        //            _sCon.Close();
        //    }
        //    return series;
        //}

        // Download the existing Excel file for this ReportID
    }
}
