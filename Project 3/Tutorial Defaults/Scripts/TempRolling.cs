using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempRolling : MonoBehaviour
{
    private Vector3 circleCenter;
    public float angle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        circleCenter = gameObject.transform.position + gameObject.transform.forward/4;
    }

    // Update is called once per frame
    void Update()
    {
        angle += 2*Time.deltaTime;
        angle %= (2 * Mathf.PI);

        gameObject.transform.position = circleCenter + new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle) * 0.5f);
    }
}
