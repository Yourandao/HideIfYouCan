using System;
using System.Collections.Generic;
using System.Linq;

using Mirror;

using Scripts.Components.Network.Messages;
using Scripts.Management.Network;
using Scripts.PlayerScripts;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Scripts.Management.Game
{
    public sealed class GameManager : NetworkBehaviour
    {
        public static GameManager singleton;

        [NonSerialized] public GameSettings gameSettings;

        private GameState gameState;

        private float time;

        public bool AllPlayersLoaded { private get; set; }

        private void Awake()
        {
            InitializeSingleton();

            NetworkServer.RegisterHandler<GameStateRequest>(ReturnGameState);
            NetworkServer.RegisterHandler<RoleCountersRequest>(ReturnRoleCounters);

            gameState = GameState.None;
        }

        private void InitializeSingleton()
        {
            if (singleton != null && singleton == this)
                return;

            if (singleton != null)
            {
                Debug.LogWarning("Multiple GameManagers detected. Duplicate will be destroyed.");

                Destroy(gameObject);

                return;
            }

            singleton = this;

            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }

        private void FixedUpdate()
        {
            if (gameState == GameState.None)
                return;

            time += Time.fixedDeltaTime;

            switch (gameState)
            {
                case GameState.Waiting:
                    if (AllPlayersLoaded || time >= gameSettings.timeSettings.maxWaitingTime)
                    {
                        gameState = GameState.FreezeTime;
                        time      = 0f;

                        Debug.Log("Game started");
                    }

                    break;

                case GameState.FreezeTime:
                    if (time >= gameSettings.timeSettings.freezeTime)
                    {
                        foreach (var player in ServerManager
                                               .GetAllPlayers()
                                               .Where(p => p.role == Role.Hider))
                        {
                            player.RpcStartGame();
                        }

                        gameState = GameState.HideTime;
                        time      = 0f;

                        Debug.Log("Freeze time expired");
                    }

                    break;
                case GameState.HideTime:
                    if (time >= gameSettings.timeSettings.hideTime)
                    {
                        foreach (var player in ServerManager
                                               .GetAllPlayers()
                                               .Where(p => p.role == Role.Seeker))
                        {
                            player.RpcStartGame();
                        }

                        gameState = GameState.SeekTime;
                        time      = 0f;

                        Debug.Log("Time to hide has ended");
                    }

                    break;
                case GameState.SeekTime:
                    if (time >= gameSettings.timeSettings.seekTime)
                    {
                        foreach (var player in ServerManager.GetAllPlayers())
                            player.RpcStopGame();

                        gameState = GameState.Ending;
                        time      = 0f;

                        Debug.Log("Game over");
                    }

                    break;
                case GameState.Ending:
                    if (time >= gameSettings.timeSettings.endingTime)
                    {
                        EndGame();

                        gameState = GameState.None;
                        time = 0f;
                    }

                    break;
            }
        }

        public void StartGame()
        {
            AssignRoles(ServerManager.singleton.roomSlots
                                     .Select(r => r.GetComponent<RoomPlayer>())
                                     .ToArray());

            gameState = GameState.Waiting;

            Debug.Log("Waiting phase");
        }

        public void EndGame()
        {
            ServerManager.singleton.ServerChangeScene(ServerManager.singleton.RoomScene);
        }

        private void AssignRoles(IReadOnlyCollection<RoomPlayer> players)
        {
            var unassignedPlayers = new List<RoomPlayer>(players);

            int seekersCount = (int) Math.Round(players.Count * gameSettings.seekersToHidersRelation,
                                                MidpointRounding.AwayFromZero);

            for (int i = 0; i < seekersCount; i++)
            {
                int index = Random.Range(0, unassignedPlayers.Count);

                unassignedPlayers[index].Role = Role.Seeker;

                unassignedPlayers.RemoveAt(index);
            }

            foreach (var player in unassignedPlayers)
                player.Role = Role.Hider;
        }

        private void ReturnGameState(NetworkConnection reciever, GameStateRequest request)
        {
            var response = new GameStateResponse { currentState = gameState };

            switch (gameState)
            {
                case GameState.Waiting:
                    response.remainingTime = gameSettings.timeSettings.maxWaitingTime - time;

                    break;
                case GameState.FreezeTime:
                    response.remainingTime = gameSettings.timeSettings.freezeTime - time;

                    break;
                case GameState.HideTime:
                    response.remainingTime = gameSettings.timeSettings.hideTime - time;

                    break;
                case GameState.SeekTime:
                    response.remainingTime = gameSettings.timeSettings.seekTime - time;

                    break;
                case GameState.Ending:
                    response.remainingTime = gameSettings.timeSettings.endingTime - time;

                    break;
            }

            reciever.Send(response);
        }

        private void ReturnRoleCounters(NetworkConnection reciever, RoleCountersRequest request)
        {
            var players = ServerManager.GetAllPlayers().ToList();

            var response = new RoleCountersResponse
            {
                seekersCount    = players.Count(p => p.role == Role.Seeker),
                hidersCount     = players.Count(p => p.role == Role.Hider),
                spectatorsCount = players.Count(p => p.role == Role.Spectator)
            };

            reciever.Send(response);
        }
    }
}