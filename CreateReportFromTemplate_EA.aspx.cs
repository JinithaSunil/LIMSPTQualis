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
using System.Web.UI;
using ZXing;
using DataTable = System.Data.DataTable;
using Table = Microsoft.Office.Interop.Word.Table;

namespace LIMS
{
    public partial class CreateReportFromTemplate_EA : System.Web.UI.Page
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
                if (Request.QueryString["t"] != null)
                    t = Request.QueryString["t"].ToString();

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

                DataTable dtCloneCRFSampleIDGet = _cm.CRFSampleDetails_LED(ID);
                if (dtCloneCRFSampleIDGet != null && dtCloneCRFSampleIDGet.Rows.Count > 0)
                {
                    foreach (DataRow row in dtCloneCRFSampleIDGet.Rows)
                    {
                        cloneCRFSampleID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["CloneCRFSampleID"].ToString());
                        reportID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["ReportID"].ToString());
                        templatePath = path + "EA_ReportTemplate_1.docx";
                        b1 = true;                        
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

                DataTable LEDdetails = _cm.EA_DetailsGet(sampleID);
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
                    FindAndReplace(wordApp, "{RequestNo}", GetSafeValue("RequestNo"));

                    FindAndReplace(wordApp, "{ClientName}", GetSafeValue("Attention"));
                    FindAndReplace(wordApp, "{ClientAddress}", GetSafeValue("Address"));
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

                    FindAndReplace(wordApp, "{ProductReceptionDate}", GetSafeDate("IssueDate"));
                    FindAndReplace(wordApp, "{DateOfTesting}", GetSafeDate("TestPeriod"));

                    ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
                    FindAndReplace(wordApp, "{TestReportNumber}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{DocNo}", "TSQ-LAB-QAF-086");

                    FindAndReplace(wordApp, "{Rating}", GetSafeValue("Ranking"));
                    FindAndReplace(wordApp, "{Rating1}", GetSafeValue("Ranking"));
                    FindAndReplace(wordApp, "{TestMethod}", GetSafeValue("TestMethod"));
                    FindAndReplace(wordApp, "{TestCategory}", GetSafeValue("TestCategory"));
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
                        //-- |Result Data|--//
                        DataTable dtReport = GetReportData(ID, reportID);
                        CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                        int departmentId = 3006;
                        List<string> equipmentNames = new List<string>();
                        DataTable dtEquipment = GetEquipmentNamesDepartmentWise(departmentId);
                        if (dtEquipment != null && dtEquipment.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtEquipment.Rows)
                            {
                                equipmentNames.Add(row["Equipment"].ToString());
                            }
                        }
                        CreateReportTableT5(wordApp, doc, "{MeasuringEquipment}", equipmentNames);

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
                /*Force - kill any lingering WINWORD processes started by this user session for locally server */
                foreach (var process in Process.GetProcessesByName("WINWORD"))
                {
                    if (string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        process.Kill();
                    }
                }
            }
        }

        public DataTable GetEquipmentNamesDepartmentWise(int departmentId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM Calibration_Equipment WHERE DepartmentId = @DepartmentId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentId", departmentId);
                    try
                    {
                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }
            return dt;
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

        /*Sample images*/
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
                    int imagesPerRow = 2;
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
                                        cell.Range.Font.Size = 10;
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
                            {// Empty cell
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
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
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

            string query = @"Select * from TestResultsLED trl
                             INNER JOIN Clauses c ON c.ClauseID = trl.ClauseID
                             WHERE SampleID = @sampleID and Result != 'TB' order by Report_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", sampleID);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }


        /* T1 Start for Result Show */

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
                    needDiagramHeader = false; // RESET when header encountered
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
                    needDiagramHeader = false; // RESET when description encountered
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
                    needDiagramHeader = false; // RESET when checkup result encountered
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
                    needDiagramHeader = false; // RESET when regular row encountered
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
                    diagramHeaderShown = false; // RESET FLAG

                    Cell c1 = table.Cell(r, 1);
                    SetCellText(c1, row["ClauseNumber"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    Cell c2 = table.Cell(r, 2);
                    SetCellText(c2, row["ClauseTitle"].ToString(), true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
                    c2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    string headerResult = GetActualResult(row);
                    Cell c4 = table.Cell(r, 4);
                    SetCellText(c4, headerResult, true, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);

                    // Merge col 2 & 3 for header area (only if distinct cells exist)
                    if (r <= table.Rows.Count)
                        table.Cell(r, 2).Merge(table.Cell(r, 3));

                    r++;
                }
                else if (isCheckUpResult)
                {
                    diagramHeaderShown = false; // RESET FLAG

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
                    // ADD DIAGRAM HEADER BEFORE FIRST DIAGRAM
                    if (!diagramHeaderShown)
                    {
                        CreateDiagramStaticHeader(table, r, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_NAME);
                        r++; // Move to next row
                        diagramHeaderShown = true;
                    }

                    // collect consecutive diagram rows with same FileName (or empty)
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
                    idx += (diagramGroup.Count - 1); // skip processed
                }
                else if (isDescription)
                {
                    diagramHeaderShown = false; // RESET FLAG

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
                    diagramHeaderShown = false; // RESET FLAG

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

            // flush any remaining checkup group
            if (currentGroup.Count > 0)
            {
                CreateCheckUpSection(table, r, currentGroup, groupType, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
            }
        }

        private void CreateReportTable_old(Application wordApp, Document doc, string placeholder, DataTable dt)
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
                // ========== CHANGE 2: ADD DIAGRAM ROW COUNT WITH HEADER ==========
                else if (isDiagram)
                {
                    if (!needDiagramHeader)
                    {
                        totalRows++; // Add one row for diagram header
                        needDiagramHeader = true;
                    }
                    totalRows++; // Add row for diagram data
                }
                // =================================================================
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

            // ========== CHANGE 3: ADD THIS FLAG FOR SHOWING DIAGRAM HEADER ==========
            bool diagramHeaderShown = false;
            // ========================================================================

            for (int idx = 0; idx < dt.Rows.Count; idx++)
            {
                DataRow row = dt.Rows[idx];
                bool isHeader = row["isHeader"].ToString() == "1";
                bool isDescription = row["isDescription"].ToString() == "1";
                bool isCheckUpResult = row["isCheckUpResult"].ToString() == "1";
                bool isDiagram = row.Table.Columns.Contains("isDiagram") && row["isDiagram"].ToString() == "1";
                string requirement = row["Requirement"]?.ToString() ?? "";

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

                    // Merge col 2 & 3 for header area (only if distinct cells exist)
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
                    // ========== CHANGE 4: ADD DIAGRAM HEADER BEFORE FIRST DIAGRAM ==========
                    if (!diagramHeaderShown)
                    {
                        CreateDiagramStaticHeader(table, r, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_NAME);
                        r++; // Move to next row
                        diagramHeaderShown = true;
                    }
                    // =======================================================================

                    // collect consecutive diagram rows with same FileName (or empty)
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
                    idx += (diagramGroup.Count - 1); // skip processed
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

            // flush any remaining checkup group
            if (currentGroup.Count > 0)
            {
                CreateCheckUpSection(table, r, currentGroup, groupType, wordApp, COLUMN_WIDTHS, DEFAULT_FONT_SIZE, DEFAULT_FONT_NAME);
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

                // FIX: Added check for "Number of switch specimens"
                if (requirement.Contains("Number of screwless terminals") ||
                    requirement.Contains("Voltage drop measured after") ||
                    requirement.Contains("Voltage drop is measured after"))
                {
                    columnCount = 5;
                    columnHeaders = new string[] { "1", "2", "3", "4", "5" };
                    thresholdValues = new string[] { "-", "-", "-", "-", "-" };
                }
                // NEW: Handle "Number of switch specimens"
                else if (requirement.Contains("Number of switch specimens"))
                {
                    columnCount = 3;
                    columnHeaders = new string[] { "1", "2", "3" };
                    thresholdValues = new string[] { "-", "-", "-" };
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

        private void CreateCheckUpSection(Table table, int startRow, List<DataRow> checkUpRows, string groupType, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            if (checkUpRows.Count == 0) return;

            // Check if this is a "Number of switch specimens" group
            bool hasSwitchSpecimens = checkUpRows.Any(row =>
                (row["Requirement"]?.ToString() ?? "").Contains("Number of switch specimens"));

            if (hasSwitchSpecimens)
            {
                CreateSwitchSpecimensSection(table, startRow, checkUpRows, wordApp, columnWidths, fontSize, fontName);
            }
            else if (groupType == "SCREWLESS_TERMINALS")
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

        private void CreateSwitchSpecimensSection(Table table, int startRow, List<DataRow> checkUpRows, Application wordApp, float[] columnWidths, float fontSize, string fontName)
        {
            int columnCount = 3;
            string[] columnHeaders = { "1", "2", "3" };
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

                // Parse pipe-separated values from Specification
                string[] specValues = ParsePipeSeparatedValues(row["Specification"]?.ToString() ?? "", columnCount);

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
                    string[] displayValues = new string[columnCount];
                    for (int j = 0; j < columnCount; j++)
                    {
                        string spec = specValues[j];
                        displayValues[j] = !string.IsNullOrEmpty(spec) ? spec : "-";
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

        /*T1 End */


        /* T5 Start for equipments show */
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

        /*T5 End*/

        

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