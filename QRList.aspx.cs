using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class QRList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                GenerateQR(Request.QueryString["ID"].ToString());
            }
        }
        protected void GenerateQR(string SampleID)
        {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            var result = QCwriter.Write(SampleID);//QCwriter.Write(lblBatchNumber.Text);
            string path = Server.MapPath("~/QR/MyQR.jpg");
            var barcodeBitmap = new Bitmap(result);

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path,
                   FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
                string sourceFile = Server.MapPath("~/QR/MyQR.jpg");
                string destinationFile = Server.MapPath("~/QR/" + SampleID + ".jpg");
                img.ImageUrl = "~/QR/" + SampleID + ".jpg";
                try
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }
    }
}