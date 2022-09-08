using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    /// <summary>
    /// This coroutine is used for the materialize effect shader 
    /// </summary>
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime,
        SpriteRenderer[] spriteRendererArray, Material defaultMaterial)
    {
        Material materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);

        //Set the material for each sprite renderer
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
        }

        float dissolveAmount = 0f;

        //While the dissolve amount is less than one, but once it is greather than one, we set the default lit material again
        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;
            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }

        //Set again the default lit material for the sprites renderes
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
