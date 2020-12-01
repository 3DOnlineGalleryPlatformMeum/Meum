﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Game.Player
{
    [RequireComponent(typeof(Camera))]
    public class LocalPlayerCamera : MonoBehaviour
    {
        #region SerializeFields
        
        [SerializeField] private float sensitivity;
        [SerializeField] private float cameraRotationLimit;
        [SerializeField] private Transform cameraPivot;
        [Header("Switching")] 
        [SerializeField] private Transform firstPersonCamTransform;
        [SerializeField] private Transform thirdPersonCamTransform;
        [SerializeField] private float switchingDuration;
        
        #endregion

        #region PublicFields
        
        public bool IsFirstPersonView { get; private set; }
        public bool IsSwitchingView
        {
            get { return !ReferenceEquals(_switching, null); }
        }
        
        #endregion

        #region PrivateFields
        
        private Vector3 _defaultEulerAngle;
        private Vector3 _cameraRotationDelta;
        private Camera _camera;
        private Transform _transform;
        private IEnumerator _switching = null;
        private bool _isRotateEnabled;
        
        #endregion

        private void Awake()
        {
            Assert.IsNotNull(cameraPivot);
            Assert.IsNotNull(firstPersonCamTransform);
            Assert.IsNotNull(thirdPersonCamTransform);
            
            _camera = GetComponent<Camera>();
            _transform = transform;

            IsFirstPersonView = false;
            _transform.localPosition = thirdPersonCamTransform.localPosition;
            _transform.localRotation = thirdPersonCamTransform.localRotation;
            _defaultEulerAngle = _transform.localEulerAngles;
            _camera.cullingMask = ~0;
        }

        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (IsSwitchingView) return;    // 인칭 전환중이라면 아무것도 안함
            if (!_isRotateEnabled) return;
            
            var value = ctx.ReadValue<Vector2>();
            _cameraRotationDelta.x -= value.y * sensitivity;
            _cameraRotationDelta.x = Mathf.Clamp(
                _cameraRotationDelta.x, 
                -cameraRotationLimit, 
                cameraRotationLimit);

            if (!IsFirstPersonView)
                cameraPivot.Rotate(Vector3.up, value.x * sensitivity);

            var newCameraRotation = _defaultEulerAngle;
            newCameraRotation += _cameraRotationDelta;
            _transform.localEulerAngles = newCameraRotation;
        }

        public void OnSwitchView(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
                StartCoroutine(_switching = SwitchView());
        }

        private IEnumerator SwitchView()
        {
            var posSrc = _transform.localPosition;
            var posDst = IsFirstPersonView
                ? thirdPersonCamTransform.localPosition
                : firstPersonCamTransform.localPosition;

            var rotSrc = _transform.localRotation;
            var rotDst = IsFirstPersonView
                ? thirdPersonCamTransform.localRotation
                : firstPersonCamTransform.localRotation;

            var pivotRotSrc = cameraPivot.localRotation;
            var pivotRotDst = Quaternion.identity;
            
            var t = 0.0f;
            while (t < 1.0f)
            {
                _transform.localPosition = Vector3.Lerp(posSrc, posDst, t);
                _transform.localRotation = Quaternion.Lerp(rotSrc, rotDst, t);
                cameraPivot.localRotation = Quaternion.Lerp(pivotRotSrc, pivotRotDst, t);
                
                t += Time.deltaTime / switchingDuration;
                yield return null;
            }

            _transform.localPosition = posDst;
            _transform.localRotation = rotDst;
            cameraPivot.localRotation = pivotRotDst;

            _defaultEulerAngle = _transform.localEulerAngles;
            _cameraRotationDelta = Vector3.zero;

            IsFirstPersonView = !IsFirstPersonView;
            if (IsFirstPersonView)
                _camera.cullingMask = ~LayerMask.GetMask("LocalPlayer");
            else
                _camera.cullingMask = ~0;
            
            _switching = null;
            
            Debug.Log(IsFirstPersonView);
        }
        
        public void OnRotateEnable(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _isRotateEnabled = value > 0.5f; // value is 1 or 0 (float)
        }
    }
}
