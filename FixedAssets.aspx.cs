using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class FixedAssets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadAssets();
        }

        private void LoadAssets()
        {
            gvAssets.DataSource = FixedAssetsModule.GetAssets();
            gvAssets.DataBind();
        }

        protected void btnAddAsset_Click(object sender, EventArgs e)
        {
            FixedAssetsModule.AddAsset(
                txtAssetCode.Text,
                txtAssetName.Text,
                DateTime.Parse(txtPurchaseDate.Text),
                decimal.Parse(txtPurchaseCost.Text),
                int.Parse(txtUsefulLife.Text)
            );
            Clear();
            LoadAssets();
        }
        protected void Clear()
        {
            txtAssetCode.Text = "";
            txtAssetName.Text = "";
            txtPurchaseDate.Text = "";
            txtPurchaseCost.Text = "";
            txtUsefulLife.Text = "";
        }
    }

}
