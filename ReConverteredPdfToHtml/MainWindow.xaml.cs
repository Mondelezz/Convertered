using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.IO;
using ReConverteredPdfToHtml.Converted.Interfaces;
using System;

namespace ReConverteredPdfToHtml
{
    public partial class MainWindow : Window
    {
        private readonly IConverted _converted;
        public MainWindow(IConverted converted)
        {
            InitializeComponent();
            _converted = converted;
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        public string ExtractTextFromPdf( string inputPath)
        {                

            string directory = System.IO.Path.GetDirectoryName(inputPath);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(inputPath);
            string fileExtension = System.IO.Path.GetExtension(inputPath);
            string outputPdfPath = System.IO.Path.Combine(directory, $"{fileNameWithoutExtension}_modified{fileExtension}");

            ITextExtractionStrategy its = new LocationTextExtractionStrategy();

            using (PdfReader reader = new PdfReader(inputPath))
            {
                StringBuilder textPdfFile = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string thePage = PdfTextExtractor.GetTextFromPage(reader, i, its);

                    string[] theLines = thePage.Split('\n');

                    foreach (var theLine in theLines)
                    { 
                        if (theLine.Trim().StartsWith("http") || theLine.Trim().StartsWith("https"))
                        {
                            textPdfFile.AppendLine(theLine.Replace("-", ""));
                        }
                        else
                        {
                            textPdfFile.AppendLine(theLine);
                        }
                    }
                }
                string inputTextFilePath = CreateTextFile(inputPath);

                StreamWriter sw = new StreamWriter(inputTextFilePath);
                sw.WriteLine(textPdfFile.ToString());
                sw.Close();

                _converted.ConvertTextToPdfInCSharp(inputTextFilePath, outputPdfPath);

                string text = File.ReadAllText(inputTextFilePath);
                
            }
            return outputPdfPath;
        }
        private string CreateTextFile(string inputPath)
        {
            if (inputPath == null)
            {
                throw new Exception("Путь не найден");
            }
            string directory = System.IO.Path.GetDirectoryName(inputPath);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(inputPath);
            string inputTextFilePath = System.IO.Path.Combine(directory, $"{fileNameWithoutExtension}_modified.txt");

            FileStream fs = File.Create(inputTextFilePath);
            fs.Close();         

            return inputTextFilePath;
        }
     

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "PDF Files|*.pdf";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ExtractTextFromPdf(filePath);
            }
        }

    }
   
        
 }