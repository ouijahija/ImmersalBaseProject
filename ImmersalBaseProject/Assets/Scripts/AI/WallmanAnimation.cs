using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WallmanAnimation : MonoBehaviour
{
    public UnityEvent nextVO;
    public Animator animator;

    public void Idle()
    {
        animator.SetBool("Move", false);
    }
    public void Move()
    {
        animator.SetBool("Move", true);
    }
    public void NextVO()
    {
        nextVO.Invoke();
    }
}
