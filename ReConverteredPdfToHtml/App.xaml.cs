using ReConverteredPdfToHtml.Converted.Interfaces;
using ReConverteredPdfToHtml.Converted.Services;

using System.Windows;

namespace ReConverteredPdfToHtml
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IConverted converted = new ConvertedPdfToHtml();


            MainWindow mainWindow = new MainWindow(converted);

            mainWindow.Show();
        }
    }

}
