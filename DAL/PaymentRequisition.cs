using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
	public class PaymentRequisition
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

        string sQuery = "";
        public DataTable GetDataUsingQuery(string sQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                if (_sCon.State != ConnectionState.Open)
                {
                    _sCon.Open();
                }
                SqlCommand cmd = new SqlCommand(sQuery, _sCon);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

            }
            catch
            {

            }
            return dt;
        }
        public DataTable getApprovalList(string PRID)
        {
            DataTable dt = new DataTable();
            string sQuery = @"select currenttier,currentApproverRole, al.* from PaymentRequisitionMaster pr
                            left join ApprovalLevels al on al.ApprovalLevel = pr.currentTier+1
                            where PaymentRequisitionID ='" + PRID + "' ";

            dt = GetDataUsingQuery(sQuery);
            return dt;
        }
    }
   
}