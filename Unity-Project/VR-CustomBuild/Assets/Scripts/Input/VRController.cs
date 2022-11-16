using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRController : MonoBehaviour
{
    public enum TargetHand {
        Left, Right
    }

    [SerializeField] private TargetHand targetType;

    //vars
    [HideInInspector] public InputDevice targetDevice;

    private void Start()
    {
        TryGetDevice();
    }

    //-------find target device---------
    private void TryGetDevice()
    {
        if (GetValidDevice(out InputDevice validDevice)) {
            targetDevice = validDevice;
        }
        else { StartCoroutine(GetDeviceCo()); }
    }
    private bool GetValidDevice(out InputDevice validDevice)
    {
        validDevice = default;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach (InputDevice device in devices) {
            if (IsValidDevice(device)) {
                validDevice = device;
                return true;
            }
        }
        return false;
    }
    private bool IsValidDevice(InputDevice device)
    {
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand)) {
            if (device.characteristics.HasFlag( targetType == TargetHand.Left ?
                InputDeviceCharacteristics.Left :
                InputDeviceCharacteristics.Right)) {
                return true;
            }
        }
        return false;
    }

    private IEnumerator GetDeviceCo()
    {
        yield return new WaitForSeconds(1f);
        TryGetDevice();
    }

    //----------------setup events--------------------
    private void InitializeTargetDevice()
    {

    }

    //------------detect inputs--------------
    private void Update()
    {
        UpdatePosAndRot();
    }

    private void UpdatePosAndRot()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);
        transform.SetPositionAndRotation(pos, rot);
    }
}
