// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using UnityTransform = UnityEngine.Transform;

namespace Exploder
{
    public struct ExploderTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
        public Transform parent;

        public ExploderTransform(UnityTransform unityTransform)
        {
            position = unityTransform.position;
            rotation = unityTransform.rotation;
            localScale = unityTransform.localScale;
            parent = unityTransform.parent;
        }

        public Vector3 InverseTransformDirection(Vector3 dir)
        {
            return Quaternion.Inverse(rotation) * dir;
        }

        public Vector3 InverseTransformPoint(Vector3 pnt)
        {
            var sInv = new Vector3(1 / localScale.x, 1 / localScale.y, 1 / localScale.z);
            return Vector3.Scale(sInv, (Quaternion.Inverse(rotation) * (pnt - position)));
        }

        public Vector3 TransformPoint(Vector3 pnt)
        {
            var m = Matrix4x4.TRS(position, rotation, localScale);
            return m.MultiplyPoint3x4(pnt);
        }
    }
}
