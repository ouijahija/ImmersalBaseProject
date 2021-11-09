using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedTrigger : MonoBehaviour
{
    public float offset = 1;
    public float every = .1f;


    public UnityEvent Trigger;
    private void Start()
    {
        InvokeRepeating(nameof(InvokeTrigger), offset, every);
    }

    void InvokeTrigger() => Trigger?.Invoke();
}
