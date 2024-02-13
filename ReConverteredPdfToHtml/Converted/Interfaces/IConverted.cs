using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReConverteredPdfToHtml.Converted.Interfaces
{
    public interface IConverted
    {
        public string PdfToHtml(string filePath);
    } 
}
