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
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;

using LIMS.DAL;
using OpenXmlPowerTools;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Excel = Microsoft.Office.Interop.Excel;


namespace LIMS
{
    public partial class CalibrationRecord : System.Web.UI.Page
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        DAL.CM _cm = new DAL.CM();
        private int reportId = 0;
        private int sampleId = 0;
        private string sampleIdSeries = "smp/2001";
        private string uploadFolder = "~/FilesUpload/";


        protected void Page_Load(object sender, EventArgs e)
        
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();

                int reportId = 0;
                int sampleId = 0;

                // Get ReportID and SampleID from query string
                if (Request.QueryString["r"] != null)
                    reportId = Convert.ToInt32(Request.QueryString["r"]);

                if (Request.QueryString["s"] != null)
                    sampleId = Convert.ToInt32(Request.QueryString["s"]);

                ViewState["ReportID"] = reportId;
                ViewState["SampleID"] = sampleId;

                // Ensure upload folder exists
                string physicalPath = Server.MapPath(uploadFolder);
                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);

                _cm.sampleId = sampleId;
                _cm.ReportID = reportId.ToString();

                // Fetch data
                DataTable dt1 = _cm.Getdataforseriesandreportid_calibration(_cm);
                DataTable dt2 = _cm.GetExistingdata_calibration(_cm);
                DataTable dtStatus = _cm.GetCalibrationSampleStatus(sampleId);

                if (dtStatus != null && dtStatus.Rows.Count > 0)
                {
                    DataRow statusRow = dtStatus.Rows[0];
                    int status = Convert.ToInt32(statusRow["CalibrationStatusID"]);

                    if (status == 40) // New Test
                    {
                        btnDownloadExisting.Visible = false;
                        btnUpdateUpload.Visible = false;
                        btnDownlaodNew.Visible = true;
                        btnConfirmUpload.Visible = true;

                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            DataRow row1 = dt1.Rows[0];
                            txtEquipmentBrand.Text = Convert.ToString(row1["Brand"]);
                            txtToolName.Text = Convert.ToString(row1["InstrumentName"]);
                            txtType.Text = Convert.ToString(row1["InstrumentType"]);
                            txtCalibrationLocation.Text = Convert.ToString(row1["Place"]);
                            //txtCalibrationMethod.Text = Convert.ToString(row1["CalibrationMethod"]);
                            //txtStandardsUsed.Text = Convert.ToString(row1["Standardsused"]);
                            txtReceiptDate.Text = Convert.ToString(row1["ToolReceiptDate"]);
                        }
                    }
                    else if (status == 1055) // Retest
                    {
                        LoadExistingTestResultForRetest(reportId, sampleId);

                        btnDownloadExisting.Visible = true;
                        btnUpdateUpload.Visible = true;
                        btnDownlaodNew.Visible = false;
                        btnConfirmUpload.Visible = false;

                        if (dt2 != null && dt2.Rows.Count > 0)
                        {
                            DataRow row2 = dt2.Rows[0];

                            txtAnalysisStart.Text = row2["AnalysisStartDate"] != DBNull.Value ? Convert.ToDateTime(row2["AnalysisStartDate"]).ToString("dd-MM-yyyy") : "";
                            txtCompleteDate.Text = row2["CompleteDate"] != DBNull.Value ? Convert.ToDateTime(row2["CompleteDate"]).ToString("dd-MM-yyyy") : "";
                            txtCalibrationDate.Text = row2["CalibrationDate"] != DBNull.Value ? Convert.ToDateTime(row2["CalibrationDate"]).ToString("dd-MM-yyyy") : "";
                            txtReceiptDate.Text = row2["ReceiptDate"] != DBNull.Value ? Convert.ToDateTime(row2["ReceiptDate"]).ToString("dd-MM-yyyy") : "";

                            txtCertificateNo.Text = Convert.ToString(row2["CertificateNo"]);
                            txtToolName.Text = Convert.ToString(row2["ToolName"]);
                            txtCalibrationLocation.Text = Convert.ToString(row2["CalibrationLocation"]);
                            txtSeriesNo.Text = Convert.ToString(row2["NoSeries"]);
                            txtCalibrationMethod.Text = Convert.ToString(row2["CalibrationMethod"]);
                            txtModelType.Text = Convert.ToString(row2["Model"]);
                            txtType.Text = Convert.ToString(row2["Type"]);
                            txtReference.Text = Convert.ToString(row2["Reference"]);
                            txtCapacity.Text = Convert.ToString(row2["Capacity"]);
                            txtRoomTempBegin.Text = Convert.ToString(row2["RoomTemperatureBegining"]);
                            txtRoomTempEnd.Text = Convert.ToString(row2["RoomTemperatureEnd"]);
                            txtAccuracy.Text = Convert.ToString(row2["Accuracy"]);
                            txtHumidityBegin.Text = Convert.ToString(row2["HumidityBegining"]);
                            txtHumidityEnd.Text = Convert.ToString(row2["HumidityEnd"]);
                            txtOwner.Text = Convert.ToString(row2["OwnerName"]);
                            txtAddress.Text = Convert.ToString(row2["Address"]);
                            txtStandardsUsed.Text = Convert.ToString(row2["Standardsused"]);
                            txtCalibratedBy.Text = Convert.ToString(row2["CalibratedBy"]);
                            txtCheckedBy.Text = Convert.ToString(row2["Checkedby"]);
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
                // Restore from ViewState
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

                    // Read Excel
                    DataTable dt = ReadExcelToDataTable(fullExcelPath);

                    using (var workbook = new XLWorkbook(fullExcelPath))
                    {
                        var ws = workbook.Worksheet(1);

                        // ===== Certificate Information =====
                        txtCertificateNo.Text = ws.Cell("B1").GetString();
                        txtCalibrationDate.Text = ws.Cell("E1").GetString();

                        // ===== Tool Information =====
                        txtToolName.Text = ws.Cell("B2").GetString();
                        txtCalibrationLocation.Text = ws.Cell("E2").GetString();

                        txtSeriesNo.Text = ws.Cell("B3").GetString();
                        txtCalibrationMethod.Text = ws.Cell("E3").GetString();

                        txtModelType.Text = ws.Cell("B4").GetString();
                        txtReference.Text = ws.Cell("E4").GetString();

                        // ===== Technical Details =====
                        txtCapacity.Text = ws.Cell("B5").GetString();
                        txtAccuracy.Text = ws.Cell("B6").GetString();

                        // ===== Environmental Conditions =====
                        txtRoomTempBegin.Text = ws.Cell("E5").GetString();
                        txtRoomTempEnd.Text = ws.Cell("F5").GetString();

                        txtHumidityBegin.Text = ws.Cell("E6").GetString();
                        txtHumidityEnd.Text = ws.Cell("F6").GetString();

                        // ===== Owner Information =====
                        txtOwner.Text = ws.Cell("B7").GetString();
                        txtAddress.Text = ws.Cell("B8").GetString();
                    }

                    // ===== Analysis Dates from DB =====
                    if (row["AnalysisStartingDate"] != DBNull.Value)
                    {
                        txtAnalysisStart.Text =
                            Convert.ToDateTime(row["AnalysisStartingDate"]).ToString("yyyy-MM-dd");
                    }

                    if (row["AnalysisCompletionDate"] != DBNull.Value)
                    {
                        txtCompleteDate.Text =
                            Convert.ToDateTime(row["AnalysisCompletionDate"]).ToString("yyyy-MM-dd");
                    }

                    // ===== Store for later use =====
                    Session["ExcelData"] = dt;
                    Session["IsRetest"] = true;
                    Session["ExistingFileName"] = fileName;
                    Session["PreviewFilePath"] = fullExcelPath;

                    ViewState["ExistingFilePath"] = excelPath;
                    ViewState["ExistingFileName"] = fileName;

                    // ===== UI Controls =====
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
            // Certificate details
            txtCertificateNo.Text = "";
            txtCalibrationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            // Tool details
            txtToolName.Text = "";
            txtCalibrationLocation.Text = "";
            txtSeriesNo.Text = "";
            txtCalibrationMethod.Text = "";
            txtModelType.Text = "";
            txtReference.Text = "";

            // Technical details
            txtCapacity.Text = "";
            txtAccuracy.Text = "";

            // Environmental conditions
            txtRoomTempBegin.Text = "";
            txtRoomTempEnd.Text = "";
            txtHumidityBegin.Text = "";
            txtHumidityEnd.Text = "";

            // Owner details
            txtOwner.Text = "";
            txtAddress.Text = "";
        }
        private DataTable GetReportDetailsFromDatabase(int reportId)
        {
            DataTable dt = _cm.GetDocNoByReportID(reportId);
            return dt;
        }

        // Helper method to get ExcelFileName with extension
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
            if (string.IsNullOrWhiteSpace(txtAnalysisStart.Text))
            {
                txtAnalysisStart.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtAnalysisStart.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtCompleteDate.Text))
            {
                txtCompleteDate.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtCompleteDate.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtToolName.Text))
            {
                txtToolName.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtToolName.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtCalibrationLocation.Text))
            {
                txtCalibrationLocation.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtCalibrationLocation.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtCertificateNo.Text))
            {
                txtCertificateNo.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtCertificateNo.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtSeriesNo.Text))
            {
                txtSeriesNo.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtSeriesNo.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtCalibrationMethod.Text))
            {
                txtCalibrationMethod.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtCalibrationMethod.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtCalibrationDate.Text))
            {
                txtCalibrationDate.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtCalibrationDate.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtModelType.Text))
            {
                txtModelType.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtModelType.BorderColor = System.Drawing.Color.Empty;

            if (string.IsNullOrWhiteSpace(txtOwner.Text))
            {
                txtOwner.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else txtOwner.BorderColor = System.Drawing.Color.Empty;

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


                    // ========== USE COMMON FUNCTION ==========
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
                        worksheet.Range("A1:E10").Clear();

                        // Define colors
                        var grayColor = XLColor.FromArgb(217, 217, 217); // #D9D9D9
                        var yellowColor = XLColor.FromArgb(255, 255, 204); // #FFFFCC

                        // Row 1: Certificate No. | Value | (Empty) | (Empty) | Calibration Date | Value | (Empty)
                        worksheet.Cell("A1").Value = "Certificate No.";
                        worksheet.Cell("A1").Style.Font.Bold = true;
                        worksheet.Cell("A1").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B1").Value = txtCertificateNo.Text;
                        worksheet.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C1").Value = "Calibration Date";
                        worksheet.Cell("C1").Style.Font.Bold = true;
                        worksheet.Cell("C1").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Range("D1:E1").Merge();
                        worksheet.Cell("D1").Value = txtCalibrationDate.Text;
                        worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 2: Tool Name | Value | (Empty) | (Empty) | Calibration Location | Value | (Empty)
                        worksheet.Cell("A2").Value = "Tool Name";
                        worksheet.Cell("A2").Style.Font.Bold = true;
                        worksheet.Cell("A2").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        worksheet.Cell("B2").Value = txtToolName.Text;
                        worksheet.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C2").Value = "Calibration Location";
                        worksheet.Cell("C2").Style.Font.Bold = true;
                        worksheet.Cell("C2").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Range("D2:E2").Merge();
                        worksheet.Cell("D2").Value = txtCalibrationLocation.Text;
                        worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 3: No. Series | Value | (Empty) | (Empty) | Calibration Method | Value | (Empty)
                        worksheet.Cell("A3").Value = "No. Series";
                        worksheet.Cell("A3").Style.Font.Bold = true;
                        worksheet.Cell("A3").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B3").Value = txtSeriesNo.Text;
                        worksheet.Cell("B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C3").Value = "Calibration Method";
                        worksheet.Cell("C3").Style.Font.Bold = true;
                        worksheet.Cell("C3").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Range("D3:E3").Merge();
                        worksheet.Cell("D3").Value = txtCalibrationMethod.Text;
                        worksheet.Cell("D3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 4: Model / Type | Value | (Empty) | (Empty) | Ref. | Value | (Empty)
                        worksheet.Cell("A4").Value = "Model / Type";
                        worksheet.Cell("A4").Style.Font.Bold = true;
                        worksheet.Cell("A4").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("B4").Value = txtModelType.Text;
                        worksheet.Cell("B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C4").Value = "Ref.";
                        worksheet.Cell("C4").Style.Font.Bold = true;
                        worksheet.Cell("C4").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Range("D4:E4").Merge();
                        worksheet.Cell("D4").Value = txtReference.Text;
                        worksheet.Cell("D4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 5: Type | Value | (Empty) | (Empty) | Room Conditions | Beginning | End
                        worksheet.Cell("A5").Value = "Type";
                        worksheet.Cell("A5").Style.Font.Bold = true;
                        worksheet.Cell("A5").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("B5").Value = txtType.Text;
                        worksheet.Cell("B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C5").Value = "Room Conditions";
                        worksheet.Cell("C5").Style.Font.Bold = true;
                        worksheet.Cell("C5").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("D5").Value = "Beginning";
                        worksheet.Cell("D5").Style.Font.Bold = true;
                        worksheet.Cell("D5").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("D5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("E5").Value = "End";
                        worksheet.Cell("E5").Style.Font.Bold = true;
                        worksheet.Cell("E5").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("E5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        // Row 6: Capacity | Value | (Empty) | (Empty) | Room Temperature | °C Beginning | °C End
                        worksheet.Cell("A6").Value = "Capacity";
                        worksheet.Cell("A6").Style.Font.Bold = true;
                        worksheet.Cell("A6").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        worksheet.Cell("B6").Value = txtCapacity.Text;

                        worksheet.Cell("C6").Value = "Room Temperature";
                        worksheet.Cell("C6").Style.Font.Bold = true;
                        worksheet.Cell("C6").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        worksheet.Cell("D6").Value = txtRoomTempBegin.Text + " °C";
                        worksheet.Cell("D6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("E6").Value = txtRoomTempEnd.Text + " °C";
                        worksheet.Cell("E6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        // Row 7: Resolution | Value | (Empty) | (Empty) | Humidity | %RH Beginning | %RH End
                        worksheet.Cell("A7").Value = "Resolution";
                        worksheet.Cell("A7").Style.Font.Bold = true;
                        worksheet.Cell("A7").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        worksheet.Cell("B7").Value = txtResolution.Text;
                        worksheet.Cell("B7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        worksheet.Cell("C7").Value = "Humidity";
                        worksheet.Cell("C7").Style.Font.Bold = true;
                        worksheet.Cell("C7").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("C7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                        worksheet.Cell("D7").Value = txtHumidityBegin.Text + " %RH";
                        worksheet.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("E7").Value = txtHumidityEnd.Text + " %RH";
                        worksheet.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 8: Owner Name | Value (spanning B8 to F8) | (Empty)
                        worksheet.Cell("A8").Value = "Owner Name";
                        worksheet.Cell("A8").Style.Font.Bold = true;
                        worksheet.Cell("A8").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Range("B8:E8").Merge();
                        worksheet.Cell("B8").Value = txtOwner.Text;
                        worksheet.Cell("B8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Row 9: Address | Value (spanning B9 to F9) | (Empty)
                        worksheet.Cell("A9").Value = "Address";
                        worksheet.Cell("A9").Style.Font.Bold = true;
                        worksheet.Cell("A9").Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell("A9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        worksheet.Range("B9:E9").Merge();
                        worksheet.Cell("B9").Value = txtAddress.Text;
                       


                        worksheet.Cell("A10").Value = "Receipt Date";
                        worksheet.Cell("A10").Style.Font.Bold = true;
                        worksheet.Cell("A10").Style.Fill.BackgroundColor = grayColor;

                        worksheet.Range("B10:E10").Merge();
                        worksheet.Cell("B10").Value = txtReceiptDate.Text;
                        worksheet.Cell("B9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        // Apply borders to all cells (A1:E9)
                        var dataRange = worksheet.Range("A1:E10");
                        dataRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // Center align all cells
                        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        // Adjust column widths
                        worksheet.Column(1).Width = 20; // Column A
                        worksheet.Column(2).Width = 15; // Column B
                        worksheet.Column(3).Width = 20;  // Column C 
                        worksheet.Column(4).Width = 20;  // Column D 
                        worksheet.Column(5).Width = 20; // Column E

                        // ========== ADD FOOTER TABLE AT THE END ==========
                        int footerStartRow = GetFooterStartRow(reportId);

                        // LEFT TABLE (5 rows × 2 columns)
                        worksheet.Cell(footerStartRow, 1).Value = "Standards used";
                        worksheet.Cell(footerStartRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow, 1).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow, 2).Value = txtStandardsUsed.Text;

                        worksheet.Cell(footerStartRow + 1, 1).Value = "Brand";
                        worksheet.Cell(footerStartRow + 1, 1).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow + 1, 1).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow + 1, 2).Value = txtEquipmentBrand.Text;

                        worksheet.Cell(footerStartRow + 2, 1).Value = "No. Series";
                        worksheet.Cell(footerStartRow + 2, 1).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow + 2, 1).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow + 2, 2).Value = txtNoSeries.Text;

                        worksheet.Cell(footerStartRow + 3, 1).Value = "Traced to SI";
                        worksheet.Cell(footerStartRow + 3, 1).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow + 3, 1).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow + 3, 2).Value = txtTracedToSI.Text;

                        worksheet.Cell(footerStartRow + 4, 1).Value = "Uncertainty";
                        worksheet.Cell(footerStartRow + 4, 1).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow + 4, 1).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow + 4, 2).Value = txtUncertainty.Text;

                        // LEFT TABLE borders
                        var leftTableRange = worksheet.Range(footerStartRow, 1, footerStartRow + 4, 2);
                        leftTableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        leftTableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        leftTableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        leftTableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        leftTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        leftTableRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        leftTableRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        // RIGHT TABLE (5 rows × 2 columns) - Column D and E
                        worksheet.Cell(footerStartRow, 4).Value = "Calibrated By :";
                        worksheet.Cell(footerStartRow, 4).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow, 4).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Cell(footerStartRow, 5).Value = "Checked by :";
                        worksheet.Cell(footerStartRow, 5).Style.Font.Bold = true;
                        worksheet.Cell(footerStartRow, 5).Style.Fill.BackgroundColor = grayColor;
                        worksheet.Cell(footerStartRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Merge cells for signature area
                        worksheet.Range(footerStartRow + 1, 4, footerStartRow + 4, 4).Merge();
                        worksheet.Cell(footerStartRow + 1, 4).Value = txtCalibratedBy.Text;
                        worksheet.Cell(footerStartRow + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(footerStartRow + 1, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;

                        worksheet.Range(footerStartRow + 1, 5, footerStartRow + 4, 5).Merge();
                        worksheet.Cell(footerStartRow + 1, 5).Value = txtCheckedBy.Text;
                        worksheet.Cell(footerStartRow + 1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(footerStartRow + 1, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;

                        // RIGHT TABLE borders
                        var rightTableRange = worksheet.Range(footerStartRow, 4, footerStartRow + 4, 5);
                        rightTableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        rightTableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        rightTableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        rightTableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        rightTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


                        // Save and download
                        string tempPath = Server.MapPath($"~/Temp/temp_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                        Directory.CreateDirectory(Server.MapPath("~/Temp/"));
                        workbook.SaveAs(tempPath);

                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //  Response.AddHeader("Content-Disposition", $"attachment; filename=CalibrationReport_{DateTime.Now:yyyyMMdd}.xlsx");
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
        private static readonly Dictionary<int, int> FooterStartRows = new Dictionary<int, int>
{
    { 5305, 34 },
    { 5306, 54 },
    { 5307, 46 },
    { 5308, 73 },
    { 5309, 45 },
    { 5310, 53 },
    { 5311, 52 },
    { 5312, 56 },
    { 5313, 55 },
    { 5314, 42 },
    { 5315, 55 },
    { 5316, 50 },
    { 5317, 45 },
    { 5318, 40 },
    { 5319, 40 },
    { 5320, 40 },
    { 5321, 33 },
    { 5322, 34 },
    { 5323, 44 },
    { 5324, 54 },
    { 5325, 40 },
    { 5326, 43 },
    { 5327, 34 },
    { 5328, 33 },
    { 5329, 50 },
    { 5330, 53 },
    { 5331, 37 },
    { 5332, 46 },
    { 5333, 46 },
    { 5334, 45 },
    { 5335, 54 },
    { 5336, 70 },     
    { 5337, 26 },
    { 5338, 25 },
    { 5339, 42 },
    { 5340, 42 },
    { 5341, 39},
    { 5342, 55 },
    { 5343, 41 },
    { 5344, 76 }
};
        private int GetFooterStartRow(int reportId)
        {
            return FooterStartRows.TryGetValue(reportId, out int row) ? row : 30;
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

                    txtCertificateNo.Text = ws.Cell("B1").GetString();
                    txtCalibrationDate.Text = ws.Cell("D1").GetFormattedString();

                    txtToolName.Text = ws.Cell("B2").GetString();
                    txtCalibrationLocation.Text = ws.Cell("D2").GetString();

                    txtSeriesNo.Text = ws.Cell("B3").GetString();
                    txtCalibrationMethod.Text = ws.Cell("D3").GetString();

                    txtModelType.Text = ws.Cell("B4").GetString();
                    txtReference.Text = ws.Cell("D4").GetString();

                    // Row 5
                    txtType.Text = ws.Cell("B5").GetString();

                    // Row 6 - Room Temperature
                    txtCapacity.Text = ws.Cell("B6").GetString();

                    txtRoomTempBegin.Text = ws.Cell("D6").GetFormattedString(); // 25 °C
                    txtRoomTempEnd.Text = ws.Cell("E6").GetFormattedString(); // 25 °C

                    // Row 7 - Humidity
                    txtResolution.Text = ws.Cell("B7").GetString();

                    txtHumidityBegin.Text = ws.Cell("D7").GetFormattedString(); // 15 %RH
                    txtHumidityEnd.Text = ws.Cell("E7").GetFormattedString(); // 45 %RH
                    txtOwner.Text = ws.Cell("B8").GetString();   // merged B8:E8
                    txtAddress.Text = ws.Cell("B9").GetString(); // merged B9:E9
                    txtReceiptDate.Text = ws.Cell("B10").GetFormattedString();


                    int footerStartRow = GetFooterStartRow(reportIdInt);

                    // Read footer values from Excel
                    ViewState["StandardsUsed"] = ws.Cell(footerStartRow, 2).GetString().Trim();
                    ViewState["EquipmentBrand"] = ws.Cell(footerStartRow + 1, 2).GetString().Trim();
                    ViewState["NoSeries"] = ws.Cell(footerStartRow + 2, 2).GetString().Trim();
                    ViewState["TracedToSI"] = ws.Cell(footerStartRow + 3, 2).GetString().Trim();
                    ViewState["Uncertainty"] = ws.Cell(footerStartRow + 4, 2).GetString().Trim();
                    ViewState["CalibratedBy"] = ws.Cell(footerStartRow + 1, 4).GetString().Trim();
                    ViewState["CheckedBy"] = ws.Cell(footerStartRow + 1, 5).GetString().Trim();
                }

                // Store ALL values in ViewState
                ViewState["CertificateNo"] = txtCertificateNo.Text;
                ViewState["CalibrationDate"] = txtCalibrationDate.Text;
                ViewState["ToolName"] = txtToolName.Text;
                ViewState["CalibrationLocation"] = txtCalibrationLocation.Text;
                ViewState["CalibrationMethod"] = txtCalibrationMethod.Text;
                ViewState["SeriesNo"] = txtSeriesNo.Text;
                ViewState["ModelType"] = txtModelType.Text;
                ViewState["Reference"] = txtReference.Text;
                ViewState["Type"] = txtType.Text;
                ViewState["Capacity"] = txtCapacity.Text;
                ViewState["Resolution"] = txtResolution.Text;
                ViewState["RoomTempBegin"] = txtRoomTempBegin.Text;
                ViewState["RoomTempEnd"] = txtRoomTempEnd.Text;
                ViewState["HumidityBegin"] = txtHumidityBegin.Text;
                ViewState["HumidityEnd"] = txtHumidityEnd.Text;
                ViewState["Owner"] = txtOwner.Text;
                ViewState["Address"] = txtAddress.Text;
                ViewState["ReceiptDate"] = txtReceiptDate.Text;
                ViewState["CalibratedBy"] = txtCalibratedBy.Text;
                ViewState["CheckedBy"] = txtCheckedBy.Text;

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
                string templatePath = Server.MapPath("~/InputFiles/Calibration.docx");
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
                string previewDir = Server.MapPath("~/PreviewFiles_pv/");
                Directory.CreateDirectory(previewDir);
                string browserAccessPath = Path.Combine(previewDir, pdfFileName);
                File.Copy(tempPdfPath, browserAccessPath, true);

                // Open in new tab
                string url = ResolveUrl("~/PreviewFiles_pv/" + pdfFileName);
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
            // 1. Create HEADER TABLE
            DataTable headerTable = new DataTable();
            headerTable.Columns.Add("Label1");
            headerTable.Columns.Add("Value1");
            headerTable.Columns.Add("Label2");
            headerTable.Columns.Add("Value2");

            headerTable.Rows.Add("Certificate No.", ViewState["CertificateNo"]?.ToString() ?? "",
                "Calibration Date", ViewState["CalibrationDate"]?.ToString() ?? "");
            headerTable.Rows.Add("Tool Name", ViewState["ToolName"]?.ToString() ?? "",
                "Calibration Location", ViewState["CalibrationLocation"]?.ToString() ?? "");
            headerTable.Rows.Add("No. Series", ViewState["SeriesNo"]?.ToString() ?? "",
                "Calibration Method", ViewState["CalibrationMethod"]?.ToString() ?? "");
            headerTable.Rows.Add("Model / Type", ViewState["ModelType"]?.ToString() ?? "",
                "Ref.", ViewState["Reference"]?.ToString() ?? "");
            headerTable.Rows.Add("Type", ViewState["Type"]?.ToString() ?? "",
                "Room Conditions", "Beginning / End");
            headerTable.Rows.Add("Capacity", ViewState["Capacity"]?.ToString() ?? "",
                "Room Temperature", ViewState["RoomTempBegin"]?.ToString() + ViewState["RoomTempEnd"]?.ToString());
            headerTable.Rows.Add("Resolution", ViewState["Resolution"]?.ToString() ?? "",
                "Humidity", ViewState["HumidityBegin"]?.ToString() + " %RH / " + ViewState["HumidityEnd"]?.ToString());
            headerTable.Rows.Add("Owner Name", ViewState["Owner"]?.ToString() ?? "", "", "");
            headerTable.Rows.Add("Address", ViewState["Address"]?.ToString() ?? "", "", "");
            headerTable.Rows.Add("ReceiptDate", ViewState["ReceiptDate"]?.ToString() ?? "",
          "ReceiptDate", ViewState["ReceiptDate"]?.ToString() ?? "");
            // 2. Create FOOTER TABLE
            DataTable footerTable = new DataTable();
            footerTable.Columns.Add("Col1");
            footerTable.Columns.Add("Col2");
            footerTable.Columns.Add("Col3");
            footerTable.Columns.Add("Col4");

            string standardsUsed = ViewState["StandardsUsed"]?.ToString() ?? "";
            string equipmentBrand = ViewState["EquipmentBrand"]?.ToString() ?? "";
            string noSeries = ViewState["NoSeries"]?.ToString() ?? "";
            string tracedToSI = ViewState["TracedToSI"]?.ToString() ?? "";
            string uncertainty = ViewState["Uncertainty"]?.ToString() ?? "";
            string calibratedBy = ViewState["CalibratedBy"]?.ToString() ?? "";
            string CheckedBy = ViewState["CheckedBy"]?.ToString() ?? "";

            footerTable.Rows.Add("Standards Used", ViewState["StandardsUsed"]?.ToString() ?? "",
                "Equipment Brand", ViewState["EquipmentBrand"]?.ToString() ?? "");
            footerTable.Rows.Add("No. Series", ViewState["NoSeries"]?.ToString() ?? "",
                "Traced to SI", ViewState["TracedToSI"]?.ToString() ?? "");
            footerTable.Rows.Add("Uncertainty", ViewState["Uncertainty"]?.ToString() ?? "",
                "Uncertainty", ViewState["Uncertainty"]?.ToString() ?? "");

            footerTable.Rows.Add("calibratedBy", ViewState["calibratedBy"]?.ToString() ?? "",
                "calibratedBy", ViewState["calibratedBy"]?.ToString() ?? "");

            footerTable.Rows.Add("CheckedBy", ViewState["CheckedBy"]?.ToString() ?? "",
                "CheckedBy", ViewState["CheckedBy"]?.ToString() ?? "");

            DataTable dtReport = GetReportDetailsFromDatabase(reportId);
            string documentNumber = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["docno"]?.ToString() ?? "" : "";
            string reportName = dtReport.Rows.Count > 0 ? dtReport.Rows[0]["ReportName"]?.ToString() ?? "" : "";

            // 4. Replace header placeholders
                ReplaceInHeaders(doc, "{docno}", documentNumber);
            ReplaceInHeaders(doc, "{reportname}", reportName);

            // 5. Replace tables in ORDER - ONLY ONCE EACH
            ReplaceTablePlaceholderCommontable(doc, "{headerTable}", headerTable);
            ReplaceAllExcelTables(doc, "{MainResultTable}", reportId);  



            DataTable leftFooterTable = new DataTable();
            leftFooterTable.Columns.Add("Label");
            leftFooterTable.Columns.Add("Value");

            leftFooterTable.Rows.Add("Standards used", ViewState["StandardsUsed"]?.ToString() ?? "");
            leftFooterTable.Rows.Add("Brand", ViewState["EquipmentBrand"]?.ToString() ?? "");
            leftFooterTable.Rows.Add("No. Series", ViewState["NoSeries"]?.ToString() ?? "");
            leftFooterTable.Rows.Add("Traced to SI", ViewState["TracedToSI"]?.ToString() ?? "");
            leftFooterTable.Rows.Add("Uncertainty", ViewState["Uncertainty"]?.ToString() ?? "");

            DataTable rightFooterTable = new DataTable();
            rightFooterTable.Columns.Add("Col1");
            rightFooterTable.Columns.Add("Col2");

            rightFooterTable.Rows.Add("Calibrated By :", "Checked By :");
            rightFooterTable.Rows.Add(ViewState["CalibratedBy"]?.ToString() ?? "", ViewState["CheckedBy"]?.ToString() ?? "");

            ReplaceFooterTables(doc, "{footerTable}", leftFooterTable, rightFooterTable);


            //rightFooterTable.Rows.Add("Calibrated By :", "Checked by :");
            //rightFooterTable.Rows.Add(ViewState["CalibratedBy"]?.ToString() ?? "", ""); 
            ReplaceFooterTables(doc, "{footerTable}", leftFooterTable, rightFooterTable);

        }
        private void ReplaceFooterTables(Microsoft.Office.Interop.Word.Document doc, string placeholder,
         DataTable leftTable, DataTable rightTable)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();

            if (range.Find.Execute(FindText: placeholder))
            {
                range.Text = "";

                int lightGrayR = 230, lightGrayG = 230, lightGrayB = 230;
                var blackColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;

                //  CREATE ONE COMBINED TABLE (5 rows × 5 columns)
                // Columns: LeftLabel | LeftValue | Gap | RightCol1 | RightCol2
                Microsoft.Office.Interop.Word.Table combinedTable = doc.Tables.Add(range, 5, 5);
                combinedTable.Borders.Enable = 1;

                // === LEFT SIDE (Columns 1-2) ===
                for (int i = 0; i < leftTable.Rows.Count; i++)
                {
                    // Column 1 - Labels (bold, gray)
                    combinedTable.Cell(i + 1, 1).Range.Text = leftTable.Rows[i][0]?.ToString() ?? "";
                    combinedTable.Cell(i + 1, 1).Range.Font.Bold = 1;
                    combinedTable.Cell(i + 1, 1).Range.Font.Size = 10;
                    combinedTable.Cell(i + 1, 1).Shading.BackgroundPatternColor =
                        (Microsoft.Office.Interop.Word.WdColor)(lightGrayR + 0x100 * lightGrayG + 0x10000 * lightGrayB);

                    // Column 2 - Values (white)
                    combinedTable.Cell(i + 1, 2).Range.Text = leftTable.Rows[i][1]?.ToString() ?? "";
                    combinedTable.Cell(i + 1, 2).Range.Font.Size = 10;
                    combinedTable.Cell(i + 1, 2).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                }

                // === MIDDLE GAP (Column 3) - No borders, invisible ===
                for (int i = 1; i <= 5; i++)
                {
                    combinedTable.Cell(i, 3).Range.Text = "";
                    combinedTable.Cell(i, 3).Borders.Enable = 0;
                    combinedTable.Cell(i, 3).Width = 20; // Small gap
                }

                // === RIGHT SIDE (Columns 4-5) ===
                // Row 1 - Headers
                combinedTable.Cell(1, 4).Range.Text = rightTable.Rows[0][0]?.ToString() ?? "";
                combinedTable.Cell(1, 4).Range.Font.Bold = 1;
                combinedTable.Cell(1, 4).Range.Font.Size = 10;
                combinedTable.Cell(1, 4).Shading.BackgroundPatternColor =
                    (Microsoft.Office.Interop.Word.WdColor)(lightGrayR + 0x100 * lightGrayG + 0x10000 * lightGrayB);

                combinedTable.Cell(1, 5).Range.Text = rightTable.Rows[0][1]?.ToString() ?? "";
                combinedTable.Cell(1, 5).Range.Font.Bold = 1;
                combinedTable.Cell(1, 5).Range.Font.Size = 10;
                combinedTable.Cell(1, 5).Shading.BackgroundPatternColor =
                    (Microsoft.Office.Interop.Word.WdColor)(lightGrayR + 0x100 * lightGrayG + 0x10000 * lightGrayB);

                // Rows 2-5 - Merge for signature area
                combinedTable.Cell(2, 4).Merge(combinedTable.Cell(5, 4));
                combinedTable.Cell(2, 4).Range.Text = rightTable.Rows[1][0]?.ToString() ?? "";
                combinedTable.Cell(2, 4).Range.Font.Size = 10;
                combinedTable.Cell(2, 4).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                combinedTable.Cell(2, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalBottom;
                combinedTable.Cell(2, 4).Height = 5;

                combinedTable.Cell(2, 5).Merge(combinedTable.Cell(5, 5));
                combinedTable.Cell(2, 5).Range.Text = rightTable.Rows[1][1]?.ToString() ?? "";
                combinedTable.Cell(2, 5).Range.Font.Size = 10;
                combinedTable.Cell(2, 5).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                combinedTable.Cell(2, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalBottom;
                combinedTable.Cell(2, 5).Height = 5;

                // === BORDERS ===
                // Left table borders (columns 1-2)
                for (int row = 1; row <= 5; row++)
                {
                    for (int col = 1; col <= 2; col++)
                    {
                        combinedTable.Cell(row, col).Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        combinedTable.Cell(row, col).Borders.OutsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                        combinedTable.Cell(row, col).Borders.OutsideColor = blackColor;
                    }
                }

                // Right table borders (columns 4-5, only visible cells)
                for (int col = 4; col <= 5; col++)
                {
                    combinedTable.Cell(1, col).Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    combinedTable.Cell(1, col).Borders.OutsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                    combinedTable.Cell(1, col).Borders.OutsideColor = blackColor;

                    combinedTable.Cell(2, col).Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    combinedTable.Cell(2, col).Borders.OutsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                    combinedTable.Cell(2, col).Borders.OutsideColor = blackColor;
                }

                // Center alignment
                combinedTable.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                combinedTable.Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                combinedTable.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPercent;
                combinedTable.PreferredWidth = 100;
                combinedTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);

                range.InsertParagraphAfter();
            }
        }
        // NEW METHOD: Replace table WITHOUT header row
        private void ReplaceTablePlaceholderCommontable(Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();
            if (range.Find.Execute(FindText: placeholder))
            {
                range.Text = "";

                // Create table with 9 rows and 5 columns to match Excel structure
                Microsoft.Office.Interop.Word.Table wordTable = doc.Tables.Add(
                    range,
                    10, // 9 rows as per Excel
                    5  // 5 columns (A, B, C, D, E)
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
                string certificateNo = ViewState["CertificateNo"]?.ToString() ?? "";
                string calibrationDate = ViewState["CalibrationDate"]?.ToString() ?? "";
                string toolName = ViewState["ToolName"]?.ToString() ?? "";
                string calibrationLocation = ViewState["CalibrationLocation"]?.ToString() ?? "";
                string seriesNo = ViewState["SeriesNo"]?.ToString() ?? "";
                string calibrationMethod = ViewState["CalibrationMethod"]?.ToString() ?? "";
                string modelType = ViewState["ModelType"]?.ToString() ?? "";
                string reference = ViewState["Reference"]?.ToString() ?? "";
                string type = ViewState["Type"]?.ToString() ?? "";
                string capacity = ViewState["Capacity"]?.ToString() ?? "";
                string resolution = ViewState["Resolution"]?.ToString() ?? "";
                string roomTempBegin = ViewState["RoomTempBegin"]?.ToString() ?? "";
                string roomTempEnd = ViewState["RoomTempEnd"]?.ToString() ?? "";
                string humidityBegin = ViewState["HumidityBegin"]?.ToString() ?? "";
                string humidityEnd = ViewState["HumidityEnd"]?.ToString() ?? "";
                string owner = ViewState["Owner"]?.ToString() ?? "";
                string address = ViewState["Address"]?.ToString() ?? "";
                string ReceiptDate = ViewState["ReceiptDate"]?.ToString() ?? "";

                // ============= FILL ALL CELLS FIRST (BEFORE ANY MERGING) =============

                // ROW 1
                wordTable.Cell(1, 1).Range.Text = "Certificate No.";
                wordTable.Cell(1, 2).Range.Text = certificateNo;
                wordTable.Cell(1, 3).Range.Text = "Calibration Date";
                wordTable.Cell(1, 4).Range.Text = calibrationDate;
                wordTable.Cell(1, 5).Range.Text = ""; // Will be merged

                // ROW 2
                wordTable.Cell(2, 1).Range.Text = "Tool Name";
                wordTable.Cell(2, 2).Range.Text = toolName;
                wordTable.Cell(2, 3).Range.Text = "Calibration Location";
                wordTable.Cell(2, 4).Range.Text = calibrationLocation;
                wordTable.Cell(2, 5).Range.Text = ""; // Will be merged

                // ROW 3
                wordTable.Cell(3, 1).Range.Text = "No. Series";
                wordTable.Cell(3, 2).Range.Text = seriesNo;
                wordTable.Cell(3, 3).Range.Text = "Calibration Method";
                wordTable.Cell(3, 4).Range.Text = calibrationMethod;
                wordTable.Cell(3, 5).Range.Text = ""; // Will be merged

                // ROW 4
                wordTable.Cell(4, 1).Range.Text = "Model / Type";
                wordTable.Cell(4, 2).Range.Text = modelType;
                wordTable.Cell(4, 3).Range.Text = "Ref.";
                wordTable.Cell(4, 4).Range.Text = reference;
                wordTable.Cell(4, 5).Range.Text = ""; // Will be merged

                // ROW 5 - Headers for Room Conditions
                wordTable.Cell(5, 1).Range.Text = "Type";
                wordTable.Cell(5, 2).Range.Text = type;
                wordTable.Cell(5, 3).Range.Text = "Room Conditions";
                wordTable.Cell(5, 4).Range.Text = "Beginning";
                wordTable.Cell(5, 5).Range.Text = "End";

                // ROW 6 - Room Temperature
                wordTable.Cell(6, 1).Range.Text = "Capacity";
                wordTable.Cell(6, 2).Range.Text = capacity;
                wordTable.Cell(6, 3).Range.Text = "Room Temperature";
                wordTable.Cell(6, 4).Range.Text = roomTempBegin;
                wordTable.Cell(6, 5).Range.Text = roomTempEnd;

                // ROW 7 - Humidity
                wordTable.Cell(7, 1).Range.Text = "Resolution";
                wordTable.Cell(7, 2).Range.Text = resolution;
                wordTable.Cell(7, 3).Range.Text = "Humidity";
                wordTable.Cell(7, 4).Range.Text = humidityBegin;
                wordTable.Cell(7, 5).Range.Text = humidityEnd;

                // ROW 8 - Owner
                wordTable.Cell(8, 1).Range.Text = "Owner Name";
                wordTable.Cell(8, 2).Range.Text = owner;
                wordTable.Cell(8, 3).Range.Text = "";
                wordTable.Cell(8, 4).Range.Text = "";
                wordTable.Cell(8, 5).Range.Text = "";

                // ROW 9 - Address
                wordTable.Cell(9, 1).Range.Text = "Address";
                wordTable.Cell(9, 2).Range.Text = address;
                wordTable.Cell(9, 3).Range.Text = "";
                wordTable.Cell(9, 4).Range.Text = "";
                wordTable.Cell(9, 5).Range.Text = "";

                // ROW 10 - Receipt Date
                wordTable.Cell(10, 1).Range.Text = "Receipt Date";
                wordTable.Cell(10, 2).Range.Text = ReceiptDate;
                wordTable.Cell(10, 3).Range.Text = "";
                wordTable.Cell(10, 4).Range.Text = "";
                wordTable.Cell(10, 5).Range.Text = "";


                // ============= NOW DO ALL MERGING =============

                // Merge D1:E1 (Calibration Date)
                wordTable.Cell(1, 4).Merge(wordTable.Cell(1, 5));

                // Merge D2:E2 (Calibration Location)
                wordTable.Cell(2, 4).Merge(wordTable.Cell(2, 5));

                // Merge D3:E3 (Calibration Method)
                wordTable.Cell(3, 4).Merge(wordTable.Cell(3, 5));

                // Merge D4:E4 (Reference)
                wordTable.Cell(4, 4).Merge(wordTable.Cell(4, 5));

                // Merge B8:E8 (Owner)
                wordTable.Cell(8, 2).Merge(wordTable.Cell(8, 5));
         

                // Merge B9:E9 (Address)
                wordTable.Cell(9, 2).Merge(wordTable.Cell(9, 5));
               

                wordTable.Cell(10, 2).Merge(wordTable.Cell(10, 5));
            

                // ============= APPLY FORMATTING =============

                // Apply font size and vertical alignment to ALL cells
                for (int i = 1; i <= 10; i++)
                {
                    for (int j = 1; j <= wordTable.Rows[i].Cells.Count; j++)
                    {
                        try
                        {
                            wordTable.Cell(i, j).Range.Font.Size = 10;
                            wordTable.Cell(i, j).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                        catch { }
                    }
                }

                // Apply gray background and bold to label columns (Column A - all rows)
                for (int i = 1; i <= 10; i++)
                {
                    try
                    {
                        wordTable.Cell(i, 1).Range.Font.Bold = 1;
                        wordTable.Cell(i, 1).Shading.BackgroundPatternColor = grayColor;
                    }
                    catch { }
                }

                // Apply gray background and bold to Column C (rows 1-7 only)
                for (int i = 1; i <= 7; i++)
                {
                    try
                    {
                        wordTable.Cell(i, 3).Range.Font.Bold = 1;
                        wordTable.Cell(i, 3).Shading.BackgroundPatternColor = grayColor;
                    }
                    catch { }
                }

                // Row 5 columns D and E should also be bold and gray (headers)
                try
                {
                    wordTable.Cell(5, 4).Range.Font.Bold = 1;
                    wordTable.Cell(5, 4).Shading.BackgroundPatternColor = grayColor;
                    wordTable.Cell(5, 5).Range.Font.Bold = 1;
                    wordTable.Cell(5, 5).Shading.BackgroundPatternColor = grayColor;
                }
                catch { }

                // Set white background for all data cells (Column B - all rows)
                for (int i = 1; i <= 10; i++)
                {
                    try
                    {
                        wordTable.Cell(i, 2).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                    }
                    catch { }
                }

                // Set white background for merged cells in rows 1-4 (column D only, since D-E are merged)
                for (int i = 1; i <= 4; i++)
                {
                    try
                    {
                        wordTable.Cell(i, 4).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                    }
                    catch { }
                }

                // Set white background for rows 6-7 (both columns D and E)
                for (int i = 6; i <= 7; i++)
                {
                    try
                    {
                        wordTable.Cell(i, 4).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                        wordTable.Cell(i, 5).Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorWhite;
                    }
                    catch { }
                }

                // Set ALL borders to BLACK with thick lines
                wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                wordTable.Borders.OutsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                wordTable.Borders.InsideLineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
                wordTable.Borders.OutsideColor = blackColor;
                wordTable.Borders.InsideColor = blackColor;
;

     

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
        private void ReplaceAllExcelTables(Microsoft.Office.Interop.Word.Document doc,string placeholder,int reportId)
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

                int footerStartRow = GetFooterStartRow(reportId);

                List<Excel.Range> mainTables = new List<Excel.Range>();

                HashSet<int> processedRows = new HashSet<int>();

                int lastRow = ws.UsedRange.Rows.Count;

                // Detect tables - ONLY MAIN TABLES (before footerStartRow)
                for (int row = 12; row <= lastRow; row++)
                {
                    if (processedRows.Contains(row)) continue;

                    Excel.Range cell = ws.Cells[row, 1];
                    if (!string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                    {
                        Excel.Range tableRange = cell.CurrentRegion;

                        // ONLY add tables BEFORE footer start row
                        if (tableRange.Row < footerStartRow)
                        {
                            mainTables.Add(tableRange);
                        }

                        for (int r = tableRange.Row;
                             r < tableRange.Row + tableRange.Rows.Count;
                             r++)
                        {
                            processedRows.Add(r);
                        }
                    }
                    Marshal.ReleaseComObject(cell);
                }

                bool isFirstTable = true;

                // Paste main tables
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

                    // Borders - wrap in try-catch for merged cells
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
                    catch
                    {
                        // Skip if borders can't be applied due to merged cells
                    }

                    // Paragraph alignment - wrap in try-catch
                    try
                    {
                        wordTable.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                    catch
                    {
                        // Skip if alignment can't be applied
                    }

                    // Vertical alignment - wrap in try-catch (often fails with merged cells)
                    try
                    {
                        wordTable.Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }
                    catch
                    {
                        // Skip if vertical alignment can't be applied due to merged cells
                    }

                    // Width settings - wrap in try-catch
                    try
                    {
                        wordTable.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPercent;
                        wordTable.PreferredWidth = 100;
                    }
                    catch
                    {
                        // Skip if width settings can't be applied
                    }

                    // AutoFit - wrap in try-catch
                    try
                    {
                        wordTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);
                    }
                    catch
                    {
                        // Skip if autofit can't be applied
                    }

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

                // Force garbage collection
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
                        // Try again after delay
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
                    ShowMessage("File already exists with this name. This shouldn't happen!");
                    return;
                }

                fileUpload.SaveAs(finalFilePath);

                // ========== Read form data from Excel ==========
                string certificateNo = "";
                string calibrationDate = "";
                string toolName = "";
                string calibrationLocation = "";
                string seriesNo = "";
                string calibrationMethod = "";
                string modelType = "";
                string reference = "";
                string type = "";
                string capacity = "";
                string resolution = "";
                string roomTempBegin = "";
                string roomTempEnd = "";
                string humidityBegin = "";
                string humidityEnd = "";
                string owner = "";
                string address = "";
                string receiptDate = "";
                string standardsUsed = "";
                string equipmentBrand = "";
                string tracedToSI = "";
                string uncertainty = "";
                string calibratedBy = "";
                string checkedBy = "";

                using (var workbook = new XLWorkbook(finalFilePath))
                {
                    var ws = workbook.Worksheet(1);

                    certificateNo = ws.Cell("B1").GetString();
                    calibrationDate = ws.Cell("D1").GetFormattedString();
                    toolName = ws.Cell("B2").GetString();
                    calibrationLocation = ws.Cell("D2").GetString();
                    seriesNo = ws.Cell("B3").GetString();
                    calibrationMethod = ws.Cell("D3").GetString();
                    modelType = ws.Cell("B4").GetString();
                    reference = ws.Cell("D4").GetString();
                    type = ws.Cell("B5").GetString();
                    capacity = ws.Cell("B6").GetString();
                    roomTempBegin = ws.Cell("D6").GetFormattedString();
                    roomTempEnd = ws.Cell("E6").GetFormattedString();
                    resolution = ws.Cell("B7").GetString();
                    humidityBegin = ws.Cell("D7").GetFormattedString();
                    humidityEnd = ws.Cell("E7").GetFormattedString();
                    owner = ws.Cell("B8").GetString();
                    address = ws.Cell("B9").GetString();
                    receiptDate = ws.Cell("B10").GetFormattedString();

                    int footerStartRow = GetFooterStartRow(reportIdInt);
                    standardsUsed = ws.Cell(footerStartRow, 2).GetString().Trim();
                    equipmentBrand = ws.Cell(footerStartRow + 1, 2).GetString().Trim();
                    tracedToSI = ws.Cell(footerStartRow + 3, 2).GetString().Trim();
                    uncertainty = ws.Cell(footerStartRow + 4, 2).GetString().Trim();
                    calibratedBy = ws.Cell(footerStartRow + 1, 4).GetString().Trim();
                    checkedBy = ws.Cell(footerStartRow + 1, 5).GetString().Trim();
                }

                // ========== POPULATE VIEWSTATE BEFORE PDF GENERATION ==========
                ViewState["CertificateNo"] = certificateNo;
                ViewState["CalibrationDate"] = calibrationDate;
                ViewState["ToolName"] = toolName;
                ViewState["CalibrationLocation"] = calibrationLocation;
                ViewState["CalibrationMethod"] = calibrationMethod;
                ViewState["SeriesNo"] = seriesNo;
                ViewState["ModelType"] = modelType;
                ViewState["Reference"] = reference;
                ViewState["Type"] = type;
                ViewState["Capacity"] = capacity;
                ViewState["Resolution"] = resolution;
                ViewState["RoomTempBegin"] = roomTempBegin;
                ViewState["RoomTempEnd"] = roomTempEnd;
                ViewState["HumidityBegin"] = humidityBegin;
                ViewState["HumidityEnd"] = humidityEnd;
                ViewState["Owner"] = owner;
                ViewState["Address"] = address;
                ViewState["ReceiptDate"] = receiptDate;
                ViewState["StandardsUsed"] = standardsUsed;
                ViewState["EquipmentBrand"] = equipmentBrand;
                ViewState["NoSeries"] = seriesNo;
                ViewState["TracedToSI"] = tracedToSI;
                ViewState["Uncertainty"] = uncertainty;
                ViewState["CalibratedBy"] = calibratedBy;
                ViewState["CheckedBy"] = checkedBy;


                DataTable excelData = ReadExcelToDataTable(finalFilePath);

                if (excelData == null || excelData.Rows.Count == 0)
                {
                    ShowMessage("No data found in Excel file. Cannot generate PDF.");
                    if (File.Exists(finalFilePath))
                        File.Delete(finalFilePath);
                    return;
                }

                if (_cm == null) _cm = new DAL.CM();

                // Populate CM object with IDs and dates BEFORE saving
                _cm.sampleId = sampleId;
                _cm.ReportID = reportIdStr;
                _cm.sampleIDSeries = "smp/2001";

                // Parse dates from form
                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;
                DateTime? CalibrationDate = null;
                DateTime? ReceiptDate = null;

                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);

                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                if (!string.IsNullOrEmpty(txtCalibrationDate.Text))
                    CalibrationDate = DateTime.Parse(txtCalibrationDate.Text);

                if (!string.IsNullOrEmpty(txtReceiptDate.Text))
                    ReceiptDate = DateTime.Parse(txtReceiptDate.Text);


                // Populate CM object with all form values
                _cm.Brand = txtEquipmentBrand?.Text?.Trim() ?? "";
                _cm.CertificateNo = txtCertificateNo?.Text?.Trim() ?? "";
                _cm.CalibrationDate = CalibrationDate;
                _cm.ReceiptDate = ReceiptDate;
                _cm.ToolName = txtToolName?.Text?.Trim() ?? "";
                _cm.CalibrationLocation = txtCalibrationLocation?.Text?.Trim() ?? "";
                _cm.NoSeries = txtSeriesNo?.Text?.Trim() ?? "";
                _cm.CalibrationMethod = txtCalibrationMethod?.Text?.Trim() ?? "";
                _cm.Model = txtModelType?.Text?.Trim() ?? "";
                _cm.Type = txtType?.Text?.Trim() ?? "";
                _cm.Reference = txtReference?.Text?.Trim() ?? "";
                _cm.Capacity = txtCapacity?.Text?.Trim() ?? "";
                _cm.Resolution = txtResolution?.Text?.Trim() ?? "";
                _cm.OwnerName = txtOwner?.Text?.Trim() ?? "";
                _cm.Address = txtAddress?.Text?.Trim() ?? "";
                _cm.HumidityBegining = txtHumidityBegin?.Text?.Trim() ?? "";
                _cm.HumidityEnd = txtHumidityEnd?.Text?.Trim() ?? "";
                _cm.RoomTemperatureBegining = txtRoomTempBegin?.Text?.Trim() ?? "";
                _cm.RoomTemperatureEnd = txtRoomTempEnd?.Text?.Trim() ?? "";
                _cm.Standardsused = txtStandardsUsed?.Text?.Trim() ?? "";
                _cm.Uncertainty = txtUncertainty?.Text?.Trim() ?? "";
                _cm.TracedtoSI = txtTracedToSI?.Text?.Trim() ?? "";
                _cm.CalibratedBy = txtCalibratedBy?.Text?.Trim() ?? "";
                _cm.Checkedby = txtCheckedBy?.Text?.Trim() ?? "";

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

                // Save all data using AddAdditionalRecordDetails
                int savedCount = _cm.AddAdditionalRecordDetails(_cm, relativeExcelPath, relativePdfPath);

                if (savedCount > 0)
                {
                    // Update statuses - commented as per requirement
                    //_cm.sampleStatusID = 27;
                    //_cm.UpdateCRFSampleDetailsStatus(_cm);
                    //_cm.UpdateCRFCloneList_Pv(_cm);
                    //_cm.UpdateSampleDetails_Pv(_cm);
                    //_cm.StatusID = 27;
                    //_cm.UpdateAllocationStatusid_mechanical(_cm);

                    btnConfirmUpload.Visible = false;

                    string message = fileExistsInDB
                        ? $"Revision saved successfully!"
                        : $"Test results saved successfully";

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

                // Populate CM object with IDs
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

                // Parse dates from form
                DateTime? AnalysisStartingDate = null;
                DateTime? AnalysisCompletionDate = null;
                DateTime? CalibrationDate = null;
                DateTime? ReceiptDate = null;

                if (!string.IsNullOrEmpty(txtAnalysisStart.Text))
                    AnalysisStartingDate = DateTime.Parse(txtAnalysisStart.Text);

                if (!string.IsNullOrEmpty(txtCompleteDate.Text))
                    AnalysisCompletionDate = DateTime.Parse(txtCompleteDate.Text);

                if (!string.IsNullOrEmpty(txtCalibrationDate.Text))
                    CalibrationDate = DateTime.Parse(txtCalibrationDate.Text);

                if (!string.IsNullOrEmpty(txtReceiptDate.Text))
                    ReceiptDate = DateTime.Parse(txtReceiptDate.Text);


                // Populate CM object with all form values BEFORE updating
                _cm.Brand = txtEquipmentBrand?.Text?.Trim() ?? "";
                _cm.CertificateNo = txtCertificateNo?.Text?.Trim() ?? "";
                _cm.CalibrationDate = CalibrationDate;
                _cm.ReceiptDate = ReceiptDate;
    
                _cm.ToolName = txtToolName?.Text?.Trim() ?? "";
                _cm.CalibrationLocation = txtCalibrationLocation?.Text?.Trim() ?? "";
                _cm.NoSeries = txtSeriesNo?.Text?.Trim() ?? "";
                _cm.CalibrationMethod = txtCalibrationMethod?.Text?.Trim() ?? "";
                _cm.Model = txtModelType?.Text?.Trim() ?? "";
                _cm.Type = txtType?.Text?.Trim() ?? "";
                _cm.Reference = txtReference?.Text?.Trim() ?? "";
                _cm.Capacity = txtCapacity?.Text?.Trim() ?? "";
                _cm.Resolution = txtResolution?.Text?.Trim() ?? "";
                _cm.OwnerName = txtOwner?.Text?.Trim() ?? "";
                _cm.Address = txtAddress?.Text?.Trim() ?? "";
                _cm.HumidityBegining = txtHumidityBegin?.Text?.Trim() ?? "";
                _cm.HumidityEnd = txtHumidityEnd?.Text?.Trim() ?? "";
                _cm.RoomTemperatureBegining = txtRoomTempBegin?.Text?.Trim() ?? "";
                _cm.RoomTemperatureEnd = txtRoomTempEnd?.Text?.Trim() ?? "";
                _cm.Standardsused = txtStandardsUsed?.Text?.Trim() ?? "";
                _cm.Uncertainty = txtUncertainty?.Text?.Trim() ?? "";
                _cm.TracedtoSI = txtTracedToSI?.Text?.Trim() ?? "";
                _cm.CalibratedBy = txtCalibratedBy?.Text?.Trim() ?? "";
                _cm.Checkedby = txtCheckedBy?.Text?.Trim() ?? "";

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

                // Update using UpdateAdditionalRecordDetails - single method call
                int updatedCount = _cm.UpdateAdditionalRecordDetails(_cm, relativeExcelPath, relativePdfPath);

                if (updatedCount > 0)
                {
                    // Update statuses - commented as per requirement
                    //_cm.sampleStatusID = 27;
                    //_cm.UpdateCRFSampleDetailsStatus(_cm);
                    //_cm.UpdateCRFCloneList_Pv(_cm);
                    //_cm.UpdateSampleDetails_Pv(_cm);
                    //_cm.StatusID = 27;
                    //_cm.UpdateAllocationStatusid_mechanical(_cm);

                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", $"alert('Revision updated successfully!');window.location='WorkList_PV.aspx';", true);
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
            string folderPath = Server.MapPath("~/FilesUpload/");
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
                DataTable dt = _cm.GetExistingTestResultCalibration(Convert.ToInt32(reportId), sampleId);

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
                string templatePath = Server.MapPath("~/InputFiles/Calibration.docx");
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

            if (!File.Exists(filePath))
            {
                ShowMessage($"File not found: {filePath}");
                return dt;
            }

            int reportIdInt = 0;
            if (ViewState["ReportID"] != null)
                reportIdInt = Convert.ToInt32(ViewState["ReportID"]);
            else if (Request.QueryString["r"] != null)
                int.TryParse(Request.QueryString["r"], out reportIdInt);

            int footerStartRow = GetFooterStartRow(reportIdInt);

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                int tableStartRow = 12;

                int lastRow = worksheet.LastRowUsed().RowNumber();
                int endRow = Math.Min(footerStartRow - 1, lastRow);

                // DEBUG CHECK
                if (endRow < tableStartRow + 1)
                {
                    ShowMessage($"ERROR: No data rows! endRow={endRow}, footerStart={footerStartRow}, tableStart={tableStartRow}");
                    return dt;
                }

                // Add columns
                for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                {
                    var cell = worksheet.Cell(tableStartRow, col);
                    string columnName = cell.GetString().Trim();

                    if (string.IsNullOrWhiteSpace(columnName))
                        columnName = $"Column{col}";

                    string finalColumnName = columnName;
                    int suffix = 1;
                    while (dt.Columns.Contains(finalColumnName))
                    {
                        finalColumnName = $"{columnName}_{suffix}";
                        suffix++;
                    }
                    dt.Columns.Add(finalColumnName);
                }

                if (dt.Columns.Count == 0)
                {
                    ShowMessage("No columns found in Excel!");
                    return dt;
                }

                // Read data rows
                int rowsAdded = 0;
                for (int row = tableStartRow + 1; row <= endRow; row++)
                {
                    DataRow dr = dt.NewRow();
                    bool isEmptyRow = true;

                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        var cellValue = worksheet.Cell(row, col + 1).GetString().Trim();
                        dr[col] = cellValue;
                        if (!string.IsNullOrWhiteSpace(cellValue))
                            isEmptyRow = false;
                    }

                    if (!isEmptyRow)
                    {
                        dt.Rows.Add(dr);
                        rowsAdded++;
                    }
                }

                // FINAL DEBUG
                if (rowsAdded == 0)
                {
                    ShowMessage($"All rows empty! Read from row {tableStartRow + 1} to {endRow}, found {rowsAdded} rows");
                }
            }

            return dt;
        }
        private void ShowMessage(string message)
        {
            message = message.Replace("'", "\\'"); // escape single quotes
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", $"alert('{message}');", true);
        }
        // EXISTING METHOD: Replace table WITH header row (for data table)
        /*css*/
        //private void ReplaceAllExcelTables(Microsoft.Office.Interop.Word.Document doc, string placeholder)
        //{
        //    string excelPath = Path.Combine(Server.MapPath("~/TempFolder"), Guid.NewGuid().ToString() + ".xlsx");
        //    fileUpload.SaveAs(excelPath);

        //    Excel.Application excelApp = null;
        //    Excel.Workbook wb = null;
        //    Excel.Worksheet ws = null;

        //    try
        //    {
        //        excelApp = new Excel.Application();
        //        excelApp.Visible = false;
        //        wb = excelApp.Workbooks.Open(excelPath);
        //        ws = wb.Sheets[1];

        //        // Find placeholder in Word
        //        Microsoft.Office.Interop.Word.Range range = doc.Content;
        //        range.Find.ClearFormatting();

        //        if (!range.Find.Execute(FindText: placeholder))
        //        {
        //            return; // Placeholder not found, skip
        //        }

        //        range.Text = "";

        //        // Auto-detect ALL tables in Excel
        //        List<Excel.Range> tables = new List<Excel.Range>();
        //        HashSet<int> processedRows = new HashSet<int>();
        //        int lastRow = ws.UsedRange.Rows.Count;

        //        for (int row = 8; row <= lastRow; row++)
        //        {
        //            if (processedRows.Contains(row)) continue;

        //            Excel.Range cell = ws.Cells[row, 1];
        //            if (!string.IsNullOrWhiteSpace(cell.Value?.ToString()))
        //            {
        //                Excel.Range tableRange = cell.CurrentRegion;
        //                tables.Add(tableRange);

        //                // Mark all rows in this table as processed
        //                for (int r = tableRange.Row; r < tableRange.Row + tableRange.Rows.Count; r++)
        //                {
        //                    processedRows.Add(r);
        //                }
        //            }
        //            Marshal.ReleaseComObject(cell);
        //        }

        //        // Copy and paste each table
        //        bool isFirstTable = true;
        //        foreach (Excel.Range tableRange in tables)
        //        {
        //            // Clear all background colors before copying
        //
        //           tableRange.Interior.ColorIndex = Excel.XlColorIndex.xlColorIndexNone;

        //            tableRange.Copy();

        //            if (!isFirstTable)
        //            {
        //                range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
        //                range.InsertParagraphAfter();
        //                range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
        //            }

        //            range.Paste();

        //            // === FORMAT THE PASTED WORD TABLE ===
        //            Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

        //            // Apply built-in Word table style (choose your favorite)
        //            // : "Table Grid", "Medium Grid 1 Accent 1", "Light Shading", "Colorful Grid"
        //            wordTable.set_Style("Table Grid");

        //            // Set borders
        //            wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
        //            wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
        //            wordTable.Borders.InsideColor = Microsoft.Office.Interop.Word.WdColor.wdColorGray15;
        //            wordTable.Borders.OutsideColor = Microsoft.Office.Interop.Word.WdColor.wdColorGray25;

        //            // Make table fit the page width
        //            wordTable.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPercent;
        //            wordTable.PreferredWidth = 100;
        //            wordTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);

        //            // Apply header formatting if table has at least one row
        //            if (wordTable.Rows.Count >= 1)
        //            {
        //                Microsoft.Office.Interop.Word.Row headerRow = wordTable.Rows[1];

        //                // Always use Gray20 for header row
        //                headerRow.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorGray20;

        //                // Make header text bold and black
        //                headerRow.Range.Font.Bold = 1;
        //                headerRow.Range.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;

        //                // Center align header text
        //                headerRow.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
        //            }



        //            // Alternate row shading (for readability)
        //            for (int i = 2; i <= wordTable.Rows.Count; i++)
        //            {
        //                if (i % 2 == 0)
        //                {
        //                    wordTable.Rows[i].Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorGray05;
        //                }
        //            }

        //            // Center align text and table
        //            wordTable.Rows.Alignment = Microsoft.Office.Interop.Word.WdRowAlignment.wdAlignRowCenter;
        //            wordTable.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

        //            // Add spacing before/after
        //            wordTable.Range.ParagraphFormat.SpaceBefore = 6;
        //            wordTable.Range.ParagraphFormat.SpaceAfter = 6;

        //            isFirstTable = false;
        //            Marshal.ReleaseComObject(tableRange);
        //        }
        //    }
        //    finally
        //    {
        //        if (wb != null) { wb.Close(false); Marshal.ReleaseComObject(wb); }
        //        if (ws != null) Marshal.ReleaseComObject(ws);
        //        if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }

        //        if (File.Exists(excelPath))
        //        {
        //            try { File.Delete(excelPath); } catch { }
        //        }
        //    }
        //}

    }
}