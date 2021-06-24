using IPA.Utilities;
using System;
using System.Linq;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SlicePause.Objects
{
    public class MenuSaber : Saber
    {
        protected VRPointer _vrPointer = null!;
        protected NoteCutter _noteCutter = null!;

        public void Awake()
        {
            _noteCutter = new NoteCutter();
            _vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
            SaberTypeObject saberTypeObject = gameObject.AddComponent<SaberTypeObject>();
            saberTypeObject.SetField<SaberTypeObject, SaberType>("_saberType", SaberType.SaberA);
            _saberType = saberTypeObject;
        }

        public void Update()
        {
            ManualUpdate();
            _noteCutter.Cut(this);
        }

        public override void ManualUpdate()
        {
            if (!gameObject.activeInHierarchy)
                return;
            _handlePos = _vrPointer.vrController.position;
            _handleRot = _vrPointer.vrController.rotation;
            _saberBladeTopPos = _vrPointer.cursorPosition;
            _saberBladeBottomPos = _vrPointer.vrController.position;
            _movementData.AddNewData(_saberBladeTopPos, _saberBladeBottomPos, TimeHelper.time);
        }

        public override void OverridePositionAndRotation(Vector3 pos, Quaternion rot)
        {
            _handlePos = pos;
            _handleRot = rot;
        }

        
    }
}
