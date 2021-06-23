using SlicePause.Managers;
using SlicePause.Objects;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Installers
{
    class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Inject(Container.Resolve<CoobCutInfoManager>());
            Container.Inject(Container.Resolve<CoobDebrisManager>());
            Container.Inject(Container.Resolve<CoobFlyingScoreManager>());
            Container.Inject(Container.Resolve<Coob>());
            Container.BindInterfacesAndSelfTo<GameplayManager>().AsSingle();

            Plugin.Log?.Info("Coob installed in game scene.");
        }
    }
}