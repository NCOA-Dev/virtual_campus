using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SVR_PHOTON_UNITY_NETWORKING_SDK
using Photon.Pun;
using Photon.Realtime;
#endif

namespace SVR
{
    public class NetworkLoadScene :
#if SVR_PHOTON_UNITY_NETWORKING_SDK
        MonoBehaviourPunCallbacks
#else
        MonoBehaviour
#endif
    {

        [Tooltip("The name of the scene to load as Network Scene")]
        public string sceneName = "Custom Scene";

        [Tooltip("The delay before load scene")]
        public float delayInSeconds = 0f;

        [Tooltip("Tick for all client to synchronize sync scene")]
        public bool syncLoadScene = true;

        [Tooltip("Tick for AR VR multiplayer")]
        public bool ARVR = false;

        [Tooltip("Index of scene")]
        public int sceneIndex = 1;

        private void Start()
        {
#if SVR_PHOTON_UNITY_NETWORKING_SDK
            PhotonNetwork.AutomaticallySyncScene = syncLoadScene;
            if(ARVR)
                PhotonNetwork.AutomaticallySyncScene = true;
#endif
        }

        // TO BE CALLED TO LOAD NETWORK SCENE
        public void LoadScene()
        {
            StartCoroutine(DelayLoadScene());
        }

        private IEnumerator DelayLoadScene()
        {
            yield return new WaitForSeconds(delayInSeconds);

#if SVR_PHOTON_UNITY_NETWORKING_SDK
            if(!ARVR)
                PhotonNetwork.LoadLevel(sceneName);
            else
                PhotonNetwork.LoadLevel(sceneIndex);
#endif
        }
    }
}
