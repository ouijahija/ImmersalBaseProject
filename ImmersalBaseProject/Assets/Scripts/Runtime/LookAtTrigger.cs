using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LookAtTrigger : MonoBehaviour
{
    public Camera camera;
    public float angle = 45;
    public UnityEvent trigger;

    private void Update()
    {
        if (Vector3.Angle(camera.transform.forward, (transform.position - camera.transform.position)) <= angle)
        {
            trigger.Invoke();
            Destroy(this);
        }
    }
}
