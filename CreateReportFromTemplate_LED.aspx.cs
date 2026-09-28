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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using ZXing;
using A = DocumentFormat.OpenXml.Drawing;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using DataTable = System.Data.DataTable;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace LIMS
{
    public partial class CreateReportFromTemplate_LED : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();

        // A4 page layout constants (same convention used across the migrated report pages)
        private const int A4_PAGE_WIDTH_TWIPS = 12240;
        private const int LEFT_MARGIN_TWIPS = 1440;
        private const int RIGHT_MARGIN_TWIPS = 1440;
        private const int USABLE_WIDTH_TWIPS = A4_PAGE_WIDTH_TWIPS - LEFT_MARGIN_TWIPS - RIGHT_MARGIN_TWIPS;

        // Unique id seed for embedded Drawing/docPr elements
        private int _drawingIdCounter = 1000;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string licenseKey = "Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTc0RnWX5cd3FcTmBdV091XA==";
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

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
                if (Request.QueryString["s"] != null) s = Request.QueryString["s"].ToString();
                _cm.sampleId = Convert.ToInt32(s);
                if (Request.QueryString["c"] != null) c = Request.QueryString["c"].ToString();
                if (Request.QueryString["t"] != null) t = Request.QueryString["t"].ToString();

                int ID = Convert.ToInt32(s);
                string SampleUID = "";
                DataTable dt = _cm.getSampleUID(ID);

                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    GenerateQR(SampleUID);
                }

                int cloneCRFSampleID = 0;
                int reportID = 0;
                bool b1 = false; bool b2 = false;
                string templatePath = "";

                DataTable dtCloneCRFSampleIDGet = _cm.CRFSampleDetails_LED(ID);
                if (dtCloneCRFSampleIDGet != null && dtCloneCRFSampleIDGet.Rows.Count > 0)
                {
                    foreach (DataRow row in dtCloneCRFSampleIDGet.Rows)
                    {
                        cloneCRFSampleID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["CloneCRFSampleID"].ToString());
                        reportID = Convert.ToInt32(dtCloneCRFSampleIDGet.Rows[0]["ReportID"].ToString());

                        if (reportID == 2212)
                        {
                            templatePath = path + "LED_ReportTemplate_1.docx";
                            b1 = true;
                        }

                        if (reportID == 2213)
                        {
                            templatePath = path + "LED_ReportTemplate_2.docx";
                            b2 = true;
                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'GenerateReport.aspx'; }, 10);</script>");
                    return;
                }

                DataTable LEDdetails = _cm.getLEDdetails(sampleID);
                if (LEDdetails == null || LEDdetails.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                    return;
                }

                bool sampleDescriptionSet = true;
                if (!sampleDescriptionSet) return;

                string uniqueFileName = SampleUID;
                string outputDocxPath = pathout + "TestReport-" + uniqueFileName + ".docx";
                string outputPdfPath = pathout + "TestReport-" + uniqueFileName + ".pdf";

                byte[] templateBytes = File.ReadAllBytes(templatePath);
                byte[] processedBytes = ProcessDocument(templateBytes, ID, sampleID, reportID, SampleUID, LEDdetails.Rows[0]);

                string directory = Path.GetDirectoryName(outputPdfPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(outputDocxPath, processedBytes);

                byte[] pdfBytes = ConvertDocxToPdfSyncfusion(processedBytes);
                File.WriteAllBytes(outputPdfPath, pdfBytes);

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=TestReport-" + uniqueFileName + ".pdf");
                Response.TransmitFile(outputPdfPath);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        /// <summary>
        /// Builds the finished DOCX in memory: QR code image, placeholder replacement (body +
        /// header/footer), product image gallery, and tables T1-T6. Returns the processed bytes
        /// ready to be written to disk / converted to PDF.
        /// </summary>
        private byte[] ProcessDocument(byte[] templateBytes, int ID, int sampleID, int reportID, string SampleUID, DataRow ledRow)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(templateBytes, 0, templateBytes.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    // QR code
                    string qrImagePath = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                    InsertSingleImageOpenXML(wordDoc, "{QR_Code}", qrImagePath, 0.9, 0.9);
                    InsertSingleImageOpenXML(wordDoc, "{QR_Code1}", qrImagePath, 1.0, 1.0);

                    // Header field placeholders
                    FindAndReplaceOpenXML(wordDoc, "{LabHeadName}", GetSafeValue(ledRow, "LabHeadName"));
                    FindAndReplaceOpenXML(wordDoc, "{WorkOrederNumber}", GetSafeValue(ledRow, "WorkorderNumber"));
                    FindAndReplaceOpenXML(wordDoc, "{ReportNumber}", GetSafeValue(ledRow, "TestReportNo"));
                    FindAndReplaceOpenXML(wordDoc, "{CompanyName}", GetSafeValue(ledRow, "Applicant"));
                    FindAndReplaceOpenXML(wordDoc, "{CompanyAddress}", GetSafeValue(ledRow, "Address"));
                    FindAndReplaceOpenXML(wordDoc, "{ManufacturerName}", GetSafeValue(ledRow, "FactoryName"));
                    FindAndReplaceOpenXML(wordDoc, "{ManufacturerAddress}", GetSafeValue(ledRow, "FactoryAddress"));
                    FindAndReplaceOpenXML(wordDoc, "{LetterNumber}", GetSafeValue(ledRow, "LotNumber"));
                    /* FindAndReplaceOpenXML(wordDoc, "{TestMethod}", GetSafeValue(ledRow, "TestMethod")); */

                    if (reportID == 2213)
                    {
                        FindAndReplaceOpenXML(wordDoc, "{CertificateNo}", GetSafeValue(ledRow, "TestReportNo"));
                        FindAndReplaceOpenXML(wordDoc, "{TestedFor}", GetSafeValue(ledRow, "Applicant"));
                        FindAndReplaceOpenXML(wordDoc, "{Address}", GetSafeValue(ledRow, "Address"));
                        FindAndReplaceOpenXML(wordDoc, "{Brand}", GetSafeValue(ledRow, "BrandProduct"));
                        FindAndReplaceOpenXML(wordDoc, "{Type}", GetSafeValue(ledRow, "MaterialType"));
                        FindAndReplaceOpenXML(wordDoc, "{SPECIFICATION}", GetSafeValue(ledRow, "ProductSpecification"));
                        FindAndReplaceOpenXML(wordDoc, "{IssuedDate}", GetSafeDate(ledRow, "IssueDate"));
                        FindAndReplaceOpenXML(wordDoc, "{ValidUntil}", GetSafeDate(ledRow, "DateOfExp"));

                        FindAndReplaceOpenXML(wordDoc, "{DateOfTest}", GetSafeDate(ledRow, "TestPeriod"));
                        FindAndReplaceOpenXML(wordDoc, "{TestStandard}", GetSafeValue(ledRow, "TestStandard"));
                        FindAndReplaceOpenXML(wordDoc, "{TestingObject}", GetSafeValue(ledRow, "ProductName"));
                    }

                    FindAndReplaceOpenXML(wordDoc, "{ProductName}", GetSafeValue(ledRow, "ProductName"));
                    FindAndReplaceOpenXML(wordDoc, "{ProductBrand}", GetSafeValue(ledRow, "BrandProduct"));
                    FindAndReplaceOpenXML(wordDoc, "{ProductType}", GetSafeValue(ledRow, "MaterialType"));
                    FindAndReplaceOpenXML(wordDoc, "{ProductSpecification}", GetSafeValue(ledRow, "ProductSpecification"));
                    FindAndReplaceOpenXML(wordDoc, "{ProductColor}", GetSafeValue(ledRow, "Color"));
                    FindAndReplaceOpenXML(wordDoc, "{ProductSerialNumber}", GetSafeValue(ledRow, "ProductSerialNo"));
                    FindAndReplaceOpenXML(wordDoc, "{ProductQuantity}", GetSafeValue(ledRow, "ProductQuantity"));

                    FindAndReplaceOpenXML(wordDoc, "{ProductReceptionDate}", GetSafeDate(ledRow, "IssueDate"));
                    FindAndReplaceOpenXML(wordDoc, "{DateOfTesting}", GetSafeDate(ledRow, "TestPeriod"));

                    // {TestReportNo} appears in the header/footer; {TestReportNo.} appears in the body.
                    // FindAndReplaceOpenXML already walks body + all headers/footers, so a single call covers both.
                    FindAndReplaceOpenXML(wordDoc, "{TestReportNo.}", GetSafeValue(ledRow, "TestReportNo"));
                    FindAndReplaceOpenXML(wordDoc, "{TestReportNo}", GetSafeValue(ledRow, "TestReportNo"));
                    FindAndReplaceOpenXML(wordDoc, "{DocNo}", "TSQ-LAB-QAF-098");

                    // Product image gallery
                    DataTable dt3 = _cm.getLEDdetailsForImages(ID);
                    List<Tuple<string, string>> imageData = new List<Tuple<string, string>>();
                    if (dt3 != null)
                    {
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
                            }
                        }
                    }
                    InsertMultipleImagesOpenXML(wordDoc, "{images}", imageData);

                    // Tables
                    if (reportID == 2213)
                    {
                        List<string> equipmentNames = new List<string>();
                        DataTable dtEquipment = _cm.GetEquipmentNamesRowWise(ID, reportID);
                        if (dtEquipment != null && dtEquipment.Rows.Count > 0)
                        {
                            equipmentNames = dtEquipment.AsEnumerable().Select(row => row["Equipment"].ToString()).ToList();
                        }
                        CreateReportTableT5OpenXML(wordDoc, "{MeasuringEquipment}", equipmentNames);

                        DataTable dtReportT6 = _cm.GetConclusionRow_LED(ID, reportID);
                        CreateReportTableT6OpenXML(wordDoc, "{Overall Conclusion}", dtReportT6);

                        /* {TestingAndMeasurement} */
                        DataTable dtReport = GetReportData(ID, reportID);
                        CreateReportTableOpenXML(wordDoc, dtReport);
                    }
                    else
                    {
                        List<string> equipmentNames = new List<string>();
                        DataTable dtEquipment = _cm.GetEquipmentNamesRowWise(ID, reportID);
                        if (dtEquipment != null && dtEquipment.Rows.Count > 0)
                        {
                            equipmentNames = dtEquipment.AsEnumerable().Select(row => row["Equipment"].ToString()).ToList();
                        }
                        CreateReportTableT5OpenXML(wordDoc, "{MeasuringEquipment}", equipmentNames);
                            
                        DataTable dtReportT6 = _cm.GetConclusionRow_LED(ID, reportID);
                        CreateReportTableT6OpenXML(wordDoc, "{Overall Conclusion}", dtReportT6);

                        /* {TestingAndMeasurement} */
                        DataTable dtReport = GetReportData(ID, reportID);
                        CreateReportTableOpenXML(wordDoc, dtReport);

                        /* {LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER} */
                        DataTable dtReportT2 = GetReportDataForT2(ID, reportID);
                        CreateReportTableT2OpenXML(wordDoc, dtReportT2);

                        /* {LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS} */
                        DataTable dtReportT3 = GetReportDataForT2(ID, reportID);
                        CreateReportTableT3OpenXML(wordDoc, dtReportT3);

                        /* APPENDIX C – TERMINAL  // {LAMPIRAN_C-TERMINAL} // */
                        DataTable dtReport_C = GetReportDataRefer(ID, reportID);
                        CreateReportTableOpenXML_Appendix_C(wordDoc, dtReport_C);

                        /* {LAMPIRAN_D-UJI_TERMAL} */
                        DataTable dtReportT4 = GetReportDataForT2(ID, reportID);
                        CreateReportTableT4OpenXML(wordDoc, dtReportT4);

                    }
                    CleanupUnresolvedPlaceholders(wordDoc);
                    wordDoc.MainDocumentPart.Document.Save();
                }
                return stream.ToArray();
            }
        }

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

        protected void GenerateQR(string SampleID)
        {
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string QRCodeFilePath = baseUrl + "/TestReport/TestReport-" + SampleID + ".pdf";
            string QRCodeFileName = SampleID + ".png";

            string barcodeFolderPath = Server.MapPath("~/TestReport/");
            if (!Directory.Exists(barcodeFolderPath))
            {
                Directory.CreateDirectory(barcodeFolderPath);
            }

            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            Bitmap barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);
            string barcodeFileName = QRCodeFileName;
            string barcodeFilePath = Path.Combine(barcodeFolderPath, barcodeFileName);
            barcodeBitmap.Save(barcodeFilePath);
        }

        #region Safe value helpers

        private string GetSafeValue(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return "";
            return row[columnName] == DBNull.Value || row[columnName] == null ? "" : row[columnName].ToString();
        }

        private string GetSafeDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return "";
            if (row[columnName] == DBNull.Value || row[columnName] == null) return "";
            DateTime parsed;
            if (DateTime.TryParse(row[columnName].ToString(), out parsed))
                return parsed.ToString("dd MMMM yyyy");
            return "";
        }

        #endregion

        #region OpenXML core helpers (find/replace, header/footer, cleanup)

        private void FindAndReplaceOpenXML(WordprocessingDocument doc, string findText, string replaceText)
        {
            if (string.IsNullOrEmpty(findText)) return;
            if (replaceText == null) replaceText = "";

            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            ReplaceAllInElementOpenXML(mainPart.Document.Body, findText, replaceText);

            foreach (var headerPart in mainPart.HeaderParts)
            {
                if (headerPart.Header != null)
                    ReplaceAllInElementOpenXML(headerPart.Header, findText, replaceText);
            }

            foreach (var footerPart in mainPart.FooterParts)
            {
                if (footerPart.Footer != null)
                    ReplaceAllInElementOpenXML(footerPart.Footer, findText, replaceText);
            }
        }

        private void ReplaceAllInElementOpenXML(OpenXmlElement element, string findText, string replaceText)
        {
            foreach (var paragraph in element.Descendants<Paragraph>().ToList())
            {
                ReplaceInParagraphOpenXML(paragraph, findText, replaceText);
            }
        }

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

        private string GetParagraphFullText(Paragraph paragraph)
        {
            return string.Join("", paragraph.Descendants<WordText>().Select(t => t.Text));
        }

        private Paragraph FindParagraphContaining(OpenXmlElement root, string placeholder)
        {
            foreach (var para in root.Descendants<Paragraph>())
            {
                if (GetParagraphFullText(para).Contains(placeholder))
                    return para;
            }
            return null;
        }

        private void CleanupUnresolvedPlaceholders(WordprocessingDocument doc)
        {
            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            var regex = new System.Text.RegularExpressions.Regex(@"\{[A-Za-z][A-Za-z0-9_.\*:'/ ]{0,60}\}");

            void CleanElement(OpenXmlElement element)
            {
                foreach (var paragraph in element.Descendants<Paragraph>().ToList())
                {
                    string fullText = GetParagraphFullText(paragraph);
                    if (string.IsNullOrEmpty(fullText)) continue;

                    var matches = regex.Matches(fullText);
                    foreach (System.Text.RegularExpressions.Match m in matches)
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

        #endregion

        #region Image embedding (replaces InsertImagesIntoDocument / InsertImagesIntoDocument1)

        private PartTypeInfo GetImagePartTypeFromExtension(string filePath)
        {
            switch (Path.GetExtension(filePath).ToLowerInvariant())
            {
                case ".png": return ImagePartType.Png;
                case ".jpg":
                case ".jpeg": return ImagePartType.Jpeg;
                case ".gif": return ImagePartType.Gif;
                case ".bmp": return ImagePartType.Bmp;
                default: return ImagePartType.Png;
            }
        }

        /// <summary>
        /// Embeds an image file into the document and returns a ready-to-append Drawing element.
        /// The checkbox-glyph and bilingual-run fixes taught us to keep each visual element
        /// self-contained; this does the same for pictures instead of text runs.
        /// </summary>
        private Drawing CreateImageDrawing(MainDocumentPart mainPart, string imagePath, double widthInches, double heightInches, string imageDescription)
        {
            PartTypeInfo imageType = GetImagePartTypeFromExtension(imagePath);
            ImagePart imagePart = mainPart.AddImagePart(imageType);
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                imagePart.FeedData(fs);
            }
            string relationshipId = mainPart.GetIdOfPart(imagePart);

            long widthEmu = (long)(widthInches * 914400);
            long heightEmu = (long)(heightInches * 914400);
            uint id = (uint)_drawingIdCounter++;

            return new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = widthEmu, Cy = heightEmu },
                    new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                    new DW.DocProperties() { Id = id, Name = "Picture " + id, Description = imageDescription ?? "" },
                    new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties() { Id = 0U, Name = "Picture " + id },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip() { Embed = relationshipId },
                                    new A.Stretch(new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = widthEmu, Cy = heightEmu }),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                    )
                )
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U
                }
            );
        }

        /// <summary>
        /// Replaces a single placeholder (e.g. {QR_Code}) with one centered image.
        /// </summary>
        private void InsertSingleImageOpenXML(WordprocessingDocument doc, string placeholder, string imagePath, double widthInches, double heightInches)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            ReplaceInParagraphOpenXML(targetParagraph, placeholder, "");

            if (!File.Exists(imagePath)) return;

            Run imageRun = new Run();
            imageRun.AppendChild(CreateImageDrawing(mainPart, imagePath, widthInches, heightInches, "QRCode"));
            targetParagraph.AppendChild(imageRun);

            if (targetParagraph.ParagraphProperties == null)
                targetParagraph.ParagraphProperties = new ParagraphProperties();
            targetParagraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
        }

        /// <summary>
        /// Replaces a placeholder (e.g. {images}) with a one-column table of product photos,
        /// each followed by its caption -- the OpenXML equivalent of InsertImagesIntoDocument1.
        /// </summary>
        private void InsertMultipleImagesOpenXML(WordprocessingDocument doc, string placeholder, List<Tuple<string, string>> imageData, double imageWidthInches = 2.2, double imageHeightInches = 2.2)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            ReplaceInParagraphOpenXML(targetParagraph, placeholder, "");

            if (imageData == null || imageData.Count == 0) return;

            Table table = new Table();
            table.AppendChild(new TableProperties(
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
            ));

            TableGrid grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = USABLE_WIDTH_TWIPS.ToString() });
            table.AppendChild(grid);

            foreach (var item in imageData)
            {
                string imagePath = item.Item1;
                string imageName = item.Item2;

                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new TableCellMargin(
                        new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                        new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                        new DocumentFormat.OpenXml.Wordprocessing.LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                        new DocumentFormat.OpenXml.Wordprocessing.RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa })
                ));

                Paragraph imgPara = new Paragraph();
                imgPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                if (File.Exists(imagePath))
                {
                    Run imgRun = new Run();
                    imgRun.AppendChild(CreateImageDrawing(mainPart, imagePath, imageWidthInches, imageHeightInches, imageName));
                    imgPara.AppendChild(imgRun);
                }
                else
                {
                    Run missingRun = new Run();
                    missingRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    missingRun.AppendChild(new WordText() { Text = $"Missing: {imageName}", Space = SpaceProcessingModeValues.Preserve });
                    imgPara.AppendChild(missingRun);
                }
                cell.AppendChild(imgPara);

                Paragraph captionPara = new Paragraph();
                captionPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run captionRun = new Run();
                captionRun.AppendChild(new RunProperties(new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" }, new FontSize() { Val = "32" }));
                captionRun.AppendChild(new WordText() { Text = imageName, Space = SpaceProcessingModeValues.Preserve });
                captionPara.AppendChild(captionRun);
                cell.AppendChild(captionPara);

                row.AppendChild(cell);
                table.AppendChild(row);
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        #endregion

        #region Data access (unchanged from the Interop version)

        private DataTable GetReportData(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT c.ClauseID, c.ClauseNumber, c.ClauseTitle, c.ClauseTitle_BH, c.Requirement, c.Requirement_BH,
                                c.ResultOptions, c.ResultOptions_BH, c.isHeader, c.standard_ID,
                                ISNULL(t.Result, '') as Results, ISNULL(t.Remark, '') as Remark, ISNULL(t.InputType, '') as SavedInputType
                            FROM Clauses c
                            inner JOIN TestResultsLED t ON c.ClauseID = t.ClauseID AND t.SampleID = @SampleID AND t.ReportID = @ReportID AND 
                                                          (c.isRefre is null or c.isRefre = 0 or c.isRefre = '')
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

        private DataTable GetReportDataRefer(int sampleID, int reportID)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT c.ClauseID, c.ClauseNumber, c.ClauseTitle, c.ClauseTitle_BH, c.Requirement, c.Requirement_BH, 
                                    c.ResultOptions, c.ResultOptions_BH, c.isHeader, c.standard_ID,
                                    ISNULL(t.Result, '') as Results, ISNULL(t.Remark, '') as Remark, ISNULL(t.InputType, '') as SavedInputType
                            FROM Clauses c
                            INNER JOIN TestResultsLED t ON c.ClauseID = t.ClauseID AND t.SampleID = @SampleID AND t.ReportID = @ReportID AND 
                                                           c.isRefre = 1
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

        #endregion

        #region T1 - Testing and Measurement

        private void CreateReportTableOpenXML(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{TestingAndMeasurement}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{TestingAndMeasurement}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{TestingAndMeasurement}", "");

            Table table = new Table();
            table.AppendChild(new TableProperties(
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
            ));

            float totalOriginalWidth = 0.6f + 3.4f + 2.0f + 1.0f;
            float scaleFactor = USABLE_WIDTH_TWIPS / (totalOriginalWidth * 1440f);
            int col1Width = (int)(0.7f * 1440f * scaleFactor);
            int col2Width = (int)(3.3f * 1440f * scaleFactor);
            int col3Width = (int)(2.0f * 1440f * scaleFactor);
            int col4Width = (int)(1.0f * 1440f * scaleFactor);

            TableGrid grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = col1Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col2Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col3Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col4Width.ToString() });
            table.AppendChild(grid);

            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            string[] headers = { "Klausul\nClause", "Persyaratan\nRequirements", "Hasil\nResult", "*Keterangan\nRemark" };
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                tableRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 300, HeightType = HeightRuleValues.AtLeast }));

                if (row["isHeader"].ToString() == "1")
                {
                    TableCell clauseCell = new TableCell();
                    clauseCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    Paragraph clausePara = new Paragraph();
                    clausePara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    Run clauseRun = new Run();
                    clauseRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    clauseRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clausePara.AppendChild(clauseRun);
                    clauseCell.AppendChild(clausePara);
                    tableRow.AppendChild(clauseCell);

                    TableCell titleCell = new TableCell();
                    titleCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                        new GridSpan() { Val = 2 }
                    ));
                    string clauseTitleBH = row["ClauseTitle_BH"]?.ToString() ?? "";
                    string clauseTitle = row["ClauseTitle"]?.ToString() ?? "";
                    AppendTwoLineParagraph(titleCell, clauseTitleBH, clauseTitle, true, JustificationValues.Left, 18);
                    tableRow.AppendChild(titleCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }
                else
                {
                    TableCell clauseNumCell = new TableCell();
                    clauseNumCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    Paragraph clauseNumPara = new Paragraph();
                    clauseNumPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    Run clauseNumRun = new Run();
                    clauseNumRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    clauseNumRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clauseNumPara.AppendChild(clauseNumRun);
                    clauseNumCell.AppendChild(clauseNumPara);
                    tableRow.AppendChild(clauseNumCell);

                    TableCell reqCell = new TableCell();
                    reqCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    string reqBH = row["Requirement_BH"]?.ToString() ?? "";
                    string reqEN = row["Requirement"]?.ToString() ?? "";
                    AppendTwoLineParagraph(reqCell, reqBH, reqEN, false, JustificationValues.Left, 18);
                    tableRow.AppendChild(reqCell);

                    TableCell resultCell = new TableCell();
                    resultCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }));
                    string inputType = GetInputTypeFromClause(row);
                    Paragraph resultPara = new Paragraph();
                    resultPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                    AppendCheckboxResultParagraph(resultPara, row, inputType);
                    resultCell.AppendChild(resultPara);
                    tableRow.AppendChild(resultCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }

                table.AppendChild(tableRow);
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateReportTableOpenXML_Appendix_C(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_C-TERMINAL}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_C-TERMINAL}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_C-TERMINAL}", "APPENDIX C – TERMINAL");
            
            Table table = new Table();
            table.AppendChild(new TableProperties(
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
            ));

            float totalOriginalWidth = 0.6f + 3.4f + 2.0f + 1.0f;
            float scaleFactor = USABLE_WIDTH_TWIPS / (totalOriginalWidth * 1440f);
            int col1Width = (int)(0.7f * 1440f * scaleFactor);
            int col2Width = (int)(3.3f * 1440f * scaleFactor);
            int col3Width = (int)(2.0f * 1440f * scaleFactor);
            int col4Width = (int)(1.0f * 1440f * scaleFactor);

            TableGrid grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = col1Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col2Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col3Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col4Width.ToString() });
            table.AppendChild(grid);

            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            string[] headers = { "Klausul\nClause", "Persyaratan\nRequirements", "Hasil\nResult", "*Keterangan\nRemark" };
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                tableRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 300, HeightType = HeightRuleValues.AtLeast }));

                if (row["isHeader"].ToString() == "1")
                {
                    TableCell clauseCell = new TableCell();
                    clauseCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    Paragraph clausePara = new Paragraph();
                    clausePara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    Run clauseRun = new Run();
                    clauseRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    clauseRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clausePara.AppendChild(clauseRun);
                    clauseCell.AppendChild(clausePara);
                    tableRow.AppendChild(clauseCell);

                    TableCell titleCell = new TableCell();
                    titleCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                        new GridSpan() { Val = 2 }
                    ));
                    string clauseTitleBH = row["ClauseTitle_BH"]?.ToString() ?? "";
                    string clauseTitle = row["ClauseTitle"]?.ToString() ?? "";
                    AppendTwoLineParagraph(titleCell, clauseTitleBH, clauseTitle, true, JustificationValues.Left, 18);
                    tableRow.AppendChild(titleCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }
                else
                {
                    TableCell clauseNumCell = new TableCell();
                    clauseNumCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    Paragraph clauseNumPara = new Paragraph();
                    clauseNumPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    Run clauseNumRun = new Run();
                    clauseNumRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    clauseNumRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clauseNumPara.AppendChild(clauseNumRun);
                    clauseNumCell.AppendChild(clauseNumPara);
                    tableRow.AppendChild(clauseNumCell);

                    TableCell reqCell = new TableCell();
                    reqCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    string reqBH = row["Requirement_BH"]?.ToString() ?? "";
                    string reqEN = row["Requirement"]?.ToString() ?? "";
                    AppendTwoLineParagraph(reqCell, reqBH, reqEN, false, JustificationValues.Left, 18);
                    tableRow.AppendChild(reqCell);

                    TableCell resultCell = new TableCell();
                    resultCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }));
                    string inputType = GetInputTypeFromClause(row);
                    Paragraph resultPara = new Paragraph();
                    resultPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                    AppendCheckboxResultParagraph(resultPara, row, inputType);
                    resultCell.AppendChild(resultPara);
                    tableRow.AppendChild(resultCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }

                table.AppendChild(tableRow);
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private string GetInputTypeFromClause(DataRow row)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            return !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "" ? "checkbox" : "text";
        }

        private string GetRemarkText(DataRow row)
        {
            if (row.Table.Columns.Contains("Remark")) return row["Remark"]?.ToString() ?? "";
            if (row.Table.Columns.Contains("Remarks")) return row["Remarks"]?.ToString() ?? "";
            if (row.Table.Columns.Contains("Keterangan")) return row["Keterangan"]?.ToString() ?? "";
            return "";
        }

        /// <summary>
        /// First line underlined, second line plain, joined with a real w:br so each line can
        /// carry its own formatting (this is the bilingual two-line pattern used everywhere).
        /// </summary>
        private void AppendTwoLineParagraph(TableCell cell, string firstLine, string secondLine, bool bold, JustificationValues alignment, int fontSize)
        {
            Paragraph para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = alignment });

            if (!string.IsNullOrEmpty(firstLine))
            {
                Run run1 = new Run();
                RunProperties rp1 = new RunProperties();
                if (bold) rp1.AppendChild(new Bold());
                rp1.AppendChild(new Underline() { Val = UnderlineValues.Single });
                rp1.AppendChild(new FontSize() { Val = fontSize.ToString() });
                run1.AppendChild(rp1);
                run1.AppendChild(new WordText() { Text = firstLine, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run1);
            }

            if (!string.IsNullOrEmpty(firstLine) && !string.IsNullOrEmpty(secondLine))
            {
                Run brRun = new Run();
                brRun.AppendChild(new Break());
                para.AppendChild(brRun);
            }

            if (!string.IsNullOrEmpty(secondLine))
            {
                Run run2 = new Run();
                RunProperties rp2 = new RunProperties();
                if (bold) rp2.AppendChild(new Bold());
                rp2.AppendChild(new FontSize() { Val = fontSize.ToString() });
                run2.AppendChild(rp2);
                run2.AppendChild(new WordText() { Text = secondLine, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run2);
            }

            cell.AppendChild(para);
        }

        /// <summary>
        /// One checkbox glyph run (Segoe UI Symbol) + one label run per option, so ☐/☒ always render.
        /// </summary>
        private void AppendCheckboxRun(Paragraph para, string optValue, string optDisplay, string[] selected)
        {
            bool isChecked = selected.Any(sVal => sVal.Trim().Equals(optValue, StringComparison.OrdinalIgnoreCase));
            string symbol = isChecked ? "☒" : "☐";

            Run symbolRun = new Run();
            symbolRun.AppendChild(new RunProperties(
                new RunFonts() { Ascii = "Segoe UI Symbol", HighAnsi = "Segoe UI Symbol", ComplexScript = "Segoe UI Symbol", EastAsia = "Segoe UI Symbol" },
                new FontSize() { Val = "20" }
            ));
            symbolRun.AppendChild(new WordText() { Text = symbol + " ", Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(symbolRun);

            Run labelRun = new Run();
            labelRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
            labelRun.AppendChild(new WordText() { Text = optDisplay, Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(labelRun);
        }

        private void AppendCheckboxResultParagraph(Paragraph resultPara, DataRow row, string inputType)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            string resultOptionsBH = row["ResultOptions_BH"]?.ToString();
            string selectedResults = row["Results"]?.ToString();

            if (inputType == "checkbox" && !string.IsNullOrEmpty(resultOptions))
            {
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : options;
                string[] selected = !string.IsNullOrEmpty(selectedResults) ? selectedResults.Split(',') : new string[0];

                for (int i = 0; i < options.Length; i += 2)
                {
                    string opt1 = options[i].Trim();
                    string disp1 = i < optionsBH.Length ? optionsBH[i].Trim() : opt1;
                    AppendCheckboxRun(resultPara, opt1, disp1, selected);

                    if (i + 1 < options.Length)
                    {
                        Run tabRun = new Run();
                        tabRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                        tabRun.AppendChild(new TabChar());
                        resultPara.AppendChild(tabRun);

                        string opt2 = options[i + 1].Trim();
                        string disp2 = (i + 1) < optionsBH.Length ? optionsBH[i + 1].Trim() : opt2;
                        AppendCheckboxRun(resultPara, opt2, disp2, selected);
                    }

                    if (i + 2 < options.Length)
                    {
                        Run brRun = new Run();
                        brRun.AppendChild(new Break());
                        resultPara.AppendChild(brRun);
                    }
                }
            }
            else
            {
                Run run = new Run();
                run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                run.AppendChild(new WordText() { Text = selectedResults ?? "", Space = SpaceProcessingModeValues.Preserve });
                resultPara.AppendChild(run);
            }
        }

        #endregion

        #region T2 - Appendix A (Components List)

        private void CreateReportTableT2OpenXML(WordprocessingDocument doc, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}",
                "LAMPIRAN A – DAFTAR KOMPONEN LUMINER\nAPPENDIX A – LIST OF LUMINARY COMPONENTS");

            Dictionary<int, int> columnMapping = new Dictionary<int, int>
            {
                { 17813, 0 }, { 17814, 1 }, { 17815, 2 }, { 17816, 3 }, { 17817, 4 }, { 17818, 5 }
            };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 6; i++) parsedColumns.Add(new string[0]);

            foreach (DataRow row in dt.Rows)
            {
                int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(reportParamMapID))
                {
                    int columnIndex = columnMapping[reportParamMapID];
                    parsedColumns[columnIndex] = row["Result"].ToString().Split(',');
                }
            }

            int numRows = parsedColumns.Max(col => col.Length);
            int numCols = 6;

            Table table = new Table();
            table.AppendChild(new TableProperties(
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
            ));

            float[] colWidths = { 1.2f, 1.0f, 1.0f, 1.0f, 0.8f, 1.5f };
            TableGrid grid = new TableGrid();
            foreach (float width in colWidths)
                grid.AppendChild(new GridColumn() { Width = ((int)(width * 1440f)).ToString() });
            table.AppendChild(grid);

            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            string[] headers = {
                "Nama Objek\nObject Name", "Pabrikan\nManufacturer", "Tipe/Model\nType/Model",
                "Data Teknis\nTechnical Data", "Standard", "Tanda Kesesuaian\nConformity Mark"
            };

            for (int col = 0; col < numCols; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                TableRow dataRow = new TableRow();
                dataRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    TableCell cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    string text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    Paragraph para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    Run run = new Run();
                    run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    run.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    dataRow.AppendChild(cell);
                }
                table.AppendChild(dataRow);
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        #endregion

        #region T3 - Appendix B (Photobiological Hazards)

        private void CreateReportTableT3OpenXML(WordprocessingDocument doc, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}",
                "LAMPIRAN B – BAHAYA FOTOBIOLOGIS\nAPPENDIX B – PHOTOBIOLOGICAL HAZARDS");

            Dictionary<int, string> measurementData = new Dictionary<int, string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["ReportParamMapID"] != DBNull.Value && row["Result"] != DBNull.Value)
                {
                    measurementData[Convert.ToInt32(row["ReportParamMapID"])] = row["Result"].ToString();
                }
            }

            Table mainTable = new Table();
            mainTable.AppendChild(new TableProperties(
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
            ));

            float[] colWidths = { 1.0f, 0.7f, 0.8f, 1.0f, 1.2f, 1.5f };
            TableGrid mainGrid = new TableGrid();
            foreach (float width in colWidths)
                mainGrid.AppendChild(new GridColumn() { Width = ((int)(width * 1440f)).ToString() });
            mainTable.AppendChild(mainGrid);

            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            string[][] headerParts = {
                new[] { "Risk", "Bahaya Optik" }, new[] { "Symbol", "Simbol" }, new[] { "Unit", "Satuan" },
                new[] { "Measurement Result", "Hasil Pengukuran" }, new[] { "Limit", "Batas" }, new[] { "Remark", "Keterangan" }
            };

            for (int col = 0; col < 6; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                Run run1 = new Run();
                run1.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run1.AppendChild(new WordText() { Text = headerParts[col][0], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run1);

                Run breakRun = new Run();
                breakRun.AppendChild(new Break());
                para.AppendChild(breakRun);

                Run run2 = new Run();
                run2.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run2.AppendChild(new WordText() { Text = headerParts[col][1], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run2);

                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            mainTable.AppendChild(headerRow);

            AddT3SectionRow(mainTable, "Actinic UV", "E_s", "W.m⁻²",
                GetMeasurementResultT3(17819, measurementData), "0,001\n(Exempt/RG0)",
                GetMeasurementResultT3(17820, measurementData), "0,003\n(Low Risk/RG1)",
                GetMeasurementResultT3(17821, measurementData), "0,03\n(Mod Risk/RG2)",
                "Risk group classification:\nRG0 (Exempt) /\nRG1 (Low Risk) /\nRG2 (Mod Risk) /\nRG3 (High Risk) /\nTest Result:\n(Pass / Fail)");

            AddT3SectionRow(mainTable, "Near UV", "E_UVA", "W.m⁻²",
                GetMeasurementResultT3(17822, measurementData), "10\n(Exempt/RG0)",
                GetMeasurementResultT3(17823, measurementData), "33\n(Low Risk/RG1)",
                GetMeasurementResultT3(17824, measurementData), "100\n(Mod Risk/RG2)",
                "Risk group classification:\nRG0 (Exempt) /\nRG1 (Low Risk) /\nRG2 (Mod Risk) /\nRG3 (High Risk) /\nTest Result:\n(Pass / Fail)");

            AddT3SectionRow(mainTable, "Blue light", "L_B", "W.m⁻².sr⁻¹",
                GetMeasurementResultT3(17825, measurementData), "100\n(Exempt/RG0)",
                GetMeasurementResultT3(17826, measurementData), "10000\n(Low Risk/RG1)",
                GetMeasurementResultT3(17827, measurementData), "4000000\n(Mod Risk/RG2)",
                "Risk group classification:\nRG0 (Exempt) /\nRG1 (Low Risk) /\nRG2 (Mod Risk) /\nRG3 (High Risk) /\nTest Result:\n(Pass / Fail)");

            string[] notes = {
                "Note:", "RG: Risk Group",
                "RG0 (Exempt group): The lamp does not pose any photobiological hazard.",
                "RG1 (Low Risk): The lamp does not pose a hazard due to normal behavioral limitations on exposure.",
                "RG2 (Moderate Risk): The lamp does not pose a hazard due to the aversion response to very bright light sources or due to thermal discomfort.",
                "RG3 (High Risk): The lamp may pose a hazard even for momentary or brief exposure.",
                "The lamps which exceed the limits for RG2 (Moderate Risk) are in RG3 (High Risk)."
            };

            foreach (string note in notes)
            {
                TableRow noteRow = new TableRow();
                noteRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));

                TableCell noteCell = new TableCell();
                noteCell.AppendChild(new TableCellProperties(
                    new GridSpan() { Val = 6 },
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                ));
                Paragraph notePara = new Paragraph();
                notePara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                Run noteRun = new Run();
                RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                if (note == "Note:") runProps.AppendChild(new Bold());
                if (note != "Note:" && note != "RG: Risk Group") runProps.AppendChild(new Italic());
                noteRun.AppendChild(runProps);
                noteRun.AppendChild(new WordText() { Text = note, Space = SpaceProcessingModeValues.Preserve });
                notePara.AppendChild(noteRun);
                noteCell.AppendChild(notePara);
                noteRow.AppendChild(noteCell);
                mainTable.AppendChild(noteRow);
            }

            targetParagraph.Parent.InsertAfter(mainTable, targetParagraph);

            Paragraph spacingPara = new Paragraph(new Run(new Break()));
            targetParagraph.Parent.InsertAfter(spacingPara, mainTable);

            Table secondTable = new Table();
            secondTable.AppendChild(new TableProperties(
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
            ));

            float[] secondColWidths = { 1.8f, 0.8f, 0.8f, 2.6f };
            TableGrid secondGrid = new TableGrid();
            foreach (float width in secondColWidths)
                secondGrid.AppendChild(new GridColumn() { Width = ((int)(width * 1440f)).ToString() });
            secondTable.AppendChild(secondGrid);

            TableRow secondHeaderRow = new TableRow();
            secondHeaderRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            string[][] secondHeaderParts = {
                new[] { "Item Test", "Item uji" }, new[] { "Symbol", "Simbol" }, new[] { "Unit", "Satuan" }, new[] { "Results", "Hasil uji" }
            };

            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                Run run1 = new Run();
                run1.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run1.AppendChild(new WordText() { Text = secondHeaderParts[col][0], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run1);

                Run breakRun2 = new Run();
                breakRun2.AppendChild(new Break());
                para.AppendChild(breakRun2);

                Run run2 = new Run();
                run2.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run2.AppendChild(new WordText() { Text = secondHeaderParts[col][1], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run2);

                cell.AppendChild(para);
                secondHeaderRow.AppendChild(cell);
            }
            secondTable.AppendChild(secondHeaderRow);

            secondTable.AppendChild(CreateT3DataRow("Blue light hazard radiance", "L_B", "W/m².sr", GetMeasurementResultT3(17828, measurementData)));
            secondTable.AppendChild(CreateT3DataRow("Luminance", "L_v", "cd/m²", GetMeasurementResultT3(17829, measurementData)));

            Paragraph spacingPara2 = new Paragraph(new Run(new Break()));
            targetParagraph.Parent.InsertAfter(spacingPara2, mainTable);
            targetParagraph.Parent.InsertAfter(secondTable, spacingPara2);
        }

        private TableRow CreateT3DataRow(string itemTest, string symbol, string unit, string result)
        {
            string[] data = { itemTest, symbol, unit, result };
            TableRow row = new TableRow();
            row.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = col == 0 ? JustificationValues.Left : JustificationValues.Center });
                Run run = new Run();
                run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                run.AppendChild(new WordText() { Text = data[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                row.AppendChild(cell);
            }
            return row;
        }

        private void AddT3SectionRow(Table table, string riskText, string symbolText, string unitText,
            string result1, string limit1, string result2, string limit2, string result3, string limit3, string remarkText)
        {
            TableRow row1 = new TableRow();
            row1.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));
            row1.AppendChild(CreateMergedCell(riskText, JustificationValues.Center, true, MergedCellValues.Restart));
            row1.AppendChild(CreateMergedCell(symbolText, JustificationValues.Center, true, MergedCellValues.Restart));
            row1.AppendChild(CreateMergedCell(unitText, JustificationValues.Center, true, MergedCellValues.Restart));
            row1.AppendChild(CreateRegularCell(result1, JustificationValues.Center, false));
            row1.AppendChild(CreateRegularCell(limit1, JustificationValues.Center, false));
            row1.AppendChild(CreateMergedCell(remarkText, JustificationValues.Left, false, MergedCellValues.Restart));
            table.AppendChild(row1);

            TableRow row2 = new TableRow();
            row2.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));
            row2.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));
            row2.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));
            row2.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));
            row2.AppendChild(CreateRegularCell(result2, JustificationValues.Center, false));
            row2.AppendChild(CreateRegularCell(limit2, JustificationValues.Center, false));
            row2.AppendChild(CreateMergedCell("", JustificationValues.Left, false, MergedCellValues.Continue));
            table.AppendChild(row2);

            TableRow row3 = new TableRow();
            row3.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));
            row3.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));
            row3.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));
            row3.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));
            row3.AppendChild(CreateRegularCell(result3, JustificationValues.Center, false));
            row3.AppendChild(CreateRegularCell(limit3, JustificationValues.Center, false));
            row3.AppendChild(CreateMergedCell("", JustificationValues.Left, false, MergedCellValues.Continue));
            table.AppendChild(row3);
        }

        private TableCell CreateMergedCell(string text, JustificationValues alignment, bool isBold, MergedCellValues mergeValue)
        {
            TableCell cell = new TableCell();
            TableCellProperties cellProps = new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
            cellProps.AppendChild(new VerticalMerge() { Val = mergeValue });
            cell.AppendChild(cellProps);
            cell.AppendChild(BuildMultiLinePara(text, alignment, isBold));
            return cell;
        }

        private TableCell CreateRegularCell(string text, JustificationValues alignment, bool isBold)
        {
            TableCell cell = new TableCell();
            cell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
            cell.AppendChild(BuildMultiLinePara(text, alignment, isBold));
            return cell;
        }

        private Paragraph BuildMultiLinePara(string text, JustificationValues alignment, bool isBold)
        {
            Paragraph para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = alignment });

            if (!string.IsNullOrEmpty(text) && text.Contains("\n"))
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    Run run = new Run();
                    RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                    if (isBold) runProps.AppendChild(new Bold());
                    run.AppendChild(runProps);
                    run.AppendChild(new WordText() { Text = lines[i], Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);

                    if (i < lines.Length - 1)
                    {
                        Run breakRun = new Run();
                        breakRun.AppendChild(new Break());
                        para.AppendChild(breakRun);
                    }
                }
            }
            else
            {
                Run run = new Run();
                RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                if (isBold) runProps.AppendChild(new Bold());
                run.AppendChild(runProps);
                run.AppendChild(new WordText() { Text = text ?? "", Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
            }

            return para;
        }

        private string GetMeasurementResultT3(int reportParamMapID, Dictionary<int, string> measurementData)
        {
            return measurementData.ContainsKey(reportParamMapID) ? (measurementData[reportParamMapID] ?? "") : "";
        }

        #endregion

        #region T4 - Appendix D (Thermal Testing)

        private void CreateReportTableT4OpenXML(WordprocessingDocument doc, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_D-UJI_TERMAL}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_D-UJI_TERMAL}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_D-UJI_TERMAL}",
                "LAMPIRAN D – UJI TERMAL\nAPPENDIX D – THERMAL TESTING");

            Dictionary<int, string[]> measurementData = new Dictionary<int, string[]>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["ReportParamMapID"] != DBNull.Value && row["Result"] != DBNull.Value)
                {
                    measurementData[Convert.ToInt32(row["ReportParamMapID"])] = row["Result"].ToString().Split('|');
                }
            }

            Table firstTable = new Table();
            firstTable.AppendChild(new TableProperties(
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
            ));

            float[] firstColWidths = { 0.6f, 3.5f, 1.2f, 1.2f };
            TableGrid firstGrid = new TableGrid();
            foreach (float width in firstColWidths)
                firstGrid.AppendChild(new GridColumn() { Width = ((int)(width * 1440f)).ToString() });
            firstTable.AppendChild(firstGrid);

            TableRow firstHeaderRow = new TableRow();
            firstHeaderRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            string[] firstHeaders = { "Klausul\nclause", "Persyaratan\nRequirement", "Hasil\nResult", "*Keterangan\nRemark*" };
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = firstHeaders[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                firstHeaderRow.AppendChild(cell);
            }
            firstTable.AppendChild(firstHeaderRow);

            string[,] firstTableData = {
                { "1.", "Tipe produk.", GetMeasurementValue(17830, 0, measurementData), GetMeasurementValue(17830, 1, measurementData) },
                { "2.", "Lampu yang digunakan.", GetMeasurementValue(17831, 0, measurementData), GetMeasurementValue(17831, 1, measurementData) },
                { "3.", "Tipe LED driver yang digunakan.", GetMeasurementValue(17832, 0, measurementData), GetMeasurementValue(17832, 1, measurementData) },
                { "4.", "Posisi pemasangan saat pengujian.", GetMeasurementValue(17833, 0, measurementData), GetMeasurementValue(17833, 1, measurementData) },
                { "5.", "Daya (W).", GetMeasurementValue(17834, 0, measurementData), GetMeasurementValue(17834, 1, measurementData) },
                { "6.", "Arus (A).", GetMeasurementValue(17835, 0, measurementData), GetMeasurementValue(17835, 1, measurementData) },
                { "7.", "Uji 1:\nTegangan pengenal (V).", GetMeasurementValue(17836, 0, measurementData), GetMeasurementValue(17836, 1, measurementData) },
                { "8.", "Uji 2:\n1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17837, 0, measurementData), GetMeasurementValue(17837, 1, measurementData) },
                { "9.", "Uji 3:\nBakan dengan dilengkapi kotak kontak 1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17838, 0, measurementData), GetMeasurementValue(17838, 1, measurementData) },
                { "10.", "Uji 4:\n1,1 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17839, 0, measurementData), GetMeasurementValue(17839, 1, measurementData) }
            };

            for (int row = 0; row < 10; row++)
            {
                TableRow dataRow = new TableRow();
                dataRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));

                for (int col = 0; col < 4; col++)
                {
                    TableCell cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    cell.AppendChild(BuildMultiLinePara(firstTableData[row, col], (col == 0 || col == 2) ? JustificationValues.Center : JustificationValues.Left, false));
                    dataRow.AppendChild(cell);
                }
                firstTable.AppendChild(dataRow);
            }

            targetParagraph.Parent.InsertAfter(firstTable, targetParagraph);

            Paragraph spacingPara = new Paragraph(new Run(new Break()));
            targetParagraph.Parent.InsertAfter(spacingPara, firstTable);

            Table secondTable = new Table();
            secondTable.AppendChild(new TableProperties(
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
            ));

            float[] secondColWidths = { 1.5f, 0.8f, 0.8f, 0.8f, 1.0f, 0.8f, 1.0f };
            TableGrid secondGrid = new TableGrid();
            foreach (float width in secondColWidths)
                secondGrid.AppendChild(new GridColumn() { Width = ((int)(width * 1440f)).ToString() });
            secondTable.AppendChild(secondGrid);

            TableRow headerRow1 = new TableRow();
            headerRow1.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            TableCell cell1 = new TableCell();
            cell1.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new VerticalMerge() { Val = MergedCellValues.Restart }
            ));
            cell1.AppendChild(BuildMultiLinePara("Bagian yang diuji", JustificationValues.Center, true));
            headerRow1.AppendChild(cell1);

            TableCell cell2 = new TableCell();
            cell2.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new GridSpan() { Val = 4 }
            ));
            cell2.AppendChild(BuildMultiLinePara("Klausul 12.4 – Uji termal\n(Operasi normal)", JustificationValues.Center, true));
            headerRow1.AppendChild(cell2);

            TableCell cell3 = new TableCell();
            cell3.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new GridSpan() { Val = 2 }
            ));
            cell3.AppendChild(BuildMultiLinePara("Klausul 12.5 – Uji termal\n(Operasi abnormal)", JustificationValues.Center, true));
            headerRow1.AppendChild(cell3);

            secondTable.AppendChild(headerRow1);

            TableRow headerRow2 = new TableRow();
            headerRow2.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            TableCell cell1Row2 = new TableCell();
            cell1Row2.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new VerticalMerge() { Val = MergedCellValues.Continue }
            ));
            headerRow2.AppendChild(cell1Row2);

            string[] subHeaders = { "Uji 1", "Uji 2", "Uji 3", "Limit Maks.", "Uji 4", "Limit Maks." };
            foreach (string sub in subHeaders)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                cell.AppendChild(BuildMultiLinePara(sub, JustificationValues.Center, true));
                headerRow2.AppendChild(cell);
            }
            secondTable.AppendChild(headerRow2);

            string[,] secondTableData = {
                { "Suhu ruang uji.", GetMeasurementValue(17840, 0, measurementData), GetMeasurementValue(17840, 1, measurementData), GetMeasurementValue(17840, 2, measurementData), GetMeasurementValue(17840, 3, measurementData), GetMeasurementValue(17840, 4, measurementData), GetMeasurementValue(17840, 5, measurementData) },
                { "Permukaan luar luminer", GetMeasurementValue(17841, 0, measurementData), GetMeasurementValue(17841, 1, measurementData), GetMeasurementValue(17841, 2, measurementData), GetMeasurementValue(17841, 3, measurementData), GetMeasurementValue(17841, 4, measurementData), GetMeasurementValue(17841, 5, measurementData) },
                { "Permukaan LED Driver.", GetMeasurementValue(17842, 0, measurementData), GetMeasurementValue(17842, 1, measurementData), GetMeasurementValue(17842, 2, measurementData), GetMeasurementValue(17842, 3, measurementData), GetMeasurementValue(17842, 4, measurementData), GetMeasurementValue(17842, 5, measurementData) },
                { "LED module / PCB.", GetMeasurementValue(17843, 0, measurementData), GetMeasurementValue(17843, 1, measurementData), GetMeasurementValue(17843, 2, measurementData), GetMeasurementValue(17843, 3, measurementData), GetMeasurementValue(17843, 4, measurementData), GetMeasurementValue(17843, 5, measurementData) },
                { "Selungkup LED module\n(Plastik)", GetMeasurementValue(17844, 0, measurementData), GetMeasurementValue(17844, 1, measurementData), GetMeasurementValue(17844, 2, measurementData), GetMeasurementValue(17844, 3, measurementData), GetMeasurementValue(17844, 4, measurementData), GetMeasurementValue(17844, 5, measurementData) },
                { "Kabel eksternal.", GetMeasurementValue(17845, 0, measurementData), GetMeasurementValue(17845, 1, measurementData), GetMeasurementValue(17845, 2, measurementData), GetMeasurementValue(17845, 3, measurementData), GetMeasurementValue(17845, 4, measurementData), GetMeasurementValue(17845, 5, measurementData) },
                { "Kabel internal.", GetMeasurementValue(17846, 0, measurementData), GetMeasurementValue(17846, 1, measurementData), GetMeasurementValue(17846, 2, measurementData), GetMeasurementValue(17846, 3, measurementData), GetMeasurementValue(17846, 4, measurementData), GetMeasurementValue(17846, 5, measurementData) }
            };

            for (int row = 0; row < 7; row++)
            {
                TableRow dataRow = new TableRow();
                dataRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));

                TableCell firstCell = new TableCell();
                firstCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                firstCell.AppendChild(BuildMultiLinePara(secondTableData[row, 0], JustificationValues.Left, false));
                dataRow.AppendChild(firstCell);

                for (int col = 1; col < 7; col++)
                {
                    TableCell cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                    cell.AppendChild(BuildMultiLinePara(secondTableData[row, col], JustificationValues.Center, false));
                    dataRow.AppendChild(cell);
                }
                secondTable.AppendChild(dataRow);
            }

            targetParagraph.Parent.InsertAfter(secondTable, spacingPara);
        }

        private string GetMeasurementValue(int reportParamMapID, int index, Dictionary<int, string[]> measurementData)
        {
            if (measurementData.ContainsKey(reportParamMapID))
            {
                string[] values = measurementData[reportParamMapID];
                if (index < values.Length) return values[index];
            }
            return string.Empty;
        }

        #endregion

        #region T5 - Measuring Equipment list

        private void CreateReportTableT5OpenXML(WordprocessingDocument doc, string placeholder, List<string> equipmentNames)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null)
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, placeholder, "");

            if (equipmentNames == null) equipmentNames = new List<string>();

            Table table = new Table();
            table.AppendChild(new TableProperties(
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
            ));

            float[] colWidths = { 0.6f, 5.9f };
            TableGrid grid = new TableGrid();
            foreach (float w in colWidths) grid.AppendChild(new GridColumn() { Width = ((int)(w * 1440f)).ToString() });
            table.AppendChild(grid);

            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 300, HeightType = HeightRuleValues.AtLeast }));
            string[] headers = { "No.", "Nama Alat/Equipment name" };
            for (int col = 0; col < 2; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph p = new Paragraph();
                p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run r = new Run();
                r.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                r.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                p.AppendChild(r);
                cell.AppendChild(p);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            for (int i = 0; i < equipmentNames.Count; i++)
            {
                TableRow row = new TableRow();
                row.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));

                TableCell noCell = new TableCell();
                noCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                noCell.AppendChild(BuildMultiLinePara((i + 1) + ".", JustificationValues.Center, false));
                row.AppendChild(noCell);

                TableCell nameCell = new TableCell();
                nameCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                Paragraph namePara = new Paragraph();
                namePara.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
                Run nameRun = new Run();
                nameRun.AppendChild(new RunProperties(new FontSize() { Val = "22" }));
                nameRun.AppendChild(new WordText() { Text = equipmentNames[i], Space = SpaceProcessingModeValues.Preserve });
                namePara.AppendChild(nameRun);
                nameCell.AppendChild(namePara);
                row.AppendChild(nameCell);

                table.AppendChild(row);
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        #endregion

        #region T6 - Overall Conclusion

        private void CreateReportTableT6OpenXML(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null)
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, placeholder, "");

            if (dt == null) return;

            Table table = new Table();
            table.AppendChild(new TableProperties(
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
            ));

            float[] colWidths = { 0.4f, 0.6f, 2.5f, 2.0f, 1.0f };
            TableGrid grid = new TableGrid();
            foreach (float w in colWidths) grid.AppendChild(new GridColumn() { Width = ((int)(w * 1440f)).ToString() });
            table.AppendChild(grid);

            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }));
            string[] headers = { "#", "Klausul\nClause", "Deskripsi uji\nTest Description", "Standard / Metode uji\nStandard/Test Method", "*Hasil uji\nTest Result" };
            for (int col = 0; col < 5; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                cell.AppendChild(BuildMultiLinePara(headers[col], JustificationValues.Center, true));
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                TableRow tr = new TableRow();
                tr.AppendChild(new TableRowProperties(new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }));

                tr.AppendChild(CreateT6Cell((i + 1).ToString(), JustificationValues.Center));
                tr.AppendChild(CreateT6Cell(row["ClauseNumber"]?.ToString() ?? "", JustificationValues.Center));
                tr.AppendChild(CreateT6Cell(row["ClauseTitle"]?.ToString() ?? "", JustificationValues.Left));

                // Original code split the standard names on ", " and put each on its own line
                // (using InsertAfter with a literal "\r" per segment); here it's just newline-joined text.
                string standardName = row["StandardName"]?.ToString() ?? "";
                string[] standards = standardName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                string standardCellText = standards.Length > 0 ? string.Join("\n", standards) : "";

                TableCell standardCell = new TableCell();
                standardCell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
                standardCell.AppendChild(BuildMultiLinePara(standardCellText, JustificationValues.Left, false));
                tr.AppendChild(standardCell);

                tr.AppendChild(CreateT6Cell(row["Result"]?.ToString() ?? "", JustificationValues.Center));

                table.AppendChild(tr);
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private TableCell CreateT6Cell(string text, JustificationValues alignment)
        {
            TableCell cell = new TableCell();
            cell.AppendChild(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }));
            cell.AppendChild(BuildMultiLinePara(text, alignment, false));
            return cell;
        }

        #endregion
    }
}

