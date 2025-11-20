using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace ArduinoAudioController
{
    public partial class MainWindow : Window
    {
        #region Поля и свойства
        private bool _isConnected = false;
        private bool _isSimulationMode = false;
        private bool _isPlaying = false;
        private bool _isDarkTheme = true;

        private int _messageCount = 0;
        private int _currentTrack = 1;
        private int _currentVolume = 15;

        private DateTime _connectionTime;
        private Timer _statusTimer;
        private Timer _simulationTimer;
        private Random _random = new Random();

        private readonly List<string> _availablePorts = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5" };
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        #region Инициализация
        private void InitializeApplication()
        {
            try
            {
                InitializeTimers();
                LoadAvailablePorts();
                UpdateConnectionStatus();
                AddToLog("🚀 Приложение запущено. Выберите COM-порт и нажмите 'ПОДКЛЮЧИТЬ'");
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка инициализации приложения: {ex.Message}");
            }
        }

        private void InitializeTimers()
        {
            _statusTimer = new Timer(1000);
            _statusTimer.Elapsed += new ElapsedEventHandler(StatusTimer_Elapsed);
            _statusTimer.Start();

            _simulationTimer = new Timer(3000);
            _simulationTimer.Elapsed += new ElapsedEventHandler(SimulationTimer_Elapsed);
            _simulationTimer.AutoReset = true;
        }

        private void LoadAvailablePorts()
        {
            try
            {
                ComPortComboBox.Items.Clear();
                foreach (var port in _availablePorts)
                {
                    ComPortComboBox.Items.Add(port);
                }

                if (ComPortComboBox.Items.Count > 0)
                    ComPortComboBox.SelectedIndex = 0;

                AddToLog($"📡 Доступны порты: {string.Join(", ", _availablePorts)}");
                AddToLog("💡 Режим: Имитация работы Arduino");
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка загрузки портов: {ex.Message}");
            }
        }
        #endregion

        #region Таймеры
        private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateStatusTime();
        }

        private void SimulationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SimulateArduinoData();
        }

        private void UpdateStatusTime()
        {
            if (!_isConnected) return;

            try
            {
                var duration = DateTime.Now - _connectionTime;
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        if (ConnectionTimeText != null)
                            ConnectionTimeText.Text = $"Время: {duration:hh\\:mm\\:ss}";
                    }
                    catch (Exception ex)
                    {
                        // Игнорируем ошибки обновления UI
                    }
                }));
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки таймера
            }
        }
        #endregion

        #region Управление подключением
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isConnected)
                {
                    DisconnectFromArduino();
                }
                else
                {
                    ConnectToArduino();
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка в обработчике подключения: {ex.Message}");
            }
        }

        private void ConnectToArduino()
        {
            try
            {
                if (ComPortComboBox.SelectedItem == null)
                {
                    AddToLog("❌ Ошибка: COM-порт не выбран");
                    return;
                }

                string selectedPort = ComPortComboBox.SelectedItem.ToString();
                if (string.IsNullOrEmpty(selectedPort))
                {
                    AddToLog("❌ Ошибка: COM-порт не выбран");
                    return;
                }

                _isSimulationMode = true;
                _isConnected = true;
                _connectionTime = DateTime.Now;
                _messageCount = 0;
                _currentTrack = 1;
                _currentVolume = 15;
                _isPlaying = false;

                UpdateConnectionStatus();
                AddToLog($"✅ Подключено к {selectedPort} (режим имитации)");

                _simulationTimer.Start();

                // Имитация начальных данных от Arduino
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var timer = new Timer(500) { AutoReset = false };
                        timer.Elapsed += new ElapsedEventHandler((s, args) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                AddToLog("← READY: Arduino Audio Controller Started");
                                AddToLog("← TRACK:001");
                                AddToLog("← VOLUME:15");
                                AddToLog("← STATUS:STOP");
                                UpdateUIAfterConnection();
                            }));
                            timer.Dispose();
                        });
                        timer.Start();
                    }
                    catch (Exception ex)
                    {
                        AddToLog($"❌ Ошибка создания таймера: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка подключения: {ex.Message}");
            }
        }

        private void DisconnectFromArduino()
        {
            try
            {
                _isConnected = false;
                _isSimulationMode = false;
                _isPlaying = false;

                _simulationTimer.Stop();
                UpdateConnectionStatus();
                AddToLog("⚠ Отключено от устройства");
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка отключения: {ex.Message}");
            }
        }

        private void UpdateConnectionStatus()
        {
            try
            {
                if (_isConnected)
                {
                    if (ConnectionStatusText != null)
                        ConnectionStatusText.Text = "Подключено (Имитация)";
                    if (ConnectionStatusIndicator != null)
                        ConnectionStatusIndicator.Background = new SolidColorBrush(Colors.Green);
                    if (ConnectButton != null)
                        ConnectButton.Content = "ОТКЛЮЧИТЬ";
                    SetControlsEnabled(true);
                }
                else
                {
                    if (ConnectionStatusText != null)
                        ConnectionStatusText.Text = "Не подключено";
                    if (ConnectionStatusIndicator != null)
                        ConnectionStatusIndicator.Background = new SolidColorBrush(Colors.Red);
                    if (ConnectButton != null)
                        ConnectButton.Content = "ПОДКЛЮЧИТЬ";
                    SetControlsEnabled(false);
                    ResetUI();
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка обновления статуса подключения: {ex.Message}");
            }
        }
        #endregion

        #region Управление UI
        private void UpdateUIAfterConnection()
        {
            try
            {
                if (CurrentTrackText != null)
                    CurrentTrackText.Text = "Трек: 001";

                if (StatusText != null)
                {
                    StatusText.Text = "Статус: Остановлено";
                    StatusText.Foreground = Brushes.LightGray;
                }

                if (VolumeInfoText != null)
                    VolumeInfoText.Text = "Громкость: 15";

                if (VolumeText != null)
                    VolumeText.Text = "15";

                if (VolumeSlider != null)
                {
                    VolumeSlider.ValueChanged -= VolumeSlider_ValueChanged;
                    VolumeSlider.Value = 15;
                    VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
                }
            }
            catch (Exception ex)
            {
                AddToLog($"⚠ Ошибка обновления интерфейса: {ex.Message}");
            }
        }

        private void ResetUI()
        {
            try
            {
                if (CurrentTrackText != null)
                    CurrentTrackText.Text = "Трек: --";

                if (StatusText != null)
                {
                    StatusText.Text = "Статус: --";
                    StatusText.Foreground = Brushes.LightGray;
                }

                if (VolumeInfoText != null)
                    VolumeInfoText.Text = "Громкость: --";

                _isPlaying = false;
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки сброса UI
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            try
            {
                if (PlayButton != null) PlayButton.IsEnabled = enabled;
                if (StopButton != null) StopButton.IsEnabled = enabled;
                if (PrevButton != null) PrevButton.IsEnabled = enabled;
                if (NextButton != null) NextButton.IsEnabled = enabled;
                if (VolumeSlider != null) VolumeSlider.IsEnabled = enabled;
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка установки состояния контролов: {ex.Message}");
            }
        }

        private void UpdatePlaybackStatus(string status)
        {
            try
            {
                if (StatusText == null) return;

                switch (status.ToUpper())
                {
                    case "PLAYING":
                        StatusText.Text = "Статус: Воспроизведение";
                        StatusText.Foreground = Brushes.LightGreen;
                        _isPlaying = true;
                        break;
                    case "STOP":
                        StatusText.Text = "Статус: Остановлено";
                        StatusText.Foreground = Brushes.LightGray;
                        _isPlaying = false;
                        break;
                    case "ERROR":
                        StatusText.Text = "Статус: Ошибка";
                        StatusText.Foreground = Brushes.Red;
                        _isPlaying = false;
                        break;
                    default:
                        StatusText.Text = $"Статус: {status}";
                        StatusText.Foreground = Brushes.White;
                        break;
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка обновления статуса: {ex.Message}");
            }
        }
        #endregion

        #region Имитация Arduino
        private void SimulateArduinoData()
        {
            if (!_isConnected || !_isSimulationMode) return;

            try
            {
                int eventType = _random.Next(0, 15);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        switch (eventType)
                        {
                            case 0 when _isPlaying:
                                AddToLog("← TRACK_END:001");
                                break;
                            case 1:
                                int newVolume = _random.Next(10, 25);
                                if (newVolume != _currentVolume)
                                {
                                    _currentVolume = newVolume;
                                    AddToLog($"← VOLUME:{newVolume}");
                                    if (VolumeInfoText != null)
                                        VolumeInfoText.Text = $"Громкость: {newVolume}";
                                }
                                break;
                            case 2:
                                AddToLog("← SYSTEM:Ready");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        AddToLog($"❌ Ошибка в имитации: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка таймера имитации: {ex.Message}");
            }
        }

        private void SendCommand(string command)
        {
            if (!_isConnected || !_isSimulationMode)
            {
                AddToLog("⚠ Устройство не подключено");
                return;
            }

            try
            {
                AddToLog($"→ {command}");
                ProcessSimulatedCommand(command);
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка отправки: {ex.Message}");
            }
        }

        private void ProcessSimulatedCommand(string command)
        {
            try
            {
                switch (command)
                {
                    case "PLAY":
                        if (!_isPlaying)
                        {
                            _isPlaying = true;
                            AddToLog("← STATUS:PLAYING");
                            UpdatePlaybackStatus("PLAYING");
                            AddToLog("🎵 Начало воспроизведения трека");
                        }
                        break;

                    case "STOP":
                        if (_isPlaying)
                        {
                            _isPlaying = false;
                            AddToLog("← STATUS:STOP");
                            UpdatePlaybackStatus("STOP");
                            AddToLog("⏹ Воспроизведение остановлено");
                        }
                        break;

                    case "NEXT":
                        _currentTrack = _currentTrack < 10 ? _currentTrack + 1 : 1;
                        AddToLog($"← TRACK:{_currentTrack:D3}");
                        if (CurrentTrackText != null)
                            CurrentTrackText.Text = $"Трек: {_currentTrack:D3}";
                        if (_isPlaying)
                        {
                            AddToLog($"🎵 Переход к треку {_currentTrack:D3}");
                        }
                        break;

                    case "PREV":
                        _currentTrack = _currentTrack > 1 ? _currentTrack - 1 : 10;
                        AddToLog($"← TRACK:{_currentTrack:D3}");
                        if (CurrentTrackText != null)
                            CurrentTrackText.Text = $"Трек: {_currentTrack:D3}";
                        if (_isPlaying)
                        {
                            AddToLog($"🎵 Переход к треку {_currentTrack:D3}");
                        }
                        break;

                    case "STATUS":
                        AddToLog($"← TRACK:{_currentTrack:D3}");
                        AddToLog($"← VOLUME:{_currentVolume}");
                        AddToLog(_isPlaying ? "← STATUS:PLAYING" : "← STATUS:STOP");
                        break;

                    default:
                        if (command.StartsWith("VOL:"))
                        {
                            if (int.TryParse(command.Substring(4), out int volume) && volume >= 0 && volume <= 30)
                            {
                                _currentVolume = volume;
                                AddToLog($"← VOLUME:{volume}");
                                if (VolumeInfoText != null)
                                    VolumeInfoText.Text = $"Громкость: {volume}";
                                AddToLog($"🔊 Громкость установлена: {volume}");
                            }
                        }
                        else if (command.StartsWith("TRACK:"))
                        {
                            if (int.TryParse(command.Substring(6), out int track) && track >= 1 && track <= 10)
                            {
                                _currentTrack = track;
                                AddToLog($"← TRACK:{track:D3}");
                                if (CurrentTrackText != null)
                                    CurrentTrackText.Text = $"Трек: {track:D3}";
                                AddToLog($"📁 Выбран трек: {track:D3}");
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка обработки команды: {ex.Message}");
            }
        }
        #endregion

        #region Обработчики управления
        private void PlayButton_Click(object sender, RoutedEventArgs e) => SendCommand("PLAY");
        private void StopButton_Click(object sender, RoutedEventArgs e) => SendCommand("STOP");
        private void PrevButton_Click(object sender, RoutedEventArgs e) => SendCommand("PREV");
        private void NextButton_Click(object sender, RoutedEventArgs e) => SendCommand("NEXT");

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int volume = (int)e.NewValue;

                if (VolumeText != null)
                    VolumeText.Text = volume.ToString();

                if (_isConnected && e.NewValue != e.OldValue)
                {
                    SendCommand($"VOL:{volume}");
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка изменения громкости: {ex.Message}");
            }
        }
        #endregion

        #region Управление логом
        private void AddToLog(string message)
        {
            try
            {
                _messageCount++;
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var logItem = $"{timestamp} | {message}";

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (EventLogTextBox != null)
                        {
                            if (string.IsNullOrEmpty(EventLogTextBox.Text))
                            {
                                EventLogTextBox.Text = logItem;
                            }
                            else
                            {
                                EventLogTextBox.Text = logItem + Environment.NewLine + EventLogTextBox.Text;
                            }

                            if (_messageCount > 1000)
                            {
                                var lines = EventLogTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                                if (lines.Length > 500)
                                {
                                    EventLogTextBox.Text = string.Join(Environment.NewLine, lines, 0, 500);
                                }
                            }
                        }

                        if (MessagesCountText != null)
                            MessagesCountText.Text = $"Сообщений: {_messageCount}";
                    }
                    catch (Exception ex)
                    {
                        // Игнорируем ошибки в логгере
                    }
                }));
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки в логгере
            }
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EventLogTextBox != null)
                    EventLogTextBox.Clear();

                _messageCount = 0;

                if (MessagesCountText != null)
                    MessagesCountText.Text = "Сообщений: 0";

                AddToLog("🗑 Лог очищен");
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка очистки лога: {ex.Message}");
            }
        }

        private void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename = $"audio_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string logContent = EventLogTextBox != null ? EventLogTextBox.Text : "Лог пуст";

                File.WriteAllText(filename, $"=== Лог Arduino Audio Controller ===\nВремя экспорта: {DateTime.Now}\nРежим: Имитация работы\n=====================================\n\n{logContent}");

                AddToLog($"💾 Лог экспортирован: {filename}");
                MessageBox.Show($"Лог успешно экспортирован в файл:\n{filename}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка экспорта: {ex.Message}");
                MessageBox.Show($"Ошибка экспорта лога:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Темы
        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isDarkTheme = !_isDarkTheme;

                if (_isDarkTheme)
                {
                    ApplyDarkTheme();
                    AddToLog("🎨 Переключена тёмная тема");
                }
                else
                {
                    ApplyLightTheme();
                    AddToLog("🎨 Переключена светлая тема");
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка переключения темы: {ex.Message}");
            }
        }

        private void ApplyDarkTheme()
        {
            try
            {
                // Фон
                this.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));

                // Панели
                if (TopPanel != null) TopPanel.Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                if (LeftPanel != null) LeftPanel.Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                if (RightPanel != null) RightPanel.Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                if (InfoPanel != null) InfoPanel.Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                if (StatsPanel != null) StatsPanel.Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));

                // Рамки
                var darkBorder = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40));
                if (TopPanel != null) TopPanel.BorderBrush = darkBorder;
                if (LeftPanel != null) LeftPanel.BorderBrush = darkBorder;
                if (RightPanel != null) RightPanel.BorderBrush = darkBorder;

                // Тексты
                var lightText = Brushes.White;
                var grayText = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));

                if (TitleText != null) TitleText.Foreground = lightText;
                if (ControlHeaderText != null) ControlHeaderText.Foreground = lightText;
                if (LogHeaderText != null) LogHeaderText.Foreground = lightText;
                if (ConnectionStatusText != null) ConnectionStatusText.Foreground = grayText;
                if (VolumeLabelText != null) VolumeLabelText.Foreground = grayText;
                if (VolumeMinText != null) VolumeMinText.Foreground = grayText;
                if (VolumeText != null) VolumeText.Foreground = grayText;
                if (CurrentTrackText != null) CurrentTrackText.Foreground = grayText;
                if (StatusText != null) StatusText.Foreground = grayText;
                if (VolumeInfoText != null) VolumeInfoText.Foreground = grayText;
                if (MessagesCountText != null) MessagesCountText.Foreground = grayText;
                if (ConnectionTimeText != null) ConnectionTimeText.Foreground = grayText;

                // Кнопки
                var darkButton = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
                var greenButton = new SolidColorBrush(Color.FromRgb(0x00, 0xC8, 0x53));
                var redButton = new SolidColorBrush(Color.FromRgb(0xFF, 0x52, 0x52));

                ApplyAllButtonsTheme(darkButton, Brushes.White, new Thickness(0), greenButton, redButton, "☀️ Светлая тема");

                // Лог
                if (EventLogTextBox != null)
                {
                    EventLogTextBox.Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                    EventLogTextBox.Foreground = grayText;
                    EventLogTextBox.BorderBrush = darkBorder;
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка применения тёмной темы: {ex.Message}");
            }
        }

        private void ApplyLightTheme()
        {
            try
            {
                // Фон
                this.Background = new SolidColorBrush(Color.FromRgb(0xFC, 0xFC, 0xFC));

                // Панели
                if (TopPanel != null) TopPanel.Background = Brushes.White;
                if (LeftPanel != null) LeftPanel.Background = Brushes.White;
                if (RightPanel != null) RightPanel.Background = Brushes.White;
                if (InfoPanel != null) InfoPanel.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0xF9, 0xFA));
                if (StatsPanel != null) StatsPanel.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0xF9, 0xFA));

                // Рамки
                var lightBorder = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
                if (TopPanel != null) TopPanel.BorderBrush = lightBorder;
                if (LeftPanel != null) LeftPanel.BorderBrush = lightBorder;
                if (RightPanel != null) RightPanel.BorderBrush = lightBorder;

                // Тексты
                var darkText = new SolidColorBrush(Color.FromRgb(0x21, 0x21, 0x21));
                var mediumText = new SolidColorBrush(Color.FromRgb(0x75, 0x75, 0x75));

                if (TitleText != null) TitleText.Foreground = darkText;
                if (ControlHeaderText != null) ControlHeaderText.Foreground = darkText;
                if (LogHeaderText != null) LogHeaderText.Foreground = darkText;
                if (ConnectionStatusText != null) ConnectionStatusText.Foreground = mediumText;
                if (VolumeLabelText != null) VolumeLabelText.Foreground = mediumText;
                if (VolumeMinText != null) VolumeMinText.Foreground = mediumText;
                if (VolumeText != null) VolumeText.Foreground = mediumText;
                if (CurrentTrackText != null) CurrentTrackText.Foreground = mediumText;
                if (StatusText != null) StatusText.Foreground = mediumText;
                if (VolumeInfoText != null) VolumeInfoText.Foreground = mediumText;
                if (MessagesCountText != null) MessagesCountText.Foreground = mediumText;
                if (ConnectionTimeText != null) ConnectionTimeText.Foreground = mediumText;

                // Кнопки
                var lightButton = Brushes.White;
                var greenButton = new SolidColorBrush(Color.FromRgb(0x00, 0xC8, 0x53));
                var redButton = new SolidColorBrush(Color.FromRgb(0xFF, 0x52, 0x52));

                ApplyAllButtonsTheme(lightButton, darkText, new Thickness(1), greenButton, redButton, "🌙 Тёмная тема");

                // Лог
                if (EventLogTextBox != null)
                {
                    EventLogTextBox.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0xF9, 0xFA));
                    EventLogTextBox.Foreground = darkText;
                    EventLogTextBox.BorderBrush = lightBorder;
                }
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Ошибка применения светлой темы: {ex.Message}");
            }
        }

        using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Timers;

namespace ArduinoAudioController
    {
        public partial class MainWindow : Window
        {
            #region Поля и свойства
            private bool _isConnected = false;
            private bool _isSimulationMode = false;
            private bool _isPlaying = false;
            private bool _isDarkTheme = true;

            private int _messageCount = 0;
            private int _currentTrack = 1;
            private int _currentVolume = 15;

            private DateTime _connectionTime;
            private Timer _statusTimer;
            private Timer _simulationTimer;
            private Random _random = new Random();

            private readonly List<string> _availablePorts = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5" };
            #endregion

            public MainWindow()
            {
                InitializeComponent();
                InitializeApplication();
            }

            #region Инициализация
            private void InitializeApplication()
            {
                try
                {
                    InitializeTimers();
                    LoadAvailablePorts();
                    UpdateConnectionStatus();
                    AddToLog("🚀 Приложение запущено. Выберите COM-порт и нажмите 'ПОДКЛЮЧИТЬ'");
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка инициализации приложения: {ex.Message}");
                }
            }

            private void InitializeTimers()
            {
                _statusTimer = new Timer(1000);
                _statusTimer.Elapsed += new ElapsedEventHandler(StatusTimer_Elapsed);
                _statusTimer.Start();

                _simulationTimer = new Timer(3000);
                _simulationTimer.Elapsed += new ElapsedEventHandler(SimulationTimer_Elapsed);
                _simulationTimer.AutoReset = true;
            }

            private void LoadAvailablePorts()
            {
                try
                {
                    ComPortComboBox.Items.Clear();
                    foreach (var port in _availablePorts)
                    {
                        ComPortComboBox.Items.Add(port);
                    }

                    if (ComPortComboBox.Items.Count > 0)
                        ComPortComboBox.SelectedIndex = 0;

                    AddToLog($"📡 Доступны порты: {string.Join(", ", _availablePorts)}");
                    AddToLog("💡 Режим: Имитация работы Arduino");
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка загрузки портов: {ex.Message}");
                }
            }
            #endregion

            #region Таймеры
            private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
            {
                UpdateStatusTime();
            }

            private void SimulationTimer_Elapsed(object sender, ElapsedEventArgs e)
            {
                SimulateArduinoData();
            }

            private void UpdateStatusTime()
            {
                if (!_isConnected) return;

                try
                {
                    var duration = DateTime.Now - _connectionTime;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            if (ConnectionTimeText != null)
                                ConnectionTimeText.Text = $"Время: {duration:hh\\:mm\\:ss}";
                        }
                        catch (Exception ex)
                        {
                            // Игнорируем ошибки обновления UI
                        }
                    }));
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки таймера
                }
            }
            #endregion

            #region Управление подключением
            private void ConnectButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    if (_isConnected)
                    {
                        DisconnectFromArduino();
                    }
                    else
                    {
                        ConnectToArduino();
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка в обработчике подключения: {ex.Message}");
                }
            }

            private void ConnectToArduino()
            {
                try
                {
                    if (ComPortComboBox.SelectedItem == null)
                    {
                        AddToLog("❌ Ошибка: COM-порт не выбран");
                        return;
                    }

                    string selectedPort = ComPortComboBox.SelectedItem.ToString();
                    if (string.IsNullOrEmpty(selectedPort))
                    {
                        AddToLog("❌ Ошибка: COM-порт не выбран");
                        return;
                    }

                    _isSimulationMode = true;
                    _isConnected = true;
                    _connectionTime = DateTime.Now;
                    _messageCount = 0;
                    _currentTrack = 1;
                    _currentVolume = 15;
                    _isPlaying = false;

                    UpdateConnectionStatus();
                    AddToLog($"✅ Подключено к {selectedPort} (режим имитации)");

                    _simulationTimer.Start();

                    // Имитация начальных данных от Arduino
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            var timer = new Timer(500) { AutoReset = false };
                            timer.Elapsed += new ElapsedEventHandler((s, args) =>
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    AddToLog("← READY: Arduino Audio Controller Started");
                                    AddToLog("← TRACK:001");
                                    AddToLog("← VOLUME:15");
                                    AddToLog("← STATUS:STOP");
                                    UpdateUIAfterConnection();
                                }));
                                timer.Dispose();
                            });
                            timer.Start();
                        }
                        catch (Exception ex)
                        {
                            AddToLog($"❌ Ошибка создания таймера: {ex.Message}");
                        }
                    }));
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка подключения: {ex.Message}");
                }
            }

            private void DisconnectFromArduino()
            {
                try
                {
                    _isConnected = false;
                    _isSimulationMode = false;
                    _isPlaying = false;

                    _simulationTimer.Stop();
                    UpdateConnectionStatus();
                    AddToLog("⚠ Отключено от устройства");
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка отключения: {ex.Message}");
                }
            }

            private void UpdateConnectionStatus()
            {
                try
                {
                    if (_isConnected)
                    {
                        if (ConnectionStatusText != null)
                            ConnectionStatusText.Text = "Подключено (Имитация)";
                        if (ConnectionStatusIndicator != null)
                            ConnectionStatusIndicator.Background = new SolidColorBrush(Colors.Green);
                        if (ConnectButton != null)
                            ConnectButton.Content = "ОТКЛЮЧИТЬ";
                        SetControlsEnabled(true);
                    }
                    else
                    {
                        if (ConnectionStatusText != null)
                            ConnectionStatusText.Text = "Не подключено";
                        if (ConnectionStatusIndicator != null)
                            ConnectionStatusIndicator.Background = new SolidColorBrush(Colors.Red);
                        if (ConnectButton != null)
                            ConnectButton.Content = "ПОДКЛЮЧИТЬ";
                        SetControlsEnabled(false);
                        ResetUI();
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка обновления статуса подключения: {ex.Message}");
                }
            }
            #endregion

            #region Управление UI
            private void UpdateUIAfterConnection()
            {
                try
                {
                    if (CurrentTrackText != null)
                        CurrentTrackText.Text = "Трек: 001";

                    if (StatusText != null)
                    {
                        StatusText.Text = "Статус: Остановлено";
                        StatusText.Foreground = Brushes.LightGray;
                    }

                    if (VolumeInfoText != null)
                        VolumeInfoText.Text = "Громкость: 15";

                    if (VolumeText != null)
                        VolumeText.Text = "15";

                    if (VolumeSlider != null)
                    {
                        VolumeSlider.ValueChanged -= VolumeSlider_ValueChanged;
                        VolumeSlider.Value = 15;
                        VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"⚠ Ошибка обновления интерфейса: {ex.Message}");
                }
            }

            private void ResetUI()
            {
                try
                {
                    if (CurrentTrackText != null)
                        CurrentTrackText.Text = "Трек: --";

                    if (StatusText != null)
                    {
                        StatusText.Text = "Статус: --";
                        StatusText.Foreground = Brushes.LightGray;
                    }

                    if (VolumeInfoText != null)
                        VolumeInfoText.Text = "Громкость: --";

                    _isPlaying = false;
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки сброса UI
                }
            }

            private void SetControlsEnabled(bool enabled)
            {
                try
                {
                    if (PlayButton != null) PlayButton.IsEnabled = enabled;
                    if (StopButton != null) StopButton.IsEnabled = enabled;
                    if (PrevButton != null) PrevButton.IsEnabled = enabled;
                    if (NextButton != null) NextButton.IsEnabled = enabled;
                    if (VolumeSlider != null) VolumeSlider.IsEnabled = enabled;
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка установки состояния контролов: {ex.Message}");
                }
            }

            private void UpdatePlaybackStatus(string status)
            {
                try
                {
                    if (StatusText == null) return;

                    switch (status.ToUpper())
                    {
                        case "PLAYING":
                            StatusText.Text = "Статус: Воспроизведение";
                            StatusText.Foreground = Brushes.LightGreen;
                            _isPlaying = true;
                            break;
                        case "STOP":
                            StatusText.Text = "Статус: Остановлено";
                            StatusText.Foreground = Brushes.LightGray;
                            _isPlaying = false;
                            break;
                        case "ERROR":
                            StatusText.Text = "Статус: Ошибка";
                            StatusText.Foreground = Brushes.Red;
                            _isPlaying = false;
                            break;
                        default:
                            StatusText.Text = $"Статус: {status}";
                            StatusText.Foreground = Brushes.White;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка обновления статуса: {ex.Message}");
                }
            }
            #endregion

            #region Имитация Arduino
            private void SimulateArduinoData()
            {
                if (!_isConnected || !_isSimulationMode) return;

                try
                {
                    int eventType = _random.Next(0, 15);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            switch (eventType)
                            {
                                case 0 when _isPlaying:
                                    AddToLog("← TRACK_END:001");
                                    break;
                                case 1:
                                    int newVolume = _random.Next(10, 25);
                                    if (newVolume != _currentVolume)
                                    {
                                        _currentVolume = newVolume;
                                        AddToLog($"← VOLUME:{newVolume}");
                                        if (VolumeInfoText != null)
                                            VolumeInfoText.Text = $"Громкость: {newVolume}";
                                    }
                                    break;
                                case 2:
                                    AddToLog("← SYSTEM:Ready");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            AddToLog($"❌ Ошибка в имитации: {ex.Message}");
                        }
                    }));
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка таймера имитации: {ex.Message}");
                }
            }

            private void SendCommand(string command)
            {
                if (!_isConnected || !_isSimulationMode)
                {
                    AddToLog("⚠ Устройство не подключено");
                    return;
                }

                try
                {
                    AddToLog($"→ {command}");
                    ProcessSimulatedCommand(command);
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка отправки: {ex.Message}");
                }
            }

            private void ProcessSimulatedCommand(string command)
            {
                try
                {
                    switch (command)
                    {
                        case "PLAY":
                            if (!_isPlaying)
                            {
                                _isPlaying = true;
                                AddToLog("← STATUS:PLAYING");
                                UpdatePlaybackStatus("PLAYING");
                                AddToLog("🎵 Начало воспроизведения трека");
                            }
                            break;

                        case "STOP":
                            if (_isPlaying)
                            {
                                _isPlaying = false;
                                AddToLog("← STATUS:STOP");
                                UpdatePlaybackStatus("STOP");
                                AddToLog("⏹ Воспроизведение остановлено");
                            }
                            break;

                        case "NEXT":
                            _currentTrack = _currentTrack < 10 ? _currentTrack + 1 : 1;
                            AddToLog($"← TRACK:{_currentTrack:D3}");
                            if (CurrentTrackText != null)
                                CurrentTrackText.Text = $"Трек: {_currentTrack:D3}";
                            if (_isPlaying)
                            {
                                AddToLog($"🎵 Переход к треку {_currentTrack:D3}");
                            }
                            break;

                        case "PREV":
                            _currentTrack = _currentTrack > 1 ? _currentTrack - 1 : 10;
                            AddToLog($"← TRACK:{_currentTrack:D3}");
                            if (CurrentTrackText != null)
                                CurrentTrackText.Text = $"Трек: {_currentTrack:D3}";
                            if (_isPlaying)
                            {
                                AddToLog($"🎵 Переход к треку {_currentTrack:D3}");
                            }
                            break;

                        case "STATUS":
                            AddToLog($"← TRACK:{_currentTrack:D3}");
                            AddToLog($"← VOLUME:{_currentVolume}");
                            AddToLog(_isPlaying ? "← STATUS:PLAYING" : "← STATUS:STOP");
                            break;

                        default:
                            if (command.StartsWith("VOL:"))
                            {
                                if (int.TryParse(command.Substring(4), out int volume) && volume >= 0 && volume <= 30)
                                {
                                    _currentVolume = volume;
                                    AddToLog($"← VOLUME:{volume}");
                                    if (VolumeInfoText != null)
                                        VolumeInfoText.Text = $"Громкость: {volume}";
                                    AddToLog($"🔊 Громкость установлена: {volume}");
                                }
                            }
                            else if (command.StartsWith("TRACK:"))
                            {
                                if (int.TryParse(command.Substring(6), out int track) && track >= 1 && track <= 10)
                                {
                                    _currentTrack = track;
                                    AddToLog($"← TRACK:{track:D3}");
                                    if (CurrentTrackText != null)
                                        CurrentTrackText.Text = $"Трек: {track:D3}";
                                    AddToLog($"📁 Выбран трек: {track:D3}");
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка обработки команды: {ex.Message}");
                }
            }
            #endregion

            #region Обработчики управления
            private void PlayButton_Click(object sender, RoutedEventArgs e) => SendCommand("PLAY");
            private void StopButton_Click(object sender, RoutedEventArgs e) => SendCommand("STOP");
            private void PrevButton_Click(object sender, RoutedEventArgs e) => SendCommand("PREV");
            private void NextButton_Click(object sender, RoutedEventArgs e) => SendCommand("NEXT");

            private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                try
                {
                    int volume = (int)e.NewValue;

                    if (VolumeText != null)
                        VolumeText.Text = volume.ToString();

                    if (_isConnected && e.NewValue != e.OldValue)
                    {
                        SendCommand($"VOL:{volume}");
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка изменения громкости: {ex.Message}");
                }
            }
            #endregion

            #region Управление логом
            private void AddToLog(string message)
            {
                try
                {
                    _messageCount++;
                    var timestamp = DateTime.Now.ToString("HH:mm:ss");
                    var logItem = $"{timestamp} | {message}";

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (EventLogTextBox != null)
                            {
                                if (string.IsNullOrEmpty(EventLogTextBox.Text))
                                {
                                    EventLogTextBox.Text = logItem;
                                }
                                else
                                {
                                    EventLogTextBox.Text = logItem + Environment.NewLine + EventLogTextBox.Text;
                                }

                                if (_messageCount > 1000)
                                {
                                    var lines = EventLogTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                                    if (lines.Length > 500)
                                    {
                                        EventLogTextBox.Text = string.Join(Environment.NewLine, lines, 0, 500);
                                    }
                                }
                            }

                            if (MessagesCountText != null)
                                MessagesCountText.Text = $"Сообщений: {_messageCount}";
                        }
                        catch (Exception ex)
                        {
                            // Игнорируем ошибки в логгере
                        }
                    }));
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки в логгере
                }
            }

            private void ClearLogButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    if (EventLogTextBox != null)
                        EventLogTextBox.Clear();

                    _messageCount = 0;

                    if (MessagesCountText != null)
                        MessagesCountText.Text = "Сообщений: 0";

                    AddToLog("🗑 Лог очищен");
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка очистки лога: {ex.Message}");
                }
            }

            private void ExportLogButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    string filename = $"audio_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    string logContent = EventLogTextBox != null ? EventLogTextBox.Text : "Лог пуст";

                    File.WriteAllText(filename, $"=== Лог Arduino Audio Controller ===\nВремя экспорта: {DateTime.Now}\nРежим: Имитация работы\n=====================================\n\n{logContent}");

                    AddToLog($"💾 Лог экспортирован: {filename}");
                    MessageBox.Show($"Лог успешно экспортирован в файл:\n{filename}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка экспорта: {ex.Message}");
                    MessageBox.Show($"Ошибка экспорта лога:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            #endregion

            #region Темы
            private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    _isDarkTheme = !_isDarkTheme;

                    if (_isDarkTheme)
                    {
                        ApplyDarkTheme();
                        AddToLog("🎨 Переключена тёмная тема");
                    }
                    else
                    {
                        ApplyLightTheme();
                        AddToLog("🎨 Переключена светлая тема");
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка переключения темы: {ex.Message}");
                }
            }

            private void ApplyDarkTheme()
            {
                try
                {
                    // Фон
                    this.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));

                    // Панели
                    if (TopPanel != null) TopPanel.Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                    if (LeftPanel != null) LeftPanel.Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                    if (RightPanel != null) RightPanel.Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                    if (InfoPanel != null) InfoPanel.Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                    if (StatsPanel != null) StatsPanel.Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));

                    // Рамки
                    var darkBorder = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40));
                    if (TopPanel != null) TopPanel.BorderBrush = darkBorder;
                    if (LeftPanel != null) LeftPanel.BorderBrush = darkBorder;
                    if (RightPanel != null) RightPanel.BorderBrush = darkBorder;

                    // Тексты
                    var lightText = Brushes.White;
                    var grayText = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));

                    if (TitleText != null) TitleText.Foreground = lightText;
                    if (ControlHeaderText != null) ControlHeaderText.Foreground = lightText;
                    if (LogHeaderText != null) LogHeaderText.Foreground = lightText;
                    if (ConnectionStatusText != null) ConnectionStatusText.Foreground = grayText;
                    if (VolumeLabelText != null) VolumeLabelText.Foreground = grayText;
                    if (VolumeMinText != null) VolumeMinText.Foreground = grayText;
                    if (VolumeText != null) VolumeText.Foreground = grayText;
                    if (CurrentTrackText != null) CurrentTrackText.Foreground = grayText;
                    if (StatusText != null) StatusText.Foreground = grayText;
                    if (VolumeInfoText != null) VolumeInfoText.Foreground = grayText;
                    if (MessagesCountText != null) MessagesCountText.Foreground = grayText;
                    if (ConnectionTimeText != null) ConnectionTimeText.Foreground = grayText;

                    // Кнопки
                    var darkButton = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
                    var greenButton = new SolidColorBrush(Color.FromRgb(0x00, 0xC8, 0x53));
                    var redButton = new SolidColorBrush(Color.FromRgb(0xFF, 0x52, 0x52));

                    ApplyAllButtonsTheme(darkButton, Brushes.White, new Thickness(0), greenButton, redButton, "☀️ Светлая тема");

                    // Лог
                    if (EventLogTextBox != null)
                    {
                        EventLogTextBox.Background = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                        EventLogTextBox.Foreground = grayText;
                        EventLogTextBox.BorderBrush = darkBorder;
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка применения тёмной темы: {ex.Message}");
                }
            }

            private void ApplyLightTheme()
            {
                try
                {
                    // Фон
                    this.Background = new SolidColorBrush(Color.FromRgb(0xFC, 0xFC, 0xFC));

                    // Панели
                    if (TopPanel != null) TopPanel.Background = Brushes.White;
                    if (LeftPanel != null) LeftPanel.Background = Brushes.White;
                    if (RightPanel != null) RightPanel.Background = Brushes.White;
                    if (InfoPanel != null) InfoPanel.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0xF9, 0xFA));
                    if (StatsPanel != null) StatsPanel.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0xF9, 0xFA));

                    // Рамки
                    var lightBorder = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
                    if (TopPanel != null) TopPanel.BorderBrush = lightBorder;
                    if (LeftPanel != null) LeftPanel.BorderBrush = lightBorder;
                    if (RightPanel != null) RightPanel.BorderBrush = lightBorder;

                    // Тексты
                    var darkText = new SolidColorBrush(Color.FromRgb(0x21, 0x21, 0x21));
                    var mediumText = new SolidColorBrush(Color.FromRgb(0x75, 0x75, 0x75));

                    if (TitleText != null) TitleText.Foreground = darkText;
                    if (ControlHeaderText != null) ControlHeaderText.Foreground = darkText;
                    if (LogHeaderText != null) LogHeaderText.Foreground = darkText;
                    if (ConnectionStatusText != null) ConnectionStatusText.Foreground = mediumText;
                    if (VolumeLabelText != null) VolumeLabelText.Foreground = mediumText;
                    if (VolumeMinText != null) VolumeMinText.Foreground = mediumText;
                    if (VolumeText != null) VolumeText.Foreground = mediumText;
                    if (CurrentTrackText != null) CurrentTrackText.Foreground = mediumText;
                    if (StatusText != null) StatusText.Foreground = mediumText;
                    if (VolumeInfoText != null) VolumeInfoText.Foreground = mediumText;
                    if (MessagesCountText != null) MessagesCountText.Foreground = mediumText;
                    if (ConnectionTimeText != null) ConnectionTimeText.Foreground = mediumText;

                    // Кнопки
                    var lightButton = Brushes.White;
                    var greenButton = new SolidColorBrush(Color.FromRgb(0x00, 0xC8, 0x53));
                    var redButton = new SolidColorBrush(Color.FromRgb(0xFF, 0x52, 0x52));

                    ApplyAllButtonsTheme(lightButton, darkText, new Thickness(1), greenButton, redButton, "🌙 Тёмная тема");

                    // Лог
                    if (EventLogTextBox != null)
                    {
                        EventLogTextBox.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0xF9, 0xFA));
                        EventLogTextBox.Foreground = darkText;
                        EventLogTextBox.BorderBrush = lightBorder;
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка применения светлой темы: {ex.Message}");
                }
            }

            private void ApplyAllButtonsTheme(Brush background, Brush foreground, Thickness borderThickness,
                                             Brush playButtonBackground, Brush stopButtonBackground, string themeButtonContent)
            {
                try
                {
                    // Все обычные кнопки
                    var ordinaryButtons = new[] { ConnectButton, PrevButton, NextButton, ClearLogButton, ExportLogButton };
                    var borderBrush = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));

                    foreach (var button in ordinaryButtons)
                    {
                        if (button != null)
                        {
                            button.Background = background;
                            button.Foreground = foreground;
                            button.BorderBrush = borderBrush;
                            button.BorderThickness = borderThickness;
                        }
                    }

                    // Специальные кнопки управления воспроизведением
                    if (PlayButton != null)
                    {
                        PlayButton.Background = playButtonBackground;
                        PlayButton.Foreground = Brushes.White;
                        PlayButton.BorderThickness = new Thickness(0);
                    }

                    if (StopButton != null)
                    {
                        StopButton.Background = stopButtonBackground;
                        StopButton.Foreground = Brushes.White;
                        StopButton.BorderThickness = new Thickness(0);
                    }

                    // Кнопка переключения темы
                    if (ThemeToggleButton != null)
                    {
                        ThemeToggleButton.Background = background;
                        ThemeToggleButton.Foreground = foreground;
                        ThemeToggleButton.Content = themeButtonContent;
                    }
                }
                catch (Exception ex)
                {
                    AddToLog($"❌ Ошибка применения темы кнопок: {ex.Message}");
                }
            }
            #endregion

            #region Завершение работы
            protected override void OnClosed(EventArgs e)
            {
                try
                {
                    _statusTimer?.Stop();
                    _statusTimer?.Dispose();
                    _simulationTimer?.Stop();
                    _simulationTimer?.Dispose();
                    DisconnectFromArduino();
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки при закрытии
                }
                base.OnClosed(e);
            }
            #endregion
        }
    }
    #endregion

    #region Завершение работы
    protected override void OnClosed(EventArgs e)
        {
            try
            {
                _statusTimer?.Stop();
                _statusTimer?.Dispose();
                _simulationTimer?.Stop();
                _simulationTimer?.Dispose();
                DisconnectFromArduino();
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки при закрытии
            }
            base.OnClosed(e);
        }
        #endregion
    }
}