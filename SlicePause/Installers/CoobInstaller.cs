using SlicePause.Objects;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Installers
{
    class CoobInstaller : MonoInstaller
    {
        const float cutTimeout = 2f;

        public override void InstallBindings() { }

        public override void Start()
        {
            Coob coob = Container.Resolve<Coob>();
            Container.Inject(coob);

            PauseController pauseController = Container.Resolve<PauseController>();

            if (coob != null) {
                coob.SetVisible(true);
                coob.cutable = true;
                coob.CoobWasCutEvent += () => pauseController.Pause();
                pauseController.didResumeEvent += () => coob.Respawn(cutTimeout);
                pauseController.didPauseEvent += () => coob.Despawn();
            }
        }
    }
}
