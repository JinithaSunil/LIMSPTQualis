using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateVendorQuotation : System.Web.UI.Page
    {

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["Email"] == null)
                {
                    Response.Redirect("AccessDenied.aspx");
                }
                else
                {
                    if (Request.QueryString["ID"] != null && Request.QueryString["type"] != null)
                    {
                        Session["VendorID"] = Request.QueryString["ID"].ToString();
                        Session["VendorQuotationID"] = Request.QueryString["type"].ToString();
                    }
                    //Session["VendorID"] = 867;
                    //Session["VendorQuotationID"] = 7951;
                    if (Session["VendorQuotationID"] != null && Session["VendorID"] != null)
                    {
                        string VendorName = "";
                        string VendorCode = "";
                        string Dealer = "";
                        string TypeofBusiness = "";
                        string TypeofEstablichment = "";
                        string YearofEstablishment = "";
                        string QuoteCurrency = "";
                        string PaymentTerms = "";
                        string slNo = "";
                        string Category = "";
                        string ItemCode = "";
                        string ItemDescription = "";
                        string ItemSpecification = "";
                        string UOM = "";
                        string OrderedQty = "";
                        string QuotationNumber = "";
                        string Rate = "";
                        string DeliveryTime = "";
                        string Remarks = "";
                        string Remarkforitem = "";
                        decimal GrandTotal = 0;
                        decimal TotalAmount = 0;
                        decimal Amount = 0;
                        string AmountinWords = "";
                        int Revision = 0;
                        decimal Tax = 0;
                        decimal TaxAmt = 0;
                        decimal Percentage = 0;
                        decimal PrevTotalAmt = 0;
                        string DiscountAmt = "";
                        string TotalAfterDiscount = "";
                        string CurrencyName = "";

                        string DiscountApplied = "";
                        decimal Subtotal = 0;

                        string AdditionalChargesDescription = "";
                        decimal AdditionalCharges = 0;

                        string Address1 = "";
                        string Address2 = "";
                        string Address3 = "";
                        string City = "";
                        string State = "";
                        string Country = "";
                        string Mobile = "";
                        string Email = "";
                        string PAN = "";
                        string Fax = "";

                        // string GSTNumber = "";
                        string VAT = "";
                        //string PAN = "";
                        string QuotationDate = "";
                        string ValidUpto = "";
                        //string UnitConversion = "";
                        string Delivery = "";
                        // string Transportation = "";
                        string Payment = "";
                        string Guarantee = "";
                        string Document = "";
                        string Certificate = "";
                        string SpecialInstruction = "";
                        string Validity = "";
                        string QuoteRevision = "";
                        string EnquiryID = "";
                        string EnquiryDate = "";
                        string PRList = "";
                        string VendorQuotationNo = "";

                        //string FarmsonUnit = "";
                        //string FPU = "";
                        string POHeader = "";
                        int VendorQuotationDetailsID = 0;
                        string purchaseContactEmail = "";
                        //  string PenaltyClause = "";

                        string VendorQuotationID = Session["VendorQuotationID"].ToString();
                        string VendorID = Session["VendorID"].ToString();
                        DataTable dt = new DataTable();
                        DataTable dtRev = new DataTable();
                        DataTable dtQuote = new DataTable();
                        DataTable dtVendor = new DataTable();

                        StringBuilder sb = new StringBuilder();
                        StringReader sr = new StringReader(sb.ToString());

                        dt.Columns.AddRange(new DataColumn[13]
                        {
                    new DataColumn("SlNo",typeof(string)),
                    new DataColumn("Category",typeof(string)),
                    new DataColumn("ItemCode",typeof(string)),
                    new DataColumn("UOM",typeof(string)),
                    new DataColumn("Quantity",typeof(string)),
                    new DataColumn("QuotationNumber",typeof(string)),
                    new DataColumn("Rate",typeof(string)),
                    new DataColumn("Discount",typeof(string)),
                    new DataColumn("DiscountApplied",typeof(decimal)),
                    new DataColumn("Subtotal",typeof(decimal)),
                    new DataColumn("GrandTotal",typeof(decimal)),
                    new DataColumn("DeliveryTime",typeof(string)),
                    new DataColumn("Remarkforitem",typeof(string)),
                        });

                        dt = _cm.GetQuotationDetails(VendorQuotationID, VendorID);
                        if (dt.Rows.Count > 0)
                        {
                            // Cache main row data
                            DataRow mainRow = dt.Rows[0];
                            VendorID = mainRow["EnquiryVendorID"].ToString();
                            EnquiryID = mainRow["EnquiryID"].ToString();
                            EnquiryDate = Convert.ToDateTime(mainRow["Created"]).ToString("dd/MMM/yyyy");
                            SpecialInstruction = mainRow["SpecialInstruction"].ToString();
                            Remarks = mainRow["Remarks"].ToString();
                            Validity = Convert.ToDateTime(mainRow["ValidityDate"]).ToString("dd/MMM/yyyy");
                            Delivery = mainRow["Delivery"].ToString();
                            Payment = mainRow["PaymentTerms"].ToString();
                            Guarantee = mainRow["Guarantee"].ToString();
                            Document = mainRow["Document"].ToString();
                            Certificate = mainRow["Certificate"].ToString();
                            QuotationDate = Convert.ToDateTime(mainRow["QuotationDate"]).ToString("dd/MMM/yyyy");
                            VendorName = mainRow["VendorName"].ToString();
                            VendorCode = mainRow["VendorCode"].ToString();
                            Dealer = mainRow["DealerName"].ToString();
                            TypeofBusiness = mainRow["TypeOfBusiness"].ToString();
                            YearofEstablishment = mainRow["YearOfEstablishment"].ToString();
                            TypeofEstablichment = mainRow["EstablishmentID"].ToString();
                            QuoteCurrency = mainRow["QuoteCurrencyid"].ToString();
                            PaymentTerms = mainRow["PaymentTerms"].ToString();

                            DataTable dtPurchaseContact = _cm.GetPurchaseContact(EnquiryID);
                            if (dtPurchaseContact.Rows.Count > 0)
                            {
                                purchaseContactEmail = dtPurchaseContact.Rows[0]["Email"].ToString();
                            }

                            DataTable dt2 = _cm.GetPRFromEnquiryID(EnquiryID);
                            if (dt2.Rows.Count > 0)
                            {
                                // Process PR details if needed
                            }

                            DataTable dt3 = _cm.GetVendorQuotationDocuments(VendorQuotationID, VendorID);
                            // OPTIMIZATION 1: Better capacity estimation
                            StringBuilder sbAdditionalDocument = new StringBuilder(dt3.Rows.Count * 150);
                            if (dt3.Rows.Count > 0)
                            {
                                foreach (DataRow dr2 in dt3.Rows)
                                {
                                    sbAdditionalDocument.Append(dr2["FileName"].ToString());
                                    sbAdditionalDocument.Append("</a><br/>\n");
                                }
                            }

                            StringBuilder sbDocument = new StringBuilder(300);
                            StringBuilder sbCertificate = new StringBuilder(300);
                            if (Document != "")
                            {
                                // OPTIMIZATION 2: Single AppendFormat instead of multiple appends
                                sbDocument.AppendFormat("<a href='http://ptqualis.rnisolutions.in:4545/QuotationDocuments/{0}' target='_blank'>{0}</a><br/>\n", Document);
                            }

                            if (Certificate != "")
                            {
                                // OPTIMIZATION 3: Single AppendFormat instead of multiple appends
                                sbCertificate.AppendFormat("<a href='http://ptqualis.rnisolutions.in:4545/QuotationDocuments/{0}' target='_blank'>{0}</a><br/>\n", Certificate);
                            }

                            dtVendor = _cm.GetVendorDetails(VendorID);
                            if (dtVendor.Rows.Count > 0)
                            {
                                DataRow vendorRow = dtVendor.Rows[0];
                                Address1 = vendorRow["AddressLine1"].ToString();
                                Address2 = vendorRow["AddressLine2"].ToString();
                                Address3 = vendorRow["AddressLine3"].ToString();
                                City = vendorRow["City"].ToString();
                                State = vendorRow["State"].ToString();
                                Country = vendorRow["Country"].ToString();
                                Mobile = vendorRow["contactmobile"].ToString();
                                Email = vendorRow["ContactEmail"].ToString();
                                Fax = vendorRow["Fax"].ToString();
                                PAN = vendorRow["PanNumber"].ToString();
                            }

                            // OPTIMIZATION 4: Better initial capacity based on expected row count
                            DataTable dtVQ = _cm.GetVendorQuotationDetails(VendorQuotationID, VendorID);
                            int estimatedSize = dtVQ.Rows.Count * 800 + 5000;
                            sb = new StringBuilder(estimatedSize);

                            sb.Append("<table class='table table-hover invoice-table' cellpadding='0' align:center cellspacing='0' width='100%' style='font-size: 12px;font-family:Arial;border-collapse:collapse'>");
                            sb.Append("<thead style='font-family: Arial, Helvetica, sans-serif; font-weight: 400; color:black; background-color:rgb(171 163 199)'>");
                            sb.Append("<tr>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Sl#</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>PP#</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Item Description</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Item Code</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Delivery Time<br/>(in days)</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Ordered Qty</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>UOM</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Rate</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Currency Name</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Amount</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Discount(%)</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Discount Applied</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Total Amount</b></td>");
                            sb.Append("<td style='text-align:center;border: 1px solid #2F4F4F;background-color:#b34788;color:white;padding:8px;'><b>Remarks</b></td>");
                            sb.Append("</tr>");
                            sb.Append("</thead>");
                            sb.Append("<tbody>");

                            DataTable dtNew = new DataTable();
                            dtNew.Columns.Add("VAT");
                            dtNew.Columns.Add("VATAmt");
                            dtNew.Columns.Add("TotalAmt");

                            decimal vatAmount = 0;
                            decimal vatPercent = 0;
                            decimal totalSubAmount = 0;
                            decimal totalDiscountAmount = 0;
                            decimal finalGrandTotal = 0;
                            string discountFromDB = "";

                            int currencyId = 0;

                            // OPTIMIZATION 5: Move revision query outside loop - CRITICAL FIX!
                            dtRev = _cm.GetVQRevisionDetails(VendorQuotationID, VendorID);

                            if (dtVQ.Rows.Count > 0)
                            {
                                // OPTIMIZATION 6: Use for loop with cached count
                                int rowCount = dtVQ.Rows.Count;
                                for (int freqCount = 0; freqCount < rowCount; freqCount++)
                                {
                                    // Cache current row reference
                                    DataRow currentRow = dtVQ.Rows[freqCount];

                                    sb.Append("<tr>");

                                    slNo = (freqCount + 1).ToString();
                                    Category = currentRow["PurchaseSubGroupName"].ToString();
                                    ItemDescription = currentRow["ItemDescription"].ToString() + "  " + currentRow["ModelNo"].ToString();
                                    ItemSpecification = currentRow["ItemSpecification"].ToString();
                                    ItemCode = currentRow["ItemCode"].ToString();
                                    UOM = currentRow["UOMName"].ToString();
                                    OrderedQty = Convert.ToInt32(currentRow["Quantity"]).ToString();
                                    QuotationNumber = currentRow["QuotationNumber"].ToString();
                                    DiscountAmt = currentRow["Discount"].ToString();
                                    TotalAfterDiscount = currentRow["TotalAfterDiscount"].ToString();
                                    CurrencyName = currentRow["CurrencyName"].ToString();
                                    QuoteCurrency = currentRow["QuoteCurrencyId"].ToString();
                                    VAT = currentRow["VATID"]?.ToString() ?? "0";

                                    // Safe VAT percent parsing with null check
                                    vatPercent = (currentRow["Percentage"] != DBNull.Value) ? Convert.ToDecimal(currentRow["Percentage"]) : 0;

                                    int.TryParse(QuoteCurrency, out currencyId);

                                    DiscountApplied = _cm.ToAnyCurrency(Convert.ToDecimal(currentRow["DiscountApplied"]), currencyId);
                                    string formattedDiscount = DiscountAmt;
                                    string formattedTotalAfterDiscount = _cm.ToAnyCurrency(Convert.ToDecimal(TotalAfterDiscount), currencyId);

                                    Subtotal = Convert.ToDecimal(currentRow["Subtotal"]);
                                    string formattedSubtotal = _cm.ToAnyCurrency(Subtotal, currencyId);

                                    AdditionalChargesDescription = currentRow["AdditionalChargeDescription"].ToString();
                                    if (currentRow["AdditionalCharges"].ToString() != "")
                                    {
                                        AdditionalCharges = Convert.ToDecimal(currentRow["AdditionalCharges"]);
                                    }
                                    
                                    
                                    GrandTotal = Convert.ToDecimal(currentRow["GrandTotal"]);
                                    string formattedGrandTotal = _cm.ToAnyCurrency(GrandTotal, currencyId);
                                    
                                    Rate = _cm.ToAnyCurrency(Convert.ToDecimal(currentRow["QuoteRate"]), currencyId);

                                    DeliveryTime = currentRow["DeliveryTime"].ToString();
                                    Remarkforitem = currentRow["Remarkforitem"].ToString();
                                    VendorQuotationNo = currentRow["QuotationNumber"].ToString();
                                    VendorQuotationDetailsID = Convert.ToInt32(currentRow["VendorQuotationDetailsID"].ToString());

                                    // Use cached dtRev instead of querying again
                                    if (dtRev.Rows.Count > 0 && !string.IsNullOrEmpty(dtRev.Rows[0]["Revision"].ToString()))
                                    {
                                        string formattedAmount = "0.00";
                                        Revision = Convert.ToInt32(dtRev.Rows[0]["Revision"].ToString());
                                        QuoteRevision = dtRev.Rows[0]["Revision"].ToString();

                                        dtQuote = _cm.GetVendorQuotationRevisionDetails(VendorQuotationID, Revision.ToString(), VendorID, VendorQuotationDetailsID);

                                        if (dtQuote.Rows.Count > 0)
                                        {
                                            _cm.Rate = Convert.ToDecimal(dtQuote.Rows[0]["Rate"]);
                                            _cm.OrderedQty = Convert.ToDecimal(dtQuote.Rows[0]["OrderedQty"]);
                                            Amount = _cm.Rate * _cm.OrderedQty;
                                        }
                                    }

                                    dtNew.Rows.Add(VAT, TaxAmt, TotalAmount);

                                    // OPTIMIZATION 7: Build row in one go
                                    sb.Append("<td style='width:3%;border: 1px solid black; text-align:center'>").Append(slNo).Append("</td>");

                                    int EnquiryDetailsID = Convert.ToInt32(mainRow["EnquiryDetailsID"].ToString());
                                    //StringBuilder sbPR = new StringBuilder(); string PR = "";
                                    DataTable dtPR = _cm.GetPR(EnquiryDetailsID);
                                    if (dtPR.Rows.Count > 0)
                                    {
                                        string PR = dtPR.Rows[0]["PR"].ToString();
                                        sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(PR).Append("</td>");
                                        //sbPR.Clear();
                                        //PR = dtPR.Rows[0]["PR"].ToString();
                                        //sbPR.Append("<a href='GeneratePurchaseRequisitionReport.aspx?ID=" + dtPR.Rows[0]["PRID"].ToString() + "' target='_blank'>" + PR + "</a>");
                                        //sb.Append($"<td style='border:1px solid #ddd; padding:5px;text-align:center;'>" + sbPR.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>&nbsp;</td>");
                                    }

                                    if (ItemDescription != "")
                                    {
                                        sb.Append("<td style='width:25%;border: 1px solid ; text-align:left'>").Append(ItemDescription).Append("<br/>").Append(ItemSpecification).Append("</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td style='width:25%;border: 1px solid ; text-align:center'>").Append(Category).Append("</td>");
                                    }

                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(ItemCode).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(DeliveryTime).Append("</td>");
                                    sb.Append("<td style='width:10%;border: 1px solid; text-align:center'>").Append(OrderedQty).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(UOM).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(Rate).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(CurrencyName).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(_cm.ToAnyCurrency(Amount, currencyId)).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(formattedDiscount).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(DiscountApplied).Append("</td>");
                                    sb.Append("<td style='width:5%;border: 1px solid ; text-align:center'>").Append(formattedTotalAfterDiscount).Append("</td>");
                                    sb.Append("<td style='width:10%;border: 1px solid  ; text-align:center'>").Append(Remarkforitem).Append("</td>");
                                    sb.Append("</tr>");
                                }

                                vatAmount = (VAT != "0") ? ((Subtotal+AdditionalCharges) * vatPercent) / 100 : 0;
                            }

                            string formattedTotalSubtotal = _cm.ToAnyCurrency(Subtotal, currencyId);
                            string formattedAdditionalCharges = _cm.ToAnyCurrency(AdditionalCharges, currencyId);
                            string formattedVatAmount = _cm.ToAnyCurrency(vatAmount, currencyId);
                            string formattedTotalGrandTotal = _cm.ToAnyCurrency(GrandTotal, currencyId);

                            sb.Append("</tbody>");
                            sb.Append("<br/><br/>");
                            sb.Append("<table style='width:20%; float:right; border-collapse:collapse; font-family:Arial; font-size:13px;margin-right:9%'>");
                            sb.Append("<tr>");
                            sb.Append("<td style='text-align:right; padding:4px;'>Sub Total</td>");
                            sb.Append("<td style='text-align:right; padding:4px;'>").Append(formattedTotalSubtotal).Append("</td>");
                            sb.Append("</tr>");

                            sb.Append("<tr>");
                            sb.Append("<td style='text-align:right; padding:4px;'>Additional Charges</td>");
                            sb.Append("<td style='text-align:right; padding:4px;'>").Append(formattedAdditionalCharges).Append("</td>");
                            sb.Append("</tr>");

                            if (vatPercent > 0)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td style='text-align:right; padding:4px;'>TAX ").Append((int)vatPercent).Append("%</td>");
                                sb.Append("<td style='text-align:right; padding:4px;'>").Append(formattedVatAmount).Append("</td>");
                                sb.Append("</tr>");
                            }
                            else
                            {
                                sb.Append("<tr>");
                                sb.Append("<td style='text-align:right; padding:4px;'>No Tax</td>");
                                sb.Append("<td style='text-align:right; padding:4px;'>-</td>");
                                sb.Append("</tr>");
                            }

                            sb.Append("<tr>");
                            sb.Append("<td style='text-align:right; padding:4px; font-weight:bold;'>Grand Total</td>");
                            sb.Append("<td style='text-align:right; padding:4px; font-weight:bold;'>").Append(formattedTotalGrandTotal).Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("</table>");

                            sb.Append("<br /><br />");

                            string attach = objEmail.PrepareMailBodyWith("QuotationVendor.html",
                                "{SupplierName}", VendorName,
                                "{VendorCode}", VendorCode,
                                "{Dealer}", Dealer,
                                "{YearofEstablishment}", YearofEstablishment,
                                "{TypeofEstablishment}", TypeofEstablichment,
                                "{TypeofBusiness}", TypeofBusiness,
                                "{QuoteCurrency}", QuoteCurrency,
                                "{PaymentTerms}", PaymentTerms,
                                "{Quotation}", sb.ToString(),
                                "{Address1}", Address1,
                                "{Address2}", Address2,
                                "{Address3}", Address3,
                                "{City}", City,
                                "{State}", State,
                                "{Country}", Country,
                                "{Mobile}", Mobile,
                                "{Email}", Email,
                                "{VAT}", VAT,
                                "{PAN}", PAN,
                                "{QuotationNo}", VendorQuotationID,
                                "{Revision}", QuoteRevision,
                                "{QuotationDate}", QuotationDate,
                                "{ValidUpto}", ValidUpto,
                                "{Delivery}", Delivery,
                                "{Payment}", Payment,
                                "{Guarantee}", Guarantee,
                                "{Document}", sbDocument.ToString(),
                                "{Certificate}", sbCertificate.ToString(),
                                "{Remarks}", Remarks,
                                "{SpecialInstruction}", SpecialInstruction,
                                "{QuotationValidDate}", Validity,
                                "{EnquiryID}", EnquiryID,
                                "{EnquiryDate}", EnquiryDate,
                                "{PRList}", PRList,
                                "{VendorQuotationNo}", VendorQuotationNo,
                                "{POHeader}", POHeader,
                                "{UOM}", UOM,
                                "{purchaseContactEmail}", purchaseContactEmail,
                                "{AdditionalDocument}", sbAdditionalDocument.ToString()
                            );

                            string str = attach.ToString();
                            string filename = Server.MapPath("//VendorQuotation/VendorQuotation-" + VendorQuotationID + VendorID + ".html");
                            System.IO.File.WriteAllText(@filename, str);

                            _cm.ReportFileName = "VendorQuotation-" + VendorQuotationID + VendorID;
                            _cm.VendorQuotationID = Convert.ToInt32(VendorQuotationID);
                            _cm.VendorID = Convert.ToInt32(VendorID);
                            _cm.UpdateVendorQuotationFilename(_cm);

                            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//VendorQuotation/VendorQuotation-" + VendorQuotationID + VendorID + ".html"));
                            divQuotation.InnerHtml = html;
                            SendMail(EnquiryID, VendorID, filename);
                        }
                    }
                }
            }
        }
        private string GetCurrencyCodeById(int currencyId)
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
        private void SendMail(string EnquiryID, string VendorID, string FileName)
        {
            DataTable dt = _cm.GetPurchaseContact(EnquiryID);
            DataTable dt2 = new DataTable();
            if (dt.Rows.Count > 0)
            {
                string FromAddress = "";
                string ToAddress = "";
                string Subject = "";
                string body = "";
                string Attachment = "";
                string BCCAddress = "";
                string CCAddress = "";
                string VendorName = "";

                ToAddress += dt.Rows[0]["Email"].ToString() + ",";
                if (dt.Rows[0]["DepartmentID"].ToString() == "1006")
                {
                    dt2 = _cm.getEmaildetails("GenerateVendorQuotation");
                }

                if (dt2.Rows.Count > 0)
                {
                    FromAddress = dt2.Rows[0]["FromAddress"].ToString();
                    ToAddress += dt2.Rows[0]["ToAddress"].ToString() + ";";
                    CCAddress = dt2.Rows[0]["CCAddress"].ToString();
                    BCCAddress = dt2.Rows[0]["BCCAddress"].ToString();
                    Subject = "ENQ-" + EnquiryID + dt2.Rows[0]["EmailSubject"].ToString();
                }

                DataTable dt3 = _cm.GetVendorDetails(VendorID);
                if (dt3.Rows.Count > 0)
                {
                    VendorName = dt3.Rows[0]["VendorName"].ToString();
                    foreach (DataRow dr in dt3.Rows)
                    {
                        string To = dr["ContactEmail"].ToString();
                        if (To.Contains(","))
                        {
                            string[] AddressList = To.Split(',');

                            foreach (string Address in AddressList)
                            {
                                ToAddress += Address + ",";
                            }
                        }
                        else
                        {
                            ToAddress += dr["ContactEmail"].ToString() + ",";
                        }
                    }
                }
                ToAddress = ToAddress.Remove(ToAddress.Length - 1, 1);
                body = objEmail.PrepareMailBodyWith("EmailVQ.html", "{VendorName}", VendorName, "{EnquiryID}", EnquiryID);
                try
                {
                    objEmail.SendMail(FromAddress, ToAddress, CCAddress, BCCAddress, FileName, Subject, body);
                    _cm.IsSentStatus = true;
                    if (Session["Failed"].ToString() == "0")
                    {
                        _cm.IsSentStatus = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                    }
                    else
                    {
                        _cm.IsSentStatus = false;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed. Please Try Again!');", true);
                }
            }
        }
    }
}
