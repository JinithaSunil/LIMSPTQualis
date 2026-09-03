using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace LIMS.DAL
{
    public class OrgTree
    {
        string tabletag = "<table class =\"tbl\" border=\"0\" " +
                              "cellpadding=\"0\" cellspacing=\"0\" style=\" " +
                              "table-layout: fixed;   width:100% ;vertical-align:top; text-align:center\" >";
        //HDiv variable Show horizontal  line .
        string HDiv = " <div style=\"position:relative; background-color:" +
                         " #000000;width: {0}%; left:{1}%;  height: 2px;\"></div>";
        //HDiv variable Show horizontal  line .
        string VDiv = " <div class=\"vertical\"></div>";
        public string CreateTreeHtml()
        {
            // CreateLevelNode funtion accept the Root node id. In this example the root node id is 1.
            return CreateLevelNode(1);
        }
        private string CreateLevelNode(int PID)
        {

            string finalHdiv;
            StringBuilder sbmainTbl = new StringBuilder();
            DataTable dtparent = GetParentNodeDetails(PID);

            //Caclulate with of tabele cell.     
            int leafNodeCount = GetLeafNodeCount(PID);
            if (leafNodeCount == 0)
                leafNodeCount = 1;//avoid divide by 0 error
            int tdWidth = 100 / leafNodeCount;

            // Get the difference in width to create blank node.
            int tdWidthDiff = 100 - leafNodeCount * tdWidth;

            sbmainTbl.AppendFormat("{0}", tabletag);

            // Add parent Node Text .
            sbmainTbl.AppendFormat("<tr><td>{0}</td></tr>",
                  CellText(dtparent.Rows[0]["Name"].ToString() + Environment.NewLine + "(" + dtparent.Rows[0]["EmployeeDepartmentName"].ToString() + "-" + dtparent.Rows[0]["DesignationName"].ToString() + ")"));

            // Get the child node Name
            DataTable dt = GetChildNodeDetails(int.Parse(dtparent.Rows[0]["ID"].ToString()));
            int childNodecount = dt.Rows.Count;

            //Draw Horizontal and Vertical line if child nore are more than one.
            if (childNodecount > 1)
            {
                if (childNodecount > 0)
                    sbmainTbl.AppendFormat("<tr><td>{0}</td></tr>", VDiv);

                if (childNodecount == 0)
                    finalHdiv = string.Format(HDiv, 0, 0);
                else
                    finalHdiv = HLineWidth(PID);
                string.Format(HDiv, 100 - 100 / childNodecount, 100 / (2 * childNodecount));
                sbmainTbl.AppendFormat("<tr><td>{0}</td></tr>", finalHdiv);
            }

            sbmainTbl.AppendFormat("<tr><td>");

            //Create Vertical line below parent Node.
            sbmainTbl.AppendFormat("{0}<tr>", tabletag);
            for (int i = 0; i < childNodecount; i++)
            {
                int leafNodeCountchild = GetLeafNodeCount(int.Parse(dt.Rows[i]["ID"].ToString()));
                if (leafNodeCountchild > 0)
                    sbmainTbl.AppendFormat("<td style=\"width:10%\" >{1}</td>",
                                           leafNodeCountchild * tdWidth, VDiv);
                else
                    sbmainTbl.AppendFormat("<td>{1}</td>", VDiv);
                // sbmainTbl.AppendFormat("<td style=\"width:{0}%\" >{1}</td>", tdWidth, VDiv);
            }
            //Create empty Cell.
            if (tdWidthDiff > 0)
            {
                sbmainTbl.AppendFormat("<td style=\"width:10%\" ></td>", tdWidthDiff);
            }
            sbmainTbl.Append("</tr>");

            sbmainTbl.Append("<tr>");
            for (int i = 0; i < childNodecount; i++)
            {
                //Create Child Node Table.
                sbmainTbl.AppendFormat("<td>{0}</td>",
                  CreateLevelNode(int.Parse(dt.Rows[i]["ID"].ToString())));
            }
            //Create empty Cell.
            if (tdWidthDiff > 0)
            {
                sbmainTbl.AppendFormat("<td style=\"width:{0}%\" ></td>", tdWidthDiff);
            }
            sbmainTbl.Append("</tr></table>");
            sbmainTbl.AppendFormat("</td></tr></table>", tabletag);
            return sbmainTbl.ToString();
        }


        private string HLineWidth(int PID)
        {
            //This function calculate the width of Horizontal line 


            float HlineWidth = 0;

            int leafNodeCount = GetLeafNodeCount(PID);
            if (leafNodeCount == 0)
                leafNodeCount = 1;//avoid divide by 0 error
            float tdWidth = 100 / leafNodeCount;
            float tdWidthDiff = 100 - tdWidth * leafNodeCount;
            DataTable dt = GetChildNodeDetails(PID);
            int childNodecount = dt.Rows.Count;

            float offset = 0;

            for (int i = 0; i < childNodecount; i++)
            {
                int leafNodeCountchild = GetLeafNodeCount(
                    int.Parse(dt.Rows[i]["ID"].ToString()));
                if (i == 0)
                {
                    offset = leafNodeCountchild * tdWidth / 2;
                    HlineWidth = HlineWidth + offset;
                }
                else if (i == (childNodecount - 1))
                {
                    HlineWidth = HlineWidth + leafNodeCountchild * tdWidth / 2;
                }
                else
                {
                    HlineWidth = HlineWidth + leafNodeCountchild * tdWidth;
                }
            }

            return string.Format(HDiv, HlineWidth, offset);
        }
        private string CellText(string Celltxt)
        {
            // Set the Node text here. You can customize this as per your requirement.
            return string.Format("<div style=\"display:block; " +
              "word-wrap: break-word; width: 99%;\">{0}</div>", Celltxt);
        }
        private DataTable GetChildNodeDetails(int parentId)
        {
            // Get the Current level child node details (ID , Name, PID)
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
            SqlCommand cmd = new SqlCommand(
              @"select o.*, ed.EmployeeDepartmentName, d.DesignationName from orgTree o 
                    inner join Employees e on o.ID = e.EmployeeID
                    inner join EmployeeDepartment ed on ed.EmployeeDepartmentID = e.DepartmentID
                    inner join designation d on d.DesignationID = e.DesignationID where o.pid = " + parentId.ToString(), con);
            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            return ds.Tables[0];
        }
        private DataTable GetParentNodeDetails(int ID)
        {
            // Get the Parent Node Name and ID
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"select o.*, ed.EmployeeDepartmentName, d.DesignationName from orgTree o 
                                                    inner join Employees e on o.ID = e.EmployeeID 
                                                    inner join EmployeeDepartment ed on ed.EmployeeDepartmentID = e.DepartmentID
                                                    inner join designation d on d.DesignationID = e.DesignationID where o.id= " + ID.ToString(), con);
            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            return ds.Tables[0];
        }
        private int GetLeafNodeCount(int ID)
        {
            // This function return the Leaf node count.
            string query = string.Format(@"WITH Node (ID, name,PID)
                                AS  (
                                    SELECT     OrgTree.ID, OrgTree.name , OrgTree.PID
                                    FROM       OrgTree
                                    WHERE      ID ={0}
                                    UNION ALL
                                    SELECT     OrgTree.ID, OrgTree.name, OrgTree.PID
                                    FROM       OrgTree
                                    INNER JOIN Node
                                    ON         OrgTree.PID = Node.ID
                                    )
                                SELECT  ID, name,PID FROM   Node
                                where  ID not in (SELECT  PID FROM   Node)
                                ", ID);

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
            SqlCommand cmd = new SqlCommand(query, con);
            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            return ds.Tables[0].Rows.Count;

        }
    }
    
}