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
	public partial class PRList : System.Web.UI.Page
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
            string Role = "";

            switch (RoleID)
            {
                case "2":
                case "9":
                case "16":
                    Role = "Manager";
                    BindGrid(currentDepartment);
                    break;
                case "22":
                    Role = "IT Manager";
                    BindGrid(Role, 2);
                    break;
                case "23":
                    Role = "QMS Manager";
                    BindGrid(Role, 2);
                    break;
                case "24":
                    Role = "HCGA Manager";
                    BindGrid(Role, 2);
                    break;
                case "25":
                    Role = "Maintenance Manager";
                    BindGrid(Role, 2);
                    break;
                case "33":
                    Role = "Facility Manager";
                    BindGrid(Role, 2);
                    break;
                case "26":
                    Role = "Director[Sales Marketing]";
                    BindGrid(Role, 3);
                    break;
                case "27":
                    Role = "Director[Lab]";
                    BindGrid(Role, 3);
                    break;
                case "28":
                    Role = "Director";
                    BindGrid(Role, 4);
                    break;               
                default:
                    break;

            }
        }

        protected void BindAllPR()
        {            
            if (Session["EmployeeID"] != null)
            {
                string query = @"SELECT 
                            RIGHT('0000' + CAST(PRNumber AS VARCHAR(4)), 4) + '/' + RIGHT('0' + CAST(MONTH(RequestDate) AS VARCHAR(2)), 2) + '/' + cast(year(RequestDate) as varchar) + '/' + deptcode AS PR,
                            pr.PRID AS PRID,
                            Upper(e.EmployeeName) EmployeeName,
                            d.EmployeeDepartmentName,
                            pr.RequestDate,
                            ps.Status,
                            pr.StoreType
                        FROM PurchaseRequisition pr
                        LEFT JOIN Employees e ON e.ID = pr.RequestBy
                        inner join PurchaseRequisitionStatus ps on ps.StatusID = pr.StatusID
                        inner JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                        where pr.PRID in(
                        select distinct PRID from PRHistory ph
                        inner join PurchaseRequisitionDetails pd on ph.PurchaseRequisitionDetailsID = pd.PurchaseRequisitionID
                        where ph.employeeid=@EmployeeID and RoleID=@RoleID)";

                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", Session["EmployeeID"].ToString());
                            cmd.Parameters.AddWithValue("@RoleID", Session["RoleID"].ToString());
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                sda.Fill(dt);
                                gvCompleted.DataSource = dt;
                                gvCompleted.DataBind();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                }
            }
        }
        private void BindGrid(string departmentID)
        {
            // Hide GM GridView and show Requests GridView
            gvRequests.Visible = true;

            if (string.IsNullOrEmpty(departmentID))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Department not specified.');", true);
                return;
            }

            string searchText = TextBox1.Text.Trim();

            string query = @"
                            SELECT distinct 
                            RIGHT('0000' + CAST(PRNumber AS VARCHAR(4)), 4) + '/' + RIGHT('0' + CAST(MONTH(RequestDate) AS VARCHAR(2)), 2) + '/' + cast(year(RequestDate) as varchar) + '/' + deptcode AS PR,
                            pr.PRID AS PRID,
                            UPPER(e.EmployeeName)EmployeeName,
                            d.EmployeeDepartmentName,
                            pr.RequestDate,
	                        ps.Status,
                            pr.StoreType
                        FROM PurchaseRequisition pr
				   inner join PurchaseRequisitionDetails pd on pr.PRID = pd.prid
                   LEFT JOIN Employees e ON e.ID = pr.RequestBy
                   inner join PurchaseRequisitionStatus ps on ps.StatusID = pd.StatusID
                   inner JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                    AND pr.DepartmentID = @DepartmentID and pr.CurrentTier=1 
                    and pd.StatusID not in(1010,1012,1014,1016)";

            // Add search condition if search text is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                query += " AND pr.PRID LIKE @SearchText";
            }

            query += " ORDER BY pr.PRID DESC";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                        // Add search parameter if search text is provided
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                        }



                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                gvRequests.DataSource = dt;
                                gvRequests.DataBind();
                            }
                            else
                            {
                                gvRequests.DataSource = null;
                                gvRequests.DataBind();
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
        private void BindGrid(string Role, int CurrentTier)
        {
            // Hide GM GridView and show Requests GridView
            gvRequests.Visible = true;


            string searchText = TextBox1.Text.Trim();

            string query = @"
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
                    and pr.CurrentTier=" + CurrentTier + " and CurrentApproverRole='"+ Role +"'" +
                  " and pd.StatusID not in(1010,1012,1014,1015,1016)";

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
                            gvRequests.DataSource = dt;
                            gvRequests.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            // Get the current department from ViewState
            currentDepartment = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(currentDepartment))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Department not found. Please refresh the page.');", true);
                return;
            }

            // Bind the appropriate grid based on department
            switch (currentDepartment.ToUpper())
            {
                case "2009": // Stores
                case "2008": // Purchase
                    BindGrid(currentDepartment);
                    break;

                case "3008": // GM
                    break;

                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error: Invalid Department');", true);
                    break;
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

                // Rebind the grid with the same data
                BindGrid(departmentID);
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
                BindAllPR();
            }
        }
    }
}