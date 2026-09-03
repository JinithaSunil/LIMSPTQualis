using iTextSharp.text;
using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CreateFinalRequestCustomerFM : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["OrganizationID"] = 1;
            if (Request.QueryString["ID"] != null)
            {
                InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                Session["InquiryID"] = InquiryID;
            }
            if (Session["InquiryID"] != null)
            {
                string CompanyName = "";
                string Address = "";
                string Phone = "";
                string fax = "";
                string Email = "";
                string ContactPerson = "";
                string Designation = "";
                string Department = "";
                string Mobile = "";
                string ShippingAgency = "";
                string AirwayBillNo = "";
                DateTime DateOfRequest = new DateTime();
                string SampleName = "";
                string BatchNo = "";
                string TestMethod = "";
                string TestParameter = "";
                string TestSubParameter = "";
                string Specification = "";
                string SizeOfSample = "";
                string Remarks = "";
                string CustomerUID = "";

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                dt = _cm.getInquiryDetails(InquiryID, 1);
                if (dt.Rows.Count > 0)
                {
                    int ContactPersonId = Convert.ToInt32(dt.Rows[0]["ContactPerson"]);
                    foreach (DataRow dr in dt.Rows)
                    {
                        InquiryID = Convert.ToInt32(dr["InquiryID"].ToString());
                        CustomerUID = dr["CustomerUID"].ToString();
                        CompanyName = dr["CustomerName"].ToString();
                        Address = dr["Address"].ToString();
                        ShippingAgency = dr["ShippingAgency"].ToString();
                        AirwayBillNo = dr["AirwayBillNo"].ToString();
                        DateOfRequest = Convert.ToDateTime(dr["Created"].ToString());
                    }
                    dt2 = _cm.getContactPersonDetails(ContactPersonId);
                    foreach (DataRow dr in dt2.Rows)
                    {
                        if (dr["Mobile"].ToString() != "") { Mobile = dr["Mobile"].ToString(); }
                        if (dr["Email"].ToString() != "") { Email = dr["Email"].ToString(); }
                        if (dr["Phone"].ToString() != "") { Phone = dr["Phone"].ToString(); }
                        Designation = dr["Designation"].ToString();
                        Department = dr["Department"].ToString();
                    }

                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        ContactPerson = dr2["CustomerName"].ToString();
                    }
                    StringBuilder sb = new StringBuilder();
                    mailer objEmail = new mailer();
                    StringReader sr = new StringReader(sb.ToString());

                    dt3.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("SampleName", typeof(string)),
                    new DataColumn("BatchNo",typeof(string)),
                    new DataColumn("TestMethod",typeof(string)),
                    new DataColumn("TestParameter",typeof(string)),
                    new DataColumn("TestSubParameter",typeof(string)),
                    new DataColumn("Specification",typeof(string)),
                    new DataColumn("SizeOfSample",typeof(string)),
                    new DataColumn("Remarks",typeof(string))});

                    dt3 = _cm.getCustomerCRFSampleDetails(InquiryID);

                    if (dt3.Rows.Count > 0)
                    {
                        sb.Append("<h4>Sample Name : " + dt3.Rows[0]["SampleName"].ToString() + "</h4>");
                        sb.Append("<h4>Batch# :" + dt3.Rows[0]["BatchNo"].ToString() + "</h4>");
                        sb.Append("<h4>Size/ Nos of Sample : " + dt3.Rows[0]["SizeofSample"].ToString() + "</h4>");
                        sb.Append("<table style='width:100%'class='tab1'>");
                        sb.Append("<tr>");
                        sb.Append("<td><b>Sl.<br>No.</b></td>");
                        sb.Append("<td><b>Test Parameters Required for<br/> Analysis / Quotation<br/> Reference</b></td>");
                        sb.Append("<td><b>Test Sub Parameter</b></td>");
                        sb.Append("<td><b>Test Methods</b></td>");
                        sb.Append("<td><b>*Any<br/> Specification<br/> to be<br/> mentioned?</b></td>");
                        sb.Append("</tr>");
                    }
                    int freqCounter = dt3.Rows.Count;
                    //Table start.

                    for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                    {
                        sb.Append(@"<tr>");
                        {
                            TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                            TestParameter = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                            TestSubParameter = dt3.Rows[freqCount - 1]["TestSubParameter"].ToString();
                            Specification = dt3.Rows[freqCount - 1]["Specification"].ToString();
                            Remarks = dt3.Rows[freqCount - 1]["Remarks"].ToString();

                            sb.Append("<td>" + freqCount.ToString("00") + "</td>");
                            sb.Append("<td>" + TestParameter + "</td>");
                            sb.Append("<td>" + TestSubParameter + "</td>");
                            sb.Append("<td>" + TestMethod + "</td>");
                            sb.Append("<td>" + Specification + "</td>");

                        }
                        sb.Append("</tr>");
                    }
                    if(dt3.Rows.Count < 30)
                    {
                        for (int i = dt3.Rows.Count; i < 30; i++)
                        {
                            sb.Append(@"<tr>");
                            {
                                sb.Append("<td>&nbsp;</td>");
                                sb.Append("<td>&nbsp;</td>");
                                sb.Append("<td>&nbsp;</td>");
                                sb.Append("<td>&nbsp;</td>");
                                sb.Append("<td>&nbsp;</td>");

                            }
                            sb.Append("</tr>");
                        }
                    }

                    sb.Append(@"</table>"
                );

                    string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                    DataTable dtDocument = _cm.getDocumentDetails("Test Request Form");//Job Allocation Form
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
                    string body = objEmail.PrepareMailBodyWith("Template.html", "{DocumentNo}", DocumentNo, "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "Company Name", CompanyName, "{Address}", Address, "Phone", Phone, "Fax", fax, "{Email}", Email, "{Contact Person}", ContactPerson, "{Designation}", Designation, "{Department}", Department, "{Mobile}", Mobile, "{ShippingAgency}", ShippingAgency, "{AirwayBillNo}", AirwayBillNo, "{InquiryID}", InquiryID.ToString(), "{Date}", DateOfRequest.ToString("dd MMM yyyy"), "{SampleName}", SampleName, "{BatchNo}", BatchNo, "{TestMethod}", TestMethod, "{TestParameter}", TestParameter, "{Specification}", Specification, "{SizeOfSample}", SizeOfSample, "{Remarks}", Remarks, " {Details}", sb.ToString());
                    string str = body.ToString();
                    string filename = Server.MapPath("//CustomerRequest/Request-Final-" + InquiryID + CustomerUID + ".htm");
                    System.IO.File.WriteAllText(@filename, str);

                    //change customer inquiry Status to CRF Received
                    _cm.inquiryStatusId = 6;
                    _cm.UpdateInquiryStatus(_cm);
                    GC.Collect();
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//CustomerRequest/Request-Final-" + InquiryID + CustomerUID + ".htm"));

                    divRequest.InnerHtml = html;
                }
            }

        }
    }
}