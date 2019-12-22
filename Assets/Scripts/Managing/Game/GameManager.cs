using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Assets.Scripts.PlayerScripts;
using Assets.Scripts.PlayerScripts.PlayerRoles;

namespace Assets.Scripts.Managing.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Singleton { get; private set; }

        public GameSettings gameSettings;

        public delegate void GameStartHandler();
        public static event GameStartHandler OnGameStart;

        private void Start()
        {
            if (Singleton != null)
                throw new MultiInstanceException(gameObject);

            Singleton = this;
        }

        public void StartGame()
        {
            var allPlayers = new List<Player>(Players.Values);

            var hiders  = new List<Player>();
            var seekers = new List<Player>();

            while (hiders.Count + seekers.Count < allPlayers.Count)
            {
                if ((float) seekers.Count / (hiders.Count + 1) < gameSettings.seekersToHidersRelation)
                {
                    var randomPlayer = allPlayers.ElementAt(Random.Range(0, allPlayers.Count));

                    randomPlayer.playerRole.role = Roles.Seeker;

                    seekers.Add(randomPlayer);
                    allPlayers.Remove(randomPlayer);
                }
                else
                {
                    hiders.AddRange(allPlayers);
                }
            }

            OnGameStart?.Invoke();
        }

        #region Player management

        private static Dictionary<string, Player> Players { get; } = new Dictionary<string, Player>();

        public static void RegisterPlayer(string name, Player player)
        {
            player.gameObject.name = name;
            player.playerRole.role = Roles.Unassigned;

            Players.Add(name, player);
        }

        public static void UnregisterPlayer(string name) =>
            Players.Remove(name);

        #endregion
    }
}