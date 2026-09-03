using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class DebitNotes : System.Web.UI.Page
    {
        DAL.APModule _ap = new DAL.APModule();
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = _ap.GetDebitNotes();
                if (dt.Rows.Count > 0)
                {
                    gvDebitNote.DataSource = dt;
                    gvDebitNote.DataBind();
                }
                else
                {
                    gvDebitNote.DataSource = null;
                    gvDebitNote.DataBind();
                }
            }
        }

        protected void btnNewDebitNote_Click(object sender, EventArgs e)
        {
            Response.Redirect("DebitNote.aspx");
        }

        protected void gvDebitNote_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string DebitNoteNo = "";

            GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            HiddenField hdDebitNoteID = row.FindControl("hdDebitNoteID") as HiddenField;
            LinkButton lnkDN = row.FindControl("lnkDN") as LinkButton;
            DebitNoteNo=lnkDN.Text;

            if (e.CommandName == "View")
            {
                string fileOpen = "/DebitNote/" + hdDebitNoteID.Value + ".htm";
                string script = $"window.open('{fileOpen}', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "OpenFile", script, true);
            }
            if (e.CommandName == "GetDebitNote")
            {
               
                DebitNoteNo = _cm.Encode(DebitNoteNo);

                Response.Redirect("DebitNote.aspx?d="+ DebitNoteNo);
            }
        }
    }
}