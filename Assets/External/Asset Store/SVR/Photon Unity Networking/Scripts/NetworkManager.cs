using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if SVR_PHOTON_UNITY_NETWORKING_SDK
using Photon.Pun;
using Photon.Realtime;
#endif

namespace SVR
{
    [RequireComponent(typeof(NetworkRoom))]
    public class NetworkManager :
#if SVR_PHOTON_UNITY_NETWORKING_SDK
        MonoBehaviourPunCallbacks
#else
        MonoBehaviour
#endif
    {

        [Tooltip("The version of the project")]
        public string gameVersion = "1";

        [Tooltip("Automatically connect to server when play")]
        public bool connectOnAwake = true;

        [Tooltip("Automatically join room after connecting to server")]
        public bool directJoinRoom = false;

        public UnityEvent onConnected;
        public UnityEvent onDisconnected;

        [Tooltip("Tick to show debug log, does not affect final build")]
        public bool showDebug = true;

        private bool connectedToServer { get; set; }

        private NetworkRoom room;

        // Start is called before the first frame update
        void Start()
        {
            connectedToServer = false;

            if (connectOnAwake)
            {
                Connect();
            }

            room = GetComponent<NetworkRoom>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Connect()
        {
#if SVR_PHOTON_UNITY_NETWORKING_SDK
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
#endif
        }

#if SVR_PHOTON_UNITY_NETWORKING_SDK
        public override void OnConnectedToMaster()
        {
            if (showDebug)
            {
                Debug.Log("[SVR VR]: Connected to server");
            }

            connectedToServer = true;

            onConnected.Invoke();

            if (directJoinRoom)
            {
                JoinRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (showDebug)
            {
                Debug.LogWarningFormat("[SVR VR]: Disconnected from server with reason {0}", cause);
            }
        }

        public void JoinRoom()
        {
            room.CreateRoom();
        }

        public bool isConnectedToServer()
        {
            return connectedToServer;
        }
#endif
    }
}
