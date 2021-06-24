using SlicePause.Objects;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Managers
{
	public class MenuManager : MonoBehaviour, IInitializable, IDisposable
	{
		protected Coob _coob = null!;
		protected Floatie _floatie = null!;
		private const float cutTimeout = 0.5f;

		[Inject]
		internal void Inject(Coob coob)
		{
			_coob = coob;
		}

		public void OnEnable()
		{
			if (_coob != null)
				Initialize();
		}
		
		public void OnDisable() => Dispose();

		public void Initialize()
		{
			_coob.Respawn(0f);
			_coob.SetVisible(false);
			_coob.Refresh();
			_coob.cutable = false;

			_floatie = _coob.GetComponent<Floatie>();
			_floatie.Init(_coob.gameObject);

			//_coob.CoobWasCutEvent += HandleCoobWasCut;
			_floatie.OnRelease += HandleCoobWasMoved;

			Plugin.Log?.Info("Set up MenuManager.");
		}

		public void Dispose()
		{
			//_coob.CoobWasCutEvent -= HandleCoobWasCut;
			_floatie.OnRelease -= HandleCoobWasMoved;
			Plugin.Log?.Info("Yeeted MenuManager.");
		}

		//private void HandleCoobWasCut() => _coob.Respawn(cutTimeout);
		private void HandleCoobWasMoved(Vector3 position, Quaternion rotation) => _coob.SetPositionAndRotation(position, rotation);
	}
}
