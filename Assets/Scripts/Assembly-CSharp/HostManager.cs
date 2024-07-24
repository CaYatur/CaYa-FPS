using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostManager : MonoBehaviour
{
	private void OnEnable()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
		}
	}

	private void OnDisable()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
		}
	}

	private void OnClientDisconnect(ulong clientId)
	{
		if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
		{
			HandleHostDisconnect();
		}
	}

	private void HandleHostDisconnect()
	{
		Debug.Log("Host has disconnected.");
		NotifyClientsToLoadLobby();
	}

	[ServerRpc(RequireOwnership = false)]
	private void NotifyClientsToLoadLobbyServerRpc()
	{
		LoadLobbyClientRpc();
	}

	[ClientRpc]
	private void LoadLobbyClientRpc()
	{
		Debug.Log("Loading Lobby Scene");
		SceneManager.LoadScene("LobbyScene");
	}

	private void NotifyClientsToLoadLobby()
	{
		if (NetworkManager.Singleton != null)
		{
			NotifyClientsToLoadLobbyServerRpc();
		}
		else
		{
			Debug.LogError("NetworkManager.Singleton is null");
		}
	}
}
