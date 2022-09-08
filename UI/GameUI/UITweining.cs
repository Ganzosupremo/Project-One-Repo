using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITweining : MonoBehaviour
{
    public GameObject objectToAnimate;
    public float duration = 4f;
    public float changeInSize = 1f;

    public LeanTweenType tweenType;

    private void Start()
    {
        Scale();
    }

    private void Scale()
    {
        LeanTween.size(objectToAnimate.GetComponent<RectTransform>(), objectToAnimate.GetComponent<RectTransform>().sizeDelta * changeInSize, duration)
            .setEase(tweenType).setLoopPingPong();
    }
}
