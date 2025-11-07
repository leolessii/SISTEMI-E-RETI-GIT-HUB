using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServerConsoleApp
{
    //Il listener agisce come server e si mette in ascolto
    public class SocketListener
    {

        public static int Main(String[] args)
        {
            StartServer();
            return 0;
        }

        public static void StartServer()
        {
            //Prende la lista di tutti gli host del computer TRAMITE classe DNS che fa tutto sola
            //IPHostEntry host = Dns.GetHostEntry("localhost");

            //Della lista prendiamo quello all'indice 1(loopback)
            //IPAddress ipAddress = host.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("10.73.0.21");

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

                //Messaggio attesa connessione
                //Il server è in ascolto
                Console.WriteLine("Waiting for a connection . . .");

                //Primitiva del sever
                Socket handler = Listener.Accept();

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
                }
                //Conferma messaggio e stampa
                //0 è la posizione degli argomenti dopo la virgola
                Console.WriteLine("Text received : {0}", data);

                //Ritrasforma messaggio in byte
                byte[] msg = Encoding.ASCII.GetBytes(data);

                //Primitiva manda al Client torna uguale ECHO
                handler.Send(msg);

                //Chiude entrambe le socket
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }

    }
}