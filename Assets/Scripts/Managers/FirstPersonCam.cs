using UnityEngine;
using System.Collections;

public class FirstPersonCam : MonoBehaviour
{
    [Header("Horizontal and Vertical Look Speeds")]
    private float speedH = 2.0f;
    private float speedV = 2.0f;
    private float yaw;
    private float pitch;

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