using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using ZXing;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
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
                int sampleID = 0;
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
                }

                string s = Request.QueryString["s"]?.ToString() ?? "";
                _cm.sampleId = Convert.ToInt32(s);
                int ReportIDGet = Convert.ToInt32(Request.QueryString["c"]?.ToString() ?? "0");
                int ID = Convert.ToInt32(s);
                string SampleUID = "";
                int revisionCount = 0;

                DataTable dtSID = _cm.GetRevisionCountSampleIdWise(sampleID);
                if (dtSID != null || dtSID.Rows.Count > 0)
                {
                    DataRow row = dtSID.Rows[0];
                    revisionCount = Convert.IsDBNull(row["RevisionCount"]) ? 0 : Convert.ToInt32(row["RevisionCount"]);
                    string QRCodeNumber = row["QR_CodeNumber"].ToString();
                    string QRCodeNumber_BH = row["QR_CodeNumber_BH"].ToString();
                }
                

                DataTable dt = _cm.getSampleUID(ID);
                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    if (revisionCount == 0)
                    {
                        GenerateQR(SampleUID);
                        GenerateQR_BH(SampleUID);
                    }
                }

                DataTable ChemicalDetails = _cm.getChemicalDetails(sampleID);
                DataTable dt1 = _cm.getReportQuery(ReportIDGet.ToString());
                List<string> pdfPaths = new List<string>(); // Store only PDF paths for download

                if (dt1.Rows.Count > 0)
                {
                    string template = dt1.Rows[0]["Template"].ToString();
                    string templatePath = Path.Combine(path, template);
                    string templatePath_BH = GetTemplatePath_BH(template, path);

                    try
                    {
                        if (Directory.Exists(path))
                        {
                            string[] allTemplates = Directory.GetFiles(path, "*.docx");
                            Debug.WriteLine($"All templates in Inputfiles ({allTemplates.Length} files):");
                            foreach (var file in allTemplates)
                            {
                                Debug.WriteLine($"  - {Path.GetFileName(file)}");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Inputfiles directory does not exist: {path}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error listing templates: {ex.Message}");
                    }

                    // Process Main Document
                    if (File.Exists(templatePath))
                    {
                        try
                        {
                            byte[] mainDocBytes = File.ReadAllBytes(templatePath);
                            byte[] mainProcessedBytes = ProcessDocumentMain(mainDocBytes, ChemicalDetails, SampleUID, ReportIDGet, s, dt1);

                            // Define file paths
                            string docxFileName = $"TestReport-{SampleUID}.docx";
                            string pdfFileName = $"TestReport-{SampleUID}.pdf";

                            string outputDocxPath = Path.Combine(pathout, docxFileName);
                            string outputPdfPath = Path.Combine(pathout, pdfFileName);

                            // ========== SAVE WORD DOCUMENT ==========
                            File.WriteAllBytes(outputDocxPath, mainProcessedBytes);
                            Debug.WriteLine($"Word saved: {outputDocxPath}");

                            // ========== CONVERT TO PDF AND SAVE ==========
                            try
                            {
                                byte[] pdfBytes = ConvertDocxToPdf_Syncfusion(mainProcessedBytes);
                                File.WriteAllBytes(outputPdfPath, pdfBytes);
                                Debug.WriteLine($"PDF saved: {outputPdfPath}");

                                // Verify PDF was created successfully
                                if (File.Exists(outputPdfPath) && new FileInfo(outputPdfPath).Length > 0)
                                {
                                    pdfPaths.Add($"/TestReport/{pdfFileName}");

                                    string qrCodeNumber = $"{SampleUID}.png";
                                    string qrCodeNumberBH = $"{SampleUID}_BH.png";

                                    if (revisionCount == 0)
                                    {
                                        bool updateSuccess = _cm.UpdateQRCodeNumbers(sampleID, qrCodeNumber, qrCodeNumberBH, 0);                                        
                                    }

                                }


                            }
                            catch (Exception pdfEx)
                            {
                                Debug.WriteLine($"PDF conversion error for main document: {pdfEx.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error processing main document: {ex.Message}");
                        }
                    }

                    // ========== PROCESS BH DOCUMENT ==========
                    // Check if BH template exists
                    bool bhTemplateExists = !string.IsNullOrEmpty(templatePath_BH) && File.Exists(templatePath_BH);

                    // If BH template doesn't exist, try to use main template as fallback
                    if (!bhTemplateExists && File.Exists(templatePath))
                    {
                        Debug.WriteLine("BH template not found, using main template as fallback");
                        // Create BH template path from main template
                        string baseName = Path.GetFileNameWithoutExtension(template);
                        string extension = Path.GetExtension(template);
                        templatePath_BH = Path.Combine(path, $"{baseName}_BH{extension}");

                        // If BH template doesn't exist, copy main template to create it
                        if (!File.Exists(templatePath_BH))
                        {
                            try
                            {
                                File.Copy(templatePath, templatePath_BH);
                                Debug.WriteLine($"Created BH template by copying: {templatePath_BH}");
                                bhTemplateExists = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error creating BH template: {ex.Message}");
                            }
                        }
                    }

                    if (bhTemplateExists || File.Exists(templatePath_BH))
                    {
                        try
                        {
                            byte[] bhDocBytes = File.ReadAllBytes(templatePath_BH);
                            byte[] bhProcessedBytes = ProcessDocumentBH(bhDocBytes, ChemicalDetails, SampleUID, ReportIDGet, s, dt1);

                            // Define BH file paths
                            string docxFileName_BH = $"TestReport-{SampleUID}_BH.docx";
                            string pdfFileName_BH = $"TestReport-{SampleUID}_BH.pdf";

                            string outputDocxPath_BH = Path.Combine(pathout, docxFileName_BH);
                            string outputPdfPath_BH = Path.Combine(pathout, pdfFileName_BH);

                            // ========== SAVE BH WORD DOCUMENT ==========
                            File.WriteAllBytes(outputDocxPath_BH, bhProcessedBytes);
                            Debug.WriteLine($"BH Word saved: {outputDocxPath_BH}");

                            // ========== CONVERT BH TO PDF AND SAVE ==========
                            try
                            {
                                byte[] pdfBytes_BH = ConvertDocxToPdf_Syncfusion(bhProcessedBytes);
                                File.WriteAllBytes(outputPdfPath_BH, pdfBytes_BH);
                                Debug.WriteLine($"BH PDF saved: {outputPdfPath_BH}");

                                // Verify PDF was created successfully
                                if (File.Exists(outputPdfPath_BH) && new FileInfo(outputPdfPath_BH).Length > 0)
                                {
                                    // Add PDF path for download
                                    pdfPaths.Add($"/TestReport/{pdfFileName_BH}");
                                }
                            }
                            catch (Exception pdfEx)
                            {
                                Debug.WriteLine($"PDF conversion error for BH document: {pdfEx.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error processing BH document: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"No BH template available. templatePath_BH: {templatePath_BH}");
                    }

                    // ========== OPEN PDF FILES IN NEW TABS ==========
                    if (pdfPaths.Count > 0)
                    {
                        // Generate script to open all PDFs in new tabs
                        string script = GenerateOpenInNewTabScript(pdfPaths);
                        ClientScript.RegisterStartupScript(this.GetType(), "OpenPDFs", script);                        
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                            "<script> alert('PDF files could not be generated.'); </script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        // ========== SINGLE GetTemplatePath_BH METHOD ==========
        private string GetTemplatePath_BH(string template, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(template))
                {
                    Debug.WriteLine("GetTemplatePath_BH: Template is null or empty");
                    return "";
                }

                Debug.WriteLine($"GetTemplatePath_BH: Looking for BH template for: {template}");
                Debug.WriteLine($"GetTemplatePath_BH: Base path: {path}");

                // Get base name without extension
                string baseName = Path.GetFileNameWithoutExtension(template);
                string extension = Path.GetExtension(template);

                Debug.WriteLine($"GetTemplatePath_BH: BaseName: {baseName}, Extension: {extension}");

                // Try different naming patterns
                string[] possibleNames = new[]
                {
                    $"{baseName}_BH{extension}",           // Template_BH.docx
                    $"{baseName}BH{extension}",            // TemplateBH.docx
                    $"{baseName} - BH{extension}",         // Template - BH.docx
                    $"{baseName} BH{extension}",           // Template BH.docx
                    $"{baseName}_BH",                      // Template_BH (no extension)
                    $"{baseName}BH",                      // TemplateBH (no extension)
                };

                foreach (string name in possibleNames)
                {
                    string fullPath = Path.Combine(path, name);
                    Debug.WriteLine($"GetTemplatePath_BH: Checking: {fullPath}");
                    if (File.Exists(fullPath))
                    {
                        Debug.WriteLine($"GetTemplatePath_BH: FOUND BH template: {fullPath}");
                        return fullPath;
                    }
                }

                // Try specific mappings
                if (template == "Chemical_TemplateGeneralK3L.docx")
                {
                    string bhPath = Path.Combine(path, "Chemical_TemplateGeneralK3L_BH.docx");
                    Debug.WriteLine($"GetTemplatePath_BH: Trying specific mapping for K3L: {bhPath}");
                    if (File.Exists(bhPath))
                        return bhPath;
                }
                else if (template == "Chemical_TemplateGeneral.docx")
                {
                    string bhPath = Path.Combine(path, "Chemical_TemplateGeneral_BH.docx");
                    Debug.WriteLine($"GetTemplatePath_BH: Trying specific mapping for General: {bhPath}");
                    if (File.Exists(bhPath))
                        return bhPath;
                }
                else if (template == "Chemical_TemplateAO1.docx")
                {
                    string bhPath = Path.Combine(path, "Chemical_TemplateAO1_BH.docx");
                    Debug.WriteLine($"GetTemplatePath_BH: Trying specific mapping for AO1: {bhPath}");
                    if (File.Exists(bhPath))
                        return bhPath;
                }

                Debug.WriteLine($"GetTemplatePath_BH: NO BH template found for: {template}");
                return "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetTemplatePath_BH ERROR: {ex.Message}");
                return "";
            }
        }

        // ========== GENERATE SCRIPT TO OPEN PDFS IN NEW TABS ==========
        private string GenerateOpenInNewTabScript(List<string> pdfPaths)
        {
            StringBuilder script = new StringBuilder();
            script.Append("<script>");

            if (pdfPaths.Count == 1)
            {
                script.AppendFormat("window.open('{0}', '_blank');", ResolveUrl(pdfPaths[0]));

                // Redirect after 1 second
                script.Append("setTimeout(function(){ window.location.href='Default.aspx'; }, 1000);");
            }
            else
            {
                for (int i = 0; i < pdfPaths.Count; i++)
                {
                    int delay = i * 800; // 800ms between opening each tab
                    script.AppendFormat(
                        "setTimeout(function(){{ window.open('{0}', '_blank'); }}, {1});",
                        ResolveUrl(pdfPaths[i]), delay);
                }

                // Redirect after the last PDF is opened
                int redirectDelay = (pdfPaths.Count * 800) + 1000;

                script.AppendFormat(
                    "setTimeout(function(){{ window.location.href='{0}'; }}, {1});",
                    ResolveUrl("~/Default.aspx"), redirectDelay);
            }

            script.Append("</script>");

            return script.ToString();
        }

        // ========== SYNC FUSION DOCX TO PDF CONVERTER ==========
        private byte[] ConvertDocxToPdf_Syncfusion(byte[] docxBytes)
        {
            string tempPdfPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pdf");

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
                throw new Exception("Syncfusion PDF conversion error: " + ex.Message, ex);
            }
            finally
            {
                if (File.Exists(tempPdfPath))
                    File.Delete(tempPdfPath);
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
        private void ProcessTableMain(WordprocessingDocument doc, string tableType, string query, string sampleId)
        {
            switch (tableType)
            {
                case "10":
                    /* DataTable dt10 = _cm.getReportQuery(1, query + sampleId); //CreateTable10(doc, "{ResultSummaryTable}", dt10); */
                    DataTable dt10 = _cm.GetResultSummaryConclusion(sampleId);
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
                                Debug.WriteLine($"Sample image missing or unreadable, skipping: {fullImagePath}");
                            }
                        }
                    }
                    InsertImagesIntoDocument(doc, "{images}", imageData);
                    break;

                case "14":
                    string Query = @"SELECT Equipment, Type, Calibration_Date FROM Calibration_Equipment where DepartmentId = 4";
                    DataTable dt14 = _cm.getReportQuery_BH(Query);
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
                    DataTable dt10 = _cm.GetResultSummaryConclusion_BH(sampleId);
                    CreateTable10BH(doc, "{ResultSummaryTable}", dt10);
                    break;

                case "11":
                    string q11 = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                   c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                   INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                   INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                   INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                   WHERE t.sampleID= '" + sampleId + "' ";
                    DataTable dt11 = _cm.getReportQuery_BH(q11);
                    CreateTable11_BH(doc, "{ResultTable1}", dt11);
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
                                Debug.WriteLine($"Sample image missing or unreadable, skipping: {fullImagePath}");
                            }
                        }
                    }
                    InsertImagesIntoDocument(doc, "{images}", imageData);
                    break;

                case "14":
                    /* string q14 = @"select Equipment ,Type,Calibration_Date as[Calibration Due Date] from TestResultDetails t 
                                   INNER JOIN Calibration_Equipment c  on c.sampleid = t.SampleId "; */
                    string q14 = @"SELECT Equipment, Type, Calibration_Date FROM Calibration_Equipment where DepartmentId = 4 ";
                    DataTable dt14 = _cm.getReportQuery_BH(q14);
                    CreateTable14BH(doc, "{Testingandmeasuringequipment}", dt14);
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
                    CreateTable16_BH(doc, "{ResultTable2}", dt16);
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
                Debug.WriteLine("Error generating QR: " + ex.Message);
            }
        }

        protected void GenerateQR_BH(string SampleID)
        {
            try
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string QRCodeFilePath = baseUrl + "/TestReport/TestReport-" + SampleID + "_BH.pdf";
                string QRCodeFileName = SampleID + "_BH.png";

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

        private void CreateTable14(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            CreateGenericTable(doc, placeholder, dt);
        }

        private void CreateTable14BH(WordprocessingDocument doc, string placeholder, DataTable dt)
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

        /*===========================|Overall Conclusion|=========================*/

        private Table BuildTable17(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return null;

            int colCount = dt.Columns.Count;
            if (colCount == 0) return null;

            var table = new Table();

            // Table properties
            var tableProps = new TableProperties();

            // Table borders
            var borders = new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
            );
            tableProps.AppendChild(borders);

            // Use full width
            int tableWidth = USABLE_WIDTH_TWIPS;
            tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });
            table.AppendChild(tableProps);

            // ========== CALCULATE COLUMN WIDTHS WITH SPECIFIC PERCENTAGES ==========
            // Column widths: 8%, 55%, 22%, 15%
            List<int> colWidths = CalculateTable17ColumnWidths(tableWidth, colCount);

            // Table grid
            var grid = new TableGrid();
            foreach (int width in colWidths)
            {
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // ========== HEADER ROW ==========
            var headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            for (int i = 0; i < colCount; i++)
            {
                var cell = new TableCell();
                var cellProps = new TableCellProperties();

                cellProps.AppendChild(new TableCellWidth
                {
                    Width = colWidths[i].ToString(),
                    Type = TableWidthUnitValues.Dxa
                });

                // Gray background for header
                cellProps.AppendChild(new Shading
                {
                    Color = "D9D9D9",
                    Fill = "D9D9D9",
                    Val = new EnumValue<ShadingPatternValues>(ShadingPatternValues.Clear)
                });

                cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                cell.AppendChild(cellProps);

                var para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center },
                    SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                };

                var run = new Run();
                var runProps = new RunProperties();
                runProps.AppendChild(new Bold());
                runProps.AppendChild(new FontSize { Val = "22" }); // Font size 11 (22 half-points)
                run.AppendChild(runProps);
                run.AppendChild(new Text(dt.Columns[i].ColumnName) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // ========== DATA ROWS ==========
            foreach (DataRow dataRow in dt.Rows)
            {
                var row = new TableRow();
                row.AppendChild(new TableRowProperties(
                    new TableRowHeight { Val = 350, HeightType = HeightRuleValues.AtLeast }
                ));

                for (int i = 0; i < colCount; i++)
                {
                    var cell = new TableCell();
                    var cellProps = new TableCellProperties();

                    cellProps.AppendChild(new TableCellWidth
                    {
                        Width = colWidths[i].ToString(),
                        Type = TableWidthUnitValues.Dxa
                    });

                    cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                    cell.AppendChild(cellProps);

                    string cellText = dataRow[i]?.ToString() ?? string.Empty;

                    var para = new Paragraph();

                    // ========== FIX: Left align the "Test Parameters" column (index 1) ==========
                    // Column 0 = No., Column 1 = Test Parameters, Column 2 = Test Method, Column 3 = Results
                    JustificationValues justification;
                    if (i == 1) // Test Parameters column - Left align
                    {
                        justification = JustificationValues.Left;
                    }
                    else // All other columns - Center align
                    {
                        justification = JustificationValues.Center;
                    }

                    para.ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification { Val = justification },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                    };

                    var run = new Run();
                    var runProps = new RunProperties();
                    runProps.AppendChild(new FontSize { Val = "22" }); // Font size 11

                    // Check if this is the Results column (last column)
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
                    run.AppendChild(new Text(cellText) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    row.AppendChild(cell);
                }
                table.AppendChild(row);
            }

            return table;
        }

        private List<int> CalculateTable17ColumnWidths(int tableWidth, int colCount)
        {
            List<int> colWidths = new List<int>();

            // Define percentage distribution based on column count // Default: 8%, 50%, 30%, 12% for 4 columns //
            double[] percentages;

            switch (colCount)
            {
                case 1:
                    percentages = new double[] { 100 };
                    break;
                case 2:
                    percentages = new double[] { 30, 70 };
                    break;
                case 3:
                    percentages = new double[] { 15, 55, 30 };
                    break;
                case 4:
                    percentages = new double[] { 10, 40, 30, 20 }; 
                    break;
                default:
                    // For 5+ columns, distribute evenly with first column smaller
                    percentages = new double[colCount];
                    double remaining = 100;
                    for (int i = 0; i < colCount; i++)
                    {
                        if (i == 0)
                            percentages[i] = 8;
                        else if (i == colCount - 1)
                            percentages[i] = 15;
                        else
                            percentages[i] = (remaining - 8 - 15) / (colCount - 2);
                    }
                    break;
            }

            // Calculate widths based on percentages
            int totalWidth = 0;
            for (int i = 0; i < colCount && i < percentages.Length; i++)
            {
                int width = (int)(tableWidth * (percentages[i] / 100.0));
                colWidths.Add(width);
                totalWidth += width;
            }

            // Adjust for any rounding issues
            if (totalWidth != tableWidth)
            {
                int diff = tableWidth - totalWidth;
                colWidths[colWidths.Count - 1] += diff;
            }

            return colWidths;
        }

        private void CreateTable17(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ForceReplaceInDocument(doc, "{Overall Conclusion}", "No data available");
                ForceReplaceInDocument(doc, "{Requirement}", "No requirements available");
                return;
            }

            var requirements = dt.AsEnumerable().Select(row => row["Requirement"]?.ToString()).Where(req => !string.IsNullOrWhiteSpace(req)).Distinct().ToList();
            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            ForceReplaceInDocument(doc, "{Requirement}", combinedRequirements);

            DataTable modifiedDt = dt.Copy();
            if (modifiedDt.Columns.Contains("Requirement"))
                modifiedDt.Columns.Remove("Requirement");

            // Find the placeholder
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            // Remove placeholder
            ReplaceInParagraph(targetParagraph, placeholder, "");

            if (modifiedDt == null || modifiedDt.Rows.Count == 0) return;

            // Build the table with specific column widths and font size 11
            var table = BuildTable17(modifiedDt);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateTable17BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ForceReplaceInDocument(doc, "{Overall Conclusion}", "Data tidak tersedia");
                ForceReplaceInDocument(doc, "{Requirement}", "Tidak ada persyaratan");
                return;
            }

            var requirements = dt.AsEnumerable().Select(row => row["Requirement"]?.ToString()).Where(req => !string.IsNullOrWhiteSpace(req)).Distinct().ToList();
            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            ForceReplaceInDocument(doc, "{Requirement}", combinedRequirements);

            DataTable modifiedDt = dt.Copy();
            if (modifiedDt.Columns.Contains("Requirement"))
                modifiedDt.Columns.Remove("Requirement");

            // Find the placeholder
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, placeholder);
            if (targetParagraph == null) return;

            // Remove placeholder
            ReplaceInParagraph(targetParagraph, placeholder, "");

            if (modifiedDt == null || modifiedDt.Rows.Count == 0) return;

            // Build the table with specific column widths and font size 11
            var table = BuildTable17(modifiedDt);
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

       
        // ========== SAMPLE DESCRIPTION TABLES ==========//
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
                Debug.WriteLine($"Error creating Sample Description table: {ex.Message}");
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
                Debug.WriteLine($"Error creating BH Sample Description table: {ex.Message}");
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

        // ========== TABLE 18 AND 19 METHODS ========== //
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
                Debug.WriteLine($"Error in CreateTable18: {ex.Message}");
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
                Debug.WriteLine($"Error in CreateTable18BH: {ex.Message}");
            }
        }

        private void CreateTable19(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            //lastUsedNumber = 0;
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

                    // Use the same approach as CreateTable18
                    int resultSubCols = allColorCodes.Count;
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
                    foreach (var code in allColorCodes)
                        headerRow.AppendChild(MakeHeaderCellA4($"Result\n(mg/kg)\n({code})", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("Requirement\n(mg/kg)", hasManyColumns));
                    table.AppendChild(headerRow);

                    // Data rows
                    foreach (DataRow dr in modifiedDt.Rows)
                    {
                        var dataRow = new TableRow();
                        dataRow.AppendChild(MakeDataCellA4(dr[0].ToString(), leftAlign: true, dataFontSize));
                        dataRow.AppendChild(MakeDataCellA4(dr[1].ToString(), fontSize: dataFontSize));

                        for (int i = 0; i < allColorCodes.Count; i++)
                        {
                            string colName = $"Result (mg/kg) ({allColorCodes[i]})";
                            string value = modifiedDt.Columns.Contains(colName) ? dr[colName].ToString() : "N.D";
                            dataRow.AppendChild(MakeDataCellA4(value, fontSize: dataFontSize));
                        }

                        int reqColIndex = 2 + allColorCodes.Count;
                        string reqValue = modifiedDt.Columns.Count > reqColIndex ? dr[reqColIndex].ToString() : "";
                        dataRow.AppendChild(MakeDataCellA4(reqValue, fontSize: dataFontSize));

                        table.AppendChild(dataRow);
                    }

                    // Remark row
                    var remarkRow = new TableRow();
                    remarkRow.AppendChild(MakeRemarkCellA4("Remarks", null, true, dataFontSize));
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));

                    for (int i = 0; i < resultSubCols; i++)
                    {
                        bool fail = i < colorCodeHasDetection.Length && colorCodeHasDetection[i];
                        remarkRow.AppendChild(MakeRemarkCellA4(
                            fail ? "FAIL" : "PASS",
                            fail ? "FF0000" : "008000",
                            true, dataFontSize));
                    }
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));
                    table.AppendChild(remarkRow);

                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;

                    // MDL line
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
                Debug.WriteLine($"Error in CreateTable19: {ex.Message}");
            }
        }

        private void CreateTable19BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            //lastUsedNumber = 0;
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

                    // Use the same approach as CreateTable18BH
                    int resultSubCols = allColorCodes.Count;
                    bool hasManyColumns = resultSubCols > 4;

                    var colWidths = CalculateColumnWidths(resultSubCols, true);
                    string dataFontSize = hasManyColumns ? "20" : "22";

                    var table = CreateStyledTableA4(colWidths, hasManyColumns);

                    // Header row
                    var headerRow = new TableRow();
                    headerRow.AppendChild(new TableRowProperties(
                        new TableRowHeight() { Val = hasManyColumns ? 600U : 800U, HeightType = HeightRuleValues.Exact }
                    ));

                    headerRow.AppendChild(MakeHeaderCellA4("Senyawa", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("CAS\nNumber", hasManyColumns));
                    foreach (var code in allColorCodes)
                        headerRow.AppendChild(MakeHeaderCellA4($"Hasil Uji\n(mg/kg)\n({code})", hasManyColumns));
                    headerRow.AppendChild(MakeHeaderCellA4("Persyaratan\n(mg/kg)", hasManyColumns));
                    table.AppendChild(headerRow);

                    // Data rows
                    foreach (DataRow dr in modifiedDt.Rows)
                    {
                        var dataRow = new TableRow();
                        dataRow.AppendChild(MakeDataCellA4(dr[0].ToString(), leftAlign: true, dataFontSize));
                        dataRow.AppendChild(MakeDataCellA4(dr[1].ToString(), fontSize: dataFontSize));

                        for (int i = 0; i < allColorCodes.Count; i++)
                        {
                            string colName = $"Hasil (mg/kg) ({allColorCodes[i]})";
                            string value = modifiedDt.Columns.Contains(colName) ? dr[colName].ToString() : "N.D";
                            dataRow.AppendChild(MakeDataCellA4(value, fontSize: dataFontSize));
                        }

                        int reqColIndex = 2 + allColorCodes.Count;
                        string reqValue = modifiedDt.Columns.Count > reqColIndex ? dr[reqColIndex].ToString() : "";
                        dataRow.AppendChild(MakeDataCellA4(reqValue, fontSize: dataFontSize));

                        table.AppendChild(dataRow);
                    }

                    // Remark row
                    var remarkRow = new TableRow();
                    remarkRow.AppendChild(MakeRemarkCellA4("Keterangan", null, true, dataFontSize));
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));

                    for (int i = 0; i < resultSubCols; i++)
                    {
                        bool fail = i < colorCodeHasDetection.Length && colorCodeHasDetection[i];
                        remarkRow.AppendChild(MakeRemarkCellA4(
                            fail ? "TIDAK LULUS" : "LULUS",
                            fail ? "FF0000" : "008000",
                            true, dataFontSize));
                    }
                    remarkRow.AppendChild(MakeRemarkCellA4("", null, false, dataFontSize));
                    table.AppendChild(remarkRow);

                    currentParagraph.Parent.InsertAfter(table, currentParagraph);
                    currentParagraph = table.Descendants<Paragraph>().FirstOrDefault() ?? currentParagraph;

                    // MDL line
                    var mdlParts = group
                        .Where(row => !string.IsNullOrWhiteSpace(row["MDL"]?.ToString()))
                        .Select(row => $"{row["Senyawa"]}: {row["MDL"]} mg/kg")
                        .Distinct()
                        .ToList();

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
                Debug.WriteLine($"Error in CreateTable19BH: {ex.Message}");
            }
        }

        // ========== TABLE 18-19 HELPER METHODS ========== //

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

                // Calculate minimum width needed for result columns based on header text
                // Each result header like "Result\n(mg/kg)\n(001+002+003+004)" needs about 2500 twips
                int minResultColWidth = Math.Min(2500, availableForResults / resultColumnsCount);

                // If there are many columns, we still need to ensure minimum readability
                int resultColWidth = resultColumnsCount > 0
                    ? Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, availableForResults / resultColumnsCount))
                    : MIN_COLUMN_WIDTH;

                // For columns with long headers (many color codes), ensure at least 2000 twips
                if (resultColumnsCount <= 4)
                {
                    resultColWidth = Math.Max(resultColWidth, 2000);
                }
                else
                {
                    // For many columns, use the calculated width but ensure minimum
                    resultColWidth = Math.Max(resultColWidth, Math.Min(1600, availableForResults / resultColumnsCount));
                }

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

                int minResultColWidth = Math.Min(2500, availableForResults / resultColumnsCount);
                int resultColWidth = resultColumnsCount > 0
                    ? Math.Max(MIN_COLUMN_WIDTH, Math.Min(MAX_COLUMN_WIDTH, availableForResults / resultColumnsCount))
                    : MIN_COLUMN_WIDTH;

                if (resultColumnsCount <= 4)
                {
                    resultColWidth = Math.Max(resultColWidth, 2000);
                }
                else
                {
                    resultColWidth = Math.Max(resultColWidth, Math.Min(1600, availableForResults / resultColumnsCount));
                }

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

            // Use smaller font when many columns
            string fontSize = hasManyColumns ? "16" : "20";
            r.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = fontSize }));
            r.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
            p.AppendChild(r);
            c.AppendChild(p);
            return c;
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

        // =================================== RESULT TABLE (PASS/FAIL) METHODS ================================================//
        // ========== A01 - INSERT RESULT TABLE ==========
        private void InsertResultTableIntoDocument_A01(WordprocessingDocument doc, string placeholder, string result, bool isBH)
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
                    table = BuildA01ResultTable("PENILAIAN KESELURUHAN", isLulusChecked, isTidakLulusChecked, "LULUS", "TIDAK LULUS");
                }
                else
                {
                    bool isPassChecked = upperResult == "PASS";
                    bool isFailChecked = upperResult == "FAIL";
                    table = BuildA01ResultTable("OVERALL RATING", isPassChecked, isFailChecked, "PASS", "FAIL");
                }

                targetParagraph.Parent.InsertAfter(table, targetParagraph);
                Paragraph breakParagraph = new Paragraph(new Run());
                targetParagraph.Parent.InsertAfter(breakParagraph, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in InsertResultTableIntoDocument_A01: {ex.Message}");
            }
        }

        // ========== A01 - BUILD RESULT TABLE ==========
        private Table BuildA01ResultTable(string headerText, bool isFirstChecked, bool isSecondChecked, string firstLabel, string secondLabel)
        {
            const string BORDER_COLOR = "1f4e79";
            const string HEADER_BG_COLOR = "D3D3D3";
            const string TEXT_COLOR = "000000";
            const uint BORDER_SIZE = 8;
            const uint HEADER_HEIGHT = 400;
            const uint ROW_HEIGHT = 350;

            // ADJUST THESE VALUES
            const string LABEL_WIDTH = "2000";          // Label column width
            const string CHECKBOX_WIDTH = "400";        // Checkbox column width (INCREASED from 180 to 400)
            const string TOTAL_WIDTH = "2400";          // Total width (LABEL_WIDTH + CHECKBOX_WIDTH)

            var table = new Table();

            // Table properties
            var tableProps = new TableProperties(
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = TOTAL_WIDTH, Type = TableWidthUnitValues.Dxa },
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

            // Table grid
            var grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = LABEL_WIDTH });
            grid.AppendChild(new GridColumn() { Width = CHECKBOX_WIDTH });
            table.AppendChild(grid);

            // ========== HEADER ROW ==========
            var headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = HEADER_HEIGHT, HeightType = HeightRuleValues.Exact }
            ));

            var headerCell = new TableCell();
            headerCell.AppendChild(new TableCellProperties(
                new TableCellWidth() { Width = TOTAL_WIDTH, Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = HEADER_BG_COLOR }
            ));
            headerCell.AppendChild(new GridSpan() { Val = 2 });

            var headerPara = new Paragraph();
            headerPara.ParagraphProperties = new ParagraphProperties();
            headerPara.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
            headerPara.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines()
            {
                Before = "120",
                After = "120",
                Line = "240",
                LineRule = LineSpacingRuleValues.AtLeast
            };

            var headerRun = new Run();
            headerRun.RunProperties = new RunProperties();
            headerRun.RunProperties.AppendChild(new Bold());
            headerRun.RunProperties.AppendChild(new FontSize() { Val = "18" });
            headerRun.RunProperties.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = TEXT_COLOR });
            headerRun.AppendChild(new Text(headerText) { Space = SpaceProcessingModeValues.Preserve });

            headerPara.AppendChild(headerRun);
            headerCell.AppendChild(headerPara);
            headerRow.AppendChild(headerCell);
            table.AppendChild(headerRow);

            // ========== FIRST ROW ==========
            table.AppendChild(BuildA01Row(isFirstChecked, firstLabel, LABEL_WIDTH, CHECKBOX_WIDTH));

            // ========== SECOND ROW ==========
            table.AppendChild(BuildA01Row(isSecondChecked, secondLabel, LABEL_WIDTH, CHECKBOX_WIDTH));

            return table;
        }

        // ========== A01 - BUILD SINGLE ROW ==========
        private TableRow BuildA01Row(bool isChecked, string labelText, string labelWidth, string checkboxWidth)
        {
            const string TEXT_COLOR = "000000";
            const uint ROW_HEIGHT = 350;

            var row = new TableRow();
            row.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = ROW_HEIGHT, HeightType = HeightRuleValues.Exact }
            ));

            // ========== LABEL CELL ==========
            var labelCell = new TableCell();
            labelCell.AppendChild(new TableCellProperties(
                new TableCellWidth() { Width = labelWidth, Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            ));

            var labelPara = new Paragraph();
            labelPara.ParagraphProperties = new ParagraphProperties();
            labelPara.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
            labelPara.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines()
            {
                Before = "80",
                After = "80",
                Line = "240",
                LineRule = LineSpacingRuleValues.AtLeast
            };

            var labelRun = new Run();
            labelRun.RunProperties = new RunProperties();
            labelRun.RunProperties.AppendChild(new Bold());
            labelRun.RunProperties.AppendChild(new FontSize() { Val = "20" });
            labelRun.RunProperties.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = TEXT_COLOR });
            labelRun.AppendChild(new Text(labelText) { Space = SpaceProcessingModeValues.Preserve });

            labelPara.AppendChild(labelRun);
            labelCell.AppendChild(labelPara);
            row.AppendChild(labelCell);

            // ========== CHECKBOX CELL ==========
            var boxCell = new TableCell();
            boxCell.AppendChild(new TableCellProperties(
                new TableCellWidth() { Width = checkboxWidth, Type = TableWidthUnitValues.Dxa },
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            ));

            var boxPara = new Paragraph();
            boxPara.ParagraphProperties = new ParagraphProperties();
            boxPara.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
            boxPara.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines()
            {
                Before = "80",
                After = "80",
                Line = "240",
                LineRule = LineSpacingRuleValues.AtLeast
            };

            if (isChecked)
            {
                var xRun = new Run();
                xRun.RunProperties = new RunProperties();
                xRun.RunProperties.AppendChild(new Bold());
                xRun.RunProperties.AppendChild(new FontSize() { Val = "24" });  // Increased X size
                xRun.RunProperties.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = TEXT_COLOR });
                xRun.AppendChild(new Text("X") { Space = SpaceProcessingModeValues.Preserve });
                boxPara.AppendChild(xRun);
            }
            boxCell.AppendChild(boxPara);
            row.AppendChild(boxCell);

            return row;
        }

        // ========== INSERT RESULT TABLE INTO DOCUMENT - FIXED DESIGN K3L REPORT ========== //        
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

        // ========================= CHEMICAL DETAILS PROCESSING ============================================//
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

            AddReplacementIfExists(replacements, "{RequestNo.}", GetSafeValue("RequestNo"));
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
            AddReplacementIfExists(replacements, "{TestSample}", GetSafeDate("TestSample"));
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


            AddReplacementIfExists(replacements, "{Buyer’sName}", GetSafeValue("Buyer'sName"));
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

            AddReplacementIfExists(replacements, "{LabHeadName}", GetSafeValue("LabHeadName"));


            string testReportNo = GetSafeValue("TestReportNo");
            AddReplacementIfExists(replacements, "{TestReportNo.}", testReportNo);
            AddReplacementIfExists(replacements, "{TestReportNo}", testReportNo);

            AddReplacementIfExists(replacements, "{DocNo}", "TSQ-LAB-QAF-087");

            FindAndReplaceMultiple(doc, replacements);

            string imageUrl;
            if (isBH)
            {
                imageUrl = Server.MapPath("~/TestReport/" + SampleUID + "_BH.png");
            }
            else
            {
                imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
            }

            if (File.Exists(imageUrl))
            {
                ReplacePlaceholderWithImage(doc, "{QR_Code}", imageUrl);
            }

            string finalResult = GetSafeValue("FinalTestResult").Trim().ToUpper();
            if (isBH)
            {
                string resultBH = finalResult == "PASS" ? "LULUS" : "TIDAK LULUS";
                InsertResultTableIntoDocument(doc, "{FinalTestResult}", resultBH, isBH: true);
                InsertResultTableIntoDocument_A01(doc, "{FinalTestResult1}", resultBH, isBH: true);
            }
            else
            {
                string resultEN = finalResult == "PASS" ? "PASS" : "FAIL";
                InsertResultTableIntoDocument(doc, "{FinalTestResult}", resultEN, isBH: false);
                InsertResultTableIntoDocument_A01(doc, "{FinalTestResult1}", resultEN, isBH: false);
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

        // ========== ADDITIONAL DATA METHODS ===============================================================//
        public void AdditionalDataAdded(WordprocessingDocument wordDoc, string placeholder)
        {
            try
            {
                string path = Server.MapPath("~/FilesUpload/");
                string sampleID = ViewState["SampleIDGet"].ToString();
                string filePath = path + sampleID + ".xlsx";
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Excel file not found.", filePath);
                }

                List<List<string>> rows = new List<List<string>>();
                Dictionary<(int row, int col), byte[]> imageMap = new Dictionary<(int, int), byte[]>();

                int firstRow, firstCol;

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var usedRange = worksheet.RangeUsed();
                    if (usedRange == null) return;

                    firstRow = usedRange.FirstRow().RowNumber();
                    firstCol = usedRange.FirstColumn().ColumnNumber();
                    int lastCol = usedRange.LastColumn().ColumnNumber();

                    // --- Read text grid (mirrors range.Copy() + wordRange.Paste() text content) ---
                    foreach (var row in usedRange.RowsUsed())
                    {
                        List<string> rowData = new List<string>();
                        foreach (var cell in row.Cells(firstCol, lastCol))
                        {
                            rowData.Add(cell.GetFormattedString() ?? "");
                        }
                        rows.Add(rowData);
                    }

                    // --- Read images, capturing raw bytes while the workbook stream is still open ---
                    foreach (var picture in worksheet.Pictures)
                    {
                        var anchorCell = picture.TopLeftCell;
                        int relRow = anchorCell.Address.RowNumber - firstRow;
                        int relCol = anchorCell.Address.ColumnNumber - firstCol;
                        if (relRow < 0 || relCol < 0) continue;

                        picture.ImageStream.Position = 0;
                        using (var ms = new MemoryStream())
                        {
                            picture.ImageStream.CopyTo(ms);
                            imageMap[(relRow, relCol)] = ms.ToArray();
                        }
                    }
                }

                if (rows.Count == 0) return;
                int columnCount = rows[0].Count;

                Body body = wordDoc.MainDocumentPart.Document.Body;
                Paragraph targetParagraph = FindParagraphWithPlaceholder(body, placeholder);
                if (targetParagraph == null) return;

                var mainPart = wordDoc.MainDocumentPart;
                Table table = BuildExcelTableWithImages(rows, columnCount, imageMap, mainPart);

                targetParagraph.InsertBeforeSelf(table);
                targetParagraph.Remove();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        public void AdditionalDataAdded_BH(WordprocessingDocument wordDoc_BH, string placeholder)
        {
            try
            {
                string path = Server.MapPath("~/FilesUpload/");
                string sampleID = ViewState["SampleIDGet"].ToString();
                string filePath = path + sampleID + ".xlsx";
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Excel file not found.", filePath);
                }

                List<List<string>> rows = new List<List<string>>();
                Dictionary<(int row, int col), byte[]> imageMap = new Dictionary<(int, int), byte[]>();

                int firstRow, firstCol;

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var usedRange = worksheet.RangeUsed();
                    if (usedRange == null) return;

                    firstRow = usedRange.FirstRow().RowNumber();
                    firstCol = usedRange.FirstColumn().ColumnNumber();
                    int lastCol = usedRange.LastColumn().ColumnNumber();

                    foreach (var row in usedRange.RowsUsed())
                    {
                        List<string> rowData = new List<string>();
                        foreach (var cell in row.Cells(firstCol, lastCol))
                        {
                            rowData.Add(cell.GetFormattedString() ?? "");
                        }
                        rows.Add(rowData);
                    }

                    foreach (var picture in worksheet.Pictures)
                    {
                        var anchorCell = picture.TopLeftCell;
                        int relRow = anchorCell.Address.RowNumber - firstRow;
                        int relCol = anchorCell.Address.ColumnNumber - firstCol;
                        if (relRow < 0 || relCol < 0) continue;

                        picture.ImageStream.Position = 0;
                        using (var ms = new MemoryStream())
                        {
                            picture.ImageStream.CopyTo(ms);
                            imageMap[(relRow, relCol)] = ms.ToArray();
                        }
                    }
                }

                if (rows.Count == 0) return;
                int columnCount = rows[0].Count;

                Body body = wordDoc_BH.MainDocumentPart.Document.Body;
                Paragraph targetParagraph = FindParagraphWithPlaceholder(body, placeholder);
                if (targetParagraph == null) return;

                var mainPart = wordDoc_BH.MainDocumentPart;
                Table table = BuildExcelTableWithImages(rows, columnCount, imageMap, mainPart);

                targetParagraph.InsertBeforeSelf(table);
                targetParagraph.Remove();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        // --- Shared helper: find paragraph by reconstructing text across split runs ---
        private Paragraph FindParagraphWithPlaceholder(Body body, string placeholder)
        {
            foreach (Paragraph para in body.Descendants<Paragraph>())
            {
                string fullText = string.Concat(para.Descendants<WordText>().Select(t => t.Text));
                if (fullText.Contains(placeholder))
                {
                    return para;
                }
            }
            return null;
        }

        // --- Builds the table: page-width-safe columns, images placed at their real anchor cell ---

        private Table BuildExcelTableWithImages(
    List<List<string>> rows,
    int columnCount,
    Dictionary<(int row, int col), byte[]> imageMap,
    MainDocumentPart mainPart)
        {
            // Define column width percentages
            List<double> columnPercentages = new List<double>();

            if (columnCount >= 6)
            {
                // Based on your Excel structure: 
                // No. (1), Product Name (2), Article No. (3), Color (4), Material (5), Brand (6), Photo (7) //
                columnPercentages.Add(5);   // Column 1: No.
                columnPercentages.Add(15);  // Column 2: Product Name
                columnPercentages.Add(15);  // Column 3: Article No.
                columnPercentages.Add(10);   // Column 4: Color
                columnPercentages.Add(30);  // Column 5: Material
                columnPercentages.Add(10);  // Column 6: Brand

                // Distribute remaining columns (7, 8, etc.)
                double remainingPercent = 100 - 5 - 15 - 15 - 10 - 30 - 10; // = 25%
                int remainingColumns = columnCount - 6;
                if (remainingColumns > 0)
                {
                    double eachRemaining = remainingPercent / remainingColumns;
                    for (int i = 6; i < columnCount; i++)
                    {
                        columnPercentages.Add(eachRemaining);
                    }
                }
            }
            else
            {
                // Fallback: distribute evenly
                double eachPercent = 100.0 / columnCount;
                for (int i = 0; i < columnCount; i++)
                {
                    columnPercentages.Add(eachPercent);
                }
            }

            // Convert percentages to twips
            List<int> colWidthsTwips = new List<int>();
            foreach (double percentage in columnPercentages)
            {
                int widthTwips = (int)((USABLE_WIDTH_TWIPS * percentage) / 100);
                colWidthsTwips.Add(widthTwips);
            }

            Table table = new Table();

            // Table properties
            TableProperties tblProps = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                ),
                new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            table.AppendChild(tblProps);

            // Table grid (defines column widths)
            TableGrid grid = new TableGrid();
            foreach (int width in colWidthsTwips)
            {
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // Build rows
            for (int r = 0; r < rows.Count; r++)
            {
                TableRow tr = new TableRow();
                List<string> rowData = rows[r];

                for (int c = 0; c < columnCount; c++)
                {
                    TableCell tc = new TableCell();

                    // Cell properties with width
                    TableCellProperties tcProps = new TableCellProperties();
                    tcProps.AppendChild(new TableCellWidth
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = colWidthsTwips[c].ToString()
                    });
                    tcProps.AppendChild(new TableCellVerticalAlignment
                    {
                        Val = TableVerticalAlignmentValues.Center
                    });
                    tc.AppendChild(tcProps);

                    // Get cell text (handle if row has fewer columns)
                    string cellText = c < rowData.Count ? rowData[c] : "";

                    // Add text paragraph - simplified version
                    Paragraph textPara = new Paragraph(
                        new ParagraphProperties(
                            new Justification { Val = JustificationValues.Center }
                        ),
                        new Run(
                            new RunProperties(
                                new FontSize { Val = "20" } // 10pt
                            ),
                            new Text(cellText) { Space = SpaceProcessingModeValues.Preserve }
                        )
                    );
                    tc.AppendChild(textPara);

                    // Add image if exists at this cell
                    if (imageMap.TryGetValue((r, c), out var rawBytes))
                    {
                        byte[] finalBytes = rawBytes;
                        PartTypeInfo partType = GetImagePartTypeFromBytes(rawBytes);

                        if (partType.Equals(ImagePartType.Emf) || partType.Equals(ImagePartType.Wmf))
                        {
                            finalBytes = ConvertToPngIfMetafile(rawBytes);
                            partType = ImagePartType.Png;
                        }

                        var imagePart = mainPart.AddImagePart(partType);
                        using (var imgStream = new MemoryStream(finalBytes))
                        {
                            imagePart.FeedData(imgStream);
                        }

                        // Calculate image size (leave padding for cell)
                        long maxWidthEmu = (long)((colWidthsTwips[c] - 80) * 635); // 40 twips padding each side
                        if (maxWidthEmu < 100000L) maxWidthEmu = 100000L;
                        long maxHeightEmu = (long)(maxWidthEmu * 0.75);

                        GetScaledEmuSizeFromBytes(finalBytes, maxWidthEmu, maxHeightEmu, out long picWidth, out long picHeight);

                        var drawing = CreateImageElement(
                            mainPart.GetIdOfPart(imagePart),
                            "Excel Image",
                            picWidth,
                            picHeight
                        );

                        // Image in its own paragraph
                        Paragraph imgPara = new Paragraph(
                            new ParagraphProperties(
                                new Justification { Val = JustificationValues.Center }
                            ),
                            new Run(drawing)
                        );
                        tc.AppendChild(imgPara);
                    }

                    tr.AppendChild(tc);
                }

                table.AppendChild(tr);
            }

            return table;
        }


        private Table BuildExcelTableWithImages_old(
            List<List<string>> rows,
            int columnCount,
            Dictionary<(int row, int col), byte[]> imageMap,
            MainDocumentPart mainPart)
        {
            // Distribute USABLE_WIDTH_TWIPS evenly across all columns so the table never exceeds the page
            int baseColWidth = USABLE_WIDTH_TWIPS / columnCount;
            int remainder = USABLE_WIDTH_TWIPS - (baseColWidth * columnCount);

            List<int> colWidthsTwips = new List<int>();
            for (int i = 0; i < columnCount; i++)
            {
                colWidthsTwips.Add(i == columnCount - 1 ? baseColWidth + remainder : baseColWidth);
            }

            Table table = new Table();

            TableProperties tblProps = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout { Type = TableLayoutValues.Fixed },
                new TableWidth { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            table.AppendChild(tblProps);

            TableGrid grid = new TableGrid();
            foreach (int w in colWidthsTwips)
            {
                grid.AppendChild(new GridColumn { Width = w.ToString() });
            }
            table.AppendChild(grid);

            for (int r = 0; r < rows.Count; r++)
            {
                List<string> rowData = rows[r];
                TableRow tr = new TableRow();

                for (int c = 0; c < rowData.Count; c++)
                {
                    int widthTwips = colWidthsTwips[c];

                    TableCell tc = new TableCell();
                    TableCellProperties tcProps = new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = widthTwips.ToString() },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    );
                    tc.AppendChild(tcProps);

                    // Text paragraph (equivalent to the pasted cell text; font size 8 like Interop's Font.Size = 8)
                    Paragraph textPara = new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(
                            new RunProperties(new FontSize { Val = "22" }), // 16 half-points = 8pt, matches Interop
                            new WordText(rowData[c]) { Space = SpaceProcessingModeValues.Preserve }
                        )
                    );
                    tc.AppendChild(textPara);

                    // If an image is anchored at this exact row/col, embed it sized to fit this cell
                    if (imageMap.TryGetValue((r, c), out var rawBytes))
                    {
                        byte[] finalBytes = rawBytes;
                        PartTypeInfo partType = GetImagePartTypeFromBytes(rawBytes);

                        // Word/OpenXML SDK renders EMF/WMF unreliably when embedded this way - rasterize to PNG
                        if (partType.Equals(ImagePartType.Emf) || partType.Equals(ImagePartType.Wmf))
                        {
                            finalBytes = ConvertToPngIfMetafile(rawBytes);
                            partType = ImagePartType.Png;
                        }

                        var imagePart = mainPart.AddImagePart(partType);
                        using (var imgStream = new MemoryStream(finalBytes))
                        {
                            imagePart.FeedData(imgStream);
                        }

                        // Convert cell width (twips) to EMU, leave a small safety margin for cell padding
                        long maxWidthEmu = (long)((widthTwips - 40) * 635);
                        if (maxWidthEmu < 100000L) maxWidthEmu = 100000L;
                        long maxHeightEmu = (long)(maxWidthEmu * 0.75);

                        GetScaledEmuSizeFromBytes(finalBytes, maxWidthEmu, maxHeightEmu, out long picWidth, out long picHeight);

                        var drawing = CreateImageElement(mainPart.GetIdOfPart(imagePart), "Excel Image", picWidth, picHeight);

                        Paragraph imgPara = new Paragraph(
                            new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                            new Run(drawing)
                        );
                        tc.AppendChild(imgPara);
                    }

                    tr.AppendChild(tc);
                }

                table.AppendChild(tr);
            }

            return table;
        }

        // --- Detects real image format from byte signature (don't trust extension/ClosedXML's Format string) ---
        private PartTypeInfo GetImagePartTypeFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length < 8)
                return ImagePartType.Png;

            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return ImagePartType.Png;

            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 && imageBytes[2] == 0xFF)
                return ImagePartType.Jpeg;

            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return ImagePartType.Bmp;

            if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46 && imageBytes[3] == 0x38)
                return ImagePartType.Gif;

            if ((imageBytes[0] == 0x49 && imageBytes[1] == 0x49 && imageBytes[2] == 0x2A && imageBytes[3] == 0x00) ||
                (imageBytes[0] == 0x4D && imageBytes[1] == 0x4D && imageBytes[2] == 0x00 && imageBytes[3] == 0x2A))
                return ImagePartType.Tiff;

            if (imageBytes[0] == 0x01 && imageBytes[1] == 0x00 && imageBytes[2] == 0x00 && imageBytes[3] == 0x00)
                return ImagePartType.Emf;

            if ((imageBytes[0] == 0xD7 && imageBytes[1] == 0xCD && imageBytes[2] == 0xC6 && imageBytes[3] == 0x9A) ||
                (imageBytes[0] == 0x01 && imageBytes[1] == 0x00 && imageBytes[2] == 0x09 && imageBytes[3] == 0x00))
                return ImagePartType.Wmf;

            Debug.WriteLine("WARNING: Unrecognized image byte signature, defaulting to PNG.");
            return ImagePartType.Png;
        }

        // --- Rasterizes EMF/WMF metafiles to PNG bytes so Word renders them reliably ---
        private byte[] ConvertToPngIfMetafile(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            using (var img = System.Drawing.Image.FromStream(ms))
            using (var bmp = new Bitmap(img.Width, img.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                using (var outMs = new MemoryStream())
                {
                    bmp.Save(outMs, System.Drawing.Imaging.ImageFormat.Png);
                    return outMs.ToArray();
                }
            }
        }

        // --- Scales an in-memory image to fit within given max EMU bounds, preserving aspect ratio ---
        private void GetScaledEmuSizeFromBytes(byte[] imageBytes, long maxWidthEmu, long maxHeightEmu, out long width, out long height)
        {
            try
            {
                using (var ms = new MemoryStream(imageBytes))
                using (var img = System.Drawing.Image.FromStream(ms))
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
                }
            }
            catch
            {
                width = maxWidthEmu;
                height = maxHeightEmu;
            }
        }

        /* ==================================================================================================== */

        private Table CreateFormattedTable(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return null;

            int colCount = dt.Columns.Count;
            if (colCount == 0) return null;

            Table table = new Table();

            // Table properties
            TableProperties tableProps = new TableProperties();

            // Table borders
            TableBorders borders = new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
            );

            tableProps.AppendChild(borders);

            // Use DXA (twips) for table width - more reliable than percentage
            int tableWidth = 9500; // Standard table width in twips
            tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });

            // Fixed layout for better control
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });

            table.AppendChild(tableProps);

            // Calculate column widths - ensure they sum to table width
            int baseColWidth = tableWidth / colCount;
            int remainder = tableWidth - (baseColWidth * colCount);
            List<int> colWidths = new List<int>();
            for (int i = 0; i < colCount; i++)
            {
                colWidths.Add(i == colCount - 1 ? baseColWidth + remainder : baseColWidth);
            }

            // Table grid
            TableGrid grid = new TableGrid();
            foreach (int width in colWidths)
            {
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // Create header row
            TableRow headerRow = new TableRow();
            TableRowProperties headerRowProps = new TableRowProperties();
            headerRowProps.AppendChild(new TableRowHeight { Val = 300, HeightType = HeightRuleValues.AtLeast });
            headerRowProps.AppendChild(new CantSplit { Val = OnOffOnlyValues.On });
            headerRow.AppendChild(headerRowProps);

            for (int i = 0; i < colCount; i++)
            {
                TableCell cell = new TableCell();
                TableCellProperties cellProps = new TableCellProperties();

                // Cell width in DXA
                cellProps.AppendChild(new TableCellWidth
                {
                    Width = colWidths[i].ToString(),
                    Type = TableWidthUnitValues.Dxa
                });

                // Gray background for header
                cellProps.AppendChild(new Shading
                {
                    Color = "D9D9D9",
                    Fill = "D9D9D9",
                    Val = new EnumValue<ShadingPatternValues>(ShadingPatternValues.Clear)
                });

                // Cell margins
                cellProps.AppendChild(new TableCellMargin
                {
                    TopMargin = new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                    BottomMargin = new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                    LeftMargin = new LeftMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin { Width = "80", Type = TableWidthUnitValues.Dxa }
                });

                cell.AppendChild(cellProps);

                Paragraph p = new Paragraph();
                p.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center },
                    SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                };

                Run run = new Run();
                RunProperties runProps = new RunProperties();
                runProps.AppendChild(new Bold());
                runProps.AppendChild(new FontSize { Val = "22" });
                run.AppendChild(runProps);
                run.AppendChild(new Text(dt.Columns[i].ColumnName) { Space = SpaceProcessingModeValues.Preserve });
                p.AppendChild(run);
                cell.AppendChild(p);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Create data rows
            foreach (DataRow dataRow in dt.Rows)
            {
                TableRow row = new TableRow();
                TableRowProperties rowProps = new TableRowProperties();
                rowProps.AppendChild(new TableRowHeight { Val = 240, HeightType = HeightRuleValues.AtLeast });
                row.AppendChild(rowProps);

                for (int i = 0; i < colCount; i++)
                {
                    TableCell cell = new TableCell();
                    TableCellProperties cellProps = new TableCellProperties();

                    cellProps.AppendChild(new TableCellWidth
                    {
                        Width = colWidths[i].ToString(),
                        Type = TableWidthUnitValues.Dxa
                    });

                    cellProps.AppendChild(new TableCellMargin
                    {
                        TopMargin = new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                        BottomMargin = new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                        LeftMargin = new LeftMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                        RightMargin = new RightMargin { Width = "80", Type = TableWidthUnitValues.Dxa }
                    });

                    cell.AppendChild(cellProps);

                    string cellText = dataRow[i]?.ToString() ?? string.Empty;

                    Paragraph p = new Paragraph();
                    p.ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification { Val = JustificationValues.Center },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                    };

                    Run run = new Run();
                    RunProperties runProps = new RunProperties();
                    runProps.AppendChild(new FontSize { Val = "20" });
                    run.AppendChild(runProps);
                    run.AppendChild(new Text(cellText) { Space = SpaceProcessingModeValues.Preserve });
                    p.AppendChild(run);
                    cell.AppendChild(p);
                    row.AppendChild(cell);
                }
                table.AppendChild(row);
            }

            return table;
        }

        /* =========================================| A01 Report |==================================================== */

        // ========== FIXED CREATE SUMMARY TABLE 10 - Full A4 Width ==========
        private Table CreateSummaryTable10(DataTable dt, bool isBH)
        {
            if (dt == null || dt.Rows.Count == 0) return null;

            int colCount = dt.Columns.Count;
            if (colCount == 0) return null;

            Table table = new Table();
            TableProperties tableProps = new TableProperties();

            // Table borders
            TableBorders borders = new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
            );
            tableProps.AppendChild(borders);

            // FIXED: Use DXA for exact width control (A4 page width)
            // USABLE_WIDTH_TWIPS = 12240 - 1440 - 1440 = 9360 twips
            int tableWidth = USABLE_WIDTH_TWIPS;
            tableProps.AppendChild(new TableWidth { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Dxa });

            // FIXED: Use Fixed layout for better control
            tableProps.AppendChild(new TableLayout { Type = TableLayoutValues.Fixed });

            // Table look
            tableProps.AppendChild(new TableLook
            {
                Val = "04A0",
                FirstRow = true,
                LastRow = false,
                FirstColumn = false,
                LastColumn = false,
                NoHorizontalBand = false,
                NoVerticalBand = true
            });

            table.AppendChild(tableProps);

            // Calculate column widths in DXA
            int baseColWidth = tableWidth / colCount;
            int remainder = tableWidth - (baseColWidth * colCount);
            List<int> colWidths = new List<int>();
            for (int i = 0; i < colCount; i++)
            {
                colWidths.Add(i == colCount - 1 ? baseColWidth + remainder : baseColWidth);
            }

            // Create TableGrid with proper column widths
            TableGrid grid = new TableGrid();
            foreach (int width in colWidths)
            {
                grid.AppendChild(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(grid);

            // Create header row
            TableRow headerRow = new TableRow();
            TableRowProperties headerRowProps = new TableRowProperties();
            headerRowProps.AppendChild(new TableRowHeight { Val = 300, HeightType = HeightRuleValues.AtLeast });
            headerRowProps.AppendChild(new CantSplit { Val = OnOffOnlyValues.On });
            headerRow.AppendChild(headerRowProps);

            for (int i = 0; i < colCount; i++)
            {
                TableCell cell = new TableCell();
                TableCellProperties cellProps = new TableCellProperties();

                // Cell width in DXA
                cellProps.AppendChild(new TableCellWidth
                {
                    Width = colWidths[i].ToString(),
                    Type = TableWidthUnitValues.Dxa
                });

                // Gray background for header
                cellProps.AppendChild(new Shading
                {
                    Color = "D9D9D9",
                    Fill = "D9D9D9",
                    Val = new EnumValue<ShadingPatternValues>(ShadingPatternValues.Clear)
                });

                // Cell margins for better spacing
                cellProps.AppendChild(new TableCellMargin
                {
                    TopMargin = new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                    BottomMargin = new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                    LeftMargin = new LeftMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin { Width = "80", Type = TableWidthUnitValues.Dxa }
                });

                // Vertical alignment
                cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });

                cell.AppendChild(cellProps);

                Paragraph p = new Paragraph();
                p.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center },
                    SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                };

                Run run = new Run();
                RunProperties runProps = new RunProperties();
                runProps.AppendChild(new Bold());
                runProps.AppendChild(new FontSize { Val = "22" });
                run.AppendChild(runProps);

                // Get column name
                string columnName = dt.Columns[i].ColumnName;
                run.AppendChild(new Text(columnName) { Space = SpaceProcessingModeValues.Preserve });
                p.AppendChild(run);
                cell.AppendChild(p);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Create data rows
            foreach (DataRow dataRow in dt.Rows)
            {
                TableRow row = new TableRow();
                TableRowProperties rowProps = new TableRowProperties();
                rowProps.AppendChild(new TableRowHeight { Val = 240, HeightType = HeightRuleValues.AtLeast });
                row.AppendChild(rowProps);

                for (int i = 0; i < colCount; i++)
                {
                    TableCell cell = new TableCell();
                    TableCellProperties cellProps = new TableCellProperties();

                    cellProps.AppendChild(new TableCellWidth
                    {
                        Width = colWidths[i].ToString(),
                        Type = TableWidthUnitValues.Dxa
                    });

                    cellProps.AppendChild(new TableCellMargin
                    {
                        TopMargin = new TopMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                        BottomMargin = new BottomMargin { Width = "40", Type = TableWidthUnitValues.Dxa },
                        LeftMargin = new LeftMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                        RightMargin = new RightMargin { Width = "80", Type = TableWidthUnitValues.Dxa }
                    });

                    cellProps.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });

                    cell.AppendChild(cellProps);

                    string cellText = dataRow[i]?.ToString() ?? string.Empty;

                    Paragraph p = new Paragraph();
                    p.ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification { Val = JustificationValues.Center },
                        SpacingBetweenLines = new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.AtLeast }
                    };

                    Run run = new Run();
                    RunProperties runProps = new RunProperties();
                    runProps.AppendChild(new FontSize { Val = "20" });

                    // Check if this is the Result column (last column)
                    bool isResultColumn = (i == colCount - 1);

                    if (isResultColumn)
                    {
                        // Check for PASS/FAIL or LULUS/TIDAK LULUS
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
                        else if (cellText.Equals("LULUS", StringComparison.OrdinalIgnoreCase))
                        {
                            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "008000" });
                            runProps.AppendChild(new Bold());
                        }
                        else if (cellText.Equals("TIDAK LULUS", StringComparison.OrdinalIgnoreCase))
                        {
                            runProps.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = "FF0000" });
                            runProps.AppendChild(new Bold());
                        }
                    }

                    run.AppendChild(runProps);
                    run.AppendChild(new Text(cellText) { Space = SpaceProcessingModeValues.Preserve });
                    p.AppendChild(run);
                    cell.AppendChild(p);
                    row.AppendChild(cell);
                }
                table.AppendChild(row);
            }

            return table;
        }

        // ========== FIXED TABLE 10 - Simple Summary Table (English) ==========
        private void CreateTable10(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ForceReplaceInDocument(doc, placeholder, "No data available");
                return;
            }

            // Find the placeholder
            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null)
            {
                // Try to find by paragraph
                var mainPart = doc.MainDocumentPart;
                var allParagraphs = mainPart.Document.Descendants<Paragraph>().ToList();
                Paragraph foundParagraph = null;
                foreach (var p in allParagraphs)
                {
                    if (GetParagraphFullText(p).Contains(placeholder))
                    {
                        foundParagraph = p;
                        break;
                    }
                }

                if (foundParagraph == null)
                {
                    Debug.WriteLine($"Placeholder '{placeholder}' not found");
                    return;
                }

                // Remove placeholder from paragraph
                RemovePlaceholderFromParagraph(foundParagraph, placeholder);

                // Create and insert table
                Table table = CreateSummaryTable10(dt, isBH: false);
                if (table != null)
                {
                    foundParagraph.InsertAfterSelf(table);
                    Paragraph afterParagraph = new Paragraph();
                    afterParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "120" }
                    };
                    table.InsertAfterSelf(afterParagraph);
                }
                return;
            }

            // Remove placeholder
            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var paragraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (paragraph == null) return;

            // Create and insert table
            Table table2 = CreateSummaryTable10(dt, isBH: false);
            if (table2 != null)
            {
                paragraph.InsertAfterSelf(table2);

                Paragraph afterParagraph = new Paragraph();
                afterParagraph.ParagraphProperties = new ParagraphProperties
                {
                    SpacingBetweenLines = new SpacingBetweenLines { After = "120" }
                };
                table2.InsertAfterSelf(afterParagraph);
            }
        }

        // ========== FIXED TABLE 10 BH - Simple Summary Table (Bahasa Indonesia) ==========
        private void CreateTable10BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ForceReplaceInDocument(doc, placeholder, "Data tidak tersedia");
                return;
            }

            // Find the placeholder
            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null)
            {
                // Try to find by paragraph
                var mainPart = doc.MainDocumentPart;
                var allParagraphs = mainPart.Document.Descendants<Paragraph>().ToList();
                Paragraph foundParagraph = null;
                foreach (var p in allParagraphs)
                {
                    if (GetParagraphFullText(p).Contains(placeholder))
                    {
                        foundParagraph = p;
                        break;
                    }
                }

                if (foundParagraph == null)
                {
                    Debug.WriteLine($"Placeholder '{placeholder}' not found");
                    return;
                }

                // Remove placeholder from paragraph
                RemovePlaceholderFromParagraph(foundParagraph, placeholder);

                // Create and insert table
                Table table = CreateSummaryTable10(dt, isBH: true);
                if (table != null)
                {
                    foundParagraph.InsertAfterSelf(table);
                    Paragraph afterParagraph = new Paragraph();
                    afterParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "120" }
                    };
                    table.InsertAfterSelf(afterParagraph);
                }
                return;
            }

            // Remove placeholder
            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var paragraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (paragraph == null) return;

            // Create and insert table with BH data
            Table table2 = CreateSummaryTable10(dt, isBH: true);
            if (table2 != null)
            {
                paragraph.InsertAfterSelf(table2);

                Paragraph afterParagraph = new Paragraph();
                afterParagraph.ParagraphProperties = new ParagraphProperties
                {
                    SpacingBetweenLines = new SpacingBetweenLines { After = "120" }
                };
                table2.InsertAfterSelf(afterParagraph);
            }
        }

        private void ReplaceTextInDocument(WordprocessingDocument doc, string placeholder, string replacement)
        {
            if (string.IsNullOrEmpty(replacement)) return;

            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            var texts = mainPart.Document.Descendants<Text>().ToList();

            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, replacement);
                }
            }
        }

        private OpenXmlElement FindPlaceholder(WordprocessingDocument doc, string placeholder)
        {
            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return null;

            var texts = mainPart.Document.Descendants<Text>().ToList();

            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    return text.Parent;
                }
            }
            return null;
        }

        // ========== FIXED CreateTable11 - Grouped by Test Parameter (Without TSSP) ==========
        public void CreateTable11(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            // Remove placeholder
            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;
            int groupNumber = 0;

            foreach (var group in groupedData)
            {
                groupNumber++;
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without extracted columns
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

                // Remove columns with all null/blank values
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
                    if (allNullOrBlank)
                    {
                        columnsToRemove.Add(column.ColumnName);
                    }
                }

                foreach (var colName in columnsToRemove)
                {
                    modifiedDt.Columns.Remove(colName);
                }

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
                if (!isFirstGroup)
                {
                    // FIXED: Only ONE spacing paragraph instead of two
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    // Add header with 3.1, 3.2, 3.3 format
                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    // Changed to 3.1, 3.2, 3.3 format
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    // Add pre-treatment line - NO extra spacing
                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    // For first group, add the header with 3.1 format
                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    // Add pre-treatment line
                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }

                // Create and insert table
                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    // FIXED: Minimal spacing after table
                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        // ========== FIXED CreateTable12 - Grouped by Test Item (With TSSP) ==========
        public void CreateTable12(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            // Remove placeholder
            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;
            int groupNumber = 0;

            foreach (var group in groupedData)
            {
                groupNumber++;
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        ReplaceTextInDocument(doc, "{Number}", $"{groupNumber}. ");
                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable
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
                if (!isFirstGroup)
                {
                    // FIXED: Only ONE spacing paragraph
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pre-Treatment based on {groupDt.Rows[0]["Method*"]}, analysis by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }

                // Create and insert table
                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        // ========== FIXED CreateTable11_BH ==========
        public void CreateTable11_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var headers = new Dictionary<string, string>
    {
        {"Test Parameter", "Parameter Uji"},
        {"Test Item", "Item Uji"},
        {"Method*", "Metode*"},
        {"CAS No.", "No. CAS"},
        {"Result (ppm)", "Hasil (ppm)"},
        {"Specification (ppm)", "Spesifikasi (ppm)"},
        {"Equipment", "Peralatan"}
    };

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;
            int groupNumber = 0;

            foreach (var group in groupedData)
            {
                groupNumber++;
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        string newName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(newName);
                    }
                }

                List<string> columnsToRemove = new List<string>();
                foreach (DataColumn column in modifiedDt.Columns)
                {
                    bool allNullOrBlank = true;
                    string originalName = headers.FirstOrDefault(x => x.Value == column.ColumnName).Key ?? column.ColumnName;
                    foreach (DataRow row in groupDt.Rows)
                    {
                        string value = row[originalName]?.ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            allNullOrBlank = false;
                            break;
                        }
                    }
                    if (allNullOrBlank)
                    {
                        columnsToRemove.Add(column.ColumnName);
                    }
                }

                foreach (var colName in columnsToRemove)
                {
                    modifiedDt.Columns.Remove(colName);
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" &&
                            col.ColumnName != "Method*" &&
                            col.ColumnName != "Equipment")
                        {
                            string newName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                            if (modifiedDt.Columns.Contains(newName))
                            {
                                newRow[newName] = row[col.ColumnName];
                            }
                        }
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup)
                {
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pra-Perlakuan berdasarkan {groupDt.Rows[0]["Method*"]}, analisis oleh {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pra-Perlakuan berdasarkan {groupDt.Rows[0]["Method*"]}, analisis oleh {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }

                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        // ========== FIXED CreateTable12_BH ==========
        public void CreateTable12_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var headers = new Dictionary<string, string>
    {
        {"Test Parameter", "Parameter Uji"},
        {"Test Item", "Item Uji"},
        {"Compound", "Senyawa"},
        {"Method*", "Metode*"},
        {"CAS No.", "No. CAS"},
        {"Result (ppm)", "Hasil (ppm)"},
        {"Specification (ppm)", "Spesifikasi (ppm)"},
        {"Equipment", "Peralatan"}
    };

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            bool isFirstGroup = true;
            int groupNumber = 0;

            foreach (var group in groupedData)
            {
                groupNumber++;
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        ReplaceTextInDocument(doc, "{Number}", $"{groupNumber}. ");
                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Test Item" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        string newName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(newName);
                    }
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    int idx = 0;
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" &&
                            col.ColumnName != "Test Item" &&
                            col.ColumnName != "Method*" &&
                            col.ColumnName != "Equipment")
                        {
                            newRow[idx++] = row[col.ColumnName];
                        }
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup)
                {
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pra-Perlakuan berdasarkan {groupDt.Rows[0]["Method*"]}, analisis oleh {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"3.{groupNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"Pra-Perlakuan berdasarkan {groupDt.Rows[0]["Method*"]}, analisis oleh {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }

                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        /* ==============================================================| GENERAL Report |============================================================*/
        private void CreateTable15(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            // Remove placeholder
            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                // Replace placeholders
                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable without extracted columns
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Equipment")
                    {
                        modifiedDt.Columns.Add(col.ColumnName);
                    }
                }

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

                // Insert section header for non-first groups
                if (!isFirstGroup)
                {
                    // Only ONE spacing paragraph
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    // Add header
                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    // Add pre-treatment line
                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"*)Analysis was performed by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    // For first group, add a blank line after placeholder removal
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;
                }

                // Create and insert table
                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    // Minimal spacing after table
                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        private void CreateTable15_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var headers = new Dictionary<string, string>
    {
        {"Test Item", "Item Uji"},
        {"Method*", "Metode*"},
        {"M.D.L", "M.D.L"},
        {"Result(s)", "Hasil"},
        {"Equipment", "Peralatan"}
    };

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                        ReplaceTextInDocument(doc, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Equipment")
                    {
                        string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(colName);
                    }
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    int idx = 0;
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" && col.ColumnName != "Equipment")
                        {
                            newRow[idx++] = row[col.ColumnName];
                        }
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup)
                {
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"*)Analisis dilakukan oleh {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;
                }

                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        private void CreateTable16(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        ReplaceTextInDocument(doc, "{Number}", $"{lastUsedNumber}. ");
                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

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

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    foreach (DataColumn col in modifiedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup)
                {
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"*)Analysis was performed by {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;
                }

                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }

        private void CreateTable16_BH(WordprocessingDocument doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

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

            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            var placeholderElement = FindPlaceholder(doc, placeholder);
            if (placeholderElement == null) return;

            var texts = placeholderElement.Descendants<Text>().ToList();
            foreach (var text in texts)
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, "");
                }
            }

            var currentParagraph = placeholderElement.Ancestors<Paragraph>().FirstOrDefault();
            if (currentParagraph == null) return;

            bool isFirstGroup = true;

            foreach (var group in groupedData)
            {
                lastUsedNumber++;

                DataTable groupDt = dt.Clone();
                foreach (var row in group)
                {
                    groupDt.ImportRow(row);
                }

                if (groupDt.Rows.Count > 0)
                {
                    if (groupDt.Columns.Contains("Test Parameter"))
                    {
                        ReplaceTextInDocument(doc, "{Number}", $"{lastUsedNumber}. ");
                        ReplaceTextInDocument(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Test Item" &&
                        col.ColumnName != "Equipment")
                    {
                        string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(colName);
                    }
                }

                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    int idx = 0;
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" &&
                            col.ColumnName != "Test Item" &&
                            col.ColumnName != "Equipment")
                        {
                            newRow[idx++] = row[col.ColumnName];
                        }
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                if (!isFirstGroup)
                {
                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;

                    Paragraph headerPara = new Paragraph();
                    Run headerRun = new Run();
                    RunProperties headerProps = new RunProperties();
                    headerProps.AppendChild(new Bold());
                    headerProps.AppendChild(new FontSize { Val = "22" });
                    headerRun.AppendChild(headerProps);
                    headerRun.AppendChild(new Text($"{lastUsedNumber}. {groupDt.Rows[0]["Test Parameter"]}: {groupDt.Rows[0]["Test Item"]}") { Space = SpaceProcessingModeValues.Preserve });
                    headerPara.AppendChild(headerRun);
                    currentParagraph.InsertAfterSelf(headerPara);
                    currentParagraph = headerPara;

                    Paragraph preTreatPara = new Paragraph();
                    Run preTreatRun = new Run();
                    preTreatRun.AppendChild(new RunProperties(new FontSize { Val = "20" }));
                    preTreatRun.AppendChild(new Text($"*)Analisis dilakukan oleh {groupDt.Rows[0]["Equipment"]}") { Space = SpaceProcessingModeValues.Preserve });
                    preTreatPara.AppendChild(preTreatRun);
                    currentParagraph.InsertAfterSelf(preTreatPara);
                    currentParagraph = preTreatPara;
                }
                else
                {
                    isFirstGroup = false;

                    Paragraph spacingPara = new Paragraph();
                    spacingPara.AppendChild(new Run(new Break()));
                    currentParagraph.InsertAfterSelf(spacingPara);
                    currentParagraph = spacingPara;
                }

                Table table = CreateFormattedTable(doc, modifiedDt);
                if (table != null)
                {
                    currentParagraph.InsertAfterSelf(table);

                    Paragraph newParagraph = new Paragraph();
                    newParagraph.ParagraphProperties = new ParagraphProperties
                    {
                        SpacingBetweenLines = new SpacingBetweenLines { After = "60" }
                    };
                    table.InsertAfterSelf(newParagraph);
                    currentParagraph = newParagraph;
                }
            }
        }



    }
}
