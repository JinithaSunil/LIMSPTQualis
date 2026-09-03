using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
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
    public partial class generateToolReceiptRecord_TSQ_CAL_FM_003 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string QuotationID = "",RequestID="";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if ((Request.QueryString["q"] != null) && (Request.QueryString["r"] != null))
                {
                    hdQuotationID.Value = Request.QueryString["q"].ToString();
                    hdRequestID.Value = Request.QueryString["r"].ToString();
                    Session["QuotationID"] = Request.QueryString["q"];


                    QuotationID = Session["QuotationID"].ToString();
                    RequestID = hdRequestID.Value.ToString();
                    DataTable dt = _cm.getToolVerificationDetails(QuotationID, RequestID);
                    if (dt.Rows.Count > 0)
                    {
                        string CustomerName = "", PONumber = "", ToolReceiptDate = "", Place = "";
                        string CheckedBy = "", CheckedByDesignation = "", CustomerContact = "", CustomerContactDesignation = "", CalibrationScope = "", Notes = "";

                        CustomerName = dt.Rows[0]["CompanyName"].ToString();
                        PONumber = dt.Rows[0]["PONumber"].ToString();
                        ToolReceiptDate = dt.Rows[0]["ToolReceiptDate"].ToString();
                        Place = dt.Rows[0]["Place"].ToString();
                        CheckedBy = dt.Rows[0]["CheckedBy"].ToString();
                        CheckedByDesignation = dt.Rows[0]["CheckedByDesignation"].ToString();
                        CustomerContact = dt.Rows[0]["CustomerContact"].ToString();
                        CustomerContactDesignation = dt.Rows[0]["CustomerContactDesignation"].ToString();
                        CalibrationScope = dt.Rows[0]["CalibrationScope"].ToString();
                        Notes = dt.Rows[0]["Notes"].ToString();

                        StringBuilder sb = new StringBuilder();
                        StringBuilder sbCalibrationScope = new StringBuilder();

                        DataTable dtCalibrationScopeAll = _cm.getCalibrationScope();
                        if (CalibrationScope[CalibrationScope.Length - 1].ToString() == ",")
                        {
                            CalibrationScope = CalibrationScope.Substring(0, (CalibrationScope.Length - 1));
                        }
                        DataTable dtCalibrationScope = _cm.getCalibrationScope(CalibrationScope);
                        if (dtCalibrationScopeAll.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in dtCalibrationScopeAll.Rows)
                            {
                                foreach (DataRow dr2 in dtCalibrationScope.Rows)
                                {
                                    if (dr1["Lab"].ToString() == dr2["Lab"].ToString())
                                    {
                                        sbCalibrationScope.Append("<input type='checkbox' id='myCheckbox' name='option1' value='value1' checked>");
                                        sbCalibrationScope.Append("<label for='myCheckbox'>" + dr2["Lab"].ToString() + "</label><br/>");
                                    }
                                }
                            }
                        }

                        mailer objEmail = new mailer();
                        StringReader sr = new StringReader(sb.ToString());
                        //Table start.
                        sb.Append("<table class='tab1' style='border-left: none !important; border-right: none !important;border-bottom: none !important;margin-top:10px;  border-spacing:0px; width:100%' cellpadding='5'>");

                        //Adding HeaderRow.
                        sb.Append("<tr>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>NO.</b></td>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Nama Alat</b></td>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Merek/Type</b></td>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Kapasitas</b></td>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Jumlah</b></td>");
                        sb.Append("<td colspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Kondisi</b></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:50%'><b>Baik</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:50%'><b>Tidak Baik</b></td>");
                        sb.Append("</tr>");

                        DataTable dt6 = _cm.GetCalibrationRequest(QuotationID,RequestID);
                        if (dt6.Rows.Count > 0)
                        {
                            string InstrumentName = "", Type = "", CalibrationRange = "", Quantity = "", VerificationStatus = "";
                            for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                            {
                                sb.Append("<tr>");
                                {
                                    InstrumentName = dt6.Rows[freqCount - 1]["InstrumentName"].ToString();
                                    Type = dt6.Rows[freqCount - 1]["InstrumentType"].ToString();
                                    CalibrationRange = dt6.Rows[freqCount - 1]["CalibrationRange"].ToString();
                                    Quantity = dt6.Rows[freqCount - 1]["Quantity"].ToString();
                                    VerificationStatus = dt6.Rows[freqCount - 1]["VerificationStatus"].ToString();
                                    sb.Append("<td style='width:5%;border-left: 1px solid #2F4F4F;'>" + freqCount.ToString("00") + "</td>");
                                    sb.Append("<td style='width:15%'>" + InstrumentName + "</td>");
                                    sb.Append("<td style='width:15%'>" + Type + "</td>");
                                    sb.Append("<td style='width:15%'>" + CalibrationRange + "</td>");
                                    sb.Append("<td style='width:15%'>" + Quantity + "</td>");
                                    if (VerificationStatus == "1")
                                    {
                                        sb.Append("<td style='width:15%'><img src='assets/img/accept1.png' height='20'/></td><td style='width:15%'></td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='width:15%'></td><td style='width:15%'><img src='assets/img/accept1.png' height='20'/></td>");
                                    }


                                }
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }

                        string body = ""; string str = ""; string filename = ""; string html = "";

                        string DocumentNo = "", IssueDate = "", IssueNo = "", Revision = "", page = "";
                        DataTable dtDocument = _cm.getDocumentDetails("Tool Receipt");
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
                        body = objEmail.PrepareMailBodyWith("ToolReceipt-TSQ-CAL-FM-003 PENERIMAAN PERALATAN.html",
                            "{DocumentNo}", DocumentNo,
                            "{IssueDate}", IssueDate,
                            "{IssueNo}", IssueNo,
                            "{Revision}", Revision,
                            "{Page}", page,
                             "{CustomerName}", CustomerName,
                             "{PONumber}", PONumber,
                             "{ToolReceiptDate}", ToolReceiptDate,
                             "{Place}", Place,
                             "{CheckedBy}", CheckedBy,
                             "{CheckedByDesignation}", CheckedByDesignation,
                             "{CustomerContact}", CustomerContact,
                             "{CustomerContactDesignation}", CustomerContactDesignation,
                             "{Notes}", Notes,
                             "{Details}", sb.ToString(),
                             "{CalibrationScope}", sbCalibrationScope.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//ToolVerification/ToolVerification-" + QuotationID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//ToolVerification/ToolVerification-" + QuotationID + ".htm"));
                        divAllocation.InnerHtml = html;
                        GC.Collect();
                        string url = "ToolsVerification.aspx?q=" + hdQuotationID.Value;
                        Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");

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