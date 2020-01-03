using Assets.Scripts.Management.Network;
using Assets.Scripts.PlayerScripts;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.Management.Game
{
    public sealed class RoomPlayer : NetworkRoomPlayer
    {
        [HideInInspector]
        public Role role;

        public override void OnClientEnterRoom()
        {
            base.OnClientEnterRoom();

            role = ServerManager.SingletonOverride.gameManager.AssignRole();
        }
    }
}