using LIMS.DAL;
using LIMS.UserControls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class RequisitionPaymentsNew : System.Web.UI.Page
    {
        DAL.GLModule _gl = new GLModule();
        DAL.APModule _ap = new DAL.APModule();
        DAL.Journal _jl = new DAL.Journal();
        DAL.BankingModule _bk = new DAL.BankingModule();
        DAL.CM _cm = new DAL.CM();
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPaymentRequisition();
            }
        }
        protected void BindPaymentRequisition()
        {
            if (Request.QueryString["ID"] != null)
            {
                string PaymentRequisitionID = Request.QueryString["ID"].ToString();
                DataTable dtQD = _cm.GetPaymentRequisitionDetails(Convert.ToInt32(PaymentRequisitionID));
                if (dtQD.Rows.Count > 0)
                {
                    gvItems.DataSource = dtQD;
                    gvItems.DataBind();
                }
            }
        }
        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Process")
            {
                int detailId = Convert.ToInt32(e.CommandArgument);

                // Load selected PR item
                LoadPaymentItem(detailId);

                // Load journal distribution
                JournalEntry1.LoadJournal(detailId);
            }
        }

        private void LoadPaymentItem(int detailId)
        {
            DataTable dt = _cm.LoadPaymetItem(detailId);

            if (dt.Rows.Count == 0)
                return;

            lblDescription.Text = dt.Rows[0]["ItemDescription"].ToString();
            lblGross.Text = Convert.ToDecimal(dt.Rows[0]["TotalPrice"]).ToString("N2");

            // txtVendor.Text = dt.Rows[0]["VendorName"].ToString();

            ViewState["DetailID"] = detailId;
        }
        private DataTable BuildJournalHeader()
        {
            _jl.AccountID = "";// hdCOAAccountCode.Value;
            _jl.JournalDate = Convert.ToDateTime(txtDate.Text);
            _jl.ReferenceNo = txtReference.Text;
            _jl.Transaction = "Payment";
            _jl.Description = ddlPayee.SelectedItem.Text;

            if (Session["EmployeeName"] != null)
                _jl.CreatedBy = Session["EmployeeName"].ToString();
            else
                _jl.CreatedBy = "Accounts User";

            DataTable dtHeader = _jl.JournalHeader();

            dtHeader.Rows.Add(
                _jl.JournalDate,
                _jl.ReferenceNo,
                _jl.Transaction,
                _jl.Description,
                _jl.CreatedBy
            );

            return dtHeader;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1. Save Payment
                    int paymentId = SavePayment(con, tran);

                    // 2. Prepare Journal
                    DataTable journalHeader = BuildJournalHeader();
                    DataTable journalLines = JournalEntry1.GetJournalLines();

                    // 3. Save Journal
                    int journalId = _jl.AddJournalWithGL(
                                        tran,
                                        journalHeader,
                                        journalLines);

                    // 4. Link Payment with Journal
                    SqlCommand cmd = new SqlCommand(
                        @"UPDATE Payment
                  SET JournalID=@JournalID
                  WHERE PaymentID=@PaymentID",
                        con, tran);

                    cmd.Parameters.AddWithValue("@JournalID", journalId);
                    cmd.Parameters.AddWithValue("@PaymentID", paymentId);

                    cmd.ExecuteNonQuery();

                    tran.Commit();

                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "msg",
                        "alert('Payment saved successfully.');",
                        true);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        private int SavePayment(SqlConnection con, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand("sp_SavePayment", con, tran);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PaymentDate", Convert.ToDateTime(txtDate.Text));
            cmd.Parameters.AddWithValue("@ReferenceNo", txtReference.Text);
            cmd.Parameters.AddWithValue("@PayeeID", ddlPayee.SelectedValue);
            cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmountPaid.Text));
            cmd.Parameters.AddWithValue("@PaymentMode", radPaymentMode.SelectedValue);
            cmd.Parameters.AddWithValue("@BankAccountID", txtBankName.Text);

            if (ViewState["PaymentRequisitionDetailsID"] != null)
                cmd.Parameters.AddWithValue("@PaymentRequisitionDetailsID",
                    ViewState["PaymentRequisitionDetailsID"]);

            object result = cmd.ExecuteScalar();

            return Convert.ToInt32(result);
        }
    }

}
