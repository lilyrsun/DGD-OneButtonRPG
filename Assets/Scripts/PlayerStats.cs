using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public int maxHealth = 50;
    [HideInInspector] public int currentHealth;

    public void Init() => currentHealth = maxHealth;
    public void TakeDamage(int amt) => currentHealth = Mathf.Max(0, currentHealth - amt);
    public bool IsDead => currentHealth <= 0;
}
