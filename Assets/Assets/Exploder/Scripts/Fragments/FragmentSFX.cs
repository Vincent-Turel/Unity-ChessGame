// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using UnityEngine;

namespace Exploder
{
    [Serializable]
    public class FragmentSFX
    {
        /// <summary>
        /// particle system emitter
        /// </summary>
        public GameObject FragmentEmitter;

        /// <summary>
        /// timeout to play next hit sound to prevent frequent sounds
        /// </summary>
        public int ChanceToPlay = 100;

        /// <summary>
        /// play hit sound only one time per fragment
        /// </summary>
        public bool PlayOnlyOnce = false;

        /// <summary>
        /// play hit sound only one time at the
        /// </summary>
        public bool MixMultipleSounds = false;

        /// <summary>
        /// maximum emitters for all fragments
        /// </summary>
        public int EmitersMax;

        /// <summary>
        /// deactivate particle on timeout
        /// </summary>
        public float ParticleTimeout;

        public FragmentSFX Clone()
        {
            return new FragmentSFX
            {
                FragmentEmitter = this.FragmentEmitter,
                ChanceToPlay = this.ChanceToPlay,
                PlayOnlyOnce = this.PlayOnlyOnce,
                MixMultipleSounds = this.MixMultipleSounds,
                EmitersMax = this.EmitersMax,
                ParticleTimeout = this.ParticleTimeout,
            };
        }
    }
}
