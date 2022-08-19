using System.Collections.Generic;
using UnityEngine;

namespace Exploder
{
    class BakeSkinManager
    {
        private readonly GameObject parent;
        private readonly List<GameObject> bakedObjects = new List<GameObject>(); 

        public BakeSkinManager(Core core)
        {
            parent = new GameObject("BakeSkinParent");
            parent.gameObject.transform.parent = core.transform;
            parent.transform.position = Vector3.zero;
        }

        public GameObject CreateBakeObject(string name)
        {
            var bakeObject = new GameObject(name);
            bakeObject.transform.parent = parent.transform;
            bakedObjects.Add(bakeObject);

            return bakeObject;
        }

        public void Clear()
        {
            for (int i = 0; i < bakedObjects.Count; i++)
            {
                if (bakedObjects[i])
                {
                    Object.Destroy(bakedObjects[i]);
                }
            }

            bakedObjects.Clear();
        }
    }
}
