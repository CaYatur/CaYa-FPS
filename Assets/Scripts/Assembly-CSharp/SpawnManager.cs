using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	private class DestroyableObject : MonoBehaviour
	{
		public SpawnManager spawnManager;

		private void OnDestroy()
		{
			if (spawnManager != null && spawnManager.isSpawning)
			{
				spawnManager.OnCloneDestroyed();
			}
		}
	}

	public GameObject[] spawnObjects;

	private GameObject currentClone;

	private bool isSpawning = true;

	private void Start()
	{
		SpawnRandomClone();
	}

	private void OnDisable()
	{
		isSpawning = false;
		if (currentClone != null)
		{
			Object.Destroy(currentClone);
		}
	}

	public void OnCloneDestroyed()
	{
		if (isSpawning)
		{
			SpawnRandomClone();
		}
	}

	private void SpawnRandomClone()
	{
		if (isSpawning)
		{
			GameObject[] array = spawnObjects;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(value: false);
			}
			int num = Random.Range(0, spawnObjects.Length);
			GameObject gameObject2 = spawnObjects[num];
			Vector3 position = gameObject2.transform.position;
			Quaternion rotation = gameObject2.transform.rotation;
			GameObject gameObject3 = Object.Instantiate(gameObject2, position, rotation);
			gameObject3.SetActive(value: true);
			DestroyableObject destroyableObject = gameObject3.AddComponent<DestroyableObject>();
			destroyableObject.spawnManager = this;
			currentClone = gameObject3;
		}
	}
}
