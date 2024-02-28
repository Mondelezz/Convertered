using System.Windows;
using System.Text.RegularExpressions;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.IO;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.Content;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace ReConverteredPdfToHtml
{
    public class CustomFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            string fontFilePath = Path.Combine("C:\\Users\\user\\Desktop\\Fonts\\", faceName + ".ttf");


            if (File.Exists(fontFilePath))
            {
                try
                {
                    return File.ReadAllBytes(fontFilePath);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return null;
        }


        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string fontFilePath = Path.Combine("C:/Windows/", familyName + ".ttf");

            if (File.Exists(fontFilePath))
            {
                return null;
            }

            return new FontResolverInfo(familyName, isBold, isItalic);
        }
    }
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
                GlobalFontSettings.FontResolver = new CustomFontResolver();
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
                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        XFont font = new XFont("arial", 14);

                        gfx.DrawString(text, font, XBrushes.Black, new XRect(10, 10, page.Width, page.Height), XStringFormats.TopLeft);
                    }

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