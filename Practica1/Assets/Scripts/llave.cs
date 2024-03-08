using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class llave : MonoBehaviour
{
    public GameObject Objetollave;
    public GameObject ColliderDetector;
    public Collider ColliderPuerta;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            ColliderDetector.gameObject.SetActive(true);
            ColliderPuerta.enabled=false;
            Destroy(Objetollave);
        }

    }
}
