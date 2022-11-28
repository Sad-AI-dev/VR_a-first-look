using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class XRRayInteractor : MonoBehaviour
{
    public class HitResult {
        public RaycastHit hit;
        public bool isInteractable;
        public XRInteractable interactable;

        public HitResult(RaycastHit hit, XRInteractable interactable) {
            this.hit = hit;
            isInteractable = interactable != null;
            this.interactable = interactable;
        }
    }

    public VRController owner;
    public GameObject model;

    [Header("Settings")]
    public float interactRange = 1f;
    public LayerMask rayMask;
    public string interactableTag = "Interactable";

    [Header("Ray Settings")]
    public Gradient lineColor;
    public Gradient invalidColor;
    public AnimationCurve lineWidth;
    public Material lineMat;

    [Header("Events")]
    public UnityEvent<HitResult> onHoverNewObject;

    //vars
    [HideInInspector] public LineRenderer line;
    private HitResult lastResult;
    //states
    private bool isInteracting;

    private void Awake()
    {
        InitializeLine(); //setup visuals
    }

    private void Update()
    {
        UpdateRayCast();
    }

    //------------------------raycast---------------------------------
    private void UpdateRayCast()
    {
        HitResult newResult = GetNewResult();
        if (newResult != null) { OnObjectFound(newResult); }
        else { NothingFoundCheck(); }
        ResetInteractionState(newResult);
        lastResult = newResult; //update last result
        //update visuals
        UpdateLine();
    }

    //----find raycast results-----
    private HitResult GetNewResult()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, rayMask, QueryTriggerInteraction.Ignore)) {
            return new HitResult(hit, TryGetInteractable(hit));
        }
        return null;
    }
    private XRInteractable TryGetInteractable(RaycastHit newHit)
    {
        if (newHit.transform.CompareTag(interactableTag)) {
            return newHit.transform.GetComponent<XRInteractable>();
        }
        return null;
    }

    //----on object found events-----
    private void OnObjectFound(HitResult newResult)
    {
        if (lastResult == null || lastResult.hit.transform != newResult.hit.transform) {
            onHoverNewObject?.Invoke(newResult);
            TryActivateInteractableHoverEvents(newResult);
        }
    }
    private void TryActivateInteractableHoverEvents(HitResult newResult)
    {
        if (newResult.interactable) {
            newResult.interactable.events.onHoverStart?.Invoke();
        }
        if (lastResult != null && lastResult.isInteractable) {
            lastResult.interactable.events.onHoverEnd?.Invoke();
        }
    }

    //-----------nothing found events------------
    private void NothingFoundCheck()
    {
        if (lastResult != null) {
            onHoverNewObject?.Invoke(null);
        }
    }

    //------------------------visuals----------------------------------
    private void InitializeLine()
    {
        line.positionCount = 2;
        line.material = lineMat;
        line.colorGradient = lineColor;
        line.widthCurve = lineWidth;
    }

    private void UpdateLine()
    {
        if (lastResult != null) {
            line.SetPositions(new Vector3[] { transform.position, lastResult.hit.point });
        }
        else { //default to full range
            line.SetPositions(new Vector3[] { transform.position, transform.position + (transform.forward * interactRange) });
        }
    }

    //---------------------------interaction-----------------------------
    private void ResetInteractionState(HitResult newResult)
    {
        if (newResult == null || isInteracting && (lastResult.hit.transform != newResult.hit.transform)) {
            TryEndInteract();
        }
    }
    
    public void TryStartInteract()
    {
        if (lastResult != null && lastResult.isInteractable) {
            isInteracting = true;
            lastResult.interactable.SetInteractor(this);
            //invoke events
            lastResult.interactable.events.onInteractStart?.Invoke();
            lastResult.interactable.events.onInteractAtPoint?.Invoke(lastResult.hit.point);
        }
    }
    public void TryEndInteract()
    {
        if (isInteracting) {
            isInteracting = false;
            lastResult.interactable.events.onInteractEnd?.Invoke();
            //decouple after invoking events
            lastResult.interactable.SetInteractor(null);
        }
    }

    public void TryActivate()
    {
        if (isInteracting) {
            lastResult.interactable.events.onActivate?.Invoke();
        }
    }

    //---------------------private var management-----------------------
    public void SetLineColor(Gradient gradient)
    {
        line.colorGradient = gradient;
    }

    public HitResult GetLastHitResult()
    {
        return lastResult;
    }

    //---------------------manage enable / disable-----------------------
    private void OnEnable()
    {
        line.enabled = true;
    }

    private void OnDisable()
    {
        line.enabled = false;
    }

    //-----------------------------editor------------------------------
    private void Reset()
    {
        //setup line renderer
        line = GetComponent<LineRenderer>();
        line.positionCount = 0; //hide line in editor
        //initialize width curve
        lineWidth = new AnimationCurve(new Keyframe(0f, 0.05f), new Keyframe(1f, 0.05f));
    }
}
