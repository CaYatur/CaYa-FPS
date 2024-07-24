using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
	public float normalSpeed = 40f;

	public float boostedSpeed = 80f;

	public KeyCode boostKey = KeyCode.LeftShift;

	public Transform playerCamera;

	private CharacterController characterController;

	private bool isSpeed = false;

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
		if (characterController == null)
		{
			Debug.LogError("CharacterController component is missing from this GameObject.");
		}
	}

	private void Update()
	{
		if (isSpeed && characterController != null && playerCamera != null)
		{
			float num = (Input.GetKey(boostKey) ? boostedSpeed : normalSpeed);
			Vector3 forward = playerCamera.forward;
			forward.y = 0f;
			Vector3 normalized = (forward.normalized * Input.GetAxis("Vertical") + playerCamera.right * Input.GetAxis("Horizontal")).normalized;
			characterController.Move(normalized * num * Time.deltaTime);
		}
	}

	public void EnableSpeedMode()
	{
		isSpeed = !isSpeed;
	}
}
