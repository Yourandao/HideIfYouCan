using System.Collections.Generic;

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
        public static ServerManager SingletonOverride { get; private set; }

        [Header("Control Components")]
        public GameManager gameManager = default;

        private static Dictionary<uint, Player> _players = new Dictionary<uint, Player>();

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

            return true;
        }

        public override void OnRoomServerPlayersReady()
        {
            base.OnRoomServerPlayersReady();

            gameManager.StartGame();
        }

        #region Player management

        public static void RegisterPlayer(uint id, Player player)
        {
            _players.Add(id, player);
        }

        public static void UnregisterPlayer(uint id, Role role)
        {
            _players.Remove(id);

            SingletonOverride.gameManager.UnassignRole(role);
        }

        public static Player GetPlayer(uint id) => _players[id];

        public static IEnumerable<Player> GetAllPlayers() => _players.Values;

        #endregion
    }
}