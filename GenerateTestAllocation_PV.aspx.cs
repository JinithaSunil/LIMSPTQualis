using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
	public partial class GenerateTestAllocation_PV : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM(); string sampleUID = ""; int CloneCRFSampleID = 0;
        DAL.mailer mailer = new DAL.mailer();
        int freqCounter = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {

                int SampleID;
                string SampleReceivedDate = "";
                string ProductCategory = "";
                string AllotedTo = "";
                string AllotedBy = "";
                string BatchNo = "";
                string DateOfRequest = "";
                int EmployeeId;
                string InquiryId = "";
                string Description = "";
                string SampleName = "";
                string TestMethod = "";
                string ReportName = "";
                string Id = "";
                string DepartmentId = "";
                string sampleIDSeries = "";
                string sampleIDSeriesAll = "";
                //string UnitOfMeasurement = "";
                string Specification = "";
                string Sr = "";
                string Temp = "";
                string RH = "";
                int Dept = 0;
                string TestsRequested = "";
                string SampleDescription = "";
                string AllocationID = "";
                string AllocatedDate = "";
                string Remarks = "";
                string body = ""; string str = ""; string filename = ""; string html = "";

                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                DataTable dt5 = new DataTable();
                DataTable dt6 = new DataTable();
                DataTable dt7 = new DataTable();

                if (Request.QueryString["s"] != null)
                {
                    Session["SampleID"] = Request.QueryString["s"];
                }

                SampleID = Convert.ToInt32(Session["SampleID"].ToString());

                dt = _cm.getSampleDetails(SampleID);
                {
                    dt2 = _cm.getSampleDetails(SampleID);
                    dt3 = _cm.GetCustomerInquiryDetails(SampleID);
                    if (dt2.Rows.Count > 0)
                    {
                        DateTime ReceivedDate = Convert.ToDateTime(dt2.Rows[0]["ReceivedDate"].ToString());
                        DateTime RequestDate = Convert.ToDateTime(dt2.Rows[0]["Created"].ToString());
                        SampleReceivedDate = ReceivedDate.ToString("dd/MMM/yyyy");
                        DateOfRequest = RequestDate.ToString("dd/MMM/yyyy");
                        Id = dt2.Rows[0]["SampleId"].ToString();
                        InquiryId = dt2.Rows[0]["EnquiryId"].ToString();
                        CloneCRFSampleID = Convert.ToInt32(dt2.Rows[0]["CloneCRFSampleID"].ToString());
                    }
                    if (dt3.Rows.Count > 0)
                    {
                        string ProductCategoryId = dt3.Rows[0]["ProductCategoryId"].ToString();
                        Dept = Convert.ToInt32(dt3.Rows[0]["DepartmentID"].ToString());
                        dt4 = _cm.GetProductGroupDetails(ProductCategoryId);
                        if (dt4.Rows.Count > 0)
                        {
                            ProductCategory = dt4.Rows[0]["productgroupname"].ToString();
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    mailer objEmail = new mailer();
                    StringReader sr = new StringReader(sb.ToString());
                    Session["CloneCRFSampleID"] = CloneCRFSampleID;


                    dt6.Columns.AddRange(new DataColumn[5] {
                                    new DataColumn("SampleName", typeof(string)),
                                    new DataColumn("BatchNo",typeof(string)),
                                    new DataColumn("ReportName",typeof(string)),
                                    new DataColumn("TestMethod",typeof(string)),
                                    new DataColumn("Specification",typeof(string))
                                });
                    dt6 = _cm.Getgeneratetestallocation_PV(CloneCRFSampleID.ToString());
                    BatchNo = dt6.Rows[0]["BatchNo"].ToString();
                    sampleIDSeries = dt6.Rows[0]["sampleIDSeries"].ToString();
                    TestsRequested = dt6.Rows[0]["TestsRequested"].ToString();
                    TestMethod = dt6.Rows[0]["TestMethod"].ToString();
                    freqCounter = dt6.Rows.Count;

                    //Table start.
                    sb.Append("<table class='tab1' style='border-left: none !important; border-right: none !important;border-bottom: none !important;margin-top:10px;  border-spacing:0px; width:100%' cellpadding='5'>");

                    //Adding HeaderRow.
                    sb.Append("<tr>");

                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Sl#</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Sample Name</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Registration No:</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Tests allotted</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Allotted to <br/>(Name)</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Allotted by<br/>(Name & Sign with Date)</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Accepted by<br/>(Name & Sign with Date)</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Remarks</b></td>");

                    sb.Append("</tr>");

                    for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                    {
                        sb.Append("<tr>");
                        {
                            AllocatedDate = Convert.ToDateTime(dt6.Rows[freqCount - 1]["AllocatedAt"]).ToString("dd/MM/yyyy");
                            SampleName = dt6.Rows[freqCount - 1]["SampleName"].ToString();
                            BatchNo = dt6.Rows[freqCount - 1]["BatchNo"].ToString();
                            TestMethod = dt6.Rows[freqCount - 1]["TestMethod"].ToString();
                            ReportName = dt6.Rows[freqCount - 1]["ReportName"].ToString();

                            Description = dt6.Rows[freqCount - 1]["DescriptionOfTest"].ToString();
                            //UnitOfMeasurement = dt6.Rows[freqCount - 1]["UnitOfMeasurement"].ToString();
                            //UnitOfMeasurement = _cm.ReplaceSuperScript(UnitOfMeasurement);
                            Specification = dt6.Rows[freqCount - 1]["Specification"].ToString();
                            AllotedTo = dt6.Rows[freqCount - 1]["EmployeeName"].ToString();
                            AllotedBy = dt6.Rows[freqCount - 1]["Supervisor"].ToString();
                            Remarks = dt6.Rows[freqCount - 1]["Remarks"].ToString();
                            sb.Append("<td style='width:5%;border-left: 1px solid #2F4F4F;'>" + freqCount.ToString("00") + "</td>");
                            sb.Append("<td style='width:15%'>" + SampleName + "</td>");
                            sb.Append("<td style='width:15%'>" + sampleIDSeries + "</td>");
                            sb.Append("<td style='width:15%'>" + ReportName + "</td>");
                            sb.Append("<td style='width:15%'>" + AllotedTo + "</td>");
                            sb.Append("<td style='width:15%'>" + AllotedBy + "(" + AllocatedDate + ")" + "</td>");
                            sb.Append("<td style='width:15%;'>" + AllotedTo + "</td>");
                            sb.Append("<td style='width:5%'>" + Remarks + "</td>");
                        }
                        sb.Append("</tr>");
                    }
                    if (freqCounter < 15)
                    {
                        for (int j = freqCounter; j < 15; j++)
                        {
                            sb.Append("<tr><td class='blank'><br /><br /></td></tr>");
                        }
                    }
                    //Table end.
                    sb.Append("</table>");

                    string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                    DataTable dtDocument = _cm.getDocumentDetails("Job Allocation Form");//Job Allocation Form
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
                    body = objEmail.PrepareMailBodyWith("Template.html", "{DocumentNo}", DocumentNo, "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString(), "{PreparedBy}", AllotedBy, "{IssueDate}", AllocatedDate);
                    str = body.ToString();
                    filename = Server.MapPath("//TestAllocation/Allocation-" + Id + ".htm");
                    System.IO.File.WriteAllText(@filename, str);
                    html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestAllocation/Allocation-" + Id + ".htm"));
                    divAllocation.InnerHtml = html;
                    GC.Collect();
                    //Response.Redirect("~/SampleList.aspx");

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}