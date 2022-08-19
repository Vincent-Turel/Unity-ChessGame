// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace Exploder
{
    abstract class Postprocess : ExploderTask
    {
        protected Postprocess(Core Core) : base(Core)
        {
        }

        public override void Init()
        {
            base.Init();

            core.poolIdx = 0;

            // run sfx
            if (core.audioSource)
            {
                core.audioSource.Play();
            }
        }
    }
}
