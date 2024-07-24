using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectScript : MonoBehaviour
{
	[SerializeField]
	private Button disconnectButton;

	[SerializeField]
	private GameObject canvasToShowOnDisconnect;

	private NetworkManager networkManager;

	private void Awake()
	{
		networkManager = NetworkManager.Singleton;
		if (disconnectButton != null)
		{
			disconnectButton.onClick.AddListener(DisconnectFromServer);
		}
		else
		{
			Debug.LogError("Disconnect button is not assigned.");
		}
	}

	private void DisconnectFromServer()
	{
		if (networkManager.IsHost || networkManager.IsClient)
		{
			networkManager.Shutdown();
		}
		if (canvasToShowOnDisconnect != null)
		{
			canvasToShowOnDisconnect.SetActive(value: true);
		}
	}
}
