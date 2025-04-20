#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;

[CustomEditor(typeof(CustomDropDown))]
public class CustomDropDownEditor : Editor
{
    private SerializedProperty m_Template;
    private SerializedProperty m_CaptionText;
    private SerializedProperty m_CaptionImage;
    private SerializedProperty m_ItemText;
    private SerializedProperty m_ItemImage;
    private SerializedProperty m_Options;
    private SerializedProperty m_DefaultSelectedOption;

    private void OnEnable()
    {
        m_Template = serializedObject.FindProperty("m_Template");
        m_CaptionText = serializedObject.FindProperty("m_CaptionText");
        m_CaptionImage = serializedObject.FindProperty("m_CaptionImage");
        m_ItemText = serializedObject.FindProperty("m_ItemText");
        m_ItemImage = serializedObject.FindProperty("m_ItemImage");

        m_Options = serializedObject.FindProperty("m_Options");
        m_DefaultSelectedOption = serializedObject.FindProperty("defaultSelectedOption");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Dropdown Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_Template);
        EditorGUILayout.PropertyField(m_CaptionText);
        EditorGUILayout.PropertyField(m_CaptionImage);
        EditorGUILayout.PropertyField(m_ItemText);
        EditorGUILayout.PropertyField(m_ItemImage);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_DefaultSelectedOption);

        EditorGUI.indentLevel++;
        for (int i = 0; i < m_Options.arraySize; i++)
        {
            SerializedProperty option = m_Options.GetArrayElementAtIndex(i);
            SerializedProperty text = option.FindPropertyRelative("text");
            SerializedProperty icon = option.FindPropertyRelative("icon");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(icon, GUIContent.none, GUILayout.Width(50));
            EditorGUILayout.PropertyField(text, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Option"))
        {
            m_Options.arraySize++;
        }

        if (GUILayout.Button("Remove Option"))
        {
            if (m_Options.arraySize > 0)
            {
                m_Options.arraySize--;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif