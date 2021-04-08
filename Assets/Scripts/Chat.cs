using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Chat : MonoBehaviour
{
    public InputField chatMessage;
    public Text chatHistory;
    public Scrollbar scrollbar;

    public void Awake()
    {
        MPlayer.OnMessage += OnPlayerMessage;
    }

	void OnPlayerMessage(MPlayer player, string message)
    {
        string prettyMessage = player.isLocalPlayer ?
            $"<color=#{ColorUtility.ToHtmlStringRGB(player.playerColour)}>{player.playerName}: </color> {message}" :
            $"<color=#{ColorUtility.ToHtmlStringRGB(player.playerColour)}>{player.playerName}: </color> {message}";
        AppendMessage(prettyMessage);
    }

    // Called by UI element SendButton.OnClick
    public void OnSend()
    {
        if (chatMessage.text.Trim() == "")
            return;

        // get our player
        MPlayer player = NetworkClient.connection.identity.GetComponent<MPlayer>();

        // send a message
        player.CmdSend(chatMessage.text.Trim());

        chatMessage.text = "";
    }

    internal void AppendMessage(string message)
    {
        StartCoroutine(AppendAndScroll(message));
    }

    IEnumerator AppendAndScroll(string message)
    {
        chatHistory.text += message + "\n";

        // it takes 2 frames for the UI to update ?!?!
        yield return null;
        yield return null;

        // slam the scrollbar down
        scrollbar.value = 0;
    }
}
