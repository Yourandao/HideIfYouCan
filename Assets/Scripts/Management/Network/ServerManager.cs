using System.Collections.Generic;

using Mirror;

using Scripts.Exceptions;
using Scripts.Management.Game;
using Scripts.PlayerScripts;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Management.Network
{
    public sealed class ServerManager : NetworkRoomManager
    {
        public new static ServerManager singleton;

        [Attributes.Scene]
        public string[] gameplayScenes;

        [SerializeField]  private GameObject gameManagerPrefab;
        [HideInInspector] public  GameObject gameManagerInstance;

        private GameManager gameManager;

        [SerializeField] private GameSettings gameSettings = new GameSettings();

        public override void Awake()
        {
            base.Awake();

            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (singleton != null && singleton == this)
                return;

            if (singleton != null)
            {
                Debug.LogWarning("Multiple ServerManagers detected. Duplicate will be destroyed.");

                Destroy(gameObject);

                return;
            }

            singleton = this;

            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }

        private void InitializeGameManager()
        {
            gameManagerInstance      = Instantiate(gameManagerPrefab);
            gameManagerInstance.name = gameManagerPrefab.name;

            gameManager              = GameManager.singleton;
            gameManager.gameSettings = gameSettings;
        }

        #region Server management

        private int loadedPlayers;

        public override void OnStartServer()
        {
            base.OnStartServer();

            InitializeGameManager();
        }

        public override void OnRoomServerPlayersReady()
        {
            _players.Clear();

            _hiderSpawnsIndex  = 0;
            _seekerSpawnsIndex = 0;

            loadedPlayers = 0;

            int sceneIndex = Random.Range(0, gameplayScenes.Length);
            ServerChangeScene(gameplayScenes[sceneIndex]);

            gameManager.StartGame();
        }

        public override bool OnRoomServerSceneLoadedForPlayer(GameObject roomPlayer, GameObject gamePlayer)
        {
            var role = roomPlayer.GetComponent<RoomPlayer>().Role;

            gamePlayer.GetComponent<Player>().role = role;

            if (role != Role.Hider && role != Role.Seeker)
                throw new UnhandledRoleException(role);

            var spawn = GetSpawn(role);

            gamePlayer.transform.position      = spawn.position;
            gamePlayer.transform.localRotation = spawn.localRotation;

            loadedPlayers++;

            if (loadedPlayers == roomSlots.Count)
                gameManager.AllPlayersLoaded = true;

            return true;
        }

        public override void OnRoomServerDisconnect(NetworkConnection connection)
        {
            base.OnRoomServerDisconnect(connection);

            if (_players.Count == 0 &&
                SceneManager.GetActiveScene().name != RoomScene)
            {
                gameManager.EndGame();
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            Destroy(gameManagerInstance);
        }

        #endregion

        #region Spawns management

        private static List<Transform> _hiderSpawns  = new List<Transform>();
        private static List<Transform> _seekerSpawns = new List<Transform>();

        private static int _hiderSpawnsIndex;
        private static int _seekerSpawnsIndex;

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

        #region Players management

        static Dictionary<uint, Player> _players = new Dictionary<uint, Player>();

        public static void RegisterPlayer(uint id, Player player) => _players.Add(id, player);

        public static void UnregisterPlayer(uint id) => _players.Remove(id);

        public static Player GetPlayer(uint id) => _players[id];

        public static IEnumerable<Player> GetAllPlayers() => _players.Values;

        #endregion
    }
}