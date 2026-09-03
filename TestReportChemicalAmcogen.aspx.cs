using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class TestReportChemicalAmcogen : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CloneCRFSampleID = 0;
        DAL.mailer objEmail = new DAL.mailer();
        string PreparedBy = "";string CheckedBy = "";string ApprovedBy = "";
        string PreparedByDesignation = ""; string CheckedByDesignation = ""; string ApprovedByDesignation = "";
        string PreparedByDate = ""; string CheckedByDate = ""; string ApprovedByDate = "";
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
                        string TemplateName = ""; string ReportType = "";string TestParameter = "";
                        StringBuilder sb = new StringBuilder();
                        
                        DataTable dt1 = _cm.GetTestResults(SampleId);
                        DataTable dt2 = _cm.GetDistinctTestParameters(SampleId);
                        DataTable dt4 = _cm.GetSampleHistory(SampleId);
                        string DateOfMfg = "";string DateOfExp = "";
                        if (dt1.Rows[0]["DateOfMfg"].ToString() != "")
                        {
                            DateOfMfg = Convert.ToDateTime(dt1.Rows[0]["DateOfMfg"]).ToString("MMM.yyyy");// DateTime.ParseExact(dt1.Rows[0]["DateOfMfg"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MMM.yyyy").ToUpper();
                        }
                        if (dt1.Rows[0]["DateOfExp"].ToString() != "")
                        {
                            DateOfExp = Convert.ToDateTime(dt1.Rows[0]["DateOfExp"]).ToString("MMM.yyyy"); // DateTime.ParseExact(dt1.Rows[0]["DateOfExp"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MMM.yyyy").ToUpper();
                        }
                        

                        sb.Append("<table><thead class='table-header'><tr><td colspan='5' style='text-align:center;text-align:center;border:none !important; border-left-style: hidden !important;border-right-style: hidden !important;border-top-style: hidden !important;font-size:20px;'>AMCOGEN ALLIANCES PVT LTD.<img src='logo.png' alt='logo' style='float:right'/></td></tr>");
                        sb.Append("<tr> <td colspan='5' style='text-align:center;'>CERTIFICATION OF ANALYSIS</td> </tr>");
                        sb.Append("<tr>	<td>Product Name </td><td colspan='4'>" + dt1.Rows[0]["SampleName"].ToString() +"</td>	</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='width:5%'>Grade</td><td colspan='2'  style='width:25%'>" + dt1.Rows[0]["TestMethod"].ToString() +"</td><td style='width:25%'>Date of Release</td><td style='width:25%'>" + Convert.ToDateTime(dt1.Rows[0]["IssueDate"]).ToString("dd.MM.yyyy") +"</td>");
                        sb.Append("</tr><tr>");
                        sb.Append("<td>Batch No.</td><td colspan='2'>" + dt1.Rows[0]["BatchNo"].ToString() +"</td><td>Mfg. Date</td><td>" + DateOfMfg +"</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td>Batch Size</td><td colspan='2'>" + dt1.Rows[0]["SizeOfSample"].ToString() +"</td><td>Exp. Date	</td><td>" + DateOfExp + "</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='border:none !important; border-left-style: hidden !important;'>&nbsp;</td>");
                        sb.Append("<td style='border:none !important;'></td>");
                        sb.Append("<td style='border:none !important;'></td>");
                        sb.Append("<td style='border:none !important; border-right-style: hidden !important;'></td>");
                        sb.Append("</tr>");
                        sb.Append("</thead>   <tbody>   <tr>");
                        sb.Append("<td style='width:10%'>  S. No</td><td style='width:30%'>	TEST NAME</td><td colspan='2' style='width:30%'>	SPECIFICATIONS	</td><td style='width:30%'>	RESULTS</td>");
                        sb.Append("</tr>");

                        if ((dt1.Rows.Count > 0)&&(dt2.Rows.Count>0))
                        {
                            int count = 1;
                            foreach (DataRow dr in dt2.Rows)
                            {
                                DataTable dt3 = _cm.GetTestResults(SampleId, dr["TestParameter"].ToString());
                                if (dt3.Rows.Count > 0)
                                {
                                    int sub = 0;
                                    foreach (DataRow dr2 in dt3.Rows)
                                    {
                                        if (dr["Total"].ToString() == "1")
                                        {
                                            sb.Append("<tr><td>" + count + "</td><td>" + dr2["TestParameter"].ToString() + "</td><td colspan='2'>" + dr2["Specification2"].ToString() + "</td><td>" + dr2["TestResult"].ToString() + " " + dr2["UnitOfMeasurement"].ToString() + "</td></tr>");
                                        }
                                        else
                                        {
                                            if (sub == 0)
                                            {
                                                sb.Append("<tr><td>" + count + "</td><td colspan='4'><b>" + dr2["TestParameter"].ToString() + "</b></td></tr>");
                                                sb.Append("<tr><td>" + "" + "</td><td>" + dr2["TestSubParameter"].ToString() + "</td><td colspan='2'>" + dr2["Specification2"].ToString() + "</td><td>" + dr2["TestResult"].ToString() + " " + dr2["UnitOfMeasurement"].ToString() + "</td></tr>");
                                            }
                                            else
                                            {
                                                sb.Append("<tr><td>" + "" + "</td><td>" + dr2["TestSubParameter"].ToString() + "</td><td colspan='2'>" + dr2["Specification2"].ToString() + "</td><td>" + dr2["TestResult"].ToString() + " " + dr2["UnitOfMeasurement"].ToString() + "</td></tr>");
                                            }
                                            sub++;
                                        }
                                        
                                    }
                                    
                                }
                                count++;
                            }
                        }
                        if(dt4.Rows.Count > 0)
                        {
                            foreach(DataRow dr in dt4.Rows)
                            {
                                switch (dr["Activity"].ToString())
                                {
                                    case "Sample Results Updated":
                                        PreparedBy = dr["EmployeeName"].ToString();
                                        PreparedByDesignation = dr["DesignationName"].ToString();
                                        PreparedByDate = Convert.ToDateTime(dr["CreatedDate"]).ToString("dd.MM.yyyy");
                                        break;
                                    case "Sample Test Report Generated":
                                        CheckedBy = dr["EmployeeName"].ToString();
                                        CheckedByDesignation = dr["DesignationName"].ToString();
                                        CheckedByDate = Convert.ToDateTime(dr["CreatedDate"]).ToString("dd.MM.yyyy");
                                        break;
                                    case "Sample Report Accepted":
                                        ApprovedBy = dr["EmployeeName"].ToString();
                                        ApprovedByDesignation = dr["DesignationName"].ToString();
                                        ApprovedByDate = Convert.ToDateTime(dr["CreatedDate"]).ToString("dd.MM.yyyy");
                                        break;
                                    default: break;
                                }
                            }
                        }
                        sb.Append("<tr><td colspan='5' style='text-align:center'>Remarks: The Product Complies As Per Above Specification</td></tr>");
                        sb.Append("</tbody>");
                        sb.Append("<tfoot class='table-footer'>");
                        sb.Append("<tr>");
                        sb.Append("<td style='border:none !important; border-left-style: hidden !important;'>&nbsp;</td>");
                        sb.Append("<td style='border:none !important;'></td>");
                        sb.Append("<td style='border:none !important; border-right-style: hidden !important;'></td>");
                        sb.Append("<tr>");
                        sb.Append("<td></td><td colspan='2'>Prepared By	</td><td>Checked By</td><td>	Approved By</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td>Name </td><td colspan='2'>"+PreparedBy+"</td><td>"+CheckedBy+"</td><td>"+ApprovedBy+"</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<tr>");
                        sb.Append("<td>Designation</td><td colspan='2'>" + PreparedByDesignation + "</td><td>" + CheckedByDesignation + "</td><td>" + ApprovedByDesignation + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td>Signature</td><td colspan='2'></td><td></td><td></td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td>Date</td><td colspan='2'>" + PreparedByDate + "</td><td>" + CheckedByDate + "</td><td>" + ApprovedByDate + "</td>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='5' style='text-align:center;border:none !important; border-left-style: hidden !important;border-right-style: hidden !important;border-bottom-style: hidden !important;'>");
                        sb.Append("<br/><hr>Corporate Office: Level 2/9, Electronic Zone, Electronic Sadan, Mahape, Navi Mumbai - 400708, India. <br/>Telephone no: 022-66735877 / 66736757</td></tr>");
                        sb.Append("</tfoot></table>");
                        ReportType = "NABL";

                        switch (ReportType)
                        {
                            case "NABL": TemplateName = "AmcogenTestReportChemical.html"; break;
                            case "NON NABL": TemplateName = "AmcogenTestReportChemicalNonNABL.html"; break;
                        }
                        string body = objEmail.PrepareMailBodyWith(TemplateName, "{TestResult}", sb.ToString());
                        string str = body.ToString();
                        string filename = Server.MapPath("//TestReport/TestReport-" + TestResultId + SampleId + ".htm");
                        System.IO.File.WriteAllText(@filename, str);

                        DataTable dt7 = _cm.CheckSignatory(EmployeeID);
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
                        _cm.sampleId = Convert.ToInt32(SampleId);
                        _cm.updateSampleStatus(_cm);

                        string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestReport/TestReport-" + TestResultId + SampleId + ".htm"));
                        divTestResult.InnerHtml = html;
                        GC.Collect();
                        //Response.Redirect("SampleList.aspx");
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
    }
}