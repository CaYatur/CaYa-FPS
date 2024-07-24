using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
	public Canvas canvasToShowOnKey;

	public Canvas[] canvasesToHideOnKey;

	public Canvas[] canvasesToHideOnStart;

	public KeyCode toggleKey = KeyCode.Escape;

	public bool lockCursorOnShow = false;

	public bool lockCursorOnHide = false;

	public bool pauseGameOnShow = false;

	private bool isGamePaused;

	private void Start()
	{
		Canvas[] array = canvasesToHideOnStart;
		foreach (Canvas canvas in array)
		{
			canvas.gameObject.SetActive(value: false);
		}
		SetCursorState(lockCursorOnHide);
	}

	private void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			ToggleCanvasVisibility();
		}
	}

	private void ToggleCanvasVisibility()
	{
		bool flag = !canvasToShowOnKey.gameObject.activeSelf;
		canvasToShowOnKey.gameObject.SetActive(flag);
		Canvas[] array = canvasesToHideOnKey;
		foreach (Canvas canvas in array)
		{
			canvas.gameObject.SetActive(!flag);
		}
		SetCursorState(flag ? lockCursorOnShow : lockCursorOnHide);
		if (pauseGameOnShow)
		{
			ToggleGamePause(flag);
		}
	}

	private void ToggleGamePause(bool pause)
	{
		isGamePaused = pause;
		Time.timeScale = (pause ? 0f : 1f);
	}

	private void SetCursorState(bool lockCursor)
	{
		Cursor.lockState = (lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
		Cursor.visible = !lockCursor;
	}
}
