using System.Collections.Generic;
using System.Linq;

using Mirror;

using Scripts.Components;
using Scripts.Exceptions;
using Scripts.Management.Game;
using Scripts.PlayerScripts;

using UnityEngine;

namespace Scripts.Management.Network
{
    public sealed class ServerManager : NetworkRoomManager
    {
        public new static ServerManager singleton;

        public GameManager gameManager;

        private static Dictionary<uint, Player> _players = new Dictionary<uint, Player>();

        #if UNITY_SERVER
        private bool isServerStarted;
        #endif

        public override void Awake()
        {
            base.Awake();

            if (singleton != null)
                throw new MultiInstanceException(gameObject);

            singleton = this;

            #if UNITY_SERVER
            if (isServerStarted)
                return;

            StartServer();

            isServerStarted = true;

            #endif
        }

        public override void OnRoomStartServer()
        {
            base.OnRoomStartServer();

            _players.Clear();
        }

        public override void OnRoomServerPlayersReady()
        {
            int sceneIndex = Random.Range(0, gameManager.gameplayScenes.Length);

            ServerChangeScene(gameManager.gameplayScenes[sceneIndex]);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(GameObject roomPlayer, GameObject gamePlayer)
        {
            gamePlayer.GetComponent<Player>().role = roomPlayer.GetComponent<RoomPlayer>().Role;

            return true;
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);

            gameManager.StartGame();
        }

        #region Player management

        public static void RegisterPlayer(uint id, Player player) => _players.Add(id, player);

        public static void UnregisterPlayer(uint id, Role role) => _players.Remove(id);

        public static Player GetPlayer(uint id) => _players[id];

        public static IEnumerable<Player> GetAllPlayers() => _players.Values;

        #endregion
    }
}