using System;
using System.Linq;

using Scripts.Components;
using Scripts.Management.Network;
using Scripts.PlayerScripts;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UserInterface : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameObject pauseUI;

    [Header("Audio")]
    [SerializeField] private AudioSource _buttonAudioSource;

    [SerializeField] private AudioClip _buttonSound;

    [Header("UI Components")]
    [SerializeField] private Button resumeButton;

    [SerializeField] private Button quitButton;

    [SerializeField] private Text gameState;
    [SerializeField] private Text time;
    [SerializeField] private Text role;
    [SerializeField] private Text seekers;
    [SerializeField] private Text hiders;
    [SerializeField] private Text spectators;

    [HideInInspector] public Player player;

    private bool paused;

    private void Start()
    {
        _buttonAudioSource.clip = _buttonSound;

        resumeButton.onClick.AddListener(() => _buttonAudioSource.PlayOneShot(_buttonSound));
        quitButton.onClick.AddListener(() => _buttonAudioSource.PlayOneShot(_buttonSound));
    }

    private void Update()
    {
        var state = ServerManager.Singleton.gameManager.GetState();

        gameState.text = state.Item1.ToString();
        time.text      = TimeSpan.FromSeconds(state.Item2).ToString(@"mm\:ss");
    }

    public void UpdateStats()
    {
        var players = ServerManager.GetAllPlayers().ToList();

        role.text = player.role.ToString();

        int seekersCount    = players.Count(p => p.role == Role.Seeker);
        int hidersCount     = players.Count(p => p.role == Role.Hider);
        int spectatorsCount = players.Count(p => p.role == Role.Spectator);

        seekers.text    = $"Seekers: {seekersCount}";
        hiders.text     = $"Hiders: {hidersCount}";
        spectators.text = $"Spectators: {spectatorsCount}";
    }

    public void TogglePause()
    {
        paused = !paused;

        player.controller.SetStop(paused);

        gameUI.SetActive(!paused);
        pauseUI.SetActive(paused);
    }

    public void Quit()
    {
        // TODO: Quit logic
    }
}