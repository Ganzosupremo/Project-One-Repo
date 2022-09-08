using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new();
    [HideInInspector] public List<string> childRoomNodeIDList = new();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code
    //The Following Code Should Only Run In The Unity Editor
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false; 
    [HideInInspector] public bool isSelected = false;

    /// <summary>
    /// Intialise The Node
    /// </summary>
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        //Load Room Node Type List
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    /// <summary>
    /// Draw Node With The Nodestyle
    /// </summary>
    /// <param name="nodeStyle"></param>
    public void Draw(GUIStyle nodeStyle)
    {
        //Draw The Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        //Start The Region To Detect PopUp Selection Changes
        EditorGUI.BeginChangeCheck();

        //If the room node has a parent or is of type entrance, if so, display a fixed label, else display the normal popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            //Display A Popup Using The RoomNodeType Name Values That Can Be Selected From (Default to the Currently set RoomNodeType)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            //If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor
                && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                //If a room node type has been changed and it already has a children, then delete the parent - child links, since we need to revalidate the links
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        //Get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        //If the child room node is not null
                        if (childRoomNode != null)
                        {
                            //Remove childID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            //Remove parentID from child room node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }
    /// <summary>
    /// Populate A String Array With The Room Node Types That Can Be Displayed And Selected
    /// </summary>
    /// <returns></returns>
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }
    /// <summary>
    /// Process Events For The Room Nodes
    /// </summary>
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //Process Mouse Button Down Events - When The Mouse Button Is Pressed
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
                //Process Mouse Up Event - When The Mouse Button Is Released
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
                //Process Mouse Drag Events - When The Mouse Button Is Being HODL
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }
    /// <summary>
    /// Process Mouse Down Events
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //Left Click Down
        if (currentEvent.button == 0)
        {
            ProcessLeftClikDownEvent();
        }
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }
    /// <summary>
    /// Process Left Click Events
    /// </summary>
    private void ProcessLeftClikDownEvent()
    {
        Selection.activeObject = this;

        //Toggle Node Selection
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

/// <summary>
/// Process The Right Click Event To Draw A Line Btw Room Nodes
/// </summary>
/// <param name="currentEvent"></param>
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    /// <summary>
    /// Process Mouse Up Event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //Left Click Up
        if (currentEvent.button == 0)
        {
            ProcessLeftClikUpEvent();
        }
    }

    /// <summary>
    /// Process Left Click Up Event
    /// </summary>
    private void ProcessLeftClikUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    /// <summary>
    /// Process The Mouse Drag Event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //Process left click drag event
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

/// <summary>
/// Process The Left Mouse Drag Event
/// </summary>
/// <param name="currentEvent"></param>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// <summary>
    /// Allows To Drag The Room Node
    /// </summary>
    /// <param name="delta"></param>
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Adds The ChildID To The Node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        //Check if the child node can be added validly to a parent node
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the child node can be validly added to the parent node - if so, return true, false otherwise
    /// </summary>
    public bool IsChildRoomValid(string childID)
    {
        bool isBossRoomAlreadyConnected = false;
        bool isShopAlreadyConnected = false;

        //Check if there's already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isBossRoomAlreadyConnected = true;
        }

        //Check if the shop is already connencted in the node graph
        foreach (RoomNodeSO shopNode in roomNodeGraph.roomNodeList)
        {
            if (shopNode.roomNodeType.isShop && shopNode.parentRoomNodeIDList.Count > 0)
                isShopAlreadyConnected = true;
        }

        //If there's already a boss room and it's connected, then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isBossRoomAlreadyConnected)
            return false;

        //If there's already a shop room and it's connected, then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isShop && isShopAlreadyConnected)
            return false;
        
        //If the child room node has a type of none node, the returns false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        //If the parent node already has a child with the same ID, then returns false
        if (childRoomNodeIDList.Contains(childID))
            return false;

        //If this node ID and the child node ID are the same, then returns false
        if (id == childID)
            return false;

        //If this Child ID is already in the ParentID list, then returns false
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        //If the child node already has a parent, the returns false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        //If this node is a Corridor and the child is also a Corridor, then returns false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        //If this node is not a Corridor and the child is also not a Corridor, then returns false
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        //When adding a new Corridor, check if this node has less than the max allowed child Corridors
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        //If the child room is an Entrance, then returns false - the Entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        //When adding a room to a corridor, check if this corridor doesn't already has a room added
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    /// <summary>
    /// Adds The ParentID To The Node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// <summary>
    /// Remove childID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveChildRoomNodeIDFromRoomNode(string parentID)
    {
        //If the node contains the childId, proceed to remove it
        if (childRoomNodeIDList.Contains(parentID))
        {
            childRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Remove the parentID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        //If the node contains the childId, proceed to remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }
#endif
    #endregion Editor Code
}
