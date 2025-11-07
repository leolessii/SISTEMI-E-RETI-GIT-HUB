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

namespace ClientWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket Sender;
        public byte[] msgBytes;
        public byte[] buffer;
        public Socket handler;
        public MainWindow()
        {
            InitializeComponent();
            btnInvioMessaggio.IsEnabled = false;
        }

        public void StartClient()
        {
            //Messaggio da mandare
            buffer = new byte[1024];

            try
            {
                //Esattamente stessa cosa di prima
                //IPHostEntry host = Dns.GetHostEntry("localhost");
                //IPAddress ipAddress = host.AddressList[0];
                IPAddress ipAddress = IPAddress.Parse("10.73.0.9");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //Primitiva CLient a remoteEP che indirizzo loopback
                    Sender.Connect(remoteEP);
                    //COnferma
                    MessageBox.Show("connsessione stabilita");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERRORE in fase di connsessione/trasferimento: " + e.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRORE generale: ", ex.Message);
            }

        }

        private void btnInvioMessaggio_Click(object sender, RoutedEventArgs e)
        {
            msgBytes = Encoding.ASCII.GetBytes(txtMessaggioDaInviare.Text);
            Sender.Send(msgBytes);

            int bytesRec = Sender.Receive(buffer);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            txtMessaggioServer.Text = response;
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
            btnInvioMessaggio.IsEnabled = true;
        }
    }
}