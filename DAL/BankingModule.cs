using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class BankingModule
    {
        int _StatementId; public int StatementId { get { return _StatementId; } set { _StatementId = value; } }
        string _BankAccountId; public string BankAccountId { get { return _BankAccountId; } set { _BankAccountId = value; } }
        DateTime _StatementDate; public DateTime StatementDate { get { return _StatementDate; } set { _StatementDate = value; } }
        decimal _OpeningBalance; public decimal OpeningBalance { get { return _OpeningBalance; } set { _OpeningBalance = value; } }
        decimal _ClosingBalance; public decimal ClosingBalance { get { return _ClosingBalance; } set { _ClosingBalance = value; } }
        DateTime _CreatedOn; public DateTime CreatedOn { get { return _CreatedOn; } set { _CreatedOn = value; } }

        //Bank Reconcilation
        int _JournalId; public int JournalId { get { return _JournalId; } set { _JournalId = value; } }
        string _ReconciledBy; public string ReconciledBy { get { return _ReconciledBy; } set { _ReconciledBy = value; } }
        string _Notes; public string Notes { get { return _Notes; } set { _Notes = value; } }
        public struct BankingVariables
        {
            public string BankAccountID { get; set; }
            public string BankName { get; set; }
            public string AccountNumber { get; set; }
            public string CurrencyCode { get; set; }
            public decimal Balance { get; set; }
        }
        public DataTable BankingHeader()
        {
            DataTable dt = new DataTable("BankingHeaderType");
            dt.Columns.Add("BankAccountId", typeof(int));
            dt.Columns.Add("StatementDate", typeof(DateTime));
            dt.Columns.Add("OpeningBalance", typeof(decimal));
            dt.Columns.Add("ClosingBalance", typeof(decimal));
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            return dt;
        }
        public DataTable BankingLines()
        {
            DataTable dt = new DataTable("BankingLineType");
            dt.Columns.Add("TxnDate", typeof(DateTime));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("Balance", typeof(decimal));
            return dt;
        }
        public DataTable BankReconciliation()
        {
            DataTable dt = new DataTable("BankReconciliation");
            dt.Columns.Add("StatementID", typeof(int));
            dt.Columns.Add("JournalID", typeof(int));
            dt.Columns.Add("ReconciledDate", typeof(DateTime));
            dt.Columns.Add("ReconciledBy", typeof(string));
            dt.Columns.Add("Notes", typeof(string));
            return dt;
        }
        public static DataTable GetBankAccounts()
        {
            return DbHelper.ExecuteQuery("SELECT * FROM BankAccount where isActive=1");
        }
        public DataTable GetBankAccount(string BankAccountId)
        {
            return DbHelper.ExecuteQuery("SELECT * FROM BankAccount where BankAccountId='" + BankAccountId + "'");
        }
        public static void AddBankAccount(DAL.BankingModule.BankingVariables obj)
        {
            DbHelper.ExecuteNonQuery(
                "INSERT INTO BankAccount (BankName,AccountNumber, Balance) VALUES (@BankName,@AccountNumber, @Balance)",
                new SqlParameter("@BankName", obj.BankName),
                new SqlParameter("@AccountNumber", obj.AccountNumber),
                new SqlParameter("@Balance", obj.Balance)
            );
        }

        public static void UpdateBalance(int bankAccountId, decimal amount)
        {
            DbHelper.ExecuteNonQuery(
                "UPDATE BankAccount SET Balance = Balance + @Amount WHERE BankAccountId = @Id",
                new SqlParameter("@Amount", amount),
                new SqlParameter("@Id", bankAccountId)
            );
        }
        public DataTable GetBankBalance(BankingModule bk)
        {
            // Create parameters
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@BankAccountID", SqlDbType.Int) { Value = bk.BankAccountId };
            parameters[1] = new SqlParameter("@AsOfDate", SqlDbType.Date) { Value = bk.StatementDate };

            return (DbHelper.ExecuteStoredProcDT("GetBankBalance", parameters));
        }
        public int AddBankStatement(DataTable dtbankingHeader, DataTable bankingLines)
        {
            // Create parameters
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@BankAccountId", SqlDbType.NVarChar, 200) { Value = dtbankingHeader.Rows[0]["BankAccountId"] };
            parameters[1] = new SqlParameter("@StatementDate", SqlDbType.Date) { Value = dtbankingHeader.Rows[0]["StatementDate"] };
            parameters[2] = new SqlParameter("@OpeningBalance", SqlDbType.Decimal) { Value = dtbankingHeader.Rows[0]["OpeningBalance"] };
            parameters[3] = new SqlParameter("@ClosingBalance", SqlDbType.Decimal) { Value = dtbankingHeader.Rows[0]["ClosingBalance"] };

            // Table-Valued Parameter
            parameters[4] = new SqlParameter("@Lines", SqlDbType.Structured)
            {
                TypeName = "dbo.BankStatementLineType",
                Value = bankingLines
            };
            return (DbHelper.ExecuteStoredProc("AddBankStatement", parameters));
        }
        public void AddBankReconciliation(DataTable BankReconciliation)
        {
            DbHelper.ExecuteNonQuery(
               @"INSERT INTO BankReconciliation (StatementID, JournalID, ReconciledDate, ReconciledBy, Notes) " +
               "VALUES (@StatementID, @JournalID, @ReconciledDate, @ReconciledBy, @Notes)",
               new SqlParameter("@StatementID", BankReconciliation.Rows[0]["StatementID"]),
               new SqlParameter("@JournalID", BankReconciliation.Rows[0]["JournalID"]),
               new SqlParameter("@ReconciledDate", BankReconciliation.Rows[0]["ReconciledDate"]),
               new SqlParameter("@ReconciledBy", BankReconciliation.Rows[0]["ReconciledBy"]),
               new SqlParameter("@Notes", BankReconciliation.Rows[0]["Notes"])
           );
        }
        public DataTable GetBankStatement(int AccountID)
        {
            return DbHelper.ExecuteQuery(@"select journalDate,Txn,j.referenceNo,Description,Debit,Credit,openingbalance,closingbalance,ReconciliationDate from bankstatement b
                                              inner join bankReconciliation br on b.statementid=br.statementid
                                              inner join JournalHeader j on j.journalId = br.JournalID
                                              inner join journallines jl on jl.JournalID = j.JournalID
                                              where b.BankAccountID=" + AccountID);
        }
    }

}