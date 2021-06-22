using HarmonyLib;
using IPA;
using IPA.Loader;
using IPALogger = IPA.Logging.Logger;
using System.Net.Http;
using SlicePause.HarmonyPatches;
using SiraUtil.Zenject;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SlicePause.Installers;
using System;
using UnityEngine.SceneManagement;
using Zenject;
using IPA.Config;
using IPA.Config.Stores;

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

        internal static GameObject? coob;

        [Init]
        public Plugin(IPALogger logger, Config conf, Zenjector zenjector, PluginMetadata pluginMetadata)
        {
            Instance = this;
            PluginMetadata = pluginMetadata;
            Log = logger;
            Config = conf.Generated<PluginConfig>();

            zenjector.On((scene, context, container) => {
                if (scene.name == "ShaderWarmup")
                {
                    coob = UnityEngine.Object.Instantiate(GameObject.Find("NormalGameNote").transform.Find("NoteCube")).gameObject;
                    UnityEngine.Object.DontDestroyOnLoad(coob);
                    coob.SetActive(false);
                }

                return true;
            });

            zenjector.OnGame<CoobInstaller>();
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
