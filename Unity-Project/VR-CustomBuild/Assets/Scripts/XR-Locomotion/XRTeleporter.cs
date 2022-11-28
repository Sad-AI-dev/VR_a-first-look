using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRTeleporter : MonoBehaviour
{
    public XRRayInteractor owner;
    public Transform targetTransform;
    [Header("Settings")]
    public string checkTag = "Teleportable";

    //vars
    bool currentHoverIsValid;

    private void Start()
    {
        owner.onHoverNewObject.AddListener(HoverPointCheck);
        SetInvalid();
    }

    public void HoverPointCheck(XRRayInteractor.HitResult hitResult)
    {
        if (hitResult != null && hitResult.hit.transform.CompareTag(checkTag)) {
            if (!currentHoverIsValid) { SetValid(); }
        }
        else if (currentHoverIsValid) {
            SetInvalid();
        }
    }

    //---------------------update state---------------------
    private void SetValid()
    {
        owner.SetLineColor(owner.lineColor);
        currentHoverIsValid = true;
    }

    private void SetInvalid()
    {
        owner.SetLineColor(owner.invalidColor);
        currentHoverIsValid = false;
    }

    //---------------------teleport-----------------
    public void Teleport()
    {
        if (currentHoverIsValid) {
            targetTransform.position = owner.GetLastHitResult().hit.point;
        }
    }
}
