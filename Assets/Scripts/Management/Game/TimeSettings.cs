namespace Scripts.Management.Game
{
    [System.Serializable]
    public sealed class TimeSettings
    {
        public float maxWaitingTime = 30f;
        public float freezeTime     = 10f;
        public float hideTime       = 60f;
        public float seekTime       = 300f;
        public float endingTime     = 30f;
    }
}