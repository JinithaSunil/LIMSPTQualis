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
    public partial class GenerateSampleStatus : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); string SampleVerifiedBy = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["EmployeeName"] != null)
                {
                    SampleVerifiedBy = Session["EmployeeName"].ToString();
                }
                if (Request.QueryString["s"] != null)
                {
                    Session["SampleId"] = Request.QueryString["s"].ToString();
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
                    //ArrayList Accepted = new ArrayList();
                    //ArrayList Rejected = new ArrayList();
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
                        SampleVerifiedBy = dt2.Rows[0]["SampleVerifiedBy"].ToString();
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
                    dt7 = _cm.getVerifierDetails(SampleVerifiedBy);
                    if (dt7.Rows.Count > 0)
                    {
                        SampleVerifiedBy = dt7.Rows[0]["EmployeeName"].ToString();
                    }
                    dt.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("ChecklistsAccepted",typeof(string)),
                    new DataColumn("Status",typeof(string))});

                    dt = _cm.GetSampleCheckListStatusDetails(SampleId);

                    //Table Start
                    sb.Append("<table cellpadding='5' align:center cellspacing='0' width='90%' style='font-size: 9pt;font-family:Arial;'>");

                    //Adding HeaderRow.
                    sb.Append("<tr>");

                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Sample Checklist</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Status</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F'><b>Remarks</b></td>");

                    sb.Append("</tr>");

                    //for (int freqCount = 1; freqCount < Accepted.Count+1 && freqCount < Rejected.Count + 1; freqCount++)



                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        sb.Append("<tr>");
                        {
                            Checklist1 = dt.Rows[i]["SampleChecklistName"].ToString();
                            Status = dt.Rows[i]["Samplelist1"].ToString();
                            Remarks = dt.Rows[i]["SampleChecklistValue"].ToString();

                            sb.Append("<td style='width:100px;border: 1px solid #2F4F4F; text-align:left'>" + Checklist1 + "</td>");
                            sb.Append("<td style='width:100px;border: 1px solid #2F4F4F; text-align:center'>" + Status + "</td>");
                            sb.Append("<td style='width:100px;border: 1px solid #2F4F4F; text-align:center'>" + Remarks + "</td>");
                        }

                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");

                    string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                    DataTable dtDocument = _cm.getDocumentDetails("Sample Verification Checklist");
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
                    string body = objEmail.PrepareMailBodyWith("Template.html", "{DocumentNo}", DocumentNo, "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{SampleId}", SamplesId, "{SamplesIdSeries}", SamplesIdSeries, "{SampleReceivedDate}", SampleReceivedDate, "{Date}", RequestedDate, "{ProductCategory}", ProductCategory, "{BatchNo}", BatchNo, "{Details}", sb.ToString(), "{SampleVerifiedBy}", SampleVerifiedBy, "{CustomerName}", CustomerName, "{temperature}", Temperature, "{humidity}", Humidity, "{samplingmethod}", SamplingMethod, "{Organization}",sbOrg.ToString());
                    string str = body.ToString();
                    string filename = Server.MapPath("//Sample/Sample-" + SamplesId + ".htm");
                    System.IO.File.WriteAllText(@filename, str);
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Sample/Sample-" + SamplesId + ".htm"));
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
