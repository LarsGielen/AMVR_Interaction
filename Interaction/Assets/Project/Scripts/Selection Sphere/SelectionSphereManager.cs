using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Project.SelectionSphere
{
    public class SelectionSphereManager: MonoBehaviour
    {
        [SerializeField] private SelectionSphere _sphere;
        [SerializeField] private Camera _camera;
        [SerializeField] private InteractionLayerMask _selectionSphereLayerMask;
        [SerializeField] private float _moveSpeed = 5;
        [SerializeField] private float _scaleSpeed = 5;

        [SerializeField] private Color _moveModeColor = new Color(0, 1, 0, 0.17f);
        [SerializeField] [ColorUsage(true, true)] private Color _moveModeHighlightColor = new Color(0, 32, 0, 1);
        [SerializeField] private Color _normalModeColor = new Color(0, 1, 1, 0.17f);
        [SerializeField] [ColorUsage(true, true)] private Color _normalModeHighLightColor = new Color(0, 32, 32, 1);

        [Header("Actions")]
        [SerializeField] private InputAction _toggleMoveModeAction;
        [SerializeField] private InputAction _toggleRelativeModeAction;
        [SerializeField] private InputAction _toggleDynamicSpeed;
        [SerializeField] private InputAction _horizontalMovementAction;
        [SerializeField] private InputAction _verticalMovementAction;
        [SerializeField] private InputAction _scaleAction;

        private XRBaseInputInteractor[] _interactors;
        private InteractionLayerMask[] _defaultLayerMasks;

        private bool _isMoveMode;
        private bool _isRelativeMoveMode = true;
        private bool _isDynamicSpeed = true;

        private Vector3 _moveDirection;
        private float _scaleDirection;


        #region Public Functions
        public Vector3 SphereMoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public float SphereScaleDirection { get => _scaleDirection; set => _scaleDirection = value; }
        public bool IsMoveMode { get => _isMoveMode; set => SetMoveMode(value); }
        public bool IsRelativeMode { get => _isRelativeMoveMode; set => _isRelativeMoveMode = value; }

        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float ScaleSpeed { get => _scaleSpeed; set => _scaleSpeed = value; }
        #endregion

        #region Unity Functions
        private void Awake() {
            Init();
        }

        private void Update() {
            UpdateMoveSphere();
        }

        void OnEnable() {
            EnableInputActions();
        }

        void OnDisable() {
            DisableInputActions();
        }
        #endregion

        #region Private Functions
        private void Init() {
            Assert.IsNotNull(_sphere, "Sphere can't be null");

            _toggleMoveModeAction.performed += context => { SetMoveMode(!_isMoveMode); };
            _toggleRelativeModeAction.performed += context => { _isRelativeMoveMode = !_isRelativeMoveMode; };
            _toggleDynamicSpeed.performed += context => { _isDynamicSpeed = !_isDynamicSpeed; };
            _horizontalMovementAction.performed += context => { _moveDirection.x = context.ReadValue<Vector2>().x; _moveDirection.z = context.ReadValue<Vector2>().y; };
            _verticalMovementAction.performed += context => { _moveDirection.y = Mathf.Abs(context.ReadValue<Vector2>().y) > 0.8f ?  context.ReadValue<Vector2>().y : 0; };
            _scaleAction.performed += context => { _scaleDirection = Mathf.Abs(context.ReadValue<Vector2>().x) > 0.8f ?  context.ReadValue<Vector2>().x : 0; };

            SetMoveMode(false);
        }

        private void UpdateMoveSphere() {
            if (!_isMoveMode) return;

            _sphere.IncrementPosition(
                (_isRelativeMoveMode ? Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) 
                * _moveDirection : _moveDirection) 
                * _moveSpeed 
                * Time.deltaTime
                * (_isDynamicSpeed ? _sphere.transform.localScale.x : 1)
            );
            _sphere.IncrementSize(_scaleDirection * _scaleSpeed * Time.deltaTime); 
        }

        private void EnableInputActions() {
            _toggleMoveModeAction.Enable();
            _toggleRelativeModeAction.Enable();
            _toggleDynamicSpeed.Enable();
            _horizontalMovementAction.Enable();
            _verticalMovementAction.Enable();
            _scaleAction.Enable();
        }

        private void DisableInputActions() {
            _toggleMoveModeAction.Disable();
            _toggleRelativeModeAction.Disable();
            _toggleDynamicSpeed.Disable();
            _horizontalMovementAction.Disable();
            _verticalMovementAction.Disable();
            _scaleAction.Disable();
        }

        private void SetMoveMode(bool isMoveMode) {
            if (isMoveMode) {
                _interactors = FindObjectsOfType<XRBaseInputInteractor>(true);
                _defaultLayerMasks = new InteractionLayerMask[_interactors.Length];
                for (int i = 0; i < _interactors.Length; i++) {
                    _defaultLayerMasks[i] = _interactors[i].interactionLayers;
                    _interactors[i].interactionLayers = _selectionSphereLayerMask;
                } 
                _sphere.SetColor(_moveModeColor, _moveModeHighlightColor);
            }
            else if (_interactors != null) {
                for (int i = 0; i < _interactors.Length; i++)
                    _interactors[i].interactionLayers = _defaultLayerMasks[i];
                _sphere.SetColor(_normalModeColor, _normalModeHighLightColor);
            }

            _isMoveMode = isMoveMode;
        }

        #endregion
    }
}
