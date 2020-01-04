using Mirror;

using Scripts.Components;
using Scripts.Management.Network;

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

            name = netId.ToString();

            if (isLocalPlayer)
                CmdSetName(name);
        }

        [Command]
        private void CmdSetName(string name) => gameObject.name = name;
    }
}