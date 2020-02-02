using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CC_RobotPair : MonoBehaviour
{
    public Transform connectionA;
    public Transform connectionB;

    public CC_Robot robotA;
    public CC_Robot robotB;

    float currentSpinSpeed;

    const float MATCH_DELAY = 2.0f;
    const float SPIN_ACCELERATION = 540.0f;



    float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            CC_GameplayManager.instance.ClearPair(this);
        }

        currentSpinSpeed += SPIN_ACCELERATION * Time.deltaTime;

        transform.Rotate(0.0f, currentSpinSpeed * Time.deltaTime, 0.0f);
    }

    public void Spawn(CC_Robot a, CC_Robot b)
    {
        robotA = a;
        robotB = b;

        timer = MATCH_DELAY;

        transform.position = (a.transform.position + b.transform.position) / 2.0f;

        Vector3 vecTo = b.transform.position - a.transform.position;

        transform.rotation = Quaternion.LookRotation(vecTo, Vector3.up);

        a.transform.SetParent(connectionA);
        b.transform.SetParent(connectionB);

        a.transform.localPosition = Vector3.zero;
        b.transform.localPosition = Vector3.zero;

        a.transform.localRotation = Quaternion.identity;
        b.transform.localRotation = Quaternion.identity;

        connectionA.DOLocalMove(Vector3.zero, MATCH_DELAY / 2.0f).SetDelay(.5f);
        connectionB.DOLocalMove(new Vector3(0.0f, 3.05f, 0.0f), MATCH_DELAY / 2.0f).SetDelay(.5f);
        connectionB.DOLocalRotate(new Vector3(180.0f, 0.0f, 0.0f), MATCH_DELAY / 2.0f).SetDelay(.5f);
    }
}
