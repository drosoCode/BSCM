using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using System.Linq;
using System.Threading;

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
        private ScoreController ScoreController;
        private SongController SongController;
        private bool clientReady = false;
        private long latency = 0;

        public void startSong()
        {
            if (ScoreController != null || SongController != null)
                return; // startSong already executed for this song

            ScoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
            if (ScoreController == null)
                Plugin.Log.Error("Couldn't find ScoreController object");

            SongController = Resources.FindObjectsOfTypeAll<SongController>().FirstOrDefault();
            if (SongController == null)
                Plugin.Log.Error("Couldn't find SongController object");

            setGameStatus(false);
            Plugin.Log.Info("Pausing Game");

            if(isServer)
            {
                Plugin.Log.Info("Waiting for client");
                // wait for client to switch to ready state
                while (!clientReady) { }
                // start game on client
                Plugin.Log.Info("Starting client game");
                clientReady = false;
                sendData("start");
                // wait for estimated latency
                Plugin.Log.Info("Waiting for latency corretion");
                Thread.Sleep(Convert.ToInt32(latency));
                // start game on server
                Plugin.Log.Info("Starting server game");
                setGameStatus(true);
                Plugin.Log.Info("Server game started");
            }
            else
            {
                // send ready state to server
                sendData("ready");
                Plugin.Log.Info("Client ready [c]");
            }
        }

        private long getTimestamp()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        private void setGameStatus(bool enabled)
        {
            if (enabled)
            {
                ScoreController.enabled = true;
                SongController.StartSong();
            }
            else
            {
                ScoreController.enabled = false;
                SongController.PauseSong();
            }
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
            // Plugin.Log.Info("Received message: " + message);

            if(isServer && message == "ready")
            {
                // when client is ready
                latency = getTimestamp();
                sendData("ping");
                Plugin.Log.Info("Sending ping");
            }
            else if(!isServer && message == "ping")
            {
                // reply to server ping
                sendData("pong");
                Plugin.Log.Info("Sending pong");
            }
            else if (isServer && message == "pong")
            {
                // when client replies to pong
                latency = getTimestamp() - latency;
                clientReady = true;
                Plugin.Log.Info("Client ready");
            }
            else if(!isServer && message == "start")
            {
                // start the client game
                setGameStatus(true);
                Plugin.Log.Info("Client game started");
            }
            else
            {
                // set new coords for remote saber
                string[] data = message.Split(';');
                latestPosition = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                latestRotation = new Quaternion(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]));
            }
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
                    Plugin.Log.Error("Error while starting WS Server");
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
                    Plugin.Log.Error("Error while connecting to WS Server");
                }
            }

            Plugin.Log.Info("New Multiplayer");
        }

        public void sendCoords(Vector3 pos, Quaternion rot)
        {
            string data = pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.x + ";" + rot.y + ";" + rot.z + ";" + rot.w;
            // Plugin.Log.Info("Sending data: " + data);
            sendData(data);
        }

        private void sendData(string data)
        {
            if (isServer)
                receiver.sendData(data);
            else
                ws.Send(data);
        }

    }
}
