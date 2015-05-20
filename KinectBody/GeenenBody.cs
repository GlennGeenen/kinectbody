using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect;

namespace KinectBody
{
    public class GeenenBody
    {
        public IReadOnlyDictionary<JointType, Joint> Joints;
        public ulong TrackingId;
        public string HandLeftState;
        public string HandRightState;

        public GeenenBody(Body body, IReadOnlyDictionary<JointType, Joint> joints)
        {
            this.Joints = joints;
            this.TrackingId = body.TrackingId;
            this.HandLeftState = body.HandLeftState.ToString();
            this.HandRightState = body.HandRightState.ToString();
        }

    }
}
