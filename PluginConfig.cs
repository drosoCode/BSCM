namespace BSCM
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public bool Enabled { get; set; }
        public string url { get; set; }
        public int port { get; set; }
        public bool isServer { get; set; }
        public bool disableRumble { get; set; }
        public bool isLeftRemoteSaber { get; set; }
    }
}
