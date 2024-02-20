using System.Windows;
using System.Text.RegularExpressions;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.IO;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.Content;
using System.Text;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Quality;
using Microsoft.Extensions.Options;

namespace ReConverteredPdfToHtml
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ProcessPdf(string filePath)
{
    try
    {
        // Загрузка PDF файла
        PdfDocument document = PdfReader.Open(filePath, PdfDocumentOpenMode.Modify);

        // Обработка каждой страницы
        foreach (PdfPage page in document.Pages)
        {
            // Получение текста со страницы
            IEnumerable<string> textLines = page.ExtractText();

            // Объединение строк в единую строку
            string text = string.Join(Environment.NewLine, textLines);

            // Удаление тире из ссылок, начинающихся с "https"
            text = Regex.Replace(text, @"https[^\s]+", match => match.Value.Replace("-", ""));

            // Очистка содержимого страницы
            page.Contents.Elements.Clear();

                    // Добавление нового содержимого на страницу
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Arial", 12);

                    // Пример добавления текста на страницу
                    gfx.DrawString("Новый текст", font, XBrushes.Black, new XRect(10, 10, page.Width, page.Height), XStringFormats.TopLeft);

            // Удаление тире из ссылок внутри PDF-страницы
            page.RemoveHyphensFromUrls();
        }

        // Сохранение изменений в новый файл
        string outputFilePath = Path.Combine(Path.GetDirectoryName(filePath), "Modified_" + Path.GetFileName(filePath));
        document.Save(outputFilePath);
        MessageBox.Show("PDF файл успешно обработан и сохранен как " + Path.GetFileName(outputFilePath));
    }
    catch (Exception ex)
    {
        MessageBox.Show("Ошибка: " + ex.Message);
    }
}



        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "PDF Files|*.pdf";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ProcessPdf(filePath);
            }
        }
    }
    public static class PdfSharpExtensions
    {
        public static IEnumerable<string> ExtractText(this PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            var text = content.ExtractText();
            return text;
        }

        public static IEnumerable<string> ExtractText(this CObject cObject)
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                        foreach (var txt in ExtractText(cOperand))
                            yield return txt;
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                    foreach (var txt in ExtractText(element))
                        yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value;
            }
        }

        public static void RemoveHyphensFromUrls(this PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            RemoveHyphensFromUrls(content);
        }

        private static void RemoveHyphensFromUrls(CObject cObject)
        {
            if (cObject is COperator cOperator &&
                (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                 cOperator.OpCode.Name == OpCodeName.TJ.ToString()))
            {
                for (int i = 0; i < cOperator.Operands.Count; i++)
                {
                    var cOperand = cOperator.Operands[i];
                    if (cOperand is CString cString)
                    {
                        // Update the Value property directly
                        cString.Value = RemoveHyphensFromUrl(cString.Value);
                    }
                    else
                    {
                        RemoveHyphensFromUrls(cOperand);
                    }
                }
            }
            else if (cObject is CSequence cSequence)
            {
                for (int i = 0; i < cSequence.Count; i++)
                {
                    RemoveHyphensFromUrls(cSequence[i]);
                }
            }
        }


        private static string RemoveHyphensFromUrl(string url)
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                // Remove hyphens from the URL
                return url.Replace("-", "");
            }

            return url;
        }
    }
}