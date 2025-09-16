using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string displayName = "Slime";
    public int maxHP = 20;
    public int damageMin = 2;
    public int damageMax = 6;
    [Range(0f, 1f)] public float attackChance = 0.6f; // new: chance this enemy attacks
}


public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public int CurrentHP { get; private set; }

    void Awake() { CurrentHP = data.maxHP; }

    public int TakeHit(int dmg)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - dmg);
        return CurrentHP;
    }

    public int RollDamage() => Random.Range(data.damageMin, data.damageMax + 1);
    public bool IsDead => CurrentHP <= 0;
}
