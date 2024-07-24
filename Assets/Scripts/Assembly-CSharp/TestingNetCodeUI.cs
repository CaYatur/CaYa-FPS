using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : MonoBehaviour
{
	[SerializeField]
	private Button startHostButton;

	[SerializeField]
	private Button startClientButton;

	[SerializeField]
	private Button startRelayHostButton;

	[SerializeField]
	private Button startRelayClientButton;

	[SerializeField]
	private GameObject canvasToHide;

	[SerializeField]
	private InputField ipAddressInputField;

	[SerializeField]
	private InputField portInputField;

	[SerializeField]
	private InputField playerNameInputField;

	[SerializeField]
	private InputField relayJoinCodeInputField;

	[SerializeField]
	private Text relayJoinCodeText;

	private NetworkManager networkManager;

	private string defaultIPAddress = "127.0.0.1";

	private ushort defaultPort = 7777;

	private string defaultPlayerName = "Player";

	private async void Awake()
	{
		networkManager = NetworkManager.Singleton;
		if (networkManager == null)
		{
			Debug.LogError("NetworkManager.Singleton is null. Ensure that the NetworkManager component is correctly set up in your scene.");
			return;
		}
		if (ipAddressInputField == null)
		{
			Debug.LogError("ipAddressInputField is not assigned.");
		}
		if (portInputField == null)
		{
			Debug.LogError("portInputField is not assigned.");
		}
		if (playerNameInputField == null)
		{
			Debug.LogError("playerNameInputField is not assigned.");
		}
		if (startHostButton == null)
		{
			Debug.LogError("startHostButton is not assigned.");
		}
		if (startClientButton == null)
		{
			Debug.LogError("startClientButton is not assigned.");
		}
		if (startRelayHostButton == null)
		{
			Debug.LogError("startRelayHostButton is not assigned.");
		}
		if (startRelayClientButton == null)
		{
			Debug.LogError("startRelayClientButton is not assigned.");
		}
		if (relayJoinCodeInputField == null)
		{
			Debug.LogError("relayJoinCodeInputField is not assigned.");
		}
		if (relayJoinCodeText == null)
		{
			Debug.LogError("relayJoinCodeText is not assigned.");
		}
		if (canvasToHide == null)
		{
			Debug.LogError("canvasToHide is not assigned.");
		}
		if (ipAddressInputField != null)
		{
			ipAddressInputField.text = defaultIPAddress;
		}
		if (portInputField != null)
		{
			portInputField.text = defaultPort.ToString();
		}
		if (playerNameInputField != null)
		{
			playerNameInputField.text = defaultPlayerName;
		}
		await InitializeUnityServices();
		if (startHostButton != null)
		{
			startHostButton.onClick.AddListener(() =>
			{
				string text = ipAddressInputField.text;
				ushort num = ushort.Parse(portInputField.text);
				string text2 = playerNameInputField.text;
				networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(text2);
				networkManager.StartHost();
				HideCanvas();
			});
		}
		if (startClientButton != null)
		{
			startClientButton.onClick.AddListener(() =>
			{
				string text = ipAddressInputField.text;
				ushort num = ushort.Parse(portInputField.text);
				string text2 = playerNameInputField.text;
				networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(text2);
				networkManager.StartClient();
				HideCanvas();
			});
		}
		if (startRelayHostButton != null)
		{
			startRelayHostButton.onClick.AddListener(async () =>
			{
				string playerName = playerNameInputField.text;
				networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(playerName);
				Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
				string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
				relayJoinCodeText.text = "Join Code: " + joinCode;
				RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
				networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
				networkManager.StartHost();
				HideCanvas();
			});
		}
		if (startRelayClientButton != null)
		{
			startRelayClientButton.onClick.AddListener(async () =>
			{
				string playerName = playerNameInputField.text;
				string joinCode = relayJoinCodeInputField.text;
				networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(playerName);
				RelayServerData relayServerData = new RelayServerData(await RelayService.Instance.JoinAllocationAsync(joinCode), "dtls");
				networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
				networkManager.StartClient();
				HideCanvas();
			});
		}
	}

	private async Task InitializeUnityServices()
	{
		await UnityServices.InitializeAsync();
		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}

	private void HideCanvas()
	{
		if (canvasToHide != null)
		{
			canvasToHide.SetActive(value: false);
		}
	}

	private void DisconnectFromServer()
	{
		if (networkManager.IsHost)
		{
			networkManager.Shutdown();
		}
		else if (networkManager.IsClient)
		{
			networkManager.Shutdown();
		}
		if (canvasToHide != null)
		{
			canvasToHide.SetActive(value: true);
		}
	}
}
