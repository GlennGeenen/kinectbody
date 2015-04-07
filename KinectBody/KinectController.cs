using System;
using System.Collections.Generic;

using Microsoft.Kinect;
using Newtonsoft.Json;

namespace KinectBody
{
    public class KinectEventArgs : EventArgs
    {
        public string bodyList { get; private set; }

        public KinectEventArgs(List<GeenenBody> bodyList)
        {
            this.bodyList = JsonConvert.SerializeObject(bodyList);
        }
    }

    public class KinectController
    {
        private KinectSensor kinectSensor = null;
        private BodyFrameReader reader = null;
        private CoordinateMapper coordinateMapper = null;
        private Body[] bodies = null;

        public event EventHandler<KinectEventArgs> KinectReceivedBody;

        public KinectController()
        {
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                this.coordinateMapper = this.kinectSensor.CoordinateMapper;

                this.kinectSensor.Open();

                this.bodies = new Body[this.kinectSensor.BodyFrameSource.BodyCount];

                this.reader = this.kinectSensor.BodyFrameSource.OpenReader();
                this.reader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        public void Close()
        {
            if (this.reader != null)
            {
                this.reader.Dispose();
                this.reader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrameReference frameReference = e.FrameReference;
            try
            {
                BodyFrame frame = frameReference.AcquireFrame();
                if (frame != null)
                {
                    // BodyFrame is IDisposable
                    using (frame)
                    {
                        frame.GetAndRefreshBodyData(this.bodies);

                        List<GeenenBody> bodyList = new List<GeenenBody>();

                        foreach (Body body in this.bodies)
                        {
                            if (body.IsTracked && body.Joints[JointType.SpineMid].Position.Z > 1)
                            {
                                bodyList.Add(new GeenenBody(body));
                            }
                        }

                        KinectReceivedBody(this, new KinectEventArgs(bodyList));
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
