using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public new Camera camera;
    public NavMeshAgent agent;

    public float lookStrength = 90;

    Vector2 cameraRotation;
    Vector2 lastMousePos;

    Vector2 targetRotation;
    Vector2 targetRotationVelocity;

    public static bool cameraLocked;

    private void Start()
    {
        Cursor.visible = false;
        cameraRotation = camera.transform.rotation.eulerAngles;
        targetRotation = cameraRotation;
        lastMousePos = Input.mousePosition;
    }

    private void Update()
    {
        UpdateMove();

        UpdateCamera();
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        var yRot = cameraRotation.y * Mathf.Deg2Rad;

        if (x == 0 && y == 0)
        {
            agent.isStopped = true;
            return;
        }

        Vector3 delta = Vector2.zero;
        delta.x = Mathf.Cos(yRot) * x + Mathf.Sin(yRot) * y; //Mathf.Cos(yRot) * x +
        delta.z = -Mathf.Sin(yRot) * x + Mathf.Cos(yRot) * y;

        var targetPos = transform.position + delta;
        agent.isStopped = false;
        agent.destination = targetPos;
    }

    private void UpdateCamera()
    {
        Vector2 mousePos = Input.mousePosition;

        if (!cameraLocked)
        {
            var delta = mousePos - lastMousePos;

            targetRotation.y += delta.x * lookStrength * Time.deltaTime;
            targetRotation.x -= delta.y * lookStrength * Time.deltaTime;

            cameraRotation = Vector2.SmoothDamp(cameraRotation, targetRotation, ref targetRotationVelocity, 0.1f);

            camera.transform.rotation = Quaternion.Euler(cameraRotation);
        }

        lastMousePos = mousePos;
    }
}
