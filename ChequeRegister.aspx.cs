using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace LIMS
{
	public partial class ChequeRegister : System.Web.UI.Page
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
                    DataTable dt = _gl.getChequeDetails(radChoice.SelectedValue);
                    BindChequeRegister(dt);
                }
			}
		}
        private void BindChequeStatus(RadioButtonList radChequeStatus)
        {
            radChequeStatus.Items.Clear();

            if (radChoice.SelectedValue == "RECEIPT")
            {
                radChequeStatus.Items.Add(new ListItem("Received", "Received"));
                radChequeStatus.Items.Add(new ListItem("Presented", "Presented"));
                radChequeStatus.Items.Add(new ListItem("Cleared", "Cleared"));
                radChequeStatus.Items.Add(new ListItem("Bounced", "Bounced"));
            }
            else if (radChoice.SelectedValue == "PAYMENT")
            {
                radChequeStatus.Items.Add(new ListItem("Issued", "Issued"));
                radChequeStatus.Items.Add(new ListItem("Presented", "Presented"));
                radChequeStatus.Items.Add(new ListItem("Cleared", "Cleared"));
                radChequeStatus.Items.Add(new ListItem("Bounced", "Bounced"));
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
        protected void Search()
        {
            DAL.GLModule.Cheque chq = new DAL.GLModule.Cheque();
            chq.FromDate = Convert.ToDateTime(txtFromDate.Text);
            chq.ToDate = Convert.ToDateTime(txtToDate.Text);
            chq.BankAccountID = txtBank.Text;
            chq.Status = ddlStatus.SelectedItem.ToString();
            chq.ChequeNo = txtChequeNo.Text;

            DataTable dt = _gl.getChequeDetails(chq,radChoice.SelectedValue);
            BindChequeRegister(dt);

        }
        private void BindChequeRegister(DataTable dt)
        {            
            if (dt.Rows.Count > 0)
            {
                gvChequeRegister.DataSource = dt;
                gvChequeRegister.DataBind();
            }
            else
            {
                gvChequeRegister.DataSource = null;
                gvChequeRegister.DataBind();
            }
        }
     
        protected void gvChequeRegister_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string status = DataBinder.Eval(e.Row.DataItem, "ChequeStatus").ToString();
                HiddenField hdChequeStatus = (HiddenField)e.Row.FindControl("hdChequeStatus");
                RadioButtonList radChequeStatus = (RadioButtonList)e.Row.FindControl("radChequeStatus");
                BindChequeStatus(radChequeStatus);
                radChequeStatus.Items.FindByText(hdChequeStatus.Value).Selected = true;

                HtmlGenericControl container =(HtmlGenericControl)e.Row.FindControl("chequeDatesDiv");
               
            }
        }

        protected void gvChequeRegister_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int flag = 0;
            if (e.CommandName == "UpdateStatus")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string ChequeID = gvChequeRegister.DataKeys[rowIndex].Value.ToString();

                GridViewRow row = gvChequeRegister.Rows[rowIndex];
                RadioButtonList radChequeStatus = (RadioButtonList)row.FindControl("radChequeStatus");
                TextBox txtDate = (TextBox)row.FindControl("txtDate");

                if ((txtDate != null) && (radChequeStatus != null))
                {
                    if (txtDate.Text == "") { txtDate.BorderColor = Color.Red; flag = 1; } else { txtDate.BorderColor = Color.Empty; }
                    if (flag == 0)
                    {
                        DAL.GLModule.Cheque chq = new DAL.GLModule.Cheque();
                        chq.ChequeID = Convert.ToInt32(ChequeID);
                        chq.StatusChangeDate = Convert.ToDateTime(txtDate.Text);
                        chq.Status = radChequeStatus.SelectedItem.ToString();
                        _gl.UpdateChequeDetails(chq);
                        DataTable dt = _gl.getChequeDetails(radChoice.SelectedValue);
                        BindChequeRegister(dt);
                    }
                }
            }
        }

        protected void radChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = _gl.getChequeDetails(radChoice.SelectedValue);
            BindChequeRegister(dt);
        }
    }
}