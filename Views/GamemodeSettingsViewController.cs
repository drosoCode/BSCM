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
    }
}
