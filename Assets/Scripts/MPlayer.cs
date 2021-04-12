using UnityEngine;
using System.Collections;
using Mirror;
using TMPro;
using System;
using UnityEngine.UI;

// Multiplayer Player controls manager

[RequireComponent(typeof(Rigidbody))]
public class MPlayer : NetworkBehaviour
{
    // Chat variables
    [SyncVar]
    public string playerName;

    [SyncVar] [HideInInspector] 
    public Color32 playerColour = Color.black;

    public static event Action<MPlayer, string> OnMessage;

    public TMP_Text username;
    private Chat chat;

    public static bool controlsEnabled { get; set; }

    void Start()
    {
        chat = GameObject.FindObjectOfType<Chat>();
        username.text = playerName;

        if (isLocalPlayer)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            controlsEnabled = true;

            // Disable own username from view
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
        if (Input.GetMouseButtonDown(0) &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            controlsEnabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            controlsEnabled = false;
        }

        if (Input.GetKeyDown(KeyCode.T))
		{
            SelectChat();
        }
        if (chat != null && chat.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Return))
        {
            chat.OnSend();
            SelectChat();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            controlsEnabled = true;
        }
    }

    private void SelectChat()
	{
        InputField textChat = chat.GetComponentInChildren<InputField>();

        if (textChat != null)
        {
            textChat.Select();
            controlsEnabled = false;
        }
        else
        {
            Debug.LogWarning("Chat not found by " + gameObject.name);
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
