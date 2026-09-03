using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LIMS
{
    public partial class ViewPO : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if ((Request.QueryString["P"] != null) && (Request.QueryString["R"] != null))
                {
                    string PurchaseOrderID1 = Request.QueryString["P"].ToString();
                    string Reference = Request.QueryString["R"].ToString();
                    _cm.PurchaseOrderID = Convert.ToInt32(PurchaseOrderID1);

                    DataTable dtCheckVendor = _cm.CheckValidVendor(_cm);
                    if (dtCheckVendor.Rows.Count > 0)
                    {
                        if (Reference == dtCheckVendor.Rows[0]["Reference"].ToString())
                        {
                            DataTable dt = _cm.GetPOReportFileName(PurchaseOrderID1);
                            if (dt.Rows.Count > 0)
                            {
                                string ReportFileName = dt.Rows[0]["ReportFileName"].ToString();
                                Session["ReportFileName"] = ReportFileName;
                                if (ReportFileName.ToString() != "")
                                {
                                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PO/" + ReportFileName + ".htm"));
                                    divPO.InnerHtml = html;
                                }
                                else
                                {
                                    divPO.InnerHtml = "<h4>PO not Generated</h4>";
                                }
                            }
                        }
                        else
                        {
                            Response.Redirect("Login.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect("Login.aspx");
                    }
                }
                else if (Session["UserID"] != null)
                {
                    if (Request.QueryString["PurchaseOrderID"] != null)
                    {
                        string PurchaseOrderID = Request.QueryString["PurchaseOrderID"].ToString();
                        DataTable dt = _cm.GetPOReportFileName(PurchaseOrderID);
                        if (dt.Rows.Count > 0)
                        {
                            string ReportFileName = dt.Rows[0]["ReportFileName"].ToString();
                            Session["ReportFileName"] = ReportFileName;
                            if (ReportFileName.ToString() != "")
                            {
                                string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PO/" + ReportFileName + ".html"));
                                divPO.InnerHtml = html;
                            }
                            else
                            {
                                divPO.InnerHtml = "<h4>PO not Generated</h4>";
                            }
                        }
                    }
                    else
                    {
                        Response.Redirect("EnquiryList.aspx");
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            string ReportFileName = Session["ReportFileName"].ToString();
            btnExportToExcel.Visible = false;
            Response.AppendHeader("content-disposition", "attachment;filename=" + ReportFileName + ".xls");

            Response.Charset = "";

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.ContentType = "application/vnd.ms-excel";

            this.EnableViewState = false;

            Response.Write(AddExcelStyling());

            Response.Write(divPO.InnerHtml);

            Response.End();
        }
        private string AddExcelStyling()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office'\n" +

            "xmlns:x='urn:schemas-microsoft-com:office:excel'\n" +
            "xmlns='http://www.w3.org/TR/REC-html40'>\n" +

            "<head>\n");
            sb.Append("<style>\n");

            sb.Append("@page");
            sb.Append("{margin:.5in .75in .5in .75in;\n");

            sb.Append("mso-header-margin:.5in;\n");
            sb.Append("mso-footer-margin:.5in;\n");

            sb.Append("mso-page-orientation:landscape;}\n");
            sb.Append("</style>\n");

            sb.Append("<!--[if gte mso 9]><xml>\n");
            sb.Append("<x:ExcelWorkbook>\n");

            sb.Append("<x:ExcelWorksheets>\n");
            sb.Append("<x:ExcelWorksheet>\n");

            sb.Append("<x:Name>Projects 3 </x:Name>\n");
            sb.Append("<x:WorksheetOptions>\n");

            sb.Append("<x:Print>\n");
            sb.Append("<x:ValidPrinterInfo/>\n");

            sb.Append("<x:PaperSizeIndex>9</x:PaperSizeIndex>\n");
            sb.Append("<x:HorizontalResolution>600</x:HorizontalResolution\n");

            sb.Append("<x:VerticalResolution>600</x:VerticalResolution\n");
            sb.Append("</x:Print>\n");

            sb.Append("<x:Selected/>\n");
            sb.Append("<x:DoNotDisplayGridlines/>\n");

            sb.Append("<x:ProtectContents>False</x:ProtectContents>\n");
            sb.Append("<x:ProtectObjects>False</x:ProtectObjects>\n");

            sb.Append("<x:ProtectScenarios>False</x:ProtectScenarios>\n");
            sb.Append("</x:WorksheetOptions>\n");

            sb.Append("</x:ExcelWorksheet>\n");
            sb.Append("</x:ExcelWorksheets>\n");

            sb.Append("<x:WindowHeight>12780</x:WindowHeight>\n");
            sb.Append("<x:WindowWidth>19035</x:WindowWidth>\n");

            sb.Append("<x:WindowTopX>0</x:WindowTopX>\n");
            sb.Append("<x:WindowTopY>15</x:WindowTopY>\n");

            sb.Append("<x:ProtectStructure>False</x:ProtectStructure>\n");
            sb.Append("<x:ProtectWindows>False</x:ProtectWindows>\n");

            sb.Append("</x:ExcelWorkbook>\n");
            sb.Append("</xml><![endif]-->\n");

            sb.Append("</head>\n");
            sb.Append("<body>\n");

            return sb.ToString();
        }

        protected void btnExportToPDF_Click(object sender, EventArgs e)
        {

        }
    }
}