using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
	public InputField inputField;

	public Text chatBox;

	public Button sendButton;

	private void Start()
	{
		sendButton.onClick.AddListener(OnSendButtonClicked);
		inputField.onEndEdit.AddListener(OnEndEdit);
		ChatManager.Instance.OnMessageReceived += DisplayMessage;
	}

	private void OnSendButtonClicked()
	{
		SendMessage();
	}

	private void OnEndEdit(string text)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			SendMessage();
		}
	}

	private void SendMessage()
	{
		string text = inputField.text;
		if (!string.IsNullOrEmpty(text))
		{
			Debug.Log("Sending message: " + text);
			ChatManager.Instance.SendMessage(text);
			inputField.text = string.Empty;
			inputField.ActivateInputField();
		}
	}

	public void DisplayMessage(string sender, string message)
	{
		Debug.Log("Displaying message from player: " + sender + " with message: " + message);
		if (message.StartsWith("!C fly"))
		{
			Debug.Log("Command typed from player: " + sender + " with message: " + message);
			PlayerControllerXX playerControllerXX = Object.FindFirstObjectByType<PlayerControllerXX>();
			if (playerControllerXX != null)
			{
				Debug.Log("Command runned from player: " + sender + " with message: " + message);
				playerControllerXX.EnableFlightMode();
			}
		}
		else if (message.StartsWith("!C speed"))
		{
			Debug.Log("Command typed from player: " + sender + " with message: " + message);
			SpeedBoost speedBoost = Object.FindFirstObjectByType<SpeedBoost>();
			if (speedBoost != null)
			{
				Debug.Log("Command runned from player: " + sender + " with message: " + message);
				speedBoost.EnableSpeedMode();
			}
		}
		else
		{
			Text text = chatBox;
			text.text = text.text + sender + ": " + message + "\n";
		}
	}

	private void OnDestroy()
	{
		ChatManager.Instance.OnMessageReceived -= DisplayMessage;
	}
}
