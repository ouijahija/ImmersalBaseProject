using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIBillboard : MonoBehaviour
{
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        var toCam = mainCamera.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(toCam, Vector3.up) * Quaternion.Euler(0, 0, 180);
    }
}
