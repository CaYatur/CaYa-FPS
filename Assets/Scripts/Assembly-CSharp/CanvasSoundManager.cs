using UnityEngine;

public class CanvasSoundManager : MonoBehaviour
{
	public Canvas canvasToShow;

	public AudioClip soundToPlay;

	private AudioSource audioSource;

	private bool hasPlayed = false;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (canvasToShow != null && canvasToShow.gameObject.activeSelf && soundToPlay != null && !hasPlayed)
		{
			PlaySound();
		}
	}

	private void PlaySound()
	{
		audioSource.PlayOneShot(soundToPlay);
		hasPlayed = true;
	}
}
