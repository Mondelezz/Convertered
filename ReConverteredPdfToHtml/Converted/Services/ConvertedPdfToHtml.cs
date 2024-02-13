using ReConverteredPdfToHtml.Converted.Interfaces;
using Spire.Pdf;

namespace ReConverteredPdfToHtml.Converted.Services
{
    public class ConvertedPdfToHtml : IConverted
    {
        public string PdfToHtml(string filePath)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(filePath);
            filePath = filePath.Replace(".pdf", "");
            string filePathHtml = filePath + ".html";
            pdf.SaveToFile(filePathHtml);

            return filePathHtml;

        }
    }
}
