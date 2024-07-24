using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthMultiplayer : NetworkBehaviour
{
	public float maxHealth = 100f;

	public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0f);

	public Text healthText;

	public string spawnTag = "Spawn";

	private GameObject[] spawnPointsC;

	public GameObject explosionPrefab;

	public int particleCount = 100;

	public float explosionForce = 10f;

	private bool isFirstSpawn = true;

	private PlayerConnectionHandler playerConnectionHandler;

	public Canvas[] canvasesToControl;

	public Canvas[] canvasesToIgnore;

	private void Start()
	{
		spawnPointsC = GameObject.FindGameObjectsWithTag(spawnTag);
		playerConnectionHandler = UnityEngine.Object.FindFirstObjectByType<PlayerConnectionHandler>();
	}

	public override void OnNetworkSpawn()
	{
		if (base.IsServer)
		{
			currentHealth.Value = maxHealth;
			if (isFirstSpawn)
			{
				Respawn();
				isFirstSpawn = false;
			}
		}
		if (base.IsOwner)
		{
			UpdateHealthUI();
			NetworkVariable<float> networkVariable = currentHealth;
			networkVariable.OnValueChanged = (NetworkVariable<float>.OnValueChangedDelegate)Delegate.Combine(networkVariable.OnValueChanged, (NetworkVariable<float>.OnValueChangedDelegate)((float oldValue, float newValue) =>
			{
				UpdateHealthUI();
			}));
			EnableCanvas(isEnabled: false);
			EnableCanvas2(isEnabled: true);
		}
		else
		{
			EnableCanvas(isEnabled: false);
			EnableCanvas2(isEnabled: false);
		}
	}

	public void TakeDamage(float damage, ulong attackerClientId)
	{
		if (base.IsServer)
		{
			currentHealth.Value -= damage;
			if (currentHealth.Value <= 0f)
			{
				Die(attackerClientId);
			}
		}
	}

	public bool IsDead()
	{
		return currentHealth.Value <= 0f;
	}

	private void Die(ulong attackerClientId)
	{
		base.gameObject.SetActive(value: false);
		SpawnExplosionClientRpc(base.transform.position);
		if (base.IsOwner)
		{
			Respawn();
		}
		else
		{
			RequestRespawnServerRpc();
		}
		playerConnectionHandler.AddKill(attackerClientId);
		PlayKillSoundClientRpc(attackerClientId);
	}

	[ClientRpc]
	private void PlayKillSoundClientRpc(ulong attackerClientId)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			ClientRpcParams clientRpcParams = default;
			FastBufferWriter bufferWriter = __beginSendClientRpc(1057758681u, clientRpcParams, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(bufferWriter, attackerClientId);
			__endSendClientRpc(ref bufferWriter, 1057758681u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			FireNetwork component = NetworkManager.Singleton.ConnectedClients[attackerClientId].PlayerObject.GetComponent<FireNetwork>();
			if (component != null)
			{
				component.PlayKillSound();
			}
		}
	}

	[ClientRpc]
	private void RespawnClientRpc(Vector3 newPosition, Quaternion newRotation, ClientRpcParams clientRpcParams = default(ClientRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
			{
				FastBufferWriter bufferWriter = __beginSendClientRpc(1426378577u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in newPosition);
				bufferWriter.WriteValueSafe(in newRotation);
				__endSendClientRpc(ref bufferWriter, 1426378577u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
			{
				__rpc_exec_stage = __RpcExecStage.Send;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.gameObject.SetActive(value: true);
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.gameObject.SetActive(value: false);
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.gameObject.SetActive(value: true);
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
				base.transform.position = newPosition;
				base.transform.rotation = newRotation;
			}
		}
	}

	private void Respawn()
	{
		if (base.IsOwner)
		{
			PerformRespawn();
		}
		else
		{
			PerformRespawn();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void RequestRespawnServerRpc(ServerRpcParams rpcParams = default(ServerRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
			{
				FastBufferWriter bufferWriter = __beginSendServerRpc(1188022835u, rpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 1188022835u, rpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
			{
				__rpc_exec_stage = __RpcExecStage.Send;
				PerformRespawn();
			}
		}
	}

	private void PerformRespawn()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(spawnTag);
		currentHealth.Value = maxHealth;
		if (array.Length != 0)
		{
			GameObject gameObject = array[UnityEngine.Random.Range(0, array.Length)];
			base.transform.position = gameObject.transform.position;
			base.transform.rotation = gameObject.transform.rotation;
			RespawnClientRpc(base.transform.position, base.transform.rotation, new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = NetworkManager.Singleton.ConnectedClientsList.Select((NetworkClient client) => client.ClientId).ToArray()
				}
			});
		}
		else
		{
			Debug.LogWarning("Spawn points with tag '" + spawnTag + "' not found.");
		}
	}

	[ClientRpc]
	private void SpawnExplosionClientRpc(Vector3 position)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			ClientRpcParams clientRpcParams = default;
			FastBufferWriter bufferWriter = __beginSendClientRpc(251495165u, clientRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in position);
			__endSendClientRpc(ref bufferWriter, 251495165u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute || (!networkManager.IsClient && !networkManager.IsHost))
		{
			return;
		}
		__rpc_exec_stage = __RpcExecStage.Send;
		if (explosionPrefab != null)
		{
			for (int i = 0; i < particleCount; i++)
			{
				Vector3 vector = base.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f;
				GameObject gameObject = UnityEngine.Object.Instantiate(explosionPrefab, vector, Quaternion.identity);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(explosionForce, vector, 1f);
				}
				UnityEngine.Object.Destroy(gameObject, 10f);
			}
		}
		else
		{
			Debug.Log("Patlama efekti bulunamadı!");
		}
	}

	private void UpdateHealthUI()
	{
		if (healthText != null)
		{
			healthText.text = $"Canın: {currentHealth.Value}";
		}
	}

	private void EnableCanvas(bool isEnabled)
	{
		Canvas[] array = canvasesToControl;
		foreach (Canvas canvas in array)
		{
			if (canvas != null)
			{
				canvas.gameObject.SetActive(isEnabled);
			}
		}
	}

	private void EnableCanvas3(bool isEnabled)
	{
		Canvas[] array = canvasesToIgnore;
		foreach (Canvas canvas in array)
		{
			if (canvas != null)
			{
				canvas.gameObject.SetActive(isEnabled);
			}
		}
	}

	private void EnableCanvas2(bool isEnabled)
	{
		Canvas componentInChildren = GetComponentInChildren<Canvas>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(isEnabled);
		}
	}

	protected override void __initializeVariables()
	{
		if (currentHealth == null)
		{
			throw new Exception("PlayerHealthMultiplayer.currentHealth cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		currentHealth.Initialize(this);
		__nameNetworkVariable(currentHealth, "currentHealth");
		NetworkVariableFields.Add(currentHealth);
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		__registerRpc(1057758681u, __rpc_handler_1057758681, "PlayKillSoundClientRpc");
		__registerRpc(1426378577u, __rpc_handler_1426378577, "RespawnClientRpc");
		__registerRpc(1188022835u, __rpc_handler_1188022835, "RequestRespawnServerRpc");
		__registerRpc(251495165u, __rpc_handler_251495165, "SpawnExplosionClientRpc");
		base.__initializeRpcs();
	}

	private static void __rpc_handler_1057758681(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out ulong value);
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((PlayerHealthMultiplayer)target).PlayKillSoundClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_1426378577(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			reader.ReadValueSafe(out Quaternion value2);
			ClientRpcParams client = rpcParams.Client;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((PlayerHealthMultiplayer)target).RespawnClientRpc(value, value2, client);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_1188022835(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ServerRpcParams server = rpcParams.Server;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((PlayerHealthMultiplayer)target).RequestRespawnServerRpc(server);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_251495165(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((PlayerHealthMultiplayer)target).SpawnExplosionClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	protected internal override string __getTypeName()
	{
		return "PlayerHealthMultiplayer";
	}
}
