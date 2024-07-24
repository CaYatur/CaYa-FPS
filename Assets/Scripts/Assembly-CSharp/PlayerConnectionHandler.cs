using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerConnectionHandler : MonoBehaviour
{
	public static PlayerConnectionHandler Instance;

	public TMP_Text connectedPlayersText;

	private Dictionary<ulong, PlayerInfo> connectedPlayers = new Dictionary<ulong, PlayerInfo>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager singleton = NetworkManager.Singleton;
			singleton.ConnectionApprovalCallback = (Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>)Delegate.Combine(singleton.ConnectionApprovalCallback, new Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>(ApprovalCheck));
			NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
			NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
		}
		if (connectedPlayersText == null)
		{
			Debug.LogError("ConnectedPlayersText is not assigned in the Inspector.");
		}
	}

	private void OnDestroy()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager singleton = NetworkManager.Singleton;
			singleton.ConnectionApprovalCallback = (Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>)Delegate.Remove(singleton.ConnectionApprovalCallback, new Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>(ApprovalCheck));
			NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
			NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
		}
	}

	private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		string baseName = Encoding.UTF8.GetString(request.Payload);
		ulong clientNetworkId = request.ClientNetworkId;
		baseName = GetUniquePlayerName(baseName);
		Debug.Log($"{baseName} has joined the game with Client ID: {clientNetworkId}.");
		connectedPlayers[clientNetworkId] = new PlayerInfo
		{
			Name = baseName,
			Kills = 0
		};
		UpdateConnectedPlayersText();
		response.Approved = true;
		response.CreatePlayerObject = true;
		response.Position = Vector3.zero;
		response.Rotation = Quaternion.identity;
		UpdateConnectedPlayersOnClients(clientNetworkId);
	}

	private void HandleClientDisconnect(ulong clientId)
	{
		if (connectedPlayers.ContainsKey(clientId))
		{
			connectedPlayers.Remove(clientId);
			UpdateConnectedPlayersText();
			UpdateConnectedPlayersOnClients(clientId);
		}
	}

	private void HandleClientConnected(ulong clientId)
	{
		SendConnectedPlayersToClient(clientId);
	}

	private void UpdateConnectedPlayersText()
	{
		if (connectedPlayersText != null)
		{
			connectedPlayersText.text = "Connected Players:\n" + string.Join("\n", connectedPlayers.Values.Select((PlayerInfo p) => $"{p.Name} - Kills: {p.Kills}"));
		}
	}

	[ClientRpc]
	private void UpdateConnectedPlayersOnClients(ulong clientId)
	{
		PlayerInfo[] playerInfos = connectedPlayers.Values.ToArray();
		UpdateConnectedPlayersOnClientsClientRpc(playerInfos, clientId);
	}

	public void SendConnectedPlayersToClient(ulong clientId)
	{
		PlayerInfo[] playerInfos = connectedPlayers.Values.ToArray();
		UpdateConnectedPlayersOnClientsClientRpc(playerInfos, clientId);
	}

	[ClientRpc]
	private void UpdateConnectedPlayersOnClientsClientRpc(PlayerInfo[] playerInfos, ulong clientId)
	{
		connectedPlayersText.text = "Oyuncular:\n" + string.Join("\n", playerInfos.Select((PlayerInfo p) => $"{p.Name} - Öldürme: {p.Kills}"));
	}

	private string GetUniquePlayerName(string baseName)
	{
		int num = 0;
		string uniqueName = baseName;
		while (connectedPlayers.Values.Any((PlayerInfo p) => p.Name == uniqueName))
		{
			uniqueName = $"{baseName}({num})";
			num++;
		}
		return uniqueName;
	}

	public IEnumerable<string> GetConnectedPlayers()
	{
		return connectedPlayers.Values.Select((PlayerInfo p) => p.Name);
	}

	public void AddKill(ulong clientId)
	{
		if (connectedPlayers.ContainsKey(clientId))
		{
			connectedPlayers[clientId].Kills++;
			UpdateConnectedPlayersText();
			UpdateConnectedPlayersOnClients(clientId);
		}
	}

	public string GetPlayerName(ulong clientId)
	{
		if (connectedPlayers.ContainsKey(clientId))
		{
			string text = connectedPlayers[clientId].Name;
			Debug.Log($"Player name for Client ID {clientId} is {text}");
			return text;
		}
		Debug.Log($"Player name for Client ID {clientId} is Unknown");
		return "Unknown";
	}
}
