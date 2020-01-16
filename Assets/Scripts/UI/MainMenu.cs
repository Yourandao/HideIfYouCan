using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Sections")]
	[SerializeField] private GameObject mainUI;

	[SerializeField] private GameObject settingsUI;

	[Header("Audio")]
	[SerializeField] private AudioSource _buttonAudioSource;

	[SerializeField] private AudioClip _buttonSound;

	[Header("UI Components")]
	[SerializeField] private Button[] buttons;

	private bool isTuning;

	// Start is called before the first frame update
	private void Start()
	{
		_buttonAudioSource.clip = _buttonSound;

		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].onClick.AddListener(() => _buttonAudioSource.PlayOneShot(_buttonSound));
		}
	}

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
