using UnityEngine;

public class BackgroundCameraRotation : MonoBehaviour
{
	public float rotationSpeed = 10f;

	private void Update()
	{
		Time.timeScale = 1f;
		base.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
