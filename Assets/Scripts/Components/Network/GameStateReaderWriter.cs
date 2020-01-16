using Mirror;

using Scripts.Management.Game;

namespace Scripts.Components.Network
{
    public static class GameStateReaderWriter
    {
        public static void WriteGameState(this NetworkWriter writer, GameState state) => writer.WriteInt32((int) state);

        public static GameState ReadGameState(this NetworkReader reader) => (GameState) reader.ReadInt32();
    }
}