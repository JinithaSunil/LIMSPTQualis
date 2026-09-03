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
	public partial class UserManagement : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM();
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                LoadReportsTo();
                LoadDepartments();
                LoadDesignations();
                LoadRoles();
            }

        }
        private void LoadDepartments()
        {
            DataTable dt = _cm.GetAllEmployeeDepartments();
            if (dt.Rows.Count > 0)
            {
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "EmployeeDepartmentName";
                ddlDepartment.DataValueField = "EmployeeDepartmentID";
                ddlDepartment.DataBind();
            }
        }
        private void LoadDesignations()
        {
            DataTable dt = _cm.GetAllDesignations();
            if (dt.Rows.Count > 0)
            {
                ddlDesignation.DataSource = dt;
                ddlDesignation.DataTextField = "DesignationName";
                ddlDesignation.DataValueField = "DesignationID";
                ddlDesignation.DataBind();
            }
        }
        private void LoadRoles()
        {
            DataTable dt = _cm.getAllRoles();
            if (dt.Rows.Count > 0)
            {
                ddlRole.DataSource = dt;
                ddlRole.DataTextField = "RoleName";
                ddlRole.DataValueField = "RoleID";
                ddlRole.DataBind();
            }
        }
        public DataTable GetAllOrg()
        {
            DataTable dt = new DataTable();
            string sQuery = @"SELECT ID, Name FROM OrgTree WHERE OrganizationID=1";
            dt = _cm.GetDataUsingQuery(sQuery);
            return dt;
        }
        private void LoadReportsTo()
        {
            DataTable dt = GetAllOrg();
            if (dt.Rows.Count > 0)
            {
                ddlReportsTo.DataSource = dt;
                ddlReportsTo.DataTextField = "Name";
                ddlReportsTo.DataValueField = "ID";
                ddlReportsTo.DataBind();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1️⃣ Insert into OrgTree
                    SqlCommand cmdOrg = new SqlCommand(@"
                INSERT INTO OrgTree (Name, PID, OrganizationID)
                VALUES (@Name, @PID, 1);
                SELECT SCOPE_IDENTITY();", con, tran);

                    cmdOrg.Parameters.AddWithValue("@Name", txtName.Text);
                    cmdOrg.Parameters.AddWithValue("@PID", ddlReportsTo.SelectedValue);

                    int orgID = Convert.ToInt32(cmdOrg.ExecuteScalar());

                    // 2️⃣ Insert into Employees
                    SqlCommand cmdEmp = new SqlCommand(@"
                INSERT INTO Employees
                (EmployeeID, DepartmentID, DesignationID, Email, Mobile, OrganizationID)
                VALUES
                (@EmployeeID, @DepartmentID, @DesignationID, @Email, @Mobile, 1);",
                        con, tran);

                    cmdEmp.Parameters.AddWithValue("@EmployeeID", orgID);
                    cmdEmp.Parameters.AddWithValue("@DepartmentID", ddlDepartment.SelectedValue);
                    cmdEmp.Parameters.AddWithValue("@DesignationID", ddlDesignation.SelectedValue);
                    cmdEmp.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmdEmp.Parameters.AddWithValue("@Mobile", txtMobile.Text);

                    cmdEmp.ExecuteNonQuery();

                    // 3️⃣ Insert into Users
                    SqlCommand cmdUser = new SqlCommand(@"
                INSERT INTO Users
                (UserName, Password, employeeID, RoleID, status, deleted, OrganizationID)
                VALUES
                (@UserName, @Password, @EmployeeID, @RoleID, 1, 0, 1);",
                        con, tran);

                    cmdUser.Parameters.AddWithValue("@UserName", txtUsername.Text);
                    cmdUser.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmdUser.Parameters.AddWithValue("@EmployeeID", orgID);
                    cmdUser.Parameters.AddWithValue("@RoleID", ddlRole.SelectedValue);

                    cmdUser.ExecuteNonQuery();

                    tran.Commit();

                    ScriptManager.RegisterStartupScript(this, GetType(), "msg",
                        "alert('User Added Successfully');", true);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ScriptManager.RegisterStartupScript(this, GetType(), "msg",
                        "alert('Error: " + ex.Message + "');", true);
                }
            }
        }
    }
}