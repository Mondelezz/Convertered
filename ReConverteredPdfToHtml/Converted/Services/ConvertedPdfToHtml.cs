using Microsoft.Extensions.Logging;
using ReConverteredPdfToHtml.Converted.Interfaces;
using System.IO;

namespace ReConverteredPdfToHtml.Converted.Services
{
    public class ConvertedPdfToHtml : IConverted
    {
        private readonly ILogger _logger;
        public ConvertedPdfToHtml(ILogger logger)
        { 
            _logger = logger;
        }
        public async Task<string> PdfToFixError(string filePath)       
        {

            using(StreamReader reader = new StreamReader(filePath)) 
            {

                string? line = string.Empty;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string subString = "https";
                    int startIndex = line.IndexOf(subString);
                    if (startIndex != 0)
                    {
                        int endIndex = line.IndexOf(" ", startIndex);
                        string result = line.Substring(startIndex, endIndex - startIndex);
                        result = result.Replace("-", "");
                        _logger.Log(LogLevel.Information, result);
                        await Console.Out.WriteLineAsync( result);
                    }
                    else
                    {
                        return line;
                    }
                   
                }
                return line;
            }

        }
    }
}
