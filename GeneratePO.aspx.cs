using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using static LIMS.DAL.GLModule;

namespace LIMS
{
    public partial class GeneratePO : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    if (Request.QueryString["POID"] != null)
                    {
                        Session["PurchaseOrderID"] = Convert.ToInt32(Request.QueryString["POID"].ToString());
                    }
                    if (Session["PurchaseOrderID"] != null)
                    {
                        string PurchaseOrderID = Session["PurchaseOrderID"].ToString();
                        string PONumber = "";
                        string PODate = "";
                        string Discount = "";
                        string DiscountAmount = "";
                        string Tax = "";
                        decimal TotalAmount = 0;
                        string SpecialInstruction = "";
                        decimal Amount = 0;
                        string VendorName = "";
                        string AddressLine1 = "";
                        string City = "";
                        string State = "";
                        string Country = "";
                        string Mobile = "";
                        string Email = "";
                        string ItemDescription = "";
                        string Quantity = "";
                        string Rate = "";
                        string VAT = "";
                        string CompanyReg = "";
                        string Fax = "";

                        string Delivery = "";
                        string Transaportation = "";
                        string Payment = "";
                        string Guarantee = "";
                        string Remarks = "";
                        string ContactPerson = "";
                        string PR = "";
                        string AmountinWords = "";
                        string QuotationDate = "";
                        string PRNumber = "";
                        string FarmsonUnit = "";

                        string POHeader = "";
                        string VendorQuotation = "";
                        string ItemID = "";
                        string PRRemarks = "";

                        string VendorID = "";
                        string Currency = "";

                        string ToAddress = "";
                        string FromAddress = "";
                        string Subject = "";
                        string BCCAddress = "";
                        string CCAddress = "";

                        int EnquiryDetailsID = 0;

                        DataTable dt = new DataTable();
                        DataTable dt2 = new DataTable();
                        DataTable dt4 = new DataTable();
                        DataTable dt5 = new DataTable();
                        DataTable dt6 = new DataTable();
                        DataTable dtQuotation = new DataTable();

                        StringBuilder sb = new StringBuilder();
                        StringBuilder sbVendorQuotation = new StringBuilder();
                        StringBuilder SignUser = new StringBuilder();
                        StringBuilder sbPR = new StringBuilder();
                        string ApproverName = "";

                        dtQuotation = _cm.GetQuotation(PurchaseOrderID);
                        if (dtQuotation.Rows.Count > 0)
                        {
                            QuotationDate = Convert.ToDateTime(dtQuotation.Rows[0]["QuotationDate"]).ToString("dd/MMM/yyyy");
                        }


                        dt = _cm.GetPODetails(PurchaseOrderID);
                        if (dt.Rows.Count > 0)
                        {
                            VendorID = dt.Rows[0]["VendorID"].ToString();
                        }
                        DataTable dtPR = _cm.GetPOPR(PurchaseOrderID);
                        if (dtPR.Rows.Count > 0)
                        {
                            string PRNumber1 = dtPR.Rows[0]["PR"].ToString();
                            PRNumber = string.Join(", ",
                                            PRNumber1.Split(',')
                                                 .Select(x => x.Trim())
                                                 .Distinct());

                        }
                        dt = _cm.GetPODetails(PurchaseOrderID, VendorID);
                        if (dt.Rows.Count > 0)
                        {
                            PONumber = dt.Rows[0]["PONumber"].ToString();
                            DateTime po = Convert.ToDateTime(dt.Rows[0]["PODate"].ToString());
                            PODate = po.ToString("dd/MMM/yyyy");
                            VendorName = dt.Rows[0]["VendorName"].ToString();
                            SpecialInstruction = dt.Rows[0]["SpecialInstructions"].ToString();
                            Currency = dt.Rows[0]["CurrencyCode"].ToString();
                            Delivery = dt.Rows[0]["Delivery"].ToString();
                            Transaportation = dt.Rows[0]["Transportation"].ToString();
                            Payment = dt.Rows[0]["Payment"].ToString();
                            Guarantee = dt.Rows[0]["Guarantee"].ToString();
                            Remarks = dt.Rows[0]["Remarks"].ToString();
                            ContactPerson = dt.Rows[0]["ContactPersonName"].ToString();
                            VendorQuotation = dt.Rows[0]["QuoteNo"].ToString();
                            EnquiryDetailsID = Convert.ToInt32(dt.Rows[0]["EnquiryDetailsID"].ToString());
                            sbVendorQuotation.Append("<a href='./VendorQuotation/" + VendorQuotation + ".html' target='_blank'>" + VendorQuotation + "</a>");


                            POHeader = "<img src='http://localhost:41414/assets/img/PO-header- style='display:block;width:100%;height:auto'/>";
                            dt2 = _cm.GetVendorDetails(VendorID);
                            if (dt2.Rows.Count > 0)
                            {
                                VendorName = dt2.Rows[0]["VendorName"].ToString();
                                AddressLine1 = dt2.Rows[0]["AddressLine1"].ToString() + " " + dt2.Rows[0]["AddressLine2"].ToString() + " " + dt2.Rows[0]["AddressLine3"].ToString();
                                City = dt2.Rows[0]["City"].ToString();
                                State = dt2.Rows[0]["State"].ToString();
                                Country = dt2.Rows[0]["Country"].ToString();
                                Mobile = dt2.Rows[0]["ContactMobile"].ToString();
                                Email = dt2.Rows[0]["ContactEmail"].ToString();
                                CompanyReg = dt2.Rows[0]["RegistrationNumber"].ToString();
                                VAT = dt2.Rows[0]["VAT"].ToString();
                                Fax = dt2.Rows[0]["Fax"].ToString();
                            }

                            // Start items table
                            sb.Append("<table style='width:100%; border-collapse:collapse; font-family:Arial; font-size:12px; margin-top:10px;'>");
                            sb.Append("<thead>");
                            sb.Append("<tr style='background-color:#f2f2f2;'>");
                            sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:5%;'>No</th>");
                            sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:5%;'>PP#</th>");
                            if (Currency == "IDR")
                            {
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:45%;'>Keterangan</th>");
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:10%;'>kuantitas</th>");
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:20%;'>Harga Sat (IDR)</th>");
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:10%;'>Disc. (%)</th>");
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:10%;'>Jumlah (IDR)</th>");
                            }
                            else
                            {
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:left; width:45%;'>Description</th>");
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:center; width:10%;'>Quantity</th>");
                                sb.Append($"<th style='border:1px solid #ddd; padding:5px; text-align:right; width:20%;'>Unit Price ({Currency})</th>");
                                sb.Append("<th style='border:1px solid #ddd; padding:5px; text-align:right; width:10%;'>Disc (%)</th>");
                                sb.Append($"<th style='border:1px solid #ddd; padding:5px; text-align:right; width:10%;'>Amount ({Currency})</th>");
                            }

                            sb.Append("</tr>");
                            sb.Append("</thead>");
                            sb.Append("<tbody>");

                            DataTable dt3 = _cm.GetPurchaseItemsDetails(PurchaseOrderID);
                            decimal subtotal = 0;
                            decimal taxAmount = 0;
                            DataTable dtPRRemarks = new DataTable();

                            for (int i = 0; i < dt3.Rows.Count; i++)
                            {
                                var row = dt3.Rows[i];
                                ItemID = row["ItemID"].ToString();
                                dtPRRemarks = _cm.GetPurchaseItemsPRRemarks(PurchaseOrderID, ItemID);
                                PRRemarks = dtPRRemarks.Rows.Count > 0 ? dtPRRemarks.Rows[0]["Remarks"].ToString() : "";

                                ItemDescription = row["ModelNo"].ToString() + " " + " " + row["ItemDescription"].ToString();
                                Quantity = row["Quantity"].ToString();

                                decimal price = Convert.ToDecimal(row["Rate"]);
                                Amount = Convert.ToDecimal(row["TotalAmount"]);

                                string strPrice = "", strAmount = "";
                                switch (Currency)
                                {
                                    case "IDR":
                                        strPrice = _cm.ToIDR(price);
                                        strAmount = _cm.ToIDR(Amount);
                                        break;
                                    case "USD":
                                        strPrice = _cm.ToUSD(price);
                                        strAmount = _cm.ToUSD(Amount);
                                        break;
                                    default:
                                        break;
                                }

                                Discount = row["Discount"].ToString();
                                DiscountAmount = row["DiscountAmount"].ToString();
                                TotalAmount = Convert.ToDecimal(row["GrandTotal"].ToString());
                                if (row["TotalVAT"].ToString() != "")
                                {
                                    taxAmount += Convert.ToDecimal(row["TotalVAT"].ToString());
                                }


                                string VATID = row["VATID"].ToString();
                                DataTable dtVAT = _cm.GetVATIDDetails(VATID);
                                Tax = dtVAT.Rows.Count > 0 ? dtVAT.Rows[0]["Percentage"].ToString() : "0";

                                decimal itemAmount = Convert.ToDecimal(Amount);
                                subtotal += itemAmount;

                                sb.Append("<tr>");
                                sb.Append($"<td style='border:1px solid #ddd; padding:5px;text-align:center;'>{i + 1}</td>");

                                DataTable dtPR1 = _cm.GetPR(EnquiryDetailsID);
                                if (dtPR1.Rows.Count > 0)
                                {
                                    PR = dtPR1.Rows[0]["PR"].ToString();
                                    sbPR.Append("<a href='GeneratePurchaseRequisitionReport.aspx?ID="+ dtPR1.Rows[0]["PRID"].ToString() + "' target='_blank'>" + PR + "</a>");
                                    sb.Append($"<td style='border:1px solid #ddd; padding:5px;text-align:center;'>" + sbPR.ToString() + "</td>");
                                }
                                else
                                {
                                    sb.Append($"<td style='border:1px solid #ddd; padding:5px;text-align:center;'>&nbsp;</td>");
                                }
                                //sb.Append($"<td style='border:1px solid #ddd; padding:5px;'>{ItemDescription}<br/>{PRRemarks}</td>");2Sep'26 Nova requested for PR remarks removal. Anton confirmed
                                sb.Append($"<td style='border:1px solid #ddd; padding:5px;'>{ItemDescription}</td>");
                                sb.Append($"<td style='border:1px solid #ddd; padding:5px; text-align:center;'>{Quantity}</td>");


                                string currencySymbol = Currency == "IDR" ? "Rp " : "";
                                sb.Append($"<td style='border:1px solid #ddd; padding:5px; text-align:right;'>{strPrice}</td>");
                                sb.Append($"<td style='border:1px solid #ddd; padding:5px; text-align:right;font-weight:bold;'>{Discount}</td>");
                                sb.Append($"<td style='border:1px solid #ddd; padding:5px; text-align:right;'>{strAmount}</td>");
                                sb.Append("</tr>");
                            }

                            // Add subtotal row
                            sb.Append("<tr>");
                            sb.Append("<td colspan='6' style='border:1px solid #ddd; padding:5px; text-align:right; font-weight:bold;'>Sub Total</td>");
                            string currencySymbolForTotals = Currency == "IDR" ? "Rp " : "";
                            sb.Append($"<td colspan='1' style='border:1px solid #ddd; padding:5px; text-align:right;'>" + _cm.ToIDR(subtotal) + "</td>");
                            sb.Append("</tr>");

                            if (Currency == "IDR")
                            {
                                sb.Append("<tr>");
                                sb.Append("<td colspan='6' style='border:1px solid #ddd; padding:5px; text-align:right; font-weight:bold;'>PPN</td>");
                                sb.Append($"<td colspan='1' style='border:1px solid #ddd; padding:5px; text-align:right;'> " + _cm.ToIDR(taxAmount) + "</td>");
                                sb.Append("</tr>");
                            }

                            // Add grand total row
                            sb.Append("<tr>");
                            sb.Append("<td colspan='6' style='border:1px solid #ddd; padding:5px; text-align:right; font-weight:bold;'>Total</td>");
                            sb.Append($"<td colspan='1' style='border:1px solid #ddd; padding:5px; text-align:right; font-weight:bold;'>" + _cm.ToIDR(TotalAmount) + "</td>");
                            sb.Append("</tr>");

                            sb.Append("</tbody>");
                            sb.Append("</table>");
                            int poId = Convert.ToInt32(Request.QueryString["POID"]);

                            DataTable dtApproval = _cm.GetPOApprovalDetails(poId);
                            string signature = "";
                            if (dtApproval.Rows.Count > 0)
                            {
                                int approvedBy = Convert.ToInt32(dtApproval.Rows[0]["ApproveRejectBy"]);
                                string approvedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApproveRejectDate"])
                                                      .ToString("dd-MM-yyyy HH:mm");

                                //Get signature of the employee who approved
                                signature = _cm.GetSignature(approvedBy, "PO");

                                if (!string.IsNullOrEmpty(signature))
                                {
                                    SignUser.Append("<div style='text-align:center;font-family:Times New Roman;'>");
                                    SignUser.Append("<div><img src='" + ResolveUrl(signature) + "' style='height:100px;' /></div>");
                                    SignUser.Append("<div style='font-size:10px;font-weight:bold;'>Date/Time: " + approvedDate + "</div>");
                                    SignUser.Append("</div>");
                                }
                            }
                            else
                            {
                                if (Request.QueryString["s"] != null)
                                {
                                    if (Request.QueryString["s"] == "1")
                                    {
                                        if ((Session["EmployeeID"] != null) && (Session["Usertype"].ToString() == "Director"))
                                        {
                                            string EmployeeID = Session["EmployeeID"].ToString();
                                            dtApproval = _cm.GetPOApprover(EmployeeID);
                                            ApproverName = dtApproval.Rows[0]["EmployeeName"].ToString().ToUpper();
                                            string ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApproveRejectDate"]).ToString("dd-MM-yyyy HH:mm");

                                            //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                            //{
                                            //    SignUser.Append("<div style='text-align:center;font-family:Times New Roman;'>");
                                            //    SignUser.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                            //    SignUser.Append("<div style='font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                            //    SignUser.Append("</div></div>");
                                            //}
                                            int userId = Convert.ToInt32(Session["UserID"]);
                                            signature = _cm.GetSignature(userId, "PO");
                                            if (!string.IsNullOrEmpty(signature))
                                            {
                                                SignUser.Append("<div style='text-align:center;font-family:Times New Roman;'>");
                                                SignUser.Append("<div><img src='" + ResolveUrl(signature) + "' style='height: 100 !important;' /></div>");
                                                SignUser.Append("<div style='font-size:10px;font-weight:bold;'>Date/Time: " + ApprovedDate + "</div>");
                                                SignUser.Append("</div>");
                                            }
                                            else if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                            {
                                                SignUser.Append("<div style='text-align:center;font-family:Times New Roman;'>");
                                                SignUser.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                                SignUser.Append("<div style='font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                                SignUser.Append("</div></div>");
                                            }
                                        }
                                    }
                                }
                            }
                            
                            StringBuilder sbRejected = new StringBuilder();
                            DataTable dtReject = _cm.GetPORejectDetails(poId);
                            if (dtReject.Rows.Count > 0)
                            {
                                sbRejected.Append("<div class='watermark'>REJECTED</div>");
                            }
                            string body = objEmail.PrepareMailBodyWith("PO.html",
                                    "{PO}", sb.ToString(),
                                    "{PONumber}", PONumber,
                                    "{PODate}", PODate,
                                    "{VendorName}", VendorName,
                                    "{AddressLine1}", AddressLine1,
                                    "{City}", City,
                                    "{State}", State,
                                    "{Country}", Country,
                                    "{Mobile}", Mobile,
                                    "{Fax}", Fax,
                                    "{Email}", Email,
                                    "{SpecialInstruction}", SpecialInstruction,
                                    "{CompanyRegistration}", CompanyReg,
                                    "{GST}", VAT,
                                    "{POHeader}", POHeader,
                                    "{Unit}", FarmsonUnit,
                                    "{Delivery}", Delivery,
                                    "{Transportation}", Transaportation,
                                    "{Payment}", Payment,
                                    "{Guarantee}", Guarantee,
                                    "{Remarks}", Remarks,
                                    "{VendorContactPerson}", ContactPerson,
                                    "{PRList}", PRNumber,
                                    "{VendorQuotation}", sbVendorQuotation.ToString(),
                                    "{QuotationDate}", QuotationDate,
                                    "{Sign}", SignUser.ToString(),
                                    "{ApproverName}", ApproverName,"{Rejected}", sbRejected.ToString());

                            string str = body.ToString();
                            string filename = Server.MapPath("//PO/PO-" + PurchaseOrderID + ".html");
                            System.IO.File.WriteAllText(@filename, str);
                            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PO/PO-" + PurchaseOrderID + ".html"));
                            divPO.InnerHtml = html;
                            //OpenPDF(filename);
                            _cm.ReportFileName = "PO-" + PurchaseOrderID;
                            _cm.PurchaseOrderID = Convert.ToInt32(PurchaseOrderID);
                            _cm.UpdatePO(_cm);

                            //Send Mail

                            if (Request.QueryString["s"] != null)
                            {
                                if (Request.QueryString["s"] == "1")
                                {
                                    if (Session["Usertype"].ToString() == "Director")
                                    {
                                        DataTable dtEmail = new DataTable();
                                        body = "";

                                        body = objEmail.PrepareMailBodyWith("EmailPO.html", "{VendorName}", VendorName, "{PONumber}", PONumber);
                                        dt = _cm.GetVendorDetails(VendorID);
                                        if (dt.Rows.Count > 0)
                                        {
                                            ToAddress = dt.Rows[0]["ContactEmail"].ToString();
                                        }

                                        dtEmail = _cm.getEmaildetails("Send PO");


                                        if (dtEmail.Rows.Count > 0)
                                        {
                                            FromAddress = dtEmail.Rows[0]["FromAddress"].ToString();
                                            Subject = PONumber + dtEmail.Rows[0]["EmailSubject"].ToString();
                                            BCCAddress = dtEmail.Rows[0]["BCCAddress"].ToString();
                                            CCAddress = dtEmail.Rows[0]["CCAddress"].ToString();
                                        }


                                        try
                                        {
                                            //objEmail.SendMail(FromAddress, ToAddress, Subject, body, filename);
                                        }
                                        catch (Exception ex)
                                        {
                                            // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed. Please Try Again!');", true);
                                        }

                                        if (Session["Failed"] != null)
                                        {
                                            if (Session["Failed"].ToString() == "0")
                                            {
                                                _cm.StatusID = 6;
                                                _cm.PurchaseOrderID = Convert.ToInt32(PurchaseOrderID);
                                                _cm.Reason = "";
                                                _cm.UpdatePurchaseOrderStatus(_cm);
                                                _cm.UpdatePOApproval(_cm);

                                                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                                                if (Session["UserID"] != null)
                                                {
                                                    if (Session["EmployeeID"] != null)
                                                    {
                                                        _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                                                        _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                                                        _cm.Remarks = "PO Approved Email Sent";
                                                        _cm.AddPOHistory(_cm);
                                                    }
                                                    else
                                                    {
                                                        Response.Redirect("Login.aspx");
                                                    }
                                                }
                                                else
                                                {
                                                    Response.Redirect("Login.aspx");
                                                }
                                            }
                                            else if (Session["Failed"].ToString() == "1")
                                            {
                                                if (Session["UserID"] != null)
                                                {
                                                    if (Session["EmployeeID"] != null)
                                                    {
                                                        _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                                                        _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                                                        _cm.Remarks = "PO Approved Email Failed";
                                                        _cm.AddPOHistory(_cm);
                                                    }
                                                    else
                                                    {
                                                        Response.Redirect("Login.aspx");
                                                    }
                                                }
                                                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed. Please Try Again!');", true);
                                            }
                                        }
                                        else
                                        {
                                            // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed. Please Try Again!');", true);
                                        }
                                    }
                                }
                            }

                            GC.Collect();
                        }
                    }
                    else
                    {
                        Response.Redirect("PurchaseOrder.aspx");
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        public DataTable RemoveDuplicateRows(DataTable table, string DistinctColumn)
        {
            try
            {
                ArrayList UniqueRecords = new ArrayList();
                ArrayList DuplicateRecords = new ArrayList();
                foreach (DataRow dRow in table.Rows)
                {
                    if (UniqueRecords.Contains(dRow[DistinctColumn]))
                        DuplicateRecords.Add(dRow);
                    else
                        UniqueRecords.Add(dRow[DistinctColumn]);
                }

                foreach (DataRow dRow in DuplicateRecords)
                {
                    table.Rows.Remove(dRow);
                }

                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //protected void OpenPDF(string path)
        //{
        //    string htmlPath = Server.MapPath(path);

        //    HtmlToPdfConverter converter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);

        //    BlinkConverterSettings settings = new BlinkConverterSettings();

        //    // Path to Blink binaries
        //    settings.BlinkPath = Server.MapPath("~/BlinkBinaries");

        //    converter.ConverterSettings = settings;

        //    //PdfDocument pdfDocument = converter.Convert("file:///" + htmlPath.Replace("\\", "/"));

        //    //using (MemoryStream ms = new MemoryStream())
        //    //{
        //    //    pdfDocument.Save(ms);
        //    //    pdfDocument.Close(true);

        //    //    Response.Clear();
        //    //    Response.ContentType = "application/pdf";
        //    //    Response.AddHeader("Content-Disposition", "attachment;filename=PurchaseOrder.pdf");
        //    //    Response.BinaryWrite(ms.ToArray());
        //    //    Response.End();
        //    //}
        //}
    }
}