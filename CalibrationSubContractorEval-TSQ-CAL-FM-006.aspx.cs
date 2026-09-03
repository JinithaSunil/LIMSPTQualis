using LIMS.DAL;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class CalibrationSubContractorEval_TSQ_CAL_FM_006 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0; int CloneCRFSampleID = 0;
        private DataTable SubContractor
        {
            get
            {
                if (ViewState["SubContractor"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ChecklistID", typeof(int));
                    dt.Columns.Add("Item", typeof(string));
                    dt.Columns.Add("Value", typeof(string));
                    dt.Columns.Add("Remarks", typeof(string));

                    DataTable dtCheckList = _cm.GetSubContractorCheckList();
                    if (dtCheckList.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtCheckList.Rows)
                        {
                            DataRow dr1 = dt.NewRow();
                            dr1["ChecklistID"] = Convert.ToInt32(dr["ID"].ToString());
                            dr1["Item"] = dr["Item"].ToString();
                            dt.Rows.Add(dr1);
                        }
                    }
                    ViewState["SubContractor"] = dt;
                }
                return (DataTable)ViewState["SubContractor"];
            }
            set { ViewState["SubContractor"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["s"] != null)
                {
                    GenerateSubcontractorEvaluation(Convert.ToInt32(Request.QueryString["s"]));
                }
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
                if (Session["EmployeeID"] != null)
                {
                    EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                }
                if (!Page.IsPostBack)
                {
                    Session["InquiryID"] = 587;
                    if (Session["InquiryID"] != null)
                    {
                        int InquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                        lblEnquiryId.Text = "ENQ" + InquiryId.ToString();
                        lblInquiryID.Text = InquiryId.ToString();

                        CalendarExtender2.EndDate = DateTime.Now;

                        BindProductCategory(InquiryId);
                        BindGrid();
                    }
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindProductCategory(int InquiryId)
        {
            DataTable dt = _cm.GetProductCategory(InquiryId);
            if (dt.Rows.Count > 0)
            {
                hdSampleCategory.Value = dt.Rows[0]["SampleCategory"].ToString();
                lblCategoryType.Text = dt.Rows[0]["ProductGroupName"].ToString();
                lblCategoryName.Text = dt.Rows[0]["SubGroupName"].ToString();
                hdSampleHistory.Value = dt.Rows[0]["SampleHistory"].ToString();
                hdDepartmentID.Value = dt.Rows[0]["DepartmentID"].ToString();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int SubContractorID = 0;
            int flag = 0;
            int EnqID = Convert.ToInt32(lblInquiryID.Text);

            if (txtSubContractorName.Text == "")
            {
                txtSubContractorName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSubContractorName.BackColor = System.Drawing.Color.Empty;
            }

            if (txtAddress.Text == "")
            {
                txtAddress.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtAddress.BackColor = System.Drawing.Color.Empty;
            }
            if (txtPhone.Text == "")
            {
                txtPhone.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPhone.BackColor = System.Drawing.Color.Empty;
            }
            if (txtFax.Text == "")
            {
                txtFax.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtFax.BackColor = System.Drawing.Color.Empty;
            }
            if (txtContactPerson.Text == "")
            {
                txtContactPerson.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (txtEmail.Text == "")
            {
                txtEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (txtEvaluatedBy.Text == "")
            {
                txtEvaluatedBy.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtEvaluatedBy.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDate.Text == "")
            {
                txtDate.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDate.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.SubContractorName = txtSubContractorName.Text;
                    _cm.SubContractorAddress = txtAddress.Text;
                    _cm.Phone = txtPhone.Text;
                    _cm.Fax = txtFax.Text;
                    _cm.ContactPerson = txtContactPerson.Text;
                    _cm.Email = txtEmail.Text;
                    _cm.EvaluatedBy = txtEvaluatedBy.Text;
                    _cm.Date = Convert.ToDateTime(txtDate.Text);

                    _cm.SubContractorID = _cm.AddSubContractorDetails(_cm);

                    if (ViewState["SubContractor"] != null)
                    {
                        DataTable dt = (DataTable)ViewState["SubContractor"];
                        foreach (DataRow dr in dt.Rows)
                        {
                            _cm.SubContractorCheckListID = Convert.ToInt32(dr["CheckListID"].ToString());
                            _cm.Item = dr["Item"].ToString();
                            _cm.Result = dr["Value"].ToString();
                            _cm.Remarks = dr["Remarks"].ToString();
                            _cm.AddSubContractorCheckListResults(_cm);
                        }
                    }

                    GenerateSubcontractorEvaluation(_cm.SubContractorID);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Fill Highlighted Fields')", true);
            }
        }
        protected void GenerateSubcontractorEvaluation(int SubContractorID)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = null;

            string path = Server.MapPath("~/Inputfiles/");
            string templatePath = path + "TSQ-CAL-FM-006.doc";
            doc = wordApp.Documents.Add(templatePath);

            System.Data.DataTable dtSubContractor = _cm.GetSubContractorDetails(SubContractorID);
            if (dtSubContractor.Rows.Count > 0)
            {
                // Replace placeholders with database values
                FindAndReplace(wordApp, "{SubContractorName}", dtSubContractor.Rows[0]["SubContractorName"].ToString());
                FindAndReplace(wordApp, "{SubContractorAddress}", dtSubContractor.Rows[0]["SubContractorAddress"].ToString());
                FindAndReplace(wordApp, "{Telephone}", dtSubContractor.Rows[0]["Phone"].ToString());
                FindAndReplace(wordApp, "{Fax}", dtSubContractor.Rows[0]["Fax"].ToString());
                FindAndReplace(wordApp, "{ContactPerson}", dtSubContractor.Rows[0]["ContactPerson"].ToString());
                FindAndReplace(wordApp, "{Email}", dtSubContractor.Rows[0]["Email"].ToString());
                FindAndReplace(wordApp, "{EvaluatedBy}", dtSubContractor.Rows[0]["EvaluatedBy"].ToString());
                FindAndReplace(wordApp, "{Date}", dtSubContractor.Rows[0]["CreatedDate"].ToString());
            }
            System.Data.DataTable dtChecklistResults = _cm.GetSubContractorCheckList(SubContractorID);
            if (dtChecklistResults.Rows.Count > 0)
            {
                int i = 0;
                foreach(DataRow dr in dtChecklistResults.Rows)
                {
                    i++;
                    FindAndReplace(wordApp, "{v"+i+"}", dr["Result"].ToString());
                    FindAndReplace(wordApp, "{r" + i + "}", dr["Remarks"].ToString());
                }
                //CreateEvaluationTable(wordApp, doc, "{Evaluation}", dtChecklistResults);
                try
                {
                    string pathout = Server.MapPath("~/SubContractor/");
                    string outputDocxPath = pathout + "SubContractor-" + SubContractorID + ".docx";
                    string outputPdfPath = pathout + "SubContractor-" + SubContractorID + ".pdf";
                    doc.SaveAs2(outputDocxPath);
                    doc.SaveAs2(outputPdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                    Response.AddHeader("content-disposition", "inline;filename=" + outputPdfPath);
                    string script = "<script>window.open('/SubContractor/SubContractor-" + SubContractorID + ".pdf" + "', '_blank');</script>";
                    string relativePath = @"SubContractor\SubContractor-" + SubContractorID + ".pdf";

                    // Convert to absolute path
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                    if (File.Exists(fullPath))
                    {
                        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                    }
                    else
                    {
                        Console.WriteLine("PDF file not found: " + fullPath);
                    }
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenPDF", script);
                    // To save changes before closing
                    doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges);
                    wordApp.Quit();

                }
                catch (Exception ex)
                {
                    string s1 = ex.Message;
                    Response.Write(s1);
                }
                finally
                {
                    if (doc != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                        doc = null;
                        wordApp = null;
                    }

                    GC.Collect();
                    Response.Redirect("SubContractorList.aspx");
                }
            }
        }
        static void FindAndReplace(Microsoft.Office.Interop.Word.Application wordApp, string findText, string replaceText)
        {
            wordApp.Selection.Find.ClearFormatting();
            wordApp.Selection.Find.Text = findText;
            wordApp.Selection.Find.Replacement.ClearFormatting();
            wordApp.Selection.Find.Replacement.Text = replaceText;
            wordApp.Selection.Find.Execute(Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
        }
        public void CreateEvaluationTable(Microsoft.Office.Interop.Word.Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, System.Data.DataTable dt)
        {
            var range = doc.Content;
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range != null)
            {
                // 4. Create a table at placeholder
                int rows = dt.Rows.Count + 1; // +1 for header
                int cols = dt.Columns.Count;

                Microsoft.Office.Interop.Word.Table wordTable = doc.Tables.Add(range, rows, cols);
                wordTable.Borders.Enable = 1;
                // 5. Insert column headers
                for (int c = 0; c < cols; c++)
                {
                    wordTable.Cell(1, c + 1).Range.Text = dt.Columns[c].ColumnName;
                    wordTable.Cell(1, c + 1).Range.Bold = 1;
                }


                // 6. Insert data rows
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        wordTable.Cell(r + 2, c + 1).Range.Text = dt.Rows[r][c].ToString();
                    }
                }
            }
        }
        private void Clear()
        {
            txtSubContractorName.Text = "";
            txtAddress.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            //  txtSampleDestroyedDate.Text = "";           
        }
        private void BindGrid()
        {
            gvSubContractorEvaluation.DataSource = SubContractor;
            gvSubContractorEvaluation.DataBind();
        }
        protected void gvSubContractorEvaluation_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvSubContractorEvaluation.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvSubContractorEvaluation_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvSubContractorEvaluation.EditIndex = -1;
            BindGrid();
        }

        protected void gvSubContractorEvaluation_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvSubContractorEvaluation.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvSubContractorEvaluation.Rows[e.RowIndex];

            HiddenField hdChecklistID = (System.Web.UI.WebControls.HiddenField)(row.FindControl("hdChecklistID"));
            TextBox txtValue = (System.Web.UI.WebControls.TextBox)(row.FindControl("txtValue"));
            TextBox txtRemarks = (System.Web.UI.WebControls.TextBox)(row.FindControl("txtRemarks"));

            DataTable dt = SubContractor;
            DataRow dr = dt.Select("ChecklistID=" + id)[0];
            dr["Value"] = txtValue.Text;
            dr["Remarks"] = txtRemarks.Text;

            SubContractor = dt;
            gvSubContractorEvaluation.EditIndex = -1;

            BindGrid();
        }
    }
}