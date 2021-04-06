using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SVR
{
    [CustomEditor(typeof(NetworkObjectHandler))]
    public class NetworkObjectHandlerEditor : Editor
    {
        SerializedProperty networkObject;
        SerializedProperty useTransform;

        SerializedProperty trans;
        SerializedProperty pos;
        SerializedProperty euler;

        SerializedProperty parent;

        public void OnEnable()
        {
            networkObject = serializedObject.FindProperty("networkObject");
            useTransform = serializedObject.FindProperty("useTransform");
            trans = serializedObject.FindProperty("trans");
            pos = serializedObject.FindProperty("pos");
            euler = serializedObject.FindProperty("euler");
            parent = serializedObject.FindProperty("parent");
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script ", MonoScript.FromMonoBehaviour((NetworkObjectHandler)target), typeof(NetworkObjectHandler), false);
            GUI.enabled = true;

            EditorGUILayout.HelpBox("Each type of Network Object require one network object handler\n\nCall SpawnNetworkObject function during Run Time to spawn the assigned network object", MessageType.Info);

            NetworkObjectHandler myTarget = (NetworkObjectHandler)target;

            EditorGUILayout.PropertyField(networkObject);
            EditorGUILayout.PropertyField(useTransform);

            EditorGUILayout.LabelField("Spawn Transformation", EditorStyles.boldLabel);

            if (myTarget.useTransform)
            {
                EditorGUILayout.PropertyField(trans, new GUIContent("Transform"));
            }
            else
            {
                EditorGUILayout.PropertyField(pos, new GUIContent("Position"));
                EditorGUILayout.PropertyField(euler, new GUIContent("Rotation"));

                myTarget.rot = Quaternion.Euler(myTarget.euler);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(parent);

            EditorGUILayout.Space();

            if (GUILayout.Button("Spawn Network Object"))
            {
                myTarget.SpawnNetworkObjectDebug();
            }
            EditorGUILayout.HelpBox("This button only helps to debug, not going to work with Photon Network", MessageType.None);

            serializedObject.ApplyModifiedProperties();
        }

    }
}
#endif

