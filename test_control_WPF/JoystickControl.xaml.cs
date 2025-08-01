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

        // Dependency Properties
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

        // X, Y (0-255)
        public int XValue { get; private set; }
        public int YValue { get; private set; }
        public bool IsInDeadZone { get; private set; }

        // Current direction (8 direct + neutral)
        public JoystickDirection CurrentDirection { get; private set; } = JoystickDirection.Neutral;

        // Event
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
            this.Dispatcher.InvokeAsync(() =>
            {
                _maxRadius = OuterCircle.ActualWidth / 2 - InnerCircle.ActualWidth / 2;
                _startPos = new Point(OuterCircle.ActualWidth / 2, OuterCircle.ActualHeight / 2);
                UpdateDeadZoneVisual();
                UpdateInnerCirclePosition();
            }, System.Windows.Threading.DispatcherPriority.Loaded); // To make sure Layout calculated ActualWidth
        }

        private static void OnDeadZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is JoystickControl joystick)
            //{
            //    // update UI when deadzone changed
            //}
            if (d is JoystickControl joystick)
            {
                joystick.UpdateDeadZoneVisual();
            }
        }
        private void UpdateDeadZoneVisual()
        {
            if (_maxRadius <= 0) return;

            double dzRadius = _maxRadius * (DeadZone / 255.0);
            double size = dzRadius * 2;

            // Make sure it is at least larger than InnerCircle
            if (size < InnerCircle.Width + 20)
            {
                size = InnerCircle.Width + 20;
            }

            DeadZoneCircle.Width = size;
            DeadZoneCircle.Height = size;
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _isCaptured = true;
            InnerCircle.CaptureMouse();
            //UpdateJoystickPosition(e.GetPosition(this));
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

            // Reset back center position
            XValue = NeutralValue;
            YValue = NeutralValue;
            UpdateInnerCirclePosition();
            UpdateDirection(JoystickDirection.Neutral);
            RaiseValueChanged();
        }

        private void UpdateJoystickPosition(Point currentPos)
        {
            // Caculate vector from center to current position 
            Vector vector = currentPos - _startPos;

            // Limited within joystick range
            if (vector.Length > _maxRadius)
            {
                vector = vector / vector.Length * _maxRadius;
            }

            // Calculate X and Y value (0-255)
            double normalizedX = vector.X / _maxRadius;
            double normalizedY = -vector.Y / _maxRadius; // Reverse Y for down=0, up=255

            double magnitude = Math.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
            IsInDeadZone = magnitude < (DeadZone / 255.0);

            if (IsInDeadZone)
            {
                // Alway reset joystick back to center
                XValue = NeutralValue;
                YValue = NeutralValue;
                UpdateInnerCirclePosition();

                if (CurrentDirection != JoystickDirection.Neutral)
                {
                    UpdateDirection(JoystickDirection.Neutral);
                }

                RaiseValueChanged();
                return;
            }

            /* Move if outside DeadZone */

            // Update position of inner circle
            InnerCircle.RenderTransform = new TranslateTransform(vector.X, vector.Y);

            XValue = NormalizeValue(normalizedX);
            YValue = NormalizeValue(normalizedY);

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
            angle = (angle + 360) % 360; // Convert back 0-360 degree

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

            if (!IsInDeadZone)
            {
                DirectionChanged?.Invoke(this, new DirectionChangedEventArgs(newDirection));
            }

            // Update UI for highlight selecting position
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
            int center = NeutralValue;      // Ex: 130
            int dz = DeadZone;              // Ex: 40
            double dzNorm = dz / 255.0;

            // Inside deadzone → Alway return back to center position
            if (Math.Abs(normalizedValue) < dzNorm)
                return center;

            if (normalizedValue < 0) // left or down
            {
                double factor = (normalizedValue + dzNorm) / (1.0 - dzNorm);  // From 0 to -1
                double value = center - dz + factor * (center - dz);
                return Clamp((int)Math.Round(value, MidpointRounding.AwayFromZero), 0, 255);
            }
            else // right or up
            {
                double factor = (normalizedValue - dzNorm) / (1.0 - dzNorm);  // From 0 to 1
                double value = center + dz + factor * (255 - center - dz);
                return Clamp((int)Math.Round(value, MidpointRounding.AwayFromZero), 0, 255);
            }
        }
        private int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
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
