using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;
using System.Globalization;
using static iTextSharp.awt.geom.Point2D;
using System.Security.Cryptography;

namespace LIMS
{
    public partial class EnquiryForm : System.Web.UI.Page
    {
        private decimal _totalDiscountAmount = 0;

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        public static List<string> ID = new List<string>();
        public static string reference = ""; int quoted = 0;


        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Populate();
                txtValidityDate.Attributes.Add("autocomplete", "off");

                if ((Request.QueryString["VendorQuotationID"] != null) && (Request.QueryString["Ven"] != null))
                {
                    string VendorQuotationID = Request.QueryString["VendorQuotationID"].ToString();
                    string VendorID = Request.QueryString["Ven"].ToString();
                    BindVAT(ddlVAT, VendorID);
                    txtQuotationNumber.ReadOnly = true;
                    Session["VendorQuotationID"] = VendorQuotationID;
                    Session["VendorID"] = VendorID;

                    GetQuotationDetails(VendorQuotationID, VendorID);
                    GetQuotation(VendorQuotationID, VendorID);
                    getVendorQuotationHeader(VendorQuotationID, VendorID);

                    DataTable dt = _cm.GetQuotationRevision(VendorQuotationID, VendorID);
                    if (dt.Rows.Count > 0 && dt.Rows[0]["Revision"].ToString() != "")
                    {
                        _cm.Revision = Convert.ToInt32(dt.Rows[0]["Revision"]) + 1;
                        Session["Revision"] = _cm.Revision;
                    }

                    DataTable dtEnquiryStatus = _cm.CheckEnquiryStatus(VendorQuotationID, VendorID);
                    if (dtEnquiryStatus.Rows.Count > 0)
                    {
                        CheckEnquiryClosed(dtEnquiryStatus.Rows[0]["EnquiryID"].ToString());
                    }

                    UpdateTotals();
                }

                if (Request.QueryString["Reference"] != null && Request.QueryString["Enquiry"] != null)
                {
                    string Reference = Request.QueryString["Reference"].ToString();
                    string EnquiryID = Request.QueryString["Enquiry"].ToString();

                    Session["Reference"] = Reference;
                    Session["EnquiryID"] = EnquiryID;

                    divVendor.Visible = false;
                    divSubmited.Visible = false;
                    divSuccess.Visible = false;
                    divClosed.Visible = false;

                    getVendorDetails(Reference);
                    getEnquiryDetails(EnquiryID, Reference);
                    getVendorQuotationDetails(EnquiryID, Reference);
                    getContactPerson(Reference);
                    CheckEnquiryClosed(EnquiryID);

                    UpdateTotals();
                }

                if (Request.QueryString["Enquiry"] == null)
                {
                    chkNA.Enabled = false;
                    chkNA.ToolTip = "Disabled because Enquiry ID is missing from the request.";
                }
            }
            else  // POSTBACK HANDLING
            {
                // Re-bind dropdowns first (they lose their datasource on postback)
                // BindQuoteCurrency();
                //BindVAT(ddlVAT);

                // Preserve user-selected values BEFORE restoring from database
                string selectedCurrency = ddlQuoteCurrency.SelectedValue;
                string selectedContact = ddlContactPerson.SelectedValue;
                string selectedVAT = ddlVAT.SelectedValue;

                // CASE 1: Editing existing quotation (VendorQuotationID + Ven in URL)
                if ((Request.QueryString["VendorQuotationID"] != null) && (Request.QueryString["Ven"] != null))
                {
                    string VendorQuotationID = Request.QueryString["VendorQuotationID"].ToString();
                    string VendorID = Request.QueryString["Ven"].ToString();

                    // Restore quotation header from database
                    getVendorQuotationHeader(VendorQuotationID, VendorID);

                    // CRITICAL: Restore user selections that were made before postback
                    // This ensures dropdown changes are preserved
                    if (!string.IsNullOrEmpty(selectedCurrency))
                    {
                        try { ddlQuoteCurrency.SelectedValue = selectedCurrency; } catch { }
                    }
                    if (!string.IsNullOrEmpty(selectedContact))
                    {
                        try { ddlContactPerson.SelectedValue = selectedContact; } catch { }
                    }
                    if (!string.IsNullOrEmpty(selectedVAT))
                    {
                        try { ddlVAT.SelectedValue = selectedVAT; } catch { }
                    }

                    // Restore vendor details from session (if available)
                    if (Session["Reference"] != null)
                    {
                        getVendorDetails(Session["Reference"].ToString());
                    }
                }
                // CASE 2: New quotation from enquiry (Reference + Enquiry in URL)
                else if (Request.QueryString["Reference"] != null && Request.QueryString["Enquiry"] != null)
                {
                    string Reference = Request.QueryString["Reference"].ToString();

                    // Restore vendor details
                    getVendorDetails(Reference);
                    getContactPerson(Reference);

                    // Restore user selections
                    if (!string.IsNullOrEmpty(selectedCurrency))
                    {
                        try { ddlQuoteCurrency.SelectedValue = selectedCurrency; } catch { }
                    }
                    if (!string.IsNullOrEmpty(selectedContact))
                    {
                        try { ddlContactPerson.SelectedValue = selectedContact; } catch { }
                    }
                    if (!string.IsNullOrEmpty(selectedVAT))
                    {
                        try { ddlVAT.SelectedValue = selectedVAT; } catch { }
                    }
                }

                // CASE 3: Restore vendor details from session if stored
                if (Session["VendorName"] != null)
                {
                    txtVendorName.Text = Session["VendorName"].ToString();
                    txtVendorCode.Text = Session["VendorCode"].ToString();
                    txtDealer.Text = Session["Dealer"].ToString();
                    txtTypeofEstablishment.Text = Session["TypeofEstablishment"].ToString();
                    txtTypeofBusiness.Text = Session["TypeofBusiness"].ToString();
                    txtYearofEstablishment.Text = Session["YearofEstablishment"].ToString();
                }
            }
        }
        private void Populate()
        {
            DataTable dt = _cm.getPurchasePaymentTerms();
            if (dt.Rows.Count > 0)
            {
                ddlPaymentTerms.DataSource = dt;
                ddlPaymentTerms.DataTextField = "PaymentTermName";
                ddlPaymentTerms.DataValueField = "PaymentTermID";
                ddlPaymentTerms.DataBind();
                ddlPaymentTerms.Items.Insert(0, "--Select Payment Terms--");

                dt = _cm.QuoteCurrency();
                ddlQuoteCurrency.DataSource = dt;
                ddlQuoteCurrency.DataTextField = "CurrencyName";
                ddlQuoteCurrency.DataValueField = "CurrencyID";
                ddlQuoteCurrency.DataBind();
                ddlQuoteCurrency.Items.Insert(0, new ListItem("Select QuoteCurrency", ""));
            }
        }
        private void getVendorQuotationHeader(string vendorQuotationID, string vendorID)
        {
            DataTable dt = _cm.GetQuotationDetails(vendorQuotationID, vendorID);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtQuotationNumber.Text = row["QuotationNumber"] != DBNull.Value ? row["QuotationNumber"].ToString() : "";
                ListItem item = ddlPaymentTerms.Items.FindByText(row["Payment"].ToString());

                if (item != null)
                {
                    ddlPaymentTerms.ClearSelection();
                    item.Selected = true;
                }
                else
                {
                    ddlPaymentTerms.SelectedIndex = 0; // default item
                }
                ddlContactPerson.SelectedValue = row["VendorContactPersonID"] != DBNull.Value ? row["VendorContactPersonID"].ToString() : "0";
                txtValidityDate.Text = row["ValidityDate"] != DBNull.Value ? Convert.ToDateTime(row["ValidityDate"]).ToString("yyyy-MM-dd") : "";
                ddlQuoteCurrency.SelectedValue = row["QuoteCurrencyId"] != DBNull.Value ? row["QuoteCurrencyId"].ToString() : "0";
                txtDelivery.Text = row["Delivery"] != DBNull.Value ? row["Delivery"].ToString() : "";
                txtRemarks.Text = row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : "";
                txtSpecialInstruction.Text = row["SpecialInstruction"] != DBNull.Value ? row["SpecialInstruction"].ToString() : "";
                //ddlVAT.SelectedValue = row["VATId"]?.ToString() ?? "0";
                int currencyId = row["QuoteCurrencyId"] != DBNull.Value ? Convert.ToInt32(row["QuoteCurrencyId"]) : 0;
                if (GetCurrencyCodeById(currencyId)?.ToUpper() == "IDR")
                {
                    ddlVAT.SelectedValue = row["VATId"]?.ToString() ?? "0";
                    ddlVAT.Enabled = false; // Keep consistent with your currency change logic
                }
                else
                {
                    ddlVAT.SelectedValue = "0"; // Not Applicable for non-IDR currencies
                    ddlVAT.Enabled = false;
                }
                if (row != null)
                {
                    string documentFile = "";
                    string certificateFile = "";

                    if (row.Table.Columns.Contains("Document") && row["Document"] != DBNull.Value)
                        documentFile = row["Document"].ToString().Trim();

                    if (row.Table.Columns.Contains("Certificate") && row["Certificate"] != DBNull.Value)
                        certificateFile = row["Certificate"].ToString().Trim();

                    // Treat "N/A" or "NA" as empty
                    if (string.Equals(documentFile, "N/A", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(documentFile, "NA", StringComparison.OrdinalIgnoreCase))
                        documentFile = "";

                    if (string.Equals(certificateFile, "N/A", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(certificateFile, "NA", StringComparison.OrdinalIgnoreCase))
                        certificateFile = "";

                    // Set label text safely (check for null)
                    if (lblDocumentName != null)
                        lblDocumentName.Text = !string.IsNullOrEmpty(documentFile) ? "Existing file: " + documentFile : "No document uploaded";

                    if (lblCertificateName != null)
                        lblCertificateName.Text = !string.IsNullOrEmpty(certificateFile) ? "Existing file: " + certificateFile : "No certificate uploaded";
                }
                else
                {
                    // Optional: clear labels or show no data
                    if (lblDocumentName != null)
                        lblDocumentName.Text = "No document uploaded";

                    if (lblCertificateName != null)
                        lblCertificateName.Text = "No certificate uploaded";
                }


                //lblSubTotal.Text = row["Subtotal"] != DBNull.Value ? Convert.ToDecimal(row["Subtotal"]).ToString("N2") : "0.00";
                //txtDiscount.Text = row["Discount"] != DBNull.Value ? row["Discount"].ToString() : "0";

                //lblDiscountAmount.Text = row["DiscountApplied"] != DBNull.Value ? Convert.ToDecimal(row["DiscountApplied"]).ToString("N2") : "0.00";
                //  lblGrandTotal.Text = row["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotal"]).ToString("N2") : "0.00";
                //SetDiscountUI(txtDiscount.Text);

            }
        }
        private void GetQuotationDetails(string VendorQuotationID, string VendorID)
        {
            DataTable dt = new DataTable();
            dt = _cm.GetVQStatusDetails(VendorQuotationID, 6, VendorID);
            if (dt.Rows.Count > 0)
            {
                divVendor.Visible = true;
                divSubmited.Visible = true;
                divSuccess.Visible = false;
            }
            else
            {
                divVendor.Visible = false;
                divSubmited.Visible = false;
                divSuccess.Visible = false;
            }
        }
        private void GetQuotation(string VendorQuotationID, string VendorID)
        {
            DataTable dt = _cm.GetQuotationDetails(VendorQuotationID, VendorID);
            if (dt.Rows.Count > 0)
            {
                gvEnquiry.DataSource = dt;
                gvEnquiry.DataBind();
                if (dt.Rows[0]["Reference"].ToString() != "")
                {
                    string Reference = dt.Rows[0]["Reference"].ToString();
                    Session["Reference"] = Reference;
                    getVendorDetails(Reference);
                    getContactPerson(Reference);
                    //BindVAT(ddlVAT);
                }
            }

        }
        private void BindVAT(DropDownList ddlVAT, string VendorID)
        {
            DataTable dt = _cm.getVAT();
            ddlVAT.DataSource = dt;
            ddlVAT.DataTextField = "VATCode";
            ddlVAT.DataValueField = "VATID";
            ddlVAT.DataBind();
            ddlVAT.Items.Insert(0, new ListItem("Select TAX", ""));
            ddlVAT.Items.Add(new ListItem("Not Applicable", "0"));
            DataTable dtVen = _cm.GetVendor(VendorID);
            if (dtVen.Rows.Count > 0)
            {
                try
                {
                    ddlVAT.ClearSelection();
                    ddlVAT.Items.FindByValue(dtVen.Rows[0]["VAT"].ToString()).Selected = true;
                    ddlVAT.Enabled = false;
                }
                catch (Exception ex) { }
            }
        }
        private void CheckEnquiryClosed(string EnquiryID)
        {
            DataTable dt = _cm.CheckEnquiry(EnquiryID);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["EnquiryStatus"].ToString() == "0")
                {
                    divClosed.Visible = true;
                    divVendor.Visible = false;
                    divSubmited.Visible = false;
                    divSuccess.Visible = false;
                }
                else
                {
                    if (quoted == 0)
                    {
                        divClosed.Visible = false;
                        divVendor.Visible = true;
                        divSubmited.Visible = false;
                        divSuccess.Visible = false;
                    }

                }
            }
        }
        private void getVendorDetails(string Reference)
        {
            DataTable dt = _cm.GetVendorForEnquiry(Reference);
            if (dt.Rows.Count > 0)
            {
                lblVendorID.Text = dt.Rows[0]["VendorID"].ToString();
                txtVendorName.Text = dt.Rows[0]["VendorName"].ToString();
                txtVendorCode.Text = dt.Rows[0]["VendorCode"].ToString();
                txtDealer.Text = dt.Rows[0]["DealerName"].ToString();
                txtTypeofEstablishment.Text = dt.Rows[0]["TypeofEstablishment"].ToString();
                txtTypeofBusiness.Text = dt.Rows[0]["TypeOfBusiness"].ToString();
                txtYearofEstablishment.Text = dt.Rows[0]["YearOfEstablishment"].ToString();

                // Store in Session for postback persistence
                Session["VendorName"] = txtVendorName.Text;
                Session["VendorCode"] = txtVendorCode.Text;
                Session["Dealer"] = txtDealer.Text;
                Session["TypeofEstablishment"] = txtTypeofEstablishment.Text;
                Session["TypeofBusiness"] = txtTypeofBusiness.Text;
                Session["YearofEstablishment"] = txtYearofEstablishment.Text;
                BindVAT(ddlVAT, lblVendorID.Text);
            }
        }
        private void getEnquiryDetails(string EnquiryID, string Reference)
        {
            DataTable dt = _cm.GetEnquiry(EnquiryID, Reference);
            if (dt.Rows.Count > 0)
            {
                //lblPenalty.Text = dt.Rows[0]["PenaltyDescription"].ToString();
                gvEnquiry.DataSource = dt;
                gvEnquiry.DataBind();

                //foreach (GridViewRow gr in gvEnquiry.Rows)
                //{
                //    DropDownList ddlVAT = (DropDownList)gr.FindControl("ddlVAT");
                //    //BindVAT(ddlVAT);
                //}

            }
        }
        private void getVendorQuotationDetails(string EnquiryID, string Reference)
        {
            DataTable dt = _cm.GetQuotationStatusDetails(EnquiryID, Reference);
            if (dt.Rows.Count > 0)
            {
                quoted = 1;
                divVendor.Visible = false;
                divSubmited.Visible = true;
                divSuccess.Visible = false;
            }
            else
            {
                divVendor.Visible = true;
                divSubmited.Visible = false;
                divSuccess.Visible = false;
            }
        }
        private void getContactPerson(string reference)
        {
            DataTable dt = _cm.GetVendorContactPerson(reference);
            if (dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "ContactPerson";
                ddlContactPerson.DataValueField = "VendorContactPersonID";
                ddlContactPerson.DataBind();
                ddlContactPerson.Items.Insert(0, new ListItem("Select Contact Person", ""));
            }
        }
        protected void chkNA_CheckedChanged(object sender, EventArgs e)
        {
            if (Request.QueryString["Enquiry"] == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid Access!');", true);
                return;
            }
            bool isValid = true;
            string EnquiryID = Request.QueryString["Enquiry"].ToString();
            _cm.GetEnquiryDetails(EnquiryID);

            if (chkNA.Checked)
            {

                if (string.IsNullOrEmpty(ddlContactPerson.SelectedValue) || ddlContactPerson.SelectedValue == "0")
                {
                    ddlContactPerson.BorderColor = System.Drawing.Color.Red;
                    isValid = false;
                }
                if (string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue) || ddlQuoteCurrency.SelectedValue == "0")
                {
                    ddlQuoteCurrency.BorderColor = System.Drawing.Color.Red;
                    isValid = false;
                }

                // Set default values in UI controls
                //txtQuotationNumber.Text = "N/A-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                txtQuotationNumber.Text = "N/A";
                ddlPaymentTerms.Items.FindByText("Advance").Selected = true;
                ddlQuoteCurrency.SelectedIndex = 0;
                ddlContactPerson.SelectedIndex = 0;
                txtValidityDate.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
                txtDelivery.Text = "N/A";
                txtSpecialInstruction.Text = "N/A";
                txtRemarks.Text = "N/A";


                txtQuotationNumber.ReadOnly = true;
                ddlPaymentTerms.Enabled = true;
                ddlQuoteCurrency.Enabled = false;
                ddlContactPerson.Enabled = false;
                txtValidityDate.ReadOnly = true;
                ddlVAT.Enabled = false;
                txtSpecialInstruction.ReadOnly = true;
                txtRemarks.ReadOnly = true;
                txtDelivery.ReadOnly = true;
                fpDocument.Enabled = false;
                fpCertificate.Enabled = false;

                for (int i = 0; i < gvEnquiry.Rows.Count; i++)
                {
                    TextBox txtRate = (TextBox)gvEnquiry.Rows[i].FindControl("txtRate");
                    TextBox txtDeliveryTime = (TextBox)gvEnquiry.Rows[i].FindControl("txtDeliveryTime");
                    TextBox txtDiscount = (TextBox)gvEnquiry.Rows[i].FindControl("txtDiscount");
                    TextBox txtRemarks1 = (TextBox)gvEnquiry.Rows[i].FindControl("txtRemarks1");

                    if (txtRate != null) { txtRate.ReadOnly = true; txtRate.Text = "0"; }
                    if (txtDeliveryTime != null) { txtDeliveryTime.ReadOnly = true; txtDeliveryTime.Text = "0"; }
                    if (txtDiscount != null) { txtDiscount.ReadOnly = true; txtDiscount.Text = "0%"; }
                    if (txtRemarks1 != null) { txtRemarks1.ReadOnly = true; txtRemarks1.Text = "N/A"; }
                }
            }
            else
            {
                // Re-enable and clear fields
                txtQuotationNumber.ReadOnly = false;
                ddlPaymentTerms.Enabled = false;
                ddlQuoteCurrency.Enabled = true;
                ddlContactPerson.Enabled = true;
                txtValidityDate.ReadOnly = false;
                ddlVAT.Enabled = true;
                txtSpecialInstruction.ReadOnly = false;
                txtRemarks.ReadOnly = false;
                txtDelivery.ReadOnly = false;
                fpDocument.Enabled = true;
                fpCertificate.Enabled = true;

                txtQuotationNumber.Text = "";
                ddlPaymentTerms.ClearSelection();
                txtPaymentTerms.Text = "";
                txtValidityDate.Text = "";
                txtSpecialInstruction.Text = "";
                txtRemarks.Text = "";
                txtDelivery.Text = "";

                ddlQuoteCurrency.SelectedIndex = 0;
                ddlContactPerson.SelectedIndex = 0;
                ddlVAT.SelectedIndex = 0;

                for (int i = 0; i < gvEnquiry.Rows.Count; i++)
                {
                    TextBox txtRate = (TextBox)gvEnquiry.Rows[i].FindControl("txtRate");
                    TextBox txtDeliveryTime = (TextBox)gvEnquiry.Rows[i].FindControl("txtDeliveryTime");
                    TextBox txtDiscount = (TextBox)gvEnquiry.Rows[i].FindControl("txtDiscount");
                    TextBox txtRemarks1 = (TextBox)gvEnquiry.Rows[i].FindControl("txtRemarks1");

                    if (txtRate != null) { txtRate.ReadOnly = false; txtRate.Text = ""; }
                    if (txtDeliveryTime != null) { txtDeliveryTime.ReadOnly = false; txtDeliveryTime.Text = ""; }
                    if (txtDiscount != null) { txtDiscount.ReadOnly = false; txtDiscount.Text = ""; }
                    if (txtRemarks1 != null) { txtRemarks1.ReadOnly = false; txtRemarks1.Text = ""; }
                }

            }

        }
        private bool ValidateForm()
        {
            bool isValid = true;

            // Reset all borders first
            ResetBorders();

            // Main form validation
            if (string.IsNullOrWhiteSpace(txtQuotationNumber.Text))
            {
                // Override CSS border directly
                txtQuotationNumber.Style["border"] = "1px solid red";
                isValid = false;
            }
            else
            {
                // Reset border to original CSS style
                txtQuotationNumber.Style["border"] = "1px solid #ccc";
            }


            if (ddlQuoteCurrency.SelectedValue == "" || ddlQuoteCurrency.SelectedValue == "0")
            {
                ddlQuoteCurrency.BorderColor = System.Drawing.Color.Red;
                isValid = false;
            }

            if ((ddlPaymentTerms.SelectedIndex == -1) || (ddlPaymentTerms.SelectedIndex == 0))
            {
                ddlPaymentTerms.BorderColor = System.Drawing.Color.Red;
                isValid = false;
            }

            if (ddlContactPerson.SelectedValue == "" || ddlContactPerson.SelectedValue == "0")
            {
                ddlContactPerson.BorderColor = System.Drawing.Color.Red;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtValidityDate.Text))
            {
                txtValidityDate.BorderColor = System.Drawing.Color.Red;
                isValid = false;
            }


            if (string.IsNullOrEmpty(ddlVAT.SelectedValue))
            {
                ddlVAT.BorderColor = System.Drawing.Color.Red;
                isValid = false;
            }
            else
            {
                ddlVAT.BorderColor = System.Drawing.Color.Empty;
            }
            // Grid validation
            for (int i = 0; i < gvEnquiry.Rows.Count; i++)
            {
                TextBox txtRate = (TextBox)gvEnquiry.Rows[i].FindControl("txtRate");
                TextBox txtDeliveryTime = (TextBox)gvEnquiry.Rows[i].FindControl("txtDeliveryTime");
                TextBox txtDiscount = (TextBox)gvEnquiry.Rows[i].FindControl("txtDiscount");

                if (txtRate != null && string.IsNullOrWhiteSpace(txtRate.Text))
                {
                    txtRate.BorderColor = System.Drawing.Color.Red;
                    isValid = false;
                }

                //if (txtDeliveryTime != null && string.IsNullOrWhiteSpace(txtDeliveryTime.Text))
                //{
                //    txtDeliveryTime.BorderColor = System.Drawing.Color.Red;
                //    isValid = false;
                //}
                if (txtDeliveryTime != null &&
    (string.IsNullOrWhiteSpace(txtDeliveryTime.Text) || txtDeliveryTime.Text.Trim() == "0"))
                {
                    txtDeliveryTime.BorderColor = System.Drawing.Color.Red;
                    isValid = false;
                }

                if (txtDiscount != null && string.IsNullOrWhiteSpace(txtDiscount.Text))
                {
                    txtDiscount.BorderColor = System.Drawing.Color.Red;
                    isValid = false;
                }
            }

            return isValid;
        }
        private void ResetBorders()
        {
            var normalColor = System.Drawing.ColorTranslator.FromHtml("#ced4da");

            // Reset main form fields
            txtQuotationNumber.BorderColor = normalColor;
            ddlQuoteCurrency.BorderColor = normalColor;
            ddlPaymentTerms.BorderColor = normalColor;
            ddlContactPerson.BorderColor = normalColor;
            txtValidityDate.BorderColor = normalColor;
            ddlVAT.BorderColor = normalColor;

            // Reset grid fields
            for (int i = 0; i < gvEnquiry.Rows.Count; i++)
            {
                TextBox txtRate = (TextBox)gvEnquiry.Rows[i].FindControl("txtRate");
                TextBox txtDeliveryTime = (TextBox)gvEnquiry.Rows[i].FindControl("txtDeliveryTime");
                TextBox txtDiscount = (TextBox)gvEnquiry.Rows[i].FindControl("txtDiscount");

                if (txtRate != null) txtRate.BorderColor = normalColor;
                if (txtDeliveryTime != null) txtDeliveryTime.BorderColor = normalColor;
                if (txtDiscount != null) txtDiscount.BorderColor = normalColor;
            }
        }
        protected void gvEnquiry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int currencyId = 0;

            string VendorQuotationID = Request.QueryString["VendorQuotationID"]?.ToString();
            string VendorID = Request.QueryString["Ven"]?.ToString();

            // Cache currencyId in ViewState for performance
            if (ViewState["CurrencyID"] == null)
            {
                DataTable dt = _cm.GetQuotationDetails(VendorQuotationID, VendorID);
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("CurrencyID"))
                {
                    currencyId = Convert.ToInt32(dt.Rows[0]["CurrencyID"]);
                    ViewState["CurrencyID"] = currencyId;
                }
            }
            else
            {
                currencyId = (int)ViewState["CurrencyID"];
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblUserRemarksOrig = (Label)e.Row.FindControl("lblUserRemarksOrig");
                Label lblUserRemarks = (Label)e.Row.FindControl("lblUserRemarks");
                string PRRemarks = lblUserRemarksOrig.Text;
                if (PRRemarks.Length > 2)
                {
                    try
                    {
                        PRRemarks = PRRemarks.Remove(1, 14);
                        PRRemarks = PRRemarks.Substring(0, PRRemarks.Length - 26) + "]";
                    }
                    catch { }
                }
                lblUserRemarks.Text = PRRemarks;

                DataTable dtDetails = _cm.GetQuotationDetails(VendorQuotationID, VendorID);
                if (dtDetails != null && dtDetails.Rows.Count > 0)
                {
                    int rowIndex = e.Row.RowIndex;
                    if (rowIndex < dtDetails.Rows.Count)
                    {
                        DataRow row = dtDetails.Rows[rowIndex];

                        DropDownList ddlVAT = (DropDownList)e.Row.FindControl("ddlVAT");
                        TextBox txtRate = (TextBox)e.Row.FindControl("txtRate");
                        TextBox txtDeliveryTime = (TextBox)e.Row.FindControl("txtDeliveryTime");
                        TextBox txtDiscount = (TextBox)e.Row.FindControl("txtDiscount");
                        TextBox txtRemarks1 = (TextBox)e.Row.FindControl("txtRemarks1");
                        Label lblOrderedQty = (Label)e.Row.FindControl("lblOrderedQty");
                        Label lblTotalAmount = (Label)e.Row.FindControl("lblTotalAmount");
                        Label lblDiscountApplied = (Label)e.Row.FindControl("lblDiscountApplied");

                        // Set placeholder and prevent editing before currency symbol
                        if (txtRate != null)
                        {
                            string currencySymbol = GetCurrencySymbolById(currencyId);
                            txtRate.Attributes["placeholder"] = currencySymbol;
                            txtRate.Attributes["onkeydown"] = "return preventPrefixEdit(event, this);";
                            txtRate.Attributes["onpaste"] = "return handlePaste(event, this);";
                            txtRate.Attributes["onclick"] = "ensureCursorAfterSymbol(this);";
                        }

                        if (row["QuoteRate"] != DBNull.Value)
                        {
                            decimal rate = Convert.ToDecimal(row["QuoteRate"]);
                            txtRate.Text = FormatCurrency(rate, currencyId);
                        }

                        //txtDeliveryTime.Text = row["DeliveryTime"]?.ToString();
                        if (txtDeliveryTime != null)
                        {
                            txtDeliveryTime.Text = row["DeliveryTime"] != DBNull.Value ? row["DeliveryTime"].ToString() : "";
                        }

                        txtDiscount.Text = row["Discount"]?.ToString();
                        if (txtRemarks1 != null)
                        {
                            txtRemarks1.Text = row["Remarkforitem"]?.ToString() ?? "";
                        }

                        if (ddlVAT != null && row.Table.Columns.Contains("VATID"))
                        {
                            string vatId = row["VATID"].ToString();
                            ddlVAT.SelectedValue = vatId;
                        }

                        // Calculate total per row (after discount)
                        if (decimal.TryParse(lblOrderedQty.Text, out decimal OrderedQty) &&
                     decimal.TryParse(RemoveCurrencySymbol(txtRate.Text), out decimal Rate))
                        {
                            CalculateRowTotal(e.Row, OrderedQty, Rate, txtDiscount, lblTotalAmount, currencyId);
                        }

                        if (lblDiscountApplied != null && row.Table.Columns.Contains("DiscountApplied"))
                        {
                            decimal discountApplied = 0m;
                            if (row["DiscountApplied"] != DBNull.Value)
                            {
                                discountApplied = Convert.ToDecimal(row["DiscountApplied"]);
                            }
                            lblDiscountApplied.Text = FormatCurrency(discountApplied, currencyId);
                        }
                    }
                }
            }
        }
        private string RemoveCurrencySymbol(string input)
        {
            if (string.IsNullOrEmpty(input)) return "0";
            return new string(input.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray()).Replace(",", "");
        }
        private void CalculateRowTotal(GridViewRow row, decimal OrderedQty, decimal Rate, TextBox txtDiscount, Label lblTotalAmount, int currencyId = 0)
        {
            decimal BaseAmount = OrderedQty * Rate;
            decimal discountAmount = 0;

            if (txtDiscount != null && !string.IsNullOrEmpty(txtDiscount.Text))
            {
                string discountText = txtDiscount.Text.Trim().Replace("%", "").Trim();

                //if (discountText.EndsWith("%"))
                //{
                //    string percentValue = discountText.Replace("%", "");
                //    decimal discountPercent = _cm.ConvertCurrencyStringToDecimal(percentValue);
                //    discountAmount = BaseAmount * discountPercent / 100;
                //}
                //else
                //{
                //    discountAmount = _cm.ConvertCurrencyStringToDecimal(discountText);
                //}

                if (decimal.TryParse(discountText, out decimal discountPercent))
                {
                    // Calculate discount amount from percentage
                    discountAmount = BaseAmount * discountPercent / 100;

                    // Show % in textbox
                    txtDiscount.Text = discountPercent.ToString("0.##") + "%";
                }
            }

            decimal RowTotal = BaseAmount - discountAmount;

            // Display formatted based on selected currency
            lblTotalAmount.Text = FormatCurrency(RowTotal, currencyId);

            Label lblDiscountApplied = (Label)row.FindControl("lblDiscountApplied");
            if (lblDiscountApplied != null)
            {
                lblDiscountApplied.Text = FormatCurrency(discountAmount, currencyId);
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string EnquiryID = "";
            string reference = "";
            bool isValid = true;

            // Check for valid request
            if (Request.QueryString["Enquiry"] != null || Request.QueryString["VendorQuotationID"] != null)
            {
                if (Request.QueryString["Enquiry"] != null)
                {
                    EnquiryID = Request.QueryString["Enquiry"].ToString();
                }
                else
                {
                    // If EnquiryID is not available, checkbox should not be selected
                    if (chkNA.Checked)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Cannot mark as Not Available without Enquiry ID.');", true);
                        chkNA.Checked = false;
                        return;
                    }
                }

                if (chkNA.Checked)
                {
                    _cm.EnquiryID = !string.IsNullOrEmpty(EnquiryID) ? Convert.ToInt32(EnquiryID) : 0;
                    _cm.VendorID = Convert.ToInt32(lblVendorID.Text);

                    _cm.QuotationStatusID = 5;
                    _cm.QuotationNumber = txtQuotationNumber.Text;
                    _cm.PaymentTerms = ddlPaymentTerms.SelectedItem.ToString();
                    _cm.PaymentTermsAdditional = txtPaymentTerms.Text;
                    // Set currency and contact person IDs to 0
                    _cm.QuoteCurrencyid = 0;
                    _cm.ContactPersonID = 0;

                    _cm.ValidityDate = string.IsNullOrEmpty(txtValidityDate.Text) ? DateTime.MinValue : Convert.ToDateTime(txtValidityDate.Text);
                    _cm.Delivery = txtDelivery.Text;
                    _cm.SpecialInstruction = txtSpecialInstruction.Text;
                    _cm.Remarks = txtRemarks.Text;
                    _cm.Revision = 0;

                    _cm.UpdateVendorQuotationForNotAvailable(_cm);
                    for (int i = 0; i < gvEnquiry.Rows.Count; i++)
                    {
                        Label lblVendorQuotationDetailsID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationDetailsID");
                        Label lblVendorQuotationID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationID");

                        // Set only the required fields for Not Available update
                        _cm.VendorQuotationDetailsID = Convert.ToInt32(lblVendorQuotationDetailsID.Text);
                        _cm.VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);
                        _cm.QuotationStatusID = 5; // Set status to Not Available

                        _cm.UpdateVendorQuotationDetailsForNotAvailable(_cm);
                    }
                    divVendor.Visible = false;
                    divSubmited.Visible = false;
                    divSuccess.Visible = true;
                    return;
                }
                else
                {
                    // Validate currency dropdown
                    if (string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue) || ddlQuoteCurrency.SelectedValue == "0")
                    {
                        ddlQuoteCurrency.BorderColor = System.Drawing.Color.Red;
                        isValid = false;
                    }
                    else
                    {
                        ddlQuoteCurrency.BorderColor = System.Drawing.Color.Empty;
                    }

                    // Validate contact person dropdown
                    if (string.IsNullOrEmpty(ddlContactPerson.SelectedValue) || ddlContactPerson.SelectedValue == "0")
                    {
                        ddlContactPerson.BorderColor = System.Drawing.Color.Red;
                        isValid = false;
                    }
                    else
                    {
                        ddlContactPerson.BorderColor = System.Drawing.Color.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(txtAdditionalCharge.Text) &&
                        decimal.TryParse(txtAdditionalCharge.Text, out decimal charge) &&
                        charge != 0)
                    {
                        if (txtAdditionalChargeDescription.Text == "")
                        {
                            txtAdditionalChargeDescription.BorderColor = Color.Red;
                            isValid = false;
                        }
                        else
                        {
                            txtAdditionalChargeDescription.BorderColor = Color.Empty;
                        }
                    }
                    if (!isValid || !ValidateForm())
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill all required fields (marked in red)!');", true);
                        return;
                    }

                    // Assign values normally
                    _cm.VATID = string.IsNullOrEmpty(ddlVAT.SelectedValue) ? 0 : Convert.ToInt32(ddlVAT.SelectedValue);
                    _cm.QuotationNumber = txtQuotationNumber.Text;
                    _cm.PaymentTerms = ddlPaymentTerms.SelectedItem.ToString();
                    _cm.PaymentTermsAdditional = txtPaymentTerms.Text;
                    _cm.QuoteCurrencyid = Convert.ToInt32(ddlQuoteCurrency.SelectedValue);
                    _cm.ContactPersonID = Convert.ToInt32(ddlContactPerson.SelectedValue);
                    _cm.ValidityDate = Convert.ToDateTime(txtValidityDate.Text);
                    _cm.Delivery = txtDelivery?.Text.Trim();
                    _cm.SpecialInstruction = txtSpecialInstruction?.Text.Trim();
                    _cm.Subtotal = _cm.ConvertCurrencyStringToDecimal(lblSubTotal.Text);
                    _cm.GrandTotal = _cm.ConvertCurrencyStringToDecimal(lblGrandTotal.Text);
                    _cm.Remarks = txtRemarks?.Text.Trim();
                    _cm.AdditionalChargeDescription = txtAdditionalChargeDescription.Text;
                    _cm.AdditionalCharges = _cm.ConvertCurrencyStringToDecimal(txtAdditionalCharge.Text);

                    // Handle file uploads ONCE - outside the loop
                    string finalDocumentName = "";
                    string finalCertificateName = "";

                    // Get the VendorQuotationID from the first row (since it's the same for all rows)
                    if (gvEnquiry.Rows.Count > 0)
                    {
                        Label lblVendorQuotationID_First = (Label)gvEnquiry.Rows[0].FindControl("lblVendorQuotationID");
                        string vendorQuotationID = lblVendorQuotationID_First.Text;
                        string vendorID = lblVendorID.Text;

                        // Get existing document and certificate names
                        string existingDocument = "";
                        string existingCertificate = "";

                        DataTable dtExisting = _cm.GetQuotationDetails(vendorQuotationID, vendorID);
                        if (dtExisting != null && dtExisting.Rows.Count > 0)
                        {
                            existingDocument = dtExisting.Rows[0]["Document"]?.ToString() ?? "";
                            existingCertificate = dtExisting.Rows[0]["Certificate"]?.ToString() ?? "";

                            // Clean up "N/A" values
                            if (string.Equals(existingDocument, "N/A", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(existingDocument, "NA", StringComparison.OrdinalIgnoreCase) ||
                                string.IsNullOrEmpty(existingDocument))
                                existingDocument = "";

                            if (string.Equals(existingCertificate, "N/A", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(existingCertificate, "NA", StringComparison.OrdinalIgnoreCase) ||
                                string.IsNullOrEmpty(existingCertificate))
                                existingCertificate = "";
                        }

                        // Set defaults to existing files
                        finalDocumentName = existingDocument;
                        finalCertificateName = existingCertificate;

                        // Handle Document Upload
                        if (fpDocument.HasFile && !string.IsNullOrEmpty(fpDocument.FileName))
                        {
                            string strpath = System.IO.Path.GetExtension(fpDocument.FileName);
                            if (!string.IsNullOrEmpty(strpath))
                            {
                                finalDocumentName = "Document-" + vendorQuotationID + strpath;
                                string filename = Server.MapPath("//QuotationDocuments/" + finalDocumentName);
                                fpDocument.SaveAs(filename);
                            }
                        }

                        // Handle Certificate Upload
                        if (fpCertificate.HasFile && !string.IsNullOrEmpty(fpCertificate.FileName))
                        {
                            string cerPath = System.IO.Path.GetExtension(fpCertificate.FileName);
                            if (!string.IsNullOrEmpty(cerPath))
                            {
                                finalCertificateName = "Certificate-" + vendorQuotationID + cerPath;
                                string cerfilename = Server.MapPath("//QuotationDocuments/" + finalCertificateName);
                                fpCertificate.SaveAs(cerfilename);
                            }
                        }
                    }

                    // Process grid view data
                    for (int i = 0; i < gvEnquiry.Rows.Count; i++)
                    {
                        TextBox txtRate = (TextBox)gvEnquiry.Rows[i].FindControl("txtRate");
                        TextBox txtSpecification = (TextBox)gvEnquiry.Rows[i].FindControl("txtSpecification");
                        TextBox txtDeliveryTime = (TextBox)gvEnquiry.Rows[i].FindControl("txtDeliveryTime");
                        TextBox txtDiscount = (TextBox)gvEnquiry.Rows[i].FindControl("txtDiscount");
                        TextBox txtRemarks1 = (TextBox)gvEnquiry.Rows[i].FindControl("txtRemarks1");
                        Label lblVendorQuotationDetailsID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationDetailsID");
                        Label lblVendorQuotationID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationID");
                        Label lblOrderedQty = (Label)gvEnquiry.Rows[i].FindControl("lblOrderedQty");
                        Label lblDiscountApplied = (Label)gvEnquiry.Rows[i].FindControl("lblDiscountApplied");
                        Label lblTotalAmount = (Label)gvEnquiry.Rows[i].FindControl("lblTotalAmount");

                        _cm.VendorQuotationDetailsID = Convert.ToInt32(lblVendorQuotationDetailsID.Text);
                        _cm.AvailableQuantity = Convert.ToDecimal(lblOrderedQty.Text);
                        _cm.Rate = _cm.ConvertCurrencyStringToDecimal(txtRate.Text);
                        _cm.DeliveryTime = Convert.ToInt32(txtDeliveryTime.Text);
                        _cm.ItemSpecification = txtSpecification.Text;

                        string discountText = txtDiscount?.Text.Trim() ?? "0";
                        discountText = discountText.Replace("%", "").Trim();

                        _cm.Discount = discountText;

                        _cm.DiscountApplied = _cm.ConvertCurrencyStringToDecimal(lblDiscountApplied.Text);
                        _cm.TotalAfterDiscount = _cm.ConvertCurrencyStringToDecimal(lblTotalAmount.Text);
                        _cm.Remarkforitem = txtRemarks1?.Text.Trim();

                        _cm.CreatedDate = DateTime.Now.AddHours(12.5);
                        _cm.OrderedQty = Convert.ToDecimal(lblOrderedQty.Text);
                        _cm.VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);


                        _cm.DocumentName = finalDocumentName;
                        _cm.Certificate = finalCertificateName;

                        if (Request.QueryString["Reference"] != null)
                        {
                            string quotationNumber = txtQuotationNumber.Text.Trim();

                            if (!string.IsNullOrEmpty(EnquiryID))
                            {
                                _cm.EnquiryID = Convert.ToInt32(EnquiryID);
                            }

                            _cm.VendorID = Convert.ToInt32(lblVendorID.Text);
                            _cm.QuotationStatusID = 2;
                            _cm.Revision = 0;
                            _cm.AddVendorQuotationRevisionDetails(_cm);
                            _cm.UpdateVendorQuotation(_cm);

                            _cm.UpdateVendorQuotationDetails(_cm);
                            Session["VendorQuotationID"] = lblVendorQuotationID.Text;
                        }

                        if ((Request.QueryString["VendorQuotationID"] != null) && (Request.QueryString["Ven"] != null))
                        {
                            _cm.QuotationStatusID = 7;
                            string VendorQuotationID = Session["VendorQuotationID"].ToString();
                            string VendorID = Request.QueryString["Ven"];
                            DataTable dt = _cm.GetQuotationDetails(VendorQuotationID);
                            if (Session["Revision"] != null)
                            {
                                _cm.Revision = Convert.ToInt32(Session["Revision"]);
                            }
                            _cm.VendorQuotationID = Convert.ToInt32(VendorQuotationID);
                            _cm.AddVendorQuotationRevisionDetails(_cm);
                            _cm.UpdateVendorQuotation(_cm);
                            _cm.Remarkforitem = txtRemarks1.Text;
                            _cm.UpdateVendorQuotationDetails(_cm);
                        }

                        _cm.VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);
                    }

                    if (ID != null)
                    {
                        foreach (string DocID in ID)
                        {
                            if (DocID != "")
                            {
                                _cm.QuotationDocumentID = Convert.ToInt32(DocID);
                                _cm.UpdateQuotationDocumentsID(_cm);
                            }
                        }
                    }
                    ID = null;

                    divVendor.Visible = false;
                    divSubmited.Visible = false;
                    divSuccess.Visible = true;

                    for (int i = 0; i < gvEnquiry.Rows.Count; i++)
                    {
                        Label lblVendorQuotationID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationID");
                        reference = Session["Reference"].ToString();
                        DataTable dt = _cm.GetVendorContactPerson(reference);
                        if (dt.Rows.Count > 0)
                        {
                            string VendorID = dt.Rows[0]["VendorID"].ToString();
                            Session["VendorID"] = VendorID;
                        }
                        Session["VendorQuotationID"] = lblVendorQuotationID.Text;
                        Response.Redirect("GenerateVendorQuotation.aspx");
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid Access!');", true);
            }
        }

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    string EnquiryID = "";
        //    string reference = "";
        //    bool isValid = true;

        //    // Check for valid request
        //    if (Request.QueryString["Enquiry"] != null || Request.QueryString["VendorQuotationID"] != null)
        //    {
        //        if (Request.QueryString["Enquiry"] != null)
        //        {
        //            EnquiryID = Request.QueryString["Enquiry"].ToString();
        //        }
        //        else
        //        {
        //            // If EnquiryID is not available, checkbox should not be selected
        //            if (chkNA.Checked)
        //            {
        //                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Cannot mark as Not Available without Enquiry ID.');", true);
        //                chkNA.Checked = false;
        //                return;
        //            }
        //        }

        //        if (chkNA.Checked)
        //        {
        //            _cm.EnquiryID = !string.IsNullOrEmpty(EnquiryID) ? Convert.ToInt32(EnquiryID) : 0;
        //            _cm.VendorID = Convert.ToInt32(lblVendorID.Text);

        //            _cm.QuotationStatusID = 5;
        //            _cm.QuotationNumber = txtQuotationNumber.Text;
        //            _cm.PaymentTerms = txtPaymentTerms.Text;

        //            // Set currency and contact person IDs to 0
        //            _cm.QuoteCurrencyid = 0;
        //            _cm.ContactPersonID = 0;

        //            _cm.ValidityDate = string.IsNullOrEmpty(txtValidityDate.Text) ? DateTime.MinValue : Convert.ToDateTime(txtValidityDate.Text);
        //            _cm.Delivery = txtDelivery.Text;
        //            _cm.SpecialInstruction = txtSpecialInstruction.Text;
        //            _cm.Remarks = txtRemarks.Text;
        //            _cm.Revision = 0;

        //            _cm.UpdateVendorQuotationForNotAvailable(_cm);

        //            divVendor.Visible = false;
        //            divSubmited.Visible = false;
        //            divSuccess.Visible = true;
        //            return;
        //        }
        //        else
        //        {

        //            // Validate currency dropdown
        //            if (string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue) || ddlQuoteCurrency.SelectedValue == "0")
        //            {
        //                ddlQuoteCurrency.BorderColor = System.Drawing.Color.Red;
        //                isValid = false;
        //            }
        //            else
        //            {
        //                ddlQuoteCurrency.BorderColor = System.Drawing.Color.Empty;
        //            }

        //            // Validate contact person dropdown
        //            if (string.IsNullOrEmpty(ddlContactPerson.SelectedValue) || ddlContactPerson.SelectedValue == "0")
        //            {
        //                ddlContactPerson.BorderColor = System.Drawing.Color.Red;
        //                isValid = false;
        //            }
        //            else
        //            {
        //                ddlContactPerson.BorderColor = System.Drawing.Color.Empty;
        //            }

        //            if (!isValid || !ValidateForm())
        //            {
        //                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill all required fields (marked in red)!');", true);
        //                return;
        //            }

        //            // Assign values normally
        //            // _cm.VATID = Convert.ToInt32(ddlVAT.SelectedValue);
        //            _cm.VATID = string.IsNullOrEmpty(ddlVAT.SelectedValue) ? 0 : Convert.ToInt32(ddlVAT.SelectedValue);

        //            _cm.QuotationNumber = txtQuotationNumber.Text;
        //            _cm.PaymentTerms = txtPaymentTerms.Text;
        //            _cm.QuoteCurrencyid = Convert.ToInt32(ddlQuoteCurrency.SelectedValue);
        //            _cm.ContactPersonID = Convert.ToInt32(ddlContactPerson.SelectedValue);
        //            _cm.ValidityDate = Convert.ToDateTime(txtValidityDate.Text);
        //            _cm.Delivery = txtDelivery?.Text.Trim();
        //            _cm.SpecialInstruction = txtSpecialInstruction?.Text.Trim();
        //            _cm.Subtotal = _cm.ConvertCurrencyStringToDecimal(lblSubTotal.Text);
        //            _cm.GrandTotal = _cm.ConvertCurrencyStringToDecimal(lblGrandTotal.Text);
        //            _cm.Remarks= txtRemarks?.Text.Trim();
        //            for (int i = 0; i < gvEnquiry.Rows.Count; i++)
        //            {
        //                TextBox txtRate = (TextBox)gvEnquiry.Rows[i].FindControl("txtRate");
        //                TextBox txtDeliveryTime = (TextBox)gvEnquiry.Rows[i].FindControl("txtDeliveryTime");
        //                TextBox txtDiscount = (TextBox)gvEnquiry.Rows[i].FindControl("txtDiscount");
        //                TextBox txtRemarks1 = (TextBox)gvEnquiry.Rows[i].FindControl("txtRemarks1");
        //                Label lblVendorQuotationDetailsID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationDetailsID");
        //                Label lblVendorQuotationID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationID");
        //                Label lblOrderedQty = (Label)gvEnquiry.Rows[i].FindControl("lblOrderedQty");
        //                Label lblDiscountApplied = (Label)gvEnquiry.Rows[i].FindControl("lblDiscountApplied");
        //                Label lblTotalAmount = (Label)gvEnquiry.Rows[i].FindControl("lblTotalAmount");

        //                _cm.VendorQuotationDetailsID = Convert.ToInt32(lblVendorQuotationDetailsID.Text);
        //                _cm.AvailableQuantity = Convert.ToDecimal(lblOrderedQty.Text);
        //                _cm.Rate = _cm.ConvertCurrencyStringToDecimal(txtRate.Text);
        //                _cm.DeliveryTime = Convert.ToInt32(txtDeliveryTime.Text);

        //                // _cm.Discount = (txtDiscount != null && !string.IsNullOrWhiteSpace(txtDiscount.Text)) ? txtDiscount.Text.Trim() : "0%";
        //                string discountText = txtDiscount?.Text.Trim() ?? "0";

        //                if (discountText.EndsWith("%"))
        //                {
        //                    // Remove % to parse the number
        //                    string numericPart = discountText.TrimEnd('%').Trim();

        //                    decimal discountValue = _cm.ConvertCurrencyStringToDecimal(numericPart);

        //                    // Save as properly formatted percentage (e.g., "5.6%")
        //                    _cm.Discount = discountValue.ToString("0.##", CultureInfo.InvariantCulture) + "%";
        //                }
        //                else
        //                {
        //                    // Save as raw decimal (e.g., 1000.25)
        //                    decimal discountValue = _cm.ConvertCurrencyStringToDecimal(discountText);
        //                    _cm.Discount = discountValue.ToString("0.##", CultureInfo.InvariantCulture);
        //                }



        //                _cm.DiscountApplied = _cm.ConvertCurrencyStringToDecimal(lblDiscountApplied.Text);
        //                _cm.TotalAfterDiscount = _cm.ConvertCurrencyStringToDecimal(lblTotalAmount.Text);
        //                _cm.Remarkforitem = txtRemarks1?.Text.Trim();

        //                _cm.CreatedDate = DateTime.Now.AddHours(12.5);
        //                _cm.OrderedQty = Convert.ToDecimal(lblOrderedQty.Text);
        //                _cm.VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);

        //                // Replace the file upload section in your btnSave_Click with this corrected code:

        //                // Get existing document and certificate names first
        //                string existingDocument = "";
        //                string existingCertificate = "";

        //                // Get existing file names from database
        //                DataTable dtExisting = _cm.GetQuotationDetails(_cm.VendorQuotationID.ToString(), _cm.VendorID.ToString());
        //                if (dtExisting != null && dtExisting.Rows.Count > 0)
        //                {
        //                    existingDocument = dtExisting.Rows[0]["Document"]?.ToString() ?? "";
        //                    existingCertificate = dtExisting.Rows[0]["Certificate"]?.ToString() ?? "";

        //                    // Clean up "N/A" values
        //                    if (string.Equals(existingDocument, "N/A", StringComparison.OrdinalIgnoreCase) ||
        //                        string.Equals(existingDocument, "NA", StringComparison.OrdinalIgnoreCase))
        //                        existingDocument = "";

        //                    if (string.Equals(existingCertificate, "N/A", StringComparison.OrdinalIgnoreCase) ||
        //                        string.Equals(existingCertificate, "NA", StringComparison.OrdinalIgnoreCase))
        //                        existingCertificate = "";
        //                }

        //                // Handle Document Upload
        //                string strpath = System.IO.Path.GetExtension(fpDocument.FileName);
        //                if (strpath != "" && fpDocument.HasFile)
        //                {
        //                    // New file selected - upload it
        //                    _cm.DocumentName = "Document-" + _cm.VendorQuotationID + strpath;
        //                    string filename = Server.MapPath("//QuotationDocuments/" + _cm.DocumentName);
        //                    fpDocument.SaveAs(filename);
        //                }
        //                else
        //                {
        //                    // No new file selected - keep existing file
        //                    _cm.DocumentName = existingDocument;
        //                }

        //                // Handle Certificate Upload
        //                string cerPath = System.IO.Path.GetExtension(fpCertificate.FileName);
        //                if (cerPath != "" && fpCertificate.HasFile)
        //                {
        //                    // New file selected - upload it
        //                    _cm.Certificate = "Certificate-" + _cm.VendorQuotationID + cerPath;
        //                    string cerfilename = Server.MapPath("//QuotationDocuments/" + _cm.Certificate);
        //                    fpCertificate.SaveAs(cerfilename);
        //                }
        //                else
        //                {
        //                    // No new file selected - keep existing file
        //                    _cm.Certificate = existingCertificate;
        //                }
        //                //string strpath = System.IO.Path.GetExtension(fpDocument.FileName);
        //                //if (strpath != "")
        //                //{
        //                //    _cm.DocumentName = "Document-" + _cm.VendorQuotationID + strpath;
        //                //    string filename = Server.MapPath("//QuotationDocuments/" + _cm.DocumentName);
        //                //    fpDocument.SaveAs(filename);
        //                //}
        //                //else
        //                //{
        //                //    _cm.DocumentName = "";
        //                //}

        //                //string cerPath = System.IO.Path.GetExtension(fpCertificate.FileName);
        //                //if (cerPath != "")
        //                //{
        //                //    _cm.Certificate = "Certificate-" + _cm.VendorQuotationID + cerPath;
        //                //    string cerfilename = Server.MapPath("//QuotationDocuments/" + _cm.Certificate);
        //                //    fpCertificate.SaveAs(cerfilename);
        //                //}
        //                //else
        //                //{
        //                //    _cm.Certificate = "";
        //                //}

        //                if (Request.QueryString["Reference"] != null)
        //                {
        //                    string quotationNumber = txtQuotationNumber.Text.Trim();

        //                    if (!string.IsNullOrEmpty(EnquiryID))
        //                    {
        //                        _cm.EnquiryID = Convert.ToInt32(EnquiryID);
        //                    }

        //                    _cm.VendorID = Convert.ToInt32(lblVendorID.Text);
        //                    _cm.QuotationStatusID = 2;
        //                    _cm.Revision = 0;
        //                    _cm.AddVendorQuotationRevisionDetails(_cm);
        //                    _cm.UpdateVendorQuotation(_cm);
        //                    //_cm.Remarkforitem = txtRemarks1.Text;
        //                    _cm.UpdateVendorQuotationDetails(_cm);
        //                    Session["VendorQuotationID"] = lblVendorQuotationID.Text;
        //                }

        //                if ((Request.QueryString["VendorQuotationID"] != null) && (Request.QueryString["Ven"] != null))
        //                {
        //                    _cm.QuotationStatusID = 7;
        //                    string VendorQuotationID = Session["VendorQuotationID"].ToString();
        //                    string VendorID = Request.QueryString["Ven"];
        //                    DataTable dt = _cm.GetQuotationDetails(VendorQuotationID);
        //                    if (Session["Revision"] != null)
        //                    {
        //                        _cm.Revision = Convert.ToInt32(Session["Revision"]);
        //                    }
        //                    _cm.VendorQuotationID = Convert.ToInt32(VendorQuotationID);
        //                    _cm.AddVendorQuotationRevisionDetails(_cm);
        //                    _cm.UpdateVendorQuotation(_cm);
        //                    _cm.Remarkforitem = txtRemarks1.Text;
        //                    _cm.UpdateVendorQuotationDetails(_cm);
        //                }

        //                _cm.VendorQuotationID = Convert.ToInt32(lblVendorQuotationID.Text);
        //            }

        //            if (ID != null)
        //            {
        //                foreach (string DocID in ID)
        //                {
        //                    if (DocID != "")
        //                    {
        //                        _cm.QuotationDocumentID = Convert.ToInt32(DocID);
        //                        _cm.UpdateQuotationDocumentsID(_cm);
        //                    }
        //                }
        //            }
        //            ID = null;

        //            divVendor.Visible = false;
        //            divSubmited.Visible = false;
        //            divSuccess.Visible = true;

        //            for (int i = 0; i < gvEnquiry.Rows.Count; i++)
        //            {
        //                Label lblVendorQuotationID = (Label)gvEnquiry.Rows[i].FindControl("lblVendorQuotationID");
        //                reference = Session["Reference"].ToString();
        //                DataTable dt = _cm.GetVendorContactPerson(reference);
        //                if (dt.Rows.Count > 0)
        //                {
        //                    string VendorID = dt.Rows[0]["VendorID"].ToString();
        //                    Session["VendorID"] = VendorID;
        //                }
        //                Session["VendorQuotationID"] = lblVendorQuotationID.Text;
        //                Response.Redirect("GenerateVendorQuotation.aspx");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid Access!');", true);
        //    }
        //}
        protected void txtQuotationNumber_TextChanged(object sender, EventArgs e)
        {

            txtQuotationNumber.Style["border"] = "none";
            txtQuotationNumber.Style["border-bottom"] = "1px solid #ced4da";

        }
        protected void txtPaymentTerms_TextChanged(object sender, EventArgs e)
        {
            ddlPaymentTerms.Style["border"] = "none";
            ddlPaymentTerms.Style["border-bottom"] = "1px solid #ced4da";
        }
        protected void txtValidityDate_TextChanged(object sender, EventArgs e)
        {
            txtValidityDate.Style["border"] = "none";
            txtValidityDate.Style["border-bottom"] = "1px solid #ced4da";
        }
        protected void ddlQuoteCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue))
            {
                int newCurrencyId = int.Parse(ddlQuoteCurrency.SelectedValue);
                string currencySymbol = GetCurrencySymbolById(newCurrencyId);
                // Update ViewState with new currency
                ViewState["CurrencyID"] = newCurrencyId;


                //// If IDR is selected, automatically select VAT 11%
                //if (GetCurrencyCodeById(newCurrencyId)?.ToUpper() == "IDR")
                //{
                //    // Find VAT 11% option
                //    DataTable dtVAT = _cm.getVAT();
                //    DataRow[] rows = dtVAT.Select("Percentage = 11");
                //    if (rows.Length > 0)
                //    {
                //        int vatId11 = Convert.ToInt32(rows[0]["VATID"]);
                //        ddlVAT.SelectedValue = vatId11.ToString();
                //        ddlVAT.Enabled = false; // Disable user changes
                //    }
                //    else
                //    {
                //        // If no 11% VAT found, fallback
                //        ddlVAT.SelectedValue = "0"; // Not Applicable
                //        ddlVAT.Enabled = false;
                //    }
                //}
                //else
                //{
                //    // For other currencies, select Not Applicable and disable
                //    ddlVAT.SelectedValue = "0"; // Not Applicable
                //    ddlVAT.Enabled = false;
                //}

                // Remove currency symbols and reformat all values in grid
                foreach (GridViewRow row in gvEnquiry.Rows)
                {
                    TextBox txtRate = (TextBox)row.FindControl("txtRate");
                    Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");
                    Label lblDiscountApplied = (Label)row.FindControl("lblDiscountApplied");
                    TextBox txtDiscount = (TextBox)row.FindControl("txtDiscount");
                    Label lblOrderedQty = (Label)row.FindControl("lblOrderedQty");

                    // Update placeholder for txtRate when currency changes
                    if (txtRate != null)
                    {
                        txtRate.Attributes["placeholder"] = currencySymbol;
                        txtRate.Attributes["onkeydown"] = "return preventPrefixEdit(event, this);";
                        txtRate.Attributes["onpaste"] = "return handlePaste(event, this);";
                        txtRate.Attributes["onclick"] = "ensureCursorAfterSymbol(this);";
                    }

                    if (txtRate != null && !string.IsNullOrEmpty(txtRate.Text))
                    {
                        decimal rate = _cm.ConvertCurrencyStringToDecimal(txtRate.Text);
                        txtRate.Text = FormatCurrency(rate, newCurrencyId);

                        if (decimal.TryParse(lblOrderedQty.Text, out decimal qty))
                        {
                            CalculateRowTotal(row, qty, rate, txtDiscount, lblTotalAmount, newCurrencyId);
                        }
                    }

                    if (lblTotalAmount != null && !string.IsNullOrEmpty(lblTotalAmount.Text))
                    {
                        decimal total = _cm.ConvertCurrencyStringToDecimal(lblTotalAmount.Text);
                        lblTotalAmount.Text = FormatCurrency(total, newCurrencyId);
                    }

                    if (lblDiscountApplied != null && !string.IsNullOrEmpty(lblDiscountApplied.Text))
                    {
                        decimal discount = _cm.ConvertCurrencyStringToDecimal(lblDiscountApplied.Text);
                        lblDiscountApplied.Text = FormatCurrency(discount, newCurrencyId);
                    }
                    if (txtDiscount != null && !string.IsNullOrEmpty(txtDiscount.Text))
                    {
                        string discountText = txtDiscount.Text.Trim();

                        decimal discountVal = 0m;

                        if (discountText.EndsWith("%"))
                        {
                            // Remove % sign and parse
                            string percentValue = discountText.TrimEnd('%');
                            discountVal = _cm.ConvertCurrencyStringToDecimal(percentValue);
                        }
                        else
                        {
                            discountVal = _cm.ConvertCurrencyStringToDecimal(discountText);
                        }

                        // Always format discount as percentage with correct culture
                        txtDiscount.Text = FormatDiscount(discountVal, newCurrencyId);
                    }
                }

                UpdateTotals();
            }

            ddlQuoteCurrency.Style["border"] = "none";
            ddlQuoteCurrency.Style["border-bottom"] = "1px solid #ced4da";
        }
        public string GetCurrencySymbolById(int currencyId)
        {
            string currencyCode = GetCurrencyCodeById(currencyId);

            switch (currencyCode?.ToUpper())
            {
                case "USD":
                    return "$";
                case "EUR":
                    return "€";
                case "GBP":
                    return "£";
                case "IDR":
                    return "Rp";
                case "INR":
                    return "₹";
                case "JPY":
                    return "¥";
                case "CNY":
                    return "¥";
                default:
                    return currencyCode ?? "$"; // Return code if symbol not found
            }
        }
        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {

            ddlContactPerson.Style["border"] = "none";
            ddlContactPerson.Style["border-bottom"] = "1px solid #ced4da";
        }
        protected void ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            // This is the main VAT dropdown (outside GridView)
            DropDownList ddlVAT = sender as DropDownList;

            if (ddlVAT.SelectedIndex == 0)
            {
                ddlVAT.BorderColor = Color.Red;
            }
            else
            {
                ddlVAT.BorderColor = Color.Empty;
                UpdateTotals(); // Only update totals when VAT is changed
            }


            ddlVAT.Style["border"] = "none";
            ddlVAT.Style["border-bottom"] = "1px solid #ced4da";
        }
        protected void txtRate_TextChanged(object sender, EventArgs e)
        {
            TextBox txtRate = sender as TextBox;
            GridViewRow row = (GridViewRow)txtRate.NamingContainer;

            Label lblOrderedQty = (Label)row.FindControl("lblOrderedQty");
            Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");
            TextBox txtDiscount = (TextBox)row.FindControl("txtDiscount");

            int currencyId = 0;
            if (!string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue))
                currencyId = int.Parse(ddlQuoteCurrency.SelectedValue);

            if (string.IsNullOrEmpty(txtRate.Text) || string.IsNullOrEmpty(lblOrderedQty.Text))
            {
                txtRate.BorderColor = Color.Red;
                lblTotalAmount.Text = "0.00";
            }
            else
            {
                if (decimal.TryParse(lblOrderedQty.Text, out decimal OrderedQty))
                {
                    decimal Rate = _cm.ConvertCurrencyStringToDecimal(txtRate.Text);
                    txtRate.BorderColor = Color.Empty;

                    // Format for display based on selected currency
                    txtRate.Text = FormatCurrency(Rate, currencyId);

                    CalculateRowTotal(row, OrderedQty, Rate, txtDiscount, lblTotalAmount, currencyId);
                    UpdateTotals();
                }
                else
                {
                    txtRate.BorderColor = Color.Red;
                    lblTotalAmount.Text = "0.00";
                }
            }
            string currencySymbol = GetCurrencySymbolById(currencyId);
            string script = $"simpleCurrencyProtect('{txtRate.ClientID}', '{currencySymbol}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), $"protect_{txtRate.ClientID}", script, true);
            txtRate.Style["border"] = "none";
            txtRate.Style["border-bottom"] = "1px solid #ced4da";
        }
        protected void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            TextBox txtDiscount = sender as TextBox;
            GridViewRow row = (GridViewRow)txtDiscount.NamingContainer;
            TextBox txtRate = (TextBox)row.FindControl("txtRate");
            Label lblOrderedQty = (Label)row.FindControl("lblOrderedQty");
            Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");

            int currencyId = 0;
            if (!string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue))
                currencyId = int.Parse(ddlQuoteCurrency.SelectedValue);

            if (!string.IsNullOrEmpty(txtRate.Text) && !string.IsNullOrEmpty(lblOrderedQty.Text))
            {
                if (decimal.TryParse(lblOrderedQty.Text, out decimal OrderedQty))
                {
                    decimal Rate = _cm.ConvertCurrencyStringToDecimal(txtRate.Text);

                    if (!txtDiscount.Text.Trim().EndsWith("%"))
                    {
                        decimal discountValue = _cm.ConvertCurrencyStringToDecimal(txtDiscount.Text);

                        if (discountValue <= 100)
                        {

                            txtDiscount.Text = FormatDiscount(discountValue, currencyId);
                        }
                        else
                        {
                            txtDiscount.Text = currencyId > 0 ? FormatCurrency(discountValue, currencyId) : FormatCurrency(discountValue);
                        }
                    }

                    CalculateRowTotal(row, OrderedQty, Rate, txtDiscount, lblTotalAmount, currencyId);
                    UpdateTotals();
                }
            }
            // RE-APPLY THE JAVASCRIPT PROTECTION FOR RATE TEXTBOX
            string currencySymbol = GetCurrencySymbolById(currencyId);
            string script = $"simpleCurrencyProtect('{txtRate.ClientID}', '{currencySymbol}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), $"protect_{txtRate.ClientID}", script, true);


            txtDiscount.Style["border"] = "none";
            txtDiscount.Style["border-bottom"] = "1px solid #ced4da";
        }
        private void UpdateTotals()
        {
            int currencyId = 0;

            if (!string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue))
                currencyId = int.Parse(ddlQuoteCurrency.SelectedValue);

            // ==========================================
            // 1. SUB TOTAL
            // ==========================================

            decimal subTotal = 0;

            foreach (GridViewRow row in gvEnquiry.Rows)
            {
                Label lblTotalAmount =
                    row.FindControl("lblTotalAmount") as Label;

                if (lblTotalAmount != null)
                {
                    decimal amount =
                        _cm.ConvertCurrencyStringToDecimal(
                            lblTotalAmount.Text);

                    subTotal += amount;
                }
            }

            // ==========================================
            // 2. ADDITIONAL CHARGE
            // ==========================================

            decimal additionalCharge = 0;

            if (!string.IsNullOrWhiteSpace(txtAdditionalCharge.Text) &&
                decimal.TryParse(txtAdditionalCharge.Text, out decimal charge) &&
                charge != 0)
            {
                if (txtAdditionalChargeDescription.Text == "")
                {
                    txtAdditionalChargeDescription.BorderColor = Color.Red;
                    return;
                }
                else
                {
                    txtAdditionalChargeDescription.BorderColor = Color.Empty;
                    additionalCharge =
                   _cm.ConvertCurrencyStringToDecimal(
                       txtAdditionalCharge.Text);
                }

            }

            // ==========================================
            // 3. TOTAL BEFORE VAT
            // ==========================================

            decimal totalBeforeVAT =
                subTotal + additionalCharge;

            // ==========================================
            // 4. VAT
            // ==========================================

            decimal vatAmount = 0;
            decimal vatPercent = 0;

            if (!string.IsNullOrEmpty(ddlVAT.SelectedValue))
            {
                int selectedVATID =
                    int.Parse(ddlVAT.SelectedValue);

                DataTable dtVAT = _cm.getVAT();

                DataRow[] rows =
                    dtVAT.Select(
                        "VATID = " + selectedVATID);

                if (rows.Length > 0 &&
                    rows[0]["Percentage"] != DBNull.Value)
                {
                    vatPercent =
                        Convert.ToDecimal(
                            rows[0]["Percentage"]);

                    // VAT is calculated AFTER additional charges
                    vatAmount =
                        totalBeforeVAT * vatPercent / 100;
                    lblVat.Text = vatAmount.ToString();
                }
            }

            // ==========================================
            // 5. GRAND TOTAL
            // ==========================================

            decimal grandTotal =
                totalBeforeVAT + vatAmount;

            grandTotal =
                Math.Round(
                    grandTotal,
                    2,
                    MidpointRounding.AwayFromZero);

            // ==========================================
            // 6. DISPLAY
            // ==========================================

            lblSubTotal.Text =
                FormatCurrency(subTotal, currencyId);

            lblTotalBeforeVAT.Text =
                FormatCurrency(totalBeforeVAT, currencyId);

            lblGrandTotal.Text =
                FormatCurrency(grandTotal, currencyId);
        }
        //private void UpdateTotals() 
        //{
        //    int currencyId = 0;
        //    if (!string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue))
        //        currencyId = int.Parse(ddlQuoteCurrency.SelectedValue);

        //    decimal subTotal = 0;

        //    foreach (GridViewRow row in gvEnquiry.Rows)
        //    {
        //        Label lblTotalAmount = row.FindControl("lblTotalAmount") as Label;
        //        if (lblTotalAmount != null)
        //        {
        //            decimal amount = _cm.ConvertCurrencyStringToDecimal(lblTotalAmount.Text);
        //            subTotal += amount;
        //        }
        //    }

        //    // Display formatted but store raw decimal
        //    lblSubTotal.Text = FormatCurrency(subTotal, currencyId);

        //    decimal vatAmount = 0;
        //    decimal vatPercent = 0;

        //    if (!string.IsNullOrEmpty(ddlVAT.SelectedValue))
        //    {
        //        int selectedVATID = int.Parse(ddlVAT.SelectedValue);
        //        DataTable dtVAT = _cm.getVAT();
        //        DataRow[] rows = dtVAT.Select("VATID = " + selectedVATID);

        //        if (rows.Length > 0 && rows[0]["Percentage"] != DBNull.Value)
        //        {
        //            vatPercent = Convert.ToDecimal(rows[0]["Percentage"]);
        //            vatAmount = subTotal * vatPercent / 100;
        //        }
        //    }

        //    decimal grandTotal = subTotal + vatAmount;
        //    decimal roundedGrandTotal = Math.Round(grandTotal, 2, MidpointRounding.AwayFromZero);

        //    // Display formatted but store raw decimal
        //    lblGrandTotal.Text = FormatCurrency(roundedGrandTotal, currencyId);

        //    if (gvEnquiry.FooterRow != null)
        //    {
        //        gvEnquiry.FooterRow.Cells[9].Text = "Sub Total";
        //        gvEnquiry.FooterRow.Cells[10].HorizontalAlign = HorizontalAlign.Center;
        //        gvEnquiry.FooterRow.Cells[10].Text = FormatCurrency(subTotal, currencyId);
        //    }
        //}
        public string FormatCurrency(decimal amount)
        {
            if (ddlQuoteCurrency == null || string.IsNullOrEmpty(ddlQuoteCurrency.SelectedValue))
                return amount.ToString("N2");
            if (!int.TryParse(ddlQuoteCurrency.SelectedValue, out int currencyId))
                return amount.ToString("N2");
            return FormatCurrency(amount, currencyId);
        }

        public string FormatCurrency(decimal amount, int currencyId)
        {
            string selectedCurrency = GetCurrencyCodeById(currencyId)?.ToUpper();
            if (string.IsNullOrEmpty(selectedCurrency))
                return amount.ToString("N2");
            return _cm.ToAnyCurrency(amount, currencyId);
        }

        // METHOD 9: GetCurrencyCodeById
        public string GetCurrencyCodeById(int currencyId)
        {
            DataTable dt = _cm.QuoteCurrency();
            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["CurrencyID"] == currencyId)
                {
                    return row["CurrencyCode"].ToString();
                }
            }
            return null;
        }
        private string FormatDiscount(decimal discountValue, int currencyId)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            string currencyCode = GetCurrencyCodeById(currencyId)?.ToUpper() ?? "";

            switch (currencyCode)
            {
                case "IDR":
                    culture = new CultureInfo("id-ID");
                    break;
                case "USD":
                    culture = new CultureInfo("en-US");
                    break;
                case "INR":
                    culture = new CultureInfo("en-IN");
                    break;
                case "CNY":
                    culture = new CultureInfo("zh-CN");
                    break;
            }

            return discountValue.ToString("0.##", culture) + "%";
        }

        protected void ddlPaymentTerms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPaymentTerms.SelectedItem != null && ddlPaymentTerms.SelectedItem.Text.Contains("Partial"))
            {
                txtPaymentTerms.Visible = true;
            }
            else
            {
                txtPaymentTerms.Visible = false;
            }
        }

        protected void txtAdditionalCharge_TextChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }


        //protected void gvEnquiry_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    int currencyId = 0;

        //    string VendorQuotationID = Request.QueryString["VendorQuotationID"]?.ToString();
        //    string VendorID = Request.QueryString["Ven"]?.ToString();

        //    // Cache currencyId in ViewState for performance
        //    if (ViewState["CurrencyID"] == null)
        //    {
        //        DataTable dt = _cm.GetQuotationDetails(VendorQuotationID, VendorID);
        //        if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("CurrencyID"))
        //        {
        //            currencyId = Convert.ToInt32(dt.Rows[0]["CurrencyID"]);
        //            ViewState["CurrencyID"] = currencyId;
        //        }
        //    }
        //    else
        //    {
        //        currencyId = (int)ViewState["CurrencyID"];
        //    }

        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        Label lblUserRemarksOrig = (Label)e.Row.FindControl("lblUserRemarksOrig");
        //        Label lblUserRemarks = (Label)e.Row.FindControl("lblUserRemarks");
        //        string PRRemarks = lblUserRemarksOrig.Text;
        //        if (PRRemarks.Length > 2)
        //        {
        //            try
        //            {
        //                PRRemarks = PRRemarks.Remove(1, 14);
        //                PRRemarks = PRRemarks.Substring(0, PRRemarks.Length - 26) + "]";
        //            }
        //            catch { }
        //        }
        //        lblUserRemarks.Text = PRRemarks;

        //        DataTable dtDetails = _cm.GetQuotationDetails(VendorQuotationID, VendorID);
        //        if (dtDetails != null && dtDetails.Rows.Count > 0)
        //        {
        //            int rowIndex = e.Row.RowIndex;
        //            if (rowIndex < dtDetails.Rows.Count)
        //            {
        //                DataRow row = dtDetails.Rows[rowIndex];

        //                DropDownList ddlVAT = (DropDownList)e.Row.FindControl("ddlVAT");
        //                TextBox txtRate = (TextBox)e.Row.FindControl("txtRate");
        //                TextBox txtDeliveryTime = (TextBox)e.Row.FindControl("txtDeliveryTime");
        //                TextBox txtDiscount = (TextBox)e.Row.FindControl("txtDiscount");
        //                TextBox txtRemarks = (TextBox)e.Row.FindControl("txtRemarks1");
        //                Label lblOrderedQty = (Label)e.Row.FindControl("lblOrderedQty");
        //                Label lblTotalAmount = (Label)e.Row.FindControl("lblTotalAmount");
        //                Label lblDiscountApplied = (Label)e.Row.FindControl("lblDiscountApplied");

        //                // Set Rate formatted
        //                if (row["QuoteRate"] != DBNull.Value)
        //                {
        //                    decimal rate = Convert.ToDecimal(row["QuoteRate"]);
        //                    txtRate.Text = FormatCurrency(rate, currencyId);
        //                }

        //                txtDeliveryTime.Text = row["DeliveryTime"]?.ToString();
        //                txtDiscount.Text = row["Discount"]?.ToString();
        //                if (txtRemarks != null)
        //                {
        //                    txtRemarks.Text = row["Remarks"]?.ToString() ?? "";
        //                }

        //                if (ddlVAT != null && row.Table.Columns.Contains("VATID"))
        //                {
        //                    ddlVAT.SelectedValue = row["VATID"].ToString();
        //                }

        //                // Calculate total per row (after discount)
        //                if (decimal.TryParse(lblOrderedQty.Text, out decimal OrderedQty) &&
        //                    decimal.TryParse(RemoveCurrencySymbol(txtRate.Text), out decimal Rate))
        //                {
        //                    CalculateRowTotal(e.Row, OrderedQty, Rate, txtDiscount, lblTotalAmount, currencyId);
        //                }


        //                if (lblDiscountApplied != null && row.Table.Columns.Contains("DiscountApplied"))
        //                {
        //                    decimal discountApplied = Convert.ToDecimal(row["DiscountApplied"]);
        //                    lblDiscountApplied.Text = FormatCurrency(discountApplied, currencyId);
        //                }

        //            }
        //        }
        //    }
        //}    
        //private void UpdateTotals()
        //{
        //    decimal subTotal = 0;
        //    CultureInfo indonesianCulture = new CultureInfo("id-ID");

        //    // Clone the culture to modify the number format without currency symbol
        //    NumberFormatInfo idrFormat = (NumberFormatInfo)indonesianCulture.NumberFormat.Clone();
        //    idrFormat.CurrencySymbol = "";  // 

        //    foreach (GridViewRow row in gvEnquiry.Rows)
        //    {
        //        Label lblTotalAmount = row.FindControl("lblTotalAmount") as Label;
        //        if (lblTotalAmount != null && decimal.TryParse(lblTotalAmount.Text, out decimal amount))
        //        {
        //            subTotal += amount;
        //        }
        //    }

        //    // Format as number with thousand separator and no decimals
        //    lblSubTotal.Text = subTotal.ToString("N0", idrFormat);

        //    // VAT Calculation
        //    decimal vatAmount = 0;
        //    if (ddlVAT.SelectedIndex > 0)
        //    {
        //        string selectedText = ddlVAT.SelectedItem.Text;
        //        int start = selectedText.IndexOf('(');
        //        int end = selectedText.IndexOf(')');
        //        if (start >= 0 && end > start)
        //        {
        //            string percentString = selectedText.Substring(start + 1, end - start - 1).Replace("%", "");
        //            if (decimal.TryParse(percentString, out decimal vatPercent))
        //            {
        //                vatAmount = subTotal * vatPercent / 100;
        //            }
        //        }
        //    }

        //    decimal grandTotal = subTotal + vatAmount;
        //    decimal decimalPart = grandTotal - Math.Floor(grandTotal);
        //    decimal roundedGrandTotal = (decimalPart < 0.05m) ? Math.Floor(grandTotal) : Math.Ceiling(grandTotal);

        //    lblGrandTotal.Text = roundedGrandTotal.ToString("N0", idrFormat);

        //    // Footer row
        //    if (gvEnquiry.FooterRow != null)
        //    {
        //        gvEnquiry.FooterRow.Cells[8].Text = "Sub Total";
        //        gvEnquiry.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Center;
        //        gvEnquiry.FooterRow.Cells[9].Text = subTotal.ToString("N0", idrFormat);
        //    }
        //}

        //protected void ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DropDownList ddlVAT = sender as DropDownList;

        //    if (ddlVAT.SelectedIndex == 0)
        //    {
        //        ddlVAT.BorderColor = Color.Red;
        //    }
        //    else
        //    {
        //        GridViewRow row = (GridViewRow)ddlVAT.NamingContainer;

        //        TextBox txtRate = (TextBox)row.FindControl("txtRate");
        //        Label lblOrderedQty = (Label)row.FindControl("lblOrderedQty");
        //        Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");

        //        if (string.IsNullOrEmpty(txtRate.Text) || string.IsNullOrEmpty(lblOrderedQty.Text))
        //        {
        //            txtRate.BorderColor = Color.Red;
        //        }
        //        else
        //        {
        //            decimal OrderedQty = 0;
        //            decimal Rate = 0;
        //            if (!decimal.TryParse(lblOrderedQty.Text, out OrderedQty) || !decimal.TryParse(txtRate.Text, out Rate))
        //            {
        //                txtRate.BorderColor = Color.Red;
        //                return;
        //            }
        //            // Extract VAT percentage from dropdown text "VAT (11%)"
        //            decimal VAT = 0;
        //            string selectedText = ddlVAT.SelectedItem.Text;
        //            int start = selectedText.IndexOf('(');
        //            int end = selectedText.IndexOf(')');
        //            if (start >= 0 && end > start)
        //            {
        //                string percentString = selectedText.Substring(start + 1, end - start - 1);

        //                percentString = percentString.Replace("%", "");
        //                decimal.TryParse(percentString, out VAT);
        //            }

        //            decimal BaseAmount = OrderedQty * Rate;
        //            decimal VATAmount = BaseAmount * VAT / 100;
        //            decimal TotalAmount = BaseAmount + VATAmount;

        //            lblTotalAmount.Text = TotalAmount.ToString("#.00");

        //            // Calculate total quotation
        //            decimal QTotal = 0;
        //            foreach (GridViewRow row1 in gvEnquiry.Rows)
        //            {
        //                Label lblTotalAmount1 = row1.FindControl("lblTotalAmount") as Label;
        //                if (lblTotalAmount1 != null && decimal.TryParse(lblTotalAmount1.Text, out decimal amount))
        //                {
        //                    QTotal += amount;
        //                }
        //            }

        //            hdQuotationTotal.Value = QTotal.ToString("#.00");

        //            if (gvEnquiry.FooterRow != null)
        //            {
        //                gvEnquiry.FooterRow.Cells[8].Text = "Total";
        //                gvEnquiry.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Center;
        //                gvEnquiry.FooterRow.Cells[9].Text = hdQuotationTotal.Value;
        //            }
        //            UpdateTotals();
        //        }
        //    }
        //}
        //protected void txtRate_TextChanged(object sender, EventArgs e)
        //{
        //    TextBox txtRate = sender as TextBox;
        //    GridViewRow row = (GridViewRow)txtRate.NamingContainer;

        //    TextBox txtRate1 = (TextBox)row.FindControl("txtRate");
        //    Label lblOrderedQty = (Label)row.FindControl("lblOrderedQty");
        //    Label lblTotalAmount = (Label)row.FindControl("lblTotalAmount");
        //    DropDownList ddlVAT = (DropDownList)row.FindControl("ddlVAT");

        //    if (string.IsNullOrEmpty(txtRate.Text) || string.IsNullOrEmpty(lblOrderedQty.Text))
        //    {
        //        txtRate.BorderColor = Color.Red;
        //    }
        //    else
        //    {
        //        decimal OrderedQty = 0;
        //        decimal Rate = 0;
        //        if (!decimal.TryParse(lblOrderedQty.Text, out OrderedQty) || !decimal.TryParse(txtRate1.Text, out Rate))
        //        {
        //            txtRate.BorderColor = Color.Red;
        //            return;
        //        }

        //        // Extract VAT percentage from dropdown text "VAT (11%)"
        //        decimal VAT = 0;
        //        if (ddlVAT != null && ddlVAT.SelectedIndex > 0)
        //        {
        //            string selectedText = ddlVAT.SelectedItem.Text;
        //            int start = selectedText.IndexOf('(');
        //            int end = selectedText.IndexOf(')');
        //            if (start >= 0 && end > start)
        //            {
        //                string percentString = selectedText.Substring(start + 1, end - start - 2);
        //                percentString = percentString.Replace("%", "");
        //                decimal.TryParse(percentString, out VAT);
        //            }
        //        }

        //        decimal BaseAmount = OrderedQty * Rate;
        //        decimal VATAmount = BaseAmount * VAT / 100;
        //        decimal TotalAmount = BaseAmount + VATAmount;

        //        lblTotalAmount.Text = TotalAmount.ToString("#.00");

        //        decimal QTotal = 0;
        //        foreach (GridViewRow row1 in gvEnquiry.Rows)
        //        {
        //            Label lblTotalAmount1 = row1.FindControl("lblTotalAmount") as Label;
        //            if (lblTotalAmount1 != null && decimal.TryParse(lblTotalAmount1.Text, out decimal amount))
        //            {
        //                QTotal += amount;
        //            }
        //        }

        //        hdQuotationTotal.Value = QTotal.ToString("#.00");

        //        if (gvEnquiry.FooterRow != null)
        //        {
        //            gvEnquiry.FooterRow.Cells[8].Text = "Total";
        //            gvEnquiry.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Center;
        //            gvEnquiry.FooterRow.Cells[9].Text = hdQuotationTotal.Value;
        //        }
        //        UpdateTotals();
        //    }
        //}
        //protected void txtDiscount_TextChanged(object sender, EventArgs e)
        //{
        //    UpdateTotals();
        //}

        //protected void rbDiscount_CheckedChanged(object sender, EventArgs e)
        //{
        //    string input = txtDiscount.Text.Replace("%", "").Trim();

        //    if (rbPercentage.Checked)
        //    {
        //        if (string.IsNullOrEmpty(input))
        //        {
        //            txtDiscount.Text = "%";
        //        }
        //        else if (decimal.TryParse(input, out decimal val))
        //        {
        //            txtDiscount.Text = val.ToString("0.##") + "%";
        //        }
        //    }
        //    else if (rbAmount.Checked)
        //    {
        //        if (string.IsNullOrEmpty(input))
        //        {
        //            txtDiscount.Text = "0.00";
        //        }
        //        else if (decimal.TryParse(input, out decimal val))
        //        {
        //            txtDiscount.Text = val.ToString("0.00");
        //        }
        //    }
        //    else if (rbNoDiscount.Checked)
        //    {
        //        txtDiscount.Text = "";
        //    }

        //    //txtDiscount.Enabled = !rbNoDiscount.Checked; // Optional: disable if no discount
        //    UpdateTotals();
        //}
        //private void UpdateTotals()
        //{
        //    decimal subTotal = 0;

        //    // Calculate subtotal from GridView
        //    foreach (GridViewRow row in gvEnquiry.Rows)
        //    {
        //        Label lblTotalAmount = row.FindControl("lblTotalAmount") as Label;
        //        if (lblTotalAmount != null && decimal.TryParse(lblTotalAmount.Text, out decimal amount))
        //        {
        //            subTotal += amount;
        //        }
        //    }

        //    lblSubTotal.Text = Math.Round(subTotal, 2).ToString("0.00");
        //    hdQuotationTotal.Value = Math.Round(subTotal, 2).ToString("0.00");

        //    // Update GridView footer
        //    if (gvEnquiry.FooterRow != null)
        //    {
        //        gvEnquiry.FooterRow.Cells[8].Text = "Sub Total";
        //        gvEnquiry.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Center;
        //        gvEnquiry.FooterRow.Cells[9].Text = Math.Round(subTotal, 2).ToString("0.00");
        //    }

        //    decimal discountAmount = 0;

        //    if ((rbPercentage.Checked || rbAmount.Checked) && !string.IsNullOrEmpty(txtDiscount.Text))
        //    {
        //        string input = txtDiscount.Text.Replace("%", "").Trim();

        //        if (decimal.TryParse(input, out decimal discountValue))
        //        {
        //            if (rbPercentage.Checked)
        //            {
        //                discountAmount = (subTotal * discountValue) / 100;
        //            }
        //            else if (rbAmount.Checked)
        //            {
        //                discountAmount = discountValue;
        //            }
        //        }
        //    }

        //    lblDiscountAmount.Text = Math.Round(discountAmount, 2).ToString("0.00");

        //    decimal grandTotal = subTotal - discountAmount;
        //    lblGrandTotal.Text = Math.Round(grandTotal, 2).ToString("0.00");
        //} 
        //private void SetDiscountUI(string discountValue)
        //{
        //    if (string.IsNullOrWhiteSpace(discountValue) || discountValue == "0.00" || discountValue == "0")
        //    {
        //        rbNoDiscount.Checked = true;
        //        txtDiscount.Text = "";
        //    }
        //    else if (discountValue.Contains("%"))
        //    {
        //        rbPercentage.Checked = true;
        //        txtDiscount.Text = discountValue;
        //    }
        //    else if (decimal.TryParse(discountValue, out decimal flatDiscount))
        //    {
        //        if (flatDiscount > 0)
        //        {
        //            rbAmount.Checked = true;
        //            txtDiscount.Text = flatDiscount.ToString("0.00");
        //        }
        //        else
        //        {
        //            rbNoDiscount.Checked = true;
        //            txtDiscount.Text = "";
        //        }
        //    }

        //    // Ensure discount textbox is visible and enabled
        //    divDiscountBox.Visible = true;
        //    txtDiscount.Enabled = true;
        //}

        // Simple validation method



    }
}