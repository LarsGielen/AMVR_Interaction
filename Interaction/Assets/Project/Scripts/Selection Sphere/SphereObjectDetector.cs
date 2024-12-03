using System.Collections.Generic;
using UnityEngine;

namespace Project.SelectionSphere
{
    public class SphereObjectDetector : MonoBehaviour
    {
        [SerializeField] private GameObject dummySphere; // Reference to the dummy sphere
        [SerializeField] private LayerMask detectionLayer; // Layer for objects to detect (e.g., "Selectable Items")

        private List<GameObject> copiedItems = new List<GameObject>();
        private SphereCollider sphereCollider; // Reference to the real sphere's collider

        void Awake()
        {
            if (dummySphere == null)
            {
                Debug.LogError("Dummy Sphere is not assigned! Please assign it in the Inspector.");
            }

            sphereCollider = GetComponent<SphereCollider>();
            if (!sphereCollider)
            {
                Debug.LogError("SphereCollider is missing on this GameObject!");
            }
        }

        void Update()
        {
            if (sphereCollider == null) return;

            float detectionRadius = sphereCollider.radius * transform.localScale.x; // Adjust for scaling

            // Detect objects within the sphere
            Collider[] detectedColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

            // Clear previous copies
            ClearDummySphere();

            // Copy detected objects to the dummy sphere
            foreach (var collider in detectedColliders)
            {
                CopyObjectToDummySphere(collider.gameObject, detectionRadius);
            }
        }

        void CopyObjectToDummySphere(GameObject original, float detectionRadius)
        {
            // Duplicate the object
            GameObject duplicate = Instantiate(original);

            // Calculate the scale ratio between the original and dummy sphere
            float scaleRatio = dummySphere.GetComponent<SphereCollider>().radius * dummySphere.transform.localScale.x / detectionRadius;

            // Position the duplicate relative to the dummy sphere
            duplicate.transform.position = dummySphere.transform.position + (original.transform.position - transform.position) * scaleRatio;

            // Adjust the scale of the duplicate
            duplicate.transform.localScale = original.transform.localScale * scaleRatio;

            // Match rotation
            duplicate.transform.rotation = original.transform.rotation;

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

        void OnDrawGizmos()
        {
            if (sphereCollider != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, sphereCollider.radius * transform.localScale.x);
            }
        }
    }
}
