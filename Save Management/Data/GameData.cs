using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    //Save the current bitcoin sats the player has to the disk
    public double satsOnHold;

    //Save the score of the player to the disk
    public HighScores highScores;

    /// <summary>
    /// The data defined here are the default values
    /// when the game starts and there's no data to load
    /// </summary>
    public GameData()
    {
        satsOnHold = 0;
        //highScores = new();
    }

    /// <summary>
    /// Gets the amount of sats that have been collected
    /// </summary>
    /// <returns>Returns the amount of sats collected and saved</returns>
    public double GetSatsOnHold()
    {
        return satsOnHold;
    }
}

