using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_GameplayManager : MonoBehaviour
{
    public CC_PlayerCharacter player;
    public List<CC_Robot> robots;

    public CC_Robot[] robotPrefabs;

    public GameObject explosionPrefab;
    public GameObject spawnLine;
    public GameObject aimPoint;

    public CC_RobotPair pairPrefab;

    float currentSpawnDelay;

    int matchesToRampUp = 0;
    int level = 0;

    const float START_SPAWN_DELAY = 1.6f;

    const float SPAWN_SPEED_UP_PER_LEVEL = .1f;

    const int MAX_LEVEL = 5;
    const int MATCHES_PER_LEVEL = 5;

    const int NUM_HEARTS = 3;

    int health;

    public static CC_GameplayManager instance;

    public float spawnTimer;

    public int score = 0;

    State state;

    public enum State
    {
        PRE_GAME, PLAYING, END_GAME, POST_GAME
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();   
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.PLAYING:
                ReceiveInput();
                spawnTimer -= Time.deltaTime;

                if(spawnTimer <= 0.0f)
                {
                    SpawnRobot();
                }

                break;
        }       
    }

    void StartGame()
    {
        state = State.PLAYING;

        currentSpawnDelay = START_SPAWN_DELAY;
        spawnTimer = currentSpawnDelay;

        health = NUM_HEARTS;

        matchesToRampUp = MATCHES_PER_LEVEL;

        score = 0;
        CC_UI.SetScore(0);
        CC_UI.ResetHearts();

        CC_UI.ShowGameOver(false);
    }

    void EndGame()
    {
        state = State.END_GAME;

        CC_UI.ShowGameOver(true);
        
        player.StopMoving();
    }

    void SpawnRobot()
    {
        spawnTimer = currentSpawnDelay;

        Vector3 spawnPosition = spawnLine.transform.position + Random.Range(-8.0f, 8.0f) * Vector3.right;

        GameObject r = GameObject.Instantiate(robotPrefabs[Random.Range(0, robotPrefabs.Length)].gameObject, transform);

        CC_Robot rComp = r.GetComponent<CC_Robot>();

        robots.Add(rComp);

        rComp.Spawn(spawnPosition);
    }

    void ScorePoints(int s)
    {
        score += s;

        CC_UI.SetScore(score);
    }

    void ReceiveInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //Move left
            player.MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //Move right
            player.MoveRight();
        }
        else
        {
            //Stop moving
            player.StopMoving();
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Floor"))))
        {
            //Transform objectHit = hit.transform;

            //Debug.Log("Raycast hit at: " + hit.point);
            //aimPoint.transform.position = hit.point;

            Vector3 vecTo = hit.point - player.transform.position;

            vecTo.y = 0;
            vecTo.Normalize();

            player.SetAimVector(vecTo);
        }

        if(Input.GetMouseButtonDown(0))
        {
            player.FireInput();
        }
    }

    public void KillRobot(CC_Robot r, bool takeDamage = false)
    {
        robots.Remove(r);

        GameObject.Destroy(r.gameObject);

        if(takeDamage)
        {
            TakeDamage();

            GameObject explosion = GameObject.Instantiate(explosionPrefab);

            explosion.transform.position = r.transform.position;

            GameObject.Destroy(explosion, 2.0f);

        }
    }

    void TakeDamage()
    {
        health--;

        CC_UI.DamageHeart();

        if (health <= 0)
            EndGame();
    }

    public void ScoreMatch(CC_Robot a, CC_Robot b)
    {
        GameObject pair = GameObject.Instantiate(pairPrefab.gameObject);

        CC_RobotPair comp = pair.GetComponent<CC_RobotPair>();

        comp.Spawn(a, b);

        ScorePoints(100);

        if(level < MAX_LEVEL)
        {
            matchesToRampUp--;

            if (matchesToRampUp <= 0)
            {
                matchesToRampUp = MATCHES_PER_LEVEL;

                level++;

                currentSpawnDelay -= SPAWN_SPEED_UP_PER_LEVEL;
            }
        } 
    }

    public void ClearPair(CC_RobotPair p)
    {
        KillRobot(p.robotA);
        KillRobot(p.robotB);

        GameObject.Destroy(p.gameObject);
    }
}
