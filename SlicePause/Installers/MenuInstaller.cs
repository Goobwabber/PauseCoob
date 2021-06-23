using SlicePause.Objects;
using SlicePause.UI;
using Zenject;

namespace SlicePause.Installers
{
    class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CoobTab>().AsSingle();
            Coob coob = Container.Resolve<Coob>();

            if (coob != null)
            {
                coob.Respawn(0f, false);
                coob.Refresh();
                coob.cutable = false;

                if (coob.GetComponent<Floatie>() == null)
                {
                    Floatie floatie = Container.InstantiateComponent<Floatie>(coob.gameObject);
                    floatie.Init(coob.gameObject);

                    floatie.OnRelease += (position, rotation) =>
                    {
                        coob.SetPositionAndRotation(position, rotation);
                    };

                    floatie.OnGrab += (position, rotation) =>
                    {
                        Plugin.Log?.Info("Grabbed the coob!");
                    };

                    Plugin.Log?.Info("Installed floatie!");
                }
            }
            else
                Plugin.Log?.Warn("Coob not found!");
        }
    }
}
