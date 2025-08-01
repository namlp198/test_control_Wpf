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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace test_control_WPF
{
    /// <summary>
    /// Interaction logic for TriStateToggleBasic.xaml
    /// </summary>
    public partial class TriStateToggleBasic : UserControl
    {
        public enum ToggleState { Off, Neutral, On }

        public ToggleState State { get; private set; } = ToggleState.Off;

        public static readonly DependencyProperty OffLabelProperty =
            DependencyProperty.Register(nameof(OffLabel), typeof(string), typeof(TriStateToggle), new PropertyMetadata("Off"));

        public static readonly DependencyProperty NeutralLabelProperty =
            DependencyProperty.Register(nameof(NeutralLabel), typeof(string), typeof(TriStateToggle), new PropertyMetadata("Neutral"));

        public static readonly DependencyProperty OnLabelProperty =
            DependencyProperty.Register(nameof(OnLabel), typeof(string), typeof(TriStateToggle), new PropertyMetadata("On"));

        public string OffLabel
        {
            get => (string)GetValue(OffLabelProperty);
            set => SetValue(OffLabelProperty, value);
        }

        public string NeutralLabel
        {
            get => (string)GetValue(NeutralLabelProperty);
            set => SetValue(NeutralLabelProperty, value);
        }

        public string OnLabel
        {
            get => (string)GetValue(OnLabelProperty);
            set => SetValue(OnLabelProperty, value);
        }

        public TriStateToggleBasic()
        {
            InitializeComponent();
            UpdateThumbPosition();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            State = (ToggleState)(((int)State + 1) % 3);
            UpdateThumbPosition();
        }

        private void UpdateThumbPosition()
        {
            double targetX = 0;

            switch (State)
            {
                case ToggleState.Off:
                    targetX = 0;
                    break;
                case ToggleState.Neutral:
                    targetX = (ActualWidth - 20) / 3;
                    break;
                case ToggleState.On:
                    targetX = 2 * (ActualWidth - 20) / 3;
                    break;
            }

            var animation = new DoubleAnimation
            {
                To = targetX,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            ThumbTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateThumbPosition();
        }
    }
}
