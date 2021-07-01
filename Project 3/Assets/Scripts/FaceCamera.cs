using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera MainCamera;
    void Start()
    {
        //MainCamera = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]/main_camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, MainCamera.transform.rotation.eulerAngles.y, 0);
    }
}
