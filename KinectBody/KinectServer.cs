
using System;
using System.Collections.Generic;
using Fleck;

namespace KinectBody
{

    public class KinectServer
    {
        private WebSocketServer m_server = null;
        private List<IWebSocketConnection> m_allSockets = new List<IWebSocketConnection>();

        public KinectServer()
        {
            m_server = new WebSocketServer("ws://0.0.0.0:3333/kinect");
            m_server.Start(socket =>
            {
                socket.OnOpen = () => {
                    m_allSockets.Add(socket);
                    Console.WriteLine("Open!");
                };
                socket.OnClose = () => {
                    m_allSockets.Remove(socket);
                    Console.WriteLine("Close!");
                };
                socket.OnMessage = message => {
                    Console.WriteLine(message);
                };
            });
        }

        public void sendMessage(string message)
        {
            foreach (IWebSocketConnection s in m_allSockets) {
                s.Send(message);
            }
        }

        public void stop()
        {
            foreach (IWebSocketConnection s in m_allSockets)
            {
                s.Close();
            }
            m_server.Dispose();
        }
    }
}
