using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string codeWord = "Bitcoin4Life";
    private readonly string backupExtension = ".bak";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID, bool allowRestoreFromBackup = true)
    {
        //If the profile ID is null, return
        if (profileID == null)
        {
            return null;
        }

        //Use the Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                //Load the serialized data from the Json file
                string dataToLoad = "";
                using (FileStream stream = new(fullPath,FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //Optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                //Deserealized the Json file back to a C# class
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch (Exception e)
            {
                //We only try to load from backup a single time, otherwise it could end in an infinite loop
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning($"Failed to load file. Attempting to roll back. \n {e}");
                    bool rollBackSucceded = AttemptRollBack(fullPath);

                    if (rollBackSucceded)
                    {
                        //Try to load data again
                        loadedData = Load(profileID, false);
                    }
                }
                else
                {
                    Debug.LogError($"Error ocurred when trying to load file from: {fullPath} and backup didn't work. \n {e}");
                }
            }
        }
        return loadedData;
    }

    public void Save(GameData gameData, string profileID)
    {
        //If the profile ID is null, return
        if (profileID == null)
        {
            return;
        }

        //Use the Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        string backupPath = fullPath + backupExtension;

        try
        {
            //Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize the C# game data into a Json
            string dataToStore = JsonUtility.ToJson(gameData, true);

            //Optionally use encryption on the Json file
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            //Write the serialized data to the Json file
            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            //Verify the newly created data isn't corrupted
            GameData verifiedGameData = Load(profileID);

            //If the data can be verified back it up
            if (verifiedGameData != null)
            {
                File.Copy(fullPath,backupPath, true);
            }
            //otherwise, something went wrong, so throw an exception
            else
            {
                throw new Exception("Data couldn't be verified and backep up");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error while saving the file: " + fullPath + "\n" + e);
        }
    }

    /// <summary>
    /// Loads all save data and shows it in the game UI
    /// </summary>
    /// <returns>Returns all the loaded saves in a Dictionary</returns>
    public Dictionary<string,GameData> LoadAllSaveProfiles()
    {
        Dictionary<string,GameData> profileDictionary = new Dictionary<string,GameData>();

        //Loop over all directory names in the data directory path of the PC
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;

            //Check if the data path exist
            //If it doesn't, then this folder isn't a save file and should be skipped
            string fullPath = Path.Combine(dataDirPath,profileID, dataFileName);
            
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"Skipping this directory, because is not a save file: {profileID}");
                continue;
            }

            //Load this save data for this profile and put it in the dictionary
            GameData profileData = Load(profileID);

            //Ensure the game data isn't null
            //if it is null, log an error so we let ourselves know that something went wrong
            if (profileData != null)
            {
                profileDictionary.Add(profileID, profileData);
            }
            else
            {
                Debug.LogError($"Tried to load data but something went wrong. ProfileID: {profileID}");
            }

        }

        return profileDictionary;
    }

    /// <summary>
    /// Get the most recent profile ID that was saved 
    /// </summary>
    /// <returns>Returns the most recent profile ID</returns>
    public string GetMostRecentProfileID()
    {
        string mostRecentProfileID = null;

        Dictionary<string, GameData> profilesGameData = LoadAllSaveProfiles();

        foreach (KeyValuePair<string, GameData> keyValue in profilesGameData)
        {
            string profileID = keyValue.Key;
            GameData profileData = keyValue.Value;

            //Skip if null
            if (profileData == null)
            {
                continue;
            }

            //If it's the first data we come across
            if (mostRecentProfileID == null)
            {
                mostRecentProfileID = profileID;
            }
            //otherwise, compare to see which date is the most recent one
            else
            {
                DateTime mostRecentTime = DateTime.FromBinary(profilesGameData[mostRecentProfileID].lastUpdated);
                DateTime newTime = DateTime.FromBinary(profileData.lastUpdated);

                if (newTime > mostRecentTime)
                {
                    mostRecentProfileID = profileID;
                }
            }
        }
        return mostRecentProfileID;
    }


    /// <summary>
    /// This method encrypts the data with XOR encryption 
    /// </summary>
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }

    /// <summary>
    /// Attemps to roll back from the backup file, if the standard file becomes corrupted
    /// </summary>
    /// <param name="fullPath">The full path where the file is stored</param>
    /// <returns>Returns true if the roll back succeded, false otherwise</returns>
    private bool AttemptRollBack(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;

        try
        {
            //If the backup exist roll back from it
            if (File.Exists(backupFilePath))
            {
                File.Copy(fullPath, backupFilePath, true);
                success = true;
                Debug.Log("Had to roll back from backup file");
            }
            //Otherwise, we don't yet have a backup file, so there's nothing to load from
            else
            {
                throw new Exception("No backup file to roll back to.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error ocurred when trying to roll back from the backup file: {backupFilePath}, \n {e}");
        }

        return success;
    }
}
