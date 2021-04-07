    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    namespace SVR
    {
        public class HideHandController : MonoBehaviour
        {
            private VRHandController[] hands;

            private bool toggle = true;

            private void Start()
            {
                hands = FindObjectsOfType<VRHandController>();
            }

            public void HideHand()
            {
                toggle = !toggle;

                foreach (VRHandController hand in hands)
                {
                    foreach(MeshRenderer mr in hand.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.enabled = toggle;
                    }
                }
            }
        }

    }