using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace LIMS
{
    public partial class PurchaseOrderListPending : System.Web.UI.Page
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
                    BindAllPO();
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
                ddlVendor.DataSource = dt;
                ddlVendor.DataTextField = "VendorName";
                ddlVendor.DataValueField = "VendorID";
                ddlVendor.DataBind();
                ddlVendor.Items.Insert(0, new ListItem("Select Vendor"));
            }
        }
        protected void BindUserDetails(String UserID)
        {
            DataTable dt = _cm.GetEmployeeDetails(UserID);
            if (dt.Rows.Count > 0)
            {
                Session["Unit"] = dt.Rows[0]["JobLocation"].ToString();
                Session["EmpID"] = dt.Rows[0]["EmpID"].ToString();
                Session["DepartmentID"] = dt.Rows[0]["DepartmentID"].ToString();
            }
        }

        protected void btnGet_Click(object sender, EventArgs e)
        {
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

            if (ddlVendor.SelectedIndex != 0)
            {
                sQueryAddVendorID = " vd.VendorID = '" + ddlVendor.SelectedValue + "'";
            }

            if (Session["StoreType"] != null)
            {
                sQueryStoreType = " it.itemType IN(" + Session["StoreType"].ToString() + ")";
            }

            DataTable dt = _cm.GetAllPendingPO(sQueryAddFrom, sQueryAddTo, sQueryAddPONumber, "",
                                                sQueryAddUnit, sQueryAddVendorID, sQueryStoreType,
                                                sQueryAddAmount, sQueryAddProjecID);
            gvPO.DataSource = dt;
            gvPO.DataBind();
            lblRecords.Text = $"{gvPO.Rows.Count} records found";
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
           
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlVendor.SelectedIndex = 0;
            txtponumber.Text = "";
            BindAllPO();
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

                if (ddlVendor.SelectedIndex != 0)
                {
                    sQueryAddVendorID = " vd.VendorID = '" + ddlVendor.SelectedValue + "'";
                }

                if (Session["StoreType"] != null)
                {
                    sQueryStoreType = " it.itemType IN(" + Session["StoreType"].ToString() + ")";
                }

                // Turn off paging and sorting BEFORE Response operations
                gvPO.AllowPaging = false;
                gvPO.AllowSorting = false;

                // Rebind data with current filters
                DataTable dt = _cm.GetAllPendingPO(sQueryAddFrom, sQueryAddTo, sQueryAddPONumber, "",
                                                    sQueryAddUnit, sQueryAddVendorID, sQueryStoreType,
                                                    sQueryAddAmount, sQueryAddProjecID);
                gvPO.DataSource = dt;
                gvPO.DataBind();

                // Hide buttons and process grid for export
                foreach (GridViewRow row in gvPO.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        // Hide reminder button
                        Button btnReminder = (Button)row.FindControl("btnReminder");
                        if (btnReminder != null)
                        {
                            btnReminder.Visible = false;
                        }

                        // Show PO Number as text instead of hyperlink
                        HyperLink hypPO = (HyperLink)row.FindControl("hypPO");
                        Label lblPONumber = (Label)row.FindControl("lblPONumber");
                        if (hypPO != null && lblPONumber != null)
                        {
                            hypPO.Visible = false;
                            lblPONumber.Visible = true;
                        }

                        // Hide hidden labels
                        Label lblPurchaseOrderID = (Label)row.FindControl("lblPurchaseOrderID");
                        Label lblReportFileName = (Label)row.FindControl("lblReportFileName");
                        Label lblVendorID = (Label)row.FindControl("lblVendorID");

                        if (lblPurchaseOrderID != null) lblPurchaseOrderID.Visible = false;
                        if (lblReportFileName != null) lblReportFileName.Visible = false;
                        if (lblVendorID != null) lblVendorID.Visible = false;
                    }
                }

                gvPO.GridLines = GridLines.Both;
                gvPO.HeaderStyle.Font.Bold = true;

                // NOW start Response operations
                Response.Clear();
                Response.Buffer = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                string FileName = "PendingPOList_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls";
                StringWriter strwritter = new StringWriter();
                HtmlTextWriter htmltextwritter = new HtmlTextWriter(strwritter);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);

                // Inject meta tag for UTF-8 support
                Response.Write("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>");

                gvPO.RenderControl(htmltextwritter);
                Response.Write(strwritter.ToString());
                Response.End();

                // Re-enable paging and sorting for normal view
                gvPO.AllowPaging = true;
                gvPO.AllowSorting = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('No records to download!');", true);
            }
        }
        private void BindAllPO()
        {
            //if (Session["Unit"] != null)
            //{
                string Unit = "1";
                if (Session["DeptName"] != null)
                {
                    switch (Session["DeptName"].ToString())
                    {
                        case "Purchase":
                            StoreType = "Engineering";
                            Unit = "0,1,2,3";
                            break;
                        case "RM Store":
                            StoreType = "RM";
                            Unit = "1,2,3";
                            break;
                        default:
                            StoreType = "Engineering";
                        Unit = "1,2,3";
                        break;

                    }

                    DataTable dt = _cm.GetAllPendingPOMasterDetails(StoreType, Unit);
                    if (dt.Rows.Count > 0)
                    {
                        gvPO.DataSource = dt;
                        gvPO.DataBind();
                        ViewState["dt"] = dt;
                        ViewState["sort"] = "Asc";
                    }
                    else
                    {
                        gvPO.DataSource = null;
                        gvPO.DataBind();
                    }
                }

            //}
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void gvPO_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)ViewState["dt"];
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sort"]) == "Asc")
                {
                    dt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sort"] = "Desc";
                }
                else
                {
                    dt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sort"] = "Asc";
                }
                gvPO.DataSource = dt;
                gvPO.DataBind();
            }
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
                Label lblQuantity = (e.Row.FindControl("lblQuantity") as Label);
                Label lblPORecvdQty = (e.Row.FindControl("lblPORecvdQty") as Label);
                Button btnReminder = (e.Row.FindControl("btnReminder") as Button);
                string PurchaseOrderID = lblPurchaseOrderID.Text;
                if (Convert.ToDecimal(lblPORecvdQty.Text) == 0)
                {
                    lblPOStatus.Text = "Pending";
                    if (lblDeliveryDate.Text != "")
                    {
                        lblDaysLeft.Text = ((int)Math.Round((Convert.ToDateTime(lblDeliveryDate.Text) - DateTime.Now).TotalDays)).ToString();
                        if (DateTime.Now > Convert.ToDateTime(lblDeliveryDate.Text))
                        {
                            e.Row.BackColor = System.Drawing.Color.LightPink;
                        }
                    }
                }
                if (lblDeliveryDate.Text != "")
                {
                    if (lblPORecvdQty.Text != "0.00")
                    {
                        if ((Convert.ToDecimal(lblQuantity.Text) - Convert.ToDecimal(lblPORecvdQty.Text) > 0))
                        {
                            lblPOStatus.Text = "Partially Delivered";
                            lblDaysLeft.Text = ((int)Math.Round((Convert.ToDateTime(lblDeliveryDate.Text) - DateTime.Now).TotalDays)).ToString();
                            if (DateTime.Now > Convert.ToDateTime(lblDeliveryDate.Text))
                            {
                                e.Row.BackColor = System.Drawing.Color.LightPink;
                            }
                        }
                    }
                }


                if ((Convert.ToDecimal(lblQuantity.Text) - Convert.ToDecimal(lblPORecvdQty.Text) == 0))
                {
                    lblPOStatus.Text = "Fully Delivered";
                }
                if (lblDaysLeft.Text != "")
                {
                    if (Convert.ToInt32(lblDaysLeft.Text) < 0)
                    {
                        if (Session["DeptName"] != null)
                        {
                            if (Session["DeptName"].ToString() == "Purchase")
                            {
                                btnReminder.Visible = true;
                            }
                            else
                            {
                                btnReminder.Visible = false;
                            }
                        }
                    }
                }

            }
        }

        protected void gvPO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reminder")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                Label lblPurchaseOrderID = (Label)gvPO.Rows[rowIndex].FindControl("lblPurchaseOrderID");
                Label lblPONumber = (Label)gvPO.Rows[rowIndex].FindControl("lblPONumber");
                Label lblVendorID = (Label)gvPO.Rows[rowIndex].FindControl("lblVendorID");
                Label lblVendorName = (Label)gvPO.Rows[rowIndex].FindControl("lblVendorName");

                int VendorID = Convert.ToInt32(lblVendorID.Text);
                int PurchaseOrderID = Convert.ToInt32(lblPurchaseOrderID.Text);

                _cm.PurchaseOrderID = PurchaseOrderID;
                _cm.VendorID = VendorID;

                SendMail(VendorID.ToString(), PurchaseOrderID.ToString());
                if (Session["Failed"] != null)
                {
                    if (Session["Failed"].ToString() == "0")
                        _cm.UpdateVendorQuotationStatus(_cm);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed!');", true);
                }
            }
        }
        private void SendMail(string VendorID, string PurchaseOrderID)
        {
            string PONumber = ""; DateTime PODate = new DateTime();
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(sb.ToString());

            StringBuilder sbEnq = new StringBuilder();
            StringReader srEnq = new StringReader(sbEnq.ToString());

            _cm.PurchaseOrderID = Convert.ToInt32(PurchaseOrderID);
            DataTable dtPO = _cm.GetPOReportFileName(PurchaseOrderID);
            if (dtPO.Rows.Count > 0)
            {
                PONumber = dtPO.Rows[0]["PONumber"].ToString();
                PODate = Convert.ToDateTime(dtPO.Rows[0]["PODate"].ToString());
            }
            DataTable dtVendor = _cm.GetVendorReference(VendorID);
            DataTable dtVendorEmail = _cm.GetVendorEmail(VendorID);
            if (dtVendor.Rows.Count > 0 && dtVendorEmail.Rows.Count > 0)
            {
                _cm.VendorName = dtVendor.Rows[0]["VendorName"].ToString();
                _cm.Reference = dtVendor.Rows[0]["Reference"].ToString();

                string Subject = "";
                string FromAddress = "";
                string ToAddress = "";
                string Email = "";
                if (dtVendorEmail.Rows.Count > 1)
                {
                    foreach (DataRow dr in dtVendorEmail.Rows)
                    {
                        Email += dr["ContactEmail"].ToString() + ",";
                    }
                    Email = Email.Remove(Email.Length - 1, 1);
                }
                else
                {
                    Email = dtVendorEmail.Rows[0]["ContactEmail"].ToString();
                }

                ToAddress = Email;

                string CCAddress = "";
                string BCCAddress = "";
                string Body = "";
                string Note = "";

                DataTable dtEmailDetails = _cm.getEmaildetails("Float Enquiry");
                if (dtEmailDetails.Rows.Count > 0)
                {
                    if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                    {
                        Subject = "PO Pending Reminder " + dtEmailDetails.Rows[0]["EmailSubject"].ToString();
                    }
                    if (dtEmailDetails.Rows[0]["FromAddress"].ToString() != "")
                    {
                        FromAddress = dtEmailDetails.Rows[0]["FromAddress"].ToString();
                    }
                    if (dtEmailDetails.Rows[0]["CCAddress"].ToString() != "")
                    {
                        CCAddress = dtEmailDetails.Rows[0]["CCAddress"].ToString();
                    }
                    if (dtEmailDetails.Rows[0]["BCCAddress"].ToString() != "")
                    {
                        BCCAddress = dtEmailDetails.Rows[0]["BCCAddress"].ToString();

                    }
                    //sb.AppendFormat(string.Format("<a href='https://ProcureSoft.rnisolutions.in:8636/ViewPO.aspx?P={0}&R={1}' target='_blank'>", _cm.PurchaseOrderID, _cm.Reference));
                    sb.AppendFormat(string.Format("<a href='https://ProcureSoft.rnisolutions.in:8636/vendorPOPendingList.aspx?ID={0}' target='_blank'>", _cm.Reference));
                    sb.AppendFormat("PendingPO");
                    sb.Append("</a>");
                    string body = objEmail.PrepareMailBodyWith("POReminder.html", "{VendorName}", _cm.VendorName, "{PONumber}", PONumber, "{PODate}", PODate.ToString("dd/MMM/yyyy"), "{Link}", sb.ToString());
                    string str = body.ToString();

                    _cm.FromAddress = FromAddress;
                    _cm.ToAddress = ToAddress;
                    _cm.CCAddress = CCAddress;
                    _cm.BCCAddress = BCCAddress;
                    _cm.Subject = Subject;
                    _cm.Purpose = "Pending PO Reminder";
                    _cm.FileName = "POReminder.html";
                    _cm.SentTo = _cm.VendorName;
                    _cm.Body = "Pending PO Reminder Sent to " + _cm.VendorName + ", Link:" + sb.ToString();


                    try
                    {
                        if (sbEnq.Length != 0)
                        {
                            objEmail.SendMail(FromAddress, ToAddress, CCAddress, BCCAddress, sbEnq.ToString(), Subject, body);
                        }
                        else
                        {
                            objEmail.SendMail(FromAddress, ToAddress, CCAddress, BCCAddress, Subject, body);
                        }
                        if (Session["Failed"].ToString() == "0")
                        {
                            _cm.SentStatus1 = true;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                        }
                        else
                        {
                            _cm.SentStatus1 = false;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        }
                        _cm.AddMailBoxDetails(_cm);

                    }
                    catch (Exception ex)
                    {
                        _cm.SentStatus1 = false;
                        _cm.AddMailBoxDetails(_cm);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        string ErrorMessage = ex.Message;
                        string Source = ex.Source;
                        string StackTrace = ex.StackTrace;

                        Session["ErrorMessage"] = ErrorMessage + "," + Source + "," + StackTrace;
                        Response.Redirect("Error.aspx");

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed. Try again Later!');", true);

                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Vendor Email Found! Kindly update Vendor Details!');", true);
                }
            }
        }
    }
}