using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(Transform))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 followOffset = new Vector3(1.5f, 0f, 0f);

        private Vector3 initialPositionOffset;

        void Start()
        {
            if (cameraTransform == null)
            {
                Debug.LogError("CameraTransform is not assigned");
                return;
            }
        }

        void Update()
        {
            if (cameraTransform == null) return;

            transform.position = cameraTransform.position + followOffset;
        }
    }
}
