using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    private Animator anim;
    public AudioSource myAudioSource;
    public AudioClip myClip;
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    void win()
    {
        myAudioSource.clip = myClip;
        myAudioSource.loop = false;
        myAudioSource.Play();
        anim.SetBool("is_win", true);
    }
}
