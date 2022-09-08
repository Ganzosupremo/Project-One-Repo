using UnityEngine;


[DisallowMultipleComponent]
public class LightTorch : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D light2D;
    private float lightTorchTimer;
    [SerializeField] private float minLightIntensity;
    [SerializeField] private float maxLightIntensity;

    [SerializeField] private float lightTorchMinTime;
    [SerializeField] private float lightTorchMaxTime;

    private void Awake()
    {
        light2D = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
    }

    private void Start()
    {
        lightTorchTimer = Random.Range(lightTorchMinTime, lightTorchMaxTime);
    }

    private void Update()
    {
        if (light2D == null) return;

        lightTorchTimer -= Time.deltaTime;

        if (lightTorchTimer < 0f)
        {
            lightTorchTimer = Random.Range(lightTorchMinTime, lightTorchMaxTime);

            RandomiseLightIntensity();
        }
    }

    private void RandomiseLightIntensity()
    {
        light2D.intensity = Random.Range(minLightIntensity, maxLightIntensity);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minLightIntensity), minLightIntensity, nameof(maxLightIntensity), maxLightIntensity, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightTorchMinTime), lightTorchMinTime, nameof(lightTorchMaxTime), lightTorchMaxTime, false);
    }
#endif
    #endregion
}
