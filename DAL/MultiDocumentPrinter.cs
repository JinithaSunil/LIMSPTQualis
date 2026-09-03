using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class MultiDocumentPrinter : IDisposable
    {
        private PrintDocument pd;
        private Queue<string> fileQueue;
        private StreamReader reader;

        private Image currentImage;
        private int currentImagePage = 0;

        private readonly Font font = new Font("Arial", 10);

        public MultiDocumentPrinter(IEnumerable<string> files)
        {
            fileQueue = new Queue<string>(files);

            pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            pd.BeginPrint += BeginPrint;
            pd.EndPrint += EndPrint;
        }

        private void BeginPrint(object sender, PrintEventArgs e)
        {
            LoadNextFile();
        }

        private void LoadNextFile()
        {
            reader?.Dispose();
            reader = null;

            currentImage?.Dispose();
            currentImage = null;

            currentImagePage = 0;

            if (fileQueue.Count == 0)
                return;

            string file = fileQueue.Dequeue();

            string ext = Path.GetExtension(file).ToLower();

            if (ext == ".txt")
            {
                reader = new StreamReader(file);
            }
            else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp")
            {
                currentImage = Image.FromFile(file);
            }
            // You can add PDF rendering using PDFiumViewer if needed
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            // TXT mode
            if (reader != null)
            {
                float lineHeight = font.GetHeight(e.Graphics);
                float y = e.MarginBounds.Top;
                int maxLines = (int)(e.MarginBounds.Height / lineHeight);

                int printed = 0;
                string line;

                while (printed < maxLines && (line = reader.ReadLine()) != null)
                {
                    e.Graphics.DrawString(line, font, Brushes.Black, e.MarginBounds.Left, y);
                    y += lineHeight;
                    printed++;
                }

                e.HasMorePages = !reader.EndOfStream;
                if (!e.HasMorePages)
                    LoadNextFile();

                return;
            }

            // IMAGE mode
            if (currentImage != null)
            {
                e.Graphics.DrawImage(currentImage, e.MarginBounds);
                LoadNextFile();

                e.HasMorePages = (currentImage != null);
                return;
            }

            e.HasMorePages = false;
        }

        private void EndPrint(object sender, PrintEventArgs e)
        {
            reader?.Dispose();
            currentImage?.Dispose();
        }

        public void Print()
        {
            pd.Print();
        }

        public void Dispose()
        {
            reader?.Dispose();
            currentImage?.Dispose();
            pd.Dispose();
        }
    }
}