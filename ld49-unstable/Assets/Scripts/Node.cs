using System;
using UnityEngine;

public class Node : MonoBehaviour
{
	[SerializeField]
	private FixedJoint2D _joint1;
	[SerializeField]
	private FixedJoint2D _joint2;

	[SerializeField]
	private LineRenderer _rod1;
	[SerializeField]
	private LineRenderer _rod2;

	private static float _highestForce;
	private static float _highestTorque;

	public Action<Node> OnNodeDestroyed;

	// Start is called before the first frame update
	void Start()
	{
		var allNodes = FindObjectsOfType<Node>();

		(Rigidbody2D body, float distance) ClosestNode1 = (null, Mathf.Infinity);
		(Rigidbody2D body, float distance) ClosestNode2 = (null, Mathf.Infinity);

		foreach (Node node in allNodes)
		{
			if (node == this) continue;

			var distance = Vector3.Distance(this.transform.position, node.transform.position);
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

		ConnectJoint(_joint1, ClosestNode1.body, ClosestNode1.distance, _rod1);
		ConnectJoint(_joint2, ClosestNode2.body, ClosestNode2.distance, _rod2);
	}

	private void ConnectJoint(FixedJoint2D joint, Rigidbody2D body, float closestDistance, LineRenderer rod)
	{
		if (closestDistance != Mathf.Infinity)
		{
			joint.connectedBody = body;
			joint.enabled = true;
			rod.enabled = true;

			//var connectedNode = body.GetComponent<Node>();
			//connectedNode.OnNodeDestroyed += OnConnectedNodeDestroyed;
		}
	}

	private void UpdateClosestBodyAndDistance(ref (Rigidbody2D body, float distance) ClosestNode, float distance, Node node)
	{
		ClosestNode.distance = distance;
		ClosestNode.body = node.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (_joint1 != null)
		{
			if (_joint1.reactionForce.magnitude > _highestForce)
			{
				_highestForce = _joint1.reactionForce.magnitude;
			}
		}

		if (_joint2 != null)
		{
			if (_joint2.reactionForce.magnitude > _highestForce)
			{
				_highestForce = _joint2.reactionForce.magnitude;
			}
		}

		Debug.Log($"reactionForce: {_highestForce}");
	}

	void Update()
	{
		UpdateRod(_joint1, _rod1);
		UpdateRod(_joint2, _rod2);
	}

	private void UpdateRod(FixedJoint2D joint, LineRenderer rod)
	{
		if (joint != null && joint.connectedBody != null)
		{
			rod.SetPosition(1, transform.InverseTransformDirection(joint.connectedBody.transform.position - transform.position));

			var forceProportion = joint.reactionForce.magnitude / joint.breakForce;

			rod.startColor = new Color(1, 1 - forceProportion, 1 - forceProportion);
			rod.endColor = new Color(1, 1 - forceProportion, 1 - forceProportion);
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (joint == _joint1)
		{
			_rod1.enabled = false;

			if (_joint2 == null)
			{
				OnNodeDestroyed?.Invoke(this);
				Destroy(gameObject);
			}
		}
		else
		{
			_rod2.enabled = false;

			if (_joint1 == null)
			{
				OnNodeDestroyed?.Invoke(this);
				Destroy(gameObject);
			}
		}
	}

	//private void OnConnectedNodeDestroyed(Node connectedNode)
	//{
	//	if (_joint2 == null)
	//	{
	//		if (_rod1 != null)
	//		{
	//			_rod1.enabled = false;
	//		}

	//		OnNodeDestroyed?.Invoke(this);
	//		Destroy(gameObject);
	//	}
	//	else
	//	{
	//		if (_rod2 != null)
	//		{
	//			_rod2.enabled = false;
	//		}

	//		if (_joint1 == null)
	//		{
	//			OnNodeDestroyed?.Invoke(this);
	//			Destroy(gameObject);
	//		}
	//	}
	//}
}
