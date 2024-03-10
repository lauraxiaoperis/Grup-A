using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationContoller : MonoBehaviour
{
    CharacterController _characterController;
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponentInParent<CharacterController>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed;
        Vector2 horizontalVelocity = new Vector2(_characterController.velocity.x , _characterController.velocity.z);
        if(horizontalVelocity.magnitude == 0 ){
            forwardPressed = false;
            animator.SetBool(isRunningHash, false);
        }
        else if(horizontalVelocity.magnitude > 0 || horizontalVelocity.magnitude < 1f){
            forwardPressed = true;
            animator.SetBool(isRunningHash, false);
        }
        else{
            animator.SetBool(isRunningHash, true);
            forwardPressed = false;
        }
        
        

        bool IsRunning = animator.GetBool(isRunningHash);
        bool IsWalking = animator.GetBool(isWalkingHash);
        bool IsJumping = animator.GetBool("IsJumping");
        //bool forwardPressed = Input.GetKey("w"); //wasd
        bool runPressed = Input.GetKey("left shift");
        bool jumpPressed = Input.GetKey("space");

        if(!IsWalking && forwardPressed){
            animator.SetBool(isWalkingHash, true);
        }

        if(IsWalking && !forwardPressed){
            animator.SetBool(isWalkingHash, false);
        }

        if(!IsRunning && (forwardPressed && runPressed)){
            animator.SetBool(isRunningHash, true);
        }

        //if(IsRunning && (!forwardPressed || !runPressed)){
          //  animator.SetBool(isRunningHash, false);
        //}

        //if()
    }
}
