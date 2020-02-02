using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CC_HowToPlay : MonoBehaviour
{
    public GameObject[] screens;
    public GameObject textInstance;
    public Transform parentLeaderboard;
    public CC_LeaderboardManager leaderboard;
    
    int currentScreen = 0;

    public void Awake()
    {
        Debug.Log(screens.Length);
        currentScreen = 0;
        for(int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(false);
        }
        screens[currentScreen].SetActive(true);
        
        leaderboard.GetTopXScores(4, scores =>
        {
            if (scores == null)
            {
                Debug.Log("Error getting scores!");
                return;
            }
            
            foreach(Scores s in scores)
            {
                Debug.Log(s.playerName);
                GameObject g = Instantiate(textInstance, parentLeaderboard) as GameObject;
                g.GetComponent<Text>().text = s.playerName + " - " + s.score;
            }
        });
        
    }

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GoToNext();
        }
    }

    public void GoToNext()
    {
        if(currentScreen == screens.Length-1)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            screens[currentScreen].SetActive(false);
            currentScreen++;
            screens[currentScreen].SetActive(true);
        }
    }

    public void GoBack()
    {
        if (currentScreen != 0)
        {
            screens[currentScreen].SetActive(false);
            currentScreen--;
            screens[currentScreen].SetActive(true);
        }
    }
}
