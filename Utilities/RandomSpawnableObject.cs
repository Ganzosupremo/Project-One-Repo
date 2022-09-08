using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    public struct chanceBoundaries
    {
        public T spawnableObject;
        public int lowerBoundaryValue;
        public int highBoundaryValue;
    }

    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList)
    {
        this.spawnableObjectByLevelList = spawnableObjectByLevelList;
    }

    /// <summary>
    /// Returns an item and spawns it based on its ratio
    /// </summary>
    public T GetItem()
    {
        int upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (SpawnableObjectByLevel<T> spawnableObjectByLevel in spawnableObjectByLevelList)
        {
            if (spawnableObjectByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectByLevel.spawnableObjectRatioList)
                {
                    int lowerBoundary = upperBoundary + 1;

                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    ratioValueTotal += spawnableObjectRatio.ratio;

                    //Add spawnable object to the list
                    chanceBoundariesList.Add(new chanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.dungeonObject,
                        lowerBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        if (chanceBoundariesList.Count == 0) return default(T);

        int lookUpValue = Random.Range(0, ratioValueTotal);

        //Loop through the list to get a random spawnableObject details
        foreach (chanceBoundaries spawnChance in chanceBoundariesList)
        {
            if (lookUpValue >= spawnChance.lowerBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}
