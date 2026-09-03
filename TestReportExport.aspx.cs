using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMSStaging
{
    public partial class TestReportExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestReport/TestReport-60337779275.htm"));
            ExportDiv.InnerHtml = html;
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            Response.AppendHeader("content-disposition", "attachment;filename=ExportedHtml.xls");

            Response.Charset = "";

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.ContentType = "application/vnd.ms-excel";

            this.EnableViewState = false;

            Response.Write(ExportDiv.InnerHtml);

            Response.End();
        }
    }
}