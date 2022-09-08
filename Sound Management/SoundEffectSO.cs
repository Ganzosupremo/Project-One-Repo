using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SFX Sound")]
public class SoundEffectSO : ScriptableObject
{
    #region Header Sound Effect Details
    [Space(10)]
    [Header("Sound Effect Basic Details")]
    #endregion

    #region Tooltip
    [Tooltip("The name for the sound effect")]
    #endregion
    public string soundEffectName;

    #region Tooltip
    [Tooltip("The prefab for the sound effect")]
    #endregion
    public GameObject soundPrefab;

    #region Tooltip
    [Tooltip("The audio clip for the sound effect")]
    #endregion
    public AudioClip soundEffectClip;

    #region Tooltip
    [Tooltip("The minimum pitch variation for the sound effect.  A random pitch variation will be generated between the minimum and maximum values.  A random pitch variation makes sound effects sound more natural.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectMinRandomValuePitch = 0.8f;

    #region Tooltip
    [Tooltip("The maximum pitch variation for the sound effect.  A random pitch variation will be generated between the minimum and maximum values.  A random pitch variation makes sound effects sound more natural.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectMaxRandomValuePitch = 1.2f;

    #region Tooltip
    [Tooltip("The sound effect volume.")]
    #endregion
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectMinRandomValuePitch), soundEffectMinRandomValuePitch, nameof(soundEffectMaxRandomValuePitch), soundEffectMaxRandomValuePitch, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
    #endregion
}
