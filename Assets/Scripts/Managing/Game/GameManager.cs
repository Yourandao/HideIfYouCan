using System.Collections.Generic;

using Assets.Scripts.Exceptions;
using Assets.Scripts.PlayerScripts;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.Managing.Game
{
    public sealed class GameManager : NetworkManager
    {
        public static GameManager Singleton { get; private set; }

        public GameSettings gameSettings = new GameSettings();

        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

        private static int _hidersCount;
        private static int _seekersCount;

        public override void Awake()
        {
            if (Singleton != null)
                throw new MultiInstanceException(gameObject);

            Singleton = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<SpawnMessage>(OnRoleAssigned);

            _players.Clear();

            _hidersCount  = 0;
            _seekersCount = 0;
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);

            var spawnMessage = new SpawnMessage();

            if ((float) _seekersCount / (_hidersCount + 1) < gameSettings.seekersToHidersRelation)
            {
                if (Random.value >= .5f)
                {
                    spawnMessage.role = Role.Hider;
                    _hidersCount++;
                }
                else
                {
                    spawnMessage.role = Role.Seeker;
                    _seekersCount++;
                }
            }
            else
            {
                spawnMessage.role = Role.Hider;
                _hidersCount++;
            }

            connection.Send(spawnMessage);
        }

        private void OnRoleAssigned(NetworkConnection connection, SpawnMessage message)
        {
            var instance = Instantiate(playerPrefab);

            var player = instance.GetComponent<Player>();
            player.role = message.role;

            NetworkServer.AddPlayerForConnection(connection, instance);
        }

        public static void RegisterPlayer(string name, Player player)
        {
            _players.Add(name, player);
        }

        public static void UnregisterPlayer(string playerName, Role role)
        {
            _players.Remove(playerName);

            switch (role)
            {
                case Role.Hider:
                    _hidersCount--;

                    break;
                case Role.Seeker:
                    _seekersCount--;

                    break;
                default: throw new UnhandledRoleException(role);
            }
        }
    }
}