using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ThemeSelectorUI : MonoBehaviour
{
    public ThemeManager themeManager;
    public GameObject menuPanel;
    public List<GameTheme> availableThemes;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public GameController gameController;

    void Start()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        foreach (GameTheme theme in availableThemes)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.GetComponentInChildren<TMPro.TMP_Text>().text = theme.ThemeName;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectTheme(theme));
        }
    }

    void SelectTheme(GameTheme theme)
    {
        themeManager.ApplyTheme(theme);

        menuPanel.SetActive(false);

        gameController.StartGame();
    }
}