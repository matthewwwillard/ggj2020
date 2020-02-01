using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_PlayerCharacter : MonoBehaviour
{
    public Vector3 aimVector;

    public const float SPEED = 10.0f;

    public GameObject[] aimLaser;

    public LayerMask laserMask;

    Rigidbody myRigidbody;

    public CC_Robot currentTarget;
    public Vector3 targetDirection;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveLeft()
    {
        myRigidbody.velocity = Vector3.left * SPEED;
    }

    public void MoveRight()
    {
        myRigidbody.velocity = Vector3.right * SPEED;
    }

    public void StopMoving()
    {
        myRigidbody.velocity = Vector3.zero;
    }

    public void FireLaser()
    {
        if(currentTarget != null)
        {
            Debug.Log("Shoot a guy!");
            currentTarget.Shot(targetDirection);
        }
    }

    public void SetAimVector(Vector3 aim)
    {
        aimVector = aim;

        transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f, laserMask))
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
                }
            }
            else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Robot"))
            {
                currentTarget = hit.collider.GetComponent<CC_Robot>();

                targetDirection = aimVector;

                aimLaser[1].SetActive(false);
            }
        }
    }
}
