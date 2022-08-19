// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace Exploder
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (isQuitting)
                {
                    return null;
                }

                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        UnityEngine.Debug.LogWarning("More than 1 singleton opened!");
                        return instance;
                    }

                    if (instance == null)
                    {
                        var singleton = new GameObject("ExploderCore");
                        instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }

        private static bool isQuitting = false;

        public virtual void OnDestroy()
        {
            isQuitting = true;
        }
    }
}
