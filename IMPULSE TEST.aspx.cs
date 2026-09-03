using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using iTextSharp.tool.xml.html.table;

namespace LIMS
{
    public partial class IMPULSE_TEST : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the GridView with an empty DataTable
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn("ItemTest", typeof(string)),
                    new DataColumn("Unit", typeof(string)),
                    new DataColumn("Specified", typeof(string)),
                    new DataColumn("Measured", typeof(string)),
                    new DataColumn("Remark", typeof(string))
                });
                ViewState["dt"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            // Get the DataTable from ViewState
            DataTable dt = (DataTable)ViewState["dt"];

            // Add rows for each input field
            AddRowToDataTable(dt, "Impulse test at Withstand Voltage", txtUnit_Imp.Text, txtSpecified_Imp.Text, txtMeasured_Imp.Text, txtRemark_Imp.Text);
            AddRowToDataTable(dt, "Peak Voltage", txtUnit_Peak.Text, txtSpecified_Peak.Text, txtMeasured_Peak.Text, txtRemark_Peak.Text);
            AddRowToDataTable(dt, "Front wave time", txtUnit_Front.Text, txtSpecified_Front.Text, txtMeasured_Front.Text, txtRemark_Front.Text);
            AddRowToDataTable(dt, "Tail wave time", txtUnit_Tail.Text, txtSpecified_Tail.Text, txtMeasured_Tail.Text, txtRemark_Tail.Text);
            AddRowToDataTable(dt, "Conductor temperature", txtUnit_Con.Text, txtSpecified_Con.Text, txtMeasured_Con.Text, txtRemark_Con.Text);
            AddRowToDataTable(dt, "Test Result", txtUnit_TResult.Text, txtSpecified_TResult.Text, txtMeasured_TResult.Text, txtRemark_TResult.Text);

            // Bind the GridView with the updated DataTable
            GridView1.DataSource = dt;
            GridView1.DataBind();
            // Clear the input fields

        }

        private void AddRowToDataTable(DataTable dt, string itemTest, string unit, string specified, string measured, string remark)
        {
            if (!string.IsNullOrEmpty(unit) || !string.IsNullOrEmpty(specified) || !string.IsNullOrEmpty(measured) || !string.IsNullOrEmpty(remark))
            {
                dt.Rows.Add(itemTest, unit, specified, measured, remark);
            }
        }



        //protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "EditRow")
        //    {
        //        // Handle the Edit button click
        //        int rowIndex = Convert.ToInt32(e.CommandArgument);
        //        DataTable dt = (DataTable)ViewState["dt"];
        //        DataRow row = dt.Rows[rowIndex];



        //        // Bind the GridView with the updated DataTable
        //        GridView1.DataSource = dt;
        //        GridView1.DataBind();
        //    }
        //}


        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridView1.EditIndex = rowIndex; // Enable edit mode for the selected row

                DataTable dt = (DataTable)ViewState["dt"];
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else if (e.CommandName == "UpdateRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["dt"];

                // Find the controls inside the GridView row
                GridViewRow row = GridView1.Rows[rowIndex];

                TextBox txtSpecified = (TextBox)row.FindControl("txtSpecified");
                TextBox txtMeasured = (TextBox)row.FindControl("txtMeasured");
                TextBox txtRemark = (TextBox)row.FindControl("txtRemark");

                // Update DataTable with new values
                dt.Rows[rowIndex]["Specified"] = txtSpecified.Text;
                dt.Rows[rowIndex]["Measured"] = txtMeasured.Text;
                dt.Rows[rowIndex]["Remark"] = txtRemark.Text;

                ViewState["dt"] = dt;

                GridView1.EditIndex = -1; // Exit edit mode
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else if (e.CommandName == "CancelEdit")
            {
                GridView1.EditIndex = -1; // Cancel edit mode
                GridView1.DataSource = (DataTable)ViewState["dt"];
                GridView1.DataBind();
            }
        }
        protected void btnCancelAll_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            txtcableType.Text = string.Empty;
            txtrefSpec.Text = string.Empty;
            txtcableSize.Text = string.Empty;
            txttestMethodNo.Text = string.Empty;
            voltage.Text = string.Empty;
            txtjobOrderNo.Text = string.Empty;
            txtdate.Text = string.Empty;
            txtWorksheetNo.Text = string.Empty;
            txtRoomNo.Text = string.Empty;
            txtRoomhumidity.Text = string.Empty;
            txtRoomTemp.Text = string.Empty;
            txtEquipment.Text = string.Empty;

            txtUnit_Imp.Text = string.Empty;
            txtSpecified_Imp.Text = string.Empty;
            txtMeasured_Imp.Text = string.Empty;
            txtRemark_Imp.Text = string.Empty;

            txtUnit_Peak.Text = string.Empty;
            txtSpecified_Peak.Text = string.Empty;
            txtMeasured_Peak.Text = string.Empty;
            txtRemark_Peak.Text = string.Empty;

            txtUnit_Front.Text = string.Empty;
            txtSpecified_Front.Text = string.Empty;
            txtMeasured_Front.Text = string.Empty;
            txtRemark_Front.Text = string.Empty;

            txtUnit_Tail.Text = string.Empty;
            txtSpecified_Tail.Text = string.Empty;
            txtMeasured_Tail.Text = string.Empty;
            txtRemark_Tail.Text = string.Empty;

            txtUnit_Con.Text = string.Empty;
            txtSpecified_Con.Text = string.Empty;
            txtMeasured_Con.Text = string.Empty;
            txtRemark_Con.Text = string.Empty;

            txtUnit_TResult.Text = string.Empty;
            txtSpecified_TResult.Text = string.Empty;
            txtMeasured_TResult.Text = string.Empty;
            txtRemark_TResult.Text = string.Empty;

            // Clear GridView data
            GridView1.DataSource = null;
            GridView1.DataBind();
        }



        protected void  ClearFields(object sender, EventArgs e)
        {
            // Clear all input fields
        

            txtUnit_Imp.Text = string.Empty;
            txtSpecified_Imp.Text = string.Empty;
            txtMeasured_Imp.Text = string.Empty;
            txtRemark_Imp.Text = string.Empty;

            txtUnit_Peak.Text = string.Empty;
            txtSpecified_Peak.Text = string.Empty;
            txtMeasured_Peak.Text = string.Empty;
            txtRemark_Peak.Text = string.Empty;

            txtUnit_Front.Text = string.Empty;
            txtSpecified_Front.Text = string.Empty;
            txtMeasured_Front.Text = string.Empty;
            txtRemark_Front.Text = string.Empty;

            txtUnit_Tail.Text = string.Empty;
            txtSpecified_Tail.Text = string.Empty;
            txtMeasured_Tail.Text = string.Empty;
            txtRemark_Tail.Text = string.Empty;

            txtUnit_Con.Text = string.Empty;
            txtSpecified_Con.Text = string.Empty;
            txtMeasured_Con.Text = string.Empty;
            txtRemark_Con.Text = string.Empty;

            txtUnit_TResult.Text = string.Empty;
            txtSpecified_TResult.Text = string.Empty;
            txtMeasured_TResult.Text = string.Empty;
            txtRemark_TResult.Text = string.Empty;

          
        }
    }
}


