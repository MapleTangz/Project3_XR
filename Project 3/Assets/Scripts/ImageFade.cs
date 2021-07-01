using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    public static ImageFade imgf;
    private Image img;
    private float duration = 3.0f;

    public bool isPoison = false;
    private bool lastPoison = false;

    public bool isBlackCurtain = false;
    private bool lastBlackCurtain = false;
    private void Awake()
    {
        imgf = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        img = this.GetComponent<Image>();
        StartCoroutine(FadeImage(true));
    }
    // Update is called once per frame
    public void BlackCurtain()
    {
        isBlackCurtain = true;
    }
    public void CleanCurtain()
    {
        isBlackCurtain = false;
    }
    private void Update()
    {
        if (isPoison && !lastPoison)
        {
            lastPoison = isPoison;
            StartCoroutine(PoisonFadeImage(false));
        }
        if (!isPoison && lastPoison)
        {
            lastPoison = isPoison;
            StartCoroutine(PoisonFadeImage(true));
        }
        if (isBlackCurtain && !lastBlackCurtain)
        {
            lastBlackCurtain = isBlackCurtain;
            StartCoroutine(FadeImage(false));
        }
        if (!isBlackCurtain && lastBlackCurtain)
        {
            lastBlackCurtain = isBlackCurtain;
            StartCoroutine(FadeImage(true));
        }
    }
    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = duration; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(0, 0, 0, i/ duration);
                yield return null;
            }
            img.color = new Color(0, 0, 0, 0);
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= duration; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(0, 0, 0, i/ duration);
                yield return null;
            }
            img.color = new Color(0, 0, 0, 1);
        }
    }
    IEnumerator PoisonFadeImage(bool fadeAway)
    {
        if (fadeAway)
        {
            for (float i = duration; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(0.73f, 0.1f, 0.99f, 1.0f / 5) * i / duration;
                Debug.Log(img.color);
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i <= duration; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(0.73f, 0.1f, 0.99f, 1.0f / 5) * i / duration;
                Debug.Log(img.color);
                yield return null;
            }
        }
    }
}
