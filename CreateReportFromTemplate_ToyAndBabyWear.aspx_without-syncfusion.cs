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
    public partial class CreateReportFromTemplate_ToyAndBabyWear : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            Application wordApp = new Application();
            Document doc = null;
            Application wordApp_BH = new Application();
            Document doc_BH = null;
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
                templatePath_BH = path + dt1.Rows[0]["Template_BH"].ToString();
                doc = wordApp.Documents.Add(templatePath);
                string template = dt1.Rows[0]["Template"].ToString();

                int InquiryId = Convert.ToInt32(ChemicalDetails.Rows[0]["InquiryId"].ToString());



                //if (ReportIDGet == 6417)
                //{
                //    if (template == "ToysAndBabyWear_Template1.docx")
                //    {
                //        templatePath_BH = path + "ToysAndBabyWear_Template1_BH.docx";
                //    }
                //}
                //else if (ReportIDGet == 6418)
                //{
                //    if (template == "ToysAndBabyWear_Template2.docx")
                //    {
                //        templatePath_BH = path + "ToysAndBabyWear_Template2_BH.docx";
                //    }
                //}
                //else if (ReportIDGet == 6419)
                //{
                //    templatePath_BH = path + "Chemical_TemplateGeneral_BH.docx";
                //}
                //else if (template == "Chemical_TemplateAO1.docx")
                //{
                //    templatePath_BH = path + "Chemical_TemplateAO1_BH.docx";
                //}

                doc_BH = wordApp_BH.Documents.Add(templatePath_BH);
                string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                List<string> imagePaths = new List<string> { imageUrl };
                InsertImagesIntoDocument(wordApp, doc, "{QR_Code}", imagePaths);
                InsertImagesIntoDocument_BH(wordApp_BH, doc_BH, "{QR_Code}", imagePaths);
                DataTable dt2 = new DataTable();
                DataTable dt2_BH = new DataTable();

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
                    FindAndReplace(wordApp, "{Attention}", GetSafeValue("Attention"));
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
                    FindAndReplace(wordApp, "{WorkorderNo}", GetSafeValue("WorkorderNumber"));
                    FindAndReplace(wordApp, "{TestCategory}", GetSafeValue("GroupofProduct"));
                    FindAndReplace(wordApp, "{TestMethod}", GetSafeValue("TestMethod"));
                    FindAndReplace(wordApp, "{ColorName}", GetSafeValue("Color"));
                    FindAndReplace(wordApp, "{TestSpecification}", GetSafeValue("ProductSpecification"));
                    FindAndReplace(wordApp, "{TestPackage}", GetSafeValue("GroupofProduct"));

                    FindAndReplace(wordApp_BH, "{RequestNo.}", GetSafeValue("sampleIDSeries"));
                    FindAndReplace(wordApp_BH, "{Type}", GetSafeValue("TypeDescription"));
                    FindAndReplace(wordApp_BH, "{GroupofProduct}", GetSafeValue("GroupOfProduct"));
                    FindAndReplace(wordApp_BH, "{ProductDescription}", GetSafeValue("DescriptionOfProduct"));
                    FindAndReplace(wordApp_BH, "{Remark}", GetSafeValue("GetRemarks"));
                    FindAndReplace(wordApp_BH, "{Testreportvaliduntil}", GetSafeDate1("DateOfExp"));
                    FindAndReplace(wordApp_BH, "{ProductGroupName}", GetSafeValue("ProductGroupName"));
                    FindAndReplace(wordApp_BH, "{GroupofProduct}", GetSafeValue("ProductGroupName"));
                    FindAndReplace(wordApp_BH, "{ProductDescription}", GetSafeValue("SampleDescription"));
                    FindAndReplace(wordApp_BH, "{LotNo}", GetSafeValue("LotNumber"));
                    FindAndReplace(wordApp_BH, "{MaterialName}", GetSafeValue("MaterialName_Code"));
                    FindAndReplace(wordApp_BH, "{Email}", GetSafeValue("Email"));
                    FindAndReplace(wordApp_BH, "{CountryOfDestination}", GetSafeValue("CountryOfDestination"));
                    FindAndReplace(wordApp_BH, "{P.O.Order}", GetSafeValue("PONo"));
                    FindAndReplace(wordApp_BH, "{ArticleNumber}", GetSafeValue("ArticleNumber"));
                    FindAndReplace(wordApp_BH, "{Attention}", GetSafeValue("Attention"));
                    FindAndReplace(wordApp_BH, "{TestSample}", GetSafeValue("TestSample"));
                    FindAndReplace(wordApp_BH, "{SampleReceivedDate}", GetSafeDate("IssueDate"));
                    FindAndReplace(wordApp_BH, "{TestPerformancedate}", GetSafeDate("Date"));
                    FindAndReplace(wordApp_BH, "{IssueDate}", GetSafeDate("IssueDate"));
                    FindAndReplace(wordApp_BH, "{TestPeriod}", GetSafeDate("TestPeriod"));
                    FindAndReplace(wordApp_BH, "{Samplecondition}", GetSafeValue("SampleCondition"));
                    FindAndReplace(wordApp_BH, "{Applicant}", GetSafeValue("Applicant"));
                    FindAndReplace(wordApp_BH, "{Address}", GetSafeValue("Address"));
                    FindAndReplace(wordApp_BH, "{SampleDescription}", GetSafeValue("SampleDescription"));
                    FindAndReplace(wordApp_BH, "{Material Name/Code}", GetSafeValue("MaterialName_Code"));
                    FindAndReplace(wordApp_BH, "{Sample1}", GetSafeValue("SampleDescription"));
                    FindAndReplace(wordApp_BH, "{Buyer'sName}", GetSafeValue("Buyer'sName"));
                    FindAndReplace(wordApp_BH, "{CountryofOrigin}", GetSafeValue("CountryOrigin"));
                    FindAndReplace(wordApp_BH, "{BrandProduct}", GetSafeValue("BrandProduct"));
                    FindAndReplace(wordApp_BH, "{AgeGroup}", GetSafeValue("AgeGroup"));
                    FindAndReplace(wordApp_BH, "{Suppliername}", GetSafeValue("Suppliername"));
                    FindAndReplace(wordApp_BH, "{FactoryName}", GetSafeValue("FactoryName"));
                    FindAndReplace(wordApp_BH, "{FactoryAddress}", GetSafeValue("FactoryAddress"));
                    FindAndReplace(wordApp_BH, "{MaterialType}", GetSafeValue("MaterialType"));
                    FindAndReplace(wordApp_BH, "{ItemStyleNo}", GetSafeValue("ItemStyleNo"));
                    FindAndReplace(wordApp_BH, "{MaterialName_Code}", GetSafeValue("MaterialName_Code"));
                    FindAndReplace(wordApp_BH, "{ColorName_Code}", GetSafeValue("ColorName_Code"));
                    FindAndReplace(wordApp_BH, "{BatchNo}", GetSafeValue("BatchNo"));
                    FindAndReplace(wordApp_BH, "{WorkorderNo}", GetSafeValue("WorkorderNumber"));
                    FindAndReplace(wordApp_BH, "{TestCategory}", GetSafeValue("GroupofProduct"));
                    FindAndReplace(wordApp_BH, "{TestMethod}", GetSafeValue("TestMethod"));
                    FindAndReplace(wordApp_BH, "{ColorName}", GetSafeValue("Color"));
                    FindAndReplace(wordApp_BH, "{TestSpecification}", GetSafeValue("ProductSpecification"));
                    FindAndReplace(wordApp_BH, "{TestPackage}", GetSafeValue("GroupofProduct"));

                    string finalResult = GetSafeValue("FinalTestResult").Trim().ToUpper();
                    if (finalResult == "PASS")
                    {
                        InsertResultTableIntoDocument(wordApp, doc, "{FinalTestResult}", "PASS");
                        InsertResultTableIntoDocument_BH(wordApp_BH, doc_BH, "{FinalTestResult}", "LULUS");
                    }
                    else
                    {
                        InsertResultTableIntoDocument(wordApp, doc, "{FinalTestResult}", "FAIL");
                        InsertResultTableIntoDocument_BH(wordApp_BH, doc_BH, "{FinalTestResult}", "TIDAK LULUS");
                    }

                    ViewState["SampleDescription"] = GetSafeValue("SampleDescription");
                    FindAndReplace(wordApp, "{TestReportNo.}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp, "{DocNo}", "TSQ-LAB-QAF-089");

                    FindAndReplace(wordApp_BH, "{TestReportNo.}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp_BH, "{TestReportNo}", GetSafeValue("TestReportNo"));
                    ReplaceInHeaderFooter(wordApp_BH, "{DocNo}", "TSQ-LAB-QAF-087");
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

                    //if (ReportIDGet == 28)
                    //{
                    //    AdditionalDataAdded(wordApp, doc, "{AdditionalData}");
                    //    AdditionalDataAdded_BH(wordApp_BH, doc_BH, "{AdditionalData}");
                    //}

                    int counter = 0;
                    if (dt1.Rows.Count > 0)
                    {
                        string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] query_BH = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');

                        for (int i = 0; i < query.Length; i++)
                        {
                            counter = i;
                            if (table[i] == "10")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable10(wordApp, doc, "{ResultSummaryTable}", dt2);

                                string Query = @"SELECT ROW_NUMBER() OVER (ORDER BY tp.TestParameter_BH) AS [No.], tp.TestParameter_BH AS [Test parameters] FROM crfsampledetails c 
                                                 INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                                 INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                 INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                 INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                 GROUP BY tp.TestParameter_BH, t.sampleID having sampleID ='" + s + "' ";

                                dt2 = _cm.getReportQuery_BH(Query);
                                CreateTable10_BH(wordApp_BH, doc_BH, "{ResultSummaryTable}", dt2);
                            }

                            if (table[i] == "11")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable11(wordApp, doc, "{ResultTable1}", dt2);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                                  c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                  INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                  INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                  WHERE t.sampleID= '" + s + "' ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                CreateTable11_BH(wordApp_BH, doc_BH, "ResultTable1}", dt2);
                            }

                            if (table[i] == "12")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable12(wordApp, doc, "{TestParameter}", dt2);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter],tsp.TestSubParameter_BH AS [Test Item],TestMethod AS [Method*],
                                                 c.MDL as [M.D.L],TestResult AS [Result(s)],t.EquipmentName AS [Equipment] FROM crfsampledetails c
                                                 INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                 INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                 INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                 WHERE t.sampleID='" + s + "' ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                CreateTable12_BH(wordApp_BH, doc_BH, "{TestParameter}", dt2);
                            }

                            if (table[i] == "13")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);

                                // First list for regular document
                                List<Tuple<string, string, string>> imageData = new List<Tuple<string, string, string>>();
                                foreach (DataRow row in dt2.Rows)
                                {
                                    string imageFileName = row["SelectedImages"].ToString().Trim();
                                    string imageName = row["SampleText"].ToString().Trim();
                                    string sampleColourName = row["SampleColourName"].ToString().Trim();
                                    if (!string.IsNullOrEmpty(imageFileName))
                                    {
                                        string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                                        if (File.Exists(fullImagePath))
                                        {
                                            imageData.Add(Tuple.Create(fullImagePath, imageName, sampleColourName));
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Image not found: {fullImagePath}");
                                        }
                                    }
                                }
                                InsertImagesIntoDocument1(wordApp, doc, "{images}", imageData);

                                // Second list for BH document
                                List<Tuple<string, string, string>> imageData_BH = new List<Tuple<string, string, string>>();
                                foreach (DataRow row in dt2.Rows)
                                {
                                    string imageFileName = row["SelectedImages"].ToString().Trim();
                                    string imageName = row["SampleText"].ToString().Trim();
                                    string sampleColourName_BH = row["SampleColourName_BH"].ToString().Trim();
                                    if (!string.IsNullOrEmpty(imageFileName))
                                    {
                                        string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;
                                        if (File.Exists(fullImagePath))
                                        {
                                            imageData_BH.Add(Tuple.Create(fullImagePath, imageName, sampleColourName_BH));
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Image not found: {fullImagePath}");
                                        }
                                    }
                                }
                                InsertImagesIntoDocument1_BH(wordApp_BH, doc_BH, "{images}", imageData_BH);
                            }

                            if (table[i] == "14")
                            {
                                dt2 = _cm.getReportQuery(1, query[i]);
                                CreateTable14(wordApp, doc, "{Testingandmeasuringequipment}", dt2);

                                string Query = @"select Equipment ,Type,Calibration_Date as[Calibration Due Date] from TestResultDetails t 
                                                 INNER JOIN Calibration_Equipment c  on c.sampleid = t.SampleId ";
                                dt2 = _cm.getReportQuery_BH(Query);
                                CreateTable14_BH(wordApp_BH, doc_BH, "{Testingandmeasuringequipment}", dt2);
                            }

                            if (table[i] == "15")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable15(wordApp, doc, "{ResultTable1}", dt2);

                                string Query = @"SELECT tp.TestParameter_BH AS[Test Parameter], tsp.TestSubParameter_BH AS[Test Item], TestMethod AS[Method *],
                                                 c.MDL as [M.D.L],TestResult AS[Result(s)], t.EquipmentName AS[Equipment] FROM crfsampledetails c
                                                 INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                 INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                 INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND(c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '')
                                                 WHERE t.sampleID ='" + s + "' ";

                                dt2 = _cm.getReportQuery_BH(Query);
                                CreateTable15_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2);
                            }

                            if (table[i] == "16")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable16(wordApp, doc, "{ResultTable2}", dt2);

                                string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Test Item], tssp.TestSubSubParameter_BH AS [Compound], 
                                                TestMethod AS [Method*], c.MDL as [M.D.L],TestResult AS [Result(s)],
                                                t.EquipmentName AS [Equipment] FROM crfsampledetails c 
                                                INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                INNER JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid AND (c.TestSubSubParameter IS NOT NULL AND c.TestSubSubParameter != '') 
                                                WHERE t.sampleID ='" + s + "' ";

                                dt2 = _cm.getReportQuery_BH(Query);
                                CreateTable16_BH(wordApp_BH, doc_BH, "{ ResultTable2}", dt2);
                            }

                            if (table[i] == "17")
                            {
                                dt2 = _cm.GetResultOverallConclusion_ToysAndBabyWear(s);
                                CreateTable17(wordApp, doc, "{Overall Conclusion}", dt2);

                                dt2_BH = _cm.GetResultOverallConclusion_BH_ToysAndBabyWear(s);
                                CreateTable17_BH(wordApp, doc_BH, "{Overall Conclusion}", dt2_BH);
                            }

                            if (table[i] == "18")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable18(wordApp, doc, "{ResultTable1}", dt2);

                                dt2_BH = _cm.getReportQuery(1, query_BH[i] + s);
                                CreateTable18_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2_BH);

                                /* string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Element], c.CASNo AS [CAS Number], TestMethod AS [Method*], 
                                                  TestResult AS [Result(mg/kg)], t.Specification AS [Requirement], t.EquipmentName AS [Equipment], ccs.ColourCodeId AS [Colour Code], 
                                                  tsp.MDL, t.Conclusion_BH as Conclusion FROM crfsampledetails c 
                                                  INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                  INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                  INNER JOIN ColourCodeSave ccs ON ccs.AdditionalColourCodeId = t.AdditionalColourCodesId WHERE t.sampleID ='" + s + "' ";
                                 dt2_BH = _cm.getReportQuery_BH(Query);
                                 CreateTable18_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2_BH); */
                            }

                            if (table[i] == "19")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable19(wordApp, doc, "{ResultTable2}", dt2);

                                dt2_BH = _cm.getReportQuery(1, query_BH[i] + s);
                                CreateTable19_BH(wordApp_BH, doc_BH, "{ResultTable2}", dt2_BH);

                                /*string Query = @"SELECT tp.TestParameter_BH AS [Parameter Uji], tsp.TestSubParameter_BH AS [Barang Tes], 
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
                                CreateTable19_BH(wordApp_BH, doc_BH, "{ResultTable2}", dt2_BH);*/
                            }

                            if (table[i] == "20")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTableforSampleDescription(wordApp, doc, "{SampleDescriptionData}", dt2);

                                dt2_BH = _cm.getReportQuery(1, query_BH[i] + s);
                                CreateTableforSampleDescription_BH(wordApp_BH, doc_BH, "{SampleDescriptionData}", dt2_BH);
                            }

                            if (table[i] == "21")
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
                                InsertImagesIntoDocument21(wordApp, doc, "{images}", imageData);
                                InsertImagesIntoDocument21_BH(wordApp_BH, doc_BH, "{images}", imageData);
                            }

                            if (table[i] == "22")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable22(wordApp, doc, "{TestParameter}", dt2);

                                dt2_BH = _cm.getReportQuery(1, query_BH[i] + s);
                                CreateTable22_BH(wordApp_BH, doc_BH, "{TestParameter}", dt2_BH);

                                //string Query = @"select tp.TestParameter_BH AS [TestParameter], tsp.TestSubParameter_BH AS [TestSubParameter], 
                                //                   tssp.TestSubSubParameter_BH AS [TestSubSubParameter], c.unitofmeasurement AS [UOM], 
                                //                   TestMethod AS [Method*], TestResult AS [Result(mg/kg)], t.EquipmentName AS [Equipment], t.Conclusion  
                                //                  FROM CRFSampleDetails c
                                //                  INNER JOIN TestResults t ON t.CRFSampleID = c.CRFSampleId
                                //                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                //                  LEFT JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                //                  LEFT JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                //                  WHERE t.sampleID ='" + s + "' ";

                                /*string Query = @"select tp.TestParameter_BH AS [TestParameter], tsp.TestSubParameter_BH AS [TestSubParameter], tssp.TestSubSubParameter_BH AS [TestSubSubParameter], c.unitofmeasurement AS [UOM], TestResult AS [Result(mg/kg)], t.EquipmentName AS [Equipment], t.Conclusion,
                                                   CASE 
														WHEN tp.Protocol IS NOT NULL AND tp.Protocol != '' THEN tp.Protocol
														WHEN tsp.Protocol IS NOT NULL AND tsp.Protocol != '' THEN tsp.Protocol
														WHEN tssp.Protocol IS NOT NULL AND tssp.Protocol != '' THEN tssp.Protocol
														ELSE NULL
												   END as [Method*]												     
                                                  FROM CRFSampleDetails c
                                                  INNER JOIN TestResults t ON t.CRFSampleID = c.CRFSampleId
                                                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                  LEFT JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                  LEFT JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                  WHERE t.sampleID ='" + s + "' ";
                                dt2_BH = _cm.getReportQuery_BH(Query);
                                CreateTable22_BH(wordApp_BH, doc_BH, "{TestParameter}", dt2_BH);*/

                            }

                            if (table[i] == "23")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable23(wordApp, doc, "{ResultTable1}", dt2);

                                dt2_BH = _cm.getReportQuery(1, query_BH[i] + s);
                                CreateTable23_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2_BH);

                                /*string Query = @"SELECT tp.TestParameter_BH AS [Test Parameter], tsp.TestSubParameter_BH AS [Element],
                                                  c.CASNo AS [CAS Number], TestResult AS [Result(mg/kg)], t.Specification AS [Requirement],
                                                     CASE 
	                                                    WHEN tp.Protocol IS NOT NULL AND tp.Protocol != '' THEN tp.Protocol 
	                                                    WHEN tsp.Protocol IS NOT NULL AND tsp.Protocol != '' THEN tsp.Protocol 
                                                     ELSE NULL END as [Method*],
                                                   t.EquipmentName AS [Equipment], tsp.MDL, t.Conclusion_BH as Conclusion FROM crfsampledetails c 
                                                  INNER JOIN TestResults t ON c.crfsampleid = t.crfsampleid 
                                                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID
                                                  INNER JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID AND (c.TestSubSubParameter IS NULL OR c.TestSubSubParameter = '') 
                                                  WHERE t.sampleID ='" + s + "' ";
                                dt2_BH = _cm.getReportQuery_BH(Query);
                                CreateTable23_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2_BH);*/
                            }

                            if (table[i] == "24")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + InquiryId);
                                CreateTable24(wordApp, doc, "{ResultTable1}", dt2);

                                dt2_BH = _cm.getReportQuery(1, query_BH[i] + InquiryId);
                                CreateTable24_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2_BH);

                                /*
                                string Query_24 = @"SELECT tp.TestParameter, tsp.TestSubParameter, tssp.TestSubSubParameter, c.unitofmeasurement AS [UOM], 
                                                      c.TestMethod AS [Method*], t.TestResult AS [Result], t.EquipmentName AS [Equipment], 
                                                      'Specimen ' + CAST(ROW_NUMBER() OVER (PARTITION BY c.TestParameter, ISNULL(c.TestSubParameter, ''), 
                                                      c.unitofmeasurement ORDER BY t.TestResultID) AS VARCHAR) AS [Specimen] 
                                                     FROM CRFSampleDetails c 
                                                     INNER JOIN TestResults t ON t.CRFSampleID = c.CRFSampleId 
                                                     INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID 
                                                     LEFT JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                     LEFT JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                     WHERE c.InquiryId = '" + InquiryId + "' ";
                                dt2 = _cm.getReportQuery_NBM(Query_24);
                                CreateTable24(wordApp, doc, "{ResultTable1}", dt2);

                                string Query = @"SELECT tp.TestParameter_BH as [TestParameter], tsp.TestSubParameter_BH as [TestSubParameter], 
                                                    tssp.TestSubSubParameter_BH as [TestSubSubParameter], c.unitofmeasurement AS [UOM], 
                                                    c.TestMethod AS [Method*], t.TestResult AS [Result], t.EquipmentName AS [Equipment], 
                                                    'Specimen ' + CAST(ROW_NUMBER() OVER (PARTITION BY c.TestParameter, ISNULL(c.TestSubParameter, ''), 
                                                    c.unitofmeasurement ORDER BY t.TestResultID) AS VARCHAR) AS [Specimen] 
                                                  FROM CRFSampleDetails c 
                                                  INNER JOIN TestResults t ON t.CRFSampleID = c.CRFSampleId 
                                                  INNER JOIN TestParameter tp ON tp.TestParameterID = c.TestParameterID 
                                                  LEFT JOIN TestSubParameter tsp ON tsp.TestSubParameterID = c.TestSubParameterID
                                                  LEFT JOIN TestSubSubParameter tssp ON tssp.TestSubSubParameterID = c.TestSubSubParameterID
                                                  WHERE c.InquiryId = '" + InquiryId + "' ";
                                dt2_BH = _cm.getReportQuery_BH(Query);
                                CreateTable24_BH(wordApp_BH, doc_BH, "{ResultTable1}", dt2_BH);
                                */
                            }
                        }

                        try
                        {
                            if (counter == query.Length - 1)
                            {
                                string directory = Path.GetDirectoryName(outputPdfPath);
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                // Save documents
                                doc.SaveAs2(outputDocxPath);
                                doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);
                                // Save documents
                                doc_BH.SaveAs2(outputDocxPath_BH);
                                doc_BH.SaveAs2(outputPdfPath_BH, WdSaveFormat.wdFormatPDF);
                                /* Clean up COM objects */
                                Marshal.ReleaseComObject(doc);
                                doc = null;
                                Marshal.ReleaseComObject(doc_BH);
                                doc_BH = null;

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
                            if (doc_BH != null)
                            {
                                Marshal.ReleaseComObject(doc_BH);
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

                    if (doc_BH != null)
                    {
                        doc_BH.Close(false);
                        Marshal.ReleaseComObject(doc_BH);
                        doc_BH = null;
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

                    if (wordApp_BH != null)
                    {
                        wordApp_BH.Quit();
                        Marshal.ReleaseComObject(wordApp_BH);
                        wordApp_BH = null;
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

        private void InsertResultTableIntoDocument(Application wordApp, Document doc, string placeholder, string result)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute() && !string.IsNullOrEmpty(result))
                {
                    // Remove the placeholder text
                    range.Text = "";

                    // Create a 2 row x 1 column table
                    Table table = doc.Tables.Add(range, 2, 1);
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
                    Range passRange = table.Cell(1, 1).Range;
                    FormField passCheckbox = passRange.FormFields.Add(passRange, WdFieldType.wdFieldFormCheckBox);

                    // Set checkbox properties and left align
                    passCheckbox.CheckBox.Size = 14;
                    passRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Add space and text after checkbox
                    passRange.InsertAfter(" PASS");

                    // Format the text part
                    passRange.Font.Size = 14;
                    passRange.Font.TextColor.RGB = 0x800000;
                    passRange.Font.Bold = 1;

                    // Center content vertically in cell
                    table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Second row - FAIL
                    Range failRange = table.Cell(2, 1).Range;
                    FormField failCheckbox = failRange.FormFields.Add(failRange, WdFieldType.wdFieldFormCheckBox);

                    // Set checkbox properties and left align
                    failCheckbox.CheckBox.Size = 14;
                    failRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                    // Add space and text after checkbox
                    failRange.InsertAfter(" FAIL");

                    // Format the text part
                    failRange.Font.Size = 14;
                    failRange.Font.TextColor.RGB = 0x800000;
                    failRange.Font.Bold = 1;

                    // Center content vertically in cell
                    table.Cell(2, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Set checkbox based on parameter
                    string upperResult = result.Trim().ToUpper();
                    if (upperResult == "PASS")
                    {
                        passCheckbox.CheckBox.Value = true;
                    }
                    else if (upperResult == "FAIL")
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

        private void InsertResultTableIntoDocumentWork(Application wordApp, Document doc, string placeholder, string result)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute() && !string.IsNullOrEmpty(result))
                {
                    // Remove the placeholder text
                    range.Text = "";

                    // Create a 2 row x 1 column table
                    Table table = doc.Tables.Add(range, 2, 1);
                    table.Borders.Enable = 1; // Enable borders

                    // Set table width
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.PreferredWidth = 145f; // table width in points (~3.5 inches)

                    // Set row height
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightExactly;
                        row.Height = 25f; // row height in points
                    }

                    // First row - PASS
                    Range passRange = table.Cell(1, 1).Range;
                    FormField passCheckbox = passRange.FormFields.Add(passRange, WdFieldType.wdFieldFormCheckBox);

                    // Set checkbox properties
                    passCheckbox.CheckBox.Size = 14;
                    //passCheckbox.CheckBox.Color = 0x800000; // Maroon color for checkbox

                    // Add space and text after checkbox
                    passRange.InsertAfter(" PASS");

                    // Format the text part
                    passRange.Font.Size = 14;
                    passRange.Font.TextColor.RGB = 0x800000; // Maroon color for text
                    passRange.Font.Bold = 1;

                    // Second row - FAIL
                    Range failRange = table.Cell(2, 1).Range;
                    FormField failCheckbox = failRange.FormFields.Add(failRange, WdFieldType.wdFieldFormCheckBox);

                    // Set checkbox properties
                    failCheckbox.CheckBox.Size = 14;
                    //failCheckbox.CheckBox.Color = 0x800000; // Maroon color for checkbox

                    // Add space and text after checkbox
                    failRange.InsertAfter(" FAIL");

                    // Format the text part
                    failRange.Font.Size = 14;
                    failRange.Font.TextColor.RGB = 0x800000; // Maroon color for text
                    failRange.Font.Bold = 1;

                    // Set checkbox based on parameter
                    string upperResult = result.Trim().ToUpper();
                    if (upperResult == "PASS")
                    {
                        passCheckbox.CheckBox.Value = true;
                    }
                    else if (upperResult == "FAIL")
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

        public void CreatePassFailTable(string result)
        {
            object missing = System.Type.Missing;
            Application wordApp = new Application();
            wordApp.Visible = true;

            Document doc = wordApp.Documents.Add();

            // Create a 2x2 table
            Table table = doc.Tables.Add(doc.Range(0, 0), 2, 2, ref missing, ref missing);
            table.Borders.Enable = 1; // Enable borders

            // Set table width
            table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
            table.PreferredWidth = 145f; // table width in points (~3.5 inches)

            // Set row height
            foreach (Row row in table.Rows)
            {
                row.HeightRule = WdRowHeightRule.wdRowHeightExactly;
                row.Height = 25f; // row height in points
            }

            // Set column widths
            table.Columns[1].PreferredWidth = 25f;  // checkbox column
            table.Columns[2].PreferredWidth = 120f; // text column

            // First row - PASS
            Cell cell1 = table.Cell(1, 1);
            cell1.Range.FormFields.Add(cell1.Range, WdFieldType.wdFieldFormCheckBox);
            cell1.Range.Font.Size = 14;
            cell1.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            table.Cell(1, 2).Range.Text = "PASS";
            table.Cell(1, 2).Range.Font.Size = 14;
            table.Cell(1, 2).Range.Font.TextColor.RGB = 0x800000;
            table.Cell(1, 2).Range.Font.Bold = 1;

            // Second row - FAIL
            Cell cell2 = table.Cell(2, 1);
            cell2.Range.FormFields.Add(cell2.Range, WdFieldType.wdFieldFormCheckBox);
            cell2.Range.Font.Size = 14;
            cell2.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

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

        private void InsertImageAtPlaceholder(Application wordApp, Document doc, string placeholder, string imagePath)
        {
            try
            {
                Range range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    range.Text = "";

                    InlineShape picture = range.InlineShapes.AddPicture(
                        imagePath,
                        LinkToFile: false,
                        SaveWithDocument: true
                    );

                    Paragraph para = picture.Range.Paragraphs[1];
                    para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script>alert('Error inserting image: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        protected void GenerateQR(string SampleID)
        {
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string QRCodeFilePath = baseUrl + "/TestReport/TestReport-" + SampleID + ".pdf";
            //string QRCodeFilePath = "http://localhost:41414//TestReport/TestReport-" + SampleID + ".pdf";
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

        /* for single/multiple Images */
        private void InsertImagesIntoDocument1(Application wordApp, Document doc, string placeholder, List<Tuple<string, string, string>> imageData)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imageData != null && imageData.Count > 0)
                {
                    int totalImages = imageData.Count;
                    int rows = totalImages + 1; // +1 for header row

                    // Create table: 1 header row + 1 row per image, 2 columns
                    Table table = doc.Tables.Add(range, rows, 2);
                    table.Borders.Enable = 1;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.PreferredWidth = 100;

                    // Set column widths (adjust as needed)
                    table.Columns[1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.Columns[1].PreferredWidth = 50;
                    table.Columns[2].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.Columns[2].PreferredWidth = 50;

                    // Apply cell formatting
                    foreach (Cell cell in table.Range.Cells)
                    {
                        cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        cell.TopPadding = 10;
                        cell.BottomPadding = 10;
                        cell.LeftPadding = 10;
                        cell.RightPadding = 10;
                    }

                    // Create header row
                    Cell headerCell1 = table.Cell(1, 1);
                    headerCell1.Range.Text = "Description and Color";
                    headerCell1.Range.Font.Bold = 1;
                    headerCell1.Range.Font.Size = 12;
                    headerCell1.Range.Font.Name = "Arial";
                    headerCell1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell headerCell2 = table.Cell(1, 2);
                    headerCell2.Range.Text = "Photo\nPhotograph";
                    headerCell2.Range.Font.Bold = 1;
                    headerCell2.Range.Font.Size = 12;
                    headerCell2.Range.Font.Name = "Arial";
                    headerCell2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Fill data rows
                    for (int i = 0; i < totalImages; i++)
                    {
                        int rowIndex = i + 2; // Start from row 2 (after header)
                        string imagePath = imageData[i].Item1;
                        string sampleText = imageData[i].Item2;  // SampleText from database
                        string sampleColourName = imageData[i].Item3;  // SampleColourName from database

                        // Left column: Description
                        Cell descCell = table.Cell(rowIndex, 1);

                        // Build the text content
                        string descriptionText = "";
                        if (!string.IsNullOrEmpty(sampleText))
                        {
                            descriptionText += sampleText + "\n\n";
                        }
                        if (!string.IsNullOrEmpty(sampleColourName))
                        {
                            descriptionText += sampleColourName;
                        }

                        descCell.Range.Text = descriptionText;
                        descCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        descCell.Range.Font.Size = 14;
                        descCell.Range.Font.Name = "Arial";

                        // Right column: Photo
                        Cell photoCell = table.Cell(rowIndex, 2);
                        photoCell.Range.Text = "";

                        if (File.Exists(imagePath))
                        {
                            try
                            {
                                InlineShape shape = photoCell.Range.InlineShapes.AddPicture(
                                    FileName: imagePath,
                                    LinkToFile: false,
                                    SaveWithDocument: true
                                );

                                // Set image size
                                shape.Width = 150;
                                shape.Height = 150;

                                // Center the image
                                photoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                            catch (Exception ex)
                            {
                                photoCell.Range.Text = $"Error loading: {sampleText}\n{ex.Message}";
                                photoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                        }
                        else
                        {
                            photoCell.Range.Text = $"Missing: {sampleText}";
                            photoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
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

        /*******************************************************************| Toys & Baby Wear Report Start |********************************************************************/

        private int lastUsedNumber = 0;

        private void ReplaceTextInDocument(Document doc, string findText, string replaceText)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = findText;
            range.Find.Replacement.Text = replaceText;
            range.Find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        private void CreateTable10(Application wordApp, Document doc, string placeholder, DataTable dt)
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
        private void CreateTable11(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

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
        private void CreateTable12(Application wordApp, Document doc, string placeholder, DataTable dt)
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
        private void CreateTable14(Application wordApp, Document doc, string placeholder, DataTable dt)
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
        private void CreateTable15(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

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
        private void CreateTable16(Application wordApp, Document doc, string placeholder, DataTable dt)
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
        private void CreateTable17(Application wordApp, Document doc, string placeholder, DataTable dt)
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

        /* ChemResultTable_K3L_Without_TSSP [CreateTable18] */
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

                    // Group by Element only (removed CAS Number grouping)
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

                    // ONLY replace these for the FIRST group
                    if (isFirstGroup)
                    {
                        ReplaceTextInDocument(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());

                        // Clean method text - remove color codes if present
                        string methodText = firstRow["Method*"].ToString();
                        if (methodText.Contains("TSQ-LAB") || methodText.Contains(","))
                        {
                            // Extract just the method name before the color codes
                            int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                            if (colorCodeStart > 0)
                            {
                                methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                            }
                            else
                            {
                                // If TSQ-LAB not found, try to get text before first comma
                                int commaIndex = methodText.IndexOf(',');
                                if (commaIndex > 0)
                                {
                                    methodText = methodText.Substring(0, commaIndex).Trim();
                                }
                            }
                        }

                        ReplaceTextInDocument(doc, "{TestMethod}", methodText);
                        ReplaceTextInDocument(doc, "{Equipment}", firstRow["Equipment"].ToString());
                    }

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

                        // Clean method text - remove color codes if present
                        string methodText = firstRow["Method*"].ToString();
                        if (methodText.Contains("TSQ-LAB") || methodText.Contains(","))
                        {
                            // Extract just the method name before the color codes
                            int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                            if (colorCodeStart > 0)
                            {
                                methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                            }
                            else
                            {
                                // If TSQ-LAB not found, try to get text before first comma
                                int commaIndex = methodText.IndexOf(',');
                                if (commaIndex > 0)
                                {
                                    methodText = methodText.Substring(0, commaIndex).Trim();
                                }
                            }
                        }

                        currentRange.Text = $"Test Method: {methodText} - Analyzed by {firstRow["Equipment"]}\n";
                        currentRange.Font.Size = 11;
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
                        int resultSubCols = allColorCodeCombinations.Count;
                        int totalColumns = 2 + resultSubCols; // Changed from 3 + resultSubCols (removed CAS Number column)
                        bool hasManyColumns = resultSubCols > 4;

                        // Create table with extra row for Remark
                        Table table = doc.Tables.Add(currentRange, elementGroups.Count + 2, totalColumns);

                        // Set table properties
                        table.Borders.Enable = 1;
                        table.AllowAutoFit = true;
                        table.PreferredWidth = wordApp.CentimetersToPoints(17f);
                        table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                        table.AllowPageBreaks = true;

                        // Set column widths based on number of columns
                        if (!hasManyColumns)
                        {
                            // Original layout for 4 or fewer columns
                            table.Columns[1].PreferredWidth = wordApp.CentimetersToPoints(3.5f); // Element
                            for (int i = 0; i < resultSubCols; i++)
                            {
                                table.Columns[2 + i].PreferredWidth = wordApp.CentimetersToPoints(2.0f); // Result columns
                            }
                            table.Columns[2 + resultSubCols].PreferredWidth = wordApp.CentimetersToPoints(2.5f); // Requirement
                        }
                        else
                        {
                            // Adjusted layout for more than 4 columns
                            table.Columns[1].PreferredWidth = wordApp.CentimetersToPoints(3.0f); // Element (slightly smaller)

                            // Calculate available width for result columns
                            float availableWidth = 17f - 3.0f - 2.5f; // Total - Element - Requirement
                            float resultColumnWidth = availableWidth / resultSubCols;

                            // Ensure reasonable width for result columns
                            resultColumnWidth = Math.Max(1.5f, Math.Min(2.0f, resultColumnWidth));

                            for (int i = 0; i < resultSubCols; i++)
                            {
                                table.Columns[2 + i].PreferredWidth = wordApp.CentimetersToPoints(resultColumnWidth);
                            }
                            table.Columns[2 + resultSubCols].PreferredWidth = wordApp.CentimetersToPoints(2.5f); // Requirement
                        }

                        // Header row configuration - reduced height for many columns
                        table.Rows[1].Height = wordApp.CentimetersToPoints(hasManyColumns ? 1.2f : 1.5f);

                        // Set Element header
                        table.Cell(1, 1).Range.Text = "Element/Compound";
                        table.Cell(1, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(1, 1).Range.Font.Bold = 1;
                        table.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Result columns headers (start from column 2 now)
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            string codeCombination = allColorCodeCombinations[i];
                            Cell headerCell = table.Cell(1, 2 + i); // Changed from 3 + i to 2 + i

                            // Clear existing content
                            headerCell.Range.Text = "";

                            // Add "Result(mg/kg)" on the first line
                            headerCell.Range.Text = "Result\n (mg/kg)";
                            headerCell.Range.InsertParagraphAfter();

                            // Use appropriate wrapping strategy
                            string wrappedCombination = hasManyColumns ?
                                WrapTextForMultipleColumns(codeCombination, resultSubCols) :
                                WrapText(codeCombination, 14);

                            headerCell.Range.InsertAfter(wrappedCombination);

                            // Apply formatting
                            headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            headerCell.Range.Font.Bold = 1;
                            headerCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            headerCell.Range.ParagraphFormat.SpaceBefore = 0;
                            headerCell.Range.ParagraphFormat.SpaceAfter = 0;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                headerCell.Range.Font.Size = 8;
                            }
                        }

                        // Requirement column header
                        Cell requirementHeaderCell = table.Cell(1, 2 + resultSubCols); // Changed from 3 + resultSubCols to 2 + resultSubCols
                        requirementHeaderCell.Range.Text = "Requirement\n (mg/kg)";
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

                            // Set element cell
                            Cell elementCell = table.Cell(dataRowIndex, 1);
                            elementCell.Range.Text = elementGroup.Key; // Use the grouped key directly
                            elementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            elementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            elementCell.WordWrap = true;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                elementCell.Range.Font.Size = 9;
                            }

                            // Requirement cell
                            Cell requirementCell = table.Cell(dataRowIndex, 2 + resultSubCols); // Changed from 3 + resultSubCols to 2 + resultSubCols
                            string requirementText = firstElementRow["Requirement"].ToString();
                            requirementText = WrapRequirementText(requirementText);
                            requirementCell.Range.Text = requirementText;
                            requirementCell.WordWrap = true;
                            requirementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            requirementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                requirementCell.Range.Font.Size = 9;
                                requirementCell.Range.ParagraphFormat.LineSpacing = wordApp.LinesToPoints(0.9f);
                            }

                            // Initialize all result columns
                            for (int i = 0; i < resultSubCols; i++)
                            {
                                Cell resultCell = table.Cell(dataRowIndex, 2 + i); // Changed from 3 + i to 2 + i
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
                                        Cell resultCell = table.Cell(dataRowIndex, 2 + comboIndex); // Changed from 3 + comboIndex to 2 + comboIndex
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
                        table.Cell(remarkRow, 1).Range.Text = "Remark";
                        table.Cell(remarkRow, 1).Range.Font.Bold = 1;
                        table.Cell(remarkRow, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(remarkRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(remarkRow, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Remark result columns (start from column 2)
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            Cell remarkCell = table.Cell(remarkRow, 2 + i); // Changed from 3 + i to 2 + i
                            string remarkText = combinationHasDetection[i] ? "FAIL" : "PASS";
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
                        Cell remarkRequirementCell = table.Cell(remarkRow, 2 + resultSubCols); // Changed from 3 + resultSubCols to 2 + resultSubCols
                        remarkRequirementCell.Range.Text = "";
                        remarkRequirementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        remarkRequirementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Format all rows and cells with proper alignment
                        foreach (Row row in table.Rows)
                        {
                            row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                            row.Height = wordApp.CentimetersToPoints(hasManyColumns ? 1.2f : 1.0f);

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
                                    cell.WordWrap = true;
                                }
                                else if (cell.ColumnIndex >= 2 && cell.ColumnIndex < 2 + resultSubCols) // Result columns
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else if (cell.ColumnIndex == 2 + resultSubCols) // Requirement column
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                    cell.WordWrap = true;
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
                            string element = elementGroup.Key; // Use the grouped key
                            string requirement = firstElementRow["MDL"].ToString();

                            if (!string.IsNullOrWhiteSpace(requirement) && requirement != "Not Detected")
                            {
                                detectionLimits.Add($"{element}: {requirement} (mg/kg)");
                            }
                        }

                        if (detectionLimits.Count > 0)
                        {
                            currentRange.Text = "Method Detection Limit= " + string.Join(", ", detectionLimits);
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

        // FIXED WrapTextForMultipleColumns method
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

        /* ChemResultTable_K3L_With_TSSP [CreateTable19] */
        private void CreateTable19(Application wordApp, Document doc, string placeholder, DataTable dt)
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

                // Group the data by Test Item
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

                        // Set column headers with normalized spacing and two-line format
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string headerText = modifiedDt.Columns[col].ColumnName;

                            // Normalize all whitespace to single spaces
                            headerText = System.Text.RegularExpressions.Regex.Replace(headerText, @"\s+", " ").Trim();

                            // Special handling for Result columns to format in two lines
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
        private void CreateTableforSampleDescription(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc.Content;

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

        /* WHEN K3L GETTING THEN NEW REPORT ATTACH from the excel file */
        public void AdditionalDataAdded(Application wordApp, Document doc, string placeholder)
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

                Range wordRange = doc.Content;
                wordRange.Find.ClearFormatting();
                wordRange.Find.Text = placeholder;
                if (wordRange.Find.Execute())
                {
                    wordRange.Text = "";
                    wordRange.Paste();
                    workbook.Save();
                }

                Table wordTable = doc.Tables[doc.Tables.Count];

                // Set font size and center alignment for the entire table
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
                        Range tableRange = doc.Range(ref start, ref end);
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

        public void AdditionalDataAddedStatic(Application wordApp, Document doc, string placeholder)
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
                Range wordRange = doc.Content;
                wordRange.Find.ClearFormatting();
                wordRange.Find.Text = placeholder;

                if (wordRange.Find.Execute())
                {
                    // Clear the placeholder text
                    wordRange.Text = "";

                    // Create a range at the placeholder position for headers
                    Range headingRange = doc.Range(wordRange.Start, wordRange.Start);

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
                    Range pasteRange = doc.Range(headingRange.End, headingRange.End);
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
                                Range tableRange = doc.Range(ref start, ref end);
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


        /*******************************************************************| Toys & Baby Wear End |********************************************************************/



        /*******************************************************************| Toys & Baby Wear BH CODE START |**********************************************************/

        private void InsertImagesIntoDocument_BH(Application wordApp_BH, Document doc_BH, string placeholder, List<string> imagePaths)
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

        private void InsertResultTableIntoDocument_BH(Application wordApp_BH, Document doc_BH, string placeholder, string result)
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
                    Range passRange = table.Cell(1, 1).Range;
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
                    Range failRange = table.Cell(2, 1).Range;
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

        private void InsertImagesIntoDocument1_BH(Application wordApp_BH, Document doc_BH, string placeholder, List<Tuple<string, string, string>> imageData)
        {
            try
            {
                var range = doc_BH.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && imageData != null && imageData.Count > 0)
                {
                    int totalImages = imageData.Count;
                    int rows = totalImages + 1; // +1 for header row

                    // Create table: 1 header row + 1 row per image, 2 columns
                    Table table = doc_BH.Tables.Add(range, rows, 2);
                    table.Borders.Enable = 1;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.PreferredWidth = 100;

                    // Set column widths (adjust as needed)
                    table.Columns[1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.Columns[1].PreferredWidth = 50;
                    table.Columns[2].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    table.Columns[2].PreferredWidth = 50;

                    // Apply cell formatting
                    foreach (Cell cell in table.Range.Cells)
                    {
                        cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        cell.TopPadding = 10;
                        cell.BottomPadding = 10;
                        cell.LeftPadding = 10;
                        cell.RightPadding = 10;
                    }

                    // Create header row
                    Cell headerCell1 = table.Cell(1, 1);
                    headerCell1.Range.Text = "Description and Color";
                    headerCell1.Range.Font.Bold = 1;
                    headerCell1.Range.Font.Size = 12;
                    headerCell1.Range.Font.Name = "Arial";
                    headerCell1.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    Cell headerCell2 = table.Cell(1, 2);
                    headerCell2.Range.Text = "Photo\nPhotograph";
                    headerCell2.Range.Font.Bold = 1;
                    headerCell2.Range.Font.Size = 12;
                    headerCell2.Range.Font.Name = "Arial";
                    headerCell2.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                    // Fill data rows
                    for (int i = 0; i < totalImages; i++)
                    {
                        int rowIndex = i + 2; // Start from row 2 (after header)
                        string imagePath = imageData[i].Item1;
                        string sampleText = imageData[i].Item2;  // SampleText from database
                        string sampleColourName = imageData[i].Item3;  // SampleColourName_BH from database

                        // Left column: Description
                        Cell descCell = table.Cell(rowIndex, 1);

                        // Build the text content
                        string descriptionText = "";
                        if (!string.IsNullOrEmpty(sampleText))
                        {
                            descriptionText += sampleText + "\n\n";
                        }
                        if (!string.IsNullOrEmpty(sampleColourName))
                        {
                            descriptionText += sampleColourName;
                        }

                        descCell.Range.Text = descriptionText;
                        descCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        descCell.Range.Font.Size = 14;
                        descCell.Range.Font.Name = "Arial";

                        // Right column: Photo
                        Cell photoCell = table.Cell(rowIndex, 2);
                        photoCell.Range.Text = "";

                        if (File.Exists(imagePath))
                        {
                            try
                            {
                                InlineShape shape = photoCell.Range.InlineShapes.AddPicture(
                                    FileName: imagePath,
                                    LinkToFile: false,
                                    SaveWithDocument: true
                                );

                                // Set image size
                                shape.Width = 150;
                                shape.Height = 150;

                                // Center the image
                                photoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                            catch (Exception ex)
                            {
                                photoCell.Range.Text = $"Error loading: {sampleText}\n{ex.Message}";
                                photoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            }
                        }
                        else
                        {
                            photoCell.Range.Text = $"Missing: {sampleText}";
                            photoCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
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

        private void CreateTable17_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                ReplaceTextInDocument(doc_BH, "{Overall Conclusion}", "No data available");
                ReplaceTextInDocument(doc_BH, "{Requirement}", "No requirements available");
                return;
            }

            // First handle Requirements placeholder
            var requirements = dt.AsEnumerable().Select(row => row["Requirement"].ToString()).Where(req => !string.IsNullOrWhiteSpace(req)).Distinct().ToList();

            string combinedRequirements = string.Join(Environment.NewLine, requirements);
            ReplaceTextInDocument(doc_BH, "{Requirement}", combinedRequirements);

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

        private void CreateTableforSampleDescription_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc_BH.Content;

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

        private void CreateTable18_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
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

                    // Group by Element only (removed CAS Number grouping)
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
                    ReplaceTextInDocument(doc_BH, "{TestParameter}", firstRow["Test Parameter"].ToString());
                    ReplaceTextInDocument(doc_BH, "{TestMethod}", firstRow["Method*"].ToString());
                    ReplaceTextInDocument(doc_BH, "{Equipment}", firstRow["Equipment"].ToString());

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
                        int totalColumns = 2 + resultSubCols; // Changed from 3 + resultSubCols (removed CAS Number column)
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
                            table.Columns[1].PreferredWidth = wordApp_BH.CentimetersToPoints(3.5f); // Item Uji
                            for (int i = 0; i < resultSubCols; i++)
                            {
                                table.Columns[2 + i].PreferredWidth = wordApp_BH.CentimetersToPoints(2.0f); // Result columns
                            }
                            table.Columns[2 + resultSubCols].PreferredWidth = wordApp_BH.CentimetersToPoints(2.5f); // Persyaratan
                        }
                        else
                        {
                            // Adjusted layout for more than 4 columns
                            table.Columns[1].PreferredWidth = wordApp_BH.CentimetersToPoints(3.0f); // Item Uji (slightly smaller)

                            // Calculate available width for result columns
                            float availableWidth = 17f - 3.0f - 2.5f; // Total - Item Uji - Persyaratan
                            float resultColumnWidth = availableWidth / resultSubCols;

                            // Ensure reasonable width for result columns
                            resultColumnWidth = Math.Max(1.5f, Math.Min(2.0f, resultColumnWidth));

                            for (int i = 0; i < resultSubCols; i++)
                            {
                                table.Columns[2 + i].PreferredWidth = wordApp_BH.CentimetersToPoints(resultColumnWidth);
                            }
                            table.Columns[2 + resultSubCols].PreferredWidth = wordApp_BH.CentimetersToPoints(2.5f); // Persyaratan
                        }

                        // Header row configuration - reduced height for many columns
                        table.Rows[1].Height = wordApp_BH.CentimetersToPoints(hasManyColumns ? 1.2f : 1.5f);

                        // Set Item Uji header
                        table.Cell(1, 1).Range.Text = "Item Uji";
                        table.Cell(1, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(1, 1).Range.Font.Bold = 1;
                        table.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Result columns headers (start from column 2 now)
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            string codeCombination = allColorCodeCombinations[i];
                            Cell headerCell = table.Cell(1, 2 + i); // Changed from 3 + i to 2 + i

                            // Clear existing content
                            headerCell.Range.Text = "";

                            // Add "Hasil Uji (mg/kg)" on the first line
                            headerCell.Range.Text = "Hasil Uji\n (mg/kg)";
                            headerCell.Range.InsertParagraphAfter();

                            // Use appropriate wrapping strategy
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

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                headerCell.Range.Font.Size = 8;
                            }
                        }

                        // Persyaratan column header
                        Cell requirementHeaderCell = table.Cell(1, 2 + resultSubCols); // Changed from 3 + resultSubCols to 2 + resultSubCols
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

                            // Set Item Uji cell
                            Cell elementCell = table.Cell(dataRowIndex, 1);
                            elementCell.Range.Text = elementGroup.Key; // Use the grouped key directly
                            elementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            elementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            elementCell.WordWrap = true;

                            // Adjust font size for many columns
                            if (hasManyColumns)
                            {
                                elementCell.Range.Font.Size = 9;
                            }

                            // Persyaratan cell
                            Cell requirementCell = table.Cell(dataRowIndex, 2 + resultSubCols); // Changed from 3 + resultSubCols to 2 + resultSubCols
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
                                Cell resultCell = table.Cell(dataRowIndex, 2 + i); // Changed from 3 + i to 2 + i
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
                                        Cell resultCell = table.Cell(dataRowIndex, 2 + comboIndex); // Changed from 3 + comboIndex to 2 + comboIndex
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

                        // Keterangan row
                        int remarkRow = dataRowIndex;

                        // Keterangan label - use first cell only
                        table.Cell(remarkRow, 1).Range.Text = "Keterangan";
                        table.Cell(remarkRow, 1).Range.Font.Bold = 1;
                        table.Cell(remarkRow, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        table.Cell(remarkRow, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        table.Cell(remarkRow, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        // Keterangan result columns (start from column 2)
                        for (int i = 0; i < resultSubCols; i++)
                        {
                            Cell remarkCell = table.Cell(remarkRow, 2 + i); // Changed from 3 + i to 2 + i
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

                        // Keterangan persyaratan cell
                        Cell remarkRequirementCell = table.Cell(remarkRow, 2 + resultSubCols); // Changed from 3 + resultSubCols to 2 + resultSubCols
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
                                if (cell.ColumnIndex == 1) // Item Uji column
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                    cell.WordWrap = true;
                                }
                                else if (cell.ColumnIndex >= 2 && cell.ColumnIndex < 2 + resultSubCols) // Hasil Uji columns
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                }
                                else if (cell.ColumnIndex == 2 + resultSubCols) // Persyaratan column
                                {
                                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                    cell.WordWrap = true;
                                }
                            }
                        }

                        currentRange = table.Range;
                        currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        // Batas Deteksi Metode
                        var detectionLimits = new List<string>();
                        foreach (var elementGroup in elementGroups)
                        {
                            var firstElementRow = elementGroup.First();
                            string element = elementGroup.Key; // Use the grouped key
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

        private void CreateTable19_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    // No rows — replace all placeholders with blank
                    ReplaceTextInDocument(doc_BH, "{Number}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{TestParameter2}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{TestSubParameter}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{TestMethod}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{Equipment2}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{ResultTable2}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{AnalysisWasPerformed}", string.Empty);
                    ReplaceTextInDocument(doc_BH, "{:}", string.Empty);
                    return;
                }

                // Group the data by Barang Tes (Test Item)
                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Barang Tes"))
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
                        if (groupDt.Columns.Contains("Parameter Uji"))
                        {
                            // Replace {Number} with the actual number in the placeholder
                            string testParamValue = $"{lastUsedNumber}.    ";
                            ReplaceTextInDocument(doc_BH, "{Number}", testParamValue);

                            ReplaceTextInDocument(doc_BH, "{TestParameter2}", groupDt.Rows[0]["Parameter Uji"].ToString());
                            ReplaceTextInDocument(doc_BH, "{AnalysisWasPerformed}", "*) Analysis was performed ");
                            ReplaceTextInDocument(doc_BH, "{:}", " : ");
                        }
                        if (groupDt.Columns.Contains("Barang Tes"))
                            ReplaceTextInDocument(doc_BH, "{TestSubParameter}", groupDt.Rows[0]["Barang Tes"].ToString());
                        if (groupDt.Columns.Contains("Method*"))
                            ReplaceTextInDocument(doc_BH, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                        if (groupDt.Columns.Contains("Peralatan"))
                            ReplaceTextInDocument(doc_BH, "{Equipment2}", groupDt.Rows[0]["Peralatan"].ToString());
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

        public void AdditionalDataAdded_BH(Application wordApp_BH, Document doc_BH, string placeholder)
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

                Range wordRange = doc_BH.Content;
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
                        Range tableRange = doc_BH.Range(ref start, ref end);
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

        private void CreateTable10_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            var range = doc_BH.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                // Create a table in Word based on the DataTable
                Table table = doc_BH.Tables.Add(range, dt.Rows.Count + 1, dt.Columns.Count);

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

        private void CreateTable11_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Header translations
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

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            Range currentRange = null;
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
                        ReplaceTextInDocument(doc_BH, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc_BH, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc_BH, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable with Indonesian headers
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        // Use Indonesian name
                        string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(colName);
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
                        // Find original column name from Indonesian name
                        string originalColName = headers.FirstOrDefault(x => x.Value == column.ColumnName).Key ?? column.ColumnName;
                        string value = row[originalColName]?.ToString();
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

                // Remove columns that are all NULL or blank
                foreach (var column in columnsToRemove)
                {
                    modifiedDt.Columns.Remove(column);
                }

                // Copy data
                foreach (DataRow row in groupDt.Rows)
                {
                    DataRow newRow = modifiedDt.NewRow();
                    int idx = 0;
                    foreach (DataColumn col in groupDt.Columns)
                    {
                        if (col.ColumnName != "Test Parameter" &&
                            col.ColumnName != "Method*" &&
                            col.ColumnName != "Equipment")
                        {
                            // Find Indonesian column name
                            string indonesianName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                            if (modifiedDt.Columns.Contains(indonesianName))
                            {
                                newRow[indonesianName] = row[col.ColumnName];
                            }
                        }
                    }
                    modifiedDt.Rows.Add(newRow);
                }

                // Find or create position
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
                else
                {
                    if (currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        currentRange.Text = $"          Pra-Perlakuan berdasarkan {groupDt.Rows[0]["Method*"]}, analisis oleh {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    Table table = doc_BH.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable12_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Header translations
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

            // Group the data by Test Item
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Item"))
                .ToList();

            Range currentRange = null;
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
                    {
                        string testParamValue = $"{lastUsedNumber}.    ";
                        ReplaceTextInDocument(doc_BH, "{Number}", testParamValue);
                        ReplaceTextInDocument(doc_BH, "{TestParameter2}", groupDt.Rows[0]["Test Parameter"].ToString());
                    }
                    if (groupDt.Columns.Contains("Test Item"))
                        ReplaceTextInDocument(doc_BH, "{TestSubParameter}", groupDt.Rows[0]["Test Item"].ToString());
                    if (groupDt.Columns.Contains("Method*"))
                        ReplaceTextInDocument(doc_BH, "{TestMethod}", groupDt.Rows[0]["Method*"].ToString());
                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc_BH, "{Equipment2}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable with Indonesian headers
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" &&
                        col.ColumnName != "Test Item" &&
                        col.ColumnName != "Method*" &&
                        col.ColumnName != "Equipment")
                    {
                        // Use Indonesian name
                        string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(colName);
                    }
                }

                // Copy data
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

                // Find or create position
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
                else
                {
                    if (currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}:{groupDt.Rows[0]["Test Item"]} \n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        currentRange.Text = $"          Pra-Perlakuan berdasarkan {groupDt.Rows[0]["Method*"]}, analisis oleh {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    Table table = doc_BH.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable14_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            var range = doc_BH.Content;
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
                Table table = doc_BH.Tables.Add(range, newDt.Rows.Count + 1, newDt.Columns.Count);
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

        private void CreateTable15_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Header translations
            var headers = new Dictionary<string, string>
            {
                {"Test Item", "Item Uji"},
                {"Method*", "Metode*"},
                {"M.D.L", "M.D.L"},
                {"Result(s)", "Hasil"},
                {"Equipment", "Peralatan"}
            };

            // Group the data by Test Parameter
            var groupedData = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Test Parameter"))
                .ToList();

            Range currentRange = null;
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
                        ReplaceTextInDocument(doc_BH, "{TestParameter}", groupDt.Rows[0]["Test Parameter"].ToString());

                    if (groupDt.Columns.Contains("Equipment"))
                        ReplaceTextInDocument(doc_BH, "{Equipment}", groupDt.Rows[0]["Equipment"].ToString());
                }

                // Create modified DataTable
                DataTable modifiedDt = new DataTable();
                foreach (DataColumn col in groupDt.Columns)
                {
                    if (col.ColumnName != "Test Parameter" && col.ColumnName != "Equipment")
                    {
                        // Use Indonesian name
                        string colName = headers.ContainsKey(col.ColumnName) ? headers[col.ColumnName] : col.ColumnName;
                        modifiedDt.Columns.Add(colName);
                    }
                }

                // Copy data
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
                else
                {
                    if (currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        currentRange.Text = $"{lastUsedNumber}.      {groupDt.Rows[0]["Test Parameter"]}\n";
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        currentRange.Text = $"          *)Analisis dilakukan oleh {groupDt.Rows[0]["Equipment"]}\n";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }
                }

                if (currentRange != null)
                {
                    Table table = doc_BH.Tables.Add(currentRange, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = 100;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                    // Set headers
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        float columnWidth = 100f / modifiedDt.Columns.Count;
                        table.Columns[col + 1].PreferredWidth = columnWidth;
                        table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                    }

                    // Fill data
                    for (int row = 0; row < modifiedDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < modifiedDt.Columns.Count; col++)
                        {
                            string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                        }
                    }

                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.AllowPageBreaks = true;
                    foreach (Row row in table.Rows)
                    {
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = 20;
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                isFirstGroup = false;
            }
        }

        private void CreateTable16_BH(Application wordApp_BH, Document doc_BH, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // Header translations
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
            // Group the data by Test Item


        }



        /*******************************************************************| Toys & Baby Wear BH CODE END |**********************************************************/

        /**************************************| Fabric Report |**************************************/
        private void InsertImagesIntoDocument21(Application wordApp, Document doc, string placeholder, List<Tuple<string, string>> imageData)
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

        private void InsertImagesIntoDocument21_BH(Application wordApp_BH, Document doc_BH, string placeholder, List<Tuple<string, string>> imageData)
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




        private void CreateTable22(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                Range currentRange = null;

                // Find placeholder
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();
                if (range.Find.Found)
                {
                    currentRange = range;
                }

                if (currentRange != null)
                {
                    // Group by Test Parameter
                    var groupedByTestParam = dt.AsEnumerable()
                        .GroupBy(row => row.Field<string>("TestParameter"))
                        .ToList();

                    // Build row structure
                    var tableRows = new List<TableRowInfo>();
                    int testParamNumber = 1;

                    foreach (var testParamGroup in groupedByTestParam)
                    {
                        var testParameter = testParamGroup.Key;

                        // First, add the main test parameter row
                        var firstRow = testParamGroup.First();
                        tableRows.Add(new TableRowInfo
                        {
                            RowNumber = testParamNumber,
                            TestParameter = testParameter,
                            SubParameter = "",
                            SubSubParameter = "",
                            DisplayValue = testParameter,
                            DataRow = firstRow,
                            ShowRowNumber = true,
                            Level = 1 // Main parameter level
                        });

                        // Get all distinct sub-parameters and sub-sub-parameters
                        var subParameterItems = new List<SubParameterInfo>();

                        foreach (var row in testParamGroup)
                        {
                            string subParam = row.Field<string>("TestSubParameter");
                            string subSubParam = row.Field<string>("TestSubSubParameter");

                            // Skip if both are null/empty
                            if (string.IsNullOrWhiteSpace(subParam) && string.IsNullOrWhiteSpace(subSubParam))
                                continue;

                            subParameterItems.Add(new SubParameterInfo
                            {
                                SubParameter = subParam ?? "",
                                SubSubParameter = subSubParam ?? "",
                                DataRow = row
                            });
                        }

                        // Group sub-parameters to avoid duplicates
                        var groupedSubParams = subParameterItems
                            .GroupBy(x => new { x.SubParameter, x.SubSubParameter })
                            .Select(g => g.First())
                            .OrderBy(x => x.SubParameter)
                            .ThenBy(x => x.SubSubParameter)
                            .ToList();

                        // Add sub-parameter rows in correct hierarchy
                        foreach (var item in groupedSubParams)
                        {
                            // If this is a sub-sub-parameter (has value in SubSubParameter)
                            if (!string.IsNullOrWhiteSpace(item.SubSubParameter))
                            {
                                // First, check if we need to add the parent sub-parameter
                                if (!string.IsNullOrWhiteSpace(item.SubParameter))
                                {
                                    // Check if parent sub-parameter already added
                                    bool parentExists = tableRows.Any(r =>
                                        r.Level == 2 &&
                                        r.SubParameter == item.SubParameter &&
                                        r.RowNumber == testParamNumber);

                                    if (!parentExists)
                                    {
                                        // Find a row with this sub-parameter to get its data
                                        var parentRow = testParamGroup.FirstOrDefault(r =>
                                            r.Field<string>("TestSubParameter") == item.SubParameter);

                                        if (parentRow != null)
                                        {
                                            tableRows.Add(new TableRowInfo
                                            {
                                                RowNumber = testParamNumber,
                                                TestParameter = testParameter,
                                                SubParameter = item.SubParameter,
                                                SubSubParameter = "",
                                                DisplayValue = item.SubParameter,
                                                DataRow = parentRow,
                                                ShowRowNumber = false,
                                                Level = 2 // Sub-parameter level
                                            });
                                        }
                                    }
                                }

                                // Add sub-sub-parameter
                                tableRows.Add(new TableRowInfo
                                {
                                    RowNumber = testParamNumber,
                                    TestParameter = testParameter,
                                    SubParameter = item.SubParameter,
                                    SubSubParameter = item.SubSubParameter,
                                    DisplayValue = item.SubSubParameter,
                                    DataRow = item.DataRow,
                                    ShowRowNumber = false,
                                    Level = 3 // Sub-sub-parameter level
                                });
                            }
                            // If this is just a sub-parameter (no sub-sub-parameter)
                            else if (!string.IsNullOrWhiteSpace(item.SubParameter))
                            {
                                // Check if this sub-parameter already added
                                bool exists = tableRows.Any(r =>
                                    r.Level == 2 &&
                                    r.SubParameter == item.SubParameter &&
                                    r.RowNumber == testParamNumber);

                                if (!exists)
                                {
                                    tableRows.Add(new TableRowInfo
                                    {
                                        RowNumber = testParamNumber,
                                        TestParameter = testParameter,
                                        SubParameter = item.SubParameter,
                                        SubSubParameter = "",
                                        DisplayValue = item.SubParameter,
                                        DataRow = item.DataRow,
                                        ShowRowNumber = false,
                                        Level = 2 // Sub-parameter level
                                    });
                                }
                            }
                        }

                        testParamNumber++;
                    }

                    // Create table
                    int totalRows = tableRows.Count + 1; // +1 for header
                    Table table = doc.Tables.Add(currentRange, totalRows, 5);

                    // Set table properties for A4 page
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = false;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.PreferredWidth = wordApp.CentimetersToPoints(17f);
                    table.AllowPageBreaks = true;

                    // Set column widths
                    table.Columns[1].SetWidth(wordApp.CentimetersToPoints(1.0f), WdRulerStyle.wdAdjustNone);   // No
                    table.Columns[2].SetWidth(wordApp.CentimetersToPoints(6.8f), WdRulerStyle.wdAdjustNone);   // Test Type
                    table.Columns[3].SetWidth(wordApp.CentimetersToPoints(2.7f), WdRulerStyle.wdAdjustNone);   // Unit
                    table.Columns[4].SetWidth(wordApp.CentimetersToPoints(4.0f), WdRulerStyle.wdAdjustNone);   // Test Method
                    table.Columns[5].SetWidth(wordApp.CentimetersToPoints(2.5f), WdRulerStyle.wdAdjustNone);   // Test Results

                    // Header row
                    string[] headers = { "#", "Test Type", "Unit", "Test Method", "Test Results" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        Cell headerCell = table.Cell(1, i + 1);
                        headerCell.Range.Text = headers[i];
                        headerCell.Range.Font.Bold = 1;
                        headerCell.Range.Font.Size = 11;
                        headerCell.Range.Font.Name = "Calibri";
                        headerCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }

                    // Fill data rows
                    for (int i = 0; i < tableRows.Count; i++)
                    {
                        var rowInfo = tableRows[i];
                        int tableRowIndex = i + 2; // +2 because row 1 is header

                        // No column
                        Cell noCell = table.Cell(tableRowIndex, 1);
                        noCell.Range.Text = rowInfo.ShowRowNumber ? rowInfo.RowNumber.ToString() + "." : "";
                        noCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        noCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        noCell.Range.Font.Size = 10;
                        noCell.Range.Font.Name = "Calibri";

                        // Test Type column - build text based on hierarchy level
                        Cell testTypeCell = table.Cell(tableRowIndex, 2);
                        string testTypeText = "";

                        switch (rowInfo.Level)
                        {
                            case 1: // Main test parameter
                                testTypeText = rowInfo.TestParameter;
                                testTypeCell.Range.Text = testTypeText;
                                testTypeCell.Range.Font.Bold = 1;
                                break;

                            case 2: // Sub-parameter (•)
                                testTypeText = "  • " + rowInfo.SubParameter;
                                testTypeCell.Range.Text = testTypeText;
                                testTypeCell.Range.Font.Bold = 0;
                                break;

                            case 3: // Sub-sub-parameter (-)
                                testTypeText = "    - " + rowInfo.SubSubParameter;
                                testTypeCell.Range.Text = testTypeText;
                                testTypeCell.Range.Font.Bold = 0;
                                break;
                        }

                        testTypeCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        testTypeCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        testTypeCell.Range.Font.Size = 10;
                        testTypeCell.Range.Font.Name = "Calibri";
                        testTypeCell.WordWrap = true;

                        // Unit column
                        Cell unitCell = table.Cell(tableRowIndex, 3);
                        unitCell.Range.Text = rowInfo.DataRow["UOM"]?.ToString() ?? "";
                        unitCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        unitCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        unitCell.Range.Font.Size = 10;
                        unitCell.Range.Font.Name = "Calibri";

                        // Test Method column
                        Cell methodCell = table.Cell(tableRowIndex, 4);
                        methodCell.Range.Text = rowInfo.DataRow["Method*"]?.ToString() ?? "";
                        methodCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        methodCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        methodCell.Range.Font.Size = 10;
                        methodCell.Range.Font.Name = "Calibri";
                        methodCell.WordWrap = true;

                        // Test Results column
                        Cell resultCell = table.Cell(tableRowIndex, 5);
                        resultCell.Range.Text = FormatTestResult(rowInfo.DataRow["Result(mg/kg)"]?.ToString() ?? "");
                        resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        resultCell.Range.Font.Size = 10;
                        resultCell.Range.Font.Name = "Calibri";
                        resultCell.WordWrap = true;
                    }

                    // Format all rows
                    for (int r = 1; r <= table.Rows.Count; r++)
                    {
                        Row row = table.Rows[r];
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = wordApp.CentimetersToPoints(r == 1 ? 1.0f : 0.7f);

                        for (int c = 1; c <= 5; c++)
                        {
                            Cell cell = table.Cell(r, c);
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                        }
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    currentRange.InsertParagraphAfter();
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error: {ex.Message.Replace("'", "\\'")}');</script>");
            }
        }

        private string FormatTestResult(string result)
        {
            if (string.IsNullOrEmpty(result))
                return "";

            // Handle format like "254 (416)" from PNG
            if (result.Contains("(") && result.Contains(")"))
            {
                return result;
            }

            // Handle space-separated values
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
            public int Level { get; set; } // 1=Main, 2=SubParam (•), 3=SubSubParam (-)
        }

        private class SubParameterInfo
        {
            public string SubParameter { get; set; }
            public string SubSubParameter { get; set; }
            public DataRow DataRow { get; set; }
        }



        private void CreateTable22_BH(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;

                Range currentRange = null;

                // Find placeholder
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();
                if (range.Find.Found)
                {
                    currentRange = range;
                }

                if (currentRange != null)
                {
                    // Group by Test Parameter (BH)
                    var groupedByTestParam = dt.AsEnumerable()
                        .GroupBy(row => row.Field<string>("TestParameter"))
                        .ToList();

                    // Build row structure
                    var tableRows = new List<TableRowInfo_BH>();
                    int testParamNumber = 1;

                    foreach (var testParamGroup in groupedByTestParam)
                    {
                        var testParameter = testParamGroup.Key;

                        // First, add the main test parameter row
                        var firstRow = testParamGroup.First();
                        tableRows.Add(new TableRowInfo_BH
                        {
                            RowNumber = testParamNumber,
                            TestParameter = testParameter,
                            SubParameter = "",
                            SubSubParameter = "",
                            DisplayValue = testParameter,
                            DataRow = firstRow,
                            ShowRowNumber = true,
                            Level = 1 // Main parameter level
                        });

                        // Get all distinct sub-parameters and sub-sub-parameters
                        var subParameterItems = new List<SubParameterInfo_BH>();

                        foreach (var row in testParamGroup)
                        {
                            string subParam = row.Field<string>("TestSubParameter");
                            string subSubParam = row.Field<string>("TestSubSubParameter");

                            // Skip if both are null/empty
                            if (string.IsNullOrWhiteSpace(subParam) && string.IsNullOrWhiteSpace(subSubParam))
                                continue;

                            subParameterItems.Add(new SubParameterInfo_BH
                            {
                                SubParameter = subParam ?? "",
                                SubSubParameter = subSubParam ?? "",
                                DataRow = row
                            });
                        }

                        // Group sub-parameters to avoid duplicates
                        var groupedSubParams = subParameterItems
                            .GroupBy(x => new { x.SubParameter, x.SubSubParameter })
                            .Select(g => g.First())
                            .OrderBy(x => x.SubParameter)
                            .ThenBy(x => x.SubSubParameter)
                            .ToList();

                        // Add sub-parameter rows in correct hierarchy
                        foreach (var item in groupedSubParams)
                        {
                            // If this is a sub-sub-parameter (has value in SubSubParameter)
                            if (!string.IsNullOrWhiteSpace(item.SubSubParameter))
                            {
                                // First, check if we need to add the parent sub-parameter
                                if (!string.IsNullOrWhiteSpace(item.SubParameter))
                                {
                                    // Check if parent sub-parameter already added
                                    bool parentExists = tableRows.Any(r =>
                                        r.Level == 2 &&
                                        r.SubParameter == item.SubParameter &&
                                        r.RowNumber == testParamNumber);

                                    if (!parentExists)
                                    {
                                        // Find a row with this sub-parameter to get its data
                                        var parentRow = testParamGroup.FirstOrDefault(r =>
                                            r.Field<string>("TestSubParameter") == item.SubParameter);

                                        if (parentRow != null)
                                        {
                                            tableRows.Add(new TableRowInfo_BH
                                            {
                                                RowNumber = testParamNumber,
                                                TestParameter = testParameter,
                                                SubParameter = item.SubParameter,
                                                SubSubParameter = "",
                                                DisplayValue = item.SubParameter,
                                                DataRow = parentRow,
                                                ShowRowNumber = false,
                                                Level = 2 // Sub-parameter level
                                            });
                                        }
                                    }
                                }

                                // Add sub-sub-parameter
                                tableRows.Add(new TableRowInfo_BH
                                {
                                    RowNumber = testParamNumber,
                                    TestParameter = testParameter,
                                    SubParameter = item.SubParameter,
                                    SubSubParameter = item.SubSubParameter,
                                    DisplayValue = item.SubSubParameter,
                                    DataRow = item.DataRow,
                                    ShowRowNumber = false,
                                    Level = 3 // Sub-sub-parameter level
                                });
                            }
                            // If this is just a sub-parameter (no sub-sub-parameter)
                            else if (!string.IsNullOrWhiteSpace(item.SubParameter))
                            {
                                // Check if this sub-parameter already added
                                bool exists = tableRows.Any(r =>
                                    r.Level == 2 &&
                                    r.SubParameter == item.SubParameter &&
                                    r.RowNumber == testParamNumber);

                                if (!exists)
                                {
                                    tableRows.Add(new TableRowInfo_BH
                                    {
                                        RowNumber = testParamNumber,
                                        TestParameter = testParameter,
                                        SubParameter = item.SubParameter,
                                        SubSubParameter = "",
                                        DisplayValue = item.SubParameter,
                                        DataRow = item.DataRow,
                                        ShowRowNumber = false,
                                        Level = 2 // Sub-parameter level
                                    });
                                }
                            }
                        }

                        testParamNumber++;
                    }

                    // Create table
                    int totalRows = tableRows.Count + 1; // +1 for header
                    Table table = doc.Tables.Add(currentRange, totalRows, 5);

                    // Set table properties for A4 page
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = false;
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.PreferredWidth = wordApp.CentimetersToPoints(17f);
                    table.AllowPageBreaks = true;

                    // Set column widths - BH reports might need slightly different widths
                    table.Columns[1].SetWidth(wordApp.CentimetersToPoints(1.0f), WdRulerStyle.wdAdjustNone);   // No
                    table.Columns[2].SetWidth(wordApp.CentimetersToPoints(6.8f), WdRulerStyle.wdAdjustNone);   // Test Type
                    table.Columns[3].SetWidth(wordApp.CentimetersToPoints(2.7f), WdRulerStyle.wdAdjustNone);   // Unit
                    table.Columns[4].SetWidth(wordApp.CentimetersToPoints(4.0f), WdRulerStyle.wdAdjustNone);   // Test Method
                    table.Columns[5].SetWidth(wordApp.CentimetersToPoints(2.5f), WdRulerStyle.wdAdjustNone);   // Test Results

                    // Header row
                    string[] headers = { "#", "Jenis Tes", "Satuan", "Metode Tes", "Hasil Tes" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        Cell headerCell = table.Cell(1, i + 1);
                        headerCell.Range.Text = headers[i];
                        headerCell.Range.Font.Bold = 1;
                        headerCell.Range.Font.Size = 11;
                        headerCell.Range.Font.Name = "Calibri";
                        headerCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    }

                    // Fill data rows
                    for (int i = 0; i < tableRows.Count; i++)
                    {
                        var rowInfo = tableRows[i];
                        int tableRowIndex = i + 2; // +2 because row 1 is header

                        // No column
                        Cell noCell = table.Cell(tableRowIndex, 1);
                        noCell.Range.Text = rowInfo.ShowRowNumber ? rowInfo.RowNumber.ToString() + "." : "";
                        noCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        noCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        noCell.Range.Font.Size = 10;
                        noCell.Range.Font.Name = "Calibri";

                        // Test Type column - build text based on hierarchy level
                        Cell testTypeCell = table.Cell(tableRowIndex, 2);
                        string testTypeText = "";

                        switch (rowInfo.Level)
                        {
                            case 1: // Main test parameter
                                testTypeText = rowInfo.TestParameter;
                                testTypeCell.Range.Text = testTypeText;
                                testTypeCell.Range.Font.Bold = 1;
                                break;

                            case 2: // Sub-parameter (•)
                                testTypeText = "  • " + rowInfo.SubParameter;
                                testTypeCell.Range.Text = testTypeText;
                                testTypeCell.Range.Font.Bold = 0;
                                break;

                            case 3: // Sub-sub-parameter (-)
                                testTypeText = "    - " + rowInfo.SubSubParameter;
                                testTypeCell.Range.Text = testTypeText;
                                testTypeCell.Range.Font.Bold = 0;
                                break;
                        }

                        testTypeCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        testTypeCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        testTypeCell.Range.Font.Size = 10;
                        testTypeCell.Range.Font.Name = "Calibri";
                        testTypeCell.WordWrap = true;

                        // Unit column
                        Cell unitCell = table.Cell(tableRowIndex, 3);
                        unitCell.Range.Text = rowInfo.DataRow["UOM"]?.ToString() ?? "";
                        unitCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        unitCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        unitCell.Range.Font.Size = 10;
                        unitCell.Range.Font.Name = "Calibri";

                        // Test Method column
                        Cell methodCell = table.Cell(tableRowIndex, 4);
                        methodCell.Range.Text = rowInfo.DataRow["Method*"]?.ToString() ?? "";
                        methodCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        methodCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        methodCell.Range.Font.Size = 10;
                        methodCell.Range.Font.Name = "Calibri";
                        methodCell.WordWrap = true;

                        // Test Results column
                        Cell resultCell = table.Cell(tableRowIndex, 5);
                        resultCell.Range.Text = FormatTestResult_BH(rowInfo.DataRow["Result(mg/kg)"]?.ToString() ?? "");
                        resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        resultCell.Range.Font.Size = 10;
                        resultCell.Range.Font.Name = "Calibri";
                        resultCell.WordWrap = true;
                    }

                    // Format all rows
                    for (int r = 1; r <= table.Rows.Count; r++)
                    {
                        Row row = table.Rows[r];
                        row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        row.Height = wordApp.CentimetersToPoints(r == 1 ? 1.0f : 0.7f);

                        for (int c = 1; c <= 5; c++)
                        {
                            Cell cell = table.Cell(r, c);
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                        }
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    currentRange.InsertParagraphAfter();
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error in CreateTable22_BH: {ex.Message.Replace("'", "\\'")}');</script>");
            }
        }

        private string FormatTestResult_BH(string result)
        {
            if (string.IsNullOrEmpty(result))
                return "";

            // Handle format like "254 (416)" from PNG
            if (result.Contains("(") && result.Contains(")"))
            {
                return result;
            }

            // Handle space-separated values
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

        private class TableRowInfo_BH
        {
            public int RowNumber { get; set; }
            public string TestParameter { get; set; }
            public string SubParameter { get; set; }
            public string SubSubParameter { get; set; }
            public string DisplayValue { get; set; }
            public DataRow DataRow { get; set; }
            public bool ShowRowNumber { get; set; }
            public int Level { get; set; } // 1=Main, 2=SubParam (•), 3=SubSubParam (-)
        }

        private class SubParameterInfo_BH
        {
            public string SubParameter { get; set; }
            public string SubSubParameter { get; set; }
            public DataRow DataRow { get; set; }
        }


        /****************************************| Quality Test Report |*****************************************/

        private void CreateTable23(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script>alert('DataTable is null or empty');</script>");
                    return;
                }

                var groupedData = dt.AsEnumerable().GroupBy(row => row.Field<string>("Test Parameter")).ToList();
                Range currentRange = null;
                bool isFirstGroup = true;
                int tableNumber = 0; // LOCAL counter - starts from 0

                foreach (var testParamGroup in groupedData)
                {
                    tableNumber++; // Increment local counter: 1, 2, 3...

                    var firstRow = testParamGroup.First();

                    if (isFirstGroup)
                    {
                        ReplaceTextInDocument(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());

                        string methodText = firstRow["Method*"].ToString().Trim();

                        if (methodText.Contains("TSQ-LAB"))
                        {
                            int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                            if (colorCodeStart > 0)
                            {
                                methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                            }
                        }

                        ReplaceTextInDocument(doc, "{TestMethod}", methodText);
                        ReplaceTextInDocument(doc, "{Equipment}", firstRow["Equipment"].ToString());

                        var range = doc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Text = placeholder;
                        if (range.Find.Execute())
                        {
                            currentRange = range;
                        }
                        else
                        {
                            currentRange = doc.Content;
                            currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                        }
                    }

                    if (!isFirstGroup && currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.Text = $"{tableNumber}.      {firstRow["Test Parameter"]}\n"; // Use local counter
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        string methodText = firstRow["Method*"].ToString().Trim();

                        if (methodText.Contains("TSQ-LAB"))
                        {
                            int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                            if (colorCodeStart > 0)
                            {
                                methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                            }
                        }

                        currentRange.Text = $"Test Method: {methodText} - Analyzed by {firstRow["Equipment"]}\n";
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    if (currentRange == null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                            "<script>alert('Cannot find insertion point for table');</script>");
                        return;
                    }

                    int dataRowCount = testParamGroup.Count();
                    int totalColumns = 3;

                    Table table = doc.Tables.Add(currentRange, dataRowCount + 1, totalColumns);

                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = wordApp.CentimetersToPoints(17f);
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.AllowPageBreaks = true;

                    table.Columns[1].PreferredWidth = wordApp.CentimetersToPoints(7.0f);
                    table.Columns[2].PreferredWidth = wordApp.CentimetersToPoints(5.0f);
                    table.Columns[3].PreferredWidth = wordApp.CentimetersToPoints(5.0f);

                    table.Rows[1].Height = wordApp.CentimetersToPoints(1.5f);

                    table.Cell(1, 1).Range.Text = "Element/Compound";
                    table.Cell(1, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    table.Cell(1, 1).Range.Font.Bold = 1;
                    table.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    Cell resultHeaderCell = table.Cell(1, 2);
                    resultHeaderCell.Range.Text = "Result\n(mg/kg)";
                    resultHeaderCell.Range.Font.Bold = 1;
                    resultHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    resultHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    resultHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    resultHeaderCell.WordWrap = true;

                    Cell specHeaderCell = table.Cell(1, 3);
                    specHeaderCell.Range.Text = "Specification\n(mg/kg)";
                    specHeaderCell.Range.Font.Bold = 1;
                    specHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    specHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    specHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    specHeaderCell.WordWrap = true;

                    int dataRowIndex = 2;
                    bool hasDetection = false;

                    foreach (var row in testParamGroup)
                    {
                        Cell elementCell = table.Cell(dataRowIndex, 1);
                        string elementValue = row["Element"]?.ToString() ?? "";
                        elementCell.Range.Text = elementValue;
                        elementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        elementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        elementCell.WordWrap = true;

                        Cell resultCell = table.Cell(dataRowIndex, 2);
                        string result = row["Result(mg/kg)"]?.ToString() ?? "N.D";
                        resultCell.Range.Text = result;
                        resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        if (result.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                            result.Equals("Detected", StringComparison.OrdinalIgnoreCase))
                        {
                            hasDetection = true;
                        }

                        Cell specCell = table.Cell(dataRowIndex, 3);
                        string requirementText = row["Requirement"]?.ToString() ?? "";
                        requirementText = WrapRequirementText_23(requirementText);
                        specCell.Range.Text = requirementText;
                        specCell.WordWrap = true;
                        specCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        specCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        dataRowIndex++;
                    }

                    foreach (Row tblRow in table.Rows)
                    {
                        tblRow.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        tblRow.Height = wordApp.CentimetersToPoints(1.0f);

                        foreach (Cell cell in tblRow.Cells)
                        {
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.Font.Size = 10;

                            if (cell.ColumnIndex == 1)
                            {
                                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                cell.WordWrap = true;
                            }
                            else
                            {
                                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                cell.WordWrap = true;
                            }
                        }
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    currentRange.InsertParagraphAfter();
                    currentRange = currentRange.Paragraphs.Last.Range;

                    var detectionLimits = new List<string>();
                    var processedElements = new HashSet<string>();

                    foreach (var row in testParamGroup)
                    {
                        string element = row["Element"]?.ToString() ?? "";
                        string mdl = row["MDL"]?.ToString() ?? "";

                        if (!processedElements.Contains(element) &&
                            !string.IsNullOrWhiteSpace(mdl) &&
                            mdl != "Not Detected")
                        {
                            detectionLimits.Add($"{element}: {mdl} (mg/kg)");
                            processedElements.Add(element);
                        }
                    }

                    if (detectionLimits.Count > 0)
                    {
                        currentRange.Text = "Method Detection Limit= " + string.Join(", ", detectionLimits);
                        currentRange.Font.Size = 10;
                        currentRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error in CreateTable23: {ex.Message.Replace("'", "\\'")}\\n{ex.StackTrace.Replace("'", "\\'").Replace("\n", "\\n")}');</script>");
            }
        }

        private void CreateTable23_BH(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script>alert('DataTable is null or empty');</script>");
                    return;
                }

                var groupedData = dt.AsEnumerable().GroupBy(row => row.Field<string>("Test Parameter")).ToList();
                Range currentRange = null;
                bool isFirstGroup = true;
                int tableNumber = 0; // LOCAL counter - starts fresh from 0

                foreach (var testParamGroup in groupedData)
                {
                    tableNumber++; // Increment local counter: 1, 2, 3...

                    var firstRow = testParamGroup.First();

                    if (isFirstGroup)
                    {
                        ReplaceTextInDocument(doc, "{TestParameter}", firstRow["Test Parameter"].ToString());

                        string methodText = CleanMethodText(firstRow["Method*"].ToString());

                        ReplaceTextInDocument(doc, "{TestMethod}", methodText);
                        ReplaceTextInDocument(doc, "{Equipment}", firstRow["Equipment"].ToString());

                        var range = doc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Text = placeholder;
                        if (range.Find.Execute())
                        {
                            currentRange = range;
                        }
                        else
                        {
                            currentRange = doc.Content;
                            currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                        }
                    }

                    if (!isFirstGroup && currentRange != null)
                    {
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                        currentRange.Text = $"{tableNumber}.      {firstRow["Test Parameter"]}\n"; // Use local counter
                        currentRange.Font.Bold = 1;
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;

                        string methodText = CleanMethodText(firstRow["Method*"].ToString());

                        currentRange.Text = $"Test Method: {methodText} - Analyzed by {firstRow["Equipment"]}\n";
                        currentRange.Font.Size = 11;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    if (currentRange == null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                            "<script>alert('Cannot find insertion point for table');</script>");
                        return;
                    }

                    int dataRowCount = testParamGroup.Count();
                    int totalColumns = 3;

                    Table table = doc.Tables.Add(currentRange, dataRowCount + 1, totalColumns);

                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;
                    table.PreferredWidth = wordApp.CentimetersToPoints(17f);
                    table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPoints;
                    table.AllowPageBreaks = true;

                    table.Columns[1].PreferredWidth = wordApp.CentimetersToPoints(7.0f);
                    table.Columns[2].PreferredWidth = wordApp.CentimetersToPoints(5.0f);
                    table.Columns[3].PreferredWidth = wordApp.CentimetersToPoints(5.0f);

                    table.Rows[1].Height = wordApp.CentimetersToPoints(1.5f);

                    table.Cell(1, 1).Range.Text = "Element/Compound";
                    table.Cell(1, 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    table.Cell(1, 1).Range.Font.Bold = 1;
                    table.Cell(1, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    Cell resultHeaderCell = table.Cell(1, 2);
                    resultHeaderCell.Range.Text = "Result\n(mg/kg)";
                    resultHeaderCell.Range.Font.Bold = 1;
                    resultHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    resultHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    resultHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    resultHeaderCell.WordWrap = true;

                    Cell specHeaderCell = table.Cell(1, 3);
                    specHeaderCell.Range.Text = "Specification\n(mg/kg)";
                    specHeaderCell.Range.Font.Bold = 1;
                    specHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    specHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    specHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    specHeaderCell.WordWrap = true;

                    int dataRowIndex = 2;
                    bool hasDetection = false;

                    foreach (var row in testParamGroup)
                    {
                        Cell elementCell = table.Cell(dataRowIndex, 1);
                        string elementValue = row["Element"]?.ToString() ?? "";
                        elementCell.Range.Text = elementValue;
                        elementCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        elementCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        elementCell.WordWrap = true;

                        Cell resultCell = table.Cell(dataRowIndex, 2);
                        string result = row["Result(mg/kg)"]?.ToString() ?? "N.D";
                        resultCell.Range.Text = result;
                        resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        if (result.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                            result.Equals("Detected", StringComparison.OrdinalIgnoreCase))
                        {
                            hasDetection = true;
                        }

                        Cell specCell = table.Cell(dataRowIndex, 3);
                        string requirementText = row["Requirement"]?.ToString() ?? "";
                        requirementText = WrapRequirementText_23_BH(requirementText);
                        specCell.Range.Text = requirementText;
                        specCell.WordWrap = true;
                        specCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        specCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        dataRowIndex++;
                    }

                    foreach (Row tblRow in table.Rows)
                    {
                        tblRow.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                        tblRow.Height = wordApp.CentimetersToPoints(1.0f);

                        foreach (Cell cell in tblRow.Cells)
                        {
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.Font.Size = 10;

                            if (cell.ColumnIndex == 1)
                            {
                                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                cell.WordWrap = true;
                            }
                            else
                            {
                                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                cell.WordWrap = true;
                            }
                        }
                    }

                    currentRange = table.Range;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                    currentRange.InsertParagraphAfter();
                    currentRange = currentRange.Paragraphs.Last.Range;

                    var detectionLimits = new List<string>();
                    var processedElements = new HashSet<string>();

                    foreach (var row in testParamGroup)
                    {
                        string element = row["Element"]?.ToString() ?? "";
                        string mdl = row["MDL"]?.ToString() ?? "";

                        if (!processedElements.Contains(element) &&
                            !string.IsNullOrWhiteSpace(mdl) &&
                            mdl != "Not Detected")
                        {
                            detectionLimits.Add($"{element}: {mdl} (mg/kg)");
                            processedElements.Add(element);
                        }
                    }

                    if (detectionLimits.Count > 0)
                    {
                        currentRange.Text = "Method Detection Limit= " + string.Join(", ", detectionLimits);
                        currentRange.Font.Size = 10;
                        currentRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    isFirstGroup = false;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error in CreateTable23_BH: {ex.Message.Replace("'", "\\'")}\\n{ex.StackTrace.Replace("'", "\\'").Replace("\n", "\\n")}');</script>");
            }
        }

        // Helper method for cleaning method text
        private string CleanMethodText(string methodText)
        {
            if (string.IsNullOrWhiteSpace(methodText))
                return string.Empty;

            methodText = methodText.Trim();

            if (methodText.Contains("TSQ-LAB"))
            {
                int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                if (colorCodeStart > 0)
                {
                    methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                }
            }

            // Remove duplicate commas
            while (methodText.Contains(",,"))
            {
                methodText = methodText.Replace(",,", ",");
            }

            methodText = methodText.Trim(',', ' ');

            return methodText;
        }

        private string WrapRequirementText_23(string text)
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

        private string WrapRequirementText_23_BH(string text)
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


        /****************************************************************| Geotexttile Test Report |******************************************************************************/


        private void CreateTable24(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script>alert('DataTable is null or empty');</script>");
                    return;
                }

                // First, clean and prepare the data - remove placeholder rows
                var cleanedRows = new List<DataRow>();
                foreach (DataRow row in dt.Rows)
                {
                    string testParam = row.Field<string>("TestParameter");
                    // Skip rows with placeholder text or empty test parameters
                    if (!string.IsNullOrEmpty(testParam) &&
                        !testParam.Contains("{TestParameter}") &&
                        testParam != "1. {TestParameter}")
                    {
                        cleanedRows.Add(row);
                    }
                }

                if (cleanedRows.Count == 0) return;

                // Group by TestParameter first to get unique test parameters
                var testParameters = cleanedRows
                    .Select(r => r.Field<string>("TestParameter"))
                    .Where(tp => !string.IsNullOrEmpty(tp))
                    .Distinct()
                    .ToList();

                Range currentRange = null;

                // Get the starting table number based on existing content in the document
                int startTableNumber = GetCurrentMaxTableNumber(doc) + 1;
                int currentTableNumber = startTableNumber;

                // Find the placeholder range
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                if (range.Find.Execute())
                {
                    currentRange = range;
                    currentRange.Text = ""; // Clear the placeholder
                }
                else
                {
                    currentRange = doc.Content;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                // Process each unique test parameter
                foreach (var testParam in testParameters)
                {
                    // Get all rows for this test parameter
                    var testParamRows = cleanedRows
                        .Where(r => r.Field<string>("TestParameter") == testParam)
                        .ToList();

                    // Add Test Parameter header with sequential number
                    currentRange.Text = $"{currentTableNumber}. {testParam}\n";
                    currentRange.Font.Bold = 1;
                    currentRange.Font.Size = 11;
                    currentRange.Font.Name = "Times New Roman";
                    currentRange.InsertParagraphAfter();
                    currentRange = currentRange.Paragraphs.Last.Range;

                    // Get unique test methods for this TestParameter
                    var testMethods = new List<string>();
                    foreach (var row in testParamRows)
                    {
                        string method = row.Field<string>("Method*");
                        if (!string.IsNullOrEmpty(method))
                        {
                            method = CleanMethodText_24(method);
                            if (!string.IsNullOrEmpty(method) && !testMethods.Contains(method))
                            {
                                testMethods.Add(method);
                            }
                        }
                    }

                    // Add Test Method line
                    if (testMethods.Count > 0)
                    {
                        currentRange.Text = $"Test Method: {string.Join(", ", testMethods)}\n";
                        currentRange.Font.Size = 11;
                        currentRange.Font.Bold = 0;
                        currentRange.Font.Name = "Times New Roman";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    // Check if this test has sub-parameters (MD/CD)
                    bool hasSubParameters = testParamRows.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));

                    if (hasSubParameters)
                    {
                        // Group by TestSubParameter
                        var subParamGroups = testParamRows
                            .GroupBy(r => r.Field<string>("TestSubParameter") ?? "Unknown")
                            .ToList();

                        foreach (var subParamGroup in subParamGroups)
                        {
                            string subParam = subParamGroup.Key;

                            // Add sub-parameter header if not empty
                            if (!string.IsNullOrEmpty(subParam))
                            {
                                currentRange.Text = $"{subParam}\n";
                                currentRange.Font.Bold = 1;
                                currentRange.Font.Size = 11;
                                currentRange.Font.Name = "Times New Roman";
                                currentRange.InsertParagraphAfter();
                                currentRange = currentRange.Paragraphs.Last.Range;
                            }

                            // Check if this group has multiple UOMs (like Strength and Elongation)
                            var uniqueUOMs = subParamGroup
                                .Select(r => r.Field<string>("UOM"))
                                .Where(u => !string.IsNullOrEmpty(u))
                                .Distinct()
                                .ToList();

                            if (uniqueUOMs.Count > 1)
                            {
                                // Multiple UOMs - create table with multiple result columns (like Capture-1.png)
                                CreateMultiColumnTableForGroup(wordApp, doc, ref currentRange, subParamGroup.ToList(), uniqueUOMs);
                            }
                            else
                            {
                                // Single UOM - create simple table with 2 columns (like Capture-2.png)
                                string uom = uniqueUOMs.FirstOrDefault() ?? "";
                                CreateSimpleTableForGroup(wordApp, doc, ref currentRange, subParamGroup.ToList(), uom);
                            }

                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
                    }
                    else
                    {
                        // No sub-parameters, create single simple table
                        string uom = testParamRows
                            .Select(r => r.Field<string>("UOM"))
                            .FirstOrDefault(u => !string.IsNullOrEmpty(u)) ?? "";

                        CreateSimpleTableForGroup(wordApp, doc, ref currentRange, testParamRows, uom);
                    }

                    // Add spacing between different Test Parameters
                    currentRange.InsertParagraphAfter();
                    currentRange = currentRange.Paragraphs.Last.Range;

                    // Increment the table number for the next Test Parameter
                    currentTableNumber++;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error in CreateTable24: {ex.Message.Replace("'", "\\'")}');</script>");
            }
        }

        private int GetCurrentMaxTableNumber(Document doc)
        {
            int maxNumber = 0;
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.MatchWildcards = true;
                range.Find.Text = "[0-9]@. ";

                while (range.Find.Execute())
                {
                    string foundText = range.Text.Trim();
                    if (foundText.Contains("."))
                    {
                        string numberPart = foundText.Substring(0, foundText.IndexOf('.'));
                        if (int.TryParse(numberPart, out int num))
                        {
                            if (num > maxNumber)
                                maxNumber = num;
                        }
                    }
                    range.Collapse(WdCollapseDirection.wdCollapseEnd);
                }
            }
            catch
            {
                // If error, start from 0
            }
            return maxNumber;
        }

        private string CleanMethodText_24(string methodText)
        {
            if (string.IsNullOrWhiteSpace(methodText))
                return string.Empty;

            methodText = methodText.Trim();

            // Remove TSQ-LAB color codes if present
            if (methodText.Contains("TSQ-LAB"))
            {
                int colorCodeStart = methodText.IndexOf("TSQ-LAB");
                if (colorCodeStart > 0)
                {
                    methodText = methodText.Substring(0, colorCodeStart).Trim().TrimEnd(',');
                }
            }

            // Remove duplicate commas and clean up
            methodText = System.Text.RegularExpressions.Regex.Replace(methodText, @",\s*,", ",");

            // Split by comma, trim each part, and remove empty/duplicate entries
            var methods = methodText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrEmpty(m) && m != "ASTM" && m.Length > 2)
                .Distinct()
                .ToList();

            // Clean up any remaining issues
            for (int i = 0; i < methods.Count; i++)
            {
                // Remove any standalone "ASTM" entries
                if (methods[i] == "ASTM" || methods[i].Length < 4)
                {
                    methods.RemoveAt(i);
                    i--;
                }
            }

            return string.Join(", ", methods);
        }

        private void CreateSimpleTableForGroup(Application wordApp, Document doc, ref Range currentRange, List<DataRow> rows, string uom)
        {
            try
            {
                // Organize specimen data
                var specimenData = new Dictionary<int, string>();

                foreach (var dataRow in rows)
                {
                    string specimenStr = dataRow.Field<string>("Specimen") ?? "Specimen 1";
                    int specimenNum = 1;

                    if (specimenStr.StartsWith("Specimen "))
                    {
                        int.TryParse(specimenStr.Replace("Specimen ", ""), out specimenNum);
                    }

                    string result = dataRow.Field<string>("Result") ?? "";

                    // Only add if result is not empty
                    if (!string.IsNullOrEmpty(result))
                    {
                        specimenData[specimenNum] = result;
                    }
                }

                if (specimenData.Count == 0) return;

                // Create table with 2 columns
                int specimenCount = specimenData.Count;
                Table table = doc.Tables.Add(currentRange, specimenCount + 2, 2);

                // Format table for A4 page width
                float pageWidth = wordApp.CentimetersToPoints(21f); // A4 width
                float leftMargin = wordApp.CentimetersToPoints(2.5f); // Left margin
                float rightMargin = wordApp.CentimetersToPoints(2.5f); // Right margin
                float availableWidth = pageWidth - leftMargin - rightMargin;

                table.PreferredWidth = availableWidth;
                table.Borders.Enable = 1;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.AllowAutoFit = true;

                // FIX: Set column widths using preferred width instead of direct column access
                // This avoids the "mixed cell widths" error
                table.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Set column widths as percentages of table width
                table.Columns[1].PreferredWidth = 30; // 30% for Specimen column
                table.Columns[2].PreferredWidth = 70; // 70% for Result column

                // Header row
                Cell firstHeaderCell = table.Cell(1, 1);
                firstHeaderCell.Range.Text = "Specimen";
                firstHeaderCell.Range.Font.Bold = 1;
                firstHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                firstHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                firstHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Build header text properly to avoid line breaks in the middle
                Cell resultHeaderCell = table.Cell(1, 2);
                string headerText = "Result";
                if (!string.IsNullOrEmpty(uom))
                {
                    // Ensure the UOM is properly formatted without causing line breaks
                    string cleanUom = uom.Trim();

                    // Check if UOM already has parentheses
                    if (cleanUom.StartsWith("(") && cleanUom.EndsWith(")"))
                    {
                        headerText += $" {cleanUom}";
                    }
                    else if (cleanUom.Contains("(") && cleanUom.Contains(")"))
                    {
                        // UOM already has parentheses in the middle, keep as is
                        headerText += $" {cleanUom}";
                    }
                    else
                    {
                        // Add parentheses around the UOM
                        headerText += $" ({cleanUom})";
                    }
                }

                resultHeaderCell.Range.Text = headerText;
                resultHeaderCell.Range.Font.Bold = 1;
                resultHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                resultHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                resultHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Add specimen rows
                int row = 2;
                List<double> values = new List<double>();

                var sortedSpecimens = specimenData.Keys.OrderBy(x => x).ToList();

                foreach (var specimenNum in sortedSpecimens)
                {
                    // Specimen column
                    Cell specCell = table.Cell(row, 1);
                    specCell.Range.Text = $"Specimen {specimenNum}";
                    specCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    specCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Result column
                    string result = specimenData[specimenNum];
                    Cell resultCell = table.Cell(row, 2);
                    resultCell.Range.Text = result;
                    resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    if (double.TryParse(result, out double val))
                    {
                        values.Add(val);
                    }

                    row++;
                }

                // Add average row
                Cell avgLabelCell = table.Cell(row, 1);
                avgLabelCell.Range.Text = "Average";
                avgLabelCell.Range.Font.Bold = 1;
                avgLabelCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                avgLabelCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                if (values.Count > 0)
                {
                    double avg = values.Average();

                    // Determine format based on the values
                    string format = "F1";
                    if (values.All(v => Math.Abs(v - Math.Round(v, 2)) > 0.001))
                    {
                        format = "F2";
                    }

                    // Special handling for percentage and strength units
                    if (!string.IsNullOrEmpty(uom) &&
                        (uom.Contains("%") ||
                         uom.Contains("Loss of Strength") ||
                         uom.Contains("mm") ||
                         uom.Contains("thickness")))
                    {
                        format = "F2";
                    }

                    Cell avgValueCell = table.Cell(row, 2);
                    avgValueCell.Range.Text = avg.ToString(format);
                    avgValueCell.Range.Font.Bold = 1;
                    avgValueCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    avgValueCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }

                // FIX: Use AutoFit behavior instead of setting fixed row heights
                // This prevents cell width conflicts
                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);

                // Then set preferred row heights
                for (int r = 1; r <= table.Rows.Count; r++)
                {
                    table.Rows[r].Height = wordApp.CentimetersToPoints(0.8f);
                    table.Rows[r].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                }

                // Format all cells font
                for (int r = 1; r <= table.Rows.Count; r++)
                {
                    for (int c = 1; c <= 2; c++)
                    {
                        Cell cell = table.Cell(r, c);
                        cell.Range.Font.Size = 10;
                        cell.Range.Font.Name = "Times New Roman";

                        // Ensure no extra line breaks in the text
                        if (cell.Range.Text.Contains("\r") && !cell.Range.Text.EndsWith("\r\a"))
                        {
                            cell.Range.Text = cell.Range.Text.Replace("\r", " ").Trim();
                        }
                    }
                }

                currentRange = table.Range;
                currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in CreateSimpleTableForGroup: {ex.Message}");
            }
        }

        private void CreateMultiColumnTableForGroup(Application wordApp, Document doc, ref Range currentRange, List<DataRow> rows, List<string> uoms)
        {
            try
            {
                // Group data by specimen number first
                var specimenGroups = rows
                    .GroupBy(r =>
                    {
                        string specimenStr = r.Field<string>("Specimen") ?? "Specimen 1";
                        int specimenNum = 1;
                        if (specimenStr.StartsWith("Specimen "))
                        {
                            int.TryParse(specimenStr.Replace("Specimen ", ""), out specimenNum);
                        }
                        return specimenNum;
                    })
                    .OrderBy(g => g.Key)
                    .ToList();

                if (specimenGroups.Count == 0) return;

                // Create table with (1 + uoms.Count) columns
                // 1 for Specimen column + columns for each UOM
                Table table = doc.Tables.Add(currentRange, specimenGroups.Count + 2, 1 + uoms.Count);

                // Format table for A4 page width
                float pageWidth = wordApp.CentimetersToPoints(21f); // A4 width
                float leftMargin = wordApp.CentimetersToPoints(2.5f); // Left margin
                float rightMargin = wordApp.CentimetersToPoints(2.5f); // Right margin
                float availableWidth = pageWidth - leftMargin - rightMargin;

                table.PreferredWidth = availableWidth;
                table.Borders.Enable = 1;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.AllowAutoFit = true;

                // FIX: Use preferred width percentages instead of direct width assignment
                table.Columns.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Set column widths as percentages
                float specColumnPercent = 20f; // 20% for Specimen column
                float resultColumnPercent = 80f / uoms.Count; // Remaining 80% divided equally among UOM columns

                table.Columns[1].PreferredWidth = specColumnPercent;
                for (int i = 2; i <= 1 + uoms.Count; i++)
                {
                    table.Columns[i].PreferredWidth = resultColumnPercent;
                }

                // Header row
                Cell firstHeaderCell = table.Cell(1, 1);
                firstHeaderCell.Range.Text = "Specimen";
                firstHeaderCell.Range.Font.Bold = 1;
                firstHeaderCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                firstHeaderCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                firstHeaderCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Add UOM headers with proper formatting
                for (int i = 0; i < uoms.Count; i++)
                {
                    Cell headerCell = table.Cell(1, i + 2);
                    string cleanUom = uoms[i].Trim();

                    // Format UOM text nicely based on content
                    if (cleanUom.Contains("kN/m") || cleanUom.Contains("%"))
                    {
                        // For units like "Strength (kN/m)" or "Loss of Strength (%)"
                        headerCell.Range.Text = cleanUom;
                    }
                    else if (cleanUom.Contains("Strength") || cleanUom.Contains("Loss"))
                    {
                        // For descriptive UOMs like "Loss of Strength (%)" - keep as is
                        headerCell.Range.Text = cleanUom;
                    }
                    else if (!cleanUom.StartsWith("(") && !cleanUom.EndsWith(")"))
                    {
                        headerCell.Range.Text = $"({cleanUom})";
                    }
                    else
                    {
                        headerCell.Range.Text = cleanUom;
                    }

                    headerCell.Range.Font.Bold = 1;
                    headerCell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Allow text wrapping in header if needed
                    headerCell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                }

                // Add specimen rows
                int row = 2;
                var valuesByUOM = new Dictionary<string, List<double>>();
                foreach (var uom in uoms)
                {
                    valuesByUOM[uom] = new List<double>();
                }

                foreach (var specimenGroup in specimenGroups)
                {
                    int specimenNum = specimenGroup.Key;

                    // Specimen column
                    Cell specCell = table.Cell(row, 1);
                    specCell.Range.Text = $"Specimen {specimenNum}";
                    specCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    specCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Fill data for each UOM
                    var dataByUOM = specimenGroup.ToDictionary(
                        r => r.Field<string>("UOM") ?? "",
                        r => r.Field<string>("Result") ?? ""
                    );

                    for (int i = 0; i < uoms.Count; i++)
                    {
                        string uom = uoms[i];
                        Cell resultCell = table.Cell(row, i + 2);

                        if (dataByUOM.ContainsKey(uom) && !string.IsNullOrEmpty(dataByUOM[uom]) && dataByUOM[uom] != "-")
                        {
                            string result = dataByUOM[uom];
                            resultCell.Range.Text = result;
                            resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            if (double.TryParse(result, out double val))
                            {
                                valuesByUOM[uom].Add(val);
                            }
                        }
                        else
                        {
                            resultCell.Range.Text = "-";
                            resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        }
                    }

                    row++;
                }

                // Add average row
                Cell avgLabelCell = table.Cell(row, 1);
                avgLabelCell.Range.Text = "Average";
                avgLabelCell.Range.Font.Bold = 1;
                avgLabelCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                avgLabelCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                for (int i = 0; i < uoms.Count; i++)
                {
                    string uom = uoms[i];
                    Cell avgValueCell = table.Cell(row, i + 2);
                    avgValueCell.Range.Font.Bold = 1;

                    if (valuesByUOM.ContainsKey(uom) && valuesByUOM[uom].Count > 0)
                    {
                        double avg = valuesByUOM[uom].Average();

                        // Determine format based on the values
                        string format = "F1";
                        if (valuesByUOM[uom].All(v => Math.Abs(v - Math.Round(v, 2)) > 0.001))
                        {
                            format = "F2";
                        }

                        // Special handling for percentage
                        if (uom.Contains("%"))
                        {
                            format = "F2";
                        }

                        avgValueCell.Range.Text = avg.ToString(format);

                        // Bold the average value as shown in the image
                        avgValueCell.Range.Font.Bold = 1;
                    }
                    else
                    {
                        avgValueCell.Range.Text = "-";
                    }

                    avgValueCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    avgValueCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }

                // FIX: Use AutoFit behavior first to resolve any width conflicts
                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);

                // Then set preferred row heights
                for (int r = 1; r <= table.Rows.Count; r++)
                {
                    table.Rows[r].Height = wordApp.CentimetersToPoints(0.8f);
                    table.Rows[r].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                }

                // Format all cells font
                for (int r = 1; r <= table.Rows.Count; r++)
                {
                    for (int c = 1; c <= 1 + uoms.Count; c++)
                    {
                        Cell cell = table.Cell(r, c);
                        cell.Range.Font.Size = 10;
                        cell.Range.Font.Name = "Times New Roman";

                        // Ensure no extra line breaks in the text
                        if (cell.Range.Text.Contains("\r") && !cell.Range.Text.EndsWith("\r\a"))
                        {
                            cell.Range.Text = cell.Range.Text.Replace("\r", " ").Trim();
                        }
                    }
                }

                currentRange = table.Range;
                currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in CreateMultiColumnTableForGroup: {ex.Message}");
            }
        }

        private void CreateTable24_BH(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script>alert('DataTable is null or empty');</script>");
                    return;
                }

                // First, clean and prepare the data - remove placeholder rows
                var cleanedRows = new List<DataRow>();
                foreach (DataRow row in dt.Rows)
                {
                    string testParam = row.Field<string>("TestParameter");
                    // Skip rows with placeholder text or empty test parameters
                    if (!string.IsNullOrEmpty(testParam) &&
                        !testParam.Contains("{TestParameter}") &&
                        testParam != "1. {TestParameter}")
                    {
                        cleanedRows.Add(row);
                    }
                }

                if (cleanedRows.Count == 0) return;

                // Group by TestParameter first to get unique test parameters
                var testParameters = cleanedRows
                    .Select(r => r.Field<string>("TestParameter"))
                    .Where(tp => !string.IsNullOrEmpty(tp))
                    .Distinct()
                    .ToList();

                Range currentRange = null;

                // Get the starting table number based on existing content in the document
                int startTableNumber = GetCurrentMaxTableNumber(doc) + 1;
                int currentTableNumber = startTableNumber;

                // Find the placeholder range
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                if (range.Find.Execute())
                {
                    currentRange = range;
                    currentRange.Text = ""; // Clear the placeholder
                }
                else
                {
                    currentRange = doc.Content;
                    currentRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                // Process each unique test parameter
                foreach (var testParam in testParameters)
                {
                    // Get all rows for this test parameter
                    var testParamRows = cleanedRows
                        .Where(r => r.Field<string>("TestParameter") == testParam)
                        .ToList();

                    // Add Test Parameter header with sequential number
                    currentRange.Text = $"{currentTableNumber}. {testParam}\n";
                    currentRange.Font.Bold = 1;
                    currentRange.Font.Size = 11;
                    currentRange.Font.Name = "Times New Roman";
                    currentRange.InsertParagraphAfter();
                    currentRange = currentRange.Paragraphs.Last.Range;

                    // Get unique test methods for this TestParameter
                    var testMethods = new List<string>();
                    foreach (var row in testParamRows)
                    {
                        string method = row.Field<string>("Method*");
                        if (!string.IsNullOrEmpty(method))
                        {
                            method = CleanMethodText_24(method);
                            if (!string.IsNullOrEmpty(method) && !testMethods.Contains(method))
                            {
                                testMethods.Add(method);
                            }
                        }
                    }

                    // Add Test Method line
                    if (testMethods.Count > 0)
                    {
                        currentRange.Text = $"Test Method: {string.Join(", ", testMethods)}\n";
                        currentRange.Font.Size = 11;
                        currentRange.Font.Bold = 0;
                        currentRange.Font.Name = "Times New Roman";
                        currentRange.InsertParagraphAfter();
                        currentRange = currentRange.Paragraphs.Last.Range;
                    }

                    // Check if this test has sub-parameters (MD/CD)
                    bool hasSubParameters = testParamRows.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));

                    if (hasSubParameters)
                    {
                        // Group by TestSubParameter
                        var subParamGroups = testParamRows
                            .GroupBy(r => r.Field<string>("TestSubParameter") ?? "Unknown")
                            .ToList();

                        foreach (var subParamGroup in subParamGroups)
                        {
                            string subParam = subParamGroup.Key;

                            // Add sub-parameter header if not empty
                            if (!string.IsNullOrEmpty(subParam))
                            {
                                currentRange.Text = $"{subParam}\n";
                                currentRange.Font.Bold = 1;
                                currentRange.Font.Size = 11;
                                currentRange.Font.Name = "Times New Roman";
                                currentRange.InsertParagraphAfter();
                                currentRange = currentRange.Paragraphs.Last.Range;
                            }

                            // Check if this group has multiple UOMs (like Strength and Elongation)
                            var uniqueUOMs = subParamGroup
                                .Select(r => r.Field<string>("UOM"))
                                .Where(u => !string.IsNullOrEmpty(u))
                                .Distinct()
                                .ToList();

                            if (uniqueUOMs.Count > 1)
                            {
                                // Multiple UOMs - create table with multiple result columns (like Capture-1.png)
                                CreateMultiColumnTableForGroup(wordApp, doc, ref currentRange, subParamGroup.ToList(), uniqueUOMs);
                            }
                            else
                            {
                                // Single UOM - create simple table with 2 columns (like Capture-2.png)
                                string uom = uniqueUOMs.FirstOrDefault() ?? "";
                                CreateSimpleTableForGroup(wordApp, doc, ref currentRange, subParamGroup.ToList(), uom);
                            }

                            currentRange.InsertParagraphAfter();
                            currentRange = currentRange.Paragraphs.Last.Range;
                        }
                    }
                    else
                    {
                        // No sub-parameters, create single simple table
                        string uom = testParamRows
                            .Select(r => r.Field<string>("UOM"))
                            .FirstOrDefault(u => !string.IsNullOrEmpty(u)) ?? "";

                        CreateSimpleTableForGroup(wordApp, doc, ref currentRange, testParamRows, uom);
                    }

                    // Add spacing between different Test Parameters
                    currentRange.InsertParagraphAfter();
                    currentRange = currentRange.Paragraphs.Last.Range;

                    // Increment the table number for the next Test Parameter
                    currentTableNumber++;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error in CreateTable24: {ex.Message.Replace("'", "\\'")}');</script>");
            }
        }
    }
}
        
