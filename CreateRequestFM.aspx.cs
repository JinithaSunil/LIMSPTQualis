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
    public partial class CreateRequestFM : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0; int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["OrganizationID"] = 1;
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
            StringBuilder sb = new StringBuilder();
            mailer objEmail = new mailer();
            StringReader sr = new StringReader(sb.ToString());
            string CustomerUID = "";
            if (Session["InquiryID"] != null)
            {
                DataTable dtCustomer = _cm.GetCRFDetails(Convert.ToInt32(Session["InquiryID"].ToString()));
                if (dtCustomer.Rows.Count > 0)
                {

                    CustomerUID = dtCustomer.Rows[0]["CustomerUID"].ToString();
                }
                DataTable dt3 = _cm.getCloneCRFSampleDetails(InquiryID, CloneCRFSampleID);
                if (dt3.Rows.Count > 0)
                {
                    sb.Append("<h4>Sample Name : " + dt3.Rows[0]["SampleName"].ToString() + "</h4>");
                    sb.Append("<h4>Batch# :" + dt3.Rows[0]["BatchNo"].ToString() + "</h4>");
                    sb.Append("<h4>Size/ Nos of Sample : " + dt3.Rows[0]["SizeofSample"].ToString() + "</h4>");
                    if (dt3.Rows[0]["Requirement"].ToString() != "")
                    {
                        sb.Append("<h4>Requirement : " + dt3.Rows[0]["Requirement"].ToString() + "</h4>");
                    }
                    sb.Append("<table style='width:100%'class='tab1'>");
                    sb.Append("<tr>");
                    sb.Append("<td><b>Sl.<br>No.</b></td>");
                    sb.Append("<td><b>Test Parameters Required for<br/> Analysis / Quotation<br/> Reference</b></td>");
                    sb.Append("<td><b>Test Sub Parameters</b></td>");
                    sb.Append("<td><b>Test Methods</b></td>");
                    sb.Append("<td><b>UOM</b></td>");
                    sb.Append("<td><b>LOD</b></td>");
                    sb.Append("<td><b>LOQ</b></td>");
                    sb.Append("<td><b>*Any<br/> Specification<br/> to be<br/> mentioned?</b></td>");
                    sb.Append("</tr>");
                }

                int freqCounter = dt3.Rows.Count;
                string SampleName = "";
                string BatchNo = "";
                string TestMethod = "";
                string TestParameter = "";
                string TestSubParameter = "";
                string unitofmeasurement = "";
                string LOD = "";string LOQ = "";
                string Specification = "";
                string SizeOfSample = "";
                string Remarks = "";

                for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                {
                    sb.Append("<tr>");
                    {
                        SampleName = dt3.Rows[freqCount - 1]["SampleName"].ToString();
                        BatchNo = dt3.Rows[freqCount - 1]["BatchNo"].ToString();
                        TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                        TestParameter = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                        TestSubParameter = dt3.Rows[freqCount - 1]["TestSubParameter"].ToString();
                        unitofmeasurement = dt3.Rows[freqCount - 1]["unitofmeasurement"].ToString();
                        LOD = dt3.Rows[freqCount - 1]["LOD"].ToString();
                        LOQ = dt3.Rows[freqCount - 1]["LOQ"].ToString();
                        Specification = dt3.Rows[freqCount - 1]["Specification"].ToString();
                        SizeOfSample = dt3.Rows[freqCount - 1]["SizeOfSample"].ToString();
                        Remarks = dt3.Rows[freqCount - 1]["Remarks"].ToString();

                        sb.Append("<td>" + freqCount.ToString("00") + "</td>");
                        sb.Append("<td>" + TestParameter + "</td>");
                        sb.Append("<td>" + TestSubParameter + "</td>");
                        sb.Append("<td>" + TestMethod + "</td>");
                        sb.Append("<td>" + unitofmeasurement + "</td>");
                        sb.Append("<td>" + LOD + "</td>");
                        sb.Append("<td>" + LOQ + "</td>");
                        sb.Append("<td>" + Specification + "</td>");

                    }
                    sb.Append("</tr>");
                }
                if (dt3.Rows.Count < 30)
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
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                        }
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</table>");

            }
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

            string body = objEmail.PrepareMailBodyWith("Template.html", 
                "{DocumentNo}", DocumentNo, 
                "{IssueNo}", IssueNo, 
                "{Revision}", Revision, 
                "{Title}", title, 
                "{Details}", sb.ToString());
            string str = body.ToString();
            string filename = Server.MapPath("//CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
            System.IO.File.WriteAllText(@filename, str);

            //CRF Finalized by Sample Management Team
            _cm.CRFStatusID = 28;
            _cm.CloneCRFSampleID = CloneCRFSampleID;
            _cm.UpdateCRFStatus(_cm);
            GC.Collect();
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm"));

            divRequest.InnerHtml = html;
            Response.Redirect("CustomerInquiryView.aspx");
        }
    }

}
   