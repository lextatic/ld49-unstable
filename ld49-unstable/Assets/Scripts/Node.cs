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

	[SerializeField]
	private bool _starterNodes = false;

	[SerializeField]
	private SpriteRenderer _nodeSprite;

	[SerializeField]
	private Sprite[] _spriteVariations;

	[SerializeField]
	private SimpleAudioEvent _joinBreakSound;

	[SerializeField]
	private SimpleAudioEvent _collisionSound;

	private static float _highestForce;
	private static float _highestTorque;

	private float _collisionSoundCooldown;

	private AudioSource _audioSource;

	public Action<Node> OnNodeDestroyed;

	public Action OnJointBrake;

	// Start is called before the first frame update
	void Start()
	{
		_nodeSprite.sprite = _spriteVariations[UnityEngine.Random.Range(0, _spriteVariations.Length)];
		_nodeSprite.flipX = UnityEngine.Random.Range(0, 2) == 0;
		_nodeSprite.flipY = UnityEngine.Random.Range(0, 2) == 0;

		_audioSource = GetComponent<AudioSource>();

		if (_starterNodes) return;

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
	//void FixedUpdate()
	//{
	//	if (_joint1 != null)
	//	{
	//		if (_joint1.reactionForce.magnitude > _highestForce)
	//		{
	//			_highestForce = _joint1.reactionForce.magnitude;
	//		}
	//	}

	//	if (_joint2 != null)
	//	{
	//		if (_joint2.reactionForce.magnitude > _highestForce)
	//		{
	//			_highestForce = _joint2.reactionForce.magnitude;
	//		}
	//	}

	//	Debug.Log($"reactionForce: {_highestForce}");
	//}

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

			var inverseForceProportion = 1 - (joint.reactionForce.magnitude / joint.breakForce);

			rod.startColor = new Color(1, inverseForceProportion, inverseForceProportion);
			rod.endColor = new Color(1, inverseForceProportion, inverseForceProportion);
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		OnJointBrake?.Invoke();

		_joinBreakSound.Play(_audioSource);

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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (Time.time - _collisionSoundCooldown > 5f)
		{
			_collisionSound.Play(_audioSource);
			_collisionSoundCooldown = Time.time;
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
