using System.Collections.Generic;
using UnityEngine;

namespace Project.SelectionSphere
{
    public class SphereObjectDetector : MonoBehaviour
    {
        [SerializeField] private Transform copySphere; // Reference to the dummy sphere
        [SerializeField] private LayerMask detectionLayer; // Layer for objects to detect (e.g., "Selectable Items")

        private List<GameObject> copiedItems = new List<GameObject>();

        void Awake()
        {
            if (copySphere == null)
            {
                Debug.LogError("Dummy Sphere is not assigned! Please assign it in the Inspector.");
            }
        }

        void Update()
        {
            // Detect objects within the sphere
            Collider[] detectedColliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, detectionLayer);

            // Clear previous copies
            ClearDummySphere();

            // Copy detected objects to the dummy sphere
            foreach (var collider in detectedColliders)
            {
                CopyObjectToDummySphere(collider.gameObject);
            }
        }

        void CopyObjectToDummySphere(GameObject original)
        {
            // Duplicate the object
            GameObject duplicate = Instantiate(original);

            // Calculate the scale ratio between the original and dummy sphere
            float scaleRatio = copySphere.localScale.x / transform.localScale.x;
            Debug.Log(scaleRatio);

            // Position the duplicate relative to the dummy sphere
            duplicate.transform.position = copySphere.position + (original.transform.position - transform.position) * scaleRatio;

            // Adjust the scale of the duplicate
            duplicate.transform.localScale = original.transform.localScale * scaleRatio;

            // Match rotation
            duplicate.transform.rotation = original.transform.rotation;

            duplicate.AddComponent<CorrespondMovement>().Init(original.transform);

            // Add to the list of copied items
            copiedItems.Add(duplicate);
        }

        void ClearDummySphere()
        {
            // Destroy all previously copied items
            foreach (var item in copiedItems)
            {
                Destroy(item);
            }
            copiedItems.Clear();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(copySphere.position, copySphere.localScale.x / 2);
        }
    }
}
