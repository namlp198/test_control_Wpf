using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace test_control_WPF
{
    // Converter for Button Height (80% of toolbar height)
    public class HeightToButtonHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return Math.Max(24, height * 0.8 - 8); // 80% minus margin
            }
            return 32;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for Font Size based on toolbar height
    public class HeightToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return Math.Max(10, Math.Min(16, height * 0.25)); // Scale between 10-16
            }
            return 12;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for Separator Width
    public class HeightToSeparatorWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return Math.Max(8, height * 0.2); // 20% of height
            }
            return 12;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for Separator Height
    public class HeightToSeparatorHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return Math.Max(12, height * 0.8); // 60% of height
            }
            return 20;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for creating chevron geometry based on actual size
    public class ChevronGeometryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is double width && values[1] is double height)
            {
                double centerX = width / 2;
                double centerY = height / 2;
                double offsetX = Math.Max(2, width * 0.25);
                double offsetY = Math.Max(2, height * 0.25);

                var geometry = new GeometryGroup();

                // First line (top half: \)
                var line1 = new LineGeometry(
                    new Point(centerX - offsetX, centerY - offsetY),
                    new Point(centerX, centerY)
                );

                // Second line (bottom half: /)
                var line2 = new LineGeometry(
                    new Point(centerX - offsetX, centerY + offsetY),
                    new Point(centerX, centerY)
                );

                geometry.Children.Add(line1);
                geometry.Children.Add(line2);

                return geometry;
            }

            // Fallback geometry
            var fallback = new GeometryGroup();
            fallback.Children.Add(new LineGeometry(new Point(2, 4), new Point(6, 8)));
            fallback.Children.Add(new LineGeometry(new Point(2, 12), new Point(6, 8)));
            return fallback;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for stroke thickness based on height
    public class StrokeThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return Math.Max(1, height * 0.08); // 8% of height, minimum 1
            }
            return 1.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ProfessionalToolbar : UserControl
    {
        // Dependency Property for ToolbarItems
        public static readonly DependencyProperty ToolbarItemsProperty =
            DependencyProperty.Register("ToolbarItems", typeof(ObservableCollection<UIElement>),
            typeof(ProfessionalToolbar), new PropertyMetadata(null));

        public ObservableCollection<UIElement> ToolbarItems
        {
            get { return (ObservableCollection<UIElement>)GetValue(ToolbarItemsProperty); }
            set { SetValue(ToolbarItemsProperty, value); }
        }

        public ProfessionalToolbar()
        {
            InitializeComponent();
            ToolbarItems = new ObservableCollection<UIElement>();
        }

        // Helper method to add controls easily
        public void AddControl(UIElement control)
        {
            ToolbarItems.Add(control);
        }

        // Helper method to add chevron separator (2 small bars)
        private void AddChevronSeparator()
        {
            var canvas = new Canvas();
            canvas.SetResourceReference(StyleProperty, "ToolbarChevronSeparator");

            // Bind to actual height for dynamic scaling
            canvas.Loaded += (sender, e) =>
            {
                var actualCanvas = sender as Canvas;
                if (actualCanvas != null)
                {
                    // Clear existing children in case of resize
                    actualCanvas.Children.Clear();

                    // Get the actual dimensions
                    double canvasWidth = actualCanvas.ActualWidth;
                    double canvasHeight = actualCanvas.ActualHeight;

                    // Calculate scaling factors
                    double centerX = canvasWidth / 2;
                    double centerY = canvasHeight / 2;
                    double offsetX = Math.Max(2, canvasWidth * 0.45); // 25% of width for horizontal offset
                    double offsetY = Math.Max(2, canvasHeight * 0.35); // 25% of height for vertical offset

                    // Create first chevron line (top half: \ )
                    var line1 = new Line
                    {
                        X1 = centerX - offsetX,
                        Y1 = centerY - offsetY,
                        X2 = centerX,
                        Y2 = centerY,
                        Stroke = new SolidColorBrush(Color.FromRgb(0xCC, 0xCE, 0xDB)),
                        StrokeThickness = Math.Max(1, canvasHeight * 0.08), // Scale stroke thickness
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    // Create second chevron line (bottom half: / )
                    var line2 = new Line
                    {
                        X1 = centerX - offsetX,
                        Y1 = centerY + offsetY,
                        X2 = centerX,
                        Y2 = centerY,
                        Stroke = new SolidColorBrush(Color.FromRgb(0xCC, 0xCE, 0xDB)),
                        StrokeThickness = Math.Max(1, canvasHeight * 0.08), // Scale stroke thickness
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    actualCanvas.Children.Add(line1);
                    actualCanvas.Children.Add(line2);
                }
            };

            // Also handle size changes
            canvas.SizeChanged += (sender, e) =>
            {
                var actualCanvas = sender as Canvas;
                if (actualCanvas != null && actualCanvas.IsLoaded)
                {
                    // Trigger the loaded event handler to redraw
                    actualCanvas.Children.Clear();

                    double canvasWidth = e.NewSize.Width;
                    double canvasHeight = e.NewSize.Height;

                    double centerX = canvasWidth / 2;
                    double centerY = canvasHeight / 2;
                    double offsetX = Math.Max(2, canvasWidth * 0.25);
                    double offsetY = Math.Max(2, canvasHeight * 0.25);

                    var line1 = new Line
                    {
                        X1 = centerX - offsetX,
                        Y1 = centerY - offsetY,
                        X2 = centerX,
                        Y2 = centerY,
                        Stroke = new SolidColorBrush(Color.FromRgb(0xCC, 0xCE, 0xDB)),
                        StrokeThickness = Math.Max(1, canvasHeight * 0.08),
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    var line2 = new Line
                    {
                        X1 = centerX - offsetX,
                        Y1 = centerY + offsetY,
                        X2 = centerX,
                        Y2 = centerY,
                        Stroke = new SolidColorBrush(Color.FromRgb(0xCC, 0xCE, 0xDB)),
                        StrokeThickness = Math.Max(1, canvasHeight * 0.08),
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    actualCanvas.Children.Add(line1);
                    actualCanvas.Children.Add(line2);
                }
            };

            ToolbarItems.Add(canvas);
        }

        // Helper method to add chevron separator (2 small bars) - FIXED VERSION using Path
        private void AddChevronSeparator_2()
        {
            var chevron = new Path();

            // Create a multi-binding to combine width and height
            var multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(new Binding("ActualWidth") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
            multiBinding.Bindings.Add(new Binding("ActualHeight") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
            multiBinding.Converter = new ChevronGeometryConverter();

            chevron.SetBinding(Path.DataProperty, multiBinding);
            chevron.Stroke = new SolidColorBrush(Color.FromRgb(0xCC, 0xCE, 0xDB));
            chevron.StrokeEndLineCap = PenLineCap.Round;
            chevron.StrokeStartLineCap = PenLineCap.Round;

            // Bind stroke thickness to height
            var thicknessBinding = new Binding("ActualHeight")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Converter = new StrokeThicknessConverter()
            };
            chevron.SetBinding(Path.StrokeThicknessProperty, thicknessBinding);

            // Apply the separator style for width/height
            chevron.SetResourceReference(StyleProperty, "ToolbarChevronSeparator");

            ToolbarItems.Add(chevron);
        }

        // Helper method to add arrow separator (keep old version for compatibility)
        public void AddArrowSeparator()
        {
            AddChevronSeparator(); // Use new chevron separator
        }

        public void AddArrowSeparator_2()
        {
            AddChevronSeparator_2(); // Use new chevron separator
        }

        // Helper method to add button
        public Button AddButton(string text, double width = 100, string tooltip = null)
        {
            var button = new Button
            {
                Width = width,
                Content = text,
                ToolTip = tooltip
            };
            button.SetResourceReference(StyleProperty, "ModernToolbarButton");
            ToolbarItems.Add(button);
            return button;
        }

        // Helper method to add combobox
        public ComboBox AddComboBox(double width = 100)
        {
            var comboBox = new ComboBox
            {
                Width = width
            };
            comboBox.SetResourceReference(StyleProperty, "ModernToolbarComboBox");
            ToolbarItems.Add(comboBox);
            return comboBox;
        }

        // Helper method to add label
        public Label AddLabel(string text)
        {
            var label = new Label
            {
                Content = text
            };
            label.SetResourceReference(StyleProperty, "ModernToolbarLabel");
            ToolbarItems.Add(label);
            return label;
        }

        // Helper method to add textbox
        public TextBox AddTextBox(double width = 150, string placeholder = null)
        {
            var textBox = new TextBox
            {
                Width = width,
                Text = placeholder ?? ""
            };
            textBox.SetResourceReference(StyleProperty, "ModernToolbarTextBox");
            ToolbarItems.Add(textBox);
            return textBox;
        }

        // Helper method to add toggle button
        public ToggleButton AddToggleButton(string text, double width = 100, string tooltip = null)
        {
            var toggleButton = new System.Windows.Controls.Primitives.ToggleButton
            {
                Width = width,
                Content = text,
                ToolTip = tooltip
            };
            toggleButton.SetResourceReference(StyleProperty, "ModernToolbarToggleButton");
            ToolbarItems.Add(toggleButton);
            return toggleButton;
        }

        // Helper method to clear all controls
        public void ClearControls()
        {
            ToolbarItems.Clear();
        }
    }
}