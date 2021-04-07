using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

namespace SVR
{
    public class SteamVRTeleportManager : MonoBehaviour
    {
#if SVR_STEAMVR_SDK
        [Tooltip("The parent transform of the teleport point spawning to")]
        public Transform parent;

        public void SpawnTeleportPoint(Vector3 position)
        {
            try
            {
                Object teleportPointPrefab = AssetDatabase.LoadAssetAtPath("Assets/SteamVR/InteractionSystem/Teleport/Prefabs/TeleportPoint.prefab", typeof(GameObject));
                GameObject teleportPoint = Instantiate(teleportPointPrefab, position, Quaternion.identity) as GameObject;
                teleportPoint.name = "Teleporting Point";
                if (parent) teleportPoint.transform.parent = parent;
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning("[SVR VR]: SteamVR Teleport Points not found. Failed to instantate Teleport Points prefabs");
            }
        }
#endif
    }

    [CustomEditor(typeof(SteamVRTeleportManager))]
    public class SteamVRTeleportManagerEditor : Editor
    {
#if SVR_STEAMVR_SDK
        public static bool isPlacing = false;

        private static GUIStyle ToggleButtonStyleNormal = null;
        private static GUIStyle ToggleButtonStyleToggled = null;

        private SteamVRTeleportManager myTarget;

        public override void OnInspectorGUI()
        {
            myTarget = (SteamVRTeleportManager)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            if (GUILayout.Button("Place Teleport Point", isPlacing ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                isPlacing = !isPlacing;
            }

            EditorGUILayout.Space();
        }

        public void OnSceneGUI()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                mousePosition = ray.origin;
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject)
                    {
                        if (isPlacing && myTarget)
                        {
                            myTarget.SpawnTeleportPoint(hit.point);
                        }
                    }
                    Event.current.Use();
                }

            }
            if (isPlacing && myTarget)
            {
                Selection.activeGameObject = myTarget.gameObject;
            }
        }
#endif
    }
}

#endif