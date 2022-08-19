namespace IL3DN
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(IL3DN_ColorManagerEffects))]
    public class IL3DN_ColorManagerEffects_Editor : Editor
    {
        private bool loaded;
        private IL3DN_ColorManagerEffects targetScript;
        Material mat;
        Texture2D IL3DN_EffectsLabel;

        private void OnEnable()
        {
            targetScript = (IL3DN_ColorManagerEffects)target;
            if (targetScript.materials.Count > 0)
            {
                loaded = true;
            }

            IL3DN_EffectsLabel = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IL3DN/EditorImages/IL3DN_Label_CM_Effects.png");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(IL3DN_EffectsLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Load Materials"))
            {
                Renderer[] allRenderers = FindObjectsOfType<Renderer>();
                foreach (Renderer rend in allRenderers)
                {
                    foreach (Material mat in rend.sharedMaterials)
                    {
                        if (!MaterialExists(mat))
                        {
                            if (mat != null && mat.shader != null && mat.hideFlags == HideFlags.None && mat.shader.name != null)
                            {
                                if (mat.shader.name == "IL3DN/WaterWip" ||
                                    mat.shader.name == "IL3DN/Fog" ||
                                    mat.shader.name == "IL3DN/Sky" ||
                                    mat.shader.name == "IL3DN/Terrain First-Pass"
                                    )
                                {
                                    Debug.Log(mat.shader);
                                    targetScript.materials.Add(new MultipleColorProperties(mat));
                                }
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

                        for (int k = 0; k < targetScript.materials[i].properties[j].colors.Count; k++)
                        {
                            EditorGUI.BeginChangeCheck();
                            Color color = EditorGUILayout.ColorField(targetScript.materials[i].properties[j].colors[k].color);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(targetScript, "Color Changed");
                                targetScript.materials[i].properties[j].colors[k].color = color;
                            }
                            Color materialColor = targetScript.materials[i].properties[targetScript.materials[i].selectedProperty].colors[k].color;
                            string propertyName = targetScript.materials[i].properties[targetScript.materials[i].selectedProperty].colors[k].name;
                            Undo.RecordObject(targetScript, "Material Color Changed");
                            targetScript.materials[i].meterial.SetColor(propertyName, materialColor);
                        }

                        string buttonName = "Active";
                        var oldColor = GUI.backgroundColor;
                        if (j == targetScript.materials[i].selectedProperty)
                        {
                            GUI.backgroundColor = Color.green;
                        }

                        if (GUILayout.Button(buttonName))
                        {
                            Undo.RecordObject(targetScript.materials[i].meterial, "Settings Applied");
                            for (int k = 0; k < targetScript.materials[i].properties[j].colors.Count; k++)
                            {
                                Color materialColor = targetScript.materials[i].properties[j].colors[k].color;
                                string propertyName = targetScript.materials[i].properties[j].colors[k].name;

                                targetScript.materials[i].meterial.SetColor(propertyName, materialColor);
                            }
                            targetScript.materials[i].selectedProperty = j;
                        }
                        GUI.backgroundColor = oldColor;

                        if (GUILayout.Button("Remove"))
                        {
                            Undo.RecordObject(targetScript, "Object Removed");
                            if (targetScript.materials[i].selectedProperty >= j)
                            {
                                targetScript.materials[i].selectedProperty--;
                                if (targetScript.materials[i].selectedProperty < 0)
                                {
                                    targetScript.materials[i].selectedProperty = 0;
                                }
                            }
                            Debug.Log(targetScript.materials[i].selectedProperty);

                            targetScript.materials[i].properties.RemoveAt(j);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Properties Set"))
                    {
                        targetScript.materials[i].properties.Add(new MaterialColors(targetScript.materials[i].meterial));
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
                    targetScript.materials.Add(new MultipleColorProperties(mat));
                    mat = null;
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