using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : MonoBehaviour
{
	public Slider sensitivitySlider;

	public Text sensitivityValueText;

	public PlayerMovement playerMovement;

	private void Start()
	{
		if (PlayerPrefs.HasKey("mouseSensitivity"))
		{
			playerMovement.mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
		}
		else
		{
			playerMovement.mouseSensitivity = 2000f;
		}
		sensitivitySlider.minValue = 0f;
		sensitivitySlider.maxValue = 2000f;
		sensitivitySlider.value = playerMovement.mouseSensitivity;
		UpdateSensitivityText();
		sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
	}

	private void OnSensitivityChanged(float value)
	{
		playerMovement.mouseSensitivity = value;
		PlayerPrefs.SetFloat("mouseSensitivity", value);
		UpdateSensitivityText();
	}

	private void UpdateSensitivityText()
	{
		sensitivityValueText.text = "Hassasiyet: " + playerMovement.mouseSensitivity.ToString("F0");
	}
}
