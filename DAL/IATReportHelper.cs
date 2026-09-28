using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace LIMS
{

    public static class IATReportHelper
    {
        public static string Checkbox(bool isChecked, string label)
        {
            string glyph = isChecked ? "\u2612" /* ☒ */ : "\u2610" /* ☐ */;
            return "<span class=\"chk\"><span class=\"chk-glyph\">" + glyph + "</span> " +
                   "<span class=\"chk-label\">" + HttpUtility.HtmlEncode(label) + "</span></span>";
        }

        /// <summary>Joins several Checkbox() outputs with the standard inline spacer.</summary>
        public static string CheckboxRow(params string[] renderedCheckboxes)
        {
            return string.Join("<span class=\"chk-sep\">&nbsp;&nbsp;&nbsp;</span>", renderedCheckboxes);
        }

        public static Dictionary<string, string> ParseTechnicalDescription(string blob)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(blob)) return result;

            foreach (var part in blob.Split('|'))
            {
                var kv = part.Split(new[] { ':' }, 2);
                if (kv.Length == 2)
                    result[kv[0].Trim()] = kv[1].Trim();
            }
            return result;
        }

        public class CompanyContactInfo
        {
            public string CompanyName = "";
            public string Address = "";
            public string Contactperson = "";
            public string Email = "";
            public string Phone = "";
            public string Fax = "";
            public string CustomerMasterId = "";
            public string PersonId = "";
        }

        public static CompanyContactInfo FetchCompanyContactDetails(SqlConnection con, int inquiryId)
        {
            const string sql = @"
        SELECT TOP 1
            cd.CompanyName,
            cd.TradingName,
            cod.Email   AS ContactEmail,
            cod.Phone   AS ContactPhone,
            cod.Fax     AS ContactFax,
            ld.AddressLine1,
            ld.AddressLine2,
            ld.City,
            ld.State,
            ld.Country,ci.CustomerMasterId,cod.PersonId,
            ld.Pincode, (Title + ' '+ FirstName + ' '+ LastName) as [ContactPerson]
        FROM [dbo].[CustomerInquiry] ci
        INNER JOIN [dbo].[CustomerMaster]  cm  ON ci.CustomerMasterId = cm.CustomerMasterId
        INNER JOIN [dbo].[CompanyDetails]  cd  ON cm.CustomerMasterId = cd.CustomerMasterId
        INNER JOIN [dbo].[ContactDetails]  cod ON cm.CustomerMasterId = cod.CustomerMasterId
        INNER JOIN [dbo].[ContactPersonDetails]  cpd ON cm.CustomerMasterId = cpd.CustomerMasterId
        INNER JOIN [dbo].[LocationDetails] ld  ON cm.CustomerMasterId = ld.CustomerMasterId
        WHERE ci.InquiryId = @InquiryId";

            var info = new CompanyContactInfo();

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return info; // no match - everything stays blank

                    string companyName = ColStr(rdr, "CompanyName");
                    string tradingName = ColStr(rdr, "TradingName");
                    info.CompanyName = string.IsNullOrWhiteSpace(companyName) ? tradingName : companyName;

                    info.Contactperson = ColStr(rdr, "ContactPerson");
                    info.Email = ColStr(rdr, "ContactEmail");
                    info.Phone = ColStr(rdr, "ContactPhone");
                    info.Fax = ColStr(rdr, "ContactFax");
                    info.CustomerMasterId = ColStr(rdr, "CustomerMasterId");
                    info.PersonId = ColStr(rdr, "PersonId");

                    var parts = new List<string>();
                    foreach (var col in new[] { "AddressLine1", "AddressLine2", "City", "State", "Country", "Pincode" })
                    {
                        string v = ColStr(rdr, col);
                        if (!string.IsNullOrWhiteSpace(v)) parts.Add(v.Trim());
                    }
                    info.Address = string.Join(", ", parts);
                }
            }

            return info;
        }

        private static string ColStr(SqlDataReader rdr, string column)
        {
            object v = rdr[column];
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }

        /// <summary>
        /// Parses the reference-documents summary written into the
        /// [AdditionalDetails].[ColorName_Code] column, e.g.
        /// "Yes: Lamp &amp; Pole Technical Data; List of Lamp Points to be Inspected"
        /// or "No reference documents provided".
        /// </summary>
        public static ReferenceDocsInfo ParseReferenceDocs(string blob)
        {
            var info = new ReferenceDocsInfo();
            if (string.IsNullOrWhiteSpace(blob) || blob.StartsWith("No reference documents"))
                return info; // Provided stays false, all flags default false

            info.Provided = true;
            const string prefix = "Yes:";
            string list = blob.StartsWith(prefix) ? blob.Substring(prefix.Length).Trim() : "";

            foreach (var raw in list.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string item = raw.Trim();
                if (item.Equals("Lamp & Pole Technical Data", StringComparison.OrdinalIgnoreCase))
                    info.LampPoleData = true;
                else if (item.Equals("List of Lamp Points to be Inspected", StringComparison.OrdinalIgnoreCase))
                    info.LampPointList = true;
                else if (item.Equals("Lamp Point Layout Drawing", StringComparison.OrdinalIgnoreCase))
                    info.LayoutDrawing = true;
                else if (item.Equals("Operational Commissioning Report", StringComparison.OrdinalIgnoreCase))
                    info.CommissioningReport = true;
                else if (item.StartsWith("Other:", StringComparison.OrdinalIgnoreCase))
                {
                    info.Other = true;
                    info.OtherText = item.Substring("Other:".Length).Trim();
                }
            }
            return info;
        }

        public class ReferenceDocsInfo
        {
            public bool Provided;
            public bool LampPoleData;
            public bool LampPointList;
            public bool LayoutDrawing;
            public bool CommissioningReport;
            public bool Other;
            public string OtherText = "";
        }

        public static ReferenceDocsSelection ParseReferenceSelection(string blob)
        {
            var info = new ReferenceDocsSelection();
            if (string.IsNullOrWhiteSpace(blob)) return info;

            string trimmed = blob.Trim();
            if (!trimmed.StartsWith("Yes", StringComparison.OrdinalIgnoreCase))
                return info; // Provided stays false

            info.Provided = true;
            string list = trimmed.StartsWith("Yes:", StringComparison.OrdinalIgnoreCase)
                ? trimmed.Substring("Yes:".Length).Trim()
                : "";

            foreach (var raw in list.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string item = raw.Trim();
                if (item.Length == 0) continue;

                if (item.StartsWith("Other:", StringComparison.OrdinalIgnoreCase))
                {
                    info.Other = true;
                    info.OtherText = item.Substring("Other:".Length).Trim();
                }
                else
                {
                    info.Selected.Add(item);
                }
            }
            return info;
        }

        public class ReferenceDocsSelection
        {
            public bool Provided;
            public HashSet<string> Selected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public bool Other;
            public string OtherText = "";

            public bool Has(string label) { return Selected.Contains(label); }
        }

        public static string RenderTemplate(string templatePath, Dictionary<string, string> map)
        {
            string html = System.IO.File.ReadAllText(templatePath);
            foreach (var kv in map)
                html = html.Replace("{" + kv.Key + "}", kv.Value ?? "");
            return html;
        }
    }
}
