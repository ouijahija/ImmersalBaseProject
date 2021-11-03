#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Based on the tutorial from DitzelGames 'C# Inverse Kinematics in Unity' https://www.youtube.com/watch?v=qqOAzn05fvk&t=23s
/// </summary>

public class MyIK : MonoBehaviour
{
#if UNITY_EDITOR
    public Color gizmoColor = Color.white;
#endif
    [Range(0, 1)]
    public float strength = 1;
    public int chainLength = 2;

    public Transform target;
    public Transform pole;

    [Header("Soler Parameters")]
    public int maxIterations = 15;

    public float tolerance = 0.01f;

#if UNITY_EDITOR
    [Header("Editor only variables")]
    public Transform root;
    public enum Axis { x, y, z, nx, ny, nz }
    public Axis poleAlignment = Axis.x;
    public float poleDistance = 1f;
#endif

    private Transform[] bones;
    private Vector3[] positions;
    private float[] boneLengths;
    private float totalLength;

    //Rotation
    private Vector3[] startDirectionSuccessor;
    private Quaternion[] startRotationBone;
    private Quaternion startRotationTarget;
    private Quaternion startRotationRoot;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        boneLengths = new float[chainLength];

        startDirectionSuccessor = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        startRotationTarget = target.rotation;
        totalLength = 0f;

        var current = transform;

        for (int i = chainLength; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if (i == chainLength)
            {
                startDirectionSuccessor[i] = target.position - current.position;
            }
            else
            {
                startDirectionSuccessor[i] = bones[i + 1].position - current.position;
                boneLengths[i] = (bones[i + 1].position - current.position).magnitude;
                totalLength += boneLengths[i];
            }

            current = current.parent;
        }
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null)
            return;

        if (bones.Length != chainLength + 1)
            Init();

        //Get Positions
        for (int i = 0; i < bones.Length; i++)
            positions[i] = bones[i].position;

        var rootRotation = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var rootRotationDifference = rootRotation * Quaternion.Inverse(startRotationRoot);

        //Calculate
        if ((target.position - bones[0].position).sqrMagnitude >= totalLength * totalLength)
        {
            var dir = (target.position - positions[0]).normalized;
            for (int i = 1; i < positions.Length; i++)
                positions[i] = positions[i - 1] + dir * boneLengths[i - 1];
        }
        else
        {
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                //Backward step
                positions[chainLength] = target.position;
                for (int i = chainLength - 1; i > 0; i--)
                {
                    var dir = (positions[i] - positions[i + 1]).normalized;
                    positions[i] = positions[i + 1] + dir * boneLengths[i];
                }

                //forward step
                for (int i = 1; i < chainLength; i++)
                {
                    var dir = (positions[i] - positions[i - 1]).normalized;
                    positions[i] = positions[i - 1] + dir * boneLengths[i - 1];
                }

                if ((positions[chainLength] - target.position).sqrMagnitude < tolerance * tolerance)
                    break;
            }
        }

        //move towards pole
        if (pole != null)
        {
            for (int i = 1; i < chainLength; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        //Set Position & rotation
        for (int i = 0; i < bones.Length; i++)
        {
            if (i == chainLength)
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSuccessor[i], positions[i + 1] - positions[i]) * startRotationBone[i];

            bones[i].position = positions[i];
        }
    }

#if UNITY_EDITOR
    public void PlacePole()
    {
        Init();

        if (pole == null)
        {
            pole = new GameObject(name + "_pole").transform;
            pole.position = transform.position;

            var rig = root.Find("Rig");
            if (!rig)
            {
                rig = new GameObject("Rig").transform;
                rig.parent = root;
                rig.localPosition = Vector3.zero;
                rig.localRotation = Quaternion.identity;
            }

            pole.parent = rig;
        }

        Vector3 polePosition = Vector3.zero;

        for (int i = 1; i < chainLength; i++)
        {
            var ikAxis = positions[i + 1] - positions[i - 1];

            var plane = new Plane(ikAxis, positions[i - 1]);
            var projectedBone = plane.ClosestPointOnPlane(positions[i]);
            var poleAxis = GetBoneAxis(bones[i], poleAlignment);
            var angle = Vector3.SignedAngle(poleAxis, projectedBone - positions[i - 1], plane.normal);

            var pos = Quaternion.AngleAxis(angle, plane.normal) * poleAxis * poleDistance + positions[i - 1] + ikAxis / 2;
            polePosition += pos;
        }

        polePosition /= (chainLength - 1);
        pole.position = polePosition;
    }

    public Vector3 GetBoneAxis(Transform bone, Axis axis)
    {
        switch (axis)
        {
            case Axis.x:
                return bone.right;
            case Axis.y:
                return bone.up;
            case Axis.z:
                return bone.forward;
            case Axis.nx:
                return -bone.right;
            case Axis.ny:
                return -bone.up;
            case Axis.nz:
                return -bone.forward;
            default:
                return Vector3.up;
        }
    }
#endif

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        var current = transform;
        for (int i = 0; i < chainLength && current != null; i++)
        {
            if (current.parent != null)
            {
                var scale = (current.position - current.parent.position).magnitude * 0.1f;
                Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
                Handles.color = gizmoColor;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            }

            current = current.parent;
        }

        if (target != null)
        {
            var outline = Color.green;
            var face = outline;
            face.a = 0.5f;

            Gizmos.color = face;
            Gizmos.DrawCube(target.position, Vector3.one * 0.1f);

            Gizmos.color = outline;
            Gizmos.DrawWireCube(target.position, Vector3.one * 0.1f);
        }

        if (pole != null)
        {
            var color = Color.red;
            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawSphere(pole.position, 0.1f);
        }
#endif
    }

    private void OnValidate()
    {
        chainLength = Mathf.Max(2, chainLength);

#if UNITY_EDITOR
        if (target == null && root != null)
        {
            target = new GameObject(name + "_target", typeof(MyIKTarget)).transform;
            target.position = transform.position;

            var rig = root.Find("Rig");
            if (!rig)
            {
                rig = new GameObject("Rig").transform;
                rig.parent = root;
                rig.localPosition = Vector3.zero;
                rig.localRotation = Quaternion.identity;
            }

            target.parent = rig;
        }
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MyIK)), CanEditMultipleObjects()]
public class MyIKEditor : Editor
{
    MyIK ik;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (ik.target != null)
        {
            if (GUILayout.Button("Recenter target"))
            {
                ik.target.position = ik.transform.position;
            }

            if (GUILayout.Button("Place pole target"))
            {
                ik.PlacePole();
            }
        }
    }

    private void OnEnable()
    {
        ik = target as MyIK;
    }
}
#endif