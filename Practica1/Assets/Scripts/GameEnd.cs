using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public GameObject winPrefab;
    public GameObject winMessage;
    private void Awake()
    {
        winMessage.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        for(int i = 1; i<4; i++)
        {
            Instantiate(winPrefab, transform.position + Vector3.up * 40*i, transform.rotation);
        }
        winMessage.SetActive(true);
        Destroy(gameObject);
    }
}
