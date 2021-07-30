using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationControl : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private bool isAnimating = false;
    public bool IsAnimating
    {
        get => isAnimating;
        set => isAnimating = value;
    }

    [SerializeField] private Animator anim = null;

    private float normalizedTime = 0f;

    #endregion

    #region UnityCallbacks

    private void Awake()
    {
        if (!anim)
            anim = GetComponent<Animator>();
    }

    #endregion

    #region methods

    public void StartRotation()
    {
        Debug.Log("Start");
        anim.speed = 1;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("RotateY"))
            normalizedTime = stateInfo.normalizedTime;
        anim.Play("Base Layer.RotateY", 0, normalizedTime);
    }

    public void StopRotation()
    {
        Debug.Log("Stop");
        anim.speed = 0;
    }

    #endregion
}
