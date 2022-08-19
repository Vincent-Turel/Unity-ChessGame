namespace IL3DN
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(IL3DN_Snow))]
    public class IL3DN_Snow_Editor : Editor
    {
        Texture2D IL3DN_SnowLabel;

        SerializedProperty Snow;
        SerializedProperty SnowPines;
        SerializedProperty SnowLeaves;
        SerializedProperty SnowBranches;
        SerializedProperty SnowRocks;
        SerializedProperty SnowGrass;
        SerializedProperty SnowTerrain;
        SerializedProperty CutoffLeaves;

        void OnEnable()
        {
            IL3DN_SnowLabel = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IL3DN/EditorImages/IL3DN_Label_Snow.png");

            Snow = serializedObject.FindProperty("Snow");
            SnowPines = serializedObject.FindProperty("SnowPines");
            SnowLeaves = serializedObject.FindProperty("SnowLeaves");
            SnowBranches = serializedObject.FindProperty("SnowBranches");
            SnowRocks = serializedObject.FindProperty("SnowRocks");
            SnowGrass = serializedObject.FindProperty("SnowGrass");
            SnowTerrain = serializedObject.FindProperty("SnowTerrain");
            CutoffLeaves = serializedObject.FindProperty("CutoffLeaves");

        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(IL3DN_SnowLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(Snow, new GUIContent("Snow"));
            EditorGUILayout.PropertyField(SnowTerrain, new GUIContent("Terrain"));
            EditorGUILayout.PropertyField(SnowPines, new GUIContent("Pines"));
            EditorGUILayout.PropertyField(SnowLeaves, new GUIContent("Leaves"));
            EditorGUILayout.PropertyField(SnowBranches, new GUIContent("Branches"));
            EditorGUILayout.PropertyField(SnowRocks, new GUIContent("Rocks"));
            EditorGUILayout.PropertyField(SnowGrass, new GUIContent("Grass"));
            EditorGUILayout.PropertyField(CutoffLeaves, new GUIContent("Cutoff Leaves"));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
