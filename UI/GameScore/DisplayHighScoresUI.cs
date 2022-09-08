using System;
using UnityEngine;

public class DisplayHighScoresUI : MonoBehaviour
{
    [SerializeField] private Transform contentAnchorTransform;

    private void Start()
    {
        DisplayScores();
    }

    /// <summary>
    /// Display the scores in the main menu scene
    /// </summary>
    private void DisplayScores()
    {
        HighScores highScores = HighScoreManager.Instance.GetHighScores();
        GameObject scoreGameObject;

        int rank = 0;
        foreach (Score score in highScores.scoreList)
        {
            rank++;

            scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);

            ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();

            //Populate the text fields in the score prefab
            scorePrefab.rankText.text = rank.ToString();
            scorePrefab.nameText.text = score.playerName;
            scorePrefab.levelText.text = score.levelName;
            scorePrefab.scoreText.text = score.playerScore.ToString("###,###,###0");
        }

        //Add a blank line
        scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
    }
}
