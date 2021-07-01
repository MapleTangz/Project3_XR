using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrition : MonoBehaviour
{
    enum Kind
    {
        Food,
        Water
    }

    enum Place
    {
        NoPoison,
        ALittlePoison,
        Poison
    }

    enum Size
    {
        Small,
        Medium,
        Large
    }
    private float FullfillmentPoint;
    private float PoisonPoint;
    private int EatCount = 2;

    [SerializeField]
    private Kind myKind;
    [SerializeField]
    private Place myPlace;
    [SerializeField]
    private Size mySize;

    public AudioSource myAudioSource;
    public AudioClip myClip;
    void Start()
    {
        switch(myPlace)
        {
            case Place.NoPoison:
                PoisonPoint = -10.0f;
                break;
            case Place.ALittlePoison:
                PoisonPoint = 5.0f;
                break;
            case Place.Poison:
                PoisonPoint = 10.0f;
                break;
        }
        switch (mySize)
        {
            case Size.Small:
                FullfillmentPoint = 3.0f;
                break;
            case Size.Medium:
                FullfillmentPoint = 10.0f;
                break;
            case Size.Large:
                FullfillmentPoint = 20.0f;
                break;
        }
    }

    public bool isWater()
    {
        return myKind == Kind.Water;
    }
    public float getFull()
    {
        EatCount = EatCount - 1;
        if (EatCount <= 0)
        {
            Invoke("die",1);
        }
        myAudioSource.PlayOneShot(myClip);
        return FullfillmentPoint;
    }
    public float getPoison()
    {
        return PoisonPoint;
    }

    public void BecomeFood()
    {
        switch(myKind)
        {
            case Kind.Food:
                this.gameObject.layer = LayerMask.NameToLayer("Food");
                break;
            case Kind.Water:
                this.gameObject.layer = LayerMask.NameToLayer("Water");
                break;
        }
    }

    //public void CheckFinished()
    //{
    //    if(EatCount < 0)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}
    
    private void die()
    {
        Destroy(this.gameObject);
    }
}
