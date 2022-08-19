// uncomment next line to work with Playmaker
//#define PLAYMAKER
#if PLAYMAKER

// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using Exploder;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Effects)]
    [Tooltip("Explode objects in the radius using Exploder.")]
    public class ExplodeRadius : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject with an Exploder component.")]
        public FsmOwnerDefault gameObject;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != null)
            {
                var exploder = Exploder.Utils.ExploderSingleton.ExploderInstance;

                if (exploder != null)
                {
                    exploder.transform.position = go.transform.position;
                    exploder.ExplodeRadius(OnExplosion);
                }
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
