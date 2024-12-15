using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CorrespondMovement : MonoBehaviour
{
    [SerializeField] private Transform referenceTransform;
    [SerializeField] private Transform dummySphere;

    private XRGrabInteractable grabInteractable;
    
    private Vector3 previousPosition;
    private bool holdingItem = false;
    private SelectEnterEventArgs onSelectArgs;

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

        float distanceFromDummyCenter = Vector3.Distance(dummySphere.position, transform.position);
        if (distanceFromDummyCenter > dummySphere.localScale.x) {
            RemoveReference(onSelectArgs);
            return;
        }

        float scale = referenceTransform.localScale.x/transform.localScale.x;

        Vector3 translation = transform.position - previousPosition;

        referenceTransform.position += translation*scale;
        referenceTransform.rotation = transform.rotation;

        previousPosition = transform.position;
    }

    public void Init(Transform reference, Transform dummySphere) {
        this.referenceTransform = reference;
        this.dummySphere = dummySphere;
    }

    public void RemoveReference(SelectEnterEventArgs args) {
        var interactionManager = grabInteractable.interactionManager;
        if (interactionManager != null)
            interactionManager.SelectExit(args.interactorObject, grabInteractable);
            interactionManager.SelectEnter(args.interactorObject, referenceTransform.gameObject.GetComponent<XRGrabInteractable>());
        Destroy(this.gameObject);
    }

    private void OnGrabbed(SelectEnterEventArgs arg0) {
        holdingItem = true;
        onSelectArgs = arg0;
    }
    private void OnReleased(SelectExitEventArgs arg0) => holdingItem = false;
}
