using Mirror;
using System.Collections;
using UnityEngine;

// First Person Player controller

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class FPPlayer : NetworkBehaviour
{
    [Header("Player Components")]
    [Tooltip("Main camera to only enable on client.")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject head;

    [Header("Movement Settings")]
    public CharacterController characterController;
    [SerializeField] private float mouseSensitivity = 1;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float gravity = 15.0f;
    [SerializeField] private float jumpSpeedReduction = 0.6f;
    [SerializeField] private float airMovementImpact = 0.25f; 
    [SerializeField] private float jumpDelay = 0.05f;

    // Character controlling
    private readonly float speedH = 2.0f;
    private readonly float speedV = 2.0f;
    private float yaw;
    private float pitch;
    private float horizontal;
    private float vertical;
    private float origMoveSpeed;
    private bool wasGrounded = false;
    private bool canJump = true;

    private Vector3 moveDirection = Vector3.zero;

    void OnValidate()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        characterController.enabled = isLocalPlayer;
        origMoveSpeed = moveSpeed;
        cam.enabled = isLocalPlayer;
    }

    void OnDisable()
    {
        if (cam != null)
        {
            cam.enabled = false;
        }
    }

    // Controls - do not register when paused
    void Update()
    {
        if (!isLocalPlayer)
            return;

        // Only allow input if controls are enabled
        if (MPlayer.controlsEnabled)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetButton("Sprint") && characterController.isGrounded)
                moveSpeed = sprintSpeed;
            else
                moveSpeed = origMoveSpeed;

            yaw += speedH * mouseSensitivity * Input.GetAxis("Mouse X");
            pitch -= speedV * mouseSensitivity * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 50f);
        }
        else
        {
            horizontal = 0;
            vertical = 0;
        }
    }

    // Physics - occur even when paused
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        // Perform turn
        head.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if (characterController.isGrounded)
        { // Move while on the ground
            moveDirection = new Vector3(horizontal, 0, vertical);
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f); // Prevent horizontal-sprinting
            moveDirection = head.transform.TransformDirection(moveDirection);
            moveDirection.y = 0; // Prevent look-jumping
            moveDirection *= moveSpeed;

            if (Input.GetButton("Jump") && MPlayer.controlsEnabled && canJump)
			{
                moveDirection *= jumpSpeedReduction;
                moveDirection.y = jumpSpeed;

                canJump = false;
            }

            // Wait for next jump when landed and has delay
            if (!wasGrounded)
			{
                StartCoroutine(JumpDelay());
            }

            wasGrounded = true;
        }
		else
		{ // Move while mid-air (reduced capacity)
            Vector3 mov = new Vector3(horizontal, 0, vertical);
            mov = Vector3.ClampMagnitude(mov, 1f);
            mov = head.transform.TransformDirection(mov);
            mov.y = 0;

            moveDirection += mov * airMovementImpact;

            wasGrounded = false;
        }

		moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // Teleport player to given point
    public void Teleport(Transform point)
    {
        if (isLocalPlayer)
		{
            characterController.enabled = false;
            moveDirection = Vector3.zero;
            transform.position = point.position;
            transform.rotation = point.rotation;
            characterController.enabled = true;
        }
    }

    private IEnumerator JumpDelay()
	{
        yield return new WaitForSeconds(jumpDelay);
        canJump = true;
    }

    // Bump physics
    void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Rigidbody body = hit.collider.attachedRigidbody;
		//Vector3 force = hit.controller.velocity * 2;

		//if (body != null)
		//{ // Apply the push
		//	body.AddForceAtPosition(force, hit.point);
		//}
	}
}
