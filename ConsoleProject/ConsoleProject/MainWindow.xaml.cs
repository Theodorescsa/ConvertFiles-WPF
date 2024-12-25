using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace WpfUploadFile
{
    public partial class MainWindow : Window
    {
        // Chuỗi kết nối MySQL
        private readonly string connectionString = "Server=localhost;Database=convertfilewpf;Uid=root;Pwd=dinhthai2004;";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Sự kiện khi nhấn nút "Chọn File"
        private void BtnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        // Sự kiện khi nhấn nút "Convert"
        private void BtnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtFilePath.Text;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                lblStatus.Content = "Vui lòng chọn một file hợp lệ!";
                return;
            }

            string selectedConversion = (dropdownOptions.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedConversion))
            {
                lblStatus.Content = "Vui lòng chọn loại chuyển đổi!";
                return;
            }

            try
            {
                string fileName = Path.GetFileName(filePath);
                string originalDirectory = @"D:\WPF-C#\ConvertFiles-WPF\ConsoleProject\ConsoleProject\media\original_files"; // Đường dẫn thư mục lưu file gốc
                Directory.CreateDirectory(originalDirectory);

                string originalFilePath = Path.Combine(originalDirectory, fileName);

                // Sao chép file gốc vào thư mục original_files
                File.Copy(filePath, originalFilePath, true); // true để ghi đè nếu file đã tồn tại

                // Sử dụng đường dẫn file gốc để truyền vào các hàm chuyển đổi
                FileConverter converter = new FileConverter();
                string file_converted = string.Empty;

                switch (selectedConversion)
                {
                    case "PDF -> HTML":
                        file_converted = converter.ConvertPdfToHtml(originalFilePath); 
                        break;

                    case "PDF -> WORD":
                        file_converted = converter.ConvertPdfToWord(originalFilePath); 
                        break;

                    case "PDF -> EXCEL":
                        file_converted = converter.ConvertPdfToExcel(originalFilePath);  
                        break;

                    default:
                        lblStatus.Content = "Không hỗ trợ chuyển đổi này!";
                        return;
                }

                // Tạo đường dẫn lưu file đã chuyển đổi
                string saveDirectory = @"D:\WPF-C#\ConvertFiles-WPF\ConsoleProject\ConsoleProject\media\converted_files";
                Directory.CreateDirectory(saveDirectory);
                string convertedFilePath = Path.Combine(saveDirectory, Path.GetFileName(file_converted));

                // Lưu file vào thư mục đã chuyển đổi (nếu có)
                if (!string.IsNullOrEmpty(file_converted))
                {
                    File.Copy(file_converted, convertedFilePath, true);
                }

                // Lưu thông tin file vào cơ sở dữ liệu
                SaveFileToDatabase(Path.GetFileName(file_converted), convertedFilePath);

                lblStatus.Content = "Chuyển đổi thành công!";
            }
            catch (Exception ex)
            {
                lblStatus.Content = $"Lỗi: {ex.Message}";
            }
        }

        // Lưu thông tin file vào MySQL
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

        // Sự kiện khi nhấn nút "Danh Sách File"
        private void BtnFileList_Click(object sender, RoutedEventArgs e)
        {
            UploadFileGrid.Visibility = Visibility.Collapsed;
            FileListGrid.Visibility = Visibility.Visible;

            LoadFileList();
        }

        // Tải danh sách file từ cơ sở dữ liệu
        private void LoadFileList()
        {
            try
            {
                var files = new System.Collections.Generic.List<FileInfo>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT file_name, file_path FROM files";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                files.Add(new FileInfo
                                {
                                    FileName = reader.GetString("file_name"),
                                    FilePath = reader.GetString("file_path")
                                });
                            }
                        }
                    }
                }

                dataGridFiles.ItemsSource = files;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách file: {ex.Message}");
            }
        }

        // Sự kiện khi nhấn nút "Quay Lại"
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            FileListGrid.Visibility = Visibility.Collapsed;
            UploadFileGrid.Visibility = Visibility.Visible;
        }
    }

    // Lớp đại diện cho file
    public class FileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    // Lớp chuyển đổi file
   
    
}
