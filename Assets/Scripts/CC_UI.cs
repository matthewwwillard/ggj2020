using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CC_UI : MonoBehaviour
{
    public TextMeshProUGUI score;

    public static CC_UI instance;

    public CC_Heart[] hearts;

    public GameObject gameOverScreen;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameOverScreen.SetActive(false);
    }

    public static void ResetHearts()
    {
        for(int i = 0; i < instance.hearts.Length; ++i)
        {
            instance.hearts[i].SetFilled(true);
        }
    }

    public static void DamageHeart()
    {
        for (int i = 0; i < instance.hearts.Length; ++i)
        {
            if (instance.hearts[i].filled)
            {
                instance.hearts[i].SetFilled(false);
                return;
            }
        }
    }

    public static void SetScore(int s)
    {
        instance.score.text = s.ToString();
    }

    public static void ShowGameOver(bool b)
    {
        instance.gameOverScreen.SetActive(b);
    }

    public void PlayAgainPressed()
    {
        SceneManager.LoadScene(0);
    }

    public void ClosePressed()
    {
        Application.Quit();
    }
}
