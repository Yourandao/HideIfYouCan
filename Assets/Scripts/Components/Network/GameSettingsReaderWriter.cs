using Mirror;

using Scripts.Management.Game;

namespace Scripts.Components.Network
{
    public static class GameSettingsReaderWriter
    {
        public static void WriteTimeSettings(this NetworkWriter writer, TimeSettings settings)
        {
            writer.WriteSingle(settings.maxWaitingTime);
            writer.WriteSingle(settings.freezeTime);
            writer.WriteSingle(settings.hideTime);
            writer.WriteSingle(settings.seekTime);
            writer.WriteSingle(settings.endingTime);
        }

        public static void WriteGameSettings(this NetworkWriter writer, GameSettings settings)
        {
            writer.WriteTimeSettings(settings.timeSettings);
            writer.WriteSingle(settings.seekersToHidersRelation);
        }

        public static TimeSettings ReadTimeSettings(this NetworkReader reader)
        {
            var settings = new TimeSettings
            {
                maxWaitingTime = reader.ReadSingle(),
                freezeTime     = reader.ReadSingle(),
                hideTime       = reader.ReadSingle(),
                seekTime       = reader.ReadSingle(),
                endingTime     = reader.ReadSingle()
            };

            return settings;
        }

        public static GameSettings ReadGameSettings(this NetworkReader reader)
        {
            var settings = new GameSettings
            {
                timeSettings            = reader.ReadTimeSettings(),
                seekersToHidersRelation = reader.ReadSingle(),
            };

            return settings;
        }
    }
}