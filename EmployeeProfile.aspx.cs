using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class EmployeeProfile : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSignatures();
            }
            if (Session["Usertype"] != null)
            {
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
                if (Session["UserID"] != null)
                {
                    int EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                    BindEmployeeDetails(EmployeeID);
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    if (RoleID == 28)
                    {
                        liSignatureTab.Visible = true;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void LoadSignatures()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            imgPRSignature.ImageUrl = _cm.GetSignature(userId, "PR");
            imgPOSignature.ImageUrl = _cm.GetSignature(userId, "PO");
        }

        protected void btnUploadPR_Click(object sender, EventArgs e)
        {
            SaveSignature(fuPRSignature, "PR");

            LoadSignatures();
        }
        protected void btnUploadPO_Click(object sender, EventArgs e)
        {
            SaveSignature(fuPOSignature, "PO");

            LoadSignatures();
        }
        private void SaveSignature(FileUpload upload, string documentType)
        {
            if (!upload.HasFile)
                return;

            int userId = Convert.ToInt32(Session["UserID"]);

            string folder = Server.MapPath("~/Signatures/" + userId);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(upload.FileName);

            string fileName = documentType + extension;

            string physicalPath = Path.Combine(folder, fileName);

            upload.SaveAs(physicalPath);

            string virtualPath = "~/Signatures/" + userId + "/" + fileName;

            _cm.SaveSignatureToDatabase(userId, documentType, virtualPath);
        }
        private void BindEmployeeDetails(int EmployeeID)
        {
            DataTable dt = _cm.getEmployeeProfile(EmployeeID);

            if (dt.Rows.Count > 0)
            {
                ulEmployee.Visible = true;
                tabName.InnerText = "Employee Details";
                lblEmployee.Text = dt.Rows[0]["EmployeeName"].ToString().ToUpper();
                lblEmployeeName.Text = dt.Rows[0]["EmployeeName"].ToString();
                lblEmployeeID.Text = dt.Rows[0]["EmployeeID"].ToString();
                lblDepartment.Text = dt.Rows[0]["EmployeeDepartmentName"].ToString();
                lblDesignation.Text = dt.Rows[0]["DesignationName"].ToString();
                lblUserName.Text = dt.Rows[0]["UserName"].ToString();
                lblRole.Text = dt.Rows[0]["RoleName"].ToString();
            }
            DataTable dtUnique = dt.AsEnumerable()
                                 .GroupBy(r => new
                                 {
                                     EmployeeName = r["EmployeeName"].ToString(),
                                     Designation = r["DesignationName"].ToString(),
                                     Mobile = r["Mobile"].ToString(),
                                     Email = r["Email"].ToString(),
                                     ContactNumber = r["ContactNumber"].ToString()
                                 })
                                 .Select(g => g.First())
                                 .CopyToDataTable();

            liContacts.DataSource = null;
            liContacts.DataBind();
            liContacts.DataSource = dtUnique;
            liContacts.DataBind();
        }

    }
}