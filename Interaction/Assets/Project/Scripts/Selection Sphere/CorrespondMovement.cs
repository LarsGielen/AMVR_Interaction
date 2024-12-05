using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CorrespondMovement : MonoBehaviour
{
    [SerializeField] Transform referenceTransform;

    private XRGrabInteractable grabInteractable;
    
    private Vector3 previousPosition;
    private bool holdingItem = false;

    // Start is called before the first frame update
    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!holdingItem) return;

        float scale = referenceTransform.localScale.x/transform.localScale.x;

        Vector3 translation = transform.position - previousPosition;

        referenceTransform.position += translation*scale;
        referenceTransform.rotation = transform.rotation;

        previousPosition = transform.position;
    }

    public void Init(Transform reference) => referenceTransform = reference;

    private void OnGrabbed(SelectEnterEventArgs arg0) => holdingItem = true;
    private void OnReleased(SelectExitEventArgs arg0) => holdingItem = false;
}
