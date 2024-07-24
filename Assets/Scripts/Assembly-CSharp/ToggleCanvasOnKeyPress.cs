using Unity.Netcode;
using UnityEngine;

public class ToggleCanvasOnKeyPress : NetworkBehaviour
{
	public Canvas targetCanvas;

	public KeyCode toggleKey = KeyCode.Tab;

	private void Start()
	{
		if (targetCanvas != null)
		{
			targetCanvas.enabled = false;
		}
	}

	private void Update()
	{
		if (base.IsOwner)
		{
			if (Input.GetKeyDown(toggleKey))
			{
				ToggleCanvasServerRpc(isShown: true);
			}
			else if (Input.GetKeyUp(toggleKey))
			{
				ToggleCanvasServerRpc(isShown: false);
			}
		}
	}

	[ServerRpc]
	private void ToggleCanvasServerRpc(bool isShown, ServerRpcParams rpcParams = default(ServerRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			if (base.OwnerClientId != networkManager.LocalClientId)
			{
				if (networkManager.LogLevel <= LogLevel.Normal)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			FastBufferWriter bufferWriter = __beginSendServerRpc(3047535798u, rpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in isShown, default(FastBufferWriter.ForPrimitives));
			__endSendServerRpc(ref bufferWriter, 3047535798u, rpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			ToggleCanvasClientRpc(isShown, rpcParams.Receive.SenderClientId);
		}
	}

	[ClientRpc]
	private void ToggleCanvasClientRpc(bool isShown, ulong clientId)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			ClientRpcParams clientRpcParams = default;
			FastBufferWriter bufferWriter = __beginSendClientRpc(4216175600u, clientRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in isShown, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(bufferWriter, clientId);
			__endSendClientRpc(ref bufferWriter, 4216175600u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			if (NetworkManager.Singleton.LocalClientId == clientId && targetCanvas != null)
			{
				targetCanvas.enabled = isShown;
			}
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		__registerRpc(3047535798u, __rpc_handler_3047535798, "ToggleCanvasServerRpc");
		__registerRpc(4216175600u, __rpc_handler_4216175600, "ToggleCanvasClientRpc");
		base.__initializeRpcs();
	}

	private static void __rpc_handler_3047535798(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
		{
			if (networkManager.LogLevel <= LogLevel.Normal)
			{
				Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
			}
		}
		else
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			ServerRpcParams server = rpcParams.Server;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((ToggleCanvasOnKeyPress)target).ToggleCanvasServerRpc(value, server);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_4216175600(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			ByteUnpacker.ReadValueBitPacked(reader, out ulong value2);
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((ToggleCanvasOnKeyPress)target).ToggleCanvasClientRpc(value, value2);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	protected internal override string __getTypeName()
	{
		return "ToggleCanvasOnKeyPress";
	}
}
