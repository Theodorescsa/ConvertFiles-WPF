using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfUploadFile
{
    public partial class HomeControl : UserControl
    {
        public HomeControl()
        {
            InitializeComponent();
        }

        // Xử lý sự kiện khi người dùng nhấn vào nút "Chọn Chức Năng"
        private void BtnChooseFunction_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng chọn đã được chọn!");
        }

        // Xử lý sự kiện khi người dùng nhấn vào nút "Quản Lý Dữ Liệu"
        private void BtnManageData_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quản lý dữ liệu đã được chọn!");
        }
    }
}
