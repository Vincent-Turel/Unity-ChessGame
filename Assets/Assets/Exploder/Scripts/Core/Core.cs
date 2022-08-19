// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

//#define DBG
#if UNITY_WEBGL
#define DISABLE_MULTITHREADING
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Exploder
{
    class Core : Singleton<Core>
    {
        public void Initialize(ExploderObject exploder)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            parameters = new ExploderParams(exploder);

            // init pool
            FragmentPool.Instance.Reset(parameters);
            frameWatch = new Stopwatch();
            explosionWatch = new Stopwatch();

            crackManager = new CrackManager(this);

            bakeSkinManager = new BakeSkinManager(this);

            //
            // init queue
            //
            queue = new ExploderQueue(this);

            //
            // init tasks
            //
            tasks = new ExploderTask[(int) TaskType.TaskMax];

            tasks[(int)TaskType.Preprocess] = new Preprocess(this);

#if DISABLE_MULTITHREADING
            tasks[(int) TaskType.ProcessCutter] = new CutterST(this);
#else
            if (parameters.ThreadOptions == ExploderObject.ThreadOptions.Disabled)
            {
                tasks[(int)TaskType.ProcessCutter] = new CutterST(this);
            }
            else
            {
                tasks[(int)TaskType.ProcessCutter] = new CutterMT(this);
            }
#endif

            tasks[(int)TaskType.IsolateMeshIslands] = new IsolateMeshIslands(this);
            tasks[(int)TaskType.PostprocessExplode] = new PostprocessExplode(this);
            tasks[(int)TaskType.PostprocessCrack] = new PostprocessCrack(this);

            PreAllocateBuffers();

            audioSource = gameObject.AddComponent<AudioSource>();
        }

		public void Enqueue(ExploderObject exploderObject, ExploderObject.OnExplosion callback, bool crack, params GameObject[] obj)
        {
            queue.Enqueue(exploderObject, callback, crack, obj);
        }

        public void ExplodeCracked(GameObject obj, ExploderObject.OnExplosion callback)
        {
            if (obj != null)
            {
                var ms = crackManager.Explode(obj);

                if (callback != null)
                {
                    callback(ms, ExploderObject.ExplosionState.ExplosionFinished);
                }
            }
            else
            {
                var ms = crackManager.ExplodeAll();

                if (callback != null)
                {
                    callback(ms, ExploderObject.ExplosionState.ExplosionFinished);
                }
            }
        }

        public void ExplodePartial(GameObject obj, Vector3 shotDir, Vector3 hitPosition, float bulletSize, ExploderObject.OnExplosion callback)
        {
            if (obj != null)
            {
                var ms = crackManager.ExplodePartial(obj, shotDir, hitPosition, bulletSize);

                if (callback != null)
                {
                    callback(ms, ExploderObject.ExplosionState.ExplosionFinished);
                }
            }
            else
            {
                var ms = crackManager.ExplodeAll();

                if (callback != null)
                {
                    callback(ms, ExploderObject.ExplosionState.ExplosionFinished);
                }
            }
        }

        public bool IsCracked(GameObject gm)
        {
            return crackManager.IsCracked(gm);
        }

        public void StartExplosionFromQueue(ExploderParams p)
        {
            ExploderUtils.Assert(currTaskType == TaskType.None, "Wrong task: " + currTaskType);
            this.parameters = p;
            processingFrames = 1;
            explosionWatch.Reset();
            explosionWatch.Start();

            var exAudioSource = p.ExploderGameObject.GetComponent<AudioSource>();

            if (exAudioSource)
            {
                ExploderUtils.CopyAudioSource(exAudioSource, audioSource);
            }

            //
            // do preprocess right away
            //
            currTaskType = TaskType.Preprocess;
            InitTask(currTaskType);
            RunTask(currTaskType);

            if (parameters.ThreadOptions != ExploderObject.ThreadOptions.Disabled)
            {
                currTaskType = NextTask(currTaskType);

                if (currTaskType != TaskType.None)
                {
                    InitTask(currTaskType);
                    RunTask(currTaskType, parameters.FrameBudget);
                }
                else
                {
                    explosionWatch.Stop();
                    queue.OnExplosionFinished(parameters.id, explosionWatch.ElapsedMilliseconds);
                }
            }
        }

        public void Update()
        {
            frameWatch.Reset();
            frameWatch.Start();

            if (currTaskType != TaskType.None)
            {
                while (frameWatch.ElapsedMilliseconds < parameters.FrameBudget)
                {
                    if (RunTask(currTaskType, parameters.FrameBudget))
                    {
                        currTaskType = NextTask(currTaskType);

                        if (currTaskType == TaskType.None)
                        {
                            explosionWatch.Stop();
                            bakeSkinManager.Clear();
                            queue.OnExplosionFinished(parameters.id, explosionWatch.ElapsedMilliseconds);
                            return;
                        }

                        InitTask(currTaskType);
                    }
                }

                processingFrames++;
            }
        }

#if DBG
        public void OnGUI()
        {
            GUI.Label(new Rect(10, 50, 300, 30), "Explosion time: " + explosionWatch.ElapsedMilliseconds + " [ms]");
            GUI.Label(new Rect(10, 80, 500, 30), "Processing frames: " + processingFrames);

            var y = 100;
            GUI.Label(new Rect(10, y += 20, 500, 30), string.Format("{0}: {1}[ms]: ", TaskType.Preprocess, tasks[(int)TaskType.Preprocess].Watch.ElapsedMilliseconds));
            GUI.Label(new Rect(10, y += 20, 500, 30), string.Format("{0}: {1}[ms]: ", TaskType.ProcessCutter, tasks[(int)TaskType.ProcessCutter].Watch.ElapsedMilliseconds));
            GUI.Label(new Rect(10, y += 20, 500, 30), string.Format("{0}: {1}[ms]: ", TaskType.PostprocessExplode, tasks[(int)TaskType.PostprocessExplode].Watch.ElapsedMilliseconds));
            GUI.Label(new Rect(10, y += 20, 500, 30), string.Format("{0}: {1}[ms]: ", TaskType.IsolateMeshIslands, tasks[(int)TaskType.IsolateMeshIslands].Watch.ElapsedMilliseconds));
        }
#endif

        public override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var task in tasks)
            {
                if (task != null)
                {
                    task.OnDestroy();
                }
            }
        }

        private void PreAllocateBuffers()
        {
            // kick memory allocator for better performance at startup
            meshSet = new HashSet<MeshObject>();

            for (int i = 0; i < 64; i++)
            {
                meshSet.Add(new MeshObject());
            }

            InitTask(TaskType.Preprocess);
            RunTask(TaskType.Preprocess);

            if (meshSet.Count > 0)
            {
                InitTask(TaskType.ProcessCutter);
                RunTask(TaskType.ProcessCutter);
            }
        }

        [NonSerialized] public ExploderParams parameters;
        [NonSerialized] public ExploderQueue queue;
        [NonSerialized] public Stopwatch explosionWatch;
        [NonSerialized] public Stopwatch frameWatch;
        [NonSerialized] public HashSet<MeshObject> meshSet;
        [NonSerialized] public Dictionary<int, int> targetFragments;
        [NonSerialized] public List<MeshObject> postList;
        [NonSerialized] public List<Fragment> pool;
        [NonSerialized] public AudioSource audioSource;
        [NonSerialized] public CrackManager crackManager;
        [NonSerialized] public int processingFrames;
        [NonSerialized] public int poolIdx;
        [NonSerialized] public bool splitMeshIslands;
        [NonSerialized] public BakeSkinManager bakeSkinManager;

        private ExploderTask[] tasks;
        private TaskType currTaskType;
        private bool initialized = false;

        private bool RunTask(TaskType taskType, float budget = 0.0f)
        {
            return tasks[(int) taskType].Run(budget);
        }

        private void InitTask(TaskType taskType)
        {
            tasks[(int)taskType].Init();
        }

        private TaskType NextTask(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.Preprocess:
                {
                    if (meshSet.Count == 0)
                    {
                        return TaskType.None;
                    }

                    return TaskType.ProcessCutter;
                }

                case TaskType.ProcessCutter:
                {
                    if (splitMeshIslands)
                    {
                        return TaskType.IsolateMeshIslands;
                    }
                    if (parameters.Crack)
                    {
                        return TaskType.PostprocessCrack;
                    }

                    return TaskType.PostprocessExplode;
                }

                case TaskType.IsolateMeshIslands:
                {
                    if (parameters.Crack)
                    {
                        return TaskType.PostprocessCrack;
                    }

                    return TaskType.PostprocessExplode;
                }

                case TaskType.PostprocessExplode:
                    return TaskType.None;

                case TaskType.PostprocessCrack:
                    return TaskType.None;

                default:
                    ExploderUtils.Assert(false, "Invalid task type!");
                    break;
            }

            ExploderUtils.Assert(false, "Invalid task type!");
            return TaskType.None;
        }
    }
}
