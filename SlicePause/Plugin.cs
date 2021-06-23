using HarmonyLib;
using IPA;
using IPA.Loader;
using IPALogger = IPA.Logging.Logger;
using System.Net.Http;
using SlicePause.HarmonyPatches;
using SiraUtil.Zenject;
using UnityEngine;
using SlicePause.Installers;
using IPA.Config;
using IPA.Config.Stores;
using SlicePause.Objects;

namespace SlicePause
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static readonly string HarmonyId = "com.github.Goobwabber.SlicePause";

        internal static Plugin Instance { get; private set; } = null!;
        internal static PluginMetadata PluginMetadata = null!;
        internal static IPALogger Log { get; private set; } = null!;
        internal static PluginConfig Config = null!;

        internal static HttpClient HttpClient { get; private set; } = null!;
        internal static Harmony? _harmony;
        internal static Harmony Harmony
        {
            get
            {
                return _harmony ??= new Harmony(HarmonyId);
            }
        }

        [Init]
        public Plugin(IPALogger logger, Config conf, Zenjector zenjector, PluginMetadata pluginMetadata)
        {
            Instance = this;
            PluginMetadata = pluginMetadata;
            Log = logger;
            Config = conf.Generated<PluginConfig>();

            zenjector.OnMenu<MenuInstaller>();
            zenjector.OnGame<CoobInstaller>();
            zenjector.On((scene, context, container) => {
                if (scene.name == "ShaderWarmup")
                {
                    GameObject coobGO = Object.Instantiate(GameObject.Find("NormalGameNote").transform.Find("NoteCube")).gameObject;
                    Object.DontDestroyOnLoad(coobGO);
                    container.ParentContainers[0].Bind<Coob>().FromNewComponentOn(coobGO).AsSingle();

                    Log?.Info("Stole the coob!!");
                }

                return true;
            });
        }

        [OnStart]
        public void OnApplicationStart()
        {
            HarmonyManager.ApplyDefaultPatches();
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }
    }
}
