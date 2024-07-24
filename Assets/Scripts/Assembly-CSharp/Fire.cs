using UnityEngine;

public class Fire : MonoBehaviour
{
	public float damage = 10f;

	public float range = 100f;

	public float fireRate = 15f;

	public float impactForce = 30f;

	public float fireAndWait = 0.5f;

	private float nextTimeToFire = 0f;

	public Transform bulletSpawn;

	public GameObject bulletPrefab;

	public Camera fpsCam;

	public GameObject impactEffect;

	public float shootForce = 10f;

	public AudioClip shootSound;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
		audioSource.clip = shootSound;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
		{
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot();
			nextTimeToFire += fireAndWait;
		}
	}

	private void Shoot()
	{
		Vector3 position = ((bulletSpawn != null) ? bulletSpawn.position : (fpsCam.transform.position + fpsCam.transform.forward * 2f));
		if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out var hitInfo, range))
		{
			if (hitInfo.transform.CompareTag("Enemy"))
			{
				EnemyHealth component = hitInfo.transform.GetComponent<EnemyHealth>();
				if (component != null)
				{
					component.TakeDamage(damage);
				}
			}
			GameObject obj = Object.Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
			Object.Destroy(obj, 2f);
			if (hitInfo.rigidbody != null)
			{
				hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce);
			}
		}
		GameObject gameObject = Object.Instantiate(bulletPrefab, position, Quaternion.LookRotation(fpsCam.transform.forward));
		gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 50f;
		if (shootSound != null && audioSource != null)
		{
			Vector3 normalized = (Camera.main.transform.position - bulletSpawn.position).normalized;
			audioSource.spatialBlend = 1f;
			audioSource.PlayOneShot(shootSound, 1f);
		}
		Object.Destroy(gameObject, 5f);
	}
}
