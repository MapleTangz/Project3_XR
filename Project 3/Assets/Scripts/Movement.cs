using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public float radius = 15;
    public float walkSpeed = 2;
    public float foodMinRange = 2;
    public float hungryConsumePerSecond = 2;
    public float thurstyConsumePerSecond = 2;
    public float mom_range = 10;
    public float hungryStartValue = 50;
    public float thurstyStartValue = 50;
    public float poisonStartValue = 0;
    public float hungryThreshold = 20;
    public float thurstyThreshold = 20;
    public float poisonThreshold = 80;
    public Slider hungry;
    public Slider thursty;
    public Slider poison;
    public Text hungryDigit;
    public Text thurstyDigit;
    public Text poisonDigit;
    public bool isPickedUp = false;
    public float PickedUpTime = 0.0f;

    private GameObject player;
    private Vector3 center;
    private int layerIndexFood;
    private int layerIndexWater;
    private int layerMaskFood;
    private int layerMaskWater;
    private int layerMask;
    private bool is_eat = false;
    private float eat_time = 3;
    private Animator anim;
    private bool isHungryRoar = false;
    public AudioSource myAudioSource;
    public AudioClip myClip;

    void Awake()
    {
        if(GameManager.gm != null)
        {
            Debug.Log("Get Data from last Scene");
            hungryStartValue = GameManager.gm.kidData[0];
            thurstyStartValue = GameManager.gm.kidData[1];
            poisonStartValue = GameManager.gm.kidData[2];    
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]/main_camera");
        hungry.value = hungryStartValue;
        thursty.value = thurstyStartValue;
        poison.value = poisonStartValue;
        hungryDigit.text = hungry.value.ToString("0");
        thurstyDigit.text = thursty.value.ToString("0");
        poisonDigit.text = poison.value.ToString("0");
        layerIndexFood = LayerMask.NameToLayer("Food");
        layerIndexWater = LayerMask.NameToLayer("Water");
        layerMaskFood = 1 << layerIndexFood;
        layerMaskWater = 1 << layerIndexWater;
        layerMask = layerMaskFood | layerMaskWater;
        // Debug.Log(layerMask);
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]/main_camera");
        }
        Debug.Log(player);
        if(GameManager.gm.m_state == GameManager.GameState.LoseForHungry||
            GameManager.gm.m_state == GameManager.GameState.LoseForPickedUp||
            GameManager.gm.m_state == GameManager.GameState.LoseForTruck)
        {
            Destroy(gameObject);
        }
        if(GameManager.gm.m_state == GameManager.GameState.Main){
            GameManager.gm.kidData[0] = hungry.value;
            GameManager.gm.kidData[1] = thursty.value;
            GameManager.gm.kidData[2] = poison.value;
            if(GameManager.gm.smallleopard == null)
                GameManager.gm.smallleopard = this.gameObject;
        }
        center = transform.position;
        Collider[] foods = Physics.OverlapSphere(center, radius, layerMask);

        // Alive
        if (hungry.value > 0 || thursty.value > 0)
        {
            hungry.value -= Time.deltaTime * hungryConsumePerSecond;
            thursty.value -= Time.deltaTime * thurstyConsumePerSecond;
            hungryDigit.text = hungry.value.ToString("0");
            thurstyDigit.text = thursty.value.ToString("0");

            // Poisoned
            if (poison.value >= poisonThreshold)
            {
                eat_time = 5;
                anim.speed = 0.5f;
            }
            else
            {
                eat_time = 3;
                anim.speed = 1.0f;
            }

            // State: Idle->Bounce
            if (hungry.value <= hungryThreshold || thursty.value <= thurstyThreshold)
            {
                anim.SetBool("is_hungry", true);
                if(!isHungryRoar)
                {
                    Invoke("HungryRoar",2.0f);
                    isHungryRoar = true;
                }
            }
            else
            {
                anim.SetBool("is_hungry", false);
            }

            // Not picked up by human
            if(!isPickedUp)
            {
                PickedUpTime = 0.0f;
                // Food Detected
                if (foods.Length > 0)
                {
                    // State: Idle->Walk
                    anim.SetFloat("speed", 0.5f);

                    GameObject food = foods[0].gameObject;
                    //Transform temp = food.transform;
                    //temp.position = new Vector3(temp.position.x, 0, temp.position.z);
                    transform.LookAt(food.transform);
                    float distance = Vector3.Distance(food.transform.position, transform.position);

                    // Go to food postition
                    if (distance > foodMinRange)
                    {
                        transform.position += transform.forward * Time.deltaTime * walkSpeed;
                    }

                    // Eat food
                    else
                    {
                        if (!is_eat)
                        {
                            StartCoroutine("eat", food);
                            is_eat = true;
                        }
                    }
                }

                // Idle / Follow mom
                else
                {
                    float mom_dist = Vector3.Distance(transform.position, player.transform.position);

                    // Follow mom
                    if (mom_dist > mom_range)
                    {
                        // State: Idle->Walk
                        anim.SetFloat("speed", 0.5f);
                        //Transform temp = player.transform;
                        //temp.position = new Vector3(temp.position.x, 0, temp.position.z);
                        transform.LookAt(player.transform);                      
                        transform.position += transform.forward * Time.deltaTime * walkSpeed;
                    }

                    // Idle
                    else
                    {           
                        anim.SetFloat("speed", 0.0f);
                    }
                }
            }

            // Picked up by human
            else
            {
                PickedUpTime += Time.deltaTime;
                CheckPickedUpDie();
            }
        }

        // Dead
        else
        {            
            anim.SetBool("is_dead", true);
            Invoke("GameOver", 1);
        }
    }

    private void CheckPickedUpDie()
    {
        if(Vector3.Distance(transform.position,player.transform.position) > 10.0f || PickedUpTime > 30.0f)
        {
            GameManager.gm.PickUpToDie();
        }
    }
    private void GameOver()
    {
        if (GameManager.gm.m_state == GameManager.GameState.Main)
        {
            GameManager.gm.HungryToDie();
        }
    }
    IEnumerator eat(GameObject food)
    {
        anim.SetBool("is_eating", true);
        yield return new WaitForSeconds(eat_time);
        is_eat = false;
        anim.SetBool("is_eating", false);
        anim.SetFloat("speed", 0.0f);
        if (LayerMask.LayerToName(food.layer) == "Food")
        {
            var script = food.GetComponents<Nutrition>();
            hungry.value += script[0].getFull();
            hungryDigit.text = hungry.value.ToString("0");
            poison.value += script[0].getPoison();
            poisonDigit.text = poison.value.ToString("0");
        }
        else if (LayerMask.LayerToName(food.layer) == "Water")
        {
            var script = food.GetComponents<Nutrition>();
            thursty.value += script[0].getFull();
            thurstyDigit.text = hungry.value.ToString("0");
            poison.value += script[0].getPoison();
            poisonDigit.text = poison.value.ToString("0");
        }
    }
    public void EnableHungry()
    {
        hungry.gameObject.SetActive(true);
        hungryConsumePerSecond = 0.33f;
    }
    public void EnableThursty()
    {
        thursty.gameObject.SetActive(true);
        thurstyConsumePerSecond = 0.33f;
    }
    public void EnablePoison()
    {
        poison.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car" && GameManager.gm.m_state != GameManager.GameState.LoseForTruck)
        {
            anim.SetBool("is_dead", true);
            GameManager.gm.TruckToDie();
        }
    }

    private void HungryRoar()
    {
        Debug.Log("Roar");
        myAudioSource.PlayOneShot(myClip);
        isHungryRoar = false;
    }
}
