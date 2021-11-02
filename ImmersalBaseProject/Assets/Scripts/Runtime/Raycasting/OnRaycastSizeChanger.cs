using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class OnRaycastSizeChanger : MonoBehaviour, IRaycastSelectable
{
   
    public void OnRaycastReceived()
    {
        var currentScale = transform.localScale;

        if (currentScale == Vector3.one)
            transform.localScale *= 3;
        else 
            transform.localScale = Vector3.one;

    }
}
