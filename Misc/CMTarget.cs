using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CMTarget : MonoBehaviour
{
    private CinemachineTargetGroup cmTargetGroup;
    #region Tooltip
    [Tooltip("Populate with the gameobject called MouseTarget")]
    #endregion
    [SerializeField] private Transform cursorTarget; 


    private void Awake()
    {
        cmTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }

    /// <summary>
    /// Set The Cinemachine Camera Target Group
    /// </summary>
    private void SetCinemachineTargetGroup()
    {
        //Creates a target group so the cinemachine camera can follow it, this follows the player and the mouse cursor
        CinemachineTargetGroup.Target cmTargetGroup_Player = new CinemachineTargetGroup.Target
        {
            weight = 1.6f,
            radius = 2f,
            target = GameManager.Instance.GetPlayer().transform
        };

        CinemachineTargetGroup.Target cmTargetGroup_Cursor = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 1f,
            target = cursorTarget
        };

        CinemachineTargetGroup.Target[] cmTargetArray = new CinemachineTargetGroup.Target[] { cmTargetGroup_Player, cmTargetGroup_Cursor };

        cmTargetGroup.m_Targets = cmTargetArray;
    }
}
