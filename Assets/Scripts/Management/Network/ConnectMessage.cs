using Assets.Scripts.PlayerScripts;

using Mirror;

namespace Assets.Scripts.Management.Network
{
    public class ConnectMessage : MessageBase
    {
        public Role Role { get; set; }
    }
}