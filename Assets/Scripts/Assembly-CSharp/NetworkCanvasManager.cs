using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkCanvasManager : NetworkBehaviour
{
	[SerializeField]
	private TMP_Text syncedText;

	private NetworkVariable<FixedString128Bytes> networkText = new NetworkVariable<FixedString128Bytes>();

	private Coroutine syncCoroutine;

	public override void OnNetworkSpawn()
	{
		if (base.IsServer)
		{
			networkText.Value = "Host default text";
			syncCoroutine = StartCoroutine(SyncTextCoroutine());
		}
		NetworkVariable<FixedString128Bytes> networkVariable = networkText;
		networkVariable.OnValueChanged = (NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate)Delegate.Combine(networkVariable.OnValueChanged, new NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate(OnNetworkTextChanged));
		if (syncedText != null)
		{
			syncedText.text = networkText.Value.ToString();
		}
	}

	private void OnNetworkTextChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
	{
		if (syncedText != null)
		{
			syncedText.text = newValue.ToString();
		}
	}

	public void UpdateText(string newText)
	{
		if (base.IsServer)
		{
			networkText.Value = newText;
		}
		else
		{
			SubmitTextRequestServerRpc(newText);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SubmitTextRequestServerRpc(string newText, ServerRpcParams rpcParams = default(ServerRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			FastBufferWriter bufferWriter = __beginSendServerRpc(2955590077u, rpcParams, RpcDelivery.Reliable);
			bool value = newText != null;
			bufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			if (value)
			{
				bufferWriter.WriteValueSafe(newText);
			}
			__endSendServerRpc(ref bufferWriter, 2955590077u, rpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			networkText.Value = newText;
		}
	}

	private IEnumerator SyncTextCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			networkText.Value = syncedText.text;
		}
	}

	public override void OnDestroy()
	{
		NetworkVariable<FixedString128Bytes> networkVariable = networkText;
		networkVariable.OnValueChanged = (NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate)Delegate.Remove(networkVariable.OnValueChanged, new NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate(OnNetworkTextChanged));
		if (syncCoroutine != null)
		{
			StopCoroutine(syncCoroutine);
		}
		base.OnDestroy();
	}

	protected override void __initializeVariables()
	{
		if (networkText == null)
		{
			throw new Exception("NetworkCanvasManager.networkText cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		networkText.Initialize(this);
		__nameNetworkVariable(networkText, "networkText");
		NetworkVariableFields.Add(networkText);
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		__registerRpc(2955590077u, __rpc_handler_2955590077, "SubmitTextRequestServerRpc");
		base.__initializeRpcs();
	}

	private static void __rpc_handler_2955590077(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			string s = null;
			if (value)
			{
				reader.ReadValueSafe(out s, false);
			}
			ServerRpcParams server = rpcParams.Server;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((NetworkCanvasManager)target).SubmitTextRequestServerRpc(s, server);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	protected internal override string __getTypeName()
	{
		return "NetworkCanvasManager";
	}
}
