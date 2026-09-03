using DocumentFormat.OpenXml.Spreadsheet;
using LIMS.DAL;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;

namespace LIMS
{
    public partial class GeneratePaymentRequisition : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        mailer objEmail = new mailer();
        string PaymentRequisitionID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                string RequestToDept = "", PRRequestTo = "";
                string RequestFromDept = "",  PRFileName = "", DeptCode = "";
                
                RequestToDept = "Finance";
                PRRequestTo = "Elan";
                

                StringBuilder sbApproval = new StringBuilder();
                PaymentRequisitionID = Request.QueryString["ID"].ToString();
                PaymentRequisitionID = _cm.Decode(PaymentRequisitionID);
                Session["PaymentRequisitionID"] = PaymentRequisitionID;

                string PaymentRequisitionDate = "";
                string RequisitionType = "";
                string RequisitionNumber = "";
                string RequisitionBy = "";
                string Remarks = "";
                string CurrencyCode = "";
                string Document = "", DocumentName = "", url="";

                // Get the quotation details
                DataTable dtQD = _cm.GetPaymentRequisitionDetails(Convert.ToInt32(PaymentRequisitionID));
                if (dtQD.Rows.Count > 0)
                {
                    PaymentRequisitionDate = Convert.ToDateTime(dtQD.Rows[0]["RequisitionDate"]).ToString("dd-MM-yyyy");
                    RequisitionNumber = dtQD.Rows[0]["RequisitionNumber"].ToString();
                    RequisitionType = dtQD.Rows[0]["RequisitionSubType"].ToString();
                    Remarks = dtQD.Rows[0]["Remarks"].ToString();
                    CurrencyCode = dtQD.Rows[0]["CurrencyCode"].ToString();
                    if (Session["EmployeeID"] != null)
                    {
                        DataTable dtEmp = _cm.GetEmployeeDetails(Session["EmployeeID"].ToString());
                        if (dtEmp.Rows.Count > 0)
                        {
                            RequisitionBy = dtEmp.Rows[0]["EmployeeName"].ToString();
                            RequestFromDept = dtEmp.Rows[0]["EmployeeDepartmentName"].ToString();
                        }
                    }

                    string tableHtml = @"
                    <table class='tab1'>
                        <tr>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 5%; text-align: center;'><u>Nomor</u></td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 30%; text-align: center;'><u>Keterangan</u></td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 20%; text-align: center;'><u>Jumlah Permintaan</u></td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'><u>Tgl. Dibutuhkan</u></td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'><u>Biaya</u></td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'><u>Total Biaya</u></td>
                        </tr>";
                    int count = 0; decimal totalAmount=0;
                    // Loop through the DataTable rows and dynamically create table rows
                    string ItemStatus = ""; string[] numbers = { "1010", "1012", "1014", "1016" };
                    foreach (DataRow row in dtQD.Rows)
                    {
                        count += 1;
                        ItemStatus = row["StatusID"].ToString();
                        bool containsAny = numbers.Any(n => ItemStatus.Contains(n));
                        if (containsAny)
                        {
                            tableHtml+="<tr style='text-decoration: line-through;'>";
                        }
                        else
                        {
                            tableHtml += "<tr>";
                        }
                        // Add each row to the table
                        int qty = Convert.ToInt32(row["Quantity"].ToString());
                        decimal unitprice = Convert.ToDecimal(row["UnitPrice"].ToString());
                        decimal TotalPrice = Convert.ToDecimal(row["TotalPrice"].ToString());
                        totalAmount += TotalPrice;
                        string strUnitprice = _cm.ToIDR(unitprice);
                        string strTotalPrice = _cm.ToIDR(TotalPrice);
                        tableHtml += $@"
                                <td style='text-align:center'>" + count.ToString() + "</td>" +
                              @"<td style='text-align:center'>" + row["Itemdescription"].ToString() + "</td>" +
                              @"<td style='text-align:center'>" + qty .ToString() + "</td>" +
                              @"<td style='text-align:center'>" + Convert.ToDateTime(row["RequirementDate"]).ToString("dd-MM-yyyy") + "</td>" +
                              @"<td style='text-align:center'>" + strUnitprice + "</td>" +
                              @"<td style='text-align:center'>" + strTotalPrice + "</td></tr>";
                    }
                    string Totamt = _cm.ToIDR(totalAmount);
                    // Close the table tag
                    tableHtml += " </table>";
                    tableHtml += "<span style='float:right'><b>Total: "+ Totamt + "</b></span>";
                    string ApprovedDate = "";
                    string User = "", UserDept = "", Tier1 = "", Tier1Dept = "";
                    string Tier2 = "", Tier2Dept = "", Tier3 = "", Tier3Dept = "", Tier4 = "", Tier4Dept = "";
                    StringBuilder SignUser = new StringBuilder(), SignTier1 = new StringBuilder() , SignTier2 = new StringBuilder(), SignTier3 = new StringBuilder(), SignTier4 = new StringBuilder();

                    DataTable dtApproval = new DataTable();
                    string[] items = { "user", "Tier-1", "Tier-2", "Tier-3", "Tier-4" };
                    string PRStatus = "";
                    foreach (string item in items)
                    {
                        switch (item)
                        {
                            case "user":
                                PRStatus = "1";
                                dtApproval = _cm.GetPaymentRequisitionHistory(PaymentRequisitionID, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    User = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    UserDept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignUser.Append("<div style='width:150px;text-align:center;font-family:Times New Roman;'>");
                                        SignUser.Append("<div> <img src='assets/img/Signature/"+ dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                        SignUser.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignUser.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: "+ ApprovedDate + "</div>");
                                        SignUser.Append("</div></div>");
                                    }
                                }
                                break;
                            case "Tier-1":
                                PRStatus = "1009";
                                dtApproval = _cm.GetPaymentRequisitionHistory(PaymentRequisitionID, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier1 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier1Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier1.Append("<div style='width:150px;text-align:center;font-family:Times New Roman;'>");
                                        //SignTier1.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");

                                        SignTier1.Append("<div style='font-family:SignatureFont,cursive;font-size:20px;line-height:40px;height:40px;font-weight:bold'>");
                                        SignTier1.Append(Tier1);
                                        SignTier1.Append("</div>");

                                        SignTier1.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignTier1.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                        SignTier1.Append("</div></div>");
                                    }
                                }
                                break;
                            case "Tier-2":
                                PRStatus = "1011";
                                dtApproval = _cm.GetPaymentRequisitionHistory(PaymentRequisitionID, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier2 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier2Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier2.Append("<div style='width:150px;text-align:center;font-family:Times New Roman;'>");
                                        //  SignTier2.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                        SignTier2.Append("<div style='font-family:SignatureFont,cursive;font-size:20px;line-height:40px;height:40px;font-weight:bold'>");
                                        SignTier2.Append(Tier2);
                                        SignTier2.Append("</div>");

                                        SignTier2.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignTier2.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                        SignTier2.Append("</div></div>");
                                    }
                                }
                                break;
                            case "Tier-3":
                                PRStatus = "1013";
                                dtApproval = _cm.GetPaymentRequisitionHistory(PaymentRequisitionID, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier3 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier3Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                   // if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier3.Append("<div style='width:150px;text-align:center;font-family:Times New Roman;'>");
                                        //   SignTier3.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                        SignTier3.Append("<div style='font-family:SignatureFont,cursive;font-size:20px;line-height:40px;height:40px;font-weight:bold'>");
                                        SignTier3.Append(Tier3);
                                        SignTier3.Append("</div>");
                                        SignTier3.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignTier3.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                        SignTier3.Append("</div></div>");
                                    }
                                }
                                break;
                            case "Tier-4":
                                PRStatus = "1015";
                                dtApproval = _cm.GetPaymentRequisitionHistory(PaymentRequisitionID, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier4 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier4Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier4.Append("<div style='width:150px;text-align:center;font-family:Times New Roman;'>");
                                        //  SignTier4.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                        SignTier4.Append("<div style='font-family:SignatureFont,cursive;font-size:20px;line-height:40px;height:40px;font-weight:bold'>");
                                        SignTier4.Append(Tier4);
                                        SignTier4.Append("</div>");
                                        SignTier4.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignTier4.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                        SignTier4.Append("</div></div>");
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                    }

                    sbApproval.Append(@"<table class='tab1' width='100%' style='font-size:10px;'>
                    <tr>
                    <td style='width:25%; text-align:center'>Dibuat Oleh</td>
                    <td style='width:25%; text-align:center'>Mengetahui</td>
                    <td style='width:25%; text-align:center'>Disetujui Oleh</td>
                    <td style='width:25%; text-align:center'>Disetujui Oleh</td>
                    </tr>
                    <tr><td height='100px'>"+SignTier1.ToString()+"</td><td>"+SignTier2.ToString()+"</td><td>"+SignTier3.ToString()+"</td><td>"+SignTier4.ToString()+"</td></tr><tr>" +
                    
                   @"<td style='width:25%; text-align:center'><b>" + Tier1+ "</b> / " + Tier1Dept + "</td>"+
                   @"<td style='width:25%; text-align:center'><b>" + Tier2 + "</b> / " + Tier2Dept + "</td> " +
                   @"<td style='width:25%; text-align:center'><b>" + Tier3 + "</b> / " + Tier3Dept + "</td> " +
                   @"<td style='width:25%; text-align:center'><b>" + Tier4 + "</b> / " + Tier4Dept + "</td> " +
                   @"</tr>
                    </table>");
                    StringBuilder sb = new StringBuilder();
                    DataTable dt = _cm.GetRequisitionType();
                    List<string> allItems = dt.AsEnumerable()
                      .Select(r => r["RequisitionType"].ToString())
                      .ToList();
                    List<string> selected = new List<string> { RequisitionType };

                    string sbHtml = BuildCheckboxTable(allItems, selected);
                    ///Attach documents if available
                    DataTable dtFiles = _cm.GetPaymentRequisitionFileUpload(PaymentRequisitionID);

                    if (dtFiles.Rows.Count > 0)
                    {
                        StringBuilder sbAttachments = new StringBuilder();

                        sbAttachments.Append("<div style='margin-top:10px;'>");
                        sbAttachments.Append("<b>Attachments (" + dtFiles.Rows.Count + ")</b>");
                        sbAttachments.Append("<table width='100%' style='margin-top:5px;'>");
                        sbAttachments.Append("<tr>");
                        sbAttachments.Append("<th style='width:5%'>#</th>");
                        sbAttachments.Append("<th>Document Name</th>");
                        sbAttachments.Append("</tr>");

                        int i = 1;

                        foreach (DataRow dr in dtFiles.Rows)
                        {
                            string fileName = dr["FileName"].ToString();
                            string documentName = dr["DocumentName"].ToString();

                            sbAttachments.Append("<tr>");
                            sbAttachments.Append("<td style='text-align:center'>" + i + "</td>");
                            sbAttachments.Append("<td>");
                            sbAttachments.Append("<a href='UploadedFiles/PaymentRequisitionDocs/" +
                                                 fileName +
                                                 "' target='_blank'>📎 " +
                                                 documentName +
                                                 "</a>");
                            sbAttachments.Append("</td>");
                            sbAttachments.Append("</tr>");

                            i++;
                        }

                        sbAttachments.Append("</table>");
                        sbAttachments.Append("</div>");

                        Document = "Attachments";
                        url = sbAttachments.ToString();
                    }

                    // Prepare the email body with the dynamic table
                    string body = objEmail.PrepareMailBodyWith("PaymentRequisition.html",
                    "{RequisitionDate}", PaymentRequisitionDate,
                        "{RequisitionNumber}", RequisitionNumber,
                        "{RequisitionType}", sbHtml.ToString(),
                        "{RequestToDept}", RequestToDept,
                        "{PRRequestTo}", PRRequestTo,
                        "{RequestFromDept}", RequestFromDept,
                        "{RequisitionBy}", RequisitionBy,
                        "{Remarks}", Remarks,
                        "{Currency}", CurrencyCode,
                        "{Document}", Document,
                        "{DocumentName}", url,
                        "{PaymentRequisition}", tableHtml,
                        "{Approval}", sbApproval.ToString()

                    ); ;

                    string folderPath = Server.MapPath("/PaymentRequisition");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Write the file to the server
                    string filename = Server.MapPath("/PaymentRequisition/" + PaymentRequisitionID + ".htm");
                    System.IO.File.WriteAllText(filename, body);

                    // Read the generated HTML file and display it on the page
                    string html = File.ReadAllText(filename);
                    divRequest.InnerHtml = html;
                }
            }
        }

        public string BuildCheckboxTable(List<string> allItems, List<string> selectedItems)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table style='font-family:Arial; font-size:12px;'>");
            sb.Append("<tr>");

            int total = allItems.Count;
            int midpoint = (total > 6) ? (int)Math.Ceiling(total / 2.0) : total;

            // LEFT COLUMN
            sb.Append("<td style='vertical-align:top; padding-right:30px;'>");
            for (int i = 0; i < midpoint; i++)
            {
                string name = allItems[i];
                string box = selectedItems.Contains(name) ? "&#9632;" : "&#9633;"; // ■ or □
                sb.Append($"<div>{box} {name}</div>");
            }
            sb.Append("</td>");

            // RIGHT COLUMN ONLY IF MORE THAN 6
            if (total > 6)
            {
                sb.Append("<td style='vertical-align:top;'>");
                for (int i = midpoint; i < total; i++)
                {
                    string name = allItems[i];
                    string box = selectedItems.Contains(name) ? "&#9632;" : "&#9633;";
                    sb.Append($"<div>{box} {name}</div>");
                }
                sb.Append("</td>");
            }

            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}
