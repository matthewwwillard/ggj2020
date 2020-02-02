using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_PlayerCharacter : MonoBehaviour
{
    public Vector3 aimVector;

    public const float SPEED = 10.0f;
    public const float SHOT_DELAY = .05f;
    
    public GameObject[] aimLaser;
    public GameObject[] fireLasers;

    public GameObject hitEffect;

    public LayerMask laserMask;

    public AudioSource audio;

    Rigidbody myRigidbody;

    public CC_Robot currentTarget;
    public Vector3 targetPoint;
    public Vector3 targetDirection;

    private float shotTimer;
    private bool queueShot = false;

    public Animator anim;
    
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();

        audio = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableFireLasers(false);
    }

    // Update is called once per frame
    void Update()
    {
        shotTimer -= Time.deltaTime;

        if (shotTimer <= 0.0f)
        {
            EnableFireLasers(false);
        }
    }

    public void MoveLeft()
    {
//        myRigidbody.AddForce(Vector3.left * 25f, ForceMode.Acceleration);
//
//        if (myRigidbody.velocity.magnitude > SPEED)
//            myRigidbody.velocity = Vector3.left * SPEED;

        //myRigidbody.velocity = Vector3.left * SPEED;

        Vector3 projectedPos = myRigidbody.position += Time.deltaTime * Vector3.left * SPEED;

        if (projectedPos.x < -9.0f)
            projectedPos.x = -9.0f;

        transform.position = projectedPos;

        if(!audio.isPlaying)
            audio.Play();
    }

    public void MoveRight()
    {
        //myRigidbody.AddForce(Vector3.right * 25f, ForceMode.Acceleration);

        //if (myRigidbody.velocity.magnitude > SPEED)
        //myRigidbody.velocity = Vector3.right * SPEED;
        
        Vector3 projectedPos = myRigidbody.position += Time.deltaTime * Vector3.right * SPEED;

        if (projectedPos.x > 9.0f)
            projectedPos.x = 9.0f;

        transform.position = projectedPos;


        if (!audio.isPlaying)
            audio.Play();
    }

    public void StopMoving()
    {
        myRigidbody.velocity = Vector3.zero;

        audio.Pause();
    }

    public void FireInput()
    { 
//        if (shotTimer >= 0.0f)
//        {
//            queueShot = true;
//            return;
//        }
        
        FireLaser();
    }
    
    public void FireLaser()
    {
        CC_SondManager.instance.PlayLaserSound();
        if (currentTarget != null)
        {
            //Debug.Log("Shoot a guy!");
            currentTarget.Shot(targetDirection, targetPoint);

            GameObject hitInstance = GameObject.Instantiate(hitEffect);

            hitInstance.transform.position = targetPoint;

            GameObject.Destroy(hitInstance, 2.0f);
        }

        anim.SetTrigger("Fire");

        EnableFireLasers(true);

        shotTimer = SHOT_DELAY;
        
        shotTimer += SHOT_DELAY;
        queueShot = false;
    }

    public void EnableFireLasers(bool e)
    {
        for (int i = 0; i < fireLasers.Length; ++i)
        {
            fireLasers[i].SetActive(e);
        }
    }

    public void SetAimVector(Vector3 aim)
    {
        aimVector = aim;

        transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        currentTarget = null;

        RaycastHit hit;
        if (Physics.Raycast(aimLaser[0].transform.position, transform.forward, out hit, 100.0f, laserMask))
        {
            aimLaser[0].transform.localScale = new Vector3(aimLaser[0].transform.localScale.x, aimLaser[0].transform.localScale.y, hit.distance);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
            {

                Vector3 reflectVector = Vector3.Reflect(aimVector, hit.normal);
                //Debug.Log("Hit normal: " + hit.normal);

                aimLaser[1].transform.rotation = Quaternion.LookRotation(reflectVector, Vector3.up);

                aimLaser[1].SetActive(true);

                aimLaser[1].transform.position = hit.point;

                if (Physics.Raycast(aimLaser[1].transform.position + reflectVector * .1f, aimLaser[1].transform.forward, out hit, 100.0f, laserMask))
                {
                    aimLaser[1].transform.localScale = new Vector3(aimLaser[1].transform.localScale.x, aimLaser[1].transform.localScale.y, hit.distance);

                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Robot"))
                    {
                        currentTarget = hit.collider.GetComponent<CC_Robot>();

                        targetPoint = hit.point;
                        targetDirection = reflectVector;
                    }
                    else
                    {
                        currentTarget = null;
                    }
                }
                else
                {
                    aimLaser[1].SetActive(false);
                    currentTarget = null;
                }
            }
            else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Robot"))
            {
                currentTarget = hit.collider.GetComponent<CC_Robot>();

                targetPoint = hit.point;
                targetDirection = aimVector;

                aimLaser[1].SetActive(false);
            }
        }
        else
        {
            currentTarget = null;
        }

    }
}
