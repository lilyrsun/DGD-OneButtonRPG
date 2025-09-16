using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Progress")]
    public int enemiesToWin = 6;   // 5 regulars + 1 boss
    private int enemiesDefeated = 0;

    [Header("Player")]
    public int playerMaxHP = 50;
    public int playerHP;
    public Vector2Int playerDamageRange = new Vector2Int(3, 10);

    [Header("UI")]
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text logText;
    public Button attackButton;

    [Header("Enemies")]
    public Enemy[] regularEnemyPrefabs;  // <- rename from enemyPrefabs
    public Enemy bossPrefab;             // <- new slot for the boss
    public Transform enemySpawnPoint;
    private Enemy currentEnemy;

    [Header("Flow")]
    public float attackCooldown = 1.0f; // seconds
    //public float enemyAttackChance = 0.6f; // 60% chance after player attack
    private bool canAct = true;

    void Start()
    {
        playerDamageRange.x = Mathf.Max(1, playerDamageRange.x); // never 0-damage player hits
        playerHP = playerMaxHP;
        SpawnNextEnemy();
        UpdateUI();
        attackButton.onClick.AddListener(OnAttackClicked);
    }


    void SpawnNextEnemy()
    {
        if (currentEnemy != null) Destroy(currentEnemy.gameObject);

        // If we’re about to spawn the final enemy, spawn the boss
        bool isBoss = (enemiesDefeated >= enemiesToWin - 1);

        Enemy prefab;
        if (isBoss)
        {
            prefab = bossPrefab;
        }
        else
        {
            // pick a random regular enemy
            prefab = regularEnemyPrefabs[Random.Range(0, regularEnemyPrefabs.Length)];
        }

        currentEnemy = Instantiate(prefab, enemySpawnPoint.position, Quaternion.identity);

        Log(isBoss
            ? $"⚠ BOSS: {currentEnemy.data.displayName} challenges you!"
            : $"A wild {currentEnemy.data.displayName} appears!");

        UpdateUI();
    }


    void OnAttackClicked()
    {
        if (!canAct) return;
        StartCoroutine(PlayerTurnRoutine());
    }

    System.Collections.IEnumerator PlayerTurnRoutine()
    {
        canAct = false;
        attackButton.interactable = false;

        int dmg = Random.Range(playerDamageRange.x, playerDamageRange.y + 1);
        currentEnemy.TakeHit(dmg);
        Log($"You hit {currentEnemy.data.displayName} for {dmg}.");
        UpdateUI();

        if (currentEnemy.IsDead)
        {
            Log($"{currentEnemy.data.displayName} is defeated!");
            enemiesDefeated++;
            if (enemiesDefeated >= enemiesToWin)
            {
                EndStateHolder.PlayerWon = true;
                SceneManager.LoadScene("End");
                yield break;
            }

            yield return new WaitForSeconds(0.4f);
            SpawnNextEnemy();
        }
        else
        {
            // Enemy may attack after your turn
            yield return new WaitForSeconds(0.4f);
            if (Random.value < currentEnemy.data.attackChance)
            {
                int eDmg = currentEnemy.RollDamage();
                playerHP = Mathf.Max(0, playerHP - eDmg);
                Log($"{currentEnemy.data.displayName} hits you for {eDmg}.");
                UpdateUI();
                if (playerHP <= 0)
                {
                    EndStateHolder.PlayerWon = false;
                    SceneManager.LoadScene("End");
                    yield break;
                }
            }
            else
            {
                Log($"{currentEnemy.data.displayName} hesitates...");
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        attackButton.interactable = true;
        canAct = true;
    }

    void UpdateUI()
    {
        playerHpText.text = $"HP: {playerHP}/{playerMaxHP}";
        enemyHpText.text = $"{currentEnemy.data.displayName} HP: {currentEnemy.CurrentHP}/{currentEnemy.data.maxHP}";
    }

    void Log(string s) { logText.text = s; }
}
