using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSmell : MonoBehaviour
{
    float distance;
    public float range = 15.0f;
    public float long_range = 60.0f;
    GameObject player;
    ParticleSystem smell;
    ParticleSystem particle;
    float angle;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
        smell = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        particle = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        smell.playbackSpeed = 3.0f;
        particle.playbackSpeed = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > range && distance < long_range)
        {
            dir = player.transform.position - transform.position;
            angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.up);
            smell.transform.rotation = Quaternion.Euler(0, angle, 0);
            smell.Play();
        }
        else
        {
            smell.Stop();
        }
    }
}
