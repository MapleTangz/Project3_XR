using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class Leopard : MonoBehaviour
{
    // Walking through shaking
    private Transform LeftController;
    private Transform RightController;
    private VRTK_ControllerEvents LeftHandControllerEvent;
    private VRTK_ControllerEvents RightHandControllerEvent;
    private CatPaw LeftPaw;
    private CatPaw RightPaw;
    private Transform Head;
    public float step_size = 4;

    public float threshold = 0.2f;

    private Vector3 HigherLastFrame;
    private Vector3 LowerLastFrame;

    public float hungryConsumePerSecond = 0;
    public float thurstyConsumePerSecond = 0;
    public float hungryStartValue = 50;
    public float thurstyStartValue = 50;
    public float poisonStartValue = 80;
    public float hungryThreshold = 20;
    public float thurstyThreshold = 20;
    public float poisonThreshold = 80;
    public Slider hungry;
    public Slider thursty;
    public Slider poison;
    public Text hungryDigit;
    public Text thurstyDigit;
    public Text poisonDigit;
    private float MAX_STEPSIZE = 6.0f;
    private float MIN_STEPSIZE = 4.0f;

    private AudioSource[] AllSources;
    [SerializeField]
    private AudioClip[] catSound;
    private AudioSource mymeow;

    private bool run = false;
    private bool attack = false;
    private bool jump = false;
    private bool bite = false;
    private bool meow = false;

    private GameObject thePrey = null;
    private GameObject Prey = null;
    private float hunt_pov = 5; //degs
    private int fps;
    private float t;
    private Vector3 p0,p1,p2;
    private float duration = 0.5f;
    private float biteCoolDown = 0.0f;
    void Awake()
    {
        if(GameManager.gm != null)
        {
            Debug.Log("Get Data from last Scene");
            hungryStartValue = GameManager.gm.playerData[0];
            thurstyStartValue = GameManager.gm.playerData[1];
            poisonStartValue = GameManager.gm.playerData[2];    
        }
    }
    void Start()
    {
        AllSources = gameObject.GetComponents<AudioSource>();
        if(AllSources.Length>=2)
        {
            mymeow = AllSources[1];
        }
        LeftController = transform.GetChild(0);
        RightController = transform.GetChild(1);
        Head = transform.GetChild(2);
        LeftHandControllerEvent = LeftController.GetChild(1).GetComponent<VRTK_ControllerEvents>();
        RightHandControllerEvent = RightController.GetChild(1).GetComponent<VRTK_ControllerEvents>();
        LeftPaw = LeftHandControllerEvent.transform.GetChild(0).GetComponent<CatPaw>();
        RightPaw = RightHandControllerEvent.transform.GetChild(0).GetComponent<CatPaw>();
        LeftController.GetChild(0).gameObject.SetActive(false);
        RightController.GetChild(0).gameObject.SetActive(false);
        // Init UI
        hungry.value = hungryStartValue;
        thursty.value = thurstyStartValue;
        poison.value = poisonStartValue;
        hungryDigit.text = hungry.value.ToString("0");
        thurstyDigit.text = thursty.value.ToString("0");
        poisonDigit.text = poison.value.ToString("0");
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            poison.value = 80;
        }
        if(Input.GetKeyDown(KeyCode.O)){
            poison.value = poison.value - 20;
        }
        biteCoolDown += Time.deltaTime;
        //Debug.Log(LeftController.position.y+"/"+ RightController.position.y);
        hungry.value -= Time.deltaTime * hungryConsumePerSecond;
        thursty.value -= Time.deltaTime * thurstyConsumePerSecond;
        hungryDigit.text = hungry.value.ToString("0");
        thurstyDigit.text = thursty.value.ToString("0");
        if(GameManager.gm.m_state == GameManager.GameState.Main)
        {
            GameManager.gm.playerData[0] = hungry.value;
            GameManager.gm.playerData[1] = thursty.value;
            GameManager.gm.playerData[2] = poison.value;
            if(GameManager.gm.player == null)
                GameManager.gm.player = this.gameObject;            
        }        
        if(GameManager.gm.m_state == GameManager.GameState.Win)
        {
            Debug.Log(catSound.Length);
            // Play the Sound of Anesthesia
            mymeow.clip = catSound[2];
            mymeow.Play();
        }
        if(GameManager.gm.m_state == GameManager.GameState.LoseForTruck || 
            GameManager.gm.m_state == GameManager.GameState.LoseForPickedUp||
            GameManager.gm.m_state == GameManager.GameState.LoseForHungry ||
            GameManager.gm.m_state == GameManager.GameState.Win)
        {
            this.enabled = false;
        }
        if(poison.value >= poisonThreshold)
        {
            ImageFade.imgf.isPoison = true;
            MAX_STEPSIZE = 3.0f;
            MIN_STEPSIZE = 2.0f;
        }
        else
        {
            ImageFade.imgf.isPoison = false;
            MAX_STEPSIZE = 6.0f;
            MIN_STEPSIZE = 4.0f;
        }
        if(hungry.value == 0 && thursty.value == 0 && GameManager.gm.m_state == GameManager.GameState.Main)
        {
            GameManager.gm.HungryToDie();
        }
        else 
        { 
            Vector3 head_pos = Head.position;
            Vector3 step = calculate_step();
            // Search for Preys
            Prey = null;
            SearchForPreys();
            //Debug.Log(LeftController.position.y.ToString() + "/" + RightController.position.y.ToString());
            // Judgement of running
            if (RightController.position.y < 0.01f || LeftController.position.y < 0.01f && Mathf.Abs(LeftController.position.y - RightController.position.y) > threshold)
            {
                run = true;
            }else
            {
                run = false;
            }    
            // Judgement of attacking
            if (LeftController.position.y - Head.position.y > 0.1f && RightController.position.y - Head.position.y > 0.1f)
            {
                attack = true;
            }
            else
            {
                attack = false;
            }
            // Judgement of biting
            if (Mathf.Abs(LeftController.position.y - RightController.position.y) < 0.1f && // Vector3.Distance(LeftController.position, RightController.position) < 0.2f && 
                Vector3.Distance(LeftController.position, Head.position) < 1.2f && Vector3.Distance(RightController.position, Head.position) < 1.2f &&
                Head.position.y > LeftController.position.y && Head.position.y > RightController.position.y &&(LeftHandControllerEvent.triggerClicked && RightHandControllerEvent.triggerClicked)) 
            {
                //Debug.Log("Biting");
                bite = true;
            }
            else
            {
                bite = false;
            }
            // Judgement of meow
            if (LeftHandControllerEvent.touchpadPressed || RightHandControllerEvent.touchpadPressed)
            {
                meow = true;
            }
            else
            {
                meow = false;
            }

            // Run
            if (run && !jump)
            {
                gameObject.transform.Translate(step_size * step);
                step_size = step_size * 1.01f > MAX_STEPSIZE? MAX_STEPSIZE : step_size * 1.01f;
            }else
            {
                step_size = step_size / 1.01f < MIN_STEPSIZE? MIN_STEPSIZE : step_size / 1.01f;
            }

            // Attack
            if (attack && !jump)
            {
                if(Prey != null)
                {
                    //Debug.Log(thePrey.name);
                    //Debug.Log("Let's attack!!!");
                    jump = true;
                    p0 = gameObject.transform.position;
                    p2 = Prey.transform.position;
                    p2.y = p0.y;
                    p1 = 0.75f * p0 + 0.25f * p2;
                    p1.y = p1.y + 3.0f;
                    fps = (int)((1  /Time.deltaTime * duration) );
                    t = 0.0f;
                    thePrey = Prey;
                    //Debug.Log("Make Bez from p0 = " + p0.ToString() + " to " + p2.ToString() + " FPS = " + fps.ToString());
                }
            }

            // Jump
            if (jump)
            {
                t = t + 1.0f;
                float p = (1 - t / fps / duration);
                gameObject.transform.position = p*p* p0 + 2 * p * (1-p) * p1 + (1 - p) * (1 - p) * p2;
                if(t > duration*fps)
                {
                    jump = false;
                    gameObject.transform.position = p2;
                    thePrey.SendMessage("BecomeMaterial");
                    thePrey = null;
                }
            }
            // Bite
            if (bite)
            {
                LeftPaw.SearchForBite();
                RightPaw.SearchForBite();
                //Debug.Log(LeftPaw.Touching.name + " / " + RightPaw.Touching.name);
                if (LeftPaw.Touching != null && RightPaw.Touching != null)
                {
                    if (LeftPaw.Touching.name == RightPaw.Touching.name)
                    {
                        if (LayerMask.LayerToName(LeftPaw.Touching.layer) == "Food" && biteCoolDown > 5.0f)
                        {
                            biteCoolDown = 0;
                            //Debug.Log("Eating");
                            var script = LeftPaw.Touching.GetComponents<Nutrition>();
                            hungry.value += script[0].getFull();
                            hungryDigit.text = hungry.value.ToString("0");
                            poison.value += script[0].getPoison();
                            poisonDigit.text = poison.value.ToString("0");
                        }
                        else if (LayerMask.LayerToName(LeftPaw.Touching.layer) == "Water"&& biteCoolDown > 5.0f)
                        {
                            biteCoolDown = 0;
                            //Debug.Log("Drinking");
                            var script = LeftPaw.Touching.GetComponents<Nutrition>();
                            thursty.value += script[0].getFull();
                            thurstyDigit.text = hungry.value.ToString("0");
                            poison.value += script[0].getPoison();
                            poisonDigit.text = poison.value.ToString("0");
                        }
                        else if (LeftPaw.Touching.tag == "Material")
                        {
                            LeftPaw.bited = true;
                        }
                        if (!LeftPaw.Touching.GetComponent<Nutrition>().isWater()) 
                        {
                            LeftPaw.Touching.transform.position = (LeftPaw.transform.position + RightPaw.transform.position) / 2; 
                        }
                    }
                }
            }
            // Meow
            if (meow)
            {
                mymeow.clip = catSound[0];
                mymeow.Play();
                if(GameManager.gm.m_state == GameManager.GameState.ToSaveSmallLeopard)
                {
                    GameObject.Find("TrashBagWithSmallLeopard").GetComponent<TrashBagWithSmallLeopard>().meow();
                }
            }
        }        
    }
    /*
     * Function: Search for preys
     * Search for the nearest prey and set the gameobject the prey to the prey
     */
    private void SearchForPreys()
    {
        Vector3 HeadDirection = Head.forward;
        Vector3 HeadPosition2D = Head.position;
        HeadPosition2D.y = 0.0f;
        HeadDirection.y = 0.0f;
        HeadDirection.Normalize();

        Quaternion right = Quaternion.AngleAxis(hunt_pov, Vector3.up);
        Quaternion left = Quaternion.AngleAxis(hunt_pov, Vector3.down);

        Vector3 leftPoint = left * HeadDirection * 3 + HeadPosition2D;
        Vector3 rightPoint = right * HeadDirection * 3 + HeadPosition2D;

        Collider[] animals = Physics.OverlapSphere(HeadPosition2D, 3);
        float nearest_distance = 9999;
        //Debug.Log("==========================================================");
        foreach (var animal in animals)
        {
            Vector3 pos = animal.transform.position;
            pos.y = 0.0f;
            float dotP = Vector3.Angle(leftPoint - HeadPosition2D, pos - HeadPosition2D);
            if (animal.tag == "Prey")
            {
                animal.SendMessage("TurnOff");
                if (dotP <= hunt_pov * 2 && dotP >= 0)
                {
                    if (Vector3.Distance(animal.transform.position, Head.position) < nearest_distance)
                    {
                        //Debug.Log("Setting the nearset prey to " + animal.name);
                        Prey = animal.gameObject;
                        nearest_distance = Vector3.Distance(animal.transform.position, Head.position);
                    }
                }
            }
        }
        if (Prey)
        {
            Prey.SendMessage("TurnOn");
        }
        return;
    }
    public void EnableHungry()
    {
        hungry.gameObject.SetActive(true);
        hungryDigit.gameObject.SetActive(true);
        hungryConsumePerSecond = 0.33f;
    }
    public void EnableThursty()
    {
        thursty.gameObject.SetActive(true);
        thurstyDigit.gameObject.SetActive(true);
        thurstyConsumePerSecond = 0.33f;
    }
    public void EnablePoison()
    {
        poison.gameObject.SetActive(true);
        poisonDigit.gameObject.SetActive(true);
    }
    private Vector3 calculate_step()
    {
        Vector3 left_pos = LeftController.position;
        Vector3 right_pos = RightController.position;
        Vector3 res = Vector3.zero;
        if (Mathf.Abs(left_pos.y - right_pos.y) > threshold)
        {
            Vector3 HigherThisFrame = right_pos, LowerThisFrame = left_pos;
            if (left_pos.y > right_pos.y)
            {
                HigherThisFrame = left_pos;
                LowerThisFrame = right_pos;
            }
            if (HigherLastFrame != Vector3.zero && LowerLastFrame != Vector3.zero)
            {
                Vector3 LLFtemp = new Vector3(LowerLastFrame.x, 0, LowerLastFrame.z);   //LowerLastFrame.y = 0.0f;
                Vector3 LTFtemp = new Vector3(LowerThisFrame.x, 0, LowerThisFrame.z);   //LowerThisFrame.y = 0.0f;
                res = (LLFtemp - LTFtemp);
            }

            // End of Moving
            if (right_pos.y < 0.01f || left_pos.y < 0.01f)
            {
                HigherLastFrame = HigherThisFrame + step_size * res;
                LowerLastFrame = LowerThisFrame + step_size * res;
            }
            else
            {
                HigherLastFrame = HigherThisFrame;
                LowerLastFrame = LowerThisFrame;
            }
        }
        else
        {
            HigherLastFrame = Vector3.zero;
            LowerLastFrame = Vector3.zero;
            res = Vector3.zero;
        }
        return res;
    }
}