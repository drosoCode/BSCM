using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;

namespace BSCM
{
    public class MessageReceiver : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Plugin.Multi.parseMessage(e.Data);
        }

        public void sendData(string data)
        {
            Send(data);
        }
    }

    public class Multiplayer
    {
        private MessageReceiver receiver = null;
        private WebSocket ws = null;
        private bool isServer = false;
        private Vector3 latestPosition;
        private Quaternion latestRotation;

        public void startGame()
        {
            Plugin.Log.Info("Game Started");
        }

        public Vector3 getLatestPosition()
        {
            return latestPosition;
        }
        public Quaternion getLatestRotation()
        {
            return latestRotation;
        }

        public void parseMessage(string message)
        {
            Plugin.Log.Info("Received message: " + message);
            string[] data = message.Split(';');
            latestPosition = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
            latestRotation = new Quaternion(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]));
        }

        public Multiplayer()
        {
            if (PluginConfig.Instance.isServer)
            {
                isServer = true;
                try
                {
                    receiver = new MessageReceiver();
                    var wssv = new WebSocketServer(PluginConfig.Instance.url);
                    wssv.AddWebSocketService("/", () => receiver);
                    wssv.Start();
                    Plugin.Log.Info("WS Server Ready");
                }
                catch
                {
                    Plugin.Log.Info("Error while starting WS Server");
                }
                //Console.ReadKey(true);
                //wssv.Stop();
            }
            else
            {
                try
                {
                    ws = new WebSocket(PluginConfig.Instance.url);
                    ws.OnMessage += (sender, e) =>
                        parseMessage(e.Data);

                    ws.Connect();
                    Plugin.Log.Info("WS Client Ready");
                }
                catch
                {
                    Plugin.Log.Info("Error while connecting to WS Server");
                }
            }
            Plugin.Log.Info("New Multiplayer");
        }

        public void sendCoords(Vector3 pos, Quaternion rot)
        {
            string data = pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.x + ";" + rot.y + ";" + rot.z + ";" + rot.w;
            Plugin.Log.Info("Sending data: " + data);

            if (isServer)
                receiver.sendData(data);
            else
                ws.Send(data);
        }

    }
}
