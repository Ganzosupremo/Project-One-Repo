using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class ConfirmationPopupMenu : MonoBehaviour
{
    [SerializeField] private Button firstSelectedButton;

    #region Header
    [Header("Components")]
    #endregion
    [SerializeField] private TextMeshProUGUI confirmationText;

    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        firstSelectedButton.Select();
    }

    public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
    {
        this.gameObject.SetActive(true);

        this.confirmationText.text = displayText;

        //Remove listener to make there are no previous listeners
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        //Assign the onClick listeners
        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });

        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });
    }

    private void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}
