using TMPro;
using UnityEngine;

public class BonusHeightBar : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro _extraAmmoText;

	public void SetAmmoText(int ammo)
	{
		_extraAmmoText.text = $"+{ammo}";
	}
}
