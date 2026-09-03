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
    public partial class CreateReportFromTemplate_Battery : System.Web.UI.Page
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

                DataTable PVdetails = _cm.getPVdetails(sampleID);
                string templatePath = "";
                //DataTable dt1 = _cm.getReportQuery(c);
                //switch case for report id 
                templatePath = path + "PV_ReportTemplate_1.docx"; /* Static */



                doc = wordApp.Documents.Add(templatePath);
                string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                List<string> imagePaths = new List<string> { imageUrl };
                InsertImagesIntoDocument(wordApp, doc, "{QR_Code}", imagePaths);
                DataTable dt2 = new DataTable();

                if (PVdetails != null && PVdetails.Rows.Count > 0)
                {
                    DataRow row = PVdetails.Rows[0];

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


                    //FindAndReplace(wordApp, "{WorkOrederNumber}", GetSafeValue("WorkorderNumber"));
                    //FindAndReplace(wordApp, "{Samplereceivaldate}", GetSafeValue("SampleReceivedDate"));
                    //FindAndReplace(wordApp, "{TestMethod}", GetSafeValue("TestMethod"));
                    //FindAndReplace(wordApp, "{ReportNumber}", GetSafeValue("TestReportNo"));
                    //FindAndReplace(wordApp, "{CompanyName}", GetSafeValue("Applicant"));
                    //FindAndReplace(wordApp, "{CompanyAddress}", GetSafeValue("Address"));
                    //FindAndReplace(wordApp, "{ManufacturerName}", GetSafeValue("FactoryName"));
                    //FindAndReplace(wordApp, "{ManufacturerAddress}", GetSafeValue("FactoryAddress"));
                    //FindAndReplace(wordApp, "{LetterNumber}", GetSafeValue("LotNumber"));

                    //FindAndReplace(wordApp, "{ProductName}", GetSafeValue("ProductName"));
                    //FindAndReplace(wordApp, "{ProductBrand}", GetSafeValue("BrandProduct"));
                    //FindAndReplace(wordApp, "{ProductType}", GetSafeValue("MaterialType"));
                    //FindAndReplace(wordApp, "{ProductSpecification}", GetSafeValue("ProductSpecification"));
                    //FindAndReplace(wordApp, "{ProductColor}", GetSafeValue("Color"));
                    //FindAndReplace(wordApp, "{ProductSerialNumber}", GetSafeValue("ProductSerialNo"));
                    //FindAndReplace(wordApp, "{ProductQuantity}", GetSafeValue("ProductQuantity"));

                    //FindAndReplace(wordApp, "{ProductReceptionDate}", GetSafeValue("IssueDate"));
                    //FindAndReplace(wordApp, "{DateOfTesting}", GetSafeValue("TestPeriod"));

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

                    DataTable dt3 = _cm.getPVdetailsForImages(ID);
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

                    DataTable dtCloneCRFSampleIDGet = _cm.CRFSampleDetails_PV(ID);


                    ////Example: Load dynamically from config, DB, or
                    //List<int> specialReportIDs = GetDynamicReportIDs(); // e.g., [3212, 2213, 5001]


                    //if (dtCloneCRFSampleIDGet != null && dtCloneCRFSampleIDGet.Rows.Count > 0)
                    //{
                    //    foreach (DataRow row in dtCloneCRFSampleIDGet.Rows)
                    //    {
                    //        int cloneCRFSampleID = Convert.ToInt32(row["CloneCRFSampleID"]);
                    //        int reportID = Convert.ToInt32(row["ReportID"]);

                    //        if (specialReportIDs.Contains(reportID))
                    //        {
                    //            //Do something dynamically for that report

                    //           Console.WriteLine($"Matched dynamic report ID: {reportID}");
                    //        }
                    //    }
                    //}


                    if (dtCloneCRFSampleIDGet != null && dtCloneCRFSampleIDGet.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtCloneCRFSampleIDGet.Rows)
                        {
                            cloneCRFSampleID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["CloneCRFSampleID"].ToString());
                            reportID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["ReportID"].ToString());

                            if (reportID == 7423)
                            {
                                b1 = true;
                            }

                            else if (reportID == 6432)
                            {
                                b1 = true;
                            }

                           else  if (reportID == 2213)
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
                        //   CreateReportFromExcel(wordApp, doc, "{TestingAndMeasurement}", dtReport);
                        CreateReportFromExcel(wordApp, doc, "{testrecord}", ID, reportID);



                        // DataTable dtReportT2 = GetReportDataForT2(ID, reportID);
                        //CreateReportTableT2(wordApp, doc, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}", dtReportT2);

                        //DataTable dtReportT3 = GetReportDataForT2(ID, reportID);
                        //CreateReportTableT3(wordApp, doc, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}", dtReportT3);

                        //DataTable dtReportT4 = GetReportDataForT2(ID, reportID);
                        //CreateReportTableT4(wordApp, doc, "{LAMPIRAN_D-UJI_TERMAL}", dtReportT4);

                        //List<string> equipmentNames = new List<string>();
                        //DataTable dtEquipment = _cm.GetEquipmentNamesRowWise(ID, reportID);
                        //if (dtEquipment != null && dtEquipment.Rows.Count > 0)
                        //{
                        //    equipmentNames = dtEquipment.AsEnumerable().Select(row => row["Equipment"].ToString()).ToList();
                        //}
                        //CreateReportTableT5(wordApp, doc, "{MeasuringEquipment}", equipmentNames);


                        //DataTable dtReportT6 = _cm.GetConclusionRow_LED(ID, reportID);
                        //CreateReportTableT6(wordApp, doc, "{Overall Conclusion}", dtReportT6);

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
                //foreach (var process in Process.GetProcessesByName("WINWORD"))
                //{
                //    if (string.IsNullOrEmpty(process.MainWindowTitle))
                //    {
                //        process.Kill();
                //    }
                //}
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
      

    }
}