using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class TestAllocationPTI : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int sampleUID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                sampleUID = Convert.ToInt32(Session["SampleId"].ToString());
                int SampleID = 0; string InquiryId = ""; string Id = "";
                string ProductCategory = ""; string AllotedTo = ""; string DepartmentId = ""; string AllotedBy = "";
                string SampleReceivedDate = "";
                DataTable dt = _cm.getSampleIDFromSampleUID(sampleUID);
                if (dt.Rows.Count > 0)
                {
                    lblSampleID.Text = dt.Rows[0]["SampleIdSeries"].ToString();
                    Session["ID"] = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                    SampleID = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                }
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                DataTable dt5 = new DataTable();
                DataTable dt7 = new DataTable();
                string AllocationID = Session["AllocationId"].ToString();
                dt1 = _cm.GetAllocationDetails(AllocationID);
                if (dt.Rows.Count > 0)
                {                    
                    SampleID = Convert.ToInt32(dt1.Rows[0]["SampleId"].ToString());
                    int EmployeeId = Convert.ToInt32(dt1.Rows[0]["EmployeeId"].ToString());
                    dt2 = _cm.getSampleDetails(SampleID);
                    dt3 = _cm.GetCustomerInquiryDetails(SampleID);
                    dt5 = _cm.GetEmployeeDetails(EmployeeId);
                    if (dt2.Rows.Count > 0)
                    {
                        DateTime ReceivedDate = Convert.ToDateTime(dt2.Rows[0]["ReceivedDate"].ToString());
                        DateTime RequestDate = Convert.ToDateTime(dt2.Rows[0]["Created"].ToString());
                        Id = dt2.Rows[0]["SampleId"].ToString();
                        SampleReceivedDate = ReceivedDate.ToString("dd/MMM/yyyy HH':'mm':'ss");
                        InquiryId = dt3.Rows[0]["InquiryId"].ToString();
                    }
                    if (dt3.Rows.Count > 0)
                    {
                        string ProductCategoryId = dt3.Rows[0]["ProductCategoryId"].ToString();
                        dt4 = _cm.GetProductCategory(Convert.ToInt32(InquiryId));
                        if (dt4.Rows.Count > 0)
                        {
                            ProductCategory = dt4.Rows[0]["productgroupname"].ToString();
                        }
                    }
                    if (dt5.Rows.Count > 0)
                    {
                        AllotedTo = dt5.Rows[0]["EmployeeName"].ToString();
                        DepartmentId = dt5.Rows[0]["DepartmentId"].ToString();
                        dt7 = _cm.GetEmployeeDetails(DepartmentId, EmployeeId);
                        if (dt7.Rows.Count > 0)
                        {
                            AllotedBy = dt7.Rows[0]["SupervisorName"].ToString();
                        }
                    }
                    lblSampleReceivedDate.Text = SampleReceivedDate;
                    lblProductCategory.Text = ProductCategory;
                    lblAllocatedTo.Text = AllotedTo;
                    lblAllocatedBy.Text = AllotedBy;

                    BindSample(sampleUID);
                }
            }
        }
        private void BindSample(int sampleUID)
        {
            DataTable dt = _cm.GetCRFListForAllocation(sampleUID);
            gvResults.DataSource = dt;
            gvResults.DataBind();
        }
        protected void gvResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            sampleUID = Convert.ToInt32(Session["SampleId"].ToString());
            int flag = 0; 
            GridViewRow row = gvResults.Rows[e.RowIndex];
            int CRFSampleId = int.Parse(gvResults.DataKeys[e.RowIndex]["CRFSampleId"].ToString());
           
            if ((row.Cells[5].Controls[0] as TextBox).Text == "")//unit of measurement
            {
                (row.Cells[5].Controls[0] as TextBox).BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                (row.Cells[5].Controls[0] as TextBox).BackColor = System.Drawing.Color.Empty;
            }
            if(txtTestsRequired.Text == "")
            {
                txtTestsRequired.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtTestsRequired.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                _cm.CRFSampleId = CRFSampleId;
                _cm.sampleId = Convert.ToInt32(Session["ID"].ToString());
                _cm.testMethod = (row.Cells[3].Controls[0] as TextBox).Text;
                _cm.testParameter = (row.Cells[4].Controls[0] as TextBox).Text;
                _cm.unit = (row.Cells[5].Controls[0] as TextBox).Text;
                _cm.specification = (row.Cells[6].Controls[0] as TextBox).Text;
                _cm.testsRequested = txtTestsRequired.Text;
                _cm.UpdateCRFSampleDetails(_cm);
                gvResults.EditIndex = -1;
                this.BindSample(sampleUID);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
            }
        }

        protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
        {
            sampleUID = Convert.ToInt32(Session["SampleId"].ToString());
            gvResults.EditIndex = e.NewEditIndex;
            this.BindSample(sampleUID);
        }

        protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            sampleUID = Convert.ToInt32(Session["SampleId"].ToString());
            gvResults.EditIndex = -1;
            this.BindSample(sampleUID);
        }

        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Save")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                int Id = int.Parse(gvResults.DataKeys[index]["CRFSampleId"].ToString());

                int rowIndex = int.Parse(e.CommandArgument.ToString());
                TextBox txtResults = (TextBox)gvResults.Rows[rowIndex].Cells[0].FindControl("txtResults");
                _cm.testResult = txtResults.Text.ToString();
                _cm.sampleId = Convert.ToInt32(Id.ToString());
            }
        }

        protected void btnAllocate_Click(object sender, EventArgs e)
        {
            Response.Redirect("GenerateTestAllocation.aspx");
        }
    }
}