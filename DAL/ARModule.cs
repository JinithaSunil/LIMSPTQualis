using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using DocumentFormat.OpenXml.Math;
using LIMS.DAL;

namespace LIMS.DAL
{
    public class ARModule
    {
        DAL.CM _cm = new DAL.CM();
        public class AdvancePayment
        {
            public int QuotationID { get; set; }
            public string QuotationNo { get; set; }
            public DateTime QuotationDate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal BalanceAmount { get; set; }
        }
        public class InvoicePayment
        {
            public string InvoiceARID { get; set; }
            public int InvoiceID { get; set; }
            public string InvoiceNo { get; set; }
            public DateTime InvoiceDate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal BalanceAmount { get; set; }
        }
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        int _CustomerID; public int CustomerID { get { return _CustomerID; } set { _CustomerID = value; } }
        DateTime _InvoiceDate; public DateTime InvoiceDate { get { return _InvoiceDate; } set { _InvoiceDate = value; } }
        DateTime _DueDate; public DateTime DueDate { get { return _DueDate; } set { _DueDate = value; } }
        string _CurrencyCode; public string CurrencyCode { get { return _CurrencyCode; } set { _CurrencyCode = value; } }
        decimal _TotalAmount; public decimal TotalAmount { get { return _TotalAmount; } set { _TotalAmount = value; } }
        string _ReceiptType; public string ReceiptType { get { return _ReceiptType; } set { _ReceiptType = value; } }
        string _InvoiceARID; public string InvoiceARID { get { return _InvoiceARID; } set { _InvoiceARID = value; } }
        string _InvoiceID; public string InvoiceID { get { return _InvoiceID; } set { _InvoiceID = value; } }
        string _QuotationID; public string QuotationID { get { return _QuotationID; } set { _QuotationID = value; } }
        decimal _AmountPaid; public decimal AmountPaid { get { return _AmountPaid; } set { _AmountPaid = value; } }
        string _Status; public string Status { get { return _Status; } set { _Status = value; } }
        int _PayerId; public int PayerId { get { return _PayerId; } set { _PayerId = value; } }
        DateTime _PaymentDate; public DateTime PaymentDate { get { return _PaymentDate; } set { _PaymentDate = value; } }
        decimal _PaymentAmount; public decimal PaymentAmount { get { return _PaymentAmount; } set { _PaymentAmount = value; } }
        int _ReceiptID; public int ReceiptID { get { return _ReceiptID; } set { _ReceiptID = value; } }
        DateTime _ReceiptDate; public DateTime ReceiptDate { get { return _ReceiptDate; } set { _ReceiptDate = value; } }
        decimal _ReceiptAmount; public decimal ReceiptAmount { get { return _ReceiptAmount; } set { _ReceiptAmount = value; } }
        int _VoucherID; public int VoucherID { get { return _VoucherID; } set { _VoucherID = value; } }
        string _VoucherNo; public string VoucherNo { get { return _VoucherNo; } set { _VoucherNo = value; } }
        decimal _BalanceDue; public decimal BalanceDue { get { return _BalanceDue; } set { _BalanceDue = value; } }
        string _PaymentMethod; public string PaymentMethod { get { return _PaymentMethod; } set { _PaymentMethod = value; } }
        string _BankAccountCode; public string BankAccountCode { get { return _BankAccountCode; } set { _BankAccountCode = value; } }
        string _ReferenceNo; public string ReferenceNo { get { return _ReferenceNo; } set { _ReferenceNo = value; } }
        string _Notes; public string Notes { get { return _Notes; } set { _Notes = value; } }
        int _JournalId; public int JournalId { get { return _JournalId; } set { _JournalId = value; } }
        DateTime _CreatedOn; public DateTime CreatedOn { get { return _CreatedOn; } set { _CreatedOn = value; } }
        string _PayerType; public string PayerType { get { return _PayerType; } set { _PayerType = value; } }
        string _PayerName; public string PayerName { get { return _PayerName; } set { _PayerName = value; } }
        string _PaymentMode; public string PaymentMode { get { return _PaymentMode; } set { _PaymentMode = value; } }
        string _Number; public string Number { get { return _Number; } set { _Number = value; } }
        int _Counter; public int Counter { get { return _Counter; } set { _Counter = value; } }
        string _Year; public string Year { get { return _Year; } set { _Year = value; } }
        public struct ARVariables
        {
            public string InvoiceARID { get; set; }
            public string customerID { get; set; }
            public string InvoiceID { get; set; }
            public string InvoiceNumber { get; set; }
            public string PayeeType { get; set; }
            public string PayerName { get; set; }
            public DateTime InvoiceDate { get; set; }
            public decimal InvoiceAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public string Reference { get; set; }
            public string QuotationNumber { get; set; }
        }
        public ARVariables arVar;
        public DataTable GetCustomers()
        {
            return DbHelper.ExecuteQuery("SELECT CompanyId,CompanyName FROM CompanyDetails order by companyname asc");
        }
        public static DataTable getPendingARCustomers()
        {
            return DbHelper.ExecuteQuery(@"Select distinct c.CustomerMasterId, CompanyName from Invoices i
                                            inner join CompanyDetails c on c.CustomerMasterId = i.CompanyID
                                            where AmountPaid < TotalAmount order by CompanyName asc");
        }
        public static DataTable getARCustomers(string filter)
        {
            string sQuery = "";
            switch (filter)
            {
                case "1"://Invoiced - Unpaid
                    sQuery = @"Select distinct c.CustomerMasterId, CompanyName from Invoices i
                                inner join CompanyDetails c on c.CustomerMasterId = i.CompanyID
                                where AmountPaid < TotalAmount order by CompanyName asc";
                    break;
                case "2"://Invoiced - Paid
                    sQuery = @"Select distinct c.CustomerMasterId, CompanyName from Invoices i
                                inner join CompanyDetails c on c.CustomerMasterId = i.CompanyID
                                where AmountPaid >= TotalAmount order by CompanyName asc";
                    break;
                case "3"://UnInvoiced
                    break;
                default:
                    break;
            }

            return DbHelper.ExecuteQuery(sQuery);
        }

        public static DataTable GetInvoice(string InvoiceID)
        {
            return DbHelper.ExecuteQuery(
                @"Select i.CompanyID, CompanyName, InvoiceId,InvoiceNumber, InvoiceDate, Totalamount from Invoices i
                    inner join CompanyDetails c on c.CustomerMasterId = i.CompanyID
                    where i.InvoiceId='" + InvoiceID + "'"
            );
        }
        //public static DataTable GetInvoices(string filter)
        //{
        //    string sQuery = "";
        //    switch (filter)
        //    {
        //        case "1":
        //            sQuery = @"select i.InvoiceId, i.InvoiceDate,i.DueDate, i.InvoiceNumber, c.companyID,CompanyName, i.TotalAmount, i.status, ir.AmountPaid from Invoices i
        //                        inner join CompanyDetails c on c.CustomerMasterID = i.companyID
        //                        inner join InvoiceAR ir on ir.InvoiceID = i.invoiceID
        //                        inner join QuotationMaster q on q.QuotationMasterID = i.QuotationID
        //                        inner join CustomerInquiry ci on ci.InquiryID=q.EnquiryID
        //                        where isnull(ir.amountPaid,0) < i.TotalAmount";
        //            break;
        //        case "2":
        //            sQuery = @"select i.InvoiceId, i.InvoiceDate,i.DueDate, i.InvoiceNumber,c.companyID, CompanyName, i.TotalAmount, i.status, ir.AmountPaid from Invoices i
        //                        inner join CompanyDetails c on c.CustomerMasterID = i.companyID
        //                        inner join InvoiceAR ir on ir.InvoiceID = i.invoiceID
        //                        inner join QuotationMaster q on q.QuotationMasterID = i.QuotationID
        //                        inner join CustomerInquiry ci on ci.InquiryID=q.EnquiryID
        //         where isnull(ir.amountPaid,0) >= i.TotalAmount";
        //            break;
        //        case "3":
        //            sQuery = @"select i.InvoiceId, i.InvoiceDate, i.InvoiceNumber, c.companyID,CompanyName, i.TotalAmount, i.status, ir.AmountPaid from Invoices i
        //                        inner join CompanyDetails c on c.CustomerMasterID = i.companyID
        //                        inner join InvoiceAR ir on ir.InvoiceID = i.invoiceID
        //                        inner join QuotationMaster q on q.QuotationMasterID = i.QuotationID
        //                        inner join CustomerInquiry ci on ci.InquiryID=q.EnquiryID
        //         where ir.status is null";
        //            break;
        //        default:
        //            break;
        //    }
        //    return DbHelper.ExecuteQuery(sQuery);
        //}

        public static DataTable GetInvoices(string filter)
        {
            string sQuery = "";
            switch (filter)
            {
                case "1"://Invoiced - Unpaid
                    sQuery = @"SELECT
                                ir.InvoiceARId,ir.InvoiceID,
                                ir.CustomerId,CompanyName,
                                SUM(i.TotalAmount) AS TotalAmount,
                                ISNULL(ir.AmountPaid, 0) AS AmountPaid,
                                SUM(i.TotalAmount) - ISNULL(ir.AmountPaid, 0) AS BalanceAmount
                            FROM InvoiceAR ir
                            CROSS APPLY STRING_SPLIT(ir.InvoiceID, ',') s
                            INNER JOIN Invoices i 
                                ON i.InvoiceID = CAST(s.value AS INT)
                            INNER JOIN CompanyDetails c 
                                ON c.CustomerMasterID = i.CompanyID
                            INNER JOIN QuotationMaster q 
                                ON q.QuotationMasterID = i.QuotationID
                            INNER JOIN CustomerInquiry ci 
                                ON ci.InquiryID = q.EnquiryID
                            GROUP BY
                                ir.InvoiceARId,ir.InvoiceID,CompanyName,
                                ir.CustomerId,
                                ir.AmountPaid
                            HAVING
                                SUM(i.TotalAmount) > ISNULL(ir.AmountPaid, 0)";
                    break;
                case "2"://Invoiced - Paid
                    sQuery = @"SELECT
                                ir.InvoiceARId,ir.InvoiceID,
                                ir.CustomerId,CompanyName,
                                SUM(i.TotalAmount) AS TotalAmount,
                                ISNULL(ir.AmountPaid, 0) AS AmountPaid,
                                SUM(i.TotalAmount) - ISNULL(ir.AmountPaid, 0) AS BalanceAmount
                            FROM InvoiceAR ir
                            CROSS APPLY STRING_SPLIT(ir.InvoiceID, ',') s
                            INNER JOIN Invoices i 
                                ON i.InvoiceID = CAST(s.value AS INT)
                            INNER JOIN CompanyDetails c 
                                ON c.CustomerMasterID = i.CompanyID
                            INNER JOIN QuotationMaster q 
                                ON q.QuotationMasterID = i.QuotationID
                            INNER JOIN CustomerInquiry ci 
                                ON ci.InquiryID = q.EnquiryID
                            GROUP BY
                                ir.InvoiceARId,ir.InvoiceID,CompanyName,
                                ir.CustomerId,
                                ir.AmountPaid
                            HAVING
                                SUM(i.TotalAmount) <= ISNULL(ir.AmountPaid, 0)";
                    break;
                case "3"://UnInvoiced
                    sQuery = @"SELECT
                                i.InvoiceID as InvoiceARId,i.InvoiceID,i.InvoiceNumber,
                                i.CompanyID as CustomerId,CompanyName,
                                SUM(i.TotalAmount) AS TotalAmount,
                                ISNULL(ir.AmountPaid, 0) AS AmountPaid,
                                SUM(i.TotalAmount) - ISNULL(ir.AmountPaid, 0) AS BalanceAmount
                            FROM InvoiceAR ir
                            CROSS APPLY STRING_SPLIT(ir.InvoiceID, ',') s
                            RIGHT JOIN Invoices i 
                                ON i.InvoiceID = CAST(s.value AS INT)
                            INNER JOIN CompanyDetails c 
                                ON c.CustomerMasterID = i.CompanyID
                            INNER JOIN QuotationMaster q 
                                ON q.QuotationMasterID = i.QuotationID
                            INNER JOIN CustomerInquiry ci 
                                ON ci.InquiryID = q.EnquiryID
							WHERE ir.InvoiceARId is null
                            GROUP BY InvoiceARId,
                                i.InvoiceID,i.InvoiceNumber,CompanyName,
                                i.CompanyID,
                                ir.AmountPaid";
                    break;
                default:
                    break;
            }
            return DbHelper.ExecuteQuery(sQuery);
        }
        public static DataTable GetPendingInvoices(string CustomerID)
        {
            return DbHelper.ExecuteQuery(
                 @"SELECT i.InvoiceID, i.InvoiceNumber
                            FROM Invoices i
                            WHERE i.CompanyID = '" + CustomerID + "'" +
                            @" AND i.Status = 1
                              AND NOT EXISTS (
                                    SELECT 1
                                    FROM InvoiceAR ar
                                    CROSS APPLY STRING_SPLIT(ar.InvoiceID, ',') s
                                    WHERE TRY_CAST(s.value AS INT) = i.InvoiceID
                              )"
             );
        }

        public static DataTable GetPendingQuotations(string CustomerID)
        {
            return DbHelper.ExecuteQuery(
                 @"SELECT QuotationMasterID,QuotationNumber
                        FROM QuotationMaster q
                        WHERE QuotationMasterID not in (
                        SELECT QuotationID
                        FROM Invoices where CompanyID='"+ CustomerID + "') and customerMasterID='" + CustomerID + "'"           
             );
        }
        public DataTable GetReceipts()
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from Receipts r
	                  inner join BankAccount b on b.AccountCode = r.BankAccountCode
	                  order by ReceiptID desc"
            );
        }
        public DataTable GetReceipts(string InvoiceARID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from Receipts r
                    inner join InvoiceAR i on r.InvoiceARId = i.InvoiceARId
                    where r.InvoiceARId='"+ InvoiceARID + "' order by ReceiptID desc");
        }
        public DataTable GetInvoiceDetails(int InvoiceID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from Invoices where InvoiceID=" + InvoiceID
            );
        }
        public List<InvoicePayment> GetInvoiceDetails(string invoiceIds)
        {
            List<InvoicePayment> invoices = new List<InvoicePayment>();

            string connStr = ConfigurationManager
                                .ConnectionStrings["CM"]
                                .ConnectionString;

            string sql = @"	SELECT
								InvoiceID,
								InvoiceNumber,
								InvoiceDate,
								TotalAmount,
								ISNULL(AmountPaid, 0) AS AmountPaid
							FROM Invoices
							WHERE InvoiceID IN (
                            SELECT value FROM STRING_SPLIT(@InvoiceIDs, ',')
                        )
                        ORDER BY InvoiceDate ";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InvoiceIDs", invoiceIds);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        decimal total = Convert.ToDecimal(dr["TotalAmount"]);
                        decimal paid = Convert.ToDecimal(dr["AmountPaid"]);

                        invoices.Add(new InvoicePayment
                        {
                            InvoiceID = Convert.ToInt32(dr["InvoiceID"]),
                            InvoiceNo = dr["InvoiceNumber"].ToString(),
                            InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]),
                            TotalAmount = total,
                            PaidAmount = paid,
                            BalanceAmount = total - paid
                        });
                    }
                }
            }

            return invoices;
        }

        public DataTable GetQuotationDetails(int QuotationID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from QuotationMaster where QuotationMasterID=" + QuotationID
            );
        }
        public List<AdvancePayment> GetQuotationDetails(string QuotationID)
        {
            List<AdvancePayment> quotation = new List<AdvancePayment>();

            string connStr = ConfigurationManager
                                .ConnectionStrings["CM"]
                                .ConnectionString;

            string sql = @"	SELECT
								QuotationMasterID,
								QuotationNumber,
								QuotationDate,
								GrandTotal as TotalAmount,
								ISNULL(AmountPaid, 0) AS AmountPaid
							FROM QuotationMaster
							WHERE QuotationMasterID =@QuotationID";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@QuotationID", QuotationID);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        decimal total = Convert.ToDecimal(dr["TotalAmount"]);
                        decimal paid = Convert.ToDecimal(dr["AmountPaid"]);

                        quotation.Add(new AdvancePayment
                        {
                            QuotationID = Convert.ToInt32(dr["QuotationMasterID"]),
                            QuotationNo = dr["QuotationNumber"].ToString(),
                            QuotationDate = Convert.ToDateTime(dr["QuotationDate"]),
                            TotalAmount = total,
                            PaidAmount = paid,
                            BalanceAmount = total - paid
                        });
                    }
                }
            }

            return quotation;
        }
        public DataTable GetReceipts(int ReceiptID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT * from Receipts where ReceiptID=" + ReceiptID
            );
        }
        public string AddInvoice(DAL.ARModule obj)
        {
            int prevIdentity = _cm.GetReseedIdentity("InvoiceAR", "InvoiceARID");
            _cm.ReseedIdentity("InvoiceAR", prevIdentity);
            string sql = @"
                INSERT INTO InvoiceAR (CustomerID," +
                "CurrencyCode," +
                "TotalAmount," +
                "Status," +
                "InvoiceID," +
                "AmountPaid) OUTPUT INSERTED.InvoiceARID VALUES (@CustomerId," +
                "@CurrencyCode," +
                "@TotalAmount," +
                "@Status," +
                "@InvoiceID," +
                "@AmountPaid)";
            SqlParameter[] param = {
                new SqlParameter("@CustomerID", obj.CustomerID),
                new SqlParameter("@CurrencyCode", obj.CurrencyCode),
                new SqlParameter("@TotalAmount", obj.TotalAmount),
                new SqlParameter("@Status", obj.Status),
                new SqlParameter("@InvoiceID", obj.InvoiceID),
                new SqlParameter("@AmountPaid", obj.AmountPaid) };
            int InvoiceARID = Convert.ToInt32(DbHelper.ExecuteInsertWithReturn(sql, param));
            return InvoiceARID.ToString();
        }
        public DataTable GetLast(string table, string field, string Year)
        {
            return DbHelper.ExecuteQuery(@"Select * from " + table + " where " + field + "='" + Year + "'");
        }
        public int AddReceipt(DAL.ARModule obj, List<InvoicePayment> invoices)
        {
            int prevIdentity = _cm.GetReseedIdentity("Receipts", "ReceiptID");
            _cm.ReseedIdentity("Receipts", prevIdentity);
            string sql = @"INSERT INTO Receipts (PayerId,
                                ReceiptDate,
                                ReceiptAmount,
                                PaymentMethod,
                                BankAccountCode,
                                ReferenceNo,
                                Notes,
                                CreatedOn,
                                PayerType,
                                PayerName,
                                InvoiceID,VoucherYear,VoucherCount,VoucherNumber,InvoiceARID,ReceiptType
                                ) OUTPUT INSERTED.ReceiptId VALUES (@PayerId,
                                @ReceiptDate,
                                @ReceiptAmount,
                                @PaymentMethod,
                                @BankAccountCode,
                                @ReferenceNo,
                                @Notes,
                                @CreatedOn,
                                @PayerType,
                                @PayerName,
                                @InvoiceID,@VoucherYear,@VoucherCount,@VoucherNumber,@InvoiceARID,@ReceiptType)";
            SqlParameter[] param = {
                                    new SqlParameter("@PayerId", obj.PayerId),
                                    new SqlParameter("@ReceiptDate", obj.ReceiptDate),
                                    new SqlParameter("@ReceiptAmount", obj.ReceiptAmount),
                                    new SqlParameter("@PaymentMethod", obj.PaymentMethod),
                                    new SqlParameter("@BankAccountCode", obj.BankAccountCode),
                                    new SqlParameter("@ReferenceNo", obj.ReferenceNo),
                                    new SqlParameter("@Notes", obj.Notes),
                                    new SqlParameter("@CreatedOn", obj.CreatedOn),
                                    new SqlParameter("@PayerType", obj.PayerType),
                                    new SqlParameter("@PayerName", obj.PayerName),
                                    new SqlParameter("@InvoiceID", obj.InvoiceID),
                                    new SqlParameter("@VoucherYear", obj.Year),
                                    new SqlParameter("@VoucherCount", obj.Counter),
                                    new SqlParameter("@VoucherNumber", obj.Number),
                                    new SqlParameter("@InvoiceARID", obj.InvoiceARID),
                                    new SqlParameter("@ReceiptType", obj.ReceiptType)
                                };
            int receiptId = Convert.ToInt32(DbHelper.ExecuteInsertWithReturn(sql, param));
            decimal remainingAmount = obj.ReceiptAmount;

            foreach (var inv in invoices.OrderBy(x => x.InvoiceDate))
            {
                if (remainingAmount <= 0)
                    break;

                decimal paidAmount = Math.Min(inv.BalanceAmount, remainingAmount);

                // Insert Receipt Detail
                InsertReceiptDetail(
                    receiptId,
                    inv.InvoiceID,
                    inv.TotalAmount,
                    paidAmount,
                    inv.BalanceAmount - paidAmount
                );

                // Update Invoice
                UpdateInvoice(inv.InvoiceID, paidAmount);

                remainingAmount -= paidAmount;
            }
            return receiptId;
        }

        public int AddAdvanceReceipt(DAL.ARModule obj, List<AdvancePayment> quotation)
        {
            int prevIdentity = _cm.GetReseedIdentity("Receipts", "ReceiptID");
            _cm.ReseedIdentity("Receipts", prevIdentity);
            string sql = @"INSERT INTO Receipts (PayerId,
                                ReceiptDate,
                                ReceiptAmount,
                                PaymentMethod,
                                BankAccountCode,
                                ReferenceNo,
                                Notes,
                                CreatedOn,
                                PayerType,
                                PayerName,
                                InvoiceID,VoucherYear,VoucherCount,VoucherNumber,InvoiceARID,ReceiptType
                                ) OUTPUT INSERTED.ReceiptId VALUES (@PayerId,
                                @ReceiptDate,
                                @ReceiptAmount,
                                @PaymentMethod,
                                @BankAccountCode,
                                @ReferenceNo,
                                @Notes,
                                @CreatedOn,
                                @PayerType,
                                @PayerName,
                                @InvoiceID,@VoucherYear,@VoucherCount,@VoucherNumber,@InvoiceARID,@ReceiptType)";
            SqlParameter[] param = {
                                    new SqlParameter("@PayerId", obj.PayerId),
                                    new SqlParameter("@ReceiptDate", obj.ReceiptDate),
                                    new SqlParameter("@ReceiptAmount", obj.ReceiptAmount),
                                    new SqlParameter("@PaymentMethod", obj.PaymentMethod),
                                    new SqlParameter("@BankAccountCode", obj.BankAccountCode),
                                    new SqlParameter("@ReferenceNo", obj.ReferenceNo),
                                    new SqlParameter("@Notes", obj.Notes),
                                    new SqlParameter("@CreatedOn", obj.CreatedOn),
                                    new SqlParameter("@PayerType", obj.PayerType),
                                    new SqlParameter("@PayerName", obj.PayerName),
                                    new SqlParameter("@InvoiceID", obj.InvoiceID),
                                    new SqlParameter("@VoucherYear", obj.Year),
                                    new SqlParameter("@VoucherCount", obj.Counter),
                                    new SqlParameter("@VoucherNumber", obj.Number),
                                    new SqlParameter("@InvoiceARID", obj.InvoiceARID),
                                    new SqlParameter("@ReceiptType", obj.ReceiptType)
                                };
            int receiptId = Convert.ToInt32(DbHelper.ExecuteInsertWithReturn(sql, param));
            decimal remainingAmount = obj.ReceiptAmount;

            foreach (var inv in quotation.OrderBy(x => x.QuotationDate))
            {
                if (remainingAmount <= 0)
                    break;

                decimal paidAmount = Math.Min(inv.BalanceAmount, remainingAmount);

                // Insert Receipt Detail
                InsertReceiptDetail(
                    receiptId,
                    inv.QuotationID,
                    inv.TotalAmount,
                    paidAmount,
                    inv.BalanceAmount - paidAmount
                );

            }
            return receiptId;
        }
        private void UpdateInvoice(
                        int invoiceId,
                        decimal paidAmount)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = @"
                    UPDATE Invoices
                    SET 
                        AmountPaid = ISNULL(AmountPaid,0) + @PaidAmount,
                        BalanceDue = TotalAmount - (ISNULL(AmountPaid,0) + @PaidAmount)
                    WHERE InvoiceID = @InvoiceID;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        private void InsertReceiptDetail(
                                int receiptId,
                                int invoiceId,
                                decimal totalAmount,
                                decimal paidAmount,
                                decimal balanceAfter)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = @"INSERT INTO ReceiptDetails
                                (ReceiptID, InvoiceID, InvoiceAmount, PaidAmount, BalanceAfterPayment)
                                VALUES
                                (@ReceiptID, @InvoiceID, @InvoiceAmount, @PaidAmount, @BalanceAfter);";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ReceiptID", receiptId);
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    cmd.Parameters.AddWithValue("@InvoiceAmount", totalAmount);
                    cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                    cmd.Parameters.AddWithValue("@BalanceAfter", balanceAfter);

                    cmd.ExecuteNonQuery();
                   con.Close();
                }
            }
        }
        public void AddPayment(DAL.ARModule obj)
        {
            int prevIdentity = _cm.GetReseedIdentity("Payments", "PaymentID");
            _cm.ReseedIdentity("Payments", prevIdentity);

            DbHelper.ExecuteNonQuery(
                @"INSERT INTO Payments (PayeeId,
                                PaymentDate,
                                PaymentAmount,
                                PaymentMethod,
                                BankAccountCode,
                                ReferenceNo,
                                Notes,
                                JournalId,
                                CreatedOn,
                                PayeeType,
                                PayeeName,
                                ID
                                ) VALUES (@PayeeId,
                                @PaymentDate,
                                @PaymentAmount,
                                @PaymentMethod,
                                @BankAccountCode,
                                @ReferenceNo,
                                @Notes,
                                @JournalId,
                                @CreatedOn,
                                @PayeeType,
                                @PayeeName,
                                @ID)",
                new SqlParameter("@PayeeId", obj.PayerId),
                new SqlParameter("@PaymentDate", obj.PaymentDate),
                new SqlParameter("@PaymentAmount", obj.PaymentAmount),
                new SqlParameter("@PaymentMethod", obj.PaymentMethod),
                new SqlParameter("@BankAccountCode", obj.BankAccountCode),
                new SqlParameter("@ReferenceNo", obj.ReferenceNo),
                new SqlParameter("@Notes", obj.Notes),
                new SqlParameter("@JournalId", obj.JournalId),
                new SqlParameter("@CreatedOn", obj.CreatedOn),
                new SqlParameter("@PayeeType", obj.PayerType),
                new SqlParameter("@PayeeName", obj.PayerName),
                new SqlParameter("@ID", obj.InvoiceID)
            );
        }
        public DataTable GetARInvoices(int InvoiceARID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT
                    ir.InvoiceARId,ir.CustomerID,
                    i.invoiceID,
                    i.InvoiceNumber
                FROM InvoiceAR ir
                CROSS APPLY STRING_SPLIT(ir.InvoiceID, ',') s
                INNER JOIN Invoices i 
                ON i.InvoiceID = CAST(s.value AS INT)
                where ir.InvoiceARId= " + InvoiceARID);
        }

        public DataTable GetARInvoiceDetails(int InvoiceARID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT distinct
                    ir.InvoiceARId,ir.invoiceID,ir.CustomerID,c.CompanyName,ir.TotalAmount
                FROM InvoiceAR ir
                CROSS APPLY STRING_SPLIT(ir.InvoiceID, ',') s
                INNER JOIN Invoices i 
				inner join CompanyDetails c on c.CustomerMasterId = i.CompanyID
                ON i.InvoiceID = CAST(s.value AS INT)
                where ir.InvoiceARId=" + InvoiceARID);
        }
        public DataTable GetARReceipts(int InvoiceARID)
        {
            return DbHelper.ExecuteQuery(
                @"SELECT 
                        SUM(ReceiptAmount) AS TotalReceipt
                    FROM 
                        Receipts
                    WHERE PayerType='Customer' and InvoiceARID= " + InvoiceARID +
              @"  GROUP BY 
                        InvoiceARID;");
        }

        public void UpdateAmountReceived(DAL.ARModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update Invoices set AmountPaid=" + obj.AmountPaid + ",BalanceDue=" + obj.BalanceDue + " where InvoiceID=" + obj.InvoiceID);
            DbHelper.ExecuteNonQuery(
                @"Update InvoiceAR set AmountPaid=" + obj.AmountPaid + " where InvoiceID=" + obj.InvoiceID);
        }
        public void UpdateJournalID(DAL.ARModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update Receipts set JournalID=" + obj.JournalId + " where ReceiptID=" + obj.ReceiptID);
        }
        public void UpdateVoucherID(DAL.ARModule obj)
        {
            DbHelper.ExecuteNonQuery(
                @"Update Receipts set VoucherID=" + obj.VoucherID + " where ReceiptID=" + obj.ReceiptID);
        }
        public DataTable GetBankFromCode(string AccountCode)
        {
            return DbHelper.ExecuteQuery(
                @"select * from BankAccount where accountcode like '%"+AccountCode+"%'");
        }

    }

}
