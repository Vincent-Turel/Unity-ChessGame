// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using UnityMesh = UnityEngine.Mesh;
using System.Collections.Generic;

namespace Exploder
{
    public class ExploderMesh
    {
        public int[] triangles;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uv;
        public Vector4[] tangents;
        public Color32[] colors32;

        public Vector3 centroid;
        public Vector3 min, max;

        public ExploderMesh()
        {
        }

        public ExploderMesh(UnityMesh unityMesh)
        {
            triangles = unityMesh.triangles;
            vertices = unityMesh.vertices;
            normals = unityMesh.normals;
            uv = unityMesh.uv;
            tangents = unityMesh.tangents;
            colors32 = unityMesh.colors32;

            CalculateCentroid(new List<Vector3>(vertices), ref centroid, ref min, ref max);
        }

        public static void CalculateCentroid(List<Vector3> vertices, ref Vector3 ctr, ref Vector3 min, ref Vector3 max)
        {
            ctr = Vector3.zero;
            var length = vertices.Count;
            min.Set(float.MaxValue, float.MaxValue, float.MaxValue);
            max.Set(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < length; i++)
            {
                if (min.x > vertices[i].x)
                    min.x = vertices[i].x;
                if (min.y > vertices[i].y)
                    min.y = vertices[i].y;
                if (min.z > vertices[i].z)
                    min.z = vertices[i].z;

                if (max.x < vertices[i].x)
                    max.x = vertices[i].x;
                if (max.y < vertices[i].y)
                    max.y = vertices[i].y;
                if (max.z < vertices[i].z)
                    max.z = vertices[i].z;

                ctr += vertices[i];
            }

            ctr /= length;
        }

        public UnityMesh ToUnityMesh()
        {
            return new UnityMesh
            {
                vertices = vertices,
                normals = normals,
                uv = uv,
                tangents = tangents,
                colors32 = colors32,
                triangles = triangles
            };
        }
    }
}
