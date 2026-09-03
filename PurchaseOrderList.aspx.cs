using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LIMS
{
    public partial class PurchaseOrderList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();
        String StoreType = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    BindVendorsPO();
                    BindPO();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        protected void BindVendorsPO()
        {
            DataTable dt = _cm.GetVendors();
            if (dt.Rows.Count > 0)
            {
                ddlVendor1.DataSource = dt;
                ddlVendor1.DataTextField = "VendorName";
                ddlVendor1.DataValueField = "VendorID";
                ddlVendor1.DataBind();
                ddlVendor1.Items.Insert(0, new ListItem("Select Vendor"));
            }
        }
        protected void btnGet_Click(object sender, EventArgs e)
        {
            string sQueryAddFrom = "", sQueryAddTo = "", sQueryAddPONumber = "",
                   sQueryAddProjecID = "", sQueryAddUnit = "", sQueryAddVendorID = "",
                   sQueryStoreType = "", sQueryAddAmount = "";

            DateTime FromDate, ToDate;
            string poNumber = "";
            decimal amount = 0;

            if (!string.IsNullOrWhiteSpace(txtFromDate.Text))
            {
                FromDate = Convert.ToDateTime(txtFromDate.Text);
                sQueryAddFrom = " po.CreatedDate >= '" + FromDate.ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrWhiteSpace(txtToDate.Text))
            {
                ToDate = Convert.ToDateTime(txtToDate.Text);
                sQueryAddTo = " po.CreatedDate <= '" + ToDate.ToString("yyyy-MM-dd") + "'";
            }

            if (!string.IsNullOrWhiteSpace(txtponumber.Text))
            {
                poNumber = txtponumber.Text;
                sQueryAddPONumber = " po.PONumber LIKE '%" + poNumber + "%'";
            }
            
            if (!string.IsNullOrWhiteSpace(txtAmountGreaterThan.Text))
            {
                amount = Convert.ToDecimal(txtAmountGreaterThan.Text);
                sQueryAddAmount = " (pd.rate * pd.quantity) >= " + amount;
            }
            if (ddlVendor1.SelectedIndex != 0)
            {
                sQueryAddVendorID = " vd.VendorID = '" + ddlVendor1.SelectedValue + "'";
            }
            if (Session["StoreType"] != null)
            {
                sQueryStoreType = " po.StoreType IN('" + Session["StoreType"].ToString() + "')";
            }

            DataTable dt = _cm.GetAllPO(sQueryAddFrom, sQueryAddTo, sQueryAddPONumber,
                                        sQueryAddUnit, sQueryAddVendorID, sQueryStoreType, sQueryAddAmount, sQueryAddProjecID);

            gvPO.DataSource = dt;
            gvPO.DataBind();
            lblRecords.Text = $"{gvPO.Rows.Count} records found";
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlVendor1.SelectedIndex = 0;
            txtponumber.Text = "";
            txtAmountGreaterThan.Text = "";

            // Clear and rebind the grid
            gvPO.DataSource = null;
            gvPO.DataBind();
            BindPO();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for exporting GridView to Excel
        }
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            if (gvPO.Rows.Count > 0)
            {
                // Get current filter values
                string sQueryAddFrom = "", sQueryAddTo = "", sQueryAddPONumber = "",
                       sQueryAddProjecID = "", sQueryAddUnit = "", sQueryAddVendorID = "",
                       sQueryStoreType = "", sQueryAddAmount = "";
                DateTime FromDate, ToDate;
                decimal amount = 0;

                if (!string.IsNullOrWhiteSpace(txtFromDate.Text))
                {
                    FromDate = Convert.ToDateTime(txtFromDate.Text);
                    sQueryAddFrom = " po.CreatedDate >= '" + FromDate.ToString("yyyy-MM-dd") + "'";
                }

                if (!string.IsNullOrWhiteSpace(txtToDate.Text))
                {
                    ToDate = Convert.ToDateTime(txtToDate.Text);
                    sQueryAddTo = " po.CreatedDate <= '" + ToDate.ToString("yyyy-MM-dd") + "'";
                }

                if (!string.IsNullOrWhiteSpace(txtponumber.Text))
                {
                    sQueryAddPONumber = " po.PONumber LIKE '%" + txtponumber.Text + "%'";
                }

                if (!string.IsNullOrWhiteSpace(txtAmountGreaterThan.Text))
                {
                    amount = Convert.ToDecimal(txtAmountGreaterThan.Text);
                    sQueryAddAmount = " (pd.rate * pd.quantity) >= " + amount;
                }

                if (ddlVendor1.SelectedIndex != 0)
                {
                    sQueryAddVendorID = " vd.VendorID = '" + ddlVendor1.SelectedValue + "'";
                }

                if (Session["StoreType"] != null)
                {
                    sQueryStoreType = " po.StoreType IN('" + Session["StoreType"].ToString() + "')";
                }

                // Get fresh data
                DataTable dt = _cm.GetAllPO(sQueryAddFrom, sQueryAddTo, sQueryAddPONumber,
                                        sQueryAddUnit, sQueryAddVendorID, sQueryStoreType, sQueryAddAmount, sQueryAddProjecID);

                // Create a new GridView for export to avoid affecting the page GridView
                GridView gvExport = new GridView();
                gvExport.AutoGenerateColumns = false;

                // Add only the columns you want to export
                BoundField colPONumber = new BoundField();
                colPONumber.DataField = "PONumber";
                colPONumber.HeaderText = "PO#";
                gvExport.Columns.Add(colPONumber);

                BoundField colPODate = new BoundField();
                colPODate.DataField = "PODate";
                colPODate.HeaderText = "PO Date";
                colPODate.DataFormatString = "{0:dd MMM yyyy}";
                gvExport.Columns.Add(colPODate);

                BoundField colDeliveryDate = new BoundField();
                colDeliveryDate.DataField = "DeliveryDate";
                colDeliveryDate.HeaderText = "Delivery Date(Max)";
                colDeliveryDate.DataFormatString = "{0:dd MMM yyyy}";
                gvExport.Columns.Add(colDeliveryDate);

                BoundField colVendorName = new BoundField();
                colVendorName.DataField = "VendorName";
                colVendorName.HeaderText = "Vendor Name";
                gvExport.Columns.Add(colVendorName);

                BoundField colPOStatus = new BoundField();
                colPOStatus.DataField = "POStatus";
                colPOStatus.HeaderText = "PO Status";
                gvExport.Columns.Add(colPOStatus);

                BoundField colReason = new BoundField();
                colReason.DataField = "Reason";
                colReason.HeaderText = "Remarks";
                gvExport.Columns.Add(colReason);

                // Bind data to export GridView
                gvExport.DataSource = dt;
                gvExport.DataBind();

                // Style the export GridView
                gvExport.GridLines = GridLines.Both;
                gvExport.HeaderStyle.Font.Bold = true;
                gvExport.HeaderStyle.BackColor = System.Drawing.Color.LightGray;

                // Setup Response
                Response.Clear();
                Response.Buffer = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                string FileName = "PendingPOList_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);

                // Create StringWriter and HtmlTextWriter
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                // Write the meta tag for UTF-8 support
                htw.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>");

                // Render the GridView as HTML table
                gvExport.RenderControl(htw);

                // Write to response
                Response.Write(sw.ToString());
                Response.End();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('No records to download!');", true);
            }
        }
        private void BindPO()
        {
            string Unit = "";
            string sQueryStoreType = "";

            if (Session["Unit"] != null)
            {
                Unit = Session["Unit"].ToString();
            }

            if (Session["DeptName"] != null)
            {
                switch (Session["DeptName"].ToString())
                {
                    case "Purchase":
                        StoreType = "Engineering";
                        Session["StoreType"] = "Engineering";
                        Unit = "1,2,3";
                        break;
                    case "RM Store":
                        StoreType = "RM";
                        Session["StoreType"] = "RM";
                        Unit = "1,2,3";
                        break;
                    default:
                        StoreType = "Engineering";
                        if (Session["Unit"] != null)
                        {
                            Unit = Session["Unit"].ToString();
                        }
                        break;
                }
            }

            // Apply StoreType filter if session exists
            if (Session["StoreType"] != null)
            {
                sQueryStoreType = " po.StoreType IN('" + Session["StoreType"].ToString() + "')";
            }

            // Bind all PO records with only StoreType filter (if applicable)
            DataTable dt = _cm.GetAllPO("", "", "", "", "", sQueryStoreType, "", "");
            gvPO.DataSource = dt;
            gvPO.DataBind();
            lblRecords.Text = $"{gvPO.Rows.Count} records found";
        }

        protected void gvPO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable dt = new DataTable();
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblPurchaseOrderID = (e.Row.FindControl("lblPurchaseOrderID") as Label);
                Label lblPODate = (e.Row.FindControl("lblPODate") as Label);
                Label lblDeliveryDate = (e.Row.FindControl("lblDeliveryDate") as Label);
                Label lblPOStatus = (e.Row.FindControl("lblPOStatus") as Label);
                Label lblDaysLeft = (e.Row.FindControl("lblDaysLeft") as Label);
                LinkButton lnkAmendPO = (e.Row.FindControl("lnkAmendPO") as LinkButton);

                string PurchaseOrderID = lblPurchaseOrderID.Text;
                dt = _cm.getMaxPOStatus(PurchaseOrderID);
                if (dt.Rows.Count > 0)
                {
                    lblPOStatus.Text = dt.Rows[0]["POStatus"].ToString();
                    switch (lblPOStatus.Text)
                    {
                        case "PO Generated":
                            lblDaysLeft.Text = ((int)Math.Round((Convert.ToDateTime(lblDeliveryDate.Text) - DateTime.Now).TotalDays)).ToString();
                            if (DateTime.Now > Convert.ToDateTime(lblDeliveryDate.Text))
                            {
                                e.Row.BackColor = System.Drawing.Color.LightPink;
                            }
                            break;
                        case "Approved":
                        case "Sent":
                            if (lblDeliveryDate.Text == "")
                            {
                                lblDaysLeft.Text = "";
                            }
                            else
                            {
                                if (DateTime.Now > Convert.ToDateTime(lblDeliveryDate.Text))
                                {
                                    e.Row.BackColor = System.Drawing.Color.LightPink;
                                }
                                lblDaysLeft.Text = ((int)Math.Round((Convert.ToDateTime(lblDeliveryDate.Text) - DateTime.Now).TotalDays)).ToString();
                            }
                            break;
                        case "Full Received":
                            break;
                        case "Amend PO":
                            if (Session["DeptName"] != null)
                            {
                                switch (Session["DeptName"].ToString())
                                {
                                    case "Purchase":
                                        lnkAmendPO.Visible = true;
                                        break;
                                    case "RM Store":
                                        if ((Session["Usertype"].ToString() == "GM") && (Session["DeptName"].ToString() == "RM Store"))
                                        {
                                            lnkAmendPO.Visible = true;
                                        }
                                        break;
                                    default:
                                        lnkAmendPO.Visible = false;
                                        break;

                                }
                            }
                            break;
                        default: break;
                    }
                }
            }
        }

        protected void gvPO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPO.PageIndex = e.NewPageIndex;
            BindPO();
        }
        protected void gvPO_Sorting(object sender, GridViewSortEventArgs e)
        {
            // Get the current data
            string sQueryAddFrom = "", sQueryAddTo = "", sQueryAddPONumber = "",
                   sQueryAddProjecID = "", sQueryAddUnit = "", sQueryAddVendorID = "",
                   sQueryStoreType = "", sQueryAddAmount = "";

            DateTime FromDate, ToDate;
            string poNumber = "";
            decimal amount = 0;

            // Apply the same filters
            if (!string.IsNullOrWhiteSpace(txtFromDate.Text))
            {
                FromDate = Convert.ToDateTime(txtFromDate.Text);
                sQueryAddFrom = " po.CreatedDate >= '" + FromDate.ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrWhiteSpace(txtToDate.Text))
            {
                ToDate = Convert.ToDateTime(txtToDate.Text);
                sQueryAddTo = " po.CreatedDate <= '" + ToDate.ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrWhiteSpace(txtponumber.Text))
            {
                poNumber = txtponumber.Text;
                sQueryAddPONumber = " po.PONumber LIKE '%" + poNumber + "%'";
            }
            if (!string.IsNullOrWhiteSpace(txtAmountGreaterThan.Text))
            {
                amount = Convert.ToDecimal(txtAmountGreaterThan.Text);
                sQueryAddAmount = " (pd.rate * pd.quantity) >= " + amount;
            }
            if (ddlVendor1.SelectedIndex != 0)
            {
                sQueryAddVendorID = " vd.VendorID = '" + ddlVendor1.SelectedValue + "'";
            }
            if (Session["StoreType"] != null)
            {
                sQueryStoreType = " po.StoreType IN('" + Session["StoreType"].ToString() + "')";
            }

            DataTable dt = _cm.GetAllPO(sQueryAddFrom, sQueryAddTo, sQueryAddPONumber,
                                        sQueryAddUnit, sQueryAddVendorID, sQueryStoreType, sQueryAddAmount, sQueryAddProjecID);

            if (dt.Rows.Count > 0)
            {
                DataView dv = new DataView(dt);

                // Determine sort direction
                string sortDirection = "ASC";
                if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == e.SortExpression)
                {
                    sortDirection = ViewState["SortDirection"] != null && ViewState["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
                }

                dv.Sort = e.SortExpression + " " + sortDirection;

                // Store sort expression and direction in ViewState
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["SortDirection"] = sortDirection;

                gvPO.DataSource = dv;
                gvPO.DataBind();
                lblRecords.Text = $"{gvPO.Rows.Count} records found";
            }
        }
        protected void btnAmendPO_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvPO.Rows)
            {
                Label lblPurchaseOrderID = gvPO.FindControl("lblPurchaseOrderID") as Label;
                Label lblPONumber = gvPO.FindControl("lblPONumber") as Label;
                Session["PONumber"] = lblPONumber.Text;
                Session["PurchaseOrderID"] = lblPurchaseOrderID.Text;
                DataTable dtPO = _cm.getPOEnquiryVendor(lblPurchaseOrderID.Text);
                if (dtPO.Rows.Count > 0)
                {
                    Session["EnquiryID"] = dtPO.Rows[0]["EnquiryID"].ToString();
                    Session["VendorID"] = dtPO.Rows[0]["VendorID"].ToString();
                    Response.Redirect("PurchaseOrder.aspx?ID=A");
                }

            }

        }

        protected void gvPO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AmendPO")
            {
                GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
                TextBox txtPassword = (TextBox)row.FindControl("txtPassword");
                Label lblPurchaseOrderID = (Label)row.FindControl("lblPurchaseOrderID");
                Label lblPONumber = (Label)row.FindControl("lblPONumber");
                Session["PONumber"] = lblPONumber.Text;
                Session["PurchaseOrderID"] = lblPurchaseOrderID.Text;
                DataTable dtPO = _cm.getPOEnquiryVendor(lblPurchaseOrderID.Text);
                if (dtPO.Rows.Count > 0)
                {
                    Session["EnquiryID"] = dtPO.Rows[0]["EnquiryID"].ToString();
                    Session["VendorID"] = dtPO.Rows[0]["VendorID"].ToString();
                    Response.Redirect("PurchaseOrder.aspx?ID=A");
                }
            }
        }
    }
}