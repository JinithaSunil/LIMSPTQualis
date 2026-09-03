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
    public partial class GenerateAnalysisDataSheet : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); string SampleID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["ID"] != null)
                {
                    Session["SampleID"] = Request.QueryString["ID"].ToString();
                }
                if (Session["SampleID"] != null)
                {
                    SampleID = Session["SampleID"].ToString();

                    if ((Session["Usertype"] != null)&& (Session["EmployeeID"]!=null))
                    {
                        DataTable dt6 = new DataTable();
                        StringBuilder sb = new StringBuilder();
                        mailer objEmail = new mailer();
                        StringReader sr = new StringReader(sb.ToString());

                        string body = ""; string str = ""; string filename = ""; string html = "";
                        string PreparedBy = "";string ReviewedBy = "";

                        string EmployeeID = Session["EmployeeID"].ToString();
                        dt6 = _cm.GetAnalystTestReportDetails(SampleID,EmployeeID);
                        //Table start.
                        sb.Append("<table border='1' style='border-spacing:0px; width:100%'>");
                        sb.Append("<thead>");
                        //Adding HeaderRow.
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'><b>Sample Name:</b></td>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'>" + dt6.Rows[0]["SampleName"].ToString()+"</td>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'><b>Analysis Started on:</b></td>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'><b>Sample Registration No.:</b></td>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'>" + dt6.Rows[0]["SampleIDSeries"].ToString() + "</td>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'><b>Analysis Completed on:</b></td>");
                        sb.Append("<td style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:25%'></td>");
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        foreach (DataRow dr in dt6.Rows)
                        {
                            PreparedBy = dr["EmployeeName"].ToString();
                            sb.Append("<tr>");
                            sb.Append("<td colspan='4' style='text-align:left;padding:5px;border: 1px solid #2F4F4F;width:10%'>");
                            sb.Append("<b>Parameter:</b>");
                            sb.Append(dr["TestParameter"].ToString() + ": <br/><br/>");
                            sb.Append("<b>Method used:</b>");
                            sb.Append(dr["TestMethod"].ToString() + "<br/><br/>");
                            sb.Append("<b>Calculation:</b><br/>");
                            for(int j = 0; j < 10; j++)
                            {
                                sb.Append("<br/>");
                            }
                            sb.Append("<b>Analyzed By:</b>" + PreparedBy);
                            sb.Append("</td>");
                            sb.Append("</tr>");
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

                        body = objEmail.PrepareMailBodyWith("Template.html", "{DocumentNo}", DocumentNo, "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{PreparedBy}", PreparedBy, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestAnalysisDataSheet/TestAnalysisDataSheet-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestAnalysisDataSheet/TestAnalysisDataSheet-" + SampleID + ".htm"));
                        divAnalysisDataSheet.InnerHtml = html;
                        GC.Collect();
                        //Response.Redirect("~/SampleList.aspx");

                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}

