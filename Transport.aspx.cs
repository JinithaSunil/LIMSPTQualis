using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Excel = Microsoft.Office.Interop.Excel;
namespace LIMS
{
    public partial class Transport : System.Web.UI.Page
    {       
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        DAL.CM _cm = new DAL.CM();
        private int reportId = 0;
        private int sampleId = 0;
        //private string sampleIdSeries = "smp/2001";
        private string uploadFolder = "~/FilesUpload/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
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

                    // Initialize DAL
                    if (_cm == null)
                        _cm = new DAL.CM();

                    _cm.sampleId = sampleId;
                    _cm.ReportID = reportId.ToString();

                    // Get sample data
                    DataTable dt1 = _cm.Getdataforseriesandreportid_Transport(_cm);
                    if (dt1 != null && dt1.Rows.Count > 0)
                    {
                        try
                        {
                            // Populate fields based on new layout
                            txtR.Text = dt1.Rows[0]["R"]?.ToString() ?? "";
                            txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"]?.ToString() ?? "";
                            txttestmethod.Text = dt1.Rows[0]["TestMethod"]?.ToString() ?? "";

                            // Set default values if needed
                            if (string.IsNullOrWhiteSpace(txtpic.Text))
                                txtpic.Text = "AD AJ DD";

                            txtR.Text = "R/2024110019";
                            txtworkorder.Text = "WO-12";
                            txtsample.Text = "Transport";
                            txtequipment.Text = "INV/2015/AT/01 ; INV/2015/AT/09";
                            txttestmethod.Text = "ES-X60210 CL.T clause 4.5.1 , ES-X83239 CL.F clause 4.12.4.1";
                            txtcalibrationduedate.Text = "08/01/2026";
                            txtstartdate.Text = "08/01/2026";
                            txtfinishdate.Text = "08/01/2026";
                        }
                        catch (Exception ex)
                        {
                            ShowMessage($"Error populating sample data: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Error in sample data population: {ex.ToString()}");
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(),
                            "showError",
                            "alert('Could not retrieve sample data. Please check sample ID.');",
                            true);
                    }

                    // Check sample status
                    DataTable dtStatus = _cm.GetSampleStatus(sampleId, reportId);
                    if (dtStatus != null && dtStatus.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtStatus.Rows)
                        {
                            try
                            {
                                int status = Convert.ToInt32(row["samplestatus"]);

                                if (status == 19) // New test
                                {
                                    btnDownloadExisting.Visible = false;
                                    btnUpdateUpload.Visible = false;
                                    btnDownlaodNew.Visible = true;
                                    btnConfirmUpload.Visible = true;
                                    System.Diagnostics.Debug.WriteLine("Sample status: New test (19)");
                                    break;
                                }
                                else if (status == 1005) // Retest
                                {
                                  //  LoadExistingTestResultForRetest(reportId, sampleId);
                                    btnDownloadExisting.Visible = true;
                                    btnUpdateUpload.Visible = true;
                                    btnDownlaodNew.Visible = false;
                                    btnConfirmUpload.Visible = false;
                                    System.Diagnostics.Debug.WriteLine("Sample status: Retest (1005)");
                                    break;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"Unknown sample status: {status}");
                                }
                            }
                            catch (Exception ex)
                            {
                                ShowMessage($"Error processing sample status: {ex.Message}");
                                System.Diagnostics.Debug.WriteLine($"Error in status processing: {ex.ToString()}");
                            }
                        }
                    }
                    else
                    {
                        btnDownloadExisting.Visible = false;
                        btnUpdateUpload.Visible = false;
                        btnDownlaodNew.Visible = false;
                        btnConfirmUpload.Visible = false;
                        ScriptManager.RegisterStartupScript(this, GetType(),
                            "showError",
                            "alert('Could not retrieve sample status.');",
                            true);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error loading page: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Page_Load Exception: {ex.ToString()}");
                }
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
            txtR.Text = "";
            txtworkorder.Text = "";
            txtequipment.Text = "";
            txtpic.Text = "";
            txttemperature.Text = "";
            txtsample.Text = "";
            txttestmethod.Text = "";         
            txtpic.Text = "";
        }

        private DataTable GetReportDetailsFromDatabase(int reportId)
        {
            DataTable dt = _cm.GetDocNoByReportID(reportId);
            return dt;
        }
                
        private string GetExcelFileName(int reportId)
        {
            string excelFileName = "ReportTemplate"; // Default
            DataTable dt = GetReportDetailsFromDatabase(reportId);

            if (dt != null && dt.Rows.Count > 0)
            {
                excelFileName = dt.Rows[0]["ExcelFileName"]?.ToString() ?? "ReportTemplate";
            }

            // Add .xlsx extension if not present
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
            int flag = 0;

            // Validation for required fields based on the image
            if (string.IsNullOrWhiteSpace(txtR.Text))
            {
                txtR.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtR.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtworkorder.Text))
            {
                txtworkorder.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtworkorder.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtsample.Text))
            {
                txtsample.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtsample.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtpic.Text))
            {
                txtpic.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtpic.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtstartdate.Text))
            {
                txtstartdate.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtstartdate.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txttemperature.Text))
            {
                txttemperature.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txttemperature.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtfinishdate.Text))
            {
                txtfinishdate.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtfinishdate.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtequipment.Text))
            {
                txtequipment.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtequipment.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txttestmethod.Text))
            {
                txttestmethod.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txttestmethod.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtcalibrationduedate.Text))
            {
                txtcalibrationduedate.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtcalibrationduedate.BorderColor = System.Drawing.Color.Empty;

            if (flag == 0)
            {
                try
                {
                    btnDownlaodNew.Visible = true;
                    btnConfirmUpload.Visible = true;
                    btnUpdateUpload.Visible = false;
                    btnDownloadExisting.Visible = false;
                    int sampleId = 0;

                    // Retrieve reportId and sampleId from ViewState
                    if (ViewState["ReportID"] != null)
                    {
                        reportId = Convert.ToInt32(ViewState["ReportID"]);
                    }

                    if (ViewState["SampleID"] != null)
                    {
                        sampleId = Convert.ToInt32(ViewState["SampleID"]);
                    }

                    // Get report details from database
                    DataTable dtReport = GetReportDetailsFromDatabase(reportId);

                    if (dtReport == null || dtReport.Rows.Count == 0)
                    {
                        ShowMessage("Report details not found.");
                        return;
                    }

                    string excelFileName = dtReport.Rows[0]["ExcelFileName"]?.ToString() ?? "ReportTemplate";
                    string reportName = dtReport.Rows[0]["ReportName"]?.ToString() ?? "";
                    string documentNumber = dtReport.Rows[0]["docno"]?.ToString() ?? "";

                    // Add extension if not present
                    if (!excelFileName.ToLower().EndsWith(".xlsx") && !excelFileName.ToLower().EndsWith(".xls"))
                    {
                        excelFileName = excelFileName + ".xlsx";
                    }

                    string templatePath = Server.MapPath($"~/InputFiles/{excelFileName}");

                    if (!File.Exists(templatePath))
                    {
                        ShowMessage($"Excel file '{excelFileName}' not found.");
                        return;
                    }

                    bool isOriginalDownload = !CheckIfFileExistsInDatabase(reportId.ToString(), sampleId);

                    string downloadFileName;
                    if (isOriginalDownload)
                    {
                        downloadFileName = GenerateFileName(reportId.ToString(), sampleId, reportName, documentNumber, ".xlsx", false, 0);
                    }
                    else
                    {
                        DataTable dtExisting = _cm.GetExistingTestResult(reportId, sampleId);
                        if (dtExisting != null && dtExisting.Rows.Count > 0)
                        {
                            downloadFileName = dtExisting.Rows[0]["FileName"].ToString();
                        }
                        else
                        {
                            downloadFileName = GenerateFileName(reportId.ToString(), sampleId, reportName, documentNumber, ".xlsx", false, 0);
                        }
                    }

                    using (var workbook = new XLWorkbook(templatePath))
                    {
                        var worksheet = workbook.Worksheet(1);

                        // Clear the header area (adjust range as needed)
                        worksheet.Range("A1:D6").Clear();

                        // Define colors
                        var grayColor = XLColor.FromArgb(217, 217, 217); // #D9D9D9

                        // Build the header table based on the image layout
                        // Row 1: #R | Value | #WO | Value
                        worksheet.Cell("A1").Value = "#R";
                        worksheet.Cell("A1").Style.Font.Bold = true;
                        worksheet.Cell("A1").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B1").Value = txtR.Text;
                        worksheet.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C1").Value = "#WO";
                        worksheet.Cell("C1").Style.Font.Bold = true;
                        worksheet.Cell("C1").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D1").Value = txtworkorder.Text;
                        worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 2: #Sample | Value | PIC | Value
                        worksheet.Cell("A2").Value = "#Sample";
                        worksheet.Cell("A2").Style.Font.Bold = true;
                        worksheet.Cell("A2").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B2").Value = txtsample.Text;
                        worksheet.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C2").Value = "PIC";
                        worksheet.Cell("C2").Style.Font.Bold = true;
                        worksheet.Cell("C2").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D2").Value = txtpic.Text;
                        worksheet.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 3: Start date | Value | Temperature | Value
                        worksheet.Cell("A3").Value = "Start date";
                        worksheet.Cell("A3").Style.Font.Bold = true;
                        worksheet.Cell("A3").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B3").Value = txtstartdate.Text;
                        worksheet.Cell("B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C3").Value = "Temperature";
                        worksheet.Cell("C3").Style.Font.Bold = true;
                        worksheet.Cell("C3").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D3").Value = txttemperature.Text;
                        worksheet.Cell("D3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 4: Finish date | Value | Equipment | Value
                        worksheet.Cell("A4").Value = "Finish date";
                        worksheet.Cell("A4").Style.Font.Bold = true;
                        worksheet.Cell("A4").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B4").Value = txtfinishdate.Text;
                        worksheet.Cell("B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C4").Value = "Equipment";
                        worksheet.Cell("C4").Style.Font.Bold = true;
                        worksheet.Cell("C4").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D4").Value = txtequipment.Text;
                        worksheet.Cell("D4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 5 & 6: Method (spans 2 rows) | Value | Calibration due | Value
                        worksheet.Cell("A5").Value = "Method";
                        worksheet.Cell("A5").Style.Font.Bold = true;
                        worksheet.Cell("A5").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Merge A5:A6 for "Method" label
                        worksheet.Range("A5:A6").Merge();
                        worksheet.Range("A5:A6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        // Merge B5:B6 for method value
                        worksheet.Range("B5:B6").Merge();
                        worksheet.Cell("B5").Value = txttestmethod.Text;
                        worksheet.Cell("B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Cell("B5").Style.Alignment.WrapText = true;

                        worksheet.Cell("C5").Value = "Calibration due";
                        worksheet.Cell("C5").Style.Font.Bold = true;
                        worksheet.Cell("C5").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Merge C5:C6 for "Calibration due" label
                        worksheet.Range("C5:C6").Merge();
                        worksheet.Range("C5:C6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        // Merge D5:D6 for calibration due value
                        worksheet.Range("D5:D6").Merge();
                        worksheet.Cell("D5").Value = txtcalibrationduedate.Text;
                        worksheet.Cell("D5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Cell("D5").Style.Alignment.WrapText = true;

                        // Apply borders to the header table (A1:D6)
                        var headerRange = worksheet.Range("A1:D6");
                        headerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        // Adjust column widths
                        worksheet.Column(1).Width = 20; // Column A
                        worksheet.Column(2).Width = 25; // Column B
                        worksheet.Column(3).Width = 20; // Column C
                        worksheet.Column(4).Width = 35; // Column D

                        // Save and download
                        string tempPath = Server.MapPath($"~/Temp/temp_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                        Directory.CreateDirectory(Server.MapPath("~/Temp/"));
                        workbook.SaveAs(tempPath);

                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", $"attachment; filename={downloadFileName}");
                        Response.TransmitFile(tempPath);
                        Response.End();

                        File.Delete(tempPath);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error downloading Excel: {ex.Message}");
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill all required fields.');", true);
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

                DataTable dtReport = GetReportDetailsFromDatabase(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";
                string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";
                string tempFilePath = Path.GetTempFileName();
                string tempExcelPath = Path.ChangeExtension(tempFilePath, fileExtension);

                // Save to temp location
                fileUpload.SaveAs(tempExcelPath);
                // Read form fields and table from Excel
                DataTable dt = ReadExcelToDataTable(tempExcelPath);

                using (var workbook = new XLWorkbook(tempExcelPath))
                {
                    var ws = workbook.Worksheet(1);

                    // Read header values based on new layout (matching the image)
                    // Row 1: #R | Value | #WO | Value
                    txtR.Text = ws.Cell("B1").GetString();
                    txtworkorder.Text = ws.Cell("D1").GetString();

                    // Row 2: #Sample | Value | PIC | Value
                    txtsample.Text = ws.Cell("B2").GetString();
                    txtpic.Text = ws.Cell("D2").GetString();

                    // Row 3: Start date | Value | Temperature | Value
                    txtstartdate.Text = ws.Cell("B3").GetFormattedString();
                    txttemperature.Text = ws.Cell("D3").GetString();

                    // Row 4: Finish date | Value | Equipment | Value
                    txtfinishdate.Text = ws.Cell("B4").GetFormattedString();
                    txtequipment.Text = ws.Cell("D4").GetString();

                    // Row 5-6: Method | Value | Calibration due | Value (merged cells)
                    txttestmethod.Text = ws.Cell("B5").GetString();
                    txtcalibrationduedate.Text = ws.Cell("D5").GetString();
                }

                // Store ALL values in ViewState
                ViewState["R"] = txtR.Text;
                ViewState["WorkOrder"] = txtworkorder.Text;
                ViewState["Sample"] = txtsample.Text;
                ViewState["PIC"] = txtpic.Text;
                ViewState["StartDate"] = txtstartdate.Text;
                ViewState["Temperature"] = txttemperature.Text;
                ViewState["FinishDate"] = txtfinishdate.Text;
                ViewState["Equipment"] = txtequipment.Text;
                ViewState["TestMethod"] = txttestmethod.Text;
                ViewState["CalibrationDueDate"] = txtcalibrationduedate.Text;

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

        private string GenerateFileName(string reportId, int sampleId, string reportName, string documentNumber, string extension, bool isPreview, int revNumber = 0)
        {
            string safereportName = reportName.Replace(" ", " ");
            string prefix = isPreview ? "preview_" : "";

            // Format: reportid=X&sampleid=Y_productname OR reportid=X&sampleid=Y_productname_revXX
            if (revNumber > 0)
            {
                //return $"{prefix}reportid={reportId}&sampleid={sampleId}_{safeProductName}_rev{revNumber:D2}{extension}";
                return $"{prefix}{reportId}_{sampleId}_{safereportName}_rev{revNumber:D2}{extension}";
            }
            else
            {
                // Original file - no revision suffix
                return $"{prefix}{reportId}_{sampleId}_{safereportName}{extension}";
            }
        }        // CHANGE 2: Update GeneratePdfPreview to open in NEW TAB

        private bool GeneratePdfPreview(DataTable dt, int reportId, int sampleId, string documentNumber)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                string templatePath = Server.MapPath("~/InputFiles/Transport.docx");
                if (!File.Exists(templatePath))
                {
                    ShowMessage("Template not found.");
                    return false;
                }

                string pdfFileName = $"Preview_{reportId}_{sampleId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string tempPdfPath = Path.Combine(Path.GetTempPath(), pdfFileName);

                wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Visible = false;
                doc = wordApp.Documents.Open(templatePath);

                ReplaceAllPlaceholders(doc, dt, reportId, sampleId);

                doc.ExportAsFixedFormat(tempPdfPath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);

                // Copy for browser access
                string previewDir = Server.MapPath("~/PreviewFiles/");
                Directory.CreateDirectory(previewDir);
                string browserAccessPath = Path.Combine(previewDir, pdfFileName);
                File.Copy(tempPdfPath, browserAccessPath, true);

                // Open in new tab
                string url = ResolveUrl("~/PreviewFiles/" + pdfFileName);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openPdfPreview",
                    $"window.open('{url}', '_blank');", true);

                Session["PdfPreviewPath"] = tempPdfPath;

                return true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating preview: {ex.Message}");
                return false;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                    Marshal.ReleaseComObject(doc);
                }
                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }
            }
        }

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
            // 1. Create HEADER TABLE based on new layout
            DataTable headerTable = new DataTable();
            headerTable.Columns.Add("Label1");
            headerTable.Columns.Add("Value1");
            headerTable.Columns.Add("Label2");
            headerTable.Columns.Add("Value2");

            headerTable.Rows.Add("#R", ViewState["R"]?.ToString() ?? "",
                "#WO", ViewState["WorkOrder"]?.ToString() ?? "");
            headerTable.Rows.Add("#Sample", ViewState["Sample"]?.ToString() ?? "",
                "PIC", ViewState["PIC"]?.ToString() ?? "");
            headerTable.Rows.Add("Start date", ViewState["StartDate"]?.ToString() ?? "",
                "Temperature", ViewState["Temperature"]?.ToString() ?? "");
            headerTable.Rows.Add("Finish date", ViewState["FinishDate"]?.ToString() ?? "",
                "Equipment", ViewState["Equipment"]?.ToString() ?? "");
            headerTable.Rows.Add("Method", ViewState["TestMethod"]?.ToString() ?? "",
                "Calibration due", ViewState["CalibrationDueDate"]?.ToString() ?? "");

            DataTable dtReport = GetReportDetailsFromDatabase(reportId);
            string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";
            string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";

            // 2. Replace header placeholders
            ReplaceInHeaders(doc, "{docno}", documentNumber);
            ReplaceInHeaders(doc, "{reportname}", reportName);

            // 3. Replace tables in ORDER - ONLY ONCE EACH
            ReplaceTablePlaceholderCommontable(doc, "{headerTable}", headerTable);
            ReplaceAllExcelTables(doc, "{MainResultTable}", reportId);
        }

        private void ReplaceTablePlaceholderCommontable(Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();
            if (range.Find.Execute(FindText: placeholder))
            {
                range.Text = "";

                // Create table with 6 rows and 4 columns to match new Excel structure
                Microsoft.Office.Interop.Word.Table wordTable = doc.Tables.Add(
                    range,
                    6, // 6 rows as per new layout
                    4  // 4 columns (A, B, C, D)
                );

                wordTable.Borders.Enable = 1;
                wordTable.AllowAutoFit = true;

                // #E6E6E6 = RGB(230, 230, 230)
                int lightGrayR = 230;
                int lightGrayG = 230;
                int lightGrayB = 230;

                var blackColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;
                var grayColor = (Microsoft.Office.Interop.Word.WdColor)(lightGrayR + 0x100 * lightGrayG + 0x10000 * lightGrayB);

                // Get data from ViewState
                string r = ViewState["R"]?.ToString() ?? "";
                string workOrder = ViewState["WorkOrder"]?.ToString() ?? "";
                string sample = ViewState["Sample"]?.ToString() ?? "";
                string pic = ViewState["PIC"]?.ToString() ?? "";
                string startDate = ViewState["StartDate"]?.ToString() ?? "";
                string temperature = ViewState["Temperature"]?.ToString() ?? "";
                string finishDate = ViewState["FinishDate"]?.ToString() ?? "";
                string equipment = ViewState["Equipment"]?.ToString() ?? "";
                string testMethod = ViewState["TestMethod"]?.ToString() ?? "";
                string calibrationDueDate = ViewState["CalibrationDueDate"]?.ToString() ?? "";

                // ============= FILL ALL CELLS FIRST (BEFORE ANY MERGING) =============

                // ROW 1: #R | Value | #WO | Value
                wordTable.Cell(1, 1).Range.Text = "#R";
                wordTable.Cell(1, 2).Range.Text = r;
                wordTable.Cell(1, 3).Range.Text = "#WO";
                wordTable.Cell(1, 4).Range.Text = workOrder;

                // ROW 2: #Sample | Value | PIC | Value
                wordTable.Cell(2, 1).Range.Text = "#Sample";
                wordTable.Cell(2, 2).Range.Text = sample;
                wordTable.Cell(2, 3).Range.Text = "PIC";
                wordTable.Cell(2, 4).Range.Text = pic;

                // ROW 3: Start date | Value | Temperature | Value
                wordTable.Cell(3, 1).Range.Text = "Start date";
                wordTable.Cell(3, 2).Range.Text = startDate;
                wordTable.Cell(3, 3).Range.Text = "Temperature";
                wordTable.Cell(3, 4).Range.Text = temperature;

                // ROW 4: Finish date | Value | Equipment | Value
                wordTable.Cell(4, 1).Range.Text = "Finish date";
                wordTable.Cell(4, 2).Range.Text = finishDate;
                wordTable.Cell(4, 3).Range.Text = "Equipment";
                wordTable.Cell(4, 4).Range.Text = equipment;

                // ROW 5: Method | Value (will span to row 6) | Calibration due | Value (will span to row 6)
                wordTable.Cell(5, 1).Range.Text = "Method";
                wordTable.Cell(5, 2).Range.Text = testMethod;
                wordTable.Cell(5, 3).Range.Text = "Calibration due";
                wordTable.Cell(5, 4).Range.Text = calibrationDueDate;

                // ROW 6: (will be merged with row 5)
                wordTable.Cell(6, 1).Range.Text = "";
                wordTable.Cell(6, 2).Range.Text = "";
                wordTable.Cell(6, 3).Range.Text = "";
                wordTable.Cell(6, 4).Range.Text = "";

                // ============= NOW DO ALL MERGING =============

                // Merge A5:A6 for "Method" label
                wordTable.Cell(5, 1).Merge(wordTable.Cell(6, 1));

                // Merge B5:B6 for method value
                wordTable.Cell(5, 2).Merge(wordTable.Cell(6, 2));

                // Merge C5:C6 for "Calibration due" label
                wordTable.Cell(5, 3).Merge(wordTable.Cell(6, 3));

                // Merge D5:D6 for calibration due value
                wordTable.Cell(5, 4).Merge(wordTable.Cell(6, 4));

                // ============= APPLY FORMATTING =============

                // Apply font size and vertical alignment to ALL cells
                for (int i = 1; i <= wordTable.Rows.Count; i++)
                {
                    for (int j = 1; j <= wordTable.Rows[i].Cells.Count; j++)
                    {
                        try
                        {
                            wordTable.Cell(i, j).Range.Font.Size = 10;
                            wordTable.Cell(i, j).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            wordTable.Cell(i, j).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error formatting cell ({i},{j}): {ex.Message}");
                        }
                    }
                }

                // Apply gray background and bold to label columns (Columns A and C)
                for (int i = 1; i <= wordTable.Rows.Count; i++)
                {
                    try
                    {
                        // Column A (labels)
                        wordTable.Cell(i, 1).Range.Font.Bold = 1;
                        wordTable.Cell(i, 1).Shading.BackgroundPatternColor = grayColor;

                        // Column C (labels)
                        wordTable.Cell(i, 3).Range.Font.Bold = 1;
                        wordTable.Cell(i, 3).Shading.BackgroundPatternColor = grayColor;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error applying gray background to row {i}: {ex.Message}");
                    }
                }

                // Set white background for all data cells (Columns B and D)
                for (int i = 1; i <= wordTable.Rows.Count; i++)
                {
                    try
                    {
                        wordTable.Cell(i, 2).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                        wordTable.Cell(i, 4).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error applying white background to row {i}: {ex.Message}");
                    }
                }

                // Enable text wrapping for Method and Calibration due cells (merged cells in rows 5-6)
                try
                {
                    wordTable.Cell(5, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    wordTable.Cell(5, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting text wrapping for merged cells: {ex.Message}");
                }

                // Set ALL borders to BLACK
                wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                wordTable.Borders.OutsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                wordTable.Borders.InsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                wordTable.Borders.OutsideColor = blackColor;
                wordTable.Borders.InsideColor = blackColor;

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
                        range.Find.Execute(FindText: placeholder, ReplaceWith: replacementText,
                            Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
                    }
                }
            }
        }
        /*original*/
        private void ReplaceAllExcelTables(Microsoft.Office.Interop.Word.Document doc, string placeholder, int reportId)
        {
            string excelPath = Path.Combine(
                Server.MapPath("~/TempFolder"),
                Guid.NewGuid().ToString() + ".xlsx");

            fileUpload.SaveAs(excelPath);

            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;

            try
            {
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                wb = excelApp.Workbooks.Open(excelPath);
                ws = wb.Sheets[1];

                // Find placeholder
                Microsoft.Office.Interop.Word.Range range = doc.Content;
                range.Find.ClearFormatting();

                if (!range.Find.Execute(FindText: placeholder))
                    return;

                range.Text = "";

                List<Excel.Range> mainTables = new List<Excel.Range>();
                HashSet<int> processedRows = new HashSet<int>();
                int lastRow = ws.UsedRange.Rows.Count;

                // Detect all tables starting from row 12
                for (int row = 12; row <= lastRow; row++)
                {
                    if (processedRows.Contains(row)) continue;

                    Excel.Range cell = ws.Cells[row, 1];
                    if (!string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                    {
                        Excel.Range tableRange = cell.CurrentRegion;
                        mainTables.Add(tableRange);

                        // Mark all rows in this table as processed
                        for (int r = tableRange.Row; r < tableRange.Row + tableRange.Rows.Count; r++)
                        {
                            processedRows.Add(r);
                        }
                    }
                    Marshal.ReleaseComObject(cell);
                }

                bool isFirstTable = true;

                // Paste all tables
                foreach (Excel.Range tableRange in mainTables)
                {
                    tableRange.Copy();

                    if (!isFirstTable)
                    {
                        range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                        range.InsertParagraphAfter();
                        range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                    }

                    range.Paste();

                    Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

                    // Apply formatting with error handling
                    try
                    {
                        wordTable.Borders.Enable = 1;
                        wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        wordTable.Borders.OutsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                        wordTable.Borders.InsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                        wordTable.Borders.OutsideColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;
                        wordTable.Borders.InsideColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;
                    }
                    catch { }

                    try
                    {
                        wordTable.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                    catch { }

                    try
                    {
                        wordTable.Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }
                    catch { }

                    try
                    {
                        wordTable.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPercent;
                        wordTable.PreferredWidth = 100;
                    }
                    catch { }

                    try
                    {
                        wordTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);
                    }
                    catch { }

                    isFirstTable = false;
                    Marshal.ReleaseComObject(tableRange);
                }
            }
            finally
            {
                if (wb != null)
                {
                    try
                    {
                        wb.Close(false);
                        Marshal.ReleaseComObject(wb);
                    }
                    catch { }
                }

                if (ws != null)
                {
                    try
                    {
                        Marshal.ReleaseComObject(ws);
                    }
                    catch { }
                }

                if (excelApp != null)
                {
                    try
                    {
                        excelApp.Quit();
                        Marshal.ReleaseComObject(excelApp);
                    }
                    catch { }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (File.Exists(excelPath))
                {
                    try
                    {
                        System.Threading.Thread.Sleep(100);
                        File.Delete(excelPath);
                    }
                    catch
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(500);
                            File.Delete(excelPath);
                        }
                        catch { }
                    }
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
                range = doc.Content;
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

                if (ViewState["SampleID"] != null)
                {
                    sampleId = Convert.ToInt32(ViewState["SampleID"]);
                }
                else if (Request.QueryString["s"] != null)
                {
                    int.TryParse(Request.QueryString["s"], out sampleId);
                }

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

                int CloneCRFSampleID = 0;
                DataTable dt1 = _cm.Getdataforseriesandreportid_transport(sampleId, reportIdInt);
                if (dt1.Rows.Count > 0)
                {
                    CloneCRFSampleID = Convert.ToInt32(dt1.Rows[0]["CloneCRFSampleID"]);
                }


                DataTable dtReport = GetReportDetailsFromDatabase(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";
                string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";

                bool fileExistsInDB = CheckIfFileExistsInDatabase(reportIdStr, sampleId);
                string finalFileName;
                if (!fileExistsInDB)
                {
                    finalFileName = GenerateFileName(reportIdStr, sampleId, reportName, documentNumber, fileExtension, false, 0);
                }
                else
                {
                    int revNumber = GetNextRevisionNumber(reportIdStr, sampleId, reportName, documentNumber);
                    finalFileName = GenerateFileName(reportIdStr, sampleId, reportName, documentNumber, fileExtension, false, revNumber);
                }

                string uploadFolder = "~/FilesUpload/";
                string uploadFolderPath = Server.MapPath(uploadFolder);
                string finalFilePath = Path.Combine(uploadFolderPath, finalFileName);

                Directory.CreateDirectory(uploadFolderPath);

                if (File.Exists(finalFilePath))
                {
                    ShowMessage("File already exists with this name.");
                    return;
                }

                fileUpload.SaveAs(finalFilePath);

                // Read data from Excel
                string rNumber = "";
                string workOrder = "";
                string sampleNumber = "";
                string pic = "";
                string startDate = "";
                string temperature = "";
                string finishDate = "";
                string equipment = "";
                string testMethodNo = "";
                string calibrationDueDate = "";

                using (var workbook = new XLWorkbook(finalFilePath))
                {
                    var ws = workbook.Worksheet(1);

                    rNumber = ws.Cell("B1").GetString();
                    workOrder = ws.Cell("D1").GetString();
                    sampleNumber = ws.Cell("B2").GetString();
                    pic = ws.Cell("D2").GetString();
                    startDate = ws.Cell("B3").GetFormattedString();
                    temperature = ws.Cell("D3").GetString();
                    finishDate = ws.Cell("B4").GetFormattedString();
                    equipment = ws.Cell("D4").GetString();
                    testMethodNo = ws.Cell("B5").GetString();
                    calibrationDueDate = ws.Cell("D5").GetFormattedString();
                }

                // Populate ViewState
                ViewState["RNumber"] = rNumber;
                ViewState["WorkOrder"] = workOrder;
                ViewState["SampleNumber"] = sampleNumber;
                ViewState["PIC"] = pic;
                ViewState["StartDate"] = startDate;
                ViewState["Temperature"] = temperature;
                ViewState["FinishDate"] = finishDate;
                ViewState["Equipment"] = equipment;
                ViewState["TestMethodNo"] = testMethodNo;
                ViewState["CalibrationDueDate"] = calibrationDueDate;

                DataTable excelData = ReadExcelToDataTable(finalFilePath);

                if (excelData == null || excelData.Rows.Count == 0)
                {
                    ShowMessage("No data found in Excel file. Cannot generate PDF.");
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return;
                }

                if (_cm == null) _cm = new DAL.CM();

                _cm.sampleId = sampleId;
                _cm.ReportID = reportIdStr;

                DataTable dt11 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt11.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }
                else
                {
                    _cm.sampleIDSeries = "smp/2001";
                }
                    

                // Parse only actual date fields
                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;
                DateTime? StartDate = null;
                DateTime? FinishDate = null;
                DateTime? CalibrationDueDate = null;

                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);

                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                if (!string.IsNullOrEmpty(txtstartdate.Text))
                    StartDate = DateTime.Parse(txtstartdate.Text);

                if (!string.IsNullOrEmpty(txtfinishdate.Text))
                    FinishDate = DateTime.Parse(txtfinishdate.Text);

                if (!string.IsNullOrEmpty(txtcalibrationduedate.Text))
                    CalibrationDueDate = DateTime.Parse(txtcalibrationduedate.Text);

                // Populate CM object
                _cm.RNumber = txtR?.Text?.Trim() ?? "";
                _cm.WorkOrder = txtworkorder?.Text?.Trim() ?? "";
                _cm.SampleNumber = txtsample?.Text?.Trim() ?? "";
                _cm.PIC = txtpic?.Text?.Trim() ?? "";
                _cm.Temperature = txttemperature?.Text?.Trim() ?? "";
                _cm.Equipment = txtequipment?.Text?.Trim() ?? "";
                _cm.TestMethodNo = txttestmethod?.Text?.Trim() ?? "";
                _cm.StartDate = StartDate;
                _cm.FinishDate = FinishDate;
                _cm.CalibrationDueDate = CalibrationDueDate;
               // _cm.AnalysisStartingDate = AnalysisStartingDate;
               // _cm.AnalysisCompletionDate = AnalysisCompletionDate;

                string pdfFileName = Path.ChangeExtension(finalFileName, ".pdf");
                string pdfFullPath = Path.Combine(uploadFolderPath, pdfFileName);
                bool pdfGenerated = GenerateFinalPdf(excelData, reportIdInt, sampleId, pdfFullPath);
                if (!pdfGenerated)
                {
                    ShowMessage("Excel file saved but PDF generation failed. Please check the template and data.");
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return;
                }

                string relativeExcelPath = "~/FilesUpload/" + finalFileName;
                string relativePdfPath = "~/FilesUpload/" + pdfFileName;
                int savedCount = _cm.AddTestResult_transport(_cm, finalFileName, relativeExcelPath, relativePdfPath,StartDate, FinishDate);
                if (savedCount > 0)
                {
                    btnConfirmUpload.Visible = false;
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailStatus(27, CloneCRFSampleID);
                    //_cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.UpdateCRFCloneList_Transport(_cm);
                    _cm.UpdateSampleDetails_Transport(_cm);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Record save Successfully'); setTimeout(function() { window.location = 'WorkList_Transport.aspx'; }, 100); </script>");
                }
                else
                {
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    if (File.Exists(pdfFullPath))
                        File.Delete(pdfFullPath);

                    ScriptManager.RegisterStartupScript(this, this.GetType(),"showError", "alert('Failed to save test results.');", true);
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

                if (ViewState["SampleID"] != null)
                {
                    sampleId = Convert.ToInt32(ViewState["SampleID"]);
                }
                else if (Request.QueryString["s"] != null)
                {
                    int.TryParse(Request.QueryString["s"], out sampleId);
                }

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

                DataTable dtReport = GetReportDetailsFromDatabase(reportIdInt);
                string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";
                string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";

                int revNumber = GetNextRevisionNumber(reportIdStr, sampleId, reportName, documentNumber);
                string finalFileName = GenerateFileName(reportIdStr, sampleId, reportName, documentNumber, fileExtension, false, revNumber);

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

                // Parse only actual date fields
                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;
                DateTime? StartDate = null;
                DateTime? FinishDate = null;
                DateTime? CalibrationDueDate = null;

                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);

                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                if (!string.IsNullOrEmpty(txtstartdate.Text))
                    StartDate = DateTime.Parse(txtstartdate.Text);

                if (!string.IsNullOrEmpty(txtfinishdate.Text))
                    FinishDate = DateTime.Parse(txtfinishdate.Text);

                if (!string.IsNullOrEmpty(txtcalibrationduedate.Text))
                    CalibrationDueDate = DateTime.Parse(txtcalibrationduedate.Text);

                // Populate CM object
                _cm.RNumber = txtR?.Text?.Trim() ?? "";
                _cm.WorkOrder = txtworkorder?.Text?.Trim() ?? "";
                _cm.SampleNumber = txtsample?.Text?.Trim() ?? "";
                _cm.PIC = txtpic?.Text?.Trim() ?? "";
                _cm.Temperature = txttemperature.Text.Trim();
                _cm.Equipment = txtequipment.Text.Trim();
                _cm.RNumber = txtR.Text.Trim();
                _cm.WorkOrder = txtworkorder.Text.Trim();
                _cm.SampleNumber = txtsample.Text.Trim();
                _cm.PIC = txtpic.Text.Trim();
                _cm.TestMethodNo = txttestmethod.Text.Trim();

                string pdfFileName = Path.ChangeExtension(finalFileName, ".pdf");
                string pdfFullPath = Path.Combine(uploadFolderPath, pdfFileName);
                bool pdfGenerated = GenerateFinalPdf(excelData, reportIdInt, sampleId, pdfFullPath);

                if (!pdfGenerated)
                {
                    ShowMessage("Excel saved but PDF generation failed.");
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return;
                }

                string relativeExcelPath = "~/FilesUpload/" + finalFileName;
                string relativePdfPath = "~/FilesUpload/" + pdfFileName;

                int updatedCount = _cm.UpdateAdditionalRecordDetails(_cm, relativeExcelPath, relativePdfPath);

                if (updatedCount > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", $"alert('Revision updated successfully!');window.location='WorkList_PV.aspx';", true);
                }
                else
                {
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
            string folderPath = Server.MapPath("~/FilesUpload/");
            string safeProductName = productName.Replace(" ", "_");

            string basePattern = $"{reportId}_{sampleId}_{safeProductName}";

            var allFiles = Directory.GetFiles(folderPath, "*.xls*").Where(f => Path.GetFileName(f).StartsWith(basePattern)).ToList();

            foreach (string file in allFiles)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                var match = Regex.Match(filename, @"_rev(\d{2})$");

                if (match.Success)
                {
                    int rev = int.Parse(match.Groups[1].Value);
                    if (rev >= nextRev)
                        nextRev = rev + 1;
                }
                else
                {
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
                DataTable dt = _cm.GetExistingTestResultCalibration(Convert.ToInt32(reportId), sampleId);

                return (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["Result"]?.ToString()));
            }
            catch
            {
                return false;
            }
        }

        private bool GenerateFinalPdf(DataTable dt, int reportId, int sampleId, string outputPdfPath)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                string templatePath = Server.MapPath("~/InputFiles/Transport.docx");
                if (!File.Exists(templatePath))
                {
                    ShowMessage("Template not found.");
                    return false;
                }

                wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Visible = false;
                doc = wordApp.Documents.Open(templatePath);

                ReplaceAllPlaceholders(doc, dt, reportId, sampleId);

                doc.ExportAsFixedFormat(outputPdfPath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);

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
                    Marshal.ReleaseComObject(doc);
                }
                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }
            }
        }

        private DataTable ReadExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            try
            {
                if (!File.Exists(filePath))
                {
                    ShowMessage($"File not found: {filePath}");
                    return dt;
                }

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);

                    // Table starts at row 8 (after the 6-row header + 1 blank row)
                    int tableStartRow = 8;
                    int lastRow = worksheet.LastRowUsed().RowNumber();

                    // If no data beyond header, return empty
                    if (lastRow < tableStartRow + 1)
                    {
                        ShowMessage($"No data rows found! Last row in Excel: {lastRow}, Table start row: {tableStartRow}");
                        return dt;
                    }

                    // Add columns from header row
                    int lastColumn = worksheet.LastColumnUsed().ColumnNumber();
                    for (int col = 1; col <= lastColumn; col++)
                    {
                        try
                        {
                            var cell = worksheet.Cell(tableStartRow, col);
                            string columnName = cell.GetString().Trim();

                            if (string.IsNullOrWhiteSpace(columnName))
                                columnName = $"Column{col}";

                            // Handle duplicate column names
                            string finalColumnName = columnName;
                            int suffix = 1;
                            while (dt.Columns.Contains(finalColumnName))
                            {
                                finalColumnName = $"{columnName}_{suffix}";
                                suffix++;
                            }

                            dt.Columns.Add(finalColumnName);
                        }
                        catch (Exception ex)
                        {
                            ShowMessage($"Error reading column {col}: {ex.Message}");
                            dt.Columns.Add($"Column{col}");
                        }
                    }

                    if (dt.Columns.Count == 0)
                    {
                        ShowMessage("No columns found in Excel!");
                        return dt;
                    }

                    // Read all data rows from tableStartRow + 1 to lastRow
                    int rowsAdded = 0;
                    for (int row = tableStartRow + 1; row <= lastRow; row++)
                    {
                        try
                        {
                            DataRow dr = dt.NewRow();
                            bool isEmptyRow = true;

                            for (int col = 0; col < dt.Columns.Count; col++)
                            {
                                try
                                {
                                    var cellValue = worksheet.Cell(row, col + 1).GetString().Trim();
                                    dr[col] = cellValue;

                                    if (!string.IsNullOrWhiteSpace(cellValue))
                                        isEmptyRow = false;
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error reading cell ({row},{col + 1}): {ex.Message}");
                                    dr[col] = "";
                                }
                            }

                            // Only add non-empty rows
                            if (!isEmptyRow)
                            {
                                dt.Rows.Add(dr);
                                rowsAdded++;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error reading row {row}: {ex.Message}");
                        }
                    }

                    // Final check
                    if (rowsAdded == 0)
                    {
                        ShowMessage($"All rows empty! Read from row {tableStartRow + 1} to {lastRow}, found {rowsAdded} non-empty rows");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Successfully read {rowsAdded} rows from Excel");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error reading Excel file: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ReadExcelToDataTable Exception: {ex.ToString()}");
            }

            return dt;
        }

        private void ShowMessage(string message)
        {
            message = message.Replace("'", "\\'"); // escape single quotes
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", $"alert('{message}');", true);
        }

    }
}