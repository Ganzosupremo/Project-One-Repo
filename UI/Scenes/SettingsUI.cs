using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicLevelText;

    [SerializeField] private TextMeshProUGUI soundsLevelText;

    private void OnEnable()
    {
        StartCoroutine(InitializeUI());
    }
    public void ReturnMainMenu()
    {
        SceneManager.UnloadSceneAsync("Settings");

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator InitializeUI()
    {
        yield return null;

        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
        soundsLevelText.SetText(SoundManager.Instance.soundsVolume.ToString());
    }

    /// <summary>
    /// Increase the volume of the music - the volume can be changed with a button in the pause menu UI
    /// </summary>
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// <summary>
    /// Decrease the volume of the music - the volume can be changed with a button in the pause menu UI
    /// </summary>
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// <summary>
    /// Increase the volume of the sound effects - the volume can be changed with a button in the pause menu UI
    /// </summary>
    public void IncreaseSoundsVolume()
    {
        SoundManager.Instance.IncreaseMusicVolume();
        soundsLevelText.SetText(SoundManager.Instance.soundsVolume.ToString());
    }

    /// <summary>
    /// Decrease the volume of the sound effects - the volume can be changed with a button in the pause menu UI
    /// </summary>
    public void DecreaseSoundsVolume()
    {
        SoundManager.Instance.DecreaseMusicVolume();
        soundsLevelText.SetText(SoundManager.Instance.soundsVolume.ToString());
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsLevelText), soundsLevelText);
    }
#endif
    #endregion
}
