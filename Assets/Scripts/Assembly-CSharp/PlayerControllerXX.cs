using UnityEngine;

public class PlayerControllerXX : MonoBehaviour
{
	public float flySpeed = 5f;

	public float groundCheckDistance = 0.1f;

	private bool isFlying = false;

	private void Update()
	{
		if (isFlying)
		{
			float num = 0f;
			if (Input.GetKey(KeyCode.Space))
			{
				num = 1f;
			}
			else if (Input.GetKey(KeyCode.LeftShift))
			{
				num = -1f;
			}
			bool flag = Physics.Raycast(base.transform.position, Vector3.down, groundCheckDistance);
			if (num > 0f || !flag)
			{
				Vector3 vector = new Vector3(0f, num * flySpeed * Time.deltaTime, 0f);
				base.transform.position += vector;
			}
		}
	}

	public void EnableFlightMode()
	{
		isFlying = !isFlying;
	}
}
