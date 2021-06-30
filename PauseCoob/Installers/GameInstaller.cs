using IPA.Utilities;
using PauseCoob.Managers;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PauseCoob.Installers
{
	class GameInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameplayManager>().AsSingle();

			EffectPoolsManualInstaller effectPoolsManualInstaller = Resources.FindObjectsOfTypeAll<EffectPoolsManualInstaller>().FirstOrDefault();
			if (effectPoolsManualInstaller == null)
				Plugin.Log?.Warn("EffectPoolsManualInstaller not found");

			BoolSO noteDebrisHD = effectPoolsManualInstaller.GetField<BoolSO, EffectPoolsManualInstaller>("_noteDebrisHDConditionVariable");
			NoteDebris noteDebris = effectPoolsManualInstaller.GetField<NoteDebris, EffectPoolsManualInstaller>(noteDebrisHD ? "_noteDebrisHDPrefab" : "_noteDebrisLWPrefab");
			NoteDebris coobNoteDebris = Object.Instantiate(noteDebris);
			coobNoteDebris.gameObject.SetActive(false);
			Container.BindMemoryPool<NoteDebris, NoteDebris.Pool>().WithInitialSize(10).FromComponentInNewPrefab(coobNoteDebris).WhenInjectedInto<CoobDebrisManager>();

			Container.Inject(Container.Resolve<CoobDebrisManager>());
		}
	}
}
