using LIMS.DAL;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;


namespace LIMS
{
    public partial class GeneratePurchaseRequisitionReport : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        mailer objEmail = new mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string prId = Request.QueryString["ID"];
                if (!string.IsNullOrEmpty(prId))
                {
                    GeneratePRReport(prId);
                }
            }
        }

        private void GeneratePRReport(string prId)
        {
            string PRNO = "", PRDate = "", PRType = "", RequestToDept = "", PRRequestTo = "";
            string RequestFromDept = "", PRRequestBy = "", PRFileName = "", DeptCode = "";
            string LocalOrImport = "";
            StringBuilder sb = new StringBuilder();
            StringBuilder sbApproval = new StringBuilder();

            DataTable dtPR = _cm.GetPurchaseRequisition(prId);

            if (dtPR.Rows.Count > 0)
            {
                sb.Append(@"
                    <table class='tab1'>
                        <tr>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 5%; text-align: center;'>No.</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 30%; text-align: center;'>Item Name</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>Qty</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>Required On</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>Purpose</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>Remarks</td>
                        </tr>");



                PRDate = Convert.ToDateTime(dtPR.Rows[0]["RequestDate"]).ToString("dd-MM-yyyy");
                PRType = dtPR.Rows[0]["StoreType"].ToString();
                RequestToDept = "Purchasing Department";
                PRRequestTo = "";
                RequestFromDept = dtPR.Rows[0]["Department"].ToString();
                PRRequestBy = dtPR.Rows[0]["RequestedBy"].ToString();
                PRFileName = dtPR.Rows[0]["ReportFileName"].ToString();
                DeptCode = dtPR.Rows[0]["DeptCode"].ToString();
                LocalOrImport = dtPR.Rows[0]["LocalOrImport"].ToString();
                PRNO = Convert.ToInt32(dtPR.Rows[0]["PRNumber"]).ToString("D4") + "/" + Convert.ToDateTime(dtPR.Rows[0]["RequestDate"]).ToString("MM") + "/" + Convert.ToDateTime(dtPR.Rows[0]["RequestDate"]).ToString("yyyy") + "/" + DeptCode;
                int count = 0;
                DataTable dt = _cm.GetPurchaseRequisitionDetails(prId);
                string ItemStatus = ""; string[] numbers = { "1010", "1012", "1014", "1016" };
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        count++;
                        ItemStatus = dr["StatusID1"].ToString();
                        bool containsAny = numbers.Any(n => ItemStatus.Contains(n));
                        if (containsAny)
                        {
                            sb.Append("<tr style='text-decoration: line-through;'>");
                        }
                        else
                        {
                            sb.Append("<tr>");
                        }
                        sb.Append($@"
                        <td style='text-align:center'>" + count + "</td>" +
                                  @"<td>" + dr["ItemName"].ToString() + "</td>" +
                                  @"<td style='text-align:center'>" + dr["Quantity"].ToString() + "</td>" +
                                  @"<td style='text-align:center'>" + Convert.ToDateTime(dr["RequiredOn"]).ToString("dd-MM-yyyy") + "</td>" +
                                  @"<td style='text-align:center'>" + dr["Purpose"].ToString() + "</td>" +
                                  @"<td style='text-align:center'>" + dr["Remarks"].ToString() + "</td>" +
                                  @"</tr>");

                    }
                    sb.Append("</table>");

                    string ApprovedDate = "";
                    string User = "", UserDept = "";
                    string Tier1 = "", Tier1Dept = "", Tier2 = "", Tier2Dept = "", Tier3 = "", Tier3Dept = "", Tier4 = "", Tier4Dept = "";
                    StringBuilder SignUser = new StringBuilder(), SignTier1 = new StringBuilder(), SignTier2 = new StringBuilder(), SignTier3 = new StringBuilder(), SignTier4 = new StringBuilder();

                    DataTable dtApproval = new DataTable();
                    string[] items = { "user", "Tier-1", "Tier-2", "Tier-3", "Tier-4" };
                    string PRStatus = "";
                    foreach (string item in items)
                    {
                        switch (item)
                        {
                            case "user":
                                PRStatus = "1";
                                dtApproval = _cm.GetPRHistory(prId, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    User = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    UserDept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignUser.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                                        SignUser.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                                        SignUser.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignUser.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                        SignUser.Append("</div></div>");
                                    }
                                }
                                break;
                            case "Tier-1":
                                PRStatus = "1009,1010";
                                dtApproval = _cm.GetPRHistory(prId, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier1 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier1Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier1.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                                        // SignTier1.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
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
                                dtApproval = _cm.GetPRHistory(prId, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier2 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier2Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    //if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier2.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                                        // SignTier2.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
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
                                PRStatus = "1013,1014";
                                dtApproval = _cm.GetPRHistory(prId, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    Tier3 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier3Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    // if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    {
                                        SignTier3.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                                        //SignTier3.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
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
                                dtApproval = _cm.GetPRHistory(prId, PRStatus);
                                if (dtApproval.Rows.Count > 0)
                                {
                                    int userId = Convert.ToInt32(dtApproval.Rows[0]["userId"]);
                                    Tier4 = dtApproval.Rows[0]["EmployeeName"].ToString();
                                    Tier4Dept = dtApproval.Rows[0]["DesignationName"].ToString();
                                    ApprovedDate = Convert.ToDateTime(dtApproval.Rows[0]["ApprovedDate"]).ToString("dd-MM-yyyy HH:mm");
                                    // if (dtApproval.Rows[0]["DigiSign"].ToString() != "")
                                    string signature = _cm.GetSignature(userId, "PR");

                                    if (!string.IsNullOrEmpty(signature))
                                    {
                                        SignTier4.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                                        SignTier4.Append("<div><img src='" + ResolveUrl(signature) + "' class='signature-image' /></div>");
                                        SignTier4.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                                        SignTier4.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + ApprovedDate + "</div>");
                                        SignTier4.Append("</div></div>");
                                    }
                                    else
                                    {
                                        SignTier4.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                                        //SignTier4.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
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
                    <tr><td height='100px'>" + SignTier1.ToString() + "</td><td>" + SignTier2.ToString() + "</td><td>" + SignTier3.ToString() + "</td><td>" + SignTier4.ToString() + "</td></tr><tr>" +

                   @"<td style='width:25%; text-align:center'><b>" + Tier1 + "</b> / " + Tier1Dept + "</td>" +
                   @"<td style='width:25%; text-align:center'><b>" + Tier2 + "</b> / " + Tier2Dept + "</td> " +
                   @"<td style='width:25%; text-align:center'><b>" + Tier3 + "</b> / " + Tier3Dept + "</td> " +
                   @"<td style='width:25%; text-align:center'><b>" + Tier4 + "</b> / " + Tier4Dept + "</td> " +
                   @"</tr>
                    </table>");

                    DataTable dtType = _cm.GetItemType();
                    List<string> allItems = dtType.AsEnumerable()
                      .Select(r => r["ItemType"].ToString())
                      .ToList();
                    List<string> selected = new List<string> { PRType };

                    string sbHtml = BuildCheckboxTable(allItems, selected);

                    List<string> LocalOrImportList = new List<string>() { "Lokal", "Import" };
                    List<string> LocalOrImportSelected = new List<string> { LocalOrImport };

                    string sbLocalOrImport = BuildLocalOrImport(LocalOrImportList, LocalOrImportSelected);

                    StringBuilder sbDocuments = new StringBuilder();

                    DataTable dtDocuments = _cm.GetPurchaseRequisitionDocuments(prId);

                    if (dtDocuments.Rows.Count > 0)
                    {
                        sbDocuments.Append("<br/><br/>");
                        sbDocuments.Append("<table width='100%'>");

                        sbDocuments.Append(@"
                                <tr>
                                    <th colspan='3' style='text-align:left;background:#efefef'>
                                        Supporting Documents
                                    </th>
                                </tr>");

                                                sbDocuments.Append(@"
                                <tr>
                                    <th width='5%'>#</th>
                                    <th width='30%'>Dsecription</th>
                                    <th>Document</th>
                                </tr>");

                                                int i = 1;

                                                foreach (DataRow dr in dtDocuments.Rows)
                                                {
                                                    string file = dr["FilePath"].ToString();

                                                    // convert physical path to relative url
                                                    file = file.Replace(Server.MapPath("~"), "")
                                                               .Replace("\\", "/");

                                                    sbDocuments.Append($@"
                                <tr>
                                    <td align='center'>{i++}</td>
                                    <td>{dr["Description"]}</td>
                                    <td>
                                        <a href='/{file}' target='_blank'>
                                            {dr["FileName"]}
                                        </a>
                                    </td>
                                </tr>");
                        }

                        sbDocuments.Append("</table>");
                    }

                    string body = objEmail.PrepareMailBodyWith("PurchaseRequisition.html",
                       "{PRNO}", PRNO,
                           "{PRDate}", PRDate,
                           "{PRType}", sbHtml.ToString(),
                           "{LocalOrImport}", sbLocalOrImport.ToString(),
                           "{RequestToDept}", RequestToDept,
                           "{PRRequestTo}", PRRequestTo,
                           "{RequestFromDept}", RequestFromDept,
                           "{PRRequestBy}", PRRequestBy,
                           "{PurchaseRequisition}", sb.ToString(),
                           "{Approval}", sbApproval.ToString(),
                           "{Attachments}", sbDocuments.ToString()
                       );

                    string folderPath = Server.MapPath("/PurchaseRequisition");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Write the file to the server
                    string filename = Server.MapPath("/PurchaseRequisition/" + PRFileName + ".htm");
                    System.IO.File.WriteAllText(filename, body);

                    // Read the generated HTML file and display it on the page
                    string html = File.ReadAllText(filename);
                    divPR.InnerHtml = html.ToString();
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
                string box = selectedItems.Contains(name)
                ? "<span style='font-size:22px;'>&#9632;</span>"   // big black square
                : "<span style='font-size:22px;'>&#9633;</span>";  // big empty square

                sb.Append($"<div style='font-size:12px'>{box} {name}</div>");
            }
            sb.Append("</td>");

            // RIGHT COLUMN ONLY IF MORE THAN 6
            if (total > 6)
            {
                sb.Append("<td style='vertical-align:top;'>");
                for (int i = midpoint; i < total; i++)
                {
                    string name = allItems[i];
                    string box = selectedItems.Contains(name)
                 ? "<span style='font-size:22px;'>&#9632;</span>"   // big black square
                 : "<span style='font-size:22px;'>&#9633;</span>";  // big empty square

                    sb.Append($"<div style='font-size:12px'>{box} {name}</div>");
                }
                sb.Append("</td>");
            }

            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

        public string BuildLocalOrImport(List<string> allItems, List<string> selectedItems)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table style='font-family:Arial; font-size:12px;'>");
            sb.Append("<tr>");


            // LEFT COLUMN
            sb.Append("<td style='vertical-align:top; padding-right:30px;'>");
            for (int i = 0; i < allItems.Count; i++)
            {
                string name = allItems[i];
                string box = selectedItems.Contains(name)
                ? "<span style='font-size:22px;margin-left: 5px;'>&#9632;</span>"   // big black square
                : "<span style='font-size:22px;margin-left: 5px;'>&#9633;</span>";  // big empty square

                // sb.Append($"<div style='font-size:12px'>{box} {name}</div>");
                sb.Append($"{box} {name}");
            }


            sb.Append("</td>");

            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }
        
    }
}