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
    public Vector2Int playerDamageRange = new Vector2Int(3, 15);

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

    public Slider playerHpBar;
    public Slider enemyHpBar;
    public Image enemyHpFill;
    public Image playerHPFill;

    public PlayerAnimatorController playerAnim;

    public AudioSource sfxSource;
    public AudioClip attackSfx;

    void Start()
    {
        playerDamageRange.x = Mathf.Max(1, playerDamageRange.x); // never 0-damage player hits

        playerHP = playerMaxHP;
        if (playerHpBar) { 
            playerHpBar.minValue = 0; 
            playerHpBar.maxValue = playerMaxHP; 
            playerHpBar.value = playerHP; 
        }
        if (playerHPFill) 
            playerHPFill.color = EvaluateHealthColor(1f);
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

        if (enemyHpBar)
        {
            enemyHpBar.minValue = 0;
            enemyHpBar.maxValue = currentEnemy.data.maxHP;
            enemyHpBar.value = currentEnemy.CurrentHP;
        }
        if (enemyHpFill)
        {
            float pct = (float)currentEnemy.CurrentHP / currentEnemy.data.maxHP;
            enemyHpFill.color = EvaluateHealthColor(pct);
        }

        Log(isBoss
            ? $"BOSS: {currentEnemy.data.displayName} challenges you!"
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
        if (playerAnim) 
            playerAnim.PlayAttack();
        if (sfxSource && attackSfx)
            sfxSource.PlayOneShot(attackSfx);
        currentEnemy.TakeHit(dmg);
        StartCoroutine(AnimateHp(enemyHpBar, enemyHpFill, currentEnemy.CurrentHP, currentEnemy.data.maxHP));
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
                StartCoroutine(AnimateHp(playerHpBar, playerHPFill, playerHP, playerMaxHP));   // NEW
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
        //if (playerHpBar) playerHpBar.value = playerHP;

        if (currentEnemy != null)
            enemyHpText.text = $"{currentEnemy.data.displayName} HP: {currentEnemy.CurrentHP}/{currentEnemy.data.maxHP}";
        else
            enemyHpText.text = "";
    }

    Color EvaluateHealthColor(float pct)
    {
        pct = Mathf.Clamp01(pct);
        if (pct <= 0.5f)
            return Color.Lerp(Color.red, Color.yellow, pct / 0.5f);          // 0..50%: red → yellow
        else
            return Color.Lerp(Color.yellow, Color.green, (pct - 0.5f) / 0.5f); // 50..100%: yellow → green
    }

    System.Collections.IEnumerator AnimateHp(Slider bar, Image fill, int newHp, int maxHp, float dur = 0.2f)
    {
        if (!bar) yield break;

        float start = bar.value;
        float target = newHp;
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(start, target, t / dur);
            bar.value = v;

            if (fill) fill.color = EvaluateHealthColor(v / maxHp);
            yield return null;
        }

        bar.value = target;
        if (fill) fill.color = EvaluateHealthColor((float)newHp / maxHp);
    }

    void Log(string s) { logText.text = s; }
}
