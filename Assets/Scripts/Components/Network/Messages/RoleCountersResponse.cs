using Mirror;

namespace Scripts.Components.Network.Messages
{
    public sealed class RoleCountersResponse : MessageBase
    {
        public int seekersCount;
        public int hidersCount;
        public int spectatorsCount;
    }
}