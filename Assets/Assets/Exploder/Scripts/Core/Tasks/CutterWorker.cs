// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

#if UNITY_WEBGL
#define DISABLE_MULTITHREADING
#endif

#if !DISABLE_MULTITHREADING

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Exploder
{
    class CutterWorker
    {
        private readonly HashSet<MeshObject> newFragments;
        private readonly HashSet<MeshObject> meshToRemove;
        private readonly HashSet<MeshObject> meshSet;
        private readonly MeshCutter cutter;
        private readonly CuttingPlane cuttingPlane;
        private readonly Core core;
        private volatile bool running = false;
        private int cutAttempt;

        private readonly ManualResetEvent mre = new ManualResetEvent(false);

        private Thread thread;

        public CutterWorker(Core core, CuttingPlane cuttingPlane)
        {
            cutter = new MeshCutter();
            cutter.Init(512, 512);
            newFragments = new HashSet<MeshObject>();
            meshToRemove = new HashSet<MeshObject>();
            meshSet = new HashSet<MeshObject>();

            this.cuttingPlane = cuttingPlane;
            this.core = core;

            thread = new Thread(ThreadRun);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Init()
        {
            meshSet.Clear();
            running = false;
            cutAttempt = 0;
        }

        public void AddMesh(MeshObject meshObject)
        {
            Debug.Assert(!running);
            meshSet.Add(meshObject);
        }

        public void Run()
        {
            running = true;
            Thread.MemoryBarrier();
            mre.Set();
        }

        void ThreadRun()
        {
            mre.WaitOne();

            try
            {
                Cut();
            }
            finally
            {
                running = false;
                Thread.MemoryBarrier();
                mre.Reset();
                thread = new Thread(ThreadRun);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public bool IsFinished()
        {
            return !running;
        }

        public HashSet<MeshObject> GetResults()
        {
            Debug.Assert(IsFinished());
            return meshSet;
        }

        public void Terminate()
        {
            mre.Close();
        }

        private void Cut()
        {
            bool cutting = true;
            var cycleCounter = 0;
            cutAttempt = 0;

            while (cutting)
            {
                cycleCounter++;

                if (cycleCounter > core.parameters.TargetFragments)
                {
                    ExploderUtils.Log("Explode Infinite loop!");
                    break;
                }

                newFragments.Clear();
                meshToRemove.Clear();

                cutting = false;

                foreach (var mesh in meshSet)
                {
                    if (core.targetFragments[mesh.id] > 1)
                    {
                        var plane = cuttingPlane.GetPlane(mesh.mesh, cutAttempt);

                        var triangulateHoles = true;
                        var crossSectionVertexColour = Color.white;
                        var crossSectionUV = new Vector4(0, 0, 1, 1);

                        if (mesh.option)
                        {
                            triangulateHoles = !mesh.option.Plane2D;
                            crossSectionVertexColour = mesh.option.CrossSectionVertexColor;
                            crossSectionUV = mesh.option.CrossSectionUV;
                            core.splitMeshIslands |= mesh.option.SplitMeshIslands;
                        }

                        if (core.parameters.Use2DCollision)
                        {
                            triangulateHoles = false;
                        }

                        List<ExploderMesh> meshes = null;
                        cutter.Cut(mesh.mesh, mesh.transform, plane, triangulateHoles, core.parameters.DisableTriangulation, ref meshes, crossSectionVertexColour, crossSectionUV);

                        cutting = true;

                        if (meshes != null)
                        {
                            foreach (var cutterMesh in meshes)
                            {
                                newFragments.Add(new MeshObject
                                    {
                                        mesh = cutterMesh,

                                        material = mesh.material,
                                        transform = mesh.transform,
                                        id = mesh.id,
                                        original = mesh.original,
                                        skinnedOriginal = mesh.skinnedOriginal,
                                        bakeObject = mesh.bakeObject,

                                        parent = mesh.transform.parent,
                                        position = mesh.transform.position,
                                        rotation = mesh.transform.rotation,
                                        localScale = mesh.transform.localScale,

                                        option = mesh.option,
                                    });
                            }

                            meshToRemove.Add(mesh);
                            core.targetFragments[mesh.id] -= 1;
                        }
                        else
                        {
                            cutAttempt++;
                        }
                    }
                }

                meshSet.ExceptWith(meshToRemove);
                meshSet.UnionWith(newFragments);
            }
        }
    }
}
#endif
