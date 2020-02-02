using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_SondManager : MonoBehaviour
{
    public AudioClip[] laserSounds;
    public AudioClip match;
    public AudioClip hit;
    public AudioClip aww;
    public AudioClip main;

    public AudioSource source;

    public static CC_SondManager instance;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void PlayLaserSound()
    {
        AudioClip laser = laserSounds[Random.Range(0, laserSounds.Length - 1)];
        source.PlayOneShot(laser);
    }

    public void PlayHit()
    {
        source.PlayOneShot(hit, .4f); 
    }

    public void PlayMatch()
    {
        source.PlayOneShot(match);
        
        Invoke("PlayAww", 1f);
    }

    public void PlayAww()
    {
        source.PlayOneShot(aww, .5f);
    }

}
