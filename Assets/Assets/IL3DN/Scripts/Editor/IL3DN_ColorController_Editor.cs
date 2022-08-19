namespace IL3DN
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    [CustomEditor(typeof(IL3DN_ColorController))]
    public class IL3DN_ColorController_Editor : Editor
    {

        static IL3DN_ColorController_Editor()
        {
            EditorApplication.hierarchyChanged += hierarchyWindowChanged;
        }

        private static void hierarchyWindowChanged()
        {
            effectsScript = FindObjectOfType<IL3DN_ColorManagerEffects>();
            texturesScript = FindObjectOfType<IL3DN_ColorManagerTextures>();
            if (texturesScript != null)
            {
                texturesScript.Refresh();
            }
            if(effectsScript!=null)
            {
                effectsScript.Refresh();
            }
        }

        private IL3DN_ColorController targetScript;
        private static IL3DN_ColorManagerEffects effectsScript;
        private static IL3DN_ColorManagerTextures texturesScript;
        private void OnEnable()
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp((IL3DN_ColorController)target);
            targetScript = (IL3DN_ColorController)target;
            effectsScript = targetScript.GetComponent<IL3DN_ColorManagerEffects>();
            texturesScript = targetScript.GetComponent<IL3DN_ColorManagerTextures>();
            if (texturesScript)
            {
                if (texturesScript.materials.Count > 0)
                {
                    targetScript.slot = texturesScript.materials[0].selectedProperty + 1;
                }
            }
            if (effectsScript)
            {
                if (effectsScript.materials.Count > 0)
                {
                    targetScript.slot = effectsScript.materials[0].selectedProperty + 1;
                }
            }
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(targetScript.slot<1)
            {
                targetScript.slot = 1;
            }
            if (GUILayout.Button("Set this slot active for all materials"))
            {
                if (effectsScript != null)
                {
                    effectsScript.SetMaterialColors(targetScript.slot);
                }
                if (texturesScript != null)
                {
                    texturesScript.SetMaterialColors(targetScript.slot);
                }
            }
        }
    }
}