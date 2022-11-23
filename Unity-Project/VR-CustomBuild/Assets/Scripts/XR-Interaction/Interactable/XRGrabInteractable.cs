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

    private VRController targetHolder;
    private Vector3 grabbedPoint;
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

    //--------------grab states-------------
    private void StartGrab(VRController grabber, Vector3 grabPoint)
    {
        targetHolder = grabber;
        grabbedPoint = grabPoint;
        state = State.moving;
        if (usingRb) { rb.isKinematic = false; }
    }

    private void StopGrab(bool start)
    {
        if (!start) {
            if (usingRb) { rb.isKinematic = true; }
            if (usingRb && state == State.held) { Throw(); } //only throw when held, otherwise, detach
            state = State.idle;
            transform.SetParent(null);
        }
    }

    private void Throw()
    {
        //get hand velocity
        rb.velocity = targetHolder.velocity * throwForceMultiplier;
    }

    //---------------movement-------------------
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (state == State.moving) {
            //move
            Vector3 targetPoint = GetTargetPoint();
            MoveToGrabPos(targetPoint);
            //reach point check
            if (ReachPointCheck(targetPoint)) {
                OnReachPoint(targetPoint);
            }
        }
    }
    private Vector3 GetTargetPoint()
    {
        Vector3 targetPoint = targetHolder.transform.position + holdOffset;
        if (!forceGrab) { targetPoint -= grabbedPoint; }
        return targetPoint;
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
        return Vector3.Distance(transform.position, targetPos) < 0.01f;
    }

    private void OnReachPoint(Vector3 targetPos)
    {
        transform.position = targetPos;
        state = State.held;
        transform.SetParent(targetHolder.transform);
    }
}
