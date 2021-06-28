using IPA.Utilities;
using PauseCoob.Managers;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PauseCoob.Installers
{
	class GameInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameplayManager>().AsSingle();

			DiContainer AppContainer = Container.ParentContainers[0];

			EffectPoolsManualInstaller effectPoolsManualInstaller = Resources.FindObjectsOfTypeAll<EffectPoolsManualInstaller>().FirstOrDefault();
			if (effectPoolsManualInstaller == null)
				Plugin.Log?.Warn("EffectPoolsManualInstaller not found");

			FlyingScoreEffect flyingScoreEffect = effectPoolsManualInstaller.GetField<FlyingScoreEffect, EffectPoolsManualInstaller>("_flyingScoreEffectPrefab");
			BoolSO noteDebrisHD = effectPoolsManualInstaller.GetField<BoolSO, EffectPoolsManualInstaller>("_noteDebrisHDConditionVariable");
			NoteDebris noteDebris = effectPoolsManualInstaller.GetField<NoteDebris, EffectPoolsManualInstaller>(noteDebrisHD ? "_noteDebrisHDPrefab" : "_noteDebrisLWPrefab");

			FlyingScoreEffect coobFlyingScoreEffect = Object.Instantiate(flyingScoreEffect);
			NoteDebris coobNoteDebris = Object.Instantiate(noteDebris);

			Object.DontDestroyOnLoad(coobFlyingScoreEffect);
			Object.DontDestroyOnLoad(coobNoteDebris);

			coobFlyingScoreEffect.gameObject.SetActive(false);
			coobNoteDebris.gameObject.SetActive(false);

			AppContainer.BindMemoryPool<SaberSwingRatingCounter, SaberSwingRatingCounter.Pool>().WithInitialSize(5).WhenInjectedInto<CoobCutInfoManager>();
			AppContainer.BindMemoryPool<FlyingScoreEffect, FlyingScoreEffect.Pool>().WithInitialSize(5).FromComponentInNewPrefab(coobFlyingScoreEffect).WhenInjectedInto<CoobFlyingScoreManager>();
			AppContainer.BindMemoryPool<NoteDebris, NoteDebris.Pool>().WithInitialSize(10).FromComponentInNewPrefab(coobNoteDebris).WhenInjectedInto<CoobDebrisManager>();

			Container.Inject(Container.Resolve<CoobCutInfoManager>());
			Container.Inject(Container.Resolve<CoobFlyingScoreManager>());
			Container.Inject(Container.Resolve<CoobDebrisManager>());

			Plugin.Log?.Info("Coob installed in game scene.");
		}
	}
}