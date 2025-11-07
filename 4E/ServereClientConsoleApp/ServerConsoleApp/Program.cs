using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;

public class SocketClient
{
    public static void Main(string[] args)
    {
        StartClient();
    }

    public static void StartClient()
    {
        //Messaggio da mandare
        byte[] buffer = new byte[1024];

        try
        {
            //Esattamente stessa cosa di prima
            //IPHostEntry host = Dns.GetHostEntry("localhost");
            //IPAddress ipAddress = host.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("10.73.0.21");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Primitiva CLient a remoteEP che indirizzo loopback
                sender.Connect(remoteEP);
                //COnferma
                Console.WriteLine("Connessione stabilita con {0}", sender.RemoteEndPoint.ToString());
                //Messaggio da mandare con EOF alla fine
                string message = "Ullallailolaiilolaiola<EOF>";
                //Converte in bytr e lo manda
                byte[] msgBytes = Encoding.ASCII.GetBytes(message);
                //Primitiva dell'invio
                sender.Send(msgBytes);
                //
                Console.WriteLine("Inviati {0} byte al server.", sender.RemoteEndPoint.ToString());
                //Riceve echo server
                int bytesRec = sender.Receive(buffer);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                Console.WriteLine("Ricevuto dal server: {0}", response);
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

        Console.WriteLine("\n Premi un tasto per uscire...");
        Console.ReadKey();

    }
}