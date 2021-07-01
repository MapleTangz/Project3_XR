using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOutline : MonoBehaviour
{
    Outline outline;
    float distance;
    public float range = 15.0f;
    GameObject player;
    float angle;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > range) {
            outline.enabled = false;
        }
        else
        {
            outline.enabled = true;
        }
    }

    public void TurnOn()
    {
        outline.enabled = true;
    }

    public void TurnOff()
    {
        outline.enabled = false;
    }
}
