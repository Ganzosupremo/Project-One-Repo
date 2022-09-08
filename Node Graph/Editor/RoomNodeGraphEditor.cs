using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Collections.Generic;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    //The Values of the Node Layout
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    //The Values For The Connecting Line
    private const float connectingLineWidth = 4f;
    private const float connectingLineArrowSize = 8f;

    //Grid Spacing
    private const float gridLarge = 200f;
    private const float gridSmall = 50f;

    [MenuItem("Room's Graph Node Editor", menuItem = "Window/Dungeon Editor/Room's Graph Node Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room's Graph Node Editor");
    }
    private void OnEnable()
    {
        //Suscribe to the Inspector Selection Changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        //Define The Style Of The Node Layout
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node5") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Changes the node room Style when a room node is selectd
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node5 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Load Room Node Types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        //Unsubscribe from the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// Open The Room Node Graph Editor Window If A Room Node Graph SO Asset Is Double Clicked In The Unity Inspector
    /// </summary>
    [OnOpenAsset(0)] //Needs The UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    ///<summary>
    ///Draw the Editor GUI
    ///</summary>
    private void OnGUI()
    {
        //If A Scriptable Object Of Type RoomNodeGraphSO Has Been Selected Then Process
        if (currentRoomNodeGraph != null)
        {
            //Draw Grid
            DrawBackgroungGrid(gridSmall, 0.4f, Color.gray);
            DrawBackgroungGrid(gridLarge, 0.7f, Color.gray);


            //Draw A Line If Being Dragged
            DrawDraggedLine();

            //Process Events
            ProcessEvents(Event.current);

            //Draws And Keeps The Connection Btw Nodes
            DrawRoomNodeConnections();

            //Draw Room Nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }
    /// <summary>
    /// Draw A Background Grid For The Room Node Graph Inpector Window
    /// </summary>
    /// <param name="gridSize"></param>
    /// <param name="gridOpacity"></param>
    /// <param name="gridColor"></param>
    private void DrawBackgroungGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0f) + gridOffset,
                new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0f) + gridOffset,
                new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            //Draw A Line From A Node To Line Position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.black, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        //Reset Graph Drag
        graphDrag = Vector2.zero;

        //If the mouse is over a room node is null or if the left click is not dragging a room node
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverARoomNode(currentEvent);
        }

        //Process Events, if the mouse is not over a node or we are currentlu draging a line from a node to another
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        //Else process the room node events
        else
        {
            //process room node events
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }
    /// <summary>
    /// Check To See If The Mouse Is Over A Room Node - If So Then, Return The Room Node, Else Returns Null 
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private RoomNodeSO IsMouseOverARoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1 ; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Process The Room Node Graph Events
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //Process The Mouse Down Events, The Right Click
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            //Process The Mouse Up Events, When The Mouse Is Resealed
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Process The Mouse Down Events On The Room Node Graph Itself (Not Over A Node)
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //Process The Rigth Click Event On The Graph Event (Show The Context Menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }

    }

    /// <summary>
    /// Shows The Context Menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete 'Em All (Selected Nodes)"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    /// <summary>
    /// Creates A Room Node On The Mouse Position
    /// </summary>
    /// <param name="mousePositionObject"></param>
    private void CreateRoomNode(object mousePositionObject)
    {
        //If the current node Editor is empty, then add a rooom node of type entrance
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// Create A Room Node At The Mouse - Oveloaded To Also Pass In RoomNodeType
    /// </summary>
    /// <param name="mousePositionObject"></param>
    /// <param name="roomNodeTypeSO"></param>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        //Create Room Node SO Asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        //Add Room NodeTo The Current Room Node Graph Room Node List
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        //Set Room Node Values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        //Add Room Node To Room Node Graph SO Asset Database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        //Makes sure the graph keeps updated
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// Deletes The Currently Selected Room Nodes
    /// </summary>
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        //Loop through all nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                //Iterate through all child room IDs
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    //Retrieve child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        //Remove parentID from the child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                //Iterate through all parent room IDs
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    //Retrieve parent room node
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null)
                    {
                        //Remove ChildID from the parent room node
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        //Delete Queued room nodes
        while (roomNodeDeletionQueue.Count > 0)
        {
            //Retrive room node from the queue
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            //Remove node from the dictionary
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            //Remove node from the node list
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            //Remove room node from the database
            DestroyImmediate(roomNodeToDelete, true);

            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// Delete the Links Btw The Selected Room Nodes
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        //Iterate through all nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    //Get child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    //If the child room node is selected
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        //Remove childID from parent room node
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        //Remove parentID from child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        //Clear all selected room nodes
        ClearAllSelectedRoomNodes();
    }

    /// <summary>
    /// Clear selection from all nodes
    /// </summary>
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Allows To Select All Room Nodes
    /// </summary>
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Process Mouse Up Events
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //If releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)  
        {
            //Check if the mouse is over a room node
            RoomNodeSO roomNode = IsMouseOverARoomNode(currentEvent);

            if (roomNode != null)
            {
                //If so, set it as a child of the parent room node, if it can be added
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    //Set parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    /// <summary>
    /// Process The Mouse Drag Event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //Process right click drag event - draw line btw nodes
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        //Process left click drag event - drag the node graph itself 
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// <summary>
    /// Process The Right Click Drag Event - Draws A Line
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectionLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// <summary>
    /// Process Left Mouse Button Drag Event - Drags The Graph
    /// </summary>
    /// <param name="dragDelta"></param>
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Draws And Drags A Connection Line From A Room Node To The Mouse Position
    /// </summary>
    /// <param name="delta"></param>
    public void DragConnectionLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// Clears A Room Node Line From The Room Node Graph Editor Window
    /// </summary>
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// Draw And Keeps Connections Btw Room Nodes In The Editor Window
    /// </summary>
    private void DrawRoomNodeConnections()
    {
        //Loop through all room nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                //Loop trough child room nodes
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    //Get child room ID from the dictionary
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws A Line Btw Connections Of Room Nodes - Parent And Child Node
    /// </summary>
    /// <param name="roomNode"></param>
    /// <param name="roomNodeSO"></param>
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        //Get the start and end line position
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        //Calculate Mid Position
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        //Vector from start to end position of the line
        Vector2 direction = endPosition - startPosition;

        //Calculate normalised perpendicular position from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        //Calculate arrow head mid position offset
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        //Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.cyan, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.cyan, null, connectingLineWidth);

        //Draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.green, null, connectingLineWidth);
        GUI.changed = true;
    }

    /// <summary>
    /// Draw Room Nodes In The Graph Window
    /// </summary>
    private void DrawRoomNodes()
    {
        //Loop Through All Room Nodes And Draws Them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
