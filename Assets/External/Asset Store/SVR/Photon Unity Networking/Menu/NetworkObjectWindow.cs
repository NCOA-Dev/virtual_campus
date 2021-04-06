using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

#if SVR_PHOTON_UNITY_NETWORKING_SDK
using Photon.Pun;
using Photon.Realtime;
#endif

#if UNITY_EDITOR
namespace SVR
{
    public class NetworkObjectWindow : EditorWindow
    {

#if SVR_PHOTON_UNITY_NETWORKING_SDK
        static bool pos = true;
        static bool rot = true;
        static bool scale = false;

        static bool color = true;
        static bool shininess = false;
        static bool metallic = false;
        static bool texture = false;

        static bool animation = false;

        //static NetworkSerializationType type;

        static bool teleport = false;
        static bool velocity = true;
        static bool angularVelocity = false;

        // CUSTOM PROPERTIES
        static bool health = false;

        // INTERACTION
        static BasicObjectInteraction interaction;
        static UnityEvent OnInteraction;

        bool handleRepaintErrors = false;
        public void OnGUI()
        {
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }
            //EditorStyles.label.wordWrap = true;
            GUILayout.Label("Select the components to be synchronized", EditorStyles.boldLabel);

            float originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160;

            //EditorGUILayout.Separator();

            //type = (NetworkSerializationType)EditorGUILayout.EnumPopup("Type of network object", type);

            //if(type == NetworkSerializationType.Scene)
            //{
            //    EditorGUILayout.HelpBox("Scene network object is static object that is shared by every clients.", MessageType.Info);
            //}
            //else if (type == NetworkSerializationType.RunTime)
            //{
            //    EditorGUILayout.HelpBox("Run Time network object should be instantiated and does not belong to the scene.", MessageType.Info);
            //}

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Transformation", EditorStyles.boldLabel);

            pos = EditorGUILayout.Toggle("Position", pos);
            rot = EditorGUILayout.Toggle("Rotation", rot);
            scale = EditorGUILayout.Toggle("Scale", scale);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);

            color = EditorGUILayout.Toggle("Color", color);
            shininess = EditorGUILayout.Toggle("Shininess", shininess);
            metallic = EditorGUILayout.Toggle("Metallic", metallic);
            texture = EditorGUILayout.Toggle("Texture", texture);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

            animation = EditorGUILayout.Toggle("Animation", animation);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Rigid Body", EditorStyles.boldLabel);

            teleport = EditorGUILayout.Toggle("Teleport", teleport);
            velocity = EditorGUILayout.Toggle("Velocity", velocity);
            angularVelocity = EditorGUILayout.Toggle("Angular Velocity", angularVelocity);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Custom Properties", EditorStyles.boldLabel);

            health = EditorGUILayout.Toggle("Health", health);

            interaction = (BasicObjectInteraction)EditorGUILayout.EnumPopup("Interaction type", interaction);

            if (interaction != BasicObjectInteraction.None)
            {
                EditorGUILayout.HelpBox("You may setup the interation event in the inspector later", MessageType.Info);
            }

            // FROM HERE
            GameObject target = Selection.activeGameObject;

            if (GUILayout.Button("Create"))
            {
                if(EditorUtility.DisplayDialog("Network Object", "Make " + target.name + " as a network object?", "OK", "Cancel"))
                {
                    PhotonView photonView;
                    PhotonTransformView photonTransformView;
                    PhotonAnimatorView photonAnimatorView;
                    PhotonRigidbodyView photonRigidbodyView;
                    NetworkObject networkObject;

                    try
                    {
                        List<Component> components = new List<Component>();

                        if (!target.GetComponent<PhotonView>())
                        {
                            photonView = target.AddComponent<PhotonView>();
                        }
                        else
                        {
                            photonView = target.GetComponent<PhotonView>();
                        }

                        if (!target.GetComponent<PhotonTransformView>())
                        {
                            photonTransformView = target.AddComponent<PhotonTransformView>();
                        }
                        else
                        {
                            photonTransformView = target.GetComponent<PhotonTransformView>();
                        }

                        photonTransformView.m_SynchronizePosition = pos;
                        photonTransformView.m_SynchronizeRotation = rot;
                        photonTransformView.m_SynchronizeScale = scale;

                        components.Add(photonTransformView as Component);

                        if (!target.GetComponent<NetworkObject>())
                        {
                            networkObject = target.AddComponent<NetworkObject>();
                        }
                        else
                        {
                            networkObject = target.GetComponent<NetworkObject>();
                        }

                        networkObject.serializeColor = color;
                        networkObject.serializeMetallic = metallic;
                        networkObject.serializeShininess = shininess;
                        networkObject.serializeTexture = texture;
                        networkObject.serializeHealth = health;
                        networkObject.interaction = interaction;
                        components.Add(networkObject as Component);

                        if (animation)
                        {
                            if (!target.GetComponent<PhotonAnimatorView>())
                            {
                                photonAnimatorView = target.AddComponent<PhotonAnimatorView>();
                            }
                            else
                            {
                                photonAnimatorView = target.GetComponent<PhotonAnimatorView>();
                            }

                            components.Add(photonAnimatorView as Component);
                        }

                        if(teleport || velocity || angularVelocity)
                        {
                            if (!target.GetComponent<PhotonRigidbodyView>())
                            {
                                photonRigidbodyView = target.AddComponent<PhotonRigidbodyView>();
                            }
                            else
                            {
                                photonRigidbodyView = target.GetComponent<PhotonRigidbodyView>();
                            }

                            photonRigidbodyView.m_TeleportEnabled = teleport;
                            photonRigidbodyView.m_SynchronizeVelocity = velocity;
                            photonRigidbodyView.m_SynchronizeAngularVelocity = angularVelocity;

                            components.Add(photonRigidbodyView as Component);
                        }

                        //if (customCount > 0)
                        //{
                        //    networkObject.customProperties = new Dictionary<string, DataType>();
                        //    for (int i=0; i< customCount; i++)
                        //    {
                        //        networkObject.customProperties.Add(dataNames[i], dataTypes[i]);
                        //    }
                        //}

                        photonView.ObservedComponents = components;

                        photonView.Synchronization = ViewSynchronization.UnreliableOnChange;

                        // OTHER SERIALIZATION CLASS, CUSTOM MAKE
                    }
                    catch (System.NullReferenceException)
                    {
                        EditorUtility.DisplayDialog("No object selected", "You must select a game object in the scene hierarchy to make it network object.", "OK");
                        return;
                    }

                    //if (type == NetworkSerializationType.Scene)
                    //{
                    //    // Do NTHG
                    //}
                    //else if (type == NetworkSerializationType.RunTime)
                    //{
                        string localPath = "Assets/SVR/Photon Unity Networking/Demos/Resources/" + target.name + ".prefab";

                        // Make sure the file name is unique, in case an existing Prefab has the same name.
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                        // Create the new Prefab.
                        PrefabUtility.SaveAsPrefabAssetAndConnect(target, localPath, InteractionMode.UserAction);

                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(localPath);

                        Object.DestroyImmediate(target);

                        //EditorUtility.DisplayDialog("Run Time Network Object", "Run Time Network Object (prefab) can be instantiated during run time via Network Object Handler", "OK");
                    //}
                }

                Close();
            }
        }
#endif
    }
}
#endif