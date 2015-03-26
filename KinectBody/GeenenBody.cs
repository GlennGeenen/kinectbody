﻿using System;
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

        public GeenenBody(Body body)
        {
            this.Joints = body.Joints;
            this.TrackingId = body.TrackingId;
        }

    }
}
