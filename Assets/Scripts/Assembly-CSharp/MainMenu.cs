using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void PlayNormalGame()
	{
		Debug.Log("Clicked NormalGame");
		SceneManager.LoadScene("NormalGame");
	}

	public void PlayDeadWar()
	{
		Debug.Log("Clicked DeadWar");
		SceneManager.LoadScene("DeadWar");
	}

	public void PlayTraining()
	{
		Debug.Log("Clicked Trainer");
		SceneManager.LoadScene("Trainer");
	}

	public void PlayMultiplayer()
	{
		Debug.Log("Clicked Multiplayer");
		SceneManager.LoadScene("Multiplayer");
	}

	public void PlayMainMenu()
	{
		Debug.Log("Clicked MainMenu");
		SceneManager.LoadScene("MainMenu");
	}
}
