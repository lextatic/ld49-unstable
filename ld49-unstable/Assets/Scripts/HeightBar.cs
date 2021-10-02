using TMPro;
using UnityEngine;

public class HeightBar : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro _extraAmmoText;

	public void SetText(string text)
	{
		_extraAmmoText.text = text;
	}
}
