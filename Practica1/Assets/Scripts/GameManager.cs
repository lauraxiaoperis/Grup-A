using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerHealth _playerHealth;
    [SerializeField] private PlayerController _playerController;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        //SI ES LA PRIMERA INSTANCIA, SETEAR COMO [LA] INSTANCIA Y METERLA
        //EN DontDestroyOnLoad
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerHealth = gameObject.GetComponent<PlayerHealth>();
    }
    public void ChangeHealth(int value)
    {
        _playerHealth.ChangeHealth(value);
    }
}
