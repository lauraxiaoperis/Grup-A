using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abrirpuerta : MonoBehaviour
{
    public string NombreAnimacionOpen;
    public string NombreAnimacionClose;
    public Collider doorCollider;
    public Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        animator.Play(NombreAnimacionOpen);
        doorCollider.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        animator.Play(NombreAnimacionClose);
        doorCollider.enabled = true;
    }
}
