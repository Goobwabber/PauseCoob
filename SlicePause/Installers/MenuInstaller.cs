using SlicePause.Managers;
using SlicePause.Objects;
using SlicePause.UI;
using System;
using Zenject;

namespace SlicePause.Installers
{
	class MenuInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<CoobTab>().AsSingle();
			Container.BindInterfacesTo<MenuManager>().FromNewComponentOnRoot().AsSingle();
			Coob coob = Container.Resolve<Coob>();

			//Container.InstantiateComponentOnNewGameObject<MenuSaber>("CoobMenuSaber");

			if (coob.GetComponent<Floatie>() == null)
				Container.InstantiateComponent<Floatie>(coob.gameObject);

			Plugin.Log?.Info("Coob installed in menu scene.");
		}
	}
}
