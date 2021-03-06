﻿using System;
using Lidgren.Network;
using UnityEngine;
using System.Linq;
using System.Threading;

namespace BSCM
{
    public class Multiplayer
    {
        private NetClient client_connection = null;
        private NetServer server_connection = null;
        private bool isServer = PluginConfig.Instance.isServer;
        private Vector3 latestPosition;
        private Quaternion latestRotation;
        private ScoreController ScoreController;
        private SongController SongController;
        private bool clientReady = false;
        private long latency = 0;

        public Multiplayer()
        {
            if (isServer)
            {
                try
                {
                    NetPeerConfiguration config = new NetPeerConfiguration("BSCM");
                    config.Port = PluginConfig.Instance.port;

                    server_connection = new NetServer(config);
                    server_connection.Start();
                }
                catch
                {
                    Plugin.Log.Error("Error while starting Server");
                }
                Plugin.Log.Info("Server Ready");
            }
            else
            {
                try
                {
                    var config = new NetPeerConfiguration("BSCM");
                    client_connection = new NetClient(config);
                    client_connection.Start();
                    client_connection.Connect(host: PluginConfig.Instance.url, port: PluginConfig.Instance.port);
                    Thread.Sleep(1000);
                    Plugin.Log.Info("Client Ready");
                }
                catch
                {
                    Plugin.Log.Error("Error while connecting to Server");
                }
            }
            Plugin.Log.Info("Multiplayer Ready !");
        }

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
                while (!clientReady) {
                    checkMessages();
                }
                // start game on client
                Plugin.Log.Info("Starting client game");
                clientReady = false;
                sendData("start");
                // wait for estimated latency
                Plugin.Log.Info("Latency: "+latency);
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

        public Vector3 getLatestPosition()
        {
            return latestPosition;
        }
        public Quaternion getLatestRotation()
        {
            return latestRotation;
        }
        public void sendCoords(Vector3 pos, Quaternion rot)
        {
            string data = pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.x + ";" + rot.y + ";" + rot.z + ";" + rot.w;
            sendData(data);
        }

        public void stop()
        {
            if (isServer)
                server_connection.Shutdown("Plugin Exit");
            else
                client_connection.Disconnect("Plugin Exit");
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

        private void parseMessage(string message)
        {
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

        public void checkMessages()
        {
            NetIncomingMessage message;
            if(isServer)
            {
                while ((message = server_connection.ReadMessage()) != null)
                {
                    if (message.MessageType == NetIncomingMessageType.Data)
                        parseMessage(message.ReadString());
                    else
                        Plugin.Log.Debug(message.MessageType.ToString() + ":" + message.ReadString());
                }
            }
            else
            {
                while ((message = client_connection.ReadMessage()) != null)
                {
                    if (message.MessageType == NetIncomingMessageType.Data)
                        parseMessage(message.ReadString());
                    else
                        Plugin.Log.Debug(message.MessageType.ToString() + ":" + message.ReadString());
                }
            }
        }

        private void sendData(string data)
        {
            if (isServer)
            {
                NetOutgoingMessage msg = server_connection.CreateMessage();
                msg.Write(data);
                if (server_connection.Connections.Count > 0)
                    server_connection.SendMessage(msg, server_connection.Connections[0], NetDeliveryMethod.ReliableOrdered);
                else
                    Plugin.Log.Critical("Client not connected");
            }
            else
            {
                NetOutgoingMessage msg = client_connection.CreateMessage();
                msg.Write(data);
                client_connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

    }
}
