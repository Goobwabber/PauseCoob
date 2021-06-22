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
            if (Plugin.coob != null) {
                Plugin.coob.SetActive(true);
                Plugin.coob.transform.position = new Vector3(Plugin.Config.posX, Plugin.Config.posY, Plugin.Config.posZ);
                Plugin.coob.transform.rotation = new Quaternion(Plugin.Config.rotX, Plugin.Config.rotY, Plugin.Config.rotZ, Plugin.Config.rotW);

                BoxCuttableBySaber[] boxes = Plugin.coob.GetComponentsInChildren<BoxCuttableBySaber>();

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
