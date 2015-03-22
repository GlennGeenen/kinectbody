using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KinectBody
{
    class Program
    {
        private static SocketServer m_server = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting the server..");

            KinectController kinectController = new KinectController();

            m_server = new SocketServer();

            kinectController.KinectReceivedBody += KinectReceivedBody;

            Console.WriteLine("Server started. Press any key to exit...");
            Console.Read();

            m_server.Stop();

        }

        private static void KinectReceivedBody(object sender, KinectEventArgs e)
        {
            m_server.Broadcast(e.bodyList);
        }
    }
}
