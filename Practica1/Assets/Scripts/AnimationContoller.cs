using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationContoller : MonoBehaviour
{
    CharacterController _characterController;
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponentInParent<CharacterController>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
        isJumpingHash = Animator.StringToHash("IsJumping");
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
        bool IsJumping = animator.GetBool(isJumpingHash);
        //bool forwardPressed = Input.GetKey("w"); //wasd
        bool runPressed = Input.GetKey("left shift");
        bool jumpPressed = Input.GetKey("space");

        //No camina i presiona cualsevol de wasd = caminar
        if(!IsWalking && forwardPressed){
            animator.SetBool(isWalkingHash, true);
        }

        //Esta caminant i deixa de presionar = no caminar
        if(IsWalking && !forwardPressed){
            animator.SetBool(isWalkingHash, false);
        }

        //No corre, i clica caminar+correr = correr
        if(!IsRunning && (forwardPressed && runPressed)){
            animator.SetBool(isRunningHash, true);
        }

        //Corre, i no hi ha clicat ni caminar o correr = para de correr
        if(IsRunning && (!forwardPressed || !runPressed)){
            animator.SetBool(isRunningHash, false);
        }

        //Salta, si no avança ni corre = saltar
        if(jumpPressed && (!forwardPressed && !runPressed)){
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
        }

        //Salta, i està avançant = saltar
        if(IsWalking && (jumpPressed && forwardPressed)){
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
        }

        //Salta, i està corrent = salta
        if(IsRunning && jumpPressed ){
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isWalkingHash, false);
        }

        //Si està saltant i deixa de saltar sense clicar res.
        if(IsJumping && (!jumpPressed && (!forwardPressed || !runPressed))){
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isWalkingHash, false);
        }

        // Si està saltant i deixa de saltar + wasd es posa a caminar
        if(IsJumping && (!jumpPressed && forwardPressed)){
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isJumpingHash, false);
        }

        // Si està saltant i deixa de saltar + wasd+shift es posa a correr
        if(IsJumping && (!jumpPressed && (forwardPressed && runPressed))){
            animator.SetBool(isRunningHash, true);
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isWalkingHash, false);
        }
        
    }
}
