using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MaterializeEffect))]
public class ChestItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TextMeshPro textMesh;
    private MaterializeEffect materializeEffect;

    [HideInInspector] public bool isItemMaterialized = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textMesh = GetComponentInChildren<TextMeshPro>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    /// <summary>
    /// Initialize the item for the chest to spawn
    /// </summary>
    public void Initialize(Sprite sprite, string text, Vector3 spawnPos, Color materializeColor)
    {
        spriteRenderer.sprite = sprite;
        transform.position = spawnPos;

        StartCoroutine(MaterializeItem(materializeColor, text));
    }

    private IEnumerator MaterializeItem(Color materializeColor, string text)
    {
        SpriteRenderer[] spriteRenderers = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader,
            materializeColor, 1.5f, spriteRenderers, GameResources.Instance.litMaterial));

        isItemMaterialized = true;

        textMesh.text = text;
    }
}
