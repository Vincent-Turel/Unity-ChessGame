using Exploder;
using UnityEngine;

namespace Exploder
{
    /// <summary>
    /// example how to get active fragments after explosion
    /// </summary>
    public class Example : MonoBehaviour
    {
        public ExploderObject Exploder;

        /// <summary>
        /// call this to explode your object
        /// </summary>
        public void ExplodeObject(GameObject obj)
        {
            // pass callback to get results when explosion is finished
            Exploder.ExplodeObject(obj, OnExplosion);
        }

        /// <summary>
        /// exploder callback
        /// </summary>
        private void OnExplosion(float time, ExploderObject.ExplosionState state)
        {
            // explosion is finished
            if (state == ExploderObject.ExplosionState.ExplosionFinished)
            {
                //!
                //! HERE IS THE LIST OF ACTIVE FRAGMENTS
                //!

                //            var list = FragmentPool.Instance.GetActiveFragments();

                // NOTE:
                // if you run another explosion afterwards and you dont clear or deactivate fragmets in fragment pool
                // you might get some of the fragments from old explosion as well
                // to deactivate fragments you can call FragmentPool.Instance.DeactivateFragments()
            }
        }

        /// <summary>
        /// call this to crack your object
        /// </summary>
        private void CrackAndExplodeObject(GameObject obj)
        {
            Exploder.CrackObject(obj, OnCracked);
        }

        /// <summary>
        /// callback when the object is cracked
        /// </summary>
        private void OnCracked(float time, ExploderObject.ExplosionState state)
        {
            // now the object is cracked we can call ExplodeCracked() now or later ...
            Exploder.ExplodeCracked(OnExplosion);
        }
    }
}
