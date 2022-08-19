// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Diagnostics;

namespace Exploder
{
    public enum TaskType
    {
        None,

        Preprocess,
        ProcessCutter,
        IsolateMeshIslands,
        PostprocessExplode,
        PostprocessCrack,

        TaskMax,
    }

    abstract class ExploderTask
    {
        public abstract TaskType Type { get; }

        protected Core core;

        public Stopwatch Watch { get; private set; }

        protected ExploderTask(Core Core)
        {
            this.core = Core;
            Watch = Stopwatch.StartNew();
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void Init()
        {
            Watch.Reset();
            Watch.Start();
        }

        public abstract bool Run(float frameBudget);
    }
}
