using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;

public class NetworkManagerCampus : NetworkManager
{
	[Header("Chat GUI")]
	public Chat chatWindow;

	// Set by UI element UsernameInput OnValueChanged
	public string PlayerName { get; set; }

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
	}

	// Called by UI element NetworkAddressInput.OnValueChanged
	public void SetHostname(string hostname)
	{
		networkAddress = hostname;
	}
	public struct CreatePlayerMessage : NetworkMessage
	{
		public string name;
	}
	public override void OnStartServer()
	{
		base.OnStartServer();
		NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
	}
	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);

		// tell the server to create a player with this name
		conn.Send(new CreatePlayerMessage { name = PlayerName });
	}

	private void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
	{
		Transform startPos = GetStartPosition();
		GameObject player = startPos != null
			? Instantiate(playerPrefab, startPos.position, startPos.rotation)
			: Instantiate(playerPrefab);

		// Set player name
		MPlayer mp = player.GetComponent<MPlayer>();
		mp.playerName = createPlayerMessage.name;

		// Name player unique for debugging
		player.name = $"{playerPrefab.name} [connId={connection.connectionId}]";
		NetworkServer.AddPlayerForConnection(connection, player);
		//NetworkServer.Spawn(player.GetComponent<FPPlayer>().grabHand, connection);

		chatWindow.gameObject.SetActive(true);
	}

	public void RespawnPlayer(GameObject player)
	{
		FPPlayer fp = player.GetComponent<FPPlayer>();
		if (fp)
		{
			fp.Teleport(GetStartPosition());
		}
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		base.OnServerDisconnect(conn);

	}
}
