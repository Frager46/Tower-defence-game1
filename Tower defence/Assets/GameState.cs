using UnityEngine;
using TMPro;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [Header("Currency")]
    public int initialGold = 75;
    private int gold;
    private TextMeshProUGUI goldText;

    [Header("Upgrades")]
    private float damageMultiplier = 1f;

    private bool level1Completed = false;
    private bool level2Completed = false;
    private bool level3Completed = false;
    private bool level4Completed = false;
    private bool level5Completed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameState Awake, instance count: " + FindObjectsOfType<GameState>().Length);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gold = initialGold;
        UpdateGoldText();
    }

    public void SetGoldText(TextMeshProUGUI text)
    {
        goldText = text;
        UpdateGoldText();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
        Debug.Log($"GameState: Added {amount} gold, now: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldText();
            Debug.Log($"GameState: Spent {amount} gold, remaining: {gold}");
            return true;
        }
        Debug.Log($"GameState: Not enough gold! Required: {amount}, available: {gold}");
        return false;
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "" + gold;
            Debug.Log($"GameState: goldText updated to '{goldText.text}'");
        }
    }

    public int GetGold()
    {
        return gold;
    }

    public void ApplyDamageUpgrade(float increase)
    {
        damageMultiplier += increase;
        Debug.Log($"GameState: Damage upgrade applied, multiplier: {damageMultiplier}");
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void CompleteLevel(int level)
    {
        if (level == 1) level1Completed = true;
        if (level == 2) level2Completed = true;
        if (level == 3) level3Completed = true;
        if (level == 4) level4Completed = true;
        if (level == 5) level5Completed = true;
        Debug.Log($"GameState: Level {level} completed");
    }

    public void ResetLevel(int level)
    {
        if (level == 1) level1Completed = false;
        if (level == 2) level2Completed = false;
        if (level == 3) level3Completed = false;
        if (level == 4) level4Completed = false;
        if (level == 5) level5Completed = false;
        Debug.Log($"GameState: Level {level} reset");
    }

    public bool IsLevel1Completed()
    {
        return level1Completed;
    }

    public bool IsLevel2Completed()
    {
        return level2Completed;
    }

    public bool IsLevel3Completed()
    {
        return level3Completed;
    }

    public bool IsLevel4Completed()
    {
        return level4Completed;
    }

    public bool IsLevel5Completed()
    {
        return level5Completed;
    }

    public void ResetGold()
    {
        gold = initialGold;
        UpdateGoldText();
        Debug.Log($"GameState: Gold reset to {gold}");
    }
}