using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LIMS
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string cs = System.Configuration.ConfigurationManager
                        .ConnectionStrings["CM"].ConnectionString;

        public int PRPending, PRApproved, PRRejected;
        public int POOpen, POClosed, POCancelled;
        public int ToolsReceived, ToolsAssigned, ToolsCalibrated, ToolsReturned;
        public class DashboardSummaryDto
        {
            public int PRPending { get; set; }
            public int PRApproved { get; set; }
            public int PRRejected { get; set; }

            public int POOpen { get; set; }
            public int POClosed { get; set; }
            public int POCancelled { get; set; }

            public int ToolsReceived { get; set; }
            public int ToolsAssigned { get; set; }
            public int ToolsCalibrated { get; set; }
            public int ToolsReturned { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPRStatus();
                LoadPOStatus();
                LoadCalibrationStatus();
                LoadCards();
            }
        }

        void LoadPRStatus()
        {
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
    CASE 
        WHEN ps.Status IN ('New', 'Submitted') THEN 'Pending'
        WHEN ps.Status IN ('Approved', 'Approved by GM','Approved by HOD','Approved by Stores','Mail Sent to Vendor ') THEN 'Approved'
        WHEN ps.Status IN ('Rejected', 'Rejected by GM','Rejected by HOD','Rejected by Stores') THEN 'Rejected'
        ELSE 'Other'
    END AS Status,
    COUNT(*) AS Total
FROM PurchaseRequisitionDetails p
INNER JOIN PurchaseRequisitionStatus ps 
    ON ps.StatusID = p.StatusID
GROUP BY
    CASE 
        WHEN ps.Status IN ('New', 'Submitted') THEN 'Pending'
        WHEN ps.Status IN ('Approved', 'Approved by GM','Approved by HOD','Approved by Stores','Mail Sent to Vendor ') THEN 'Approved'
        WHEN ps.Status IN ('Rejected', 'Rejected by GM','Rejected by HOD','Rejected by Stores') THEN 'Rejected'
        ELSE 'Other'
    END;", cs))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                PRPending = GetCount(dt, "Pending");
                PRApproved = GetCount(dt, "Approved");
                PRRejected = GetCount(dt, "Rejected");
            }
        }

        void LoadPOStatus()
        {
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"select 
	  CASE 
        WHEN ps.POStatus IN ('PO Generated', 'Amend PO') THEN 'Pending'
        WHEN ps.POStatus IN ('Approved', 'Item Stored') THEN 'Approved'
        WHEN ps.POStatus IN ('Rejected') THEN 'Rejected'
        ELSE 'Other'
    END AS Status,
	COUNT(*) AS Total
FROM PurchaseOrder p
INNER JOIN POStatus ps on p.PurchaseOrderStatus = ps.POStatusID
group by 
 CASE 
        WHEN ps.POStatus IN ('PO Generated', 'Amend PO') THEN 'Pending'
        WHEN ps.POStatus IN ('Approved', 'Item Stored') THEN 'Approved'
        WHEN ps.POStatus IN ('Rejected') THEN 'Rejected'
        ELSE 'Other'
END", cs))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                POOpen = GetCount(dt, "Pending");
                POClosed = GetCount(dt, "Approved");
                POCancelled = GetCount(dt, "Rejected");
            }
        }

        void LoadCalibrationStatus()
        {
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
 CASE 
        WHEN ps.CalibrationStatus IN ('Tools Received by CS','Tools Accepted by CS','SPK to Calibration Staff') THEN 'Received'
		WHEN ps.CalibrationStatus IN ('PO Generated', 'Amend PO') THEN 'Rejected'
		WHEN ps.CalibrationStatus IN ('Allocated for Calibration','Calibration Result Submitted','Re-Test') THEN 'Assigned'
        WHEN ps.CalibrationStatus IN ('Certificate Sent to CS','Approved By Calibtration  manager') THEN 'Calibrated'
        WHEN ps.CalibrationStatus IN ('Tools Rejected by CS') THEN 'Returned'
		WHEN ps.CalibrationStatus IN ('Tools Sent to SubContractor','SubContractor Result Submitted') THEN 'Sub Contracted'
        ELSE 'Other'
    END AS Status,
COUNT(*) Total FROM ToolVerification t
inner join CalibrationStatus ps on t.CalibrationStatusID=ps.CalibrationStatusID
GROUP BY CASE 
        WHEN ps.CalibrationStatus IN ('Tools Received by CS','Tools Accepted by CS','SPK to Calibration Staff') THEN 'Received'
		WHEN ps.CalibrationStatus IN ('PO Generated', 'Amend PO') THEN 'Rejected'
		WHEN ps.CalibrationStatus IN ('Allocated for Calibration','Calibration Result Submitted','Re-Test') THEN 'Assigned'
        WHEN ps.CalibrationStatus IN ('Certificate Sent to CS','Approved By Calibtration  manager') THEN 'Calibrated'
        WHEN ps.CalibrationStatus IN ('Tools Rejected by CS') THEN 'Returned'
		WHEN ps.CalibrationStatus IN ('Tools Sent to SubContractor','SubContractor Result Submitted') THEN 'Sub Contracted'
        ELSE 'Other'
    END ", cs))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ToolsReceived = GetCount(dt, "Received");
                ToolsAssigned = GetCount(dt, "Assigned");
                ToolsCalibrated = GetCount(dt, "Calibrated");
                ToolsReturned = GetCount(dt, "Returned");
            }
        }

        void LoadCards()
        {
            var cards = new List<dynamic>
            {
                new { Title="PR Pending", Count=PRPending },
                new { Title="PR Approved", Count=PRApproved },
                new { Title="PR Rejected", Count=PRRejected },
                new { Title="PO Open", Count=POOpen },
                new { Title="Tools Due", Count=ToolsAssigned },
                new { Title="Returned", Count=ToolsReturned }
            };

            rptCards.DataSource = cards;
            rptCards.DataBind();
        }

        int GetCount(DataTable dt, string status)
        {
            DataRow[] rows = dt.Select($"Status='{status}'");
            return rows.Length > 0 ? Convert.ToInt32(rows[0]["Total"]) : 0;
        }
    }
}
