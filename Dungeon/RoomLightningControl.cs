using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;

[RequireComponent(typeof(InstantiatedRoom))]
[DisallowMultipleComponent]
public class RoomLightningControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        //Suscribe to the on room change event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChange;
    }

    private void OnDisable()
    {
        //Unsuscribe to the on room change event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChange;
    }

    /// <summary>
    /// Handle the room change event
    /// </summary>
    private void StaticEventHandler_OnRoomChange(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            //Fade in the lightning on the rooms
            FadeInRoomLightning();

            instantiatedRoom.ActivateEnvironmentObjects();

            //Fade in the environment objects alongside the rooms
            FadeInEnvironmentLights();

            //Fade in the lightning also on the doors
            FadeInDoors();

            instantiatedRoom.room.isLit = true;
        }
    }

    /// <summary>
    /// Fade In The Room Lightning
    /// </summary>
    private void FadeInRoomLightning()
    {
        //Fade in the lightning on the room tilemaps
        StartCoroutine(FadeInRoomLightningCoroutine(instantiatedRoom));
    }

    private IEnumerator FadeInRoomLightningCoroutine(InstantiatedRoom instantiatedRoom)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundtilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decorations1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decorations2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);

            yield return null;
        }

        //Set the material back to the lit material
        instantiatedRoom.groundtilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decorations1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decorations2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    /// <summary>
    /// Fades in the environment decoration objects 
    /// </summary>
    private void FadeInEnvironmentLights()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        Environment[] environmentComponents = GetComponentsInChildren<Environment>();

        foreach (Environment environment in environmentComponents)
        {
            if (environment.spriteRenderer != null)
            {
                environment.spriteRenderer.material = material;
            }
        }

        StartCoroutine(FadeInEnvironmentRoutine(material, environmentComponents));
    }

    private IEnumerator FadeInEnvironmentRoutine(Material material, Environment[] environmentComponents)
    {
        //Gradually fade in the lighting
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        //Set the material to the lit default one
        foreach (Environment environment in environmentComponents)
        {
            if (environment.spriteRenderer != null)
            {
                environment.spriteRenderer.material = GameResources.Instance.litMaterial;
            }
        }
    }

    /// <summary>
    /// Fade In The Doors
    /// </summary>
    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightningControl doorLightningControl = door.GetComponentInChildren<DoorLightningControl>();

            doorLightningControl.FadeInDoor(door);
        }
    }
}
