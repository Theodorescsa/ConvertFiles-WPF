using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace WpfUploadFile
{
    public partial class FileListControl : UserControl
    {
        private readonly string connectionString = "Server=localhost;Database=convertfilewpf;Uid=root;Pwd=dinhthai2004;";

        public FileListControl()
        {
            InitializeComponent();
            LoadFileList();  // Tải danh sách file ngay khi khởi tạo UserControl
        }

        // Hàm tải lại dữ liệu từ DB
        private void Reload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadFileList();
        }

        // Hàm để tải danh sách file từ cơ sở dữ liệu và hiển thị trong DataGrid
        private void LoadFileList()
        {
            try
            {
                var files = new List<FileInfo>();

                // Kết nối với cơ sở dữ liệu MySQL và lấy dữ liệu
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

                // Hiển thị dữ liệu trong DataGrid
                dataGridFiles.ItemsSource = files;
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có
                System.Windows.MessageBox.Show($"Lỗi tải danh sách file: {ex.Message}");
            }
        }
    }

    // Class đại diện cho thông tin file
    public class FileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
