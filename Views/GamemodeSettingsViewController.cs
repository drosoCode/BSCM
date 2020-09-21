using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace BSCM.Views
{
    public class GamemodeSettingsViewController : BSMLResourceViewController
    {
        public const string Resource = "BSCM.Views.GamemodeSettings.bsml";

        public override string ResourceName => Resource;

        public event EventHandler IsEnabledChanged;
        public event EventHandler SettingChanged;

        [UIValue("isEnabled")]
        public bool IsEnabled
        {
            get => PluginConfig.Instance.Enabled;
            set
            {
                PluginConfig.Instance.Enabled = value;
                IsEnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [UIValue("disableRumble")]
        public bool disableRumble
        {
            get => PluginConfig.Instance.disableRumble;
            set
            {
                PluginConfig.Instance.disableRumble = value;
            }
        }

        [UIValue("isLeftRemoteSaber")]
        public bool isLeftRemoteSaber
        {
            get => PluginConfig.Instance.isLeftRemoteSaber;
            set
            {
                PluginConfig.Instance.isLeftRemoteSaber = value;
                SettingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
