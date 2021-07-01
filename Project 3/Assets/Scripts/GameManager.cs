using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public Animator fadeAnim;
    public float transitionTime = 1.5f;
    public enum GameState
    {
        ToSaveSmallLeopard,
        FindFood,
        FindWater,
        GetPoison,
        Hunt,
        Main,
        LoseForHungry,
        LoseForTruck,
        LoseForPickedUp,
        Win
    }
    public GameState m_state = GameState.ToSaveSmallLeopard;

    public GameObject smallleopard;
    public GameObject player;
    public GameObject smallleopardprefab;
    private Leopard playerScript;
    private Movement kidScript;
    public float[] playerData = {50,50,0};
    public float[] kidData = {50,50,0};

    private float StartTime;

    public void Awake()
    {
        if(gm == null)
            gm = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (SceneManager.GetActiveScene().name == "Lose")
        {
            Invoke("Lose", 1);
        }
        if (SceneManager.GetActiveScene().name == "Win")
        {
            Invoke("Win", 1);
        }
        
        
    }
    void Start()
    {
        Debug.Log("Start"); 
        Debug.Log(SceneManager.GetActiveScene().name);        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameManager.gm.m_state = GameState.ToSaveSmallLeopard;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameManager.gm.m_state = GameState.FindFood;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameManager.gm.m_state = GameState.FindWater;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GameManager.gm.m_state = GameState.Hunt;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GameManager.gm.m_state = GameState.GetPoison;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            GameManager.gm.m_state = GameState.Main;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GameManager.gm.m_state = GameState.LoseForHungry;
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            GameManager.gm.m_state = GameState.LoseForPickedUp;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            GameManager.gm.m_state = GameState.LoseForTruck;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameManager.gm.m_state = GameState.Win;
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(SceneManager.GetActiveScene().name == "Final main")
                GameManager.gm.SwitchToUnderground();
            if(SceneManager.GetActiveScene().name == "Underground")
                GameManager.gm.SwitchToFinalMain();
        }
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            GameManager.gm.SwitchToFinalMain();
        }
        if (SceneManager.GetActiveScene().name == "Final main" || SceneManager.GetActiveScene().name == "Underground")
        {
            switch (m_state)
            {
                case GameState.ToSaveSmallLeopard:
                    if (GameObject.Find("TrashBagWithSmallLeopard") == null)
                    {
                        gm.m_state = GameState.FindFood;
                    }
                    break;
                case GameState.FindFood:
                    if (smallleopard == null)
                    {
                        smallleopard = GameObject.Find("Little Leopard");
                    }
                    if (player == null)
                    {
                        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
                    }
                    smallleopard.SendMessage("EnableHungry");
                    player.SendMessage("EnableHungry");
                    if (GameObject.Find("TheVeryFirstFood") == null)
                    {
                        gm.m_state = GameState.FindWater;
                    }
                    break;
                case GameState.FindWater:
                    if (smallleopard == null)
                    {
                        smallleopard = GameObject.Find("Little Leopard");
                    }
                    if (player == null)
                    {
                        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
                    }
                    smallleopard.SendMessage("EnableThursty");
                    player.SendMessage("EnableThursty");
                    if (GameObject.Find("TheVeryFirstWater") == null)
                    {
                        gm.m_state = GameState.Hunt;
                    }
                    break;
                case GameState.Hunt:
                    if (GameObject.Find("TheVeryFirstPrey") == null)
                    {
                        gm.m_state = GameState.GetPoison;
                    }
                    break;

                case GameState.GetPoison:
                    if (smallleopard == null)
                    {
                        smallleopard = GameObject.Find("Little Leopard");
                    }
                    if (player == null)
                    {
                        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
                    }
                    smallleopard.SendMessage("EnablePoison");
                    player.SendMessage("EnablePoison");
                    if (GameObject.Find("TheCleanWater") == null)
                    {
                        gm.m_state = GameState.Main;
                        StartTime = Time.time;
                    }
                    break;
                case GameState.Main:                         
                    Destroy(GameObject.Find("TheVeryFirstFood"));
                    //Destroy(GameObject.Find("TrashBagWithSmallLeopard"));
                    Destroy(GameObject.Find("TheVeryFirstWater"));
                    Destroy(GameObject.Find("TheVeryFirstPrey"));
                    Destroy(GameObject.Find("TheCleanWater"));             
                    if (player != null)
                    {
                        player.SendMessage("EnableHungry");
                        player.SendMessage("EnableThursty");
                        player.SendMessage("EnablePoison");
                    }
                    if (smallleopard != null)
                    {                        
                        smallleopard.SendMessage("EnablePoison");
                        smallleopard.SendMessage("EnableThursty");
                        smallleopard.SendMessage("EnableHungry");
                    }
                    if(Time.time - StartTime > 240)//20 * 60)
                    {
                        ImageFade.imgf.BlackCurtain();
                        gm.m_state = GameState.Win;
                    }
                    break;
                case GameState.Win:
                case GameState.LoseForHungry:
                case GameState.LoseForTruck:
                case GameState.LoseForPickedUp:
                    ImageFade.imgf.BlackCurtain();
                    break;
            }
            if(Input.GetKeyDown(KeyCode.R)){
            //StartTime = Time.time;
            GameManager.gm.m_state = GameState.ToSaveSmallLeopard;
            playerData[0] = 50;
            playerData[1] = 50;
            playerData[2] = 0;
            kidData[0] = 50;
            kidData[1] = 50;
            kidData[2] = 0;
            SceneManager.LoadScene("Final main");
        }
        }
        if (SceneManager.GetActiveScene().name == "WeiAnTesting")
        {
            //switch(m_state)
            //{
            //    case GameState.ToSaveSmallLeopard:
            //        if (GameObject.Find("TrashBagWithSamllLeopard") == null)
            //        {
            //            gm.m_state = GameState.FindFood;
            //        }
            //        break;
            //    case GameState.FindFood:
            //        if (smallleopard == null)
            //        {
            //            smallleopard = GameObject.Find("Little Leopard");
            //            smallleopard.SendMessage("EnableHungry");
            //        }
            //        if (player == null)
            //        {
            //            player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
            //            player.SendMessage("EnableHungry");
            //        }
            //        if (GameObject.Find("TheVeryFirstFood") == null) 
            //        {
            //            gm.m_state = GameState.FindWater;
            //        }
            //        break;
            //    case GameState.FindWater:
            //        smallleopard.SendMessage("EnableThursty");
            //        player.SendMessage("EnableThursty");
            //        if (GameObject.Find("TheVeryFirstWater") == null)
            //        {
            //            gm.m_state = GameState.Hunt;
            //        }
            //        break;
            //    case GameState.Hunt:
            //        if (GameObject.Find("TheVeryFirstPrey") == null)
            //        {
            //            gm.m_state = GameState.GetPoison;
            //        }
            //        break;

            //    case GameState.GetPoison:
            //        smallleopard.SendMessage("EnablePoison");
            //        player.SendMessage("EnablePoison");
            //        if (GameObject.Find("TheCleanWater") == null)
            //        {
            //            gm.m_state = GameState.Main;
            //        }
            //        break;
            //    case GameState.Main:
            //        break;
            //}
        }

    }
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Lose()
    {
        GameObject.Find("Lamb_Animations").SendMessage("die");
    }
    public void Win()
    {
        GameObject.Find("Little Leopard").SendMessage("win");
    }
    public void ToLose()
    {
        // SceneManager.LoadScene("Lose");
        StartCoroutine("LoadLevel", "Lose");
    }
    public void ToWin()
    {
        // SceneManager.LoadScene("Win");
        StartCoroutine("LoadLevel", "Win");
    }
    public void TruckToDie()
    {
        GameManager.gm.m_state = GameState.LoseForTruck;
    }
    public void HungryToDie()
    {
        GameManager.gm.m_state = GameState.LoseForHungry;
    }
    public void PickUpToDie()
    {
        GameManager.gm.m_state = GameState.LoseForPickedUp;
    }
    public void SwitchToUnderground()
    {
        if (GameManager.gm.m_state == GameState.Main)
        {
            StartCoroutine("LoadLevel", "Underground");
        }
    }
    public void SwitchToFinalMain()
    {

        if (GameManager.gm.m_state == GameState.Main)
        {
            StartCoroutine("LoadLevel", "Final main");
        }
    }

    IEnumerator LoadLevel(string sceneName)
    {
        // Play animation
        fadeAnim.SetTrigger("Fade_Out");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        SceneManager.LoadScene(sceneName);

        fadeAnim.SetTrigger("Fade_In");

        // Wait
        yield return new WaitForSeconds(transitionTime);
    }
}
