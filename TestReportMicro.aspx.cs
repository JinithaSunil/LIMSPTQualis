using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class TestReportMicro : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CloneCRFSampleID = 0;
        DAL.mailer objEmail = new DAL.mailer();
        DateTime DateOfIsuue = new DateTime();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {

                if (Request.QueryString["s"] != null)
                {
                    Session["SampleId"] = Request.QueryString["s"].ToString();
                }
                if (Request.QueryString["c"] != null)
                {
                    Session["CloneCRFSampleID"] = Request.QueryString["c"].ToString();
                }
                if (Request.QueryString["t"] != null)
                {
                    Session["TestResultId"] = Request.QueryString["t"].ToString();
                }
                if (Session["EmployeeID"] != null)
                {
                    string EmployeeID = Session["EmployeeID"].ToString();
                    string SampleId = Session["SampleId"].ToString();
                    string TestResultId = Session["TestResultId"].ToString();

                    string ULRNo = "";
                    string IssueDate = "";
                    string CRFReceivedDate = "";
                    string LabSampleId = "";

                    string Department = "";
                    string ProductCategory = "";
                    string CustomerName = "";
                    string NoOfSample = "";
                    string ReceivedDate = "";
                    string ContactPerson = "";
                    string Mobile = "";
                    string Email = "";
                    string Dated = "";
                    string BatchNo = "";
                    string AnalysisStartingDate = "";
                    string AnalysisCompletionDate = "";
                    string NameOfProduct = "";
                    string DateOfMfg = "";
                    string DateOfExp = "";
                    string SampleQuality = "";
                    string TestRequired = "";
                    string SampleCondition = "";
                    string Sampling = "";
                    string SamplingMethod = "";
                    string SamplingLocation = "";
                    string TransportAndReceiptTemp = "";
                    string DateOfSampling = "";
                    string Specification = "";
                    string TestParameter = "";
                    string TestMethod = "";
                    string Sr = "";
                    string Unit = "";
                    string TestResult = "";
                    string TestReportNo = "";
                    string inquiryID = "";
                    string CustomerReference = "";
                    string Remarks = "";
                    string SignatoryID = "";
                    string SignatoryName = "";
                    string SignatoryDesignation = "";
                    string ReportType = "";
                    string TemplateName = "";
                    string ReportFormat = "";

                    DataTable dt3 = new DataTable();
                    DataTable dt1 = _cm.GetTestReportDetails1(SampleId);
                    DataTable dt4 = _cm.GetTestReportDetails2(SampleId);
                    StringBuilder sbAddress = new StringBuilder();
                    if (dt1.Rows.Count > 0)
                    {
                        Department = dt1.Rows[0]["DepartmentName"].ToString();
                        ProductCategory = dt1.Rows[0]["productgroupname"].ToString();
                        NoOfSample = dt1.Rows[0]["SizeOfSample"].ToString();
                        DateTime ReceivedDt = Convert.ToDateTime(dt1.Rows[0]["ReceivedDate"].ToString());
                        ReceivedDate = ReceivedDt.ToString("dd-MM-yyyy");
                        SampleId = dt1.Rows[0]["SampleId"].ToString();
                        Session["SampleUID"] = SampleId;
                        LabSampleId = dt1.Rows[0]["SampleIdSeries"].ToString();
                        DateTime date = Convert.ToDateTime(dt1.Rows[0]["inquiryDate"].ToString());
                        Dated = date.ToString("dd-MM-yyyy");
                        BatchNo = dt1.Rows[0]["BatchNo"].ToString();
                        inquiryID = dt1.Rows[0]["inquiryID"].ToString();
                        CloneCRFSampleID = Convert.ToInt32(dt1.Rows[0]["CloneCRFSampleID"].ToString());
                    }
                    if (dt4.Rows.Count > 0)
                    {
                        CustomerName = dt4.Rows[0]["CustomerName"].ToString();
                        ContactPerson = dt4.Rows[0]["ContactPerson"].ToString();
                        Mobile = dt4.Rows[0]["Mobile"].ToString();
                        Email = dt4.Rows[0]["Email"].ToString();
                        if (dt4.Rows[0]["AddressLine1"].ToString() != "")
                        {
                            sbAddress.Append(dt4.Rows[0]["AddressLine1"].ToString());
                        }
                        if (dt4.Rows[0]["AddressLine2"].ToString() != "")
                        {
                            sbAddress.Append(dt4.Rows[0]["AddressLine2"].ToString());
                        }
                        if (dt4.Rows[0]["AddressLine3"].ToString() != "")
                        {
                            sbAddress.AppendLine();
                            sbAddress.Append(dt4.Rows[0]["AddressLine3"].ToString());
                        }
                        if (dt4.Rows[0]["City"].ToString() != "")
                        {
                            sbAddress.Append("Village - " + dt4.Rows[0]["City"].ToString());
                        }
                        if (dt4.Rows[0]["Taluka"].ToString() != "")
                        {
                            sbAddress.Append("Taluka - " + dt4.Rows[0]["Taluka"].ToString());
                        }
                        if (dt4.Rows[0]["District"].ToString() != "")
                        {
                            sbAddress.AppendLine();
                            sbAddress.Append(dt4.Rows[0]["District"].ToString());
                        }
                        if (dt4.Rows[0]["Pincode"].ToString() != "")
                        {
                            sbAddress.Append("-" + dt4.Rows[0]["Pincode"].ToString());
                        }
                        if (dt4.Rows[0]["State"].ToString() != "")
                        {
                            sbAddress.AppendLine();
                            sbAddress.Append(dt4.Rows[0]["State"].ToString());
                        }
                        if (dt4.Rows[0]["Country"].ToString() != "")
                        {
                            sbAddress.Append("," + dt4.Rows[0]["Country"].ToString());
                        }
                        if (ContactPerson != "") { sbAddress.Append("<br/>Contact Person: " + ContactPerson); }
                        if (Mobile != "") { sbAddress.Append(" <br/>Mobile: " + Mobile); }
                        if (Email != "") { sbAddress.Append(" <br/>Email: " + Email); }
                    }
                    SampleId = Session["SampleId"].ToString();
                    DataTable dt2 = _cm.GetTestResultDetails(SampleId);
                    if (dt2.Rows.Count > 0)
                    {
                        ULRNo = dt2.Rows[0]["ULRNo"].ToString();
                        TestReportNo = dt2.Rows[0]["TestReportNo"].ToString();
                        DateOfIsuue = Convert.ToDateTime(dt2.Rows[0]["IssueDate"].ToString());
                        IssueDate = DateOfIsuue.ToString("dd-MM-yyyy");
                        DateTime CRFdate = Convert.ToDateTime(dt2.Rows[0]["CRFdate"].ToString());
                        CRFReceivedDate = CRFdate.ToString("dd-MM-yyyy");
                        DateTime StartDate = Convert.ToDateTime(dt2.Rows[0]["AnalysisStartingDate"].ToString());
                        AnalysisStartingDate = StartDate.ToString("dd-MM-yyyy");
                        DateTime EndDate = Convert.ToDateTime(dt2.Rows[0]["AnalysisCompletionDate"].ToString());
                        AnalysisCompletionDate = EndDate.ToString("dd-MM-yyyy");
                        NameOfProduct = dt2.Rows[0]["NameOfProduct"].ToString();
                        DateOfMfg = dt2.Rows[0]["DateOfMfg"].ToString();
                        DateOfExp = dt2.Rows[0]["DateOfExp"].ToString();
                        TestRequired = dt2.Rows[0]["TestRequired"].ToString();
                        SampleCondition = dt2.Rows[0]["SampleCondition"].ToString();
                        Sampling = dt2.Rows[0]["Sampling"].ToString();
                        SamplingMethod = dt2.Rows[0]["SamplingMethod"].ToString();
                        SamplingLocation = dt2.Rows[0]["SamplingLocation"].ToString();
                        TransportAndReceiptTemp = dt2.Rows[0]["TransportAndReceiptTemp"].ToString();
                        if ((dt2.Rows[0]["DateOfSampling"].ToString() != "")&& (dt2.Rows[0]["DateOfSampling"].ToString() != "-")&&(dt2.Rows[0]["DateOfSampling"].ToString() != "NA"))
                        {
                            DateOfSampling = Convert.ToDateTime(dt2.Rows[0]["DateOfSampling"]).ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            DateOfSampling = dt2.Rows[0]["DateOfSampling"].ToString();
                        }
                        SampleQuality = dt2.Rows[0]["SampleQuality"].ToString();
                        CustomerReference = dt2.Rows[0]["CustomerReference"].ToString();
                        Remarks = dt2.Rows[0]["Remarks"].ToString();
                        SignatoryID = dt2.Rows[0]["authorizedSignatoryID"].ToString();
                        ReportType = dt2.Rows[0]["ReportType"].ToString();


                    }
                    int ID = 0;
                    StringBuilder sb = new StringBuilder();
                    StringReader sr = new StringReader(sb.ToString());
                    ID = Convert.ToInt32(SampleId);
                    dt3.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("Sr", typeof(string)),
                    new DataColumn("TestParameter", typeof(string)),
                    new DataColumn("TestResult",typeof(string)),
                    new DataColumn("UnitOfMeasurement",typeof(string)),
                    new DataColumn("Spec",typeof(string)),
                    new DataColumn("TestMethod",typeof(string)),
                });

                    dt3 = _cm.GetTestReportResult(ID.ToString());

                    sb.Append("<table style='width:100%'");

                    //Adding HeaderRow.
                    sb.Append("<tr>");
                    sb.Append("<td style='width:5%'><b>Sr.No.</b></td>");
                    sb.Append("<td style='width:20%'><b>Test Parameters</b></td>");
                    sb.Append("<td style='width:5%'><b>Results</b></td>");
                    sb.Append("<td style='width:5%'><b>Unit</b></td>");
                    sb.Append("<td style='width:15%'><b>Method of Test</b></td>");
                    sb.Append("<td style='width:25%'><b>Requirement As Per Customer</b></td>");

                    sb.Append("</tr>");

                    for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                    {
                        sb.Append("<tr>");
                        {
                            Sr = dt3.Rows[freqCount - 1]["Sr"].ToString();
                            TestParameter = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                            TestResult = dt3.Rows[freqCount - 1]["TestResult"].ToString();
                            Unit = dt3.Rows[freqCount - 1]["UnitOfMeasurement"].ToString();
                            TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                            Specification = dt3.Rows[freqCount - 1]["Specification"].ToString();
                            TestResult = _cm.ReplaceSuperScript(TestResult);
                            Unit = _cm.ReplaceSuperScript(Unit);
                            sb.Append("<td style='width:5%;'>" + Sr + "</td>");
                            sb.Append("<td style='width:5%;'>" + TestParameter + "</td>");
                            sb.Append("<td style='width:10%;'>" + TestResult + "</td>");
                            sb.Append("<td style='width:5%;'>" + Unit + "</td>");
                            sb.Append("<td style='width:15%;'>" + TestMethod + "</td>");
                            sb.Append("<td style='width:25%;'>" + Specification + "</td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    sb.Append("<br />");
                    //Table end.
                    DataTable dtReportFormat = _cm.GetTestReportNumber("Microbiology");
                    //if (dtReportFormat != null)
                    //{
                    //    ReportFormat = dtReportFormat.Rows[0]["ReportFormatNumber"].ToString();
                    //}
                    DataTable dt5 = _cm.getSignatory(SignatoryID);
                    if (dt5.Rows.Count > 0)
                    {
                        SignatoryName = dt5.Rows[0]["EmployeeName"].ToString();
                        SignatoryDesignation = dt5.Rows[0]["DesignationName"].ToString();
                    }

                    DataTable dt7 = _cm.CheckSignatory(EmployeeID);
                    StringBuilder sbSignature = new StringBuilder();
                    StringBuilder sbQRCode = new StringBuilder();
                    if (dt7.Rows.Count > 0)
                    {
                        SampleId = Session["SampleUID"].ToString();
                        sbSignature.Append("<div style='text-align:right;border:none !important; padding-right:1%;background:url(../assets/img/Signature/ValidSign.png) no-repeat;background-position: right;background-size:33%'>");
                        sbSignature.Append("<div style='text-align:right;border:none !important; padding-right:1%;'>");
                        sbSignature.Append("<img src='http://amcogenlims.rnisolutions.in:1234/assets/img/Signature/" + EmployeeID + ".png' width='auto' height='60' style='padding-bottom:10%'>");
                        sbSignature.Append("<br/><span style='font-size:9px;width:20%; text-align:right !important; line-height:10px'>Date: " + Convert.ToDateTime(DateOfIsuue).ToString("yyyy.MM.dd") + "<br/></span>");
                        //sbSignature.Append("<br/><span style='font-size:9px;width:20%; text-align:right !important; line-height:10px'>Date: " + DateTime.Now.AddHours(12.5).ToString("yyyy.MM.dd HH:mm:ss") + "<br/></span>");
                        sbSignature.Append("<br/><span style='font-size:9px;width:20%; text-align:right !important; line-height:10px'><br/></span>");
                        sbSignature.Append("<span>AUTHORIZED SIGNATORY</span><br/><span>");
                        sbSignature.Append("<span>" + SignatoryName + "</span><br/><span>(" + SignatoryDesignation + ")</span><br/></div>");
                        string QR = "http://amcogenlims.rnisolutions.in:1234/TestReport/TestReport-" + TestResultId + SampleId + ".htm";
                        GenerateQR(QR, TestResultId + SampleId);
                        sbQRCode.Append("<img src='../TestReport/" + TestResultId + SampleId + ".jpg' width='auto' height='100'>");
                    }
                    else
                    {
                        SampleId = Session["SampleUID"].ToString();
                        sbSignature.Append("<div style='background-size:40%'>");
                        sbSignature.Append("<br/><br/><br/><br/>");
                        sbSignature.Append("<br/><span><br/></span>");
                        sbSignature.Append("<span>AUTHORIZED SIGNATORY</span><br/><span>");
                        sbSignature.Append("<span>" + SignatoryName + "</span><br/><span>(" + SignatoryDesignation + ")</span><br/></div>");
                        string QR = "http://amcogenlims.rnisolutions.in:1234/TestReport/TestReport-" + TestResultId + SampleId + ".htm";
                        GenerateQR(QR, TestResultId + SampleId);
                        sbQRCode.Append("<img src='../TestReport/" + TestResultId + SampleId + ".jpg' width='auto' height='100'>");
                    }
                    switch (ReportType.Trim())
                    {
                        case "ACCREDITATION": TemplateName = "TestReportMicro.html"; break;
                        case "NON-ACCREDITATION": TemplateName = "TestReportMicroNonNABL.html"; break;
                        default: break;
                    }
                    string body = objEmail.PrepareMailBodyWith(TemplateName,
                        "{ULRNo}", ULRNo,
                        "{DateOfIssue}", IssueDate,
                        "{TestReportNo}", TestReportNo,
                        "{Department}", Department,
                        "{ProductGroup}", ProductCategory,
                        "{CustomerName}", CustomerName,
                        "{Address}", sbAddress.ToString(),
                        "{NoOfSample}", NoOfSample,
                        "{ReceivedDate}", ReceivedDate,
                        "{LabSampleId}", LabSampleId,
                        "{SampleId}", LabSampleId,
                        "{StartDate}", AnalysisStartingDate,
                        "{CompletionDate}", AnalysisCompletionDate,
                        "{ContactPerson}", ContactPerson,
                        "{Date}", CRFReceivedDate,
                        "{BatchNo}", BatchNo,
                        "{SampleRegistrationDate}", Dated,
                        "{TestRequired}", TestRequired,
                        "{SampleCondition}", SampleCondition,
                        "{Sampling}", Sampling,
                        "{SamplingMethod}", SamplingMethod,
                        "{DateOfMfg}", DateOfMfg,
                        "{ExpDate}", DateOfExp,
                        "{SampleQuantity}", SampleQuality,
                        "{NameOfProduct}", NameOfProduct,
                        "{CustomerReference}", CustomerReference,
                        "{Remarks}", Remarks,
                        "{SignatoryName}", SignatoryName,
                        "{SignatoryDesignation}", SignatoryDesignation,
                        "{TestResult}", sb.ToString(),
                        "{ReportFormatNumber}", ReportFormat,
                        "{Signature}", sbSignature.ToString(),
                        "{SamplingLocation}", SamplingLocation.ToString(),
                        "{TransportAndReceiptTemp}", TransportAndReceiptTemp.ToString(),
                        "{DateOfSampling}", DateOfSampling.ToString(),
                        "{QRCode}", sbQRCode.ToString());
                    SampleId = Session["SampleUID"].ToString();
                    string str = body.ToString();
                    string filename = Server.MapPath("//TestReport/TestReport-" + TestResultId + SampleId + ".htm");
                    System.IO.File.WriteAllText(@filename, str);
                    if (dt7.Rows.Count == 0)
                    {
                        _cm.CRFStatusID = 12;
                        _cm.sampleStatusID = 12;//Test Report Submitted for Review 
                    }
                    else
                    {
                        _cm.CRFStatusID = 24;
                        _cm.sampleStatusID = 24;//Reviewed Accepted
                    }
                    _cm.CloneCRFSampleID = CloneCRFSampleID;
                    _cm.UpdateCRFStatus(_cm);
                    _cm.sampleId = Convert.ToInt32(ID);
                    _cm.updateSampleStatus(_cm);

                    //Change Quotation status to 'Sample Accepted By Lab'
                    _cm.QuotationStatusID = 1014;//Sample Accepted By Lab
                    _cm.UpdateQuotationStatus(_cm);

                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestReport/TestReport-" + TestResultId + SampleId + ".htm"));
                    divTestResult.InnerHtml = html;
                    GC.Collect();
                    //Response.Redirect("SampleList.aspx");
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }

            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        protected void GenerateQR(string QR, string ReportNo)
        {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            var result = QCwriter.Write(QR);//QCwriter.Write(lblBatchNumber.Text);
            string path = Server.MapPath("~/TestReport/MyQR.jpg");
            var barcodeBitmap = new Bitmap(result);

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path,
                   FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
                string sourceFile = Server.MapPath("~/TestReport/MyQR.jpg");
                string destinationFile = Server.MapPath("~/TestReport/" + ReportNo + ".jpg");
                try
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }
        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            Response.AppendHeader("content-disposition", "attachment;filename=ExportedHtml.xls");

            Response.Charset = "";

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.ContentType = "application/vnd.ms-excel";

            this.EnableViewState = false;

            Response.Write(divTestResult.InnerHtml);

            Response.End();
        }
    }
}