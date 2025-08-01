using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;

namespace test_control_WPF
{
    [TemplatePart(Name = "PART_Track", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Thumb))]
    public class TripleToggleSwitch : Control
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(TripleToggleSwitch),
                new PropertyMetadata(0, OnValueChanged));

        public static readonly DependencyProperty State0TextProperty =
            DependencyProperty.Register("State0Text", typeof(string), typeof(TripleToggleSwitch),
                new PropertyMetadata("Off"));

        public static readonly DependencyProperty State1TextProperty =
            DependencyProperty.Register("State1Text", typeof(string), typeof(TripleToggleSwitch),
                new PropertyMetadata("Middle"));

        public static readonly DependencyProperty State2TextProperty =
            DependencyProperty.Register("State2Text", typeof(string), typeof(TripleToggleSwitch),
                new PropertyMetadata("On"));

        public static readonly DependencyProperty SideImageProperty =
            DependencyProperty.Register("SideImage", typeof(UIElement), typeof(TripleToggleSwitch));

        public static readonly RoutedEvent ValueChangedEvent =
           EventManager.RegisterRoutedEvent(
               "ValueChanged",
               RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<int>),
               typeof(TripleToggleSwitch));

        public event RoutedPropertyChangedEventHandler<int> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string State0Text
        {
            get => (string)GetValue(State0TextProperty);
            set => SetValue(State0TextProperty, value);
        }

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

        public UIElement SideImage
        {
            get => (UIElement)GetValue(SideImageProperty);
            set => SetValue(SideImageProperty, value);
        }

        static TripleToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TripleToggleSwitch),
                new FrameworkPropertyMetadata(typeof(TripleToggleSwitch)));
        }

        private FrameworkElement track;
        private Thumb thumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            thumb = GetTemplateChild("PART_Thumb") as Thumb;
            track = GetTemplateChild("PART_Track") as FrameworkElement;

            if (thumb != null)
            {
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragCompleted += Thumb_DragCompleted;
            }

            UpdateThumbPosition();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (track == null || thumb == null) return;

            var newPos = Canvas.GetTop(thumb) + e.VerticalChange;
            newPos = Math.Max(0, Math.Min(newPos, track.ActualHeight - thumb.ActualHeight));
            Canvas.SetTop(thumb, newPos);
        }

        private void UpdateThumbPosition()
        {
            if (track == null || thumb == null) return;

            double sectionHeight = track.ActualHeight / 3;
            thumb.Height = sectionHeight;

            Canvas.SetTop(thumb, (2 - Value) * sectionHeight);

            thumb.Width = track.ActualWidth;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (track == null || thumb == null) return;

            double sectionHeight = track.ActualHeight / 3;
            double thumbPosition = Canvas.GetTop(thumb);

            if (thumbPosition < sectionHeight * 0.5)
            {
                Value = 2;
            }
            else if (thumbPosition > sectionHeight * 1.5)
            {
                Value = 0;
            }
            else
            {
                Value = 1;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateThumbPosition();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TripleToggleSwitch)d;
            control.UpdateThumbPosition();

            var oldValue = (int)e.OldValue;
            var newValue = (int)e.NewValue;
            control.RaiseEvent(new RoutedPropertyChangedEventArgs<int>(oldValue, newValue)
            {
                RoutedEvent = TripleToggleSwitch.ValueChangedEvent
            });
        }
    }
}
