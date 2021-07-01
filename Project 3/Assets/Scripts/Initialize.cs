using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Initialize : MonoBehaviour
{
    private Camera main_camera;
    private Camera second_camera;

    // Start is called before the first frame update
    private void Start()
    {        
        // Process Culling Plane
        main_camera = this.gameObject.transform.GetChild(3).gameObject.GetComponent<Camera>();
        second_camera = this.gameObject.transform.GetChild(2).gameObject.GetComponent<Camera>();
        main_camera.name = "main_camera";
        main_camera.enabled = false;
        main_camera.farClipPlane = 50;
        main_camera.clearFlags = CameraClearFlags.Depth;
        second_camera.nearClipPlane = 1;
        second_camera.cullingMask = (1 << 9);
        main_camera.depth = 0;
        second_camera.depth = -1;
        second_camera.clearFlags = CameraClearFlags.Skybox;
        main_camera.cullingMask = ~ second_camera.cullingMask;
        main_camera.enabled = true;
        second_camera.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}
