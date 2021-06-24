using SlicePause.HarmonyPatches;
using System;
using UnityEngine;
using Zenject;

namespace SlicePause.Managers
{
    public class CoobDebrisManager : INoteDebrisDidFinishEvent
    {
        protected NoteDebris.Pool _noteDebrisPool = null!;

        [Inject]
        internal void Inject([InjectOptional] NoteDebris.Pool noteDebrisPool)
        {
            _noteDebrisPool = noteDebrisPool;
        }

        public void SpawnDebris(Transform noteTransform, NoteCutInfo noteCutInfo, Color noteColor, Vector3 noteMoveVec, float lifetime = 2f, float separationSpeed = 2f, float cutSpeedMultiplier = 0.1f, float torqueMultiplier = 2f)
        {
            if (_noteDebrisPool != null)
            {
                NoteDebris noteDebris = _noteDebrisPool.Spawn();
                noteDebris.didFinishEvent.Add(this);
                noteDebris.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                NoteDebris noteDebris2 = _noteDebrisPool.Spawn();
                noteDebris2.didFinishEvent.Add(this);
                noteDebris2.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                float magnitude = noteMoveVec.magnitude;
                Vector3 vector = Vector3.ProjectOnPlane(noteCutInfo.saberDir, noteMoveVec / magnitude);
                Vector3 vector2 = vector * (noteCutInfo.saberSpeed * cutSpeedMultiplier) + noteMoveVec;

                Vector3 force = Quaternion.identity * (-(noteCutInfo.cutNormal + UnityEngine.Random.onUnitSphere * 0.1f) * separationSpeed + vector2);
                Vector3 force2 = Quaternion.identity * ((noteCutInfo.cutNormal + UnityEngine.Random.onUnitSphere * 0.1f) * separationSpeed + vector2);
                Vector3 torque = Quaternion.identity * Vector3.Cross(noteCutInfo.cutNormal, vector) * torqueMultiplier;

                DebrisColorPatch.enabled = true;
                DebrisColorPatch.color = noteColor;

                noteDebris.Init(ColorType.None, noteTransform.position, noteTransform.rotation, noteMoveVec, noteTransform.localScale, Vector3.zero, Quaternion.identity, noteCutInfo.cutPoint, -noteCutInfo.cutNormal, force, -torque, lifetime);
                noteDebris2.Init(ColorType.None, noteTransform.position, noteTransform.rotation, noteMoveVec, noteTransform.localScale, Vector3.zero, Quaternion.identity, noteCutInfo.cutPoint, noteCutInfo.cutNormal, force2, torque, lifetime);

                DebrisColorPatch.enabled = false;
            }
            else
                Plugin.Log?.Warn("NoteDebrisPool not found!");
        }

        public void HandleNoteDebrisDidFinish(NoteDebris noteDebris)
        {
            noteDebris.didFinishEvent.Remove(this);
            _noteDebrisPool.Despawn(noteDebris);
        }
    }
}
