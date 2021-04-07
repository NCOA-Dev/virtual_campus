using UnityEngine;
using System.Collections;
using Mirror;
using TMPro;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MPlayer : NetworkBehaviour
{
    // Chat variables
    [SyncVar]
    public string playerName;
    public static event Action<MPlayer, string> OnMessage;

    public TMP_Text username;
    private Chat chat;

    void Start()
    {
        chat = GameObject.FindObjectOfType<Chat>();
        username.text = playerName;

        if (isLocalPlayer)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Disable own username
            username.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Stop update if is not client player
        if (!isLocalPlayer)
            return;

        Controls();
    }

    private void Controls()
	{
        if (Cursor.visible && Input.GetMouseButtonDown(0) &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.T))
		{
            // get inputfield
            // inputfield.Select()
            // enable cursor
		}

        if (Cursor.visible && chat != null && chat.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Return))
        {
            chat.OnSend();
        }
    }

	#region chat

	[Command]
    public void CmdSend(string message)
    {
        if (message.Trim() != "")
            RpcReceive(message.Trim());
    }
    [ClientRpc]
    public void RpcReceive(string message)
    {
        OnMessage?.Invoke(this, message);
    }

	#endregion
}
