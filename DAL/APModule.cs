using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LIMS.DAL;
using OpenXmlPowerTools;
using static LIMS.DAL.GLModule;

namespace LIMS.DAL
{
    public class APModule
    {
        DAL.CM _cm = new DAL.CM();
        private static string connStr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        int _VendorID; public int VendorID { get { return _VendorID; } set { _VendorID = value; } }
        string _VendorTaxInvoiceNumber; public string VendorTaxInvoiceNumber { get { return _VendorTaxInvoiceNumber; } set { _VendorTaxInvoiceNumber = value; } }
        string _InvoiceNumber; public string InvoiceNumber { get { return _InvoiceNumber; } set { _InvoiceNumber = value; } }
        DateTime _InvoiceDate; public DateTime InvoiceDate { get { return _InvoiceDate; } set { _InvoiceDate = value; } }
        DateTime _DueDate; public DateTime DueDate { get { return _DueDate; } set { _DueDate = value; } }
        string _CurrencyCode; public string CurrencyCode { get { return _CurrencyCode; } set { _CurrencyCode = value; } }
        decimal _TotalAmount; public decimal TotalAmount { get { return _TotalAmount; } set { _TotalAmount = value; } }
        decimal _BalanceAmount; public decimal BalanceAmount { get { return _BalanceAmount; } set { _BalanceAmount = value; } }
        int _PurchaseOrderID; public int PurchaseOrderID { get { return _PurchaseOrderID; } set { _PurchaseOrderID = value; } }
        decimal _AmountPaid; public decimal AmountPaid { get { return _AmountPaid; } set { _AmountPaid = value; } }
        string _Status; public string Status { get { return _Status; } set { _Status = value; } }
        int _PayeeId; public int PayeeId { get { return _PayeeId; } set { _PayeeId = value; } }
        int _PaymentID; public int PaymentID { get { return _PaymentID; } set { _PaymentID = value; } }
        DateTime _PaymentDate; public DateTime PaymentDate { get { return _PaymentDate; } set { _PaymentDate = value; } }
        decimal _PaymentAmount; public decimal PaymentAmount { get { return _PaymentAmount; } set { _PaymentAmount = value; } }
        decimal _PrevPaymentAmount; public decimal PrevPaymentAmount { get { return _PrevPaymentAmount; } set { _PrevPaymentAmount = value; } }
        string _PaymentMethod; public string PaymentMethod { get { return _PaymentMethod; } set { _PaymentMethod = value; } }
        int _VoucherID; public int VoucherID { get { return _VoucherID; } set { _VoucherID = value; } }
        string _VoucherNo; public string VoucherNo { get { return _VoucherNo; } set { _VoucherNo = value; } }

        string _COAAccount; public string COAAccount { get { return _COAAccount; } set { _COAAccount = value; } }
        string _BankAccountCode; public string BankAccountCode { get { return _BankAccountCode; } set { _BankAccountCode = value; } }
        string _ReferenceNo; public string ReferenceNo { get { return _ReferenceNo; } set { _ReferenceNo = value; } }
        string _Notes; public string Notes { get { return _Notes; } set { _Notes = value; } }
        int _JournalId; public int JournalId { get { return _JournalId; } set { _JournalId = value; } }
        DateTime _CreatedOn; public DateTime CreatedOn { get { return _CreatedOn; } set { _CreatedOn = value; } }
        string _PayeeType; public string PayeeType { get { return _PayeeType; } set { _PayeeType = value; } }
        string _PayeeName; public string PayeeName { get { return _PayeeName; } set { _PayeeName = value; } }
        string _PaymentMode; public string PaymentMode { get { return _PaymentMode; } set { _PaymentMode = value; } }
        string _PONumber; public string PONumber { get { return _PONumber; } set { _PONumber = value; } }
        string _SupplierInvoiceNo; public string SupplierInvoiceNo { get { return _SupplierInvoiceNo; } set { _SupplierInvoiceNo = value; } }
        string _Number; public string Number { get { return _Number; } set { _Number = value; } }
        int _Counter; public int Counter { get { return _Counter; } set { _Counter = value; } }
        string _Year; public string Year { get { return _Year; } set { _Year = value; } }
        string _GRNIDs; public string GRNIDs { get { return _GRNIDs; } set { _GRNIDs = value; } }
        public struct APVariables //Accounts Payable
        {
            public string vendorID { get; set; }
            public string PurchaseOrderID { get; set; }
            public string PONumber { get; set; }
            public string PayeeType { get; set; }
            public string PayeeName { get; set; }
            public DateTime PODate { get; set; }
            public decimal POAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public string SupplierInvoiceNo { get; set; }

        }
        public APVariables apVar;
        public struct PRVariables //Payment Requisition
        {
            public string EmployeeID { get; set; }
            public string PaymentRequisitionID { get; set; }
            public string PaymentRequisitionNumber { get; set; }
            public string PayeeType { get; set; }
            public string PayeeName { get; set; }
            public DateTime RequisitionDate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PrevAmountPaid { get; set; }
            public decimal CurrAmountPaid { get; set; }
            public decimal BalanceAmount { get; set; }
            public decimal AmountPaid { get; set; }

        }
        public PRVariables prVar;
        public DataTable GetVendors()
        {
            return DbHelper.ExecuteQuery("SELECT VendorId, VendorName FROM VendorDetails order by VendorName asc");
        }
        public DataTable GetPayee(string PayeeType)
        {
            string sQuery = "";
            switch (PayeeType)
            {
                case "Vendor":
                    sQuery = @"SELECT distinct VendorId, VendorName FROM VendorDetails v
                    inner join Payments p on p.payeename = v.VendorName
                    where p.PayeeType='" + PayeeType + "' order by VendorName asc";
                    break;
                case "Employee":
                    sQuery = @"SELECT distinct EmployeeID, EmployeeName FROM Employees e
                    inner join Payments p on p.payeename = e.EmployeeName
                    where p.PayeeType='" + PayeeType + "' order by EmployeeName asc";
                    break;
            }
            return DbHelper.ExecuteQuery(sQuery);
        }
        public static DataTable getPendingAPVendors()
        {
            return DbHelper.ExecuteQuery(@"select distinct p.VendorID,VendorName from PurchaseOrder p
                                            inner join VendorDetails v on v.VendorID = p.VendorId
                                            where isnull(AmountPaid,0) < isnull(GrandTotal,0)
                                            order by VendorName asc");
        }
        public void AddVendor(string vendorName)
        {
            DbHelper.ExecuteNonQuery(
                "INSERT INTO VendorDetails (VendorName) VALUES (@Name)",
                new SqlParameter("@Name", vendorName)
            );
        }

        public static DataTable GetInvoices(string filter)
        {
            string sQuery = "";
            switch (filter)
            {
                case "1"://Invoiced - Unpaid
                    sQuery = @"select i.InvoiceID,p.PurchaseOrderID,PODate,PONumber,PINumber, vendorname,GrandTotal,Status,i.AmountPaid from PurchaseOrder p
                        inner join VendorDetails v on v.VendorID = p.VendorID 
                        inner join InvoiceAP i on i.PurchaseOrderID = p.PurchaseOrderID
					    where i.amountPaid < GrandTotal";
                    break;
                case "2"://Invoiced - Paid
                    sQuery = @"select i.InvoiceID,p.PurchaseOrderID,PODate,PONumber,PINumber, vendorname,GrandTotal,Status,i.AmountPaid from PurchaseOrder p
                        inner join VendorDetails v on v.VendorID = p.VendorID 
                        inner join InvoiceAP i on i.PurchaseOrderID = p.PurchaseOrderID
					    where i.amountPaid >= GrandTotal";
                    break;
                case "3"://UnInvoiced
                    sQuery = @"select i.InvoiceID,p.PurchaseOrderID,PODate,PONumber,PINumber, vendorname,GrandTotal,Status,i.AmountPaid from PurchaseOrder p
                    inner join TransactionMaster tm on tm.PurchaseOrderID = p.PurchaseOrderID
                    inner join VendorDetails v on v.VendorID = p.VendorID 
                    left join InvoiceAP i on i.PurchaseOrderID = p.PurchaseOrderID
					where status is null";
                    break;
                default:
                    break;
            }
            return DbHelper.ExecuteQuery(sQuery);
        }
        public DataTable GetLast(string table, string field, string Year)
        {
            return DbHelper.ExecuteQuery(@"Select * from " + table + " where " + field + "='" + Year + "'");
        }
        public DataTable GetPayments(string PayeeType)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from Payments p
                      inner join PaymentDetails pd on pd.PaymentID = p.PaymentID
	                  inner join BankAndCashAccounts b on b.AccountName = pd.BankAccountCode
					  where PayeeType='" + PayeeType + "' order by p.PaymentID desc"
            );
        }
        public DataTable GetPayments(int EmployeeID,int dummy)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from Payments p
                      inner join PaymentDetails pd on pd.PaymentID = p.PaymentID
	                  inner join BankAndCashAccounts b on b.AccountName = pd.BankAccountCode
					  where PayeeId=" + EmployeeID + " order by p.PaymentID desc"
            );
        }
        public DataTable GetPOPayments(int PaymentID)
        {
            return DbHelper.ExecuteQuery(
                @"select VoucherCount,VoucherNumber,PayeeName,PaymentDate,PaymentAmount,
                    p.paymentid,InvoiceNumber,p.PONumber,InvoiceDate,p.TotalAmount,
                    0 as Discounts,previouspayments from Payments p
                    inner join PurchaseOrder po on po.PurchaseOrderID = p.ID
                    inner join VendorDetails v on v.VendorID = po.VendorID
					left join InvoiceAP ap on ap.PurchaseOrderID = po.PurchaseOrderID
                    where p.paymentid=" + PaymentID
            );
        }
        public DataTable GetPRPayments(int PaymentID)
        {
            return DbHelper.ExecuteQuery(
                @"select distinct VoucherCount,VoucherNumber,PayeeName,PaymentDate,PaymentAmount,
                    p.PONumber,p.TotalAmount,
                    0 as Discounts,previouspayments,ItemDescription,totalprice,
                    pd.COAAccountCode,pd.BankAccountCode, AccountName, pd.Amount
					from Payments p
					inner join paymentRequisitionmaster po on po.PaymentRequisitionID = p.ID 
					inner join PaymentRequisitionDetails pdd on pdd.PaymentRequisitionID = po.PaymentRequisitionID
					inner join PaymentDetails pd on pd.id = pdd.PaymentRequisitionDetailsID                 
                    inner join ChartOfAccountsDetail cd on cd.AccountCode = pd.COAAccountCode
					where p.paymentid=" + PaymentID
            );
        }
        public DataTable GetPayments(int PurchaseOrderID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT 
                        SUM(PaymentAmount) AS TotalPayments
                    FROM 
                        Payments
                    WHERE PayeeType='Vendor' and ID= " + PurchaseOrderID +
                @" GROUP BY 
                        ID;");
        }
        public static DataTable GetPendingInvoices(string VendorID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT i.InvoiceId, v.VendorName, i.TotalAmount, i.DueDate 
              FROM InvoiceAP i
              JOIN VendorDetails v ON i.VendorId = v.VendorId
               where v.vendorid='" + VendorID + "' and Status=1"
            );
        }
        public static DataTable GetPendingPurchaseInvoices(string VendorID)
        {
            return DbHelper.ExecuteQuery(
                                    @"SELECT DISTINCT
                           po.PurchaseOrderID,
                           po.PONumber
                    FROM PurchaseOrder po
                    INNER JOIN TransactionMaster t
                        ON t.PurchaseOrderID = po.PurchaseOrderID
                    WHERE t.InwardID NOT IN (
                        SELECT CAST(value AS INT)
                        FROM InvoiceAP
                        CROSS APPLY STRING_SPLIT(GRNID, ',')
                        WHERE GRNID IS NOT NULL) and t.GRNStatus=13 and po.VendorID=" + VendorID
            ) ;
        }
        public int AddInvoice(DAL.APModule obj)
        {
            int prevIdentity = _cm.GetReseedIdentity("InvoiceAP", "InvoiceID");
            _cm.ReseedIdentity("InvoiceAP", prevIdentity);

            int InvoiceID = 0;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                try
                {
                    // Insert master
                    string sql = @"INSERT INTO InvoiceAP (VendorId," +
                                        "VendorTaxInvoiceNumber," +
                                        "InvoiceNumber," +
                                        "InvoiceDate," +
                                        "DueDate," +
                                        "CurrencyCode," +
                                        "TotalAmount," +
                                        "Status," +
                                        "PurchaseOrderID," +
                                        "AmountPaid," +
                                        "PIYear," +
                                        "PICount," +
                                        "PINumber,GRNID) OUTPUT INSERTED.InvoiceID VALUES (@VendorId," +
                                        "@VendorTaxInvoiceNumber," +
                                        "@InvoiceNumber," +
                                        "@InvoiceDate," +
                                        "@DueDate," +
                                        "@CurrencyCode," +
                                        "@TotalAmount," +
                                        "@Status," +
                                        "@PurchaseOrderID," +
                                        "@AmountPaid," +
                                        "@PIYear," +
                                        "@PICount," +
                                        "@PINumber,@GRNID)";
                    SqlParameter[] param = {
                        new SqlParameter("@VendorId", obj.VendorID),
                        new SqlParameter("@VendorTaxInvoiceNumber", obj.VendorTaxInvoiceNumber),
                        new SqlParameter("@InvoiceNumber", obj.InvoiceNumber),
                        new SqlParameter("@InvoiceDate", obj.InvoiceDate),
                        new SqlParameter("@DueDate", obj.DueDate),
                        new SqlParameter("@CurrencyCode", obj.CurrencyCode),
                        new SqlParameter("@TotalAmount", obj.TotalAmount),
                        new SqlParameter("@Status", obj.Status),
                        new SqlParameter("@PurchaseOrderID", obj.PurchaseOrderID),
                        new SqlParameter("@AmountPaid", obj.AmountPaid),
                        new SqlParameter("@PIYear", obj.Year),
                        new SqlParameter("@PICount", obj.Counter),
                        new SqlParameter("@PINumber", obj.Number),
                        new SqlParameter("@GRNID", obj.GRNIDs)
                    };
                    InvoiceID = Convert.ToInt32(DbHelper.ExecuteInsertWithReturn(sql, param));
                    
                }
                catch
                {
                    throw;
                }
            }
            return InvoiceID;
        }
        public static DataTable GetPO(string PurchaseOrderID)
        {
            return DbHelper.ExecuteQuery(
                @"Select v.VendorID,VendorName,p.PurchaseOrderID,PONumber,PODate,GrandTotal,
                    ap.InvoiceNumber,payment,DATEADD(day, DayCount, PODate) AS DueDate  from PurchaseOrder p
                    inner join VendorDetails v on v.VendorID = p.VendorID
					left join InvoiceAP ap on ap.PurchaseOrderID = p.PurchaseOrderID
                    inner join PaymentTerms pt on pt.PaymentTermName = p.Payment
                    where p.PurchaseOrderID='" + PurchaseOrderID + "'");
        }
        public int AddPayment(DAL.APModule obj, DataTable dt)
        {
            int newId = 0;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // Insert master
                    string masterSql =
                @"INSERT INTO Payments (PayeeId,
                                PaymentDate,
                                PaymentAmount,
                                PreviousPayments,
                                PaymentMethod,
                                ReferenceNo,
                                Notes,
                                CreatedOn,
                                PayeeType,
                                PayeeName,
                                ID,
                                VoucherYear,
                                VoucherCount,
                                VoucherNumber,
                                PONumber, 
                                SupplierInvoiceNo,
                                TotalAmount,
                                BalanceAmount
                                ) 
                                OUTPUT INSERTED.PaymentID    
                                VALUES (@PayeeId,
                                @PaymentDate,
                                @PaymentAmount,
                                @PreviousPayments,
                                @PaymentMethod,
                                @ReferenceNo,
                                @Notes,
                                @CreatedOn,
                                @PayeeType,
                                @PayeeName,
                                @ID,
                                @VoucherYear,
                                @VoucherCount,
                                @VoucherNumber,
                                @PONumber,
                                @SupplierInvoiceNo,
                                @TotalAmount,
                                @BalanceAmount)";
                    SqlCommand cmd = new SqlCommand(masterSql, con, trans);
                    cmd.Parameters.AddWithValue("@PayeeId", obj.PayeeId);
                    cmd.Parameters.AddWithValue("@PaymentDate", obj.PaymentDate);
                    cmd.Parameters.AddWithValue("@PaymentAmount", obj.PaymentAmount);
                    cmd.Parameters.AddWithValue("@PreviousPayments", obj.PrevPaymentAmount);
                    cmd.Parameters.AddWithValue("@PaymentMethod", obj.PaymentMethod);
                    cmd.Parameters.AddWithValue("@ReferenceNo", obj.ReferenceNo);
                    cmd.Parameters.AddWithValue("@Notes", obj.Notes);
                    cmd.Parameters.AddWithValue("@CreatedOn", obj.CreatedOn);
                    cmd.Parameters.AddWithValue("@PayeeType", obj.PayeeType);
                    cmd.Parameters.AddWithValue("@PayeeName", obj.PayeeName);
                    cmd.Parameters.AddWithValue("@ID", obj.PurchaseOrderID);//PaymentRequisitionID
                    cmd.Parameters.AddWithValue("@VoucherYear", obj.Year);
                    cmd.Parameters.AddWithValue("@VoucherCount", obj.Counter);
                    cmd.Parameters.AddWithValue("@VoucherNumber", obj.Number);
                    cmd.Parameters.AddWithValue("@PONumber", obj.PONumber);//RequisitionNumber
                    cmd.Parameters.AddWithValue("@SupplierInvoiceNo", obj.SupplierInvoiceNo);
                    cmd.Parameters.AddWithValue("@TotalAmount", obj.TotalAmount);
                    cmd.Parameters.AddWithValue("@BalanceAmount", obj.BalanceAmount);
                    newId = (int)cmd.ExecuteScalar();

                    // Insert detail rows
                    foreach (DataRow d in dt.Rows)
                    {
                        string detailSql = @"INSERT INTO PaymentDetails
                                    (PaymentId,
                                    COAAccountCode,
                                    BankAccountCode,
                                    Amount,
                                    CreatedDate, ID
                                    )
                                    VALUES (@PaymentId,
                                    @COAAccountCode,
                                    @BankAccountCode,
                                    @Amount,
                                    @CreatedDate,@ID)";

                        SqlCommand cmdDetail = new SqlCommand(detailSql, con, trans);
                        cmdDetail.Parameters.AddWithValue("@PaymentId", newId);
                        cmdDetail.Parameters.AddWithValue("@COAAccountCode", d["COAAccountCode"].ToString());
                        cmdDetail.Parameters.AddWithValue("@BankAccountCode", d["BankAccountCode"].ToString());
                        cmdDetail.Parameters.AddWithValue("@Amount", d["Amount"].ToString());
                        cmdDetail.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmdDetail.Parameters.AddWithValue("@ID", d["PaymentRequisitionDetailsID"]);
                        cmdDetail.ExecuteNonQuery();
                    }

                    trans.Commit();


                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            return newId;
        }

        public void UpdatePOAmountPaid(DAL.APModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update PurchaseOrder set AmountPaid=" + obj.AmountPaid + " where PurchaseOrderID=" + obj.PurchaseOrderID);
            DbHelper.ExecuteNonQuery(
                @"Update InvoiceAP set AmountPaid=" + obj.AmountPaid + " where PurchaseOrderID=" + obj.PurchaseOrderID);
        }
        public void UpdatePRAmountPaid(DAL.APModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update PaymentRequisitionmaster set StatusID=1017, AmountPaid=" + obj.prVar.AmountPaid + " where PaymentRequisitionID=" + obj.prVar.PaymentRequisitionID);

        }
        public DataTable GenerateDebitNoteNo()
        {
            return DbHelper.ExecuteQuery("SELECT ISNULL(MAX(DebitNoteID),0) + 1 FROM DebitNoteMaster");
        }
        public int SaveDebitNote(DebitNoteMaster master, List<DebitNoteDetail> details)
        {
            int newId = 0;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // Insert master
                    string masterSql = @"INSERT INTO DebitNoteMaster
                                 (DebitNoteNo, DebitNoteDate, SupplierID, TotalAmount, Remarks, CreatedBy)
                                 OUTPUT INSERTED.DebitNoteID
                                 VALUES (@No, @Date, @Supplier, @Total, @Remarks, @User)";

                    SqlCommand cmd = new SqlCommand(masterSql, con, trans);
                    cmd.Parameters.AddWithValue("@No", master.DebitNoteNo);
                    cmd.Parameters.AddWithValue("@Date", master.DebitNoteDate);
                    cmd.Parameters.AddWithValue("@Supplier", master.SupplierID);
                    cmd.Parameters.AddWithValue("@Total", master.TotalAmount);
                    cmd.Parameters.AddWithValue("@Remarks", master.Remarks);
                    cmd.Parameters.AddWithValue("@User", master.CreatedBy);

                    newId = (int)cmd.ExecuteScalar();

                    // Insert detail rows
                    foreach (var d in details)
                    {
                        string detailSql = @"INSERT INTO DebitNoteDetails
                                    (DebitNoteID, ItemID,ItemName, Qty, Rate, Amount)
                                    VALUES (@ID, @Item,@ItemName, @Qty, @Rate, @Amount)";

                        SqlCommand cmdDetail = new SqlCommand(detailSql, con, trans);
                        cmdDetail.Parameters.AddWithValue("@ID", newId);
                        cmdDetail.Parameters.AddWithValue("@Item", d.ItemID);
                        cmdDetail.Parameters.AddWithValue("@ItemName", d.ItemName);
                        cmdDetail.Parameters.AddWithValue("@Qty", d.Qty);
                        cmdDetail.Parameters.AddWithValue("@Rate", d.Rate);
                        cmdDetail.Parameters.AddWithValue("@Amount", d.Amount);

                        cmdDetail.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            return newId;
        }
        public DataTable GetDebitNotes()
        {
            return DbHelper.ExecuteQuery("SELECT * FROM DebitNoteMaster dm inner join VendorDetails v on v.VendorID=dm.SupplierID");
        }
        public DataTable GetDebitNotes(string DebitNoteNo)
        {
            return DbHelper.ExecuteQuery("SELECT * FROM DebitNoteMaster dm inner join VendorDetails v on v.VendorID=dm.SupplierID where DebitNoteNo='" + DebitNoteNo + "'");
        }
        public DataTable GetDebitNoteDetails(string DebitNoteID)
        {
            return DbHelper.ExecuteQuery("SELECT * FROM DebitNoteMaster dm inner join DebitNoteDetails dd on dd.DebitNoteID=dm.DebitNoteID where dd.DebitNoteID='" + DebitNoteID + "'");
        }
        public DataTable GetPaymentRequisition()
        {
            return DbHelper.ExecuteQuery("SELECT * FROM PaymentRequisitionMaster dm inner join PaymentRequisitionDetails v on v.PaymentRequisitionID=dm.PaymentRequisitionID");
        }
        public DataTable GetPaymentRequisition(string PaymentRequisitionNumber)
        {
            return DbHelper.ExecuteQuery("SELECT * FROM PaymentRequisitionMaster dm inner join PaymentRequisitionDetails v on v.PaymentRequisitionID=dm.PaymentRequisitionID where PaymentRequisitionNumber='" + PaymentRequisitionNumber + "'");
        }
        public DataTable GetPaymentRequisition(int PaymentRequisitionID)
        {
            return DbHelper.ExecuteQuery(@"SELECT *,dd.TotalPrice as Amount FROM PaymentRequisitionMaster dm 
                                            inner join Employees e on e.ID = dm.RequisitionBy
                                            inner join PaymentRequisitionDetails dd on dd.PaymentRequisitionID=dm.PaymentRequisitionID "+
                                          @" where dd.PaymentRequisitionID = '" + PaymentRequisitionID + "'");
        }
        public DataTable GetRequisitionPayment(int PaymentRequisitionID)
        {
            return DbHelper.ExecuteQuery(@"select * from PaymentDetails pd
                            inner join Payments p on p.PaymentId = pd.PaymentId
                            where pd.ID="+ PaymentRequisitionID + " and p.PayeeType='Employee'");
        }
        public DataTable GetEmployeePaymentRequisition(string EmployeeID)
        {
            return DbHelper.ExecuteQuery(@"SELECT * FROM PaymentRequisitionMaster dm 
                                            inner join Employees e on e.ID = dm.RequisitionBy
                                            inner join PaymentRequisitionDetails dd on dd.PaymentRequisitionID=dm.PaymentRequisitionID 
                                            where e.ID='" + EmployeeID + "'");
        }
        public DataTable GetEmployees()
        {
            return DbHelper.ExecuteQuery("SELECT ID, EmployeeName FROM Employees order by EmployeeName asc");
        }
        public void UpdateJournalID(DAL.APModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update Payments set JournalID=" + obj.JournalId + " where PaymentsID=" + obj.PaymentID);
        }
        public void UpdateVoucherID(DAL.APModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update Payments set VoucherID=" + obj.VoucherID + " where PaymentID=" + obj.PaymentID);
        }
        public DataTable GetBankFromCode(string AccountCode)
        {
            return DbHelper.ExecuteQuery(
                @"select * from BankAccount where accountcode='" + AccountCode + "'");
        }
        public void UpdatePaymentRequisitionPayment(DataTable dt)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                    {
                        conn.Open();
                        foreach (DataRow dr in dt.Rows)
                        {
                            string updateQuery = @"UPDATE PaymentRequisitionDetails 
                                                    SET 
                                                    StatusID = @StatusID,
                                                    COAAccountIDDr = @COAAccountIDDr,
                                                    COAAccountCode = @COAAccountCode,
                                                    COAAccountNameDr = @COAAccountNameDr,
                                                    COAAccountIDCr = @COAAccountIDCr,
                                                    COAAccountCodeCr = @COAAccountCodeCr,
                                                    COAAccountNameCr = @COAAccountNameCr,
                                                    BankAccountID = @BankAccountID,
                                                    BankAccountCode = @BankAccountCode,
                                                    BankAccountName = @BankAccountName                                           
                                                    WHERE PaymentRequisitionDetailsID = @PaymentRequisitionDetailsID";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@COAAccountIDDr", dr["COAAccountIDDr"].ToString());
                                cmd.Parameters.AddWithValue("@COAAccountCode", dr["COAAccountCode"].ToString());
                                cmd.Parameters.AddWithValue("@COAAccountNameDr", dr["COAAccountNameDr"].ToString());
                                cmd.Parameters.AddWithValue("@COAAccountIDCr", dr["COAAccountIDCr"].ToString());
                                cmd.Parameters.AddWithValue("@COAAccountCodeCr", dr["COAAccountCodeCr"].ToString());
                                cmd.Parameters.AddWithValue("@COAAccountNameCr", dr["COAAccountNameCr"].ToString());
                                cmd.Parameters.AddWithValue("@BankAccountID", dr["BankAccountID"].ToString());
                                cmd.Parameters.AddWithValue("@BankAccountCode", dr["BankAccountCode"].ToString());
                                cmd.Parameters.AddWithValue("@BankAccountName", dr["BankAccountName"].ToString());
                                cmd.Parameters.AddWithValue("@StatusID", 1017);//Paid in PurchaseRequisitionStatus
                                cmd.Parameters.AddWithValue("@PaymentRequisitionDetailsID", dr["PaymentRequisitionDetailsID"]);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

}
