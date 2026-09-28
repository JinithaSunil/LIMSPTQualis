using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace LIMS
{
    public partial class CreateRequestCustomerFM_IAT : Page
    {
        private const string SampleTypeIAT = "Public Street Lighting (PJU)";
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
            List<SampleRow> tests;
            IATReportHelper.CompanyContactInfo company;

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                con.Open();
                data = FetchAdditionalDetails(con, inquiryId);

                if (data == null)
                {
                    divCRFReview.InnerHtml = "<p>No inspection request found for InquiryID " + inquiryId + ".</p>";
                    return;
                }

                tests = FetchSampleRows(con, data.InquiryID, data.RequestId);
                company = IATReportHelper.FetchCompanyContactDetails(con, data.InquiryID);
            }

            string html = BuildReportHtml(data, tests, company);

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
            [BuyerCustomerName],[BuyerAddressLine1],
            [PayerContactPerson],[PayerContactNo],[PayerContactEmail],
            [ReportRecipient],[InvoiceRecipient],
            [IATFieldsJson],[IATTXTFieldsJson]
        FROM [dbo].[AdditionalDetails_Certification]
        WHERE [InquiryID] = @InquiryID AND [DepartmentCode] = @DepartmentCode
        ORDER BY [AdditionalDetailsID] DESC";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_LIGHTING");

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;

                    var row = new AdditionalDetailsRow();
                    row.AdditionalDetailsID = rdr["AdditionalDetailsID"] as int? ?? 0;
                    row.InquiryID = rdr["InquiryID"] as int? ?? 0;
                    row.RequestId = rdr["RequestId"] as int? ?? 0;

                    row.ApplicantName = AsString(rdr, "BuyerCustomerName");
                    row.Address = AsString(rdr, "BuyerAddressLine1");
                    row.ContactPerson = AsString(rdr, "PayerContactPerson");
                    row.Phone = AsString(rdr, "PayerContactNo");
                    row.InvoiceEmail = AsString(rdr, "PayerContactEmail");
                    row.ReportRecipient = AsString(rdr, "ReportRecipient");
                    row.InvoiceRecipient = AsString(rdr, "InvoiceRecipient");

                    var common = DAL.JsonFieldHelper.FromJson<DAL.IATCommonFields>(rdr["IATFieldsJson"]);
                    var dept = DAL.JsonFieldHelper.FromJson<DAL.LightingDeptFields>(rdr["IATTXTFieldsJson"]);

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

                    // --- lighting department fields ---
                    row.RoadLocation = dept.RoadLocation ?? "";
                    row.LampPower = dept.LampPowerWatt ?? "";
                    row.NumberOfLampPoints = dept.NumberOfLampPoints;
                    row.PoleHeight = dept.PoleHeightMeter ?? "";
                    row.LampType = dept.LampType ?? "";
                    row.LampSpacing = dept.LampSpacingMeter ?? "";

                    row.RefLampPoleData = dept.RefLampPoleData == "Yes";
                    row.RefLampPointList = dept.RefLampPointList == "Yes";
                    row.RefLayout = dept.RefLayout == "Yes";
                    row.RefCommissioning = dept.RefCommissioning == "Yes";

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

        private List<SampleRow> FetchSampleRows(SqlConnection con, int inquiryId, int requestId)
        {
            var finalized = FetchSampleRowsFromTable(con, "CRFSampleDetails", inquiryId, requestId);
            if (finalized.Count > 0) return finalized;

            return FetchSampleRowsFromTable(con, "CustomerCRFSampleDetails", inquiryId, requestId);
        }

        private List<SampleRow> FetchSampleRowsFromTable(SqlConnection con, string tableName, int inquiryId, int requestId)
        {
            string sql = string.Format(@"
                SELECT [TestParameter],[Remarks]
                FROM [dbo].[{0}]
                WHERE [InquiryId] = @InquiryId AND [CustomerRequestId] = @RequestId AND [SampleType] = @SampleType
                ORDER BY [Created]", tableName);

            var list = new List<SampleRow>();
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                cmd.Parameters.AddWithValue("@RequestId", requestId);
                cmd.Parameters.AddWithValue("@SampleType", SampleTypeIAT);

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var row = new SampleRow();
                        row.TestParameter = rdr["TestParameter"] == DBNull.Value ? "" : rdr["TestParameter"].ToString();
                        row.Remarks = rdr["Remarks"] == DBNull.Value ? "" : rdr["Remarks"].ToString();
                        list.Add(row);
                    }
                }
            }
            return list;
        }

        private string BuildReportHtml(AdditionalDetailsRow data, List<SampleRow> tests, IATReportHelper.CompanyContactInfo company)
        {
            var knownTestNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Average Illuminance Level Inspection",
                "Uniformity of Illuminance Inspection",
                "Dark Spot & Bright Area Mapping (Lighting Spot Analysis)",
                "Average Luminance Level Inspection",
                "Uniformity of Luminance Inspection",
                "Glare Potential Evaluation (Threshold Increment, TI)",
                "Re-Inspection"
            };

            var otherTest = tests.FirstOrDefault(t => !knownTestNames.Contains(t.TestParameter));
            string otherTestName = otherTest != null ? otherTest.TestParameter : "";

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
                IATReportHelper.Checkbox(data.ReportRecipient == "Applicant", "Applicant"),
                IATReportHelper.Checkbox(data.ReportRecipient == "Project Owner", "Project Owner"));
            map["CC"] = HttpUtility.HtmlEncode(data.CC);

            map["InvoiceRecipientBoxes"] = IATReportHelper.CheckboxRow(
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Applicant", "Applicant"),
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Project Owner", "Project Owner"),
                IATReportHelper.Checkbox(data.InvoiceRecipient == "Lamp Manufacturer", "Lamp Manufacturer"));
            map["InvoiceEmail"] = HttpUtility.HtmlEncode(!string.IsNullOrWhiteSpace(data.InvoiceEmail) ? data.InvoiceEmail : company.Email);
            map["OtherInstructions"] = HttpUtility.HtmlEncode(data.OtherInstructions);

            map["ServiceTypeBoxesRow1"] = IATReportHelper.CheckboxRow(
                IATReportHelper.Checkbox(HasTest(tests, "Average Illuminance Level Inspection"), "Average Illuminance Level (Lux)"),
                IATReportHelper.Checkbox(HasTest(tests, "Uniformity of Illuminance Inspection"), "Uniformity of Illuminance"),
                IATReportHelper.Checkbox(HasTest(tests, "Dark Spot & Bright Area Mapping (Lighting Spot Analysis)"), "Dark Spot & Bright Area Mapping"));
            map["ServiceTypeBoxesRow2"] = IATReportHelper.CheckboxRow(
                IATReportHelper.Checkbox(HasTest(tests, "Average Luminance Level Inspection"), "Average Luminance Level (cd/m2)"),
                IATReportHelper.Checkbox(HasTest(tests, "Uniformity of Luminance Inspection"), "Uniformity of Luminance"),
                IATReportHelper.Checkbox(HasTest(tests, "Glare Potential Evaluation (Threshold Increment, TI)"), "Glare Potential Evaluation (TI)"));
            map["ReInspectionBox"] = IATReportHelper.Checkbox(HasTest(tests, "Re-Inspection"), "Re-Inspection, Previous Report No.:");
            map["PreviousReportNo"] = HttpUtility.HtmlEncode(data.PreviousReportNo);
            map["OtherServiceBox"] = IATReportHelper.Checkbox(otherTest != null, "Other:");
            map["OtherServiceText"] = HttpUtility.HtmlEncode(otherTestName);

            map["RoadLocation"] = HttpUtility.HtmlEncode(data.RoadLocation);
            map["LampPower"] = HttpUtility.HtmlEncode(data.LampPower);
            map["NumberOfLampPoints"] = HttpUtility.HtmlEncode(data.NumberOfLampPoints.HasValue ? data.NumberOfLampPoints.Value.ToString(CultureInfo.InvariantCulture) : "");
            map["PoleHeight"] = HttpUtility.HtmlEncode(data.PoleHeight);
            map["LampType"] = HttpUtility.HtmlEncode(data.LampType);
            map["LampSpacing"] = HttpUtility.HtmlEncode(data.LampSpacing);

            map["ReferenceYesBox"] = IATReportHelper.Checkbox(data.ReferenceProvided, "Yes");
            map["ReferenceNoBox"] = IATReportHelper.Checkbox(!data.ReferenceProvided, "No");
            map["RefLampPoleDataBox"] = IATReportHelper.Checkbox(data.RefLampPoleData, "Lamp & Pole Technical Data");
            map["RefLampPointListBox"] = IATReportHelper.Checkbox(data.RefLampPointList, "List of Lamp Points to be Inspected");
            map["RefLayoutBox"] = IATReportHelper.Checkbox(data.RefLayout, "Lamp Point Layout Drawing");
            map["RefCommissioningBox"] = IATReportHelper.Checkbox(data.RefCommissioning, "Operational Commissioning Report");
            map["RefOtherBox"] = IATReportHelper.Checkbox(data.RefOther, "Other:");
            map["OtherReference"] = HttpUtility.HtmlEncode(data.OtherReferenceText);

            map["InspectionLocation"] = HttpUtility.HtmlEncode(data.InspectionLocation);
            map["RequestedInspectionDate"] = data.RequestedInspectionDate.HasValue
                ? data.RequestedInspectionDate.Value.ToString("dd MMMM yyyy", ReportCulture)
                : "";

            map["FactorySupplierName"] = HttpUtility.HtmlEncode(data.FactorySupplierName);
            map["FactoryAddress"] = HttpUtility.HtmlEncode(data.FactoryAddress ?? "").Replace("\r\n", "<br />").Replace("\n", "<br />");
            map["FactoryContactPerson"] = HttpUtility.HtmlEncode(data.FactoryContactPerson);
            map["FactoryPhone"] = HttpUtility.HtmlEncode(data.FactoryPhone);
            map["FactoryFax"] = HttpUtility.HtmlEncode(data.FactoryFax);

            map["DocNo"] = "TSQ-LI-FM-049";
            map["RevNo"] = "00";
            map["EffectiveDate"] = "21 August 2025";

            string templatePath = Server.MapPath("~/InspectionRequestReport_IAT.html");
            return IATReportHelper.RenderTemplate(templatePath, map);
        }

        private static bool HasTest(List<SampleRow> tests, string name)
        {
            return tests.Any(t => string.Equals(t.TestParameter, name, StringComparison.OrdinalIgnoreCase));
        }

        private class AdditionalDetailsRow
        {
            public int AdditionalDetailsID;
            public int InquiryID;
            public int RequestId;

            public string ApplicantName;
            public string Address;
            public string ContactPerson;
            public string Phone;
            public string InvoiceEmail;

            public string Commodity;
            public string ReportRecipient;
            public string InvoiceRecipient;

            public string BookingNo;
            public DateTime RequestDate;
            public string Attn;
            public string Email;
            public string Fax;
            public string CC;
            public string OtherInstructions;

            public string RoadLocation;
            public string LampPower;
            public int? NumberOfLampPoints;
            public string PoleHeight;
            public string LampType;
            public string LampSpacing;

            public bool ReferenceProvided;
            public bool RefLampPoleData;
            public bool RefLampPointList;
            public bool RefLayout;
            public bool RefCommissioning;
            public bool RefOther;
            public string OtherReferenceText;

            public string InspectionLocation;
            public DateTime? RequestedInspectionDate;
            public string PreviousReportNo;

            public string FactorySupplierName;
            public string FactoryAddress;
            public string FactoryContactPerson;
            public string FactoryPhone;
            public string FactoryFax;
        }

        private class SampleRow
        {
            public string TestParameter;
            public string Remarks;
        }
    }
}