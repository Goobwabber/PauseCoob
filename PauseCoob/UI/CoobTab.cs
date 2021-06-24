using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using PauseCoob.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PauseCoob.UI
{
	class CoobTab : MonoBehaviour, IInitializable, IDisposable, INotifyPropertyChanged
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

		[UIComponent("cut-angle-tolerance")]
		private readonly IncrementSetting cutAngleTolerance = null!;

		[UIValue("coob-angle-tolerance")]
		public float CoobAngleToleranceValue
		{
			get => coob.cutAngleTolerance * 2;
			set
			{
				coob.cutAngleTolerance = value / 2;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoobAngleToleranceValue)));
			}
		}

		[UIValue("coob-type")]
		public Coob.CoobType CoobTypeValue
		{
			get => coob.type;
			set
			{
				coob.type = value;
				cutAngleTolerance.interactable = value == Coob.CoobType.Arrow;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoobTypeValue)));
			}
		}

		[UIValue("coob-type-options")]
		public List<object> CoobTypeOptions = new object[] { Coob.CoobType.None, Coob.CoobType.Arrow, Coob.CoobType.Circle }.ToList();

		[UIAction("coob-type-formatter")]
		public string CoobTypeFormatter(Coob.CoobType type) => type.ToString();

		[Inject]
		internal void Inject(Coob _coob, GameplaySetupViewController _gameplaySetupViewController)
		{
			coob = _coob;
			gameplaySetupViewController = _gameplaySetupViewController;
		}

		public void Initialize()
		{
			gameplaySetupViewController.didDeactivateEvent += YeetModalEvent;

			GameplaySetup.instance.AddTab("Coob", "PauseCoob.UI.CoobTab.bsml", this, MenuType.All);

			Plugin.Log?.Info("Installed Coob UI");
		}

		public void OnEnable()
		{
			if (coob != null)
				ShowCoobValue = false;
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
