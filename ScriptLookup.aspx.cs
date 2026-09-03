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
    public partial class ScriptLookup : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); 
        protected void Page_Load(object sender, EventArgs e)
        {

            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liViewSamples2 = (HtmlGenericControl)this.Master.FindControl("liViewSamples2");
			HtmlControl liAllSamples2 = (HtmlGenericControl)this.Master.FindControl("liAllSamples2");

            if (!Page.IsPostBack)
            {
                liSampleManagement1.Attributes.Add("class", "active");
                liViewSamples2.Attributes.Add("class", "active");

                BindScriptLookup();
                BindScriptSample();
            }
        }
        private void BindScriptLookup()
        {
            DataTable dt = _cm.GetScriptCodes();
            gvScriptLookUp.DataSource = dt;
            gvScriptLookUp.DataBind();
        }
        private void BindScriptSample()
        {
            DataTable dt = _cm.GetScriptSample();
            gvScriptSample.DataSource = dt;
            gvScriptSample.DataBind();
        }

        protected void gvScriptLookUp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = 0;
            string scriptHex = "";
            if (e.CommandName == "getScript")
            {
                rowIndex = int.Parse(e.CommandArgument.ToString());
                scriptHex = (string)this.gvScriptLookUp.DataKeys[rowIndex]["scriptHex"];
                txtInput.Text += System.Convert.ToChar(System.Convert.ToUInt32(scriptHex, 16)).ToString() + " ";
            }
        }
        protected void gvScriptSample_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int ID = Convert.ToInt32(e.CommandArgument);
                _cm.DeleteScriptSample(ID);
                BindScriptSample();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            _cm.ScriptSample = txtType.Text;
            _cm.AddScriptCode(_cm);
            BindScriptSample();
        }

        protected void gvScriptLookUp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int index = e.Row.RowIndex;
                Label lblSymbol = (e.Row.FindControl("lblSymbol") as Label);
                string value = lblSymbol.Text;
                lblSymbol.Text = System.Convert.ToChar(System.Convert.ToUInt32(value, 16)).ToString();
            }
        }

        protected void gvScriptSample_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvScriptSample.DataKeys[e.RowIndex].Value.ToString());
            _cm.DeleteScriptSample(ID);
            BindScriptSample();
        }

        protected void txtType_TextChanged(object sender, EventArgs e)
        {
            string text = txtType.Text;//caret symbol Shift + 6
            txtType.Text = _cm.ReplaceSuperScript(text);
            
        }
    }
}