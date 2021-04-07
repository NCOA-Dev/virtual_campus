using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#if SVR_STEAMVR_SDK
using Valve.VR.InteractionSystem;
using System.IO;
using UnityEditor.SceneManagement;
#endif

namespace SVR
{
    public class SteamVR_Menu : EditorWindow
    {

#if SVR_STEAMVR_SDK
        [MenuItem("SVR/Steam VR/Input/Generate", false, 42)]
        public static void SteamVRGenerateInput()
        {
            Valve.VR.SteamVR_Input_EditorWindow.ShowWindow();
        }

        [MenuItem("SVR/Steam VR/Input/Reset", false, 43)]
        public static void SteamVRResetInput()
        {
            if (EditorUtility.DisplayDialog("Reset SteamVR Input?", "Reset this will make the current input to be erased.", "Reset", "Cancel"))
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\actions.json");
                AssetDatabase.Refresh();
            }

            Valve.VR.SteamVR_Input_EditorWindow.ShowWindow();
        }

        [MenuItem("SVR/Steam VR/Open Sample Scene", false, 80)]
        private static void SteamVROpenScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/SteamVR/InteractionSystem/Samples/Interactions_Example.unity");
        }

        // SAMPLE (TO BE UPDATED)
        [MenuItem("SVR/Steam VR/Add SteamVR Player", false, 41)]
        private static void SteamVRPlayer()
        {
            try
            {
                Object playerPrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Core/Prefabs/Player.prefab", typeof(GameObject));
                GameObject player = Instantiate(playerPrefab) as GameObject;
                player.name = "SteamVR Player";
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR]: SteamVR Player not found. Failed to instantate Player prefabs");
            }
        }

        [MenuItem("SVR/Steam VR/Teleport Manager", false, 45)]
        private static void SteamVRTeleport()
        {
            try
            {
                Object teleportingPrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Teleport/Prefabs/Teleporting.prefab", typeof(GameObject));
                GameObject teleporting = Instantiate(teleportingPrefab) as GameObject;
                teleporting.name = "Teleporting";
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR]: SteamVR Teleporting not found. Failed to instantate Teleporting prefabs");
            }

            try
            {
                GameObject teleportManagerPrefab = Resources.Load<GameObject>("SteamVR Teleport Manager") as GameObject;
                GameObject teleportManager = Instantiate(teleportManagerPrefab);
                teleportManager.name = "SteamVR Teleport Manager";
                Selection.activeGameObject = teleportManager;
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR]: SteamVR Teleport Manager not found. Failed to instantate Teleport Manager prefabs");
            }
        }

        [MenuItem("SVR/Steam VR/Button/Hover Button", false, 48)]
        private static void SteamVRHoverButton()
        {
            try
            {
                Object buttonPrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Samples/Prefabs/Button.prefab", typeof(GameObject));
                GameObject button = Instantiate(buttonPrefab) as GameObject;
                button.name = "Hover Button";
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR]: SteamVR Hover Button not found. Failed to instantate Hover Button prefabs");
            }
        }

        [MenuItem("SVR/Steam VR/Interactive Object/Throwable/Create Your Own", false, 46)]
        private static void SteamVRThrowable()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("Add Throwable failed", "You must select a game object in the scene hierarchy to add Throwable.", "OK");
                return;
            }

            GameObject target = Selection.activeGameObject;

            try
            {
                // Must have collider to add interaction
                if (!target.GetComponent<Collider>())
                {
                    EditorUtility.DisplayDialog("Collider Missing", "Add required component of type 'BoxCollider' or 'CapsuleCollider' or 'CharacterController' or 'MeshCollider' or 'SphereCollider' or 'TerrainCollider' or 'WheelCollider' to the game object.", "OK");
                    return;
                }

                try
                {
                    Object cubePrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Samples/Prefabs/ThrowableCube.prefab", typeof(GameObject));
                    GameObject throwable = Instantiate(cubePrefab) as GameObject;
                    throwable.name = "Throwable " + target.name;
                    DestroyImmediate(throwable.transform.Find("Cube").gameObject);
                    throwable.transform.parent = target.transform.parent;
                    throwable.transform.localPosition = new Vector3(0, 0, 0);
                    target.transform.parent = throwable.transform;
                    target.transform.localPosition = new Vector3(0, 0, 0);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogWarning("[SVR]: SteamVR Throwable Ball not found. Failed to instantate Throwable Ball prefabs");
                }

            }
            catch (System.NullReferenceException)
            {
                EditorUtility.DisplayDialog("Add Interaction failed", "You must select a game object in the scene hierarchy to add interaction.", "OK");
                return;
            }

        }

        [MenuItem("SVR/Steam VR/Interactive Object/Throwable/Sample Ball", false, 46)]
        private static void SteamVRThrowableBall()
        {
            try
            {
                Object ballPrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Samples/Prefabs/ThrowableBall.prefab", typeof(GameObject));
                GameObject ball = Instantiate(ballPrefab) as GameObject;
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR]: SteamVR Throwable Ball not found. Failed to instantate Throwable Ball prefabs");
            }
        }

        [MenuItem("SVR/Steam VR/Interactive Object/Throwable/Sample Cube", false, 46)]
        private static void SteamVRThrowableCube()
        {
            try
            {
                Object cubePrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Samples/Prefabs/ThrowableCube.prefab", typeof(GameObject));
                GameObject cube = Instantiate(cubePrefab) as GameObject;
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR]: SteamVR Throwable Ball not found. Failed to instantate Throwable Ball prefabs");
            }
        }

        [MenuItem("SVR/Steam VR/Interactive Object/Interactable/Simple", false, 47)]
        private static void SteamVRInteractableSimple()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("Add Interactable failed", "You must select a game object in the scene hierarchy to add Interactable.", "OK");
                return;
            }

            GameObject target = Selection.activeGameObject;

            try
            {
                // Must have collider to add interaction
                if (!target.GetComponent<Collider>())
                {
                    EditorUtility.DisplayDialog("Collider Missing", "Add required component of type 'BoxCollider' or 'CapsuleCollider' or 'CharacterController' or 'MeshCollider' or 'SphereCollider' or 'TerrainCollider' or 'WheelCollider' to the game object.", "OK");
                    return;
                }

                try
                {
                    GameObject interact = new GameObject("Interactable Simple " + target.name, typeof(Interactable));
                    interact.transform.parent = target.transform.parent;
                    interact.transform.localPosition = new Vector3(0, 0, 0);
                    target.transform.parent = interact.transform;
                    target.transform.localPosition = new Vector3(0, 0, 0);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogWarning("[SVR]: SteamVR Throwable Ball not found. Failed to instantate Throwable Ball prefabs");
                }

            }
            catch (System.NullReferenceException)
            {
                EditorUtility.DisplayDialog("Add Interaction failed", "You must select a game object in the scene hierarchy to add interaction.", "OK");
                return;
            }
        }

        [MenuItem("SVR/Steam VR/Interactive Object/Interactable/Return to Position", false, 47)]
        private static void SteamVRInteractableReturn()
        {
            // To exclude the game object in asset folder
            if (Selection.assetGUIDs.Length > 0)
            {
                EditorUtility.DisplayDialog("Add Interactable failed", "You must select a game object in the scene hierarchy to add Interactable.", "OK");
                return;
            }

            GameObject target = Selection.activeGameObject;

            try
            {
                // Must have collider to add interaction
                if (!target.GetComponent<Collider>())
                {
                    EditorUtility.DisplayDialog("Collider Missing", "Add required component of type 'BoxCollider' or 'CapsuleCollider' or 'CharacterController' or 'MeshCollider' or 'SphereCollider' or 'TerrainCollider' or 'WheelCollider' to the game object.", "OK");
                    return;
                }

                try
                {
                    GameObject interact = new GameObject("Interactable Return " + target.name, typeof(Interactable));
                    interact.transform.parent = target.transform.parent;
                    interact.transform.localPosition = new Vector3(0, 0, 0);
                    interact.AddComponent<InteractableCustom>();

                    target.transform.parent = interact.transform;
                    target.transform.localPosition = new Vector3(0, 0, 0);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogWarning("[SVR]: SteamVR Throwable Ball not found. Failed to instantate Throwable Ball prefabs");
                }

            }
            catch (System.NullReferenceException)
            {
                EditorUtility.DisplayDialog("Add Interaction failed", "You must select a game object in the scene hierarchy to add interaction.", "OK");
                return;
            }
        }

        [MenuItem("SVR/Steam VR/Button/UI Button", false, 49)]
        private static void SteamVRUIButton()
        {
            //try
            //{
            //    Object buttonPrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Samples/Prefabs/Button.prefab", typeof(GameObject));
            //    GameObject button = Instantiate(buttonPrefab) as GameObject;
            //    button.name = "Hover Button";
            //}
            //catch (System.ArgumentException)
            //{
            //    Debug.LogWarning("[SVR]: SteamVR Hover Button not found. Failed to instantate Hover Button prefabs");
            //}

            EditorUtility.DisplayDialog("Trick you", "This function is currently unavailable, but you may go ahead to sample scene to check out the sample UI button", "OK");
        }

#endif

    }
}

#endif