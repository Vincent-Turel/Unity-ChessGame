// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace Exploder
{
    struct MeshObject
    {
        public ExploderMesh mesh;
        public Material material;
        public ExploderTransform transform;
        public Transform parent;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
        public GameObject original;
        public ExploderOption option;
        public GameObject skinnedOriginal;
        public GameObject bakeObject;

        public float distanceRatio;
        public int id;
    }
}
