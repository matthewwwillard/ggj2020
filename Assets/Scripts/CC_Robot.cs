using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_Robot : MonoBehaviour
{
    public int type;

    public State state = State.DEFAULT;

    public enum State
    {
        DEFAULT, KNOCK_BACK, MATCHED, DISABLED
    }

    const float BASE_SPEED = 2.5f;
    const float KNOCKBACK_SPEED = 10.0f;

    const float MATCH_TIME = 2.0f;
    const float KNOCKBACK_TIME = .75f;

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
                myRigidbody.AddForce(-5.0f * Vector3.forward, ForceMode.Acceleration);
                if (myRigidbody.velocity.magnitude > BASE_SPEED)
                    myRigidbody.velocity = myRigidbody.velocity.normalized * BASE_SPEED;
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

    public void Shot(Vector3 direction, Vector3 point)
    {
        state = State.KNOCK_BACK;

        timer = KNOCKBACK_TIME;

        knockbackDirection = direction;

        myRigidbody.AddForceAtPosition(25.0f * direction, point, ForceMode.Impulse);

        //myRigidbody.velocity = direction * KNOCKBACK_SPEED;
    }

    public void EnterDefaultState()
    {
        state = State.DEFAULT;

        //myRigidbody.velocity = Vector3.forward * -BASE_SPEED;
    }

    public void EnterMatchedState()
    {
        timer = MATCH_TIME;

        state = State.MATCHED;

        myRigidbody.velocity = Vector3.zero;
        myRigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    public void EnterDisableState()
    {
        state = State.DISABLED;

        gameObject.layer = LayerMask.NameToLayer("DisabledRobot");
        
        myRigidbody.velocity = Vector3.forward * -BASE_SPEED;
        myRigidbody.constraints = 0;
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
        else if (col.gameObject.layer == LayerMask.NameToLayer("DisableZone"))
        {
            EnterDisableState();
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
