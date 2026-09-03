using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIMS.DAL;

namespace LIMS
{
    public partial class Sample : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await LoadSamples();
            }
        }
        private async Task LoadSamples()
        {
            try
            {
                var data =
                await ApiHelper.GetAsync<List<SampleModel>>
                ("api/sample/getsamples");

            gvSamples.DataSource = data;
            gvSamples.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }
    }
}