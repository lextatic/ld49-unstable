using UnityEngine;

public class Node : MonoBehaviour
{
	[SerializeField]
	private FixedJoint2D _joint1;
	[SerializeField]
	private FixedJoint2D _joint2;

	private static float _highestForce;
	private static float _highestTorque;

	// Start is called before the first frame update
	void Start()
	{
		var allNodes = FindObjectsOfType<Node>();

		(Rigidbody2D body, float closestDistance) ClosestNode1 = (null, Mathf.Infinity);
		(Rigidbody2D body, float closestDistance) ClosestNode2 = (null, Mathf.Infinity);

		foreach (Node node in allNodes)
		{
			if (node == this) continue;

			var distance = Vector3.Distance(this.transform.position, node.transform.position);
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

		ConnectOrDisableJoint(_joint1, ClosestNode1.body, ClosestNode1.closestDistance);
		ConnectOrDisableJoint(_joint2, ClosestNode2.body, ClosestNode2.closestDistance);
	}

	private void ConnectOrDisableJoint(FixedJoint2D joint, Rigidbody2D body, float closestDistance)
	{
		if (closestDistance != Mathf.Infinity)
		{
			joint.connectedBody = body;
			joint.enabled = true;
		}
	}

	private void UpdateClosestBodyAndDistance(ref (Rigidbody2D body, float closestDistance) ClosestNode1, float distance, Node node)
	{
		ClosestNode1.closestDistance = distance;
		ClosestNode1.body = node.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{
		if (_joint1 != null)
		{
			if (_joint1.reactionForce.magnitude > _highestForce)
			{
				_highestForce = _joint1.reactionForce.magnitude;
			}

			if (_joint1.reactionTorque > _highestTorque)
			{
				_highestTorque = _joint1.reactionTorque;
			}
		}

		if (_joint2 != null)
		{
			if (_joint2.reactionForce.magnitude > _highestForce)
			{
				_highestForce = _joint2.reactionForce.magnitude;
			}

			if (_joint2.reactionTorque > _highestTorque)
			{
				_highestTorque = _joint2.reactionTorque;
			}
		}

		Debug.Log($"reactionForce: {_highestForce}");
		Debug.Log($"reactionTorque: {_highestTorque}");
	}

	void OnDrawGizmos()
	{
		if (_joint1 != null && _joint1.connectedBody != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, _joint1.connectedBody.transform.position);
		}

		if (_joint2 != null && _joint2.connectedBody != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(transform.position, _joint2.connectedBody.transform.position);
		}
	}
}
