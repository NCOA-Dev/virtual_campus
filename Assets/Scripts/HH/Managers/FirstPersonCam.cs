using UnityEngine;
using System.Collections;
using Mirror;

public class FirstPersonCam : NetworkBehaviour
{
    [Header("Horizontal and Vertical Look Speeds")]
    private float speedH = 2.0f;
    private float speedV = 2.0f;
    private float yaw;
    private float pitch;

    // Update is called once per frame
    void Update()
    {
        // WebGL-only speed
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            speedH = 1f;
            speedV = 1f;
        }

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
