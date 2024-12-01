using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Project.SelectionSphere
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class SelectionSphere : MonoBehaviour
    {
        [SerializeField] float _maxSize = 5f;
        [SerializeField] float _minSize = 0.2f;

        [SerializeField] MeshRenderer _renderer;

        private Material _material;

        private void Awake() {
            _material = _renderer.material;
        }

        public void IncrementSize(float increment) {
            float scale = transform.localScale.x + increment;
            transform.localScale = Mathf.Clamp(scale, _minSize, _maxSize) * Vector3.one;
        }
        public void IncrementPosition(Vector3 increment) {
            transform.position += increment;
        }

        public void SetColor(Color baseColor, Color highlightColor) {
            _material.SetColor("_BaseColor", baseColor);
            _material.SetColor("_EmissiveColor", highlightColor);
        }
    }
}
