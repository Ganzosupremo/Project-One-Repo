using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightningControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        door = GetComponent<Door>();
    }

    /// <summary>
    /// The Door Fade In
    /// </summary>
    public void FadeInDoor(Door door)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            SpriteRenderer[] spriteRenderesArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRenderesArray)
            {
                StartCoroutine(FadeInDoorCoroutine(spriteRenderer, material));
            }
            isLit = true;
        }
    }

    /// <summary>
    /// Fade In Coroutine
    /// </summary>
    private IEnumerator FadeInDoorCoroutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);

            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FadeInDoor(door);
    }
}
