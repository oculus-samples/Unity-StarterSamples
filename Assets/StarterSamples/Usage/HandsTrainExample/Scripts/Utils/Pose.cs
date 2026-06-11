// Copyright (c) Meta Platforms, Inc. and affiliates.


using Meta.XR.Samples;
using UnityEngine;

namespace OculusSampleFramework
{
    [MetaCodeSample("StarterSample-HandsTrain")]
    public class Pose
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public Pose()
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
        }

        public Pose(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
