using SlicePause.Objects;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Managers
{
	public class GameplayManager : IInitializable, IDisposable
	{
		protected readonly Coob _coob;
		protected readonly PauseController _pauseController;
		protected Floatie _floatie = null;
		private const float cutTimeout = 2f;

		internal GameplayManager(Coob coob, PauseController pauseController)
		{
			_coob = coob;
			_pauseController = pauseController;
		}

		public void Initialize()
		{
			_coob.SetVisible(true);
			_coob.cutable = true;
			_floatie = _coob.GetComponent<Floatie>();

			_coob.CoobWasCutEvent += HandleCoobWasCut;
			_pauseController.didPauseEvent += HandlePauseEvent;
			_pauseController.didResumeEvent += HandleResumeEvent;

			Plugin.Log?.Info("Set up GameplayManager.");
		}

		public void Dispose()
		{
			_coob.CoobWasCutEvent -= HandleCoobWasCut;
			_pauseController.didPauseEvent -= HandlePauseEvent;
			_pauseController.didResumeEvent -= HandleResumeEvent;

			Plugin.Log?.Info("Yeeted GameplayManager.");
		}

		private void HandleCoobWasCut() => _pauseController.Pause();
		private void HandlePauseEvent() => _coob.Despawn();
		private void HandleResumeEvent() => _coob.Respawn(cutTimeout, true, true);
	}
}
