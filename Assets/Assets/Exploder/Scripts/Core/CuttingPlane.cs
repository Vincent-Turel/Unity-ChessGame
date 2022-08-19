// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Exploder
{
    class CuttingPlane
    {
        private readonly System.Random random;
        private readonly Exploder.Plane plane;
        private readonly Core core;

        private static Vector3[] rectAxis = 
        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
        };

        public CuttingPlane(Core core)
        {
            random = new System.Random();
            plane = new Exploder.Plane(Vector3.one, Vector3.zero);
            this.core = core;
        }

        private Exploder.Plane GetRandomPlane(ExploderMesh mesh)
        {
            var randomPlaneNormal = new Vector3((float)random.NextDouble() * 2.0f - 1.0f,
                                                (float)random.NextDouble() * 2.0f - 1.0f,
                                                (float)random.NextDouble() * 2.0f - 1.0f);

            plane.Set(randomPlaneNormal, mesh.centroid);

            return plane;
        }

        private Exploder.Plane GetRectangularRegularPlane(ExploderMesh mesh, int attempt)
        {
            var diffX = mesh.max.x - mesh.min.x;
            var diffY = mesh.max.y - mesh.min.y;
            var diffZ = mesh.max.z - mesh.min.z;
            var axis = 0;

            if (diffX > diffY)
            {
                if (diffX > diffZ)
                {
                    axis = 0;
                }
                else
                {
                    axis = 2;
                }
            }
            else
            {
                if (diffY > diffZ)
                {
                    axis = 1;
                }
                else
                {
                    axis = 2;
                }
            }

            axis += attempt;

            if (axis > 2)
            {
                return GetRandomPlane(mesh);
            }

            plane.Set(rectAxis[axis], mesh.centroid);
            return plane;
        }

        private Exploder.Plane GetRectangularRandom(ExploderMesh mesh, int attempt)
        {
            var axis = random.Next(0, 3);

            axis += attempt;

            if (axis > 2)
            {
                return GetRandomPlane(mesh);
            }

            plane.Set(rectAxis[axis], mesh.centroid);
            return plane;
        }

        public Exploder.Plane GetPlane(ExploderMesh mesh, int attempt)
        {
            switch (core.parameters.CuttingStyle)
            {
                case ExploderObject.CuttingStyleOption.Random:
                    return GetRandomPlane(mesh);

                case ExploderObject.CuttingStyleOption.RectangularRandom:
                    return GetRectangularRandom(mesh, attempt);

                case ExploderObject.CuttingStyleOption.RectangularRegular:
                    return GetRectangularRegularPlane(mesh, attempt);
            }

            return null;
        }
    }
}
