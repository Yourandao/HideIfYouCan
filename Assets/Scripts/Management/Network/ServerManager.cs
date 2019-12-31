using System.Collections.Generic;

using Assets.Scripts.Exceptions;
using Assets.Scripts.Management.Game;
using Assets.Scripts.PlayerScripts;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.Management.Network
{
    public sealed class ServerManager : NetworkManager
    {
        public static ServerManager NewSingleton { get; private set; }

        [Header("Control Components")]
        [SerializeField] private GameManager gameManager = default;

        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

        private static int _readyPlayersNum;

        public override void Awake()
        {
            base.Awake();

            if (NewSingleton != null)
                throw new MultiInstanceException(gameObject);

            NewSingleton = this;
        }

        #region Server management

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<SpawnMessage>(OnSetupDone);

            _players.Clear();
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);

            var spawnMessage = new SpawnMessage
            {
                role = gameManager.AssignRole()
            };

            connection.Send(spawnMessage);
        }

        private void OnSetupDone(NetworkConnection connection, SpawnMessage message)
        {
            var instance = Instantiate(playerPrefab);

            var player = instance.GetComponent<Player>();
            player.role = message.role;

            NetworkServer.AddPlayerForConnection(connection, instance);
        }

        #endregion

        #region Player management

        public static void RegisterPlayer(string name, Player player)
        {
            _players.Add(name, player);
        }

        public static void UnregisterPlayer(string playerName, Role role)
        {
            _players.Remove(playerName);

            NewSingleton.gameManager.UnassignRole(role);
        }

        public static IEnumerable<Player> GetAllPlayers() => _players.Values;

        public void PlayerReady()
        {
            _readyPlayersNum++;

            if (_readyPlayersNum != _players.Count)
                return;

            gameManager.AllPlayersReady();
        }

        #endregion
    }
}