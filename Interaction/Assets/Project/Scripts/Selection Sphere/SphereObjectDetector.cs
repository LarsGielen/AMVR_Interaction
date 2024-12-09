using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.SelectionSphere
{
    public class SphereObjectDetector : MonoBehaviour
    {
        [SerializeField] private Transform copySphere; // Reference to the dummy sphere
        [SerializeField] private LayerMask detectionLayer; // Layer for objects to detect (e.g., "Selectable Items")

        private Dictionary<GameObject, GameObject> copiedItems = new Dictionary<GameObject, GameObject>();

        void Awake()
        {
            if (copySphere == null)
            {
                Debug.LogError("Dummy Sphere is not assigned");
            }
        }

        void Update()
        {
            Collider[] detectedColliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, detectionLayer);
            HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

            foreach (var collider in detectedColliders)
            {
                GameObject original = collider.gameObject;
                currentlyDetected.Add(original);

                if (copiedItems.ContainsKey(original))
                {
                    // Update position of the existing copy
                    Debug.Log("Update copy position");
                    UpdateCopyPosition(original);
                }
                else
                {
                    // Create a new copy
                    CreateCopy(original);
                }
            }

           foreach (var kvp in copiedItems.ToArray())
            {
                if (!currentlyDetected.Contains(kvp.Key))
                {
                    Destroy(kvp.Value);
                    copiedItems.Remove(kvp.Key);
                }
            }
        }

        void CreateCopy(GameObject original)
        {
            // Duplicate the object
            GameObject duplicate = Instantiate(original);

            // Calculate the scale ratio between the original and dummy sphere
            float scaleRatio = copySphere.localScale.x / transform.localScale.x;

            // Position the duplicate relative to the dummy sphere
            duplicate.transform.position = copySphere.position + (original.transform.position - transform.position) * scaleRatio;

            // Adjust the scale of the duplicate
            duplicate.transform.localScale = original.transform.localScale * scaleRatio;

            // Match rotation
            duplicate.transform.rotation = original.transform.rotation;

            duplicate.AddComponent<CorrespondMovement>().Init(original.transform);

            // Add the original and its duplicate to the dictionary
            copiedItems[original] = duplicate;
        }

        void UpdateCopyPosition(GameObject original)
        {
            if (copiedItems.TryGetValue(original, out var duplicate))
            {
                // Calculate the scale ratio between the original and dummy sphere
                float scaleRatio = copySphere.localScale.x / transform.localScale.x;

                // Update the position of the duplicate
                duplicate.transform.position = copySphere.position + (original.transform.position - transform.position) * scaleRatio;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(copySphere.position, copySphere.localScale.x / 2);
        }
    }
}
