using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel_", menuName = "Scriptable Objects/Dungeon/Game Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header Basic Level Details
    [Space(10)]
    [Header("Basic Level Details")]
    #endregion Header Basic Level Details

    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion Tooltip
    public string levelName;

    #region Header Room Templates For The Level
    [Space(10)]
    [Header("The Room Templates For The Level Generation")]
    #endregion Header Room Templates For The Level

    #region Tooltip
    [Tooltip("Populate the list with the room templates that you want to be part of the level. You need to make sure that " +
        "room templates are included for all the room node types specified in the Room Node Graph.")]
    #endregion Tooltip
    public List<RoomTemplateSO> roomTemplatesList;

    #region Header Room Node Graphs For The Level
    [Space(10)]
    [Header("The Room Node Graph For The Level")]
    #endregion Header Room Templates For The Level

    #region Tooltip
    [Tooltip("Populate this list with all the room node graph that you want to be picked randomly from.")]
    #endregion Tooltip
    public List<RoomNodeGraphSO> roomNodeGraphsList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplatesList), roomTemplatesList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphsList), roomNodeGraphsList))
            return;

        //Check to make sure that room templates are specified for all room node types in the specified graphs


        //First check that NS corridor, EW corridors and entrances have been specified
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach (RoomTemplateSO roomTemplateSO in roomTemplatesList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No EW Corridor Room Type Specified.");
        }

        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No NS Corridor Room Type Specified.");
        }

        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Corridor Room Type Specified.");
        }

        //Loop through all node graph
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphsList)
        {
            if (roomNodeGraph == null)
                return;

            //Loop through all room nodes in the graph
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                //Check that a room template has been specified for each roomNode Type
                //Corridors and Entrances already check
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS ||
                    roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;
                
                //Loop through all room templates to check that this node type has been specified
                foreach (RoomTemplateSO roomTemplateSO in roomTemplatesList)
                {
                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + ": No room template for " + roomNodeSO.roomNodeType.name.ToString()
                        + " Found for the node graph " + roomNodeGraph.name.ToString() + ".");
                }
            }
        }

    }
#endif
    #endregion
}
