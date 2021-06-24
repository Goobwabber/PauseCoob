using UnityEngine;
using Zenject;

namespace SlicePause.Managers
{
    public class CoobCutInfoManager : ISaberSwingRatingCounterDidFinishReceiver
    {
        protected SaberSwingRatingCounter.Pool _saberSwingRatingCounterPool = null!;

        [Inject]
        internal void Inject([InjectOptional] SaberSwingRatingCounter.Pool saberSwingRatingCounterPool)
        {
            _saberSwingRatingCounterPool = saberSwingRatingCounterPool;
        }

        public NoteCutInfo GetCutInfo(Transform noteTransform, Saber saber, Vector3 cutPoint, Quaternion orientation, Vector3 cutDirVec, float cutAngleTolerance)
        {
            if (_saberSwingRatingCounterPool != null)
            {
                bool speedOK;
                bool directionOK;
                bool saberTypeOK;
                float cutDirDeviation;
                float cutDirAngle;
                NoteBasicCutInfoHelper.GetBasicCutInfo(noteTransform, ColorType.ColorA, NoteCutDirection.Any, SaberType.SaberA, saber.bladeSpeed, cutDirVec, cutAngleTolerance, out directionOK, out speedOK, out saberTypeOK, out cutDirDeviation, out cutDirAngle);

                SaberSwingRatingCounter saberSwingRatingCounter = null!;
                if (speedOK && directionOK && saberTypeOK)
                {
                    saberSwingRatingCounter = _saberSwingRatingCounterPool.Spawn();
                    saberSwingRatingCounter.Init(saber.movementData, noteTransform);
                    saberSwingRatingCounter.RegisterDidFinishReceiver(this);
                }

                Vector3 vector = orientation * Vector3.up;
                Plane plane = new Plane(vector, cutPoint);
                float cutDistanceToCenter = Mathf.Abs(plane.GetDistanceToPoint(noteTransform.position));
                return new NoteCutInfo(speedOK, directionOK, saberTypeOK, false, saber.bladeSpeed, cutDirVec, saber.saberType, 0.01f, cutDirDeviation, plane.ClosestPointOnPlane(noteTransform.position), vector, cutDistanceToCenter, cutDirAngle, saberSwingRatingCounter);
            }
            else
                Plugin.Log?.Warn("SaberSwingRatingCounterPool not found!");

            return new NoteCutInfo();
        }

        public virtual void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
        {
            _saberSwingRatingCounterPool.Despawn((SaberSwingRatingCounter)saberSwingRatingCounter);
            saberSwingRatingCounter.UnregisterDidFinishReceiver(this);
        }
    }
}
