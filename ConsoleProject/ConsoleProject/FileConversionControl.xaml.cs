using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace WpfUploadFile
{
    public partial class FileConversionControl : UserControl
    {
        private readonly string connectionString = "Server=localhost;Database=convertfilewpf;Uid=root;Pwd=dinhthai2004;";

        public FileConversionControl()
        {
            InitializeComponent();
        }

        private void BtnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private async void BtnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtFilePath.Text;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Vui lòng chọn một file hợp lệ!");
                return;
            }

            string selectedConversion = (dropdownOptions.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedConversion))
            {
                MessageBox.Show("Vui lòng chọn loại chuyển đổi!");
                return;
            }

            try
            {
                string fileName = Path.GetFileName(filePath);
                string originalDirectory = @"D:\WPF-C#\ConvertFiles-WPF\ConsoleProject\ConsoleProject\media\original_files";
                Directory.CreateDirectory(originalDirectory);

                string originalFilePath = Path.Combine(originalDirectory, fileName);

                // Sao chép file gốc vào thư mục original_files
                File.Copy(filePath, originalFilePath, true);

                FileConverter converter = new FileConverter();
                string fileConverted = string.Empty;

                switch (selectedConversion)
                {
                    case "PDF -> HTML":
                        fileConverted = await converter.ConvertPdfToHtml(originalFilePath);

                        // Hiển thị PDF trên Canvas1
                        DisplayPdfOnCanvas(originalFilePath, pdfCanvas1);

                        // Hiển thị HTML trên Canvas2
                        string htmlOutputPath = @"D:\converted_output.html"; // Đường dẫn lưu tệp HTML
                        File.WriteAllText(htmlOutputPath, fileConverted);
                        DisplayHtmlOnCanvas(htmlOutputPath, pdfCanvas2);
                        break;

                    case "PDF -> WORD":
                        fileConverted = await converter.ConvertPdfToWord(originalFilePath);
                        break;

                    case "PDF -> EXCEL":
                        fileConverted = await converter.ConvertPdfToExcel(originalFilePath);
                        break;

                    default:
                        MessageBox.Show("Không hỗ trợ chuyển đổi này!");
                        return;
                }

                if (!string.IsNullOrEmpty(fileConverted))
                {
                    try
                    {
                        JObject responseJson = JObject.Parse(fileConverted);
                        string outputFileUrl = responseJson["ouput_file_url"]?.ToString();

                        if (!string.IsNullOrEmpty(outputFileUrl))
                        {
                            SaveFileToDatabase(Path.GetFileName(outputFileUrl), outputFileUrl);
                            MessageBox.Show("Chuyển đổi thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy 'ouput_file_url' trong phản hồi.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xử lý phản hồi JSON: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void DisplayPdfOnCanvas(string pdfPath, Canvas canvas)
        {
            // Sử dụng thư viện như PdfiumViewer hoặc PdfSharp để render PDF thành ảnh
            // Render ảnh từ PDF và vẽ lên canvas (ví dụ sử dụng ImageBrush hoặc DrawingContext)
            MessageBox.Show($"Hiển thị PDF tại: {pdfPath}");
        }

        private void DisplayHtmlOnCanvas(string htmlPath, Canvas canvas)
        {
            // Sử dụng WebBrowser hoặc render HTML thành ảnh và vẽ lên canvas
            MessageBox.Show($"Hiển thị HTML tại: {htmlPath}");
        }

        private void SaveFileToDatabase(string fileName, string filePath)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO files (file_name, file_path) VALUES (@FileName, @FilePath)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void DropdownOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dropdownOptions.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedValue = selectedItem.Content.ToString();
                switch (selectedValue)
                {
                    case "PDF -> HTML":
                        pdfCanvas1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/media/images/pdf.png")));
                        pdfCanvas2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/media/images/html.jpg")));
                        break;

                    case "PDF -> WORD":
                        pdfCanvas1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/media/images/pdf.png")));
                        pdfCanvas2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/media/images/word.jpg")));
                        break;

                    case "PDF -> EXCEL":
                        pdfCanvas1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/media/images/pdf.png")));
                        pdfCanvas2.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/media/images/excel.jpg")));
                        break;

                    default:
                        // Xóa nền nếu không khớp
                        pdfCanvas1.Background = Brushes.White;
                        pdfCanvas2.Background = Brushes.White;
                        break;
                }
            }
        }

    }
}
