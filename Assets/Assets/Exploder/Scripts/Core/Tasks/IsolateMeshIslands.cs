// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;

namespace Exploder
{
    class IsolateMeshIslands : ExploderTask
    {
        private readonly List<MeshObject> islands;

        public IsolateMeshIslands(Core Core) : base(Core)
        {
            islands = new List<MeshObject>();
        }

        public override TaskType Type { get { return TaskType.IsolateMeshIslands; } }

        public override void Init()
        {
            base.Init();
            islands.Clear();

            core.poolIdx = 0;
            core.postList = new List<MeshObject>(core.meshSet);
        }

        public override bool Run(float frameBudget)
        {
            var count = core.postList.Count;

            while (core.poolIdx < count)
            {
                var mesh = core.postList[core.poolIdx];
                core.poolIdx++;

                var islandsFound = false;

                if (core.parameters.SplitMeshIslands || (mesh.option && mesh.option.SplitMeshIslands))
                {
                    var meshIslands = MeshUtils.IsolateMeshIslands(mesh.mesh);

                    if (meshIslands != null)
                    {
                        islandsFound = true;

                        foreach (var meshIsland in meshIslands)
                        {
                            islands.Add(new MeshObject
                            {
                                mesh = meshIsland,

                                material = mesh.material,
                                transform = mesh.transform,
                                original = mesh.original,
                                skinnedOriginal = mesh.skinnedOriginal,

                                parent = mesh.transform.parent,
                                position = mesh.transform.position,
                                rotation = mesh.transform.rotation,
                                localScale = mesh.transform.localScale,

                                option = mesh.option,
                            });
                        }
                    }
                }

                if (!islandsFound)
                {
                    islands.Add(mesh);
                }

                if (Watch.ElapsedMilliseconds > frameBudget)
                {
                    return false;
                }
            }

#if DBG
        ExploderUtils.Log("Replacing fragments: " + postList.Count + " by islands: " + islands.Count);
#endif

            // replace postList by island list
            core.postList = islands;

            Watch.Stop();

            return true;
        }
    }
}
