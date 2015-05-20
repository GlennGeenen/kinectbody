using System;
using System.Collections.Generic;

using Microsoft.Kinect;
using Newtonsoft.Json;
using System.Linq;

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
        private KinectJointFilter[] filters = null;

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

            this.filters = new KinectJointFilter[] {new KinectJointFilter(), new KinectJointFilter()};
            foreach (var filter in this.filters)
            {
                filter.Init();
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

                        // Find bodies in the middle
                        this.bodies.OrderBy(x => Math.Abs(x.Joints[JointType.SpineMid].Position.X));

                        List<GeenenBody> bodyList = new List<GeenenBody>();

                        int i = 0;
                        foreach (Body body in this.bodies)
                        {
                            if (body.IsTracked)
                            {
                                var splinePos = body.Joints[JointType.SpineMid].Position;

                                // Check Position
                                // Between 1 and 3 meters in front of kinect
                                // Between -2 and 2 meters aside of kinect
                                if (splinePos.Z > 1 &&
                                    splinePos.Z < 3 &&
                                    splinePos.Y < 2 &&
                                    splinePos.Y > -2)
                                {
                                    // Filter joints
                                    this.filters[i].UpdateFilter(body);
                                    bodyList.Add(new GeenenBody(body, this.filters[i].getJoints()));

                                    // Only return 2 bodies
                                    if (2 == ++i)
                                    {
                                        break;
                                    }
                                }

                            }
                        }

                        // Left player always first
                        bodyList.OrderBy(x => x.Joints[JointType.SpineMid].Position.X);

                        foreach (var body in bodyList)
                        {

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
