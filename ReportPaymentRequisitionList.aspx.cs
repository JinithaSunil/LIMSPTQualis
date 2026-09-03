using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

namespace LIMS
{
    public partial class ReportPaymentRequisitionList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        protected void BindList()
        {
            if ((txtFromDate.Text != "") && (txtToDate.Text != ""))
            {
                _cm.FromDate = Convert.ToDateTime(txtFromDate.Text);
                _cm.ToDate = Convert.ToDateTime(txtToDate.Text);
                DataTable dt = _cm.GetPaymentRequisitionList(_cm);
                if (dt.Rows.Count > 0)
                {
                    ViewState["dirState"] = dt;
                    ViewState["sortdr"] = "Asc";
                    gvList.DataSource = dt;
                    gvList.DataBind();
                }

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindList();
        }

        protected void gvList_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvList.DataSource = dtrslt;
                gvList.DataBind();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if ((txtFromDate.Text != "") && (txtToDate.Text != ""))
            {
                _cm.FromDate = Convert.ToDateTime(txtFromDate.Text);
                _cm.ToDate = Convert.ToDateTime(txtToDate.Text);
                DataTable dt = _cm.GetPaymentRequisitionList(_cm);
                if (dt.Rows.Count > 0)
                {

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "PR Report");

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=PaymentRequisition_Report.xlsx");

                        using (MemoryStream ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            ms.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }
    }
}