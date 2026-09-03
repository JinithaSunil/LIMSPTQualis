using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class JournalEntries : System.Web.UI.Page
    {
        DAL.Journal j = new DAL.Journal();
        public class ChildItem
        {
            public int ChildID { get; set; }
            public string ChildName { get; set; }
        }

        public class ParentItem
        {
            public int ParentID { get; set; }
            public string ParentName { get; set; }
            public List<ChildItem> Children { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYear();
                BindMonths();
                HighlightActiveMonth();
                BindJournal();
            }
        }
        protected void BindYear()
        {
            DataTable dt = new DataTable();
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                years.Add(year);
            }

            // Bind the dropdownlist
            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, "Select Year");
            ddlYear.Items.FindByText(DateTime.Now.ToString("yyyy")).Selected=true;
            hfMonth.Value = DateTime.Now.ToString("MM").ToString();
        }
        protected void BindJournal()
        {            
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            int month = Convert.ToInt32(hfMonth.Value);

            DataTable dt = j.GetJournalEntries(year, month);
            DataTable dtParent = j.GetJournalHeader(year, month);
            DataTable dtChild = j.GetJournalLines();
            if (dtParent.Rows.Count > 0)
            {
                ViewState["ChildTable"] = dtChild;
                gvJournalEntries.DataSource = dt;
                gvJournalEntries.DataBind();
            }
            else
            {
                gvJournalEntries.DataSource = null;
                gvJournalEntries.DataBind();
            }

        }
        private void BindMonths()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Text");
            dt.Columns.Add("Value");

            dt.Rows.Add("All", "0");

            for (int i = 1; i <= 12; i++)
            {
                dt.Rows.Add(i.ToString("00"), i.ToString());
            }

            rptMonths.DataSource = dt;
            rptMonths.DataBind();
        }
        protected void Month_Command(object sender, CommandEventArgs e)
        {
            hfMonth.Value =  e.CommandArgument.ToString().PadLeft(2,'0');
            HighlightActiveMonth();
            BindJournal();
        }
        private void HighlightActiveMonth()
        {
            foreach (RepeaterItem item in rptMonths.Items)
            {
                Button btn = (Button)item.FindControl("btnMonth");
                btn.CssClass = "month-btn";

                if (btn.CommandArgument.PadLeft(2,'0') == hfMonth.Value)
                {
                    btn.CssClass += " active";
                }
            }
        }

        protected void rptParent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int parentId = Convert.ToInt32(drv["JournalID"]);

                DataTable dtChild = ViewState["ChildTable"] as DataTable;

                DataRow[] rows = dtChild.Select("JournalID = " + parentId);

                if (rows.Length > 0)
                {
                    DataTable dtFiltered = rows.CopyToDataTable();

                    Repeater rptChild = (Repeater)e.Item.FindControl("rptChild");
                    rptChild.DataSource = dtFiltered;
                    rptChild.DataBind();
                }
            }
        }

        protected void btnNewJournalEntry_Click(object sender, EventArgs e)
        {
            Response.Redirect("JournalEntry.aspx");
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindJournal();
        }
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            // Export the GridView to Excel
            ExportGridViewToExcel(gvJournalEntries, "JournalEntries.xlsx");
        }
        private void ExportGridViewToExcel(GridView gridView, string fileName)
        {
            // Create an instance of Excel application
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = false; // Do not show Excel application
            excelApp.DisplayAlerts = false; // Suppress alerts

            // Add a workbook
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;

            try
            {
                // Add headers
                for (int i = 0; i < gridView.HeaderRow.Cells.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = gridView.HeaderRow.Cells[i].Text;
                }

                // Add rows
                for (int i = 0; i < gridView.Rows.Count; i++)
                {
                    for (int j = 0; j < gridView.Rows[i].Cells.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = gridView.Rows[i].Cells[j].Text;
                    }
                }

                // Set the path to the Downloads folder on the C drive
                string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string filePath = Path.Combine(downloadFolder, fileName);

                // Save the workbook to the Downloads folder
                workbook.SaveAs(filePath);

                // Close the workbook and quit Excel
                workbook.Close(false);
                excelApp.Quit();

                // Send the file to the client
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(filePath);
                Response.End();

                // Optionally, delete the file after sending to the client (if required)
                // File.Delete(filePath);
            }
            catch (Exception ex)
            {
                // Handle errors
                Response.Write("An error occurred: " + ex.Message);
            }
            finally
            {
                // Release COM objects to avoid memory leaks
                if (workbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                if (excelApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }


    }
}