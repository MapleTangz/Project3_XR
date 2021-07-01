using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose : MonoBehaviour
{
    // Script used in Lose Scene
    private Animator anim;
    public AudioSource myAudioSource;
    public AudioClip myClip;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    void die()
    {
        myAudioSource.clip = myClip;
        myAudioSource.loop = true;
        myAudioSource.Play();
        Invoke("animationPlay",0f);
    }

    void animationPlay(){
        anim.SetBool("is_dead", true);
    }
}
