using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Sections")]
	[SerializeField] private GameObject mainUI;

	[SerializeField] private GameObject settingsUI;

	[FormerlySerializedAs("_buttonAudioSource")]
	[Header("Audio")]
	[SerializeField] private AudioSource _menuAudioSource;

	[SerializeField] private AudioMixer _audioMixer;

	[Header("UI Components")]
	[SerializeField] private Button[] buttons;
	[SerializeField] private AudioClip _buttonSound;

	[Space]
	[SerializeField] private Dropdown resolutionDropdown;
	[SerializeField] private AudioClip dropdownSelectSound;

	[Space]
	[SerializeField] private Slider volumeSlider;
	[SerializeField] private AudioClip sliderChangeSound;

	[Space]
	[SerializeField] private Toggle fullscreenToggle;
	[SerializeField] private AudioClip toggleSound;

	private bool isTuning;

	private List<Resolution> resolutions;

	// Start is called before the first frame update
	private void Start()
	{
		resolutions = Screen.resolutions.ToList();

		resolutionDropdown.ClearOptions();
		resolutionDropdown.AddOptions(resolutions.Select(res => res.width + " x " + res.height).ToList());

		resolutionDropdown.value = resolutions.IndexOf(Screen.currentResolution);
		resolutionDropdown.RefreshShownValue();

		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].onClick.AddListener(() => _menuAudioSource.PlayOneShot(_buttonSound));
		}

		resolutionDropdown.onValueChanged.AddListener(_ => _menuAudioSource.PlayOneShot(dropdownSelectSound));
		volumeSlider.onValueChanged.AddListener(_ => _menuAudioSource.PlayOneShot(sliderChangeSound));
		fullscreenToggle.onValueChanged.AddListener(_ => _menuAudioSource.PlayOneShot(toggleSound));
	}

	#region Settings interaction

	public void SetVolume(float volume)
	{
		_audioMixer.SetFloat("MasterVolume", volume);
	}

	public void SetFullscreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}

	public void SetResolution(int resolutionIndex)
	{
		var selectedResolution = resolutions[resolutionIndex];
		Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
	}

	#endregion

	public void StartGame()
	{

	}

	public void ToggleSettings()
	{
		isTuning = !isTuning;

		mainUI.SetActive(!isTuning);
		settingsUI.SetActive(isTuning);
	}

	public void QuitGame()
	{

	}
}
