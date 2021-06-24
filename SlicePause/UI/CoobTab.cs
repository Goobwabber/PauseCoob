using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using SlicePause.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
			get => coob.visible;
			set
			{
				coob.visible = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowCoobValue)));
			}
		}

		[UIValue("coob-color")]
		public Color CoobColorValue
		{
			get => coob.color;
			set => coob.color = value;
		}

		[UIValue("coob-scale")]
		public float CoobScaleValue
		{
			get => coob.scale;
			set
			{
				coob.scale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoobScaleValue)));
			}
		}

		[UIValue("coob-type")]
		public Coob.CoobType CoobTypeValue
		{
			get => coob.type;
			set
			{
				coob.type = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoobTypeValue)));
			}
		}

		[UIValue("coob-type-options")]
		public List<object> CoobTypeOptions = new object[] { Coob.CoobType.None, Coob.CoobType.Arrow, Coob.CoobType.Circle }.ToList();

		[UIAction("coob-type-formatter")]
		public string CoobTypeFormatter(Coob.CoobType type) => type.ToString();

		public CoobTab(Coob _coob, GameplaySetupViewController _gameplaySetupViewController)
		{
			coob = _coob;
			gameplaySetupViewController = _gameplaySetupViewController;
		}

		public void Initialize()
		{
			gameplaySetupViewController.didDeactivateEvent += YeetModalEvent;

			GameplaySetup.instance.AddTab("Coob", "SlicePause.UI.CoobTab.bsml", this, MenuType.All);

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
