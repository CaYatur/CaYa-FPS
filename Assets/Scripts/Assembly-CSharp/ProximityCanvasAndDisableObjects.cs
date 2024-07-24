using UnityEngine;

public class ProximityCanvasAndDisableObjects : MonoBehaviour
{
	public GameObject targetCanvas;

	public GameObject targetObject;

	public GameObject[] objectsToDisable;

	public float triggerDistance = 5f;

	private bool isTriggered = false;

	private void Start()
	{
		if (targetCanvas != null)
		{
			targetCanvas.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (isTriggered || !(targetObject != null))
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float num = Vector3.Distance(gameObject.transform.position, targetObject.transform.position);
			if (!(num <= triggerDistance))
			{
				continue;
			}
			Debug.Log("Player objeye yaklaşıldı: " + gameObject.name + " Mesafe: " + num);
			if (targetCanvas != null)
			{
				targetCanvas.SetActive(value: true);
				Debug.Log("Canvas aktif edildi");
			}
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			GameObject[] array3 = objectsToDisable;
			foreach (GameObject gameObject2 in array3)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(value: false);
					Debug.Log(gameObject2.name + " devre dışı bırakıldı");
				}
			}
			isTriggered = true;
			break;
		}
	}
}
