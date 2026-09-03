using Microsoft.Office.Interop.Word;
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
using System.Text.RegularExpressions;
using System.Web.UI;
using ZXing;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;
using Table = Microsoft.Office.Interop.Word.Table;

namespace LIMS
{
    public partial class CreateReportFromTemplate_Flatware : System.Web.UI.Page
    {

        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();

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
                //if (Request.QueryString["t"] != null)
                //    t = Request.QueryString["t"].ToString();

                //int ReportIDGet = Convert.ToInt32(Request.QueryString["c"].ToString());
                int ID = Convert.ToInt32(s); string SampleUID = "";
                DataTable dt = _cm.getSampleUID(ID);

                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    GenerateQR(SampleUID);
                }

                int cloneCRFSampleID = 0;
                int reportID = 0;
                bool b1 = false; bool b2 = false; bool b3 = false;
                string templatePath = "";

                DataTable dtCloneCRFSampleIDGet = _cm.CRFSampleDetails_Softline(ID);
                if (dtCloneCRFSampleIDGet != null && dtCloneCRFSampleIDGet.Rows.Count > 0)
                {
                    foreach (DataRow row in dtCloneCRFSampleIDGet.Rows)
                    {
                        cloneCRFSampleID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["CloneCRFSampleID"].ToString());
                        reportID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["ReportID"].ToString());

                        if (reportID == 6421)
                        {
                            templatePath = path + "StainlessSteelFlatWear_ReportTemplate_1.docx";
                            b1 = true;
                        }

                        if (reportID == 6422)
                        {
                            templatePath = path + "Metalcookware_ReportTemplate_1.docx";
                            b2 = true;
                        }

                    }

                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'GenerateReport.aspx'; }, 10);</script>");
                }





                doc = wordApp.Documents.Add(templatePath);
                string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                List<string> imagePaths = new List<string> { imageUrl };
                InsertImagesIntoDocument(wordApp, doc, "{QR_Code}", imagePaths);

                DataTable LEDdetails = _cm.getFlatwaredetails(sampleID);
                if (LEDdetails != null && LEDdetails.Rows.Count > 0)
                {
                    DataRow row = LEDdetails.Rows[0];

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


                    FindAndReplace(wordApp, "{WorkOrederNumber}", GetSafeValue("WorkorderNumber"));
                    FindAndReplace(wordApp, "{ReportNumber}", GetSafeValue("TestReportNo"));
                    FindAndReplace(wordApp, "{CompanyName}", GetSafeValue("Applicant"));
                    FindAndReplace(wordApp, "{CompanyAddress}", GetSafeValue("Address"));
                    FindAndReplace(wordApp, "{ManufacturerName}", GetSafeValue("FactoryName"));
                    FindAndReplace(wordApp, "{ManufacturerAddress}", GetSafeValue("FactoryAddress"));
                    // FindAndReplace(wordApp, "{LetterNumber}", GetSafeValue("LotNumber"));

                    FindAndReplace(wordApp, "{ProductName}", GetSafeValue("ProductName"));
                    FindAndReplace(wordApp, "{ProductBrand}", GetSafeValue("BrandProduct"));
                    FindAndReplace(wordApp, "{ProductType}", GetSafeValue("MaterialType"));
                    FindAndReplace(wordApp, "{ProductSpecification}", GetSafeValue("ProductSpecification"));
                    // FindAndReplace(wordApp, "{ProductColor}", GetSafeValue("Color"));
                    FindAndReplace(wordApp, "{ProductSerialNumber}", GetSafeValue("ProductSerialNo"));

                    FindAndReplace(wordApp, "{countryofdestination}", GetSafeValue("CountryOrigin"));
                    FindAndReplace(wordApp, "{countryoforigin}", GetSafeValue("CountryDestination"));
                    FindAndReplace(wordApp, "{ProductQuantity}", GetSafeValue("ProductQuantity"));

                    FindAndReplace(wordApp, "{testmethod}", GetSafeValue("TestMethod"));

                    FindAndReplace(wordApp, "{ProductReceptionDate}", GetSafeDate("IssueDate"));
                    FindAndReplace(wordApp, "{DateOfTesting}", GetSafeDate("TestPeriod"));

                    ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
                    FindAndReplace(wordApp, "{TestReportNo.}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{DocNo}", "TSQ-LAB-QAF-098");
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

                    DataTable dt3 = _cm.getLEDdetailsForImages(ID);
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


                    if (b1 == true)
                    {

                        DataTable dtReport = GetReportData(ID, reportID);
                        CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                        DataTable dtReportT2 = GetReportDataForT2(ID, reportID);
                        CreateReportTableT2(wordApp, doc, "{Tableware}", dtReportT2);

                        DataTable dtReportT3 = GetReportDataForT3(ID, reportID);
                        CreateReportTableT3(wordApp, doc, "{Tablecookingequipment}", dtReportT3);

                        //DataTable dtReportT4 = GetReportDataForT2(ID, reportID);
                        //CreateReportTableT4(wordApp, doc, "{LAMPIRAN_D-UJI_TERMAL}", dtReportT4);

                        List<string> equipmentNames = new List<string>();
                        DataTable dtEquipment = _cm.GetEquipmentNamesRowWise(ID, reportID);
                        if (dtEquipment != null && dtEquipment.Rows.Count > 0)
                        {
                            equipmentNames = dtEquipment.AsEnumerable().Select(row => row["Equipment"].ToString()).ToList();
                        }
                        // CreateReportTableT5(wordApp, doc, "{MeasuringEquipment}", equipmentNames);


                        DataTable dtReportT6 = _cm.GetConclusionRow_LED(ID, reportID);
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
                        // First template - Metalcookware_ReportTemplate_2
                        DataTable dtReport = GetReportData(ID, reportID);
                        CreateReportTableforcookware(wordApp, doc, "{TestingAndMeasurement_MetalCookware}", dtReport);


                        DataTable dtReport_Strength = GetReportDataForStrength(ID, reportID);
                        CreateReportTableStrength(wordApp, doc, "{Strengthtest}", dtReport_Strength);


                        DataTable dtReport_FormTest = GetReportDataForFormTest(ID, reportID);
                        CreateReportTableFormTest(wordApp, doc, "{FormTest}", dtReport_FormTest);



                        DataTable dtReport_BaseMaterial = GetReportDataForBaseMaterial(ID, reportID);
                        CreateReportTableBaseMaterial(wordApp, doc, "{BaseMaterial}", dtReport_BaseMaterial);





                        DataTable dtReport_FatigueResistance = GetReportDataForFatigueResistance(ID, reportID);
                        CreateReportTableFatigueResistance(wordApp, doc, "{FatigueResistance}", dtReport_FatigueResistance);





                        DataTable dtReport_NonStickCoating = GetReportDataForNonStickCoating(ID, reportID);
                        CreateReportTableNonStickCoating(wordApp, doc, "{NonStickCoating}", dtReport_NonStickCoating);




                        DataTable dtReport_ThermalShock = GetReportDataForThermalShock(ID, reportID);
                        CreateReportTableThermalShock(wordApp, doc, "{ThermalShock}", dtReport_ThermalShock);





                        DataTable dtReport_CoatingThickness = GetReportDataForCoatingThickness(ID, reportID);
                        CreateReportTableCoatingThickness(wordApp, doc, "{CoatingThickness}", dtReport_CoatingThickness);




                        doc.SaveAs2(outputDocxPath);
                        doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);

                        // Second template - MetalCookware_ReportTemplate_1
                        string templatePath2 = path + "MetalCookware_ReportTemplate_1.docx";
                        Document doc2 = wordApp.Documents.Add(templatePath2);

                       
                        string outputDocxPath2 = pathout + "TestReport-" + uniqueFileName + "_1.docx";
                        string outputPdfPath2 = pathout + "TestReport-" + uniqueFileName + "_1.pdf";

                        doc2.SaveAs2(outputDocxPath2);
                        doc2.SaveAs2(outputPdfPath2, WdSaveFormat.wdFormatPDF);

                        Marshal.ReleaseComObject(doc2);

                        // Return PDF
                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-disposition", "inline; filename=TestReport-" + uniqueFileName + ".pdf");
                        Response.WriteFile(outputPdfPath);
                        Response.End();
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
                /*Force - kill any lingering WINWORD processes started by this user session for locally server */
               /* foreach (var process in Process.GetProcessesByName("WINWORD"))
                {
                    if (string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        process.Kill();
                    }
                }*/
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

            /* Rest of your barcode generation code remains the same */
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
                                    //shape.Width = 100; // Set desired width (adjust as needed)
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
                                c.ClauseTitle,c.isDescription,t.UnitName as Unit,
                                c.ClauseTitle_BH,
                                c.Requirement,t.Result,t.Result_BH,t.Specification,t.Specification_BH,
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
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (17954, 17955, 17956, 17957, 17958, 17960)";
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
        private DataTable GetReportDataForT3(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (18961, 18962, 18963, 18964, 18965, 18966)";
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
        private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

            // Column order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
            float[] COLUMN_WIDTHS = { 0.6f, 2.50f, 0.70f, 1.17f, 1.30f };
            float SUB_COLUMN1_WIDTH = 1.5f;
            float SUB_COLUMN2_WIDTH = 1.0f;
            float TABLE_TOTAL_WIDTH = 6.27f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // STEP 1: Count rows
            int totalRows = 1; // header row
            bool needDescHeader = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";

                if (isHeader)
                {
                    totalRows++;
                    needDescHeader = false;
                }
                else if (isDescription)
                {
                    if (!needDescHeader)
                    {
                        totalRows += 1; // sub-column header row
                        needDescHeader = true;
                    }
                    totalRows++; // actual data row
                }
                else
                {
                    totalRows++;
                    needDescHeader = false;
                }
            }

            // STEP 2: Build table with 5 main columns
            Table table = doc.Tables.Add(range, totalRows, 5);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);

            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            // Set column widths
            for (int c = 1; c <= 5; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            // MAIN HEADER ROW
            // Col order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
            string[] headers = {
        "Klausul\nClause",
        "Parameter Uji\nTest Parameters",
        "Satuan\nUnit",
        "Hasil Tes\nTest Results",
        "Persyaratan Mutu\nQuality Requirements"
    };

            for (int col = 1; col <= 5; col++)
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

            int r = 2;
            bool descHeaderShown = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";

                // ── HEADER ROW ───────────────────────────────────────────────────────
                if (isHeader)
                {
                    descHeaderShown = false;

                    // Col 1: Clause Number
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Clause Title (Test Parameters)
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Col 3: Unit - empty for header
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Quality Requirements - empty for header
                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
                // ── DESCRIPTION ROW ──────────────────────────────────────────────────
                else if (isDescription)
                {
                    // Sub-header row (shown once per description section)
                    if (!descHeaderShown)
                    {
                        Cell d1 = table.Cell(r, 1);
                        SetCellText(d1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 2: Sub-table header for Description | Condition
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

                        // Col 3: Unit header - empty
                        Cell d3 = table.Cell(r, 3);
                        SetCellText(d3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 4: Result header - empty
                        Cell d4 = table.Cell(r, 4);
                        SetCellText(d4, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 5: Quality Requirements header - empty
                        Cell d5 = table.Cell(r, 5);
                        SetCellText(d5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        r++;
                        descHeaderShown = true;
                    }

                    // Description data row
                    Cell a1 = table.Cell(r, 1);
                    SetCellText(a1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Sub-table data for Description | Condition
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

                    // Col 3: Unit (Satuan)
                    Cell a3 = table.Cell(r, 3);
                    SetCellText(a3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Specification (Quality Requirements)
                    Cell a5 = table.Cell(r, 5);
                    SetCellText(a5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
                // ── NORMAL ROW ───────────────────────────────────────────────────────
                else
                {
                    descHeaderShown = false;

                    // Col 1: Clause Number
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Requirement (Parameter Uji)
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Col 3: Unit (Satuan)
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Specification (Quality Requirements)
                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
            }
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

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 17954, 0 },  // Group
        { 17957, 1 },  // Type Of Products
        { 17955, 2 },  // Measurement results (mm)
        { 17956, 3 },  // Min thickness requirement (mm)
        { 17958, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++)
                parsedColumns.Add(new string[0]);

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

            int numRows = parsedColumns.Max(col => col.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            // +3 = 2 title rows + 1 column header row
            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            float[] colWidths = { 0.80f, 1.20f, 1.20f, 1.27f, 0.80f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── ROW 1: "Table: Product Thickness Measurement" ─────────────────────

            table.Rows[1].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell mergedRow1 = table.Cell(1, 1);
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            mergedRow1.Range.Text = "8.2\tTable: Product Thickness Measurement";
            mergedRow1.Range.Bold = 1;
            mergedRow1.Range.Font.Size = 10;
            mergedRow1.Range.Font.Name = "Calibri";
            mergedRow1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow1.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow1.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 2: "Tableware" ────────────────────────────────────────────────
            Cell mergedRow2 = table.Cell(2, 1);
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            mergedRow2.Range.Text = "8.2.1\tTableware";
            mergedRow2.Range.Bold = 1;
            mergedRow2.Range.Font.Size = 10;
            mergedRow2.Range.Font.Name = "Calibri";
            mergedRow2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow2.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow2.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 3: Column Headers ─────────────────────────────────────────────
            string[] headers = {
        "Group",
        "Type Of Products",
        "Measurement results\r\n(mm)",
        "Min thickness requirement\r\n(mm)",
        "Decision"
    };

            table.Rows[3].Height = wordApp.InchesToPoints(0.55f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 12;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── DATA ROWS ─────────────────────────────────────────────────────────
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length)
                        ? parsedColumns[colIndex][rowIndex].Trim()
                        : "";

                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10;
                    cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0;
                    cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                    if (colIndex == 0 || colIndex == 1)
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    else
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            int additionalInfoRow = numRows + 4;
            table.Rows[additionalInfoRow].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[additionalInfoRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            // Merge all columns
            table.Cell(additionalInfoRow, 1).Merge(table.Cell(additionalInfoRow, numCols));
            Cell additionalInfoCell = table.Cell(additionalInfoRow, 1);

            // Get value from dt where ReportParamMapID = 17960
            string additionalInfoText = "Additional information: ";
            DataRow additionalRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 17960);
            if (additionalRow != null)
                additionalInfoText += additionalRow["Result"]?.ToString() ?? "";

            additionalInfoCell.Range.Text = additionalInfoText;
            additionalInfoCell.Range.Bold = 0;
            additionalInfoCell.Range.Font.Size = 10;
            additionalInfoCell.Range.Font.Name = "Calibri";
            additionalInfoCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            additionalInfoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            additionalInfoCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            additionalInfoCell.Range.ParagraphFormat.SpaceBefore = 0;
            additionalInfoCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /* T2 Start */

        /* T3 START */
        private void CreateReportTableT3(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 18964, 0 },  // Group
        { 18961, 1 },  // Type Of Products
        { 18965, 2 },  // Measurement results (mm)
        { 18966, 3 },  // Min thickness requirement (mm)
        { 18962, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++)
                parsedColumns.Add(new string[0]);

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

            int numRows = parsedColumns.Max(col => col.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            // +3 = 2 title rows + 1 column header row
            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            float[] colWidths = { 0.80f, 1.20f, 1.20f, 1.27f, 0.80f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── ROW 1: "Table: Product Thickness Measurement" ─────────────────────

            table.Rows[1].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell mergedRow1 = table.Cell(1, 1);
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            mergedRow1.Range.Text = "8.2\tTable: Product Thickness Measurement";
            mergedRow1.Range.Bold = 1;
            mergedRow1.Range.Font.Size = 10;
            mergedRow1.Range.Font.Name = "Calibri";
            mergedRow1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow1.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow1.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 2: "Tableware" ────────────────────────────────────────────────
            Cell mergedRow2 = table.Cell(2, 1);
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            mergedRow2.Range.Text = "8.2.2\tCooking equipment";
            mergedRow2.Range.Bold = 1;
            mergedRow2.Range.Font.Size = 10;
            mergedRow2.Range.Font.Name = "Calibri";
            mergedRow2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow2.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow2.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 3: Column Headers ─────────────────────────────────────────────
            string[] headers = {
        "Group",
        "Type Of Products",
        "Measurement results\r\n(mm)",
        "Min thickness requirement\r\n(mm)",
        "Decision"
    };

            table.Rows[3].Height = wordApp.InchesToPoints(0.55f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 12;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── DATA ROWS ─────────────────────────────────────────────────────────
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length)
                        ? parsedColumns[colIndex][rowIndex].Trim()
                        : "";

                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10;
                    cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0;
                    cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                    if (colIndex == 0 || colIndex == 1)
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    else
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            int additionalInfoRow = numRows + 4;
            table.Rows[additionalInfoRow].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[additionalInfoRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            // Merge all columns
            table.Cell(additionalInfoRow, 1).Merge(table.Cell(additionalInfoRow, numCols));
            Cell additionalInfoCell = table.Cell(additionalInfoRow, 1);

            // Get value from dt where ReportParamMapID = 17960
            string additionalInfoText = "Additional information: ";
            DataRow additionalRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 18963);
            if (additionalRow != null)
                additionalInfoText += additionalRow["Result"]?.ToString() ?? "";

            additionalInfoCell.Range.Text = additionalInfoText;
            additionalInfoCell.Range.Bold = 0;
            additionalInfoCell.Range.Font.Size = 10;
            additionalInfoCell.Range.Font.Name = "Calibri";
            additionalInfoCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            additionalInfoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            additionalInfoCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            additionalInfoCell.Range.ParagraphFormat.SpaceBefore = 0;
            additionalInfoCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
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

        /*T3 end */

        /* T4 Start*/
     
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
        private void SetCellText(Cell cell, string text, bool bold, float fontSize, string fontName)
        {
            cell.Range.Text = text;
            cell.Range.Bold = bold ? 1 : 0;
            cell.Range.Font.Size = fontSize;
            cell.Range.Font.Name = fontName;
            ApplyBorders(cell);
        }
        private void ApplyBorders(Cell cell)
        {
            cell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            cell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
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
     
        private void ApplySubTableVerticalBorders(Table subTable)
        {
            // Remove all borders first
            subTable.Borders.Enable = 0;

            // Apply only vertical border between columns (right border of first cell)
            foreach (Row subRow in subTable.Rows)
            {
                if (subRow.Cells.Count >= 2)
                {
                    // Apply right border only to the first cell to create vertical line between columns
                    Cell firstCell = subRow.Cells[1];
                    firstCell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                }
            }
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

        private void CreateReportTableforcookware(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            const float DEFAULT_FONT_SIZE = 10;
            const string DEFAULT_FONT_NAME = "Calibri";
            const float HEADER_FONT_SIZE = 10;

            // Column order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
            float[] COLUMN_WIDTHS = { 0.6f, 2.50f, 0.70f, 1.17f, 1.30f };
            float SUB_COLUMN1_WIDTH = 1.5f;
            float SUB_COLUMN2_WIDTH = 1.0f;
            float TABLE_TOTAL_WIDTH = 6.27f;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // STEP 1: Count rows
            int totalRows = 1; // header row
            bool needDescHeader = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";

                if (isHeader)
                {
                    totalRows++;
                    needDescHeader = false;
                }
                else if (isDescription)
                {
                    if (!needDescHeader)
                    {
                        totalRows += 1; // sub-column header row
                        needDescHeader = true;
                    }
                    totalRows++; // actual data row
                }
                else
                {
                    totalRows++;
                    needDescHeader = false;
                }
            }

            // STEP 2: Build table with 5 main columns
            Table table = doc.Tables.Add(range, totalRows, 5);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(TABLE_TOTAL_WIDTH);

            table.Range.Font.Size = DEFAULT_FONT_SIZE;
            table.Range.Font.Name = DEFAULT_FONT_NAME;

            // Set column widths
            for (int c = 1; c <= 5; c++)
                table.Columns[c].SetWidth(wordApp.InchesToPoints(COLUMN_WIDTHS[c - 1]), WdRulerStyle.wdAdjustNone);

            // MAIN HEADER ROW
            // Col order: Clause | Test Parameters | Unit | Test Results | Quality Requirements
            string[] headers = {
        "Klausul\nClause",
        "Parameter Uji\nTest Parameters",
        "Satuan\nUnit",
        "Hasil Tes\nTest Results",
        "Persyaratan Mutu\nQuality Requirements"
    };

            for (int col = 1; col <= 5; col++)
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

            int r = 2;
            bool descHeaderShown = false;

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";

                // ── HEADER ROW ───────────────────────────────────────────────────────
                if (isHeader)
                {
                    descHeaderShown = false;

                    // Col 1: Clause Number
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Clause Title (Test Parameters)
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Col 3: Unit - empty for header
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Quality Requirements - empty for header
                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    r++;
                }
                // ── DESCRIPTION ROW ──────────────────────────────────────────────────
                else if (isDescription)
                {
                    // Sub-header row (shown once per description section)
                    if (!descHeaderShown)
                    {
                        Cell d1 = table.Cell(r, 1);
                        SetCellText(d1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 2: Sub-table header for Description | Condition
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

                        // Col 3: Unit header - empty
                        Cell d3 = table.Cell(r, 3);
                        SetCellText(d3, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 4: Result header - empty
                        Cell d4 = table.Cell(r, 4);
                        SetCellText(d4, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        // Col 5: Quality Requirements header - empty
                        Cell d5 = table.Cell(r, 5);
                        SetCellText(d5, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                        r++;
                        descHeaderShown = true;
                    }

                    // Description data row
                    Cell a1 = table.Cell(r, 1);
                    SetCellText(a1, "", false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Sub-table data for Description | Condition
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

                    // Col 3: Unit (Satuan)
                    Cell a3 = table.Cell(r, 3);
                    SetCellText(a3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell a4 = table.Cell(r, 4);
                    SetCellText(a4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Specification (Quality Requirements)
                    Cell a5 = table.Cell(r, 5);
                    SetCellText(a5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    a5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
                // ── NORMAL ROW ───────────────────────────────────────────────────────
                else
                {
                    descHeaderShown = false;

                    // Col 1: Clause Number
                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Col 2: Requirement (Parameter Uji)
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["Requirement"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Col 3: Unit (Satuan)
                    Cell c3 = table.Cell(r, 3);
                    SetCellText(c3, row["Unit"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c3.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 4: Result (Hasil Tes)
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, row["Result"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c4.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Col 5: Specification (Quality Requirements)
                    Cell c5 = table.Cell(r, 5);
                    SetCellText(c5, row["Specification"].ToString(), false, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    r++;
                }
            }
        }

        private DataTable GetReportDataFor_cookware(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (18967, 18968,18969,18960,17960)";
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

        /********************************************************************************************************************************************/

        private DataTable GetReportDataForStrength(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
                     AND ClauseID = @ClauseID
                     AND trl.ReportParamMapID IN (18967, 18968, 18969, 18970, 17958)";
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
        private void CreateReportTableStrength(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
{
    { 18967, 0 },  // Diameter
    { 18968, 1 },  // Test duration (minutes)
    { 18969, 2 },  // Test results
    { 18970, 3 }   // Decision
};


            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 4; i++)  // ── 
                parsedColumns.Add(new string[0]);

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

            int numRows = parsedColumns.Max(col => col.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 4;

            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            float[] colWidths = { 1.00f, 1.80f, 1.80f, 1.67f };  // ── FIXED: 
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── ROW 1: merged title ───────────────────────────────────────────────
            table.Rows[1].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell mergedRow1 = table.Cell(1, 1);
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            mergedRow1.Range.Text = "6.4\tTable: Strength Test";
            mergedRow1.Range.Bold = 1;
            mergedRow1.Range.Font.Size = 10;
            mergedRow1.Range.Font.Name = "Calibri";
            mergedRow1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow1.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow1.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 2: sub-title ─────────────────────────────────────────────────
            Cell mergedRow2 = table.Cell(2, 1);
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            mergedRow2.Range.Text = "6.4.1\tBottom sandwich strength test of stainless steel cookware";
            mergedRow2.Range.Bold = 1;
            mergedRow2.Range.Font.Size = 10;
            mergedRow2.Range.Font.Name = "Calibri";
            mergedRow2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            mergedRow2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            mergedRow2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            mergedRow2.Range.ParagraphFormat.SpaceBefore = 0;
            mergedRow2.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 3: column headers ─────────────────────────────────────────────
            string[] headers = {
    "Diameter",
    "Test duration (minutes)",
    "Test results",
    "Decision"
};

            table.Rows[3].Height = wordApp.InchesToPoints(0.55f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 12;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── DATA ROWS ─────────────────────────────────────────────────────────
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length)
                        ? parsedColumns[colIndex][rowIndex].Trim()
                        : "";

                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10;
                    cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0;
                    cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                    if (colIndex == 0 || colIndex == 1)
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    else
                        cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // ── ADDITIONAL INFO ROW ───────────────────────────────────────────────
            int additionalInfoRow = numRows + 4;
            table.Rows[additionalInfoRow].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[additionalInfoRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            table.Cell(additionalInfoRow, 1).Merge(table.Cell(additionalInfoRow, numCols));
            Cell additionalInfoCell = table.Cell(additionalInfoRow, 1);

            string additionalInfoText = "Additional information: ";
            DataRow additionalRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 18971);  // ──
            if (additionalRow != null)
                additionalInfoText += additionalRow["Result"]?.ToString() ?? "";

            additionalInfoCell.Range.Text = additionalInfoText;
            additionalInfoCell.Range.Bold = 0;
            additionalInfoCell.Range.Font.Size = 10;
            additionalInfoCell.Range.Font.Name = "Calibri";
            additionalInfoCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            additionalInfoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            additionalInfoCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            additionalInfoCell.Range.ParagraphFormat.SpaceBefore = 0;
            additionalInfoCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        /*******************************************************************************************************************************************/
        private DataTable GetReportDataForCoatingThickness(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
             AND ClauseID = @ClauseID
             AND trl.ReportParamMapID IN (19997, 19998, 19999, 20002, 20003, 20004, 20005, 20006, 20007)";
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
        private void CreateReportTableCoatingThickness(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19997, 0 },  // Type of material
        { 19998, 1 },  // Coating Material
        { 19999, 2 },  // Measurement 1
        { 20002, 3 },  // Measurement 2
        { 20003, 4 },  // Measurement 3
        { 20004, 5 },  // Average
        { 20005, 6 },  // Min layer thickness
        { 20006, 7 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 8; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 8;

            // rows: 1 section header + 2 header rows + numRows data + 1 additional info
            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 0.70f, 0.70f, 0.65f, 0.65f, 0.65f, 0.65f, 0.77f, 0.70f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // ── STEP 1: Merge additional info row FIRST ───────────────────────
            int addRow = numRows + 4;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));

            // ── STEP 2: Merge section header row ─────────────────────────────
            table.Cell(1, 1).Merge(table.Cell(1, numCols));

            // ── ROW 1: Section header content ────────────────────────────────
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.8\tCoating Thickness Test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // ── ROW 2: Top header row  ────────────────────────────
            table.Rows[2].Height = wordApp.InchesToPoints(0.35f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            string[] headers2 = {
        "Type of\r\nmaterial",
        "Coating\r\nMaterial",
        "Results of measuring layer thickness (\u00b5)",
        "",
        "",
        "",
        "Min layer\r\nthickness (\u00b5)",
        "Decision"
    };
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers2[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── ROW 3: Sub-header row (no merges) ────────────────────────────
            table.Rows[3].Height = wordApp.InchesToPoints(0.40f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            string[] headers3 = {
        "",
        "",
        "Measurement\r\n1",
        "Measurement\r\n2",
        "Measurement\r\n3",
        "Average",
        "",
        ""
    };
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers3[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // ── DATA ROWS (start at row 4) ────────────────────────────────────
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length)
                        ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = (colIndex < 2)
                        ? WdParagraphAlignment.wdAlignParagraphLeft
                        : WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // ── ADDITIONAL INFO ROW content  ───────
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 20007);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /*******************************************************************************************************************************************/
        private DataTable GetReportDataForThermalShock(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
        INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
        INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
        WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
        AND ClauseID = @ClauseID
        AND trl.ReportParamMapID IN (19992, 19993, 19994, 19995, 19996)";
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

        private void CreateReportTableThermalShock(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19992, 0 },  // Test Temperature 1
        { 19993, 1 },  // Test Temperature 2
        { 19994, 2 },  // Test results
        { 19995, 3 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 4; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 4;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.50f, 1.50f, 1.77f, 1.50f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.7\tThermal shock test for enameled cookware";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = { "Test Temperature 1", "Test Temperature 2", "Test results", "Decision" };
            table.Rows[2].Height = wordApp.InchesToPoints(0.40f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19996);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        /*******************************************************************************************************************************************/

        private DataTable GetReportDataForNonStickCoating(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
          INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
          INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
          WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
          AND ClauseID = @ClauseID
          AND trl.ReportParamMapID IN (19986, 19987, 19988, 19989, 19990, 19991)";
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

        private void CreateReportTableNonStickCoating(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19986, 0 },  // Soaking duration (minutes)
        { 19987, 1 },  // Number of scratches
        { 19988, 2 },  // Stroke distance
        { 19989, 3 },  // Test results
        { 19990, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.40f, 1.20f, 1.20f, 1.47f, 1.00f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.6\tNon-stick coating adhesion test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = {
        "Soaking duration\r\n(minutes)", "Number of\r\nscratches",
        "Stroke\r\ndistance", "Test results", "Decision"
    };
            table.Rows[2].Height = wordApp.InchesToPoints(0.50f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19991);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /*******************************************************************************************************************************************/

        private DataTable GetReportDataForFatigueResistance(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
          INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
          INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
          WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
          AND ClauseID = @ClauseID
          AND trl.ReportParamMapID IN (19980, 19981, 19982, 19983, 19984, 19985)";
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

        private void CreateReportTableFatigueResistance(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19980, 0 },  // Water volume
        { 19981, 1 },  // 5 x 1.5 Water volume (V)
        { 19982, 2 },  // Number of cycles 15,000 times
        { 19983, 3 },  // Test results
        { 19985, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.10f, 1.30f, 1.40f, 1.47f, 1.00f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.4.2\tFatigue resistance test on handle strength";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = {
        "Water volume", "5 x 1.5 Water\r\nvolume (V)",
        "Number of cycles\r\n15,000 times", "Test results", "Decision"
    };
            table.Rows[2].Height = wordApp.InchesToPoints(0.50f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19984);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }

        /*******************************************************************************************************************************************/

        private DataTable GetReportDataForBaseMaterial(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
         INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
         INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
         WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
         AND ClauseID = @ClauseID
         AND trl.ReportParamMapID IN (19973, 19974, 19975, 19976, 19977, 19978, 19979)";
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

        private void CreateReportTableBaseMaterial(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19973, 0 },  // 0° angle
        { 19974, 1 },  // 90° angle
        { 19975, 2 },  // 180° angle
        { 19976, 3 },  // 270° angle
        { 19977, 4 },  // Average
        { 19978, 5 },  // Thickness(t) Min(mm)
        { 19979, 6 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 7; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 7;

            Table table = doc.Tables.Add(tableRange, numRows + 3, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.87f, 0.40f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.3\tBase material thickness test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Column headers
            string[] headers = {
        "0° angle\r\npoint", "90° angle\r\npoint", "180° angle\r\npoint", "270° angle\r\npoint",
        "Average", "Thickness (t)\r\nMin (mm)", "Decision"
    };
            table.Rows[2].Height = wordApp.InchesToPoints(0.50f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(2, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 3].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 3, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 3;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            addCell.Range.Text = "Additional information: ";
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /*******************************************************************************************************************************************/

        private DataTable GetReportDataForFormTest(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM TestResultsLED trl
        INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
        INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
        WHERE SampleID = @SampleID AND trl.ReportID = @ReportID 
        AND ClauseID = @ClauseID
        AND trl.ReportParamMapID IN (19967, 19968, 19969, 19970, 19971, 19972)";
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

        private void CreateReportTableFormTest(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found.");

            Range tableRange = range;
            tableRange.Text = "";

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
    {
        { 19967, 0 },  // D1
        { 19968, 1 },  // D2
        { 19969, 2 },  // Roundness value
        { 19970, 3 },  // Roundness value tolerance Max 2%
        { 19971, 4 },  // Decision
    };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 5; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(id))
                    parsedColumns[columnMapping[id]] = row["Result"].ToString().Split(',');
            }

            int numRows = parsedColumns.Max(c => c.Length);
            if (numRows == 0) numRows = 1;
            int numCols = 5;

            Table table = doc.Tables.Add(tableRange, numRows + 4, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.27f);

            float[] colWidths = { 1.00f, 1.00f, 1.27f, 1.80f, 1.20f };
            for (int i = 0; i < numCols; i++)
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);

            // Row 1: Section header
            table.Rows[1].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(1, 1).Merge(table.Cell(1, numCols));
            Cell r1 = table.Cell(1, 1);
            r1.Range.Text = "6.2\tForm test";
            r1.Range.Bold = 1; r1.Range.Font.Size = 10; r1.Range.Font.Name = "Calibri";
            r1.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r1.Range.ParagraphFormat.SpaceBefore = 0; r1.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 2: Sub-title
            table.Rows[2].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[2].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(2, 1).Merge(table.Cell(2, numCols));
            Cell r2 = table.Cell(2, 1);
            r2.Range.Text = "6.2.1\tMeasuring round cookware";
            r2.Range.Bold = 1; r2.Range.Font.Size = 10; r2.Range.Font.Name = "Calibri";
            r2.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            r2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            r2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            r2.Range.ParagraphFormat.SpaceBefore = 0; r2.Range.ParagraphFormat.SpaceAfter = 0;

            // Row 3: Column headers
            string[] headers = { "D1", "D2", "Roundness value", "Roundness value tolerance\r\nMax 2%", "Decision" };
            table.Rows[3].Height = wordApp.InchesToPoints(0.45f);
            table.Rows[3].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(3, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1; cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 4].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 4].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 4, colIndex + 1);
                    cell.Range.Text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Font.Size = 10; cell.Range.Font.Name = "Calibri";
                    cell.Range.ParagraphFormat.SpaceBefore = 0; cell.Range.ParagraphFormat.SpaceAfter = 0;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }

            // Additional info row
            int addRow = numRows + 4;
            table.Rows[addRow].Height = wordApp.InchesToPoints(0.30f);
            table.Rows[addRow].HeightRule = WdRowHeightRule.wdRowHeightExactly;
            table.Cell(addRow, 1).Merge(table.Cell(addRow, numCols));
            Cell addCell = table.Cell(addRow, 1);
            string addText = "Additional information: ";
            DataRow addDataRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["ReportParamMapID"]) == 19972);
            if (addDataRow != null) addText += addDataRow["Result"]?.ToString() ?? "";
            addCell.Range.Text = addText;
            addCell.Range.Font.Size = 10; addCell.Range.Font.Name = "Calibri";
            addCell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            addCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            addCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            addCell.Range.ParagraphFormat.SpaceBefore = 0; addCell.Range.ParagraphFormat.SpaceAfter = 0;

            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
    }
}