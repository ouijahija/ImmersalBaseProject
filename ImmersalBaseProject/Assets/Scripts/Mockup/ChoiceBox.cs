using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChoiceBox : MonoBehaviour
{
    public UnityEvent OnNext;

    private void OnEnable()
    {
        Cursor.visible = true;
        PlayerController.cameraLocked = true;
    }

    private void OnDisable()
    {
        Cursor.visible = false;
        PlayerController.cameraLocked = false;
    }

    public void Next()
    {
        OnNext?.Invoke();

        gameObject.SetActive(false);
    }
}
