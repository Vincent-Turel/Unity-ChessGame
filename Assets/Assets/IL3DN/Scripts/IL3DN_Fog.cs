namespace IL3DN
{
    using UnityEngine;

    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class IL3DN_Fog : MonoBehaviour
    {
        public Material material;
        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, material);
        }
    }
}