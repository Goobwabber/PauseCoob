using HarmonyLib;
using UnityEngine;

namespace PauseCoob.HarmonyPatches
{
	[HarmonyPatch(typeof(ColorManager), nameof(ColorManager.ColorForType), MethodType.Normal)]
	internal class DebrisColorPatch
	{
		internal static bool enabled = false;
		internal static Color color;

		static bool Prefix(ref Color __result)
		{
			if (enabled)
			{
				__result = color;
				return false;
			}
			return true;
		}
	}
}
