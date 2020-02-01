using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_Robot : MonoBehaviour
{
    public int type;

    public State state = State.DEFAULT;

    public enum State
    {
        DEFAULT, KNOCK_BACK, MATCHED
    }

    const float BASE_SPEED = 4.0f;
    const float KNOCKBACK_SPEED = 10.0f;

    const float MATCH_TIME = 2.0f;
    const float KNOCKBACK_TIME = 1.0f;

    float timer;

    Vector2 knockbackDirection;

    Rigidbody myRigidbody;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Spawn(Vector3 position)
    {
        transform.position = position;

        EnterDefaultState();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.DEFAULT:
                //transform.position = transform.position - Vector.forward * BASE_SPEED * Time.deltaTime;
                break;
            case State.KNOCK_BACK:
                timer -= Time.deltaTime;

                if(timer <= 0.0f)
                {
                    EnterDefaultState();
                }
                //transform.position = transform
                break;
            case State.MATCHED:
                timer -= Time.deltaTime;

                if(timer <= 0.0f)
                {
                    CC_GameplayManager.instance.KillRobot(this);
                }
                break;
        }
    }

    public void Shot(Vector3 direction)
    {
        state = State.KNOCK_BACK;

        timer = KNOCKBACK_TIME;

        knockbackDirection = direction;

        myRigidbody.velocity = direction * KNOCKBACK_SPEED;
    }

    public void EnterDefaultState()
    {
        myRigidbody.velocity = Vector3.forward * -BASE_SPEED;
    }

    public void EnterMatchedState()
    {
        timer = MATCH_TIME;

        state = State.MATCHED;

        myRigidbody.velocity = Vector3.zero;
        myRigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    void Match(CC_Robot otherBot)
    {
        EnterMatchedState();
        otherBot.EnterMatchedState();

        CC_GameplayManager.instance.ScoreMatch(this, otherBot);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Killzone"))
        {
            CC_GameplayManager.instance.KillRobot(this, true);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        //if(state == State.KNOCK_BACK)
        //{
        //    ContactPoint cp = col.GetContact(0);

        //    if (cp.otherCollider.gameObject.layer == LayerMask.NameToLayer("Default"))
        //    {
        //        Debug.Log("Bounce off wall: " + cp.normal + " New Velocity is: " + Vector3.Reflect(myRigidbody.velocity, cp.normal));
        //        myRigidbody.velocity = Vector3.Reflect(myRigidbody.velocity, cp.normal);
        //    }
        //}

        ContactPoint cp = col.GetContact(0);

        CC_Robot otherBot = cp.otherCollider.gameObject.GetComponent<CC_Robot>();

        if(otherBot!= null && otherBot.type == type)
        {
            if(otherBot.state != State.MATCHED)
                Match(otherBot);
        }
        
    }
}
