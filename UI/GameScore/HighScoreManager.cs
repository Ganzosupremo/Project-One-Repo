using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighScoreManager : SingletonMonoBehaviour<HighScoreManager>, IPersistenceData
{
    public bool useEncryption;

    private string scoreFileName = "HighScores";
    private string dataDirPath; //Deprecated
    private HighScores highScores = new();

    protected override void Awake()
    {
        base.Awake();

        string persistentDataPath = Application.persistentDataPath;
        dataDirPath = persistentDataPath;

        //LoadScores();
        PersistenceDataManager.Instance.LoadGame();
    }

    private void OnDestroy()
    {
        PersistenceDataManager.Instance.SaveGame();
    }

    /// <summary>
    /// Load the scores from disk. Deprecated
    /// </summary>
    private void LoadScores()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string fullPath = Path.Combine(dataDirPath, scoreFileName);

        if (File.Exists(fullPath))
        {
            ClearScoreList();

            FileStream file = File.OpenRead(fullPath);

            highScores = (HighScores)binaryFormatter.Deserialize(file);

            file.Close();
        }
    }

    private void ClearScoreList()
    {
        highScores.scoreList.Clear();
    }

    /// <summary>
    /// Adds the current score to the high score list
    /// </summary>
    public void AddScore(Score score, int rank)
    {
        highScores.scoreList.Insert(rank - 1, score);

        //Maintain the max number of scores that can be saved
        if (highScores.scoreList.Count > Settings.maxNumberOfHighScoresToSave)
        {
            highScores.scoreList.RemoveAt(Settings.maxNumberOfHighScoresToSave);
        }

        //SaveScores();
        //SaveData(gameData);
        PersistenceDataManager.Instance.SaveGame();
    }

    /// <summary>
    /// Saves the scores to the disk. Deprecated
    /// </summary>
    private void SaveScores()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string fullPath = Path.Combine(dataDirPath, scoreFileName);

        FileStream fileStream = File.Create(fullPath);

        binaryFormatter.Serialize(fileStream, highScores);

        fileStream.Close();
    }

    /// <summary>
    /// Gets the list of high scores saved to the disk
    /// </summary>
    /// <returns>Returns the list of high Scores</returns>
    public HighScores GetHighScores()
    {
        return highScores;
    }

    /// <summary>
    /// Returns the rank of the player scores compared to the other high scores - returns 0 if the score is not higher
    /// </summary>
    public int GetRank(long playerScore)
    {
        //If there are no high scores this should be at the top
        if (highScores.scoreList.Count == 0) return 1;

        int index = 0;

        //Loop to find the rank of this score
        for (int i = 0; i < highScores.scoreList.Count; i++)
        {
            index++;

            if (playerScore >= highScores.scoreList[i].playerScore)
            {
                return index;
            }
        }

        if (highScores.scoreList.Count < Settings.maxNumberOfHighScoresToSave)
            return index + 1;

        return 0;
    }

    public void SaveData(GameData data)
    {
        //data.highScores = this.gameData.highScores;
        data.highScores = this.highScores;

        //data.SaveHighScores(this.highScores);
    }

    public void LoadData(GameData data)
    {
        this.highScores = data.highScores;
    }
}
