using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatPaw : MonoBehaviour
{
    public GameObject Paw;

    public GameObject Touching;

    public bool bited = false;
    private float speed = 0.0f;
    private Vector3 LastFrame;

    public float damage = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        LastFrame = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Paw.transform.position;
        this.transform.rotation = Paw.transform.rotation;
        speed = Vector3.Distance(this.transform.position,LastFrame);
        LastFrame = this.transform.position;
    }

    public float GetSpeed()
    {
        return speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car" && GameManager.gm.m_state != GameManager.GameState.LoseForTruck)
        {
            GameManager.gm.TruckToDie();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(bited)
        {
            bited = false;
            if (Touching.tag == "Material")
            {
                Touching.SendMessage("BecomeFood");
            }
        }
        Touching = null;
    }
    public void SearchForBite()
    {
        Collider[] pawRange = Physics.OverlapSphere(this.transform.position, 0.2f);
        float nearest_distance = 9999;
        //Debug.Log("==========================================================");
        foreach (var obj in pawRange)
        {
            if (obj.gameObject.tag == "Material" || LayerMask.LayerToName(obj.gameObject.layer) == "Food" || LayerMask.LayerToName(obj.gameObject.layer) == "Water")
            {
                    if (Vector3.Distance(obj.transform.position, this.transform.position) < nearest_distance)
                    {
                    //Debug.Log("Setting the nearset prey to " + animal.name);
                    Touching = obj.gameObject;
                        nearest_distance = Vector3.Distance(obj.transform.position, this.transform.position);
                    }                
            }
        }
        return;
    }
}
