using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abrirpuerta : MonoBehaviour
{
    public string NombreAnimacionOpen;
    public string NombreAnimacionClose;
    public Collider doorCollider;
    public Animator animator;

    //Abrir puerta sin llave
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            animator.Play(NombreAnimacionOpen);
            doorCollider.enabled = false;
        }
    }

    //Cerrar puerta sin llave
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            animator.Play(NombreAnimacionClose);
            doorCollider.enabled = true;
        }
    }
}
