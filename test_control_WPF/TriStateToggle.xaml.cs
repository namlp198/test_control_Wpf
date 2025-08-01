using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace test_control_WPF
{
    /// <summary>
    /// Interaction logic for TriStateToggle.xaml
    /// </summary>
    public partial class TriStateToggle : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public enum ToggleState
        {
            State1,
            State2,
            State3
        }

        // Định nghĩa DependencyProperty với getter và setter đầy đủ
        public static readonly DependencyProperty State1TextProperty =
            DependencyProperty.Register("State1Text", typeof(string), typeof(TriStateToggle),
                new PropertyMetadata("STATE1", OnStateTextChanged));

        public static readonly DependencyProperty State2TextProperty =
            DependencyProperty.Register("State2Text", typeof(string), typeof(TriStateToggle),
                new PropertyMetadata("STATE2", OnStateTextChanged));

        public static readonly DependencyProperty State3TextProperty =
            DependencyProperty.Register("State3Text", typeof(string), typeof(TriStateToggle),
                new PropertyMetadata("STATE3", OnStateTextChanged));

        public string State1Text
        {
            get => (string)GetValue(State1TextProperty);
            set => SetValue(State1TextProperty, value);
        }

        public string State2Text
        {
            get => (string)GetValue(State2TextProperty);
            set => SetValue(State2TextProperty, value);
        }

        public string State3Text
        {
            get => (string)GetValue(State3TextProperty);
            set => SetValue(State3TextProperty, value);
        }

        private ToggleState _currentState = ToggleState.State1;

        public ToggleState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    UpdateVisualState();
                    StateChanged?.Invoke(this, new StateChangedEventArgs(value));
                    OnPropertyChanged(nameof(CurrentState));
                }
            }
        }

        private void UpdateButtonTexts()
        {
            OffButton.Content = State1Text;
            On1Button.Content = State2Text;
            On2Button.Content = State3Text;
        }

        private static void OnStateTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TriStateToggle control)
            {
                control.UpdateButtonTexts();
                control.OnPropertyChanged(e.Property.Name);
            }
        }

        public TriStateToggle()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                // Đợi layout hoàn tất trước khi cập nhật vị trí
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateVisualState();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            };
            SizeChanged += (s, e) => UpdateVisualState();
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVisualState(); // Cập nhật lại vị trí khi kích thước thay đổi
        }
        private void StateButton_Checked(object sender, RoutedEventArgs e)
        {
            // Khi click button -> DÙNG MoveThumb CÓ ANIMATION
            if (sender == OffButton)
            {
                CurrentState = ToggleState.State1;
                MoveThumb(0); // <-- GỌI HÀM NÀY KHI CLICK
            }
            else if (sender == On1Button)
            {
                CurrentState = ToggleState.State2;
                MoveThumb(1); // <-- GỌI HÀM NÀY KHI CLICK
            }
            else if (sender == On2Button)
            {
                CurrentState = ToggleState.State3;
                MoveThumb(2); // <-- GỌI HÀM NÀY KHI CLICK
            }
        }

        private void UpdateVisualState()
        {
            if (!IsLoaded) return;

            // KHỞI TẠO BAN ĐẦU - KHÔNG DÙNG ANIMATION
            ThumbTransform.BeginAnimation(TranslateTransform.XProperty, null);

            int column = (int)CurrentState;
            double newX = CalculateThumbPosition(column);
            ThumbTransform.X = newX; // Đặt vị trí ngay lập tức
        }

        private void MoveThumb(int column)
        {
            if (!IsLoaded) return;

            // Đảm bảo layout đã được tính toán xong
            UpdateLayout(); // <-- Thêm dòng này

            double newX = CalculateThumbPosition(column);

            // Sử dụng Storyboard để animation mượt hơn
            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                To = newX,
                Duration = TimeSpan.FromMilliseconds(300), // Tăng thời gian
                EasingFunction = new ElasticEase
                {
                    Oscillations = 1,
                    Springiness = 4,
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(animation, ThumbTransform);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        // Hàm helper tính toán vị trí
        private double CalculateThumbPosition(int column)
        {
            Dispatcher.VerifyAccess(); // Đảm bảo chạy trên UI thread
            double totalWidth = ActualWidth;
            if (totalWidth <= 0 || double.IsNaN(totalWidth)) return 0;

            double segmentWidth = totalWidth / 3;
            double thumbWidth = Thumb.ActualWidth;
            double position = (column * segmentWidth) + (segmentWidth - thumbWidth) / 2;

            return position;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StateChangedEventArgs : EventArgs
    {
        public TriStateToggle.ToggleState NewState { get; }

        public StateChangedEventArgs(TriStateToggle.ToggleState newState)
        {
            NewState = newState;
        }
    }

    public class MathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string expression && expression.StartsWith("@"))
            {
                if (double.TryParse(expression.Substring(1), out double adjustment))
                {
                    return Math.Max(40, doubleValue + adjustment); // Đảm bảo width tối thiểu hợp lý
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
