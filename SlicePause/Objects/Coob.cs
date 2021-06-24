using IPA.Utilities;
using SlicePause.Managers;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace SlicePause.Objects
{
	public class Coob : MonoBehaviour, IDisposable
	{
		private MeshRenderer _renderer = null!;
		private Collider _collider = null!;
		private CutoutAnimateEffect _animateEffect = null!;
		private BoxCuttableBySaber[] _cutableBoxes = null!;
		private BoxCuttableBySaber _mainBox = null!;

		private LineRenderer _hitBoxRenderer = null!;
		private LineRenderer _hitAngleRenderer = null!;

		protected CoobCutInfoManager _coobCutInfoManager = null!;
		protected CoobDebrisManager _coobDebrisManager = null!;
		protected CoobFlyingScoreManager _coobFlyingScoreManager = null!;

		private bool _cutable = false;
		private bool _visible = false;
		private float _cutAngleTolerance = 90f;

		public bool anyDirection => type != CoobType.Arrow;
		public float cutAngleTolerance
		{
			get => _cutAngleTolerance;
			set
			{
				_cutAngleTolerance = value;
				Plugin.Config.CutAngleTolerance = _cutAngleTolerance;
			}
		}

		const float cutoutTime = 1f;
		Color debugColor = Color.white;

		public event Action CoobWasCutEvent = null!;

		public bool visible
		{
			get => _visible;
			set => SetVisible(value);
		}

		public bool cutable
		{
			get => _cutable;
			set => SetCutable(value);
		}

		public Color color
		{
			get => _renderer.material.color;
			set => SetColor(value);
		}

		public float scale
		{
			get => Plugin.Config.Scale;
			set => SetScale(value);
		}

		public CoobType type
		{
			get => (CoobType)Plugin.Config.Type;
			set => SetType(value);
		}

		[Inject]
		internal void Inject(CoobCutInfoManager coobCutInfoManager, CoobDebrisManager coobDebrisManager, CoobFlyingScoreManager coobFlyingScoreManager)
		{
			_coobCutInfoManager = coobCutInfoManager;
			_coobDebrisManager = coobDebrisManager;
			_coobFlyingScoreManager = coobFlyingScoreManager;
		}

		public void Awake()
		{
			gameObject.layer = 0;
			_collider = gameObject.AddComponent<BoxCollider>();
			_renderer = GetComponent<MeshRenderer>();
			_animateEffect = gameObject.AddComponent<CutoutAnimateEffect>();
			CutoutEffect[] cutoutEffects = GetComponents<CutoutEffect>();
			_animateEffect.SetField<CutoutAnimateEffect, CutoutEffect[]>("_cuttoutEffects", cutoutEffects);
			_cutableBoxes = GetComponentsInChildren<BoxCuttableBySaber>();
			_mainBox = transform.Find("BigCuttable").GetComponent<BoxCuttableBySaber>();

			foreach (BoxCuttableBySaber box in _cutableBoxes)
			{
				box.wasCutBySaberEvent += WasCutBySaber;
			}
		}

		public void Dispose()
		{
			foreach (BoxCuttableBySaber box in _cutableBoxes)
			{
				box.wasCutBySaberEvent -= WasCutBySaber;
			}
		}

		public void SetVisible(bool value)
		{
			_visible = value;
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			foreach (Renderer render in renderers)
			{
				render.enabled = value;
			}
			_renderer.enabled = value;

			Collider[] colliders = GetComponentsInChildren<Collider>();
			foreach (Collider col in colliders)
			{
				col.enabled = value;
			}
			_collider.enabled = value;
		}

		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			transform.position = position;
			Plugin.Config.PosX = position.x;
			Plugin.Config.PosY = position.y;
			Plugin.Config.PosZ = position.z;

			transform.rotation = rotation;
			Plugin.Config.RotX = rotation.x;
			Plugin.Config.RotY = rotation.y;
			Plugin.Config.RotZ = rotation.z;
			Plugin.Config.RotW = rotation.w;
		}

		public void SetColor(Color color)
		{
			_renderer.material.color = color;
			Plugin.Config.Color = "#" + ColorUtility.ToHtmlStringRGBA(color);
		}

		public void SetScale(float scale)
		{
			transform.localScale = Vector3.one * scale;
			Plugin.Config.Scale = scale;
		}

		public void SetType(CoobType type)
		{
			transform.Find("NoteArrow").gameObject.SetActive(type == CoobType.Arrow);
			transform.Find("NoteArrowGlow").gameObject.SetActive(type == CoobType.Arrow);
			transform.Find("NoteCircleGlow").gameObject.SetActive(type == CoobType.Circle);

			SetVisible(_visible);

			Plugin.Config.Type = (int)type;
		}

		public void SetCutable(bool value)
		{
			_cutable = value;
			transform.Find("BigCuttable").gameObject.SetActive(value);
			transform.Find("SmallCuttable").gameObject.SetActive(value);
			//GetComponent<Floatie>().enabled = !value;

			SetVisible(_visible);
		}

		public void Refresh()
		{
			Vector3 position = new Vector3(Plugin.Config.PosX, Plugin.Config.PosY, Plugin.Config.PosZ);
			Quaternion rotation = new Quaternion(Plugin.Config.RotX, Plugin.Config.RotY, Plugin.Config.RotZ, Plugin.Config.RotW);
			SetPositionAndRotation(position, rotation);
			SetScale(Plugin.Config.Scale);
			SetType((CoobType)Plugin.Config.Type);

			cutAngleTolerance = Plugin.Config.CutAngleTolerance;

			Color color;
			if (ColorUtility.TryParseHtmlString(Plugin.Config.Color, out color))
			{
				SetColor(color);
			}
		}

		private void WasCutBySaber(Saber saber, Vector3 cutPoint, Quaternion orientation, Vector3 cutDirVec)
		{
			if (cutable == true)
			{
				NoteCutInfo noteCutInfo;
				if (_coobCutInfoManager != null && _coobCutInfoManager.GetCutInfo(this.transform, saber, cutPoint, orientation, cutDirVec, cutAngleTolerance, anyDirection, out noteCutInfo))
				{
					if (_coobFlyingScoreManager != null)
						_coobFlyingScoreManager.SpawnFlyingScore(this.transform, in noteCutInfo, Vector3.up, color, 0.7f, 1);

					if (_coobDebrisManager != null)
						_coobDebrisManager.SpawnDebris(this.transform, noteCutInfo, color, Vector3.up);

					cutable = false;
					StartCoroutine(DespawnCoob());
					CoobWasCutEvent?.Invoke();
				}
			}
		}

		public void Respawn(float delay, bool cuttable = false, bool visibility = false)
		{
			StartCoroutine(RespawnCoob(delay, cuttable, visibility));
		}

		public void Despawn()
		{
			StartCoroutine(DespawnCoob());
		}

		IEnumerator DespawnCoob()
		{
			yield return _animateEffect.AnimateToCutoutCoroutine(0f, 1f, Time.deltaTime * cutoutTime);
			SetVisible(false);
			yield return null;
		}

		IEnumerator RespawnCoob(float delay, bool cuttable, bool visibility)
		{
			yield return new WaitForSeconds(delay);
			yield return _animateEffect.AnimateToCutoutCoroutine(1f, 0f, Time.deltaTime * cutoutTime);
			SetVisible(visibility);
			SetCutable(cuttable);
			yield return null;
		}

		public enum CoobType
		{
			None,
			Arrow,
			Circle
		}
	}
}
