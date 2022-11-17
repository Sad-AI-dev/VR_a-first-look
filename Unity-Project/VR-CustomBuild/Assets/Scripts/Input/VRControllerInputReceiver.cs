using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRControllerInputReceiver : MonoBehaviour
{
    //input codes
    public enum ButtonCodes {
        trigger, grip, primary, secondary, primaryAxis, secondaryAxis, menu
    }

    public enum AxisCodes {
        trigger, grip, primaryAxisX, primaryAxisY, secondaryAxisX, secondaryAxisY
    }

    public enum DirectionalCodes {
        primaryAxis, secondaryAxis
    }


    [System.Serializable]
    public class ButtonInput
    {
        public string name;
        public List<ButtonCodes> inputCodes;

        [Header("Events")]
        public UnityEvent onButtonDown;
        public UnityEvent onButtonHeld;
        public UnityEvent onButtonUp;

        //vars
        [HideInInspector] public bool pressed;

        public void OnStateChanged(bool newState) {
            if (newState != pressed) {
                pressed = newState;
                if (pressed) { onButtonDown?.Invoke(); }
                else { onButtonUp?.Invoke(); }
            }
        }
    }

    [System.Serializable]
    public class AxisInput
    {
        public string name;
        public List<AxisCodes> inputCodes;
        [Header("Events")]
        public UnityEvent<float> output;
    }

    [System.Serializable]
    public class DirectionalInput
    {
        public string name;
        public List<DirectionalCodes> inputCodes;
        [Header("Events")]
        public UnityEvent<Vector2> output;
    }

    public VRController targetController;

    [Header("Input Events")]
    [SerializeField] private List<ButtonInput> buttonInputs;
    [SerializeField] private List<AxisInput> axisInputs;
    [SerializeField] private List<DirectionalInput> directionalInputs;

    private void Start()
    {
        if (targetController != null) {
            RegisterInputs();
        } 
        else {
            Debug.LogWarning($"{name}: no target controller set, make sure to set a target controller!");
        }
    }

    //-------------------------------------------------
    //-----------------register inputs-----------------
    //-------------------------------------------------
    private void RegisterInputs()
    {
        RegisterButtonInputs();
        RegisterAxisInputs();
        RegisterDirectionalInputs();
    }

    //--------------------button inputs------------------
    private void RegisterButtonInputs()
    {
        foreach (ButtonInput input in buttonInputs) {
            RegisterButtonInput(input);
        }
    }
    private void RegisterButtonInput(ButtonInput input)
    {
        foreach (ButtonCodes inputCode in input.inputCodes) {
            switch (inputCode) {
                case ButtonCodes.trigger:
                    targetController.triggerButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;

                case ButtonCodes.grip:
                    targetController.gripButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;

                case ButtonCodes.primary:
                    targetController.primaryButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;

                case ButtonCodes.secondary:
                    targetController.secondaryButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;

                case ButtonCodes.primaryAxis:
                    targetController.primaryAxisButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;

                case ButtonCodes.secondaryAxis:
                    targetController.secondaryAxisButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;

                case ButtonCodes.menu:
                    targetController.menuButtonInput.onValueChanged.AddListener(input.OnStateChanged);
                    break;
            }
        }
    }

    //-------------------axis inputs---------------------
    private void RegisterAxisInputs()
    {
        foreach (AxisInput input in axisInputs) {
            RegisterAxisInput(input);
        }
    }
    private void RegisterAxisInput(AxisInput input)
    {
        foreach (AxisCodes inputCode in input.inputCodes) {
            switch (inputCode) {
                case AxisCodes.trigger:
                    targetController.triggerInput.onValueChanged.AddListener((float f) => input.output?.Invoke(f));
                    break;

                case AxisCodes.grip:
                    targetController.gripInput.onValueChanged.AddListener((float f) => input.output?.Invoke(f));
                    break;

                case AxisCodes.primaryAxisX:
                    targetController.primaryAxisInput.onValueChanged.AddListener((Vector2 v) => input.output?.Invoke(v.x));
                    break;

                case AxisCodes.primaryAxisY:
                    targetController.primaryAxisInput.onValueChanged.AddListener((Vector2 v) => input.output?.Invoke(v.y));
                    break;

                case AxisCodes.secondaryAxisX:
                    targetController.secondaryAxisInput.onValueChanged.AddListener((Vector2 v) => input.output?.Invoke(v.x));
                    break;

                case AxisCodes.secondaryAxisY:
                    targetController.secondaryAxisInput.onValueChanged.AddListener((Vector2 v) => input.output?.Invoke(v.y));
                    break;
            }
        }
    }

    //--------------------directional inputs-----------------------
    private void RegisterDirectionalInputs()
    {
        foreach (DirectionalInput input in directionalInputs) {
            RegisterDirectionalInput(input);
        }
    }
    private void RegisterDirectionalInput(DirectionalInput input)
    {
        foreach (DirectionalCodes inputCode in input.inputCodes) {
            switch (inputCode) {
                case DirectionalCodes.primaryAxis:
                    targetController.primaryAxisInput.onValueChanged.AddListener((Vector2 v) => input.output.Invoke(v));
                    break;

                case DirectionalCodes.secondaryAxis:
                    targetController.secondaryAxisInput.onValueChanged.AddListener((Vector2 v) => input.output.Invoke(v));
                    break;
            }
        }
    }

    //---------------------------------------------------
    //--------------------read inputs--------------------
    //---------------------------------------------------
    private void Update()
    {
        UpdatePressedButtons();
    }

    private void UpdatePressedButtons()
    {
        foreach (ButtonInput input in buttonInputs) {
            if (input.pressed) {
                input.onButtonHeld?.Invoke();
            }
        }
    }
}
