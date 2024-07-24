using UnityEngine;

public class CustomShooter : MonoBehaviour
{
	public GameObject bulletPrefab;

	public Transform firePoint;

	public AudioClip fireSound;

	public float bulletSpeed = 30f;

	public int damageAmount = 10;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
	}

	public void Shoot()
	{
		if (fireSound != null && audioSource != null && firePoint != null)
		{
			audioSource.spatialBlend = 1f;
			audioSource.PlayOneShot(fireSound, 1f);
		}
		else
		{
			Debug.LogWarning("Null reference detected in Shoot method. Check fireSound, audioSource, and firePoint.");
		}
		GameObject gameObject = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		Vector3 normalized = (Camera.main.transform.position - firePoint.position).normalized;
		component.velocity = normalized * bulletSpeed;
		Object.Destroy(gameObject, 5f);
		if (Physics.Raycast(firePoint.position, normalized, out var hitInfo, float.PositiveInfinity) && hitInfo.transform.CompareTag("Player"))
		{
			PlayerHealth component2 = hitInfo.transform.GetComponent<PlayerHealth>();
			if (component2 != null)
			{
				component2.TakeDamage(damageAmount);
			}
		}
	}
}
