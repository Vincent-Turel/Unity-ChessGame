// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using Exploder;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (Fragment))]
public class EditorFragmentObject : UnityEditor.Editor
{
    SerializedObject exploderSerObject;
    ExploderObject exploder;
    Fragment fragment;

    private void OnEnable()
    {
        fragment = this.target as Fragment;

        if (fragment)
        {
            exploder = GameObject.FindObjectOfType<ExploderObject>();

            if (exploder)
            {
                exploderSerObject = new SerializedObject(exploder);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (exploderSerObject != null)
        {
            exploderSerObject.Update();
            var change = false;

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Fragment Audio", 20);
            EditorGUILayout.Space();

            var hasAudioSource = fragment.gameObject.GetComponent<AudioSource>();

            if (!hasAudioSource && EditorExploderUtils.Button("Add Hit Audio Source"))
            {
                fragment.audioSource = fragment.gameObject.AddComponent<AudioSource>();
                fragment.audioSource.playOnAwake = false;
            }

            if (change)
            {
                exploderSerObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(exploder);
            }

            EditorGUILayout.Separator();
        }
    }
}
