using ReConverteredPdfToHtml.Converted.Interfaces;
using Spire.Pdf.Graphics;
using Spire.Pdf;
using System.IO;
using System.Drawing;
using PdfDocument = Spire.Pdf.PdfDocument;


namespace ReConverteredPdfToHtml.Converted.Services
{
    public class ConvertedPdfToHtml : IConverted
    {      
        public void ConvertTextToPdfInCSharp(string inputTextFilePath, string outputPdfPath)
        {
            if (inputTextFilePath == null)
            {
                throw new Exception("path null");
            }
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add();
            string bodyText = File.ReadAllText(inputTextFilePath);
            PdfSolidBrush brushBlack = new PdfSolidBrush(new PdfRGBColor(System.Drawing.Color.Black));

            PdfTrueTypeFont headingFont = new PdfTrueTypeFont(new Font("Times New Roman", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont subHeadingFont = new PdfTrueTypeFont(new Font("Times New Roman", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont paraFont = new PdfTrueTypeFont(new Font("Times New Roman", 12f, FontStyle.Regular), true);

            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Center;

            PdfTextWidget widget = new PdfTextWidget(bodyText, paraFont, brushBlack);

            Rectangle rect = new Rectangle(0, 100, (int)page.Canvas.ClientSize.Width, (int)page.Canvas.ClientSize.Height);

            PdfTextLayout layout = new PdfTextLayout();
            layout.Layout = PdfLayoutType.Paginate;

            widget.Draw(page, rect, layout);
            File.ReadAllText(inputTextFilePath);

       
            doc.SaveToFile("outputPdfPath");
        }
    }

}

