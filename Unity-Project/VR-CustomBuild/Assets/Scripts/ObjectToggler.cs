using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectToggler : MonoBehaviour
{
    [Header("settings")]
    public GameObject startObject;
    public GameObject altObject;
    [Space(10f)]
    [Range(0.01f, 0.9f)] public float inputSensitivity = 0.1f;
    [Header("Events")]
    public UnityEvent onSwitchStart;
    public UnityEvent onSwitchEnd;
    //vars
    private bool lastState;

    private void Start()
    {
        Toggle(false);
    }

    public void TryToggle(float input)
    {
        bool validInput = input > 1f - inputSensitivity;
        if (validInput != lastState) {
            Toggle(validInput);
            lastState = validInput;
        }
    }

    public void Toggle(bool state)
    {
        //events
        if (state) { onSwitchStart?.Invoke(); }
        else { onSwitchEnd?.Invoke(); }
        //toggle objects
        startObject.SetActive(!state);
        altObject.SetActive(state);
    }
}
