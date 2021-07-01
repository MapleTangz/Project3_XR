using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BGMManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] BGMs;
    private AudioSource BGMPlayer;

    private Vector3 player;

    // What is playing currently
    public int playTrack;
    public bool playing = false;

    public AudioSource myAudioSource;
    public AudioClip myBGM;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]/Camera (head)2").transform.position;
        BGMPlayer = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]").GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "Final main")
        {
            if(GameManager.gm.m_state != GameManager.GameState.Main)
            {
                PlayBGM(0, true);
            }else
            {
                PlayBGM(1,true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Final main")
        {
            if(GameManager.gm.m_state == GameManager.GameState.LoseForHungry||
                GameManager.gm.m_state == GameManager.GameState.LoseForPickedUp||
                GameManager.gm.m_state == GameManager.GameState.LoseForTruck){
                    if(playTrack != 3)// The index of dead conversation
                    StartCoroutine(SwitchBGM(3));
                }
        }
        if (SceneManager.GetActiveScene().name == "Lose")
        {

        }
        if (SceneManager.GetActiveScene().name == "Win")
        {

        }
        if (SceneManager.GetActiveScene().name == "Underground")
        {
            PlayBGM(0,true);
        }
    }
    IEnumerator SwitchBGM(int track){
        PlayBGM(playTrack,false);
        yield return new WaitForSeconds(3);        
        PlayBGM(track,true);
    }
    public void PlayBGM(int Track, bool toggle)
    {
        if(SceneManager.GetActiveScene().name == "Final main"){
            if(toggle != playing || playTrack != Track)
            {
                if (toggle)
                {
                    playTrack = Track;
                    BGMPlayer.clip = BGMs[Track];
                    BGMPlayer.Play();
                    BGMPlayer.loop = true;
                    StartCoroutine(FadeIn());
                }
                else
                {
                    StartCoroutine(FadeOut());
                }
                playing = toggle;
            }
        }else if (SceneManager.GetActiveScene().name == "Underground"){
            if(toggle != playing)
            {
                if (toggle)
                {
                    myAudioSource.clip = myBGM;
                    myAudioSource.Play();
                    myAudioSource.loop = true;
                    StartCoroutine(FadeIn());
                }
                else
                {
                    StartCoroutine(FadeOut());
                }
                playing = toggle;
            }
        }
    }

    IEnumerator FadeOut()
    {
        Debug.Log("FadeOut");
        for (float i = 3.0f; i >= 0; i -= Time.deltaTime)
        {
            BGMPlayer.volume = i / 3.0f;
            yield return null;
        }
        BGMPlayer.volume = 0;
    }
    IEnumerator FadeIn()
    {
        Debug.Log("FadeIn");

        for (float i = 0; i <= 3.0f; i += Time.deltaTime)
        {
            BGMPlayer.volume = i / 3.0f;
            yield return null;
        }
    }
}
