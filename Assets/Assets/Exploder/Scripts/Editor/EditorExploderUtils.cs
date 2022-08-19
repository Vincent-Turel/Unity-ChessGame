// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.IO;
using UnityEditor;
using UnityEngine;

static class EditorExploderUtils
{
    /// <summary>
    /// create slider with edit box for float values
    /// </summary>
    /// <param name="label">name of the slider</param>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <param name="value">current value</param>
    /// <returns>true if value has been changed</returns>
    public static bool SliderEdit(string label, string toolTip, float min, float max, SerializedProperty value)
    {
        EditorGUILayout.BeginHorizontal();
        var oldValue = value.floatValue;
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value.floatValue = GUILayout.HorizontalSlider(value.floatValue, min, max);
        GUILayout.Space(10);
        value.floatValue = EditorGUILayout.FloatField(value.floatValue, GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();

        value.floatValue = Mathf.Clamp(value.floatValue, min, max);

        return oldValue != value.floatValue;
    }

    /// <summary>
    /// create slider with edit box for Vector2 values
    /// </summary>
    /// <param name="label0">name of the first slider</param>
    /// <param name="label1">name of the second slider</param>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <param name="value">current value</param>
    /// <returns>true if value has been changed</returns>
    public static bool SliderEdit(string label0, string label1, float min, float max, ref Vector2 value)
    {
        var oldValue = value;
        EditorGUILayout.BeginHorizontal();
        value.x = EditorGUILayout.FloatField(label0, value.x);
        value.x = GUILayout.HorizontalSlider(value.x, min, max);
        EditorGUILayout.EndHorizontal();
        value.x = Mathf.Clamp(value.x, min, max);

        EditorGUILayout.BeginHorizontal();
        value.y = EditorGUILayout.FloatField(label1, value.y);
        value.y = GUILayout.HorizontalSlider(value.y, min, max);
        EditorGUILayout.EndHorizontal();
        value.y = Mathf.Clamp(value.y, min, max);

        return oldValue != value;
    }

    public static bool Button(string label)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var val = GUILayout.Button(label, GUILayout.MaxWidth(200), GUILayout.MaxHeight(25));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        return val;
    }

    public static bool ButtonWithSpace(string label)
    {
        GUILayout.BeginHorizontal();
        var val = GUILayout.Button(label);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();
        return val;
    }

    /// <summary>
    /// create slider with edit box for int values
    /// </summary>
    /// <param name="label">name of the slider</param>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <param name="value">current value</param>
    /// <returns>true if value has been changed</returns>
    public static bool SliderEdit(string label, string toolTip, int min, int max, SerializedProperty value)
    {
        EditorGUILayout.BeginHorizontal();
        var oldValue = value.intValue;
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value.intValue = (int)GUILayout.HorizontalSlider(value.intValue, min, max);
        GUILayout.Space(10);
        value.intValue = EditorGUILayout.IntField(value.intValue, GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();

        value.intValue = Mathf.Clamp(value.intValue, min, max);

        return oldValue != value.intValue;
    }

    /// <summary>
    /// create slider with edit box for int values
    /// </summary>
    /// <param name="label">name of the slider</param>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <param name="value">current value</param>
    /// <returns>true if value has been changed</returns>
    public static bool IntEdit(string label, string toolTip, int min, int max, SerializedProperty value)
    {
        EditorGUILayout.BeginHorizontal();
        var oldValue = value.intValue;
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value.intValue = EditorGUILayout.IntField(value.intValue);
        EditorGUILayout.EndHorizontal();
        value.intValue = Mathf.Clamp(value.intValue, min, max);
        return oldValue != value.intValue;
    }

    /// <summary>
    /// create toggle control
    /// </summary>
    /// <param name="label">name of the toggle</param>
    /// <param name="value">bool value of toggle</param>
    /// <returns>true if value has been changed</returns>
    public static bool Toggle(string label, string toolTip, SerializedProperty value)
    {
        var oldValue = value.boolValue;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value.boolValue = EditorGUILayout.Toggle(value.boolValue);
        EditorGUILayout.EndHorizontal();
        return oldValue != value.boolValue;
    }

    public static bool ToggleBool(string label, string toolTip, ref bool value)
    {
        var oldValue = value;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value = EditorGUILayout.Toggle(value);
        EditorGUILayout.EndHorizontal();
        return oldValue != value;
    }

    public static bool Toggle2(string label, string toolTip, string label0, string label1, SerializedProperty value0, SerializedProperty value1)
    {
        var oldValue0 = value0.boolValue;
        var oldValue1 = value1.boolValue;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value0.boolValue = GUILayout.Toggle(value0.boolValue, label0);
        value1.boolValue = GUILayout.Toggle(value1.boolValue, label1);

        EditorGUILayout.EndHorizontal();
        return oldValue0 != value0.boolValue || oldValue1 != value1.boolValue;
    }

    public static bool Toggle3(string label, string toolTip, string label0, string label1, string label2, SerializedProperty value0, SerializedProperty value1, SerializedProperty value2)
    {
        var oldValue0 = value0.boolValue;
        var oldValue1 = value1.boolValue;
        var oldValue2 = value2.boolValue;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        value0.boolValue = GUILayout.Toggle(value0.boolValue, label0);
        value1.boolValue = GUILayout.Toggle(value1.boolValue, label1);
        value2.boolValue = GUILayout.Toggle(value2.boolValue, label2);

        EditorGUILayout.EndHorizontal();
        return oldValue0 != value0.boolValue || oldValue1 != value1.boolValue || oldValue2 != value2.boolValue;
    }

    /// <summary>
    /// create game object selection box
    /// </summary>
    /// <param name="label">name of the box</param>
    /// <param name="obj">game object to select</param>
    /// <returns>true if value has been changed</returns>
    public static bool GameObjectSelection(string label, SerializedProperty obj)
    {
        var oldValue = obj.objectReferenceValue;
        EditorGUILayout.BeginHorizontal();
        obj.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(label, obj.objectReferenceValue, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        return oldValue != obj.objectReferenceValue;
    }

    public static bool ObjectSelection<T>(string label, string toolTip, SerializedProperty obj) where T : UnityEngine.Object
    {
        var oldValue = obj.objectReferenceValue;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        obj.objectReferenceValue = EditorGUILayout.ObjectField(obj.objectReferenceValue, typeof(T), true);
        EditorGUILayout.EndHorizontal();
        return oldValue != obj.objectReferenceValue;
    }

    public static bool ObjectSelection<T>(string label, string toolTip, ref T obj) where T : UnityEngine.Object
    {
        var oldValue = obj;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        obj = (T) EditorGUILayout.ObjectField(obj, typeof (T), true);
        EditorGUILayout.EndHorizontal();
        return oldValue != obj;
    }

    /// <summary>
    /// create game object selection box
    /// </summary>
    /// <param name="label">name of the box</param>
    /// <param name="obj">game object to select</param>
    /// <returns>true if value has been changed</returns>
    public static bool TransformSelection(string label, ref Transform obj)
    {
        var oldValue = obj;
        EditorGUILayout.BeginHorizontal();
        obj = (Transform)EditorGUILayout.ObjectField(label, obj, typeof(Transform), true);
        EditorGUILayout.EndHorizontal();
        return oldValue != obj;
    }

    /// <summary>
    /// create game object selection box
    /// </summary>
    /// <param name="label">name of the box</param>
    /// <param name="obj">game object to select</param>
    /// <returns>true if value has been changed</returns>
    public static bool TextureSelection(string label, ref Texture2D obj)
    {
        var oldValue = obj;
        EditorGUILayout.BeginHorizontal();
        obj = (Texture2D)EditorGUILayout.ObjectField(label, obj, typeof(Texture2D), true);
        EditorGUILayout.EndHorizontal();
        return oldValue != obj;
    }

    public static bool Selection(string label, string[] labels, ref int index)
    {
        var oldValue = index;
        EditorGUILayout.BeginHorizontal();
        index = EditorGUILayout.Popup(label, index, labels);
        EditorGUILayout.EndHorizontal();
        return index != oldValue;
    }

    public static System.Enum EnumSelection(string label, string toolTip, System.Enum selectedEnum, SerializedProperty selected, ref bool changed)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        var newValue = EditorGUILayout.EnumPopup(selectedEnum);
        EditorGUILayout.EndHorizontal();
        changed |= newValue != selectedEnum;
        selected.enumValueIndex = System.Convert.ToInt32(newValue);

        return newValue;
    }

    public static bool String(string label, string toolTip, SerializedProperty input)
    {
        var oldValue = input.stringValue;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        input.stringValue = EditorGUILayout.TextField(input.stringValue);
        EditorGUILayout.EndHorizontal();
        return input.stringValue != oldValue;
    }

    public static bool Vector2(string label, ref Vector2 input)
    {
        var oldValue = input;
        EditorGUILayout.BeginHorizontal();
        input = EditorGUILayout.Vector2Field(label, input);
        EditorGUILayout.EndHorizontal();
        return input != oldValue;
    }

    public static bool Vector3(string label, string toolTip, SerializedProperty input)
    {
        var oldValue = input.vector3Value;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent(label, toolTip), GUILayout.MaxWidth(150));
        input.vector3Value = EditorGUILayout.Vector3Field("", input.vector3Value);
        EditorGUILayout.EndHorizontal();
        return input.vector3Value != oldValue;
    }

    public static void Separator(string label, float height)
    {
        GUILayout.Box(label, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(height) });
    }

    public static void WarningBox(string label, float height)
    {
        var color = GUI.color;
        GUI.color = Color.yellow;
        GUILayout.Box(label, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(height) });
        GUI.color = color;
    }
}
