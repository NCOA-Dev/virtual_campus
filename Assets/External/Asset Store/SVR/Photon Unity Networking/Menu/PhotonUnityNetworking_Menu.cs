using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#if SVR_PHOTON_UNITY_NETWORKING_SDK
using Photon.Pun;
using Photon.Realtime;
#endif

namespace SVR
{
    public class PhotonUnityNetworking_Menu : EditorWindow
    {
#if SVR_PHOTON_UNITY_NETWORKING_SDK
        // SETTING UP API KEYS
        [MenuItem("SVR/Photon/Setup Settings", false, 51)]
        public static void PhotonSetupSettings()
        {
            Selection.activeObject = Resources.Load("PhotonServerSettings");
            //EditorUtility.DisplayDialog("Setup Photon Unity Networking", "Make sure you setup the APP ID for Photon Unity Networking", "OK");
        }

        // CONNECTION TO MASTER SERVER
        [MenuItem("SVR/Photon/Network Manager", false, 52)]
        public static void PhotonConnect()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("No object selected", "You must select a game object in the scene hierarchy to be Network Manager.", "OK");
                return;
            }

            AddTag("Network Manager");

            GameObject target = Selection.activeGameObject;

            if(GameObject.FindWithTag("Network Manager"))
            {
                EditorUtility.DisplayDialog("Duplicated Network Manager", "Only one Network Manager can exists", "OK");
                Selection.activeGameObject = GameObject.FindWithTag("Network Manager");
                return;
            }

            try
            {
                if (!target.GetComponent<NetworkManager>())
                {
                    target.AddComponent<NetworkManager>();
                }

                if (!target.GetComponent<NetworkRoom>())
                {
                    target.AddComponent<NetworkRoom>();
                }

                target.name = "Network Manager";
                target.tag = "Network Manager";
            }
            catch (System.NullReferenceException)
            {
                EditorUtility.DisplayDialog("Network Manager", "You must select a game object in the scene hierarchy to be Network Manager.", "OK");
                return;
            }
        }

        //// CLIENT OPTIONS
        [MenuItem("SVR/Photon/Network Client Interface", false, 53)]
        private static void PhotonNetworkClient()
        {
            // Spawn a canvas to let user input name
            GameObject go = Instantiate(Resources.Load<GameObject>("Network UI Canvas")) as GameObject;

            go.name = "Network UI Canvas";

            Canvas canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = FindObjectOfType<Camera>();

            GameObject go2 = Instantiate(Resources.Load<GameObject>("Network Client")) as GameObject;
            go2.name = "Network Client";

            NetworkClient client = go2.GetComponent<NetworkClient>();
            client.clientNameText = go.transform.Find("Name Input").Find("Name").GetComponent<Text>();
        }

        // LOAD SCENE
        // Require Scene Name or Index
        [MenuItem("SVR/Photon/Network Load Scene", false, 54)]
        private static void PhotonScene()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("No object selected", "You must select a game object in the scene hierarchy to be Network Load Scene Object.", "OK");
                return;
            }

            GameObject target = Selection.activeGameObject;

            try
            {
                if (!target.GetComponent<NetworkLoadScene>())
                {
                    target.AddComponent<NetworkLoadScene>();
                }

                target.name = "Network Scene Loader";
            }
            catch (System.NullReferenceException)
            {
                EditorUtility.DisplayDialog("Network Scene", "You must select a game object in the scene hierarchy to be Network Load Scene Object.", "OK");
                return;
            }
        }

        // TURN SCENE OBJECT INTO PHOTON SERIALIZABLE OBJECT
        // Till This
        [MenuItem("SVR/Photon/Network Object", false, 80)]
        private static void PhotonNetworkObject()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("No object selected", "You must select a game object in the scene hierarchy to make it network object.", "OK");
                return;
            }

            GetWindow(typeof(NetworkObjectWindow), false, "Network Object");

        }

        [MenuItem("SVR/Photon/Network Object Handler", false, 81)]
        private static void PhotonNetworkObjectHandler()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("No object selected", "You must select a game object in the scene hierarchy to make it Network Object Handler.", "OK");
                return;
            }

            //GetWindow(typeof(NetworkObjectWindow), false, "Network Object");

            GameObject target = Selection.activeGameObject;

            try
            {
                if (!target.GetComponent<NetworkObjectHandler>())
                {
                    target.AddComponent<NetworkObjectHandler>();
                }

                target.name = "Network Object Handler";
            }
            catch (System.NullReferenceException)
            {
                EditorUtility.DisplayDialog("Network Object Handler", "Failed to make the object into Network Object Handler.", "OK");
                return;
            }

        }

        // SET UP THE RPC CALL ON PHOTON SERIALIZABLE OBJECT
        // HAVE SOME BASIC PHOTON RPC FUNCTION SETUP
        //[MenuItem("SVR/Photon/RPC", false, 82)]
        //private static void PhotonRPCSetup()
        //{
            
        //}

        //[MenuItem("SVR/Photon/Sample Scene/Simple/Lobby", false, 100)]
        //private static void PhotonSampleSceneSimple()
        //{
        //    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        //    EditorSceneManager.OpenScene("Assets/SVR/Photon Unity Networking/Demos/Scenes/Photon Demo.unity");
        //}

        //[MenuItem("SVR/Photon/Sample Scene/Simple/Main", false, 101)]
        //private static void PhotonSampleSceneSimpleMain()
        //{
        //    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        //    EditorSceneManager.OpenScene("Assets/SVR/Photon Unity Networking/Demos/Scenes/Photon Demo 1.unity");
        //}

        [MenuItem("SVR/Photon/Sample Scene/Complex", false, 102)]
        private static void PhotonSampleSceneComplex()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Photon/PhotonUnityNetworking/Demos/PunBasics-Tutorial/Scenes/PunBasics-Launcher.unity");
        }

        [MenuItem("SVR/Photon/Photon Documentation", false, 103)]
        private static void PhotonDocumentation()
        {
            Application.OpenURL("https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro");
        }

        // ADD TAG FUNCTION
        public static void AddTag(string tag)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            if (asset != null)
            { // sanity checking
                var so = new SerializedObject(asset);
                var tags = so.FindProperty("tags");

                bool found = false;

                var numTags = tags.arraySize;
                // do not create duplicates
                for (int i = 0; i < numTags; i++)
                {
                    var existingTag = tags.GetArrayElementAtIndex(i);
                    if (existingTag.stringValue == tag)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    tags.InsertArrayElementAtIndex(numTags);
                    tags.GetArrayElementAtIndex(numTags).stringValue = tag;
                    so.ApplyModifiedProperties();
                    so.Update();
                }
            }
        }
#endif
    }
}


#endif