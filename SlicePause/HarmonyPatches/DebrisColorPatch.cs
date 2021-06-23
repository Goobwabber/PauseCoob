using HarmonyLib;
using UnityEngine;

namespace SlicePause.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorManager), nameof(ColorManager.ColorForType), MethodType.Normal)]
    internal class DebrisColorPatch
    {
        internal static bool enabled = false;

        static bool Prefix(ref Color __result)
        {
            if (enabled)
            {
                Color color;
                if (ColorUtility.TryParseHtmlString(Plugin.Config.Color, out color))
                {
                    __result = color;
                    return false;
                }
            }
            return true;
        }
    }
}
