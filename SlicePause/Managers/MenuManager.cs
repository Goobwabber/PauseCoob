using SlicePause.Objects;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Managers
{
    class MenuManager : IInitializable, IDisposable
    {
        protected Coob _coob = null!;
        protected Floatie _floatie = null!;
        private const float cutTimeout = 0.5f;

        [Inject]
        internal void Inject(Coob coob)
        {
            _coob = coob;
        }

        public void Initialize()
        {
            _coob.Respawn(0f, false);
            _coob.Refresh();
            _coob.cutable = false;

            _floatie = _coob.GetComponent<Floatie>();
            _floatie.Init(_coob.gameObject);

            //_coob.CoobWasCutEvent += HandleCoobWasCut;
            _floatie.OnRelease += HandleCoobWasMoved;
        }

        public void Dispose()
        {
            //_coob.CoobWasCutEvent -= HandleCoobWasCut;
            _floatie.OnRelease -= HandleCoobWasMoved;
        }

        //private void HandleCoobWasCut() => _coob.Respawn(cutTimeout);
        private void HandleCoobWasMoved(Vector3 position, Quaternion rotation) => _coob.SetPositionAndRotation(position, rotation);
    }
}
