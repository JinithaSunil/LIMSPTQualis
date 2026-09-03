using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;
using LIMS.DAL;

namespace LIMS
{
    public partial class PRListFinal : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        private string currentDepartment;
        private string currentUserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Assuming UserID is stored in session after login
            currentUserID = Session["UserID"]?.ToString();

            if (!IsPostBack && !string.IsNullOrEmpty(currentUserID))
            {
                DataTable userDetails = _cm.GetEmpDetails(currentUserID);
                if (userDetails != null && userDetails.Rows.Count > 0)
                {
                    // Store department in ViewState
                    ViewState["CurrentDepartment"] = userDetails.Rows[0]["DepartmentID"]?.ToString() ?? string.Empty;
                    currentDepartment = ViewState["CurrentDepartment"].ToString();

                    if (Session["RoleID"] != null)
                    {
                        BindPR();
                    }
                }
                else
                {
                    // Handle case where user details cannot be retrieved
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Detail Not Found');", true);
                }
            }
            else
            {
                // Retrieve department from ViewState on postback
                currentDepartment = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;
            }
        }
        protected void BindPR()
        {
            string RoleID = Session["RoleID"].ToString();
            string Role = "Director";
            hdCurrentApprover.Value = Role;
            BindGrid(Role, 4);
        }



        private void BindGrid(string Role, int CurrentTier)
        {
            // Hide GM GridView and show Requests GridView
            gvRequests.Visible = true;


            string searchText = TextBox1.Text.Trim();

            string query = "";
            if (radChoice.SelectedValue == "0")
            {
                query = @"
                            SELECT distinct
                            RIGHT('0000' + CAST(PRNumber AS VARCHAR(4)), 4) + '/' + RIGHT('0' + CAST(MONTH(RequestDate) AS VARCHAR(2)), 2) + '/' + cast(year(RequestDate) as varchar) + '/' + deptcode AS PR,
                            pr.PRID AS PRID,
                            e.EmployeeName,
                            d.EmployeeDepartmentName,
                            pr.RequestDate,
	                        ps.Status,
                            pr.StoreType
                        FROM PurchaseRequisition pr
				   inner join PurchaseRequisitionDetails pd on pr.PRID = pd.prid
                   LEFT JOIN Employees e ON e.ID = pr.RequestBy
                   inner join PurchaseRequisitionStatus ps on ps.StatusID = pd.StatusID
                   inner JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                    and pr.CurrentTier=" + CurrentTier + " and CurrentApproverRole='" + Role + "'" +
                   " and pd.StatusID not in(1010,1012,1014,1015,1016)";
            }
            else
            {
                query = @"SELECT distinct
                                  RIGHT('0000' + CAST(PRNumber AS VARCHAR(4)), 4) + '/' + RIGHT('0' + CAST(MONTH(RequestDate) AS VARCHAR(2)), 2) + '/' + cast(year(RequestDate) as varchar) + '/' + deptcode AS PR,
                                  pr.PRID AS PRID,
                                  e.EmployeeName,
                                  d.EmployeeDepartmentName,
                                  pr.RequestDate,
                                  ps.Status,
                                  pr.StoreType
                              FROM PurchaseRequisition pr
                         inner join PurchaseRequisitionDetails pd on pr.PRID = pd.prid
                         LEFT JOIN Employees e ON e.ID = pr.RequestBy
                         inner join PurchaseRequisitionStatus ps on ps.StatusID = pd.StatusID
                         inner JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                         and pd.StatusID in(1015,1016)";
            }
            // Add search condition if search text is provided

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                if (radChoice.SelectedValue == "0")
                                {
                                    gvRequests.Visible = true;
                                    gvCompleted.Visible = false;

                                    gvRequests.DataSource = dt;
                                    gvRequests.DataBind();
                                }
                                else
                                {
                                    gvRequests.Visible = false;
                                    gvCompleted.Visible = true;

                                    gvCompleted.DataSource = dt;
                                    gvCompleted.DataBind();
                                }
                            }
                            else
                            {
                                gvRequests.DataSource = null;
                                gvRequests.DataBind();
                                gvCompleted.DataSource = null;
                                gvCompleted.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            Button btnChangeStatus = (Button)sender;

            // Find the GridView row that the button belongs to
            GridViewRow row = (GridViewRow)btnChangeStatus.NamingContainer;

            // Find the DropDownList in the same row as the button
            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");

            // Get the PRID from the row (assuming it's in the first column, adjust if needed)
            string prID = row.Cells[1].Text; // Ensure this is the correct index for your PRID column
            Response.Redirect("PurchaseRequisitionListHOD.aspx?p=" + prID);
        }
        protected void gvRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                // Set the new page index
                gvRequests.PageIndex = e.NewPageIndex;

                // Get the department ID from ViewState (already stored in Page_Load)
                string departmentID = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Paging Error: {ex.Message}');", true);
            }
        }

        protected void radChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radChoice.SelectedValue == "0")
            {
                gvRequests.Visible = true;
                gvCompleted.Visible = false;
                BindPR();
            }
            else
            {
                gvRequests.Visible = false;
                gvCompleted.Visible = true;
                BindPR();
            }

        }

        protected void gvRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve" || e.CommandName == "Reject")
            {
                int prId = Convert.ToInt32(e.CommandArgument);
                string status = e.CommandName == "Approve" ? "1015" : "1016";
                string StatusName = e.CommandName == "Approve" ? "Approved" : "Rejected";

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                {
                    string updateQuery = @"
                UPDATE PurchaseRequisitionDetails 
                SET StatusID = @StatusID 
                WHERE PRID = @PRID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PRID", prId);
                        cmd.Parameters.AddWithValue("@StatusID", status);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                var User = new DAL.User();
                User.EmployeeID = Session["EmployeeID"]?.ToString();
                User.DepartmentID = Session["DepartmentID"]?.ToString();
                User.UserID = Session["UserID"]?.ToString();
                User.RoleID = Session["RoleID"]?.ToString();
                User.Remarks = hdCurrentApprover.Value + "-" + StatusName;
                User.PRStatus = status;
                DataTable dtPRDetails = _cm.GetPRDetailsID(prId.ToString());
                if (dtPRDetails.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPRDetails.Rows)
                    {
                        User.PurchaseRequisitionDetailsID = dr["PurchaseRequisitionID"].ToString();
                        _cm.AddPRHistory(User);
                    }
                }
                BindPR(); // Rebind grid
            }
        }
    }
}