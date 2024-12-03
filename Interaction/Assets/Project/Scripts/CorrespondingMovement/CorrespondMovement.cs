using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CorrespondMovement : MonoBehaviour
{
    [SerializeField] GameObject smallObject;

    private XRGrabInteractable grabInteractable;
    
    private Vector3 previousPosition;
    private bool holdingItem = false;

    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Abonneer je op de events van de XRGrabInteractable
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!holdingItem) return;

        float scale = smallObject.transform.localScale.x/transform.localScale.x;

        Vector3 translation = transform.position - previousPosition;

        smallObject.transform.position += translation*scale;
        smallObject.transform.rotation = transform.rotation;

        previousPosition = transform.position;
    }

    private void OnGrabbed(SelectEnterEventArgs arg0)
    {
        holdingItem = true;
    }
    private void OnReleased(SelectExitEventArgs arg0)
    {
        holdingItem = false;
    }
}
