using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button returnButton;

    #region Header
    [Header("Confirmation Popup Menu")]
    #endregion
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private SaveSlot[] saveSlots;
    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
        SetFirstSelectedButton(firstSelectedButton);
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        //Disable the save slot buttons
        DisableSlotButtons();

        //case - load game
        if (isLoadingGame)
        {
            PersistenceDataManager.Instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            SaveAndStartGame();
        }
        //case - new game, but already contains data
        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu
                ("Are u sure you want to override this data?",
                //Function to execute is we select 'confirm'
                () =>
                {
                    PersistenceDataManager.Instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
                    PersistenceDataManager.Instance.NewGame();
                    SaveAndStartGame();
                },
                //Function to execute id we select 'cancel'
                () =>
                {
                    this.ActivateMenu(isLoadingGame);
                }
                );
        }
        //case - new game, and the save slot has no data
        else
        {
            PersistenceDataManager.Instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            PersistenceDataManager.Instance.NewGame();
            SaveAndStartGame();
        }
        
    }

    private void SaveAndStartGame()
    {
        //Save the game anytime before loading a scene
        PersistenceDataManager.Instance.SaveGame();

        //Load the scene, which will save the game, because of the OnSceneUnloaded in the persistence manager
        MainMenuUI.Instance.StartGame();
    }

    /// <summary>
    /// Loads all the profiles that exist
    /// </summary>
    public void ActivateMenu(bool isLoadingGame)
    {
        //Set the mode
        this.isLoadingGame = isLoadingGame;

        //Load all the existing profiles
        Dictionary<string, GameData> profilesGameData = PersistenceDataManager.Instance.GetAllProfileGameData();

        returnButton.interactable = true;

        //Loop through each save slot in the Ui and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    private void DisableSlotButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        returnButton.interactable = false;
    }

    /// <summary>
    /// Selects the first button
    /// </summary>
    /// <param name="firstSelected">The button that will be selected first</param>
    private void SetFirstSelectedButton(Button firstSelected)
    {
        firstSelected.Select();
    }
}
