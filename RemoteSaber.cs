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
            Plugin.Log.Info("Searching Sabers ...");

            var saberManagerObj = gameCore.transform
                .Find("Origin")
                ?.Find("VRGameCore")
                ?.Find("SaberManager");

            if (saberManagerObj == null)
            {
                Plugin.Log.Critical("Couldn't find SaberManager !");
                return;
            }

            var saberManager = saberManagerObj.GetComponent<SaberManager>();

            LeftSaber = saberManager.GetPrivateField<Saber>("_leftSaber").GetComponent<VRController>();
            RightSaber = saberManager.GetPrivateField<Saber>("_rightSaber").GetComponent<VRController>();

            if (LeftSaber is null || RightSaber is null)
            {
                Plugin.Log.Critical("Sabers cannot be found !");
                return;
            }

            Plugin.Log.Info("Sabers Found !");
        }
    }

    [HarmonyPatch(typeof(VRController))]
    [HarmonyPatch("Update")]
    class RemoteSaberVRControllerPatch
    {
        static void Postfix(VRController __instance)
        {
            if (!PluginConfig.Instance.Enabled)
            {
                Plugin.Log.Info("Plugin not enabled");
                return;
            }

            Plugin.Multi.checkMessages();
            if (ReferenceEquals(__instance, RemoteSaber.LeftSaber))
            {
                if(PluginConfig.Instance.isLeftRemoteSaber)
                {
                    __instance.transform.position = Plugin.Multi.getLatestPosition();
                    __instance.transform.rotation = Plugin.Multi.getLatestRotation();
                }
                else
                {
                    Plugin.Multi.sendCoords(__instance.transform.position, __instance.transform.rotation);
                }
            }
            else if (ReferenceEquals(__instance, RemoteSaber.RightSaber))
            {
                if (!PluginConfig.Instance.isLeftRemoteSaber)
                {
                    __instance.transform.position = Plugin.Multi.getLatestPosition();
                    __instance.transform.rotation = Plugin.Multi.getLatestRotation();
                }
                else
                {
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
