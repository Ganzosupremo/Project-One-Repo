using UnityEngine;

public class GridNodes
{
    private int gridWidth;
    private int gridHeight;

    private AStarNode[,] gridNode;

    public GridNodes(int gridWidth, int gridHeight)
    {
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        gridNode = new AStarNode[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridNode[x,y] = new AStarNode(new Vector2Int(x, y));
            }
        }
    }

    public AStarNode GetGridNode(int xPosition,int yPosition)
    {
        if (xPosition < gridWidth && yPosition < gridHeight)
        {
            return gridNode[xPosition,yPosition];
        }
        else
        {
            Debug.Log("Requested Grid Node is out of range");
            return null;
        }
    }
}
