using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBGM : MonoBehaviour
{
    public int PlayTrack;
    public bool OnOff;
    private BGMManager mBGMManager;
    private void Start()
    {
        mBGMManager = GameObject.Find("Management/BGMPlayer").GetComponent<BGMManager>();      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "CatPaw")
        {
            mBGMManager.PlayBGM(PlayTrack, OnOff);
        }
    }
}
