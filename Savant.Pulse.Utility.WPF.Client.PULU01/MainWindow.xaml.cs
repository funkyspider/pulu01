using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Savant.Pulse.Utility.WPF.Client.PULU01.Configuration;
using Savant.Pulse.Utility.WPF.Client.PULU01.Services;
using Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces;

namespace Savant.Pulse.Utility.WPF.Client.PULU01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IApplicationService _applicationService;
        private readonly IProgressTrackingService _progressTrackingService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isProcessing;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            InitializeEventHandlers();
        }

        private void InitializeServices()
        {
            // Simple dependency injection setup for .NET 4.8 - services will be created in constructor
            var mockApiService = new MockApiClientService();
            var csvReaderService = new CsvReaderService();
            var processingPersistenceService = new ProcessingPersistenceService();
            _progressTrackingService = new ProgressTrackingService();
            var processingWorkerService = new ProcessingWorkerService(
                mockApiService, 
                processingPersistenceService, 
                _progressTrackingService);
            
            _applicationService = new ApplicationService(
                csvReaderService,
                processingWorkerService);
        }

        private void InitializeEventHandlers()
        {
            _progressTrackingService.ProgressUpdated += OnProgressUpdated;
        }

        private void OnProgressUpdated(object sender, ProgressEventArgs e)
        {
            // Update UI on the dispatcher thread
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainProgressBar.Value = e.PercentComplete;
                ProcessedCountLabel.Text = $"Processed: {e.ProcessedCount:N0}";
                SuccessCountLabel.Text = $"Success: {e.SuccessCount:N0}";
                FailedCountLabel.Text = $"Failed: {e.FailedCount:N0}";
                SkippedCountLabel.Text = $"Skipped: {e.SkippedCount:N0}";
                StatusBarText.Text = $"Processing... {e.PercentComplete:F1}% complete";
            }));
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Select CSV file to process"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilePathTextBox.Text))
            {
                MessageBox.Show("Please select a CSV file to process.", "File Required", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!File.Exists(FilePathTextBox.Text))
            {
                MessageBox.Show("The selected file does not exist.", "File Not Found", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int threadCount;
            if (!int.TryParse(ThreadCountTextBox.Text, out threadCount) || threadCount < 1 || threadCount > 50)
            {
                MessageBox.Show("Thread count must be a number between 1 and 50.", "Invalid Thread Count", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await StartProcessingAsync(threadCount);
        }

        private async Task StartProcessingAsync(int threadCount)
        {
            try
            {
                _isProcessing = true;
                _cancellationTokenSource = new CancellationTokenSource();

                // Update UI
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                StatusLabel.Text = "Starting processing...";
                LogTextBox.Text = "";
                ResetProgressDisplay();

                // Create configuration
                var configuration = new AppConfiguration
                {
                    FilePath = FilePathTextBox.Text,
                    ThreadCount = threadCount
                };

                LogMessage("Starting PULU01 processing...");
                LogMessage($"File: {configuration.FilePath}");
                LogMessage($"Threads: {configuration.ThreadCount}");
                LogMessage("");

                // Start processing
                await _applicationService.RunAsync(configuration, _cancellationTokenSource.Token);

                LogMessage("");
                LogMessage("Processing completed successfully!");
                StatusLabel.Text = "Processing completed";
            }
            catch (OperationCanceledException)
            {
                LogMessage("");
                LogMessage("Processing was cancelled by user");
                StatusLabel.Text = "Processing cancelled";
            }
            catch (Exception ex)
            {
                LogMessage("");
                LogMessage($"Error during processing: {ex.Message}");
                StatusLabel.Text = "Processing failed";
                MessageBox.Show($"An error occurred during processing: {ex.Message}", "Processing Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessing = false;
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                StatusBarText.Text = "Ready";
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null && _isProcessing)
            {
                _cancellationTokenSource.Cancel();
                StatusLabel.Text = "Stopping processing...";
                LogMessage("Stop requested - waiting for graceful shutdown...");
            }
        }

        private void ResetProgressDisplay()
        {
            MainProgressBar.Value = 0;
            ProcessedCountLabel.Text = "Processed: 0";
            SuccessCountLabel.Text = "Success: 0";
            FailedCountLabel.Text = "Failed: 0";
            SkippedCountLabel.Text = "Skipped: 0";
        }

        private void LogMessage(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LogTextBox.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
                LogTextBox.ScrollToEnd();
            }));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_isProcessing)
            {
                var result = MessageBox.Show(
                    "Processing is currently running. Do you want to stop and exit?", 
                    "Confirm Exit", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _cancellationTokenSource?.Cancel();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }
    }
}