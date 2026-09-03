using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CurrencyFormatting : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            decimal amount = Convert.ToDecimal(txtAmt.Text);

            //// Indonesian Rupiah
            //CultureInfo rupiahCulture = new CultureInfo("id-ID");
            //string rupiahFormatted = string.Format(rupiahCulture, "{0:C}", amount);

            //// USD
            //CultureInfo usdCulture = new CultureInfo("en-US");
            //string usdFormatted = string.Format(usdCulture, "{0:C}", amount);

            lblIDR.Text = _cm.ToIDR(amount);
            lblUSD.Text = _cm.ToUSD(amount);
            //Console.WriteLine("IDR: " + rupiahFormatted); // Rp1.234.567,89
            //Console.WriteLine("USD: " + usdFormatted);    // $1,234,567.89
        }
    }
}