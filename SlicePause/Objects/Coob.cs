using IPA.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SlicePause.Objects
{
    public class Coob : MonoBehaviour, ISaberSwingRatingCounterDidFinishReceiver
    {
        MeshRenderer renderer = null!;
        Material material = null!;
        Collider collider = null!;
        CutoutAnimateEffect animateEffect = null!;
        BoxCuttableBySaber[] boxes = null!;
        NoteCutScoreSpawner noteCutScoreSpawner = null!;
        FlyingScoreSpawner flyingScoreSpawner = null!;
        SaberSwingRatingCounter.Pool saberSwingRatingCounterPool = null!;
        NoteCutCoreEffectsSpawner noteCutCoreEffectsSpawner = null!;
        NoteDebrisSpawner noteDebrisSpawner = null!;

        bool _cutable = false;
        bool _visible = false;

        const float cutoutTime = 1f;

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

        public Color color {
            get => material.color;
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
        internal void Inject([InjectOptional]NoteCutScoreSpawner _noteCutScoreSpawner, [InjectOptional]SaberSwingRatingCounter.Pool _saberSwingRatingCounterPool)
        {
            noteCutScoreSpawner = _noteCutScoreSpawner;
            saberSwingRatingCounterPool = _saberSwingRatingCounterPool;

            if (noteCutScoreSpawner != null)
            {
                flyingScoreSpawner = noteCutScoreSpawner.GetField<FlyingScoreSpawner, NoteCutScoreSpawner>("_flyingScoreSpawner");
            }

            noteDebrisSpawner = Resources.FindObjectsOfTypeAll<NoteDebrisSpawner>().FirstOrDefault();
        }

        public void Awake()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            renderer = GetComponent<MeshRenderer>();
            material = renderer.material;
            animateEffect = gameObject.AddComponent<CutoutAnimateEffect>();
            CutoutEffect[] cutoutEffects = GetComponents<CutoutEffect>();
            animateEffect.SetField<CutoutAnimateEffect, CutoutEffect[]>("_cuttoutEffects", cutoutEffects);
            boxes = GetComponentsInChildren<BoxCuttableBySaber>();

            foreach (BoxCuttableBySaber box in boxes)
            {
                box.wasCutBySaberEvent += WasCutBySaber;
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
            renderer.enabled = value;

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = value;
            }
            collider.enabled = value;
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
            material.color = color;
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

            SetVisible(_visible);
        }

        public void Refresh()
        {
            Vector3 position = new Vector3(Plugin.Config.PosX, Plugin.Config.PosY, Plugin.Config.PosZ);
            Quaternion rotation = new Quaternion(Plugin.Config.RotX, Plugin.Config.RotY, Plugin.Config.RotZ, Plugin.Config.RotW);
            SetPositionAndRotation(position, rotation);
            SetScale(Plugin.Config.Scale);
            SetType((CoobType)Plugin.Config.Type);

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
                cutable = false;
                StartCoroutine(DespawnCoob());

                if (saberSwingRatingCounterPool != null && flyingScoreSpawner != null)
                {
                    bool speedOK;
                    bool directionOK;
                    bool saberTypeOK;
                    float cutDirDeviation;
                    float cutDirAngle;
                    NoteBasicCutInfoHelper.GetBasicCutInfo(this.transform, ColorType.ColorA, NoteCutDirection.Any, SaberType.SaberA, saber.bladeSpeed, cutDirVec, 90f, out directionOK, out speedOK, out saberTypeOK, out cutDirDeviation, out cutDirAngle);

                    SaberSwingRatingCounter saberSwingRatingCounter = null;
                    if (speedOK && directionOK && saberTypeOK)
                    {
                        saberSwingRatingCounter = saberSwingRatingCounterPool.Spawn();
                        saberSwingRatingCounter.Init(saber.movementData, transform);
                        saberSwingRatingCounter.RegisterDidFinishReceiver(this);
                    }

                    Vector3 vector = orientation * Vector3.up;
                    Plane plane = new Plane(vector, cutPoint);
                    float cutDistanceToCenter = Mathf.Abs(plane.GetDistanceToPoint(transform.position));
                    NoteCutInfo noteCutInfo = new NoteCutInfo(speedOK, directionOK, saberTypeOK, false, saber.bladeSpeed, cutDirVec, saber.saberType, 0.01f, cutDirDeviation, plane.ClosestPointOnPlane(transform.position), vector, cutDistanceToCenter, cutDirAngle, saberSwingRatingCounter);

                    flyingScoreSpawner.SpawnFlyingScore(in noteCutInfo, 0, 8, transform.position, transform.rotation, Quaternion.Inverse(transform.rotation), color);

                    if (noteDebrisSpawner != null)
                    {
                        HarmonyPatches.DebrisColorPatch.enabled = true;
                        noteDebrisSpawner.SpawnDebris(noteCutInfo.cutPoint, noteCutInfo.cutNormal, noteCutInfo.saberSpeed, noteCutInfo.saberDir, transform.position, transform.rotation, transform.localScale, ColorType.ColorA, 10f, new Vector3(0, 1, 0));
                        HarmonyPatches.DebrisColorPatch.enabled = false;
                    }
                    else
                        Plugin.Log?.Warn("Debris Spawner not found.");
                }
                else
                    Plugin.Log?.Warn("Requirements for block cut not found.");

                CoobWasCutEvent?.Invoke();
            }
        }

        public virtual void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
        {
            saberSwingRatingCounterPool.Despawn((SaberSwingRatingCounter)saberSwingRatingCounter);
            saberSwingRatingCounter.UnregisterDidFinishReceiver(this);
        }

        public void Respawn(float delay, bool visibility = true)
        {
            StartCoroutine(RespawnCoob(delay, visibility));
        }

        public void Despawn()
        {
            StartCoroutine(DespawnCoob());
        }

        IEnumerator DespawnCoob()
        {
            yield return animateEffect.AnimateToCutoutCoroutine(0f, 1f, Time.deltaTime * cutoutTime);
            SetVisible(false);
            yield return null;
        }

        IEnumerator RespawnCoob(float delay, bool visibility)
        {
            yield return new WaitForSeconds(delay);
            yield return animateEffect.AnimateToCutoutCoroutine(1f, 0f, Time.deltaTime * cutoutTime);
            SetVisible(visibility);
            cutable = visibility;
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
