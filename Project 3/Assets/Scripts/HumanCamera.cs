using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamera : MonoBehaviour
{
    public float detectRange = 15;
    public float walkSpeed = 3;
    public float leopardMinRange = 5;
    public float shockTime = 5;

    private Vector3 pos;
    private int layerIndexLeopard;
    private int layerMaskLeopard;
    private Animator anim;
    private bool isShot = false;
    private Animator flashAnim;
    private GameObject white;

    // Start is called before the first frame update
    void Start()
    {
        white = GameObject.Find("Lights/FlashLight/White");
        layerIndexLeopard = LayerMask.NameToLayer("Leopard");
        layerMaskLeopard = 1 << layerIndexLeopard;

        anim = GetComponent<Animator>();
        flashAnim = white.GetComponent<Animator>();

        // State: No weapon
        anim.SetInteger("WeaponType_int", 1);

        anim.SetFloat("Speed_f", 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        Collider[] leopards = Physics.OverlapSphere(pos, detectRange, layerMaskLeopard);

        // Not pick up the leopard yet
        if (!isShot)
        {
            // Leopard detected
            if (leopards.Length > 0)
            {
                GameObject leopard = leopards[0].gameObject;
                transform.LookAt(leopard.transform);
                float distance = Vector3.Distance(transform.position, leopard.transform.position);

                // Walk to the leopard
                if (distance > leopardMinRange)
                {
                    // State: Idle->Walk_Static
                    anim.SetFloat("Speed_f", 0.5f);
                    transform.position += transform.forward * Time.deltaTime * walkSpeed;
                }

                // Shoot the leopard
                else
                {
                    StartCoroutine("Shoot", leopard);
                    Invoke("Die",10);
                }
            }
        }
    }

    IEnumerator Shoot(GameObject leopard)
    {
        anim.SetFloat("Speed_f", 0.0f);
        anim.SetBool("Crouch_b", true);
        // anim.SetBool("Crossed_Arms", true);
        isShot = true;
        Animator leopardAnim = leopard.GetComponent<Animator>();
        Movement movementScript = leopard.GetComponent<Movement>();
        movementScript.isPickedUp = true;

        yield return new WaitForSeconds(0.5f);

        flashAnim.Play("Shoot");
        anim.SetBool("Crouch_b", false);

        // State: Idle->Fear
        leopardAnim.SetBool("is_picked_up", true);

        yield return new WaitForSeconds(shockTime);

        movementScript.isPickedUp = false;
        leopardAnim.SetBool("is_picked_up", false);
        leopardAnim.SetFloat("speed", 0.0f);
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
