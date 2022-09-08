using System;
using UnityEngine;

public class AStarNode : IComparable<AStarNode>
{
    public Vector2Int gridPosition;
    public int GCost = 0; //distance from the starting node
    public int HCost = 0; //distance from the finish node
    public AStarNode parentNode;

    public AStarNode(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }

    public int FCost
    {
        get  
        {
            return GCost + HCost; 
        }
    }
    public int CompareTo(AStarNode nodeToCompare)
    {
        //Compare will be < 0 if this instance FCost is less than nodeToCompare.FCost
        //Compare will be > 0 if this instance FCost is greater than nodeToCompare.FCost
        //Compare will be == 0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }

        return compare;
    }
}
