using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
	public Action<Node> OnNodeCreated;

	public Action OnOutOfAmmo;

	private List<Transform> _nodesList = new List<Transform>();

	[SerializeField]
	private GameObject _nodePrefab;

	[SerializeField]
	private float _maxConnectionDistance = 4f;
	[SerializeField]
	private float _minConnectionDistance = 0.5f;

	[SerializeField]
	private SpriteRenderer _pointer;
	[SerializeField]
	private LineRenderer _rod1;
	[SerializeField]
	private LineRenderer _rod2;
	[SerializeField]
	private float _cursorDistanceFade;
	[SerializeField]
	private TextMeshProUGUI _ammoLabel;
	[SerializeField]
	private SimpleAudioEvent _newPieceSound;

	private int _ammunition = 0;

	public Texture2D cursor;

	private void Start()
	{
		var starterNodes = FindObjectsOfType<Node>();

		foreach (Node node in starterNodes)
		{
			node.OnNodeDestroyed += NodeDestroyed;
			_nodesList.Add(node.transform);

			OnNodeCreated?.Invoke(node);
		}

		Cursor.visible = false;
		Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
	}

	void Update()
	{
		(Transform node, float distance) ClosestNode1 = (null, Mathf.Infinity);
		(Transform node, float distance) ClosestNode2 = (null, Mathf.Infinity);

		var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

		foreach (Transform node in _nodesList)
		{
			var distance = Vector3.Distance(point, node.transform.position);

			if (distance < ClosestNode1.distance)
			{
				if (ClosestNode1.distance < ClosestNode2.distance)
				{
					UpdateClosestBodyAndDistance(ref ClosestNode2, distance, node);
				}
				else
				{
					UpdateClosestBodyAndDistance(ref ClosestNode1, distance, node);
				}
			}
			else if (distance < ClosestNode2.distance)
			{
				UpdateClosestBodyAndDistance(ref ClosestNode2, distance, node);
			}
		}

		_pointer.transform.position = point;

		var pointerAlpha = 0.5f;

		if ((ClosestNode1.distance + ClosestNode2.distance) / 2 > _cursorDistanceFade)
		{
			pointerAlpha = Mathf.Max(0.5f - (((ClosestNode1.distance + ClosestNode2.distance) / 2) - _cursorDistanceFade) / (_cursorDistanceFade * 0.5f) * 0.5f, 0f);

		}

		UpdateRod(_rod1, ClosestNode1.node, ClosestNode1.distance, pointerAlpha);
		UpdateRod(_rod2, ClosestNode2.node, ClosestNode2.distance, pointerAlpha);

		var gbColorComponent = (ClosestNode1.distance > _maxConnectionDistance || ClosestNode1.distance < _minConnectionDistance ||
			ClosestNode2.distance > _maxConnectionDistance || ClosestNode2.distance < _minConnectionDistance) ? 0 : 1;

		_pointer.color = new Color(1, gbColorComponent, gbColorComponent, 0.5f);

		if (Input.GetMouseButtonDown(0))
		{

			if (_ammunition > 0 &&
				//(ClosestNode1.node != null && ClosestNode2.node != null &&
				!(ClosestNode1.distance > _maxConnectionDistance || ClosestNode1.distance < _minConnectionDistance ||
					ClosestNode2.distance > _maxConnectionDistance || ClosestNode2.distance < _minConnectionDistance)
				)
			{
				_ammunition--;
				var nodeObject = Instantiate(_nodePrefab, point, Quaternion.identity).transform;
				var node = nodeObject.GetComponent<Node>();
				node.OnNodeDestroyed += NodeDestroyed;
				_nodesList.Add(nodeObject);

				OnNodeCreated?.Invoke(node);
				if (_ammunition <= 0)
				{
					StartCoroutine(CheckGameOver());
				}

				_ammoLabel.text = _ammunition.ToString();

				_newPieceSound.Play(node.GetComponent<AudioSource>());
			}
		}
	}

	private IEnumerator CheckGameOver()
	{
		yield return new WaitForSecondsRealtime(2f);

		if (_ammunition <= 0)
		{
			OnOutOfAmmo?.Invoke();
		}
	}

	public void AddAmmo(int ammoToAdd)
	{
		_ammunition += ammoToAdd;
		_ammoLabel.text = _ammunition.ToString();

		_ammoLabel.transform.DOScale(2f, 0.4f).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo);
	}

	private void UpdateRod(LineRenderer rod, Transform joint, float distance, float alpha)
	{
		if (joint != null)
		{
			rod.enabled = true;

			rod.SetPosition(1, transform.InverseTransformDirection(joint.position - _pointer.transform.position));

			var gbColorComponent = (distance > _maxConnectionDistance || distance < _minConnectionDistance) ? 0 : 1;

			rod.startColor = new Color(1, gbColorComponent, gbColorComponent, alpha);
			rod.endColor = new Color(1, gbColorComponent, gbColorComponent, alpha);
		}
	}

	private void NodeDestroyed(Node nodeDestroyed)
	{
		nodeDestroyed.OnNodeDestroyed -= NodeDestroyed;
		_nodesList.Remove(nodeDestroyed.transform);
	}

	private void UpdateClosestBodyAndDistance(ref (Transform node, float closestDistance) ClosestNode, float distance, Transform node)
	{
		ClosestNode.closestDistance = distance;
		ClosestNode.node = node;
	}
}
