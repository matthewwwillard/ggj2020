using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_RobotPair : MonoBehaviour
{
    public Transform connectionA;
    public Transform connectionB;

    public CC_Robot robotA;
    public CC_Robot robotB;

    const float MATCH_DELAY = 2.0f;

    float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            CC_GameplayManager.instance.ClearPair(this);
        }
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
    }
}
