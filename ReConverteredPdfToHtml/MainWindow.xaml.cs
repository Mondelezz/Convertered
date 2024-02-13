using ReConverteredPdfToHtml.Converted.Interfaces;
using ReConverteredPdfToHtml.Converted.Services;
using System.Windows;

namespace ReConverteredPdfToHtml
{
    public partial class MainWindow : Window
    {
        private readonly ConvertedPdfToHtml _converted;
        public MainWindow()
        {           
            InitializeComponent();
        }
        private void UploadButton_CLick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileBrowse = new Microsoft.Win32.OpenFileDialog();
            fileBrowse.Filter = "Pdf Files|*.pdf";
            bool? response = fileBrowse.ShowDialog();
            if (response == true) 
            {               
                string filePath = fileBrowse.FileName;               
                MessageBox.Show(filePath);
                ConvertedPdfToHtml converted = new ConvertedPdfToHtml();
                converted.PdfToHtml(filePath);
            }
            throw new Exception("Ошибка получения html файла");
        }
    }
}