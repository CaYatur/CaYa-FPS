using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnManager : MonoBehaviour
{
	public Transform[] spawnPoints;

	private List<Transform> usedSpawnPoints = new List<Transform>();

	public Transform GetRandomSpawnPoint()
	{
		if (spawnPoints == null || spawnPoints.Length == 0)
		{
			Debug.LogError("No spawn points set in ObjectSpawnManager.");
			return null;
		}
		List<Transform> list = new List<Transform>();
		Transform[] array = spawnPoints;
		foreach (Transform transform in array)
		{
			if (transform != null && !usedSpawnPoints.Contains(transform))
			{
				list.Add(transform);
			}
		}
		if (list.Count == 0)
		{
			list.AddRange(spawnPoints);
			usedSpawnPoints.Clear();
		}
		int index = Random.Range(0, list.Count);
		Transform transform2 = list[index];
		usedSpawnPoints.Add(transform2);
		return transform2;
	}
}
