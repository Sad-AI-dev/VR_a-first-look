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
    public float throwForceAngularMultiplier = 1;

    //vars
    private bool usingRb;
    private Rigidbody rb;

    private State state;
    //movement vars
    private float moveTimer = 0f;
    private Vector3 grabPointOffset;
    private Vector3 startPos;
    private Quaternion startRot;

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
        StartMoving(grabPoint);
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
    private void StartMoving(Vector3 grabPoint)
    {
        moveTimer = 0f;
        state = State.moving;
        //record start data
        startPos = transform.position;
        startRot = transform.rotation;
        //calc grab point offset
        grabPointOffset = transform.position - grabPoint;
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
        Vector3 targetPoint = interactor.transform.position + GetInteractorLocalVector(holdOffset);
        if (!forceGrab) { targetPoint += grabPointOffset; }
        return targetPoint;
    }
    private Vector3 GetInteractorLocalVector(Vector3 v)
    {
        return v.x * interactor.transform.right + v.y * interactor.transform.up + v.z * interactor.transform.forward;
    }

    private void MoveToPoint(Vector3 targetPos)
    {
        transform.position = Vector3.Lerp(startPos, targetPos, moveTimer / moveTime);
    }

    //----rotate---
    private Quaternion GetTargetRotation()
    {
        return forceGrab ? interactor.transform.rotation : transform.rotation;
    }

    private void RotateToRotation(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(startRot, targetRotation, moveTimer / moveTime);
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
            rb.AddForce(GetInteractorLocalVector(interactor.owner.velocity) * (100 * throwForceMultiplier));
            rb.angularVelocity = EulerToRadiansPerSecond(GetInteractorLocalVector(interactor.owner.angularVelocity)) * (-1 * throwForceAngularMultiplier);
        }
    }

    //rb angular v is measured in radians per second, but owner.angularVelocity is measured in euler angles
    private Vector3 EulerToRadiansPerSecond(Vector3 eulers)
    {
        //step 1: convert deg to rad
        eulers *= Mathf.Deg2Rad;
        //step 2: convert to per second measurement
        return eulers / Time.deltaTime;
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
