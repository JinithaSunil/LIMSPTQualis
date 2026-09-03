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
    public partial class generateWork_Order_TSQ_CAL_FM_002_SPK : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string QuotationID = "", WorkOrderID = "", RequestID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["w"] != null)
                {
                    WorkOrderID = Request.QueryString["w"];
                    Session["WorkOrderID"] = WorkOrderID;
                }
                WorkOrderID = Session["WorkOrderID"].ToString();
                if (Request.QueryString["q"] != null)
                {
                    QuotationID = Request.QueryString["q"];
                    Session["QuotationID"] = QuotationID;
                }
                if (Request.QueryString["r"] != null)
                {
                    RequestID = Request.QueryString["r"];
                    Session["RequestID"] = RequestID;
                } 

                DataTable dt = _cm.getWorkOrderDetails(WorkOrderID);
                if (dt.Rows.Count > 0)
                {
                    string WorkOrderNumber = "";
                    string DateReceived = "";
                    string ServiceDueDate = "";
                    string CertificateIssueDate = "";
                    string ApplicationNo = "";
                    string CustomerMasterID = "";
                    string CustomerName = "";
                    string PreparedBy = "";
                    string PreparedDate = "";
                    string CheckedBy = "";
                    string CheckedDate = "";
                    string ReceivedBy = "";
                    string ReceivedDate = "";
                    string ReturnedBy = "";
                    string ReturnedDate = "";
                    string createdDate = "";
                    string Lab = "";

                    WorkOrderNumber = dt.Rows[0]["WONumber"].ToString();
                    DateReceived = dt.Rows[0]["DateReceived"].ToString();
                    DateReceived = string.IsNullOrEmpty(DateReceived) ? "" : Convert.ToDateTime(DateReceived).ToString("dd-MM-yyyy");
                    ServiceDueDate = dt.Rows[0]["ServiceDueDate"].ToString();
                    ServiceDueDate = string.IsNullOrEmpty(ServiceDueDate) ? "" : Convert.ToDateTime(ServiceDueDate).ToString("dd-MM-yyyy");
                    CertificateIssueDate = dt.Rows[0]["CertificateIssueDate"].ToString();
                    CertificateIssueDate = string.IsNullOrEmpty(CertificateIssueDate) ? "" : Convert.ToDateTime(CertificateIssueDate).ToString("dd-MM-yyyy");
                    ApplicationNo = dt.Rows[0]["ApplicationNo"].ToString();
                    CustomerName = dt.Rows[0]["CompanyName"].ToString();
                    CustomerMasterID = dt.Rows[0]["CustomerMasterID"].ToString();
                    PreparedBy = dt.Rows[0]["PreparedBy"].ToString();
                    PreparedDate = dt.Rows[0]["PreparedDate"].ToString();
                    PreparedDate = string.IsNullOrEmpty(PreparedDate) ? "" : Convert.ToDateTime(PreparedDate).ToString("dd-MM-yyyy");
                    CheckedBy = dt.Rows[0]["CheckedBy"].ToString();
                    CheckedDate = dt.Rows[0]["CheckedDate"].ToString();
                    CheckedDate = string.IsNullOrEmpty(CheckedDate) ? "" : Convert.ToDateTime(CheckedDate).ToString("dd-MM-yyyy");
                    ReceivedBy = dt.Rows[0]["ReceivedBy"].ToString();
                    ReceivedDate = dt.Rows[0]["ReceivedDate"].ToString();
                    ReceivedDate = string.IsNullOrEmpty(ReceivedDate) ? "" : Convert.ToDateTime(ReceivedDate).ToString("dd-MM-yyyy");
                    ReturnedBy = dt.Rows[0]["ReturnedBy"].ToString();
                    ReturnedDate = dt.Rows[0]["ReturnedDate"].ToString();
                    ReturnedDate = string.IsNullOrEmpty(ReturnedDate) ? "" : Convert.ToDateTime(ReturnedDate).ToString("dd-MM-yyyy");
                    createdDate = dt.Rows[0]["createdDate"].ToString();
                    Lab = dt.Rows[0]["Lab"].ToString();


                    StringBuilder sb = new StringBuilder();

                   
                    mailer objEmail = new mailer();
                    StringReader sr = new StringReader(sb.ToString());
                    //Table start.
                    sb.Append("<table class='tab1' style='border-left: none !important; border-right: none !important;border-bottom: none !important;margin-top:10px;  border-spacing:0px; width:100%' cellpadding='5'>");

                    //Adding HeaderRow.
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>NO.</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Nama Alat</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Status</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>No. Seri</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Kapasitas / Spesifikasi</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Jumlah</b></td>");
                    sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Keterangan</b></td>");
                    sb.Append("</tr>");
                  
                    DataTable dt6 = _cm.GetCalibrationRequest(QuotationID,RequestID);
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

                    string DocumentNo = "", IssueDate="", IssueNo = "", Revision = "", page = "";
                    DataTable dtDocument = _cm.getDocumentDetails("SPK");//Job Allocation Form
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
                    body = objEmail.PrepareMailBodyWith("WorkOrder_TSQ-CAL-FM-002-SPK.html",
                        "{DocumentNo}", DocumentNo,
                        "{IssueDate}", IssueDate,
                        "{IssueNo}", IssueNo,
                        "{Revision}", Revision,
                        "{Page}", page,
                         "{WorkOrderNumber}", WorkOrderNumber,
                         "{DateReceived}", DateReceived,
                         "{ServiceDueDate}", ServiceDueDate,
                         "{CertificateIssueDate}", CertificateIssueDate,
                         "{ApplicationNo}", ApplicationNo,
                         "{CustomerName}", CustomerName,
                         "{CustomerMasterID}", CustomerMasterID,
                         "{PreparedBy}", PreparedBy,
                         "{PreparedDate}", PreparedDate,
                         "{CheckedBy}", CheckedBy,
                         "{CheckedDate}", CheckedDate,
                         "{ReceivedBy}", ReceivedBy,
                         "{ReceivedDate}", ReceivedDate,
                         "{ReturnedBy}", ReturnedBy,
                         "{ReturnedDate}", ReturnedDate,
                         "{Lab}", Lab,
                         "{Details}", sb.ToString());
                    str = body.ToString();
                    filename = Server.MapPath("//WorkOrder/WO-" + QuotationID + ".htm");
                    System.IO.File.WriteAllText(@filename, str);
                    html = File.ReadAllText(HttpContext.Current.Server.MapPath("//WorkOrder/WO-" + QuotationID + ".htm"));
                    divAllocation.InnerHtml = html;
                    GC.Collect();
                    string url = "ToolsVerification.aspx?q=" + QuotationID;
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