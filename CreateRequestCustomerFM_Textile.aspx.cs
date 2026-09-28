using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace LIMS
{
    public partial class CreateRequestCustomerFM_Textile : System.Web.UI.Page
    {
        private static readonly CultureInfo ReportCulture = CultureInfo.GetCultureInfo("en-US");

        protected void Page_Load(object sender, EventArgs e)
        {
            int inquiryId;
            if (Request.QueryString["ID"] == null || !int.TryParse(Request.QueryString["ID"], out inquiryId))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            AdditionalDetailsRow data;
            List<SampleRow> services;
            IATReportHelper.CompanyContactInfo company;

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                con.Open();
                data = FetchAdditionalDetails(con, inquiryId);

                if (data == null)
                {
                    divCRFReview.InnerHtml = "<p>No booking form found for InquiryID " + inquiryId + ".</p>";
                    return;
                }

                services = FetchSampleRows(con, data.InquiryID, data.RequestId, data.Commodity);
                company = IATReportHelper.FetchCompanyContactDetails(con, data.InquiryID);
            }

            string html = BuildReportHtml(data, services, company);

            string style;
            string bodyContent;
            SplitStandaloneHtml(html, out style, out bodyContent);

            litReportStyle.Text = style;
            divCRFReview.InnerHtml = bodyContent;
        }

        private static void SplitStandaloneHtml(string fullHtml, out string style, out string bodyContent)
        {
            style = "";
            bodyContent = fullHtml;

            int styleStart = fullHtml.IndexOf("<style", StringComparison.OrdinalIgnoreCase);
            if (styleStart >= 0)
            {
                int styleCloseTagEnd = fullHtml.IndexOf('>', styleStart) + 1;
                int styleEnd = fullHtml.IndexOf("</style>", styleCloseTagEnd, StringComparison.OrdinalIgnoreCase);
                if (styleEnd >= 0)
                {
                    style = fullHtml.Substring(styleStart, (styleEnd + "</style>".Length) - styleStart);
                }
            }

            int bodyStart = fullHtml.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
            if (bodyStart >= 0)
            {
                int bodyOpenTagEnd = fullHtml.IndexOf('>', bodyStart) + 1;
                int bodyEnd = fullHtml.IndexOf("</body>", bodyOpenTagEnd, StringComparison.OrdinalIgnoreCase);
                if (bodyEnd >= 0)
                {
                    bodyContent = fullHtml.Substring(bodyOpenTagEnd, bodyEnd - bodyOpenTagEnd);
                }
            }
        }
        private AdditionalDetailsRow FetchAdditionalDetails(SqlConnection con, int inquiryId)
        {
            const string sql = @"
        SELECT TOP 1
            [AdditionalDetailsID],[InquiryID],[RequestId],
            [BuyerCustomerName],[BuyerAddressLine1],[BuyerCountry],
            [ReportRecipient],[InvoiceRecipient],
            [PayerContactPerson],[PayerContactNo],[PayerContactEmail],
            [ItemStyleNo],[PONo],[CreatedDate],
            [IATFieldsJson],[IATTXTFieldsJson]
        FROM [dbo].[AdditionalDetails_Certification]
        WHERE [InquiryID] = @InquiryID AND [DepartmentCode] = @DepartmentCode
        ORDER BY [AdditionalDetailsID] DESC";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_TEXTILE");

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;

                    var row = new AdditionalDetailsRow();
                    row.AdditionalDetailsID = rdr["AdditionalDetailsID"] as int? ?? 0;
                    row.InquiryID = rdr["InquiryID"] as int? ?? 0;
                    row.RequestId = rdr["RequestId"] as int? ?? 0;

                    row.ApplicantName = AsString(rdr, "BuyerCustomerName");
                    row.Address = AsString(rdr, "BuyerAddressLine1");
                    row.BuyerCountry = AsString(rdr, "BuyerCountry");
                    row.ReportRecipient = AsString(rdr, "ReportRecipient");
                    row.InvoiceRecipient = AsString(rdr, "InvoiceRecipient");
                    row.ContactPerson = AsString(rdr, "PayerContactPerson");
                    row.Phone = AsString(rdr, "PayerContactNo");
                    row.InvoiceEmail = AsString(rdr, "PayerContactEmail");
                    row.ItemStyleNo = AsString(rdr, "ItemStyleNo");
                    row.PONumber = AsString(rdr, "PONo");

                    var common = DAL.JsonFieldHelper.FromJson<DAL.IATCommonFields>(rdr["IATFieldsJson"]);
                    var dept = DAL.JsonFieldHelper.FromJson<DAL.TextileDeptFields>(rdr["IATTXTFieldsJson"]);

                    // --- common IAT fields ---
                    row.Commodity = common.ServiceRequired ?? "";
                    row.BookingNo = common.BookingFormNo ?? "";
                    row.RequestDate = common.RequestDate ?? DateTime.MinValue;
                    row.Attn = common.Attn ?? "";
                    row.Email = common.ApplicantEmail ?? "";
                    row.Fax = common.ApplicantFax ?? "";
                    row.CC = common.CC ?? "";
                    row.OtherInstructions = common.OtherInstructions ?? "";

                    row.ReferenceProvided = common.ReferenceProvided == "Yes";
                    row.RefOther = common.RefOther == "Yes";
                    row.OtherReferenceText = common.OtherReferenceText ?? "";

                    row.InspectionLocation = common.InspectionLocation ?? "";
                    row.RequestedInspectionDate = common.RequestedInspectionDate;
                    row.PreviousReportNo = common.PreviousReportNo ?? "";

                    // --- textile department fields ---
                    row.BuyerName = dept.BuyerName ?? "";
                    row.ProductDescription = dept.ProductDescription ?? "";
                    row.ShipmentQty = dept.ShipmentQty ?? "";
                    row.PercentReady = dept.PercentReady ?? "";
                    row.ShippingDate = dept.ShippingDate;

                    row.RefApprovedSamples = dept.RefApprovedSamples == "Yes";
                    row.RefSwatchCard = dept.RefSwatchCard == "Yes";
                    row.RefLCCopy = dept.RefLCCopy == "Yes";
                    row.RefCertificate = dept.RefCertificate == "Yes";
                    row.RefProductSpec = dept.RefProductSpec == "Yes";
                    row.RefPOCopy = dept.RefPOCopy == "Yes";
                    row.RefClientInstructionSheet = dept.RefClientInstructionSheet == "Yes";
                    row.RefConstructionDataForm = dept.RefConstructionDataForm == "Yes";
                    row.RefDigitalImages = dept.RefDigitalImages == "Yes";
                    row.RefPackingListInvoice = dept.RefPackingListInvoice == "Yes";
                    row.RefRoadMap = dept.RefRoadMap == "Yes";

                    row.FactorySupplierName = dept.FactorySupplierName ?? "";
                    row.FactoryAddress = dept.FactoryAddress ?? "";
                    row.FactoryContactPerson = dept.FactoryContactPerson ?? "";
                    row.FactoryPhone = dept.FactoryPhone ?? "";
                    row.FactoryFax = dept.FactoryFax ?? "";

                    return row;
                }
            }
        }

        private static string AsString(SqlDataReader rdr, string column)
        {
            object v = rdr[column];
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }

        private List<SampleRow> FetchSampleRows(SqlConnection con, int inquiryId, int requestId, string commodity)
        {
            const string sql = @"
                SELECT [TestParameter],[Remarks]
                FROM [dbo].[CustomerCRFSampleDetails]
                WHERE [InquiryId] = @InquiryId AND [CustomerRequestId] = @RequestId AND [SampleType] = @SampleType
                ORDER BY [Created]";

            var list = new List<SampleRow>();
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                cmd.Parameters.AddWithValue("@RequestId", requestId);
                cmd.Parameters.AddWithValue("@SampleType", commodity ?? "");

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var row = new SampleRow();
                        row.TestParameter = rdr["TestParameter"].ToString();
                        row.Remarks = rdr["Remarks"].ToString();
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        private string BuildReportHtml(AdditionalDetailsRow data, List<SampleRow> services, IATReportHelper.CompanyContactInfo company)
        {
            var knownServiceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Initial Production Inspection (IPI)",
                "During Production Inspection (DPI)",
                "Final Random Inspection (FRI)",
                "Loading Supervision (LS)",
                "Sample Sealing",
                "Fabric Inspection",
                "Re-Inspection"
            };

            var otherService = services.FirstOrDefault(s => !knownServiceNames.Contains(s.TestParameter));
            string otherServiceName = otherService != null ? otherService.TestParameter : "";

            var map = new Dictionary<string, string>();
            map["Commodity"] = HttpUtility.HtmlEncode(data.Commodity);
            map["BookingNo"] = HttpUtility.HtmlEncode(data.BookingNo);
            map["RequestDate"] = data.RequestDate == DateTime.MinValue ? "" : data.RequestDate.ToString("dd MMMM yyyy", ReportCulture);
            map["Attn"] = HttpUtility.HtmlEncode(data.Attn);

            map["ApplicantName"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.ApplicantName) ? data.ApplicantName : company.CompanyName);
            map["Address"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.Address) ? data.Address : company.Address);
            map["ContactPerson"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.ContactPerson) ? data.ContactPerson : company.Contactperson);
            map["Phone"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.Phone) ? data.Phone : company.Phone);
            map["Fax"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.Fax) ? data.Fax : company.Fax);
            map["Email"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.Email) ? data.Email : company.Email);

            map["ReportRecipientBoxes"] = IATReportHelper.CheckboxRow(
                IATReportHelper.Checkbox(data.ReportRecipient == "Buyer", "Buyer"),
                IATReportHelper.Checkbox(data.ReportRecipient == "Applicant", "Applicant"));
            map["CC"] = HttpUtility.HtmlEncode(data.CC);

            map["InvoiceRecipientBoxes"] = IATReportHelper.CheckboxRow(
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Buyer", "Buyer"),
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Applicant", "Applicant"),
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Supplier", "Supplier"),
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Factory", "Factory"));
            map["InvoiceEmail"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.InvoiceEmail) ? data.InvoiceEmail : company.Email);
            map["OtherInstructions"] = HttpUtility.HtmlEncode(data.OtherInstructions);
            map["BuyerName"] = HttpUtility.HtmlEncode(data.BuyerName);
            map["BuyerCountry"] = HttpUtility.HtmlEncode(data.BuyerCountry);

            map["Chk_IPI"] = IATReportHelper.Checkbox(HasService(services, "Initial Production Inspection (IPI)"), "Initial Production Inspection (IPI)");
            map["Chk_DPI"] = IATReportHelper.Checkbox(HasService(services, "During Production Inspection (DPI)"), "During Production Inspection (DPI)");
            map["Chk_FRI"] = IATReportHelper.Checkbox(HasService(services, "Final Random Inspection (FRI)"), "Final Random Inspection (FRI)");
            map["Chk_LoadingSupervision"] = IATReportHelper.Checkbox(HasService(services, "Loading Supervision (LS)"), "Loading Supervision (LS)");
            map["Chk_SampleSealing"] = IATReportHelper.Checkbox(HasService(services, "Sample Sealing"), "Sample Sealing");
            map["Chk_FabricInspection"] = IATReportHelper.Checkbox(HasService(services, "Fabric Inspection"), "Fabric Inspection");
            map["Chk_ReInspection"] = IATReportHelper.Checkbox(HasService(services, "Re-Inspection"), "Re-Inspection (Previous Report No.:");
            map["PreviousReportNo"] = HttpUtility.HtmlEncode(data.PreviousReportNo) + ")";
            map["Chk_Other"] = IATReportHelper.Checkbox(otherService != null, "Others:");
            map["OtherService"] = HttpUtility.HtmlEncode(otherServiceName);

            map["ProductDescription"] = HttpUtility.HtmlEncode(data.ProductDescription);
            map["ShipmentQty"] = HttpUtility.HtmlEncode(data.ShipmentQty);
            map["ItemStyleNo"] = HttpUtility.HtmlEncode(data.ItemStyleNo);
            map["PercentReady"] = HttpUtility.HtmlEncode(data.PercentReady);
            map["PONumber"] = HttpUtility.HtmlEncode(data.PONumber);
            map["ShippingDate"] = data.ShippingDate.HasValue ? data.ShippingDate.Value.ToString("dd MMMM yyyy", ReportCulture) : "";

            map["ReferenceYesBox"] = IATReportHelper.Checkbox(data.ReferenceProvided, "Yes");
            map["ReferenceNoBox"] = IATReportHelper.Checkbox(!data.ReferenceProvided, "No");
            map["Chk_RefApprovedSamples"] = IATReportHelper.Checkbox(data.RefApprovedSamples, "Approved Samples");
            map["Chk_RefProductSpec"] = IATReportHelper.Checkbox(data.RefProductSpec, "Product Specification / Drawing");
            map["Chk_RefDigitalImages"] = IATReportHelper.Checkbox(data.RefDigitalImages, "Digital Images");
            map["Chk_RefSwatchCard"] = IATReportHelper.Checkbox(data.RefSwatchCard, "Swatch Card");
            map["Chk_RefPOCopy"] = IATReportHelper.Checkbox(data.RefPOCopy, "PO Copy");
            map["Chk_RefPackingListInvoice"] = IATReportHelper.Checkbox(data.RefPackingListInvoice, "Packing List / Invoice Copy");
            map["Chk_RefLCCopy"] = IATReportHelper.Checkbox(data.RefLCCopy, "LC Copy");
            map["Chk_RefClientInstructionSheet"] = IATReportHelper.Checkbox(data.RefClientInstructionSheet, "Client Instruction Sheet");
            map["Chk_RefRoadMap"] = IATReportHelper.Checkbox(data.RefRoadMap, "Road Map");
            map["Chk_RefCertificate"] = IATReportHelper.Checkbox(data.RefCertificate, "Certificate");
            map["Chk_RefConstructionDataForm"] = IATReportHelper.Checkbox(data.RefConstructionDataForm, "Construction Data Form");
            map["Chk_RefOther"] = IATReportHelper.Checkbox(data.RefOther, "Other:");
            map["OtherReference"] = HttpUtility.HtmlEncode(data.OtherReferenceText);

            map["InspectionLocation"] = HttpUtility.HtmlEncode(data.InspectionLocation);
            map["FactorySupplierName"] = HttpUtility.HtmlEncode(data.FactorySupplierName);
            map["FactoryAddress"] = HttpUtility.HtmlEncode(data.FactoryAddress);
            map["FactoryContactPerson"] = HttpUtility.HtmlEncode(data.FactoryContactPerson);
            map["FactoryPhone"] = HttpUtility.HtmlEncode(data.FactoryPhone);
            map["FactoryFax"] = HttpUtility.HtmlEncode(data.FactoryFax);
            map["RequestedInspectionDate"] = data.RequestedInspectionDate.HasValue
                ? data.RequestedInspectionDate.Value.ToString("dd MMMM yyyy", ReportCulture)
                : "";

            map["DocNo"] = "TSQ-LI-FM-039";
            map["RevNo"] = "03";
            map["EffectiveDate"] = "23 September 2021";

            string templatePath = Server.MapPath("~/BookingFormReport_Textile.html");
            return IATReportHelper.RenderTemplate(templatePath, map);
        }

        private static bool HasService(List<SampleRow> services, string name)
        {
            return services.Any(s => string.Equals(s.TestParameter, name, StringComparison.OrdinalIgnoreCase));
        }

        private class AdditionalDetailsRow
        {
            public int AdditionalDetailsID;
            public int InquiryID;
            public int RequestId;
            public string ApplicantName;
            public string Address;
            public string BuyerCountry;
            public string Commodity;
            public string ReportRecipient;
            public string InvoiceRecipient;
            public string ContactPerson;
            public string Phone;
            public string InvoiceEmail;
            public string ItemStyleNo;
            public string PONumber;

            public string BookingNo;
            public DateTime RequestDate;
            public string Attn;
            public string Email;
            public string Fax;
            public string CC;
            public string OtherInstructions;
            public string BuyerName;

            public string ProductDescription;
            public string ShipmentQty;
            public string PercentReady;
            public DateTime? ShippingDate;

            public bool ReferenceProvided;
            public bool RefApprovedSamples;
            public bool RefSwatchCard;
            public bool RefLCCopy;
            public bool RefCertificate;
            public bool RefProductSpec;
            public bool RefPOCopy;
            public bool RefClientInstructionSheet;
            public bool RefConstructionDataForm;
            public bool RefDigitalImages;
            public bool RefPackingListInvoice;
            public bool RefRoadMap;
            public bool RefOther;
            public string OtherReferenceText;

            public string InspectionLocation;
            public string FactorySupplierName;
            public string FactoryAddress;
            public string FactoryContactPerson;
            public string FactoryPhone;
            public string FactoryFax;
            public DateTime? RequestedInspectionDate;
            public string PreviousReportNo;
        }

        private class SampleRow
        {
            public string TestParameter;
            public string Remarks;
        }
    }
}