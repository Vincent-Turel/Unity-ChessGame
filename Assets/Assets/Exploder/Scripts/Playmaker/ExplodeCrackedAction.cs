// uncomment next line to work with Playmaker
//#define PLAYMAKER
#if PLAYMAKER

// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using Exploder;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Effects)]
    [Tooltip("Explode cracked objects using Exploder")]
    public class ExplodeCracked : FsmStateAction
    {
        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            var exploder = Exploder.Utils.ExploderSingleton.ExploderInstance;

            if (exploder != null)
            {
                exploder.ExplodeCracked(OnExplosion);
            }
        }

        void OnExplosion(float timeMS, ExploderObject.ExplosionState state)
        {
            if (state == ExploderObject.ExplosionState.ExplosionFinished)
            {
                Finish();
            }
        }
    }
}

#endif
