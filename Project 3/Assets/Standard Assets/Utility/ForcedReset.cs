using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI; //在此处添加

[RequireComponent(typeof (Image))] //在此处修改
public class ForcedReset : MonoBehaviour
{
    private void Update()
    {
        // if we have forced a reset ...
        if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
        {
            //... reload the scene
            Application.LoadLevelAsync(Application.loadedLevelName);
        }
    }
}
