using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;

namespace Project
{
    [RequireComponent(typeof(Outline))]
    public class OutlineItem : MonoBehaviour
    {   
        private struct OutlineState {
            public Color color;
            public float width;
            public bool enabled;
        }

        private OutlineState _startState;
        [SerializeField] private Color _hoverOutlineColor;
        [SerializeField] private float _hoverOutlineWidth;

        private void Start() {
            Outline outline = GetComponent<Outline>();
            _startState = new OutlineState();
            _startState.color = outline.OutlineColor;
            _startState.width = outline.OutlineWidth;
            _startState.enabled = outline.enabled;
        }

        public void StartHover() {
            GetComponent<Outline>().OutlineColor = _hoverOutlineColor;
            GetComponent<Outline>().OutlineWidth = _hoverOutlineWidth;
            GetComponent<Outline>().enabled = true;
        }

        public void EndHover() {
            GetComponent<Outline>().OutlineColor = _startState.color;
            GetComponent<Outline>().OutlineWidth = _startState.width;
            GetComponent<Outline>().enabled = _startState.enabled;
        }
    }
}
