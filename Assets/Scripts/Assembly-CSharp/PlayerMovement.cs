using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	public CharacterController controller;

	public float walkSpeed = 5f;

	public float runSpeed = 10f;

	public float jumpHeight = 3f;

	public float gravity = -9.81f;

	public float mouseSensitivity = 2000f;

	public Transform playerBody;

	public Camera playerCamera;

	private Vector3 velocity;

	private bool isGrounded;

	private float xRotation = 0f;

	private void Start()
	{
		if (!base.IsOwner)
		{
			playerCamera.enabled = false;
			return;
		}
		playerCamera.enabled = true;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		if (base.IsOwner)
		{
			MovePlayer();
			RotateCamera();
		}
	}

	private void MovePlayer()
	{
		if (base.IsOwner)
		{
			isGrounded = controller.isGrounded;
			if (isGrounded && velocity.y < 0f)
			{
				velocity.y = -2f;
			}
			float axis = Input.GetAxis("Vertical");
			float axis2 = Input.GetAxis("Horizontal");
			Vector3 vector = base.transform.forward * axis + base.transform.right * axis2;
			float num = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
			controller.Move(vector * num * Time.deltaTime);
			if (Input.GetButtonDown("Jump") && isGrounded)
			{
				Jump();
			}
			velocity.y += gravity * Time.deltaTime;
			controller.Move(velocity * Time.deltaTime);
		}
	}

	private void RotateCamera()
	{
		float num = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float num2 = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		xRotation -= num2;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		playerBody.Rotate(Vector3.up * num);
	}

	private void ToggleRun()
	{
	}

	private void Jump()
	{
		if (isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}
	}

	private void MoveForward()
	{
		Vector3 forward = base.transform.forward;
		float num = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
		controller.Move(forward * num * Time.deltaTime);
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
		return "PlayerMovement";
	}
}
