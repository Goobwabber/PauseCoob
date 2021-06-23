using System;
using System.Linq;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SlicePause.Objects
{
    class Floatie : MonoBehaviour
    {
        protected const float MinScrollDistance = 0.25f;
        protected const float MaxLaserDistance = 50;

        protected VRPointer _vrPointer = null!;
        protected Transform _handle = null!;
        protected VRController _grabbingController = null!;
        protected Vector3 _grabPos;
        protected Quaternion _grabRot;
        protected Vector3 _realPos;
        protected Quaternion _realRot;
        protected FirstPersonFlyingController _fpfc = null!;

        public Action<Vector3, Quaternion> OnGrab = null!;
        public Action<Vector3, Quaternion> OnRelease = null!;

        public virtual void Init(GameObject handle)
        {
            _handle = handle.transform;
            _realPos = transform.position;
            _realRot = transform.rotation;
            _fpfc = Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().FirstOrDefault();
            _vrPointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
        }

        private bool IsFpfc => _fpfc != null && _fpfc.enabled;

        protected virtual void Update()
        {
            VRPointer pointer = _vrPointer;
            if (pointer?.vrController != null)
                if (pointer.vrController.triggerValue > 0.9f || Input.GetMouseButton(0))
                {
                    if (_grabbingController != null) return;
                    if (Physics.Raycast(pointer.vrController.position, pointer.vrController.forward, out RaycastHit hit, MaxLaserDistance))
                    {
                        Plugin.Log?.Info(hit.transform.name);
                        if (hit.transform != _handle && !hit.transform.IsChildOf(_handle)) return;
                        _grabbingController = pointer.vrController;
                        _grabPos = pointer.vrController.transform.InverseTransformPoint(transform.position);
                        _grabRot = Quaternion.Inverse(pointer.vrController.transform.rotation) * transform.rotation;
                        OnGrab?.Invoke(transform.position, transform.rotation);
                    }
                }

            if (_grabbingController == null || !IsFpfc && _grabbingController.triggerValue > 0.9f ||
                IsFpfc && Input.GetMouseButton(0)) return;
            _grabbingController = null!;
            OnRelease?.Invoke(transform.position, transform.rotation);
        }

        protected void OnDestroy()
        {
            OnGrab = null!;
            OnRelease = null!;
            _vrPointer = null!;
            _handle = null!;
            _grabbingController = null!;
        }

        protected virtual void LateUpdate()
        {
            if (_grabbingController != null)
            {
                float diff = _grabbingController.verticalAxisValue * Time.unscaledDeltaTime;
                if (_grabPos.magnitude > MinScrollDistance)
                {
                    _grabPos -= Vector3.forward * diff;
                }
                else
                {
                    _grabPos -= Vector3.forward * Mathf.Clamp(diff, float.MinValue, 0);
                }
                _realPos = _grabbingController.transform.TransformPoint(_grabPos);
                _realRot = _grabbingController.transform.rotation * _grabRot;
            }
            else return;


            transform.position = Vector3.Lerp(transform.position, _realPos, 10 * Time.unscaledDeltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, _realRot, 5 * Time.unscaledDeltaTime);
        }
    }
}
