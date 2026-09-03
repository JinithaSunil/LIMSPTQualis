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
    public partial class SampleForwardingMemo : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0; int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["ID1"] != null)
            {
                InquiryID = Convert.ToInt32(Request.QueryString["ID1"].ToString());
                Session["InquiryID"] = InquiryID;
            }
            if (Request.QueryString["ID2"] != null)
            {
                CloneCRFSampleID = Convert.ToInt32(Request.QueryString["ID2"].ToString());
                Session["CloneCRFSampleID"] = CloneCRFSampleID;
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
                string SampleIDseries = "";
                string BatchNo = "";
                string TestMethod = "";
                string TestParameter = "";
                string Specification = "";
                string SizeOfSample = "";
                string Remarks = "";
                string CustomerUID = "";

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();

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

                    dt3.Columns.AddRange(new DataColumn[7] {
                    new DataColumn("SampleName", typeof(string)),
                    new DataColumn("BatchNo",typeof(string)),
                    new DataColumn("TestMethod",typeof(string)),
                    new DataColumn("TestParameter",typeof(string)),
                    new DataColumn("Specification",typeof(string)),
                    new DataColumn("SizeOfSample",typeof(string)),
                    new DataColumn("Remarks",typeof(string))});

                    dt3 = _cm.getCRFSampleDetails(InquiryID, CloneCRFSampleID);
                    dt4 =_cm.getSampleForwarding(InquiryID, CloneCRFSampleID);
                    int freqCounter = dt3.Rows.Count;
                    if(dt4.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt4.Rows)
                        {
                            SampleIDseries = dr["SampleIDseries"].ToString();
                            sb.Append("<table>");
                            sb.Append("<tr><td><b><u>Sample Registration Date: </u></b>" + Convert.ToDateTime(dr["CreatedDate"]).ToString("dd/MM/yyyy") +"<br/><br/></td></tr>");
                            sb.Append("<tr><td><b><u>Sample Forwarded to: </u></b> Laboratory<br/><br/></td></tr>");
                            sb.Append("</table>");
                        }                       
                    }
                   
                    //Table start.
                    sb.Append(@"<table class='tab1' style='border-left: none !important; border-right: none !important;border-bottom: none !important;margin-top:10px;  border-spacing:0px; width:100%' cellpadding='5'>
                                      <tr>
                                          <td style='width:5%; text-align: center'>Sl.No.</td>
                                          <td style='width:20%; text-align: center'>Sample Name & Registration No:</td>
                                          <td style='width:5%; text-align: center'>Quantity</td>
                                          <td style='width:30%; text-align: center'>Test Parameters</td>
                                          <td style='width:20%; text-align: center'>Test Methods to<br/> be followed</td>
                                          <td style='width:20%; text-align: center'>Should the<br/> limits be<br/> mentioned <br/> or not?</td>
                                      </tr>");
                    for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                    {
                        sb.Append(@"<tr>");
                        {
                            SampleName = dt3.Rows[freqCount - 1]["SampleName"].ToString();
                            BatchNo = dt3.Rows[freqCount - 1]["BatchNo"].ToString();
                            TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                            TestParameter = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                            Specification = dt3.Rows[freqCount - 1]["Specification"].ToString();
                            SizeOfSample = dt3.Rows[freqCount - 1]["SizeOfSample"].ToString();
                            Remarks = dt3.Rows[freqCount - 1]["Remarks"].ToString();

                            sb.Append("<td>" + freqCount.ToString("00") + "</td>");
                            sb.Append("<td>" + SampleName + "<br/>" + SampleIDseries + "</td>");
                            sb.Append("<td>" + SizeOfSample + "</td>");
                            sb.Append("<td>" + TestParameter + "</td>");
                            sb.Append("<td>" + TestMethod + "</td>");                            
                            sb.Append("<td>" + Specification + "</td>");
                        }
                        sb.Append("</tr>");
                    }
                    if (dt4.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt4.Rows)
                        {
                            sb.Append("<table>");
                            sb.Append("<tr><td><b><u>Sample Storage Condition:</u></b>" + dr["StorageCondition"] +"<br/><br/></td></tr>");
                            sb.Append("<tr><td><b><u>Report Required Within(Expected):</u>" + dr["RequiredBy"] +"</b><br/><br/></td></tr>");
                            sb.Append("<tr><td><b><u>Special Instruction(if any):</u></b>" + dr["SpecialInstruction"] +"<br/><br/></td></tr>");
                            sb.Append("</table>");
                        }
                    }
                    if (freqCounter+5 < 15)
                    {
                        for (int j = freqCounter+5; j < 15; j++)
                        {
                            sb.Append("<tr><td class='blank'><br /><br /></td></tr>");
                        }
                    }
                    string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                    DataTable dtDocument = _cm.getDocumentDetails("Sample Forwarding Memo");
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

                    string body = objEmail.PrepareMailBodyWith("Template.html", "{DocumentNo}", DocumentNo, "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "Company Name", CompanyName, "{Address}", Address, "Phone", Phone, "Fax", fax, "{Email}", Email, "{Contact Person}", ContactPerson, "{Designation}", Designation, "{Department}", Department, "{Mobile}", Mobile, "{ShippingAgency}", ShippingAgency, "{AirwayBillNo}", AirwayBillNo, "{InquiryID}", InquiryID.ToString(), "{Date}", DateOfRequest.ToString("dd MMM yyyy"), "{SampleName}", SampleName, "{BatchNo}", BatchNo, "{TestMethod}", TestMethod, "{TestParameter}", TestParameter, "{Specification}", Specification, "{SizeOfSample}", SizeOfSample, "{Remarks}", Remarks, "{Details}", sb.ToString());
                    string str = body.ToString();
                    string filename = Server.MapPath("//Sample/SampleForwarding-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                    System.IO.File.WriteAllText(@filename, str);

                    //CRF Finalized by Sample Management Team
                        _cm.CRFStatusID = 28;
						_cm.CloneCRFSampleID = CloneCRFSampleID;
						_cm.UpdateCRFStatus(_cm);
						GC.Collect();
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Sample/SampleForwarding-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm"));

                    divRequest.InnerHtml = html;
                }
            }

        }
    }
}