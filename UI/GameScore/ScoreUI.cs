using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        //Suscribe to the points scored event
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
    }

    private void OnDisable()
    {
        //unsuscribe to the points scored event
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
    }

    /// <summary>
    /// Handle the score changed event
    /// </summary>
    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs scoreChangedArgs)
    {
        //Update the UI
        scoreText.text = "SCORE: " + scoreChangedArgs.score.ToString("###,###, ###0")
            + "\nMULTIPLIER: x" + scoreChangedArgs.multiplier;
    }
}
