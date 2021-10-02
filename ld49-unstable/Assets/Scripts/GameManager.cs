using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private int _bonusIndex;

	private NodeCreator _nodeCreator;

	private float _breakThreshold;

	private int _breakCounts;

	private float _maxHeightAchieved;

	private List<Node> _allNodesList = new List<Node>();

	private float _highScore;

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
		_breakCounts = 0;
		_breakThreshold = 0;
		_maxHeightAchieved = 0;
		_nodeCreator.AddAmmo(_initialAmmo);
	}

	private void OnNodeCreated(Node newNode)
	{
		newNode.OnJointBrake += OnConnectionBreak;
		newNode.OnNodeDestroyed += OnNodeDestroyed;
		_allNodesList.Add(newNode);
	}

	private void OnOutOfAmmo()
	{
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
		_gameOverPanel.SetActive(true);
		Time.timeScale = 1f;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(0);
		}

		_breakThreshold = Mathf.Max(_breakThreshold - (_breakThresholdCooldown * Time.deltaTime), 0);

		if (_breakThreshold > 0)
		{
			Debug.Log($"BreakThreshold: {_breakThreshold}\nBreakCount: {_breakCounts}");
		}

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

		if (_maxHeightAchieved > _victoryHeight)
		{
			Debug.Log("Victoly!");
		}
	}
}
