
using System;
using System.Collections.Generic;
using System.Net;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace KinectBody
{
    public class SocketServer
    {
        private WebSocketServer m_server = null;

        public SocketServer()
        {
            m_server = new WebSocketServer("ws://localhost:3333");
            m_server.AddWebSocketService<SendMessage> ("/kinect");
            m_server.Start();

            m_server.WaitTime = TimeSpan.FromHours(1);
        }

        public void Stop()
        {
            m_server.Stop();
        }

        public void Broadcast(string message)
        {
            m_server.WebSocketServices.Broadcast(message);
        }
    }

    public class SendMessage : WebSocketBehavior
    {
        public SendMessage()
        {
        }
    }
}
