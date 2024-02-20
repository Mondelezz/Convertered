using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReConverteredPdfToHtml.Converted.Interfaces
{
    public interface IConverted
    {
        public Task<string> PdfToFixError(string filePath);
    } 
}
