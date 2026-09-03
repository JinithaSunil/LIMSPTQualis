using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static LIMS.DAL.APModule;

namespace LIMS.DAL
{
    public class Journal
    {
        DAL.CM _cm = new DAL.CM();
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        int _JournalId; public int JournalId { get { return _JournalId; } set { _JournalId = value; } }
        DateTime _JournalDate; public DateTime JournalDate { get { return _JournalDate; } set { _JournalDate = value; } }
        string _ReferenceNo; public string ReferenceNo { get { return _ReferenceNo; } set { _ReferenceNo = value; } }
        string _Transaction; public string Transaction { get { return _Transaction; } set { _Transaction = value; } }
        string _Description; public string Description { get { return _Description; } set { _Description = value; } }
        string _CreatedBy; public string CreatedBy { get { return _CreatedBy; } set { _CreatedBy = value; } }
        string _AccountID; public string AccountID { get { return _AccountID; } set { _AccountID = value; } }
        string _Debit; public string Debit { get { return _Debit; } set { _Debit = value; } }
        string _Credit; public string Credit { get { return _Credit; } set { _Credit = value; } }
        string _LineDescription; public string LineDescription { get { return _LineDescription; } set { _LineDescription = value; } }
        public struct JLVariables
        {
            public int JournalId { get; set; }
            public DateTime JournalDate { get; set; }
            public string ReferenceNo { get; set; }
            public string Description { get; set; }
            public string CreatedBy { get; set; }
            public string Debit { get; set; }
            public string Credit { get; set; }
            public string LineDescription { get; set; }
        }
        public DataTable JournalHeader()
        {
            DataTable dt = new DataTable("JournalHeaderType");
            dt.Columns.Add("JournalDate", typeof(DateTime));
            dt.Columns.Add("ReferenceNo", typeof(string));
            dt.Columns.Add("Transaction", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("CreatedBy", typeof(string));
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            return dt;
        }
        public DataTable JournalLines()
        {
            DataTable dt = new DataTable("JournalLineType");
            dt.Columns.Add("JournalID", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("LineDescription", typeof(string));
            dt.Columns.Add("AccountID", typeof(string));
            return dt;
        }
        public int AddJournalHeader(DataTable dt)
        {
            int prevIdentity = _cm.GetReseedIdentity("JournalHeader", "JournalID");
            _cm.ReseedIdentity("JournalHeader", prevIdentity);

            string sql = @"INSERT INTO JournalHeader (JournalDate,ReferenceNo,Txn, Description, CreatedBy, Posted) 
                          VALUES (@JournalDate,@ReferenceNo,@Transaction, @Description,@CreatedBy,1); 
                          SELECT SCOPE_IDENTITY();";

            SqlParameter[] param = {
                                    new SqlParameter("@JournalDate",dt.Rows[0]["JournalDate"] ),
                                    new SqlParameter("@ReferenceNo",dt.Rows[0]["ReferenceNo"] ),
                                    new SqlParameter("@Transaction",dt.Rows[0]["Transaction"] ),
                                    new SqlParameter("@Description",dt.Rows[0]["Description"] ),
                                    new SqlParameter("@CreatedBy",dt.Rows[0]["CreatedBy"] ),
                                    new SqlParameter("@Posted",1 ),
                                };

            int JournalId = DbHelper.ExecuteInsertWithReturn(sql, param);
            return JournalId;
        }
        public void AddJournalLines(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                int prevIdentity = _cm.GetReseedIdentity("JournalLines", "LineID");
                _cm.ReseedIdentity("JournalLines", prevIdentity);

                DbHelper.ExecuteNonQuery(
                    @"INSERT INTO JournalLines (JournalID,Debit,Credit, LineDescription, AccountID) 
                          VALUES (@JournalID,@Debit,@Credit, @LineDescription, @AccountID)",
                                        new SqlParameter("@JournalID", dr["JournalID"]),
                                        new SqlParameter("@Debit", dr["Debit"]),
                                        new SqlParameter("@Credit", dr["Credit"]),
                                        new SqlParameter("@LineDescription", dr["LineDescription"]),
                                        new SqlParameter("@AccountID", dr["AccountID"])
                );
            }

        }

        
        public void AddGeneralLedger(int journalID)
        {
            int prevIdentity = _cm.GetReseedIdentity("GeneralLedger", "GLID");
            _cm.ReseedIdentity("GeneralLedger", prevIdentity);

            DbHelper.ExecuteNonQuery(
                @"INSERT INTO GeneralLedger (TransactionDate,JournalID, Debit, Credit,AccountID)
                            SELECT 
                                JournalDate as TransactionDate,
                                jl.JournalID,
                                Debit,
                                Credit,
                                AccountID
                            FROM JournalLines jl
                            inner join JournalHeader jh on jh.JournalId = jl.JournalId
                            WHERE jl.JournalID = @JournalID",
                new SqlParameter("@JournalID", journalID)
            );
        }
        public int AddJournalWithGL(DataTable dtJournalHeader, DataTable JournalLines)
        {
            // Create parameters
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@JournalDate", SqlDbType.Date) { Value = dtJournalHeader.Rows[0]["JournalDate"] };
            parameters[1] = new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["ReferenceNo"] };
            parameters[2] = new SqlParameter("@Transaction", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["Transaction"] };
            parameters[3] = new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["Description"] };
            parameters[4] = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["CreatedBy"] };

            // Table-Valued Parameter
            parameters[5] = new SqlParameter("@Lines", SqlDbType.Structured)
            {
                TypeName = "dbo.JournalLineType",
                Value = JournalLines
            };

            return (DbHelper.ExecuteStoredProc("AddJournalWithGL", parameters));
        }
        public int AddJournalWithGL(SqlTransaction tran,DataTable dtJournalHeader, DataTable JournalLines)
        {
            // Create parameters
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@JournalDate", SqlDbType.Date) { Value = dtJournalHeader.Rows[0]["JournalDate"] };
            parameters[1] = new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["ReferenceNo"] };
            parameters[2] = new SqlParameter("@Transaction", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["Transaction"] };
            parameters[3] = new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["Description"] };
            parameters[4] = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = dtJournalHeader.Rows[0]["CreatedBy"] };

            // Table-Valued Parameter
            parameters[5] = new SqlParameter("@Lines", SqlDbType.Structured)
            {
                TypeName = "dbo.JournalLineType",
                Value = JournalLines
            };

            return (DbHelper.ExecuteStoredProc("AddJournalWithGL", parameters));
        }
        public DataTable GetJournalEntries()
        {
            return DbHelper.ExecuteQuery(@"SELECT * from JournalHeader jh
                                        inner join JournalLines jl on jh.JournalID = jl.JournalID");
        }
        public DataTable GetJournalHeader()
        {
            return DbHelper.ExecuteQuery("SELECT * from JournalHeader");
        }
        public DataTable GetJournalLines()
        {
            return DbHelper.ExecuteQuery("SELECT * from JournalLines");
        }
        public DataTable GetJournalEntries(int year,int month)
        {
            return DbHelper.ExecuteQuery(@"SELECT * from JournalHeader jh
                                        inner join JournalLines jl on jh.JournalID = jl.JournalID
                                        where year(journaldate) = "+year+" and month(JournalDate)=" + month);
        }
        public DataTable GetJournalHeader(int year, int month)
        {
            return DbHelper.ExecuteQuery("SELECT * from JournalHeader where year(journaldate) = "+year+" and month(JournalDate)=" + month);
        }
        public DataTable GetJournalLines(int year, int month)
        {
            return DbHelper.ExecuteQuery("SELECT * from JournalLines");
        }
    }
}