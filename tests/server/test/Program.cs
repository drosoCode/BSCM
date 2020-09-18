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
            NetPeerConfiguration config = new NetPeerConfiguration("BSCM");
            config.Port = 14242;

            NetServer server_connection = new NetServer(config);
            server_connection.Start();
            Console.WriteLine("Server ready");
            NetIncomingMessage message;
            while (true)
            {
                while ((message = server_connection.ReadMessage()) != null)
                {
                    Console.WriteLine("message available");
                    Console.WriteLine(message.MessageType);
                    string m = message.ReadString();
                    Console.WriteLine(m);
                    if(m == "ready")
                    {
                        NetOutgoingMessage msg = server_connection.CreateMessage();
                        msg.Write("start");
                        server_connection.SendMessage(msg, server_connection.Connections[0], NetDeliveryMethod.UnreliableSequenced);
                    }
                    else if(message.MessageType == NetIncomingMessageType.Data)
                    {
                        string[] data = m.Split(';');
                        data[0] = (float.Parse(data[0]) + 0.5).ToString();
                        NetOutgoingMessage msg = server_connection.CreateMessage();
                        msg.Write(string.Join("; ", data));
                        server_connection.SendMessage(msg, server_connection.Connections[0], NetDeliveryMethod.UnreliableSequenced);
                    }
                }
            }
        }
    }
}
