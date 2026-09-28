using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ReportingHierarchy : System.Web.UI.Page
    {
        string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEmployees();
            }
        }


        private void LoadEmployees()
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT ID, Name
                    FROM orgtree
                    ORDER BY Name";

                using (SqlCommand cmd =
                       new SqlCommand(sql, con))
                {
                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        ddlEmployee.DataSource = dr;
                        ddlEmployee.DataTextField = "Name";
                        ddlEmployee.DataValueField = "ID";
                        ddlEmployee.DataBind();
                    }
                }
            }

            ddlEmployee.Items.Insert(
                0,
                new System.Web.UI.WebControls.ListItem(
                    "-- Select Employee --",
                    ""));
        }

        private void LoadApprovers(int employeeID)
        {
            ddlApprover.Items.Clear();

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT ID, Name
            FROM orgtree
            WHERE ID <> @EmployeeID
            ORDER BY Name";

                using (SqlCommand cmd =
                       new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(
                        "@EmployeeID",
                        SqlDbType.Int).Value = employeeID;

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        ddlApprover.DataSource = dr;
                        ddlApprover.DataTextField = "Name";
                        ddlApprover.DataValueField = "ID";
                        ddlApprover.DataBind();
                    }
                }
            }

            ddlApprover.Items.Insert(
                0,
                new System.Web.UI.WebControls.ListItem(
                    "-- Select Approver --",
                    ""));
        }
        protected void ddlEmployee_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
                return;

            int employeeID =
                Convert.ToInt32(ddlEmployee.SelectedValue);

            LoadApprovers(employeeID);

            LoadCurrentPID(employeeID);

            LoadHierarchy(employeeID);
        }
        private void LoadCurrentPID(int employeeID)
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT PID
            FROM orgtree
            WHERE ID = @EmployeeID";

                using (SqlCommand cmd =
                       new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(
                        "@EmployeeID",
                        SqlDbType.Int).Value = employeeID;

                    con.Open();

                    object result = cmd.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        string pid = result.ToString();

                        if (ddlApprover.Items.FindByValue(pid)
                            != null)
                        {
                            ddlApprover.SelectedValue = pid;
                        }
                    }
                }
            }
        }
        protected void btnSave_Click(
    object sender,
    EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                lblMessage.Text = "Please select an employee.";
                return;
            }

            if (string.IsNullOrEmpty(ddlApprover.SelectedValue))
            {
                lblMessage.Text =
                    "Please select the Level-1 approver.";

                return;
            }

            int employeeID =
                Convert.ToInt32(ddlEmployee.SelectedValue);

            int approverID =
                Convert.ToInt32(ddlApprover.SelectedValue);


            if (employeeID == approverID)
            {
                lblMessage.Text =
                    "An employee cannot report to himself/herself.";

                return;
            }


            if (CreatesCircularHierarchy(
                employeeID,
                approverID))
            {
                lblMessage.Text =
                    "Invalid hierarchy. The selected approver " +
                    "would create a circular reporting structure.";

                return;
            }


            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string sql = @"
            UPDATE orgtree
            SET PID = @ApproverID
            WHERE ID = @EmployeeID";

                using (SqlCommand cmd =
                       new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(
                        "@ApproverID",
                        SqlDbType.Int).Value = approverID;

                    cmd.Parameters.Add(
                        "@EmployeeID",
                        SqlDbType.Int).Value = employeeID;

                    con.Open();

                    cmd.ExecuteNonQuery();
                }
            }


            lblMessage.Text =
                "Reporting / approval hierarchy updated successfully.";

            LoadHierarchy(employeeID);
        }
        private bool CreatesCircularHierarchy(
    int employeeID,
    int newApproverID)
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string sql = @"
            WITH ReportingTree AS
            (
                SELECT
                    ID,
                    PID
                FROM orgtree
                WHERE ID = @NewApproverID

                UNION ALL

                SELECT
                    o.ID,
                    o.PID
                FROM orgtree o
                INNER JOIN ReportingTree r
                    ON o.ID = r.PID

                WHERE r.PID IS NOT NULL
            )

            SELECT COUNT(*)
            FROM ReportingTree
            WHERE ID = @EmployeeID

            OPTION (MAXRECURSION 100);";


                using (SqlCommand cmd =
                       new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(
                        "@NewApproverID",
                        SqlDbType.Int).Value = newApproverID;

                    cmd.Parameters.Add(
                        "@EmployeeID",
                        SqlDbType.Int).Value = employeeID;

                    con.Open();

                    int count =
                        Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }
        private void LoadHierarchy(int employeeID)
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string sql = @"
            WITH ApprovalHierarchy AS
            (
                SELECT
                    0 AS Level,
                    ID,
                    Name,
                    PID
                FROM orgtree
                WHERE ID = @EmployeeID

                UNION ALL

                SELECT
                    h.Level + 1,
                    o.ID,
                    o.Name,
                    o.PID
                FROM ApprovalHierarchy h
                INNER JOIN orgtree o
                    ON o.ID = h.PID  where o.ID not in(15112)
            )

            SELECT
                Level,
                ID,
                Name
            FROM ApprovalHierarchy

            ORDER BY Level

            OPTION (MAXRECURSION 100);";


                using (SqlCommand cmd =
                       new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(
                        "@EmployeeID",
                        SqlDbType.Int).Value = employeeID;


                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        gvHierarchy.DataSource = dt;
                        gvHierarchy.DataBind();
                    }
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {

        }
    }
    }