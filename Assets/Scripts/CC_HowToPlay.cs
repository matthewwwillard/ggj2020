using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CC_HowToPlay : MonoBehaviour
{
    public GameObject[] screens;
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
