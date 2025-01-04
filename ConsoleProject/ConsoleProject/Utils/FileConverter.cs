using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WpfUploadFile
{
    public class FileConverter
    {
        private readonly string apiUrl = "http://127.0.0.1:8000/upload/";
        private readonly string bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlbl90eXBlIjoiYWNjZXNzIiwiZXhwIjoxNzM5MDI5MDQ0LCJpYXQiOjE3MzM4NDUwNDQsImp0aSI6ImVlYjIyMTlmNDdkYTRhMDRiZGViNTY2NGJkYTJiYTJkIiwidXNlcl9pZCI6MX0.7JmiOTahvDoK53Qw2lFll1ITbyKgeYeMq7wxWehaKIY"; 

        public async Task<string> ConvertPdfToHtml(string filePath)
        {
            return await UploadFileToApi(filePath, "pdf2html");
        }

        public async Task<string> ConvertPdfToWord(string filePath)
        {
            return await UploadFileToApi(filePath, "pdf2word");
        }

        public async Task<string> ConvertPdfToExcel(string filePath)
        {
            return await UploadFileToApi(filePath, "pdf2excel");
        }

        private async Task<string> UploadFileToApi(string filePath, string requestType)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Thêm Authorization Header
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    // Tạo nội dung multipart/form-data
                    using (var form = new MultipartFormDataContent())
                    {
                        // Đọc tệp và thêm vào form
                        var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                        form.Add(fileContent, "file", Path.GetFileName(filePath));

                        // Thêm tham số request_type
                        form.Add(new StringContent(requestType), "request_type");

                        // Gửi yêu cầu POST
                        var response = await client.PostAsync(apiUrl, form);

                        if (response.IsSuccessStatusCode)
                        {
                            // Xử lý kết quả trả về
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Chuyển đổi thành công: {responseBody}");
                            return responseBody;
                        }
                        else
                        {
                            Console.WriteLine($"Lỗi: {response.StatusCode}");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
