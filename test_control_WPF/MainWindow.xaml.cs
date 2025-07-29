using System;
using System.Collections.Generic;
using System.Globalization;
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
            UpdateUI();

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

            DataContext = this;
        }

        private void YawSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GyroView.Yaw = e.NewValue;
            YawText.Text = ((int)e.NewValue).ToString();
        }

        private void PitchSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GyroView.Pitch = e.NewValue;
            PitchText.Text = ((int)e.NewValue).ToString();
        }

        private void RollSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GyroView.Roll = e.NewValue;
            RollText.Text = ((int)e.NewValue).ToString();
        }

        private void UpdateUI()
        {
            YawSlider.Value = GyroView.Yaw;
            PitchSlider.Value = GyroView.Pitch;
            RollSlider.Value = GyroView.Roll;
        }

        private void Joystick_ValueChanged(object sender, JoystickEventArgs e)
        {
            ValueDisplay.Text = $"X: {e.XValue}, Y: {e.YValue}";

            // Gửi giá trị đến robot ở đây
            // Ví dụ: SendToRobot(e.XValue, e.YValue);
        }

        private void Joystick_DirectionChanged(object sender, DirectionChangedEventArgs e)
        {
            DirectionDisplay.Text = $"Direction: {e.NewDirection}";
        }
    }

    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && int.TryParse(str, out int result))
                return result;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}
