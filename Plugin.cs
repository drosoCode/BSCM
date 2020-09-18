using HarmonyLib;
using IPA;
using System;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;
using Config = IPA.Config.Config;
using IPA.Config.Stores;

[assembly: AssemblyTitle("BSCM")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2020 Steffan Donal & DrosoCode")]

namespace BSCM
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal const string CapabilityName = @"BSCM";

        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        public static Multiplayer Multi { get; internal set; }
        public static IPALogger Log { get; internal set; }
        static bool _isInitialized;
        static Gamemode _gamemode;

        [Init]
        public void Init(object _, IPALogger log, Config config)
        {
            Log = log;
            PluginConfig.Instance = config.Generated<PluginConfig>();
        }

        [OnStart]
        public void OnStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(OnStart)} called more than once! Critical failure.");
            _isInitialized = true;

            try
            {
                var harmony = new Harmony("com.github.drosocode.bscm");
                harmony.PatchAll(Assembly);
            }
            catch (Exception e)
            {
                Log.Error("This plugin requires Harmony. Make sure you installed the plugin properly, as the Harmony DLL should have been installed with it.");
                Log.Error(e.ToString());
                return;
            }

            _gamemode = new Gamemode();
            Log.Info($"v{Version} loaded!");

            if (PluginConfig.Instance.Enabled)
            {
                SongCore.Collections.RegisterCapability(Plugin.CapabilityName);
                Multi = new Multiplayer();
            }
        }

        [OnExit]
        public void OnExit()
        {
            if (Multi != null)
                Multi.stop();
        }
    }
}
