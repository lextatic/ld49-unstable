using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Mute : MonoBehaviour
{
	[SerializeField]
	private AudioMixer _audioMixer;

	private void Start()
	{
		var volume = 0f;
		_audioMixer.GetFloat("musicVolume", out volume);

		if (volume == -80)
		{
			GetComponent<Toggle>().isOn = true;
		}
	}

	public void ToggleMute(bool value)
	{
		_audioMixer.SetFloat("musicVolume", value ? -80 : 0);
		_audioMixer.SetFloat("sfxVolume", value ? -80 : 0);
	}
}
