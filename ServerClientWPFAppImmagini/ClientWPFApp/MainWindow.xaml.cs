using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO;
using Microsoft.Win32;

namespace ClientWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;
        private string ip;
        public MainWindow()
        {
            InitializeComponent();
            btnInviaImmagine.IsEnabled = false;
        }

        public async void StartClient()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, 12345);
                _stream = _client.GetStream();
                _cts = new CancellationTokenSource();
                _ = Task.Run(() => ReceiveImages(_cts.Token));
                btnStart.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void ReceiveImages(CancellationToken token)
        {
            var lengthBuffer = new byte[4];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await ReadExactlyAsync(_stream, lengthBuffer, 0, 4, token);
                    int imageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    var imageBuffer = new byte[imageLength];
                    await ReadExactlyAsync(_stream, imageBuffer, 0, imageLength, token);

                    Dispatcher.Invoke(() => {
                        DisplayImage(imageBuffer, imgReceived);
                    });
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartClient();
        }

        private void btnCambiaIp_Click(object sender, RoutedEventArgs e)
        {
            while (string.IsNullOrEmpty(txtIp.Text))
            {
                MessageBox.Show("Ip non valido");
            }
            btnInviaImmagine.IsEnabled = true;
            ip = txtIp.Text;
        }

        private void btnScegliImmagine_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Immagini|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var imageData = File.ReadAllBytes(openFileDialog.FileName);
                    DisplayImage(imageData, imgSent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore invio: {ex.Message}");
                }
            }
        }

        private async void btnInviaImmagine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var imageData = File.ReadAllBytes(imgSent.Source.ToString());
                await SendImage(imageData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore invio: {ex.Message}");
            }
        }

        private async Task SendImage(byte[] imageData)
        {
            if (_stream == null) return;

            var lengthData = BitConverter.GetBytes(imageData.Length);
            await _stream.WriteAsync(lengthData, 0, lengthData.Length);
            await _stream.WriteAsync(imageData, 0, imageData.Length);
        }

        private void DisplayImage(byte[] imageData, Image target)
        {
            using (var ms = new MemoryStream(imageData))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                target.Source = bitmap;
            }
        }

        private static async Task ReadExactlyAsync(NetworkStream stream, byte[] buffer, int offset, int count, CancellationToken token)
        {
            while (count > 0)
            {
                int read = await stream.ReadAsync(buffer, offset, count, token);
                if (read == 0) throw new EndOfStreamException();
                offset += read;
                count -= read;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _cts?.Cancel();
            _client?.Close();
            base.OnClosing(e);
        }
    }
}