// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;

namespace Exploder
{
    /// <summary>
    /// options for deactivating the fragment
    /// </summary>
    public enum DeactivateOptions
    {
        /// <summary>
        /// fragments remain active until they are needed for next explosion
        /// </summary>
        Never,

        /// <summary>
        /// fragment will be deactivated if it is not visible by main camera
        /// </summary>
        OutsideOfCamera,

        /// <summary>
        /// fragment will be deactivated after timeout
        /// </summary>
        Timeout,
    }

    /// <summary>
    /// options for fadeout fragments
    /// </summary>
    public enum FadeoutOptions
    {
        /// <summary>
        /// fragments will be fully visible until deactivation
        /// </summary>
        None,

        /// <summary>
        /// fragments will fadeout on deactivateTimeout
        /// </summary>
        FadeoutAlpha,

        /// <summary>
        /// fragments will scale down to zero on deactivateTimeout
        /// </summary>
        ScaleDown,
    }

    [Serializable]
    public class FragmentDeactivation
    {
        /// <summary>
        /// options for deactivating the fragment after explosion
        /// </summary>
        public DeactivateOptions DeactivateOptions;

        /// <summary>
        /// deactivate timeout, valid only if DeactivateOptions == DeactivateTimeout
        /// </summary>
        public float DeactivateTimeout = 10.0f;

        /// <summary>
        /// options for fading out fragments after explosion
        /// </summary>
        public FadeoutOptions FadeoutOptions = FadeoutOptions.None;

        public FragmentDeactivation Clone()
        {
            return new FragmentDeactivation
            {
                DeactivateOptions = this.DeactivateOptions,
                DeactivateTimeout = this.DeactivateTimeout,
                FadeoutOptions = this.FadeoutOptions,
            };
        }
    }
}
