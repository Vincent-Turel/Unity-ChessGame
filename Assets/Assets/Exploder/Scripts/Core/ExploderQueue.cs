// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace Exploder
{
    public class ExploderParams
    {
        public Vector3 Position;
        public Vector3 ForceVector;
        public Vector3 CubeRadius;
        public Vector3 HitPosition;
        public Vector3 ShotDir;

        public float Force;
        public float FrameBudget;
        public float Radius;
        public float BulletSize;

        public int id;
        public int TargetFragments;
        public int FragmentPoolSize;

        public ExploderObject.ThreadOptions ThreadOptions;

        public ExploderObject.OnExplosion Callback;
        public FragmentOption FragmentOptions;
        public FragmentDeactivation FragmentDeactivation;
        public FragmentSFX FragmentSFX;
        public ExploderObject.CuttingStyleOption CuttingStyle;

        public GameObject[] Targets;
        public GameObject ExploderGameObject;

        public bool UseCubeRadius;
        public bool DontUseTag;
        public bool UseForceVector;
        public bool ExplodeSelf;
        public bool HideSelf;
        public bool DestroyOriginalObject;
        public bool SplitMeshIslands;
        public bool Use2DCollision;
        public bool DisableRadiusScan;
        public bool UniformFragmentDistribution;
        public bool DisableTriangulation;
        public bool Crack;
        public bool processing;

        public ExploderParams(ExploderObject exploder)
        {
            Position = ExploderUtils.GetCentroid(exploder.gameObject);
            DontUseTag = exploder.DontUseTag;
            Radius = exploder.Radius;
            UseCubeRadius = exploder.UseCubeRadius;
            CubeRadius = exploder.CubeRadius;
            ForceVector = exploder.ForceVector;
            UseForceVector = exploder.UseForceVector;
            Force = exploder.Force;
            FrameBudget = exploder.FrameBudget;
            TargetFragments = exploder.TargetFragments;
            ExplodeSelf = exploder.ExplodeSelf;
            HideSelf = exploder.HideSelf;
            ThreadOptions = exploder.ThreadOption;
            DestroyOriginalObject = exploder.DestroyOriginalObject;
            SplitMeshIslands = exploder.SplitMeshIslands;
            FragmentOptions = exploder.FragmentOptions.Clone();
            FragmentDeactivation = exploder.FragmentDeactivation.Clone();
            FragmentSFX = exploder.FragmentSFX.Clone();
            Use2DCollision = exploder.Use2DCollision;
            FragmentPoolSize = exploder.FragmentPoolSize;
            DisableRadiusScan = exploder.DisableRadiusScan;
            UniformFragmentDistribution = exploder.UniformFragmentDistribution;
            DisableTriangulation = exploder.DisableTriangulation;
            ExploderGameObject = exploder.gameObject;
            CuttingStyle = exploder.CuttingStyle;
        }
    }

    class ExploderQueue
    {
        private readonly Queue<ExploderParams> queue;
        private readonly Core core;

        public ExploderQueue(Core core)
        {
            this.core = core;
            queue = new Queue<ExploderParams>();
        }

        public void Enqueue(ExploderObject exploderObject, ExploderObject.OnExplosion callback, bool crack, params GameObject[] target)
        {
            var settings = new ExploderParams(exploderObject)
            {
                Callback = callback,
                Targets = target,
                Crack = crack,
                processing = false
            };

            queue.Enqueue(settings);
            ProcessQueue();
        }

        void ProcessQueue()
        {
            if (queue.Count > 0)
            {
                var peek = queue.Peek();

                if (!peek.processing)
                {
                    peek.id = Random.Range(int.MinValue, int.MaxValue);
                    peek.processing = true;
                    core.StartExplosionFromQueue(peek);
                }
            }
        }

        public void OnExplosionFinished(int id, long ellapsedMS)
        {
            var explosion = queue.Dequeue();
            ExploderUtils.Assert(explosion.id == id, "Explosion id mismatch!");

            if (explosion.Callback != null)
            {
                explosion.Callback(ellapsedMS, explosion.Crack ? ExploderObject.ExplosionState.ObjectCracked : ExploderObject.ExplosionState.ExplosionFinished);
            }

            ProcessQueue();
        }
    }
}
