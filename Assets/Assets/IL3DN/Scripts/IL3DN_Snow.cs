namespace IL3DN
{
    using UnityEngine;
    [ExecuteInEditMode]

    public class IL3DN_Snow : MonoBehaviour
    {
        public bool Snow = false;
        [Range(0f, 20f)]
        public float SnowPines = 1f;
        [Range(0f, 20f)]
        public float SnowLeaves = 1f;
        [Range(0f, 20f)]
        public float SnowBranches = 1f;
        [Range(0f, 20f)]
        public float SnowRocks = 1f;
        [Range(0f, 20f)]
        public float SnowGrass = 1f;
        [Range(0f, 1f)]
        public float SnowTerrain = 1f;
        [Range(1f, 2.1f)]
        public float CutoffLeaves = 1f;


        void Update()
        {
            if (Snow)
            {
                Shader.EnableKeyword("_SNOW_ON");
            }
            else
            {
                Shader.DisableKeyword("_SNOW_ON");
            }

            Shader.SetGlobalFloat("SnowPinesFloat", SnowPines);
            Shader.SetGlobalFloat("SnowLeavesFloat", SnowLeaves);
            Shader.SetGlobalFloat("SnowBranchesFloat", SnowBranches);
            Shader.SetGlobalFloat("SnowRocksFloat", SnowRocks);
            Shader.SetGlobalFloat("SnowGrassFloat", SnowGrass);
            Shader.SetGlobalFloat("SnowTerrainFloat", SnowTerrain);
            Shader.SetGlobalFloat("AlphaCutoffFloat", CutoffLeaves);

        }
    }
}