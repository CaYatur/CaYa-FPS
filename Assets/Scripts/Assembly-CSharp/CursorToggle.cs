using UnityEngine;

public class CursorToggle : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}
}
