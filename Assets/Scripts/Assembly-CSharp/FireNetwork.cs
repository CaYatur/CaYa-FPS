using Unity.Netcode;
using UnityEngine;

public class FireNetwork : NetworkBehaviour
{
	public float damage = 10f;

	public float range = 100f;

	public float fireRate = 15f;

	public float impactForce = 30f;

	public float fireAndWait = 0.5f;

	private float nextTimeToFire = 0f;

	public Transform bulletSpawn;

	public GameObject bulletPrefab;

	public Camera fpsCam;

	public GameObject impactEffect;

	public float shootForce = 10f;

	public AudioClip shootSound;

	public AudioClip killSound;

	private AudioSource audioSource;

	private PlayerConnectionHandler playerConnectionHandler;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
		playerConnectionHandler = Object.FindFirstObjectByType<PlayerConnectionHandler>();
	}

	private void Update()
	{
		if (base.IsOwner && Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
		{
			nextTimeToFire = Time.time + 1f / fireRate;
			ShootServerRpc(fpsCam.transform.position, fpsCam.transform.forward, base.OwnerClientId);
			nextTimeToFire += fireAndWait;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void ShootServerRpc(Vector3 camPosition, Vector3 camForward, ulong shooterClientId)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			ServerRpcParams serverRpcParams = default;
			FastBufferWriter bufferWriter = __beginSendServerRpc(772378961u, serverRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in camPosition);
			bufferWriter.WriteValueSafe(in camForward);
			BytePacker.WriteValueBitPacked(bufferWriter, shooterClientId);
			__endSendServerRpc(ref bufferWriter, 772378961u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute || (!networkManager.IsServer && !networkManager.IsHost))
		{
			return;
		}
		__rpc_exec_stage = __RpcExecStage.Send;
		Vector3 position = bulletSpawn.position;
		Vector3 direction = camForward;
		if (Physics.Raycast(camPosition, camForward, out var hitInfo, range))
		{
			if (hitInfo.transform.CompareTag("Player") && hitInfo.transform.GetComponent<NetworkObject>().OwnerClientId != shooterClientId)
			{
				PlayerHealthMultiplayer component = hitInfo.transform.GetComponent<PlayerHealthMultiplayer>();
				if (component != null)
				{
					component.TakeDamage(damage, shooterClientId);
					if (component.currentHealth.Value <= 0f)
					{
						PlayKillSoundClientRpc(shooterClientId);
					}
				}
			}
			CreateImpactEffectClientRpc(hitInfo.point, hitInfo.normal);
			if (hitInfo.rigidbody != null)
			{
				hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce);
			}
		}
		CreateBulletClientRpc(position, direction, shooterClientId);
	}

	[ClientRpc]
	private void CreateImpactEffectClientRpc(Vector3 position, Vector3 normal)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default;
				FastBufferWriter bufferWriter = __beginSendClientRpc(2529250610u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in position);
				bufferWriter.WriteValueSafe(in normal);
				__endSendClientRpc(ref bufferWriter, 2529250610u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
			{
				__rpc_exec_stage = __RpcExecStage.Send;
				GameObject obj = Object.Instantiate(impactEffect, position, Quaternion.LookRotation(normal));
				Object.Destroy(obj, 2f);
			}
		}
	}

	[ClientRpc]
	private void CreateBulletClientRpc(Vector3 position, Vector3 direction, ulong shooterClientId, ClientRpcParams clientRpcParams = default(ClientRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			FastBufferWriter bufferWriter = __beginSendClientRpc(3193448912u, clientRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in position);
			bufferWriter.WriteValueSafe(in direction);
			BytePacker.WriteValueBitPacked(bufferWriter, shooterClientId);
			__endSendClientRpc(ref bufferWriter, 3193448912u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			GameObject gameObject = Object.Instantiate(bulletPrefab, position, Quaternion.LookRotation(direction));
			gameObject.GetComponent<Rigidbody>().velocity = direction * 50f;
			Object.Destroy(gameObject, 5f);
			if (NetworkManager.Singleton.LocalClientId == shooterClientId && shootSound != null && audioSource != null)
			{
				audioSource.PlayOneShot(shootSound, 1f);
			}
		}
	}

	[ClientRpc]
	private void PlayKillSoundClientRpc(ulong shooterClientId, ClientRpcParams clientRpcParams = default(ClientRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			FastBufferWriter bufferWriter = __beginSendClientRpc(2620924141u, clientRpcParams, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(bufferWriter, shooterClientId);
			__endSendClientRpc(ref bufferWriter, 2620924141u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			if (NetworkManager.Singleton.LocalClientId == shooterClientId && killSound != null && audioSource != null)
			{
				audioSource.PlayOneShot(killSound, 1f);
			}
		}
	}

	public void PlayKillSound()
	{
		if (killSound != null && audioSource != null)
		{
			audioSource.PlayOneShot(killSound, 1f);
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		__registerRpc(772378961u, __rpc_handler_772378961, "ShootServerRpc");
		__registerRpc(2529250610u, __rpc_handler_2529250610, "CreateImpactEffectClientRpc");
		__registerRpc(3193448912u, __rpc_handler_3193448912, "CreateBulletClientRpc");
		__registerRpc(2620924141u, __rpc_handler_2620924141, "PlayKillSoundClientRpc");
		base.__initializeRpcs();
	}

	private static void __rpc_handler_772378961(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			reader.ReadValueSafe(out Vector3 value2);
			ByteUnpacker.ReadValueBitPacked(reader, out ulong value3);
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((FireNetwork)target).ShootServerRpc(value, value2, value3);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_2529250610(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			reader.ReadValueSafe(out Vector3 value2);
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((FireNetwork)target).CreateImpactEffectClientRpc(value, value2);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_3193448912(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			reader.ReadValueSafe(out Vector3 value2);
			ByteUnpacker.ReadValueBitPacked(reader, out ulong value3);
			ClientRpcParams client = rpcParams.Client;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((FireNetwork)target).CreateBulletClientRpc(value, value2, value3, client);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_2620924141(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out ulong value);
			ClientRpcParams client = rpcParams.Client;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((FireNetwork)target).PlayKillSoundClientRpc(value, client);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	protected internal override string __getTypeName()
	{
		return "FireNetwork";
	}
}
