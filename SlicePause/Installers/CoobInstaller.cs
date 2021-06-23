using SlicePause.Objects;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Installers
{
    class CoobInstaller : MonoInstaller
    {
        public override void InstallBindings() { }

        public override void Start()
        {
            Coob coob = Container.Resolve<Coob>();
            if (coob != null) {
                coob.SetVisible(true);

                BoxCuttableBySaber[] boxes = coob.GetComponentsInChildren<BoxCuttableBySaber>();

                BoxCuttableBySaber.WasCutBySaberDelegate pausepls = (Saber saber, Vector3 cutPoint, Quaternion orientation, Vector3 cutDirVec) =>
                {
                    Container.Resolve<PauseController>().Pause();
                };

                foreach(BoxCuttableBySaber box in boxes)
                {
                    box.wasCutBySaberEvent += pausepls;
                }
            }
        }
    }
}
