using System;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
	public static ChatManager Instance;

	public event Action<string, string> OnMessageReceived;

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

	public new void SendMessage(string message)
	{
		ulong localClientId = NetworkManager.Singleton.LocalClientId;
		string playerName = PlayerConnectionHandler.Instance.GetPlayerName(localClientId);
		Debug.Log($"Sending message from player: {playerName} with Client ID: {localClientId}");
		ChatMessage chatMessage = new ChatMessage
		{
			sender = playerName,
			message = message
		};
		SendChatMessageServerRpc(chatMessage);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SendChatMessageServerRpc(ChatMessage chatMessage)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default;
				FastBufferWriter bufferWriter = __beginSendServerRpc(2253306847u, serverRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in chatMessage, default(FastBufferWriter.ForNetworkSerializable));
				__endSendServerRpc(ref bufferWriter, 2253306847u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
			{
				__rpc_exec_stage = __RpcExecStage.Send;
				Debug.Log($"Server received chat message from player: {chatMessage.sender} with message: {chatMessage.message}");
				ReceiveChatMessageClientRpc(chatMessage);
			}
		}
	}

	[ClientRpc]
	private void ReceiveChatMessageClientRpc(ChatMessage chatMessage)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default;
				FastBufferWriter bufferWriter = __beginSendClientRpc(464017086u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in chatMessage, default(FastBufferWriter.ForNetworkSerializable));
				__endSendClientRpc(ref bufferWriter, 464017086u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
			{
				__rpc_exec_stage = __RpcExecStage.Send;
				Debug.Log($"Client received chat message from player: {chatMessage.sender} with message: {chatMessage.message}");
				OnMessageReceived?.Invoke(chatMessage.sender.ToString(), chatMessage.message.ToString());
			}
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		__registerRpc(2253306847u, __rpc_handler_2253306847, "SendChatMessageServerRpc");
		__registerRpc(464017086u, __rpc_handler_464017086, "ReceiveChatMessageClientRpc");
		base.__initializeRpcs();
	}

	private static void __rpc_handler_2253306847(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out ChatMessage value, default(FastBufferWriter.ForNetworkSerializable));
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((ChatManager)target).SendChatMessageServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_464017086(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out ChatMessage value, default(FastBufferWriter.ForNetworkSerializable));
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((ChatManager)target).ReceiveChatMessageClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	protected internal override string __getTypeName()
	{
		return "ChatManager";
	}
}
