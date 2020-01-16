using Mirror;

using Scripts.Management.Game;

namespace Scripts.Components.Network.Messages
{
    public sealed class GameStateResponse : MessageBase
    {
        public GameState currentState;
        public float     remainingTime;
    }
}