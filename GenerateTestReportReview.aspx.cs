using iTextSharp.text;
using LIMS.DAL;
using System;
using System.Collections;
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
    public partial class GenerateTestReportReview : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); string ReviewedBy = "";int ReviewCount = 0; string TestReportNo = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["EmployeeName"] != null)
                {
                    ReviewedBy = Session["EmployeeName"].ToString();
                }
                if (Request.QueryString["s"] != null)
                {
                    Session["SampleId"] = Request.QueryString["s"].ToString();
                }
                if (Request.QueryString["c"] != null)
                {
                    Session["ReviewCount"] = Request.QueryString["c"].ToString();
                    ReviewCount = Convert.ToInt32(Session["ReviewCount"].ToString());
                }
                Session["Sample"] = Session["SampleId"];
                if (Session["Sample"] != null)
                {
                    string Checklist1 = "";
                    string Status = "";
                    string Remarks = "";

                    List<string[]> Accepted = new List<string[]>();
                    List<string[]> Rejected = new List<string[]>();
                    String SamplesId = "";
                    string SamplesIdSeries = "";
                    string SampleReceivedDate = "";
                    string Temperature = "";
                    string Humidity = "";
                    string SamplingMethod = "";
                    string ProductCategory = "";
                    string BatchNo = "";
                    string RequestedDate = "";
                    string InquiryId = "";
                    string Id = "";
                    string CustomerName = "";
                    string ReviewerID = "";
                    string ReviewerName = "";
                    string ReviewDate = "";

                    DataTable dt = new DataTable();
                    DataTable dt2 = new DataTable();
                    DataTable dt3 = new DataTable();
                    DataTable dt4 = new DataTable();
                    DataTable dt5 = new DataTable();
                    DataTable dt6 = new DataTable();
                    DataTable dt7 = new DataTable();
                    DataTable dt8 = new DataTable();

                    int SampleId = Convert.ToInt32(Session["Sample"].ToString());


                    StringBuilder sb = new StringBuilder();

                    mailer objEmail = new mailer();
                    StringReader sr = new StringReader(sb.ToString());

                    StringBuilder sbOrg = new StringBuilder();

                    dt8 = _cm.GetOrganizationDetails();
                    if (dt8.Rows.Count > 0)
                    {
                        sbOrg.Append("<h3 class='name'>" + dt8.Rows[0]["Name"].ToString() + "</h3>");
                        sbOrg.Append("<div>" + dt8.Rows[0]["AddressLine1"].ToString() + "</div>");
                        if (dt8.Rows[0]["AddressLine2"].ToString() != "")
                        {
                            sbOrg.Append("<div>" + dt8.Rows[0]["AddressLine2"].ToString() + ",</div>");
                        }
                        sbOrg.Append("<div>" + dt8.Rows[0]["District"].ToString() + "-" + dt8.Rows[0]["Pincode"].ToString() + "</div>");
                        sbOrg.Append("<div>" + dt8.Rows[0]["State"].ToString() + "- INDIA.</div>");
                        sbOrg.Append("<div>Phone : +91 " + dt8.Rows[0]["Phone"].ToString() + "</div>");
                        if (dt8.Rows[0]["email"].ToString() != "")
                        {
                            sbOrg.Append("<div>E-mail : <a href = 'mailto:'" + dt8.Rows[0]["email"].ToString() + ">" + dt8.Rows[0]["email"].ToString() + "</a>,</div>");
                        }
                        if (dt8.Rows[0]["website"].ToString() != "")
                        {
                            sbOrg.Append("<div><a href = '" + dt8.Rows[0]["website"].ToString() + " target= '_blank'>" + dt8.Rows[0]["website"].ToString() + "</a ></div>");
                        }
                        
                    }

                    dt2 = _cm.getSampleDetails(SampleId);
                    dt3 = _cm.GetCustomerInquiryDetails(SampleId);
                    if (dt2.Rows.Count > 0)
                    {
                        SamplesId = dt2.Rows[0]["SampleId"].ToString();
                        SamplesIdSeries = dt2.Rows[0]["SampleIdSeries"].ToString();
                        ReviewedBy = dt2.Rows[0]["SampleVerifiedBy"].ToString();
                        DateTime ReceivedDate = Convert.ToDateTime(dt2.Rows[0]["ReceivedDate"].ToString());
                        DateTime RequestDate = Convert.ToDateTime(dt2.Rows[0]["Created"].ToString());
                        SampleReceivedDate = ReceivedDate.ToString("dd/MMM/yyyy");
                        RequestedDate = RequestDate.ToString("dd/MMM/yyyy");

                        Temperature = _cm.ReplaceSuperScript(dt2.Rows[0]["TransportAndReceiptTemp"].ToString());
                        Humidity = dt2.Rows[0]["Humidity"].ToString();
                        SamplingMethod = dt2.Rows[0]["SamplingMethod"].ToString();
                    }
                    if (dt3.Rows.Count > 0)
                    {
                        string ProductCategoryId = dt3.Rows[0]["ProductCategoryId"].ToString();
                        dt4 = _cm.GetProductCategoryDetails(ProductCategoryId);
                        if (dt4.Rows.Count > 0)
                        {
                            ProductCategory = dt4.Rows[0]["subgroupname"].ToString();
                        }
                        InquiryId = dt3.Rows[0]["InquiryId"].ToString();
                    }
                    dt5 = _cm.GetCRFSample(InquiryId);
                    if (dt5.Rows.Count > 0)
                    {
                        BatchNo = dt5.Rows[0]["BatchNo"].ToString();
                    }
                    dt6 = _cm.getVerifierCustomerDetails(InquiryId);
                    if (dt6.Rows.Count > 0)
                    {
                        CustomerName = dt6.Rows[0]["CustomerName"].ToString();
                    }
                    dt7 = _cm.getVerifierDetails(ReviewedBy);
                    if (dt7.Rows.Count > 0)
                    {
                        ReviewedBy = dt7.Rows[0]["EmployeeName"].ToString();
                    }
                    DataTable dt9 = _cm.getTestReportID(SampleId);
                    if (dt9.Rows.Count > 0)
                    {
                        TestReportNo = dt9.Rows[0]["TestReportNo"].ToString();
                    }
                    dt.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("ChecklistsAccepted",typeof(string)),
                    new DataColumn("Status",typeof(string))});

                    dt = _cm.GetTestReportReviewCheckListStatusDetails(SampleId,ReviewCount);

                    //Table Start
                    sb.Append("<table cellpadding='5' align:center cellspacing='0' width='90%' style='font-size: 9pt;font-family:Arial;'>");
                    sb.Append("<tr>");
                    sb.Append("<td colspan='4' style='border: 1px solid #2F4F4F'><b>Test Report Number:</b>" + TestReportNo+"</td>");
                    sb.Append("</tr>");
                    //Adding HeaderRow.
                    sb.Append("<tr>");
                    sb.Append("<th style='width:2%;text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Sl No:</b></th>");
                    sb.Append("<th style='width:70%;text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Checklist</b></th>");
                    sb.Append("<th style='width:15%;text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Tick (&#10004; = Yes, X = No, NA = Not Applicable)</b></th>");
                    sb.Append("<th style='width:13%;text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Remarks</b></th>");

                    sb.Append("</tr>");

                    //for (int freqCount = 1; freqCount < Accepted.Count+1 && freqCount < Rejected.Count + 1; freqCount++)



                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("<tr>");
                        {
                            ReviewerID = dt.Rows[i]["ReviewerID"].ToString();
                            ReviewerName = dt.Rows[i]["ReviewerName"].ToString();
                            Checklist1 = dt.Rows[i]["Description"].ToString();
                            Status = dt.Rows[i]["Samplelist1"].ToString();
                            Remarks = dt.Rows[i]["ReviewChecklistValue"].ToString();
                            ReviewDate = Convert.ToDateTime(dt.Rows[i]["createdDate"]).ToString("dd/MM/yyyy");

                            if (Status == "Yes") { Status = "&#10004;"; }
                            if (Status == "No") { Status = "X"; }
                            if (Status == "N/A") { Status = "NA"; }
                            sb.Append("<td style='border: 1px solid #2F4F4F; text-align:left'>" + (i+1).ToString() + "</td>");
                            sb.Append("<td style='border: 1px solid #2F4F4F; text-align:left'>" + Checklist1 + "</td>");
                            sb.Append("<td style='border: 1px solid #2F4F4F; text-align:center; font-size:16px'>" + Status + "</td>");
                            sb.Append("<td style='border: 1px solid #2F4F4F; text-align:center'>" + Remarks + "</td>");
                        }

                        sb.Append("</tr>");
                    }
                    sb.Append("</table><br/><br/>");
                    sb.Append("<b>Name of the Reviewer: </b>" + ReviewerName);
                    sb.Append("<br/><br/>");
                    sb.Append("<b>Signature with Date: </b>");
                    sb.Append("<div style='text-align:left;border:none !important; padding-right:1%;'>");
                    sb.Append("<img src='http://CiscoLIMS.rnisolutions.in:8608/assets/img/Signature/" + ReviewerID + ".png' width='auto' height='60'>");
                    sb.Append("<br/>" + ReviewDate + "<br/></div>");



                    string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                    DataTable dtDocument = _cm.getDocumentDetails("Test Report Review Checklist");//Job Allocation Form
                    if (dtDocument.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtDocument.Rows)
                        {
                            DocumentNo = dr["DocumentNo"].ToString();
                            IssueNo = dr["IssueNo"].ToString();
                            Revision = dr["Revision"].ToString();
                            title = dr["Title"].ToString();
                        }
                    }
                    string body = objEmail.PrepareMailBodyWith("Template.html", "{DocumentNo}", DocumentNo, "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{SampleId}", SamplesId, "{SamplesIdSeries}", SamplesIdSeries, "{SampleReceivedDate}", SampleReceivedDate, "{Date}", RequestedDate, "{ProductCategory}", ProductCategory, "{BatchNo}", BatchNo, "{ReviewChecklist}", sb.ToString(), "{ReviewedBy}", ReviewedBy, "{CustomerName}", CustomerName, "{temperature}", Temperature, "{humidity}", Humidity, "{samplingmethod}", SamplingMethod, "{Organization}",sbOrg.ToString(),"{TestReportNo}",TestReportNo,"{Details}", sb.ToString(), "{PreparedBy}", "", "{IssueDate}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string str = body.ToString();
                    string filename = Server.MapPath("//Sample/TestReportReview-" + SamplesId + "-" + ReviewCount + ".htm");
                    System.IO.File.WriteAllText(@filename, str);
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Sample/TestReportReview-" + SamplesId + "-" + ReviewCount + ".htm"));
                    divSampleChecklist.InnerHtml = html;
                    GC.Collect();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}
