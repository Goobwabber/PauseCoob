using SlicePause.Managers;
using SlicePause.Objects;
using SlicePause.UI;
using System.Linq;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SlicePause.Installers
{
    class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CoobTab>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuManager>().AsSingle();
            Coob coob = Container.Resolve<Coob>();

            //Container.InstantiateComponentOnNewGameObject<MenuSaber>("CoobMenuSaber");

            if (coob.GetComponent<Floatie>() == null)
                Container.InstantiateComponent<Floatie>(coob.gameObject);

            Plugin.Log?.Info("Coob installed in menu scene.");
        }
    }
}
