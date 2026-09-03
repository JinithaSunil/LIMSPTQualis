using ClosedXML.Excel;
using LIMS.DAL;
using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ManagementReportList : System.Web.UI.Page
    {
        //SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        //string CM = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindQuotation();
            }
        }
        protected void BindQuotation()
        {
            DataTable dt = _cm.getQuotationNEW();
            if (dt.Rows.Count > 0)
            {
                ViewState["GridData"] = dt;
                lblRecords.Text = dt.Rows.Count.ToString();
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
            else
            {
                ViewState["GridData"] = null;
                gvQuotation.DataSource = null;
                gvQuotation.DataBind();
            }
        }
        protected void BindQuotationFilter(DAL.CM _cm)
        {
            DataTable dt = _cm.getQuotationFilter(_cm);
            if (dt.Rows.Count > 0)
            {
                ViewState["GridData"] = dt;
                lblRecords.Text = dt.Rows.Count.ToString();
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
            else
            {
                ViewState["GridData"] = null;
                gvQuotation.DataSource = null;
                gvQuotation.DataBind();
            }
            if (gvQuotation.HeaderRow != null)
            {
                (gvQuotation.HeaderRow.FindControl("txtCustomerName") as TextBox).Text = ViewState["customerName"] as string ?? "";
                (gvQuotation.HeaderRow.FindControl("txtQuotationNumber") as TextBox).Text = ViewState["QuotationNumber"] as string ?? "";
                (gvQuotation.HeaderRow.FindControl("txtDate") as TextBox).Text = ViewState["Date"] as string ?? "";
                (gvQuotation.HeaderRow.FindControl("txtQuotationStatus") as TextBox).Text = ViewState["QuotationStatus"] as string ?? "";
            }

        }
        protected void gvQuotation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Set the new page index
            gvQuotation.PageIndex = e.NewPageIndex;
            // Re-bind the GridView with the same data source
            BindQuotation();
        }

        protected void gvQuotation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (Session["DepartmentID"] != null)
            {
                int DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                // Check if the row is a data row (not header or footer)
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Get the value of QuotationStatus for this row
                    string quotationStatus = DataBinder.Eval(e.Row.DataItem, "QuotationStatus").ToString();

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }


        protected void gvQuotation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Filter")
            {
                TextBox txtCustomerName = (TextBox)gvQuotation.HeaderRow.FindControl("txtCustomerName");
                TextBox txtQuotationNumber = (TextBox)gvQuotation.HeaderRow.FindControl("txtQuotationNumber");
                TextBox txtDate = (TextBox)gvQuotation.HeaderRow.FindControl("txtDate");
                TextBox txtQuotationStatus = (TextBox)gvQuotation.HeaderRow.FindControl("txtQuotationStatus");

                _cm.customerName = txtCustomerName.Text.Trim();
                _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
                if (txtDate.Text.Trim() != "")
                {
                    _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
                }
                _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

                BindQuotationFilter(_cm);
            }

            if (e.CommandName == "ClearFilter")
            {
                GridViewRow currentRow = (GridViewRow)(((Control)e.CommandSource).NamingContainer);

                // Find a TextBox named "txtData" within the current row
                TextBox txtCustomerName = (TextBox)currentRow.FindControl("txtCustomerName");
                TextBox txtQuotationNumber = (TextBox)currentRow.FindControl("txtQuotationNumber");
                TextBox txtDate = (TextBox)currentRow.FindControl("txtDate");
                TextBox txtQuotationStatus = (TextBox)currentRow.FindControl("txtQuotationStatus");
                txtCustomerName.Text = "";
                txtQuotationNumber.Text = "";
                txtDate.Text = "";
                txtQuotationStatus.Text = "";
                BindQuotation();
            }

            if (e.CommandName == "MRGenerate")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int InquiryID = (int)gvQuotation.DataKeys[rowIndex].Values[0];

                //int rowIndex = Convert.ToInt32(e.CommandArgument);
                //GridViewRow row = gvQuotation.Rows[rowIndex];
                //var quotationMasterID = gvQuotation.DataKeys[rowIndex]["QuotationMasterID"];
                Response.Redirect("ManagementReport.aspx?i="+ InquiryID);
            }

        }



      
        protected void radInv_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (radInv.SelectedItem.ToString() == "UnInvoiced")
            {
                dt = _cm.getQuotationUninvoiced();
            }
            else if (radInv.SelectedItem.ToString() == "Invoiced")
            {
                dt = _cm.getQuotationInvoiced();
            }
            else
            {
                dt = _cm.getQuotationNEW();
            }
            if (dt.Rows.Count > 0)
            {
                lblRecords.Text = dt.Rows.Count.ToString();
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
            else
            {
                gvQuotation.DataSource = null;
                gvQuotation.DataBind();
            }
        }

        protected void txtQuotationNumber_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);

        }

        protected void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);
        }

        protected void txtQuotationStatus_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ViewState["GridData"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                    return;

                ExportToExcel(dt, "QuotationList");
            }
            catch(Exception ex) 
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "exportError",
                    $"alert('Error exporting to Excel: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }
        private void ExportToExcel(DataTable dt, string fileName)
        {
            Response.Clear();
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}.xls");
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            hw.Write("<table border='1'>");

            hw.Write("<tr>");
            foreach (DataColumn col in dt.Columns)
                hw.Write("<th>" + col.ColumnName + "</th>");
            hw.Write("</tr>");

            foreach (DataRow row in dt.Rows)
            {
                hw.Write("<tr>");
                foreach (var cell in row.ItemArray)
                    hw.Write("<td>" + cell.ToString() + "</td>");
                hw.Write("</tr>");
            }

            hw.Write("</table>");

            Response.Write(sw.ToString());
            Response.End();
        }
    }
}

