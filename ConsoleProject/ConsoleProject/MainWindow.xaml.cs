using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace WpfUploadFile
{
    public partial class MainWindow : Window
    {
        // Chuỗi kết nối MySQL
        private string connectionString = "Server=localhost;Database=convertfilewpf;Uid=root;Pwd=dinhthai2004;";

        public MainWindow()
        {
            InitializeComponent();
        }
        // Sự kiện khi nhấn nút "Chọn File"
        private void BtnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ chọn file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Hiển thị đường dẫn của file đã chọn trong TextBox
                txtFilePath.Text = openFileDialog.FileName;
            }
        }
        // Sự kiện khi nhấn nút "Upload"
        private void BtnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtFilePath.Text;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                lblStatus.Content = "Vui lòng chọn một file hợp lệ!";
                return;
            }
            try
            {
                string fileName = Path.GetFileName(filePath);
                string fileSavePath = @"D:\WPF-C#\media\files\" + fileName; // Đường dẫn lưu trữ file trên server

                // Di chuyển file vào thư mục lưu trữ
                File.Copy(filePath, fileSavePath, true);

                // Lưu thông tin vào cơ sở dữ liệu MySQL
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO files (file_name, file_path) VALUES (@FileName, @FilePath)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FileName", fileName);
                        cmd.Parameters.AddWithValue("@FilePath", fileSavePath);
                        cmd.ExecuteNonQuery();
                    }
                }
                lblStatus.Content = "Tải lên thành công!";
            }
            catch (Exception ex)
            {
                lblStatus.Content = "Lỗi: " + ex.Message;
            }
        }
    }
}
