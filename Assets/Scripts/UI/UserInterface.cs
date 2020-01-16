using System;

using Mirror;

using Scripts.Components.Network.Messages;
using Scripts.Management.Game;
using Scripts.PlayerScripts;

using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class UserInterface : MonoBehaviour
    {
        [Header("Sections")]
        [SerializeField] private GameObject gameUI;

        [SerializeField] private GameObject pauseUI;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private AudioClip buttonClickSound;

        [Header("UI Components")]
        [SerializeField] private Button resumeButton;

        [SerializeField] private Button quitButton;
        [SerializeField] private Text   gameState;
        [SerializeField] private Text   time;
        [SerializeField] private Text   role;
        [SerializeField] private Text   seekers;
        [SerializeField] private Text   hiders;
        [SerializeField] private Text   spectators;

        [HideInInspector] public Player player;

        [SerializeField] private int skipUpdates = 100;

        private int skipped;

        private bool paused;

        private GameStateRequest    gameStateRequest;
        private RoleCountersRequest roleCountersRequest;

        private void Awake()
        {
            gameStateRequest    = new GameStateRequest();
            roleCountersRequest = new RoleCountersRequest();

            // TODO: Replace role counters with all players stats in future

            NetworkClient.RegisterHandler<GameStateResponse>(CompileGameState);
            NetworkClient.RegisterHandler<RoleCountersResponse>(UpdateRoleCounters);
        }

        private void Start()
        {
            audioSource.clip = buttonClickSound;

            resumeButton.onClick.AddListener(() => audioSource.PlayOneShot(buttonClickSound));
            quitButton.onClick.AddListener(() => audioSource.PlayOneShot(buttonClickSound));

            NetworkClient.Send(gameStateRequest);
            NetworkClient.Send(roleCountersRequest);
        }

        private void FixedUpdate()
        {
            NetworkClient.Send(gameStateRequest);

            skipped++;

            if (skipped == skipUpdates)
            {
                NetworkClient.Send(roleCountersRequest);

                skipped = 0;
            }
        }

        private void CompileGameState(NetworkConnection sender, GameStateResponse response)
        {
            if (response.currentState == GameState.Ending)
                enabled = false;

            gameState.text = response.currentState.ToString();
            time.text      = TimeSpan.FromSeconds(response.remainingTime).ToString(@"mm\:ss");
        }

        private void UpdateRoleCounters(NetworkConnection sender, RoleCountersResponse response)
        {
            seekers.text    = $"Seekers: {response.seekersCount}";
            hiders.text     = $"Hiders: {response.hidersCount}";
            spectators.text = $"Spectators: {response.spectatorsCount}";
        }

        public void UpdateRole(Role newRole) => role.text = newRole.ToString();

        public void TogglePause()
        {
            gameUI.SetActive(player.Paused);
            pauseUI.SetActive(player.Paused);
        }

        public void Quit()
        {
            // TODO: Quit logic
        }
    }
}