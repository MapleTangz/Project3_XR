using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    float speed = 10;
    int state = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
        //Debug.Log("state: " + state);
        //Debug.Log(Vector3.Distance(transform.position, new Vector3(-22.25f, 0, 187.3f)));
        if (state == 1 && Vector3.Distance(transform.position, new Vector3(102.5f, 0, 187.3f)) < 1)
        {
            state = 2;
            transform.position = new Vector3(102.5f, 0, 187.3f);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0);
        }
        else if (state == 2 && Vector3.Distance(transform.position, new Vector3(-22.25f, 0, 187.3f)) < 1)
        {
            state = 3;
            transform.position = new Vector3(-22.25f, 0, 187.3f);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0);
        }
        else if (state == 3 && Vector3.Distance(transform.position, new Vector3(-22.25f, 0, 82.3f)) < 1)
        {
            state = 4;
            transform.position = new Vector3(-22.25f, 0, 82.3f);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0);
        }
        else if (state == 4 && Vector3.Distance(transform.position, new Vector3(102.5f, 0, 82.3f)) < 1)
        {
            state = 1;
            transform.position = new Vector3(102.5f, 0, 82.3f);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0);
        }
    }
}
