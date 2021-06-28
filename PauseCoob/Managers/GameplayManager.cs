using PauseCoob.HarmonyPatches;
using PauseCoob.Objects;
using System;
using UnityEngine;
using Zenject;

namespace PauseCoob.Managers
{
	public class GameplayManager : IInitializable, IDisposable
	{
		protected readonly Coob _coob;
		protected readonly PauseController _pauseController;
		protected readonly MultiplayerLocalActivePlayerInGameMenuController _multiplayerPauseController;
		protected Floatie _floatie = null;
		private const float cutTimeout = 2f;

		internal GameplayManager(Coob coob, [InjectOptional] PauseController pauseController, [InjectOptional] MultiplayerLocalActivePlayerInGameMenuController multiplayerPauseController)
		{
			_coob = coob;
			_pauseController = pauseController;
			_multiplayerPauseController = multiplayerPauseController;
		}

		public void Initialize()
		{
			_coob.SetVisible(true);
			_coob.cutable = true;
			_floatie = _coob.GetComponent<Floatie>();

			_coob.CoobWasCutEvent += HandleCoobWasCut;

			if (_pauseController != null)
			{
				_pauseController.didPauseEvent += HandlePauseEvent;
				_pauseController.didResumeEvent += HandleResumeEvent;
			}

			ShowMultiplayerMenuPatch.didPauseEvent += HandlePauseEvent;
			HideMultiplayerMenuPatch.didHideEvent += HandleResumeEvent;

			Plugin.Log?.Info("Set up GameplayManager.");
		}

		public void Dispose()
		{
			_coob.CoobWasCutEvent -= HandleCoobWasCut;

			if (_pauseController != null)
			{
				_pauseController.didPauseEvent -= HandlePauseEvent;
				_pauseController.didResumeEvent -= HandleResumeEvent;
			}

			ShowMultiplayerMenuPatch.didPauseEvent -= HandlePauseEvent;
			HideMultiplayerMenuPatch.didHideEvent -= HandleResumeEvent;

			Plugin.Log?.Info("Yeeted GameplayManager.");
		}

		private void HandleCoobWasCut()
		{
			_pauseController?.Pause();
			_multiplayerPauseController?.ShowInGameMenu();
		}

		private void HandlePauseEvent() => _coob.Despawn();
		private void HandleResumeEvent() => _coob.Respawn(cutTimeout, true, true);
	}
}
