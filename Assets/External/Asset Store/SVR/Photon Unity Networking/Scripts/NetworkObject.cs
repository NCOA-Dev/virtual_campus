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
#if SVR_PHOTON_UNITY_NETWORKING_SDK
    [RequireComponent(typeof(PhotonView))]
#endif
    public class NetworkObject :
#if SVR_PHOTON_UNITY_NETWORKING_SDK
            MonoBehaviourPunCallbacks, IPunObservable
#else
            MonoBehaviour
 #endif
    {

        public NetworkSerializationType type;
        public bool serializeColor;
        public bool serializeShininess;
        public bool serializeMetallic;
        public bool serializeTexture;
        
        public bool serializeHealth;

#if SVR_PHOTON_UNITY_NETWORKING_SDK

        private Material material;
        private bool texture = false;
        public Texture textureFile;
        public float health = 100f;
        private float metallic = 0;
        private float shininess = 0;
        private float color_R = 0;
        private float color_G = 0;
        private float color_B = 0;

        private PhotonView PhotonView;

#endif

        public BasicObjectInteraction interaction = BasicObjectInteraction.None;
        public UnityEvent OnInteraction;


#if SVR_PHOTON_UNITY_NETWORKING_SDK
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (serializeColor)
                {
                    stream.SendNext(color_R);
                    stream.SendNext(color_G);
                    stream.SendNext(color_B);
                }

                if (serializeHealth)
                {
                    stream.SendNext(health);
                }

                if (serializeMetallic)
                {
                    stream.SendNext(metallic);
                }

                if (serializeShininess)
                {
                    stream.SendNext(shininess);
                }

                if (serializeTexture)
                {
                    stream.SendNext(texture);
                }
            }
            else
            {
                if (serializeColor)
                {
                    color_R = (float)stream.ReceiveNext();
                    color_G = (float)stream.ReceiveNext();
                    color_B = (float)stream.ReceiveNext();

                    ApplyColor(color_R, color_G, color_B);
                }

                if (serializeHealth)
                {
                    health = (float)stream.ReceiveNext();
                    SetHealth(health);
                }

                if (serializeMetallic)
                {
                    metallic = (float)stream.ReceiveNext();
                    SetMetallic(metallic);
                }

                if (serializeShininess)
                {
                    shininess = (float)stream.ReceiveNext();
                    SetShininess(shininess);
                }

                if (serializeTexture)
                {
                    texture = (bool)stream.ReceiveNext();
                    SetTexture(texture);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            material = GetComponentInChildren<MeshRenderer>().material;

            color_R = material.color.r;
            color_G = material.color.g;
            color_B = material.color.b;

            metallic = material.GetFloat("_Metallic");
            shininess = material.GetFloat("_Glossiness");

            PhotonView = GetComponent<PhotonView>();

        }

        // Update is called once per frame
        void Update()
        {
            if (type != NetworkSerializationType.Scene)
            {
                if(!PhotonView.IsMine)
                    return; 
            }

            switch (interaction)
            {
                case BasicObjectInteraction.LeftMousePressed:
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            OnInteraction.Invoke();
                        }
                        break;
                    }
                case BasicObjectInteraction.RightMousePressed:
                    {
                        if (Input.GetMouseButtonDown(1))
                        {
                            OnInteraction.Invoke();
                        }
                        break;
                    }
                case BasicObjectInteraction.LeftMousePressedHold:
                    {
                        if (Input.GetMouseButton(0))
                        {
                            OnInteraction.Invoke();
                        }
                        break;
                    }
                case BasicObjectInteraction.RightMousePressedHold:
                    {
                        if (Input.GetMouseButton(1))
                        {
                            OnInteraction.Invoke();
                        }
                        break;
                    }
                case BasicObjectInteraction.LeftMouseSelect:
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform == this.transform)
                                {
                                    OnInteraction.Invoke();
                                }
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.RightMouseSelect:
                    {
                        if (Input.GetMouseButtonDown(1))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform == this.transform)
                                {
                                    OnInteraction.Invoke();
                                }
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.LeftMouseSelectHold:
                    {
                        if (Input.GetMouseButton(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform == this.transform)
                                {
                                    OnInteraction.Invoke();
                                }
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.RightMouseSelectHold:
                    {
                        if (Input.GetMouseButton(1))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform == this.transform)
                                {
                                    OnInteraction.Invoke();
                                }
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.EnterKeyPressed:
                    {
                        if (Input.GetKeyDown(KeyCode.Return))
                        {
                            OnInteraction.Invoke();
                        }
                        break;
                    }
                case BasicObjectInteraction.SpaceKeyPressed:
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            OnInteraction.Invoke();
                        }
                        break;
                    }
                case BasicObjectInteraction.TouchScreen:
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Began)
                            {
                                OnInteraction.Invoke();
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.TouchScreenHold:
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Stationary)
                            {
                                OnInteraction.Invoke();
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.TouchScreenSelect:
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Began)
                            {
                                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                                RaycastHit hit;

                                if (Physics.Raycast(ray, out hit))
                                {
                                    if (hit.transform == this.transform)
                                    {
                                        OnInteraction.Invoke();
                                    }
                                }
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.TouchScreenSelectHold:
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Stationary)
                            {
                                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                                RaycastHit hit;

                                if (Physics.Raycast(ray, out hit))
                                {
                                    if (hit.transform == this.transform)
                                    {
                                        OnInteraction.Invoke();
                                    }
                                }
                            }
                        }
                        break;
                    }
                case BasicObjectInteraction.TouchScreenMove:
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Moved)
                            {
                                OnInteraction.Invoke();
                            }
                        }
                        break;
                    }
            }
        }

        public void UpdateColorR(float r)
        {
            color_R = r;

            ApplyColor(r, color_G, color_B);
        }

        public void UpdateColorG(float g)
        {
            color_G = g;

            ApplyColor(color_R, g, color_B);
        }

        public void UpdateColorB(float b)
        {
            color_B = b;

            ApplyColor(color_R, color_G, b);
        }

        public void UpdateColor(float r, float g, float b)
        {
            color_R = r;
            color_G = g;
            color_B = b;

            ApplyColor(r, g, b);
        }

        private void ApplyColor(float r, float g, float b)
        {
            Color c = new Color(r, g, b);
            material.color = c;
        }

        public void SetHealth(float h)
        {
            health = h;
        }

        public float GetHealth()
        {
            return health;
        }

        public void SetMetallic(float m)
        {
            metallic = m;
            material.SetFloat("_Metallic", metallic);
        }

        public float GetMetallic()
        {
            return metallic;
        }

        public void SetShininess(float s)
        {
            shininess = s;
            material.SetFloat("_Glossiness", shininess);
        }

        public float GetShininess()
        {
            return shininess;
        }

        public void SetTexture(bool t)
        {
            texture = t;
            GetComponent<MeshRenderer>().material.mainTexture = texture ? textureFile : null;
        }

        public bool GetTexture()
        {
            return texture;
        }
#endif

        public void ChangeOwner()
        {
#if SVR_PHOTON_UNITY_NETWORKING_SDK
            if (GetComponent<PhotonView>().Owner.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            }
#endif
        }
    }
}
