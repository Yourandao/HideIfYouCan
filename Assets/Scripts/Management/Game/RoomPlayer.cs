using Mirror;

using Scripts.Management.Network;
using Scripts.PlayerScripts;

using UnityEngine;

namespace Scripts.Management.Game
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