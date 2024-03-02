using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] int maxHealth;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    private void Awake()
    {
        UpdateHearts();
    }
    public void UpdateHearts()
    {
        for(int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            } else
            {
                hearts[i].enabled = false;
            }
        }
    }
    public void ResetHearts()
    {
        health = maxHealth;
        UpdateHearts();
    }
    public void ChangeHealth(int value)
    {
        int result = health += value;
        if(result > maxHealth)
        {
            health = maxHealth;
        }
        else if (result < 1)
        {
            PlayerDie();
        }
        else
        {
            health = result;
        }
        UpdateHearts();
    }
    public void PlayerDie()
    {
        Debug.Log("You Dead");
    }
}
