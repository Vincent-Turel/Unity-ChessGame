// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace Exploder
{
    /// <summary>
    /// simple script for enabling slow motion physics
    /// </summary>
    public class ExploderSlowMotion : MonoBehaviour
    {
        public float slowMotionTime = 1.0f;
        private ExploderObject Exploder;
        private float slowMotionSpeed = 1.0f;
        private bool slowmo;

        void Start()
        {
            Exploder = Utils.ExploderSingleton.Instance;
        }

        /// <summary>
        /// slow motion mode ... just slow time
        /// this mode is using mesh colliders so use it wisely!
        /// </summary>
        /// <param name="status"></param>
        public void EnableSlowMotion(bool status)
        {
            slowmo = status;

            if (slowmo)
            {
                slowMotionSpeed = 0.05f;

                if (Exploder)
                {
                    Exploder.FragmentOptions.MeshColliders = true;
                }
            }
            else
            {
                slowMotionSpeed = 1.0f;
                if (Exploder)
                {
                    Exploder.FragmentOptions.MeshColliders = false;
                }
            }

            slowMotionTime = slowMotionSpeed;
        }

        public void Update()
        {
            slowMotionSpeed = slowMotionTime;
            Time.timeScale = slowMotionSpeed;
            Time.fixedDeltaTime = slowMotionSpeed*0.02f;

            if (Input.GetKeyDown(KeyCode.T))
            {
                slowmo = !slowmo;
                EnableSlowMotion(slowmo);
            }
        }
    }
}
