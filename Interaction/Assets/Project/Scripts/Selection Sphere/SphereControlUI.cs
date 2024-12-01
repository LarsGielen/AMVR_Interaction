using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.SelectionSphere
{
    public class SphereControlUI : MonoBehaviour
    {
        [SerializeField] SelectionSphereManager _selectionSphereManager;
        [SerializeField] private GameObject _controlButtonsCanvas; 

        [Header("Buttons")]
        [SerializeField] private Button _toggleMoveButton;
        [SerializeField] private Button _toggleRelativeButton;
        [SerializeField] private Button _moveUpButton;
        [SerializeField] private Button _moveDownButton;
        [SerializeField] private Button _moveFrontButton;
        [SerializeField] private Button _moveBackButton;
        [SerializeField] private Button _moveLeftButton;
        [SerializeField] private Button _moveRightButton;
        [SerializeField] private Button _scaleUp;
        [SerializeField] private Button _scaleDown;

        private void Awake() {
            if (_selectionSphereManager == null) _selectionSphereManager = FindObjectOfType<SelectionSphereManager>();
            Assert.IsNotNull(_selectionSphereManager, "selectionSphereManager can't be null, add one in the scene");
            SetupButtonActions();
        }

        private void Update() {
            {
                ColorBlock cb = _toggleMoveButton.colors;
                cb.normalColor = _selectionSphereManager.IsMoveMode? Color.green : new Color(0.1254f, 0.5882f, 0.9529f);
                cb.highlightedColor = _selectionSphereManager.IsMoveMode? Color.green : new Color(0.0941f, 0.4392f, 0.7137f);
                _toggleMoveButton.colors = cb;
            }

            {
                ColorBlock cb = _toggleRelativeButton.colors;
                cb.normalColor = _selectionSphereManager.IsRelativeMode? Color.green : new Color(0.1254f, 0.5882f, 0.9529f);
                cb.highlightedColor = _selectionSphereManager.IsRelativeMode? Color.green : new Color(0.0941f, 0.4392f, 0.7137f);
                _toggleRelativeButton.colors = cb;
            }

            _controlButtonsCanvas.SetActive(_selectionSphereManager.IsMoveMode);
        }

        private void SetupButtonActions() {
            _toggleMoveButton.onClick.AddListener(() => _selectionSphereManager.IsMoveMode = !_selectionSphereManager.IsMoveMode);
            _toggleRelativeButton.onClick.AddListener(() => _selectionSphereManager.IsRelativeMode = !_selectionSphereManager.IsRelativeMode);

            CreateOnPointerDownEvent(_moveUpButton,    () => _selectionSphereManager.SphereMoveDirection = new Vector3(0, 1, 0));
            CreateOnPointerDownEvent(_moveDownButton,  () => _selectionSphereManager.SphereMoveDirection = new Vector3(0, -1, 0));
            CreateOnPointerDownEvent(_moveFrontButton, () => _selectionSphereManager.SphereMoveDirection = new Vector3(0, 0, 1));
            CreateOnPointerDownEvent(_moveBackButton,  () => _selectionSphereManager.SphereMoveDirection = new Vector3(0, 0, -1));
            CreateOnPointerDownEvent(_moveLeftButton,  () => _selectionSphereManager.SphereMoveDirection = new Vector3(-1, 0, 0));
            CreateOnPointerDownEvent(_moveRightButton, () => _selectionSphereManager.SphereMoveDirection = new Vector3(1, 0, 0));
            CreateOnPointerDownEvent(_scaleUp,   () => _selectionSphereManager.SphereScaleDirection = 1);
            CreateOnPointerDownEvent(_scaleDown, () => _selectionSphereManager.SphereScaleDirection = -1);

            CreateOnPointerUpEvent(_moveUpButton,    () => _selectionSphereManager.SphereMoveDirection = Vector3.zero);
            CreateOnPointerUpEvent(_moveDownButton,  () => _selectionSphereManager.SphereMoveDirection = Vector3.zero);
            CreateOnPointerUpEvent(_moveFrontButton, () => _selectionSphereManager.SphereMoveDirection = Vector3.zero);
            CreateOnPointerUpEvent(_moveBackButton,  () => _selectionSphereManager.SphereMoveDirection = Vector3.zero);
            CreateOnPointerUpEvent(_moveLeftButton,  () => _selectionSphereManager.SphereMoveDirection = Vector3.zero);
            CreateOnPointerUpEvent(_moveRightButton, () => _selectionSphereManager.SphereMoveDirection = Vector3.zero);
            CreateOnPointerUpEvent(_scaleUp,   () => _selectionSphereManager.SphereScaleDirection = 0);
            CreateOnPointerUpEvent(_scaleDown, () => _selectionSphereManager.SphereScaleDirection = 0);
        }

        private void CreateOnPointerUpEvent(Button button, Action function) {
            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            if (eventTrigger == null) eventTrigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) => function?.Invoke());
            eventTrigger.triggers.Add(entry);
        }

        private void CreateOnPointerDownEvent(Button button, Action function) {
            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            if (eventTrigger == null) eventTrigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => function?.Invoke());
            eventTrigger.triggers.Add(entry);
        }
    }
}
