using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using ZXing;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using DataTable = System.Data.DataTable;
using Table = Microsoft.Office.Interop.Word.Table;

namespace LIMS
{
    public partial class CreateReportFromTemplate_Transport : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DAL.CM _cm = new DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            Application wordApp = new Application();
            Document doc = null;
            try
            {
                int sampleID = 0;
                string CategoryName = "";
                string path = Server.MapPath("~/Inputfiles/");
                string pathout = Server.MapPath("~/TestReport/");

                if (Request.QueryString["s"] != null && Request.QueryString["c"] != null)
                {
                    sampleID = Convert.ToInt32(Request.QueryString["s"]);
                    ViewState["SampleIDGet"] = sampleID;
                    CategoryName = Request.QueryString["c"].ToString();
                }
                string s = "", c = "", t = "";

                if (Request.QueryString["s"] != null)
                    s = Request.QueryString["s"].ToString();
                _cm.sampleId = Convert.ToInt32(s);
                if (Request.QueryString["c"] != null)
                    c = Request.QueryString["c"].ToString();
                if (Request.QueryString["t"] != null)
                    t = Request.QueryString["t"].ToString();

                int ReportIDGet = Convert.ToInt32(Request.QueryString["c"].ToString());
                int ID = Convert.ToInt32(s); string SampleUID = "";
                DataTable dt = _cm.getSampleUID(ID);

                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    GenerateQR(SampleUID);
                }

                DataTable Transportdetails = _cm.getdetails_Transport(sampleID);
                string templatePath = "";
                //DataTable dt1 = _cm.getReportQuery(c);
                //switch case for report id 
                templatePath = path + "Transport_ReportTemplate.docx"; /* Static */



                doc = wordApp.Documents.Add(templatePath);
                string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                List<string> imagePaths = new List<string> { imageUrl };
                InsertImagesIntoDocument(wordApp, doc, "{QR_Code}", imagePaths);
                DataTable dt2 = new DataTable();

                if (Transportdetails != null && Transportdetails.Rows.Count > 0)
                {
                    DataRow row = Transportdetails.Rows[0];

                    string GetSafeValue(string columnName)
                    {
                        return row[columnName] == DBNull.Value || row[columnName] == null ? "" : row[columnName].ToString();
                    }

                    string GetSafeDate(string columnName)
                    {
                        return row[columnName] == DBNull.Value || row[columnName] == null ? "" : Convert.ToDateTime(row[columnName]).ToString("dd MMMM yyyy");
                    }

                    string GetSafeDate1(string columnName)
                    {
                        if (row[columnName] == DBNull.Value || row[columnName] == null)
                        {
                            return "";
                        }
                        string dateString = row[columnName].ToString();
                        string expectedFormat = "dd/MM/yyyy";
                        DateTime dateValue;
                        if (DateTime.TryParseExact(dateString, expectedFormat,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out dateValue))
                        {
                            return dateValue.ToString("dd MMMM yyyy");
                        }
                        else
                        {
                            return "";
                        }
                    }

                    FindAndReplace(wordApp, "{workordernumber}", GetSafeValue("WorkorderNumber"));
                    FindAndReplace(wordApp, "{samplesubmissiondate}", GetSafeValue("SampleReceivedDate"));
                    FindAndReplace(wordApp, "{TestMethod}", GetSafeValue("TestMethod"));
                    FindAndReplace(wordApp, "{testreportdate}", GetSafeValue("IssueDate"));
                    FindAndReplace(wordApp, "{TestPeriod}", GetSafeValue("TestPeriod"));
                    FindAndReplace(wordApp, "{ReportNumber}", GetSafeValue("TestReportNo"));
                    FindAndReplace(wordApp, "{Samplecondition}", GetSafeValue("SampleCondition"));
                    FindAndReplace(wordApp, "{Applicant}", GetSafeValue("Applicant"));
                    FindAndReplace(wordApp, "{Address}", GetSafeValue("Address"));
                    // or another field
                    FindAndReplace(wordApp, "{MaterialName/Code}", GetSafeValue("MaterialType"));
                    FindAndReplace(wordApp, "{Buyer'sName}", GetSafeValue("Buyer'sName")?.Trim());
                    FindAndReplace(wordApp, "{CompanyName}", GetSafeValue("Applicant"));
                    FindAndReplace(wordApp, "{CompanyAddress}", GetSafeValue("Address"));
                    FindAndReplace(wordApp, "{ManufacturerName}", GetSafeValue("FactoryName"));
                    FindAndReplace(wordApp, "{ManufacturerAddress}", GetSafeValue("FactoryAddress"));
                    FindAndReplace(wordApp, "{LetterNumber}", GetSafeValue("LotNumber"));
                    FindAndReplace(wordApp, "{ProductName}", GetSafeValue("ProductName"));
                    FindAndReplace(wordApp, "{ProductBrand}", GetSafeValue("BrandProduct"));
                    FindAndReplace(wordApp, "{ProductType}", GetSafeValue("MaterialType"));
                    FindAndReplace(wordApp, "{ProductSpecification}", GetSafeValue("ProductSpecification"));
                    FindAndReplace(wordApp, "{ProductColor}", GetSafeValue("Color"));
                    FindAndReplace(wordApp, "{ProductSerialNumber}", GetSafeValue("ProductSerialNo"));
                    FindAndReplace(wordApp, "{ProductQuantity}", GetSafeValue("ProductQuantity"));
                    FindAndReplace(wordApp, "{ProductReceptionDate}", GetSafeValue("IssueDate"));
                    FindAndReplace(wordApp, "{DateOfTesting}", GetSafeValue("TestPeriod"));
                    ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
                    FindAndReplace(wordApp, "{TestReportNo.}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{DocNo}", "");

                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                }

                if (ViewState["SampleDescription"] != null)
                {
                    string uniqueFileName = SampleUID;
                    string outputDocxPath = pathout + "TestReport-" + uniqueFileName + ".docx";
                    string outputPdfPath = pathout + "TestReport-" + uniqueFileName + ".pdf";

                    DataTable dt3 = _cm.getdetailsForImages_Transport(ID);
                    List<Tuple<string, string>> imageData = new List<Tuple<string, string>>();
                    foreach (DataRow row in dt3.Rows)
                    {
                        string imageFileName = row["filename"].ToString().Trim();
                        string imageName = row["SampleText"].ToString();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (File.Exists(fullImagePath))
                            {
                                imageData.Add(Tuple.Create(fullImagePath, imageName));
                            }
                            else
                            {
                                Console.WriteLine($"Image not found: {fullImagePath}");
                            }
                        }
                    }
                    InsertImagesIntoDocument1(wordApp, doc, "{images}", imageData);


                    int cloneCRFSampleID = 0;
                    int reportID = 0;
                    bool b1 = false; bool b2 = false; bool b3 = false;

                    DataTable dtCloneCRFSampleIDGet = _cm.CRFSampleDetails_Transport(ID);

                    if (dtCloneCRFSampleIDGet != null && dtCloneCRFSampleIDGet.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtCloneCRFSampleIDGet.Rows)
                        {
                            cloneCRFSampleID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["CloneCRFSampleID"].ToString());
                            reportID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["ReportID"].ToString());

                            if (reportID == 4)
                            {
                                b1 = true;
                            }

                            if (reportID == 4)
                            {
                                b2 = true;
                            }
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'GenerateReport.aspx'; }, 10);</script>");
                    }

                    if (b1 == true)
                    {
                        DataTable dtReport = GetReportData(ID, reportID);
                        CreateReportFromExcel(wordApp, doc, "{testrecord}", ID, reportID);

                        try
                        {
                            string directory = Path.GetDirectoryName(outputPdfPath);
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                            // Save documents
                            doc.SaveAs2(outputDocxPath);
                            doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);

                            /* Clean up COM objects */
                            Marshal.ReleaseComObject(doc);
                            doc = null;
                            /* Return PDF to client */
                            Response.Clear();
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("content-disposition", "inline; filename=TestReport-" + uniqueFileName + ".pdf");
                            Response.WriteFile(outputPdfPath);
                            Response.End();
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = "Error generating PDF: " + ex.Message;
                            if (ex.InnerException != null)
                            {
                                errorMsg += "\nInner Exception: " + ex.InnerException.Message;
                            }
                            Response.Write(errorMsg);
                        }
                        finally
                        {
                            if (doc != null)
                            {
                                Marshal.ReleaseComObject(doc);
                            }
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }

                    if (b2 == true)
                    {

                    }

                }
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
                ///*Force - kill any lingering WINWORD processes started by this user session for locally server */
                foreach (var process in Process.GetProcessesByName("WINWORD"))
                {
                    if (string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        process.Kill();
                    }
                }
            }
        }
        protected void GenerateQR(string SampleID)
        {
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string QRCodeFilePath = baseUrl + "/TestReport/TestReport-" + SampleID + ".pdf";
            string QRCodeFileName = SampleID + ".png";

            /* Barcode generation and saving */
            string barcodeFolderPath = Server.MapPath("~/TestReport/");
            if (!Directory.Exists(barcodeFolderPath))
            {
                Directory.CreateDirectory(barcodeFolderPath);
            }

            /* Rest of  barcode generation code remains the same */
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            /* Choose a barcode format */
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            Bitmap barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);
            /* Save the barcode image to the folder */
            string barcodeFileName = QRCodeFileName;
            string barcodeFilePath = Path.Combine(barcodeFolderPath, barcodeFileName);
            barcodeBitmap.Save(barcodeFilePath);

        }
        static void FindAndReplace(Application wordApp, string findText, string replaceText)
        {
            Document doc = wordApp.ActiveDocument;
            Range range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = findText;
            range.Find.Forward = true;
            // Find each occurrence of the placeholder and replace manually
            while (range.Find.Execute())
            {
                range.Text = replaceText;
                range.Start = range.End;
                range.End = doc.Content.End;
            }
        }
        /* for QR Code */
        private void InsertImagesIntoDocument(Application wordApp, Document doc, string placeholder, List<string> imagePaths)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imagePaths != null && imagePaths.Count > 0)
                {
                    // Determine the number of rows and columns needed
                    int imagesPerRow = 1;
                    int totalImages = 1;
                    int rows = 1;
                    // Create a table with rows and columns
                    Table table = doc.Tables.Add(range, rows, 1);
                    // Populate the table with images
                    int imageIndex = 0;
                    for (int row = 1; row <= rows; row++)
                    {
                        for (int col = 1; col <= imagesPerRow; col++)
                        {
                            if (imageIndex < totalImages)
                            {
                                string imagePath = imagePaths[imageIndex];
                                if (System.IO.File.Exists(imagePath))
                                {
                                    Cell cell = table.Cell(row, col);
                                    cell.Range.InlineShapes.AddPicture(imagePath);
                                    // Resize the image as needed
                                    InlineShape shape = cell.Range.InlineShapes[1];
                                    //shape.Width = 100; // Set desired width ()
                                    //shape.Height = 100; // Maintain aspect ratio
                                    imageIndex++;
                                }
                            }
                        }
                    }
                    // Move the range to the end of the table to continue processing if needed
                    range.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }

        }
        private void InsertImagesIntoDocument1(Application wordApp, Document doc, string placeholder, List<Tuple<string, string>> imageData)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imageData != null && imageData.Count > 0)
                {
                    int imagesPerRow = 1;
                    int totalImages = imageData.Count;
                    int rows = (int)Math.Ceiling((double)totalImages / imagesPerRow);

                    Table table = doc.Tables.Add(range, rows, imagesPerRow);
                    table.Borders.Enable = 1;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.PreferredWidth = 100;

                    // Apply cell formatting
                    foreach (Cell cell in table.Range.Cells)
                    {
                        cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        cell.TopPadding = 10;
                        cell.BottomPadding = 10;
                        cell.LeftPadding = 10;
                        cell.RightPadding = 10;
                    }

                    int imageIndex = 0;
                    for (int row = 1; row <= rows; row++)
                    {
                        for (int col = 1; col <= imagesPerRow; col++)
                        {
                            if (imageIndex < totalImages)
                            {
                                string imagePath = imageData[imageIndex].Item1;
                                string imageName = imageData[imageIndex].Item2;
                                Cell cell = table.Cell(row, col);

                                cell.Range.Text = "";

                                if (File.Exists(imagePath))
                                {
                                    try
                                    {
                                        InlineShape shape = cell.Range.InlineShapes.AddPicture(
                                            FileName: imagePath,
                                            LinkToFile: false,
                                            SaveWithDocument: true
                                        );

                                        // Set image size
                                        shape.Width = 150;
                                        shape.Height = 150;

                                        // Add label
                                        cell.Range.InsertParagraphAfter();
                                        cell.Range.InsertAfter(imageName);

                                        // Formatting
                                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                        cell.Range.Font.Size = 16;
                                        cell.Range.Font.Name = "Arial";
                                    }
                                    catch (Exception ex)
                                    {
                                        cell.Range.Text = $"Error loading: {imageName}\n{ex.Message}";
                                    }
                                }
                                else
                                {
                                    cell.Range.Text = $"Missing: {imageName}";
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }

                                imageIndex++;
                            }
                            else
                            {
                                // Empty cell
                                table.Cell(row, col).Range.Text = "";
                            }
                        }
                    }

                    // Merge last row if only 1 image
                    if (totalImages % imagesPerRow == 1)
                    {
                        int lastRow = rows;
                        table.Cell(lastRow, 1).Merge(table.Cell(lastRow, imagesPerRow));
                        table.Cell(lastRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }
        /* placeholder replace for header and footer all pages |Start| */
        private void ReplaceInHeaderFooter(Application wordApp, string placeholder, string replacement)
        {
            // Replace in headers
            foreach (Section section in wordApp.ActiveDocument.Sections)
            {
                foreach (HeaderFooter headerFooter in section.Headers)
                {
                    FindAndReplaceInRange(headerFooter.Range, placeholder, replacement);
                }

                foreach (HeaderFooter headerFooter in section.Footers)
                {
                    FindAndReplaceInRange(headerFooter.Range, placeholder, replacement);
                }
            }
        }
        private void FindAndReplaceInRange(Range range, string placeholder, string replacement)
        {
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            find.Replacement.ClearFormatting();
            find.Replacement.Text = replacement;
            find.Execute(Replace: WdReplace.wdReplaceAll);
        }
        /* placeholder replace for header and footer all pages |End | */
        private int lastUsedNumber = 0;
        private DataTable GetReportData(int sampleID, int reportID)
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
                cmd.Parameters.AddWithValue("@SampleID", sampleID);
                cmd.Parameters.AddWithValue("@ReportID", reportID);


                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }
        private DataTable GetReportDataForT2(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * FROM TestResultsLED trl
                             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID AND ClauseID = @ClauseID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleID);
                cmd.Parameters.AddWithValue("@ReportID", reportID);
                cmd.Parameters.AddWithValue("@ClauseID", 0);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }
        /*T1 Start */
        private void CreateReportFromExcel(Microsoft.Office.Interop.Word.Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, int sampleID, int reportID)
        {
            string excelPath = GetExcelPathFromDatabase(sampleID, reportID);

            if (string.IsNullOrEmpty(excelPath) || !File.Exists(excelPath))
            {
                throw new Exception($"Excel file not found for SampleID {sampleID}, ReportID {reportID}. Path tried: {excelPath}");
            }

            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;

            try
            {
                excelApp = new Excel.Application();
                excelApp.Visible = false;

                wb = excelApp.Workbooks.Open(excelPath);
                ws = wb.Sheets[1];

                // Find placeholder in Word
                Microsoft.Office.Interop.Word.Range range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (!range.Find.Execute())
                {
                    throw new Exception($"Placeholder '{placeholder}' not found in document.");
                }

                range.Text = ""; // clear placeholder text

                // Collect tables in Excel (starting from row 8)
                List<Excel.Range> tables = new List<Excel.Range>();
                HashSet<int> processedRows = new HashSet<int>();
                int lastRow = ws.UsedRange.Rows.Count;

                for (int row = 8; row <= lastRow; row++)
                {
                    if (processedRows.Contains(row)) continue;

                    Excel.Range cell = ws.Cells[row, 1];
                    if (!string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                    {
                        Excel.Range tableRange = cell.CurrentRegion;
                        tables.Add(tableRange);

                        for (int r = tableRange.Row; r < tableRange.Row + tableRange.Rows.Count; r++)
                        {
                            processedRows.Add(r);
                        }
                    }
                    Marshal.ReleaseComObject(cell);
                }

                // Paste each table into Word
                bool isFirstTable = true;
                foreach (Excel.Range tableRange in tables)
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
                    wordTable.Borders.Enable = 1;
                    wordTable.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    wordTable.Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    wordTable.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPercent;
                    wordTable.PreferredWidth = 100;
                    wordTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow);

                    isFirstTable = false;
                    Marshal.ReleaseComObject(tableRange);
                }
            }
            finally
            {
                if (wb != null) { wb.Close(false); Marshal.ReleaseComObject(wb); }
                if (ws != null) Marshal.ReleaseComObject(ws);
                if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        // Database query function - add this to  CM file or data access layer
        private string GetExcelPathFromDatabase(int sampleID, int reportID)
        {
            string excelPath = string.Empty;

            string query = @"
        SELECT Result
        FROM TestResultsPTQualis
        WHERE SampleID = @SampleID AND ReportID = @ReportID
        ORDER BY createdDate DESC";  // 

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleID);
                cmd.Parameters.AddWithValue("@ReportID", reportID);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string dbPath = result.ToString().Trim();

                    // Handle virtual path (e.g., "~/FilesUpload/...") → convert to full server path
                    if (dbPath.StartsWith("~"))
                    {
                        excelPath = Server.MapPath(dbPath);
                    }
                    else if (!Path.IsPathRooted(dbPath))
                    {
                        // if path is relative but not prefixed with "~"
                        excelPath = Path.Combine(Server.MapPath("~/"), dbPath);
                    }
                    else
                    {
                        // already a full path
                        excelPath = dbPath;
                    }
                }
            }

            return excelPath;
        }
        /*T1 End */
        /* T2 Start */
        private void CreateReportTableT2(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // COMPLETELY REMOVE SPACING - Use exact line spacing
            range.Text = "LAMPIRAN A – DAFTAR KOMPONEN LUMINER" + (char)11 + "APPENDIX A – LIST OF LUMINARY COMPONENTS";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            // Remove all spacing
            range.ParagraphFormat.SpaceBefore = 0;
            range.ParagraphFormat.SpaceAfter = 0;
            range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
            range.ParagraphFormat.LineSpacing = 14;

            // Create table immediately after heading with NO extra space
            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // NO extra paragraph after - create table directly
            // tableRange.InsertParagraphAfter(); // REMOVE THIS LINE

            // Define the mapping between ReportParamMapID and column positions
            Dictionary<int, int> columnMapping = new Dictionary<int, int>
            {
                { 17813, 0 }, // Nama Objek
                { 17814, 1 }, // Pabrikan
                { 17815, 2 }, // Tipe / Model
                { 17816, 3 }, // Data Teknis
                { 17817, 4 }, // Standard
                { 17818, 5 }  // Tanda Kesesuaian
            };

            // Parse data based on ReportParamMapID
            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 6; i++)
            {
                parsedColumns.Add(new string[0]);
            }

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

            // Determine number of rows based on the maximum length of any column
            int numRows = parsedColumns.Max(col => col.Length);
            int numCols = 6;

            // Create table using tableRange instead of range
            Table table = doc.Tables.Add(tableRange, numRows + 1, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.5f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            // Adjusted column widths
            float[] colWidths = { 1.2f, 1.0f, 1.0f, 1.0f, 0.8f, 1.5f };
            for (int i = 0; i < numCols; i++)
            {
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
            }

            // Set header row with proper two-line formatting
            string[] headers = {
                                "Nama Objek\r\nObject Name",
                                "Pabrikan\r\nManufacturer",
                                "Tipe/Model\r\nType/Model",
                                "Data Teknis\r\nTechnical Data",
                                "Standard",
                                "Tanda Kesesuaian\r\nConformity Mark"
                                };

            // Set the header row height to accommodate two lines
            table.Rows[1].Height = wordApp.InchesToPoints(0.4f); // Reduced height
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10; // Smaller font for header
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Remove all spacing in header cells
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 10;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Fill data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 2].Height = wordApp.InchesToPoints(0.25f); // Reduced data row height
                table.Rows[rowIndex + 2].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 2, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10; // Slightly smaller font for data
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                    // Remove spacing in data cells
                    cell.Range.ParagraphFormat.SpaceBefore = 0;
                    cell.Range.ParagraphFormat.SpaceAfter = 0;
                }
            }

            // Remove table spacing
            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /* T2 Start */
        /* T3 START */
        private void CreateReportTableT3(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc.Content;
                Find find = range.Find;
                find.ClearFormatting();
                find.Text = placeholder;

                if (!find.Execute())
                    throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

                // COMPLETELY REMOVE SPACING - Use exact line spacing
                range.Text = "LAMPIRAN B – BAHAYA FOTOBIOLOGIS" + (char)11 + "APPENDIX B – PHOTOBIOLOGICAL HAZARDS";
                range.Font.Size = 12;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                // Remove all spacing
                range.ParagraphFormat.SpaceBefore = 0;
                range.ParagraphFormat.SpaceAfter = 0;
                range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                range.ParagraphFormat.LineSpacing = 14;

                // Create table immediately after heading with NO extra space
                Range tableRange = doc.Range(range.End, range.End);
                tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                // Parse data from database based on ParameterID
                Dictionary<int, string> measurementData = new Dictionary<int, string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["ReportParamMapID"] != DBNull.Value && row["Result"] != DBNull.Value)
                    {
                        int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                        string result = row["Result"].ToString();
                        measurementData[reportParamMapID] = result;
                    }
                }

                // Create main table for photobiological hazards -
                Table mainTable = doc.Tables.Add(tableRange, 15, 6);
                mainTable.Borders.Enable = 1;
                mainTable.AllowAutoFit = false;

                float totalWidthInches = 7.0f;
                mainTable.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

                // Adjusted column widths for the new structure
                float[] colWidths = { 1.0f, 0.7f, 0.8f, 1.0f, 1.0f, 1.5f };
                for (int i = 0; i < 6; i++)
                {
                    mainTable.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
                }

                // NEW HEADER STYLE - Single row with line breaks
                string[] mainHeaders = {
                    "Risk\r\nBahaya Optik",
                    "Symbol\r\nSimbol",
                    "Unit\r\nSatuan",
                    "Measurement Result\r\nHasil Pengukuran",
                    "Limit\r\nBatas",
                    "Remark\r\nKeterangan"
                };

                for (int col = 1; col <= 6; col++)
                {
                    SafeSetCellTextT3(mainTable, 1, col, mainHeaders[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    // Set gray background for header
                    SafeSetCellShadingT3(mainTable, 1, col, WdColor.wdColorGray25);
                }

                // Set row height to accommodate two lines
                mainTable.Rows[1].Height = wordApp.InchesToPoints(0.4f);
                mainTable.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // Actinic UV Section - ADJUSTED ROW NUMBERS (starting from row 2 now)
                SafeSetCellTextT3(mainTable, 2, 1, "Actinic UV");
                SafeSetCellTextT3(mainTable, 2, 2, "E_s");
                SafeSetCellTextT3(mainTable, 2, 3, "W.m⁻²");

                // Merge cells for Actinic UV section
                SafeMergeCellsT3(mainTable, 2, 1, 4, 1); // Risk merged
                SafeMergeCellsT3(mainTable, 2, 2, 4, 2); // Symbol merged  
                SafeMergeCellsT3(mainTable, 2, 3, 4, 3); // Unit merged
                SafeMergeCellsT3(mainTable, 2, 6, 4, 6); // Remark merged

                // Actinic UV measurement results and limits
                SafeSetCellTextT3(mainTable, 2, 4, GetMeasurementResultT3(17819, measurementData));
                SafeSetCellTextT3(mainTable, 2, 5, "0,001\r\n(Exempt/RG0)");

                SafeSetCellTextT3(mainTable, 3, 4, GetMeasurementResultT3(17820, measurementData));
                SafeSetCellTextT3(mainTable, 3, 5, "0,003\r\n(Low Risk/RG1)");

                SafeSetCellTextT3(mainTable, 4, 4, GetMeasurementResultT3(17821, measurementData));
                SafeSetCellTextT3(mainTable, 4, 5, "0,03\r\n(Mod Risk/RG2)");

                // Remark for Actinic UV
                SafeSetCellTextT3(mainTable, 2, 6, "Risk group classification:\r\nRG0 (Exempt) /\r\nRG1 (Low Risk) /\r\nRG2 (Mod Risk) /\r\nRG3 (High Risk) /\r\nTest Result:\r\n(Pass / Fail)");

                // Near UV Section - ADJUSTED ROW NUMBERS
                SafeSetCellTextT3(mainTable, 5, 1, "Near UV");
                SafeSetCellTextT3(mainTable, 5, 2, "E_UVA");
                SafeSetCellTextT3(mainTable, 5, 3, "W.m⁻²");

                // Merge cells for Near UV section
                SafeMergeCellsT3(mainTable, 5, 1, 7, 1); // Risk merged
                SafeMergeCellsT3(mainTable, 5, 2, 7, 2); // Symbol merged  
                SafeMergeCellsT3(mainTable, 5, 3, 7, 3); // Unit merged
                SafeMergeCellsT3(mainTable, 5, 6, 7, 6); // Remark merged

                // Near UV measurement results and limits
                SafeSetCellTextT3(mainTable, 5, 4, GetMeasurementResultT3(17822, measurementData));
                SafeSetCellTextT3(mainTable, 5, 5, "10\r\n(Exempt/RG0)");

                SafeSetCellTextT3(mainTable, 6, 4, GetMeasurementResultT3(17823, measurementData));
                SafeSetCellTextT3(mainTable, 6, 5, "33\r\n(Low Risk/RG1)");

                SafeSetCellTextT3(mainTable, 7, 4, GetMeasurementResultT3(17824, measurementData));
                SafeSetCellTextT3(mainTable, 7, 5, "100\r\n(Mod Risk/RG2)");

                // Remark for Near UV
                SafeSetCellTextT3(mainTable, 5, 6, "Risk group classification:\r\nRG0 (Exempt) /\r\nRG1 (Low Risk) /\r\nRG2 (Mod Risk) /\r\nRG3 (High Risk) /\r\nTest Result:\r\n(Pass / Fail)");

                // Blue Light Section - ADJUSTED ROW NUMBERS
                SafeSetCellTextT3(mainTable, 8, 1, "Blue light");
                SafeSetCellTextT3(mainTable, 8, 2, "L_B");
                SafeSetCellTextT3(mainTable, 8, 3, "W.m⁻².sr⁻¹");

                // Merge cells for Blue Light section
                SafeMergeCellsT3(mainTable, 8, 1, 10, 1); // Risk merged
                SafeMergeCellsT3(mainTable, 8, 2, 10, 2); // Symbol merged  
                SafeMergeCellsT3(mainTable, 8, 3, 10, 3); // Unit merged
                SafeMergeCellsT3(mainTable, 8, 6, 10, 6); // Remark merged

                // Blue Light measurement results and limits
                SafeSetCellTextT3(mainTable, 8, 4, GetMeasurementResultT3(17825, measurementData));
                SafeSetCellTextT3(mainTable, 8, 5, "100\r\n(Exempt/RG0)");

                SafeSetCellTextT3(mainTable, 9, 4, GetMeasurementResultT3(17826, measurementData));
                SafeSetCellTextT3(mainTable, 9, 5, "10000\r\n(Low Risk/RG1)");

                SafeSetCellTextT3(mainTable, 10, 4, GetMeasurementResultT3(17827, measurementData));
                SafeSetCellTextT3(mainTable, 10, 5, "4000000\r\n(Mod Risk/RG2)");

                // Remark for Blue Light
                SafeSetCellTextT3(mainTable, 8, 6, "Risk group classification:\r\nRG0 (Exempt) /\r\nRG1 (Low Risk) /\r\nRG2 (Mod Risk) /\r\nRG3 (High Risk) /\r\nTest Result:\r\n(Pass / Fail)");

                // Notes section - ADJUSTED ROW NUMBERS
                SafeMergeCellsT3(mainTable, 11, 1, 11, 6);
                SafeSetCellTextT3(mainTable, 11, 1, "Note:", true, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 11, 1, true);

                SafeMergeCellsT3(mainTable, 12, 1, 12, 6);
                SafeSetCellTextT3(mainTable, 12, 1, "RG: Risk Group", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 12, 1, true);

                SafeMergeCellsT3(mainTable, 13, 1, 13, 6);
                SafeSetCellTextT3(mainTable, 13, 1, "RG0 (Exempt group): The lamp does not pose any photobiological hazard.", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 13, 1, true);

                SafeMergeCellsT3(mainTable, 14, 1, 14, 6);
                SafeSetCellTextT3(mainTable, 14, 1, "RG1 (Low Risk): The lamp does not pose a hazard due to normal behavioral limitations on exposure.", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 14, 1, true);

                SafeMergeCellsT3(mainTable, 15, 1, 15, 6);
                SafeSetCellTextT3(mainTable, 15, 1, "RG2 (Moderate Risk): The lamp does not pose a hazard due to the aversion response to very bright light sources or due to thermal discomfort.\r\nRG3 (High Risk): The lamp may pose a hazard even for momentary or brief exposure.\r\nThe lamps which exceed the limits for RG2 (Moderate Risk) are in RG3 (High Risk).", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 15, 1, true);

                // Apply formatting to all cells
                for (int row = 1; row <= 15; row++)
                {
                    for (int col = 1; col <= 6; col++)
                    {
                        if (CellExistsT3(mainTable, row, col))
                        {
                            Cell cell = mainTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Remove spacing
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;

                            // Set white background for data rows (rows 2-15)
                            if (row >= 2)
                            {
                                SafeSetCellShadingT3(mainTable, row, col, WdColor.wdColorWhite);
                            }
                        }
                    }
                }

                // Center align specific columns
                for (int row = 2; row <= 10; row++)
                {
                    for (int col = 2; col <= 5; col++) // Symbol, Unit, Measurement, Limit columns
                    {
                        if (CellExistsT3(mainTable, row, col))
                        {
                            mainTable.Cell(row, col).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                    }
                }

                // Remove table spacing
                mainTable.Range.ParagraphFormat.SpaceAfter = 0;
                mainTable.Range.ParagraphFormat.SpaceBefore = 0;

                // Add space between tables
                Range betweenTablesRange = doc.Range(mainTable.Range.End, mainTable.Range.End);
                betweenTablesRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                betweenTablesRange.InsertParagraphAfter();
                betweenTablesRange.InsertParagraphAfter();

                // Create second table for Blue light hazard radiance and Luminance - REDUCED to 4 rows
                Range secondTableRange = doc.Range(betweenTablesRange.End, betweenTablesRange.End);
                secondTableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                Table secondTable = doc.Tables.Add(secondTableRange, 4, 4);
                secondTable.Borders.Enable = 1;
                secondTable.AllowAutoFit = false;
                secondTable.PreferredWidth = wordApp.InchesToPoints(6.0f);

                // Column widths for second table
                float[] secondColWidths = { 1.8f, 0.8f, 0.8f, 2.6f };
                for (int i = 0; i < 4; i++)
                {
                    secondTable.Columns[i + 1].Width = wordApp.InchesToPoints(secondColWidths[i]);
                }

                // NEW SECOND TABLE HEADER STYLE - Single row with line breaks
                string[] secondHeaders = {
                    "Item Test\r\nItem uji",
                    "Symbol\r\nSimbol",
                    "Unit\r\nSatuan",
                    "Results\r\nHasil uji"
                };

                for (int col = 1; col <= 4; col++)
                {
                    SafeSetCellTextT3(secondTable, 1, col, secondHeaders[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    // Set gray background for header
                    SafeSetCellShadingT3(secondTable, 1, col, WdColor.wdColorGray25);
                }

                // Set row height to accommodate two lines
                secondTable.Rows[1].Height = wordApp.InchesToPoints(0.4f);
                secondTable.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // Second table data with DYNAMIC Results - ADJUSTED ROW NUMBERS (starting from row 2 now)
                string blueLightResult = GetMeasurementResultT3(17828, measurementData);
                string luminanceResult = GetMeasurementResultT3(17829, measurementData);

                // Blue light hazard radiance row (now row 2)
                SafeSetCellTextT3(secondTable, 2, 1, "Blue light hazard radiance");
                SafeSetCellTextT3(secondTable, 2, 2, "L_B");
                SafeSetCellTextT3(secondTable, 2, 3, "W/m².sr");
                SafeSetCellTextT3(secondTable, 2, 4, string.IsNullOrEmpty(blueLightResult) ? "LB (11 mrad)\r\npada hasil uji lab IEC 62788" : blueLightResult);

                // Luminance row (now row 3)  
                SafeSetCellTextT3(secondTable, 3, 1, "Luminance");
                SafeSetCellTextT3(secondTable, 3, 2, "L_v");
                SafeSetCellTextT3(secondTable, 3, 3, "cd/m²");
                SafeSetCellTextT3(secondTable, 3, 4, string.IsNullOrEmpty(luminanceResult) ? "L (11 mrad)\r\npada hasil uji lab IEC 62788" : luminanceResult);

                // Apply formatting to second table
                for (int row = 1; row <= 4; row++)
                {
                    for (int col = 1; col <= 4; col++)
                    {
                        if (CellExistsT3(secondTable, row, col))
                        {
                            Cell cell = secondTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Remove spacing
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;

                            // Set white background for data rows (rows 2-4)
                            if (row >= 2)
                            {
                                SafeSetCellShadingT3(secondTable, row, col, WdColor.wdColorWhite);
                            }
                        }
                    }
                }

                // Center align symbol and unit columns
                for (int row = 2; row <= 3; row++)
                {
                    for (int col = 2; col <= 3; col++)
                    {
                        if (CellExistsT3(secondTable, row, col))
                        {
                            secondTable.Cell(row, col).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                    }
                }

                // Remove spacing from second table
                secondTable.Range.ParagraphFormat.SpaceAfter = 0;
                secondTable.Range.ParagraphFormat.SpaceBefore = 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating photobiological hazards table: {ex.Message}", ex);
            }
        }
        private void SafeSetCellShadingT3(Table table, int row, int col, WdColor color)
        {
            if (CellExistsT3(table, row, col))
            {
                table.Cell(row, col).Shading.BackgroundPatternColor = color;
            }
        }
        private bool CellExistsT3(Table table, int row, int column)
        {
            try
            {
                var cell = table.Cell(row, column);
                return cell != null;
            }
            catch
            {
                return false;
            }
        }
        private void SafeMergeCellsT3(Table table, int startRow, int startCol, int endRow, int endCol)
        {
            if (CellExistsT3(table, startRow, startCol) && CellExistsT3(table, endRow, endCol))
            {
                table.Cell(startRow, startCol).Merge(table.Cell(endRow, endCol));
            }
        }
        private void SafeSetCellTextT3(Table table, int row, int col, string text, bool bold = false, WdParagraphAlignment alignment = WdParagraphAlignment.wdAlignParagraphLeft)
        {
            if (CellExistsT3(table, row, col))
            {
                Cell cell = table.Cell(row, col);
                cell.Range.Text = text;
                cell.Range.Font.Bold = bold ? 1 : 0;
                cell.Range.ParagraphFormat.Alignment = alignment;
            }
        }
        private void SafeSetCellItalicT3(Table table, int row, int col, bool italic)
        {
            if (CellExistsT3(table, row, col))
            {
                table.Cell(row, col).Range.Font.Italic = italic ? 1 : 0;
            }
        }
        private string GetMeasurementResultT3(int reportParamMapID, Dictionary<int, string> measurementData)
        {
            if (measurementData.ContainsKey(reportParamMapID))
            {
                string result = measurementData[reportParamMapID];
                System.Diagnostics.Debug.WriteLine($"Retrieving data for {reportParamMapID}: {result}");
                return result;
            }
            System.Diagnostics.Debug.WriteLine($"No data found for {reportParamMapID}");
            return string.Empty;
        }

        private void CreateReportTableT4(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc.Content;
                Find find = range.Find;
                find.ClearFormatting();
                find.Text = placeholder;

                if (!find.Execute())
                    throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

                // COMPLETELY REMOVE SPACING - Use exact line spacing
                range.Text = "LAMPIRAN D – UJI TERMAL" + (char)11 + "APPENDIX D – THERMAL TESTING";
                range.Font.Size = 12;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                // Remove all spacing
                range.ParagraphFormat.SpaceBefore = 0;
                range.ParagraphFormat.SpaceAfter = 0;
                range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                range.ParagraphFormat.LineSpacing = 14;

                // Create table immediately after placeholder
                Range tableRange = doc.Range(range.End, range.End);
                tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                // Parse data from database based on ParameterID
                Dictionary<int, string[]> measurementData = new Dictionary<int, string[]>();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["ReportParamMapID"] != DBNull.Value && row["Result"] != DBNull.Value)
                    {
                        int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                        string result = row["Result"].ToString();
                        string[] values = result.Split('|');
                        measurementData[reportParamMapID] = values;
                    }
                }

                // Create first table (Klausul table) - 11 rows, 4 columns
                Table firstTable = doc.Tables.Add(tableRange, 11, 4);
                firstTable.Borders.Enable = 1;
                firstTable.AllowAutoFit = false;

                float firstTableWidthInches = 6.5f;
                firstTable.PreferredWidth = wordApp.InchesToPoints(firstTableWidthInches);

                // Column widths for first table
                float[] firstColWidths = { 0.6f, 3.5f, 1.2f, 1.2f };
                for (int i = 0; i < 4; i++)
                {
                    firstTable.Columns[i + 1].Width = wordApp.InchesToPoints(firstColWidths[i]);
                }

                // First table header - multi-line
                string[] firstHeaders = {
                    "Klausul\r\nclause",
                    "Persyaratan\r\nRequirement",
                    "Hasil\r\nResult",
                    "*Keterangan\r\nRemark*"
                };

                for (int col = 1; col <= 4; col++)
                {
                    SafeSetCellTextT4(firstTable, 1, col, firstHeaders[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    SafeSetCellShadingT4(firstTable, 1, col, WdColor.wdColorGray25);
                }

                // Set row height for header
                firstTable.Rows[1].Height = wordApp.InchesToPoints(0.4f);
                firstTable.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // First table data
                string[,] firstTableData = {
                    { "1.", "Tipe produk.", GetMeasurementValue(17830, 0, measurementData), GetMeasurementValue(17830, 1, measurementData) },
                    { "2.", "Lampu yang digunakan.", GetMeasurementValue(17831, 0, measurementData), GetMeasurementValue(17831, 1, measurementData) },
                    { "3.", "Tipe LED driver yang digunakan.", GetMeasurementValue(17832, 0, measurementData), GetMeasurementValue(17832, 1, measurementData) },
                    { "4.", "Posisi pemasangan saat pengujian.", GetMeasurementValue(17833, 0, measurementData), GetMeasurementValue(17833, 1, measurementData) },
                    { "5.", "Daya (W).", GetMeasurementValue(17834, 0, measurementData), GetMeasurementValue(17834, 1, measurementData) },
                    { "6.", "Arus (A).", GetMeasurementValue(17835, 0, measurementData), GetMeasurementValue(17835, 1, measurementData) },
                    { "7.", "Uji 1:\r\nTegangan pengenal (V).", GetMeasurementValue(17836, 0, measurementData), GetMeasurementValue(17836, 1, measurementData) },
                    { "8.", "Uji 2:\r\n1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17837, 0, measurementData), GetMeasurementValue(17837, 1, measurementData) },
                    { "9.", "Uji 3:\r\nBakan dengan dilengkapi kotak kontak 1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17838, 0, measurementData), GetMeasurementValue(17838, 1, measurementData) },
                    { "10.", "Uji 4:\r\n1,1 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17839, 0, measurementData), GetMeasurementValue(17839, 1, measurementData) }
                };

                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        SafeSetCellTextT4(firstTable, row + 2, col + 1, firstTableData[row, col], false,
                            col == 0 || col == 2 ? WdParagraphAlignment.wdAlignParagraphCenter : WdParagraphAlignment.wdAlignParagraphLeft);
                        SafeSetCellShadingT4(firstTable, row + 2, col + 1, WdColor.wdColorWhite);
                    }
                }

                // Apply formatting to first table
                for (int row = 1; row <= 11; row++)
                {
                    for (int col = 1; col <= 4; col++)
                    {
                        if (CellExistsT4(firstTable, row, col))
                        {
                            Cell cell = firstTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;
                        }
                    }
                }

                firstTable.Range.ParagraphFormat.SpaceAfter = 0;
                firstTable.Range.ParagraphFormat.SpaceBefore = 0;

                // Add space between tables
                Range betweenTablesRange = doc.Range(firstTable.Range.End, firstTable.Range.End);
                betweenTablesRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                betweenTablesRange.InsertParagraphAfter();
                betweenTablesRange.InsertParagraphAfter();

                // Create second table - 9 rows (2 header rows + 7 data rows), 7 columns
                Range secondTableRange = doc.Range(betweenTablesRange.End, betweenTablesRange.End);
                secondTableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                Table secondTable = doc.Tables.Add(secondTableRange, 9, 7);
                secondTable.Borders.Enable = 1;
                secondTable.AllowAutoFit = false;

                float secondTableWidthInches = 7.0f;
                secondTable.PreferredWidth = wordApp.InchesToPoints(secondTableWidthInches);

                // Column widths for second table
                float[] secondColWidths = { 1.5f, 0.8f, 0.8f, 0.8f, 1.0f, 0.8f, 1.0f };
                for (int i = 0; i < 7; i++)
                {
                    secondTable.Columns[i + 1].Width = wordApp.InchesToPoints(secondColWidths[i]);
                }

                // HEADER ROW 1 - Set all text and formatting BEFORE merging
                // Cell[1,1] - "Bagian yang diuji" (will span rows 1-2)
                SafeSetCellTextT4(secondTable, 1, 1, "Bagian yang diuji", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 1, WdColor.wdColorGray25);

                // Cell[1,2] - "Klausul 12.4..." (will span columns 2-5)
                SafeSetCellTextT4(secondTable, 1, 2, "Klausul 12.4 – Uji termal\r\n(Operasi normal)", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 2, WdColor.wdColorGray25);

                // Cells [1,3], [1,4], [1,5] - Empty (will be merged into cell [1,2])
                SafeSetCellTextT4(secondTable, 1, 3, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 3, WdColor.wdColorGray25);
                SafeSetCellTextT4(secondTable, 1, 4, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 4, WdColor.wdColorGray25);
                SafeSetCellTextT4(secondTable, 1, 5, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 5, WdColor.wdColorGray25);

                // Cell[1,6] - "Klausul 12.5..." (will span columns 6-7)
                SafeSetCellTextT4(secondTable, 1, 6, "Klausul 12.5 – Uji termal\r\n(Operasi abnormal)", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 6, WdColor.wdColorGray25);

                // Cell[1,7] - Empty (will be merged into cell [1,6])
                SafeSetCellTextT4(secondTable, 1, 7, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 7, WdColor.wdColorGray25);

                // HEADER ROW 2
                // Cell[2,1] - Empty (will be merged with [1,1])
                SafeSetCellTextT4(secondTable, 2, 1, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 2, 1, WdColor.wdColorGray25);

                // Cells[2,2] through [2,7] - Column headers
                string[] row2Headers = { "", "Uji 1", "Uji 2", "Uji 3", "Limit Maks.", "Uji 4", "Limit Maks." };
                for (int col = 2; col <= 7; col++)
                {
                    SafeSetCellTextT4(secondTable, 2, col, row2Headers[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    SafeSetCellShadingT4(secondTable, 2, col, WdColor.wdColorGray25);
                }

                // NOW PERFORM THE MERGES
                // Merge "Bagian yang diuji" - rowspan=2
                SafeMergeCellsT4(secondTable, 1, 1, 2, 1);

                // Merge "Klausul 12.4..." - colspan=4 (columns 2-5)
                SafeMergeCellsT4(secondTable, 1, 2, 1, 5);

                // Merge "Klausul 12.5..." - colspan=2 (columns 6-7)
                SafeMergeCellsT4(secondTable, 1, 6, 1, 7);

                // Data rows (rows 3-9)
                string[,] secondTableData = {
                    { "Suhu ruang uji.",
                      GetMeasurementValue(17840, 0, measurementData), GetMeasurementValue(17840, 1, measurementData), GetMeasurementValue(17840, 2, measurementData),
                      GetMeasurementValue(17840, 3, measurementData), GetMeasurementValue(17840, 4, measurementData), GetMeasurementValue(17840, 5, measurementData) },
                    { "Permukaan luar luminer",
                      GetMeasurementValue(17841, 0, measurementData), GetMeasurementValue(17841, 1, measurementData), GetMeasurementValue(17841, 2, measurementData),
                      GetMeasurementValue(17841, 3, measurementData), GetMeasurementValue(17841, 4, measurementData), GetMeasurementValue(17841, 5, measurementData) },
                    { "Permukaan LED Driver.",
                      GetMeasurementValue(17842, 0, measurementData), GetMeasurementValue(17842, 1, measurementData), GetMeasurementValue(17842, 2, measurementData),
                      GetMeasurementValue(17842, 3, measurementData), GetMeasurementValue(17842, 4, measurementData), GetMeasurementValue(17842, 5, measurementData) },
                    { "LED module / PCB.",
                      GetMeasurementValue(17843, 0, measurementData), GetMeasurementValue(17843, 1, measurementData), GetMeasurementValue(17843, 2, measurementData),
                      GetMeasurementValue(17843, 3, measurementData), GetMeasurementValue(17843, 4, measurementData), GetMeasurementValue(17843, 5, measurementData) },
                    { "Selungkup LED module\r\n(Plastik)",
                      GetMeasurementValue(17844, 0, measurementData), GetMeasurementValue(17844, 1, measurementData), GetMeasurementValue(17844, 2, measurementData),
                      GetMeasurementValue(17844, 3, measurementData), GetMeasurementValue(17844, 4, measurementData), GetMeasurementValue(17844, 5, measurementData) },
                    { "Kabel eksternal.",
                      GetMeasurementValue(17845, 0, measurementData), GetMeasurementValue(17845, 1, measurementData), GetMeasurementValue(17845, 2, measurementData),
                      GetMeasurementValue(17845, 3, measurementData), GetMeasurementValue(17845, 4, measurementData), GetMeasurementValue(17845, 5, measurementData) },
                    { "Kabel internal.",
                      GetMeasurementValue(17846, 0, measurementData), GetMeasurementValue(17846, 1, measurementData), GetMeasurementValue(17846, 2, measurementData),
                      GetMeasurementValue(17846, 3, measurementData), GetMeasurementValue(17846, 4, measurementData), GetMeasurementValue(17846, 5, measurementData) }
                };

                for (int row = 0; row < 7; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        SafeSetCellTextT4(secondTable, row + 3, col + 1, secondTableData[row, col], false,
                            col == 0 ? WdParagraphAlignment.wdAlignParagraphLeft : WdParagraphAlignment.wdAlignParagraphCenter);
                        SafeSetCellShadingT4(secondTable, row + 3, col + 1, WdColor.wdColorWhite);
                    }
                }

                // Apply formatting to entire second table
                for (int row = 1; row <= 9; row++)
                {
                    for (int col = 1; col <= 7; col++)
                    {
                        if (CellExistsT4(secondTable, row, col))
                        {
                            Cell cell = secondTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;
                        }
                    }
                }

                secondTable.Range.ParagraphFormat.SpaceAfter = 0;
                secondTable.Range.ParagraphFormat.SpaceBefore = 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating table T4: {ex.Message}", ex);
            }
        }
        private string GetMeasurementValue(int reportParamMapID, int index, Dictionary<int, string[]> measurementData)
        {
            if (measurementData.ContainsKey(reportParamMapID))
            {
                string[] values = measurementData[reportParamMapID];
                if (index < values.Length)
                {
                    return values[index];
                }
            }
            return string.Empty;
        }
        private bool CellExistsT4(Table table, int row, int column)
        {
            try
            {
                var cell = table.Cell(row, column);
                return cell != null;
            }
            catch
            {
                return false;
            }
        }
        private void SafeMergeCellsT4(Table table, int startRow, int startCol, int endRow, int endCol)
        {
            if (CellExistsT4(table, startRow, startCol) && CellExistsT4(table, endRow, endCol))
            {
                table.Cell(startRow, startCol).Merge(table.Cell(endRow, endCol));
            }
        }
        private void SafeSetCellTextT4(Table table, int row, int col, string text, bool bold = false, WdParagraphAlignment alignment = WdParagraphAlignment.wdAlignParagraphLeft)
        {
            if (CellExistsT4(table, row, col))
            {
                Cell cell = table.Cell(row, col);
                cell.Range.Text = text;
                cell.Range.Font.Bold = bold ? 1 : 0;
                cell.Range.ParagraphFormat.Alignment = alignment;
            }
        }
        private void SafeSetCellShadingT4(Table table, int row, int col, WdColor color)
        {
            if (CellExistsT4(table, row, col))
            {
                table.Cell(row, col).Shading.BackgroundPatternColor = color;
            }
        }
        /*T4 END */
        /* T5 Start */
        private void CreateReportTableT5(Application wordApp, Document doc, string placeholder, List<string> equipmentNames)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // Simply remove the placeholder without adding any heading
            range.Text = "";

            // Create table immediately at the placeholder position
            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Create table with 2 columns only
            int numCols = 2;
            int numRows = equipmentNames.Count + 1; // +1 for header row
            Table table = doc.Tables.Add(tableRange, numRows, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.5f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            // Column widths
            float[] colWidths = { 0.6f, 5.9f };
            for (int i = 0; i < numCols; i++)
            {
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
            }

            // Set header row
            string[] headers = { "No.", "Nama Alat/Equipment name" };
            table.Rows[1].Height = wordApp.InchesToPoints(0.3f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 10;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Fill data rows - ALL equipment items
            for (int i = 0; i < equipmentNames.Count; i++)
            {
                int rowIndex = i + 2; // +2 because row 1 is header

                table.Rows[rowIndex].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // Number column
                Cell noCell = table.Cell(rowIndex, 1);
                noCell.Range.Text = (i + 1) + ".";
                noCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                noCell.Range.Font.Size = 10;
                noCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                noCell.Range.ParagraphFormat.SpaceBefore = 0;
                noCell.Range.ParagraphFormat.SpaceAfter = 0;
                noCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                // Equipment name column
                Cell nameCell = table.Cell(rowIndex, 2);
                nameCell.Range.Text = equipmentNames[i];
                nameCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                nameCell.Range.Font.Size = 11;
                nameCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                nameCell.Range.ParagraphFormat.SpaceBefore = 0;
                nameCell.Range.ParagraphFormat.SpaceAfter = 0;
                nameCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            }

            // Remove table spacing
            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        private void CreateReportTableT6(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            range.Text = "";

            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Create table with 5 columns
            int numCols = 5; // No, Clause Number, Clause Title, Standard, Result
            int numRows = dt.Rows.Count + 1; // +1 for header row

            Table table = doc.Tables.Add(tableRange, numRows, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.5f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            // Column widths - adjust as needed
            float[] colWidths = { 0.4f, 0.6f, 2.5f, 2.0f, 1.0f }; // No, ClauseNumber, ClauseTitle, Standard, Result
            for (int i = 0; i < numCols; i++)
            {
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
            }

            // Set header row
            string[] headers = { "#", "Klausul\r\nClause", "Deskripsi uji\r\nTest Description", "Standard / Metode uji\r\nStandard/Test Method", "*Hasil uji\r\nTest Result" };
            table.Rows[1].Height = wordApp.InchesToPoints(0.5f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 10;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Fill data rows from DataTable
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int rowIndex = i + 2; // +2 because row 1 is header
                DataRow row = dt.Rows[i];

                table.Rows[rowIndex].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex].HeightRule = WdRowHeightRule.wdRowHeightAtLeast;

                // Column 1: Number
                Cell noCell = table.Cell(rowIndex, 1);
                noCell.Range.Text = (i + 1).ToString();
                noCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                noCell.Range.Font.Size = 10;
                noCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                noCell.Range.ParagraphFormat.SpaceBefore = 0;
                noCell.Range.ParagraphFormat.SpaceAfter = 0;
                noCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                // Column 2: Clause Number
                Cell clauseNumCell = table.Cell(rowIndex, 2);
                clauseNumCell.Range.Text = row["ClauseNumber"].ToString();
                clauseNumCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                clauseNumCell.Range.Font.Size = 10;
                clauseNumCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                clauseNumCell.Range.ParagraphFormat.SpaceBefore = 0;
                clauseNumCell.Range.ParagraphFormat.SpaceAfter = 0;
                clauseNumCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                // Column 3: Clause Title
                Cell clauseTitleCell = table.Cell(rowIndex, 3);
                clauseTitleCell.Range.Text = row["ClauseTitle"].ToString();
                clauseTitleCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                clauseTitleCell.Range.Font.Size = 10;
                clauseTitleCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                clauseTitleCell.Range.ParagraphFormat.SpaceBefore = 0;
                clauseTitleCell.Range.ParagraphFormat.SpaceAfter = 0;
                clauseTitleCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                // Column 4: Standard Name
                // Column 4: Standard Name
                Cell standardCell = table.Cell(rowIndex, 4);
                string standardName = row["StandardName"].ToString();
                string[] standards = standardName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                standardCell.Range.Text = standards[0];
                for (int s = 1; s < standards.Length; s++)
                {
                    standardCell.Range.InsertAfter(", \r" + standards[s]);
                }

                standardCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                standardCell.Range.Font.Size = 10;
                standardCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                standardCell.Range.ParagraphFormat.SpaceBefore = 0;
                standardCell.Range.ParagraphFormat.SpaceAfter = 0;
                //standardCell.Shading.BackgroundPatternColor = WdColor.wdColorYellow;

                // Column 5: Result (L/TL/TB)
                Cell resultCell = table.Cell(rowIndex, 5);
                string result = row["Result"].ToString();
                resultCell.Range.Text = result;
                resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                resultCell.Range.Font.Size = 10;
                resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                resultCell.Range.ParagraphFormat.SpaceBefore = 0;
                resultCell.Range.ParagraphFormat.SpaceAfter = 0;

                // Apply yellow background for L/TL/TB results
                //if (result.Contains("L/TL/TB") || result.Contains("L/TL") || result.Contains("TL/TB"))
                //{
                //    resultCell.Shading.BackgroundPatternColor = WdColor.wdColorYellow;
                //}
                //else
                //{
                resultCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                //}
            }

            // Remove table spacing
            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        /* T6 End */
        private string GetInputTypeFromClause(DataRow row)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
                return "checkbox";
            else
                return "text";
        }

        private string FormatResultsForExport(string inputType, string resultOptions, string resultOptionsBH, string selectedResults)
        {
            if (inputType == "checkbox" && !string.IsNullOrEmpty(resultOptions))
            {
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : options;
                string[] selected = !string.IsNullOrEmpty(selectedResults) ? selectedResults.Split(',') : new string[0];
                StringBuilder sb = new StringBuilder();

                // Arrange checkboxes in pairs (2 per row)
                for (int i = 0; i < options.Length; i += 2)
                {
                    // First checkbox in the pair
                    string option1 = options[i].Trim();
                    string optionDisplay1 = i < optionsBH.Length ? optionsBH[i].Trim() : option1;
                    bool isSelected1 = selected.Any(s => s.Trim().Equals(option1, StringComparison.OrdinalIgnoreCase));
                    string checkbox1 = isSelected1 ? "☒" : "☐";
                    sb.Append($"{checkbox1} {optionDisplay1}");

                    // Second checkbox in the pair (if exists)
                    if (i + 1 < options.Length)
                    {
                        string option2 = options[i + 1].Trim();
                        string optionDisplay2 = (i + 1) < optionsBH.Length ? optionsBH[i + 1].Trim() : option2;
                        bool isSelected2 = selected.Any(s => s.Trim().Equals(option2, StringComparison.OrdinalIgnoreCase));
                        string checkbox2 = isSelected2 ? "☒" : "☐";

                        // Add spacing between the two checkboxes in the same row
                        sb.Append($"\t\t{checkbox2} {optionDisplay2}");
                    }

                    // Add line break after each pair (except for the last pair)
                    if (i + 2 < options.Length)
                    {
                        sb.Append("\r\n");
                    }
                }
                return sb.ToString();
            }
            return selectedResults ?? "";
        }


    }
}