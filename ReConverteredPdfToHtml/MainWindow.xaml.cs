using System.IO;
using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ReConverteredPdfToHtml.Converted.Services;

namespace ReConverteredPdfToHtml
{
    public partial class MainWindow : Window
    {
        private ConvertedPdfToHtml pdfConverter;

        public MainWindow()
        {
            InitializeComponent();
            pdfConverter = new ConvertedPdfToHtml();
        }

        public string ExtractAndSplitPdf(string inputPath, int maxPagesPerFile)
        {
            List<string> filesTXT = new List<string>();

            using (PdfReader reader = new PdfReader(inputPath))
            {
                int totalNumberOfPages = reader.NumberOfPages;
                int totalPagesProcessed = 0;

                while (totalPagesProcessed < totalNumberOfPages)
                {
                    // Создаем новый файл для каждой порции страниц
                    string outputTxtPath = CreateTextFile(inputPath, totalPagesProcessed);
                    filesTXT.Add(outputTxtPath);

                    // Определение числа страниц в текущем файле
                    int pagesInCurrentFile = Math.Min(maxPagesPerFile, totalNumberOfPages - totalPagesProcessed);

                    using (StreamWriter sw = new StreamWriter(outputTxtPath))
                    {
                        // Обработка страниц в текущем файле
                        for (int i = totalPagesProcessed + 1; i <= totalPagesProcessed + pagesInCurrentFile; i++)
                        {
                            string thePage = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                            WriteTextToTxtFile(thePage, sw);
                        }
                    }

                    totalPagesProcessed += pagesInCurrentFile;
                }
            }

            string directory = System.IO.Path.GetDirectoryName(inputPath);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(inputPath);
            string outputPdfPath = System.IO.Path.Combine(directory, $"{fileNameWithoutExtension}_merged_{Guid.NewGuid()}.pdf");

            // Объединение созданных текстовых файлов в один PDF
            pdfConverter.MergePdfFiles(filesTXT, outputPdfPath);

            // Удаление текстовых файлов после объединения
            foreach (var file in filesTXT)
            {
                File.Delete(file);
            }

            MessageBox.Show($"PDF файлы успешно сохранены и объединены по пути:\n{outputPdfPath}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

            return outputPdfPath;
        }

        private void WriteTextToTxtFile(string text, StreamWriter sw)
        {
            string[] lines = text.Split('\n');

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("http") || line.Trim().StartsWith("https"))
                {
                    sw.WriteLine(line.Replace("-", ""));
                }
                else
                {
                    sw.WriteLine(line);
                }
            }
        }

        private string CreateTextFile(string inputPath, int pageIndex)
        {
            string directory = System.IO.Path.GetDirectoryName(inputPath);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(inputPath);
            string outputTxtPath = System.IO.Path.Combine(directory, $"{fileNameWithoutExtension}_part_{pageIndex + 1}.txt");

            using (File.Create(outputTxtPath)) { }

            return outputTxtPath;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "PDF Files|*.pdf";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ExtractAndSplitPdf(filePath, 100);
            }
        }
    }
}
