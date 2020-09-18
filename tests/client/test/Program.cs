using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lidgren.Network;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("BSCM");
            NetClient client_connection = new NetClient(config);
            client_connection.Start();
            client_connection.Connect(host: "192.168.1.9", port: 14242);
            Console.WriteLine("Client ready");
            Thread.Sleep(1000);


            NetOutgoingMessage ms = client_connection.CreateMessage();
            ms.Write("ready");
            client_connection.SendMessage(ms, NetDeliveryMethod.UnreliableSequenced);

            NetIncomingMessage message;
            while (true)
            {
                while ((message = client_connection.ReadMessage()) != null)
                {
                    Console.WriteLine("message available");
                    Console.WriteLine(message.MessageType);
                    string m = message.ReadString();
                    Console.WriteLine(m);
                    if (m == "ping")
                    {
                        NetOutgoingMessage msg = client_connection.CreateMessage();
                        msg.Write("pong");
                        client_connection.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced);
                    }
                    else if (m == "start")
                    {
                        Console.WriteLine("=============== START GAME =====================");
                    }
                    else if (message.MessageType == NetIncomingMessageType.Data)
                    {
                        string[] data = m.Split(';');
                        data[0] = (float.Parse(data[0]) + 0.5).ToString();
                        NetOutgoingMessage msg = client_connection.CreateMessage();
                        msg.Write(string.Join("; ", data));
                        client_connection.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced);
                    }
                }
            }
        }
    }
}
