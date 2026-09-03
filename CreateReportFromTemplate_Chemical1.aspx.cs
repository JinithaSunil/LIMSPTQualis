using DocumentFormat.OpenXml.Vml;
using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using DataTable = System.Data.DataTable;
using LIMS.DAL;
using Syncfusion.DocIORenderer;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.Pdf;
using ZXing;

namespace LIMS
{
    public partial class CreateReportFromTemplate_Chemical1 : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        SyncfusionReportHelper sync = new SyncfusionReportHelper();
        protected void Page_Load(object sender, EventArgs e)
        {
            string logFile = Server.MapPath("~/App_Data/ReportLog.txt");
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            // wordApp.Visible = false;
            //            wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;

            object confirmConversions = false;
            object readOnly = false;
            object addToRecentFiles = false;
            object passwordDocument = Type.Missing;
            object passwordTemplate = Type.Missing;
            object revert = false;
            object writePasswordDocument = Type.Missing;
            object writePasswordTemplate = Type.Missing;
            object format = Type.Missing;
            object encoding = Type.Missing;
            object visible = false;
            object openAndRepair = true;
            object documentDirection = Type.Missing;
            object noEncodingDialog = true;
            object xmlTransform = Type.Missing;

            WordDocument doc = new WordDocument();
            WordDocument doc_BH = new WordDocument();
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
                string templatePath_BH = "";
                DataTable dt1 = _cm.getReportQuery(c);
                templatePath = path + dt1.Rows[0]["Template"].ToString();
                File.AppendAllText(logFile, "2. Word Created" + Environment.NewLine);
                File.AppendAllText(logFile, "Template Path = " + templatePath + Environment.NewLine);

                File.AppendAllText(logFile, "Template Exists = " + File.Exists(templatePath) + Environment.NewLine);
                File.AppendAllText(logFile, "Before Open " + File.Exists(templatePath) + Environment.NewLine);
                try
                {                   
                    FileStream stream1 = new FileStream(templatePath, FileMode.Open);
                    doc.Open(stream1, FormatType.Automatic);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(logFile, "while opening " + ex.Message + ex.StackTrace + Environment.NewLine);
                }

                string template = dt1.Rows[0]["Template"].ToString();

                if (template == "Chemical_TemplateGeneralK3L.docx")
                {
                    templatePath_BH = path + "Chemical_TemplateGeneralK3L_BH.docx";
                }
                else if (template == "Chemical_TemplateGeneral.docx")
                {
                    templatePath_BH = path + "Chemical_TemplateGeneral_BH.docx";
                }
                else if (template == "Chemical_TemplateAO1.docx")
                {
                    templatePath_BH = path + "Chemical_TemplateAO1_BH.docx";
                }

                doc_BH = new WordDocument();
                FileStream stream = new FileStream(templatePath_BH, FileMode.Open);
                doc_BH.Open(stream, FormatType.Automatic);

                File.AppendAllText(logFile, "3. Template Opened" + Environment.NewLine);
                string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                List<string> imagePaths = new List<string> { imageUrl };
                
                sync.InsertImages(doc,"{QR_Code}",imagePaths);
                sync.InsertImages(doc_BH, "{QR_Code}", imagePaths);

                DataTable dt2 = new DataTable();
                DataTable dt2_BH = new DataTable();
                File.AppendAllText(logFile, "3-2. InsertImagesIntoDocument done" + Environment.NewLine);
                if (ChemicalDetails != null && ChemicalDetails.Rows.Count > 0)
                {
                    File.AppendAllText(logFile, "Line#126" + Environment.NewLine);
                    DataRow row = ChemicalDetails.Rows[0];
                    File.AppendAllText(logFile, "Line#128" + Environment.NewLine);
                    string GetSafeValue(string columnName)
                    {
                        return row[columnName] == DBNull.Value || row[columnName] == null ? "" : row[columnName].ToString();
                    }
                    File.AppendAllText(logFile, "GetSafeValue done" + Environment.NewLine);
                    string GetSafeDate(string columnName)
                    {
                        return row[columnName] == DBNull.Value || row[columnName] == null ? "" : Convert.ToDateTime(row[columnName]).ToString("dd MMMM yyyy");
                    }
                    File.AppendAllText(logFile, "GetSafeDate done" + Environment.NewLine);
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
                    File.AppendAllText(logFile, "GetSafeDate1 done" + Environment.NewLine);
                    File.AppendAllText(logFile, "Before sync.ReplaceText block" + Environment.NewLine);
                    try
                    {
                        File.AppendAllText(logFile, "doc before sync.ReplaceText null=" + (doc == null) + Environment.NewLine);

                        if (doc != null)
                        {
                            File.AppendAllText(logFile, "doc hash before sync.ReplaceText=" + doc.GetHashCode() + Environment.NewLine);

                            sync.ReplaceText(doc, "{RequestNo.}", GetSafeValue("sampleIDSeries"));
                            File.AppendAllText(logFile, "After replace sampleIDSeries" + Environment.NewLine);
                            sync.ReplaceText(doc, "{Type}", GetSafeValue("TypeDescription"));
                            sync.ReplaceText(doc, "{GroupofProduct}", GetSafeValue("GroupOfProduct"));
                            sync.ReplaceText(doc, "{ProductDescription}", GetSafeValue("DescriptionOfProduct"));
                            sync.ReplaceText(doc, "{Remark}", GetSafeValue("GetRemarks"));
                            sync.ReplaceText(doc, "{Testreportvaliduntil}", GetSafeDate1("DateOfExp"));
                            sync.ReplaceText(doc, "{ProductGroupName}", GetSafeValue("ProductGroupName"));
                            sync.ReplaceText(doc, "{GroupofProduct}", GetSafeValue("ProductGroupName"));
                            sync.ReplaceText(doc, "{ProductDescription}", GetSafeValue("SampleDescription"));
                            sync.ReplaceText(doc, "{LotNo}", GetSafeValue("LotNumber"));
                            sync.ReplaceText(doc, "{MaterialName}", GetSafeValue("MaterialName_Code"));
                            sync.ReplaceText(doc, "{Email}", GetSafeValue("Email"));
                            sync.ReplaceText(doc, "{CountryOfDestination}", GetSafeValue("CountryOfDestination"));
                            sync.ReplaceText(doc, "{P.O.Order}", GetSafeValue("PONo"));
                            sync.ReplaceText(doc, "{ArticleNumber}", GetSafeValue("ArticleNumber"));
                            sync.ReplaceText(doc, "{Attention}", GetSafeValue("Attention"));
                            sync.ReplaceText(doc, "{TestSample}", GetSafeValue("TestSample"));
                            sync.ReplaceText(doc, "{SampleReceivedDate}", GetSafeDate("IssueDate"));
                            sync.ReplaceText(doc, "{TestPerformancedate}", GetSafeDate("Date"));
                            sync.ReplaceText(doc, "{IssueDate}", GetSafeDate("IssueDate"));
                            sync.ReplaceText(doc, "{TestPeriod}", GetSafeDate("TestPeriod"));
                            sync.ReplaceText(doc, "{Samplecondition}", GetSafeValue("SampleCondition"));
                            sync.ReplaceText(doc, "{Applicant}", GetSafeValue("Applicant"));
                            sync.ReplaceText(doc, "{Address}", GetSafeValue("Address"));
                            sync.ReplaceText(doc, "{SampleDescription}", GetSafeValue("SampleDescription"));
                            sync.ReplaceText(doc, "{Material Name/Code}", GetSafeValue("MaterialName_Code"));
                            sync.ReplaceText(doc, "{Sample1}", GetSafeValue("SampleDescription"));
                            sync.ReplaceText(doc, "{Buyer'sName}", GetSafeValue("Buyer'sName"));
                            sync.ReplaceText(doc, "{CountryofOrigin}", GetSafeValue("CountryOrigin"));
                            sync.ReplaceText(doc, "{BrandProduct}", GetSafeValue("BrandProduct"));
                            sync.ReplaceText(doc, "{AgeGroup}", GetSafeValue("AgeGroup"));
                            sync.ReplaceText(doc, "{Suppliername}", GetSafeValue("Suppliername"));
                            sync.ReplaceText(doc, "{FactoryName}", GetSafeValue("FactoryName"));
                            sync.ReplaceText(doc, "{FactoryAddress}", GetSafeValue("FactoryAddress"));
                            sync.ReplaceText(doc, "{MaterialType}", GetSafeValue("MaterialType"));
                            sync.ReplaceText(doc, "{ItemStyleNo}", GetSafeValue("ItemStyleNo"));
                            sync.ReplaceText(doc, "{MaterialName_Code}", GetSafeValue("MaterialName_Code"));
                            sync.ReplaceText(doc, "{ColorName_Code}", GetSafeValue("ColorName_Code"));
                            sync.ReplaceText(doc, "{BatchNo}", GetSafeValue("BatchNo"));
                            sync.ReplaceText(doc, "{WorkorderNo}", GetSafeValue("WorkorderNumber"));
                            sync.ReplaceText(doc, "{TestCategory}", GetSafeValue("GroupofProduct"));
                            sync.ReplaceText(doc, "{TestMethod}", GetSafeValue("TestMethod"));
                            sync.ReplaceText(doc, "{ColorName}", GetSafeValue("Color"));
                            sync.ReplaceText(doc, "{TestSpecification}", GetSafeValue("ProductSpecification"));
                            sync.ReplaceText(doc, "{TestPackage}", GetSafeValue("GroupofProduct"));
                            File.AppendAllText(logFile, "After sync.ReplaceText block" + Environment.NewLine);
                        }
                        else
                        {
                            File.AppendAllText(logFile, "doc is null so sync.ReplaceText not called" + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(logFile, "Exception:" + ex.Message + ex.StackTrace + Environment.NewLine);
                    }


                    File.AppendAllText(logFile, "Line#206" + Environment.NewLine);

                    sync.ReplaceText(doc_BH, "{RequestNo.}", GetSafeValue("sampleIDSeries"));
                    sync.ReplaceText(doc_BH, "{Type}", GetSafeValue("TypeDescription"));
                    sync.ReplaceText(doc_BH, "{GroupofProduct}", GetSafeValue("GroupOfProduct"));
                    sync.ReplaceText(doc_BH, "{ProductDescription}", GetSafeValue("DescriptionOfProduct"));
                    sync.ReplaceText(doc_BH, "{Remark}", GetSafeValue("GetRemarks"));
                    sync.ReplaceText(doc_BH, "{Testreportvaliduntil}", GetSafeDate1("DateOfExp"));
                    sync.ReplaceText(doc_BH, "{ProductGroupName}", GetSafeValue("ProductGroupName"));
                    sync.ReplaceText(doc_BH, "{GroupofProduct}", GetSafeValue("ProductGroupName"));
                    sync.ReplaceText(doc_BH, "{ProductDescription}", GetSafeValue("SampleDescription"));
                    sync.ReplaceText(doc_BH, "{LotNo}", GetSafeValue("LotNumber"));
                    sync.ReplaceText(doc_BH, "{MaterialName}", GetSafeValue("MaterialName_Code"));
                    sync.ReplaceText(doc_BH, "{Email}", GetSafeValue("Email"));
                    sync.ReplaceText(doc_BH, "{CountryOfDestination}", GetSafeValue("CountryOfDestination"));
                    sync.ReplaceText(doc_BH, "{P.O.Order}", GetSafeValue("PONo"));
                    sync.ReplaceText(doc_BH, "{ArticleNumber}", GetSafeValue("ArticleNumber"));
                    sync.ReplaceText(doc_BH, "{Attention}", GetSafeValue("Attention"));
                    sync.ReplaceText(doc_BH, "{TestSample}", GetSafeValue("TestSample"));
                    sync.ReplaceText(doc_BH, "{SampleReceivedDate}", GetSafeDate("IssueDate"));
                    sync.ReplaceText(doc_BH, "{TestPerformancedate}", GetSafeDate("Date"));
                    sync.ReplaceText(doc_BH, "{IssueDate}", GetSafeDate("IssueDate"));
                    sync.ReplaceText(doc_BH, "{TestPeriod}", GetSafeDate("TestPeriod"));
                    sync.ReplaceText(doc_BH, "{Samplecondition}", GetSafeValue("SampleCondition"));
                    sync.ReplaceText(doc_BH, "{Applicant}", GetSafeValue("Applicant"));
                    sync.ReplaceText(doc_BH, "{Address}", GetSafeValue("Address"));
                    sync.ReplaceText(doc_BH, "{SampleDescription}", GetSafeValue("SampleDescription"));
                    sync.ReplaceText(doc_BH, "{Material Name/Code}", GetSafeValue("MaterialName_Code"));
                    sync.ReplaceText(doc_BH, "{Sample1}", GetSafeValue("SampleDescription"));
                    sync.ReplaceText(doc_BH, "{Buyer'sName}", GetSafeValue("Buyer'sName"));
                    sync.ReplaceText(doc_BH, "{CountryofOrigin}", GetSafeValue("CountryOrigin"));
                    sync.ReplaceText(doc_BH, "{BrandProduct}", GetSafeValue("BrandProduct"));
                    sync.ReplaceText(doc_BH, "{AgeGroup}", GetSafeValue("AgeGroup"));
                    sync.ReplaceText(doc_BH, "{Suppliername}", GetSafeValue("Suppliername"));
                    sync.ReplaceText(doc_BH, "{FactoryName}", GetSafeValue("FactoryName"));
                    sync.ReplaceText(doc_BH, "{FactoryAddress}", GetSafeValue("FactoryAddress"));
                    sync.ReplaceText(doc_BH, "{MaterialType}", GetSafeValue("MaterialType"));
                    sync.ReplaceText(doc_BH, "{ItemStyleNo}", GetSafeValue("ItemStyleNo"));
                    sync.ReplaceText(doc_BH, "{MaterialName_Code}", GetSafeValue("MaterialName_Code"));
                    sync.ReplaceText(doc_BH, "{ColorName_Code}", GetSafeValue("ColorName_Code"));
                    sync.ReplaceText(doc_BH, "{BatchNo}", GetSafeValue("BatchNo"));
                    sync.ReplaceText(doc_BH, "{WorkorderNo}", GetSafeValue("WorkorderNumber"));
                    sync.ReplaceText(doc_BH, "{TestCategory}", GetSafeValue("GroupofProduct"));
                    sync.ReplaceText(doc_BH, "{TestMethod}", GetSafeValue("TestMethod"));
                    sync.ReplaceText(doc_BH, "{ColorName}", GetSafeValue("Color"));
                    sync.ReplaceText(doc_BH, "{TestSpecification}", GetSafeValue("ProductSpecification"));
                    sync.ReplaceText(doc_BH, "{TestPackage}", GetSafeValue("GroupofProduct"));
                    File.AppendAllText(logFile, "Line#253" + Environment.NewLine);
                    string finalResult = GetSafeValue("FinalTestResult").Trim().ToUpper();
                    File.AppendAllText(logFile, "3-3." + finalResult + Environment.NewLine);
                    if (finalResult == "PASS")
                    {
                        File.AppendAllText(logFile, "Before ResultTable" + Environment.NewLine);
                        sync.InsertResultTable( doc, "{FinalTestResult}", "PASS");
                        sync.InsertResultTable(doc_BH, "{FinalTestResult}", "LULUS");
                    }
                    else
                    {
                        sync.InsertResultTable( doc, "{FinalTestResult}", "FAIL");
                        sync.InsertResultTable(doc_BH, "{FinalTestResult}", "TIDAK LULUS");
                    }
                    ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
                    sync.ReplaceText(doc, "{TestReportNo.}", GetSafeValue("TestReportNo"));

                    sync.ReplaceHeaderFooter(doc, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    sync.ReplaceHeaderFooter(doc, "{DocNo}", "TSQ-LAB-QAF-089");

                    sync.ReplaceText(doc_BH, "{TestReportNo.}", GetSafeValue("TestReportNo"));
                    sync.ReplaceHeaderFooter(doc_BH, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    sync.ReplaceHeaderFooter(doc_BH, "{DocNo}", "TSQ-LAB-QAF-089");
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

                    string uniqueFileName_BH = SampleUID + "_BH";
                    string outputDocxPath_BH = pathout + "TestReport-" + uniqueFileName_BH + ".docx";
                    string outputPdfPath_BH = pathout + "TestReport-" + uniqueFileName_BH + ".pdf";

                    if (ReportIDGet == 28)
                    {
                        path = Server.MapPath("~/FilesUpload/");
                        sampleID = Convert.ToInt32(ViewState["SampleIDGet"].ToString());
                        string filePath = System.IO.Path.Combine(path, sampleID + ".xlsx");

                        sync.AdditionalDataAdded(doc, "{AdditionalData}", filePath);
                        sync.AdditionalDataAdded(doc_BH, "{AdditionalData}",filePath);
                    }

                    int counter = 0;
                    if (dt1.Rows.Count > 0)
                    {
                        string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');
                        int lastUsedNumber = 0;
                        for (int i = 0; i < query.Length; i++)
                        {
                            counter = i; 
                            File.AppendAllText(logFile, table[i].ToString() + Environment.NewLine);
                            if (table[i] == "10")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateSimpleTable(doc, "{ResultSummaryTable}", dt2);

                                string Query = @"SELECT ROW_NUMBER() OVER (ORDER BY tp.TestParameter_BH) AS [No.], tp.TestParameter_BH AS [Test parameters] FROM crfsampledetails c 
                                                 INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                                 INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                 INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                 INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                 GROUP BY tp.TestParameter_BH, t.sampleID having sampleID ='" + s + "' ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                sync.CreateSimpleTable(doc_BH, "{ResultSummaryTable}", dt2);
                            }

                            if (table[i] == "11")
                            {
                               
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateGroupedTable(
                                doc,
                                "{Table11}",
                                dt2,
                                "Test Parameter",
                                new string[]
                                {
                                    "Test Parameter",
                                    "Method*",
                                    "Equipment"
                                },
                                row => $"{lastUsedNumber}. {row["Test Parameter"]}",
                                row =>
                                    $"Pre-Treatment based on {row["Method*"]}, analysis by {row["Equipment"]}",
                                ref lastUsedNumber);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                                  c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                  INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                  INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                  WHERE t.sampleID= '" + s + "' ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                sync.CreateGroupedTable(
                                doc_BH,
                                "{Table11}",
                                dt2,
                                "Test Parameter",
                                new string[]
                                {
                                    "Test Parameter",
                                    "Method*",
                                    "Equipment"
                                },
                                row => $"{lastUsedNumber}. {row["Test Parameter"]}",
                                row =>
                                    $"Pre-Treatment based on {row["Method*"]}, analysis by {row["Equipment"]}",
                                ref lastUsedNumber);
                            }

                            if (table[i] == "12")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateGroupedTable(
                                 doc,
                                 "{TestParameter}",
                                 dt2,
                                 "Test Item",
                                 new string[]
                                 {
                                    "Test Parameter",
                                    "Test Item",
                                    "Method*",
                                    "Equipment"
                                 },
                                 row => $"{lastUsedNumber}. {row["Test Parameter"]}: {row["Test Item"]}",
                                 row => $"Pre-Treatment based on {row["Method*"]}, analysis by {row["Equipment"]}",
                                 ref lastUsedNumber);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                                 c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                                 INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                 INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                 INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                 WHERE t.sampleID='" + s + "' ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                sync.CreateGroupedTable(
                                        doc_BH,
                                        "{TestParameter}",
                                        dt2,
                                        "Test Item",
                                        new string[]
                                        {
                                            "Test Parameter",
                                            "Test Item",
                                            "Method*",
                                            "Equipment"
                                        },
                                        row => $"{lastUsedNumber}. {row["Test Parameter"]}: {row["Test Item"]}",
                                        row => $"Pre-Treatment based on {row["Method*"]}, analysis by {row["Equipment"]}",
                                        ref lastUsedNumber);
                            }

                            if (table[i] == "13")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                List<Tuple<string, string>> imageData = new List<Tuple<string, string>>();

                                foreach (DataRow row in dt2.Rows)
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
                                sync.InsertImages(
                                    doc,
                                    "{images}",
                                    imageData.Select(x => x.Item1).ToList());
                                sync.InsertImages(
                                    doc_BH,
                                    "{images}",
                                    imageData.Select(x => x.Item1).ToList());                               
                            }

                            if (table[i] == "14")
                            {
                                dt2 = _cm.getReportQuery(1, query[i]);
                                sync.CreateSimpleTable(doc, "{Testingandmeasuringequipment}", dt2);

                                string Query = @"select Equipment ,Type,Calibration_Date as[Calibration Due Date] from TestResultDetails t 
                                                 INNER JOIN Calibration_Equipment c  on c.sampleid = t.SampleId ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                sync.CreateSimpleTable(doc_BH, "{Testingandmeasuringequipment}", dt2);
                            }

                            if (table[i] == "15")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                               
                                sync.CreateGroupedTable(
                                    doc,
                                    "{ResultTable1}",
                                    dt2,
                                    "Test Parameter",
                                    new string[]
                                    {
                                        "Test Parameter",
                                        "Equipment"
                                    },
                                    row => $"{lastUsedNumber}. {row["Test Parameter"]}",
                                    row => $"*)Analysis was performed by {row["Equipment"]}",
                                    ref lastUsedNumber);

                                string Query = @"SELECT tp.TestParameter_BH AS[Test Parameter], tsp.TestSubParameter_BH AS[Test Item], TestMethod AS[Method *],
                                                 c.MDL as [M.D.L],TestResult AS[Result(s)], t.EquipmentName AS[Equipment] FROM crfsampledetails c
                                                 INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                 INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                 INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND(c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '')
                                                 WHERE t.sampleID ='" + s + "' ";

                                dt2 = _cm.getReportQuery_BH(Query);
                                sync.CreateGroupedTable(
                                        doc_BH,
                                        "{ResultTable1}",
                                        dt2,
                                        "Test Parameter",
                                        new string[]
                                        {
                                            "Test Parameter",
                                            "Equipment"
                                        },
                                        row => $"{lastUsedNumber}. {row["Test Parameter"]}",
                                        row => $"*)Analysis was performed by {row["Equipment"]}",
                                        ref lastUsedNumber);
                            }

                            if (table[i] == "16")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateGroupedTable(
                                        doc,
                                        "{ResultTable2}",
                                        dt2,
                                        "Test Item",
                                        new string[]
                                        {
                                            "Test Parameter",
                                            "Test Item",
                                            "Equipment"
                                        },
                                        row => $"{lastUsedNumber}. {row["Test Parameter"]}: {row["Test Item"]}",
                                        row => $"*)Analysis was performed by {row["Equipment"]}",
                                        ref lastUsedNumber);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Test Item], tssp.TestSubSubParameter_BH AS [Compound], 
                                                TestMethod AS [Method*], c.MDL as [M.D.L],TestResult AS [Result(s)],
                                                t.EquipmentName AS [Equipment] FROM crfsampledetails c 
                                                INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NOT NULL AND c.TestSubSubParameter != '') 
                                                WHERE t.sampleID ='" + s + "' ";

                                dt2 = _cm.getReportQuery_BH(Query);
                                sync.CreateGroupedTable(
                                        doc_BH,
                                        "{ResultTable2}",
                                        dt2,
                                        "Test Item",
                                        new string[]
                                        {
                                            "Test Parameter",
                                            "Test Item",
                                            "Equipment"
                                        },
                                        row => $"{lastUsedNumber}. {row["Test Parameter"]}: {row["Test Item"]}",
                                        row => $"*)Analysis was performed by {row["Equipment"]}",
                                        ref lastUsedNumber);
                            }

                            if (table[i] == "17")
                            {
                                dt2 = _cm.GetResultOverallConclusion(s);
                                sync.CreateSimpleTable(doc, "{Overall Conclusion}", dt2);

                                dt2_BH = _cm.GetResultOverallConclusion_BH(s);
                                sync.CreateSimpleTable(doc_BH, "{Overall Conclusion}", dt2);
                            }

                            if (table[i] == "18")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateChemicalResultTable(doc, "{ResultTable1}", dt2, ref lastUsedNumber);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Element], c.CASNo AS [CAS Number], TestMethod AS [Method*], 
                                                TestResult AS [Result(mg/kg)], t.Specification AS [Requirement], t.EquipmentName AS [Equipment], ccs.ColourCodeId AS [Colour Code], 
                                                tsp.MDL, t.Conclusion_BH as Conclusion FROM crfsampledetails c 
                                                INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                                INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                INNER JOIN ColourCodeSave ccs ON ccs.AdditionalColourCodeId = t.AdditionalColourCodesId WHERE t.sampleID ='" + s + "' ";
                                dt2_BH = _cm.getReportQuery_BH(Query);
                                sync.CreateSimpleTable(doc_BH, "{ResultTable1}", dt2_BH);
                            }

                            if (table[i] == "19")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateSimpleTable(doc, "{ResultTable2}", dt2);
                                string Query = @"SELECT tp.TestParameter_BH AS [Parameter Uji], tsp.TestSubParameter_BH AS [Barang Tes], 
                                                tssp.TestSubSubParameter_BH AS [Senyawa], c.CASNo AS [CAS Number],TestMethod AS [Method*], 
                                                TestResult AS [Hasil(mg/kg)],t.EquipmentName AS [Peralatan],
                                                ccs.ColourCodeId AS [Colour Code], tsp.MDL, t.Conclusion_BH AS Conclusion 
                                                FROM crfsampledetails c 
                                                INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                                INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID AND (c.TestSubSubParameter IS NOT NULL AND c.TestSubSubParameter != '') 
                                                INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                INNER JOIN ColourCodeSave ccs ON ccs.AdditionalColourCodeId = t.AdditionalColourCodesId
                                                WHERE t.sampleID = '" + s + "' ";

                                dt2_BH = _cm.getReportQuery_BH(Query);
                                sync.CreateSimpleTable(doc_BH, "{ResultTable2}", dt2_BH);
                            }

                            if (table[i] == "20")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                sync.CreateSampleDescriptionTable(doc, "{SampleDescriptionData}", dt2);

                                string Query = @"select SampleText+ ' - ' + SampleName + ' (' + SampleColourName_BH +')' AS [Sample Description], SampleColorCode AS [Sample ID] from SDColourCodeImages where SampleId ='" + s + "' ";
                                dt2_BH = _cm.getReportQuery_BH(Query);
                                sync.CreateSampleDescriptionTable(doc_BH, "{SampleDescriptionData}", dt2_BH);
                            }

                        }

                        try
                        {
                            if (counter == query.Length - 1)
                            {
                                string directory = System.IO.Path.GetDirectoryName(outputPdfPath);
                                File.AppendAllText(logFile, "3-1. Directory: " + directory + Environment.NewLine);

                                try
                                {
                                    if (!Directory.Exists(directory))
                                    {
                                        Directory.CreateDirectory(directory);
                                    }

                                    // Save DOCX
                                    using (FileStream fs = new FileStream(
                                             outputDocxPath,
                                             FileMode.Create,
                                             FileAccess.Write))
                                    {
                                        doc.Save(fs, FormatType.Docx);
                                    }

                                    // Save PDF
                                    DocIORenderer renderer =  new DocIORenderer();

                                    PdfDocument pdf =
                                        renderer.ConvertToPDF(doc);

                                    using (FileStream fs =
                                        new FileStream(
                                            outputPdfPath,
                                            FileMode.Create,
                                            FileAccess.Write))
                                    {
                                        pdf.Save(fs);
                                    }

                                    pdf.Close(true);

                                    // Save DOCX (BH)
                                    using (FileStream fs = new FileStream(
                                             outputDocxPath_BH,
                                             FileMode.Create,
                                             FileAccess.Write))
                                    {
                                        doc_BH.Save(fs, FormatType.Docx);
                                    }

                                    // Save PDF (BH)
                                    DocIORenderer rendererBH = new DocIORenderer();

                                    PdfDocument pdfBH = rendererBH.ConvertToPDF(doc_BH);

                                    using (FileStream fs = new FileStream(
                                            outputPdfPath_BH,
                                            FileMode.Create,
                                            FileAccess.Write))
                                    {
                                        pdfBH.Save(fs);
                                    }

                                    pdfBH.Close(true);
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(
                                        logFile,
                                        ex.Message +
                                        ex.StackTrace +
                                        Environment.NewLine);
                                }

                                File.AppendAllText(logFile, "4. Doc Saved" + Environment.NewLine);

                                ///* Clean up COM objects */
                                //Marshal.ReleaseComObject(doc);
                                //doc = null;

                                //Marshal.ReleaseComObject(doc_BH);
                                //doc_BH = null;

                                // Return PDF to client

                                string pdf1 = "/TestReport/TestReport-" + SampleUID + ".pdf";
                                string pdf2 = "/TestReport/TestReport-" + SampleUID + "_BH.pdf";

                                string script = $@"<script>
                                                        window.open('{pdf1}', '_blank');
                                                        setTimeout(function() {{
                                                            window.open('{pdf2}', '_blank');
                                                        }}, 800);
                                                    </script>";
                                ClientScript.RegisterStartupScript(this.GetType(), "OpenPDFs", script);

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
                            //                   if (doc != null)
                            //                   {
                            //                       System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                            //                   }
                            //                   if (wordApp != null)
                            //                   {
                            //                       System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                            //                   }
                            //if (doc_BH != null)
                            //                   {
                            //                       Marshal.ReleaseComObject(doc_BH);
                            //                   }
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
                        doc.Close();
                        doc.Dispose();
                        doc = null;
                    }

                    if (doc_BH != null)
                    {
                        doc_BH.Close();
                        doc_BH.Dispose();
                        doc_BH = null;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

        }
         public void CreatePassFailTable(string result)
        {
            string logFile = Server.MapPath("~/App_Data/ReportLog.txt");
            object missing = System.Type.Missing;
            File.AppendAllText(logFile, "1. Starting" + Environment.NewLine);
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;

            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add();

            // Create a 2x2 table
            Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(doc.Range(0, 0), 2, 2, ref missing, ref missing);
            table.Borders.Enable = 1; // Enable borders

            // Set table width
            table.PreferredWidthType = Microsoft.Office.Interop.Word.WdPreferredWidthType.wdPreferredWidthPoints;
            table.PreferredWidth = 145f; // table width in points (~3.5 inches)

            // Set row height
            foreach (Microsoft.Office.Interop.Word.Row row in table.Rows)
            {
                row.HeightRule = Microsoft.Office.Interop.Word.WdRowHeightRule.wdRowHeightExactly;
                row.Height = 25f; // row height in points
            }

            // Set column widths
            table.Columns[1].PreferredWidth = 25f;  // checkbox column
            table.Columns[2].PreferredWidth = 120f; // text column

            // First row - PASS
            Microsoft.Office.Interop.Word.Cell cell1 = table.Cell(1, 1);
            cell1.Range.FormFields.Add(cell1.Range, Microsoft.Office.Interop.Word.WdFieldType.wdFieldFormCheckBox);
            cell1.Range.Font.Size = 14;
            cell1.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            table.Cell(1, 2).Range.Text = "PASS";
            table.Cell(1, 2).Range.Font.Size = 14;
            table.Cell(1, 2).Range.Font.TextColor.RGB = 0x800000;
            table.Cell(1, 2).Range.Font.Bold = 1;

            // Second row - FAIL
            Microsoft.Office.Interop.Word.Cell cell2 = table.Cell(2, 1);
            cell2.Range.FormFields.Add(cell2.Range, Microsoft.Office.Interop.Word.WdFieldType.wdFieldFormCheckBox);
            cell2.Range.Font.Size = 14;
            cell2.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            table.Cell(2, 2).Range.Text = "FAIL";
            table.Cell(2, 2).Range.Font.Size = 14;
            table.Cell(2, 2).Range.Font.TextColor.RGB = 0x800000;
            table.Cell(2, 2).Range.Font.Bold = 1;

            // Set checkbox based on parameter
            if (!string.IsNullOrEmpty(result))
            {
                string upperResult = result.Trim().ToUpper();
                if (upperResult == "PASS")
                {
                    table.Cell(1, 1).Range.FormFields[1].CheckBox.Value = true;
                }
                else if (upperResult == "FAIL")
                {
                    table.Cell(2, 1).Range.FormFields[1].CheckBox.Value = true;
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

      
        private string WrapTextForMultipleColumns(string text, int columnCount)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // For many columns, keep text compact - don't add line breaks
            // This will prevent the header from becoming too tall
            return text;
        }

        private string WrapText(string text, int maxLength)
        {
            if (text.Length <= maxLength) return text;

            // Find the best split point (after a '+')
            int splitPos = text.LastIndexOf('+', maxLength);
            if (splitPos <= 0) splitPos = maxLength; // Fallback

            return text.Substring(0, splitPos) + "\n" + text.Substring(splitPos);
        }

        private string WrapRequirementText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Common breaking points for requirement text
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

            // If no natural break point found, break at space near middle
            int middle = text.Length / 2;
            int spaceIndex = text.LastIndexOf(' ', middle);
            if (spaceIndex > 0)
            {
                return text.Substring(0, spaceIndex) + "\n" + text.Substring(spaceIndex + 1);
            }

            return text;
        }

        private void CreateTable19(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    // No rows — replace all placeholders with blank
                    sync.ReplaceText(doc, "{Number}", string.Empty);
                    sync.ReplaceText(doc, "{TestParameter2}", string.Empty);
                    sync.ReplaceText(doc, "{TestSubParameter}", string.Empty);
                    sync.ReplaceText(doc, "{TestMethod}", string.Empty);
                    sync.ReplaceText(doc, "{Equipment2}", string.Empty);
                    sync.ReplaceText(doc, "{ResultTable2}", string.Empty);
                    sync.ReplaceText(doc, "{AnalysisWasPerformed}", string.Empty);
                    sync.ReplaceText(doc, "{:}", string.Empty);
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
                            sync.ReplaceText(doc, "{Number}", testParamValue);

                            sync.ReplaceText(doc, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                            sync.ReplaceText(doc, "{AnalysisWasPerformed}", "*) Analysis was performed ");
                            sync.ReplaceText(doc, "{:}", " : ");
                        }
                        if (groupDt.Columns.Contains("Test Item"))
                            sync.ReplaceText(doc, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                        if (groupDt.Columns.Contains("Method*"))
                            sync.ReplaceText(doc, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                        if (groupDt.Columns.Contains("Equipment"))
                            sync.ReplaceText(doc, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                    }

                    // Extract all unique color codes for this group - TRIM AND CLEAN
                    var allColorCodes = group
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .SelectMany(code => code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(code => code.Trim()) // Additional trim for individual codes
                        .Where(code => !string.IsNullOrWhiteSpace(code)) // Filter again after trim
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
                            col.ColumnName != "Result(mg/kg)" &&
                            col.ColumnName != "Conclusion" &&
                            col.ColumnName != "MDL") // Exclude MDL column
                        {
                            modifiedDt.Columns.Add(col.ColumnName);
                        }
                    }

                    // Add dynamic result columns for each color code - with consistent spacing
                    foreach (var colorCode in allColorCodes)
                    {
                        // Clean the color code and create consistent column name
                        string cleanColorCode = colorCode.Trim();
                        // Use consistent spacing pattern: single space after comma if multiple codes
                        string columnName = $"Result (mg/kg) ({cleanColorCode})";
                        modifiedDt.Columns.Add(columnName);
                    }

                    // Group by all non-result columns to combine duplicate compounds
                    var compoundGroups = group
                        .GroupBy(row => string.Join("|",
                            modifiedDt.Columns.Cast<DataColumn>()
                                .Where(c => !c.ColumnName.StartsWith("Result (mg/kg)"))
                                .Select(c => row[c.ColumnName]?.ToString() ?? "")))
                        .ToList();

                    // Track detection per color code for remarks
                    bool[] colorCodeHasDetection = new bool[allColorCodes.Count];

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

                        // Initialize result columns with default values
                        for (int i = 0; i < allColorCodes.Count; i++)
                        {
                            string columnName = $"Result (mg/kg) ({allColorCodes[i].Trim()})";
                            newRow[columnName] = "N.D"; // Default value
                        }

                        // Handle result columns based on color codes
                        foreach (var row in compoundGroup)
                        {
                            string colourCodeRaw = row["Colour Code"]?.ToString()?.Trim() ?? "";
                            string resultValue = row["Result(mg/kg)"]?.ToString()?.Trim() ?? "";

                            if (!string.IsNullOrWhiteSpace(colourCodeRaw))
                            {
                                var individualCodes = colourCodeRaw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var code in individualCodes)
                                {
                                    string cleanCode = code.Trim();
                                    string columnName = $"Result (mg/kg) ({cleanCode})";
                                    if (modifiedDt.Columns.Contains(columnName))
                                    {
                                        newRow[columnName] = resultValue;

                                        // Check for detection
                                        int colorIndex = allColorCodes.FindIndex(c => c.Trim().Equals(cleanCode, StringComparison.OrdinalIgnoreCase));
                                        if (colorIndex >= 0 &&
                                            (resultValue.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                             resultValue.Equals("Detected", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            colorCodeHasDetection[colorIndex] = true;
                                        }
                                    }
                                }
                            }
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
                            currentRange.Text = $"Test Method:{groupDt.Rows[0]["Method*"]}, analyzed by {groupDt.Rows[0]["Equipment"]}\n";
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
                    }

                    if (currentRange != null)
                    {
                        // Create table with modified DataTable + 1 extra row for Remarks
                        Table table = doc.Tables.Add(currentRange, modifiedDt.Rows.Count + 2, modifiedDt.Columns.Count);

                        // Set overall table properties
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = true;
                        table.PreferredWidth = 100;
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                        // Set column headers with normalized spacing
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string headerText = modifiedDt.Columns[col].ColumnName;

                            // Normalize all whitespace to single spaces
                            headerText = System.Text.RegularExpressions.Regex.Replace(headerText, @"\s+", " ").Trim();

                            // Special handling for Result columns to ensure consistent formatting
                            if (headerText.StartsWith("Result (mg/kg)"))
                            {
                                // Extract the color codes part after "Result (mg/kg)"
                                string pattern = @"Result \(mg/kg\) \((.+)\)";
                                var match = System.Text.RegularExpressions.Regex.Match(headerText, pattern);

                                if (match.Success)
                                {
                                    string colorCodes = match.Groups[1].Value.Trim();
                                    // Format: Line 1 = "Result (mg/kg)", Line 2 = "(color codes)"
                                    headerText = $"Result (mg/kg)\n({colorCodes})";
                                }
                            }

                            table.Cell(1, col + 1).Range.Text = headerText;
                            table.Cell(1, col + 1).Range.Bold = 1;
                            table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            table.Cell(1, col + 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(1, col + 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Adjust column widths - make result columns narrower
                            float columnWidth;
                            if (modifiedDt.Columns[col].ColumnName.StartsWith("Result (mg/kg)"))
                            {
                                columnWidth = 60f / modifiedDt.Columns.Count;
                            }
                            else
                            {
                                columnWidth = 100f / modifiedDt.Columns.Count;
                            }

                            table.Columns[col + 1].PreferredWidth = columnWidth;
                            table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                        }

                        // Fill the table with data
                        for (int row = 0; row < modifiedDt.Rows.Count; row++)
                        {
                            for (int col = 0; col < modifiedDt.Columns.Count; col++)
                            {
                                string cellText = modifiedDt.Rows[row][col]?.ToString()?.Trim() ?? string.Empty;
                                var cell = table.Cell(row + 2, col + 1);
                                cell.Range.Text = cellText;

                                // Check column name for alignment
                                string colName = modifiedDt.Columns[col].ColumnName;

                                if (colName == "CAS Number" || colName.StartsWith("Result (mg/kg)"))
                                {
                                    // Center align CAS Number & Result columns
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else
                                {
                                    // Left align Compound or other columns
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                }

                                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            }
                        }

                        // Add Remarks row
                        int remarksRowIndex = modifiedDt.Rows.Count + 2;

                        // Set "Remarks" in the first column
                        table.Cell(remarksRowIndex, 1).Range.Text = "Remarks";
                        table.Cell(remarksRowIndex, 1).Range.Bold = 1;
                        table.Cell(remarksRowIndex, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                        // Fill empty cells for non-result columns
                        int firstResultColumn = -1;
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            if (modifiedDt.Columns[col].ColumnName.StartsWith("Result (mg/kg)"))
                            {
                                if (firstResultColumn == -1) firstResultColumn = col;

                                // Add PASS/FAIL for each color code column
                                int colorIndex = col - firstResultColumn;
                                if (colorIndex < colorCodeHasDetection.Length)
                                {
                                    string remarkText = colorCodeHasDetection[colorIndex] ? "FAIL" : "PASS";
                                    table.Cell(remarksRowIndex, col + 1).Range.Text = remarkText;
                                    table.Cell(remarksRowIndex, col + 1).Range.Font.Color =
                                        colorCodeHasDetection[colorIndex] ? WdColor.wdColorRed : WdColor.wdColorDarkGreen;
                                    table.Cell(remarksRowIndex, col + 1).Range.Font.Bold = 1;
                                    table.Cell(remarksRowIndex, col + 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                            }
                            else if (col > 0) // Skip first column (already filled with "Remarks")
                            {
                                table.Cell(remarksRowIndex, col + 1).Range.Text = "";
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
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Method Detection Limit - only for compounds with MDL values
                        var detectionLimits = group
                            .Where(row => !string.IsNullOrWhiteSpace(row["MDL"]?.ToString()))
                            .Select(row => $"{row["Element/Compound"]}: {row["MDL"]} mg/kg")
                            .Distinct()
                            .ToList();

                        if (detectionLimits.Count > 0)
                        {
                            string detectionLimitText = "Method Detection Limits- " + string.Join(", ", detectionLimits);
                            currentRange.Text = detectionLimitText;
                            currentRange.Font.Size = 10;
                            currentRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
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
                Microsoft.Office.Interop.Word.Range range = doc.Content;

                if (dt == null || dt.Rows.Count == 0)
                {
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;

                    if (range.Find.Execute())
                    {
                        range.Text = "";
                    }
                    Console.WriteLine($"Table for '{placeholder}' skipped - no data available");
                    return;
                }

                range = doc.Content;
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
                    wordRange.Text = "";
                    wordRange.Paste();
                    workbook.Save();
                }

                Table wordTable = doc.Tables[doc.Tables.Count];
                try
                {
                    wordTable.Select();
                    wordApp.Selection.Font.Size = 8;
                    wordApp.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    wordApp.Selection.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }
                catch (Exception fontEx1)
                {
                    try
                    {
                        object start = wordTable.Range.Start;
                        object end = wordTable.Range.End;
                        Microsoft.Office.Interop.Word.Range tableRange = doc.Range(ref start, ref end);
                        tableRange.Font.Size = 8;
                        tableRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        tableRange.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }
                    catch (Exception fontEx2)
                    {
                        try
                        {
                            foreach (Row row in wordTable.Rows)
                            {
                                row.Range.Font.Size = 8;
                                row.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                row.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
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
                                        wordTable.Cell(row, col).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                        wordTable.Cell(row, col).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
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

        public void AdditionalDataAddedStatic(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder)
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

                // Find the placeholder first
                Microsoft.Office.Interop.Word.Range wordRange = doc.Content;
                wordRange.Find.ClearFormatting();
                wordRange.Find.Text = placeholder;

                if (wordRange.Find.Execute())
                {
                    // Clear the placeholder text
                    wordRange.Text = "";

                    // Create a range at the placeholder position for headers
                    Microsoft.Office.Interop.Word.Range headingRange = doc.Range(wordRange.Start, wordRange.Start);

                    // 1st row: ATTACHMENT
                    headingRange.InsertAfter("LAMPIRAN");
                    headingRange.Font.Bold = 1;
                    headingRange.Font.Size = 16;
                    headingRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headingRange.InsertParagraphAfter();


                    headingRange.InsertAfter("ATTACHMENT");
                    headingRange.Font.Bold = 1;
                    headingRange.Font.Size = 16;
                    headingRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headingRange.InsertParagraphAfter();

                    // 2nd row: Table of Product Grouping
                    headingRange = doc.Range(headingRange.End, headingRange.End);
                    headingRange.InsertAfter("Table of Product Grouping");
                    headingRange.Font.Bold = 1;
                    headingRange.Font.Size = 14;
                    headingRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headingRange.InsertParagraphAfter();

                    // 3rd row: PT. HOME CENTER INDONESIA
                    headingRange = doc.Range(headingRange.End, headingRange.End);
                    headingRange.InsertAfter("PT. HOME CENTER INDONESIA");
                    headingRange.Font.Bold = 1;
                    headingRange.Font.Size = 12;
                    headingRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headingRange.InsertParagraphAfter();

                    // 4th row: Address line 1
                    headingRange = doc.Range(headingRange.End, headingRange.End);
                    headingRange.InsertAfter("GEDUNG KAWAN LAMA, JL. PURI KENCANA NO. 1, Desa/Kelurahan Kembangan Selatan,");
                    headingRange.Font.Bold = 0;
                    headingRange.Font.Size = 10;
                    headingRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headingRange.InsertParagraphAfter();

                    // 5th row: Address line 2
                    headingRange = doc.Range(headingRange.End, headingRange.End);
                    headingRange.InsertAfter("Kec. Kembangan, Kota Adm. Jakarta Barat, Provinsi DKI Jakarta, Kode Pos: 11610.");
                    headingRange.Font.Bold = 0;
                    headingRange.Font.Size = 10;
                    headingRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headingRange.InsertParagraphAfter();

                    // Add two empty lines after headings before the table
                    headingRange = doc.Range(headingRange.End, headingRange.End);
                    headingRange.InsertParagraphAfter();
                    headingRange.InsertParagraphAfter();


                    // Open Excel file and copy data
                    excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    workbook = excelApp.Workbooks.Open(filePath);
                    sheets = workbook.Sheets;
                    worksheet = (Excel.Worksheet)sheets[1];
                    range = worksheet.UsedRange;
                    range.Copy();

                    // Position cursor at the end of headers to paste Excel data
                    Microsoft.Office.Interop.Word.Range pasteRange = doc.Range(headingRange.End, headingRange.End);
                    pasteRange.Paste();

                    // Save the workbook
                    workbook.Save();

                    // Get the last table (the one we just pasted)
                    if (doc.Tables.Count > 0)
                    {
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
                                        Debug.WriteLine("Could not set font size: " +
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
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
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


        /*******************************************************************| Chemical End |********************************************************************/







        private void InsertImagesIntoDocument_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, List<string> imagePaths)
        {

            try
            {
                var range = doc_BH.Content;
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
                    Table table = doc_BH.Tables.Add(range, rows, 1);
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

        private void InsertResultTableIntoDocument_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, string result)
        {
            try
            {
                var range = doc_BH.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute() && !string.IsNullOrEmpty(result))
                {
                    // Remove the placeholder text
                    range.Text = "";

                    // Create a 2 row x 1 column table
                    Table table = doc_BH.Tables.Add(range, 2, 1);
                    table.Borders.Enable = 1; // Enable borders

                    // Set table width
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.PreferredWidth = 145f;

                    // Set row height and alignment
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightExactly;
                        row.Height = 25f;
                        row.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Left align row content
                    }

                    // First row - PASS
                    Microsoft.Office.Interop.Word.Range passRange = table.Cell(1, 1).Range;
                    FormField passCheckbox = passRange.FormFields.Add(passRange, WdFieldType.wdFieldFormCheckBox);

                    // Set checkbox properties and left align
                    passCheckbox.CheckBox.Size = 14;
                    passRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Add space and text after checkbox
                    passRange.InsertAfter("LULUS");

                    // Format the text part
                    passRange.Font.Size = 14;
                    passRange.Font.TextColor.RGB = 0x800000;
                    passRange.Font.Bold = 1;

                    // Center content vertically in cell
                    table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Second row - FAIL
                    Microsoft.Office.Interop.Word.Range failRange = table.Cell(2, 1).Range;
                    FormField failCheckbox = failRange.FormFields.Add(failRange, WdFieldType.wdFieldFormCheckBox);

                    // Set checkbox properties and left align
                    failCheckbox.CheckBox.Size = 14;
                    failRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Add space and text after checkbox
                    failRange.InsertAfter("TIDAK LULUS");

                    // Format the text part
                    failRange.Font.Size = 14;
                    failRange.Font.TextColor.RGB = 0x800000;
                    failRange.Font.Bold = 1;

                    // Center content vertically in cell
                    table.Cell(2, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Set checkbox based on parameter
                    string upperResult = result.Trim().ToUpper();
                    if (upperResult == "LULUS")
                    {
                        passCheckbox.CheckBox.Value = true;
                    }
                    else if (upperResult == "TIDAK LULUS")
                    {
                        failCheckbox.CheckBox.Value = true;
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

        private void InsertImagesIntoDocument1_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, List<Tuple<string, string>> imageData)
        {
            try
            {
                var range = doc_BH.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imageData != null && imageData.Count > 0)
                {
                    int imagesPerRow = 2;
                    int totalImages = imageData.Count;
                    int rows = (int)Math.Ceiling((double)totalImages / imagesPerRow);

                    Table table = doc_BH.Tables.Add(range, rows, imagesPerRow);
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

        private void CreateTable17_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                sync.ReplaceText(doc_BH, "{Overall Conclusion}", "No data available");
                sync.ReplaceText(doc_BH, "{Requirement}", "No requirements available");
                return;
            }

            // First handle Requirements placeholder
            var requirements = dt.AsEnumerable().Select(row => row["Requirement"].ToString()).Where(req => !string.IsNullOrWhiteSpace(req)).Distinct().ToList();

            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            sync.ReplaceText(doc_BH, "{Requirement}", combinedRequirements);

            // Create a new DataTable without the Requirement column
            DataTable modifiedDt = dt.Copy();
            modifiedDt.Columns.Remove("Requirement");

            // Handle table creation for Overall Conclusion
            var range = doc_BH.Content;
            range.Find.ClearFormatting();
            range.Find.Text = "{Overall Conclusion}";
            range.Find.Execute();

            if (range.Find.Found)
            {
                // Create a table in Word based on the modified DataTable
                Table table = doc_BH.Tables.Add(range, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                // Set overall table properties
                table.Borders.Enable = 1;
                table.AllowAutoFit = false;
                table.PreferredWidth = 100;
                table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Define fixed widths in % for each column
                float[] columnWidths = { 10f, 35f, 35f, 20f }; // Adjust if needed
                float usableWidth = wordApp_BH.InchesToPoints(6.5f); // Assumed usable width

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

        private void CreateTableforSampleDescription_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            try
            {
                Microsoft.Office.Interop.Word.Range range = doc_BH.Content;

                if (dt == null || dt.Rows.Count == 0)
                {
                    range.Find.ClearFormatting();
                    range.Find.Text = placeholder;

                    if (range.Find.Execute())
                    {
                        range.Text = "";
                    }
                    Console.WriteLine($"Table for '{placeholder}' skipped - no data available");
                    return;
                }

                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    range.Text = "";

                    int totalRows = dt.Rows.Count + 1; // header + data rows
                    int totalColumns = 3; // Sr.No, Sample Description, Sample ID

                    Table table = doc_BH.Tables.Add(range, totalRows, totalColumns);
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = false;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.PreferredWidth = 100;

                    float totalWidthPoints = wordApp_BH.InchesToPoints(6.5f); // Assuming usable page width is ~6.5 inches
                    float[] columnWidths = { 10f, 65f, 25f }; // Percentages

                    // Set header texts
                    table.Cell(1, 1).Range.Text = "#";
                    table.Cell(1, 2).Range.Text = "Deskripsi Sample Uji";
                    table.Cell(1, 3).Range.Text = "ID Sampel";

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


        private void CreateTable18_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
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

                    var allColorCodeCombinations = testParamGroup
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    var firstRow = testParamGroup.First();
                    sync.ReplaceText(doc_BH, "{TestParameter}", firstRow["Test Parameter"].ToString());
                    sync.ReplaceText(doc_BH, "{TestMethod}", firstRow["Method*"].ToString());
                    sync.ReplaceText(doc_BH, "{Equipment}", firstRow["Equipment"].ToString());

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
                        currentRange.Text = $"Metode Tes: {firstRow["Method*"]}, analyzed by {firstRow["Equipment"]}\n";
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    if (isFirstGroup)
                    {
                        var range = doc_BH.Content;
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
                        int resultSubCols = allColorCodeCombinations.Count;
                        int totalColumns = 3 + resultSubCols;
                        bool hasManyColumns = resultSubCols > 4;

                        // Create table with extra row for Remark
                        Table table = doc_BH.Tables.Add(currentRange, elementGroups.Count + 2, totalColumns);

                        // Set table properties
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = true;
                        table.PreferredWidth = wordApp_BH.CentimetersToPoints(17f);
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                        table.AllowPageBreaks = true;

                        // Set column widths based on number of columns
                        if (!hasManyColumns)
                        {
                            // Original layout for 4 or fewer columns
                            table.Columns[1].PreferredWidth = wordApp_BH.CentimetersToPoints(3.5f); // Element
                            table.Columns[2].PreferredWidth = wordApp_BH.CentimetersToPoints(2.0f); // CAS Number
                            for (int i = 0; i < resultSubCols; i++)
                            {
                                table.Columns[3 + i].PreferredWidth = wordApp_BH.CentimetersToPoints(2.0f); // Result columns
                            }
                            table.Columns[3 + resultSubCols].PreferredWidth = wordApp_BH.CentimetersToPoints(2.5f); // Requirement
                        }
                        else
                        {
                            // Adjusted layout for more than 4 columns
                            table.Columns[1].PreferredWidth = wordApp_BH.CentimetersToPoints(3.0f); // Element (slightly smaller)
                            table.Columns[2].PreferredWidth = wordApp_BH.CentimetersToPoints(1.8f); // CAS Number (slightly smaller)

                            // Calculate available width for result columns
                            float availableWidth = 17f - 3.0f - 1.8f - 2.5f; // Total - Element - CAS - Requirement
                            float resultColumnWidth = availableWidth / resultSubCols;

                            // Ensure reasonable width for result columns
                            resultColumnWidth = Math.Max(1.5f, Math.Min(2.0f, resultColumnWidth));

                            for (int i = 0; i < resultSubCols; i++)
                            {
                                table.Columns[3 + i].PreferredWidth = wordApp_BH.CentimetersToPoints(resultColumnWidth);
                            }
                            table.Columns[3 + resultSubCols].PreferredWidth = wordApp_BH.CentimetersToPoints(2.5f); // Requirement
                        }

                        // Header row configuration - reduced height for many columns
                        table.Rows[1].Height = wordApp_BH.CentimetersToPoints(hasManyColumns ? 1.2f : 1.5f);

                        // Set Element header
                        table.Cell(1, 1).Range.Text = "Item Uji";
                        table.Cell(1, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(1, 1).Range.Font.Bold = 1;
                        table.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Set CAS Number header
                        table.Cell(1, 2).Range.Text = "CAS\n Number";
                        table.Cell(1, 2).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(1, 2).Range.Font.Bold = 1;
                        table.Cell(1, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(1, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Result columns headers
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            string codeCombination = allColorCodeCombinations[i];
                            Cell headerCell = table.Cell(1, 3 + i);

                            // Clear existing content
                            headerCell.Range.Text = "";

                            // Add "Result(mg/kg)" on the first line
                            headerCell.Range.Text = "Hasil Uji\n (mg/kg)";
                            headerCell.Range.InsertParagraphAfter();

                            // Use appropriate wrapping strategy - FIXED VERSION
                            string wrappedCombination = hasManyColumns ?
                                WrapTextForMultipleColumns_BH(codeCombination, resultSubCols) :
                                WrapText_BH(codeCombination, 14);

                            headerCell.Range.InsertAfter(wrappedCombination);

                            // Apply formatting
                            headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            headerCell.Range.Font.Bold = 1;
                            headerCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            headerCell.Range.ParagraphFormat.SpaceBefore = 0;
                            headerCell.Range.ParagraphFormat.SpaceAfter = 0;

                            // Adjust font size for many columns - made even smaller
                            if (hasManyColumns)
                            {
                                headerCell.Range.Font.Size = 8; // Changed from 9 to 8
                            }
                        }

                        // Requirement column header
                        Cell requirementHeaderCell = table.Cell(1, 3 + resultSubCols);
                        requirementHeaderCell.Range.Text = "Persyaratan\n (mg/kg)";
                        requirementHeaderCell.Range.Font.Bold = 1;
                        requirementHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        requirementHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        requirementHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        requirementHeaderCell.WordWrap = true;

                        // Data rows
                        int dataRowIndex = 2;
                        bool[] combinationHasDetection = new bool[resultSubCols];

                        foreach (var elementGroup in elementGroups)
                        {
                            var firstElementRow = elementGroup.First();

                            // Set element cell with proper wrapping for long names
                            Cell elementCell = table.Cell(dataRowIndex, 1);
                            elementCell.Range.Text = firstElementRow["Element"].ToString();
                            elementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            elementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            elementCell.WordWrap = true;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                elementCell.Range.Font.Size = 9;
                            }

                            // Set CAS Number cell
                            table.Cell(dataRowIndex, 2).Range.Text = firstElementRow["CAS Number"].ToString();
                            table.Cell(dataRowIndex, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(dataRowIndex, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                table.Cell(dataRowIndex, 2).Range.Font.Size = 9;
                            }

                            // Requirement cell
                            Cell requirementCell = table.Cell(dataRowIndex, 3 + resultSubCols);
                            string requirementText = firstElementRow["Requirement"].ToString();
                            requirementText = WrapRequirementText_BH(requirementText);
                            requirementCell.Range.Text = requirementText;
                            requirementCell.WordWrap = true;
                            requirementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            requirementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                requirementCell.Range.Font.Size = 9;
                                requirementCell.Range.ParagraphFormat.LineSpacing = wordApp_BH.LinesToPoints(0.9f);
                            }

                            // Initialize all result columns
                            for (int i = 0; i < resultSubCols; i++)
                            {
                                Cell resultCell = table.Cell(dataRowIndex, 3 + i);
                                resultCell.Range.Text = "N.D";
                                resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                                // Adjust font size for many columns
                                if (hasManyColumns)
                                {
                                    resultCell.Range.Font.Size = 9;
                                }
                            }

                            // Fill actual results
                            foreach (var row in elementGroup)
                            {
                                string result = row["Result(mg/kg)"].ToString();
                                string colourCodeCombination = row["Colour Code"]?.ToString() ?? "";

                                if (!string.IsNullOrWhiteSpace(colourCodeCombination))
                                {
                                    int comboIndex = allColorCodeCombinations.IndexOf(colourCodeCombination);
                                    if (comboIndex >= 0)
                                    {
                                        Cell resultCell = table.Cell(dataRowIndex, 3 + comboIndex);
                                        resultCell.Range.Text = result;
                                        resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                        resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                                        // Adjust font size for many columns
                                        if (hasManyColumns)
                                        {
                                            resultCell.Range.Font.Size = 9;
                                        }

                                        if (result.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                            result.Equals("Detected", StringComparison.OrdinalIgnoreCase))
                                        {
                                            combinationHasDetection[comboIndex] = true;
                                        }
                                    }
                                }
                            }
                            dataRowIndex++;
                        }

                        // Remark row
                        int remarkRow = dataRowIndex;

                        // Remark label
                        table.Cell(remarkRow, 1).Range.Text = "Keterangan";
                        table.Cell(remarkRow, 1).Range.Font.Bold = 1;
                        table.Cell(remarkRow, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(remarkRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(remarkRow, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Empty CAS Number cell in remark row
                        table.Cell(remarkRow, 2).Range.Text = "";
                        table.Cell(remarkRow, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(remarkRow, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Remark result columns
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            Cell remarkCell = table.Cell(remarkRow, 3 + i);
                            string remarkText = combinationHasDetection[i] ? "TIDAK LULUS" : "LULUS";
                            remarkCell.Range.Text = remarkText;
                            remarkCell.Range.Font.Color = combinationHasDetection[i] ? WdColor.wdColorRed : WdColor.wdColorDarkGreen;
                            remarkCell.Range.Font.Bold = 1;
                            remarkCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            remarkCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                remarkCell.Range.Font.Size = 9;
                            }
                        }

                        // Remark requirement cell
                        Cell remarkRequirementCell = table.Cell(remarkRow, 3 + resultSubCols);
                        remarkRequirementCell.Range.Text = "";
                        remarkRequirementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        remarkRequirementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Format all rows and cells with proper alignment
                        foreach (Row row in table.Rows)
                        {
                            row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                            row.Height = wordApp_BH.CentimetersToPoints(hasManyColumns ? 1.2f : 1.0f);

                            foreach (Cell cell in row.Cells)
                            {
                                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                cell.Range.ParagraphFormat.SpaceAfter = 0;
                                cell.Range.ParagraphFormat.SpaceBefore = 0;

                                // Maintain font size 10 for 4 or fewer columns, adjust for more
                                if (!hasManyColumns)
                                {
                                    cell.Range.Font.Size = 10;
                                }
                                else
                                {
                                    cell.Range.Font.Size = 9;
                                }

                                // Apply specific alignment based on column
                                if (cell.ColumnIndex == 1) // Element column
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                    cell.WordWrap = true; // Enable wrapping for long element names
                                }
                                else if (cell.ColumnIndex == 2) // CAS Number column
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else if (cell.ColumnIndex >= 3 && cell.ColumnIndex < 3 + resultSubCols) // Result columns
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else if (cell.ColumnIndex == 3 + resultSubCols) // Requirement column
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                    cell.WordWrap = true; // Enable wrapping for requirement text
                                }
                            }
                        }

                        currentRange = table.Range;
                        currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Method Detection Limit
                        var detectionLimits = new List<string>();
                        foreach (var elementGroup in elementGroups)
                        {
                            var firstElementRow = elementGroup.First();
                            string element = firstElementRow["Element"].ToString();
                            string requirement = firstElementRow["MDL"].ToString();

                            if (!string.IsNullOrWhiteSpace(requirement) && requirement != "Not Detected")
                            {
                                detectionLimits.Add($"{element}: {requirement} (mg/kg)");
                            }
                        }

                        if (detectionLimits.Count > 0)
                        {
                            currentRange.Text = "Batas Deteksi Metode- " + string.Join(", ", detectionLimits);
                            currentRange.Font.Size = 10;
                            currentRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
                    }
                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error: {ex.Message.Replace("'", "\\'")}');</script>");
            }
        }

        private string WrapTextForMultipleColumns_BH(string text, int columnCount)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // For many columns, keep text compact - don't add line breaks
            // This will prevent the header from becoming too tall
            return text;
        }

        private string WrapText_BH(string text, int maxLength)
        {
            if (text.Length <= maxLength) return text;

            // Find the best split point (after a '+')
            int splitPos = text.LastIndexOf('+', maxLength);
            if (splitPos <= 0) splitPos = maxLength; // Fallback

            return text.Substring(0, splitPos) + "\n" + text.Substring(splitPos);
        }

        private string WrapRequirementText_BH(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Common breaking points for requirement text
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

            // If no natural break point found, break at space near middle
            int middle = text.Length / 2;
            int spaceIndex = text.LastIndexOf(' ', middle);
            if (spaceIndex > 0)
            {
                return text.Substring(0, spaceIndex) + "\n" + text.Substring(spaceIndex + 1);
            }

            return text;
        }


        private void CreateTable19_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    // No rows — replace all placeholders with blank
                    sync.ReplaceText(doc_BH, "{Number}", string.Empty);
                    sync.ReplaceText(doc_BH, "{TestParameter2}", string.Empty);
                    sync.ReplaceText(doc_BH, "{TestSubParameter}", string.Empty);
                    sync.ReplaceText(doc_BH, "{TestMethod}", string.Empty);
                    sync.ReplaceText(doc_BH, "{Equipment2}", string.Empty);
                    sync.ReplaceText(doc_BH, "{ResultTable2}", string.Empty);
                    sync.ReplaceText(doc_BH, "{AnalysisWasPerformed}", string.Empty);
                    sync.ReplaceText(doc_BH, "{:}", string.Empty);
                    return;
                }

                // Group the data by Barang Tes (Test Item)
                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Barang Tes"))
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
                        if (groupDt.Columns.Contains("Parameter Uji"))
                        {
                            // Replace {Number} with the actual number in the placeholder
                            string testParamValue = $"{lastUsedNumber}.    ";
                            sync.ReplaceText(doc_BH, "{Number}", testParamValue);

                            sync.ReplaceText(doc_BH, "{TestParameter2}", groupDt.Rows[0]["Parameter Uji"].ToString());
                            sync.ReplaceText(doc_BH, "{AnalysisWasPerformed}", "*) Analysis was performed ");
                            sync.ReplaceText(doc_BH, "{:}", " : ");
                        }
                        if (groupDt.Columns.Contains("Barang Tes"))
                            sync.ReplaceText(doc_BH, "{TestSubParameter}", groupDt.Rows[0]["Barang Tes"].ToString());
                        if (groupDt.Columns.Contains("Method*"))
                            sync.ReplaceText(doc_BH, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                        if (groupDt.Columns.Contains("Peralatan"))
                            sync.ReplaceText(doc_BH, "{Equipment2}", groupDt.Rows[0]["Peralatan"].ToString());
                    }

                    // Extract all unique color codes for this group - TRIM AND CLEAN
                    var allColorCodes = group
                        .Select(row => row.Field<string>("Colour Code")?.Trim() ?? "")
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .SelectMany(code => code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(code => code.Trim()) // Additional trim for individual codes
                        .Where(code => !string.IsNullOrWhiteSpace(code)) // Filter again after trim
                        .Distinct()
                        .OrderBy(code => code)
                        .ToList();

                    // Create modified DataTable with fixed columns and dynamic result columns
                    DataTable modifiedDt = new DataTable();

                    // Add fixed columns (excluding those we'll handle specially)
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Parameter Uji" &&
                            col.ColumnName != "Barang Tes" &&
                            col.ColumnName != "Method*" &&
                            col.ColumnName != "Peralatan" &&
                            col.ColumnName != "Colour Code" &&
                            col.ColumnName != "Hasil(mg/kg)" &&
                            col.ColumnName != "Conclusion" &&
                            col.ColumnName != "MDL") // Exclude MDL column
                        {
                            modifiedDt.Columns.Add(col.ColumnName);
                        }
                    }

                    // Add dynamic result columns for each color code - with consistent spacing
                    foreach (var colorCode in allColorCodes)
                    {
                        // Clean the color code and create consistent column name
                        string cleanColorCode = colorCode.Trim();
                        // Use consistent spacing pattern: single space after comma if multiple codes
                        string columnName = $"Result (mg/kg) ({cleanColorCode})";
                        modifiedDt.Columns.Add(columnName);
                    }

                    // Group by all non-result columns to combine duplicate compounds
                    var compoundGroups = group
                        .GroupBy(row => string.Join("|",
                            modifiedDt.Columns.Cast<DataColumn>()
                                .Where(c => !c.ColumnName.StartsWith("Result (mg/kg)"))
                                .Select(c => row[c.ColumnName]?.ToString() ?? "")))
                        .ToList();

                    // Track detection per color code for remarks
                    bool[] colorCodeHasDetection = new bool[allColorCodes.Count];

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

                        // Initialize result columns with default values
                        for (int i = 0; i < allColorCodes.Count; i++)
                        {
                            string columnName = $"Result (mg/kg) ({allColorCodes[i].Trim()})";
                            newRow[columnName] = "N.D"; // Default value
                        }

                        // Handle result columns based on color codes
                        foreach (var row in compoundGroup)
                        {
                            string colourCodeRaw = row["Colour Code"]?.ToString()?.Trim() ?? "";
                            string resultValue = row["Hasil(mg/kg)"]?.ToString()?.Trim() ?? "";

                            if (!string.IsNullOrWhiteSpace(colourCodeRaw))
                            {
                                var individualCodes = colourCodeRaw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var code in individualCodes)
                                {
                                    string cleanCode = code.Trim();
                                    string columnName = $"Result (mg/kg) ({cleanCode})";
                                    if (modifiedDt.Columns.Contains(columnName))
                                    {
                                        newRow[columnName] = resultValue;

                                        // Check for detection
                                        int colorIndex = allColorCodes.FindIndex(c => c.Trim().Equals(cleanCode, StringComparison.OrdinalIgnoreCase));
                                        if (colorIndex >= 0 &&
                                            (resultValue.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                                             resultValue.Equals("Detected", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            colorCodeHasDetection[colorIndex] = true;
                                        }
                                    }
                                }
                            }
                        }

                        modifiedDt.Rows.Add(newRow);
                    }

                    // Find or create position for the table
                    if (isFirstGroup)
                    {
                        // For the first group, use the original placeholder
                        var range = doc_BH.Content;
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
                            currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Parameter Uji"]}:{groupDt.Rows[0]["Barang Tes"]} \n";
                            currentRange.Font.Bold = 1;
                            currentRange.Font.Size = 11;
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;

                            // Insert pre-treatment line
                            currentRange.Text = $"Test Method:{groupDt.Rows[0]["Method*"]}, analyzed by {groupDt.Rows[0]["Peralatan"]}\n";
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
                    }

                    if (currentRange != null)
                    {
                        // Create table with modified DataTable + 1 extra row for Remarks
                        Table table = doc_BH.Tables.Add(currentRange, modifiedDt.Rows.Count + 2, modifiedDt.Columns.Count);

                        // Set overall table properties
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = true;
                        table.PreferredWidth = 100;
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                        // Set column headers with normalized spacing
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string headerText = modifiedDt.Columns[col].ColumnName;

                            // Normalize all whitespace to single spaces
                            headerText = System.Text.RegularExpressions.Regex.Replace(headerText, @"\s+", " ").Trim();

                            // Special handling for Result columns to ensure consistent formatting
                            if (headerText.StartsWith("Result (mg/kg)"))
                            {
                                // Extract the color codes part after "Result (mg/kg)"
                                string pattern = @"Result \(mg/kg\) \((.+)\)";
                                var match = System.Text.RegularExpressions.Regex.Match(headerText, pattern);

                                if (match.Success)
                                {
                                    string colorCodes = match.Groups[1].Value.Trim();
                                    // Format: Line 1 = "Result (mg/kg)", Line 2 = "(color codes)"
                                    headerText = $"Result (mg/kg)\n({colorCodes})";
                                }
                            }

                            table.Cell(1, col + 1).Range.Text = headerText;
                            table.Cell(1, col + 1).Range.Bold = 1;
                            table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            table.Cell(1, col + 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(1, col + 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Adjust column widths - make result columns narrower
                            float columnWidth;
                            if (modifiedDt.Columns[col].ColumnName.StartsWith("Result (mg/kg)"))
                            {
                                columnWidth = 60f / modifiedDt.Columns.Count;
                            }
                            else
                            {
                                columnWidth = 100f / modifiedDt.Columns.Count;
                            }

                            table.Columns[col + 1].PreferredWidth = columnWidth;
                            table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                        }

                        // Fill the table with data
                        for (int row = 0; row < modifiedDt.Rows.Count; row++)
                        {
                            for (int col = 0; col < modifiedDt.Columns.Count; col++)
                            {
                                string cellText = modifiedDt.Rows[row][col]?.ToString()?.Trim() ?? string.Empty;
                                var cell = table.Cell(row + 2, col + 1);
                                cell.Range.Text = cellText;

                                // Check column name for alignment
                                string colName = modifiedDt.Columns[col].ColumnName;

                                if (colName == "CAS Number" || colName.StartsWith("Result (mg/kg)"))
                                {
                                    // Center align CAS Number & Result columns
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else
                                {
                                    // Left align Compound or other columns
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                }

                                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            }
                        }

                        // Add Remarks row
                        int remarksRowIndex = modifiedDt.Rows.Count + 2;

                        // Set "Remarks" in the first column
                        table.Cell(remarksRowIndex, 1).Range.Text = "Keterangan";
                        table.Cell(remarksRowIndex, 1).Range.Bold = 1;
                        table.Cell(remarksRowIndex, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                        // Fill empty cells for non-result columns
                        int firstResultColumn = -1;
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            if (modifiedDt.Columns[col].ColumnName.StartsWith("Result (mg/kg)"))
                            {
                                if (firstResultColumn == -1) firstResultColumn = col;

                                // Add PASS/FAIL for each color code column
                                int colorIndex = col - firstResultColumn;
                                if (colorIndex < colorCodeHasDetection.Length)
                                {
                                    string remarkText = colorCodeHasDetection[colorIndex] ? "TIDAK LULUS" : "LULUS";
                                    table.Cell(remarksRowIndex, col + 1).Range.Text = remarkText;
                                    table.Cell(remarksRowIndex, col + 1).Range.Font.Color =
                                        colorCodeHasDetection[colorIndex] ? WdColor.wdColorRed : WdColor.wdColorDarkGreen;
                                    table.Cell(remarksRowIndex, col + 1).Range.Font.Bold = 1;
                                    table.Cell(remarksRowIndex, col + 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                            }
                            else if (col > 0) // Skip first column (already filled with "Remarks")
                            {
                                table.Cell(remarksRowIndex, col + 1).Range.Text = "";
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
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Method Detection Limit - only for compounds with MDL values
                        var detectionLimits = group
                            .Where(row => !string.IsNullOrWhiteSpace(row["MDL"]?.ToString()))
                            .Select(row => $"{row["Senyawa"]}: {row["MDL"]} mg/kg")
                            .Distinct()
                            .ToList();

                        if (detectionLimits.Count > 0)
                        {
                            string detectionLimitText = "Batas Deteksi Metode- " + string.Join(", ", detectionLimits);
                            currentRange.Text = detectionLimitText;
                            currentRange.Font.Size = 10;
                            currentRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
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

        public void AdditionalDataAdded_BH(Microsoft.Office.Interop.Word.Application wordApp_BH, Document doc_BH, string placeholder)
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

                Microsoft.Office.Interop.Word.Range wordRange = doc_BH.Content;
                wordRange.Find.ClearFormatting();
                wordRange.Find.Text = placeholder;
                if (wordRange.Find.Execute())
                {
                    wordRange.Text = "";
                    wordRange.Paste();
                    workbook.Save();
                }

                Table wordTable = doc_BH.Tables[doc_BH.Tables.Count];

                // Set font size and center alignment for the entire table
                try
                {
                    wordTable.Select();
                    wordApp_BH.Selection.Font.Size = 8;
                    wordApp_BH.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    wordApp_BH.Selection.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }
                catch (Exception fontEx1)
                {
                    try
                    {
                        object start = wordTable.Range.Start;
                        object end = wordTable.Range.End;
                        Microsoft.Office.Interop.Word.Range tableRange = doc_BH.Range(ref start, ref end);
                        tableRange.Font.Size = 8;
                        tableRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        tableRange.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }
                    catch (Exception fontEx2)
                    {
                        try
                        {
                            foreach (Row row in wordTable.Rows)
                            {
                                row.Range.Font.Size = 8;
                                row.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                row.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
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
                                        wordTable.Cell(row, col).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                        wordTable.Cell(row, col).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                    }
                                }
                            }
                            catch (Exception fontEx4)
                            {
                                Debug.WriteLine("Could not set font size or alignment: " +
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
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
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
  
        /*******************************************************************| Chemical BH CODE END |**********************************************************/
    }
}