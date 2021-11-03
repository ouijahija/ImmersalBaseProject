using UnityEngine;

public class MyIKTarget : MonoBehaviour
{
    public LayerMask groundMask;

    [Header("Movement")]
    public float stepDuration = 0.5f;
    public float stepHeight = 0.2f;
    public MyIKTarget[] dontLiftAtTheSameTime;

    [Header("Probing")]
    public float probeHeight = 1;
    public float triggerRadius = 0.2f;

    public Transform root;

    Vector3 lastFixedPos;
    Vector3 fixedPos;
    Vector3 rootOffset;
    float stepTime = 0;

    bool locked = true;

    Vector3 lastTriggerCenter;
    Vector3 triggerCenterDelta;

    Vector3 currentPositionVel;

    Vector3 TriggerCenter { get => root.position + root.rotation * rootOffset; }

    private void Awake()
    {
        transform.position = Place(transform.position);
        fixedPos = transform.position;
        CalculateRootOffset();
    }

    private Vector3 Place(Vector3 probePos)
    {
        RaycastHit hit;
        Ray ray = new Ray(probePos + Vector3.up * probeHeight, Vector3.down);
        if (Physics.Raycast(ray, out hit, probeHeight * 2, groundMask))
        {
            return hit.point;
        }

        return probePos;
    }

    private void CalculateRootOffset()
    {
        rootOffset = transform.position - root.position;
    }

    private void LateUpdate()
    {
        if (locked)
        {
            SetPosition(fixedPos);
            var delta = TriggerCenter - transform.position;
            if (delta.sqrMagnitude > triggerRadius * triggerRadius)
            {
                triggerCenterDelta = (TriggerCenter - lastTriggerCenter).normalized;

                lastFixedPos = transform.position;
                fixedPos = Place(TriggerCenter + triggerCenterDelta * triggerRadius * 0.98f);
                stepTime = Time.time;

                bool canUnlock = true;

                foreach (var neighbour in dontLiftAtTheSameTime)
                {
                    if (!neighbour.locked)
                    {
                        canUnlock = false;
                        break;
                    }
                }

                if (canUnlock)
                    locked = false;
            }
        }
        else
        {
            //stepTime += Time.deltaTime;
            var t = Mathf.Clamp01((Time.time - stepTime) / stepDuration);

            fixedPos = Place(TriggerCenter + triggerCenterDelta * triggerRadius);
            SetPosition(Place(Vector3.Lerp(lastFixedPos, fixedPos, t)) + Vector3.up * Mathf.Sin(t * Mathf.PI) * stepHeight);
            if (t == 1)
                locked = true;
        }
        lastTriggerCenter = TriggerCenter;
    }

    void SetPosition(Vector3 worldPos)
    {
        transform.position = Vector3.SmoothDamp(transform.position, worldPos, ref currentPositionVel, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(TriggerCenter, triggerRadius);
        else
            Gizmos.DrawWireSphere(transform.position, triggerRadius);

    }

}
