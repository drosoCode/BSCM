using BeatSaberMarkupLanguage.GameplaySetup;
using BSCM.Modifiers;
using BSCM.Views;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using BS_Utils.Gameplay;

namespace BSCM
{
    internal class Gamemode
    {
        RemoteSaber _remoteSaber;
        GamemodeSettingsViewController _gamemodeSettingsView;

        internal Gamemode()
        {
            SceneManager.sceneLoaded += OnSceneManagerSceneLoaded;
        }

        void OnSceneManagerSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            if (loadedScene.name == "GameCore") OnGameLoaded(loadedScene);
            if (loadedScene.name == "MenuCore") OnMenuLoaded();
        }

        void OnGameLoaded(Scene loadedScene)
        {
            if(PluginConfig.Instance.Enabled)
            {
                if (PluginConfig.Instance.disableSumbission)
                    ScoreSubmission.DisableSubmission("BSCM");

                var gameCore = loadedScene.GetRootGameObjects().First();
                _remoteSaber = new RemoteSaber(gameCore);
            }
        }

        void OnMenuLoaded()
        {
            _gamemodeSettingsView = new GamemodeSettingsViewController();
            _gamemodeSettingsView.IsEnabledChanged += OnGamemodeToggled;
            _gamemodeSettingsView.SettingChanged += onSettingChanged;

            GameplaySetup.instance.AddTab(
                "BSCM",
                GamemodeSettingsViewController.Resource,
                _gamemodeSettingsView
            );
        }

        void OnGamemodeToggled(object sender, EventArgs e)
        {
            UpdateCapability();
        }

        void onSettingChanged(object sender, EventArgs e)
        {
            // reload multiplayer
            UpdateCapability(0);
            UpdateCapability(1);
        }

        void UpdateCapability(int status = -1)
        {
            if (status == 1 || (PluginConfig.Instance.Enabled && status != 0))
            {
                SongCore.Collections.RegisterCapability(Plugin.CapabilityName);
                Plugin.Multi = null;
                Plugin.Multi = new Multiplayer();
            }
            else
            {
                SongCore.Collections.DeregisterizeCapability(Plugin.CapabilityName);
                if (Plugin.Multi != null)
                    Plugin.Multi.stop();
            }
        }
    }
}
