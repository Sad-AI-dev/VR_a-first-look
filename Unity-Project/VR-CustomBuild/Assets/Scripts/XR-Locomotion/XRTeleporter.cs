//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class XRTeleporter : MonoBehaviour
//{
//    public RaycastInteractor owner;
//    public Transform targetTransform;
//    [Header("Settings")]
//    public string checkTag;

//    //vars
//    bool currentHoverIsValid;

//    private void Start()
//    {
//        owner.onHoverNewObject.AddListener(HoverPointCheck);
//        SetInvalid();
//    }

//    public void HoverPointCheck()
//    {
//        if (owner.GetLastHit().transform.CompareTag(checkTag)) {
//            if (!currentHoverIsValid) {
//                SetValid();
//            }
//        }
//        else if (currentHoverIsValid) {
//            SetInvalid();
//        }
//    }

//    //---------------------update state---------------------
//    private void SetValid()
//    {
//        owner.SetColor(owner.lineColor);
//        currentHoverIsValid = true;
//    }

//    private void SetInvalid()
//    {
//        owner.SetColor(owner.invalidColor);
//        currentHoverIsValid = false;
//    }

//    //---------------------teleport-----------------
//    public void Teleport()
//    {
//        if (currentHoverIsValid) {
//            targetTransform.position = owner.GetLastHit().point;
//        }
//    }
//}
