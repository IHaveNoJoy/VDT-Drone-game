using UnityEngine;

public class BossSpawnManager : MonoBehaviour
{
    public static BossSpawnManager Instance { get; private set; }

    [Header("UI")]
    public GameObject victoryScreen;

    void Awake()
    {
        Instance = this;
    }

    public void OnBossDefeated()
    {
        if (victoryScreen != null)
        {
            Debug.Log("There is a victory screen");
            victoryScreen.SetActive(true);
        }
        Time.timeScale = 0;
    }
}