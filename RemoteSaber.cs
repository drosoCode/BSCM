using HarmonyLib;
using UnityEngine;

namespace BSCM.Modifiers
{
    public class RemoteSaber
    {
        internal static VRController LeftSaber = null;
        internal static VRController RightSaber = null;

        public RemoteSaber(GameObject gameCore)
        {
            Plugin.Log.Info("Setting up coop mod...");

            var saberManagerObj = gameCore.transform
                .Find("Origin")
                ?.Find("VRGameCore")
                ?.Find("SaberManager");

            if (saberManagerObj == null)
            {
                Plugin.Log.Critical("Couldn't find SaberManager, bailing!");
                return;
            }

            var saberManager = saberManagerObj.GetComponent<SaberManager>();

            LeftSaber = saberManager.GetPrivateField<Saber>("_leftSaber").GetComponent<VRController>();
            RightSaber = saberManager.GetPrivateField<Saber>("_rightSaber").GetComponent<VRController>();

            if (LeftSaber is null || RightSaber is null)
            {
                Plugin.Log.Critical("Sabers cannot be found. Bailing!");
                return;
            }

            Plugin.Log.Info("Coop mod ready!");
        }
    }

    [HarmonyPatch(typeof(VRController))]
    [HarmonyPatch("Update")]
    class RemoteSaberVRControllerPatch
    {
        static void Postfix(VRController __instance)
        {
            // Plugin.Log.Info("Method called");
            if (!PluginConfig.Instance.Enabled)
            {
                Plugin.Log.Info("Plugin not enabled");
                return;
            }

            if (ReferenceEquals(__instance, RemoteSaber.LeftSaber))
            {
                if(PluginConfig.Instance.isLeftRemoteSaber)
                {
                    // Plugin.Log.Info("updating coords");
                    __instance.transform.position = Plugin.Multi.getLatestPosition();
                    __instance.transform.rotation = Plugin.Multi.getLatestRotation();
                }
                else
                {
                    // Plugin.Log.Info("sending coords");
                    Plugin.Multi.sendCoords(__instance.transform.position, __instance.transform.rotation);
                }
            }
            else if (ReferenceEquals(__instance, RemoteSaber.RightSaber))
            {
                if (!PluginConfig.Instance.isLeftRemoteSaber)
                {
                    // Plugin.Log.Info("updating coords");
                    __instance.transform.position = Plugin.Multi.getLatestPosition();
                    __instance.transform.rotation = Plugin.Multi.getLatestRotation();
                }
                else
                {
                    // Plugin.Log.Info("sending coords");
                    Plugin.Multi.sendCoords(__instance.transform.position, __instance.transform.rotation);
                }
            }
        }
    }

    [HarmonyPatch(typeof(AudioTimeSyncController))]
    [HarmonyPatch("StartSong")]
    class AudioTimeSyncControllerPatch
    {
        static void Postfix(AudioTimeSyncController __instance)
        {
            Plugin.Multi.startSong();
        }
    }
}
