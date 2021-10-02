using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public UnityEvent onTakeDamage;
    public UnityEvent onOutOfHealth;

    private void Start()
    {
        if (currentHealth <= 0 || currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        onTakeDamage.Invoke();
        currentHealth -= damage;
        if (currentHealth <= 0)
            onOutOfHealth.Invoke();
    }
}