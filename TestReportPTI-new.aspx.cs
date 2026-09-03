using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class TestReportPTI : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
           // Session["SampleId"] = 41302324;
            Session["TestResultId"] = 5;

            string SampleId = Session["SampleId"].ToString();
            string TestResultId = Session["TestResultId"].ToString();
            if (Session["TestResultId"] != null)
            {
                string TestReportNo = "";
                string ULRNo = "";
                string IssueDate = "";
                string LabSampleId = "";

                string Department = "";
                string ProductCategory = "";
                string CustomerName = "";
                string Address = "";
                string NoOfSample = "";
                string ReceivedDate = "";

                string Model = "";
                string SrNo = "";
                string CalibrationCertificateNo = "";
                string CalibrationCertificateDate = "";
                string CalibrationDueDate = "";
                string AnalysisStartDate = "";
                string AnalysisCompletionDate = "";
                string MaterialDetails = "";
                string Mobile = "";
                string Email = "";
                string ContactPerson = "";
                string SampleName = "";
                string Remarks = "";
                string TestResult = "";
                string PETResin = "";
                string Preforms = "";
                string Bottles = "";
                string SizeOfSample = "";
                string Sr = "";
                DataTable dt3 = new DataTable();

                DataTable dt1 = _cm.GetTestReportDetails1(SampleId);
                DataTable dt4 = _cm.GetTestReportDetails1(SampleId);
                if (dt1.Rows.Count > 0)
                {
                    Department = dt1.Rows[0]["DepartmentName"].ToString();
                    ProductCategory = dt1.Rows[0]["SubGroupName"].ToString();                    
                    Address = dt1.Rows[0]["AddressLine1"].ToString();
                    NoOfSample = dt1.Rows[0]["SizeOfSample"].ToString();
                    ReceivedDate = dt1.Rows[0]["ReceivedDate"].ToString();
                    SampleId = dt1.Rows[0]["SampleId"].ToString();
                }
                if (dt4.Rows.Count > 0)
                {
                    CustomerName = dt4.Rows[0]["CustomerName"].ToString();
                    ContactPerson = dt4.Rows[0]["ContactPerson"].ToString();
                    Mobile = dt4.Rows[0]["Mobile"].ToString();
                    Email = dt4.Rows[0]["Email"].ToString();
                }
                    DataTable dt2 = _cm.GetTestResultDetailsMechanical(TestResultId);
                if (dt2.Rows.Count > 0)
                {
                    TestReportNo = dt2.Rows[0]["TestReportNo"].ToString();
                    ULRNo = dt2.Rows[0]["ULRNo"].ToString();
                    DateTime DateOfIsuue = Convert.ToDateTime(dt2.Rows[0]["IssueDate"].ToString());
                    IssueDate = DateOfIsuue.ToString("dd/MMM/yyyy");
                    Model = dt2.Rows[0]["Model"].ToString();
                    SrNo = dt2.Rows[0]["SrNo"].ToString();
                    CalibrationCertificateNo = dt2.Rows[0]["CalibrationCertificateNo"].ToString();
                    DateTime calCertiDate = Convert.ToDateTime(dt2.Rows[0]["CalibrationCertificateDate"].ToString());
                    DateTime calDueDate = Convert.ToDateTime(dt2.Rows[0]["CalibrationDueDate"].ToString());
                    CalibrationCertificateDate = calCertiDate.ToString("dd/MMM/yyyy");
                    CalibrationDueDate = calDueDate.ToString("dd/MMM/yyyy");
                    //DateTime AnalysisStartingDate =Convert.ToDateTime( dt2.Rows[0]["AnalysisStartingDate"].ToString());
                    //AnalysisStartDate = AnalysisStartingDate.ToString("dd/MMM/yyyy");
                    //DateTime AnalysisCompleteDate= Convert.ToDateTime(dt2.Rows[0]["AnalysisCompletionDate"].ToString());
                    //AnalysisCompletionDate = AnalysisCompleteDate.ToString("dd/MMM/yyyy");
                    MaterialDetails = dt2.Rows[0]["ReferenceMaterialDetails"].ToString();
                }
                StringBuilder sb = new StringBuilder();
                StringReader sr = new StringReader(sb.ToString());

                dt3.Columns.AddRange(new DataColumn[3] {
                    new DataColumn("TestParameter", typeof(string)),
                    new DataColumn("TestResult",typeof(string)),
                    new DataColumn("TestMethod",typeof(string)),});

                dt3 = _cm.GetTestReportResultPTI(SampleId);

                sb.Append("<table border='1' style='border-spacing:0px; width:100%' cellpadding='5'>");

                //Adding HeaderRow.
                sb.Append("<tr>");
                sb.Append("<td rowspan='2' style='width:5%'><b>Sr.No.</b></td>");
                sb.Append("<td rowspan='2' style='width:20%'><b>Sample ID</b></td>");
                sb.Append("<td colspan='3' style='width:20%'><b>Description of Test</b></td>");
                sb.Append("<td rowspan='2' style='width:20%'><b>No of Samples (If Resin then No of Pouches/ Wt of sample,If Preform and/or  Bottles then specify Nos)</b></td>");
                sb.Append("<td style='width:15%'><b>Remark<br/>Or any other relevant information regarding the test / contract</b></td>");

                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='width:5%'><bFor PET resin: </b>Colour,Moisture content,Solution intrinsic viscosity(SIV), Acetaldehyde content Test(AAC), FTIR, DSC.</td>");
                sb.Append("<td style='width:5%'><bFor Preforms: </b>Finish dimensions, Concentricity, Check for defects & all PET resin tests FTIR, DSC</td>");
                sb.Append("<td style='width:5%'><bFor Bottles: </b>Colour-Haze,Section weights,Drop impact, Stress crack resistance, Volume and fill height, Top load resistance, Bottle expansion and burst & all PET resin & Preforms tests, FTIR, DSC.</td>");
                sb.Append("</tr>");

                for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                {
                    sb.Append("<tr>");
                    {
                        Sr = dt3.Rows[freqCount - 1]["Sr"].ToString();
                        SampleName = dt3.Rows[freqCount - 1]["SampleName"].ToString();
                        LabSampleId = dt3.Rows[freqCount - 1]["Lab ID"].ToString();
                        PETResin = dt3.Rows[freqCount - 1]["Description1"].ToString();
                        Preforms = dt3.Rows[freqCount - 1]["Description2"].ToString();
                        Bottles = dt3.Rows[freqCount - 1]["Description2"].ToString();
                        SizeOfSample = dt3.Rows[freqCount - 1]["SizeOfSample"].ToString();
                        Remarks = dt3.Rows[freqCount - 1]["Remarks"].ToString();

                        sb.Append("<td style='width:5%;'>" + Sr + "</td>");
                        sb.Append("<td style='width:20%;'>" + SampleName + "</td>");
                        sb.Append("<td style='width:20%;'>" + PETResin + "</td>");
                        sb.Append("<td style='width:15%;'>" + Preforms + "</td>");
                        sb.Append("<td style='width:15%;'>" + Bottles + "</td>");
                        sb.Append("<td style='width:15%;'>" + SizeOfSample + "</td>");
                        sb.Append("<td style='width:15%;'>" + Remarks + "</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                sb.Append("<br />");
                //Table end.

                string body = objEmail.PrepareMailBodyWith("TestReportPTI.html", "{ULRNo}", ULRNo, "{ReportNo}", TestReportNo, "{DateOfIssue}", IssueDate, "{Department}", Department, "{ProductGroup}", ProductCategory, "{CustomerName}", CustomerName, "{Address}", Address, "{NoOfSample}", NoOfSample, "{ReceivedDate}", ReceivedDate, "{LabSampleId}", LabSampleId, "{Model}", Model, "{SrNo}", SrNo, "{TestDateWVTR}", "{CalibratedCertificateNo}", CalibrationCertificateNo, "{CalibratedCertificateDate}", CalibrationCertificateDate, "{CalibrationDueOn}", CalibrationDueDate, "{CalibrationBy}", "{SampleId}", SampleId, "{StartDate}", AnalysisStartDate, "{CompletionDate}", AnalysisCompletionDate, "{MaterialDetails}", MaterialDetails, "{Mobile}", Mobile, "{Email}", Email, "{ContactPerson}", ContactPerson, "{TestResult}",sb.ToString());
                string str = body.ToString();
                string filename = Server.MapPath("//TestReport/TestReport-" + TestResultId + SampleId + ".htm");
                System.IO.File.WriteAllText(@filename, str);
				GC.Collect();
                string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestReport/TestReport-" + TestResultId + SampleId + ".htm"));
                divTestResults.InnerHtml = html;
            }
        }
    }
}