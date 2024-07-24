using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public Transform targetObject;

	public float distance = 5f;

	public float height = 3f;

	private void Update()
	{
		if (targetObject == null)
		{
			Debug.LogWarning("Target object is not assigned!");
			return;
		}
		Vector3 position = targetObject.position;
		Vector3 position2 = position - targetObject.forward * distance + Vector3.up * height;
		base.transform.position = position2;
		base.transform.LookAt(position);
	}
}
