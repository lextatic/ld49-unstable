using System;
using System.Collections.Generic;
using UnityEngine;

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
	private Transform _heightBar;
	[SerializeField]
	private int _initialAmmo = 10;
	[SerializeField]
	private HeightAndAmmo[] AllBonuses;
	[SerializeField]
	private GameObject _bonusHeightBarPrefab;

	private int _bonusIndex;

	private NodeCreator _nodeCreator;

	private float _breakThreshold;

	private int _breakCounts;

	private float _maxHeightAchieved;

	private List<Node> _allNodesList = new List<Node>();

	void Awake()
	{
		_nodeCreator = GetComponent<NodeCreator>();
		_nodeCreator.OnNodeCreated += OnNodeCreated;

		for (int i = 0; i < AllBonuses.Length; i++)
		{
			AllBonuses[i].HeightAndAmmoLine = Instantiate(_bonusHeightBarPrefab, new Vector3(0, AllBonuses[i].Height, 0), Quaternion.identity);
		}
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

	private void OnNodeDestroyed(Node nodeDestroyed)
	{
		_allNodesList.Remove(nodeDestroyed);
	}

	private void OnConnectionBreak()
	{
		_breakThreshold++;

		if (_breakThreshold > _maxBreakThreshold)
		{
			Time.timeScale = 0.5f;
			Debug.Log("Derrota mermão");
		}
	}

	void Update()
	{
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

				_heightBar.position = new Vector3(0, _maxHeightAchieved, 0);
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
	}
}
