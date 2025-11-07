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
using System.Windows.Interop;

namespace ServerWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket handler;
        public string ip;
        public MainWindow()
        {
            InitializeComponent();
            btnInvioMessaggio.IsEnabled = false;
        }

        public void StartServer()
        {
            //Prende la lista di tutti gli host del computer TRAMITE classe DNS che fa tutto sola
            //IPHostEntry host = Dns.GetHostEntry("localhost");

            //Della lista prendiamo quello all'indice 1(loopback)
            //IPAddress ipAddress = host.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse(ip);

            //Associ indirizzo IP alla porta 11000
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            try
            {
                //Genera una socket Ipv4/Ipv6 (a se4conda dell'indirizzo
                //Di tipo "stream" (TCP) e protocollo TCP
                //Prima sokcet con tutti i parametri indirizzo ip, tipo trasmissione (duplex), Tipo di protocollo
                Socket Listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //Primitva del server
                Listener.Bind(localEndPoint);

                //Massimo 10 processi in attessa
                Listener.Listen(10);

                //Primitiva del sever
                handler = Listener.Accept();

                MessageBox.Show("connsessione stabilita");

                //inizializzo la stringa che conterrà i dati ricevuti
                string data = null;

                //buffer che conterrà di volta in volta i byte ricevuti
                byte[] bytes = null;

                //per ogni byte che ricevo aggiungo alla stringa e ovviamente converto da byte in stringa
                while (true)
                {
                    //Messagio del Client
                    bytes = new byte[1024];

                    //riceve i byte dalla socket
                    //Primitiva prendi messaggio e lo metto nel bytesRec è la lunghezza quindi quanti byte effettivi sono arrivati
                    int bytesRec = handler.Receive(bytes);

                    //Metti in stringa il messaggio con la lunghezza del messaggio
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    //Se trova EOF chiude comunicazione
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }

                    txtMessaggioClient.Text = data;
                    if (txtMessaggioClient.Text != string.Empty)
                    {
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private void btnInvioMessaggio_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = Encoding.ASCII.GetBytes(txtMessaggioDaInviare.Text);
            handler.Send(msg);
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