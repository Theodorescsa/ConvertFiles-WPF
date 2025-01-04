using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace WpfUploadFile
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Server=localhost;Database=convertfilewpf;Uid=root;Pwd=dinhthai2004;";

        public MainWindow()
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
                string originalDirectory = @"D:\WPF-C#\ConvertFiles-WPF\ConsoleProject\ConsoleProject\media\original_files";
                Directory.CreateDirectory(originalDirectory);

                string originalFilePath = Path.Combine(originalDirectory, fileName);

                // Sao chép file gốc vào thư mục original_files
                File.Copy(filePath, originalFilePath, true);

                // Sử dụng đường dẫn file gốc để truyền vào các hàm chuyển đổi
                FileConverter converter = new FileConverter();
                string fileConverted = string.Empty;

                switch (selectedConversion)
                {
                    case "PDF -> HTML":
                        fileConverted = await converter.ConvertPdfToHtml(originalFilePath); // Await the async method
                        break;

                    case "PDF -> WORD":
                        fileConverted = await converter.ConvertPdfToWord(originalFilePath); // Await the async method
                        break;

                    case "PDF -> EXCEL":
                        fileConverted = await converter.ConvertPdfToExcel(originalFilePath); // Await the async method
                        break;

                    default:
                        lblStatus.Content = "Không hỗ trợ chuyển đổi này!";
                        return;
                }

                if (!string.IsNullOrEmpty(fileConverted))
                {
                    try
                    {
                        // Parse response body to extract the "ouput_file_url"
                        JObject responseJson = JObject.Parse(fileConverted);
                        string outputFileUrl = responseJson["ouput_file_url"]?.ToString();

                        if (!string.IsNullOrEmpty(outputFileUrl))
                        {
                            // Lưu thông tin file vào cơ sở dữ liệu
                            SaveFileToDatabase(Path.GetFileName(outputFileUrl), outputFileUrl);

                            lblStatus.Content = "Chuyển đổi thành công!";
                        }
                        else
                        {
                            lblStatus.Content = "Không tìm thấy 'ouput_file_url' trong phản hồi.";
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Content = $"Lỗi khi xử lý phản hồi JSON: {ex.Message}";
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Content = $"Lỗi: {ex.Message}";
            }
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


        private void BtnFileList_Click(object sender, RoutedEventArgs e)
        {
            UploadFileGrid.Visibility = Visibility.Collapsed;
            FileListGrid.Visibility = Visibility.Visible;
            LoadFileList();
        }

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

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            FileListGrid.Visibility = Visibility.Collapsed;
            UploadFileGrid.Visibility = Visibility.Visible;
        }
    }

    public class FileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
