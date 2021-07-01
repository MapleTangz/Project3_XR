using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPickUp : MonoBehaviour
{
    public float detectRange = 15;
    public float walkSpeed = 3;
    public float leopardMinRange = 3;
    private float humanHP = 20;
    public GameObject skin;
    public float upOffset = 1.0f;
    public float forwardOffset = 0.2f;

    private GameObject player;
    private GameObject leopardTemp;
    private new Rigidbody rigidbody;
    private Vector3 pos;
    private int layerIndexLeopard;
    private int layerMaskLeopard;
    private Animator anim;
    private bool isPickedUp = false;
    public bool isDead = false;
    private SkinnedMeshRenderer humanRenderer;
    private Movement movementScript;
    private BGMManager mBGMManager;
    

    // Start is called before the first frame update
    void Start()
    {
        mBGMManager = GameObject.Find("Management/BGMPlayer").GetComponent<BGMManager>();     
        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
        humanRenderer = skin.GetComponent<SkinnedMeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        layerIndexLeopard = LayerMask.NameToLayer("Leopard");
        layerMaskLeopard = 1 << layerIndexLeopard;

        anim = GetComponent<Animator>();
        // State: No weapon
        anim.SetInteger("WeaponType_int", 0);
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        Collider[] leopards = Physics.OverlapSphere(pos, detectRange, layerMaskLeopard);

        if (Input.GetKeyDown(KeyCode.A))
        {
            humanHP -= 10;
            StartCoroutine("Hit", leopardTemp);
        }

        if (!isDead)
        {
            // Not pick up the leopard yet
            if (!isPickedUp)
            {
                // Leopard detected
                if (leopards.Length > 0)
                {                   
                    mBGMManager.PlayBGM(4, true);
                    GameObject leopard = leopards[0].gameObject;
                    movementScript = leopard.GetComponent<Movement>();
                    leopardTemp = leopard;
                    transform.LookAt(leopard.transform);
                    float distance = Vector3.Distance(transform.position, leopard.transform.position);

                    // Walk to the leopard
                    if (distance > leopardMinRange)
                    {
                        // State: Idle->Walk_Static
                        anim.SetFloat("Speed_f", 0.5f);
                        transform.position += transform.forward * Time.deltaTime * walkSpeed;
                    }

                    // Pick up the leopard
                    else
                    {
                        StartCoroutine("PickUp", leopard);
                    }
                }
            }

            // Pick up the leopard
            else
            {
                movementScript.transform.rotation = transform.rotation;
                movementScript.transform.position = transform.position + Vector3.up * upOffset + transform.forward * forwardOffset;
            }
        }

        // BGM end
        else{            
                mBGMManager.PlayBGM(4, false);
                mBGMManager.PlayBGM(0, true);
                Invoke("Die",2);
        }

    }

    IEnumerator PickUp(GameObject leopard)
    {
        anim.SetFloat("Speed_f", 0.0f);
        anim.SetBool("Crouch_b", true);
        anim.SetBool("Crossed_Arms", true);
        yield return new WaitForSeconds(1.0f);
        anim.SetBool("Crouch_b", false);
        isPickedUp = true;
        Animator leopardAnim = leopard.GetComponent<Animator>();
        Movement movementScript = leopard.GetComponent<Movement>();
        movementScript.isPickedUp = true;


        // State: Idle->Fear
        leopardAnim.SetBool("is_picked_up", true);
    }
    IEnumerator PutDown(GameObject leopard)
    {
        anim.SetBool("Crouch_b", true);
        anim.SetBool("Crossed_Arms", false);

        yield return new WaitForSeconds(1.0f);

        anim.SetBool("Crouch_b", false);
        isPickedUp = false;
        Animator leopardAnim = leopard.GetComponent<Animator>();
        Movement movementScript = leopard.GetComponent<Movement>();
        movementScript.isPickedUp = false;

        // State: Fear->Walk
        leopardAnim.SetBool("is_picked_up", false);
    }

    IEnumerator Hit(GameObject leopard)
    {
        if (humanHP <= 0)
        {
            isDead = true;
            anim.SetFloat("Speed_f", 0.0f);
            if (isPickedUp)
            {
                StartCoroutine("PutDown", leopard);
            }
        }
        transform.LookAt(player.transform);
        transform.transform.rotation = Quaternion.identity;
        Rigidbody leopardRigidbody = leopard.GetComponent<Rigidbody>();
        for (int i = 0; i < humanRenderer.materials.Length; i++)
        {
            humanRenderer.materials[i].SetColor("_Color", Color.red);
        }
        Vector3 forceDirection = transform.forward;
        forceDirection.y = 0.0f;
        //rigidbody.AddForce(transform.forward * -10, ForceMode.Impulse);
        rigidbody.AddForce(forceDirection * -10, ForceMode.Impulse);
        if (isPickedUp)
        {
            //leopardRigidbody.AddForce(transform.forward * -10, ForceMode.Impulse);
            leopardRigidbody.AddForce(forceDirection * -10, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(0.2f);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.Sleep();
        leopardRigidbody.velocity = Vector3.zero;
        leopardRigidbody.angularVelocity = Vector3.zero;
        leopardRigidbody.Sleep();
        for (int i = 0; i < humanRenderer.materials.Length; i++)
        {
            humanRenderer.materials[i].SetColor("_Color", Color.white);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "CatPaw")
        {
            CatPaw catPaw = collider.gameObject.GetComponent<CatPaw>();
            if (catPaw.GetSpeed() > 0.02f)
            {
                humanHP -= catPaw.damage;
                StartCoroutine("Hit", leopardTemp);
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
