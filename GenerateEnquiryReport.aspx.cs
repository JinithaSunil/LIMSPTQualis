using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;

namespace LIMS
{
    public partial class GenerateEnquiryReport : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    // Check if EnquiryID is passed via QueryString first, then Session
                    string enquiryID = Request.QueryString["EnquiryID"];
                    if (string.IsNullOrEmpty(enquiryID))
                    {
                        if (Session["EnquiryID"] == null) { Session["EnquiryID"] = "15"; }
                        enquiryID = Session["EnquiryID"].ToString();
                    }
                    else
                    {
                        Session["EnquiryID"] = enquiryID;
                    }

                    if (!string.IsNullOrEmpty(enquiryID))
                    {
                        GenerateReport();
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void GenerateReport()
        {
            string EnquiryID = Session["EnquiryID"].ToString();
            StringBuilder html = new StringBuilder();

            // Add CSS styles at the beginning
            html.Append(@"
    <style>
        .enquiry-header { display: flex; justify-content: space-between; margin-bottom: 20px; }
        .header-left { flex: 1; }
        .header-right { flex: 0 0 200px; }
        .enquiry-details { margin-bottom: 20px; }
        .enquiry-table { width: 100%; border-collapse: collapse; margin-top: 20px; }
        .enquiry-table th, .enquiry-table td { 
            border: 1px solid #ddd; 
            padding: 8px; 
            text-align: center; 
        }
        .enquiry-table th { 
            background-color: #B94D8F; 
            color: white; 
            text-align: center;
        }
        .enquiry-table tr:nth-child(even) { background-color: #f2f2f2;  text-align: center;}
        .ok-button {
            margin-top: 20px;
            text-align: center;
        }
        .ok-button button {
            padding: 10px 40px;
            background-color: #B94D8F;
            color: white;
            border: none;
            cursor: pointer;
            font-size: 13px;
            width: 150px;
        }
        .ok-button button:hover {
            background-color: #9a3a6f;
        }
    </style>");

            DataTable dtEn = _cm.GetAllEnquiry(EnquiryID);
            //DataTable dt = RemoveDuplicateRows(dtEn, "EnquiryID");

            if (dtEn.Rows.Count > 0)
            {
                // Table Section
                html.Append("<div class='enquiry-details'>");
                html.Append("<table class='enquiry-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>PR#</th>");
                html.Append("<th>Item Description</th>");
                html.Append("<th>Quantity</th>");
                html.Append("<th>UOM</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                // FIXED: Remove duplicates based on ItemCode to show unique items
                //DataTable dtEnq = RemoveDuplicateRows(dtEn, "ItemCode");
                foreach (DataRow row in dtEn.Rows)
                {
                    string displayPRID = row["PRID"].ToString();
                    string PRID = row["PRID"].ToString();
                    string PRNo = _cm.GetPRNumber(Convert.ToInt32(PRID));
                    html.Append("<tr>");
                    html.Append($"<td><a href='GeneratePurchaseRequisitionReport.aspx?ID={PRID}' target='_blank'>{PRNo}</a></td>");
                    html.Append($"<td>{row["ItemName"]}</td>"); // Changed from ItemDescription to ItemName
                    html.Append($"<td>{row["Quantity"]}</td>");
                    html.Append($"<td>{row["UOMName"]}</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody>");
                html.Append("</table>");
                html.Append("</div>");

                // Vendor Information
                string vendorNames = GetVendorNames(EnquiryID);

                // Save and display the report
                string purchaseEmail = Session["Email"]?.ToString() ?? "jinitha.s@rni-solutions.com";
                string body = objEmail.PrepareMailBodyWith("FloatEnquiry.html",
                    "{EnquiryID}", EnquiryID,
                    "{EnquiryDate}", dtEn.Rows[0]["Created"].ToString(),
                    "{PurchaseSubGroupName}", dtEn.Rows[0]["PurchaseSubGroupName"].ToString(),
                    "{VendorName}", vendorNames,
                    "{Enquiry}", html.ToString(),
                    "{purchaseEmail}", purchaseEmail);

                // Create directory path
                string directoryPath = Server.MapPath("~/Enquiries");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save file
                string filename = Path.Combine(directoryPath, "Enq-" + EnquiryID + ".html");
                System.IO.File.WriteAllText(filename, body);

                divEnquiry.InnerHtml = body;

                // Update enquiry filename
                _cm.ReportFileName = "Enq-" + EnquiryID;
                _cm.EnquiryID = Convert.ToInt32(EnquiryID);
                _cm.UpdateEnquiryFileName(_cm);
            }
        }
        public bool GenerateIndividualReport(int enquiryID)
        {
            try
            {
                StringBuilder html = new StringBuilder();

                // Add CSS styles at the beginning
                html.Append(@"
        <style>
            .enquiry-header { display: flex; justify-content: space-between; margin-bottom: 20px; }
            .header-left { flex: 1; }
            .header-right { flex: 0 0 200px; }
            .enquiry-details { margin-bottom: 20px; }
            .enquiry-table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            .enquiry-table th, .enquiry-table td { 
                border: 1px solid #ddd; 
                padding: 8px; 
                text-align: center; 
            }
            .enquiry-table th { 
                background-color: #B94D8F; 
                color: white; 
                text-align: center;
            }
            .enquiry-table tr:nth-child(even) { background-color: #f2f2f2;  text-align: center;}
            .ok-button {
                margin-top: 20px;
                text-align: center;
            }
            .ok-button button {
                padding: 10px 40px;
                background-color: #B94D8F;
                color: white;
                border: none;
                cursor: pointer;
                font-size: 13px;
                width: 150px;
            }
            .ok-button button:hover {
                background-color: #9a3a6f;
            }
        </style>");

                DataTable dtEn = _cm.GetAllEnquiry(enquiryID.ToString());
                DataTable dt = dtEn;

                if (dt.Rows.Count > 0)
                {
                    html.Append("<div class='enquiry-details'>");
                    html.Append("<table class='enquiry-table'>");
                    html.Append("<thead>");
                    html.Append("<tr>");
                    html.Append("<th>PR#</th>");
                    html.Append("<th>Item Description</th>");
                    html.Append("<th>Quantity</th>");
                    html.Append("<th>UOM</th>");
                    html.Append("</tr>");
                    html.Append("</thead>");
                    html.Append("<tbody>");

                    // ✅ Remove duplicates only by ItemCode
                    DataTable dtEnq = RemoveDuplicateRows(dt, "ItemCode");

                    foreach (DataRow row in dtEnq.Rows)
                    {
                        string displayPRID = row["PRID"].ToString();
                        string PRID = row["PRID"].ToString();

                        html.Append("<tr>");
                        html.Append($"<td><a href='GeneratePurchaseRequisitionReport.aspx?ID={PRID}' target='_blank'>{displayPRID}</a></td>");
                        html.Append($"<td>{row["ItemName"]}</td>");
                        html.Append($"<td>{row["Quantity"]}</td>");
                        html.Append($"<td>{row["UOMName"]}</td>");
                        html.Append("</tr>");
                    }

                    html.Append("</tbody>");
                    html.Append("</table>");
                    html.Append("</div>");

                    // Vendor Information
                    string vendorNames = GetVendorNames(enquiryID.ToString());

             
                    // Save and display the report
                    string purchaseEmail = Session["Email"]?.ToString() ?? "jinitha.s@rni-solutions.com";
                    string body = objEmail.PrepareMailBodyWith("FloatEnquiry.html",
                        "{EnquiryID}", enquiryID.ToString(),
                        "{EnquiryDate}", dt.Rows[0]["Created"].ToString(),
                        "{PurchaseSubGroupName}", dt.Rows[0]["PurchaseSubGroupName"].ToString(),
                        "{VendorName}", vendorNames,
                        "{Enquiry}", html.ToString(),
                        "{purchaseEmail}", purchaseEmail);

                    // Create directory path
                    string directoryPath = Server.MapPath("~/Enquiries");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Save file
                    string filename = Path.Combine(directoryPath, "Enq-" + enquiryID + ".html");
                    System.IO.File.WriteAllText(filename, body);

                    // Update enquiry filename
                    _cm.ReportFileName = "Enq-" + enquiryID;
                    _cm.EnquiryID = enquiryID;
                    _cm.UpdateEnquiryFileName(_cm);

                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No data found for EnquiryID {enquiryID}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error generating report for EnquiryID {enquiryID}: {ex.Message}");
                return false;
            }
        }
        private string GetVendorNames(string EnquiryID)
        {
            DataTable dt1 = _cm.GetAllEnquiry(EnquiryID);

            if (dt1 == null || dt1.Rows.Count == 0)
                return string.Empty;

            // Use HashSet to store unique vendor names
            HashSet<string> uniqueVendorNames = new HashSet<string>();

            // Get VendorID from the first row only (all rows have the same VendorID)
            string vendorIDString = dt1.Rows[0]["VendorID"].ToString();

            if (!string.IsNullOrEmpty(vendorIDString))
            {
                // If vendorID contains comma-separated values, split them
                string[] vendorIDs = vendorIDString.Split(',');

                foreach (string vendorID in vendorIDs)
                {
                    if (string.IsNullOrEmpty(vendorID.Trim()))
                        continue;

                    DataTable dtVendor = _cm.GetVendor(vendorID.Trim());

                    // Check if dtVendor has rows before accessing them
                    if (dtVendor != null && dtVendor.Rows.Count > 0)
                    {
                        string vendorName = dtVendor.Rows[0]["VendorName"].ToString();
                        uniqueVendorNames.Add(vendorName); // HashSet automatically handles duplicates
                    }
                }
            }

            // Join unique vendor names with comma
            return string.Join(", ", uniqueVendorNames);
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
            catch (Exception)
            {
                return null;
            }
        }
    }
}