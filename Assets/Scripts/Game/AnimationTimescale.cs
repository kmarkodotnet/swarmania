using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTimescale : MonoBehaviour
{
    Animator m_Animator;

    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
    }

    void OnGUI()
    {
        m_Animator.speed = Constants.TimeScale;
    }
}
