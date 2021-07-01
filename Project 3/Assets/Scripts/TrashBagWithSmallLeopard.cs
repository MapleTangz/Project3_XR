using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBagWithSmallLeopard : MonoBehaviour
{
    private  float myhp = 30;
    public GameObject smallleopard;
    private bool die = false;

    public AudioSource mySource;
    public AudioClip myClip;

    private void Start()
    {
        mySource = this.GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if(die || GameManager.gm.m_state != GameManager.GameState.ToSaveSmallLeopard)
        {
            this.gameObject.GetComponent<MeshCollider>().enabled = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            Vector3 temp = new Vector3(transform.position.x,0,transform.position.z);
            var leopard = Instantiate(smallleopard, temp, Quaternion.identity);
            leopard.name = "Little Leopard";
            leopard.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "CatPaw")
        {
            var CatPaw = col.gameObject.GetComponent<CatPaw>();
            if (CatPaw.GetSpeed() > 0.02f)
            {
                myhp = myhp - CatPaw.damage;
            }
            if (myhp <= 0)
            {
                die = true;
            }
        }
    }

    public void meow()
    {
        Invoke("playSound", 3);
    }
    private void playSound()
    {
        mySource.PlayOneShot(myClip);
    }
     
}

