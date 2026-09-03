using LIMS.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class ItemInward : System.Web.UI.Page
    {
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");

        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["EmailId"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
            }
            else
            {
                if (!IsPostBack)
                {
                    txtReceivedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    string roleId = Session["RoleId"].ToString();
                    string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                    string pageName = System.IO.Path.GetFileName(currentPageUrl);
                    string urlToCheck = pageName;

                    try
                    {
                        /* 23Apr26 Elan - Provide drop down to select Vendor                         
                        DataTable dt = _cm.GetInwardPO();
                        if (dt.Rows.Count > 0)
                        {
                            ddlPoNo.DataSource = dt;
                            ddlPoNo.DataTextField = "PONumber";
                            ddlPoNo.DataValueField = "PurchaseOrderID";
                            ddlPoNo.DataBind();
                            ddlPoNo.Items.Insert(0, new ListItem("Select PO Number", ""));
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('PO Complete First/Records Not Found.'); </script>");
                        }*/
                        if (Session["GRNFiles"] == null)
                        {
                            DataTable dt1 = new DataTable();
                            dt1.Columns.Add("DocumentName");
                            dt1.Columns.Add("OriginalFileName");
                            dt1.Columns.Add("TempFileName");

                            Session["GRNFiles"] = dt1;
                        }

                        DataTable dt = _cm.GetInwardPOVendor();
                        if (dt.Rows.Count > 0)
                        {
                            ddlVendor.DataSource = dt;
                            ddlVendor.DataTextField = "VendorName";
                            ddlVendor.DataValueField = "VendorID";
                            ddlVendor.DataBind();
                            ddlVendor.Items.Insert(0, new ListItem("Select Vendor", ""));
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Pendign PO Records Found.'); </script>");
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
        }
        protected void btnAddDocument_Click(object sender, EventArgs e)
        {
            if (txtDocumentName.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "msg", "alert('Enter Document Name');", true);
                return;
            }

            if (!fpFile.HasFile)
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "msg", "alert('Select File');", true);
                return;
            }

            DataTable dt;

            if (Session["GRNFiles"] == null)
            {
                dt = new DataTable();
                dt.Columns.Add("DocumentName");
                dt.Columns.Add("OriginalFileName");
                dt.Columns.Add("TempFileName");
            }
            else
            {
                dt = (DataTable)Session["GRNFiles"];
            }

            string folder = Server.MapPath("~/UploadedFiles/Temp/");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string savedFileName = Guid.NewGuid().ToString("N") +
                                   Path.GetExtension(fpFile.FileName);

            fpFile.SaveAs(Path.Combine(folder, savedFileName));

            DataRow dr = dt.NewRow();
            dr["DocumentName"] = txtDocumentName.Text.Trim();
            dr["OriginalFileName"] = fpFile.FileName;
            dr["TempFileName"] = savedFileName;

            dt.Rows.Add(dr);

            Session["GRNFiles"] = dt;

            gvAttachments.DataSource = dt;
            gvAttachments.DataBind();

            txtDocumentName.Text = "";
        }
        protected void gvAttachments_RowDeleting(object sender,
    GridViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)Session["GRNFiles"];

            string tempFile = Server.MapPath("~/UploadedFiles/Temp/")
                + dt.Rows[e.RowIndex]["TempFileName"].ToString();

            if (File.Exists(tempFile))
                File.Delete(tempFile);

            dt.Rows.RemoveAt(e.RowIndex);

            Session["GRNFiles"] = dt;

            gvAttachments.DataSource = dt;
            gvAttachments.DataBind();
        }
        protected void BindGrid()
        {
            if (ViewState["DT"] != null)
            {
                DataTable dt = (DataTable)ViewState["DT"];
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        protected void CheckViewState()
        {
            if (ViewState["DT"] != null)
            {
                DataTable dt = (DataTable)ViewState["DT"];

                DataColumnCollection columns = dt.Columns;
                if (!columns.Contains("ReceivedQty"))
                {
                    // Add a new column named "NewColumn" of type string
                    dt.Columns.Add("ReceivedQty", typeof(string));
                }
                // Optional: Add a default value to the new column for existing rows
                foreach (DataRow row in dt.Rows)
                {
                    row["ReceivedQty"] = "0";
                }

                // Store the modified DataTable back into ViewState
                ViewState["DT"] = dt;
            }
            else
            {

            }
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGrid();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            TextBox txtReceivedQty = (TextBox)row.FindControl("txtReceivedQty");

            int rowIndexToUpdate = e.RowIndex;
            int PODetailsID = Convert.ToInt32(e.Keys["PODetailsID"].ToString());
            DataTable dt = (DataTable)ViewState["DT"];

            DataRow dr = dt.AsEnumerable()
               .SingleOrDefault(r => r.Field<int>("PODetailsID") == PODetailsID);           //dt.Select($"{PODetailsID} = '{PODetailsID}'");


            dr["ReceivedQty"] = txtReceivedQty.Text;
            ViewState["DT"] = dt;

            GridView1.EditIndex = -1; // Exit edit mode
            BindGrid(); // Rebind to reflect changes
        }
        public void GetGRNNumber()
        {
            DateTime ReceivedDate = Convert.ToDateTime(txtReceivedDate.Text);
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now;
            DateTime startOfPeriod = new DateTime(currentYear, 4, 1);       /* Start on April 1st */
            DateTime endOfPeriod = new DateTime(currentYear + 1, 3, 31);    /* End on March 31st of next year */

            string GRN_Number; string GRNYear = "";
            int GRNCounter = 1;
            DataTable dtLastGRN = new DataTable();
            DataTable dtGRN = _cm.GetGRNYear(ReceivedDate);
            if (dtGRN.Rows.Count > 0)
            {
                GRNYear = dtGRN.Rows[0]["GRNPostFix"].ToString();
                dtLastGRN = _cm.GetLastGRN(GRNYear);
                if (dtLastGRN.Rows.Count > 0)
                {
                    GRNCounter = dtLastGRN.Rows.Count + 1;
                }
                GRN_Number = "GRN/" + GRNYear + "/" + GRNCounter.ToString("D4");
                ViewState["GRNNumber"] = GRN_Number;
                ViewState["GRNCounter"] = GRNCounter;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Add Current Financial Year to POYear.)", true);
            }

        }
        protected void btnSaveInward_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ddlPoNo.SelectedIndex == 0)
            {
                flag = 1;
                ddlPoNo.BorderColor = Color.Red;
            }
            else
            {
                ddlPoNo.BorderColor = Color.Empty;
            }
            if (txtReceivedDate.Text == "")
            {
                flag = 1;
                txtReceivedDate.BorderColor = Color.Red;
            }
            else
            {
                txtReceivedDate.BorderColor = Color.Empty;
            }
            if (fpFile.PostedFiles == null || fpFile.PostedFiles.Count == 0)
            {
                fpFile.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                fpFile.BorderColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                if (ViewState["DT"] != null)
                {
                    DataTable dt = (DataTable)ViewState["DT"];
                    _cm.PurchaseOrderID = Convert.ToInt32(ddlPoNo.SelectedValue.ToString());
                    _cm.ReceivedDate = txtReceivedDate.Text;
                    _cm.ReceivedBy = Session["EmployeeID"].ToString();

                    // Ensure at least one item has Received Qty > 0
                    bool hasReceivedQty = false;

                    if (ViewState["DT"] != null)
                    {
                        DataTable dt1 = (DataTable)ViewState["DT"];

                        hasReceivedQty = dt1.AsEnumerable().Any(r =>
                        {
                            decimal qty;
                            return decimal.TryParse(r["ReceivedQty"].ToString(), out qty) && qty > 0;
                        });
                    }

                    if (!hasReceivedQty)
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "msg",
                            "alert('Please enter Received Quantity greater than 0 for at least one item.');",
                            true);

                        return;
                    }
                    try
                    {
                        int InwardID = _cm.AddItemInward(_cm);
                        hdInwardID.Value = InwardID.ToString();
                        foreach (DataRow dr in dt.Rows)
                        {
                            _cm.TransactionID = InwardID;
                            _cm.ItemID = Convert.ToInt32(dr["ItemID"].ToString());
                            _cm.PODetailsID = Convert.ToInt32(dr["PODetailsID"].ToString());
                            _cm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                            _cm.PreviousReceivedQty = Convert.ToDecimal(dr["POReceivedQty"].ToString());
                            _cm.ReceivedQty = Convert.ToDecimal(dr["ReceivedQty"].ToString());
                            if (_cm.ReceivedQty > 0)
                            {
                                _cm.AddItemInwardDetails(_cm);
                                _cm.POReceivedQty = Convert.ToDecimal(_cm.PreviousReceivedQty + _cm.ReceivedQty);
                                _cm.UpdatePOReceivedQty(_cm);
                            }
                        }
                        GetGRNNumber();
                        _cm.GRNNumber = ViewState["GRNNumber"].ToString();
                        _cm.GRNCounter = Convert.ToInt32(ViewState["GRNCounter"]);
                        _cm.GRNYear = Convert.ToDateTime(_cm.ReceivedDate).ToString("yyyy");
                        _cm.UpdateGRN(_cm);
                        GRNFileUpload(_cm.GRNNumber);
                        string s = _cm.Encode(_cm.TransactionID.ToString());
                        string url = $"GenerateGRN.aspx?ID={s}";
                        string url1 = "GRNGenerateShow.aspx";
                        Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                        Response.Write("<script> window.open( '" + url1 + "','_blank' ); </script>");
                    }
                    catch (Exception ex)
                    {
                        DataTable dtFiles = Session["GRNFiles"] as DataTable;

                        if (dtFiles != null)
                        {
                            foreach (DataRow dr in dtFiles.Rows)
                            {
                                string path = Server.MapPath("~/UploadedFiles/GRNDocs/" +
                                                             dr["TempFileName"]);

                                if (File.Exists(path))
                                    File.Delete(path);
                            }

                            Session.Remove("GRNFiles");
                        }

                        throw;
                    }
                   
                }
            }
        }

        protected void GRNFileUpload(string GRNNumber)
        {
            DataTable dt = (DataTable)Session["GRNFiles"];

            foreach (DataRow dr in dt.Rows)
            {
                string tempFile = Server.MapPath("~/UploadedFiles/Temp/")
                    + dr["TempFileName"].ToString();

                string ext = Path.GetExtension(tempFile);

                string newName = hdInwardID.Value + "_" +
                                 Guid.NewGuid().ToString("N").Substring(0, 8) + ext;

                string finalPath = Server.MapPath("~/UploadedFiles/GRNDocs/")
                    + newName;

                File.Move(tempFile, finalPath);

                _cm.TransactionID = Convert.ToInt32(hdInwardID.Value);
                _cm.GRNNumber = GRNNumber;
                _cm.DocumentName = dr["DocumentName"].ToString();
                _cm.FileName = newName;

                _cm.GRNFileUpload(_cm);
            }

            Session.Remove("GRNFiles");

            ScriptManager.RegisterStartupScript(this, GetType(),
                "msg", "alert('Files uploaded successfully.');", true);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string POID = ddlPoNo.SelectedItem.Value;
            DataTable dt1 = new DataTable();
            DataTable dt = _cm.GetPurchaseOrderItemInward(POID);
            if (ViewState["DT"] != null)
            {
                dt1 = (DataTable)ViewState["DT"];
                dt1.PrimaryKey = new DataColumn[] { dt1.Columns["PoDetailsID"] };
                dt1.Merge(dt);
                CheckViewState();
                GridView1.DataSource = dt1;
                GridView1.DataBind();
            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    ViewState["DT"] = dt;
                    CheckViewState();
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'ItemInward.aspx'; }, 10);</script>");
                }
            }
        }

        protected void ddlVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (ddlVendor.SelectedIndex != 0)
            {
                dt = _cm.GetInwardPO(ddlVendor.SelectedValue.ToString());
                if (dt.Rows.Count > 0)
                {
                    ddlPoNo.DataSource = dt;
                    ddlPoNo.DataTextField = "PONumber";
                    ddlPoNo.DataValueField = "PurchaseOrderID";
                    ddlPoNo.DataBind();
                    ddlPoNo.Items.Insert(0, new ListItem("Select PO Number", ""));
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Pending PO Found.'); </script>");
                }
            }
        }
    }
}