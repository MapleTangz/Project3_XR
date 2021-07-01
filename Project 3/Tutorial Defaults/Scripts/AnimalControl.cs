using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Animation Curve
// Ref:https://www.youtube.com/watch?v=W2UPwVl_1kY&ab_channel=tdHendrix

public class AnimalControl : MonoBehaviour
{
    // Walking through shaking
    public Transform LeftController;
    public Transform RightController;

    public float threshold = 0.3f;

    private Vector3 HigherLastFrame;
    private Vector3 LowerLastFrame;

    void Start()
    {
    }
    void Update()
    {
        Vector3 left_pos = LeftController.position;
        Vector3 right_pos = RightController.position;
        float step = 0f;
        if(Mathf.Abs(left_pos.y - right_pos.y) > threshold)
        {
            Vector3 HigherThisFrame = right_pos, LowerThisFrame = left_pos;
            if (left_pos.y > right_pos.y)
            {
                HigherThisFrame = left_pos;
                LowerThisFrame = right_pos;
            }
            if(HigherLastFrame != Vector3.zero)
            {
                LowerLastFrame.y = 0.0f;
                LowerThisFrame.y = 0.0f;
                step = Vector3.Distance(LowerLastFrame, LowerThisFrame);
            }

            // End of Moving
            HigherLastFrame = HigherThisFrame;
            LowerLastFrame = LowerThisFrame;
        }
        else
        {
            HigherLastFrame = Vector3.zero;
            LowerLastFrame = Vector3.zero;
        }
        gameObject.transform.Translate(transform.forward * step);
    }
}