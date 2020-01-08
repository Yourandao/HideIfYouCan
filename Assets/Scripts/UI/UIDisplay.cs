using System;
using System.Collections.Generic;
using System.Linq;

using Scripts.Components;
using Scripts.Management.Game;
using Scripts.Management.Network;
using Scripts.PlayerScripts;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIDisplay : MonoBehaviour
{
	[SerializeField] private AudioClip _buttonSound;

	[SerializeField] private Button _resumeButton;
	[SerializeField] private Button _quitButton;

	[SerializeField] private GameObject _mainUI;
	[SerializeField] private GameObject _pauseUI;

	[SerializeField] private Text role;
	[SerializeField] private Text seekersCount;
	[SerializeField] private Text hidersCount;
	[SerializeField] private Text spectatorsCount;
	[SerializeField] private Text time;

	private Role _playerRole = default;

	private int _seekersCount = default;
	private int _hidersCount = default;
	private int _spectatorsCount = default;

	private List<Player> _players;

	private bool _isPaused = false;

	private AudioSource _buttonAudioSource;

	private void Start()
	{
		_buttonAudioSource = GetComponent<AudioSource>();

		_buttonAudioSource.clip = _buttonSound;
		_buttonAudioSource.volume = .5f;

		_resumeButton.onClick.AddListener(() => _buttonAudioSource.PlayOneShot(_buttonSound));
		_quitButton.onClick.AddListener(() => _buttonAudioSource.PlayOneShot(_buttonSound));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_isPaused)
			{
				Resume();
			}
			else
			{
				ShowMenu();
			}
		}

		UpdateUiState();
	}

	private void UpdateUiState()
	{
		_players = ServerManager.GetAllPlayers().ToList();

		_seekersCount = _players.Count(p => p.role == Role.Seeker);
		_hidersCount = _players.Count(p => p.role == Role.Hider);
		_spectatorsCount = _players.Count(p => p.role == Role.Spectator);

		role.text = _playerRole.ToString();

		seekersCount.text = $"Seekers: {_seekersCount}";
		hidersCount.text = $"Hiders: {_hidersCount}";
		spectatorsCount.text = $"Spectators: {_spectatorsCount}";

		time.text = TimeSpan.FromSeconds((double)new decimal(GameManager.time)).ToString(@"hh\:mm\:ss");
	}

	private void ShowMenu()
	{
		_mainUI.SetActive(false);
		_pauseUI.SetActive(true);

		_isPaused = true;
	}

	public void Resume()
	{
		_mainUI.SetActive(true);
		_pauseUI.SetActive(false);

		_isPaused = false;
	}

	public void Quit()
	{
		//Quit logic
	}
}
