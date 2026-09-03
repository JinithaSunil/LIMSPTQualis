using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ZXing;


namespace LIMS
{
    public partial class AllSamples : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usertype"] != null)
                {

                    BindYear();

                    if (Session["EmployeeName"] != null)
                    {
                        Label lblUserName = (Label)Master.FindControl("lblUserName");
                        lblUserName.Text = Session["EmployeeName"].ToString();
                    }
                    if (Session["Email"] != null)
                    {
                        Label lblUserEmail = (Label)Master.FindControl("lblUserEmail");
                        lblUserEmail.Text = Session["Email"].ToString();
                    }

                    if (Session["RoleID"] != null)
                    {
                        int RoleID = Convert.ToInt32(Session["RoleID"].ToString());

                    }
                    if (!Page.IsPostBack)
                    {
                        BindYear();
                        string year = DateTime.Now.Year.ToString();
                        ddlYear.Items.FindByValue(year).Selected = true;
                        string month = DateTime.Now.Month.ToString();
                        ddlMonth.Items.FindByValue(month).Selected = true;
                        BindSampleList();
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
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
        }
        private void BindSampleList()
        {
            string DepartmentID = "";
            if (ddlYear.SelectedIndex != 0)
            {
                int year = Convert.ToInt32(ddlYear.SelectedValue);
                int month = Convert.ToInt32(ddlMonth.SelectedValue);

                DataTable dt = new DataTable();
                DepartmentID = Session["DepartmentID"].ToString();
                switch (DepartmentID)
                {
                    case "2":
                        DepartmentID = DepartmentID + ",4";
                        dt = _cm.getAllSamples(DepartmentID, year, month);
                        break;
                    case "4":
                        DepartmentID = DepartmentID + ",2";
                        dt = _cm.getAllSamples(DepartmentID, year, month);
                        break;
                    case "3006":
                        dt = _cm.getAllSamples(DepartmentID, year, month);
                        break;
                    case "4007":
                        dt = _cm.getAllSamples(DepartmentID, year, month);
                        break;
                    case "1005":
                        dt = _cm.getAllSamples(year, month);
                        break;
                    default:
                        dt = _cm.getAllSamples(DepartmentID, year, month);
                        break;
                }
                gvSampleList.DataSource = dt;
                gvSampleList.DataBind();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Select Year');", true);
            }
        }

        protected void gvSampleList_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvSampleList.DataSource = dtrslt;
                gvSampleList.DataBind();


            }
        }

        protected void BindCustomerDetails(DropDownList ddlCustomer)
        {
            DataTable dt = _cm.GetCustomerName();
            ddlCustomer.DataSource = dt;
            ddlCustomer.DataTextField = "CustomerName";
            ddlCustomer.DataValueField = "CustomerMasterID";
            ddlCustomer.DataBind();
        }
        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlCustomer = (DropDownList)sender;
            string CustomerMasterID = ddlCustomer.SelectedValue;
            ViewState["CustomerName"] = ddlCustomer.SelectedValue;
            this.BindCustomerDetails(ddlCustomer);
            ddlCustomer.SelectedValue = CustomerMasterID;
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSampleList();

        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindSampleList();
            }
        }

        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "QR")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int ID = (int)this.gvSampleList.DataKeys[rowIndex]["ID"];
                ImageButton imgQR = (ImageButton)gvSampleList.Rows[rowIndex].FindControl("imgQR");
                DataTable dt = _cm.getSampleUID(ID);
                if (dt.Rows.Count > 0)
                {
                    GenerateQR(imgQR, dt.Rows[0]["SampleIDSeries"].ToString(), ID);
                }

            }
        }
        protected void GenerateQR(ImageButton imgQR, string SampleIDSeries, int SampleID)
        {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            var result = QCwriter.Write(SampleIDSeries);//QCwriter.Write(lblBatchNumber.Text);
            string path = Server.MapPath("~/QR/MyQR.jpg");
            var barcodeBitmap = new Bitmap(result);

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path,
                   FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
                string sourceFile = Server.MapPath("~/QR/MyQR.jpg");
                string destinationFile = Server.MapPath("~/QR/" + SampleID + ".jpg");
                imgQR.ImageUrl = "~/QR/" + SampleID + ".jpg";
                try
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }

        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imgQR = (e.Row.FindControl("imgQR") as ImageButton);
                int ID = (int)gvSampleList.DataKeys[e.Row.RowIndex].Values[0];
                string path = "QR/" + ID + ".jpg";
                if (File.Exists(path))
                {
                    imgQR.ImageUrl = path;
                }
                else
                {
                    imgQR.ImageUrl = "QR/MyQR1.jpg";
                }
            }
        }
    }
}