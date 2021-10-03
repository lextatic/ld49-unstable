using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	void Update()
	{
		if (Input.anyKeyDown)
		{
			SceneManager.LoadScene(1);
		}
	}
}
