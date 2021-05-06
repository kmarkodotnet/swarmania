using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] int shield;

    int maxHealth;

    private void Awake()
    {
        maxHealth = health;
    }
    private void Update()
    {
    }

    public int GetHealth()
    {
        return health;
    }
    public void ResetHealth()
    {
        health = maxHealth;
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Dead);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public bool DealDamage(int damage)
    {
        var realDamage = Mathf.Max(damage - shield,0);
        if (health - realDamage >= 0)
        {
            health -= realDamage;
            return true;
        }
        else
        {
            Die();
            return false;
        }
    }

    private void Die()
    {
        if(new CommonService().IsAlive(gameObject))
        {
            health = 0;
            new CommonService().MakeDead(gameObject);
            
            gameObject.GetComponent<SelectableObject>().DestroySelection();
        }
        
        //Destroy(gameObject);
    }

    internal void AddHealth(int healthIncreasingValue)
    {
        health += healthIncreasingValue;
    }
}
