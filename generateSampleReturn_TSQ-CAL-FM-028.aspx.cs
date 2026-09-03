using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class generateSampleReturn_TSQ_CAL_FM_028 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string QuotationID = "", SampleReturnID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["s"] != null)
                {
                    Session["SampleReturnID"] = Request.QueryString["s"];
                }
                SampleReturnID = Session["SampleReturnID"].ToString();
                if (Request.QueryString["q"] != null)
                {
                    Session["QuotationID"] = Request.QueryString["q"];
                }
                if (Session["QuotationID"] != null)
                {
                    QuotationID = Session["QuotationID"].ToString();
                }

                DataTable dt = _cm.getSampleReturnDetails(SampleReturnID);
                if (dt.Rows.Count > 0)
                {
                    string QuotationID = "";
                    string DateReceived = "";
                    string ReturnPriority = "";
                    string ReturnDate = "";
                    string Lab = "";
                    string WONumber = "";
                    string SampleIDSeries = "";
                    string CustomerMasterID = "";
                    string PreparedBy = "";
                    string PreparedDate = "";
                    string CheckedBy = "";
                    string CheckedDate = "";
                    string ApprovedBy = "";
                    string ApprovedDate = "";
                    string SentBy = "";
                    string SentDate = "";
                    string AcceptedBy = "";
                    string AcceptedDate = "";

                    QuotationID = dt.Rows[0]["QuotationID"].ToString();
                    DateReceived = dt.Rows[0]["DateReceived"].ToString();
                    ReturnPriority = dt.Rows[0]["ReturnPriority"].ToString();
                    ReturnDate = dt.Rows[0]["ReturnDate"].ToString();
                    Lab = dt.Rows[0]["Lab"].ToString();
                    WONumber = dt.Rows[0]["WONumber"].ToString();
                    SampleIDSeries = dt.Rows[0]["SampleIDSeries"].ToString();
                    CustomerMasterID = dt.Rows[0]["CustomerMasterID"].ToString();
                    PreparedBy = dt.Rows[0]["PreparedBy"].ToString();
                    PreparedDate = dt.Rows[0]["PreparedDate"].ToString();
                    CheckedBy = dt.Rows[0]["CheckedBy"].ToString();
                    CheckedDate = dt.Rows[0]["CheckedDate"].ToString();
                    if (CheckedDate != "") { CheckedDate = Convert.ToDateTime(CheckedDate).ToString("dd-MM-yyyy"); }
                    ApprovedBy = dt.Rows[0]["ApprovedBy"].ToString();
                    ApprovedDate = dt.Rows[0]["ApprovedDate"].ToString();
                    if (ApprovedDate != "") { ApprovedDate = Convert.ToDateTime(ApprovedDate).ToString("dd-MM-yyyy"); }
                    SentBy = dt.Rows[0]["SentBy"].ToString();
                    SentDate = dt.Rows[0]["SentDate"].ToString();
                    if (SentDate != "") { SentDate = Convert.ToDateTime(SentDate).ToString("dd-MM-yyyy"); }
                    AcceptedBy = dt.Rows[0]["AcceptedBy"].ToString();
                    AcceptedDate = dt.Rows[0]["AcceptedDate"].ToString();
                    if (AcceptedDate != "") { AcceptedDate = Convert.ToDateTime(AcceptedDate).ToString("dd-MM-yyyy"); }

                    StringBuilder sb = new StringBuilder();


                    mailer objEmail = new mailer();
                    StringReader sr = new StringReader(sb.ToString());
                    //Table start.
                    sb.Append("<table class='tab1' style='border-left: none !important; border-right: none !important;border-bottom: none !important;margin-top:10px;  border-spacing:0px; width:100%' cellpadding='5'>");

                    //Adding HeaderRow.
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>NO.</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Nama Alat</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>No. Sampel</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>No. Seri</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Kapasitas</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Jumlah</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Kondisi Alat<br/>(Baik/Rusak)</b></td>");
                    sb.Append("</tr>");

                    DataTable dt6 = _cm.GetCalibrationRequest(QuotationID);
                    if (dt6.Rows.Count > 0)
                    {
                        string InstrumentName = "", Type = "", SerialNumber = "", CalibrationRange = "", Quantity = "", Remarks = "";
                        for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                        {
                            sb.Append("<tr>");
                            {
                                InstrumentName = dt6.Rows[freqCount - 1]["InstrumentName"].ToString();
                                Type = dt6.Rows[freqCount - 1]["InstrumentType"].ToString();
                                SerialNumber = dt6.Rows[freqCount - 1]["SerialNumber"].ToString();
                                CalibrationRange = dt6.Rows[freqCount - 1]["CalibrationRange"].ToString();
                                Quantity = dt6.Rows[freqCount - 1]["Quantity"].ToString();
                                Remarks = "";
                                sb.Append("<td style='width:5%;border-left: 1px solid #2F4F4F;'>" + freqCount + "</td>");
                                sb.Append("<td style='width:15%'>" + InstrumentName + "</td>");
                                sb.Append("<td style='width:15%'>" + Type + "</td>");
                                sb.Append("<td style='width:15%'>" + SerialNumber + "</td>");
                                sb.Append("<td style='width:15%'>" + CalibrationRange + "</td>");
                                sb.Append("<td style='width:15%'>" + Quantity + "</td>");
                                sb.Append("<td style='width:15%'>" + Remarks + "</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append("</table>");
                    }

                    string body = ""; string str = ""; string filename = ""; string html = "";

                    string DocumentNo = "", IssueDate = "", IssueNo = "", Revision = "", page = "";
                    DataTable dtDocument = _cm.getDocumentDetails("Sample Return");//Job Allocation Form
                    if (dtDocument.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtDocument.Rows)
                        {
                            DocumentNo = dr["DocumentNo"].ToString();
                            IssueDate = Convert.ToDateTime(dr["CreatedDate"]).ToString("dd MMM yyyy");
                            IssueNo = dr["IssueNo"].ToString();
                            Revision = dr["Revision"].ToString();
                            page = "1 dari 1";
                        }
                    }
                    body = objEmail.PrepareMailBodyWith("SampleReturn_TSQ-CAL-FM-028.html",
                        "{DocumentNo}", DocumentNo,
                        "{IssueDate}", IssueDate,
                        "{IssueNo}", IssueNo,
                        "{Revision}", Revision,
                        "{Page}", page,
                         "{AcceptedDate}", AcceptedDate,
                         "{QuotationID}", QuotationID,
                         "{DateReceived}", Convert.ToDateTime(DateReceived).ToString("dd-MM-yyyy"),
                         "{ReturnPriority}", ReturnPriority,
                         "{ReturnDate}", Convert.ToDateTime(ReturnDate).ToString("dd-MM-yyyy"),
                         "{Lab}", Lab,
                         "{WONumber}", WONumber,
                         "{SampleIDSeries}", SampleIDSeries,
                         "{CustomerMasterID}", CustomerMasterID,
                         "{PreparedBy}", PreparedBy,
                         "{PreparedDate}", Convert.ToDateTime(PreparedDate).ToString("dd-MM-yyyy"),
                         "{CheckedBy}", CheckedBy,
                         "{CheckedDate}", CheckedDate,
                         "{ApprovedBy}", ApprovedBy,
                         "{ApprovedDate}", ApprovedDate,
                         "{SentBy}", SentBy,
                         "{SentDate}", SentDate,
                         "{AcceptedBy}", AcceptedBy,
                         "{AcceptedDate}", AcceptedDate,
                         "{Details}", sb.ToString());
                    str = body.ToString();
                    filename = Server.MapPath("//SampleReturn/SR-" + SampleReturnID + ".htm");
                    System.IO.File.WriteAllText(@filename, str);
                    html = File.ReadAllText(HttpContext.Current.Server.MapPath("//SampleReturn/SR-" + SampleReturnID + ".htm"));
                    divAllocation.InnerHtml = html;
                    GC.Collect();
                    string url = "CustomerPOList.aspx";
                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }


    }
}