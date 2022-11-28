using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;

public class VRController : MonoBehaviour
{
    public enum TargetHand {
        Left, Right
    }

    //-----------input events---------------
    public class InputEvent<T>
    {
        public UnityEvent<T> onValueChanged;
        public InputFeatureUsage<T> feature;
        public T lastValue = default;

        public InputEvent(InputFeatureUsage<T> targetFeature, T value = default)
        { //constructor
            feature = targetFeature;
            lastValue = value;
            onValueChanged = new UnityEvent<T>();
        }

        public void TryUpdateValue(T newValue)
        {
            if (!newValue.Equals(lastValue)) {
                lastValue = newValue;
                onValueChanged?.Invoke(lastValue);
            }
        }
    }

    //------------------vars---------------
    [SerializeField] private TargetHand targetType;

    //vars
    private bool isConnected;
    [HideInInspector] public InputDevice targetDevice;
    //read input event
    private Action onTryReadInputs;

    //input vars
    public Vector3 velocity { get; private set; }
    public Vector3 angularVelocity { get; private set; }

    //bool events
    public InputEvent<bool> triggerButtonInput;
    public InputEvent<bool> gripButtonInput;
    public InputEvent<bool> primaryButtonInput;
    public InputEvent<bool> secondaryButtonInput;
    public InputEvent<bool> menuButtonInput;
    public InputEvent<bool> primaryAxisButtonInput;
    public InputEvent<bool> secondaryAxisButtonInput;
    //float events
    public InputEvent<float> triggerInput;
    public InputEvent<float> gripInput;
    //vector2 events
    public InputEvent<Vector2> primaryAxisInput;
    public InputEvent<Vector2> secondaryAxisInput;

    //input lists
    private InputEvent<bool>[] boolInputs;
    private InputEvent<float>[] floatInputs;
    private InputEvent<Vector2>[] vectorInputs;

    private void Awake()
    {
        InitializeInputs();
    }

    //---------------------------------------------------------------------------
    //------------------------------DEVICE CONNECTION----------------------------
    //---------------------------------------------------------------------------
    //--------enable events-----
    private void OnEnable()
    {
        //register events
        InputDevices.deviceConnected += OnConnect;
        InputDevices.deviceDisconnected += OnDisconnect;
        //see if controller is available
        TryGetDevice();
    }

    private void OnDisable()
    {
        //unregister events
        InputDevices.deviceConnected -= OnConnect;
        InputDevices.deviceDisconnected -= OnDisconnect;
        //disconnect controller
        UnRegisterDevice();
    }

    //--------connect / disconnect events------
    private void OnConnect(InputDevice newDevice)
    {
        if (!isConnected) {
            if (IsValidDevice(newDevice)) {
                RegisterDevice(newDevice);
            }
        }
    }

    private void OnDisconnect(InputDevice disconnectedDevice)
    {
        if (targetDevice.Equals(disconnectedDevice)) {
            UnRegisterDevice();
        }
    }

    //-------------register events------------
    private void RegisterDevice(InputDevice device)
    {
        isConnected = true;
        targetDevice = device;
        //read input events
        onTryReadInputs += ReadInputs;
    }

    private void UnRegisterDevice()
    {
        isConnected = false;
        //read input events
        onTryReadInputs -= ReadInputs;
    }

    //-------find target device---------
    private void TryGetDevice()
    {
        if (GetValidDevice(out InputDevice validDevice)) {
            RegisterDevice(validDevice);
        }
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

    //---------------------------------------------------------------------------
    //------------------------------INPUT HANDLING-------------------------------
    //---------------------------------------------------------------------------
    //------------initialize events----------
    private void InitializeInputs()
    {
        //trigger button
        triggerButtonInput = new InputEvent<bool>(CommonUsages.triggerButton);
        triggerInput = new InputEvent<float>(CommonUsages.trigger);
        //grip button
        gripButtonInput = new InputEvent<bool>(CommonUsages.gripButton);
        gripInput = new InputEvent<float>(CommonUsages.grip);
        //primary + secondary button
        primaryButtonInput = new InputEvent<bool>(CommonUsages.primaryButton);
        secondaryButtonInput = new InputEvent<bool>(CommonUsages.secondaryButton);
        //menu button
        menuButtonInput = new InputEvent<bool>(CommonUsages.menuButton);
        //primary axis
        primaryAxisInput = new InputEvent<Vector2>(CommonUsages.primary2DAxis);
        primaryAxisButtonInput = new InputEvent<bool>(CommonUsages.primary2DAxisClick);
        //secondary axis
        secondaryAxisInput = new InputEvent<Vector2>(CommonUsages.secondary2DAxis);
        secondaryAxisButtonInput = new InputEvent<bool>(CommonUsages.secondary2DAxisClick);

        //----------compile lists-----------
        boolInputs = new InputEvent<bool>[] { triggerButtonInput, gripButtonInput,
            primaryButtonInput, secondaryButtonInput, menuButtonInput, primaryAxisButtonInput,
            secondaryAxisButtonInput };
        floatInputs = new InputEvent<float>[] { triggerInput, gripInput };
        vectorInputs = new InputEvent<Vector2>[] { primaryAxisInput, secondaryAxisInput };
    }

    //------------detect inputs--------------
    private void Update()
    {
        onTryReadInputs?.Invoke();
    }

    private void ReadInputs()
    {
        UpdatePhysicalAttributes();
        ReadButtons();
    }

    private void UpdatePhysicalAttributes()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);
        transform.localPosition = pos;
        transform.localRotation = rot;
        //velocity
        targetDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 newVelocity);
        velocity = newVelocity;
        //rot velocity
        targetDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 newRotVelocity);
        angularVelocity = newRotVelocity;
    }

    //-------read button inputs------
    private void ReadButtons()
    {
        ReadBoolInputs();
        ReadFloatInputs();
        ReadVectorInputs();
    }

    private void ReadBoolInputs()
    {
        foreach (InputEvent<bool> input in boolInputs) {
            if (input.onValueChanged != null) {
                if (targetDevice.TryGetFeatureValue(input.feature, out bool value)) {
                    input.TryUpdateValue(value);
                }
            }
        }
    }

    private void ReadFloatInputs()
    {
        foreach (InputEvent<float> input in floatInputs) {
            if (input.onValueChanged != null) {
                if (targetDevice.TryGetFeatureValue(input.feature, out float value)) {
                    input.TryUpdateValue(value);
                }
            }
        }
    }

    private void ReadVectorInputs()
    {
        foreach (InputEvent<Vector2> input in vectorInputs) {
            if (input.onValueChanged != null) {
                if (targetDevice.TryGetFeatureValue(input.feature, out Vector2 value)) {
                    input.TryUpdateValue(value);
                }
            }
        }
    }
}
