using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
// ADD THESE USING STATEMENTS FOR SYNC FUSION
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
    public partial class CreateReportFromTemplate_Chemical : System.Web.UI.Page
    {
        private LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        private int lastUsedNumber = 0;

        // A4 Page width constants
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
                // Register Syncfusion license (put your actual license key here)
                string licenseKey = "Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTc0RnWX5cd3FcTmBdV091XA==";
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

                int sampleID = 0;
                string path = Server.MapPath("~/Inputfiles/");
                string pathout = Server.MapPath("~/TestReport/");

                // Ensure TestReport directory exists
                if (!Directory.Exists(pathout))
                {
                    Directory.CreateDirectory(pathout);
                }

                if (Request.QueryString["s"] != null && Request.QueryString["c"] != null)
                {
                    sampleID = Convert.ToInt32(Request.QueryString["s"]);
                    ViewState["SampleIDGet"] = sampleID;
                }

                string s = Request.QueryString["s"]?.ToString() ?? "";
                _cm.sampleId = Convert.ToInt32(s);

                int ReportIDGet = Convert.ToInt32(Request.QueryString["c"]?.ToString() ?? "0");
                int ID = Convert.ToInt32(s);
                string SampleUID = "";

                DataTable dt = _cm.getSampleUID(ID);
                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    GenerateQR(SampleUID);
                }

                DataTable ChemicalDetails = _cm.getChemicalDetails(sampleID);
                DataTable dt1 = _cm.getReportQuery(ReportIDGet.ToString());

                List<string> filePaths = new List<string>();

                if (dt1.Rows.Count > 0)
                {
                    string template = dt1.Rows[0]["Template"].ToString();
                    string templatePath = path + template;
                    string templatePath_BH = GetTemplatePath_BH(template, path);

                    // Process Main Document
                    if (File.Exists(templatePath))
                    {
                        try
                        {
                            byte[] mainDocBytes = File.ReadAllBytes(templatePath);
                            byte[] mainProcessedBytes = ProcessDocumentMain(mainDocBytes, ChemicalDetails, SampleUID, ReportIDGet, s, dt1);

                            // ========== SAVE DOCX ==========
                            string outputDocxPath = pathout + "TestReport-" + SampleUID + ".docx";
                            File.WriteAllBytes(outputDocxPath, mainProcessedBytes);
                            filePaths.Add("/TestReport/TestReport-" + SampleUID + ".docx");

                            // ========== CONVERT TO PDF USING SYNC FUSION ==========
                            try
                            {
                                byte[] pdfBytes = ConvertDocxToPdf_Syncfusion(mainProcessedBytes);
                                string outputPdfPath = pathout + "TestReport-" + SampleUID + ".pdf";
                                File.WriteAllBytes(outputPdfPath, pdfBytes);
                                filePaths.Add("/TestReport/TestReport-" + SampleUID + ".pdf");
                            }
                            catch (Exception pdfEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"PDF conversion error for main document: {pdfEx.Message}");
                                // Continue even if PDF conversion fails
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing main document: {ex.Message}");
                        }
                    }

                    // Process BH Document
                    if (!string.IsNullOrEmpty(templatePath_BH) && File.Exists(templatePath_BH))
                    {
                        try
                        {
                            byte[] bhDocBytes = File.ReadAllBytes(templatePath_BH);
                            byte[] bhProcessedBytes = ProcessDocumentBH(bhDocBytes, ChemicalDetails, SampleUID, ReportIDGet, s, dt1);

                            // ========== SAVE BH DOCX ==========
                            string outputDocxPath_BH = pathout + "TestReport-" + SampleUID + "_BH.docx";
                            File.WriteAllBytes(outputDocxPath_BH, bhProcessedBytes);
                            filePaths.Add("/TestReport/TestReport-" + SampleUID + "_BH.docx");

                            // ========== CONVERT BH TO PDF USING SYNC FUSION ==========
                            try
                            {
                                byte[] pdfBytes_BH = ConvertDocxToPdf_Syncfusion(bhProcessedBytes);
                                string outputPdfPath_BH = pathout + "TestReport-" + SampleUID + "_BH.pdf";
                                File.WriteAllBytes(outputPdfPath_BH, pdfBytes_BH);
                                filePaths.Add("/TestReport/TestReport-" + SampleUID + "_BH.pdf");
                            }
                            catch (Exception pdfEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"PDF conversion error for BH document: {pdfEx.Message}");
                                // Continue even if PDF conversion fails
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing BH document: {ex.Message}");
                        }
                    }

                    // Open all files
                    if (filePaths.Count > 0)
                    {
                        string script = GenerateDownloadScript(filePaths);
                        ClientScript.RegisterStartupScript(this.GetType(), "OpenDocs", script);
                    }
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        // ========== SYNC FUSION DOCX TO PDF CONVERTER ==========
        private byte[] ConvertDocxToPdf_Syncfusion(byte[] docxBytes)
        {
            try
            {
                // Load the DOCX from byte array
                using (MemoryStream docxStream = new MemoryStream(docxBytes))
                using (WordDocument wordDocument = new WordDocument(docxStream, FormatType.Automatic))
                {
                    // Create renderer and convert to PDF
                    using (DocIORenderer renderer = new DocIORenderer())
                    using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                    {
                        // Save PDF to memory stream
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

        // ========== UPDATED GENERATE DOWNLOAD SCRIPT ==========
        private string GenerateDownloadScript(List<string> filePaths)
        {
            if (filePaths.Count == 1)
            {
                return $@"<script>window.open('{filePaths[0]}', '_blank');</script>";
            }
            else
            {
                string script = "<script>";
                for (int i = 0; i < filePaths.Count; i++)
                {
                    int delay = i * 800; // Increased delay for multiple files
                    script += $@"setTimeout(function() {{ window.open('{filePaths[i]}', '_blank'); }}, {delay});";
                }
                script += "</script>";
                return script;
            }
        }

        // ========== GET TEMPLATE PATH FOR BH ==========
        private string GetTemplatePath_BH(string template, string path)
        {
            if (template == "Chemical_TemplateGeneralK3L.docx")
                return path + "Chemical_TemplateGeneralK3L_BH.docx";
            else if (template == "Chemical_TemplateGeneral.docx")
                return path + "Chemical_TemplateGeneral_BH.docx";
            else if (template == "Chemical_TemplateAO1.docx")
                return path + "Chemical_TemplateAO1_BH.docx";
            return "";
        }

        // ========== PROCESS DOCUMENT MAIN ==========
        private byte[] ProcessDocumentMain(byte[] docBytes, DataTable ChemicalDetails, string SampleUID, int ReportIDGet, string sampleId, DataTable dt1)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(docBytes, 0, docBytes.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    // Reset lastUsedNumber at start
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
                                ProcessTableMain(wordDoc, table[i], query[i], sampleId);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error processing table {table[i]}: {ex.Message}");
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
                    // Reset lastUsedNumber at start
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
                                ProcessTableBH(wordDoc, table[i], query[i], sampleId);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error processing BH table {table[i]}: {ex.Message}");
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
        private void ProcessTableMain(WordprocessingDocument doc, string tableType, string query, string sampleId)
        {
            switch (tableType)
            {
                case "10":
                    DataTable dt10 = _cm.getReportQuery(1, query + sampleId);
                    CreateSummaryTable(doc, "{ResultSummaryTable}", dt10);
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
                    List<Tuple<string, string>> imageData = new List<Tuple<string, string>>();
                    foreach (DataRow row in dt13.Rows)
                    {
                        string imageFileName = row["filename"].ToString().Trim();
                        string imageName = row["SampleText"].ToString();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (IsUsableImageFile(fullImagePath))
                            {
                                imageData.Add(Tuple.Create(fullImagePath, imageName));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Sample image missing or unreadable, skipping: {fullImagePath}");
                            }
                        }
                    }
                    InsertImagesIntoDocument(doc, "{images}", imageData);
                    break;
                case "14":
                    DataTable dt14 = _cm.getReportQuery(1, query);
                    CreateEquipmentTable(doc, "{Testingandmeasuringequipment}", dt14);
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
                    DataTable dt17 = _cm.GetResultOverallConclusion(sampleId);
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
            }
        }

        // ========== PROCESS TABLE BH ==========
        private void ProcessTableBH(WordprocessingDocument doc, string tableType, string query, string sampleId)
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
                    CreateSummaryTable(doc, "{ResultSummaryTable}", dt10);
                    break;
                case "11":
                    string q11 = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                   c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                   WHERE t.sampleID= '" + sampleId + "' ";
                    DataTable dt11 = _cm.getReportQuery_BH(q11);
                    CreateTable11BH(doc, "{ResultTable1}", dt11);
                    break;
                case "12":
                    string q12 = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                   c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                   WHERE t.sampleID='" + sampleId + "' ";
                    DataTable dt12 = _cm.getReportQuery_BH(q12);
                    CreateTable12BH(doc, "{TestParameter}", dt12);
                    break;
                case "13":
                    DataTable dt13 = _cm.getReportQuery(1, query + sampleId);
                    List<Tuple<string, string>> imageData = new List<Tuple<string, string>>();
                    foreach (DataRow row in dt13.Rows)
                    {
                        string imageFileName = row["filename"].ToString().Trim();
                        string imageName = row["SampleText"].ToString();
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                            if (IsUsableImageFile(fullImagePath))
                            {
                                imageData.Add(Tuple.Create(fullImagePath, imageName));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Sample image missing or unreadable, skipping: {fullImagePath}");
                            }
                        }
                    }
                    InsertImagesIntoDocument(doc, "{images}", imageData);
                    break;
                case "14":
                    string q14 = @"select Equipment ,Type,Calibration_Date as[Calibration Due Date] from TestResultDetails t 
                                   INNER JOIN Calibration_Equipment c  on c.sampleid = t.SampleId ";
                    DataTable dt14 = _cm.getReportQuery_BH(q14);
                    CreateEquipmentTable(doc, "{Testingandmeasuringequipment}", dt14);
                    break;
                case "15":
                    string q15 = @"SELECT tp.TestParameter_BH AS[Test Parameter], tsp.TestSubParameter_BH AS[Test Item], TestMethod AS[Method *],
                                   c.MDL as [M.D.L],TestResult AS[Result(s)], t.EquipmentName AS[Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND(c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '')
                                   WHERE t.sampleID ='" + sampleId + "' ";
                    DataTable dt15 = _cm.getReportQuery_BH(q15);
                    CreateTable15BH(doc, "{ResultTable1}", dt15);
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
                    CreateTable16BH(doc, "{ResultTable2}", dt16);
                    break;
                case "17":
                    DataTable dt17 = _cm.GetResultOverallConclusion_BH(sampleId);
                    CreateTable17BH(doc, "{Overall Conclusion}", dt17);
                    break;
                case "18":
                    string q18 = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Element], c.CASNo AS [CAS Number], TestMethod AS [Method*], 
                                   TestResult AS [Result(mg/kg)], t.Specification AS [Requirement], t.EquipmentName AS [Equipment], ccs.ColourCodeId AS [Colour Code], 
                                   tsp.MDL, t.Conclusion_BH as Conclusion FROM crfsampledetails c 
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                   INNER JOIN ColourCodeSave ccs ON ccs.AdditionalColourCodeId = t.AdditionalColourCodesId WHERE t.sampleID ='" + sampleId + "' ";
                    DataTable dt18 = _cm.getReportQuery_BH(q18);
                    CreateTable18BH(doc, "{ResultTable1}", dt18);
                    break;
                case "19":
                    string q19 = @"SELECT tp.TestParameter_BH AS [Parameter Uji], tsp.TestSubParameter_BH AS [Barang Tes], 
                                   tssp.TestSubSubParameter_BH AS [Senyawa], c.CASNo AS [CAS Number],TestMethod AS [Method*], 
                                   TestResult AS [Hasil(mg/kg)],t.EquipmentName AS [Peralatan],
                                   ccs.ColourCodeId AS [Colour Code], tsp.MDL, t.Conclusion_BH AS Conclusion 
                                   FROM crfsampledetails c 
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID AND (c.TestSubSubParameter IS NOT NULL AND c.TestSubSubParameter != '') 
                                   INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                   INNER JOIN ColourCodeSave ccs ON ccs.AdditionalColourCodeId = t.AdditionalColourCodesId
                                   WHERE t.sampleID = '" + sampleId + "' ";
                    DataTable dt19 = _cm.getReportQuery_BH(q19);
                    CreateTable19BH(doc, "{ResultTable2}", dt19);
                    break;
                case "20":
                    string q20 = @"select SampleText+ ' - ' + SampleName + ' (' + SampleColourName_BH +')' AS [Sample Description], SampleColorCode AS [Sample ID] from SDColourCodeImages where SampleId ='" + sampleId + "' ";
                    DataTable dt20 = _cm.getReportQuery_BH(q20);
                    CreateTableforSampleDescriptionBH(doc, "{SampleDescriptionData}", dt20);
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
                System.Diagnostics.Debug.WriteLine("Error generating QR: " + ex.Message);
            }
        }

        // ========== PLACEHOLDER HELPER METHODS ==========
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

        private void ForceReplaceInDocument(WordprocessingDocument doc, string findText, string replaceText)
        {
            FindAndReplace(doc, findText, replaceText);
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
                        System.Diagnostics.Debug.WriteLine($"Failed to replace {kvp.Key}: {ex.Message}");
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
                            System.Diagnostics.Debug.WriteLine($"Unresolved placeholder left in document, blanking: {m.Value}");
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

        private Paragraph FindAndReplacePlaceholder(WordprocessingDocument doc, string placeholder)
        {
            var body = doc.MainDocumentPart.Document.Body;
            var paragraph = FindParagraphContaining(body, placeholder);
            if (paragraph == null) return null;

            ReplaceInParagraph(paragraph, placeholder, "");
            return paragraph;
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

        // ========== IMAGE AND QR CODE METHODS ==========
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

        private void InsertImagesIntoDocument(WordprocessingDocument doc, string placeholder, List<Tuple<string, string>> imageData)
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

            var tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.None, Size = 0 },
                    new BottomBorder() { Val = BorderValues.None, Size = 0 },
                    new LeftBorder() { Val = BorderValues.None, Size = 0 },
                    new RightBorder() { Val = BorderValues.None, Size = 0 },
                    new InsideHorizontalBorder() { Val = BorderValues.None, Size = 0 },
                    new InsideVerticalBorder() { Val = BorderValues.None, Size = 0 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = usablePageWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa }
            );
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

                for (int col = 0; col < imagesPerRow; col++)
                {
                    var cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellWidth() { Width = colWidthTwips.ToString(), Type = TableWidthUnitValues.Dxa }
                    ));

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

                            var paragraph = new Paragraph();
                            var run = new Run();

                            var element = CreateImageElement(mainPart.GetIdOfPart(imagePart), "Image " + (imageIndex + 1), picWidth, picHeight);
                            run.AppendChild(element);

                            paragraph.AppendChild(run);

                            var captionRun = new Run();
                            captionRun.AppendChild(new RunProperties(
                                new FontSize() { Val = "20" },
                                new RunFonts() { Ascii = "Arial" }
                            ));
                            captionRun.AppendChild(new Break());
                            captionRun.AppendChild(new WordText() { Text = imageName, Space = SpaceProcessingModeValues.Preserve });
                            paragraph.AppendChild(captionRun);

                            paragraph.ParagraphProperties = new ParagraphProperties(
                                new Justification() { Val = JustificationValues.Center }
                            );
                            cell.AppendChild(paragraph);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Sample image missing or unreadable, leaving cell blank: {imagePath}");
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

        // ========== TABLE CREATION METHODS ==========
        private void CreateGenericTable(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            ReplaceInParagraph(targetParagraph, placeholder, "");

            if (dt == null || dt.Rows.Count == 0) return;

            var table = BuildTableFromDataTable(dt);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateSummaryTable(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            CreateGenericTable(doc, placeholder, dt);
        }

        private void CreateEquipmentTable(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            CreateGenericTable(doc, placeholder, dt);
        }

        private Table BuildTableFromDataTable(DataTable dt)
        {
            var table = new Table();

            int colCount = Math.Max(1, dt.Columns.Count);
            int colWidth = USABLE_WIDTH_TWIPS / colCount;
            int remainder = USABLE_WIDTH_TWIPS - (colWidth * colCount);

            var tableProps = new TableProperties(
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
            );
            table.AppendChild(tableProps);

            var grid = new TableGrid();
            for (int i = 0; i < colCount; i++)
            {
                int width = (i == colCount - 1) ? colWidth + remainder : colWidth;
                grid.AppendChild(new GridColumn() { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // Header row
            var headerRow = new TableRow();
            foreach (DataColumn col in dt.Columns)
            {
                var cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));

                var para = new Paragraph();
                var run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = col.ColumnName, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
                cell.AppendChild(para);

                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            foreach (DataRow row in dt.Rows)
            {
                var dataRow = new TableRow();
                foreach (DataColumn col in dt.Columns)
                {
                    var cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));

                    var para = new Paragraph();
                    var text = row[col.ColumnName]?.ToString() ?? "";
                    var run = new Run();
                    run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    run.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    para.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    cell.AppendChild(para);

                    dataRow.AppendChild(cell);
                }
                table.AppendChild(dataRow);
            }

            return table;
        }

        private DataTable CreateModifiedDataTable(DataTable source, string[] excludeColumns)
        {
            DataTable modifiedDt = new DataTable();
            foreach (DataColumn col in source.Columns)
            {
                if (!excludeColumns.Contains(col.ColumnName))
                {
                    modifiedDt.Columns.Add(col.ColumnName, col.DataType);
                }
            }

            List<DataColumn> columnsToRemove = new List<DataColumn>();
            foreach (DataColumn column in modifiedDt.Columns)
            {
                bool allNullOrBlank = true;
                foreach (DataRow row in source.Rows)
                {
                    string value = row[column.ColumnName]?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        allNullOrBlank = false;
                        break;
                    }
                }
                if (allNullOrBlank)
                {
                    columnsToRemove.Add(column);
                }
            }

            foreach (DataColumn column in columnsToRemove)
            {
                modifiedDt.Columns.Remove(column);
            }

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = modifiedDt.NewRow();
                foreach (DataColumn col in modifiedDt.Columns)
                {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                modifiedDt.Rows.Add(newRow);
            }

            return modifiedDt;
        }

        private DataTable TranslateDataTable(DataTable source, Dictionary<string, string> headers)
        {
            DataTable translatedDt = new DataTable();
            foreach (DataColumn col in source.Columns)
            {
                string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                translatedDt.Columns.Add(colName);
            }

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = translatedDt.NewRow();
                foreach (DataColumn col in source.Columns)
                {
                    string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                    newRow[colName] = row[col.ColumnName];
                }
                translatedDt.Rows.Add(newRow);
            }

            return translatedDt;
        }

        private Paragraph AddGroupHeader(Paragraph currentParagraph, int number, DataTable groupDt, string template)
        {
            currentParagraph.Parent.AppendChild(new Paragraph(new Run(new Break())));
            currentParagraph.Parent.AppendChild(new Paragraph(new Run(new Break())));

            var headerPara = currentParagraph.Parent.AppendChild(new Paragraph());
            var headerRun = new Run();
            headerRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
            headerRun.AppendChild(new WordText() { Text = $"{number}.      {groupDt.Rows[0]["Test Parameter"]}\n", Space = SpaceProcessingModeValues.Preserve });
            headerPara.AppendChild(headerRun);

            var preTreatPara = currentParagraph.Parent.AppendChild(new Paragraph());
            string preTreatText = template
                .Replace("{Method*}", groupDt.Rows[0]["Method*"]?.ToString() ?? "")
                .Replace("{Equipment}", groupDt.Rows[0]["Equipment"]?.ToString() ?? "");
            var preTreatRun = new Run();
            preTreatRun.AppendChild(new RunProperties(new FontSize() { Val = "22" }));
            preTreatRun.AppendChild(new WordText() { Text = $"          {preTreatText}\n", Space = SpaceProcessingModeValues.Preserve });
            preTreatPara.AppendChild(preTreatRun);

            return preTreatPara;
        }

        // ========== TABLE 11-17 METHODS ==========
        private void CreateTable11(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            Paragraph currentParagraph = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                    groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ForceReplaceInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ForceReplaceInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ForceReplaceInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = CreateModifiedDataTable(groupDt, new string[] { "Test Parameter", "Method*", "Equipment" });

                if (isFirstGroup)
                {
                    currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                }
                else
                {
                    if (currentParagraph != null)
                    {
                        currentParagraph = AddGroupHeader(currentParagraph, lastUsedNumber, groupDt, "Pre-Treatment based on {Method*}, analysis by {Equipment}");
                    }
                }

                if (currentParagraph != null)
                {
                    var table = BuildTableFromDataTable(modifiedDt);
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.GetFirstChild<TableRow>()?.GetFirstChild<TableCell>()?.GetFirstChild<Paragraph>();
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable12(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            if (dt == null || dt.Rows.Count == 0)
            {
                ForceReplaceInDocument(doc, placeholder, "");
                ForceReplaceInDocument(doc, "{Number}", "");
                ForceReplaceInDocument(doc, "{TestParameter2}", "");
                ForceReplaceInDocument(doc, "{TestSubParameter}", "");
                ForceReplaceInDocument(doc, "{TestMethod}", "");
                ForceReplaceInDocument(doc, "{Equipment2}", "");
                return;
            }

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            Paragraph currentParagraph = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                    groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        string testParamValue = $"{lastUsedNumber}.    ";
                        ForceReplaceInDocument(doc, "{Number}", testParamValue);
                        ForceReplaceInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ForceReplaceInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ForceReplaceInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ForceReplaceInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = CreateModifiedDataTable(groupDt, new string[] { "Test Parameter", "Test Item", "Method*", "Equipment" });

                if (isFirstGroup)
                {
                    currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                }
                else
                {
                    if (currentParagraph != null)
                    {
                        currentParagraph = AddGroupHeader(currentParagraph, lastUsedNumber, groupDt, "Pre-Treatment based on {Method*}, analysis by {Equipment}");
                    }
                }

                if (currentParagraph != null)
                {
                    var table = BuildTableFromDataTable(modifiedDt);
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.GetFirstChild<TableRow>()?.GetFirstChild<TableCell>()?.GetFirstChild<Paragraph>();
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable15(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            Paragraph currentParagraph = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                    groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ForceReplaceInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ForceReplaceInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = CreateModifiedDataTable(groupDt, new string[] { "Test Parameter", "Equipment" });

                if (isFirstGroup)
                {
                    currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                }
                else
                {
                    if (currentParagraph != null)
                    {
                        currentParagraph = AddGroupHeader(currentParagraph, lastUsedNumber, groupDt, "*)Analysis was performed by {Equipment}");
                    }
                }

                if (currentParagraph != null)
                {
                    var table = BuildTableFromDataTable(modifiedDt);
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.GetFirstChild<TableRow>()?.GetFirstChild<TableCell>()?.GetFirstChild<Paragraph>();
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable16(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            Paragraph currentParagraph = null;
            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                    groupDt.ImportRow(row);

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        string testParamValue = $"{lastUsedNumber}.    ";
                        ForceReplaceInDocument(doc, "{Number}", testParamValue);
                        ForceReplaceInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ForceReplaceInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ForceReplaceInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = CreateModifiedDataTable(groupDt, new string[] { "Test Parameter", "Test Item", "Equipment" });

                if (isFirstGroup)
                {
                    currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                }
                else
                {
                    if (currentParagraph != null)
                    {
                        currentParagraph = AddGroupHeader(currentParagraph, lastUsedNumber, groupDt, "*)Analysis was performed by {Equipment}");
                    }
                }

                if (currentParagraph != null)
                {
                    var table = BuildTableFromDataTable(modifiedDt);
                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.GetFirstChild<TableRow>()?.GetFirstChild<TableCell>()?.GetFirstChild<Paragraph>();
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable17(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ForceReplaceInDocument(doc, "{Overall Conclusion}", "No data available");
                ForceReplaceInDocument(doc, "{Requirement}", "No requirements available");
                return;
            }

            var requirements = dt.AsEnumerable()
                .Select(row => row["Requirement"]?.ToString())
                .Where(req => !string.IsNullOrWhiteSpace(req))
                .Distinct()
                .ToList();

            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            ForceReplaceInDocument(doc, "{Requirement}", combinedRequirements);

            DataTable modifiedDt = dt.Copy();
            if (modifiedDt.Columns.Contains("Requirement"))
                modifiedDt.Columns.Remove("Requirement");

            CreateGenericTable(doc, "{Overall Conclusion}", modifiedDt);
        }

        // ========== BH VERSIONS OF TABLES 11-17 ==========
        private void CreateTable11BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var headers = new Dictionary<string, string>
                {
                    {"Test Parameter", "Parameter Uji"},
                    {"Test Item", "Item Uji"},
                    {"Method*", "Metode*"},
                    {"M.D.L", "M.D.L"},
                    {"Result(s)", "Hasil"},
                    {"Equipment", "Peralatan"}
                };
                DataTable translatedDt = TranslateDataTable(dt, headers);
                CreateTable11(doc, placeholder, translatedDt);
            }
        }

        private void CreateTable12BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var headers = new Dictionary<string, string>
                {
                    {"Test Parameter", "Parameter Uji"},
                    {"Test Item", "Item Uji"},
                    {"Compound", "Senyawa"},
                    {"Method*", "Metode*"},
                    {"M.D.L", "M.D.L"},
                    {"Result(s)", "Hasil"},
                    {"Equipment", "Peralatan"}
                };
                DataTable translatedDt = TranslateDataTable(dt, headers);
                CreateTable12(doc, placeholder, translatedDt);
            }
        }

        private void CreateTable15BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var headers = new Dictionary<string, string>
                {
                    {"Test Parameter", "Parameter Uji"},
                    {"Test Item", "Item Uji"},
                    {"Method*", "Metode*"},
                    {"M.D.L", "M.D.L"},
                    {"Result(s)", "Hasil"},
                    {"Equipment", "Peralatan"}
                };
                DataTable translatedDt = TranslateDataTable(dt, headers);
                CreateTable15(doc, placeholder, translatedDt);
            }
        }

        private void CreateTable16BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var headers = new Dictionary<string, string>
                {
                    {"Test Parameter", "Parameter Uji"},
                    {"Test Item", "Item Uji"},
                    {"Compound", "Senyawa"},
                    {"Method*", "Metode*"},
                    {"M.D.L", "M.D.L"},
                    {"Result(s)", "Hasil"},
                    {"Equipment", "Peralatan"}
                };
                DataTable translatedDt = TranslateDataTable(dt, headers);
                CreateTable16(doc, placeholder, translatedDt);
            }
        }

        private void CreateTable17BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            CreateTable17(doc, placeholder, dt);
        }

        // ========== SAMPLE DESCRIPTION TABLES ==========
        private void CreateTableforSampleDescription(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                if (dt == null || dt.Rows.Count == 0) return;

                var bodyElement = targetParagraph.Parent;

                Table table = CreateTableFromDataTable(dt, false);
                bodyElement.InsertAfter(table, targetParagraph);

                Paragraph breakParagraph = new Paragraph(new Run());
                bodyElement.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating Sample Description table: {ex.Message}");
            }
        }

        private void CreateTableforSampleDescriptionBH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            try
            {
                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                if (dt == null || dt.Rows.Count == 0) return;

                var bodyElement = targetParagraph.Parent;

                Table table = CreateTableFromDataTable(dt, true);
                bodyElement.InsertAfter(table, targetParagraph);

                Paragraph breakParagraph = new Paragraph(new Run());
                bodyElement.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating BH Sample Description table: {ex.Message}");
            }
        }

        private Table CreateTableFromDataTable(DataTable dt, bool isBahasa)
        {
            int colCount = dt.Columns.Count;
            int baseColWidth = USABLE_WIDTH_TWIPS / colCount;
            int remainder = USABLE_WIDTH_TWIPS - (baseColWidth * colCount);
            var colWidths = new List<int>();
            for (int i = 0; i < colCount; i++)
                colWidths.Add(i == colCount - 1 ? baseColWidth + remainder : baseColWidth);

            var table = new Table();

            var tableProps = new TableProperties(
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa },
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                )
            );
            table.AppendChild(tableProps);

            var grid = new TableGrid();
            foreach (var w in colWidths)
                grid.AppendChild(new GridColumn() { Width = w.ToString() });
            table.AppendChild(grid);

            // Header row
            var headerRow = new TableRow();
            for (int col = 0; col < colCount; col++)
            {
                var cell = new TableCell();
                var cellProps = new TableCellProperties(
                    new TableCellWidth() { Width = colWidths[col].ToString(), Type = TableWidthUnitValues.Dxa },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                );
                cell.AppendChild(cellProps);

                var para = new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }));
                var run = new Run(new RunProperties(new Bold()), new WordText(dt.Columns[col].ColumnName) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            foreach (DataRow row in dt.Rows)
            {
                var tableRow = new TableRow();
                for (int col = 0; col < colCount; col++)
                {
                    var cell = new TableCell();
                    var cellProps = new TableCellProperties(
                        new TableCellWidth() { Width = colWidths[col].ToString(), Type = TableWidthUnitValues.Dxa },
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    );
                    cell.AppendChild(cellProps);

                    string value = row[col]?.ToString() ?? "";
                    var para = new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }));
                    var run = new Run(new WordText(value) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    tableRow.AppendChild(cell);
                }
                table.AppendChild(tableRow);
            }

            return table;
        }

        // ========== TABLE 18 AND 19 METHODS ==========
        private void CreateTable18(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Parameter"))
                    .ToList();

                Paragraph currentParagraph = null;
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

                    var allColorCodeCombinations = testParamGroup
                        .Select(row => CleanColourCode(row.Field<string>("Colour Code")))
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    var firstRow = testParamGroup.First();

                    var methods = testParamGroup
                        .Select(r => r["Method*"]?.ToString()?.Trim() ?? "")
                        .Where(m => !string.IsNullOrEmpty(m))
                        .Distinct()
                        .ToList();
                    var equipment = testParamGroup
                        .Select(r => r["Equipment"]?.ToString()?.Trim() ?? "")
                        .Where(e => !string.IsNullOrEmpty(e))
                        .Distinct()
                        .ToList();

                    string methodLine = "Test Method: " + string.Join(", ", methods) +
                                         " analyzed by " + string.Join(", ", equipment);

                    if (isFirstGroup)
                    {
                        currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                        if (currentParagraph == null) continue;

                        RemoveStrayMethodSummaryParagraphs(currentParagraph);

                        currentParagraph.RemoveAllChildren<Run>();

                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
                        headerRun.AppendChild(new WordText()
                        {
                            Text = $"{lastUsedNumber}.  {firstRow["Test Parameter"]}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        currentParagraph.AppendChild(headerRun);

                        var methodPara = new Paragraph();
                        var methodRun = new Run();
                        methodRun.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        methodRun.AppendChild(new WordText() { Text = methodLine, Space = SpaceProcessingModeValues.Preserve });
                        methodPara.AppendChild(methodRun);
                        currentParagraph.Parent.InsertAfter(methodPara, currentParagraph);
                        currentParagraph = methodPara;
                    }
                    else if (currentParagraph != null)
                    {
                        var br1 = new Paragraph(new Run(new Break()));
                        currentParagraph.Parent.InsertAfter(br1, currentParagraph);
                        currentParagraph = br1;

                        var headerPara = new Paragraph();
                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
                        headerRun.AppendChild(new WordText()
                        {
                            Text = $"{lastUsedNumber}.  {firstRow["Test Parameter"]}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        headerPara.AppendChild(headerRun);
                        currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                        currentParagraph = headerPara;

                        var methodPara2 = new Paragraph();
                        var methodRun2 = new Run();
                        methodRun2.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        methodRun2.AppendChild(new WordText()
                        {
                            Text = methodLine,
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        methodPara2.AppendChild(methodRun2);
                        currentParagraph.Parent.InsertAfter(methodPara2, currentParagraph);
                        currentParagraph = methodPara2;
                    }

                    if (currentParagraph == null) { isFirstGroup = false; continue; }

                    // Build the table
                    int resultSubCols = allColorCodeCombinations.Count;
                    bool hasManyColumns = resultSubCols > 4;

                    var colWidths = CalculateColumnWidths(resultSubCols, true);
                    string dataFontSize = hasManyColumns ? "20" : "22";

                    var table = CreateStyledTableA4(colWidths, hasManyColumns);

                    // Header row
                    var headerRow = new TableRow();
                    headerRow.AppendChild(new TableRowProperties(
                        new TableRowHeight() { Val = hasManyColumns ? 600U : 800U, HeightType = HeightRuleValues.Exact }
                    ));

                    headerRow.AppendChild(MakeHeaderCellA4("Element/Compound", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("CAS\nNumber", hasManyColumns));
                    foreach (var code in allColorCodeCombinations)
                        headerRow.AppendChild(MakeHeaderCellA4($"Result\n(mg/kg)\n({code})", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("Requirement\n(mg/kg)", hasManyColumns));
                    table.AppendChild(headerRow);

                    // Data rows
                    bool[] combinationHasDetection = new bool[resultSubCols];

                    foreach (var elementGroup in elementGroups)
                    {
                        var firstEl = elementGroup.First();
                        var dataRow = new TableRow();

                        dataRow.AppendChild(MakeDataCellA4(firstEl["Element"].ToString(), leftAlign: true, dataFontSize));
                        dataRow.AppendChild(MakeDataCellA4(firstEl["CAS Number"].ToString(), fontSize: dataFontSize));

                        var resultCells = new List<TableCell>();
                        for (int i = 0; i < resultSubCols; i++)
                            resultCells.Add(MakeDataCellA4("N.D", fontSize: dataFontSize));
                        foreach (var rc in resultCells) dataRow.AppendChild(rc);

                        foreach (var row in elementGroup)
                        {
                            string result = row["Result(mg/kg)"].ToString();
                            string code = CleanColourCode(row["Colour Code"]?.ToString());
                            if (string.IsNullOrWhiteSpace(code)) continue;

                            int idx = allColorCodeCombinations.IndexOf(code);
                            if (idx < 0) continue;

                            var textEl = resultCells[idx]
                                .Descendants<WordText>().FirstOrDefault();
                            if (textEl != null) textEl.Text = result;

                            if (result.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                result.Equals("Detected", StringComparison.OrdinalIgnoreCase))
                                combinationHasDetection[idx] = true;
                        }

                        dataRow.AppendChild(MakeDataCellA4(firstEl["Requirement"].ToString(), fontSize: dataFontSize));
                        table.AppendChild(dataRow);
                    }

                    // Remark row
                    var remarkRow = new TableRow();

                    remarkRow.AppendChild(MakeRemarkCellA4("Remark", null, true, dataFontSize));
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));

                    for (int i = 0; i < resultSubCols; i++)
                    {
                        bool fail = combinationHasDetection[i];
                        remarkRow.AppendChild(MakeRemarkCellA4(
                            fail ? "FAIL" : "PASS",
                            fail ? "FF0000" : "008000",
                            true, dataFontSize));
                    }
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));
                    table.AppendChild(remarkRow);

                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;

                    // Method Detection Limit line
                    var mdlParts = elementGroups
                        .Select(eg => eg.First())
                        .Where(r => !string.IsNullOrWhiteSpace(r["MDL"]?.ToString()))
                        .Select(r => $"{r["Element"]}: {r["MDL"]} (mg/kg)")
                        .ToList();

                    if (mdlParts.Count > 0)
                    {
                        var mdlPara = new Paragraph();
                        var mdlRun = new Run();
                        mdlRun.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        mdlRun.AppendChild(new WordText()
                        {
                            Text = "Method Detection Limit= " + string.Join(", ", mdlParts),
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        mdlPara.AppendChild(mdlRun);
                        table.Parent.InsertAfter(mdlPara, table);
                        currentParagraph = mdlPara;
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateTable18: {ex.Message}");
            }
        }

        private void CreateTable18BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Parameter"))
                    .ToList();

                Paragraph currentParagraph = null;
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

                    var allColorCodeCombinations = testParamGroup
                        .Select(row => CleanColourCode(row.Field<string>("Colour Code")))
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    var firstRow = testParamGroup.First();
                    var methods = testParamGroup
                        .Select(r => r["Method*"]?.ToString()?.Trim() ?? "")
                        .Where(m => !string.IsNullOrEmpty(m)).Distinct().ToList();
                    var equipment = testParamGroup
                        .Select(r => r["Equipment"]?.ToString()?.Trim() ?? "")
                        .Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();

                    string methodLine = "Metode Tes: " + string.Join(", ", methods) +
                                         " dianalisis dengan " + string.Join(", ", equipment);

                    if (isFirstGroup)
                    {
                        currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                        if (currentParagraph == null) continue;

                        RemoveStrayMethodSummaryParagraphs(currentParagraph);

                        currentParagraph.RemoveAllChildren<Run>();

                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
                        headerRun.AppendChild(new WordText()
                        {
                            Text = $"{lastUsedNumber}.  {firstRow["Test Parameter"]}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        currentParagraph.AppendChild(headerRun);

                        var mp = new Paragraph();
                        var mr = new Run();
                        mr.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        mr.AppendChild(new WordText() { Text = methodLine, Space = SpaceProcessingModeValues.Preserve });
                        mp.AppendChild(mr);
                        currentParagraph.Parent.InsertAfter(mp, currentParagraph);
                        currentParagraph = mp;
                    }
                    else if (currentParagraph != null)
                    {
                        var br1 = new Paragraph(new Run(new Break()));
                        currentParagraph.Parent.InsertAfter(br1, currentParagraph);
                        currentParagraph = br1;

                        var headerPara = new Paragraph();
                        var headerRun = new Run();
                        headerRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
                        headerRun.AppendChild(new WordText()
                        {
                            Text = $"{lastUsedNumber}.  {firstRow["Test Parameter"]}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        headerPara.AppendChild(headerRun);
                        currentParagraph.Parent.InsertAfter(headerPara, currentParagraph);
                        currentParagraph = headerPara;

                        var mp2 = new Paragraph();
                        var mr2 = new Run();
                        mr2.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        mr2.AppendChild(new WordText() { Text = methodLine, Space = SpaceProcessingModeValues.Preserve });
                        mp2.AppendChild(mr2);
                        currentParagraph.Parent.InsertAfter(mp2, currentParagraph);
                        currentParagraph = mp2;
                    }

                    if (currentParagraph == null) { isFirstGroup = false; continue; }

                    int resultSubCols = allColorCodeCombinations.Count;
                    bool hasManyColumns = resultSubCols > 4;

                    var colWidths = CalculateColumnWidths(resultSubCols, true);
                    string dataFontSize = hasManyColumns ? "20" : "22";

                    var table = CreateStyledTableA4(colWidths, hasManyColumns);

                    var headerRow = new TableRow();
                    headerRow.AppendChild(new TableRowProperties(
                        new TableRowHeight() { Val = hasManyColumns ? 600U : 800U, HeightType = HeightRuleValues.Exact }
                    ));

                    headerRow.AppendChild(MakeHeaderCellA4("Item Uji", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("CAS\nNumber", hasManyColumns));
                    foreach (var code in allColorCodeCombinations)
                        headerRow.AppendChild(MakeHeaderCellA4($"Hasil Uji\n(mg/kg)\n({code})", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("Persyaratan\n(mg/kg)", hasManyColumns));
                    table.AppendChild(headerRow);

                    // Track which color code columns have "D" (Detected) results
                    bool[] combinationHasDetection = new bool[resultSubCols];

                    foreach (var elementGroup in elementGroups)
                    {
                        var firstEl = elementGroup.First();
                        var dataRow = new TableRow();
                        dataRow.AppendChild(MakeDataCellA4(firstEl["Element"].ToString(), leftAlign: true, dataFontSize));
                        dataRow.AppendChild(MakeDataCellA4(firstEl["CAS Number"].ToString(), fontSize: dataFontSize));

                        var resultCells = new List<TableCell>();
                        for (int i = 0; i < resultSubCols; i++)
                            resultCells.Add(MakeDataCellA4("N.D", fontSize: dataFontSize));
                        foreach (var rc in resultCells) dataRow.AppendChild(rc);

                        foreach (var row in elementGroup)
                        {
                            string result = row["Result(mg/kg)"].ToString();
                            string code = CleanColourCode(row["Colour Code"]?.ToString());
                            if (string.IsNullOrWhiteSpace(code)) continue;
                            int idx = allColorCodeCombinations.IndexOf(code);
                            if (idx < 0) continue;
                            var textEl = resultCells[idx].Descendants<WordText>().FirstOrDefault();
                            if (textEl != null) textEl.Text = result;

                            // Check if result is "D" (Detected) - this means FAIL
                            if (result.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                result.Equals("Detected", StringComparison.OrdinalIgnoreCase))
                                combinationHasDetection[idx] = true;
                        }

                        dataRow.AppendChild(MakeDataCellA4(firstEl["Requirement"].ToString(), fontSize: dataFontSize));
                        table.AppendChild(dataRow);
                    }

                    // FIXED: Remark row - Show LULUS for N.D (Not Detected) and TIDAK LULUS for D (Detected)
                    var remarkRow = new TableRow();
                    remarkRow.AppendChild(MakeRemarkCellA4("Keterangan", null, true, dataFontSize));
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));

                    for (int i = 0; i < resultSubCols; i++)
                    {
                        // If there's any detection (D), it's TIDAK LULUS, otherwise LULUS
                        bool fail = combinationHasDetection[i];
                        remarkRow.AppendChild(MakeRemarkCellA4(
                            fail ? "TIDAK LULUS" : "LULUS",  // FIXED: Show LULUS when no detection
                            fail ? "FF0000" : "008000",
                            true,
                            dataFontSize));
                    }
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));
                    table.AppendChild(remarkRow);

                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;

                    var mdlParts = elementGroups
                        .Select(eg => eg.First())
                        .Where(r => !string.IsNullOrWhiteSpace(r["MDL"]?.ToString()))
                        .Select(r => $"{r["Element"]}: {r["MDL"]} (mg/kg)")
                        .ToList();

                    if (mdlParts.Count > 0)
                    {
                        var mdlPara = new Paragraph();
                        var mdlRun = new Run();
                        mdlRun.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        mdlRun.AppendChild(new WordText()
                        {
                            Text = "Batas Deteksi Metode= " + string.Join(", ", mdlParts),
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        mdlPara.AppendChild(mdlRun);
                        table.Parent.InsertAfter(mdlPara, table);
                        currentParagraph = mdlPara;
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateTable18BH: {ex.Message}");
            }
        }

        private void CreateTable19(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    foreach (var p in new[] { "{Number}", "{TestParameter2}", "{TestSubParameter}",
                       "{TestMethod}", "{Equipment2}", "{ResultTable2}",
                       "{AnalysisWasPerformed}", "{:}" })
                        ForceReplaceInDocument(doc, p, string.Empty);
                    return;
                }

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Item"))
                    .ToList();

                Paragraph currentParagraph = null;
                bool isFirstGroup = true;

                foreach (var group in groupedData)
                {
                    lastUsedNumber++;

                    DataTable groupDt = dt.Clone();
                    foreach (var row in group) groupDt.ImportRow(row);

                    string testParam = groupDt.Rows[0]["Test Parameter"].ToString();
                    string testItem = groupDt.Rows[0]["Test Item"].ToString();
                    string method = groupDt.Rows[0]["Method*"].ToString();
                    string equip = groupDt.Rows[0]["Equipment"].ToString();

                    if (isFirstGroup)
                    {
                        ForceReplaceInDocument(doc, "{Number}", $"{lastUsedNumber}.    ");
                        ForceReplaceInDocument(doc, "{TestParameter2}", testParam);
                        ForceReplaceInDocument(doc, "{TestSubParameter}", testItem);
                        ForceReplaceInDocument(doc, "{TestMethod}", method);
                        ForceReplaceInDocument(doc, "{Equipment2}", equip);
                        ForceReplaceInDocument(doc, "{AnalysisWasPerformed}", "*) Analysis was performed ");
                        ForceReplaceInDocument(doc, "{:}", " : ");
                    }

                    var allColorCodes = group
                        .Select(row => CleanColourCode(row.Field<string>("Colour Code")))
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .SelectMany(c => c.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Distinct()
                        .OrderBy(c => c)
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
                            string rawCode = CleanColourCode(row["Colour Code"]?.ToString());
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

                    if (isFirstGroup)
                        currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                    else if (currentParagraph != null)
                    {
                        currentParagraph.Parent.InsertAfter(new Paragraph(new Run(new Break())), currentParagraph);
                        currentParagraph = currentParagraph.NextSibling<Paragraph>() ?? currentParagraph;

                        var hp = new Paragraph();
                        var hr = new Run();
                        hr.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
                        hr.AppendChild(new WordText()
                        {
                            Text = $"{lastUsedNumber}.  {testParam} : {testItem}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        hp.AppendChild(hr);
                        currentParagraph.Parent.InsertAfter(hp, currentParagraph);
                        currentParagraph = hp;

                        var ap = new Paragraph();
                        var ar = new Run();
                        ar.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        ar.AppendChild(new WordText()
                        {
                            Text = $"*) Analysis was performed  {equip}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        ap.AppendChild(ar);
                        currentParagraph.Parent.InsertAfter(ap, currentParagraph);
                        currentParagraph = ap;
                    }

                    if (currentParagraph == null) { isFirstGroup = false; continue; }

                    bool hasManyColumns = modifiedDt.Columns.Count > 5;
                    var table = BuildTableFromDataTableA4(modifiedDt, hasManyColumns);

                    int colCount = modifiedDt.Columns.Count;
                    var remarkRow = new TableRow();

                    TableCell MakeRmkCell(string text, string colorHex, bool bold)
                    {
                        var c = new TableCell();
                        c.AppendChild(new TableCellProperties(
                            new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                            new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                        ));
                        var p = new Paragraph();
                        p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                        var r = new Run();
                        var rp = new RunProperties(new FontSize() { Val = "22" });
                        if (bold) rp.AppendChild(new Bold());
                        if (!string.IsNullOrEmpty(colorHex))
                            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = colorHex });
                        r.AppendChild(rp);
                        r.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                        p.AppendChild(r);
                        c.AppendChild(p);
                        return c;
                    }

                    remarkRow.AppendChild(MakeRmkCell("Remarks", null, true));

                    int firstResultCol = -1;
                    for (int col = 1; col < colCount; col++)
                    {
                        if (modifiedDt.Columns[col].ColumnName.StartsWith("Result (mg/kg)"))
                        {
                            if (firstResultCol < 0) firstResultCol = col;
                            int ci = col - firstResultCol;
                            if (ci < colorCodeHasDetection.Length)
                            {
                                bool fail = colorCodeHasDetection[ci];
                                remarkRow.AppendChild(MakeRmkCell(
                                    fail ? "FAIL" : "PASS",
                                    fail ? "FF0000" : "008000",
                                    true));
                            }
                        }
                        else
                        {
                            remarkRow.AppendChild(MakeRmkCell("", null, false));
                        }
                    }
                    table.AppendChild(remarkRow);

                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;

                    var mdlParts = group
                        .Where(row => !string.IsNullOrWhiteSpace(row["MDL"]?.ToString()))
                        .Select(row => $"{row["Element/Compound"]}: {row["MDL"]} mg/kg")
                        .Distinct()
                        .ToList();

                    if (mdlParts.Count > 0)
                    {
                        var mdlPara = new Paragraph();
                        var mdlRun = new Run();
                        mdlRun.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        mdlRun.AppendChild(new WordText()
                        {
                            Text = "Method Detection Limits- " + string.Join(", ", mdlParts),
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        mdlPara.AppendChild(mdlRun);
                        table.Parent.InsertAfter(mdlPara, table);
                        currentParagraph = mdlPara;
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateTable19: {ex.Message}");
            }
        }

        private void CreateTable19BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            lastUsedNumber = 0;
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    foreach (var p in new[] { "{Number}", "{TestParameter2}", "{TestSubParameter}",
               "{TestMethod}", "{Equipment2}", "{ResultTable2}",
               "{AnalysisWasPerformed}", "{:}" })
                        ForceReplaceInDocument(doc, p, string.Empty);
                    return;
                }

                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Barang Tes"))
                    .ToList();

                Paragraph currentParagraph = null;
                bool isFirstGroup = true;

                foreach (var group in groupedData)
                {
                    lastUsedNumber++;

                    DataTable groupDt = dt.Clone();
                    foreach (var row in group) groupDt.ImportRow(row);

                    string testParam = groupDt.Rows[0]["Parameter Uji"].ToString();
                    string testItem = groupDt.Rows[0]["Barang Tes"].ToString();
                    string method = groupDt.Rows[0]["Method*"].ToString();
                    string equip = groupDt.Rows[0]["Peralatan"].ToString();

                    if (isFirstGroup)
                    {
                        ForceReplaceInDocument(doc, "{Number}", $"{lastUsedNumber}.    ");
                        ForceReplaceInDocument(doc, "{TestParameter2}", testParam);
                        ForceReplaceInDocument(doc, "{TestSubParameter}", testItem);
                        ForceReplaceInDocument(doc, "{TestMethod}", method);
                        ForceReplaceInDocument(doc, "{Equipment2}", equip);
                        ForceReplaceInDocument(doc, "{AnalysisWasPerformed}", "*) Analisis dilakukan ");
                        ForceReplaceInDocument(doc, "{:}", " : ");
                    }

                    var allColorCodes = group
                        .Select(row => CleanColourCode(row.Field<string>("Colour Code")))
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .SelectMany(c => c.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();

                    DataTable modifiedDt = new DataTable();
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Parameter Uji" && col.ColumnName != "Barang Tes" &&
                            col.ColumnName != "Method*" && col.ColumnName != "Peralatan" &&
                            col.ColumnName != "Colour Code" && col.ColumnName != "Hasil(mg/kg)" &&
                            col.ColumnName != "Conclusion" && col.ColumnName != "MDL")
                            modifiedDt.Columns.Add(col.ColumnName);
                    }

                    foreach (var code in allColorCodes)
                        modifiedDt.Columns.Add($"Hasil (mg/kg) ({code})");

                    // Track which color code columns have "D" (Detected) results
                    bool[] colorCodeHasDetection = new bool[allColorCodes.Count];

                    var compoundGroups = group
                        .GroupBy(row => string.Join("|",
                            modifiedDt.Columns.Cast<DataColumn>()
                                .Where(c => !c.ColumnName.StartsWith("Hasil (mg/kg)"))
                                .Select(c => row[c.ColumnName]?.ToString() ?? "")))
                        .ToList();

                    foreach (var cg in compoundGroups)
                    {
                        DataRow newRow = modifiedDt.NewRow();
                        var fr = cg.First();
                        foreach (DataColumn col in modifiedDt.Columns)
                            if (!col.ColumnName.StartsWith("Hasil (mg/kg)"))
                                newRow[col.ColumnName] = fr[col.ColumnName];
                        for (int i = 0; i < allColorCodes.Count; i++)
                            newRow[$"Hasil (mg/kg) ({allColorCodes[i]})"] = "N.D";

                        foreach (var row in cg)
                        {
                            string rawCode = CleanColourCode(row["Colour Code"]?.ToString());
                            string val = row["Hasil(mg/kg)"]?.ToString()?.Trim() ?? "";
                            if (string.IsNullOrWhiteSpace(rawCode)) continue;
                            foreach (var code in rawCode.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string clean = code.Trim();
                                string cn = $"Hasil (mg/kg) ({clean})";
                                if (!modifiedDt.Columns.Contains(cn)) continue;
                                newRow[cn] = val;
                                int ci = allColorCodes.FindIndex(c => c.Equals(clean, StringComparison.OrdinalIgnoreCase));
                                if (ci >= 0 && (val.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                                val.Equals("Detected", StringComparison.OrdinalIgnoreCase)))
                                    colorCodeHasDetection[ci] = true;
                            }
                        }
                        modifiedDt.Rows.Add(newRow);
                    }

                    if (isFirstGroup)
                        currentParagraph = FindAndReplacePlaceholder(doc, placeholder);
                    else if (currentParagraph != null)
                    {
                        currentParagraph.Parent.InsertAfter(new Paragraph(new Run(new Break())), currentParagraph);
                        currentParagraph = currentParagraph.NextSibling<Paragraph>() ?? currentParagraph;

                        var hp = new Paragraph();
                        var hr = new Run();
                        hr.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "22" }));
                        hr.AppendChild(new WordText()
                        {
                            Text = $"{lastUsedNumber}.  {testParam} : {testItem}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        hp.AppendChild(hr);
                        currentParagraph.Parent.InsertAfter(hp, currentParagraph);
                        currentParagraph = hp;

                        var ap = new Paragraph();
                        var ar = new Run();
                        ar.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        ar.AppendChild(new WordText()
                        {
                            Text = $"*) Analisis dilakukan  {equip}",
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        ap.AppendChild(ar);
                        currentParagraph.Parent.InsertAfter(ap, currentParagraph);
                        currentParagraph = ap;
                    }

                    if (currentParagraph == null) { isFirstGroup = false; continue; }

                    bool hasManyColumns = modifiedDt.Columns.Count > 5;
                    var table = BuildTableFromDataTableA4(modifiedDt, hasManyColumns);

                    int colCount = modifiedDt.Columns.Count;
                    var remarkRow = new TableRow();

                    TableCell MakeRmk(string text, string colorHex, bool bold)
                    {
                        var c = new TableCell();
                        c.AppendChild(new TableCellProperties(
                            new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                            new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                        ));
                        var p = new Paragraph();
                        p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                        var r = new Run();
                        var rp = new RunProperties(new FontSize() { Val = "22" });
                        if (bold) rp.AppendChild(new Bold());
                        if (!string.IsNullOrEmpty(colorHex))
                            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = colorHex });
                        r.AppendChild(rp);
                        r.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                        p.AppendChild(r);
                        c.AppendChild(p);
                        return c;
                    }

                    remarkRow.AppendChild(MakeRmk("Keterangan", null, true));
                    int firstResCol = -1;
                    for (int col = 1; col < colCount; col++)
                    {
                        if (modifiedDt.Columns[col].ColumnName.StartsWith("Hasil (mg/kg)"))
                        {
                            if (firstResCol < 0) firstResCol = col;
                            int ci = col - firstResCol;
                            if (ci < colorCodeHasDetection.Length)
                            {
                                bool fail = colorCodeHasDetection[ci];
                                // FIXED: Show LULUS for N.D (No Detection) and TIDAK LULUS for D (Detection)
                                remarkRow.AppendChild(MakeRmk(
                                    fail ? "TIDAK LULUS" : "LULUS",  // FIXED: Show LULUS when no detection
                                    fail ? "FF0000" : "008000",
                                    true));
                            }
                        }
                        else
                            remarkRow.AppendChild(MakeRmk("", null, false));
                    }
                    table.AppendChild(remarkRow);

                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;

                    var mdlParts = group
                        .Where(row => !string.IsNullOrWhiteSpace(row["MDL"]?.ToString()))
                        .Select(row => $"{row["Senyawa"]}: {row["MDL"]} mg/kg")
                        .Distinct().ToList();

                    if (mdlParts.Count > 0)
                    {
                        var mdlPara = new Paragraph();
                        var mdlRun = new Run();
                        mdlRun.AppendChild(new RunProperties(new FontSize() { Val = "20" }));
                        mdlRun.AppendChild(new WordText()
                        {
                            Text = "Batas Deteksi Metode- " + string.Join(", ", mdlParts),
                            Space = SpaceProcessingModeValues.Preserve
                        });
                        mdlPara.AppendChild(mdlRun);
                        table.Parent.InsertAfter(mdlPara, table);
                        currentParagraph = mdlPara;
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateTable19BH: {ex.Message}");
            }
        }

        // ========== TABLE 18-19 HELPER METHODS ==========
        private List<int> CalculateColumnWidths(int resultColumnsCount, bool isTable18 = true)
        {
            var widths = new List<int>();

            if (isTable18)
            {
                const int ELEMENT_COL = 2500;
                const int CAS_COL = 1800;
                const int REQUIREMENT_COL = 1800;

                int fixedWidth = ELEMENT_COL + CAS_COL + REQUIREMENT_COL;
                int availableForResults = USABLE_WIDTH_TWIPS - fixedWidth;

                int resultColWidth = resultColumnsCount > 0
                    ? Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, availableForResults / resultColumnsCount))
                    : MIN_COLUMN_WIDTH;

                widths.Add(ELEMENT_COL);
                widths.Add(CAS_COL);
                for (int i = 0; i < resultColumnsCount; i++)
                    widths.Add(resultColWidth);
                widths.Add(REQUIREMENT_COL);
            }
            else
            {
                const int FIXED_COL_WIDTH = 2000;
                int fixedWidth = FIXED_COL_WIDTH * 2;
                int availableForResults = USABLE_WIDTH_TWIPS - fixedWidth;

                int resultColWidth = resultColumnsCount > 0
                    ? Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, availableForResults / resultColumnsCount))
                    : MIN_COLUMN_WIDTH;

                widths.Add(FIXED_COL_WIDTH);
                widths.Add(FIXED_COL_WIDTH);
                for (int i = 0; i < resultColumnsCount; i++)
                    widths.Add(resultColWidth);
            }

            int totalWidth = widths.Sum();
            if (totalWidth > USABLE_WIDTH_TWIPS)
            {
                double scaleFactor = (double)USABLE_WIDTH_TWIPS / totalWidth;
                for (int i = 0; i < widths.Count; i++)
                {
                    widths[i] = Math.Max(MIN_COLUMN_WIDTH, (int)(widths[i] * scaleFactor));
                }
            }

            return widths;
        }

        private Table CreateStyledTableA4(List<int> colWidths, bool hasManyColumns)
        {
            var table = new Table();

            int totalWidth = colWidths.Sum();
            if (totalWidth > USABLE_WIDTH_TWIPS)
            {
                double scaleFactor = (double)USABLE_WIDTH_TWIPS / totalWidth;
                for (int i = 0; i < colWidths.Count; i++)
                {
                    colWidths[i] = Math.Max(MIN_COLUMN_WIDTH, (int)(colWidths[i] * scaleFactor));
                }
            }

            var tblProps = new TableProperties(
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = Math.Min(colWidths.Sum(), USABLE_WIDTH_TWIPS).ToString(), Type = TableWidthUnitValues.Dxa },
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                )
            );
            table.AppendChild(tblProps);

            var grid = new TableGrid();
            foreach (var w in colWidths)
                grid.AppendChild(new GridColumn() { Width = w.ToString() });
            table.AppendChild(grid);

            return table;
        }

        private Table BuildTableFromDataTableA4(DataTable modifiedDt, bool hasManyColumns = false)
        {
            if (modifiedDt == null || modifiedDt.Rows.Count == 0)
                return new Table();

            int resultColumns = modifiedDt.Columns.Count - 2;
            var colWidths = CalculateColumnWidths(resultColumns, false);

            var table = CreateStyledTableA4(colWidths, hasManyColumns);

            var headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = hasManyColumns ? 600U : 800U, HeightType = HeightRuleValues.Exact }
            ));

            foreach (DataColumn col in modifiedDt.Columns)
            {
                var cell = MakeHeaderCellA4(col.ColumnName, hasManyColumns);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            string dataFontSize = hasManyColumns ? "20" : "22";
            foreach (DataRow dataRow in modifiedDt.Rows)
            {
                var row = new TableRow();
                foreach (DataColumn col in modifiedDt.Columns)
                {
                    string value = dataRow[col]?.ToString() ?? "";
                    bool isFirstCol = col.Ordinal == 0;
                    var cell = MakeDataCellA4(value, isFirstCol, dataFontSize);
                    row.AppendChild(cell);
                }
                table.AppendChild(row);
            }

            return table;
        }

        private TableCell MakeHeaderCellA4(string text, bool hasManyColumns, bool isBH = false)
        {
            var c = new TableCell();
            c.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
            ));
            var p = new Paragraph();
            p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            var r = new Run();
            r.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = hasManyColumns ? "18" : "20" }));
            r.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
            p.AppendChild(r);
            c.AppendChild(p);
            return c;
        }

        private TableCell MakeDataCellA4(string text, bool leftAlign = false, string fontSize = "22")
        {
            var c = new TableCell();
            c.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            ));
            var p = new Paragraph();
            p.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = leftAlign ? JustificationValues.Left : JustificationValues.Center }
            );
            var r = new Run();
            r.AppendChild(new RunProperties(new FontSize() { Val = fontSize }));
            r.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
            p.AppendChild(r);
            c.AppendChild(p);
            return c;
        }

        private TableCell MakeRemarkCellA4(string text, string colorHex, bool isBold, string fontSize = "22")
        {
            var c = new TableCell();
            c.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
            ));
            var p = new Paragraph();
            p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            var r = new Run();
            var rProps = new RunProperties(new FontSize() { Val = fontSize });
            if (isBold) rProps.AppendChild(new Bold());
            if (!string.IsNullOrEmpty(colorHex))
                rProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = colorHex });
            r.AppendChild(rProps);
            r.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
            p.AppendChild(r);
            c.AppendChild(p);
            return c;
        }

        private void RemoveStrayMethodSummaryParagraphs(OpenXmlElement anchor)
        {
            if (anchor?.Parent == null) return;

            var parent = anchor.Parent;
            var candidates = parent.Elements<Paragraph>()
                .Where(p =>
                {
                    string text = p.InnerText ?? "";
                    if (string.IsNullOrWhiteSpace(text)) return false;

                    bool looksLikeMethodLine =
                        text.TrimStart().StartsWith("Test Method:", StringComparison.OrdinalIgnoreCase) ||
                        text.TrimStart().StartsWith("Metoda Uji:", StringComparison.OrdinalIgnoreCase) ||
                        text.TrimStart().StartsWith("Metode Uji:", StringComparison.OrdinalIgnoreCase);

                    int commaCount = text.Count(c => c == ',');
                    bool endsWithDanglingEquipment =
                        Regex.IsMatch(text.TrimEnd(), @"(analyzed by|dianalisis dengan)\s*$", RegexOptions.IgnoreCase);

                    return looksLikeMethodLine && (commaCount >= 3 || endsWithDanglingEquipment);
                })
                .ToList();

            foreach (var stray in candidates)
                stray.Remove();
        }

        private string CleanColourCode(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            return Regex.Replace(raw, @"[()]", "").Trim();
        }

        // ========== RESULT TABLE (PASS/FAIL) METHODS ==========
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
                System.Diagnostics.Debug.WriteLine($"Error in InsertResultTableIntoDocument: {ex.Message}");
            }
        }

        private Table BuildPassFailCheckboxTable(bool isFirstRowChecked, bool isSecondRowChecked, string firstLabel, string secondLabel)
        {
            const string BORDER_COLOR = "1f4e79";
            const string TEXT_COLOR = "000080";
            const string BOX_WIDTH = "380";  // Increased from 350
            const string LABEL_WIDTH = "1800"; // Increased from 1600 to accommodate "TIDAK LULUS"
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

            table.AppendChild(BuildCheckboxRow(isFirstRowChecked, firstLabel,
                BOX_WIDTH, LABEL_WIDTH, BORDER_COLOR, TEXT_COLOR, BORDER_SIZE));

            table.AppendChild(BuildCheckboxRow(isSecondRowChecked, secondLabel,
                BOX_WIDTH, LABEL_WIDTH, BORDER_COLOR, TEXT_COLOR, BORDER_SIZE));

            return table;
        }
        private TableRow BuildCheckboxRow(bool isChecked, string labelText, string boxWidth, string labelWidth, string borderColor, string textColor, uint borderSize)
        {
            var row = new TableRow();
            row.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 420U, HeightType = HeightRuleValues.Exact }
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
                        new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = textColor }
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
                    new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = textColor }
                ),
                new WordText() { Text = labelText, Space = SpaceProcessingModeValues.Preserve }
            );
            para.AppendChild(run);
            labelCell.AppendChild(para);
            row.AppendChild(labelCell);

            return row;
        }

        // ========== CHEMICAL DETAILS PROCESSING ==========
        private void ProcessChemicalDetails(WordprocessingDocument doc, DataTable ChemicalDetails, string SampleUID, int ReportIDGet, bool isBH)
        {
            if (ChemicalDetails == null || ChemicalDetails.Rows.Count == 0) return;

            DataRow row = ChemicalDetails.Rows[0];

            string GetSafeValue(string columnName)
            {
                if (!row.Table.Columns.Contains(columnName)) return "";
                return row[columnName] == DBNull.Value || row[columnName] == null ? "" : row[columnName].ToString();
            }

            string GetSafeDate(string columnName)
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

            string GetSafeDate1(string columnName)
            {
                if (!row.Table.Columns.Contains(columnName)) return "";
                if (row[columnName] == DBNull.Value || row[columnName] == null) return "";
                string dateString = row[columnName].ToString();
                DateTime dateValue;
                if (DateTime.TryParseExact(dateString, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out dateValue))
                {
                    return dateValue.ToString("dd MMMM yyyy");
                }
                if (DateTime.TryParse(dateString, System.Globalization.CultureInfo.InvariantCulture,
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

            string testReportValidUntil = GetSafeDate1("DateOfExp");
            AddReplacementIfExists(replacements, "{Testreportvaliduntil}", testReportValidUntil);
            AddReplacementIfExists(replacements, "{TestReportvaliduntil}", testReportValidUntil);

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

            string issueDate = GetSafeDate("IssueDate");
            AddReplacementIfExists(replacements, "{IssueDate}", issueDate);
            AddReplacementIfExists(replacements, "{Issuedate}", issueDate);

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

            string materialType = GetSafeValue("MaterialType");
            AddReplacementIfExists(replacements, "{MaterialType}", materialType);
            AddReplacementIfExists(replacements, "{Materialtype}", materialType);

            AddReplacementIfExists(replacements, "{ItemStyleNo}", GetSafeValue("ItemStyleNo"));
            AddReplacementIfExists(replacements, "{MaterialName_Code}", GetSafeValue("MaterialName_Code"));
            AddReplacementIfExists(replacements, "{ColorName_Code}", GetSafeValue("ColorName_Code"));
            AddReplacementIfExists(replacements, "{BatchNo}", GetSafeValue("BatchNo"));
            AddReplacementIfExists(replacements, "{WorkorderNo}", GetSafeValue("WorkorderNumber"));
            AddReplacementIfExists(replacements, "{TestCategory}", GetSafeValue("GroupofProduct"));
            AddReplacementIfExists(replacements, "{TestMethod}", GetSafeValue("TestMethod"));
            AddReplacementIfExists(replacements, "{ColorName}", GetSafeValue("Color"));
            AddReplacementIfExists(replacements, "{TestSpecification}", GetSafeValue("ProductSpecification"));
            AddReplacementIfExists(replacements, "{TestPackage}", GetSafeValue("GroupofProduct"));

            string testReportNo = GetSafeValue("TestReportNo");
            AddReplacementIfExists(replacements, "{TestReportNo.}", testReportNo);
            AddReplacementIfExists(replacements, "{TestReportNo}", testReportNo);

            AddReplacementIfExists(replacements, "{DocNo}", "TSQ-LAB-QAF-089");

            FindAndReplaceMultiple(doc, replacements);

            string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
            if (File.Exists(imageUrl))
            {
                ReplacePlaceholderWithImage(doc, "{QR_Code}", imageUrl);
            }

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

            if (ReportIDGet == 28)
            {
                AdditionalDataAdded(doc, "{AdditionalData}");
                AdditionalDataAdded_BH(doc, "{AdditionalData}");
            }
            else
            {
                FindAndReplace(doc, "{AdditionalData}", "");
            }
        }

        // ========== ADDITIONAL DATA METHODS ==========

                
        public void AdditionalDataAdded(WordprocessingDocument doc, string placeholder)
        {
            try
            {
                string path = Server.MapPath("~/FilesUpload/");
                string sampleIDFromViewState = ViewState["SampleIDGet"]?.ToString();

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                var candidateNames = new List<string>();
                if (!string.IsNullOrEmpty(sampleIDFromViewState)) candidateNames.Add(sampleIDFromViewState);

                string filePath = null;
                foreach (var candidate in candidateNames)
                {
                    string tryPath = path + candidate + ".xlsx";
                    if (File.Exists(tryPath))
                    {
                        filePath = tryPath;
                        break;
                    }
                }

                if (filePath == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"AdditionalData workbook not found. Looked for: {string.Join(", ", candidateNames.Select(c => path + c + ".xlsx"))}");
                    RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                    return;
                }

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var usedRange = worksheet.RangeUsed();
                    if (usedRange == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"AdditionalData workbook '{filePath}' has no used range / is empty.");
                        return;
                    }

                    int rowCount = usedRange.RowCount();
                    int colCount = Math.Max(1, usedRange.ColumnCount());

                    var colWidths = new List<int>();
                    int totalWidth = USABLE_WIDTH_TWIPS;

                    if (colCount > 8)
                    {
                        int perColWidth = Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, totalWidth / colCount));
                        for (int i = 0; i < colCount; i++)
                            colWidths.Add(perColWidth);
                    }
                    else
                    {
                        int[] defaultWidths = { 1800, 2000, 2200, 2400, 2000, 2200, 1800, 2400 };
                        for (int i = 0; i < colCount && i < defaultWidths.Length; i++)
                            colWidths.Add(defaultWidths[i]);

                        int remainingCols = colCount - defaultWidths.Length;
                        if (remainingCols > 0)
                        {
                            int usedWidth = colWidths.Sum();
                            int remainingWidth = totalWidth - usedWidth;
                            int extraColWidth = remainingWidth / remainingCols;
                            for (int i = 0; i < remainingCols; i++)
                                colWidths.Add(Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, extraColWidth)));
                        }
                    }

                    int totalCalculatedWidth = colWidths.Sum();
                    if (totalCalculatedWidth > USABLE_WIDTH_TWIPS)
                    {
                        double scaleFactor = (double)USABLE_WIDTH_TWIPS / totalCalculatedWidth;
                        for (int i = 0; i < colWidths.Count; i++)
                        {
                            colWidths[i] = Math.Max(MIN_COLUMN_WIDTH, (int)(colWidths[i] * scaleFactor));
                        }
                    }

                    var table = new Table();

                    var tableProps = new TableProperties();
                    tableProps.AppendChild(new TableLayout() { Type = TableLayoutValues.Fixed });
                    tableProps.AppendChild(new TableWidth()
                    {
                        Width = Math.Min(colWidths.Sum(), USABLE_WIDTH_TWIPS).ToString(),
                        Type = TableWidthUnitValues.Dxa
                    });

                    var borders = new TableBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                        new RightBorder() { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                    );
                    tableProps.AppendChild(borders);
                    table.AppendChild(tableProps);

                    var grid = new TableGrid();
                    foreach (var width in colWidths)
                    {
                        grid.AppendChild(new GridColumn() { Width = width.ToString() });
                    }
                    table.AppendChild(grid);

                    bool hasManyColumns = colCount > 6;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        var tableRow = new TableRow();

                        int rowHeight = (row == 1) ? (hasManyColumns ? 600 : 700) : (hasManyColumns ? 400 : 500);
                        tableRow.AppendChild(new TableRowProperties(
                            new TableRowHeight() { Val = (uint)rowHeight, HeightType = HeightRuleValues.AtLeast }
                        ));

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cell = new TableCell();
                            var value = usedRange.Cell(row, col).GetString();

                            var cellProps = new TableCellProperties(
                                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                            );

                            if (row == 1)
                            {
                                cellProps.AppendChild(new Shading()
                                {
                                    Val = ShadingPatternValues.Clear,
                                    Color = "auto",
                                    Fill = "D3D3D3"
                                });
                            }
                            cell.AppendChild(cellProps);

                            var para = new Paragraph();
                            var run = new Run();

                            int fontSize = (row == 1)
                                ? (hasManyColumns ? 18 : 20)
                                : (hasManyColumns ? 16 : 18);

                            var runProps = new RunProperties(new FontSize() { Val = fontSize.ToString() });
                            if (row == 1)
                            {
                                runProps.AppendChild(new Bold());
                            }
                            run.AppendChild(runProps);

                            if (value.Contains("\n") || value.Contains("\r\n"))
                            {
                                var lines = value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (i > 0)
                                        run.AppendChild(new Break());
                                    run.AppendChild(new WordText()
                                    {
                                        Text = lines[i],
                                        Space = SpaceProcessingModeValues.Preserve
                                    });
                                }
                            }
                            else
                            {
                                run.AppendChild(new WordText()
                                {
                                    Text = value,
                                    Space = SpaceProcessingModeValues.Preserve
                                });
                            }

                            para.AppendChild(run);
                            para.ParagraphProperties = new ParagraphProperties(
                                new Justification() { Val = JustificationValues.Center }
                            );
                            cell.AppendChild(para);
                            tableRow.AppendChild(cell);
                        }

                        table.AppendChild(tableRow);
                    }

                    targetParagraph.Parent.InsertAfter(table, targetParagraph);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AdditionalDataAdded: {ex}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public void AdditionalDataAdded_BH(WordprocessingDocument doc, string placeholder)
        {
            try
            {
                string path = Server.MapPath("~/FilesUpload/");
                string sampleIDFromViewState = ViewState["SampleIDGet"]?.ToString();

                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
                if (targetParagraph == null) return;

                var candidateNames = new List<string>();
                if (!string.IsNullOrEmpty(sampleIDFromViewState)) candidateNames.Add(sampleIDFromViewState);

                string filePath = null;
                foreach (var candidate in candidateNames)
                {
                    string tryPath = path + candidate + ".xlsx";
                    if (File.Exists(tryPath))
                    {
                        filePath = tryPath;
                        break;
                    }
                }

                if (filePath == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"AdditionalData workbook not found. Looked for: {string.Join(", ", candidateNames.Select(c => path + c + ".xlsx"))}");
                    RemovePlaceholderFromParagraph(targetParagraph, placeholder);
                    return;
                }

                RemovePlaceholderFromParagraph(targetParagraph, placeholder);

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var usedRange = worksheet.RangeUsed();
                    if (usedRange == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"AdditionalData workbook '{filePath}' has no used range / is empty.");
                        return;
                    }

                    int rowCount = usedRange.RowCount();
                    int colCount = Math.Max(1, usedRange.ColumnCount());

                    var colWidths = new List<int>();
                    int totalWidth = USABLE_WIDTH_TWIPS;

                    if (colCount > 8)
                    {
                        int perColWidth = Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, totalWidth / colCount));
                        for (int i = 0; i < colCount; i++)
                            colWidths.Add(perColWidth);
                    }
                    else
                    {
                        int[] defaultWidths = { 1800, 2000, 2200, 2400, 2000, 2200, 1800, 2400 };
                        for (int i = 0; i < colCount && i < defaultWidths.Length; i++)
                            colWidths.Add(defaultWidths[i]);

                        int remainingCols = colCount - defaultWidths.Length;
                        if (remainingCols > 0)
                        {
                            int usedWidth = colWidths.Sum();
                            int remainingWidth = totalWidth - usedWidth;
                            int extraColWidth = remainingWidth / remainingCols;
                            for (int i = 0; i < remainingCols; i++)
                                colWidths.Add(Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, extraColWidth)));
                        }
                    }

                    int totalCalculatedWidth = colWidths.Sum();
                    if (totalCalculatedWidth > USABLE_WIDTH_TWIPS)
                    {
                        double scaleFactor = (double)USABLE_WIDTH_TWIPS / totalCalculatedWidth;
                        for (int i = 0; i < colWidths.Count; i++)
                        {
                            colWidths[i] = Math.Max(MIN_COLUMN_WIDTH, (int)(colWidths[i] * scaleFactor));
                        }
                    }

                    var table = new Table();

                    var tableProps = new TableProperties();
                    tableProps.AppendChild(new TableLayout() { Type = TableLayoutValues.Fixed });
                    tableProps.AppendChild(new TableWidth()
                    {
                        Width = Math.Min(colWidths.Sum(), USABLE_WIDTH_TWIPS).ToString(),
                        Type = TableWidthUnitValues.Dxa
                    });

                    var borders = new TableBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                        new RightBorder() { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                    );
                    tableProps.AppendChild(borders);
                    table.AppendChild(tableProps);

                    var grid = new TableGrid();
                    foreach (var width in colWidths)
                    {
                        grid.AppendChild(new GridColumn() { Width = width.ToString() });
                    }
                    table.AppendChild(grid);

                    bool hasManyColumns = colCount > 6;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        var tableRow = new TableRow();

                        int rowHeight = (row == 1) ? (hasManyColumns ? 600 : 700) : (hasManyColumns ? 400 : 500);
                        tableRow.AppendChild(new TableRowProperties(
                            new TableRowHeight() { Val = (uint)rowHeight, HeightType = HeightRuleValues.AtLeast }
                        ));

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cell = new TableCell();
                            var value = usedRange.Cell(row, col).GetString();

                            var cellProps = new TableCellProperties(
                                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                            );

                            if (row == 1)
                            {
                                cellProps.AppendChild(new Shading()
                                {
                                    Val = ShadingPatternValues.Clear,
                                    Color = "auto",
                                    Fill = "D3D3D3"
                                });
                            }
                            cell.AppendChild(cellProps);

                            var para = new Paragraph();
                            var run = new Run();

                            int fontSize = (row == 1)
                                ? (hasManyColumns ? 18 : 20)
                                : (hasManyColumns ? 16 : 18);

                            var runProps = new RunProperties(new FontSize() { Val = fontSize.ToString() });
                            if (row == 1)
                            {
                                runProps.AppendChild(new Bold());
                            }
                            run.AppendChild(runProps);

                            if (value.Contains("\n") || value.Contains("\r\n"))
                            {
                                var lines = value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (i > 0)
                                        run.AppendChild(new Break());
                                    run.AppendChild(new WordText()
                                    {
                                        Text = lines[i],
                                        Space = SpaceProcessingModeValues.Preserve
                                    });
                                }
                            }
                            else
                            {
                                run.AppendChild(new WordText()
                                {
                                    Text = value,
                                    Space = SpaceProcessingModeValues.Preserve
                                });
                            }

                            para.AppendChild(run);
                            para.ParagraphProperties = new ParagraphProperties(
                                new Justification() { Val = JustificationValues.Center }
                            );
                            cell.AppendChild(para);
                            tableRow.AppendChild(cell);
                        }

                        table.AppendChild(tableRow);
                    }

                    targetParagraph.Parent.InsertAfter(table, targetParagraph);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AdditionalDataAdded_BH: {ex}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        


    }
}