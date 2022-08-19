// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Exploder
{
    class Preprocess : ExploderTask
    {
        struct MeshData
        {
            public Mesh sharedMesh;
            public Material sharedMaterial;
            public GameObject gameObject;
            public GameObject parentObject;
            public GameObject skinnedBakeOriginal;
            public Vector3 centroid;
        }

        public Preprocess(Core Core) : base(Core)
        {
            Core.targetFragments = new Dictionary<int, int>(4);
        }

        public override TaskType Type { get { return TaskType.Preprocess; } }

        public override void Init()
        {
            base.Init();
            core.targetFragments.Clear();
        }

        public override bool Run(float frameBudget)
        {
            // find meshes around exploder centroid
            var meshList = GetMeshList();

            // nothing do destroy
            if (meshList.Count == 0)
            {
                Watch.Stop();
                core.meshSet.Clear();
                return true;
            }

            core.meshSet.Clear();

            foreach (var meshObject in meshList)
            {
                if (core.targetFragments[meshObject.id] > 0)
                {
                    core.meshSet.Add(meshObject);
                }
            }

            core.splitMeshIslands = core.parameters.SplitMeshIslands;

            Watch.Stop();
            return true;
        }

        private List<MeshObject> GetMeshList()
        {
            List<GameObject> objects = null;

            if (core.parameters.Targets != null)
            {
				objects = new List<GameObject>(core.parameters.Targets);
            }
            else
            {
                if (core.parameters.DontUseTag)
                {
                    var objs = Object.FindObjectsOfType(typeof(Explodable));
                    objects = new List<GameObject>(objs.Length);

                    foreach (var o in objs)
                    {
                        var ex = (Explodable)o;

                        if (ex)
                        {
                            objects.Add(ex.gameObject);
                        }
                    }
                }
                else
                {
                    objects = new List<GameObject>(GameObject.FindGameObjectsWithTag(ExploderObject.Tag));
                }
            }

            if (core.parameters.ExplodeSelf)
            {
                objects.Add(core.parameters.ExploderGameObject);
            }

            var list = new List<MeshObject>(objects.Count);

            int counter = 0;

            var ctr = Vector3.zero;
            var ctrCounter = 0;

            foreach (var o in objects)
            {
                // in case of destroyed objects
                if (!o)
                {
                    continue;
                }

                // don't destroy the destroyer :)
                if (!core.parameters.ExplodeSelf && o == core.parameters.ExploderGameObject)
                {
                    continue;
                }

                // stop scanning for object is case of settings.ExplodeSelf
                if (o != core.parameters.ExploderGameObject && core.parameters.ExplodeSelf && core.parameters.DisableRadiusScan)
                {
                    continue;
                }

                if (core.parameters.Targets != null || IsInRadius(o))
                {
                    var meshData = GetMeshData(o);
                    var meshDataLen = meshData.Count;

                    for (var i = 0; i < meshDataLen; i++)
                    {
                        var centroid = meshData[i].centroid;

                        // overwrite settings.Position in case of settings.Target
                        if (core.parameters.Targets != null)
                        {
                            core.parameters.Position = centroid;
                            ctr += centroid;
                            ctrCounter ++;
                        }

                        var distance = (centroid - core.parameters.Position).magnitude;

                        //                    UnityEngine.Debug.Log("Distance: " + distance + " " + meshData[i].gameObject.name);

                        list.Add(new MeshObject
                        {
                            id = counter++,
                            mesh = new ExploderMesh(meshData[i].sharedMesh),
                            material = meshData[i].sharedMaterial,
                            transform = new ExploderTransform(meshData[i].gameObject.transform),

                            parent = meshData[i].gameObject.transform.parent,
                            position = meshData[i].gameObject.transform.position,
                            rotation = meshData[i].gameObject.transform.rotation,
                            localScale = meshData[i].gameObject.transform.localScale,
                            bakeObject = meshData[i].gameObject,

                            distanceRatio = GetDistanceRatio(distance, core.parameters.Radius),
                            original = meshData[i].parentObject,
                            skinnedOriginal = meshData[i].skinnedBakeOriginal,

                            option = o.GetComponent<ExploderOption>(),
                        });
                    }
                }
            }

            if (ctrCounter > 0)
            {
                ctr /= ctrCounter;
                core.parameters.Position = ctr;
            }

            if (list.Count == 0)
            {
                ExploderUtils.Log("No explodable objects found!");
                return list;
            }

            if (core.parameters.UniformFragmentDistribution || core.parameters.Targets != null)
            {
                var fragmentPerObject = core.parameters.TargetFragments/list.Count;

                int cnt = core.parameters.TargetFragments;
                foreach (var meshObject in list)
                {
                    core.targetFragments[meshObject.id] = fragmentPerObject;
                    cnt -= fragmentPerObject;
                }

                while (cnt > 0)
                {
                    cnt--;

                    var randMeshObject = list[UnityEngine.Random.Range(0, list.Count - 1)];
                    core.targetFragments[randMeshObject.id] += 1;
                }
            }
            else
            {
                var sum = 0.0f;
                var sumFragments = 0;

                foreach (var o in list)
                {
                    sum += o.distanceRatio;
                }

                foreach (var mesh in list)
                {
                    core.targetFragments[mesh.id] = (int)((mesh.distanceRatio / sum) * core.parameters.TargetFragments);
                    sumFragments += core.targetFragments[mesh.id];
                }

                if (sumFragments < core.parameters.TargetFragments)
                {
                    var diff = core.parameters.TargetFragments - sumFragments;

                    while (diff > 0)
                    {
                        foreach (var mesh in list)
                        {
                            core.targetFragments[mesh.id] += 1;
                            diff --;

                            if (diff == 0)
                                break;
                        }
                    }
                }
            }

            return list;
        }

        private List<MeshData> GetMeshData(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            var meshFilters = obj.GetComponentsInChildren<MeshFilter>();

            ExploderUtils.Warning(renderers.Length == meshFilters.Length, "Renderers and meshes don't match!");

            if (renderers.Length != meshFilters.Length)
            {
                return new List<MeshData>();
            }

            var outList = new List<MeshData>(renderers.Length);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (meshFilters[i].sharedMesh == null)
                {
                    ExploderUtils.Log("Missing shared mesh in " + meshFilters[i].name);
                    continue;
                }

                if (!meshFilters[i].sharedMesh || !meshFilters[i].sharedMesh.isReadable)
                {
                    UnityEngine.Debug.LogWarning("Mesh is not readable: " + meshFilters[i].name);
                    continue;
                }

                if (/*IsExplodable(meshFilters[i].gameObject)*/true)
                {
                    outList.Add(new MeshData
                    {
                        sharedMesh = meshFilters[i].sharedMesh,
                        sharedMaterial = renderers[i].sharedMaterial,
                        gameObject = renderers[i].gameObject,
                        centroid = renderers[i].bounds.center,
                        parentObject = obj,
                    });
                }
            }

            // find skinned mesh
            var renderersSkinned = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < renderersSkinned.Length; i++)
            {
                var bakeMesh = new Mesh();
                renderersSkinned[i].BakeMesh(bakeMesh);
                var bakeObj = core.bakeSkinManager.CreateBakeObject(obj.name);
                var meshFilter = bakeObj.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = bakeMesh;
                var meshRenderer = bakeObj.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = renderersSkinned[i].material;
                bakeObj.transform.position = renderersSkinned[i].gameObject.transform.position;
                bakeObj.transform.rotation = renderersSkinned[i].gameObject.transform.rotation;
                ExploderUtils.SetVisible(bakeObj, false);

                outList.Add(new MeshData
                {
                    sharedMesh = bakeMesh,
                    sharedMaterial = meshRenderer.sharedMaterial,
                    gameObject = bakeObj,
                    centroid = meshRenderer.bounds.center,
                    parentObject = bakeObj,
                    skinnedBakeOriginal = obj,
                });
            }

            return outList;
        }

        private float GetDistanceRatio(float distance, float radius)
        {
            return 1.0f - Mathf.Clamp01(distance / radius);
        }

        private bool IsInRadius(GameObject o)
        {
            var centroid = ExploderUtils.GetCentroid(o);

            if (core.parameters.UseCubeRadius)
            {
                var localP = core.parameters.ExploderGameObject.transform.InverseTransformPoint(centroid);
                var localC = core.parameters.ExploderGameObject.transform.InverseTransformPoint(core.parameters.Position);

                return (Mathf.Abs(localP.x - localC.x) < core.parameters.CubeRadius.x &&
                        Mathf.Abs(localP.y - localC.y) < core.parameters.CubeRadius.y &&
                        Mathf.Abs(localP.z - localC.z) < core.parameters.CubeRadius.z);
            }

            return core.parameters.Radius * core.parameters.Radius > (centroid - core.parameters.Position).sqrMagnitude;
        }
    }
}
