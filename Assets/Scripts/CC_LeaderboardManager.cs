using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using Newtonsoft.Json;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Scores
{
    public int score { get; set; }
    public string playerName { get; set; }
}

public class SaveScore
{
    public Scores score { get; set; }
}
public class LeaderboardReturn
{
    public Scores[] scores { get; set; }
}

public class CC_LeaderboardManager : MonoBehaviour
{

    public TextMeshProUGUI playerNameText;
    public GameObject inputHolder;
    public TextMeshProUGUI requestText;
    
    public delegate void PostCallback(bool success);

    public delegate void BoardCallback(Scores[] scores);

    public void ClickedSubmit()
    {
        this.inputHolder.SetActive(false);
        this.requestText.text = "Attempting to save your score...";
        this.requestText.gameObject.SetActive(true);

        if (CC_GameplayManager.instance.score <= 0)
        {
            this.requestText.text = "You need to try better!";
            return;
        }
        
        this.PostScore(CC_GameplayManager.instance.score, playerNameText.text, (bool success) =>
        {
            if (success)
            {
                this.requestText.text = "Thanks for playing!";
                this.requestText.color = Color.green;
            }
            else
            {
                this.requestText.text = "There was an error! Please try again!";
                Invoke("resetLeaderboardInput", 5);
            }
        });
    }

    private void resetLeaderboardInput()
    {
        this.requestText.gameObject.SetActive(false);
        this.inputHolder.SetActive(true);
    }
    
    public void PostScore(int score, string playername, PostCallback callback)
    {
        HTTPRequest request = new HTTPRequest(new Uri("https://api.ezleaderboards.com/api/v1/scores/cybercupid3030/main/score"),HTTPMethods.Post, ((originalRequest, response) =>
        {
            if (response.IsSuccess && response.StatusCode == 200)
            {
                
                callback(true);
            }
            else
            {
                Debug.Log(response.DataAsText);
                callback(false);
            }
        }));

        Scores s = new Scores() {playerName = playername, score = score};
        
        
        request.RawData =  Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SaveScore() {score = s}));
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.AddHeader("gameToken","5ce83910-e6e4-4e66-9f8b-6323482d969b");
        request.AddHeader("leaderboardToken","c3b35c98-d7d1-4701-98a6-0cecc011857c");
        request.Send();
    }

    public void GetTopXScores(int number, BoardCallback callback)
    {
        HTTPRequest request = new HTTPRequest(new Uri("https://api.ezleaderboards.com/api/v1/scores/cybercupid3030/main/top/"+number), ((originalRequest, response) =>
        {
            if (response.IsSuccess)
            {
                LeaderboardReturn l = JsonConvert.DeserializeObject<LeaderboardReturn>(response.DataAsText);
                Debug.Log(l.scores);
                callback(l.scores);
            }
            else
            {
                Debug.Log(response.DataAsText);
                callback(null);
            }
            
        }));
        request.AddHeader("gameToken","5ce83910-e6e4-4e66-9f8b-6323482d969b");
        request.AddHeader("leaderboardToken","c3b35c98-d7d1-4701-98a6-0cecc011857c");
        request.Send();
    }
    
}
