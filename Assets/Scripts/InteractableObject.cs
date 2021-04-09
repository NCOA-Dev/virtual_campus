using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class InteractableObject : NetworkTransform
{
	[Header("Interactable Object")]
    public UnityEvent onInteract;

	public enum InteractableType { Button, Grabbable };
	public InteractableType type;

	[ColorUsage(true, true)] public Color highlightCol = Color.white;
	[ColorUsage(true, true)] private Color origEmission = Color.white;
	private Rigidbody rb;
	private Vector3 startPosition;


	[SyncVar] private Vector3 currentPosition = Vector3.zero;
	[SyncVar] private Transform currentParent = null;
	[SyncVar] private Quaternion currentRotation = Quaternion.identity;
	private Animator anim;

	private void Start()
	{
		Material mat = GetComponent<MeshRenderer>().material;
		if (mat != null)
		{
			origEmission = mat.GetColor("_EmissionColor");
		}

		rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = isLocalPlayer;
		}

		startPosition = transform.position;
	}

	public void Interact(FPPlayer player)
	{
		switch (type)
		{
			case InteractableType.Button:
				PressButton();
				break;
			case InteractableType.Grabbable:
				if (!player.holdingObjectR)
				{ // Grab if not already holding
					Grab(player.grabHand);
					player.heldObject = gameObject;
					player.holdingObjectR = true;
					onInteract.Invoke();
				}
				break;
		}
	}

	public void Highlight(bool on)
	{
		Material mat = GetComponent<MeshRenderer>().material;

		if (mat != null && on)
		{
			// Activate and set emission color
			mat.EnableKeyword("_EMISSION");
			mat.SetColor("_EmissionColor", highlightCol);
		}
		else
		{
			mat.SetColor("_EmissionColor", origEmission);
		}
	}

	public void ResetAllGrabbables()
	{
		InteractableObject[] objs = GameObject.FindObjectsOfType<InteractableObject>();
		foreach (InteractableObject obj in objs)
		{
			if (obj.type == InteractableType.Grabbable)
			{
				obj.ResetPosition();
			}
		}
	}

	public void ResetPosition()
	{ // Reset the object and freeze it for a frame
		transform.position = startPosition;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.Sleep();
	}

	[Command]
	private void PressButton()
	{
		anim = GetComponent<Animator>();

		// Allow press if idle
		if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.IsInTransition(0))
		{
			SyncTriggers();
			onInteract.Invoke();
		}
	}

	//[Command]
	private void Grab(GameObject hand)
	{
		transform.SetParent(hand.transform);
		currentParent = hand.transform;
		transform.position = hand.transform.position;
		transform.rotation = hand.transform.rotation;

		rb.isKinematic = true;

		//currentParent = hand.transform;
		//currentPosition = transform.position;
		//currentRotation = transform.rotation;
	}

	#region sync
	
	//[ClientRpc]
	private void SyncTriggers()
	{
		if (anim)
		{
			anim.ResetTrigger("Press");
			anim.SetTrigger("Press");
		}
	}

	[ClientRpc]
	private void RpcSyncPositionWithClients(Vector3 positionToSync)
	{
		currentPosition = positionToSync;
	}

	[ClientRpc]
	private void RpcSyncRotationWithClients(Quaternion rotationToSync)
	{
		currentRotation = rotationToSync;
	}

	private void Update()
	{
		if (isServer)
		{
			RpcSyncPositionWithClients(transform.position);
			RpcSyncRotationWithClients(transform.rotation);
		}
	}

	private void LateUpdate()
	{
		if (!isServer)
		{
			if (currentParent != null)
			{
				currentPosition = currentParent.transform.position;
			}

			transform.position = currentPosition;
			transform.rotation = currentRotation;
		}
	}

	#endregion
}
