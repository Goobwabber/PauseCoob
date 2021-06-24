using SlicePause.Managers;
using SlicePause.Objects;
using UnityEngine;
using Zenject;

namespace SlicePause.Installers
{
    class WarmupInstaller : MonoInstaller
    {
        GameObject coobGO = null!;

        public override void InstallBindings()
        {
            Plugin.Log?.Info("Starting ShaderWarmup");
            coobGO = Object.Instantiate(GameObject.Find("NormalGameNote").transform.Find("NoteCube")).gameObject;
            Object.DontDestroyOnLoad(coobGO);

            DiContainer AppContainer = Container.ParentContainers[0];

            AppContainer.BindInterfacesAndSelfTo<CoobCutInfoManager>().AsSingle();
            AppContainer.BindInterfacesAndSelfTo<CoobDebrisManager>().AsSingle();
            AppContainer.BindInterfacesAndSelfTo<CoobFlyingScoreManager>().AsSingle();
            AppContainer.Bind<Coob>().FromNewComponentOn(coobGO).AsSingle();

            Plugin.Log?.Info("ShaderWarmup finished.");
        }
    }
}
