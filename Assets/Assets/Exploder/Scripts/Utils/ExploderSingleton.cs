// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace Exploder.Utils
{
    /// <summary>
    /// utility class for easy accessing single exploder object in the scene
    /// assign this class to exploder game object
    /// </summary>
    public class ExploderSingleton : MonoBehaviour
    {
        /// <summary>
        /// instance of the exploder object
        /// </summary>
        [System.Obsolete("ExploderInstance is obsolete, please use Instance instead.")]
        public static ExploderObject ExploderInstance;

        public static ExploderObject Instance;

        void Awake()
        {
            Instance = gameObject.GetComponent<ExploderObject>();

#pragma warning disable 618
            ExploderInstance = Instance;
#pragma warning restore 618
        }
    }
}
