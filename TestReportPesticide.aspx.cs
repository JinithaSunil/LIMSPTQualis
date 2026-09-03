using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
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
    public partial class TestReportPesticide : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CloneCRFSampleID = 0;
        DAL.mailer objEmail = new DAL.mailer();
        string Signatory = ""; string Designation = "";
        DateTime DateOfIsuue = new DateTime();
        string ReportName = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["Usertype"] != null)
            {
                if (Session["EmployeeID"] != null)
                {
                    string EmployeeID = Session["EmployeeID"].ToString();
                }
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
                    string SignatoryEmployeeID = "";
                    if (Session["CloneCRFSampleID"] != null)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                    }
                    if (Session["SampleId"] != null)
                    {
                        string SampleId = Session["SampleId"].ToString();
                        string TestResultId = Session["TestResultId"].ToString();

                        string ULRNo = "";
                        string IssueDate = "";
                        string LabSampleId = "";
                        string inquiryDate = "";
                        string CRFReceivedDate = "";

                        string Department = "";
                        string ProductCategory = "";
                        string CustomerName = "";
                        string Address = "";
                        string NoOfSample = "";
                        string ReceivedDate = "";
                        string ContactPerson = "";
                        string ContactNumber = "";
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
                        string SamplingMethod = "NA";
                        string SamplingLocation = "";
                        string TransportAndReceiptTemp = "";
                        string DateOfSampling = "";
                        string Specification = "";
                        string TestParameter = "";
                        string TestResult = "";
                        string TestMethod = "";
                        string TestReportNo = "";
                        string Description = "";
                        string Sr = "";
                        string Unit = "";
                        string inquiryID = "";
                        string CustomerReference = "";
                        string Remarks = "";
                        string SignatoryID = "";
                        string SignatoryName = "";
                        string SignatoryDesignation = "";
                        string ReportType = "";
                        string TemplateName = "";
                        string ReportFormat = "";
                        string sampleUID = "";
                        string Temperature = "";
                        string Humidity = "";
                        string Basis = "";
                        string LoadingDate = "";
                        string NoOfTrucks = "";
                        string LoadedQty = "";
                        string TotalMoisture = "";
                        string SamplingBy = "";
                        string SizeOfSample = "";
                        string Environmental = "NA";
                        string Deviation = "NA";
                        string SampleQty = "NA";
                        string ItemSampled = "NA";
                        StringBuilder sbCoal = new StringBuilder();
                        DataTable dt3 = new DataTable();

                        DataTable dt1 = _cm.GetTestReportDetails1(SampleId);
                        DataTable dt4 = _cm.GetTestReportDetails2(SampleId);
                        StringBuilder sbAddress = new StringBuilder();
                        DataTable dt8 = _cm.GetAbbreviation(SampleId);

                        string type = "NA";

                        DataTable dtReport = _cm.GetReportFormat(Session["SampleId"].ToString());
                        
                        if (Request.QueryString["t1"] != null)
                        {
                            type = Request.QueryString["t1"].ToString();
                            if (type == "1") { type = "Pesticides"; }
                            if (type == "0") { type = "Non NABL"; }
                        }

                        StringBuilder sbAbbrev = new StringBuilder();
                        if (dt8.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt8.Rows)
                            {
                                sbAbbrev.Append(dr["Abbrev"].ToString() + "</br>");
                            }
                        }
                        if (dt1.Rows.Count > 0)
                        {
                            SamplingBy = dt1.Rows[0]["SamplingBy"].ToString();
                            SizeOfSample = dt1.Rows[0]["SizeOfSample"].ToString();
                            Department = dt1.Rows[0]["DepartmentName"].ToString();
                            ProductCategory = dt1.Rows[0]["subgroupname"].ToString();
                            Temperature = _cm.ReplaceSuperScript(dt1.Rows[0]["TransportAndReceiptTemp"].ToString());
                            Humidity = dt1.Rows[0]["Humidity"].ToString();
                            if (ProductCategory == "Coal")
                            {
                                sbCoal.Append("<b>Environmental Condition:</b> " + Humidity + " humidity and " + Temperature + " temperature was maintained during the analysis.");
                            }
                            NoOfSample = dt1.Rows[0]["SizeOfSample"].ToString();

                            DateTime ReceivedDt = Convert.ToDateTime(dt1.Rows[0]["ReceivedDate"].ToString());
                            ReceivedDate = ReceivedDt.ToString("dd.MM.yyyy");
                            DateTime InquiryDt = Convert.ToDateTime(dt1.Rows[0]["InquiryDate"].ToString());
                            inquiryDate = InquiryDt.ToString("dd.MM.yyyy");
                            SampleId = dt1.Rows[0]["SampleId"].ToString();
                            LabSampleId = dt1.Rows[0]["SampleIdSeries"].ToString();
                            DateTime date = Convert.ToDateTime(dt1.Rows[0]["Created"].ToString());
                            Dated = date.ToString("dd.MM.yyyy");
                            BatchNo = dt1.Rows[0]["BatchNo"].ToString();
                            inquiryID = dt1.Rows[0]["inquiryID"].ToString();
                            CloneCRFSampleID = Convert.ToInt32(dt1.Rows[0]["CloneCRFSampleID"].ToString());
                        }
                        if (dt4.Rows.Count > 0)
                        {
                            CustomerName = dt4.Rows[0]["CustomerName"].ToString();
                            ContactPerson = dt4.Rows[0]["ContactPerson"].ToString();
                            ContactNumber = dt4.Rows[0]["Mobile"].ToString();
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
                            //if (ContactPerson != "") { sbAddress.Append("<br/>Contact Person: " + ContactPerson); }
                            //if (ContactNumber != "") { sbAddress.Append(" <br/>Mobile: " + ContactNumber); }
                            //if (Email != "") { sbAddress.Append(" <br/>Email: " + Email); }
                        }
                        string ReportName = dtReport.Rows[0]["ReportName"].ToString();

                        if (dtReport.Rows.Count > 0)
                        {
                            DataTable dt2 = new DataTable();
                            switch (ReportName)
                            {
                                case "Pesticides":
                                case "NonNABL":
                                    dt2 = _cm.GetTestResultDetailsChemical(TestResultId, 1);
                                    if (dt2.Rows.Count > 0)
                                    {
                                        ULRNo = dt2.Rows[0]["ULRNo"].ToString();
                                        if (type == "Non NABL")
                                        {
                                            TestReportNo = dt2.Rows[0]["TestReportNo"].ToString();
                                            TestReportNo += "_NN";
                                        }

                                        DateOfIsuue = Convert.ToDateTime(dt2.Rows[0]["IssueDate"].ToString());
                                        IssueDate = DateOfIsuue.ToString("dd.MM.yyyy");
                                        DateTime CRFdate = Convert.ToDateTime(dt2.Rows[0]["CRFdate"].ToString());
                                        CRFReceivedDate = CRFdate.ToString("dd.MM.yyyy");
                                        DateTime StartDate = Convert.ToDateTime(dt2.Rows[0]["AnalysisStartingDate"].ToString());
                                        AnalysisStartingDate = StartDate.ToString("dd.MM.yyyy");
                                        DateTime EndDate = Convert.ToDateTime(dt2.Rows[0]["AnalysisCompletionDate"].ToString());
                                        AnalysisCompletionDate = EndDate.ToString("dd.MM.yyyy");
                                        NameOfProduct = dt2.Rows[0]["NameOfProduct"].ToString();
                                        DateOfMfg = dt2.Rows[0]["DateOfMfg"].ToString();
                                        DateOfExp = dt2.Rows[0]["DateOfExp"].ToString();
                                        TestRequired = dt2.Rows[0]["TestRequired"].ToString();
                                        SampleCondition = dt2.Rows[0]["SampleCondition"].ToString();
                                        Sampling = dt2.Rows[0]["Sampling"].ToString();
                                        SamplingLocation = dt2.Rows[0]["SamplingLocation"].ToString();
                                        TransportAndReceiptTemp = dt2.Rows[0]["TransportAndReceiptTemp"].ToString();
                                        if ((dt2.Rows[0]["DateOfSampling"].ToString() != "") && (dt2.Rows[0]["DateOfSampling"].ToString() != "-") && (dt2.Rows[0]["DateOfSampling"].ToString() != "NA"))
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
                                        SampleId = dt2.Rows[0]["SampleID"].ToString();
                                        sampleUID = dt2.Rows[0]["sampleUID"].ToString();
                                        Session["sampleUID"] = sampleUID;
                                    }
                                    break;
                                default: break;
                            }
                        }
                        int ID = 0;
                        StringBuilder sb = new StringBuilder();
                        StringReader sr = new StringReader(sb.ToString());
                        if (sampleUID != "")
                        {
                            DataTable dt5 = _cm.getSampleIDFromSampleUID(Convert.ToInt32(sampleUID));
                            if (dt5.Rows.Count > 0)
                            {
                                ID = Convert.ToInt32(dt5.Rows[0]["id"].ToString());
                            }
                            dt3.Columns.AddRange(new DataColumn[4] {
                    new DataColumn("TestParameter", typeof(string)),
                    new DataColumn("TestResult",typeof(string)),
                    new DataColumn("Specification",typeof(string)),
                    new DataColumn("DescriptionOfTest",typeof(string)),});

                            if (dtReport.Rows.Count > 0)
                            {
                                sb.Append("<table style='border-spacing:0px;width:100%;' cellspacing='0' cellpadding='0' class='tab1'>");
                                sb.Append("<tr>");
                                ReportName = dtReport.Rows[0]["ReportName"].ToString();
                                switch (ReportName)
                                {
                                    case "Pesticides":
                                    case "NonNABL":
                                        sb.Append("<th style='width:5%'>S.No</th><th style='width:30%'>Parameters Tested</th><th style='width:15%'>Unit of Measurement (UOM)</th><th style='width:30%'>Method Used</th><th style='width:10%'>Requirement/<br> Specification<br> (If Any)</th><th style='width:10%'>Test Result</th>");
                                        break;
                                    default: break;
                                }

                                sb.Append("</tr>");
                            }

                            dt3 = _cm.GetTestReportResult(Session["SampleId"].ToString(), type);
                            if (dt3.Rows.Count > 0)
                            {
                                for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                                {
                                    sb.Append("<tr>");
                                    {
                                        Sr = dt3.Rows[freqCount - 1]["Sr"].ToString();
                                        TestParameter = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                                        TestResult = dt3.Rows[freqCount - 1]["TestResult"].ToString();
                                        Unit = dt3.Rows[freqCount - 1]["UnitOfMeasurement"].ToString();
                                        TestResult = _cm.ReplaceSuperScript(TestResult);
                                        Unit = _cm.ReplaceSuperScript(Unit);
                                        //Basis = dt3.Rows[freqCount - 1]["Basis"].ToString();
                                        TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                                        Specification = dt3.Rows[freqCount - 1]["Specification"].ToString();
                                        switch (ReportName)
                                        {
                                            case "Pesticides":
                                            case "NonNABL":
                                                sb.Append("<td style='width:5%;border:1px solid; text-align:center'>" + Sr + "</td>");
                                                sb.Append("<td style='width:20%;border:1px solid; text-align:left'>" + TestParameter + "</td>");
                                                sb.Append("<td style='width:15%;border:1px solid; text-align:center'>" + Unit + "</td>");
                                                sb.Append("<td style='width:15%;border:1px solid; text-align:center'>" + TestMethod + "</td>");
                                                sb.Append("<td style='width:20%;border:1px solid; text-align:center'>" + Specification + "</td>");
                                                sb.Append("<td style='width:20%;border:1px solid; text-align:center'>" + TestResult + "</td>");
                                                break;
                                            default: break;
                                        }

                                    }
                                    sb.Append("</tr>");
                                }
                            }
                            sb.Append("</table>");
                            sb.Append("<br />");
                            //Table end.
                            DataTable dtReportFormat = _cm.GetTestReportNumber(ReportName);
                            if (dtReportFormat.Rows.Count > 0)
                            {
                                ReportFormat = dtReportFormat.Rows[0]["ReportFormatNumber"].ToString();
                            }
                            DataTable dt6 = _cm.getSignatory(SignatoryID);
                            if (dt6.Rows.Count > 0)
                            {
                                SignatoryName = dt6.Rows[0]["AuthorizedSignatoryName"].ToString();
                                SignatoryDesignation = dt6.Rows[0]["DesignationName"].ToString();
                                SignatoryEmployeeID = dt6.Rows[0]["EmployeeID"].ToString();
                            }
                            DataTable dt7 = _cm.CheckSignatory(EmployeeID);
                            StringBuilder sbSignature = new StringBuilder();
                            StringBuilder sbQRCode = new StringBuilder();
                            if (dt7.Rows.Count > 0)
                            {
                                SampleId = Session["SampleUID"].ToString();
                                //sbSignature.Append("<div id='SivaDiv'>");
                                sbSignature.Append("<div style='text-align:right;border:none !important; padding-right:1%;background:url(../assets/img/Signature/ValidSign.png) no-repeat;background-position: right;background-size:33%'>");
                                sbSignature.Append("<div style='text-align:right;border:none !important; padding-right:1%;'>");
                                sbSignature.Append("<img src='http://CiscoLIMS.rnisolutions.in:8608/assets/img/Signature/" + SignatoryEmployeeID + ".png' width='auto' height='60' style='padding-bottom:20%'>");
                                sbSignature.Append("<br/><span style='font-size:9px;width:20%; text-align:right !important; line-height:10px'>Date: " + DateOfIsuue.ToString("yyyy.MM.dd") + "<br/></span>");
                                //.. sbSignature.Append("<br/><span style='font-size:9px;width:20%; text-align:right !important; line-height:10px'>Date: " + DateTime.Now.AddHours(12.5).ToString("yyyy.MM.dd HH:mm:ss") + "<br/></span>");
                                sbSignature.Append("<br/><span style='font-size:9px;width:20%; text-align:right !important; line-height:10px'><br/></span>");
                                sbSignature.Append("<span>AUTHORIZED SIGNATORY</span><br/>");
                                sbSignature.Append("<span>" + SignatoryName + "</span><br/><span>(" + SignatoryDesignation + ")</span><br/></div>");
                                string QR = "http://CiscoLIMS.rnisolutions.in:8608/TestReport/TestReport-" + TestResultId + sampleUID + ".htm";
                                GenerateQR(QR, TestResultId + sampleUID);
                                sbQRCode.Append("<img src='../TestReport/" + TestResultId + sampleUID + ".jpg' width='auto' height='100'>");
                            }
                            else
                            {
                                SampleId = Session["SampleUID"].ToString();
                                sbSignature.Append("<div style='background-size:40%'>");
                                sbSignature.Append("<br/><br/><br/><br/>");
                                sbSignature.Append("<br/><span><br/></span>");
                                sbSignature.Append("<span>AUTHORIZED SIGNATORY</span><br/>");
                                sbSignature.Append("<span>" + SignatoryName + "</span><br/><span>(" + SignatoryDesignation + ")</span><br/></div>");
                                string QR = "http://CiscoLIMS.rnisolutions.in:8608/TestReport/TestReport-" + TestResultId + sampleUID + ".htm";
                                GenerateQR(QR, TestResultId + sampleUID);
                                sbQRCode.Append("<img src='../TestReport/" + TestResultId + sampleUID + ".jpg' width='auto' height='100'>");
                            }
                            TemplateName = ReportName + ".html";
                            //if (dtReport.Rows.Count > 0)
                            //{
                            //    switch (ReportType)
                            //    {
                            //        case "NABL": TemplateName = ReportName + ".html"; break;
                            //        case "NON NABL": TemplateName = ReportName + "NonNABL.html"; break;
                            //    }
                            //}
                            string CargoDescription = "";
                            string Receiver = "";
                            string ModeOfTransport = "";
                            string PlaceOfLoading = "";
                            string RRNo = "";
                            string RRDate = "";
                            string RRQty = "";
                            string StackQty = "";

                            StringBuilder sbSampling = new StringBuilder();
                            DataTable dt10 = _cm.GetOrganizationDetails();
                            if (dt10.Rows.Count > 0)
                            {
                                if ((type == "NABL")|| (type == "Pesticides"))
                                {
                                    sbSampling.Append("<table style='text-align:center; width:100%'><tr><td width='15%'></td><td width='60%'><h3>" + dt10.Rows[0]["Name"].ToString() +"</h3>(TESTING & ANALYTICAL SERVICES)<br/><hr>NABL Accredited Laboratory( as per ISO/IEC 17025:2017)<br/>" + dt10.Rows[0]["Addressline1"].ToString() + " <br/>" + dt10.Rows[0]["Addressline2"].ToString() + ", Dist:" + dt10.Rows[0]["district"].ToString() + " - " + dt10.Rows[0]["pincode"].ToString() + ", " + dt10.Rows[0]["state"].ToString() + ", India<br>Mobile:" + dt10.Rows[0]["Phone"].ToString() + ". Email: " + dt10.Rows[0]["Email"].ToString() + ".Website:" + dt10.Rows[0]["Website"].ToString() + "</td><td width='25%' style='float:right'><img src='../images/NABL.png' height='100' width='auto' /><br/><b>" + dt10.Rows[0]["NABLCode"].ToString() + "</b><br/></td></tr></table><br/><h3>Test Report</h3><br>");
                                    sbSampling.Append("<table style='width:100%; border-collapse:collapse' class='tab'>");

                                    sbSampling.Append("<tr><td style='border-bottom:1px solid'><b>ULR No:</b></td><td style='border-bottom:1px solid'>" + ULRNo + "</td><td style='border-bottom:1px solid'></td><td style='border-bottom:1px solid'></td></tr>");
                                    sbSampling.Append("<tr><td><b>Discipline:</b></td><td>" + Department + "</td><td><b>Report No:</b></td><td>" + TestReportNo + "</td></tr>");
                                    sbSampling.Append("<tr><td><b>Product Group:</b></td><td>" + ProductCategory + "</td><td><b>Report Issue Date:</b></td><td>" + IssueDate + "</td></tr>");
                                    sbSampling.Append("</table><br />");
                                }
                                else
                                {
                                    sbSampling.Append("<table style='text-align:center; width:100%'><tr><td width='10%'></td><td width='90%'><h3>" + dt10.Rows[0]["Name"].ToString() +"</h3>(TESTING & ANALYTICAL SERVICES)<br/><hr>NABL Accredited Laboratory( as per ISO/IEC 17025:2017)<br/>" + dt10.Rows[0]["Addressline1"].ToString() + " <br/>" + dt10.Rows[0]["Addressline2"].ToString() + ", Dist:" + dt10.Rows[0]["district"].ToString() + " - " + dt10.Rows[0]["pincode"].ToString() + ", " + dt10.Rows[0]["state"].ToString() + ", India<br>Mobile:" + dt10.Rows[0]["Phone"].ToString() + ". Email: " + dt10.Rows[0]["Email"].ToString() + ".Website:" + dt10.Rows[0]["Website"].ToString() + "</td><td width='25%' style='float:right'><br/><br/></td></tr></table><br/><h3>Test Report</h3><br>");
                                    sbSampling.Append("<table style='width:100%; border-collapse:collapse' class='tab'>");

                                    sbSampling.Append("<tr><td style='border-bottom:1px solid'><b>Report No:</b>" + TestReportNo + "</td></tr>");
                                    sbSampling.Append("<tr><td><b>Report Issue Date:</b>" + IssueDate + "</td></tr>");
                                    sbSampling.Append("</table><br />");
                                }
                                sbSampling.Append("<table style='width:100%;border:none; border-collapse:collapse' cellspacing='0' cellpadding='0' border='0'>");
                                sbSampling.Append("<tr><td style='width:50%'> <b>Customer Name:</b> " + CustomerName + "</td><td style='width:50%'><b> Customer Address: </b>" + sbAddress.ToString() + "</td></tr>");
                                sbSampling.Append("<tr><td> <b> Kind Attention:</b>" + ContactPerson + "</td></tr></table><br/>");

                                sbSampling.Append("<center><b><u>Sample Details</u></b></center><br />");
                                sbSampling.Append("<table style='width:100%; border:1px solid black; border-spacing:5px;' cellspacing='0' cellpadding='0'>");
                                sbSampling.Append("<tr><td width='24%'>Name of the Sample</td><td width='25%'>: " + NameOfProduct + "</td><td width='25%'>Sample Received Date	:</td><td width='25%'>: " + ReceivedDate + "</td></tr>");
                                sbSampling.Append("<tr><td> Sample Type</td><td>: " + ProductCategory + "</td><td>Sample Registration Date	:</td><td>: " + Dated + "</td></tr>");
                                sbSampling.Append("<tr><td>Unique Identification No	:</td><td>: " + TestReportNo + "</td><td>Sample Quantity Received	:</td><td>: " + SizeOfSample + "</td></tr>");
                                sbSampling.Append("<tr><td>Mark on Sample	:</td><td>: " + BatchNo + "</td></tr></table><br/>");

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
                           "{SampleId}", SampleId,
                           "{StartDate}", AnalysisStartingDate,
                           "{CompletionDate}", AnalysisCompletionDate,
                           "{ContactPerson}", ContactPerson,
                           "{ContactNumber}", ContactNumber,
                           "{Date}", CRFReceivedDate,
                           "{BatchNo}", BatchNo,
                           "{SampleRegistrationDate}", Dated,
                           "{IdentificationNo}", TestReportNo,
                           "{TestRequired}", TestRequired,
                           "{SampleCondition}", SampleCondition,
                           "{Sampling}", Sampling,
                           "{SamplingMethod}", SamplingMethod,
                           "{DateOfMfg}", DateOfMfg,
                           "{ExpDate}", DateOfExp,
                           "{SampleQuantity}", SampleQuality,
                           "{NameOfProduct}", NameOfProduct,
                           "{TestResult}", sb.ToString(),
                           "{SignatoryName}", SignatoryName,
                           "{SignatoryDesignation}", SignatoryDesignation,
                           "{CustomerReference}", CustomerReference,
                           "{Remarks}", Remarks,
                           "{ReportFormatNumber}", ReportFormat,
                       "{Signature}", sbSignature.ToString(),
                       "{SamplingLocation}", SamplingLocation.ToString(),
                       "{TransportAndReceiptTemp}", TransportAndReceiptTemp.ToString(),
                       "{DateOfSampling}", DateOfSampling.ToString(),
                       "{QRCode}", sbQRCode.ToString(),
                       "{EnvironmentalCondition}", sbCoal.ToString(),
                       "{Abbrev}", sbAbbrev.ToString(),
                       "{Sample1}", sbSampling.ToString());
                            string str = body.ToString();
                            string filename = "";string file = "";
                            if ((type == "NABL")||(type == "NA")|| (type == "Pesticides"))
                            {
                                file="//TestReport/TestReport-" + TestResultId + SampleId + ".htm";
                            }
                            if (type == "Non NABL")
                            {
                                file = "//TestReport/TestReport-" + TestResultId + SampleId + "_NN.htm";
                            }
                            filename = Server.MapPath(file);
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

                            string html = File.ReadAllText(HttpContext.Current.Server.MapPath(file));
                            divTestResult.InnerHtml = html;
                            GC.Collect();
                            //Response.Redirect("SampleList.aspx");
                        }



                    }
                    else
                    {
                        Response.Redirect("Default.aspx");
                    }
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
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