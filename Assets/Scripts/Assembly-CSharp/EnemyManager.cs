using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
	public enum DisplayMode
	{
		RemainingEnemies = 0,
		KilledEnemies = 1
	}

	public DisplayMode displayMode;

	public Text enemyCountText;

	private int totalEnemies;

	private int killedEnemies;

	public AudioClip DownSound;

	private AudioSource audioSource;

	public PlayerHealth playerHealth;

	public float healthIncreaseAmount = 10f;

	private void Start()
	{
		EnemyAI[] array = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
		totalEnemies = array.Length;
		killedEnemies = 0;
		UpdateEnemyCountUI();
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
		audioSource.clip = DownSound;
		if (playerHealth == null)
		{
			Debug.LogWarning("PlayerHealth is not assigned!");
		}
	}

	public void EnemyDied()
	{
		totalEnemies--;
		killedEnemies++;
		UpdateEnemyCountUI();
		audioSource.Play();
		if (playerHealth != null)
		{
			playerHealth.IncreaseHealth(healthIncreaseAmount);
		}
	}

	private void UpdateEnemyCountUI()
	{
		if (enemyCountText != null)
		{
			if (displayMode == DisplayMode.RemainingEnemies)
			{
				enemyCountText.text = "Kalan Düşman: " + totalEnemies;
			}
			else if (displayMode == DisplayMode.KilledEnemies)
			{
				enemyCountText.text = "Öldürülen Düşman: " + killedEnemies;
			}
		}
		else
		{
			Debug.LogWarning("Enemy Count Text is not assigned!");
		}
	}
}
