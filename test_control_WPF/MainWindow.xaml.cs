using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace test_control_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                LogViewer.AddInfo("Application started", "MainWindow");

                var btn1 = toolBar.AddButton("INITIALIZE", 120, "Khởi tạo lại hệ thống");

                toolBar.AddArrowSeparator();
                LogViewer.AddInfo("Added INITIALIZE button", "MainWindow");

                var btn2 = toolBar.AddToggleButton("MANUAL", 120, "Vận hành");

                toolBar.AddArrowSeparator();
                LogViewer.AddInfo("Added TOGGLE button", "MainWindow");

                var btn3 = toolBar.AddButton("MODEL", 120, "Chọn recipe");

                toolBar.AddArrowSeparator();
                LogViewer.AddInfo("Added MODEL button", "MainWindow");

                var combo = toolBar.AddComboBox(120);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
