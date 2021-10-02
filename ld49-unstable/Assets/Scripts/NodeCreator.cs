using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
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

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		(Transform node, float distance) ClosestNode1 = (null, Mathf.Infinity);
		(Transform node, float distance) ClosestNode2 = (null, Mathf.Infinity);

		var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

		foreach (Transform node in _nodesList)
		{
			var distance = Vector3.Distance(point, node.transform.position);

			//if (distance > _maxConnectionDistance || distance < _minConnectionDistance) continue;


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

		UpdateRod(_rod1, ClosestNode1.node, ClosestNode1.distance);
		UpdateRod(_rod2, ClosestNode2.node, ClosestNode2.distance);

		var gbColorComponent = (ClosestNode1.distance > _maxConnectionDistance || ClosestNode1.distance < _minConnectionDistance ||
			ClosestNode2.distance > _maxConnectionDistance || ClosestNode2.distance < _minConnectionDistance) ? 0 : 1;

		_pointer.color = new Color(1, gbColorComponent, gbColorComponent, 0.5f);

		if (Input.GetMouseButtonDown(0))
		{
			if ((ClosestNode1.node != null && ClosestNode2.node != null &&
				!(ClosestNode1.distance > _maxConnectionDistance || ClosestNode1.distance < _minConnectionDistance ||
					ClosestNode2.distance > _maxConnectionDistance || ClosestNode2.distance < _minConnectionDistance)
				) || _nodesList.Count < 2)
			{
				var node = Instantiate(_nodePrefab, point, Quaternion.identity).transform;
				node.GetComponent<Node>().OnNodeDestroyed += NodeDestroyed;
				_nodesList.Add(node);
			}
		}
	}

	private void UpdateRod(LineRenderer rod, Transform joint, float distance)
	{
		if (joint != null)
		{
			rod.enabled = true;

			rod.SetPosition(1, transform.InverseTransformDirection(joint.position - _pointer.transform.position));

			var gbColorComponent = (distance > _maxConnectionDistance || distance < _minConnectionDistance) ? 0 : 1;

			rod.startColor = new Color(1, gbColorComponent, gbColorComponent, 0.5f);
			rod.endColor = new Color(1, gbColorComponent, gbColorComponent, 0.5f);
		}
	}

	private void NodeDestroyed(Node nodeDestroyed)
	{
		nodeDestroyed.OnNodeDestroyed -= NodeDestroyed;
		_nodesList.Remove(nodeDestroyed.transform);
	}

	//private void OnDrawGizmos()
	//{
	//	(Transform node, float distance) ClosestNode1 = (null, Mathf.Infinity);
	//	(Transform node, float distance) ClosestNode2 = (null, Mathf.Infinity);

	//	var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

	//	foreach (Transform node in _nodesList)
	//	{
	//		var distance = Vector3.Distance(point, node.transform.position);
	//		if (distance < ClosestNode1.distance)
	//		{
	//			if (ClosestNode1.distance < ClosestNode2.distance)
	//			{
	//				UpdateClosestBodyAndDistance(ref ClosestNode2, distance, node);
	//			}
	//			else
	//			{
	//				UpdateClosestBodyAndDistance(ref ClosestNode1, distance, node);
	//			}
	//		}
	//		else if (distance < ClosestNode2.distance)
	//		{
	//			UpdateClosestBodyAndDistance(ref ClosestNode2, distance, node);
	//		}
	//	}

	//	if (ClosestNode1.node != null)
	//	{
	//		if (ClosestNode1.distance > 4)
	//		{
	//			Gizmos.color = Color.red;
	//		}
	//		else
	//		{
	//			Gizmos.color = Color.yellow;
	//		}

	//		Gizmos.DrawLine(point, ClosestNode1.node.position);
	//	}

	//	if (ClosestNode2.node != null)
	//	{
	//		if (ClosestNode2.distance > 4)
	//		{
	//			Gizmos.color = Color.red;
	//		}
	//		else
	//		{
	//			Gizmos.color = Color.yellow;
	//		}

	//		Gizmos.DrawLine(point, ClosestNode2.node.position);
	//	}
	//}

	private void UpdateClosestBodyAndDistance(ref (Transform node, float closestDistance) ClosestNode, float distance, Transform node)
	{
		ClosestNode.closestDistance = distance;
		ClosestNode.node = node;
	}
}