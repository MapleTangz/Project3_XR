using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    enum State
    {
        alive,
        dead,
        waitforkill
    }
    public float mHealth;
    public GameObject content;
    private State state = State.alive;

    public AudioSource myAudioSource;
    public AudioClip myClip;

 
    // Update is called once per frame
    void Update()
    {
        if (this.tag == "TrashBag")
        {
            // Delay destroy for playing the particle
            if (state == State.dead)
            {
                this.gameObject.GetComponent<MeshCollider>().enabled = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.Translate(0, 1, 0);
                var steak = Instantiate(content, transform.position, transform.rotation);
                steak.SetActive(true);
                //steak.name = "TheVeryFirstFood";
                state = State.waitforkill;
                this.gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
                Invoke("kill", 1);
            }
        }
        if (this.tag == "Prey")
        {
            if (state == State.dead)
            {
                this.gameObject.GetComponent<MeshCollider>().enabled = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.Translate(0, 1, 0);
                var steak = Instantiate(content, transform.position, content.transform.rotation);
                steak.SetActive(true);
                state = State.waitforkill;
                Invoke("kill", 1);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (this.tag == "TrashBag")
        {
            if (col.gameObject.tag == "CatPaw")
            {
                // Play the sound of garbage bag
                myAudioSource.PlayOneShot(myClip);
                var CatPaw = col.gameObject.GetComponent<CatPaw>();
                if (CatPaw.GetSpeed() > 0.02f)
                {
                    mHealth = mHealth - CatPaw.damage;
                }
                if (mHealth <= 0)
                {
                    state = State.dead;
                }
            }
        }else if (this.tag == "Switching")
        {
            GameManager.gm.SwitchToUnderground();
        }
    }
    void BecomeMaterial()
    {
        state = State.dead;
    }

    void kill()
    {
        Destroy(this.gameObject);
    }
}
