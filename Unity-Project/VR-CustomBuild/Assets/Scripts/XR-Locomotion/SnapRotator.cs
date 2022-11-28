using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapRotator : MonoBehaviour
{
    private enum InputState {
        left, idle, right
    }

    public Transform target;
    [Header("Settings")]
    public float turnAngle = 10f;
    [Range(0.01f, 0.9f)] public float inputSensitivity = 0.1f; //0.1 means low sensitivity, means input must reach 0.9 to activate

    //vars
    private InputState currentInputState;
    private bool turnLeftReq, turnRightReq;
    
    public void TryTurn(float input)
    {
        InputState newState = GetNewInputState(input);
        if (newState != currentInputState) {
            currentInputState = newState; //update current
            TurnCheck();
        }
    }

    private InputState GetNewInputState(float input)
    {
        if (Mathf.Abs(input) < 1 - inputSensitivity) { return InputState.idle; }
        else { return input > 0f ? InputState.right : InputState.left; }
    }

    private void TurnCheck()
    {
        switch (currentInputState) {
            case InputState.left:
                turnLeftReq = true;
                break;

            case InputState.right:
                turnRightReq = true;
                break;
        }
    }

    //-------------turn--------------------
    public void TurnLeft()
    {
        target.Rotate(new Vector3(0, -turnAngle, 0));
    }

    public void TurnRight()
    {
        target.Rotate(new Vector3(0, turnAngle, 0));
    }

    private void FixedUpdate()
    {
        if (turnLeftReq) { 
            TurnLeft();
            turnLeftReq = false;
        }
        else if (turnRightReq) { 
            TurnRight();
            turnRightReq = false;
        }
    }
}
