using HarmonyLib;
using IPA;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using IPALogger = IPA.Logging.Logger;
using Config = IPA.Config.Config;
using IPA.Config.Stores;
using UnityEngine;

[assembly: AssemblyTitle("BSCM")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2020 Steffan Donal & DrosoCode")]

// [assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace BSCM
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal const string CapabilityName = @"BSCM";

        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        /// <summary>
        /// True if the mod is enabled in-game.
        /// </summary>

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


            if (PluginConfig.Instance.Enabled)
            {
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

                Log.Info("Starting");

                Multi = new Multiplayer();

                //new GameObject("PauseController").AddComponent<PauseController>();

                _gamemode = new Gamemode();

                Log.Info($"v{Version} loaded!");
            }
        }

        [OnExit]
        public void OnExit()
        {
            
        }
    }
}
