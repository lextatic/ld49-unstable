using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
	private List<Transform> _nodesList = new List<Transform>();

	[SerializeField]
	private GameObject _nodePrefab;

	private float _maxConnectionDistance = 4f;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			(Transform node, float closestDistance) ClosestNode1 = (null, Mathf.Infinity);
			(Transform node, float closestDistance) ClosestNode2 = (null, Mathf.Infinity);

			var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

			foreach (Transform node in _nodesList)
			{
				var distance = Vector3.Distance(point, node.transform.position);

				if (distance > _maxConnectionDistance) continue;

				if (distance < ClosestNode1.closestDistance)
				{
					if (ClosestNode1.closestDistance < ClosestNode2.closestDistance)
					{
						UpdateClosestBodyAndDistance(ref ClosestNode2, distance, node);
					}
					else
					{
						UpdateClosestBodyAndDistance(ref ClosestNode1, distance, node);
					}
				}
				else if (distance < ClosestNode2.closestDistance)
				{
					UpdateClosestBodyAndDistance(ref ClosestNode2, distance, node);
				}
			}

			//var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

			if ((ClosestNode1.node != null && ClosestNode2.node != null) || _nodesList.Count < 2)
			{
				_nodesList.Add(Instantiate(_nodePrefab, point, Quaternion.identity).transform);
			}
		}
	}

	private void OnDrawGizmos()
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

		if (ClosestNode1.node != null)
		{
			if (ClosestNode1.distance > 4)
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.yellow;
			}

			Gizmos.DrawLine(point, ClosestNode1.node.position);
		}

		if (ClosestNode2.node != null)
		{
			if (ClosestNode2.distance > 4)
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.yellow;
			}

			Gizmos.DrawLine(point, ClosestNode2.node.position);
		}
	}

	private void UpdateClosestBodyAndDistance(ref (Transform node, float closestDistance) ClosestNode, float distance, Transform node)
	{
		ClosestNode.closestDistance = distance;
		ClosestNode.node = node;
	}
}
