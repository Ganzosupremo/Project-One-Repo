using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Builds a path for the room, from the startGridPosition till the endGridPosition, and adds
    /// movement steps to the returned stack, returns null if no path has been found
    /// </summary>
    /// <param name="room"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        //Adjust positions by the lower bounds
        startGridPosition -= (Vector3Int)room.tilemapLowerBounds;
        endGridPosition -= (Vector3Int)room.tilemapLowerBounds;

        //Create the open node list and the closed node hashset
        List<AStarNode> openNodeList = new List<AStarNode>();
        HashSet<AStarNode> closedNodeHashset = new HashSet<AStarNode>();

        //Create the grid nodes for the path finding
        GridNodes gridNodes = new GridNodes(room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1,
            room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1);

        AStarNode startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        AStarNode endNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        AStarNode endPathNode = FindShortestPath(startNode, endNode, gridNodes, openNodeList, closedNodeHashset, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    /// <summary>
    /// Finds the shortest path to the target - returns the end node if a path has been found, else returns null
    /// </summary>
    private static AStarNode FindShortestPath(AStarNode startNode, AStarNode endNode, GridNodes gridNodes, List<AStarNode> openNodeList, HashSet<AStarNode> closedNodeHashset, InstantiatedRoom instantiatedRoom)
    {
        //Add start node to the open list
        openNodeList.Add(startNode);

        //Loop until the node list is empty
        while (openNodeList.Count > 0)
        {
            //Sort the list
            openNodeList.Sort();

            //Current node = the node with the lowest FCost
            AStarNode currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            //If the currentNode == the end node, then finish
            if (currentNode == endNode)
                return currentNode;

            //Add currentNode to the closed node list
            closedNodeHashset.Add(currentNode);

            //Evaluate the cost of all neighbour nodes of the current node
            EvaluateCurrentNodeNeighbours(currentNode, endNode, gridNodes, openNodeList, closedNodeHashset, instantiatedRoom);
        }

        return null;
    }

    /// <summary>
    /// Creates a Stack<Vector3> containing the movement path
    /// </summary>
    private static Stack<Vector3> CreatePathStack(AStarNode targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        AStarNode nextNode = targetNode;

        //Get the mid point of the cell
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            //Convert grid position to world position
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.tilemapLowerBounds.x,
                nextNode.gridPosition.y + room.tilemapLowerBounds.y, 0));

            //Add the world position to the middle of the grid cell
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// <summary>
    /// Evaluates the neighbour nodes of the current node
    /// </summary>
    private static void EvaluateCurrentNodeNeighbours(AStarNode currentNode, AStarNode endNode, GridNodes gridNodes, List<AStarNode> openNodeList, HashSet<AStarNode> closedNodeHashset, InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        AStarNode validNeighbourNode;

        //Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNeighbourNode(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, 
                    gridNodes, closedNodeHashset, instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    //Calculate a new GCost for the neighbour node
                    int newCostForNeighbour;

                    //Get the movement penalty
                    //Non- walkable paths have a penalty of 0, Default penalty is set in the Settings Script
                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x,
                        validNeighbourNode.gridPosition.y];

                    newCostForNeighbour = currentNode.GCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostForNeighbour < validNeighbourNode.GCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.GCost = newCostForNeighbour;
                        validNeighbourNode.HCost = GetDistance(validNeighbourNode, endNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                            openNodeList.Add(validNeighbourNode);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the distance btw nodeA and nodeB, returns it as an int
    /// </summary>
    private static int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int distanceY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY); //10 used instead of 1, and 14 is the aprox of the pythagoras SQRT(10*10 + 10*10)
                                                                  //to avoid the use of floats
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    /// <summary>
    /// Evaluates a neighbour node at neighbourNodeXPos, neighbourNodeYPos using the specified gridNodes,
    /// closedNodeHashset and the instantiatedRoom, returns null if the node is invalid
    /// </summary>
    private static AStarNode GetValidNeighbourNode(int neighbourNodeXPos, int neighbourNodeYPos, GridNodes gridNodes, HashSet<AStarNode> closedNodeHashset, InstantiatedRoom instantiatedRoom)
    {
        //If the neighbour node is beyond the grid, return null
        if (neighbourNodeXPos >= instantiatedRoom.room.tilemapUpperBounds.x - instantiatedRoom.room.tilemapLowerBounds.x || neighbourNodeXPos < 0
            || neighbourNodeYPos >= instantiatedRoom.room.tilemapUpperBounds.y - instantiatedRoom.room.tilemapLowerBounds.y || neighbourNodeYPos < 0)
        {
            return null;
        }

        //Get neighbour node
        AStarNode neighbourNode = gridNodes.GetGridNode(neighbourNodeXPos, neighbourNodeYPos);

        //Check for obstacles at that position
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPos, neighbourNodeYPos];

        int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPos, neighbourNodeYPos];

        //If the neighbour node is an obstacle or is already in the closed list, the skip it
        if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 || closedNodeHashset.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
}
