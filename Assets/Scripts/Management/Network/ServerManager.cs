using System.Collections.Generic;

using Assets.Scripts.Exceptions;
using Assets.Scripts.Management.Game;
using Assets.Scripts.PlayerScripts;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.Management.Network
{
    public sealed class ServerManager : NetworkRoomManager
    {
        public static ServerManager SingletonOverride { get; private set; }

        [Header("Control Components")]
        public GameManager gameManager = default;

        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

        #if UNITY_SERVER
        private bool isServerStarted;
        #endif

        public override void Awake()
        {
            base.Awake();

            if (SingletonOverride != null)
                throw new MultiInstanceException(gameObject);

            SingletonOverride = this;

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

        public override bool OnRoomServerSceneLoadedForPlayer(GameObject roomPlayer, GameObject gamePlayer)
        {
            var role = roomPlayer.GetComponent<RoomPlayer>().role;

            gamePlayer.GetComponent<Player>().role = role;

            gameManager.StartGame();

            return true;
        }

        #region Player management

        public static void RegisterPlayer(string name, Player player)
        {
            _players.Add(name, player);
        }

        public static void UnregisterPlayer(string playerName, Role role)
        {
            _players.Remove(playerName);

            SingletonOverride.gameManager.UnassignRole(role);
        }

        public static IEnumerable<Player> GetAllPlayers() => _players.Values;

        #endregion
    }
}