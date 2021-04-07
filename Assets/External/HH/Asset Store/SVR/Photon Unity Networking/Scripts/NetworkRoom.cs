using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if SVR_PHOTON_UNITY_NETWORKING_SDK
using Photon.Pun;
using Photon.Realtime;
#endif

namespace SVR
{

    [RequireComponent(typeof(NetworkManager))]
    public class NetworkRoom :
#if SVR_PHOTON_UNITY_NETWORKING_SDK
        MonoBehaviourPunCallbacks
#else
        MonoBehaviour
#endif
    {
        public string roomName = "Custom Room";

        [Tooltip("Using text will override the set room name")]
        public Text roomNameText;

        public int maxNumberOfPlayers = 4;
        public int minNumberOfPlayers = 1;

        public bool isOpen = true;
        public bool isVisible = true;

        public UnityEvent OnJoinedRoomSuccess;
        public UnityEvent OnJoinedRoomFailed;

        private NetworkManager master;

#if SVR_PHOTON_UNITY_NETWORKING_SDK

        private bool connectedToRoom = false;

        private RoomOptions options;

        private void Start()
        {
            connectedToRoom = false;
            options = new RoomOptions
            {
                MaxPlayers = (byte)maxNumberOfPlayers,
                IsOpen = isOpen,
                IsVisible = isVisible
            };

            master = GetComponent<NetworkManager>();
        }

        public void CreateRoom()
        {
            if (!roomNameText)
            {
                PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
            }
            else
            {
                PhotonNetwork.JoinOrCreateRoom(roomNameText.text, options, TypedLobby.Default);
            }
        }

        public override void OnJoinedRoom()
        {
            connectedToRoom = true;
            if (master.showDebug)
            {
                if(roomNameText)
                    Debug.LogFormat("[SVR VR]: Join room success, client is in {0} room.", roomNameText.text);
                else
                    Debug.LogFormat("[SVR VR]: Join room success, client is in {0} room.", roomName);
            }

            OnJoinedRoomSuccess.Invoke();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (master.showDebug)
            {
                Debug.LogFormat("[SVR VR]: Join room failed due to {0}", message);
            }

            OnJoinedRoomFailed.Invoke();
        }

        public bool isConnectedToRoom()
        {
            return connectedToRoom;
        }
#endif
    }

}