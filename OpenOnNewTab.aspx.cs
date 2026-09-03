using System;
using System.Data;

namespace LIMS
{
    public partial class OpenOnNewTab : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null || Request.QueryString["TestResultID"] != null)
            {
                if (Request.QueryString["ID"] != null)
                {
                    string SampleId = Request.QueryString["ID"].ToString();
                    string TestResultId = Request.QueryString["TestResultID"].ToString(); string code = "";

                    DataTable dt = _cm.NABLOrNonNABL(SampleId);
                    if (dt.Rows.Count == 2)
                    {
                        for(int i=1;i<= dt.Rows.Count; i++)
                        {
                            string url = "TestReportChemical.aspx?s=" + SampleId + "&t=" + TestResultId + "&t1=" + i;
                            Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                        }                        
                    }
                    else
                    {
                        if (dt.Rows[0]["NABLOrNonNABL"].ToString() == "NABL")
                        {
                            string url = "TestReportChemical.aspx?s=" + SampleId + "&t=" + TestResultId + "&t1=1";
                            Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                        }
                        else if (dt.Rows[0]["NABLOrNonNABL"].ToString() == "Non NABL")
                        {
                            string url = "TestReportChemical.aspx?s=" + SampleId + "&t=" + TestResultId + "&t1=2";
                            Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                        }
                        
                    }

                }
            }
        }
    }
}