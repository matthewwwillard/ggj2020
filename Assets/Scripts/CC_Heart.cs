using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CC_Heart : MonoBehaviour
{
    public Image fillSprite;

    public bool filled;

    public void SetFilled(bool s)
    {
        filled = s;
        fillSprite.gameObject.SetActive(s);
    }
}
