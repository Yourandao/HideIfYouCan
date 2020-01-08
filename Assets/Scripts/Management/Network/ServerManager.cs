using System.Collections.Generic;

using Mirror;

using Scripts.Components;
using Scripts.Exceptions;
using Scripts.Management.Game;
using Scripts.PlayerScripts;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Scripts.Management.Network
{
    [RequireComponent(typeof(GameManager))]
    public sealed class ServerManager : NetworkRoomManager
    {
        public static ServerManager Singleton { get; private set; }

        public GameManager gameManager;

        private static List<Transform> _hiderSpawns  = new List<Transform>();
        private static List<Transform> _seekerSpawns = new List<Transform>();

        private static int _hiderSpawnsIndex;
        private static int _seekerSpawnsIndex;

        private static Dictionary<uint, Player> _players = new Dictionary<uint, Player>();

        public static int AllPlayers
        {
            get => _players.Count;
        }

        public static int LoadedPlayers { get; private set; }

        #if UNITY_SERVERD
        private bool isServerStarted;
        #endif

        public override void Awake()
        {
            base.Awake();

            if (Singleton != null)
                throw new MultiInstanceException(gameObject);

            Singleton = this;

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

            gameManager.StartGame();
        }

        public override bool OnRoomServerSceneLoadedForPlayer(GameObject roomPlayer, GameObject gamePlayer)
        {
            var role = roomPlayer.GetComponent<RoomPlayer>().Role;

            gamePlayer.GetComponent<Player>().role = role;

            if (role != Role.Hider && role != Role.Seeker)
                throw new UnhandledRoleException(role);

            var spawn = GetSpawn(role);

            gamePlayer.transform.position = spawn.position;
            gamePlayer.transform.rotation = spawn.rotation;

            return true;
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);

            LoadedPlayers = 0;
        }

        public override void OnRoomClientSceneChanged(NetworkConnection connection)
        {
            base.OnRoomClientSceneChanged(connection);

            LoadedPlayers++;
        }

        #region Spawns management

        public static void RegisterHiderSpawn(Transform transform) => _hiderSpawns.Add(transform);

        public static void RegisterSeekerSpawn(Transform transform) => _seekerSpawns.Add(transform);

        public static void UnregisterHiderSpawn(Transform transform) => _hiderSpawns.Remove(transform);

        public static void UnregisterSeekerSpawn(Transform transform) => _seekerSpawns.Remove(transform);

        private Transform GetSpawn(Role role)
        {
            int @default = default;

            List<Transform> spawns;
            ref int         index = ref @default;

            if (role == Role.Hider)
            {
                spawns = _hiderSpawns;
                index  = ref _hiderSpawnsIndex;
            }
            else
            {
                spawns = _seekerSpawns;
                index  = ref _seekerSpawnsIndex;
            }

            if (playerSpawnMethod == PlayerSpawnMethod.Random)
                return spawns[Random.Range(0, spawns.Count)];

            var spawn = spawns[index];
            index = (index + 1) % spawns.Count;

            return spawn;
        }

        #endregion

        #region Player management

        public static void RegisterPlayer(uint id, Player player) => _players.Add(id, player);

        public static void UnregisterPlayer(uint id, Role role) => _players.Remove(id);

        public static Player GetPlayer(uint id) => _players[id];

        public static IEnumerable<Player> GetAllPlayers() => _players.Values;

        #endregion
    }
}