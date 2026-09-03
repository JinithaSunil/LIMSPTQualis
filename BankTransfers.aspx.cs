using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class BankTransfers : System.Web.UI.Page
    {
        DAL.GLModule _gl = new DAL.GLModule();
        protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                if (radChoice.SelectedIndex != -1)
                {
                    txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    DataTable dt = _gl.GetBankTransfers(radChoice.SelectedValue);
                    BindBankTransfer(dt);
                }
            }
		}
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
        protected void Search()
        {
            DAL.GLModule.BankTransfer bnk = new DAL.GLModule.BankTransfer();
            bnk.FromDate = Convert.ToDateTime(txtFromDate.Text);
            bnk.ToDate = Convert.ToDateTime(txtToDate.Text);

            DataTable dt = _gl.GetBankTransfers(bnk, radChoice.SelectedValue);
            BindBankTransfer(dt);

        }
        private void BindBankTransfer(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                gvBankTransfers.DataSource = dt;
                gvBankTransfers.DataBind();
            }
            else
            {
                gvBankTransfers.DataSource = null;
                gvBankTransfers.DataBind();
            }
        }

      
        protected void radChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = _gl.GetBankTransfers(radChoice.SelectedValue);
            BindBankTransfer(dt);
        }
    }
}