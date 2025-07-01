using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    // Enum cho các mức độ log
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    // Model cho log entry
    public class LogEntry : INotifyPropertyChanged
    {
        private DateTime _timestamp;
        private LogLevel _level;
        private string _message;
        private string _source;

        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }

        public LogLevel Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string Source
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Converter để chuyển LogLevel thành màu sắc
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        return new SolidColorBrush(Colors.Gray);
                    case LogLevel.Info:
                        return new SolidColorBrush(Colors.Black);
                    case LogLevel.Warning:
                        return new SolidColorBrush(Colors.Orange);
                    case LogLevel.Error:
                        return new SolidColorBrush(Colors.Red);
                    case LogLevel.Fatal:
                        return new SolidColorBrush(Colors.DarkRed);
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Custom UserControl cho LogViewer
    public partial class LogViewerControl : UserControl
    {
        private ObservableCollection<LogEntry> _logEntries;
        private bool _autoScroll = true;
        private int _maxLogEntries = 1000;
        private ListView _logListView;

        public LogViewerControl()
        {
            InitializeComponent();
            _logEntries = new ObservableCollection<LogEntry>();

            // Tìm ListView trong template
            _logListView = FindName("LogListView") as ListView;
            if (_logListView != null)
            {
                _logListView.ItemsSource = _logEntries;
            }
        }

        // Override để đảm bảo template được load
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _logListView = FindName("LogListView") as ListView;
            if (_logListView != null && _logEntries != null)
            {
                _logListView.ItemsSource = _logEntries;
            }
        }

        // Dependency Properties
        public static readonly DependencyProperty MaxLogEntriesProperty =
            DependencyProperty.Register("MaxLogEntries", typeof(int), typeof(LogViewerControl),
                new PropertyMetadata(1000, OnMaxLogEntriesChanged));

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.Register("AutoScroll", typeof(bool), typeof(LogViewerControl),
                new PropertyMetadata(true));

        public int MaxLogEntries
        {
            get => (int)GetValue(MaxLogEntriesProperty);
            set => SetValue(MaxLogEntriesProperty, value);
        }

        public bool AutoScroll
        {
            get => (bool)GetValue(AutoScrollProperty);
            set => SetValue(AutoScrollProperty, value);
        }

        private static void OnMaxLogEntriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LogViewerControl control)
            {
                control._maxLogEntries = (int)e.NewValue;
                control.TrimLogEntries();
            }
        }

        // Methods để thêm log
        public void AddLog(LogLevel level, string message, string source = "")
        {
            if (string.IsNullOrEmpty(message))
                message = "";
            if (string.IsNullOrEmpty(source))
                source = "";

            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Source = source
            };

            // Kiểm tra nếu đang trong UI thread
            if (Application.Current?.Dispatcher?.CheckAccess() == true)
            {
                AddLogEntry(logEntry);
            }
            else
            {
                // Sử dụng Invoke thay vì BeginInvoke để đảm bảo thread safety
                Application.Current?.Dispatcher?.Invoke(() => AddLogEntry(logEntry));
            }
        }

        private void AddLogEntry(LogEntry logEntry)
        {
            if (_logEntries == null)
                _logEntries = new ObservableCollection<LogEntry>();

            _logEntries.Add(logEntry);
            TrimLogEntries();

            if (AutoScroll && _logListView?.Items != null && _logListView.Items.Count > 0)
            {
                try
                {
                    _logListView.ScrollIntoView(_logListView.Items[_logListView.Items.Count - 1]);
                }
                catch
                {
                    // Ignore scroll errors
                }
            }
        }

        public void AddDebug(string message, string source = "") => AddLog(LogLevel.Debug, message, source);
        public void AddInfo(string message, string source = "") => AddLog(LogLevel.Info, message, source);
        public void AddWarning(string message, string source = "") => AddLog(LogLevel.Warning, message, source);
        public void AddError(string message, string source = "") => AddLog(LogLevel.Error, message, source);
        public void AddFatal(string message, string source = "") => AddLog(LogLevel.Fatal, message, source);

        private void TrimLogEntries()
        {
            if (_logEntries == null) return;

            while (_logEntries.Count > _maxLogEntries)
            {
                _logEntries.RemoveAt(0);
            }
        }

        public void ClearLogs()
        {
            if (Application.Current?.Dispatcher?.CheckAccess() == true)
            {
                _logEntries?.Clear();
            }
            else
            {
                Application.Current?.Dispatcher?.Invoke(() => _logEntries?.Clear());
            }
        }

        public void SaveLogsToFile(string filePath)
        {
            if (_logEntries == null || _logEntries.Count == 0)
            {
                AddWarning("No logs to save", "LogViewer");
                return;
            }

            try
            {
                var lines = _logEntries.Select(entry =>
                    $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{entry.Level}] {entry.Source}: {entry.Message}");
                System.IO.File.WriteAllLines(filePath, lines);
                AddInfo($"Logs saved to {filePath}", "LogViewer");
            }
            catch (Exception ex)
            {
                AddError($"Failed to save logs: {ex.Message}", "LogViewer");
            }
        }

        // Filter methods
        public void FilterByLevel(LogLevel minLevel)
        {
            if (_logEntries == null) return;

            var view = CollectionViewSource.GetDefaultView(_logEntries);
            if (view != null)
            {
                view.Filter = item =>
                {
                    if (item is LogEntry entry)
                    {
                        return entry.Level >= minLevel;
                    }
                    return true;
                };
            }
        }

        public void FilterByText(string searchText)
        {
            if (_logEntries == null) return;

            var view = CollectionViewSource.GetDefaultView(_logEntries);
            if (view != null)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    view.Filter = null;
                }
                else
                {
                    view.Filter = item =>
                    {
                        if (item is LogEntry entry)
                        {
                            return (entry.Message?.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                   (entry.Source?.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
                        }
                        return true;
                    };
                }
            }
        }

        public void ClearFilter()
        {
            if (_logEntries == null) return;

            var view = CollectionViewSource.GetDefaultView(_logEntries);
            if (view != null)
            {
                view.Filter = null;
            }
        }

        // Event handlers
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLogs();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*",
                    DefaultExt = ".txt"
                };

                if (dialog.ShowDialog() == true)
                {
                    SaveLogsToFile(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                AddError($"Error opening save dialog: {ex.Message}", "LogViewer");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is TextBox textBox)
                {
                    FilterByText(textBox.Text);
                }
            }
            catch (Exception ex)
            {
                AddError($"Error in search: {ex.Message}", "LogViewer");
            }
        }

        private void LevelFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
                {
                    if (item.Tag is string levelString && Enum.TryParse<LogLevel>(levelString, out var level))
                    {
                        FilterByLevel(level);
                    }
                    else if (item.Tag as string == "All")
                    {
                        ClearFilter();
                    }
                }
            }
            catch (Exception ex)
            {
                AddError($"Error in level filter: {ex.Message}", "LogViewer");
            }
        }
    }
}
