using UnityEngine;
using Zenject;

namespace SlicePause.Managers
{
	public class CoobFlyingScoreManager : IFlyingObjectEffectDidFinishEvent
	{
		protected FlyingScoreEffect.Pool _flyingScoreEffectPool = null!;

		[Inject]
		internal void Inject([InjectOptional] FlyingScoreEffect.Pool flyingScoreEffectPool)
		{
			_flyingScoreEffectPool = flyingScoreEffectPool;
		}

		public void SpawnFlyingScore(Transform noteTransform, in NoteCutInfo noteCutInfo, Vector3 targetOffset, Color color, float duration, int multiplier)
		{
			if (_flyingScoreEffectPool != null)
			{
				FlyingScoreEffect flyingScoreEffect = _flyingScoreEffectPool.Spawn();
				flyingScoreEffect.didFinishEvent.Add(this);
				flyingScoreEffect.transform.position = noteTransform.position;
				flyingScoreEffect.InitAndPresent(noteCutInfo, multiplier, duration, targetOffset + noteTransform.position, Quaternion.LookRotation(noteTransform.position * -1f, Vector3.up), color);
			}
			else
				Plugin.Log?.Warn("FlyingScoreEffectPool not found!");
		}

		public void HandleFlyingObjectEffectDidFinish(FlyingObjectEffect flyingObjectEffect)
		{
			flyingObjectEffect.didFinishEvent.Remove(this);
			_flyingScoreEffectPool.Despawn(flyingObjectEffect as FlyingScoreEffect);
		}
	}
}
