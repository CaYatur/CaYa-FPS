using Unity.Netcode;
using UnityEngine;

public class PM : NetworkBehaviour
{
	[SerializeField]
	private float moveSpeed = 5f;

	private void Update()
	{
		HandlePlayerMovement();
	}

	private void HandlePlayerMovement()
	{
		if (base.IsOwner)
		{
			Vector3 zero = Vector3.zero;
			if (Input.GetKey(KeyCode.W))
			{
				zero.z = 1f;
			}
			if (Input.GetKey(KeyCode.S))
			{
				zero.z = -1f;
			}
			if (Input.GetKey(KeyCode.A))
			{
				zero.x = -1f;
			}
			if (Input.GetKey(KeyCode.D))
			{
				zero.x = 1f;
			}
			base.transform.position += zero * moveSpeed * Time.deltaTime;
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	protected internal override string __getTypeName()
	{
		return "PM";
	}
}
