using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AI {
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardEditor : Editor {
        #region Constants
        private const string k_Path_ParameterNames = "m_parameterNames";
        private const string k_Path_ParameterTypes = "m_parameterTypes";

        private const string k_DisplayName_ParameterName = "Name";
        private const string k_DisplayName_ParameterType = "Type";
        private const string k_DisplayName_ParameterValue = "Value";
        private const string k_DisplayName_Create = "+";
        private const string k_DisplayName_Remove = "-";
        private const string k_DisplayName_Up = "Up";
        private const string k_DisplayName_Down = "Down";
        private const string k_ErrorMsg_SameKey = "Already exist this key";

        private static readonly GUIContent k_Content_ParameterName = new GUIContent(k_DisplayName_ParameterName);
        private static readonly GUIContent k_Content_ParameterType = new GUIContent(k_DisplayName_ParameterType);
        private static readonly GUIContent k_Content_ParameterValue = new GUIContent(k_DisplayName_ParameterValue);
        #endregion

        #region Cached Variables
        private Blackboard m_blackboard;

        private SerializedProperty m_parameterNames;
        private SerializedProperty m_parameterTypes;

        private string m_wantedName;
        private EParameterType m_wantedType;
        #endregion


        public void OnEnable () {
            m_blackboard = target as Blackboard;

            m_parameterNames = serializedObject.FindProperty(k_Path_ParameterNames);
            m_parameterTypes = serializedObject.FindProperty(k_Path_ParameterTypes);
        }
        public override void OnInspectorGUI () {
            serializedObject.Update();

            // Draw item craete menu (Rumtime Mode)
            {
                EditorGUILayout.BeginHorizontal();
                m_wantedName = EditorGUILayout.TextField(m_wantedName);
                m_wantedType = (EParameterType)EditorGUILayout.EnumPopup(m_wantedType);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(k_DisplayName_Create)) {
                    if (m_blackboard.ExistKey(m_wantedName)) {
                        Debug.LogError(k_ErrorMsg_SameKey);
                    } else {
                        switch (m_wantedType) {
                        case EParameterType.Bool: m_blackboard.SetBool(m_wantedName, default); break;
                        case EParameterType.Int: m_blackboard.SetInt(m_wantedName, default); break;
                        case EParameterType.Float: m_blackboard.SetFloat(m_wantedName, default); ; break;
                        case EParameterType.String: m_blackboard.SetString(m_wantedName, default); ; break;
                        case EParameterType.Vector2: m_blackboard.SetVector2(m_wantedName, default); ; break;
                        case EParameterType.Vector3: m_blackboard.SetVector3(m_wantedName, default); ; break;
                        case EParameterType.Object: m_blackboard.SetObject(m_wantedName, default); ; break;
                        }
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            // Draw items (Runtime Mode)
            {
                string deleteKey = null;
                
                List<string> paramNames = new List<string>(m_blackboard.GetParameterNames());
                List<EParameterType> paramTypes = new List<EParameterType>(m_blackboard.GetParameterTypes());

                for (int beg = 0, end = paramNames.Count; beg != end; ++beg) {
                    string paramName = paramNames[beg];
                    EParameterType paramType = paramTypes[beg];
                    // Header for expand
                    SerializedProperty name = m_parameterNames.GetArrayElementAtIndex(beg);

                    // Draw Header
                    {
                        EditorGUILayout.BeginHorizontal();
                        // Draw Foldout menu
                        name.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(name.isExpanded, paramName);
                        GUILayout.FlexibleSpace();
                        // Draw move up menu
                        if (GUILayout.Button(k_DisplayName_Up) && beg > 0) {
                            SwapParameter(beg, beg - 1);
                        }
                        // Draw move up down
                        if (GUILayout.Button(k_DisplayName_Down) && beg < end - 1) {
                            SwapParameter(beg, beg + 1);
                        }
                        // Draw Remove menu
                        if (GUILayout.Button(k_DisplayName_Remove)) {
                            deleteKey = paramName;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    // Draw parameter infons only expanded
                    if (name.isExpanded) {
                        EParameterType newParamType = (EParameterType)EditorGUILayout.EnumPopup(k_Content_ParameterType, paramType);
                        if (paramType != newParamType) {
                            switch (newParamType) {
                            case EParameterType.Bool: m_blackboard.SetBool(paramName, default); break;
                            case EParameterType.Int: m_blackboard.SetInt(paramName, default); break;
                            case EParameterType.Float: m_blackboard.SetFloat(paramName, default); ; break;
                            case EParameterType.String: m_blackboard.SetString(paramName, default); ; break;
                            case EParameterType.Vector2: m_blackboard.SetVector2(paramName, default); ; break;
                            case EParameterType.Vector3: m_blackboard.SetVector3(paramName, default); ; break;
                            case EParameterType.Object: m_blackboard.SetObject(paramName, default); ; break;
                            }
                            paramType = newParamType;
                        }
                        

                        switch (paramType) {
                        case EParameterType.Bool: {
                            bool oldVal = m_blackboard.GetBool(paramName);
                            bool newVal = EditorGUILayout.Toggle(k_Content_ParameterValue, oldVal);
                            if (oldVal != newVal) m_blackboard.SetBool(paramName, newVal);
                            break;
                        }
                        case EParameterType.Int: {
                            int oldVal = m_blackboard.GetInt(paramName);
                            int newVal = EditorGUILayout.IntField(k_Content_ParameterValue, oldVal);
                            if (oldVal != newVal) m_blackboard.SetInt(paramName, newVal);
                            break;
                        }
                        case EParameterType.Float: {
                            float oldVal = m_blackboard.GetFloat(paramName);
                            float newVal = EditorGUILayout.FloatField(k_Content_ParameterValue, oldVal);
                            if (oldVal != newVal) m_blackboard.SetFloat(paramName, newVal);
                            break;
                        }
                        case EParameterType.String: {
                            string oldVal = m_blackboard.GetString(paramName);
                            string newVal = EditorGUILayout.TextField(k_Content_ParameterValue, oldVal);
                            if (oldVal != newVal) m_blackboard.SetString(paramName, newVal);
                            break;
                        }
                        case EParameterType.Vector2: {
                            Vector2 oldVal = m_blackboard.GetVector2(paramName);
                            Vector2 newVal = EditorGUILayout.Vector2Field(k_Content_ParameterValue, oldVal);
                            if (oldVal != newVal) m_blackboard.SetVector2(paramName, newVal);
                            break;
                        }
                        case EParameterType.Vector3: {
                            Vector3 oldVal = m_blackboard.GetVector3(paramName);
                            Vector3 newVal = EditorGUILayout.Vector3Field(k_Content_ParameterValue, oldVal);
                            if (oldVal != newVal) m_blackboard.SetVector3(paramName, newVal);
                            break;
                        }
                        case EParameterType.Object: {
                            Object oldVal = m_blackboard.GetObject(paramName);
                            Object newVal = EditorGUILayout.ObjectField(k_Content_ParameterValue, oldVal, typeof(Object), false);
                            if (oldVal != newVal) m_blackboard.SetObject(paramName, newVal);
                            break;
                        }
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }

                // Delete item after draw all
                if (deleteKey != null) {
                    m_blackboard.Remove(deleteKey);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
        private void SwapParameter(int lhsIndex, int rhsIndex) {
            SerializedProperty name = m_parameterNames.GetArrayElementAtIndex(lhsIndex);
            SerializedProperty type = m_parameterTypes.GetArrayElementAtIndex(lhsIndex);
            SerializedProperty neighbourName = m_parameterNames.GetArrayElementAtIndex(rhsIndex);
            SerializedProperty neighbourType = m_parameterTypes.GetArrayElementAtIndex(rhsIndex);

            {
                string tmp = name.stringValue;
                name.stringValue = neighbourName.stringValue;
                neighbourName.stringValue = tmp;
            }
            {
                int tmp = type.enumValueIndex;
                type.enumValueIndex = neighbourType.enumValueIndex;
                neighbourType.enumValueIndex = tmp;
            }
        }
    }
}