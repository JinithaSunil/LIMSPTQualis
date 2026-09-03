using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using ZXing;
using A = DocumentFormat.OpenXml.Drawing;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using DataTable = System.Data.DataTable;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace LIMS
{
    public partial class CreateReportFromTemplate_ToyAndBabyWear : System.Web.UI.Page
    {
        private LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        private int lastUsedNumber = 0;
        private const int A4_PAGE_WIDTH_TWIPS = 12240;
        private const int LEFT_MARGIN_TWIPS = 1440;
        private const int RIGHT_MARGIN_TWIPS = 1440;
        private const int USABLE_WIDTH_TWIPS = A4_PAGE_WIDTH_TWIPS - LEFT_MARGIN_TWIPS - RIGHT_MARGIN_TWIPS;
        private const int MIN_COLUMN_WIDTH = 800;
        private const int MAX_COLUMN_WIDTH = 3000;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                /* Register Syncfusion License Key */
                string licenseKey = "Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTc0RnWX5cd3FcTmBdV091XA==";
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

                int sampleID = 0;
                string CategoryName = "";
                string path = Server.MapPath("~/Inputfiles/");
                string pathout = Server.MapPath("~/TestReport/");

                if (!Directory.Exists(pathout))
                {
                    Directory.CreateDirectory(pathout);
                }

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
                int ID = Convert.ToInt32(s);
                string SampleUID = "";
                DataTable dt = _cm.getSampleUID(ID);
                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    GenerateQR(SampleUID);
                }

                DataTable ChemicalDetails = _cm.getTestResultDetails_ToyAndBabyWear(sampleID);
                DataTable dt1 = _cm.getReportQuery(c);
                string template = dt1.Rows[0]["Template"].ToString();
                string templatePath = Path.Combine(path, template);
                string templatePath_BH = Path.Combine(path, dt1.Rows[0]["Template_BH"].ToString());
                int InquiryId = Convert.ToInt32(ChemicalDetails.Rows[0]["InquiryId"].ToString());
                ViewState["InquiryId_VS"] = InquiryId.ToString();

                List<string> pdfPaths = new List<string>();
                // Process Main Document
                byte[] mainDocBytes = File.ReadAllBytes(templatePath);
                byte[] mainProcessedBytes = ProcessDocumentMain(mainDocBytes, ChemicalDetails, SampleUID, ReportIDGet, s, dt1);
                // Process BH Document
                byte[] bhDocBytes = File.ReadAllBytes(templatePath_BH);
                byte[] bhProcessedBytes = ProcessDocumentBH(bhDocBytes, ChemicalDetails, SampleUID, ReportIDGet, s, dt1);
                // Define file paths
                string docxFileName = $"TestReport-{SampleUID}.docx";
                string pdfFileName = $"TestReport-{SampleUID}.pdf";
                string docxFileName_BH = $"TestReport-{SampleUID}_BH.docx";
                string pdfFileName_BH = $"TestReport-{SampleUID}_BH.pdf";
                string outputDocxPath = Path.Combine(pathout, docxFileName);
                string outputPdfPath = Path.Combine(pathout, pdfFileName);
                string outputDocxPath_BH = Path.Combine(pathout, docxFileName_BH);
                string outputPdfPath_BH = Path.Combine(pathout, pdfFileName_BH);

                // Save Word documents //
                File.WriteAllBytes(outputDocxPath, mainProcessedBytes);
                File.WriteAllBytes(outputDocxPath_BH, bhProcessedBytes);
                // Convert to PDF //
                try
                {
                    byte[] pdfBytes = ConvertDocxToPdf_Syncfusion(mainProcessedBytes);
                    File.WriteAllBytes(outputPdfPath, pdfBytes);
                    pdfPaths.Add($"/TestReport/{pdfFileName}");
                }
                catch (Exception pdfEx)
                {
                    Debug.WriteLine($"PDF conversion error for main document: {pdfEx.Message}");
                }

                try
                {
                    byte[] pdfBytes_BH = ConvertDocxToPdf_Syncfusion(bhProcessedBytes);
                    File.WriteAllBytes(outputPdfPath_BH, pdfBytes_BH);
                    pdfPaths.Add($"/TestReport/{pdfFileName_BH}");
                }
                catch (Exception pdfEx)
                {
                    Debug.WriteLine($"PDF conversion error for BH document: {pdfEx.Message}");
                }

                // Open PDFs in new tabs //
                if (pdfPaths.Count > 0)
                {
                    string script = GenerateOpenInNewTabScript(pdfPaths);
                    ClientScript.RegisterStartupScript(this.GetType(), "OpenPDFs", script);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script> alert('PDF files could not be generated.'); </script>");
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        // ========== SYNC FUSION DOCX TO PDF CONVERTER ========== //
        private byte[] ConvertDocxToPdf_Syncfusion(byte[] docxBytes)
        {
            try
            {
                using (MemoryStream docxStream = new MemoryStream(docxBytes))
                using (WordDocument wordDocument = new WordDocument(docxStream, FormatType.Automatic))
                {
                    using (DocIORenderer renderer = new DocIORenderer())
                    using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                    {
                        using (MemoryStream pdfStream = new MemoryStream())
                        {
                            pdfDocument.Save(pdfStream);
                            return pdfStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Syncfusion PDF conversion error: {ex.Message}");
            }
        }

        // ========== GENERATE SCRIPT TO OPEN PDFS ==========
        private string GenerateOpenInNewTabScript(List<string> pdfPaths)
        {
            if (pdfPaths.Count == 1)
            {
                return $@"<script>window.open('{pdfPaths[0]}', '_blank');</script>";
            }
            else
            {
                string script = "<script>";
                for (int i = 0; i < pdfPaths.Count; i++)
                {
                    int delay = i * 800;
                    script += $@"setTimeout(function() {{ window.open('{pdfPaths[i]}', '_blank'); }}, {delay});";
                }
                script += "</script>";
                return script;
            }
        }

        // ========== PROCESS DOCUMENT MAIN ==========
        private byte[] ProcessDocumentMain(byte[] docBytes, DataTable ChemicalDetails, string SampleUID, int ReportIDGet, string sampleId, DataTable dt1)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(docBytes, 0, docBytes.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    lastUsedNumber = 0;

                    if (ChemicalDetails != null && ChemicalDetails.Rows.Count > 0)
                    {
                        ProcessChemicalDetails(wordDoc, ChemicalDetails, SampleUID, ReportIDGet, false);
                    }

                    if (dt1.Rows.Count > 0)
                    {
                        string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');

                        for (int i = 0; i < query.Length; i++)
                        {
                            try
                            {
                                int InquiryId = Convert.ToInt32(ViewState["InquiryId_VS"]);
                                ProcessTableMain(wordDoc, table[i], query[i], sampleId, ReportIDGet, InquiryId);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error processing table {table[i]}: {ex.Message}");
                            }
                        }
                    }

                    CleanupUnresolvedPlaceholders(wordDoc);
                    wordDoc.MainDocumentPart.Document.Save();
                }
                return stream.ToArray();
            }
        }

        // ========== PROCESS DOCUMENT BH ==========
        private byte[] ProcessDocumentBH(byte[] docBytes, DataTable ChemicalDetails, string SampleUID, int ReportIDGet, string sampleId, DataTable dt1)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(docBytes, 0, docBytes.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    lastUsedNumber = 0;

                    if (ChemicalDetails != null && ChemicalDetails.Rows.Count > 0)
                    {
                        ProcessChemicalDetails(wordDoc, ChemicalDetails, SampleUID, ReportIDGet, true);
                    }

                    if (dt1.Rows.Count > 0)
                    {
                        string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');

                        for (int i = 0; i < query.Length; i++)
                        {
                            try
                            {
                                int InquiryId = Convert.ToInt32(ViewState["InquiryId_VS"]);
                                ProcessTableBH(wordDoc, table[i], query[i], sampleId, ReportIDGet, InquiryId);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error processing BH table {table[i]}: {ex.Message}");
                            }
                        }
                    }

                    CleanupUnresolvedPlaceholders(wordDoc);
                    wordDoc.MainDocumentPart.Document.Save();
                }
                return stream.ToArray();
            }
        }

        // ========== PROCESS TABLE MAIN ==========
        private void ProcessTableMain(WordprocessingDocument doc, string tableType, string query, string sampleId, int ReportIDGet, int InquiryId)
        {
            switch (tableType)
            {
                case "10":
                    DataTable dt10 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable10(doc, "{ResultSummaryTable}", dt10);
                    break;

                case "11":
                    DataTable dt11 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable11(doc, "{ResultTable1}", dt11);
                    break;

                case "12":
                    DataTable dt12 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable12(doc, "{TestParameter}", dt12);
                    break;

                case "13":
                    DataTable dt13 = _cm.getReportQuery(1, query + sampleId);
                    List<Tuple<string, string, string>> imageData = new List<Tuple<string, string, string>>();
                    foreach (DataRow row in dt13.Rows)
                    {
                        string imageFileName = row["SelectedImages"].ToString().Trim();
                        string imageName = row["SampleText"].ToString().Trim();
                        string sampleColourName = row["SampleColourName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (IsUsableImageFile(fullImagePath))
                            {
                                imageData.Add(Tuple.Create(fullImagePath, imageName, sampleColourName));
                            }
                        }
                    }
                    InsertImagesIntoDocument1(doc, "{images}", imageData);
                    break;

                case "14":
                    DataTable dt14 = _cm.getReportQuery(1, query);
                    CreateTable14(doc, "{Testingandmeasuringequipment}", dt14);
                    break;

                case "15":
                    DataTable dt15 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable15(doc, "{ResultTable1}", dt15);
                    break;

                case "16":
                    DataTable dt16 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable16(doc, "{ResultTable2}", dt16);
                    break;

                case "17":
                    DataTable dt17 = _cm.GetResultOverallConclusion_ToysAndBabyWear(sampleId);
                    CreateTable17(doc, "{Overall Conclusion}", dt17);
                    break;

                case "18":
                    DataTable dt18 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable18(doc, "{ResultTable1}", dt18);
                    break;

                case "19":
                    DataTable dt19 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable19(doc, "{ResultTable2}", dt19);
                    break;

                case "20":
                    DataTable dt20 = _cm.getReportQuery(1, query + sampleId);
                    CreateTableforSampleDescription(doc, "{SampleDescriptionData}", dt20);
                    break;

                case "21":
                    DataTable dt21 = _cm.getReportQuery(1, query + sampleId);
                    List<Tuple<string, string>> imageData21 = new List<Tuple<string, string>>();
                    foreach (DataRow row in dt21.Rows)
                    {
                        string imageFileName = row["filename"].ToString().Trim();
                        string imageName = row["SampleText"].ToString();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (IsUsableImageFile(fullImagePath))
                            {
                                imageData21.Add(Tuple.Create(fullImagePath, imageName));
                            }
                        }
                    }
                    InsertImagesIntoDocument21(doc, "{images}", imageData21);
                    break;

                case "22":
                    DataTable dt22 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable22(doc, "{TestParameter}", dt22);
                    break;

                case "23":
                    DataTable dt23 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable23(doc, "{ResultTable1}", dt23);
                    break;

                case "24":
                    DataTable dt24 = _cm.getReportQuery(1, query + InquiryId);
                    CreateTable24(doc, "{ResultTable1}", dt24);
                    break;
            }
        }

        // ========== PROCESS TABLE BH ==========
        private void ProcessTableBH(WordprocessingDocument doc, string tableType, string query, string sampleId, int ReportIDGet, int InquiryId)
        {
            switch (tableType)
            {
                case "10":
                    string q10 = @"SELECT ROW_NUMBER() OVER (ORDER BY tp.TestParameter_BH) AS [No.], tp.TestParameter_BH AS [Test parameters] FROM crfsampledetails c 
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                   GROUP BY tp.TestParameter_BH, t.sampleID having sampleID ='" + sampleId + "' ";
                    DataTable dt10 = _cm.getReportQuery_BH(q10);
                    CreateTable10_BH(doc, "{ResultSummaryTable}", dt10);
                    break;

                case "11":
                    string q11 = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                   c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                   WHERE t.sampleID= '" + sampleId + "' ";
                    DataTable dt11 = _cm.getReportQuery_BH(q11);
                    CreateTable11_BH(doc, "ResultTable1}", dt11);
                    break;

                case "12":
                    string q12 = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                   c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                   WHERE t.sampleID='" + sampleId + "' ";
                    DataTable dt12 = _cm.getReportQuery_BH(q12);
                    CreateTable12_BH(doc, "{TestParameter}", dt12);
                    break;

                case "13":
                    DataTable dt13 = _cm.getReportQuery(1, query + sampleId);
                    List<Tuple<string, string, string>> imageDataBH = new List<Tuple<string, string, string>>();
                    foreach (DataRow row in dt13.Rows)
                    {
                        string imageFileName = row["SelectedImages"].ToString().Trim();
                        string imageName = row["SampleText"].ToString().Trim();
                        string sampleColourName_BH = row["SampleColourName_BH"].ToString().Trim();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (IsUsableImageFile(fullImagePath))
                            {
                                imageDataBH.Add(Tuple.Create(fullImagePath, imageName, sampleColourName_BH));
                            }
                        }
                    }
                    InsertImagesIntoDocument1_BH(doc, "{images}", imageDataBH);
                    break;

                case "14":
                    string q14 = @"select Equipment ,Type,Calibration_Date as[Calibration Due Date] from TestResultDetails t 
                                   INNER JOIN Calibration_Equipment c on c.sampleid = t.SampleId ";
                    DataTable dt14 = _cm.getReportQuery_BH(q14);
                    CreateTable14_BH(doc, "{Testingandmeasuringequipment}", dt14);
                    break;

                case "15":
                    string q15 = @"SELECT tp.TestParameter_BH AS[Test Parameter], tsp.TestSubParameter_BH AS[Test Item], TestMethod AS[Method *],
                                   c.MDL as [M.D.L],TestResult AS[Result(s)], t.EquipmentName AS[Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND(c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '')
                                   WHERE t.sampleID ='" + sampleId + "' ";
                    DataTable dt15 = _cm.getReportQuery_BH(q15);
                    CreateTable15_BH(doc, "{ResultTable1}", dt15);
                    break;

                case "16":
                    string q16 = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Test Item], tssp.TestSubSubParameter_BH AS [Compound], 
                                   TestMethod AS [Method*], c.MDL as [M.D.L],TestResult AS [Result(s)],
                                   t.EquipmentName AS [Equipment] FROM crfsampledetails c 
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NOT NULL AND c.TestSubSubParameter != '') 
                                   WHERE t.sampleID ='" + sampleId + "' ";
                    DataTable dt16 = _cm.getReportQuery_BH(q16);
                    CreateTable16_BH(doc, "{ ResultTable2}", dt16);
                    break;

                case "17":
                    DataTable dt17 = _cm.GetResultOverallConclusion_BH_ToysAndBabyWear(sampleId);
                    CreateTable17_BH(doc, "{Overall Conclusion}", dt17);
                    break;

                case "18":
                    DataTable dt18 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable18_BH(doc, "{ResultTable1}", dt18);
                    break;

                case "19":
                    DataTable dt19 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable19_BH(doc, "{ResultTable2}", dt19);
                    break;

                case "20":
                    DataTable dt20 = _cm.getReportQuery(1, query + sampleId);
                    CreateTableforSampleDescription_BH(doc, "{SampleDescriptionData}", dt20);
                    break;

                case "21":
                    DataTable dt21 = _cm.getReportQuery(1, query + sampleId);
                    List<Tuple<string, string>> imageData21BH = new List<Tuple<string, string>>();
                    foreach (DataRow row in dt21.Rows)
                    {
                        string imageFileName = row["filename"].ToString().Trim();
                        string imageName = row["SampleText"].ToString();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (IsUsableImageFile(fullImagePath))
                            {
                                imageData21BH.Add(Tuple.Create(fullImagePath, imageName));
                            }
                        }
                    }
                    InsertImagesIntoDocument21_BH(doc, "{images}", imageData21BH);
                    break;

                case "22":
                    DataTable dt22 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable22_BH(doc, "{TestParameter}", dt22);
                    break;

                case "23":
                    DataTable dt23 = _cm.getReportQuery(1, query + sampleId);
                    CreateTable23_BH(doc, "{ResultTable1}", dt23);
                    break;

                case "24":
                    DataTable dt24 = _cm.getReportQuery(1, query + InquiryId);
                    CreateTable24_BH(doc, "{ResultTable1}", dt24);
                    break;
            }
        }

        // ========== GENERATE QR CODE ==========
        protected void GenerateQR(string SampleID)
        {
            try
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

                var options = new ZXing.Common.EncodingOptions
                {
                    Width = 3000,
                    Height = 3000,
                    Margin = 2
                };
                barcodeWriter.Options = options;

                Bitmap barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);

                string barcodeFilePath = Path.Combine(barcodeFolderPath, QRCodeFileName);
                barcodeBitmap.Save(barcodeFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error generating QR: " + ex.Message);
            }
        }

        // ========== PLACEHOLDER HELPER METHODS ==========
        private string GetParagraphFullText(Paragraph paragraph)
        {
            return string.Join("", paragraph.Descendants<WordText>().Select(t => t.Text));
        }

        private List<Paragraph> FindAllParagraphsContaining(OpenXmlElement root, string placeholder)
        {
            var result = new List<Paragraph>();
            foreach (var para in root.Descendants<Paragraph>())
            {
                if (GetParagraphFullText(para).Contains(placeholder))
                    result.Add(para);
            }
            return result;
        }

        private bool ReplaceInParagraph(Paragraph paragraph, string findText, string replaceText)
        {
            var runs = paragraph.Descendants<Run>().ToList();
            if (runs.Count == 0) return false;

            foreach (var run in runs)
            {
                var textElement = run.Descendants<WordText>().FirstOrDefault();
                if (textElement != null && textElement.Text.Contains(findText))
                {
                    textElement.Text = textElement.Text.Replace(findText, replaceText);
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

        private void FindAndReplace(WordprocessingDocument doc, string findText, string replaceText)
        {
            if (string.IsNullOrEmpty(findText)) return;
            if (replaceText == null) replaceText = "";

            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            ReplaceAllInElement(mainPart.Document.Body, findText, replaceText);

            foreach (var headerPart in mainPart.HeaderParts)
            {
                if (headerPart.Header != null)
                    ReplaceAllInElement(headerPart.Header, findText, replaceText);
            }

            foreach (var footerPart in mainPart.FooterParts)
            {
                if (footerPart.Footer != null)
                    ReplaceAllInElement(footerPart.Footer, findText, replaceText);
            }
        }

        private void ReplaceAllInElement(OpenXmlElement element, string findText, string replaceText)
        {
            foreach (var paragraph in FindAllParagraphsContaining(element, findText))
            {
                ReplaceInParagraph(paragraph, findText, replaceText);
            }
        }

        private void FindAndReplaceMultiple(WordprocessingDocument doc, Dictionary<string, string> replacements)
        {
            foreach (var kvp in replacements)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    try
                    {
                        FindAndReplace(doc, kvp.Key, kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to replace {kvp.Key}: {ex.Message}");
                    }
                }
            }
        }

        private void AddReplacementIfExists(Dictionary<string, string> replacements, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (replacements.ContainsKey(key))
                    replacements[key] = value;
                else
                    replacements.Add(key, value);
            }
        }

        private void CleanupUnresolvedPlaceholders(WordprocessingDocument doc)
        {
            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            var regex = new Regex(@"\{[A-Za-z][A-Za-z0-9_.\*:'/ ]{0,60}\}");

            void CleanElement(OpenXmlElement element)
            {
                foreach (var paragraph in element.Descendants<Paragraph>().ToList())
                {
                    string fullText = GetParagraphFullText(paragraph);
                    if (string.IsNullOrEmpty(fullText)) continue;

                    var matches = regex.Matches(fullText);
                    foreach (Match m in matches)
                    {
                        if (GetParagraphFullText(paragraph).Contains(m.Value))
                        {
                            Debug.WriteLine($"Unresolved placeholder left in document, blanking: {m.Value}");
                            ReplaceInParagraph(paragraph, m.Value, "");
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

        private void RemovePlaceholderFromParagraph(Paragraph paragraph, string placeholder)
        {
            ReplaceInParagraph(paragraph, placeholder, "");
        }

        private bool IsUsableImageFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return false;
            try
            {
                if (new FileInfo(path).Length == 0) return false;
            }
            catch
            {
                return false;
            }
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif" || ext == ".tiff" || ext == ".tif";
        }

        private PartTypeInfo GetImagePartTypeForFile(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    return ImagePartType.Jpeg;
                case ".bmp":
                    return ImagePartType.Bmp;
                case ".gif":
                    return ImagePartType.Gif;
                case ".tif":
                case ".tiff":
                    return ImagePartType.Tiff;
                case ".png":
                default:
                    return ImagePartType.Png;
            }
        }

        private void GetScaledEmuSize(string path, long maxWidthEmu, long maxHeightEmu, out long width, out long height)
        {
            try
            {
                using (var img = System.Drawing.Image.FromFile(path))
                {
                    double pxWidth = img.Width;
                    double pxHeight = img.Height;
                    double aspect = pxWidth / pxHeight;

                    double targetWidth = maxWidthEmu;
                    double targetHeight = targetWidth / aspect;

                    if (targetHeight > maxHeightEmu)
                    {
                        targetHeight = maxHeightEmu;
                        targetWidth = targetHeight * aspect;
                    }

                    width = (long)targetWidth;
                    height = (long)targetHeight;
                    return;
                }
            }
            catch
            {
                width = maxWidthEmu;
                height = maxHeightEmu;
            }
        }

        // ========== IMAGE METHODS ==========
        private void ReplacePlaceholderWithImage(WordprocessingDocument doc, string placeholder, string imagePath)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            ReplaceInParagraph(targetParagraph, placeholder, "");

            if (IsUsableImageFile(imagePath))
            {
                var imagePart = mainPart.AddImagePart(GetImagePartTypeForFile(imagePath));
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    imagePart.FeedData(stream);
                }

                var element = CreateImageElement(mainPart.GetIdOfPart(imagePart), "QR Code", 700000L, 700000L);
                targetParagraph.AppendChild(new Run(element));

                targetParagraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
            }
        }

        private Drawing CreateImageElement(string relationshipId, string name, long width = 200000L, long height = 200000L)
        {
            return new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = width, Cy = height },
                    new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                    new DW.DocProperties() { Id = 1U, Name = name },
                    new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties() { Id = 0U, Name = name },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip(
                                        new A.BlipExtensionList(
                                            new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" })
                                    )
                                    {
                                        Embed = relationshipId,
                                        CompressionState = A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = width, Cy = height }),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }))
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U
                });
        }

        // ========== PROCESS CHEMICAL DETAILS ==========
        private void ProcessChemicalDetails(WordprocessingDocument doc, DataTable ChemicalDetails, string SampleUID, int ReportIDGet, bool isBH)
        {
            if (ChemicalDetails == null || ChemicalDetails.Rows.Count == 0) return;

            DataRow row = ChemicalDetails.Rows[0];

            string GetSafeValue(string columnName)
            {
                if (!row.Table.Columns.Contains(columnName)) return "";
                return row[columnName] == DBNull.Value || row[columnName] == null ? "" : row[columnName].ToString();
            }

            string GetSafeDate_old(string columnName)
            {
                if (!row.Table.Columns.Contains(columnName)) return "";
                if (row[columnName] == DBNull.Value || row[columnName] == null) return "";
                try
                {
                    return Convert.ToDateTime(row[columnName]).ToString("dd MMMM yyyy");
                }
                catch
                {
                    return row[columnName].ToString();
                }
            }

            string GetSafeDate(string columnName)
            {
                if (!row.Table.Columns.Contains(columnName)) return "";
                if (row[columnName] == DBNull.Value || row[columnName] == null) return "";

                string dateString = row[columnName].ToString().Trim();
                DateTime dateValue;

                string[] formats = new string[]
                {
                    "dd/MM/yyyy",
                    "dd/MM/yyyy HH:mm:ss",
                    "dd/MM/yyyy HH:mm:ss.fff",
                    "d/M/yyyy",
                    "d/M/yyyy HH:mm:ss",
                    "yyyy-MM-dd",
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss.fff",
                    "yyyy-MM-ddTHH:mm:ss",
                    "yyyy-MM-ddTHH:mm:ss.fff",
                    "yyyy-MM-ddTHH:mm:ss.fffZ",
                    "MM/dd/yyyy",
                    "MM/dd/yyyy HH:mm:ss",
                    "M/d/yyyy",
                    "M/d/yyyy HH:mm:ss",
                    "dd-MM-yyyy",
                    "dd-MM-yyyy HH:mm:ss",
                    "d-M-yyyy",
                    "dd.MM.yyyy",
                    "dd.MM.yyyy HH:mm:ss",
                    "d.M.yyyy",
                    "dd MMM yyyy",      // 31 Aug 2026
                    "dd MMMM yyyy",     // 31 August 2026
                    "MMM dd, yyyy",     // Aug 31, 2026
                    "MMMM dd, yyyy",    // August 31, 2026
                    "dd-MMM-yyyy",      // 31-Aug-2026    
                    // With timezone
                    "yyyy-MM-dd HH:mm:ss zzz",
                    "yyyy-MM-ddTHH:mm:ss.fffK",    
                    // Short date + time
                    "dd/MM/yy",
                    "dd/MM/yy HH:mm:ss",
                    "M/d/yy",
                    "yyyy-MM-dd HH:mm",
                    "dd/MM/yyyy HH:mm",    
                    // Without leading zeros
                    "d/M/yyyy",
                    "M/d/yyyy",
                    "d-M-yyyy",
                    "d.M.yyyy",    
                    // With AM/PM
                    "dd/MM/yyyy hh:mm:ss tt",
                    "MM/dd/yyyy hh:mm:ss tt",
                    "yyyy-MM-dd hh:mm:ss tt"
                };

                if (DateTime.TryParseExact(dateString, formats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out dateValue))
                {
                    return dateValue.ToString("dd MMMM yyyy");
                }

                // Fallback to generic parsing
                if (DateTime.TryParse(dateString,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out dateValue))
                {
                    return dateValue.ToString("dd MMMM yyyy");
                }

                return dateString;
            }

            
            var replacements = new Dictionary<string, string>();

            AddReplacementIfExists(replacements, "{RequestNo.}", GetSafeValue("sampleIDSeries"));
            AddReplacementIfExists(replacements, "{Type}", GetSafeValue("TypeDescription"));
            AddReplacementIfExists(replacements, "{GroupofProduct}", GetSafeValue("GroupOfProduct"));
            AddReplacementIfExists(replacements, "{ProductDescription}", GetSafeValue("DescriptionOfProduct"));
            AddReplacementIfExists(replacements, "{Remark}", GetSafeValue("GetRemarks"));
            
            AddReplacementIfExists(replacements, "{ProductGroupName}", GetSafeValue("ProductGroupName"));
            AddReplacementIfExists(replacements, "{LotNo}", GetSafeValue("LotNumber"));
            AddReplacementIfExists(replacements, "{MaterialName}", GetSafeValue("MaterialName_Code"));
            AddReplacementIfExists(replacements, "{Email}", GetSafeValue("Email"));
            AddReplacementIfExists(replacements, "{CountryOfDestination}", GetSafeValue("CountryOfDestination"));
            AddReplacementIfExists(replacements, "{P.O.Order}", GetSafeValue("PONo"));
            AddReplacementIfExists(replacements, "{ArticleNumber}", GetSafeValue("ArticleNumber"));
            AddReplacementIfExists(replacements, "{Attention}", GetSafeValue("Attention"));
            AddReplacementIfExists(replacements, "{TestSample}", GetSafeValue("TestSample"));
            AddReplacementIfExists(replacements, "{SampleReceivedDate}", GetSafeDate("IssueDate"));
            AddReplacementIfExists(replacements, "{TestPerformancedate}", GetSafeDate("Date"));
            AddReplacementIfExists(replacements, "{IssueDate}", GetSafeDate("IssueDate"));
            AddReplacementIfExists(replacements, "{TestPeriod}", GetSafeDate("TestPeriod"));
            AddReplacementIfExists(replacements, "{Samplecondition}", GetSafeValue("SampleCondition"));
            AddReplacementIfExists(replacements, "{Applicant}", GetSafeValue("Applicant"));
            AddReplacementIfExists(replacements, "{Address}", GetSafeValue("Address"));
            AddReplacementIfExists(replacements, "{SampleDescription}", GetSafeValue("SampleDescription"));
            AddReplacementIfExists(replacements, "{Material Name/Code}", GetSafeValue("MaterialName_Code"));
            AddReplacementIfExists(replacements, "{Sample1}", GetSafeValue("SampleDescription"));
            AddReplacementIfExists(replacements, "{Buyer'sName}", GetSafeValue("Buyer'sName"));
            AddReplacementIfExists(replacements, "{CountryofOrigin}", GetSafeValue("CountryOrigin"));
            AddReplacementIfExists(replacements, "{BrandProduct}", GetSafeValue("BrandProduct"));
            AddReplacementIfExists(replacements, "{AgeGroup}", GetSafeValue("AgeGroup"));
            AddReplacementIfExists(replacements, "{Suppliername}", GetSafeValue("Suppliername"));
            AddReplacementIfExists(replacements, "{FactoryName}", GetSafeValue("FactoryName"));
            AddReplacementIfExists(replacements, "{FactoryAddress}", GetSafeValue("FactoryAddress"));
            AddReplacementIfExists(replacements, "{MaterialType}", GetSafeValue("MaterialType"));
            AddReplacementIfExists(replacements, "{ItemStyleNo}", GetSafeValue("ItemStyleNo"));
            AddReplacementIfExists(replacements, "{MaterialName_Code}", GetSafeValue("MaterialName_Code"));
            AddReplacementIfExists(replacements, "{ColorName_Code}", GetSafeValue("ColorName_Code"));
            AddReplacementIfExists(replacements, "{BatchNo}", GetSafeValue("BatchNo"));
            AddReplacementIfExists(replacements, "{WorkorderNo}", GetSafeValue("WorkorderNumber"));
            AddReplacementIfExists(replacements, "{TestCategory}", GetSafeValue("GroupofProduct"));
            //AddReplacementIfExists(replacements, "{TestMethod}", GetSafeValue("TestMethod"));
            AddReplacementIfExists(replacements, "{SampleTestMethodSummary}", GetSafeValue("TestMethod"));
            AddReplacementIfExists(replacements, "{ColorName}", GetSafeValue("Color"));
            AddReplacementIfExists(replacements, "{TestSpecification}", GetSafeValue("ProductSpecification"));
            AddReplacementIfExists(replacements, "{TestPackage}", GetSafeValue("GroupofProduct"));

            AddReplacementIfExists(replacements, "{LabHeadName}", GetSafeValue("LabHeadName"));
            AddReplacementIfExists(replacements, "{CountryofOrigin}", GetSafeValue("CountryOrigin"));
            AddReplacementIfExists(replacements, "{Materialtype}", GetSafeValue("MaterialType"));
            AddReplacementIfExists(replacements, "{Issuedate}", GetSafeDate("IssueDate"));
            AddReplacementIfExists(replacements, "{TestReportvaliduntil}", GetSafeDate("DateOfExp"));

            AddReplacementIfExists(replacements, "{TestReportNo.}", GetSafeValue("TestReportNo"));
            AddReplacementIfExists(replacements, "{DocNo}", "TSQ-LAB-QAF-089");

            FindAndReplaceMultiple(doc, replacements);

            // Replace in header/footer
            ReplaceInHeaderFooter(doc, "{TestReportNo}", GetSafeValue("TestReportNo"));
            ReplaceInHeaderFooter(doc, "{DocNo}", isBH ? "TSQ-LAB-QAF-087" : "TSQ-LAB-QAF-089");

            // QR Code
            string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
            if (File.Exists(imageUrl))
            {
                ReplacePlaceholderWithImage(doc, "{QR_Code}", imageUrl);
            }

            // Final Result
            string finalResult = GetSafeValue("FinalTestResult").Trim().ToUpper();
            if (isBH)
            {
                string resultBH = finalResult == "PASS" ? "LULUS" : "TIDAK LULUS";
                InsertResultTableIntoDocument(doc, "{FinalTestResult}", resultBH, isBH: true);
            }
            else
            {
                string resultEN = finalResult == "PASS" ? "PASS" : "FAIL";
                InsertResultTableIntoDocument(doc, "{FinalTestResult}", resultEN, isBH: false);
            }

            ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
        }

        // ========== REPLACE IN HEADER/FOOTER ==========
        private void ReplaceInHeaderFooter(WordprocessingDocument doc, string placeholder, string replacement)
        {
            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            foreach (var headerPart in mainPart.HeaderParts)
            {
                if (headerPart.Header != null)
                    ReplaceAllInElement(headerPart.Header, placeholder, replacement);
            }

            foreach (var footerPart in mainPart.FooterParts)
            {
                if (footerPart.Footer != null)
                    ReplaceAllInElement(footerPart.Footer, placeholder, replacement);
            }
        }

        // ========== INSERT RESULT TABLE (CHECKBOX) ==========
        private void InsertResultTableIntoDocument(WordprocessingDocument doc, string placeholder, string result, bool isBH)
        {
            try
            {
                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null || string.IsNullOrEmpty(result)) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                string upperResult = result.Trim().ToUpper();

                Table table;
                if (isBH)
                {
                    bool isLulusChecked = upperResult == "LULUS";
                    bool isTidakLulusChecked = upperResult == "TIDAK LULUS";
                    table = BuildPassFailCheckboxTable(isLulusChecked, isTidakLulusChecked, "LULUS", "TIDAK LULUS");
                }
                else
                {
                    bool isPassChecked = upperResult == "PASS";
                    bool isFailChecked = upperResult == "FAIL";
                    table = BuildPassFailCheckboxTable(isPassChecked, isFailChecked, "PASS", "FAIL");
                }

                targetParagraph.Parent.InsertAfter(table, targetParagraph);
                Paragraph breakParagraph = new Paragraph(new Run());
                targetParagraph.Parent.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in InsertResultTableIntoDocument: {ex.Message}");
            }
        }

        private Table BuildPassFailCheckboxTable(bool isFirstRowChecked, bool isSecondRowChecked, string firstLabel, string secondLabel)
        {
            const string BORDER_COLOR = "1f4e79";
            const string TEXT_COLOR = "000080";
            const string BOX_WIDTH = "380";
            const string LABEL_WIDTH = "1800";
            const uint BORDER_SIZE = 8;

            var table = new Table();

            var tableProps = new TableProperties(
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = (int.Parse(BOX_WIDTH) + int.Parse(LABEL_WIDTH)).ToString(), Type = TableWidthUnitValues.Dxa },
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = BORDER_SIZE, Color = BORDER_COLOR },
                    new BottomBorder() { Val = BorderValues.Single, Size = BORDER_SIZE, Color = BORDER_COLOR },
                    new LeftBorder() { Val = BorderValues.Single, Size = BORDER_SIZE, Color = BORDER_COLOR },
                    new RightBorder() { Val = BorderValues.Single, Size = BORDER_SIZE, Color = BORDER_COLOR },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = BORDER_SIZE, Color = BORDER_COLOR },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = BORDER_SIZE, Color = BORDER_COLOR }
                )
            );
            table.AppendChild(tableProps);

            var grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = BOX_WIDTH });
            grid.AppendChild(new GridColumn() { Width = LABEL_WIDTH });
            table.AppendChild(grid);

            table.AppendChild(BuildCheckboxRow(isFirstRowChecked, firstLabel, BOX_WIDTH, LABEL_WIDTH));
            table.AppendChild(BuildCheckboxRow(isSecondRowChecked, secondLabel, BOX_WIDTH, LABEL_WIDTH));

            return table;
        }

        private TableRow BuildCheckboxRow(bool isChecked, string labelText, string boxWidth, string labelWidth)
        {
            const string TEXT_COLOR = "000080";
            const uint ROW_HEIGHT = 420;

            var row = new TableRow();
            row.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = ROW_HEIGHT, HeightType = HeightRuleValues.Exact }
            ));

            var boxCell = new TableCell();
            boxCell.AppendChild(new TableCellProperties(
                new TableCellWidth() { Width = boxWidth, Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            ));

            var boxPara = new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }));
            if (isChecked)
            {
                var xRun = new Run(
                    new RunProperties(
                        new Bold(),
                        new FontSize() { Val = "36" },
                        new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = TEXT_COLOR }
                    ),
                    new WordText() { Text = "X", Space = SpaceProcessingModeValues.Preserve }
                );
                boxPara.AppendChild(xRun);
            }
            boxCell.AppendChild(boxPara);
            row.AppendChild(boxCell);

            var labelCell = new TableCell();
            labelCell.AppendChild(new TableCellProperties(
                new TableCellWidth() { Width = labelWidth, Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            ));
            var para = new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Left }));
            var run = new Run(
                new RunProperties(
                    new Bold(),
                    new FontSize() { Val = "28" },
                    new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = TEXT_COLOR }
                ),
                new WordText() { Text = labelText, Space = SpaceProcessingModeValues.Preserve }
            );
            para.AppendChild(run);
            labelCell.AppendChild(para);
            row.AppendChild(labelCell);

            return row;
        }

        // ========== TABLE CREATION METHODS ==========
        private Table CreateFormattedTable(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return null;

            int colCount = dt.Columns.Count;
            if (colCount == 0) return null;

            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);

            int tableWidth = USABLE_WIDTH_TWIPS;
            tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            int baseColWidth = tableWidth / colCount;
            int remainder = tableWidth - (baseColWidth * colCount);
            List<int> colWidths = new List<int>();
            for (int i = 0; i < colCount; i++)
            {
                colWidths.Add(i == colCount - 1 ? baseColWidth + remainder : baseColWidth);
            }

            var grid = new TableGrid();
            foreach (int width in colWidths)
            {
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // Header row
            var headerRow = new TableRow();
            var headerRowProps = new TableRowProperties();
            headerRowProps.AppendChild(new TableRowHeight { Val = 300, HeightType = HeightRuleValues.AtLeast });
            headerRow.AppendChild(headerRowProps);

            for (int i = 0; i < colCount; i++)
            {
                var cell = new TableCell();
                var cellProps = new TableCellProperties();
                cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                cellProps.AppendChild(new Shading { Color = "D9D9D9", Fill = "D9D9D9", Val = ShadingPatternValues.Clear });
                cellProps.AppendChild(new TableCellMargin
                {
                    TopMargin = new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                    BottomMargin = new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                    LeftMargin = new LeftMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin { Width = "80", Type = TableWidthUnitValues.Dxa }
                });
                cell.AppendChild(cellProps);

                var p = new Paragraph();
                p.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center },
                    SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                };

                var run = new Run();
                var runProps = new RunProperties();
                runProps.AppendChild(new Bold());
                runProps.AppendChild(new FontSize { Val = "22" });
                run.AppendChild(runProps);
                run.AppendChild(new WordText(dt.Columns[i].ColumnName) { Space = SpaceProcessingModeValues.Preserve });
                p.AppendChild(run);
                cell.AppendChild(p);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            foreach (DataRow dataRow in dt.Rows)
            {
                var row = new TableRow();
                var rowProps = new TableRowProperties();
                rowProps.AppendChild(new TableRowHeight { Val = 240, HeightType = HeightRuleValues.AtLeast });
                row.AppendChild(rowProps);

                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    var cellProps = new TableCellProperties();
                    cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                    cellProps.AppendChild(new TableCellMargin
                    {
                        TopMargin = new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                        BottomMargin = new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                        LeftMargin = new LeftMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                        RightMargin = new RightMargin { Width = "80", Type = TableWidthUnitValues.Dxa }
                    });
                    cell.AppendChild(cellProps);

                    string cellText = dataRow[i]?.ToString() ?? string.Empty;

                    var p = new Paragraph();
                    p.ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification { Val = JustificationValues.Center },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                    };

                    var run = new Run();
                    var runProps = new RunProperties();
                    runProps.AppendChild(new FontSize { Val = "20" });

                    // Check if this is the Result column (last column)
                    bool isResultColumn = (i == colCount - 1);
                    if (isResultColumn)
                    {
                        if (cellText.Equals("PASS", StringComparison.OrdinalIgnoreCase) ||
                            cellText.Equals("Pass", StringComparison.OrdinalIgnoreCase))
                        {
                            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "008000" });
                            runProps.AppendChild(new Bold());
                        }
                        else if (cellText.Equals("FAIL", StringComparison.OrdinalIgnoreCase) ||
                                 cellText.Equals("Fail", StringComparison.OrdinalIgnoreCase))
                        {
                            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "FF0000" });
                            runProps.AppendChild(new Bold());
                        }
                    }

                    run.AppendChild(runProps);
                    run.AppendChild(new WordText(cellText) { Space = SpaceProcessingModeValues.Preserve });
                    p.AppendChild(run);
                    cell.AppendChild(p);
                    row.AppendChild(cell);
                }
                table.AppendChild(row);
            }

            return table;
        }

        // ========== TABLE 10 ==========
        private void CreateTable10(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                FindAndReplace(doc, placeholder, "No data available");
                return;
            }

            var table = CreateFormattedTable(doc, dt);
            if (table == null) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);

            var afterParagraph = new Paragraph();
            afterParagraph.ParagraphProperties = new ParagraphProperties
            {
                SpacingBetweenLines = new SpacingBetweenLines { After = "120" }
            };
            table.InsertAfterSelf(afterParagraph);
        }

        // ========== TABLE 11 ==========
        private void CreateTable11(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            Paragraph currentParagraph = targetParagraph;
            bool isFirstGroup = true;
            int groupNumber = 0;

            foreach (var group in groupedData)
            {
                groupNumber++;
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group) groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        FindAndReplace(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        FindAndReplace(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        FindAndReplace(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Method*" && col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                // Check for empty columns
                List<string> columnsToRemove = new List<string>();
                foreach (DataColumn column in modifiedDt.Columns)
                {
                    bool allNullOrBlank = true;
                    foreach (DataRow row in groupDt.Rows)
                    {
                        string value = row[column.ColumnName]?.ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            allNullOrBlank = false;
                            break;
                        }
                    }
                    if (allNullOrBlank) columnsToRemove.Add(column.ColumnName);
                }
                foreach (var colName in columnsToRemove) modifiedDt.Columns.Remove(colName);

                // Copy data
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Insert section header
                if (!isFirstGroup && currentParagraph != null)
                {
                    var spacingPara = new Paragraph(new Run(new Break()));
                    currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                    currentParagraph = spacingPara;

                    var headerPara = new Paragraph();
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    var preTreatPara = new Paragraph();
                    var preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new WordText($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.Parent.InsertAfter(preTreatPara, currentParagraph);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    var headerPara = new Paragraph();
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    var preTreatPara = new Paragraph();
                    var preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new WordText($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.Parent.InsertAfter(preTreatPara, currentParagraph);
                    currentParagraph = preTreatPara;
                }

                var table = CreateFormattedTable(doc, modifiedDt);
                if (table != null && currentParagraph != null)
                {
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;
                }
            }
        }

        // ========== TABLE 12 ==========
        private void CreateTable12(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            Paragraph currentParagraph = targetParagraph;
            bool isFirstGroup = true;
            int groupNumber = 0;

            foreach (var group in groupedData)
            {
                groupNumber++;
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group) groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        FindAndReplace(doc, "{Number}", $"{groupNumber}. ");
                        FindAndReplace(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        FindAndReplace(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        FindAndReplace(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        FindAndReplace(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Test Item" &&
                        col.ColumnName != "Method*" && col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup && currentParagraph != null)
                {
                    var spacingPara = new Paragraph(new Run(new Break()));
                    currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                    currentParagraph = spacingPara;

                    var headerPara = new Paragraph();
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    var preTreatPara = new Paragraph();
                    var preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new WordText($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.Parent.InsertAfter(preTreatPara, currentParagraph);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    var headerPara = new Paragraph();
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    var preTreatPara = new Paragraph();
                    var preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new WordText($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.Parent.InsertAfter(preTreatPara, currentParagraph);
                    currentParagraph = preTreatPara;
                }

                var table = CreateFormattedTable(doc, modifiedDt);
                if (table != null && currentParagraph != null)
                {
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;
                }
            }
        }

        // ========== TABLE 14 ==========
        private void CreateTable14(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                FindAndReplace(doc, placeholder, "No calibration equipment data available.");
                return;
            }

            var table = CreateFormattedTable(doc, dt);
            if (table == null) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        // ========== TABLE 15 ==========
        private void CreateTable15(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            Paragraph currentParagraph = targetParagraph;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group) groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        FindAndReplace(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        FindAndReplace(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup && currentParagraph != null)
                {
                    var spacingPara = new Paragraph(new Run(new Break()));
                    currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                    currentParagraph = spacingPara;

                    var headerPara = new Paragraph();
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    var preTreatPara = new Paragraph();
                    var preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new WordText($"*)Analysis was performed by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.Parent.InsertAfter(preTreatPara, currentParagraph);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;
                    var spacingPara = new Paragraph(new Run(new Break()));
                    currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                    currentParagraph = spacingPara;
                }

                var table = CreateFormattedTable(doc, modifiedDt);
                if (table != null && currentParagraph != null)
                {
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;
                }
            }
        }

        // ========== TABLE 16 ==========
        private void CreateTable16(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            Paragraph currentParagraph = targetParagraph;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group) groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        FindAndReplace(doc, "{Number}", $"{lastUsedNumber}. ");
                        FindAndReplace(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        FindAndReplace(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        FindAndReplace(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Test Item" && col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup && currentParagraph != null)
                {
                    var spacingPara = new Paragraph(new Run(new Break()));
                    currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                    currentParagraph = spacingPara;

                    var headerPara = new Paragraph();
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    var preTreatPara = new Paragraph();
                    var preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new WordText($"*)Analysis was performed by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.Parent.InsertAfter(preTreatPara, currentParagraph);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;
                    var spacingPara = new Paragraph(new Run(new Break()));
                    currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                    currentParagraph = spacingPara;
                }

                var table = CreateFormattedTable(doc, modifiedDt);
                if (table != null && currentParagraph != null)
                {
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;
                }
            }
        }

        // ========== TABLE 17 (Overall Conclusion) ==========
        private void CreateTable17(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                FindAndReplace(doc, "{Overall Conclusion}", "No data available");
                FindAndReplace(doc, "{Requirement}", "No requirements available");
                return;
            }

            var requirements = dt.AsEnumerable()
                .Select(row => row["Requirement"]?.ToString())
                .Where(req => !string.IsNullOrWhiteSpace(req))
                .Distinct()
                .ToList();
            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            FindAndReplace(doc, "{Requirement}", combinedRequirements);

            DataTable modifiedDt = dt.Copy();
            if (modifiedDt.Columns.Contains("Requirement"))
                modifiedDt.Columns.Remove("Requirement");

            var table = BuildTable17(modifiedDt);
            if (table == null) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private Table BuildTable17(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return null;

            int colCount = dt.Columns.Count;
            if (colCount == 0) return null;

            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);

            int tableWidth = USABLE_WIDTH_TWIPS;
            tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            // Column widths: 10%, 35%, 35%, 20%
            double[] percentages = new double[] { 10, 35, 35, 20 };
            List<int> colWidths = new List<int>();
            for (int i = 0; i < colCount && i < percentages.Length; i++)
            {
                colWidths.Add((int)(tableWidth * (percentages[i] / 100.0)));
            }

            var grid = new TableGrid();
            foreach (int width in colWidths)
            {
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // Header row
            var headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight { Val = 400, HeightType = HeightRuleValues.AtLeast }));

            for (int i = 0; i < colCount; i++)
            {
                var cell = new TableCell();
                var cellProps = new TableCellProperties();
                cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                cellProps.AppendChild(new Shading { Color = "D9D9D9", Fill = "D9D9D9", Val = ShadingPatternValues.Clear });
                cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                cell.AppendChild(cellProps);

                var para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center },
                    SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                };

                var run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                run.AppendChild(new WordText(dt.Columns[i].ColumnName) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            foreach (DataRow dataRow in dt.Rows)
            {
                var row = new TableRow();
                row.AppendChild(new TableRowProperties(new TableRowHeight { Val = 350, HeightType = HeightRuleValues.AtLeast }));

                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    var cellProps = new TableCellProperties();
                    cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                    cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                    cell.AppendChild(cellProps);

                    string cellText = dataRow[i]?.ToString() ?? string.Empty;

                    var para = new Paragraph();
                    JustificationValues justification = (i == 1) ? JustificationValues.Left : JustificationValues.Center;
                    para.ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification { Val = justification },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                    };

                    var run = new Run();
                    var runProps = new RunProperties(new FontSize { Val = "22" });

                    bool isResultColumn = (i == colCount - 1);
                    if (isResultColumn)
                    {
                        if (cellText.Equals("PASS", StringComparison.OrdinalIgnoreCase) ||
                            cellText.Equals("Pass", StringComparison.OrdinalIgnoreCase) ||
                            cellText.Equals("LULUS", StringComparison.OrdinalIgnoreCase))
                        {
                            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "008000" });
                            runProps.AppendChild(new Bold());
                        }
                        else if (cellText.Equals("FAIL", StringComparison.OrdinalIgnoreCase) ||
                                 cellText.Equals("Fail", StringComparison.OrdinalIgnoreCase) ||
                                 cellText.Equals("TIDAK LULUS", StringComparison.OrdinalIgnoreCase))
                        {
                            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "FF0000" });
                            runProps.AppendChild(new Bold());
                        }
                    }

                    run.AppendChild(runProps);
                    run.AppendChild(new WordText(cellText) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    row.AppendChild(cell);
                }
                table.AppendChild(row);
            }

            return table;
        }

        // ========== TABLE 18 ==========//
        private void CreateTable18(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Parameter"))
                    .ToList();

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                Paragraph currentParagraph = targetParagraph;
                bool isFirstGroup = true;

                foreach (var testParamGroup in groupedData)
                {
                    lastUsedNumber++;

                    var elementGroups = testParamGroup
                        .GroupBy(row => row.Field<string>("Element"))
                        .ToList();

                    var allColorCodeCombinations = testParamGroup
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    var firstRow = testParamGroup.First();

                    if (isFirstGroup)
                    {
                        FindAndReplace(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());

                        string methodText = firstRow["Method*"].ToString();
                        if (methodText.Contains("TSQ-LAB") || methodText.Contains(","))
                        {
                            int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                            if (colorCodeStart > 0)
                                methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                            else
                            {
                                int commaIndex = methodText.IndexOf(',');
                                if (commaIndex > 0)
                                    methodText = methodText.Substring(0, commaIndex).Trim();
                            }
                        }
                        FindAndReplace(doc, "{TestMethod}", methodText);
                        FindAndReplace(doc, "{Equipment}", firstRow["Equipment"].ToString());
                    }

                    if (!isFirstGroup && currentParagraph != null)
                    {
                        var spacingPara = new Paragraph(new Run(new Break()));
                        currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                        currentParagraph = spacingPara;

                        var headerPara = new Paragraph();
                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                        headerRun.AppendChild(new WordText($"{lastUsedNumber}. {firstRow["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                        headerPara.AppendChild(headerRun);
                        currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                        currentParagraph = headerPara;

                        string methodText = firstRow["Method*"].ToString();
                        if (methodText.Contains("TSQ-LAB") || methodText.Contains(","))
                        {
                            int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                            if (colorCodeStart > 0)
                                methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                            else
                            {
                                int commaIndex = methodText.IndexOf(',');
                                if (commaIndex > 0)
                                    methodText = methodText.Substring(0, commaIndex).Trim();
                            }
                        }

                        var methodPara = new Paragraph();
                        var methodRun = new Run();
                        methodRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        methodRun.AppendChild(new WordText($"Test Method: {methodText} - Analyzed by {firstRow["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                        methodPara.AppendChild(methodRun);
                        currentParagraph.Parent.InsertAfter(methodPara, currentParagraph);
                        currentParagraph = methodPara;
                    }

                    if (currentParagraph != null)
                    {
                        int resultSubCols = allColorCodeCombinations.Count;
                        bool hasManyColumns = resultSubCols > 4;

                        var table = BuildTable18(elementGroups, allColorCodeCombinations, hasManyColumns);
                        if (table != null)
                        {
                            currentParagraph.Parent.InsertAfter(table, currentParagraph);

                            // Insert a fresh paragraph after the table to continue writing into,
                            // rather than reusing a paragraph that lives inside the table.
                            var afterTablePara = new Paragraph();
                            table.Parent.InsertAfter(afterTablePara, table);
                            currentParagraph = afterTablePara;
                        }

                        // MDL line
                        var mdlParts = elementGroups
                            .Select(eg => eg.First())
                            .Where(r => !string.IsNullOrWhiteSpace(r["MDL"]?.ToString()))
                            .Select(r => $"{r["Element"]}: {r["MDL"]} (mg/kg)")
                            .ToList();

                        if (mdlParts.Count > 0 && currentParagraph != null)
                        {
                            var mdlRun = new Run();
                            mdlRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                            mdlRun.AppendChild(new WordText($"Method Detection Limit= {string.Join(", ", mdlParts)}") { Space = SpaceProcessingModeValues.Preserve });
                            currentParagraph.AppendChild(mdlRun);

                            var nextPara = new Paragraph();
                            currentParagraph.Parent.InsertAfter(nextPara, currentParagraph);
                            currentParagraph = nextPara;
                        }
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTable18: {ex.Message}");
            }
        }

        private Table BuildTable18(IEnumerable<IGrouping<string, DataRow>> elementGroups, List<string> allColorCodeCombinations, bool hasManyColumns)
        {
            int resultSubCols = allColorCodeCombinations.Count;

            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);
            tableProps.AppendChild(new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            // Column widths
            var colWidths = new List<int>();
            int elementWidth = hasManyColumns ? 2500 : 3000;
            int requirementWidth = hasManyColumns ? 1800 : 2000;
            int availableForResults = USABLE_WIDTH_TWIPS - elementWidth - requirementWidth;
            int resultColWidth = resultSubCols > 0
                ? Math.Max(800, Math.Min(2000, availableForResults / resultSubCols))
                : 0;

            colWidths.Add(elementWidth);
            for (int i = 0; i < resultSubCols; i++)
                colWidths.Add(resultColWidth);
            colWidths.Add(requirementWidth);

            var grid = new TableGrid();
            foreach (int width in colWidths)
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            table.AppendChild(grid);

            // Header row
            var headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight { Val = hasManyColumns ? 600U : 800U, HeightType = HeightRuleValues.Exact }));

            headerRow.AppendChild(MakeHeaderCell(new[] { "Element/Compound" }, colWidths[0]));
            for (int i = 0; i < resultSubCols; i++)
                headerRow.AppendChild(MakeHeaderCell(
                    new[] { "Result", "(mg/kg)", $"({allColorCodeCombinations[i]})" },
                    colWidths[i + 1],
                    hasManyColumns ? "16" : "20"));
            headerRow.AppendChild(MakeHeaderCell(new[] { "Requirement", "(mg/kg)" }, colWidths[colWidths.Count - 1]));
            table.AppendChild(headerRow);

            // Data rows
            bool[] combinationHasDetection = new bool[resultSubCols];

            foreach (var elementGroup in elementGroups)
            {
                var row = new TableRow();
                string fontSize = hasManyColumns ? "18" : "20";
                row.AppendChild(MakeDataCell(elementGroup.Key, colWidths[0], leftAlign: true, fontSize: fontSize));

                var resultCells = new List<TableCell>();
                for (int i = 0; i < resultSubCols; i++)
                    resultCells.Add(MakeDataCell("N.D", colWidths[i + 1], fontSize: fontSize));
                foreach (var rc in resultCells) row.AppendChild(rc);

                foreach (var dataRow in elementGroup)
                {
                    string result = dataRow["Result(mg/kg)"].ToString();
                    string code = dataRow["Colour Code"]?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrWhiteSpace(code)) continue;

                    int idx = allColorCodeCombinations.IndexOf(code);
                    if (idx < 0) continue;

                    var textEl = resultCells[idx].Descendants<WordText>().FirstOrDefault();
                    if (textEl != null) textEl.Text = result;

                    if (result.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                        result.Equals("Detected", StringComparison.OrdinalIgnoreCase))
                        combinationHasDetection[idx] = true;
                }

                row.AppendChild(MakeDataCell(elementGroup.First()["Requirement"].ToString(), colWidths[colWidths.Count - 1], fontSize: fontSize));
                table.AppendChild(row);
            }

            // Remark row
            var remarkRow = new TableRow();
            remarkRow.AppendChild(MakeRemarkCell("Remark", colWidths[0], true));
            for (int i = 0; i < resultSubCols; i++)
            {
                bool fail = combinationHasDetection[i];
                remarkRow.AppendChild(MakeRemarkCell(fail ? "FAIL" : "PASS", colWidths[i + 1], true, fail ? "FF0000" : "008000"));
            }
            remarkRow.AppendChild(MakeRemarkCell("", colWidths[colWidths.Count - 1], false));
            table.AppendChild(remarkRow);

            return table;
        }

        // ========== TABLE 19 ==========//
        private void CreateTable19(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    foreach (var p in new[] { "{Number}", "{TestParameter2}", "{TestSubParameter}",
                                              "{TestMethod}", "{Equipment2}", "{ResultTable2}",
                                              "{AnalysisWasPerformed}", "{:}" })
                        FindAndReplace(doc, p, string.Empty);
                    return;
                }

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Item"))
                    .ToList();

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                Paragraph currentParagraph = targetParagraph;
                bool isFirstGroup = true;

                foreach (var group in groupedData)
                {
                    lastUsedNumber++;

                    DataTable groupDt = dt.Clone();
                    foreach (var row in group) groupDt.ImportRow(row);

                    if (groupDt.Rows.Count > 0)
                    {
                        if (groupDt.Columns.Contains("Test Parameter"))
                        {
                            FindAndReplace(doc, "{Number}", $"{lastUsedNumber}. ");
                            FindAndReplace(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                            FindAndReplace(doc, "{AnalysisWasPerformed}", "*) Analysis was performed ");
                            FindAndReplace(doc, "{:}", " : ");
                        }
                        if (groupDt.Columns.Contains("Test Item"))
                            FindAndReplace(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                        if (groupDt.Columns.Contains("Method*"))
                            FindAndReplace(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                        if (groupDt.Columns.Contains("Equipment"))
                            FindAndReplace(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                    }

                    var allColorCodes = group
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .SelectMany(code => code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(code => code.Trim())
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    DataTable modifiedDt = new DataTable();
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" && col.ColumnName != "Test Item" &&
                            col.ColumnName != "Method*" && col.ColumnName != "Equipment" &&
                            col.ColumnName != "Colour Code" && col.ColumnName != "Result(mg/kg)" &&
                            col.ColumnName != "Conclusion" && col.ColumnName != "MDL")
                            modifiedDt.Columns.Add(col.ColumnName);
                    }

                    foreach (var code in allColorCodes)
                        modifiedDt.Columns.Add($"Result (mg/kg) ({code})");

                    bool[] colorCodeHasDetection = new bool[allColorCodes.Count];

                    var compoundGroups = group
                        .GroupBy(row => string.Join("|",
                            modifiedDt.Columns.Cast<DataColumn>()
                                .Where(c => !c.ColumnName.StartsWith("Result (mg/kg)"))
                                .Select(c => row[c.ColumnName]?.ToString() ?? "")))
                        .ToList();

                    foreach (var compoundGroup in compoundGroups)
                    {
                        DataRow newRow = modifiedDt.NewRow();
                        var fr = compoundGroup.First();
                        foreach (DataColumn col in modifiedDt.Columns)
                            if (!col.ColumnName.StartsWith("Result (mg/kg)"))
                                newRow[col.ColumnName] = fr[col.ColumnName];

                        for (int i = 0; i < allColorCodes.Count; i++)
                            newRow[$"Result (mg/kg) ({allColorCodes[i]})"] = "N.D";

                        foreach (var row in compoundGroup)
                        {
                            string rawCode = row["Colour Code"]?.ToString()?.Trim() ?? "";
                            string val = row["Result(mg/kg)"]?.ToString()?.Trim() ?? "";
                            if (string.IsNullOrWhiteSpace(rawCode)) continue;

                            foreach (var code in rawCode.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string clean = code.Trim();
                                string col = $"Result (mg/kg) ({clean})";
                                if (!modifiedDt.Columns.Contains(col)) continue;
                                newRow[col] = val;
                                int ci = allColorCodes.FindIndex(c => c.Equals(clean, StringComparison.OrdinalIgnoreCase));
                                if (ci >= 0 && (val.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                                val.Equals("Detected", StringComparison.OrdinalIgnoreCase)))
                                    colorCodeHasDetection[ci] = true;
                            }
                        }
                        modifiedDt.Rows.Add(newRow);
                    }

                    if (!isFirstGroup && currentParagraph != null)
                    {
                        var spacingPara = new Paragraph(new Run(new Break()));
                        currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                        currentParagraph = spacingPara;

                        var headerPara = new Paragraph();
                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                        headerRun.AppendChild(new WordText($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                        headerPara.AppendChild(headerRun);
                        currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                        currentParagraph = headerPara;

                        var methodPara = new Paragraph();
                        var methodRun = new Run();
                        methodRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        methodRun.AppendChild(new WordText($"Test Method: {groupDt.Rows[0]["Method*"]}, analyzed by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                        methodPara.AppendChild(methodRun);
                        currentParagraph.Parent.InsertAfter(methodPara, currentParagraph);
                        currentParagraph = methodPara;
                    }

                    if (currentParagraph != null)
                    {
                        int resultSubCols = allColorCodes.Count;
                        bool hasManyColumns = resultSubCols > 4;

                        var table = BuildTable19(modifiedDt, allColorCodes, colorCodeHasDetection, hasManyColumns);
                        if (table != null)
                        {
                            currentParagraph.Parent.InsertAfter(table, currentParagraph);

                            var afterTablePara = new Paragraph();
                            table.Parent.InsertAfter(afterTablePara, table);
                            currentParagraph = afterTablePara;
                        }

                        var mdlParts = group
                            .Where(row => !string.IsNullOrWhiteSpace(row["MDL"]?.ToString()))
                            .Select(row => $"{row["Element/Compound"]}: {row["MDL"]} mg/kg")
                            .Distinct()
                            .ToList();

                        if (mdlParts.Count > 0 && currentParagraph != null)
                        {
                            var mdlRun = new Run();
                            mdlRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                            mdlRun.AppendChild(new WordText($"Method Detection Limits- {string.Join(", ", mdlParts)}") { Space = SpaceProcessingModeValues.Preserve });
                            currentParagraph.AppendChild(mdlRun);

                            var nextPara = new Paragraph();
                            currentParagraph.Parent.InsertAfter(nextPara, currentParagraph);
                            currentParagraph = nextPara;
                        }
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTable19: {ex.Message}");
            }
        }

        private Table BuildTable19(DataTable modifiedDt, List<string> allColorCodes, bool[] colorCodeHasDetection, bool hasManyColumns)
        {
            int resultSubCols = allColorCodes.Count;

            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);
            tableProps.AppendChild(new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            // Column widths
            var colWidths = new List<int>();
            int fixedCols = modifiedDt.Columns.Count - resultSubCols;
            int fixedWidth = (int)(USABLE_WIDTH_TWIPS * 0.35);
            int resultWidth = resultSubCols > 0 ? (USABLE_WIDTH_TWIPS - fixedWidth) / resultSubCols : 0;

            for (int i = 0; i < fixedCols; i++)
                colWidths.Add(fixedWidth / Math.Max(fixedCols, 1));
            for (int i = 0; i < resultSubCols; i++)
                colWidths.Add(resultWidth);

            var grid = new TableGrid();
            foreach (int width in colWidths)
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            table.AppendChild(grid);

            // Header row
            var headerRow = new TableRow();
            int colIndex = 0;
            string headerFontSize = hasManyColumns ? "16" : "20";
            foreach (DataColumn col in modifiedDt.Columns)
            {
                if (col.ColumnName.StartsWith("Result (mg/kg)"))
                {
                    string code = allColorCodes[colIndex - (modifiedDt.Columns.Count - resultSubCols)];
                    headerRow.AppendChild(MakeHeaderCell(new[] { "Result", "(mg/kg)", $"({code})" }, colWidths[colIndex], headerFontSize));
                }
                else
                {
                    headerRow.AppendChild(MakeHeaderCell(new[] { col.ColumnName }, colWidths[colIndex], headerFontSize));
                }
                colIndex++;
            }
            table.AppendChild(headerRow);

            // Data rows
            foreach (DataRow row in modifiedDt.Rows)
            {
                var dataRow = new TableRow();
                for (int i = 0; i < modifiedDt.Columns.Count; i++)
                {
                    bool isResultCol = modifiedDt.Columns[i].ColumnName.StartsWith("Result (mg/kg)");
                    dataRow.AppendChild(MakeDataCell(row[i]?.ToString() ?? "", colWidths[i], leftAlign: !isResultCol));
                }
                table.AppendChild(dataRow);
            }

            // Remark row
            var remarkRow = new TableRow();
            int fixedColCount = modifiedDt.Columns.Count - resultSubCols;
            remarkRow.AppendChild(MakeRemarkCell("Remarks", colWidths[0], true));
            for (int i = 1; i < fixedColCount; i++)
                remarkRow.AppendChild(MakeRemarkCell("", colWidths[i], false));

            for (int i = 0; i < resultSubCols; i++)
            {
                bool fail = i < colorCodeHasDetection.Length && colorCodeHasDetection[i];
                remarkRow.AppendChild(MakeRemarkCell(fail ? "FAIL" : "PASS", colWidths[fixedColCount + i], true, fail ? "FF0000" : "008000"));
            }
            table.AppendChild(remarkRow);

            return table;
        }

        // ========== SHARED HELPERS ==========//

        /// <summary>
        /// Finds the first paragraph whose concatenated run text contains the placeholder,
        /// even if Word has split it across multiple runs.
        /// </summary>
        private Paragraph FindParagraphContaining(Body body, string placeholder)
        {
            return body.Descendants<Paragraph>()
                .FirstOrDefault(p => string.Concat(p.Descendants<WordText>().Select(t => t.Text))
                    .Contains(placeholder));
        }

        /// <summary>
        /// Header cell with real multi-line support: each array entry becomes its own
        /// paragraph line inside the cell, joined by <w:br/> (not "\n", which OpenXML
        /// renders as a literal backslash-n instead of a line break).
        /// </summary>
        private TableCell MakeHeaderCell(string[] lines, int width, string fontSize = "20")
        {
            var cell = new TableCell();
            cell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
                new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
            ));

            var para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });

            var run = new Run();
            run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = fontSize }));
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0) run.AppendChild(new Break());
                run.AppendChild(new WordText(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
            }
            para.AppendChild(run);
            cell.AppendChild(para);
            return cell;
        }

        private TableCell MakeDataCell(string text, int width, bool leftAlign = false, string fontSize = "20")
        {
            var cell = new TableCell();
            cell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ));

            var para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(
                new Justification { Val = leftAlign ? JustificationValues.Left : JustificationValues.Center }
            );
            var run = new Run();
            run.AppendChild(new RunProperties(new FontSize { Val = fontSize }));
            run.AppendChild(new WordText(text) { Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(run);
            cell.AppendChild(para);
            return cell;
        }

        private TableCell MakeRemarkCell(string text, int width, bool isBold, string colorHex = null, string fontSize = "20")
        {
            var cell = new TableCell();
            cell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
                new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
            ));

            var para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
            var run = new Run();
            var runProps = new RunProperties(new FontSize { Val = fontSize });
            if (isBold) runProps.AppendChild(new Bold());
            if (!string.IsNullOrEmpty(colorHex))
                runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = colorHex });
            run.AppendChild(runProps);
            run.AppendChild(new WordText(text) { Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(run);
            cell.AppendChild(para);
            return cell;
        }

        // ========== SAMPLE DESCRIPTION TABLE ==========//
        private void CreateTableforSampleDescription(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    FindAndReplace(doc, placeholder, "");
                    return;
                }

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                int colCount = 3;
                int tableWidth = USABLE_WIDTH_TWIPS;
                int baseColWidth = tableWidth / colCount;
                int remainder = tableWidth - (baseColWidth * colCount);
                List<int> colWidths = new List<int>();
                for (int i = 0; i < colCount; i++)
                    colWidths.Add(i == colCount - 1 ? baseColWidth + remainder : baseColWidth);

                var table = new Table();

                var tableProps = new TableProperties();
                var borders = new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                );
                tableProps.AppendChild(borders);
                tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });
                tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
                table.AppendChild(tableProps);

                var grid = new TableGrid();
                foreach (int w in colWidths)
                    grid.AppendChild(new GridColumn { Width = w.ToString() });
                table.AppendChild(grid);

                // Header row
                var headerRow = new TableRow();
                string[] headers = { "Sr. No.", "Sample Description", "Sample ID" };
                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    var cellProps = new TableCellProperties();
                    cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                    cellProps.AppendChild(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" });
                    cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                    cell.AppendChild(cellProps);

                    var para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
                    var run = new Run();
                    run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    run.AppendChild(new WordText(headers[i]) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    headerRow.AppendChild(cell);
                }
                table.AppendChild(headerRow);

                // Data rows
                int rowNum = 1;
                foreach (DataRow row in dt.Rows)
                {
                    var dataRow = new TableRow();
                    for (int i = 0; i < colCount; i++)
                    {
                        var cell = new TableCell();
                        var cellProps = new TableCellProperties();
                        cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                        cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                        cell.AppendChild(cellProps);

                        var para = new Paragraph();
                        para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
                        var run = new Run();
                        run.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        string text = i == 0 ? rowNum.ToString() : row[i - 1]?.ToString() ?? "";
                        run.AppendChild(new WordText(text) { Space = SpaceProcessingModeValues.Preserve });
                        para.AppendChild(run);
                        cell.AppendChild(para);
                        dataRow.AppendChild(cell);
                    }
                    table.AppendChild(dataRow);
                    rowNum++;
                }

                targetParagraph.Parent.InsertAfter(table, targetParagraph);
                var breakParagraph = new Paragraph(new Run());
                targetParagraph.Parent.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating Sample Description table: {ex.Message}");
            }
        }

        // ========== IMAGE TABLE 21 ==========//        
        private void InsertImagesIntoDocument21(WordprocessingDocument doc, string placeholder, List<Tuple<string, string>> imageData)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            ReplaceInParagraph(targetParagraph, placeholder, "");

            if (imageData == null || imageData.Count == 0) return;

            int imagesPerRow = 2;
            int totalImages = imageData.Count;
            int rows = (int)Math.Ceiling((double)totalImages / imagesPerRow);
            var table = CreateImageTable(mainPart, imageData, rows, imagesPerRow);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private Table CreateImageTable(MainDocumentPart mainPart, List<Tuple<string, string>> imageData, int rows, int imagesPerRow)
        {
            var table = new Table();

            int usablePageWidthTwips = 9356;
            int colWidthTwips = usablePageWidthTwips / imagesPerRow;

            // Special case: only one image in total — no border, no merge, left aligned
            bool isSingleImageTotal = imageData.Count == 1;

            // Table properties with borders (skip borders entirely for single-image case)
            TableProperties tableProps;
            if (isSingleImageTotal)
            {
                tableProps = new TableProperties(
                    new TableLayout() { Type = TableLayoutValues.Fixed },
                    new TableWidth() { Width = usablePageWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa }
                );
            }
            else
            {
                tableProps = new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                        new RightBorder() { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                    ),
                    new TableLayout() { Type = TableLayoutValues.Fixed },
                    new TableWidth() { Width = usablePageWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa }
                );
            }
            table.AppendChild(tableProps);

            var grid = new TableGrid();
            for (int i = 0; i < imagesPerRow; i++)
            {
                grid.AppendChild(new GridColumn() { Width = colWidthTwips.ToString() });
            }
            table.AppendChild(grid);

            long maxPhotoWidthEmu = 2400000L;
            long maxPhotoHeightEmu = 2000000L;

            int imageIndex = 0;
            for (int row = 0; row < rows; row++)
            {
                var tableRow = new TableRow();

                bool isLastRow = (row == rows - 1);
                int remainingImages = imageData.Count - imageIndex;
                bool isOddLastRow = isLastRow && (remainingImages % imagesPerRow != 0);
                int cellsInThisRow = Math.Min(imagesPerRow, remainingImages);

                int columnsToCreate = isSingleImageTotal ? 1 : (isOddLastRow ? 1 : imagesPerRow);

                for (int col = 0; col < columnsToCreate; col++)
                {
                    var cell = new TableCell();

                    var cellProps = new TableCellProperties();

                    if (isSingleImageTotal)
                    {
                        cellProps.AppendChild(new TableCellWidth() { Width = colWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa });
                    }
                    else if (isOddLastRow && col == 0)
                    {
                        cellProps.AppendChild(new TableCellWidth() { Width = "100", Type = TableWidthUnitValues.Pct });
                        cellProps.AppendChild(new GridSpan() { Val = imagesPerRow });

                        cellProps.AppendChild(new TableCellBorders(
                            new TopBorder() { Val = BorderValues.Single, Size = 4 },
                            new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                            new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                            new RightBorder() { Val = BorderValues.Single, Size = 4 }
                        ));
                    }
                    else
                    {
                        cellProps.AppendChild(new TableCellWidth() { Width = colWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa });

                        cellProps.AppendChild(new TableCellBorders(
                            new TopBorder() { Val = BorderValues.Single, Size = 4 },
                            new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                            new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                            new RightBorder() { Val = BorderValues.Single, Size = 4 }
                        ));
                    }

                    cell.AppendChild(cellProps);

                    if (imageIndex < imageData.Count)
                    {
                        string imagePath = imageData[imageIndex].Item1;
                        string imageName = imageData[imageIndex].Item2;

                        if (IsUsableImageFile(imagePath))
                        {
                            var imagePart = mainPart.AddImagePart(GetImagePartTypeForFile(imagePath));
                            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                imagePart.FeedData(stream);
                            }

                            GetScaledEmuSize(imagePath, maxPhotoWidthEmu, maxPhotoHeightEmu, out long picWidth, out long picHeight);

                            var element = CreateImageElement(mainPart.GetIdOfPart(imagePart), "Image " + (imageIndex + 1), picWidth, picHeight);

                            if (isSingleImageTotal)
                            {
                                // Build a nested single-cell table sized to the IMAGE'S OWN width.
                                // Centering the caption inside this narrow table makes the caption
                                // align to the image's width instead of the whole (wider) outer cell.
                                long nestedWidthTwips = picWidth / 635; // EMU -> twips
                                if (nestedWidthTwips < 500) nestedWidthTwips = 500; // safety minimum

                                var nestedTable = new Table();

                                var nestedTableProps = new TableProperties(
                                    new TableWidth() { Width = nestedWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa },
                                    new TableJustification() { Val = TableRowAlignmentValues.Left }
                                );
                                nestedTable.AppendChild(nestedTableProps);

                                var nestedGrid = new TableGrid();
                                nestedGrid.AppendChild(new GridColumn() { Width = nestedWidthTwips.ToString() });
                                nestedTable.AppendChild(nestedGrid);

                                var nestedRow = new TableRow();
                                var nestedCell = new TableCell();
                                nestedCell.AppendChild(new TableCellProperties(
                                    new TableCellWidth() { Width = nestedWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa }
                                ));

                                // Image paragraph, centered within the narrow nested cell
                                var imageParagraph = new Paragraph();
                                imageParagraph.ParagraphProperties = new ParagraphProperties(
                                    new Justification() { Val = JustificationValues.Center }
                                );
                                var imageRun = new Run();
                                imageRun.AppendChild(element);
                                imageParagraph.AppendChild(imageRun);
                                nestedCell.AppendChild(imageParagraph);

                                // Caption paragraph, centered within the same narrow nested cell
                                // -> lines up with the image's center, not the outer cell's center
                                var captionParagraph = new Paragraph();
                                captionParagraph.ParagraphProperties = new ParagraphProperties(
                                    new Justification() { Val = JustificationValues.Center }
                                );
                                var captionRunSingle = new Run();
                                captionRunSingle.AppendChild(new RunProperties(
                                    new FontSize() { Val = "20" },
                                    new RunFonts() { Ascii = "Arial" },
                                    new Bold()
                                ));
                                captionRunSingle.AppendChild(new WordText() { Text = imageName, Space = SpaceProcessingModeValues.Preserve });
                                captionParagraph.AppendChild(captionRunSingle);
                                nestedCell.AppendChild(captionParagraph);

                                nestedRow.AppendChild(nestedCell);
                                nestedTable.AppendChild(nestedRow);

                                cell.AppendChild(nestedTable);
                                // A table cell must not end directly with a table; append a trailing empty paragraph
                                cell.AppendChild(new Paragraph());
                            }
                            else
                            {
                                // Original behavior: image + caption in same paragraph, both centered
                                var paragraph = new Paragraph();
                                paragraph.ParagraphProperties = new ParagraphProperties(
                                    new Justification() { Val = JustificationValues.Center }
                                );

                                var run = new Run();
                                run.AppendChild(element);
                                paragraph.AppendChild(run);

                                var captionRun = new Run();
                                captionRun.AppendChild(new RunProperties(
                                    new FontSize() { Val = "20" },
                                    new RunFonts() { Ascii = "Arial" },
                                    new Bold()
                                ));
                                captionRun.AppendChild(new Break());
                                captionRun.AppendChild(new WordText() { Text = imageName, Space = SpaceProcessingModeValues.Preserve });
                                paragraph.AppendChild(captionRun);

                                cell.AppendChild(paragraph);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Sample image missing or unreadable, leaving cell blank: {imagePath}");
                            cell.AppendChild(new Paragraph());
                        }
                        imageIndex++;
                    }
                    else
                    {
                        cell.AppendChild(new Paragraph());
                    }

                    tableRow.AppendChild(cell);
                }

                table.AppendChild(tableRow);
            }

            return table;
        }

        // ========== TABLE 22 ==========//
        private void CreateTable22(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                // Group by TestParameter
                var groupedByTestParam = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("TestParameter"))
                    .ToList();

                // Build table rows
                var tableRows = new List<TableRowInfo>();
                int testParamNumber = 1;

                foreach (var testParamGroup in groupedByTestParam)
                {
                    var testParameter = testParamGroup.Key;
                    var firstRow = testParamGroup.First();

                    tableRows.Add(new TableRowInfo
                    {
                        RowNumber = testParamNumber,
                        TestParameter = testParameter,
                        DisplayValue = testParameter,
                        DataRow = firstRow,
                        ShowRowNumber = true,
                        Level = 1
                    });

                    var subParamGroups = testParamGroup
                        .GroupBy(row => new
                        {
                            SubParam = row.Field<string>("TestSubParameter") ?? "",
                            SubSubParam = row.Field<string>("TestSubSubParameter") ?? ""
                        })
                        .Select(g => new { g.Key.SubParam, g.Key.SubSubParam, Rows = g.ToList() })
                        .OrderBy(x => x.SubParam)
                        .ThenBy(x => x.SubSubParam)
                        .ToList();

                    foreach (var item in subParamGroups)
                    {
                        if (!string.IsNullOrWhiteSpace(item.SubSubParam))
                        {
                            if (!string.IsNullOrWhiteSpace(item.SubParam))
                            {
                                bool parentExists = tableRows.Any(r =>
                                    r.Level == 2 &&
                                    r.SubParameter == item.SubParam &&
                                    r.RowNumber == testParamNumber);

                                if (!parentExists)
                                {
                                    var parentRow = testParamGroup.FirstOrDefault(r =>
                                        r.Field<string>("TestSubParameter") == item.SubParam);

                                    if (parentRow != null)
                                    {
                                        tableRows.Add(new TableRowInfo
                                        {
                                            RowNumber = testParamNumber,
                                            TestParameter = testParameter,
                                            SubParameter = item.SubParam,
                                            DisplayValue = item.SubParam,
                                            DataRow = parentRow,
                                            ShowRowNumber = false,
                                            Level = 2
                                        });
                                    }
                                }
                            }

                            tableRows.Add(new TableRowInfo
                            {
                                RowNumber = testParamNumber,
                                TestParameter = testParameter,
                                SubParameter = item.SubParam,
                                SubSubParameter = item.SubSubParam,
                                DisplayValue = item.SubSubParam,
                                DataRow = item.Rows.First(),
                                ShowRowNumber = false,
                                Level = 3
                            });
                        }
                        else if (!string.IsNullOrWhiteSpace(item.SubParam))
                        {
                            bool exists = tableRows.Any(r =>
                                r.Level == 2 &&
                                r.SubParameter == item.SubParam &&
                                r.RowNumber == testParamNumber);

                            if (!exists)
                            {
                                tableRows.Add(new TableRowInfo
                                {
                                    RowNumber = testParamNumber,
                                    TestParameter = testParameter,
                                    SubParameter = item.SubParam,
                                    DisplayValue = item.SubParam,
                                    DataRow = item.Rows.First(),
                                    ShowRowNumber = false,
                                    Level = 2
                                });
                            }
                        }
                    }

                    testParamNumber++;
                }

                if (tableRows.Count == 0) return;

                // Create table
                int totalRows = tableRows.Count + 1;
                int colCount = 5;
                int tableWidth = USABLE_WIDTH_TWIPS;
                List<int> colWidths = new List<int> { 800, 4000, 2000, 2500, 2000 };

                var table = new Table();

                var tableProps = new TableProperties();
                var borders = new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                );
                tableProps.AppendChild(borders);
                tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });
                tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
                table.AppendChild(tableProps);

                var grid = new TableGrid();
                foreach (int w in colWidths)
                    grid.AppendChild(new GridColumn { Width = w.ToString() });
                table.AppendChild(grid);

                // Header row
                var headerRow = new TableRow();
                string[] headers = { "#", "Test Type", "Unit", "Test Method", "Test Results" };
                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa },
                        new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));

                    var para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
                    var run = new Run();
                    run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    run.AppendChild(new WordText(headers[i]) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    headerRow.AppendChild(cell);
                }
                table.AppendChild(headerRow);

                // Data rows
                foreach (var rowInfo in tableRows)
                {
                    var dataRow = new TableRow();

                    // No column
                    var noCell = new TableCell();
                    noCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[0].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));
                    var noPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                    noPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                        new WordText(rowInfo.ShowRowNumber ? rowInfo.RowNumber.ToString() + "." : "") { Space = SpaceProcessingModeValues.Preserve }));
                    noCell.AppendChild(noPara);
                    dataRow.AppendChild(noCell);

                    // Test Type column
                    var testTypeCell = new TableCell();
                    testTypeCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[1].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));

                    string displayText = "";
                    switch (rowInfo.Level)
                    {
                        case 1: displayText = rowInfo.TestParameter; break;
                        case 2: displayText = "  • " + rowInfo.SubParameter; break;
                        case 3: displayText = "    - " + rowInfo.SubSubParameter; break;
                    }

                    var testTypePara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Left }));
                    var testTypeRun = new Run(new RunProperties(new FontSize { Val = "20" }));
                    if (rowInfo.Level == 1) testTypeRun.RunProperties.AppendChild(new Bold());
                    testTypeRun.AppendChild(new WordText(displayText) { Space = SpaceProcessingModeValues.Preserve });
                    testTypePara.AppendChild(testTypeRun);
                    testTypeCell.AppendChild(testTypePara);
                    dataRow.AppendChild(testTypeCell);

                    // Unit column
                    var unitCell = new TableCell();
                    unitCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[2].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));
                    var unitPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                    unitPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                        new WordText(rowInfo.DataRow["UOM"]?.ToString() ?? "") { Space = SpaceProcessingModeValues.Preserve }));
                    unitCell.AppendChild(unitPara);
                    dataRow.AppendChild(unitCell);

                    // Method column
                    var methodCell = new TableCell();
                    methodCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[3].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));
                    var methodPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Left }));
                    methodPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                        new WordText(rowInfo.DataRow["Method*"]?.ToString() ?? "") { Space = SpaceProcessingModeValues.Preserve }));
                    methodCell.AppendChild(methodPara);
                    dataRow.AppendChild(methodCell);

                    // Result column
                    var resultCell = new TableCell();
                    resultCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[4].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));
                    string resultText = rowInfo.DataRow["Result(mg/kg)"]?.ToString() ?? "";
                    resultText = FormatTestResult(resultText);
                    var resultPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                    resultPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                        new WordText(resultText) { Space = SpaceProcessingModeValues.Preserve }));
                    resultCell.AppendChild(resultPara);
                    dataRow.AppendChild(resultCell);

                    table.AppendChild(dataRow);
                }

                targetParagraph.Parent.InsertAfter(table, targetParagraph);
                var afterParagraph = new Paragraph(new Run(new Break()));
                targetParagraph.Parent.InsertAfter(afterParagraph, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTable22: {ex.Message}");
            }
        }

        private string FormatTestResult(string result)
        {
            if (string.IsNullOrEmpty(result)) return "";
            if (result.Contains("(") && result.Contains(")")) return result;
            if (result.Contains(" "))
            {
                var parts = result.Split(' ');
                if (parts.Length >= 2)
                {
                    return $"{parts[0]} ({parts[1]})";
                }
            }
            return result;
        }

        private class TableRowInfo
        {
            public int RowNumber { get; set; }
            public string TestParameter { get; set; }
            public string SubParameter { get; set; }
            public string SubSubParameter { get; set; }
            public string DisplayValue { get; set; }
            public DataRow DataRow { get; set; }
            public bool ShowRowNumber { get; set; }
            public int Level { get; set; }
        }
                
        private string BuildGroupMethodText_23(IEnumerable<DataRow> groupRows)
        {
            var methods = groupRows
                .Select(r => r["Method*"]?.ToString()?.Trim() ?? "")
                .Where(m => !string.IsNullOrEmpty(m))
                .Select(m => CleanMethodText_24(m)) // strips any TSQ-LAB tail / ASTM / short junk per value
                .SelectMany(m => m.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct()
                .ToList();

            return string.Join(", ", methods);
        }

        // ========== TABLE 23 (EN) ==========
        private void CreateTable23(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Parameter"))
                    .ToList();

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                Paragraph currentParagraph = targetParagraph;
                bool isFirstGroup = true;
                int tableNumber = 0;

                foreach (var testParamGroup in groupedData)
                {
                    tableNumber++;
                    var groupRowsList = testParamGroup.ToList();
                    var firstRow = groupRowsList.First();

                    // FIX: build method text from ALL rows in the group (union + dedupe),
                    // not just firstRow — matches the working CreateTable18 pattern.
                    string methodText = BuildGroupMethodText_23(groupRowsList);

                    if (isFirstGroup)
                    {
                        FindAndReplace(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());
                        FindAndReplace(doc, "{TestMethod}", methodText);
                        FindAndReplace(doc, "{Equipment}", firstRow["Equipment"].ToString());
                    }

                    if (!isFirstGroup && currentParagraph != null)
                    {
                        var spacingPara = MakeSpacerParagraph("60");
                        currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                        currentParagraph = spacingPara;

                        var headerPara = new Paragraph();
                        headerPara.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines { Before = "60", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                            new ParagraphMarkRunProperties(new FontSize { Val = "22" })
                        );
                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                        headerRun.AppendChild(new WordText($"{tableNumber}. {firstRow["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                        headerPara.AppendChild(headerRun);
                        currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                        currentParagraph = headerPara;

                        var methodPara = new Paragraph();
                        methodPara.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines { Before = "0", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                            new ParagraphMarkRunProperties(new FontSize { Val = "20" })
                        );
                        var methodRun = new Run();
                        methodRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        methodRun.AppendChild(new WordText($"Test Method: {methodText} - Analyzed by {firstRow["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                        methodPara.AppendChild(methodRun);
                        currentParagraph.Parent.InsertAfter(methodPara, currentParagraph);
                        currentParagraph = methodPara;
                    }

                    if (currentParagraph != null)
                    {
                        var table = BuildTable23(groupRowsList, WrapRequirementText_23);
                        if (table != null)
                        {
                            currentParagraph.Parent.InsertAfter(table, currentParagraph);

                            var afterTablePara = MakeSpacerParagraph("60");
                            table.Parent.InsertAfter(afterTablePara, table);
                            currentParagraph = afterTablePara;
                        }

                        // MDL line
                        var detectionLimits = new List<string>();
                        var processedElements = new HashSet<string>();
                        foreach (var row in groupRowsList)
                        {
                            string element = row["Element"]?.ToString() ?? "";
                            string mdl = row["MDL"]?.ToString() ?? "";
                            if (!processedElements.Contains(element) && !string.IsNullOrWhiteSpace(mdl) && mdl != "Not Detected")
                            {
                                detectionLimits.Add($"{element}: {mdl} (mg/kg)");
                                processedElements.Add(element);
                            }
                        }

                        if (detectionLimits.Count > 0 && currentParagraph != null)
                        {
                            var mdlRun = new Run();
                            mdlRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                            mdlRun.AppendChild(new WordText($"Method Detection Limit= {string.Join(", ", detectionLimits)}") { Space = SpaceProcessingModeValues.Preserve });
                            currentParagraph.AppendChild(mdlRun);

                            var nextPara = MakeSpacerParagraph("80");
                            currentParagraph.Parent.InsertAfter(nextPara, currentParagraph);
                            currentParagraph = nextPara;
                        }
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTable23: {ex.Message}");
            }
        }

        // ========== TABLE 23 (BH) ==========
        private void CreateTable23_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Parameter"))
                    .ToList();

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                Paragraph currentParagraph = targetParagraph;
                bool isFirstGroup = true;
                int tableNumber = 0;

                foreach (var testParamGroup in groupedData)
                {
                    tableNumber++;
                    var groupRowsList = testParamGroup.ToList();
                    var firstRow = groupRowsList.First();

                    // FIX: same union+dedupe pattern as CreateTable23 (EN)
                    string methodText = BuildGroupMethodText_23(groupRowsList);

                    if (isFirstGroup)
                    {
                        FindAndReplace(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());
                        FindAndReplace(doc, "{TestMethod}", methodText);
                        FindAndReplace(doc, "{Equipment}", firstRow["Equipment"].ToString());
                    }

                    if (!isFirstGroup && currentParagraph != null)
                    {
                        var spacingPara = MakeSpacerParagraph("60");
                        currentParagraph.Parent.InsertAfter(spacingPara, currentParagraph);
                        currentParagraph = spacingPara;

                        var headerPara = new Paragraph();
                        headerPara.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines { Before = "60", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                            new ParagraphMarkRunProperties(new FontSize { Val = "22" })
                        );
                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                        headerRun.AppendChild(new WordText($"{tableNumber}. {firstRow["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                        headerPara.AppendChild(headerRun);
                        currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                        currentParagraph = headerPara;

                        var methodPara = new Paragraph();
                        methodPara.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines { Before = "0", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                            new ParagraphMarkRunProperties(new FontSize { Val = "20" })
                        );
                        var methodRun = new Run();
                        methodRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        methodRun.AppendChild(new WordText($"Test Method: {methodText} - Analyzed by {firstRow["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                        methodPara.AppendChild(methodRun);
                        currentParagraph.Parent.InsertAfter(methodPara, currentParagraph);
                        currentParagraph = methodPara;
                    }

                    if (currentParagraph != null)
                    {
                        var table = BuildTable23(groupRowsList, WrapRequirementText_23_BH);
                        if (table != null)
                        {
                            currentParagraph.Parent.InsertAfter(table, currentParagraph);

                            var afterTablePara = MakeSpacerParagraph("60");
                            table.Parent.InsertAfter(afterTablePara, table);
                            currentParagraph = afterTablePara;
                        }

                        var detectionLimits = new List<string>();
                        var processedElements = new HashSet<string>();
                        foreach (var row in groupRowsList)
                        {
                            string element = row["Element"]?.ToString() ?? "";
                            string mdl = row["MDL"]?.ToString() ?? "";
                            if (!processedElements.Contains(element) && !string.IsNullOrWhiteSpace(mdl) && mdl != "Not Detected")
                            {
                                detectionLimits.Add($"{element}: {mdl} (mg/kg)");
                                processedElements.Add(element);
                            }
                        }

                        if (detectionLimits.Count > 0 && currentParagraph != null)
                        {
                            var mdlRun = new Run();
                            mdlRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                            mdlRun.AppendChild(new WordText($"Method Detection Limit= {string.Join(", ", detectionLimits)}") { Space = SpaceProcessingModeValues.Preserve });
                            currentParagraph.AppendChild(mdlRun);

                            var nextPara = MakeSpacerParagraph("80");
                            currentParagraph.Parent.InsertAfter(nextPara, currentParagraph);
                            currentParagraph = nextPara;
                        }
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTable23_BH: {ex.Message}");
            }
        }
               
        private Table BuildTable23(List<DataRow> rows, Func<string, string> wrapRequirementText)
        {
            int colCount = 3;
            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);
            tableProps.AppendChild(new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            List<int> colWidths = new List<int> { 4000, 2500, 2500 };
            var grid = new TableGrid();
            foreach (int w in colWidths)
                grid.AppendChild(new GridColumn { Width = w.ToString() });
            table.AppendChild(grid);

            // Header row — uses Break() for the two-line labels instead of "\n"
            var headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(new TableRowHeight { Val = 800U, HeightType = HeightRuleValues.Exact }));
            headerRow.AppendChild(MakeHeaderCell(new[] { "Element/Compound" }, colWidths[0]));
            headerRow.AppendChild(MakeHeaderCell(new[] { "Result", "(mg/kg)" }, colWidths[1]));
            headerRow.AppendChild(MakeHeaderCell(new[] { "Specification", "(mg/kg)" }, colWidths[2]));
            table.AppendChild(headerRow);

            // Data rows
            foreach (var row in rows)
            {
                var dataRow = new TableRow();

                dataRow.AppendChild(MakeDataCell(row["Element"]?.ToString() ?? "", colWidths[0], leftAlign: true));

                string result = row["Result(mg/kg)"]?.ToString() ?? "N.D";
                dataRow.AppendChild(MakeDataCell(result, colWidths[1]));

                string requirementText = wrapRequirementText(row["Requirement"]?.ToString() ?? "");
                dataRow.AppendChild(MakeWrappedDataCell(requirementText, colWidths[2]));

                table.AppendChild(dataRow);
            }

            return table;
        }
               
        private TableCell MakeWrappedDataCell(string text, int width, string fontSize = "20")
        {
            var cell = new TableCell();
            cell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ));

            var para = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
            var run = new Run();
            run.AppendChild(new RunProperties(new FontSize { Val = fontSize }));

            var lines = (text ?? "").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0) run.AppendChild(new Break());
                run.AppendChild(new WordText(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
            }
            para.AppendChild(run);
            cell.AppendChild(para);
            return cell;
        }

        private string WrapRequirementText_23(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string[] breakPoints = { " to ", " total ", " mg/kg", " Maximum ", " limit ", " and ", " or " };
            foreach (string breakPoint in breakPoints)
            {
                int index = text.IndexOf(breakPoint, StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                {
                    return text.Substring(0, index + breakPoint.Length).Trim() + "\n" +
                           text.Substring(index + breakPoint.Length).Trim();
                }
            }

            int middle = text.Length / 2;
            int spaceIndex = text.LastIndexOf(' ', middle);
            if (spaceIndex > 0)
                return text.Substring(0, spaceIndex) + "\n" + text.Substring(spaceIndex + 1);

            return text;
        }

        private string WrapRequirementText_23_BH(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string[] breakPoints = { " to ", " total ", " mg/kg", " Maximum ", " limit ", " and ", " or " };
            foreach (string breakPoint in breakPoints)
            {
                int index = text.IndexOf(breakPoint, StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                {
                    return text.Substring(0, index + breakPoint.Length).Trim() + "\n" +
                           text.Substring(index + breakPoint.Length).Trim();
                }
            }

            int middle = text.Length / 2;
            int spaceIndex = text.LastIndexOf(' ', middle);
            if (spaceIndex > 0)
                return text.Substring(0, spaceIndex) + "\n" + text.Substring(spaceIndex + 1);

            return text;
        }

        // ========== TIGHT SPACER / HEADER PARAGRAPH HELPER ==========
        // Explicit spacing (Before/After) + explicit ParagraphMarkRunProperties font size
        // prevents Word from falling back to the Normal style's default spacing/line-height,
        // which was causing the large gaps between grouped tables in CreateTable24.
        private Paragraph MakeSpacerParagraph(string afterTwips = "40", string fontSize = "20")
        {
            var para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(
                new SpacingBetweenLines
                {
                    Before = "0",
                    After = afterTwips,
                    Line = "240",
                    LineRule = LineSpacingRuleValues.Auto
                },
                new ParagraphMarkRunProperties(new FontSize { Val = fontSize })
            );
            return para;
        }

        // ========== TABLE 24 (EN) ==========
        private void CreateTable24(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var cleanedRows = new List<DataRow>();
                foreach (DataRow row in dt.Rows)
                {
                    string testParam = row.Field<string>("TestParameter");
                    if (!string.IsNullOrEmpty(testParam) && !testParam.Contains("{TestParameter}") && testParam != "1. {TestParameter}")
                    {
                        cleanedRows.Add(row);
                    }
                }

                if (cleanedRows.Count == 0) return;
                var testParameters = cleanedRows.Select(r => r.Field<string>("TestParameter")).Where(tp => !string.IsNullOrEmpty(tp)).Distinct().ToList();
                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;
                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;
                RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                Paragraph currentParagraph = targetParagraph;
                int currentTableNumber = GetCurrentMaxTableNumber(doc) + 1;

                foreach (var testParam in testParameters)
                {
                    var testParamRows = cleanedRows.Where(r => r.Field<string>("TestParameter") == testParam).ToList();

                    // Header — tight explicit spacing (was inheriting Normal style's default spacing)
                    var headerPara = new Paragraph();
                    headerPara.ParagraphProperties = new ParagraphProperties(
                        new SpacingBetweenLines { Before = "120", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                        new ParagraphMarkRunProperties(new FontSize { Val = "22" })
                    );
                    var headerRun = new Run();
                    headerRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    headerRun.AppendChild(new WordText($"{currentTableNumber}. {testParam}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                    currentParagraph = headerPara;

                    // Methods — dedup + ASTM/short-token filtering via CleanMethodText_24
                    var testMethods = testParamRows.Select(r => CleanMethodText_24(r.Field<string>("Method*") ?? ""))
                        .SelectMany(m => m.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)).Where(m => !string.IsNullOrEmpty(m)).Distinct().ToList();

                    if (testMethods.Count > 0)
                    {
                        var methodPara = new Paragraph();
                        methodPara.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines { Before = "0", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                            new ParagraphMarkRunProperties(new FontSize { Val = "20" })
                        );
                        var methodRun = new Run();
                        methodRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        methodRun.AppendChild(new WordText($"Test Method: {string.Join(", ", testMethods)}") { Space = SpaceProcessingModeValues.Preserve });
                        methodPara.AppendChild(methodRun);
                        currentParagraph.Parent.InsertAfter(methodPara, currentParagraph);
                        currentParagraph = methodPara;
                    }

                    bool hasSubParameters = testParamRows.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));

                    if (hasSubParameters)
                    {
                        var subParamGroups = testParamRows.GroupBy(r => r.Field<string>("TestSubParameter") ?? "Unknown").ToList();
                        foreach (var subParamGroup in subParamGroups)
                        {
                            string subParam = subParamGroup.Key;

                            if (!string.IsNullOrEmpty(subParam))
                            {
                                var subHeaderPara = new Paragraph();
                                subHeaderPara.ParagraphProperties = new ParagraphProperties(
                                    new SpacingBetweenLines { Before = "60", After = "40", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                                    new ParagraphMarkRunProperties(new FontSize { Val = "22" })
                                );
                                var subHeaderRun = new Run();
                                subHeaderRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                                subHeaderRun.AppendChild(new WordText(subParam) { Space = SpaceProcessingModeValues.Preserve });
                                subHeaderPara.AppendChild(subHeaderRun);
                                currentParagraph.Parent.InsertAfter(subHeaderPara, currentParagraph);
                                currentParagraph = subHeaderPara;
                            }

                            var uniqueUOMs = subParamGroup.Select(r => r.Field<string>("UOM")).Where(u => !string.IsNullOrEmpty(u)).Distinct().ToList();
                            var table = uniqueUOMs.Count > 1 ? BuildMultiColumnTable24(subParamGroup.ToList(), uniqueUOMs)
                                : BuildSimpleTable24(subParamGroup.ToList(), uniqueUOMs.FirstOrDefault() ?? "");

                            if (table != null)
                            {
                                currentParagraph.Parent.InsertAfter(table, currentParagraph);

                                // FIX: single tight spacer after the table (was: afterTablePara + a second
                                // Break()-paragraph, which doubled the gap and used default spacing).
                                var afterTablePara = MakeSpacerParagraph("60");
                                table.Parent.InsertAfter(afterTablePara, table);
                                currentParagraph = afterTablePara;
                            }
                            // Removed the redundant extra spacingPara/Break() paragraph that used to
                            // follow here — MakeSpacerParagraph above already provides the gap.
                        }
                    }
                    else
                    {
                        string uom = testParamRows.Select(r => r.Field<string>("UOM")).FirstOrDefault(u => !string.IsNullOrEmpty(u)) ?? "";
                        var table = BuildSimpleTable24(testParamRows, uom);
                        if (table != null)
                        {
                            currentParagraph.Parent.InsertAfter(table, currentParagraph);
                            var afterTablePara = MakeSpacerParagraph("60");
                            table.Parent.InsertAfter(afterTablePara, table);
                            currentParagraph = afterTablePara;
                        }
                    }

                    // Tight spacer between whole test-parameter sections (replaces old Break()-paragraph)
                    var breakPara = MakeSpacerParagraph("120");
                    currentParagraph.Parent.InsertAfter(breakPara, currentParagraph);
                    currentParagraph = breakPara;
                    currentTableNumber++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateTable24: {ex.Message}");
            }
        }

        // ========== TABLE 24 (BH) ==========
        // Interop's CreateTable24_BH is a verbatim duplicate of CreateTable24 (same logic, no BH-specific
        // text handling was ever added), so this delegates rather than re-implementing identical code.
        private void CreateTable24_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            CreateTable24(doc, placeholder, dt);
        }

        // ========== TABLE NUMBERING HELPER ==========
        private int GetCurrentMaxTableNumber(WordprocessingDocument doc)
        {
            int maxNumber = 0;
            try
            {
                var body = doc.MainDocumentPart.Document.Body;
                var regex = new Regex(@"^(\d+)\.\s+");
                foreach (var para in body.Descendants<Paragraph>())
                {
                    string text = GetParagraphFullText(para);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var match = regex.Match(text.Trim());
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                        {
                            if (num > maxNumber) maxNumber = num;
                        }
                    }
                }
            }
            catch { }
            return maxNumber;
        }

        // ========== METHOD TEXT CLEANING (Table 24 specific) ==========
        private string CleanMethodText_24(string methodText)
        {
            if (string.IsNullOrWhiteSpace(methodText))
                return string.Empty;

            methodText = methodText.Trim();

            if (methodText.Contains("TSQ-LAB"))
            {
                int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                if (colorCodeStart > 0)
                    methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
            }

            methodText = Regex.Replace(methodText, @",\s*,", ",");

            var methods = methodText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrEmpty(m) && m != "ASTM" && m.Length > 2)
                .Distinct()
                .ToList();

            for (int i = 0; i < methods.Count; i++)
            {
                if (methods[i] == "ASTM" || methods[i].Length < 4)
                {
                    methods.RemoveAt(i);
                    i--;
                }
            }

            return string.Join(", ", methods);
        }

        // ========== SIMPLE TABLE (single UOM) ==========
        private Table BuildSimpleTable24(List<DataRow> rows, string uom)
        {
            var specimenData = new Dictionary<int, string>();
            foreach (var row in rows)
            {
                string specimenStr = row.Field<string>("Specimen") ?? "Specimen 1";
                int specimenNum = 1;
                if (specimenStr.StartsWith("Specimen "))
                    int.TryParse(specimenStr.Replace("Specimen ", ""), out specimenNum);

                string result = row.Field<string>("Result") ?? "";
                if (!string.IsNullOrEmpty(result))
                    specimenData[specimenNum] = result;
            }

            if (specimenData.Count == 0) return null;

            int colCount = 2;
            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);
            tableProps.AppendChild(new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            List<int> colWidths = new List<int> { (int)(USABLE_WIDTH_TWIPS * 0.3), (int)(USABLE_WIDTH_TWIPS * 0.7) };
            var grid = new TableGrid();
            foreach (int w in colWidths)
                grid.AppendChild(new GridColumn { Width = w.ToString() });
            table.AppendChild(grid);

            // Header
            var headerRow = new TableRow();
            string headerText = "Result";
            if (!string.IsNullOrEmpty(uom))
            {
                string cleanUom = uom.Trim();
                if (cleanUom.StartsWith("(") && cleanUom.EndsWith(")"))
                    headerText += $" {cleanUom}";
                else if (cleanUom.Contains("(") && cleanUom.Contains(")"))
                    headerText += $" {cleanUom}";
                else
                    headerText += $" ({cleanUom})";
            }

            string[] headers = { "Specimen", headerText };
            for (int i = 0; i < colCount; i++)
            {
                var cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa },
                    new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));
                var para = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                var run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                run.AppendChild(new WordText(headers[i]) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            var sortedSpecimens = specimenData.Keys.OrderBy(x => x).ToList();
            List<double> values = new List<double>();

            foreach (var specimenNum in sortedSpecimens)
            {
                var row = new TableRow();

                var specCell = new TableCell();
                specCell.AppendChild(new TableCellProperties(
                    new TableCellWidth { Width = colWidths[0].ToString(), Type = TableWidthUnitValues.Dxa },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));
                var specPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                specPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                    new WordText($"Specimen {specimenNum}") { Space = SpaceProcessingModeValues.Preserve }));
                specCell.AppendChild(specPara);
                row.AppendChild(specCell);

                string result = specimenData[specimenNum];
                var resultCell = new TableCell();
                resultCell.AppendChild(new TableCellProperties(
                    new TableCellWidth { Width = colWidths[1].ToString(), Type = TableWidthUnitValues.Dxa },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));
                var resultPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                resultPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                    new WordText(result) { Space = SpaceProcessingModeValues.Preserve }));
                resultCell.AppendChild(resultPara);
                row.AppendChild(resultCell);

                table.AppendChild(row);

                if (double.TryParse(result, out double val))
                    values.Add(val);
            }

            // Average row
            var avgRow = new TableRow();

            var avgLabelCell = new TableCell();
            avgLabelCell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = colWidths[0].ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ));
            var avgLabelPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
            var avgLabelRun = new Run();
            avgLabelRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "20" }));
            avgLabelRun.AppendChild(new WordText("Average") { Space = SpaceProcessingModeValues.Preserve });
            avgLabelPara.AppendChild(avgLabelRun);
            avgLabelCell.AppendChild(avgLabelPara);
            avgRow.AppendChild(avgLabelCell);

            var avgValueCell = new TableCell();
            avgValueCell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = colWidths[1].ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ));
            var avgValuePara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
            var avgValueRun = new Run();
            avgValueRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "20" }));

            if (values.Count > 0)
            {
                double avg = values.Average();
                string format = values.All(v => Math.Abs(v - Math.Round(v, 2)) > 0.001) ? "F2" : "F1";
                if (!string.IsNullOrEmpty(uom) && (uom.Contains("%") || uom.Contains("Loss of Strength")))
                    format = "F2";
                avgValueRun.AppendChild(new WordText(avg.ToString(format)) { Space = SpaceProcessingModeValues.Preserve });
            }
            else
            {
                avgValueRun.AppendChild(new WordText("-") { Space = SpaceProcessingModeValues.Preserve });
            }

            avgValuePara.AppendChild(avgValueRun);
            avgValueCell.AppendChild(avgValuePara);
            avgRow.AppendChild(avgValueCell);

            table.AppendChild(avgRow);

            return table;
        }

        // ========== MULTI-COLUMN TABLE (multiple UOMs) ==========
        private Table BuildMultiColumnTable24(List<DataRow> rows, List<string> uoms)
        {
            var specimenGroups = rows
                .GroupBy(r =>
                {
                    string specimenStr = r.Field<string>("Specimen") ?? "Specimen 1";
                    int specimenNum = 1;
                    if (specimenStr.StartsWith("Specimen "))
                        int.TryParse(specimenStr.Replace("Specimen ", ""), out specimenNum);
                    return specimenNum;
                })
                .OrderBy(g => g.Key)
                .ToList();

            if (specimenGroups.Count == 0) return null;

            int colCount = 1 + uoms.Count;
            var table = new Table();

            var tableProps = new TableProperties();
            var borders = new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            );
            tableProps.AppendChild(borders);
            tableProps.AppendChild(new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            List<int> colWidths = new List<int> { (int)(USABLE_WIDTH_TWIPS * 0.2) };
            int remaining = USABLE_WIDTH_TWIPS - colWidths[0];
            int eachWidth = remaining / uoms.Count;
            for (int i = 0; i < uoms.Count; i++)
                colWidths.Add(eachWidth);

            var grid = new TableGrid();
            foreach (int w in colWidths)
                grid.AppendChild(new GridColumn { Width = w.ToString() });
            table.AppendChild(grid);

            // Header
            var headerRow = new TableRow();
            var specHeaderCell = new TableCell();
            specHeaderCell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = colWidths[0].ToString(), Type = TableWidthUnitValues.Dxa },
                new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ));
            var specHeaderPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
            specHeaderPara.AppendChild(new Run(new RunProperties(new Bold(), new FontSize { Val = "22" }),
                new WordText("Specimen") { Space = SpaceProcessingModeValues.Preserve }));
            specHeaderCell.AppendChild(specHeaderPara);
            headerRow.AppendChild(specHeaderCell);

            for (int i = 0; i < uoms.Count; i++)
            {
                var cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellWidth { Width = colWidths[i + 1].ToString(), Type = TableWidthUnitValues.Dxa },
                    new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));
                var para = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                var run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "20" }));
                string headerText = uoms[i];
                if (!headerText.StartsWith("(") && !headerText.EndsWith(")"))
                    headerText = $"({headerText})";
                run.AppendChild(new WordText(headerText) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            var valuesByUOM = new Dictionary<string, List<double>>();
            foreach (var uom in uoms)
                valuesByUOM[uom] = new List<double>();

            foreach (var specimenGroup in specimenGroups)
            {
                var row = new TableRow();

                var specCell = new TableCell();
                specCell.AppendChild(new TableCellProperties(
                    new TableCellWidth { Width = colWidths[0].ToString(), Type = TableWidthUnitValues.Dxa },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));
                var specPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                specPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                    new WordText($"Specimen {specimenGroup.Key}") { Space = SpaceProcessingModeValues.Preserve }));
                specCell.AppendChild(specPara);
                row.AppendChild(specCell);

                var dataByUOM = specimenGroup.ToDictionary(
                    r => r.Field<string>("UOM") ?? "",
                    r => r.Field<string>("Result") ?? ""
                );

                for (int i = 0; i < uoms.Count; i++)
                {
                    string uom = uoms[i];
                    var cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidths[i + 1].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));
                    var para = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                    var run = new Run(new RunProperties(new FontSize { Val = "20" }));

                    if (dataByUOM.ContainsKey(uom) && !string.IsNullOrEmpty(dataByUOM[uom]) && dataByUOM[uom] != "-")
                    {
                        string result = dataByUOM[uom];
                        run.AppendChild(new WordText(result) { Space = SpaceProcessingModeValues.Preserve });
                        if (double.TryParse(result, out double val))
                            valuesByUOM[uom].Add(val);
                    }
                    else
                    {
                        run.AppendChild(new WordText("-") { Space = SpaceProcessingModeValues.Preserve });
                    }

                    para.AppendChild(run);
                    cell.AppendChild(para);
                    row.AppendChild(cell);
                }

                table.AppendChild(row);
            }

            // Average row
            var avgRow = new TableRow();

            var avgLabelCell = new TableCell();
            avgLabelCell.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = colWidths[0].ToString(), Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ));
            var avgLabelPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
            var avgLabelRun = new Run();
            avgLabelRun.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "20" }));
            avgLabelRun.AppendChild(new WordText("Average") { Space = SpaceProcessingModeValues.Preserve });
            avgLabelPara.AppendChild(avgLabelRun);
            avgLabelCell.AppendChild(avgLabelPara);
            avgRow.AppendChild(avgLabelCell);

            for (int i = 0; i < uoms.Count; i++)
            {
                string uom = uoms[i];
                var cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellWidth { Width = colWidths[i + 1].ToString(), Type = TableWidthUnitValues.Dxa },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));
                var para = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                var run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "20" }));

                if (valuesByUOM.ContainsKey(uom) && valuesByUOM[uom].Count > 0)
                {
                    double avg = valuesByUOM[uom].Average();
                    string format = valuesByUOM[uom].All(v => Math.Abs(v - Math.Round(v, 2)) > 0.001) ? "F2" : "F1";
                    if (uom.Contains("%")) format = "F2";
                    run.AppendChild(new WordText(avg.ToString(format)) { Space = SpaceProcessingModeValues.Preserve });
                }
                else
                {
                    run.AppendChild(new WordText("-") { Space = SpaceProcessingModeValues.Preserve });
                }

                para.AppendChild(run);
                cell.AppendChild(para);
                avgRow.AppendChild(cell);
            }

            table.AppendChild(avgRow);

            return table;
        }

        // ========== BH TABLE METHODS (Simplified - similar to main but with BH headers) ========== //
        private void CreateTable10_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                FindAndReplace(doc, placeholder, "Data tidak tersedia");
                return;
            }

            var table = CreateFormattedTable(doc, dt);
            if (table == null) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateTable11_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            // Use same logic as CreateTable11 but with BH headers
            CreateTable11(doc, placeholder, dt);
        }

        private void CreateTable12_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            // Use same logic as CreateTable12 but with BH headers
            CreateTable12(doc, placeholder, dt);
        }

        private void CreateTable14_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                FindAndReplace(doc, placeholder, "Data peralatan kalibrasi tidak tersedia.");
                return;
            }

            var table = CreateFormattedTable(doc, dt);
            if (table == null) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateTable15_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            // Use same logic as CreateTable15 but with BH headers
            CreateTable15(doc, placeholder, dt);
        }

        private void CreateTable16_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            // Use same logic as CreateTable16 but with BH headers
            CreateTable16(doc, placeholder, dt);
        }

        private void CreateTable17_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                FindAndReplace(doc, "{Overall Conclusion}", "Data tidak tersedia");
                FindAndReplace(doc, "{Requirement}", "Tidak ada persyaratan");
                return;
            }

            var requirements = dt.AsEnumerable()
                .Select(row => row["Requirement"]?.ToString())
                .Where(req => !string.IsNullOrWhiteSpace(req))
                .Distinct()
                .ToList();
            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            FindAndReplace(doc, "{Requirement}", combinedRequirements);

            DataTable modifiedDt = dt.Copy();
            if (modifiedDt.Columns.Contains("Requirement"))
                modifiedDt.Columns.Remove("Requirement");

            var table = BuildTable17(modifiedDt);
            if (table == null) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            RemovePlaceholderFromParagraph(targetParagraph, placeholder);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateTable18_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            // Use same logic as CreateTable18 but with BH headers
            CreateTable18(doc, placeholder, dt);
        }

        private void CreateTable19_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            // Use same logic as CreateTable19 but with BH headers
            CreateTable19(doc, placeholder, dt);
        }

        private void CreateTableforSampleDescription_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    FindAndReplace(doc, placeholder, "");
                    return;
                }

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                int colCount = 3;
                int tableWidth = USABLE_WIDTH_TWIPS;
                int baseColWidth = tableWidth / colCount;
                int remainder = tableWidth - (baseColWidth * colCount);
                List<int> colWidths = new List<int>();
                for (int i = 0; i < colCount; i++)
                    colWidths.Add(i == colCount - 1 ? baseColWidth + remainder : baseColWidth);

                var table = new Table();

                var tableProps = new TableProperties();
                var borders = new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                );
                tableProps.AppendChild(borders);
                tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });
                tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
                table.AppendChild(tableProps);

                var grid = new TableGrid();
                foreach (int w in colWidths)
                    grid.AppendChild(new GridColumn { Width = w.ToString() });
                table.AppendChild(grid);

                // Header row
                var headerRow = new TableRow();
                string[] headers = { "#", "Deskripsi Sample Uji", "ID Sampel" };
                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    var cellProps = new TableCellProperties();
                    cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                    cellProps.AppendChild(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" });
                    cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                    cell.AppendChild(cellProps);

                    var para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
                    var run = new Run();
                    run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    run.AppendChild(new WordText(headers[i]) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    headerRow.AppendChild(cell);
                }
                table.AppendChild(headerRow);

                // Data rows
                int rowNum = 1;
                foreach (DataRow row in dt.Rows)
                {
                    var dataRow = new TableRow();
                    for (int i = 0; i < colCount; i++)
                    {
                        var cell = new TableCell();
                        var cellProps = new TableCellProperties();
                        cellProps.AppendChild(new TableCellWidth { Width = colWidths[i].ToString(), Type = TableWidthUnitValues.Dxa });
                        cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                        cell.AppendChild(cellProps);

                        var para = new Paragraph();
                        para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
                        var run = new Run();
                        run.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                        string text = i == 0 ? rowNum.ToString() : row[i - 1]?.ToString() ?? "";
                        run.AppendChild(new WordText(text) { Space = SpaceProcessingModeValues.Preserve });
                        para.AppendChild(run);
                        cell.AppendChild(para);
                        dataRow.AppendChild(cell);
                    }
                    table.AppendChild(dataRow);
                    rowNum++;
                }

                targetParagraph.Parent.InsertAfter(table, targetParagraph);
                var breakParagraph = new Paragraph(new Run());
                targetParagraph.Parent.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating BH Sample Description table: {ex.Message}");
            }
        }

        private void InsertImagesIntoDocument1_BH(WordprocessingDocument doc, string placeholder, List<Tuple<string, string, string>> imageData)
        {
            InsertImagesIntoDocument1(doc, placeholder, imageData);
        }

        private void InsertImagesIntoDocument21_BH(WordprocessingDocument doc, string placeholder, List<Tuple<string, string>> imageData)
        {
            InsertImagesIntoDocument21(doc, placeholder, imageData);
        }

        private void CreateTable22_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            CreateTable22(doc, placeholder, dt);
        }

        // ========== INSERT IMAGES INTO DOCUMENT 1 ==========
        private void InsertImagesIntoDocument1(WordprocessingDocument doc, string placeholder, List<Tuple<string, string, string>> imageData)
        {
            try
            {
                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null || imageData == null || imageData.Count == 0) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                int totalImages = imageData.Count;
                int rows = totalImages + 1;
                int colCount = 2;

                int colWidth = USABLE_WIDTH_TWIPS / colCount;
                var table = new Table();

                var tableProps = new TableProperties();
                var borders = new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                );
                tableProps.AppendChild(borders);
                tableProps.AppendChild(new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa });
                tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
                table.AppendChild(tableProps);

                var grid = new TableGrid();
                for (int i = 0; i < colCount; i++)
                    grid.AppendChild(new GridColumn { Width = colWidth.ToString() });
                table.AppendChild(grid);

                // Header row
                var headerRow = new TableRow();
                string[] headers = { "Description and Color", "Photo / Photograph" };
                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidth.ToString(), Type = TableWidthUnitValues.Dxa },
                        new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));
                    var para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
                    var run = new Run();
                    run.AppendChild(new RunProperties(new Bold(), new FontSize { Val = "22" }));
                    run.AppendChild(new WordText(headers[i]) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    headerRow.AppendChild(cell);
                }
                table.AppendChild(headerRow);

                long maxWidthEmu = 2000000L;
                long maxHeightEmu = 2000000L;

                for (int i = 0; i < totalImages; i++)
                {
                    var row = new TableRow();
                    string imagePath = imageData[i].Item1;
                    string sampleText = imageData[i].Item2;
                    string sampleColourName = imageData[i].Item3;

                    // Description cell
                    var descCell = new TableCell();
                    descCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidth.ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));

                    string descriptionText = "";
                    if (!string.IsNullOrEmpty(sampleText))
                        descriptionText += sampleText + "\n\n";
                    if (!string.IsNullOrEmpty(sampleColourName))
                        descriptionText += sampleColourName;

                    var descPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                    descPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                        new WordText(descriptionText) { Space = SpaceProcessingModeValues.Preserve }));
                    descCell.AppendChild(descPara);
                    row.AppendChild(descCell);

                    // Photo cell
                    var photoCell = new TableCell();
                    photoCell.AppendChild(new TableCellProperties(
                        new TableCellWidth { Width = colWidth.ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));

                    if (IsUsableImageFile(imagePath))
                    {
                        try
                        {
                            var imagePart = mainPart.AddImagePart(GetImagePartTypeForFile(imagePath));
                            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                imagePart.FeedData(stream);
                            }

                            GetScaledEmuSize(imagePath, maxWidthEmu, maxHeightEmu, out long picWidth, out long picHeight);

                            var element = CreateImageElement(mainPart.GetIdOfPart(imagePart), "Image " + (i + 1), picWidth, picHeight);

                            var photoPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                            photoPara.AppendChild(new Run(element));
                            photoCell.AppendChild(photoPara);
                        }
                        catch (Exception ex)
                        {
                            var errorPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                            errorPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                                new WordText($"Error loading: {sampleText}") { Space = SpaceProcessingModeValues.Preserve }));
                            photoCell.AppendChild(errorPara);
                        }
                    }
                    else
                    {
                        var missingPara = new Paragraph(new ParagraphProperties(new Justification { Val = JustificationValues.Center }));
                        missingPara.AppendChild(new Run(new RunProperties(new FontSize { Val = "20" }),
                            new WordText($"Missing: {sampleText}") { Space = SpaceProcessingModeValues.Preserve }));
                        photoCell.AppendChild(missingPara);
                    }

                    row.AppendChild(photoCell);
                    table.AppendChild(row);
                }

                targetParagraph.Parent.InsertAfter(table, targetParagraph);
                var breakParagraph = new Paragraph(new Run());
                targetParagraph.Parent.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in InsertImagesIntoDocument1: {ex.Message}");
            }
        }





    }
}