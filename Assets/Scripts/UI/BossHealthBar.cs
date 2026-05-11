using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiContainer; // NEW: The parent object that holds the visible bar
    public Image healthFill;
    public TextMeshProUGUI healthText;
    public RectTransform barContainer;

    [Header("Internal Data")]
    private BossController targetBoss;
    private float maxHealth;
    private int lastKnownHP = -1;
    private int lastSegmentCount = 10;

    void Start()
    {
        // 1. Hide the UI immediately when the game starts
        if (uiContainer != null) uiContainer.SetActive(false);

        // 2. Begin searching for a boss
        StartCoroutine(SearchForBossRoutine());
    }

    IEnumerator SearchForBossRoutine()
    {
        // Check the scene every 0.5 seconds to see if a BossController exists
        while (targetBoss == null)
        {
            targetBoss = FindObjectOfType<BossController>();
            yield return new WaitForSeconds(0.5f);
        }

        // --- BOSS FOUND! ---
        maxHealth = targetBoss.MaxHp;
        lastKnownHP = targetBoss.CurrentHP;
        lastSegmentCount = 10;

        UpdateUI(targetBoss.CurrentHP);

        // Show the UI
        if (uiContainer != null) uiContainer.SetActive(true);
    }

    void Update()
    {
        // If we have a boss, monitor its health every frame
        if (targetBoss != null)
        {
            if (targetBoss.CurrentHP != lastKnownHP)
            {
                HandleHealthChange(targetBoss.CurrentHP);
                lastKnownHP = targetBoss.CurrentHP;
            }
        }
        // If the targetBoss is null but the UI is still showing (Boss was killed/destroyed)
        else if (uiContainer != null && uiContainer.activeSelf)
        {
            uiContainer.SetActive(false);           // Hide the UI
            StartCoroutine(SearchForBossRoutine()); // Start searching for the next boss
        }
    }

    private void HandleHealthChange(float currentHP)
    {
        // Calculate the 10-segment math
        float healthPercent = currentHP / maxHealth;
        int currentSegmentCount = Mathf.CeilToInt(healthPercent * 10);

        // Did we drop a segment?
        if (currentSegmentCount < lastSegmentCount)
        {
            StartCoroutine(ShakeBar());
            lastSegmentCount = currentSegmentCount;
        }

        UpdateUI(currentHP);
    }

    private void UpdateUI(float currentHP)
    {
        // Prevent negative UI display
        float displayHP = Mathf.Max(0, currentHP);
        healthFill.fillAmount = displayHP / maxHealth;
        healthText.text = $"{Mathf.RoundToInt(displayHP)} / {maxHealth}";
    }

    IEnumerator ShakeBar()
    {
        Vector3 originalPos = barContainer.localPosition;
        float elapsed = 0.0f;
        while (elapsed < 0.2f)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-5f, 5f);
            barContainer.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        barContainer.localPosition = originalPos;
    }
}