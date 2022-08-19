// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

#if UNITY_WEBGL
#define DISABLE_MULTITHREADING
#endif

#if !DISABLE_MULTITHREADING

using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Exploder
{
    class CutterMT : CutterST
    {
        public CutterMT(Core Core) : base(Core)
        {
            splitIDs = new int[2];

            THREAD_MAX = Mathf.Clamp((int)Core.parameters.ThreadOptions + 2, 1, 4);

            // UnityEngine.Debug.LogFormat("Exploder: using {0} threads.", THREAD_MAX-1);

            workers = new CutterWorker[THREAD_MAX - 1];

            for (int i = 0; i < THREAD_MAX - 1; i++)
            {
                workers[i] = new CutterWorker(Core, new CuttingPlane(Core));
            };
        }

        public override TaskType Type { get { return TaskType.ProcessCutter; } }

        protected readonly int THREAD_MAX;
        protected readonly CutterWorker[] workers;

        private readonly int[] splitIDs;
        private readonly Stopwatch localWatch = new Stopwatch();
        private bool cutInitialized = false;

        public override void Init()
        {
            base.Init();
            cutInitialized = false;

            foreach (var worker in workers)
            {
                worker.Init();
            }

            localWatch.Reset();
        }

        public override void OnDestroy()
        {
            foreach (var worker in workers)
            {
                worker.Terminate();
            }
        }

        public override bool Run(float frameBudget)
        {
            var mainCut = Cut(frameBudget);

            if (mainCut)
            {
                var finished = true;
                foreach (var worker in workers)
                {
                    finished &= worker.IsFinished();
                }

                if (finished)
                {
                    foreach (var worker in workers)
                    {
                        core.meshSet.UnionWith(worker.GetResults());
                    }

                    Watch.Stop();
                    return true;
                }
            }

            return false;
        }

        protected override bool Cut(float frameBudget)
        {
            if (cutInitialized)
            {
                return true;
            }

            localWatch.Start();

            cutInitialized = true;

            //
            // thread_max = <2, 4>
            //
            Debug.Assert(THREAD_MAX > 1, "At least one worker is required!");

            if (core.parameters.TargetFragments < 2 ||
                core.meshSet.Count == core.parameters.TargetFragments)
            {
                return true;
            }

            //
            // cut object if necessary
            //
            var cuts = THREAD_MAX - 1 - core.meshSet.Count;
//            Debug.AssertFormat(cuts <= 2, "Invalid number of cuts: {0}", cuts);
//            var cutsTotal = cuts;

            if (cuts > core.parameters.TargetFragments-1)
            {
                cuts = core.parameters.TargetFragments-1;
            }

            var cycleCounter = 0;

            while (cuts > 0)
            {
                newFragments.Clear();

                foreach (var mesh in core.meshSet)
                {
                    var meshes = CutSingleMesh(mesh);
                    cycleCounter++;

                    if (cycleCounter > core.parameters.TargetFragments)
                    {
                        ExploderUtils.Log("Explode Infinite loop!");
                        cuts = 0;
                        break;
                    }

                    if (meshes != null)
                    {
                        cuts--;

                        var ids = SplitMeshTargetFragments(mesh.id);
                        var ctr = 0;

                        foreach (var cutterMesh in meshes)
                        {
                            var fragment = new MeshObject
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
                            };

                            fragment.id = ids[ctr++];
                            newFragments.Add(fragment);
                        }

                        meshToRemove.Add(mesh);
                        break;
                    }
                }

                core.meshSet.ExceptWith(meshToRemove);
                core.meshSet.UnionWith(newFragments);
            }

            if (core.meshSet.Count >= core.parameters.TargetFragments)
            {
                return true;
            }

            //
            // assign meshes to workers
            //
            var meshPerThread = core.meshSet.Count/(THREAD_MAX - 1);
            var workerId = 0;
            var meshCounter = 0;

            foreach (var meshObject in core.meshSet)
            {
                workers[workerId].AddMesh(meshObject);
                meshCounter++;

                if (meshCounter >= meshPerThread && workerId < THREAD_MAX - 2)
                {
                    meshCounter = 0;
                    workerId++;
                }
            }

            core.meshSet.Clear();

            //
            // kick off workers
            //
            foreach (var worker in workers)
            {
                worker.Run();
            }

            localWatch.Stop();
//            Debug.Log("MeshesPerThread: " + meshPerThread + " workers: " + workers.Length + " cuts: " + cutsTotal + " time: " + localWatch.ElapsedMilliseconds);

            return true;
        }

        private int[] SplitMeshTargetFragments(int id)
        {
            var tar0 = core.targetFragments[id] / 2;
            var tar1 = tar0;

            if (core.targetFragments[id]%2 == 1)
            {
                tar1 ++;
            }

            splitIDs[0] = (id+1)*100;
            splitIDs[1] = (id+1)*200;

            core.targetFragments.Add(splitIDs[0], tar0);
            core.targetFragments.Add(splitIDs[1], tar1);

            return splitIDs;
        }
    }
}
#endif
