using UnityEngine;
using UnityEngine.Events;

public class XRInteractable : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<bool> onHover;
    public UnityEvent<bool> onInteract;
    public UnityEvent onActivate;
}
