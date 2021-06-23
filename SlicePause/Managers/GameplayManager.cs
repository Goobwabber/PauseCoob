using SlicePause.Objects;
using System;
using Zenject;

namespace SlicePause.Managers
{
    class GameplayManager : IInitializable, IDisposable
    {
        protected Coob _coob = null!;
        protected PauseController _pauseController = null!;
        private const float cutTimeout = 2f;

        [Inject]
        internal void Inject(Coob coob, PauseController pauseController)
        {
            _coob = coob;
            _pauseController = pauseController;
        }

        public void Initialize()
        {
            _coob.SetVisible(true);
            _coob.cutable = true;

            _coob.CoobWasCutEvent += HandleCoobWasCut;
            _pauseController.didPauseEvent += HandlePauseEvent;
            _pauseController.didResumeEvent += HandleResumeEvent;
        }

        public void Dispose()
        {
            _coob.CoobWasCutEvent -= HandleCoobWasCut;
            _pauseController.didPauseEvent -= HandlePauseEvent;
            _pauseController.didResumeEvent -= HandleResumeEvent;
        }

        private void HandleCoobWasCut() => _pauseController.Pause();
        private void HandlePauseEvent() => _coob.Despawn();
        private void HandleResumeEvent() => _coob.Respawn(cutTimeout);
    }
}
