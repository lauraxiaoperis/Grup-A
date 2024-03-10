using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationContoller : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
    }

    // Update is called once per frame
    void Update()
    {
        bool IsRunning = animator.GetBool(isRunningHash);
        bool IsWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        if(!IsWalking && forwardPressed){
            animator.SetBool(isWalkingHash, true);
        }

        if(IsWalking && !forwardPressed){
            animator.SetBool(isWalkingHash, false);
        }

        if(!IsRunning && (forwardPressed && runPressed)){
            animator.SetBool(isRunningHash, true);
        }

        if(IsRunning && (!forwardPressed || !runPressed)){
            animator.SetBool(isRunningHash, false);
        }
    }
}
