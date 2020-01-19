using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Sections")]
	[SerializeField] private GameObject mainUI;

	[SerializeField] private GameObject settingsUI;

	[FormerlySerializedAs("_buttonAudioSource")]
	[Header("Audio")]
	[SerializeField] private AudioSource menuAudioSource;

	[SerializeField] private AudioMixer audioMixer;

	[Header("UI Components")]
	[SerializeField] private Button[] buttons;
	[SerializeField] private AudioClip buttonSound;
	[SerializeField] private AudioClip hoverSound;

	[Space]
	[SerializeField] private Dropdown resolutionDropdown;
	[SerializeField] private AudioClip dropdownSelectSound;
	[SerializeField] private AudioClip dropdownHoverSound;

	[Space]
	[SerializeField] private Slider volumeSlider;
	[SerializeField] private AudioClip sliderChangeSound;
	[SerializeField] private AudioClip sliderHoverSound;

	[Space]
	[SerializeField] private Toggle fullscreenToggle;
	[SerializeField] private AudioClip toggleSound;

	private bool isTuning;

	private List<Resolution> resolutions;

	private void Start()
	{
		resolutions = Screen.resolutions.ToList();

		resolutionDropdown.ClearOptions();
		resolutionDropdown.AddOptions(resolutions.Select(res => res.width + " x " + res.height).ToList());

		resolutionDropdown.value = resolutions.IndexOf(Screen.currentResolution);
		resolutionDropdown.RefreshShownValue();

		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].onClick.AddListener(() => menuAudioSource.PlayOneShot(buttonSound));
		}

		resolutionDropdown.onValueChanged.AddListener(_ => menuAudioSource.PlayOneShot(dropdownSelectSound));
		volumeSlider.onValueChanged.AddListener(_ => menuAudioSource.PlayOneShot(sliderChangeSound));
		fullscreenToggle.onValueChanged.AddListener(_ => menuAudioSource.PlayOneShot(toggleSound));
	}

	#region Settings interaction

	public void SetVolume(float volume)
	{
		audioMixer.SetFloat("MasterVolume", volume);
		menuAudioSource.volume = volume / volumeSlider.maxValue;
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

	#region Button interaction

	public void OnButtonClick() => menuAudioSource.PlayOneShot(buttonSound);

	public void OnButtonHover() => menuAudioSource.PlayOneShot(hoverSound);

	#endregion

	#region Slider interaction

	public void OnSliderHover() => menuAudioSource.PlayOneShot(sliderHoverSound);

	#endregion

	#region Dropdown interaction

	public void OnDropDownHover() => menuAudioSource.PlayOneShot(dropdownHoverSound);

	#endregion
}
