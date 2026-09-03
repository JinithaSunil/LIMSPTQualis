using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LIMS.Models
{
    public class Parameters
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
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

        // Get active departments
        public DataTable GetActiveDepartments()
        {
            string query = "SELECT DepartmentID, DepartmentName FROM Department WHERE IsActive = 1 ORDER BY DepartmentName";
            return GetDataUsingQuery(query);
        }

        // Get all UOMs
        public DataTable GetUOMs()
        {
            string query = "SELECT UOMID, UOMName FROM UOM ORDER BY UOMName";
            return GetDataUsingQuery(query);
        }

        // Get all test methods
        public DataTable GetTestMethods()
        {
            string query = "SELECT * FROM TestMethod ORDER BY TestMethod";
            return GetDataUsingQuery(query);
        }

        // Get all requirements
        public DataTable GetRequirements()
        {
            string query = "SELECT * FROM Requirement ORDER BY Requirement";
            return GetDataUsingQuery(query);
        }

        public DataTable GetCalibrationEquipments()
        {
            string query = "SELECT * FROM Calibration_Equipment";
            return GetDataUsingQuery(query);
        }
        public DataTable GetTestParametersByDepartment(string departmentid)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM TestParameter WHERE departmentID = @departmentID ORDER BY TestParameter", _sCon))
                {
                    cmd.Parameters.AddWithValue("@departmentID", departmentid);

                    if (_sCon.State != ConnectionState.Open)
                    {
                        _sCon.Open();
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch
            {
                // Optional: log or handle error
            }
            return dt;
        }
        public DataTable GetTestSubParameters()
        {
            string query = "SELECT * FROM TestSubParameter";
            return GetDataUsingQuery(query);
        }
        public DataTable GetRequirementsByDepartment(string departmentId)
        {
            string query = "SELECT RequirementID, Requirement FROM Requirement WHERE DepartmentID = '" + departmentId + "' ORDER BY Requirement";
            return GetDataUsingQuery(query);
        }


        //Managing Sub Group
        public DataTable GetProductSubGroups()
        {
            string query = " SELECT SubGroupID, SubGroupName FROM ProductSubGroup WHERE DepartmentID = 4 ORDER BY SubGroupName";
            return GetDataUsingQuery(query);
        }
        public DataTable GetTestParametersSoftline()
        {
            string query = " SELECT * FROM TestParameter where  DepartmentID=4 ";
            return GetDataUsingQuery(query);
        }
        public DataTable GetTestSubParametersSoftline(int testParameterId)
        {
            string query = "SELECT * FROM TestSubParameter WHERE TestParameterID=" + testParameterId;
            return GetDataUsingQuery(query);
        }
        public DataTable GetTestSubSubParametersSoftline(int testSubParameterId)
        {
            string query = "SELECT * FROM TestSubSubParameter WHERE TestSubParameterID=" + testSubParameterId;
            return GetDataUsingQuery(query);
        }
        public DataTable GetProductSubGroupParameters(int subGroupId)
        {
            string query = @"SELECT TestParameterId, TestSubParameterId, TestSubSubParameterId 
                     FROM ProductSubGroup 
                     WHERE SubGroupID = @SubGroupID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public int UpdateProductSubGroupParameters(int subGroupId, string testParameterIds, string testSubParameterIds, string testSubSubParameterIds)
        {
            string query = @"UPDATE ProductSubGroup 
                     SET TestParameterId = @TestParameterId,
                         TestSubParameterId = @TestSubParameterId,
                         TestSubSubParameterId = @TestSubSubParameterId
                     WHERE SubGroupID = @SubGroupID";

            int rowsAffected = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestParameterId", testParameterIds ?? "");
                    cmd.Parameters.AddWithValue("@TestSubParameterId", testSubParameterIds ?? "");
                    cmd.Parameters.AddWithValue("@TestSubSubParameterId", testSubSubParameterIds ?? "");
                    cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);

                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating ProductSubGroup: " + ex.Message);
            }
            return rowsAffected;
        }
        public DataTable GetProductSubGroupParametersDetails(int? subGroupId = null)
        {
            string query = @"
        ;WITH P AS
        (
          SELECT
              psg.SubGroupID,
              psg.SubGroupName,
              psg.DepartmentID,
              psg.TestSubParameterId,      
              psg.TestSubSubParameterId,   
              TRY_CAST(s.value AS int) AS TestParameterID 
          FROM ProductSubGroup psg
          CROSS APPLY STRING_SPLIT(psg.TestParameterId, ',') AS s
        )
        SELECT DISTINCT
            P.SubGroupID,
            P.SubGroupName,
            tp.TestParameter,tp.TestParameterID,tsp.TestSubParameterID,tssp.TestSubSubParameterID,
            ISNULL(tsp.TestSubParameter, '') as TestSubParameter,
            ISNULL(tssp.TestSubSubParameter, '') as TestSubSubParameter,
            d.DepartmentName
        FROM P
        JOIN TestParameter tp
          ON tp.TestParameterID = P.TestParameterID
        LEFT JOIN TestSubParameter tsp         
          ON tsp.TestParameterID = tp.TestParameterID
         AND EXISTS ( SELECT 1
                      FROM STRING_SPLIT(P.TestSubParameterId, ',') ss
                      WHERE TRY_CAST(ss.value AS int) = tsp.TestSubParameterID )
        LEFT JOIN TestSubSubParameter tssp     
          ON tssp.TestSubParameterID = tsp.TestSubParameterID
         AND EXISTS ( SELECT 1
                      FROM STRING_SPLIT(P.TestSubSubParameterId, ',') sss
                      WHERE TRY_CAST(sss.value AS int) = tssp.TestSubSubParameterID )
        LEFT JOIN Department d
          ON d.DepartmentID = P.DepartmentID
        /**WHERE_CLAUSE**/
        ORDER BY P.SubGroupID, tp.TestParameter";

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (subGroupId.HasValue)
                    {
                        query = query.Replace("/**WHERE_CLAUSE**/", "WHERE P.SubGroupID = @SubGroupID");
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@SubGroupID", subGroupId.Value);
                    }
                    else
                    {
                        query = query.Replace("/**WHERE_CLAUSE**/", "");
                        cmd.CommandText = query;
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting product sub group parameters details: " + ex.Message);
            }
        }

        public int GetTestParameterIdByName(string testParameterName)
        {
            string query = "SELECT TestParameterID FROM TestParameter WHERE TestParameter = @TestParameterName";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestParameterName", testParameterName);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test parameter ID: " + ex.Message);
            }
        }
        public int GetTestSubParameterIdByName(string testSubParameterName, int TestParameterID)
        {
            string query = "SELECT TestSubParameterID FROM TestSubParameter WHERE TestSubParameter = @TestSubParameterName AND TestParameterID=@TestParameterID";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestSubParameterName", testSubParameterName);
                    cmd.Parameters.AddWithValue("@TestParameterID", TestParameterID);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test sub parameter ID: " + ex.Message);
            }
        }
        public int GetTestSubSubParameterIdByName(string testSubSubParameterName, int TestSubParameterID)
        {
            string query = "SELECT TestSubSubParameterID FROM TestSubSubParameter WHERE TestSubSubParameter = @TestSubSubParameterName AND TestSubParameterID=@TestSubParameterID";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestSubSubParameterName", testSubSubParameterName);
                    cmd.Parameters.AddWithValue("@TestSubParameterID", TestSubParameterID);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test sub sub parameter ID: " + ex.Message);
            }
        }

        public bool DeleteSpecificParameter(int subGroupId, string testParameter, string testSubParameter, string testSubSubParameter)
        {
            try
            {
                _sCon.Open();
                using (SqlTransaction transaction = _sCon.BeginTransaction())
                {
                    try
                    {
                        // Get current parameter IDs
                        DataTable currentData = GetProductSubGroupParameters(subGroupId);

                        if (currentData.Rows.Count == 0)
                            return false;

                        string testParameterIds = currentData.Rows[0]["TestParameterId"]?.ToString() ?? "";
                        string testSubParameterIds = currentData.Rows[0]["TestSubParameterId"]?.ToString() ?? "";
                        string testSubSubParameterIds = currentData.Rows[0]["TestSubSubParameterId"]?.ToString() ?? "";

                        // Get the TestParameter ID first (needed for subsequent lookups)
                        int testParamId = GetTestParameterIdByName(testParameter);
                        if (testParamId <= 0)
                            return false; // TestParameter not found

                        // Only remove the specific TestSubSubParameter ID if it exists
                        if (!string.IsNullOrEmpty(testSubSubParameter) && !string.IsNullOrEmpty(testSubParameter))
                        {
                            // Get TestSubParameter ID (needed to find TestSubSubParameter)
                            int testSubParamId = GetTestSubParameterIdByName(testSubParameter, testParamId);
                            if (testSubParamId > 0)
                            {
                                int testSubSubParamId = GetTestSubSubParameterIdByName(testSubSubParameter, testSubParamId);
                                if (testSubSubParamId > 0)
                                {
                                    testSubSubParameterIds = RemoveIdFromCommaSeparatedString(testSubSubParameterIds, testSubSubParamId.ToString());
                                }
                            }
                        }

                        // Check if we need to remove TestSubParameter (only if no more SubSubParameters depend on it)
                        if (!string.IsNullOrEmpty(testSubParameter))
                        {
                            int testSubParamId = GetTestSubParameterIdByName(testSubParameter, testParamId);
                            if (testSubParamId > 0)
                            {
                                // Check if any SubSubParameters still exist for this SubParameter
                                bool hasOtherSubSubParams = HasOtherSubSubParametersForSubParameter(testSubParamId, testSubSubParameterIds);

                                if (!hasOtherSubSubParams)
                                {
                                    testSubParameterIds = RemoveIdFromCommaSeparatedString(testSubParameterIds, testSubParamId.ToString());
                                }
                            }
                        }

                        // Check if we need to remove TestParameter (only if no more SubParameters depend on it)
                        if (testParamId > 0)
                        {
                            // Check if any SubParameters still exist for this TestParameter
                            bool hasOtherSubParams = HasOtherSubParametersForTestParameter(testParamId, testSubParameterIds);

                            if (!hasOtherSubParams)
                            {
                                testParameterIds = RemoveIdFromCommaSeparatedString(testParameterIds, testParamId.ToString());
                            }
                        }

                        // Update the ProductSubGroup table
                        string updateQuery = @"
                         UPDATE ProductSubGroup 
                         SET TestParameterId = @TestParameterId,
                             TestSubParameterId = @TestSubParameterId,
                             TestSubSubParameterId = @TestSubSubParameterId
                         WHERE SubGroupID = @SubGroupID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, _sCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);
                            cmd.Parameters.AddWithValue("@TestParameterId", testParameterIds);
                            cmd.Parameters.AddWithValue("@TestSubParameterId", testSubParameterIds);
                            cmd.Parameters.AddWithValue("@TestSubSubParameterId", testSubSubParameterIds);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting specific parameter: " + ex.Message);
            }
            finally
            {
                if (_sCon.State == ConnectionState.Open)
                    _sCon.Close();
            }
        }
        private bool HasOtherSubSubParametersForSubParameter(int subParameterId, string remainingSubSubParameterIds)
        {
            if (string.IsNullOrEmpty(remainingSubSubParameterIds))
                return false;

            try
            {
                // Get all SubSubParameters that belong to this SubParameter
                string query = "SELECT TestSubSubParameterID FROM TestSubSubParameter WHERE TestSubParameterID = @TestSubParameterID";

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestSubParameterID", subParameterId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string subSubParamId = reader["TestSubSubParameterID"].ToString();
                            if (remainingSubSubParameterIds.Contains(subSubParamId))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }

            return false;
        }
        private bool HasOtherSubParametersForTestParameter(int testParameterId, string remainingSubParameterIds)
        {
            if (string.IsNullOrEmpty(remainingSubParameterIds))
                return false;

            try
            {
                // Get all SubParameters that belong to this TestParameter
                string query = "SELECT TestSubParameterID FROM TestSubParameter WHERE TestParameterID = @TestParameterID";

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestParameterID", testParameterId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string subParamId = reader["TestSubParameterID"].ToString();
                            if (remainingSubParameterIds.Contains(subParamId))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }

            return false;
        }
        public bool DeleteOnlySubSubParameter(int subGroupId, string testSubSubParameter)
        {
            try
            {
                _sCon.Open();
                using (SqlTransaction transaction = _sCon.BeginTransaction())
                {
                    try
                    {
                        // Get current parameter IDs
                        DataTable currentData = GetProductSubGroupParameters(subGroupId);
                        if (currentData.Rows.Count == 0)
                            return false;

                        string testSubParameterIds = currentData.Rows[0]["TestSubParameterId"]?.ToString() ?? "";
                        string testSubSubParameterIds = currentData.Rows[0]["TestSubSubParameterId"]?.ToString() ?? "";

                        // Only remove the specific TestSubSubParameter ID
                        if (!string.IsNullOrEmpty(testSubSubParameter))
                        {
                            // We need to find which TestSubParameter this TestSubSubParameter belongs to
                            // Parse existing TestSubParameter IDs and check each one
                            string[] subParamIds = testSubParameterIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            int testSubSubParamId = 0;
                            foreach (string subParamIdStr in subParamIds)
                            {
                                if (int.TryParse(subParamIdStr.Trim(), out int subParamId))
                                {
                                    testSubSubParamId = GetTestSubSubParameterIdByName(testSubSubParameter, subParamId);
                                    if (testSubSubParamId > 0)
                                        break; // Found it
                                }
                            }

                            if (testSubSubParamId > 0)
                            {
                                testSubSubParameterIds = RemoveIdFromCommaSeparatedString(testSubSubParameterIds, testSubSubParamId.ToString());
                            }
                            else
                            {
                                // TestSubSubParameter not found in any of the existing SubParameters
                                return false;
                            }
                        }

                        // Update only the TestSubSubParameterId column
                        string updateQuery = @"
                         UPDATE ProductSubGroup 
                         SET TestSubSubParameterId = @TestSubSubParameterId
                         WHERE SubGroupID = @SubGroupID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, _sCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);
                            cmd.Parameters.AddWithValue("@TestSubSubParameterId", testSubSubParameterIds);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting specific sub sub parameter: " + ex.Message);
            }
            finally
            {
                if (_sCon.State == ConnectionState.Open)
                    _sCon.Close();
            }
        }
        private string RemoveIdFromCommaSeparatedString(string commaSeparatedIds, string idToRemove)
        {
            if (string.IsNullOrEmpty(commaSeparatedIds) || string.IsNullOrEmpty(idToRemove))
                return commaSeparatedIds;

            var idList = commaSeparatedIds.Split(',').Where(id => id.Trim() != idToRemove).ToList();
            return string.Join(",", idList);
        }
        private string AddIdToCommaSeparatedString(string commaSeparatedIds, string idToAdd)
        {
            if (string.IsNullOrEmpty(idToAdd))
                return commaSeparatedIds ?? "";

            if (string.IsNullOrEmpty(commaSeparatedIds))
                return idToAdd;

            // Check if ID already exists
            var existingIds = commaSeparatedIds.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToList();
            if (!existingIds.Contains(idToAdd))
            {
                return commaSeparatedIds + "," + idToAdd;
            }

            return commaSeparatedIds;
        }
        public bool DeleteOnlySubSubParameter(int subGroupId, int testSubSubParameterId)
        {
            try
            {
                _sCon.Open();
                using (SqlTransaction transaction = _sCon.BeginTransaction())
                {
                    try
                    {
                        // Get current parameter IDs
                        DataTable currentData = GetProductSubGroupParameters(subGroupId);
                        if (currentData.Rows.Count == 0)
                            return false;

                        string testSubSubParameterIds = currentData.Rows[0]["TestSubSubParameterId"]?.ToString() ?? "";

                        // Remove the specific TestSubSubParameter ID
                        if (testSubSubParameterId > 0)
                        {
                            testSubSubParameterIds = RemoveIdFromCommaSeparatedString(testSubSubParameterIds, testSubSubParameterId.ToString());
                        }
                        else
                        {
                            return false;
                        }

                        // Update only the TestSubSubParameterId column
                        string updateQuery = @"
                 UPDATE ProductSubGroup 
                 SET TestSubSubParameterId = @TestSubSubParameterId
                 WHERE SubGroupID = @SubGroupID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, _sCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);
                            cmd.Parameters.AddWithValue("@TestSubSubParameterId", testSubSubParameterIds);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting specific sub sub parameter: " + ex.Message);
            }
            finally
            {
                if (_sCon.State == ConnectionState.Open)
                    _sCon.Close();
            }
        }
        public bool UpdateSpecificParameter(int subGroupId, int originalTestParameterId, int originalTestSubParameterId,
        int originalTestSubSubParameterId, int newTestParameterId, int newTestSubParameterId, int newTestSubSubParameterId)
        {
            try
            {
                _sCon.Open();
                using (SqlTransaction transaction = _sCon.BeginTransaction())
                {
                    try
                    {
                        // Get current parameter IDs
                        DataTable currentData = GetProductSubGroupParameters(subGroupId);

                        if (currentData.Rows.Count == 0)
                            return false;

                        string testParameterIds = currentData.Rows[0]["TestParameterId"]?.ToString() ?? "";
                        string testSubParameterIds = currentData.Rows[0]["TestSubParameterId"]?.ToString() ?? "";
                        string testSubSubParameterIds = currentData.Rows[0]["TestSubSubParameterId"]?.ToString() ?? "";

                        // Handle TestParameter change
                        if (originalTestParameterId > 0 && newTestParameterId > 0 && originalTestParameterId != newTestParameterId)
                        {
                            // Remove old TestParameter if not used by other combinations
                            if (!IsTestParameterUsedByOtherCombinations(subGroupId, originalTestParameterId, originalTestSubParameterId, originalTestSubSubParameterId))
                            {
                                testParameterIds = RemoveIdFromCommaSeparatedString(testParameterIds, originalTestParameterId.ToString());
                            }

                            // Add new TestParameter
                            testParameterIds = AddIdToCommaSeparatedString(testParameterIds, newTestParameterId.ToString());
                        }

                        // Handle TestSubParameter change
                        if (originalTestSubParameterId > 0 && newTestSubParameterId > 0 && originalTestSubParameterId != newTestSubParameterId)
                        {
                            // Remove old TestSubParameter if not used by other combinations
                            if (!IsTestSubParameterUsedByOtherCombinations(subGroupId, originalTestSubParameterId, originalTestSubSubParameterId))
                            {
                                testSubParameterIds = RemoveIdFromCommaSeparatedString(testSubParameterIds, originalTestSubParameterId.ToString());
                            }

                            // Add new TestSubParameter
                            testSubParameterIds = AddIdToCommaSeparatedString(testSubParameterIds, newTestSubParameterId.ToString());
                        }

                        // Handle TestSubSubParameter change
                        if (originalTestSubSubParameterId > 0)
                        {
                            testSubSubParameterIds = RemoveIdFromCommaSeparatedString(testSubSubParameterIds, originalTestSubSubParameterId.ToString());
                        }

                        // Add new TestSubSubParameter ID if provided
                        if (newTestSubSubParameterId > 0)
                        {
                            testSubSubParameterIds = AddIdToCommaSeparatedString(testSubSubParameterIds, newTestSubSubParameterId.ToString());
                        }

                        // Update the ProductSubGroup table
                        string updateQuery = @"
                UPDATE ProductSubGroup 
                SET TestParameterId = @TestParameterId,
                    TestSubParameterId = @TestSubParameterId,
                    TestSubSubParameterId = @TestSubSubParameterId
                WHERE SubGroupID = @SubGroupID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, _sCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);
                            cmd.Parameters.AddWithValue("@TestParameterId", testParameterIds);
                            cmd.Parameters.AddWithValue("@TestSubParameterId", testSubParameterIds);
                            cmd.Parameters.AddWithValue("@TestSubSubParameterId", testSubSubParameterIds);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating specific parameter: " + ex.Message);
            }
            finally
            {
                if (_sCon.State == ConnectionState.Open)
                    _sCon.Close();
            }
        }
        public bool UpdateOnlyTestSubSubParameter(int subGroupId, int originalTestSubSubParameterId, int newTestSubSubParameterId)
        {
            try
            {
                _sCon.Open();
                using (SqlTransaction transaction = _sCon.BeginTransaction())
                {
                    try
                    {
                        // Get current parameter IDs
                        DataTable currentData = GetProductSubGroupParameters(subGroupId);
                        if (currentData.Rows.Count == 0)
                            return false;

                        string testSubSubParameterIds = currentData.Rows[0]["TestSubSubParameterId"]?.ToString() ?? "";

                        // Remove old TestSubSubParameter ID if exists
                        if (originalTestSubSubParameterId > 0)
                        {
                            testSubSubParameterIds = RemoveIdFromCommaSeparatedString(testSubSubParameterIds, originalTestSubSubParameterId.ToString());
                        }

                        // Add new TestSubSubParameter ID if provided
                        if (newTestSubSubParameterId > 0)
                        {
                            testSubSubParameterIds = AddIdToCommaSeparatedString(testSubSubParameterIds, newTestSubSubParameterId.ToString());
                        }

                        // Update only TestSubSubParameterId
                        string updateQuery = @"
                 UPDATE ProductSubGroup 
                 SET TestSubSubParameterId = @TestSubSubParameterId
                 WHERE SubGroupID = @SubGroupID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, _sCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SubGroupID", subGroupId);
                            cmd.Parameters.AddWithValue("@TestSubSubParameterId", testSubSubParameterIds);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating test sub sub parameter: " + ex.Message);
            }
            finally
            {
                if (_sCon.State == ConnectionState.Open)
                    _sCon.Close();
            }
        }
        private bool IsTestParameterUsedByOtherCombinations(int subGroupId, int testParameterId, int excludeTestSubParameterId, int excludeTestSubSubParameterId)
        {
            try
            {
                string query = @"
        SELECT tsp.TestSubParameterID, tssp.TestSubSubParameterID
        FROM TestSubParameter tsp
        LEFT JOIN TestSubSubParameter tssp ON tssp.TestSubParameterID = tsp.TestSubParameterID
        WHERE tsp.TestParameterID = @TestParameterID";

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestParameterID", testParameterId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int subParamId = reader["TestSubParameterID"] != DBNull.Value ? Convert.ToInt32(reader["TestSubParameterID"]) : 0;
                            int subSubParamId = reader["TestSubSubParameterID"] != DBNull.Value ? Convert.ToInt32(reader["TestSubSubParameterID"]) : 0;

                            // If there's another combination for this TestParameter (other than the one being updated)
                            if (subParamId != excludeTestSubParameterId || subSubParamId != excludeTestSubSubParameterId)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false; // Assume it's used to be safe
            }
        }
        private bool IsTestSubParameterUsedByOtherCombinations(int subGroupId, int testSubParameterId, int excludeTestSubSubParameterId)
        {
            try
            {
                string query = @"
        SELECT tssp.TestSubSubParameterID 
        FROM TestSubSubParameter tssp 
        WHERE tssp.TestSubParameterID = @TestSubParameterID";

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestSubParameterID", testSubParameterId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int subSubParamId = reader["TestSubSubParameterID"] != DBNull.Value ? Convert.ToInt32(reader["TestSubSubParameterID"]) : 0;

                            // If there's another TestSubSubParameter for this TestSubParameter (other than the one being updated)
                            if (subSubParamId != excludeTestSubSubParameterId && subSubParamId > 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false; // Assume it's used to be safe
            }
        }
        public int ExecuteNonQuery(string sQuery)
        {
            int rowsAffected = 0;
            try
            {
                if (_sCon.State != ConnectionState.Open)
                {
                    _sCon.Open();
                }
                SqlCommand cmd = new SqlCommand(sQuery, _sCon);
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing query: " + ex.Message);
            }
            return rowsAffected;
        }

    }
}
