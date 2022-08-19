namespace IL3DN
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(IL3DN_ColorManagerTextures))]
    public class IL3DN_ColorManagerTextures_Editor : Editor
    {
        private bool loaded;
        private IL3DN_ColorManagerTextures targetScript;
        private Material mat;

        Texture2D IL3DN_TexturesLabel;

        private void OnEnable()
        {
            
           targetScript = (IL3DN_ColorManagerTextures)target;
            if (targetScript.materials.Count > 0)
            {
                loaded = true;
            }

            IL3DN_TexturesLabel = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IL3DN/EditorImages/IL3DN_Label_CM_Textures.png");

        }

        private void UpdateScripts()
        {
            throw new NotImplementedException();
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(IL3DN_TexturesLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //base.OnInspectorGUI();
            if (GUILayout.Button("Load Materials"))
            {
                Renderer[] allRenderers = FindObjectsOfType<Renderer>();
                foreach (Renderer rend in allRenderers)
                {
                    foreach (Material mat in rend.sharedMaterials)
                    {
                        if (!MaterialExists(mat))
                        {

                            if (mat != null && mat.shader != null && mat.hideFlags == HideFlags.None && mat.shader.name != null && mat.HasProperty("_Color") && mat.HasProperty("_MainTex"))
                            {
                                targetScript.materials.Add(new MaterialProperties(mat));
                            }
                        }
                    }
                }

                loaded = true;
            }
            if (loaded)
            {
                EditorGUILayout.Space();
                for (int i = 0; i < targetScript.materials.Count; i++)
                {
                    Color defaultColor = GUI.color;
                    Color blackColor = new Color(0.65f, 0.65f, 0.65f, 1);
                    GUI.color = blackColor;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.color = defaultColor;
                    targetScript.materials[i].meterial = (Material)EditorGUILayout.ObjectField(targetScript.materials[i].meterial, typeof(Material), true);
                    for (int j = 0; j < targetScript.materials[i].properties.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        Color color = EditorGUILayout.ColorField(targetScript.materials[i].properties[j].color);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(targetScript, "Color Changed");
                            targetScript.materials[i].properties[j].color = color;
                        }

                        EditorGUI.BeginChangeCheck();
                        Texture2D mainTex = (Texture2D)EditorGUILayout.ObjectField(targetScript.materials[i].properties[j].mainTex, typeof(Texture2D), true);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(targetScript, "Texture Changed");
                            targetScript.materials[i].properties[j].mainTex = mainTex;
                            targetScript.Refresh();
                        }
                        if (targetScript.materials[i].selectedProperty < 0)
                        {
                            targetScript.materials[i].selectedProperty = 0;
                        }

                        Color materialColor = targetScript.materials[i].properties[targetScript.materials[i].selectedProperty].color;
                        Undo.RecordObject(targetScript, "Material Color Changed");
                        targetScript.materials[i].meterial.color = materialColor;

                        string buttonName = "Active";
                        var oldColor = GUI.backgroundColor;
                        if (j == targetScript.materials[i].selectedProperty)
                        {
                            GUI.backgroundColor = Color.green;
                        }

                        if (GUILayout.Button(buttonName))
                        {
                            Undo.RecordObject(targetScript.materials[i].meterial, "Settings Applied");
                            targetScript.materials[i].meterial.color = targetScript.materials[i].properties[j].color;
                            targetScript.materials[i].meterial.mainTexture = targetScript.materials[i].properties[j].mainTex;
                            targetScript.materials[i].selectedProperty = j;
                        }
                        GUI.backgroundColor = oldColor;

                        if (GUILayout.Button("Remove"))
                        {
                            Undo.RecordObject(targetScript, "Object Removed");

                            if (targetScript.materials[i].selectedProperty >= j)
                            {
                                targetScript.materials[i].selectedProperty--;
                            }
                            targetScript.materials[i].properties.RemoveAt(j);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Properties Set"))
                    {
                        targetScript.materials[i].properties.Add(new ShaderProperties(targetScript.materials[i].meterial.color, targetScript.materials[i].meterial.mainTexture as Texture2D));
                    }
                    if (GUILayout.Button("Remove Material"))
                    {
                        Undo.RecordObject(targetScript, "Object Removed");
                        targetScript.materials.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    GUI.color = defaultColor;
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("Add new material");
            mat = (Material)EditorGUILayout.ObjectField(mat, typeof(Material), true);
            if (mat != null)
            {
                if (GUILayout.Button("Add New Material"))
                {
                    targetScript.materials.Add(new MaterialProperties(mat));
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private bool MaterialExists(Material mat)
        {
            for (int i = 0; i < targetScript.materials.Count; i++)
            {
                if (targetScript.materials[i].meterial == mat)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
