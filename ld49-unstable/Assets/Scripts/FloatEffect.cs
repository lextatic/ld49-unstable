using DG.Tweening;
using UnityEngine;

public class FloatEffect : MonoBehaviour
{
	public float FloatAmmount;

	public float FloatDuration;

	// Start is called before the first frame update
	void Start()
	{
		transform.DOMoveY(transform.position.y + FloatAmmount, FloatDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
	}
}
