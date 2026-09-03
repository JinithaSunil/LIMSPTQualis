using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class StoreEnquiries : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvAllEnquiries.Visible = false;

                BindPurchaseGroup();
                BindGrid();
                BindAllGrid();
            }
        }
        private void BindGrid()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    string baseQuery = @"SELECT
                        pd.PurchaseRequisitionID AS PR,
                        pr.PRID AS PRID,
                        d.EmployeeDepartmentName as 'Department',
                        pg.PurchaseGroupName as 'Group',
                        psg.PurchaseSubGroupName as 'Sub Group',
                        ItemDescription as 'Item',
                        pr.ReportFileName as 'PR#',
                        pd.purpose as 'Purpose',
                        u.UOMName as 'UOM',
                        pd.Stock as 'Stock',
                        ps.Status as 'Status'
                        FROM PurchaseRequisition pr
                        LEFT JOIN PurchaseRequisitionDetails pd ON pd.PRID = pr.PRID
                        LEFT JOIN Items i ON i.ItemCode = pd.ItemCode
                        LEFT JOIN UOM u ON u.UOMID = i.UOMID
                        LEFT JOIN PurchaseGroup pg ON pg.PurchaseGroupID = i.PurchaseGroupID
						LEFT JOIN PurchaseSubGroup psg ON psg.PurchaseSubGroupID = i.PurchaseSubGroupID
                        LEFT JOIN PurchaseRequisitionStatus ps ON ps.StatusID = pd.StatusID
                        LEFT JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
						where ps.StatusID in (6)";

                    // Add PRID filter if textbox has a valid numeric value
                    if (!string.IsNullOrEmpty(txtPRID.Text.Trim()))
                    {
                        int parsedPRID;
                        if (int.TryParse(txtPRID.Text.Trim(), out parsedPRID))  // Validate that PRID is numeric
                        {
                            baseQuery += " AND pr.PRID = @PRID";
                            cmd.Parameters.AddWithValue("@PRID", parsedPRID);
                        }
                        else
                        {
                            // Handle the case where PRID is not a valid integer, e.g., show a message or ignore the filter
                            // Example:
                            // ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", "alert('Invalid PRID.');", true);
                        }
                    }

                    // Add Purchase Group filter if applicable and valid numeric value is selected
                    if (!chkAll.Checked && ddlPurchaseGroup.SelectedValue != "0")
                    {
                        int parsedPurchaseGroupID;
                        if (int.TryParse(ddlPurchaseGroup.SelectedValue, out parsedPurchaseGroupID))  // Validate PurchaseGroupID is numeric
                        {
                            baseQuery += " AND pg.PurchaseGroupID = @PurchaseGroupID";
                            cmd.Parameters.AddWithValue("@PurchaseGroupID", parsedPurchaseGroupID);
                        }
                        else
                        {
                            // Handle the case where Purchase Group ID is not valid
                        }
                    }
                    baseQuery += " ORDER BY pr.PRID DESC";
                    cmd.CommandText = baseQuery;
                    try
                    {
                        conn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            // Add Serial Number column
                            dt.Columns.Add("SrNo", typeof(int));
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dt.Rows[i]["SrNo"] = i + 1 + (gvEnquiry.PageSize * gvEnquiry.PageIndex);
                            }
                            gvEnquiry.DataSource = dt;
                            gvEnquiry.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(),
                            "ErrorAlert",
                            "alert('Error loading data: " + ex.Message.Replace("'", "\\'") + "');",
                            true);
                    }
                }
            }
        }
        private void BindPurchaseGroup()
        {
            System.Data.DataTable dt = _cm.FillParentTable();
            if (dt.Rows.Count > 0)
            {
                ddlPurchaseGroup.DataSource = dt;
                ddlPurchaseGroup.DataTextField = "PurchaseGroupName";
                ddlPurchaseGroup.DataValueField = "PurchaseGroupID";
                ddlPurchaseGroup.DataBind();
                ddlPurchaseGroup.Items.Insert(0, new ListItem("Select Purchase Group", "NA"));
            }
        }
        private void BindAllGrid()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    string baseQuery = @"SELECT
                    d.EmployeeDepartmentName as 'Department',
                    pg.PurchaseGroupName as 'Group',
                    psg.PurchaseSubGroupName as 'Sub Group',
                    ItemDescription as 'Item',
                    pr.ReportFileName as 'PR#',
                    pd.purpose as 'Purpose',
                    u.UOMName as 'UOM',
                    pd.Stock as 'Stock',
                    ps.Status as 'Status'
                    FROM PurchaseRequisition pr
                    LEFT JOIN PurchaseRequisitionDetails pd ON pd.PRID = pr.PRID
                    LEFT JOIN Items i ON i.ItemCode = pd.ItemCode
                    LEFT JOIN UOM u ON u.UOMID = i.UOMID
                    LEFT JOIN PurchaseGroup pg ON pg.PurchaseGroupID = i.PurchaseGroupID
					LEFT JOIN PurchaseSubGroup psg ON psg.PurchaseSubGroupID = i.PurchaseSubGroupID
                    LEFT JOIN PurchaseRequisitionStatus ps ON ps.StatusID = pd.StatusID
                    LEFT JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                    ORDER BY pr.ReportFileName DESC";

                    cmd.CommandText = baseQuery;

                    try
                    {
                        conn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Add Serial Number column
                            dt.Columns.Add("SrNo", typeof(int));
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dt.Rows[i]["SrNo"] = i + 1 + (gvAllEnquiries.PageSize * gvAllEnquiries.PageIndex);
                            }

                            gvAllEnquiries.DataSource = dt;
                            gvAllEnquiries.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(),
                            "ErrorAlert",
                            "alert('Error loading data: " + ex.Message.Replace("'", "\\'") + "');",
                            true);
                    }
                }
            }
        }
        protected void ddlPurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPurchaseGroup.SelectedValue != "NA")
            {
                gvEnquiry.Visible = true;
                gvAllEnquiries.Visible = false;
                chkAll.Checked = false;
                gvEnquiry.PageIndex = 0;
                BindGrid();
            }
            else
            {
                gvEnquiry.Visible = false;
            }
        }
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.Checked)
            {
                gvAllEnquiries.Visible = true;
                gvEnquiry.Visible = false;
                ddlPurchaseGroup.SelectedValue = "NA";
                gvAllEnquiries.PageIndex = 0;
                BindAllGrid();
            }
            else
            {
                gvAllEnquiries.Visible = false;
            }
        }
        protected void txtPRID_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPRID.Text.Trim()))
            {
                gvEnquiry.Visible = true;
                gvAllEnquiries.Visible = false;
                chkAll.Checked = false;
                BindGrid();
            }
        }
        protected void gvEnquiry_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gridView = (GridView)sender;
            gridView.PageIndex = e.NewPageIndex;

            if (gridView.ID == "gvEnquiry")
            {
                BindGrid();
            }
            else if (gridView.ID == "gvAllEnquiries")
            {
                BindAllGrid();
            }
        }
        protected void gvEnquiry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                ddlStatus.SelectedValue = currentStatus;
            }
        }
        protected void btnChangeStatus_Click(object sender, EventArgs e)
        {
            Button btnChangeStatus = (Button)sender;
            GridViewRow row = (GridViewRow)btnChangeStatus.NamingContainer;
            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
            string PurchaseRequisitionID = row.Cells[1].Text;
            string newStatusID = ddlStatus.SelectedValue;
            string StatusName = ddlStatus.SelectedItem.ToString();

            if (string.IsNullOrEmpty(newStatusID))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a status.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    string updateQuery = @"
                UPDATE PurchaseRequisitionDetails 
                SET StatusID = @StatusID 
                WHERE PurchaseRequisitionID = @PurchaseRequisitionID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@StatusID", newStatusID);
                        cmd.Parameters.AddWithValue("@PurchaseRequisitionID", PurchaseRequisitionID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                var User = new DAL.User();
                User.EmployeeID = Session["EmployeeID"]?.ToString();
                User.DepartmentID = Session["DepartmentID"]?.ToString();
                User.UserID = Session["UserID"]?.ToString();
                User.RoleID = Session["RoleID"]?.ToString();
                User.Remarks = "Store - " + StatusName;
                User.PRStatus = newStatusID.ToString();
                User.PurchaseRequisitionDetailsID = PurchaseRequisitionID.ToString();
                _cm.AddPRHistory(User);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Status updated successfully.');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "location.reload();", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error updating status: {ex.Message}');", true);
            }
        }

    }
}