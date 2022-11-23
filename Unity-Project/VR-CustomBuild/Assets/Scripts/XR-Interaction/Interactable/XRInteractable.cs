using UnityEngine;
using UnityEngine.Events;

public class XRInteractable : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<bool> onHover;
    public UnityEvent<bool> onInteract;
    [HideInInspector] public UnityEvent<VRController, Vector3> onInteractAtPoint;
    public UnityEvent onActivate;
}
