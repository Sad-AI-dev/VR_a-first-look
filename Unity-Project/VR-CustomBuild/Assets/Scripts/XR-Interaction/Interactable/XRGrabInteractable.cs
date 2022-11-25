using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRGrabInteractable : XRInteractable
{
    private enum State { idle, moving, held }

    [Header("settings")]
    public bool hideHolderModel;
    public bool forceGrab;
    public Vector3 holdOffset;

    [Header("Movement Settings")]
    public float moveTime = 0.5f;

    [Header("Throw Settings")]
    public float throwForceMultiplier = 1;

    //vars
    private bool usingRb;
    private Rigidbody rb;

    private State state;
    private float moveTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        usingRb = rb != null;
        //setup events
        events.onInteractAtPoint.AddListener(StartGrab);
        events.onInteractEnd.AddListener(StopGrab);
    }

    //------------------------------start stop---------------------------------
    public void StartGrab(Vector3 grabPoint)
    {
        interactor.enabled = false; //disable interactor
        if (usingRb) { rb.isKinematic = true; } //deactivate physics
        StartMoving();
    }

    public void StopGrab()
    {
        interactor.enabled = true;
        if (usingRb) { rb.isKinematic = false; } //activate physics
        if (state == State.held) { 
            DetachFromInteractor(interactor);
            Throw();
        }
        state = State.idle;
    }

    //-------------------------------------movement--------------------------------------
    private void StartMoving()
    {
        moveTimer = 0f;
        state = State.moving;
    }

    private void FixedUpdate()
    {
        if (state == State.moving) {
            moveTimer += Time.deltaTime;
            if (moveTimer < moveTime) {
                MoveToPoint(GetTargetPoint());
                RotateToRotation(GetTargetRotation());
            }
            else { OnReachDestination(GetTargetPoint(), GetTargetRotation()); }
        }
    }

    //-----move----
    private Vector3 GetTargetPoint()
    {
        return interactor.transform.position;
    }

    private void MoveToPoint(Vector3 targetPos)
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, moveTimer / moveTime);
    }

    //----rotate---
    private Quaternion GetTargetRotation()
    {
        return interactor.transform.rotation;
    }

    private void RotateToRotation(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveTimer / moveTime);
    }

    //------------on reach destination-------------
    private void OnReachDestination(Vector3 targetPos, Quaternion targetRotation)
    {
        state = State.held;
        transform.SetPositionAndRotation(targetPos, targetRotation);
        AttachToInteractor(interactor);
    }

    //----------------------------------------throw behavior------------------------------------------
    private void Throw()
    {
        if (usingRb) {
            rb.velocity = interactor.owner.velocity * throwForceMultiplier;
        }
    }

    //------------------------------------------update interactor----------------------------------------
    public override void SetInteractor(XRRayInteractor rayInteractor)
    {
        if (state != State.idle) {
            interactor.TryEndInteract();
        }
         interactor = rayInteractor;
    }

    //------------------------------------------attach functions-----------------------------------------
    private void AttachToInteractor(XRRayInteractor rayInteractor)
    {
        transform.SetParent(rayInteractor.transform);
        if (hideHolderModel) {
            rayInteractor.model.SetActive(false);
        }
    }

    private void DetachFromInteractor(XRRayInteractor rayInteractor)
    {
        transform.SetParent(null);
        if (hideHolderModel) {
            rayInteractor.model.SetActive(true);
        }
    }
}
