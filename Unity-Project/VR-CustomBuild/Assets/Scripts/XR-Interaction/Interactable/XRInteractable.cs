using UnityEngine;
using UnityEngine.Events;

public class XRInteractable : MonoBehaviour
{
    [System.Serializable]
    public struct InteractableEvents {
        public UnityEvent onHoverStart;
        public UnityEvent onHoverEnd;
        public UnityEvent onInteractStart;
        public UnityEvent onInteractEnd;
        [HideInInspector] public UnityEvent<Vector3> onInteractAtPoint;
        public UnityEvent onActivate;
    }

    [Header("Events")]
    public InteractableEvents events;

    //vars
    protected XRRayInteractor interactor;

    public virtual void SetInteractor(XRRayInteractor rayInteractor)
    {
        interactor = rayInteractor;
    }
}
