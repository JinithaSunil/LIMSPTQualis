using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace LIMS
{
    public partial class COA : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DataTable dtHeader;  // Level 2
        DataTable dtDetail;  // Level 3
        DataTable dtGroup;   // Level 1 (optional if you have group table)
        DataTable dtDetails;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCOAGrid();
            }
        }
        bool HasChild(int accountId, DataTable dtDetail)
        {
            return dtDetail.AsEnumerable()
                           .Any(r => Convert.ToInt32(r["ParentID"]) == accountId);
        }
        private void BindCOAGrid()
        {
            dtGroup = _cm.GetCOA(1);   // Level 1 table
            dtHeader = _cm.GetCOA(2);  // Level 2 table
            dtDetail = _cm.GetCOA(3);  // Level 3 table
            dtDetails = _cm.GetCOADetails();

            DataTable dtFinal = new DataTable();
            dtFinal.Columns.Add("Indent", typeof(int));
            dtFinal.Columns.Add("AccountCode");
            dtFinal.Columns.Add("AccountName");
            dtFinal.Columns.Add("AccountNameEng");
            dtFinal.Columns.Add("Tax");
            dtFinal.Columns.Add("Balance");
            dtFinal.Columns.Add("HasChild", typeof(bool));
            int accountId = 0; bool hasChild = false;
            // Level 1 groups
            foreach (DataRow g in dtGroup.Rows)
            {
                AddRow(dtFinal, 0, g,true);

                // Level 2 headers
                foreach (DataRow h in dtHeader.Select("ParentID=" + g["AccountID"]))
                {
                    accountId = Convert.ToInt32(h["AccountID"]);
                    hasChild = HasChild(accountId, dtDetail);
                    AddRow(dtFinal, 1, h, hasChild);

                    // Level 3 details
                    foreach (DataRow d in dtDetail.Select("ParentID=" + h["AccountID"]))
                    {
                        accountId = Convert.ToInt32(d["AccountID"]);
                        hasChild = HasChild(accountId, dtDetails);

                        AddRow(dtFinal, 2, d, hasChild);

                        // Level 4 details
                        //foreach (DataRow d2 in dtDetail.Select("ParentID=" + h["AccountID"]))
                        //{
                            foreach (DataRow d3 in dtDetails.Select("ParentID=" + d["AccountID"]))
                            {
                            accountId = Convert.ToInt32(d3["AccountID"]);
                            hasChild = HasChild(accountId, dtDetails);

                            AddRow(dtFinal, 3, d3, hasChild);
                            }
                        //}
                    }
                }
            }

            gvCOA.DataSource = dtFinal;
            gvCOA.DataBind();
        }

        void AddRow(DataTable dt, int level, DataRow src, bool hasChildren)
        {
            dt.Rows.Add(
                level,                
                src["AccountCode"],
                src["AccountName"],
                src["AccountNameEng"],
                src["Tax"],
                src["Balance"],
                hasChildren
            );
        }
        protected void gvCOA_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int level = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Indent"));
                bool hasChild = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "HasChild"));

                // EXPOSE LEVEL FOR JAVASCRIPT
                e.Row.Attributes["data-level"] = level.ToString();

                // INDENT THE TREE CELL
                HtmlGenericControl sp = (HtmlGenericControl)e.Row.FindControl("spTree");
                sp.Style["padding-left"] = (level * 20) + "px";

              
                if (level == 0 || level == 1 || level == 2 || level == 2)
                {
                    if (hasChild)
                    {
                        e.Row.Style["font-weight"] = "bold";
                        e.Row.Style["text-transform"] = "uppercase";
                        sp.InnerHtml =
                       "<span class='node-icon' style='cursor:pointer;font-size:14px;margin-right:4px;' " +
                       "onclick='toggleNode(this.closest(\"tr\"))'>▶</span>";
                    }
                    else
                    {
                        // NO CHILD → normal
                        e.Row.Style["font-weight"] = "normal";
                        e.Row.Style["text-transform"] = "none";
                        sp.InnerHtml = "<span style='display:inline-block;width:16px;'></span>";
                    }
                }


                // collapse children initially
                if (level > 0)
                    e.Row.Style["display"] = "none";
                //// FORMAT LEVELS (LEVEL 1, 2, 3)
                //if (level == 0 || level == 1 || level == 2)
                //{
                //    e.Row.Style["font-weight"] = "bold";
                //    e.Row.Style["text-transform"] = "uppercase";  // convert text to upper case
                //}
                //// COLLAPSE ALL NON-ROOT ROWS INITIALLY
                //if (level > 0)
                //{
                //    e.Row.Style["display"] = "none";
                //}
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("COAEdit.aspx");
        }

        protected void lnkEdit_Click(object sender, EventArgs e)
        {

        }
    }
}