using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace BSCM
{
    // void HitNote(XRNode node)
    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPatch("HitNote")]
    [HarmonyPatch(new Type[] {
        typeof(XRNode)})]
    internal static class HapticFeedbackControllerHitNote
    {
        static bool Prefix(HapticFeedbackController __instance, XRNode node)
        {
            if (PluginConfig.Instance.Enabled && PluginConfig.Instance.disableRumble)
            {
                if (node == XRNode.LeftHand && PluginConfig.Instance.isLeftRemoteSaber)
                    return false;
                else if (node == XRNode.RightHand && !PluginConfig.Instance.isLeftRemoteSaber)
                    return false;
            }
            return true;
        }
    }
}
