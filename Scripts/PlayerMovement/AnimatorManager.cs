using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    int horizontalValue;
    int verticalValue;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalValue = Animator.StringToHash("Horizontal");
        verticalValue = Animator.StringToHash("Vertical");

    }
    //Cursor
    public void ChangeAnimatorValue(float horizotalMovement,float vertotalMovement,bool isSprinti)
    {
        float snappedHorizotalMovement;
        float snappedVerticalMovement;

        #region snapped Horizantal
        if (horizotalMovement > 0f && horizotalMovement < 0.55f)
        {
            snappedHorizotalMovement = 0.5f;
        }
        else if (horizotalMovement > 0.55f)
        {
            snappedHorizotalMovement = 1f;
        }
        else if (horizotalMovement < 0 && horizotalMovement > -0.55f)
        {
            snappedHorizotalMovement = -0.55f;
        }
        else if (horizotalMovement < -0.55f ) 
        {
            snappedHorizotalMovement = -1f;
        }
        else
        {
            snappedHorizotalMovement = 0f;
        }
        #endregion

        #region snapped vertical
        if (vertotalMovement > 0f && vertotalMovement < 0.55f)
        {
            snappedVerticalMovement = 0.5f;
        }
        else if (vertotalMovement > 0.55f)
        {
            snappedVerticalMovement = 1f;
        }
        else if (vertotalMovement < 0 && vertotalMovement > -0.55f)
        {
            snappedVerticalMovement = -0.55f;
        }
        else if (vertotalMovement < -0.55f)
        {
            snappedVerticalMovement = -1f;
        }
        else
        {
            snappedVerticalMovement = 0f;
        }
        #endregion

        if(isSprinti)
        {
            snappedHorizotalMovement=horizotalMovement;
            snappedVerticalMovement = 2;
        }
        animator.SetFloat(horizontalValue, snappedHorizotalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalValue, snappedVerticalMovement, 0.1f, Time.deltaTime);

    }

    public void PlayTargetAnim(string targetAnim,bool isInterActing)
    {
        animator.SetBool("isIneracting", isInterActing);
        animator.CrossFade(targetAnim, 0.2f);
    }
}
