using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;

namespace LIMS
{
    public partial class MRFormShowforApproval : Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
        private int Year;
        DataAccessLayer dal = new DataAccessLayer();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();
            try
            {
                if (Session["EmailId"] == null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                }
                else
                {
                    if (!IsPostBack)
                    {
                        string roleId = Session["RoleId"].ToString();
                        string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                        string pageName = System.IO.Path.GetFileName(currentPageUrl);
                        string urlToCheck = pageName;

                        if (Request.QueryString["PageID"] != null)
                        {
                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                            int employeeId = Convert.ToInt32(Session["UserID"]);

                            List<MaterialRequestData> response = dal.MaterialRequestGetData();
                            List<MaterialRequestData> res = response.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text)).ToList();
                            if (res != null && res.Count > 0)
                            {
                                MaterialRequestData selectedItem = res[0];

                                lblMRNo.Text = selectedItem.MRM_NUMBER.ToString();
                                lblMRGeneratedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();
                                lblPreparedDate.Text = selectedItem.MRM_CreatedDateTime.ToString();
                                lblProjectCode.Text = selectedItem.MRM_ProjectCode.ToString();
                                lblPreparedBy.Text = selectedItem.ApprovalFirstName.ToString();
                                //int sendEmpId = Convert.ToInt32(selectedItem.MRM_EmployeeID.ToString());
                                //ViewState["MRM_EmployeeID"] = sendEmpId;
                                //string MR_No = selectedItem.MRM_NUMBER.ToString();
                                //ViewState["MRM_NUMBER"] = MR_No;
                                GridView1.DataSource = res;
                                GridView1.DataBind();

                                int EmpID = Convert.ToInt32(selectedItem.MRM_EmployeeID.ToString());
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                            }

                            btnApproved.Enabled = true;
                            btnRejected.Enabled = true;

                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something wrong, PageID Not Getting!!'); setTimeout(function() { window.location = 'GRNGenerateShow.aspx'; }, 100); </script>");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);

                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                /* Assuming the index of the column MR_RequireDate is 8 */
                int columnIndex = 5;

                if (e.Row.Cells.Count > columnIndex)
                {
                    string originalDate = e.Row.Cells[columnIndex].Text;

                    if (!string.IsNullOrEmpty(originalDate))
                    {
                        DateTime parsedDate;
                        if (DateTime.TryParse(originalDate, out parsedDate))
                        {
                            e.Row.Cells[columnIndex].Text = parsedDate.ToString("dd-MMM-yyyy");
                        }
                    }
                }
            }
        }

        protected void btnApproved_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = @"UPDATE MaterialRequestMaster set MRM_StatusId = @MRM_StatusId, MRM_UpdatedDateTime = @MRM_UpdatedDateTime,
                                MRM_ApprovedEmployeeID = @MRM_ApprovedEmployeeID, MRM_ApprovedDateTime = @MRM_ApprovedDateTime
                                WHERE MRM_ID = @MRM_ID";

                SqlCommand cmd = new SqlCommand(str1, con);

                cmd.Parameters.AddWithValue("@MRM_ID", lblPageID.Text);
                cmd.Parameters.AddWithValue("@MRM_ApprovedEmployeeID", Session["EmployeeID"].ToString());
                cmd.Parameters.AddWithValue("@MRM_StatusId", 21);
                cmd.Parameters.AddWithValue("@MRM_UpdatedDateTime", GetDateTime);
                cmd.Parameters.AddWithValue("@MRM_ApprovedDateTime", GetDateTime);

                con.Open();
                int j = cmd.ExecuteNonQuery();
                con.Close();

                if (j > 0)
                {
                    var TableRowId = lblPageID.Text;
                    bool success = dal.AuditLogData("Material Request Status Update for Approval", "MaterialRequestMaster", TableRowId, "");
                    if (success)
                    { }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                    }


                    using (SqlConnection con4 = new SqlConnection(constr))
                    {
                        string str4 = @"UPDATE MaterialRequestDetails set MR_StatusId = @MR_StatusId WHERE MR_MRMID = @MR_MRMID";

                        SqlCommand cmd4 = new SqlCommand(str4, con4);
                        cmd4.Parameters.AddWithValue("@MR_StatusId", 21);
                        cmd4.Parameters.AddWithValue("@MR_MRMID", lblPageID.Text);

                        con4.Open();
                        int e1 = cmd4.ExecuteNonQuery();
                        con4.Close();

                        if (e1 > 0)
                        {
                            var TableRowId1 = lblPageID.Text;
                            bool success1 = dal.AuditLogData("Material Request Details Status Update for Approval", "MaterialRequestDetails", TableRowId1, "");
                            if (success1)
                            {
                                
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Successfully Approve'); setTimeout(function() { window.location = 'MRShowforApproval.aspx'; }, 100); </script>");

                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' Failed to update MR-Details! '); </script>");
                            //throw new Exception("Failed to update MR-Details.");
                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);

                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }
        }

        protected void btnRejected_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = @"UPDATE MaterialRequestMaster set MRM_StatusId = @MRM_StatusId, MRM_UpdatedDateTime = @MRM_UpdatedDateTime WHERE MRM_ID = @MRM_ID";

                SqlCommand cmd = new SqlCommand(str1, con);
                cmd.Parameters.AddWithValue("@MRM_ID", lblPageID.Text);
                cmd.Parameters.AddWithValue("@MRM_StatusId", 22);
                cmd.Parameters.AddWithValue("@MRM_UpdatedDateTime", GetDateTime);

                con.Open();
                int j = cmd.ExecuteNonQuery();
                con.Close();

                if (j > 0)
                {
                    var TableRowId = lblPageID.Text;
                    bool success = dal.AuditLogData("Material Request Status update for Reject", "MaterialRequestMaster", TableRowId, "");
                    if (success)
                    { }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                    }

                    using (SqlConnection con4 = new SqlConnection(constr))
                    {
                        string str4 = @"UPDATE MaterialRequestDetails set MR_StatusId = @MR_StatusId WHERE MR_MRMID = @MR_MRMID";

                        SqlCommand cmd4 = new SqlCommand(str4, con4);
                        cmd4.Parameters.AddWithValue("@MR_StatusId", 22);
                        cmd4.Parameters.AddWithValue("@MR_MRMID", lblPageID.Text);

                        con4.Open();
                        int e1 = cmd4.ExecuteNonQuery();
                        con4.Close();

                        if (e1 > 0)
                        {
                            var TableRowId2 = lblPageID.Text;
                            bool success2 = dal.AuditLogData("Material Request Details Status update for Reject", "MaterialRequestMaster", TableRowId2, "");
                            if (success2)
                            { }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }

                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Rejected!'); setTimeout(function() { window.location = 'MRShowforApproval.aspx'; }, 100); </script>");

                            //bool notificationSent = dal.NotificationSend("Your-Department", "MR Rejected", "MR No: " + ViewState["MRM_NUMBER"].ToString() + " - Rejection confirmed. We regret to inform you that the purchase request has been declined.", ViewState["MRM_EmployeeID"].ToString());
                            //if (notificationSent)
                            //{
                            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Rejected!'); setTimeout(function() { window.location = 'MRShowforApproval.aspx'; }, 100); </script>");
                            //}
                            //else
                            //{
                            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully But Notification not Send');</script>");
                            //}
                        }
                        else
                        {
                            throw new Exception("Failed to update MR-Details.");
                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);

                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }
        }

        protected void btnCancle_Click(object sender, EventArgs e)
        {
            Response.Redirect("MRShowforApproval.aspx");
        }


    }
}