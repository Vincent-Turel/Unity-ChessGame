// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace Exploder
{
    class CutterST : ExploderTask
    {
        protected readonly HashSet<MeshObject> newFragments;
        protected readonly HashSet<MeshObject> meshToRemove;
        protected readonly MeshCutter cutter;
        protected readonly CuttingPlane cuttingPlane;
        private int cutAttempt;

        public CutterST(Core Core) : base(Core)
        {
            // init cutter
            cutter = new MeshCutter();
            cutter.Init(512, 512);
            newFragments = new HashSet<MeshObject>();
            meshToRemove = new HashSet<MeshObject>();
            cuttingPlane = new CuttingPlane(Core);
        }

        public override TaskType Type { get { return TaskType.ProcessCutter; } }

        public override void Init()
        {
            base.Init();
            newFragments.Clear();
            meshToRemove.Clear();
            cutAttempt = 0;
        }

        public override bool Run(float frameBudget)
        {
            if (Cut(frameBudget))
            {
                Watch.Stop();
                return true;
            }

            return false;
        }

        protected virtual bool Cut(float frameBudget)
        {
            bool cutting = true;
            var cycleCounter = 0;
            bool timeBudgetStop = false;
            cutAttempt = 0;

            while (cutting)
            {
                cycleCounter++;

                if (cycleCounter > core.parameters.TargetFragments)
                {
                    ExploderUtils.Log("Explode Infinite loop!");
                    return true;
                }

                newFragments.Clear();
                meshToRemove.Clear();

                cutting = false;

                foreach (var mesh in core.meshSet)
                {
                    if (!core.targetFragments.ContainsKey(mesh.id))
                    {
                        Debug.Assert(false);
                    }

                    if (core.targetFragments[mesh.id] > 1)
                    {
                        var meshes = CutSingleMesh(mesh);

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

                            // computation took more than settings.FrameBudget ... 
                            if (Watch.ElapsedMilliseconds > frameBudget && cycleCounter > 2)
                            {
                                timeBudgetStop = true;
                                break;
                            }
                        }
                    }
                }

                core.meshSet.ExceptWith(meshToRemove);
                core.meshSet.UnionWith(newFragments);

                if (timeBudgetStop)
                {
                    break;
                }
            }

            // explosion is finished
            return !timeBudgetStop;
        }

        protected List<ExploderMesh> CutSingleMesh(MeshObject mesh)
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

            triangulateHoles &= !core.parameters.DisableTriangulation;

            List<ExploderMesh> meshes = null;
            cutter.Cut(mesh.mesh, mesh.transform, plane, triangulateHoles, core.parameters.DisableTriangulation, ref meshes, crossSectionVertexColour, crossSectionUV);

            if (meshes == null)
            {
                cutAttempt++;
            }

            return meshes;
        }
    }
}
