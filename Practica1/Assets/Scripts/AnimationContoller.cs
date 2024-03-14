using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationContoller : MonoBehaviour
{
    PlayerController _pcScript;
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int IsCrouchingHash;
    // Start is called before the first frame update
    void Start()
    {
        _pcScript = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
        isJumpingHash = Animator.StringToHash("IsJumping");
        IsCrouchingHash = Animator.StringToHash("IsCrouching");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            AnimatorControllerParameter parameter = animator.GetParameter(i);
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                // Set the boolean parameter to false
                animator.SetBool(parameter.name, false);
            }
        }
        if (_pcScript.isJumping)
        {
            animator.SetBool(isJumpingHash, true);
        }
        else if (_pcScript.isCrouching)
        {
            animator.SetBool(IsCrouchingHash, true);
        }
        else if (_pcScript.horizontalDirection.sqrMagnitude != 0 && _pcScript.isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        if (_pcScript.horizontalDirection.sqrMagnitude != 0)
        {
            animator.SetBool(isWalkingHash, true);
        }
    }
}
