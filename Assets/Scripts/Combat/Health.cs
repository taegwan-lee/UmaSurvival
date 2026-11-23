using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityDuration = 0f;

    private int currentHealth;
    private float invincibilityTimer;
    private bool isDead;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    //플레이어 사망 
    public event Action Died;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if(!isActiveAndEnabled || 
        isDead ||
        damage <= 0 ||
        invincibilityTimer >0f)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        invincibilityTimer = invincibilityDuration;

        if(currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(isDead)
        {
            return;
        }

        isDead = true;

        Died?.Invoke();
    }
}
