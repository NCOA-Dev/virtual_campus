using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SVR
{
    [CustomEditor(typeof(NetworkLoadScene))]
    public class NetworkLoadSceneEditor : Editor
    {
        SerializedProperty sceneName;
        SerializedProperty delayInSeconds;
        SerializedProperty syncLoadScene;
        SerializedProperty ARVR;
        SerializedProperty sceneIndex;

        public void OnEnable()
        {
            sceneName = serializedObject.FindProperty("sceneName");
            delayInSeconds = serializedObject.FindProperty("delayInSeconds");
            syncLoadScene = serializedObject.FindProperty("syncLoadScene");
            ARVR = serializedObject.FindProperty("ARVR");
            sceneIndex = serializedObject.FindProperty("sceneIndex");
        }

        public override void OnInspectorGUI()
        {
            NetworkLoadScene myTarget = (NetworkLoadScene)target;

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((NetworkLoadScene)target), typeof(NetworkLoadScene), false);
            GUI.enabled = true;

            if (!myTarget.ARVR)
            {
                EditorGUILayout.PropertyField(sceneName);
                EditorGUILayout.PropertyField(delayInSeconds);
                EditorGUILayout.PropertyField(syncLoadScene);

                EditorGUILayout.PropertyField(ARVR);
            }
            else
            {
                EditorGUILayout.PropertyField(ARVR);
                EditorGUILayout.PropertyField(sceneIndex);
                EditorGUILayout.PropertyField(delayInSeconds);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif