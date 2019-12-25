using System.Collections.Generic;

using Assets.Scripts.Exceptions;
using Assets.Scripts.PlayerScripts;
using Assets.Scripts.PlayerScripts.PlayerRoles;

using Mirror;

using Random = UnityEngine.Random;

namespace Assets.Scripts.Managing.Game
{
    public sealed class GameManager : NetworkManager
    {
        public static GameManager Singleton { get; private set; }

        public GameSettings gameSettings = new GameSettings();

        public static Dictionary<string, Player> Players { get; } = new Dictionary<string, Player>();

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

            Players.Clear();

            _hidersCount  = 0;
            _seekersCount = 0;
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);

            var spawnMessage = new SpawnMessage();

            if ((float) _seekersCount / (_hidersCount + 1) < gameSettings.seekersToHidersRelation)
            {
                //if (Random.value >= .5f)
                //{
                    spawnMessage.role = Roles.Hider;
                    _hidersCount++;
                //}
                //else
                //{
                //    spawnMessage.role = Roles.Seeker;
                //    _seekersCount++;
                //}
            }
            else
            {
                spawnMessage.role = Roles.Hider;
                _hidersCount++;
            }

            connection.Send(spawnMessage);
        }

        private void OnRoleAssigned(NetworkConnection connection, SpawnMessage message)
        {
            var instance = Instantiate(playerPrefab);

            var player = instance.GetComponent<Player>();
            player.playerRole.role = message.role;

            NetworkServer.AddPlayerForConnection(connection, instance);
        }

        public static void UnregisterPlayer(string playerName, Roles role)
        {
            Players.Remove(playerName);

            switch (role)
            {
                case Roles.Hider:
                    _hidersCount--;

                    break;
                case Roles.Seeker:
                    _seekersCount--;

                    break;
                default: throw new UnhandledRoleException(role);
            }
        }
    }
}