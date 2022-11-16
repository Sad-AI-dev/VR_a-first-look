using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevKit;

public class TimerActivator : MonoBehaviour
{
    public IntervalTimerManager timers;

    public void StartTimer(string name)
    {
        timers.ActivateTimer(name);
    }
}
