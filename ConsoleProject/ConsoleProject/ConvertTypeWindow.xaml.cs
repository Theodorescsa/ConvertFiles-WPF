using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace WpfUploadFile
{
    public partial class FileListWindow : Window
    {
        private string connectionString = "Server=localhost;Database=convertfilewpf;Uid=root;Pwd=dinhthai2004;";

        public FileListWindow()
        {
            InitializeComponent();
            LoadFileList();
        }

        private void LoadFileList()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, file_name AS 'Tên File', file_path AS 'Đường Dẫn' FROM files";
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridFiles.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
