using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_GameplayManager : MonoBehaviour
{
    public CC_PlayerCharacter player;
    public List<CC_Robot> robots;

    public CC_Robot[] robotPrefabs;

    public GameObject spawnLine;
    public GameObject aimPoint;

    float currentSpawnDelay;

    const float START_SPAWN_DELAY = 1f;

    const float SPAWN_SPEED_UP_PER_LEVEL = .1f;

    const int MAX_LEVEL = 5;
    const int MATCHES_PER_LEVEL = 5;

    public static CC_GameplayManager instance;

    public float spawnTimer;

    State state;

    public enum State
    {
        PRE_GAME, PLAYING, POST_GAME
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
    }

    void SpawnRobot()
    {
        spawnTimer = currentSpawnDelay;

        Vector3 spawnPosition = spawnLine.transform.position + Random.Range(-7.0f, 7.0f) * Vector3.right;

        GameObject r = GameObject.Instantiate(robotPrefabs[Random.Range(0, robotPrefabs.Length)].gameObject, transform);

        CC_Robot rComp = r.GetComponent<CC_Robot>();

        robots.Add(rComp);

        rComp.Spawn(spawnPosition);
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
            player.FireLaser();
        }
    }

    public void KillRobot(CC_Robot r)
    {
        robots.Remove(r);

        GameObject.Destroy(r.gameObject);
    }

    public void ScoreMatch(CC_Robot a, CC_Robot b)
    {

    }
}
