using UnityEngine;
using UnityEngine.Audio;

public class Mute : MonoBehaviour
{
	[SerializeField]
	private AudioMixer _audioMixer;

	public void ToggleMute(bool value)
	{
		_audioMixer.SetFloat("musicVolume", value ? -80 : 0);
		_audioMixer.SetFloat("sfxVolume", value ? -80 : 0);
	}
}
