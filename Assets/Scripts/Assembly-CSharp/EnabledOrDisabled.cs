using UnityEngine;

public class EnabledOrDisabled : MonoBehaviour
{
	public GameObject pic;

	public void Trigger()
	{
		if (!pic.activeInHierarchy)
		{
			pic.SetActive(value: true);
		}
		else
		{
			pic.SetActive(value: false);
		}
	}
}
