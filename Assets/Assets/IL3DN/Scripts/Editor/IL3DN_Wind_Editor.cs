namespace IL3DN
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(IL3DN_Wind))]
    public class IL3DN_Wind_Editor : Editor
    {
        SerializedProperty NoiseTexture;
        SerializedProperty Wind;
        SerializedProperty WindStrenght;
        SerializedProperty WindSpeed;
        SerializedProperty WindTurbulence;
        SerializedProperty Wiggle;
        SerializedProperty LeavesWiggle;
        SerializedProperty GrassWiggle;
        Texture2D IL3DN_WindLabel;
        Texture2D IL3DN_LeavesLabel;


        void OnEnable()
        {
            NoiseTexture = serializedObject.FindProperty("NoiseTexture");
            Wind = serializedObject.FindProperty("Wind");
            WindStrenght = serializedObject.FindProperty("WindStrenght");
            WindSpeed = serializedObject.FindProperty("WindSpeed");
            WindTurbulence = serializedObject.FindProperty("WindTurbulence");

            Wiggle = serializedObject.FindProperty("Wiggle");
            LeavesWiggle = serializedObject.FindProperty("LeavesWiggle");
            GrassWiggle = serializedObject.FindProperty("GrassWiggle");

            IL3DN_WindLabel = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IL3DN/EditorImages/IL3DN_Label_Wind_VertexAnimations.png");
            IL3DN_LeavesLabel = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IL3DN/EditorImages/IL3DN_Label_Wind_UVAnimations.png");

        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(IL3DN_WindLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(Wind, new GUIContent("Wind"));
            EditorGUILayout.PropertyField(WindStrenght, new GUIContent("Strength"));
            EditorGUILayout.PropertyField(WindSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(WindTurbulence, new GUIContent("Turbulence"));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(IL3DN_LeavesLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(Wiggle, new GUIContent("Wiggle"));
            EditorGUILayout.PropertyField(LeavesWiggle, new GUIContent("Leaves"));
            EditorGUILayout.PropertyField(GrassWiggle, new GUIContent("Grass"));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(NoiseTexture, new GUIContent("Noise Texture"));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
