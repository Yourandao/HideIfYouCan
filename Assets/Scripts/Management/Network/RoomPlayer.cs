using Mirror;

using Scripts.PlayerScripts;

namespace Scripts.Management.Network
{
    public sealed class RoomPlayer : NetworkRoomPlayer
    {
        public Role Role { get; set; }

        public override void OnClientEnterRoom()
        {
            base.OnClientEnterRoom();

            name = netId.ToString();

            if (isLocalPlayer)
                CmdSetName(name);
        }

        [Command]
        private void CmdSetName(string name) => gameObject.name = name;
    }
}