using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static iTextSharp.text.pdf.AcroFields;
using static System.Net.Mime.MediaTypeNames;

namespace LIMS
{
    public partial class DebitNote : System.Web.UI.Page
    {
        DAL.APModule _ap = new DAL.APModule();
        DAL.CM _cm = new DAL.CM();
        private List<DebitNoteDetail> ItemList
        {
            get { return (List<DebitNoteDetail>)ViewState["ItemList"]; }
            set { ViewState["ItemList"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadVendors();
                ItemList = new List<DebitNoteDetail>();
                if (Request.QueryString["d"] != null)
                {
                    string DebitNoteNo = _cm.Decode(Request.QueryString["d"]);
                    BindDebitNote(DebitNoteNo);
                    divAdd.Visible = false;
                    btnSave.Visible = false;
                }
                else
                {
                    divAdd.Visible = true;
                    txtDebitNoteNo.Text = GenerateDebitNoteNo();
                    txtDebitNoteDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }
        }
        protected void BindDebitNote(string DebitNote)
        {
            string DebitNoteId = "";
            DataTable dt=_ap.GetDebitNotes(DebitNote);
            if (dt.Rows.Count > 0)
            {
                txtDebitNoteDate.Text = Convert.ToDateTime(dt.Rows[0]["DebitNoteDate"]).ToString("yyyy-MM-dd");
                txtDebitNoteNo.Text = dt.Rows[0]["DebitNoteNo"].ToString();
                ddlVendors.Items.FindByValue(dt.Rows[0]["SupplierID"].ToString()).Selected = true;
                txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();
                DebitNoteId = dt.Rows[0]["DebitNoteId"].ToString();
                dt = _ap.GetDebitNoteDetails(DebitNoteId);
                if (dt.Rows.Count > 0)
                {
                    gvItems.DataSource = dt;
                    gvItems.DataBind();
                }
                else
                {
                    gvItems.DataSource = null;
                    gvItems.DataBind();
                }
            }
        }
        string GenerateDebitNoteNo()
        {
            // Example simple auto-number
            return "DN-" + DateTime.Now.Ticks.ToString().Substring(10);
        }
        private void LoadVendors()
        {
            var vendors = _ap.GetVendors();

            ddlVendors.DataSource = vendors;
            ddlVendors.DataTextField = "VendorName";
            ddlVendors.DataValueField = "VendorId";
            ddlVendors.DataBind();
            ddlVendors.Items.Insert(0, "Select Vendor");
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            SaveRepeaterValues();
        }

        void SaveRepeaterValues()
        {         
            if ((txtItem.Text != "") && (txtQty.Text != "") && (txtRate.Text != ""))
            {
                ItemList.Add(new DebitNoteDetail());
                var detail = ItemList[gvItems.Rows.Count];
                detail.ItemID = gvItems.Rows.Count;
                detail.ItemName = txtItem.Text;
                detail.Qty = Convert.ToDecimal(txtQty.Text);
                detail.Rate = Convert.ToDecimal(txtRate.Text);
                BindRepeaterAndCalculate();
                Clear();
            }
        }
        protected void Clear()
        {
            txtItem.Text = "";
            txtQty.Text = "";
            txtRate.Text = "";
        }
        protected void QtyRate_TextChanged(object sender, EventArgs e)
        {
            SaveRepeaterValues();
        }

        void BindRepeaterAndCalculate()
        {
            gvItems.DataSource = ItemList;
            gvItems.DataBind();

            decimal total = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                total += ItemList[i].Amount;
            }

            lblTotal.Text = total.ToString("0.00");
        }

        protected void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                ItemList.RemoveAt(index);
                BindRepeaterAndCalculate();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtDebitNoteNo.Text == "")
            {
                flag = 1;
                txtDebitNoteNo.BorderColor = Color.Red;
            }
            else
            {
                txtDebitNoteNo.BorderColor = Color.Empty;
            }
            if (txtDebitNoteDate.Text == "")
            {
                flag = 1;
                txtDebitNoteDate.BorderColor = Color.Red;
            }
            else
            {
                txtDebitNoteDate.BorderColor = Color.Empty;
            }
            if (ddlVendors.SelectedIndex==0)
            {
                flag = 1;
                ddlVendors.BorderColor = Color.Red;
            }
            else
            {
                ddlVendors.BorderColor = Color.Empty;
            }
            if (flag == 0)
            {
                DebitNoteMaster master = new DebitNoteMaster
                {
                    DebitNoteNo = txtDebitNoteNo.Text,
                    DebitNoteDate = Convert.ToDateTime(txtDebitNoteDate.Text),
                    SupplierID = Convert.ToInt32(ddlVendors.SelectedValue),
                    Remarks = txtRemarks.Text,
                    TotalAmount = Convert.ToDecimal(lblTotal.Text),
                    CreatedBy = "Admin"
                };
                int id = _ap.SaveDebitNote(master, ItemList);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Data Saved Successfully)", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }           
        }
    }
}