using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SpiderAI : MonoBehaviour
{
    public LayerMask groundMask;

    public Transform graphics;
    public Transform target;
    public float rotationAlignmentSpeed = 1;

    NavMeshAgent agent;
    Vector3 lastTargetPos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (lastTargetPos != target.position)
        {
            agent.SetDestination(target.position);
            lastTargetPos = target.position;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 10, groundMask))
        {
            var rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            graphics.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * rotationAlignmentSpeed);
        }
    }
}
