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
            if(PluginConfig.Instance.disableSumbission)
                ScoreSubmission.DisableSubmission("BSCM");

            var gameCore = loadedScene.GetRootGameObjects().First();
            _remoteSaber = new RemoteSaber(gameCore);
        }

        void OnMenuLoaded()
        {
            _gamemodeSettingsView = new GamemodeSettingsViewController();
            _gamemodeSettingsView.IsEnabledChanged += OnGamemodeToggled;

            GameplaySetup.instance.AddTab(
                "BSCM",
                GamemodeSettingsViewController.Resource,
                _gamemodeSettingsView
            );

            UpdateCapability();
        }

        void OnGamemodeToggled(object sender, EventArgs e)
        {
            UpdateCapability();
        }

        void UpdateCapability()
        {
            if (PluginConfig.Instance.Enabled)
                SongCore.Collections.RegisterCapability(Plugin.CapabilityName);
            else
                SongCore.Collections.DeregisterizeCapability(Plugin.CapabilityName);
        }
    }
}
