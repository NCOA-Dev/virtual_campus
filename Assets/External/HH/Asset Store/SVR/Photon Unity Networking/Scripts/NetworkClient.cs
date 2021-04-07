using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if SVR_PHOTON_UNITY_NETWORKING_SDK
using Photon.Pun;
using Photon.Realtime;
#endif

namespace SVR
{
    public class NetworkClient :
#if SVR_PHOTON_UNITY_NETWORKING_SDK
        MonoBehaviourPunCallbacks
#else
        MonoBehaviour
#endif
    {

        [Tooltip("The name of the client")]
        public string clientName;

        [Tooltip("If set, we will read the name from text input field")]
        public Text clientNameText;

        // CALLED AFTER THE CONNECTION TO SERVER HAS BEEN ESTABLISHED AND JOINED A NETWORK ROOM
        public void SetClientName()
        {
#if SVR_PHOTON_UNITY_NETWORKING_SDK
            PhotonNetwork.NickName = clientName;
#endif
        }

        private void FixedUpdate()
        {
            if(clientNameText)
                clientName = clientNameText.text;
        }
    }
}
