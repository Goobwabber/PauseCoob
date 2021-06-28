using HarmonyLib;
using System;

namespace PauseCoob.HarmonyPatches
{
	[HarmonyPatch(typeof(MultiplayerLocalActivePlayerInGameMenuController), nameof(MultiplayerLocalActivePlayerInGameMenuController.ShowInGameMenu), MethodType.Normal)]
	internal class ShowMultiplayerMenuPatch
	{
		internal static event Action didPauseEvent = null!;

		static void Postfix(bool ____gameMenuIsShown)
		{
			if (____gameMenuIsShown)
				didPauseEvent?.Invoke();
		}
	}

	[HarmonyPatch(typeof(MultiplayerLocalActivePlayerInGameMenuController), nameof(MultiplayerLocalActivePlayerInGameMenuController.HideInGameMenu), MethodType.Normal)]
	internal class HideMultiplayerMenuPatch
	{
		internal static event Action didHideEvent = null!;

		static void Postfix(bool ____gameMenuIsShown)
		{
			if (!____gameMenuIsShown)
				didHideEvent?.Invoke();
		}
	}
}
