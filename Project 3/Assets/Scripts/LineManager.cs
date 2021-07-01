using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LineManager : MonoBehaviour
{
    public AudioSource myAudioSource;
    public AudioClip myClip;
    private GameObject LeopardContext;
    private GameObject HintUI;
    private GameObject FireFighterUI;
    private Text LeopardContextText;
    private Text HintUIText;
    private Text FireFighterText;

    private GameManager.GameState lastState = GameManager.GameState.ToSaveSmallLeopard;
    private bool playing = false;
    private bool finishthisstage = false;

    private int LineCount = 0;
    private string[] ToSaveSmallLeopardLines = { "這裡是哪裡......","好陌生的地方....", "阿!得先找找我的孩子!"};
    private string[] ToSaveSmallLeopardHints = { "可以用 Touchpad 喵叫尋找附近的小石虎" };

    private string[] FindFoodLines = { "我的孩子看起來也餓了", "哪裡會有吃的呢?"};
    private string[] FindFoodHints = { "眼前以及小石虎頭上的橘色量條代表飽足度","觀察看看,\n有些垃圾袋打破後會出現食物", "小石虎只會吃媽媽拿過的東西","用兩隻手接近食物","同時按下Trigger即可拿取食物","石虎媽媽胃口比較大,\n會一次吃光食物","請記得讓小石虎先吃" };

    private string[] FindWaterLines = { "水源,這附近有水源嗎", "讓我找找" };
    private string[] FindWaterHints = { "藍色的量條代表水分","觀察看看,\n有些水是可以喝的", "小石虎一樣只會喝媽媽喝過的東西", "用兩隻手接近水源,\n同時按下Trigger即可引用水源","若飽足度與水分都歸零\n遊戲失敗"};

    private string[] HuntLines = { "那隻老鼠看起來很好吃", "去狩獵看看吧" };
    private string[] HuntHints = { "接近老鼠,\n當老鼠身旁出現邊框鎖定的時候", "可以高舉控制器進行狩獵"};

    private string[] GetPoisonLines = { "怎麼感覺怪怪的", "食物或水源好像不太乾淨" };
    private string[] GetPoisonHints = { "人類的居住地充滿毒素", "有些食物吃下去後\n會累積毒素", "公園附近的食物會比較乾淨","可以去那邊找看看"};

    private string[] MainLines = { };
    private string[] MainHints = {"加油!\n在這座城市努力活到早上!"};

    private string[] LoseForHungryLines = {"好餓...","好想喝水...","撐不住了..."};
    private string[] LoseForHungryHints = { "要在這座城市生存\n果然太難了呢" };

    private string[] LoseForPickedUpLines = {"我的孩子","被人類帶走了..." };
    private string[] LoseForPickedUpHints = { "失去活下去的支柱了..." };

    private string[] LoseForTruckLines = { "呃阿!!!"};
    private string[] LoseForTruckHints = { "被車撞死了...","石虎路殺是常見的危機之一..." };

    private string[] WinLines = {"?","沒力氣了..."};
    private string[] WinFireFighterLines = {"抓到石虎跟石虎媽媽了!","辛苦了!","可以回家了!" };

    // Start is called before the first frame update
    void Start()
    {
        // Find Objects
        LeopardContext = this.gameObject.transform.parent.Find("Talk").gameObject;
        HintUI = this.gameObject.transform.parent.Find("HintCanvas").gameObject;
        FireFighterUI = this.gameObject.transform.parent.Find("FireFighterTalk").gameObject;
        LeopardContextText = this.gameObject.transform.parent.Find("Talk/LeoPards-Cat/Talk/Talk_context").gameObject.GetComponent<Text>();
        HintUIText = this.gameObject.transform.parent.Find("HintCanvas/HintPanel/HintText").gameObject.GetComponent<Text>();
        FireFighterText = this.gameObject.transform.parent.Find("FireFighterTalk/LeoPards-Cat/Talk/Talk_context").gameObject.GetComponent<Text>();
        // Invisible
        LeopardContext.SetActive(false);
        HintUI.SetActive(false);
        FireFighterUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(lastState != GameManager.gm.m_state)
        {
            lastState = GameManager.gm.m_state;
            LineCount = 0;
        }
        if (!playing)
        {
            switch (GameManager.gm.m_state)
            {
                case GameManager.GameState.ToSaveSmallLeopard:
                    if (LineCount < ToSaveSmallLeopardLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(ToSaveSmallLeopardLines[LineCount]));
                        LineCount = LineCount + 1;
                    }else if(LineCount < ToSaveSmallLeopardLines.Length + ToSaveSmallLeopardHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(ToSaveSmallLeopardHints[LineCount - ToSaveSmallLeopardLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else
                    {
                        HintUI.SetActive(false);
                    }
                    break;
                case GameManager.GameState.FindFood:
                    if (LineCount < FindFoodLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(FindFoodLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < FindFoodLines.Length + FindFoodHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(FindFoodHints[LineCount - FindFoodLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else
                    {
                        HintUI.SetActive(false);
                    }
                    break;
                case GameManager.GameState.FindWater:
                    if (LineCount < FindWaterLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(FindWaterLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < FindWaterLines.Length + FindWaterHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(FindWaterHints[LineCount - FindWaterLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else
                    {
                        HintUI.SetActive(false);
                    }
                    break;
                case GameManager.GameState.GetPoison:
                    if (LineCount < GetPoisonLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(GetPoisonLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < GetPoisonLines.Length + GetPoisonHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(GetPoisonHints[LineCount - GetPoisonLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else
                    {
                        HintUI.SetActive(false);
                    }
                    break;
                case GameManager.GameState.Hunt:
                    if (LineCount < HuntLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(HuntLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < HuntLines.Length + HuntHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(HuntHints[LineCount - HuntLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else
                    {
                        HintUI.SetActive(false);
                    }
                    break;
                case GameManager.GameState.Main:
                    if (LineCount < MainLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(MainLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < MainLines.Length + MainHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(MainHints[LineCount - MainLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else
                    {
                        HintUI.SetActive(false);
                    }
                    break;
                case GameManager.GameState.LoseForHungry:
                    if (LineCount < LoseForHungryLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(LoseForHungryLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < LoseForHungryLines.Length + LoseForHungryHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(LoseForHungryHints[LineCount - LoseForHungryLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else if(HintUI.activeSelf)
                    {
                        HintUI.SetActive(false);
                        GameManager.gm.ToLose();
                    }
                    break;
                case GameManager.GameState.LoseForPickedUp:
                    if (LineCount < LoseForPickedUpLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(LoseForPickedUpLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < LoseForPickedUpLines.Length + LoseForPickedUpHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(LoseForPickedUpHints[LineCount - LoseForPickedUpLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else if(HintUI.activeSelf)
                    {
                        HintUI.SetActive(false);
                        GameManager.gm.ToLose();
                    }
                    break;
                case GameManager.GameState.LoseForTruck:
                    if (LineCount < LoseForTruckLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(LoseForTruckLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < LoseForTruckLines.Length + LoseForTruckHints.Length)
                    {
                        HintUI.SetActive(true);
                        LeopardContext.SetActive(false);
                        StartCoroutine(playHint(LoseForTruckHints[LineCount - LoseForTruckLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else if(HintUI.activeSelf)
                    {
                        HintUI.SetActive(false);
                        GameManager.gm.ToLose();
                    }
                    break;
                case GameManager.GameState.Win:
                    if (LineCount < WinLines.Length)
                    {
                        LeopardContext.SetActive(true);
                        StartCoroutine(playOS(WinLines[LineCount]));
                        LineCount = LineCount + 1;
                    }
                    else if (LineCount < WinLines.Length + WinFireFighterLines.Length)
                    {
                        LeopardContext.SetActive(false);
                        FireFighterUI.SetActive(true);
                        StartCoroutine(playFireFighter(WinFireFighterLines[LineCount - WinLines.Length]));
                        LineCount = LineCount + 1;
                    }
                    else if(FireFighterUI.activeSelf)
                    {
                        GameManager.gm.ToWin();
                        FireFighterUI.SetActive(false);
                    }
                    break;
            }
        }
    }
    
    IEnumerator playHint(string line)
    {
        playing = true;
        HintUIText.text = line;
        if(GameManager.gm.m_state == GameManager.GameState.LoseForHungry||
            GameManager.gm.m_state == GameManager.GameState.LoseForPickedUp||
            GameManager.gm.m_state == GameManager.GameState.LoseForTruck)
        {
            yield return new WaitForSeconds(2);
            playing = false;
        }else
        {
            myAudioSource.PlayOneShot(myClip);
            yield return new WaitForSeconds(2);
            playing = false;
        }
    }
    IEnumerator playOS(string line)
    {
        playing = true;
        LeopardContextText.text = line;
        yield return new WaitForSeconds(3);
        playing = false;
    }
    IEnumerator playFireFighter(string line)
    {
        playing = true;
        FireFighterText.text = line;
        yield return new WaitForSeconds(2);
        playing = false;
    }
}
