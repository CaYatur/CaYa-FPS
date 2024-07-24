using Unity.Netcode;
using UnityEngine;

public class NetworkToggleCanvas : NetworkBehaviour
{
	public Canvas canvasToShowOnKey;

	public Canvas[] canvasesToHideOnKey;

	public Canvas[] canvasesToHideOnStart;

	public KeyCode toggleKey = KeyCode.Escape;

	public bool lockCursorOnShow = false;

	public bool lockCursorOnHide = false;

	public bool pauseGameOnShow = false;

	private bool isGamePaused;

	private void Start()
	{
		if (base.IsOwner)
		{
			Canvas[] array = canvasesToHideOnStart;
			foreach (Canvas canvas in array)
			{
				canvas.gameObject.SetActive(value: false);
			}
			SetCursorState(lockCursorOnHide);
		}
	}

	private void Update()
	{
		if (base.IsOwner && Input.GetKeyDown(toggleKey))
		{
			ToggleCanvasVisibilityServerRpc();
		}
	}

	private void ToggleCanvasVisibility()
	{
		bool flag = !canvasToShowOnKey.gameObject.activeSelf;
		canvasToShowOnKey.gameObject.SetActive(flag);
		Canvas[] array = canvasesToHideOnKey;
		foreach (Canvas canvas in array)
		{
			canvas.gameObject.SetActive(!flag);
		}
		SetCursorState(flag ? lockCursorOnShow : lockCursorOnHide);
		if (pauseGameOnShow)
		{
			ToggleGamePause(flag);
		}
	}

	private void ToggleGamePause(bool pause)
	{
		isGamePaused = pause;
		Time.timeScale = (pause ? 0f : 1f);
	}

	private void SetCursorState(bool lockCursor)
	{
		Cursor.lockState = (lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
		Cursor.visible = !lockCursor;
	}

	[ServerRpc(RequireOwnership = false)]
	private void ToggleCanvasVisibilityServerRpc(ServerRpcParams rpcParams = default(ServerRpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
			{
				FastBufferWriter bufferWriter = __beginSendServerRpc(157507052u, rpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 157507052u, rpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
			{
				__rpc_exec_stage = __RpcExecStage.Send;
				ToggleCanvasVisibilityClientRpc();
			}
		}
	}

	[ClientRpc]
	private void ToggleCanvasVisibilityClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Execute && (networkManager.IsServer || networkManager.IsHost))
		{
			ClientRpcParams clientRpcParams = default;
			FastBufferWriter bufferWriter = __beginSendClientRpc(208263169u, clientRpcParams, RpcDelivery.Reliable);
			__endSendClientRpc(ref bufferWriter, 208263169u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Execute && (networkManager.IsClient || networkManager.IsHost))
		{
			__rpc_exec_stage = __RpcExecStage.Send;
			if (base.IsOwner)
			{
				ToggleCanvasVisibility();
			}
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		__registerRpc(157507052u, __rpc_handler_157507052, "ToggleCanvasVisibilityServerRpc");
		__registerRpc(208263169u, __rpc_handler_208263169, "ToggleCanvasVisibilityClientRpc");
		base.__initializeRpcs();
	}

	private static void __rpc_handler_157507052(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ServerRpcParams server = rpcParams.Server;
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((NetworkToggleCanvas)target).ToggleCanvasVisibilityServerRpc(server);
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	private static void __rpc_handler_208263169(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Execute;
			((NetworkToggleCanvas)target).ToggleCanvasVisibilityClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.Send;
		}
	}

	protected internal override string __getTypeName()
	{
		return "NetworkToggleCanvas";
	}
}
