using UnityEngine;

//TODO: must use by castles
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
