using Mirror;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class FPPlayer : NetworkBehaviour
{
    [Header("Player Components")]
    [Tooltip("Camera to replace with main camera.")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject head;

    [Header("Movement Settings")]
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 3f;

    // Character controlling
    private float speedH = 2.0f;
    private float speedV = 2.0f;
    private float yaw;
    private float pitch;
    public CharacterController characterController;

    // Diagnostics
    private float horizontal;
    private float vertical;
    private float jumpSpeed;
    private bool isGrounded = true;
    private bool isFalling;

    void OnValidate()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        characterController.enabled = isLocalPlayer;

		if (isLocalPlayer)
		{
            cam.transform.SetParent(head.transform);
            cam.transform.position = cam.transform.position;
            cam.transform.rotation = Quaternion.identity;
		}
        else
		{
            cam.enabled = false;
		}
	}

    void OnDisable()
    {
        if (cam != null)
        {
            cam.enabled = false;
        }
    }

    void Update()
    {
        if (!isLocalPlayer || Cursor.visible)
            return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        yaw += speedH * mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= speedV * mouseSensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 50f);

        //if (Input.GetKey(KeyCode.T))
        //chat

        if (isGrounded)
            isFalling = false;

        if ((isGrounded || !isFalling) && jumpSpeed < jumpForce && Input.GetKey(KeyCode.Space))
        {
            jumpSpeed = Mathf.Lerp(jumpSpeed, jumpForce, 0.5f);
        }
        else if (!isGrounded)
        {
            isFalling = true;
            jumpSpeed = 0;
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || characterController == null || Cursor.visible)
            return;

        // Perform turn
        head.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Calculate movement
        Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);
        bool notMoving = direction.x == 0 && direction.z == 0;
        direction = head.transform.TransformDirection(direction);
        if (notMoving)
            direction = new Vector3(0, direction.y, 0); 
        direction *= moveSpeed;

        // Move character controller
        if (jumpSpeed > 0)
            characterController.Move(direction * Time.fixedDeltaTime);
        else
            characterController.SimpleMove(direction);

        isGrounded = characterController.isGrounded;
    }
}
