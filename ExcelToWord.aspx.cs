using iTextSharp.text.pdf.parser;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.UI;
using ZXing;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;
using Table = Microsoft.Office.Interop.Word.Table;


namespace LIMS
{
    public partial class ExcelToWord : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = null;
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

                DataTable ChemicalDetails = _cm.getChemicalDetails(sampleID);
                string templatePath = "";
                DataTable dt1 = _cm.getReportQuery(c);
                templatePath = path + dt1.Rows[0]["Template"].ToString();
                doc = wordApp.Documents.Add(templatePath);
                string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                List<string> imagePaths = new List<string> { imageUrl };
                InsertImagesIntoDocument(wordApp, doc, "{QR_Code}", imagePaths);
                DataTable dt2 = new DataTable();

                if (ChemicalDetails != null && ChemicalDetails.Rows.Count > 0)
                {
                    DataRow row = ChemicalDetails.Rows[0];

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

                    FindAndReplace(wordApp, "{RequestNo.}", GetSafeValue("sampleIDSeries"));
                    FindAndReplace(wordApp, "{Type}", GetSafeValue("TypeDescription"));
                    FindAndReplace(wordApp, "{GroupofProduct}", GetSafeValue("GroupOfProduct"));
                    FindAndReplace(wordApp, "{ProductDescription}", GetSafeValue("DescriptionOfProduct"));
                    FindAndReplace(wordApp, "{Remark}", GetSafeValue("GetRemarks"));

                    FindAndReplace(wordApp, "{Testreportvaliduntil}", GetSafeDate1("DateOfExp"));
                    FindAndReplace(wordApp, "{ProductGroupName}", GetSafeValue("ProductGroupName"));
                    FindAndReplace(wordApp, "{GroupofProduct}", GetSafeValue("ProductGroupName"));
                    FindAndReplace(wordApp, "{ProductDescription}", GetSafeValue("SampleDescription"));
                    FindAndReplace(wordApp, "{LotNo}", GetSafeValue("LotNumber"));
                    FindAndReplace(wordApp, "{MaterialName}", GetSafeValue("MaterialName_Code"));
                    FindAndReplace(wordApp, "{Email}", GetSafeValue("Email"));
                    FindAndReplace(wordApp, "{CountryOfDestination}", GetSafeValue("CountryOfDestination"));
                    FindAndReplace(wordApp, "{P.O.Order}", GetSafeValue("PONo"));
                    FindAndReplace(wordApp, "{ArticleNumber}", GetSafeValue("ArticleNumber"));
                    FindAndReplace(wordApp, "{Attention}", GetSafeValue("Applicant"));
                    FindAndReplace(wordApp, "{TestSample}", GetSafeValue("TestSample"));
                    FindAndReplace(wordApp, "{SampleReceivedDate}", GetSafeDate("IssueDate"));
                    FindAndReplace(wordApp, "{TestPerformancedate}", GetSafeDate("Date"));
                    FindAndReplace(wordApp, "{IssueDate}", GetSafeDate("IssueDate"));
                    FindAndReplace(wordApp, "{TestPeriod}", GetSafeDate("TestPeriod"));
                    FindAndReplace(wordApp, "{Samplecondition}", GetSafeValue("SampleCondition"));
                    FindAndReplace(wordApp, "{Applicant}", GetSafeValue("Applicant"));
                    FindAndReplace(wordApp, "{Address}", GetSafeValue("Address"));
                    FindAndReplace(wordApp, "{SampleDescription}", GetSafeValue("SampleDescription"));
                    FindAndReplace(wordApp, "{Material Name/Code}", GetSafeValue("MaterialName_Code"));
                    FindAndReplace(wordApp, "{Sample1}", GetSafeValue("SampleDescription"));
                    FindAndReplace(wordApp, "{Buyer'sName}", GetSafeValue("Buyer'sName"));
                    FindAndReplace(wordApp, "{CountryofOrigin}", GetSafeValue("CountryOrigin"));
                    FindAndReplace(wordApp, "{BrandProduct}", GetSafeValue("BrandProduct"));
                    FindAndReplace(wordApp, "{AgeGroup}", GetSafeValue("AgeGroup"));
                    FindAndReplace(wordApp, "{Suppliername}", GetSafeValue("Suppliername"));
                    FindAndReplace(wordApp, "{FactoryName}", GetSafeValue("FactoryName"));
                    FindAndReplace(wordApp, "{FactoryAddress}", GetSafeValue("FactoryAddress"));
                    FindAndReplace(wordApp, "{MaterialType}", GetSafeValue("MaterialType"));
                    FindAndReplace(wordApp, "{ItemStyleNo}", GetSafeValue("ItemStyleNo"));
                    FindAndReplace(wordApp, "{MaterialName_Code}", GetSafeValue("MaterialName_Code"));
                    FindAndReplace(wordApp, "{ColorName_Code}", GetSafeValue("ColorName_Code"));
                    FindAndReplace(wordApp, "{BatchNo}", GetSafeValue("BatchNo"));
                    ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
                    FindAndReplace(wordApp, "{TestReportNo.}", GetSafeValue("TestReportNo"));

                    ReplaceInHeaderFooter(wordApp, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{DocNo}", "TSQ-LAB-QAF-089");
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

                    if (ReportIDGet == 28)
                    {
                        //ExcelToWord();
                        AdditionalDataAdded(wordApp, doc, "{AdditionalData}");
                       // AdditionalDataAdded1(wordApp, doc, "{AdditionalData}", outputDocxPath);
                    }

                    int counter = 0;
                    if (dt1.Rows.Count > 0)
                    {
                        string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');

                        for (int i = 0; i < query.Length; i++)
                        {
                            counter = i;
                            if (table[i] == "10")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable10(wordApp, doc, "{ResultSummaryTable}", dt2);
                            }

                            if (table[i] == "11")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable11(wordApp, doc, "{ResultTable1}", dt2);
                            }

                            if (table[i] == "12")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable11(wordApp, doc, "{TestParameter}", dt2);
                            }

                            if (table[i] == "13")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                List<string> imagePaths1 = new List<string>();
                                foreach (DataRow row in dt2.Rows)
                                {
                                    string imageFileName = row["filename"].ToString().Trim();
                                    if (!string.IsNullOrEmpty(imageFileName))
                                    {
                                        string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                                        if (File.Exists(fullImagePath))
                                        {
                                            imagePaths1.Add(fullImagePath);
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Image not found: {fullImagePath}");
                                        }
                                    }
                                }
                                InsertImagesIntoDocument1(wordApp, doc, "{images}", imagePaths1);
                            }

                            if (table[i] == "14")
                            {
                                dt2 = _cm.getReportQuery(1, query[i]);
                                CreateTable14(wordApp, doc, "{Testingandmeasuringequipment}", dt2);
                            }

                            if (table[i] == "15")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable15(wordApp, doc, "{ResultTable1}", dt2);
                            }

                            if (table[i] == "16")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable16(wordApp, doc, "{ResultTable2}", dt2);
                            }

                            if (table[i] == "17")
                            {
                                //dt2 = _cm.getReportQuery(1, query[i] + s);
                                dt2 = _cm.GetResultOverallConclusion(s);
                                CreateTable17(wordApp, doc, "{Overall Conclusion}", dt2);
                            }

                            if (table[i] == "18")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable18(wordApp, doc, "{ResultTable1}", dt2);
                            }

                            if (table[i] == "19")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable19(wordApp, doc, "{ResultTable2}", dt2);
                            }

                            if (table[i] == "20")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTableforSampleDescription(wordApp, doc, "{SampleDescriptionData}", dt2);
                            }

                        }

                        try
                        {
                            if (counter == query.Length - 1)
                            {
                                string directory = System.IO.Path.GetDirectoryName(outputPdfPath);
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                // Save documents
                                doc.SaveAs2(outputDocxPath);
                                doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);

                                //// Clean up COM objects
                                if (doc != null)
                                {
                                    doc.Close(false, Type.Missing, Type.Missing);
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                                }


                                // Return PDF to client
                                Response.Clear();
                                Response.ContentType = "application/pdf";
                                Response.AddHeader("content-disposition", "inline; filename=TestReport-" + uniqueFileName + ".pdf");
                                Response.WriteFile(outputPdfPath);
                                Response.End();
                            }
                        }
                        catch (Exception ex)
                        {
                            // More detailed error handling
                            string errorMsg = "Error generating PDF: " + ex.Message;
                            if (ex.InnerException != null)
                            {
                                errorMsg += "\nInner Exception: " + ex.InnerException.Message;
                            }
                            Response.Write(errorMsg);
                        }
                        finally
                        {
                            // Ensure cleanup even if error occurs
                            if (doc != null)
                            {
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                            }
                            if (wordApp != null)
                            {
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                            }
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
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

                //// Force-kill any lingering WINWORD processes started by this user session
                //foreach (var process in System.Diagnostics.Process.GetProcessesByName("WINWORD"))
                //{
                //    if (string.IsNullOrEmpty(process.MainWindowTitle))
                //    {
                //        process.Kill();
                //    }
                //}



            }

        }

        protected void ExcelToWord1()
        {
            // Create Excel application
            var excelApp = new Excel.Application();
            excelApp.Visible = false;

            // Open Excel workbook
            var workbook = excelApp.Workbooks.Open(@"D:\Projects\Projects-2024-25\LIMSPTQualis\FilesUpload\262.xlsx");
            Excel._Worksheet worksheet = workbook.Sheets[1];
            Excel.Range rangeToCopy = worksheet.Range["A1:B5"]; // Change range as needed

            // Copy the range
            rangeToCopy.Copy();

            // Create Word application
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true; // Optional: set to false if you don't want to show Word
            var wordDoc = wordApp.Documents.Add();

            // Paste into Word document
            Microsoft.Office.Interop.Word.Range wordRange = wordDoc.Content;
            wordRange.Paste();

            // Optional: Save and close
            // wordDoc.SaveAs2(@"C:\Path\To\Save\WordFile.docx");
            // wordDoc.Close();
            // wordApp.Quit();

            // Cleanup Excel
            workbook.Close(false);
            excelApp.Quit();

            // Release COM objects
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rangeToCopy);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);

            Console.WriteLine("Range copied from Excel and pasted into Word.");
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

            /* Rest of your barcode generation code remains the same */
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            /* Choose a barcode format */
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            Bitmap barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);
            /* Save the barcode image to the folder */
            string barcodeFileName = QRCodeFileName;
            string barcodeFilePath = System.IO.Path.Combine(barcodeFolderPath, barcodeFileName);
            barcodeBitmap.Save(barcodeFilePath);

        }

        static void FindAndReplace(Microsoft.Office.Interop.Word.Application wordApp, string findText, string replaceText)
        {
            Document doc = wordApp.ActiveDocument;
            Microsoft.Office.Interop.Word.Range range = doc.Content;
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
        private void InsertImagesIntoDocument(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, List<string> imagePaths)
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
                    Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(range, rows, 1);
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
                                    Microsoft.Office.Interop.Word.Cell cell = table.Cell(row, col);
                                    cell.Range.InlineShapes.AddPicture(imagePath);
                                    // Resize the image as needed
                                    Microsoft.Office.Interop.Word.InlineShape shape = cell.Range.InlineShapes[1];
                                    //shape.Width = 100; // Set desired width (adjust as needed)
                                    //shape.Height = 100; // Maintain aspect ratio
                                    imageIndex++;
                                }
                            }
                        }
                    }
                    // Move the range to the end of the table to continue processing if needed
                    range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }

        }

        /* for single/multiple Images */
        private void InsertImagesIntoDocument1Work(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, List<string> imagePaths1)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imagePaths1 != null && imagePaths1.Count > 0)
                {
                    // Determine the number of rows and columns needed (2 images per row)
                    int imagesPerRow = 2;
                    int totalImages = imagePaths1.Count;
                    int rows = (int)Math.Ceiling((double)totalImages / imagesPerRow);

                    // Create a table with rows and columns (2 images per row)
                    Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(range, rows, imagesPerRow);

                    // Set table properties (remove table borders)
                    table.Borders.Enable = 0; // Disable table borders

                    // Apply padding around the images and cell properties
                    foreach (Microsoft.Office.Interop.Word.Cell cell in table.Range.Cells)
                    {
                        cell.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // Center the image vertically
                        cell.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic; // Remove background color
                        cell.TopPadding = 5; // Set top padding (adjust as needed)
                        cell.BottomPadding = 5; // Set bottom padding (adjust as needed)
                        cell.LeftPadding = 5; // Set left padding (adjust as needed)
                        cell.RightPadding = 5; // Set right padding (adjust as needed)
                    }

                    // Populate the table with images
                    int imageIndex = 0;
                    for (int row = 1; row <= rows; row++)
                    {
                        for (int col = 1; col <= imagesPerRow; col++)
                        {
                            if (imageIndex < totalImages)
                            {
                                string imagePath1 = imagePaths1[imageIndex];
                                if (System.IO.File.Exists(imagePath1))
                                {
                                    Microsoft.Office.Interop.Word.Cell cell = table.Cell(row, col);
                                    cell.Range.InlineShapes.AddPicture(imagePath1);

                                    // Resize the image as needed
                                    Microsoft.Office.Interop.Word.InlineShape shape = cell.Range.InlineShapes[1];
                                    shape.Width = 100; // Set desired width (adjust as needed)
                                    shape.Height = 100; // Maintain aspect ratio (or adjust as needed)

                                    imageIndex++;
                                }
                            }
                        }
                    }

                    // Move the range to the end of the table to continue processing if needed
                    range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        private void InsertImagesIntoDocument1(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, List<string> imagePaths1)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imagePaths1 != null && imagePaths1.Count > 0)
                {
                    // Determine the number of rows and columns needed (2 images per row)
                    int imagesPerRow = 2;
                    int totalImages = imagePaths1.Count;
                    int rows = (int)Math.Ceiling((double)totalImages / imagesPerRow);

                    // Create a table with rows and columns (2 images per row)
                    Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(range, rows, imagesPerRow);

                    // Set table properties (remove table borders)
                    table.Borders.Enable = 1; // Disable table borders

                    // Apply padding around the images and cell properties
                    foreach (Microsoft.Office.Interop.Word.Cell cell in table.Range.Cells)
                    {
                        cell.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // Center the image vertically
                        cell.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic; // Remove background color
                        cell.TopPadding = 10; // Set top padding (adjust as needed)
                        cell.BottomPadding = 10; // Set bottom padding (adjust as needed)
                        cell.LeftPadding = 10; // Set left padding (adjust as needed)
                        cell.RightPadding = 10; // Set right padding (adjust as needed)
                    }

                    // Populate the table with images
                    int imageIndex = 0;
                    for (int row = 1; row <= rows; row++)
                    {
                        for (int col = 1; col <= imagesPerRow; col++)
                        {
                            if (imageIndex < totalImages)
                            {
                                string imagePath1 = imagePaths1[imageIndex];
                                if (System.IO.File.Exists(imagePath1))
                                {
                                    Microsoft.Office.Interop.Word.Cell cell = table.Cell(row, col);
                                    cell.Range.InlineShapes.AddPicture(imagePath1);

                                    // Resize the image as needed
                                    Microsoft.Office.Interop.Word.InlineShape shape = cell.Range.InlineShapes[1];
                                    shape.Width = 125; // Set desired width (adjust as needed)
                                    shape.Height = 125; // Maintain aspect ratio (or adjust as needed)

                                    imageIndex++;
                                }
                            }
                        }
                    }

                    // Move the range to the end of the table to continue processing if needed
                    range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        /* placeholder replace for header and footer all pages |Start| */

        private void ReplaceInHeaderFooter(Microsoft.Office.Interop.Word.Application wordApp, string placeholder, string replacement)
        {
            // Replace in headers
            foreach (Microsoft.Office.Interop.Word.Section section in wordApp.ActiveDocument.Sections)
            {
                foreach (Microsoft.Office.Interop.Word.HeaderFooter headerFooter in section.Headers)
                {
                    FindAndReplaceInRange(headerFooter.Range, placeholder, replacement);
                }

                foreach (Microsoft.Office.Interop.Word.HeaderFooter headerFooter in section.Footers)
                {
                    FindAndReplaceInRange(headerFooter.Range, placeholder, replacement);
                }
            }
        }

        private void FindAndReplaceInRange(Microsoft.Office.Interop.Word.Range range, string placeholder, string replacement)
        {
            Microsoft.Office.Interop.Word.Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            find.Replacement.ClearFormatting();
            find.Replacement.Text = replacement;
            find.Execute(Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
        }

        /* placeholder replace for header and footer all pages |End | */
        /*******************************************************************| Chemical Start |********************************************************************/

        private int lastUsedNumber = 0;

        private void ReplaceTextInDocument(Document doc, string findText, string replaceText)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = findText;
            range.Find.Replacement.Text = replaceText;
            range.Find.Execute(Replace: WdReplace.wdReplaceAll);
        }


        private void CreateTable10(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                // Create a table in Word based on the DataTable
                Table table = doc.Tables.Add(range, dt.Rows.Count + 1, dt.Columns.Count);

                // Set overall table properties
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;

                // Set preferred width for the entire table (100% of page width)
                table.PreferredWidth = 100;
                table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Set column headers
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = dt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                    // Set equal width for each column
                    float columnWidth = 100f / dt.Columns.Count;
                    table.Columns[col + 1].PreferredWidth = columnWidth;
                    table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                }

                // Fill the table with data
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        string cellText = dt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;
                    }
                }

                // Set table formatting
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;



                // Ensure text wrapping is enabled
                table.AllowPageBreaks = true;

                // Optional: Set minimum row height if needed
                foreach (Row row in table.Rows)
                {
                    row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                    row.Height = 20; // Minimum height in points
                }
            }
        }


        /* ChemResultTable_AO1_Without_TSSP */
        private void CreateTable11(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            // Keep track of the current placeholder position
            Microsoft.Office.Interop.Word.Range currentRange = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++; // Increment the number for each group

                // Create a new DataTable for this group
                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders with values from the first row of this group
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without the extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                // List to hold columns to remove
                List<DataColumn> columnsToRemove = new List<DataColumn>();

                // Check each column for all NULL or blank values
                foreach (DataColumn column in modifiedDt.Columns)
                {
                    bool allNullOrBlank = true;

                    foreach (DataRow row in groupDt.Rows)
                    {
                        string value = row[column.ColumnName]?.ToString();
                        if (!string.IsNullOrWhiteSpace(value)) // Check if not null or empty
                        {
                            allNullOrBlank = false;
                            break;
                        }
                    }

                    if (allNullOrBlank)
                    {
                        columnsToRemove.Add(column); // Add to list of columns to remove
                    }
                }

                // Now remove the columns that were identified as all NULL or blank
                foreach (var column in columnsToRemove)
                {
                    modifiedDt.Columns.Remove(column);
                }

                // Copy the data to new DataTable (modifiedDt will now have columns removed)
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position for the table
                if (isFirstGroup)
                {
                    // For the first group, use the original placeholder
                    var range = doc.Content;
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;
                    range.Find.Execute();
                    if (range.Find.Found)
                    {
                        currentRange = range;
                    }
                }
                else
                {
                    // For subsequent groups, duplicate the section template and update placeholders
                    if (currentRange != null)
                    {
                        // Add two paragraph breaks for spacing
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert new section header and placeholder text using lastUsedNumber
                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert pre-treatment line
                        currentRange.Text = $"          Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    // Create table with modified DataTable
                    Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    // Set overall table properties
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set column headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill the table with data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    // Set table formatting
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    // Update current range to end of table
                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }


        /* ChemResultTable_AO1_With_TSSP */
        private void CreateTable12(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            // Keep track of the current placeholder position
            Microsoft.Office.Interop.Word.Range currentRange = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++; // Continue numbering from where the previous method left off

                // Create a new DataTable for this group
                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders with values from the first row of this group
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        // Replace {Number} with the actual number in the placeholder
                        string testParamValue = $"{lastUsedNumber}.    ";
                        ReplaceTextInDocument(doc, "{Number}", testParamValue);

                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without the extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Test Item" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                // Copy the data to new DataTable
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position for the table
                if (isFirstGroup)
                {
                    // For the first group, use the original placeholder
                    var range = doc.Content;
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;
                    range.Find.Execute();
                    if (range.Find.Found)
                    {
                        currentRange = range;
                    }
                }
                else
                {
                    // For subsequent groups, duplicate the section template and update placeholders
                    if (currentRange != null)
                    {
                        // Add two paragraph breaks for spacing
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert new section header and placeholder text using lastUsedNumber
                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}:{groupDt.Rows[0]["Test Item"]} \n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert pre-treatment line
                        currentRange.Text = $"          Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    // Create table with modified DataTable
                    Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    // Set overall table properties
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set column headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill the table with data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    // Set table formatting
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    // Update current range to end of table
                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }


        /* CreateCalibrationEquipmentTable */
        private void CreateTable14(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                // Create a new DataTable to hold the necessary columns
                DataTable newDt = new DataTable();

                // Copy all columns from the DataTable (no filtering in this case, keeping them dynamic)
                foreach (DataColumn col in dt.Columns)
                {
                    newDt.Columns.Add(col.ColumnName, col.DataType);
                }

                // Add rows to newDt for final table
                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = newDt.NewRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        newRow[col.ColumnName] = row[col];
                    }
                    newDt.Rows.Add(newRow);
                }

                // Create the table in Word document
                Table table = doc.Tables.Add(range, newDt.Rows.Count + 1, newDt.Columns.Count);
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;

                // Set the column headers dynamically based on the DataTable
                for (int col = 0; col < newDt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = newDt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                }

                // Fill in the table data rows dynamically based on the DataTable
                for (int row = 0; row < newDt.Rows.Count; row++)
                {
                    for (int col = 0; col < newDt.Columns.Count; col++)
                    {
                        string cellText = newDt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;
                    }
                }

                // Auto-fit and finalize table formatting
                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            }
            else
            {
                range.Text = "No calibration equipment data available.";
            }
        }


        /* ChemResultTable_General_Without_TSSP */
        private void CreateTable15(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            // Keep track of the current placeholder position
            Microsoft.Office.Interop.Word.Range currentRange = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++; // Increment the number for each group

                // Create a new DataTable for this group
                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders with values from the first row of this group
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());

                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without the extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                // Copy the data to new DataTable
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position for the table
                if (isFirstGroup)
                {
                    // For the first group, use the original placeholder
                    var range = doc.Content;
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;
                    range.Find.Execute();
                    if (range.Find.Found)
                    {
                        currentRange = range;
                    }
                }
                else
                {
                    // For subsequent groups, duplicate the section template and update placeholders
                    if (currentRange != null)
                    {
                        // Add two paragraph breaks for spacing
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert new section header and placeholder text using lastUsedNumber
                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert pre-treatment line
                        currentRange.Text = $"          *)Analysis was performed by {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    // Create table with modified DataTable
                    Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    // Set overall table properties
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set column headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill the table with data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    // Set table formatting
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    // Update current range to end of table
                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }


        /* ChemResultTable_General_Without_TSSP */
        private void CreateTable16(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            // Keep track of the current placeholder position
            Microsoft.Office.Interop.Word.Range currentRange = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++; // Continue numbering from where the previous method left off

                // Create a new DataTable for this group
                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders with values from the first row of this group
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        // Replace {Number} with the actual number in the placeholder
                        string testParamValue = $"{lastUsedNumber}.    ";
                        ReplaceTextInDocument(doc, "{Number}", testParamValue);

                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without the extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Test Item" &&
                        col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                // Copy the data to new DataTable
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position for the table
                if (isFirstGroup)
                {
                    // For the first group, use the original placeholder
                    var range = doc.Content;
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;
                    range.Find.Execute();
                    if (range.Find.Found)
                    {
                        currentRange = range;
                    }
                }
                else
                {
                    // For subsequent groups, duplicate the section template and update placeholders
                    if (currentRange != null)
                    {
                        // Add two paragraph breaks for spacing
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert new section header and placeholder text using lastUsedNumber
                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}:{groupDt.Rows[0]["Test Item"]} \n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert pre-treatment line
                        currentRange.Text = $"          *)Analysis was performed by {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    // Create table with modified DataTable
                    Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    // Set overall table properties
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set column headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill the table with data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    // Set table formatting
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    // Update current range to end of table
                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }


        /* K3L_Overallcoclusion */
        private void CreateTable17(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ReplaceTextInDocument(doc, "{Overall Conclusion}", "No data available");
                ReplaceTextInDocument(doc, "{Requirement}", "No requirements available");
                return;
            }

            // First handle Requirements placeholder
            var requirements = dt.AsEnumerable()
                .Select(row => row["Requirement"].ToString())
                .Where(req => !string.IsNullOrWhiteSpace(req))
                .Distinct()
                .ToList();

            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            ReplaceTextInDocument(doc, "{Requirement}", combinedRequirements);

            // Create a new DataTable without the Requirement column
            DataTable modifiedDt = dt.Copy();
            modifiedDt.Columns.Remove("Requirement");

            // Handle table creation for Overall Conclusion
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = "{Overall Conclusion}";
            range.Find.Execute();

            if (range.Find.Found)
            {
                // Create a table in Word based on the modified DataTable
                Table table = doc.Tables.Add(range, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                // Set overall table properties
                table.Borders.Enable = 1;
                table.AllowAutoFit = false;
                table.PreferredWidth = 100;
                table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Define fixed widths in % for each column
                float[] columnWidths = { 10f, 35f, 35f, 20f }; // Adjust if needed
                float usableWidth = wordApp.InchesToPoints(6.5f); // Assumed usable width

                // Apply width to entire columns (header + data)
                for (int col = 0; col < modifiedDt.Columns.Count; col++)
                {
                    float colWidth = (columnWidths[col] / 100f) * usableWidth;
                    table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.Columns[col + 1].PreferredWidth = colWidth;
                }

                // Set column headers
                for (int col = 0; col < modifiedDt.Columns.Count; col++)
                {
                    var cell = table.Cell(1, col + 1);
                    cell.Range.Text = modifiedDt.Columns[col].ColumnName;
                    cell.Range.Bold = 1;
                    cell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                }

                // Fill the table with data
                for (int row = 0; row < modifiedDt.Rows.Count; row++)
                {
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;
                    }
                }

                // Format: center align all cells
                foreach (Row row in table.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }
                }

                // Set heading row
                table.Rows[1].HeadingFormat = -1;

                // Borders and row height
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.AllowPageBreaks = true;

                foreach (Row row in table.Rows)
                {
                    row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                    row.Height = 20;
                }
            }
        }


        /* ChemResultTable_K3L_Without_TSSP */
        /* old code */
        /*
        private void CreateTable18(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable().GroupBy(row => row.Field<string>("Test Parameter")).ToList();
            // Keep track of the current placeholder position
            Range currentRange = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++; // Increment the number for each group
                // Create a new DataTable for this group
                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }
                // Replace placeholders with values from the first row of this group
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }
                // Create modified DataTable without the extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }
                // List to hold columns to remove
                List<DataColumn> columnsToRemove = new List<DataColumn>();
                // Check each column for all NULL or blank values
                foreach (DataColumn column in modifiedDt.Columns)
                {
                    bool allNullOrBlank = true;

                    foreach (DataRow row in groupDt.Rows)
                    {
                        string value = row[column.ColumnName]?.ToString();
                        if (!string.IsNullOrWhiteSpace(value)) // Check if not null or empty
                        {
                            allNullOrBlank = false;
                            break;
                        }
                    }

                    if (allNullOrBlank)
                    {
                        columnsToRemove.Add(column); // Add to list of columns to remove
                    }
                }

                // Now remove the columns that were identified as all NULL or blank
                foreach (var column in columnsToRemove)
                {
                    modifiedDt.Columns.Remove(column);
                }

                // Copy the data to new DataTable (modifiedDt will now have columns removed)
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position for the table
                if (isFirstGroup)
                {
                    // For the first group, use the original placeholder
                    var range = doc.Content;
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;
                    range.Find.Execute();
                    if (range.Find.Found)
                    {
                        currentRange = range;
                    }
                }
                else
                {
                    // For subsequent groups, duplicate the section template and update placeholders
                    if (currentRange != null)
                    {
                        // Add two paragraph breaks for spacing
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert new section header and placeholder text using lastUsedNumber
                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert pre-treatment line
                        currentRange.Text = $"          Test Method:{groupDt.Rows[0]["Method*"]}, analyzed by {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    // Create table with modified DataTable
                    Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    // Set overall table properties
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set column headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill the table with data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    // Set table formatting
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    // Update current range to end of table
                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }
        */

        /*
        private void CreateTable18(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable().GroupBy(row => row.Field<string>("Test Parameter")).ToList();
                Range currentRange = null;
                bool isFirstGroup = true;

                foreach (var testParamGroup in groupedData)
                {
                    lastUsedNumber++;

                    var elementGroups = testParamGroup
                        .GroupBy(row => new
                        {
                            Element = row.Field<string>("Element"),
                            CASNumber = row.Field<string>("CAS Number")
                        })
                        .ToList();

                    var allColorCodes = testParamGroup
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .SelectMany(code => code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    var firstRow = testParamGroup.First();
                    ReplaceTextInDocument(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());
                    ReplaceTextInDocument(doc, "{TestMethod}", firstRow["Method*"].ToString());
                    ReplaceTextInDocument(doc, "{Equipment}", firstRow["Equipment"].ToString());

                    if (!isFirstGroup && currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.Text = $"{lastUsedNumber}.      {firstRow["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.Text = $"          Test Method: {firstRow["Method*"]}, analyzed by {firstRow["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    if (isFirstGroup)
                    {
                        var range = doc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Text = placeholder;
                        range.Find.Execute();
                        if (range.Find.Found)
                        {
                            currentRange = range;
                        }
                    }

                    if (currentRange != null)
                    {
                        int fixedColumns = 3;
                        int resultSubCols = allColorCodes.Count;
                        int totalColumns = fixedColumns + resultSubCols;

                        Table table = doc.Tables.Add(currentRange, elementGroups.Count + 1, totalColumns);
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = false;
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                        table.PreferredWidth = 100;

                        // Set column widths
                        table.Columns[1].PreferredWidth = 100; // Element
                        table.Columns[2].PreferredWidth = 70;  // CAS Number
                        for (int i = 3; i <= 2 + resultSubCols; i++)
                        {
                            table.Columns[i].PreferredWidth = 60; // Result columns
                        }
                        table.Columns[totalColumns].PreferredWidth = 70; // Requirement

                        // HEADER ROW - Left alignment
                        table.Cell(1, 1).Range.Text = "Element";
                        table.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        table.Cell(1, 2).Range.Text = "CAS Number";
                        table.Cell(1, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        for (int i = 0; i < allColorCodes.Count; i++)
                        {
                            table.Cell(1, 3 + i).Range.Text = "Result (mg/kg) " + allColorCodes[i];
                            table.Cell(1, 3 + i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        }

                        table.Cell(1, totalColumns).Range.Text = "Requirement";
                        table.Cell(1, totalColumns).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        // Header formatting
                        for (int j = 1; j <= totalColumns; j++)
                        {
                            table.Cell(1, j).Range.Bold = 1;
                            table.Cell(1, j).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            table.Cell(1, j).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            table.Cell(1, j).Range.ParagraphFormat.WordWrap = -1;
                        }

                        // DATA ROWS - Left alignment
                        int dataRowIndex = 2;
                        foreach (var elementGroup in elementGroups)
                        {
                            var firstElementRow = elementGroup.First();

                            // Element column
                            table.Cell(dataRowIndex, 1).Range.Text = firstElementRow["Element"].ToString();
                            table.Cell(dataRowIndex, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                            // CAS Number column
                            table.Cell(dataRowIndex, 2).Range.Text = firstElementRow["CAS Number"].ToString();
                            table.Cell(dataRowIndex, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                            // Result columns
                            for (int i = 0; i < allColorCodes.Count; i++)
                            {
                                table.Cell(dataRowIndex, 3 + i).Range.Text = "";
                                table.Cell(dataRowIndex, 3 + i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            }

                            foreach (var row in elementGroup)
                            {
                                string result = row["Result(mg/kg)"].ToString();
                                string colourCodeRaw = row["Colour Code"]?.ToString() ?? "";

                                if (!string.IsNullOrWhiteSpace(colourCodeRaw))
                                {
                                    var individualCodes = colourCodeRaw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var code in individualCodes)
                                    {
                                        int codeIndex = allColorCodes.IndexOf(code);
                                        if (codeIndex >= 0)
                                        {
                                            table.Cell(dataRowIndex, 3 + codeIndex).Range.Text = result;
                                            table.Cell(dataRowIndex, 3 + codeIndex).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                        }
                                    }
                                }
                            }

                            // Requirement column
                            table.Cell(dataRowIndex, totalColumns).Range.Text = firstElementRow["Requirement"].ToString();
                            table.Cell(dataRowIndex, totalColumns).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                            dataRowIndex++;
                        }

                        // Format all cells
                        foreach (Row row in table.Rows)
                        {
                            row.HeightRule = WdRowHeightRule.wdRowHeightAuto;

                            foreach (Cell cell in row.Cells)
                            {
                                cell.Range.ParagraphFormat.WordWrap = -1;
                                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                cell.TopPadding = 4;
                                cell.BottomPadding = 4;
                                cell.LeftPadding = 4;
                                cell.RightPadding = 4;
                            }
                        }

                        currentRange = table.Range;
                        currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }
        */


        private void CreateTable18(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable().GroupBy(row => row.Field<string>("Test Parameter")).ToList();
                Microsoft.Office.Interop.Word.Range currentRange = null;
                bool isFirstGroup = true;

                foreach (var testParamGroup in groupedData)
                {
                    lastUsedNumber++;

                    var elementGroups = testParamGroup
                        .GroupBy(row => new
                        {
                            Element = row.Field<string>("Element"),
                            CASNumber = row.Field<string>("CAS Number")
                        })
                        .ToList();

                    var allColorCodes = testParamGroup
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .SelectMany(code => code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    var firstRow = testParamGroup.First();
                    ReplaceTextInDocument(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());
                    ReplaceTextInDocument(doc, "{TestMethod}", firstRow["Method*"].ToString());
                    ReplaceTextInDocument(doc, "{Equipment}", firstRow["Equipment"].ToString());

                    if (!isFirstGroup && currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.Text = $"{lastUsedNumber}.      {firstRow["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.Text = $"          Test Method: {firstRow["Method*"]}, analyzed by {firstRow["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    if (isFirstGroup)
                    {
                        var range = doc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Text = placeholder;
                        range.Find.Execute();
                        if (range.Find.Found)
                        {
                            currentRange = range;
                        }
                    }

                    if (currentRange != null)
                    {
                        int resultSubCols = allColorCodes.Count;
                        int totalColumns = 3 + resultSubCols;

                        Table table = doc.Tables.Add(currentRange, elementGroups.Count + 1, totalColumns);
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = false;
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;

                        // Set fixed column widths (in points)
                        float elementWidth = 130f;
                        float casWidth = 70f;
                        float resultWidth = 120f;
                        float reqWidth = 70f;

                        table.Columns[1].PreferredWidth = elementWidth;
                        table.Columns[2].PreferredWidth = casWidth;
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            table.Columns[3 + i].PreferredWidth = resultWidth;
                        }
                        table.Columns[3 + resultSubCols].PreferredWidth = reqWidth;

                        // HEADER ROW
                        table.Cell(1, 1).Range.Text = "Element";
                        table.Cell(1, 2).Range.Text = "CAS Number";

                        for (int i = 0; i < allColorCodes.Count; i++)
                        {
                            table.Cell(1, 3 + i).Range.Text = "Result (mg/kg)\n" + allColorCodes[i];
                        }

                        table.Cell(1, totalColumns).Range.Text = "Requirement";

                        // Header formatting
                        for (int j = 1; j <= totalColumns; j++)
                        {
                            var cell = table.Cell(1, j);
                            cell.Range.Bold = 1;
                            cell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            cell.WordWrap = true;
                        }

                        // DATA ROWS
                        int dataRowIndex = 2;
                        foreach (var elementGroup in elementGroups)
                        {
                            var firstElementRow = elementGroup.First();

                            table.Cell(dataRowIndex, 1).Range.Text = firstElementRow["Element"].ToString();
                            table.Cell(dataRowIndex, 2).Range.Text = firstElementRow["CAS Number"].ToString();

                            for (int i = 0; i < allColorCodes.Count; i++)
                            {
                                table.Cell(dataRowIndex, 3 + i).Range.Text = "";
                            }

                            foreach (var row in elementGroup)
                            {
                                string result = row["Result(mg/kg)"].ToString();
                                string colourCodeRaw = row["Colour Code"]?.ToString() ?? "";

                                if (!string.IsNullOrWhiteSpace(colourCodeRaw))
                                {
                                    var individualCodes = colourCodeRaw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var code in individualCodes)
                                    {
                                        int codeIndex = allColorCodes.IndexOf(code);
                                        if (codeIndex >= 0)
                                        {
                                            table.Cell(dataRowIndex, 3 + codeIndex).Range.Text = result;
                                        }
                                    }
                                }
                            }

                            table.Cell(dataRowIndex, totalColumns).Range.Text = firstElementRow["Requirement"].ToString();

                            // Apply cell formatting for the row
                            for (int c = 1; c <= totalColumns; c++)
                            {
                                var cell = table.Cell(dataRowIndex, c);
                                cell.Range.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                cell.WordWrap = true;
                                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                cell.TopPadding = 4;
                                cell.BottomPadding = 4;
                                cell.LeftPadding = 4;
                                cell.RightPadding = 4;
                            }

                            dataRowIndex++;
                        }

                        // Auto row height
                        foreach (Row row in table.Rows)
                        {
                            row.HeightRule = WdRowHeightRule.wdRowHeightAuto;
                        }

                        // Finalize current range
                        currentRange = table.Range;
                        currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }


        /* ChemResultTable_K3L_With_TSSP */
        /* old code*/
        /* private void CreateTable19(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            // Keep track of the current placeholder position
            Range currentRange = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++; // Continue numbering from where the previous method left off

                // Create a new DataTable for this group
                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders with values from the first row of this group
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        // Replace {Number} with the actual number in the placeholder
                        string testParamValue = $"{lastUsedNumber}.    ";
                        ReplaceTextInDocument(doc, "{Number}", testParamValue);

                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without the extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Test Item" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                // Copy the data to new DataTable
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position for the table
                if (isFirstGroup)
                {
                    // For the first group, use the original placeholder
                    var range = doc.Content;
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;
                    range.Find.Execute();
                    if (range.Find.Found)
                    {
                        currentRange = range;
                    }
                }
                else
                {
                    // For subsequent groups, duplicate the section template and update placeholders
                    if (currentRange != null)
                    {
                        // Add two paragraph breaks for spacing
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert new section header and placeholder text using lastUsedNumber
                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}:{groupDt.Rows[0]["Test Item"]} \n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Insert pre-treatment line
                        currentRange.Text = $"          Test Method:{groupDt.Rows[0]["Method*"]}, analyzed by {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    // Create table with modified DataTable
                    Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    // Set overall table properties
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set column headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill the table with data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    // Set table formatting
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    // Update current range to end of table
                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }*/


        private void CreateTable19(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    // No rows — replace all placeholders with blank
                    ReplaceTextInDocument(doc, "{Number}", string.Empty);
                    ReplaceTextInDocument(doc, "{TestParameter2}", string.Empty);
                    ReplaceTextInDocument(doc, "{TestSubParameter}", string.Empty);
                    ReplaceTextInDocument(doc, "{TestMethod}", string.Empty);
                    ReplaceTextInDocument(doc, "{Equipment2}", string.Empty);
                    ReplaceTextInDocument(doc, "{ResultTable2}", string.Empty);
                    ReplaceTextInDocument(doc, "{AnalysisWasPerformed}", string.Empty);
                    ReplaceTextInDocument(doc, "{:}", string.Empty);
                    return;
                }

                // Group the data by Test Parameter
                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Item"))
                    .ToList();

                // Keep track of the current placeholder position
                Microsoft.Office.Interop.Word.Range currentRange = null;
                bool isFirstGroup = true;

                foreach (var group in groupedData)
                {
                    lastUsedNumber++; // Continue numbering from where the previous method left off

                    // Create a new DataTable for this group
                    DataTable groupDt = dt.Clone();
                    foreach (var row in group)
                    {
                        groupDt.ImportRow(row);
                    }

                    // Replace placeholders with values from the first row of this group
                    if (groupDt.Rows.Count > 0)
                    {
                        if (groupDt.Columns.Contains("Test Parameter"))
                        {
                            // Replace {Number} with the actual number in the placeholder
                            string testParamValue = $"{lastUsedNumber}.    ";
                            ReplaceTextInDocument(doc, "{Number}", testParamValue);

                            ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                            ReplaceTextInDocument(doc, "{AnalysisWasPerformed}", "*) Analysis was performed ");
                            ReplaceTextInDocument(doc, "{:}", " : ");
                        }
                        if (groupDt.Columns.Contains("Test Item"))
                            ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                        if (groupDt.Columns.Contains("Method*"))
                            ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                        if (groupDt.Columns.Contains("Equipment"))
                            ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());


                    }


                    // Extract all unique color codes for this group
                    var allColorCodes = group
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .SelectMany(code => code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    // Create modified DataTable with fixed columns and dynamic result columns
                    DataTable modifiedDt = new DataTable();

                    // Add fixed columns (excluding those we'll handle specially)
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" &&
                            col.ColumnName != "Test Item" &&
                            col.ColumnName != "Method*" &&
                            col.ColumnName != "Equipment" &&
                            col.ColumnName != "Colour Code" &&
                            col.ColumnName != "Result(mg/kg)")
                        {
                            modifiedDt.Columns.Add(col.ColumnName);
                        }
                    }

                    // Add dynamic result columns for each color code
                    foreach (var colorCode in allColorCodes)
                    {
                        modifiedDt.Columns.Add($"Result (mg/kg) ({colorCode})");
                    }

                    // Group by all non-result columns to combine duplicate compounds
                    var compoundGroups = group
                        .GroupBy(row => string.Join("|",
                            modifiedDt.Columns.Cast<DataColumn>()
                                .Where(c => !c.ColumnName.StartsWith("Result (mg/kg)"))
                                .Select(c => row[c.ColumnName]?.ToString() ?? "")))
                        .ToList();

                    // Copy the data to new DataTable - one row per unique compound
                    foreach (var compoundGroup in compoundGroups)
                    {
                        DataRow newRow = modifiedDt.NewRow();

                        // Copy fixed columns from the first row in this compound group
                        var firstRow = compoundGroup.First();
                        foreach (DataColumn col in modifiedDt.Columns)
                        {
                            if (!col.ColumnName.StartsWith("Result (mg/kg)"))
                            {
                                newRow[col.ColumnName] = firstRow[col.ColumnName];
                            }
                        }

                        // Handle result columns based on color codes
                        foreach (var row in compoundGroup)
                        {
                            string colourCodeRaw = row["Colour Code"]?.ToString() ?? "";
                            string resultValue = row["Result(mg/kg)"]?.ToString() ?? "";

                            if (!string.IsNullOrWhiteSpace(colourCodeRaw))
                            {
                                var individualCodes = colourCodeRaw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var code in individualCodes)
                                {
                                    string columnName = $"Result (mg/kg) ({code})";
                                    if (modifiedDt.Columns.Contains(columnName))
                                    {
                                        newRow[columnName] = resultValue;
                                    }
                                }
                            }
                        }

                        modifiedDt.Rows.Add(newRow);
                    }

                    // Rest of your existing code remains the same...
                    // Find or create position for the table
                    if (isFirstGroup)
                    {
                        // For the first group, use the original placeholder
                        var range = doc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Text = placeholder;
                        range.Find.Execute();
                        if (range.Find.Found)
                        {
                            currentRange = range;
                        }
                    }
                    else
                    {
                        // For subsequent groups, duplicate the section template and update placeholders
                        if (currentRange != null)
                        {
                            // Add two paragraph breaks for spacing
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;

                            // Insert new section header and placeholder text using lastUsedNumber
                            currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}:{groupDt.Rows[0]["Test Item"]} \n";
                            currentRange.Font.Bold = 1;
                            currentRange.Font.Size = 11;
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;

                            // Insert pre-treatment line
                            currentRange.Text = $"          Test Method:{groupDt.Rows[0]["Method*"]}, analyzed by {groupDt.Rows[0]["Equipment"]}\n";
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
                    }

                    if (currentRange != null)
                    {
                        // Create table with modified DataTable
                        Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                        // Set overall table properties
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = true;
                        table.PreferredWidth = 100;
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                        // Set column headers
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                            table.Cell(1, col + 1).Range.Bold = 1;
                            table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                            // Adjust column widths - make result columns narrower
                            float columnWidth = modifiedDt.Columns[col].ColumnName.StartsWith("Result (mg/kg)")
                                ? 60f / modifiedDt.Columns.Count
                                : 100f / modifiedDt.Columns.Count;

                            table.Columns[col + 1].PreferredWidth = columnWidth;
                            table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                        }

                        // Fill the table with data
                        for (int row = 0; row < modifiedDt.Rows.Count; row++)
                        {
                            for (int col = 0; col < modifiedDt.Columns.Count; col++)
                            {
                                string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                                table.Cell(row + 2, col + 1).Range.Text = cellText;
                            }
                        }

                        // Set table formatting
                        table.Rows[1].HeadingFormat = -1;
                        table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table.AllowPageBreaks = true;
                        foreach (Row row in table.Rows)
                        {
                            row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                            row.Height = 20;
                        }

                        // Update current range to end of table
                        currentRange = table.Range;
                        currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    }
                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }


        /* for Sample Description */
        private void CreateTableforSampleDescription(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                    throw new Exception("No data available to create the table.");

                Microsoft.Office.Interop.Word.Range range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    range.Text = "";

                    int totalRows = dt.Rows.Count + 1; // header + data rows
                    int totalColumns = 3; // Sr.No, Sample Description, Sample ID

                    Table table = doc.Tables.Add(range, totalRows, totalColumns);
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = false;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.PreferredWidth = 100;

                    float totalWidthPoints = wordApp.InchesToPoints(6.5f); // Assuming usable page width is ~6.5 inches
                    float[] columnWidths = { 10f, 65f, 25f }; // Percentages

                    // Set header texts
                    table.Cell(1, 1).Range.Text = "Sr. No.";
                    table.Cell(1, 2).Range.Text = "Sample Description";
                    table.Cell(1, 3).Range.Text = "Sample ID";

                    // Format header
                    for (int col = 1; col <= totalColumns; col++)
                    {
                        var cell = table.Cell(1, col);
                        cell.Range.Bold = 1;
                        cell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }

                    // Fill data rows
                    for (int row = 0; row < dt.Rows.Count; row++)
                    {
                        int rowIndex = row + 2;

                        table.Cell(rowIndex, 1).Range.Text = (row + 1).ToString(); // Sr. No.
                        table.Cell(rowIndex, 2).Range.Text = dt.Rows[row]["Sample Description"].ToString();
                        table.Cell(rowIndex, 3).Range.Text = dt.Rows[row]["Sample ID"].ToString();
                    }

                    // Set column widths
                    for (int i = 0; i < totalColumns; i++)
                    {
                        float width = (columnWidths[i] / 100f) * totalWidthPoints;
                        table.Columns[i + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                        table.Columns[i + 1].PreferredWidth = width;
                    }

                    // Center align all cells
                    foreach (Row row in table.Rows)
                    {
                        foreach (Cell cell in row.Cells)
                        {
                            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                    }

                    table.Rows[1].HeadingFormat = -1; // Mark first row as header
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                }
                else
                {
                    throw new Exception($"Placeholder '{placeholder}' not found in the document.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Sample Description table: " + ex.Message);
            }
        }


        /* WHEN K3L GETTING THEN NEW REPORT ATTACH */

        public void AdditionalDataAdded(Microsoft.Office.Interop.Word.Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            Excel.Range range = null;
            Excel.Sheets sheets = null;

            try
            {
                string path = Server.MapPath("~/FilesUpload/");
                string sampleID = ViewState["SampleIDGet"].ToString();
                string filePath = path + sampleID + ".xlsx";
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Excel file not found.", filePath);
                }
               
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                workbook = excelApp.Workbooks.Open(filePath);
                sheets = workbook.Sheets;
                worksheet = (Excel.Worksheet)sheets[1];
                range = worksheet.UsedRange;
                range.Copy();

                System.Threading.Thread.Sleep(500);

                var wordRange = doc.Content;
                wordRange.Find.ClearFormatting();
                wordRange.Find.Text = placeholder;
                // Paste into Word document
               
                if (wordRange.Find.Execute())
                {
                    //wordRange.Text = "";
                    wordRange.Paste();
                    workbook.Save();
                }

                Table wordTable = doc.Tables[doc.Tables.Count];
                try
                {
                    wordTable.Select();
                    wordApp.Selection.Font.Size = 8;
                }
                catch (Exception fontEx1)
                {
                    try
                    {
                        object start = wordTable.Range.Start;
                        object end = wordTable.Range.End;
                        Microsoft.Office.Interop.Word.Range tableRange = doc.Range(ref start, ref end);
                        tableRange.Font.Size = 8;
                    }
                    catch (Exception fontEx2)
                    {
                        try
                        {
                            foreach (Row row in wordTable.Rows)
                            {
                                row.Range.Font.Size = 8;
                            }
                        }
                        catch (Exception fontEx3)
                        {
                            try
                            {
                                for (int row = 1; row <= wordTable.Rows.Count; row++)
                                {
                                    for (int col = 1; col <= wordTable.Columns.Count; col++)
                                    {
                                        wordTable.Cell(row, col).Range.Font.Size = 8;
                                    }
                                }
                            }
                            catch (Exception fontEx4)
                            {
                                System.Diagnostics.Debug.WriteLine("Could not set font size: " +
                                    fontEx1.Message + " | " + fontEx2.Message + " | " +
                                    fontEx3.Message + " | " + fontEx4.Message);
                            }
                        }
                    }
                }

                // Set fixed column widths for A to H
                float[] columnWidths = { 50f, 70f, 90f, 110f, 80f, 100f, 60f, 120f };
                int maxColumns = Math.Min(wordTable.Columns.Count, columnWidths.Length);
                for (int col = 1; col <= maxColumns; col++)
                {
                    wordTable.Columns[col].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    wordTable.Columns[col].PreferredWidth = columnWidths[col - 1];
                }
            }
            catch (Exception ex)
            {               
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
            finally
            {
                // Proper cleanup of Excel COM objects
                if (workbook != null)
                {
                    workbook.Close(false, Type.Missing, Type.Missing);
                    Marshal.ReleaseComObject(workbook);
                }
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (sheets != null) Marshal.ReleaseComObject(sheets);
                if (range != null) Marshal.ReleaseComObject(range);

                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void AdditionalDataAdded1(Microsoft.Office.Interop.Word.Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, string docPath)
        {
            try
            {
                // Create Excel app
                var excelApp = new Excel.Application();
                string path = Server.MapPath("~/FilesUpload/");
                string sampleID = ViewState["SampleIDGet"].ToString();
                string filePath = path + sampleID + ".xlsx";
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Excel file not found.", filePath);
                }
                var workbook = excelApp.Workbooks.Open(filePath);
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                Excel.Range range = worksheet.Range["A1", "C5"];
                range.Copy();

                // Small delay to ensure clipboard gets populated
                System.Threading.Thread.Sleep(500);

                // Create Word app
                wordApp.Visible = false;
                Microsoft.Office.Interop.Word.Range wrdRange = doc.Range(0, 0);
                wrdRange.Paste();

                // Save and close
                string wordPath = docPath+ ".docx";
                doc.SaveAs2(wordPath);
                doc.Close();
                wordApp.Quit();

                workbook.Close(false);
                excelApp.Quit();

                // Return file to browser
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Output.docx");
                Response.TransmitFile(wordPath);
                Response.End();
            }
            catch (Exception ex)
            {
                string s= ex.Message;
            }
        }


            /*******************************************************************| Chemical End |********************************************************************/





        }
}