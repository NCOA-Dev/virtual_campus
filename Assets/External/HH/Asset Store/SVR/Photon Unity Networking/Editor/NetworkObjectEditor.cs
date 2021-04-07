using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SVR
{
    [CustomEditor(typeof(NetworkObject))]
    public class NetworkObjectEditor : Editor
    {
        SerializedProperty serializeColor;
        SerializedProperty serializeShininess;
        SerializedProperty serializeMetallic;
        SerializedProperty serializeTexture;
        SerializedProperty serializeHealth;

        SerializedProperty textureFile;
        SerializedProperty health;

        SerializedProperty interaction;
        SerializedProperty OnInteraction;

        public void OnEnable()
        {
            serializeColor = serializedObject.FindProperty("serializeColor");
            serializeShininess = serializedObject.FindProperty("serializeShininess");
            serializeMetallic = serializedObject.FindProperty("serializeMetallic");
            serializeTexture = serializedObject.FindProperty("serializeTexture");
            serializeHealth = serializedObject.FindProperty("serializeHealth");
            textureFile = serializedObject.FindProperty("textureFile");
            health = serializedObject.FindProperty("health");

            interaction = serializedObject.FindProperty("interaction");
            OnInteraction = serializedObject.FindProperty("OnInteraction");

        }

        public override void OnInspectorGUI()
        {
            NetworkObject myTarget = (NetworkObject)target;

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((NetworkObject)target), typeof(NetworkObject), false);
            GUI.enabled = true;

            EditorGUILayout.HelpBox("Only the client that spawn this can interact directly on this object", MessageType.Info);

            EditorGUILayout.PropertyField(serializeColor);
            EditorGUILayout.PropertyField(serializeShininess);
            EditorGUILayout.PropertyField(serializeMetallic);
            EditorGUILayout.PropertyField(serializeTexture);

            if (myTarget.serializeTexture)
            {
                EditorGUILayout.PropertyField(textureFile);
            }

            EditorGUILayout.PropertyField(serializeHealth);

            if (myTarget.serializeHealth)
            {
                EditorGUILayout.PropertyField(health);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(interaction);

            if(myTarget.interaction != BasicObjectInteraction.None)
            {
                EditorGUILayout.PropertyField(OnInteraction);
            }                

            serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}
