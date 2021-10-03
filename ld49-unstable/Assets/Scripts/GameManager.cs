using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct HeightAndAmmo
{
	public float Height;
	public int Ammo;
	public GameObject HeightAndAmmoLine;
}

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private float _maxBreakThreshold = 5f;
	[SerializeField]
	private float _breakThresholdCooldown = 1f;
	[SerializeField]
	private HeightBar _currentHeightBar;
	[SerializeField]
	private HeightBar _victoryHeightBar;
	[SerializeField]
	private HeightBar _hightScoreBar;
	[SerializeField]
	private float _victoryHeight;
	[SerializeField]
	private int _initialAmmo = 10;
	[SerializeField]
	private HeightAndAmmo[] AllBonuses;
	[SerializeField]
	private HeightBar _bonusHeightBarPrefab;
	[SerializeField]
	private GameObject _gameOverPanel;
	[SerializeField]
	private Image _fadePanel;

	private int _bonusIndex;

	private NodeCreator _nodeCreator;

	private float _breakThreshold;

	private float _maxHeightAchieved;

	private List<Node> _allNodesList = new List<Node>();

	private float _highScore;

	private bool _victoryAchieved;

	void Awake()
	{
		_nodeCreator = GetComponent<NodeCreator>();
		_nodeCreator.OnNodeCreated += OnNodeCreated;
		_nodeCreator.OnOutOfAmmo += OnOutOfAmmo;

		for (int i = 0; i < AllBonuses.Length; i++)
		{
			var newBar = Instantiate(_bonusHeightBarPrefab, new Vector3(0, AllBonuses[i].Height, 0), Quaternion.identity);
			newBar.SetText($"+{AllBonuses[i].Ammo}");
			AllBonuses[i].HeightAndAmmoLine = newBar.gameObject;
		}

		_victoryHeightBar.transform.position = new Vector3(0, _victoryHeight, 0);
		_victoryHeightBar.SetText("Victory!");

		_highScore = PlayerPrefs.GetFloat("highScore", 0);
		_hightScoreBar.transform.position = new Vector3(0, _highScore, 0);
		_hightScoreBar.SetText($"High Score: {_highScore:0.##}");
	}

	private void Start()
	{
		_bonusIndex = 0;
		_breakThreshold = 0;
		_maxHeightAchieved = 0;
		_victoryAchieved = false;
		_nodeCreator.AddAmmo(_initialAmmo);
		Time.timeScale = 1f;

		_fadePanel.DOFade(0, 0.3f).SetEase(Ease.OutSine);
	}

	private void OnNodeCreated(Node newNode)
	{
		newNode.OnJointBrake += OnConnectionBreak;
		newNode.OnNodeDestroyed += OnNodeDestroyed;
		_allNodesList.Add(newNode);
	}

	private void OnOutOfAmmo()
	{
		if (_victoryAchieved) return;
		_gameOverPanel.SetActive(true);
	}

	private void OnNodeDestroyed(Node nodeDestroyed)
	{
		_allNodesList.Remove(nodeDestroyed);
	}

	private void OnConnectionBreak()
	{
		_breakThreshold++;

		if (_breakThreshold > _maxBreakThreshold)
		{
			Time.timeScale = 0.2f;
			StartCoroutine(GameOver());

		}
	}

	private IEnumerator GameOver()
	{
		yield return new WaitForSecondsRealtime(2f);
		Time.timeScale = 1f;

		if (!_victoryAchieved)
		{
			_gameOverPanel.SetActive(true);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			StartCoroutine(LoadGame());
		}

		_breakThreshold = Mathf.Max(_breakThreshold - (_breakThresholdCooldown * Time.deltaTime), 0);

		//if (_breakThreshold > 0)
		//{
		//	Debug.Log($"BreakThreshold: {_breakThreshold}\nBreakCount: {_breakCounts}");
		//}

		foreach (Node node in _allNodesList)
		{
			if (node.transform.position.y > _maxHeightAchieved)
			{
				_maxHeightAchieved = node.transform.position.y;

				_currentHeightBar.transform.position = new Vector3(0, _maxHeightAchieved, 0);

				_currentHeightBar.SetText($"{_maxHeightAchieved:0.##}");

				if (_maxHeightAchieved > _highScore)
				{
					_highScore = _maxHeightAchieved;
					PlayerPrefs.SetFloat("highScore", _highScore);
					_hightScoreBar.transform.position = new Vector3(0, _highScore, 0);
					_hightScoreBar.SetText($"High Score: {_highScore:0.##}");
				}
			}
		}

		if (_bonusIndex != -1 && _maxHeightAchieved > AllBonuses[_bonusIndex].Height)
		{
			_nodeCreator.AddAmmo(AllBonuses[_bonusIndex].Ammo);

			Destroy(AllBonuses[_bonusIndex].HeightAndAmmoLine);

			if (_bonusIndex < AllBonuses.Length - 1)
			{
				_bonusIndex++;
			}
			else
			{
				_bonusIndex = -1;
			}
		}

		if (!_victoryAchieved && _maxHeightAchieved > _victoryHeight)
		{
			_victoryAchieved = true;
			//Debug.Log("Victoly!");
		}
	}

	private IEnumerator LoadGame()
	{
		_fadePanel.DOFade(1, 0.3f).SetEase(Ease.OutSine);

		yield return new WaitForSecondsRealtime(0.3f);

		SceneManager.LoadScene(1);
	}
}
