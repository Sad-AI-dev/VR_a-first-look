using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastInteractor : MonoBehaviour
{
    [Header("settings")]
    public float grabRange = 1f;
    public LayerMask layerMask;
    [Space(10)]
    public bool hideModel;
    public GameObject modelObject;
    [Header("ray settings")]
    public Gradient lineColor;
    public AnimationCurve lineWidth;
    public Material lineMaterial;

    //vars
    private bool hasInteractable;
    private XRInteractable currentInteractable;
    //line renderer
    LineRenderer lineRenderer;
    private bool initializedRenderer;

    private void Start()
    {
        if (hideModel && modelObject == null) {
            Debug.LogWarning($"{name}: No model reference set!");
        }
        //get external components
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        if (!lineRenderer) {
            lineRenderer = GetComponent<LineRenderer>();
        }
        //set vars
        lineRenderer.positionCount = 2;
        lineRenderer.colorGradient = lineColor;
        lineRenderer.widthCurve = lineWidth;
        lineRenderer.material = lineMaterial;
    }

    private void Update()
    {
        if (transform.hasChanged) {
            RaycastCheck();
            transform.hasChanged = false;
        }
    }

    //------------------raycast-------------------------------
    private void RaycastCheck()
    {
        XRInteractable foundInteractable = null;
        //raycast
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange, layerMask, QueryTriggerInteraction.Ignore)) {
            //check if hit object is interactable
            if (hit.transform.CompareTag("Interactable")) {
                hit.transform.TryGetComponent(out foundInteractable);
            }
            UpdateLineRender(hit.point);
        }
        else { SetLineRenderToDefault(); } //default line render value
        //no interactable found, set interactable to null
        OnFindInteractable(foundInteractable);
    }

    private void OnFindInteractable(XRInteractable interactable)
    {
        if (currentInteractable != interactable) {
            UpdateCurrentInteractable(interactable);
        }
    }

    private void UpdateCurrentInteractable(XRInteractable newInteractable)
    {
        //trigger events
        (hasInteractable ? currentInteractable : newInteractable).onHover?.Invoke(!hasInteractable);
        //update internal target interactable
        currentInteractable = newInteractable;
        hasInteractable = newInteractable != null;
    }

    //------------update line--------------
    private void UpdateLineRender(Vector3 endPoint)
    {
        lineRenderer.SetPositions(new Vector3[] { transform.position, endPoint });
    }

    private void SetLineRenderToDefault()
    {
        UpdateLineRender(transform.position + (transform.forward * grabRange));
    }

    //------------------------interaction----------------------------
    public void TryInteract(bool start)
    {
        if (hasInteractable) {
            currentInteractable.onInteract?.Invoke(start);
            HideModel(start);
        }
    }

    public void TryActivate()
    {
        if (hasInteractable) {
            currentInteractable.onActivate?.Invoke();
        }
    }

    //hide model feature
    public void HideModel(bool state)
    {
        modelObject.SetActive(state);
    }

    //------------------editor--------------------
    private void OnValidate()
    {
        if (!initializedRenderer) {
            InitializeLineInEditor();
            InitializeLineWidth();
        }
    }

    private void InitializeLineInEditor()
    {
        initializedRenderer = true;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }
    private void InitializeLineWidth()
    {
        lineWidth = new AnimationCurve(new Keyframe(0f, 0.1f), new Keyframe(1f, 0.1f));
    }
}
