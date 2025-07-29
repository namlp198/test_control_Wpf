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
    /// Interaction logic for JoystickControl.xaml
    /// </summary>
    public partial class JoystickControl : UserControl
    {
        private Point _startPos;
        private bool _isCaptured;
        private double _maxRadius;

        // Dependency Properties cho các tham số có thể cấu hình
        public static readonly DependencyProperty NeutralValueProperty =
            DependencyProperty.Register("NeutralValue", typeof(int), typeof(JoystickControl),
                new PropertyMetadata(130));

        public static readonly DependencyProperty DeadZoneProperty =
            DependencyProperty.Register("DeadZone", typeof(int), typeof(JoystickControl),
                new PropertyMetadata(40, OnDeadZoneChanged));

        public int NeutralValue
        {
            get => (int)GetValue(NeutralValueProperty);
            set => SetValue(NeutralValueProperty, value);
        }

        public int DeadZone
        {
            get => (int)GetValue(DeadZoneProperty);
            set => SetValue(DeadZoneProperty, value);
        }

        // Giá trị X và Y (0-255)
        public int XValue { get; private set; }
        public int YValue { get; private set; }

        // Hướng hiện tại (8 hướng + neutral)
        public JoystickDirection CurrentDirection { get; private set; } = JoystickDirection.Neutral;

        // Sự kiện khi giá trị thay đổi
        public event EventHandler<JoystickEventArgs> ValueChanged;
        public event EventHandler<DirectionChangedEventArgs> DirectionChanged;

        public JoystickControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            XValue = NeutralValue;
            YValue = NeutralValue;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _maxRadius = OuterCircle.ActualWidth / 2 - InnerCircle.ActualWidth / 2;
            _startPos = new Point(OuterCircle.ActualWidth / 2, OuterCircle.ActualHeight / 2);
            UpdateInnerCirclePosition();
        }

        private static void OnDeadZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JoystickControl joystick)
            {
                // Cập nhật UI khi deadzone thay đổi
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _isCaptured = true;
            InnerCircle.CaptureMouse();
            UpdateJoystickPosition(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isCaptured)
            {
                UpdateJoystickPosition(e.GetPosition(this));
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _isCaptured = false;
            InnerCircle.ReleaseMouseCapture();

            // Reset về vị trí trung tâm
            XValue = NeutralValue;
            YValue = NeutralValue;
            UpdateInnerCirclePosition();
            UpdateDirection(JoystickDirection.Neutral);
            RaiseValueChanged();
        }

        private void UpdateJoystickPosition(Point currentPos)
        {
            // Tính vector từ tâm đến vị trí hiện tại
            Vector vector = currentPos - _startPos;

            // Giới hạn trong phạm vi joystick
            if (vector.Length > _maxRadius)
            {
                vector = vector / vector.Length * _maxRadius;
            }

            // Cập nhật vị trí inner circle
            InnerCircle.RenderTransform = new TranslateTransform(vector.X, vector.Y);

            // Tính giá trị X và Y (0-255)
            double normalizedX = vector.X / _maxRadius;
            double normalizedY = -vector.Y / _maxRadius; // Đảo ngược Y để down=0, up=255

            // Áp dụng dead zone và scaling
            XValue = NormalizeValue(normalizedX);
            YValue = NormalizeValue(normalizedY);

            // Xác định hướng
            var newDirection = CalculateDirection(normalizedX, normalizedY);
            if (newDirection != CurrentDirection)
            {
                UpdateDirection(newDirection);
            }

            RaiseValueChanged();
        }

        private JoystickDirection CalculateDirection(double x, double y)
        {
            if (Math.Abs(x) < DeadZone / 255.0 && Math.Abs(y) < DeadZone / 255.0)
                return JoystickDirection.Neutral;

            double angle = Math.Atan2(y, x) * (180 / Math.PI);
            angle = (angle + 360) % 360; // Chuyển về 0-360 độ

            if (angle >= 337.5 || angle < 22.5) return JoystickDirection.Right;
            if (angle >= 22.5 && angle < 67.5) return JoystickDirection.UpRight;
            if (angle >= 67.5 && angle < 112.5) return JoystickDirection.Up;
            if (angle >= 112.5 && angle < 157.5) return JoystickDirection.UpLeft;
            if (angle >= 157.5 && angle < 202.5) return JoystickDirection.Left;
            if (angle >= 202.5 && angle < 247.5) return JoystickDirection.DownLeft;
            if (angle >= 247.5 && angle < 292.5) return JoystickDirection.Down;
            return JoystickDirection.DownRight;
        }

        private void UpdateDirection(JoystickDirection newDirection)
        {
            CurrentDirection = newDirection;
            DirectionChanged?.Invoke(this, new DirectionChangedEventArgs(newDirection));

            // Cập nhật UI để highlight hướng đang chọn
            ResetDirectionIndicators();

            switch (newDirection)
            {
                case JoystickDirection.Up:
                    UpIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.Down:
                    DownIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.Left:
                    LeftIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.Right:
                    RightIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.UpLeft:
                    UpLeftIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.UpRight:
                    UpRightIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.DownLeft:
                    DownLeftIndicator.Fill = Brushes.Green;
                    break;
                case JoystickDirection.DownRight:
                    DownRightIndicator.Fill = Brushes.Green;
                    break;
            }
        }

        private void ResetDirectionIndicators()
        {
            var defaultBrush = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            UpIndicator.Fill = defaultBrush;
            DownIndicator.Fill = defaultBrush;
            LeftIndicator.Fill = defaultBrush;
            RightIndicator.Fill = defaultBrush;
            UpLeftIndicator.Fill = defaultBrush;
            UpRightIndicator.Fill = defaultBrush;
            DownLeftIndicator.Fill = defaultBrush;
            DownRightIndicator.Fill = defaultBrush;
        }

        private int NormalizeValue(double normalizedValue)
        {
            double deadZoneNormalized = DeadZone / 255.0;

            if (Math.Abs(normalizedValue) < deadZoneNormalized)
            {
                return NeutralValue;
            }

            // Scale từ [-1, -deadZone] và [deadZone, 1] đến [0, NeutralValue-DeadZone] và [NeutralValue+DeadZone, 255]
            if (normalizedValue < 0)
            {
                return (int)(NeutralValue + (normalizedValue + deadZoneNormalized) *
                    ((NeutralValue - DeadZone) / (-1 + deadZoneNormalized)));
            }
            else
            {
                return (int)(NeutralValue + (normalizedValue - deadZoneNormalized) *
                    ((255 - NeutralValue - DeadZone) / (1 - deadZoneNormalized)));
            }
        }

        private void UpdateInnerCirclePosition()
        {
            InnerCircle.RenderTransform = new TranslateTransform(0, 0);
        }

        private void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, new JoystickEventArgs(XValue, YValue, CurrentDirection));
        }
    }

    public enum JoystickDirection
    {
        Neutral,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public class JoystickEventArgs : EventArgs
    {
        public int XValue { get; }
        public int YValue { get; }
        public JoystickDirection Direction { get; }

        public JoystickEventArgs(int xValue, int yValue, JoystickDirection direction)
        {
            XValue = xValue;
            YValue = yValue;
            Direction = direction;
        }
    }

    public class DirectionChangedEventArgs : EventArgs
    {
        public JoystickDirection NewDirection { get; }

        public DirectionChangedEventArgs(JoystickDirection newDirection)
        {
            NewDirection = newDirection;
        }
    }
}
