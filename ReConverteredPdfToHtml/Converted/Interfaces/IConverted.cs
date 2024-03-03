
namespace ReConverteredPdfToHtml.Converted.Interfaces
{
    public interface IConverted
    {
        public void MergePdfFiles(ICollection<string> inputTxtFiles, string outputPdfPath);
    } 
}
