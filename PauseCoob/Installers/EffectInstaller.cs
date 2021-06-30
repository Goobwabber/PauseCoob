using IPA.Utilities;
using PauseCoob.Managers;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PauseCoob.Installers
{
	class EffectInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			EffectPoolsManualInstaller effectPoolsManualInstaller = Resources.FindObjectsOfTypeAll<EffectPoolsManualInstaller>().FirstOrDefault();
			if (effectPoolsManualInstaller == null)
				Plugin.Log?.Warn("EffectPoolsManualInstaller not found");

			FlyingScoreEffect flyingScoreEffect = effectPoolsManualInstaller.GetField<FlyingScoreEffect, EffectPoolsManualInstaller>("_flyingScoreEffectPrefab");
			FlyingScoreEffect coobFlyingScoreEffect = Object.Instantiate(flyingScoreEffect);
			coobFlyingScoreEffect.gameObject.SetActive(false);

			Container.BindMemoryPool<SaberSwingRatingCounter, SaberSwingRatingCounter.Pool>().WithInitialSize(5).WhenInjectedInto<CoobCutInfoManager>();
			Container.BindMemoryPool<FlyingScoreEffect, FlyingScoreEffect.Pool>().WithInitialSize(5).FromComponentInNewPrefab(coobFlyingScoreEffect).WhenInjectedInto<CoobFlyingScoreManager>();

			Container.Inject(Container.Resolve<CoobCutInfoManager>());
			Container.Inject(Container.Resolve<CoobFlyingScoreManager>());

			Plugin.Log?.Info("Coob installed in game scene.");
		}
	}
}