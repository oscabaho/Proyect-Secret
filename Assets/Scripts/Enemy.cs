using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float enemyHealth = 100f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecieveDamage(float damage)
    {
        enemyHealth -= damage;
        Debug.Log(enemyHealth);
        if(enemyHealth <= 0) { Death(); }
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
