using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puerta : MonoBehaviour
{
    public string NombreAnimacion;
    public Animator animator;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            animator.Play(NombreAnimacion);
            Destroy(gameObject);
        }
    }
}
