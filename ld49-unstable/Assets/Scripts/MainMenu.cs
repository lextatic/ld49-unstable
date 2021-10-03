using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private Image _fadePanel;

	void Update()
	{
		if (Input.anyKeyDown)
		{
			StartCoroutine(LoadGame());
		}
	}

	private IEnumerator LoadGame()
	{
		_fadePanel.DOFade(1, 0.3f).SetEase(Ease.OutSine);

		yield return new WaitForSecondsRealtime(0.3f);

		SceneManager.LoadScene(1);
	}
}
