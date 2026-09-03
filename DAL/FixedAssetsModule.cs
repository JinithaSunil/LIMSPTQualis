using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class FixedAssetsModule
    {
        public static DataTable GetAssets()
        {
            return DbHelper.ExecuteQuery("SELECT AssetId, AssetCode, AssetName, PurchaseDate, PurchaseCost, UsefulLifeMonths FROM FixedAsset");
        }

        public static void AddAsset(string assetcode, string assetName, DateTime purchaseDate, decimal purchaseCost, int UsefulLifeMonths)
        {
            DbHelper.ExecuteNonQuery(
                "INSERT INTO FixedAsset (AssetCode, " +
                "AssetName, " +
                "PurchaseDate, " +
                "PurchaseCost, " +
                "UsefulLifeMonths)" +
                " VALUES (@AssetCode, " +
                "@AssetName, " +
                "@PurchaseDate, " +
                "@PurchaseCost, " +
                "@Life)",
                new SqlParameter("@AssetCode", assetcode),
                new SqlParameter("@AssetName", assetName),
                new SqlParameter("@PurchaseDate", purchaseDate),
                new SqlParameter("@PurchaseCost", purchaseCost),
                new SqlParameter("@Life", UsefulLifeMonths)
            );
        }
    }

}