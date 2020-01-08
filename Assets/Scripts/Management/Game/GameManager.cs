﻿using System;
using System.Collections.Generic;
using System.Linq;

using Mirror;

using Scripts.Components;
using Scripts.Management.Network;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Scripts.Management.Game
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings = new GameSettings();

        [Attributes.Scene]
        public string[] gameplayScenes;

        private GameState gameState;

        private float time;

        private void Awake()
        {
            gameState = GameState.NotStarted;
        }

        private void FixedUpdate()
        {
            if (gameState == GameState.Finished || gameState == GameState.NotStarted)
                return;

            time += Time.fixedDeltaTime;

            switch (gameState)
            {
                case GameState.Waiting:
                    if (ServerManager.LoadedPlayers == ServerManager.AllPlayers ||
                        time >= gameSettings.maxWaitingTime)
                    {
                        gameState = GameState.FreezeTime;
                        time      = 0f;

                        Debug.Log("Game started");
                    }

                    break;

                case GameState.FreezeTime:
                    if (time >= gameSettings.freezeTime)
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
                    if (time >= gameSettings.hideTime)
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
                    if (time >= gameSettings.seekTime)
                    {
                        foreach (var player in ServerManager.GetAllPlayers())
                            player.RpcStopGame();

                        gameState = GameState.Ending;
                        time      = 0f;

                        Debug.Log("Round ended");
                    }

                    break;
                case GameState.Ending:
                    if (time >= gameSettings.endingTime)
                    {
                        int sceneIndex = Random.Range(0, gameplayScenes.Length);

                        NetworkManager.singleton.ServerChangeScene(gameplayScenes[sceneIndex]);

                        gameState = GameState.FreezeTime;
                    }

                    break;
            }
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

        public void StartGame()
        {
            gameState = GameState.Waiting;

            AssignRoles(ServerManager.Singleton.roomSlots
                                     .Select(r => r.GetComponent<RoomPlayer>())
                                     .ToArray());

            Debug.Log("Waiting phase");
        }

        public (GameState, float) GetState()
        {
            var value = (gameState, default(float));

            switch (gameState)
            {
                case GameState.Waiting:
                    value.Item2 = gameSettings.maxWaitingTime - time;

                    break;
                case GameState.FreezeTime:
                    value.Item2 = gameSettings.freezeTime - time;

                    break;
                case GameState.HideTime:
                    value.Item2 = gameSettings.hideTime - time;

                    break;
                case GameState.SeekTime:
                    value.Item2 = gameSettings.seekTime - time;

                    break;
                case GameState.Ending:
                    value.Item2 = gameSettings.endingTime - time;

                    break;
            }

            return value;
        }
    }
}