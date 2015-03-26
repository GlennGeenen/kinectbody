using System;

namespace KinectBody
{
    class Program
    {
        private static KinectServer m_server = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting the server..");

            KinectController kinectController = new KinectController();

            m_server = new KinectServer();

            kinectController.KinectReceivedBody += KinectReceivedBody;

            Console.WriteLine("Server started. Press any key to exit...");
            Console.Read();

            m_server.stop();
        }

        private static void KinectReceivedBody(object sender, KinectEventArgs e)
        {
            m_server.sendMessage(e.bodyList);
        }
    }
}
