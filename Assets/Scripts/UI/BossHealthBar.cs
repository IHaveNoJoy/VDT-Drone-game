using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiContainer;
    public Image healthFill;
    public TextMeshProUGUI healthText;
    public RectTransform barContainer;

    [Header("Settings")]
    public float lerpSpeed = 5f;

    private BossController targetBoss;
    private float maxHealth;
    private float targetFillAmount; // Used for smooth animation
    private int lastKnownHP = -1;
    private int lastSegmentCount = 10;

    void Start()
    {
        if (uiContainer != null) uiContainer.SetActive(false);
        StartCoroutine(SearchForBossRoutine());
    }

    IEnumerator SearchForBossRoutine()
    {
        while (targetBoss == null)
        {
            targetBoss = FindObjectOfType<BossController>();
            yield return new WaitForSeconds(0.5f);
        }

        maxHealth = targetBoss.MaxHp;
        lastKnownHP = targetBoss.CurrentHP;
        lastSegmentCount = 10;

        // Initialize fill amount
        targetFillAmount = targetBoss.CurrentHP / maxHealth;
        healthFill.fillAmount = targetFillAmount;

        UpdateUI(targetBoss.CurrentHP);
        if (uiContainer != null) uiContainer.SetActive(true);
    }

    void Update()
    {
        if (targetBoss != null)
        {
            // Smoothly animate the fill
            if (healthFill.fillAmount != targetFillAmount)
            {
                healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillAmount, Time.deltaTime * lerpSpeed);
            }

            if (targetBoss.CurrentHP != lastKnownHP)
            {
                HandleHealthChange(targetBoss.CurrentHP);
                lastKnownHP = targetBoss.CurrentHP;
            }
        }
        else if (uiContainer != null && uiContainer.activeSelf)
        {
            uiContainer.SetActive(false);
            StartCoroutine(SearchForBossRoutine());
        }
    }

    private void HandleHealthChange(float currentHP)
    {
        float healthPercent = currentHP / maxHealth;
        int currentSegmentCount = Mathf.CeilToInt(healthPercent * 10);

        if (currentSegmentCount < lastSegmentCount)
        {
            StartCoroutine(ShakeBar());
            lastSegmentCount = currentSegmentCount;
        }

        // Update the target fill amount for the Lerp in Update()
        targetFillAmount = Mathf.Max(0, healthPercent);

        // Update text immediately
        healthText.text = $"{Mathf.RoundToInt(Mathf.Max(0, currentHP))} / {maxHealth}";
    }

    private void UpdateUI(float currentHP)
    {
        // This is now primarily handled by the Lerp in Update()
        targetFillAmount = Mathf.Max(0, currentHP / maxHealth);
        healthText.text = $"{Mathf.RoundToInt(Mathf.Max(0, currentHP))} / {maxHealth}";
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