using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public float maxHealth = 100f;

	private float currentHealth;

	public GameObject explosionEffect;

	public Transform explosionPosition;

	public GameObject playerModel;

	public Transform cameraTarget;

	public float cameraMoveSpeed = 5f;

	public float deathDelay = 1f;

	public Camera mainCamera;

	public Camera secondaryCamera;

	public float explosionForce = 250f;

	public int numberOfExplosions = 50;

	private bool isDying = false;

	public Text healthText;

	public GameObject deadScreenCanvas;

	public GameObject[] gameObjectsToDisable;

	public AudioClip deathSound;

	private AudioSource audioSource;

	private void Start()
	{
		if (deadScreenCanvas != null)
		{
			deadScreenCanvas.SetActive(value: false);
		}
		currentHealth = maxHealth;
		UpdateHealthUI();
		if (secondaryCamera != null)
		{
			secondaryCamera.gameObject.SetActive(value: false);
		}
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
	}

	public void TakeDamage(float amount)
	{
		if (!isDying)
		{
			currentHealth -= amount;
			if (currentHealth <= 0f)
			{
				StartCoroutine(DieWithDelay());
			}
			UpdateHealthUI();
		}
	}

	public void IncreaseHealth(float amount)
	{
		currentHealth += amount;
		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
		UpdateHealthUI();
	}

	private IEnumerator DieWithDelay()
	{
		isDying = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		if (mainCamera != null && secondaryCamera != null)
		{
			mainCamera.gameObject.SetActive(value: false);
			secondaryCamera.gameObject.SetActive(value: true);
		}
		yield return new WaitForSeconds(deathDelay);
		playerModel.SetActive(value: false);
		for (int i = 0; i < numberOfExplosions; i++)
		{
			Vector3 explosionPosition = base.transform.position + Random.insideUnitSphere * 0.5f;
			GameObject explosion = Object.Instantiate(explosionEffect, explosionPosition, Quaternion.identity);
			Rigidbody rb = explosion.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.AddExplosionForce(explosionForce, explosionPosition, 1f);
			}
		}
		GameObject[] array = gameObjectsToDisable;
		foreach (GameObject go in array)
		{
			if (go != null)
			{
				go.SetActive(value: false);
			}
			else
			{
				Debug.LogWarning("One of the GameObjects in gameObjectsToDisable array is not assigned!");
			}
		}
		if (deadScreenCanvas != null)
		{
			deadScreenCanvas.SetActive(value: true);
		}
		Debug.Log("Player has died!");
	}

	private void UpdateHealthUI()
	{
		if (healthText != null)
		{
			healthText.text = "Canın: " + currentHealth;
		}
	}
}
