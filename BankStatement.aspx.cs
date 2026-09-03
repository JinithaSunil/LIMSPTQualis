using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class BankStatement : System.Web.UI.Page
    {
        DAL.BankingModule _bk= new DAL.BankingModule();
        int BankAccountID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                if (Session["BankAccountID"] != null)
                {
                    BankAccountID = Convert.ToInt32(Session["BankAccountID"].ToString());
                    DataTable dt = _bk.GetBankAccount(BankAccountID.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        lblBankName.Text = dt.Rows[0]["BankName"].ToString();
                        DataTable dtStatement = _bk.GetBankStatement(BankAccountID);
                        if (dtStatement.Rows.Count > 0)
                        {
                            gvBankStatement.DataSource = dtStatement;
                            gvBankStatement.DataBind();
                        }
                        else
                        {
                            gvBankStatement.DataSource = null;
                            gvBankStatement.DataBind();
                        }
                    }
                }
            }
        }
    }
}