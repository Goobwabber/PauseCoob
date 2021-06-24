using PauseCoob.Managers;
using PauseCoob.Objects;
using PauseCoob.UI;
using System;
using Zenject;

namespace PauseCoob.Installers
{
	class MenuInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<CoobTab>().FromNewComponentOnRoot().AsSingle();
			Container.BindInterfacesTo<MenuManager>().FromNewComponentOnRoot().AsSingle();
			Coob coob = Container.Resolve<Coob>();

			//Container.InstantiateComponentOnNewGameObject<MenuSaber>("CoobMenuSaber");

			if (coob.GetComponent<Floatie>() == null)
				Container.InstantiateComponent<Floatie>(coob.gameObject);

			Plugin.Log?.Info("Coob installed in menu scene.");
		}
	}
}
