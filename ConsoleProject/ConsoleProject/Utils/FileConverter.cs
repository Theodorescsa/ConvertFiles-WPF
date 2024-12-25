using System;

namespace WpfUploadFile
{
    public class FileConverter
    {
       
        
        public string ConvertPdfToHtml(string filePath)
        {
            // Logic chuyển đổi giả lập
            //string convertedFilePath = filePath.Replace(".pdf", "_converted.html");
            Console.WriteLine($"File {filePath} đã được chuyển đổi sang HTML: {filePath}");
            return filePath;
        }


        public string ConvertPdfToWord(string filePath)
        {
            // Logic chuyển đổi giả lập
            //string convertedFilePath = filePath.Replace(".pdf", "_converted.docx");
            Console.WriteLine($"File {filePath} đã được chuyển đổi sang Word: {filePath}");
            return filePath;
        }

        public string ConvertPdfToExcel(string filePath)
        {
            // Logic chuyển đổi giả lập
            //string convertedFilePath = filePath.Replace(".pdf", "_converted.xlsx");
            Console.WriteLine($"File {filePath} đã được chuyển đổi sang Excel: {filePath}");
            return filePath;
        }
    }
}
