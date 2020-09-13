using BeatSaberMarkupLanguage.GameplaySetup;
using BSCM.Modifiers;
using BSCM.Views;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

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
            var gameCore = loadedScene.GetRootGameObjects().First();

            _remoteSaber = new RemoteSaber(gameCore, 1);
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
