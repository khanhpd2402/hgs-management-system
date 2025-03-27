using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class ExportExcelException : Exception
    {
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public ExportExcelException(byte[] fileBytes, string fileName, string contentType)
        {
            FileBytes = fileBytes;
            FileName = fileName;
            ContentType = contentType;
        }
    }
}
