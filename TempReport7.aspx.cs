using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class TempReport7 : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClauseTableForReport();
            }
        }

        private void LoadClauseTableForReport()
        {
            DataTable dt = GetClauseData();
            DataTable saved = GetSavedResults(); // fetch TestResultsLED

            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                bool isHeader = row["isHeader"].ToString() == "1";

                if (isHeader)
                {
                    // Header section (same as before)
                    sb.Append("<table border='1' style='border-collapse: collapse; width:100%; margin-bottom:15px;'>");
                    sb.Append("<tr style='background-color:#f0f0f0;'>");
                    sb.Append("<td colspan='4'>");
                    sb.Append($"<b>{row["ClauseNumber"]}</b> {row["ClauseTitle_BH"]}.<br/>");
                    sb.Append($"<i>{row["ClauseTitle"]}</i>");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                    sb.Append("<tr>");
                    sb.Append("<th>Klausul<br/>Clause</th>");
                    sb.Append("<th>Persyaratan<br/>Requirements</th>");
                    sb.Append("<th>Hasil<br/>Result</th>");
                    sb.Append("<th>*Keterangan<br/>Remark</th>");
                    sb.Append("</tr>");
                }
                else
                {
                    int clauseID = Convert.ToInt32(row["ClauseID"]);

                    sb.Append("<tr>");
                    sb.Append($"<td>{row["ClauseNumber"]}</td>");

                    sb.Append("<td>");
                    sb.Append($"{row["Requirement_BH"]}<br/>");
                    sb.Append($"<i>{row["Requirement"]}</i>");
                    sb.Append("</td>");

                    // --- Results column ---
                    sb.Append("<td>");
                    string resultOptions = row["ResultOptions"]?.ToString().Trim();

                    if (!string.IsNullOrEmpty(resultOptions))
                    {
                        string[] options = resultOptions.Split(',');

                        // Get saved results for this ClauseID
                        var savedRows = saved.Select("ClauseID=" + clauseID);

                        foreach (string opt in options)
                        {
                            string isChecked = "";
                            if (savedRows.Length > 0 && savedRows.Any(r => r["Result"].ToString() == opt.Trim()))
                            {
                                isChecked = "checked='checked'";
                            }

                            sb.Append($"<label><input type='checkbox' disabled='disabled' {isChecked}/> {opt.Trim()}</label><br/>");
                        }
                    }
                    sb.Append("</td>");

                    // --- Remark column ---
                    string savedRemark = "";
                    var savedRow = saved.Select("ClauseID=" + clauseID).FirstOrDefault();
                    if (savedRow != null)
                        savedRemark = savedRow["Remark"].ToString();

                    sb.Append($"<td>{savedRemark}</td>");
                    sb.Append("</tr>");
                }
            }

            sb.Append("</table>");
            clauseContainer.InnerHtml = sb.ToString();
        }

        private DataTable GetClauseData()
        {
            DataTable dt = new DataTable();

            string query = "SELECT ClauseID, ClauseNumber, ClauseTitle, ClauseTitle_BH, Requirement, Requirement_BH, isHeader, ResultOptions, ResultOptions_BH FROM Clauses";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        da.Fill(dt);
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        private DataTable GetSavedResults()
        {
            DataTable dt = new DataTable();
            string query = "SELECT ClauseID, Result, Remark FROM TestResultsLED";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}