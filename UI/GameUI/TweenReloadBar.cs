using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenReloadBar : MonoBehaviour
{
    public Image barImage;
    public float tweenTime;
    public Color beginColor, endColor;

    private void Start()
    {
        LeanTween.init(800);
    }

    public void Tween()
    {
        LeanTween.value(gameObject, 0.1f, 1f, tweenTime)
            .setEasePunch().setOnUpdate((value) =>
            {
                barImage.fillAmount = value;
                barImage.color = Color.Lerp(beginColor, endColor, value);
            });
    }
}
