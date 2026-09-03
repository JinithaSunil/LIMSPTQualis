using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Office.CustomUI;
using OpenXmlPowerTools;
using static LIMS.DAL.GLModule;

namespace LIMS.DAL
{
    public class GLModule
    {
        DAL.CM _cm = new CM();
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        string _AccountName; public string AccountName { get { return _AccountName; } set { _AccountName = value; } }
        decimal _Amount; public decimal Amount { get { return _Amount; } set { _Amount = value; } }
        string _AccountCode; public string AccountCode { get { return _AccountCode; } set { _AccountCode = value; } }
        int _AccountCategoryID; public int AccountCategoryID { get { return _AccountCategoryID; } set { _AccountCategoryID = value; } }

        public class LinkedAccount
        {
            public string Module { get; set; }
            public string ReceivableAccountId { get; set; }
            public string PayableAccountId { get; set; }
            public string BankAccountId { get; set; }
            public string DepositAccountId { get; set; }
            public int ItemReceipt { get; set; }
            public int TrackDeposits { get; set; }
            public int ChargeFreight { get; set; }
            public int EarlyPaymentDiscount { get; set; }
            public int LatePaymentCharges { get; set; }

        }
        public class Account
        {
            public int AccountId { get; set; }
            public string AccountName { get; set; }
            public string AccountType { get; set; } // BalanceSheet / ProfitAndLoss
            public int? ParentAccountId { get; set; }
            public List<Account> Children { get; set; } = new List<Account>();
        }

        public class Voucher
        {
            public int VoucherId { get; set; }
            public string VoucherNo { get; set; }
            public string VoucherType { get; set; }
            public DateTime VoucherDate { get; set; }
            public string Narration { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; }
            public string FinancialYear { get; set; }
            public int ApprovalStatus { get; set; }
            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public int ModifiedBy { get; set; }
            public DateTime ModifiedOn { get; set; }
            public int AccountId { get; set; }
            public string AccountCode { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public int BankClearingAccountID { get; set; }
            public string PaymentMode { get; set; }

        }

        public class Cheque
        {
            public int VoucherId { get; set; }
            public string PartyName { get; set; }
            public int ChequeID { get; set; }
            public string ChequeNo { get; set; }
            public DateTime ChequeDate { get; set; }
            public string Status { get; set; }
            public string BankAccountID { get; set; }
            public decimal Amount { get; set; }
            public DateTime StatusChangeDate { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        public class BankTransfer
        {
            public int VoucherId { get; set; }
            public string PartyName { get; set; }
            public int TransferID { get; set; }
            public string BankName { get; set; }
            public string BankAccountID { get; set; }
            public DateTime TransferDate { get; set; }
            public string Status { get; set; }
            public decimal Amount { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
        public class AccountActivityLog
        {
            public int ActivityID { get; set; }
            public DateTime ActivityDate { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string ActivityType { get; set; } // -- Create, Update, Delete, Approve, Bounce etc.
            public string ModuleName { get; set; } // -- Voucher, Cheque, Journal, Payment
            public int ReferenceId { get; set; } // -- VoucherId / ChequeId etc.
            public string ReferenceNo { get; set; } //-- VoucherNo / ChequeNo
            public string AccountId { get; set; }
            public decimal Amount { get; set; }

        }

        public class GeneralLedgerDto
        {
            public DateTime VoucherDate { get; set; }
            public string VoucherNo { get; set; }
            public string VoucherType { get; set; }
            public string Narration { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
            public decimal RunningBalance { get; set; }
        }

        public DataTable GetBankAndCashAccounts()
        {
            return DbHelper.ExecuteQuery(@"select * from BankAndCashAccounts where isActive=1 order by AccountName asc");
        }
        public DataTable GetBankAndCashAccounts(string DebitOrCredit, int dummy)
        {
            return DbHelper.ExecuteQuery(@"select cd.accountID, cd.accountname,(cd.AccountCode + ' - ' + cd.accountname) as accountname1,
                                                AccountCategory,cd.AccountCode,cd.DebitOrCredit, cd.Balance 
                                                from ChartOfAccountsHeader c
                                                inner join ChartOfAccountsDetail cd on c.AccountId = cd.ParentID
                                                inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID 
                                                where cd.Accountname like '%bank%' and cd.DebitOrCredit='" + DebitOrCredit + "' order by accountcode asc");
        }
        public DataTable GetBankAndCashAccounts(string AccountID)
        {
            return DbHelper.ExecuteQuery(@"select * from BankAndCashAccounts where AccountID='" + AccountID + "'");
        }
        public DataTable GetChartOfAccounts()
        {
            return DbHelper.ExecuteQuery(@"select accountID, (AccountCategory + ' - ' + accountname) as accountname,AccountCategory,AccountCode,Balance from ChartOfAccountsHeader c
                                            inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID order by accountname asc");
        }
        public DataTable GetCOA()
        {
            return DbHelper.ExecuteQuery(@"select cd.accountID, cd.accountname,(cd.AccountCode + ' - ' + cd.accountname) as accountname1,AccountCategory,cd.AccountCode,cd.Balance 
                                            from ChartOfAccountsHeader c
                                            inner join ChartOfAccountsDetail cd on c.AccountId = cd.ParentID
                                            inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID order by accountcode asc");
        }
        public DataTable GetCOADrCr(string DebitOrCredit)
        {
            return DbHelper.ExecuteQuery(@"select cd.accountID, cd.accountname,(cd.AccountCode + ' - ' + cd.accountname) as accountname1,
                                                AccountCategory,cd.AccountCode,cd.DebitOrCredit, cd.Balance 
                                                from ChartOfAccountsHeader c
                                                inner join ChartOfAccountsDetail cd on c.AccountId = cd.ParentID
                                                inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID 
                                                where cd.DebitOrCredit='"+ DebitOrCredit + "' order by accountcode asc");
        }
        public DataTable GetCOABank(string DebitOrCredit)
        {
            return DbHelper.ExecuteQuery(@"select cd.accountID, cd.accountname,(cd.AccountCode + ' - ' + cd.accountname) as accountname1,
                                                AccountCategory,cd.AccountCode,cd.DebitOrCredit, cd.Balance 
                                                from ChartOfAccountsHeader c
                                                inner join ChartOfAccountsDetail cd on c.AccountId = cd.ParentID
                                                inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID 
                                                where cd.AccountName like '%bank%' order by accountcode asc");
        }
        public DataTable GetCOA(string AccountType)
        {
            return DbHelper.ExecuteQuery(@"select cd.accountID, (cd.AccountCode + ' - ' + cd.accountname) as AccCode,AccountCategory,cd.AccountName,cd.AccountNameEng,cd.AccountCode,cd.Balance 
                                            from ChartOfAccountsHeader c
                                            inner join ChartOfAccountsDetail cd on c.AccountId = cd.ParentID
                                            inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID
                                            where cd.AccountNameEng='" + AccountType + "' order by cd.accountcode asc");
        }

        public DataTable GetCOAPurchaseItems()
        {
            return DbHelper.ExecuteQuery(@"SELECT 
                                        cd.accountID,
                                        (cd.AccountCode + ' - ' + cd.accountname) AS accountname,
                                        AccountCategory,
                                        cd.AccountCode,
                                        cd.Balance
                                    FROM ChartOfAccountsHeader c
                                    INNER JOIN ChartOfAccountsDetail cd 
                                        ON c.AccountId = cd.ParentID
                                    INNER JOIN AccountCategory a 
                                        ON a.AccountCategoryID = c.AccountCategoryID
                                    WHERE cd.accountid IN (
                                        SELECT TRY_CAST(LTRIM(RTRIM(value)) AS INT)
                                        FROM FunctionalityAccountMapping
                                        CROSS APPLY STRING_SPLIT(accounts, ',')
                                        WHERE functionality = 'Purchase Item Add'
                                    )
                                    ORDER BY accountcode ASC;");
        }
        public DataTable GetLinkedAccounts(string Module)
        {
            return DbHelper.ExecuteQuery(@"select * from LinkedAccounts where Module='" + Module + "'");
        }
        public static DataTable GetChartOfAccounts(string AccountCategoryID)
        {
            return DbHelper.ExecuteQuery(@"select AccountCategory,c.* from ChartOfAccountsHeader c
                                           inner join AccountCategory a on a.AccountCategoryID=c.AccountCategoryID where a.AccountCategoryID='" + AccountCategoryID + "'");
        }
        public static DataTable GetAccountCategory()
        {
            return DbHelper.ExecuteQuery(@"select * from AccountCategory");
        }
        public static DataTable GetAccountCategory(string AccountType)
        {
            return DbHelper.ExecuteQuery(@"select * from AccountCategory where AccountType ='" + AccountType + "'");
        }

        public static void AddAccount(DAL.GLModule obj)
        {
            DbHelper.ExecuteNonQuery(
                "INSERT INTO ChartOfAccountsHeader (AccountName, Balance, AccountCode,AccountCategoryID) VALUES (@Name, @Amount, @AccountCode,@AccountCategoryID)",
                new SqlParameter("@Name", obj.AccountName),
                new SqlParameter("@Amount", obj.Amount),
                new SqlParameter("@AccountCode", obj.AccountCode),
                new SqlParameter("@AccountCategoryID", obj.AccountCategoryID)
            );
        }

        public int SaveVoucher(Voucher v)
        {
            int voucherId;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1️⃣ Insert VoucherMaster
                        SqlCommand cmdMaster = new SqlCommand(@"
                    INSERT INTO VoucherMaster
                    (VoucherNo, VoucherType, VoucherDate, Narration,
                    TotalAmount, Status,FinancialYear, CreatedBy,CreatedOn,PaymentMode)
                    VALUES
                    (@VoucherNo,@VoucherType, @VoucherDate, @Narration,
                     @TotalAmount, @Status, @FinancialYear, @CreatedBy, @CreatedOn,@PaymentMode);
                     SELECT SCOPE_IDENTITY();", con, tran);

                        cmdMaster.Parameters.AddWithValue("@VoucherNo", v.VoucherNo);
                        cmdMaster.Parameters.AddWithValue("@VoucherType", v.VoucherType);
                        cmdMaster.Parameters.AddWithValue("@VoucherDate", v.VoucherDate);
                        cmdMaster.Parameters.AddWithValue("@Narration", v.Narration);
                        cmdMaster.Parameters.AddWithValue("@TotalAmount", v.TotalAmount);
                        cmdMaster.Parameters.AddWithValue("@Status", 1);
                        cmdMaster.Parameters.AddWithValue("@FinancialYear", v.FinancialYear);
                        cmdMaster.Parameters.AddWithValue("@CreatedBy", v.CreatedBy);
                        cmdMaster.Parameters.AddWithValue("@CreatedOn", v.CreatedOn);
                        cmdMaster.Parameters.AddWithValue("@PaymentMode", v.PaymentMode);

                        voucherId = Convert.ToInt32(cmdMaster.ExecuteScalar());

                        switch (v.VoucherType)
                        {
                            case "PAYMENT":
                                SqlCommand cmdDr = new SqlCommand(@"
                                        INSERT INTO VoucherDetails
                                        (VoucherId, AccountId, Debit, Credit)
                                        VALUES
                                        (@VoucherId, @AccountId, @Debit, 0)", con, tran);

                                cmdDr.Parameters.AddWithValue("@VoucherId", voucherId);
                                cmdDr.Parameters.AddWithValue("@AccountId", v.AccountId);
                                cmdDr.Parameters.AddWithValue("@Debit", v.Debit);
                                cmdDr.ExecuteNonQuery();

                                // 3️⃣ Bank Clearing Credit
                                SqlCommand cmdCr = new SqlCommand(@"
                                        INSERT INTO VoucherDetails
                                        (VoucherId, AccountId, Debit, Credit)
                                        VALUES
                                        (@VoucherId, @AccountId, 0, @Credit)", con, tran);

                                cmdCr.Parameters.AddWithValue("@VoucherId", voucherId);
                                cmdCr.Parameters.AddWithValue("@AccountId", v.BankClearingAccountID);
                                cmdCr.Parameters.AddWithValue("@Credit", v.Credit);
                                cmdCr.ExecuteNonQuery();
                                break;
                            case "RECEIPT":
                                SqlCommand cmdDr1 = new SqlCommand(@"
                                        INSERT INTO VoucherDetails
                                        (VoucherId, AccountId, Debit, Credit)
                                        VALUES
                                        (@VoucherId, @AccountId, 0, @Credit)", con, tran);

                                cmdDr1.Parameters.AddWithValue("@VoucherId", voucherId);
                                cmdDr1.Parameters.AddWithValue("@AccountId", v.AccountId);
                                cmdDr1.Parameters.AddWithValue("@Credit", v.Credit);
                                cmdDr1.ExecuteNonQuery();

                                // 3️⃣ Bank Clearing Credit
                                SqlCommand cmdCr1 = new SqlCommand(@"
                                        INSERT INTO VoucherDetails
                                        (VoucherId, AccountId, Debit, Credit)
                                        VALUES
                                        (@VoucherId, @AccountId, @Debit, 0)", con, tran);

                                cmdCr1.Parameters.AddWithValue("@VoucherId", voucherId);
                                cmdCr1.Parameters.AddWithValue("@AccountId", v.BankClearingAccountID);
                                cmdCr1.Parameters.AddWithValue("@Debit", v.Debit);
                                cmdCr1.ExecuteNonQuery();
                                break;


                        }
                        // 2️⃣ Party Debit


                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return voucherId;
        }

        public int AddChequeDetails(DAL.GLModule.Cheque obj)
        {
            string sql = @"INSERT INTO ChequeDetails
                          (VoucherID, ChequeNo, ChequeDate, BankAccountID, Amount, ChequeStatus, PartyName)
                          OUTPUT INSERTED.ChequeId
                          VALUES
                          (@VoucherID, @ChequeNo, @ChequeDate, @BankAccountID, @Amount, @ChequeStatus, @PartyName)";
            SqlParameter[] param = {
                new SqlParameter("@VoucherID", obj.VoucherId),
                new SqlParameter("@ChequeNo", obj.ChequeNo),
                new SqlParameter("@ChequeDate", obj.ChequeDate),
                new SqlParameter("@BankAccountID", obj.BankAccountID),
                new SqlParameter("@Amount", obj.Amount),
                new SqlParameter("@ChequeStatus", obj.Status),
                new SqlParameter("@PartyName", obj.PartyName)
            };
            int chequeID = Convert.ToInt32(DbHelper.ExecuteInsertWithReturn(sql, param));
            return chequeID;
        }
        public DataTable getChequeDetails(string ChequeType)
        {
            DataTable dt = new DataTable();
            string sQuery = @"SELECT
                    cd.ChequeId,
                    vm.VoucherId,
                    vm.VoucherNo,
                    vm.VoucherDate,

                    cd.ChequeNo,
                    cd.ChequeDate,

                    BankAccountId  AS BankName,
		            PartyName,

                    cd.Amount,
                    cd.ChequeStatus,

                    cd.PresentedDate,
                    cd.ClearedDate,
                    cd.BounceDate,
                    cd.Remarks

                FROM ChequeDetails cd
                INNER JOIN VoucherMaster vm 
                    ON cd.VoucherId = vm.VoucherId
					where VoucherType='" + ChequeType + "' and vm.PaymentMode='CHEQUE'";
            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sQuery, con))
            {
                try
                {
                    dt = _cm.GetDataUsingQuery(sQuery);
                }
                catch { }
            }

            return dt;
        }
        public DataTable getChequeDetails(DAL.GLModule.Cheque chq, string ChequeType)
        {
            DataTable dt = new DataTable();
            string sQuery = @"SELECT
                    cd.ChequeId,
                    vm.VoucherId,
                    vm.VoucherNo,
                    vm.VoucherDate,

                    cd.ChequeNo,
                    cd.ChequeDate,

                    BankAccountId  AS BankName,
		            PartyName,

                    cd.Amount,
                    cd.ChequeStatus,

                    cd.PresentedDate,
                    cd.ClearedDate,
                    cd.BounceDate,
                    cd.Remarks

                FROM ChequeDetails cd
                INNER JOIN VoucherMaster vm 
                    ON cd.VoucherId = vm.VoucherId
                WHERE
                    vm.VoucherType = '" + ChequeType + "'";

            if ((chq.FromDate != DateTime.MinValue) && (chq.ToDate != DateTime.MinValue))
            {
                sQuery += @" AND (cd.ChequeDate >= convert(char(10),'" + chq.FromDate + "', 126))" +
                          @" AND (cd.ChequeDate <= convert(char(10), '" + chq.ToDate + "', 126))";
            }
            if (chq.BankAccountID != "")
            {
                sQuery += @"AND (cd.BankAccountId = '" + chq.BankAccountID + "')";
            }
            if ((chq.Status != "") && (chq.Status != "All"))
            {
                sQuery += @" AND (cd.ChequeStatus = '" + chq.Status + "')";
            }
            if (chq.ChequeNo != "")
            {
                sQuery += @" AND (cd.ChequeNo LIKE '%" + chq.ChequeNo + "%')";
            }
            sQuery += " ORDER BY cd.ChequeDate DESC, cd.ChequeNo";

            dt = _cm.GetDataUsingQuery(sQuery);
            return dt;
        }
        public void UpdateChequeDetails(DAL.GLModule.Cheque chq)
        {
            string s = "";
            switch (chq.Status)
            {
                case "Presented":
                    s = "PresentedDate";
                    break;
                case "Bounced":
                    s = "BouncedDate";
                    break;
                case "Cleared":
                    s = "ClearedDate";
                    break;
            }
            string sQuery = "Update ChequeDetails set ChequeStatus='" + chq.Status + "', " + s + " = convert(char(10),'" + chq.StatusChangeDate + "', 126)  where ChequeID=" + chq.ChequeID;
            DbHelper.ExecuteNonQuery(sQuery);
        }
        public int AddBankTransfer(DAL.GLModule.BankTransfer obj)
        {
            string sql = @"INSERT INTO BankTransfers
                          (VoucherID, BankName, TransferDate, BankAccountID, Amount, PartyName)
                          OUTPUT INSERTED.TransferID
                          VALUES
                          (@VoucherID, @BankName, @TransferDate, @BankAccountID, @Amount, @PartyName)";
            SqlParameter[] param = {
                new SqlParameter("@VoucherID", obj.VoucherId),
                new SqlParameter("@BankName", obj.BankName),
                new SqlParameter("@TransferDate", obj.TransferDate),
                new SqlParameter("@BankAccountID", obj.BankAccountID),
                new SqlParameter("@Amount", obj.Amount),
                new SqlParameter("@PartyName", obj.PartyName)
            };
            int chequeID = Convert.ToInt32(DbHelper.ExecuteInsertWithReturn(sql, param));
            return chequeID;
        }

        public DataTable GetBankTransfers(string VoucherType)
        {
            return DbHelper.ExecuteQuery(@"select b.VoucherId,v.VoucherNo,VoucherType, PartyName,BankName,BankAccountID,Amount, transferDate
                    from BankTransfers b
                    inner join VoucherMaster v on v.VoucherId = b.VoucherId where VoucherType='" + VoucherType + "'");
        }
        public DataTable GetBankTransfers(DAL.GLModule.BankTransfer obj, string VoucherType)
        {
            string sQuery = @"select b.VoucherId,v.VoucherNo,VoucherType, PartyName,BankName,BankAccountID,Amount, transferDate
                    from BankTransfers b
                    inner join VoucherMaster v on v.VoucherId = b.VoucherId 
                    WHERE VoucherType='" + VoucherType + "'";
            if ((obj.FromDate != DateTime.MinValue) && (obj.ToDate != DateTime.MinValue))
            {
                sQuery += @" AND (b.TransferDate >= convert(char(10),'" + obj.FromDate + "', 126))" +
                          @" AND (b.TransferDate <= convert(char(10), '" + obj.ToDate + "', 126))";
            }
            return DbHelper.ExecuteQuery(sQuery);
        }
        public void LogAccountActivity(DAL.GLModule.AccountActivityLog log)
        {
            using (SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
                                                    INSERT INTO AccountActivityLog
                                                    (UserId, UserName, ActivityType, ModuleName,
                                                     ReferenceId, ReferenceNo,
                                                     AccountId, Amount)
                                                    VALUES
                                                    (@UserId, @UserName, @ActivityType, @ModuleName,
                                                     @ReferenceId, @ReferenceNo,
                                                     @AccountId, @Amount)", con);

                cmd.Parameters.AddWithValue("@UserId", log.UserId);
                cmd.Parameters.AddWithValue("@UserName", log.UserName);
                cmd.Parameters.AddWithValue("@ActivityType", log.ActivityType);
                cmd.Parameters.AddWithValue("@ModuleName", log.ModuleName);
                cmd.Parameters.AddWithValue("@ReferenceId", log.ReferenceId);
                cmd.Parameters.AddWithValue("@ReferenceNo", log.ReferenceNo ?? "");
                cmd.Parameters.AddWithValue("@AccountId", log.AccountId);
                cmd.Parameters.AddWithValue("@Amount", log.Amount);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable getAccountActivityLog()
        {
            string sQuery = @"select top 5 * from AccountActivityLog order by ActivityID desc";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            return dt;
        }
        public DataTable BindActivities(int pageIndex, int PageSize)
        {
            DataTable dt = new DataTable();
            int offset = pageIndex * PageSize;

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT ActivityId, ActivityDate,ModuleName, UserName, ActivityType,
                   ReferenceNo, Amount
            FROM AccountActivityLog
            ORDER BY ActivityDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*) FROM AccountActivityLog;", con))
            {
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dt.Load(dr);
                }
            }
            return dt;
        }

        private int GetTotalCount(SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM AccountActivityLog", con))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public decimal GetOpeningBalance(int accountId, DateTime fromDate)
        {
            decimal openingBalance = 0;

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT ISNULL(SUM(Debit - Credit),0) as OpeningBalance
            FROM GeneralLedger
            WHERE AccountId = @AccountId
            AND TransactionDate >= convert(char(10),@FromDate, 126)", con))
            {
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);

                con.Open();
                openingBalance = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            return openingBalance;
        }

        public List<GeneralLedgerDto> GetGeneralLedger(
                     int accountId,
                     DateTime fromDate,
                     DateTime toDate,
                     decimal openingBalance)
        {
            List<GeneralLedgerDto> list = new List<GeneralLedgerDto>();

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                        SELECT
                            VoucherDate,
                            VoucherNo,
                            VoucherType,
                            Narration,
                            DebitAmount,
                            CreditAmount,
                            SUM(DebitAmount - CreditAmount)
                                OVER (ORDER BY VoucherDate, GLId)
                            + @OpeningBalance AS RunningBalance
                        FROM GeneralLedger
                        WHERE AccountId = @AccountId
                          AND VoucherDate BETWEEN @FromDate AND @ToDate
                        ORDER BY VoucherDate, GLId", con))
            {
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@OpeningBalance", openingBalance);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new GeneralLedgerDto
                        {
                            VoucherDate = Convert.ToDateTime(dr["VoucherDate"]),
                            VoucherNo = dr["VoucherNo"].ToString(),
                            VoucherType = dr["VoucherType"].ToString(),
                            Narration = dr["Narration"].ToString(),
                            DebitAmount = Convert.ToDecimal(dr["DebitAmount"]),
                            CreditAmount = Convert.ToDecimal(dr["CreditAmount"]),
                            RunningBalance = Convert.ToDecimal(dr["RunningBalance"])
                        });
                    }
                }
            }

            return list;
        }
        public DataTable GetIncomeExpenseYearly(int year)
        {
            SqlCommand cmd = new SqlCommand(@"
                    SELECT  
                         MONTH(TransactionDate) AS MonthNo,
                         DATENAME(MONTH, TransactionDate) AS MonthName,
                         SUM(CASE WHEN credit>0 THEN Credit - Debit ELSE 0 END) AS Income,
                         SUM(CASE WHEN debit>0 THEN Debit - Credit ELSE 0 END) AS Expense
                     FROM GeneralLedger
                     WHERE YEAR(TransactionDate) = @Year
                     GROUP BY MONTH(TransactionDate), DATENAME(MONTH, TransactionDate)
                     ORDER BY MonthNo");

            cmd.Parameters.AddWithValue("@Year", year);

            return DbHelper.ExecuteDataTable(cmd);
        }

        public void UpdateLinkedAccount(DAL.GLModule.LinkedAccount LA)
        {
            string sQuery = "Update LinkedAccounts set ReceivableAccountId='" + LA.ReceivableAccountId +
                                    "',PayableAccountId='" + LA.PayableAccountId +
                                    "',BankAccountId='" + LA.BankAccountId +
                                    "',DepositAccountId='" + LA.DepositAccountId +
                                    "',TrackDeposits='" + LA.TrackDeposits +
                                    "',ChargeFreight='" + LA.ChargeFreight +
                                    "',EarlyPaymentDiscount='" + LA.EarlyPaymentDiscount +
                                    "',LatePaymentCharges='" + LA.LatePaymentCharges +
                                    "',ItemReceipt='" + LA.ItemReceipt +
                                    "' where Module='" + LA.Module + "'";
            DbHelper.ExecuteNonQuery(sQuery);
        }
        public int InsertCustomerAdvance(DAL.ARModule _ar)
        {
            int AdvanceID = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                using(SqlConnection con = new SqlConnection(cs))
                {
                    cmd.Connection = con;
                    cmd.CommandText = @"
                    INSERT INTO CustomerAdvance
                    (
                        CustomerID, QuotationID,
                        AdvanceAmount, AdjustedAmount, Status
                    )
                    VALUES
                    (
                        @CustomerID, @QuotationID,
                        @AdvanceAmount, 0, 'Open'
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = _ar.CustomerID;
                    cmd.Parameters.Add("@QuotationID", SqlDbType.VarChar).Value = _ar.QuotationID;
                    cmd.Parameters.Add("@AdvanceAmount", SqlDbType.Decimal).Value = _ar.TotalAmount;
                    con.Open();
                    AdvanceID = (int)cmd.ExecuteScalar();
                    con.Close();
                }

                return AdvanceID;
            }
        }


    }

}