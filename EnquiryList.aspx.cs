using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class EnquiryList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["Usertype"] != null)
                {
                    //BindUserDetails(Session["UserID"].ToString());
                    GetEnquiryList();
                    Session["EmpID"] = Session["EmployeeID"].ToString();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        protected void BindUserDetails(String UserID)
        {
            DataTable dt = _cm.GetEmployeeDetails(UserID);
            if (dt.Rows.Count > 0)
            {
                Session["EmployeeName"] = dt.Rows[0]["EmployeeName"].ToString();
                Session["EmpID"] = dt.Rows[0]["EmployeeID"].ToString();
                Session["DepartmentID"] = dt.Rows[0]["DepartmentID"].ToString();
            }
        }
        private void GetEnquiryList()
        {
            DataTable dt = new DataTable();
            if (Session["DeptName"] != null)
            {
                string s = Session["DeptName"].ToString();
                switch (Session["DeptName"].ToString())
                {
                    case "Purchase":
                        dt = _cm.GetEnquiryList();
                        break;
                }
            }

            if (dt.Rows.Count > 0)
            {
                gvEnquiry.DataSource = dt;
                gvEnquiry.DataBind();
            }
            else
            {
                gvEnquiry.DataSource = null;
                gvEnquiry.DataBind();
            }

            gvEnquiry.Visible = true;
            gvOtherEnquiry.Visible = false;

            // Reset all buttons first
            ResetAllButtons();

            // Set New button as active
            btnNew.CssClass = "btn btn-primary active";
        }

        private void ResetAllButtons()
        {
            btnNew.CssClass = "btn btn-primary";
            btnQuoteReceived.CssClass = "btn btn-primary";
            btnAccepted.CssClass = "btn btn-primary";
            btnPOGenerated.CssClass = "btn btn-primary";
            btnOthers.CssClass = "btn btn-primary";
        }
        protected void gvEnquiry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find controls
                HyperLink hypEnquiryID = (HyperLink)e.Row.FindControl("hypEnquiryID");
                Label lblVendors = (Label)e.Row.FindControl("lblVendors");
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                HyperLink hypVendor = (HyperLink)e.Row.FindControl("hypVendor");
                Button btnRelease = (e.Row.FindControl("btnRelease") as Button);

                string VendorID = lblVendors.Text;
                DataTable dtVendor = _cm.GetVendorReference(VendorID);
                if (dtVendor.Rows.Count > 0)
                {
                    string reference = dtVendor.Rows[0]["Reference"].ToString();
                    hypVendor.NavigateUrl = "EnquiryForm.aspx?Reference=" + reference + "&Enquiry=" + hypEnquiryID.Text;
                }
                switch (lblStatus.Text)
                {
                    case "New":
                        btnRelease.Visible = true;
                        break;
                }
                }
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            ResetAllButtons();
            btnNew.CssClass = "btn btn-primary active";
            GetEnquiryList(); // This will show new enquiries
        }

        protected void btnQuoteReceived_Click(object sender, EventArgs e)
        {
            ResetAllButtons();
            btnQuoteReceived.CssClass = "btn btn-primary active";
            // Call your method to get quote received enquiries
            GetEnquiryList("2,7", "QuoteSent");
        }
        protected void btnAccepted_Click(object sender, EventArgs e)
        {
            string EnquiryType = "Accepted";
            GetEnquiryList("3", EnquiryType);
        }
        protected void btnPOGenerated_Click(object sender, EventArgs e)
        {
            string EnquiryType = "POGenerated";
            GetEnquiryList("9", EnquiryType);
        }
        protected void btnOthers_Click(object sender, EventArgs e)
        {
            string EnquiryType = "Others";
            GetEnquiryList("4,5,6,7", EnquiryType);
        }

        protected void gvOtherEnquiry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Label lblSent = (e.Row.FindControl("lblSent") as Label);
                Label lblStatus = (e.Row.FindControl("lblStatus") as Label);
                Label lblVendorID = (e.Row.FindControl("lblVendorID") as Label);
                Label lblPrice = (e.Row.FindControl("lblPrice") as Label);
                HyperLink hypEnquiryID = (e.Row.FindControl("hypEnquiryID") as HyperLink);
                HiddenField hdEnquiryDetailsID = (e.Row.FindControl("hdEnquiryDetailsID") as HiddenField);
                HyperLink hypQuote = (e.Row.FindControl("hypQuote") as HyperLink);
                HyperLink hypVendor = (e.Row.FindControl("hypVendor") as HyperLink);
                HyperLink hyRegerenate = (e.Row.FindControl("hyRegerenate") as HyperLink);
                Button btnResend = (e.Row.FindControl("btnResend") as Button);
                Button btnRelease = (e.Row.FindControl("btnRelease") as Button);

                decimal BalanceQty = 0;
                DataTable dtBalanceQty = new DataTable();
                if (hdEnquiryDetailsID != null)
                {
                    dtBalanceQty = _cm.GetPendingQtyForPO(hdEnquiryDetailsID.Value);
                    if (dtBalanceQty.Rows.Count > 0)
                    {
                        BalanceQty = Convert.ToDecimal(dtBalanceQty.Rows[0]["BalanceQty"].ToString());
                        if (BalanceQty > 0)
                        {
                            hypEnquiryID.NavigateUrl = "QuoteAnalysis.aspx?ID=" + hypEnquiryID.Text;
                        }
                        else
                        {
                            hypEnquiryID.NavigateUrl = "ViewEnquiry.aspx?ID=" + hypEnquiryID.Text;
                        }
                    }
                }
                
                if (hypVendor.Text == "0" || hypVendor.Text == "")
                {                    
                    hypEnquiryID.Visible = true;
                }
                else
                {
                    hypEnquiryID.Visible = true;
                }
                switch (lblStatus.Text)
                {
                    case "New":
                        btnRelease.Visible = true;
                        break;
                    case "Quotation Received"://2
                    case "Partial PO Generated"://1017
                        if (hypVendor.Text == "0" || hypVendor.Text == "")
                        {
                            hypEnquiryID.NavigateUrl = "ViewEnquiry.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.Visible = true;
                        }
                        else
                        {
                            hypEnquiryID.NavigateUrl = "QuoteAnalysis.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.Visible = true;
                            if (lblPrice.Text != "")
                            {
                                hyRegerenate.Visible = true;
                                btnResend.Visible = true;
                               // btnRelease.Visible = true;
                            }
                        }
                        break;
                    case "Accepted"://3
                        if (hypVendor.Text == "0" || hypVendor.Text == "")
                        {
                            hypEnquiryID.NavigateUrl = "ViewEnquiry.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.Visible = true;
                        }
                        else
                        {
                            hypEnquiryID.NavigateUrl = "QuoteAnalysis.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.Visible = true;
                        }
                        break;
                    case "Revised Quote"://7
                        if (hypVendor.Text == "0" || hypVendor.Text == "")
                        {
                            hypEnquiryID.NavigateUrl = "ViewEnquiry.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.Visible = true;
                        }
                        else
                        {
                            hypEnquiryID.NavigateUrl = "QuoteAnalysis.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.Visible = true;
                            if (lblPrice.Text != "")
                            {
                                hyRegerenate.Visible = true;
                                btnResend.Visible = true;
                                //btnRelease.Visible = true;
                            }
                        }
                        break;

                    case "PO Generated"://9
                        DataTable dtPO = _cm.GetPOStatus(hypEnquiryID.Text);
                        if (dtPO.Rows.Count > 0)
                        {
                            string PurchaseOrderID = dtPO.Rows[0]["PurchaseOrderID"].ToString();
                            hypEnquiryID.NavigateUrl = "ViewEnquiry.aspx?ID=" + hypEnquiryID.Text;
                            hypVendor.NavigateUrl = "QuoteAnalysis.aspx?ID=" + hypEnquiryID.Text;
                            hypQuote.NavigateUrl = "ViewPO.aspx?PurchaseOrderID=" + PurchaseOrderID;
                            hyRegerenate.NavigateUrl = "GeneratePO.aspx?POID=" + PurchaseOrderID;
                        }
                        break;
                    default://4,5,6
                        hypEnquiryID.NavigateUrl = "ViewEnquiry.aspx?ID=" + hypEnquiryID.Text;
                        break;
                }

            }
        }
        private void GetEnquiryList(string statusIDs = null, string enquiryType = null)
        {
            DataTable dt = new DataTable();
            if (Session["DeptName"] != null)
            {
                switch (Session["DeptName"].ToString())
                {
                    case "Purchase":
                        dt = _cm.GetEnquiryList(statusIDs, enquiryType);
                        break;
                }
            }

            if (dt.Rows.Count > 0)
            {
                gvOtherEnquiry.DataSource = dt;
                gvOtherEnquiry.DataBind();
            }
            else
            {
                gvEnquiry.DataSource = null;
                gvEnquiry.DataBind();
            }

            gvEnquiry.Visible = false;
            gvOtherEnquiry.Visible = true;

            // Set active button - default to "New" if no specific type provided
            SetActiveButton(enquiryType ?? "New");
        }
        private void SetActiveButton(string buttonType)
        {
            // Reset all buttons first
            btnNew.CssClass = "btn btn-primary";
            btnQuoteReceived.CssClass = "btn btn-primary";
            btnAccepted.CssClass = "btn btn-primary";
            btnPartialPOGenerated.CssClass = "btn btn-primary";
            btnPOGenerated.CssClass = "btn btn-primary";
            btnOthers.CssClass = "btn btn-primary";

            // Set the active button
            switch (buttonType)
            {
                case "QuoteSent":
                    btnQuoteReceived.CssClass = "btn btn-primary active";
                    break;
                case "Accepted":
                    btnAccepted.CssClass = "btn btn-primary active";
                    break;
                case "PartialPOGenerated":
                    btnPartialPOGenerated.CssClass = "btn btn-primary active";
                    break;
                case "POGenerated":
                    btnPOGenerated.CssClass = "btn btn-primary active";
                    break;
                case "Others":
                    btnOthers.CssClass = "btn btn-primary active";
                    break;
                default: // "New" or any other case
                    btnNew.CssClass = "btn btn-primary active";
                    break;
            }
        }

        protected void gvOtherEnquiry_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Send")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                Label lblEnquiryID = (Label)gvOtherEnquiry.Rows[rowIndex].FindControl("lblEnquiryID");
                Label lblVendorID = (Label)gvOtherEnquiry.Rows[rowIndex].FindControl("lblVendorID");
                Label lblVendorQuotationID = (Label)gvOtherEnquiry.Rows[rowIndex].FindControl("lblVendorQuotationID");
                int VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);
                int VendorID = Convert.ToInt32(lblVendorID.Text);
                int EnquiryID = Convert.ToInt32(lblEnquiryID.Text);

                _cm.QuotationStatusID = 6;
                _cm.VendorQuotationID = VendorQuotationID;
                _cm.VendorID = VendorID;

                SendMail(VendorID.ToString(), VendorQuotationID.ToString(), EnquiryID.ToString());
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
            if (e.CommandName == "Release")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                Label lblEnquiryID = (Label)gvOtherEnquiry.Rows[rowIndex].FindControl("lblEnquiryID");
                Label lblItemCode = (Label)gvOtherEnquiry.Rows[rowIndex].FindControl("lblItemCode");
                HiddenField hdEnquiryDetailsID = (HiddenField)gvOtherEnquiry.Rows[rowIndex].FindControl("hdEnquiryDetailsID");

                _cm.EnquiryID = Convert.ToInt32(lblEnquiryID.Text);
                _cm.EnquiryDetailsID = Convert.ToInt32(hdEnquiryDetailsID.Value);
                _cm.StatusID = 2;//Approve by Purchase
                _cm.ItemCode = lblItemCode.Text;
                try
                {
                    _cm.ReleaseEnquiryItem(_cm);
                    _cm.EnquiryStatusID = 0;
                    if (Session["EmployeeName"] != null)
                    {
                        _cm.Remarks = "Enquiry Released by " + Session["EmployeeName"].ToString() + " on " + DateTime.Now.AddHours(12.5);
                    }
                    else
                    {
                        _cm.Remarks = "Enquiry Released";
                    }
                   // _cm.UpdateEnquiryRelease(_cm);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Item Released to Float!');", true);

                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Release Failed!');", true);
                }

            }
        }
        private void SendMail(string VendorID, string VendorQuotationID, string EnquiryID)
        {
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(sb.ToString());

            StringBuilder sbEnq = new StringBuilder();
            StringReader srEnq = new StringReader(sbEnq.ToString());

            _cm.VendorID = Convert.ToInt32(VendorID);
            DataTable dtQuotation = _cm.GetQuotationRevision(VendorQuotationID, VendorID);
            if (dtQuotation.Rows.Count > 0)
            {
                _cm.Revision = Convert.ToInt32(dtQuotation.Rows[0]["Revision"].ToString());
            }

            _cm.EnquiryID = Convert.ToInt32(EnquiryID);
            DataTable dtVendor = _cm.GetVendorReference(VendorID);
            DataTable dtVendorEmail = _cm.GetVendorEmail(VendorID);
            if (dtVendor.Rows.Count > 0 && dtVendorEmail.Rows.Count > 0)
            {
                _cm.VendorName = dtVendor.Rows[0]["VendorName"].ToString();
                _cm.Reference = dtVendor.Rows[0]["Reference"].ToString();

                string Subject = "";
                string FromAddress = "";
                string ToAddress = dtVendorEmail.Rows[0]["ContactEmail"].ToString();
                string CCAddress = "";
                string BCCAddress = "";
                string Body = "";
                string Note = "";

                string EnqID = EnquiryID;
                DataTable dte = _cm.GetEnquiryNote(EnqID);
                if (dte.Rows.Count > 0)
                {
                    Note = dte.Rows[0]["Remarks"].ToString();
                }

                DataTable dtEmailDetails = _cm.getEmaildetails("Float Enquiry");
                if (dtEmailDetails.Rows.Count > 0)
                {
                    if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                    {
                        Subject = "Enquiry#" + _cm.EnquiryID + dtEmailDetails.Rows[0]["EmailSubject"].ToString();
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
                    string baseUrl = "http://localhost:41414/"; //  default
                    DataTable dtOrg = _cm.GetOrganizationDetails();
                    if (dtOrg.Rows.Count > 0 && dtOrg.Columns.Contains("url"))
                    {
                        baseUrl = dtOrg.Rows[0]["url"].ToString();
                        if (!baseUrl.EndsWith("/"))
                            baseUrl += "/";
                    }
                    sb.AppendFormat(string.Format("<a href='"+ baseUrl + "EnquiryForm.aspx?VendorQuotationID={0}&Ven={1}' target='_blank'>", _cm.VendorQuotationID, _cm.VendorID));
                    sb.AppendFormat("Quotation");
                    sb.Append("</a>");


                    string body = objEmail.PrepareMailBodyWith("Enquiry.html", "{VendorName}", _cm.VendorName, "{Link}", sb.ToString(), "{Note}", Note);
                    string str = body.ToString();

                    string filename = Server.MapPath("//Enquiry/Enquiry-" + _cm.EnquiryID + _cm.Reference + "-" + _cm.Revision + ".htm");
                    System.IO.File.WriteAllText(@filename, str);

                    DataTable dtEnq = _cm.GetEnquiryDocuments(EnquiryID);
                    if (dtEnq.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEnq.Rows)
                        {
                            string html = HttpContext.Current.Server.MapPath("//EnquiryDocuments/" + dr["FileName"].ToString());
                            sbEnq.Append(html + ";");
                        }
                    }
                    _cm.FromAddress = FromAddress;
                    _cm.ToAddress = ToAddress;
                    _cm.CCAddress = CCAddress;
                    _cm.BCCAddress = BCCAddress;
                    _cm.Subject = Subject;
                    _cm.Purpose = "Enquiry Resend";
                    _cm.FileName = "EnquiryRevisition.html";
                    _cm.SentTo = _cm.VendorName;
                    _cm.Body = body;


                    try
                    {
                        if (sbEnq.Length > 0)
                        {
                            objEmail.SendMail(FromAddress, ToAddress, Subject, body, sbEnq.ToString().TrimEnd(';'));
                        }
                        else
                        {
                            objEmail.SendMail(FromAddress, ToAddress, Subject, body);
                        }

                        if (Session["Failed"].ToString() == "0")
                        {
                            _cm.SentStatus1 = true;
                            _cm.AddMailBoxDetails(_cm);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                        }
                        else
                        {
                            _cm.SentStatus1 = false;
                            _cm.AddMailBoxDetails(_cm);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed. Try again Later!');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Vendor Email Found! Kindly update Vendor Details!');", true);
                }
            }
        }

        private void SendMail(string VendorID, string EnquiryID)
        {
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(sb.ToString());

            StringBuilder sbEnq = new StringBuilder();
            StringReader srEnq = new StringReader(sbEnq.ToString());

            _cm.EnquiryID = Convert.ToInt32(EnquiryID);
            DataTable dtVendor = _cm.GetVendorReference(VendorID);
            DataTable dtVendorEmail = _cm.GetVendorEmail(VendorID);
            if (dtVendor.Rows.Count > 0 && dtVendorEmail.Rows.Count > 0)
            {
                _cm.VendorName = dtVendor.Rows[0]["VendorName"].ToString();
                _cm.Reference = dtVendor.Rows[0]["Reference"].ToString();

                string Subject = "";
                string FromAddress = "";
                string ToAddress = dtVendorEmail.Rows[0]["ContactEmail"].ToString();
                string CCAddress = "";
                string BCCAddress = "";
                string Body = "";
                string Note = "";

                string EnqID = EnquiryID;
                DataTable dte = _cm.GetEnquiryNote(EnqID);
                if (dte.Rows.Count > 0)
                {
                    Note = dte.Rows[0]["Remarks"].ToString();
                }

                DataTable dtEmailDetails = _cm.getEmaildetails("Float Enquiry");
                if (dtEmailDetails.Rows.Count > 0)
                {
                    if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                    {
                        Subject = "Reminder to Quote - Enquiry#" + _cm.EnquiryID + dtEmailDetails.Rows[0]["EmailSubject"].ToString();
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
                    string baseUrl = "http://localhost:41414/"; //  default
                    DataTable dtOrg = _cm.GetOrganizationDetails();
                    if (dtOrg.Rows.Count > 0 && dtOrg.Columns.Contains("url"))
                    {
                        baseUrl = dtOrg.Rows[0]["url"].ToString();
                        if (!baseUrl.EndsWith("/"))
                            baseUrl += "/";
                    }
                    sb.AppendFormat(string.Format("<a href='"+baseUrl+"EnquiryForm.aspx?Reference={0}&Enquiry={1}' target='_blank'>", _cm.Reference, _cm.EnquiryID));
                    sb.AppendFormat("Quotation");
                    sb.Append("</a>");
                    string body = objEmail.PrepareMailBodyWith("Enquiry.html", "{VendorName}", _cm.VendorName, "{Link}", sb.ToString(), "{Note}", Note);
                    string str = body.ToString();

                    string filename = Server.MapPath("//Enquiry/Enquiry-" + _cm.EnquiryID + _cm.Reference + ".htm");
                    System.IO.File.WriteAllText(@filename, str);

                    DataTable dtEnq = _cm.GetEnquiryDocuments(EnquiryID);
                    if (dtEnq.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEnq.Rows)
                        {
                            string html = HttpContext.Current.Server.MapPath("//EnquiryDocuments/" + dr["FileName"].ToString());
                            sbEnq.Append(html + ";");
                        }
                    }
                    _cm.FromAddress = FromAddress;
                    _cm.ToAddress = ToAddress;
                    _cm.CCAddress = CCAddress;
                    _cm.BCCAddress = BCCAddress;
                    _cm.Subject = Subject;
                    _cm.Purpose = "Float Enquiry";
                    _cm.FileName = "Enquiry.html";
                    _cm.SentTo = _cm.VendorName;
                    _cm.Body = "Enquiry Reminder Sent to " + _cm.VendorName + ", Link:" + sb.ToString() + ", Note" + Note;


                    try
                    {
                        if (sbEnq.Length > 0)
                        {
                            objEmail.SendMail(FromAddress, ToAddress, Subject, body, sbEnq.ToString().TrimEnd(';'));
                        }
                        else
                        {
                            objEmail.SendMail(FromAddress, ToAddress, Subject, body);
                        }

                        if (Session["Failed"].ToString() == "0")
                        {
                            _cm.SentStatus1 = true;
                            _cm.AddMailBoxDetails(_cm);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                        }
                        else
                        {
                            _cm.SentStatus1 = false;
                            _cm.AddMailBoxDetails(_cm);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed. Try again Later!');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Vendor Email Found! Kindly update Vendor Details!');", true);
                }
            }
        }

        protected void gvEnquiry_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reminder")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                HyperLink hypEnquiryID = (HyperLink)gvEnquiry.Rows[rowIndex].FindControl("hypEnquiryID");
                Label lblVendors = (Label)gvEnquiry.Rows[rowIndex].FindControl("lblVendors");
                HyperLink hypVendor = (HyperLink)gvEnquiry.Rows[rowIndex].FindControl("hypVendor");
               
                DataTable dt = _cm.GetPendingToQuoteVendors(hypEnquiryID.Text);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SendMail(dr["VendorID"].ToString(), hypEnquiryID.Text);
                        if (Session["Failed"] != null)
                        {
                            if (Session["Failed"].ToString() == "0")
                                _cm.UpdateVendorQuotationStatus(_cm);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed For " + dr["VendorName"].ToString() + "');", true);
                        }
                    }
                }
            }
            if (e.CommandName == "Release")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                HyperLink hypEnquiryID = (HyperLink)gvEnquiry.Rows[rowIndex].FindControl("hypEnquiryID");
                Label lblItemCode = (Label)gvEnquiry.Rows[rowIndex].FindControl("lblItemCode");
                _cm.EnquiryID = Convert.ToInt32(hypEnquiryID.Text);
                _cm.StatusID = 2;//Approved by Purchase
                _cm.ItemCode = lblItemCode.Text;
                try
                {
                    _cm.ReleaseEnquiryItem(_cm);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Item Released to Float!');", true);

                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Release Failed!');", true);
                }

            }
        }

        protected void btnPartialPOGenerated_Click(object sender, EventArgs e)
        {
            string EnquiryType = "PartialPOGenerated";
            GetEnquiryList("1017", EnquiryType);
        }
    }
}
