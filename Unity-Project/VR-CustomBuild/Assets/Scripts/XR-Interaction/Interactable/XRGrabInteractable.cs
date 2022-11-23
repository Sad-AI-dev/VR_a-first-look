using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRGrabInteractable : XRInteractable
{
    private enum State {
        idle, moving, held
    }
    public enum MoveMode {
        linear, lerp
    }

    [Header("Grab Settings")]
    public Vector3 holdOffset;

    public bool forceGrab;
    public MoveMode moveMode;
    public float grabMoveSpeed = 1f;
    public float grabRotateSpeed = 1f;

    [Header("Throw Settings")]
    public float throwForceMultiplier = 1f;

    //vars
    private bool usingRb;
    private Rigidbody rb;

    private RaycastInteractor holder;
    private Vector3 grabbedPointOffset;
    private State state;

    private void Awake()
    {
        onInteractAtPoint.AddListener(StartGrab);
        onInteract.AddListener(StopGrab);
    }

    private void Start()
    {
        usingRb = TryGetComponent(out Rigidbody rigidBody);
        rb = rigidBody;
    }

    //-------------------------------------State Management------------------------------------
    private void StartGrab(RaycastInteractor grabber, Vector3 grabPoint)
    {
        if (holder != null) { DetachFromInteractor(holder); } //detach from old holder
        //update vars
        holder = grabber;
        grabbedPointOffset = transform.position - grabPoint;
        //start move
        state = State.moving;
        StartMove();
    }

    private void StopGrab(bool start)
    {
        if (!start) {
            DetachFromInteractor(holder);
            state = State.idle;
        }
    }

    //-----------interactor attachment----------------
    private void AttachtoInteractor(RaycastInteractor interactor)
    {
        transform.SetParent(interactor.transform);
    }

    private void DetachFromInteractor(RaycastInteractor interactor)
    {
        transform.SetParent(null);
        if (usingRb) { 
            rb.isKinematic = false; //enable rb
            if (state == State.held) { Throw(); }
        }
        interactor.enabled = true;
        holder = null;
    }

    //----------------------------------movement-------------------------------------
    private void Throw()
    {
        //get hand velocity
        rb.velocity = holder.owner.velocity * throwForceMultiplier;
    }

    private void StartMove()
    {
        if (usingRb) { rb.isKinematic = true; }
        holder.enabled = false;
    }

    private void FixedUpdate()
    {
        if (state == State.moving) {
            Rotate();
            Move();
        }
    }

    private void Rotate()
    {
        if (forceGrab) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, holder.transform.rotation, grabRotateSpeed * Time.deltaTime);
        }
    }

    private void Move()
    {
        Vector3 targetPoint = GetTargetPoint();
        MoveToGrabPos(targetPoint);
        //reach point check
        if (ReachPointCheck(targetPoint)) {
            OnReachPoint(targetPoint);
        }
    }
    private Vector3 GetTargetPoint()
    {
        Vector3 targetPoint = holder.transform.position + GetLocalOffset();
        if (!forceGrab) { targetPoint += grabbedPointOffset; }
        return targetPoint;
    }
    private Vector3 GetLocalOffset()
    {
        return holdOffset.x * holder.transform.right + holdOffset.y * holder.transform.up + holdOffset.z * holder.transform.forward;
    }

    //-------------move object------------
    private void MoveToGrabPos(Vector3 targetPos)
    {
        float speed = grabMoveSpeed * 100f * Time.deltaTime;
        switch (moveMode) {
            case MoveMode.linear:
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
                break;

            case MoveMode.lerp:
                transform.position = Vector3.Lerp(transform.position, targetPos, speed);
                break;
        }
    }

    //------------------on point reach------------------
    private bool ReachPointCheck(Vector3 targetPos)
    {
        return Vector3.Distance(transform.position, targetPos) < 0.05f;
    }

    private void OnReachPoint(Vector3 targetPos)
    {
        transform.position = targetPos;
        state = State.held;
        AttachtoInteractor(holder);
        if (forceGrab) {
            transform.localRotation = Quaternion.identity;
        }
    }
}
