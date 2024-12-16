using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Project.UserTest
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class UserTestItem : MonoBehaviour
    {
        [SerializeField] private bool _isTarget;
        [SerializeField] private bool _isStarter;

        public bool IsTarget => _isTarget;
        public bool IsStarter => _isStarter;

        private void Start() {
            XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
            UserTestManager testManager = FindObjectOfType<UserTestManager>();
            grabInteractable.selectEntered.AddListener(context => testManager.RegisterGrabbedItem(this));
        }
    }
}
