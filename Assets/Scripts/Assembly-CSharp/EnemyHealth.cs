using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	public float maxHealth = 100f;

	private float currentHealth;

	public GameObject explosionEffect;

	public int numberOfExplosions = 20;

	public float explosionForce = 250f;

	public AudioClip deathSound;

	private EnemyManager enemyManager;

	private AudioSource audioSource;

	private void Start()
	{
		currentHealth = maxHealth;
		enemyManager = Object.FindFirstObjectByType<EnemyManager>();
		if (enemyManager == null)
		{
			Debug.LogWarning("EnemyManager not found in the scene!");
		}
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
	}

	public void TakeDamage(float amount)
	{
		currentHealth -= amount;
		if (currentHealth <= 0f)
		{
			Die();
		}
	}

	private void Die()
	{
		if (enemyManager != null)
		{
			enemyManager.EnemyDied();
		}
		if (explosionEffect != null)
		{
			for (int i = 0; i < numberOfExplosions; i++)
			{
				Vector3 vector = base.transform.position + Random.insideUnitSphere * 0.5f;
				GameObject gameObject = Object.Instantiate(explosionEffect, vector, Quaternion.identity);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(explosionForce, vector, 1f);
				}
				if (numberOfExplosions > 50)
				{
					Object.Destroy(gameObject, 20f);
				}
			}
		}
		else
		{
			Debug.Log("Patlama efekti bulunamadı!");
		}
		Debug.Log(base.gameObject.name + " has died!");
		Object.Destroy(base.gameObject);
	}
}
