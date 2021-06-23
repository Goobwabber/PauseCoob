using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using SlicePause.Objects;
using System;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace SlicePause.UI
{
    class CoobTab : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private GameplaySetupViewController gameplaySetupViewController;
        private Coob coob;
        public event PropertyChangedEventHandler PropertyChanged = null!;

        [UIComponent("root")]
        private readonly RectTransform rootTransform = null!;

        [UIComponent("modal")]
        private readonly RectTransform modalTransform = null!;

        [UIValue("show-coob")]
        public bool ShowCoobValue
        {
            get => coob.isActiveAndEnabled;
            set {
                if (value)
                    Plugin.Log?.Debug("Showing coob");

                if (coob != null)
                    coob.SetVisible(value);
                else
                    Plugin.Log?.Warn("Coob not found!");

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowCoobValue)));
            }
        }

        [UIValue("coob-color")]
        public Color CoobColor
        {
            get => coob.color;
            set => coob.color = value;
        }

        [UIValue("coob-scale")]
        public float CoobScale
        {
            get => coob.scale;
            set {
                coob.scale = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoobScale)));
            }
        }

        public CoobTab(Coob _coob, GameplaySetupViewController _gameplaySetupViewController)
        {
            coob = _coob;
            gameplaySetupViewController = _gameplaySetupViewController;
        }

        public void Initialize()
        {
            ShowCoobValue = false;
            GameplaySetup.instance.AddTab("Coob", "SlicePause.UI.CoobTab.bsml", this, MenuType.All);
            gameplaySetupViewController.didDeactivateEvent += YeetModalEvent;
            Plugin.Log?.Info("Installed Coob UI");
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab("Coob");
            gameplaySetupViewController.didDeactivateEvent -= YeetModalEvent;
            Plugin.Log?.Info("Yeeted Coob UI");
        }

        private void YeetModalEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (rootTransform != null && modalTransform != null)
            {
                modalTransform.SetParent(rootTransform);
                modalTransform.gameObject.SetActive(false);
            }
        }
    }
}
