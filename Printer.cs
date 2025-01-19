using Emgu.CV;
using System.Drawing.Printing;

namespace CyberedgeImageProcess2024
{
    public class Printer
    {
        private EdgeImagePlus imp;
        private static int scaling = 100;
        private static bool drawBorder;
        private static bool center = true;
        private static bool label;
        private static bool printSelection;
        private static bool rotate;
        private static bool actualSize;
        private static int fontSize = 12;

        private PageSettings pageSettings;
        public Printer(EdgeImagePlus imp)
        {
            this.imp = imp;
        }

        public void PageSetup()
        {
            Roi roi = imp != null ? imp.Roi : null;
            bool isRoi = roi != null && roi.IsArea();
            PrinterSetup printerSetup = new PrinterSetup(imp);

            printerSetup.Scaling = scaling;
            printerSetup.Center = center;
            printerSetup.DrawBorder = drawBorder;
            printerSetup.PrintTitle = label;
            printerSetup.Rotate = rotate;
            printerSetup.ActualSize = actualSize;

            if (printerSetup.ShowDialog() == DialogResult.Cancel) return;


            scaling = printerSetup.Scaling;
            if (scaling < 5.0) scaling = 5;
            drawBorder = printerSetup.DrawBorder;
            center = printerSetup.Center;
            label = printerSetup.PrintTitle;
            if (isRoi)
                printSelection = printerSetup.PrintSelection;
            else
                printSelection = false;
            rotate = printerSetup.Rotate;
            actualSize = printerSetup.ActualSize;

        }

        public void Print()
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.DocumentName = "Cyberedge Print";
            printDocument.PrintPage += new PrintPageEventHandler(PrintPage);

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDialog.Document.Print();
            }
        }

        public void SetPageFormat(PageSettings pageSettings)
        {
            this.pageSettings = pageSettings;
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            Roi roi = imp.Roi;
            EdgeImagePlus imp2 = imp;

            ImageProcessor ip = imp2.GetProcessor();
            if (printSelection && roi != null && roi.IsArea())
                ip.SetRoi(roi);
            ip = ip.Crop();

            int width = ip.Width;
            int height = ip.Height;
            int margin = 0;
            if (drawBorder) margin = 1;
            double scale = scaling / 100.0;
            int dstWidth = (int)(width * scale);
            int dstHeight = (int)(height * scale);
            int pageX = (int)pageSettings.PrintableArea.X;
            int pageY = (int)pageSettings.PrintableArea.Y;
            int dstX = pageX + margin;
            int dstY = pageY + margin;
            Image img = ip.CreateImage().ToBitmap();

            double pageWidth = pageSettings.PrintableArea.Width - 2 * margin;
            double pageHeight = pageSettings.PrintableArea.Height - 2 * margin;
            if (label && pageWidth - dstWidth < fontSize + 5)
            {
                dstY += fontSize + 5;
                pageHeight -= fontSize + 5;
            }
            if (actualSize)
            {
                if (center && dstWidth < pageWidth && dstHeight < pageHeight)
                {
                    dstX += (int)(pageWidth - dstWidth) / 2;
                    dstY += (int)(pageHeight - dstHeight) / 2;
                }
            }
            else if (dstWidth > pageWidth || dstHeight > pageHeight)
            {
                // scale to fit page
                double hscale = pageWidth / dstWidth;
                double vscale = pageHeight / dstHeight;
                double scale2 = hscale <= vscale ? hscale : vscale;
                dstWidth = (int)(dstWidth * scale2);
                dstHeight = (int)(dstHeight * scale2);
            }
            else if (center)
            {
                dstX += (int)(pageWidth - dstWidth) / 2;
                dstY += (int)(pageHeight - dstHeight) / 2;
            }


            g.DrawImage(img, new Rectangle(
        dstX, dstY, dstWidth, dstHeight), new Rectangle(
        0, 0, width, height), GraphicsUnit.Pixel);

            if (drawBorder)
                g.DrawRectangle(new Pen(Color.Black), dstX - 1, dstY - 1, dstWidth + 1, dstHeight + 1);
            if (label)
            {
                g.DrawString(imp.Title,
                    new Font(new FontFamily("宋体"), fontSize, FontStyle.Regular),
                    System.Drawing.Brushes.Black,
                    pageX + 5,
                    pageY + fontSize);
            }
        }
    }

}
